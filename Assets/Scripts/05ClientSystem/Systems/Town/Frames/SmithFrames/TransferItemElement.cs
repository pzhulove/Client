using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TransferItemElement : MonoBehaviour
    {
        [SerializeField] private GameObject itemParent;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemCount;
        [SerializeField] private GameObject itemCheckMark;

        private ComItemNew comItemNew;

        private ItemData itemData;
        public ItemData ItemData
        {
            get { return itemData; }
        }
        public void OnItemVisiable(ItemData itemData)
        {
            if (itemData == null)
                return;

            this.itemData = itemData;

            if (comItemNew == null)
                comItemNew = ComItemManager.CreateNew(itemParent);

            comItemNew.Setup(itemData, Utility.OnItemClicked);

            if(itemName != null)
            {
                itemName.text = itemData.GetColorName();
            }

            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)itemData.TableID);
            int iCostCount = (itemData == null) ? 1 : itemData.Count;

            if (null != itemCount)
            {
                itemCount.text = string.Format("{0}/{1}", iHasCount, iCostCount);
                itemCount.color = iHasCount >= iCostCount ? Color.white : Color.red;
            }
        }

        public void OnChangedDisplay(bool value)
        {
            if (itemCheckMark != null)
                itemCheckMark.CustomActive(value);
        }

        private void OnDestroy()
        {
            comItemNew = null;
            itemData = null;
        }
    }
}