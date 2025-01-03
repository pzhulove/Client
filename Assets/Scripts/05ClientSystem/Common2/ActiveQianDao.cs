using UnityEngine;
using System.Collections;

namespace GameClient
{
    class ActiveQianDao : MonoBehaviour
    {
        public UIGray comGray;
        public int iBeginID;
        public int iEndID;

        void Start()
        {
            ActiveManager.GetInstance().onAddMainActivity += _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity += _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity += _OnUpdateMainActivity;
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;

            _Update();
        }

        void _OnAddMainActivity(ActiveManager.ActiveData data)
        {
            _Update();
        }

        void _OnRemoveMainActivity(ActiveManager.ActiveData data)
        {
            _Update();
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            _Update();
        }

        void _OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            _Update();
        }

        void OnDestroy()
        {
            ActiveManager.GetInstance().onAddMainActivity -= _OnAddMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity -= _OnUpdateMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity -= _OnRemoveMainActivity;
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
        }

        void _Update()
        {
            bool bEnable = -1 != _GetSubmitID();
            if(null != comGray)
            {
                comGray.enabled = !bEnable;
            }
        }

        public void OnSubmitID()
        {
            int iSubID = _GetSubmitID();
            if(-1 != iSubID)
            {
                ActiveManager.GetInstance().SendSubmitActivity(iSubID);
            }
        }

        int _GetSubmitID()
        {
            int iSubID = -1;
            var activities = GamePool.ListPool<ActiveManager.ActivityData>.Get();
            for (int i = iBeginID; i <= iEndID; ++i)
            {
                var activityData = ActiveManager.GetInstance().GetChildActiveData(i);
                if (null != activityData)
                {
                    activities.Add(activityData);
                }
            }
            activities.Sort(Cmp);
            for (int i = 0; i < activities.Count; ++i)
            {
                if (activities[i].status == (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    iSubID = activities[i].ID;
                    break;
                }
            }
            GamePool.ListPool<ActiveManager.ActivityData>.Release(activities);
            return iSubID;
        }

        public static int Cmp(ActiveManager.ActivityData left, ActiveManager.ActivityData right)
        {
            if (left.activeItem.SortPriority != right.activeItem.SortPriority)
            {
                return left.activeItem.SortPriority - right.activeItem.SortPriority;
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

            if (left.activeItem.SortPriority2 != right.activeItem.SortPriority2)
            {
                return left.activeItem.SortPriority2 - right.activeItem.SortPriority2;
            }

            return left.activeItem.ID - right.activeItem.ID;
        }
    }
}