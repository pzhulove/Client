using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ShopNewCostItem : MonoBehaviour
    {

        private int _costItemId;
        private int _costNumber;
        private int _vipDiscount;

        [Space(5)]
        [HeaderAttribute("NormalContent")]

        [SerializeField] private Image costItemImage;
        [SerializeField] private Text costValueText;

        public void InitCostItem(int costItemId, int costNumber, int vipDisCount = 100)
        {
            _costItemId = costItemId;
            _costNumber = costNumber;
            _vipDiscount = vipDisCount;

            UpdateCostItemImage();
        }

        private void UpdateCostItemImage()
        {
            if (costItemImage != null)
            {
                var equalTable = TableManager.GetInstance().GetTableItem<EqualItemTable>(_costItemId);
                if (equalTable != null 
                    && equalTable.EqualItemIDs != null
                    && equalTable.EqualItemIDs.Count > 0)
                {
                    var equalItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(equalTable.EqualItemIDs[0]);
                    if(equalItemTable != null)
                        ETCImageLoader.LoadSprite(ref costItemImage, equalItemTable.Icon);
                }
                else
                {
                    var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_costItemId);
                    if (itemTable != null)
                        ETCImageLoader.LoadSprite(ref costItemImage, itemTable.Icon);
                }
            }
        }

        public void UpdateCostItemValue()
        {
            var costNumber = _costNumber;
            costNumber = ShopNewUtility.GetRealCostValue(costNumber, _vipDiscount);

            if (costValueText != null)
            {
                costValueText.text = costNumber.ToString();

                var ownerItemCount = ItemDataManager.GetInstance().GetOwnedItemCount(_costItemId);
                if (ownerItemCount >= costNumber)
                    costValueText.color = Color.white;
                else
                    costValueText.color = Color.red;
            }
        }

        public void UpdateCostItemValueByBuyNumber(int buyNumber)
        {
            if (costValueText == null)
                return;

            var costNumber = _costNumber;
            costNumber = ShopNewUtility.GetRealCostValue(costNumber, _vipDiscount);
            
            var ownerItemCount = ItemDataManager.GetInstance().GetOwnedItemCount(_costItemId);
            var totalNeedNumber = costNumber * buyNumber;

            costValueText.text = totalNeedNumber.ToString();
            costValueText.color = ownerItemCount < totalNeedNumber
                ? Color.red
                : ShopNewDataManager.GetInstance().specialColor;
        }

    }
}
