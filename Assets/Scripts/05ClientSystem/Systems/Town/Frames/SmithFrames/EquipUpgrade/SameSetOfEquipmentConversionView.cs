using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using ProtoTable;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class EquipmentConversionUtility
    {
        public static int SoreEquipments(ItemData left,ItemData right)
        {
            if (left.PackageType != right.PackageType)
            {
                return (int)right.PackageType - (int)left.PackageType;
            }

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
    }

    public class SameSetOfEquipmentConversionView : MonoBehaviour, ISmithShopNewView
    {
        [SerializeField] private ComUIListScript mEquipmentComUIList;
        [SerializeField] private ComUIListScript mConverterEquipmentUIList;
        [SerializeField] private ComUIListScript mMaterailExpendItemComUIListScript;
        [SerializeField] private GameObject mCurrentEquipmentItemParent;
        [SerializeField] private GameObject mConverterItemParent;
        [SerializeField] private GameObject mMaterailExpendItemRoot;
        [SerializeField] private GameObject mConverterItemRoot;
        [SerializeField] private Text mCurrentEquipmentName;
        [SerializeField] private Text mConverterItemName;
        [SerializeField] private Text mConverterItemCount;
        [SerializeField] private Image mConverterItemBackground;
        [SerializeField] private Image mConverterItemIcon;
        [SerializeField] private Button mConverterBtn;
        [SerializeField] private Button mConversionBtn;
        [SerializeField] private Toggle mConverterReplacePropTog;

        private string selectedConverterDes = "选择转化器";
        /// <summary>
        /// 装备列表
        /// </summary>
        private List<ItemData> mAllEquipmentList = new List<ItemData>();
        /// <summary>
        /// 消耗材料列表
        /// </summary>
        private List<ItemSimpleData> mMaterailExpendItems = new List<ItemSimpleData>();

        /// <summary>
        /// 转换器列表
        /// </summary>
        private List<ItemData> mConverterItemDataList = new List<ItemData>();

        /// <summary>
        /// 转化器表格数据
        /// </summary>
        private List<ItemSimpleData> mConverterItems = new List<ItemSimpleData>();

        /// <summary>
        /// 当前选择的橙色装备
        /// </summary>
        private ItemData mCurrentEquipmentItemData;
        /// <summary>
        /// 转化的目标橙色装备
        /// </summary>
        private ItemData mTargetEquipmentItemData;
        /// <summary>
        /// 转化器道具
        /// </summary>
        private ItemData mConverterItemData;

        /// <summary>
        /// 是否勾选转化器替换材料
        /// </summary>
        private bool bIsCheckConverter = false;

        /// <summary>
        /// 当前选择装备
        /// </summary>
        private ComItemNew mCurrentEquipmentComItem = null;

        /// <summary>
        /// 目标装备列表
        /// </summary>
        private List<ItemData> mTargetEquipmentList = new List<ItemData>();

        private void Awake()
        {
            RegisterEventHandle();
            InitConverterEquipmentUIList();
            InitEquipmentComUIListScript();
            InitMaterailExpendItemComUIListScript();
            AddListener();
        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            UnRegisterEventHandle();
            UnInitConverterEquipmentUIList();
            UnInitEquipmentComUIListScript();
            UnInitMaterailExpendItemComUIListScript();
            RemoveListener();
            mAllEquipmentList.Clear();
            mMaterailExpendItems.Clear();
            mConverterItemDataList.Clear();
            mTargetEquipmentList.Clear();
            bIsCheckConverter = false;
            mCurrentEquipmentItemData = null;
            mTargetEquipmentItemData = null;
            mConverterItemData = null;
            mCurrentEquipmentComItem = null;
        }

        public void InitView(SmithShopNewLinkData linkData)
        {
            LoadAllEquipmemnt();
        }

        public void OnDisableView()
        {
            UnRegisterEventHandle();
        }

        public void OnEnableView()
        {
            RegisterEventHandle();
            LoadAllEquipmemnt();
        }

        #region OnClick 

        private void AddListener()
        {
            if (mConverterBtn != null)
            {
                mConverterBtn.onClick.RemoveAllListeners();
                mConverterBtn.onClick.AddListener(OnConverterBtnClick);
            }

            if (mConverterReplacePropTog != null)
            {
                mConverterReplacePropTog.onValueChanged.RemoveAllListeners();
                mConverterReplacePropTog.onValueChanged.AddListener(OnConverterReplacePropTogClick);
            }

            if (mConversionBtn != null)
            {
                mConversionBtn.onClick.RemoveAllListeners();
                mConversionBtn.onClick.AddListener(OnConversionBtnClick);
            }
        }

        private void RemoveListener()
        {
            if (mConverterBtn != null)
            {
                mConverterBtn.onClick.RemoveListener(OnConverterBtnClick);
            }

            if (mConverterReplacePropTog != null)
            {
                mConverterReplacePropTog.onValueChanged.RemoveListener(OnConverterReplacePropTogClick);
            }

            if (mConversionBtn != null)
            {
                mConversionBtn.onClick.RemoveListener(OnConversionBtnClick);
            }
        }
        
        private void OnConverterBtnClick()
        {
            mConverterItemDataList.Clear();
            for (int i = 0; i < mConverterItems.Count; i++)
            {
                var simpleData = mConverterItems[i];
                if (simpleData == null)
                {
                    continue;
                }

                var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);
                for (int j = 0; j < items.Count; j++)
                {
                    var data = ItemDataManager.GetInstance().GetItem(items[j]);
                    if (data == null)
                    {
                        continue;
                    }

                    if ((int)data.ThirdType != simpleData.ItemID)
                    {
                        continue;
                    }

                    if (data.useLimitType != ItemTable.eUseLimiteType.SUITLV)
                    {
                        continue;
                    }

                    if (data.useLimitValue != simpleData.level)
                    {
                        continue;
                    }

                    if (data.Count <= 0)
                    {
                        continue;
                    }

                    int timeLeft = 0;
                    bool bStartCountdown = false;

                    data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                    //失效了
                    if (timeLeft <= 0 && bStartCountdown)
                    {
                        continue;
                    }
                    
                    mConverterItemDataList.Add(data);
                }
            }

            if (mConverterItemDataList.Count <= 0)
            {
                ItemComeLink.OnLink(310, 0);
                return;
            }

            EpicTransformationConverterDataModel epicTransformationConverterDataModel = new EpicTransformationConverterDataModel();
            epicTransformationConverterDataModel.converterList = mConverterItemDataList;
            epicTransformationConverterDataModel.onConverterClick += RefreshConverterItemInfo;

            ClientSystemManager.GetInstance().OpenFrame<EpicTransformationConverterFrame>(FrameLayer.Middle, epicTransformationConverterDataModel);
        }

        private void OnConverterReplacePropTogClick(bool value)
        {
            bIsCheckConverter = value;

            mMaterailExpendItemRoot.CustomActive(!bIsCheckConverter);
            mConverterItemRoot.CustomActive(bIsCheckConverter);

            if (!bIsCheckConverter)
            {
                RefreshExpendMaterail();
            }
        }

        private void OnConversionBtnClick()
        {
            if (mCurrentEquipmentItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipmentConversion_PutEquipment"));
                return;
            }

            if (mCurrentEquipmentItemData.PackageType == EPackageType.WearEquip
                || mCurrentEquipmentItemData.IsItemInUnUsedEquipPlan == true)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipmentConversion_WearEquipDesc"));
                return;
            }

            if (mTargetEquipmentItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipConversion_TargetEquipmentDesc"));
                return;
            }

            //使用转化器替代材料
            if (bIsCheckConverter)
            {
                if (mConverterItemData == null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipConversion_ConverterDesc"));
                    return;
                }
                else
                {
                    int timeLeft;
                    bool bStartCountdown;
                    mConverterItemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                    //失效了
                    if (timeLeft <= 0 && bStartCountdown)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_item"));
                        return;
                    }
                }
            }
            else
            {
                //材料是否足够
                bool isEnough = true;
                int mCount = 0;
                for (int i = 0; i < mMaterailExpendItems.Count; i++)
                {
                    var simpleData = mMaterailExpendItems[i];

                    ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(simpleData.ItemID);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.Type == ItemTable.eType.MATERIAL)
                    {
                        mCount = ItemDataManager.GetInstance().GetItemCount(itemData.TableID);
                    }
                    else
                    {
                        mCount = ItemDataManager.GetInstance().GetOwnedItemCount(itemData.TableID);
                    }

                    if (mCount < simpleData.Count)
                    {
                        isEnough = false;
                        break;
                    }
                }

                //表示材料不足
                if (!isEnough)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipConversion_LackOfMaterial"));
                    return;
                }

                int bindGoldId = 0;
                int bindGoldCount = 0;
                int totalBindGoldCount = 0;
                for (int i = 0; i < mMaterailExpendItems.Count; i++)
                {
                    var simpleData = mMaterailExpendItems[i];

                    ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(simpleData.ItemID);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.Type == ItemTable.eType.MATERIAL)
                    {
                        continue;
                    }

                    bindGoldId = itemData.TableID;
                    totalBindGoldCount = ItemDataManager.GetInstance().GetOwnedItemCount(itemData.TableID,false);
                    bindGoldCount = simpleData.Count;
                    break; 
                }

                //表示金币不足，弹通用的绑金不足，使用金币抵扣的提示
                if (totalBindGoldCount < bindGoldCount)
                {
                    CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = bindGoldId, nCount = bindGoldCount }, OnSceneEquipConvertReq);
                    return;
                }
            }

            OnSceneEquipConvertReq();
        }

        private void OnSceneEquipConvertReq()
        {
            string content = string.Empty;

            if(mCurrentEquipmentItemData != null && mTargetEquipmentItemData != null)
            {
                if (PlayerBaseData.GetInstance().Level >= mTargetEquipmentItemData.LevelLimit)
                {
                    content = TR.Value("EquipConversion_ConformLevel", mCurrentEquipmentItemData.GetColorName(), mTargetEquipmentItemData.GetColorName());
                }
                else
                {
                    content = TR.Value("EquipConversion_NoConformLevel", mCurrentEquipmentItemData.GetColorName(), mTargetEquipmentItemData.GetColorName());
                }

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(content, () =>
                {
                    ulong convertorGuid = 0;

                    if (mConverterItemData != null)
                    {
                        convertorGuid = bIsCheckConverter == true ? mConverterItemData.GUID : 0;
                    }

                    EpicEquipmentTransformationDataManager.GetInstance().OnSceneEquipConvertReq((byte)EquipConvertType.EQCT_SAME, mCurrentEquipmentItemData.GUID, (uint)mTargetEquipmentItemData.TableID, convertorGuid);
                });
            }
        }
        #endregion

        #region RegisterEvent 

        private void RegisterEventHandle()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEpicEquipmentConversionSuccessed, OnEpicEquipmentConversionSuccessed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnEpicEquipmentConversionSuccessed);
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnAddNewItem;

            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        private void UnRegisterEventHandle()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEpicEquipmentConversionSuccessed, OnEpicEquipmentConversionSuccessed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnEpicEquipmentConversionSuccessed);
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnAddNewItem;

            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }
        
        private void OnEpicEquipmentConversionSuccessed(UIEvent uiEvent)
        {
            LoadAllEquipmemnt();
        }

        private void OnAddNewItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type == ProtoTable.ItemTable.eType.MATERIAL)
                {
                    RefreshExpendMaterail();
                }

                if (itemData.Type == ItemTable.eType.EQUIP &&
                   itemData.Quality == ItemTable.eColor.YELLOW)
                {
                    LoadAllEquipmemnt();
                }
            }
        }

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            if (eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_BIND_GOLD ||
                eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_GOLD)
            {
                RefreshExpendMaterail();
            }
        }
        #endregion

        #region LoadAllEquipment

        private void LoadAllEquipmemnt()
        {
            if (mAllEquipmentList != null)
            {
                mAllEquipmentList.Clear();
            }

            var wearEquipList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            var packageEquipmentList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            FiltraterEquipment(wearEquipList);
            FiltraterEquipment(packageEquipmentList);
            mAllEquipmentList.Sort(EquipmentConversionUtility.SoreEquipments);

            if (mEquipmentComUIList != null)
            {
                mEquipmentComUIList.SetElementAmount(mAllEquipmentList.Count);
            }

            TrySetDefaultEquipment();
        }

        private void FiltraterEquipment(List<ulong> items)
        {
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

            for (int i = 0; i < items.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Quality != ItemTable.eColor.YELLOW)
                {
                    continue;
                }

                //装备带有气息过滤掉
                if (itemData.EquipType == EEquipType.ET_BREATH)
                {
                    continue;
                }

                //装备身上有转移石，不显示在列表中
                if (itemData.HasTransfered)
                {
                    continue;
                }

                //辅助装备过滤掉
                if (itemData.SubType == (int)ItemTable.eSubType.ST_ASSIST_EQUIP)
                {
                    continue;
                }

                //魔法石装备过滤掉
                if (itemData.SubType == (int)ItemTable.eSubType.ST_MAGICSTONE_EQUIP)
                {
                    continue;
                }

                //耳环装备过滤掉
                if (itemData.SubType == (int)ItemTable.eSubType.ST_EARRINGS_EQUIP)
                {
                    continue;
                }

                //辟邪玉装备过滤掉
                if (itemData.SubType == (int)ItemTable.eSubType.ST_BXY_EQUIP)
                {
                    continue;
                }

                if (!EpicEquipmentTransformationDataManager.GetInstance().CheckEpicEquipmentCanbeDisplayedInTheEquipmentList(itemData))
                {
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

                mAllEquipmentList.Add(itemData);
            }
        }

        private void TrySetDefaultEquipment()
        {
            int index = -1;

            if (EquipUpgradeItem.ms_selected != null)
            {
                var find = mAllEquipmentList.Find(x => { return x.GUID == EquipUpgradeItem.ms_selected.GUID; });

                if (find != null)
                {
                    EquipUpgradeItem.ms_selected = find;
                }
                else
                {
                    EquipUpgradeItem.ms_selected = null;
                }
            }

            if (EquipUpgradeItem.ms_selected != null)
            {
                for (int i = 0; i < mAllEquipmentList.Count; i++)
                {
                    ulong guid = mAllEquipmentList[i].GUID;
                    if (guid != EquipUpgradeItem.ms_selected.GUID)
                    {
                        continue;
                    }

                    index = i;
                }
            }
            else
            {
                if (mAllEquipmentList.Count > 0)
                {
                    index = 0;
                }
            }

            SetSelectItem(index);
        }

        private void SetSelectItem(int index)
        {
            if (index >= 0 && index < mAllEquipmentList.Count)
            {
                EquipUpgradeItem.ms_selected = mAllEquipmentList[index];
                if (!mEquipmentComUIList.IsElementInScrollArea(index))
                {
                    mEquipmentComUIList.EnsureElementVisable(index);
                }
                mEquipmentComUIList.SelectElement(index);
            }
            else
            {
                mEquipmentComUIList.SelectElement(-1);
                EquipUpgradeItem.ms_selected = null;
            }

            OnEquipmentItemClick(EquipUpgradeItem.ms_selected);
        }
        #endregion

        #region ComUIListScript

        private void InitEquipmentComUIListScript()
        {
            if (mEquipmentComUIList != null)
            {
                mEquipmentComUIList.Initialize();
                mEquipmentComUIList.onBindItem += OnBindItemDelegate;
                mEquipmentComUIList.onItemVisiable += OnItemVisiableDelegate;
                mEquipmentComUIList.onItemSelected += OnItemSelectedDelegate;
                mEquipmentComUIList.onItemChageDisplay += OnItemChangeDisplayDelegate;
            }
        }

        private void UnInitEquipmentComUIListScript()
        {
            if (mEquipmentComUIList != null)
            {
                mEquipmentComUIList.onBindItem -= OnBindItemDelegate;
                mEquipmentComUIList.onItemVisiable -= OnItemVisiableDelegate;
                mEquipmentComUIList.onItemSelected -= OnItemSelectedDelegate;
                mEquipmentComUIList.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }
        }

        private EquipUpgradeItem OnBindItemDelegate(GameObject itemObjcet)
        {
            return itemObjcet.GetComponent<EquipUpgradeItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var equipmentItem = item.gameObjectBindScript as EquipUpgradeItem;
            if (equipmentItem != null && item.m_index >= 0 && item.m_index < mAllEquipmentList.Count)
            {
                equipmentItem.OnItemVisiable(mAllEquipmentList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var equipmentItem = item.gameObjectBindScript as EquipUpgradeItem;
            if (equipmentItem != null)
            {
                EquipUpgradeItem.ms_selected = equipmentItem.ItemData;
                OnEquipmentItemClick(equipmentItem.ItemData);
            }
        }
        private void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var equipmentItem = item.gameObjectBindScript as EquipUpgradeItem;
            if (equipmentItem != null)
            {
                equipmentItem.OnItemChangeDisplay(bSelected);
            }
        }
        #endregion

        #region MaterailExpendItemComUIListScript 

        private void InitMaterailExpendItemComUIListScript()
        {
            if (mMaterailExpendItemComUIListScript != null)
            {
                mMaterailExpendItemComUIListScript.Initialize();
                mMaterailExpendItemComUIListScript.onBindItem += OnMaterailExpendBindItemDelegate;
                mMaterailExpendItemComUIListScript.onItemVisiable += OnMaterailExpendItemVisiableDelegate;
            }
        }

        private void UnInitMaterailExpendItemComUIListScript()
        {
            if (mMaterailExpendItemComUIListScript != null)
            {
                mMaterailExpendItemComUIListScript.onBindItem -= OnMaterailExpendBindItemDelegate;
                mMaterailExpendItemComUIListScript.onItemVisiable -= OnMaterailExpendItemVisiableDelegate;
            }
        }

        private EquipUpgradeCostItem OnMaterailExpendBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EquipUpgradeCostItem>();
        }

        private void OnMaterailExpendItemVisiableDelegate(ComUIListElementScript item)
        {
            var equipUpgradeCostItem = item.gameObjectBindScript as EquipUpgradeCostItem;
            if (equipUpgradeCostItem != null && item.m_index >= 0 && item.m_index < mMaterailExpendItems.Count)
            {
                equipUpgradeCostItem.OnItemVisiable(mMaterailExpendItems[item.m_index]);
            }
        }

        public void OnSetMaterailExpendElementAmount()
        {
            if (mMaterailExpendItemComUIListScript != null)
            {
                mMaterailExpendItemComUIListScript.SetElementAmount(mMaterailExpendItems.Count);
            }
        }
        #endregion

        #region RefreshInfo

        private void ClearInfo()
        {
            mConverterItemRoot.CustomActive(false);
            mConverterReplacePropTog.CustomActive(false);
            mConverterItemParent.CustomActive(false);
            mConverterItemName.text = selectedConverterDes;
            mCurrentEquipmentItemData = null;
            mTargetEquipmentItemData = null;
            mConverterItemData = null;
            mMaterailExpendItemComUIListScript.SetElementAmount(0);
        }

        private void OnEquipmentItemClick(ItemData itemData)
        {
            ClearInfo();
            RefreshCurrentEquipmentInfo(itemData);
            RefreshTargetEquipment(itemData);
        }

        /// <summary>
        /// 刷新当前选择的装备
        /// </summary>
        /// <param name="itemData"></param>
        private void RefreshCurrentEquipmentInfo(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            mCurrentEquipmentItemData = itemData;

            if (mCurrentEquipmentComItem == null)
            {
                mCurrentEquipmentComItem = ComItemManager.CreateNew(mCurrentEquipmentItemParent);
            }

            mCurrentEquipmentComItem.Setup(mCurrentEquipmentItemData, Utility.OnItemClicked);

            if (mCurrentEquipmentName != null)
            {
                mCurrentEquipmentName.text = mCurrentEquipmentItemData.GetColorName();
            }
        }

        /// <summary>
        /// 刷新目标装备
        /// </summary>
        /// <param name="itemData"></param>
        private void RefreshTargetEquipment(ItemData itemData)
        {
            if (itemData == null)
                return;

            mTargetEquipmentList = EpicEquipmentTransformationDataManager.GetInstance().GetTargetEquipmentList(itemData);
            OnSetConverterEquipmentElementAmount();
        }

        private void RefreshTargetEquipmentInfo(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            mTargetEquipmentItemData = itemData;

            mConverterItemRoot.CustomActive(bIsCheckConverter);
            mConverterReplacePropTog.CustomActive(true);
            
            mMaterailExpendItems.Clear();
            mConverterItems.Clear();
            EpicEquipmentTransformationDataManager.GetInstance().GetConvertedTargetEquipmentMaterial(EpicEquipmentTransformationType.SetOfConversion,mTargetEquipmentItemData, ref mMaterailExpendItems, ref mConverterItems);

            if (mMaterailExpendItems.Count > 0 && mConverterItems.Count > 0)
            {
                mMaterailExpendItemRoot.CustomActive(true);
                mConverterReplacePropTog.CustomActive(true);

                //选择了转化器代替材料
                if (bIsCheckConverter)
                {
                    mMaterailExpendItemRoot.CustomActive(false);
                    //并且之前选过
                    if (mConverterItemData != null)
                    {
                        bool isFind = false;
                        for (int i = 0; i < mConverterItemDataList.Count; i++)
                        {
                            if (mConverterItemDataList[i].TableID != mConverterItemData.TableID)
                            {
                                continue;
                            }

                            isFind = true;
                            break;
                        }

                        if (!isFind)
                        {
                            mConverterItemData = null;
                            mConverterItemParent.CustomActive(false);
                        }
                        else
                        {
                            RefreshConverterItemInfo(mConverterItemData);
                        }
                    }
                }
                else
                {
                    RefreshExpendMaterail();
                }
            }
            else if (mMaterailExpendItems.Count > 0 && mConverterItems.Count <= 0)
            {
                if (mConverterReplacePropTog.isOn == true)
                {
                    mConverterReplacePropTog.isOn = false;
                }

                mMaterailExpendItemRoot.CustomActive(true);
                mConverterReplacePropTog.CustomActive(false);

                RefreshExpendMaterail();
            }
            else if (mMaterailExpendItems.Count <= 0 && mConverterItems.Count > 0)
            {
                mConverterReplacePropTog.isOn = true;
                mMaterailExpendItemRoot.CustomActive(false);
                mConverterReplacePropTog.CustomActive(false);
            }
        }

        /// <summary>
        /// 刷新需要消耗的材料
        /// </summary>
        private void RefreshExpendMaterail()
        {
            if (mMaterailExpendItemComUIListScript != null)
            {
                mMaterailExpendItemComUIListScript.SetElementAmount(mMaterailExpendItems.Count);
            }
        }

        /// <summary>
        /// 刷新转化器道具
        /// </summary>
        private void RefreshConverterItemInfo(ItemData itemData)
        {
            if (itemData == null)
            {
                mConverterItemParent.CustomActive(false);
                return;
            }

            mConverterItemParent.CustomActive(true);

            mConverterItemData = itemData;

            if (mConverterItemBackground != null)
            {
                ETCImageLoader.LoadSprite(ref mConverterItemBackground, mConverterItemData.GetQualityInfo().Background);
            }

            if (mConverterItemIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mConverterItemIcon, mConverterItemData.Icon);
            }

            if (mConverterItemName != null)
            {
                mConverterItemName.text = mConverterItemData.GetColorName();
            }

            if (mConverterItemCount != null)
            {
                mConverterItemCount.text = string.Format("{0}/1", mConverterItemData.Count);
            }
        }
        #endregion

        #region 可转换的装备

        private void InitConverterEquipmentUIList()
        {
            if(mConverterEquipmentUIList != null)
            {
                mConverterEquipmentUIList.Initialize();
                mConverterEquipmentUIList.onBindItem += OnBindConverterEquipmentItemDelegate;
                mConverterEquipmentUIList.onItemVisiable += OnConverterEquipmentItemVisiableDelegate;
                mConverterEquipmentUIList.onItemSelected += OnConverterEquipmentItemSelectedDelegate;
                mConverterEquipmentUIList.onItemChageDisplay += OnConverterEquipmentChangedDisplayDelegate;
            }
        }

        private void UnInitConverterEquipmentUIList()
        {
            if (mConverterEquipmentUIList != null)
            {
                mConverterEquipmentUIList.onBindItem -= OnBindConverterEquipmentItemDelegate;
                mConverterEquipmentUIList.onItemVisiable -= OnConverterEquipmentItemVisiableDelegate;
                mConverterEquipmentUIList.onItemSelected -= OnConverterEquipmentItemSelectedDelegate;
                mConverterEquipmentUIList.onItemChageDisplay -= OnConverterEquipmentChangedDisplayDelegate;
            }
        }

        private EpicTransformationTargetEquipmentItem OnBindConverterEquipmentItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EpicTransformationTargetEquipmentItem>();
        }

        private void OnConverterEquipmentItemVisiableDelegate(ComUIListElementScript item)
        {
            var equipmentItem = item.gameObjectBindScript as EpicTransformationTargetEquipmentItem;
            if (equipmentItem != null && item.m_index >= 0 && item.m_index < mTargetEquipmentList.Count)
            {
                equipmentItem.OnItemVisiable(mTargetEquipmentList[item.m_index]);
            }
        }

        private void OnConverterEquipmentItemSelectedDelegate(ComUIListElementScript item)
        {
            var equipmentItem = item.gameObjectBindScript as EpicTransformationTargetEquipmentItem;
            if (equipmentItem != null)
            {
                RefreshTargetEquipmentInfo(equipmentItem.EquipmentItemData);
            }
        }

        private void OnConverterEquipmentChangedDisplayDelegate(ComUIListElementScript item,bool bSelected)
        {
            var equipmentItem = item.gameObjectBindScript as EpicTransformationTargetEquipmentItem;
            if (equipmentItem != null)
            {
                equipmentItem.OnChangedDisplay(bSelected);
            }
        }

        private void OnSetConverterEquipmentElementAmount()
        {
            if(mConverterEquipmentUIList != null)
            {
                mConverterEquipmentUIList.ResetSelectedElementIndex();
                mConverterEquipmentUIList.SetElementAmount(mTargetEquipmentList.Count);
            }
        }
        #endregion
    }
}