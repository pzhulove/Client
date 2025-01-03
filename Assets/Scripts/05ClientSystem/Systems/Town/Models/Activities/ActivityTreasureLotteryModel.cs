using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;
//using BehaviorDesigner.Runtime.Tasks;
using Protocol;

namespace DataModel
{
    public abstract class ActivityTreasureLotteryModelBase : IActivityTreasureLotteryModelBase
    {
        public string IconPath { get; private set; }

        public int ItemId { get; private set; }

        public uint ItemNum { get; private set; }

        public uint LotteryId { get; private set; }

        public string Name { get; private set; }

        public int SortId { get; private set; }

        public ActivityTreasureLotteryModelBase(int itemId, uint itemNum, uint lotteryId, int sortId)
        {
            ItemId = itemId;
            ProtoTable.ItemTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemId);
            IconPath = "";
            Name = "";
            if (tableData != null)
            {
                IconPath = tableData.Icon;
                Name = tableData.Name;
            }
            ItemNum = itemNum;
            LotteryId = lotteryId;
            SortId = sortId;
        }
    }

    /// <summary>
    /// 夺宝活动数据模型
    /// </summary>
    public sealed class ActivityTreasureLotteryModel : ActivityTreasureLotteryModelBase, IActivityTreasureLotteryModel
    {
        //每组的item数量
        public int TotalNum { get; private set; }
        //剩余的组数
        public int LeftGroupNum { get; private set; }

        public ushort GroupId { get; private set; }

        //总的组数
        public int TotalGroup { get; private set; }
        //当前组剩余的物品数量
        public uint LeftNum { get; private set; }

        //是否已关闭
        public GambingItemStatus State { get; private set; }

        public int BoughtNum { get; private set; }

        public int UnitPrice { get; private set; }

        public int MoneyId { get; private set; }

        public ItemReward[] Compensation { get; private set; }
        public string TimeRemainStr
        {
            get
            {
                return Function.SetShowLeftTime((int)_sellBeginTime);
            }
        }

        public int TimeRemain
        {
            get
            {
                return (int)_sellBeginTime - (int)TimeManager.GetInstance().GetServerTime();
            }
        }


        uint _sellBeginTime;
#if ACTIVITY_TEST
        public ActivityTreasureLotteryModel(int id, int numberPerGroup, int totalGroup, uint leftNum, int leftGroupNum, GambingItemStatus state, int unitPrice, int moneyLinkId, int boughtNum, ushort groupId, uint lotteryId, float beginTime = 0)
        {
            ProtoTable.ItemTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
            ItemId = id;
            TotalNum = numberPerGroup;
            LeftNum = leftNum;
            TotalGroup = totalGroup;
            LeftGroupNum = leftGroupNum;
            Name = tableData.Name;
            IconPath = tableData.Icon;
            State = state;
            UnitPrice = unitPrice;
            MoneyId = moneyLinkId;
            BoughtNum = boughtNum;
            _sellBeginTime = beginTime;
            GroupId = groupId;
            LotteryId = lotteryId;
            SortId = (int)lotteryId;
            ItemNum = 999;
            Compensation = new ItemReward[0];
        }
#endif

        public ActivityTreasureLotteryModel(GambingItemInfo itemInfo) : 
            base((int)itemInfo.itemDataId, itemInfo.gambingItemNum, itemInfo.gambingItemId, itemInfo.sortId)
        {
            TotalNum = (int)itemInfo.totalCopiesOfCurGroup;
            LeftNum = itemInfo.totalCopiesOfCurGroup - itemInfo.soldCopiesInCurGroup;
            TotalGroup = itemInfo.totalGroups;
            State = (GambingItemStatus)itemInfo.statusOfCurGroup;
            UnitPrice = (int)itemInfo.unitPrice;
            MoneyId = (int)itemInfo.costMoneyId;
            BoughtNum = (int)itemInfo.mineGambingInfo.investCopies;
            _sellBeginTime = itemInfo.sellBeginTime;
            LeftGroupNum = itemInfo.restGroups;
            if (State == GambingItemStatus.GIS_SELLING || State == GambingItemStatus.GIS_PREPARE)
            {
                LeftGroupNum += 1;
            }
            GroupId = itemInfo.curGroupId;
            Compensation = itemInfo.rewardsPerCopy;
        }
    }

    /// <summary>
    /// 我的夺宝数据
    /// </summary>
    public sealed class ActivityTreasureLotteryMyLotteryModel : ActivityTreasureLotteryModelBase, IActivityTreasureLotteryMyLotteryModel
    {
        public int BoughtNum { get; private set; }

        public string CurrencyName { get; private set; }

        public int GroupId { get; private set; }

        public GambingMineStatus Status { get; private set; }

        public int RestNum { get; private set; }

        public int MyInvestment { get; private set; }

        public int TotalNum { get; private set; }

        public int WinnerInvestment { get; private set; }

        public string WinnerName { get; private set; }

        public string WinnerServer { get; private set; }

        //玩家自己的胜率
        public string WinRate { get; private set; }

        public List<GambingParticipantInfo> AllPlayerInvestInfo { get; private set; }

#if ACTIVITY_TEST
        public ActivityTreasureLotteryMyLotteryModel(int id, int boughtNum, int groupId, bool isWin, int leftNum, int myInvestment, int totalNum, int moneyLinkId, int winnerInvestment, string winnerName, float winRate)
        {
            ItemId = id;
            ProtoTable.ItemTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
            Name = tableData.Name;
            IconPath = tableData.Icon;
            BoughtNum = boughtNum;
            GroupId = groupId;
            Status = GambingMineStatus.GMS_SUCCESS;
            RestNum = leftNum;
            MyInvestment = myInvestment;
            TotalNum = totalNum;
            try
            {
                CurrencyName = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(moneyLinkId).Name;
            }
            catch
            {
                CurrencyName = "未知";
            }
            WinnerInvestment = winnerInvestment;
            WinnerName = winnerName;
            WinRate = winRate;
            AllPlayerInvestInfo = null;
            LotteryId = 0;
            ItemNum = 999;
            SortId = (int)LotteryId;
        }
#endif
        public ActivityTreasureLotteryMyLotteryModel(GambingMineInfo info) :
            base((int)info.itemDataId, info.gambingItemNum, info.gambingItemId, info.sortId)
        {
            BoughtNum = (int)info.mineGambingInfo.investCopies;
            GroupId = info.groupId;
            Status = (GambingMineStatus)info.mineGambingInfo.status;
            RestNum = (int)(info.totalCopies - info.soldCopies);
            MyInvestment = (int)info.mineGambingInfo.investMoney;
            TotalNum = (int)info.totalCopies;
            WinnerInvestment = (int)info.gainersGambingInfo.investMoney;
            WinnerServer = info.gainersGambingInfo.participantServerName;
            WinnerName = info.gainersGambingInfo.participantName;
            WinRate = info.mineGambingInfo.gambingRate;
            AllPlayerInvestInfo = new List<GambingParticipantInfo>();
            AllPlayerInvestInfo.AddRange(info.participantsGambingInfo);

            CurrencyName = "";
            var currencyData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)info.mineGambingInfo.investMoneyId);
            if (currencyData != null)
            {
                CurrencyName = currencyData.Name;
            }
            else
            {
                Logger.LogError("错误的moneyId :" + info.mineGambingInfo.investMoneyId);
            }
        }

    }

    /// <summary>
    /// 夺宝记录数据
    /// </summary>
    public sealed class ActivityTreasureLotteryHistoryModel : ActivityTreasureLotteryModelBase, IActivityTreasureLotteryHistoryModel
    {
        public string CurrencyName { get; private set; }

        //是否所有组都售完
        public bool IsSellOut { get; private set; }

        public HistroyPlayerData[] PlayerList { get; private set; }

        public bool IsWin { get; private set; }

        public string TimeSoldOut { get; private set; }

#if ACTIVITY_TEST
        public ActivityTreasureLotteryHistoryModel(int id, bool isWin, GambingGroupRecordData[] playerList, int soldOutTimestamp, int lotteryId)
        {
            ItemId = id;
            var tableData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(ItemId);
            PlayerList = new HistroyPlayerData[playerList.Length];
            IsWin = isWin;
            for (int i = 0; i < playerList.Length; ++i)
            {
                if (!IsWin && playerList[i].gainerId == PlayerBaseData.GetInstance().RoleID)
                {
                    IsWin = true;
                }
                PlayerList[i] = new HistroyPlayerData(playerList[i].gainerName, playerList[i].groupId, playerList[i].investCurrencyNum, false);
            }
            IsSellOut = soldOutTimestamp != 0;
            LotteryId = (uint)lotteryId;
            SortId = lotteryId;
            TimeSoldOut = Function.GetShortTimeString(soldOutTimestamp);
            try
            {
                CurrencyName = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)playerList[0].investCurrencyId).Name;
            }
            catch
            {
                CurrencyName = "未知";
            }
            Name = tableData.Name;
            IconPath = tableData.Icon;
            ItemNum = 999;
        }
#endif
        public ActivityTreasureLotteryHistoryModel(GambingItemRecordData data) :
            base((int)data.itemDataId, data.gambingItemNum, data.gambingItemId, data.sortId)
        {
            PlayerList = new HistroyPlayerData[data.groupRecordData.Length];
            IsWin = false;
            for (int i = 0; i < data.groupRecordData.Length; ++i)
            {
                bool isSamePlatform = String.Compare(SDKInterface.Instance.GetPlatformNameByChannel(), data.groupRecordData[i].gainerENPlatform, StringComparison.Ordinal) == 0;
                if (!IsWin && isSamePlatform && data.groupRecordData[i].gainerId == PlayerBaseData.GetInstance().RoleID)
                {
                    IsWin = true;
                }

                bool isPlayer = data.groupRecordData[i].gainerId == PlayerBaseData.GetInstance().RoleID && isSamePlatform;
                PlayerList[i] = new HistroyPlayerData(data.groupRecordData[i].gainerName, data.groupRecordData[i].gainerServerName, data.groupRecordData[i].gainerPlatform, data.groupRecordData[i].groupId, data.groupRecordData[i].investCurrencyNum, isPlayer);
            }
            IsSellOut = data.soldOutTimestamp != 0;

            TimeSoldOut = Function.GetShortTimeString((int)data.soldOutTimestamp);
            CurrencyName = "";
            if (data.groupRecordData.Length > 0)
            {
                var currencyData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)data.groupRecordData[0].investCurrencyId);
                if (currencyData != null)
                {
                    CurrencyName = currencyData.Name;
                }
                else
                {
                    Logger.LogError("服务端传来的investCurrencyId 在ItemTable表中找不到，investCurrencyId为:" + data.groupRecordData[0].investCurrencyId);
                }
            }
        }
    }

    public struct HistroyPlayerData
    {
        public string Name { get; private set; }
        public string ServerName { get; private set; }
        public string PlatformName { get; private set; }
        public int GroupId { get; private set; }
        public uint TotalInvestment { get; private set; }

        public bool IsPlayer { get; private set; }

        public HistroyPlayerData(string name, string serverName, string platformName, int groupId, uint investment, bool isPlayer) : this()
        {
            Name = name;
            ServerName = serverName;
            PlatformName = platformName;
            GroupId = groupId;
            TotalInvestment = investment;
            IsPlayer = isPlayer;
        }
    }

    public struct DrawWinnerData
    {
        public DrawWinnerData(string rate, string name, string serverName, string platformName) : this()
        {
            this.Rate = rate;
            ServerName = serverName;
            Name = name;
            PlatformName = platformName;
        }

        public DrawWinnerData(GambingParticipantInfo info) : this()
        {
            this.Rate = info.gambingRate;
            Name = info.participantName;
            ServerName = info.participantServerName;
            PlatformName = info.participantPlatform;
        }

        public string Name { get; private set; }
        public string ServerName { get; private set; }
        public string PlatformName { get; private set; }
        public string Rate { get; private set; }


    }

    public class ActivityTreasureLotteryDrawModel : IActivityTreasureLotteryDrawModel
    {
        public string WinnerName { get; private set; }

        public string WinnerRate { get; private set; }

        public string ServerName { get; private set; }

        public string PlatformName { get; private set; }

        public DrawWinnerData[] TopFiveInvestPlayers { get; private set; }

        public bool IsPlayerWin { get; private set; }

        public int ItemId { get; private set; }

        public ActivityTreasureLotteryDrawModel(GambingParticipantInfo winner, GambingParticipantInfo[] topFiveInvestPlayers)
        {
            WinnerName = winner.participantName;
            ServerName = winner.participantServerName;
            PlatformName = winner.participantPlatform;
            WinnerRate = winner.gambingRate;
            TopFiveInvestPlayers = new DrawWinnerData[topFiveInvestPlayers.Length];
            for (int i = 0; i < topFiveInvestPlayers.Length; ++i)
            {
                TopFiveInvestPlayers[i] = new DrawWinnerData(topFiveInvestPlayers[i]);
                
            }

            IsPlayerWin = winner.participantId == PlayerBaseData.GetInstance().RoleID && String.Compare(SDKInterface.Instance.GetPlatformNameByChannel(), winner.participantENPlatform, StringComparison.Ordinal) == 0;
            ItemId = ActivityTreasureLotteryDataManager.GetInstance().GetItemIdByLotteryId((int)winner.gambingItemId);
        }

    }
}