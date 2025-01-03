using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class AuctionNewBuyNoticeDetailController : AuctionNewBuyNoticeBaseController
    {

        private bool _isNoticeTab;          //是否为关注页签，
        private AuctionItemBaseInfo _auctionItemBaseInfo;                   //具体的某个商品信息，点击具体的某个在售的商品进行赋值。表示非关注页签
        private AuctionNewMenuTabDataModel _auctionNewMenuTabDataModel;     //关注页签的相关信息,点击关注页签才进行赋值，表示关注页签

        private AuctionNewMainTabType _mainTabType;

        private int _itemId;
        private AuctionSortType _sortType = AuctionSortType.PriceDesc;

        private AuctionNewItemDetailData _auctionNewItemDetailData = null;

        private int _curPage;
        private int _maxPage;

        private int _selectedStrengthenLevel = -1;      //在附魔卡详情界面起作用

        //过滤器存在和翻页按钮的响应的标识
        //服务器决定 或者 珍品物件和非珍品的装备,决定
        //限制过滤器的显示和是否可以翻页
        private bool _isHaveFilterAndPageButtonEnabled = true;     

        private OnUpdateFilterBackground _onUpdateFilterBackground;
        private Action _onBackButtonClick;

        private AuctionNewFilterData _filterData;           //在界面打开的时候不重置

        [SerializeField] private AuctionNewFilterView auctionNewFilterView;

        [Space(10)] [HeaderAttribute("ItemList")] [Space(5)]
        [SerializeField] private ComUIListScriptEx detailItemList;
        [SerializeField] private GameObject detailItemContentRoot;
        [SerializeField] private GameObject detailItemHaveFilterRoot;
        [SerializeField] private GameObject detailItemNoFilterRoot;

        [Space(10)]
        [HeaderAttribute("PageButton")]
        [Space(5)]
        [SerializeField] private Button prePageButton;
        [SerializeField] private UIGray prePageGray;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private UIGray nextPageGray;
        [SerializeField] private Text pageValue;

        [Space(10)] [HeaderAttribute("ReturnButton")] [Space(5)]
        [SerializeField] private GameObject backButtonRoot;
        [SerializeField] private Button backButton;
        [SerializeField] private Text backButtonText;
        [SerializeField] private Button strengthenLevelButton;
        [SerializeField] private Text strengthenLevelText;

        #region Init
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ResetData();
            _filterData = null;
        }

        private void OnEnable()
        {
            //请求成功之后，进行刷新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewReceiveItemDetailDataResSucceed,
                OnAuctionNewReceiveItemDetailResSucceed);
            //服务器发送要求去进行数据的拉取
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewNotifyRefreshToRequestDetailItems,
                OnAuctionNewNotifyRefreshToRequestDetailItems);

            //收到强化等级改变的请求
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewSelectMagicCardStrengthenLevel,
                OnAuctionNewSelectMagicCardStrengthenLevel);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewReceiveItemDetailDataResSucceed,
                OnAuctionNewReceiveItemDetailResSucceed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewNotifyRefreshToRequestDetailItems,
                OnAuctionNewNotifyRefreshToRequestDetailItems);
            //收到强化等级改变的请求
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewSelectMagicCardStrengthenLevel,
                OnAuctionNewSelectMagicCardStrengthenLevel);
        }

        private void BindEvents()
        {
            if (detailItemList != null)
            {
                detailItemList.Initialize();
                detailItemList.onItemVisiable += OnDetailItemVisible;
                detailItemList.OnItemRecycle += OnDetailItemRecycle;
            }
            
            if (prePageButton != null)
                prePageButton.onClick.AddListener(OnPrePageButtonClick);

            if (nextPageButton != null)
                nextPageButton.onClick.AddListener(OnNextPageButtonClick);

            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(OnBackButtonClick);
            }

            if (strengthenLevelButton != null)
            {
                strengthenLevelButton.onClick.RemoveAllListeners();
                strengthenLevelButton.onClick.AddListener(OnStrengthenLevelButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (detailItemList != null)
            {
                detailItemList.onItemVisiable -= OnDetailItemVisible;
                detailItemList.OnItemRecycle -= OnDetailItemRecycle;
            }
            
            if (prePageButton != null)
                prePageButton.onClick.RemoveAllListeners();

            if(nextPageButton != null)
                nextPageButton.onClick.RemoveAllListeners();

            if(backButton != null)
                backButton.onClick.RemoveAllListeners();

            if(strengthenLevelButton != null)
                strengthenLevelButton.onClick.RemoveAllListeners();
        }

        private void ResetData()
        {
            _isNoticeTab = false;
            _itemId = 0;
            _auctionItemBaseInfo = null;
            _mainTabType = AuctionNewMainTabType.None;
            _curPage = 0;
            _maxPage = 0;
            _auctionNewItemDetailData = null;
        }
       
        public void InitDetailController(OnUpdateFilterBackground onUpdateFilterBackground,
            Action onBackButtonClick = null)
        {
            _onUpdateFilterBackground = onUpdateFilterBackground;
            _onBackButtonClick = onBackButtonClick;

            InitBackButtonText();
        }


        #endregion

        #region UpdateDetailControllerView

        //进入页面详情：直接点击关注页签，isNoticeTab = true, 不需要AuctionBaseInfo;
        //通过具体的物品进入页面详情，isNoticeTab = false， 需要AuctionBaseInfo
        public void OnEnableDetailController(AuctionNewMainTabType auctionNewMainTabType,
            bool isNoticeTab, 
            AuctionItemBaseInfo auctionItemBaseInfo = null,
            bool isResetStrengthenLevel = false)
        {
            ResetData();

            _mainTabType = auctionNewMainTabType;
            _isNoticeTab = isNoticeTab;

            _curPage = 1;
            _maxPage = 1;

            _auctionItemBaseInfo = auctionItemBaseInfo;

            _itemId = 0;
            //非关注页签，AuctionItemBaseInfo不存在
            if (_isNoticeTab == false)
            {
                if (_auctionItemBaseInfo == null)
                {
                    Logger.LogErrorFormat("This is not NoticeTab and auctionItemBaseInfo is null");
                    return;
                }

                _itemId = (int) _auctionItemBaseInfo.itemTypeId;
            }

            //当从在售数量界面进入附魔卡的详情页面，对强化等级进行重置。主页签来回切换不重置
            if (isResetStrengthenLevel == true
                && AuctionNewUtility.IsMagicCardItem((uint)_itemId) == true)
            {
                _selectedStrengthenLevel = -1;
            }

            UpdateDetailController();
        }

        private void UpdateDetailController()
        {
            //重置展示页面的按钮和ItemList
            ResetDetailControllerView();

            //关注页签展示内容
            if (_isNoticeTab == true)
            {
                UpdateDetailControllerViewOfNoticeTab();
            }
            else
            {
                //非关注页签展示内容
                UpdateDetailControllerViewOnNormalTab();
            }

            //发送消息
            SendAuctionNewItemDetailListReq(_curPage);
        }

        //关注页签，不显示返回按钮和过滤器，显示非过滤器的背景
        private void UpdateDetailControllerViewOfNoticeTab()
        {
            if (backButtonRoot != null)
                backButtonRoot.CustomActive(false);

            if (_onUpdateFilterBackground != null)
                _onUpdateFilterBackground(false);

            if (auctionNewFilterView != null)
                auctionNewFilterView.gameObject.CustomActive(false);

            if (detailItemContentRoot != null && detailItemNoFilterRoot != null)
            {
                detailItemContentRoot.transform.localPosition = new Vector3(
                    detailItemContentRoot.transform.localPosition.x,
                    detailItemNoFilterRoot.transform.localPosition.y,
                    detailItemContentRoot.transform.localPosition.z);
            }
        }

        //关注页签，显示返回按钮，显示过滤器的背景，根据需求设置过滤器
        private void UpdateDetailControllerViewOnNormalTab()
        {
            if (backButtonRoot != null)
                backButtonRoot.CustomActive(true);

            if (_onUpdateFilterBackground != null)
                _onUpdateFilterBackground(true);

            if (detailItemContentRoot != null && detailItemHaveFilterRoot != null)
            {
                detailItemContentRoot.transform.localPosition = new Vector3(
                    detailItemContentRoot.transform.localPosition.x,
                    detailItemHaveFilterRoot.transform.localPosition.y,
                    detailItemContentRoot.transform.localPosition.z);
            }

            //过滤信息
            InitDetailControllerFilterData();
            UpdateBuyNoticeDetailFilterView();

            UpdateStrengthenLevelButton();

        }

        //重置展示页面的按钮和ItemList
        private void ResetDetailControllerView()
        {
            //按钮和也是重置
            UpdatePageButtonView();
            //ItemList进行重置
            if (detailItemList != null)
            {
                detailItemList.ResetContentPosition();
                detailItemList.SetElementAmount(0);
            }
        }
        #endregion

        #region FilterControl

        //过滤器相关数据
        private void InitDetailControllerFilterData()
        {
            //同步过来，返回true， 开关是关闭；没有同步过来，返回false，开关是打开的。
            //true 说明功能开关处于关闭状态:（所有的物品都可以翻页和价格筛选)
            //false 说明功能开关处在打开的状态：（物品有针对的可以翻页和价格筛选)
            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_AUCTION_PAGE) == true)
            {
                _isHaveFilterAndPageButtonEnabled = true;
                return;
            }

            //服务器决定限制按钮
            _isHaveFilterAndPageButtonEnabled = false;  //由Item来决定标志位的设置

            if (_auctionItemBaseInfo.isTreas == 1)
            {
                _isHaveFilterAndPageButtonEnabled = true;
                return;
            }

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_itemId);

            if (itemTable != null && itemTable.Type == ItemTable.eType.EQUIP)
            {
                _isHaveFilterAndPageButtonEnabled = true;
                return;
            }
        }

        //更新过滤器
        private void UpdateBuyNoticeDetailFilterView()
        {
            //不存在过滤器
            if (_isHaveFilterAndPageButtonEnabled == false)
            {
                _sortType = AuctionSortType.PriceAsc;

                if(auctionNewFilterView != null)
                    auctionNewFilterView.gameObject.CustomActive(false);
                return;
            }
            else
            {
                if (auctionNewFilterView != null)
                    auctionNewFilterView.gameObject.CustomActive(true);
            }

            //初始化过滤器
            if (_filterData == null)
            {
                var filterItemType = AuctionNewFrameTable.eFilterItemType.FIT_PRICE;
                var filterSortType = 1;

                _filterData = AuctionNewDataManager.GetInstance().GetDefaultAuctionNewFilterData(
                    filterItemType,
                    filterSortType);

                InitAuctionNewFilterView();
            }
            _sortType = GetAuctionSortType(_filterData);
        }

        //初始化过滤器
        private void InitAuctionNewFilterView()
        {
            if (auctionNewFilterView != null)
            {
                auctionNewFilterView.InitAuctionNewFilterView(_filterData,
                    OnFilterElementItemSelected,
                    null,
                    null);
            }
        }

        private void OnFilterElementItemSelected(AuctionNewFilterData auctionNewFilterData)
        {
            _filterData = auctionNewFilterData;
            _sortType = GetAuctionSortType(_filterData);

            //发送消息
            SendAuctionNewItemDetailListReq(_curPage);
        }

        private AuctionSortType GetAuctionSortType(AuctionNewFilterData filterData)
        {
            if (filterData == null || filterData.Sort == 1)
            {
                return AuctionSortType.PriceAsc;
            }

            return AuctionSortType.PriceDesc;

        }
        #endregion 

        #region PageButton

        private void OnPrePageButtonClick()
        {
            //关注页签的按钮点击
            //只是页面的切换和ItemList的刷新，不会请求数据
            if (_isNoticeTab == true)
            {
                if (_curPage <= 1)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_itemDetail_first_page"));
                }
                else
                {
                    _curPage -= 1;

                    UpdatePageData();
                    UpdateDetailItemList();

                }
                return;
            }

            //给关注页签
            //翻页功能不可点击
            if (_isHaveFilterAndPageButtonEnabled == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_itemDetail_limit_item"));
                return;
            }

            if (_curPage > 1)
            {
                SendAuctionNewItemDetailListReq(_curPage - 1);
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_itemDetail_first_page"));
            }
        }

        private void OnNextPageButtonClick()
        {
            //关注页签
            if (_isNoticeTab == true)
            {
                if (_curPage >= _maxPage)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_itemDetail_last_page"));
                }
                else
                {
                    _curPage = _curPage + 1;
                    UpdatePageData();
                    UpdateDetailItemList();
                }
                return;
            }

            //翻页功能不可点击
            if (_isHaveFilterAndPageButtonEnabled == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_itemDetail_limit_item"));
                return;
            }

            if (_curPage >= _maxPage)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_itemDetail_last_page"));
            }
            else
            {
                SendAuctionNewItemDetailListReq(_curPage + 1);
            }
        }
        
        #endregion

        #region ReceiveItemDetailInfo
        private void OnAuctionNewReceiveItemDetailResSucceed(UIEvent uiEvent)
        {
            if(uiEvent == null || uiEvent.Param1 == null)
                return;

            var goodState = (int) uiEvent.Param1;

            var auctionGoodState = (AuctionGoodState)goodState;

            //mainTabType的类型是否相同，是否是我要购买或者公式商品界面
            var isSameState = AuctionNewUtility.IsAuctionNewSameState(_mainTabType, auctionGoodState);
            if(isSameState == false)
                return;

            var auctionNewItemDetailData = AuctionNewDataManager.GetInstance().GetAuctionNewItemDetailData(goodState);
            //是否是对应页签下的商品详情数据
            if ((_isNoticeTab == true && auctionNewItemDetailData.NoticeType != 1)
                || (_isNoticeTab == false && auctionNewItemDetailData.NoticeType == 1))
                return;
            
            //数据为空
            if (auctionNewItemDetailData == null)
            {
                Logger.LogErrorFormat("_AuctionNewItemDetailData is null");
                return;
            }

            //如果返回的当前页数大于最大页数，再次拉去一次
            if (auctionNewItemDetailData.CurPage > auctionNewItemDetailData.MaxPage
                && (auctionNewItemDetailData.ItemDetailDataList == null ||
                    auctionNewItemDetailData.ItemDetailDataList.Count <= 0))
            {
                SendAuctionNewItemDetailListReq(auctionNewItemDetailData.MaxPage);
                return;
            }

            //满足要求，进行赋值，界面进行更新
            _auctionNewItemDetailData = auctionNewItemDetailData;

            _curPage = _auctionNewItemDetailData.CurPage;

            _maxPage = _auctionNewItemDetailData.MaxPage;
            UpdateMaxPageInfoInNoticeTab();

            //更新页数和按钮
            UpdatePageData();

            //更新ItemList
            UpdateDetailItemList();
        }

        //关注页签下的最大页数，重新计算，避免服务器返回不一致，以及购买之后减少一个的时候，最大页数进行重置
        private void UpdateMaxPageInfoInNoticeTab()
        {
            if (_isNoticeTab == true)
            {
                //关注页签，避免最大页数与返回的数量不一致
                if (_auctionNewItemDetailData != null && _auctionNewItemDetailData.ItemDetailDataList != null)
                {
                    if (_auctionNewItemDetailData.ItemDetailDataList.Count <= 0)
                        _maxPage = 1;
                    else
                    {
                        _maxPage = 1 + (_auctionNewItemDetailData.ItemDetailDataList.Count - 1) /
                                   AuctionNewDataManager.GetInstance().PageNumber;
                    }
                }
                if (_curPage > _maxPage)
                    _curPage = _maxPage;
            }
        }

        //结果返回之后，更新相应的数据
        private void UpdatePageData()
        {
            if (_maxPage < 1)
                _maxPage = 1;

            if (_curPage < 1)
            {
                _curPage = 1;
            }
            //else if (_curPage > _maxPage)
            //{
            //    _curPage = _maxPage;
            //}

            //按钮状态
            UpdatePageButtonView();
        }

        private void UpdatePageButtonView()
        {
            if (_curPage <= 1)
            {
                if (prePageButton != null)
                {
                    prePageButton.interactable = false;
                }

                if (prePageGray != null)
                {
                    prePageGray.enabled = true;
                }
            }
            else
            {
                if (prePageButton != null)
                    prePageButton.interactable = true;

                if (prePageGray != null)
                    prePageGray.enabled = false;
            }

            if (_curPage >= _maxPage)
            {
                if (nextPageButton != null)
                    nextPageButton.interactable = false;
                if (nextPageGray != null)
                    nextPageGray.enabled = true;
            }
            else
            {
                if (nextPageButton != null)
                    nextPageButton.interactable = true;
                if (nextPageGray != null)
                    nextPageGray.enabled = false;
            }

            //显示的数字
            if (pageValue != null)
            {
                pageValue.text = string.Format(TR.Value("auction_new_itemDetail_page_value"),
                    _curPage,
                    _maxPage);
            }
        }
        #endregion

        #region DetailItemList
        //非关注页签，显示服务器发送的所有数据，最多不会超过6个数据
        //关注页签，服务器会把关注的数据都发送过来，按照分页进行显示，每页只显示
        private void UpdateDetailItemList()
        {
            if (detailItemList != null)
            {
                detailItemList.ResetContentPosition();
                if (_auctionNewItemDetailData.ItemDetailDataList == null ||
                    _auctionNewItemDetailData.ItemDetailDataList.Count <= 0)
                {
                    detailItemList.SetElementAmount(0);
                }
                else
                {
                    //非关注页签
                    if (_isNoticeTab == false)
                    {
                        detailItemList.SetElementAmount(_auctionNewItemDetailData.ItemDetailDataList.Count);
                    }
                    else
                    {
                        var noticeTotalItemNumber = _auctionNewItemDetailData.ItemDetailDataList.Count;
                        var curPageNumber = AuctionNewDataManager.GetInstance().PageNumber;
                        //关注页签，当前页签只显示相应的数量
                        //如果总数量不够当前页面显示,只显示相应数量的物品
                        if (noticeTotalItemNumber < _curPage * AuctionNewDataManager.GetInstance().PageNumber)
                        {
                            curPageNumber = noticeTotalItemNumber -
                                            (_curPage - 1) * AuctionNewDataManager.GetInstance().PageNumber;
                            //避免出现负数的情况
                            if (curPageNumber <= 0)
                                curPageNumber = 0;
                        }
                        detailItemList.SetElementAmount(curPageNumber);
                    }
                }
            }
        }

        private void OnDetailItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var detailItem = item.GetComponent<AuctionNewBuyNoticeDetailItem>();
            if (detailItem == null)
                return;


            AuctionBaseInfo auctionBaseInfo = null;
            if (_isNoticeTab == false)
            {
                //非关注界面
                if (item.m_index < 0 || item.m_index >= _auctionNewItemDetailData.ItemDetailDataList.Count)
                    return;
                auctionBaseInfo = _auctionNewItemDetailData.ItemDetailDataList[item.m_index];
            }
            else
            {
                //关注界面：
                if (item.m_index < 0)
                    return;

                var curDetailDataIndex = (_curPage - 1) * AuctionNewDataManager.GetInstance().PageNumber + item.m_index;
                if (curDetailDataIndex < 0 || curDetailDataIndex >= _auctionNewItemDetailData.ItemDetailDataList.Count)
                    return;

                auctionBaseInfo = _auctionNewItemDetailData.ItemDetailDataList[curDetailDataIndex];
            }

            if (auctionBaseInfo != null)
            {
                detailItem.InitItem(_mainTabType, auctionBaseInfo);
            }
        }

        private void OnDetailItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var detailItem = item.GetComponent<AuctionNewBuyNoticeDetailItem>();
            if(detailItem != null)
                detailItem.OnItemRecycle();
        }
        #endregion

        #region DetailItemReq
        //自动请求相关的数据
        private void OnAuctionNewNotifyRefreshToRequestDetailItems(UIEvent uiEvent)
        {
            //非关注页签，直接拉去数据
            if (_isNoticeTab == false)
            {
                SendAuctionNewItemDetailListReq(_curPage);
            }
            else
            {
                //关注界面，
                //更新相应的道具数据，并进行刷新，不拉去。关注界面只在点击界面的时候拉去一次
                if(uiEvent == null || uiEvent.Param1 == null)
                    return;

                var buyGuid = (ulong) uiEvent.Param1;
                RefreshNoticeDetailViewByBuyItem(buyGuid);
            }
        }

        //购买关注界面下的一个Item， 通过购买Item的guid，进行相关的刷新操作
        private void RefreshNoticeDetailViewByBuyItem(ulong buyGuid)
        {
            if (_auctionNewItemDetailData == null || _auctionNewItemDetailData.ItemDetailDataList == null
                                                  || _auctionNewItemDetailData.ItemDetailDataList.Count <= 0)
            {
                Logger.LogErrorFormat(
                    "auctionNewItemDetailData is null or ItemDetailDataList is null or count is zero");
                return;
            }

            //在缓存中查找数据：如果找到，删除数据，并对界面进行刷新
            for (var i = 0; i < _auctionNewItemDetailData.ItemDetailDataList.Count; i++)
            {
                var curItem = _auctionNewItemDetailData.ItemDetailDataList[i];
                if (curItem != null)
                {
                    if (curItem.guid == buyGuid)
                    {
                        //删除数据
                        _auctionNewItemDetailData.ItemDetailDataList.RemoveAt(i);
                        //更新相关的数据
                        UpdateMaxPageInfoInNoticeTab();
                        UpdatePageData();
                        UpdateDetailItemList();
                        return;
                    }
                }
            }
            return;
        }

        //请求相关的数据
        private void SendAuctionNewItemDetailListReq(int pageIndex)
        {
            //非关注页面请求物品的详情
            if (_isNoticeTab == false)
            {
                //附魔卡
                if (AuctionNewUtility.IsMagicCardItem((uint)_itemId) == true)
                {
                    //需要处理强化等级
                    //0,表示查询所有等级，如果只查询0级的附魔卡，
                    //设置等级为100，和服务器约定
                    int minStrengthLevel = 0;
                    int maxStrengthenLevel = 0;
                    if (_selectedStrengthenLevel == 0)
                    {
                        minStrengthLevel = AuctionNewDataManager.GetInstance().DefaultMagicCardZeroStrengthenLevelQuery;
                        maxStrengthenLevel =
                            AuctionNewDataManager.GetInstance().DefaultMagicCardZeroStrengthenLevelQuery;
                    }
                    else if (_selectedStrengthenLevel > 0)
                    {
                        minStrengthLevel = _selectedStrengthenLevel;
                        maxStrengthenLevel = _selectedStrengthenLevel;
                    }
                    
                    AuctionNewDataManager.GetInstance().SendAuctionNewItemDetailListReq(_itemId,
                        _mainTabType,
                        pageIndex,
                        (int) _sortType,
                        0,
                        minStrengthLevel,
                        maxStrengthenLevel);
                }
                else
                {
                    AuctionNewDataManager.GetInstance().SendAuctionNewItemDetailListReq(_itemId,
                        _mainTabType,
                        pageIndex,
                        (int)_sortType,
                        0);
                }
            }
            else
            {
                //按照价格升序
                //关注界面请求关注的物品
                AuctionNewDataManager.GetInstance().SendAuctionNewItemDetailListReq(0,
                    _mainTabType,
                    pageIndex,
                    (int)AuctionSortType.PriceAsc,
                    1); 
            }
        }
       
        #endregion

        #region ReturnButton

        private void InitBackButtonText()
        {
            if (backButtonText != null)
                backButtonText.text = TR.Value("auction_new_return_layer");
        }

        private void OnBackButtonClick()
        {
            if (_onBackButtonClick != null)
                _onBackButtonClick();
        }

        #endregion

        #region MagicCardStrengthenLevel

        private void UpdateStrengthenLevelButton()
        {
            strengthenLevelButton.gameObject.CustomActive(false);
            if (strengthenLevelButton != null)
            {
                if (_auctionItemBaseInfo != null
                    && AuctionNewUtility.IsMagicCardItem(_auctionItemBaseInfo.itemTypeId) == true)
                {
                    strengthenLevelButton.gameObject.CustomActive(true);
                    UpdateStrengthenLevelText();
                    //附魔卡界面存在等级按钮就不显示价格过滤器
                    if (auctionNewFilterView != null)
                        auctionNewFilterView.gameObject.CustomActive(false);
                }
            }
        }

        private void UpdateStrengthenLevelText()
        {
            if (strengthenLevelText != null)
            {
                if (_selectedStrengthenLevel <= -1)
                {
                    strengthenLevelText.text = TR.Value("auction_new_magic_card_all_level");
                }
                else
                {
                    strengthenLevelText.text = TR.Value("auction_new_magic_card_strengthen_level_normal",
                        _selectedStrengthenLevel);
                }
            }
        }


        private void OnStrengthenLevelButtonClick()
        {
            AuctionNewUtility.OnOpenAuctionNewMagicCardStrengthLevelFrame(_auctionItemBaseInfo.itemTypeId,
                _selectedStrengthenLevel);
        }

        private void OnAuctionNewSelectMagicCardStrengthenLevel(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var itemId = (int) uiEvent.Param1;
            var selectLevel = (int) uiEvent.Param2;

            if (_isNoticeTab == true)
                return;

            if (_itemId != itemId)
                return;

            if (AuctionNewUtility.IsMagicCardItem((uint) _itemId) == false)
                return;

            ////等级相同，不用刷新
            //if (selectLevel == _selectedStrengthenLevel)
            //    return;

            _selectedStrengthenLevel = selectLevel;

            //更新界面
            UpdateStrengthenLevelText();

            SendAuctionNewItemDetailListReq(1);
        }


        #endregion


    }
}