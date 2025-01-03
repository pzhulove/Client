using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using System;

namespace GameClient
{
    public class BossActivityDataManager
    {
        public List<int> BossExchangeID = new List<int>();//兑换活动对应的活动id
        public Dictionary<int, ActivityInfo> ActivityDic = new Dictionary<int, ActivityInfo>();//id - 活动数据
        public Dictionary<int, string> ChildActivityCount = new Dictionary<int, string>();
        public Dictionary<int, int> ChildActivityStatus = new Dictionary<int, int>();
        
        public List<KillBossData> killBossDataList = new List<KillBossData>();//前往击杀boss活动的数据
        public bool HaveBossActivity = false;
        public bool CanUpdateTime = false;

        public bool BossExchangeIsOpen = false;
        public bool BossKillIsOpen = false;

        public int ExchangeActivityID = -1;
        public int KillBossActivityID = -1;

        public string BossActivityBtIconPath = "";
        public string BossActivityBtIconName = "";

        public class KillBossData
        {
            public string mName;

            /// <summary>
            /// 只有12:00到24:00的时候为1。无论有没有打完boss
            /// </summary>
            public byte mActivity;
            public UInt32 mId;
            public UInt32 mRemainNum;
            public UInt32 mNextRollStartTime;
            public int mStartTime;
            public int mEndTime;
            public int mMonsterType;
            public List<DropItem> mDrops = new List<DropItem>();
        }
        public void Initialize()
        {
            Clear();
            _BindNetMsg();
            //initTaskIDList();
        }

        public void Clear()
        {
            _UnBindNetMsg();
            if(ActivityDic != null)
            {
                ActivityDic.Clear();
            }
            
            if(BossExchangeID != null)
            {
                BossExchangeID.Clear();
            }
            
            if(ChildActivityStatus != null)
            {
                ChildActivityStatus.Clear();
            }
            
            if(ChildActivityCount != null)
            {
                ChildActivityCount.Clear();
            }
            
            CanUpdateTime = false;

            if(killBossDataList != null)
            {
                killBossDataList.Clear();
            }
            

            BossExchangeIsOpen = false;
            BossKillIsOpen = false;
            BossActivityBtIconPath = "";
            BossActivityBtIconName = "";
        }

        public bool IsHasTaskFinished()
        {
            foreach (var status in this.ChildActivityStatus.Values)
            {
                if (status == (byte)Protocol.TaskStatus.TASK_FINISHED)
                {
                    return true;
                }
            }

            return false;
        }

        void initTaskIDList()
        {
            BossExchangeID.Clear();
            var activeTableAllData = TableManager.GetInstance().GetTable<ActiveTable>();
            var enumerator = activeTableAllData.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var activeTableData = enumerator.Current.Value as ActiveTable;
                if (activeTableData.TemplateID == ExchangeActivityID)
                {
                    BossExchangeID.Add(activeTableData.ID);
                }
            }
        }

        void _BindNetMsg()
        {
            NetProcess.AddMsgHandler(SceneSyncClientActivities.MsgID, OnRecvWorldSyncClientActivitiesNormal);
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskVar.MsgID, OnRecvSceneNotifyActiveTaskVar);
            NetProcess.AddMsgHandler(WorldActivityMonsterRes.MsgID, OnRecvActivityMonsterInfo);
            NetProcess.AddMsgHandler(WorldNotifyClientActivity.MsgID, OnRecvWorldNotifyClientActivity);
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(SceneSyncClientActivities.MsgID, OnRecvWorldSyncClientActivitiesNormal);
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskVar.MsgID, OnRecvSceneNotifyActiveTaskVar);
            NetProcess.RemoveMsgHandler(WorldActivityMonsterRes.MsgID, OnRecvActivityMonsterInfo);
            NetProcess.RemoveMsgHandler(WorldNotifyClientActivity.MsgID, OnRecvWorldNotifyClientActivity);
        }

        public string GetActivityTime(int id)
        {
            if (ActivityDic == null || !this.ActivityDic.ContainsKey(id))
            {
                return "";
            }

            return  GetDate((int)ActivityDic[id].startTime, (int)ActivityDic[id].dueTime);
        }

        

        private bool TimeEffect(int time)
        {
            if (TimeSpanToDateTime(time).ToString().Split(' ')[0].Split('/').Length > 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetDate(int startTime,int endTime)
        {
            System.DateTime getTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime startdt = getTime.AddSeconds(startTime);
            DateTime enddt = getTime.AddSeconds(endTime);
            return startdt.ToString("yyyy年MM月dd日") + "到" + enddt.ToString("yyyy年MM月dd日");
        }
        public string GetDateTime(int startTime, int endTime)
        {
            System.DateTime getTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime startdt = getTime.AddSeconds(startTime);
            DateTime enddt = getTime.AddSeconds(endTime);
            return startdt.ToString("yyyy年MM月dd日HH:mm") + "到" + enddt.ToString("yyyy年MM月dd日HH:mm");
        }

        public string GetTime(int startTime,int endTime)
        {
            System.DateTime getTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime startdt = getTime.AddSeconds(startTime);
            DateTime enddt = getTime.AddSeconds(endTime);
            return startdt.ToString("HH:mm") + "-" + enddt.ToString("HH:mm");
        }
        
        public DateTime TimeSpanToDateTime(long span)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            time = startTime.AddSeconds(span);
            return time;
        }

        /// <summary>
        /// 请求前往击杀boss页签下的数据
        /// </summary>
        public void SendBossKillData()
        {
            WorldActivityMonsterReq req = new WorldActivityMonsterReq();

            if(KillBossActivityID == 0)
            {
                return;
            }
            var activeMainTableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>(KillBossActivityID);
            if(activeMainTableData == null)
            {
                return;
            }
            string[] BossId = activeMainTableData.BossId.Split(',');
            uint[] BossActivityId = new uint[BossId.Length];
            for (int i = 0; i < BossId.Length; i++)
            {
                uint result = 0;
                uint.TryParse(BossId[i], out result);
                BossActivityId[i] = result;
            }
            req.ids = BossActivityId;
            killBossDataList.Clear();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

        }

        //登录时同步活动状态，筛选出两个需要的活动
        //[MessageHandle(SceneSyncClientActivities.MsgID)]
        void OnRecvWorldSyncClientActivitiesNormal(MsgDATA data)
        {
            SceneSyncClientActivities activities = new SceneSyncClientActivities();
            activities.decode(data.bytes);

            Logger.LogProcessFormat("OnRecvWorldSyncClientActivities {0}", ObjectDumper.Dump(activities));
            for (int i = 0; i < activities.activities.Length; ++i)
            {
                var activeMainTableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)activities.activities[i].id);
                if(activeMainTableData == null)
                {
                    continue;
                }
                if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.None)
                {
                    continue;
                }
                if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.ExchangeActivity)
                {
                    if(activeMainTableData.TownBtText != "" && activeMainTableData.TownBtText != null)
                    {
                        BossActivityBtIconName = activeMainTableData.TownBtText;
                    }
                    if(activeMainTableData.TownBtIconPath != "" && activeMainTableData.TownBtIconPath != null)
                    {
                        BossActivityBtIconPath = activeMainTableData.TownBtIconPath;
                    }

                    BossExchangeIsOpen = true;
                    ExchangeActivityID = (int)activities.activities[i].id;
                    if(ActivityDic.ContainsKey(ExchangeActivityID))
                    {
                        ActivityDic[ExchangeActivityID] = activities.activities[i];
                    }
                    else
                    {
                        ActivityDic.Add(ExchangeActivityID, activities.activities[i]);
                    }

                }

                if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.KillBossActivity)
                {
                    if (activeMainTableData.TownBtText != "" && activeMainTableData.TownBtText != null)
                    {
                        BossActivityBtIconName = activeMainTableData.TownBtText;
                    }
                    if (activeMainTableData.TownBtIconPath != "" && activeMainTableData.TownBtIconPath != null)
                    {
                        BossActivityBtIconPath = activeMainTableData.TownBtIconPath;
                    }

                    BossKillIsOpen = true;
                    KillBossActivityID = (int)activities.activities[i].id;
                    ActivityDic[KillBossActivityID] = activities.activities[i];
                    //ActivityDic.Add(KillBossActivityID, activities.activities[i]);
                }

                initTaskIDList();
            }
        }

        //任务状态更新的时候刷新
        //[MessageHandle(SceneNotifyActiveTaskStatus.MsgID)]
        void OnRecvSceneNotifyActiveTaskStatus(MsgDATA data)
        {
            SceneNotifyActiveTaskStatus kRecv = new SceneNotifyActiveTaskStatus();
            kRecv.decode(data.bytes);
            for (int i = 0; i < BossExchangeID.Count; i++)
            {
                if (kRecv.taskId == BossExchangeID[i])
                {
                    ChildActivityStatus[(int)kRecv.taskId] = kRecv.status;
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BossExchangeUpdate);
        }

        //剩余兑换次数发生改变时候刷新
        //[MessageHandle(SceneNotifyActiveTaskVar.MsgID)]
        void OnRecvSceneNotifyActiveTaskVar(MsgDATA data)
        {
            SceneNotifyActiveTaskVar kRecv = new SceneNotifyActiveTaskVar();
            kRecv.decode(data.bytes);

            //for (int i = 0; i < BossExchangeID.Count; i++)
            //{

            //    if (kRecv.taskId == BossExchangeID[i])
            //    {
            //        var activeTableData = TableManager.GetInstance().GetTableItem<ActiveTable>(BossExchangeID[i]);
            //        if (activeTableData == null)
            //        {
            //            Logger.LogErrorFormat("can not get activeTable id is {0}", BossExchangeID[i]);
            //            continue;
            //        }
            //        if (activeTableData.TaskCycleKey == kRecv.key)
            //        {
            //            ChildActivityCount[BossExchangeID[i]] = kRecv.val;

            //        }
            //    }
            //}
            ChildActivityCount[(int)kRecv.taskId] = kRecv.val;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BossExchangeUpdate);
        }

        //向服务器请求击杀boss的相关活动
        //[MessageHandle(WorldActivityMonsterRes.MsgID)]
        void OnRecvActivityMonsterInfo(MsgDATA data)
        {
            WorldActivityMonsterRes kRecv = new WorldActivityMonsterRes();
            kRecv.decode(data.bytes);
            this.killBossDataList.Clear();
            for (int i = 0; i < kRecv.monsters.Length; i++)
            {
                KillBossData TempkillBossData = new KillBossData();
                TempkillBossData.mName = kRecv.monsters[i].name;
                TempkillBossData.mActivity = kRecv.monsters[i].activity;
                TempkillBossData.mId = kRecv.monsters[i].id;
                TempkillBossData.mRemainNum = kRecv.monsters[i].remainNum;
                TempkillBossData.mNextRollStartTime = kRecv.monsters[i].nextRollStartTime;
                TempkillBossData.mStartTime = (int)kRecv.monsters[i].startTime;
                TempkillBossData.mEndTime = (int)kRecv.monsters[i].endTime;
                TempkillBossData.mMonsterType = (int)kRecv.monsters[i].pointType;
                for (int j = 0; j < kRecv.monsters[i].drops.Length; j++)
                {
                    TempkillBossData.mDrops.Add(kRecv.monsters[i].drops[j]);
                }
                killBossDataList.Add(TempkillBossData);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BossKillActivityExist);
            CanUpdateTime = true;
        }

        //服务器返回boss击杀活动的协议
        //[MessageHandle(WorldNotifyClientActivity.MsgID)]
        void OnRecvWorldNotifyClientActivity(MsgDATA data)
        {
            WorldNotifyClientActivity kRecv = new WorldNotifyClientActivity();
            kRecv.decode(data.bytes);
            var activeMainTableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)kRecv.id);
            if(activeMainTableData == null)
            {
                return;
            }
            if(activeMainTableData.ActivityType == ActiveMainTable.eActivityType.ExchangeActivity)
            {
                ActivityInfo activityInfo = new ActivityInfo();
                activityInfo.state = kRecv.type;
                activityInfo.id = kRecv.id;
                activityInfo.level = kRecv.level;
                activityInfo.name = kRecv.name;
                activityInfo.preTime = kRecv.preTime;
                activityInfo.startTime = kRecv.startTime;
                activityInfo.dueTime = kRecv.dueTime;
                if (kRecv.type == 0)
                {
                    BossExchangeIsOpen = false;
                    if(ActivityDic.ContainsKey((int)kRecv.id))
                    {
                        ActivityDic.Remove((int)kRecv.id);
                    }
                }
                else
                {
                    BossExchangeIsOpen = true;
                    if (activeMainTableData.TownBtText != "" && activeMainTableData.TownBtText != null)
                    {
                        BossActivityBtIconName = activeMainTableData.TownBtText;
                    }
                    if (activeMainTableData.TownBtIconPath != "" && activeMainTableData.TownBtIconPath != null)
                    {
                        BossActivityBtIconPath = activeMainTableData.TownBtIconPath;
                    }
                    
                    ExchangeActivityID = (int)activityInfo.id;
                    initTaskIDList();
                    ActivityDic.Add((int)kRecv.id, activityInfo);
                }
            }

            if(activeMainTableData.ActivityType == ActiveMainTable.eActivityType.KillBossActivity)
            {
                ActivityInfo activityInfo = new ActivityInfo();
                activityInfo.state = kRecv.type;
                activityInfo.id = kRecv.id;
                activityInfo.level = kRecv.level;
                activityInfo.name = kRecv.name;
                activityInfo.preTime = kRecv.preTime;
                activityInfo.startTime = kRecv.startTime;
                activityInfo.dueTime = kRecv.dueTime;
                if (kRecv.type == 0)
                {
                    BossKillIsOpen = false;
                    if (ActivityDic.ContainsKey((int)kRecv.id))
                    {
                        ActivityDic.Remove((int)kRecv.id);
                    }
                }
                else
                {
                    BossKillIsOpen = true;
                    if (activeMainTableData.TownBtText != "" && activeMainTableData.TownBtText != null)
                    {
                        BossActivityBtIconName = activeMainTableData.TownBtText;
                    }
                    if (activeMainTableData.TownBtIconPath != "" && activeMainTableData.TownBtIconPath != null)
                    {
                        BossActivityBtIconPath = activeMainTableData.TownBtIconPath;
                    }
                    KillBossActivityID = (int)activityInfo.id;
                    ActivityDic.Add((int)kRecv.id, activityInfo);
                }
            }
            if(BossExchangeIsOpen == true || BossKillIsOpen == true)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateBossActivityState, 1);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateBossActivityState, 0);
            }
        }
    }
}