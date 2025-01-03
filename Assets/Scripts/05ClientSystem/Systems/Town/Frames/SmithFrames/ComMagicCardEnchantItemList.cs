using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
    class ComMagicCardEnchantItemList
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
        public SmithShopNewLinkData linkData = null;
        public EnchantmentsFunctionData data = null;
        public ItemData Selected
        {
            get
            {
                return ComMagicCardEnchantItem.ms_selected;
            }
            set
            {
                ComMagicCardEnchantItem.ms_selected = value;
            }
        }

        ComMagicCardEnchantItem _OnBindItemDelegate(GameObject itemObject)
        {
            ComMagicCardEnchantItem comMagicCardEnchantItem = itemObject.GetComponent<ComMagicCardEnchantItem>();
            return comMagicCardEnchantItem;
        }

        public bool HasEquipments()
        {
            return equipments.Count > 0;
        }

        public bool HasObject(ulong guid)
        {
            for(int i = 0; i < equipments.Count; ++i)
            {
                if(equipments[i] != null && equipments[i].GUID == guid)
                {
                    return true;
                }
            }
            return false;
        }

        public int FindObject(ulong guid)
        {
            for (int i = 0; i < equipments.Count; ++i)
            {
                if (equipments[i] != null && equipments[i].GUID == guid)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Initialize(GameObject gameObject,
            OnItemSelected onItemSelected, 
            SmithShopNewLinkData linkData,
            EnchantmentsFunctionData data)
        {
            if(bInitialize)
            {
                return;
            }
            bInitialize = true;

            Selected = null;
            this.gameObject = gameObject;
            this.onItemSelected += onItemSelected;
            this.linkData = linkData;
            this.data = data;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();
            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            comUIListScript.onItemSelected += _OnItemSelectedDelegate;
            comUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;

            _LoadAllEquipments(ref equipments);
        }

        public void RefreshAllEquipments()
        {
            if(Initilized)
            {
                _LoadAllEquipments(ref equipments);
            }
        }

        void _LoadAllEquipments(ref List<ItemData> kEquipments)
        {
            kEquipments.Clear();

            var itemids = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.EXPENDABLE);
            for (int i = 0; i < itemids.Count; ++i)
            {
                var itemData = ComMagicCardEnchantItem._TryAddMagicCard(itemids[i]);
                if (itemData != null)
                {
                    kEquipments.Add(itemData);
                }
            }

            kEquipments.RemoveAll(x =>
            {
                var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>((int)x.TableID);
                if(magicItem == null)
                {
                    return true;
                }

                if(data == null || data.rightItem == null)
                {
                    return true;
                }

                if (magicItem.Parts.Contains((int)data.rightItem.EquipWearSlotType))
                {
                    return false;
                }

                return true;
            });

            kEquipments.Sort(ComMagicCardEnchantItem.Sort);

            comUIListScript.SetElementAmount(equipments.Count);
        }
        
        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComMagicCardEnchantItem;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index]);
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComMagicCardEnchantItem;
            Selected = (current == null) ? null : current.ItemData;
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(Selected);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComMagicCardEnchantItem;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }

        public void ClearSelectedItem()
        {
            Selected = null;
            if(comUIListScript != null)
            {
                comUIListScript.SelectElement(-1);
            }
        }

        public void UnInitialize()
        {
            if(comUIListScript != null)
            {
                comUIListScript.onBindItem -= _OnBindItemDelegate;
                comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                comUIListScript.onItemSelected -= _OnItemSelectedDelegate;
                comUIListScript.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
                comUIListScript = null;
            }

            this.data = null;
            this.linkData = null;
            this.onItemSelected -= onItemSelected;
            this.gameObject = null;
            this.equipments.Clear();
            Selected = null;
            bInitialize = false;
        }
    }
}