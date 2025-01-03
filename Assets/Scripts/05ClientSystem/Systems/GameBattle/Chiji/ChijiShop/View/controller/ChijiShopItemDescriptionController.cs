using System;
using System.Collections.Generic;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChijiShopItemDescriptionController : MonoBehaviour
    {
        private ChijiShopItemDataModel _chijiShopItemDataModel;
        private ItemData _itemData;

        private float _itemDescriptionLabelPosY;

        [Space(10)] [HeaderAttribute("Root")] [Space(10)]
        [SerializeField] private GameObject controllerRoot;

        [Space(10)] [HeaderAttribute("itemDescription")] [Space(10)]
        [SerializeField] private Text itemNameLabel;
        [SerializeField] private Text itemDescriptionLabel;


        [Space(10)] [HeaderAttribute("DealButton")] [Space(10)]
        [SerializeField] private GameObject itemDealRoot;
        [SerializeField] private Button itemButton;
        [SerializeField] private Text itemButtonText;
        [SerializeField] private UIGray itemButtonGray;

        [Space(10)]
        [HeaderAttribute("DealCost")]
        [Space(10)]
        [SerializeField] private Image itemDealIcon;
        [SerializeField] private Text itemDealValueText;

        private void Awake()
        {
            InitData();
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
        }

        private void InitData()
        {
            if (itemDescriptionLabel != null)
            {
                _itemDescriptionLabelPosY = itemDescriptionLabel.transform.localPosition.y;
            }
        }

        private void BindUiEvents()
        {
            if (itemButton != null)
            {
                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(OnItemButtonClicked);
            }
        }

        private void UnBindUiEvents()
        {
            if(itemButton != null)
                itemButton.onClick.RemoveAllListeners();
        }

        public void UpdateItemDescriptionController(ChijiShopItemDataModel chijiShopItemDataModel,
            ItemData itemData)
        {

            _chijiShopItemDataModel = chijiShopItemDataModel;
            _itemData = itemData;

            if (_chijiShopItemDataModel == null
                || _itemData == null)
            {
                CommonUtility.UpdateGameObjectVisible(controllerRoot, false);
                return;
            }
            
            CommonUtility.UpdateGameObjectVisible(controllerRoot, true);

            UpdateItemDetailLabel();
            UpdateItemDealContent();
            
        }

        private void UpdateItemDetailLabel()
        {

            if (itemNameLabel != null)
            {
                var itemNameStr = _itemData.GetColorName();
                itemNameLabel.text = itemNameStr;
            }

            if (itemDescriptionLabel != null)
            {
                //重置位置
                var itemDescriptionLabelLocalPos = itemDescriptionLabel.transform.localPosition;
                itemDescriptionLabel.transform.localPosition = new Vector3(
                    itemDescriptionLabelLocalPos.x,
                    _itemDescriptionLabelPosY,
                    itemDescriptionLabelLocalPos.z);
                
                var showDetailStr = ChijiShopUtility.GetItemDetailStr(_itemData, false);
                itemDescriptionLabel.text = showDetailStr;
            }
        }

        private void UpdateItemDealContent()
        {

            UpdateItemDealDisplay();

            if (_chijiShopItemDataModel.ChijiShopType == ChijiShopType.Sell)
            {
                if (itemButtonText != null)
                    itemButtonText.text = TR.Value("Chiji_Shop_Item_Sell_Label");
                
                CommonUtility.UpdateButtonState(itemButton, itemButtonGray,
                    true);

                //交易的价格
                if (itemDealIcon != null)
                {
                    var dealItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_itemData.PriceItemID);
                    if (dealItemTable != null)
                        ETCImageLoader.LoadSprite(ref itemDealIcon, dealItemTable.Icon);
                }

                if (itemDealValueText != null)
                    itemDealValueText.text = _itemData.Price.ToString();

            }
            else if(_chijiShopItemDataModel.ChijiShopType == ChijiShopType.Buy)
            {
                if (itemButtonText != null)
                    itemButtonText.text = TR.Value("Chiji_Shop_Item_Buy_Label");

                if (itemDealIcon != null)
                {
                    var dealItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(
                        _chijiShopItemDataModel.ShopItemTable.CostItemID);
                    if (dealItemTable != null)
                        ETCImageLoader.LoadSprite(ref itemDealIcon, dealItemTable.Icon);

                    if (itemDealValueText != null)
                        itemDealValueText.text = _chijiShopItemDataModel.ShopItemTable.CostNum.ToString();
                }

                UpdateItemButtonState();
            }

        }

        public void UpdateItemButtonState()
        {
            if (ChijiShopUtility.GetCurrentOwnerGloryCoinNumber() <
                _chijiShopItemDataModel.ShopItemTable.CostNum)
            {
                CommonUtility.UpdateButtonState(itemButton, itemButtonGray,
                    false);
            }
            else
            {
                CommonUtility.UpdateButtonState(itemButton, itemButtonGray,
                    true);
            }

        }

        public void UpdateItemDealDisplay()
        {
            if (_chijiShopItemDataModel == null)
                return;

            if (_chijiShopItemDataModel.ChijiShopType == ChijiShopType.Sell)
            {
                CommonUtility.UpdateGameObjectVisible(itemDealRoot, true);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(itemDealRoot,
                    !_chijiShopItemDataModel.IsSoldOver);
            }
        }


        #region ButtonClicked


        private void OnItemButtonClicked()
        {
            if (_chijiShopItemDataModel == null)
                return;

            if (_itemData == null)
                return;

            if (_chijiShopItemDataModel.ChijiShopType == ChijiShopType.Sell)
            {
                ChijiShopUtility.OnSellItemInChijiScene(_itemData);
            }
            else if (_chijiShopItemDataModel.ChijiShopType == ChijiShopType.Buy)
            {
                ChijiShopUtility.OnBuyItemInChijiScene(_chijiShopItemDataModel);
            }
        }


        #endregion 

    }
}
