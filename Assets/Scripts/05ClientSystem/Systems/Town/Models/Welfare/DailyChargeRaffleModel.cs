using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using GameClient;

public class DailyChargeRaffleModel
{
    //对应活动表 任务
    public int Id = 0;
    public int SortIndex = 1;                                                               //排序序号
    public List<ItemSimpleData> AwardItemDataList = new List<ItemSimpleData>();
    public int RaffleTableId = 0;                                                           //抽奖表ID

    //对应抽奖表
    public ProtoTable.DrawPrizeTable.eRaffleTicketType RaffleTicketType 
        = ProtoTable.DrawPrizeTable.eRaffleTicketType.RaffleTicketType_None;                //抽奖券类型

    //对应充值商城表
    public int ChargeItemId;
    public int ChargePrice;
    public ChargeMallType ChargeMallType = ChargeMallType.DayChargeWelfare;

    //动态数据
    public TaskStatus Status = TaskStatus.TASK_INIT;

    public int accLimitChargeNum = 0;                                                           //当前购买次数
    public int accLimitChargeMax = 0;                                                           //限购次数

    public void Clear()
    {
        Id = 0;
        SortIndex = 1;
        if (AwardItemDataList != null)
        {
            AwardItemDataList.Clear();
        }
        RaffleTableId = 0;
        ChargeItemId = -1;
        ChargePrice = 1;
        ChargeMallType = Protocol.ChargeMallType.DayChargeWelfare;

        RaffleTicketType = ProtoTable.DrawPrizeTable.eRaffleTicketType.RaffleTicketType_None;

        Status = TaskStatus.TASK_INIT;
        accLimitChargeNum = 0;
		accLimitChargeMax = 0;
    }
}
