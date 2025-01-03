using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnEquipJichengEmptyClick(bool isEquipJichengA);
    public class EquipJichengItem : MonoBehaviour
    {
        [SerializeField] private GameObject mEquipJichengRoot;
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mEquipJichengName;
        [SerializeField] private Text mEquipJichengAttr;

        [SerializeField] private bool IsEquipJichengA;

        private OnEquipJichengEmptyClick mOnEquipJichengEmptyClick;
        private ComItemNew mComItem;
      
        private void OnDestroy()
        {
            mOnEquipJichengEmptyClick = null;
            mComItem = null;
        }

        public void InitEquipJichengItem(OnEquipJichengEmptyClick onEquipJichengEmptyClick)
        {
            mOnEquipJichengEmptyClick = onEquipJichengEmptyClick;

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(null, null);
        }

        public void UpdateEquipJichengItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            var equipJichengItemData = ItemDataManager.CreateItemDataFromTable(itemData.TableID);
            if (equipJichengItemData == null)
            {
                return;
            }

            mEquipJichengRoot.CustomActive(true);

            if (mComItem != null)
            {
                mComItem.Setup(itemData, Utility.OnItemClicked);
            }

            if (mEquipJichengName != null)
            {
                mEquipJichengName.text = equipJichengItemData.GetColorName();
            }

            if (mEquipJichengAttr != null)
            {
                int strengthenLevel = itemData.StrengthenLevel;
                string strengthenLevelDesc = "强化（激化）等级：无";
                if(strengthenLevel > 0)
                {
                    strengthenLevelDesc = "强化（激化）等级：" + strengthenLevel + "级";
                }

                string beadDescs = itemData.GetBeadDescs1();
                if (beadDescs == string.Empty)
                {
                    beadDescs = "宝珠：无";
                }
                else
                {
                    beadDescs = "宝珠：" + beadDescs;
                }

                string magicDescs = itemData.GetMagicDescs();
                if (magicDescs == string.Empty)
                {
                    magicDescs = "附魔：无";
                }
                else
                {
                    magicDescs = "附魔：" + magicDescs;
                }

                string inscriptionAttrDesc = string.Empty;
                if(itemData.InscriptionHoles != null)
                {
                    for (int i = 0; i < itemData.InscriptionHoles.Count; i++)
                    {
                        var inscriptionHoleData = itemData.InscriptionHoles[i];
                        if (inscriptionHoleData == null)
                        {
                            continue;
                        }

                        if (inscriptionHoleData.IsOpenHole == false)
                        {
                            continue;
                        }

                        var mItemData = ItemDataManager.CreateItemDataFromTable(inscriptionHoleData.InscriptionId);
                        if (mItemData == null)
                        {
                            continue;
                        }
                        inscriptionAttrDesc = mItemData.GetInscriptionAttrDesc();
                        break;
                    }
                }
                if (inscriptionAttrDesc == string.Empty)
                {
                    inscriptionAttrDesc = "铭文：无";
                }
                else
                {
                    inscriptionAttrDesc = "铭文：" + inscriptionAttrDesc;
                }
                mEquipJichengAttr.text = strengthenLevelDesc + "\n" + beadDescs + "\n" + magicDescs + "\n" + inscriptionAttrDesc;
            }
        }

        public void OnEquipJichengEmptyClick()
        {
            Reset();

            if (mOnEquipJichengEmptyClick != null)
            {
                mOnEquipJichengEmptyClick.Invoke(IsEquipJichengA);
            }
        }

        public void Reset()
        {
            mEquipJichengRoot.CustomActive(false);

            if (mComItem != null)
            {
                mComItem.Setup(null, null);
            }

            if (mEquipJichengName != null)
            {
                mEquipJichengName.text = string.Empty;
            }

            if (mEquipJichengAttr != null)
            {
                mEquipJichengAttr.text = string.Empty;
            }
        }
    }
}