using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;

namespace GameClient
{
    class EquipPriceFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipRecovery/EquipPriceFrame";
        }
        protected sealed override void _OnOpenFrame()
        {

        }
        protected sealed override void _OnCloseFrame()
        {

        }
    }
}