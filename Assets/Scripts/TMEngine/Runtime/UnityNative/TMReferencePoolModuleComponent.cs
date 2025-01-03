using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tenmove/ReferencePoolModule")]
    public class TMReferencePoolModuleComponent : TMModuleComponent
    {
        private ITMReferencePoolManager m_ObjectPoolManager = null;
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected sealed override void Awake()
        {
            base.Awake();

            m_ObjectPoolManager = ModuleManager.GetModule<ITMReferencePoolManager>();
            if (m_ObjectPoolManager == null)
            {
                Debugger.LogError("Object pool manager is invalid!");
                return;
            }
        }

        private void Start()
        {

        }

        /// <summary>
        /// 获取对象池数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_ObjectPoolManager.Count;
            }
        }

        /// <summary>
        /// 检查是否存在对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>是否存在对象池。</returns>
        public bool HasObjectPool<T>() where T : Referable
        {
            return m_ObjectPoolManager.HasReferencePool<T>();
        }


        /// <summary>
        /// 检查是否存在对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <returns>是否存在对象池。</returns>
        public bool HasObjectPool<T>(string name) where T : Referable
        {
            return m_ObjectPoolManager.HasReferencePool<T>(name);
        }

        /// <summary>
        /// 获取对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>要获取的对象池。</returns>
        public ITMReferencePool<T> GetObjectPool<T>() where T : Referable
        {
            return m_ObjectPoolManager.GetReferencePool<T>();
        }

        /// <summary>
        /// 获取对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <returns>要获取的对象池。</returns>
        public ITMReferencePool<T> GetObjectPool<T>(string name) where T : Referable
        {
            return m_ObjectPoolManager.GetReferencePool<T>(name);
        }


        /// <summary>
        /// 获取所有对象池。
        /// </summary>
        public void GetAllObjectPools(List<ReferencePoolBase> objectPools)
        {
            m_ObjectPoolManager.GetAllReferencePools(objectPools);
        }

        /// <summary>
        /// 获取所有对象池。
        /// </summary>
        /// <param name="sort">是否根据对象池的优先级排序。</param>
        /// <returns>所有对象池。</returns>
        public void GetAllObjectPools(List<ReferencePoolBase> objectPools, bool sort)
        {
            m_ObjectPoolManager.GetAllReferencePools(objectPools, sort);
        }

        /// <summary>
        /// 创建允许单次获取的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <returns>要创建的允许单次获取的对象池。</returns>
        public ITMReferencePool<T> CreateSingleSpawnObjectPool<T>(string name) where T : Referable
        {
            return m_ObjectPoolManager.CreateSingleSpawnReferencePool<T>(name);
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
        public ITMReferencePool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : Referable
        {
            return m_ObjectPoolManager.CreateSingleSpawnReferencePool<T>(name, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">对象池名称。</param>
        /// <returns>要创建的允许多次获取的对象池。</returns>
        public ITMReferencePool<T> CreateMultiSpawnObjectPool<T>(string name) where T : Referable
        {
            return m_ObjectPoolManager.CreateMultiSpawnReferencePool<T>(name);
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
        public ITMReferencePool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : Referable
        {
            return m_ObjectPoolManager.CreateMultiSpawnReferencePool<T>(name, capacity, expireTime, priority);
        }

        /// <summary>
        /// 销毁对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="name">要销毁的对象池名称。</param>
        /// <returns>是否销毁对象池成功。</returns>
        public bool DestroyObjectPool<T>(string name) where T : Referable
        {
            return m_ObjectPoolManager.DestroyReferencePool<T>(name);
        }

        /// <summary>
        /// 销毁对象池。
        /// </summary>
        /// <param name="objectPool">要销毁的对象池。</param>
        /// <returns>是否销毁对象池成功。</returns>
        public bool DestroyObjectPool(ReferencePoolBase objectPool)
        {
            return m_ObjectPoolManager.DestroyReferencePool(objectPool);
        }

        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        public void Release()
        {
            Debugger.LogInfo("Object pool release...");
            m_ObjectPoolManager.Release();
        }

        /// <summary>
        /// 释放对象池中的所有未使用对象。
        /// </summary>
        public void ReleaseAllUnused()
        {
            Debugger.LogInfo("Object pool release all unused...");
            m_ObjectPoolManager.ReleaseAllUnused();
        }

    }
}

