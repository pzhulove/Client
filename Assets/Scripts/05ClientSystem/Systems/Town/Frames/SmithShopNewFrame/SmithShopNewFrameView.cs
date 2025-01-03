using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProtoTable;

namespace GameClient
{
    #region MainTab

    /// <summary>
    /// 主页签数据
    /// </summary>
    [Serializable]
    public class SmithShopNewMainTabDataModel
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int index;
        /// <summary>
        /// 页签类型
        /// </summary>
        public SmithShopNewTabType tabType;
        /// <summary>
        /// tab名字
        /// </summary>
        public string tabName;
        /// <summary>
        /// 加载节点
        /// </summary>
        public GameObject content;
        /// <summary>
        /// 是否有子页签
        /// </summary>
        public bool isSunTAB;
    }

    /// <summary>
    /// 页签类型
    /// </summary>
    public enum SmithShopNewTabType
    {
        /// <summary>
        /// 强化
        /// </summary>
        SSNTT_STRENGTHEN = 0,
        /// <summary>
        /// 激化
        /// </summary>
        SSNTT_GROWTH,
        /// <summary>
        /// 附魔卡
        /// </summary>
        SSNTT_ENCHANTMENTCARD,
        /// <summary>
        /// 宝珠
        /// </summary>
        SSNTT_BEAD,
        /// <summary>
        /// 铭文
        /// </summary>
        SSNTT_INSCRIPTION,
        /// <summary>
        /// 品级调整
        /// </summary>
        SSNTT_ADJUST,
        /// <summary>
        /// 装备进化
        /// </summary>
        SSNTT_EQUIPMENTEVOLUTION,
        /// <summary>
        /// 装备封装
        /// </summary>
        SSNTT_EQUIPMENTSEAL,
        /// <summary>
        /// 传家
        /// </summary>
        SSNTT_EQUIPMENTTRANSFER,
        /// <summary>
        /// 材料合成
        /// </summary>
        SSNTT_MATERIALSSYNTHESIS,
        /// <summary>
        /// 宝珠升级
        /// </summary>
        SSNIT_BEADUPGRADE,

        /// <summary>
        /// 辟邪玉合成
        /// </summary>
        SSNIT_BXYMERGE,

        SSNTT_COUNT,
    }
    #endregion

    #region SubTab
    
    /// <summary>
    /// 激化子页签数据
    /// </summary>
    [Serializable]
    public class SecondTabDataModel
    {
        public int index;
        public string name;
        public GameObject content;
    }

    /// <summary>
    /// 激化子页签类型
    /// </summary>
    public enum GrowthSubTabType
    {
        /// <summary>
        /// 激化
        /// </summary>
        GSTT_GROWTH = 0,
        /// <summary>
        /// 清除
        /// </summary>
        GSTT_CLEAT,
        /// <summary>
        /// 激活
        /// </summary>
        GSTT_ACTIVATE,
        /// <summary>
        /// 转化
        /// </summary>
        GSTT_CHANGE,
    }
    
    /// <summary>
    /// 附魔卡子页签类型
    /// </summary>
    public enum EnchantmentCardSubTabType
    {
        /// <summary>
        /// 装备附魔
        /// </summary>
        ECSTT_EQUIPMENTENCHANT = 0,
        /// <summary>
        /// 附魔卡合成
        /// </summary>
        ECSTT_ENCHANTMENTCARDMERGE,
        /// <summary>
        /// 附魔卡升级
        /// </summary>
        ECSTT_ENCHANTMENTCARDUPGRADE,
    }

    /// <summary>
    /// 装备进化子页签
    /// </summary>
    public enum EquipmentEvolutionSunTabType
    {
        /// <summary>
        /// 装备升级
        /// </summary>
        EEST_EQUIPMENTUPGRADE = 0,
        /// <summary>
        /// 同套转化
        /// </summary>
        EEST_SAMEEQUIPMENTCONVERTER,
        /// <summary>
        /// 跨套转化
        /// </summary>
        EEST_CROSSEQUIPMENTCONVERTER,
    }

    /// <summary>
    /// 铭文子页签数据
    /// </summary>
    [Serializable]
    public class InscriptionTabModel
    {
        public int Index;
        public InscriptionTabType ITType;
        public string Name;
        public GameObject ContentRoot;
    }

    /// <summary>
    /// 铭文子页签类型
    /// </summary>
    public enum InscriptionTabType
    {
        /// <summary>
        /// 铭文镶嵌
        /// </summary>
        InscriptionMosaic = 0,
        /// <summary>
        /// 铭文合成
        /// </summary>
        InscriptionSynthesis,
    }

    #endregion
    
    public delegate void OnFirstTabToggleClick(SmithShopNewMainTabDataModel mainTabDataModel);
    public delegate void OnSecondTabToggleClick(SmithShopNewMainTabDataModel mainTabDataModel, SecondTabDataModel secondTabDataModel);
    public class SmithShopNewFrameView : MonoBehaviour
    {
        [SerializeField] private List<SmithShopNewMainTabDataModel> mMainTabs;
        [SerializeField] private List<SecondTabDataModel> mGrowthSubTabs;
        [SerializeField] private List<SecondTabDataModel> mEnchantmentCardSubTabs;
        [SerializeField] private List<SecondTabDataModel> mInscriptionSubTabs;
        [SerializeField] private List<SecondTabDataModel> mEquipmentEvolutionSubTabs;
        [SerializeField] private GameObject mMainMenuTabContent;
        [SerializeField] private GameObject mMenuItemGroup;
        [SerializeField] private HelpAssistant mHelpAssistant;
        [SerializeField] private GameObject mStrengthenGrowthRoot;
        [SerializeField] private RectTransform mFilterContent;
        [SerializeField] private Vector2 mNormalPos = new Vector2(-710, 405);
        [SerializeField] private Vector2 mBeadPos = new Vector2(-710, 346);
        [SerializeField] private int[] ms_iOpenLevel;/// 功能解锁表IDs
        [SerializeField] private ComDropDownControl mComDropDownControlLevel;
        [SerializeField] private ComDropDownControl mComDropDownControlQuality;

        private GameObject mStrengthenGrowthViewGo;
        private StrengthenGrowthView mStrengthenGrowthView;
        public static ItemData mLastSelectedItemData = null;
        private int iDefaultFirstTabId = 0;
        private int iDefaultSecondTabId = 0;
        private SmithShopNewLinkData mLinkData;
        private SecondTabDataModel mSecondTabDataModel;
        public static List<SmithShopNewSecondTabItem> mSecondTabItemList = null;

        public static ComControlData iDefaultLevelData;
        public static int iDefaultQuality = 0;

        /// <summary>
        /// 品质列表
        /// </summary>
        private List<ComControlData> mQulityTabDataList = new List<ComControlData>();
        /// <summary>
        /// 等级列表
        /// </summary>
        private List<ComControlData> mLevelTabDataList = new List<ComControlData>();

        //上次选中的主页签
        private SmithShopNewTabType _preSelectedFirstTabType = SmithShopNewTabType.SSNTT_COUNT;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            mStrengthenGrowthViewGo = null;
            mStrengthenGrowthView = null;
            mLastSelectedItemData = null;
            iDefaultFirstTabId = 0;
            iDefaultSecondTabId = 0;
            mSecondTabDataModel = null;

            ResetPreSelectedFirstTabType();
        }
        
        public void InitView(SmithShopNewLinkData linkData)
        {
            mLinkData = linkData;
            iDefaultFirstTabId = 0;
            iDefaultSecondTabId = 0;
            if (linkData != null)
            {
                mLastSelectedItemData = linkData.itemData;
                iDefaultFirstTabId = linkData.iDefaultFirstTabId;
                iDefaultSecondTabId = linkData.iDefaultSecondTabId;
            }

            mLevelTabDataList = StrengthenDataManager.GetInstance().GetLevelDataList();
            mQulityTabDataList = StrengthenDataManager.GetInstance().GetQualiyDataList();

            InitLevelDrop();
            InitQulityDrop();

            LoadStrengthenGrowthView();

            for (int i = 0; i < mMainTabs.Count; i++)
            {
                var currentMainTabDataModel = mMainTabs[i];
                if (currentMainTabDataModel == null)
                {
                    continue;
                }

                if (GetOpenLevel(currentMainTabDataModel.tabType) > PlayerBaseData.GetInstance().Level)
                {
                    continue;
                }

                var isSelected = iDefaultFirstTabId == (int)currentMainTabDataModel.tabType;
                
                var curMenuItemGroup = Instantiate(mMenuItemGroup) as GameObject;
                if (curMenuItemGroup != null)
                {
                    curMenuItemGroup.CustomActive(true);
                    curMenuItemGroup.name = currentMainTabDataModel.tabType.ToString();

                    if (currentMainTabDataModel.tabType == SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER)
                    {
                        curMenuItemGroup.CustomActive(iDefaultFirstTabId == (int)SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER);
                    }

                    Utility.AttachTo(curMenuItemGroup, mMainMenuTabContent);

                    var smithshopNewMenuItemGroup = curMenuItemGroup.GetComponent<SmithShopNewMenuItemGroup>();
                    if (smithshopNewMenuItemGroup != null && smithshopNewMenuItemGroup.mFirstTabItem != null)
                    {
                        smithshopNewMenuItemGroup.mFirstTabItem.InitTabItem(currentMainTabDataModel,
                            null,
                            isSelected,
                            mStrengthenGrowthView,
                            linkData,
                            iDefaultSecondTabId,
                            OnFirstTabToggleClick,
                            OnSecondTabToggleClick);
                    }
                }
            }
        }

        private void OnFirstTabToggleClick(SmithShopNewMainTabDataModel mainTabDataModel)
        {
            if (mainTabDataModel == null)
            {
                return;
            }

            if (mStrengthenGrowthViewGo != null)
            {
                mStrengthenGrowthViewGo.CustomActive(mainTabDataModel.tabType == SmithShopNewTabType.SSNTT_STRENGTHEN || mainTabDataModel.tabType == SmithShopNewTabType.SSNTT_GROWTH);
            }
            
            //从某个页签切走
            OnFirstLayerTabChanged(mainTabDataModel);

            //进入到某个页签
            //页签类型为强化
            if (mainTabDataModel.tabType == SmithShopNewTabType.SSNTT_STRENGTHEN)
            {
                StrengthenGrowthType type = StrengthenGrowthType.SGT_Strengthen;
                OnSetStrengthGrowthType(type);

                //强化页签
                StrengthenDataManager.GetInstance().IsEquipStrengthened = false;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessStart);

            }
            else if (mainTabDataModel.tabType == SmithShopNewTabType.SSNTT_GROWTH)
            {
                //激化页签
                StrengthenDataManager.GetInstance().IsEquipStrengthened = false;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessStart);

                OnSecondTabToggleClick(mainTabDataModel, mSecondTabDataModel);
            }
            else
            {
                OnSetFilterContent(true);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessReset);
            }

            SetHelpAssistant(mainTabDataModel);

            _preSelectedFirstTabType = mainTabDataModel.tabType;

            if(mainTabDataModel.tabType != SmithShopNewTabType.SSNTT_BEAD)
            {
                mFilterContent.anchoredPosition = mNormalPos;
            }
            else
            {
                mFilterContent.anchoredPosition = mBeadPos;
            }
        }

        private void OnSecondTabToggleClick(SmithShopNewMainTabDataModel mainTabDataModel, SecondTabDataModel secondTabDataModel)
        {
            if (mainTabDataModel == null || secondTabDataModel == null)
            {
                return;
            }

            mSecondTabDataModel = secondTabDataModel;

            //主页签是激化
            if (mainTabDataModel.tabType == SmithShopNewTabType.SSNTT_GROWTH)
            {
                StrengthenGrowthType type = StrengthenGrowthType.SGT_Gtowth;
                if (secondTabDataModel.index == (int)GrowthSubTabType.GSTT_GROWTH)
                {
                    type = StrengthenGrowthType.SGT_Gtowth;
                }
                else if (secondTabDataModel.index == (int)GrowthSubTabType.GSTT_CLEAT)
                {
                    type = StrengthenGrowthType.SGT_Clear;
                }
                else if (secondTabDataModel.index == (int)GrowthSubTabType.GSTT_ACTIVATE)
                {
                    type = StrengthenGrowthType.SGT_Activate;
                }
                else if (secondTabDataModel.index == (int)GrowthSubTabType.GSTT_CHANGE)
                {
                    type = StrengthenGrowthType.SGT_Change;
                }

                OnSetStrengthGrowthType(type);
            }

            SetHelpAssistant(mainTabDataModel, secondTabDataModel);
        }

        private void OnSetStrengthGrowthType(StrengthenGrowthType type)
        {
           if (mStrengthenGrowthView != null)
           {
                mStrengthenGrowthView.OnSetStrengthGrowthType(type);
           }
        }

        private void LoadStrengthenGrowthView()
        {
            if (mStrengthenGrowthRoot != null)
            {
                var uiPrefabWrapper = mStrengthenGrowthRoot.GetComponent<UIPrefabWrapper>();
                if (uiPrefabWrapper != null)
                {
                    GameObject uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                    if (uiPrefab != null)
                    {
                        uiPrefab.transform.SetParent(mStrengthenGrowthRoot.transform, false);
                        mStrengthenGrowthViewGo = uiPrefab;
                    }
                }
            }
           

            if (mStrengthenGrowthViewGo != null)
            {
                mStrengthenGrowthView = mStrengthenGrowthViewGo.GetComponent<StrengthenGrowthView>();
                if (mStrengthenGrowthView != null)
                {
                    mStrengthenGrowthView.InitView(mLinkData);
                }
            }
        }
        
        private int GetOpenLevel(SmithShopNewTabType tabType)
        {
            if (tabType >= 0 && tabType < SmithShopNewTabType.SSNTT_COUNT)
            {
                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(ms_iOpenLevel[(int)tabType]);
                if (FuncUnlockdata != null)
                {
                    return FuncUnlockdata.FinishLevel;
                }
            }

            return 9999;
        }

        public static ItemData GetLastSelectItem(SmithShopNewTabType tabType)
        {
            ItemData linkItem = null;
            switch (tabType)
            {
                case SmithShopNewTabType.SSNTT_STRENGTHEN:
                case SmithShopNewTabType.SSNTT_GROWTH:
                    {
                        linkItem = StrengthenGrowthView.mLastSelectedItemData;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_ENCHANTMENTCARD:
                    {
                        linkItem = ComEquipment.ms_selected;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_BEAD:
                    {
                        linkItem = ComBeadEquipment.ms_selected;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_INSCRIPTION:
                    {
                        linkItem = InscriptionEquipmentItem.mSelectItemData;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_ADJUST:
                case SmithShopNewTabType.SSNTT_EQUIPMENTSEAL:
                case SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER:
                    {
                        linkItem = ComSealEquipment.ms_selected;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_EQUIPMENTEVOLUTION:
                    {
                        linkItem = EquipUpgradeItem.ms_selected;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_MATERIALSSYNTHESIS:
                    break;
                default:
                    {
                        linkItem = null;
                    }
                    break;
            }

            return linkItem;
        }

        public static void SetLastSelectItem(SmithShopNewTabType tabType)
        {
            if (mLastSelectedItemData == null)
            {
                return;
            }

            switch (tabType)
            {
                case SmithShopNewTabType.SSNTT_STRENGTHEN:
                case SmithShopNewTabType.SSNTT_GROWTH:
                    {
                        StrengthenGrowthView.mLastSelectedItemData = mLastSelectedItemData;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_ENCHANTMENTCARD:
                    {
                        ComEquipment.ms_selected = mLastSelectedItemData;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_BEAD:
                    {
                        ComBeadEquipment.ms_selected = mLastSelectedItemData;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_INSCRIPTION:
                    {
                        InscriptionEquipmentItem.mSelectItemData = mLastSelectedItemData;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_ADJUST:
                case SmithShopNewTabType.SSNTT_EQUIPMENTSEAL:
                case SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER:
                    {
                        ComSealEquipment.ms_selected = mLastSelectedItemData;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_EQUIPMENTEVOLUTION:
                    {
                        EquipUpgradeItem.ms_selected = mLastSelectedItemData;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_MATERIALSSYNTHESIS:
                    break;
                default:
                    {
                        
                    }
                    break;
            }
        }

        private void SetHelpAssistant(SmithShopNewMainTabDataModel mainTabDataModel, SecondTabDataModel secondTabDataModel)
        {
            if (mainTabDataModel == null || secondTabDataModel == null)
            {
                return;
            }

            if(mHelpAssistant == null)
            {
                return;
            }

            if (mainTabDataModel.tabType == SmithShopNewTabType.SSNTT_ENCHANTMENTCARD)
            {
                if (secondTabDataModel.index == (int)EnchantmentCardSubTabType.ECSTT_EQUIPMENTENCHANT)
                {
                    mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_ADDMAGIC;
                }
                else if (secondTabDataModel.index == (int)EnchantmentCardSubTabType.ECSTT_ENCHANTMENTCARDMERGE)
                {
                    mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_MAGICCOPMOSE;
                }
                else if (secondTabDataModel.index == (int)EnchantmentCardSubTabType.ECSTT_ENCHANTMENTCARDUPGRADE)
                {
                    mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_ENCHANTMENTCARUPGRADE;
                }
            }
            else if (mainTabDataModel.tabType == SmithShopNewTabType.SSNTT_EQUIPMENTEVOLUTION)
            {
                if (secondTabDataModel.index == (int)EquipmentEvolutionSunTabType.EEST_EQUIPMENTUPGRADE)
                {
                    mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_EQUIPUPGRADE;
                }
                else if (secondTabDataModel.index == (int)EquipmentEvolutionSunTabType.EEST_SAMEEQUIPMENTCONVERTER)
                {
                    mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_SAMEEQUIPMENTCONVER;
                }
                else if (secondTabDataModel.index == (int)EquipmentEvolutionSunTabType.EEST_CROSSEQUIPMENTCONVERTER)
                {
                    mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_CROSSEQUIPMENTCONVER;
                }
            }
        }

        private void SetHelpAssistant(SmithShopNewMainTabDataModel mainTabDataModel)
        {
            if (mainTabDataModel == null)
            {
                return;
            }

            if (mHelpAssistant == null)
            {
                return;
            }

            switch (mainTabDataModel.tabType)
            {
                case SmithShopNewTabType.SSNTT_STRENGTHEN:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_STRENTH;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_GROWTH:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_STRENTH;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_ENCHANTMENTCARD:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_ADDMAGIC;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_BEAD:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_PEARLINLAY;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_INSCRIPTION:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_INSCRIPTION;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_ADJUST:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_QUALITYCHANGE;
                    }
                    break;
                //case SmithShopNewTabType.SSNTT_EQUIPMENTEVOLUTION:
                //    {
                //        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_EQUIPUPGRADE;
                //    }
                //    break;
                case SmithShopNewTabType.SSNTT_EQUIPMENTSEAL:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_SEAL;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_TRANSFER;
                    }
                    break;
                case SmithShopNewTabType.SSNTT_MATERIALSSYNTHESIS:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_MATERIALSSYNTHESIS;
                    }
                    break;
                case SmithShopNewTabType.SSNIT_BEADUPGRADE:
                    {
                        mHelpAssistant.eType = HelpFrameContentTable.eHelpType.HT_BEADUPGRADE;
                    }
                    break;
            }
        }

        private void OnSetFilterContent(bool value)
        {
            if (mFilterContent != null)
                mFilterContent.CustomActive(value);
        }

        #region TabChanged
        //一级页签改变的时候，进行相关的处理
        private void OnFirstLayerTabChanged(SmithShopNewMainTabDataModel mainTabDataModel)
        {
            if (mainTabDataModel == null)
                return;

            //从强化页签切走
            if (_preSelectedFirstTabType == SmithShopNewTabType.SSNTT_STRENGTHEN
                && mainTabDataModel.tabType != SmithShopNewTabType.SSNTT_STRENGTHEN)
            {
                //装备强化过
                if (StrengthenDataManager.GetInstance().IsEquipStrengthened == true)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessFinish);
                }

                StrengthenDataManager.GetInstance().IsEquipStrengthened = false;
            }
            else if (_preSelectedFirstTabType == SmithShopNewTabType.SSNTT_GROWTH
                     && mainTabDataModel.tabType != SmithShopNewTabType.SSNTT_GROWTH)
            {
                //从激化页签中切走
                //激化过
                if (EquipGrowthDataManager.GetInstance().IsEquipIntensify == true)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessFinish);
                }

                EquipGrowthDataManager.GetInstance().IsEquipIntensify = false;
            }
        }

        private void ResetPreSelectedFirstTabType()
        {
            //强化页签，并且强化过
            if (_preSelectedFirstTabType == SmithShopNewTabType.SSNTT_STRENGTHEN
                && StrengthenDataManager.GetInstance().IsEquipStrengthened == true)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessFinish);
            }
            else if (_preSelectedFirstTabType == SmithShopNewTabType.SSNTT_GROWTH
                     && EquipGrowthDataManager.GetInstance().IsEquipIntensify == true)
            {
                //激化页签并且激化过，
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessFinish);
            }
            else
            {
                //标志重置
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessReset);
            }

            _preSelectedFirstTabType = SmithShopNewTabType.SSNTT_COUNT;
            StrengthenDataManager.GetInstance().IsEquipStrengthened = false;
            EquipGrowthDataManager.GetInstance().IsEquipIntensify = false;
        }
        #endregion


        private void InitQulityDrop()
        {
            if (mQulityTabDataList != null && mQulityTabDataList.Count > 0)
            {
                var qulityTabData = mQulityTabDataList[0];
                iDefaultQuality = qulityTabData.Id;
                if (mComDropDownControlQuality != null)
                {
                    mComDropDownControlQuality.InitComDropDownControl(qulityTabData, mQulityTabDataList, OnQulityDropDownItemClicked);
                }
            }
        }

        private void OnQulityDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if (iDefaultQuality == comControlData.Id)
                return;

            //赋值选中的品质
            iDefaultQuality = comControlData.Id;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshEquipmentList);
        }

        private void InitLevelDrop()
        {
            if (mLevelTabDataList != null && mLevelTabDataList.Count > 0)
            {
                var levelTabData = mLevelTabDataList[0];
                iDefaultLevelData = levelTabData;
                if (mComDropDownControlLevel != null)
                {
                    mComDropDownControlLevel.InitComDropDownControl(levelTabData, mLevelTabDataList, OnLevelDropDownItemClicked);
                }
            }
        }

        private void OnLevelDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if(iDefaultLevelData != null)
            {
                if (iDefaultLevelData.Id == comControlData.Id)
                    return;
            }
            
            //赋值选中的品质
            iDefaultLevelData = comControlData;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshEquipmentList);
        }
    }
}