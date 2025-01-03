using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;

namespace GameClient
{
    [System.Serializable]
    public class EquipmentTransformationTabData
    {
        public int id;
        public string tabName;
        public string normalIconPath;
        public string checkIconPath;
        public GameObject content;
    }

    public class EquipmentTransformationView : MonoBehaviour, ISmithShopNewView
    {
        [SerializeField] private List<EquipmentTransformationTabData> equipmentTransformationTabDataList;
        [SerializeField] private ComUIListScript tabUIList;

        private SmithShopNewLinkData linkData;
        private List<EquipmentTransformationTabData> realEquipmentTransformationTabDataList = new List<EquipmentTransformationTabData>();
        private List<EquipmentTransformationTabItem> equipmentTransformationTabItemList = new List<EquipmentTransformationTabItem>();
        public void InitView(SmithShopNewLinkData linkData)
        {
            this.linkData = linkData;
            InitTabData();
            InitUIListScript();
        }

        private void InitTabData()
        {
            if (realEquipmentTransformationTabDataList == null)
                realEquipmentTransformationTabDataList = new List<EquipmentTransformationTabData>();

            realEquipmentTransformationTabDataList.Clear();
            for (int i = 0; i < equipmentTransformationTabDataList.Count; i++)
            {
                EquipmentTransformationTabData equipmentTransformationTabData = equipmentTransformationTabDataList[i];
                if (equipmentTransformationTabData == null)
                {
                    continue;
                }

                if (equipmentTransformationTabData.id == 1 || equipmentTransformationTabData.id == 2)
                {
                    var jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
                    if (jobTable == null)
                    {
                        continue;
                    }

                    if (jobTable.JobType != 1)
                    {
                        continue;
                    }
                }

                realEquipmentTransformationTabDataList.Add(equipmentTransformationTabData);
            }
        }

        private void InitUIListScript()
        {
            if (tabUIList != null)
            {
                tabUIList.Initialize();
                tabUIList.onBindItem += OnBindItemDelegate;
                tabUIList.onItemVisiable += OnItemVisiableDelegate;

                tabUIList.SetElementAmount(realEquipmentTransformationTabDataList.Count);
            }
        }

        private void UnInitUIListScript()
        {
            if (tabUIList != null)
            {
                tabUIList.onBindItem -= OnBindItemDelegate;
                tabUIList.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private EquipmentTransformationTabItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EquipmentTransformationTabItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            EquipmentTransformationTabItem equipmentTransformationTabItem = item.gameObjectBindScript as EquipmentTransformationTabItem;
            if (equipmentTransformationTabItem != null && item.m_index >= 0 && item.m_index < realEquipmentTransformationTabDataList.Count)
            {
                EquipmentTransformationTabData equipmentTransformationTabData = realEquipmentTransformationTabDataList[item.m_index];
                bool bSelected = linkData != null ? equipmentTransformationTabData.id == linkData.iDefaultSecondTabId : equipmentTransformationTabData.id == 0;
                equipmentTransformationTabItem.InitTabItem(equipmentTransformationTabData, bSelected, linkData, OnTabClick);
                equipmentTransformationTabItemList.Add(equipmentTransformationTabItem);
            }
        }

        private void OnTabClick(EquipmentTransformationTabData equipmentTransformationTabData)
        {
           
        }

        public void OnDisableView()
        {
            for (int i = 0; i < equipmentTransformationTabItemList.Count; i++)
            {
                var tabItem = equipmentTransformationTabItemList[i];
                tabItem.OnEnabelTabItem(false);
            }
        }

        public void OnEnableView()
        {
            for (int i = 0; i < equipmentTransformationTabItemList.Count; i++)
            {
                var tabItem = equipmentTransformationTabItemList[i];
                tabItem.OnEnabelTabItem();
            }
        }

        private void OnDestroy()
        {
            UnInitUIListScript();
            linkData = null;
            if (equipmentTransformationTabItemList != null)
                equipmentTransformationTabItemList.Clear();

            if (realEquipmentTransformationTabDataList != null)
                realEquipmentTransformationTabDataList.Clear();
        }
    }
}