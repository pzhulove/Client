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
    class EquipmentList
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
        public ItemData Selected
        {
            get
            {
                return ComEquipment.ms_selected;
            }
        }

        ComEquipment _OnBindItemDelegate(GameObject itemObject)
        {
            ComEquipment comEquipment = itemObject.GetComponent<ComEquipment>();
            return comEquipment;
        }

        public ComEquipment GetEquipment(ulong guid)
        {
            for(int i = 0; i < equipments.Count; ++i)
            {
                if(equipments[i].GUID == guid)
                {
                    var element = comUIListScript.GetElemenet(i);
                    if(element != null)
                    {
                        return element.gameObjectBindScript as ComEquipment;
                    }
                    break;
                }
            }
            return null;
        }

        public void Initialize(GameObject gameObject,
            OnItemSelected onItemSelected,
            SmithShopNewLinkData linkData)
        {
            if(bInitialize)
            {
                return;
            }
            bInitialize = true;
            this.gameObject = gameObject;
            this.onItemSelected += onItemSelected;
            this.linkData = linkData;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();
            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            comUIListScript.onItemSelected += _OnItemSelectedDelegate;
            comUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;

            _LoadAllEquipments(ref equipments);
            _BindLinkData();
            _TrySetDefaultEquipment();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnRefreshEquipmentList);
        }

        public void RefreshAllEquipments()
        {
            _LoadAllEquipments(ref equipments);
            _TrySetDefaultEquipment();
        }

        void _LoadAllEquipments(ref List<ItemData> kEquipments)
        {
            kEquipments.Clear();

            int minLevel = 0;
            int maxLevel = 0;
            if (SmithShopNewFrameView.iDefaultLevelData != null)
            {
                var smithshopFilterTable = TableManager.GetInstance().GetTableItem<SmithShopFilterTable>(SmithShopNewFrameView.iDefaultLevelData.Id);
                if (smithshopFilterTable != null)
                {
                    if (smithshopFilterTable.Parameter.Count >= 2)
                    {
                        minLevel = smithshopFilterTable.Parameter[0];
                        maxLevel = smithshopFilterTable.Parameter[1];
                    }
                }
            }

            //if (eEquipFilterType == SmithShopFrame.EquipFilterType.EFT_INBAG)
            {
                var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
                for (int i = 0; i < uids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData != null && !itemData.IsLease)
                    {
                        kEquipments.Add(itemData);
                    }
                }
            }

            //if(eEquipFilterType == SmithShopFrame.EquipFilterType.EFT_WEARED)
            {
                var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                for (int i = 0; i < uids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData != null && !itemData.IsLease && itemData.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                    {
                        kEquipments.Add(itemData);
                    }
                }
            }
            kEquipments.RemoveAll(x=> 
            {
                if (x.EquipType == EEquipType.ET_BREATH)
                {
                    return true;
                }

                if (SmithShopNewFrameView.iDefaultQuality != 0)
                {
                    if ((int)x.Quality != SmithShopNewFrameView.iDefaultQuality)
                        return true;
                }

                if (x.LevelLimit < minLevel)
                    return true;

                if (x.LevelLimit > maxLevel)
                    return true;

                // 过滤辟邪玉
                if (x.SubType == 199)
                {
                    return true;
                }

                return x.isInSidePack;
            });
            kEquipments.Sort(_SortEquipments);

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
                ComEquipment.ms_selected = equipments[iBindIndex];
                if (!comUIListScript.IsElementInScrollArea(iBindIndex))
                {
                    comUIListScript.EnsureElementVisable(iBindIndex);
                }
                comUIListScript.SelectElement(iBindIndex);
            }
            else
            {
                comUIListScript.SelectElement(-1);
                ComEquipment.ms_selected = null;
            }
            
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(ComEquipment.ms_selected);
            }
        }

        void _BindLinkData()
        {
            if (linkData != null)
            {
                if (linkData.itemData != null)
                {
                    var magicItems = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>((int)linkData.itemData.TableID);
                    if (magicItems != null)
                    {
                        for (int i = 0; i < equipments.Count; ++i)
                        {
                            if (magicItems.Parts.Contains((int)equipments[i].EquipWearSlotType))
                            {
                                _TrySelectedItem(equipments[i].GUID);
                                break;
                            }
                        }
                    }
                }
            }
        }

        void _TrySetDefaultEquipment()
        {
            if(ComEquipment.ms_selected != null)
            {
                var find = equipments.Find(x =>
                {
                    return x.GUID == ComEquipment.ms_selected.GUID;
                });
                if(find != null)
                {
                    ComEquipment.ms_selected = ItemDataManager.GetInstance().GetItem(ComEquipment.ms_selected.GUID);
                }
                else
                {
                    ComEquipment.ms_selected = null;
                }
            }

            int iBindIndex = -1;
            if (ComEquipment.ms_selected != null)
            {
                for (int i = 0; i < equipments.Count; ++i)
                {
                    if (equipments[i].GUID == ComEquipment.ms_selected.GUID)
                    {
                        iBindIndex = i;
                        break;
                    }
                }
            }
            else
            {
                if (equipments.Count > 0)
                {
                    iBindIndex = 0;
                }
            }

            _SetSelectedItem(iBindIndex);
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComEquipment;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index]);
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComEquipment;
            ComEquipment.ms_selected = (current == null) ? null : current.ItemData;
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(ComEquipment.ms_selected);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComEquipment;
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

            this.linkData = null;
            this.onItemSelected -= onItemSelected;
            this.gameObject = null;
            this.equipments.Clear();
            ComEquipment.ms_selected = null;
            bInitialize = false;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnRefreshEquipmentList);
        }

        int _SortEquipments(ItemData left, ItemData right)
        {
            if (left.PackageType != right.PackageType)
            {
                return (int)right.PackageType - (int)left.PackageType;
            }

            //在未启用的装备方案中，排在前面
            if (left.IsItemInUnUsedEquipPlan != right.IsItemInUnUsedEquipPlan)
            {
                if (left.IsItemInUnUsedEquipPlan == true)
                    return -1;
                if (right.IsItemInUnUsedEquipPlan == true)
                    return 1;
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

        private void OnRefreshEquipmentList(UIEvent uIEvent)
        {
            RefreshAllEquipments();
        }
    }
}