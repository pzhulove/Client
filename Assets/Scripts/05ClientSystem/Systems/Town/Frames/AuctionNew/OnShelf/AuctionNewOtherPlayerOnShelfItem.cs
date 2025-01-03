using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;


namespace GameClient
{

    public class AuctionNewOtherPlayerOnShelfItem : MonoBehaviour
    {

        private AuctionBaseInfo _auctionBaseInfo;

        private ItemData _itemData;

        [SerializeField] private GameObject itemPos;

        [Space(10)]
        [HeaderAttribute("NormalContent")]
        [Space(10)]
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemScoreLabel;
        [SerializeField] private Text itemPriceValueLabel;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ResetData();
        }

        private void BindEvents()
        {
        }

        private void UnBindEvents()
        {
        }

        private void ResetData()
        {
            _auctionBaseInfo = null;
            _itemData = null;
        }
        
        public void InitItem(AuctionBaseInfo auctionBaseInfo)
        {
            _auctionBaseInfo = auctionBaseInfo;

            if (_auctionBaseInfo == null)
            {
                Logger.LogError("OtherPlayerOnShelf InitItem auctionBaseInfo is null");
                return;
            }

            InitItemView();
        }

        private void InitItemView()
        {
            _itemData = ItemDataManager.CreateItemDataFromTable((int)_auctionBaseInfo.itemTypeId);
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)_auctionBaseInfo.itemTypeId);
            if (_itemData == null || itemTable == null)
            {
                Logger.LogErrorFormat("ItemData or ItemTable is null and itemTypeid is {0}",
                    _auctionBaseInfo.itemTypeId);
                return;
            }

            _itemData.Count = (int)_auctionBaseInfo.num;
            _itemData.StrengthenLevel = (int)_auctionBaseInfo.strengthed;
            _itemData.GUID = _auctionBaseInfo.guid;
            AuctionNewUtility.UpdateItemDataByEquipType(_itemData, _auctionBaseInfo);

            if (itemPos != null)
            {
                var comItem = itemPos.GetComponentInChildren<ComItem>();
                if (comItem == null)
                    comItem = ComItemManager.Create(itemPos);

                if (comItem != null)
                {
                    comItem.Setup(_itemData, OnShowOtherPlayerItemTip);
                    var isTreasure = _auctionBaseInfo.isTreas == 1 ? true : false;
                    comItem.SetShowTreasure(isTreasure);
                }
            }

            if (itemName != null)
                itemName.text = _itemData.GetColorName();

            //在售和公示可能显示评分
            if (itemScoreLabel != null)
            {
                if (_auctionBaseInfo.itemScore > 0)
                {
                    CommonUtility.UpdateTextVisible(itemScoreLabel, true);
                    itemScoreLabel.text = string.Format(TR.Value("auction_new_itemDetail_score_value"),
                        _auctionBaseInfo.itemScore);
                }
                else
                {
                    CommonUtility.UpdateTextVisible(itemScoreLabel, false);
                }
            }

            //在售和公示的单价
            if (itemPriceValueLabel != null)
            {
                var singlePrice = (ulong)_auctionBaseInfo.price;
                if (_auctionBaseInfo.num > 0)
                    singlePrice = (ulong)_auctionBaseInfo.price / (ulong)_auctionBaseInfo.num;

                itemPriceValueLabel.text = Utility.ToThousandsSeparator(singlePrice);
            }
        }
        
        public void OnItemRecycle()
        {
            ResetData();            
        }

        private void OnShowOtherPlayerItemTip(GameObject obj, ItemData itemData)
        {
            AuctionNewDataManager.GetInstance().OnShowItemDetailTipFrame(itemData,
                itemData.GUID);
        }

    }
}