using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class ChallengeDungeonRewardItem : MonoBehaviour
    {

        private AwardItemData _awardItemData;
        private ItemData _itemData;

        [SerializeField] private Text itemName;
        [SerializeField] private GameObject itemRoot;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _awardItemData = null;
            _itemData = null;
        }

        public void InitItem(AwardItemData awardItemData)
        {

            _awardItemData = awardItemData;

            if (_awardItemData == null)
            {
                Logger.LogErrorFormat("AwardItemData is null");
                return;
            }

            InitContent();
        }

        private void InitContent()
        {
            _itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(_awardItemData.ID);
            if (_itemData != null)
            {
                _itemData.Count = _awardItemData.Num;
                if (itemRoot != null)
                {
                    var comItem = itemRoot.GetComponentInChildren<ComItem>();
                    if (comItem == null)
                        comItem = ComItemManager.Create(itemRoot);
                    if (comItem != null)
                        comItem.Setup(_itemData, OnItemClicked);
                }

                if (itemName != null)
                    itemName.text = _itemData.GetColorName();
            }

        }


        private void OnItemClicked(GameObject go, ItemData itemData)
        {
            if (_awardItemData == null)
                return;

            if (itemData != null)
                ItemTipManager.GetInstance().ShowTip(itemData);
        }

        public void OnItemRecycle()
        {
            ClearData();
        }

    }
}
