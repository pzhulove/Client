using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class AnniversaryAccumulateClearanceActivity : LimitTimeCommonActivity
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
            return "UIFlatten/Prefabs/OperateActivity/Anniversary/Acitivity/AnniversaryAccumulateClearanceActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/Anniversary/Item/AnniversaryAccumulateClearanceItem";
        }
    }
}
