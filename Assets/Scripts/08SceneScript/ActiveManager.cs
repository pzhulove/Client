using UnityEngine;
using System.Collections.Generic;
using Protocol;
using Network;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Reflection;
///////删除linq
using ProtoTable;

namespace GameClient
{
    class ActiveParams
    {
        public ulong param0;
        public ProtoTable.ActivityDungeonTable.eActivityType type = ProtoTable.ActivityDungeonTable.eActivityType.Daily;
        public int dungeonId;
    }

    public delegate void OnLinkOk();
    public delegate void OnLinkFailed();

    public class ActiveManager : DataManager<ActiveManager>
    {
        #region delegate
        public enum ActivityUpdateType
        {
            AUT_CREATE = 0,
            AUT_KEY_CHANGED,
            AUT_STATUS_CHANGED,
            AUT_RED_CHANGED,
        }
        public delegate void OnActivityUpdate(ActivityData data, ActivityUpdateType EActivityUpdateType);
        public OnActivityUpdate onActivityUpdate;
        public delegate void OnAddMainActivity(ActiveData data);
        public OnAddMainActivity onAddMainActivity;
        public delegate void OnUpdateMainActivity(ActiveData data);
        public OnUpdateMainActivity onUpdateMainActivity;
        public delegate void OnRemoveMainActivity(ActiveData data);
        public OnRemoveMainActivity onRemoveMainActivity;
        #endregion

        private bool bInited = false;

        /// <summary>
        /// 用来判断选中的页签活动ID是这两个活动ID
        /// </summary>
        public static int[] activityId = new int[] { 8100, 8200 };

        private bool welfareTABEnergyRedPointFlag = false;
        /// <summary>
        /// 精力找回页签红点标志位
        /// </summary>
        public bool WelfareTABEnergyRedPointFlag
        {
            get { return welfareTABEnergyRedPointFlag; }
            set { welfareTABEnergyRedPointFlag = value; }
        }

        private bool welfareTABRewardRedPointFlag = false;
        /// <summary>
        /// 奖励找回页签红点标志位
        /// </summary>
        public bool WelfareTABRewardRedPointFlag
        {
            get { return welfareTABRewardRedPointFlag; }
            set
            {
                welfareTABRewardRedPointFlag = value;
            }
        }

        public bool IsNotifyNormalGetBack { get; set; }//奖励找回 50%找回时是否提示

        public class VarBinder
        {
            public static Regex ms_condition_match = new Regex(@"<varName=(\w+) default=(\d+) do=<got=(\d+) ct=(\d+) cpv=(\w+) op=(\d+)>>", RegexOptions.Singleline);
            public string analyString = "<varName=nice default=100 do=<got=1 ct=1 cpv=10 op=0>>";

            public enum CompareType
            {
                CT_GREAT = 0,
                CT_LESS,
                CT_EQUAL,
                CT_GREAT_EQUAL,
                CT_LESS_EQUAL,
            }


            public enum GameObjectType
            {
                GOT_TRANSFORM = 0,
                GOT_TEXT,
                GOT_IMAGE,
                GOT_BUTTON,
            }

            public enum OpType
            {
                OT_SHOW = 0,
                OT_GRAY,
                OT_ENABLE,
                OT_TEXT,
                OT_IMAGE,
                OT_COLOR,
            }

            public string m_kKey;
            public int m_iDefaultValue;
            public GameObjectType m_eGameObjectType;
            public CompareType m_eCompareType;
            public int m_iCompareValue;
            public OpType m_eOpType;
            bool m_bResult = false;
            bool m_bAnalysis = false;
            public bool AnalysisOK()
            {
                return m_bResult;
            }

            public bool Analysis()
            {
                if(m_bAnalysis)
                {
                    return m_bResult;
                }
                m_bAnalysis = true;
                m_bResult = false;
                try
                {
                    foreach (Match match in ms_condition_match.Matches(analyString))
                    {
                        if (match != null && !string.IsNullOrEmpty(match.Groups[0].Value))
                        {
                            m_kKey = match.Groups[1].Value;
                            m_iDefaultValue = int.Parse(match.Groups[2].Value);
                            m_eGameObjectType = (GameObjectType)int.Parse(match.Groups[3].Value);
                            m_eCompareType = (CompareType)int.Parse(match.Groups[4].Value);
                            m_iCompareValue = int.Parse(match.Groups[5].Value);
                            m_eOpType = (OpType)int.Parse(match.Groups[6].Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    return false;
                }
                m_bResult = true;
                return true;
            }
        }

        #region init

        public class ControlData
        {
            public enum ControlDataType
            {
                CDT_INVALID = -1,
                CDT_IMAGE,
                CDT_BUTTON,
                CDT_TEXT,
                CDT_GAMEOBJECT,
                CDT_COUNT,
            }

            static Regex ms_kStatusValue = new Regex(@"<Status=(\d+) Value=(.+)>", RegexOptions.Singleline);

            public ControlData()
            {
                valueDic = null;
                name = null;
                eType = ControlDataType.CDT_INVALID;
                shows = null;
                statusValues = null;
            }

            public ControlDataType Type
            {
                get { return eType; }
            }

            public string Name
            {
                get { return name; }
            }

            public bool NeedShow(int iStatus)
            {
                return shows != null && shows.Contains(iStatus);
            }

            Dictionary<int, string> valueDic;
            string name;
            ControlDataType eType;
            List<int> shows;
            public class StatusValue
            {
                public int iStatus;
                public string value;
            }
            List<StatusValue> statusValues;
            public StatusValue GetStatusValue(int iStatus)
            {
                if(statusValues != null)
                {
                    return statusValues.Find(x => { return x.iStatus == iStatus; });
                }
                return null;
            }

            public void Analysis(string name,string type,string show,string statusvalue)
            {
                this.name = name;
                eType = ControlDataType.CDT_INVALID;
                switch (type)
                {
                    case "Text":
                        {
                            eType = ControlDataType.CDT_TEXT;
                        }
                        break;
                    case "Button":
                        {
                            eType = ControlDataType.CDT_BUTTON;
                        }
                        break;
                    case "Image":
                        {
                            eType = ControlDataType.CDT_IMAGE;
                        }
                        break;
                    case "GameObject":
                        {
                            eType = ControlDataType.CDT_GAMEOBJECT;
                        }
                        break;
                }
                shows = new List<int>();
                var showValues = show.Split(new char[] { ',' });
                for(int i = 0; i < showValues.Length; ++i)
                {
                    int showV = 0;
                    if(!string.IsNullOrEmpty(showValues[i]) && int.TryParse(showValues[i],out showV))
                    {
                        shows.Add(showV);
                    }
                }

                statusValues = new List<StatusValue>();
                if (!string.IsNullOrEmpty(statusvalue))
                {
                    var targets = statusvalue.Split('|');
                    for(int i = 0; i < targets.Length; ++i)
                    {
                        if(!string.IsNullOrEmpty(targets[i]))
                        {
                            Match match = ms_kStatusValue.Match(targets[i]);
                            int iStatus = 0;
                            if(!string.IsNullOrEmpty(match.Groups[0].Value) && int.TryParse(match.Groups[1].Value,out iStatus))
                            {
                                StatusValue sv = new StatusValue();
                                sv.iStatus = iStatus;
                                sv.value = match.Groups[2].Value;
                                statusValues.Add(sv);
                            }
                        }
                    }
                }
            }
        }

        public class ActivityPrefab
        {
            public string parent;
            public string local;
            public string key;
        }

        public class ActivityData
        {
            public ActivityData()
            {
                ID = 0;
                activeItem = null;
                akActivityValues = new List<TaskPair>();
                akActivityValues.Clear();
                status = 0;
            }
            public int ID;
            public ProtoTable.ActiveTable activeItem;
            public List<TaskPair> akActivityValues;
            public byte status;


			public Dictionary<uint, int> GetAwards()
			{
				Dictionary<uint, int> finalAwards = new Dictionary<uint, int>();

				if (activeItem.Awards.Length > 1)
				{
					var awards = activeItem.Awards.Split(new char[] { ',' });
					for (int i = 0; i < awards.Length; ++i)
					{
						if (!string.IsNullOrEmpty(awards[i]))
						{
							var substrings = awards[i].Split(new char[] { '_' });
							if (substrings.Length == 2)
							{
								int id = int.Parse(substrings[0]);
								int iCount = int.Parse(substrings[1]);

								finalAwards.Add((uint)id, iCount);
							}
						}
					}
				}
					
				return finalAwards;
			}
        }
        public class ActiveData
        {
            public ActiveData()
            {
                iActiveID = 0;
                mainItem = null;
                iActiveSortID = 0;
                akChildItems = new List<ActivityData>();
                akChildItems.Clear();
                mainInfo = null;
                mainKeyValue = new List<ActiveMainKeyValue>();
                mainKeyValue.Clear();
                updateMainKeys = new List<ActiveMainUpdateKey>();
                updateMainKeys.Clear();
                values = null;
                prefabs = null;
            }
            public int iActiveID;
            public ProtoTable.ActiveMainTable mainItem;
            public int iActiveSortID;
            public List<ActivityData> akChildItems;
            public ActivityInfo mainInfo;
            public List<ActiveMainKeyValue> mainKeyValue;
            public List<ActiveMainUpdateKey> updateMainKeys;

            public Dictionary<string, List<ControlData>> values;
            public Dictionary<string, ActivityPrefab> prefabs;
        }

        Dictionary<int, ActiveData> m_akActiveDictionary = new Dictionary<int, ActiveData>();
        public Dictionary<int, ActiveData> ActiveDictionary
        {
            get
            {
                return m_akActiveDictionary;
            }
        }

        //临时屏蔽月卡页签
        
            

        //int[] m_aiDiscardedTemplateIds = new int[] { 6000 };

        public Dictionary<int, List<ProtoTable.ActiveTable>> m_akTemplate2idList = new Dictionary<int, List<ProtoTable.ActiveTable>>();
        void _LoadTemplate2IdLists()
        {
            if (m_akTemplate2idList.Count == 0)
            {
                var activeTables = TableManager.GetInstance().GetTable<ProtoTable.ActiveTable>();
                var enumerator = activeTables.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var value = enumerator.Current.Value as ProtoTable.ActiveTable;
                    
                    //if (m_aiDiscardedTemplateIds.Contains(value.TemplateID) && GameClient.FreeVersionDataManager.GetInstance().IsFreeVersion)
                    //{
                    //    continue;
                    //}

                    var activeMain = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveMainTable>(value.TemplateID);
                    if (activeMain != null)
                    {
                        List<ProtoTable.ActiveTable> outValue = null;
                        if(!m_akTemplate2idList.TryGetValue(value.TemplateID,out outValue))
                        {
                            outValue = new List<ProtoTable.ActiveTable>();
                            m_akTemplate2idList.Add(value.TemplateID, outValue);
                        }
                        outValue.Add(value);
                    }
                }
            }
        }

        Dictionary<int, List<ProtoTable.ActiveMainTable>> m_akType2Templates = new Dictionary<int, List<ProtoTable.ActiveMainTable>>();
        public List<ProtoTable.ActiveMainTable> GetType2Templates(int iConfigID)
        {
            if(m_akType2Templates.ContainsKey(iConfigID))
            {
                return m_akType2Templates[iConfigID];
            }
            return null;
        }
        void _LoadActiveType2Templates()
        {
            m_akType2Templates.Clear();
            var activeMainTables = TableManager.GetInstance().GetTable<ProtoTable.ActiveMainTable>();
            var enumerator = activeMainTables.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var value = enumerator.Current.Value as ProtoTable.ActiveMainTable;
                if(value != null)
                {
                    List<ProtoTable.ActiveMainTable> outValue = null;
                    if (!m_akType2Templates.TryGetValue(value.ActiveTypeID, out outValue))
                    {
                        outValue = new List<ProtoTable.ActiveMainTable>();
                        m_akType2Templates.Add(value.ActiveTypeID, outValue);
                    }
                    outValue.Add(value);
                }
            }
        }

        void _LoadMainKeyValues()
        {
            List<CounterInfo> arrCountInfos = CountDataManager.GetInstance().GetCountInfos();
            if (arrCountInfos != null)
            {
                for(int i = 0; i < arrCountInfos.Count; ++i)
                {
                    OnCountChanged(arrCountInfos[i]);
                }
            }
        }

        public void RemoveOneActiveData(int activityTemplateId)
        {
            if (m_akActiveDictionary == null || m_akActiveDictionary.Count <= 0)
                return;

            m_akActiveDictionary.Remove(activityTemplateId);
        }

        public void AddOneActiveData(int activityTemplateId)
        {
            if (m_akActiveDictionary == null)
                return;

            //已经包含则不添加
            if (m_akActiveDictionary.ContainsKey(activityTemplateId) == true)
                return;

            var activeMainTable = TableManager.GetInstance().GetTableItem<ActiveMainTable>(activityTemplateId);
            if (activeMainTable != null)
            {
                ActiveData activeData = new ActiveData();
                activeData.iActiveID = activityTemplateId;
                activeData.mainItem = activeMainTable;
                activeData.iActiveSortID = activeMainTable.SortID;

                activeData.mainInfo = new ActivityInfo();
                activeData.mainInfo.state = 0;
                activeData.akChildItems.Clear();
                m_akActiveDictionary.Add(activityTemplateId, activeData);
            }
        }

        void _LoadTableActiveData()
        {
            if(m_akActiveDictionary.Count == 0)
            {
                var activeTables = TableManager.GetInstance().GetTable<ProtoTable.ActiveTable>();
                var enumerator = activeTables.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var value = enumerator.Current.Value as ProtoTable.ActiveTable;
                    if(value.TemplateID / 1000 != 7)
                    {
                        continue;
                    }
                    var activeMain = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveMainTable>(value.TemplateID);
                    if (activeMain != null)
                    {
                        ActiveData outValue = null;
                        if (!m_akActiveDictionary.TryGetValue(value.TemplateID,out outValue))
                        {
                            outValue = new ActiveData();
                            outValue.iActiveID = value.TemplateID;
                            outValue.mainItem = activeMain;
                            outValue.iActiveSortID = activeMain.SortID;
                            outValue.mainInfo = new ActivityInfo();
                            outValue.mainInfo.state = 0;
                            outValue.akChildItems.Clear();
                            m_akActiveDictionary.Add(value.TemplateID, outValue);
                        }

                        _LoadActivityDataFromTable(outValue, value.TemplateID);
                        _LoadKey2Prefab(outValue, value.TemplateID);
                        _LoadKey2Content(outValue, value.TemplateID);
                    }
                    else
                    {
                        Logger.LogErrorFormat("ProtoTable.ActiveTable ID = {0} It's TemplateID = {1} which mapped ActiveMainTable is wrong !", value.ID, value.TemplateID);
                    }
                }
            }
        }
        #endregion

        #region process
        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.ActiveManager;
        }

        public override void Initialize()
        {
            RegisterNetHandler();

            _LoadTemplate2IdLists();
            _LoadActiveType2Templates();
            _LoadTableActiveData();
            _LoadMainKeyValues();
            IsNotifyNormalGetBack = true;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        public override void OnEnterSystem()
        {
            m_akActiveFrameConfigs.Clear();
        }

        public override void OnExitSystem()
        {
            m_akActiveFrameConfigs.Clear();
        }

        public override void Clear()
        {
            UnRegisterNetHandler();

            welfareTABRewardRedPointFlag = false;
            welfareTABEnergyRedPointFlag = false;
            bInited = false;
            m_akActivitiesDic.Clear();
            m_akActiveFrameConfigs.Clear();
            m_akActiveDictionary.Clear();
            m_akRedPointMap.Clear();
            IsNotifyNormalGetBack = true;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskVar.MsgID, OnRecvSceneNotifyActiveTaskVar);
            NetProcess.AddMsgHandler(SceneSyncActiveTaskList.MsgID, OnRecvSceneSyncActiveTaskListNormal);
            NetProcess.AddMsgHandler(WorldNotifyClientActivity.MsgID, OnRecvWorldNotifyClientActivity);
            NetProcess.AddMsgHandler(SceneSyncClientActivities.MsgID, OnRecvWorldSyncClientActivitiesNormal);
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);
            NetProcess.AddMsgHandler(SceneActiveRestTimeRet.MsgID, OnRecvSceneActiveRestTimeRet);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskVar.MsgID, OnRecvSceneNotifyActiveTaskVar);
            NetProcess.RemoveMsgHandler(SceneSyncActiveTaskList.MsgID, OnRecvSceneSyncActiveTaskListNormal);
            NetProcess.RemoveMsgHandler(WorldNotifyClientActivity.MsgID, OnRecvWorldNotifyClientActivity);
            NetProcess.RemoveMsgHandler(SceneSyncClientActivities.MsgID, OnRecvWorldSyncClientActivitiesNormal);
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);
            NetProcess.RemoveMsgHandler(SceneActiveRestTimeRet.MsgID, OnRecvSceneActiveRestTimeRet);
        }

        void _OnCountValueChanged(UIEvent a_event)
        {
            CounterInfo info = CountDataManager.GetInstance().GetCountInfo(a_event.Param1 as string);
            if (info != null)
            {
                OnCountChanged(info);
            }
        }

        void OnCountChanged(CounterInfo info)
        {
            if(!string.IsNullOrEmpty(info.name))
            {
                var keyValues = info.name.Split('_');
                if(keyValues.Length == 2)
                {
                    int iKey = 0;
                    if(int.TryParse(keyValues[0],out iKey) && !string.IsNullOrEmpty(keyValues[1]))
                    {
                        if(ActiveDictionary.ContainsKey(iKey))
                        {
                            var activeData = ActiveDictionary[iKey];
                            if(activeData != null)
                            {
                                var find = activeData.mainKeyValue.Find(x => { return x.key == keyValues[1]; });
                                if(find == null)
                                {
                                    find = new ActiveMainKeyValue();
                                    find.key = keyValues[1];
                                    activeData.mainKeyValue.Add(find);
                                }
                                find.value = info.value.ToString();

                                var findUpdateKey = activeData.updateMainKeys.Find(x => { return x.key == keyValues[1]; });
                                if(findUpdateKey != null)
                                {
                                    findUpdateKey.fRecievedTime = TimeManager.GetInstance().GetServerTime();
                                    findUpdateKey.value = info.value.ToString();
                                }

                                if (onUpdateMainActivity != null)
                                {
                                    onUpdateMainActivity(activeData);
                                    Logger.LogProcessFormat("更新主活动{0}[{1}][key={2},Value={3}]",
                                        activeData.mainItem.Name, 
                                        activeData.mainItem.ID,
                                        find.key,
                                        find.value);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region NetMsg
        public class ActiveMainKeyValue
        {
            public string key;
            public string value;
        }

        public class ActiveMainUpdateKey
        {
            public static Regex s_regex = new Regex(@"<key=(\w+) default=(\d+) value=(.+)>", RegexOptions.Singleline);
            public string key;
            public int iDefValue;
            public string content;
            public string value = "0";
            public double fRecievedTime;//收到消息的时间
        }

        //[MessageHandle(SceneNotifyActiveTaskVar.MsgID)]
        void OnRecvSceneNotifyActiveTaskVar(MsgDATA data)
        {
            SceneNotifyActiveTaskVar kRecv = new SceneNotifyActiveTaskVar();
            kRecv.decode(data.bytes);

            Logger.LogProcessFormat("OnRecvSceneNotifyActiveTaskVar!");

            if (_IsSevenDaysActiveByActiveId(kRecv.taskId))      //如果是七日活动就在sevendaysmanager中处理
            {
                SevendaysDataManager.GetInstance().UpdateSevenDaysSceneNotifyActiveTaskVar(kRecv);
                return;
            }

            int iKey = (int)kRecv.taskId;

            ProtoTable.ActiveTable activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(iKey);
            if(activeItem != null)
            {
                ActiveData outValue = null;
                if(m_akActiveDictionary.TryGetValue(activeItem.TemplateID,out outValue))
                {
                    var find = outValue.akChildItems.Find(x => { return x.ID == iKey; });
                    if(find != null)
                    {
                        var keyValue = find.akActivityValues.Find(x => { return x.key == kRecv.key; });
                        if(keyValue != null)
                        {
                            keyValue.value = kRecv.val;
                        }
                        else
                        {
                            TaskPair pair = new TaskPair();
                            pair.key = kRecv.key;
                            pair.value = kRecv.val;
                            find.akActivityValues.Add(pair);
                        }

                        if(onActivityUpdate != null)
                        {
                            onActivityUpdate(find, ActivityUpdateType.AUT_KEY_CHANGED);
                        }

                        if(find.activeItem.ReplaceID.Count > 0)
                        {
                            for(int i = 0; i < find.activeItem.ReplaceID.Count; ++i)
                            {
                                var curReplaceID = find.activeItem.ReplaceID[i];
                                var findReplace = outValue.akChildItems.Find(x => { return curReplaceID == x.ID; });
                                if (findReplace != null)
                                {
                                    var replaceKeyValue = findReplace.akActivityValues.Find(x => { return x.key == kRecv.key; });
                                    if (replaceKeyValue != null)
                                    {
                                        replaceKeyValue.value = kRecv.val;
                                    }
                                    else
                                    {
                                        TaskPair pair = new TaskPair();
                                        pair.key = kRecv.key;
                                        pair.value = kRecv.val;
                                        findReplace.akActivityValues.Add(pair);
                                    }

                                    if (onActivityUpdate != null)
                                    {
                                        onActivityUpdate(findReplace, ActivityUpdateType.AUT_KEY_CHANGED);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnBindEnterGameMsg()
        {
            EnterGameBinding eb = new EnterGameBinding();
            eb.id = SceneSyncActiveTaskList.MsgID;

            try
            {
                eb.method = OnRecvSceneSyncActiveTaskList;
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("错误!! 绑定消息{0}(ID:{1})到方法", ProtocolHelper.instance.GetName(eb.id), eb.id);
            }

            m_arrEnterGameBindings.Add(eb);
        }


        //[EnterGameMessageHandleAttribute(SceneSyncActiveTaskList.MsgID)]
        //[MessageHandle(SceneSyncActiveTaskList.MsgID)]
        void OnRecvSceneSyncActiveTaskList(MsgDATA data)
        {
            bInited = true;
            OnRecvSceneSyncActiveTaskListNormal(data);
        }

        //[EnterGameMessageHandleAttribute(SceneSyncActiveTaskList.MsgID,2)]
        //[MessageHandle(SceneSyncActiveTaskList.MsgID)]
        void OnRecvSceneSyncActiveTaskListNormal(MsgDATA data)
        {
            if(bInited == false)
            {
                return;
            }

            SceneSyncActiveTaskList kRect = new SceneSyncActiveTaskList();
            kRect.decode(data.bytes);

            SevendaysDataManager.GetInstance().UpdateRecvSceneSyncActiveTaskListNormal(kRect);  

            Logger.LogProcessFormat("OnRecvSceneSyncActiveTaskList!");

            for (int i = 0; i < kRect.tasks.Length; ++i)
            {
                if (_IsSevenDaysActiveByActiveId(kRect.tasks[i].taskID))    //如果是七日活动就在sevendaysmanager中处理
                {
                    continue;
                }

                var current = kRect.tasks[i];
                ProtoTable.ActiveTable activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>((int)current.taskID);
                if (activeItem != null)
                {
                    ActiveData outValue = null;
                    if (m_akActiveDictionary.TryGetValue(activeItem.TemplateID, out outValue))
                    {
                        var find = outValue.akChildItems.Find(x => { return x.ID == (int)current.taskID; });
                        if (find != null)
                        {
                            find.status = current.status;
                            find.akActivityValues.Clear();
                            for(int j = 0; j < current.akMissionPairs.Length; ++j)
                            {
                                TaskPair pair = new TaskPair();
                                pair.key = current.akMissionPairs[j].key;
                                pair.value = current.akMissionPairs[j].value;
                                find.akActivityValues.Add(pair);
                            }

                            if (onActivityUpdate != null)
                            {
                                onActivityUpdate(find, ActivityUpdateType.AUT_CREATE);
                            }

                            if(find.activeItem.ReplaceID.Count > 0)
                            {
                                for(int k = 0; k < find.activeItem.ReplaceID.Count; ++k)
                                {
                                    var curReplaceID = find.activeItem.ReplaceID[k];
                                    var findReplace = outValue.akChildItems.Find(x => { return x.ID == curReplaceID; });
                                    if (findReplace != null)
                                    {
                                        findReplace.status = current.status;
                                        findReplace.akActivityValues.Clear();
                                        for (int j = 0; j < current.akMissionPairs.Length; ++j)
                                        {
                                            TaskPair pair = new TaskPair();
                                            pair.key = current.akMissionPairs[j].key;
                                            pair.value = current.akMissionPairs[j].value;
                                            findReplace.akActivityValues.Add(pair);
                                        }

                                        if (onActivityUpdate != null)
                                        {
                                            onActivityUpdate(findReplace, ActivityUpdateType.AUT_CREATE);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPayResultNotify, "10");
        }

        //[MessageHandle(WorldNotifyClientActivity.MsgID)]
        void OnRecvWorldNotifyClientActivity(MsgDATA data)
        {
            WorldNotifyClientActivity kRecv = new WorldNotifyClientActivity();
            kRecv.decode(data.bytes);

            if (_IsSevenDaysActiveByActiveMainId(kRecv.id)) //如果是七日活动就在sevendaysmanager中处理
            {
                SevendaysDataManager.GetInstance().UpdateRecvSevendaysNotifyClientActivity(kRecv);
                return;
            }

			Logger.LogProcessFormat("OnRecvWorldNotifyClientActivity {0}", ObjectDumper.Dump(kRecv));

            if(kRecv.type == 0)
            {
                int iID = (int)kRecv.id;
                
                if (m_akActiveDictionary.ContainsKey(iID))
                {
                    var activeData = m_akActiveDictionary[iID];
                    activeData.mainInfo.state = 0;

                    if (onRemoveMainActivity != null)
                    {
                        onRemoveMainActivity(activeData);
                    }
                    Logger.LogProcessFormat("主活动{0}[{1}]关闭!", activeData.mainItem.Name, activeData.mainItem.ID);
                }

                if(allActivities.ContainsKey(iID))
                {
                    var activeData = allActivities[iID];
                    activeData.state = 0;
                }
            }
            else if(kRecv.type == 1 || kRecv.type == 2)
            {
                int iID = (int)kRecv.id;
                if (!m_akActiveDictionary.ContainsKey(iID))
                {
                    ActivityInfo current = new ActivityInfo();
                    current.id = kRecv.id;
                    current.level = kRecv.level;
                    current.name = kRecv.name;
                    current.startTime = kRecv.startTime;
                    current.state = kRecv.type;
                    current.dueTime = kRecv.dueTime;

                    _OnAddActivety(current);
                }
                else
                {
                    var activeData = m_akActiveDictionary[iID];
                    ActivityInfo current = new ActivityInfo();
                    current.id = kRecv.id;
                    current.level = kRecv.level;
                    current.name = kRecv.name;
                    current.startTime = kRecv.startTime;
                    current.state = kRecv.type;
                    current.dueTime = kRecv.dueTime;
                    activeData.mainInfo = current;
                    if(onUpdateMainActivity != null)
                    {
                        onUpdateMainActivity(activeData);
                    }
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityUpdate, kRecv.id);
        }

        int iBudoActive = 0;
        public int BudoActive
        {
            get
            {
                return iBudoActive;
            }

            set
            {
                iBudoActive = value;
            }
        }

        public bool IsBudoActive(int iID)
        {
            var lower = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WUDAO_ACTIVITY_ID_BEGIN);
            var high = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WUDAO_ACTIVITY_ID_END);
            if(null == lower || null == high)
            {
                return false;
            }

            return lower.Value <= iID && iID <= high.Value;
        }

        private bool _IsSevenDaysActiveByActiveMainId(uint id)
        {
            ActiveMainTable activeMainTable = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)id);
            if (activeMainTable != null && SevendaysDataManager.activityTypeId == activeMainTable.ActiveTypeID)
            {
                return true;
            }

            return false;
        }

        private bool _IsSevenDaysActiveByActiveId(uint id)
        {
            SevenDaysActiveTable sevenDaysActiveTable = TableManager.GetInstance().GetTableItem<SevenDaysActiveTable>((int)id);
            return sevenDaysActiveTable != null;
        }

        void _OnAddActivety(ActivityInfo current)
        {
            if (current.id == 8700)  //每日礼包暂时屏蔽 by chenhangjie
            {
                return;
            }

            if (!m_akActivitiesDic.ContainsKey((int)current.id))
            {
                m_akActivitiesDic.Add((int)current.id, current);
            }
            else 
            {
                m_akActivitiesDic[(int)current.id] = current;
            }

            if(IsBudoActive((int)current.id))
            {
                BudoActive = (int)current.id;
            }

			Logger.LogProcessFormat("OnRecvWorldSyncClientActivities id = {0},name={1}, {2}->{3}", current.id, current.name, current.startTime, current.dueTime);

            int iTemplateID = (int)current.id;
            var activeMain = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveMainTable>(iTemplateID);
            if (activeMain != null)
            {
                if (m_akTemplate2idList.ContainsKey(iTemplateID))
                {
                    ActiveData activeData = null;

                    m_akActiveDictionary.Remove(iTemplateID);

                    if (!m_akActiveDictionary.TryGetValue(iTemplateID, out activeData))
                    {
                        activeData = new ActiveData();
                        activeData.iActiveID = iTemplateID;
                        activeData.mainItem = activeMain;
                        activeData.iActiveSortID = activeMain.SortID;
                        activeData.akChildItems.Clear();
                        activeData.mainInfo = current;
                        //mianKeyValue Initialize
                        {
                            activeData.mainKeyValue.Clear();

                            ActiveMainKeyValue mainKeyValue = new ActiveMainKeyValue();
                            mainKeyValue.key = "ActiveName";
                            mainKeyValue.value = activeMain.Name;
                            activeData.mainKeyValue.Add(mainKeyValue);

                            DateTime dateTime = Function.ConvertIntDateTime(activeData.mainInfo.startTime);
                            mainKeyValue = new ActiveMainKeyValue();
                            mainKeyValue.key = "ActiveTime";
                            mainKeyValue.value = dateTime.ToString(TR.Value("activity_normal_data_format"), System.Globalization.DateTimeFormatInfo.InvariantInfo);
                            mainKeyValue.value += "~";
                            dateTime = Function.ConvertIntDateTime(activeData.mainInfo.dueTime);
                            mainKeyValue.value += dateTime.ToString(TR.Value("activity_normal_data_format"), System.Globalization.DateTimeFormatInfo.InvariantInfo);
                            activeData.mainKeyValue.Add(mainKeyValue);

                            mainKeyValue = new ActiveMainKeyValue();
                            mainKeyValue.key = "ActiveDesc";
                            mainKeyValue.value = activeMain.PurDesc;
                            activeData.mainKeyValue.Add(mainKeyValue);
                        }
                        //updateMainKeyValue
                        {
                            activeData.updateMainKeys.Clear();
                            if(!string.IsNullOrEmpty(activeData.mainItem.UpdateMainKeys))
                            {
                                var mainKeys = activeData.mainItem.UpdateMainKeys.Split(new char[] { '\r', '\n' });
                                for(int i = 0; i < mainKeys.Length; ++i)
                                {
                                    if(!string.IsNullOrEmpty(mainKeys[i]))
                                    {
                                        Match match = ActiveMainUpdateKey.s_regex.Match(mainKeys[i]);
                                        if (!string.IsNullOrEmpty(match.Groups[0].Value))
                                        {
                                            ActiveMainUpdateKey updateKey = new ActiveMainUpdateKey();
                                            updateKey.key = match.Groups[1].Value;
                                            updateKey.iDefValue = int.Parse(match.Groups[2].Value);
                                            updateKey.content = match.Groups[3].Value;
                                            updateKey.fRecievedTime = TimeManager.GetInstance().GetServerTime();
                                            activeData.updateMainKeys.Add(updateKey);
                                            var countInfo = CountDataManager.GetInstance().GetCountInfo(current.id + "_" + updateKey.key);
                                            if(countInfo != null)
                                            {
                                                updateKey.iDefValue = (int)countInfo.value;
                                            }
                                            updateKey.value = updateKey.iDefValue.ToString();
                                        }
                                    }
                                }
                            }
                        }

                        m_akActiveDictionary.Add(iTemplateID, activeData);
                    }

                    _LoadKey2Prefab(activeData, iTemplateID);
                    _LoadActivityDataFromTable(activeData,iTemplateID);
                    _LoadKey2Content(activeData, iTemplateID);

                    if (onAddMainActivity != null)
                    {
                        onAddMainActivity(activeData);
                        Logger.LogProcessFormat("主活动{0}[{1}]开启!", activeData.mainItem.Name, activeData.mainItem.ID);
                    }
                }
            }
            else
            {
                //Logger.LogErrorFormat("ProtoTable.ActiveMainTable ID = {0} is wrong !", iTemplateID);
            }
        }

        void _LoadActivityDataFromTable(ActiveData activeData,int iTemplateID)
        {
            if(activeData != null && m_akTemplate2idList.ContainsKey(iTemplateID))
            {
                var activityids = m_akTemplate2idList[iTemplateID];
                for (int j = 0; j < activityids.Count; ++j)
                {
                    ActivityData activityData = new ActivityData();
                    activityData.ID = activityids[j].ID;
                    activityData.activeItem = activityids[j];

                    var find = activeData.akChildItems.Find(x => { return x.ID == activityids[j].ID; });
                    if (find == null)
                    {
                        activeData.akChildItems.Add(activityData);
                    }
                    else
                    {
                        Logger.LogWarningFormat("find repeated id = {0}", find.ID);
                    }
                }
                return;
            }

            Logger.LogErrorFormat("_LoadActivtyDataFromTable error occued !");
        }
        void _LoadKey2Prefab(ActiveData activeData,int iTemplateID)
        {
            var activeMain = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveMainTable>(iTemplateID);
            if(activeMain != null)
            {
                activeData.prefabs = null;
                Regex matchString = new Regex(@"<key=(\w+) parent=([A-Za-z0-9/]+) local=([A-Za-z0-9/]+)>", RegexOptions.Singleline);
                if (!string.IsNullOrEmpty(activeMain.prefabDesc))
                {
                    foreach (Match prefabMatch in matchString.Matches(activeMain.prefabDesc))
                    {
                        if (prefabMatch != null && !string.IsNullOrEmpty(prefabMatch.Groups[0].Value))
                        {
                            if (activeData.prefabs == null)
                            {
                                activeData.prefabs = new Dictionary<string, ActivityPrefab>();
                                activeData.prefabs.Clear();
                            }

                            var prefabObject = new ActivityPrefab();
                            prefabObject.key = prefabMatch.Groups[1].Value;
                            prefabObject.parent = prefabMatch.Groups[2].Value;
                            prefabObject.local = prefabMatch.Groups[3].Value;

                            if (!activeData.prefabs.ContainsKey(prefabObject.key))
                            {
                                activeData.prefabs.Add(prefabObject.key, prefabObject);
                            }
                        }
                    }

                }
            }
        }

        void _LoadKey2Content(ActiveData activeData,int iTemplateID)
        {
            var activeMain = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveMainTable>(iTemplateID);
            if(activeMain != null)
            {
                Regex kRegex = new Regex(@"<Key=(\w+) Name=([A-Za-z0-9/]+) Type=(\w+) Show=([0-9,]+) Value=(.*)>", RegexOptions.Singleline);
                var statuDescArray = activeMain.PrefabStatusDesc.Split(new char[] { '\r', '\n' });
                for (int i = 0; i < statuDescArray.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(statuDescArray[i]))
                    {
                        Match match = kRegex.Match(statuDescArray[i]);
                        if (!string.IsNullOrEmpty(match.Groups[0].Value))
                        {
                            if (activeData.values == null)
                            {
                                activeData.values = new Dictionary<string, List<ControlData>>();
                                activeData.values.Clear();
                            }

                            var key = match.Groups[1].Value;
                            var name = match.Groups[2].Value;
                            var type = match.Groups[3].Value;
                            var show = match.Groups[4].Value;
                            var value = match.Groups[5].Value;

                            List<ControlData> controlDatas = null;
                            if (!activeData.values.TryGetValue(key, out controlDatas))
                            {
                                controlDatas = new List<ControlData>();
                                activeData.values.Add(key, controlDatas);
                            }

                            ControlData controlData = new ControlData();
                            controlData.Analysis(name, type, show, value);
                            controlDatas.Add(controlData);
                        }
                    }
                }
            }
        }

       
        //[MessageHandle(SceneSyncClientActivities.MsgID)]
        void OnRecvWorldSyncClientActivitiesNormal(MsgDATA data)
        {
            SceneSyncClientActivities activities = new SceneSyncClientActivities();
            activities.decode(data.bytes);

            Logger.LogProcessFormat("OnRecvWorldSyncClientActivities {0}", ObjectDumper.Dump(activities));
            SevendaysDataManager.GetInstance().UpdateRecvSevenDaysSyncClientActivitiesNormal(activities);

            for (int i = 0; i < activities.activities.Length; ++i)
            {
                if (_IsSevenDaysActiveByActiveMainId(activities.activities[i].id))  //如果是七日活动就在sevendaysmanager中处理
                {
                    continue;
                }

                _OnAddActivety(activities.activities[i]);
                if (activities.activities[i] != null)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityUpdate, activities.activities[i].id);
                }
            }
        }

        //[MessageHandle(SceneNotifyActiveTaskStatus.MsgID)]
        void OnRecvSceneNotifyActiveTaskStatus(MsgDATA data)
        {
            SceneNotifyActiveTaskStatus kRecv = new SceneNotifyActiveTaskStatus();
            kRecv.decode(data.bytes);

            if (_IsSevenDaysActiveByActiveId(kRecv.taskId))    //如果是七日活动就在sevendaysmanager中处理
            {
                SevendaysDataManager.GetInstance().UpdateRecvSceneNotifyActiveTaskStatus(kRecv);
                return;
            }

            int key = (int)kRecv.taskId;

            var tableActive = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(key);
            if (tableActive != null)
            {
                if (m_akActiveDictionary.ContainsKey(tableActive.TemplateID))
                {
                    var current = m_akActiveDictionary[tableActive.TemplateID];
                    var curData = current.akChildItems.Find(x => { return x.ID == key; });
                    if(curData != null)
                    {
                        Logger.LogProcessFormat("活动{0}状态更新 [{1} --> {2}]", curData.activeItem.ID, (TaskStatus)curData.status, (TaskStatus)kRecv.status);

                        curData.status = kRecv.status;
                        if(onActivityUpdate != null)
                        {
                            onActivityUpdate(curData, ActivityUpdateType.AUT_STATUS_CHANGED);
                        }

                        if(curData.activeItem.ReplaceID.Count > 0)
                        {
                            for(int i = 0; i < curData.activeItem.ReplaceID.Count; ++i)
                            {
                                var curReplaceID = curData.activeItem.ReplaceID[i];
                                var replaceData = current.akChildItems.Find(x => { return x.ID == curReplaceID; });
                                if (replaceData != null)
                                {
                                    replaceData.status = kRecv.status;
                                    if (onActivityUpdate != null)
                                    {
                                        onActivityUpdate(replaceData, ActivityUpdateType.AUT_STATUS_CHANGED);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SendSevenDayTimeReq()
        {
            SceneActiveRestTimeReq kSend = new SceneActiveRestTimeReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
            Logger.LogProcessFormat("[activity]SendSevenDayTimeReq !!!");
        }

        public delegate void OnSevenDayTimeChanged(uint time1,uint time2, uint time3,double recvTime);
        public OnSevenDayTimeChanged onSevenDayTimeChanged;

        //[MessageHandle(SceneActiveRestTimeRet.MsgID)]
        void OnRecvSceneActiveRestTimeRet(MsgDATA msgData)
        {
            SceneActiveRestTimeRet kRet = new SceneActiveRestTimeRet();
            kRet.decode(msgData.bytes);
            Logger.LogProcessFormat("<color=#00ff00>[activity]OnRecvSceneActiveRestTimeRet time1={0} time2={1} time3={2}!!!</color>", kRet.time1,kRet.time2, kRet.time3);

            double serverTime = TimeManager.GetInstance().GetServerTime();
            if(onSevenDayTimeChanged != null)
            {
                onSevenDayTimeChanged.Invoke(kRet.time1, kRet.time2, kRet.time3,serverTime);
            }
        }

        public void SendSubmitActivity(int iChildID,uint iParam = 0)
        {
            SceneActiveTaskSubmit kSend = new SceneActiveTaskSubmit();
            kSend.taskId = (uint)iChildID;
            kSend.param1 = iParam;

            Logger.LogProcessFormat("[activity]SendSubmitActivity ID={0}", iChildID);

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }

        int _SortActivities(ActivityData left, ActivityData right)
        {
            return left.activeItem.ID - right.activeItem.ID;
        }

        public void SendSceneActiveTaskSubmitRp(int iTemplateID, bool bOnce = true)
        {
            List<uint> akTaskIDs = new List<uint>();
            if(ActiveDictionary.ContainsKey(iTemplateID))
            {
                var activeData = ActiveDictionary[iTemplateID];
                if(activeData != null)
                {
                    int iMaxID = 0;
                    for(int i = 0; i < activeData.akChildItems.Count; ++i)
                    {
                        if(activeData.akChildItems[i].status == (int)Protocol.TaskStatus.TASK_OVER)
                        {
                            iMaxID = Math.Max(iMaxID, activeData.akChildItems[i].ID);
                        }
                    }
                    List<ActivityData> data = new List<ActivityData>();
                    data.InsertRange(0, activeData.akChildItems);
                    data.RemoveAll(x => { return x.status == (int)Protocol.TaskStatus.TASK_OVER; });
                    data.Sort(_SortActivities);

                    akTaskIDs.Clear();
                    int iSIRp = GetTemplateValue(iTemplateID, "SIRp");
                    if(bOnce && iSIRp > 1)
                    {
                        iSIRp = 1;
                    }
                    for (int i = 0; i < data.Count && i < iSIRp;++i)
                    {
                        akTaskIDs.Add((uint)data[i].ID);
                    }
                }
            }

            if(akTaskIDs.Count > 0)
            {
                SceneActiveTaskSubmitRp kSend = new SceneActiveTaskSubmitRp();
                kSend.taskId = akTaskIDs.ToArray();

                Logger.LogProcessFormat("[activity]SceneActiveTaskSubmitRp Array count = {0}", akTaskIDs.Count);

                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
            }
        }
        #endregion

        Dictionary<int, ActivityInfo> m_akActivitiesDic = new Dictionary<int, ActivityInfo>();

		public Dictionary<int, ActivityInfo> allActivities
		{
			get 
			{
				return m_akActivitiesDic;
			}
		}
        public class ActiveFrameConfig
        {
            public int iConfigID;
            public string prefabpath;
            public List<ProtoTable.ActiveMainTable> templates = new List<ProtoTable.ActiveMainTable>();
            public int iLinkTemplateID;
        }
        Queue<ActiveFrameConfig> m_akActiveFrameConfigs = new Queue<ActiveFrameConfig>();
        public ActiveFrameConfig PopAcitveFrameConfig()
        {
            if(m_akActiveFrameConfigs.Count > 0)
            {
                return m_akActiveFrameConfigs.Dequeue();
            }
            return null;
        }

        public void OpenActiveFrame(int iConfigID,int iLinkTemplateID = 0)
        {
            bool bNeedOpen = false;
            string kPrefabPath = "";
            if(m_akType2Templates.ContainsKey(iConfigID))
            {
                var templates = m_akType2Templates[iConfigID];
                if(templates != null && templates.Count > 0)
                {
                    for(int i = 0; i < templates.Count; ++i)
                    {
                        if (ActiveDictionary.ContainsKey(templates[i].ID))
                        {
                            kPrefabPath = templates[i].ActiveFrame;
                            bNeedOpen = true;
                            break;
                        }
                    }
                }
            }

            if(bNeedOpen)
            {
                ActiveFrameConfig kActiveFrameConfig = new ActiveFrameConfig();
                kActiveFrameConfig.iConfigID = iConfigID;
                kActiveFrameConfig.prefabpath = kPrefabPath;
                kActiveFrameConfig.templates = GetType2Templates(iConfigID);
                kActiveFrameConfig.iLinkTemplateID = iLinkTemplateID;
                m_akActiveFrameConfigs.Enqueue(kActiveFrameConfig);

                var frameName = "ActiveChargeFrame" + iConfigID;

                ClientSystemManager.GetInstance().CloseFrame(frameName);
                ClientSystemManager.GetInstance().OpenFrame<ActiveChargeFrame>(FrameLayer.Middle, kActiveFrameConfig, frameName);
            }
        }

        #region onclickActive
        enum ActiveIDType
        {
            AIDT_MinotaurParadise = 1000,
            AIDT_Nanbuxigu = 1001,
        }

        public int GetTemplateUpdateValue(int iTemplateID, string key, int iDefaultValue = 0)
        {
            int iSrcValue = iDefaultValue;
            ActiveManager.ActiveData data = null;
            if (ActiveManager.GetInstance().ActiveDictionary.TryGetValue(iTemplateID, out data))
            {
                var find = data.updateMainKeys.Find(x => { return x.key == key; });
                if (find != null && int.TryParse(find.value, out iSrcValue))
                {

                }
            }
            return iSrcValue;
        }

        public int GetTemplateValue(int iTemplateID,string key,int iDefaultValue = 0)
        {
            int iSrcValue = iDefaultValue;
            ActiveManager.ActiveData data = null;
            if (ActiveManager.GetInstance().ActiveDictionary.TryGetValue(iTemplateID, out data))
            {
                var find = data.mainKeyValue.Find(x => { return x.key == key; });
                if (find != null)
                {
                    int.TryParse(find.value, out iSrcValue);
                }
                else
                {
                    var countInfo = CountDataManager.GetInstance().GetCountInfo(iTemplateID + "_" + key);
                    if(null != countInfo)
                    {
                        iSrcValue = (int)countInfo.value;
                    }
                }
            }
            return iSrcValue;
        }

        public ActiveData GetActiveData(int iTemplateID)
        {
            ActiveData activeData = null;
            if (ActiveManager.GetInstance().ActiveDictionary.TryGetValue(iTemplateID, out activeData))
            {

            }
            return activeData;
        }

        /// <summary>
        /// 根据活动名得到活动数据
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public ActivityInfo GetActivityInfo(string activityName)
        {
            ActivityInfo activityInfo = null;

            var iter = allActivities.GetEnumerator();
            while (iter.MoveNext())
            {
                ActivityInfo info = iter.Current.Value as ActivityInfo;
                if (info == null)
                {
                    continue;
                }
                
                if (info.name != activityName)
                {
                    continue;
                }

                if (info.state != (byte)StateType.Running)
                {
                    continue;
                }

                activityInfo = info;
                break;
            }

            return activityInfo;
        }

        public ActivityData GetChildActiveData(int iActiveID)
        {
            var activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(iActiveID);
            if(activeItem != null)
            {
                ActiveData activeData = null;
                if (ActiveManager.GetInstance().ActiveDictionary.TryGetValue(activeItem.TemplateID, out activeData))
                {
                    var find = activeData.akChildItems.Find(x => { return x.activeItem.ID == iActiveID; });
                    return find;
                }
            }
            return null;
        }

        public int GetActiveItemValue(int iActiveID,string key, int iDefaultValue=0)
        {
            int iRet = iDefaultValue;
            var activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(iActiveID);
            if (activeItem != null)
            {
                ActiveManager.ActiveData activeData = null;
                if (ActiveManager.GetInstance().ActiveDictionary.TryGetValue(activeItem.TemplateID, out activeData))
                {
                    var find = activeData.akChildItems.Find(x => { return x.activeItem.ID == iActiveID; });
                    if (find != null)
                    {
                        var keyValuePair = find.akActivityValues.Find(x => { return x.key == key; });
                        if (keyValuePair != null && int.TryParse(keyValuePair.value, out iRet))
                        {
                        }
                    }
                }
            }

            //特殊情况，精力找回活动，活动中包含VIP字段，活动中VIP的字段由server同步。对应的活动没有同步下来，但是VIP可能存在，
            //从而导致系统中展示VIP的字段不正确。
            //这种情况下，应该使用用户的VIPLevel。
            //（活动同步数据的设计以及相关数据表格设计存在严重的缺陷）。。。。。。。
            if(iActiveID == 8101 && key == "vip")
            {
                //是否使用用户的VIPLevel
                if (PlayerBaseData.GetInstance().VipLevel > iRet)
                    iRet = PlayerBaseData.GetInstance().VipLevel;
            }

            return iRet;
        }

        void _OnLinkNext(List<string> nextLinks)
        {
            if (nextLinks != null && nextLinks.Count > 0)
            {
                string nextLink = nextLinks[0];
                nextLinks.RemoveAt(0);
                OnClickLinkInfo(nextLink, nextLinks);
            }
        }

        public static void CloseFrameByName(string name)
        {
            ClientSystemManager.GetInstance().CloseFrame(name);
        }

        public static void OnTeamListClicked(string param)
        {
            FunctionUnLock data = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Team);
            if (data == null)
            {
                return;
            }

            if (PlayerBaseData.GetInstance().Level < data.FinishLevel)
            {
                SystemNotifyManager.SystemNotify(1300031);
                return;
            }

            TeamListFrame.TryOpenTeamListOrTeamMyFrame();
            //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle);
        }

        public void OnClickLinkInfo(string link,List<string> nextLinks = null,bool isEliteDungeon = false)
        {
            try
            {
            var currentSystem = ClientSystemManager.GetInstance().GetCurrentSystem();
            if (currentSystem != null)
            {
                //if (currentSystem is ClientSystemBattle)
                //{
                //    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("jump_smithshopframe_tips"));
                //    return;
                //}

                if (currentSystem is ClientSystemGameBattle)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("chiji_scence_jump_smithshopframe_tips"));
                    return;
                }
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<GuildMainFrame>())
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildCloseMainFrame);
            }

            if (!string.IsNullOrEmpty(link))
            {
                if(nextLinks == null)
                {
                    var linkTokens = link.Split(new char[] { '\r', '\n' });
                        if(linkTokens == null)
                        {
                            return;
                        }
                    var linkLists = linkTokens.ToList();
                        if(linkLists == null)
                        {
                            return;
                        }
                    linkLists.RemoveAll(x =>
                    {
                        return string.IsNullOrEmpty(x);
                    });
                    if (linkLists.Count <= 0)
                    {
                        return;
                    }

                    var target = linkLists[0];
                    linkLists.RemoveAt(0);
                    OnClickLinkInfo(target, linkLists, isEliteDungeon);
                    return;
                }

                //地图链接
                Regex regex = new Regex(@"<type=mapid value=(\d+)>");
                    if(regex == null)
                    {
                        return;
                    }
                Match match = regex.Match(link);
                    if(match == null)
                    {
                        return;
                    }
                    if(match.Groups == null)
                    {
                        return;
                    }
                    if(match.Groups.Count <= 0)
                    {
                        return;
                    }
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                        if(match.Groups.Count <= 1)
                        {
                            return;
                        }
                    Parser.DungeonParser.OnClickLink(int.Parse(match.Groups[1].Value));
                    return;
                }
                //场景链接A
                regex = new Regex(@"<type=sceneid value=([0-9\|]+)>");
                    if (regex == null)
                    {
                        return;
                    }
                match = regex.Match(link);
                    if (match == null)
                    {
                        return;
                    }
                    if (match.Groups == null)
                    {
                        return;
                    }
                    if (match.Groups.Count <= 0)
                    {
                        return;
                    }
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                    if (systemTown == null)
                    {
                        systemTown = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                        if (systemTown == null)
                        {
                            return;
                        }
                    }

                    if (systemTown.MainPlayer != null)
                    {
                        int sceneid = 0;
                        List<int> sceneids = GamePool.ListPool<int>.Get();
                            if(sceneids == null)
                            {
                                return;
                            }
                            if(match.Groups.Count <= 1)
                            {
                                return;
                            }
                        var tokens = match.Groups[1].Value.Split(new char[] { '|'});
                            if(tokens == null)
                            {
                                return;
                            }
                        for (int matchIndex = 0; matchIndex < tokens.Length; ++matchIndex)
                        {
                            if(int.TryParse(tokens[matchIndex], out sceneid))
                            {
                                sceneids.Add(sceneid);
                            }
                        }

                        sceneid = -1;
                        for (int i = 0; i < sceneids.Count; i++)
                        {
                            if(isEliteDungeon)
                            {
                                if (GameClient.ChapterUtility.IsEliteChapterOpenBySceneId(sceneids[i]))
                                {
                                    sceneid = sceneids[i];
                                    break;
                                }
                            }
                            else
                            {
                                if (GameClient.ChapterUtility.IsChapterOpenBySceneID(sceneids[i]))
                                {
                                    sceneid = sceneids[i];
                                    break;
                                }
                            }
                           
                        }

                        GamePool.ListPool<int>.Release(sceneids);

                        if (-1 != sceneid)
                        {
                            Parser.SceneJump sceneJump = new Parser.SceneJump();
                                if(sceneJump == null)
                                {
                                    return;
                                }
                            sceneJump.onLinkOk = () =>
                            {
                                _OnLinkNext(nextLinks);
                            };
                                if(systemTown.MainPlayer == null)
                                {
                                    return;
                                }
                            BeTownPlayerMain.CommandStopAutoMove();
                            BeTownPlayerMain.OnMoveStateChanged.AddListener(sceneJump.OnMoveStateChanged);
                            BeTownPlayerMain.OnAutoMoveSuccess.AddListener(sceneJump.OnMoveSuccess);
                            BeTownPlayerMain.OnAutoMoveFail.AddListener(sceneJump.OnAutoMoveFail);
                            systemTown.MainPlayer.CommandMoveToScene(sceneid);
                        }
                        else
                        {
                            if(isEliteDungeon)
                            {
                                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("IntegrationChallenge_JYdungeonCannotChanllengeTip"));
                            }
                            else
                            {
                                 //Logger.LogError("link info error! to 董恰!");
                                  SystemNotifyManager.SysNotifyFloatingEffect("章节未解锁");
                            }
                           
                        }
                    }
                    
                    return;
                }

                    //场景链接B
                regex = new Regex(@"<type=sceneJump id=(\d+) doorid=(\d+)>");
                if (regex == null)
                {
                    return;
                }
                match = regex.Match(link);
                if (match == null)
                {
                    return;
                }
                if (match.Groups == null)
                {
                    return;
                }
                if (match.Groups.Count <= 0)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                    if (systemTown == null)
                    {
                        return;
                    }
                    if (match.Groups.Count <= 1)
                    {
                        return;
                    }
                    int targetSceneID = int.Parse(match.Groups[1].Value);
                    if (match.Groups.Count <= 2)
                    {
                        return;
                    }
                    int doorId = int.Parse(match.Groups[2].Value);

                    // dd: 临时代码，去掉PK
                    if (!SwitchFunctionUtility.IsPKOpen && targetSceneID == 5003)
                    {
                        return;
                    }

                    systemTown.SwitchToTargetScene(targetSceneID, doorId,
                    () =>
                    {
                        _OnLinkNext(nextLinks);
                    });
                    return;
                }

                    //界面链接
                    regex = new Regex(@"<type=framename param=([-0-9A-Za-z|]+) value=(.+)>");
                    if(regex == null)
                    {
                        return;
                    }
                match = regex.Match(link);
                    if(match == null)
                    {
                        return;
                    }
                    if(match.Groups == null)
                    {
                        return;
                    }
                    if(match.Groups.Count <= 0)
                    {
                        return;
                    }
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                        if(match.Groups.Count <= 1)
                        {
                            return;
                        }
                    string strParam = match.Groups[1].Value;
                    Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
                        if(match.Groups.Count <= 2)
                        {
                            return;
                        }
                    Type type = assembly.GetType(match.Groups[2].Value);
                        if(type == null)
                        {
                            return;
                        }

                    MethodInfo mf = type.GetMethod("OpenLinkFrame");
                    if(mf != null)
                    {
						ClientSystemManager.GetInstance().CloseFrameByType(type,true);
                        mf.Invoke(null, new object[] { strParam });
                    }
                    return;
                }
                regex = new Regex(@"<type=framename value=(.+)>");
                    if(regex == null)
                    {
                        return;
                    }
                match = regex.Match(link);
                    if(match == null)
                    {
                        return;
                    }
                    if(match.Groups == null)
                    {
                        return;
                    }
                    if(match.Groups.Count <= 0)
                    {
                        return;
                    }
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
					//Logger.LogErrorFormat("try GetExecutingAssembly match.Groups[0].Value:{0}", match.Groups[0].Value);
                    Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
                        if(match.Groups.Count <= 1)
                        {
                            return;
                        }
                    Type type = assembly.GetType(match.Groups[1].Value);
                    if (type != null)
                    {
						//Logger.LogErrorFormat("try open frame:{0}", match.Groups[1].Value);
						GameClient.ClientSystemManager.GetInstance().CloseFrameByType(type, true);
                        GameClient.ClientSystemManager.GetInstance().OpenFrame(type, FrameLayer.Middle);
                    }
					else
					{
						Logger.LogErrorFormat("can't find GetExecutingAssembly:{0}", match.Groups[0].Value);
					}
                    return;
                }

                //静态函数链接
                regex = new Regex(@"<type=funtionname param=([-0-9A-Za-z|]+) space=([0-9a-zA-Z\.]+) name=([a-zA-Z_][0-9a-zA-Z_]*)>");
                    if(regex == null)
                    {
                        return;
                    }
                match = regex.Match(link);
                    if(match == null)
                    {
                        return;
                    }
                    if(match.Groups == null)
                    {
                        return;
                    }
                if(match.Success)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
                        if (match.Groups.Count <= 2)
                        {
                            return;
                        }
                    Type type = assembly.GetType(match.Groups[2].Value);
                    if(null != type)
                    {
                            if(match.Groups.Count <= 3)
                            {
                                return;
                            }
                        MethodInfo mf = type.GetMethod(match.Groups[3].Value);
                        if(null == mf)
                        {
                            Logger.LogErrorFormat("can not find funtion with name = {0} assembly = {1}!!!", match.Groups[3].Value, match.Groups[2].Value);
                            Logger.LogErrorFormat("linkinfo error : {0}", link);
                            return;
                        }

                        try
                        {
                            mf.Invoke(null, new object[] { match.Groups[1].Value });
                        }
                        catch (System.Exception ex)
                        {
                            Logger.LogErrorFormat("linkinfo error : {0}", link);
                            Logger.LogErrorFormat("linkinfo error : {0}", ex.ToString());
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("can't find GetExecutingAssembly:{0}", match.Groups[2].Value);
                    }
                    return;
                }

                //先NPC再商店链接
                regex = new Regex(@"<type=npcframe npcid=(\d+) param=([0-9A-Za-z|]*)>");
                    if(regex == null)
                    {
                        return;
                    }
                match = regex.Match(link);
                    if(match == null)
                    {
                        return;
                    }
                    if(match.Groups == null)
                    {
                        return;
                    }
                    if(match.Groups.Count <= 0)
                    {
                        return;
                    }
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                        if(match.Groups.Count <= 1)
                        {
                            return;
                        }
                    int iNpcID = int.Parse(match.Groups[1].Value);
                    var npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
                    if (npcItem != null)
                    {
                        Parser.NpcParser.OnClickLink(iNpcID,
                            () =>
                            {
                                TaskNpcAccess.OnClickFunctionNpc(iNpcID, 0, match.Groups[2].Value);
                            });
                    }
                    return;
                }

                //查找怪物攻城
                regex = new Regex(@"<type=attackcitymonster>");
                    if(regex == null)
                    {
                        return;
                    }
                match = regex.Match(link);
                    if(match == null)
                    {
                        return;
                    }
                    if(match.Groups == null)
                    {
                        return;
                    }
                    if(match.Groups.Count <= 0)
                    {
                        return;
                    }
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    AttackCityMonsterDataManager.GetInstance().EnterFindPathProcessByActivityDuplication();
                    return;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogErrorFormat("GameClient.ActiveManager.OnClickLinkInfo,catch Exception:Exception = {0}", ex.ToString());
                string nextLinkStr = "";
                if(nextLinks != null)
                {
                    foreach (var str in nextLinks)
                    {
                        nextLinkStr += str;
                        nextLinkStr += ",";
                    }
                }
                Logger.LogErrorFormat("GameClient.ActiveManager.OnClickLinkInfo,catch Exception:Params = {0}|{1}|{2}", link,nextLinkStr,isEliteDungeon);
            }
        }

        public bool CheckCondition(ActiveManager.ActivityData childData)
        {
            if(string.IsNullOrEmpty(childData.activeItem.Param0))
            {
                Logger.LogErrorFormat("CheckCondition Error!");
                return false;
            }

            var tokens = childData.activeItem.Param0.Split(',');
            for(int i = 0; i < tokens.Length; ++i)
            {
                var condition = tokens[i].Split('_');
                int dataId = 0;
                int count = 0;

                if(condition.Length != 2 || !int.TryParse(condition[0],out dataId) || !int.TryParse(condition[1],out count))
                {
                    Logger.LogErrorFormat("CheckCondition Error!");
                    continue;
                }

                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(dataId, true);
                if(count > 0)
                {
                    if(iHasCount < count)
                    {
                        GameClient.ItemComeLink.OnLink(dataId, count - iHasCount, true,null,true);
                        return false;
                    }
                }
            }

            return true;
        }

        public bool _CheckCanJumpGo(IList<int> datas,bool bNeedMsg)
        {
            if(null == datas || datas.Count != 2)
            {
                return true;
            }

            if(datas[0] < 0)
            {
                var function = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(datas[1]);
                if(null != function && PlayerBaseData.GetInstance().Level < function.FinishLevel)
                {
                    if(bNeedMsg)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("active_jump_need_lv", function.FinishLevel));
                    }
                    return false;
                }
                return true;
            }

            if(1 == datas[0])
            {
                if(PlayerBaseData.GetInstance().Level < datas[1])
                {
                    if (bNeedMsg)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("active_jump_need_lv", datas[1]));
                    }
                    return false;
                }
                return true;
            }

            return true;
        }

        public void OnClickActivity(ActiveData activityData,OnClickActive onClick, ActiveManager.ActivityData childData)
        {
            if(activityData == null || onClick == null)
            {
                Logger.LogError("ActiveData is null or missing script OnClickActive!");
                return;
            }
            if (!(onClick.m_eOnClickActiveType > OnClickActive.OnClickActiveType.OCAT_INVALID && onClick.m_eOnClickActiveType < OnClickActive.OnClickActiveType.OCAT_COUNT))
            {
                Logger.LogError("onClick.m_eOnClickActiveType is Invalid!");
                return;
            }

            if (onClick.m_eNodeType == OnClickActive.NodeType.NT_ROOT)
            {
                if (onClick.m_eOnClickActiveType == OnClickActive.OnClickActiveType.OCAT_GO)
                {
                    Regex regex = new Regex(@"<type=(\w+) id=(\d+)>");
                    Match macth = regex.Match(activityData.mainItem.FunctionParse);
                    if(macth != null && !string.IsNullOrEmpty(macth.Groups[0].Value))
                    {
                        if(macth.Groups[1].Value == "dungen")
                        {
                            int iLinkDungen = 0;
                            if(int.TryParse(macth.Groups[2].Value,out iLinkDungen))
                            {
                                Parser.DungeonParser.OnClickLink(iLinkDungen);
                                //if (Parser.DungeonParser.OnClickLink(iLinkDungen))
                                //{
                                //    ClientSystemManager.GetInstance().CloseFrame<ActiveChargeFrame>();
                                //}
                            }
                        }
                    }
                    Logger.LogProcessFormat("OCAT_GO {0}", activityData.mainItem.Name);
                }
                else if (onClick.m_eOnClickActiveType == OnClickActive.OnClickActiveType.OCAT_ACQUIRED)
                {
                    Logger.LogProcessFormat("OCAT_ACQUIRED {0}", activityData.mainItem.Name);
                }
                else if(onClick.m_eOnClickActiveType == OnClickActive.OnClickActiveType.OCAT_EVENT)
                {
                    if (onClick.m_eEventType == OnClickActive.EventType.EventType_OpenSignFrame)
                    {
                        if (ClientSystemManager.GetInstance().IsFrameOpen<SignFrame>())
                        {
                            ClientSystemManager.GetInstance().CloseFrame<SignFrame>();
                        }
                        ClientSystemManager.GetInstance().OpenFrame<SignFrame>();
                    }
                    else if (onClick.m_eEventType == OnClickActive.EventType.EventType_OpenSeventDayAwardFrame)
                    {
                    }
                    else if (onClick.m_eEventType == OnClickActive.EventType.EventType_PerfectAcquireAward)
                    {
                    }
                    else if (onClick.m_eEventType == OnClickActive.EventType.EventType_NormalAcquireAward)
                    {
                    }
                    else if (onClick.m_eEventType == OnClickActive.EventType.EventType_Pl_Normal_AcquireAward)
                    {
                        if (childData.status == (int)OnClickActive.BindStatus.BS_FINISH)
                        {

                        }
                    }
                    else if (onClick.m_eEventType == OnClickActive.EventType.EventType_Pl_Perfect_AcquireAward)
                    {
                        if (childData.status == (int)OnClickActive.BindStatus.BS_FINISH)
                        {

                        }
                    }
                    else if (onClick.m_eEventType == OnClickActive.EventType.EventType_Diamond_BeVip)
                    {
                        ActiveChargeFrame.CloseMe();

                        var vipFrame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                        if (vipFrame != null)
                        {
                            vipFrame.OpenPayTab();
                        }
                    }
                }
            }
            else if (onClick.m_eNodeType == OnClickActive.NodeType.NT_CHILD && childData != null)
            {
                if((int)onClick.m_eBindStatus == childData.status)
                {
                    if (onClick.m_eBindStatus == OnClickActive.BindStatus.BS_FINISH)
                    {
                        if(onClick.m_eAttachParamsType == OnClickActive.AttachParamsType.APT_NONE)
                        {
                            SendSubmitActivity(childData.ID);
                        }
                        else if(onClick.m_eAttachParamsType == OnClickActive.AttachParamsType.APT_CHECK_CONDITION)
                        {
                            if(CheckCondition(childData))
                            {
                                SendSubmitActivity(childData.ID);
                            }
                        }
                        else
                        {
                            SendSubmitActivity(childData.ID);
                        }
                    }
                    else if(onClick.m_eBindStatus == OnClickActive.BindStatus.BS_UNFINISH ||
                        onClick.m_eBindStatus == OnClickActive.BindStatus.BS_INIT)
                    {
                        if(_CheckCanJumpGo(childData.activeItem.LinkLimit,true))
                        {
                            OnClickLinkInfo(childData.activeItem.LinkInfo);
                        }
                    }
                }
            }
        }

        public void SignalRedPoint(ActivityData data)
        {
            if(!m_akRedPointMap.ContainsKey(data.activeItem.ID))
            {
                m_akRedPointMap.Add(data.activeItem.ID, 0);
                if(onActivityUpdate != null)
                {
                    onActivityUpdate.Invoke(data, ActivityUpdateType.AUT_RED_CHANGED);
                }
            }
        }

        public bool CheckHasFinishedChildItem(ActiveData activityData,List<string> keys)
        {
            if(activityData != null)
            {
                for(int i = 0; i < activityData.akChildItems.Count; ++i)
                {
                    if(keys == null || !keys.Contains(activityData.akChildItems[i].activeItem.PrefabKey))
                    {
                        continue;
                    }

                    if(CheckChildRedPass(activityData.akChildItems[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckChildRedPass(ActivityData data)
        {
            if(data == null)
            {
                return false;
            }

            if (data.activeItem.DoesWorkToRedPoint != 1)
            {
                return false;
            }

            if (data.status != (int)Protocol.TaskStatus.TASK_FINISHED)
            {
                return false;
            }

            if(data.activeItem.IsWorkWithFullLevel == 0)
            {
                if(PlayerBaseData.GetInstance().IsLevelFull)
                {
                    return false;
                }
            }

            if (data.activeItem.RedPointWorkMode == 0)
            {
                return true;
            }

            if (data.activeItem.RedPointWorkMode == 1)
            {
                if (m_akRedPointMap.ContainsKey(data.activeItem.ID))
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        Dictionary<int, int> m_akRedPointMap = new Dictionary<int, int>();

        public List<AwardItemData> GetActiveAwards(int iActiveId)
        {
            ProtoTable.ActiveTable activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(iActiveId);
            if (activeItem == null)
            {
                return null;
            }

            List<AwardItemData> awardDatas = new List<AwardItemData>();
            Regex s_dynamic_award = new Regex(@"<KeyNum=(\w+) KeyId=(\w+) KeySize=(\w+)>", RegexOptions.Singleline);

            bool bDynamic = false;
            if (!string.IsNullOrEmpty(activeItem.DanymicAwards))
            {
                bDynamic = true;
                var match = s_dynamic_award.Match(activeItem.DanymicAwards);
                if (match != null && !string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    int iCount = ActiveManager.GetInstance().GetActiveItemValue(activeItem.ID, match.Groups[3].Value);
                    for (int i = 0; i < iCount; ++i)
                    {
                        int iTableID = ActiveManager.GetInstance().GetActiveItemValue(activeItem.ID, match.Groups[2].Value + (i + 1).ToString());
                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iTableID);
                        if(item == null)
                        {
                            continue;
                        }
                        int iNum = ActiveManager.GetInstance().GetActiveItemValue(activeItem.ID, match.Groups[1].Value + (i + 1).ToString());
                        if (iNum <= 0)
                        {
                            continue;
                        }

                        awardDatas.Add(new AwardItemData
                        {
                            ID = iTableID,
                            Num = iNum,
                        });
                    }
                }
                else
                {
                    Logger.LogErrorFormat("MATCH ERROR WITH DanymicAwards ActiveID is {0}", activeItem.ID);
                }
            }

            if (!bDynamic && !string.IsNullOrEmpty(activeItem.Awards))
            {
                var awards = activeItem.Awards.Split(new char[] { ',' });
                for (int i = 0; i < awards.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(awards[i]))
                    {
                        var substrings = awards[i].Split(new char[] { '_' });
                        if (substrings.Length == 2)
                        {
                            int id = int.Parse(substrings[0]);
                            int iCount = int.Parse(substrings[1]);
                            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
                            if (item == null || iCount <= 0)
                            {
                                continue;
                            }

                            awardDatas.Add(new AwardItemData
                            {
                                ID = id,
                                Num = iCount,
                            });
                        }
                        else if (substrings.Length == 4)
                        {
                            int id = int.Parse(substrings[0]);
                            int iCount = int.Parse(substrings[1]);
                            int iEquipType = int.Parse(substrings[2]);
                            int iStrengthenLevel = int.Parse(substrings[3]);
                            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
                            if (item == null || iCount <= 0)
                            {
                                continue;
                            }

                            awardDatas.Add(new AwardItemData
                            {
                                ID = id,
                                Num = iCount,
                                EquipType = iEquipType,
                                StrengthenLevel = iStrengthenLevel
                            });
                        }
                    }
                }
            }

            return awardDatas;
        }
        #endregion
    }
}
