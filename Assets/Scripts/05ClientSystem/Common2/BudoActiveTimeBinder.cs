using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace GameClient
{
    public enum BudoActiveStatus
    {
        BAS_CLOSE = 0,
        BAS_PLAYING = 1,
        BAS_READY = 2,
    }

    [Serializable]
    public class StatusConfig
    {
        public BudoActiveStatus eBudoActiveStatus;
        public Sprite sprite;
        public string fmtString;
    }

    class BudoActiveTimeBinder : MonoBehaviour
    {
        public StatusConfig[] akConfigs = new StatusConfig[0];
        public Text timer;
        BudoActiveStatus m_eBudoActiveStatus = BudoActiveStatus.BAS_CLOSE;

        void _UpdateActive()
        {
            m_eBudoActiveStatus = BudoActiveStatus.BAS_CLOSE;
            if (ActiveManager.GetInstance().allActivities.ContainsKey(BudoManager.ActiveID))
            {
                var activity = ActiveManager.GetInstance().allActivities[BudoManager.ActiveID];
                if(activity != null)
                {
                    if(activity.state == 1)
                    {
                        m_eBudoActiveStatus = BudoActiveStatus.BAS_PLAYING;
                    }
                    else if(activity.state == 2)
                    {
                        m_eBudoActiveStatus = BudoActiveStatus.BAS_READY;
                    }
                    else
                    {
                        m_eBudoActiveStatus = BudoActiveStatus.BAS_CLOSE;
                    }
                    _SetTimer(activity);
                }
            }

            gameObject.CustomActive(m_eBudoActiveStatus == BudoActiveStatus.BAS_PLAYING);
        }

        void _SetTimer(Protocol.ActivityInfo activity)
        {
            if(activity == null)
            {
                return;
            }

            if(m_eBudoActiveStatus != BudoActiveStatus.BAS_PLAYING)
            {
                return;
            }

            uint deltaTime = 0;
            if (m_eBudoActiveStatus == BudoActiveStatus.BAS_PLAYING)
            {
                deltaTime = TimeManager.GetInstance().GetServerTime() <= activity.dueTime ? activity.dueTime - TimeManager.GetInstance().GetServerTime() : 0;
            }
            else
            {
                deltaTime = TimeManager.GetInstance().GetServerTime() <= activity.startTime ? activity.startTime - TimeManager.GetInstance().GetServerTime() : 0;
            }

            StatusConfig curConfig = null;
            for(int i = 0; i < akConfigs.Length; ++i)
            {
                if(akConfigs[i].eBudoActiveStatus == m_eBudoActiveStatus)
                {
                    curConfig = akConfigs[i];
                    break;
                }
            }
            if(curConfig == null || string.IsNullOrEmpty(curConfig.fmtString))
            {
                return;
            }

            if (timer != null)
            {
                uint iH = deltaTime / 3600 % 24;
                uint iM = deltaTime / 60 % 60;
                uint iS = deltaTime % 60;
                timer.text = string.Format(curConfig.fmtString, string.Format("{0:D2}:{1:D2}:{2:D2}", iH, iM, iS));
            }
        }

        void OnBudoInfoChanged()
        {
            _UpdateActive();
        }

        // Use this for initialization
        void Start()
        {
            BudoManager.GetInstance().onBudoInfoChanged += OnBudoInfoChanged;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdated);

            CancelInvoke("_UpdateActive");
            InvokeRepeating("_UpdateActive", 0.0f, 1.0f);
        }

        void _OnActivityUpdated(UIEvent a_event)
        {
            uint nID = (uint)a_event.Param1;
            if (nID == BudoManager.ActiveID)
            {
                _UpdateActive();
            }
        }

        void OnDestroy()
        {
            BudoManager.GetInstance().onBudoInfoChanged -= OnBudoInfoChanged;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdated);
            CancelInvoke("_UpdateActive");
        }
    }
}