using DataModel;
using Protocol;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        /// <summary>
        /// 夺宝活动开奖界面 draw lottery开奖
        /// </summary>
        public sealed class ActivityTreasureLotteryDrawFrame : ClientFrame
        {
            public override string GetPrefabPath()
            {
                return "UIFlatten/Prefabs/ActivityTreasureLottery/ActivityTreasureLotteryDrawFrame";
            }

            protected override void _OnOpenFrame()
            {
                base._OnOpenFrame();
                var model = ActivityTreasureLotteryDataManager.GetInstance().DequeueDrawLottery();
#if !UNITY_EDITOR
                if (mView == null || model == null)
                {
                    return;
                }
#endif
                mView.Init(model, model.TopFiveInvestPlayers.Length > 1);
            }

            protected override void _OnCloseFrame()
            {
                base._OnCloseFrame();
#if !UNITY_EDITOR
                if (mView == null)
                {
                    return;
                }
#endif
                mView.Dispose();
            }

            #region ExtraUIBind
            private Button mButtonRecord = null;
            private ActivityTreasureLotteryDrawView mView = null;
            private Button mButtonClose = null;

            protected override void _bindExUI()
            {
                mButtonRecord = mBind.GetCom<Button>("ButtonRecord");
                mButtonRecord.SafeAddOnClickListener(_onButtonRecordButtonClick);
                mView = mBind.GetCom<ActivityTreasureLotteryDrawView>("View");
                mButtonClose = mBind.GetCom<Button>("ButtonClose");
                mButtonClose.SafeAddOnClickListener(_onButtonCloseButtonClick);
            }

            protected override void _unbindExUI()
            {
                mButtonRecord.SafeRemoveOnClickListener(_onButtonRecordButtonClick);
                mButtonRecord = null;
                mView = null;
                mButtonClose.SafeRemoveOnClickListener(_onButtonCloseButtonClick);
                mButtonClose = null;
            }
            #endregion

            #region Callback
            private void _onButtonRecordButtonClick()
            {
                if (ActivityTreasureLotteryDataManager.GetInstance().GetState() == ETreasureLotterState.Open)
                {
                    ClientSystemManager.GetInstance().OpenFrame<ActivityTreasureLotteryFrame>(FrameLayer.Middle, EActivityTreasureLotteryView.HistoryView);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotteryShowActivity);
                }
                else
                {
                    SystemNotifyManager.SystemNotify(9062);
                }
                Close();
            }
            private void _onButtonCloseButtonClick()
            {
                Close();
            }
            #endregion
        }
    }
}