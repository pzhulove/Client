using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;


namespace GameClient
{

    public class AuctionNewForSellElementItem : MonoBehaviour
    {
        private AuctionSellItemData _auctionSellItemData;
        private ItemData _itemData;
        private ulong _elementItemId;
        private ComItem _elementComItem;
        private bool _isInCoolTime = false;

        private bool _isItemTradeLimit = false;
        private int _itemTradeLeftTime = 0;

        [SerializeField] private GameObject itemIcon;
        [SerializeField] private Button elementItemButton;
        [SerializeField] private UIGray itemUiGray;

        private void Awake()
        {
            BindEvents();
        }

        private void BindEvents()
        {
            if (elementItemButton != null)
            {
                elementItemButton.onClick.RemoveAllListeners();
                elementItemButton.onClick.AddListener(OnElementItemButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (elementItemButton != null)
            {
                elementItemButton.onClick.RemoveAllListeners();
            }
        }

        private void OnDestroy()
        {
            if (_elementComItem != null)
            {
                ComItemManager.Destroy(_elementComItem);
                _elementComItem = null;
            }

            ResetData();
            UnBindEvents();
        }

        private void ResetData()
        {
            _elementItemId = 0;
            _auctionSellItemData = null;
            _itemData = null;
            _isInCoolTime = false;

            _isItemTradeLimit = false;
            _itemTradeLeftTime = 0;
        }

        public void InitItem(AuctionSellItemData auctionSellItemData)
        {
            _auctionSellItemData = auctionSellItemData;

            InitView();
        }

        private void InitView()
        {

            _itemData = ItemDataManager.GetInstance().GetItem(_auctionSellItemData.uId);
            if (_itemData == null)
            {
                Logger.LogErrorFormat("itemData is null and auctionSellItemData is {0}", _auctionSellItemData.uId);
                return;
            }

            AuctionNewDataManager.GetInstance().AddItemDataDetailInfo(_auctionSellItemData.uId, _itemData);

            _isInCoolTime = AuctionNewUtility.IsAuctionNewItemInCoolTime(_itemData);

            //得到物品交易的限制
            _isItemTradeLimit = ItemDataUtility.IsItemTradeLimitBuyNumber(_itemData);
            _itemTradeLeftTime = ItemDataUtility.GetItemTradeLeftTime(_itemData);

            _elementComItem = itemIcon.GetComponentInChildren<ComItem>();
            if (_elementComItem == null)
            {
                _elementComItem = ComItemManager.Create(itemIcon);
            }

            if (_elementComItem != null)
            {
                _elementComItem.Setup(_itemData, null);
                _elementComItem.SetShowTreasure(_itemData.IsTreasure);
            }

            if (itemUiGray != null)
            {
                if (_isInCoolTime == true)
                {
                    itemUiGray.enabled = true;
                }
                else
                {
                    itemUiGray.enabled = false;
                }

                //交易受到限制
                if (_isItemTradeLimit == true)
                {
                    //剩余次数为0;
                    if(_itemTradeLeftTime == 0)
                    {
                        //设置为灰色
                        itemUiGray.enabled = true;
                    }
                }
            }
        }


        //回收的时候，elementItem重置，点击无效
        public void OnItemRecycle()
        {
            ResetData();
        }

        private void OnElementItemButtonClick()
        {
            if(_auctionSellItemData == null)
                return;

            if(_itemData == null)
                return;
            
            AuctionNewUtility.CloseAuctionNewOnShelfFrame();


            if (_itemData.IsNeedPacking() == true)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("Auction_item_packing"),
                    OnClickOnPacking);
            }
            else
            {

                if (_isInCoolTime == true)
                {
                    AuctionNewUtility.ShowItemInCoolTimeFloatingEffect(_itemData.AuctionCoolTimeStamp,
                        TimeManager.GetInstance().GetServerTime());
                    return;
                }

                //存在交易限制
                if (_isItemTradeLimit == true)
                {
                    //剩余次数为0；
                    if (_itemTradeLeftTime <= 0)
                    {
                        //飘字
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_item__trade_number_zero"));
                        return;
                    }
                    else
                    {
                        //弹窗
                        var contentStr = string.Format(TR.Value("auction_new_item_on_shelf_with_trade_number"),
                            _itemTradeLeftTime);

                        AuctionNewUtility.OnShowItemTradeLimitFrame(contentStr,
                            OnOpenAuctionNewOnShelfFrame);
                        return;
                    }
                }

                //打开上架界面
                OnOpenAuctionNewOnShelfFrame();
            }
        }

        private void OnOpenAuctionNewOnShelfFrame()
        {
            AuctionNewUtility.OpenAuctionNewOnShelfFrame(
                _auctionSellItemData.uId,
                _itemData.IsTreasure,
                AuctionNewDataManager.GetInstance().BaseShelfNum + PlayerBaseData.GetInstance().AddAuctionFieldsNum,
                AuctionNewDataManager.GetInstance().OnShelfItemNumber);

        }

        //解封
        private void OnClickOnPacking()
        {
            AuctionNewDataManager.GetInstance().OnClickOnPacking(_itemData);
        }

    }
}