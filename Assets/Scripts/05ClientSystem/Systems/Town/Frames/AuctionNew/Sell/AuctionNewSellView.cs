using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;

namespace GameClient
{

    public enum ActionNewSellTabType
    {
        None = 0,
        AuctionNewSellEquipType,    //装备
        AuctionNewSellMaterialType, //材料
    }

    [SerializeField]
    public class AuctionNewSellTabDataModel
    {
        public int index;
        public ActionNewSellTabType sellTabType;
        public string sellTabName;
    }

    public class AuctionNewSellView : AuctionNewContentBaseView
    {

        private AuctionNewMainTabType _mainTabType;
        private int _defaultSellTabIndex = 0;
        private ActionNewSellTabType _sellTabType;

        private List<AuctionSellItemData> _packageForSellItemDataList;

        private List<AuctionNewSellTabDataModel> _sellTabDataModelList = new List<AuctionNewSellTabDataModel>();

        private List<AuctionNewOnShelfDataModel> _onSelfDataModelList = null;
        private AuctionNewUserData _auctionNewUserData = null;
        private ItemData _auctionNewUserItemData = null;

        private bool _isShowSellRecordButton = false;  //是否显示销售记录的按钮， true表示显示

        [Space(15)]
        [HeaderAttribute("ComUIList")]
        [Space(10)]
        [SerializeField] private ComUIListScript sellTitleTabItemList;
        [SerializeField] private ComUIListScriptEx forSellItemList;
        [SerializeField] private ComUIListScriptEx onShelfItemList;


        [Space(10)] [HeaderAttribute("Other")] [SerializeField]
        private Button sellRecordButton;

        [SerializeField] private Text onShelfLabel;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
        }

        private void BindEvents()
        {
            if (sellRecordButton != null)
            {
                sellRecordButton.onClick.RemoveAllListeners();
                sellRecordButton.onClick.AddListener(OnSellRecordButtonClick);
            }

            if (sellTitleTabItemList != null)
            {
                sellTitleTabItemList.Initialize();
                sellTitleTabItemList.onItemVisiable += OnSellTitleTabVisible;
            }

            if (forSellItemList != null)
            {
                forSellItemList.Initialize();
                forSellItemList.onItemVisiable += OnForSellItemVisible;
                forSellItemList.OnItemRecycle += OnForSellItemRecycle;
            }

            if (onShelfItemList != null)
            {
                onShelfItemList.Initialize();
                onShelfItemList.onItemVisiable += OnShelfItemVisible;
                onShelfItemList.OnItemRecycle += OnShelfItemRecycle;
            }

        }

        private void UnBindEvents()
        {
            if(sellRecordButton != null)
                sellRecordButton.onClick.RemoveAllListeners();

            if (sellTitleTabItemList != null)
                sellTitleTabItemList.onItemVisiable -= OnSellTitleTabVisible;

            if (forSellItemList != null)
            {
                forSellItemList.onItemVisiable -= OnForSellItemVisible;
                forSellItemList.OnItemRecycle -= OnForSellItemRecycle;
            }

            if (onShelfItemList != null)
            {
                onShelfItemList.onItemVisiable -= OnShelfItemVisible;
                onShelfItemList.OnItemRecycle -= OnShelfItemRecycle;
            }
        }

        private void ResetData()
        {
            _auctionNewUserData = null;
            _auctionNewUserItemData = null;
        }

        private void OnEnable()
        {
            //刷新自己上架道具的数据
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewReceiveSelfListResSucceed, OnReceiveSelfListSucceed);
            //刷新购买架子的数据
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewBuyShelfResSucceed, OnBuyShelfSucceed);

            //背包中添加一个新的道具，刷新待售道具列表
            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.OnItemInPackageAddedMessage, OnItemInPackageAddedMessage);
            //道具删除，刷新待售道具列表
            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.OnItemInPackageRemovedMessage, OnItemInPackageRemovedMessage);
            //道具的属性更改的时候，刷新道具列表
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemPropertyChanged, OnItemPropertyChanged);
            //铭文镶嵌成功，刷新道具列表
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnItemEquipInscriptionSucceed, OnItemEquipInscriptionSucceed);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewReceiveSelfListResSucceed, OnReceiveSelfListSucceed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewBuyShelfResSucceed, OnBuyShelfSucceed);

            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.OnItemInPackageAddedMessage, OnItemInPackageAddedMessage);
            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.OnItemInPackageRemovedMessage, OnItemInPackageRemovedMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemPropertyChanged, OnItemPropertyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnItemEquipInscriptionSucceed, OnItemEquipInscriptionSucceed);
            ResetData();
        }

        public override void InitView(AuctionNewMainTabType mainTabType, 
            AuctionNewUserData auctionNewUserData = null)
        {
            _mainTabType = mainTabType;
            _auctionNewUserData = auctionNewUserData;

            //AuctionNewDataManager.GetInstance().UpdatePackageItemUid();

            InitSellBaseView();

            AuctionNewDataManager.GetInstance().SendSelfAuctionListRequest();
        }

        private void InitTabDataModelList()
        {
            if (_sellTabDataModelList == null)
            {
                _sellTabDataModelList = new List<AuctionNewSellTabDataModel>();
            }

            _sellTabDataModelList.Clear();
            var firstTabDataModel = new AuctionNewSellTabDataModel();
            firstTabDataModel.index = 1;
            firstTabDataModel.sellTabType = ActionNewSellTabType.AuctionNewSellEquipType;
            firstTabDataModel.sellTabName = "auction_new_sell_equip_type";

            var secondTabDataModel = new AuctionNewSellTabDataModel();
            secondTabDataModel.index = 2;
            secondTabDataModel.sellTabType = ActionNewSellTabType.AuctionNewSellMaterialType;
            secondTabDataModel.sellTabName = "auction_new_sell_cost_material";

            _sellTabDataModelList.Add(firstTabDataModel);
            _sellTabDataModelList.Add(secondTabDataModel);

        }

        private void InitSellBaseView()
        {

            InitTabDataModelList();

            var sellTabTitleNumber = _sellTabDataModelList.Count;
            if(sellTabTitleNumber <= 0)
                return;

            SetDefaultSellTabIndex();

            if (sellTitleTabItemList != null)
                sellTitleTabItemList.SetElementAmount(sellTabTitleNumber);

            //是否显示寄售记录的按钮，默认显示，数值由服务器同步
            _isShowSellRecordButton = true;

            //珍品机制没有打开，不显示recordButton
            if (AuctionNewUtility.IsAuctionTreasureItemOpen() == false)
                _isShowSellRecordButton = false;
            
            sellRecordButton.gameObject.CustomActive(_isShowSellRecordButton);
        }

        private void SetDefaultSellTabIndex()
        {
            _defaultSellTabIndex = 0;
            if (_auctionNewUserData == null)
                return;

            _auctionNewUserItemData = ItemDataManager.GetInstance().GetItem(_auctionNewUserData.ItemLinkId);
            if (_auctionNewUserItemData == null)
                return;

            var defaultTabType = ActionNewSellTabType.AuctionNewSellEquipType;
            if (AuctionNewUtility.IsEquipItems(_auctionNewUserItemData) == false)
                defaultTabType = ActionNewSellTabType.AuctionNewSellMaterialType;

            var sellTabTitleNumber = _sellTabDataModelList.Count;
            if (sellTabTitleNumber <= 0)
                return;

            for (var i = 0; i < _sellTabDataModelList.Count; i++)
            {
                if (_sellTabDataModelList[i] != null && _sellTabDataModelList[i].sellTabType == defaultTabType)
                    _defaultSellTabIndex = i;
            }
        }

        public override void OnEnableView(AuctionNewMainTabType mainTabType)
        {
            //更新ForSaleItemList
            if (forSellItemList != null)
                forSellItemList.ResetComUiListScriptEx();
            UpdateForSaleItemList();

            //更新OnSaleList
            AuctionNewDataManager.GetInstance().SendSelfAuctionListRequest();
        }

        #region SellTitle
        private void OnSellTitleTabVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            var sellTitleTabItem = item.GetComponent<AuctionNewSellTitleTabItem>();
            if(sellTitleTabItem == null)
                return;

            if (_sellTabDataModelList != null
                && item.m_index >= 0 && item.m_index < _sellTabDataModelList.Count)
            {
                var sellTabDataModel = _sellTabDataModelList[item.m_index];
                var isSelected = _defaultSellTabIndex == item.m_index;

                if (sellTabDataModel != null)
                    sellTitleTabItem.Init(sellTabDataModel, OnTitleTabClick, isSelected);
            }
        }

        private void OnTitleTabClick(ActionNewSellTabType sellTabType)
        {
            if (sellTabType != ActionNewSellTabType.AuctionNewSellEquipType
                && sellTabType != ActionNewSellTabType.AuctionNewSellMaterialType)
            {
                Logger.LogErrorFormat("Error TabType and AuctionNewSellTabType is {0}", sellTabType.ToString());
                return;
            }

            if(_sellTabType == sellTabType)
                return;

            _sellTabType = sellTabType;

            //停止滑动和位置重置
            if(forSellItemList != null)
                forSellItemList.ResetComUiListScriptEx();

            UpdateForSaleItemList();
        }
        #endregion

        #region ForSellItem
        private void UpdateForSaleItemList()
        {
            //获得数据，并展示
            _packageForSellItemDataList =
                AuctionNewDataManager.GetInstance().GetAuctionSellItemDataByType(_sellTabType);

            if (forSellItemList != null)
            {
                //forSellItemList.ResetContentPosition();
                forSellItemList.SetElementAmount(_packageForSellItemDataList.Count);
            }
        }

        private void OnForSellItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            if(_packageForSellItemDataList == null)
                return;

            if(item.m_index < 0 || item.m_index >= _packageForSellItemDataList.Count)
                return;

            var sellItemData = _packageForSellItemDataList[item.m_index];
            var forSellElementItem = item.GetComponent<AuctionNewForSellElementItem>();

            if (sellItemData != null && forSellElementItem != null)
                forSellElementItem.InitItem(sellItemData);
        }

        private void OnForSellItemRecycle(ComUIListElementScript item)
        {
            if(item == null)
                return;

            var forSellElementItem = item.GetComponent<AuctionNewForSellElementItem>();
            if(forSellElementItem != null)
                forSellElementItem.OnItemRecycle();
        }
        #endregion

        #region OnShelfItem
        
        //第一次打开的时候，根据传进来的参数，进行打开上架页面，或者解封提示，之后数据设置为null
        private void ShowAuctionSellFrame()
        {
            if (_auctionNewUserData == null)
            {
                _auctionNewUserItemData = null;
                return;
            }

            AuctionNewUtility.CloseAuctionNewOnShelfFrame();

            _auctionNewUserItemData = ItemDataManager.GetInstance().GetItem(_auctionNewUserData.ItemLinkId);
            if (_auctionNewUserItemData == null)
            {
                _auctionNewUserData = null;
                return;
            }

            if (_auctionNewUserItemData.IsNeedPacking() == true)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("Auction_item_packing"), ClickOkPacking);
            }
            else
            {
                AuctionNewUtility.OpenAuctionNewOnShelfFrame(_auctionNewUserData.ItemLinkId,
                    _auctionNewUserItemData.IsTreasure,
                    AuctionNewDataManager.GetInstance().BaseShelfNum + PlayerBaseData.GetInstance().AddAuctionFieldsNum,
                    AuctionNewDataManager.GetInstance().OnShelfItemNumber);

                _auctionNewUserItemData = null;
            }
            _auctionNewUserData = null;
        }

        private void ClickOkPacking()
        {
            AuctionNewDataManager.GetInstance().OnClickOnPacking(_auctionNewUserItemData);
            _auctionNewUserItemData = null;
        }

        private void OnShelfItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            if(_onSelfDataModelList == null)
                return;

            if(item.m_index < 0 || item.m_index >= _onSelfDataModelList.Count)
                return;

            var onShelfElementItem = item.GetComponent<AuctionNewOnShelfElementItem>();
            var onShelfDataModel = _onSelfDataModelList[item.m_index];

            if (onShelfElementItem != null && onShelfDataModel != null)
                onShelfElementItem.InitItem(onShelfDataModel);

        }

        private void OnShelfItemRecycle(ComUIListElementScript item)
        {
            if(item == null)
                return;

            var onShelfElementItem = item.GetComponent<AuctionNewOnShelfElementItem>();
            if(onShelfElementItem != null)
                onShelfElementItem.OnItemRecycle();

        }
        #endregion

        #region UIEvent

        //购买架子成功，进行刷新
        private void OnBuyShelfSucceed(UIEvent uiEvent)
        {
            _onSelfDataModelList = AuctionNewDataManager.GetInstance().GetOnShelfDataModelList();
            if (_onSelfDataModelList != null)
            {
                if (onShelfItemList != null)
                    onShelfItemList.SetElementAmount(_onSelfDataModelList.Count);
            }
            UpdateOnShelfTitle();
        }

        //请求自己上架的物品成功，刷新自己上架的物品
        private void OnReceiveSelfListSucceed(UIEvent uiEvent)
        {
            _onSelfDataModelList = AuctionNewDataManager.GetInstance().GetOnShelfDataModelList();

            if (_onSelfDataModelList != null)
            {
                if (onShelfItemList != null)
                    onShelfItemList.SetElementAmount(_onSelfDataModelList.Count);
            }

            //UpdateForSaleItemList();

            UpdateOnShelfTitle();

            //如果是超链接直接进入到寄售页面的话，可能会展示拍卖行的销售页面
            ShowAuctionSellFrame();
        }

        //铭文镶嵌成功，对拍卖行在售界面进行刷新
        private void OnItemEquipInscriptionSucceed(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            //装备铭文的Guid
            var itemGuid = (ulong) uiEvent.Param1;

            var isItemInForSaleList = IsItemInForSaleItemList(itemGuid);

            //当前待售的架子上不存在，不用刷新
            if (isItemInForSaleList == false)
                return;

            UpdateForSaleItemList();
        }

        //属性改变
        private void OnItemPropertyChanged(UIEvent uiEvent)
        {
            ItemData itemData = (ItemData) uiEvent.Param1;
            EItemProperty prop = (EItemProperty)uiEvent.Param2;

            if (prop == EItemProperty.EP_SEAL_STATE
                || prop == EItemProperty.EP_IS_TREAS
                || prop == EItemProperty.EA_AUCTION_TRANS_NUM)
            {
                if (itemData == null)
                    return;
                
                UpdateForSaleItemList();
            }

            //道具的数量发生改变
            if (prop == EItemProperty.EP_NUM)
            {
                if (itemData == null)
                    return;

                //道具不在待售的架子上，不需要刷新
                var isItemInForSaleItemList = IsItemInForSaleItemList((ulong) itemData.GUID);
                if (isItemInForSaleItemList == false)
                    return;

                //数量改变，进行刷新道具
                UpdateForSaleItemList();
            }

            //道具的Pack属性发生改变
            if (prop == EItemProperty.EP_PACK)
            {
                UpdateForSaleItemList();
            }

        }

        //背包中的道具删除之后，进行刷新
        private void OnItemInPackageRemovedMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            //删除的道具是否在架子上
            var removeItemGuid = (ulong) uiEvent.Param1;
            var isItemInForSaleItemList = IsItemInForSaleItemList(removeItemGuid);
            if (isItemInForSaleItemList == false)
                return;

            //在架子上，刷新架子
            UpdateForSaleItemList();
        }

        //背包中添加一个道具，进行刷新
        private void OnItemInPackageAddedMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var itemGuid = (ulong) uiEvent.Param1;

            //判断新增加的物品是否可以展示当前的架子上
            var isItemCanShowInForSaleList = AuctionNewDataManager.GetInstance().IsPackageItemCanInForSaleList(
                itemGuid,
                _sellTabType);
            if (isItemCanShowInForSaleList == false)
                return;

            //如果新增物品可以展示，则刷新架子
            UpdateForSaleItemList();
        }

        #endregion


        private void UpdateOnShelfTitle()
        {
            if (onShelfLabel != null)
            {
                var onShelfItemNumber = AuctionNewDataManager.GetInstance().OnShelfItemNumber;
                var maxShelfNumber = PlayerBaseData.GetInstance().AddAuctionFieldsNum
                                     + AuctionNewDataManager.GetInstance().BaseShelfNum;
                onShelfLabel.text = string.Format(TR.Value("auction_new_sell_item_auction_number"),
                    onShelfItemNumber, maxShelfNumber);
            }
        }

        private void OnSellRecordButtonClick()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewSellRecordFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<AuctionNewSellRecordFrame>();
            ClientSystemManager.GetInstance().OpenFrame<AuctionNewSellRecordFrame>(FrameLayer.Middle);
        }


        #region Helper
        //判断某个道具，是否在当前的架子上
        private bool IsItemInForSaleItemList(ulong itemGuid)
        {
            //架子的数据为null
            if (_packageForSellItemDataList == null || _packageForSellItemDataList.Count <= 0)
                return false;

            //展示的在售道具中是否存在相应的道具
            var isFind = false;
            for (var i = 0; i < _packageForSellItemDataList.Count; i++)
            {
                var sellItemData = _packageForSellItemDataList[i];
                if (sellItemData == null)
                    continue;

                //存在相应的道具
                if (sellItemData.uId == itemGuid)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

    }
}