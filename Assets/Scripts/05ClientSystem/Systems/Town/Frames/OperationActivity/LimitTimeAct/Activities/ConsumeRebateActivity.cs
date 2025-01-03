using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class ConsumeRebateActivity : LimitTimeCommonActivity
    {

       
        protected override string _GetPrefabPath()
        {
            string prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ConsumeRebateActivity";
            if (mDataModel != null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                prefabPath = mDataModel.ActivityPrefafPath;
                return prefabPath;
            }
            return prefabPath;
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ConsumeRebateItem";
        }


    }
}
