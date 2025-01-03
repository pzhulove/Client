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
    public enum AuctionNewOtherPlayerShelfItemType
    {
        None = 0,
        OnSale = 1,             //在售
        OnNotice = 2,           //公示
        SellRecord = 3,         //成交记录
    }

    public delegate void OnSendWorldAuctionQueryItemPriceListReq(byte auctionState);

    public delegate void OnSendWorldAuctionQueryItemTransListReq();             //查询最近交易记录

    public class AuctionNewOtherPlayerControl : MonoBehaviour
    {

        private bool _isTreasure = false;       //是否为珍品

        //消息发送的标志。消息只发送一次
        private bool _isOnNoticeAlreadySendReq = false;        //关注页签是否发送请求,只有在第一次点击的时候发送一次。true表示已经发送，不在发送

        //页签数据
        private ComControlData _tabData;
        private List<ComControlData> _tabsDataList = new List<ComControlData>();

        //其他玩家的数据
        private WorldAuctionQueryItemPricesRes _worldAuctionQueryItemPriceRes;
        private List<AuctionBaseInfo> _otherPlayerOnShelfDataList;
        //在售，公示
        private List<AuctionBaseInfo> _otherPlayerOnSaleDataList = new List<AuctionBaseInfo>();
        private List<AuctionBaseInfo> _otherPlayerOnNoticeDataList = new List<AuctionBaseInfo>();

        //请求在售和公示消息的回调
        private OnSendWorldAuctionQueryItemPriceListReq _onSendWorldAuctionQueryItemPriceListReq;
        //查询最近交易的回调
        private OnSendWorldAuctionQueryItemTransListReq _onSendWorldAuctionQueryItemTransListReq;

        //最近销售的控制器
        private AuctionNewOtherPlayerSellRecordControl _otherPlayerSellRecordControl;

        [Space(10)] [HeaderAttribute("tabControl")] [Space(5)]
        [SerializeField] private ComToggleControl tabControl;

        [Space(15)] [HeaderAttribute("NormalItemList")] [Space(15)]
        [SerializeField] private GameObject normalItemListRoot;
        [SerializeField] private ComUIListScript otherPlayerOnShelfItemList;
        [SerializeField] private Text otherPlayerNoItemTip;

        [Space(15)] [HeaderAttribute("SellRecordItemList")] [Space(15)]
        [SerializeField] private GameObject sellRecordItemListRoot;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (otherPlayerOnShelfItemList != null)
            {
                otherPlayerOnShelfItemList.Initialize();
                otherPlayerOnShelfItemList.onItemVisiable += OnOtherPlayerOnShelfItemVisible;
                otherPlayerOnShelfItemList.OnItemRecycle += OnOtherPlayerOnShelfItemRecycle;
            }
        }

        private void UnBindEvents()
        {
            if (otherPlayerOnShelfItemList != null)
            {
                otherPlayerOnShelfItemList.onItemVisiable -= OnOtherPlayerOnShelfItemVisible;
                otherPlayerOnShelfItemList.OnItemRecycle -= OnOtherPlayerOnShelfItemRecycle;
            }
        }

        private void ClearData()
        {
            _otherPlayerOnShelfDataList = null;
            _otherPlayerOnSaleDataList.Clear();
            _otherPlayerOnNoticeDataList.Clear();
            _tabsDataList.Clear();
            _worldAuctionQueryItemPriceRes = null;
            _isTreasure = false;
            _isOnNoticeAlreadySendReq = false;

            _otherPlayerSellRecordControl = null;

            _onSendWorldAuctionQueryItemPriceListReq = null;
            _onSendWorldAuctionQueryItemTransListReq = null;
        }

        //其他玩家的基本信息
        public void InitOtherPlayerControlBaseView(bool isTreasure,
            OnSendWorldAuctionQueryItemPriceListReq onSendWorldAuctionQueryItemPriceListReq = null,
            OnSendWorldAuctionQueryItemTransListReq onSendWorldAuctionQueryItemTransListReq = null)
        {
            _isTreasure = isTreasure;
            _onSendWorldAuctionQueryItemPriceListReq = onSendWorldAuctionQueryItemPriceListReq;
            _onSendWorldAuctionQueryItemTransListReq = onSendWorldAuctionQueryItemTransListReq;

            InitOnShelfOtherPlayerTabs();
        }

        //TabsList
        private void InitOnShelfOtherPlayerTabs()
        {
            _tabsDataList.Clear();

            var onSaleData = new ComControlData
            {
                Id = (int) AuctionNewOtherPlayerShelfItemType.OnSale,
                Name = TR.Value("auction_new_other_on_sale_label"),
                IsSelected = true,
            };
            _tabsDataList.Add(onSaleData);

            //珍品
            if (_isTreasure == true)
            {
                //添加公示页签
                var onNoticeData = new ComControlData
                {
                    Id = (int) AuctionNewOtherPlayerShelfItemType.OnNotice,
                    Name = TR.Value("auction_new_other_on_notice_label"),
                    IsSelected = false,
                };
                _tabsDataList.Add(onNoticeData);

                ////添加成交记录页签
                //var sellRecordData = new ComControlData
                //{
                //    Id = (int)AuctionNewOtherPlayerShelfItemType.SellRecord,
                //    Name = TR.Value("auction_new_other_sell_record_label"),
                //    IsSelected = false,
                //};
                //_tabsDataList.Add(sellRecordData);
            }

            if (tabControl != null)
                tabControl.InitComToggleControl(_tabsDataList,
                    OnTabClick);

        }

        private void OnTabClick(ComControlData comToggleData)
        {
            if (comToggleData == null)
                return;

            if (_tabData != null && _tabData.Id == comToggleData.Id)
                return;
            
            _tabData = comToggleData;

            DealWithTabClick();
        }

        //页签点击的时候，是否发送相应的消息
        private void DealWithTabClick()
        {
            if (_tabData == null)
                return;

            CommonUtility.UpdateGameObjectVisible(normalItemListRoot, false);
            CommonUtility.UpdateGameObjectVisible(sellRecordItemListRoot, false);

            //公示或者在售类型
            var curOtherPlayerShelfItemType = (AuctionNewOtherPlayerShelfItemType) _tabData.Id;
            if (curOtherPlayerShelfItemType == AuctionNewOtherPlayerShelfItemType.OnNotice
                || curOtherPlayerShelfItemType == AuctionNewOtherPlayerShelfItemType.OnSale)
            {
                CommonUtility.UpdateGameObjectVisible(normalItemListRoot, true);

                SetOnShelfNoItemTip();
                UpdateOtherPlayerOnShelfItemList();

                //只有公示页签第一次点击的时候，才发送消息，请求公示的商品
                if (curOtherPlayerShelfItemType == AuctionNewOtherPlayerShelfItemType.OnNotice)
                {
                    if (_isOnNoticeAlreadySendReq == false)
                    {
                        _isOnNoticeAlreadySendReq = true;
                        // 1：公示
                        if (_onSendWorldAuctionQueryItemPriceListReq != null)
                            _onSendWorldAuctionQueryItemPriceListReq((byte)AuctionGoodState.AGS_PUBLIC);
                    }
                }
                
            }
            else if(curOtherPlayerShelfItemType == AuctionNewOtherPlayerShelfItemType.SellRecord)
            {
                ////最近记录
                //CommonUtility.UpdateGameObjectVisible(sellRecordItemListRoot, true);

                ////第一次进行加载操作
                //if (_otherPlayerSellRecordControl == null)
                //{
                //    _otherPlayerSellRecordControl = LoadOtherPlayerSellRecordControl(sellRecordItemListRoot);
                //    if (_otherPlayerSellRecordControl != null)
                //    {
                //        _otherPlayerSellRecordControl.Init();
                //        //发送请求消息
                //        if (_onSendWorldAuctionQueryItemTransListReq != null)
                //            _onSendWorldAuctionQueryItemTransListReq();
                //        //发送消息
                //    }
                //}
                //else
                //{
                //    _otherPlayerSellRecordControl.OnEnableControl();
                //}
            }
        }

        //获得其他玩家的在售数据，并进行更新
        public void InitOtherPlayerOnSaleItemList(AuctionBaseInfo[] auctionItems)
        {
            if (auctionItems == null || auctionItems.Length <= 0)
                return;

            _otherPlayerOnSaleDataList.Clear();

            for (var i = 0; i < auctionItems.Length; i++)
            {
                var actionBaseInfo = auctionItems[i];
                if (actionBaseInfo != null)
                    _otherPlayerOnSaleDataList.Add(actionBaseInfo);
            }

            //按照单价进行排序
            AuctionNewUtility.SortItemListBySinglePrice(_otherPlayerOnSaleDataList);

            //如果页签是在售的页签，则更新相应的ItemList
            if (_tabData != null && _tabData.Id == (int) AuctionNewOtherPlayerShelfItemType.OnSale)
                UpdateOtherPlayerOnShelfItemList();
        }

        //获得其他玩家的公示数据，并进行更新
        public void InitOtherPlayerOnNoticeItemList(AuctionBaseInfo[] auctionItems)
        {
            if (auctionItems == null || auctionItems.Length <= 0)
                return;

            _otherPlayerOnNoticeDataList.Clear();

            for (var i = 0; i < auctionItems.Length; i++)
            {
                var actionBaseInfo = auctionItems[i];
                if (actionBaseInfo != null)
                    _otherPlayerOnNoticeDataList.Add(actionBaseInfo);
            }

            //按照单价进行排序
            AuctionNewUtility.SortItemListBySinglePrice(_otherPlayerOnNoticeDataList);

            //如果页签是在售的公示，则更新相应的ItemList
            if (_tabData != null && _tabData.Id == (int)AuctionNewOtherPlayerShelfItemType.OnNotice)
                UpdateOtherPlayerOnShelfItemList();
        }

        //获得其他玩家的成交记录，并进行更新
        public void InitOtherPlayerSellRecordItemList(AuctionTransaction[] auctionTransactions)
        {
            //if (_otherPlayerSellRecordControl != null)
            //    _otherPlayerSellRecordControl.UpdateSellRecordControl(auctionTransactions);
        }

        private void UpdateOtherPlayerOnShelfItemList()
        {
            //默认显示在售
            _otherPlayerOnShelfDataList = _otherPlayerOnSaleDataList;

            if (_tabData != null)
            {
                if (_tabData.Id == (int) AuctionNewOtherPlayerShelfItemType.OnNotice)
                {
                    //公示页签，显示公示的内容
                    _otherPlayerOnShelfDataList = _otherPlayerOnNoticeDataList;
                }
            }

            var onShelfItemNumber = 0;
            if (_otherPlayerOnShelfDataList != null)
                onShelfItemNumber = _otherPlayerOnShelfDataList.Count;

            if (otherPlayerOnShelfItemList != null)
                otherPlayerOnShelfItemList.SetElementAmount(onShelfItemNumber);
        }

        #region ItemListAction
        private void OnOtherPlayerOnShelfItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_otherPlayerOnShelfDataList == null
                || item.m_index < 0 || item.m_index >= _otherPlayerOnShelfDataList.Count)
                return;

            var auctionBaseInfo = _otherPlayerOnShelfDataList[item.m_index];
            var otherPlayerItem = item.GetComponent<AuctionNewOtherPlayerOnShelfItem>();

            if (otherPlayerItem != null)
                otherPlayerItem.InitItem(auctionBaseInfo);
        }

        private void OnOtherPlayerOnShelfItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var otherPlayerOnShelfItem = item.GetComponent<AuctionNewOtherPlayerOnShelfItem>();
            if (otherPlayerOnShelfItem != null)
                otherPlayerOnShelfItem.OnItemRecycle();
        }
        #endregion

        #region Helper

        //物品不存在的提示
        private void SetOnShelfNoItemTip()
        {
            //Item不存在的展示页签
            if (otherPlayerNoItemTip != null)
            {
                if ((AuctionNewOtherPlayerShelfItemType)_tabData.Id == AuctionNewOtherPlayerShelfItemType.OnNotice)
                {
                    otherPlayerNoItemTip.text = TR.Value("auction_new_other_not_on_notice_label");
                }
                else if ((AuctionNewOtherPlayerShelfItemType)_tabData.Id == AuctionNewOtherPlayerShelfItemType.OnSale)
                {
                    otherPlayerNoItemTip.text = TR.Value("auction_new_other_not_on_sale_label");
                }
            }
        }

        //加载最近销售的资源
        private AuctionNewOtherPlayerSellRecordControl LoadOtherPlayerSellRecordControl(GameObject sellRecordRoot)
        {
            if (sellRecordRoot == null)
                return null;

            var sellRecordViewPrefab = CommonUtility.LoadGameObject(sellRecordRoot);
            if (sellRecordViewPrefab == null)
                return null;

            var sellRecordControl = sellRecordViewPrefab.GetComponent<AuctionNewOtherPlayerSellRecordControl>();
            return sellRecordControl;
        }


        #endregion

    }
}