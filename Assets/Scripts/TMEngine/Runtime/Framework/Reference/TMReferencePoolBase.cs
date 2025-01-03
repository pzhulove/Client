using System;

namespace Tenmove.Runtime
{
    public abstract class ReferencePoolBase
    {
        private readonly string m_Name;

        /// <summary>
        /// 初始化对象池基类的新实例。
        /// </summary>
        public ReferencePoolBase()
            : this("Tenmove Object Pool:Unnamed")
        {

        }

        /// <summary>
        /// 初始化对象池基类的新实例。
        /// </summary>
        /// <param name="name">对象池名称。</param>
        public ReferencePoolBase(string name)
        {
            m_Name = name ?? string.Empty;
        }

        /// <summary>
        /// 获取对象池名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 获取对象池对象类型。
        /// </summary>
        public abstract Type ObjectType
        {
            get;
        }

        /// <summary>
        /// 获取对象池中对象的数量。
        /// </summary>
        public abstract int ObjectCount
        {
            get;
        }

        /// <summary>
        /// 获取或设置对象池的容量。
        /// </summary>
        public abstract int Capacity
        {
            get;
        }

        /// <summary>
        /// 获取或设置对象池对象过期秒数。
        /// </summary>
        public abstract float ExpireTime
        {
            get;
        }

        /// <summary>
        /// 获取对象池中能被释放的对象的数量。
        /// </summary>
        public abstract int CanReleasedCount
        {
            get;
        }

        /// <summary>
        /// 获取是否允许对象被多次获取。
        /// </summary>
        public abstract bool AllowMultiRef
        {
            get;
        }

        /// <summary>
        /// 获取或设置对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public abstract float AutoPurgeInterval
        {
            get;
        }

        /// <summary>
        /// 获取或设置对象池的优先级。
        /// </summary>
        public abstract int Priority
        {
            get;
        }


        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        /// <param name="timeSlice">执行释放的时间片。</param>
        public abstract void PurgePool(ref float timeSlice);

        /// <summary>
        /// 释放对象池中的所有未使用对象。
        /// </summary>
        /// <param name="releaseAll">释放所有未使用的对象。</param>
        public abstract void ReleaseUnusedObject(bool releaseAll);

        internal abstract void Update(float elapseSeconds, float realElapseSeconds);

        internal abstract void Shutdown();

        /// <summary>
        /// 获取所有对象信息。
        /// </summary>
        /// <returns>所有对象信息。</returns>
        public abstract ObjectDesc[] GetAllObjectInfos();

    }
}

