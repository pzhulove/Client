using System.Collections.Generic;
namespace GameClient
{
    /// <summary>
    /// UI事件信息管理类
    /// </summary>
    public class UIEventManager:Singleton<UIEventManager>
    {
        Dictionary<int, List<UIEventNew.UIEventHandleNew>> m_EventDic;
        public UIEventManager()
        {
            m_EventDic = new Dictionary<int, List<UIEventNew.UIEventHandleNew>>();
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        public UIEventNew.UIEventHandleNew RegisterUIEvent(EUIEventID type, UIEventNew.UIEventHandleNew.Function function)
        {
            UIEventNew.UIEventHandleNew eventHandle = new UIEventNew.UIEventHandleNew((int)type, function);
            if (!m_EventDic.ContainsKey((int)type))
            {
                List<UIEventNew.UIEventHandleNew> list = new List<UIEventNew.UIEventHandleNew>();
                list.Add(eventHandle);
                m_EventDic.Add((int)type, list);
            }
            else
            {
                var list = m_EventDic[(int)type];
                list.Add(eventHandle);
            }
            return eventHandle;
        }
        public void UnRegisterUIEvent(UIEventNew.UIEventHandleNew handler)
        {
            if (handler == null) return;
            List<UIEventNew.UIEventHandleNew> eventHandlers;
            m_EventDic.TryGetValue((int)handler.m_EventType, out eventHandlers);
            if (eventHandlers != null)
            {
                eventHandlers.Remove(handler);
            }
            handler.Remove();
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        public void SendUIEvent(EUIEventID eventType, UIEventNew.UIEventParamNew eventParam)
        {
            if (!m_EventDic.ContainsKey((int)eventType))
                return;
            var eventHandleList = m_EventDic[(int)eventType];
            for (int i = eventHandleList.Count - 1; i >= 0; i--)
            {
                if (eventHandleList[i] == null)
                    continue;
                eventHandleList[i].m_Fn(eventParam);
            }
        }

        /// <summary>
        /// 触发事件 使触发事件调用那边的代码更简洁 更容易使用
        /// </summary>
        public UIEventParam SendUIEvent(EUIEventID eventType, UIEventParam eventParam)
        {
            if (!m_EventDic.ContainsKey((int)eventType))
                return eventParam;

            var p = DataStructPool.UIEventParamPool.Get();
            p.FromStruct(eventParam);
            var eventHandleList = m_EventDic[(int)eventType];
            for (int i = eventHandleList.Count - 1; i >= 0; i--)
            {
                if (eventHandleList[i] == null)
                    continue;
                eventHandleList[i].m_Fn(p);

            }

            p.ToStruct(out eventParam);
            DataStructPool.UIEventParamPool.Release(p);
            return eventParam;
        }

        /// <summary>
        /// 清除所有事件
        /// </summary>
        public void ClearAll()
        {
            m_EventDic.Clear();
        }

        /// <summary>
        /// 获取先得事件监听列表
        /// </summary>
        public Dictionary<int, List<UIEventNew.UIEventHandleNew>> GetNewEventHandleList()
        {
            return m_EventDic;
        }
    }
}
