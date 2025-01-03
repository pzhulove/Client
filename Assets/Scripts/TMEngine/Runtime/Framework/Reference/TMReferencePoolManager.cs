using System.Collections.Generic;

namespace Tenmove.Runtime
{
    /// <summary>
    /// 对象池管理器。
    /// </summary>
    internal partial class ReferencePoolManager : BaseModule,ITMReferencePoolManager
    {
        private const int DefaultCapacity = int.MaxValue;
        private const float DefaultExpireTime = float.MaxValue;

        // 默认优先级为0，释放时最先释放
        private const int DefaultPriority = 0;

        private readonly Dictionary<string, ReferencePoolBase> m_ObjectPools;
        private List<ReferencePoolBase> m_TempSortedObjectPool;
        private float m_PerframeReleaseTime = 10.0f; // 每帧释放资源时间限制在10ms

        /// <summary>
        /// 初始化对象池管理器的新实例。
        /// </summary>
        public ReferencePoolManager()
        {
            m_ObjectPools = new Dictionary<string, ReferencePoolBase>();
            m_TempSortedObjectPool = new List<ReferencePoolBase>(5);
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal override int Priority
        {
            get
            {
                return 90;
            }
        }

        /// <summary>
        /// 获取对象池数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_ObjectPools.Count;
            }
        }

        /// <summary>
        /// 对象池管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal sealed override void Update(float elapseSeconds, float realElapseSeconds)
        {
            float fRemainingTime = m_PerframeReleaseTime;
            Dictionary<string, ReferencePoolBase>.Enumerator itr = m_ObjectPools.GetEnumerator();
            while (itr.MoveNext())
            {
                itr.Current.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理对象池管理器。
        /// </summary>
        internal sealed override void Shutdown()
        {
            foreach (KeyValuePair<string, ReferencePoolBase> objectPool in m_ObjectPools)
            {
                objectPool.Value.Shutdown();
            }

            m_ObjectPools.Clear();
        }

        /// <summary>
        /// 检查是否存在对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>是否存在对象池。</returns>
        public bool HasReferencePool<T>() where T : Referable
        {
            return InternalHasObjectPool(Utility.Text.GetNameWithType<T>(string.Empty));
        }

        /// <summary>
        /// 检查是否存在对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <returns>是否存在对象池。</returns>
        public bool HasReferencePool<T>(string name) where T : Referable
        {
            return InternalHasObjectPool(Utility.Text.GetNameWithType<T>(name));
        }

        /// <summary>
        /// 获取对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>要获取的对象池。</returns>
        public ITMReferencePool<T> GetReferencePool<T>() where T : Referable
        {
            return (ITMReferencePool<T>)InternelGetObjectPool(Utility.Text.GetNameWithType<T>(string.Empty));
        }

        /// <summary>
        /// 获取对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <returns>要获取的对象池。</returns>
        public ITMReferencePool<T> GetReferencePool<T>(string name) where T : Referable
        {
            return (ITMReferencePool<T>)InternelGetObjectPool(Utility.Text.GetNameWithType<T>(name));
        }

        /// <summary>
        /// 获取所有对象池。
        /// </summary>
        /// <returns>所有对象池。</returns>
        public void GetAllReferencePools(List<ReferencePoolBase> objectPools)
        {
            GetAllReferencePools(objectPools, false);
        }

        /// <summary>
        /// 获取所有对象池。
        /// </summary>
        /// <param name="sort">是否根据对象池的优先级排序。</param>
        /// <returns>所有对象池。</returns>
        public void GetAllReferencePools(List<ReferencePoolBase> objectPools, bool sort)
        {
            objectPools.Clear();
            objectPools.AddRange(m_ObjectPools.Values);
            if (sort)
            {
                objectPools.Sort(ObjectPoolComparer);
            }
        }

        /// <summary>
        /// 创建允许单次获取的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <returns>要创建的允许单次获取的对象池。</returns>
        public ITMReferencePool<T> CreateSingleSpawnReferencePool<T>(string name) where T : Referable
        {
            return InternalCreateObjectPool<T>(name, false, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <param name="capacity">对象池的容量。</param>
        /// <param name="expireTime">对象池对象过期秒数。</param>
        /// <param name="priority">对象池的优先级。</param>
        /// <returns>要创建的允许单次获取的对象池。</returns>
        public ITMReferencePool<T> CreateSingleSpawnReferencePool<T>(string name, int capacity, float expireTime, int priority) where T : Referable
        {
            return InternalCreateObjectPool<T>(name, false, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <returns>要创建的允许多次获取的对象池。</returns>
        public ITMReferencePool<T> CreateMultiSpawnReferencePool<T>(string name) where T : Referable
        {
            return InternalCreateObjectPool<T>(name, true, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <param name="capacity">对象池的容量。</param>
        /// <param name="expireTime">对象池对象过期秒数。</param>
        /// <param name="priority">对象池的优先级。</param>
        /// <returns>要创建的允许多次获取的对象池。</returns>
        public ITMReferencePool<T> CreateMultiSpawnReferencePool<T>(string name, int capacity, float expireTime, int priority) where T : Referable
        {
            return InternalCreateObjectPool<T>(name, true, capacity, expireTime, priority);
        }

        /// <summary>
        /// 销毁对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">要销毁的对象池名称。</param>
        /// <returns>是否销毁对象池成功。</returns>
        public bool DestroyReferencePool<T>(string name) where T : Referable
        {
            return InternalDestroyObjectPool(Utility.Text.GetNameWithType<T>(name));
        }

        /// <summary>
        /// 销毁对象池。
        /// </summary>
        /// <param name="objectPool">要销毁的对象池。</param>
        /// <returns>是否销毁对象池成功。</returns>
        public bool DestroyReferencePool(ReferencePoolBase objectPool)
        {
            if (objectPool == null)
            {
                throw new EngineException("Object pool is invalid.");
            }

            return InternalDestroyObjectPool(Utility.Text.GetNameWithType(objectPool.ObjectType, objectPool.Name));
        }

        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        public void Release()
        {
            float fRemainingTime = m_PerframeReleaseTime;
            // Release时是按照Priority由低到高顺序释放的，AssetInstance优先级最低，然后Asset，最后AssetBundle。
            GetAllReferencePools(m_TempSortedObjectPool, true);
            for (int i = 0; i < m_TempSortedObjectPool.Count; ++i)
            {
                m_TempSortedObjectPool[i].PurgePool(ref fRemainingTime);
            }
            m_TempSortedObjectPool.Clear();
        }

        /// <summary>
        /// 释放对象池中的所有未使用对象。
        /// </summary>
        public void ReleaseAllUnused()
        {
            GetAllReferencePools(m_TempSortedObjectPool, true);
            for (int i = 0; i < m_TempSortedObjectPool.Count; ++i)
            {
                m_TempSortedObjectPool[i].ReleaseUnusedObject(true);
            }
            m_TempSortedObjectPool.Clear();
        }

        private bool InternalHasObjectPool(string fullName)
        {
            return m_ObjectPools.ContainsKey(fullName);
        }

        private ReferencePoolBase InternelGetObjectPool(string fullName)
        {
            ReferencePoolBase objectPool = null;
            if (m_ObjectPools.TryGetValue(fullName, out objectPool))
            {
                return objectPool;
            }

            return null;
        }

        private ITMReferencePool<T> InternalCreateObjectPool<T>(string name, bool allowMultiSpawn, int capacity, float expireTime, int priority) where T : Referable
        {
            if (HasReferencePool<T>(name))
            {
                throw new EngineException(string.Format("Already exist object pool '{0}'.", Utility.Text.GetNameWithType<T>(name)));
            }

            ReferencePool<T> objectPool = new ReferencePool<T>(name, allowMultiSpawn, Utility.Time.SecondsToTicks( expireTime), priority,capacity,300);
            m_ObjectPools.Add(Utility.Text.GetNameWithType<T>(name), objectPool);
            return objectPool;
        }

        private bool InternalDestroyObjectPool(string fullName)
        {
            ReferencePoolBase objectPool = null;
            if (m_ObjectPools.TryGetValue(fullName, out objectPool))
            {
                objectPool.Shutdown();
                return m_ObjectPools.Remove(fullName);
            }

            return false;
        }

        private int ObjectPoolComparer(ReferencePoolBase a, ReferencePoolBase b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

    }
}

