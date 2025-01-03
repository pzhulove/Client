using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AccountShopElementItem : MonoBehaviour
    {
        private uint _shopId = 0;
        private AccountShopItemInfo _accShopItemInfo = null;
        private ItemData _shopItemData = null;
        private ItemData _shopCostItemData = null;

        private ComItem _comItem = null;
        private string _limitTimeStr = null;


        [Space(5)]
        [HeaderAttribute("NormalContent")]

        [SerializeField] private GameObject itemRoot;
        [SerializeField] private Text itemName;

        [SerializeField] private GameObject accBuyLimitRoot;
        [SerializeField] private Text accBuyLimitTime;
        [SerializeField] private GameObject roleBuyLimitRoot;
        [SerializeField] private Text roleBuyLimitTime;

        [Space(10)]
        [HeaderAttribute("BuyContent")]

        [SerializeField] private GameObject buyContentRoot;

        // [SerializeField] private Button buyButton;
        // [SerializeField] private UIGray buyButtonGray;

        [SerializeField] private GameObject priceRoot;
        [SerializeField] private Image priceIcon;
        [SerializeField] private Text priceValue;

        [SerializeField] private Text soldOverText;

        [Space(10)]
        [HeaderAttribute("LockContent")]
        [SerializeField] private GameObject itemLockRoot;
        [SerializeField] private Text itemLockText;

        private string tr_bless_shop_role_cannot_buy_format = "";
        private string tr_bless_shop_role_limit_buy_format = "";

        private string tr_bless_shop_acc_cannot_buy_format = "";
        private string tr_bless_shop_acc_limit_buy_format = "";

        private string tr_bless_shop_level_unlock = "";
        private string tr_bless_shop_refresh_day = "";
        private string tr_bless_shop_refresh_week = "";
        private string tr_bless_shop_refresh_month = "";
        private string tr_bless_shop_refresh_season = "";

        private void Awake()
        {
            BindUiEventSystem();
            _InitTR();
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void BindUiEventSystem()
        {
            // if (buyButton != null)
            // {
            //     buyButton.onClick.AddListener(OnBuyButtonClick);
            // }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountShopItemUpdata, _OnAdventureTeamBlessShopBuySucc);
        }

        private void UnBindUiEventSystem()
        {
            // if (buyButton != null)
            // {
            //     buyButton.onClick.RemoveListener(OnBuyButtonClick);
            // }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountShopItemUpdata, _OnAdventureTeamBlessShopBuySucc);
        }

        private void _InitTR()
        {
            tr_bless_shop_role_cannot_buy_format = TR.Value("adventure_team_shop_role_cannot_buy_format");
            tr_bless_shop_role_limit_buy_format = TR.Value("adventure_team_shop_role_limit_buy_format");
            tr_bless_shop_acc_cannot_buy_format = TR.Value("adventure_team_shop_acc_cannot_buy_format");
            tr_bless_shop_acc_limit_buy_format = TR.Value("adventure_team_shop_acc_limit_buy_format");

            tr_bless_shop_level_unlock = TR.Value("adventure_team_shop_level_unlock");

            tr_bless_shop_refresh_day = TR.Value("adventure_team_shop_refresh_day");
            tr_bless_shop_refresh_week = TR.Value("adventure_team_shop_refresh_week");
            tr_bless_shop_refresh_month = TR.Value("adventure_team_shop_refresh_month");
            tr_bless_shop_refresh_season = TR.Value("adventure_team_shop_refresh_season");
        }

        private void ClearData()
        {
            OnRecycleElementItem();

            if (_comItem != null)
            {
                ComItemManager.Destroy(_comItem);
                _comItem = null;
            }
            _limitTimeStr = null;

            tr_bless_shop_role_cannot_buy_format = "";
            tr_bless_shop_role_limit_buy_format = "";
            tr_bless_shop_acc_cannot_buy_format = "";
            tr_bless_shop_acc_limit_buy_format = "";

            tr_bless_shop_level_unlock = "";
            tr_bless_shop_refresh_day = "";
            tr_bless_shop_refresh_week = "";
            tr_bless_shop_refresh_month = "";
            tr_bless_shop_refresh_season = "";

            _shopId = 0;
            _accShopItemInfo = null;
        }

        private void InitElementItemView()
        {
            BindItemEventSystem();

            //Icon name
            InitElementItemComItem();

            //限制次数
            _RefreshElementItemLimitNumber();

            //购买内容
            InitElementBuyContent();

            //等级限制
            _RefreshElementItemLevelLimit();
        }

        private void InitElementItemComItem()
        {
            if (_shopItemData == null)
            {
                return;
            }

            //商品Item
            if (itemRoot != null)
            {
                ItemData itemData = _shopItemData;
                if (itemData.TableData != null && itemData.TableData.ExpireTime > 0)
                {
                    itemData.DeadTimestamp = itemData.TableData.ExpireTime;
                }
                _comItem = itemRoot.GetComponentInChildren<ComItem>();
                if (_comItem == null)
                    _comItem = ComItemManager.Create(itemRoot);
                _comItem.Setup(_shopItemData, ShowElementItemTip);
            }
            if (itemName != null)
            {
                itemName.text = _shopItemData.GetColorName();
            }
        }

        private void InitElementBuyContent()
        {
            if (_accShopItemInfo == null)
            {
                return;
            }

            //价格
            if (priceIcon != null)
            {
                ETCImageLoader.LoadSprite(ref priceIcon, _shopCostItemData.Icon);
                priceIcon.gameObject.CustomActive(true);
            }
            
            UpdateCostValue();
        }

        //次数限制
        private void _RefreshElementItemLimitNumber()
        {
            _SetBuyLimitTimeText();
        }

        //等级限制
        private void _RefreshElementItemLevelLimit()
        {
            if (_accShopItemInfo == null)
            {
                return;
            }
            ShopTable shopTable = TableManager.GetInstance().GetTableItem<ShopTable>((int)_shopId);
            if(shopTable != null && shopTable.ShopKind == ShopTable.eShopKind.SK_AdventureCoin)
            {
                int lockLv = (int)_accShopItemInfo.extensibleCondition;      
                if (AdventurerPassCardDataManager.GetInstance().CardLv >= lockLv)
                {
                    buyContentRoot.CustomActive(true);
                    itemLockRoot.CustomActive(false);
                }
                else
                {
                    buyContentRoot.CustomActive(false);
                    itemLockRoot.CustomActive(true);
                    if (itemLockText)
                    {
                        itemLockText.text = TR.Value("adventurer_pass_card_shop_lv_limit", lockLv);
                    }
                }
            }
            else
            {
            if (_accShopItemInfo.extensibleCondition <= AdventureTeamDataManager.GetInstance().GetAdventureTeamLevel())
            {
                buyContentRoot.CustomActive(true);
                itemLockRoot.CustomActive(false);
            }
            else
            {
                buyContentRoot.CustomActive(false);
                itemLockRoot.CustomActive(true);
                if (itemLockText)
                {
                    itemLockText.text = string.Format(tr_bless_shop_level_unlock, _accShopItemInfo.extensibleCondition.ToString());
                    }
                }
            }            
        }

        private void _SetBuyLimitTimeText()
        {
            bool accShow = false;
            bool roleShow = false;
            if (_accShopItemInfo == null)
            {
                accBuyLimitRoot.CustomActive(accShow);
                roleBuyLimitRoot.CustomActive(roleShow);
                return;
            }
            int typeCount = (int)AccountShopDataManager.GetInstance().GetAccShopItemBuyLimitType(_accShopItemInfo);
            if (typeCount == (int)AccountShopPurchaseType.RoleBind)
            {
                roleShow = true;
            }
            else if (typeCount == (int)AccountShopPurchaseType.AccountBind)
            {
                accShow = true;
            }
            // else if (typeCount > (int)AccountShopPurchaseType.AccountBind)
            // {
            //     accShow = true;
            //     roleShow = true;
            // }
            accBuyLimitRoot.CustomActive(accShow);
            roleBuyLimitRoot.CustomActive(roleShow);

            if (accShow)
            {
                if (_accShopItemInfo.accountRestBuyNum > 0)
                {
                    //accBuyLimitTime.text = string.Format(tr_bless_shop_limit_buy_count_enough, _accShopItemInfo.accountRestBuyNum.ToString(), ////_accShopItemInfo.accountLimitBuyNum.ToString());

                    accBuyLimitTime.text = string.Format(tr_bless_shop_acc_limit_buy_format, _GetShopRrefreshTimeTitle((AccountShopRefreshType)_accShopItemInfo.accountRefreshType), _accShopItemInfo.accountRestBuyNum.ToString());

                    accBuyLimitRoot.CustomActive(true);
                    _SetBuySoldOutActive(false);
                    // _SetBuyButtonEnable(true);
                }
                else
                {
                    //accBuyLimitTime.text = string.Format(tr_bless_shop_limit_buy_count_notenough, _accShopItemInfo.accountRestBuyNum.ToString(), _accShopItemInfo.accountLimitBuyNum.ToString());

                    accBuyLimitTime.text = tr_bless_shop_acc_cannot_buy_format;

                    accBuyLimitRoot.CustomActive(false);
                    _SetBuySoldOutActive(true);
                    // _SetBuyButtonEnable(false);
                }
            }
            else if (roleShow)
            {
                if (_accShopItemInfo.roleRestBuyNum > 0)
                {
                    //roleBuyLimitTime.text = string.Format(tr_bless_shop_limit_buy_count_enough, _accShopItemInfo.roleRestBuyNum.ToString(), _accShopItemInfo.roleLimitBuyNum.ToString());
                    roleBuyLimitTime.text = string.Format(tr_bless_shop_role_limit_buy_format, _GetShopRrefreshTimeTitle((AccountShopRefreshType)_accShopItemInfo.roleRefreshType), _accShopItemInfo.roleRestBuyNum.ToString());

                    if (accShow && _accShopItemInfo.accountRestBuyNum <= 0)
                    {
                        roleBuyLimitRoot.CustomActive(false);
                        _SetBuySoldOutActive(true);
                        // _SetBuyButtonEnable(false);
                    }
                    else
                    {
                        roleBuyLimitRoot.CustomActive(true);
                        _SetBuySoldOutActive(false);
                        // _SetBuyButtonEnable(true);
                    }
                }
                else
                {
                    //roleBuyLimitTime.text = string.Format(tr_bless_shop_limit_buy_count_notenough, _accShopItemInfo.roleRestBuyNum.ToString(), _accShopItemInfo.roleLimitBuyNum.ToString());

                    roleBuyLimitTime.text = tr_bless_shop_role_cannot_buy_format;

                    if (accShow && _accShopItemInfo.accountRestBuyNum > 0)
                    {
                        roleBuyLimitRoot.CustomActive(true);
                        _SetBuySoldOutActive(false);
                        // _SetBuyButtonEnable(false);
                    }
                    else
                    {
                        roleBuyLimitRoot.CustomActive(false);
                        _SetBuySoldOutActive(true);
                        // _SetBuyButtonEnable(false);
                    }
                }
            }
            else
            {
                _SetBuySoldOutActive(false);
            }
        }

        // private void _SetBuyButtonEnable(bool bEnable)
        // {
        //     if (buyButton)
        //     {
        //         buyButton.enabled = bEnable;
        //     }
        //     if (buyButtonGray)
        //     {
        //         buyButtonGray.enabled = !bEnable;
        //     }
        // }

        private void _SetBuySoldOutActive(bool bEnable)
        {
            if (soldOverText)
            {
                soldOverText.CustomActive(bEnable);
            }
            if (priceRoot)
            {
                priceRoot.CustomActive(!bEnable);
            }
        }

        #region BindItemEvent

        private void BindItemEventSystem()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;

            //添加冒险团的等级
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamLevelUp, _OnAdventureTeamLevelUp);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
        }

        private void UnBindItemEventSystem()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamLevelUp, _OnAdventureTeamLevelUp);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
        }

        private void OnAddNewItem(List<Item> itemList)
        {
            for (var i = 0; i < itemList.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemList[i].uid);

                if (itemData != null)
                {
                    if (itemData.TableID == _shopCostItemData.TableID)
                    {
                        UpdateCostValue();
                        break;
                    }
                }
            }
        }

        private void OnRemoveItem(ItemData itemData)
        {
            if (itemData == null)
                return;

            if (itemData.TableID == _shopCostItemData.TableID)
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
                if (itemData == null)
                    continue;

                if (itemData.TableID == _shopCostItemData.TableID)
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

        private void _OnAdventureTeamLevelUp(UIEvent uiEvent)
        {
            _RefreshElementItemLevelLimit();
        }
        private void _OnUpdateAventurePassStatus(UIEvent uiEvent)
        {
            _RefreshElementItemLevelLimit();
        }

        #endregion

        #region UpdateCostValue
        private void UpdateCostValue()
        {
            if (priceValue == null)
                return;

            if (_accShopItemInfo == null)
                return;

            if (_shopCostItemData == null)
                return;

            var costItemList = _accShopItemInfo.costItems;
            if(costItemList == null)
            {
                return;
            }

            for (int i = 0; i < costItemList.Length; i++)
			{
                var costValue = costItemList[i];

                priceValue.text = costValue.num.ToString();

                //var iCurCount = ItemDataManager.GetInstance().GetOwnedItemCount(_shopCostItemData.TableID);
                var iCurCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)costValue.id);
                if (iCurCount >= costValue.num)
                {
                    priceValue.color = Color.white;
                }
                else
                {
                    priceValue.color = Color.red;
                    break;
                }
			}
        }

        #endregion

        //消耗次数的更新
        private void _OnAdventureTeamBlessShopBuySucc(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (_accShopItemInfo == null)
                return;

            var shopQueryIndex = (AccountShopQueryIndex)uiEvent.Param1;
            if (shopQueryIndex == null)
                return;

            if (_accShopItemInfo.tabType != shopQueryIndex.tabType ||
                _accShopItemInfo.jobType != shopQueryIndex.jobType)
                return;

            //TODO 
            //Limit Buy Num
            //if (AdventureTeamDataManager.GetInstance().IsBlessShopItemBuyLimit(shopItemId))
            //{
            //    return;
            //}

            _RefreshElementItemLevelLimit();
            _RefreshElementItemLimitNumber();
        }
        
        private void OnBuyButtonClick()
        {
            if (_accShopItemInfo == null)
                return;

            AccountShopPurchaseItemInfo purchaseItemInfo = new AccountShopPurchaseItemInfo(_shopId, _accShopItemInfo);
            //添加埋点
            Utility.DoStartFrameOperation("AccountShopElementItem", string.Format("ShopItemID/{0}", _accShopItemInfo.shopItemId));
            if (ClientSystemManager.GetInstance().IsFrameOpen<AccountShopPurchaseItemFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<AccountShopPurchaseItemFrame>();
            ClientSystemManager.GetInstance().OpenFrame<AccountShopPurchaseItemFrame>(FrameLayer.Middle, purchaseItemInfo);
        }

        private void ShowElementItemTip(GameObject go, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        private string _GetShopRrefreshTimeTitle(AccountShopRefreshType refreshType)
        {
            switch (refreshType)
            {
                case AccountShopRefreshType.EachDay:
                    return tr_bless_shop_refresh_day;
                case AccountShopRefreshType.EachWeekend:
                    return tr_bless_shop_refresh_week;
                case AccountShopRefreshType.EachMonth:
                    return tr_bless_shop_refresh_month;
                case AccountShopRefreshType.EachSeason:
                    return tr_bless_shop_refresh_season;
                default:
                    return "";
            }
        }

        #region PUBLIC METHOD

        //Item回收的时候
        public void OnRecycleElementItem()
        {
            UnBindItemEventSystem();

            _accShopItemInfo = null;
            _shopItemData = null;
            _shopCostItemData = null;
        }

        //Item显示的时候
        public void InitElementItem(uint shopId, AccountShopItemInfo itemInfo)
        {
            this._shopId = shopId;
            this._accShopItemInfo = itemInfo;

            if (_accShopItemInfo == null)
                return;

            _shopItemData = ItemDataManager.CreateItemDataFromTable((int)_accShopItemInfo.itemDataId);
            if (_shopItemData == null)
            {
                Logger.LogErrorFormat("[AccountShopElementItem] InitElementItemData shopItemData is null and ItemId is {0}", _accShopItemInfo.itemDataId.ToString());
                return;
            }
            //商品次数
            _shopItemData.Count = (int)itemInfo.itemNum;

            var costItemDataList = itemInfo.costItems;
            if (costItemDataList == null && costItemDataList.Length == 0)
            {
                Logger.LogErrorFormat("[AccountShopElementItem] InitElementItemData shopCostItemData is null or length is 0, buy ItemId is {0}", _accShopItemInfo.itemDataId.ToString());
                return;
            }

            //默认取第一个
            _shopCostItemData = ItemDataManager.CreateItemDataFromTable((int)costItemDataList[0].id);
            if (_shopCostItemData == null)
            {
                Logger.LogErrorFormat("ShopCostItemData is null and costItemId is {0}", (int)costItemDataList[0].id);
                return;
            }

            InitElementItemView();
        }

        [SerializeField] private GameObject mObjSelect;
        public void SetSelect(bool isSelect)
        {
            if (null != mObjSelect)
            {
                mObjSelect.CustomActive(isSelect);
            }
        }

        #endregion

    }
}
