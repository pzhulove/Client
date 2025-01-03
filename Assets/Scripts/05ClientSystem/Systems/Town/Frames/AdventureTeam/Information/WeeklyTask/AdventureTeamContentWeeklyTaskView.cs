using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using SingleMissionInfo = GameClient.MissionManager.SingleMissionInfo;
///////删除linq
using System;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public class AdventureTeamContentWeeklyTaskView : AdventureTeamContentBaseView
    {
        [SerializeField] private Text mRefreshTip;
        [SerializeField] private Text mProgressTip;
        [SerializeField] private ComUIListScript mTaskItemUIListScript;

        private string tr_weekly_task_refresh_tip = "";
        private string tr_weekly_task_progress_tip = "";
        private string tr_weekly_task_progress_max_tip = "";

        List<SingleMissionInfo> missionList = null;
        ClientFrame clientFrame = null;
        private void Awake()
        {
            _InitTR();
            BindEvents();
            _bindEvents();
        }

        private void OnDestroy()
        {
            _ClearTR();
            UnBindEvents();
            _unBindEvents();
            UnInitialize();
        }

        private void BindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamWeeklyTaskChange, _LoadAdventureTeamWeeklyMission);
        }

        private void UnBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamWeeklyTaskChange, _LoadAdventureTeamWeeklyMission);
        }

        private void _InitTR()
        {
            tr_weekly_task_refresh_tip = TR.Value("adventure_team_weekly_task_refresh_tip");
            tr_weekly_task_progress_tip = TR.Value("adventure_team_weekly_task_progress_tip");
            tr_weekly_task_progress_max_tip = TR.Value("adventure_team_weekly_task_progress_max");
        }

        private void _ClearTR()
        {
            tr_weekly_task_refresh_tip = null;
            tr_weekly_task_progress_tip = null;
            tr_weekly_task_progress_max_tip = null;
        }

        private void _bindEvents()
        {

        }

        private void _unBindEvents()
        {

        }

        public override void InitData()
        {
            Initialize();
            if(mRefreshTip != null)
            {
                mRefreshTip.text = tr_weekly_task_refresh_tip;
            }

            AdventureTeamDataManager.GetInstance().OnFirstCheckWeeklyTaskFlag = false;
        }

        public override void OnEnableView()
        {
            AdventureTeamDataManager.GetInstance().OnFirstCheckWeeklyTaskFlag = false;
        }

        private void Initialize()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen("AdventureTeamInformationFrame"))
            {
                clientFrame = ClientSystemManager.GetInstance().GetFrame("AdventureTeamInformationFrame") as AdventureTeamInformationFrame;
            }

            if(mTaskItemUIListScript != null)
            {
                mTaskItemUIListScript.Initialize();

                mTaskItemUIListScript.onBindItem += _OnBindItemDelegate;
                mTaskItemUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            }

            _LoadAdventureTeamWeeklyMission(null);
        }

        public void UnInitialize()
        {
            if(mTaskItemUIListScript != null)
            {
                mTaskItemUIListScript.SetElementAmount(0);

                mTaskItemUIListScript.onBindItem -= _OnBindItemDelegate;
                mTaskItemUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                mTaskItemUIListScript = null;
            }

            this.clientFrame = null;
        }

        void _LoadAdventureTeamWeeklyMission(UIEvent uiEvent)
        {
            missionList = AdventureTeamDataManager.GetInstance().ADTMissionList;

            if(missionList != null&& mTaskItemUIListScript != null)
            {
                missionList.Sort(_Sort);
                mTaskItemUIListScript.SetElementAmount(missionList.Count);
                
            }
            _UpdateWeeklyMissionProgress();
        }

        void _UpdateWeeklyMissionProgress()
        {
            int finishedNum = AdventureTeamDataManager.GetInstance()._GetFinishedWeeklyTaskNum();
            int maxNum = AdventureTeamDataManager.GetInstance().ADTMissionFinishMaxNum;
            int lastFinishNum = maxNum - finishedNum;
            bool isAccquireNumMax = lastFinishNum <= 0;
            if (lastFinishNum < 0)
            {
                lastFinishNum = 0;
            }            
            if (mProgressTip)
            {
                if (isAccquireNumMax)
                {
                    mProgressTip.text = string.Format(tr_weekly_task_progress_max_tip, lastFinishNum, maxNum);
                }
                else
                {
                    mProgressTip.text = string.Format(tr_weekly_task_progress_tip, lastFinishNum, maxNum);
                }
            }
        }

        int _Sort(SingleMissionInfo left, SingleMissionInfo right)
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

            int leftDiff;
            int rightDiff;
            if (int.TryParse(left.missionItem.MissionParam, out leftDiff)) ;
            if (int.TryParse(right.missionItem.MissionParam, out rightDiff)) ;
            if (leftDiff != rightDiff)
            {
                return rightDiff - leftDiff;
            }

            if (left.missionItem.SortID != right.missionItem.SortID)
            {
                return left.missionItem.SortID - right.missionItem.SortID;
            }

            if(left.taskID != right.taskID)
            {
                return left.taskID < right.taskID ? -1 : 1;
            }

            return 0;
        }

        #region delegate

        AdventureTeamWeeklyTaskItem _OnBindItemDelegate(GameObject itemObject)
        {
            if (itemObject == null)
            {
                return null;
            }
            return itemObject.GetComponent<AdventureTeamWeeklyTaskItem>();
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            if (item != null && item.m_index >= 0 && item.m_index < missionList.Count)
            {
                var temp = item.gameObjectBindScript as AdventureTeamWeeklyTaskItem;
                if (temp != null)
                {
                    temp.Init(missionList[item.m_index], clientFrame);
                }
            }
        }
        #endregion

        #region WeeklyTaskUtility

        public static int _GetFinishedWeeklyTaskNum()
        {
            int total = 0;
            var missions = AdventureTeamDataManager.GetInstance().ADTMissionList;
            if(missions != null)
            {
                for(int i = 0; i < missions.Count; ++i)
                {
                    if (missions[i].status == (int)Protocol.TaskStatus.TASK_OVER)
                    {
                        total++;
                    }
                }
            }
            return total;
        }
        #endregion

        #region ComUIList

        #endregion
    }
}

