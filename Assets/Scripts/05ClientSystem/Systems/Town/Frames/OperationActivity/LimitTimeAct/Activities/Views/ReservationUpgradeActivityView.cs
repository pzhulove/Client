using Network;
using Protocol;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ReservationUpgradeActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField] private Text mTextCoin;
        [SerializeField]
        private Button mGoRoleSelectBtn;
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
            mTextCoin.SafeSetText(string.Format(TR.Value("activity_reservation_upgrade_coin_num"), CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_ACTIVITY_COIN_NUM)));
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            if(mGoRoleSelectBtn!=null)
            {
                mGoRoleSelectBtn.SafeAddOnClickListener(OnGoRoleSelectBtnClick);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            if (mGoRoleSelectBtn != null)
            {
                mGoRoleSelectBtn.SafeRemoveOnClickListener(OnGoRoleSelectBtnClick);
            }
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
            mTextCoin.SafeSetText(string.Format(TR.Value("activity_reservation_upgrade_coin_num"), CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_ACTIVITY_COIN_NUM)));
        }
        /// <summary>
        /// 打开是否前往角色选择界面的提示
        /// </summary>
        private void OnGoRoleSelectBtnClick()
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = string.Format(TR.Value("limitactivity_xiariyuyue_conetnt")),
                IsShowNotify = false,
                LeftButtonText = TR.Value("limitactivity_xiariyuyue_cancel"),
                RightButtonText = TR.Value("limitactivity_xiariyuyue_ok"),
                OnRightButtonClickCallBack = OnOKBtnClick
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }
        /// <summary>
        /// 前往打开角色选择界面
        /// </summary>
        private void OnOKBtnClick()
        {
            RoleSwitchReq req = new RoleSwitchReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            ClientSystemLogin.mSwitchRole = true;


            VoiceSDK.SDKVoiceManager.GetInstance().LeaveVoiceSDK();
        }
    }
}
