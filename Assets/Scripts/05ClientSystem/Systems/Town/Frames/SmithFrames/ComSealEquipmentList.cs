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
    class ComSealEquipmentList
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
        public SmithShopNewTabType eFunctionType = SmithShopNewTabType.SSNTT_EQUIPMENTSEAL;
        public SmithShopNewLinkData linkData = null;
        public ItemData Selected
        {
            get
            {
                return ComSealEquipment.ms_selected;
            }
            set
            {
                ComSealEquipment.ms_selected = value;
            }
        }

        ComSealEquipment _OnBindItemDelegate(GameObject itemObject)
        {
            ComSealEquipment comSealEquipment = itemObject.GetComponent<ComSealEquipment>();
            return comSealEquipment;
        }

        public ComSealEquipment GetComSealEquipment(ulong guid)
        {
            for(int i = 0; i < equipments.Count; ++i)
            {
                if(equipments[i].GUID == guid)
                {
                    var element = comUIListScript.GetElemenet(i);
                    if(element != null)
                    {
                        return element.gameObjectBindScript as ComSealEquipment;
                    }
                    break;
                }
            }
            return null;
        }

        public void Initialize(GameObject gameObject,
            OnItemSelected onItemSelected, 
            SmithShopNewTabType eFunctionType,
            SmithShopNewLinkData linkData)
        {
            if(bInitialize)
            {
                return;
            }
            bInitialize = true;
            this.gameObject = gameObject;
            this.onItemSelected += onItemSelected;
            this.eFunctionType = eFunctionType;
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

        void _TryKeepLastSelectedEquipment()
        {
            if (null != ComSealEquipment.ms_selected)
            {
                ComSealEquipment.ms_selected = ItemDataManager.GetInstance().GetItem(ComSealEquipment.ms_selected.GUID);
                if (null != ComSealEquipment.ms_selected)
                {
                    if (_CheckNeedFilter(ComSealEquipment.ms_selected))
                    {
                        ComSealEquipment.ms_selected = null;
                    }
                }
            }
        }

        public void RefreshAllEquipments(SmithShopNewTabType eFunctionType,
            bool bForce = true)
        {
            if(!bForce && this.eFunctionType == eFunctionType)
            {
                return;
            }

            if(!Initilized)
            {
                return;
            }

            this.eFunctionType = eFunctionType;
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
            if (x.EquipType == EEquipType.ET_BREATH)
            {
                return true;
            }

            if (x.isInSidePack)
                return true;  
            if (eFunctionType == SmithShopNewTabType.SSNTT_EQUIPMENTSEAL)
            {
                if (x.PackageType != EPackageType.Equip)
                {
                    return true;
                }
                return !ComSealEquipment.CheckCanSeal(x);
            }
            else if(eFunctionType == SmithShopNewTabType.SSNTT_ADJUST)
            {
                return false;
            }
            else if (eFunctionType == SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER)
            {
                return !(!x.IsEquiped() && !x.HasTransfered && x.Quality == ItemTable.eColor.YELLOW && (/*x.BindAttr == ItemTable.eOwner.ACCBIND || */ x.BindAttr == ItemTable.eOwner.ROLEBIND));
            }

            return false;
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

            var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            for (int i = 0; i < uids.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);

                if(itemData == null)
                    continue;

                //装备租赁
                if(itemData.IsLease == true)
                    continue;

                //装备转移
                if (this.eFunctionType == SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER)
                {
                    //不显示换装方案的道具
                    if(itemData.IsItemInUnUsedEquipPlan == true)
                        continue;
                }

                if (SmithShopNewFrameView.iDefaultQuality != 0)
                {
                    if ((int)itemData.Quality != SmithShopNewFrameView.iDefaultQuality)
                        continue;
                }

                if (itemData.LevelLimit < minLevel)
                    continue;

                if (itemData.LevelLimit > maxLevel)
                    continue;

                kEquipments.Add(itemData);
                
            }

            uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < uids.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                if (itemData != null && !itemData.IsLease && itemData.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                {
                    if (SmithShopNewFrameView.iDefaultQuality != 0)
                    {
                        if ((int)itemData.Quality != SmithShopNewFrameView.iDefaultQuality)
                            continue;
                    }

                    if (itemData.LevelLimit < minLevel)
                        continue;

                    if (itemData.LevelLimit > maxLevel)
                        continue;

                    kEquipments.Add(itemData);
                }
            }

            kEquipments.RemoveAll(x =>
            {
                // 过滤辟邪玉
                if(x.SubType == 199)
                {
                    return true;
                }
                return _CheckNeedFilter(x);
            });

            kEquipments.Sort(_SortEquipments);

            comUIListScript.SetElementAmount(kEquipments.Count);
        }

        public bool HasComSealEquipments()
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
            if (linkData != null)
            {
                if(eFunctionType == SmithShopNewTabType.SSNTT_EQUIPMENTSEAL && linkData.iDefaultFirstTabId == (int)SmithShopNewTabType.SSNTT_EQUIPMENTSEAL ||
                    eFunctionType == SmithShopNewTabType.SSNTT_ADJUST && linkData.iDefaultFirstTabId == (int)SmithShopNewTabType.SSNTT_ADJUST)
                {
                    if (linkData.itemData != null)
                    {
                        _TrySelectedItem(linkData.itemData.GUID);
                    }
                }
                linkData = null;
            }
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
            var current = item.gameObjectBindScript as ComSealEquipment;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index],eFunctionType);
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComSealEquipment;
            ComSealEquipment.ms_selected = (current == null) ? null : current.ItemData;
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(ComSealEquipment.ms_selected);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComSealEquipment;
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

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnRefreshEquipmentList);

            this.linkData = null;
            this.eFunctionType = SmithShopNewTabType.SSNTT_EQUIPMENTSEAL;
            this.onItemSelected -= onItemSelected;
            this.gameObject = null;
            this.clientFrame = null;
            this.equipments.Clear();
            ComSealEquipment.ms_selected = null;
            bInitialize = false;
        }

        int _SortEquipments(ItemData left, ItemData right)
        {
            if (this.eFunctionType != SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER)
            {
                if (left.PackageType != right.PackageType)
                {
                    return (int)right.PackageType - (int)left.PackageType;
                }

                //在未启用的装备方案中
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
            }

            return right.LevelLimit - left.LevelLimit;
        }

        private void OnRefreshEquipmentList(UIEvent uIEvent)
        {
            RefreshAllEquipments(eFunctionType);
        }
    }
}