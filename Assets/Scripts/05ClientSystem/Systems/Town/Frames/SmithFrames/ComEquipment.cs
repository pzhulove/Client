using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    class ComEquipment : MonoBehaviour
    {
        public static ItemData ms_selected = null;
        public Text Name;
        public Text CardAttr;
        public GameObject equiptedMark;
        public GameObject goItemParent;
        public GameObject goCheckMark;
        public GameObject goCanBeSet;
        public GameObject goHasBeenSet;
        public GameObject goCardParent;
        public Image imageItemBack;
        ComItemNew comItem;
        ComItemNew mCardComItem;
        ItemData itemData;
        public ItemData ItemData
        {
            get
            {
                return itemData;
            }
        }
        
        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
        }

      
        public void OnItemVisible(ItemData itemData)
        {
            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(goItemParent);
            }

            this.itemData = itemData;
            comItem.Setup(itemData, Utility.OnItemClicked);
            Name.text = itemData.GetColorName();

            goCanBeSet.CustomActive(false);
            goHasBeenSet.CustomActive(false);

            if (itemData.mPrecEnchantmentCard != null)
            {
                var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(itemData.mPrecEnchantmentCard.iEnchantmentCardID);
                if (magicItem != null)
                {
                    ItemData cardItemData = ItemDataManager.CreateItemDataFromTable(magicItem.ID);
                    cardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel = itemData.mPrecEnchantmentCard.iEnchantmentCardLevel;
                    
                    goHasBeenSet.CustomActive(true);

                    if (mCardComItem == null)
                    {
                        mCardComItem = ComItemManager.CreateNew(goCardParent);
                    }

                    mCardComItem.Setup(cardItemData, Utility.OnItemClicked);

                    CardAttr.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(itemData.mPrecEnchantmentCard.iEnchantmentCardID, itemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                }
                else
                {
                    goCanBeSet.CustomActive(true);
                }
            }
        
            equiptedMark.CustomActive(itemData.PackageType == EPackageType.WearEquip);
            gameObject.name = itemData.TableID.ToString();
        }

        void OnDestroy()
        {
            if(comItem != null)
            {
                //comItem.imgBackGround.enabled = true;
                comItem = null;
            }
        }
    }
}