using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public delegate void OnFirstLayerTabToggleClick(AuctionNewMenuTabDataModel auctionNewMenuTabDataModel);
    public delegate void OnSecondLayerTabToggleClick(AuctionNewMenuTabDataModel parentNewMenuTabDataModel,AuctionNewMenuTabDataModel auctionNewMenuTabDataModel);

    public delegate void OnUpdateFilterBackground(bool isHaveFilter);

    public class AuctionNewBuyNoticeView : AuctionNewContentBaseView
    {

        private AuctionItemBaseInfo _auctionItemBaseInfo = null;

        private AuctionNewMainTabType _curMainTabType = AuctionNewMainTabType.None;
        private AuctionNewMenuTabDataModel _firstLayerMenuTabDataModel = null;
        private AuctionNewMenuTabDataModel _secondLayerMenuTabDataModel = null;
        private AuctionNewMenuTabDataModel _thirdLayerMenuTabDataModel = null;

        private AuctionNewBuyNoticeTypeController _buyNoticeTypeController;
        private AuctionNewBuyNoticeOnSaleController _buyNoticeOnSaleController;
        private AuctionNewBuyNoticeDetailController _buyNoticeDetailController;

        private int _defaultFirstLayerTabId;
        private int _defaultSecondLayerTabId;
        private AuctionNewUserData _auctionNewUserData;

        private List<AuctionNewMenuTabDataModel> _auctionNewFirstLayerTabDataModelList;

        [Space(10)]
        [HeaderAttribute("BuyNoticeContent")]
        [Space(5)]
        [SerializeField] private GameObject buyNoticeTypeRoot;
        [SerializeField] private GameObject buyNoticeOnSaleRoot;
        [SerializeField] private GameObject buyNoticeDetailRoot;
        
        [Space(10)]
        [HeaderAttribute("MenuTabs")]
        [Space(5)]
        [SerializeField] private GameObject menuTabContent;
        [SerializeField] private GameObject menuItemGroup;

        [Space(15)] [HeaderAttribute("FilterBackground")] [Space(5)]
        [SerializeField] private GameObject noFilterBackground;
        [SerializeField] private GameObject haveFilterBackground;

        private void OnDestroy()
        {
            ResetData();
        }

        private void ResetData()
        {
            _auctionItemBaseInfo = null;

            _curMainTabType = AuctionNewMainTabType.None;

            _firstLayerMenuTabDataModel = null;
            _secondLayerMenuTabDataModel = null;
            _thirdLayerMenuTabDataModel = null;
            _buyNoticeTypeController = null;
            _buyNoticeOnSaleController = null;
            _buyNoticeDetailController = null;

            _auctionNewUserData = null;
            _defaultFirstLayerTabId = 0;
            _defaultSecondLayerTabId = 0;

            _auctionNewFirstLayerTabDataModelList = null;
        }

        #region InitView
        //只是初始化所需要的表格数据
        public override void InitView(AuctionNewMainTabType mainTabType, AuctionNewUserData auctionNewUserData = null)
        {
            _curMainTabType = mainTabType;
            _auctionNewUserData = auctionNewUserData;

            //第一层级的DataModelList; 购买界面：由表格决定；公示界面：由表格和服务器共同决定
            _auctionNewFirstLayerTabDataModelList =
                AuctionNewUtility.GetAuctionNewFirstLayerTabDataModelList(_curMainTabType);

            if (_auctionNewFirstLayerTabDataModelList == null || _auctionNewFirstLayerTabDataModelList.Count <= 0)
            {
                Logger.LogErrorFormat("First Layer TabDataModel is null or count is 0");
                return;
            }

            InitBuyNoticeDefaultData();
            InitBuyNoticeView();
        }

        private void InitBuyNoticeDefaultData()
        {
            UpdateDefaultFirstLayerTabId();
        }

        private void UpdateDefaultFirstLayerTabId()
        {
            //默认选中第一个
            _defaultFirstLayerTabId = _auctionNewFirstLayerTabDataModelList[0].Id;
            _defaultSecondLayerTabId = 0;

            //使用传过来的参数
            if (_auctionNewUserData != null)
            {
                //第一层级的默认选中
                if (_auctionNewUserData.FirstLayerTabId > 0)
                {
                    //在list中查找，查看是否传进来的参数有效
                    for (var i = 0; i < _auctionNewFirstLayerTabDataModelList.Count; i++)
                    {
                        if (_auctionNewFirstLayerTabDataModelList[i].Id == _auctionNewUserData.FirstLayerTabId)
                        {
                            _defaultFirstLayerTabId = _auctionNewUserData.FirstLayerTabId;
                            break;
                        }
                    }
                }

                //第二层级的默认选中
                if (_auctionNewUserData.SecondLayerTabId > 0)
                    _defaultSecondLayerTabId = _auctionNewUserData.SecondLayerTabId;

            }
        }


        private void InitBuyNoticeView()
        {
            if (menuTabContent == null || menuItemGroup == null)
            {
                Logger.LogErrorFormat("Item is null ");
                return;
            }

            for (var i = 0; i < _auctionNewFirstLayerTabDataModelList.Count; i++)
            {
                var curFirstLayerTabDataModel = _auctionNewFirstLayerTabDataModelList[i];

                var isSelected = _defaultFirstLayerTabId == curFirstLayerTabDataModel.Id;
                //第一层级下的第二层级的DataModelList; 购买界面：由表格决定；关注界面：由表格和服务器共同决定
                var secondLayerTabDataModelList = AuctionNewUtility.GetAuctionNewChildrenLayerTabDataModelList(
                    curFirstLayerTabDataModel,
                    _curMainTabType);

                var curMenuItemGroup = Instantiate(menuItemGroup) as GameObject;
                if (curMenuItemGroup != null)
                {
                    curMenuItemGroup.CustomActive(true);
                    Utility.AttachTo(curMenuItemGroup, menuTabContent);

                    var auctionNewMenuItemGroup = curMenuItemGroup.GetComponent<AuctionNewMenuItemGroup>();
                    if (auctionNewMenuItemGroup != null
                        && auctionNewMenuItemGroup.firstLayerTabItem != null)
                    {
                        auctionNewMenuItemGroup.firstLayerTabItem.InitTabItem(curFirstLayerTabDataModel,
                            secondLayerTabDataModelList,
                            isSelected,
                            _defaultSecondLayerTabId,
                            OnFirstLayerTabToggleClick,
                            OnSecondLayerTabToggleClick);
                    }
                }
            }
        }


        public override void OnEnableView(AuctionNewMainTabType mainTabType)
        {
            //if(_curMainTabType == mainTabType)
            //    return;

            _curMainTabType = mainTabType;

            UpdateBuyNoticeController();
        }

        public override void ResetViewType()
        {
            _curMainTabType = AuctionNewMainTabType.None;
        }

        //更新购买和展示页面
        private void UpdateBuyNoticeController()
        {
            //保存当前选中的第一层和第二层的ID
            SetLastTimeUserData();

            if (_buyNoticeTypeController != null && _buyNoticeTypeController.gameObject.activeInHierarchy == true)
            {
                _buyNoticeTypeController.OnEnableTypeController(_secondLayerMenuTabDataModel, _curMainTabType);
                return;
            }

            if (_buyNoticeOnSaleController != null && _buyNoticeOnSaleController.gameObject.activeInHierarchy == true)
            {
                _buyNoticeOnSaleController.OnEnableOnSaleController(_firstLayerMenuTabDataModel,
                    _secondLayerMenuTabDataModel,
                    _thirdLayerMenuTabDataModel,
                    _curMainTabType);
                return;
            }

            if (_buyNoticeDetailController != null && _buyNoticeDetailController.gameObject.activeInHierarchy == true)
            {
                //关注类型
                if (_firstLayerMenuTabDataModel != null
                    && _firstLayerMenuTabDataModel.AuctionNewFrameTable != null
                    && _firstLayerMenuTabDataModel.AuctionNewFrameTable.MenuType ==
                    AuctionNewFrameTable.eMenuType.Auction_Menu_Notice)
                {
                    //关注类型
                    _buyNoticeDetailController.OnEnableDetailController(_curMainTabType, true);
                }
                else
                {
                    _buyNoticeDetailController.OnEnableDetailController(_curMainTabType,
                        false, _auctionItemBaseInfo);
                }
            }
        }

        private void SetLastTimeUserData()
        {
            if (_firstLayerMenuTabDataModel != null)
                AuctionNewDataManager.GetInstance().SetLastTimeUserDataFirstLayerTabId(_firstLayerMenuTabDataModel.Id);

            if (_secondLayerMenuTabDataModel != null)
                AuctionNewDataManager.GetInstance()
                    .SetLastTimeUserDataSecondLayerTabId(_secondLayerMenuTabDataModel.Id);
        }

        #endregion

        #region TabClick
        //第一层级按钮点击
        private void OnFirstLayerTabToggleClick(AuctionNewMenuTabDataModel auctionNewMenuTabDataModel)
        {
            _firstLayerMenuTabDataModel = auctionNewMenuTabDataModel;

            if (_firstLayerMenuTabDataModel != null && _firstLayerMenuTabDataModel.AuctionNewFrameTable != null)
            {
                //特殊情况：关注按钮
                if (_firstLayerMenuTabDataModel.AuctionNewFrameTable.MenuType ==
                    AuctionNewFrameTable.eMenuType.Auction_Menu_Notice)
                {
                    OnShowBuyNoticeDetailContent(true);

                    _secondLayerMenuTabDataModel = null;
                    _thirdLayerMenuTabDataModel = null;
                    _auctionItemBaseInfo = null;
                    AuctionNewDataManager.GetInstance().SetLastTimeUserDataSecondLayerTabId(0);

                }

                AuctionNewDataManager.GetInstance().SetLastTimeUserDataFirstLayerTabId(_firstLayerMenuTabDataModel.Id);

            }
            else
            {
                Logger.LogErrorFormat("firstLayerMenuTabModel is null or auctionNewFrameTable is null");
            }

        }

        //第二层级按钮点击, 三级页签和OnSaleItem重置
        private void OnSecondLayerTabToggleClick(AuctionNewMenuTabDataModel parentTabDataModel,AuctionNewMenuTabDataModel auctionNewMenuTabDataModel)
        {
            //if(_firstLayerMenuTabDataModel == parentTabDataModel && _secondLayerMenuTabDataModel == auctionNewMenuTabDataModel)
            //    return;

            _firstLayerMenuTabDataModel = parentTabDataModel;
            _secondLayerMenuTabDataModel = auctionNewMenuTabDataModel;
            _thirdLayerMenuTabDataModel = null;
            _auctionItemBaseInfo = null;

            if (_firstLayerMenuTabDataModel == null || _firstLayerMenuTabDataModel.AuctionNewFrameTable == null
                                                    || _secondLayerMenuTabDataModel == null ||
                                                    _secondLayerMenuTabDataModel.AuctionNewFrameTable == null)
            {
                Logger.LogError("OnSecondLayerTabToggleClick is Error");
                return;
            }

            if (_secondLayerMenuTabDataModel.IsOwnerChildren == true)
            {
                OnShowBuyNoticeTypeContent();
            }
            else
            {
                OnShowBuyNoticeOnSaleContent();
            }

            AuctionNewDataManager.GetInstance().SetLastTimeUserDataSecondLayerTabId(_secondLayerMenuTabDataModel.Id);
        }

        #endregion

        #region BuyNoticeContent

        //显示Type类型的内容
        private void OnShowBuyNoticeTypeContent()
        {

            ResetBuyNoticeRoot();

            //类型界面
            if (buyNoticeTypeRoot == null)
            {
                Logger.LogErrorFormat("BuyNoticeView buyNoticeTypeRoot is null");
                return;
            }

            if (buyNoticeTypeRoot.activeSelf == false)
                buyNoticeTypeRoot.CustomActive(true);

            //初始化
            if (_buyNoticeTypeController == null)
            {
                _buyNoticeTypeController =
                    LoadBuyNoticeBaseController(buyNoticeTypeRoot) as AuctionNewBuyNoticeTypeController;
                if (_buyNoticeTypeController != null)
                    _buyNoticeTypeController.InitTypeControllerData(OnBuyNoticeTypeItemClick,
                        OnUpdateFilterBackground);
            }

            if (_buyNoticeTypeController != null)
            {
                _buyNoticeTypeController.OnEnableTypeController(_secondLayerMenuTabDataModel,
                    _curMainTabType);
            }
        }

        //显示在售商品的类型
        private void OnShowBuyNoticeOnSaleContent()
        {

            ResetBuyNoticeRoot();

            if (buyNoticeOnSaleRoot == null)
            {
                Logger.LogErrorFormat("BuyNoticeView buyNoticeOnSaleRoot is null");
                return;
            }

            //在售页面
            if (buyNoticeOnSaleRoot.activeSelf == false)
                buyNoticeOnSaleRoot.CustomActive(true);

            //初始化
            if (_buyNoticeOnSaleController == null)
            {
                _buyNoticeOnSaleController =
                    LoadBuyNoticeBaseController(buyNoticeOnSaleRoot) as AuctionNewBuyNoticeOnSaleController;
                if (_buyNoticeOnSaleController != null)
                    _buyNoticeOnSaleController.InitOnSaleController(OnBuyNoticeOnSaleItemClick,
                        OnUpdateFilterBackground,
                        _curMainTabType);
            }

            if (_buyNoticeOnSaleController != null)
            {
                _buyNoticeOnSaleController.OnEnableOnSaleController(_firstLayerMenuTabDataModel,
                    _secondLayerMenuTabDataModel,
                    _thirdLayerMenuTabDataModel,
                    _curMainTabType);
            }
        }

        //显示商品价格详情的内容
        private void OnShowBuyNoticeDetailContent(bool isNoticeTab = false)
        {

            ResetBuyNoticeRoot();

            //详情界面
            if (buyNoticeDetailRoot == null)
            {
                Logger.LogErrorFormat("BuyNoticeView buyNoticeDetailRoot is null");
                return;
            }

            if (buyNoticeDetailRoot.activeSelf == false)
                buyNoticeDetailRoot.CustomActive(true);
            //进行初始化
            if (_buyNoticeDetailController == null)
            {
                _buyNoticeDetailController =
                    LoadBuyNoticeBaseController(buyNoticeDetailRoot) as AuctionNewBuyNoticeDetailController;
                if (_buyNoticeDetailController != null)
                    _buyNoticeDetailController.InitDetailController(OnUpdateFilterBackground,
                        OnReturnButtonClick);
            }

            if (_buyNoticeDetailController != null)
            {
                //关注页签
                if (isNoticeTab == true)
                {
                    _buyNoticeDetailController.OnEnableDetailController(_curMainTabType, true);
                }
                else
                {
                    _buyNoticeDetailController.OnEnableDetailController(_curMainTabType,
                        false, _auctionItemBaseInfo,
                        true);
                }
            }
        }

        #endregion

        #region Helper

        private void ResetBuyNoticeRoot()
        {
            if (buyNoticeOnSaleRoot != null)
                buyNoticeOnSaleRoot.CustomActive(false);

            if (buyNoticeTypeRoot != null)
                buyNoticeTypeRoot.CustomActive(false);

            if (buyNoticeDetailRoot != null)
                buyNoticeDetailRoot.CustomActive(false);
        }

        //加载界面
        private AuctionNewBuyNoticeBaseController LoadBuyNoticeBaseController(GameObject contentRoot)
        {

            AuctionNewBuyNoticeBaseController buyNoticeBaseController = null;

            var uiPrefabWrapper = contentRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                var uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(contentRoot.transform, false);

                    buyNoticeBaseController = uiPrefab.GetComponent<AuctionNewBuyNoticeBaseController>();
                }
            }

            return buyNoticeBaseController;
        }

        #endregion

        #region buyNoticeClick
        //Type ItemClick
        private void OnBuyNoticeTypeItemClick(AuctionNewMenuTabDataModel auctionNewMenuTabDataModel)
        {
            _thirdLayerMenuTabDataModel = auctionNewMenuTabDataModel;
            _auctionItemBaseInfo = null;

            //显示在售的页面
            OnShowBuyNoticeOnSaleContent();
        }

        //OnSaleItemClick
        private void OnBuyNoticeOnSaleItemClick(AuctionItemBaseInfo auctionItemBaseInfo)
        {
            _auctionItemBaseInfo = auctionItemBaseInfo;
            OnShowBuyNoticeDetailContent();
        }

        //detail页面中的返回按钮
        private void OnReturnButtonClick()
        {
            _auctionItemBaseInfo = null;
            //显示在售的页面
            OnShowBuyNoticeOnSaleContent();
        }


        //FilterClick
        private void OnUpdateFilterBackground(bool isHaveFilter)
        {
            //存在过滤器
            if (isHaveFilter == true)
            {
                if (noFilterBackground != null)
                    noFilterBackground.gameObject.CustomActive(false);

                if (haveFilterBackground != null)
                    haveFilterBackground.gameObject.CustomActive(true);
            }
            else
            {
                //不存在过滤器
                if (haveFilterBackground != null)
                    haveFilterBackground.gameObject.CustomActive(false);

                if (noFilterBackground != null)
                    noFilterBackground.gameObject.CustomActive(true);
            }
        }
        #endregion

    }
}