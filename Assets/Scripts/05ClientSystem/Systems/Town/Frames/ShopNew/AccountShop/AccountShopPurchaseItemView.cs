using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class AccountShopPurchaseItemView : MonoBehaviour
    {
        private AccountShopPurchaseItemInfo _accShopLocalItemInfo = null;
        private ComItem _comItem = null;
        private ItemData _costItemData = null;

        private bool _isShowOldChangeNewFlag = false;
        private int _curBuyNumber = 1;                  //当前准备购买的次数
        private int _totalCanBuyNumber = 1;             //总共可以购买的次数

        private bool _bEventBinded = false;

        //private List<GameObject> _oldGameObjectList = new List<GameObject>();
        //private List<ComItem> _oldItemComItemList = new List<ComItem>();
        //private List<ItemInfo> _itemInfoList = new List<ItemInfo>();

        [Space(10)]
        [HeaderAttribute("Close")]
        [SerializeField] private Text purchaseItemTitle;
        [SerializeField] private Button closeButton;

        [Space(10)]
        [HeaderAttribute("Item")]
        [SerializeField] private Text buyItemName;

        [SerializeField] private GameObject buyItemRoot;

        [SerializeField] private GameObject buyItemDiscountRoot;
        [SerializeField] private Text buyItemDiscountText;
        [SerializeField] private Text buyItemDescriptionText;

        [Space(15)] [HeaderAttribute("Cost")] [SerializeField]
        private Text costTitle;
        [SerializeField] private Image costIcon;
        [SerializeField] private Text costNameText;
        [SerializeField] private Text costNumberText;
        [SerializeField] private ComOldChangeNewItem comOldChangeNewItem;

        [Space(15)] [HeaderAttribute("Selected")]
        [SerializeField] private GameObject selectedRoot;

        [SerializeField] private Text leftItemNumberText;

        [SerializeField] private Button minButton;
        [SerializeField] private UIGray minButtonGray;
        [SerializeField] private Button minusButton;
        [SerializeField] private UIGray minusButtonGray;
        [SerializeField] private Button addButton;
        [SerializeField] private UIGray addButtonGray;
        [SerializeField] private Button maxButton;
        [SerializeField] private UIGray maxButtonGray;

        [SerializeField] private Slider selectedSlider;

        [SerializeField] private InputField selectedCountInputField;

        [Space(15)] [HeaderAttribute("OldChangeNew")] 
        [SerializeField] private GameObject OldChangeNewRoot;
        [SerializeField] private GameObject oldItemScrollView;
        [SerializeField] private GameObject oldItemScrollViewContent;
        [SerializeField] private ToggleGroup oldItemScrollViewGroup;
        [SerializeField] private GameObject oldItemPrefab;
        [SerializeField] private GameObject oldItemNotExistNoticeText;

        [Space(15)] [HeaderAttribute("BuyButton")] 
        [SerializeField] private Button buyButton;
        [SerializeField] private Text buyButtonText;
        [SerializeField] private SetComButtonCD buyButtonCD;
        

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (_bEventBinded)
            {
                return;
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseFrame);
            }
            if (selectedSlider != null)
            {
                selectedSlider.onValueChanged.AddListener(OnSelectedSliderChangeValue);
            }
            if (selectedCountInputField != null)
            {
                selectedCountInputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            }
            if (minButton != null)
            {
                minButton.onClick.AddListener(OnMinButtonClickCallBack);
            }
            if (minusButton != null)
            {
                minusButton.onClick.AddListener(OnMinusButtonClickCallBack);
            }
            if (maxButton != null)
            {
                maxButton.onClick.AddListener(OnMaxButtonClickCallBack);
            }
            if (addButton != null)
            {
                addButton.onClick.AddListener(OnAddButtonClickCallBack);
            }
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(OnBuyButtonClickCallBack);
            }
            _bEventBinded = true;
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void ClearData()
        {
            _accShopLocalItemInfo = null;
            if (_comItem != null)
            {
                ComItemManager.Destroy(_comItem);
                _comItem = null;
            }

            _curBuyNumber = 1;
            _totalCanBuyNumber = 1;
            _isShowOldChangeNewFlag = false;

            _costItemData = null;
        }

        private void UnBindUiEventSystem()
        {
            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (selectedSlider != null)
                selectedSlider.onValueChanged.RemoveAllListeners();

            if (selectedCountInputField != null)
                selectedCountInputField.onValueChanged.RemoveAllListeners();

            if (minButton != null)
                minButton.onClick.RemoveAllListeners();

            if (minusButton != null)
                minusButton.onClick.RemoveAllListeners();

            if (maxButton != null)
                maxButton.onClick.RemoveAllListeners();

            if (addButton != null)
                addButton.onClick.RemoveAllListeners();

            if(buyButton != null)
                buyButton.onClick.RemoveAllListeners();

            _bEventBinded = false;
        }

        public void InitShop(AccountShopPurchaseItemInfo accShopItemInfo)
        {
            this._accShopLocalItemInfo = accShopItemInfo;
            if (_accShopLocalItemInfo == null)
            {
                Logger.LogErrorFormat("[AccountShopPurchaseItemView] InitShop ItemData is null");
                return;
            }

            InitShopData();
            InitShopView();
        }

        //主要初始化当前可以购买的总数量，以及购买的数量
        private void InitShopData()
        {
            if (_accShopLocalItemInfo == null)
            {
                return;
            }

            int costItemId = _GetFirstCostItemId();
            var buyCostCount = _GetFirstCostItemNum();

            _costItemData = ItemDataManager.CreateItemDataFromTable(costItemId);
            if(_costItemData == null)
            {
                Logger.LogErrorFormat("InitShopData costItem id can not find in ItemTable and costItemId is {0}", costItemId.ToString());
                return;
            }

            var ownerCostCount = _GetOwnFirstCostItemCount(costItemId);
            if (buyCostCount <= 0)
            {
                Logger.LogErrorFormat("InitShopData CostItem CostNumber is Invalid and costItemId is {0}", costItemId.ToString());
                return;
            }

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)_accShopLocalItemInfo.ItemInfo.itemDataId);
            if (itemTable == null)
            {
                Logger.LogErrorFormat("itemTable is null and itemId is {0}", _accShopLocalItemInfo.ItemInfo.itemDataId.ToString());
                return;
            }

            //消耗品可以购买多少
            _totalCanBuyNumber = ownerCostCount / buyCostCount;
            //商品自身ItemTable的限制
            _totalCanBuyNumber = itemTable.MaxNum < _totalCanBuyNumber ? itemTable.MaxNum : _totalCanBuyNumber;
            int totalCanBuyNumberTemp = _totalCanBuyNumber;
            //刷新导致的限制
            int limitBuyTimes = _GetLimitTimeBuyNum();
            if (limitBuyTimes >= 0)
            {
                _totalCanBuyNumber = limitBuyTimes < _totalCanBuyNumber ? limitBuyTimes : _totalCanBuyNumber;
            }

            // 没有购买限制
            if (limitBuyTimes == 0)
            {
                // 通信证商店的购买最大值要特殊处理下
                ShopTable shopTable = TableManager.GetInstance().GetTableItem<ShopTable>((int)_accShopLocalItemInfo.ShopId);
                if (shopTable != null && shopTable.ShopKind == ShopTable.eShopKind.SK_AdventureCoin)
                {
                    _totalCanBuyNumber = totalCanBuyNumberTemp;
                }
            }

            if (_totalCanBuyNumber < 1)
                _totalCanBuyNumber = 1;
        }


        private void InitShopView()
        {
            InitShopBuyItem();

            InitShopCostItem();

            InitShopSelectedInfo();

            //InitShopOldChangedNewItem();

            InitShopBuyButton();

            OnValueChanged();
        }

        private void InitShopBuyItem()
        {
            if (_accShopLocalItemInfo == null)
            {
                return;
            }
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)_accShopLocalItemInfo.ItemInfo.itemDataId);
            if (itemTable == null)
            {
                Logger.LogErrorFormat("itemTable is null and itemId is {0}", _accShopLocalItemInfo.ItemInfo.itemDataId);
                return;
            }

            buyItemDescriptionText.text = itemTable.Description;

            var itemData = ItemDataManager.CreateItemDataFromTable((int)_accShopLocalItemInfo.ItemInfo.itemDataId);
            if (itemData == null)
            {
                Logger.LogErrorFormat("ItemData is null and ItemId is {0}", _accShopLocalItemInfo.ItemInfo.itemDataId);
                return;
            }

            buyItemName.text = itemData.GetColorName();
            itemData.Count = (int)_accShopLocalItemInfo.ItemInfo.itemNum;
            _comItem = buyItemRoot.GetComponent<ComItem>();
            if (_comItem == null)
            {
                _comItem = ComItemManager.Create(buyItemRoot);
            }
            _comItem.Setup(itemData, null);
        }

        private void InitShopCostItem()
        {
            int costItemId = _GetFirstCostItemId();
            var costItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(costItemId);
            if (costItemTable == null)
            {
                Logger.LogErrorFormat("CostItemTable is null and costItemId is {0}", costItemId);
                return;
            }

            ETCImageLoader.LoadSprite(ref costIcon, costItemTable.Icon);
            costNameText.text = costItemTable.Name;
            bool isShowName = ShopNewDataManager.GetInstance().IsMoneyItemShowName(costItemId);
            if (isShowName == true)
            {
                costNameText.gameObject.CustomActive(true);
                costIcon.gameObject.CustomActive(false);
            }
            else
            {
                costNameText.gameObject.CustomActive(false);
                costIcon.gameObject.CustomActive(true);
            }

            UpdateShopCostItem();

            //_isShowOldChangeNewFlag = ShopNewDataManager.GetInstance().IsShowOldChangeNew(_shopNewShopItemTable);
            comOldChangeNewItem.gameObject.CustomActive(_isShowOldChangeNewFlag);
            if (_isShowOldChangeNewFlag == true)
            {
                //comOldChangeNewItem.SetItemId(_shopItemTable.ID);

                //selectedRoot.gameObject.CustomActive(false);
                //OldChangeNewRoot.CustomActive(true);
            }
            else
            {
                selectedRoot.gameObject.CustomActive(true);
                OldChangeNewRoot.CustomActive(false);
            }

            int limitBuyTimes = _GetLimitTimeBuyNum();
            leftItemNumberText.text = string.Format(TR.Value("shop_buy_limit",limitBuyTimes));
            leftItemNumberText.gameObject.CustomActive(limitBuyTimes > 0 && !_isShowOldChangeNewFlag);
        }

        private void UpdateShopCostItem()
        {
            if (_accShopLocalItemInfo == null)
            {
                return;
            }
            if (_accShopLocalItemInfo.ItemInfo.costItems == null || _accShopLocalItemInfo.ItemInfo.costItems.Length == 0)
            {
                return;
            }
            int costItemId = _GetFirstCostItemId();
            var ownerCostItemCount = _GetOwnFirstCostItemCount(costItemId);
            int costCount = _GetFirstCostItemNum();
            var totalCostCount = costCount * _curBuyNumber;

            costNumberText.text = totalCostCount.ToString();
            costNumberText.color = ownerCostItemCount < totalCostCount
                ? Color.red
                : new Color(0xF4 * 1.0f / 0xFF, 0xDC * 1.0f / 0xFF, 0x89 * 1.0f / 0xFF, 1.0f);
        }

        private void InitShopSelectedInfo()
        {

        }

        //private void InitShopOldChangedNewItem()
        //{
        //    //如果不存在OldChangeNew，直接返回
        //    if(_isShowOldChangeNewFlag == false)
        //        return;

        //    //存在OldChangeNew
        //    var allOldConsumeItem = ShopNewDataManager.GetInstance().GetPackageOldChangeNewEquip(_shopItemTable.ID);

        //    if (allOldConsumeItem == null || allOldConsumeItem.Count <= 0)
        //    {
        //        oldItemNotExistNoticeText.gameObject.CustomActive(true);
        //        oldItemScrollView.gameObject.CustomActive(false);
        //    }
        //    else
        //    {
        //        oldItemNotExistNoticeText.gameObject.CustomActive(false);
        //        oldItemScrollView.gameObject.CustomActive(true);

        //        for (var i = 0; i < allOldConsumeItem.Count; i++)
        //        {
        //            var itemGuid = allOldConsumeItem[i];
        //            var itemData = ItemDataManager.GetInstance().GetItem(itemGuid);
        //            if(itemData == null)
        //                continue;

        //            var gameObjectPrefab = Instantiate(oldItemPrefab) as GameObject;
        //            if (gameObjectPrefab == null)
        //                continue;

        //            var purchaseOldItem = gameObjectPrefab.GetComponent<ShopNewPurchaseOldItem>();
        //            if (purchaseOldItem == null)
        //            {
        //                Destroy(gameObjectPrefab);
        //                continue;
        //            }

        //            _oldGameObjectList.Add(gameObjectPrefab);
        //            Utility.AttachTo(gameObjectPrefab, oldItemScrollViewContent);
        //            gameObjectPrefab.CustomActive(true);

        //            var comItem = ComItemManager.Create(purchaseOldItem.ItemParent);
        //            _oldItemComItemList.Add(comItem);
        //            comItem.Setup(itemData, ShowOldItemTipFrame);

        //            purchaseOldItem.ItemGuid = itemGuid;

        //            purchaseOldItem.SelectedToggle.group = oldItemScrollViewGroup;
        //            purchaseOldItem.SelectedToggle.onValueChanged.RemoveAllListeners();
        //            purchaseOldItem.SelectedToggle.onValueChanged.AddListener((isOn) =>
        //                {
        //                    OnToggleValueChanged(isOn, purchaseOldItem);
        //                });

        //        }
        //    }
        //}

        //private void OnToggleValueChanged(bool value, ShopNewPurchaseOldItem purchaseOldItem)
        //{
        //    purchaseOldItem.SelectedFlag.gameObject.CustomActive(value);
        //    if (value == true)
        //    {
        //        var itemInfo = new ItemInfo();
        //        itemInfo.uid = purchaseOldItem.ItemGuid;
        //        itemInfo.num = 1;
        //        UpdateConsumeItemGuid(itemInfo);
        //    }
        //}

        //private void ShowOldItemTipFrame(GameObject go, ItemData itemData)
        //{
        //    ItemTipManager.GetInstance().ShowTip(itemData);
        //}

        //private void UpdateConsumeItemGuid(ItemInfo itemInfo)
        //{
        //    _itemInfoList.Clear();
        //    _itemInfoList.Add(itemInfo);
        //}

        private void InitShopBuyButton()
        {

        }

        private void OnMinButtonClickCallBack()
        {
            _curBuyNumber = 1;
            OnValueChanged();
        }

        private void OnMinusButtonClickCallBack()
        {
            _curBuyNumber = _curBuyNumber - 1;
            OnValueChanged();
        }

        private void OnMaxButtonClickCallBack()
        {
            _curBuyNumber = _totalCanBuyNumber;
            OnValueChanged();
        }

        private void OnAddButtonClickCallBack()
        {
            _curBuyNumber = _curBuyNumber + 1;
            OnValueChanged();
        }

        private void OnInputFieldValueChanged(string value)
        {
            int curCount = 0;
            if (value.Length > 0)
            {
                curCount = Int32.Parse(value);
            }

            _curBuyNumber = curCount;

            OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (_curBuyNumber < 1)
                _curBuyNumber = 1;

            if (_curBuyNumber > _totalCanBuyNumber)
                _curBuyNumber = _totalCanBuyNumber;

            //更新按钮状态
            minusButton.enabled = _curBuyNumber > 1;
            minusButtonGray.enabled = !minusButton.enabled;

            minButton.enabled = _curBuyNumber > 1;
            minButtonGray.enabled = !minButton.enabled;

            addButton.enabled = _curBuyNumber < _totalCanBuyNumber;
            addButtonGray.enabled = !addButton.enabled;

            maxButton.enabled = _curBuyNumber < _totalCanBuyNumber;
            maxButtonGray.enabled = !maxButton.enabled;

            //更新展示的数值
            selectedCountInputField.text = _curBuyNumber.ToString();

            //更新slider的位置
            if (_totalCanBuyNumber > 1)
            {
                var curSliderValue = (_curBuyNumber - 1) * 1.0f / (_totalCanBuyNumber - 1);
                if (selectedSlider.value != curSliderValue)
                {
                    selectedSlider.value = curSliderValue;
                }
            }
            else
            {
                if (selectedSlider.value != 1.0f)
                    selectedSlider.value = 1.0f;
            }

            UpdateShopCostItem();
        }

        private void OnSelectedSliderChangeValue(float fValue)
        {
            int curValue = 0;
            if (int.TryParse(selectedCountInputField.text, out curValue))
            {
                float fRealValue = 1.0f;
                int iRealValue = 1;
                if (_totalCanBuyNumber <= 1)
                {
                    iRealValue = 1;
                    if (fValue != 1.0f)
                    {
                        selectedSlider.value = 1.0f;
                        return;
                    }
                    selectedCountInputField.text = 1.ToString();
                    return;
                }

                iRealValue = (int)(fValue / (1.0f / (_totalCanBuyNumber - 1)) + 0.50f) + 1;
                if (iRealValue < 1)
                {
                    iRealValue = 1;
                }

                fRealValue = (iRealValue - 1) * 1.0f / (_totalCanBuyNumber - 1);
                if (fRealValue != fValue)
                {
                    selectedSlider.value = fRealValue;
                    return;
                }

                if (curValue != iRealValue)
                {
                    selectedCountInputField.text = iRealValue.ToString();
                }
            }
        }

        private void ShopItemTipFrame(GameObject go, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        private void OnBuyButtonClickCallBack()
        {
            if (buyButtonCD != null && !buyButtonCD.IsBtnWork())
            {
                return;
            }

            //不存在OldChangeNew
            if (_isShowOldChangeNewFlag == false)
            {
                OnPurcahseShopItem();
            }
            else
            {
                /*

                //存在OldChangeNew
                ItemData oldChangeNewItem = new ItemData(0);
                if (ShopNewDataManager.GetInstance().IsPackageHaveExchangeEquipment(_shopItemTable.ID,
                        EPackageType.WearEquip, ref oldChangeNewItem)
                    && ShopNewDataManager.GetInstance().IsPackageHaveExchangeEquipment(_shopItemTable.ID,
                        EPackageType.Equip, ref oldChangeNewItem))
                {
                    OnPurcahseShopItem();
                    return;
                }

                if (ShopNewDataManager.GetInstance()
                    .IsPackageHaveExchangeEquipment(_shopItemTable.ID, EPackageType.WearEquip, ref oldChangeNewItem))
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_pack_wearequip_have_changeequipmaterials")));
                    return;
                }

                if (ShopNewDataManager.GetInstance()
                        .IsPackageHaveExchangeEquipment(_shopItemTable.ID, EPackageType.Equip, ref oldChangeNewItem) ==
                    false)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_insufficient_materials", oldChangeNewItem.Name)));
                    return;
                }

                if (_itemInfoList == null || _itemInfoList.Count <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_pack_Consum_Item")));
                    return;
                }

                OnPurcahseShopItem();
                
              */
            }
            
        }

        private void OnPurcahseShopItem()
        {
            if(_costItemData == null)
            {
                return;
            }
            int costCount = _GetFirstCostItemNum();
            var totalNeedCount = costCount * _curBuyNumber;
            var totalOwnerCount = _GetOwnFirstCostItemCount(_costItemData.TableID);

            //消耗品不足
            if (totalNeedCount > totalOwnerCount)
            {
                if (ItemComeLink.IsLinkMoney(_costItemData.TableID, OnCloseFrame))
                {

                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_insufficient_materials"), _costItemData.Name));
                }
                return;
            }

            if(_curBuyNumber <= 0)
                return;


            var costInfo = new CostItemManager.CostInfo()
            {
               nMoneyID = _costItemData.TableID,
               nCount = totalNeedCount,
            };
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, OnPurchaseShopItemAction);
        }

        private void OnPurchaseShopItemAction()
        {
            if (_accShopLocalItemInfo == null)
            {
                return;
            }

            AccountShopQueryIndex queryIndex = new AccountShopQueryIndex();
            queryIndex.shopId = _accShopLocalItemInfo.ShopId;
            queryIndex.tabType = (byte)_accShopLocalItemInfo.ItemInfo.tabType;
            queryIndex.jobType = (byte)_accShopLocalItemInfo.ItemInfo.jobType;
            AccountShopDataManager.GetInstance().SendWorldAccountShopItemBuyReq(queryIndex, (uint)_accShopLocalItemInfo.ItemInfo.shopItemId, (uint)_curBuyNumber);

            OnCloseFrame();
        }

        private void OnCloseFrame()
        {            
            ClientSystemManager.GetInstance().CloseFrame<AccountShopPurchaseItemFrame>();
        }

        #region Extend Method

        private int _GetFirstCostItemId()
        {
            if (_accShopLocalItemInfo == null)
            {
                return 0;
            }
            if (_accShopLocalItemInfo.ItemInfo.costItems == null || _accShopLocalItemInfo.ItemInfo.costItems.Length == 0)
            {
                return 0;
            }
            int costItemId = (int)_accShopLocalItemInfo.ItemInfo.costItems[0].id;

            return costItemId;
        }

        private int _GetFirstCostItemNum()
        {
            if (_accShopLocalItemInfo == null)
            {
                return 0;
            }
            if (_accShopLocalItemInfo.ItemInfo.costItems == null || _accShopLocalItemInfo.ItemInfo.costItems.Length == 0)
            {
                return 0;
            }

            int costCount = (int)_accShopLocalItemInfo.ItemInfo.costItems[0].num;
            return costCount;
        }

        private int _GetLimitTimeBuyNum()
        {
            if (_accShopLocalItemInfo == null)
            {
                return 0;
            }
            return AccountShopDataManager.GetInstance().GetAccountShopItemCanBuyNum(_accShopLocalItemInfo.ItemInfo);
        }

        private int _GetOwnFirstCostItemCount(int costItemTableId)
        {
            return ItemDataManager.GetInstance().GetOwnedItemCount(costItemTableId);
        }

        #endregion
    }
}
