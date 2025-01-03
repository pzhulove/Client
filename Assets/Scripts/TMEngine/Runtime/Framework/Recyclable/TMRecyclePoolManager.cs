
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class RecyclePoolManager : BaseModule, ITMRecyclePoolManager
    {
        private class RecyclePoolDesc
        {
            public RecyclePoolDesc(RecyclePoolBase pool,Type objType,Type poolType)
            {
                m_RecyclePool = pool;
                m_RecyclableType = objType;
                m_RecyclePoolType = poolType;
            }

            public RecyclePoolBase m_RecyclePool;
            public Type m_RecyclableType;
            public Type m_RecyclePoolType;
        }

        public const int CONST_INVALID_POOL_ID = ~0;

        private readonly LinkedList<RecyclePoolDesc> m_RecyclePoolList;
        private int m_RecyclePoolAllocCount = 0;

        public RecyclePoolManager()
        {
            m_RecyclePoolList = new LinkedList<RecyclePoolDesc>();
        }

        internal override int Priority
        {
            get { return 0; }
        }

        public RecyclePoolBase CreateRecyclePool<T>(CreateRecyclable onCreateAction) where T : Recyclable, new()
        {
            Type recyclableType = typeof(T);
            RecyclePoolBase dstPool = new RecyclePool<T>(m_RecyclePoolAllocCount++,onCreateAction);
            m_RecyclePoolList.AddLast(new RecyclePoolDesc(dstPool as RecyclePoolBase, recyclableType, dstPool.GetType()));
            return dstPool;
        }

        public void DestroyRecyclePool(RecyclePoolBase objPoolBase)
        {
            if (null == objPoolBase)
            {
                Debugger.LogWarning("Pool instance can not be null!");
                return;
            }

            LinkedListNode<RecyclePoolDesc> cur = m_RecyclePoolList.First;
            while (null != cur)
            {
                RecyclePoolDesc curDesc = cur.Value;
                if (curDesc.m_RecyclePool.PoolID == objPoolBase.PoolID)
                {
                    curDesc.m_RecyclePool.Shutdown();
                    m_RecyclePoolList.Remove(cur);
                    return;
                }

                cur = cur.Next;
            }

            Debugger.AssertFailed(string.Format("Can not find pool instance of specific pool!(ID:{0})", objPoolBase.PoolID));
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
            LinkedListNode<RecyclePoolDesc> cur = m_RecyclePoolList.First;
            LinkedListNode<RecyclePoolDesc> next;
            while (null != cur)
            {
                RecyclePoolDesc curDesc = cur.Value;
                next = cur.Next;

                curDesc.m_RecyclePool.Shutdown();
                m_RecyclePoolList.Remove(cur);

                cur = next;
            }
        }

        private RecyclePoolBase _GetRecyclePool<T>(int poolID, CreateRecyclable onCreateAction) where T : Recyclable, new()
        {
            RecyclePoolBase dstPool = null;
            Type recyclableType = typeof(T);
            LinkedListNode<RecyclePoolDesc> cur = m_RecyclePoolList.First;
            while (null != cur)
            {
                RecyclePoolDesc curDesc = cur.Value;
                if (curDesc.m_RecyclableType == recyclableType)
                {
                    if (CONST_INVALID_POOL_ID == poolID || curDesc.m_RecyclePool.PoolID == poolID)
                    {
                        dstPool = curDesc.m_RecyclePool;
                        if (null == dstPool)
                        {
                            Debugger.LogError("Recycle pool type cast error! Required type:'{0}' Source type:'{1}'!", recyclableType.Name, dstPool.GetType().Name);
                            return null;
                        }
                        return dstPool;
                    }
                }

                cur = cur.Next;
            }

            if (CONST_INVALID_POOL_ID == poolID)
            {
                return CreateRecyclePool<T>(onCreateAction);
            }
            else
            {
                Debugger.LogError("Can not find recycle pool with ID '{0}' type '{1}'!", poolID, dstPool.GetType().Name);
                return null;
            }
        }
    }

}

