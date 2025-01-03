using UnityEngine;

namespace GameClient
{
    public sealed class HellTicketActivity : LimitTimeCommonActivity
	{
		public enum LotteryType
		{
			ConsumeLottery = 10002,
		}
		public override bool IsHaveRedPoint()
        {
            return CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_TIME) > 0;
        }

        public override void Init(uint activityId)
        {
            base.Init(activityId);
            if (mDataModel != null)
            {
                mDataModel.SortTaskByState();
            }
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        public override void Show(Transform root)
        {
            base.Show(root);
            var view = mView as HellTicketActivityView;
            if (view != null)
            {
                view.OnLotteryClick = _DrawLottery;
                view.OnPreviewAwardsClick = _PreviewAwards;
            }
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/HellTicketActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/HellTicketItem";
        }

        void _OnCountValueChanged(UIEvent uiEvent)
        {
            var view = mView as HellTicketActivityView;
            if (view != null)
            {
                view.UpdateLotteryCount();
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, this);
        }

        void _PreviewAwards()
        {
            ClientSystemManager.GetInstance().OpenFrame<RewardShow>(FrameLayer.Middle, (int)LotteryType.ConsumeLottery);
        }

        void _DrawLottery()
        {
            ClientSystemManager.GetInstance().OpenFrame<TurnTable>(FrameLayer.Middle, (int)LotteryType.ConsumeLottery);
        }
    }
}