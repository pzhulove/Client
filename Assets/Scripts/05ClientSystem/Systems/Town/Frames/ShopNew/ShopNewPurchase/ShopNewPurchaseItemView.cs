using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class ShopNewPurchaseItemView : MonoBehaviour
    {

        private ShopNewShopItemInfo _shopNewShopItemTable = null;
        private ShopItemTable _shopItemTable = null;
        private ComItem _comItem = null;

        private ItemData _costItemData = null;

        private bool _isShowOldChangeNewFlag = false;
        private int _curBuyNumber = 1;                  //当前准备购买的次数
        private int _totalCanBuyNumber = 1;             //总共可以购买的次数

        private List<GameObject> _oldGameObjectList = new List<GameObject>();
        private List<ComItem> _oldItemComItemList = new List<ComItem>();
        private List<ItemInfo> _itemInfoList = new List<ItemInfo>();

        //兑换材料只有2个的第二个材料
        private int _shopNewSecondCostItemId = 0;
        private int _shopNewSecondCostItemNumber = 0;

        //多种兑换材料，至少3种
        private List<ShopNewCostItemDataModel> _shopNewTotalCostItemDataModelList = new List<ShopNewCostItemDataModel>();
        private ShopNewSpecialPriceControl _shopNewSpecialPriceControl;

        private int _discountValue;         //折扣

        [Space(10)]
        [HeaderAttribute("Close")]
        [SerializeField]
        private Text purchaseItemTitle;
        [SerializeField]
        private Button closeButton;


        [Space(10)]
        [HeaderAttribute("Item")]
        [SerializeField]
        private Text buyItemName;

        [SerializeField] private GameObject buyItemRoot;

        [SerializeField] private GameObject buyItemDiscountRoot;
        [SerializeField] private Text buyItemDiscountText;
        [SerializeField] private Text buyItemDescriptionText;

        [Space(15)] [HeaderAttribute("Cost")]
        [SerializeField] private Text costTitle;
        [SerializeField] private Image costIcon;
        [SerializeField] private Text costNameText;
        [SerializeField] private Text costNumberText;
        [SerializeField] private ComOldChangeNewItem comOldChangeNewItem;

        [Space(15)]
        [HeaderAttribute("ShopNewOtherCostItem")]
        [Space(10)]
        [SerializeField] private ShopNewCostItem shopNewOtherCostItem;
        [SerializeField] private GameObject normalPriceRoot;

        //多种兑换材料
        [Space(20)]
        [HeaderAttribute("ShopNewSpecialPriceRoot")]
        [Space(5)]
        [SerializeField] private GameObject specialPriceRoot;
        [SerializeField] private string specialPriceControlPath = "UIFlatten/Prefabs/ShopNew/Control/SpecialCostPriceControl";


        [Space(15)] [HeaderAttribute("Selected")] [SerializeField]
        private GameObject selectedRoot;

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

        [Space(15)] [HeaderAttribute("OldChangeNew")] [SerializeField]
        private GameObject OldChangeNewRoot;
        [SerializeField] private GameObject oldItemScrollView;
        [SerializeField] private GameObject oldItemScrollViewContent;
        [SerializeField] private ToggleGroup oldItemScrollViewGroup;
        [SerializeField] private GameObject oldItemPrefab;
        [SerializeField] private GameObject oldItemNotExistNoticeText;

        [Space(15)] [HeaderAttribute("BuyButton")] [SerializeField]
        private Button buyButton;
        [SerializeField] private Text buyButtonText;
        

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

            if (selectedSlider != null)
            {
                selectedSlider.onValueChanged.RemoveAllListeners();
                selectedSlider.onValueChanged.AddListener(OnSelectedSliderChangeValue);
            }

            if (selectedCountInputField != null)
            {
                selectedCountInputField.onValueChanged.RemoveAllListeners();
                selectedCountInputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            }

            if (minButton != null)
            {
                minButton.onClick.RemoveAllListeners();
                minButton.onClick.AddListener(OnMinButtonClickCallBack);
            }

            if (minusButton != null)
            {
                minusButton.onClick.RemoveAllListeners();
                minusButton.onClick.AddListener(OnMinusButtonClickCallBack);
            }

            if (maxButton != null)
            {
                maxButton.onClick.RemoveAllListeners();
                maxButton.onClick.AddListener(OnMaxButtonClickCallBack);
            }

            if (addButton != null)
            {
                addButton.onClick.RemoveAllListeners();
                addButton.onClick.AddListener(OnAddButtonClickCallBack);
            }

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnBuyButtonClickCallBack);
            }

            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void ClearData()
        {
            _shopNewShopItemTable = null;
            _shopItemTable = null;
            if (_comItem != null)
            {
                ComItemManager.Destroy(_comItem);
                _comItem = null;
            }

            _curBuyNumber = 1;
            _totalCanBuyNumber = 1;
            _isShowOldChangeNewFlag = false;

            if(_itemInfoList != null)
                _itemInfoList.Clear();

            if (_oldGameObjectList != null)
                _oldGameObjectList.Clear();

            if (_oldItemComItemList != null)
            {
                for (var i = 0; i < _oldItemComItemList.Count; i++)
                {
                    ComItemManager.Destroy(_oldItemComItemList[i]);
                }
            }

            _costItemData = null;

            ResetShopNewCostItemData();
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

            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
        }

        public void InitShop(ShopNewShopItemInfo shopNewShopItemTable)
        {
            _shopNewShopItemTable = shopNewShopItemTable;
            if (_shopNewShopItemTable == null)
            {
                Logger.LogErrorFormat("ShopNewPurchaseItemView InitShop ItemData is null");
                return;
            }

            if (_shopNewShopItemTable.ShopItemTable == null)
            {
                Logger.LogErrorFormat("ShopNewShopItemTable InitShop shopItemTable is null");
                return;
            }

            InitShopItemDiscountValue();

            _shopItemTable = _shopNewShopItemTable.ShopItemTable;

            InitShopItemBuyData();

            InitShopItemView();
        }

        //初始化折扣的价格
        private void InitShopItemDiscountValue()
        {
            _discountValue = 0;
            if (_shopNewShopItemTable.VipDiscount > 0 && _shopNewShopItemTable.VipDiscount < 100)
            {
                _discountValue = _shopNewShopItemTable.VipDiscount;
            }
            else if (_shopNewShopItemTable.GoodDiscount > 0 && _shopNewShopItemTable.GoodDiscount < 100)
            {
                _discountValue = _shopNewShopItemTable.GoodDiscount;
            }

        }

        //主要初始化当前可以购买的总数量，以及购买的数量
        private void InitShopItemBuyData()
        {

            //是否显示以旧换新
            _isShowOldChangeNewFlag = ShopNewDataManager.GetInstance().IsShowOldChangeNew(_shopNewShopItemTable);

            //第一个消耗品
            _costItemData = ItemDataManager.CreateItemDataFromTable(_shopItemTable.CostItemID);
            var ownerCostCount = ItemDataManager.GetInstance().GetOwnedItemCount(_costItemData.TableID);
            //消耗品
            var buyCostCount = _shopItemTable.CostNum;
            buyCostCount = ShopNewUtility.GetRealCostValue(buyCostCount, _discountValue);

            if (buyCostCount <= 0)
            {
                Logger.LogErrorFormat("InitShopData CostItem CostNumber is Invalid and costItemId is {0}",
                    _shopItemTable.CostItemID);
                return;
            }
            
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_shopItemTable.ItemID);
            if (itemTable == null)
            {
                Logger.LogErrorFormat("itemTable is null and itemId is {0}", _shopItemTable.ItemID);
                return;
            }
            //有消耗品决定购买数量
            _totalCanBuyNumber = ownerCostCount / buyCostCount;

            //其他消耗品数量
            InitShopNewCostItemData();

            //只有两种消耗品，第二种消耗品
            if (_shopNewSecondCostItemId > 0 && _shopNewSecondCostItemNumber > 0)
            {
                //计算其他消耗品最多可以购买的数量
                var canBuyNumberByOtherCost = ShopNewUtility.ShopNewBuyItemNumberByOtherCostItem(_shopNewSecondCostItemId,
                    _shopNewSecondCostItemNumber,
                    _discountValue);
                _totalCanBuyNumber = _totalCanBuyNumber > canBuyNumberByOtherCost
                    ? canBuyNumberByOtherCost
                    : _totalCanBuyNumber;
            }

            //多种消耗品
            if (_shopNewTotalCostItemDataModelList != null && _shopNewTotalCostItemDataModelList.Count > 0)
            {
                for (var i = 0; i < _shopNewTotalCostItemDataModelList.Count; i++)
                {
                    var costItemDataModel = _shopNewTotalCostItemDataModelList[i];
                    if (costItemDataModel != null)
                    {
                        var canBuyNumber = ShopNewUtility.ShopNewBuyItemNumberByOtherCostItem(
                            costItemDataModel.CostItemId,
                            costItemDataModel.CostItemNumber,
                            _discountValue);
                        _totalCanBuyNumber = _totalCanBuyNumber > canBuyNumber
                            ? canBuyNumber
                            : _totalCanBuyNumber;
                    }
                }
            }

            //商品自身ItemTable的限制
            _totalCanBuyNumber = itemTable.MaxNum < _totalCanBuyNumber ? itemTable.MaxNum : _totalCanBuyNumber;
            //刷新导致的限制
            var limitBuyTimes = _shopNewShopItemTable.LimitBuyTimes;
            if (limitBuyTimes >= 0)
            {
                _totalCanBuyNumber = limitBuyTimes < _totalCanBuyNumber ? limitBuyTimes : _totalCanBuyNumber;
            }

            //商品表中配置的一次最多购买数量
            //如果配置BuyLimit字段大于0，则最多可以购买数量为buyLimit
            if (_shopItemTable.BuyLimit > 0)
            {
                if (_totalCanBuyNumber > _shopItemTable.BuyLimit)
                    _totalCanBuyNumber = _shopItemTable.BuyLimit;
            }

            if (_totalCanBuyNumber < 1)
                _totalCanBuyNumber = 1;
            
        }

        private void InitShopItemView()
        {
            InitShopBuyItem();

            InitShopCostItem();

            InitItemLeftNumberInfo();

            InitShopOldChangedNewItem();

            OnValueChanged();
        }

        private void InitShopBuyItem()
        {

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_shopItemTable.ItemID);
            if (itemTable == null)
            {
                Logger.LogErrorFormat("itemTable is null and itemId is {0}", _shopItemTable.ItemID);
                return;
            }

            buyItemDescriptionText.text = itemTable.Description;

            var itemData = ItemDataManager.CreateItemDataFromTable(_shopItemTable.ItemID);
            if (itemData == null)
            {
                Logger.LogErrorFormat("ItemData is null and ItemId is {0}", _shopItemTable.ItemID);
                return;
            }

            buyItemName.text = itemData.GetColorName();
            itemData.Count = _shopItemTable.GroupNum;
            _comItem = buyItemRoot.GetComponent<ComItem>();
            if (_comItem == null)
            {
                _comItem = ComItemManager.Create(buyItemRoot);
            }
            _comItem.Setup(itemData, null);

        }

        private void InitShopCostItem()
        {
            //兑换品不少于3种
            if (_shopNewTotalCostItemDataModelList != null && _shopNewTotalCostItemDataModelList.Count > 0)
            {
                InitItemSpecialPriceControl();
                return;
            }

            //只有一种或者2中兑换品
            CommonUtility.UpdateGameObjectVisible(specialPriceRoot, false);
            CommonUtility.UpdateGameObjectVisible(normalPriceRoot, true);
            
            var costItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_shopItemTable.CostItemID);
            if (costItemTable == null)
            {
                Logger.LogErrorFormat("CostItemTable is null and costItemId is {0}", _shopItemTable.CostItemID);
                return;
            }
            EqualItemTable equalTable = TableManager.GetInstance().GetTableItem<EqualItemTable>(_shopItemTable.CostItemID);
            if(equalTable != null)
            {
                var tempTable = TableManager.GetInstance().GetTableItem<ItemTable>(equalTable.EqualItemIDs[0]);
                ETCImageLoader.LoadSprite(ref costIcon, tempTable.Icon);
            }
            else
            {
                ETCImageLoader.LoadSprite(ref costIcon, costItemTable.Icon);
            }
            costNameText.text = costItemTable.Name;
            bool isShowName = ShopNewDataManager.GetInstance().IsMoneyItemShowName(_shopItemTable.CostItemID);
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

            InitShopNewSecondCostItemView();
            UpdateShopCostItem();
        }

        private void InitItemSpecialPriceControl()
        {
            CommonUtility.UpdateGameObjectVisible(normalPriceRoot, false);

            CommonUtility.UpdateGameObjectVisible(specialPriceRoot, true);

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
                    _discountValue,
                    _curBuyNumber);
        }

        private void InitItemLeftNumberInfo()
        {
            var limitBuyTimes = _shopNewShopItemTable.LimitBuyTimes;
            leftItemNumberText.text = string.Format(TR.Value("shop_buy_limit", limitBuyTimes));
            leftItemNumberText.gameObject.CustomActive(limitBuyTimes > 0 && !_isShowOldChangeNewFlag);
        }

        private void UpdateShopCostItem()
        {
            //多种消耗品
            if (_shopNewTotalCostItemDataModelList != null && _shopNewTotalCostItemDataModelList.Count > 0)
            {
                if (_shopNewSpecialPriceControl != null)
                    _shopNewSpecialPriceControl.UpdateCostItemListValueByNumber(_curBuyNumber);
                return;
            }

            //消耗品数量不超过两种
            var ownerCostItemCount = ItemDataManager.GetInstance().GetOwnedItemCount(_shopItemTable.CostItemID);
            var costCount = _shopItemTable.CostNum;

            //折扣
            costCount = ShopNewUtility.GetRealCostValue(costCount, _discountValue);
            
            var totalCostCount = costCount * _curBuyNumber;

            costNumberText.text = totalCostCount.ToString();
            costNumberText.color = ownerCostItemCount < totalCostCount
                ? Color.red
                : ShopNewDataManager.GetInstance().specialColor;

            UpdateShopNewOtherCostItemView();
        }

        private void InitShopOldChangedNewItem()
        {
            comOldChangeNewItem.gameObject.CustomActive(_isShowOldChangeNewFlag);
            if (_isShowOldChangeNewFlag == true)
            {
                comOldChangeNewItem.SetItemId(_shopItemTable.ID);

                selectedRoot.gameObject.CustomActive(false);
                OldChangeNewRoot.CustomActive(true);
            }
            else
            {
                selectedRoot.gameObject.CustomActive(true);
                OldChangeNewRoot.CustomActive(false);
            }

            //如果不存在OldChangeNew，直接返回
            if (_isShowOldChangeNewFlag == false)
                return;

            //存在OldChangeNew
            var allOldConsumeItem = ShopNewDataManager.GetInstance().GetPackageOldChangeNewEquip(_shopItemTable.ID);

            if (allOldConsumeItem == null || allOldConsumeItem.Count <= 0)
            {
                oldItemNotExistNoticeText.gameObject.CustomActive(true);
                oldItemScrollView.gameObject.CustomActive(false);
            }
            else
            {
                oldItemNotExistNoticeText.gameObject.CustomActive(false);
                oldItemScrollView.gameObject.CustomActive(true);

                for (var i = 0; i < allOldConsumeItem.Count; i++)
                {
                    var itemGuid = allOldConsumeItem[i];
                    var itemData = ItemDataManager.GetInstance().GetItem(itemGuid);
                    if(itemData == null)
                        continue;

                    var gameObjectPrefab = GameObject.Instantiate(oldItemPrefab) as GameObject;
                    if (gameObjectPrefab == null)
                        continue;

                    var purchaseOldItem = gameObjectPrefab.GetComponent<ShopNewPurchaseOldItem>();
                    if (purchaseOldItem == null)
                    {
                        Destroy(gameObjectPrefab);
                        continue;
                    }

                    _oldGameObjectList.Add(gameObjectPrefab);
                    Utility.AttachTo(gameObjectPrefab, oldItemScrollViewContent);
                    gameObjectPrefab.CustomActive(true);

                    var comItem = ComItemManager.Create(purchaseOldItem.ItemParent);
                    _oldItemComItemList.Add(comItem);
                    comItem.Setup(itemData, ShowOldItemTipFrame);

                    purchaseOldItem.ItemGuid = itemGuid;

                    purchaseOldItem.SelectedToggle.group = oldItemScrollViewGroup;
                    purchaseOldItem.SelectedToggle.onValueChanged.RemoveAllListeners();
                    purchaseOldItem.SelectedToggle.onValueChanged.AddListener((isOn) =>
                        {
                            OnToggleValueChanged(isOn, purchaseOldItem);
                        });
                }
            }
        }

        private void OnToggleValueChanged(bool value, ShopNewPurchaseOldItem purchaseOldItem)
        {
            purchaseOldItem.SelectedFlag.gameObject.CustomActive(value);
            if (value == true)
            {
                var itemInfo = new ItemInfo();
                itemInfo.uid = purchaseOldItem.ItemGuid;
                itemInfo.num = 1;
                UpdateConsumeItemGuid(itemInfo);
            }
        }

        private void ShowOldItemTipFrame(GameObject go, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        private void UpdateConsumeItemGuid(ItemInfo itemInfo)
        {
            _itemInfoList.Clear();
            _itemInfoList.Add(itemInfo);
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
            //不存在OldChangeNew
            if (_isShowOldChangeNewFlag == false)
            {
                OnPurchaseShopItem();
            }
            else
            {
                //存在OldChangeNew
                ItemData oldChangeNewItem = new ItemData(0);
                if (ShopNewDataManager.GetInstance().IsPackageHaveExchangeEquipment(_shopItemTable.ID,
                        EPackageType.WearEquip, ref oldChangeNewItem)
                    && ShopNewDataManager.GetInstance().IsPackageHaveExchangeEquipment(_shopItemTable.ID,
                        EPackageType.Equip, ref oldChangeNewItem))
                {
                    OnPurchaseShopItem();
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
                OnPurchaseShopItem();
            }
        }

        private void OnPurchaseShopItem()
        {
            var costCount = _shopItemTable.CostNum;

            costCount = ShopNewUtility.GetRealCostValue(costCount, _discountValue);

            var totalNeedCount = costCount * _curBuyNumber;
            var totalOwnerCount = ItemDataManager.GetInstance().GetOwnedItemCount(_costItemData.TableID);

            var costItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_costItemData.TableID);
            if (costItemTable == null)
            {
                Logger.LogErrorFormat("OnPurchaseShopItem CostItemTable is null and costtItemTableId is {0}",
                    _costItemData.TableID);
                return;
            }

            //基础消耗品不足
            if (totalNeedCount > totalOwnerCount)
            {
                if (ItemComeLink.IsLinkMoney(_costItemData.TableID, OnCloseFrame))
                {

                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_insufficient_materials"), _costItemData.Name));
                    ItemComeLink.OnLink(_costItemData.TableID, 0);
                }
                return;
            }

            //检测其他消耗品是否足够(第二种消耗品)
            if (_shopNewSecondCostItemId > 0 && _shopNewSecondCostItemNumber > 0)
            {
                var secondCostItemCount =
                    ShopNewUtility.GetRealCostValue(_shopNewSecondCostItemNumber, _discountValue);

                var totalNeedOtherCostItemNumber = secondCostItemCount * _curBuyNumber;
                var ownerOtherCostItemNumber = ItemDataManager.GetInstance().GetOwnedItemCount(_shopNewSecondCostItemId);

                //其他消耗品不足
                if (totalNeedOtherCostItemNumber > ownerOtherCostItemNumber)
                {
                    if (ItemComeLink.IsLinkMoney(_shopNewSecondCostItemId, OnCloseFrame))
                    {

                    }
                    else
                    {
                        var otherCostItemTable =
                            TableManager.GetInstance().GetTableItem<ItemTable>(_shopNewSecondCostItemId);
                        if (otherCostItemTable != null)
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(
                                TR.Value("common_purchase_insufficient_materials"),
                                otherCostItemTable.Name));

                        ItemComeLink.OnLink(_shopNewSecondCostItemId, 0);
                    }
                    return;
                }
            }

            //多种消耗品(至少三种)
            if (_shopNewTotalCostItemDataModelList != null && _shopNewTotalCostItemDataModelList.Count > 0)
            {
                for (var i = 0; i < _shopNewTotalCostItemDataModelList.Count; i++)
                {
                    var costItemDataModel = _shopNewTotalCostItemDataModelList[i];
                    if (costItemDataModel != null)
                    {
                        //消耗品需要数量和拥有的数量
                        var costItemCount =
                            ShopNewUtility.GetRealCostValue(costItemDataModel.CostItemNumber, _discountValue);
                        var needCostItemNumber = costItemCount * _curBuyNumber;
                        var ownerCostItemNumber =
                            ItemDataManager.GetInstance().GetOwnedItemCount(costItemDataModel.CostItemId);

                        if (needCostItemNumber > ownerCostItemNumber)
                        {
                            //存在消耗品不足的情况，提示之后，直接返回
                            if (ItemComeLink.IsLinkMoney(costItemDataModel.CostItemId, OnCloseFrame))
                            {

                            }
                            else
                            {
                                var curCostItemTable = TableManager.GetInstance()
                                    .GetTableItem<ItemTable>(costItemDataModel.CostItemId);
                                if(curCostItemTable != null)
                                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(
                                        TR.Value("common_purchase_insufficient_materials"),
                                        curCostItemTable.Name));
                                ItemComeLink.OnLink(costItemDataModel.CostItemId, 0);
                            }

                            return;
                        }
                    }
                }
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
            ShopNewDataManager.GetInstance().BuyGoods(_shopNewShopItemTable.ShopId,
                _shopNewShopItemTable.ShopItemId,
                _curBuyNumber,
                _itemInfoList);

            OnCloseFrame();
        }

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<ShopNewPurchaseItemFrame>();
        }

        #region ShopNewCostItem

        private void ResetShopNewCostItemData()
        {
            _shopNewSecondCostItemNumber = 0;
            _shopNewSecondCostItemId = 0;

            _shopNewTotalCostItemDataModelList.Clear();
            _shopNewSpecialPriceControl = null;
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
                //只有一个
                var otherCostItemDataModel = otherCostItemDataModelList[0];
                if (otherCostItemDataModel != null)
                {
                    _shopNewSecondCostItemId = otherCostItemDataModel.CostItemId;
                    _shopNewSecondCostItemNumber = otherCostItemDataModel.CostItemNumber;
                }
            }
            else if (otherCostItemDataModelList.Count > 1)
            {
                //超过一个， 总货币至少3个
                ShopNewCostItemDataModel firstCostItemDataModel = new ShopNewCostItemDataModel();
                firstCostItemDataModel.CostItemId = _shopItemTable.CostItemID;
                firstCostItemDataModel.CostItemNumber = _shopItemTable.CostNum;
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

            shopNewOtherCostItem.UpdateCostItemValueByBuyNumber(_curBuyNumber);
        }

        private void UpdateShopNewOtherCostItemView()
        {
            if (shopNewOtherCostItem == null)
                return;

            if (_shopNewSecondCostItemId <= 0 || _shopNewSecondCostItemNumber <= 0)
                return;

            shopNewOtherCostItem.UpdateCostItemValueByBuyNumber(_curBuyNumber);
        }

        #endregion

        #region OnUpdateItem
        //数据进行刷新

        private void OnAddNewItem(List<Item> itemList)
        {
            for (var i = 0; i < itemList.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemList[i].uid);

                if (itemData != null)
                {
                    if (IsCostItemData(itemData.TableID) == true)
                    {
                        OnCostItemNumberChanged();
                        break;
                    }
                }
            }
        }

        private void OnRemoveItem(ItemData itemData)
        {

            if (itemData == null)
                return;

            if (IsCostItemData(itemData.TableID) == true)
            {
                OnCostItemNumberChanged();
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

                if (IsCostItemData(itemData.TableID) == true)
                {
                    OnCostItemNumberChanged();
                    break;
                }
            }
        }

        //消耗品的数量发生改变
        private void OnCostItemNumberChanged()
        {
            UpdateTotalBuyNumber();
            OnValueChanged();
        }

        //更新可以购买的最大数量
        private void UpdateTotalBuyNumber()
        {
            ItemTable curItemTable = null;
            if (_costItemData != null && _shopItemTable != null)
            {
                var ownerCostCount = ItemDataManager.GetInstance().GetOwnedItemCount(_costItemData.TableID);
                var buyCostCount = _shopItemTable.CostNum;
                buyCostCount = ShopNewUtility.GetRealCostValue(buyCostCount, _discountValue);

                if (buyCostCount <= 0)
                {
                    Logger.LogErrorFormat("InitShopData CostItem CostNumber is Invalid and costItemId is {0}",
                        _shopItemTable.CostItemID);
                    return;
                }

                curItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_shopItemTable.ItemID);
                if (curItemTable == null)
                {
                    Logger.LogErrorFormat("itemTable is null and itemId is {0}", _shopItemTable.ItemID);
                    return;
                }
                //更新最大数值
                _totalCanBuyNumber = ownerCostCount / buyCostCount;
            }

            //只有两种消耗品，第二种消耗品
            if (_shopNewSecondCostItemId > 0 && _shopNewSecondCostItemNumber > 0)
            {
                //计算其他消耗品最多可以购买的数量
                var canBuyNumberByOtherCost = ShopNewUtility.ShopNewBuyItemNumberByOtherCostItem(_shopNewSecondCostItemId,
                    _shopNewSecondCostItemNumber,
                    _discountValue);
                _totalCanBuyNumber = _totalCanBuyNumber > canBuyNumberByOtherCost
                    ? canBuyNumberByOtherCost
                    : _totalCanBuyNumber;
            }

            //多种消耗品
            if (_shopNewTotalCostItemDataModelList != null && _shopNewTotalCostItemDataModelList.Count > 0)
            {
                for (var i = 0; i < _shopNewTotalCostItemDataModelList.Count; i++)
                {
                    var costItemDataModel = _shopNewTotalCostItemDataModelList[i];
                    if (costItemDataModel != null)
                    {
                        var canBuyNumber = ShopNewUtility.ShopNewBuyItemNumberByOtherCostItem(
                            costItemDataModel.CostItemId,
                            costItemDataModel.CostItemNumber,
                            _discountValue);
                        _totalCanBuyNumber = _totalCanBuyNumber > canBuyNumber
                            ? canBuyNumber
                            : _totalCanBuyNumber;
                    }
                }
            }

            //商品自身ItemTable的限制
            if (curItemTable != null)
            {
                _totalCanBuyNumber = curItemTable.MaxNum < _totalCanBuyNumber ? curItemTable.MaxNum : _totalCanBuyNumber;
            }

            //刷新导致的限制
            if (_shopNewShopItemTable != null)
            {
                var limitBuyTimes = _shopNewShopItemTable.LimitBuyTimes;
                if (limitBuyTimes >= 0)
                {
                    _totalCanBuyNumber = limitBuyTimes < _totalCanBuyNumber ? limitBuyTimes : _totalCanBuyNumber;
                }
            }

            //商品表中配置的一次最多购买数量
            //如果配置BuyLimit字段大于0，则最多可以购买数量为buyLimit
            if (_shopItemTable != null && _shopItemTable.BuyLimit > 0)
            {
                if (_totalCanBuyNumber > _shopItemTable.BuyLimit)
                    _totalCanBuyNumber = _shopItemTable.BuyLimit;
            }

            if (_totalCanBuyNumber < 1)
                _totalCanBuyNumber = 1;
        }

        //判断物品是否为消耗品
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
                return false;
            }


            if (_costItemData == null)
                return false;

            //正常消耗品
            if (_costItemData.TableID == itemId)
                return true;

            //其他消耗品
            if (_shopNewSecondCostItemId > 0 && _shopNewSecondCostItemId == itemId)
                return true;

            return false;
        }

        #endregion

    }
}
