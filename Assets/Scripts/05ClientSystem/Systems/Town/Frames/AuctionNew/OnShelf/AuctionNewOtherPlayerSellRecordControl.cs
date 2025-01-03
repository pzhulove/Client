using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{


    public class AuctionNewOtherPlayerSellRecordControl : MonoBehaviour
    {
        private List<AuctionTransaction> _sellRecordDataModelList = new List<AuctionTransaction>();

        [Space(15)]
        [HeaderAttribute("AuctionNewSellRecord")]
        [Space(5)]
        [SerializeField] private Text noItemTipLabel;
        [SerializeField] private ComUIListScriptEx sellRecordItemList;

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
            _sellRecordDataModelList.Clear();
        }
        
        private void BindEvents()
        {
            if (sellRecordItemList != null)
            {
                sellRecordItemList.Initialize();
                sellRecordItemList.onItemVisiable += OnSellRecordItemVisible;
                sellRecordItemList.OnItemRecycle += OnSellRecordItemRecycle;
            }
        }

        private void UnBindEvents()
        {
            if (sellRecordItemList != null)
            {
                sellRecordItemList.onItemVisiable -= OnSellRecordItemVisible;
                sellRecordItemList.OnItemRecycle -= OnSellRecordItemRecycle;
            }
        }

        //需要传递默认的参数
        public void Init()
        {
            InitBaseView();
        }

        public void OnEnableControl()
        {
            if (sellRecordItemList != null)
            {
                sellRecordItemList.ResetComUiListScriptEx();
            }

            UpdateSellRecordItemList();
        }

        private void InitBaseView()
        {
            if (noItemTipLabel != null)
                noItemTipLabel.text = TR.Value("auction_new_other_not_sell_record_label");

            UpdateSellRecordItemList();
        }

        public void UpdateSellRecordControl(AuctionTransaction[] auctionTransactions)
        {
            _sellRecordDataModelList.Clear();
            if (auctionTransactions != null && auctionTransactions.Length > 0)
            {
                for (var i = 0; i < auctionTransactions.Length; i++)
                {
                    if (auctionTransactions[i] != null)
                        _sellRecordDataModelList.Add(auctionTransactions[i]);
                }
            }
            UpdateSellRecordItemList();
        }

        private void UpdateSellRecordItemList()
        {
            var itemNumber = 0;
            if (_sellRecordDataModelList != null)
                itemNumber = _sellRecordDataModelList.Count;

            if (sellRecordItemList != null)
                sellRecordItemList.SetElementAmount(itemNumber);
        }

        private void OnSellRecordItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (sellRecordItemList == null)
                return;

            if (_sellRecordDataModelList == null || _sellRecordDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _sellRecordDataModelList.Count)
                return;

            var sellRecordDataModel = _sellRecordDataModelList[item.m_index];
            var sellRecordItem = item.GetComponent<AuctionNewOtherPlayerSellRecordItem>();

            if (sellRecordItem != null && sellRecordDataModel != null)
                sellRecordItem.InitItem(sellRecordDataModel);
        }

        private void OnSellRecordItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var sellRecordItem = item.GetComponent<AuctionNewOtherPlayerSellRecordItem>();
            if (sellRecordItem != null)
                sellRecordItem.OnItemRecycle();
        }
        
    }

}