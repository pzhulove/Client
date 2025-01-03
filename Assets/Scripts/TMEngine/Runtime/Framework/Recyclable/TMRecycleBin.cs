
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class RecycleBin : BaseModule, ITMRecycleBin
    {
        private readonly Dictionary<Type, RecyclePoolBase> m_RecycablePoolTable;
        private readonly ITMRecyclePoolManager m_RecycablePoolManager;

        private Type m_CachedObjectType = null;
        private RecyclePoolBase m_CachedObjectPool = null;

        public RecycleBin()
        {
            ITMRecyclePoolManager recycablePoolManager = ModuleManager.GetModule<ITMRecyclePoolManager>();
            if (null == recycablePoolManager)
                Debugger.AssertFailed( "Recyclable pool manager can not be null!");

            m_RecycablePoolManager = recycablePoolManager;
            m_RecycablePoolTable = new Dictionary<Type, RecyclePoolBase>();
        }

        internal sealed override int Priority
        {
            get { return 0; }
        }

        internal sealed override void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        /// <summary>
        /// 关闭回收站，将清理所有的对象
        /// </summary>
        internal sealed override void Shutdown()
        {
            Dictionary<Type, RecyclePoolBase>.Enumerator it = m_RecycablePoolTable.GetEnumerator();
            while (it.MoveNext())
            {
                RecyclePoolBase curPool = it.Current.Value;
                if (null != curPool)
                    m_RecycablePoolManager.DestroyRecyclePool(curPool);
            }

            m_RecycablePoolTable.Clear();
            m_CachedObjectType = null;
            m_CachedObjectPool = null;
        }

        /// <summary>
        /// 设置指定类型的可回收对象保留个数
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <param name="reserveCount">保留个数</param>
        /// <returns></returns>
        public void SetReserveCountOfType(Type type, int reserveCount)
        {
            if(null == type)
            {
                Debugger.LogWarning("Type can not be null!");
                return;
            }

            if(type == m_CachedObjectType)
            {
                m_CachedObjectPool.SetReserveCount(reserveCount);
                return;
            }

            RecyclePoolBase dstPool = null;
            if(m_RecycablePoolTable.TryGetValue(type,out dstPool))
            {
                m_CachedObjectType = type;
                m_CachedObjectPool = dstPool;
                dstPool.SetReserveCount(reserveCount);
                return;
            }

            Debugger.LogWarning("Can not find pool for type '{0}'!", type.Name);
        }

        /// <summary>
        /// 获取指定类型的可回收对象保留个数
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns>指定类型的对象保留个数</returns>
        public int GetResserveCountOfType(Type type)
        {
            if (null == type)
            {
                Debugger.LogWarning("Type can not be null!");
                return 0;
            }

            if (type == m_CachedObjectType)
                return m_CachedObjectPool.ReserveCount;

            RecyclePoolBase dstPool = null;
            if (m_RecycablePoolTable.TryGetValue(type, out dstPool))
            {
                m_CachedObjectType = type;
                m_CachedObjectPool = dstPool;

                return dstPool.RecycleCount;
            }

            Debugger.LogWarning("Can not find pool for type '{0}'!", type.Name);
            return 0;
        }

        /// <summary>
        /// 申请对象
        /// </summary>
        /// <returns>返回申请的对象</returns>
        public T Acquire<T>() where T : Recyclable, new()
        {
            Type dstType = typeof(T);
            RecyclePoolBase dstPool = null;

            if (dstType == m_CachedObjectType && null != m_CachedObjectPool)
            {
                return m_CachedObjectPool.QureyInterface<ITMRecyclePool<T>>().Acquire();
            }

            if (m_RecycablePoolTable.TryGetValue(dstType, out dstPool))
            {
                m_CachedObjectType = dstType;
                m_CachedObjectPool = dstPool;

                return dstPool.QureyInterface<ITMRecyclePool<T>>().Acquire();
            }

            dstPool = m_RecycablePoolManager.CreateRecyclePool<T>(null);
            if (null != dstPool)
            {
                m_CachedObjectType = dstType;
                m_CachedObjectPool = dstPool;

                m_RecycablePoolTable.Add(dstType, dstPool);

                return dstPool.QureyInterface<ITMRecyclePool<T>>().Acquire();
            }
            else
                Debugger.LogError("Create recyclable object pool has failed!");

            return null;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">要被回收的对象</param>
        public void Recycle<T>(T obj) where T : Recyclable, new()
        {
            Type dstType = typeof(T);
            if (dstType == m_CachedObjectType)
            {
                m_CachedObjectPool.QureyInterface<ITMRecyclePool<T>>().Recycle(obj);
                return;
            }

            RecyclePoolBase dstPool = null;
            if (m_RecycablePoolTable.TryGetValue(dstType, out dstPool))
            {
                m_CachedObjectType = dstType;
                m_CachedObjectPool = dstPool;

                dstPool.QureyInterface<ITMRecyclePool<T>>().Recycle(obj);
                return;
            }

            Debugger.AssertFailed(string.Format("Can not find recyclable object pool with type '{0}'!", dstType.Name));
        }

        /// <summary>
        /// 清理所有指定类型的对象
        /// </summary>
        /// <param name="type">要清理对象的类型</param>
        public void ClearAllObjectOfType<T>() where T : Recyclable,new()
        {
            Type dstType = typeof(T);

            if (dstType == m_CachedObjectType)
            {
                m_CachedObjectPool.PurgePool(true);
                return;
            }

            RecyclePoolBase dstPool = null;
            if (m_RecycablePoolTable.TryGetValue(dstType, out dstPool))
            {
                m_CachedObjectType = dstType;
                m_CachedObjectPool = dstPool;

                dstPool.PurgePool(true);
                return;
            }

            Debugger.AssertFailed(string.Format("Can not find recyclable object pool with type '{0}'!", dstType.Name));
        }

        /// <summary>
        /// 清洗池子
        /// </summary>
        /// <param name="clearAll">是否清理所有对象，如果是则清理所有，如果为否则保留Reserve所指定的对象个数</param>
        public void Purge(bool clearAll)
        {
            Dictionary<Type, RecyclePoolBase>.Enumerator it = m_RecycablePoolTable.GetEnumerator();
            while(it.MoveNext())
            {
                RecyclePoolBase curPool = it.Current.Value;
                if (null != curPool)
                    curPool.PurgePool(clearAll);
            }
        }
    }

}

