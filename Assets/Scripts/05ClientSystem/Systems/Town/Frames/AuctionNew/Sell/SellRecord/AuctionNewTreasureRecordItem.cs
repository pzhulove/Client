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
    public class AuctionNewTreasureRecordItem : MonoBehaviour
    {

        private bool _isSellRecord = true;
        private AuctionTransaction _auctionTransaction = null;

        private ComItem _elementComItem;
        private float _baseIntervalTime = 0.0f;

        [Space(10)]
        [HeaderAttribute("ItemContent")]
        [Space(5)]
        [SerializeField] private GameObject itemRoot;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemScore;
        [SerializeField] private Text itemPriceLabel;
        [SerializeField] private Text itemPriceValue;
        [SerializeField] private Text itemGetTimeLabel;
        [SerializeField] private CountDownTimeController countDownTimeController;
        [SerializeField] private Text itemGetFlagLabel;

        private void OnDestroy()
        {
            if (_elementComItem != null)
            {
                ComItemManager.Destroy(_elementComItem);
                _elementComItem = null;
            }

            _auctionTransaction = null;
        }

        public void InitItem(AuctionTransaction auctionTransaction, bool isSellRecord = true)
        {
            _auctionTransaction = auctionTransaction;
            _isSellRecord = isSellRecord;

            if (_auctionTransaction == null)
                return;

            InitItemView();
        }

        private void InitItemView()
        {
            var itemData = ItemDataManager.CreateItemDataFromTable((int) _auctionTransaction.item_id);
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int) _auctionTransaction.item_id);
            if (itemData == null || itemTable == null)
            {
                Logger.LogErrorFormat("TreasureItemData is null and itemId is {0}", _auctionTransaction.item_id);
                return;
            }

            itemData.Count = (int) _auctionTransaction.item_num;
            if (itemName != null)
                itemName.text = AuctionNewUtility.GetQualityColorString(itemData.Name, itemTable.Color);

            if (itemRoot != null)
            {
                _elementComItem = itemRoot.GetComponentInChildren<ComItem>();
                if (_elementComItem == null)
                    _elementComItem = ComItemManager.Create(itemRoot);

                if (_elementComItem != null)
                {
                    _elementComItem.Setup(itemData, OnShowItemTip);
                    _elementComItem.SetShowTreasure(true);
                }
            }

            if (itemScore != null)
            {
                if (_auctionTransaction.item_score <= 0)
                {
                    itemScore.gameObject.CustomActive(false);
                }
                else
                {
                    itemScore.gameObject.CustomActive(true);
                    itemScore.text = string.Format(TR.Value("auction_new_itemDetail_score_value"),
                        _auctionTransaction.item_score);
                }
            }

            if (itemPriceLabel != null)
                itemPriceLabel.text = TR.Value("auction_new_sell_item_single_price");

            if (itemPriceValue != null)
            {
                var singlePrice = (ulong) _auctionTransaction.unit_price;

                itemPriceValue.text = Utility.GetShowPrice(singlePrice);
                itemPriceValue.text = Utility.ToThousandsSeparator(singlePrice);
            }

            //出售记录
            if (_isSellRecord == true)
            {
                ResetItemGetLabel();
                if (TimeManager.GetInstance().GetServerTime() > _auctionTransaction.verify_end_time)
                {
                    if (itemGetFlagLabel != null)
                        itemGetFlagLabel.gameObject.CustomActive(true);
                }
                else
                {
                    if (itemGetTimeLabel != null)
                    {
                        itemGetTimeLabel.gameObject.CustomActive(true);
                        itemGetTimeLabel.text = CountDownTimeUtility.GetCountDownTimeByHourMinute(
                            _auctionTransaction.verify_end_time,
                            TimeManager.GetInstance().GetServerTime());

                        if (countDownTimeController != null)
                        {
                            countDownTimeController.EndTime = _auctionTransaction.verify_end_time;
                            countDownTimeController.InitCountDownTimeController();
                        }
                    }
                }
            }
            else
            {
                ResetItemGetLabel();
            }
        }

        private void ResetItemGetLabel()
        {
            if (itemGetFlagLabel != null)
                itemGetFlagLabel.gameObject.CustomActive(false);
            if (itemGetTimeLabel != null)
                itemGetTimeLabel.gameObject.CustomActive(false);
        }

        public void OnItemRecycle()
        {
            _auctionTransaction = null;
            if(countDownTimeController != null)
                countDownTimeController.ResetCountDownTimeController();
        }

        private void OnShowItemTip(GameObject obj, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTip(itemData);
        }
        

    }
}