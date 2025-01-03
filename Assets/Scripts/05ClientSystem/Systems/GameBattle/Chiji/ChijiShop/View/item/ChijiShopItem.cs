using System;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public delegate void OnShopItemSelected(int itemIndex,
        ChijiShopItemDataModel chijiShopItemDataModel,
        ItemData itemData);
    
    public class ChijiShopItem : MonoBehaviour
    {
        private ChijiShopItemDataModel _chijiShopItemDataModel;
        private OnShopItemSelected _onShopItemSelected;

        //(出售界面，表示道具的ItemData；购买界面表示获得商品的ItemData);
        private ItemData _itemData;
        

        [Space(10)]
        [HeaderAttribute("Item")]
        [Space(10)]
        [SerializeField] private Text itemName;
        [SerializeField] private GameObject itemRoot;
        [SerializeField] private GameObject beWearFlag;

        [Space(10)] [HeaderAttribute("Cost")] [Space(10)]
        [SerializeField] private Image costIcon;
        [SerializeField] private Text costValueText;
        [SerializeField] private GameObject itemCostRoot;
        [SerializeField] private GameObject itemSoldOverRoot;

        [Space(10)] [HeaderAttribute("Selected")] [Space(10)]
        [SerializeField] private Button itemSelectedButton;
        [SerializeField] private GameObject itemSelectedFlag;

        private void Awake()
        {
            BindUiEvents();
        }
        

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void BindUiEvents()
        {
            if (itemSelectedButton != null)
            {
                itemSelectedButton.onClick.RemoveAllListeners();
                itemSelectedButton.onClick.AddListener(OnItemSelectedButtonClicked);
            }

        }

        private void UnBindUiEvents()
        {
            if(itemSelectedButton != null)
                itemSelectedButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _chijiShopItemDataModel = null;
            _itemData = null;
        }

        public void InitItem(ChijiShopItemDataModel chijiShopItemDataModel,
            OnShopItemSelected onShopItemSelected)
        {
            _chijiShopItemDataModel = chijiShopItemDataModel;
            _onShopItemSelected = onShopItemSelected;

            if (_chijiShopItemDataModel == null)
                return;

            _itemData = ChijiShopUtility.GetItemDataByChijiShopItemDataModel(_chijiShopItemDataModel);
            
            if (_itemData == null)
                return;

            if (_chijiShopItemDataModel.ChijiShopType == ChijiShopType.Buy)
            {
                if (_chijiShopItemDataModel.ShopItemTable == null)
                    return;
            }

            InitItemView();

        }

        private void InitItemView()
        {
            CommonUtility.UpdateGameObjectVisible(beWearFlag, false);

            //出售界面
            if (_chijiShopItemDataModel.ChijiShopType == ChijiShopType.Sell)
            {
                if (itemName != null)
                    itemName.text = _itemData.GetColorName();

                CommonUtility.UpdateGameObjectVisible(itemSoldOverRoot, false);
                CommonUtility.UpdateGameObjectVisible(itemCostRoot, true);

                if (costValueText != null)
                    costValueText.text = _itemData.Price.ToString();

                if (costIcon != null)
                {
                    var costItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_itemData.PriceItemID);
                    if (costItemTable != null)
                        ETCImageLoader.LoadSprite(ref costIcon, costItemTable.Icon);
                }
            }
            else
            {
                //购买界面
                var shopItemTable = _chijiShopItemDataModel.ShopItemTable;
                //购买，展示的道具名字
                if (itemName != null)
                    itemName.text = shopItemTable.CommodityName;

                if (costValueText != null)
                    costValueText.text = shopItemTable.CostNum.ToString();

                if (costIcon != null)
                {
                    var costItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(
                        shopItemTable.CostItemID);

                    if (costItemTable != null)
                        ETCImageLoader.LoadSprite(ref costIcon, costItemTable.Icon);
                }

                if (_itemData != null)
                {
                    //是否已经穿戴的标志
                    var wearItemData = ItemDataManager.GetInstance().GetWearEquipDataBySlotType(
                        _itemData.EquipWearSlotType);
                    //道具的TableId和同位置上的穿戴的道具TableId相同
                    if (wearItemData != null && wearItemData.TableID == _itemData.TableID)
                    {
                        CommonUtility.UpdateGameObjectVisible(beWearFlag, true);
                    }
                }

                UpdateShopItemSoldState();
            }

            if (itemRoot != null)
            {
                var commonNewItemDataModel = new CommonNewItemDataModel()
                {
                    ItemId = _itemData.TableID,
                    ItemCount = _itemData.Count,
                    ItemStrengthenLevel = _itemData.StrengthenLevel,
                };

                var commonNewItem = itemRoot.GetComponentInChildren<CommonNewItem>();
                if (commonNewItem == null)
                    commonNewItem = CommonUtility.CreateCommonNewItem(itemRoot);

                if (commonNewItem != null)
                    commonNewItem.InitItem(commonNewItemDataModel);
            }

            UpdateItemSelectedFlag();
        }

        //只对商品有效
        private void UpdateShopItemSoldState()
        {
            //已经出售
            if (_chijiShopItemDataModel.IsSoldOver == true)
            {
                CommonUtility.UpdateGameObjectVisible(itemSoldOverRoot, true);
                CommonUtility.UpdateGameObjectVisible(itemCostRoot, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(itemSoldOverRoot, false);
                CommonUtility.UpdateGameObjectVisible(itemCostRoot, true);
            }
        }

        private void UpdateItemSelectedFlag()
        {
            if (itemSelectedFlag == null)
                return;

            if (_chijiShopItemDataModel == null)
                return;

            if (_itemData == null)
                return;

            CommonUtility.UpdateGameObjectVisible(itemSelectedFlag,
                _chijiShopItemDataModel.IsSelected);

        }

        public void UpdateItem()
        {
            UpdateItemSelectedFlag();

            if (_chijiShopItemDataModel != null
                && _chijiShopItemDataModel.ChijiShopType == ChijiShopType.Buy)
            {
                UpdateShopItemSoldState();
            }
        }

        public void RecycleItem()
        {
            ClearData();
        }

        #region ButtonClicked
        private void OnItemSelectedButtonClicked()
        {
            if (_chijiShopItemDataModel == null)
                return;

            if (_itemData == null)
                return;


            //已经选中
            if (_chijiShopItemDataModel.IsSelected == true)
                return;

            _chijiShopItemDataModel.IsSelected = true;
            UpdateItemSelectedFlag();

            if (_onShopItemSelected != null)
                _onShopItemSelected(_chijiShopItemDataModel.ItemIndex,
                    _chijiShopItemDataModel,
                    _itemData);
        }
        #endregion


    }
}
