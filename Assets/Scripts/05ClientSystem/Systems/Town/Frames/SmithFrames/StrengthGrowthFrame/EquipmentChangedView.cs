using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using System;
using ProtoTable;

namespace GameClient
{
    /// <summary>
    /// 增幅属性
    /// </summary>
    [Serializable]
    public class GrowthArrtData
    {
        public int mIndex;
        public EGrowthAttrType mGrowthAttrType;
        public string mAttrDesc;
        public string mIconPath;
    }

    class SaveAttrData
    {
        public GrowthArrtData mGrowthArrtData;
        public ComCommonBind mBind;
    }

    class EquipmentChangedView : StrengthGrowthBaseView
    {
        [SerializeField]private List<GrowthArrtData> mGrowthArrtDataList = new List<GrowthArrtData>();
        [SerializeField]private GameObject mItemParent;
        [SerializeField]private Button mChangedBtn;
        [SerializeField]private Text mArrtDesc;
        [SerializeField]private ComUIListScript mArrtUIListScript;
        [SerializeField]private StateController mEquipStateCtrl;
        [SerializeField]private ItemComeLink itemComeLink;
        [SerializeField]private ComUIListScript mExpendUIList;
        [SerializeField]private int mMinLevel = 8;
        [SerializeField]private int mMaxLevel = 10;

        private List<ItemData> expendItemList = new List<ItemData>();

        private StrengthenGrowthType mStrengthenGrowthType;
        private StrengthenGrowthView mStrengthenGrowthView;
        private ComItemNew mEquipComItem;
        private EGrowthAttrType mSelectChangeGrowthArrtType = EGrowthAttrType.GAT_NONE;//选择要转换的属性类型
        private ItemData mExpendItemData;
        private ItemData mSelectEquipItemData;
        private int iExpendCount = 0;
        ItemData creatItemData; //虚空物质生成装置
        ItemData overLoadItemData; //虚空物质过载装置
        ItemData changedItemData; // 虚空物质转换器
        private List<SaveAttrData> mChangeGrowthArrtList = new List<SaveAttrData>();
        private GrowthArrtData mGrowthArrtData;

        int timeLeft;
        bool bStartCountdown;
        public sealed override void IniteData(SmithShopNewLinkData linkData, StrengthenGrowthType type, StrengthenGrowthView strengthenGrowthView)
        {
            mStrengthenGrowthType = type;
            mStrengthenGrowthView = strengthenGrowthView;

            CreatComitem();
            if (mEquipStateCtrl != null)
            {
                mEquipStateCtrl.Key = "notHasEquip";
            }
        }

        private void Awake()
        {
            InitExpendUIList();
            RegisterUIEventHandle();
            InitGrowthArrtUILIstScript();
            
            if (mChangedBtn != null)
            {
                mChangedBtn.onClick.RemoveAllListeners();
                mChangedBtn.onClick.AddListener(OnChangedBtnClick);
            }
        }

        private void OnDestroy()
        {
            UnInitExpendUIList();
            UnRegisterUIEventHandle();
            UnInitGrowthArrtUILIstScript();
            mEquipComItem = null;
            mExpendItemData = null;
            mSelectEquipItemData = null;
            mSelectChangeGrowthArrtType = EGrowthAttrType.GAT_NONE;
            mStrengthenGrowthView = null;
            mChangeGrowthArrtList.Clear();
            mGrowthArrtData = null;
            if (expendItemList != null)
                expendItemList.Clear();
        }

        private void RegisterUIEventHandle()
        {
            StrengthenGrowthView.mOnStrengthenGrowthEquipItemClick += OnStrengthenGrowthEquipItemClick;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRedMarkEquipChangedSuccess, OnRedMarkEquipChangedSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEquipmentListNoEquipment, OnEquipmentListNoEquipment);
        }

        private void UnRegisterUIEventHandle()
        {
            StrengthenGrowthView.mOnStrengthenGrowthEquipItemClick -= OnStrengthenGrowthEquipItemClick;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRedMarkEquipChangedSuccess, OnRedMarkEquipChangedSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEquipmentListNoEquipment, OnEquipmentListNoEquipment);
        }

        private void OnStrengthenGrowthEquipItemClick(ItemData itemData, StrengthenGrowthType eStrengthenGrowthType)
        {
            if (itemData == null)
            {
                return;
            }

            mSelectEquipItemData = itemData;
            if (eStrengthenGrowthType == mStrengthenGrowthType)
            {
                if (mEquipStateCtrl != null)
                {
                    mEquipStateCtrl.Key = "hasEquip";
                }

                mGrowthArrtData = null;
                mSelectChangeGrowthArrtType = EGrowthAttrType.GAT_NONE;
                mExpendItemData = null;
                UpdateEquipItem(itemData);
                UpdateGrowthExpendItem(itemData);
                UpdateItemComeLink();
                LoadAllExpendItem();
                UpdateGrowthArrtElementAmount(mGrowthArrtDataList.Count);
            }
        }

        private void OnRedMarkEquipChangedSuccess(UIEvent uiEvent)
        {
            if (mStrengthenGrowthView != null)
            {
                mStrengthenGrowthView.RefreshAllEquipments();
            }
        }

        private void OnEquipmentListNoEquipment(UIEvent uiEvent)
        {
            mExpendItemData = null;
            mSelectEquipItemData = null;
            mSelectChangeGrowthArrtType = EGrowthAttrType.GAT_NONE;

            UpdateEquipItem(mSelectEquipItemData);
            UpdateGrowthArrtElementAmount(0);

            if (mEquipStateCtrl != null)
            {
                mEquipStateCtrl.Key = "notHasEquip";
            }
        }
        
        private void UpdateGrowthExpendItem(ItemData equipItemData)
        {
            if (equipItemData == null)
            {
                mExpendItemData = null;
            }
            else
            {
                creatItemData = null; //虚空物质生成装置
                overLoadItemData = null; //虚空物质过载装置
                changedItemData = null; // 虚空物质转换器
                var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);
                //普通装备强化等级为0
                if (equipItemData.EquipType == EEquipType.ET_COMMON && equipItemData.StrengthenLevel <= 0)
                {
                    //查找生成装置（绑定）
                    FindBindExpendItemData(equipItemData, items, ItemTable.eSubType.ST_ZENGFU_CREATE, ref creatItemData);
                    
                    //未找到生成装置（绑定）
                    if (creatItemData == null)
                    {
                        //查找非绑定生成装置
                        FindUnBindExpendItemData(equipItemData, items, ItemTable.eSubType.ST_ZENGFU_CREATE, ref creatItemData);
                    }

                    if (creatItemData == null)
                    {
                        //查找生成装置（绑定）
                        FindBindExpendItemData(equipItemData, items, ItemTable.eSubType.ST_ZENGFU_OVERLOAD, ref overLoadItemData);

                        //未找到生成装置（绑定）
                        if (overLoadItemData == null)
                        {
                            //查找非绑定生成装置
                            FindUnBindExpendItemData(equipItemData, items, ItemTable.eSubType.ST_ZENGFU_OVERLOAD, ref overLoadItemData);
                        }
                    }
                }//普通装备强化等级大于0
                else if (equipItemData.EquipType == EEquipType.ET_COMMON && equipItemData.StrengthenLevel > 0)
                {
                    //查找生成装置（绑定）
                    FindBindExpendItemData(equipItemData, items, ItemTable.eSubType.ST_ZENGFU_OVERLOAD, ref overLoadItemData);

                    //未找到生成装置（绑定）
                    if (overLoadItemData == null)
                    {
                        //查找非绑定生成装置
                        FindUnBindExpendItemData(equipItemData, items, ItemTable.eSubType.ST_ZENGFU_OVERLOAD, ref overLoadItemData);
                    }
                } //红字装备
                else if (equipItemData.EquipType == EEquipType.ET_REDMARK)
                {
                    //查找转换器（绑定）
                    FindBindExpendItemData(equipItemData, items, ItemTable.eSubType.ST_ZENGFU_CHANGE, ref changedItemData);

                    //未找到转换器（绑定）
                    if (changedItemData == null)
                    {
                        //查找非绑定转换器
                        FindUnBindExpendItemData(equipItemData, items, ItemTable.eSubType.ST_ZENGFU_CHANGE, ref changedItemData);
                    }
                }
                
                //普通装备，强化等级为0
                if (equipItemData.EquipType == EEquipType.ET_COMMON && equipItemData.StrengthenLevel <= 0)
                {
                    if (creatItemData != null)
                    {
                        SetExpendItemData(creatItemData);
                    }
                    else
                    {
                        mExpendItemData = null;
                    }
                }//普通装备，强化等级大于0
                else if (equipItemData.EquipType == EEquipType.ET_COMMON && equipItemData.StrengthenLevel > 0)
                {
                    if (overLoadItemData != null)
                    {
                        SetExpendItemData(overLoadItemData);
                    }
                    else
                    {
                        mExpendItemData = null;
                    }
                }//红字装备
                else if (equipItemData.EquipType == EEquipType.ET_REDMARK)
                {
                    if (changedItemData != null)
                    {
                        SetExpendItemData(changedItemData);
                    }
                    else
                    {
                        mExpendItemData = null;
                    }
                }
            }
        }

        /// <summary>
        /// 查找绑定道具
        /// </summary>
        /// <param name="equipItemData"></param>
        /// <param name="items"></param>
        /// <param name="expendItemData"></param>
        private void FindBindExpendItemData(ItemData equipItemData, List<ulong> items, ItemTable.eSubType subType, ref ItemData expendItemData)
        {
            //如果是过载装置
            if (subType == ItemTable.eSubType.ST_ZENGFU_OVERLOAD)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var data = ItemDataManager.GetInstance().GetItem(items[i]);
                    if (data == null)
                        continue;
                    
                    if (data.SubType != (int)subType)
                    {
                        continue;
                    }

                    if (data.BindAttr <= ProtoTable.ItemTable.eOwner.NOTBIND)
                    {
                        continue;
                    }

                    expendItemData = data;
                    break;
                }
            }
            else
            {
                ItemData itemData = null;

                //如果是紫色装备并且不是红字装备
                if (equipItemData.Quality == ItemTable.eColor.PURPLE && equipItemData.EquipType != EEquipType.ET_REDMARK)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var data = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (data == null)
                            continue;

                        //优先填充紫色道具
                        if (data.ThirdType != ItemTable.eThirdType.ZENGFU_COLOR_PURPLE)
                        {
                            continue;
                        }

                        if (data.SubType != (int)subType)
                        {
                            continue;
                        }

                        if (data.BindAttr <= ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            continue;
                        }

                        data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                        //道具过期了 过滤掉
                        if (timeLeft <= 0 && bStartCountdown)
                        {
                            continue;
                        }

                        itemData = data;
                        break;
                    }

                    if (itemData == null)
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            var data = ItemDataManager.GetInstance().GetItem(items[i]);
                            if (data == null)
                                continue;

                            //绿色道具
                            if (data.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_GREEN)
                            {
                                continue;
                            }

                            if (data.SubType != (int)subType)
                            {
                                continue;
                            }

                            if (data.BindAttr <= ProtoTable.ItemTable.eOwner.NOTBIND)
                            {
                                continue;
                            }

                            data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                            //道具过期了 过滤掉
                            if (timeLeft <= 0 && bStartCountdown)
                            {
                                continue;
                            }

                            expendItemData = data;
                            break;
                        }
                    }
                    else
                    {
                        expendItemData = itemData;
                    }
                }
                else if (equipItemData.Quality == ItemTable.eColor.GREEN && equipItemData.EquipType != EEquipType.ET_REDMARK)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var data = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (data == null)
                            continue;

                        //优先填充绿色道具
                        if (data.ThirdType != ItemTable.eThirdType.ZENGFU_COLOR_GREEN)
                        {
                            continue;
                        }

                        if (data.SubType != (int)subType)
                        {
                            continue;
                        }

                        if (data.BindAttr <= ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            continue;
                        }

                        data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                        //道具过期了 过滤掉
                        if (timeLeft <= 0 && bStartCountdown)
                        {
                            continue;
                        }

                        itemData = data;
                        break;
                    }

                    if (itemData == null)
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            var data = ItemDataManager.GetInstance().GetItem(items[i]);
                            if (data == null)
                                continue;

                            if (data.SubType != (int)subType)
                            {
                                continue;
                            }

                            //优先填充紫色道具
                            if (data.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_PURPLE)
                            {
                                continue;
                            }

                            if (data.BindAttr <= ProtoTable.ItemTable.eOwner.NOTBIND)
                            {
                                continue;
                            }

                            data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                            //道具过期了 过滤掉
                            if (timeLeft <= 0 && bStartCountdown)
                            {
                                continue;
                            }

                            expendItemData = data;
                            break;
                        }
                    }
                    else
                    {
                        expendItemData = itemData;
                    }
                }
                else
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var data = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (data == null)
                            continue;

                        if (equipItemData.Quality > ItemTable.eColor.PURPLE && equipItemData.EquipType != EEquipType.ET_REDMARK)
                        {
                            if (data.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_PURPLE)
                            {
                                continue;
                            }

                            if (data.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_GREEN)
                            {
                                continue;
                            }
                        }

                        if (data.SubType != (int)subType)
                        {
                            continue;
                        }

                        if (data.BindAttr <= ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            continue;
                        }

                        data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                        //道具过期了 过滤掉
                        if (timeLeft <= 0 && bStartCountdown)
                        {
                            continue;
                        }

                        expendItemData = data;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 非查找绑定道具
        /// </summary>
        /// <param name="equipItemData"></param>
        /// <param name="items"></param>
        /// <param name="expendItemData"></param>
        private void FindUnBindExpendItemData(ItemData equipItemData, List<ulong> items, ItemTable.eSubType subType, ref ItemData expendItemData)
        {
            //如果是过载装置
            if (subType == ItemTable.eSubType.ST_ZENGFU_OVERLOAD)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var data = ItemDataManager.GetInstance().GetItem(items[i]);
                    if (data == null)
                        continue;
                    
                    if (data.SubType != (int)subType)
                    {
                        continue;
                    }

                    if (data.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND)
                    {
                        continue;
                    }

                    expendItemData = data;
                    break;
                }
            }
            else
            {
                ItemData itemData = null;

                //如果是紫色装备并且不是红字装备
                if (equipItemData.Quality == ItemTable.eColor.PURPLE && equipItemData.EquipType != EEquipType.ET_REDMARK)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var data = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (data == null)
                            continue;

                        //优先填充紫色道具
                        if (data.ThirdType != ItemTable.eThirdType.ZENGFU_COLOR_PURPLE)
                        {
                            continue;
                        }

                        if (data.SubType != (int)subType)
                        {
                            continue;
                        }

                        if (data.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            continue;
                        }

                        data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                        //道具过期了 过滤掉
                        if (timeLeft <= 0 && bStartCountdown)
                        {
                            continue;
                        }

                        itemData = data;
                        break;
                    }

                    if (itemData == null)
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            var data = ItemDataManager.GetInstance().GetItem(items[i]);
                            if (data == null)
                                continue;

                            //绿色道具
                            if (data.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_GREEN)
                            {
                                continue;
                            }

                            if (data.SubType != (int)subType)
                            {
                                continue;
                            }

                            if (data.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND)
                            {
                                continue;
                            }

                            data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                            //道具过期了 过滤掉
                            if (timeLeft <= 0 && bStartCountdown)
                            {
                                continue;
                            }

                            expendItemData = data;
                            break;
                        }
                    }
                    else
                    {
                        expendItemData = itemData;
                    }
                }
                else if (equipItemData.Quality == ItemTable.eColor.GREEN && equipItemData.EquipType != EEquipType.ET_REDMARK)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var data = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (data == null)
                            continue;

                        //优先填充绿色道具
                        if (data.ThirdType != ItemTable.eThirdType.ZENGFU_COLOR_GREEN)
                        {
                            continue;
                        }

                        if (data.SubType != (int)subType)
                        {
                            continue;
                        }

                        if (data.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            continue;
                        }

                        data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                        //道具过期了 过滤掉
                        if (timeLeft <= 0 && bStartCountdown)
                        {
                            continue;
                        }

                        itemData = data;
                        break;
                    }

                    if (itemData == null)
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            var data = ItemDataManager.GetInstance().GetItem(items[i]);
                            if (data == null)
                                continue;

                            if (data.SubType != (int)subType)
                            {
                                continue;
                            }

                            //优先填充紫色道具
                            if (data.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_PURPLE)
                            {
                                continue;
                            }

                            if (data.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND)
                            {
                                continue;
                            }

                            data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                            //道具过期了 过滤掉
                            if (timeLeft <= 0 && bStartCountdown)
                            {
                                continue;
                            }

                            expendItemData = data;
                            break;
                        }
                    }
                    else
                    {
                        expendItemData = itemData;
                    }
                }
                else
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var data = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (data == null)
                            continue;

                        if (equipItemData.Quality > ItemTable.eColor.PURPLE && equipItemData.EquipType != EEquipType.ET_REDMARK)
                        {
                            if (data.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_PURPLE)
                            {
                                continue;
                            }

                            if (data.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_GREEN)
                            {
                                continue;
                            }
                        }

                        if (data.SubType != (int)subType)
                        {
                            continue;
                        }

                        if (data.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND)
                        {
                            continue;
                        }

                        data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                        //道具过期了 过滤掉
                        if (timeLeft <= 0 && bStartCountdown)
                        {
                            continue;
                        }

                        expendItemData = data;
                        break;
                    }
                }
            }
        }

        private void SetExpendItemData(ItemData itemData)
        {
            mExpendItemData = itemData;
        }

        private void UpdateItemComeLink()
        {
            if (mSelectEquipItemData.EquipType == EEquipType.ET_COMMON && mSelectEquipItemData.StrengthenLevel <= 0)
            {
                if (creatItemData == null && overLoadItemData == null)
                {
                    itemComeLink.iItemLinkID = 300000202;
                }
            }
            else if (mSelectEquipItemData.EquipType == EEquipType.ET_COMMON && mSelectEquipItemData.StrengthenLevel > 0)
            {
                if (overLoadItemData == null)
                {
                    itemComeLink.iItemLinkID = 310000228;
                }
            }
            else if (mSelectEquipItemData.EquipType == EEquipType.ET_REDMARK)
            {
                if (changedItemData == null)
                {
                    itemComeLink.iItemLinkID = 300000204;
                }
            }
        }
        
        private void CreatComitem()
        {
            if (mEquipComItem == null)
            {
                mEquipComItem = ComItemManager.CreateNew(mItemParent);
            }

            mEquipComItem.Setup(null, null);
        }
        
        private void UpdateEquipItem(ItemData itemData)
        {
            if (mEquipComItem != null)
            {
                mEquipComItem.Setup(itemData, Utility.OnItemClicked);
            }

            if (itemData != null)
            {
                if (itemData.EquipType == EEquipType.ET_REDMARK)
                {
                    mArrtDesc.text = TR.Value("growth_attr_des", EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(itemData.GrowthAttrType), itemData.GrowthAttrNum);
                }
                else if (itemData.EquipType == EEquipType.ET_COMMON)
                {
                    mArrtDesc.text = "待转化";
                }
            }
        }

        private void UpdateGrowthArrtElementAmount(int Count)
        {
            mChangeGrowthArrtList.Clear();

            if (mArrtUIListScript != null)
            {
                mArrtUIListScript.SetElementAmount(Count);
            }
        }

        private void InitExpendUIList()
        {
            if (mExpendUIList != null)
            {
                mExpendUIList.Initialize();
                mExpendUIList.onBindItem += OnBindItemDelegate;
                mExpendUIList.onItemVisiable += OnItemVisiableDelegate;
                mExpendUIList.onItemSelected += OnItemSelectedDelegate;
                mExpendUIList.onItemChageDisplay += OnItemChangeDisplayDelegate;
            }
        }

        private void UnInitExpendUIList()
        {
            if (mExpendUIList != null)
            {
                mExpendUIList.onBindItem -= OnBindItemDelegate;
                mExpendUIList.onItemVisiable -= OnItemVisiableDelegate;
                mExpendUIList.onItemSelected -= OnItemSelectedDelegate;
                mExpendUIList.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }
        }

        private CommonImplementItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonImplementItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null && item.m_index >= 0 && item.m_index < expendItemList.Count)
            {
                ulong guid = expendItemList[item.m_index].GUID;
                commonImplementItem.OnItemVisiable(guid);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
            {
                SetExpendItemData(commonImplementItem.ItemData);
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bIsSelected)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
            {
                commonImplementItem.OnChangeDisplay(bIsSelected);
            }
        }

        private void LoadAllExpendItem()
        {
            if (expendItemList == null)
                expendItemList = new List<ItemData>();
            expendItemList.Clear();

            var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);
            for (int i = 0; i < items.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                if (itemData == null)
                    continue;

                if (mSelectEquipItemData != null)
                {
                    //紫色装备可以使用
                    if (itemData.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_PURPLE)
                    {
                        if (mSelectEquipItemData.Quality != ItemTable.eColor.PURPLE)
                        {
                            continue;
                        }
                    }

                    //绿色装备可以使用
                    if (itemData.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_GREEN)
                    {
                        if (mSelectEquipItemData.Quality != ItemTable.eColor.GREEN)
                        {
                            continue;
                        }
                    }

                    //如果是红字装备
                    if (mSelectEquipItemData.EquipType == EEquipType.ET_REDMARK)
                    {
                        //虚空物质转换器
                        if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_CHANGE)
                        {
                            itemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                            //失效了
                            if (timeLeft <= 0 && bStartCountdown)
                            {
                                continue;
                            }
                            AddExpendItem(itemData);
                            continue;
                        }
                    }//如果是普通装备并且强化等级小于等于0
                    else if (mSelectEquipItemData.EquipType == EEquipType.ET_COMMON && mSelectEquipItemData.StrengthenLevel <= 0)
                    {//虚空物质生成装置
                        if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_CREATE)
                        {
                            itemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                            //失效了
                            if (timeLeft <= 0 && bStartCountdown)
                            {
                                continue;
                            }

                            AddExpendItem(itemData);
                            continue;
                        }//虚空物质过载装置
                        else if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_OVERLOAD)
                        {
                            AddExpendItem(itemData);
                            continue;
                        }
                    }//如果是普通装备并且强化等级大于0
                    else if (mSelectEquipItemData.EquipType == EEquipType.ET_COMMON && mSelectEquipItemData.StrengthenLevel > 0)
                    {
                        //虚空物质过载装置
                        if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_OVERLOAD)
                        {
                            AddExpendItem(itemData);
                            continue;
                        }
                    }
                }
            }

            OnSetElementAmount();
        }

        private void AddExpendItem(ItemData itemData)
        {
            expendItemList.Add(itemData);
        }

        private void OnSetElementAmount()
        {
            if (mExpendUIList != null)
            {
                mExpendUIList.ResetSelectedElementIndex();
                mExpendUIList.SetElementAmount(expendItemList.Count);
            }

            TrySetDefaultExpendItem();
        }

        private void TrySetDefaultExpendItem()
        {
            if (mExpendItemData == null)
            {
                return;
            }

            int iSelectedIndex = -1;
            for (int i = 0; i < expendItemList.Count; i++)
            {
                if (expendItemList[i].GUID != mExpendItemData.GUID)
                    continue;

                iSelectedIndex = i;
                break;
            }

            if (iSelectedIndex >= 0 && iSelectedIndex < expendItemList.Count)
            {
                if (mExpendUIList != null)
                {
                    if (!mExpendUIList.IsElementInScrollArea(iSelectedIndex))
                    {
                        mExpendUIList.EnsureElementVisable(iSelectedIndex);
                    }

                    mExpendUIList.SelectElement(iSelectedIndex);
                }
            }
        }


        #region GrowthArrt
        private void InitGrowthArrtUILIstScript()
        {
            if (mArrtUIListScript != null)
            {
                mArrtUIListScript.Initialize();
                mArrtUIListScript.onBindItem += OnBindArrtItemDelegate;
                mArrtUIListScript.onItemVisiable += OnArrtItemVisiableDelegate;
            }
        }

        private void UnInitGrowthArrtUILIstScript()
        {
            if (mArrtUIListScript != null)
            {
                mArrtUIListScript.onBindItem -= OnBindItemDelegate;
                mArrtUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private ComCommonBind OnBindArrtItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnArrtItemVisiableDelegate(ComUIListElementScript item)
        {
            var mBind = item.gameObjectBindScript as ComCommonBind;
            if (mBind != null && item.m_index >=0 && item.m_index < mGrowthArrtDataList.Count)
            {
                Text mArrtDesc = mBind.GetCom<Text>("ArrtDesc");
                Toggle mArrtTog = mBind.GetCom<Toggle>("ArrtTog");
                GameObject mCheckMarkGo = mBind.GetGameObject("CheckMark");
                Image mIcon = mBind.GetCom<Image>("Icon");
                mCheckMarkGo.CustomActive(false);
                var mArrtData = mGrowthArrtDataList[item.m_index];

                if (!string.IsNullOrEmpty(mArrtData.mIconPath))
                {
                    if (mIcon != null)
                        ETCImageLoader.LoadSprite(ref mIcon, mArrtData.mIconPath);
                }

                if (mArrtDesc != null)
                {
                    if (mSelectEquipItemData != null && mSelectEquipItemData.EquipType == EEquipType.ET_REDMARK && mSelectEquipItemData.GrowthAttrType == mArrtData.mGrowthAttrType)
                    {
                        mArrtDesc.text = string.Format("{0}\n(当前)", mArrtData.mAttrDesc);
                    }
                    else
                    {
                        mArrtDesc.text = mArrtData.mAttrDesc;
                    }
                }

                if (mArrtTog != null)
                {
                    mArrtTog.onValueChanged.RemoveAllListeners();
                    mArrtTog.onValueChanged.AddListener((bool value)=> 
                    {
                        if (value)
                        {
                            OnGrowthArrtToggleClick(mArrtData);
                        }

                        mCheckMarkGo.CustomActive(value);
                    } );
                }

                SaveAttrData attrData = new SaveAttrData();
                attrData.mGrowthArrtData = mArrtData;
                attrData.mBind = mBind;

                if(!mChangeGrowthArrtList.Contains(attrData))
                {
                    mChangeGrowthArrtList.Add(attrData);
                }
            }
        }

        private void OnGrowthArrtToggleClick(GrowthArrtData data)
        {
            if (data != null)
            {
                mGrowthArrtData = data;
                mSelectChangeGrowthArrtType = data.mGrowthAttrType;

                OnRefreshGrowthAttrDesc(mGrowthArrtData);
            }
        }

        private void OnRefreshGrowthAttrDesc(GrowthArrtData data)
        {
            if (data == null)
            {
                return;
            }

            if (mSelectEquipItemData.EquipType == EEquipType.ET_COMMON && mExpendItemData != null)
            {
                for (int i = 0; i < mChangeGrowthArrtList.Count; i++)
                {
                    var attrData = mChangeGrowthArrtList[i];
                    if (attrData == null)
                    {
                        continue;
                    }

                    Text mAttrDesc = attrData.mBind.GetCom<Text>("ArrtDesc");
                    if (mAttrDesc != null)
                    {
                        mAttrDesc.text = attrData.mGrowthArrtData.mAttrDesc;
                    }
                }

                if (mExpendItemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_OVERLOAD)
                {
                    SaveAttrData saveAttrData = null;
                    for (int i = 0; i < mChangeGrowthArrtList.Count; i++)
                    {
                        var attrData = mChangeGrowthArrtList[i];
                        if (attrData == null)
                        {
                            continue;
                        }

                        if (attrData.mGrowthArrtData.mGrowthAttrType != data.mGrowthAttrType)
                        {
                            continue;
                        }

                        saveAttrData = attrData;
                        break;
                    }

                    Text mArrtDesc = saveAttrData.mBind.GetCom<Text>("ArrtDesc");
                    if (mArrtDesc != null)
                    {
                        mArrtDesc.text = saveAttrData.mGrowthArrtData.mAttrDesc + GetIntervalValue();
                    }
                }
            }
        }

        private string GetIntervalValue()
        {
            if (mSelectEquipItemData != null)
            {
                float minValue = EquipGrowthDataManager.GetInstance().GetGrowthAttributeValue(mSelectEquipItemData, mMinLevel);
                float maxValue = EquipGrowthDataManager.GetInstance().GetGrowthAttributeValue(mSelectEquipItemData, mMaxLevel);

                return string.Format("\n+{0}~+{1}", minValue, maxValue);
            }

            return string.Empty;
        }
        #endregion

        #region 转化按钮

        private void OnChangedBtnClick()
        {
            if (mSelectEquipItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("请选择装备");
                return;
            }

            if (mSelectEquipItemData != null && mSelectEquipItemData.bLocked)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("已加锁的装备无法转化");
                return;
            }


            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            if (mExpendItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("请选择消耗道具");
                return;
            }

            if (mSelectChangeGrowthArrtType == EGrowthAttrType.GAT_NONE)
            {
                SystemNotifyManager.SysNotifyTextAnimation("请选择转化的属性");
                return;
            }

            if (mSelectEquipItemData.EquipType == EEquipType.ET_REDMARK)
            {
                if (mSelectEquipItemData.GrowthAttrType == mSelectChangeGrowthArrtType)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("equipment_changed_check_desc"));
                    return;
                }
            }

            if (mExpendItemData != null)
            {
                ItemData expendItemData = ItemDataManager.GetInstance().GetItem(mExpendItemData.GUID);
                if (expendItemData != null)
                {
                    int timeLeft;
                    bool bStartCountdown;
                    mExpendItemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                    //失效了
                    if (timeLeft <= 0 && bStartCountdown)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_item"));
                        return;
                    }
                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_item"));
                    return;
                }
            }

            //如果选择的消耗道具是转换器
            if (mExpendItemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_CHANGE)
            {
                string mContent = TR.Value("change_converter_desc",
                    EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(mSelectEquipItemData.GrowthAttrType),
                    mSelectEquipItemData.GrowthAttrNum,
                    EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(mSelectChangeGrowthArrtType));

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                {
                    OnSceneEquipEnhanceChg();
                });
                return;
            }//如果是生成装置
            else if (mExpendItemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_CREATE)
            {
                string mContent = TR.Value("change_generatingdevice_desc",
                    EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(mSelectChangeGrowthArrtType),
                    EquipGrowthDataManager.GetInstance().GetGrowthAttributeValue(mSelectEquipItemData, mSelectEquipItemData.StrengthenLevel));

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                {
                    OnSceneEquipEnhanceChg();
                });
                return;
            }

            //普通装备，强化等大于等于11级
            if (mSelectEquipItemData.EquipType == EEquipType.ET_COMMON && mSelectEquipItemData.StrengthenLevel >= 11)
            {
                string mContent = TR.Value("equipment_changed_desc");
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () => 
                {
                    OnSceneEquipEnhanceChg();
                });
                return;
            }
            
            OnSceneEquipEnhanceChg();
        }

        private void OnSceneEquipEnhanceChg()
        {
            EquipGrowthDataManager.GetInstance().OnSceneEquipEnhanceChg(mSelectEquipItemData, (UInt32)mExpendItemData.TableID, (byte)mSelectChangeGrowthArrtType);
        }
         
#endregion
    }
}