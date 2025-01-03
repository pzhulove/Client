using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GameClient;
using Protocol;
using Network;
using System;
using FashionLimitTimeBuy;
using LimitTimeGift;
using ProtoTable;

/*限时活动*/
namespace ActivityLimitTime
{
    public enum ActivityType
    {
        ChargeSingleDaily = 0, //每日单笔充值
        ChargeTotalDay, //每日累计充值
        ChargeTotal, //累计充值
        ChargeSingle, //单笔充值
        ChargeContinous, //连续充值


        None
    }

    public enum ActivityTabTag
    {
        None = 0,
        New = 1,
        LimitTime = 2
    }

    public enum ActivityCircleType
    {
        Daily = 0,
        Once,
        Week,
        None
    }

    public enum ActivityState
    {
        End = 0,
        Start,
        Prepare,
        None
    }

    public enum ActivityTaskState
    {
        Init = 0, //初始状态
        UnFinish, //已经接任务
        Finished, //已完成，未提交
        Failed, //失败
        Submitting, //正在提交中（已完成并且正在提交中)
        Over //已完成,已提交
    }

    public enum ActivityLimitTimeID
    {
        FashionDiscount = 1000,
    }

    public enum ActivityDetailStatus
    {
        Received,
        UnReceived,
        Undone
    }

    /// <summary>
    /// 限时活动 - 单个活动内容
    /// </summary>
    public class ActivityLimitTimeData
    {
        private UInt32 dataId;

        public UInt32 DataId
        {
            get { return dataId; }
            set { dataId = value; }
        }

        private ActivityState activityState;

        public ActivityState ActivityState
        {
            get { return activityState; }
            set { activityState = value; }
        }

        /*
        private ActivityType activityType;
        public ActivityType ActivityType
        {
            get { return activityType; }
            set { activityType = value; }
        }
        */

        private OpActivityTmpType activityType;

        public OpActivityTmpType ActivityType
        {
            get { return activityType; }
            set { activityType = value; }
        }

        private ActivityTabTag activityTabTag;

        public ActivityTabTag ActivityTabTag
        {
            get { return activityTabTag; }
            set { activityTabTag = value; }
        }

        //任务更新 - 每日 ， 一次性， 每周
        private ActivityCircleType activityCircleType;

        public ActivityCircleType ActivityCircleType
        {
            get { return activityCircleType; }
            set { activityCircleType = value; }
        }

        //活动页签显示文本 - 活动名
        private string activityTabName;

        public string ActivityTabName
        {
            get { return activityTabName; }
            set { activityTabName = value; }
        }


        private string logoDesc;

        public string LogoDesc
        {
            get { return logoDesc; }
            set { logoDesc = value; }
        }

        //活动时间
        private string activityTimePre;

        public string ActivityTimePre
        {
            get { return activityTimePre; }
            set { activityTimePre = value; }
        }

        public string Description { get; set; }

        private UInt32 activityStartTime;

        public UInt32 ActivityStartTime
        {
            get { return activityStartTime; }
            set { activityStartTime = value; }
        }

        private UInt32 activityEndTime;

        public UInt32 ActivityEndTime
        {
            get { return activityEndTime; }
            set { activityEndTime = value; }
        }

        //活动规则
        private string activityRole;

        public string ActivityRole
        {
            get { return activityRole; }
            set { activityRole = value; }
        }

        //任务描述 （一个活动内格式通用）
        private string activityTaskDesc;

        public string ActivityTaskDesc
        {
            get { return activityTaskDesc; }
            set { activityTaskDesc = value; }
        }

        //当前活动具体数据集
        public List<ActivityLimitTimeDetailData> activityDetailDataList = new List<ActivityLimitTimeDetailData>();


        #region Methods

        public ActivityLimitTimeData()
        {
            ResetData();
        }

        public void ResetData()
        {
            activityState = ActivityState.None;
            //activityType = ActivityType.None;
            activityType = OpActivityTmpType.OAT_NONE;
            activityTabTag = ActivityTabTag.None;
            activityCircleType = ActivityCircleType.None;
            activityTabName = "";
            activityStartTime = 0;
            activityEndTime = 0;
            activityRole = "";
            activityTaskDesc = "";
            if (activityDetailDataList != null)
            {
                activityDetailDataList.Clear();
            }
        }

        #endregion
    }

    /// <summary>
    /// 限时活动 - 单个活动具体每一项数据
    /// </summary>
    public class ActivityLimitTimeDetailData
    {
        private UInt32 dataId;

        public UInt32 DataId
        {
            get { return dataId; }
            set { dataId = value; }
        }

        private ActivityTaskState activityDetailState;

        public ActivityTaskState ActivityDetailState
        {
            get { return activityDetailState; }
            set { activityDetailState = value; }
        }

        //任务描述 （一个活动内格式通用）
        private string activityTaskDesc;

        public string ActivityTaskDesc
        {
            get { return activityTaskDesc; }
            set { activityTaskDesc = value; }
        }

        //完成数目
        private int doneNum;

        public int DoneNum
        {
            get { return doneNum; }
            set { doneNum = value; }
        }

        //需完成总数目
        private int totalNum;

        public int TotalNum
        {
            get { return totalNum; }
            set { totalNum = value; }
        }

        private List<int> paramNums;

        public List<int> ParamNums
        {
            get { return paramNums; }
            set { paramNums = value; }
        }

        //该具体活动项具体的奖励 图片 读表 id
        public List<ActivityLimitTimeAward> awardDataList = new List<ActivityLimitTimeAward>();

        #region Methods

        public ActivityLimitTimeDetailData()
        {
            ResetData();
        }

        public void ResetData()
        {
            activityDetailState = ActivityTaskState.Init;
            doneNum = 0;
            totalNum = 0;
            paramNums = null;
            if (awardDataList != null)
            {
                awardDataList.Clear();
            }
        }

        #endregion
    }

    /// <summary>
    /// 限时活动 - 单个活动具体每一项活动数据 - 每一个奖励数据
    /// </summary>
    public class ActivityLimitTimeAward
    {
        private UInt32 id;

        public UInt32 Id
        {
            get { return id; }
            set { id = value; }
        }

        private int num;

        public int Num
        {
            get { return num; }
            set { num = value; }
        }

        private byte strenth;

        public byte Strenth
        {
            get { return strenth; }
            set { strenth = value; }
        }

        public void Reset()
        {
            id = 0;
            num = 0;
            strenth = 0;
        }

        public ActivityLimitTimeAward()
        {
            Reset();
        }
    }

    /// <summary>
    /// Activity limit time manager.
    /// 
    /// 限时活动 
    /// 
    /// 活动 
    /// 
    /// 任务
    /// 
    /// </summary>
    public class ActivityLimitTimeManager
    {
        public List<ActivityLimitTimeData> activityLimitTimeDataList; //全部  活动数据
        public Dictionary<UInt32, List<ActivityLimitTimeDetailData>> activityLimitTimeTasksDic; //对应活动 id 的任务数据
        public bool HaveActivity = false; //是否有显示活动，除手机绑定外

        private ActivityLimitTimeData bindPhoneOtherData;
        public int fatigueBurnType = 0; //疲劳燃烧类型

        public ActivityLimitTimeData BindPhoneOtherData
        {
            get { return bindPhoneOtherData; }
        }

        private Dictionary<UInt32, ActivityLimitTimeDetailData> activitiesTasksDic; //全部  任务id + 对应任务数据
        private List<ActivityLimitTimeDetailData> activityLTDetailList; //缓存服务器同步的  全部 的 任务数据

        public event System.Action ServerSyncActivityDataListener;
        public event System.Action ServerSyncTaskDataListener;
        public event System.Action ServerSyncTaskDataChangeListener;
        public event System.Action<ActivityLimitTimeData> ServerSyncActivityStateChangeListener;

        public bool HaveFashionDiscountActivity = false;


        bool needOpen = false;
        float TimeIntrval = 0.0f;
        
        public void Initialize()
        {
            Clear();

            activityLimitTimeDataList = new List<ActivityLimitTimeData>();
            activityLimitTimeTasksDic = new Dictionary<uint, List<ActivityLimitTimeDetailData>>();
            activitiesTasksDic = new Dictionary<uint, ActivityLimitTimeDetailData>();

            NetProcess.AddMsgHandler(SyncOpActivityDatas.MsgID, OnSyncLimitTimeActivity);
            NetProcess.AddMsgHandler(SyncOpActivityTasks.MsgID, OnSyncActivityTasks);
            NetProcess.AddMsgHandler(SyncOpActivityTaskChange.MsgID, OnSyncActivityTaskChange);
            NetProcess.AddMsgHandler(SyncOpActivityStateChange.MsgID, OnSyncActivityStateChange);
        }

        public void Clear()
        {
            activityLimitTimeDataList = null;
            activityLimitTimeTasksDic = null;
            activitiesTasksDic = null;

            bindPhoneOtherData = null;

            NetProcess.RemoveMsgHandler(SyncOpActivityDatas.MsgID, OnSyncLimitTimeActivity);
            NetProcess.RemoveMsgHandler(SyncOpActivityTasks.MsgID, OnSyncActivityTasks);
            NetProcess.RemoveMsgHandler(SyncOpActivityTaskChange.MsgID, OnSyncActivityTaskChange);
            NetProcess.RemoveMsgHandler(SyncOpActivityStateChange.MsgID, OnSyncActivityStateChange);
            fatigueBurnType = 0;
        }

        #region RedPoint Check from datas

        /// <summary>
        /// 疲劳燃烧活动最低疲劳值
        /// </summary>
        int fatigueValue
        {
            get
            {
                int value = 0;

                if (int.TryParse(TR.Value("fatigue_combustion_value"),out value))
                {
                    return value;
                }
                return 0;
            }
        }

        /// <summary>
        /// 寻找疲劳燃烧活动是否开启
        /// </summary>
        /// <param name="isFlag">活动是否开启</param>
        /// <param name="data">活动数据</param>
        public void FindFatigueCombustionActivityIsOpen(ref bool isFlag, ref ActivityLimitTimeData data)
        {
            for (int i = 0; i < activityLimitTimeDataList.Count; i++)
            {
                ActivityLimitTimeData mData = activityLimitTimeDataList[i];

                if (mData.ActivityType != OpActivityTmpType.OAT_FATIGUE_BURNING)
                {
                    continue;
                }

                if (mData.ActivityState != ActivityState.Start)
                {
                    continue;
                }

                if (PlayerBaseData.GetInstance().fatigue <= fatigueValue)
                {
                    continue;
                }

                for (int j = 0; j < mData.activityDetailDataList.Count; j++)
                {
                    ActivityLimitTimeDetailData mActivityLimitTimeDetailData = mData.activityDetailDataList[j];

                    if (mActivityLimitTimeDetailData.ActivityDetailState == ActivityTaskState.Init ||
                        mActivityLimitTimeDetailData.ActivityDetailState == ActivityTaskState.UnFinish)
                    {
                        continue;
                    }

                    isFlag = true;
                    data = activityLimitTimeDataList[i];
                    return;
                }
            }

            isFlag = false;
            data = null;
        }

        //获取到当前燃烧活动开启中的buff信息
        public ActivityLimitTimeDetailData GetCurUseFatigueData()
        {
            bool isOpen = false;
            ActivityLimitTimeData activityData = null;
            FindFatigueCombustionActivityIsOpen(ref isOpen,ref activityData);
            //没有开启或者没获取到活动数据
            if (!isOpen || null == activityData)
                return null;
            for (int i = 0; i < activityData.activityDetailDataList.Count; i++)
            {
                if (activityData.activityDetailDataList[i].ActivityDetailState != ActivityLimitTime.ActivityTaskState.Finished)
                {
                    continue;
                }
                return activityData.activityDetailDataList[i];
            }
            return null;
        }

        public bool CheckHasTaskWaitToReceive()
        {
            for (var i = 0; i < this.activityLimitTimeDataList.Count; ++i)
            {
                var tempData = activityLimitTimeDataList[i];
                if (tempData.ActivityState == ActivityState.Start)
                {
                    if (tempData.ActivityType == Protocol.OpActivityTmpType.OAT_BIND_PHONE) //屏蔽手机绑定活动！！！
                        continue;

                    if (tempData.ActivityType == Protocol.OpActivityTmpType.OAT_GAMBING) //暂时在这里屏蔽夺宝活动,应该数据层就屏蔽。从其他接口进入
                        continue;

                    if (tempData.DataId == ActivityLimitTimeCombineManager.SUMMER_WATERMELON_ID)
                        continue;

                    if (tempData.activityDetailDataList == null || tempData.activityDetailDataList.Count <= 0)
                        continue;;

                    for (int j = 0; j < tempData.activityDetailDataList.Count; j++)
                    {
                        if (tempData.activityDetailDataList[j].ActivityDetailState == ActivityTaskState.Finished 
                            //深渊抽奖有次数也红点提示
                            || (tempData.ActivityType == OpActivityTmpType.OAT_HELL_TICKET_FOR_DRAW_PRIZE && CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_TIME) > 0))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion


        /// <summary>
        /// 初始时 服务器同步 本地限时活动 全部静态数据
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncLimitTimeActivity(MsgDATA msg)
        {
            int pos = 0;
            SyncOpActivityDatas syncActivityDatas = new SyncOpActivityDatas();
            syncActivityDatas.decode(msg.bytes, ref pos);

            ActivityNetData2LocalData(syncActivityDatas);

            if (ServerSyncActivityDataListener != null)
                ServerSyncActivityDataListener();

            HaveActivity = false;

            if (syncActivityDatas.datas == null)
                return;

            for (int i = 0; i < syncActivityDatas.datas.Length; i++)
            {
                var syncData = syncActivityDatas.datas[i];
                if (syncData == null)
                    return;
                if (syncData.tmpType == (uint) OpActivityTmpType.OAT_BIND_PHONE)
                {
                    continue;
                }

                if (syncData.tmpType == (uint)OpActivityTmpType.OAT_GAMBING)
                {
                    continue;
                }

                if (syncData.state == (int) ActivityState.Start)
                {
                    HaveActivity = true;
                    break;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshActivityLimitTimeBtn);

            //Logger.LogError("OnSyncLimitTimeActivity only bind phone 3333 !!!!" + syncActivityDatas.datas.Length + " | " + HaveFashionDiscount);
        }

        //活动数据 同步到本地
        private void ActivityNetData2LocalData(SyncOpActivityDatas netDatas)
        {
            if (netDatas != null)
            {
                //注释原因， 活动太多服务器做了分包处理，这里会收到服务器多次返回
                //if (activityLimitTimeDataList != null)
                //    activityLimitTimeDataList.Clear();
                //else
                //    activityLimitTimeDataList = new List<ActivityLimitTimeData>();


                //以下代码为当两个打折活动同时存在的时候只显示节日的打折活动
                bool haveFashionDiscount1 = false;
                bool haveFashionDiscount2 = false;
                HaveFashionDiscountActivity = false;
                for (int i = 0; i < netDatas.datas.Length; i++)
                {
                    if (netDatas.datas[i].tmpType == (int) OpActivityTmpType.OAT_BUY_FASHION &&
                        netDatas.datas[i].state == (int) ActivityState.Start)
                    {
                        haveFashionDiscount1 = true;
                    }

                    if (netDatas.datas[i].tmpType == (int) OpActivityTmpType.OAT_MALL_DISCOUNT_FOR_NEW_SERVER &&
                        netDatas.datas[i].state == (int) ActivityState.Start)
                    {
                        haveFashionDiscount2 = true;
                        HaveFashionDiscountActivity = true;
                    }
                }

                for (int i = 0; i < netDatas.datas.Length; i++)
                {
                    if (haveFashionDiscount1 && haveFashionDiscount2)
                    {
                        if (netDatas.datas[i].tmpType == (int) OpActivityTmpType.OAT_MALL_DISCOUNT_FOR_NEW_SERVER)
                        {
                            continue;
                        }
                    }

                    var netData = netDatas.datas[i];
                    var localData = new ActivityLimitTimeData();
                    localData.DataId = netData.dataId;
                    localData.ActivityState = (ActivityState) netData.state;
                    //localData.ActivityType = (ActivityType)netData.tmpType;
                    localData.ActivityType = (OpActivityTmpType) netData.tmpType;
                    localData.ActivityTabName = netData.name;
                    localData.LogoDesc = netData.logoDesc;
                    localData.ActivityTabTag = (ActivityTabTag) netData.tag;
                    localData.ActivityStartTime = netData.startTime;
                    localData.ActivityEndTime = netData.endTime;
                    localData.ActivityTimePre = netData.desc;
                    localData.ActivityRole = netData.ruleDesc;
                    localData.ActivityTaskDesc = netData.taskDesc;
                    localData.Description = netData.desc;

                    if (netData.tasks != null && netData.tasks.Length > 0)
                    {
                        if (netData.tmpType == (int) OpActivityTmpType.OAT_LEVEL_SHOW_FOR_NEW_SERVER)
                        {
                            for (int ii = 0; ii < netData.tasks.Length; ii++)
                            {
                                for (int jj = 0; jj < netData.tasks.Length - 1; jj++)
                                {
                                    if (netData.tasks[jj].completeNum > netData.tasks[jj + 1].completeNum)
                                    {
                                        var temp = netData.tasks[jj];
                                        netData.tasks[jj] = netData.tasks[jj + 1];
                                        netData.tasks[jj + 1] = temp;
                                    }
                                }
                            }
                        }

                        for (int j = 0; j < netData.tasks.Length; j++)
                        {
                            var actTaskData =
                                ActivityTaskNetData2LocalData(localData.ActivityTaskDesc, netData.tasks[j]);
                            if (actTaskData != null)
                            {
                                localData.activityDetailDataList.Add(actTaskData);
                            }

                            /*
                         var localTaskData = new ActivityLimitTimeDetailData();
                         ActivityTaskNetDataToLocal(ref localTaskData, localData.ActivityTaskDesc, netData.tasks[j]);
                         localData.activityDetailDataList.Add(localTaskData);
                          */
                        }
                    }

                    if (localData.ActivityType == OpActivityTmpType.OAT_BIND_PHONE)
                    {
                        bindPhoneOtherData = localData;
                        //return;
                    }

                    activityLimitTimeDataList.Add(localData);
                }


                AllTasksDataDicToActivityDataList();
                AllActivitiesDataToDicByActId();
            }
        }

        public void ReOpenActivityLimittimeFrame()
        {
            //if (ClientSystemManager.GetInstance().IsFrameOpen<ActivityCombineFrame>())
            //{
            //    ClientSystemManager.GetInstance().CloseFrame<ActivityCombineFrame>();
            //}

            needOpen = true;
        }

        public void OnUpdate(float timeElapsed)
        {
            if (needOpen)
            {
                TimeIntrval += timeElapsed;

                if (TimeIntrval > 0.5f)
                {
                    //if (!ClientSystemManager.GetInstance().IsFrameOpen<ActivityCombineFrame>())
                    //{
                    //    ClientSystemManager.GetInstance().OpenFrame<ActivityCombineFrame>();
                    //}

                    needOpen = false;
                    TimeIntrval = 0.0f;
                }
            }
        }

        private void ActivityTaskNetDataToLocal(ref ActivityLimitTimeDetailData localTaskData, string taskDesc,
            OpActTaskData data)
        {
            if (localTaskData != null && data != null)
            {
                localTaskData.DataId = data.dataid;
                localTaskData.ActivityTaskDesc = taskDesc;
                localTaskData.DoneNum = 0;
                localTaskData.TotalNum = (int) data.completeNum;
                if (data.variables != null)
                {
                    localTaskData.ParamNums = new List<int>();
                    for (int i = 0; i < data.variables.Length; i++)
                    {
                        localTaskData.ParamNums.Add((int) data.variables[i]);
                    }
                }

                localTaskData.ActivityDetailState = ActivityTaskState.Init;

                if (data.rewards != null && data.rewards.Length > 0)
                {
                    localTaskData.awardDataList = new List<ActivityLimitTimeAward>();
                    for (int i = 0; i < data.rewards.Length; i++)
                    {
                        var localAward = new ActivityLimitTimeAward();
                        ActTaskAwardNetDataToLocal(ref localAward, data.rewards[i]);
                        localTaskData.awardDataList.Add(localAward);
                    }
                }
            }
        }

        private void ActTaskAwardNetDataToLocal(ref ActivityLimitTimeAward localAward, OpTaskReward netData)
        {
            if (localAward != null && netData != null)
            {
                localAward.Id = netData.id;
                localAward.Num = (int) netData.num;
                localAward.Strenth = netData.strenth;
            }
        }


        //活动的任务数据  同步到本地
        private ActivityLimitTimeDetailData ActivityTaskNetData2LocalData(string taskDesc, OpActTaskData data)
        {
            var localTaskData = new ActivityLimitTimeDetailData();
            if (localTaskData != null && data != null)
            {
                localTaskData.DataId = data.dataid;
                localTaskData.ActivityTaskDesc = taskDesc;
                localTaskData.DoneNum = 0;
                localTaskData.TotalNum = (int) data.completeNum;
                if (data.variables != null)
                {
                    localTaskData.ParamNums = new List<int>();
                    for (int i = 0; i < data.variables.Length; i++)
                    {
                        localTaskData.ParamNums.Add((int) data.variables[i]);
                    }
                }

                localTaskData.ActivityDetailState = ActivityTaskState.Init;

                if (data.rewards != null && data.rewards.Length > 0)
                {
                    localTaskData.awardDataList = new List<ActivityLimitTimeAward>();
                    for (int i = 0; i < data.rewards.Length; i++)
                    {
                        var rewardData = ActivityTaskAwardNet2Local(data.rewards[i]);
                        if (rewardData != null)
                        {
                            localTaskData.awardDataList.Add(rewardData);
                        }
                    }
                }

                return localTaskData;
            }

            return null;
        }

        //活动任务的奖励数据  同步到本地
        private ActivityLimitTimeAward ActivityTaskAwardNet2Local(OpTaskReward netData)
        {
            var localAward = new ActivityLimitTimeAward();
            if (localAward != null && netData != null)
            {
                localAward.Id = netData.id;
                localAward.Num = (int) netData.num;
                return localAward;
            }

            return null;
        }

        //同步活动任务
        private void OnSyncActivityTasks(MsgDATA data)
        {
            int pos = 0;
            SyncOpActivityTasks taskDatas = new SyncOpActivityTasks();
            taskDatas.decode(data.bytes, ref pos);

            ActivityTasksNet2Local(taskDatas);
            if (ServerSyncTaskDataListener != null)
                ServerSyncTaskDataListener();
        }

        //活动任务数据  同步到本地
        private void ActivityTasksNet2Local(SyncOpActivityTasks taskDatas)
        {
            if (taskDatas != null)
            {
                var actResTasks = taskDatas.tasks;
                if (actResTasks == null)
                    return;
                if (actResTasks.Length > 0)
                {
                    activityLTDetailList = new List<ActivityLimitTimeDetailData>();
                    for (int i = 0; i < actResTasks.Length; i++)
                    {
                        var actLTDetailData = new ActivityLimitTimeDetailData();
                        actLTDetailData.DataId = actResTasks[i].dataId;
                        actLTDetailData.DoneNum = (int) actResTasks[i].curNum;
                        actLTDetailData.ActivityDetailState = (ActivityTaskState) actResTasks[i].state;
                        activityLTDetailList.Add(actLTDetailData);
                        if (activitiesTasksDic != null)
                        {
                            var currTaskId = actLTDetailData.DataId;
                            if (activitiesTasksDic.ContainsKey(currTaskId))
                                activitiesTasksDic.Remove(currTaskId);
                            activitiesTasksDic.Add(currTaskId, actLTDetailData);
                        }
                    }

                    AllTasksDataDicToActivityDataList();
                    AllActivitiesDataToDicByActId();
                }
            }
        }

        //同步活动任务状态和数据的改变
        private void OnSyncActivityTaskChange(MsgDATA data)
        {
            int pos = 0;
            SyncOpActivityTaskChange taskChangeDatas = new SyncOpActivityTaskChange();
            taskChangeDatas.decode(data.bytes, ref pos);

            ActivityTasksNet2Local(taskChangeDatas);
            if (ServerSyncTaskDataChangeListener != null)
                ServerSyncTaskDataChangeListener();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskUpdate);
        }

        //活动任务状态和数据的改变  同步到本地
        private void ActivityTasksNet2Local(SyncOpActivityTaskChange taskDatas)
        {
            if (taskDatas != null)
            {
                var actResTasks = taskDatas.tasks;
                if (actResTasks == null)
                    return;
                if (actResTasks.Length > 0)
                {
                    activityLTDetailList = new List<ActivityLimitTimeDetailData>();
                    for (int i = 0; i < actResTasks.Length; i++)
                    {
                        var actLTDetailData = new ActivityLimitTimeDetailData();
                        actLTDetailData.DataId = actResTasks[i].dataId;
                        actLTDetailData.DoneNum = (int) actResTasks[i].curNum;
                        actLTDetailData.ActivityDetailState = (ActivityTaskState) actResTasks[i].state;
                        activityLTDetailList.Add(actLTDetailData);
                        if (activitiesTasksDic != null)
                        {
                            var currTaskId = actLTDetailData.DataId;
                            if (activitiesTasksDic.ContainsKey(currTaskId))
                                activitiesTasksDic.Remove(currTaskId);
                            activitiesTasksDic.Add(currTaskId, actLTDetailData);
                        }
                    }

                    /****    此处代码被返回调用 - 服务器一直在发活动任务状态改变请求    ****/
                    AllTasksDataDicToActivityDataList();
                    AllActivitiesDataToDicByActId();
                }
            }
        }

        /// <summary>
        ///  全部活动的任务数据 根据任务id 添加动态数据到 全部活动列表中 
        /// </summary>
        private void AllTasksDataDicToActivityDataList()
        {
            if (activityLimitTimeDataList == null)
                return;
            if (activitiesTasksDic == null)
                return;
            for (int i = 0; i < activityLimitTimeDataList.Count; i++)
            {
                var actTaskDatas = activityLimitTimeDataList[i].activityDetailDataList;
                if (actTaskDatas != null)
                {
                    for (int j = 0; j < actTaskDatas.Count; j++)
                    {
                        var taskId = actTaskDatas[j].DataId;
                        if (activitiesTasksDic.ContainsKey(taskId))
                        {
                            activityLimitTimeDataList[i].activityDetailDataList[j].DoneNum
                                = activitiesTasksDic[taskId].DoneNum;
                            activityLimitTimeDataList[i].activityDetailDataList[j].ActivityDetailState
                                = activitiesTasksDic[taskId].ActivityDetailState;
                        }
                    }

                    if (activityLimitTimeDataList[i].ActivityType == OpActivityTmpType.OAT_BIND_PHONE)
                    {
                        bindPhoneOtherData = activityLimitTimeDataList[i];
                    }
                }
            }
        }

        /// <summary>
        ///  根据活动id  将 全部活动数据 添加到 活动字典 （活动id + 对应任务数据）
        /// </summary>
        private void AllActivitiesDataToDicByActId()
        {
            if (activityLimitTimeTasksDic == null)
                return;
            if (activityLimitTimeDataList != null)
            {
                for (int i = 0; i < activityLimitTimeDataList.Count; i++)
                {
                    if (activityLimitTimeDataList[i].activityDetailDataList != null)
                    {
                        var dataId = activityLimitTimeDataList[i].DataId;
                        var actDetailTaskList = activityLimitTimeDataList[i].activityDetailDataList;
                        if (activityLimitTimeTasksDic.ContainsKey(dataId))
                        {
                            activityLimitTimeTasksDic.Remove(dataId);
                        }

                        activityLimitTimeTasksDic.Add(dataId, actDetailTaskList);
                    }
                }
            }
        }

        //同步活动状态改变
        private void OnSyncActivityStateChange(MsgDATA data)
        {
            int pos = 0;
            SyncOpActivityStateChange actStateChangeData = new SyncOpActivityStateChange();
            actStateChangeData.decode(data.bytes, ref pos);

            if (actStateChangeData.data.dataId == (int) ActivityLimitTimeID.FashionDiscount &&
                actStateChangeData.data.state == 0)
            {
                //if (ClientSystemManager.GetInstance().IsFrameOpen<MallFrame>() &&
                //    MallDataManager.GetInstance().isInFashionTab)
                //{
                //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.updateFashionTab);
                //}

                if (ClientSystemManager.GetInstance().IsFrameOpen<FashionLimitTimeBuyFrame>() &&
                    FashionLimitTimeBuyManager.GetInstance().haveFashionDiscount)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("时装打折活动已结束",
                        () => { ClientSystemManager.GetInstance().CloseFrame<FashionLimitTimeBuyFrame>(); });
                }
            }

            ActivityStateNet2Local(actStateChangeData);

            HaveActivity = false;

            if (activityLimitTimeDataList == null)
                return;

            for (int i = 0; i < activityLimitTimeDataList.Count; i++)
            {
                var syncData = activityLimitTimeDataList[i];
                if (syncData == null)
                    return;
                if (syncData.ActivityType == OpActivityTmpType.OAT_BIND_PHONE)
                {
                    continue;
                }

                if (syncData.ActivityType == OpActivityTmpType.OAT_GAMBING)
                {
                    continue;
                }

                if (activityLimitTimeDataList[i].ActivityState == ActivityState.Start)
                {
                    HaveActivity = true;
                    break;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshActivityLimitTimeBtn);

            if(actStateChangeData.data != null && actStateChangeData.data.tmpType == (uint)OpActivityTmpType.OAT_FATIGUE_BURNING)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateTownBuf);
            }

            //Logger.LogError("OnSyncActivityStateChange only bind phone 3333 !!!!" + activityLimitTimeDataList.Count + " | " + HaveFashionDiscount);
        }

        //活动状态改变  同步到本地
        private void ActivityStateNet2Local(SyncOpActivityStateChange actStateChange)
        {
            if (actStateChange == null)
                return;
            if (actStateChange.data == null)
                return;
            if (activityLimitTimeDataList != null)
            {
                int actCount = activityLimitTimeDataList.Count;
                if (actCount > 0)
                {
                    for (int i = 0; i < activityLimitTimeDataList.Count; i++)
                    {
                        var actData = activityLimitTimeDataList[i];
                        if (actData.DataId == actStateChange.data.dataId)
                        {
                            activityLimitTimeDataList[i] = SyncNetActDataToLocalActData(actStateChange.data);

                            var syncActData = activityLimitTimeDataList[i];

                            if (syncActData == null)
                                return;

                            //执行活动更新 监听
                            if (ServerSyncActivityStateChangeListener != null)
                                ServerSyncActivityStateChangeListener(syncActData);

                            //新加  GM直接关闭手机绑定时  活动状态
                            if (syncActData.ActivityType == OpActivityTmpType.OAT_BIND_PHONE)
                            {
                                if (syncActData.ActivityState == ActivityState.End)
                                {
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SDKBindPhoneFinished, false);
                                }
                                else if (syncActData.ActivityState == ActivityState.Start)
                                {
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SDKBindPhoneFinished, true);
                                }
                            }

                            break;
                        }

                        //if (actData.DataId == actStateChange.dataId)
                        //{
                        //    actData.ActivityStartTime = actStateChange.startTime;
                        //    actData.ActivityEndTime = actStateChange.endTime;
                        //    actData.ActivityState = (ActivityState)actStateChange.state;
                        //    activityLimitTimeDataList[i] = actData;
                        //    //执行活动更新 监听
                        //    if (ServerSyncActivityStateChangeListener != null)
                        //        ServerSyncActivityStateChangeListener(activityLimitTimeDataList[i]);
                        //    break;
                        //}
                    }
                }
                else
                {
                    Logger.LogProcessFormat("SyncActivityState  data is null");
                }

                AllTasksDataDicToActivityDataList();
                AllActivitiesDataToDicByActId();
            }
        }

        //活动状态改变  同步到本地 具体每个活动处理
        private ActivityLimitTimeData SyncNetActDataToLocalActData(OpActivityData opData)
        {
            if (opData == null) return null;
            var actData = new ActivityLimitTimeData
            {
                DataId = opData.dataId,
                ActivityState = (ActivityState) opData.state,
                ActivityType = (OpActivityTmpType) opData.tmpType,
                ActivityTabName = opData.name,
                ActivityTabTag = (ActivityTabTag) opData.tag,
                ActivityStartTime = opData.startTime,
                ActivityEndTime = opData.endTime,
                ActivityTimePre = opData.desc,
                LogoDesc = opData.logoDesc,
                ActivityRole = opData.ruleDesc,
                ActivityTaskDesc = opData.taskDesc,
                Description = opData.desc
            };
            string[] skillDesc = actData.ActivityTaskDesc.Split('|');
            if (opData.tasks != null && opData.tasks.Length > 0)
            {
                actData.activityDetailDataList = new List<ActivityLimitTimeDetailData>();

                for (int i = 0; i < opData.tasks.Length; i++)
                {
                    if (skillDesc.Length == opData.tasks.Length)
                    {
                        var actTaskData = ActivityTaskNetData2LocalData(skillDesc[i], opData.tasks[i]);
                        if (actTaskData != null)
                            actData.activityDetailDataList.Add(actTaskData);
                    }
                    else
                    {
                        var actTaskData = ActivityTaskNetData2LocalData(actData.ActivityTaskDesc, opData.tasks[i]);
                        if (actTaskData != null)
                            actData.activityDetailDataList.Add(actTaskData);
                    }
                }
            }

            if (actData.ActivityType == OpActivityTmpType.OAT_BIND_PHONE)
            {
                bindPhoneOtherData = actData;
            }

            return actData;
        }

        //请求活动任务操作
        public void RequestOnTakeActTask(UInt32 activityDataId, UInt32 taskDataId)
        {
            TakeOpActTaskReq taskReq = new TakeOpActTaskReq();
            taskReq.activityDataId = activityDataId;
            taskReq.taskDataId = taskDataId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, taskReq);

            //GameFrameWork.instance.StartCoroutine(WaitToRefreshTask(activityDataId,taskDataId));
            //GameFrameWork.instance.StartCoroutine(WaitToRefreshAct());
        }

        public void OnSceneOpActivityTaskInfoReq(UInt32 activityDataId)
        {
            SceneOpActivityTaskInfoReq Req = new SceneOpActivityTaskInfoReq();
            Req.opActId = activityDataId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, Req);
        }

        #region _

        /*Test
        private IEnumerator WaitToRefreshTask(UInt32 actId,UInt32 taskId)
        {
            yield return Yielders.GetWaitForSecondsRealtime(3f);
            activityLTDetailList = new List<ActivityLimitTimeDetailData>();
            for (int i = 0; i < 1; i++)
            {
                var actLTDetailData = new ActivityLimitTimeDetailData();
                actLTDetailData.DataId = taskId;
                actLTDetailData.DoneNum = 6;
                actLTDetailData.ActivityDetailState = ActivityTaskState.Over;
                activityLTDetailList.Add(actLTDetailData);
                if (activitiesTasksDic != null)
                {
                    var currTaskId = actLTDetailData.DataId;
                    if (activitiesTasksDic.ContainsKey(currTaskId))
                        activitiesTasksDic.Remove(currTaskId);
                    activitiesTasksDic.Add(currTaskId, actLTDetailData);
                }
            }

            AllTasksDataDicToActivityDataList();
            AllActivitiesDataToDicByActId();

            if (ServerSyncTaskDataChangeListener != null)
                ServerSyncTaskDataChangeListener();
        }
        private IEnumerator WaitToRefreshAct()
        {
            yield return Yielders.GetWaitForSecondsRealtime(3f);
            if (activityLimitTimeDataList != null)
            {
                for (int i = 0; i < activityLimitTimeDataList.Count; i++)
                {
                    var actData = activityLimitTimeDataList[i];
                    if (actData.DataId == 4)
                    {
                        actData.ActivityState = ActivityState.End;
                        activityLimitTimeDataList[i] = actData;
                        if (ServerSyncActivityStateChangeListener != null)
                            ServerSyncActivityStateChangeListener(activityLimitTimeDataList[i]);
                        break;
                    }
                }
            }
        }
         */

        /*
         
        /// <summary>
        /// 请求 活动 具体内容 
        /// </summary>
        /// <param name="activityId">活动id</param>
        public void RequesetActivityTaskById(UInt32 activityId)
        {
            
           // OpActivityTaskReq taskReq = new OpActivityTaskReq();
           // taskReq.dataId = activityId;
           // currActivityId = activityId;
           // NetManager.Instance().SendCommand(ServerType.GATE_SERVER, taskReq);
           
            GameFrameWork.instance.StartCoroutine(WaitForActivityTaskReq(activityId));
        }

        public IEnumerator WaitForActivityTaskReq(UInt32 activityId)
        {
            var msgEvent = new MessageEvents();
            var req = new OpActivityTaskReq();
            req.dataId = activityId;
            currActivityId = activityId;

            var res = new OpActivityTaskRes();
            yield return MessageUtility.Wait<OpActivityTaskReq, OpActivityTaskRes>(ServerType.GATE_SERVER, msgEvent,req,res,true);
            if (msgEvent.IsAllMessageReceived())
            {
                 ActivityDetailNet2Local(res);
                 if (ServerSyncTaskDataListener != null)
                     ServerSyncTaskDataListener();
            }
        }

        /// <summary>
        /// 领取 活动 任务奖励
        /// </summary>
        /// <param name="taskId"></param>
        public void TakeActivityTaskAwardById(UInt32 taskId)
        {
            
           // TakeOpActTaskReq takeTaskReq = new TakeOpActTaskReq();
           // takeTaskReq.taskDataId = taskId;
          //  currTaskId = taskId;
            //NetManager.Instance().SendCommand(ServerType.GATE_SERVER, takeTaskReq);

            GameFrameWork.instance.StartCoroutine(WaitForTakingTaskReq(taskId));
        }

        public IEnumerator WaitForTakingTaskReq(UInt32 taskId)
        {
            var msgEvent = new MessageEvents();
            var req = new TakeOpActTaskReq();
            req.taskDataId = taskId;

            var res = new OpActivityTaskRes();
            yield return MessageUtility.Wait<TakeOpActTaskReq, OpActivityTaskRes>(ServerType.GATE_SERVER, msgEvent, req, res, true);
            if (msgEvent.IsAllMessageReceived())
            {
                ActivityDetailNet2Local(res);
                if (ServerSyncTaskDataListener != null)
                    ServerSyncTaskDataListener();
            }
        }
     
        /// <summary>
        /// 服务器 同步 活动 任务 数据
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncActivityTask(MsgDATA msg)
        {
            int pos = 0;
            OpActivityTaskRes taskDatas = new OpActivityTaskRes();
            taskDatas.decode(msg.bytes, ref pos);

            ActivityDetailNet2Local(taskDatas);
            if (ServerSyncTaskDataListener != null)
                ServerSyncTaskDataListener();
        }
        private void ActivityDetailNet2Local(OpActivityTaskRes activityTaskRes)
        {
            if (activityTaskRes == null)
                return;
            var actResTasks = activityTaskRes.tasks;
            if (actResTasks == null)
                return;
            if (actResTasks.Length > 0)
            {
                activityLTDetailList = new List<ActivityLimitTimeDetailData>();
                for (int i = 0; i < actResTasks.Length; i++)
                {
                    var actLTDetailData = new ActivityLimitTimeDetailData();
                    actLTDetailData.DataId = actResTasks[i].dataId;
                    actLTDetailData.DoneNum = (int)actResTasks[i].curNum;
                    actLTDetailData.ActivityDetailState = (ActivityTaskState)actResTasks[i].state;
                    activityLTDetailList.Add(actLTDetailData);

                    if (detailTaskDic != null)
                    {
                        var taskId = actLTDetailData.DataId;
                        if (detailTaskDic.ContainsKey(taskId))
                            detailTaskDic.Remove(taskId);
                        detailTaskDic.Add(taskId, actLTDetailData);
                    }
                }
                if (activityTasksDic != null)
                {
                    if (activityTasksDic.ContainsKey(currActivityId))
                        activityTasksDic.Remove(currActivityId);
                    activityTasksDic.Add(currActivityId, activityLTDetailList);
                }
            }
        }

        */

        #endregion

        #region Callback

        public void AddSyncActivityDataListener(System.Action handler)
        {
            RemoveAllSyncActivityDataListener();
            if (ServerSyncActivityDataListener == null)
                ServerSyncActivityDataListener += handler;
        }

        public void RemoveAllSyncActivityDataListener()
        {
            if (ServerSyncActivityDataListener != null)
            {
                var invocations = ServerSyncActivityDataListener.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        ServerSyncActivityDataListener -= invocations[i] as Action;
                    }
                }

                /*
                foreach (Delegate d in ServerSyncActivityDataListener.GetInvocationList())
                {
                    ServerSyncActivityDataListener -= d as Action;
                }
                 * */
            }
        }

        public void AddSyncTaskDataListener(System.Action handler)
        {
            RemoveAllSyncTaskDataListener();
            if (ServerSyncTaskDataListener == null)
                ServerSyncTaskDataListener += handler;
        }

        public void RemoveAllSyncTaskDataListener()
        {
            if (ServerSyncTaskDataListener != null)
            {
                var invocations = ServerSyncTaskDataListener.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        ServerSyncTaskDataListener -= invocations[i] as Action;
                    }
                }

                /*
                foreach (Delegate d in ServerSyncTaskDataListener.GetInvocationList())
                {
                    ServerSyncTaskDataListener -= d as Action;
                }
                 * */
            }
        }

        public void AddSyncTaskDataChangeListener(System.Action handler)
        {
            ServerSyncTaskDataChangeListener += handler;
        }

        public void RemoveSyncTaskDataChangeListener(System.Action handler)
        {
            if (ServerSyncTaskDataChangeListener != null && handler != null)
                ServerSyncTaskDataChangeListener -= handler;
        }

        public void RemoveAllSyncTaskDataChangeListener()
        {
            if (ServerSyncTaskDataChangeListener != null)
            {
                var invocations = ServerSyncTaskDataChangeListener.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        ServerSyncTaskDataChangeListener -= invocations[i] as Action;
                    }
                }

                /*
                foreach (Delegate d in ServerSyncTaskDataChangeListener.GetInvocationList())
                {
                    ServerSyncTaskDataChangeListener -= d as Action;
                }*/
            }
        }

        public void AddSyncActStateChangeListener(System.Action<ActivityLimitTimeData> handler)
        {
            RemoveAllSyncActStateChangeListener();
            if (ServerSyncActivityStateChangeListener == null)
                ServerSyncActivityStateChangeListener += handler;
        }

        public void RemoveAllSyncActStateChangeListener()
        {
            if (ServerSyncActivityStateChangeListener != null)
            {
                var invocations = ServerSyncActivityStateChangeListener.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        ServerSyncActivityStateChangeListener -= invocations[i] as Action<ActivityLimitTimeData>;
                    }
                }

                /*
                foreach (Delegate d in ServerSyncActivityStateChangeListener.GetInvocationList())
                {
                    ServerSyncActivityStateChangeListener -= d as Action<ActivityLimitTimeData>;
                }*/
            }
        }

        /// <summary>
        /// 传入抽奖表id，预览奖池
        /// </summary>
        /// <param name="id"></param>
        public void ViewAwards(int id)
        {
            ClientSystemManager.GetInstance().OpenFrame<RewardShow>(FrameLayer.Middle, id);
        }

        /// <summary>
        /// 传入抽奖表id，打开转盘
        /// </summary>
        /// <param name="id"></param>
        public void lottery(int id)
        {
            //if(CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_TIME) <= 0)
            //{
            //    SystemNotifyManager.SysNotifyFloatingEffect("抽奖次数不足");
            //}
            //else
            //{
            ClientSystemManager.GetInstance().OpenFrame<TurnTable>(FrameLayer.Middle, id);
            //}
        }

        public ActivityTaskState getTaskState(int taskID)
        {
            ActivityLimitTimeDetailData taskData = new ActivityLimitTimeDetailData();
            activitiesTasksDic.TryGetValue((uint) taskID, out taskData);
            return taskData.ActivityDetailState;
        }

        #endregion

        #region Tool

        //test
        public UInt32 DateTimeToUnix()
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            UInt32 timeStamp = (UInt32) (DateTime.Now - startTime).TotalSeconds;
            return timeStamp;
        }

        #endregion
    }
}