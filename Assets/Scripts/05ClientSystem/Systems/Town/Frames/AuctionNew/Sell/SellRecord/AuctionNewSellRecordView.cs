using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{


    public class AuctionNewSellRecordView : MonoBehaviour
    {

        private List<AuctionTransaction> _treasureSellRecordList;
        private List<AuctionTransaction> _treasureBuyRecordList;

        [Space(5)]
        [HeaderAttribute("Title")]
        [SerializeField] private Text titleLabel = null;

        [Space(5)]
        [HeaderAttribute("Button")]
        [SerializeField] private Button closeButton;

        [Space(15)] [HeaderAttribute("AuctionNewSellRecord")]
        [Space(5)]
        [SerializeField] private Text sellItemLabel;
        [SerializeField] private Text getTimeLabel;
        [SerializeField] private Text zeroSellItemTips;
        [SerializeField] private ComUIListScript treasureItemSellList;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ResetData();
        }

        private void ResetData()
        {
            _treasureBuyRecordList = null;
            _treasureSellRecordList = null;
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewGetTreasureTransactionRecordSucceed,
                OnReceiveTreasureItemTransactionRes);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnAuctionNewGetTreasureTransactionRecordSucceed,
                OnReceiveTreasureItemTransactionRes);
        }

        private void BindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            if (treasureItemSellList != null)
            {
                treasureItemSellList.Initialize();
                treasureItemSellList.onItemVisiable += OnTreasureItemSellVisible;
                treasureItemSellList.OnItemRecycle += OnTreasureItemSellRecycle;
            }

        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (treasureItemSellList != null)
            {
                treasureItemSellList.onItemVisiable -= OnTreasureItemSellVisible;
                treasureItemSellList.OnItemRecycle -= OnTreasureItemSellRecycle;
            }

        }

        //需要传递默认的参数
        public void InitView()
        {
            InitBaseView();
            SendTreasureItemTransactionReq();
        }

        private void InitBaseView()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("auction_new_sell_record");

            if (sellItemLabel != null)
                sellItemLabel.text = TR.Value("auction_new_sell_record_sell_item");

            if (getTimeLabel != null)
                getTimeLabel.text = TR.Value("auction_new_sell_record_get_time");

            if (zeroSellItemTips != null)
                zeroSellItemTips.text = TR.Value("auction_new_sell_record_zero_sell_item");

        }

        private void SendTreasureItemTransactionReq()
        {
            //发送拉去交易记录的信息
            AuctionNewDataManager.GetInstance().SendAuctionNewTreasureTransactionReq();
        }

        private void OnReceiveTreasureItemTransactionRes(UIEvent uiEvent)
        {
            _treasureBuyRecordList = AuctionNewDataManager.GetInstance().GetTreasureItemBuyRecordList();
            _treasureSellRecordList = AuctionNewDataManager.GetInstance().GetTreasureItemSaleRecordList();

            if (treasureItemSellList != null)
            {
                if (_treasureSellRecordList != null && _treasureSellRecordList.Count > 0)
                {
                    treasureItemSellList.SetElementAmount(_treasureSellRecordList.Count);
                }
                else
                {
                    treasureItemSellList.SetElementAmount(0);
                }
            }
        }

        private void OnTreasureItemBuyVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if(_treasureBuyRecordList == null)
                return;

            if(item.m_index < 0 || item.m_index >= _treasureBuyRecordList.Count)
                return;

            var auctionTransaction = _treasureBuyRecordList[item.m_index];
            var treasureRecordItem = item.GetComponent<AuctionNewTreasureRecordItem>();

            if (treasureRecordItem != null && auctionTransaction != null)
                treasureRecordItem.InitItem(auctionTransaction,false);
        }

        private void OnTreasureItemBuyRecycle(ComUIListElementScript item)
        {
            if(item == null)
                return;

            var treasureRecordItem = item.GetComponent<AuctionNewTreasureRecordItem>();
            if(treasureRecordItem != null)
                treasureRecordItem.OnItemRecycle();
        }

        private void OnTreasureItemSellVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_treasureSellRecordList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _treasureSellRecordList.Count)
                return;

            var auctionTransaction = _treasureSellRecordList[item.m_index];
            var treasureRecordItem = item.GetComponent<AuctionNewTreasureRecordItem>();

            if (treasureRecordItem != null && auctionTransaction != null)
                treasureRecordItem.InitItem(auctionTransaction);
        }

        private void OnTreasureItemSellRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var treasureRecordItem = item.GetComponent<AuctionNewTreasureRecordItem>();
            if (treasureRecordItem != null)
                treasureRecordItem.OnItemRecycle();
        }        

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<AuctionNewSellRecordFrame>();
        }

    }
}