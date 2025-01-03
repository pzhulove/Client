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
    //周签到助手类
    public static class WeekSignInUtility
    {

        //打开罐子抽奖界面
        public static void OpenBoxOpenFrame(WeekSignInType weekSignInType,
            int awardItemId,
            int awardItemNumber)
        {
            BoxDataModel boxDataModel = new BoxDataModel();

            List<WeekSignInPreviewAwardDataModel> previewAwardDataModelList = null;
            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                boxDataModel.BoxModelPath = WeekSignInDataManager.NewPlayerTaskAwardBoxEffectPath;
                previewAwardDataModelList = WeekSignInDataManager.GetInstance()
                    .GetPreviewAwardItemDataModelListByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn);
            }
            else
            {
                boxDataModel.BoxModelPath = WeekSignInDataManager.ActivityTaskAwardBoxEffectPath;
                previewAwardDataModelList = WeekSignInDataManager.GetInstance()
                    .GetPreviewAwardItemDataModelListByWeekSignInType(WeekSignInType.ActivityWeekSignIn);
            }

            for (var i = 0; i < previewAwardDataModelList.Count; i++)
            {
                var commonNewItemDataModel = new CommonNewItemDataModel()
                {
                    ItemId = previewAwardDataModelList[i].ItemId,
                    ItemCount = previewAwardDataModelList[i].ItemNumber,
                };
                boxDataModel.CommonNewItemDataModelList.Add(commonNewItemDataModel);

                //开出的奖励是否为大奖
                if (previewAwardDataModelList[i].IsSpecialAward == true)
                {
                    if (previewAwardDataModelList[i].ItemId == awardItemId)
                    {
                        boxDataModel.IsSpecialAward = true;
                    }
                }
            }

            //罐子打开之后奖励的数据
            boxDataModel.AwardItemData = new CommonNewItemDataModel()
            {
                ItemId = awardItemId,
                ItemCount = awardItemNumber,
            };
            

            OpenBoxOpenFrame(boxDataModel);
        }

        public static void OpenBoxOpenFrame(BoxDataModel boxDataModel)
        {
            CloseBoxOpenFrame();
            ClientSystemManager.GetInstance().OpenFrame<OpenBoxFrame>(FrameLayer.Middle, boxDataModel);
        }


        public static void CloseBoxOpenFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<OpenBoxFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<OpenBoxFrame>();
        }

        public static void OpenActiveFrameByNewPlayerWeekSignIn()
        {
            OpenActiveFrameByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn);
        }

        public static void OpenActiveFrameByActivityWeekSignIn()
        {
            OpenActiveFrameByWeekSignInType(WeekSignInType.ActivityWeekSignIn);
        }

        private static void OpenActiveFrameByWeekSignInType(WeekSignInType weekSignInType)
        {
            if (weekSignInType != WeekSignInType.ActivityWeekSignIn &&
                weekSignInType != WeekSignInType.NewPlayerWeekSignIn)
                return;

            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                ActiveManager.GetInstance().OpenActiveFrame(WeekSignInDataManager.WeekSignInConfigId,
                    (int)WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId);
            }
            else
            {
                ActiveManager.GetInstance().OpenActiveFrame(WeekSignInDataManager.WeekSignInConfigId,
                    (int) WeekSignInDataManager.ActivityWeekSignInOpActTypeId);
            }
        }
        

        //周签到的获奖界面
        public static void OnCloseWeekSignInAwardRecordFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<WeekSignInAwardRecordFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<WeekSignInAwardRecordFrame>();
        }

        public static void OnOpenWeekSignInAwardRecordFrame(int weekSignInType)
        {
            OnCloseWeekSignInAwardRecordFrame();
            ClientSystemManager.GetInstance().OpenFrame<WeekSignInAwardRecordFrame>(FrameLayer.Middle, weekSignInType);
        }

        public static List<WeekSignSumTable> GetWeekSignSumTableListByWeekSignSumTablesInType(WeekSignInType weekSignInType)
        {
            List<WeekSignSumTable> weekSignSumTableList = new List<WeekSignSumTable>();

            var opActType = WeekSignInDataManager.GetInstance().GetOpActTypeByWeekSignInType(weekSignInType);
            var weekSignSumTables = TableManager.GetInstance().GetTable<WeekSignSumTable>();
            var tableIter = weekSignSumTables.GetEnumerator();
            while (tableIter.MoveNext())
            {
                var curTable = tableIter.Current.Value as WeekSignSumTable;
                if (curTable != null && curTable.opActType == opActType)
                    weekSignSumTableList.Add(curTable);
            }

            weekSignSumTableList.Sort((x, y) => x.weekSum.CompareTo(y.weekSum));

            return weekSignSumTableList;
        }

        //得到第一个可以领取累计奖励的索引
        public static int GetFirstTotalAwardItemIndex(WeekSignInType weekSignInType)
        {
            if (weekSignInType != WeekSignInType.ActivityWeekSignIn
                && weekSignInType != WeekSignInType.NewPlayerWeekSignIn)
                return 0;

            var awardDataModel = WeekSignInDataManager.GetInstance()
                .GetWeekSignInAwardDataModelByWeekSignInType(weekSignInType);

            if (awardDataModel == null || awardDataModel.AlreadyReceiveWeekList == null
                                       || awardDataModel.AlreadyReceiveWeekList.Count <= 0
                                       || awardDataModel.AlreadySignInWeek <= 1)
                return 0;

            //遍历累计周数
            for (var i = 0; i < awardDataModel.WeekSignSumTableList.Count; i++)
            {
                var weekSignSumTable = awardDataModel.WeekSignSumTableList[i];
                if(weekSignSumTable == null)
                    continue;

                //如果累计签到周数大于当前的周数
                if (awardDataModel.AlreadySignInWeek >= weekSignSumTable.weekSum)
                {
                    //查找是否已经领取奖励了
                    bool isFind = false;
                    for (var j = 0; j < awardDataModel.AlreadyReceiveWeekList.Count; j++)
                    {
                        if (awardDataModel.AlreadyReceiveWeekList[j] == weekSignSumTable.weekSum)
                            isFind = true;
                    }
                    
                    //没有领取奖励，则返回索引
                    if (isFind == false)
                        return i;
                }
            }

            return 0;
        }

        public static WeekSignInAwardState GetWeekSignInTotalAwardState(WeekSignInType weekSignInType,
            WeekSignSumTable sumTable)
        {
            if (weekSignInType != WeekSignInType.ActivityWeekSignIn &&
                weekSignInType != WeekSignInType.NewPlayerWeekSignIn)
                return WeekSignInAwardState.UnFinished;

            if (sumTable == null)
                return WeekSignInAwardState.UnFinished;

            var awardDataModel = WeekSignInDataManager.GetInstance()
                .GetWeekSignInAwardDataModelByWeekSignInType(weekSignInType);

            //是否已经领取
            if (awardDataModel.AlreadyReceiveWeekList != null)
            {
                for (var i = 0; i < awardDataModel.AlreadyReceiveWeekList.Count; i++)
                {
                    if (awardDataModel.AlreadyReceiveWeekList[i] == sumTable.weekSum)
                        return WeekSignInAwardState.Received;
                }
            }

            //是否已经完成，可以领取
            if (awardDataModel.AlreadySignInWeek >= sumTable.weekSum)
                return WeekSignInAwardState.Finished;

            //未完成
            return WeekSignInAwardState.UnFinished;

        }


        #region WeekSignInStatus
        //判断周签到是否开始
        public static bool IsWeekSignInOpenByWeekSignInType(WeekSignInType weekSignInType)
        {
            var opActivityData = WeekSignInDataManager.GetInstance().GetWeekSignInOpActivityDataByWeekSignInType(weekSignInType);

            //数据不存在
            if (opActivityData == null)
                return false;

            //状态非In
            if ((OpActivityState)opActivityData.state != OpActivityState.OAS_IN)
                return false;

            //时间超过了
            if (TimeManager.GetInstance().GetServerTime() > opActivityData.endTime)
                return false;

            return true;
        }

        //是否存在红点提示
        public static bool IsWeekSignInRedPointVisibleByWeekSignInType(WeekSignInType weekSignInType)
        {
            //周签到没有开启
            if (IsWeekSignInOpenByWeekSignInType(weekSignInType) == false)
                return false;

            //是否为新的一天开启
            if (IsWeekSignInShowRedPointByDailyLogin(weekSignInType) == true)
                return true;

            //周签到任务奖励可以领取
            if (IsWeekSignInTaskAwardCanReceived(weekSignInType) == true)
                return true;

            //周签到的累计奖励可以领取
            if (IsWeekSignInTotalAwardCanReceived(weekSignInType) == true)
                return true;

            return false;
        }

        //判断主界面上周签到的按钮是否显示
        public static bool IsWeekSignInVisibleByWeekSignInType(WeekSignInType weekSignInType)
        {

            //周签到没有开始
            if (IsWeekSignInOpenByWeekSignInType(weekSignInType) == false)
                return false;

            //存在任务可以做或者任务可以领取
            if (IsWeekSignInTaskAwardCanDoOrReceived(weekSignInType) == true)
                return true;
            
            //存在可以领取的周签到
            if (IsWeekSignInTotalAwardCanReceived(weekSignInType) == true)
                return true;

            return false;
        }

        //周签到的任务可以做
        public static bool IsWeekSignInTaskAwardCanDoOrReceived(WeekSignInType weekSignInType)
        {
            var opActivityData = WeekSignInDataManager.GetInstance()
                .GetWeekSignInOpActivityDataByWeekSignInType(weekSignInType);

            if (opActivityData == null || opActivityData.tasks == null || opActivityData.tasks.Length <= 0)
                return false;

            for (var i = 0; i < opActivityData.tasks.Length; i++)
            {
                var taskData = opActivityData.tasks[i];
                if (taskData == null)
                    continue;

                var opActTask = ActivityDataManager.GetInstance().GetLimitTimeTaskData(taskData.dataid);
                if (opActTask == null)
                    continue;

                //存在未完成的任务
                if ((OpActTaskState)opActTask.state == OpActTaskState.OATS_UNFINISH)
                    return true;

                //存在完成可以领取的任务
                if ((OpActTaskState)opActTask.state == OpActTaskState.OATS_FINISHED)
                    return true;
            }

            return false;
        }

        //周签到的任务可以领取
        public static bool IsWeekSignInTaskAwardCanReceived(WeekSignInType weekSignInType)
        {
            var opActivityData = WeekSignInDataManager.GetInstance()
                .GetWeekSignInOpActivityDataByWeekSignInType(weekSignInType);

            if (opActivityData == null || opActivityData.tasks == null || opActivityData.tasks.Length <= 0)
                return false;

            for (var i = 0; i < opActivityData.tasks.Length; i++)
            {
                var taskData = opActivityData.tasks[i];
                if(taskData == null)
                    continue;

                var opActTask = ActivityDataManager.GetInstance().GetLimitTimeTaskData(taskData.dataid);
                if(opActTask == null)
                    continue;

                //存在完成的任务
                if ((OpActTaskState) opActTask.state == OpActTaskState.OATS_FINISHED)
                    return true;
            }

            return false;
        }

        //周签到的累计奖励可以领取
        public static bool IsWeekSignInTotalAwardCanReceived(WeekSignInType weekSignInType)
        {
            var curWeekSignInAwardDataModel = WeekSignInDataManager.GetInstance()
                .GetWeekSignInAwardDataModelByWeekSignInType(weekSignInType);

            if (curWeekSignInAwardDataModel == null)
                return false;

            if (weekSignInType != curWeekSignInAwardDataModel.WeekSignInType)
                return false;

            //判断是否有奖励可以领取
            if (curWeekSignInAwardDataModel.IsTotalAwardCanReceived() == false)
                return false;

            return true;
        }

        //是否为新的一天开启按钮
        public static bool IsWeekSignInShowRedPointByDailyLogin(WeekSignInType weekSignInType)
        {

            var weekSignInStr = WeekSignInDataManager.ActivityWeekSignInStr;
            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
                weekSignInStr = WeekSignInDataManager.NewPlayerWeekSignInStr;

            //字段不存在，第一次登陆，则显示
            var keyName = PlayerBaseData.GetInstance().RoleID + weekSignInStr;
            if (PlayerPrefs.HasKey(keyName) == false)
                return true;

            //字符串空
            var lastTimeStr = PlayerPrefs.GetString(keyName);
            if (string.IsNullOrEmpty(lastTimeStr) == true)
                return true;

            //非同一天，则显示
            var lastTime = ulong.Parse(lastTimeStr);
            if (TimeUtility.IsSameDayOfTwoTime(lastTime, TimeManager.GetInstance().GetServerTime()) == false)
                return true;

            return false;
        }

        //设置周签的红点时间
        public static void SetWeekSignInShowRedPointTimeByDailyLogin(WeekSignInType weekSignInType)
        {
            var weekSignInStr = WeekSignInDataManager.ActivityWeekSignInStr;
            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
                weekSignInStr = WeekSignInDataManager.NewPlayerWeekSignInStr;

            var currentTimeStr = TimeManager.GetInstance().GetServerTime().ToString();

            //字段不存在，第一次登陆，则显示
            var keyName = PlayerBaseData.GetInstance().RoleID + weekSignInStr;
            if (PlayerPrefs.HasKey(keyName) == false)
            {
                PlayerPrefs.SetString(keyName, currentTimeStr);
                SendRedPointUpdateByChangeDailyLoginTime(weekSignInType);
                return;
            }

            //字符串空
            var lastTimeStr = PlayerPrefs.GetString(keyName);
            if (string.IsNullOrEmpty(lastTimeStr) == true)
            {
                PlayerPrefs.SetString(keyName, currentTimeStr);
                SendRedPointUpdateByChangeDailyLoginTime(weekSignInType);
                return;
            }

            //非同一天，则缓存时间
            var lastTime = ulong.Parse(lastTimeStr);
            if (TimeUtility.IsSameDayOfTwoTime(lastTime, TimeManager.GetInstance().GetServerTime()) == false)
            {
                PlayerPrefs.SetString(keyName, currentTimeStr);
                SendRedPointUpdateByChangeDailyLoginTime(weekSignInType);
                return;
            }
        }

        private static void SendRedPointUpdateByChangeDailyLoginTime(WeekSignInType weekSignInType)
        {
            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnNewPlayerWeekSignInRedPointChanged);
            }
            else if(weekSignInType == WeekSignInType.ActivityWeekSignIn)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnActivityWeekSignInRedPointChanged);
            }
        }

        #endregion
    }
}