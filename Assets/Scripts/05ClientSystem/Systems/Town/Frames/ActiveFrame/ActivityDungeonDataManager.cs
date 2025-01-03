using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using ProtoTable;
using Protocol;

namespace GameClient
{
    class ActivityInfoNode : IComparable<ActivityInfoNode>
    {
        public int                   activityId   { get; private set;}

        public ActivityInfo          activityInfo { get; private set;}

        public eActivityDungeonState state        { get; private set;}

        public int                   level        { get; private set;}
        public string                servername   { get; private set;}
        public UInt32                pretime      { get; private set;}
        public UInt32                starttime    { get; private set;}
        public UInt32                endtime      { get; private set;}

        public ActivityInfoNode(int activityId)
        {
            Logger.LogProcessFormat("[活动副本] 构造ActivityInfoNode活动ID {0}", activityId);

            this.activityId = activityId;

            _findActivityInfoData();
            _initFromActivityInfoData();
            _updateActivityState();
        }

        private void _findActivityInfoData()
        {
            if (ActiveManager.GetInstance().allActivities.ContainsKey(activityId))
            {
                Logger.LogProcessFormat("[活动副本] 查找活动数据活动ID {0}", activityId);

                activityInfo = ActiveManager.GetInstance().allActivities[activityId];
            }
            else
            {
               // Logger.LogErrorFormat("[活动副本] 查找活动数据活动ID {0} 失败", activityId);
            }
        }

        private void _initFromActivityInfoData()
        {
            if (null == activityInfo)
            {
                return;
            }

            level        = activityInfo.level;
            pretime      = activityInfo.preTime;
            starttime    = activityInfo.startTime;
            endtime      = activityInfo.dueTime;
            servername   = activityInfo.name;

            Logger.LogProcessFormat("[活动副本] 初始化数据 等级:{0}, 准备时间:{1}, 开始时间:{2}, 结束时间:{3}, 服务器名字:{4}", level, pretime, starttime, endtime, servername);
        }

        private void _updateActivityState()
        {
            state = eActivityDungeonState.None;

            if (null == activityInfo)
            {
                //Logger.LogErrorFormat("[活动副本] activityInfo活动数据为空 更新活动状态 {0}", state); 
                return;
            }

            StateType serverSt = (StateType)activityInfo.state;

            switch (serverSt)
            {
                case StateType.End:
                    state      = eActivityDungeonState.End;
                    break;
                case StateType.Ready:
                    state      = eActivityDungeonState.Prepare;
                    break;
                case StateType.Running:
                    state      = eActivityDungeonState.Start;
                    break;

            }

            if (state == eActivityDungeonState.Start || 
                state == eActivityDungeonState.Prepare)
            {
                if (level > PlayerBaseData.GetInstance().Level)
                {
                    state = eActivityDungeonState.LevelLimit;
                }
            }

            Logger.LogProcessFormat("[活动副本] 更新活动状态 {0}", state); 
        }

        public void UpdateActivity()
        {
            _findActivityInfoData();
            _updateActivityState();
        }

        public bool IsValidActivityInfo()
        {
            return eActivityDungeonState.None != state;
        }

#region IComparable implementation
        public int CompareTo(ActivityInfoNode other)
        {
            if (pretime != other.pretime)
            {
                return _cmpuint(pretime, other.pretime);
            }
            else if (starttime != other.starttime)
            {
                return _cmpuint(starttime, other.starttime);
            }
            else
            {
                return _cmpuint(endtime, other.endtime);
            }
        }

        private int _cmpuint(UInt32 a, UInt32 b)
        {
            if (a > b)
            {
                return 1;
            }
            else if (a == b)
            {
                return 0;
            }
            else 
            {
                return -1;
            }
        }
#endregion
    }

    public class ActivityMutiInfo : IComparable<ActivityMutiInfo>
    {
        private List<ActivityInfoNode> activitys = new List<ActivityInfoNode>();
        private IList<int> activityIds;


        public ActivityMutiInfo(IList<int> activityIds)
        {
            this.activityIds = activityIds;

            _initActivitys();
        }

        private void _initActivitys()
        {
            if (null == activityIds)
            {
                return;
            }

            for (int i = 0; i < activityIds.Count; ++i)
            {
                _addOneActivityAndSort(activityIds[i]);
            }
        }

        private void _addOneActivityAndSort(int activityId)
        {
            Logger.LogProcessFormat("[活动副本] 添加活动ID {0}", activityId);

            ActivityInfoNode node = new ActivityInfoNode(activityId);

            //if (node.IsValidActivityInfo())
            {
                activitys.Add(node);
            }

            activitys.Sort();
        }

        public int                      activityId
        {
            get 
            {
                ActivityInfoNode node = _findNearListNodeByServerTime();

                if (null != node)
                {
                    return node.activityId;
                } 

                return -1;
            }
        }

        public eActivityDungeonState    state 
        {
            get 
            {
                ActivityInfoNode node = _findNearListNodeByServerTime();

                if (null != node)
                {
                    return node.state;
                }

                return eActivityDungeonState.None;
            }
        }

        public int                      level
        {
            get 
            {
                ActivityInfoNode node = _findNearListNodeByServerTime();

                if (null != node)
                {
                    return node.level;
                }

                return 0;
            }
        }

        public string                   servername
        {
            get 
            {
                ActivityInfoNode node = _findNearListNodeByServerTime();

                if (null != node)
                {
                    return node.servername;
                }

                return string.Empty;
            }
        }

        public UInt32                   pretime
        {
            get 
            {
                ActivityInfoNode node = _findNearListNodeByServerTime();

                if (null != node)
                {
                    return node.pretime;
                }

                return 0;
            }
        }

        public UInt32                   starttime
        {
            get 
            {
                ActivityInfoNode node = _findNearListNodeByServerTime();

                if (null != node)
                {
                    return node.starttime;
                }

                return 0;
            }
        }

        public UInt32                   endtime
        {
            get 
            {
                ActivityInfoNode node = _findNearListNodeByServerTime();

                if (null != node)
                {
                    return node.endtime;
                }

                return 0;
            }
        }
     

        private ActivityInfoNode _findNearListNodeByServerTime()
        {
            uint serverTime = TimeManager.GetInstance().GetServerTime();

            for (int i = 0; i < activitys.Count; ++i)
            {
                if (serverTime < activitys[i].endtime)
                {
                    return activitys[i];
                }
            }

            return null;
        }

        public void UpdateActivityByID(int activityId)
        {
           ActivityInfoNode node = _findActivityInfoByID(activityId);
           if (null != node)
           {
               node.UpdateActivity();
           }
        }

        private ActivityInfoNode _findActivityInfoByID(int activityId)
        {
            for (int i = 0; i < activitys.Count; ++i)
            {
                if (activitys[i].activityId == activityId)
                {
                    return activitys[i];
                }
            }

            return null;
        }

#region IComparable implementation
        public int CompareTo(ActivityMutiInfo other)
        {
            return 0;
        }
#endregion

        public override string ToString()
        {
            StringBuilder build = new StringBuilder();

            build.Append("活动ID:");

            for (int i = 0; i < activityIds.Count; ++i)
            {
                build.Append(activityIds[i]);
                build.Append(",");
            }

            build.AppendLine();

            build.Append("活动详情:");
            for (int i = 0; i < activitys.Count; ++i)
            {
                build.AppendFormat("ID:{0} 名字{1} 等级:{2} 状态:{3} 准备时间:{4} 开始时间:{5} 结束时间:{6}", 
                        activitys[i].activityId,
                        activitys[i].servername,
                        activitys[i].level,
                        activitys[i].state,
                        Utility.ToUtcTime2Local(activitys[i].pretime).ToString("tt yyMMdd hh:mm:ss",    Utility.cultureInfo),
                        Utility.ToUtcTime2Local(activitys[i].starttime).ToString("tt yyMMdd hh:mm:ss",  Utility.cultureInfo),
                        Utility.ToUtcTime2Local(activitys[i].endtime).ToString("tt yyMMdd hh:mm:ss",    Utility.cultureInfo)
                );
            }

            return build.ToString();
        }

    }

    class BaseActivityDungeonUpdateData : IActivityDungeonUpdateData
    {
#region IActivityDungeonUpdateData implementation
        public bool IsChanged()
        {
            return false;
        }

        public void Update(float delta)
        {

        }

        public void Reset()
        {

        }
#endregion

    }

    class ActivityDungeonDeadTowerUpdateData : IActivityDungeonUpdateData
    {
        private bool mLastIsTimeEnd = false;
        private uint mWaitTime = 0;

        public ActivityDungeonDeadTowerUpdateData()
        {
            _init();
        }

        private void _init()
        {
            mState = eState.onWaitEnd;

            mWaitTime = _getPersistendTime();

            mLastIsTimeEnd = _isTimeEnd();

            if (mLastIsTimeEnd) // 时间到了
            {
                if (!_isVisisted())
                {
                    mState = eState.onWaitView;
                }
                else
                {
                    mState = eState.onNone;
                }
            }
            else
            {
                mState = eState.onWaitEnd;
            }
        }

        private enum eState 
        {
            onNone,
            onWaitEnd,
            onWaitView,
        }

        private eState mState = eState.onNone;

        private bool _isTimeEnd()
        {
            return mWaitTime < TimeManager.GetInstance().GetServerTime();
        }

        private uint _getPersistendTime()
        {
            return ActivityDungeonPersistentDataManager.instance.GetWipeEndTime
            (
                PlayerBaseData.GetInstance().RoleID
            );
        }

        private bool _isVisisted()
        {
            //uint time = _getPersistendTime();

            //if (time != PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime)
            //{
            //    Logger.LogErrorFormat("[ActivityDungeonDeadTowerUpdateData] 文件中数据和PlayerData中数据不同 {0} != {1}", time, PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime);
            //}

            return ActivityDungeonPersistentDataManager.instance.HasWipeEndVisited
            (
                PlayerBaseData.GetInstance().RoleID
            );
        }

        private bool _isDeathTowerWipeoutEndTimeChanged()
        {
            return mWaitTime < _getPersistendTime();
        }

#region IActivityDungeonUpdateData implementation
        public bool IsChanged()
        {
            return mState == eState.onWaitView;
        }

        public void Update(float delta)
        {
            switch (mState)
            {
                case eState.onNone:
                    if (_isDeathTowerWipeoutEndTimeChanged())
                    {
                        //ActivityDungeonPersistentDataManager.instance.UpdateWipeEndTimeAndSave
                        //( 
                        //    PlayerBaseData.GetInstance().RoleID,
                        //    PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime
                        //);
                        _init();
                    }
                    break;
                case eState.onWaitEnd:
                    if (_isTimeEnd())
                    {
                        mState = eState.onWaitView;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonDeadTowerWipeEnd);
                        RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ActivityDungeon);
                    }
                    break;

            }
        }

        public void Reset()
        {
            if (mState == eState.onWaitView)
            {
                mState = eState.onNone;

                ActivityDungeonPersistentDataManager.instance.SetWipeEndTimeVistedAndSave(PlayerBaseData.GetInstance().RoleID);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonDeadTowerWipeEnd);

                RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ActivityDungeon);
            }
        }
#endregion
    }

    public class ActivityDungeonSub : IComparable<ActivityDungeonSub>  
    {
        public string                tabname;

        /// 从表中获取
        public int                   id;
        public ActivityDungeonTable  table;
        public DungeonTable          dungeonTable;
        public int                   priority;
        public int                   dungeonId;
        public int                   guidedungenId;
        public string                singleIcon;
        public bool                  ishell = false;

        public string                name;
        public string                mode;
        public string                background;
        public string                desc;
        public string                extradesc;
        public IList<int> drops = new List<int>();

        public ProtoTable.ActivityDungeonTable.eActivityType type;

        /// 从ActivityInfo中获取
        
        public ActivityMutiInfo      activityInfo { get; private set; }
        public int                   activityId   { get; private set; }

        public bool                  isshowred    { get; private set; }
        public bool                  hasleftcount { get; private set; }
        public bool                  isfinish     { get; private set; }

        public IActivityDungeonUpdateData updateData { get; private set; }

        public eActivityDungeonState state        { get; private set; }

		public int                   level        
		{ 
			get 
			{
                if(IsUltimateChallengeActivity())
                {
                    return Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.KingTower);
                }
				if (null != dungeonTable)
				{
					return dungeonTable.MinLevel;
				}
				
				return 0;
			}

			private	set { } 
		}

        public string                servername { get; private set; }
        public UInt32                pretime    { get; private set; }
        public UInt32                starttime  { get; private set; }
        public UInt32                endtime    { get; private set; }

        private bool _isAlreadyShowRed;

        // TODO 修改红点状态
        public void UpdateStateAndRedPoint()
        {
            if (null == table)
            {
                return;
            }
            //_updateActivityState();
            //activityInfo.UpdateActivityByID(
            _updateRedPoint();
            UpdateState();
        }

        private void _updateRedPoint()
        {
            isshowred = _isShowRed(table);
            hasleftcount = _hasLeftCount();
            isfinish = _hasFinishActivity();
        }

        private bool _hasFinishActivity()
        {
            if (hasleftcount)
            {
                return false;
            }

            if(IsUltimateChallengeActivity())
            {
                return ActivityDataManager.GetInstance().GetUltimateChallengeLeftCount() == 0;
            }
            if (null != dungeonTable)
            {
                if (dungeonTable.SubType == DungeonTable.eSubType.S_SIWANGZHITA)
                {
                    return CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_TOP_FLOOR) == ChapterUtility.kDeadTowerTopLevel;
                }
            }

            return true;
        }

        public bool IsUltimateChallengeActivity()
        {
            return ActivityDungeonDataManager.IsUltimateChallengeActivity(table);
        }
        // TODO modify the state function to the real update state
        public void UpdateState()
        {
            if (state == eActivityDungeonState.Start || state == eActivityDungeonState.LevelLimit)
            {
                if (level > PlayerBaseData.GetInstance().Level)
                {
                    state = eActivityDungeonState.LevelLimit;
                }
                else
                {
                    state = eActivityDungeonState.Start;
                }
            }
        }

        private bool _isCanUseSingle(ActivityDungeonTable table)
        {
            return table.ActivityType == ActivityDungeonTable.eActivityType.Daily;
        }

        private bool _isShowRed(ActivityDungeonTable table)
        {
            if (null != table)
            {
                switch (table.ActivityType)
                {
                    case ActivityDungeonTable.eActivityType.Daily:
                        {
                            // 这里是由于高棉提新需求，要显示死亡之塔扫荡完成的红点
                            // by dd
                            // 蛋疼
                            return state == eActivityDungeonState.Start && /*_hasLeftCount() &&*/ updateData.IsChanged();
                        }
                    case ActivityDungeonTable.eActivityType.TimeLimit:
                        {
                            //显示活动是否显示红点 特殊处理
                            if (ActivityDungeonDataManager.GetInstance().mIsLimitActivityRedPoint == false)
                                return false;

                            //活动没有开始
                            if (state != eActivityDungeonState.Start)
                                return false;

                            //限时活动中的怪物攻城类型活动
                            if (ActivityDungeonDataManager.GetInstance().IsActivityDungeonBeAttackCityMonster(table) ==
                                true)
                            {
								//已经完成total次数
                                if (AttackCityMonsterDataManager.GetInstance().IsAlreadyFinishTotalBeatTimes() == true)
                                    return false;
                            }

							//已经显示过
                            if (_isAlreadyShowRed == true)
                                return false;

                            return true;
                        }
                }
            }

            return false;
        }

        public void SetIsAlreadyShowRedFlag()
        {
            if (isshowred == true)
            {
                if (_isAlreadyShowRed == false)
                {
                    _isAlreadyShowRed = true;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshLimitTimeState);
                }
            }
        }

        private bool _hasLeftCount()
        {
            if (null != dungeonTable)
            {
                IActivityConsumeData data = null;

                if (DungeonTable.eSubType.S_SIWANGZHITA == dungeonTable.SubType)
                {
                    data = new DeadTowerActivityConsumeData(table.DungeonID);
                }
                else if(IsUltimateChallengeActivity())
                {
                    data = new FinalTestActivityConsumeData(table.DungeonID);
                }
                else
                {
                    data = new NormalActivityConsumeData(table.DungeonID);
                }

                return data.GetLeftCount() > 0;
            }

            return false;
        }


        public ActivityDungeonSub(int id, string tabname)
        {
            this.tabname = tabname;
            this.id      = id;

            _findTableDataAndInitFromData();
            _createMutiNodeAndInitFormData();

            _createUpdateData();
            //_updateStateAndRedPoint();
            UpdateStateAndRedPoint();
        }

        public void Update(float delta)
        {
            if (null != updateData)
            {
                updateData.Update(delta);
            }
        }

        private void _findTableDataAndInitFromData()
        {
            _findActivityTableData();
            _initFromTableData();
        }
        
        private void _findActivityTableData()
        {
            table        = TableManager.instance.GetTableItem<ActivityDungeonTable>(id);
        }

        private void _initFromTableData()
        {
            if (null == table)
            {
                return;
            }

            priority        = table.SubPriority;
            dungeonId       = table.DungeonID;
            dungeonTable    = TableManager.instance.GetTableItem<DungeonTable>(dungeonId);
            guidedungenId   = -1;
            background      = table.ImagePath;
            singleIcon      = table.SingleTabIcon;
            extradesc       = table.ExtraDescription;
            type            = table.ActivityType;
            mode            = table.Mode;
            desc            = _getDesc();
            name            = _getName();
            drops           = _getDrops();
            ishell          = _getIsHellMode();
        }

        private string _getDesc()
        {
            string desc = "";

            if (null != table)
            {
                switch (table.DescriptionType)
                {
                    case ActivityDungeonTable.eDescriptionType.DungeonDescription:
                        if (_isCanUseSingle(table))
                        {
                            DungeonTable dta = TableManager.instance.GetTableItem<DungeonTable>(table.DungeonID);
                            if (null != dta)
                            {
                                desc = dta.Description;
                            }
                        }
                        break;
                    case ActivityDungeonTable.eDescriptionType.CustomDescription:
                        desc = table.Description;
                        break;
                }
            }

            return desc;
        }

        private string _getName()
        {
            string name = "";

            if (null != table)
            {
                switch (table.SubNameType)
                {
                    case ActivityDungeonTable.eSubNameType.DungeonName:
                        if (_isCanUseSingle(table))
                        {
                            DungeonTable dta = TableManager.instance.GetTableItem<DungeonTable>(table.DungeonID);
                            if (null != dta)
                            {
                                name = dta.Name;
                            }
                        }
                        break;
                    case ActivityDungeonTable.eSubNameType.CustomName:
                        name = table.SubName;
                        break;
                }
            }

            return name;
        }

        private bool _getIsHellMode()
        {
            if (null != table)
            {
                switch (table.ActivityType)
                {
                    case ActivityDungeonTable.eActivityType.Daily:
                        {
                            DungeonTable duntable = TableManager.instance.GetTableItem<DungeonTable>(table.DungeonID);
                            if (null != duntable)
                            {
                                return (duntable.SubType == DungeonTable.eSubType.S_HELL || duntable.SubType == DungeonTable.eSubType.S_HELL_ENTRY);
                            }
                        }
                        break;
                }
            }

            return false;
        }

        private IList<int> _getDrops()
        {
            

            if (null != table)
            {
                switch (table.DropType)
                {
                    case ActivityDungeonTable.eDropType.CustomDrop:
                        return table.DropItems;
                        break;
                    case ActivityDungeonTable.eDropType.DungeonDrop:
                        if (_isCanUseSingle(table))
                        {
                            DungeonTable dta = TableManager.instance.GetTableItem<DungeonTable>(table.DungeonID);
                            if (null != dta)
                            {
                              return  dta.DropItems;
                            }
                        }
                        break;
                }
            }

            return new List<int>();
        }

        private void _createMutiNodeAndInitFormData()
        {
            //Logger.LogErrorFormat("[活动副本] 创建MutiNodeAndInitFormData {0}", table.ID);

            activityInfo = new ActivityMutiInfo(table.ActivityID);

            if (null != activityInfo)
            {
                state        = activityInfo.state;
                activityId   = activityInfo.activityId;
                level        = activityInfo.level;
                pretime      = activityInfo.pretime;
                starttime    = activityInfo.starttime;
                endtime      = activityInfo.endtime;
                servername   = activityInfo.servername;

                Logger.LogProcessFormat(activityInfo.ToString());
            }
        }

        private void _updateStateAndRedPoint()
        {
            if (null != table)
            {
                isshowred = _isShowRed(table);
            }

            hasleftcount = _hasLeftCount();
            isfinish = _hasFinishActivity();
        }

        private void _createUpdateData()
        {
            if (_isDeadTower())
            {
                updateData = new ActivityDungeonDeadTowerUpdateData();
            }
            else
            {
                updateData = new BaseActivityDungeonUpdateData();
            }
        }

        private bool _isDeadTower()
        {
            if (null != dungeonTable)
            {
                return ProtoTable.DungeonTable.eSubType.S_SIWANGZHITA == dungeonTable.SubType;
            }

            return false;
        }


        public string GetDungeonRecommendLevel()
        {
            if (null != dungeonTable)
            {
                return dungeonTable.RecommendLevel;
            }

            return string.Empty;
        }

        private int _cmpuint(UInt32 a, UInt32 b)
        {
            if (a > b)
            {
                return 1;
            }
            else if (a == b)
            {
                return 0;
            }
            else 
            {
                return -1;
            }
        }

        public int CompareTo(ActivityDungeonSub other)
        {
            do 
            {
                // 开启状态
                if (state != other.state)
                {
                    return (int)state - (int)other.state;
                }

                if (endtime != other.endtime)
                {
                    return _cmpuint(endtime, other.endtime);
                }

                if (level != other.level)
                {
                    return level - other.level;
                }

                break;

            } while (true);

            // 默认排序表中配置，不得重复
            return priority - other.priority;
        }
    }

    public class ActivityDungeonTab : IComparable<ActivityDungeonTab>
    {

        public int                      priority;
        public string                   name;

        public ActivityDungeonTab()
        {
            //chlevel    = PlayerBaseData.GetInstance().Level;
            //servertime = TimeManager.GetInstance().GetServerTime();
        }

        public List<ActivityDungeonSub> subs = new List<ActivityDungeonSub>();

        public void AddOneSub(int id)
        {
            ActivityDungeonSub sub = subs.Find(x=>{return x.id == id;});
            if (null == sub)
            {
                subs.Add(new ActivityDungeonSub(id, name));
                subs.Sort();
            }
        }

        public int CompareTo(ActivityDungeonTab other)
        {
            //if (this.priority != other.priority)
            {
                return this.priority - other.priority;
            }
        }
    }

    public class ActivityDungeonDataManager : DataManager<ActivityDungeonDataManager>
    {
        protected List<ActivityDungeonTab>  mTabs = new List<ActivityDungeonTab>();

        protected bool mIsInitedTableData = false;

        public bool mIsLimitActivityRedPoint = false;

        private int iRotteneterHellActivityID = 25000;

        public List<ActivityDungeonTab> tabs
        {
            get 
            {
                return mTabs;
            }
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.ActivityDungeonDataManager;
        }

        public override void Initialize()
        {
            Logger.LogProcessFormat("[活动副本] 活动数据初始化");
            _bindEvent();
            _loadTableData();
            mIsLimitActivityRedPoint = true;
        }

        public override void Clear()
        {
            Logger.LogProcessFormat("[活动副本] 活动数据反初始化");

            _unBindEvent();
            _unloadTableData();
            mIsLimitActivityRedPoint = true;
        }

        private void _loadTableData()
        {
            Dictionary<int, object> dicts = TableManager.instance.GetTable<ActivityDungeonTable>();
            var iter = dicts.GetEnumerator();
            while (iter.MoveNext())
            {
                ActivityDungeonTable adt = iter.Current.Value as ActivityDungeonTable;
                _addOne(adt.ID);
            }
        }

        private void _unloadTableData()
        {
            for (int i = 0; i < mTabs.Count; ++i)
            {
                mTabs[i].subs.Clear();
            }

            mTabs.Clear();
        }

        private void _addOne(int id)
        {
            Logger.LogProcessFormat("[活动副本] 活动数据添加 {0}", id);
            ActivityDungeonTable table = TableManager.instance.GetTableItem<ActivityDungeonTable>(id);

            if (IsUltimateChallengeActivity(table))     //如果是堕落之塔就不添加进数据
            {
                return;
            }

            if (null != table)
            {
                ActivityDungeonTab tab = mTabs.Find(x=>{ return x.name == table.TabName; });
                if (null == tab)
                {
                    tab = new ActivityDungeonTab();
                    tab.priority = table.TabPriority;
                    tab.name     = table.TabName;

                    mTabs.Add(tab);
                    mTabs.Sort();
                }

                tab.AddOneSub(id);
            }
        }
		private void _updateActivityEvent(UIEvent ui)
		{
			uint id = (uint)ui.Param1;
			_updateActivity (id);
		}
		private void _updateActivity(uint id)
        {

            for (int i = 0; i < mTabs.Count; ++i)
            {
                var tab = mTabs[i];
				var unit = tab.subs.Find(x=> { return x.table.ActivityID.Contains((int)id); });

				if (null != unit) {
					Logger.LogProcessFormat("[活动副本] 活动数据更新 {0}", id);

					tab.subs.Remove (unit);
					tab.AddOneSub (unit.id);
					UIEventSystem.GetInstance ().SendUIEvent (EUIEventID.ActivityDungeonUpdate);
                    if (id == iRotteneterHellActivityID)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonStateUpdate);
                    }
                    break;
				} 
            }
        }

        private void _updateStateAndRedState(UIEvent ui)
        {
            for (int i = 0; i < mTabs.Count; ++i)
            {
                var tab = mTabs[i];

                for (int j = 0; j < tab.subs.Count; ++j)
                {
                    tab.subs[j].UpdateStateAndRedPoint();
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.ActivityDungeon);
        }

        private void _bindEvent()
        {
			//ActiveManager.GetInstance().onActivityUpdate += _onUpdateActiivty;
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, _updateActivityEvent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _updateStateAndRedState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _updateStateAndRedState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityDungeonDeadTowerWipeEnd, _updateStateAndRedState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshLimitTimeState, _updateStateAndRedState);
        }

		//private void _onUpdateActiivty(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
		//{
		//	_updateActivity((uint)data.ID);
		//}

        private void _unBindEvent()
        {
			//ActiveManager.GetInstance().onActivityUpdate -= _onUpdateActiivty;

			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, _updateActivityEvent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _updateStateAndRedState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _updateStateAndRedState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityDungeonDeadTowerWipeEnd, _updateStateAndRedState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshLimitTimeState, _updateStateAndRedState);
        }

        public bool IsShowRed()
        {
            for (int i = 0; i < mTabs.Count; ++i)
            {
                if (IsTabShowRed(mTabs[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public void Update(float delta)
        {
            for (int i = 0; i < mTabs.Count; ++i)
            {
                for (int j = 0; j < mTabs[i].subs.Count; ++j)
                {
                    mTabs[i].subs[j].Update(delta);
                }
            }
        }

        public bool IsTabShowRed(ActivityDungeonTab tab)
        {
            return GetTabRedCount(tab) > 0;
        }

        public int GetTabRedCount(ActivityDungeonTab tab)
        {
            int cnt = 0;

            for (int i = 0; i < tab.subs.Count; ++i)
            {
                if (tab.subs[i].isshowred)
                {
                    cnt++;
                }
            }

            return cnt;
        }

        public ActivityDungeonSub GetSubByDungeonID(int did)
        {
            for (int i = 0; i < mTabs.Count; ++i)
            {
                for (int j = 0; j < mTabs[i].subs.Count; ++j)
                {
                    if (did == mTabs[i].subs[j].dungeonId)
                    {
                        return mTabs[i].subs[j];
                    }
                }
            }

            return null;
        }

        public ActivityDungeonSub GetSubByActivityID(int activityId)
        {
            for (int i = 0; i < mTabs.Count; ++i)
            {
                for (int j = 0; j < mTabs[i].subs.Count; ++j)
                {
                    if (activityId == mTabs[i].subs[j].activityId)
                    {
                        return mTabs[i].subs[j];
                    }
                }
            }

            return null;
        }

        public ActivityDungeonSub GetSubByActivityDungeonID(int activityDungeonId)
        {
            if (null == mTabs)
            {
                return null;
            }
            for (int i = 0; i < mTabs.Count; ++i)
            {
                for (int j = 0; j < mTabs[i].subs.Count; ++j)
                {
                    if (activityDungeonId == mTabs[i].subs[j].id)
                    {
                        return mTabs[i].subs[j];
                    }
                }
            }

            return null;
        }

        //每日地下城的相关数据
        public ActivityDungeonTab GetDailyDungeonTab()
        {
            var dailyType = ActivityDungeonTable.eActivityType.Daily;
            var tabs = GetTabByActivityType(dailyType);

            if (_isTabsEmptyOrNull(tabs))
                return null;

            //每日地下城相关数据
            for (var i = 0; i < tabs.Count; i++)
            {
                if (tabs[i] != null && tabs[i].subs != null && tabs[i].subs.Count > 0)
                {
                    var activityDungeonTable = tabs[i].subs[0].table;
                    if (activityDungeonTable != null && activityDungeonTable.DailyActivityType == 1)
                        return tabs[i];
                }

                ////应该用类型来决定,不应该是字符串
                //if (tabs[i].name == "每日地下城")
                //{
                //    return tabs[i];
                //}
            }

            return null;
        }

        public ActivityDungeonTab GetTabByDungeonID(ActivityDungeonTable.eActivityType type, int dungeonId)
        {
            List<ActivityDungeonTab> tabs = GetTabByActivityType(type);

            if (_isTabsEmptyOrNull(tabs))
            {
                return null;
            }

            for (int i = 0; i < tabs.Count; ++i)
            {
                for (int j = 0; j < tabs[i].subs.Count; ++j)
                {
                    if (dungeonId == tabs[i].subs[j].dungeonId)
                    {
                        return tabs[i];
                    }
                }
            }

            return null;
        }

        public ActivityDungeonTab GetTabByActivityID(ActivityDungeonTable.eActivityType type, int activityId)
        {
            List<ActivityDungeonTab> tabs = GetTabByActivityType(type);

            if (_isTabsEmptyOrNull(tabs))
            {
                return null;
            }

            for (int i = 0; i < tabs.Count; ++i)
            {
                for (int j = 0; j < tabs[i].subs.Count; ++j)
                {
                    if (activityId == tabs[i].subs[j].activityId)
                    {
                        return tabs[i];
                    }
                }
            }

            return null;
        }

        public ActivityDungeonTab GetTabByDungeonIDDefaultFirst(ActivityDungeonTable.eActivityType type, int dungeonId)
        {
            ActivityDungeonTab tab = GetTabByDungeonID(type, dungeonId);

            if (null == tab)
            {
                List<ActivityDungeonTab> tabs = GetTabByActivityType(type);

                if (!_isTabsEmptyOrNull(tabs))
                {
                    return tabs[0];
                }
            }

            return tab;
        }

        public ActivityDungeonTab GetTabByAcitivtyIDDefaultFirst(ActivityDungeonTable.eActivityType type, int activityId)
        {
            ActivityDungeonTab tab = GetTabByActivityID(type, activityId);

            if (null == tab)
            {
                List<ActivityDungeonTab> tabs = GetTabByActivityType(type);

                if (!_isTabsEmptyOrNull(tabs))
                {
                    return tabs[0];
                }
            }

            return tab;
        }

        public List<ActivityDungeonTab> GetTabByActivityType(ActivityDungeonTable.eActivityType type)
        {
            List<ActivityDungeonTab> tabs = new List<ActivityDungeonTab>();

            for (int i = 0; i < mTabs.Count; ++i)
            {
                if (mTabs[i].subs.Count > 0 && type == mTabs[i].subs[0].type)
                {
                    tabs.Add(mTabs[i]);
                }
            }

            tabs.Sort();

            return tabs;
        }

        public List<ActivityDungeonSub> GetSubByActivityType(ActivityDungeonTable.eActivityType type)
        {
            List<ActivityDungeonSub> subs = new List<ActivityDungeonSub>();

            List<ActivityDungeonTab> tabs = GetTabByActivityType(type);

            for (int i = 0; i < tabs.Count; ++i)
            {
                subs.AddRange(tabs[i].subs);
            }

            subs.Sort();

            return subs;
        }

        public bool IsShowRedByActivityType(ActivityDungeonTable.eActivityType type)
        {
            return GetRedCountByActivityType(type) > 0;
        }

        public int GetRedCountByActivityType(ActivityDungeonTable.eActivityType type)
        {
            int cnt = 0;

            List<ActivityDungeonTab> tabs = GetTabByActivityType(type);
            for (int i = 0; i < tabs.Count; ++i)
            {
                cnt += GetTabRedCount(tabs[i]);
            }

            return cnt;
        }

        private bool _isTabsEmptyOrNull(List<ActivityDungeonTab> tabs)
        {
            return null == tabs || tabs.Count <= 0;
        }

        //当前活动副本是否为攻城怪物
        public bool IsActivityDungeonBeAttackCityMonster(ActivityDungeonTable table)
        {
            if (table == null)
                return false;

            if (AttackCityMonsterDataManager.GetInstance().IsAttackCityMonsterStr(table.GoLinkInfo) == true)
                return true;
            return false;
        }

        public static int UltimateChallengeDungeonID = 10087;

        // 是否是勇者试炼活动
        public static bool IsUltimateChallengeActivity(ActivityDungeonTable table)
        {
            if (table == null)
                return false;
            DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(table.DungeonID);
            if(dungeonTable == null)
            {
                return false;
            }
            return dungeonTable.SubType == DungeonTable.eSubType.S_FINALTEST_PVE;
        }
        //攻城怪物副本自动寻路找到攻城怪物
        public void ActivityDungeonFindAttackCityMonster()
        {
           AttackCityMonsterDataManager.GetInstance().EnterFindPathProcessByActivityDuplication();
        }

        //当前活动副本是否为吃鸡荣耀战场
        public bool IsActivityDungeonBeHonorBattleField(ActivityDungeonTable table)
        {
            if (table == null)
            {
                return false;
            }

            if (ChijiDataManager.GetInstance().IsHonorBattleFieldStr(table.GoLinkInfo) == true)
            {
                return true;
            }

            return false;
        }
        //当前活动副本是否为公平竞技场
        public bool IsActivityDungeonFairDuelField(ActivityDungeonTable table)
        {
            if (table == null)
            {
                return false;
            }

            if (FairDuelDataManager.GetInstance().IsFairDuelFieldStr(table.GoLinkInfo))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断地下城ID是否是活动堕落深渊
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        public bool _judgeDungeonIDIsRotteneterHell(int dungeonId)
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (mDungeonTable == null)
            {
                return false;
            }
            if (mDungeonTable.Type == DungeonTable.eType.L_ACTIVITY)
            {
                if (mDungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL
                    ||mDungeonTable.SubType==DungeonTable.eSubType.S_ANNIVERSARY_HARD
                    ||mDungeonTable.SubType==DungeonTable.eSubType.S_ANNIVERSARY_NORMAL
                    ||mDungeonTable.SubType==DungeonTable.eSubType.S_TREASUREMAP
                    ||mDungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
