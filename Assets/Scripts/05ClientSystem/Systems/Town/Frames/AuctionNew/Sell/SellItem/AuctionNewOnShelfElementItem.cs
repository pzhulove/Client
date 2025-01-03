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

    public enum AuctionNewOnShelfItemState
    {
        None = 0,
        OnNoticeState,          //公示状态
        PrepareShelfState,      //待上架状态
        OnShelfState,           //上架状态
        TimeOverState,          //过期状态
    }

    public class AuctionNewOnShelfElementItem : MonoBehaviour
    {
        private AuctionNewOnShelfDataModel _onShelfDataModel;

        private ComItem _elementComItem;

        private float _timeInterval = 0.0f;
        private AuctionNewOnShelfItemState _onShelfItemState = AuctionNewOnShelfItemState.None;


        [Space(15)] [HeaderAttribute("EmptyRoot")]
        [Space(5)]
        [SerializeField] private GameObject emptyRoot;

        [Space(15)] [HeaderAttribute("buyFieldRoot")] [Space(5)]
        [SerializeField] private GameObject buyFieldRoot;

        [SerializeField] private Text costMoneyText;
        [SerializeField] private Image costIcon;
        [SerializeField] private Button buyShelfButton;

        [Space(15)] [HeaderAttribute("SellItemRoot")] [Space(5)]
        [SerializeField] private GameObject sellItemRoot;

        [SerializeField] private GameObject itemRoot;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemScore;
        [SerializeField] private Text priceValue;
        [SerializeField] private Button downShelfItemButton;

        [Space(10)]
        [HeaderAttribute("ItemTime")]
        [Space(10)]
        [SerializeField] private GameObject itemTimeRoot;
        [SerializeField] private Text itemLeftTimeText;
        [SerializeField] private Text itemTimeInvalidText;

        [Space(15)] [HeaderAttribute("ItemState")] [Space(10)] [SerializeField]
        private GameObject itemStateRoot;
        [SerializeField] private GameObject noticeStateFlag;
        [SerializeField] private GameObject prepareShelfStateFlag;
        [SerializeField] private GameObject onShelfStateFlag;
        [SerializeField] private GameObject timeOverStateFlag;
        [SerializeField] private UIGray itemUiGray;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            if (_elementComItem != null)
            {
                ComItemManager.Destroy(_elementComItem);
                _elementComItem = null;
            }

            ResetData();
            UnBindEvents();
        }

        private void BindEvents()
        {
            if (buyShelfButton != null)
                buyShelfButton.onClick.AddListener(OnBuyShelfButtonClick);

            if (downShelfItemButton != null)
                downShelfItemButton.onClick.AddListener(OnDownShelfItemClick);
        }

        private void UnBindEvents()
        {
            if(buyShelfButton != null)
                buyShelfButton.onClick.RemoveAllListeners();

            if(downShelfItemButton != null)
                downShelfItemButton.onClick.RemoveAllListeners();
        }

        private void ResetData()
        {
            _onShelfDataModel = null;
            _timeInterval = 0.0f;
            _onShelfItemState = AuctionNewOnShelfItemState.None;
        }

        public void InitItem(AuctionNewOnShelfDataModel onShelfDataModel)
        {

            ResetItemRoot();
            _onShelfDataModel = onShelfDataModel;

            if (_onShelfDataModel == null)
            {
                Logger.LogErrorFormat("The onShelfDataModel is null");
                return;
            }

            InitItemView();

        }

        private void ResetItemRoot()
        {
            if (emptyRoot != null)
                emptyRoot.CustomActive(false);

            if (buyFieldRoot != null)
                buyFieldRoot.CustomActive(false);

            if (sellItemRoot != null)
                sellItemRoot.CustomActive(false);
        }

        private void InitItemView()
        {
            switch (_onShelfDataModel.onShelfState)
            {
                case AuctionNewOnShelfState.Empty:
                    InitEmptyItem();
                    return;
                case AuctionNewOnShelfState.BuyField:
                    InitBuyFieldItem();
                    return;
                case AuctionNewOnShelfState.OwnerItem:
                    InitOnShelfItem();
                    return;
            }
        }

        private void InitEmptyItem()
        {
            if (emptyRoot != null)
                emptyRoot.CustomActive(true);
        }

        #region BuyFieldItem
        private void InitBuyFieldItem()
        {
            if (_onShelfDataModel.boothTableData == null)
            {
                Logger.LogErrorFormat("BoothTableData is null");
                return;
            }

            if (costMoneyText != null)
            {
                costMoneyText.text = Utility.GetShowPrice((ulong) _onShelfDataModel.boothTableData.Num);
            }

            if (costIcon != null)
            {
                ETCImageLoader.LoadSprite(ref costIcon, ItemDataManager.GetInstance().GetOwnedItemIconPath(
                    _onShelfDataModel.boothTableData.CostItemID));
            }
            if (buyFieldRoot != null)
                buyFieldRoot.CustomActive(true);
        }

        private void OnBuyShelfButtonClick()
        {
            if(_onShelfDataModel == null || _onShelfDataModel.boothTableData == null)
                return;

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            SystemNotifyManager.SysNotifyMsgBoxOkCancel(string.Format(TR.Value("auction_new_onSale_buy_shelf_cost"), 
               _onShelfDataModel.boothTableData.Num, 
                ItemDataManager.GetInstance().GetOwnedItemName(_onShelfDataModel.boothTableData.CostItemID)),
                OnBuyShelfOk);
        }

        private void OnBuyShelfOk()
        {
            if (AuctionNewDataManager.GetInstance().IsAuctionNewCanBuyShelfField() == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_onSale_reach_max_field"));
                return;
            }

            //空判断
            if (_onShelfDataModel == null || _onShelfDataModel.boothTableData == null)
                return;

            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            costInfo.nMoneyID = _onShelfDataModel.boothTableData.CostItemID;
            costInfo.nCount = _onShelfDataModel.boothTableData.Num;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, SendBuyShelfRequest);
        }

        private void SendBuyShelfRequest()
        {
            AuctionNewDataManager.GetInstance().SendBuyShelfRequest();
        }
        #endregion


        #region OnShelfItem
        private void InitOnShelfItem()
        {
            if (_onShelfDataModel.auctionBaseInfo == null)
            {
                Logger.LogErrorFormat("AuctionBaseInfo is null");
                return;
            }

            var itemTable = TableManager.GetInstance()
                .GetTableItem<ItemTable>((int) _onShelfDataModel.auctionBaseInfo.itemTypeId);
            if (itemTable == null)
            {
                Logger.LogErrorFormat("itemTable is null");
                return;
            }

            var itemData = ItemDataManager.CreateItemDataFromTable((int)_onShelfDataModel.auctionBaseInfo.itemTypeId);
            if (itemData == null)
            {
                Logger.LogErrorFormat("itemData is null");
                return;
            }

            itemData.Count = (int) _onShelfDataModel.auctionBaseInfo.num;
            itemData.StrengthenLevel = (int) _onShelfDataModel.auctionBaseInfo.strengthed;
            AuctionNewUtility.UpdateItemDataByEquipType(itemData, _onShelfDataModel.auctionBaseInfo);

            _elementComItem = itemRoot.GetComponentInChildren<ComItem>();
            if (_elementComItem == null)
                _elementComItem = ComItemManager.Create(itemRoot);

            if (_elementComItem != null)
            {
                _elementComItem.Setup(itemData, ShowOnShelfItemTips);
                var isTreasure = _onShelfDataModel.auctionBaseInfo.isTreas == 1 ? true : false;
                _elementComItem.SetShowTreasure(isTreasure);
            }

            if(itemName != null)
                itemName.text = PetDataManager.GetInstance().GetColorName(itemTable.Name, (PetTable.eQuality)itemTable.Color);

            if (_onShelfDataModel.auctionBaseInfo.itemScore > 0)
            {
                itemScore.gameObject.CustomActive(true);
                itemScore.text = string.Format(TR.Value("auction_new_itemDetail_score_value"),
                    _onShelfDataModel.auctionBaseInfo.itemScore);
            }
            else
            {
                itemScore.gameObject.CustomActive(false);
            }

            InitItemTime();
            
            var singlePrice = (ulong) _onShelfDataModel.auctionBaseInfo.price;
            if (_onShelfDataModel.auctionBaseInfo.num > 0)
                singlePrice = (ulong) _onShelfDataModel.auctionBaseInfo.price /
                              (ulong) _onShelfDataModel.auctionBaseInfo.num;


            if (priceValue != null)
            {
                priceValue.text = Utility.GetShowPrice(singlePrice);
                priceValue.text = Utility.ToThousandsSeparator(singlePrice);
            }

            if (sellItemRoot != null)
                sellItemRoot.CustomActive(true);

            //初始化的时候，获得上架物品的状态
            _onShelfItemState = GetOnShelfItemState();
            UpdateOnShelfItemViewByState();
        }

        private void InitItemTime()
        {
            CommonUtility.UpdateGameObjectVisible(itemTimeRoot, false);

            bool isItemOwnerTimeValid = AuctionNewUtility.IsItemOwnerTimeValid((int)_onShelfDataModel.auctionBaseInfo.itemTypeId);
            //非时效道具
            if (isItemOwnerTimeValid == false)
                return;

            //时效道具
            CommonUtility.UpdateGameObjectVisible(itemTimeRoot, true);

            bool isItemInValidTimeInterval = AuctionNewUtility.IsItemInValidTimeInterval(_onShelfDataModel.auctionBaseInfo.itemDueTime);
            if (isItemInValidTimeInterval == true)
            {
                //有效期内
                CommonUtility.UpdateTextVisible(itemTimeInvalidText, false);

                CommonUtility.UpdateTextVisible(itemLeftTimeText, true);
                if (itemLeftTimeText != null)
                {
                    var leftTimeStr = AuctionNewUtility.GetTimeValidItemLeftTimeStr(_onShelfDataModel.auctionBaseInfo.itemDueTime);
                    itemLeftTimeText.text = leftTimeStr;
                }
            }
            else
            {
                CommonUtility.UpdateTextVisible(itemLeftTimeText, false);
                CommonUtility.UpdateTextVisible(itemTimeInvalidText, true);
            }
        }

        private void OnDownShelfItemClick()
        {
            if (_onShelfDataModel == null || _onShelfDataModel.auctionBaseInfo == null)
            {
                Logger.LogErrorFormat("onShelfDataModel is null or auctionBaseInfo is null");
                return;
            }

            //过期物品，弹出重新上架的界面
            if (_onShelfItemState == AuctionNewOnShelfItemState.TimeOverState)
            {                
                //过期商品重新上架
                AuctionNewUtility.OpenAuctionNewOnShelfFrameByTimeOverItem(_onShelfDataModel.auctionBaseInfo);
            }
            else
            {
                //没有过期，弹出下架提示框
                SystemNotifyManager.SystemNotify(1038, OnDownShelfItem);
            }
        }

        private void OnDownShelfItem()
        {
            if (_onShelfDataModel == null || _onShelfDataModel.auctionBaseInfo == null)
            {
                Logger.LogErrorFormat("onShelfDataModel is null or auctionBaseInfo is null");
                return;
            }

            //商品已经过期
            if (_onShelfItemState == AuctionNewOnShelfItemState.TimeOverState)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_item_is_already_time_over"));
            }
            else
            {
                //时效商品，没有处在有效期
                if (AuctionNewUtility.IsItemOwnerTimeValid((int)_onShelfDataModel.auctionBaseInfo.itemTypeId) == true)
                {
                    if (AuctionNewUtility.IsItemInValidTimeInterval(_onShelfDataModel.auctionBaseInfo.itemDueTime) ==
                        false)
                    {
                        //无法取回提示
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_item_not_back_by_time_invalid"));
                        return;
                    }
                }

                //非时效商品，或者时效商品并且在有效期内，则可以正常下架
                //商品正常下架
                AuctionNewDataManager.GetInstance().SendDownShelfItemRequest(_onShelfDataModel.auctionBaseInfo.guid);
            }
        }

        private void ShowOnShelfItemTips(GameObject obj, ItemData itemData)
        {
            if (_onShelfDataModel == null || _onShelfDataModel.auctionBaseInfo == null)
            {
                Logger.LogErrorFormat("onShelfDataModel is null or auctionBaseInfo is null");
                return;
            }

            AuctionNewDataManager.GetInstance().OnShowItemDetailTipFrame(itemData,
                _onShelfDataModel.auctionBaseInfo.guid);
        }

        #endregion


        //回收的时候，elementItem重置，点击无效
        public void OnItemRecycle()
        {
            ResetData();
        }

        #region ItemState
        private void Update()
        {
            if (_onShelfDataModel == null)
                return;

            if (_onShelfDataModel.onShelfState != AuctionNewOnShelfState.OwnerItem)
                return;

            if (_onShelfDataModel.auctionBaseInfo == null)
                return;

            if (_onShelfItemState == AuctionNewOnShelfItemState.None)
                return;

            _timeInterval += Time.deltaTime;
            if (_timeInterval >= 1.0f)
            {
                _timeInterval = 0.0f;
                UpdateOnShelfItemState();
            }
        }

        //更新道具的状态
        private void UpdateOnShelfItemState()
        {
            //状态没有改变，不进行刷新
            var curItemState = GetOnShelfItemState();
            if (curItemState == _onShelfItemState)
                return;

            //设置道具状态，并进行更新
            _onShelfItemState = curItemState;
            UpdateOnShelfItemViewByState();
        }

        //得到道具的状态
        public AuctionNewOnShelfItemState GetOnShelfItemState()
        {
            if (_onShelfDataModel == null)
                return AuctionNewOnShelfItemState.None;

            if (_onShelfDataModel.onShelfState != AuctionNewOnShelfState.OwnerItem)
                return AuctionNewOnShelfItemState.None;

            if (_onShelfDataModel.auctionBaseInfo == null)
                return AuctionNewOnShelfItemState.None;

            //公示期
            if (TimeManager.GetInstance().GetServerTime() <= _onShelfDataModel.auctionBaseInfo.publicEndTime)
                return AuctionNewOnShelfItemState.OnNoticeState;
            
            //待上架期，主要是针对非珍品
            //todo
            if (TimeManager.GetInstance().GetServerTime() <= _onShelfDataModel.auctionBaseInfo.onSaleTime)
                return AuctionNewOnShelfItemState.PrepareShelfState;

            //上架期
            if (TimeManager.GetInstance().GetServerTime() <= _onShelfDataModel.auctionBaseInfo.duetime)
                return AuctionNewOnShelfItemState.OnShelfState;

            //已经过期
            return AuctionNewOnShelfItemState.TimeOverState;
        }
        
        //更新道具的显示
        private void UpdateOnShelfItemViewByState()
        {
            if (_onShelfItemState == AuctionNewOnShelfItemState.None)
            {
                if (itemStateRoot != null)
                    itemStateRoot.gameObject.CustomActive(false);
                return;
            }
            else
            {
                if (itemStateRoot != null)
                    itemStateRoot.gameObject.CustomActive(true);

                ResetOnShelfItemFlag();
                switch (_onShelfItemState)
                {
                    case AuctionNewOnShelfItemState.OnNoticeState:
                        if (noticeStateFlag != null)
                            noticeStateFlag.gameObject.CustomActive(true);
                        return;
                    case AuctionNewOnShelfItemState.PrepareShelfState:
                        if (prepareShelfStateFlag != null)
                            prepareShelfStateFlag.gameObject.CustomActive(true);
                        return;
                    case AuctionNewOnShelfItemState.OnShelfState:
                        if (onShelfStateFlag != null)
                            onShelfStateFlag.gameObject.CustomActive(true);
                        return;
                    case AuctionNewOnShelfItemState.TimeOverState:
                        if (timeOverStateFlag != null)
                            timeOverStateFlag.gameObject.CustomActive(true);
                        //Icon置灰
                        if (itemUiGray != null)
                            itemUiGray.enabled = true;
                        return;
                }
            }
        }

        //重置ItemFlag
        private void ResetOnShelfItemFlag()
        {
            if (noticeStateFlag != null)
                noticeStateFlag.gameObject.CustomActive(false);
            if (onShelfStateFlag != null)
                onShelfStateFlag.gameObject.CustomActive(false);
            if (timeOverStateFlag != null)
                timeOverStateFlag.gameObject.CustomActive(false);
            if (itemUiGray != null)
                itemUiGray.enabled = false;
        }
        
        #endregion
    }
}