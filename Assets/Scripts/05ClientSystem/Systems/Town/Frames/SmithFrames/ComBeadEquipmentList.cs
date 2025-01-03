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
    class BeadEquipmentList
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
        /// <summary>
        /// 表示选中的页签是“装备” 还是 “称号”
        /// </summary>
        ItemTabType eItemTabType = ItemTabType.ITT_EQUIP;
        //public SmithShopFrame.EquipFilterType eEquipFilterType = SmithShopFrame.EquipFilterType.EFT_WEARED;
        public SmithShopNewLinkData linkData = null;
        public ItemData Selected
        {
            get
            {
                return ComBeadEquipment.ms_selected;
            }
        }

        ComBeadEquipment _OnBindItemDelegate(GameObject itemObject)
        {
            ComBeadEquipment combeadEquipment = itemObject.GetComponent<ComBeadEquipment>();
          
            return combeadEquipment;
        }

        public ComBeadEquipment GetEquipment(ulong guid)
        {
            for(int i = 0; i < equipments.Count; ++i)
            {
                if(equipments[i].GUID == guid)
                {
                    var element = comUIListScript.GetElemenet(i);
                    if(element != null)
                    {
                        return element.gameObjectBindScript as ComBeadEquipment;
                    }
                    break;
                }
            }
            return null;
        }

        public void Initialize(GameObject gameObject,
            OnItemSelected onItemSelected, 
            ItemTabType eItemTabType,
            SmithShopNewLinkData linkData)
        {
            if(bInitialize)
            {
                return;
            }
            bInitialize = true;
            this.gameObject = gameObject;
            this.onItemSelected += onItemSelected;
            this.eItemTabType = eItemTabType;
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

        public void RefreshAllEquipments(ItemTabType eItemTabType)
        {
            this.eItemTabType = eItemTabType;
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

            if (eItemTabType == ItemTabType.ITT_EQUIP)
            { 
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

                // var uids1 = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Bxy);
                // for (int i = 0; i < uids1.Count; ++i)
                // {
                //     var itemData = ItemDataManager.GetInstance().GetItem(uids1[i]);
                //     if (itemData != null && !itemData.IsLease)
                //     {
                //         kEquipments.Add(itemData);
                //     }
                // }
                
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
            }
            else
            {
                var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Title);
                for (int i = 0; i < uids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData != null && !itemData.IsLease)
                    {
                        kEquipments.Add(itemData);
                    }
                }

                var wearUids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                for (int i = 0; i < wearUids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(wearUids[i]);
                    if (itemData != null && !itemData.IsLease && itemData.Type == ProtoTable.ItemTable.eType.FUCKTITTLE)
                    {
                        kEquipments.Add(itemData);
                    }
                }
            }

            kEquipments.RemoveAll(x=> 
            {
                bool isBeadHole = false;
                for (int i = 0; i < x.PreciousBeadMountHole.Length; i++)
                {
                    if (x.PreciousBeadMountHole[i] == null)
                    {
                        continue;
                    }

                    isBeadHole = true;
                }

                //无镶嵌孔数据，移出列表
                if (!isBeadHole)
                {
                    return true;
                }

                //表示限时装备或称号不显示在列表中
                if (x.FixTimeLeft > 0)
                {
                    return true;
                }

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

                return x.isInSidePack;
            });
            kEquipments.Sort(_SortEquipments);
            
            List<Vector2> sizeList = new List<Vector2>();
            for (int i = 0; i < kEquipments.Count; i++)
            {
                 PrecBead[] precBeadList = kEquipments[i].PreciousBeadMountHole;
                if (precBeadList == null)
                    continue;

                bool bIsDoubleHole = false;
                for (int j = 0; j < precBeadList.Length; j++)
                {
                    PrecBead precBead = precBeadList[j];
                    if(precBead == null)
                    {
                        bIsDoubleHole = false;
                        break;
                    }

                    bIsDoubleHole = true;
                }

                if(bIsDoubleHole)
                {
                    sizeList.Add(new Vector2(422, 211));
                }
                else
                {
                    sizeList.Add(new Vector2(422, 151));
                }
            }


            comUIListScript.SetElementAmount(equipments.Count, sizeList);
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
                ComBeadEquipment.ms_selected = equipments[iBindIndex];
                if (!comUIListScript.IsElementInScrollArea(iBindIndex))
                {
                    comUIListScript.EnsureElementVisable(iBindIndex);
                }
                comUIListScript.SelectElement(iBindIndex);
            }
            else
            {
                comUIListScript.SelectElement(-1);
                ComBeadEquipment.ms_selected = null;
            }
            
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(ComBeadEquipment.ms_selected);
            }
        }

        void _BindLinkData()
        {
            if (linkData != null)
            {
                if (linkData.itemData != null)
                {
                    var beadItems = TableManager.GetInstance().GetTableItem<ProtoTable.BeadTable>((int)linkData.itemData.TableID);
                    if (beadItems != null)
                    {
                        for (int i = 0; i < equipments.Count; ++i)
                        {
                            if (beadItems.Parts.Contains((int)equipments[i].EquipWearSlotType))
                            {
                                _TrySelectedItem(equipments[i].GUID);
                                break;
                            }
                        }
                    }
                    else
                    {
                        _TrySelectedItem(linkData.itemData.GUID);
                    }
                }
            }
        }

        void _TrySetDefaultEquipment()
        {
            if(ComBeadEquipment.ms_selected != null)
            {
                var find = equipments.Find(x =>
                {
                    return x.GUID == ComBeadEquipment.ms_selected.GUID;
                });
                if(find != null)
                {
                    ComBeadEquipment.ms_selected = ItemDataManager.GetInstance().GetItem(ComBeadEquipment.ms_selected.GUID);
                }
                else
                {
                    ComBeadEquipment.ms_selected = null;
                }
            }

            int iBindIndex = -1;
            if (ComBeadEquipment.ms_selected != null)
            {
                for (int i = 0; i < equipments.Count; ++i)
                {
                    if (equipments[i].GUID == ComBeadEquipment.ms_selected.GUID)
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
            var current = item.gameObjectBindScript as ComBeadEquipment;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index]);
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComBeadEquipment;
            ComBeadEquipment.ms_selected = (current == null) ? null : current.ItemData;
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(ComBeadEquipment.ms_selected);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComBeadEquipment;
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
            this.eItemTabType = ItemTabType.ITT_EQUIP;
            this.onItemSelected -= onItemSelected;
            this.gameObject = null;
            this.equipments.Clear();
            ComBeadEquipment.ms_selected = null;
            bInitialize = false;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnRefreshEquipmentList);
        }

        int _SortEquipments(ItemData left, ItemData right)
        {
            if (eItemTabType == ItemTabType.ITT_EQUIP)
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
            }
            else
            {
                if (left.PackageType != right.PackageType)
                {
                    return (int)left.PackageType - (int)right.PackageType;
                }

                //在未启用的装备方案中，排在前面
                if (left.IsItemInUnUsedEquipPlan != right.IsItemInUnUsedEquipPlan)
                {
                    if (left.IsItemInUnUsedEquipPlan == true)
                        return -1;
                    if (right.IsItemInUnUsedEquipPlan == true)
                        return 1;
                }

                if (left.Packing != right.Packing)
                {
                    return left.Packing.CompareTo(right.Packing);
                }
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
            RefreshAllEquipments(eItemTabType);
        }
    }
}