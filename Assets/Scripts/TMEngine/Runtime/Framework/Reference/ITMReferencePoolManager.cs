using System.Collections.Generic;


namespace Tenmove.Runtime
{
    /// <summary>
    /// 引用池管理器。
    /// </summary>
    public interface ITMReferencePoolManager
    {
        /// <summary>
        /// 获取引用池数量。
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// 检查是否存在引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <returns>是否存在引用池。</returns>
        bool HasReferencePool<T>() where T : Referable;

        /// <summary>
        /// 检查是否存在引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <param name="name">引用池名称。</param>
        /// <returns>是否存在引用池。</returns>
        bool HasReferencePool<T>(string name) where T : Referable;

        /// <summary>
        /// 获取引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <returns>要获取的引用池。</returns>
        ITMReferencePool<T> GetReferencePool<T>() where T : Referable;

        /// <summary>
        /// 获取引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <param name="name">引用池名称。</param>
        /// <returns>要获取的引用池。</returns>
        ITMReferencePool<T> GetReferencePool<T>(string name) where T : Referable;

        /// <summary>
        /// 获取所有引用池。
        /// </summary>
        /// <returns>所有引用池。</returns>
        void GetAllReferencePools(List<ReferencePoolBase> objectPools);

        /// <summary>
        /// 获取所有引用池。
        /// </summary>
        /// <param name="sort">是否根据引用池的优先级排序。</param>
        /// <returns>所有引用池。</returns>
        void GetAllReferencePools(List<ReferencePoolBase> objectPools, bool sort);

        // <summary>
        /// 创建允许单次获取的引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <param name="name">引用池名称。</param>
        /// <returns>要创建的允许单次获取的引用池。</returns>
        ITMReferencePool<T> CreateSingleSpawnReferencePool<T>(string name) where T : Referable;

        /// <summary>
        /// 创建允许单次获取的引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <param name="name">引用池名称。</param>
        /// <param name="capacity">引用池的容量。</param>
        /// <param name="expireTime">引用池对象过期秒数。</param>
        /// <param name="priority">引用池的优先级。</param>
        /// <returns>要创建的允许单次获取的引用池。</returns>
        ITMReferencePool<T> CreateSingleSpawnReferencePool<T>(string name, int capacity, float expireTime, int priority) where T : Referable;

        /// <summary>
        /// 创建允许多次获取的引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <returns>要创建的允许多次获取的引用池。</returns>
        ITMReferencePool<T> CreateMultiSpawnReferencePool<T>(string name) where T : Referable;

        /// <summary>
        /// 创建允许多次获取的引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <param name="name">引用池名称。</param>
        /// <param name="capacity">引用池的容量。</param>
        /// <param name="expireTime">引用池对象过期秒数。</param>
        /// <param name="priority">引用池的优先级。</param>
        /// <returns>要创建的允许多次获取的引用池。</returns>
        ITMReferencePool<T> CreateMultiSpawnReferencePool<T>(string name, int capacity, float expireTime, int priority) where T : Referable;

        /// <summary>
        /// 销毁引用池。
        /// </summary>
        /// <typeparam name="T">对象类型。<peparam>
        /// <param name="name">要销毁的引用池名称。</param>
        /// <returns>是否销毁引用池成功。</returns>
        bool DestroyReferencePool<T>(string name) where T : Referable;

        /// <summary>
        /// 销毁引用池。
        /// </summary>
        /// <param name="objectPool">要销毁的引用池。</param>
        /// <returns>是否销毁引用池成功。</returns>
        bool DestroyReferencePool(ReferencePoolBase objectPool);

        /// <summary>
        /// 释放引用池中的可释放对象。
        /// </summary>
        void Release();

        /// <summary>
        /// 释放引用池中的所有未使用对象。
        /// </summary>
        void ReleaseAllUnused();
    }
}

