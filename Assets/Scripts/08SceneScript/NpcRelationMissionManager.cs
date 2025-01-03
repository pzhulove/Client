using System;
using System.Collections.Generic;
///////删除linq
using MissionValue = GameClient.MissionManager.SingleMissionInfo;

namespace GameClient
{
    public class NpcRelationMissionManager : DataManager<NpcRelationMissionManager>
    {
        public delegate void OnNpcRelationMissionChanged(int iNpcId);
        public OnNpcRelationMissionChanged onNpcRelationMissionChanged;

        class MissionLinkObject
        {
            public List<MissionValue> parent;
            public int iNpcId;
        }

        Dictionary<int, List<MissionValue>> m_dicNpc2Missions = new Dictionary<int, List<MissionValue>>();
        Dictionary<int, MissionLinkObject> m_dicTask2List = new Dictionary<int, MissionLinkObject>();
        Stack<MissionLinkObject> m_akCachedLinkObjects = new Stack<MissionLinkObject>();
        int[] ms_status_order = new int[(int)Protocol.TaskStatus.TASK_OVER + 1]
        {
            2,//TASK_INIT = 0,
            1,//TASK_UNFINISH = 1,
            0,//TASK_FINISHED = 2,
            3,//TASK_FAILED = 3,
            4,//TASK_SUBMITTING = 4,
            5,//TASK_OVER = 5,
        };

        public List<MissionValue> GetNpcRelationMissions(int iNpcId)
        {
            if(!m_dicNpc2Missions.ContainsKey(iNpcId))
            {
                return null;
            }

            return m_dicNpc2Missions[iNpcId];
        }

        public override void Initialize()
        {
            MissionManager.GetInstance().onAddNewMission += OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateMission;
            MissionManager.GetInstance().onDeleteMissionValue += OnRemoveMission;
            MissionManager.GetInstance().missionChangedDelegate += OnMissionChanged;
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;

            RegisterAllNpcMissions();
        }

        public override void Clear()
        {
            MissionManager.GetInstance().onAddNewMission -= OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission -= OnUpdateMission;
            MissionManager.GetInstance().onDeleteMissionValue -= OnRemoveMission;
            MissionManager.GetInstance().missionChangedDelegate -= OnMissionChanged;
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;

            UnRegisterAllNpcMissions();
        }

        void OnNotifyNpcRelationMissionChanged(int iNpcId)
        {
            if (onNpcRelationMissionChanged != null)
            {
                onNpcRelationMissionChanged.Invoke(iNpcId);
            }
        }

        void OnAddNewMission(UInt32 taskID)
        {
            int iNpcId = GetRelationNpcId(taskID);
            if(0 == iNpcId)
            {
                return;
            }
            RegisterMission(iNpcId,MissionManager.GetInstance().GetMission(taskID));
        }

        void OnUpdateMission(UInt32 taskID)
        {
            UnRegisterMission(MissionManager.GetInstance().GetMission(taskID));
            int iNpcId = GetRelationNpcId(taskID);
            if (0 == iNpcId)
            {
                return;
            }
            RegisterMission(iNpcId, MissionManager.GetInstance().GetMission(taskID));
        }

        void OnRemoveMission(MissionValue value)
        {
            UnRegisterMission(value);
        }

        void OnMissionChanged()
        {
            UnRegisterAllNpcMissions();
            RegisterAllNpcMissions();
        }

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            UnRegisterAllNpcMissions();
            RegisterAllNpcMissions();
        }

        int GetStatusValue(int iStatus)
        {
            if(iStatus >= 0 && iStatus < ms_status_order.Length)
            {
                return ms_status_order[iStatus];
            }
            return ms_status_order.Length;
        }

        int GetRelationNpcId(MissionValue value)
        {
            if (!ComMainMissionScript.IsLegalMainMission(value))
            {
                return 0;
            }

            if (value.status == (int)Protocol.TaskStatus.TASK_INIT &&
                value.missionItem.AcceptType == ProtoTable.MissionTable.eAcceptType.ACT_NPC)
            {
                var npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(value.missionItem.MissionTakeNpc);
                if (npcItem != null)
                {
                    return npcItem.ID;
                }
                return 0;
            }

            if (value.status == (int)Protocol.TaskStatus.TASK_FINISHED &&
                value.missionItem.FinishType == ProtoTable.MissionTable.eFinishType.FINISH_TYPE_NPC)
            {
                var npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(value.missionItem.MissionFinishNpc);
                if (npcItem != null)
                {
                    return npcItem.ID;
                }
                return 0;
            }

            if (value.status == (int)Protocol.TaskStatus.TASK_UNFINISH &&
                value.missionItem.TaskFinishType == ProtoTable.MissionTable.eTaskFinishType.TFT_SUBMIT_ITEM)
            {
                var npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(value.missionItem.MissionFinishNpc);
                if (npcItem != null)
                {
                    return npcItem.ID;
                }
                return 0;
            }

            return 0;
        }

        int GetRelationNpcId(UInt32 taskID)
        {
            MissionValue value = MissionManager.GetInstance().GetMission(taskID);
            if(value == null)
            {
                return 0;
            }

            return GetRelationNpcId(value);
        }

        void RegisterAllNpcMissions()
        {
            var values = MissionManager.GetInstance().taskGroup.Values.ToList();
            for(int i = 0; i < values.Count; ++i)
            {
                int iNpcId = GetRelationNpcId(values[i].taskID);
                if (0 == iNpcId)
                {
                    continue;
                }

                RegisterMission(iNpcId,values[i]);
            }
        }

        void UnRegisterAllNpcMissions()
        {
            var enumerator = m_dicNpc2Missions.GetEnumerator();
            while(enumerator.MoveNext())
            {
                enumerator.Current.Value.Clear();
                OnNotifyNpcRelationMissionChanged(enumerator.Current.Key);
            }
            m_dicNpc2Missions.Clear();
            m_dicTask2List.Clear();
            m_akCachedLinkObjects.Clear();
        }

        void RegisterMission(int iNpcId,MissionValue value)
        {
            if(iNpcId == 0 || value == null || value.missionItem == null)
            {
                return;
            }

            if(PlayerBaseData.GetInstance().Level < value.missionItem.MinPlayerLv)
            {
                return;
            }

            if (m_dicTask2List.ContainsKey(value.missionItem.ID))
            {
                return;
            }

            List<MissionValue> outValues = null;
            if(!m_dicNpc2Missions.TryGetValue(iNpcId,out outValues))
            {
                outValues = new List<MissionValue>();
                m_dicNpc2Missions.Add(iNpcId,outValues);
            }

            for(int i = 0; i < outValues.Count; ++i)
            {
                if(outValues[i].missionItem.ID == value.missionItem.ID)
                {
                    return;
                }
            }

            MissionLinkObject linkObject = null;
            if(m_akCachedLinkObjects.Count > 0)
            {
                linkObject = m_akCachedLinkObjects.Pop();
            }
            else
            {
                linkObject = new MissionLinkObject();
            }

            m_dicTask2List.Add(value.missionItem.ID, linkObject);
            linkObject.parent = outValues;
            outValues.Add(value);
            linkObject.iNpcId = iNpcId;
            
            outValues.Sort((x, y) =>
            {
                if(x.status != y.status)
                {
                    return GetStatusValue(x.status) - GetStatusValue(y.status);
                }

                return x.taskID < y.taskID ? -1 : 1;
            });

            OnNotifyNpcRelationMissionChanged(iNpcId);
        }

        void UnRegisterMission(MissionValue value)
        {
            if(value == null || !m_dicTask2List.ContainsKey(value.missionItem.ID))
            {
                return;
            }

            var linkObject = m_dicTask2List[value.missionItem.ID];
            if(linkObject != null)
            {
                m_akCachedLinkObjects.Push(linkObject);
                m_dicTask2List.Remove(value.missionItem.ID);
                linkObject.parent.Remove(value);
                OnNotifyNpcRelationMissionChanged(linkObject.iNpcId);
            }
        }
    }
}