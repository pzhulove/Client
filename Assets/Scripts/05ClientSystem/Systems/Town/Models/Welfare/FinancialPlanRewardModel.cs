using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class FinancialPlanRewardModel
    {
        public int Index = 1;
        public int Id = 0;

        public int LevelLimit = 0;
        //其实只需要一个就可以了。。。
        public List<ItemSimpleData> RewardItemList = new List<ItemSimpleData>();

        //保存真实的状态（服务器同步的数据）
        public FinancialPlanState RewardState = FinancialPlanState.Invalid;
        //保存展示的状态（展示在view中的数据）
        public FinancialPlanState ShowRewardState = FinancialPlanState.Invalid;

        public int GetRewardItemId()
        {
            if (RewardItemList.Count > 0)
            {
                return RewardItemList[0].ItemID;
            }

            return 0;
        }

        public int GetRewardItemCount()
        {
            if (RewardItemList.Count > 0)
            {
                return RewardItemList[0].Count;
            }

            return 0;
        }

        public void PrintRewardModel()
        {
        }

    }
}