using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class HalloweenOnlineActivity : LimitTimeCommonActivity
    {
        public sealed override bool IsHaveRedPoint()
        {
            return CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_ONLINE_TIME) > 0;
        }

        public sealed override void Init(uint activityId)
        {
            base.Init(activityId);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        public sealed override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        void _OnCountValueChanged(UIEvent uiEvent)
        {
            var view = mView as HalloweenOnlineActivityView;
            if (view != null)
            {
                view.UpdateLotteryCount();
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, this);
        }

        protected sealed override string _GetPrefabPath()
        {
            string prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/HalloweenOnlineActivity";
            if (mDataModel != null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                prefabPath = mDataModel.ActivityPrefafPath;
                return prefabPath;
            }
            return prefabPath;
        }

        protected sealed override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/HalloweenOnlineItem";
        }
    }
}