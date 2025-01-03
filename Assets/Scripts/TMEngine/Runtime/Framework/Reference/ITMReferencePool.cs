using System;

namespace Tenmove.Runtime
{
    public interface ITMReferencePool<T> where T: Referable
    {
        /// <summary>
        /// 获取对象池名称。
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 获取对象池对象类型。
        /// </summary>
        Type ObjectType
        {
            get;
        }

        /// <summary>
        /// 获取对象池中对象的数量。
        /// </summary>
        int ObjectCount
        {
            get;
        }

        /// <summary>
        /// 获取对象池中可释放的对象数量。
        /// </summary>
        int CanReleasedCount
        {
            get;
        }

        /// <summary>
        /// 获取或设置对象池对象过期秒数。
        /// </summary>
        float ExpireTime
        {
            get;
        }

        /// <summary>
        /// 获取或设置对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float AutoPurgeInterval
        {
            get;
        }

        /// <summary>
        /// 获取或设置对象池的优先级。
        /// </summary>
        int Priority
        {
            get;
        }

        int Capacity
        {
            get;
        }

        void Register(T obj,bool withSpawn);

        bool CanSpawn(string objectKey);

        T Spawn(string objectKey);

        void Unspawn(T obj);

        void Unspawn(string objectKey);

        void Lock(string objectKey, bool bLock);

        void SetPriority(int priority);

        void SetExpireTime(float seconds);

        void SetCapacity(int capacity);

        void SetAutoPurgeInterval(float autoPurgeInterval);

        //void Release(string objectKey, bool releaseAll);

        void PurgePool(ref float timeSlice);

        void ReleaseUnusedObject(bool releaseAll);

        T Peek(string objectKey);

        ObjectDesc[] GetAllObjectInfos();
    }
}

