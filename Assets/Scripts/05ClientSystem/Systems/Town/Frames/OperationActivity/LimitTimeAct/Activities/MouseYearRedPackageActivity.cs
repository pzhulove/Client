using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class MouseYearRedPackageActivity: LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            if (data != null)
            {
                mDataModel = new LimitTimeActivityModel(data, _GetItemPrefabPath());
            }
        }
        
        protected override string _GetPrefabPath()
        {
            string prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/MouseYearRedPackageActivity";
            if (mDataModel != null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                prefabPath = mDataModel.ActivityPrefafPath;
                return prefabPath;
            }
            return prefabPath;
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/MouseYearRedPackageItem";
        }

      
    }

}
