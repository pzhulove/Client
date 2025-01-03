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
    class MainMissionList
    {
        ClientFrame clientFrame = null;
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        List<MissionValue> missions = null;
        MissionValue curSelected = null;
        FilterType m_eFilterType = FilterType.FZT_ACCEPTED;
        public delegate void OnRedPointChanged(bool bCheck);
        public OnRedPointChanged onRedPointChanged;
        bool _initialized = false;
        public bool Initialized
        {
            get
            {
                return _initialized;
            }
        }
        public static MissionValue Selected
        {
            get
            {
                return ComMainMissionScript.ms_selected;
            }
            set
            {
                ComMainMissionScript.ms_selected = value;
            }
        }


        bool bLockCycle = false;

        bool _IsLegalMainMission(MissionManager.SingleMissionInfo value)
        {
            if (value == null)
            {
                return false;
            }

            if(/*m_eFilterType == FilterType.FZT_UNACCEPTED || */
                m_eFilterType == FilterType.FZT_ACCEPTED)
            {
                if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_BRANCH)
                {
                    return true;
                }

                if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN)
                {
                    return true;
                }

                if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE)
                {
                    return true;
                }

                if(value.missionItem.TaskType == MissionTable.eTaskType.TT_AWAKEN)
                {
                    return true;
                }
            }

            if(m_eFilterType == FilterType.FZT_TITLE_TASK)
            {
                return value.missionItem.TaskType == MissionTable.eTaskType.TT_TITLE;
            }

            return false;
        }

        bool _IsCycleMission(uint iId)
        {
            if (!MissionManager.GetInstance().taskGroup.ContainsKey(iId))
            {
                return false;
            }

            return _IsCycleMission(MissionManager.GetInstance().taskGroup[iId]);
        }

        bool _IsCycleMission(MissionManager.SingleMissionInfo value)
        {
            if (value == null || value.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_CYCLE)
            {
                return false;
            }
            return true;
        }

        ComMainMissionScript _OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComMainMissionScript>();
        }

        void _OnFilter(List<MissionValue> missions, FilterType eFilterZero)
        {
            missions.RemoveAll((x) =>
            {
                return _GetFilterType(x) != eFilterZero;
            });
        }

        public void Filter(FilterType eFilterZero)
        {
            if(m_eFilterType != eFilterZero)
            {
                m_eFilterType = eFilterZero;
                _LoadMainMissions(eFilterZero);
            }
        }

        public void SetCurFilterType(FilterType filterType)
        {
            m_eFilterType = filterType;
        }

        FilterType _GetFilterType(MissionValue value)
        {
            if(value.missionItem.TaskType == MissionTable.eTaskType.TT_TITLE)
            {
                return FilterType.FZT_TITLE_TASK;
            }

            //if(value.status == (int)Protocol.TaskStatus.TASK_INIT)
            //{
            //    return FilterType.FZT_UNACCEPTED;
            //}

            return FilterType.FZT_ACCEPTED;
        }

        public void Initialize(ClientFrame clientFrame,GameObject gameObject,OnRedPointChanged onRedPointChanged, FilterType eFilterType,bool bCycle,bool bLocked)
        {
            if(_initialized)
            {
                return;
            }
            _initialized = true;
            this.clientFrame = clientFrame;
            this.gameObject = gameObject;
            this.onRedPointChanged = onRedPointChanged;
            this.m_eFilterType = eFilterType;
            this.bLockCycle = bCycle;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();

            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            comUIListScript.onItemSelected += _OnItemSelectedDelegate;
            comUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;

            MissionManager.GetInstance().missionChangedDelegate += OnMissionListChanged;
            MissionManager.GetInstance().onDeleteMissionValue += OnDeleteMissionValue;
            MissionManager.GetInstance().onAddNewMission += OnAddNewMainMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateMainMission;
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;

            _LoadMainMissions(m_eFilterType, bLocked);
        }

        List<MissionValue> _GetMainMissions(MissionFrameNew.FilterZeroType eFilterZero = MissionFrameNew.FilterZeroType.FZT_COUNT)
        {
            var missions = MissionManager.GetInstance().taskGroup.Values.ToList();
            missions.RemoveAll(x =>
            {
                return !_IsLegalMainMission(x);
            });

            missions.RemoveAll((x) => 
            {
                return x != null && x.status == (byte)Protocol.TaskStatus.TASK_OVER;
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

        void SortMissions()
        {
            if(missions == null)
            {
                return;
            }

            missions.Sort(_Sort);
            SortBranchTasks(ref missions);
        }

        bool _SelectedCycleMission()
        {
            if(!bLockCycle)
            {
                return false;
            }

            //Logger.LogErrorFormat("_SelectedCycleMission cycle true!!");
            missions = _GetMainMissions();
            SortMissions();

            int iBindIndex = -1;
            for (int i = 0; i < missions.Count; ++i)
            {
                if (missions[i] != null && missions[i].missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE)
                {
                    iBindIndex = i;
                    break;
                }
            }

            //if(bLockCycle)
            //{
            //    Logger.LogErrorFormat("_SelectedCycleMission cycle true!! iBindIndex = {0}", iBindIndex);
            //}

            if(-1 != iBindIndex)
            {
                _TryChangeFilter(missions[iBindIndex]);
            }
            comUIListScript.SetElementAmount(missions.Count);

            iBindIndex = -1;
            for (int i = 0; i < missions.Count; ++i)
            {
                if (missions[i] != null && missions[i].missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE)
                {
                    iBindIndex = i;
                    break;
                }
            }

            if (iBindIndex != -1)
            {
                _SetSelectedItem(iBindIndex);
            }

            return -1 != iBindIndex;
        }

        bool _SelectedFixedMission()
        {
            if(Selected == null)
            {
                return false;
            }

            missions = _GetMainMissions();
            if(missions == null)
            {
                return false;
            }
            _TryChangeFilter(Selected);
            SortMissions();
            if(comUIListScript == null)
            {
                return false;
            }
            comUIListScript.SetElementAmount(missions.Count);

            int iBindIndex = -1;
            for (int i = 0; i < missions.Count; ++i)
            {
                if (missions[i] != null && missions[i].taskID == Selected.taskID)
                {
                    iBindIndex = i;
                    break;
                }
            }

            if (-1 != iBindIndex)
            {
                _SetSelectedItem(iBindIndex);
            }

            return -1 != iBindIndex;
        }

        bool _SelectedFilterMission(FilterType eFilterZero)
        {
            if(eFilterZero == FilterType.FZT_COUNT)
            {
                return false;
            }

            if(clientFrame == null)
            {
                return false;
            }

            missions = _GetMainMissions();
            _OnFilter(missions, eFilterZero);
            m_eFilterType = eFilterZero;
            (clientFrame as MissionFrameNew).OnFilterZeroChanged(eFilterZero);
            SortMissions();
            comUIListScript.SetElementAmount(missions.Count);

            int iBindIndex = -1;
            if(Selected != null)
            {
                for (int i = 0; i < missions.Count; ++i)
                {
                    if (missions[i].taskID == Selected.taskID)
                    {
                        iBindIndex = i;
                        break;
                    }
                }
            }

            if (iBindIndex == -1 && missions.Count > 0)
            {
                iBindIndex = 0;
            }

            _SetSelectedItem(iBindIndex);

            return -1 != iBindIndex;
        }

        void _SelectedRandomMission()
        {
            missions = _GetMainMissions();
            SortMissions();

            if (missions.Count > 0)
            {
                _TryChangeFilter(missions[0]);
                _SetSelectedItem(0);
            }
            comUIListScript.SetElementAmount(missions.Count);
        }

        void _TryChangeFilter(MissionManager.SingleMissionInfo value)
        {
            if(value != null)
            {
                FilterType eTarget = FilterType.FZT_TITLE_TASK;
                if( value.missionItem != null && value.missionItem.TaskType != MissionTable.eTaskType.TT_TITLE)
                {
                    eTarget = FilterType.FZT_ACCEPTED;
                    //eTarget = value.status == (int)Protocol.TaskStatus.TASK_INIT ? FilterType.FZT_UNACCEPTED : FilterType.FZT_ACCEPTED;
                }
                m_eFilterType = eTarget;
                _OnFilter(missions, m_eFilterType);
                MissionFrameNew frame = clientFrame as MissionFrameNew;
                if (frame != null)
                {
                    frame.OnFilterZeroChanged(eTarget);
                }
            }
        }

        void _TrySetDefaultMission(FilterType eFilterZero,bool bLocked)
        {
            if (Selected != null)
            {
                Selected = MissionManager.GetInstance().GetMission(Selected.taskID);
            }

            if(bLocked)
            {
                if (_SelectedCycleMission())
                {
                    return;
                }

                if(_SelectedFixedMission())
                {
                    return;
                }

                if (_SelectedFilterMission(eFilterZero))
                {
                    return;
                }

                _SelectedRandomMission();
            }
            else
            {
                if(eFilterZero != FilterType.FZT_COUNT)
                {
                    _SelectedFilterMission(eFilterZero);
                }
                else
                {
                    m_eFilterType = FilterType.FZT_ACCEPTED;
                    if(!_SelectedFilterMission(FilterType.FZT_ACCEPTED))
                    {
                        m_eFilterType = FilterType.FZT_ACCEPTED;
                        _SelectedFilterMission(FilterType.FZT_ACCEPTED);
                    }
                }
            }
        }

        void _SetSelectedItem(int iBindIndex)
        {
            if (iBindIndex >= 0 && iBindIndex < missions.Count)
            {
                Selected = missions[iBindIndex];
                if (!comUIListScript.IsElementInScrollArea(iBindIndex))
                {
                    comUIListScript.EnsureElementVisable(iBindIndex);
                }
                comUIListScript.SelectElement(iBindIndex);
            }
            else
            {
                comUIListScript.SelectElement(-1);
                Selected = null;
            }

            (clientFrame as MissionFrameNew).OnMissionSelected(Selected);
        }

        private static int[] _sortOrder = new int[]
            {
                3, //TASK_INIT = 0, // 可接取
                2, //TASK_UNFINISH = 1, // 进行中
                1, //TASK_FINISHED = 2, // 可交付
                4, //TASK_FAILED = 3,   // 已失败
                0, //TASK_SUBMITTING = 4, // 交付中
                5, //TASK_OVER = 5, // 已结束
            };

        // 任务界面里的支线任务排序调整：（主界面左侧任务区域支线任务排序需要同步）
        // 先按照任务进度进行排序（可交付＞进行中＞可接取），相同任务进度按照任务等级从高到低排，相同等级则按照任务ID从小到大
        // 注意 此函数是将一个任务列表中的支线任务进行再次排序(任务列表中可能有其他类型的任务)
        public static void SortBranchTasks(ref List<MissionValue> missions)
        {
            List<MissionValue> singleMissionInfos = new List<MissionValue>();
            if (singleMissionInfos == null)
            {
                return;
            }

            if (missions == null)
            {
                return;
            }

            List<int> indexs = new List<int>();
            if (indexs == null)
            {
                return;
            }

            for (int i = 0; i < missions.Count; i++)
            {
                MissionValue singleMissionInfo = missions[i];
                if (singleMissionInfo == null)
                {
                    continue;
                }

                if (singleMissionInfo.missionItem == null)
                {
                    continue;
                }

                if (singleMissionInfo.missionItem.TaskType == MissionTable.eTaskType.TT_BRANCH)
                {
                    indexs.Add(i);
                    singleMissionInfos.Add(singleMissionInfo);
                }
            }

            singleMissionInfos.Sort((a, b) =>
            {
                if (a == null || b == null)
                {
                    return 0;
                }

                if (a.missionItem == null || b.missionItem == null)
                {
                    return 0;
                }

                if (a.status != b.status)
                {
                    return _sortOrder[a.status] - _sortOrder[b.status];
                }

                if(a.missionItem.MinPlayerLv != b.missionItem.MinPlayerLv)
                {
                    return b.missionItem.MinPlayerLv.CompareTo(a.missionItem.MinPlayerLv);
                }

                if(a.missionItem.ID != b.missionItem.ID)
                {
                    return a.missionItem.ID.CompareTo(b.missionItem.ID);
                }
                
                return 0;
            });

            if (indexs.Count != singleMissionInfos.Count)
            {
                return;
            }

            for (int i = 0;i < singleMissionInfos.Count;i++)
            {
                int idx = indexs[i];
                if(idx >= missions.Count)
                {
                    continue;
                }

                missions[idx] = singleMissionInfos[i];
            }

            return;
        }

        void _LoadMainMissions(FilterType eFilterZero,bool bLoced = false)
        {
            missions = _GetMainMissions();
            SortMissions();

            if (onRedPointChanged != null)
            {
                onRedPointChanged.Invoke(_CheckRedPoint(missions));
            }

            _TrySetDefaultMission(eFilterZero,bLoced);
        }

        int _Sort(MissionValue left, MissionValue right)
        {
            if(left.missionItem.TaskType != right.missionItem.TaskType)
            {
                return MissionManager.GetInstance().getSortIDUseType(right.missionItem.TaskType) - MissionManager.GetInstance().getSortIDUseType(left.missionItem.TaskType);
            }

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

            return left.taskID < right.taskID ? -1 : 1;
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            if (item != null && item.m_index >= 0 && item.m_index < missions.Count)
            {
                var current = item.gameObjectBindScript as ComMainMissionScript;
                if (current != null)
                {
                    current.OnVisible(missions[item.m_index],clientFrame);
                }
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            if (item != null)
            {
                var current = item.gameObjectBindScript as ComMainMissionScript;
                if (current != null)
                {
                    Selected = current.Value;
                    (clientFrame as MissionFrameNew).OnMissionSelected(current.Value);
                }
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            if (item != null)
            {
                var comMainScript = item.gameObjectBindScript as ComMainMissionScript;
                if(comMainScript != null)
                {
                    comMainScript.OnDisplayChange(bSelected);
                }
            }
        }

        public void UnInitialize()
        {
            if(_initialized)
            {
                MissionManager.GetInstance().onUpdateMission -= OnUpdateMainMission;
                MissionManager.GetInstance().onAddNewMission -= OnAddNewMainMission;
                MissionManager.GetInstance().onDeleteMissionValue -= OnDeleteMissionValue;
                MissionManager.GetInstance().missionChangedDelegate -= OnMissionListChanged;
                PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;

                if (comUIListScript != null)
                {
                    comUIListScript.onBindItem -= _OnBindItemDelegate;
                    comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                    comUIListScript.onItemSelected -= _OnItemSelectedDelegate;
                    comUIListScript.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
                    comUIListScript = null;
                }

                this.onRedPointChanged = null;
                this.clientFrame = null;
                this.gameObject = null;

                _initialized = false;
            }
        }

        void OnMissionListChanged()
        {
            _LoadMainMissions(m_eFilterType);
        }

        void OnAddNewMainMission(UInt32 taskID)
        {
            if (bLockCycle)
            {
                //Logger.LogErrorFormat("OnAddNewMainMission cycle true!!");
                _LoadMainMissions(m_eFilterType, true);
                bLockCycle = false;
            }
            else
            {
                //Logger.LogErrorFormat("ERROR ===>  OnAddNewMainMission cycle true!!");
                _LoadMainMissions(m_eFilterType);
            }
        }

        void OnDeleteMissionValue(MissionManager.SingleMissionInfo value)
        {
            if (_IsCycleMission(value))
            {
                //Logger.LogErrorFormat("OnDeleteMissionValue cycle true!!");
                bLockCycle = true;
            }
            else
            {
                _LoadMainMissions(m_eFilterType);
            }
        }

        void OnUpdateMainMission(UInt32 taskID)
        {
            _LoadMainMissions(m_eFilterType,true);
        }

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            _LoadMainMissions(m_eFilterType, true);
        }
    }
}