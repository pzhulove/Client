using GamePool;

namespace GameClient
{
    class FrameMarkDataRunTimePool
    {
        private static readonly ObjectPool<FrameMarkDataRunTime> m_EventPool = new ObjectPool<FrameMarkDataRunTime>(null, null);

        public static FrameMarkDataRunTime Get()
        {
            return m_EventPool.Get();
        }

        public static void Release(FrameMarkDataRunTime inst)
        {
            inst.Recycle();
            m_EventPool.Release(inst);
        }
        public static void Clear()
        {
            m_EventPool.Clear();
        }
    }
    class FrameMarkDataPool
    {
        private static readonly ObjectPool<FrameMarkData> m_EventPool = new ObjectPool<FrameMarkData>(null, null);

        public static FrameMarkData Get()
        {
            return m_EventPool.Get();
        }

        public static void Release(FrameMarkData inst)
        {
            inst.Recycle();
            m_EventPool.Release(inst);
        }
        public static void Clear()
        {
            m_EventPool.Clear();
        }
    }

    class MarkDataRunTimePool
    {
        private static readonly ObjectPool<MarkDataRunTime> m_EventPool = new ObjectPool<MarkDataRunTime>(null, null);

        public static MarkDataRunTime Get()
        {
            return m_EventPool.Get();
        }

        public static void Release(MarkDataRunTime inst)
        {
            inst.Recycle();
            m_EventPool.Release(inst);
        }
        public static void Clear()
        {
            m_EventPool.Clear();
        }
    }
    class MarkDataPool
    {
        private static readonly ObjectPool<MarkData> m_EventPool = new ObjectPool<MarkData>(null, null);

        public static MarkData Get()
        {
            return m_EventPool.Get();
        }

        public static void Release(MarkData inst)
        {
            inst.Recycle();
            m_EventPool.Release(inst);
        }
        public static void Clear()
        {
            m_EventPool.Clear();
        }
    }

    class MarkNodeDataPool
    {
        private static readonly ObjectPool<NodeData> m_EventPool = new ObjectPool<NodeData>(null, null);

        public static NodeData Get()
        {
           return m_EventPool.Get();
        }

        public static void Release(NodeData inst)
        {
            inst.Recycle();
            m_EventPool.Release(inst);
        }
        public static void Clear()
        {
            m_EventPool.Clear();
        }
    }
}
