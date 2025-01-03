using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameClient;
using System.Text.RegularExpressions;
using ActivityLimitTime;

namespace MobileBind
{
    public class MobileBindStateText
    {
        public const string UNENABLE_RECEIVE = "无法领取";
        public const string TO_RECEIVE = "领取";
        public const string RECEIVED = "已领取";
        public const string SEND_VERIFICATION_CODE = "发送验证码";
        public const string PUSH_VERIFY = "提交";

        public const string PhoneNumIsEmpty = "手机号为空，请检查";
        public const string PhoneNumNotEleven = "手机号位数不等于11位，请检查";
        public const string PhoneNumOrVerifyEmpty = "手机号或验证码为空，请检查";
    }

    public class MobilePhoneNumVerifyText
    {
        public const string CD = "手机号验证中";
        public const string DBError = "手机号验证，数据库错误";
        public const string InvalidAccount = "手机号验证，帐号无效";
        public const string RepeatBind = "手机号验证，重复绑定";
        public const string InvalidVerifyCode = "手机号验证，验证码无效";
        public const string InvalidServerId = "手机号验证，服务器ID错误";
        public const string NoVerifyCode = "手机号验证，无验证码";
        public const string ServerError = "手机号验证，服务器错误";
        public const string PlayerOffine = "角色离线了";
        public const string InvalidPhoneNum = "手机号错误，请检查";
    }

    public class MobileBindFrame : ClientFrame
    {
#region UI View
        public enum ReceiveBtnState
        {
            UnReceive,
            ToReceive,
            Received
        }

        public enum SendVerifyBtnState
        {
            UnEnable,
            Normal,
            Sending
        }

        public enum BindPhoneMode
        {
            Normal,     //正常绑定流程
            OpenSDK,    //调用SDK绑定流程
            Finished,   //完成绑定流程
        }

        private GameObject awardParent;
        private Button receiveBtn;
        private UIGray receiveBtnGray;
        private Text receiveBtnText;

        //bind phone modes
        private Button sdkGotoBindBtn;
        private UIGray sdkGotoBindBtnGray;
        private GameObject normalBindGo;

        private InputField mobileNumInput;
        private InputField mobileVerifyInput;
        private Button sendVerifyBtn;
        private Text sendVerifyBtnText;
        private UIGray sendVerifyGray;
        private Button pushVerifyBtn;
        private GameObject[] awardItems;

        private Button closeBtn;

        private ReceiveBtnState receiveBtnState;
        private SendVerifyBtnState sendVerifyBtnState;

        private float coolDownNum;
        private bool isCoolDown;

#endregion
        //frame  + data
        private ActivityLimitTimeData currData;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MobileBind/MobileBindFrame";
        }

        protected override void _bindExUI()
        {
            awardParent = mBind.GetGameObject("AwardParent");
            if (awardParent)
            {
                int count = awardParent.transform.childCount;
                awardItems = new GameObject[count];
                for (int i = 0; i < count; i++)
                {
                    awardItems[i] = awardParent.transform.GetChild(i).gameObject;
                }
            }
            receiveBtn = mBind.GetCom<Button>("ReceiveBtn");
            if (receiveBtn)
            {
                receiveBtn.onClick.RemoveListener(OnReceiveBtnClick);
                receiveBtn.onClick.AddListener(OnReceiveBtnClick);
            }
            receiveBtnText = mBind.GetCom<Text>("ReceiveBtnText");
            receiveBtnGray = mBind.GetCom<UIGray>("ReceiveBtnGray");

            sdkGotoBindBtn = mBind.GetCom<Button>("SDKGotoBind");
            if(sdkGotoBindBtn)
            {
                sdkGotoBindBtn.onClick.RemoveListener(OnGoToBindPhoneInSDK);
                sdkGotoBindBtn.onClick.AddListener(OnGoToBindPhoneInSDK);
                sdkGotoBindBtnGray = sdkGotoBindBtn.gameObject.GetComponent<UIGray>();
            }
            normalBindGo = mBind.GetGameObject("NormalBindGo");

            mobileNumInput = mBind.GetCom<InputField>("MobileNumInput");
            if (mobileNumInput)
            {
                mobileNumInput.onEndEdit.RemoveListener(OnInputPhoneNumEnd);
                mobileNumInput.onEndEdit.AddListener(OnInputPhoneNumEnd);
            }
            mobileVerifyInput = mBind.GetCom<InputField>("MobileVerifyInput");
            sendVerifyBtn = mBind.GetCom<Button>("SendMobileVerifyBtn");
            if (sendVerifyBtn)
            {
                sendVerifyBtn.onClick.RemoveListener(OnSendVerifyBtnClick);
                sendVerifyBtn.onClick.AddListener(OnSendVerifyBtnClick);
            }
            sendVerifyBtnText = mBind.GetCom<Text>("SendMobileVerifyBtnText");
            sendVerifyGray = mBind.GetCom<UIGray>("VerifyBtnGray");
            pushVerifyBtn = mBind.GetCom<Button>("PushBtn");
            if (pushVerifyBtn)
            {
                pushVerifyBtn.onClick.RemoveListener(OnPushVerifyBtnClick);
                pushVerifyBtn.onClick.AddListener(OnPushVerifyBtnClick);
            }

            closeBtn = mBind.GetCom<Button>("closeBtn");
            if (closeBtn)
            {
                closeBtn.onClick.RemoveListener(OnCloseFrameBtnClick);
                closeBtn.onClick.AddListener(OnCloseFrameBtnClick);
            }
        }

        protected override void _unbindExUI()
        {
            awardParent = null;
            if (receiveBtn)
            {
                receiveBtn.onClick.RemoveListener(OnReceiveBtnClick);
            }
            receiveBtn = null;
            receiveBtnGray = null;
            receiveBtnText = null;

            if (sdkGotoBindBtn)
            {
                sdkGotoBindBtn.onClick.RemoveListener(OnGoToBindPhoneInSDK);
            }
            sdkGotoBindBtn = null;
            sdkGotoBindBtnGray = null;

            normalBindGo = null;

            if (mobileNumInput)
            {
                mobileNumInput.onEndEdit.RemoveListener(OnInputPhoneNumEnd);
            }
            mobileNumInput = null;
            mobileVerifyInput = null;
            if (sendVerifyBtn)
            {
                sendVerifyBtn.onClick.RemoveListener(OnSendVerifyBtnClick);
            }
            sendVerifyBtn = null;
            sendVerifyBtnText = null;
            sendVerifyGray = null;
            if (pushVerifyBtn)
            {
                pushVerifyBtn.onClick.RemoveListener(OnPushVerifyBtnClick);
            }
            pushVerifyBtn = null;

            if (closeBtn)
            {
                closeBtn.onClick.RemoveListener(OnCloseFrameBtnClick);
            }
            closeBtn = null;
           
        }

        protected override void _OnOpenFrame()
        {
            coolDownNum = 60;
            isCoolDown = false;
            InitBindPhoneModeShow();
            InitBtnsState();
            InitAwardItems();

            if (mobileNumInput)
            {
                mobileNumInput.text = "";
            }
            if (mobileVerifyInput)
            {
                mobileVerifyInput.text = "";
            }

            MobileBindManager.GetInstance().AddSendPhoneNumSuccHandler(OnSendPhoneNumSucc);
            MobileBindManager.GetInstance().AddVerifySuccHandler(OnVerifyPhoneSucc);

            MobileBindManager.GetInstance().AddSDKBindPhoneNumSuccHandler(OnSDKBindPhoneSucc);

            if (ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
            {
                ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.AddSyncTaskDataChangeListener(OnTaskStateChanged);
            }

            if (MobileBindManager.GetInstance().SendPhoneVerifySucc == false)
            {
                MobileBindManager.GetInstance().HasSendVerify = false;
            }
            if (MobileBindManager.GetInstance().HasBindPhone())
            {
                MobileBindManager.GetInstance().HasSendVerify = true;
            }

            //打开界面时 检查玩家是否绑定过手机号 等待回调 如果绑定成功 就不等待跳转sdk绑定界面的回调了
            if (MobileBindManager.GetInstance().hasRoleBindPhoneAwardActive == false)
            {
                SDKInterface.Instance.CheckIsPhoneBind();
            }
        }

        protected override void _OnCloseFrame()
        {

            MobileBindManager.GetInstance().RemoveAllSendPhoneNumSuccHandler();
            MobileBindManager.GetInstance().RemoveAllVerifySuccHandler();

            MobileBindManager.GetInstance().RemoveAllSDKBindPhoneNumSuccHandler();

            //fixed null reference !!!
            if( ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
            {
                ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RemoveAllSyncTaskDataChangeListener();
            }

            currData = null;

            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.SDKBindPhone);
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            StartSendVerifyCountDown();

            if (MobileBindManager.GetInstance().HasSendVerify == false)
            {
                //SDKInterface.instance.CheckIsPhoneBind();
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        void InitAwardItems()
        {
            this.currData = MobileBindManager.GetInstance().BindPhoneActData;
            if (awardItems == null)
                return;
            if (currData != null)
            {
                var taskDatas = currData.activityDetailDataList;
                if (taskDatas == null || taskDatas.Count <= 0)
                    return;
                if (taskDatas[0] != null)
                {
                    var currAwardDatas = taskDatas[0].awardDataList;
                    if (currAwardDatas != null && currAwardDatas.Count <= awardItems.Length)
                    {
                        for (int i = 0; i < currAwardDatas.Count; i++)
                        {
                            ComItem awardItem = this.CreateComItem(awardItems[i].gameObject);
                            var itemData = ItemDataManager.CreateItemDataFromTable((int)currAwardDatas[i].Id);
                            itemData.Count = currAwardDatas[i].Num;
                            awardItem.Setup(itemData, (GameObject go, ItemData iData) =>
                            {
                                ItemTipManager.GetInstance().ShowTip(iData);
                            });
                        }
                    }
                    else
                    {
                        Logger.LogError("手机绑定，奖励数量太多了，目前最多4个呀！！！");
                    }
                }
            }
        }

        bool CheckIsRightMobileNum(string mNum)
        {
            if (string.IsNullOrEmpty(mNum))
                return false;
            Regex regex = new Regex(@"^[1]+\d{10}");
            return regex.IsMatch(mNum);
        }

        #region Set Btns Status
        void InitBtnsState()
        {
            OnTaskStateChanged();

            SetSendVerifyBtnState(SendVerifyBtnState.UnEnable);
        }

        void SetReceiveBtnState(ReceiveBtnState state)
        {
            if (receiveBtnText == null)
                return;
            switch (state)
            {
                case ReceiveBtnState.UnReceive:
                    receiveBtnText.text = MobileBindStateText.UNENABLE_RECEIVE;
                    EnableReceiveBtn(false);
                    break;
                case ReceiveBtnState.ToReceive:
                    receiveBtnText.text = MobileBindStateText.TO_RECEIVE;
                    EnableReceiveBtn(true);
                    break;
                case ReceiveBtnState.Received:
                    receiveBtnText.text = MobileBindStateText.RECEIVED;
                    EnableReceiveBtn(false);
                    break;
            }
        }

        void EnableReceiveBtn(bool enabled)
        {
            if (receiveBtnGray)
                receiveBtnGray.enabled = !enabled;
            if (receiveBtn)
                receiveBtn.interactable = enabled;
        }

        void SetSendVerifyBtnState(SendVerifyBtnState state)
        {
            if (sendVerifyBtnText == null) return;
            switch (state)
            {
                case SendVerifyBtnState.UnEnable:
                    sendVerifyBtnText.text = MobileBindStateText.SEND_VERIFICATION_CODE;
                    EnableSendVerifyBtn(false);
                    break;
                case SendVerifyBtnState.Normal:
                    sendVerifyBtnText.text = MobileBindStateText.SEND_VERIFICATION_CODE;
                    EnableSendVerifyBtn(true);
                    break;
                case SendVerifyBtnState.Sending:
                    sendVerifyBtnText.text = MobileBindStateText.SEND_VERIFICATION_CODE + "(" + coolDownNum + ")";
                    EnableSendVerifyBtn(false);
                    break;
            }
        }
        void EnableSendVerifyBtn(bool enabled)
        {
            if (sendVerifyBtn)
                sendVerifyBtn.interactable = enabled;
            if (sendVerifyGray)
                sendVerifyGray.enabled = !enabled;
        }

        void StartSendVerifyCountDown()
        {
            if (isCoolDown == false)
                return;
            if (coolDownNum <= 0)
                return;
            coolDownNum -= Time.deltaTime;
            if (coolDownNum > 0)
                SetSendVerifyBtnState(SendVerifyBtnState.Sending);
            else
            {
                SetSendVerifyBtnState(SendVerifyBtnState.Normal);
                isCoolDown = false;
            }
        }

        void InitBindPhoneModeShow()
        {
            if (SDKInterface.Instance.NeedSDKBindPhoneOpen())
            {
                SetBindPhoneModeShow(BindPhoneMode.OpenSDK);
            }
            else
            {
                SetBindPhoneModeShow(BindPhoneMode.Normal);
            }
        }

        void SetBindPhoneModeShow(BindPhoneMode bindPhoneMode)
        {
            if (bindPhoneMode == BindPhoneMode.Normal)
            {
                if (normalBindGo)
                    normalBindGo.CustomActive(true);
                if (sdkGotoBindBtn)
                    sdkGotoBindBtn.gameObject.CustomActive(false);
            }
            else if(bindPhoneMode == BindPhoneMode.OpenSDK)
            {
                if (normalBindGo)
                    normalBindGo.CustomActive(false);
                if (sdkGotoBindBtn)
                    sdkGotoBindBtn.gameObject.CustomActive(true);

                EnableSDKOpenBindPhoneBtn(true);
            }
            else if (bindPhoneMode == BindPhoneMode.Finished)
            {
                EnableSDKOpenBindPhoneBtn(false);
            }
        }

        void EnableSDKOpenBindPhoneBtn(bool enabled)
        {
            if (sdkGotoBindBtnGray)
                sdkGotoBindBtnGray.enabled = !enabled;
            if (sdkGotoBindBtn)
                sdkGotoBindBtn.interactable = enabled;
        }

        #endregion

        #region Listeners
        void OnSendPhoneNumSucc()
        {
            //StartSendVerifyCountDown();
            isCoolDown = true;
        }

        void OnVerifyPhoneSucc()
        {
            //TODO
            SetReceiveBtnState(ReceiveBtnState.ToReceive);

            //android 隐藏打开sdk 手机绑定界面按钮
            SetBindPhoneModeShow(BindPhoneMode.Finished);
        }

        void OnSDKBindPhoneSucc(string sdkBindPhoneNum)
        {
            SendVerifyCodeBySDK(sdkBindPhoneNum);
        }

        void OnTaskStateChanged()
        {
            this.currData = MobileBindManager.GetInstance().BindPhoneActData;
           
            if (currData != null)
            {
                if (currData.activityDetailDataList == null)
                    return;
                if (currData.activityDetailDataList.Count > 0)
                {
                    var actTaskState = currData.activityDetailDataList[0].ActivityDetailState;
                    switch (actTaskState)
                    {
                        case ActivityTaskState.Over:
                            SetReceiveBtnState(ReceiveBtnState.Received);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SDKBindPhoneFinished,false);
                            if (ClientSystemManager.GetInstance().IsFrameOpen<MobileBindFrame>())
                            {
                                frameMgr.CloseFrame<MobileBindFrame>();
                            }

                            MobileBindManager.GetInstance().hasRoleBindPhoneAwardActive = true;
                            break;
                        case ActivityTaskState.Finished:
                            SetReceiveBtnState(ReceiveBtnState.ToReceive);
                            EnableSDKOpenBindPhoneBtn(false);

                            MobileBindManager.GetInstance().hasRoleBindPhoneAwardActive = true;
                            break;
                        default:
                            SetReceiveBtnState(ReceiveBtnState.UnReceive);

                            MobileBindManager.GetInstance().hasRoleBindPhoneAwardActive = false;
                            break;
                    }
                }
            }
        }
        #endregion

        #region UI CallBack

        void OnReceiveBtnClick()
        {
            if (currData == null)
                return;
            if(currData.activityDetailDataList==null)
                return;
            if (currData.activityDetailDataList.Count == 0)
                return;
            if (ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
            {
                ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RequestOnTakeActTask(currData.DataId, currData.activityDetailDataList[0].DataId);
            }
        }

        void OnSendVerifyBtnClick()
        {
            if (mobileNumInput)
            {
                string num = mobileNumInput.text;
                if (string.IsNullOrEmpty(num))
                {
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobileBindStateText.PhoneNumIsEmpty);
                }
                else if (num.Length != 11)
                {
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobileBindStateText.PhoneNumNotEleven);
                }
                else
                {
                    MobileBindManager.GetInstance().SendPhoneNumber(num);
                }
            }
        }

        void OnPushVerifyBtnClick()
        {
            if (mobileVerifyInput && mobileNumInput)
            {
                string phoneNum = mobileNumInput.text;
                string verify = mobileVerifyInput.text;
                if (string.IsNullOrEmpty(verify) || string.IsNullOrEmpty(phoneNum))
                {
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobileBindStateText.PhoneNumOrVerifyEmpty);
                }
                else
                {
                    MobileBindManager.GetInstance().SendNumberVerify(phoneNum, verify);
                }
            }
        }

        void OnInputPhoneNumEnd(string input)
        {
            if (CheckIsRightMobileNum(input))
            {
                SetSendVerifyBtnState(SendVerifyBtnState.Normal);
            }
            else {
                SetSendVerifyBtnState(SendVerifyBtnState.UnEnable);
            }
        }

        void OnGoToBindPhoneInSDK()
        {
            MobileBindManager.GetInstance().HasSendVerify = false;
            SDKInterface.Instance.OpenBindPhone();
        }

        void OnCloseFrameBtnClick()
        {
            this.Close();
        }

        #endregion

        /// <summary>
        ///  SDK 手机绑定成功回调 -> 发送绑定成功手机号给服务器验证 - 区别正常验证模式
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="verifyCode"></param>
        void SendVerifyCodeBySDK(string phoneNum = "", string verifyCode = "")
        {
            if (SDKInterface.Instance.NeedSDKBindPhoneOpen())
            {
                MobileBindManager.GetInstance().SendNumberVerify(phoneNum);
                Logger.LogProcessFormat("发送手机绑定验证到服务器 3-frame,{0} + {1}", phoneNum, verifyCode);
            }
        }
    }
}
