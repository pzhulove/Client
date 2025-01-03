using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using ProtoTable;
using Scripts.UI;
using MissionValue = GameClient.MissionManager.SingleMissionInfo;
using FilterType = GameClient.MissionFrameNew.FilterZeroType;

namespace GameClient
{
    class DailyMissionList
    {
        ClientFrame clientFrame = null;
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        List<MissionValue> missions = null;
        public delegate void OnRedPointChanged(bool bCheck);
        public OnRedPointChanged onRedPointChanged;

        public static bool HasFinishedDailyTask()
        {
            var missions = _GetDailyMissions();
            if(_CheckRedPoint(missions))
            {
                return true;
            }
            return false;
        }

        public static int GetFinishedDailyTask()
        {
            int iRet = 0;
            var missions = _GetDailyMissions();
            if(missions != null)
            {
                for (int i = 0; i < missions.Count; ++i)
                {
                    if (missions[i].status == (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        ++iRet;
                    }
                }
            }
            return iRet;
        }

        static bool _IsLegalDailyMission(MissionValue value)
        {
            if (value == null || value.missionItem == null)
            {
                return false;
            }

            if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_DIALY
                && value.missionItem.SubType == ProtoTable.MissionTable.eSubType.Daily_Task)
            {
                return true;
            }

            return false;
        }

        ComDailyScript _OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComDailyScript>();
        }

        public void Initialize(ClientFrame clientFrame,GameObject gameObject,OnRedPointChanged onRedPointChanged)
        {
            this.clientFrame = clientFrame;
            this.gameObject = gameObject;
            this.onRedPointChanged = onRedPointChanged;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();

            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            comUIListScript.onItemSelected += _OnItemSelectedDelegate;

            MissionManager.GetInstance().missionChangedDelegate += OnMissionListChanged;
            MissionManager.GetInstance().onAddNewMission += OnAddDailyMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateDailyMission;
            MissionManager.GetInstance().onDeleteMission += OnDeleteDailyMission;

            _LoadDailyMissions();
        }

        static List<MissionValue> _GetDailyMissions()
        {
            var missions = MissionManager.GetInstance().taskGroup.Values.ToList();
            missions.RemoveAll(x =>
            {
                return !_IsLegalDailyMission(x);
            });
            return missions;
        }

        static bool _CheckRedPoint(List<MissionValue> values)
        {
            if(values != null)
            {
                for (int i = 0; i < values.Count; ++i)
                {
                    if(values[i].status == (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckRedPoint()
        {
            return _CheckRedPoint(missions);
        }

        void _LoadDailyMissions()
        {
            missions = _GetDailyMissions();

            if(missions != null && comUIListScript!=null)
            {
                missions.Sort(_Sort);
                comUIListScript.SetElementAmount(missions.Count);

                if (onRedPointChanged != null)
                {
                    onRedPointChanged.Invoke(_CheckRedPoint(missions));
                }
            }
        }

        int _Sort(MissionValue left, MissionValue right)
        {
            if (left.status != right.status)
            {
                if (left.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return 1;
                }

                if (right.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return -1;
                }

                return (int)right.status - (int)left.status;
            }

            if (left.missionItem.SortID != right.missionItem.SortID)
            {
                return left.missionItem.SortID - right.missionItem.SortID;
            }

            if (left.missionItem.MinPlayerLv != right.missionItem.MinPlayerLv)
            {
                return right.missionItem.MinPlayerLv - left.missionItem.MinPlayerLv;
            }

            if (left.taskID != right.taskID)
            {
                return left.taskID < right.taskID ? -1 : 1;
            }

            return 0;
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            if (item != null && item.m_index >= 0 && item.m_index < missions.Count)
            {
                var current = item.gameObjectBindScript as ComDailyScript;
                if (current != null)
                {
                    current.OnVisible(missions[item.m_index],clientFrame);
                }
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            if (item != null && item.m_index >= 0 && item.m_index < missions.Count)
            {
                var current = item.gameObjectBindScript as ComDailyScript;
                if (current != null)
                {
                    //current.OnSelected(true);
                    //(clientFrame as MissionFrameNew).OnMissionSelected(current.Value);
                }
            }
        }


        public void UnInitialize()
        {
            comUIListScript.SetElementAmount(0);

            MissionManager.GetInstance().onDeleteMission -= OnDeleteDailyMission;
            MissionManager.GetInstance().onUpdateMission -= OnUpdateDailyMission;
            MissionManager.GetInstance().onAddNewMission -= OnAddDailyMission;
            MissionManager.GetInstance().missionChangedDelegate -= OnMissionListChanged;

            comUIListScript.onBindItem -= _OnBindItemDelegate;
            comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
            comUIListScript.onItemSelected -= _OnItemSelectedDelegate;
            comUIListScript = null;

            this.onRedPointChanged = null;
            this.clientFrame = null;
            this.gameObject = null;
        }

        void OnAddDailyMission(UInt32 taskID)
        {
            _LoadDailyMissions();
        }

        void OnUpdateDailyMission(UInt32 taskID)
        {
            _LoadDailyMissions();
        }

        void OnDeleteDailyMission(UInt32 taskID)
        {
            _LoadDailyMissions();
        }

        void OnMissionListChanged()
        {
            _LoadDailyMissions();
        }
    }
}