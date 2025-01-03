using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class ConsumePointActivity : LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            base.Init(activityId);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
        
        
        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ConsumePointActivityItem";
        }
    }
}