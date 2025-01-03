using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;
///////删除linq
using System.Text.RegularExpressions;
using UnityEngine.UI;
using MissionValue = GameClient.MissionManager.SingleMissionInfo;

namespace GameClient
{
    public class LegendNotifyData
    {
        public int iNotifyID;
        public bool bNotify;
    }

    public interface ITask
    {
        void DoSomething();
    }

    public class DelayTask
    {
        float fStartTime = 0.0f;
        float fDelayTime = 0.0f;
        UnityEngine.Events.UnityAction callback;
        public DelayTask(float fStartTime,float fDelayTime,UnityEngine.Events.UnityAction callback)
        {
            this.fStartTime = fStartTime;
            this.fDelayTime = fDelayTime;
            this.callback = callback;
        }

        public void DoSomething()
        {
            if(callback != null)
            {
                callback.Invoke();
            }
        }

        public bool CanExecuted()
        {
            return Time.time >= fStartTime + fDelayTime;
        }
    }

    public class MissionManager : DataManager<MissionManager>
    {
        #region _Declare
        bool bLoadingScene = false;
        public class CachedMsg
        {
            public CachedMsg(UInt32 id,MsgDATA msgData)
            {
                this.id = id;
                this.msgData = msgData;
            }
            public UInt32 id;
            public MsgDATA msgData;
        }
        Queue<CachedMsg> m_akCachedNetMsg = new Queue<CachedMsg>();

        public class SingleMissionInfo : IComparable<SingleMissionInfo>
        {
            public UInt32 taskID = 0;
            public Byte status = 0;
            public Dictionary<string, string> taskContents = new Dictionary<string, string>();
            public ProtoTable.MissionTable missionItem = null;
            public uint finTime = 0;
            public byte submitCount = 0;            //领取次数
            public SingleMissionInfo()
            {
                taskID = 0;
                status = 0;
                taskContents.Clear();
                missionItem = null;
                finTime = 0;
                submitCount = 0;
            }
            public string GetKeyValue(string key)
            {
                if(taskContents.ContainsKey(key))
                {
                    return taskContents[key];
                }
                return "";
            }

            public int GetIntValue(string key)
            {
                int iRet = 0;
                if (taskContents.ContainsKey(key))
                {
                    if(!int.TryParse(taskContents[key],out iRet))
                    {
                        Logger.LogErrorFormat("attempt to convert non-int string 2 int type");
                    }
                }
                return iRet;
            }
#region IComparable implementation

            private static int[] _sortOrder = new int[] 
            {
                2, //TASK_INIT = 0,
                3, //TASK_UNFINISH = 1,
                1, //TASK_FINISHED = 2,
                4, //TASK_FAILED = 3,
                0, //TASK_SUBMITTING = 4,
                5, //TASK_OVER = 5,
            };

            public int CompareTo(GameClient.MissionManager.SingleMissionInfo other)
            {
                if (status != other.status)
                {
                    return _sortOrder[status] - _sortOrder[other.status];
                }
                else
                {
                    int cid = (int)taskID;
                    int oid = (int)other.taskID;
                    return cid - oid;
                }
            }

            private static int[] _LegendSortOrder = new int[]
            {
                4, //TASK_INIT = 0,
                2, //TASK_UNFINISH = 1,
                1, //TASK_FINISHED = 2,
                3, //TASK_FAILED = 3,
                0, //TASK_SUBMITTING = 4,
                5, //TASK_OVER = 5,
            };

            public int LegendCompareTo(GameClient.MissionManager.SingleMissionInfo other)
            {
                if (status != other.status)
                {
                    return _LegendSortOrder[status] - _LegendSortOrder[other.status];
                }

                if(missionItem.SortID != other.missionItem.SortID)
                {
                    return missionItem.SortID - other.missionItem.SortID;
                }

                int cid = (int)taskID;
                int oid = (int)other.taskID;

                return cid - oid;
            }
            #endregion
        }

        private Dictionary<UInt32, SingleMissionInfo> taskSet = new Dictionary<UInt32, SingleMissionInfo>();
        public Dictionary<UInt32, SingleMissionInfo> taskGroup
        {
            get
            {
                return taskSet;
            }
        }

        public SingleMissionInfo GetMissionInfo(UInt32 missionID)
        {
            if(taskGroup != null)
            {
                SingleMissionInfo info = null;
                if( taskGroup.TryGetValue(missionID,out info) )
                {
                    return info;
                }
            }

            return null;
        }
        
        private Dictionary<ProtoTable.MissionTable.eTaskType, List<SingleMissionInfo>> m_akDiffTasks = new Dictionary<MissionTable.eTaskType, List<SingleMissionInfo>>();

        private Dictionary<Int32, TaskDialogFrame> dialogFrames = new Dictionary<Int32, TaskDialogFrame>();
        private Queue<Int32> dialogIds = new Queue<Int32>();

        Dictionary<UInt32, UInt32> cachedAutoAcceptTask = new Dictionary<UInt32, UInt32>();
        public Dictionary<UInt32, UInt32> CachedAutoAcceptTask
        {
            get { return cachedAutoAcceptTask; }
        }

        public bool dungenStart
        {
            get;set;
        }

        public int getSortIDUseType(ProtoTable.MissionTable.eTaskType type)
        {
            int sortID = 0;
            switch(type)
            {
                
                case (ProtoTable.MissionTable.eTaskType.TT_AWAKEN):
                    sortID = 200;
                    break;
                case (ProtoTable.MissionTable.eTaskType.TT_CHANGEJOB):
                    sortID = 150;
                    break;
                case (ProtoTable.MissionTable.eTaskType.TT_MAIN):
                    sortID = 100;
                    break;
                case (ProtoTable.MissionTable.eTaskType.TT_CYCLE):
                    sortID = 75;
                    break;
                case (ProtoTable.MissionTable.eTaskType.TT_BRANCH):
                    sortID = 50;
                    break;
                default:
                    sortID = 0;
                    break;

            }
            return sortID;
        }

        #endregion

        public override void OnEnterSystem()
        {

        }

        public override void OnExitSystem()
        {
            while (dialogIds.Count > 0)
            {
                dialogIds.Dequeue();
            }
            dialogFrames.Clear();
            cachedAutoAcceptTask.Clear();
            iLockedMissionID = 0;
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.MissionManager;
        }

        /// <summary>
        /// 武研院是否有红点
        /// </summary>
        /// <returns></returns>
        public bool HasRedPoint()
        {
            List<InstituteTable> list = TableManager.instance.GetJobInstituteData(PlayerBaseData.GetInstance().JobTableID);
            for (int i = 0; i < list.Count; i++)
            {
                int state = GetState(list[i]);
                if (state == 0) return true;
            }
            return false;
        }

        /// <summary>
        /// 0可以挑战1复习2尚未解锁3等级限制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public  int GetState(InstituteTable data)
        {
            if (data.LevelLimit > PlayerBaseData.GetInstance().Level)
                return 3;
            List<Battle.DungeonOpenInfo> list = BattleDataManager.GetInstance().ChapterInfo.openedList;

            Battle.DungeonOpenInfo dungeonData = list.Find(x => { return x.id == data.DungeonID; });
            if (dungeonData == null)
                return 2;
            else
            {
                MissionTable missionTableData = TableManager.GetInstance().GetTableItem<MissionTable>(data.MissionID);
                if (missionTableData != null)
                {
                    SingleMissionInfo task = GetMissionInfo((uint)missionTableData.ID);
                    if (task != null)
                    {
                        if (task.status != (int)Protocol.TaskStatus.TASK_OVER)
                        {
                            return 0;
                        }
                    }
                }
                return 1;
            }
        }

        #region _Interface
        public IClientFrame CreateDialogFrame(Int32 dialogID, Int32 iCurTaskId,TaskDialogFrame.OnDialogOver callback = null)
        {
            if (!dialogFrames.ContainsKey(dialogID))
            {
                dialogIds.Enqueue(dialogID);
                if (ClientSystemManager.GetInstance().IsFrameOpen<TaskDialogFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<TaskDialogFrame>();
                }
                IClientFrame frame = ClientSystemManager.GetInstance().OpenFrame<TaskDialogFrame>();

                UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                uiEvent.EventParams.taskData.taskID = iCurTaskId;
                uiEvent.EventID = EUIEventID.Dlg2TaskId;
                UIEventSystem.GetInstance().SendUIEvent(uiEvent);

                if(callback != null)
                {
                    UIEventSystem.GetInstance().SendUIEvent(new UIEventDialogCallBack(callback));
                }

                return frame;
            }
            return null;
        }

        public void CloseAllDialog()
        {
            while (dialogIds.Count > 0)
            {
                dialogIds.Dequeue();
            }

            List<int> test = new List<int>(dialogFrames.Keys);
            for (int i = 0; i < test.Count; ++i)
            {
                TaskDialogFrame dlgFrame;
                if (dialogFrames.TryGetValue(test[i], out dlgFrame))
                {
                    dialogFrames.Remove(test[i]);
                    dlgFrame.OnClose();
                }
            }
            dialogFrames.Clear();
        }

        public Int32 AddKeyDlg2Frame(TaskDialogFrame clientFrame)
        {
            if (dialogIds.Count > 0 && null != clientFrame)
            {
                dialogFrames.Add(dialogIds.Peek(), clientFrame);
                return dialogIds.Dequeue();
            }

            return 0;
        }

        public TaskDialogFrame GetDlgFrameByName(string dlgName)
        {
            var keys = dialogFrames.Keys.ToList();
            var values = dialogFrames.Values.ToList();
            for(int i = 0; i < keys.Count; ++i)
            {
                if (values[i] == null)
                {
                    continue;
                }

                string name = values[i].GetName().Replace("(Clone)", "");
                if (dlgName.EndsWith(name))
                {
                    return values[i];
                }
            }

            return default(TaskDialogFrame);
        }

        public void RemoveDlgFrame(Int32 key)
        {
            if (dialogFrames.ContainsKey(key))
            {
                dialogFrames.Remove(key);
            }
        }
        #endregion

        public override void Initialize()
        {
            RegisterNetHandler();
            _InitLevel2MissionItems();
            _InitMissionScore();
            ClientSystemManager.GetInstance().OnSwitchSystemBegin.AddListener(OnSceneLoadBegin);
            ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(OnSceneLoadEnd);
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
            PlayerBaseData.GetInstance().onMissionScoreChanged += OnMissionScoreChanged;

            finisedTaskIDs = new List<uint>();
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneDailyTaskList.MsgID, OnRecvDailyTaskList);
            NetProcess.AddMsgHandler(SceneAchievementTaskList.MsgID, OnRecvAchievementTaskList);
            NetProcess.AddMsgHandler(SceneTaskListRet.MsgID, OnRecvTaskList);
            NetProcess.AddMsgHandler(SceneNotifyNewTaskRet.MsgID, OnRecvNotifyNewTask);
            NetProcess.AddMsgHandler(SceneNotifyDeleteTaskRet.MsgID, OnRecvNotifyDeleteTask);
            NetProcess.AddMsgHandler(SceneNotifyTaskStatusRet.MsgID, OnRecvNotifyTaskStatus);
            NetProcess.AddMsgHandler(SceneNotifyTaskVarRet.MsgID, OnRecvNotifyTaskVar);
            NetProcess.AddMsgHandler(SceneLegendTaskListRes.MsgID, OnRecvLegendTaskList);
            NetProcess.AddMsgHandler(SceneResetTaskSync.MsgID, OnReceiveTaskSync);
            NetProcess.AddMsgHandler(SceneDailyScoreRewardRes.MsgID, OnReceiveSceneDailyScoreRewardRes);
            NetProcess.AddMsgHandler(SceneFinishedTaskList.MsgID, OnReceiveSceneFinishedTaskList);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneDailyTaskList.MsgID, OnRecvDailyTaskList);
            NetProcess.RemoveMsgHandler(SceneAchievementTaskList.MsgID, OnRecvAchievementTaskList);
            NetProcess.RemoveMsgHandler(SceneTaskListRet.MsgID, OnRecvTaskList);
            NetProcess.RemoveMsgHandler(SceneNotifyNewTaskRet.MsgID, OnRecvNotifyNewTask);
            NetProcess.RemoveMsgHandler(SceneNotifyDeleteTaskRet.MsgID, OnRecvNotifyDeleteTask);
            NetProcess.RemoveMsgHandler(SceneNotifyTaskStatusRet.MsgID, OnRecvNotifyTaskStatus);
            NetProcess.RemoveMsgHandler(SceneNotifyTaskVarRet.MsgID, OnRecvNotifyTaskVar);
            NetProcess.RemoveMsgHandler(SceneLegendTaskListRes.MsgID, OnRecvLegendTaskList);
            NetProcess.RemoveMsgHandler(SceneResetTaskSync.MsgID, OnReceiveTaskSync);
            NetProcess.RemoveMsgHandler(SceneDailyScoreRewardRes.MsgID, OnReceiveSceneDailyScoreRewardRes);
            NetProcess.RemoveMsgHandler(SceneFinishedTaskList.MsgID, OnReceiveSceneFinishedTaskList);
        }

        public SingleMissionInfo GetMission(uint iTaskID)
        {
            if(taskGroup.ContainsKey(iTaskID))
            {
                return taskGroup[iTaskID];
            }
            return null;
        }

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            _CheckLevelFitDailyMission();
        }

        void OnMissionScoreChanged(int iValue)
        {
            Score = iValue;
        }

        public void AddSystemInvoke()
        {
            RemoveSystemInvoke();
            ClientSystemManager.GetInstance().OnSwitchSystemBegin.AddListener(OnSceneLoadBegin);
            ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(OnSceneLoadEnd);
        }

        public void RemoveSystemInvoke()
        {
            ClientSystemManager.GetInstance().OnSwitchSystemBegin.RemoveListener(OnSceneLoadBegin);
            ClientSystemManager.GetInstance().OnSwitchSystemFinished.RemoveListener(OnSceneLoadEnd);
        }
        int m_iFunctionTraceId = -1;
        public int FunctionTraceID
        {
            get
            {
                return m_iFunctionTraceId;
            }
            set
            {
                m_iFunctionTraceId = value;
            }
        }
        public void AutoTraceTask(Int32 iSelectedID,
            UnityEngine.Events.UnityAction onSuccessed = null,
            UnityEngine.Events.UnityAction onFailed = null,
            bool bForceSubmit = false)
        {
            ProtoTable.MissionTable mission = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iSelectedID);
            if (mission != null)
            {
                GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_FIND_ROAD,iSelectedID);

                MissionManager.SingleMissionInfo singleMissionInfo = null;
                if (MissionManager.GetInstance().taskGroup.TryGetValue((uint)mission.ID, out singleMissionInfo))
                {
                    if (singleMissionInfo.status == (int)Protocol.TaskStatus.TASK_INIT)
                    {
                        MissionManager.GetInstance().OnExecuteAcceptTask(mission.ID,true,onSuccessed,onFailed, bForceSubmit);
                        return;
                    }
                    else if (singleMissionInfo.status == (int)Protocol.TaskStatus.TASK_UNFINISH)
                    {
                        if (mission.SubType == ProtoTable.MissionTable.eSubType.SummerNpc)
                        {
                            //召唤怪物类型的任务，直接进入自动寻路到怪物的流程
                            AttackCityMonsterDataManager.GetInstance().EnterFindPathProcessByMissionContent(singleMissionInfo.taskContents,
                                onSuccessed,
                                onFailed);
                        }
                        else
                        {
                            if (mission.TaskFinishType == ProtoTable.MissionTable.eTaskFinishType.TFT_ACCESS_SHOP)
                            {
                                ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(mission.SeekingTarget);
                                if (npcItem != null && npcItem.Function == ProtoTable.NpcTable.eFunction.shopping)
                                {
                                    Parser.NpcParser.OnClickLink(npcItem.ID, mission.ID, false, onSuccessed, onFailed);
                                    return;
                                }
                            }
                            else if (mission.TaskFinishType == ProtoTable.MissionTable.eTaskFinishType.TFT_SUBMIT_ITEM)
                            {
                                ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(mission.SeekingTarget);
                                if (npcItem != null /*&& npcItem.Function == ProtoTable.NpcTable.eFunction.shopping*/)
                                {
                                    if (!bForceSubmit)
                                    {
                                        Parser.NpcParser.OnClickLink(npcItem.ID, mission.ID, false, () => { ClientSystemManager.instance.OpenFrame<SubmitItemDlg>(FrameLayer.High, mission.ID); }, onFailed);
                                    }
                                    else
                                    {
                                        ClientSystemManager.instance.OpenFrame<SubmitItemDlg>(FrameLayer.High, mission.ID);
                                    }
                                    return;
                                }
                            }
                            else if (mission.TaskFinishType == ProtoTable.MissionTable.eTaskFinishType.TFT_LINKS)
                            {
                                if (!string.IsNullOrEmpty(mission.LinkInfo))
                                {
                                    if (onSuccessed != null)
                                    {
                                        onSuccessed.Invoke();
                                    }
                                    //Logger.LogErrorFormat("before ActiveManager.GetInstance().OnClickLinkInfo:{0}", mission.LinkInfo);
                                    ActiveManager.GetInstance().OnClickLinkInfo(mission.LinkInfo);
                                }
                                return;
                            }
                            else
                            {
                                MissionManager.GetInstance().OnExecuteDungenTrace(mission.ID, onSuccessed, onFailed);
                                return;
                            }
                        }
                    }
                    else if (singleMissionInfo.status == (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        MissionManager.GetInstance().OnExecuteSubmitTask(mission.ID, onSuccessed, onFailed,bForceSubmit);
                        return;
                    }
                }
                else
                {
                    //ExceptionManager.GetInstance().RecordLog(string.Format("mission is not existed in dictionary with mission.ID = {0}", mission.ID));
                }
            }
            else
            {
                //ExceptionManager.GetInstance().RecordLog(string.Format("mission is null with iSelectedID = {0}", iSelectedID));
            }

            if (onFailed != null)
            {
                onFailed.Invoke();
            }
        }

        public void AcceptChangeJobMissions(Int32 iJobID)
        {
            var table = TableManager.GetInstance().GetTable<MissionTable>();
            var enumerator = table.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var Item = enumerator.Current.Value as MissionTable;

                if (Item.TaskType != MissionTable.eTaskType.TT_CHANGEJOB)
                {
                    continue;
                }

                Int32 iTargetID = Int32.Parse(Item.MissionParam);
                if (iTargetID != iJobID)
                {
                    continue;
                }

                sendCmdAcceptTask((uint)Item.ID, (TaskSubmitType)Item.AcceptType, (uint)Item.MissionTakeNpc);

                break;
            }
        }

        public void AcceptAwakeMissions(Int32 iJobID)
        {
            var enumerator = taskGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)enumerator.Current.Key);
                if (missionItem == null || missionItem.TaskType != MissionTable.eTaskType.TT_AWAKEN)
                {
                    continue;
                }
                if (missionItem.JobID != iJobID && missionItem.JobID != 0)
                {
                    continue;
                }

                SingleMissionInfo singleMissionInfo = null;
                if (!taskGroup.TryGetValue(enumerator.Current.Key, out singleMissionInfo))
                {
                    continue;
                }

                sendCmdAcceptTask((uint)missionItem.ID, (TaskSubmitType)missionItem.AcceptType, (uint)missionItem.MissionTakeNpc);
            }
        }

        public bool IsChangeJobMainMission(int iTaskId, ref SingleMissionInfo missioninfo)
        {
            if(iTaskId != 2271)
            {
                return false;
            }

            if (taskGroup == null || taskGroup.Count <= 0)
            {
                return false;
            }

            SingleMissionInfo singleMissionInfo = null;
            if (!taskGroup.TryGetValue((uint)iTaskId, out singleMissionInfo))
            {
                return false;
            }

            missioninfo = singleMissionInfo;

            if (singleMissionInfo.status <= (int)TaskStatus.TASK_INIT)
            {
                return false;
            }

            return true;
        }

        public bool HasAcceptedChangeJobMainMission()
        {
            if (taskGroup == null || taskGroup.Count <= 0)
            {
                return false;
            }

            var enumerator = taskGroup.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)enumerator.Current.Key);
                if (missionItem == null)
                {
                    continue;
                }

                if(missionItem.ID != 2271)
                {
                    continue;
                }

                SingleMissionInfo singleMissionInfo = null;
                if (!taskGroup.TryGetValue((uint)missionItem.ID, out singleMissionInfo))
                {
                    return false;
                }

                if (singleMissionInfo.status != (int)TaskStatus.TASK_INIT && singleMissionInfo.status != (int)TaskStatus.TASK_OVER)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAcceptedChangeJobMission()
        {
            if (taskGroup == null || taskGroup.Count <= 0)
            {
                return true;
            }

            var enumerator = taskGroup.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)enumerator.Current.Key);
                if (missionItem == null || missionItem.TaskType != MissionTable.eTaskType.TT_CHANGEJOB)
                {
                    continue;
                }

                SingleMissionInfo singleMissionInfo = null;
                if (!taskGroup.TryGetValue(enumerator.Current.Key, out singleMissionInfo))
                {
                    continue;
                }

                if (singleMissionInfo.status == (byte)TaskStatus.TASK_OVER)
                {
                    continue;
                }

                if (singleMissionInfo.status != (int)TaskStatus.TASK_INIT)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasAcceptedAwakeMission()
        {
            if(taskGroup == null || taskGroup.Count <= 0)
            {
                return true;
            }

            var enumerator = taskGroup.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)enumerator.Current.Key);
                if (missionItem == null || missionItem.TaskType != MissionTable.eTaskType.TT_AWAKEN)
                {
                    continue;
                }

                SingleMissionInfo singleMissionInfo = null;
                if (!taskGroup.TryGetValue(enumerator.Current.Key, out singleMissionInfo))
                {
                    continue;
                }                

                if (singleMissionInfo.status != (int)TaskStatus.TASK_INIT)
                {
                    return true;
                }
            }

            return false;
        }

        // 只在提交完成任务返回的回调里有用，判断觉醒任务有没有完成，不能只靠这个接口判断，客户端是不保存已完成的任务的。by wang bo
        public bool IsFinishingAwakeMission(int iTaskId)
        {
            if (taskGroup == null || taskGroup.Count <= 0)
            {
                return false;
            }

            var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>(iTaskId);
            if(missionItem == null)
            {
                return false;
            }

            if (missionItem.TaskType != MissionTable.eTaskType.TT_AWAKEN)
            {
                return false;
            }

            if(missionItem.AfterID > 0)
            {
                return false;
            }

            return true;
        }

        public void OnSceneLoadEnd()
        {
            bLoadingScene = false;
            if (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown)
            {
                _TryOpenFunctionFrame();

                var enumerator = taskGroup.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    TaskNpcAccess.AddMissionListener(enumerator.Current.Key);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementComplete);
        }

		public bool TryOpenTaskGuideInBattle()
		{
			DungeonID id = new DungeonID(BattleDataManager.GetInstance().BattleInfo.dungeonId);
			return _TryOpenTaskGuideFrame(GetMainTask(id.dungeonIDWithOutDiff), id.dungeonID,true);
		}

        public void OnSceneLoadBegin()
        {
            bLoadingScene = true;
        }

        public void Update()
        {
            if (!bLoadingScene)
            {
                _UpdateExecuteNetMsg();
            }
        }

        public override void Clear()
        {
            UnRegisterNetHandler();
            mListCnt = 0;
            taskSet.Clear();
            m_akDiffTasks.Clear();
            CloseAllDialog();
            cachedAutoAcceptTask.Clear();
            dungenStart = false;
            m_akCachedNetMsg.Clear();
            m_akType2MissionItems.Clear();
            iLockedMissionID = 0;
            dicLegendNotifies.Clear();
            //onAddNewMission = null;
            //onUpdateMission = null;
            //onDeleteMission = null;
            //missionChangedDelegate = null;
            ClientSystemManager.GetInstance().OnSwitchSystemBegin.RemoveListener(OnSceneLoadBegin);
            ClientSystemManager.GetInstance().OnSwitchSystemFinished.RemoveListener(OnSceneLoadEnd);
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            PlayerBaseData.GetInstance().onMissionScoreChanged -= OnMissionScoreChanged;

            m_akMissionScoreDatas.Clear();
            m_iScore = 0;
            m_akAcquiredChestIDs.Clear();

            finisedTaskIDs = null;
        }

        public class OnTriggerLoginEnd
        {
            MessageEvents msgEvent;
            public OnTriggerLoginEnd(MessageEvents msgEvent)
            {
                this.msgEvent = msgEvent;
            }

            public void OnTrigger()
            {
                ClientSystemManager.GetInstance().OnSwitchSystemFinished.RemoveListener(OnTrigger);
            }
        }

        public void OnExecuteAcceptTask(int iTaskID,bool bNeedDialog = true,
                        UnityEngine.Events.UnityAction onSuccessed = null,
            UnityEngine.Events.UnityAction onFailed = null,
            bool bForceSubmit = false)
        {
            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            if (missionItem == null)
            {
                if(onFailed != null)
                {
                    onFailed.Invoke();
                }
                return;
            }

            if(taskGroup.ContainsKey((uint)iTaskID))
            {
                var value = taskGroup[(uint)iTaskID];
                if(value == null || value.status != (int)Protocol.TaskStatus.TASK_INIT)
                {
                    if (onFailed != null)
                    {
                        onFailed.Invoke();
                    }
                    return;
                }
            }

            if (missionItem.AcceptType != ProtoTable.MissionTable.eAcceptType.ACT_NPC || bForceSubmit)
            {
                ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(missionItem.BefTaskDlgID);
                if (!bNeedDialog || talkItem == null)
                {
                    sendCmdAcceptTask((uint)iTaskID, (TaskSubmitType)missionItem.AcceptType, (UInt32)missionItem.MissionFinishNpc);
                }
                else
                {
                    if (bForceSubmit)
                    {
                        var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                        if (null != systemTown)
                        {
                            systemTown.PlayNpcSound((int)missionItem.MissionTakeNpc, NpcVoiceComponent.SoundEffectType.SET_Start);
                        }
                    }

                    TaskDialogFrame.OnDialogOver onTaskOverCallback = null;
                    if (bForceSubmit)
                    {
                        onTaskOverCallback = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                        {
                            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                            if (null != systemTown)
                            {
                                systemTown.PlayNpcSound((int)missionItem.MissionTakeNpc, NpcVoiceComponent.SoundEffectType.SET_End);
                            }
                        });
                    }
                    CloseAllDialog();
                    CreateDialogFrame(missionItem.BefTaskDlgID, iTaskID,onTaskOverCallback);
                }

                if (onSuccessed != null)
                {
                    onSuccessed.Invoke();
                }
                return;
            }
            else
            {
                ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(missionItem.MissionTakeNpc);
                if (npcItem != null)
                {
                    Parser.NpcParser.OnClickLink(missionItem.MissionTakeNpc, iTaskID, bNeedDialog,onSuccessed,onFailed);
                    return;
                }
                else
                {
                    Logger.LogErrorFormat("npcId is wrong whick taskID = {0},npcID = {1}", iTaskID, missionItem.MissionTakeNpc);
                }
            }

            if (onFailed != null)
            {
                onFailed.Invoke();
            }
        }

        public void OnExecuteInMissionDialog(uint iTaskID)
        {
            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)iTaskID);
            if (missionItem == null)
            {
                return;
            }

            SingleMissionInfo singleMissionInfo = null;
            if(!taskGroup.TryGetValue(iTaskID,out singleMissionInfo))
            {
                return;
            }

            if(singleMissionInfo.status != (int)Protocol.TaskStatus.TASK_UNFINISH)
            {
                return;
            }

            Int32 iDlgID = 0;
            ProtoTable.TalkTable dlgItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(iDlgID);
            if(dlgItem == null)
            {
                return;
            }

            CloseAllDialog();
            CreateDialogFrame(iDlgID, (int)iTaskID);
        }

        public bool OnExecuteSubmitTask(int iSelectedID,
             UnityEngine.Events.UnityAction onSuccessed = null,
            UnityEngine.Events.UnityAction onFailed = null,
            bool bForceSubmit = false)
        {
            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iSelectedID);
            if (missionItem == null)
            {
                if(onFailed != null)
                {
                    onFailed.Invoke();
                }
                //ExceptionManager.GetInstance().RecordLog(string.Format("missionItem = null with iSelectedID = {0}", iSelectedID));
                return false;
            }

            SingleMissionInfo singleMissionInfo;
            if (!taskGroup.TryGetValue((uint)iSelectedID, out singleMissionInfo))
            {
                if (onFailed != null)
                {
                    onFailed.Invoke();
                }
                //ExceptionManager.GetInstance().RecordLog(string.Format("mission is not existed in dictionary with mission.ID = {0}", missionItem.ID));
                return false;
            }

            if (singleMissionInfo.status != (int)Protocol.TaskStatus.TASK_FINISHED)
            {
                if (onFailed != null)
                {
                    onFailed.Invoke();
                }
                return false;
            }

            if (missionItem.FinishType != ProtoTable.MissionTable.eFinishType.FINISH_TYPE_NPC ||
                bForceSubmit)
            {
                ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(missionItem.AftTaskDlgID);
                if (talkItem == null)
                {
                    OpenAwardFrame((uint)iSelectedID);
                    if(onSuccessed != null)
                    {
                        onSuccessed.Invoke();
                    }
                    return false;
                }
                else
                {
                    TaskDialogFrame.OnDialogOver onTaskOverCallback = null;
                    if (bForceSubmit)
                    {
                        if (true)
                        {
                            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                            if (null != systemTown)
                            {
                                systemTown.PlayNpcSound((int)missionItem.MissionFinishNpc, NpcVoiceComponent.SoundEffectType.SET_Start);
                            }
                        }
                        onTaskOverCallback = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                        {
                            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                            if (null != systemTown)
                            {
                                systemTown.PlayNpcSound((int)missionItem.MissionFinishNpc, NpcVoiceComponent.SoundEffectType.SET_End);
                            }
                        });
                    }

                    CloseAllDialog();
                    CreateDialogFrame(missionItem.AftTaskDlgID, iSelectedID,onTaskOverCallback);
                    if (onSuccessed != null)
                    {
                        onSuccessed.Invoke();
                    }
                    return false;
                }
            }
            else
            {
                ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(missionItem.MissionFinishNpc);
                if (npcItem != null)
                {
                    Parser.NpcParser.OnClickLink(missionItem.MissionFinishNpc, missionItem.ID,true,onSuccessed,onFailed);
                    return true;
                }
                else
                {
                    Logger.LogErrorFormat("[Mission] [id = {0}] [npcID = {1}] npcId is wrong!", missionItem.ID, missionItem.MissionFinishNpc);
                    //ExceptionManager.GetInstance().RecordLog(string.Format("[Mission] [id = {0}] [npcID = {1}] npcId is wrong!", missionItem.ID, missionItem.MissionFinishNpc));
                }
            }

            if (onFailed != null)
            {
                onFailed.Invoke();
            }

            return false;
        }

        public void OnExecuteDungenTrace(Int32 iTaskID,
            UnityEngine.Events.UnityAction onSuccessed = null,
            UnityEngine.Events.UnityAction onFailed = null)
        {
            GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            ProtoTable.MissionTable mission = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            if(mission == null)
            {
                if(onFailed != null)
                {
                    onFailed.Invoke();
                }
                return;
            }

            ProtoTable.DungeonTable dungeon = TableManager.GetInstance().GetTableItem<ProtoTable.DungeonTable>(mission.MapID);
            if (dungeon != null && clientSystem != null && clientSystem.MainPlayer != null)
            {
                BeTownPlayerMain.CommandStopAutoMove();
                TaskTrace.DungenTrace taskTrace = new TaskTrace.DungenTrace();
                taskTrace.iTaskID = iTaskID;
                taskTrace.iDungenID = mission.MapID;
                taskTrace.onSucceed = onSuccessed;
                taskTrace.onFailed = onFailed;
                BeTownPlayerMain.OnMoveStateChanged.AddListener(taskTrace.OnMoveStateChanged);
                BeTownPlayerMain.OnAutoMoveSuccess.AddListener(taskTrace.OnMoveSuccess);
                BeTownPlayerMain.OnAutoMoveFail.AddListener(taskTrace.OnAutoMoveFail);
                clientSystem.MainPlayer.CommandMoveToDungeon(mission.MapID);
                return;
            }

            if (onFailed != null)
            {
                onFailed.Invoke();
            }
        }

        #region OnMissionDelegate
        public delegate void OnMissionChanged();
        public OnMissionChanged missionChangedDelegate;
        public void OnTaskChanged()
        {
            if (missionChangedDelegate != null)
            {
                missionChangedDelegate();
            }
        }

        public delegate void DelegateAddNewMission(UInt32 taskID);
        public DelegateAddNewMission onAddNewMission;
        public void OnAddNewMission(UInt32 taskID)
        {
            if(onAddNewMission != null)
            {
                onAddNewMission(taskID);
            }

            _TryOpenFunctionFrame();
            _TryOpenTaskGuideFrame((int)taskID);
            TaskNpcAccess.AddMissionListener(taskID);
        }

        public delegate void DelegateDeleteMission(UInt32 taskID);
        public DelegateDeleteMission onDeleteMission;

        public delegate void OnDeleteMissionValue(SingleMissionInfo value);
        public OnDeleteMissionValue onDeleteMissionValue;

        public void OnDeleteMission(UInt32 taskID)
        {
            if (onDeleteMission != null)
            {
                onDeleteMission(taskID);
            }
            _TryOpenFunctionFrame();
        }

        public void OpenAwardFrame(UInt32 taskID)
        {
            var awards = GetMissionAwards((int)taskID);
            if(awards != null)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<TaskAward>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<TaskAward>();
                }

                TaskAward.TaskAwardData data = new TaskAward.TaskAwardData();
                data.iID = (int)taskID;
                data.awards = awards;

                ClientSystemManager.GetInstance().OpenFrame<TaskAward>(FrameLayer.Top, data);
            }
        }

        public bool CanOpenDlgFrame()
        {
            return !(ClientSystemManager.GetInstance().CurrentSystem is ClientSystemBattle && dungenStart);
        }

        public delegate void DelegateSyncMission(UInt32 taskID);
        public DelegateSyncMission onSyncMission;
        public void OnSyncMission(UInt32 taskID)
        {
            if (onSyncMission != null)
            {
                onSyncMission(taskID);
            }
        }

        public delegate void DelegateUpdateMission(UInt32 taskID);
        public DelegateUpdateMission onUpdateMission;
        public void OnUpdateMission(UInt32 taskID)
        {
            if(onUpdateMission != null)
            {
                onUpdateMission(taskID);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MissionUpdated, taskID);

            _TryOpenFunctionFrame();
        }
        #endregion

        void _CachedNetMsg(UInt32 msgID, MsgDATA msgData)
        {
            m_akCachedNetMsg.Enqueue(new CachedMsg(msgID, msgData));
        }

        public Int32 GetAchievementMissionStatusCount(int iStatus)
        {
            Int32 iCount = 0;
            var enumerator = taskGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if(enumerator.Current.Value.status == iStatus && Utility.IsAchievementMissionNormal(enumerator.Current.Key))
                {
                    ++iCount;
                }
            }
            return iCount;
        }

        public Int32 GetTitleMissionStatusCount(int iStatus)
        {
            Int32 iCount = 0;
            var enumerator = taskGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.status != iStatus)
                {
                    continue;
                }

                var mission = enumerator.Current.Value.missionItem;
                if (null == mission)
                {
                    continue;
                }

                if(mission.TaskType != MissionTable.eTaskType.TT_TITLE)
                {
                    continue;
                }

                ++iCount;
            }
            return iCount;
        }

        public Int32 GetMainMissionStatusCount(int iStatus)
        {
            Int32 iCount = 0;
            var enumerator = taskGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.status != iStatus)
                {
                    continue;
                }

                var mission = enumerator.Current.Value.missionItem;
                if (null == mission)
                {
                    continue;
                }

                if (mission.TaskType != MissionTable.eTaskType.TT_MAIN &&
                    mission.TaskType != MissionTable.eTaskType.TT_BRANCH &&
                    mission.TaskType != MissionTable.eTaskType.TT_CYCLE)
                {
                    continue;
                }

                ++iCount;
            }
            return iCount;
        }

        public Int32 GetDailyNormalMissionStatusCount(int iStatus)
        {
            Int32 iCount = 0;
            var enumerator = taskGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.status == iStatus && Utility.IsDailyNormal(enumerator.Current.Key))
                {
                    ++iCount;
                }
            }
            return iCount;
        }

        public bool IsAcceptMission(UInt32 iTaskID)
        {
            var enumerator = taskGroup.GetEnumerator();

            while (enumerator.MoveNext())
            {
                SingleMissionInfo singleMissionInfo = null;
                if (!taskGroup.TryGetValue(enumerator.Current.Key, out singleMissionInfo))
                {
                    continue;
                }

                if(singleMissionInfo.taskID != iTaskID)
                {
                    continue;
                }

                if(singleMissionInfo.status > (byte)TaskStatus.TASK_UNFINISH)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        void _UpdateExecuteNetMsg()
        {
            if(bLoadingScene)
            {
                return;
            }

            while(m_akCachedNetMsg.Count > 0)
            {
                var cachedMsg = m_akCachedNetMsg.Dequeue();
                if (cachedMsg.id == SceneTaskListRet.MsgID)
                {
                    OnRecvTaskListCached(cachedMsg.msgData);
                }
                else if (cachedMsg.id == SceneNotifyNewTaskRet.MsgID)
                {
                    OnRecvNotifyNewTaskCached(cachedMsg.msgData);
                }
                else if (cachedMsg.id == SceneNotifyDeleteTaskRet.MsgID)
                {
                    OnRecvNotifyDeleteTaskCached(cachedMsg.msgData);
                }
                else if (cachedMsg.id == SceneNotifyTaskStatusRet.MsgID)
                {
                    OnRecvNotifyTaskStatusCached(cachedMsg.msgData);
                }
                else if (cachedMsg.id == SceneNotifyTaskVarRet.MsgID)
                {
                    OnRecvNotifyTaskVarCached(cachedMsg.msgData);
                }
                else if(cachedMsg.id == SceneDailyTaskList.MsgID)
                {
                    OnRecvDailyTaskListCached(cachedMsg.msgData);
                }
                else if(cachedMsg.id == SceneAchievementTaskList.MsgID)
                {
                    OnRecvAchievementTaskListCached(cachedMsg.msgData);
                }
                else if(cachedMsg.id == SceneLegendTaskListRes.MsgID)
                {
                    OnRecvLegendTaskListCached(cachedMsg.msgData);
                }
                else if (cachedMsg.id == SceneResetTaskSync.MsgID)
                {
                    OnReceivedSceneTaskSyncCached(cachedMsg.msgData);
                }
            }
        }

        bool bTaskDirty = false;

        #region OnRecvMissionMessage
        #region SceneDailyTaskList
        void _ClearAllDailyTask()
        {
            //清除所有日常
            var enumerator = taskGroup.GetEnumerator();
            List<UInt32> akRemoveKey = new List<uint>();
            akRemoveKey.Clear();
            while (enumerator.MoveNext())
            {
                if (Utility.IsDailyMission(enumerator.Current.Key))
                {
                    akRemoveKey.Add(enumerator.Current.Key);
                }
            }

            for(int i = 0; i < akRemoveKey.Count; ++i)
            {
                SingleMissionInfo current = null;
                if(taskGroup.ContainsKey(akRemoveKey[i]))
                {
                    current = taskGroup[akRemoveKey[i]];
                }
                taskGroup.Remove(akRemoveKey[i]);
                if (onDeleteMissionValue != null && current != null)
                {
                    onDeleteMissionValue.Invoke(current);
                }
                if (onDeleteMission != null)
                {
                    onDeleteMission(akRemoveKey[i]);
                }
            }
        }

        void _OnDailyAddOrUpdate(ref SceneDailyTaskList ret, UInt32[] arrayRemoveKeys)
        {
            for (int i = 0; i < ret.tasks.Length; ++i)
            {
                var current = ret.tasks[i];

                Int32 iFind = Array.BinarySearch(arrayRemoveKeys, current.taskID);
                if (iFind >= 0 && iFind < arrayRemoveKeys.Length)
                {
                    SingleMissionInfo kSingleMissionInfo = null;
                    if (taskGroup.TryGetValue(current.taskID, out kSingleMissionInfo))
                    {
                        kSingleMissionInfo.taskID = current.taskID;
                        kSingleMissionInfo.status = current.status;
                        kSingleMissionInfo.finTime = current.finTime;
                        kSingleMissionInfo.submitCount = current.submitCount;
                        kSingleMissionInfo.taskContents.Clear();
                        var taskContents = kSingleMissionInfo.taskContents;

                        for (int j = 0; j < current.akMissionPairs.Length; ++j)
                        {
                            var pairs = current.akMissionPairs[j];
                            
                            if (taskContents.ContainsKey(pairs.key))
                            {
                                taskContents.Remove(pairs.key);
                            }
                            taskContents.Add(pairs.key, pairs.value);
                        }

                        if (onUpdateMission != null)
                        {
                            onUpdateMission(current.taskID);
                        }
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MissionUpdated, current.taskID);
                    }
                }
                else
                {
                    SingleMissionInfo kSingleMissionInfo = new SingleMissionInfo();
                    kSingleMissionInfo.taskID = current.taskID;
                    kSingleMissionInfo.status = current.status;
                    kSingleMissionInfo.finTime = current.finTime;
                    kSingleMissionInfo.submitCount = current.submitCount;
                    kSingleMissionInfo.taskContents.Clear();
                    kSingleMissionInfo.missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)current.taskID);
                    var taskContents = kSingleMissionInfo.taskContents;

                    for (int j = 0; j < current.akMissionPairs.Length; ++j)
                    {
                        var pairs = current.akMissionPairs[j];

                        if (taskContents.ContainsKey(pairs.key))
                        {
                            taskContents.Remove(pairs.key);
                        }
                        taskContents.Add(pairs.key, pairs.value);
                    }

                    if(taskGroup.ContainsKey(current.taskID))
                    {
                        taskGroup.Remove(current.taskID);
                        Logger.LogErrorFormat("current.taskID = {0}", current.taskID);
                    }
                    taskGroup.Add(current.taskID, kSingleMissionInfo);

                    _OnAddDiffTask(kSingleMissionInfo);

                    if (onAddNewMission != null)
                    {
                        onAddNewMission(current.taskID);
                    }
                }
            }
        }

        public class DailyComparser : IComparer<MissionInfo>
        {
            public int Compare(MissionInfo x, MissionInfo y)
            {
                if (x.taskID == y.taskID)
                {
                    return 0;
                }

                if (x.taskID < y.taskID)
                {
                    return -1;
                }

                return 1;
            }
        }


        void _OnDailyDelete(ref SceneDailyTaskList ret, UInt32[] arrayRemoveKeys)
        {
            MissionInfo temp = new MissionInfo();
            temp.taskID = 0;
            DailyComparser iDailyComparser = new DailyComparser();

            for (int i = 0; i < arrayRemoveKeys.Length; ++i)
            {
                temp.taskID = arrayRemoveKeys[i];
                var currentFind = Array.BinarySearch(ret.tasks, temp, iDailyComparser);
                if (currentFind < 0 || currentFind >= ret.tasks.Length)
                {
                    _OnRemoveDiffTask((int)temp.taskID, (int)temp.taskID);
                    SingleMissionInfo current = null;
                    if (taskGroup.ContainsKey(arrayRemoveKeys[i]))
                    {
                        current = taskGroup[arrayRemoveKeys[i]];
                    }
                    taskGroup.Remove(arrayRemoveKeys[i]);
                    if (onDeleteMissionValue != null && current != null)
                    {
                        onDeleteMissionValue.Invoke(current);
                    }
                    if (onDeleteMission != null)
                    {
                        onDeleteMission(arrayRemoveKeys[i]);
                    }
                }
            }
        }

        class SortMissionInfo : IComparer<MissionInfo>
        {
            public int Compare(MissionInfo x, MissionInfo y)
            {
                if (x.taskID < y.taskID) return -1;
                if (x.taskID > y.taskID) return 1;
                return 0;
            }
        }
        SortMissionInfo sortMissionInfoCmp = new SortMissionInfo();


        void OnRecvDailyTaskListCached(MsgDATA msg)
        {
            SceneDailyTaskList ret = new SceneDailyTaskList();
            ret.decode(msg.bytes);

            Logger.LogWarningFormat("OnRecvDailyTaskListCached !!!");

            if (ret.tasks == null || ret.tasks.Length <= 0)
            {
                _ClearAllDailyTask();
                return;
            }

            Array.Sort(ret.tasks, sortMissionInfoCmp.Compare);
            List<UInt32> akRemoveKeys = new List<uint>();
            var enumerator = taskGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (Utility.IsDailyMission(enumerator.Current.Key))
                {
                    akRemoveKeys.Add(enumerator.Current.Key);
                }
            }
            UInt32[] arrayRemoveKeys = akRemoveKeys.ToArray();
            Array.Sort(arrayRemoveKeys);

            _OnDailyAddOrUpdate(ref ret, arrayRemoveKeys);
            _OnDailyDelete(ref ret, arrayRemoveKeys);
        }

        //[EnterGameMessageHandle(SceneDailyTaskList.MsgID)]
        //[MessageHandle(SceneDailyTaskList.MsgID)]
        void OnRecvDailyTaskList(MsgDATA msg)
        {
            _CachedNetMsg(SceneDailyTaskList.MsgID, msg);
        }
        #endregion

        #region SceneAchievementTaskList
        void _ClearAllAchievementTask()
        {
            //清除所有日常
            var enumerator = taskGroup.GetEnumerator();
            List<UInt32> akRemoveKey = new List<UInt32>();
            akRemoveKey.Clear();
            while (enumerator.MoveNext())
            {
                if (Utility.IsAchievementMission(enumerator.Current.Key))
                {
                    akRemoveKey.Add(enumerator.Current.Key);
                }
            }

            for(int i = 0; i < akRemoveKey.Count; ++i)
            {
                SingleMissionInfo current = null;
                if (taskGroup.ContainsKey(akRemoveKey[i]))
                {
                    current = taskGroup[akRemoveKey[i]];
                }
                taskGroup.Remove(akRemoveKey[i]);
                if (onDeleteMissionValue != null && current != null)
                {
                    onDeleteMissionValue.Invoke(current);
                }
                if (onDeleteMission != null)
                {
                    onDeleteMission(akRemoveKey[i]);
                }
            }
        }
        void _OnAchievementAddOrUpdate(ref SceneAchievementTaskList ret, UInt32[] arrayRemoveKeys)
        {
            for (int i = 0; i < ret.tasks.Length; ++i)
            {
                var current = ret.tasks[i];
				var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)current.taskID);
				if(missionItem == null)
				{
					continue;
				}

                Int32 iFind = Array.BinarySearch(arrayRemoveKeys, current.taskID);
                if (iFind >= 0 && iFind < arrayRemoveKeys.Length)
                {
                    SingleMissionInfo kSingleMissionInfo = null;
                    if (taskGroup.TryGetValue(current.taskID, out kSingleMissionInfo))
                    {
                        kSingleMissionInfo.taskID = current.taskID;
                        kSingleMissionInfo.status = current.status;
                        kSingleMissionInfo.finTime = current.finTime;
                        kSingleMissionInfo.submitCount = current.submitCount;
                        kSingleMissionInfo.taskContents.Clear();
                        var taskContents = kSingleMissionInfo.taskContents;

                        for (int j = 0; j < current.akMissionPairs.Length; ++j)
                        {
                            var pairs = current.akMissionPairs[j];

                            if (taskContents.ContainsKey(pairs.key))
                            {
                                taskContents.Remove(pairs.key);
                            }
                            taskContents.Add(pairs.key, pairs.value);
                        }

                        if (onUpdateMission != null)
                        {
                            onUpdateMission(current.taskID);
                        }
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MissionUpdated, current.taskID);
                    }
                }
                else
                {
                    SingleMissionInfo kSingleMissionInfo = new SingleMissionInfo();
                    kSingleMissionInfo.taskID = current.taskID;
                    kSingleMissionInfo.status = current.status;
                    kSingleMissionInfo.finTime = current.finTime;
                    kSingleMissionInfo.submitCount = current.submitCount;
                    kSingleMissionInfo.taskContents.Clear();
					kSingleMissionInfo.missionItem = missionItem;
                    var taskContents = kSingleMissionInfo.taskContents;

                    for (int j = 0; j < current.akMissionPairs.Length; ++j)
                    {
                        var pairs = current.akMissionPairs[j];

                        if (taskContents.ContainsKey(pairs.key))
                        {
                            taskContents.Remove(pairs.key);
                        }
                        taskContents.Add(pairs.key, pairs.value);
                    }

                    if (!taskGroup.ContainsKey(current.taskID))
                        taskGroup.Add(current.taskID, kSingleMissionInfo);

                    _OnAddDiffTask(kSingleMissionInfo);

                    if (onAddNewMission != null)
                    {
                        onAddNewMission(current.taskID);
                    }
                }

                RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.Institute);
            }
        }
        void _OnAchievementDelete(ref SceneAchievementTaskList ret, UInt32[] arrayRemoveKeys)
        {
            MissionInfo temp = new MissionInfo();
            temp.taskID = 0;
            DailyComparser iDailyComparser = new DailyComparser();

            for (int i = 0; i < arrayRemoveKeys.Length; ++i)
            {
                temp.taskID = arrayRemoveKeys[i];
                var currentFind = Array.BinarySearch(ret.tasks, temp, iDailyComparser);
                if (currentFind < 0 || currentFind >= ret.tasks.Length)
                {
                    _OnRemoveDiffTask((int)temp.taskID, (int)temp.taskID);
                    SingleMissionInfo current = null;
                    if (taskGroup.ContainsKey(arrayRemoveKeys[i]))
                    {
                        current = taskGroup[arrayRemoveKeys[i]];
                    }
                    taskGroup.Remove(arrayRemoveKeys[i]);
                    if (onDeleteMissionValue != null && current != null)
                    {
                        onDeleteMissionValue.Invoke(current);
                    }
                    if (onDeleteMission != null)
                    {
                        onDeleteMission(arrayRemoveKeys[i]);
                    }
                }
            }
        }
        void OnRecvAchievementTaskListCached(MsgDATA msg)
        {
            SceneAchievementTaskList ret = new SceneAchievementTaskList();
            ret.decode(msg.bytes);

            Logger.LogWarningFormat("OnRecvAchievementTaskListCached !!!");

            if (ret.tasks == null || ret.tasks.Length <= 0)
            {
                _ClearAllAchievementTask();
                return;
            }

            Array.Sort(ret.tasks, sortMissionInfoCmp.Compare);
            List<UInt32> akRemoveKeys = new List<uint>();
            var enumerator = taskGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (Utility.IsAchievementMission(enumerator.Current.Key))
                {
                    akRemoveKeys.Add(enumerator.Current.Key);
                }
            }
            UInt32[] arrayRemoveKeys = akRemoveKeys.ToArray();
            Array.Sort(arrayRemoveKeys);

            _OnAchievementAddOrUpdate(ref ret, arrayRemoveKeys);
            _OnAchievementDelete(ref ret, arrayRemoveKeys);
        }

        //[EnterGameMessageHandle(SceneAchievementTaskList.MsgID)]
        //[MessageHandle(SceneAchievementTaskList.MsgID)]
        void OnRecvAchievementTaskList(MsgDATA msg)
        {
            _CachedNetMsg(SceneAchievementTaskList.MsgID, msg);
        }
        #endregion

        #region LegendTaskList
        void _ClearAllLegendTask()
        {
            //清除所有日常
            var enumerator = taskGroup.GetEnumerator();
            List<UInt32> akRemoveKey = new List<UInt32>();
            akRemoveKey.Clear();
            while (enumerator.MoveNext())
            {
                if (Utility.IsLegendMission(enumerator.Current.Key))
                {
                    akRemoveKey.Add(enumerator.Current.Key);
                }
            }

            for (int i = 0; i < akRemoveKey.Count; ++i)
            {
                SingleMissionInfo current = null;
                if (taskGroup.ContainsKey(akRemoveKey[i]))
                {
                    current = taskGroup[akRemoveKey[i]];
                }
                taskGroup.Remove(akRemoveKey[i]);
                if (onDeleteMissionValue != null && current != null)
                {
                    onDeleteMissionValue.Invoke(current);
                }
                if (onDeleteMission != null)
                {
                    onDeleteMission(akRemoveKey[i]);
                }
            }
        }

        void _OnLegendAddOrUpdate(ref SceneLegendTaskListRes ret, UInt32[] arrayRemoveKeys)
        {

            for (int i = 0; i < ret.tasks.Length; ++i)
            {
                var current = ret.tasks[i];
                var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)current.taskID);
                if (missionItem == null)
                {
                    continue;
                }

                Int32 iFind = Array.BinarySearch(arrayRemoveKeys, current.taskID);
                if (iFind >= 0 && iFind < arrayRemoveKeys.Length)
                {
                    SingleMissionInfo kSingleMissionInfo = null;
                    if (taskGroup.TryGetValue(current.taskID, out kSingleMissionInfo))
                    {
                        kSingleMissionInfo.taskID = current.taskID;
                        kSingleMissionInfo.status = current.status;
                        kSingleMissionInfo.finTime = current.finTime;
                        kSingleMissionInfo.submitCount = current.submitCount;
                        kSingleMissionInfo.taskContents.Clear();
                        var taskContents = kSingleMissionInfo.taskContents;

                        for (int j = 0; j < current.akMissionPairs.Length; ++j)
                        {
                            var pairs = current.akMissionPairs[j];

                            if (taskContents.ContainsKey(pairs.key))
                            {
                                taskContents.Remove(pairs.key);
                            }
                            taskContents.Add(pairs.key, pairs.value);
                        }

                        if (onUpdateMission != null)
                        {
                            onUpdateMission(current.taskID);
                        }
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MissionUpdated, current.taskID);
                    }
                }
                else
                {
                    SingleMissionInfo kSingleMissionInfo = new SingleMissionInfo();
                    kSingleMissionInfo.taskID = current.taskID;
                    kSingleMissionInfo.status = current.status;
                    kSingleMissionInfo.finTime = current.finTime;
                    kSingleMissionInfo.submitCount = current.submitCount;
                    kSingleMissionInfo.taskContents.Clear();
                    kSingleMissionInfo.missionItem = missionItem;
                    var taskContents = kSingleMissionInfo.taskContents;

                    for (int j = 0; j < current.akMissionPairs.Length; ++j)
                    {
                        var pairs = current.akMissionPairs[j];

                        if (taskContents.ContainsKey(pairs.key))
                        {
                            taskContents.Remove(pairs.key);
                        }
                        taskContents.Add(pairs.key, pairs.value);
                    }

                    taskGroup.Add(current.taskID, kSingleMissionInfo);
                    _OnAddDiffTask(kSingleMissionInfo);

                    if (onAddNewMission != null)
                    {
                        onAddNewMission(current.taskID);
                    }
                }
            }
        }

        void _OnLegendDelete(ref SceneLegendTaskListRes ret, UInt32[] arrayRemoveKeys)
        {
            MissionInfo temp = new MissionInfo();
            temp.taskID = 0;
            DailyComparser iDailyComparser = new DailyComparser();

            for (int i = 0; i < arrayRemoveKeys.Length; ++i)
            {
                temp.taskID = arrayRemoveKeys[i];
                var currentFind = Array.BinarySearch(ret.tasks, temp, iDailyComparser);
                if (currentFind < 0 || currentFind >= ret.tasks.Length)
                {
                    _OnRemoveDiffTask((int)temp.taskID, (int)temp.taskID);
                    SingleMissionInfo current = null;
                    if (taskGroup.ContainsKey(arrayRemoveKeys[i]))
                    {
                        current = taskGroup[arrayRemoveKeys[i]];
                    }
                    taskGroup.Remove(arrayRemoveKeys[i]);
                    if (onDeleteMissionValue != null && current != null)
                    {
                        onDeleteMissionValue.Invoke(current);
                    }
                    if (onDeleteMission != null)
                    {
                        onDeleteMission(arrayRemoveKeys[i]);
                    }
                }
            }
        }

        //任务重置的同步
        private void OnReceivedSceneTaskSyncCached(MsgDATA msg)
        {
            var ret = new SceneResetTaskSync();
            ret.decode(msg.bytes);

            if (ret.taskInfo == null)
            {
                return;
            }

            var taskInfo = ret.taskInfo;
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int) taskInfo.taskID);

            if (missionItem == null)
            {
                return;
            }

            SingleMissionInfo kSingleMissionInfo = null;
            if (taskGroup.TryGetValue(taskInfo.taskID, out kSingleMissionInfo))
            {
                kSingleMissionInfo.taskID = taskInfo.taskID;
                kSingleMissionInfo.missionItem = missionItem;
                kSingleMissionInfo.status = taskInfo.status;
                kSingleMissionInfo.finTime = taskInfo.finTime;
                kSingleMissionInfo.submitCount = taskInfo.submitCount;
                kSingleMissionInfo.taskContents.Clear();

                var taskContents = kSingleMissionInfo.taskContents;

                for (var j = 0; j < taskInfo.akMissionPairs.Length; j++)
                {
                    var pairs = taskInfo.akMissionPairs[j];
                    if (taskContents.ContainsKey(pairs.key))
                    {
                        taskContents.Remove(pairs.key);
                    }
                    taskContents.Add(pairs.key, pairs.value);
                }
            }
            else
            {
                kSingleMissionInfo = new SingleMissionInfo();

                kSingleMissionInfo.taskID = taskInfo.taskID;
                kSingleMissionInfo.missionItem = missionItem;
                kSingleMissionInfo.status = taskInfo.status;
                kSingleMissionInfo.finTime = taskInfo.finTime;
                kSingleMissionInfo.submitCount = taskInfo.submitCount;
                kSingleMissionInfo.taskContents.Clear();

                var taskContents = kSingleMissionInfo.taskContents;

                for (var j = 0; j < taskInfo.akMissionPairs.Length; j++)
                {
                    var pairs = taskInfo.akMissionPairs[j];
                    if (taskContents.ContainsKey(pairs.key))
                    {
                        taskContents.Remove(pairs.key);
                    }
                    taskContents.Add(pairs.key, pairs.value);
                }
                taskGroup.Add(taskInfo.taskID, kSingleMissionInfo);
            }

            //同步数据
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MissionSync, taskInfo.taskID);
            OnSyncMission(taskInfo.taskID);
        }

        void OnRecvLegendTaskListCached(MsgDATA msg)
        {
            SceneLegendTaskListRes ret = new SceneLegendTaskListRes();
            ret.decode(msg.bytes);
            if (ret.tasks == null || ret.tasks.Length <= 0)
            {
                _ClearAllLegendTask();
                return;
            }


            Array.Sort(ret.tasks, sortMissionInfoCmp.Compare);
            List<UInt32> akRemoveKeys = new List<uint>();
            var enumerator = taskGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (Utility.IsLegendMission(enumerator.Current.Key))
                {
                    akRemoveKeys.Add(enumerator.Current.Key);
                }
            }
            UInt32[] arrayRemoveKeys = akRemoveKeys.ToArray();
            Array.Sort(arrayRemoveKeys);

            _OnLegendAddOrUpdate(ref ret, arrayRemoveKeys);
            _OnLegendDelete(ref ret, arrayRemoveKeys);
        }

        //[MessageHandle(SceneLegendTaskListRes.MsgID)]
        void OnRecvLegendTaskList(MsgDATA msg)
        {
            _CachedNetMsg(SceneLegendTaskListRes.MsgID, msg);
        }
        #endregion

        void _OnAddDiffTask(SingleMissionInfo value)
        {
			if(value != null && value.missionItem != null)
            {
                List<SingleMissionInfo> outValue = null;
                if (!m_akDiffTasks.TryGetValue(value.missionItem.TaskType, out outValue))
                {
                    outValue = new List<SingleMissionInfo>();
                    m_akDiffTasks.Add(value.missionItem.TaskType, outValue);
                }
                outValue.Add(value);
            }
        }

        public List<SingleMissionInfo> GetDiffTask(ProtoTable.MissionTable.eTaskType eType)
        {
            if(m_akDiffTasks.ContainsKey(eType) && m_akDiffTasks[eType].Count > 0)
            {
                return m_akDiffTasks[eType];
            }
            return null;
        }

        public List<SingleMissionInfo> GetParticularDiffTask(ProtoTable.MissionTable.eTaskType eType)
        {
            if(eType != MissionTable.eTaskType.TT_DIALY)
            {
                return GetDiffTask(eType);
            }

            var current = GetDiffTask(eType);
            if(current != null)
            {
                current.RemoveAll(x => { return x.missionItem.SubType != MissionTable.eSubType.Daily_Task; });
            }

            return current;
        }

        void _OnRemoveDiffTask(int iTaskID,int iTableID)
        {
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            if(missionItem != null)
            {
                List<SingleMissionInfo> outValue = null;
                if(m_akDiffTasks.TryGetValue(missionItem.TaskType,out outValue))
                {
                    if(missionItem.TaskType == MissionTable.eTaskType.TT_DIALY)
                    {
                        Logger.LogWarningFormat("remove daily!!!");
                        Logger.LogWarningFormat("remove daily!!!");
                        Logger.LogWarningFormat("remove daily!!!");
                    }
                    outValue.RemoveAll(x => { return x.taskID == iTaskID; });
                }
            }
        }

        #region SceneTaskListRet
        public void OnRecvTaskListCached(MsgDATA msg)
        {
            SceneTaskListRet ret = new SceneTaskListRet();
            ret.decode(msg.bytes);

            //Logger.LogWarningFormat("OnRecvTaskListCached !!!");

            for (int i = 0; i < ret.tasks.Length; ++i)
            {
                MissionInfo taskInfo = ret.tasks[i];
                var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)taskInfo.taskID);
                if(missionItem == null)
                {
                    Logger.LogErrorFormat("taskInfo.taskID = {0} can not find in MissionTable!", taskInfo.taskID);
                    continue;
                }

                SingleMissionInfo retValue;
                if (this.taskSet.TryGetValue(taskInfo.taskID, out retValue))
                {
                    retValue.status = taskInfo.status;
                    retValue.finTime = taskInfo.finTime;
                    retValue.submitCount = taskInfo.submitCount;
                    for (int j = 0; j < taskInfo.akMissionPairs.Length; ++j)
                    {
                        if (retValue.taskContents.ContainsKey(taskInfo.akMissionPairs[j].key))
                        {
                            retValue.taskContents.Remove(taskInfo.akMissionPairs[j].key);
                        }

                        retValue.taskContents.Add(taskInfo.akMissionPairs[j].key, taskInfo.akMissionPairs[j].value);
                    }
                }
                else
                {
                    retValue = new SingleMissionInfo();

                    retValue.taskID = taskInfo.taskID;
                    retValue.status = taskInfo.status;
                    retValue.finTime = taskInfo.finTime;
                    retValue.submitCount = taskInfo.submitCount;
                    retValue.missionItem = missionItem;
                    for (var j = 0; j < taskInfo.akMissionPairs.Length; ++j)
                    {
                        retValue.taskContents.Add(taskInfo.akMissionPairs[j].key, taskInfo.akMissionPairs[j].value);
                    }

                    taskSet.Add(retValue.taskID, retValue);
                }

                _OnAddDiffTask(retValue);

                Logger.LogWarningFormat("[Mission] you have received a new task  [id = {0}] [status = {1}]! from SceneTaskListRet", retValue.taskID, ((Protocol.TaskStatus)retValue.status).ToString());

                //auto accept
                _TryAcceptAutoTask(retValue.taskID);
                TaskNpcAccess.AddMissionListener(retValue.taskID);
                _TryOpenTaskGuideFrame((int)retValue.taskID);

                _OnAddTypeMission(retValue,false);
            }
            _SortAllTypeMission();
            _TryOpenFunctionFrame();

            OnTaskChanged();
        }

        //[EnterGameMessageHandle(SceneTaskListRet.MsgID)]
        //[MessageHandle(SceneTaskListRet.MsgID)]
        void OnRecvTaskList(MsgDATA msg)
        {
            _CachedNetMsg(SceneTaskListRet.MsgID, msg);
        }
        #endregion

        #region SceneNotifyNewTaskRet
        public void OnRecvNotifyNewTaskCached(MsgDATA msg)
        {
            SceneNotifyNewTaskRet ret = new SceneNotifyNewTaskRet();
            ret.decode(msg.bytes);

            Logger.LogWarningFormat("OnRecvNotifyNewTaskCached !!!");

            var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)ret.taskInfo.taskID);
            if(missionItem == null)
            {
                Logger.LogErrorFormat("OnRecvNotifyNewTaskCached taskid = {0} can not find in MissionTable!", ret.taskInfo.taskID);
                return;
            }

            SingleMissionInfo retValue;
            if (taskSet.TryGetValue(ret.taskInfo.taskID, out retValue))
            {
                retValue.taskID = ret.taskInfo.taskID;
                retValue.status = ret.taskInfo.status;
                retValue.finTime = ret.taskInfo.finTime;
                retValue.submitCount = ret.taskInfo.submitCount;

                retValue.taskContents.Clear();

             //   Logger.LogError("SceneNotifyNewTaskRet task has existed! taskid = " + ret.taskInfo.taskID);
            }
            else
            {
                retValue = new SingleMissionInfo();
                retValue.taskID = ret.taskInfo.taskID;
                retValue.status = ret.taskInfo.status;
                retValue.finTime = ret.taskInfo.finTime;
                retValue.submitCount = ret.taskInfo.submitCount;
                retValue.missionItem = missionItem;
                taskSet.Add(retValue.taskID, retValue);
            }

            _OnAddDiffTask(retValue);
            for (int i = 0; i < ret.taskInfo.akMissionPairs.Length; ++i)
            {
                retValue.taskContents.Add(ret.taskInfo.akMissionPairs[i].key, ret.taskInfo.akMissionPairs[i].value);
            }

            Logger.LogWarningFormat("[Mission] you have received a new task  [id = {0}] [status = {1}]!", retValue.taskID,((Protocol.TaskStatus)retValue.status).ToString());
            //auto accept
            if (retValue.status == (int)Protocol.TaskStatus.TASK_INIT)
            {
                _TryAcceptAutoTask(retValue.taskID);
            }
            else if (retValue.status == (int)Protocol.TaskStatus.TASK_UNFINISH)
            {
                if (!CanOpenDlgFrame())
                {
                    UInt32 iValue = 0;
                    SingleMissionInfo singleMissionInfo = null;
                    if (taskGroup.TryGetValue(retValue.taskID, out singleMissionInfo))
                    {
                        if (!cachedAutoAcceptTask.TryGetValue(retValue.taskID, out iValue))
                        {
                            cachedAutoAcceptTask.Add(retValue.taskID, retValue.status);
                        }
                    }
                }
                else
                {
                    CreateTaskDlgFrame((int)retValue.taskID, TaskDlgType.TDT_MIDDLE);
                }
            }
            else if(retValue.status == (int)Protocol.TaskStatus.TASK_FINISHED)
            {
                if (!CanOpenDlgFrame())
                {
                    UInt32 iValue = 0;
                    SingleMissionInfo singleMissionInfo = null;
                    if (taskGroup.TryGetValue(retValue.taskID, out singleMissionInfo))
                    {
                        if (!cachedAutoAcceptTask.TryGetValue(retValue.taskID, out iValue))
                        {
                            cachedAutoAcceptTask.Add(retValue.taskID, retValue.status + (uint)9527);
                        }
                    }
                }
                else
                {
                    CreateTaskDlgFrame((int)retValue.taskID, TaskDlgType.TDT_BEGIN);
                }
            }

            _OnAddTypeMission(retValue,true);
            OnAddNewMission(retValue.taskID);
        }
        //[MessageHandle(SceneNotifyNewTaskRet.MsgID)]
        void OnRecvNotifyNewTask(MsgDATA msg)
        {
            _CachedNetMsg(SceneNotifyNewTaskRet.MsgID, msg);
        }
        #endregion

        #region SceneNotifyDeleteTaskRet
        public void OnRecvNotifyDeleteTaskCached(MsgDATA msg)
        {
            SceneNotifyDeleteTaskRet ret = new SceneNotifyDeleteTaskRet();
            ret.decode(msg.bytes);

            OnNotifyDeleteTask(ret);
        }

        //[MessageHandle(SceneNotifyDeleteTaskRet.MsgID)]
        void OnRecvNotifyDeleteTask(MsgDATA msg)
        {
            _CachedNetMsg(SceneNotifyDeleteTaskRet.MsgID, msg);
        }

        void OnNotifyDeleteTask(SceneNotifyDeleteTaskRet ret)
        {
            if(ret == null)
            {
                return;
            }

            Logger.LogWarningFormat("OnRecvNotifyDeleteTaskCached !!!");

            Logger.LogWarningFormat("[Mission][id = {0}] finished !", ret.taskID);

            _TryUnbindNpcForMission(ret.taskID, (int)Protocol.TaskStatus.TASK_FINISHED);

            SingleMissionInfo outValue;
            if (taskSet.TryGetValue(ret.taskID, out outValue))
            {
                taskSet.Remove(ret.taskID);
            }
            if (onDeleteMissionValue != null && outValue != null)
            {
                onDeleteMissionValue.Invoke(outValue);
            }
            _OnRemoveTypeMission((int)ret.taskID);
            _OnRemoveDiffTask((int)ret.taskID, (int)ret.taskID);
            OnDeleteMission(ret.taskID);
        }

        #endregion

        #region SceneNotifyTaskStatusRet
        public void OnRecvNotifyTaskStatusCached(MsgDATA msg)
        {
            SceneNotifyTaskStatusRet ret = new SceneNotifyTaskStatusRet();
            ret.decode(msg.bytes);
            SingleMissionInfo retValue;
            if (taskSet.TryGetValue(ret.taskID, out retValue))
            {
                if (retValue.status != ret.status)
                {
                    _TryUnbindNpcForMission(retValue.taskID, retValue.status);
                    if (ret.status == (int)Protocol.TaskStatus.TASK_INIT)
                    {
                        retValue.taskContents.Clear();

                        Utility.OnPopupTaskChangedMsg("你放弃了任务", (int)ret.taskID);
                        
                        //AudioManager.instance.PlaySound(Utility.GetSoundPath(Utility.SoundKind.SK_ABANDON_TASK), AudioType.AudioEffect);
                    }
                    else if (ret.status == (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        if(taskGroup.ContainsKey(ret.taskID))
                        {
                            SingleMissionInfo missionValue = taskGroup[ret.taskID];
                            if(null != missionValue && null != missionValue.missionItem)
                            {
                                if(missionValue.missionItem.TaskType == MissionTable.eTaskType.TT_ACHIEVEMENT &&
                                    missionValue.missionItem.SubType == MissionTable.eSubType.Daily_Null)
                                {
                                    if (missionValue.status != ret.status)
                                    {
                                        _PushAchievementItems(missionValue.missionItem.ID);
                                        if(!bLoadingScene)
                                        {
                                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementComplete);
                                        }
                                    }
                                }
                            }
                        }

                        if (!CanOpenDlgFrame())
                        {
                            UInt32 iValue = 0;
                            SingleMissionInfo singleMissionInfo = null;
                            if (taskGroup.TryGetValue(ret.taskID, out singleMissionInfo))
                            {
                                if (!cachedAutoAcceptTask.TryGetValue(ret.taskID, out iValue))
                                {
                                    cachedAutoAcceptTask.Add(ret.taskID, ret.status);
                                }
                            }
                        }
                        else
                        {
                            CreateTaskDlgFrame((int)ret.taskID, TaskDlgType.TDT_END);
                        }
                    }
                    else if(ret.status == (int)Protocol.TaskStatus.TASK_OVER)
                    {
                        if(Utility.IsLegendMission(ret.taskID))
                        {
                            if(null != retValue && null != retValue.missionItem && !string.IsNullOrEmpty(retValue.missionItem.LinkInfo))
                            {
#if UNITY_EDITOR
                                //Logger.LogErrorFormat("LegendMision ID = {0} LinkInfo={1}", ret.taskID, retValue.missionItem.LinkInfo);
#endif
                                //ActiveManager.GetInstance().OnClickLinkInfo(retValue.missionItem.LinkInfo);
                            }
                        }
                        if (taskGroup.ContainsKey(ret.taskID))
                        {
                            SingleMissionInfo missionValue = taskGroup[ret.taskID];
                            if (null != missionValue && null != missionValue.missionItem)
                            {
                                if (missionValue.missionItem.TaskType == MissionTable.eTaskType.TT_ACHIEVEMENT &&
                                    missionValue.missionItem.SubType == MissionTable.eSubType.Daily_Null)
                                {
                                    if (missionValue.status != ret.status)
                                    {
                                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementOver, missionValue.missionItem.ID);
                                    }
                                }
                            }
                        }
                        //Utility.IsAchievementMission(ret.taskID) 
                        //Utility.IsDailyNormal(ret.taskID)||
                        //if (Utility.IsChangeJobMission(ret.taskID) || Utility.IsAwakeMission(ret.taskID))
                        //{
                        //    List<AwardItemData> awardItems = GetMissionAwards((int)ret.taskID);
                        //    if (awardItems.Count > 0)
                        //    {
                        //        for(int i = 0; i < awardItems.Count; i++)
                        //        {
                        //            ItemTable TableData = TableManager.GetInstance().GetTableItem<ItemTable>(awardItems[i].ID);
                        //            if (TableData == null)
                        //            {
                        //                continue;
                        //            }

                        //            string str = string.Format("{0} * {1}", TableData.Name, awardItems[i].Num);
                        //            SystemNotifyManager.SysNotifyFloatingEffect(str, CommonTipsDesc.eShowMode.SI_QUEUE, awardItems[i].ID);
                        //        }

                        //        //SystemNotifyManager.CreateSysNotifyCommonItemAwardFrame().SetAwardItems(awardItems);
                        //    }
                        //}
                    }

                    retValue.status = ret.status;
                    retValue.finTime = ret.finTime;
                    //必须放在值改变以后
                    TaskNpcAccess.AddMissionListener(retValue.taskID);
                }
            }

            OnUpdateMission(ret.taskID);

            if(ret.status == (byte)TaskStatus.TASK_OVER)
            {
                OnNotifyDeleteTask(new SceneNotifyDeleteTaskRet() { taskID = ret.taskID });
            }
        }
        //[MessageHandle(SceneNotifyTaskStatusRet.MsgID)]
        void OnRecvNotifyTaskStatus(MsgDATA msg)
        {
            _CachedNetMsg(SceneNotifyTaskStatusRet.MsgID, msg);
        }
        #endregion

        #region SceneNotifyTaskVarRet
        public void OnRecvNotifyTaskVarCached(MsgDATA msg)
        {
            SceneNotifyTaskVarRet ret = new SceneNotifyTaskVarRet();
            ret.decode(msg.bytes);

            SingleMissionInfo retValue;
            if (taskSet.TryGetValue(ret.taskID, out retValue))
            {
                if (retValue.taskContents.ContainsKey(ret.key))
                {
                    retValue.taskContents.Remove(ret.key);
                }
                retValue.taskContents.Add(ret.key, ret.value);
            }

            OnUpdateMission(ret.taskID);
        }
        //[MessageHandle(SceneNotifyTaskVarRet.MsgID)]
        void OnRecvNotifyTaskVar(MsgDATA msg)
        {
            _CachedNetMsg(SceneNotifyTaskVarRet.MsgID, msg);
        }

        private void OnReceiveTaskSync(MsgDATA msg)
        {
            _CachedNetMsg(SceneResetTaskSync.MsgID, msg);
        }

        #endregion
        #endregion

        #region OnSendMissionMessage
        public void sendCmdAcceptTask(System.UInt32 iTaskID, TaskSubmitType eSubmitType, System.UInt32 iNpcID)
        {
            SingleMissionInfo singleMissionInfo = null;
            if(taskGroup.TryGetValue(iTaskID,out singleMissionInfo) && singleMissionInfo.status == (Int32)Protocol.TaskStatus.TASK_INIT)
            {
                Logger.LogWarning("[Mission]sendCmdAcceptTask!");
                SceneAcceptTaskReq kCmd = new SceneAcceptTaskReq();
                kCmd.taskID = iTaskID;
                kCmd.npcID = iNpcID;
                kCmd.acceptType = (byte)(eSubmitType);

                NetManager.Instance().SendCommand<SceneAcceptTaskReq>(ServerType.GATE_SERVER, kCmd);

                GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_ACCEPT, (int)iTaskID);
            }


            Utility.OnPopupTaskChangedMsg("你接受了任务", (int)iTaskID);
            
            //AudioManager.instance.PlaySound(Utility.GetSoundPath(Utility.SoundKind.SK_ACCEPT_TASK), AudioType.AudioEffect);
        }

        public void sendCmdSubmitTask(System.UInt32 iTaskID, TaskSubmitType eSubmitType, System.UInt32 iNpcID)
        {
            SingleMissionInfo singleMissionInfo = null;
            if (taskGroup.TryGetValue(iTaskID, out singleMissionInfo) && singleMissionInfo.status == (Int32)Protocol.TaskStatus.TASK_FINISHED)
            {
                if(Utility.IsDailyMission(iTaskID))
                {
                    Logger.LogProcessFormat("[Mission]向服务器提交每日任务! ID = {0}", iTaskID);
                    SceneSubmitDailyTask kCmd = new SceneSubmitDailyTask();
                    kCmd.taskId = iTaskID;

                    NetManager.Instance().SendCommand<SceneSubmitDailyTask>(ServerType.GATE_SERVER, kCmd);
                    GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_FINISH, (int)iTaskID);
                }
                else if(Utility.IsAchievementMission(iTaskID))
                {
                    Logger.LogProcessFormat("[Mission]向服务器提交成就任务! ID = {0}", iTaskID);
                    SceneSubmitAchievementTask kCmd = new SceneSubmitAchievementTask();
                    kCmd.taskId = iTaskID;

                    NetManager.Instance().SendCommand<SceneSubmitAchievementTask>(ServerType.GATE_SERVER, kCmd);
                    GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_FINISH, (int)iTaskID);
                }
                else if(Utility.IsAccountAchievementMission(iTaskID) || Utility.IsAdventureTeamAccountWeeklyMission(iTaskID))
                {
                    Logger.LogProcessFormat("[Mission]向服务器提交帐号任务! ID = {0}", iTaskID);
					WorldSubmitAccountTask kCmd = new WorldSubmitAccountTask();
            		kCmd.taskId = iTaskID;
            		NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kCmd);
                    GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_FINISH, (int)iTaskID);
                }
                else if(Utility.IsLegendMission(iTaskID))
                {
                    Logger.LogProcessFormat("[Mission]向服务器提交传奇任务! ID = {0}", iTaskID);
                    SceneSubmitLegendTask kCmd = new SceneSubmitLegendTask();
                    kCmd.taskId = iTaskID;

                    NetManager.Instance().SendCommand<SceneSubmitLegendTask>(ServerType.GATE_SERVER, kCmd);
                    GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_FINISH, (int)iTaskID);
                }
                else
                {
                    Logger.LogProcessFormat("[Mission]向服务器提交常规任务! ID = {0}", iTaskID);
                    SceneSubmitTaskReq kCmd = new SceneSubmitTaskReq();
                    kCmd.taskID = iTaskID;
                    kCmd.npcID = iNpcID;
                    kCmd.submitType = (byte)(eSubmitType);

                    NetManager.Instance().SendCommand<SceneSubmitTaskReq>(ServerType.GATE_SERVER, kCmd);
                    GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_FINISH, (int)iTaskID);
                }
            }
        }
        public void sendUnFinishTask(System.UInt32 iTaskID, TaskSubmitType eSubmitType, System.UInt32 iNpcID)
        {
            SceneSubmitTaskReq kCmd = new SceneSubmitTaskReq();
            kCmd.taskID = iTaskID;
            kCmd.npcID = iNpcID;
            kCmd.submitType = (byte)(eSubmitType);
            NetManager.Instance().SendCommand<SceneSubmitTaskReq>(ServerType.GATE_SERVER, kCmd);
            GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_FINISH, (int)iTaskID);
        }

        public void sendCmdAbandomTask(System.UInt32 iTaskID)
        {
            SingleMissionInfo singleMissionInfo = null;
            if (taskGroup.TryGetValue(iTaskID, out singleMissionInfo) && singleMissionInfo.status != (Int32)Protocol.TaskStatus.TASK_FINISHED)
            {
                Logger.LogWarning("[Mission]sendCmdAbandomTask!");
                SceneAbandonTaskReq kCmd = new SceneAbandonTaskReq();
                kCmd.taskID = iTaskID;

                NetManager.Instance().SendCommand<SceneAbandonTaskReq>(ServerType.GATE_SERVER, kCmd);
            }
        }

        public void sendCmdRefreshTask(System.UInt32 iTaskID = 0)
        {
            SceneRefreshCycleTask kCmd = new SceneRefreshCycleTask();
            NetManager.Instance().SendCommand<SceneRefreshCycleTask>(ServerType.GATE_SERVER, kCmd);
        }
        #endregion

        #region uiInterFace

        public string GetMissionValueByKey(System.UInt32 taskId,string key)
        {
            string res = "0";

            MissionManager.SingleMissionInfo missionInfo;
            if(taskSet.TryGetValue(taskId,out missionInfo))
            {
                string value;
                if(missionInfo.taskContents.TryGetValue(key,out value))
                {
                    return value;
                }
            }

            return res;
        }

        public List<AwardItemData> GetMissionOccuAwards(string desc,int targetOccu)
        {
            List<AwardItemData> ret = new List<AwardItemData>();

            if(-1 == targetOccu)
            {
                targetOccu = PlayerBaseData.GetInstance().JobTableID;
            }

            var values = desc.Split(new char[] {','});
            Array.ForEach(values, x =>
            {
                if(string.IsNullOrEmpty(x))
                {
                    return;
                }

                var tokens = x.Split('_');
                int occu = 0;
                int tableid = 0;
                int num = 0;
                int equipType = 0;
                int strengthenLevel = 0;

                if (tokens.Length == 3)
                {
                    int.TryParse(tokens[0], out occu);
                    int.TryParse(tokens[1], out tableid);
                    int.TryParse(tokens[2], out num);
                }
                else if (tokens.Length == 5)
                {
                    int.TryParse(tokens[0], out occu);
                    int.TryParse(tokens[1], out tableid);
                    int.TryParse(tokens[2], out num);
                    int.TryParse(tokens[3], out equipType);
                    int.TryParse(tokens[4], out strengthenLevel);
                }

                //if(!(tokens.Length == 3 && int.TryParse(tokens[0],out occu) &&
                //int.TryParse(tokens[1],out tableid) &&
                //int.TryParse(tokens[2],out num)))
                //{
                //    return;
                //}

                if(num <= 0)
                {
                    return;
                }

                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(tableid);
                if(null == item)
                {
                    return;
                }

                var occuItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(occu);
                if(null == occuItem)
                {
                    return;
                }

                if(targetOccu != occu)
                {
                    return;
                }

                ret.Add(new AwardItemData { ID = tableid, Num = num,EquipType = equipType, StrengthenLevel = strengthenLevel });
            });

            return ret;
        }

        public int GetFinalTitleMission(int iTaskID)
        {
            int iRet = -1;
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            while(null != missionItem)
            {
                iRet = missionItem.ID;
                missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(missionItem.AfterID);
            }
            return iRet;
        }

        public enum MaterialRegexType
        {
            MRT_KEY = 0,
            MRT_KEY_VALUE,
            MRT_COUNT,
        }

        public class MaterialMatchInfo
        {
            public Match match;
            public MaterialRegexType eMaterialRegexType;
        }

        static Regex[] m_akRegexs = new Regex[(int)MaterialRegexType.MRT_COUNT]
        {
            new Regex(@"<key=([a-zA-Z0-9]+)/>"),
            new Regex(@"<key=([a-zA-Z0-9]+)/([0-9]+)/>"),
        };
        public delegate TokenObject OnTokenize(MaterialMatchInfo matchInfo);

        public class TokenObject
        {
            public MaterialRegexType eMaterialRegexType;
            public string tokenedValue;
            public object param0;
            public object param1;
            public object param2;
        }

        public class ParseObject
        {
            public string content;
            public List<TokenObject> tokens = new List<TokenObject>();
        }

        public static ParseObject Parse(string constent, OnTokenize onToken)
        {
            ParseObject retValue = new ParseObject();

            List<object> pools = GamePool.ListPool<object>.Get();
            for(int i =0; i < (int)MaterialRegexType.MRT_COUNT; ++i)
            {
                Regex regex = m_akRegexs[i];
                var matches = regex.Matches(constent);
                for(int j = 0; j < matches.Count; ++j)
                {
                    if(matches[j].Success)
                    {
                        pools.Add(new MaterialMatchInfo
                        {
                            match = matches[j],
                            eMaterialRegexType = (MaterialRegexType)i
                        });
                    }
                }
            }

            pools.Sort((x, y) => 
            {
                return (x as MaterialMatchInfo).match.Index - (y as MaterialMatchInfo).match.Index;
            });

            if(pools.Count > 0)
            {
                System.Text.StringBuilder stringBuilder = StringBuilderCache.Acquire();
                stringBuilder.Clear();
                int position = 0;
                for(int i = 0; i < pools.Count; ++i)
                {
                    var matchInfo = pools[i] as MaterialMatchInfo;
                    var match = matchInfo.match;
                    stringBuilder.Append(constent.Substring(position, match.Index - position));

                    if(null != onToken)
                    {
                        var tokenObject = onToken.Invoke(matchInfo);
                        if(null != tokenObject)
                        {
                            stringBuilder.Append(tokenObject.tokenedValue);
                            tokenObject.eMaterialRegexType = matchInfo.eMaterialRegexType;
                            retValue.tokens.Add(tokenObject);
                        }
                    }

                    position = match.Index + match.Length;
                }

                stringBuilder.Append(constent.Substring(position, constent.Length - position));

                retValue.content = stringBuilder.ToString();
                StringBuilderCache.Release(stringBuilder);
            }
            GamePool.ListPool<object>.Release(pools);
            return retValue;
        }

        public static TokenObject _TokenMaterials(int iTaskID,ItemData itemData,MaterialMatchInfo matchInfo)
        {
            TokenObject token = new TokenObject();
            switch (matchInfo.eMaterialRegexType)
            {
                case MaterialRegexType.MRT_KEY:
                    {
                        token.tokenedValue = GetInstance().GetMissionValueByKey((uint)iTaskID, matchInfo.match.Groups[1].Value);
                        break;
                    }
                case MaterialRegexType.MRT_KEY_VALUE:
                    {
                        string keyValue = GetInstance().GetMissionValueByKey((uint)iTaskID, matchInfo.match.Groups[1].Value);
                        bool bIncome = false;
                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
                        if(null != item)
                        {
                            bIncome = item.Type == ProtoTable.ItemTable.eType.INCOME;
                        }
                        int iPre = 0;
                        int.TryParse(keyValue, out iPre);
                        int iAft = 0;
                        int.TryParse(matchInfo.match.Groups[2].Value, out iAft);
                        bool bTaskOver = false;
                        var missionValue = GetInstance().GetMission((uint)iTaskID);
                        if(null != missionValue && missionValue.status == (int)Protocol.TaskStatus.TASK_OVER)
                        {
                            iPre = iAft;
                            bTaskOver = true;
                        }
                        iPre = IntMath.Min(iPre, iAft);
                        if (!bIncome)
                        {
                            token.tokenedValue = string.Format("{0}/{1}", iPre, iAft);
                        }
                        else
                        {
                            token.tokenedValue = string.Format("{0}",iAft);
                        }
                        token.param0 = iPre;
                        token.param1 = iAft;
                        token.param2 = bTaskOver;
                        break;
                    }
            }
            return token;
        }

        public List<ItemData> GetMissionMaterials(int iTaskID)
        {
            List<ItemData> ret = new List<ItemData>();

            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            if(null != missionItem)
            {
                for(int i = 0; i < missionItem.MissionMaterials.Count; ++i)
                {
                    var tokens = missionItem.MissionMaterials[i].Split('_');
                    int iId = 0;
                    int iCount = 0;
                    if(tokens.Length == 2 && int.TryParse(tokens[0],out iId) && int.TryParse(tokens[1],out iCount) && iCount > 0)
                    {
                        var itemData = ItemDataManager.CreateItemDataFromTable(iId);
                        if(null != itemData)
                        {
                            itemData.Count = iCount;
                            ret.Add(itemData);
                        }
                    }
                }
            }

            return ret;
        }

        public List<AwardItemData> GetMissionAwards(int iTaskID,int occu = -1)
        {
            List<AwardItemData> awardItems = new List<AwardItemData>();
            awardItems.Clear();
            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            string awardValue = null;
            if (missionItem != null)
            {
                ProtoTable.RewardAdapterTable awards = TableManager.GetInstance().GetTableItem<ProtoTable.RewardAdapterTable>(missionItem.RewardAdapter);
                if (awards != null)
                {
                    string value = MissionManager.GetInstance().GetMissionValueByKey((uint)iTaskID, "DAILYLEVEL");
                    int iLevel = PlayerBaseData.GetInstance().Level;
                    if (int.TryParse(value, out iLevel))
                    {
                        Type type = awards.GetType();
                        var propertyInfo = type.GetProperty("Level" + value);
                        if (propertyInfo != null)
                        {
                            awardValue = propertyInfo.GetValue(awards, null) as string;
                        }
                    }
                }
                else
                {
                    awardValue = missionItem.Award;
                }

                if (!string.IsNullOrEmpty(awardValue))
                {
                    var contents = awardValue.Split(new char[] { ',' });
                    for (int i = 0; i < contents.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(contents[i]))
                        {
                            var tokens = contents[i].Split(new char[] { '_' });
                            if(tokens.Length == 2)
                            {
                                AwardItemData itemData = new AwardItemData();
                                if(int.TryParse(tokens[0],out itemData.ID) &&
                                    int.TryParse(tokens[1],out itemData.Num))
                                {
                                    awardItems.Add(itemData);
                                }
                            }
                            else if (tokens.Length == 4)
                            {
                                AwardItemData itemData = new AwardItemData();
                                if (int.TryParse(tokens[0], out itemData.ID) &&
                                    int.TryParse(tokens[1], out itemData.Num)&&
                                    int.TryParse(tokens[2],out itemData.EquipType)&&
                                    int.TryParse(tokens[3],out itemData.StrengthenLevel))
                                {
                                    awardItems.Add(itemData);
                                }
                            }
                        }
                    }
                }

                awardItems.AddRange(GetMissionOccuAwards(missionItem.OccuAward, occu));
            }
            return awardItems;
        }

        public string GetMissionNameAppendBystatus(int status,int iTaskID = 0)
        {
            string ret = "";
            switch (status)
            {
                case (Int32)Protocol.TaskStatus.TASK_INIT:
                    {
                        ret = TR.Value("mission_status_desc_init");
                        break;
                    }
                case (Int32)Protocol.TaskStatus.TASK_FINISHED:
                    {
                        ret = TR.Value("mission_status_desc_finished");
                        break;
                    }
                case (Int32)Protocol.TaskStatus.TASK_UNFINISH:
                    {
                        ret = TR.Value("mission_status_desc_unfinished");
                        break;
                    }
                default:
                    {
                        ret = "";
                        break;
                    }
            }

            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            if(null != missionItem && !IsLevelFit(iTaskID) && missionItem.TaskType == MissionTable.eTaskType.TT_TITLE)
            {
                ret = TR.Value("mission_status_desc_condition");
            }

            return ret;
        }

        public bool IsLevelFit(int iTaskID)
        {
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            if(null == missionItem)
            {
                return false;
            }

            return PlayerBaseData.GetInstance().Level >= missionItem.MinPlayerLv;
        }

        public string GetMissionName(UInt32 iTaskID)
        {
            string name = "";
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)iTaskID);
            if(missionItem != null)
            {
                var eTaskType = missionItem.TaskType;
                string color = "f3db49";
                if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN)
                {
                    color = "f3db49";
                }
                else if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_ACTIVITY)
                {
                    color = "B0B0AF";
                }
                else if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_EXTENTION)
                {
                    color = "FB3231";
                }
                else if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_SYSTEM)
                {
                    color = "f3db49";
                }

                string colorProgress = color;

                if (missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE)
                {
                    if (taskGroup.ContainsKey(iTaskID))
                    {
                        var missionInfo = taskGroup[iTaskID];
                        int iKey = missionInfo.GetIntValue("cycle_task_count");
                        var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_CYCLE_TASK_MAX);
                        int iValue = 0;
                        if(SystemValueTableData != null)
                        {
                            iValue = SystemValueTableData.Value;
                        }
                        iKey = (int)IntMath.Clamp(iKey, 0, iValue);
                        var progress = iKey + "/" + iValue;

                        name = string.Format("<color=#{0}>{1}(<color=#{2}>{3}</color>)</color>",
                            color,
                            missionItem.TaskName,
                            colorProgress,
                            progress);
                    }
                }
                else
                {
                    name = string.Format("<color=#{0}>{1}</color>",
                        color,
                        missionItem.TaskName);
                }
            }
            return name;
        }

        public SingleMissionInfo GetMissionInfoByDungeonID(int dungeonID)
        {
            var enumrator = taskGroup.GetEnumerator();
            while (enumrator.MoveNext())
            {
                SingleMissionInfo info = enumrator.Current.Value;
                if (
                    info.missionItem.TaskType == MissionTable.eTaskType.TT_MAIN   ||
                    info.missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB)
                {
					if(info.status >= (Int32)TaskStatus.TASK_INIT && info.status <= (Int32)TaskStatus.TASK_FAILED)
					{
						if (info.missionItem.MapID == dungeonID)
						{
							return info;
						}
					}
                    
                }
            }

            return null;
        }

        public TaskStatus GetMissionStatus(UInt32 iTaskID)
        {
            TaskStatus eStatus = TaskStatus.TASK_FAILED;

            SingleMissionInfo singleMissionInfo = null;
            if(taskGroup.TryGetValue(iTaskID,out singleMissionInfo))
            {
                if(singleMissionInfo.status >= (Int32)TaskStatus.TASK_INIT && singleMissionInfo.status <= (Int32)TaskStatus.TASK_FAILED)
                {
                    return (TaskStatus)singleMissionInfo.status;
                }
                else
                {
                    Logger.LogError("GetMissionStatus status value is out of enum");
                }
            }

            return eStatus;
        }

        private void _TryUnbindNpcForMission(UInt32 iTaskID,Int32 status)
        {
            ProtoTable.MissionTable missionInfo = TableManager.instance.GetTableItem<ProtoTable.MissionTable>((int)iTaskID);
            if (missionInfo == null)
            {
                return;
            }

            Int32 iNpcID = 0;
            if (status == (int)Protocol.TaskStatus.TASK_INIT)
            {
                if (missionInfo.AcceptType == ProtoTable.MissionTable.eAcceptType.ACT_NPC)
                {
                    iNpcID = missionInfo.MissionTakeNpc;
                }
            }
            else if (status == (int)Protocol.TaskStatus.TASK_FINISHED)
            {
                if (missionInfo.FinishType == ProtoTable.MissionTable.eFinishType.FINISH_TYPE_NPC)
                {
                    iNpcID = missionInfo.MissionFinishNpc;
                }
            }

            ProtoTable.NpcTable npcItem = TableManager.instance.GetTableItem<ProtoTable.NpcTable>(iNpcID);
            if (npcItem == null)
            {
                return;
            }

            TaskNpcAccess.RemoveMissionListener(npcItem.ID,(int)iTaskID);
            TaskNpcAccess.AddDialogListener(npcItem.ID);
        }

        private void _TryAcceptAutoTask(UInt32 iTaskID,bool bTriggerImmediately = false)
        {
            SingleMissionInfo retValue = null;
            if(!taskGroup.TryGetValue(iTaskID,out retValue))
            {
                return;
            }

            if (retValue.status == (int)Protocol.TaskStatus.TASK_INIT)
            {
                IClientSystem current = ClientSystemManager.GetInstance().CurrentSystem;
                ProtoTable.MissionTable missionInfo = TableManager.instance.GetTableItem<ProtoTable.MissionTable>((int)retValue.taskID);
                if (missionInfo != null && missionInfo.AcceptType == ProtoTable.MissionTable.eAcceptType.ACT_AUTO && current != null)
                {
                    if(!CanOpenDlgFrame() && !bTriggerImmediately)
                    {
                        UInt32 iValue = 0;
                        SingleMissionInfo singleMissionInfo = null;
                        if (taskGroup.TryGetValue(iTaskID, out singleMissionInfo))
                        {
                            if (!cachedAutoAcceptTask.TryGetValue(iTaskID, out iValue))
                            {
                                cachedAutoAcceptTask.Add(iTaskID, singleMissionInfo.status);
                            }
                        }
                    }
                    else
                    {
                        CreateTaskDlgFrame((int)retValue.taskID,TaskDlgType.TDT_BEGIN);
                    }
                }
            }
        }

        public void TriggerDungenBegin()
        {
            dungenStart = true;
        }

        public void TriggerDungenEnd()
        {
            iLockedMissionID = 0;

            var CachedAutoAcceptTask = MissionManager.GetInstance().CachedAutoAcceptTask;
            var enumerator = CachedAutoAcceptTask.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if(enumerator.Current.Value == (int)Protocol.TaskStatus.TASK_INIT)
                {
                    _TryAcceptAutoTask(enumerator.Current.Key, true);
                }
                else if(enumerator.Current.Value == (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    //OpenAwardFrame(enumerator.Current.Key);
                    CreateTaskDlgFrame((int)enumerator.Current.Key,TaskDlgType.TDT_END);
                }
                else if (enumerator.Current.Value == (int)Protocol.TaskStatus.TASK_FINISHED + 9527)
                {
                    //OpenAwardFrame(enumerator.Current.Key);
                    CreateTaskDlgFrame((int)enumerator.Current.Key, TaskDlgType.TDT_BEGIN);
                }
            }
            CachedAutoAcceptTask.Clear();
            dungenStart = false;
        }

        /// <summary>
        /// 查找任务接口
        ///
        /// <param name="type"> 主要类型 </param>
        /// <param name="subTypes"> 子类型, 若为null, 则返回所有类型，若为subTypes.Length == 0, 返回空列表 </param>
        /// </summary>
        public List<SingleMissionInfo> GetAllTaskByType(MissionTable.eTaskType type, MissionTable.eSubType[] subTypes = null)
        {
            List<SingleMissionInfo> list = null;
            List<SingleMissionInfo> reList = new List<SingleMissionInfo>();

            if (m_akDiffTasks.ContainsKey(type))
            {
                list = m_akDiffTasks[type];
            }
            else
            {
                if(type == MissionTable.eTaskType.TT_ACHIEVEMENT)
                {
                    Logger.LogError("[关卡宝箱], 获取任务类型，[查章节宝箱界面问题]，m_akDiffTasks doesn`t contain MissionTable.eTaskType.TT_ACHIEVEMENT");
                }
            }

            if (null != list)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (subTypes == null)
                    {
                        reList.Add(list[i]);
                    }
                    else
                    {
                        for (int j = 0; j < subTypes.Length; ++j)
                        {
                            if (subTypes[j] == list[i].missionItem.SubType)
                            {
                                reList.Add(list[i]);
                                break;
                            }
                        }
                    }
                }
            }

            if (subTypes != null && subTypes.Length > 0)
            {
                if(subTypes[0] == MissionTable.eSubType.Dungeon_Chest)
                {
                    if(reList.Count <= 0)
                    {
                        if(list == null)
                        {
                            Logger.LogErrorFormat("[关卡宝箱]，reList.Count = 0, list == null");
                        }
                        else
                        {
                            Logger.LogErrorFormat("[关卡宝箱]，reList.Count = 0, list.Count = {0}", list.Count);
                        }
                    }
                }
            }

            return reList;
        }


        public List<SingleMissionInfo> GetTaskByType(MissionTable.eTaskType eType,Protocol.TaskStatus eStatus = Protocol.TaskStatus.TASK_INIT,bool bInverse = false)
        {
            if(!m_akDiffTasks.ContainsKey(eType) || m_akDiffTasks[eType].Count <= 0)
            {
                return null;
            }

            var values = m_akDiffTasks[eType];
            List<SingleMissionInfo> ret = null;
            for(int i = 0; i < values.Count; ++i)
            {
                if (!bInverse && values[i].status == (int)eStatus ||
                    bInverse && values[i].status != (int)eStatus)
                {
                    if (ret == null)
                    {
                        ret = new List<SingleMissionInfo>();
                    }
                    ret.Add(values[i]);
                }
            }

            return ret;
        }

        public Int32 GetMainTask(Int32 iChapterID = 0)
        {
            Int32 iTaskID = 0;

            if(taskGroup != null)
            {
                var enumrator = taskGroup.GetEnumerator();
                while (enumrator.MoveNext())
                {
                    var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)enumrator.Current.Key);
                    if(missionItem != null &&
                        (missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN||
                        missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE ||
                        missionItem.TaskType == MissionTable.eTaskType.TT_BRANCH ||
							missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB ||
                            missionItem.TaskType == MissionTable.eTaskType.TT_TITLE))
                    {
	                    if(iChapterID == 0)
                        {
                            iTaskID = missionItem.ID;
                            break;
                        }
                        else if(iChapterID/10 == missionItem.MapID/10)
                        {
                            iTaskID = missionItem.ID;
                            break;
                        }
                    }
                }
            }

            return iTaskID;
        }

		public Int32 GetMainTaskMainMission(Int32 iChapterID = 0)
		{
			Int32 iTaskID = 0;

			if(taskGroup != null)
			{
				var enumrator = taskGroup.GetEnumerator();
				while (enumrator.MoveNext())
				{
                    if (null != enumrator.Current.Value && enumrator.Current.Value.status == (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)enumrator.Current.Key);
                        if(missionItem != null &&
                                (missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN||
                                 missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB))
                        {
                            if(iChapterID == 0)
                            {
                                iTaskID = missionItem.ID;
                                break;
                            }
                            else if(iChapterID/10 == missionItem.MapID/10)
                            {
                                iTaskID = missionItem.ID;
                                break;
                            }
                        }
                    }
				}
			}

			return iTaskID;
		}


		public bool IsMainTaskDungeon(int dungeonID)
		{
			if (taskGroup != null)
			{
				var enumrator = taskGroup.GetEnumerator();
				while(enumrator.MoveNext())
				{
					var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)enumrator.Current.Key);
					if (missionItem != null && 
						(missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN || missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB) && 
					missionItem.MapID == dungeonID)
						return true;
				}
			}
			return false;
		}

        public class MissionLevelItems
        {
            public int iLevel;
            public List<MissionTable> akMissionItems;
        }
        List<MissionLevelItems> m_akMissionItems = new List<MissionLevelItems>();

        public List<uint> finisedTaskIDs = new List<uint>();

        int compareLevelItems(MissionLevelItems left, MissionLevelItems right)
        {
            return left.iLevel - right.iLevel;
        }

        void _InitLevel2MissionItems()
        {
            m_akMissionItems.Clear();
            var missionTable = TableManager.GetInstance().GetTable<MissionTable>();
            if(missionTable != null)
            {
                var enumerator = missionTable.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var current = enumerator.Current.Value as MissionTable;
                    if(current != null && current.MissionOnOff == 1)
                    {
                        bool bFind = false;
                        for(int i = 0; i < m_akMissionItems.Count; ++i)
                        {
                            if(m_akMissionItems[i].iLevel == current.MinPlayerLv)
                            {
                                m_akMissionItems[i].akMissionItems.Add(current);
                                bFind = true;
                                break;
                            }
                        }
                        if(!bFind)
                        {
                            var levelItem = new MissionLevelItems();
                            levelItem.iLevel = current.MinPlayerLv;
                            levelItem.akMissionItems = new List<MissionTable>();
                            m_akMissionItems.Add(levelItem);

                            levelItem.akMissionItems.Add(current);
                        }
                    }
                }
            }

            m_akMissionItems.Sort(compareLevelItems);

            for(int i = 0; i < m_akMissionItems.Count; ++i)
            {
                var curLevelMissions = m_akMissionItems[i].akMissionItems;
                if(null != curLevelMissions)
                {
                    curLevelMissions.Sort((x, y) =>
                    {
                        int iXOrder = _GetTaskTypeOrder(x.TaskType);
                        int iYOrder = _GetTaskTypeOrder(y.TaskType);

                        int XSubTypeOrder = _GetSubTypeOrder(x.SubType);
                        int YSubTypeOrder = _GetSubTypeOrder(y.SubType);
                        if (iXOrder != iYOrder)
                        {
                            return iXOrder - iYOrder;
                        }
                        else if(XSubTypeOrder != YSubTypeOrder)
                        {
                            return XSubTypeOrder - YSubTypeOrder;
                        }
                        else
                        {
                            return x.ID - y.ID;
                        }
                    });
                }
            }
        }

        int _GetTaskTypeOrder(ProtoTable.MissionTable.eTaskType eTask)
        {
            if (eTask == MissionTable.eTaskType.TT_CHANGEJOB)
            {
                return 6;
            }

            if (eTask == MissionTable.eTaskType.TT_MAIN)
            {
                return 1;
            }

            if(eTask == MissionTable.eTaskType.TT_BRANCH)
            {
                return 2;
            }

            if(eTask == MissionTable.eTaskType.TT_BRANCH)
            {
                return 3;
            }

            return 100;
        }

        int _GetSubTypeOrder(ProtoTable.MissionTable.eSubType eSub)
        {
            if(eSub == MissionTable.eSubType.NewbieGuide_Mission)
            {
                return 10;
            }
            else
            {
                return 100;
            }
        }

        List<MissionTable> m_akUnOpenDailyMissions = new List<MissionTable>();
        public List<MissionTable> UnOpenDailyMissions
        {
            get
            {
                return m_akUnOpenDailyMissions;
            }
        }
        public delegate void OnUnOpenDailyMissionChanged();
        public OnUnOpenDailyMissionChanged onUnOpenDailyMissionChanged;

        void _CheckLevelFitDailyMission()
        {
            m_akUnOpenDailyMissions.Clear();
            int iCurLevel = PlayerBaseData.GetInstance().Level;
            for(int i = 0; i < m_akMissionItems.Count; ++i)
            {
                //<=当前等级的过滤掉
                if(m_akMissionItems[i].iLevel <= iCurLevel)
                {
                    continue;
                }

                var currentMissions = m_akMissionItems[i].akMissionItems;

                for (int j = 0; j < currentMissions.Count; ++j)
                {
                    var current = currentMissions[j];
                    if(current == null || current.TaskType != MissionTable.eTaskType.TT_DIALY ||
                        current.SubType != MissionTable.eSubType.Daily_Task)
                    {
                        continue;
                    }

                    m_akUnOpenDailyMissions.Add(current);
                }
            }

            m_akUnOpenDailyMissions.Sort((x, y) =>
            {
                return x.MinPlayerLv - y.MinPlayerLv;
            });

            if (onUnOpenDailyMissionChanged != null)
            {
                onUnOpenDailyMissionChanged.Invoke();
            }
        }

        public class MissionScoreData
        {
            public MissionScoreTable missionScoreItem;
            public float fPostion;
            public bool bOpen = false;
            //public Sprite GetIcon()
            //{
            //    if(!bOpen)
            //    {
            //        return AssetLoader.instance.LoadRes(missionScoreItem.UnOpenedChestBoxIcon, typeof(Sprite)).obj as Sprite;
            //    }

            //    return AssetLoader.instance.LoadRes(missionScoreItem.OpenedChestBoxIcon, typeof(Sprite)).obj as Sprite;
            //}
            public void GetIcon(ref Image image)
            {
                if (!bOpen)
                {
                    // return AssetLoader.instance.LoadRes(missionScoreItem.UnOpenedChestBoxIcon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref image, missionScoreItem.UnOpenedChestBoxIcon);
                    return;
                }

                // return AssetLoader.instance.LoadRes(missionScoreItem.OpenedChestBoxIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref image, missionScoreItem.OpenedChestBoxIcon);
            }
            public List<AwardItemData> awards = new List<AwardItemData>();
        };
        List<MissionScoreData> m_akMissionScoreDatas = new List<MissionScoreData>();
        public List<MissionScoreData> MissionScoreDatas
        {
            get
            {
                return m_akMissionScoreDatas;
            }
        }
        void _InitMissionScore()
        {
            if(m_akMissionScoreDatas.Count > 0)
            {
                return;
            }
            m_akMissionScoreDatas.Clear();
            m_iMaxScore = 200;

            var values = TableManager.GetInstance().GetTable<MissionScoreTable>().Values.ToList();
            values.Sort((x, y) =>
            {
                return (x as MissionScoreTable).Score - (y as MissionScoreTable).Score;
            });

            for (int i = 0; i < values.Count; ++i)
            {
                var current = values[i] as MissionScoreTable;
                if(current != null && current.TotalScore > 0)
                {
                    var data = new MissionScoreData();
                    data.missionScoreItem = current;
                    data.fPostion = data.missionScoreItem.Score * 1.0f / data.missionScoreItem.TotalScore;
                    data.awards.Clear();
                    m_iMaxScore = data.missionScoreItem.TotalScore;
                    if (data.missionScoreItem.Awards.Count > 0)
                    {
                        for(int j = 0; j < data.missionScoreItem.Awards.Count; ++j)
                        {
                            if(!string.IsNullOrEmpty(data.missionScoreItem.Awards[j]))
                            {
                                var tokens = data.missionScoreItem.Awards[j].Split('_');
                                int guid = 0;
                                int num = 0;
                                if(tokens.Length == 2 && int.TryParse(tokens[0],out guid) && int.TryParse(tokens[1],out num))
                                {
                                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(guid);
                                    if(item != null && num > 0)
                                    {
                                        var award = new AwardItemData();
                                        award.ID = guid;
                                        award.Num = num;
                                        data.awards.Add(award);
                                    }
                                }
                            }
                        }
                    }
                    m_akMissionScoreDatas.Add(data);
                }
            }
        }

        public delegate void OnDailyScoreChanged(int score);
        public OnDailyScoreChanged onDailyScoreChanged;
        int m_iScore = 0;
        public int Score
        {
            get
            {
                return m_iScore;
            }
            set
            {
                m_iScore = value;
                if(onDailyScoreChanged != null)
                {
                    onDailyScoreChanged.Invoke(m_iScore);
                }
            }
        }
        int m_iMaxScore;
        public int MaxScore
        {
            get
            {
                return m_iMaxScore;
            }
        }

        List<LegendNotifyData> dicLegendNotifies = new List<LegendNotifyData>();
        public List<LegendNotifyData> DicLegendNotifies
        {
            get
            {
                return dicLegendNotifies;
            }

            private set
            {
                dicLegendNotifies = value;
            }
        }

        public void ClearLegendNotifies()
        {
            for (int i = 0; i < MissionManager.GetInstance().DicLegendNotifies.Count; ++i)
            {
                MissionManager.GetInstance().DicLegendNotifies[i].bNotify = false;
            }
        }

        List<int> m_akAcquiredChestIDs = new List<int>();
        public List<int> AcquiredChestIDs
        {
            get
            {
                return m_akAcquiredChestIDs;
            }
            set
            {
                m_akAcquiredChestIDs = value;
                if(onChestIdsChanged != null)
                {
                    onChestIdsChanged.Invoke();
                }
            }
        }
        public delegate void OnChestIdsChanged();
        public OnChestIdsChanged onChestIdsChanged;
        public void SetDailyMaskProperty(DailyTaskMaskProperty property)
        {
            var table = TableManager.GetInstance().GetTable<ProtoTable.MissionScoreTable>();
            List<int> values = new List<int>();
            for(uint i = 0; i < table.Count; ++i)
            {
                int id = (int)(i + 1);
                if(property.CheckMask((uint)id))
                {
                    var scoreItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionScoreTable>(id);
                    if(scoreItem != null)
                    {
                        values.Add(id);
                    }
                }
            }
            AcquiredChestIDs = values;
        }

        public void SendAcquireAwards(int id)
        {
            SceneDailyScoreRewardReq kSend = new SceneDailyScoreRewardReq();
            kSend.boxId = (byte)id;
            NetManager.Instance().SendCommand<SceneDailyScoreRewardReq>(ServerType.GATE_SERVER,kSend);
        }

        //领取每日奖励的返回
        private void OnReceiveSceneDailyScoreRewardRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneDailyScoreRewardRes scoreRewardRes = new SceneDailyScoreRewardRes();
            scoreRewardRes.decode(msgData.bytes);

            if (scoreRewardRes.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) scoreRewardRes.result);
            }
        }

        private void OnReceiveSceneFinishedTaskList(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneFinishedTaskList sceneFinishedTaskList = new SceneFinishedTaskList();
            sceneFinishedTaskList.decode(msgData.bytes);

            finisedTaskIDs = sceneFinishedTaskList.taskIds.ToList();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinishedTaskListSync);
        }

        public int GetNextLevelMission(int iLevel)
        {
            for(int i = 0; i < m_akMissionItems.Count; ++i)
            {
                if (m_akMissionItems[i].iLevel > iLevel)
                {
                    for(int j = 0; j < m_akMissionItems[i].akMissionItems.Count; ++j)
                    {
                        if(m_akMissionItems[i].akMissionItems[j].TaskType == MissionTable.eTaskType.TT_MAIN)
                        {
                            return m_akMissionItems[i].iLevel;
                        }
                    }
                }
            }

            return iLevel;
        }

        public MissionTable GetNextMissionItem(int iLevel)
        {
            for (int i = 0; i < m_akMissionItems.Count; ++i)
            {
                if (m_akMissionItems[i].iLevel > iLevel)
                {
                    for (int j = 0; j < m_akMissionItems[i].akMissionItems.Count; ++j)
                    {
                        if (m_akMissionItems[i].akMissionItems[j].TaskType == MissionTable.eTaskType.TT_MAIN)
                        {
                            if(m_akMissionItems[i].akMissionItems.Count > 0)
                            {
                                return m_akMissionItems[i].akMissionItems[0];
                            }
                        }
                    }
                }
            }
            return null;
        }

        public bool HasLevelUpMission(int iLevel,MissionTable.eTaskType eTaskType)
        {
            if(m_akMissionItems != null && m_akMissionItems.Count > 0)
            {
                var all = m_akMissionItems.FindAll(x => { return x.iLevel > iLevel; });
                for(int i = 0; i < all.Count; ++i)
                {
                    for(int j = 0; j < all[i].akMissionItems.Count; ++j)
                    {
                        if(all[i].akMissionItems[j].TaskType == eTaskType)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        Dictionary<MissionTable.eTaskType, List<SingleMissionInfo>> m_akType2MissionItems = new Dictionary<MissionTable.eTaskType, List<SingleMissionInfo>>();
        int _CmpMissionTypeSort(SingleMissionInfo x, SingleMissionInfo y)
        {
            if (x.missionItem.MinPlayerLv != y.missionItem.MinPlayerLv)
            {
                return x.missionItem.MinPlayerLv - y.missionItem.MinPlayerLv;
            }
            return x.missionItem.ID - y.missionItem.ID;
        }

        void _OnAddTypeMission(SingleMissionInfo missionInfo,bool bNeedSort)
        {
            if(missionInfo != null)
            {
                List<SingleMissionInfo> outValue = null;
                if(!m_akType2MissionItems.TryGetValue(missionInfo.missionItem.TaskType,out outValue))
                {
                    outValue = new List<SingleMissionInfo>();
                    m_akType2MissionItems.Add(missionInfo.missionItem.TaskType, outValue);
                }
                outValue.RemoveAll(x => { return x.missionItem.ID == missionInfo.missionItem.ID; });
                outValue.Add(missionInfo);

                if(bNeedSort)
                {
                    outValue.Sort(_CmpMissionTypeSort);
                }
            }
        }

        void _SortAllTypeMission()
        {
            var enumerator = m_akType2MissionItems.GetEnumerator();
            while(enumerator.MoveNext())
            {
                enumerator.Current.Value.Sort(_CmpMissionTypeSort);
            }
        }

        void _OnRemoveTypeMission(int iTaskID)
        {
            var missionItem = TableManager.GetInstance().GetTableItem<MissionTable>(iTaskID);
            if (missionItem != null)
            {
                List<SingleMissionInfo> outValue = null;
                if (m_akType2MissionItems.TryGetValue(missionItem.TaskType, out outValue))
                {
                    outValue.RemoveAll(x => { return x.missionItem.ID == iTaskID; });
                }
            }
        }

        public List<int> GetMissionDungenTraceList()
        {
            List<int> taskList = new List<int>();
            if(!dungenStart)
            {
                return taskList;
            }
            var keys = taskGroup.Keys.ToList();
            var values = taskGroup.Values.ToList();
            for(int i = 0; i < keys.Count; ++i)
            {
                var x = values[i];
                if (!IsLevelFit(x.missionItem.ID))
                {
                    continue;
                }

                if(x == null || x.missionItem == null)
                {
                    continue;
                }

                if(!(x.missionItem.TaskType == MissionTable.eTaskType.TT_MAIN ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_BRANCH ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_AWAKEN ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_TITLE))
                {
                    continue;
                }

                if(x.missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB &&
                    (x.status != (int)Protocol.TaskStatus.TASK_UNFINISH &&
                    x.status != (int)Protocol.TaskStatus.TASK_FINISHED))
                {
                    continue;
                }

                if(!(BattleDataManager.GetInstance().BattleInfo.dungeonId / 10 == x.missionItem.MapID / 10 && BattleDataManager.GetInstance().BattleInfo.dungeonId >= x.missionItem.MapID))
                {
                    continue;
                }

                taskList.Add((int)keys[i]);
            }
            return taskList;
        }

        int[] mList = new int[6];
        int mListCnt = 0;
        public int ListCnt
        {
            get
            {
                return mListCnt;
            }
        }

        public int[] TraceList
        {
            get
            {
                return mList;
            }
        }

        public int[] GetTraceTaskList()
        {
            mListCnt = 0;

            // 请遵循可重入原则，每次重新计算都应该把旧的数据清除
            // add by qxy 2019-11-22
            if(mList != null)
            {
                for(int i = 0;i < mList.Length;i++)
                {
                    mList[i] = 0;
                }
            }

            //taskList.Add(0);//delete
            //return taskList;//delete
            var missionItemsTemp = new SingleMissionInfo[taskGroup.Count];
            int iLen = 0;
            var enumerator = taskGroup.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current.Value == null)
                {
                    continue;
                }

                // 过滤掉已经完成的任务
                if(enumerator.Current.Value.status == (byte)TaskStatus.TASK_OVER)
                {
                    continue;
                }

                missionItemsTemp[iLen++] = enumerator.Current.Value;
            }
            int end = iLen;

            int[] aiStatus = new int[(int)Protocol.TaskStatus.TASK_OVER + 1]
            {
                2,//(int)Protocol.TaskStatus.TASK_INIT,
                1,//(int)Protocol.TaskStatus.TASK_UNFINISH,
               0,// (int)Protocol.TaskStatus.TASK_FINISHED,
                3,//(int)Protocol.TaskStatus.TASK_FAILED,
                4,//(int)Protocol.TaskStatus.TASK_SUBMITTING,
                5,//(int)Protocol.TaskStatus.TASK_OVER,
            };

            int[] aiMissionType = new int[(int)MissionTable.eTaskType.TT_TITLE + 1]
            {
                4,//TT_DIALY
                -1,//TT_MAIN
                3,//TT_BRANCH
                5,//TT_ACHIEVEMENT
                6,//TT_SYSTEM
                7,//TT_ACTIVITY
                8,//TT_EXTENTION
                0,//TT_CHANGEJOB
                -2,//TT_AWAKEN
                1,//TT_CYCLE
                1,//TT_RED_PACKET
                2,//TT_TITLE
            };

            for(int i = 0; i < end; ++i)
            {
                var x = missionItemsTemp[i];
                if(x == null || x.missionItem == null)
                {
                    missionItemsTemp[i] = missionItemsTemp[end - 1];
                    missionItemsTemp[end - 1] = null;
                    --i;
                    --end;
                    continue;
                }

                if(!IsLevelFit(x.missionItem.ID))
                {
                    missionItemsTemp[i] = missionItemsTemp[end - 1];
                    missionItemsTemp[end - 1] = null;
                    --i;
                    --end;
                    continue;
                }

                if((x.missionItem.TaskType != MissionTable.eTaskType.TT_MAIN && x.missionItem.TaskType != MissionTable.eTaskType.TT_CYCLE &&
                    x.missionItem.TaskType != MissionTable.eTaskType.TT_BRANCH && x.missionItem.TaskType != MissionTable.eTaskType.TT_CHANGEJOB &&
                    x.missionItem.TaskType != MissionTable.eTaskType.TT_TITLE && x.missionItem.TaskType != MissionTable.eTaskType.TT_AWAKEN))
                {
                    missionItemsTemp[i] = missionItemsTemp[end - 1];
                    missionItemsTemp[end - 1] = null;
                    --i;
                    --end;
                    continue;
                }

                if(x.missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB && x.status == (int)Protocol.TaskStatus.TASK_INIT)
                {
                    missionItemsTemp[i] = missionItemsTemp[end - 1];
                    missionItemsTemp[end - 1] = null;
                    --i;
                    --end;
                    continue;
                }
            }

            System.Array.Sort(missionItemsTemp,(l, r) =>
             {
                 if(null == l && null == r)
                 {
                     return 0;
                 }

                 if((null == l) != (null == r))
                 {
                     return null != l ? -1 : 1;
                 }

                 if (l.missionItem.TaskType != r.missionItem.TaskType)
                 {
                     return aiMissionType[(int)l.missionItem.TaskType] -
                     aiMissionType[(int)r.missionItem.TaskType];
                 }

                 else if (l.missionItem.SubType != r.missionItem.SubType)
                 {
                     if (l.missionItem.SubType == MissionTable.eSubType.NewbieGuide_Mission)
                     {
                         return -1;
                     }

                     if (r.missionItem.SubType == MissionTable.eSubType.NewbieGuide_Mission)
                     {
                         return 1;
                     }
                 }

                 if (l.status != r.status)
                 {
                     return aiStatus[l.status] - aiStatus[r.status];
                 }

                 return r.missionItem.MinPlayerLv - l.missionItem.MinPlayerLv;
             });

            for(int i = 0; i < missionItemsTemp.Length && i < end && i < mList.Length; ++i)
            {
                if(mListCnt < mList.Length)
                {
                    mList[mListCnt++] = (int)missionItemsTemp[i].taskID;
                }
            }

            if(_HasFinishedCurrentAll(MissionTable.eTaskType.TT_MAIN))
            {
                if (_HasFinishedCurLevelAndHasNextLv(MissionTable.eTaskType.TT_MAIN))
                {
                    if(mListCnt < mList.Length)
                    {
                        mList[mListCnt++] = 0;
                    }
                    else
                    {
                        mList[mListCnt - 1] = 0;
                    }
                }
                else
                {
                    if (mListCnt < mList.Length)
                    {
                        mList[mListCnt++] = 2;
                    }
                    else
                    {
                        mList[mListCnt - 1] = 2;
                    }
                }

                //put main front of queue
                int temp = mList[mListCnt - 1];
                for (int j = mListCnt - 1; j > 0; --j)
                {
                    mList[j] = mList[j - 1];
                }
                mList[0] = temp;
            }

            /*
            if(_HasFinshedAll(MissionTable.eTaskType.TT_MAIN) &&
                _HasFinshedAll(MissionTable.eTaskType.TT_BRANCH) &&
                _HasFinshedAll(MissionTable.eTaskType.TT_CYCLE))
            {
                taskList.Add(1);
            }
            else if(_HasFinishedCurLevelAndHasNextLv(MissionTable.eTaskType.TT_MAIN))
            {
                taskList.Add(0);
            }
            */

            List<MissionValue> missions = new List<MissionValue>();
            if(missions != null)
            {
                for(int i = 0;i < mList.Length;i++)
                {
                    missions.Add(MissionManager.GetInstance().GetMission((uint)mList[i]));
                }

                MainMissionList.SortBranchTasks(ref missions);
                for(int i = 0;i < missions.Count;i++)
                {
                    MissionValue mission = missions[i];
                    if(mission == null)
                    {
                        continue;
                    }

                    mList[i] = (int)mission.taskID;
                }
            }           

            return mList;
        }

        bool _HasFinishedCurLevelAndHasNextLv(MissionTable.eTaskType eType)
        {
            if(!_HasFinishedCurrentAll(eType))
            {
                return false;
            }

            if (!HasLevelUpMission(PlayerBaseData.GetInstance().Level,eType))
            {
                return false;
            }

            return true;
        }

        bool _HasFinishedCurrentAll(MissionTable.eTaskType eType)
        {
            if (m_akType2MissionItems.ContainsKey(eType) && m_akType2MissionItems[eType].Count > 0)
            {
                return false;
            }

            return true;
        }

        bool _HasFinshedAll(MissionTable.eTaskType eType)
        {
            if(m_akType2MissionItems.ContainsKey(eType) && m_akType2MissionItems[eType].Count > 0)
            {
                return false;
            }

            if(HasLevelUpMission(PlayerBaseData.GetInstance().Level,eType))
            {
                return false;
            }

            return true;
        }

        void _TryOpenFunctionFrame()
        {
            if (!bLoadingScene && ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (current != null)
                {
                    CitySceneTable TownTableData = TableManager.instance.GetTableItem<CitySceneTable>(current.CurrentSceneID);
                    if (TownTableData == null)
                    {
                        return;
                    }

                    if (TownTableData.SceneType == CitySceneTable.eSceneType.PK_PREPARE)
                    {
                        if (ClientSystemManager.instance.IsFrameOpen<FunctionFrame>())
                        {
                            ClientSystemManager.instance.CloseFrame<FunctionFrame>();
                        }
                        return;
                    }

                    if (!ClientSystemManager.instance.IsFrameOpen<FunctionFrame>())
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SwitchToMission);
                    }

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.FunctionFrameUpdate;
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
            }
        }

        int iLockedMissionID = 0;
        public int LockedMissionID
        {
            get { return iLockedMissionID; }
        }

        bool _TryOpenTaskGuideFrame(Int32 taskID,Int32 chapterID = 0,bool bCreateNPC = false)
        {
			bool ret = false;
            if (!bLoadingScene && ClientSystem.IsTargetSystem<ClientSystemBattle>())
            {
                ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(taskID);
				if (!BattleMain.IsModeTrain(BattleMain.battleType) && 
					!BattleMain.IsModeMultiplayer(BattleMain.mode) && 
					missionItem != null &&
                    chapterID >= missionItem.MapID &&
					(missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN ||
                        missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE ||
                        missionItem.TaskType == MissionTable.eTaskType.TT_BRANCH ||
                        missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB ||
                        missionItem.TaskType == MissionTable.eTaskType.TT_AWAKEN ||
                        missionItem.TaskType == MissionTable.eTaskType.TT_TITLE))
                {
                    MissionDungenFrame.Open();
                    iLockedMissionID = taskID;

					//创建npc任务的npc
					if (missionItem.TaskLevelType == ProtoTable.MissionTable.eTaskLevelType.NPC_PROTECT)
					{
						PVEBattle battle = BattleMain.instance.GetBattle() as PVEBattle;
						if (battle != null)
						{
							//8011011
                            try
                            {
                                var npcID = Convert.ToInt32(missionItem.MissionParam);
                                if (npcID <= 0)
                                    Logger.LogErrorFormat("npcid:{0}", npcID);
                                else if(bCreateNPC)
                                    battle.CreateNPC(npcID);
                            }
                            catch (Exception e)
                            {
                                Logger.LogErrorFormat("error:{0}", e.ToString());
                            }
							
						}
					}

                    var dlgItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(missionItem.InTaskDlgID);
                    if(taskGroup.ContainsKey((uint)taskID))
                    {
                        var value = taskGroup[(uint)taskID];
						if(value != null && value.status == (int)Protocol.TaskStatus.TASK_UNFINISH)
                        {
                            if (dlgItem != null && dungenStart && BattleDataManager.GetInstance().BattleInfo.dungeonId / 10 == missionItem.MapID / 10 && BattleDataManager.GetInstance().BattleInfo.dungeonId >= missionItem.MapID)
                            {
								ret = true;
                                if (!BattleMain.IsModePvP(BattleMain.battleType) && BattleMain.instance.Main != null)
                                {
                                    BattleMain.instance.GetDungeonManager().PauseFight(false, "mission");
                                }

                                CreateDialogFrame(dlgItem.ID, taskID, new TaskDialogFrame.OnDialogOver().AddListener(
                                    () =>
                                    {
										var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
                                        if (battleUI != null)
                                            battleUI.ShowTraceAnimation();

                                        if (!BattleMain.IsModePvP(BattleMain.battleType) && BattleMain.instance.Main != null)
                                        {
                                            BattleMain.instance.GetDungeonManager().ResumeFight(false, "mission");
                                        }
                                    })
                                );

                            }
                        }
                    }
                }
            }

			return ret;
        }

        enum TaskDlgType
        {
            TDT_BEGIN = 0,
            TDT_MIDDLE,
            TDT_END,
        }
        void CreateTaskDlgFrame(Int32 iTaskID, TaskDlgType eTaskDlgType)
        {
            ProtoTable.MissionTable missionInfo = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            if (missionInfo != null)
            {
                if (eTaskDlgType == TaskDlgType.TDT_BEGIN || eTaskDlgType == TaskDlgType.TDT_MIDDLE)
                {
                    ProtoTable.TalkTable talkInfo = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(missionInfo.BefTaskDlgID);
                    if (talkInfo != null)
                    {
                        //Logger.LogWarning("CloseAllDialog()");
                        CloseAllDialog();
                        //Logger.LogWarningFormat("CreateDialogFrame() DlgID = {0} taskID = {1}", missionInfo.BefTaskDlgID,iTaskID);
                        CreateDialogFrame(missionInfo.BefTaskDlgID,iTaskID);
                    }
                    else
                    {
                        //Logger.LogWarningFormat("[mission] 前置对话ID = {0} 在对话表中没有找到", missionInfo.BefTaskDlgID);
                    }
                }
                else if(eTaskDlgType == TaskDlgType.TDT_END)
                {
                    if (missionInfo.FinishType == ProtoTable.MissionTable.eFinishType.FINISH_TYPE_AUTO)
                    {
                        ProtoTable.TalkTable talkInfo = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(missionInfo.AftTaskDlgID);
                        if (talkInfo != null)
                        {
                            Logger.LogWarning("CloseAllDialog()");
                            CloseAllDialog();
                            Logger.LogWarningFormat("CreateDialogFrame() DlgID = {0} taskID = {1}", missionInfo.AftTaskDlgID,iTaskID);
                            CreateDialogFrame(missionInfo.AftTaskDlgID,iTaskID);
                        }
                        else
                        {
                            Logger.LogWarningFormat("[mission] 后置对话ID = {0} 在对话表中没有找到", missionInfo.AftTaskDlgID);
                        }
                    }
                }
            }
        }
        #endregion

        #region achievement PopMsgFrameData
        List<int> _PopAchievementItems = new List<int>(8);
        const int _MaxItems = 3;
        void _PushAchievementItems(int iId)
        {
            if(_PopAchievementItems.Count < _MaxItems)
            {
                _PopAchievementItems.Add(iId);
            }
            else
            {
                _PopAchievementItems.RemoveAt(0);
                _PopAchievementItems.Add(iId);
            }
        }
        public int PopAchievementItem()
        {
            if(_PopAchievementItems.Count > 0)
            {
                int iRet = _PopAchievementItems[0];
                _PopAchievementItems.RemoveAt(0);
                return iRet;
            }
            return 0;
        }

        public bool HasAchievementItem()
        {
            return _PopAchievementItems.Count > 0;
        }

        public void PushTestAchievementItems(int iId)
        {
            _PushAchievementItems(iId);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementComplete);
        }

        public static void CommandOpen(object argv)
        {
            GetInstance().PushTestAchievementItems(3251);
        }
        #endregion
        public void FinishAccountTaskReq(int ID)
        {
            WorldSubmitAccountTask req = new WorldSubmitAccountTask();
            req.taskId = (UInt32)ID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
			GameStatisticManager.GetInstance().DoStatTask(StatTaskType.TASK_FINISH, ID);
        }
    }
}
