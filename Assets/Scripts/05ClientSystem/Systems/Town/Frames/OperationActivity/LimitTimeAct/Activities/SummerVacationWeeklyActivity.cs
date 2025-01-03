using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class SummerVacationWeeklyActivity : LimitTimeCommonActivity
    {

        public override void Init(uint activityId)
        {
            base.Init(activityId);
            
        }

        public override void Show(Transform root)
        {
            base.Show(root);

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
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/SummerVacationWeeklyItem";
        }
        /// <summary>
        /// 本身活动预制体的路径
        /// </summary>
        /// <returns></returns>
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/SummerVacationWeeklyActivity";
        }
    }
}
