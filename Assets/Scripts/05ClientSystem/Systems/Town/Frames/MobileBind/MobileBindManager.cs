using UnityEngine;
using System.Collections;
using GameClient;
using ActivityLimitTime;
using System;

namespace MobileBind
{
    public enum MobileBindReturnCode
    {
        None                = -1,
        Success             = 0,
        DBError             = 1,
        CD                  = 2,
        InvalidAccount      = 3,
        RepeatBind          = 4,
        InvalidVerifyCode   = 5,
        InvalidServerId     = 6,
        NoVerifyCode        = 7,
        ServerError         = 8,
        PlayerOffine        = 9,
        InvalidPhoneNum     = 10,
        Count,
    }
    public class MobileBindManager : DataManager<MobileBindManager>
    {
        //手机绑定返回数据  -  来自限时活动
        public ActivityLimitTimeData BindPhoneActData
        {
            get { 
                    if( ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                    {
                        return ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.BindPhoneOtherData ;
                    }
                    return null;
                }
        }

        private Coroutine sendPhoneNumCor;
        private Coroutine sendPhoneVerifyCor;

        private int sendPhoneNumRetCode = -1;
        private bool sendPhoneNumSucc;
        public bool SendPhoneNumSucc
        {
            get { return sendPhoneNumSucc; }
        }

        private int sendPhoneVerifyRetCode = -1;
        private bool sendPhoneVerifySucc;
        public bool SendPhoneVerifySucc
        {
            get { return sendPhoneVerifySucc; }
        }

        private bool hasSendVerify;
        public bool HasSendVerify
        {
            get { return hasSendVerify; }
            set { hasSendVerify = value; }
        }

        public override EEnterGameOrder GetOrder()
        {
            return base.GetOrder();
        }
        public override void Initialize()
        {
            Clear();
        }

        public override void Clear()
        {
            if (sendPhoneNumCor != null)
            {
                GameFrameWork.instance.StopCoroutine(sendPhoneNumCor);
                sendPhoneNumCor = null;
            }
            if (sendPhoneVerifyCor != null)
            {
                GameFrameWork.instance.StopCoroutine(sendPhoneVerifyCor);
                sendPhoneVerifyCor = null;
            }
            ResetSendParams();
        }

        void ResetSendParams()
        {
            sendPhoneNumRetCode = 0;
            sendPhoneVerifyRetCode = 0;
            sendPhoneNumSucc = false;
            sendPhoneVerifySucc = false;
        }

        public void SendPhoneNumber(string phoneNum)
        {
            sendPhoneNumRetCode = 0;

            if (sendPhoneNumCor != null)
            {
                GameFrameWork.instance.StopCoroutine(sendPhoneNumCor);
                sendPhoneNumCor = null;
            }
            sendPhoneNumCor = GameFrameWork.instance.StartCoroutine(_SendPhoneNum(phoneNum));
        }

        public void SendNumberVerify(string phoneNum,string verifyNum="")
        {
            sendPhoneVerifyRetCode = 0;

            if (sendPhoneVerifyCor != null)
            {
                GameFrameWork.instance.StopCoroutine(sendPhoneVerifyCor);
                sendPhoneVerifyCor = null;
            }
            sendPhoneVerifyCor = GameFrameWork.instance.StartCoroutine(_SendPhoneVerify(phoneNum,verifyNum));
        }

        /*
        private string sendPhoneNumRetParams;
        public string SendPhoneNumRetParams
        {
            get { return sendPhoneNumRetParams; }
        }
        */
        IEnumerator _SendPhoneNum(string phoneNum)
        {
            string url = string.Format("http://{0}/bind?platform={1}&openid={2}&accid={3}&phonenumber={4}",
                             Global.VERIFY_BIND_PHONE_ADDRESS,
                             Global.SDKChannelName[(int)(Global.Settings.sdkChannel)],
                             ClientApplication.playerinfo.openuid,
                             ClientApplication.playerinfo.accid,
                             phoneNum);
            if (string.IsNullOrEmpty(url))
                yield break;
               
            BaseWaitHttpRequest bwhr = new BaseWaitHttpRequest();
            bwhr.url = url;

            Logger.LogProcessFormat("手机绑定发送请求地址：{0}",url);

            yield return bwhr;
            if (bwhr.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                string resStr = bwhr.GetResultString();
                if (string.IsNullOrEmpty(resStr))
                {
                    sendPhoneNumRetCode = (int)MobileBindReturnCode.None;
                    Logger.LogProcessFormat("发送手机码，返回值为空");
                }
                else
                {
                    sendPhoneNumRetCode = Int32.Parse(resStr);
                    Logger.LogProcessFormat("发送手机码，返回值为 {0}",sendPhoneNumRetCode);
                }
            }
            if (sendPhoneNumRetCode != 0)
            {
                sendPhoneNumSucc = false;
                yield break;
            }
            else
            {
                sendPhoneNumSucc = true;
                //if (SendPhoneNumSuccListener != null)
                //     SendPhoneNumSuccListener();
            }
        }

        IEnumerator _SendPhoneVerify(string mobilePhone,string verifyCode = "")
        {
            Logger.LogProcessFormat("发送手机绑定验证到服务器 4-final,{0} + {1}", mobilePhone, verifyCode);
			hasSendVerify = true;

            string url = string.Format("http://{0}/verify?platform={1}&openid={2}&accid={3}&roleid={4}&code={5}&sid={6}&phonenumber={7}",
                              Global.VERIFY_BIND_PHONE_ADDRESS,
                              Global.SDKChannelName[(int)(Global.Settings.sdkChannel)],
                              ClientApplication.playerinfo.openuid,
                              ClientApplication.playerinfo.accid,
                              GameClient.PlayerBaseData.GetInstance().RoleID,
                              verifyCode,
                              ClientApplication.playerinfo.serverID + "",
                              mobilePhone);
       
            if (string.IsNullOrEmpty(url))
                yield break;
              
            BaseWaitHttpRequest bwhr = new BaseWaitHttpRequest();
            bwhr.url = url;
            yield return bwhr;
            if (bwhr.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                string resStr = bwhr.GetResultString();
                if (string.IsNullOrEmpty(resStr))
                {
                    sendPhoneVerifyRetCode = (int)MobileBindReturnCode.None;
                    Logger.LogProcessFormat("发送手机验证码，返回值为空");
                }
                else
                {
                    sendPhoneVerifyRetCode = Int32.Parse(resStr);
                    Logger.LogProcessFormat("发送手机验证码，返回值为{0}",sendPhoneVerifyRetCode);
                }
            }
            if (sendPhoneVerifyRetCode != 0)
            {
                sendPhoneVerifySucc = false;
                SystemNotifyText((MobileBindReturnCode)sendPhoneVerifyRetCode);
                yield break;
            }
            else
            {
                sendPhoneVerifySucc = true;
                //if (VerifySuccListener != null)
                //    VerifySuccListener();
            }
        }

        void SystemNotifyText(MobileBindReturnCode code)
        {
            switch (code)
            {
                case MobileBindReturnCode.CD:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.CD);
                    break;
                case MobileBindReturnCode.DBError:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.DBError);
                    break;
                case MobileBindReturnCode.InvalidAccount:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.InvalidAccount);
                    break;
                case MobileBindReturnCode.InvalidPhoneNum:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.InvalidPhoneNum);
                    break;
                case MobileBindReturnCode.InvalidServerId:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.InvalidServerId);
                    break;
                case MobileBindReturnCode.InvalidVerifyCode:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.InvalidVerifyCode);
                    break;
                case MobileBindReturnCode.NoVerifyCode:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.NoVerifyCode);
                    break;
                case MobileBindReturnCode.PlayerOffine:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.PlayerOffine);
                    break;
                case MobileBindReturnCode.RepeatBind:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.RepeatBind);
                    break;
                case MobileBindReturnCode.ServerError:
                    GameClient.SystemNotifyManager.SysNotifyTextAnimation(MobilePhoneNumVerifyText.ServerError);
                    break;
            }
        }


        #region UIEvent

        public event System.Action VerifySuccListener;
        public void AddVerifySuccHandler(System.Action handler)
        {
            RemoveAllVerifySuccHandler();
            if (VerifySuccListener == null)
                VerifySuccListener += handler;
        }

        public void RemoveAllVerifySuccHandler()
        {
            if (VerifySuccListener != null)
            {
                var invocations = VerifySuccListener.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        VerifySuccListener -= invocations[i] as Action;
                    }
                }
            }
        }

        public event System.Action SendPhoneNumSuccListener;
        public void AddSendPhoneNumSuccHandler(System.Action handler)
        {
            RemoveAllSendPhoneNumSuccHandler();
            if (SendPhoneNumSuccListener == null)
                SendPhoneNumSuccListener += handler;
        }

        public void RemoveAllSendPhoneNumSuccHandler()
        {
            if (SendPhoneNumSuccListener != null)
            {
                var invocations = SendPhoneNumSuccListener.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        SendPhoneNumSuccListener -= invocations[i] as Action;
                    }
                }
            }
        }

        public event System.Action<string> SDKBindPhoneNumSuccListener;
        public void AddSDKBindPhoneNumSuccHandler(System.Action<string> handler)
        {
            RemoveAllSDKBindPhoneNumSuccHandler();
            if (SDKBindPhoneNumSuccListener == null)
                SDKBindPhoneNumSuccListener += handler;
        }

        public void RemoveAllSDKBindPhoneNumSuccHandler()
        {
            if (SDKBindPhoneNumSuccListener != null)
            {
                var invocations = SDKBindPhoneNumSuccListener.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        SDKBindPhoneNumSuccListener -= invocations[i] as System.Action<string>;
                    }
                }
            }
        }

        #endregion

        #region 是否开启 手机绑定

        private bool toOpenMobileBind = true;

        private ulong bindMobileRoleId;
        public ulong BindMobileRoleId
        {
            get
            {
                return bindMobileRoleId;
            }
            set
            {
                bindMobileRoleId = value;
                //切帐号时重置
                hasRoleBindPhoneAwardActive = false;
            }
        }

        //帐号有角色已激活奖励，已领取或可领取
        public bool hasRoleBindPhoneAwardActive
        {
            get;
            set;
        }

        public void ToOpenMobileBind(bool open)
        {
            toOpenMobileBind = open;
        }
        public bool IsMobileBindFuncEnable()
        {
            if (!toOpenMobileBind)
                return false;
            if (IsRoleLevelEnable() == false)
                return false;
            if (SDKInterface.Instance.NeedSDKBindPhoneOpen() == false)
                return false;
            if (IsBindMobileDataEmpty())
                return false;
            if (IsBindPhoneActivityEnd())
                return false;
            if (HasBindPhone() && !HasBindMobileAwardToReceive())
                return false;
            if (hasRoleBindPhoneAwardActive && !HasBindMobileAwardToReceive())
                return false;
            return true;
        }

        public bool IsRoleLevelEnable()
        {
            int mobileBindOpen = 16;
            int enableLevel = 8;
            var clientTable = TableManager.GetInstance().GetTableItem<ProtoTable.SwitchClientFunctionTable>(mobileBindOpen);
            if (clientTable != null)
            {
                enableLevel = clientTable.ValueA;
            }
            if (PlayerBaseData.GetInstance().Level < enableLevel)
            {
                return false;
            }
            return true;
        }

        public bool HasBindPhone()
        {
            if (BindMobileRoleId > 0)
            {
                hasRoleBindPhoneAwardActive = true;
                return true;
            }
            return false;
        }
        //当前角色是否有奖励可领取
        public bool HasBindMobileAwardToReceive()
        {
            if (BindPhoneActData == null)
                return false;
            if (BindPhoneActData.activityDetailDataList == null)
                return false;
            if (BindPhoneActData.activityDetailDataList.Count > 0)
            {
                //活动任务奖励不可领取
                if (BindPhoneActData.activityDetailDataList[0].ActivityDetailState != ActivityTaskState.Finished)
                    return false;
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 手机活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool IsBindPhoneActivityEnd()
        {
            if (BindPhoneActData == null)
                return true;
            if (BindPhoneActData.ActivityState == ActivityState.End)
                return true;
            return false;
        }

        /// <summary>
        ///  礼包数据为空，表示不开启手机绑定
        ///  
        ///  当活动任务结束，手机活动关闭
        /// </summary>
        /// <returns></returns>
        public bool IsBindMobileDataEmpty()
        {
            if (BindPhoneActData == null)
                return true;
            if (BindPhoneActData.activityDetailDataList == null)
                return true;
            if (BindPhoneActData.activityDetailDataList.Count > 0 && BindPhoneActData.activityDetailDataList[0].ActivityDetailState == ActivityTaskState.Over)
                return true;
            return false;
        }


        /// <summary>
        ///  Bind Phone Succ
        /// </summary>
        /// <param name="bindPhoneNum">绑定成功的手机号</param>
        public void SDKBindPhoneSucc(string bindPhoneNum)
        {
            if (SDKBindPhoneNumSuccListener != null)
            {
                SDKBindPhoneNumSuccListener(bindPhoneNum);
            }
        }
        #endregion
    }

}