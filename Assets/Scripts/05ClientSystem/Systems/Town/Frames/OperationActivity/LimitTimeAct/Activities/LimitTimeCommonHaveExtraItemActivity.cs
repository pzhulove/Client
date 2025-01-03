using System;
using System.Collections.Generic;
using ActivityLimitTime;
using GameClient;
using Protocol;
using UnityEngine;

namespace GameClient
{ 
    public class LimitTimeCommonHaveExtraItemActivity : LimitTimeCommonActivity
    {
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/CommonHaveExtraItemActivity";
        }
    }
}