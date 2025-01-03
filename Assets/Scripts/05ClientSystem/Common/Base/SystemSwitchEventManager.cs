using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;


namespace GameClient
{
    class SystemSwitchEventManager : Singleton<SystemSwitchEventManager>
    {
        Dictionary<SystemEventType, SystemUnityAction> m_events = new Dictionary<SystemEventType, SystemUnityAction>();

        public void RegisterEvent(SystemEventType eSystemEventType, SystemUnityAction action)
        {
            if(!m_events.ContainsKey(eSystemEventType))
            {
                m_events.Add(eSystemEventType, action);
            }
            else
            {
                m_events[eSystemEventType] = action;
            }
        }

        public void TriggerEvent(SystemEventType eSystemEventType)
        {
            if(m_events.ContainsKey(eSystemEventType))
            {
                var action = m_events[eSystemEventType];
                if(null != action)
                {
                    action.Invoke();
                }
                m_events.Remove(eSystemEventType);
            }
        }

        public void RemoveEvent(SystemEventType eSystemEventType)
        {
            if(m_events.ContainsKey(eSystemEventType))
            {
                m_events.Remove(eSystemEventType);
            }
        }
    }
}
