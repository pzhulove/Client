using Network;
using Protocol;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public struct DailyTodoTimeStruct
    {
        public int startTimeHour;
        public int startTimeMinute;
        public int endTimeHour;
        public int endTimeMinute;
    }

    public enum DailyTodoFuncState
    {
        None = 0,        
        Start,
        Finishing,
        End,
    }

    public class DailyTodoModel
    {
        public int tableId;
        public DailyTodoTable.eType type = DailyTodoTable.eType.TP_NONE;
        public DailyTodoTable.eSubType subType = DailyTodoTable.eSubType.DTSTP_NONE;
        public string name;        
        public string backgroundImgPath;
        public string linkinfo;
        public System.Func<DailyTodoTable.eSubType, bool> isTodayOpenedHandler;                             //当天功能是否开放  

        public int refreshHour = 0;                                                                         //刷新时刻

        //开放时间数据
        public List<int> openWeeks = new List<int>();
        public DailyTodoTimeStruct openTimes = new DailyTodoTimeStruct();

        public DailyTodoModel()
        {
            Clear();
        }

        public virtual void Clear()
        {
            tableId = 0;
            type = DailyTodoTable.eType.TP_NONE;
            subType = DailyTodoTable.eSubType.DTSTP_NONE;
            name = "";
            backgroundImgPath = "";
            linkinfo = "";

            if (isTodayOpenedHandler != null)
            {
                var handlerList = isTodayOpenedHandler.GetInvocationList();
                if (handlerList != null && handlerList.Length > 0)
                {
                    for (int i = 0; i < handlerList.Length; i++)
                    {
                        var handler = handlerList[i] as System.Func<DailyTodoTable.eSubType, bool>;
                        isTodayOpenedHandler -= handler;
                    }
                }
                isTodayOpenedHandler = null;
            }

            if (null != openWeeks)
            {
                openWeeks.Clear();
            }
            openTimes = default(DailyTodoTimeStruct);

            refreshHour = 0;
        }
    }

    public class DailyTodoActivity : DailyTodoModel, IComparable<DailyTodoActivity>
    {
        public int activityDungeonId = 0;
        public int startTimestamp;
        public int endTimestamp;
        public string timeDesc;
        public List<int> rewardItemIds = new List<int>();

        public eActivityDungeonState activityDungeonState = eActivityDungeonState.None;

        public System.Action<DailyTodoActivity> gotoHandler;                 //前往功能

        public DailyTodoActivity() : base()
        {            
            Clear();

            type = DailyTodoTable.eType.TP_ACTIVITY;
        }

        public override void Clear()
        {            
            activityDungeonId = 0;
            timeDesc = "";

            if (null != rewardItemIds)
            {
                rewardItemIds.Clear();
            }


            if (gotoHandler != null)
            {
                var handlerList = gotoHandler.GetInvocationList();
                if (handlerList != null && handlerList.Length > 0)
                {
                    for (int i = 0; i < handlerList.Length; i++)
                    {
                        var handler = handlerList[i] as System.Action<DailyTodoActivity>;
                        gotoHandler -= handler;
                    }
                }
                gotoHandler = null;
            }

            activityDungeonState = eActivityDungeonState.None;

            base.Clear();
        }

        public int CompareTo(DailyTodoActivity other)
        {
            if (null == other)
            {
                return 0;
            }
            return this.startTimestamp - other.startTimestamp;  //时间早的在前 升序
        }
    }

    public class DailyTodoFunction : DailyTodoModel, IComparable<DailyTodoFunction>
    {
        public DailyTodoTable.eRecommendNumType recommendType = DailyTodoTable.eRecommendNumType.RT_NONE;
        public int dayRecommendTotalCount;            //日推荐总数

        private int weekRecommendFinishTimestamp = 0;
        public int WeekRecommendFinishTimestamp
        {
            get {
                int localfileTime = _GetLocalFuncWeekFinishTimeStamp();
                if (localfileTime < 0)
                {
                    return weekRecommendFinishTimestamp;
                }
                return localfileTime;
            }
            set {
                weekRecommendFinishTimestamp = _GetLocalFuncWeekFinishTimeStamp();
                //大于0表示 当前周推荐已完成 已存了一个完成时的状态 不再用新的时间戳替换 ！！！
                if (weekRecommendFinishTimestamp > 0 && value > 0)
                {
                    return;
                }
                if (weekRecommendFinishTimestamp != value)
                {
                    PlayerPrefsManager.GetInstance().SetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.DailyTodoFunctionWeekFinishTime,
                        value, this.subType.ToString());

                    weekRecommendFinishTimestamp = value;
                }
            }
        }
        public bool IsWeekRecommendShow                 //是否展示周推荐
        {
            get {
                return _GetLocalFuncWeekRecommendShow();
            }
        }

        public string characterDesc;
        private DailyTodoFuncState recommendState = DailyTodoFuncState.None;
        public DailyTodoFuncState RecommendState //日和周合计的推荐数状态
        {
            get
            {
                DailyTodoFuncState localFileState = _GetLocalFuncState();
                if ((int)localFileState < (int)DailyTodoFuncState.None)
                {
                    return recommendState;
                }
                return localFileState;
            }
            set
            {
                recommendState = _GetLocalFuncState();
                if (recommendState == DailyTodoFuncState.End &&
                    value == DailyTodoFuncState.Finishing)
                {
                    return;
                }
                if (recommendState != value)
                {                    
                    PlayerPrefsManager.GetInstance().SetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.DailyTodoFunctionRefreshState,
                        (int)value, this.subType.ToString());                   

                    recommendState = value;
                }
                if (value == DailyTodoFuncState.End)
                {
                    NearlyRecommendEndTimeStamp = Function.GetCurrTimeStamp();
                }
                else
                {
                    NearlyRecommendEndTimeStamp = 0;
                }
            }
        }

        private int nearlyRecommendEndTimeStamp = 0;
        public int NearlyRecommendEndTimeStamp
        {
            get
            {
                int localTime = _GetLocalFuncEndStateTimeStamp();
                if (localTime < 0)
                {
                    return nearlyRecommendEndTimeStamp;
                }
                return localTime;
            }
            set
            {
                nearlyRecommendEndTimeStamp = _GetLocalFuncEndStateTimeStamp();
                if (nearlyRecommendEndTimeStamp != value)
                {
                    PlayerPrefsManager.GetInstance().SetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.DailyTodoFunctionEndStateTime,
                        (int)value, this.subType.ToString());

                    nearlyRecommendEndTimeStamp = value;
                }
            }
        }

        public string dayRecommendDesc;

        public System.Action<DailyTodoFunction> gotoHandler;                            //前往功能

        public DailyTodoFunction() : base()
        {
            Clear();

            type = DailyTodoTable.eType.TP_FUNCTION;
        }

        public override void Clear()
        {
            recommendType = DailyTodoTable.eRecommendNumType.RT_NONE;
            dayRecommendTotalCount = 0;

            weekRecommendFinishTimestamp = 0;

            characterDesc = "";
            recommendState = DailyTodoFuncState.None;
            dayRecommendDesc = "";

            if (gotoHandler != null)
            {
                var handlerList = gotoHandler.GetInvocationList();
                if (handlerList != null && handlerList.Length > 0)
                {
                    for (int i = 0; i < handlerList.Length; i++)
                    {
                        var handler = handlerList[i] as System.Action<DailyTodoFunction>;
                        gotoHandler -= handler;
                    }
                }
                gotoHandler = null;
            }

            base.Clear();
        }

        public int CompareTo(DailyTodoFunction other)
        {
            if (null == other)
            {
                return 0;
            }

            return this.tableId - other.tableId;    //id小的在前 升序
        }

        private DailyTodoFuncState _GetLocalFuncState()
        {
            int localFileState = PlayerPrefsManager.GetInstance().
                            GetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.DailyTodoFunctionRefreshState, this.subType.ToString());           
            return (DailyTodoFuncState)localFileState;
        }

        private int _GetLocalFuncEndStateTimeStamp()
        {
            int localfileTime = PlayerPrefsManager.GetInstance().
                            GetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.DailyTodoFunctionEndStateTime, this.subType.ToString());
            return localfileTime;
        }

        private int _GetLocalFuncWeekFinishTimeStamp()
        {
            int localfileTime = PlayerPrefsManager.GetInstance().
                            GetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.DailyTodoFunctionWeekFinishTime, this.subType.ToString());
            return localfileTime;
        }

        /// <summary>
        /// 获取本地数据 周推荐完成时间 是否是 在今天 完成的
        /// </summary>
        /// <returns></returns>
        private bool _GetLocalFuncWeekRecommendShow()
        {
            if (refreshHour < 0)
            {
                return false;
            }
            int localfileTime = WeekRecommendFinishTimestamp;
			//未完成
            if (localfileTime <= 0)
            {
                return true;
            }
            //在今天刷新时间后完成 即表示今天
            if (localfileTime >= Function.GetRefreshHourTimeStamp(refreshHour))
            {
                return true;
            }
            return false;
        }
    }

    public class DailyTodoDataManager : DataManager<DailyTodoDataManager>
    {
        #region MODEL PARAMS

        private bool m_IsInited = false;

        public bool BFuncOpen
        {
            get {
                return ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_DAILY_TODO) &&
                  Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.DailyTodo);
            }
        }

        private Dictionary<int, DailyTodoModel> m_DailyTodoTotalDic = new Dictionary<int, DailyTodoModel>();
        private List<DailyTodoActivity> m_ActivityDailyTodoList = new List<DailyTodoActivity>();
        private List<DailyTodoFunction> m_FunctionDailyTodoList = new List<DailyTodoFunction>();

        //界面展示数据
        private List<DailyTodoActivity> m_TempShowActivityDailyTodoList = new List<DailyTodoActivity>();
        private List<DailyTodoFunction> m_TempShowFunctionDailyTodoList = new List<DailyTodoFunction>();

        #region TR

        private string tr_daily_todo_activity_time_Key = "daily_todo_activity_time_format";
        private string tr_daily_todo_recommend_num_Key = "daily_todo_recommend_num_format";
        private string tr_daily_todo_recommend_active_Key = "daily_todo_recommend_active_format";

        #endregion

        #endregion

        #region PRIVATE METHODS

        public sealed override void Initialize()
        {
            if (m_IsInited)
            {
                return;
            }

            _BindNetEvent();
            _InitLocalData();

            m_IsInited = true;
        }

        public sealed override void Clear()
        {
            _ClearData();
            _UnBindNetEvent();
        }

        private void _InitLocalData()
        {
            var dailyTodoTable = TableManager.GetInstance().GetTable<DailyTodoTable>();
            DailyTodoModel tempModel = null;
            if (null != dailyTodoTable)
            {
                var enumerator_1 = dailyTodoTable.GetEnumerator();
                while (enumerator_1.MoveNext())
                {
                    var dailyTodoItem = enumerator_1.Current.Value as DailyTodoTable;
                    if (null == dailyTodoItem)
                    {
                        continue;
                    }
                    if (dailyTodoItem.Type == DailyTodoTable.eType.TP_ACTIVITY)
                    {
                        tempModel = new DailyTodoActivity();

                        _InitBaseDailyTodo(tempModel, dailyTodoItem);

                        DailyTodoActivity tempAct = tempModel as DailyTodoActivity;
                        if (tempAct != null)
                        {
                            _InitDailyTodoActivity(tempAct, dailyTodoItem);
                            if (m_ActivityDailyTodoList != null)
                            {
                                m_ActivityDailyTodoList.Add(tempAct);
                            }
                        }
                    }
                    else if (dailyTodoItem.Type == DailyTodoTable.eType.TP_FUNCTION)
                    {
                        tempModel = new DailyTodoFunction();

                        _InitBaseDailyTodo(tempModel, dailyTodoItem);

                        DailyTodoFunction tempFunc = tempModel as DailyTodoFunction;
                        if (tempFunc != null)
                        {
                            _InitDailyTodoFunction(tempFunc, dailyTodoItem);
                            if (m_FunctionDailyTodoList != null)
                            {
                                m_FunctionDailyTodoList.Add(tempFunc);
                            }
                        }
                    }

                    if (m_DailyTodoTotalDic != null)
                    {
                        m_DailyTodoTotalDic[(int)tempModel.subType] = tempModel;
                    }
                }
                //排序
                m_ActivityDailyTodoList.Sort((x, y) => x.CompareTo(y));
                m_FunctionDailyTodoList.Sort((x, y) => x.CompareTo(y));
            }
        }    

        private void _ClearData()
        {
            if (m_ActivityDailyTodoList != null)
            {
                for (int i = 0; i < m_ActivityDailyTodoList.Count; i++)
                {
                    var act = m_ActivityDailyTodoList[i];
                    if (act == null) continue;
                    act.Clear();
                }

                m_ActivityDailyTodoList.Clear();
            }
            if (m_FunctionDailyTodoList != null)
            {
                for (int i = 0; i < m_FunctionDailyTodoList.Count; i++)
                {
                    var func = m_FunctionDailyTodoList[i];
                    if (func == null) continue;
                    func.Clear();
                }

                m_FunctionDailyTodoList.Clear();
            }
			
			if (m_DailyTodoTotalDic != null)
            {
                m_DailyTodoTotalDic.Clear();
            }

            if (m_TempShowActivityDailyTodoList != null)
            {
                m_TempShowActivityDailyTodoList.Clear();
            }
            if (m_TempShowFunctionDailyTodoList != null)
            {
                m_TempShowFunctionDailyTodoList.Clear();
            }

            m_IsInited = false;
        }

        private void _InitBaseDailyTodo(DailyTodoModel model, DailyTodoTable table)
        {
            if (model == null || table == null)
            {
                return;
            }
            model.tableId = table.ID;
            model.name = table.Name;
            model.backgroundImgPath = table.BackgroundPath;
            model.linkinfo = table.LinkInfo;
            model.subType = table.SubType;

            //设置开放状态
            model.isTodayOpenedHandler = _IsTodayOpened;

            string openWeekDayStr = table.OpenWeekDay;
            if (!string.IsNullOrEmpty(openWeekDayStr) && null != model.openWeeks)
            {
                string[] openWeekDays = openWeekDayStr.Split('|');
                if (null != openWeekDays)
                {
                    for (int i = 0; i < openWeekDays.Length; i++)
                    {
                        int week = 0;
                        if (int.TryParse(openWeekDays[i], out week))
                        {
                            if (week == 0) continue;
                            model.openWeeks.Add(week);
                        }
                    }
                }
            }

            string openDayTimeStr = table.OpenDayTime;
            if (!string.IsNullOrEmpty(openDayTimeStr))
            {
                string[] openDayTimes = openDayTimeStr.Split('-');
                if (null != openDayTimes && openDayTimes.Length == 2)
                {
                    string startTime = openDayTimes[0];
                    string endTime = openDayTimes[1];
                    if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                    {
                        int[] startTimeInt = Function.TransferTimeSplitByColon(startTime);
                        if (startTimeInt != null && startTimeInt.Length >= 2)
                        {
                            model.openTimes.startTimeHour = startTimeInt[0];
                            model.openTimes.startTimeMinute = startTimeInt[1];
                        }
                        int[] endTimeInt = Function.TransferTimeSplitByColon(endTime);
                        if (endTimeInt != null && endTimeInt.Length >= 2)
                        {
                            model.openTimes.endTimeHour = endTimeInt[0];
                            model.openTimes.endTimeMinute = endTimeInt[1];
                        }
                    }
                }
            }

            var systemValue = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DAILY_TODO_REFRESH_TIME);
            if (systemValue != null)
            {
                model.refreshHour = systemValue.Value;
            }
        }

        private void _InitDailyTodoActivity(DailyTodoActivity model, DailyTodoTable table)
        {
            if (model == null || table == null)
            {
                return;
            }
            model.activityDungeonId = table.ActivityDungeonID;
            //设置前往
            model.gotoHandler = _OnGoActivityDailyTodo;
			model.timeDesc = TR.Value(tr_daily_todo_activity_time_Key, table.OpenDayTime);
            _UpdateDailyTodoActivity(model);
        }

        private void _UpdateDailyTodoActivity(DailyTodoActivity model)
        {
            if (model == null)
            {
                return;
            }
            var activityDungeonData = ActivityDungeonDataManager.GetInstance().GetSubByActivityDungeonID(model.activityDungeonId);
            //var activityDungeonData = _GetActivityDungeonSubByDungeonId(model.activityDungeonId);
            if (null == activityDungeonData)
            {
                return;
            }

            //设置开始结束时间
            //string timestampDesc = "";
            //if (activityDungeonData.starttime == 0 || activityDungeonData.endtime == 0)
            //{
            //    model.startTimestamp = System.Convert.ToInt32(Function.GetTodayGivenHourAndMinuteTimestamp(model.openTimes.startTimeHour, model.openTimes.startTimeMinute));
            //    model.endTimestamp = System.Convert.ToInt32(Function.GetTodayGivenHourAndMinuteTimestamp(model.openTimes.endTimeHour, model.openTimes.endTimeMinute));
            //}
            //else
            //{
            //    model.startTimestamp = (int)activityDungeonData.starttime;
            //    model.endTimestamp = (int)activityDungeonData.endtime;
            //}

            //直接用表数据    限时活动管理器数据不会清掉，会残留前一天的数据
            model.startTimestamp = System.Convert.ToInt32(Function.GetTodayGivenHourAndMinuteTimestamp(model.openTimes.startTimeHour, model.openTimes.startTimeMinute));
            model.endTimestamp = System.Convert.ToInt32(Function.GetTodayGivenHourAndMinuteTimestamp(model.openTimes.endTimeHour, model.openTimes.endTimeMinute));

            //timestampDesc = Function.GetTimeChinese(model.startTimestamp, model.endTimestamp);
            //model.timeDesc = TR.Value(tr_daily_todo_activity_time_Key, timestampDesc);

            //掉落奖励
            if (null != activityDungeonData.drops && null != model.rewardItemIds)
            {
                model.rewardItemIds.Clear();

                for (int i = 0; i < activityDungeonData.drops.Count; i++)
                {
                    model.rewardItemIds.Add(activityDungeonData.drops[i]);
                }
            }

            //特殊限时活动 活动状态判断 同步 GameClient.ActivityDungeonFrame
            if (activityDungeonData.activityInfo != null)
            {
                var actState = activityDungeonData.activityInfo.state;

                if (activityDungeonData.dungeonId == ActivityDungeonFrame.pk3v3CrossDungeonID)
                {
                    actState = ActivityDungeonFrame.Get3v3CrossDungeonActivityState();
                }
                else if (activityDungeonData.dungeonId == ActivityDungeonFrame.guildDungeonID)
                {
                    actState = ActivityDungeonFrame.GetGuildDungeonActivityState();
                }
                else if (activityDungeonData.dungeonId == ActivityDungeonFrame.guildBattleID)
                {
                    actState = ActivityDungeonFrame.GetGuildBattleActivityState();
                }
                else if (activityDungeonData.dungeonId == ActivityDungeonFrame.guildCrossBattleID)
                {
                    actState = ActivityDungeonFrame.GetGuildCrossBattleActivityState();
                }
                else if (activityDungeonData.dungeonId == ActivityDungeonFrame.pk2v2CrossDungeonID)
                {
                    actState = ActivityDungeonFrame.Get2v2CrossDungeonActivityState();
                }

                // TODO 限时活动的等级限制在活动没有开始的时候是无法显示的
                // 由于限时活动只在开始活动之后才同步信息下来  
                //如果当前服务器时间大于等于结束时间戳
                if (actState == eActivityDungeonState.None && model.endTimestamp <= Function.GetCurrTimeStamp())
                {
                    actState = eActivityDungeonState.End;
                }
                model.activityDungeonState = actState;
            }
        }

        private bool _IsTodayOpened(DailyTodoTable.eSubType subType)
        {
            switch (subType)
            {
                case DailyTodoTable.eSubType.DTSTP_ALD_BUDO:
                    return IsAldBudoTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_REWARD_BUDO:
                    return IsMoneyRewardBudoTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_3V3_PK:
                    return IsPk3v3CrossTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_GUILD_BATTLE:
                    return IsGuildBattleTodayOpened();                    
                case DailyTodoTable.eSubType.DTSTP_CROSS_SERVER_GUILD_BATTLE:
                    return IsCrossGuildBattleTodayOpened();                    
                case DailyTodoTable.eSubType.DTSTP_GUILD_DUNGEON:
                    return IsGuildDungeonTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_DIALY_TASK:
                    return IsDailyTaskTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_MAIN_DUNGEON:
                    return IsMainDungeonTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_CITY_MONSTER_DUNGEON:
                    return IsCityMonstorTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_SHENYUAN_DUNGEON:
                    return IsDeepDungeonTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_YUANGU_DUNGEON:
                    return IsAncientDungeonTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_HUNDUN_DUNGEON:
                    return IsWeekHellDungeonTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_XUKONG_DUNGEON:
                    return IsVanityFractureTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_2v2_SCORE_WAR:
                    return IsPk2v2ScoreWarTodayOpened();
                case DailyTodoTable.eSubType.DTSTP_CHIJI_WAR:
                    return IsChiJiWarTodayOpened();
            }

            return false;
        }

        private void _OnGoActivityDailyTodo(DailyTodoActivity model)
        {
            if (null == model)
            {
                return;
            }
            if (!string.IsNullOrEmpty(model.linkinfo))
            {
                ActiveManager.GetInstance().OnClickLinkInfo(model.linkinfo);
            }
            else
            {
                ActivityDungeonSub dungeonSub = ActivityDungeonDataManager.GetInstance().GetSubByActivityDungeonID(model.activityDungeonId);
                //ActivityDungeonSub dungeonSub = _GetActivityDungeonSubByDungeonId(model.activityDungeonId);
                if (null == dungeonSub || null == dungeonSub.table)
                {
                    return;
                }
                ActiveManager.GetInstance().OnClickLinkInfo(dungeonSub.table.GoLinkInfo);
            }

            //关闭界面
            DailyTodoFrame.CloseFrame();
        }

        private ActivityDungeonSub _GetActivityDungeonSubByDungeonId(int activityDungeonId)
        {
            ActivityDungeonSub dungeonSub = null;
            var activityDungeonTabs = ActivityDungeonDataManager.GetInstance().GetTabByActivityType(ActivityDungeonTable.eActivityType.TimeLimit);
            if (activityDungeonTabs != null)
            {
                for (int i = 0; i < activityDungeonTabs.Count; i++)
                {
                    var tab = activityDungeonTabs[i];
                    if (tab == null || tab.subs == null)
                        continue;
                    for (int j = 0; j < tab.subs.Count; j++)
                    {
                        var sub = tab.subs[j];
                        if (sub == null) continue;
                        if (sub.id == activityDungeonId)
                        {
                            dungeonSub = sub;
                            break;
                        }
                    }
                }
            }
            return dungeonSub;
        }

        private void _InitDailyTodoFunction(DailyTodoFunction model, DailyTodoTable table)
        {
            if (model == null || table == null)
            {
                return;
            }

            model.recommendType = table.RecommendNumType;
            model.dayRecommendTotalCount = table.DayRecommendNum;

            model.characterDesc = table.FuncCharacter;

            _UpdateDailyTodoFuncDayRecommendMax(model, table.DayRecommendNum);

            if (model.RecommendState == DailyTodoFuncState.None)
            {
                model.RecommendState = DailyTodoFuncState.Start;                    //初始化 状态为开始
            }
            model.gotoHandler = _OnGoFunctionDailyTodo;
        }

        private void _UpdateDailyTodoFunction(Protocol.DailyTodoInfo info)
        { 
            if (info == null)
            {
                return;
            }
            if (m_FunctionDailyTodoList == null || m_FunctionDailyTodoList.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < m_FunctionDailyTodoList.Count; i++)
            {
                var func = m_FunctionDailyTodoList[i];
                if (func == null)
                    continue;
                if (func.tableId == info.dataId)
                {
                    _UpdateDailyTodoFuncDayRecommendMax(func, (int)info.dayProgMax);

                    bool isWeekEnough = false;
                    bool isDayEnough = false;

                    isWeekEnough = info.weekProgress < info.weekProgMax ? true : false;
                    isDayEnough = info.dayProgress < info.dayProgMax ? true : false;

                    //刷新推荐完成状态
                    if (isWeekEnough && isDayEnough)
                    {
                        func.RecommendState = DailyTodoFuncState.Start;
                    }
                    else
                    {
                        func.RecommendState = DailyTodoFuncState.Finishing;
                    }
                    if (!isWeekEnough)
                    {
                        func.WeekRecommendFinishTimestamp = Function.GetCurrTimeStamp();
                    }
                    else
                    {
                        func.WeekRecommendFinishTimestamp = 0;
                    }
                }
            }            
        }

        private void _UpdateDailyTodoFuncDayRecommendMax(DailyTodoFunction model, int dayRecommedCount)
        {
            if (null == model)
            {
                return;
            }
            model.dayRecommendTotalCount = dayRecommedCount;
            if (model.recommendType == DailyTodoTable.eRecommendNumType.RT_NUMBER)
            {
                model.dayRecommendDesc = TR.Value(tr_daily_todo_recommend_num_Key, dayRecommedCount.ToString());
            }
            else if (model.recommendType == DailyTodoTable.eRecommendNumType.RT_ACTIVE)
            {
                model.dayRecommendDesc = TR.Value(tr_daily_todo_recommend_active_Key, dayRecommedCount.ToString());
            }
        }

        private bool _CheckTodayTimeWillOpen(DailyTodoModel model)
        {
            if (null == model)
            {
                return false;
            }
            int todayWeek = Function.GetTodayWeek();
            if (todayWeek == 0)
            {
                todayWeek = 7;
            }
            //如果设置了开放周天 则打开有限制
            //不设置则不限制
            if (model.openWeeks != null && !model.openWeeks.Contains(todayWeek))
            {
                return false;
            }
            return true;
        }

        private bool _CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType subType)
        {
            if (null == m_DailyTodoTotalDic)
            {
                return false;
            }
            int key  = (int)subType;
            if (m_DailyTodoTotalDic.ContainsKey(key))
            {
                return _CheckTodayTimeWillOpen(m_DailyTodoTotalDic[key]);
            }
            return false;
        }

        private bool _CheckFunctionRecommendFinish(DailyTodoTable.eSubType subType)
        {
            if (m_FunctionDailyTodoList == null || m_FunctionDailyTodoList.Count <= 0)
            {
                return false;
            }
            for (int i = 0; i < m_FunctionDailyTodoList.Count; i++)
            {
                var func = m_FunctionDailyTodoList[i];
                if (func == null)
                    continue;
                if (func.subType == subType)
                {
                    return func.RecommendState == DailyTodoFuncState.End;
                }
            }
            return false;
        }

        private void _OnGoFunctionDailyTodo(DailyTodoFunction model)
        {
            if (null == model)
            {
                return;
            }

            if (!string.IsNullOrEmpty(model.linkinfo))
            {
                ActiveManager.GetInstance().OnClickLinkInfo(model.linkinfo);
            }
            else
            {
                switch (model.subType)
                {
                    case DailyTodoTable.eSubType.DTSTP_DIALY_TASK:
                        break;
                    case DailyTodoTable.eSubType.DTSTP_MAIN_DUNGEON:
                        int mainTask = MissionManager.GetInstance().GetMainTask();
                        MissionManager.GetInstance().AutoTraceTask(mainTask);
                        break;
                    case DailyTodoTable.eSubType.DTSTP_SHENYUAN_DUNGEON:
                        break;
                    case DailyTodoTable.eSubType.DTSTP_YUANGU_DUNGEON:
                        break;
                    case DailyTodoTable.eSubType.DTSTP_HUNDUN_DUNGEON:
                        break;
                    case DailyTodoTable.eSubType.DTSTP_CITY_MONSTER_DUNGEON:
                        AttackCityMonsterDataManager.GetInstance().EnterFindPathProcessByActivityDuplication();
                        break;
                    case DailyTodoTable.eSubType.DTSTP_XUKONG_DUNGEON:
                        Utility.PathfindingYiJieMap();
                        break;
                }
            }

            //关闭界面
            DailyTodoFrame.CloseFrame();
        }


        #region EVENT

        private void _BindNetEvent()
        {
            NetProcess.AddMsgHandler(WorldGetPlayerDailyTodosRes.MsgID, _OnWorldGetPlayerDailyTodosRes);
        }
        
        private void _UnBindNetEvent()
        {
            NetProcess.RemoveMsgHandler(WorldGetPlayerDailyTodosRes.MsgID, _OnWorldGetPlayerDailyTodosRes);
        }

        private void _OnWorldGetPlayerDailyTodosRes(MsgDATA msg)
        {
            if (null == msg)
            {
                return;
            }
            WorldGetPlayerDailyTodosRes ret = new WorldGetPlayerDailyTodosRes();
            ret.decode(msg.bytes);
            var dailyTodoInfos = ret.dailyTodos;
            if (null == dailyTodoInfos || dailyTodoInfos.Length <= 0)
            {
                return;
            }
            for (int i = 0; i < dailyTodoInfos.Length; i++)
            {
                var info = dailyTodoInfos[i];
                if (null == info)
                    continue;
                _UpdateDailyTodoFunction(info);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnDailyTodoFuncStateUpdate);
        }

        #endregion

        #endregion

        #region  PUBLIC METHODS

        public List<DailyTodoActivity> GetShowDailyTodoActivityList()
        {
            if (m_TempShowActivityDailyTodoList != null)
            {
                m_TempShowActivityDailyTodoList.Clear();
            }

            if (m_ActivityDailyTodoList == null)
            {
                return m_TempShowActivityDailyTodoList;
            }
            for (int i = 0; i < m_ActivityDailyTodoList.Count; i++)
            {
                var tempAct = m_ActivityDailyTodoList[i];
                if (tempAct == null)
                    continue;
                if (tempAct.isTodayOpenedHandler != null && 
                    tempAct.isTodayOpenedHandler(tempAct.subType))
                {
                    m_TempShowActivityDailyTodoList.Add(tempAct);
                }
            }
            return m_TempShowActivityDailyTodoList;
        }

        public List<DailyTodoFunction> GetShowDailyTodoFunctionListByCount(int needCount = 3)
        {
            if (m_TempShowFunctionDailyTodoList != null)
            {
                m_TempShowFunctionDailyTodoList.Clear();
            }

            if (m_FunctionDailyTodoList == null || m_FunctionDailyTodoList.Count <= 0)
            {
                return m_TempShowFunctionDailyTodoList;
            }
            for (int i = 0; i < m_FunctionDailyTodoList.Count; i++)
            {
                var tempFunc = m_FunctionDailyTodoList[i];
                if (tempFunc == null)
                    continue; 
                if (tempFunc.isTodayOpenedHandler != null &&
                    tempFunc.isTodayOpenedHandler(tempFunc.subType))
                {
                    if (needCount <= m_TempShowFunctionDailyTodoList.Count)
                    {
                        return m_TempShowFunctionDailyTodoList;
                    }
                    m_TempShowFunctionDailyTodoList.Add(tempFunc);
                }
            }

            //特殊处理 如果获取到当天开启的列表数量小于 需要的数量
            //则从整个列表中从后取值 把  《最近的 并且 已完成的》 加进去
            int tempShowListCount = m_TempShowFunctionDailyTodoList.Count;
            int countDelta = needCount - tempShowListCount;
            if (countDelta > 0)
            {
                List<DailyTodoFunction> tempEndStateFuncList = GamePool.ListPool<DailyTodoFunction>.Get();      
                for (int i = m_FunctionDailyTodoList.Count - 1; i >= 0; i--)
                {
                    var tempFunc = m_FunctionDailyTodoList[i];
                    if (tempFunc == null)
                        continue;
                    //加入条件限制
                    //未开启 + 已结束 + 当天完成周推荐次数
                    if (tempFunc.isTodayOpenedHandler != null &&
                        tempFunc.isTodayOpenedHandler(tempFunc.subType) == false &&
                        tempFunc.RecommendState == DailyTodoFuncState.End &&
                        tempFunc.IsWeekRecommendShow)
                    {
                        tempEndStateFuncList.Add(tempFunc);
                    }
                }
                tempEndStateFuncList.Sort((x, y) => -x.NearlyRecommendEndTimeStamp.CompareTo(y.NearlyRecommendEndTimeStamp));
                if (countDelta > tempEndStateFuncList.Count)
                {
                    return m_TempShowFunctionDailyTodoList;
                }
                for (int j = 0; j < countDelta; j++)
                {
                    m_TempShowFunctionDailyTodoList.Add(tempEndStateFuncList[j]);
                }
                m_TempShowFunctionDailyTodoList.Sort((x, y) => x.CompareTo(y));
                GamePool.ListPool<DailyTodoFunction>.Release(tempEndStateFuncList);
            }
            return m_TempShowFunctionDailyTodoList;
        }

        public void ClearTempShowDailyTodoData()
        {
            if (m_TempShowActivityDailyTodoList != null)
            {
                m_TempShowActivityDailyTodoList.Clear();
            }

            if (m_TempShowFunctionDailyTodoList != null)
            {
                m_TempShowFunctionDailyTodoList.Clear();
            }
        }

        public void ReqDailyTodoFunctionState()
        {
            WorldGetPlayerDailyTodosReq req = new WorldGetPlayerDailyTodosReq();
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void UpdateDailyTodoActivityList()
        {
            if (m_ActivityDailyTodoList == null || m_ActivityDailyTodoList.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < m_ActivityDailyTodoList.Count; i++)
            {
                _UpdateDailyTodoActivity(m_ActivityDailyTodoList[i]);
            }
        }

        #region Other Methods
        #region Todo Activity
        /// <summary>
        /// 阿拉德武道大会 - 今天是否开启
        /// </summary>
        /// <returns></returns>
        public bool IsAldBudoTodayOpened()
        {
            //开启等级
            //if (BudoManager.GetInstance().IsLevelFit == false)
            //根据决斗开始等级判断武道会开启
            if(!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Duel))
            {
                return false;
            }

            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_ALD_BUDO))
            {
                return false;
            }

            return true;
        }

        public bool IsAldBudoOpened()
        {
            if (IsAldBudoTodayOpened() == false)
            {
                return false;
            }

            if (BudoManager.GetInstance().IsOpen == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 3v3积分跨服赛 - 今天是否开放
        /// </summary>
        /// <returns></returns>
        public bool IsPk3v3CrossTodayOpened()
        {
            //等级未解锁
            var systemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
            if (systemValueTableData != null && PlayerBaseData.GetInstance().Level < systemValueTableData.Value)
            {
                return false;
            }

            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_3V3_PK))
            {
                return false;
            }

            return true;
        }

        public bool IsPk3v3CrossOpened()
        {
            if (IsPk3v3CrossTodayOpened() == false)
            {
                return false;
            }
            if (!Pk3v3CrossDataManager.GetInstance().IsIDOpened(ClientApplication.playerinfo.accid))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 赏金武道大会 - 今天是否开放
        /// </summary>
        /// <returns></returns>
        public bool IsMoneyRewardBudoTodayOpened()
        {
            if (MoneyRewardsDataManager.GetInstance().isLevelFit == false)
            {
                return false;
            }

            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_REWARD_BUDO))
            {
                return false;
            }

            return true;
        }

        public bool IsMoneyRewardBudoOpened()
        {
            if (IsMoneyRewardBudoTodayOpened() == false)
            {
                return false;
            }

            if (!MoneyRewardsDataManager.GetInstance().isOpen)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 公会战 - 今天是否开放
        /// </summary>
        /// <returns></returns>
        public bool IsGuildBattleTodayOpened()
        {
            //TODO 解锁等级
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Guild))
            {
                return false;
            }
            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_GUILD_BATTLE))
            {
                return false;
            }
            return true;
        }

        public bool IsGuildBattleOpened()
        {
            if (IsGuildBattleTodayOpened() == false)
            {
                return false;
            }

            if (GuildDataManager.GetInstance().myGuild == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 跨服公会战 - 今天是否开放
        /// </summary>
        /// <returns></returns>
        public bool IsCrossGuildBattleTodayOpened()
        {
            //TODO 解锁等级
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Guild))
            {
                return false;
            }

            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_CROSS_SERVER_GUILD_BATTLE))
            {
                return false;
            }

            return true;
        }

        public bool IsCrossGuildBattleOpened()
        {
            if (IsCrossGuildBattleTodayOpened() == false)
            {
                return false;
            }


            if (GuildDataManager.GetInstance().myGuild == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 公会地下城 - 今天是否开放
        /// </summary>
        /// <returns></returns>
        public bool IsGuildDungeonTodayOpened()
        {
            //TODO 解锁等级
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Guild))
            {
                return false;
            }

            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_GUILD_DUNGEON))
            {
                return false;
            }

            return true;
        }

        public bool IsGuildDungeonOpened()
        {
            if (IsGuildDungeonTodayOpened() == false)
            {
                return false;
            }

            if (GuildDataManager.GetInstance().myGuild == null)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 2v2积分赛 - 今天是否开放
        /// </summary>
        /// <returns></returns>
        public bool IsPk2v2ScoreWarTodayOpened()
        {
            //等级未解锁
            var systemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LEVEL);
            if (systemValueTableData != null && PlayerBaseData.GetInstance().Level < systemValueTableData.Value)
            {
                return false;
            }

            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_2v2_SCORE_WAR))
            {
                return false;
            }

            return true;
        }

        public bool IsPk2v2ScoreWarOpened()
        {
            if (IsPk2v2ScoreWarTodayOpened() == false)
            {
                return false;
            }
            if (!Pk2v2CrossDataManager.GetInstance().IsIDOpened(ClientApplication.playerinfo.accid))
            {
                return false;
            }
            return true;
        }

        public bool IsChiJiWarTodayOpened()
        {
            //等级未解锁
            if (PlayerBaseData.GetInstance().Level < ChijiDataManager.GetInstance().GetChijiOpenLevel())
            {
                return false;
            }

            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_CHIJI_WAR))
            {
                return false;
            }
            return true;
        }

        public bool IsChiJiWarOpened()
        {
            if (IsChiJiWarTodayOpened() == false)
            {
                return false;
            }
            if (!ChijiDataManager.GetInstance().MainFrameChijiButtonIsShow())
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Todo Function

        public bool IsDailyTaskTodayOpened()
        {
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.DailyTask))
            {
                return false;
            }
            if (_CheckFunctionRecommendFinish(DailyTodoTable.eSubType.DTSTP_DIALY_TASK))
            {
                return false;
            }
            return true;
        }

        public bool IsMainDungeonTodayOpened()
        {
            //var nextMissionItem = MissionManager.GetInstance().GetNextMissionItem(PlayerBaseData.GetInstance().Level);
            //if (nextMissionItem == null)
            //{
            //    return false;
            //}
            if (_CheckFunctionRecommendFinish(DailyTodoTable.eSubType.DTSTP_MAIN_DUNGEON))
            {
                return false;
            }
            return true;
        }

        public bool IsCityMonstorTodayOpened()
        {
            if (AttackCityMonsterDataManager.LimitLevel > PlayerBaseData.GetInstance().Level)
            {
                return false;
            }

            //TODO 开启时间戳
            if (!_CheckTodayTimeWillOpenBySubType(DailyTodoTable.eSubType.DTSTP_CITY_MONSTER_DUNGEON))
            {
                return false;
            }

            if (_CheckFunctionRecommendFinish(DailyTodoTable.eSubType.DTSTP_CITY_MONSTER_DUNGEON))
            {
                return false;
            }

            return true;
        }

        public bool IsDeepDungeonTodayOpened()
        {
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.DeepDungeon))
            {
                return false;
            }

            if (_CheckFunctionRecommendFinish(DailyTodoTable.eSubType.DTSTP_SHENYUAN_DUNGEON))
            {
                return false;
            }

            return true;
        }

        public bool IsAncientDungeonTodayOpened()
        {
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.AncientDungeon))
            {
                return false;
            }

            if (_CheckFunctionRecommendFinish(DailyTodoTable.eSubType.DTSTP_YUANGU_DUNGEON))
            {
                return false;
            }

            return true;
        }

        public bool IsWeekHellDungeonTodayOpened()
        {
            if (ChallengeUtility.GetChallengeDungeonLockLevelByModelType(DungeonModelTable.eType.WeekHellModel) > PlayerBaseData.GetInstance().Level)
            {
                return false;
            }

            if (_CheckFunctionRecommendFinish(DailyTodoTable.eSubType.DTSTP_HUNDUN_DUNGEON))
            {
                return false;
            }

            return true;
        }

        public bool IsVanityFractureTodayOpened()
        {
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.VanityFracture))
            {
                return false;
            }

            if (_CheckFunctionRecommendFinish(DailyTodoTable.eSubType.DTSTP_XUKONG_DUNGEON))
            {
                return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region PUBLIC STATIC METHODS

        #endregion

        #endregion
    }
}
