using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{
    public class ShopNewMoneyView : MonoBehaviour
    {
        private int _shopId = 0;
        private List<int> _shopCostItemIdList = null;

        [SerializeField] private List<ComCommonConsume> shopNewConsumeItemList;

        public void InitShopNewMoney(int shopId)
        {
            _shopId = shopId;
            _shopCostItemIdList = ShopNewDataManager.GetInstance().GetShopCostItems(_shopId);

            if(_shopCostItemIdList == null || _shopCostItemIdList.Count <= 0)
                return;

            InitShopNewMoneyView();
        }

        public void InitShopNewMoneyByShopTab(int shopId, int shopTab)
        {
            _shopCostItemIdList = ShopNewDataManager.GetInstance().GetShopCostItemsByShopTab(shopId, shopTab);
            if (_shopCostItemIdList == null || _shopCostItemIdList.Count <= 0)
                return;

            InitShopNewMoneyView();
        }
        
        private void InitShopNewMoneyView()
        {

            if (shopNewConsumeItemList == null || shopNewConsumeItemList.Count <= 0)
                return;

            var costItemCount = _shopCostItemIdList.Count;
            int index =  0;
            for (; index < costItemCount; ++index)
            {
                var comitem = shopNewConsumeItemList[index];
                if (comitem != null)
                {
                    comitem.SetData(ComCommonConsume.eType.Item, ComCommonConsume.eCountType.Fatigue, _shopCostItemIdList[index]);
                    comitem.CustomActive(true);
                }
            }
            for (; index < shopNewConsumeItemList.Count; ++index)
            {
                shopNewConsumeItemList[index].CustomActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_shopCostItemIdList != null)
            {
                _shopCostItemIdList.Clear();
            }
        }

    }
}