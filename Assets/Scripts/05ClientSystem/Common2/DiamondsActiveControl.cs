using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    class DiamondsActiveControl : MonoBehaviour
    {
        public List<GameObject> VipShowList = null;
        public List<GameObject> VipHideList = null;

        [SerializeField] private Text mTextBtn;
        [SerializeField] private Button btnGetReward;
        [SerializeField] private OnClickActive mOnBuyActive;
        [SerializeField] private UIGray btnRewardGray;
        [SerializeField] private Text mTextLeftDays;

        public int mComHelpNewTableId = 6200;
        public int mActiveTemplateId = 6000;
        public int mActivityId = 2500;

        private Button btnBuyAgain;
        private Button btnRewardLockers;
        private UIGray btnRewardLockersGray;

        private Text monthCardDesc;

        private GameObject lockerRedPointGo;
        private GameObject btnEffectRoot = null;
        private DOTweenAnimation btnAnim = null;

        // Use this for initialization
        void Start()
        {
            //_Update();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MonthCardChanged, _HandleUpdate);
            ActiveManager.GetInstance().onAddMainActivity += _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity += _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity += _OnUpdateMainActivity;
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;

			var bind = GetComponent<ComCommonBind>();
			if (bind != null)
			{
			    btnBuyAgain = bind.GetCom<Button>("btnBuyAgain");
				if (btnBuyAgain != null)
				{
					btnBuyAgain.onClick.AddListener(_onBtnBuyAgainButtonClick);
				}

                btnRewardLockers = bind.GetCom<Button>("BtnRewardLockers");
                if (btnRewardLockers != null)
                {
                    btnRewardLockers.onClick.AddListener(_onBtnOpenRewardLockers);
                }

                btnRewardLockersGray = bind.GetCom<UIGray>("BtnRewardLockersGray");
                monthCardDesc = bind.GetCom<Text>("MonthCardDesc");

                lockerRedPointGo = bind.GetGameObject("MonthCardLockerRedPoint");

                btnEffectRoot = bind.GetGameObject("btnEffectRoot");
                btnAnim = bind.GetCom<DOTweenAnimation>("btnAnim");
            }

            _Update();
            _RefreshMonthCardContents();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthCardRewardUpdate, _OnMonthCardRewardUpdate);

            //放在最后
            //请求刷新月卡翻牌奖励数据
            MonthCardRewardLockersDataManager.GetInstance().ReqMonthCardRewardLockersItems();

#if APPLE_STORE
            //屏蔽奖励暂存箱
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT))
            {
                btnRewardLockers.CustomActive(false);
            }
#endif 
        }

        public void OnBtnGetRewardClick()
        {
            var activityData = ActiveManager.GetInstance().GetChildActiveData(mActivityId);
            ActiveManager.GetInstance().OnClickActivity(ActiveManager.GetInstance().GetActiveData(mActiveTemplateId), btnGetReward.GetComponent<OnClickActive>(), activityData);
        }

        public void OnBtnBuyClick()
        {
            var activityData = ActiveManager.GetInstance().GetChildActiveData(mActivityId);
            ActiveManager.GetInstance().OnClickActivity(ActiveManager.GetInstance().GetActiveData(mActiveTemplateId), mOnBuyActive, activityData);
        }


        private void _onBtnBuyAgainButtonClick()
		{
            ActiveChargeFrame.CloseMe();
			ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.PAY);
		}

        private void _onBtnOpenRewardLockers()
        {
            bool rewardLockersEmpty = MonthCardRewardLockersDataManager.GetInstance().IsMonthCardRewardLockersEmpty();
            if (rewardLockersEmpty)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("vip_month_card_box_empty_tips"));
                return;
            } 

            ClientSystemManager.GetInstance().OpenFrame<MonthCardRewardLockersFrame>(FrameLayer.Middle);
        }

        void _HandleUpdate(UIEvent uiEvent)
        {
            _Update();
        }

        void _Update()
        {
            var activityData = ActiveManager.GetInstance().GetChildActiveData(2500);
            bool bVip = false;
            if (activityData != null && activityData.status != (int)Protocol.TaskStatus.TASK_UNFINISH &&
                activityData.status != (int)Protocol.TaskStatus.TASK_INIT)
            {
                bVip = true;
            }
            if (VipShowList != null)
            {
                for(int i = 0; i < VipShowList.Count; ++i)
                {
                    VipShowList[i].CustomActive(bVip);
                }
            }
            if(VipHideList != null)
            {
                for (int i = 0; i < VipHideList.Count; ++i)
                {
                    VipHideList[i].CustomActive(!bVip);
                }
            }

            int leftDays = ((int)PlayerBaseData.GetInstance().MonthCardLv - (int)TimeManager.GetInstance().GetServerDoubleTime()) / 24 / 60 / 60;
            leftDays = leftDays < 0 ? 0 : leftDays;
            mTextLeftDays.SafeSetText(string.Format(TR.Value("vip_month_card_left_days"), leftDays.ToString()));
            btnRewardGray.enabled = activityData.status == (int)Protocol.TaskStatus.TASK_OVER;
            btnGetReward.enabled = activityData.status != (int)Protocol.TaskStatus.TASK_OVER;

            switch((Protocol.TaskStatus)activityData.status)
            {
                case Protocol.TaskStatus.TASK_INIT:
                    mTextBtn.SafeSetText(TR.Value("month_card_accquire_btn_buy"));
                    break;
                case Protocol.TaskStatus.TASK_FINISHED:
                    mTextBtn.SafeSetText(TR.Value("month_card_accquire_btn_get"));
                    break;
                case Protocol.TaskStatus.TASK_OVER:
                    mTextBtn.SafeSetText(TR.Value("month_card_accquire_btn_has_get"));
                    break;
            }

            _RefreshBtnRewardLockersStatus();
        }

        void _OnAddMainActivity(ActiveManager.ActiveData data)
        {
            _Update();
        }

        void _OnRemoveMainActivity(ActiveManager.ActiveData data)
        {
            _Update();
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            _Update();
        }

        void _OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            _Update();
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MonthCardChanged, _HandleUpdate);
            ActiveManager.GetInstance().onAddMainActivity -= _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity -= _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity -= _OnUpdateMainActivity;
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;


            if (btnBuyAgain != null)
            {
                btnBuyAgain.onClick.RemoveListener(_onBtnBuyAgainButtonClick);
            }
            if (btnRewardLockers != null)
            {
                btnRewardLockers.onClick.RemoveListener(_onBtnOpenRewardLockers);
            }
            btnRewardLockersGray = null;
            monthCardDesc = null;

            lockerRedPointGo = null;

            btnEffectRoot = null;
            btnAnim = null;

            if (ClientSystemManager.GetInstance().IsFrameOpen<MonthCardRewardLockersFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MonthCardRewardLockersFrame>();
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthCardRewardUpdate, _OnMonthCardRewardUpdate);
        }

        private void _RefreshBtnRewardLockersStatus()
        {
            //bool monthCardEnabled = PayManager.GetInstance().HasMonthCardEnabled();
            bool rewardLockersEmpty = MonthCardRewardLockersDataManager.GetInstance().IsMonthCardRewardLockersEmpty();

            //bool isLockersBtnEnabled =
            //    (monthCardEnabled == false) ||
            //    (monthCardEnabled && rewardLockersEmpty == false);
            bool isLockersBtnEnabled = !rewardLockersEmpty;
            //if (btnRewardLockers)
            //{
            //    btnRewardLockers.enabled = isLockersBtnEnabled;
            //}
            if (btnRewardLockersGray)
            {
                // btnRewardLockersGray.SetEnable(!isLockersBtnEnabled);
                btnRewardLockersGray.enabled = !isLockersBtnEnabled;
            }

            bool isRedPointShow = MonthCardRewardLockersDataManager.GetInstance().IsRedPointShow();
            if (lockerRedPointGo)
            {
                lockerRedPointGo.CustomActive(isRedPointShow);
            }
            if (btnEffectRoot)
            {
                btnEffectRoot.CustomActive(isRedPointShow);
            }
            if (btnAnim)
            {
                if (isRedPointShow)
                {
                    btnAnim.DORestart();
                }
                else
                {
                    btnAnim.DOPause();
                }
            }
        }

        private void _RefreshMonthCardContents()
        {
            var _commonHelpTable = TableManager.GetInstance().GetTableItem<ProtoTable.CommonHelpTable>(mComHelpNewTableId);
            if (_commonHelpTable == null)
            {
                Logger.LogErrorFormat("CommonHelpTable is null and helpId is {0}", mComHelpNewTableId);
                return;
            }
            string tableText = _commonHelpTable.Descs;
            if (string.IsNullOrEmpty(tableText))
            {
                return;
            }
            if (monthCardDesc)
            {
                monthCardDesc.text = tableText.Replace("\\n", "\n");
            }
        }

        private void _OnMonthCardRewardUpdate(UIEvent uiEvent)
        {
            _RefreshBtnRewardLockersStatus();
        }
    }
}