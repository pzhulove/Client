using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class AccumulateClearanceActivity : LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            base.Init(activityId);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
        

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/AccumulateClearanceActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/AccumulateClearanceItem";
        }
    }
}