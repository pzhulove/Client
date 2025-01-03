using System.Collections.Generic;
using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    internal sealed partial class UnityGameObjectPool : BaseModule, ITMUnityGameObjectPool
    {
        private class ObjectPoolDesc
        {
            private GameObjectUsage m_GameObjectUsage;
            private readonly RecyclePoolBase m_GameObjectPool;

            public ObjectPoolDesc(RecyclePoolBase gameObjectPool, GameObjectUsage gameObjectUsage)
            {
                m_GameObjectUsage = gameObjectUsage;
                m_GameObjectPool = gameObjectPool;
            }

            public  void SetGameObjectUsage(GameObjectUsage gameObjectUsage)
            {
                m_GameObjectUsage = gameObjectUsage;
            }

            public GameObjectUsage GameObjectType
            {
                get { return m_GameObjectUsage; }
            }
            public RecyclePoolBase GameObjectPool
            {
                get { return m_GameObjectPool; }
            }
            
        }

        private readonly ITMRecyclePoolManager m_RecyclePoolManager;
        private readonly ITMAssetManager m_AssetManager;
        private GameObject m_GameObjectRoot;
        private uint m_RequestIDCount = 0;
        private readonly uint m_RequestIDType = 1;
        private bool m_IsPurgingAsset;

        private readonly Dictionary<string, ObjectPoolDesc> m_GameObjectPoolSet = new Dictionary<string, ObjectPoolDesc>();
        private readonly Dictionary<int, Object> m_ObjectRevMap = new Dictionary<int, Object>();

        private struct ObjectRequestCache
        {
            public ObjectRequestCache(AssetLoadCallbacks<object> callbacks, string prefabPath,GameObject go, int requestID,float duration,object userData)
            {
                m_Callback = callbacks;
                m_PrefabPath = prefabPath;
                m_GameObject = go;
                m_RequestID = requestID;
                m_Duration = duration;
                m_UserData = userData;
            }

            public AssetLoadCallbacks<object> m_Callback;
            public string m_PrefabPath;
            public GameObject m_GameObject;
            public int m_RequestID;
            public float m_Duration;
            public object m_UserData;
        }

        private Queue<ObjectRequestCache> m_RequestCacheQueue = new Queue<ObjectRequestCache>();

        private class ObjectRequestContext
        {
            public ObjectRequestContext(UnityGameObjectPool gameObjectManager, AssetLoadCallbacks<object> callbacks, string prefabPath, int requestID, object userData)
            {
                m_GameObjectManager = gameObjectManager;
                m_Callback = callbacks;
                m_RequestID = requestID;
                m_UserData = userData;
            }

            public UnityGameObjectPool m_GameObjectManager;
            public AssetLoadCallbacks<object> m_Callback;
            public int m_RequestID;
            public object m_UserData;
        }

        AssetLoadCallbacks<object> m_AssetLoadCallbacks = new AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure, _OnLoadAssetUpdate);

        static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
        {
            if (null == userData)
            {
                Debugger.LogError("User data can not be null when invoke success callback with asset '{0}'!",assetPath);
                return;
            }

            ObjectRequestContext context = userData as ObjectRequestContext;
            if(null == context)
            {
                Debugger.LogError("User data type [{0}] error, expect type[{1}]!",userData.GetType().Name,typeof(ObjectRequestContext).Name);
                return;
            }

            GameObject gameObject = asset as GameObject;
            if(null == gameObject)
            {
                Debugger.LogError("Asset type [{0}] error, expect type[{1}]!", userData.GetType().Name, typeof(GameObject).Name);
                return;
            }

            Object newObject = new Object();
            newObject.Fill(assetPath, gameObject);
            context.m_GameObjectManager._RegiestPoolObject(gameObject.GetInstanceID(), newObject);
            newObject.OnReuse();
            context.m_Callback.OnAssetLoadSuccess(assetPath, gameObject, context.m_RequestID, duration, context.m_UserData);
            return;
        }

        static void _OnLoadAssetFailure(string assetPath,int taskID, AssetLoadErrorCode errorCode, string errorMessage, object userData)
        {
            if (null == userData)
            {
                Debugger.LogError("User data can not be null when invoke success callback with asset '{0}'!", assetPath);
                return;
            }

            ObjectRequestContext context = userData as ObjectRequestContext;
            if (null == context)
            {
                Debugger.LogError("User data type [{0}] error, expect type[{1}]!", userData.GetType().Name, typeof(ObjectRequestContext).Name);
                return;
            }

            context.m_Callback.OnAssetLoadFailure(assetPath,taskID, errorCode, errorMessage, context.m_UserData);
        }

        static void _OnLoadAssetUpdate(string path, int taskID,float progress, object userData)
        {

        }

        public UnityGameObjectPool()
        {
            m_RecyclePoolManager = ModuleManager.GetModule<ITMRecyclePoolManager>();
            m_AssetManager = ModuleManager.GetModule<ITMAssetManager>();
            m_IsPurgingAsset = false;
        }

        /// <summary>
        /// 设置池子根节点
        /// </summary>
        /// <param name="root">设置池子根节点</param>
        public void SetPoolRootNode(GameObject root)
        {
            m_GameObjectRoot = root;
        }

        public void SetObjectPoolDesc(string prefabRes,GameObjectUsage objectUsage, int reserouceCount, float expireTime, int priority)
        {
            ObjectPoolDesc objectPoolDesc = _GetObjectPoolDesc(prefabRes, false) ;
            if(null != objectPoolDesc)
            {
                objectPoolDesc.SetGameObjectUsage(objectUsage);
                objectPoolDesc.GameObjectPool.SetExpireTime(expireTime);
                objectPoolDesc.GameObjectPool.SetPriority(priority);
                objectPoolDesc.GameObjectPool.SetReserveCount(reserouceCount);

                return;
            }

            Debugger.LogWarning("Can not find game object pool with resource-path '{0}'!");
        }

        public GameObject AcquireGameObjectSync(string prefabRes,uint flag)
        {
            if (string.IsNullOrEmpty(prefabRes))
            {
                Debugger.LogError("Prefab resource path can not be null or empty!");
                return null;
            }

            //if(m_IsPurgingAsset)
            //{
            //    Debugger.LogError("Can not acquire game object during GC!");
            //    return null;
            //}

            ObjectPoolDesc objectPoolDesc = _GetObjectPoolDesc(prefabRes, true);

            Object newObject = objectPoolDesc.GameObjectPool.QureyInterface<ITMRecyclePool<Object>>().Acquire();
            if (null != newObject)
            {
                if(newObject.IsValid)
                {
                    newObject.GameObject.SetActive(true);
                    newObject.GameObject.transform.SetParent(null, false);
                    return newObject.GameObject;
                }
                else
                {
                    newObject.OnRelease();
                    _UnregiesterPoolObject(newObject.InstanceID);
                }
            }

            GameObject go = m_AssetManager.LoadAsset(prefabRes, typeof(GameObject), null,flag) as GameObject;
            if(null == go)
            {
                Debugger.LogError("Can not load prefab with path '{0}'!",prefabRes);
                return null;
            }

            newObject = new Object();
            newObject.Fill(prefabRes, go);
            _RegiestPoolObject(newObject.InstanceID, newObject);
            newObject.OnReuse();
            return newObject.GameObject;
        }

        public int AcquireGameObjectAsync(string prefabRes,object userData, AssetLoadCallbacks<object> callbacks, uint flag)
        {
            if(string.IsNullOrEmpty(prefabRes))
            {
                Debugger.LogError("Prefab resource path can not be null or empty!");
                return ~0;
            }

            int requestID = (int)_AllocHandle();
            ObjectPoolDesc objectPoolDesc = _GetObjectPoolDesc(prefabRes, true);
            Object newObject = objectPoolDesc.GameObjectPool.QureyInterface<ITMRecyclePool<Object>>().Acquire();
            if (null != newObject)
            {
                newObject.GameObject.transform.SetParent(null, false);
                m_RequestCacheQueue.Enqueue(new ObjectRequestCache(callbacks, prefabRes, newObject.GameObject, requestID, 0, userData));
                return requestID;
            }

            ObjectRequestContext context = new ObjectRequestContext(this,callbacks,prefabRes, requestID, userData);
            m_AssetManager.LoadAssetAsync(prefabRes, typeof(GameObject), m_AssetLoadCallbacks , context, 0);

            return requestID;
        }

        public void RecycleGameObject(GameObject gameObject)
        {
            if (null == gameObject)
                return;

            if (null != m_GameObjectRoot)
            {
                int instanceID = gameObject.GetInstanceID();
                Object curPoolObject = _GetPoolObject(instanceID);
                if (null != curPoolObject)
                {
                    ObjectPoolDesc objectPoolDesc = _GetObjectPoolDesc(curPoolObject.Name, false);
                    if (null != objectPoolDesc)
                    {
                        gameObject.transform.SetParent(m_GameObjectRoot.transform, false);
                        objectPoolDesc.GameObjectPool.QureyInterface<ITMRecyclePool<Object>>().Recycle(curPoolObject);
                    }
                    else
                    {
                        Debugger.LogWarning("Can not find pool with res path '{0}'!", curPoolObject.Name);
                        _UnregiesterPoolObject(instanceID);
                        curPoolObject.OnRelease();
                    }

                    return;
                }
                else
                    Debugger.LogWarning("Game object '{0}' is not in object pool!", gameObject.name);
            }
            else
                Debugger.LogWarning( "Root game object can not be null!");

            GameObject.Destroy(gameObject);
        }

        public void PurgePool(HashSet<string> reserveAssets,bool clearAll)
        {
            List<string> releaseDescList = FrameStackList<string>.Acquire();
            Dictionary<string, ObjectPoolDesc>.Enumerator it = m_GameObjectPoolSet.GetEnumerator();
            while(it.MoveNext())
            {
                ObjectPoolDesc curDesc = it.Current.Value;
                if(null == curDesc)
                {
                    releaseDescList.Add(it.Current.Key);
                    continue;
                }

                if (null != reserveAssets && reserveAssets.Contains(it.Current.Key))
                    continue;

                curDesc.GameObjectPool.PurgePool(clearAll);
            }

            for(int i = 0,icnt = releaseDescList.Count;i<icnt;++i)
                m_GameObjectPoolSet.Remove(releaseDescList[i]);

            FrameStackList<string>.Recycle(releaseDescList);
            _PurgePoolObject();
        }

        public void ClearGameObject(string prefabRes)
        {
            if (string.IsNullOrEmpty(prefabRes))
            {
                Debugger.LogWarning("Prefab resource path can not be null or empty!");
                return;
            }

            ObjectPoolDesc objectPoolDesc = _GetObjectPoolDesc(prefabRes, true);
            if (null != objectPoolDesc)
                objectPoolDesc.GameObjectPool.PurgePool(true);
        }

        /// <summary>
        /// 获取所有目标GameObject的池子的信息
        /// </summary>
        /// <param name="poolInfoList">所有的池子信息</param>
        public void GetAllPoolInfo(ref List<UnityGameObjectPoolInfo> poolInfoList)
        {
            poolInfoList.Clear();
            Dictionary<string, ObjectPoolDesc>.Enumerator it = m_GameObjectPoolSet.GetEnumerator();
            while (it.MoveNext())
            {
                ObjectPoolDesc curDesc = it.Current.Value;
                if (null == curDesc)
                    continue;

                RecyclePoolBase pool = curDesc.GameObjectPool;
                poolInfoList.Add(new UnityGameObjectPoolInfo(it.Current.Key, curDesc.GameObjectType,
                    pool.ReserveCount, pool.ExpireTime,pool.Priority,pool.UnusedObjectCount, pool.UsingObjectCount,
                    pool.AcquireCount,pool.RecycleCount, pool.CreateCount,pool.ReleaseCount));
            }
        }

        public void OnPurgePool(object sender, PurgePoolEventArgs args)
        {
            if (null != args)
                PurgePool(args.ReserveAssetKeys, true);
            else
                Debugger.LogWarning("On purge pool args is null!");
        }

        public void OnUnloadAssetStateChanged(bool isPurgingAsset)
        {
            m_IsPurgingAsset = isPurgingAsset;
        }

        public int GameObjectPoolCount
        {
            get { return m_GameObjectPoolSet.Count; }
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal override int Priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal sealed override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_IsPurgingAsset)
                return;

            long remainTime = Utility.Time.MicrosecondsToTicks(3);
            long lastTime = Utility.Time.GetTicksNow();
            while(m_RequestCacheQueue.Count > 0 && Utility.Time.GetTicksNow() - lastTime < remainTime)
            {
                ObjectRequestCache curCache = m_RequestCacheQueue.Dequeue();
                curCache.m_Callback.OnAssetLoadSuccess(curCache.m_PrefabPath, curCache.m_GameObject, curCache.m_RequestID, 0.0f, curCache.m_UserData);
            }
        }

        /// <summary>
        /// 关闭并清理游戏框架模块。
        /// </summary>
        internal sealed override void Shutdown()
        {
            Dictionary<string, ObjectPoolDesc>.Enumerator it = m_GameObjectPoolSet.GetEnumerator();
            while (it.MoveNext())
            {
                ObjectPoolDesc curDesc = it.Current.Value;
                if (null == curDesc)
                    continue;

                m_RecyclePoolManager.DestroyRecyclePool(curDesc.GameObjectPool);
            }
            m_GameObjectPoolSet.Clear();
            m_ObjectRevMap.Clear();
        }

        private ObjectPoolDesc _GetObjectPoolDesc(string resKey,bool createIfNil)
        {
            ObjectPoolDesc objectPoolDesc = null;
            if (!m_GameObjectPoolSet.TryGetValue(resKey, out objectPoolDesc) && createIfNil)
            {
                RecyclePoolBase newObjectPool = m_RecyclePoolManager.CreateRecyclePool<Object>(_CreateObject);
                objectPoolDesc = new ObjectPoolDesc(newObjectPool, GameObjectUsage.Default);
                m_GameObjectPoolSet.Add(resKey, objectPoolDesc);
            }

            return objectPoolDesc;
        }

        private void _RegiestPoolObject(int instanceID,Object pooledObj)
        {
#if UNITY_EDITOR
            if (m_ObjectRevMap.ContainsKey(instanceID))
            {
                Debugger.LogError("Pool object with name '{0}' and ID '{1}' has already register in pool!", pooledObj.Name, instanceID);
                return;
            }
#endif

            m_ObjectRevMap.Add(instanceID, pooledObj);
        }

        private void _UnregiesterPoolObject(int instanceID)
        {
#if UNITY_EDITOR
            if (!m_ObjectRevMap.ContainsKey(instanceID))
            {
                Debugger.LogError("Pool object with ID '{0}' does not belong to pool!",  instanceID);
                return;
            }
#endif

            m_ObjectRevMap.Remove(instanceID);
        }

        private Object _GetPoolObject(int instanceID)
        {
            Object curPoolObject = null;
            if (m_ObjectRevMap.TryGetValue(instanceID, out curPoolObject))
                return curPoolObject;

            return null;
        }

        private void _PurgePoolObject()
        {
            List<int> releaseList = FrameStackList<int>.Acquire();
            Dictionary<int, Object>.Enumerator it = m_ObjectRevMap.GetEnumerator();
            while (it.MoveNext())
            {
                Object curObj = it.Current.Value;
                if (null == curObj.GameObject)
                    releaseList.Add(it.Current.Key);
            }

            for (int i = 0, icnt = releaseList.Count; i < icnt; ++i)
                m_ObjectRevMap.Remove(releaseList[i]);

            FrameStackList<int>.Recycle(releaseList);
        }

        private Recyclable _CreateObject()
        {
            return null;
        }

        protected uint _AllocHandle()
        {
            /// 前两位预留
            if (m_RequestIDCount + 1 >= uint.MaxValue >> 2)
                m_RequestIDCount = 0;
            return (m_RequestIDCount++) | ((m_RequestIDType & 0x03) << 30);
        }
    }
}

