using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EquipUpgradeItem : MonoBehaviour
    {
        public static ItemData ms_selected = null;

        [SerializeField]
        private GameObject mItemParent;

        [SerializeField]
        private Text mName;
        
        [SerializeField]
        private GameObject mGoCheckMark;

        [SerializeField]
        private GameObject mEquiptedMark;
        [SerializeField]private UIGray mGrayObj;

        private ItemData itemData;

        public ItemData ItemData
        {
            get { return itemData; }
            set { itemData = value; }
        }

        ComItemNew comItem = null;

        public void OnItemVisiable(ItemData item)
        {
            this.itemData = item;

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemParent);
            }

            comItem.Setup(itemData, Utility.OnItemClicked);

            if (mName != null)
                mName.text = itemData.GetColorName();

            //穿在身上或者在未启用的方案中
            bool isSetMark = (itemData.PackageType == EPackageType.WearEquip);
            if (mEquiptedMark != null)
                mEquiptedMark.CustomActive(isSetMark);

            if (mGrayObj != null)
                mGrayObj.enabled = false;

            bool isSetGray = (itemData.PackageType == EPackageType.WearEquip)
                             || (itemData.IsItemInUnUsedEquipPlan == true);
            if (mGrayObj != null)
                mGrayObj.enabled = isSetGray;
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            if(mGoCheckMark != null)
                mGoCheckMark.CustomActive(bSelected);
        }

        void OnDestroy()
        {
            comItem = null;
            itemData = null;
        }
    }
}

