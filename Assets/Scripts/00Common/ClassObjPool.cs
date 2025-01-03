using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledClassObj : ObjectLeakDitector
{
    virtual public void OnReused() { }
    virtual public void OnRecycle() { }
}


public class ClassObjListPool<T> : IObjectPool where T : PooledClassObj,new ()
{
    List<T> m_ClassObjectListPool = new List<T>();

    #region poolInfo

    string poolKey = "Invalid";
    string poolName = "UnknownPool";

    int totalInst = 0;
    int remainInst = 0;

    public string GetPoolName()
    {
        return poolName;
    }

    public string GetPoolInfo()
    {
        return string.Format("{0}/{1}", remainInst, totalInst);
    }

    public string GetPoolDetailInfo()
    {
        return "detailInfo";
    }
    #endregion

    public virtual void Init()
    {
        poolKey = GetType().ToString();
        poolName = string.Format("{0}Pool", typeof(T).ToString());

#if !SERVER_LOGIC
        CPoolManager.GetInstance().RegisterPool(poolKey, this);
#endif
    }

    public T GetPooledObject()
    {
        T newPooledObject = null;
        if (m_ClassObjectListPool.Count > 0)
        {
            int lstIdx = m_ClassObjectListPool.Count - 1;
            newPooledObject = m_ClassObjectListPool[lstIdx];
            m_ClassObjectListPool.RemoveAt(lstIdx);

            newPooledObject.OnReused();

            return newPooledObject;
        }

        return new T();
    }

    public void RecyclePooledObject(T obj)
    {
        obj.OnRecycle();
        m_ClassObjectListPool.Add(obj);
    }
}
