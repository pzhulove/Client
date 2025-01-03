using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class RecyclePoolManager
    {
        private partial class RecyclePool<T> : RecyclePoolBase, ITMRecyclePool<T> where T : Recyclable, new()
        {
            private readonly Queue<T> m_RecyclableObjects;
            private readonly CreateRecyclable m_CreateAction;

            public RecyclePool(int poolID, CreateRecyclable createAction, int reserveCount = 3)
                : base(poolID,reserveCount)
            {
                m_RecyclableObjects = new Queue<T>();
                m_CreateAction = createAction;
            }

            public override sealed int UnusedObjectCount
            {
                get { return m_RecyclableObjects.Count; }
            }

            public T Acquire()
            {
                T obj = _AquireRecycledObject();
                if (null != obj)
                {
                    Debugger.Assert(obj.IsRecycled, "Recyclable object '{0}' recycle state [Unrecycled] is wrong!", typeof(T));
                    obj.OnReuse();
                    return obj;
                }

                ++m_CreateCount;

                if (null != m_CreateAction)
                    obj = m_CreateAction() as T;
                else
                    obj = new T();

                if(null != obj)
                    obj.OnCreate();

                return obj;
            }

            public void Recycle(T obj)
            {
                if(null == obj)
                {
                    Debugger.LogWarning("Recycle object can not be null!");
                    return;
                }

                if (typeof(T) != obj.GetType())
                {
                    Debugger.LogWarning("Recycle object type '{0}' miss-match pool type '{1}'!", obj.GetType(), typeof(T));
                    return;
                }

                if (!obj.IsRecycled)
                {
                    --m_UsingObjectCount;
                    ++m_RecycleCount;
                    obj.OnRecycle();
                    lock (m_RecyclableObjects)
                    {
                        m_RecyclableObjects.Enqueue(obj as T);
                    }
                }
                else
                    Debugger.LogWarning("Recyclable object '{0}' recycle state [Recycled] is wrong! (Maybe it has been recycled twice)", typeof(T));
            }

            public sealed override void PurgePool(bool clearAll)
            {
                int purgeCount = 0;
                lock (m_RecyclableObjects)
                    purgeCount = m_RecyclableObjects.Count;
                purgeCount -= (clearAll ? 0 : m_ReserveCount);
                purgeCount = purgeCount < 0 ? 0 : purgeCount;

                _ReleaseUnusedObject(purgeCount);
            }

            public sealed override I QureyInterface<I>()
            {
                return this as I;
            }

            public sealed override void Shutdown()
            {
                PurgePool(true);
            }

            private T _AquireRecycledObject()
            {
                ++m_UsingObjectCount;
                ++m_AcquireCount;

                lock (m_RecyclableObjects)
                {
                    if (m_RecyclableObjects.Count > 0)
                    {
                        return m_RecyclableObjects.Dequeue();
                    }
                }

                return null;
            }

            private void _ReleaseUnusedObject(int releaseCount)
            {
                lock(m_RecyclableObjects)
                {
                    if (releaseCount > m_RecyclableObjects.Count)
                        releaseCount = m_RecyclableObjects.Count;

                    m_ReleaseCount += releaseCount;
                    while(releaseCount-- > 0)
                    {
                        T obj = m_RecyclableObjects.Dequeue();
                        obj.OnRelease();
                    }
                }
            }
        }
    }

}

