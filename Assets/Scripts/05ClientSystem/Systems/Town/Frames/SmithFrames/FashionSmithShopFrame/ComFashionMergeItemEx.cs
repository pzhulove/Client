using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameClient
{
    class ComFashionMergeItemEx : MonoBehaviour
    {
        public GameObject goItemParent;
        public Text Name;
        public Text Atrribute;
        public GameObject goCheckMark;
        ComItem comItem;
        public ItemData ItemData
        {
            get
            {
                return comItem == null ? null : comItem.ItemData;
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(false);
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
            if(null != itemData)
            {
                if(null != Name)
                {
                    Name.text = itemData.GetColorName();
                }
                if(null != Atrribute)
                {
                    Atrribute.text = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(itemData.FashionAttributeID);
                }
                if(null == comItem)
                {
                    comItem = ComItemManager.Create(goItemParent);
                }
                if(null != comItem)
                {
                    comItem.Setup(itemData, OnItemClicked);
                }
                gameObject.name = itemData.TableID.ToString();
            }
        }

        public static void LoadAllEquipments(ref List<ItemData> kEquipments, ProtoTable.ItemTable.eSubType eMergeType, bool bLocationLeft,bool needBlue = true)
        {
            kEquipments.Clear();

            var itemids = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.FASHION);
            for (int i = 0; i < itemids.Count; ++i)
            {
                var itemData = _TryAddMergeFashionItem(itemids[i]);
                if (itemData != null)
                {
                    kEquipments.Add(itemData);
                }
            }

            for (int i = 0; i < kEquipments.Count; ++i)
            {
                var equip = kEquipments[i];
                if (null == equip)
                {
                    kEquipments.RemoveAt(i--);
                    continue;
                }

                if (!bLocationLeft)
                {
                    if (ComFashionMergeDataBinder.SLValue != null &&
                    ComFashionMergeDataBinder.SLValue.GUID == equip.GUID && equip.Count <= 1)
                    {
                        kEquipments.RemoveAt(i--);
                        continue;
                    }
                }
                else
                {
                    if (ComFashionMergeDataBinder.SRValue != null &&
                    ComFashionMergeDataBinder.SRValue.GUID == equip.GUID && equip.Count <= 1)
                    {
                        kEquipments.RemoveAt(i--);
                        continue;
                    }
                }

                if ((int)eMergeType != equip.SubType)
                {
                    kEquipments.RemoveAt(i--);
                    continue;
                }
                if(!needBlue)
                {
                    if(equip.Quality == ProtoTable.ItemTable.eColor.BLUE)
                    {
                        kEquipments.RemoveAt(i--);
                        continue;
                    }
                }
            }

            kEquipments.Sort(_SortFashion);
        }

        public static ItemData _TryAddMergeFashionItem(ulong guid)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData == null || itemData.Type != ProtoTable.ItemTable.eType.FASHION ||
                itemData.PackageType != EPackageType.Fashion)
            {
                return null;
            }

            if (itemData.bFashionItemLocked)
            {
                return null;
            }

            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemData.TableID);
            if(null == item || item.TimeLeft > 0 && itemData.DeadTimestamp != 0)
            {
                return null;
            }

            //如果是天穹守护者套不参与合成
            if (item.SuitID == 101139)
            {
                return null;
            }
            
            if (FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                if (item.Color == ProtoTable.ItemTable.eColor.BLUE)
                {
                    return null;
                }
            }

            if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                var specialItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(itemData.TableID);
                if (specialItem != null)
                {
                    if (specialItem.Type < (int)FashionType.FT_NATIONALDAY)
                    {
                        return null;
                    }
                }
            }
            else
            {
                var specialItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(itemData.TableID);
                if (specialItem != null)
                {
                    return null;
                }
            }
               
            //wind is not allowed to merge
            if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.FASHION_HAIR)
            {
                return null;
            }

            if (itemData.Quality == ProtoTable.ItemTable.eColor.PINK)
            {
                return null;
            }

            return itemData;
        }

        public static int _SortFashion(ItemData left, ItemData right)
        {
            // 被锁住的道具靠后显示
            if(left.bFashionItemLocked != right.bFashionItemLocked)
            {
                return left.bFashionItemLocked.CompareTo(right.bFashionItemLocked);
            }
            if (left.FashionWearSlotType != right.FashionWearSlotType)
            {
                return left.FashionWearSlotType - right.FashionWearSlotType;
            }

            if (left.Quality != right.Quality)
            {
                return left.Quality - right.Quality;
            }

            if(left.TableID != right.TableID)
            {
                return left.TableID - right.TableID;
            }

            return (int)(left.GUID - right.GUID);
        }

        void OnDestroy()
        {
            if (comItem != null)
            {
                ComItemManager.Destroy(comItem);
                comItem = null;
            }
        }
    }
}