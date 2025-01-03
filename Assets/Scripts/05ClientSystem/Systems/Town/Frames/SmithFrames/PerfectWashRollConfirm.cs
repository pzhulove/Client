using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
   
    class PerfectWashRollConfirm : ClientFrame
    {
        int m_perfectScrollsID;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/PerfectWashRollConfirm";
        }

        ComItem comItem;
        Text name;
        GameObject goParent;
        GameObject goPrefab;
        protected override void _OnOpenFrame()
        {
            m_perfectScrollsID = int.Parse(TR.Value("ItemKeyPerfectScrollsID"));

            goParent = Utility.FindChild(frame, "middle/body/Viewport/content");
            goPrefab = Utility.FindChild(goParent, "prefabs");
            goPrefab.CustomActive(true);
            comItem = CreateComItem(Utility.FindChild(goPrefab, "itemParent"));
            comItem.Setup(null, null);
            name = Utility.FindComponent<Text>(goPrefab, "name");

            _SetData();
        }

        protected override void _OnCloseFrame()
        {
            comItem.Setup(null, null);
            name = null;
        }

        void OnItemClicked(GameObject obj, ItemData item)
        {

        }

        void _SetData()
        {
            var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(m_perfectScrollsID);
            comItem.Setup(itemData, OnItemClicked);

            if (itemData != null)
            {
                name.text = itemData.GetColorName();
            }
        }

        [UIEventHandle("ok")]
        void OnClickOk()
        {
            //StrengthenDataManager.GetInstance().StrengthenItem(m_kData.equipData, false, m_kData.itemData.GUID);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUsePerfectWashRoll);
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("close/image")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }
    }
}