using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace GameClient
{
    public class ComMissionRedBinder : MonoBehaviour
    {
        public enum MissionRedBinderType
        {
            MRBT_MAIN = (1 << 0),
            MRBT_DAILY = (1 << 1),
            MRBT_DAILY_CHEST = (1 << 2),
            MRBT_ACHIEVEMENT = (1 << 3),
            MRBT_TITLE = (1 << 4),
        }

        public MissionRedBinderType[] mFlags = new MissionRedBinderType[0];
        public UnityEvent onSucceed;
        public UnityEvent onFailed;
        bool mDirty = false;

        public void AddRedPointBinder(MissionRedBinderType[] binders)
        {
            mFlags = binders;
            _MarkDirty();
        }

        void Awake()
        {
            MissionManager.GetInstance().onAddNewMission += _OnAddNewMainMission;
            MissionManager.GetInstance().onUpdateMission += _OnUpdateMainMission;
            MissionManager.GetInstance().onDeleteMission += _OnDeleteMission;
            MissionManager.GetInstance().missionChangedDelegate += _OnMissionChanged;
            MissionManager.GetInstance().onChestIdsChanged += _OnChestIdsChanged;
        }

        void OnDestroy()
        {
            MissionManager.GetInstance().onAddNewMission -= _OnAddNewMainMission;
            MissionManager.GetInstance().onUpdateMission -= _OnUpdateMainMission;
            MissionManager.GetInstance().onDeleteMission -= _OnDeleteMission;
            MissionManager.GetInstance().missionChangedDelegate -= _OnMissionChanged;
            MissionManager.GetInstance().onChestIdsChanged -= _OnChestIdsChanged;
            InvokeMethod.RemoveInvokeCall(this);
            mDirty = false;
        }

        void _OnAddNewMainMission(UInt32 taskID)
        {
            _MarkDirty();
        }

        void _OnDeleteMission(UInt32 taskID)
        {
            _MarkDirty();
        }

        void _OnUpdateMainMission(UInt32 taskID)
        {
            _MarkDirty();
        }

        void _OnMissionChanged()
        {
            _MarkDirty();
        }

        void _OnChestIdsChanged()
        {
            _MarkDirty();
        }

        bool _CheckDaily()
        {
            return DailyMissionList.GetFinishedDailyTask() > 0;
        }

        bool _CheckDailyChest()
        {
            return MissionDailyFrame.GetChestRedPoint() > 0;
        }

        bool _CheckAchievement()
        {
            return MissionManager.GetInstance().GetAchievementMissionStatusCount((int)Protocol.TaskStatus.TASK_FINISHED) > 0;
        }

        bool _CheckTitle()
        {
            return MissionManager.GetInstance().GetTitleMissionStatusCount((int)Protocol.TaskStatus.TASK_FINISHED) > 0;
        }

        bool _CheckMain()
        {
            return MissionManager.GetInstance().GetMainMissionStatusCount((int)Protocol.TaskStatus.TASK_FINISHED) > 0;
        }

        void _MarkDirty()
        {
            if(mDirty)
            {
                return;
            }
            mDirty = true;
            InvokeMethod.Invoke(this, 0.50f, _Update);
        }

        void Start()
        {
            _MarkDirty();
        }

        void _Update()
        {
            mDirty = false;

            bool bAction = false;

            for(int i = 0; i < mFlags.Length && !bAction; ++i)
            {
                switch(mFlags[i])
                {
                    case MissionRedBinderType.MRBT_MAIN:
                        {
                            bAction = _CheckMain();
                        }
                        break;
                    case MissionRedBinderType.MRBT_DAILY:
                        {
                            bAction = _CheckDaily();
                        }
                        break;
                    case MissionRedBinderType.MRBT_DAILY_CHEST:
                        {
                            bAction = _CheckDailyChest();
                        }
                        break;
//                     case MissionRedBinderType.MRBT_ACHIEVEMENT:
//                         {
//                             bAction = _CheckAchievement();
//                         }
//                         break;
                    case MissionRedBinderType.MRBT_TITLE:
                        {
                            bAction = _CheckTitle();
                        }
                        break;
                }
            }

            var action = bAction ? onSucceed : onFailed;
            if(null != action)
            {
                action.Invoke();
            }
        }
    }
}