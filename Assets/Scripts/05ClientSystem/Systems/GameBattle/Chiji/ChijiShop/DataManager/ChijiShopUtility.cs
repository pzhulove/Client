using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //吃鸡商店助手
    public static class ChijiShopUtility
    {

        //判断某个商品是否已经购买了
        public static bool IsChijiShopItemAlreadySoldOver(int shopItemId)
        {
            if (ChijiShopDataManager.GetInstance().ChijiAlreadyBuyShopItemIdList == null
                || ChijiShopDataManager.GetInstance().ChijiAlreadyBuyShopItemIdList.Count <= 0)
                return false;

            for (var i = 0; i < ChijiShopDataManager.GetInstance().ChijiAlreadyBuyShopItemIdList.Count; i++)
            {
                var curShopItemId = ChijiShopDataManager.GetInstance().ChijiAlreadyBuyShopItemIdList[i];
                if (curShopItemId == shopItemId)
                    return true;
            }

            return false;
        }

        //商店是否可以自动刷新
        public static bool IsChijiShopCanAutoRefresh()
        {
            if (ChijiShopDataManager.GetInstance().ChijiShopRefreshTimeStamp <= 0)
                return false;

            return true;
        }

        //吃鸡商店自动刷新时间到了
        public static bool IsChijiShopRefreshTimeUp()
        {
            //时间戳大于0
            if (ChijiShopDataManager.GetInstance().ChijiShopRefreshTimeStamp <= 0)
                return false;

            //时间没有到——
            if (TimeManager.GetInstance().GetServerTime() <
                ChijiShopDataManager.GetInstance().ChijiShopRefreshTimeStamp)
                return false;

            return true;
            
        }

        //判断是否可以刷新商店
        public static bool IsCanRefreshChijiShopByGloryCoin()
        {
            if (GetCurrentOwnerGloryCoinNumber() >= ChijiShopDataManager.GetInstance().ChijiShopRefreshCostValue)
                return true;

            return false;
        }

        public static ChiJiShopItemTable GetChijiShopItemTable(int shopItemId)
        {
            var chijiShopItemTable = TableManager.GetInstance().GetTableItem<ChiJiShopItemTable>(
                shopItemId);

            return chijiShopItemTable;
        }

        //吃鸡商店购买商品
        public static void OnBuyItemInChijiScene(ChijiShopItemDataModel shopItemDataModel)
        {
            if (shopItemDataModel == null)
                return;

            if (shopItemDataModel.ShopItemTable == null)
                return;

            var shopItemTable = shopItemDataModel.ShopItemTable;

            var shopCostItemId = shopItemTable.CostItemID;
            var shopCostItemNumber = shopItemTable.CostNum;

            var shopCostItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(shopCostItemId);
            if (shopCostItemTable == null)
                return;

            var itemId = shopItemTable.ItemID;
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            if (itemTable == null)
                return;
            
            var itemName = itemTable.Name;
            var shopCostItemName = shopCostItemTable.Name;

            var buyItemTipStr = TR.Value("Chiji_Shop_Buy_Item_Tip",
                shopCostItemNumber,
                shopCostItemName,
                itemName);
            
            CommonUtility.OnShowCommonMsgBox(
                buyItemTipStr,
                () => { BuyItem(shopItemDataModel); },
                TR.Value("Chiji_Shop_Item_Buy_Label"));

        }

        private static void BuyItem(ChijiShopItemDataModel chijiShopItemDataModel)
        {
            if (chijiShopItemDataModel == null)
                return;

            ChijiShopDataManager.GetInstance().OnSendBuyShopItemReq(chijiShopItemDataModel.ShopId,
                chijiShopItemDataModel.ShopItemId);
            
        }

        //吃鸡商店出售商品
        public static void OnSellItemInChijiScene(ItemData itemData)
        {
            if (itemData == null)
                return;
            
            var sellItemId = itemData.PriceItemID;
            var sellItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(sellItemId);
            if (sellItemTable == null)
            {
                return;
            }

            //var itemName = itemData.Name;
            //var sellItemName = sellItemTable.Name;
            //var sellItemPrice = itemData.Price;

            //var sellItemTipStr = TR.Value("Chiji_Shop_Sell_Item_Tip",
            //    itemName, sellItemPrice, sellItemName);

            //CommonUtility.OnShowCommonMsgBox(
            //    sellItemTipStr,
            //    () => { SellItem(itemData); },
            //    TR.Value("tip_sell"));

            //直接出售，没有二次弹窗
            SellItem(itemData);

        }

        private static void SellItem(ItemData itemData)
        {
            if (itemData == null)
                return;

            ItemDataManager.GetInstance().SellItem(itemData, 1);

            ItemTipManager.GetInstance().CloseAll();
        }

        public static ItemData GetItemDataByChijiShopItemDataModel(ChijiShopItemDataModel chijiShopItemDataModel)
        {
            if (chijiShopItemDataModel == null)
                return null;

            ItemData itemData = null;

            if (chijiShopItemDataModel.ChijiShopType == ChijiShopType.Sell)
            {
                itemData = ItemDataManager.GetInstance().GetItem(chijiShopItemDataModel.ItemGuid);
            }
            else
            {
                if (chijiShopItemDataModel.ShopItemTable != null)
                    itemData = ItemDataManager.CreateItemDataFromTable(chijiShopItemDataModel.ShopItemTable.ItemID);
            }

            return itemData;
        }

        //得到当前拥有的荣耀币数量
        public static int GetCurrentOwnerGloryCoinNumber()
        {
            //使用Counter数值
            var itemNumber = CountDataManager.GetInstance().GetCount(ChijiShopDataManager.GloryCoinCounterStr);
            
            return itemNumber;
        }


        //道具描述的详情,默认显示道具的名字
        public static string GetItemDetailStr(ItemData itemData, bool isNeedItemName = true)
        {
            if (itemData == null)
                return "";

            List<string> detailStrList = new List<string>();

            //是否添加名字
            if (isNeedItemName == true)
                detailStrList.Add(itemData.GetColorName());

            //基础属性
            var baseMainPropStrList = itemData.GetBaseMainPropDescs();
            if (baseMainPropStrList != null && baseMainPropStrList.Count > 0)
                detailStrList.AddRange(baseMainPropStrList.ToList());

            //四维属性
            var fourAttrStrList = itemData.GetFourAttrAndResistMagicDescs();
            if (fourAttrStrList != null && fourAttrStrList.Count > 0)
                detailStrList.AddRange(fourAttrStrList.ToList());

            //随机属性
            var randomAttrStrList = itemData.GetRandomAttrDescs();
            if (randomAttrStrList != null && randomAttrStrList.Count > 0)
                detailStrList.AddRange(randomAttrStrList.ToList());

            //固有附加属性
            var attachAttrStrList = itemData.GetAttachAttrDescs();
            if (attachAttrStrList != null && attachAttrStrList.Count > 0)
                detailStrList.AddRange(attachAttrStrList.ToList());

            //固有复杂属性
            var complexAttrStrList = itemData.GetComplexAttrDescs();
            if (complexAttrStrList != null && complexAttrStrList.Count > 0)
                detailStrList.AddRange(complexAttrStrList.ToList());

            //未开放属性
            var masterAttrStrList = itemData.GetMasterAttrDescs();
            if (masterAttrStrList != null && masterAttrStrList.Count > 0)
                detailStrList.AddRange(masterAttrStrList.ToList());

            //道具描述
            var itemDescriptionStr = itemData.GetDescription();
            detailStrList.Add(itemDescriptionStr);

            if (detailStrList.Count <= 0)
                return "";

            var showDetailStr = "";

            for (var i = 0; i < detailStrList.Count; i++)
            {
                showDetailStr += detailStrList[i] + "\n";
            }

            return showDetailStr;

        }

    }
}
