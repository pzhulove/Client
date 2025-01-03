using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class TuanBenActivity : LimitTimeCommonActivity
    {
      

        protected sealed override string _GetPrefabPath()
        {
            if (mDataModel != null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                return mDataModel.ActivityPrefafPath;
            }
            return "UIFlatten/Prefabs/OperateActivity/Anniversary/Acitivity/TuanBenActivity";
        }
        protected override string _GetItemPrefabPath()
        {
            return string.Empty;
        }
     
        
    }
}
