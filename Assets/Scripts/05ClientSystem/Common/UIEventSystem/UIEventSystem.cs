using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace GameClient
{
    public class ClientEventNode
    {
        public EUIEventID                       id;
        public ClientEventSystem.UIEventHandler handle;
    }

    public class ClientEventSystem
    {
        public delegate void UIEventHandler(UIEvent uiEvent);

        protected Dictionary<EUIEventID, List<UIEventHandler>> m_eventProcessors = new Dictionary<EUIEventID, List<UIEventHandler>>();
        protected List<UIEvent> m_eventBuffers = new List<UIEvent>();

        private   bool bLock = false;
        public void Clear()
        {
            m_eventBuffers.Clear();
            m_eventProcessors.Clear();
        }

        public void PopupLeakedEvents()
        {
            Logger.LogWarning("PopupLeakedEvents");
            var enumerator = m_eventProcessors.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current.Value.Count > 0)
                {
                    var list = enumerator.Current.Value;
                    for (int i = 0; i < list.Count; ++i)
                    {
						Logger.LogWarningFormat("这个倒底是怎么发生的呢？ 真是奇怪 ? Leaked UIEventID = [" + enumerator.Current.Key.ToString() + "]" +
    "Leaked UIEventValue = [" + list[i].Method.ToString() + "]");
                    }
                }
            }
        }

        public void RegisterEventHandler(EUIEventID id, UIEventHandler eventHandler)
        {
            if (m_eventProcessors.ContainsKey(id) == false)
            {
                m_eventProcessors.Add(id, new List<UIEventHandler>());
            }

            List<UIEventHandler> eventHandlers = m_eventProcessors[id];
            if (eventHandlers.Contains(eventHandler) == false)
            {
                eventHandlers.Add(eventHandler);
            }
        }

        public void UnRegisterEventHandler(EUIEventID id, UIEventHandler eventHandler)
        {
            List<UIEventHandler> eventHandlers;
            m_eventProcessors.TryGetValue(id, out eventHandlers);
            if (eventHandlers != null)
            {
                eventHandlers.Remove(eventHandler);
            }
        }

        public UIEvent GetIdleUIEvent()
        {
            UIEvent uiEvent;
            for (int i = 0; i < m_eventBuffers.Count; i++)
            {
                uiEvent = m_eventBuffers[i];
                if (uiEvent.IsUsing == false)
                {
                    uiEvent.Initialize();
                    uiEvent.IsUsing = true;
                    return uiEvent;
                }
            }

            uiEvent = new UIEvent();
            m_eventBuffers.Add(uiEvent);
            uiEvent.Initialize();
            uiEvent.IsUsing = true;
            return uiEvent;
        }

        public static void SendUIEventEx(EUIEventID id, object param1 = null, object param2 = null, object param3 = null, object param4 = null)
        {
#if !LOGIC_SERVER
 

            var sys = UIEventSystem.GetInstance();

            if(sys == null)
            {
                return;
            }

            UIEvent uiEvent = sys.GetIdleUIEvent();
            uiEvent.EventID = id;
            uiEvent.Param1 = param1;
            uiEvent.Param2 = param2;
            uiEvent.Param3 = param3;
            uiEvent.Param4 = param4;
            sys._HandleUIEvent(uiEvent);

 #endif

        }

        public  void SendUIEvent(EUIEventID id, object param1 = null, object param2 = null, object param3 = null, object param4 = null)
        {
#if !LOGIC_SERVER
 
            UIEvent uiEvent = GetIdleUIEvent();
            uiEvent.EventID = id;
            uiEvent.Param1 = param1;
            uiEvent.Param2 = param2;
            uiEvent.Param3 = param3;
            uiEvent.Param4 = param4;
            _HandleUIEvent(uiEvent);
 #endif
        }

        public void SendUIEvent(UIEvent uiEvent)
        {
            _HandleUIEvent(uiEvent);
        }

        protected void _HandleUIEvent(UIEvent uiEvent)
        {
            if(uiEvent != null)
            {
                try
                {
                    //List<UIEventHandler> eventHandlers;

                    //if (m_eventProcessors != null)
                    //{
                    //    m_eventProcessors.TryGetValue(uiEvent.EventID, out eventHandlers);

                    //    if (eventHandlers != null)
                    //    {
                    //        List<UIEventHandler> temp = new List<UIEventHandler>(eventHandlers);
                    //        for (int i = 0; i < temp.Count; ++i)
                    //        {
                    //            temp[i](uiEvent);
                    //        }
                    //    }
                    //}

                    //这边使用一个临时变量保存下  是不是因为遍历的时候 可能会删除Handle? 可以改成脏标记的模式
                    List<UIEventHandler> eventHandlers = GamePool.ListPool<UIEventHandler>.Get();
                    if (m_eventProcessors != null && m_eventProcessors.ContainsKey(uiEvent.EventID))
                    {
                        for (int i = 0; i < m_eventProcessors[uiEvent.EventID].Count; i++)
                        {
                            eventHandlers.Add(m_eventProcessors[uiEvent.EventID][i]);
                        }
                    }
                    for (int i = 0; i < eventHandlers.Count; i++)
                    {
                        eventHandlers[i](uiEvent);
                    }
                    GamePool.ListPool<UIEventHandler>.Release(eventHandlers);

                    uiEvent.IsUsing = false;
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }
            else
            {
                Logger.LogError("uiEvent is null in [_HandleUIEvent].");
            }
        }
    }

    class UIEventSystem : ClientEventSystem
    {
        public static UIEventSystem ms_instance = new UIEventSystem();
        public static UIEventSystem GetInstance()
        {
            return ms_instance;
        }
    }

    class GlobalEventSystem : ClientEventSystem
    {
        public static GlobalEventSystem ms_instance = new GlobalEventSystem();
        public static GlobalEventSystem GetInstance()
        {
            return ms_instance;
        }
    }
}
