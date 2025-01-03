using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ShopNewSpecialPriceControl : MonoBehaviour
    {

        [Space(10)] [HeaderAttribute("SpecialPriceItem")] [Space(10)] [SerializeField]
        private List<ShopNewCostItem> shopNewCostItemList = new List<ShopNewCostItem>();

        //货币数量不少于3个才有效
        public void InitSpecialPriceControl(List<ShopNewCostItemDataModel> costItemDataModelList,
            int vipDisCount = 100,
            int curBuyNumber = 1)
        {
            if (costItemDataModelList == null || costItemDataModelList.Count <=0)
                return;

            ResetCostItem();

            //取最小的数量
            var costItemNumber = shopNewCostItemList.Count >= costItemDataModelList.Count
                ? costItemDataModelList.Count
                : shopNewCostItemList.Count;

            for (var i = 0; i < costItemNumber; i++)
            {
                var costItem = shopNewCostItemList[i];
                var costItemData = costItemDataModelList[i];
                if(costItem == null || costItemData == null)
                    continue;

                CommonUtility.UpdateGameObjectVisible(costItem.gameObject, true);
                costItem.InitCostItem(costItemData.CostItemId, costItemData.CostItemNumber, vipDisCount);
                if (curBuyNumber > 1)
                {
                    costItem.UpdateCostItemValueByBuyNumber(curBuyNumber);
                }
                else
                {
                    costItem.UpdateCostItemValue();
                }
            }
        }

        private void ResetCostItem()
        {
            if (shopNewCostItemList == null || shopNewCostItemList.Count <= 0)
                return;

            for (var i = 0; i < shopNewCostItemList.Count; i++)
            {
                if (shopNewCostItemList[i] != null)
                    CommonUtility.UpdateGameObjectVisible(shopNewCostItemList[i].gameObject, false);
            }
        }
        
        public void UpdateCostItemListValue()
        {
            for (var i = 0; i < shopNewCostItemList.Count; i++)
            {
                var costItem = shopNewCostItemList[i];
                if (costItem != null && costItem.gameObject.activeSelf == true)
                {
                    costItem.UpdateCostItemValue();
                }
            }
        }

        public void UpdateCostItemListValueByNumber(int buyNumber)
        {
            for (var i = 0; i < shopNewCostItemList.Count; i++)
            {
                var costItem = shopNewCostItemList[i];
                if (costItem != null && costItem.gameObject.activeSelf == true)
                    costItem.UpdateCostItemValueByBuyNumber(buyNumber);
            }
        }
        
    }
}
