using System;
using System.Collections.Generic;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChijiShopItemController : MonoBehaviour
    {
        private ChijiShopType _chijiShopType = ChijiShopType.None;

        private List<ChijiShopItemDataModel> _chijiShopItemDataModelList = new List<ChijiShopItemDataModel>();

        private ChijiShopItemDataModel _defaultChijiShopItemDataModel;
        

        [Space(10)]
        [HeaderAttribute("noItemTip")]
        [Space(10)]
        [SerializeField] private Text noItemTipText;

        [Space(10)] [HeaderAttribute("ItemList")] [Space(10)]

        [SerializeField] private ComUIListScriptEx shopItemListEx;
        [SerializeField] private GameObject middleLine;

        [Space(10)] [HeaderAttribute("ItemDescription")] [Space(10)]
        [SerializeField] private ChijiShopItemDescriptionController itemDescriptionController;

        #region Init

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();

            _chijiShopItemDataModelList.Clear();
        }

        private void BindUiEvents()
        {
            if (shopItemListEx != null)
            {
                shopItemListEx.Initialize();

                shopItemListEx.onItemVisiable += OnItemVisible;
                shopItemListEx.OnItemUpdate += OnItemUpdate;
                shopItemListEx.OnItemRecycle += OnItemRecycle;
            }


            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveSceneShopItemBuySucceed,
                OnReceiveSceneShopItemBuySucceed);
        }

        private void UnBindUiEvents()
        {
            if (shopItemListEx != null)
            {
                shopItemListEx.onItemVisiable -= OnItemVisible;
                shopItemListEx.OnItemUpdate -= OnItemUpdate;
                shopItemListEx.OnItemRecycle -= OnItemRecycle;
            }


            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveSceneShopItemBuySucceed,
                OnReceiveSceneShopItemBuySucceed);
        }
        


        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_chijiShopItemDataModelList == null
                || _chijiShopItemDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _chijiShopItemDataModelList.Count)
                return;

            var chijiShopItemDataModel = _chijiShopItemDataModelList[item.m_index];
            var chijiShopItem = item.GetComponent<ChijiShopItem>();

            if(chijiShopItemDataModel != null
               && chijiShopItem != null)
                chijiShopItem.InitItem(chijiShopItemDataModel,
                    OnShopItemSelected);
        }



        private void OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var chijiShopItem = item.GetComponent<ChijiShopItem>();
            if(chijiShopItem != null)
                chijiShopItem.UpdateItem();
        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var chijiShopItem = item.GetComponent<ChijiShopItem>();
            if(chijiShopItem != null)
                chijiShopItem.RecycleItem();
        }

        private void OnShopItemSelected(int itemIndex,
            ChijiShopItemDataModel chijiShopItemDataModel,
            ItemData itemData)
        {
            UpdateShopItemDataModel(itemIndex);

            if(shopItemListEx != null)
                shopItemListEx.UpdateElement();

            UpdateItemDescriptionController(chijiShopItemDataModel,
                itemData);
        }
        
        #endregion

        //刷新界面
        public void InitShopItemController(ChijiShopType chijiShopType)
        {

            _chijiShopType = chijiShopType;
            UpdateNoItemTipText();
            
            _defaultChijiShopItemDataModel = null;
            InitShopItemDataModel();

            UpdateShopItemList();

            InitShopItemDescriptionController();

        }
        
        //商店中的商品
        private void InitBuyItemDataModel()
        {
            var shopItemIdList = ChijiShopDataManager.GetInstance().ChijiShopItemIdList;
            
            if (shopItemIdList == null || shopItemIdList.Count <= 0)
                return;

            for (var i = 0; i < shopItemIdList.Count; i++)
            {
                var shopItemId = shopItemIdList[i];
                if (shopItemId <= 0)
                    continue;

                var chijiShopItemTable = ChijiShopUtility.GetChijiShopItemTable(shopItemId);
                if(chijiShopItemTable == null)
                    continue;

                ChijiShopItemDataModel chijiShopItemDataModel = new ChijiShopItemDataModel();
                chijiShopItemDataModel.ChijiShopType = _chijiShopType;
                chijiShopItemDataModel.ItemIndex = i;

                chijiShopItemDataModel.ShopId = ChijiShopDataManager.GetInstance().ChijiShopId;
                chijiShopItemDataModel.ShopItemId = shopItemId;
                chijiShopItemDataModel.ShopItemTable = chijiShopItemTable;

                if (i == 0)
                {
                    chijiShopItemDataModel.IsSelected = true;
                    _defaultChijiShopItemDataModel = chijiShopItemDataModel;
                }
                else
                {
                    chijiShopItemDataModel.IsSelected = false;
                }

                bool isAlreadySoldOver = ChijiShopUtility.IsChijiShopItemAlreadySoldOver(shopItemId);
                chijiShopItemDataModel.IsSoldOver = isAlreadySoldOver;
                
                _chijiShopItemDataModelList.Add(chijiShopItemDataModel);
            }

        }

        //背包中可以出售的商品
        private void InitSellItemDataModel()
        {
            List<ulong> itemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);

            if (itemGuidList != null)
            {
                for (var i = 0; i < itemGuidList.Count; i++)
                {
                    var itemGuid = itemGuidList[i];
                    if (itemGuid <= 0)
                        continue;

                    ChijiShopItemDataModel chijiShopItemDataModel = new ChijiShopItemDataModel();
                    chijiShopItemDataModel.ChijiShopType = _chijiShopType;
                    chijiShopItemDataModel.ItemGuid = itemGuid;
                    chijiShopItemDataModel.ItemIndex = i;

                    if (i == 0)
                    {
                        chijiShopItemDataModel.IsSelected = true;
                        _defaultChijiShopItemDataModel = chijiShopItemDataModel;
                    }
                    else
                    {
                        chijiShopItemDataModel.IsSelected = false;
                    }

                    _chijiShopItemDataModelList.Add(chijiShopItemDataModel);

                }
            }
            
        }

        private void InitShopItemDataModel()
        {
            _chijiShopItemDataModelList.Clear();
            if (_chijiShopType == ChijiShopType.Sell)
            {
                InitSellItemDataModel();
            }
            else
            {
                InitBuyItemDataModel();
            }
        }

        private void UpdateShopItemDataModel(int selectedIndex)
        {
            if (_chijiShopItemDataModelList == null || _chijiShopItemDataModelList.Count <= 0)
                return;

            for (var i = 0; i < _chijiShopItemDataModelList.Count; i++)
            {
                var chijiShopItemDataModel = _chijiShopItemDataModelList[i];
                if(chijiShopItemDataModel == null)
                    continue;

                if (chijiShopItemDataModel.ItemIndex == selectedIndex)
                {
                    chijiShopItemDataModel.IsSelected = true;
                }
                else
                {
                    chijiShopItemDataModel.IsSelected = false;
                }
            }
        }


        private void UpdateShopItemList()
        {
            int count = _chijiShopItemDataModelList.Count;
            if (shopItemListEx != null)
            {
                shopItemListEx.ResetComUiListScriptEx();
                shopItemListEx.SetElementAmount(count);
                if (count == 0)
                {
                    CommonUtility.UpdateGameObjectVisible(middleLine, false);
                }
                else
                {
                    CommonUtility.UpdateGameObjectVisible(middleLine, true);
                }
            }
        }


        private void UpdateNoItemTipText()
        {
            if (noItemTipText == null)
                return;

            var tipStr = TR.Value("Chiji_Shop_Item_Can_Buy_Is_Zero");
            if (_chijiShopType == ChijiShopType.Sell)
            {
                tipStr = TR.Value("Chiji_Shop_Item_Can_Sell_Is_Zero");
            }

            noItemTipText.text = tipStr;
            
        }


        #region ShopItemDescriptionController

        private void InitShopItemDescriptionController()
        {
            if (itemDescriptionController == null)
                return;

            var itemData = ChijiShopUtility.GetItemDataByChijiShopItemDataModel(_defaultChijiShopItemDataModel);

            UpdateItemDescriptionController(_defaultChijiShopItemDataModel, itemData);
        }

        private void UpdateItemDescriptionController(ChijiShopItemDataModel chijiShopItemDataModel,
            ItemData itemData)
        {
            if (itemDescriptionController == null)
                return;

            itemDescriptionController.UpdateItemDescriptionController(chijiShopItemDataModel,
                itemData);
        }

        #endregion

        public void UpdateShopItemContentByGloryCoinChanged()
        {
            if (_chijiShopType == ChijiShopType.Sell)
                return;

            if (itemDescriptionController == null)
                return;
            
            itemDescriptionController.UpdateItemButtonState();

        }

        #region UIEvent

        private void OnReceiveSceneShopItemBuySucceed(UIEvent uiEvent)
        {
            if (_chijiShopType != ChijiShopType.Buy)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;
            
            var shopItemId = (int)uiEvent.Param1;
            if (shopItemId <= 0)
                return;

            if (_chijiShopItemDataModelList == null
                || _chijiShopItemDataModelList.Count <= 0)
                return;

            bool isFind = false;
            for (var i = 0; i < _chijiShopItemDataModelList.Count; i++)
            {
                var shopItemDataModel = _chijiShopItemDataModelList[i];
                if(shopItemDataModel == null)
                    continue;

                if (shopItemDataModel.ShopItemId == shopItemId)
                {
                    shopItemDataModel.IsSoldOver = true;
                    isFind = true;
                    break;
                }
            }

            //存在商品已经购买
            if (isFind == true)
            {
                if (shopItemListEx != null)
                    shopItemListEx.UpdateElement();

                if(itemDescriptionController != null)
                    itemDescriptionController.UpdateItemDealDisplay();
            }

        }

        #endregion

    }
}
