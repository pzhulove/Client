using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePool;

namespace GameClient
{
    public struct EventParamNode : System.IDisposable
    {
        public static EventParamNode New()
        {
            return new EventParamNode(DataStructPool.EventParamPool.Get());
        }

        public BeEvent.BeEventParam Ref { get; private set; }

        public EventParamNode(BeEvent.BeEventParam r)
        {
            Ref = r;
        }

        public void Dispose()
        {
            DataStructPool.EventParamPool.Release(Ref);
            Ref = null;
        }
    }

    /// <summary>
    /// 数据结构池子 避免频繁GC
    /// </summary>
    public static partial class DataStructPool
    {
        /// <summary>
        /// 事件参数池子
        /// </summary>
        public static partial class EventParamPool
        {
            private static readonly ObjectPool<BeEvent.BeEventParam> m_EventPool = new ObjectPool<BeEvent.BeEventParam>(null, null);

            public static BeEvent.BeEventParam Get()
            {
                return m_EventPool.Get();
            }

            public static void Release(BeEvent.BeEventParam eventParam)
            {
                eventParam.Reset();
                m_EventPool.Release(eventParam);
            }
        }

        public static partial class UIEventParamPool
        {
            private static readonly ObjectPool<UIEventNew.UIEventParamNew> m_EventPool = new ObjectPool<UIEventNew.UIEventParamNew>(null, null);

            public static UIEventNew.UIEventParamNew Get()
            {
                return m_EventPool.Get();
            }

            public static void Release(UIEventNew.UIEventParamNew eventParam)
            {
                eventParam.Reset();
                m_EventPool.Release(eventParam);
            }
        }
    }
}
