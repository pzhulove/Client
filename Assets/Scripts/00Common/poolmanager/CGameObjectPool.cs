using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum GameObjectPoolFlag
{
    None = 0x00,
    HideAfterLoad = 0x01,
    ReserveLast = 0x02,

    HighPriority = 0x10,
}

public sealed class CGameObjectPool : Singleton<CGameObjectPool>
{
    /////////////////////////////////////////////////////// New API //////////////////////////////////////////////////////////
    private static Tenmove.Runtime.Unity.ITMUnityGameObjectPool m_GameObjectPool = null;

    private static Tenmove.Runtime.Unity.ITMUnityGameObjectPool _GetGameObjectPool()
    {
        if (null == m_GameObjectPool)
            m_GameObjectPool = Tenmove.Runtime.ModuleManager.GetModule<Tenmove.Runtime.Unity.ITMUnityGameObjectPool>();

        return m_GameObjectPool;
    }

    private static GameObject _GetGameObject_TMEngine(string prefabFullPath, uint poolFlag)
    {
        Tenmove.Runtime.Unity.ITMUnityGameObjectPool gameObjectPool = _GetGameObjectPool();
        GameObject gameObject = null;
        if (null != gameObjectPool)
        {
            gameObject = gameObjectPool.AcquireGameObjectSync(prefabFullPath, 0);
        }

        return gameObject;
    }
    
    private static uint _GetGameObjectAsync_TMEngine(string prefabFullPath, Tenmove.Runtime.AssetLoadCallbacks<object> callbacks, object userData ,uint poolFlag, uint waterMark = 0x0)
    {
        Tenmove.Runtime.Unity.ITMUnityGameObjectPool gameObjectPool = _GetGameObjectPool();
        if (null != gameObjectPool)
            return (uint)gameObjectPool.AcquireGameObjectAsync(prefabFullPath, userData, callbacks, 0);

        return ~0u;
    }

    private static void _RecycleGameObject_TMEngine(GameObject gameObject)
    {
        Tenmove.Runtime.Unity.ITMUnityGameObjectPool gameObjectPool = _GetGameObjectPool();
        if (null != gameObjectPool)
        {
            gameObjectPool.RecycleGameObject(gameObject);
            return;
        }
        GameObject.Destroy(gameObject);
    }


    public static GameObject GetGameObject(string prefabFullPath, uint poolFlag)
    {
        return _GetGameObject_TMEngine(prefabFullPath, poolFlag);
    }

    public static uint GetGameObjectAsync(string path, Tenmove.Runtime.AssetLoadCallbacks<object> callbacks, object userData, uint flag = 0u, uint waterMark = 0x0)
    {
        return (uint)_GetGameObjectAsync_TMEngine(path, callbacks, userData, flag, waterMark);
    }

    public static void RecycleGameObjectEx(GameObject gameObject)
    {
        _RecycleGameObject_TMEngine(gameObject);
    }

    public static void AbortAcquireRequest(uint handle)
    {

    }
    /////////////////////////////////////////////////////// New API //////////////////////////////////////////////////////////

    static public readonly uint INVILID_HANDLE = ~0u;

    private bool m_clearPooledObjects;
    private int m_clearPooledObjectsExecuteFrame;
    private DictionaryView<string, Queue<CPooledGameObjectScript>> m_pooledGameObjectMap = new DictionaryView<string, Queue<CPooledGameObjectScript>>();
    private GameObject m_poolRoot;
    private static int s_frameCounter;

    private bool m_IsRecycling = false;

    private int m_QureyCnt = 0;
    private readonly int QUREY_STEP = 1;

    #region 新异步加载管理
    
    Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);
    static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
    {
        GameObjectLoadContext curContext = userData as GameObjectLoadContext;
        if (null != curContext)
        {
            GameObject gameObject = asset as GameObject;
            if (null != gameObject)
            {
                CPooledGameObjectScript component = null;
                IPooledMonoBehaviour[] pooledMonoBehaviours = curContext.m_This.GetPooledMonoBehaviours(gameObject);
                //DebugHelper.Assert(gameObject != null);
                component = gameObject.GetComponent<CPooledGameObjectScript>();
                if (component == null)
                    component = gameObject.AddComponent<CPooledGameObjectScript>();

                component.m_prefabKey = curContext.m_PrefabKey;
                component.m_pooledMonoBehaviours = pooledMonoBehaviours;
                component.m_defaultScale = component.transform.localScale;
                component.m_isInit = true;
                component.m_IsRecycled = false;
                component.m_IsOriginInVisible = !component.gameObject.activeSelf;

                curContext.m_This.HandlePooledMonoBehaviour(component.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);

                if (null != curContext.m_CallerCallbacks && null != curContext.m_CallerCallbacks.OnAssetLoadSuccess)
                    curContext.m_CallerCallbacks.OnAssetLoadSuccess(curContext.m_AssetPath, gameObject, (int)curContext.m_RequestID, duration, curContext.m_UserData);
            }
            else
                Tenmove.Runtime.Debugger.LogError("Game object is null when load game object '{0}'!", assetPath);
        }
        else
            Tenmove.Runtime.Debugger.LogError("Game object load context type error when callback with game object '{0}'!", assetPath);
    }

    static void _OnLoadAssetFailure(string assetPath,int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
    {
    }

    class GameObjectRequestCache
    {
        public GameObjectRequestCache(CPooledGameObjectScript script, string assetPath, uint requestID, object userData, Tenmove.Runtime.AssetLoadCallbacks<object> callbacks)
        {
            m_Script = script;
            m_AssetPath = assetPath;
            m_RequestID = requestID;
            m_UserData = userData;
            m_CallerCallbacks = callbacks;
        }

        public CPooledGameObjectScript m_Script;
        public string m_AssetPath;
        public object m_UserData;
        public uint m_RequestID;
        public Tenmove.Runtime.AssetLoadCallbacks<object> m_CallerCallbacks;
    }

    class GameObjectLoadContext: GameObjectRequestCache
    {
        public GameObjectLoadContext(CGameObjectPool gameObjectPool,string prefabKey,CPooledGameObjectScript script, string assetPath, uint requestID, object userData, Tenmove.Runtime.AssetLoadCallbacks<object> callbacks)
            : base( script, assetPath, requestID, userData, callbacks)
        {
            Tenmove.Runtime.Debugger.Assert(null != gameObjectPool, "Game object pool object can not be null!");
            m_This = gameObjectPool;
            m_PrefabKey = prefabKey;
        }

        public string m_PrefabKey;
        public CGameObjectPool m_This;
    }

    private uint m_PoolRequestIDCount = 0;

    List<GameObjectRequestCache> m_GameObjectRequestCache = new List<GameObjectRequestCache>();

    public uint GetGameObjectAsync_TMEngine(string prefabFullPath, Tenmove.Runtime.AssetLoadCallbacks<object> callbacks,object userData, enResourceType resourceType, uint poolFlag, uint waterMark = 0x0)
    {
        if(null == callbacks)
        {
            Tenmove.Runtime.Debugger.LogError("Asset load callbacks can not be null on get game object '{0}'!",prefabFullPath);
            return ~0u;
        }

        string key = CFileManager.EraseExtension(prefabFullPath);
        Queue<CPooledGameObjectScript> queue = null;
        if (!this.m_pooledGameObjectMap.TryGetValue(key, out queue))
        {
            queue = new Queue<CPooledGameObjectScript>();
            this.m_pooledGameObjectMap.Add(key, queue);
        }
        CPooledGameObjectScript script = null;
        while (queue.Count > 0)
        {
            script = queue.Dequeue();
            if ((script != null) && (script.gameObject != null))
            {
                script.gameObject.transform.SetParent(null, true);
                script.gameObject.transform.localScale = script.m_defaultScale;

                break;
            }
            script = null;
        }

        uint curRequestID = m_PoolRequestIDCount++;
        if (script != null)
        {
            script.m_IsRecycled = false;
            this.HandlePooledMonoBehaviour(script.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Get);

            m_GameObjectRequestCache.Add(new GameObjectRequestCache(script,prefabFullPath, curRequestID, userData, callbacks));
            return curRequestID;
        }
        else
        {
            return this._CreateGameObjectAsync(prefabFullPath,callbacks, userData,resourceType, curRequestID, key, poolFlag);
        }
    }

    private uint _CreateGameObjectAsync(string prefabFullPath, Tenmove.Runtime.AssetLoadCallbacks<object> callbacks, object userData, enResourceType resourceType, uint requestID,string prefabKey, uint poolFlag)
    {
        GameObjectLoadContext loadContext = new GameObjectLoadContext(this, prefabKey, null, prefabFullPath, requestID, userData, callbacks);
        AssetLoader.LoadResAsGameObjectAsync(prefabFullPath, m_AssetLoadCallbacks, loadContext, 0);
        return requestID;
    }

    private void _OnUpdate()
    {
        if (null != m_GameObjectRequestCache && m_GameObjectRequestCache.Count > 0)
        {

            for (int i = 0, icnt = m_GameObjectRequestCache.Count; i < icnt; ++i)
            {
                GameObjectRequestCache curCache = m_GameObjectRequestCache[i];
                if (null == curCache)
                    continue;

                if (null != curCache.m_CallerCallbacks && null != curCache.m_CallerCallbacks.OnAssetLoadSuccess)
                    curCache.m_CallerCallbacks.OnAssetLoadSuccess(curCache.m_AssetPath, curCache.m_Script.gameObject, (int)curCache.m_RequestID, 0, curCache.m_UserData);
            }

            m_GameObjectRequestCache.Clear();
        }
    }

    #endregion

    #region 异步加载管理
    private class GameObjPoolAsyncRequest : AsyncLoadRequest<UnityEngine.Object>
    {
        //public IAssetInstRequest m_AssetInstReq = null;
        public uint m_AssetInstReq = AssetLoader.INVILID_HANDLE;
        public string m_PrefabKey = null;
        public Vector3 m_Pos = Vector3.zero;
        public Quaternion m_Rot = Quaternion.identity;
        public bool m_UseRotation = false;
        public uint m_PoolFlag = 0u;
        public enResourceType m_ResourceType = enResourceType.BattleScene;


        public override UnityEngine.Object Extract()
        {
            //#if UNITY_EDITOR
            //            Logger.LogError("Extract GameObjPoolAsyncRequest:" + m_ResPath + " " + m_WaterMark.ToString("x"));
            //#endif

            /// GameObject gameObject = m_ResObject as GameObject;
            /// CPooledGameObjectScript component = gameObject.GetComponent<CPooledGameObjectScript>();
            /// if (component != null)
            /// {
            ///     component.m_DebugInfo += "(Extracted)\n";
            /// }
            /// 
            /// Debug.LogErrorFormat("Extract res '{0}'!", m_PrefabKey);
            return base.Extract();
        }

        public override void Abort()
        {
            if(null != m_ResObject)
                GameObject.Destroy(m_ResObject);
            
            base.Abort();
        }


        public new void Reset()
        {
            m_AssetInstReq = AssetLoader.INVILID_HANDLE;
            m_PrefabKey = null;
            m_Pos = Vector3.zero;
            m_Rot = Quaternion.identity;
            m_UseRotation = false;

            m_ResObject = null;
            m_IsDone = false;
            m_Extracted = false;
            m_IsAbort = false;
            m_WaterMark = 0x0;
        }
    }

    private List<GameObjPoolAsyncRequest> m_AsyncReqList = new List<GameObjPoolAsyncRequest>();
    private List<GameObjPoolAsyncRequest> m_HighPriorityAsyncReqList = new List<GameObjPoolAsyncRequest>();
    private List<GameObjPoolAsyncRequest> m_IdleReqPool = new List<GameObjPoolAsyncRequest>();
    private List<GameObjPoolAsyncRequest> m_CompleteList = new List<GameObjPoolAsyncRequest>();

    public int CompleteTaskCount
    {
        get { return m_CompleteList.Count; }
    }

    public int LoadingTaskCount
    {
        get { return m_HighPriorityAsyncReqList.Count + m_AsyncReqList.Count; }
    }

    private GameObjPoolAsyncRequest _AllocAsyncRequest()
    {
        GameObjPoolAsyncRequest availableReq = null;
        if (m_IdleReqPool.Count > 0)
        {
            availableReq = m_IdleReqPool[0];
            m_IdleReqPool.RemoveAt(0);
        }

        if (null == availableReq)
            availableReq = new GameObjPoolAsyncRequest();

        availableReq.Reset();
        return availableReq;
    }

    private void _OnGameObjectLoaded(GameObjPoolAsyncRequest request, GameObject gameObject)
    {
        if (null != gameObject)
        {
            CPooledGameObjectScript component = null;
            gameObject.transform.position = request.m_Pos;
            if (request.m_UseRotation)
            {
                gameObject.transform.rotation = request.m_Rot;
            }

            if (!m_IsRecycling
#if DEBUG_REPORT_ROOT
                && !DebugSettings.instance.DisableGameObjPool
#endif
               )
            {
                IPooledMonoBehaviour[] pooledMonoBehaviours = this.GetPooledMonoBehaviours(gameObject);
                //DebugHelper.Assert(gameObject != null);
                component = gameObject.GetComponent<CPooledGameObjectScript>();
                if (component == null)
                    component = gameObject.AddComponent<CPooledGameObjectScript>();

                if (null == gameObject.GetComponent<AssetProxy>())
                    gameObject.AddComponent<AssetProxy>();

                component.m_prefabKey = request.m_PrefabKey;
                component.m_pooledMonoBehaviours = pooledMonoBehaviours;
                component.m_defaultScale = component.transform.localScale;
                component.m_isInit = true;
                component.m_IsRecycled = false;
                component.m_IsOriginInVisible = !component.gameObject.activeSelf;

                ///component.m_DebugInfo = StackTraceUtility.ExtractStackTrace();

                this.HandlePooledMonoBehaviour(component.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);

                //if (request.m_ReserveLast)
                /// Queue<CPooledGameObjectScript> queue = null;
                /// if (!this.m_pooledGameObjectMap.TryGetValue(request.m_PrefabKey, out queue))
                /// {
                ///     queue = new Queue<CPooledGameObjectScript>();
                ///     this.m_pooledGameObjectMap.Add(request.m_PrefabKey, queue);
                /// }
                /// 
                /// if (0 == queue.Count && _HasFlag(request.m_PoolFlag, GameObjectPoolFlag.ReserveLast))
                /// {
                ///     CPooledGameObjectScript lastScript = _InstantiateGameObjectInst(component.gameObject, request.m_ResourceType, request.m_PrefabKey, component.m_IsOriginInVisible);
                ///     if (null != lastScript)
                ///     {
                ///         queue.Enqueue(lastScript);
                ///         this.HandlePooledMonoBehaviour(lastScript.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);
                ///         lastScript.gameObject.transform.SetParent(this.m_poolRoot.transform, false);
                ///         lastScript.gameObject.SetActive(false);
                ///         lastScript.m_IsRecycled = true;
                ///     }
                /// }
            }
        }
    }

    private GameObjPoolAsyncRequest _CheckAsyncRequest(List<GameObjPoolAsyncRequest> asyncReqList)
    {
        GameObjPoolAsyncRequest completeReq = null;
        for (int i = 0, icnt = asyncReqList.Count; i < icnt; ++i)
        {
            GameObjPoolAsyncRequest curReq = asyncReqList[i];
            if (null != curReq)
            {
                if (!curReq.m_IsAbort)
                {
                    //if (null != curReq.m_AssetInstReq)
                    if (AssetLoader.INVILID_HANDLE != curReq.m_AssetInstReq)
                    {
                        //if (!curReq.m_AssetInstReq.IsDone()) continue;
                        if (!AssetLoader.instance.IsValidHandle(curReq.m_AssetInstReq))
                        {
                            curReq.Reset();
                            m_IdleReqPool.Add(curReq);
                            asyncReqList.RemoveAt(i);
                            --i;
                            --icnt;
                            continue;
                        }

                        if (!AssetLoader.instance.IsRequestDone(curReq.m_AssetInstReq)) continue;

                        /// 添加池内管理脚本
                        //AssetInst content = curReq.m_AssetInstReq.Extract();
                        AssetInst content = AssetLoader.instance.Extract(curReq.m_AssetInstReq);

                        GameObject gameObject = null;
                        if (null != content)
                        {
                            gameObject = content.obj as GameObject;
                            _OnGameObjectLoaded(curReq, gameObject);
                        }

                        curReq.m_IsDone = true;
                        curReq.m_Extracted = false;
                        curReq.m_ResObject = gameObject;
                        curReq.m_AssetInstReq = AssetLoader.INVILID_HANDLE;
                    }
                }
                else
                {
                    AssetLoader.instance.AbortRequest(curReq.m_AssetInstReq);
                    curReq.Reset();
                    asyncReqList.RemoveAt(i);
                    m_IdleReqPool.Add(curReq);
                    --i;
                    --icnt;
                    continue;
                }
            }

            completeReq = curReq;
            asyncReqList.RemoveAt(i);
            break;
        }

        return completeReq;
    }


    private void _UpdateAsync()
    {
        if (EngineConfig.useTMEngine)
            return;

        /// ++m_QureyCnt;
        /// if (m_QureyCnt >= QUREY_STEP)
        ///     m_QureyCnt = 0;
        /// else
        ///     return;

        GameObjPoolAsyncRequest completeReq = null;
        /// 检查高优先级异步加载队列
        completeReq = _CheckAsyncRequest(m_HighPriorityAsyncReqList);

        /// 检查异步加载队列
        if (null == completeReq)
            completeReq = _CheckAsyncRequest(m_AsyncReqList);
        /// else
        /// {
        ///     Debug.LogErrorFormat("### 当前优先队列请求数量：{0}!", m_HighPriorityAsyncReqList.Count);
        ///     Debug.LogErrorFormat("Finish check high request in gameobject pool '{0}(Time:{1})'!", completeReq.m_PrefabKey, Time.realtimeSinceStartup);
        /// }
//         GameObjPoolAsyncRequest completeReq = null;
//         for (int i = 0, icnt = m_AsyncReqList.Count; i < icnt; ++i)
//         {
//             GameObjPoolAsyncRequest curReq = m_AsyncReqList[i];
//             if (null != curReq)
//             {
//                 if (!curReq.m_IsAbort)
//                 {
//                     //if (null != curReq.m_AssetInstReq)
//                     if (AssetLoader.INVILID_HANDLE != curReq.m_AssetInstReq)
//                     {
//                         //if (!curReq.m_AssetInstReq.IsDone()) continue;
//                         if(!AssetLoader.instance.IsValidHandle(curReq.m_AssetInstReq))
//                         {
//                             curReq.Reset();
//                             m_IdleReqPool.Add(curReq);
//                             m_AsyncReqList.RemoveAt(i);
//                             break;
//                         }
// 
//                         if (!AssetLoader.instance.IsRequestDone(curReq.m_AssetInstReq)) continue;
// 
//                         /// 添加池内管理脚本
//                         CPooledGameObjectScript component = null;
//                         //AssetInst content = curReq.m_AssetInstReq.Extract();
//                         AssetInst content = AssetLoader.instance.Extract(curReq.m_AssetInstReq);
// 
//                         GameObject gameObject = null;
//                         if (null != content)
//                         {
//                             gameObject = content.obj as GameObject;
//                             if (null != gameObject)
//                             {
//                                 gameObject.transform.position = curReq.m_Pos;
//                                 if (curReq.m_UseRotation)
//                                 {
//                                     gameObject.transform.rotation = curReq.m_Rot;
//                                 }
// 
//                                 if (!m_IsRecycling
// #if DEBUG_REPORT_ROOT
//                             && !DebugSettings.instance.DisableGameObjPool
// #endif
//                         )
//                                 {
//                                     IPooledMonoBehaviour[] pooledMonoBehaviours = this.GetPooledMonoBehaviours(gameObject);
//                                     //DebugHelper.Assert(gameObject != null);
//                                     component = gameObject.GetComponent<CPooledGameObjectScript>();
//                                     if (component == null)
//                                         component = gameObject.AddComponent<CPooledGameObjectScript>();
// 
//                                     if (null == gameObject.GetComponent<AssetProxy>())
//                                         gameObject.AddComponent<AssetProxy>();
// 
//                                     component.m_prefabKey = curReq.m_PrefabKey;
//                                     component.m_pooledMonoBehaviours = pooledMonoBehaviours;
//                                     component.m_defaultScale = component.transform.localScale;
//                                     component.m_isInit = true;
//                                     component.m_IsRecycled = false;
//                                     component.m_IsOriginInVisible = !component.gameObject.activeSelf;
// 
//                                     this.HandlePooledMonoBehaviour(component.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);
// 
//                                     //if (curReq.m_ReserveLast)
//                                     Queue<CPooledGameObjectScript> queue = null;
//                                     if (!this.m_pooledGameObjectMap.TryGetValue(curReq.m_PrefabKey, out queue))
//                                     {
//                                         queue = new Queue<CPooledGameObjectScript>();
//                                         this.m_pooledGameObjectMap.Add(curReq.m_PrefabKey, queue);
//                                     }
// 
//                                     if (0 == queue.Count && _HasFlag(curReq.m_PoolFlag, GameObjectPoolFlag.ReserveLast))
//                                     {
//                                         CPooledGameObjectScript lastScript = _InstantiateGameObjectInst(component.gameObject, curReq.m_ResourceType, curReq.m_PrefabKey, component.m_IsOriginInVisible);
//                                         if(null != lastScript)
//                                         {
//                                             queue.Enqueue(lastScript);
//                                             this.HandlePooledMonoBehaviour(lastScript.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);
//                                             lastScript.gameObject.transform.SetParent(this.m_poolRoot.transform, false);
//                                             lastScript.gameObject.SetActive(false);
//                                             lastScript.m_IsRecycled = true;
//                                         }
//                                     }
//                                 }
//                             }
//                         }
// 
//                         curReq.m_IsDone = true;
//                         curReq.m_Extracted = false;
//                         curReq.m_ResObject = gameObject;
//                         curReq.m_AssetInstReq = AssetLoader.INVILID_HANDLE;
//                     }
//                 }
//                 else
//                 {
//                     AssetLoader.instance.AbortRequest(curReq.m_AssetInstReq);
//                     curReq.Reset();
//                     m_AsyncReqList.RemoveAt(i);
//                     m_IdleReqPool.Add(curReq);
//                     --i;
//                     --icnt;
//                     continue;
//                 }
//             }
// 
//             completeReq = curReq;
//             m_AsyncReqList.RemoveAt(i);
//             break;
//         }

        /// 检查完成队列
        int Step = AssetLoader.instance.GetAsyncLoadStep();
        if (m_CompleteList.Count > 200)
            Step *= 2;
        for (int i = 0, icnt = m_CompleteList.Count; i < icnt; ++i)
        {
            GameObjPoolAsyncRequest curReq = m_CompleteList[i];
            if (null != curReq)
            {
                if (curReq.m_IsAbort)
                {
                    GameObject curObj = curReq.Extract() as GameObject;
                    if (null != curObj)
                    {
                        // Debug.LogErrorFormat("curReq {0} put in recycle {1}!", curReq.m_AssetPath, curObj.name);
                        RecycleGameObject(curObj);

                        //#if UNITY_EDITOR
                        //                        Logger.LogError("AssetInstReq Destory:" + curObj.name + " " + curReq.m_WaterMark.ToString("x"));
                        //#endif

                        //GameObject.Destroy(curObj);
                    }

                    curReq.Reset();
                    m_CompleteList.RemoveAt(i);
                    m_IdleReqPool.Add(curReq);
                    --i;
                    --icnt;
                    continue;
                }
                else
                {
                    if (!curReq.m_Extracted)
                        continue;
                }

                curReq.Reset();
                m_IdleReqPool.Add(curReq);
            }

            m_CompleteList.RemoveAt(i);
            --i;
            --icnt;
            --Step;
            if(Step <= 0)
                break;
        }

        if (null != completeReq)
            m_CompleteList.Add(completeReq);
    }
    #endregion

    #region 异步句柄管理

    AsyncRequestHandleAllocator<IAsyncLoadRequest<UnityEngine.Object>> m_AsyncRequestHandleAlloc = new AsyncRequestHandleAllocator<IAsyncLoadRequest<UnityEngine.Object>>(1);

    #endregion

    //private void _RecycleGameObject(GameObject pooledGameObject, bool setIsInit)
    //{
    //    if (pooledGameObject != null)
    //    {
    //        CPooledGameObjectScript component = pooledGameObject.GetComponent<CPooledGameObjectScript>();
    //        if (component != null)
    //        {
    //            if (true == component.m_IsRecycled)
    //                return;
    //
    //            Queue<CPooledGameObjectScript> queue = null;
    //            if (this.m_pooledGameObjectMap.TryGetValue(component.m_prefabKey, out queue))
    //            {
    //                if (null != queue)
    //                {
    //                    if (null == m_poolRoot)
    //                    {
    //                        UnityEngine.Object.Destroy(pooledGameObject);
    //                    }
    //                    else
    //                    {
    //                        queue.Enqueue(component);
    //                        this.HandlePooledMonoBehaviour(component.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Recycle);
    //                        component.gameObject.transform.SetParent(this.m_poolRoot.transform, false);
    //                        component.gameObject.SetActive(false);
    //                        component.m_isInit = setIsInit;
    //                        component.m_IsRecycled = true;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                queue = new Queue<CPooledGameObjectScript>();
    //                this.m_pooledGameObjectMap.Add(component.m_prefabKey, queue);
    //
    //                queue.Enqueue(component);
    //                this.HandlePooledMonoBehaviour(component.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Recycle);
    //                component.gameObject.transform.SetParent(this.m_poolRoot.transform, false);
    //                component.gameObject.SetActive(false);
    //                component.m_isInit = setIsInit;
    //                component.m_IsRecycled = true;
    //            }
    //
    //            return;
    //        }
    //
    //        UnityEngine.Object.Destroy(pooledGameObject);
    //        Logger.LogWarningFormat("Object Leak:Game object [{0}] does not belong to object pool!", pooledGameObject.name);
    //    }
    //}

    private void _RecycleGameObject(GameObject pooledGameObject, bool setIsInit)
    {
        if (EngineConfig.useTMEngine)
        {
            RecycleGameObjectEx(pooledGameObject);
            return;
        }

        if (pooledGameObject != null)
        {
            bool isPoolObject = false;
            List<CPooledGameObjectScript> scriptLst = GamePool.ListPool<CPooledGameObjectScript>.Get();
            pooledGameObject.GetComponentsInChildren<CPooledGameObjectScript>(scriptLst);

            for(int i = 0,icnt = scriptLst.Count;i<icnt;++i)
            {
                CPooledGameObjectScript component = scriptLst[i];
                if(null == component) continue;

                if (pooledGameObject == component.gameObject)
                    isPoolObject = true;

                if (true == component.m_IsRecycled)
                    continue;

                Queue<CPooledGameObjectScript> queue = null;
                if (!this.m_pooledGameObjectMap.TryGetValue(component.m_prefabKey, out queue))
                {
                    queue = new Queue<CPooledGameObjectScript>();
                    this.m_pooledGameObjectMap.Add(component.m_prefabKey, queue);
                }

                if (null == m_poolRoot)
                {
                    UnityEngine.Object.Destroy(pooledGameObject);
                }
                else
                {
                    queue.Enqueue(component);
                    this.HandlePooledMonoBehaviour(component.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Recycle);
                    component.gameObject.transform.SetParent(this.m_poolRoot.transform, false);
                    component.m_isInit = setIsInit;
                    component.gameObject.SetActive(false);
                    component.m_IsRecycled = true;
                }
            }
            GamePool.ListPool<CPooledGameObjectScript>.Release(scriptLst);

            if (!isPoolObject)
            {
                //if (!pooledGameObject.activeSelf)
                //    pooledGameObject.SetActive(true);
                UnityEngine.Object.Destroy(pooledGameObject);
                Logger.LogWarningFormat("Object Leak:Game object [{0}] does not belong to object pool!", pooledGameObject.name);
            }
        }
    }


    public void ClearPooledObjects()
    {
        this.m_clearPooledObjects = true;
        this.m_clearPooledObjectsExecuteFrame = s_frameCounter + 1;
    }

    private CPooledGameObjectScript _InstantiateGameObjectInst(GameObject go, enResourceType resourceType, string prefabKey, bool isOriginInVisible = false)
    {
        CPooledGameObjectScript component = null;

        GameObject gameObject = GameObject.Instantiate(go);
        if (null == gameObject)
        {
            Logger.LogErrorFormat("Instantiate gameobject has failed with prefab key[{0}]!",prefabKey);
            return null;
        }

        gameObject.name = go.name;

        AssetProxy assetProxySrc = go.GetComponent<AssetProxy>();
        AssetProxy assetProxyDst = gameObject.GetComponent<AssetProxy>();
        if (null != assetProxySrc && null != assetProxyDst)
            assetProxyDst.AddResRef(assetProxySrc);

        IPooledMonoBehaviour[] pooledMonoBehaviours = this.GetPooledMonoBehaviours(gameObject);
        //DebugHelper.Assert(gameObject != null);
        component = gameObject.GetComponent<CPooledGameObjectScript>();
        if (component == null)
            component = gameObject.AddComponent<CPooledGameObjectScript>();

        if (null == gameObject.GetComponent<AssetProxy>())
            gameObject.AddComponent<AssetProxy>();

        if (null == component)
            Debug.LogWarning("Create CPooledGameObjectScript component has failed!");

        component.m_prefabKey = prefabKey;
        component.m_pooledMonoBehaviours = pooledMonoBehaviours;
        component.m_defaultScale = component.transform.localScale;
        component.m_isInit = true;
        component.m_IsRecycled = false;
        component.m_IsOriginInVisible = isOriginInVisible;

        /// component.m_DebugInfo = StackTraceUtility.ExtractStackTrace();

        this.HandlePooledMonoBehaviour(component.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);
        return component;
    }

    private CPooledGameObjectScript CreateGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, string prefabKey)
    {
        CPooledGameObjectScript component = null;
        bool needCached = resourceType == enResourceType.BattleScene;
        //GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(prefabFullPath, typeof(GameObject), resourceType, needCached, false).m_content as GameObject;
        //if (content == null)
        //{
        //    return null;
        //}
        //GameObject gameObject = null;
        //if (useRotation)
        //{
        //    gameObject = UnityEngine.Object.Instantiate(content, pos, rot) as GameObject;
        //}
        //else
        //{
        //    gameObject = UnityEngine.Object.Instantiate(content) as GameObject;
        //    gameObject.transform.position = pos;
        //}
        
        GameObject gameObject = AssetLoader.instance.LoadResAsGameObject(prefabFullPath);
        if(null == gameObject)
            return null;

        gameObject.transform.position = pos;
        if (useRotation)
            gameObject.transform.rotation = rot;

        IPooledMonoBehaviour[] pooledMonoBehaviours = this.GetPooledMonoBehaviours(gameObject);
        //DebugHelper.Assert(gameObject != null);
        component = gameObject.GetComponent<CPooledGameObjectScript>();
        if (component == null)
            component = gameObject.AddComponent<CPooledGameObjectScript>();

        if (null == gameObject.GetComponent<AssetProxy>())
            gameObject.AddComponent<AssetProxy>();

        if (null == component)
            Debug.LogWarning("Create CPooledGameObjectScript component has failed!");

        component.m_prefabKey = prefabKey;
        component.m_pooledMonoBehaviours = pooledMonoBehaviours;
        component.m_defaultScale = component.transform.localScale;
        component.m_isInit = true;
        component.m_IsRecycled = false;

        /// component.m_DebugInfo = StackTraceUtility.ExtractStackTrace();

        this.HandlePooledMonoBehaviour(component.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);
        return component;
    }

    private IAsyncLoadRequest<UnityEngine.Object> _CreateGameObjectAsync(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, string prefabKey, uint poolFlag,uint waterMark)
    {
        GameObjPoolAsyncRequest newAsyncReq = _AllocAsyncRequest();

        uint loadFlag = (uint)(_HasFlag(poolFlag, GameObjectPoolFlag.HideAfterLoad) ? AssetLoadFlag.HideAfterLoad : AssetLoadFlag.None);
        if (_HasFlag(poolFlag, GameObjectPoolFlag.HighPriority))
            loadFlag |= (uint)AssetLoadFlag.HighPriority;

        newAsyncReq.m_AssetInstReq = AssetLoader.instance.LoadResAsyncAsGameObject(prefabFullPath, false, loadFlag);
        newAsyncReq.m_PrefabKey = prefabKey;
        newAsyncReq.m_Pos = pos;
        newAsyncReq.m_Rot = rot;
        newAsyncReq.m_UseRotation = useRotation;
        newAsyncReq.m_PoolFlag = poolFlag;
        newAsyncReq.m_ResourceType = resourceType;
        newAsyncReq.m_ResPath = prefabFullPath + "(From Pool By Create)";
        newAsyncReq.m_WaterMark = waterMark;

        if (_HasFlag(poolFlag, GameObjectPoolFlag.HighPriority))
            m_HighPriorityAsyncReqList.Add(newAsyncReq);
        else
            m_AsyncReqList.Add(newAsyncReq);

        return newAsyncReq;
    }

    public void ExecuteClearPooledObjects()
    {
        if(EngineConfig.useTMEngine)
        {
            Tenmove.Runtime.Unity.ITMUnityGameObjectPool gameObjectPool = _GetGameObjectPool();
            if (null != gameObjectPool)
                gameObjectPool.PurgePool(null,true);
            return;
        }

        m_IsRecycling = true;

        Logger.Log("Execute clear pooled objects!");
        DictionaryView<string, Queue<CPooledGameObjectScript>>.Enumerator enumerator = this.m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, Queue<CPooledGameObjectScript>> current = enumerator.Current;
            Queue<CPooledGameObjectScript> queue = current.Value;
            while (queue.Count > 0)
            {
                CPooledGameObjectScript script = queue.Dequeue();
                if ((script != null) && (script.gameObject != null))
                {
                    //script.gameObject.SetActive(true);
                    UnityEngine.Object.Destroy(script.gameObject);
                }
            }
        }
        this.m_pooledGameObjectMap.Clear();
        m_IsRecycling = false;
        //for (int i = 0, icnt = m_AsyncReqList.Count; i < icnt; ++i)
        //{
        //    GameObjPoolAsyncRequest curReq = m_AsyncReqList[i];
        //    if (null == curReq) continue;
        //
        //}
    }

    public void ExecuteClearPooledObjects(List<string> keyNotClear)
	{
		if (keyNotClear == null)
			ExecuteClearPooledObjects();

        m_IsRecycling = true;
		DictionaryView<string, Queue<CPooledGameObjectScript>>.Enumerator enumerator = this.m_pooledGameObjectMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Queue<CPooledGameObjectScript>> current = enumerator.Current;


			if (!keyNotClear.Contains(current.Key))
			{
                _checkStringInListIgnoreCase(current.Key, keyNotClear);

				Queue<CPooledGameObjectScript> queue = current.Value;

				while (queue.Count > 0)
				{
					CPooledGameObjectScript script = queue.Dequeue();

					if ((script != null) && (script.gameObject != null))
                    {
                        //script.gameObject.SetActive(true);
                        UnityEngine.Object.Destroy(script.gameObject);
					}
				}

				queue.Clear();
			}
		}
		m_IsRecycling = false;
	}

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void _checkStringInListIgnoreCase(string key, List<string> strs)
    {
        if (null == strs)
        {
            return ;
        }

        for (int i = 0; i < strs.Count; ++i)
        {
            if (string.Equals(key, strs[i], StringComparison.OrdinalIgnoreCase))
            {
                Logger.LogErrorFormat("[字符串检查] {0} {1} 有大小写差异", key, strs[i]);
                break;
            }
        }
    }

    public GameObject GetGameObject(string prefabFullPath, enResourceType resourceType, uint poolFlag)
    {
        bool isInit = false;
        return this.GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, poolFlag, out isInit);
    }

    public GameObject GetGameObject(string prefabFullPath, enResourceType resourceType, uint poolFlag, out bool isInit)
    {
        return this.GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, poolFlag, out isInit);
    }

    /// public GameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType, uint poolFlag)
    /// {
    ///     bool isInit = false;
    ///     return this.GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, poolFlag, out isInit);
    /// }
    /// 
    /// public GameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType, uint poolFlag, out bool isInit)
    /// {
    ///     return this.GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, poolFlag, out isInit);
    /// }
    /// 
    /// public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType, uint poolFlag)
    /// {
    ///     bool isInit = false;
    ///     return this.GetGameObject(prefabFullPath, pos, rot, true, resourceType, poolFlag, out isInit);
    /// }
    /// 
    /// public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType, uint poolFlag, out bool isInit)
    /// {
    ///     return this.GetGameObject(prefabFullPath, pos, rot, true, resourceType, poolFlag, out isInit);
    /// }

    private GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, uint poolFlag, out bool isInit)
    {
        if (EngineConfig.useTMEngine)
        {
            isInit = false;
            return GetGameObject(prefabFullPath, 0);
            //return AssetLoader.instance.LoadResAsGameObject(prefabFullPath);
        }

		if (
			#if DEBUG_REPORT_ROOT
			DebugSettings.instance.DisableGameObjPool || 
			#endif
			m_IsRecycling)
        {
            isInit = true;
            GameObject poolGameObj = AssetLoader.instance.LoadResAsGameObject(prefabFullPath);
            if(null != poolGameObj)
            {
                if (!poolGameObj.activeSelf)
                    poolGameObj.SetActive(true);
            }
            return poolGameObj;
        }

        string key = CFileManager.EraseExtension(prefabFullPath);
        Queue<CPooledGameObjectScript> queue = null;
        if (!this.m_pooledGameObjectMap.TryGetValue(key, out queue))
        {
            queue = new Queue<CPooledGameObjectScript>();
            this.m_pooledGameObjectMap.Add(key, queue);
        }
        CPooledGameObjectScript script = null;
        while (queue.Count > 0)
        {
            script = queue.Dequeue();
            if ((script != null) && (script.gameObject != null))
            {
                script.gameObject.transform.SetParent(null, true);
                script.gameObject.transform.position = pos;
                script.gameObject.transform.rotation = rot;
                script.gameObject.transform.localScale = script.m_defaultScale;

                /// if(0 == queue.Count && _HasFlag(poolFlag, GameObjectPoolFlag.ReserveLast))
                /// {
                ///     CPooledGameObjectScript lastScript = _InstantiateGameObjectInst(script.gameObject, resourceType, key);
                ///     queue.Enqueue(lastScript);
                ///     this.HandlePooledMonoBehaviour(lastScript.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);
                ///     lastScript.gameObject.transform.SetParent(this.m_poolRoot.transform, false);
                ///     lastScript.gameObject.SetActive(false);
                ///     lastScript.m_IsRecycled = true;
                /// }

                break;
            }
            script = null;
        }
        if (script == null)
        {
            script = this.CreateGameObject(prefabFullPath, pos, rot, useRotation, resourceType, key);

            /// if (null != script && _HasFlag(poolFlag, GameObjectPoolFlag.ReserveLast))
            /// {
            ///     CPooledGameObjectScript lastScript = _InstantiateGameObjectInst(script.gameObject, resourceType, key);
            ///     queue.Enqueue(lastScript);
            ///     this.HandlePooledMonoBehaviour(lastScript.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);
            ///     lastScript.gameObject.transform.SetParent(this.m_poolRoot.transform, false);
            ///     lastScript.gameObject.SetActive(false);
            ///     lastScript.m_IsRecycled = true;
            /// }
        }

        if (script == null)
        {
            isInit = false;
            return null;
        }

        isInit = script.m_isInit;
        script.m_IsRecycled = false;

        /// script.m_DebugInfo = StackTraceUtility.ExtractStackTrace();

        script.gameObject.SetActive(true);
        this.HandlePooledMonoBehaviour(script.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Get);
        return script.gameObject;
    }

    //public uint GetGameObjectAsync(string prefabFullPath, enResourceType resourceType, OnObjLoadCallback callback, int clientHash)
    public uint GetGameObjectAsync(string prefabFullPath, enResourceType resourceType, uint poolFlag, uint waterMark = 0x0)
    {
        return this._GetGameObjectAsyncHandle(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, poolFlag,waterMark);
    }

    public uint GetGameObjectAsync(string prefabFullPath, Vector3 pos, enResourceType resourceType, uint poolFlag, uint waterMark = 0x0)
    {
        return this._GetGameObjectAsyncHandle(prefabFullPath, pos, Quaternion.identity, false, resourceType, poolFlag,waterMark);
    }

    public uint GetGameObjectAsync(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType, uint poolFlag, uint waterMark = 0x0)
    {
        return this._GetGameObjectAsyncHandle(prefabFullPath, pos, rot, true, resourceType, poolFlag,waterMark);
    }

    public bool IsRequestDone(uint handle)
    {
        IAsyncLoadRequest<UnityEngine.Object> request = m_AsyncRequestHandleAlloc.GetAsyncRequestByHandle(handle);
        if (null != request)
            return request.IsDone();
        else
            Logger.LogError("Asset async-load handle is invalid or expired!");

        return false;
    }

    public UnityEngine.Object ExtractAsset(uint handle)
    {
        IAsyncLoadRequest<UnityEngine.Object> request = m_AsyncRequestHandleAlloc.GetAsyncRequestByHandle(handle);
        if (null != request)
        {
            if (request.IsDone())
            {
                m_AsyncRequestHandleAlloc.RemoveAsyncRequest(handle);
                return request.Extract();
            }
        }

        return null;
    }

    public void AbortRequest(uint handle)
    {
        IAsyncLoadRequest<UnityEngine.Object> request = m_AsyncRequestHandleAlloc.GetAsyncRequestByHandle(handle);
        if (null != request)
        {
            m_AsyncRequestHandleAlloc.RemoveAsyncRequest(handle);
            request.Abort();
        }
    }

    public bool IsValidHandle(uint handle)
    {
        return null != m_AsyncRequestHandleAlloc.GetAsyncRequestByHandle(handle);
    }

    private uint _GetGameObjectAsyncHandle(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, uint poolFlag, uint waterMark)
    {
        if(EngineConfig.useTMEngine)
            throw new Exception("Method expired!");

        /// Debug.LogFormat("Get game object async with path:'{0}'!", prefabFullPath);

        IAsyncLoadRequest<UnityEngine.Object> assetRequest = _GetGameObjectAsync(prefabFullPath, pos, rot, useRotation, resourceType, poolFlag, waterMark);
        if (null != assetRequest)
        {
            return m_AsyncRequestHandleAlloc.AddAsyncRequest(assetRequest);
        }
        else
            Logger.LogErrorFormat("Async load asset [{0}] has failed!", prefabFullPath);

        return uint.MaxValue;
    }

    private IAsyncLoadRequest<UnityEngine.Object> _GetGameObjectAsync(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, uint poolFlag, uint waterMark)
    {
        string key = CFileManager.EraseExtension(prefabFullPath);
        
		if (
			#if DEBUG_REPORT_ROOT
			DebugSettings.instance.DisableGameObjPool || 
			#endif
			m_IsRecycling)
            return this._CreateGameObjectAsync(prefabFullPath, pos, rot, useRotation, resourceType, key, poolFlag,waterMark);

        Queue<CPooledGameObjectScript> queue = null;
        if (!this.m_pooledGameObjectMap.TryGetValue(key, out queue))
        {
            queue = new Queue<CPooledGameObjectScript>();
            this.m_pooledGameObjectMap.Add(key, queue);
        }
        CPooledGameObjectScript script = null;
        while (queue.Count > 0)
        {
            script = queue.Dequeue();
            if ((script != null) && (script.gameObject != null))
            {
                script.gameObject.transform.SetParent(null, true);
                script.gameObject.transform.position = pos;
                script.gameObject.transform.rotation = rot;
                script.gameObject.transform.localScale = script.m_defaultScale;

                /// script.m_DebugInfo = StackTraceUtility.ExtractStackTrace();

                /// if (0 == queue.Count && _HasFlag(poolFlag, GameObjectPoolFlag.ReserveLast))
                /// {
                ///     CPooledGameObjectScript lastScript = _InstantiateGameObjectInst(script.gameObject, resourceType, key);
                ///     queue.Enqueue(lastScript);
                ///     this.HandlePooledMonoBehaviour(lastScript.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Create);
                ///     lastScript.gameObject.transform.SetParent(this.m_poolRoot.transform, false);
                ///     lastScript.gameObject.SetActive(false);
                ///     lastScript.m_IsRecycled = true;
                ///     lastScript.m_IsOriginInVisible = script.m_IsOriginInVisible;
                /// }

                break;
            }
            script = null;
        }
        
        if (script != null)
        {
            //if (!script.m_IsOriginInVisible)
            //{
            //    script.gameObject.SetActive(true);
            //}

            script.m_IsRecycled = false;
            this.HandlePooledMonoBehaviour(script.m_pooledMonoBehaviours, enPooledMonoBehaviourAction.Get);


            /// script.m_DebugInfo = StackTraceUtility.ExtractStackTrace();

            GameObjPoolAsyncRequest newAsyncReq = _AllocAsyncRequest();
            newAsyncReq.m_AssetInstReq = AssetLoader.INVILID_HANDLE;
            newAsyncReq.m_PrefabKey = prefabFullPath;
            newAsyncReq.m_Pos = pos;
            newAsyncReq.m_Rot = rot;
            newAsyncReq.m_UseRotation = useRotation;
            newAsyncReq.m_PoolFlag = poolFlag & ~(uint)GameObjectPoolFlag.ReserveLast;
            newAsyncReq.m_ResourceType = resourceType;

            newAsyncReq.m_IsAbort = false;
            newAsyncReq.m_IsDone = true;
            newAsyncReq.m_ResObject = script.gameObject;
            newAsyncReq.m_Extracted = false;
            newAsyncReq.m_ResPath = prefabFullPath + "(From Pool By Get In Pool)";
            newAsyncReq.m_WaterMark = 0xbeefbeef;

            m_CompleteList.Add(newAsyncReq);
            return newAsyncReq;
        }
        else
        {
            return this._CreateGameObjectAsync(prefabFullPath, pos, rot, useRotation, resourceType, key, poolFlag,waterMark);
        }
    }

    private IPooledMonoBehaviour[] GetPooledMonoBehaviours(GameObject gameObject)
    {
        IPooledMonoBehaviour[] behaviourArray = null;
        MonoBehaviour[] componentsInChildren = gameObject.GetComponentsInChildren<MonoBehaviour>();
        if ((componentsInChildren == null) || (componentsInChildren.Length <= 0))
        {
            return new IPooledMonoBehaviour[0];
        }
        int index = 0;
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (componentsInChildren[i] is IPooledMonoBehaviour)
            {
                index++;
            }
        }
        behaviourArray = new IPooledMonoBehaviour[index];
        index = 0;
        for (int j = 0; j < componentsInChildren.Length; j++)
        {
            IPooledMonoBehaviour behaviour2 = componentsInChildren[j] as IPooledMonoBehaviour;
            if (behaviour2 != null)
            {
                behaviourArray[index] = behaviour2;
                index++;
            }
        }
        return behaviourArray;
    }

    private void HandlePooledMonoBehaviour(IPooledMonoBehaviour[] pooledMonoBehaviours, enPooledMonoBehaviourAction pooledMonoBehaviourAction)
    {
        if (null == pooledMonoBehaviours)
            return;

        for (int i = 0; i < pooledMonoBehaviours.Length; i++)
        {
            IPooledMonoBehaviour behaviour = pooledMonoBehaviours[i];
            if ((behaviour != null) && (behaviour as MonoBehaviour).enabled)
            {
                switch (pooledMonoBehaviourAction)
                {
                    case enPooledMonoBehaviourAction.Create:
                        behaviour.OnCreate();
                        break;

                    case enPooledMonoBehaviourAction.Get:
                        behaviour.OnGet();
                        break;

                    case enPooledMonoBehaviourAction.Recycle:
                        behaviour.OnRecycle();
                        break;
                }
            }
        }
    }

    public override void Init()
    {
        RebuildRoot();
        //GameObject obj2 = GameObject.Find("BootObj");
        //
        //if (obj2 != null)
        //{
        //    this.m_poolRoot.transform.SetParent(obj2.transform);
        //}
    }

    public void RebuildRoot()
    {
        if (null == m_poolRoot)
        {
            this.m_poolRoot = new GameObject("CGameObjectPool");
            this.m_poolRoot.transform.position = new Vector3(0, -1000, 0);
        }
    }

    public void ClearAll()
    {
        ExecuteClearPooledObjects();

        if (null != m_poolRoot)
        {
            GameObject.Destroy(m_poolRoot);
            m_poolRoot = null;
        }
    }

    public void PrepareGameObject(string prefabFullPath, enResourceType resourceType, int amount)
    {
        if (EngineConfig.useTMEngine)
        {
            GameObject cur = GetGameObject(prefabFullPath, 0);
            RecycleGameObjectEx(cur);
            return;
        }

        if (
			#if DEBUG_REPORT_ROOT
			DebugSettings.instance.DisableGameObjPool || 
			#endif
			m_IsRecycling)
            return;

        string key = CFileManager.EraseExtension(prefabFullPath);
        Queue<CPooledGameObjectScript> queue = null;
        if (!this.m_pooledGameObjectMap.TryGetValue(key, out queue))
        {
            queue = new Queue<CPooledGameObjectScript>();
            this.m_pooledGameObjectMap.Add(key, queue);
        }
        if (queue.Count < amount)
        {
            amount -= queue.Count;
            for (int i = 0; i < amount; i++)
            {
                CPooledGameObjectScript item = this.CreateGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, key);
                DebugHelper.Assert(item != null);
                if (item != null)
                {
                    queue.Enqueue(item);
                    item.gameObject.transform.SetParent(this.m_poolRoot.transform, true);
                    item.gameObject.SetActive(false);
                    item.m_IsRecycled = true;
                }
            }
        }
    }

    public void RecycleGameObject(GameObject pooledGameObject)
    {
        this._RecycleGameObject(pooledGameObject, false);
    }

    public void RecyclePreparedGameObject(GameObject pooledGameObject)
    {
        this._RecycleGameObject(pooledGameObject, true);
    }

    public override void UnInit()
    {
        if(m_poolRoot)
        {
            GameObject.Destroy(m_poolRoot);
            m_poolRoot = null;
        }
    }

    public void Update()
    {
        _UpdateAsync();

        s_frameCounter++;
        if (this.m_clearPooledObjects && (this.m_clearPooledObjectsExecuteFrame == s_frameCounter))
        {
            this.ExecuteClearPooledObjects();
            this.m_clearPooledObjects = false;
        }
    }

    public int GetPooledGameObjectNum()
    {
        int res = 0;
        DictionaryView<string, Queue<CPooledGameObjectScript>>.Enumerator enumerator = this.m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, Queue<CPooledGameObjectScript>> current = enumerator.Current;
            Queue<CPooledGameObjectScript> queue = current.Value;
            res += queue.Count;
        }

        return res;
    }

    public void DumpGameObjectInfo(ref List<string> assetList)
    {
        assetList.Clear();
        DictionaryView<string, Queue<CPooledGameObjectScript>>.Enumerator enumerator = this.m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, Queue<CPooledGameObjectScript>> current = enumerator.Current;
            Queue<CPooledGameObjectScript> queue = current.Value;

            string content = string.Format("Asset:{0}   (Object Count:{1})", current.Key, queue.Count);
            assetList.Add(content);
        }
    }

    private bool _HasFlag(uint flag,GameObjectPoolFlag eflag)
    {
        return 0 != (flag & (uint)eflag);
    }

    private enum enPooledMonoBehaviourAction
    {
        Create,
        Get,
        Recycle
    }
}

