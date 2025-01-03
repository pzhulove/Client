using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using System;

namespace GameClient
{
    class DevelopGuidanceEntranceFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/DevelopGuidanceFrame/DevelopGuidanceEntranceFrame";
        }

        void _InitEntranceItems()
        {
            GameObject goParent = Utility.FindChild(frame, "Content");
            GameObject goPrefab = Utility.FindChild(frame, "Content/GuidanceItem");
            goPrefab.CustomActive(false);
            m_akGuidanceEntranceItems.RecycleAllObject();
            int iLevel = PlayerBaseData.GetInstance().Level;
            var entranceLevelItem = TableManager.GetInstance().GetTableItem<ProtoTable.GuidanceLevelTable>(iLevel);
            if (entranceLevelItem == null)
            {
                return;
            }

            for (int j = 0; j < entranceLevelItem.RelationIds.Count; ++j)
            {
                var entranceItem = TableManager.GetInstance().GetTableItem<ProtoTable.GuidanceEntranceTable>(entranceLevelItem.RelationIds[j]);
                if (entranceItem == null)
                {
                    continue;
                }

                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(entranceItem.FunctionId);
                if (FuncUnlockdata == null || FuncUnlockdata.FinishLevel > PlayerBaseData.GetInstance().Level)
                {
                    continue;
                }

                m_akGuidanceEntranceItems.Create(new object[] { goParent, goPrefab, new EntranceData { entranceItem = entranceItem }, false });
            }
        }

        protected override void _OnOpenFrame()
        {
            _InitEntranceItems();
        }

        protected override void _OnCloseFrame()
        {
            m_akGuidanceEntranceItems.DestroyAllObjects();
        }

        [UIEventHandle("Title/Close")]
        void OnClickCloseFrame()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("BtnLink")]
        void OnClickLink()
        {
            ClientSystemManager.GetInstance().OpenFrame<DevelopGuidanceMainFrame>();
            frameMgr.CloseFrame(this);
        }

        CachedObjectListManager<GuidanceEntrance> m_akGuidanceEntranceItems = new CachedObjectListManager<GuidanceEntrance>();
    }
}