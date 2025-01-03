using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ChijiHandInEquipmentFrame : ClientFrame
    {
        ChijiNpcData NpcData = new ChijiNpcData();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiHandInEquipmentFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                ChijiNpcData npcData = userData as ChijiNpcData;
                if (npcData != null)
                {
                    NpcData.guid = npcData.guid;
                    NpcData.npcTableId = npcData.npcTableId;
                }
            }

            if (mChijiHandInEquipmentView != null)
            {
                mChijiHandInEquipmentView.InitView(NpcData);
            }
        }

        protected override void _OnCloseFrame()
        {
            if (NpcData != null)
            {
                NpcData.guid = 0;
                NpcData.npcTableId = 0;
            }

            base._OnCloseFrame();
        }

        #region ExtraUIBind
        private ChijiHandInEquipmentView mChijiHandInEquipmentView = null;

        protected override void _bindExUI()
        {
            mChijiHandInEquipmentView = mBind.GetCom<ChijiHandInEquipmentView>("ChijiHandInEquipmentView");
        }

        protected override void _unbindExUI()
        {
            mChijiHandInEquipmentView = null;
        }
        #endregion

    }
}