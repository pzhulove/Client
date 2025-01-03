using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using ActivityLimitTime;

namespace GameClient
{

    [Serializable]
    public class FashionMallPositionSlotData
    {
        public FashionMallPositionTabType PositionTabType;
        public GameObject SlotRoot;
        public string SlotName;
    }

    public class FashionMallProfessionTabData : ComControlData
    {
        public FashionMallProfessionTabData(int index, int id, string name, bool isSelected)
            : base(index, id, name, isSelected)
        {

        }
    }
    
    //时装商城View
    public class MallNewFashionMallView : MallNewBaseView
    {

        private bool _isAlreadyInit = false;

        private FashionMallClothTabType _clothTabType = FashionMallClothTabType.None;
        private FashionMallPositionTabType _positionTabType = FashionMallPositionTabType.None;

        private int _curMallTableId = 0;
        private int _curMallType = 0;
        private int _curSubType = 0;

        private Dictionary<EFashionWearSlotType, MallItemInfo> _heroFashionTryOnDic =
            new Dictionary<EFashionWearSlotType, MallItemInfo>();
        private MallItemInfo mTryOnSuitItem;
        private List<ulong> _alreadyBuyFashionItemIds = new List<ulong>();

        //时装商城的元素信息
        private List<FashionMallElementData> _fashionMallElementDataModelList = new List<FashionMallElementData>();
        private List<ComItem> _heroFashionSlotItemList = new List<ComItem>();

        //套装 单品 武器
        private List<FashionMallClothTabData> fashionMallClothTabDataModelList;
        //单品位置
        private List<FashionMallPositionTabData> fashionMallPositionTabDataModelList;

        //职业
        private List<ComControlData> fashionMallProfessionTabDataModelList;
        private int _professionBaseJobId = 0;       //当前的基础职业

        [Space(10)]
        [HeaderAttribute("FashionClothInfo")]
        [SerializeField] private List<Toggle> mTabToggleList;
        [SerializeField] private GameObject mObjTotalCost;
        [SerializeField] private Text mTextTotalCost;
        
        //职业
        [Space(10)]
        [HeaderAttribute("FashionProfessionInfo")]
        [SerializeField] private ComDropDownControl professionDropDownControl;

        //部位
        [Space(10)]
        [HeaderAttribute("FashionPositionInfo")]

        [SerializeField] private ComUIListScript fashionMallPositionTabList;
        [SerializeField] private GameObject fashionMallPositionTabListRoot;

        //元素
        [Space(10)] [HeaderAttribute("FashionElementInfo")]
        [SerializeField] private ComUIListScript fashionMallSuitElementList;
        [SerializeField] private GameObject fashionMallSuitElementListRoot;
        [SerializeField] private ComUIListScript fashionMallSingleElementList;
        [SerializeField] private GameObject fashionMallSingleElementListRoot;
        [SerializeField] private ComUIListScript fashionMallWeaponElementList;
        [SerializeField] private GameObject fashionMallWeaponElementListRoot;
        // [SerializeField] private GameObject normalWeaponElementListPosition;
        // [SerializeField] private GameObject specialWeaponElementListPosition;
        // [SerializeField] private FashionMallSuitElementItem fashionMallSuitElementItem;

        [Space(10)]
        [HeaderAttribute("HeroActor")]
        [SerializeField] private GeAvatarRendererEx heroAvatarRenderer;
        // [SerializeField] private Text fashionNameText;
        [SerializeField] private GameObject fashionShowRoot;

        [Space(10)]
        [HeaderAttribute("FashionSlot")]
        [SerializeField] private GameObject fashionSlotRoot;
        [SerializeField] private List<FashionMallPositionSlotData> fashionSlotDataList = new List<FashionMallPositionSlotData>();

        //按钮
        [Space(10)]
        [HeaderAttribute("FashionOther")]
        [SerializeField] private Button resetButton;
        [SerializeField] private Button tryOnBuyButton;

        #region Init
        private void Awake()
        {
            _isAlreadyInit = false;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            //时装部位类型列表
            if (fashionMallPositionTabList != null)
            {
                fashionMallPositionTabList.onItemVisiable += OnPositionTabItemVisible;
            }

            //套装列表
            if (fashionMallSuitElementList != null)
            {
                fashionMallSuitElementList.Initialize();
                fashionMallSuitElementList.onItemVisiable += OnElementItemVisible;
                fashionMallSuitElementList.OnItemUpdate += OnElementItemVisible;
            }

            //部位单件列表
            if (fashionMallSingleElementList != null)
            {
                fashionMallSingleElementList.Initialize();
                fashionMallSingleElementList.onItemVisiable += OnElementItemVisible;
                fashionMallSingleElementList.OnItemUpdate += OnElementItemVisible;
            }

            //单品武器列表
            if (fashionMallWeaponElementList != null)
            {
                fashionMallWeaponElementList.Initialize();
                fashionMallWeaponElementList.onItemVisiable += OnElementItemVisible;
                fashionMallWeaponElementList.OnItemUpdate += OnElementItemVisible;
            }

            //重置按钮
            if (resetButton != null)
            {
                resetButton.onClick.RemoveAllListeners();
                resetButton.onClick.AddListener(OnResetButtonClicked);
            }

            //购买试穿按钮
            if (tryOnBuyButton != null)
            {
                tryOnBuyButton.onClick.RemoveAllListeners();
                tryOnBuyButton.onClick.AddListener(OnTryOnBuyButtonClicked);
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void ClearData()
        {
            if (_fashionMallElementDataModelList != null)
            {
                _fashionMallElementDataModelList.Clear();
            }

            if (_heroFashionSlotItemList != null)
            {
                for(var i = _heroFashionSlotItemList.Count - 1; i >= 0; i--)
                {
                    if (_heroFashionSlotItemList[i] != null)
                    {
                        ComItemManager.Destroy(_heroFashionSlotItemList[i]);
                    }
                }

                _heroFashionSlotItemList.Clear();
            }

            ResetHeroFashionTryOnDictionary();
            _alreadyBuyFashionItemIds.Clear();

            _clothTabType = FashionMallClothTabType.None;
            _positionTabType = FashionMallPositionTabType.None;

            _curMallTableId = 0;
            _curMallType = 0;
            _curSubType = 0;
            _professionBaseJobId = 0;

            if (fashionMallClothTabDataModelList != null)
            {
                fashionMallClothTabDataModelList.Clear();
                fashionMallClothTabDataModelList = null;
            }

            if (fashionMallPositionTabDataModelList != null)
            {
                fashionMallPositionTabDataModelList.Clear();
                fashionMallPositionTabDataModelList = null;
            }

            if (fashionMallProfessionTabDataModelList != null)
            {
                fashionMallProfessionTabDataModelList.Clear();
                fashionMallProfessionTabDataModelList = null;
            }
        }

        private void UnBindUiEventSystem()
        {

            //时装位置
            if (fashionMallPositionTabList != null)
            {
                fashionMallPositionTabList.onItemVisiable -= OnPositionTabItemVisible;
            }

            //套装元素
            if (fashionMallSuitElementList != null)
            {
                fashionMallSuitElementList.onItemVisiable -= OnElementItemVisible;
            }

            //单品元素
            if (fashionMallSingleElementList != null)
            {
                fashionMallSingleElementList.onItemVisiable -= OnElementItemVisible;
            }

            //单品武器
            if (fashionMallWeaponElementList != null)
            {
                fashionMallWeaponElementList.onItemVisiable -= OnElementItemVisible;
            }
            
            //重置按钮
            if (resetButton != null)
            {
                resetButton.onClick.RemoveAllListeners();
            }

            //购买试穿按钮
            if (tryOnBuyButton != null)
            {
                tryOnBuyButton.onClick.RemoveAllListeners();
            }
        }

        private int mDefaultSuitIndex = -1;
        public override void InitData(int index, int secondIndex = 0, int thirdIndex = 0)
        {
            if (_isAlreadyInit == true)
                return;
            _isAlreadyInit = true;

            //初始化数据
            //套装相关数据
            fashionMallClothTabDataModelList = FashionMallUtility.GetClothTabDataModelList();
            //角色部位数据
            fashionMallPositionTabDataModelList = FashionMallUtility.GetPositionTabDataModelList();

            //职业数据
            fashionMallProfessionTabDataModelList = FashionMallUtility.GetProfessionTabDataModelList();
            //当前的基础职业
            _professionBaseJobId = GetDefaultBaseJobId(index);

            //套装or单品
            if (secondIndex <= 0 || secondIndex >= (int)FashionMallClothTabType.Number)
            {
                _clothTabType = FashionMallClothTabType.Suit;
                mDefaultSuitIndex = thirdIndex - 1;
            }
            else
            {
                _clothTabType = (FashionMallClothTabType) secondIndex;
            }

            //单品
            if (_clothTabType == FashionMallClothTabType.Single)
            {
                if (thirdIndex < 0 || thirdIndex > 4)
                {
                 _positionTabType = FashionMallPositionTabType.Head;
                }
                else
                {
                    _positionTabType = (FashionMallPositionTabType) thirdIndex;
                }
            }
            else
            {
                //套装,或者武器，位置Tab无意义
                _positionTabType = FashionMallPositionTabType.Head;
            }

            InitProfessionDropDownControl();

            InitClothTabList();

        }

        #endregion

        #region ClothTab

        //时装商城中的时装类型Tab
        private void InitClothTabList()
        {
            //选中对应
            if (mTabToggleList[(int)_clothTabType].isOn)
            {
                OnClothTabClicked((int)_clothTabType);
            }   
            else
            {
                mTabToggleList[(int)_clothTabType].isOn = true;
            } 
        }

        public void OnClickToggle0(bool value)
        {
            if (value)
                OnClothTabClicked(0);
        }
        public void OnClickToggle1(bool value)
        {
            if (value)
                OnClothTabClicked(1);
        }
        public void OnClickToggle2(bool value)
        {
            if (value)
                OnClothTabClicked(2);
        }
        private void OnClothTabClicked(int index)
        {
            if (index >= fashionMallClothTabDataModelList.Count)
                return;
            var type = _clothTabType;
            _clothTabType = fashionMallClothTabDataModelList[index].ClothTabType;
            _curMallTableId = fashionMallClothTabDataModelList[index].MallTableId;
            UpdateFashionMallContentByClothTab();
            //更新英雄信息
            // 注释by ckm
            //if (type == FashionMallClothTabType.Suit || _clothTabType == FashionMallClothTabType.Suit)
                UpdateHeroRelationInfo();
        }

        //选择套装Tab
        private void UpdateFashionMallContentByClothTab()
        {
            if (_clothTabType == FashionMallClothTabType.Single)
            {
                InitPositionTabList();
                InitHeroFashionSlots();
            }
            else if (_clothTabType == FashionMallClothTabType.Weapon)
            {
                InitHeroFashionSlots();
            }

            UpdateFashionMallShowTypeByClothTab(_clothTabType);

            //新元素
            UpdateFashionMallElementList();
        }

        //根据时装商城时装的种类（套装，单品）,展示不同的内容
        private void UpdateFashionMallShowTypeByClothTab(FashionMallClothTabType clothTabType)
        {
            switch (clothTabType)
            {
                //单品
                case FashionMallClothTabType.Single:
                    //单品
                    fashionMallSuitElementListRoot.CustomActive(false);
                    fashionMallWeaponElementListRoot.CustomActive(false);
                    mObjTotalCost.CustomActive(true);
                    fashionMallPositionTabListRoot.CustomActive(true);
                    fashionMallSingleElementListRoot.CustomActive(true);
                    // resetButton.CustomActive(true);
                    // tryOnBuyButton.CustomActive(true);
                    fashionSlotRoot.CustomActive(true);
                    break;
                case FashionMallClothTabType.Weapon:
                    //武器
                    fashionMallSuitElementListRoot.CustomActive(false);
                    fashionMallSingleElementListRoot.CustomActive(false);
                    fashionMallPositionTabListRoot.CustomActive(false);
                    mObjTotalCost.CustomActive(true);
                    fashionMallWeaponElementListRoot.CustomActive(true);
                    // resetButton.CustomActive(true);
                    // tryOnBuyButton.CustomActive(true);
                    fashionSlotRoot.CustomActive(true);

                    break;
                default:
                    //套装
                    fashionMallPositionTabListRoot.CustomActive(false);
                    fashionMallSingleElementListRoot.CustomActive(false);
                    fashionMallWeaponElementListRoot.CustomActive(false);
                    // resetButton.CustomActive(false);
                    // tryOnBuyButton.CustomActive(false);
                    fashionSlotRoot.CustomActive(false);
                    mObjTotalCost.CustomActive(false);
                    fashionMallSuitElementListRoot.CustomActive(true);
                    break;
            }
        }
        #endregion

        #region ProfessionTab
        
        //得到默认的基础职业
        private int GetDefaultBaseJobId(int index)
        {
            //0 代表当前角色的基础职业
            if (index <= 0)
                return FashionMallUtility.GetSelfBaseJobId();

            if (fashionMallProfessionTabDataModelList == null || fashionMallProfessionTabDataModelList.Count <= 0)
                return FashionMallUtility.GetSelfBaseJobId();

            //Index所指示的基础职业
            for (var i = 0; i < fashionMallProfessionTabDataModelList.Count; i++)
            {
                if (fashionMallProfessionTabDataModelList[i].Index == index)
                    return fashionMallProfessionTabDataModelList[i].Id;
            }

            return FashionMallUtility.GetSelfBaseJobId();
        }

        //职业下拉单数据
        private void InitProfessionDropDownControl()
        {

            if (fashionMallProfessionTabDataModelList != null
                && fashionMallProfessionTabDataModelList.Count > 0)
            {
                var defaultProfessionDropDownData = fashionMallProfessionTabDataModelList[0];
                for (var i = 0; i < fashionMallProfessionTabDataModelList.Count; i++)
                {
                    if (_professionBaseJobId == fashionMallProfessionTabDataModelList[i].Id)
                    {
                        defaultProfessionDropDownData = fashionMallProfessionTabDataModelList[i];
                        break;
                    }
                }

                if (professionDropDownControl != null)
                    professionDropDownControl.InitComDropDownControl(defaultProfessionDropDownData,
                        fashionMallProfessionTabDataModelList,
                        OnProfessionDropDownItemClicked);
            }
        }

        private void UpdateFashionMallContentByProfessionBaseId()
        {
            //更新时装元素
            UpdateFashionMallElementList();

            //更新英雄的相关信息
            UpdateHeroRelationInfo();
        }

        private void OnProfessionDropDownItemClicked(ComControlData comControlData)
        {
            if(comControlData == null)
                return;

            //基础职业相同，直接返回
            if (_professionBaseJobId == comControlData.Id)
                return;

            //赋值选中的基础职业
            _professionBaseJobId = comControlData.Id;

            //根据选中的职业进行更新
            UpdateFashionMallContentByProfessionBaseId();
        }

        #endregion

        #region PositionTab
        //时装商城中的部位Tab
        private void InitPositionTabList()
        {
            //没有初始化的时候，首先进行初始化操作
            if (fashionMallPositionTabList.IsInitialised() == false)
            {
                fashionMallPositionTabList.Initialize();

                if (fashionMallPositionTabDataModelList != null
                    && fashionMallPositionTabDataModelList.Count > 0
                    && fashionMallPositionTabList != null)
                {
                    fashionMallPositionTabList.SetElementAmount(fashionMallPositionTabDataModelList.Count);
                }
            }
        }

        private void OnPositionTabItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (fashionMallPositionTabDataModelList == null
                || fashionMallPositionTabDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= fashionMallPositionTabDataModelList.Count)
                return;

            var fashionMallPositionTabDataModel = fashionMallPositionTabDataModelList[item.m_index];
            var fashionMallPositionTabItem = item.GetComponent<FashionMallPositionTabItem>();
            if (fashionMallPositionTabDataModel != null
                && fashionMallPositionTabItem != null)
            {
                var isSelected = fashionMallPositionTabDataModel.PositionTabType == _positionTabType;
                fashionMallPositionTabItem.InitData(fashionMallPositionTabDataModel,
                    OnPositionTabClicked, 
                    isSelected);
            }
        }

        //选择位置Tab
        private void OnPositionTabClicked(int index, FashionMallPositionTabType positionTabType)
        {

            //避免初始化的时候，进行时装元素的更新
            if (_positionTabType == positionTabType)
                return;
            _positionTabType = positionTabType;

            UpdateFashionMallContentByPositionTab();
        }

        //选择位置Tab
        private void UpdateFashionMallContentByPositionTab()
        {
            //只更新时装元素
            UpdateFashionMallElementList();
        }

        #endregion

        #region FashionMallContent
        
        //更新时装商城的ElementItem
        //只更新右边ScrollViewList中的元素
        private void UpdateFashionMallElementList()
        {
            _curMallType = FashionMallUtility.GetMallType(_curMallTableId);
            _curSubType = FashionMallUtility.GetMallTableSubTypeIndex(_clothTabType, _positionTabType);

            //首先查找MallItem元素
            List<MallItemInfo> mallItems = MallNewDataManager.GetInstance().GetMallItemInfoList(_curMallType,
                _curSubType,
                _professionBaseJobId);

            //如果不存在，则发送请求
            if (mallItems == null)
            {
                MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(_curMallTableId, _curSubType, _professionBaseJobId);
            }
            else
            {
                //如果存在，则直接更新
                ShowFashionMallElementList(mallItems);
            }
        }

        //时装元素列表刷新
        private void OnElementItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            if(_fashionMallElementDataModelList == null
               || _fashionMallElementDataModelList.Count <= 0)
                return;

            if(item.m_index < 0 || item.m_index >= _fashionMallElementDataModelList.Count)
                return;

            var elementItem = item.GetComponent<FashionMallElementItem>();
            var elementDataModel = _fashionMallElementDataModelList[item.m_index];
            if (elementItem != null && elementDataModel != null)
            {
                bool isTryOn = false;

                //单件试穿
                foreach (MallItemInfo info in _heroFashionTryOnDic.Values)
                {
                    if (info.id == elementDataModel.MallItemInfo.id)
                    {
                        isTryOn = true;
                        break;
                    }
                }
                //套装试穿
                if (null != mTryOnSuitItem && mTryOnSuitItem.id == elementDataModel.MallItemInfo.id)
                    isTryOn = true;
                elementItem.InitData(elementDataModel, OnElementItemTryOn, isTryOn);
            }
        }
        #endregion

        #region HeroRelationInfo
        //更新英雄相关信息：Actor， FashionSlot，试穿缓存
        private void UpdateHeroRelationInfo()
        {
            //重置数据
            ResetHeroFashionTryOnDictionary();
            ResetHeroFashionSlots();
            var selfBaseId = FashionMallUtility.GetSelfBaseJobId();
            var selectedHeroBaseJobId = _professionBaseJobId;
            if (selectedHeroBaseJobId == selfBaseId)
            {
                //基础职业相同的情况，选择当前的职业，可能为小职业
                selectedHeroBaseJobId = PlayerBaseData.GetInstance().JobTableID;

                if (_clothTabType == FashionMallClothTabType.Single
                    || _clothTabType == FashionMallClothTabType.Weapon)
                {
                    UpdateHeroFashionSlots();
                }
            }
            CreateHeroActor(selectedHeroBaseJobId);
        }

        //创建英雄
        private void CreateHeroActor(int heroJobId)
        {
            if (heroAvatarRenderer == null)
            {
                Logger.LogErrorFormat("HeroAvatarRenderer is null");
                return;
            }

            var jobItem = TableManager.GetInstance().GetTableItem<JobTable>(heroJobId);
            if (jobItem == null)
            {
                Logger.LogErrorFormat("Cannot find JobItem and JobId is {0}", heroJobId);
                return;
            }

            var resItem = TableManager.GetInstance().GetTableItem<ResTable>(jobItem.Mode);
            if (resItem == null)
            {
                Logger.LogErrorFormat("Cannot find ResItem with id : {0} ", jobItem.Mode);
                return;
            }

            heroAvatarRenderer.ClearAvatar();
            heroAvatarRenderer.LoadAvatar(resItem.ModelPath);

            if (heroJobId == PlayerBaseData.GetInstance().JobTableID)
            {
                PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(heroAvatarRenderer);
            }

            heroAvatarRenderer.AttachAvatar("Aureole", 
                "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS",
                "[actor]Orign",
                false);

            heroAvatarRenderer.SuitAvatar();
            heroAvatarRenderer.ChangeAction("Anim_Show_Idle",
                1f, 
                true);
        }
        #endregion

        #region FashionSlot
        //Init
        private void InitHeroFashionSlots()
        {
            if(_heroFashionSlotItemList.Count > 0)
                return;
                
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
            for (var i = 0; i < fashionSlotDataList.Count; i++)
            {
                var slotRoot = fashionSlotDataList[i].SlotRoot;
                var comItem = ComItemManager.Create(slotRoot);
                comItem.SetupSlot(ComItem.ESlotType.Opened, fashionSlotDataList[i].SlotName);
                comItem.labSlot.fontSize = 35;      //调整字体大小

                _heroFashionSlotItemList.Add(comItem);
                if (i < itemGuids.Count)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                    comItem.Setup(itemData, null);
                    comItem.SetFashionMaskShow(false);
                }
            }
        }

        //Reset 重置
        private void ResetHeroFashionSlots()
        {
            for (var i = 0; i < _heroFashionSlotItemList.Count; i++)
            {
                _heroFashionSlotItemList[i].Setup(null, null);
                mObjTalkOff[i].CustomActive(false);
            }
        }

        //重置试穿时装的缓存
        private void ResetHeroFashionTryOnDictionary()
        {
            if(_heroFashionTryOnDic != null)
                _heroFashionTryOnDic.Clear();
            mTryOnSuitItem  = null;
            mTextTotalCost.SafeSetText("0");
        }

        //刷新
        private void UpdateHeroFashionSlots()
        {
            ResetHeroFashionSlots();
            var guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);

            for (var i = 0; i < guids.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                if(itemData == null)
                    continue;

                for (var j = 0; j < fashionSlotDataList.Count; j++)
                {
                    if(FashionMallUtility.IsSameSlotType(itemData.FashionWearSlotType, fashionSlotDataList[j].PositionTabType) == true)
                    {
                        if (j < _heroFashionSlotItemList.Count)
                        {
                            _heroFashionSlotItemList[j].Setup(itemData, null);
                            _heroFashionSlotItemList[j].SetFashionMaskShow(false);
                            break;
                        }
                    }
                }
            }
        }

        //试穿
        private void UpdateHeroFashionSlotByItemId(int itemId, EFashionWearSlotType fashionWearSlotType)
        {
            for (var i = 0; i < fashionSlotDataList.Count; i++)
            {
                if(FashionMallUtility.IsSameSlotType(fashionWearSlotType, fashionSlotDataList[i].PositionTabType) == false)
                    continue;

                var itemData = ItemDataManager.CreateItemDataFromTable(itemId);
                if (itemData == null)
                    return;

                if (i < _heroFashionSlotItemList.Count)
                {
                    _heroFashionSlotItemList[i].Setup(itemData, ShowItemTipFrame);
                    _heroFashionSlotItemList[i].SetFashionMaskShow(false);
                }
            }
        }

        private void ShowItemTipFrame(GameObject go, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTipWithoutModelAvatar(itemData);
        }

        #endregion

        #region BuyAndResetFashion
        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, OnSyncWorldMallQueryItem);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncMallBatchBuySucceed, OnSyncMallBatchBuyRes);
        }

        public override void OnEnableView()
        {
            //初始化之后，再次显示时装商城，在SetEnable(true)之后，显示
            if (_isAlreadyInit == true)
            {
                UpdateFashionMallContentByOnEnable();
            }
        }


        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, OnSyncWorldMallQueryItem);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncMallBatchBuySucceed, OnSyncMallBatchBuyRes);
            FashionMallUtility.CloseFashionLimitTimeBuyFrame();
        }

        private void OnSyncWorldMallQueryItem(UIEvent uiEvent)
        {
            if(uiEvent == null)
                return;

            var mallType = (int) uiEvent.Param1;
            var subType = (int) uiEvent.Param2;
            var jobId = (int) uiEvent.Param3;
            
            if(mallType != _curMallType
               || subType != _curSubType
               || jobId != _professionBaseJobId)
                return;

            var mallItems = MallNewDataManager.GetInstance().GetMallItemInfoList(_curMallType,
                _curSubType,
                _professionBaseJobId);

            ShowFashionMallElementList(mallItems);
        }

        //显示时装元素

        private void ShowFashionMallElementList(List<MallItemInfo> mallItems)
        {
            if(mallItems == null || mallItems.Count <= 0)
                return;

            for (var i = 0; i < _fashionMallElementDataModelList.Count; i++)
            {
                _fashionMallElementDataModelList[i].MallItemInfo = null;
            }
            _fashionMallElementDataModelList.Clear();

            var recommendFashionElementData = new FashionMallElementData()
            {
                ClothTabType = _clothTabType,
                MallItemInfo = mallItems[0],
            };

            for (int i = 1; i < mallItems.Count; i++)
            {
                var curFashionElementData = new FashionMallElementData()
                {
                    ClothTabType = _clothTabType,
                    MallItemInfo = mallItems[i],
                };
                _fashionMallElementDataModelList.Add(curFashionElementData);
            }

            StopFashionMallElementListMovement();
            ResetFashionMallElementList();

            if (_clothTabType == FashionMallClothTabType.Suit)
            {
                //套装
                // if (fashionMallSuitElementItem != null)
                //     fashionMallSuitElementItem.InitData(recommendFashionElementData,
                //         OnElementItemBuy,
                //         OnElementItemTryOn);
                fashionMallSuitElementList.SetElementAmount(_fashionMallElementDataModelList.Count);
                fashionMallSuitElementList.ResetContentPosition();
                if (-1 != mDefaultSuitIndex)
                {
                    //只有套装才选中
                    if (_clothTabType == FashionMallClothTabType.Suit)
                    {
                        OnElementItemTryOn(_fashionMallElementDataModelList[mDefaultSuitIndex].MallItemInfo);
                    }
                    mDefaultSuitIndex = -1;
                }
            }
            else if(_clothTabType == FashionMallClothTabType.Single)
            {
                //单品
                _fashionMallElementDataModelList.Insert(0,recommendFashionElementData);
                fashionMallSingleElementList.SetElementAmount(_fashionMallElementDataModelList.Count);
                fashionMallSingleElementList.ResetContentPosition();
            }
            else if (_clothTabType == FashionMallClothTabType.Weapon)
            {

                //武器
                _fashionMallElementDataModelList.Insert(0, recommendFashionElementData);
                // UpdateFashionWeaponElementListRootPosition(_fashionMallElementDataModelList.Count);
                fashionMallWeaponElementList.SetElementAmount(_fashionMallElementDataModelList.Count);
                fashionMallWeaponElementList.ResetContentPosition();
            }
        }

        private void ResetFashionMallElementList()
        {
            if (fashionMallSingleElementList != null)
                fashionMallSingleElementList.SetElementAmount(0);
            if (fashionMallWeaponElementList != null)
                fashionMallWeaponElementList.SetElementAmount(0);
            if (fashionMallSuitElementList != null)
                fashionMallSuitElementList.SetElementAmount(0);
        }

        private void StopFashionMallElementListMovement()
        {
            if (fashionMallSingleElementList != null
                &&fashionMallSingleElementList.m_scrollRect != null)
            {
                fashionMallSingleElementList.m_scrollRect.StopMovement();
            }

            if (fashionMallSuitElementList != null && fashionMallSuitElementList.m_scrollRect != null)
            {
                fashionMallSuitElementList.m_scrollRect.StopMovement();
            }

            if (fashionMallWeaponElementList != null && fashionMallWeaponElementList.m_scrollRect != null)
            {
                fashionMallWeaponElementList.m_scrollRect.StopMovement();
            }
        }

        private void UpdateFashionMallContentByOnEnable()
        {
            //更新人物
            UpdateHeroRelationInfo();

            //更新元素
            var mallItems = MallNewDataManager.GetInstance().GetMallItemInfoList(_curMallType,
                _curSubType,
                _professionBaseJobId);

            ShowFashionMallElementList(mallItems);

        }
        
        // private void OnElementItemBuy(MallItemInfo mallItemInfo)
        // {
        //     if (mallItemInfo == null)
        //         return;

        //     FashionMallUtility.CloseFashionMallBuyFrame();
        //     FashionMallUtility.CloseFashionLimitTimeBuyFrame();

        //     if (mallItemInfo.goodsSubType != 0)
        //     {
        //         FashionOutComeData outData = new FashionOutComeData();
        //         outData.mallItemInfos = new List<MallItemInfo>() {mallItemInfo};
        //         outData.fashionTypeIndex = (_clothTabType == FashionMallClothTabType.Suit)
        //             ? FashionMallMainIndex.FashionAll
        //             : FashionMallMainIndex.FashionOne;

        //         ClientSystemManager.GetInstance().OpenFrame<FashionLimitTimeBuyFrame>(FrameLayer.Middle, outData);
        //     }
        //     else
        //     {
        //         ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, mallItemInfo);
        //     }
        // }

        private void OnElementItemTryOn(MallItemInfo mallItemInfo)
        {
            if (mallItemInfo == null)
                return;
            //设置试穿的衣服
            if (heroAvatarRenderer == null)
                return;

            #region UpdateFashion
            var itemIds = FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance()
                .TryGetItemIdsInMallItemInfo(mallItemInfo);
            if(itemIds == null || itemIds.Length <= 0 || (itemIds.Length == 1 && itemIds[0] == 0))
                return;
            for (var i = 0; i < itemIds.Length; i++)
            {
                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int) itemIds[i]);
                if(itemTable == null)
                    continue;
                UpdateHeroActorFashion(itemTable);
            }
            //保存试穿数据，只有单品和武器的时候才保存，并更新左侧的Slot
            //套装储存
            if (_clothTabType  == FashionMallClothTabType.Suit)
                mTryOnSuitItem  = mallItemInfo;
            //单件储存
            if (_clothTabType == FashionMallClothTabType.Single
                || _clothTabType == FashionMallClothTabType.Weapon)
            {
                //保存试穿的数据
                EFashionWearSlotType slotType = FashionMallUtility.GetEquipSlotType(TableManager.GetInstance().GetTableItem<ItemTable>((int) itemIds[0]));
                if (_heroFashionTryOnDic.ContainsKey(slotType) == false)
                    _heroFashionTryOnDic.Add(slotType, null);
                _heroFashionTryOnDic[slotType] = mallItemInfo;
                mObjTalkOff[mTypeList.IndexOf(slotType)].CustomActive(true);
                //设置右侧的Slot
                UpdateHeroFashionSlotByItemId((int)mallItemInfo.itemid, slotType);
            }
            int cost = 0;
            if  (null != mTryOnSuitItem)
                    cost = Utility.GetMallRealPrice(mTryOnSuitItem);
            else
            {
                foreach (MallItemInfo info in _heroFashionTryOnDic.Values)
                {
                    if (null != info)
                        cost += Utility.GetMallRealPrice(info);
                }
            }
            mTextTotalCost.SafeSetText(cost.ToString());
            #endregion
            //时装单件
            if (_clothTabType == FashionMallClothTabType.Single)
            {
                fashionMallSingleElementList.UpdateElement();
            }
            //武器单件
            else if (_clothTabType == FashionMallClothTabType.Weapon)
            {
                fashionMallWeaponElementList.UpdateElement();
            }
            //套装
            else if (_clothTabType == FashionMallClothTabType.Suit)
            {
                fashionMallSuitElementList.UpdateElement();
            }
        }

        //脱下试穿
        [SerializeField] private List<EFashionWearSlotType>  mTypeList;
        [SerializeField] private List<GameObject> mObjTalkOff;
        public void OnClickTalkOffTryOnItem(int index)
        {
            if (index > mTypeList.Count || !_heroFashionTryOnDic.ContainsKey(mTypeList[index]) || null == _heroFashionTryOnDic[mTypeList[index]])
                return;
            _heroFashionTryOnDic.Remove(mTypeList[index]);
            ResetHeroFashionSlots();
            var selfBaseId = FashionMallUtility.GetSelfBaseJobId();
            var selectedHeroBaseJobId = _professionBaseJobId;
            if (selectedHeroBaseJobId == selfBaseId)
            {
                //基础职业相同的情况，选择当前的职业，可能为小职业
                selectedHeroBaseJobId = PlayerBaseData.GetInstance().JobTableID;

                if (_clothTabType == FashionMallClothTabType.Single
                    || _clothTabType == FashionMallClothTabType.Weapon)
                {
                    UpdateHeroFashionSlots();
                }
            }
            CreateHeroActor(selectedHeroBaseJobId);
            //穿上试穿道具
            foreach (MallItemInfo mallItemInfo in _heroFashionTryOnDic.Values)
            {
                var itemIds = FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance().TryGetItemIdsInMallItemInfo(mallItemInfo);
                if (itemIds == null || itemIds.Length <= 0 || (itemIds.Length == 1 && itemIds[0] == 0))
                    return;
                for (var i = 0; i < itemIds.Length; i++)
                {
                    var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)itemIds[i]);
                    if (itemTable == null)
                        continue;
                    UpdateHeroActorFashion(itemTable);
                }
                //单件储存
                if (_clothTabType == FashionMallClothTabType.Single
                    || _clothTabType == FashionMallClothTabType.Weapon)
                {
                    EFashionWearSlotType slotType = FashionMallUtility.GetEquipSlotType(TableManager.GetInstance().GetTableItem<ItemTable>((int)itemIds[0]));
                    mObjTalkOff[mTypeList.IndexOf(slotType)].CustomActive(true);
                    UpdateHeroFashionSlotByItemId((int)mallItemInfo.itemid, slotType);
                }
            }
            int cost = 0;
            if (null != mTryOnSuitItem)
                cost = Utility.GetMallRealPrice(mTryOnSuitItem);
            else
            {
                foreach (MallItemInfo info in _heroFashionTryOnDic.Values)
                {
                    if (null != info)
                        cost += Utility.GetMallRealPrice(info);
                }
            }
            mTextTotalCost.SafeSetText(cost.ToString());
            //时装单件
            if (_clothTabType == FashionMallClothTabType.Single)
            {
                fashionMallSingleElementList.UpdateElement();
            }
            //武器单件
            else if (_clothTabType == FashionMallClothTabType.Weapon)
            {
                fashionMallWeaponElementList.UpdateElement();
            }
            //套装
            else if (_clothTabType == FashionMallClothTabType.Suit)
            {
                fashionMallSuitElementList.UpdateElement();
            }
        }

        private void UpdateHeroActorFashion(ItemTable itemTable)
        {
            EFashionWearSlotType slotType = FashionMallUtility.GetEquipSlotType(itemTable);
            //显示武器时装
            if (itemTable.SubType == ItemTable.eSubType.FASHION_WEAPON)
            {
                var heroBaseJobId = _professionBaseJobId;
                //如果当前选中的职业和角色的职业相同，则使用角色的职业
                if (_professionBaseJobId == FashionMallUtility.GetSelfBaseJobId())
                {
                    heroBaseJobId = PlayerBaseData.GetInstance().JobTableID;
                }
                PlayerBaseData.GetInstance().AvatarEquipWeapon(heroAvatarRenderer, heroBaseJobId, itemTable.ID);
            }
            else
            {
                //首先清空对应部位的时装资源。避免有的道具存在资源有的道具不存在资源，造成某种资源没有卸载掉
                PlayerBaseData.GetInstance().AvatarEquipPart(heroAvatarRenderer, slotType, 0);
				//单品部位的时装
                PlayerBaseData.GetInstance().AvatarEquipPart(heroAvatarRenderer, slotType, itemTable.ID);
            }
            heroAvatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
        }

        //重置和购买试穿按钮
        private void OnTryOnBuyButtonClicked()
        {

            if (_heroFashionTryOnDic.Count <= 0 && null == mTryOnSuitItem)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Fashion_mall_Try_On_Nothing"));
                return;
            }

            //只有在单品界面，才会出现购买试穿的可能
            // if (_clothTabType != FashionMallClothTabType.Single
            //     && _clothTabType != FashionMallClothTabType.Weapon)
            // {
            //     SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Fashion_mall_Try_On_Nothing"));
            //     return;
            // }

            FashionMallUtility.CloseFashionLimitTimeBuyFrame();
            
            //购买试穿
            BuyTryOnFashions();

        }

        //购买试穿
        private void BuyTryOnFashions()
        {
            var tryOnMallItemInfos = new List<MallItemInfo>();
            //购买套装
            if (null != mTryOnSuitItem)
            {
                tryOnMallItemInfos.Add(mTryOnSuitItem);
            }
            //购买单件
            else
            {
                var iter = _heroFashionTryOnDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    MallItemInfo mallItemInfo = iter.Current.Value as MallItemInfo;
                    tryOnMallItemInfos.Add(mallItemInfo);
                }
            }
            FashionOutComeData outData = new FashionOutComeData()
            {
                mallItemInfos = tryOnMallItemInfos,
                fashionTypeIndex = null == mTryOnSuitItem ? FashionMallMainIndex.FashionOne : FashionMallMainIndex.FashionAll,
            };
            ClientSystemManager.GetInstance().OpenFrame<FashionLimitTimeBuyFrame>(FrameLayer.Middle, outData);
        }

        private void OnResetButtonClicked()
        {
            UpdateHeroRelationInfo();
        }

        private void OnSyncMallBatchBuyRes(UIEvent uiEvent)
        {
            if(uiEvent == null)
                return;

            var res = uiEvent.Param1 as SCMallBatchBuyRes;
            if(res == null)
                return;

            //关掉界面
            FashionMallUtility.CloseFashionLimitTimeBuyFrame();

            if (ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.HaveFashionDiscountActivity 
                && _clothTabType == FashionMallClothTabType.Suit)
            {
                MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(_curMallTableId, _curSubType, _professionBaseJobId);
            }

            //非自身
            if (FashionMallUtility.GetSelfBaseJobId() != _professionBaseJobId)
            {
                return;
            }

            _alreadyBuyFashionItemIds.Clear();
            for (var i = 0; i < res.itemUids.Length; i++)
            {
                _alreadyBuyFashionItemIds.Add(res.itemUids[i]);
            }

            if (PlayerBaseData.GetInstance().isNotify == true)
            {
                SystemNotifyManager.SetMallBuyMsgBoxOKCancel(TR.Value("Fashion_mall_Wear_Fashion_RightNow"),
                    ClickOkWearFashion);
            }
            else
            {
                if (PlayerBaseData.GetInstance().isWear)
                {
                    ClickOkWearFashion();
                }
            }
            //时装单件
            if (_clothTabType == FashionMallClothTabType.Single)
            {
                fashionMallSingleElementList.UpdateElement();
            }
            else if (_clothTabType == FashionMallClothTabType.Weapon)
            {
                fashionMallWeaponElementList.UpdateElement();
            }
            else if (_clothTabType == FashionMallClothTabType.Suit)
            {
                fashionMallSuitElementList.UpdateElement();
            }
        }

        //穿上否买的道具
        private void ClickOkWearFashion()
        {
            for (var i = 0; i < _alreadyBuyFashionItemIds.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(_alreadyBuyFashionItemIds[i]);
                if (itemData == null)
                {
                    continue;
                }

                ItemDataManager.GetInstance().UseItem(itemData);

                ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemData.TableID);
                if (itemTable == null)
                {
                    continue;
                }

                //更新到人物身上
                UpdateHeroActorFashion(itemTable);

                //更新道具的Slot
                if (_clothTabType == FashionMallClothTabType.Single
                    || _clothTabType == FashionMallClothTabType.Weapon)
                {
                    EFashionWearSlotType slotType = FashionMallUtility.GetEquipSlotType(itemTable);
                    UpdateHeroFashionSlotByItemId(itemTable.ID, slotType);
                }
            }
            PlayerBaseData.GetInstance().isWear = true;
        }

        #endregion

        #region Helper

        #endregion

    }
}