using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    class ComMagicCardEnchantItem : MonoBehaviour
    {
        public static ItemData ms_selected = null;
        public GameObject goItemParent;
        public Text Name;
        public Text magicText;
        public GameObject goCheckMark;
        ComItem comItem;
        public ItemData ItemData
        {
            get
            {
                return comItem == null ? null : comItem.ItemData;
            }
        }

        public static int Sort(ItemData left, ItemData right)
        {
            if (left.Quality != right.Quality)
            {
                return (int)right.Quality - (int)left.Quality;
            }

            return right.LevelLimit - left.LevelLimit;
        }

        public static ItemData _TryAddMagicCard(ulong guid)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData != null && itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
                itemData.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard &&
                itemData.PackageType != EPackageType.Storage && itemData.PackageType != EPackageType.RoleStorage)
            {
                return itemData;
            }
            return null;
        }

        public void OnSelectedItem()
        {
            if(null != comItem && null != comItem.ItemData)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEnchantCardSelected, comItem.ItemData);
            }
        }
        
        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
        }

        void OnItemClicked(GameObject obj, ItemData item)
        {
            if (item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        public void OnItemVisible(ItemData itemData)
        {
            var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>((int)itemData.TableID);
            Name.text = itemData.GetColorName();
           
            if (comItem == null)
            {
                comItem = ComItemManager.Create(goItemParent);
            }
            comItem.Setup(itemData, OnItemClicked);
            gameObject.name = itemData.TableID.ToString();
            magicText.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(itemData.TableID,itemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
        }

        void OnDestroy()
        {
            if(comItem != null)
            {
                comItem = null;
            }
        }
    }
}