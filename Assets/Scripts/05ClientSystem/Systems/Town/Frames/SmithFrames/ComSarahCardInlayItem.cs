using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GameClient
{
    public class ComSarahCardInlayItem : MonoBehaviour
    {
        public static ItemData ms_selected = null;
        public GameObject goItemParent;
        public Text Name;
        public Text sarahText;
        public GameObject goCheckMark;
        public ScrollRect mScrollRect;
        [HeaderAttribute("是否显示宝珠置换次数")]
        [SerializeField]
        private bool mIsShowBeadReplaceNumber = false;
        ComItemNew comItem;

        private ItemData itemData;
        public ItemData ItemData
        {
            get
            {
                return itemData;
            }
        }

        public static int Sort(ItemData left,ItemData right)
        {
            if (left.Quality != right.Quality)
            {
                return (int)right.Quality - (int)left.Quality;
            }

            var leftTable = TableManager.GetInstance().GetTableItem<BeadTable>(left.TableID);
            var rightTable = TableManager.GetInstance().GetTableItem<BeadTable>(right.TableID);

            if (leftTable.Level != rightTable.Level)
            {
                return rightTable.Level - leftTable.Level;
            }

            return right.LevelLimit - left.LevelLimit;
        }

        public static ItemData _TryAddSarahCard(ulong guid)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData != null && itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
                itemData.SubType == (int)ProtoTable.ItemTable.eSubType.Bead && itemData.PackageType != EPackageType.Storage && itemData.PackageType != EPackageType.RoleStorage)
            {
                return itemData;
            }
            return null;
        }
        
        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
        }
        

        public void OnItemVisible(ItemData itemData)
        {
            this.itemData = itemData;
            var beadItem = TableManager.GetInstance().GetTableItem<ProtoTable.BeadTable>((int)itemData.TableID);
            Name.text = itemData.GetColorName();

            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(goItemParent);
            }

            comItem.Setup(itemData, Utility.OnItemClicked);
            gameObject.name = itemData.TableID.ToString();
            if (itemData.BeadAdditiveAttributeBuffID > 0)
            {
                sarahText.text = BeadCardManager.GetInstance().GetAttributesDesc(beadItem.ID) + "\n" +
                   string.Format("附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(itemData.BeadAdditiveAttributeBuffID));
            }
            else
            {
                sarahText.text = BeadCardManager.GetInstance().GetAttributesDesc(beadItem.ID);
            }

            ShowBeadReplaceRemainNumber(itemData);
            mScrollRect.verticalNormalizedPosition = 1;
        }
        public void ShowBeadReplaceRemainNumber(ItemData mItemData)
        {
            if (mIsShowBeadReplaceNumber)
            {
                string mReplaceNumberDes = BeadCardManager.GetInstance().GetBeadReplaceRemainNumber(mItemData.TableID, mItemData.BeadReplaceNumber);
                if (mReplaceNumberDes != "")
                {
                    sarahText.text += "\n" + mReplaceNumberDes;
                }
            }
        }
        void OnDestroy()
        {
            if (comItem != null)
            {
                comItem = null;
            }
        }
    }
}
