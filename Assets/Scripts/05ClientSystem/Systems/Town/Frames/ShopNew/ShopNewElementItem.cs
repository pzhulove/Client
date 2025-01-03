using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ShopNewElementItem : MonoBehaviour
    {

        private ShopNewShopItemInfo _shopNewShopItemTable = null;

        private ItemData _shopItemData = null;
        private ItemData _shopCostItemData = null;
        private List<int> _shopCostEqualItemIdList;     //代替物，道具等价表决定(EqualItemTable)

        private ComItem _comItem = null;
        private string _limitTimeStr = null;
        private bool _isShowOldChangeNewComItem = false;
        private List<AwardItemData> _oldChangeNewItemList = new List<AwardItemData>();

        //兑换材料只有2个的第二个材料
        private int _shopNewSecondCostItemId = 0;
        private int _shopNewSecondCostItemNumber = 0;
        private List<int> _shopNewSecondCostEqualItemIdList;

        //兑换材料不少于3个的数据
        private List<ShopNewCostItemDataModel> _shopNewTotalCostItemDataModelList = new List<ShopNewCostItemDataModel>();
        private List<int> _shopNewTotalCostEqualItemIdList;
        private ShopNewSpecialPriceControl _shopNewSpecialPriceControl;

        //折扣值
        private int _discountValue;         //VipDiscount 和 GoodDiscount只能显示一个（只有一个有效）， 0表示没有折扣


        [Space(5)] [HeaderAttribute("NormalContent")]
        [SerializeField] private GameObject itemRoot;
        [SerializeField] private Text itemName;

        [SerializeField] private Text itemLimitTimes;

        [Space(10)] [HeaderAttribute("PriceInfo")]
        [SerializeField] private GameObject priceRoot;

        [SerializeField] private GameObject normalPriceRoot;
        [SerializeField] private Text priceName;
        [SerializeField] private Image priceIcon;
        [SerializeField] private Text priceValue;
        [SerializeField] private ComOldChangeNewItem comOldChangeNewItem;

        [Space(15)]
        [HeaderAttribute("ShopNewOtherCostItem")]
        [Space(10)]
        [SerializeField] private ShopNewCostItem shopNewOtherCostItem;

        [Space(20)] [HeaderAttribute("ShopNewSpecialPriceRoot")] [Space(5)]
        [SerializeField] private GameObject specialPriceRoot;
        [SerializeField] private string specialPriceControlPath = "UIFlatten/Prefabs/ShopNew/Control/ElementItemCostPriceControl";

        [Space(10)] [HeaderAttribute("OtherInfo")]
        [SerializeField] private Text soldOverText;

        [SerializeField] private GameObject vipRoot;
        [SerializeField] private Text vipText;

        [SerializeField] private GameObject itemLockLimitRoot;
        [SerializeField] private Button itemLockLimitButton;
        [SerializeField] private Text itemLockLimitText;

        [SerializeField] private GameObject canNotBuyMask;

        [Space(20)]
        [HeaderAttribute("Discount")]
        [Space(15)]
        [SerializeField] private Text discountValueLabel;
        [SerializeField] private GameObject discountRoot;
        
        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            // if (buyButton != null)
            // {
            //     buyButton.onClick.RemoveAllListeners();
            //     buyButton.onClick.AddListener(OnBuyButtonClick);
            // }

            if (itemLockLimitButton != null)
            {
                itemLockLimitButton.onClick.RemoveAllListeners();
                itemLockLimitButton.onClick.AddListener(OnBuyButtonClick);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, OnShopNewBuyItemSucceed);
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void UnBindUiEventSystem()
        {
            // if (buyButton != null)
            // {
            //     buyButton.onClick.RemoveAllListeners();
            // }

            if (itemLockLimitButton != null)
            {
                itemLockLimitButton.onClick.RemoveAllListeners();
            }

            if (_comItem != null)
            {
                ComItemManager.Destroy(_comItem);
                _comItem = null;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, OnShopNewBuyItemSucceed);
        }

        private void ClearData()
        {
            OnRecycleElementItem();
            _limitTimeStr = null;
            _shopCostEqualItemIdList = null;

            _discountValue = 0;

            ResetShopNewCostItemData();
        }

        //Item显示的时候
        public void InitElementItem(ShopNewShopItemInfo shopNewShopItemTable)
        {
            _shopNewShopItemTable = shopNewShopItemTable;

            if(_shopNewShopItemTable == null)
                return;

            if(_shopNewShopItemTable.ShopItemTable == null)
                return;

            //获得商品的折扣
            InitShopItemDiscountValue();

            _shopItemData = ItemDataManager.CreateItemDataFromTable(_shopNewShopItemTable.ShopItemTable.ItemID);
            if (_shopItemData == null)
            {
                Logger.LogErrorFormat("ShopNewElementItem InitElementItemData shopItemData is null and ItemId is {0}",
                    _shopNewShopItemTable.ShopItemTable.ItemID);
                return;
            }
            //商品次数
            _shopItemData.Count = _shopNewShopItemTable.ShopItemTable.GroupNum;

            //商品购买的花费数据
            _shopCostItemData = ItemDataManager.CreateItemDataFromTable(_shopNewShopItemTable.ShopItemTable.CostItemID);
            if (_shopCostItemData == null)
            {
                Logger.LogErrorFormat("ShopCostItemData is null and costItemId is {0}",
                    _shopNewShopItemTable.ShopItemTable.ID);
                return;
            }
            //花费道具的替代品
            _shopCostEqualItemIdList = ShopNewUtility.GetShopCostItemEqualItemIdListByOneItem(_shopCostItemData.TableID);

            InitShopNewCostItemData();

            InitElementItemView();
        }

        //商品的折扣
        private void InitShopItemDiscountValue()
        {
            _discountValue = 0;         //表示没有折扣
            //只能取一个
            if (_shopNewShopItemTable.VipDiscount > 0 && _shopNewShopItemTable.VipDiscount < 100)
            {
                _discountValue = _shopNewShopItemTable.VipDiscount;
            }
            else if (_shopNewShopItemTable.GoodDiscount > 0 && _shopNewShopItemTable.GoodDiscount < 100)
            {
                _discountValue = _shopNewShopItemTable.GoodDiscount;
            }
        }

        private void InitElementItemView()
        {
            BindItemEventSystem();

            InitElementItemComItem();
            InitElementItemPrice();

            InitElementItemLimitData();
            UpdateElementItemLimitInfo();

            InitElementOtherInfo();
        }

        private void InitElementItemComItem()
        {
            //商品Item
            if (itemRoot != null)
            {
                _comItem = itemRoot.GetComponentInChildren<ComItem>();

                if (_comItem == null)
                {
                    _comItem = ComItemManager.Create(itemRoot);
                }
                    
                if(_comItem != null && _shopItemData != null)
                {
                    _comItem.Setup(_shopItemData, ShowElementItemTip);
                }
            }

            if (itemName != null && _shopItemData != null)
            {
                itemName.text = _shopItemData.GetColorName();
            }
        }

        private void InitElementItemPrice()
        {
            //兑换品不少于3种
            if (_shopNewTotalCostItemDataModelList != null && _shopNewTotalCostItemDataModelList.Count > 0)
            {
                InitElementSpecialPriceControl();
                return;
            }

            //只有一种或者2中兑换品

            CommonUtility.UpdateGameObjectVisible(specialPriceRoot, false);
            CommonUtility.UpdateGameObjectVisible(normalPriceRoot, true);

            //价格
            var isShowPriceName = ShopNewDataManager.GetInstance().IsMoneyItemShowName(_shopCostItemData.TableID);
            if (isShowPriceName == true)
            {
                //价格
                if (priceIcon != null)
                {
                    priceIcon.gameObject.CustomActive(false);
                }

                if (priceName != null)
                {
                    priceName.text = _shopCostItemData.Name;
                    priceName.gameObject.CustomActive(true);

                }
            }
            else
            {
                //价格
                if (priceIcon != null)
                {
                    EqualItemTable equalTable = TableManager.GetInstance().GetTableItem<EqualItemTable>(_shopCostItemData.TableID);
                    if (equalTable != null)
                    {
                        var tempTable = TableManager.GetInstance().GetTableItem<ItemTable>(equalTable.EqualItemIDs[0]);
                        ETCImageLoader.LoadSprite(ref priceIcon, tempTable.Icon);
                    }
                    else
                    {
                        ETCImageLoader.LoadSprite(ref priceIcon, _shopCostItemData.Icon);
                    }
                    
                    priceIcon.gameObject.CustomActive(true);
                }

                if (priceName != null)
                {
                    priceName.gameObject.CustomActive(false);
                }
            }

            //旧物品换新标志
            _isShowOldChangeNewComItem = ShopNewDataManager.GetInstance().IsShowOldChangeNew(_shopNewShopItemTable) == true 
                                         && _shopNewShopItemTable.LimitBuyTimes != 0;
            comOldChangeNewItem.gameObject.CustomActive(_isShowOldChangeNewComItem);

            _oldChangeNewItemList.Clear();
            if (_isShowOldChangeNewComItem == true)
            {
                ShopNewDataManager.GetInstance()
                    .GetOldChangeNewItem(_shopNewShopItemTable.ShopItemTable, _oldChangeNewItemList);
            }

            InitShopNewSecondCostItemView();

            UpdateCostValue();

        }

        private void InitElementSpecialPriceControl()
        {
            CommonUtility.UpdateGameObjectVisible(specialPriceRoot, true);
            CommonUtility.UpdateGameObjectVisible(normalPriceRoot, false);

            if (_shopNewSpecialPriceControl == null)
            {
                var priceControlPrefab =
                    AssetLoader.GetInstance().LoadResAsGameObject(specialPriceControlPath);

                if (priceControlPrefab != null)
                {
                    priceControlPrefab.transform.SetParent(specialPriceRoot.transform, false);

                    _shopNewSpecialPriceControl = priceControlPrefab.GetComponent<ShopNewSpecialPriceControl>();
                }
            }

            if (_shopNewSpecialPriceControl != null)
                _shopNewSpecialPriceControl.InitSpecialPriceControl(_shopNewTotalCostItemDataModelList,
                    _discountValue);

        }


        //限制的字符串类型
        private void InitElementItemLimitData()
        {
            try
            {
                var shopNewTableId = _shopNewShopItemTable.ShopItemTable.ShopID;

                var isCanRefresh = false;
                var refreshIndex = -1;
                var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopNewTableId);
                if (shopNewTable == null)
                    return;

                if ((shopNewTable.Refresh == 2 || shopNewTable.Refresh == 1)
                    && (shopNewTable.SubType.Count == shopNewTable.NeedRefreshTabs.Count)
                    && (shopNewTable.SubType.Count == shopNewTable.RefreshCycle.Count))
                {
                    for (var i = 0; i < shopNewTable.SubType.Count; i++)
                    {
                        if ((int) shopNewTable.SubType[i] == (int) _shopNewShopItemTable.ShopItemTable.SubType)
                        {
                            refreshIndex = i;
                            break;
                        }
                    }
                }

                if (refreshIndex != -1 && shopNewTable.NeedRefreshTabs[refreshIndex] == 1)
                    isCanRefresh = true;

                if (isCanRefresh == true)
                {
                    switch (shopNewTable.RefreshCycle[refreshIndex])
                    {
                        case ShopTable.eRefreshCycle.REFRESH_CYCLE_NONE:
                            _limitTimeStr = "shop_item_limit_buy_forever";
                            break;
                        case ShopTable.eRefreshCycle.REFRESH_CYCLE_DAILY:
                            _limitTimeStr = "shop_item_limit_buy_daily";
                            if (shopNewTable.Refresh == 1)
                            {
                                _limitTimeStr = "shop_item_limit_buy_mystery";
                            }
                            break;
                        case ShopTable.eRefreshCycle.REFRESH_CYCLE_WEEK:
                            _limitTimeStr = "shop_item_limit_buy_weekly";
                            break;
                        case ShopTable.eRefreshCycle.REFRESH_CYCLE_MONTH:
                            _limitTimeStr = "shop_item_limit_buy_monthly";
                            break;
                        case ShopTable.eRefreshCycle.REFRESH_CYCLE_ACTIVITY:
                            _limitTimeStr = "shop_item_limit_buy_activity";
                            break;
                    }
                }
                else if (_shopNewShopItemTable.LimitBuyTimes > 0)
                {
                    _limitTimeStr = "shop_item_limit_buy_forever";
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("ShopNewElementItem InitElementItemLimit Exception is {0}", e.ToString());
            }
        }

        [SerializeField] private GameObject mObjSelect;
        public void SetSelect(bool isSelect)
        {
            if (null != mObjSelect)
            {
                mObjSelect.CustomActive(isSelect);
            }
        }

        //限购次数引起的变化
        private void UpdateElementItemLimitInfo()
        {
            //显示次数
            var limitBuyTimes = _shopNewShopItemTable.LimitBuyTimes;
            itemLimitTimes.gameObject.CustomActive(limitBuyTimes > 0);
            if (limitBuyTimes > 0 && string.IsNullOrEmpty(_limitTimeStr) == false)
            {
                itemLimitTimes.text = TR.Value(_limitTimeStr, _shopNewShopItemTable.LimitBuyTimes);
            }

            if (limitBuyTimes == 0)
            {
                // buyButton.enabled = false;
                // buyButtonGray.enabled = true;

                soldOverText.gameObject.CustomActive(true);

                priceRoot.gameObject.CustomActive(false);
            }
            else
            {
                // buyButton.enabled = true;
                // buyButtonGray.enabled = false;

                soldOverText.gameObject.CustomActive(false);

                priceRoot.gameObject.CustomActive(true);
            }
        }
        
        //vip discount mask, oldCom
        private void InitElementOtherInfo()
        {

            //限制内容
            var isCanBuy = true;
            var shopItemLimitBuyType = (GoodsLimitButyType)_shopNewShopItemTable.ShopItemTable.ExLimite;
            var iLimitValue = _shopNewShopItemTable.ShopItemTable.ExValue;
            var iCurValue = 0;

            switch (shopItemLimitBuyType)
            {
                case GoodsLimitButyType.GLBT_NONE:
                    itemLockLimitRoot.gameObject.CustomActive(false);
                    break;
                case GoodsLimitButyType.GLBT_TOWER_LEVEL:
                    iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                    itemLockLimitRoot.CustomActive(iCurValue < iLimitValue);
                    itemLockLimitText.text = string.Format(TR.Value("shop_refresh_need_tower_level"), iLimitValue);
                    isCanBuy = iCurValue >= iLimitValue;
                    break;
                case GoodsLimitButyType.GLBT_FIGHT_SCORE:
                    iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                    itemLockLimitRoot.CustomActive(iCurValue < iLimitValue);
                    var rankName = SeasonDataManager.GetInstance().GetRankName(iLimitValue);
                    itemLockLimitText.text = string.Format(TR.Value("shop_refresh_need_fight_score"), rankName);
                    isCanBuy = iCurValue >= iLimitValue;
                    break;
                case GoodsLimitButyType.GLBT_GUILD_LEVEL:
                    iCurValue = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.SHOP);
                    itemLockLimitRoot.CustomActive(iCurValue < iLimitValue);
                    itemLockLimitText.text = string.Format(TR.Value("shop_refresh_need_guild_level"), iLimitValue);
                    isCanBuy = iCurValue >= iLimitValue;
                    break;
                case GoodsLimitButyType.GLBT_HONOR_LEVEL_LIMIT:
                    iCurValue = (int)HonorSystemDataManager.GetInstance().PlayerHonorLevel;
                    itemLockLimitRoot.CustomActive(iCurValue < iLimitValue);
                    itemLockLimitText.text = TR.Value("Honor_System_Shop_Item_Level_Limit", iLimitValue);
                    isCanBuy = iCurValue >= iLimitValue;
                    break;
            }

            //如果限制的内容显示，按钮和价格就不显示
            if (itemLockLimitRoot.activeSelf == true)
            {
                // buyButton.gameObject.CustomActive(false);
                priceRoot.gameObject.CustomActive(false);
            }
            else
            {
                // buyButton.gameObject.CustomActive(true);
                priceRoot.gameObject.CustomActive(!soldOverText.gameObject.activeSelf);
            }


            //Vip 类型
            GoodsData.GoodsDataShowType goodsDataShowType = GetElementShowType();
            vipRoot.gameObject.CustomActive(false);
            if (goodsDataShowType == GoodsData.GoodsDataShowType.GDST_VIP)
            {
                vipRoot.gameObject.CustomActive(true);
                vipText.text = string.Format(TR.Value("vipFormat"), _shopNewShopItemTable.VipLimitLevel);
            }
            //Vip 等级
            if (isCanBuy == true
                && _shopNewShopItemTable.VipLimitLevel > 0
                && _shopNewShopItemTable.VipLimitLevel > PlayerBaseData.GetInstance().VipLevel)
            {
                isCanBuy = false;
            }
            canNotBuyMask.CustomActive(!isCanBuy);

            if (discountRoot != null)
            {
                CommonUtility.UpdateGameObjectVisible(discountRoot, false);
                //存在折扣
                if (_discountValue > 0 && _discountValue < 100)
                {
                    CommonUtility.UpdateGameObjectVisible(discountRoot, true);

                    var shopItemDiscountController = discountRoot.GetComponentInChildren<ShopItemDiscountController>();
                    if (shopItemDiscountController == null)
                    {
                        //加载物体
                        CommonUtility.LoadGameObject(discountRoot);
                        shopItemDiscountController = discountRoot.GetComponentInChildren<ShopItemDiscountController>();
                    }

                    if (shopItemDiscountController != null)
                        shopItemDiscountController.InitShopItemDiscount(_discountValue);
                }
            }


        }

        #region BindItemEvent
        private void BindItemEventSystem()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        private void UnBindItemEventSystem()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }

        private void OnAddNewItem(List<Item> itemList)
        {
            for (var i = 0; i < itemList.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemList[i].uid);

                if (itemData != null)
                {
                    if (IsCostItemData(itemData.TableID) == true) 
                    {
                        UpdateCostValue();
                        break;
                    }

                    if (IsItemOldChangeNew(itemData.TableID) == true)
                    {
                        UpdateCostValue();
                        break;
                    }
                }
            }
        }

        private void OnRemoveItem(ItemData itemData)
        {

            if(itemData == null)
                return;

            if(IsCostItemData(itemData.TableID) == true)
            {
                UpdateCostValue();
                return;
            }

            if (IsItemOldChangeNew(itemData.TableID) == true)
            {
                UpdateCostValue();
                return;
            }
        }

        private void OnUpdateItem(List<Item> itemList)
        {
            for (var i = 0; i < itemList.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemList[i].uid);
                if(itemData == null)
                    continue;

                if(IsCostItemData(itemData.TableID) == true)
                {
                    UpdateCostValue();
                    break;
                }

                if (IsItemOldChangeNew(itemData.TableID) == true)
                {
                    UpdateCostValue();
                    break;
                }
            }
        }

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eTarget)
        {
            UpdateCostValue();
        }

        //判断物品是否为消耗品或者消耗品的取代品
        private bool IsCostItemData(int itemId)
        {
            
            //总的消耗品（消耗品超过3个）
            if (_shopNewTotalCostItemDataModelList != null && _shopNewTotalCostItemDataModelList.Count > 0)
            {
                for (var i = 0; i < _shopNewTotalCostItemDataModelList.Count; i++)
                {
                    var costItemDataModel = _shopNewTotalCostItemDataModelList[i];
                    if (costItemDataModel == null)
                        continue;
                    if (costItemDataModel.CostItemId == itemId)
                        return true;
                }
                //总消耗品的替代品（消耗品超过3个）
                if (_shopNewTotalCostEqualItemIdList != null && _shopNewTotalCostEqualItemIdList.Count > 0)
                {
                    for (var i = 0; i < _shopNewTotalCostEqualItemIdList.Count; i++)
                    {
                        if (_shopNewTotalCostEqualItemIdList[i] == itemId)
                            return true;
                    }
                }

                return false;
            }


            if (_shopCostItemData == null)
                return false;

            //正常消耗品
            if (_shopCostItemData.TableID == itemId)
                return true;

            if (_shopCostEqualItemIdList == null || _shopCostEqualItemIdList.Count <= 0)
                return false;

            //消耗的取代品
            for (var i = 0; i < _shopCostEqualItemIdList.Count; i++)
            {
                if (_shopCostEqualItemIdList[i] == itemId)
                    return true;
            }

            //其他消耗品
            if (_shopNewSecondCostItemId > 0 && _shopNewSecondCostItemId == itemId)
                return true;
            //其他消耗品的替代品
            if (_shopNewSecondCostEqualItemIdList != null && _shopNewSecondCostEqualItemIdList.Count > 0)
            {
                //其他消耗品的替代品
                for (var i = 0; i < _shopNewSecondCostEqualItemIdList.Count; i++)
                {
                    if (_shopNewSecondCostEqualItemIdList[i] == itemId)
                        return true;
                }
            }


            return false;
        }
        #endregion

        #region UpdateCostValue
        private void UpdateCostValue()
        {
            //多种兑换品（至少3种）
            if (_shopNewTotalCostItemDataModelList != null && _shopNewTotalCostItemDataModelList.Count > 0)
            {
                if(_shopNewSpecialPriceControl != null)
                    _shopNewSpecialPriceControl.UpdateCostItemListValue();

                return;
            }

            UpdateShopNewSecondCostItemView();

            if (priceValue == null)
                return;

            if (_shopNewShopItemTable == null)
                return;

            if (_shopCostItemData == null)
                return;


            var costValue = _shopNewShopItemTable.ShopItemTable.CostNum;
            costValue = ShopNewUtility.GetRealCostValue(costValue, _discountValue);
            
            priceValue.text = costValue.ToString();

            var iCurCount = ItemDataManager.GetInstance().GetOwnedItemCount(_shopCostItemData.TableID);
            if (iCurCount >= costValue)
            {
                priceValue.color = Color.white;
            }
            else
            {
                priceValue.color = Color.red;
            }

            UpdateOldChangeNewComItem();
        }

        private void UpdateOldChangeNewComItem()
        {
            if (_isShowOldChangeNewComItem == true && comOldChangeNewItem.gameObject.activeSelf == true)
            {
                comOldChangeNewItem.SetItemId(_shopNewShopItemTable.ShopItemTable.ID);
            }
        }

        private bool IsItemOldChangeNew(int itemId)
        {
            if (_oldChangeNewItemList == null || _oldChangeNewItemList.Count <= 0)
                return false;

            for (var i = 0; i < _oldChangeNewItemList.Count; i++)
            {
                if (_oldChangeNewItemList[i].ID == itemId)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
        //消耗次数的更新
        private void OnShopNewBuyItemSucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (_shopNewShopItemTable == null)
                return;

            var shopItemId = (int)uiEvent.Param1;
            if (_shopNewShopItemTable.ShopItemId != shopItemId)
                return;

            if (_shopNewShopItemTable.LimitBuyTimes == -1)
                return;

            UpdateElementItemLimitInfo();

        }

        //Item回收的时候
        public void OnRecycleElementItem()
        {
            UnBindItemEventSystem();
            _shopNewShopItemTable = null;

            ResetShopNewCostItemData();
        }

        private GoodsData.GoodsDataShowType GetElementShowType()
        {
            if (_shopNewShopItemTable.VipLimitLevel > 0)
            {
                return GoodsData.GoodsDataShowType.GDST_VIP;
            }
            else if (_shopNewShopItemTable.LimitBuyTimes >= 0)
            {
                return GoodsData.GoodsDataShowType.GDST_LIMIT_COUNT;
            }

            return GoodsData.GoodsDataShowType.GDST_NORMAL;
        }


        private void OnBuyButtonClick()
        {
            if(_shopNewShopItemTable == null || _shopNewShopItemTable.ShopItemTable == null)
                return;

            //添加埋点
            Utility.DoStartFrameOperation("ShopNewElementItem", string.Format("ShopItemID/{0}", _shopNewShopItemTable.ShopItemId));
            var shopItemLimitBuyType = (GoodsLimitButyType) _shopNewShopItemTable.ShopItemTable.ExLimite;
            var shopItemLimitValue = _shopNewShopItemTable.ShopItemTable.ExValue;

            if (shopItemLimitBuyType != GoodsLimitButyType.GLBT_NONE)
            {
                if (shopItemLimitBuyType == GoodsLimitButyType.GLBT_TOWER_LEVEL)
                {
                    var iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                    if (iCurValue < shopItemLimitValue)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_tower_level"), shopItemLimitValue));
                        return;
                    }
                }
                else if (shopItemLimitBuyType == GoodsLimitButyType.GLBT_FIGHT_SCORE)
                {
                    var iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                    if (iCurValue < shopItemLimitValue)
                    {
                        var rankName = SeasonDataManager.GetInstance().GetRankName(shopItemLimitValue);
                        SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_fight_score"), rankName));
                        return;
                    }
                }
                else if (shopItemLimitBuyType == GoodsLimitButyType.GLBT_GUILD_LEVEL)
                {
                    var iCurValue = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.SHOP);
                    if (iCurValue < shopItemLimitValue)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_guild_level"), shopItemLimitValue));
                        return;
                    }
                }
                else if (shopItemLimitBuyType == GoodsLimitButyType.GLBT_HONOR_LEVEL_LIMIT)
                {
                    //荣誉商店的荣誉等级限制
                    var iCurValue = (int)HonorSystemDataManager.GetInstance().PlayerHonorLevel;
                    if (iCurValue < shopItemLimitValue)
                    {
                        var unlockLimitLevelStr = string.Format(TR.Value("Honor_System_Item_UnLock_Need_Level_In_Shop"),
                            shopItemLimitValue);
                        SystemNotifyManager.SysNotifyTextAnimation(unlockLimitLevelStr);
                        return;
                    }
                }
            }

            var vipLimitLevel = _shopNewShopItemTable.VipLimitLevel;

            if (vipLimitLevel  > 0 && vipLimitLevel > PlayerBaseData.GetInstance().VipLevel)
            {
                SystemNotifyManager.SystemNotify(1800011, () =>
                {
                    var vipFrame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                    if(vipFrame != null)
                        vipFrame.OpenPayTab();
                });
                return;
            }

            if(ClientSystemManager.GetInstance().IsFrameOpen<ShopNewPurchaseItemFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ShopNewPurchaseItemFrame>();
            ClientSystemManager.GetInstance()
                .OpenFrame<ShopNewPurchaseItemFrame>(FrameLayer.Middle, _shopNewShopItemTable);
           
        }

        private void ShowElementItemTip(GameObject go, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        #region ShopNewCostItem

        private void ResetShopNewCostItemData()
        {
            _isShowOldChangeNewComItem = false;
            _oldChangeNewItemList.Clear();

            _shopNewSecondCostItemNumber = 0;
            _shopNewSecondCostItemId = 0;
            _shopNewSecondCostEqualItemIdList = null;

            _shopNewTotalCostItemDataModelList.Clear();
            _shopNewTotalCostEqualItemIdList = null;
        }

        private void InitShopNewCostItemData()
        {
            ResetShopNewCostItemData();

            if (_shopNewShopItemTable == null || _shopNewShopItemTable.ShopItemTable == null)
                return;

            if (string.IsNullOrEmpty(_shopNewShopItemTable.ShopItemTable.OtherCostItems) == true)
                return;

            var otherCostItemDataModelList =
                ShopNewUtility.GetShopItemOtherCostItemDataModelList(_shopNewShopItemTable.ShopItemTable
                    .OtherCostItems);

            if (otherCostItemDataModelList == null || otherCostItemDataModelList.Count < 1)
                return;

            if (otherCostItemDataModelList.Count == 1)
            {
                //只取第一个
                var otherCostItemDataModel = otherCostItemDataModelList[0];
                if (otherCostItemDataModel != null)
                {
                    _shopNewSecondCostItemId = otherCostItemDataModel.CostItemId;
                    _shopNewSecondCostItemNumber = otherCostItemDataModel.CostItemNumber;
                }

                _shopNewSecondCostEqualItemIdList = ShopNewUtility.GetShopCostItemEqualItemIdListByOneItem(_shopNewSecondCostItemId);
            }
            else if (otherCostItemDataModelList.Count > 1)
            {
                //超过一个， 总货币至少3个
                ShopNewCostItemDataModel firstCostItemDataModel = new ShopNewCostItemDataModel();
                firstCostItemDataModel.CostItemId = _shopCostItemData.TableID;
                firstCostItemDataModel.CostItemNumber = _shopNewShopItemTable.ShopItemTable.CostNum;
                _shopNewTotalCostItemDataModelList.Add(firstCostItemDataModel);

                for (var i = 0; i < otherCostItemDataModelList.Count; i++)
                {
                    var otherCostItemDataModel = otherCostItemDataModelList[i];
                    if (otherCostItemDataModel != null)
                    {
                        var costItemDataModel = new ShopNewCostItemDataModel();
                        costItemDataModel.CostItemId = otherCostItemDataModel.CostItemId;
                        costItemDataModel.CostItemNumber = otherCostItemDataModel.CostItemNumber;
                        _shopNewTotalCostItemDataModelList.Add(costItemDataModel);
                    }
                }

                //物品等价物
                _shopNewTotalCostEqualItemIdList =
                    ShopNewUtility.GetShopCostItemEqualItemIdListByItemList(_shopNewTotalCostItemDataModelList);
            }
        }

        private void InitShopNewSecondCostItemView()
        {
            if (shopNewOtherCostItem == null)
                return;

            shopNewOtherCostItem.gameObject.CustomActive(false);

            if (_shopNewSecondCostItemId <= 0 || _shopNewSecondCostItemNumber <= 0)
                return;

            shopNewOtherCostItem.gameObject.CustomActive(true);

            shopNewOtherCostItem.InitCostItem(_shopNewSecondCostItemId,
                _shopNewSecondCostItemNumber,
                _discountValue);
            shopNewOtherCostItem.UpdateCostItemValue();
        }

        private void UpdateShopNewSecondCostItemView()
        {
            if (shopNewOtherCostItem == null)
                return;

            if (_shopNewSecondCostItemId <= 0 || _shopNewSecondCostItemNumber <= 0)
                return;

            shopNewOtherCostItem.UpdateCostItemValue();
        }


        #endregion

    }
}
