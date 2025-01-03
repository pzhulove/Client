using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class ShopNewView : MonoBehaviour
    {
        //整体的ShopID
        private int _shopId = 17;

        private int _npcId = -1;

        private int _selectedMainTabIndex = 0;

        private ShopTable _shopNewTable = null;
        private ShopNewShopData _shopNewShopData = null;

        //商店中商品的基础信息
        private List<ShopNewShopItemInfo> _shopNewShopItemTableList = null;

        private List<ShopNewMainTabData> _shopMainTabDataList = null;

        private ShopNewMainTabData _shopNewMainTabDefaultSelectedData = new ShopNewMainTabData();
        private int _defaultMainTabIndex = 0;           //默认选择的主页签的ID

        //选中的按钮或者过滤器，用于选择展示的元素信息
        private ShopNewMainTabData _shopNewMainTabSelectedData = new ShopNewMainTabData();
        private ShopNewFilterData _shopNewFirstFilterSelectedData = new ShopNewFilterData();
        private ShopNewFilterData _shopNewSecondFilterSelectedData = new ShopNewFilterData();

        [Space(5)]
        [HeaderAttribute("Title")]
        [SerializeField] private Text shopNameText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button helpButton;

        [Space(5)]
        [HeaderAttribute("Money")]
        [SerializeField] private ShopNewMoneyView shopNewMoneyView;

        [Space(10)] [HeaderAttribute("ShopMainTabList")]
        [SerializeField] private CommonTabToggleGroup mToggleGroup;
        [Space(10)] [HeaderAttribute("ShopNewFilterView")] [SerializeField]
        private ShopNewFilterView shopNewFilterView;

        [Space(10)] [HeaderAttribute("ShopElementItemList")] [SerializeField]
        private ComUIListScript shopNewElementItemList;

        [SerializeField] private Text shopDescriptionText = null;

        [Space(10)]
        [HeaderAttribute("ShopTimeRefresh")]
        [SerializeField] private TimeRefresh shopTimeRefreshControl;
        [SerializeField] private Text shopTimeRefreshText;
        [SerializeField] private Text shopTimeRefreshLabel;

        [Space(10)] [HeaderAttribute("HonorLevelRoot")] [Space(10)]
        [SerializeField] private GameObject honorLevelRoot;
        [SerializeField] private Text honorLevelValueLabel;

        [Space(10)] [HeaderAttribute("NoElementTips")] [Space(10)]
        [SerializeField] private Text noElementTipLabel;

        private void Awake()
        {
            BindUiEventSystem();
        }
        
        private void BindUiEventSystem()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            if (helpButton != null)
            {
                helpButton.onClick.RemoveAllListeners();
                helpButton.onClick.AddListener(OnHelpButtonClick);
            }

            if (shopNewElementItemList != null)
            {
                shopNewElementItemList.onItemVisiable += OnElementItemVisible;
                shopNewElementItemList.OnItemRecycle += OnElementItemRecycle;
                shopNewElementItemList.onItemSelected += OnElementItemSelect;
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopNewRequestChildrenShopDataSucceed,
                OnRequestChildrenShopDataSucceed);
        }

        private void OnDestroy()
        {
            PlayShopNpcSound(NpcVoiceComponent.SoundEffectType.SET_End);

            UnBindUiEventSystem();
            ClearData();
        }

        private void ClearData()
        {
            _shopNewTable = null;
            _shopNewShopData = null;
            _shopNewShopItemTableList = null;
            
            _defaultMainTabIndex = 0;
            _shopNewMainTabDefaultSelectedData = null;
            _shopNewMainTabSelectedData = null;
            _shopNewFirstFilterSelectedData = null;
            _shopNewSecondFilterSelectedData = null;
            _selectedMainTabIndex = 0;
        }

        private void UnBindUiEventSystem()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if(helpButton != null)
                helpButton.onClick.RemoveAllListeners();

            if (shopNewElementItemList != null)
            {
                shopNewElementItemList.onItemVisiable -= OnElementItemVisible;
                shopNewElementItemList.OnItemRecycle -= OnElementItemRecycle;
                shopNewElementItemList.onItemSelected -= OnElementItemSelect;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopNewRequestChildrenShopDataSucceed,
                OnRequestChildrenShopDataSucceed);
        }

        private int mDefaultItemId = 0;
        public void InitShop(ShopNewParamData shopNewParamData)
        {

            if (shopNewParamData == null)
            {
                Logger.LogErrorFormat("InitShop ShowNewParamData is null");
                return;
            }

            _shopId = shopNewParamData.ShopId;
            _shopNewMainTabDefaultSelectedData.MainTabType = ShopNewMainTabType.ShopType;
            _shopNewMainTabDefaultSelectedData.Index = _shopId;
            mDefaultItemId = shopNewParamData.shopItemId;

            _npcId = shopNewParamData.NpcId;

            //商店的子商店或者Item类型设置，用于初始化页签的时候进行选择
            if (shopNewParamData.ShopChildId > 0)
            {
                _shopNewMainTabDefaultSelectedData.MainTabType = ShopNewMainTabType.ShopType;
                _shopNewMainTabDefaultSelectedData.Index = shopNewParamData.ShopChildId;
            }
            else
            {
                //商店的类型设置
                if (shopNewParamData.ShopItemType > 0)
                {
                    _shopNewMainTabDefaultSelectedData.MainTabType = ShopNewMainTabType.ItemType;
                    _shopNewMainTabDefaultSelectedData.Index = shopNewParamData.ShopItemType;
                }
            }

            _shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(_shopId);

            if (_shopNewTable == null)
            {
                Logger.LogErrorFormat("The shopNewTable is null and shopId is {0}", _shopId);
                return;
            }

            _shopNewShopData = ShopNewDataManager.GetInstance().GetShopNewShopData(_shopId);

            InitShopView();
        }
        
        private void InitShopView()
        {
            InitShopTitle();

            InitShopElementItemList();

            InitShopMoney();

            InitShopMainTabList();

            //荣誉等级的View，只在特殊的商店中展示
            InitShopHonorLevelView();

            PlayShopNpcSound(NpcVoiceComponent.SoundEffectType.SET_Start);
        }

        private void InitShopTitle()
        {
            if (_shopNewTable == null)
                return;

            if (shopNameText != null)
                shopNameText.text = _shopNewTable.ShopName;

            if (_shopNewTable.HelpID <= 0)
            {
                if (helpButton != null)
                {
                    helpButton.gameObject.CustomActive(false);
                }
            }
            else
            {
                if (helpButton != null)
                {
                    helpButton.gameObject.CustomActive(true);


                    var helpAssistant = helpButton.GetComponent<HelpAssistant>();
                    if (helpAssistant == null)
                    {
                        helpAssistant = helpButton.gameObject.AddComponent<HelpAssistant>();
                    }

                    if (helpAssistant != null)
                    {
                        helpAssistant.eType = (HelpFrameContentTable.eHelpType) _shopNewTable.HelpID;
                    }
                }
            }
        }

        private void InitShopMoney()
        {
            //非按照页签显示消耗品
            if(_shopNewTable.CurrencyShowType == 0)
                UpdateShopMoneyByShopId(_shopId);
        }

        private void UpdateShopMoneyByShopId(int shopId)
        {
            if (shopNewMoneyView != null)
            {
                shopNewMoneyView.InitShopNewMoney(shopId);
            }
        }

        private void UpdateShopTabMoneyByShopTab(int shopId, int shopTab)
        {
            if (shopNewMoneyView != null)
                shopNewMoneyView.InitShopNewMoneyByShopTab(shopId, shopTab);
        }

        private void InitShopMainTabList()
        {
            _shopMainTabDataList = ShopNewDataManager.GetInstance().GetShopNewMainTabDataList(_shopId);

            _defaultMainTabIndex = 0;
            for (var i = 0; i < _shopMainTabDataList.Count; i++)
            {
                var curMainTabData = _shopMainTabDataList[i];
                if (curMainTabData.MainTabType == _shopNewMainTabDefaultSelectedData.MainTabType
                    && curMainTabData.Index == _shopNewMainTabDefaultSelectedData.Index)
                {
                    _defaultMainTabIndex = i;
                    break;
                }
            }

            if (_shopMainTabDataList == null)
            {
                Logger.LogErrorFormat("Shop MainTab is null and shopId is {0}", _shopId);
                return;
            }

            if (null != mToggleGroup)
            {
                var tabList = new List<CommonTabData>();
                int index = 0;
                foreach (var data in _shopMainTabDataList)
                {
                    var tabData = new CommonTabData();
                    tabData.id = index;
                    tabData.tabName = data.Name;
                    ++index;
                    tabList.Add(tabData);
                }
                mToggleGroup.InitComTab(_ToggleClick, _defaultMainTabIndex, tabList);
            }

        }

        private void InitShopElementItemList()
        {

            if (shopNewElementItemList != null)
            {
                shopNewElementItemList.Initialize();
                shopNewElementItemList.SetElementAmount(0);
                shopNewElementItemList.SelectElement(0);
            }

            _shopNewShopItemTableList = null;
        }

        private void PlayShopNpcSound(NpcVoiceComponent.SoundEffectType eSound)
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (null == current)
                return;

            if(_npcId <= 0)
                return;

            var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(_npcId);
            if(npcItem == null)
                return;

            current.PlayNpcSound(_npcId, eSound);
        }

        private void InitShopHonorLevelView()
        {
            CommonUtility.UpdateGameObjectVisible(honorLevelRoot, false);
            if (_shopNewTable == null)
                return;

            //不是决斗商店的类型，不显示
            if (_shopNewTable.ShopKind != ShopTable.eShopKind.SK_Fight)
                return;

            //荣誉系统没有开启或者解锁，不显示
            if (HonorSystemUtility.IsShowHonorSystem() == false)
                return;

            //展示荣誉等级:
            CommonUtility.UpdateGameObjectVisible(honorLevelRoot, true);
            if (honorLevelValueLabel != null)
            {
                var honorLevelStr = TR.Value("Honor_System_Current_Level_Format",
                    HonorSystemDataManager.GetInstance().PlayerHonorLevel);
                honorLevelValueLabel.text = honorLevelStr;
            }
        }

        #region ElementItemsList
       
        private int mCurSelectIndex = 0;
        private void OnElementItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if(_shopNewShopItemTableList == null)
                return;

            if(item.m_index < 0 || item.m_index >= _shopNewShopItemTableList.Count)
                return;

            var shopNewElementItem = item.GetComponent<ShopNewElementItem>();
            var shopNewShopItemTable = _shopNewShopItemTableList[item.m_index];

            if(shopNewShopItemTable == null ||shopNewElementItem == null)
                return;

            shopNewElementItem.InitElementItem(shopNewShopItemTable);
            shopNewElementItem.SetSelect(item.m_index == mCurSelectIndex);

        }

        private void OnElementItemSelect(ComUIListElementScript item)
        {
            var elementItem = item.GetComponent<ShopNewElementItem>();
            if (elementItem != null && item.m_index >= 0 && item.m_index < _shopNewShopItemTableList.Count)
            {
                var selectItem = shopNewElementItemList.GetElemenet(mCurSelectIndex);
                if (null != selectItem)
                {
                    var selectElementItem = selectItem.GetComponent<ShopNewElementItem>();
                    if (null != selectElementItem)
                        selectElementItem.SetSelect(false);
                }
                elementItem.SetSelect(true);
                InitShopContent(_shopNewShopItemTableList[item.m_index]);
                mCurSelectIndex = item.m_index;
            }
        }
        [SerializeField] private MallNewItemContent mContent;
        private void InitShopContent(ShopNewShopItemInfo itemData)
        {
            mContent.OnInitShopItem(itemData);
        }

        private void OnElementItemRecycle(ComUIListElementScript item)
        {
            if(item == null)
                return;

            var shopNewElementItem = item.GetComponent<ShopNewElementItem>();
            if(shopNewElementItem != null)
                shopNewElementItem.OnRecycleElementItem();
        }
        #endregion

        #region MainTabs
        //点击tab事件
        private void _ToggleClick(CommonTabData data)
        {
            if (null != _shopMainTabDataList && _shopMainTabDataList.Count > data.id)
                OnMainTabClickCallBack(data.id, _shopMainTabDataList[data.id]);
        }

        private void OnMainTabClickCallBack(int mainTabIndex,ShopNewMainTabData shopNewMainTabData)
        {
            _selectedMainTabIndex = mainTabIndex;

            //检测主按钮的数据是否相同， 如果相同，则直接返回
            if(_shopNewMainTabSelectedData == shopNewMainTabData)
            {
                Logger.LogErrorFormat("The same mainTabType and index. the mainTabType is {0}, the index is {1}",
                    shopNewMainTabData.MainTabType.ToString(), shopNewMainTabData.Index);
                return;
            }

            _shopNewMainTabSelectedData = shopNewMainTabData;

            //商店类型的页签 
            if (_shopNewMainTabSelectedData.MainTabType == ShopNewMainTabType.ShopType)
            {
                var childrenShopId = _shopNewMainTabSelectedData.Index;
                //更新对应商店的金币类型
                UpdateShopMoneyByShopId(childrenShopId);

                //第一次点击
                if (_shopNewMainTabSelectedData.IsClicked == false)
                {
                    var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(childrenShopId);
                    //子页签是商店类型，并且子页签需要刷新，第一次点击的时候，进行请求数据，并返回
                    // 数据请求结束之后，进行刷新
                    if (shopNewTable != null && shopNewTable.Refresh > 0)
                    {
                        UpdateMainTabSelectedData();
                        ShopNewDataManager.GetInstance().SendRequestChildrenShopData(_shopId, childrenShopId);
                        return;
                    }
                }
            }
            else if (_shopNewMainTabSelectedData.MainTabType == ShopNewMainTabType.ItemType)
            {
                //按照页签显示消耗品
                if (_shopNewTable.CurrencyShowType == 1)
                {
                    UpdateShopTabMoneyByShopTab(_shopId, _shopNewMainTabSelectedData.Index);
                }
            }

            UpdateMainTabSelectedData();
            UpdateShopView(_selectedMainTabIndex);
        }

        private void OnRequestChildrenShopDataSucceed(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var shopId = (int)uiEvent.Param1;
            var childrenShopId = (int)uiEvent.Param2;

            //主页签非商店类型
            if (_shopNewMainTabSelectedData.MainTabType != ShopNewMainTabType.ShopType)
                return;

            //父商店不同
            if (_shopId != shopId)
                return;

            //子商店不同
            if(_shopNewMainTabSelectedData.Index != childrenShopId)
                return;

            //收到子商店的请求数据，对View进行刷新操作
            UpdateShopView(_selectedMainTabIndex);

        }


        //更新按钮的点击状态
        private void UpdateMainTabSelectedData()
        {
            if (_shopNewMainTabSelectedData.IsClicked == false)
                _shopNewMainTabSelectedData.IsClicked = true;
        }

        //更新商店的View

        private void UpdateShopView(int mainTabIndex)
        {

            //更新每个主页签所对应过滤器的数量和类型
            UpdateFilterView(mainTabIndex);

            //针对主页签的类型，更新NoElementTip的内容
            UpdateShopNoElementTipLabel();

            //更新商店中商品的元素
            ShowShopElementItemList();

            SetShopDescriptionByMainTabChanged();

            UpdateShopTimeRefresh(mainTabIndex);
        }


        //当主页签改变的时候，更新第一第二过滤器
        private void UpdateFilterView(int index)
        {
            var firstFilterIndex = ShopNewDataManager.GetInstance().GetFirstFilterIndex(index);
            var secondFilterIndex = ShopNewDataManager.GetInstance().GetSecondFilterIndex(index);
            var filterTitleValue = ShopNewDataManager.GetInstance().GetFilterTitleIndex(index);

            if (firstFilterIndex < 0 || firstFilterIndex >= (int)ShopTable.eFilter.SF_COUNT)
                firstFilterIndex = 0;
            if (secondFilterIndex < 0 || secondFilterIndex >= (int)ShopTable.eFilter.SF_COUNT)
                secondFilterIndex = 0;

            var firstFilterType = (ShopTable.eFilter) firstFilterIndex;
            var secondFilterType = (ShopTable.eFilter) secondFilterIndex;

            if (firstFilterType == ShopTable.eFilter.SF_NONE)
            {
                _shopNewFirstFilterSelectedData = null;
            }
            else 
            {
                //第一个过滤器存在设置的数值
                if (_shopNewMainTabSelectedData.FirstFilterData != null)
                {
                    _shopNewFirstFilterSelectedData = _shopNewMainTabSelectedData.FirstFilterData;
                }
                else
                {
                    _shopNewFirstFilterSelectedData = ShopNewDataManager.GetInstance()
                        .GetDefaultFilterDataByFilterType(firstFilterType);
                }
            }

            if (secondFilterType == ShopTable.eFilter.SF_NONE)
            {
                _shopNewSecondFilterSelectedData = null;
            }
            else
            {
                //第一个过滤器存在设置的数值
                if (_shopNewMainTabSelectedData.SecondFilterData != null)
                {
                    _shopNewSecondFilterSelectedData = _shopNewMainTabSelectedData.SecondFilterData;
                }
                else
                {
                    _shopNewSecondFilterSelectedData = ShopNewDataManager.GetInstance()
                        .GetDefaultFilterDataByFilterType(secondFilterType);
                }
            }

            //商店类型的页签显示
            var isShowFilterTitle = _shopNewMainTabSelectedData != null &&
                _shopNewMainTabSelectedData.MainTabType == ShopNewMainTabType.ShopType;

            //非商店类型的页签，根据配置判断是否显示
            if (isShowFilterTitle == false)
            {
                isShowFilterTitle = filterTitleValue == 1 ? true : false;
            }
            

            if (shopNewFilterView != null)
            {
                shopNewFilterView.InitShopNewFilterView(_shopNewFirstFilterSelectedData,
                    OnFirstFilterElementItemTabClick,
                    _shopNewSecondFilterSelectedData,
                    OnSecondFilterElementItemTabClick,
                    isShowFilterTitle);
            }
        }

        //第一个过滤器
        private void OnFirstFilterElementItemTabClick(ShopNewFilterData shopNewFilterData)
        {
            _shopNewFirstFilterSelectedData = shopNewFilterData;

            ShowShopElementItemList();
        }

        //第二个过滤器
        private void OnSecondFilterElementItemTabClick(ShopNewFilterData shopNewFilterData)
        {
            _shopNewSecondFilterSelectedData = shopNewFilterData;

            ShowShopElementItemList();
        }

        private void ShowShopElementItemList()
        {
            //获得响应的商品元素
            _shopNewMainTabSelectedData.FirstFilterData = _shopNewFirstFilterSelectedData;
            _shopNewMainTabSelectedData.SecondFilterData = _shopNewSecondFilterSelectedData;

            _shopNewShopItemTableList = ShopNewDataManager.GetInstance().GetShopNewNeedShowingShopItemList(_shopId,
                _shopNewMainTabSelectedData,
                _shopNewFirstFilterSelectedData,
                _shopNewSecondFilterSelectedData);

            UpdateShopElementItemList();
        }

        private void SetShopDescriptionByMainTabChanged()
        {
            if(shopDescriptionText == null)
                return;

            shopDescriptionText.gameObject.CustomActive(false);

            if(_shopNewShopItemTableList == null || _shopNewShopItemTableList.Count <= 0)
                return;

            for (var i = 0; i < _shopNewShopItemTableList.Count; i++)
            {
                var shopNewShopItemTable = _shopNewShopItemTableList[i];
                if(shopNewShopItemTable == null || shopNewShopItemTable.ShopItemTable == null)
                    continue;

                var shopItemLimitBuyType = (GoodsLimitButyType) shopNewShopItemTable.ShopItemTable.ExLimite;
                if (shopItemLimitBuyType == GoodsLimitButyType.GLBT_FIGHT_SCORE)
                {
                    shopDescriptionText.gameObject.CustomActive(true);

                    var iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                    var rankName = SeasonDataManager.GetInstance().GetRankName(iCurValue);
                    shopDescriptionText.text = string.Format(TR.Value("shop_max_fight_score"), rankName);

                    break;
                }

                if (shopItemLimitBuyType == GoodsLimitButyType.GLBT_TOWER_LEVEL)
                {
                    shopDescriptionText.gameObject.CustomActive(true);
                    var iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                    shopDescriptionText.text = string.Format(TR.Value("shop_max_tower_level"), iCurValue);
                    break;
                }
            }
        }

        private void UpdateShopTimeRefresh(int mainTabIndex = 0)
        {
            if(shopTimeRefreshControl == null)
                return;

            if (shopTimeRefreshText == null)
                return;

            if(_shopNewTable == null)
                return;

            if(_shopNewShopData == null)
                return;

            var isCanRefresh = false;

            if (mainTabIndex >= 0 && mainTabIndex < _shopNewTable.NeedRefreshTabs.Count)
            {
                if (_shopNewTable.NeedRefreshTabs[mainTabIndex] == 1)
                {
                    isCanRefresh = true;
                }
            }

            if (isCanRefresh == true)
            {
                shopTimeRefreshText.enabled = false;
                shopTimeRefreshLabel.enabled = false;
                switch (_shopNewTable.RefreshCycle[mainTabIndex])
                {
                    case ShopTable.eRefreshCycle.REFRESH_CYCLE_DAILY:
                        SetShopTimeRefresh(_shopNewShopData.ResetRefreshTime);
                        break;
                    case ShopTable.eRefreshCycle.REFRESH_CYCLE_WEEK:
                        SetShopTimeRefresh(_shopNewShopData.WeekResetRefreshTime);
                        break;
                    case ShopTable.eRefreshCycle.REFRESH_CYCLE_MONTH:
                        SetShopTimeRefresh(_shopNewShopData.MonthRefreshTime);
                        break;
                }
            }
            else if (_shopNewTable.Refresh == 1)
            {
                SetShopTimeRefresh(_shopNewShopData.ResetRefreshTime);
            }
            else
            {
                shopTimeRefreshText.enabled = false;
                shopTimeRefreshLabel.enabled = false;
                shopTimeRefreshControl.Initialize();
                shopTimeRefreshControl.SetFormatString("{0}");
            }
        }

        private void SetShopTimeRefresh(uint refreshTime)
        {
            shopTimeRefreshText.enabled = true;
            shopTimeRefreshLabel.enabled = true;
            shopTimeRefreshControl.Initialize();
            shopTimeRefreshControl.SetFormatString("{0}");
            shopTimeRefreshControl.Time = refreshTime;
            shopTimeRefreshControl.enabled = true;
        }


        //只是拿到ElementItem数据，之后对ScrollView进行更新
        private void UpdateShopElementItemList()
        {
            var elementItemCount = 0;
            if (_shopNewShopItemTableList == null || _shopNewShopItemTableList.Count <= 0)
            {
                elementItemCount = 0;
            }
            else
            {
                elementItemCount = _shopNewShopItemTableList.Count;
            }

            if (shopNewElementItemList != null)
            {
                mCurSelectIndex = 0;
                //默认选中
                if (0 != mDefaultItemId)
                {
                    for (int index = 0; index < _shopNewShopItemTableList.Count; ++index)
                    {
                        if (_shopNewShopItemTableList[index].ShopItemId == mDefaultItemId)
                        {
                            mCurSelectIndex = index;
                            break;
                        }
                    }
                    mDefaultItemId = 0;
                }
                shopNewElementItemList.Initialize();
                shopNewElementItemList.SetElementAmount(elementItemCount);
                shopNewElementItemList.ResetContentPosition();
                shopNewElementItemList.SelectElement(mCurSelectIndex);
                if (elementItemCount > 0)
                    InitShopContent(_shopNewShopItemTableList[mCurSelectIndex]);
            }
        }

        //更新NoElementTip的内容
        private void UpdateShopNoElementTipLabel()
        {
            if (noElementTipLabel == null)
                return;

            //存在本职业过滤
            if (_shopNewFirstFilterSelectedData != null &&
                _shopNewFirstFilterSelectedData.FilterType == ShopTable.eFilter.SF_PLAY_OCCU)
            {
                //职业是否转职
                bool isProfessionChanged = PlayerBaseData.IsJobChanged();
                var noElementTipStr = TR.Value("shop_new_no_element_by_Profession");
                //已经转职
                if (isProfessionChanged == true)
                {
                    noElementTipStr = TR.Value("shop_new_all_element_SoldOut_by_Profession");
                }

                noElementTipLabel.text = noElementTipStr;
                return;
            }

			//存在本职业过滤
            if (_shopNewSecondFilterSelectedData != null
                && _shopNewSecondFilterSelectedData.FilterType == ShopTable.eFilter.SF_PLAY_OCCU)
            {
                //职业是否转职
                bool isProfessionChanged = PlayerBaseData.IsJobChanged();
                var noElementTipStr = TR.Value("shop_new_no_element_by_Profession");
                //已经转职
                if (isProfessionChanged == true)
                {
                    noElementTipStr = TR.Value("shop_new_all_element_SoldOut_by_Profession");
                }
                noElementTipLabel.text = noElementTipStr;
                return;
            }


            noElementTipLabel.text = TR.Value("shop_new_no_element_by_normal");
        }

        #endregion

        #region Help

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<ShopNewFrame>();
        }

        private void OnHelpButtonClick()
        {
            Logger.LogError("Help button Clicked");
        }

        #endregion

    }
}
