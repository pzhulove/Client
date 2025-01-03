using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class SummerVacationGrandTotalActivity : LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            base.Init(activityId);
        }
        public override void UpdateData()
        {
            base.UpdateData();

        }
        public override void Dispose()
        {
            base.Dispose();
        }
        /// <summary>
        /// Item路径
        /// </summary>
        /// <returns></returns>
        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/SummerVacationGrandTotalItem";
        }
        /// <summary>
        /// 本身活动预制体的路径0
        /// </summary>
        /// <returns></returns>
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/SummerVacationGrandTotalActivity";
        }

    }
}
