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

    public delegate void OnAuctionNewOnSaleItemClick(AuctionItemBaseInfo auctionItemBaseInfo);

    //分为在售和公示两个部分
    public class AuctionNewBuyNoticeOnSaleItem : MonoBehaviour
    {
        private AuctionNewMainTabType _mainTabType;
        private AuctionItemBaseInfo _auctionItemBaseInfo;
        private uint _onSaleItemId;
        private ComItem _onSaleComItem;
        private OnAuctionNewOnSaleItemClick _onAuctionNewOnSaleItemClick;

        [SerializeField] private GameObject itemIconRoot;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemNumberLabel;
        [SerializeField] private Button onSaleItemButton;

        private void Awake()
        {
            BindEvents();
        }

        private void BindEvents()
        {
            if (onSaleItemButton != null)
            {
                onSaleItemButton.onClick.RemoveAllListeners();
                onSaleItemButton.onClick.AddListener(OnOnSaleItemButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (onSaleItemButton != null)
            {
                onSaleItemButton.onClick.RemoveAllListeners();
            }
        }

        private void OnDestroy()
        {
            if (_onSaleComItem != null)
            {
                ComItemManager.Destroy(_onSaleComItem);
                _onSaleComItem = null;
            }

            ResetData();
            UnBindEvents();
        }

        private void ResetData()
        {
            _onSaleItemId = 0;
            _onAuctionNewOnSaleItemClick = null;
            _mainTabType = AuctionNewMainTabType.None;
            _auctionItemBaseInfo = null;
        }

        public void InitItem(AuctionNewMainTabType mainTabType,
            AuctionItemBaseInfo auctionItemBaseInfo,
            OnAuctionNewOnSaleItemClick onAuctionNewOnSaleItemClick = null)
        {
            _mainTabType = mainTabType;
            _auctionItemBaseInfo = auctionItemBaseInfo;
            _onAuctionNewOnSaleItemClick = onAuctionNewOnSaleItemClick;

            InitItemView();
        }

        private void InitItemView()
        {
            //名字
            var itemId = (int)_auctionItemBaseInfo.itemTypeId;
            var itemNumber = (int) _auctionItemBaseInfo.num;
            var itemData = ItemDataManager.CreateItemDataFromTable(itemId);
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)itemId);
            if (itemData == null || itemTable == null)
            {
                if (itemName != null)
                    itemName.text = TR.Value("auction_new_onSale_itemError");
                return;
            }

            itemData.IsTreasure = _auctionItemBaseInfo.isTreas == 1 ? true : false;

            if (itemName != null)
                itemName.text = AuctionNewUtility.GetQualityColorString(itemData.Name, itemTable.Color);

            var saleStr = string.Format(TR.Value("auction_new_onSale_sell_number"), itemNumber);
            if (_mainTabType == AuctionNewMainTabType.AuctionNoticeType)
                saleStr = string.Format(TR.Value("auction_new_onSale_notice_number"), itemNumber);

            if (itemNumberLabel != null)
                itemNumberLabel.text = saleStr;

            if (itemIconRoot != null)
            {
                _onSaleComItem = itemIconRoot.GetComponentInChildren<ComItem>();
                if (_onSaleComItem == null)
                    _onSaleComItem = ComItemManager.Create(itemIconRoot);

                if (_onSaleComItem != null)
                {
                    _onSaleComItem.Setup(itemData, OnShowItemTip);
                    var isTreasure = _auctionItemBaseInfo.isTreas == 1 ? true : false;
                    _onSaleComItem.SetShowTreasure(isTreasure);
                }
            }
        }

        //回收的时候，elementItem重置，点击无效
        public void OnItemRecycle()
        {
            ResetData();
        }

        private void OnOnSaleItemButtonClick()
        {
            if(_auctionItemBaseInfo == null)
                return;

            if(_auctionItemBaseInfo.itemTypeId <= 0)
                return;

            //数量为0，不可点击，或者飘字提示
            if(_auctionItemBaseInfo.num <= 0)
                return;

            if (_onAuctionNewOnSaleItemClick == null)
                return;

            _onAuctionNewOnSaleItemClick(_auctionItemBaseInfo);
        }

        private void OnShowItemTip(GameObject obj, ItemData itemData)
        {
            AuctionNewDataManager.GetInstance().OnShowItemDetailTipFrame(itemData);
        }

    }
}