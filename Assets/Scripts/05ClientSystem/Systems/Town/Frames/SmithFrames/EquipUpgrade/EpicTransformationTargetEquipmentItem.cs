using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EpicTransformationTargetEquipmentItem : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mItemName;
        [SerializeField] private GameObject mCheckRoot;

        private ItemData equipItemData;
        public ItemData EquipmentItemData
        {
            get { return equipItemData; }
        }

        private ComItemNew equipmentComItem;

        public void OnItemVisiable(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            equipItemData = itemData;

            if (equipmentComItem == null)
            {
                equipmentComItem = ComItemManager.CreateNew(mItemParent);
            }

            equipmentComItem.Setup(equipItemData, Utility.OnItemClicked);

            if (mItemName != null)
            {
                mItemName.text = equipItemData.GetColorName();
            }
        }

        public void OnChangedDisplay(bool value)
        {
            if(mCheckRoot != null)
            {
                mCheckRoot.CustomActive(value);
            }
        }

        private void OnDestroy()
        {
            equipmentComItem = null;
            equipItemData = null;
        }
    }
}