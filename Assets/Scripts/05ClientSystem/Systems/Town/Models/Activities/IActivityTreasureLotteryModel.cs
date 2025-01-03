using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;
using Protocol;

namespace DataModel
{

    public interface IActivityTreasureLotteryModelBase
    {
        //物品的id
        int ItemId { get; }
        string IconPath { get; }
        string Name { get; }
        //彩票的id
        uint LotteryId { get; }
        //彩票的sort id
        int SortId { get; }

        //一个宝物里面包含ItemId这种道具的数量，用于名字显示
        uint ItemNum { get; }
    }

    public interface IActivityTreasureLotteryModel : IActivityTreasureLotteryModelBase
    {

        //每组的item数量
        int TotalNum { get; }
        //当前组剩余的物品数量
        uint LeftNum { get; }
        //剩余的组数
        int LeftGroupNum { get; }
        //总的组数
        int TotalGroup { get;  }
        //玩家下注的数量
        int BoughtNum { get; }

        //每个物品的单价
        int UnitPrice { get; }

        //购买物品需要的货币的id
        int MoneyId { get; }
        ushort GroupId { get; }
        string TimeRemainStr { get; }

        int TimeRemain { get; }

        //补偿的物品
        ItemReward[] Compensation { get; }
        //当前组的状态
        GambingItemStatus State { get; }

    }

    public interface IActivityTreasureLotteryMyLotteryModel : IActivityTreasureLotteryModelBase
    {
        //获奖状态
        GambingMineStatus Status { get; }
        //我获胜的几率
        string WinRate { get; }
        //当前第几组
        int GroupId { get; }

        //我买入的份额的货币总价
        int MyInvestment { get; }

        //货币的名字
        string CurrencyName { get; }

        string WinnerName { get; }

        string WinnerServer { get; }
        //获奖者投入的份额
        int WinnerInvestment { get; }

        //我购买的份额
        int BoughtNum { get; }

        int TotalNum { get; }
        //当前组剩余的物品数量
        int RestNum { get; }

        //所有玩家购买信息
        List<GambingParticipantInfo> AllPlayerInvestInfo { get; }
    }

    public interface IActivityTreasureLotteryHistoryModel : IActivityTreasureLotteryModelBase
    {
        string TimeSoldOut { get; }

        bool IsSellOut { get; }

        bool IsWin { get; }

        string CurrencyName { get; }

        HistroyPlayerData[] PlayerList { get; }
    }

    public interface IActivityTreasureLotteryDrawModel
    {
        string WinnerName { get; }
        string WinnerRate { get; }
        string ServerName { get; }
        string PlatformName { get; }

        bool IsPlayerWin { get;}

        int ItemId { get; }
        DrawWinnerData[] TopFiveInvestPlayers { get; }
    }

}