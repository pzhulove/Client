using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 事件信息管理类
    /// </summary>
    public class BeEventManager
    {
        Dictionary<int, List<BeEvent.BeEventHandleNew>> m_EventDic;
        private int m_SenderPId ;
        public BeEventManager(int senderPId)
        {
            m_EventDic = new Dictionary<int, List<BeEvent.BeEventHandleNew>>();
            m_SenderPId = senderPId;
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        public BeEvent.BeEventHandleNew RegisterEvent(int type,BeEvent.BeEventHandleNew.Function function)
        {
            BeEvent.BeEventHandleNew eventHandle = new BeEvent.BeEventHandleNew((int)type, function);
            if (!m_EventDic.ContainsKey((int)type))
            {
                List<BeEvent.BeEventHandleNew> list = new List<BeEvent.BeEventHandleNew>();
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
        
        /// <summary>
        /// 触发事件 使触发事件调用那边的代码更简洁 更容易使用
        /// </summary>
        public EventParam TriggerEvent(int eventType, EventParam eventParam)
        {
            if (!m_EventDic.ContainsKey(eventType))
                return eventParam;

            var p = DataStructPool.EventParamPool.Get();
            p.m_SenderId = m_SenderPId;
            p.FromStruct(eventParam);
            var eventHandleList = m_EventDic[eventType];
            for (int i = eventHandleList.Count - 1; i >= 0; i--)
            {
                var handle = eventHandleList[i];
                if (handle != null && handle.m_CanRemove)
                {
                    eventHandleList.RemoveAt(i);
                }
                else
                {
                    handle.m_Fn(p);
                }
            }

            p.ToStruct(out eventParam);
            DataStructPool.EventParamPool.Release(p);
            return eventParam;
        }

        /// <summary>
        /// 移除指定的事件监听
        /// </summary>
        /// <param name="handle">需要移除的事件数据</param>
        public void RemoveHandle(BeEvent.BeEventHandleNew handle)
        {
            if (handle != null && m_EventDic.ContainsKey(handle.m_EventType))
            {
                var eventHandleList = m_EventDic[handle.m_EventType];
                eventHandleList.Remove(handle);
            }
        }
        
        /// <summary>
        /// 移除指定的事件监听
        /// </summary>
        /// <param name="handle">需要移除的事件数据</param>
        public void RemoveHandle(int eventType, BeEvent.BeEventHandleNew.Function func)
        {
            if (m_EventDic.ContainsKey(eventType))
            {
                var eventHandleList = m_EventDic[eventType];
                eventHandleList.RemoveAll((i) => i.m_Fn == func);
            }
        }

        /// <summary>
        /// 清除所有事件
        /// </summary>
        public void ClearAll()
        {
            var kiter = m_EventDic.Keys.GetEnumerator();
            while (kiter.MoveNext())
            {
                var key = kiter.Current;
                var handles = m_EventDic[key];
                for (int i = 0; i < handles.Count; i++)
                {
                    handles[i].m_Fn = null;
                }
                handles.Clear();
            }
            m_EventDic.Clear();
        }

        /// <summary>
        /// 获取先得事件监听列表
        /// </summary>
        public Dictionary<int,List<BeEvent.BeEventHandleNew>> GetNewEventHandleList()
        {
            return m_EventDic;
        }

        /// <summary>
        /// 移除死亡的事件
        /// </summary>
        public  void RemoveDeadHandle()
        {
            var enumerator = m_EventDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                for(int i= current.Value.Count - 1; i >= 0; i--)
                {
                    var list = current.Value;
                    if (list[i]!=null && list[i].m_CanRemove)
                    {
                        list[i].m_Fn = null;
                        list.RemoveAt(i);
                    }
                }
            }
        }
    }
}
