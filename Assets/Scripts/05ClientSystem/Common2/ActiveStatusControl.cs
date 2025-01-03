using UnityEngine;
using System.Collections;

namespace GameClient
{
    class ActiveStatusControl : MonoBehaviour
    {
        public int iActiveConfigID;
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
            bool bOpened = false;
            var activities = ActiveManager.GetInstance().GetType2Templates(iActiveConfigID);
            for(int i = 0; activities != null && i < activities.Count; ++i)
            {
                if(!ActiveManager.GetInstance().ActiveDictionary.ContainsKey(activities[i].ID))
                {
                    continue;
                }
                var activeData = ActiveManager.GetInstance().ActiveDictionary[activities[i].ID];
                if(activeData == null)
                {
                    continue;
                }
                if(activeData.mainInfo != null && activeData.mainInfo.state != 0)
                {
                    bOpened = true;
                    break;
                }
            }

            gameObject.CustomActive(bOpened);
        }

        void _OnAddMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }

        void _OnRemoveMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }

        void _OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }
    }
}