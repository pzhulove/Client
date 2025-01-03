using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;

namespace GameClient
{
    [System.Serializable]
    public class EquipmentIntensifiedTabData
    {
        public int id;
        public string tabName;
        public string normalIconPath;
        public string checkIconPath;
        public GameObject content;
    }

    class EquipmentIntensifiedView : StrengthGrowthBaseView
    {
        [SerializeField] private List<EquipmentIntensifiedTabData> equipmentIntensifiedTabDataList;
        [SerializeField] private ComUIListScript comUIListScript;

        private SmithShopNewLinkData linkData;
        private StrengthenGrowthView mStrengthenGrowthView;
        private StrengthenGrowthType mType;
        private List<EquipmentIntensifiedTabItem> equipmentIntensifiedTabItemList = new List<EquipmentIntensifiedTabItem>();
        public override void IniteData(SmithShopNewLinkData linkData, StrengthenGrowthType type, StrengthenGrowthView strengthenGrowthView)
        {
            if (equipmentIntensifiedTabItemList != null)
                equipmentIntensifiedTabItemList.Clear();

            this.linkData = linkData;
            mType = type;
            mStrengthenGrowthView = strengthenGrowthView;

            InitUIListScript();
        }

        private void InitUIListScript()
        {
            if(comUIListScript != null)
            {
                comUIListScript.Initialize();
                comUIListScript.onBindItem += OnBindItemDelegate;
                comUIListScript.onItemVisiable += OnItemVisiableDelegate;

                comUIListScript.SetElementAmount(equipmentIntensifiedTabDataList.Count);
            }
        }

        private void UnInitUIListScript()
        {
            if (comUIListScript != null)
            {
                comUIListScript.onBindItem -= OnBindItemDelegate;
                comUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private EquipmentIntensifiedTabItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EquipmentIntensifiedTabItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            EquipmentIntensifiedTabItem equipmentIntensifiedTabItem = item.gameObjectBindScript as EquipmentIntensifiedTabItem;
            if(equipmentIntensifiedTabItem != null && item.m_index >= 0 && item.m_index < equipmentIntensifiedTabDataList.Count)
            {
                EquipmentIntensifiedTabData equipmentIntensifiedTabData = equipmentIntensifiedTabDataList[item.m_index];
                bool bSelected = linkData != null ? equipmentIntensifiedTabData.id == linkData.iDefaultSecondTabId : equipmentIntensifiedTabData.id == 0;
                equipmentIntensifiedTabItem.InitTabItem(equipmentIntensifiedTabData, bSelected, mStrengthenGrowthView, linkData, OnTabClick);
                equipmentIntensifiedTabItemList.Add(equipmentIntensifiedTabItem);
            }
        }

        private void OnTabClick(EquipmentIntensifiedTabData equipmentIntensifiedTabData)
        {
            StrengthenGrowthType type = StrengthenGrowthType.SGT_Gtowth;
            if (equipmentIntensifiedTabData.id == (int)GrowthSubTabType.GSTT_GROWTH)
            {
                type = StrengthenGrowthType.SGT_Gtowth;
            }
            else if (equipmentIntensifiedTabData.id == (int)GrowthSubTabType.GSTT_CLEAT)
            {
                type = StrengthenGrowthType.SGT_Clear;
            }
            else if (equipmentIntensifiedTabData.id == (int)GrowthSubTabType.GSTT_ACTIVATE)
            {
                type = StrengthenGrowthType.SGT_Activate;
            }
            else if (equipmentIntensifiedTabData.id == (int)GrowthSubTabType.GSTT_CHANGE)
            {
                type = StrengthenGrowthType.SGT_Change;
            }

            OnSetStrengthGrowthType(type);
        }

        private void OnSetStrengthGrowthType(StrengthenGrowthType type)
        {

            if (mStrengthenGrowthView != null)
            {
                mStrengthenGrowthView.OnSetStrengthGrowthType(type);
            }
        }

        public override void OnEnableView()
        {
            for (int i = 0; i < equipmentIntensifiedTabItemList.Count; i++)
            {
                var tabItem = equipmentIntensifiedTabItemList[i];
                tabItem.OnEnabelTabItem();
            }
        }

        public override void OnDisableView()
        {
            for (int i = 0; i < equipmentIntensifiedTabItemList.Count; i++)
            {
                var tabItem = equipmentIntensifiedTabItemList[i];
                tabItem.OnEnabelTabItem(false);
            }
        }

        private void OnDestroy()
        {
            UnInitUIListScript();
            linkData = null;
            mStrengthenGrowthView = null;
            mType = StrengthenGrowthType.SGT_NONE;
            if (equipmentIntensifiedTabItemList != null)
                equipmentIntensifiedTabItemList.Clear();
        }
    }
}