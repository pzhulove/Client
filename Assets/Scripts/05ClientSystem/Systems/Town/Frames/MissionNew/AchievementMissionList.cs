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
    class AchievementMissionList
    {
        ClientFrame clientFrame = null;
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        List<MissionValue> missions = null;
        public delegate void OnRedPointChanged(bool bCheck);
        public OnRedPointChanged onRedPointChanged;
        bool _Initialized = false;
        public bool Initialized
        {
            get
            {
                return _Initialized;
            }
        }

        bool _IsLegalAchievementMission(MissionValue value)
        {
            if(value.missionItem.TaskType == MissionTable.eTaskType.TT_ACHIEVEMENT &&
                value.missionItem.SubType == MissionTable.eSubType.Daily_Null)
            {
                return true;
            }

            return false;
        }

        ComAchievementScript _OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComAchievementScript>();
        }

        public void Initialize(ClientFrame clientFrame,GameObject gameObject,OnRedPointChanged onRedPointChanged)
        {
            if(_Initialized)
            {
                return;
            }
            _Initialized = true;
            this.clientFrame = clientFrame;
            this.gameObject = gameObject;
            this.onRedPointChanged = onRedPointChanged;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();

            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            comUIListScript.onItemSelected += _OnItemSelectedDelegate;

            MissionManager.GetInstance().missionChangedDelegate += OnMissionListChanged;
            MissionManager.GetInstance().onAddNewMission += OnAddAchievementMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateAchievementMission;
            MissionManager.GetInstance().onDeleteMission += OnDeleteAchievementMission;

            _LoadAchievementMissions();
        }

        List<MissionValue> _GetAchievementMissions()
        {
            var missions = MissionManager.GetInstance().taskGroup.Values.ToList();
            missions.RemoveAll(x =>
            {
                return !_IsLegalAchievementMission(x);
            });
            return missions;
        }

        bool _CheckRedPoint(List<MissionValue> values)
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

        void _LoadAchievementMissions()
        {
            missions = _GetAchievementMissions();
            missions.Sort(_Sort);
            comUIListScript.SetElementAmount(missions.Count);

            if (onRedPointChanged != null)
            {
                onRedPointChanged.Invoke(_CheckRedPoint(missions));
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
                var current = item.gameObjectBindScript as ComAchievementScript;
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
                var current = item.gameObjectBindScript as ComAchievementScript;
                if (current != null)
                {
                    //current.OnSelected(true);
                    //(clientFrame as MissionFrameNew).OnMissionSelected(current.Value);
                }
            }
        }


        public void UnInitialize()
        {
            if(_Initialized)
            {
                MissionManager.GetInstance().onDeleteMission -= OnDeleteAchievementMission;
                MissionManager.GetInstance().onUpdateMission -= OnUpdateAchievementMission;
                MissionManager.GetInstance().onAddNewMission -= OnAddAchievementMission;
                MissionManager.GetInstance().missionChangedDelegate -= OnMissionListChanged;

                if(null != comUIListScript)
                {
                    comUIListScript.onBindItem -= _OnBindItemDelegate;
                    comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                    comUIListScript.onItemSelected -= _OnItemSelectedDelegate;
                    comUIListScript = null;
                }

                this.onRedPointChanged = null;
                this.clientFrame = null;
                this.gameObject = null;
                _Initialized = false;
            }
        }

        void OnMissionListChanged()
        {
            _LoadAchievementMissions();
        }

        void OnAddAchievementMission(UInt32 taskID)
        {
            _LoadAchievementMissions();
        }

        void OnUpdateAchievementMission(UInt32 taskID)
        {
            _LoadAchievementMissions();
        }

        void OnDeleteAchievementMission(UInt32 taskID)
        {
            _LoadAchievementMissions();
        }
    }
}