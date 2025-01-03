using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //商店的助手类
    public static class ShopNewUtility
    {

        //关闭商店的界面
        public static void OnCloseShopNewFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<ShopNewFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ShopNewFrame>();
        }

        public static void OnCloseShowNewPurchaseItemFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ShopNewPurchaseItemFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ShopNewPurchaseItemFrame>();
        }

        //得到兑换商店中的活动商店的相关信息
        public static OpActivityData GetActivityShopActivityData(int activityId)
        {
            var activityShopActivityData = ActivityDataManager.GetInstance().GetLimitTimeActivityData((uint)activityId);

            return activityShopActivityData;
        }

        //判断商店是否为运营活动商店；
        //活动商店是否处在开启状态
        //活动商店并且是处在开启的状态才返回true
        public static bool IsActivityShop(ShopTable shopTable)
        {
            if (shopTable == null)
                return false;

            if (shopTable.ShopKind != ShopTable.eShopKind.SK_Activity)
            {
                return false;
            }

            return true;
        }

        //活动商店是否处在开启状态
        public static bool IsActivityShopInStartState(ShopTable shopTable)
        {

            var activityId = shopTable.Param1;

            var activityShopActivityData = GetActivityShopActivityData(activityId);

            if (activityShopActivityData != null &&
                (OpActivityState) activityShopActivityData.state == OpActivityState.OAS_IN)
                return true;

            return false;
        }

        //活动商店的起止时间信息
        public static string GetActivityShopTimeData(ShopTable shopTable)
        {
            var activityId = shopTable.Param1;

            var activityShopActivityData = GetActivityShopActivityData(activityId);

            if (activityShopActivityData == null)
                return "";

            return Function.GetTimeWithoutYearNoZero((int) activityShopActivityData.startTime,
                (int) activityShopActivityData.endTime);
        }

        //得到某一个货币的等价物
        public static List<int> GetShopCostItemEqualItemIdListByOneItem(int costItemId)
        {
            EqualItemTable equalTable = TableManager.GetInstance().GetTableItem<EqualItemTable>(costItemId);
            if (equalTable == null)
                return null;

            return equalTable.EqualItemIDs.ToList();
        }

        //得到若干个货币的等价物
        public static List<int> GetShopCostItemEqualItemIdListByItemList(List<ShopNewCostItemDataModel> itemList)
        {
            List<int> equalItemIdList = new List<int>();
            if (itemList == null || itemList.Count <= 0)
                return equalItemIdList;

            for (var i = 0; i < itemList.Count; i++)
            {
                var costItemDataModel = itemList[i];
                if(costItemDataModel == null)
                    continue;

                var equalTable = TableManager.GetInstance().GetTableItem<EqualItemTable>(costItemDataModel.CostItemId);
                if(equalTable == null || equalTable.EqualItemIDs == null 
                   || equalTable.EqualItemIDs.Count <= 0)
                    continue;

                equalItemIdList.AddRange(equalTable.EqualItemIDs.ToList());
            }

            if (equalItemIdList.Count > 1)
            {
                //去重
                return equalItemIdList.Distinct();
            }
            else
            {
                return equalItemIdList;
            }
        }

        //由字符串解析出其他消耗品的信息
        public static List<ShopNewCostItemDataModel> GetShopItemOtherCostItemDataModelList(string otherCostItems)
        {

            var costItemStrArray = otherCostItems.Split(',');

            if (costItemStrArray.Length <= 0)
                return null;


            List<ShopNewCostItemDataModel> otherCostItemDataModelList = new List<ShopNewCostItemDataModel>();

            for (var i = 0; i < costItemStrArray.Length; i++)
            {
                var costItemStr = costItemStrArray[i];
                if(string.IsNullOrEmpty(costItemStr) == true)
                    continue;

                var costItemIdNumberArray = costItemStr.Split('_');
                if(costItemIdNumberArray.Length != 2)
                    continue;

                int itemId = 0;
                int itemNumber = 0;

                if((int.TryParse(costItemIdNumberArray[0], out itemId) == false)
                    || (int.TryParse(costItemIdNumberArray[1], out itemNumber)) == false)
                    continue;

                if(itemId <= 0 || itemNumber <= 0)
                    continue;

                ShopNewCostItemDataModel otherCostItemDataModel = new ShopNewCostItemDataModel();
                otherCostItemDataModel.CostItemId = itemId;
                otherCostItemDataModel.CostItemNumber = itemNumber;
                otherCostItemDataModelList.Add(otherCostItemDataModel);

            }

            return otherCostItemDataModelList;
        }

        //其他消耗品可以购买的数量
        public static int ShopNewBuyItemNumberByOtherCostItem(int otherCostItemId, 
            int otherCostItemNumber,
            int discountValue = 0)
        {
            //拥有的数量
            var otherCostOwnerNumber = ItemDataManager.GetInstance().GetOwnedItemCount(otherCostItemId);

            //检测折扣（消耗数量）
            otherCostItemNumber = GetRealCostValue(otherCostItemNumber, discountValue);

            //消耗数量的限制
            if (otherCostItemNumber <= 0)
                return 0;

            return otherCostOwnerNumber / otherCostItemNumber;
        }


        //根据折扣得到实际价格
        public static int GetRealCostValue(int costValue, int discountValue)
        {
            if (costValue <= 0)
                return 0;

            //折扣在1-99之间
            if (discountValue > 0 && discountValue < 100)
            {
                costValue = Mathf.CeilToInt(discountValue / 100.0f * costValue);
            }

            return costValue;
        }

        //得到折扣的字符串：7折或者7.5折
        public static string GetDiscountStr(int discountValue)
        {
            if (discountValue > 0 && discountValue < 100)
            {
                var lastNumber = discountValue % 10;
                var firstNumber = discountValue / 10;
                if (lastNumber == 0)
                {
                    //整数，3折
                    var discountStr = TR.Value("shop_item_discount_normal_format",
                        firstNumber);
                    return discountStr;
                }
                else
                {
                    //小数：7.5折
                    var specialValue = discountValue / 10.0f;
                    var discountStr = TR.Value("shop_item_discount_special_format",
                        specialValue);
                    return discountStr;
                }
                
            }

            return "";
        }

    }
}