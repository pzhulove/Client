using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
///////删除linq
using ProtoTable;
using UnityEngine.UI;
using System;
using Protocol;

namespace GameClient
{
    class ComSarahCardInlayItemList
    {
        bool bInitialize = false;
        public bool Initilized
        {
            get
            {
                return bInitialize;
            }
        }
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        public delegate void OnItemSelected(ItemData itemData);
        public OnItemSelected onItemSelected;
        List<ItemData> equipments = new List<ItemData>();
        public ItemData equipItemData = null;

        public ItemData Selected
        {
            get
            {
                return ComSarahCardInlayItem.ms_selected;
            }
            set
            {
                ComSarahCardInlayItem.ms_selected = value;
            }
        }

        ComSarahCardInlayItem _OnBindItemDelegate(GameObject itemObject)
        {
            ComSarahCardInlayItem comSarahCardInlayItem = itemObject.GetComponent<ComSarahCardInlayItem>();
            return comSarahCardInlayItem;
        }

        public bool HasEquipments()
        {
            return equipments.Count > 0;
        }

        public bool HasObject(ulong guid)
        {
            for (int i = 0; i < equipments.Count; ++i)
            {
                if (equipments[i] != null && equipments[i].GUID == guid)
                {
                    return true;
                }
            }

            return false;
        }

        public int FindObject(ulong guid)
        {
            for (int i = 0; i < equipments.Count; i++)
            {
                if (equipments[i]!= null && equipments[i].GUID == guid)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Initialize(GameObject gameObject, 
            OnItemSelected onItemSelected,
            ItemData data)
        {
            if (bInitialize)
            {
                return;
            }
            bInitialize = true;

            Selected = null;
            this.gameObject = gameObject;
            this.onItemSelected = onItemSelected;
            equipItemData = data;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();
            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            comUIListScript.onItemSelected += _OnItemSelectedDelegate;
            comUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;

            _LoadAllEquipments(ref equipments);
        }

        public void RefreshAllEquipments(ItemData itemData, int beadId = 0)
        {
            equipItemData = itemData;
            if (Initilized)
            {
                _LoadAllEquipments(ref equipments, beadId);
                this.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
            }
        }

        void _LoadAllEquipments(ref List<ItemData> kEquipments, int beadId = 0)
        {
            kEquipments.Clear();

            BeadTable beadTable = null;

            if (beadId > 0)
                beadTable = TableManager.GetInstance().GetTableItem<BeadTable>(beadId);

            var itemids = ItemDataManager.GetInstance().GetItemsByType(ItemTable.eType.EXPENDABLE);
            for (int i = 0; i < itemids.Count; ++i)
            {
                var itemData = ComSarahCardInlayItem._TryAddSarahCard(itemids[i]);
                if (itemData != null)
                {
                    if (beadTable != null)
                    {
                        //是同名宝珠过滤掉
                        if (beadTable.SameBeadID.Contains(itemData.TableID))
                        {
                            continue;
                        }
                    }

                    kEquipments.Add(itemData);
                }
            }

            kEquipments.RemoveAll(x =>
            {
                var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>((int)x.TableID);
                if (beadItem == null)
                {
                    return true;
                }
                
                if (equipItemData == null)
                {
                    return true;
                }

                if (beadItem.Parts.Contains((int)equipItemData.EquipWearSlotType))
                {
                    return false;
                }

                return true;
            });

            kEquipments.Sort(ComSarahCardInlayItem.Sort);

            comUIListScript.SetElementAmount(equipments.Count);
        }

        void _TrySelectedItem(ulong guid)
        {
            int iBindIndex = -1;
            for (int i = 0; i < equipments.Count; ++i)
            {
                if (equipments[i].GUID == guid)
                {
                    iBindIndex = i;
                    break;
                }
            }

            _SetSelectedItem(iBindIndex);
        }

        void _SetSelectedItem(int iBindIndex)
        {
            if (iBindIndex >= 0 && iBindIndex < equipments.Count)
            {
                Selected = equipments[iBindIndex];
                if (!comUIListScript.IsElementInScrollArea(iBindIndex))
                {
                    comUIListScript.EnsureElementVisable(iBindIndex);
                }
                comUIListScript.SelectElement(iBindIndex);
            }
            else
            {
                Selected = null;
                comUIListScript.SelectElement(-1);
            }

            if (onItemSelected != null)
            {
                onItemSelected.Invoke(Selected);
            }
        }
        
        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComSarahCardInlayItem;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index]);
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComSarahCardInlayItem;
            Selected = (current == null) ? null : current.ItemData;
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(Selected);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComSarahCardInlayItem;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }

        public void ClearSelectedItem()
        {
            Selected = null;
            if (comUIListScript != null)
            {
                comUIListScript.SelectElement(-1);
            }
        }

        public void UnInitialize()
        {
            if (comUIListScript != null)
            {
                comUIListScript.onBindItem -= _OnBindItemDelegate;
                comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                comUIListScript.onItemSelected -= _OnItemSelectedDelegate;
                comUIListScript.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
                comUIListScript = null;
            }

            equipItemData = null;
            this.onItemSelected -= onItemSelected;
            this.gameObject = null;
            this.equipments.Clear();
            Selected = null;
            bInitialize = false;
        }
    }
}


