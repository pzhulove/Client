using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenGrowthEquipItem : MonoBehaviour
    {
        [SerializeField]private Text mName;
        [SerializeField]private Text mAttrName;
        [SerializeField]private Text mAttrDescs;
        [SerializeField]private GameObject mItemParent;
        [SerializeField]private GameObject mEquipMark;
        [SerializeField]private GameObject mAttrRoot;
        [SerializeField]private GameObject mNameRoot;
        [SerializeField]private GameObject mCheckMark;

        private ComItemNew comItem;

        ItemData mItemData = null;
        public ItemData EquipItemData
        {
            get { return mItemData; }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            if (mCheckMark != null)
            {
                mCheckMark.CustomActive(bSelected);
            }
        }

        public void OnItemVisible(ItemData itemData, StrengthenGrowthType eStrengthenGrowthType)
        {
            if (itemData == null)
            {
                return;
            }

            mItemData = itemData;
            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(mItemParent);
            }

            mNameRoot.CustomActive(itemData.EquipType == EEquipType.ET_COMMON);
            mAttrRoot.CustomActive(itemData.EquipType != EEquipType.ET_COMMON);

            comItem.Setup(itemData, OnItemClicked);
            mName.text = mAttrName.text = itemData.GetColorName();

            if (mAttrDescs != null)
            {
                if (itemData.EquipType == EEquipType.ET_BREATH)
                {
                    mAttrDescs.text = TR.Value("growth_breath_des");
                }
                else if (itemData.EquipType == EEquipType.ET_REDMARK)
                {
                    mAttrDescs.text = TR.Value("growth_attr_des", EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(itemData.GrowthAttrType), itemData.GrowthAttrNum);
                }
            }

            mEquipMark.CustomActive(itemData.PackageType == EPackageType.WearEquip);
        }

        void OnItemClicked(GameObject obj, IItemDataModel item)
        {
            ItemTipManager.GetInstance().ShowTip(item as ItemData);
        }

        private void OnDestroy()
        {
            comItem = null;
        }
    }
}