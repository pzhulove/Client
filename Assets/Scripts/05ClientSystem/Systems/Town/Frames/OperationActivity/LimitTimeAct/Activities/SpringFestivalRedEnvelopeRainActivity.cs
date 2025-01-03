using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class SpringFestivalRedEnvelopeRainActivity : LimitTimeCommonActivity
    {
        protected sealed override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/SpringFestivalRedEnvelopeRainActivity";
        }

        protected sealed override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/SpringFestivalRedEnvelopeRainItem";
        }
    }
}