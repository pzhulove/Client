using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace GameClient
{
    public class ComAchievementTabReadPointBinder : MonoBehaviour
    {
        public int mainId = -1;
        public int subId = -1;
        public UnityEvent onSucceed;
        public UnityEvent onFailed;
        bool mDirty = false;
        List<ProtoTable.AchievementGroupSubItemTable> _Items = new List<ProtoTable.AchievementGroupSubItemTable>();
        public void SetId(int mainId, int subId)
        {
            this.mainId = mainId;
            this.subId = subId;
            _MarkDirty();
        }

        void Awake()
        {
            MissionManager.GetInstance().onAddNewMission += _OnAddNewMainMission;
            MissionManager.GetInstance().onUpdateMission += _OnUpdateMainMission;
            MissionManager.GetInstance().onDeleteMission += _OnDeleteMission;
            MissionManager.GetInstance().missionChangedDelegate += _OnMissionChanged;
            MissionManager.GetInstance().onChestIdsChanged += _OnChestIdsChanged;
            _MarkDirty();
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
            _Items.Clear();
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

        void _MarkDirty()
        {
            if (mDirty)
            {
                return;
            }
            mDirty = true;
            InvokeMethod.Invoke(this, 0.50f, _Update);
        }

        void _Update()
        {
            mDirty = false;

            bool bAction = false;

            if(-1 == mainId)
            {
                AchievementGroupDataManager.GetInstance().GetAllItems(ref _Items);
            }
            else
            {
                var mainItem = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementGroupMainItemTable>(mainId);
                var subItem = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementGroupSecondMenuTable>(subId);
                AchievementGroupDataManager.GetInstance().GetSubItemsByTag(mainItem, subItem, ref _Items);
            }

            if(null != _Items)
            {
                for(int i = 0; i < _Items.Count; ++i)
                {
                    if(null == _Items[i])
                    {
                        continue;
                    }
                    var missionValue = MissionManager.GetInstance().GetMission((uint)_Items[i].ID);
                    if(null == missionValue || null == missionValue.missionItem)
                    {
                        continue;
                    }
                    if(missionValue.status == (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        bAction = true;
                        break;
                    }
                }
            }
            
            var action = bAction ? onSucceed : onFailed;
            if (null != action)
            {
                action.Invoke();
            }
        }
    }
}