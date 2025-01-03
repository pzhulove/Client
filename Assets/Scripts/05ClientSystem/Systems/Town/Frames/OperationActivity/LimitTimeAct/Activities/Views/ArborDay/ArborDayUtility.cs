using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //植树节助手
    public static class ArborDayUtility
    {

        //得到自己的种子数目
        public static int GetTreeSeedOwnerNumber(int seedId)
        {
            var itemNumber = ItemDataManager.GetInstance().GetOwnedItemCount(
                seedId);

            return itemNumber;
        }
        
        //种植树木需要的种子数量
        public static int GetTreeSeedNeedNumber()
        {
            return 1;
        }
        
        public static int GetCounterValueByCounterStr(string counterStr)
        {
            int counterValue = CountDataManager.GetInstance().GetCount(counterStr);

            return counterValue;
        }

        #region NetEvents

        public static void OnReceiveArborDayRewardItem(int itemId)
        {

        }

        #endregion

        public static List<int> GetRewardItemList()
        {
            return null;
        }

        public static void OnOpenRewardReviewFrame(int firstRewardItemId = 0, int secondRewardItemId = 0)
        {

            var preViewDataModel = new PreViewDataModel();

            if (firstRewardItemId > 0)
            {
                var firstItemData = new PreViewItemData()
                {
                    itemId = firstRewardItemId,
                };
                preViewDataModel.preViewItemList.Add(firstItemData);
            }

            if (secondRewardItemId > 0)
            {
                var secondItemData = new PreViewItemData()
                {
                    itemId = secondRewardItemId,
                };
                preViewDataModel.preViewItemList.Add(secondItemData);
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<PreviewModelFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<PreviewModelFrame>();
            ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, preViewDataModel);
        }

        //得到某个奖励对应物品的名字
        public static int GetRewardItemId(ILimitTimeActivityTaskDataModel taskDataModel)
        {
            var rewardItemId = 0;
            if (taskDataModel.AwardDataList != null && taskDataModel.AwardDataList.Count == 1)
            {
                var firstRewardData = taskDataModel.AwardDataList[0];
                if (firstRewardData != null)
                {
                    rewardItemId = (int)firstRewardData.id;
                }
            }

            return rewardItemId;
        }

        public static string GetRewardItemName(int rewardItemId)
        {
            
            var rewardItemName = "";
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(rewardItemId);
            rewardItemName = CommonUtility.GetItemColorName(itemTable);
            return rewardItemName;
        }

    }
}
