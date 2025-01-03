using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class ReferencePoolManager
    {
        public class ReferencePool<T> : ReferencePoolBase, ITMReferencePool<T> where T : Referable
        {
            LinkedList<T> m_ObjectList;
            LinkedList<T> m_RemovedCacheList;

            private long m_ExpireTime;
            private int m_Priority;
            private float m_AutoPurgeInterval;
            private float m_AutoPurgeElapsedTime;

            private readonly bool m_AllowMultiRef;
            private int m_Capacity; 

            public ReferencePool(string name,bool allowMultiRef, float expireTimeInSeconds, int priority,int capacity,float autoPurgeInterval)
                : base(name)
            {
                m_ObjectList = new LinkedList<T>();
                m_RemovedCacheList = new LinkedList<T>();

                m_ExpireTime = Utility.Time.SecondsToTicks(expireTimeInSeconds);
                m_AllowMultiRef = allowMultiRef;
                m_Priority = priority;
                m_AutoPurgeInterval = autoPurgeInterval;
                m_AutoPurgeElapsedTime = 0.0f;
                m_Capacity = capacity;
            }

            public override Type ObjectType
            {
                get { return typeof(T); }
            }

            public override int ObjectCount
            {
                get { return m_ObjectList.Count + m_RemovedCacheList.Count; }
            }

            public override int CanReleasedCount
            {
                get
                {
                    return m_RemovedCacheList.Count;
                }
            }

            public override float ExpireTime
            {
                get { return Utility.Time.TicksToSeconds( m_ExpireTime); }
            }

            public override int Capacity
            {
                get { return m_Capacity; }
            }


            /// <summary>
            /// 获取或设置对象池的优先级。
            /// </summary>
            public override int Priority
            {
                get { return m_Priority; }
            }
            /// <summary>
            /// 获取是否允许对象被多次获取。
            /// </summary>
            public override bool AllowMultiRef
            {
                get { return m_AllowMultiRef; }
            }

            /// <summary>
            /// 获取或设置对象池自动释放可释放对象的间隔秒数。
            /// </summary>
            public override float AutoPurgeInterval
            {
                get { return m_AutoPurgeInterval; }
            }

            public void Register(T obj, bool withSpawn)
            {
                if (null == obj)
                    Debugger.AssertFailed("Object can not be null!");
                
                m_ObjectList.AddFirst(obj);
                if (withSpawn)
                    obj.OnSpawn();
            }

            public bool CanSpawn(string objectKey)
            {
                if (string.IsNullOrEmpty(objectKey))
                {
                    Debugger.LogWarning("Object key can not be null or empty string!");
                    return false;
                }

                int keyHashCode = objectKey.GetHashCode();
                T obj;
                LinkedListNode< T> cur = m_RemovedCacheList.First;
                while(null != cur)
                {
                    obj = cur.Value;
                    if(keyHashCode == obj.NameHashCode && objectKey == obj.Name)
                    {
                        return true;
                    }

                    cur = cur.Next;
                }

                if(m_AllowMultiRef)
                {
                    cur = m_ObjectList.First;
                    while (null != cur)
                    {
                        obj = cur.Value;
                        if (keyHashCode == obj.NameHashCode && objectKey == obj.Name)
                        {
                            return true;
                        }

                        cur = cur.Next;
                    }
                }

                return false;
            }

            public T Spawn(string objectKey)
            {
                T obj = _Spawn(objectKey);
                if(null != obj)
                {
                    if(!obj.IsInUse)
                    {
                        _Release(obj);
                        obj = null;
                    }
                }

                return obj;
            }

            private T _Spawn(string objectKey)
            {
                if (string.IsNullOrEmpty(objectKey))
                {
                    Debugger.LogWarning("Object key can not be null or empty string!");
                    return null;
                }

                int keyHashCode = objectKey.GetHashCode();

                T obj;
                LinkedListNode<T> cur = m_RemovedCacheList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    if (keyHashCode == obj.NameHashCode && objectKey == obj.Name)
                    {
                        m_RemovedCacheList.Remove(cur);
                        m_ObjectList.AddFirst(cur);

                        Debugger.LogProcedure(this,"Object pool respawn object named with [{0}] (type:{1})", ObjectType, obj.Name);
                        obj.OnSpawn();
                        return obj;
                    }

                    cur = cur.Next;
                }

                if (m_AllowMultiRef)
                {
                    cur = m_ObjectList.First;
                    while (null != cur)
                    {
                        obj = cur.Value;
                        if (keyHashCode == obj.NameHashCode && objectKey == obj.Name)
                        {
                            Debugger.LogProcedure(this,"Object pool spawn object named with [{0}] (type:{1})", ObjectType, obj.Name);
                            obj.OnSpawn();
                            return obj;
                        }

                        cur = cur.Next;
                    }
                }

                return null;
            }

            public void Lock(string objectKey, bool bLock)
            {
                if (string.IsNullOrEmpty(objectKey))
                {
                    Debugger.LogWarning("Object key can not be null or empty string!");
                    return;
                }

                int keyHashCode = objectKey.GetHashCode();

                T obj;
                LinkedListNode<T> cur = m_RemovedCacheList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    if (keyHashCode == obj.NameHashCode && objectKey == obj.Name)
                    {
                        obj.Lock(bLock);
                        return;
                    }

                    cur = cur.Next;
                }

                if (m_AllowMultiRef)
                {
                    cur = m_ObjectList.First;
                    while (null != cur)
                    {
                        obj = cur.Value;
                        if (keyHashCode == obj.NameHashCode && objectKey == obj.Name)
                        {
                            obj.Lock(bLock);
                            return;
                        }

                        cur = cur.Next;
                    }
                }
            }

            public void Unspawn(T removed)
            {
                if (null == removed)
                    Debugger.AssertFailed("Object can not be null!");

                T obj;
                LinkedListNode<T> cur = m_ObjectList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    if (removed.NameHashCode == obj.NameHashCode && removed.Name == obj.Name)
                    {
                        Debugger.LogProcedure(this,"Object pool unspawn object named with [{0}] (type:{1})", ObjectType, obj.Name);
                        obj.OnUnspawn();

                        if(!obj.IsInUse)
                        {
                            m_ObjectList.Remove(cur);
                            m_RemovedCacheList.AddFirst(cur);
                        }

                        return;
                    }

                    cur = cur.Next;
                }

                Debugger.AssertFailed(string.Format("Can not find target in object pool '{0}'!", removed.Name));
            }

            public void Unspawn(string objectKey)
            {
                T obj;
                LinkedListNode<T> cur = m_ObjectList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    if (objectKey == obj.Name)
                    {
                        Debugger.LogProcedure(this,"Object pool unspawn object named with [{0}] (type:{1})", ObjectType, obj.Name);
                        obj.OnUnspawn();

                        if (!obj.IsInUse)
                        {
                            m_ObjectList.Remove(cur);
                            m_RemovedCacheList.AddFirst(cur);
                        }

                        return;
                    }

                    cur = cur.Next;
                }

                Debugger.AssertFailed(string.Format("Can not find target in object pool '{0}'!", objectKey));
            }

            public void SetPriority(int priority)
            {
                m_Priority = priority;
            }

            public void SetExpireTime(float seconds)
            {
                m_ExpireTime = Utility.Time.SecondsToTicks(seconds);
            }

            public void SetCapacity(int capacity)
            {
                m_Capacity = capacity;
            }

            public void SetAutoPurgeInterval(float autoPurgeInterval)
            {
                m_AutoPurgeInterval = autoPurgeInterval;
            }

            public sealed override void PurgePool(ref float timeSlice)
            {
                _Purge(m_ObjectList.Count + m_RemovedCacheList.Count - m_Capacity, ref timeSlice);
            }

            private void _Purge(int toReleaseCount ,ref float remainTime)
            {
                if (0 == m_RemovedCacheList.Count || remainTime <= 0.0f)
                    return;

                long timeStampExpired = DateTime.Now.Ticks - m_ExpireTime;

                T obj;
                LinkedListNode<T> cur = m_RemovedCacheList.Last;
                LinkedListNode<T> next;
                long beginTime = DateTime.Now.Ticks;
                while (null != cur)
                {
                    obj = cur.Value;
                    next = cur.Previous;
                    if (!obj.IsInUse && !obj.IsLocked)
                    {
                        // 超出Capacity的没用的强制删除，不管有没有超过ExpireTime。没超出Capacity的没用的，超过ExprireTime才会删除。
                        if (obj.LastUseTime < timeStampExpired || toReleaseCount > 0)
                        {
                            m_RemovedCacheList.Remove(cur);
                            obj.OnRelease();
                            --toReleaseCount;

                            long endTime = DateTime.Now.Ticks;
                            remainTime -= Utility.Time.TicksToSeconds(endTime - beginTime);
                            beginTime = endTime;
                            if (remainTime < 0)
                                break;

                            Debugger.LogProcedure(this,"Asset pool release asset named with [{0}] (type:{1})", ObjectType, obj.Name);
                        }
                        else
                        {
                            // m_AssetRemovedList列表是按照时间排序的，前面的LastUseTime更早。所以只需要从前往后迭代链表
                            // 如果LastUseTime比expireTime早，或者Count大于Capacity，就删除。
                            break;
                        }
                    }

                    cur = next;
                }
            }

            public sealed override void ReleaseUnusedObject(bool releaseAll)
            {
                T obj;
                LinkedListNode<T> cur = m_ObjectList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    if (obj.IsInUse || obj.IsLocked)
                    {
                        cur = cur.Next;
                        continue;
                    }

                    LinkedListNode<T> next = cur.Next;
                    m_ObjectList.Remove(cur);

                    cur.Value.OnRelease();
                    Debugger.LogProcedure(this,"Object pool release object named with [{0}] (type:{1})", ObjectType, obj.Name);

                    cur = next;
                }

                cur = m_RemovedCacheList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    if (obj.IsInUse || obj.IsLocked)
                    {
                        cur = cur.Next;
                        continue;
                    }

                    LinkedListNode<T> next = cur.Next;
                    m_RemovedCacheList.Remove(cur);
                    cur.Value.OnRelease();

                    Debugger.LogProcedure(this,"Object pool release object named with [{0}] (type:{1})", ObjectType, obj.Name);
                    cur = next;
                }
            }

            internal sealed override void Update(float elapseSeconds, float realElapseSeconds)
            {
                m_AutoPurgeElapsedTime += realElapseSeconds;
                if (m_AutoPurgeElapsedTime > m_AutoPurgeInterval)
                {
                    m_AutoPurgeElapsedTime = 0;
                    float timeSlice = 0.08f;
                    _Purge(m_ObjectList.Count + m_RemovedCacheList.Count - m_Capacity, ref timeSlice);
                }
            }

            internal sealed override void Shutdown()
            {
                T obj;
                LinkedListNode<T> cur = m_RemovedCacheList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    LinkedListNode<T> next = cur.Next;

                    m_RemovedCacheList.Remove(cur);
                    cur.Value.OnRelease();

                    Debugger.LogProcedure(this,"Object pool force release object named with [{0}] (type:{1})", ObjectType, obj.Name);
                    cur = cur.Next;
                }

                cur = m_ObjectList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    LinkedListNode<T> next = cur.Next;

                    m_ObjectList.Remove(cur);
                    cur.Value.OnRelease();

                    Debugger.LogProcedure(this,"Object pool force release object named with [{0}] (type:{1})", ObjectType, obj.Name);
                    cur = cur.Next;
                }
            }

            public T Peek(string objectKey)
            {
                if (string.IsNullOrEmpty(objectKey))
                {
                    Debugger.LogWarning("Object key can not be null or empty string!");
                    return null;
                }

                int keyHashCode = objectKey.GetHashCode();

                T obj;
                LinkedListNode<T> cur = m_RemovedCacheList.First;
                while (null != cur)
                {
                    obj = cur.Value;
                    if (keyHashCode == obj.NameHashCode && objectKey == obj.Name)
                        return obj;

                    cur = cur.Next;
                }

                if (m_AllowMultiRef)
                {
                    cur = m_ObjectList.First;
                    while (null != cur)
                    {
                        obj = cur.Value;
                        if (keyHashCode == obj.NameHashCode && objectKey == obj.Name)
                            return obj;

                        cur = cur.Next;
                    }
                }

                return null;
            }

            /// <summary>
            /// 获取所有对象信息。
            /// </summary>
            /// <returns>所有对象信息。</returns>
            public override sealed ObjectDesc[] GetAllObjectInfos()
            {
                int idx = 0;
                ObjectDesc[] objectInfos = new ObjectDesc[m_ObjectList.Count + m_RemovedCacheList.Count];
                LinkedListNode<T> cur = m_ObjectList.First;
                while(null != cur)
                {
                    T curObj = cur.Value;
                    objectInfos[idx++] = new Tenmove.Runtime.ObjectDesc(curObj.Name, curObj.IsLocked, curObj.LastUseTime, curObj.SpawnCount, true);
            
                    cur = cur.Next;
                }
            
                cur = m_RemovedCacheList.First;
                while (null != cur)
                {
                    T curObj = cur.Value;
                    objectInfos[idx++] = new Tenmove.Runtime.ObjectDesc(curObj.Name, curObj.IsLocked, curObj.LastUseTime, curObj.SpawnCount, false);
            
                    cur = cur.Next;
                }
            
                return objectInfos;
            }

            private void _Release(T obj)
            {
                T curObj;
                LinkedListNode<T> cur = m_ObjectList.First;
                while(null != cur)
                {
                    curObj = cur.Value;

                    if(curObj.NameHashCode == obj.NameHashCode && curObj.Name == obj.Name)
                    {
                        if (curObj.IsLocked)
                            Debugger.LogWarning("Object pool force release asset named with [{0}] (type:{1})", ObjectType, obj.Name);

                        m_ObjectList.Remove(cur);
                        curObj.OnRelease();
                        Debugger.LogProcedure(this,"Asset pool release asset named with [{0}] (type:{1})", obj.Name, ObjectType);
                        return;
                    }

                    cur = cur.Next;
                }
            }
        }
    }
}

