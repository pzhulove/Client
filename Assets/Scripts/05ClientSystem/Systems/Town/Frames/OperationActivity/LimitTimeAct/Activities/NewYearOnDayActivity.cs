using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class NewYearOnDayActivity : LimitTimeCommonActivity
    {
        
        protected override string _GetPrefabPath()
        {
            string prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/NewYearOnDayActivity";
            if (mDataModel!= null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                prefabPath = mDataModel.ActivityPrefafPath;
                return prefabPath;
            }
            return prefabPath;
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/NewYearOnDayItem";
        }
    }
}
