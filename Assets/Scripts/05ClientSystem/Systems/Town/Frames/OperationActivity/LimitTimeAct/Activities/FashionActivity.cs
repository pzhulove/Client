using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class FashionActivity : LimitTimeCommonActivity
    {
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/FashionActivity";
        }
    }
}