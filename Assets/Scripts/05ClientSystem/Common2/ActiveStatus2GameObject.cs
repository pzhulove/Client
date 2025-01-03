using UnityEngine;
using System.Collections;

namespace GameClient
{
    [System.Serializable]
    public class Status
    {
        public Protocol.TaskStatus eStatus = Protocol.TaskStatus.TASK_INIT;
        public GameObject[] targets = null;
    }

    public class ActiveStatus2GameObject : MonoBehaviour
    {
        public int iActiveConfigID = 7100;
        public int ActiveConfigID
        {
            get
            {
                return iActiveConfigID;
            }
            set
            {
                iActiveConfigID = value;
                _CheckStatus();
            }
        }
        public Status[] status = null;
        // Use this for initialization
        void Start()
        {
            _CheckStatus();
            ActiveManager.GetInstance().onAddMainActivity += _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity += _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity += _OnUpdateMainActivity;
        }

        void OnDestroy()
        {
            ActiveManager.GetInstance().onAddMainActivity -= _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity -= _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity -= _OnUpdateMainActivity;
        }

        void _CheckStatus()
        {
            int iStatus = 0;
            if (ActiveManager.GetInstance().ActiveDictionary.ContainsKey(iActiveConfigID))
            {
                iStatus = ActiveManager.GetInstance().ActiveDictionary[iActiveConfigID].mainInfo.state;
            }

            for(int i = 0; i < status.Length; ++i)
            {
                if((int)status[i].eStatus != iStatus)
                {
                    System.Array.ForEach(status[i].targets, x => {
                        x.CustomActive(false);
                    });
                }
            }

            for (int i = 0; i < status.Length; ++i)
            {
                if ((int)status[i].eStatus == iStatus)
                {
                    System.Array.ForEach(status[i].targets, x =>
                    {
                        x.CustomActive(true);
                    });
                }
            }
        }

        void _OnAddMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }

        void _OnRemoveMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }

        void _OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }
    }
}