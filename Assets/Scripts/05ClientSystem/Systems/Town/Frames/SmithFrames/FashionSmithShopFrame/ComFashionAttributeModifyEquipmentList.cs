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
    class ComFashionAttributeModifyEquipmentList
    {
        bool bInitialize = false;
        public bool Initilized
        {
            get
            {
                return bInitialize;
            }
        }
        ClientFrame clientFrame = null;
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        public delegate void OnItemSelected(ItemData itemData);
        public OnItemSelected onItemSelected;
        List<ItemData> equipments = new List<ItemData>();
        public ItemData Selected
        {
            get
            {
                return ComFashionAttributeModifyEquipment.ms_selected;
            }
            set
            {
                ComFashionAttributeModifyEquipment.ms_selected = value;
            }
        }

        ComFashionAttributeModifyEquipment _OnBindItemDelegate(GameObject itemObject)
        {
            ComFashionAttributeModifyEquipment comSealEquipment = itemObject.GetComponent<ComFashionAttributeModifyEquipment>();
            if(comSealEquipment != null)
            {
                comSealEquipment.OnCreate(clientFrame);
            }
            return comSealEquipment;
        }

        public ComFashionAttributeModifyEquipment GetComSealEquipment(ulong guid)
        {
            for(int i = 0; i < equipments.Count; ++i)
            {
                if(equipments[i].GUID == guid)
                {
                    var element = comUIListScript.GetElemenet(i);
                    if(element != null)
                    {
                        return element.gameObjectBindScript as ComFashionAttributeModifyEquipment;
                    }
                    break;
                }
            }
            return null;
        }

        public void Initialize(ClientFrame clientFrame,
            GameObject gameObject,
            OnItemSelected onItemSelected,
            ulong linkGUID)
        {
            if(bInitialize)
            {
                return;
            }
            bInitialize = true;
            ComFashionAttributeModifyEquipment.ms_selected = null;
            this.clientFrame = clientFrame;
            this.gameObject = gameObject;
            this.onItemSelected += onItemSelected;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();
            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            comUIListScript.onItemSelected += _OnItemSelectedDelegate;
            comUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;
            Selected = ItemDataManager.GetInstance().GetItem(linkGUID);

            _LoadAllEquipments(ref equipments);
            _TrySetDefaultEquipment();
        }

        void _TryKeepLastSelectedEquipment()
        {
            if (null != ComFashionAttributeModifyEquipment.ms_selected)
            {
                ComFashionAttributeModifyEquipment.ms_selected = ItemDataManager.GetInstance().GetItem(ComFashionAttributeModifyEquipment.ms_selected.GUID);
                if (null != ComFashionAttributeModifyEquipment.ms_selected)
                {
                    if (_CheckNeedFilter(ComFashionAttributeModifyEquipment.ms_selected))
                    {
                        ComFashionAttributeModifyEquipment.ms_selected = null;
                    }
                }
            }
        }

        public void RefreshAllEquipments()
        {
            if(!Initilized)
            {
                return;
            }

            _TryKeepLastSelectedEquipment();
            _LoadAllEquipments(ref equipments);
            _TrySetDefaultEquipment();
        }

        public bool _TrySelectItem(ItemData target)
        {
            _LoadAllEquipments(ref equipments);
            Selected = target;
            _TrySetDefaultEquipment();
            if(Selected.GUID == target.GUID)
            {
                return true;
            }
            return false;
        }

        bool _CheckNeedFilter(ItemData x)
        {
            return false;
        }

        void _LoadAllEquipments(ref List<ItemData> kEquipments)
        {
            kEquipments.Clear();

            var itemids = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.FASHION);
            for (int i = 0; i < itemids.Count; ++i)
            {
                var itemData = Utility._TryAddFashionItem(itemids[i]);
                if (itemData != null)
                {
                    kEquipments.Add(itemData);
                }
            }

            kEquipments.RemoveAll(x =>
            {
                return x.SubType == (int)ProtoTable.ItemTable.eSubType.FASHION_HAIR || !x.HasFashionAttribute;
                //return x.SubType == (int)ProtoTable.ItemTable.eSubType.FASHION_HAIR ||
                //x.FashionFreeTimes > 0;
            });

            kEquipments.Sort(Utility._SortFashion);

            comUIListScript.SetElementAmount(equipments.Count);
        }

        public bool HasEquipments()
        {
            return equipments.Count > 0;
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

        void _BindLinkData()
        {
        }

        void _TrySetDefaultEquipment()
        {
            if (Selected != null)
            {
                Selected = ItemDataManager.GetInstance().GetItem(Selected.GUID);
                if (Selected != null && !HasObject(Selected.GUID))
                {
                    Selected = null;
                }
            }

            int iBindIndex = -1;
            if(Selected != null)
            {
                for (int i = 0; i < equipments.Count; ++i)
                {
                    if(equipments[i].GUID == Selected.GUID)
                    {
                        iBindIndex = i;
                        break;
                    }
                }
            }
            else
            {
                if(equipments.Count > 0)
                {
                    iBindIndex = 0;
                }
            }

            _SetSelectedItem(iBindIndex);
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

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComFashionAttributeModifyEquipment;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index]);
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComFashionAttributeModifyEquipment;
            ComFashionAttributeModifyEquipment.ms_selected = (current == null) ? null : current.ItemData;
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(ComFashionAttributeModifyEquipment.ms_selected);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComFashionAttributeModifyEquipment;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
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

            this.onItemSelected -= onItemSelected;
            this.gameObject = null;
            this.clientFrame = null;
            this.equipments.Clear();
            ComFashionAttributeModifyEquipment.ms_selected = null;
            bInitialize = false;
        }

        int _SortEquipments(ItemData left, ItemData right)
        {
            if (left.PackageType != right.PackageType)
            {
                return (int)right.PackageType - (int)left.PackageType;
            }

            if (left.Quality != right.Quality)
            {
                return (int)right.Quality - (int)left.Quality;
            }

            if (left.SubType != right.SubType)
            {
                return (int)left.SubType - (int)right.SubType;
            }

            if (left.StrengthenLevel != right.StrengthenLevel)
            {
                return right.StrengthenLevel - left.StrengthenLevel;
            }

            return right.LevelLimit - left.LevelLimit;
        }
    }
}