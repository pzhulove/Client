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

    public enum BoxOpenType
    {
        None = 0,
        Preparing = 1,      //准备中
        Opening = 2,        //开罐中
        Finished = 3,       //罐子已经打开
    }

    //周签到的类型：
    //新人周签到和角色周签到
    public enum WeekSignInType
    {
        None = 0,
        NewPlayerWeekSignIn = 1,    //新人周签到
        ActivityWeekSignIn = 2,     //活动周签到
    }


    //奖励预览的数据模型
    public class WeekSignInPreviewAwardDataModel
    {
        public int OpActType;
        public int ItemId;                  //道具ID
        public int ItemNumber;              //道具的数量
        public int SortId;
        public bool IsSpecialAward;         //是否为大奖
    }

    //任务奖励的状态
    public enum WeekSignInAwardState
    {
        None = 0,
        Received = 1,           //已经领取
        Finished = 2,           //已经完成，可以领取
        UnFinished = 3,         //未完成,不可领取
    }

    //周签到奖励的数据模型，与服务器相关
    public class WeekSignInAwardDataModel
    {
        public WeekSignInType WeekSignInType;

        public uint AlreadySignInWeek;           //已经签到的周数
        public List<uint> AlreadyReceiveWeekList = new List<uint>();    //已经领取的周数
        public List<WeekSignSumTable> WeekSignSumTableList = new List<WeekSignSumTable>();  //表中配置的周数

        public List<WeekSignBox> WeekSignInBoxItemList = new List<WeekSignBox>();   //任务开箱后的奖励

        public void Reset()
        {
            WeekSignInType = WeekSignInType.None;
            AlreadySignInWeek = 0;

            AlreadyReceiveWeekList.Clear();
            WeekSignSumTableList.Clear();
            WeekSignInBoxItemList.Clear();
        }

        //累计奖励是否可以领取
        public bool IsTotalAwardCanReceived()
        {
            if (WeekSignSumTableList == null || WeekSignSumTableList.Count <= 0)
                return false;

            for (var i = 0; i < WeekSignSumTableList.Count; i++)
            {
                var curWeekSignSumTable = WeekSignSumTableList[i];
                if(curWeekSignSumTable == null)
                    continue;

                //当前周数大于累计周数
                if(curWeekSignSumTable.weekSum > AlreadySignInWeek)
                    continue;

                //当前周数小于累计周数
                //已领取的列表不存在
                if (AlreadyReceiveWeekList == null || AlreadyReceiveWeekList.Count <= 0)
                    return true;

                //在已领取的列表中查找
                bool isAlreadyReceived = false;
                for (var j = 0; j < AlreadyReceiveWeekList.Count; j++)
                {
                    if (AlreadyReceiveWeekList[j] == curWeekSignSumTable.weekSum)
                    {
                        //找到，说明已经领取
                        isAlreadyReceived = true;
                    }
                }
                //当前周数没有领取，直接返回可以领取
                if (isAlreadyReceived == false)
                    return true;

            }
            return false;
        }
    }
}