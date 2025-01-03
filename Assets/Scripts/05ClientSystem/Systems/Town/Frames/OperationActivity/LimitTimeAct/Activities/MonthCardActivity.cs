using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class MonthCardActivity : LimitTimeCommonActivity
    {
        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/MonthCardItem";
        }
    }
}