using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using ProtoTable;
using Scripts.UI;
using Protocol;

namespace GameClient
{

    interface IBeadItemModel
    {
        /// <summary>
        /// 类型 [0]:未镶嵌 [1]:已镶嵌
        /// </summary>
        int mountedType { get; }
        
        ItemData equipItemData { get; }

        int eqPrecHoleIndex { get; }
        
	    ItemData beadItemData { get; }
        int buffID { get; }
       
        int beadPickNumber { get; }
        int holeType { get; }
        int replaceNumber { get; }
    }

    public class BeadItemModel : IBeadItemModel
    {
        /// <summary>
        /// 装备宝珠孔索引 类型1时设置
        /// </summary>
        public int eqPrecHoleIndex { get; private set; }

        /// <summary>
		/// 装备ItemData
		/// </summary>
        public ItemData equipItemData { get; private set; }

        /// <summary>
        /// 类型 [0]:未镶嵌 [1]:已镶嵌
        /// </summary>
        public int mountedType { get; private set; }
        
        /// <summary>
        /// 宝珠ItemData
        /// </summary>
        public ItemData beadItemData { get; private set; }

        /// <summary>
        /// 宝珠附加属性ID
        /// </summary>
        public int buffID { get; private set; }
        /// <summary>
        /// 宝珠摘取次数
        /// </summary>
        public int beadPickNumber { get; private set; }
        /// <summary>
        /// 孔类型
        /// </summary>
        public int holeType { get; private set; }
        /// <summary>
        /// 置换次数
        /// </summary>
        public int replaceNumber { get; private set; }

        public BeadItemModel(int mountedType, ItemData beadItemData, int eqPrecHoleIndex,ItemData equipItemData,int buffID,int beadPickNumber,int holeType,int replceNumber)
        {
            this.mountedType = mountedType;
            if (this.mountedType == (int)UpgradePrecType.UnMounted)
            {
                
            }
            else
            {
                this.equipItemData = equipItemData;
                this.eqPrecHoleIndex = eqPrecHoleIndex;
            }
            this.beadItemData = beadItemData;
            this.buffID = buffID;
            this.beadPickNumber = beadPickNumber;
            this.holeType = holeType;
            this.replaceNumber = replceNumber;
        }
    }

    class BeadItemList
    {
        bool bInitilized = false;
        public bool Initilized
        {
            get
            {
                return bInitilized;
            }
        }
        
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        public delegate void OnItemSelect(BeadItemModel model);
        public OnItemSelect onItemSelect;
        List<BeadItemModel> equipments = new List<BeadItemModel>();
        public SmithShopNewLinkData linkData = null;

        public List<BeadItemModel> GetBeadItemList()
        {
            return equipments;
        }

        BeadItem _OnBindItemDelegate(GameObject itemObject)
        {
            BeadItem beadItem = itemObject.GetComponent<BeadItem>();
            return beadItem;
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as BeadItem;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index]);
            }
        }

        void _OnItemSelectDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as BeadItem;
            BeadItem.ms_select = current == null ? null : current.Model;
            if (onItemSelect != null)
            {
                onItemSelect(BeadItem.ms_select);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item , bool bSelected)
        {
            var current = item.gameObjectBindScript as BeadItem;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }
        public void Initialize(GameObject gameObject,OnItemSelect onItemSelected,SmithShopNewLinkData linkData)
        {
            if (Initilized)
            {
                return;
            }
            
            bInitilized = true;
            this.gameObject = gameObject;
            this.onItemSelect += onItemSelected;
            this.linkData = linkData;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();
            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            comUIListScript.onItemSelected += _OnItemSelectDelegate;
            comUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;

            _LoadAllBeadItems(ref equipments);
            _BindLinkData();
            _TrySetDefultBeadItem();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnBeadUpgradeSuccess, _OnBeadUpgradeSuccess);
        }

        public void RefreshAllBeadItems()
        {
            _LoadAllBeadItems(ref equipments);
            _TrySetDefultBeadItem();
        }

        void _LoadAllBeadItems(ref List<BeadItemModel> kBeadItems)
        {
            kBeadItems.Clear();
            List<BeadItemModel> mEquipList = new List<BeadItemModel>();
            List<BeadItemModel> mEquipBeadList = _AddBeadItems(EPackageType.Equip);
            List<BeadItemModel> mWearEquipBeadList =_AddBeadItems(EPackageType.WearEquip);
            List<BeadItemModel> mBeadItemList = _AddBeadItems(EPackageType.Consumable);
            mEquipList.AddRange(mEquipBeadList);
            mEquipList.AddRange(mWearEquipBeadList);
            mEquipList.Sort(_SortBeadItems);
            mBeadItemList.Sort(_SortBeadItems);
            kBeadItems.AddRange(mEquipList);
            kBeadItems.AddRange(mBeadItemList);
            comUIListScript.SetElementAmount(equipments.Count);
        }

        int _SortBeadItems(BeadItemModel left,BeadItemModel right)
        {
            if (left.beadItemData.Quality != right.beadItemData.Quality)
            {
                return (int)right.beadItemData.Quality - (int)left.beadItemData.Quality;
            }
            return GetBeadLevel(right.beadItemData.TableID) - GetBeadLevel(left.beadItemData.TableID);
        }

        int GetBeadLevel(int beadId)
        {
            int level = 0;
            var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(beadId);
            if (mBeadTable != null)
            {
                level = mBeadTable.Level;
            }

            return level;
        }
        List<BeadItemModel> _AddBeadItems(EPackageType ePackageType)
        {
            List<BeadItemModel> mBeadItemList = new List<BeadItemModel>();

            if (ePackageType == EPackageType.Equip || ePackageType == EPackageType.WearEquip)
            {
                var uids = ItemDataManager.GetInstance().GetItemsByPackageType(ePackageType);
                for (int i = 0; i < uids.Count; i++)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.PreciousBeadMountHole == null)
                    {
                        continue;
                    }

                    for (int j = 0; j < itemData.PreciousBeadMountHole.Length; j++)
                    {

                        var mPreciousBeadMountHole = itemData.PreciousBeadMountHole[j];
                        if (mPreciousBeadMountHole == null)
                        {
                            continue;
                        }

                        var beadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)mPreciousBeadMountHole.preciousBeadId);
                        if (beadItemData == null)
                        {
                            continue;
                        }
                        BeadItemModel model = new BeadItemModel((int)UpgradePrecType.Mounted, beadItemData, (int)mPreciousBeadMountHole.index, itemData, (int)mPreciousBeadMountHole.randomBuffId, mPreciousBeadMountHole.pickNumber, mPreciousBeadMountHole.type,mPreciousBeadMountHole.beadReplaceNumber);
                        mBeadItemList.Add(model);
                    }
                }
            }
           else if (ePackageType == EPackageType.Consumable)
            {
                var uids = ItemDataManager.GetInstance().GetItemsByPackageType(ePackageType);
                for (int i = 0; i < uids.Count; i++)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.SubType != (int)ItemTable.eSubType.Bead)
                    {
                        continue;
                    }

                    BeadItemModel model = new BeadItemModel((int)UpgradePrecType.UnMounted, itemData, 0, null, itemData.BeadAdditiveAttributeBuffID, itemData.BeadPickNumber, 0,itemData.BeadReplaceNumber);
                    mBeadItemList.Add(model);
                }
            }

            return mBeadItemList;
        }

        void _TrySetDefultBeadItem()
        {
            int iBindIndex = -1;
            if (BeadItem.ms_select != null)
            {
                for (int i = 0; i < equipments.Count; i++)
                {
                    if (BeadItem.ms_select.mountedType == (byte)UpgradePrecType.Mounted)
                    {
                        if (equipments[i].beadItemData.TableID == BeadItem.ms_select.beadItemData.TableID && 
                            equipments[i].equipItemData.GUID == BeadItem.ms_select.equipItemData.GUID)
                        {
                            iBindIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        if (equipments[i].beadItemData.GUID == BeadItem.ms_select.beadItemData.GUID)
                        {
                            iBindIndex = i;
                            break;
                        }
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

        void _TrySelectedItem(ulong guid)
        {
            int iBindIndex = -1;
            for (int i = 0; i < equipments.Count; ++i)
            {
                if (equipments[i].beadItemData.GUID == guid)
                {
                    iBindIndex = i;
                    break;
                }
            }

            _SetSelectedItem(iBindIndex);
        }

        void _SetSelectedItem(int iBindIndex)
        {
            if (iBindIndex < 0 || equipments.Count < 0)
            {
                return;
            }

            if (iBindIndex >= 0 && iBindIndex < equipments.Count)
            {
                BeadItem.ms_select = equipments[iBindIndex];
                if (!comUIListScript.IsElementInScrollArea(iBindIndex))
                {
                    comUIListScript.EnsureElementVisable(iBindIndex);
                }
                comUIListScript.SelectElement(iBindIndex);
            }
            else
            {
                comUIListScript.SelectElement(-1);
                BeadItem.ms_select = null;
            }

            if (onItemSelect != null)
            {
                onItemSelect.Invoke(BeadItem.ms_select);
            }
        }

        void _BindLinkData()
        {
            if (linkData != null)
            {
                if (linkData.itemData != null)
                {
                    _TrySelectedItem(linkData.itemData.GUID);
                }
            }
        }
        void _OnBeadUpgradeSuccess(UIEvent uiEvent)
        {
            BeadUpgradeResultData mData = uiEvent.Param1 as BeadUpgradeResultData;

            if (mData != null)
            {
                ItemData mBeadItemData = null;
                ItemData mEquipItemData = null;
                if (mData.mountedType == (int)UpgradePrecType.Mounted)
                {
                    mBeadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mData.mBeadID);
                    mEquipItemData = ItemDataManager.GetInstance().GetItem(mData.equipGuid);
                    if (mEquipItemData == null)
                    {
                        Logger.LogErrorFormat("宝珠升级返回协议 [SceneUpgradePreciousbeadRes] 装备GUID  [equipGuid] 数据异常。");
                    }
                }
                else
                {
                    mBeadItemData = ItemDataManager.GetInstance().GetItem(mData.mBeadGUID);
                }

                if (mBeadItemData == null)
                {
                    Logger.LogErrorFormat("宝珠升级返回协议 [SceneUpgradePreciousbeadRes] 宝珠ID  [precId] 数据异常。");
                }
                BeadItemModel mode = new BeadItemModel(mData.mountedType, mBeadItemData, 0, mEquipItemData, 0, 0, 0,0);
                BeadItem.ms_select = mode;

                RefreshAllBeadItems();

            }
        }
        public void UnInitialize()
        {
            if (comUIListScript != null)
            {
                comUIListScript.onBindItem -= _OnBindItemDelegate;
                comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                comUIListScript.onItemSelected -= _OnItemSelectDelegate;
                comUIListScript.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
                comUIListScript = null;
            }
            
            this.onItemSelect -= onItemSelect;
            this.gameObject = null;
            this.equipments.Clear();
            BeadItem.ms_select = null;
            bInitilized = false;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnBeadUpgradeSuccess, _OnBeadUpgradeSuccess);
        }
    }
}
