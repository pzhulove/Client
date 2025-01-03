using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public delegate void OnBuyNoticeOnSaleItemClick(AuctionItemBaseInfo auctionItemBaseInfo);

    public class AuctionNewBuyNoticeOnSaleController : AuctionNewBuyNoticeBaseController
    {

        private bool _isHaveFilter = false;

        private AuctionNewMenuTabDataModel _firstLayerMenuDataModel;
        private AuctionNewMenuTabDataModel _secondLayerMenuDataModel;
        private AuctionNewMenuTabDataModel _thirdLayerMenuDataModel;

        private AuctionNewFilterData _firstFilterData;
        private AuctionNewFilterData _secondFilterData;
        private AuctionNewFilterData _thirdFilterData;

        private AuctionNewMainTabType _mainTabType;

        private List<AuctionItemBaseInfo> _auctionNewItemResList;

        [SerializeField] private AuctionNewFilterView auctionNewFilterView;

        private OnBuyNoticeOnSaleItemClick _onBuyNoticeOnSaleItemClick;
        private OnUpdateFilterBackground _onUpdateFilterBackground;

        private List<int> _itemIdList;          //对应三个层级下的ItemId的列表
        private bool _isShowTreasure = false;     //是否显示珍品过滤，只在我要购买中起作用，由服务器控制。false,

        private bool _onlyShowOnSaleItemFlag = true;     //只显示在售的商品，默认为true
        private bool _onlyShowTreasureItmFlag = false;   //只显示珍品，默认为false;

        [Space(10)]
        [HeaderAttribute("ItemList")]
        [Space(5)]
        [SerializeField] private ComUIListScript onSaleItemList;

        [SerializeField] private GameObject onSaleItemListRoot;
        [SerializeField] private GameObject onSaleItemHaveFilterRoot;
        [SerializeField] private GameObject onSaleItemNoFilterRoot;

        [Space(10)] [HeaderAttribute("ToggleSelected")] [Space(5)] [SerializeField]
        private AuctionNewToggleSelectedController onlyShowOnSaleItemController;
        [SerializeField] private AuctionNewToggleSelectedController onlyShowTreasureItemController;
        

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ResetData();
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewReceiveItemNumResSucceed,
                OnAuctionNewReceiveItemNumResSucceed);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewReceiveItemNumResSucceed,
                OnAuctionNewReceiveItemNumResSucceed);

            //重置OnSaleList
            ResetOnSaleItemList();
        }

        private void BindEvents()
        {
            if (onSaleItemList != null)
            {
                onSaleItemList.Initialize();
                onSaleItemList.onItemVisiable += OnItemVisible;
                onSaleItemList.OnItemRecycle += OnItemRecycle;
            }
        }

        private void UnBindEvents()
        {
            if (onSaleItemList != null)
            {
                onSaleItemList.onItemVisiable -= OnItemVisible;
                onSaleItemList.OnItemRecycle -= OnItemRecycle;
            }
        }

        private void ResetData()
        {
            _onBuyNoticeOnSaleItemClick = null;
            _firstLayerMenuDataModel = null;
            _secondLayerMenuDataModel = null;
            _thirdLayerMenuDataModel = null;
            _mainTabType = AuctionNewMainTabType.None;
            _auctionNewItemResList = null;
            _isHaveFilter = false;

            ResetToggleControllerFlag();
        }

        #region InitOnSale
        public void InitOnSaleController(OnBuyNoticeOnSaleItemClick onBuyNoticeOnSaleItemClick,
            OnUpdateFilterBackground onUpdateFilterBackground,
            AuctionNewMainTabType auctionNewMainTabType = AuctionNewMainTabType.AuctionBuyType)
        {
            _onBuyNoticeOnSaleItemClick = onBuyNoticeOnSaleItemClick;
            _onUpdateFilterBackground = onUpdateFilterBackground;

            //第一次打开的时候，初始化ToggleSelected
            InitToggleSelectedController(auctionNewMainTabType);
        }

        //设置Toggle的标志位默认值
        private void ResetToggleControllerFlag()
        {
            _onlyShowOnSaleItemFlag = true;
            _onlyShowTreasureItmFlag = false;
        }

        private void InitToggleSelectedController(AuctionNewMainTabType auctionNewMainTabType)
        {
            ResetToggleControllerFlag();

            _isShowTreasure = true;

            //公示标签下，仅显示有货Toggle显示
            if (auctionNewMainTabType == AuctionNewMainTabType.AuctionNoticeType)
            {
                if (onlyShowTreasureItemController != null)
                    onlyShowTreasureItemController.gameObject.CustomActive(false);

                if (onlyShowOnSaleItemController != null)
                {
                    onlyShowOnSaleItemController.InitToggleSelectedController(_onlyShowOnSaleItemFlag,
                        TR.Value("auction_new_only_show_onSale"),
                        OnOnlyShowOnSaleItemButtonClick);
                }
            }
            else
            {
                //在售页签

                //珍品机制没有打开
                if (AuctionNewUtility.IsAuctionTreasureItemOpen() == false)
                    _isShowTreasure = false;

                if(onlyShowOnSaleItemController != null)
                    onlyShowOnSaleItemController.InitToggleSelectedController(_onlyShowOnSaleItemFlag,
                        TR.Value("auction_new_only_show_onSale"),
                        OnOnlyShowOnSaleItemButtonClick);

                if (onlyShowTreasureItemController != null)
                    onlyShowTreasureItemController.InitToggleSelectedController(
                        _onlyShowTreasureItmFlag,
                        TR.Value("auction_new_only_show_treasure"),
                        OnOnlyShowTreasureItemButtonClick);

                if (_isShowTreasure == false)
                {
                    if (onlyShowTreasureItemController != null)
                        onlyShowTreasureItemController.gameObject.CustomActive(false);
                }

            }
        }

        #endregion

        #region OnEnableOnSale
        public void OnEnableOnSaleController(AuctionNewMenuTabDataModel firstLayerMenuTabDataModel,
            AuctionNewMenuTabDataModel secondLayerMenuTabDataModel,
            AuctionNewMenuTabDataModel thirdLayerMenuTabDataModel,
            AuctionNewMainTabType auctionNewMainTabType = AuctionNewMainTabType.None)
        {

            var isSameOnSaleLayer = IsSameOnSaleLayer(firstLayerMenuTabDataModel,
                secondLayerMenuTabDataModel,
                thirdLayerMenuTabDataModel,
                auctionNewMainTabType);
            //如果不是同一类型，则List首先重置；否则的话，List可以先不重置，避免频繁点击二级按钮List可能是null的情况
            if (isSameOnSaleLayer == false)
            {
                ResetOnSaleItemList();
                _itemIdList = null;
            }

            ResetOnSaleToggleController();

            _firstLayerMenuDataModel = firstLayerMenuTabDataModel;
            _secondLayerMenuDataModel = secondLayerMenuTabDataModel;
            _thirdLayerMenuDataModel = thirdLayerMenuTabDataModel;
            _mainTabType = auctionNewMainTabType;

            UpdateBuyNoticeOnSaleFilter();
            UpdateOnSaleItemListPosition();

            if (_onUpdateFilterBackground != null)
                _onUpdateFilterBackground(_isHaveFilter);

            BuyNoticeOnSaleSendReq();
        }

        //重置TreasureToggle 和 OnSaleToggle
        private void ResetOnSaleToggleController()
        {
            if (onlyShowOnSaleItemController != null)
                onlyShowOnSaleItemController.ResetAuctionNewToggleSelectedController();

            if (onlyShowTreasureItemController != null)
                onlyShowTreasureItemController.ResetAuctionNewToggleSelectedController();
        }

        //更新ItemList的位置
        private void UpdateOnSaleItemListPosition()
        {
            //OnSaleItemList
            if (_isHaveFilter == true)
            {
                if (onSaleItemListRoot != null && onSaleItemHaveFilterRoot != null)
                {
                    onSaleItemListRoot.transform.localPosition = new Vector3(
                        onSaleItemListRoot.transform.localPosition.x,
                        onSaleItemHaveFilterRoot.transform.localPosition.y,
                        onSaleItemListRoot.transform.localPosition.z);
                }
            }
            else
            {
                if (onSaleItemListRoot != null && onSaleItemNoFilterRoot != null)
                {
                    onSaleItemListRoot.transform.localPosition = new Vector3(
                        onSaleItemListRoot.transform.localPosition.x,
                        onSaleItemNoFilterRoot.transform.localPosition.y,
                        onSaleItemListRoot.transform.localPosition.z);
                }
            }
        }

        private void ResetOnSaleItemList()
        {
            if (onSaleItemList != null)
                onSaleItemList.SetElementAmount(0);
        }

        private void BuyNoticeOnSaleSendReq()
        {
            AuctionNewDataManager.GetInstance().SendAuctionNewOnSaleItemListReq(_mainTabType,
                _firstLayerMenuDataModel,
                _secondLayerMenuDataModel,
                _thirdLayerMenuDataModel,
                _firstFilterData,
                _secondFilterData,
                _thirdFilterData);
        }
        #endregion

        #region FilterData
        //更新过滤器，过滤器的类型
        //确认过滤器的类型
        private void UpdateBuyNoticeOnSaleFilter()
        {
            var filterItemType = _firstLayerMenuDataModel.AuctionNewFrameTable.FilterItemType;
            var filterSortType = _firstLayerMenuDataModel.AuctionNewFrameTable.FilterSortType;

            if (_thirdLayerMenuDataModel != null && _thirdLayerMenuDataModel.AuctionNewFrameTable != null)
            {
                filterItemType = _thirdLayerMenuDataModel.AuctionNewFrameTable.FilterItemType;
                filterSortType = _thirdLayerMenuDataModel.AuctionNewFrameTable.FilterSortType;
            }
            else if (_secondLayerMenuDataModel != null && _secondLayerMenuDataModel.AuctionNewFrameTable != null)
            {
                filterItemType = _secondLayerMenuDataModel.AuctionNewFrameTable.FilterItemType;
                filterSortType = _secondLayerMenuDataModel.AuctionNewFrameTable.FilterSortType;
            }

            if (filterItemType == null || filterItemType.Count <= 0
                                       || (filterItemType.Count == 1 && filterItemType[0] ==
                                           AuctionNewFrameTable.eFilterItemType.FIT_NONE))
            {
                _firstFilterData = null;
                _secondFilterData = null;
                _thirdFilterData = null;
                InitAuctionNewFilterView();
            }
            else
            {
                //一个过滤器
                if (filterItemType.Count == 1)
                {
                    var curFilterItemType = filterItemType[0];
                    var curFilterSortType = 1;
                    if (filterSortType != null &&
                        filterSortType.Count == 1
                        && filterSortType[0] != 0)
                        curFilterSortType = filterSortType[0];

                    _secondFilterData = null;
                    _thirdFilterData = null;

                    if (_firstFilterData == null || _firstFilterData.FilterItemType != curFilterItemType)
                    {
                        _firstFilterData = AuctionNewDataManager.GetInstance().GetDefaultAuctionNewFilterData(
                            curFilterItemType,
                            curFilterSortType); //只有一个需要重置
                    }
                    InitAuctionNewFilterView();
                }
                else if (filterItemType.Count == 2)
                {
                    //两个过滤器
                    var firstFilterItemType = filterItemType[0];
                    var secondFilterItemType = filterItemType[1];
                    var firstFilterSortType = 1;
                    var secondFilterSortType = 1;

                    if (filterSortType != null && filterSortType.Count == 2)
                    {
                        firstFilterSortType = filterSortType[0];
                        secondFilterSortType = filterSortType[1];
                    }

                    if (_firstFilterData != null && _firstFilterData.FilterItemType == firstFilterItemType
                                                 && _secondFilterData != null && _secondFilterData.FilterItemType ==
                                                 secondFilterItemType
                                                 && _thirdFilterData == null)
                        return;

                    _thirdFilterData = null;

                    if (_firstFilterData == null || _firstFilterData.FilterItemType != firstFilterItemType)
                        _firstFilterData = AuctionNewDataManager.GetInstance().GetDefaultAuctionNewFilterData(
                            firstFilterItemType,
                            firstFilterSortType); //第一个重置

                    if (_secondFilterData == null || _secondFilterData.FilterItemType != secondFilterItemType)
                        _secondFilterData = AuctionNewDataManager.GetInstance().GetDefaultAuctionNewFilterData(
                            secondFilterItemType,
                            secondFilterSortType);  //第二个重置

                    InitAuctionNewFilterView();

                }
                else if (filterItemType.Count == 3)
                {
                    //三个过滤器
                    var firstFilterItemType = filterItemType[0];
                    var secondFilterItemType = filterItemType[1];
                    var thirdFilterItemType = filterItemType[2];
                    var firstFilterSortType = 1;
                    var secondFilterSortType = 1;
                    var thirdFilterSortType = 1;

                    if (filterSortType != null && filterSortType.Count == 3)
                    {
                        firstFilterSortType = filterSortType[0];
                        secondFilterSortType = filterSortType[1];
                        thirdFilterSortType = filterSortType[2];
                    }

                    if (_firstFilterData != null
                        && _firstFilterData.FilterItemType == firstFilterItemType
                        && _secondFilterData != null
                        && _secondFilterData.FilterItemType == secondFilterItemType
                        && _thirdFilterData != null
                        && _thirdFilterData.FilterItemType == thirdFilterItemType)
                        return;

                    if (_firstFilterData == null || _firstFilterData.FilterItemType != firstFilterItemType)
                        _firstFilterData = AuctionNewDataManager.GetInstance().GetDefaultAuctionNewFilterData(
                            firstFilterItemType,
                            firstFilterSortType); //第一个重置

                    if (_secondFilterData == null || _secondFilterData.FilterItemType != secondFilterItemType)
                        _secondFilterData = AuctionNewDataManager.GetInstance().GetDefaultAuctionNewFilterData(
                            secondFilterItemType,
                            secondFilterSortType);  //第二个重置

                    //第三个重置
                    if (_thirdFilterData == null || _thirdFilterData.FilterItemType != thirdFilterItemType)
                        _thirdFilterData = AuctionNewDataManager.GetInstance().GetDefaultAuctionNewFilterData(
                            thirdFilterItemType,
                            thirdFilterSortType);
                    

                    InitAuctionNewFilterView();

                }
            }
        }

        private void InitAuctionNewFilterView()
        {
            if (auctionNewFilterView != null)
            {
                auctionNewFilterView.InitAuctionNewFilterView(_firstFilterData,
                    OnFirstFilterElementItemSelected,
                    _secondFilterData,
                    OnSecondFilterElementItemSelected,
                    _thirdFilterData,
                    OnThirdFilterElementItemSelected);
            }

            //是否拥有过滤器的标志
            _isHaveFilter = false;
            if (_firstFilterData != null &&
                _firstFilterData.FilterItemType != AuctionNewFrameTable.eFilterItemType.FIT_NONE)
                _isHaveFilter = true;

            if (_secondFilterData != null &&
                _secondFilterData.FilterItemType != AuctionNewFrameTable.eFilterItemType.FIT_NONE)
                _isHaveFilter = true;

            if (_isHaveFilter == false)
            {
                if (_thirdFilterData != null
                    && _thirdFilterData.FilterItemType != AuctionNewFrameTable.eFilterItemType.FIT_NONE)
                    _isHaveFilter = true;
            }

        }

        private void OnFirstFilterElementItemSelected(AuctionNewFilterData auctionNewFilterData)
        {
            _firstFilterData = auctionNewFilterData;
            BuyNoticeOnSaleSendReq();
        }

        private void OnSecondFilterElementItemSelected(AuctionNewFilterData auctionNewFilterData)
        {
            _secondFilterData = auctionNewFilterData;
            BuyNoticeOnSaleSendReq();
        }

        private void OnThirdFilterElementItemSelected(AuctionNewFilterData auctionNewFilterData)
        {
            _thirdFilterData = auctionNewFilterData;
            BuyNoticeOnSaleSendReq();
        }

        #endregion

        #region OnSaleViewElementItem

        private void OnAuctionNewReceiveItemNumResSucceed(UIEvent uiEvent)
        {
            if(uiEvent == null || uiEvent.Param1 == null)
                return;

            var goodState = (int) uiEvent.Param1;
            var auctionGoodState = (AuctionGoodState) goodState;

            var isSameState = AuctionNewUtility.IsAuctionNewSameState(_mainTabType, auctionGoodState);
            if(isSameState == false)
                return;

            //_auctionNewItemResList = AuctionNewDataManager.GetInstance().GetAuctionNewItemNumResList(goodState);

            //显示在售和非在售的ItemList
            var auctionNewOnSaleItemList = AuctionNewDataManager.GetInstance().GetAuctionNewItemNumResList(goodState);
            UpdateAuctionNewShowItemList(auctionNewOnSaleItemList);

            OnUpdateOnSaleItemView();

        }

        private void OnUpdateOnSaleItemView()
        {

            if (_auctionNewItemResList == null)
                return;

            var itemResListCount = _auctionNewItemResList.Count;

            StopOnSaleItemScrollView();

            if (onSaleItemList != null)
            {
                onSaleItemList.ResetContentPosition();
                onSaleItemList.SetElementAmount(itemResListCount);
            }
        }

        private void StopOnSaleItemScrollView()
        {
            if(onSaleItemList != null
               && onSaleItemList.m_scrollRect != null)
                onSaleItemList.m_scrollRect.StopMovement();
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if(item.m_index < 0 || item.m_index >= _auctionNewItemResList.Count)
                return;

            var auctionItemBaseInfo = _auctionNewItemResList[item.m_index];
            var onSaleItem = item.GetComponent<AuctionNewBuyNoticeOnSaleItem>();

            if (auctionItemBaseInfo != null && onSaleItem != null)
            {
                onSaleItem.InitItem(_mainTabType,
                    auctionItemBaseInfo,
                    OnAuctionNewOnSaleItemClick);
            }

        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var onSaleItem = item.GetComponent<AuctionNewBuyNoticeOnSaleItem>();
            if(onSaleItem != null)
                onSaleItem.OnItemRecycle();
        }

        private void OnAuctionNewOnSaleItemClick(AuctionItemBaseInfo auctionItemBaseInfo)
        {
            if (_onBuyNoticeOnSaleItemClick != null && auctionItemBaseInfo != null)
                _onBuyNoticeOnSaleItemClick(auctionItemBaseInfo);
        }


        #endregion

        #region ItemNumberList
        private void UpdateAuctionNewShowItemList(List<AuctionItemBaseInfo> auctionNewOnSaleItemList)
        {
            if (_auctionNewItemResList == null)
                _auctionNewItemResList = new List<AuctionItemBaseInfo>();
            _auctionNewItemResList.Clear();

            //过滤在售的物品
            for (var i = 0; i < auctionNewOnSaleItemList.Count; i++)
            {
                if (auctionNewOnSaleItemList[i] == null)
                    continue;

                //我要购买页面，可能需要过滤非珍品
                if (_mainTabType == AuctionNewMainTabType.AuctionBuyType)
                {
                    //只显示珍品，则只添加珍品
                    if (_onlyShowTreasureItmFlag == true)
                    {
                        if (auctionNewOnSaleItemList[i].isTreas == 1)
                            _auctionNewItemResList.Add(auctionNewOnSaleItemList[i]);
                    }
                    else
                    {
                        _auctionNewItemResList.Add(auctionNewOnSaleItemList[i]);
                    }
                }
                else
                {
                    _auctionNewItemResList.Add(auctionNewOnSaleItemList[i]);
                }
            }

            AuctionNewUtility.SortOnSaleItemList(_auctionNewItemResList);

            //只显示有货
            if(_onlyShowOnSaleItemFlag == true)
                return;

            //需要显示非在售的物品
            //获得所有的ItemId，只有第一次使用的时候才计算，并进行缓存，之后使用缓存数据。
            if (_itemIdList == null)
                _itemIdList = AuctionNewDataManager.GetInstance().GetAuctionNewItemIdList(_firstLayerMenuDataModel,
                    _secondLayerMenuDataModel,
                    _thirdLayerMenuDataModel);

            if (_itemIdList == null || _itemIdList.Count <= 0)
                return;

            //过滤非在售物品
            List<ItemTable> notOnSaleItemTableList = new List<ItemTable>();
            //添加符合条件的非在售的物品
            for (var i = 0; i < _itemIdList.Count; i++)
            {
                var itemId = _itemIdList[i];
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(_itemIdList[i]);
                if (itemTableData == null)
                    continue;

                //我要构面界面，过滤掉非珍品
                if (_mainTabType == AuctionNewMainTabType.AuctionBuyType
                    && _onlyShowTreasureItmFlag == true
                    && itemTableData.IsTreas == 0)
                    continue;

                //公示页面，过滤掉非珍品
                if (_mainTabType == AuctionNewMainTabType.AuctionNoticeType
                   && itemTableData.IsTreas == 0)
                    continue;

                //两级过滤器进行过滤：等级，品质，珠宝的转化率，职业相关
                //三个过滤器
                var isShowItem = AuctionNewUtility.IsItemCanShow(itemTableData, _firstFilterData);
                if (isShowItem == true)
                    isShowItem = AuctionNewUtility.IsItemCanShow(itemTableData, _secondFilterData);
                if (isShowItem == true)
                    isShowItem = AuctionNewUtility.IsItemCanShow(itemTableData, _thirdFilterData);

                if (isShowItem == false)
                    continue;

                //非在售物品需要被屏蔽（服务器同步数据确定屏蔽)
                var isNeedDeletedByServer = AuctionNewDataManager.GetInstance().IsNotOnSaleItemNeedBeDeleted(itemId);
                if(isNeedDeletedByServer == true)
                    continue;

                notOnSaleItemTableList.Add(itemTableData);
            }

            //排序非在售物品
            AuctionNewUtility.SortOnSaleItemTableList(notOnSaleItemTableList);

            //添加非在售物品的数据模型
            for (var i = 0; i < notOnSaleItemTableList.Count; i++)
            {
                var curItemTable = notOnSaleItemTableList[i];
                if (curItemTable == null)
                    continue;

                //物品在售，不添加
                var isItemOnSale = AuctionNewUtility.IsItemOnSale(curItemTable, auctionNewOnSaleItemList);
                if (isItemOnSale == true)
                    continue;

                var auctionItemBaseInfo = new AuctionItemBaseInfo
                {
                    itemTypeId = (uint)curItemTable.ID,
                    num = 0,
                    isTreas = (byte)curItemTable.IsTreas,
                };

                //不显示珍品，我要构面界面中所有的珍品重置为非珍品标志
                if (_mainTabType == AuctionNewMainTabType.AuctionBuyType
                    && _isShowTreasure == false)
                    auctionItemBaseInfo.isTreas = 0;

                _auctionNewItemResList.Add(auctionItemBaseInfo);
            }
        }

        private void OnOnlyShowOnSaleItemButtonClick(bool value)
        {
            //更新内容
            _onlyShowOnSaleItemFlag = value;
            BuyNoticeOnSaleSendReq();
        }

        private void OnOnlyShowTreasureItemButtonClick(bool value)
        {
            //更新内容
            _onlyShowTreasureItmFlag = value;
            BuyNoticeOnSaleSendReq();
        }
        #endregion

        //判断是否是同一层级，主要用于避免频繁点击二级按钮，可能造成的list为null情况
        private bool IsSameOnSaleLayer(AuctionNewMenuTabDataModel firstLayerMenuTabDataModel,
            AuctionNewMenuTabDataModel secondLayerMenuTabDataModel,
            AuctionNewMenuTabDataModel thirdLayerMenuTabDataModel,
            AuctionNewMainTabType auctionNewMainTabType = AuctionNewMainTabType.None)
        {
            if (auctionNewMainTabType != _mainTabType)
                return false;

            if ((thirdLayerMenuTabDataModel != null && _thirdLayerMenuDataModel == null)
                || (thirdLayerMenuTabDataModel == null && _thirdLayerMenuDataModel != null))
                return false;

            if (thirdLayerMenuTabDataModel != null && _thirdLayerMenuDataModel != null
                                                   && thirdLayerMenuTabDataModel.Id != _thirdLayerMenuDataModel.Id)
                return false;

            if ((secondLayerMenuTabDataModel != null && _secondLayerMenuDataModel == null)
                || (secondLayerMenuTabDataModel == null && _secondLayerMenuDataModel != null))
                return false;

            if (secondLayerMenuTabDataModel != null && _secondLayerMenuDataModel != null
                                                    && secondLayerMenuTabDataModel.Id != _secondLayerMenuDataModel.Id)
                return false;

            if ((firstLayerMenuTabDataModel != null && _firstLayerMenuDataModel == null)
                || (firstLayerMenuTabDataModel == null && _firstLayerMenuDataModel != null))
                return false;

            if (firstLayerMenuTabDataModel != null && _firstLayerMenuDataModel != null
                                                   && firstLayerMenuTabDataModel.Id != _firstLayerMenuDataModel.Id)
                return false;


            return true;
        }



    }
}