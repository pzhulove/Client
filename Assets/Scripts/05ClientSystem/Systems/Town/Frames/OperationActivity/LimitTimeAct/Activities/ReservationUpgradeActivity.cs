using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class ReservationUpgradeActivity : LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            base.Init(activityId);
            if (mDataModel != null)
            {
                mDataModel.SortTaskByState();
            }
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ReservationUpgradeActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ReservationUpgradeItem";
        }
    }
}