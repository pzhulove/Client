using System;
using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {
			public static readonly int INVALID_HANDLE = ~0;
			
            private class StackScope : IDisposable
            {
                private bool m_Disposed; // 要检测冗余调用
                private Function<string, float> m_Callback;
                private string m_Asset;
                private long m_TimeStamp;

                public StackScope(string asset, Function<string, float> callback)
                {
                    m_Asset = asset;
                    m_Callback = callback;
                    m_TimeStamp = Utility.Time.GetTicksNow();
                    m_Disposed = false;
                }

                protected void _Dispose(bool disposing)
                {
                    if (!m_Disposed)
                    {
                        if (disposing)
                            m_Callback(m_Asset, Utility.Time.TicksToMicroseconds(Utility.Time.GetTicksNow() - m_TimeStamp));

                        m_Disposed = true;
                    }
                }

                public void Dispose()
                {
                    _Dispose(true);
                }
            }

            private struct AsyncLoadFailedCallbackContext
            {
                public OnAssetLoadFailure OnAssetLoadFailure { set; get; }
                public string AssetPath { set; get; }
                public int TaskGroupID { set; get; }
                public AssetLoadErrorCode AssetLoadErrorCode { set; get; }
                public string Message { set; get; }
                public object UserData { set; get; }
            }

            private struct SyncLoadCallbackContext
            {
                public string AssetPath { set; get; }
                public AssetLoadCallbacks<object> AsyncLoadCallbacks { set; get; }
                public object UserData { set; get; }
                public object Asset { set; get; }
                public int TaskGroupID { set; get; }
            }

            private class AssetByteLoadCallbackContext
            {
                public AssetLoadCallbacks<byte[]> AsyncLoadCallbacks { set; get; }
                public object UserData { set; get; }
            }

            private readonly AssetManager m_AssetManager;

            private readonly TaskPool<LoadTaskBase> m_TaskPool;
            private readonly ITMReferencePool<Asset> m_AssetPool;
            private readonly ITMReferencePool<AssetPackage> m_AssetPackagePool;
            private readonly string m_ResAsyncLoaderTypeName;
            private readonly string m_ResSyncLoaderTypeName;

            private readonly TMResSyncLoader m_ResSyncLoader;
			private readonly AssetByteLoader m_AssetByteLoader;
            private readonly LinkedList<TMResAsyncLoader> m_ResAsyncLoaderList;

            private readonly List<AsyncLoadFailedCallbackContext> m_LoadFailedCallbackContext;
            private readonly List<SyncLoadCallbackContext> m_SyncLoadCallbackContext;

            private uint m_TaskGroupIDCnt;
            private readonly uint m_HandleType;
            private bool m_IsPurgingAsset;

            private Function<string, float> m_OnLoadAsset;
            private Function<string, float> m_OnLoadAssetPackage;

            private readonly string[] m_InvalidDependencyAssetName = new string[0];

            public AssetLoader(AssetManager assetManager, ITMReferencePool<Asset> assetPool, ITMReferencePool<AssetPackage> assetPackagePool, string resAsyncLoaderTypeName, string resSyncLoaderTypeName,string assetByteLoaderTypeName)
            {
                if (null == assetManager)
                    Debugger.AssertFailed("Asset manager can not be null!");

                if (null == assetPool)
                    Debugger.AssertFailed("Asset pool can not be null!");

                if (null == assetPackagePool)
                    Debugger.AssertFailed("Asset package pool can not be null!");

                if (string.IsNullOrEmpty(resAsyncLoaderTypeName))
                    Debugger.AssertFailed("Res-async loader type name can not be null or empty!");

                if (string.IsNullOrEmpty(resAsyncLoaderTypeName))
                    Debugger.AssertFailed("Res-sync loader type name can not be null or empty!");

                m_TaskGroupIDCnt = 0;
                m_HandleType = 0;
                m_IsPurgingAsset = false;

                m_AssetManager = assetManager;
                m_TaskPool = new TaskPool<LoadTaskBase>();
                m_AssetPool = assetPool;
                m_AssetPackagePool = assetPackagePool;
                m_ResAsyncLoaderTypeName = resAsyncLoaderTypeName;
                m_ResSyncLoaderTypeName = resSyncLoaderTypeName;
                m_ResAsyncLoaderList = new LinkedList<TMResAsyncLoader>();
                m_ResSyncLoader = _CreateResourceLoader(resSyncLoaderTypeName) as TMResSyncLoader;
                if (null == m_ResSyncLoader)
                    Debugger.AssertFailed("Create resource sync-loader has failed!");

				m_AssetByteLoader = _CreateAssetByteLoader(assetByteLoaderTypeName);

                m_AsyncLoadRecordAsset = new AsyncLoadRecorder();
                m_AsyncLoadRecordPackage = new AsyncLoadRecorder();

                m_LoadFailedCallbackContext = new List<AsyncLoadFailedCallbackContext>();
                m_SyncLoadCallbackContext = new List<SyncLoadCallbackContext>();
                m_TaskGroupIDCnt = 0;
            }

            public event Function<string, float> OnLoadAsset {
                add { m_OnLoadAsset += value; }
                remove { m_OnLoadAsset -= value; }
            }

            public event Function<string, float> OnLoadAssetPackage {
                add { m_OnLoadAssetPackage += value; }
                remove { m_OnLoadAssetPackage -= value; }
            }

            public string ReadOnlyPath {
                get { return m_AssetManager.ReadOnlyPath; }
            }

            public string ReadWritePath {
                get { return m_AssetManager.ReadWritePath; }
            }

            public string PackageRootFolder {
                get { return m_AssetManager.PackageRootFolder; }
            }

            public int LoadAgentTotalCount {
                get { return m_TaskPool.TotalAgentCount; }
            }

            public int LoadAgentBaseCount {
                get { return m_TaskPool.LoadAgentBaseCount; }
            }

            public int LoadAgentExtraCount {
                get { return m_TaskPool.LoadAgentExtraCount; }
            }

            public int FreeAgentCount {
                get { return m_TaskPool.FreeAgentCount; }
            }

            public int WorkingAgentCount {
                get { return m_TaskPool.WorkingAgentCount; }
            }

            public int WaitingTaskCount {
                get { return m_TaskPool.WaitingTaskCount; }
            }

            public TMResSyncLoader ResSyncLoader {
                get { return m_ResSyncLoader; }
            }

            public bool IsAsyncInLoading {
                get { return m_AsyncLoadRecordAsset.Count > 0 || m_AsyncLoadRecordPackage.Count > 0; }
            }

            public bool IsAssetExist(string assetName, System.Type assetType, bool loadFromPackage)
            {
                string mainResPath;
                string subResName;
                _ParseAssetPath(assetName, out mainResPath, out subResName);

                int assetPackageID = INVALID_HANDLE;
                string assetNameInPackage = null;
                AssetPackageDesc? assetPackageDesc = null;
                return _CheckAsset(mainResPath, loadFromPackage, out assetPackageDesc, out assetNameInPackage, out assetPackageID);
            }

            public byte[] LoadAssetByte(string assetName, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                object nativeAsset = LoadAsset(assetName, m_AssetByteLoader.NativeByteAssetType, userData, loadFromPackage, uFlag);
                return m_AssetByteLoader.LoadAssetByte(nativeAsset).AssetBytes;
            }

            public int LoadAssetByteSync(string assetName, AssetLoadCallbacks<byte[]> fileLoadCallback, object userData, bool loadFromPackage)
            {
                return LoadAssetSync(assetName, m_AssetByteLoader.NativeByteAssetType, 
                    new AssetLoadCallbacks<object>(_OnByteAssetLoadSuccess, _OnByteAssetLoadFailure),
                    new AssetByteLoadCallbackContext() { AsyncLoadCallbacks = fileLoadCallback, UserData = userData }, loadFromPackage);
            }

            public int LoadAssetByteAsync(string assetName, AssetLoadCallbacks<byte[]> fileLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                return LoadAssetAsync(assetName, m_AssetByteLoader.NativeByteAssetType,
                    new AssetLoadCallbacks<object>(_OnByteAssetLoadSuccess, _OnByteAssetLoadFailure),
                    new AssetByteLoadCallbackContext() { AsyncLoadCallbacks = fileLoadCallback, UserData = userData }, loadFromPackage, priorityLevel);
            }

            public object LoadAsset(string assetName, System.Type assetType, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                return _LoadAsset(assetName, assetType,new AssetObjectDesc(), userData, loadFromPackage, uFlag);
            }

            public object LoadAsset(string assetName, System.Type assetType, Transform transform, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                return _LoadAsset(assetName, assetType,new AssetObjectDesc() { Transform = transform}, userData, loadFromPackage, uFlag);
            }

            public object LoadAsset(string assetName, System.Type assetType, object parent, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                return _LoadAsset(assetName, assetType,new AssetObjectDesc(parent), userData, loadFromPackage, uFlag);
            }

            public object LoadAsset(string assetName, System.Type assetType, object parent, Transform transform, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                return _LoadAsset(assetName, assetType,new AssetObjectDesc(parent) { Transform = transform}, userData, loadFromPackage, uFlag);
            }

            public int LoadAssetSync(string assetName, System.Type assetType, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage)
            {
                return _LoadAssetSync(assetName, assetType, new AssetObjectDesc(), assetLoadCallback, userData, loadFromPackage);
            }

            public int LoadAssetSync(string assetName, System.Type assetType, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage)
            {
                return _LoadAssetSync(assetName, assetType, new AssetObjectDesc() { Transform = transform}, assetLoadCallback, userData, loadFromPackage);
            }

            public int LoadAssetSync(string assetName, System.Type assetType, object parent, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage)
            {
                return _LoadAssetSync(assetName, assetType, new AssetObjectDesc(parent), assetLoadCallback, userData, loadFromPackage);
            }

            public int LoadAssetSync(string assetName, System.Type assetType, object parent, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage)
            {
                return _LoadAssetSync(assetName, assetType, new AssetObjectDesc(parent) { Transform = transform}, assetLoadCallback, userData, loadFromPackage);
            }

            public int LoadAssetAsync(string assetName, System.Type assetType,AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                return _LoadAssetAsync(assetName, assetType, new AssetObjectDesc(), assetLoadCallback, userData, loadFromPackage, priorityLevel);
            }

            public int LoadAssetAsync(string assetName, System.Type assetType, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                return _LoadAssetAsync(assetName, assetType, new AssetObjectDesc() { Transform = transform}, assetLoadCallback, userData, loadFromPackage, priorityLevel);
            }

            public int LoadAssetAsync(string assetName, System.Type assetType,object parent, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                return _LoadAssetAsync(assetName, assetType, new AssetObjectDesc(parent), assetLoadCallback, userData, loadFromPackage, priorityLevel);
            }

            public int LoadAssetAsync(string assetName, System.Type assetType, object parent, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                return _LoadAssetAsync(assetName, assetType, new AssetObjectDesc(parent) {Transform = transform }, assetLoadCallback, userData, loadFromPackage, priorityLevel);
            }

            /// <summary>
            /// 预加载资源，即使是GameObject，也不会实例化
            /// </summary>
            /// <param name="assetName"></param>
            /// <param name="assetType"></param>
            /// <param name="userData"></param>
            /// <param name="loadFromPackage"></param>
            /// <param name="uFlag"></param>
            /// <returns></returns>
            public bool PreLoadAsset(string assetName, System.Type assetType, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                //if (m_IsPurgingAsset)
                //{
                //    Debugger.LogError("Can not preload asset '{0}' during GC!",assetName);
                //    return false;
                //}

                using (new StackScope(assetName, _OnAssetLoad))
                {
                    string mainResPath;
                    string subResName;
                    _ParseAssetPath(assetName, out mainResPath, out subResName);

                    EnumHelper<AssetLoadFlag> loadFlag = new EnumHelper<AssetLoadFlag>(uFlag);
                    Asset asset = m_AssetPool.Spawn(assetName);
                    if (null != asset)
                        return true;

                    object assetObj = null;
                    AssetPackage assetPackage = null;
                    string assetNameInPackage = null;
                    int assetPackageID = ~0;

                    AssetPackageDesc? assetPackageDesc = null;
                    if (!_CheckAsset(mainResPath, loadFromPackage, out assetPackageDesc, out assetNameInPackage, out assetPackageID))
                    {
                        Debugger.LogWarning("Asset with name [{0}] check failed!", assetName);
                        return false;
                    }

                    if (loadFromPackage)
                    {
                        List<AssetPackage> assetPackageList = null;
                        if (assetPackageDesc.HasValue && !string.IsNullOrEmpty(assetNameInPackage))
                        {
                            if (!_LoadPackageSync(assetPackageID, false, userData, ref assetPackageList, ref assetPackage))
                            {
                                Debugger.LogError("Can not load dependency package '{0}' when load asset '{1}'!", assetPackageDesc.Value.PackageName.Name, assetName);
                                return false;
                            }
                        }
                    }

                    if (null == assetPackage)
                        assetObj = m_ResSyncLoader.LoadAsset(null, mainResPath, subResName, assetType);
                    else
                        assetObj = m_ResSyncLoader.LoadAsset(assetPackage.Object, assetNameInPackage, subResName, assetType);

                    if (null == assetObj)
                        return false;

                    asset = new Asset(assetName, assetObj as ITMAssetObject, assetPackage, m_AssetPackagePool);
                    m_AssetPool.Register(asset, true);

                    return true;
                }
            }

            /// <summary>
            /// 加载资源，如果是GameObject，会实例化
            /// </summary>
            /// <param name="assetName"></param>
            /// <param name="assetType"></param>
            /// <param name="userData"></param>
            /// <param name="loadFromPackage"></param>
            /// <param name="uFlag"></param>
            /// <returns></returns>

            public void QurreyAssetPackage(string assetName, List<string> packages)
            {
                _QureyAssetPackages(assetName, packages);
            }
            
            public void OnUnloadAssetStateChanged(bool isPurgingAsset)
            {
                m_IsPurgingAsset = isPurgingAsset;
            }

            public void SetAssetLoadAgentCount(int baseCount, int extraCountPerPriority)
            {
                _ClearAssetLoadAgent();

                //if (baseCount > 64)
                //    baseCount = 64;

                // 创建Normal优先级可用Agent
                for (int i = 0, icnt = baseCount; i < icnt; ++i)
                {
                    LoadTaskAgent agent = new LoadTaskAgent(m_AssetPool, m_AssetPackagePool, this);
                    m_TaskPool.AddAgent(agent);
                }

                // 创建其他优先级额外可用Agent
                for (int i = (int)AssetLoadPriority.Normal + 1, icnt = (int)AssetLoadPriority.Max_Num; i < icnt; ++i)
                {
                    for (int j = 0; j < extraCountPerPriority; ++j)
                    {
                        LoadTaskAgent agent = new LoadTaskAgent(m_AssetPool, m_AssetPackagePool, this);
                        m_TaskPool.AddAgent(agent);
                    }
                }

                m_TaskPool.LoadAgentBaseCount = baseCount;
                m_TaskPool.LoadAgentExtraCount = extraCountPerPriority;
            }

            /// <summary>
            /// 加载资源器轮询。
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
            public void Update(float elapseSeconds, float realElapseSeconds)
            {

                List<AsyncLoadFailedCallbackContext> callbackList = FrameStackList<AsyncLoadFailedCallbackContext>.Acquire();
                callbackList.AddRange(m_LoadFailedCallbackContext);
                m_LoadFailedCallbackContext.Clear();

                for (int i = 0,icnt = callbackList.Count;i<icnt;++i)
                {
                    AsyncLoadFailedCallbackContext cur = callbackList[i];
                    cur.OnAssetLoadFailure( cur.AssetPath, cur.TaskGroupID,
                        cur.AssetLoadErrorCode, cur.Message,
                        cur.UserData );
                }
                FrameStackList<AsyncLoadFailedCallbackContext>.Recycle(callbackList);

                List<SyncLoadCallbackContext> contextList = FrameStackList<SyncLoadCallbackContext>.Acquire();
                contextList.AddRange(m_SyncLoadCallbackContext);
                m_SyncLoadCallbackContext.Clear();

                for (int i = 0,icnt = contextList.Count;i<icnt;++i)
                {
                    SyncLoadCallbackContext context = contextList[i];
                    object asset = context.Asset;
                    if (null != asset)
                    {
                        context.AsyncLoadCallbacks.OnAssetLoadSuccess(
                            context.AssetPath,
                            asset,
                            context.TaskGroupID,
                            0,
                            context.UserData
                            );
                    }
                    else
                    {
                        context.AsyncLoadCallbacks.OnAssetLoadFailure(
                            context.AssetPath,
                            context.TaskGroupID,
                            AssetLoadErrorCode.NotExist,
                            string.Format("Sync load asset with path '{0}' has failed!", context.AssetPath),
                            context.UserData
                            );
                    }
                }
                FrameStackList<SyncLoadCallbackContext>.Recycle(contextList);

				if (m_IsPurgingAsset)
                {
                    Debugger.LogInfo("During garbage collection!,pause all loading operation!");
                    return;
                }
				
                m_TaskPool.Update(elapseSeconds, realElapseSeconds);
            }

            private void _ClearAssetLoadAgent()
            {
                m_TaskPool.ClearFreeAgent();
            }

            private int _LoadAssetSync(string assetName, System.Type assetType, AssetObjectDesc desc, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage)
            {
                if (string.IsNullOrEmpty(assetName))
                {
                    m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                    {
                        OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                        AssetPath = assetName,
                        TaskGroupID = INVALID_HANDLE,
                        AssetLoadErrorCode = AssetLoadErrorCode.InvalidParam,
                        Message = "Parameter 'assetName' can not be null or empty string!",
                        UserData = userData
                    });

                    return INVALID_HANDLE;
                }

                if (!_CheckAssetPathValid(assetName))
                {
                    m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                    {
                        OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                        AssetPath = assetName,
                        TaskGroupID = INVALID_HANDLE,
                        AssetLoadErrorCode = AssetLoadErrorCode.InvalidParam,
                        Message = string.Format("Asset with path [{0}] check path format has failed!", assetName),
                        UserData = userData
                    });

                    return INVALID_HANDLE;
                }

                if (loadFromPackage && !(m_AssetManager.m_AssetDescTableIsReady && m_AssetManager.m_AssetPackageDescTableIsReady))
                {
                    m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                    {
                        OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                        AssetPath = assetName,
                        TaskGroupID = INVALID_HANDLE,
                        AssetLoadErrorCode = AssetLoadErrorCode.NotReady,
                        Message = string.Format("Can not load asset '{0}' since asset desc table is not ready!", assetName),
                        UserData = userData
                    });

                    return INVALID_HANDLE;
                }

                int taskID = (int)_AllocHandle();
                SyncLoadCallbackContext context = new SyncLoadCallbackContext()
                {
                    AssetPath = assetName,
                    Asset = _LoadAsset(assetName, assetType,desc, userData, loadFromPackage),
                    AsyncLoadCallbacks = assetLoadCallback,
                    UserData = userData,
                    TaskGroupID = taskID,
                };

                m_SyncLoadCallbackContext.Add(context);
                return taskID;
            }

            private object _LoadAsset(string assetName, System.Type assetType,AssetObjectDesc desc, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                //if (m_IsPurgingAsset)
                //{
                //    Debugger.LogError("Can not load asset '{0}' during GC!", assetName);
                //    //return null;
                //}
                using (new StackScope(assetName, _OnAssetLoad))
                {
                    if (string.IsNullOrEmpty(assetName))
                    {
                        Debugger.LogWarning("Parameter 'assetName' can not be null or empty string!");
                        return null;
                    }

                    if (!_CheckAssetPathValid(assetName))
                    {
                        Debugger.LogWarning("Asset with path [{0}] check path format has failed!", assetName);
                        return null;
                    }
                    
                    if(loadFromPackage && !(m_AssetManager.m_AssetDescTableIsReady && m_AssetManager.m_AssetPackageDescTableIsReady))
                    {
                        Debugger.LogWarning("Can not load asset '{0}' since asset desc table is not ready!", assetName);
                        return null;
                    }
                    string mainResPath;
                    string subResName;
                    _ParseAssetPath(assetName, out mainResPath, out subResName);

                    LoadTaskAgent loadAgent = m_AsyncLoadRecordAsset.FindRecordAgent(assetName);
                    if (null != loadAgent)
                    {/// 说明该资源正在异步加载
                     /// 强制同步加载完成
                        if (!loadAgent.ForceSyncLoadAsset())
                        {
                            Debugger.LogWarning("Asset with name [{0}] sync load failed!", assetName);
                            return null;
                        }
                    }
                    EnumHelper<AssetLoadFlag> loadFlag = new EnumHelper<AssetLoadFlag>(uFlag);
                    Asset asset = m_AssetPool.Spawn(assetName);
                    if (null != asset)
                        return asset.CreateAssetInst(desc);

                    object assetObj = null;
                    AssetPackage assetPackage = null;
                    string assetNameInPackage = null;
                    int assetPackageID = INVALID_HANDLE;

                    AssetPackageDesc? assetPackageDesc = null;
                    if (!_CheckAsset(mainResPath, loadFromPackage, out assetPackageDesc, out assetNameInPackage, out assetPackageID))
                    {
                        Debugger.LogWarning("Asset with name [{0}] check failed!", assetName);
                        return null;
                    }

                    if (loadFromPackage)
                    {
                        List<AssetPackage> assetPackageList = null;
                        if (assetPackageDesc.HasValue && !string.IsNullOrEmpty(assetNameInPackage))
                        {
                            if (!_LoadPackageSync(assetPackageID, false, userData, ref assetPackageList, ref assetPackage))
                            {
                                Debugger.LogError("Can not load dependency package '{0}' when load asset '{1}'!", assetPackageDesc.Value.PackageName.Name, assetName);
                                return null;
                            }
                        }
                    }

                    if (null == assetPackage)
                        assetObj = m_ResSyncLoader.LoadAsset(null, mainResPath, subResName, assetType);
                    else
                        assetObj = m_ResSyncLoader.LoadAsset(assetPackage.Object, assetNameInPackage, subResName, assetType);

                    if (null == assetObj)
                        return null;

                    asset = new Asset(assetName, assetObj as ITMAssetObject, assetPackage, m_AssetPackagePool);
                    m_AssetPool.Register(asset, true);
                    return asset.CreateAssetInst(desc);
                }
          }

            private int _LoadAssetAsync(string assetName, System.Type assetType, AssetObjectDesc desc, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
				_OnAssetLoad(assetName, 0);
                if(string.IsNullOrEmpty(assetName))
                {
                    m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                    {
                        OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                        AssetPath = assetName,
                        TaskGroupID = INVALID_HANDLE,
                        AssetLoadErrorCode = AssetLoadErrorCode.InvalidParam,
                        Message = "Parameter 'assetName' can not be null or empty string!",
                        UserData = userData
                    });

                    return INVALID_HANDLE;
                }

                if (!_CheckAssetPathValid(assetName))
                {
                    m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                    {
                        OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                        AssetPath = assetName,
                        TaskGroupID = INVALID_HANDLE,
                        AssetLoadErrorCode = AssetLoadErrorCode.InvalidParam,
                        Message = string.Format("Asset with path [{0}] check path format has failed!", assetName),
                        UserData = userData
                    });

                    return INVALID_HANDLE;
                }

                if (loadFromPackage && !(m_AssetManager.m_AssetDescTableIsReady && m_AssetManager.m_AssetPackageDescTableIsReady))
                {
                    m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                    {
                        OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                        AssetPath = assetName,
                        TaskGroupID = INVALID_HANDLE,
                        AssetLoadErrorCode = AssetLoadErrorCode.NotReady,
                        Message = string.Format("Can not load asset '{0}' since asset desc table is not ready!", assetName),
                        UserData = userData
                    });

                    return INVALID_HANDLE;
                }

                string mainResPath;
                string subResName;
                _ParseAssetPath(assetName, out mainResPath, out subResName);

                AssetPackageDesc? assetPackageDesc = null;
                string assetNameInPackage = null;
                int assetPackageID = INVALID_HANDLE;

                if (!_CheckAsset(mainResPath, loadFromPackage, out assetPackageDesc, out assetNameInPackage, out assetPackageID))
                {
                    m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                    {
                        OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                        AssetPath = assetName,
                        TaskGroupID = INVALID_HANDLE,
                        AssetLoadErrorCode = AssetLoadErrorCode.NotExist,
                        Message = string.Format("Asset with name [{0}] check asset has failed!", assetName),
                        UserData = userData
                    });

                    return INVALID_HANDLE;
                }

                AssetTask mainTask = new AssetTask(this, m_AssetPool, m_AssetPackagePool, mainResPath, mainResPath, subResName, assetType, desc,assetLoadCallback, assetPackageDesc.Value,
                    assetNameInPackage, (int)_AllocHandle(), userData);

                if (loadFromPackage)
                {
                    if (!_LoadPackageAsync(assetPackageID, false, mainTask, userData, priorityLevel))
                    {
                        string errorMessage = string.Format("Can not load package '{0}' when load asset '{1}'!", assetPackageDesc.Value.PackageName.Name, assetName);
                        m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                        {
                            OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                            AssetPath = assetName,
                            TaskGroupID = INVALID_HANDLE,
                            AssetLoadErrorCode = AssetLoadErrorCode.DependencyLoadError,
                            Message = errorMessage,
                            UserData = userData
                        });

                        return INVALID_HANDLE;
                    }
                }

                m_TaskPool.AddTask(mainTask, priorityLevel);
                return mainTask.TaskGroupID;
            }
            private bool _LoadPackageSync(int assetPackageDescID, bool asDependency, object userData, ref List<AssetPackage> assetPackageList, ref AssetPackage assetPackage)
            {
                AssetPackageDesc assetPackageDesc;
                if (m_AssetManager.m_AssetPackageDescTable.TryGetValueAt(assetPackageDescID, out assetPackageDesc))
                {
                    using (new StackScope(assetPackageDesc.PackageName.Name, _OnAssetPackageLoad))
                    {
                        AssetPackage curAssetPackage = m_AssetPackagePool.Spawn(assetPackageDesc.PackageName.Name);
                        if (null != curAssetPackage)
                        {
                            if (asDependency)
                            {
                                assetPackageList.Add(curAssetPackage);
                                return true;
                            }
                            else
                            {
                                if (!curAssetPackage.AsDependency)
                                {
                                    assetPackage = curAssetPackage;
                                    return true;
                                }
                            }
                        }

                        if (!asDependency)
                        {
                            /// 要重新加载依赖
                            assetPackageList = new List<AssetPackage>();
                            int[] dependencyPackageIDs = assetPackageDesc.DependencyPackageIDs;
                            for (int i = 0, icnt = dependencyPackageIDs.Length; i < icnt; ++i)
                            {
                                if (!_LoadPackageSync(dependencyPackageIDs[i], true, userData, ref assetPackageList, ref assetPackage))
                                {
                                    Debugger.LogError("Can not load dependency package when load asset package'{0}'!", assetPackageDesc.PackageName.Name);
                                    return false;
                                }
                            }
                        }

                        if (null == curAssetPackage)
                        {
                            string packageFullName = assetPackageDesc.PackageName.Name;
                            /// string packageLoadPath = Utility.Path.Combine(
                            ///     assetPackageDesc.StorageInReadOnly ? ReadOnlyPath : ReadWritePath,
                            ///     Utility.Path.Combine(PackageRootFolder, packageFullName));
                            /// object package = m_ResSyncLoader.LoadPackage(packageLoadPath);

                            string packageLoadPath = Utility.Path.Combine(ReadWritePath,
                                Utility.Path.Combine(PackageRootFolder, packageFullName));
                            if (!AssetManager._IsAssetExistWithPath(packageLoadPath))
                            {
                                packageLoadPath = Utility.Path.Combine(ReadOnlyPath,
                                    Utility.Path.Combine(PackageRootFolder, packageFullName));
                            }
#if USE_SMALLPACKAGE && UNITY_EDITOR
                        ExceptionManager.instance.PrintABPackage(packageLoadPath);
#endif

                            object package = m_ResSyncLoader.LoadPackage(packageLoadPath);

                            if (null == package)
                                return false;

                            curAssetPackage = new AssetPackage(packageFullName, package, m_ResSyncLoader, m_AssetPackagePool);
                            m_AssetPackagePool.Register(curAssetPackage, true);
                        }

                        if (asDependency)
                            assetPackageList.Add(curAssetPackage);
                        else
                            curAssetPackage.AddDependency(assetPackageList);

                        assetPackage = curAssetPackage;
                        return true;
                    }
                }
                else
                    Debugger.LogError("Can not load dependency package when load package '{0}'!", assetPackageDesc.PackageName.Name, assetPackageDesc.PackageName.Name);

                return false;
            }

            private bool _LoadPackageAsync(int assetPackageID, bool asDependency, LoadTaskBase mainTask, object userData, int priorityLevel)
            {
                Debugger.Assert(null != mainTask, "Main task can not be null!");

                AssetPackageDesc assetPackageDesc;
                if (m_AssetManager.m_AssetPackageDescTable.TryGetValueAt(assetPackageID, out assetPackageDesc))
                {
                    _OnAssetPackageLoad(assetPackageDesc.PackageName.Name, 0);
                    PackageTask packageLoadTask = new PackageTask(this, m_AssetPool, m_AssetPackagePool, assetPackageDesc.PackageName.Name, mainTask, assetPackageDesc, userData, asDependency);
                    if (!asDependency)
                    {
                        int[] dependencyPackages = assetPackageDesc.DependencyPackageIDs;
                        for (int i = 0, icnt = dependencyPackages.Length; i < icnt; ++i)
                        {
                            if (!_LoadPackageAsync(dependencyPackages[i], true, packageLoadTask, userData, priorityLevel))
                            {
                                Debugger.LogError("Can not load dependency package when load package '{1}'!", assetPackageDesc.PackageName.Name);
                                return false;
                            }
                        }
                    }

                    m_TaskPool.AddTask(packageLoadTask, priorityLevel);
                    return true;
                }
                else
                    Debugger.LogError("Can not load dependency package when load package '{0}'!", assetPackageDesc.PackageName.Name);

                return false;
            }

            private bool _CheckAsset(string assetName, bool loadFromPackage, out AssetPackageDesc? assetPackageDesc, out string assetNameInPackage, out int assetPackageID)
            {
                assetPackageDesc = default(AssetPackageDesc);
                assetNameInPackage = null;
                assetPackageID = INVALID_HANDLE;

                if (string.IsNullOrEmpty(assetName))
                {
#if UNITY_EDITOR
                    Debugger.LogError("Asset name is invalid!");
#endif
                    return false;
                }

                if (loadFromPackage)
                {
                    AssetDesc targetAssetDesc;
                    AssetPackageDesc targetAssetPackageDesc;
                    if (m_AssetManager.m_AssetDescTable.TryGetValue(Utility.Path.ChangeExtension(assetName, null).ToLower(), out targetAssetDesc))
                    {
                        if (0 <= targetAssetDesc.AssetPackageID && targetAssetDesc.AssetPackageID < m_AssetManager.m_AssetPackageDescTable.Count)
                        {
                            if (m_AssetManager.m_AssetPackageDescTable.TryGetValueAt(targetAssetDesc.AssetPackageID, out targetAssetPackageDesc))
                            {
                                assetPackageDesc = targetAssetPackageDesc;
                                assetPackageID = targetAssetDesc.AssetPackageID;

                                if (assetPackageDesc.Value.AssetPackageUsage.HasFlag(AssetPackageUsage.LoadAssetWithGUIDName))
                                {
                                    assetNameInPackage = targetAssetDesc.AssetGUIDName;
                                }
                                else
                                {
                                    //int assetNameInPackageIndex = assetName.LastIndexOf('/');
                                    //if (assetNameInPackageIndex + 1 >= assetName.Length)
                                    //{
                                    //    Debugger.LogWarning("Invalid asset name [{0}]!", assetName);
                                    //    return false;
                                    //}
                                    //assetNameInPackage = assetName.Substring(assetNameInPackageIndex + 1);
                                    assetNameInPackage = assetName;
                                }

                                return true;
                            }
                        }
                    }

                    return false;
                }
                else
                    return _IsAssetExist(assetName);
            }

            private void _QureyAssetPackages(string assetName, List<string> packages)
            {
                AssetDesc targetAssetDesc;
                AssetPackageDesc targetAssetPackageDesc;
                if (m_AssetManager.m_AssetDescTable.TryGetValue(Utility.Path.ChangeExtension(assetName, null).ToLower(), out targetAssetDesc))
                {
                    if (0 <= targetAssetDesc.AssetPackageID && targetAssetDesc.AssetPackageID < m_AssetManager.m_AssetPackageDescTable.Count)
                    {
                        if (m_AssetManager.m_AssetPackageDescTable.TryGetValueAt(targetAssetDesc.AssetPackageID, out targetAssetPackageDesc))
                        {
                            packages.Add(targetAssetPackageDesc.m_PackageName.Name);
                            if (null != targetAssetPackageDesc.DependencyPackageIDs)
                            {
                                AssetPackageDesc dependencyPackageDesc;
                                for (int i = 0, icnt = targetAssetPackageDesc.DependencyPackageIDs.Length; i < icnt; ++i)
                                {
                                    if (m_AssetManager.m_AssetPackageDescTable.TryGetValueAt(
                                        targetAssetPackageDesc.DependencyPackageIDs[i], out dependencyPackageDesc))
                                    {
                                        packages.Add(dependencyPackageDesc.m_PackageName.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private bool _IsAssetExist(string assetPath)
            {
                return true;
            }

            private ITMResourceLoader _CreateResourceLoader(string resLoaderTypeName)
            {
                ITMResourceLoader targetLoader = (ITMResourceLoader)Utility.Assembly.CreateInstance(resLoaderTypeName);
                if (null != targetLoader)
                {
                    return targetLoader;
                }
                else
                    Debugger.AssertFailed(string.Format("Can not create resource loader with type '{0}'!", resLoaderTypeName));

                return null;
            }

            private AssetByteLoader _CreateAssetByteLoader(string assetByteLoader)
            {
                AssetByteLoader targetLoader = (AssetByteLoader)Utility.Assembly.CreateInstance(assetByteLoader);
                if (null != targetLoader)
                {
                    return targetLoader;
                }
                else
                    Debugger.AssertFailed(string.Format("Can not create asset byte loader with type '{0}'!", assetByteLoader));

                return null;
            }

            public TMResAsyncLoader AllocateResAsyncLoader()
            {
                LinkedListNode<TMResAsyncLoader> first = m_ResAsyncLoaderList.First;
                if (null != first)
                {
                    m_ResAsyncLoaderList.RemoveFirst();
                    return first.Value;
                }

                return _CreateResourceLoader(m_ResAsyncLoaderTypeName) as TMResAsyncLoader;
            }

            public void RecycleAsyncLoader(TMResAsyncLoader resAsyncLoader)
            {
                if (null == resAsyncLoader)
                {
                    Debugger.LogError("Resource loader can not be null!");
                    return;
                }

                m_ResAsyncLoaderList.AddLast(resAsyncLoader);
            }

            public void Shutdown()
            {
                if (null != m_ResSyncLoader)
                    m_ResSyncLoader.Reset();

                LinkedListNode<TMResAsyncLoader> cur = m_ResAsyncLoaderList.First;
                while (null != cur)
                {
                    LinkedListNode<TMResAsyncLoader> next = cur.Next;
                    cur.Value.Reset();
                    m_ResAsyncLoaderList.Remove(cur);
                    cur = next;
                }
                
                if(null != m_TaskPool)
                    m_TaskPool.Shutdown();
            }

            void _ParseAssetPath(string assetPath, out string mainRes, out string subRes)
            {
                mainRes = assetPath;
                subRes = "";
                try
                {
                    int idxSpliter = assetPath.LastIndexOf(':');
                    if (0 <= idxSpliter && idxSpliter < assetPath.Length)
                    {
                        mainRes = assetPath.Substring(0, idxSpliter);
                        subRes = assetPath.Substring(idxSpliter + 1);
                    }
                    else
                    {
                        mainRes = assetPath;
                        subRes = "";
                    }
                    /// 兼容1.0不带后缀的 后面去掉  如果带后缀名 请把打包的生成的描述也加上后缀名
                    mainRes = Utility.Path.ChangeExtension(mainRes, null);
                }
                catch (System.Exception e)
                {
                    Debugger.LogWarning("Pares path has failed with exception:{0}", e.Message);
                }
            }

            string _GetAssetPackageNameByPackageID(int packageID)
            {
                if (0 <= packageID && packageID < m_AssetManager.m_AssetPackageDescTable.Count)
                {
                    return m_AssetManager.m_AssetPackageDescTable[packageID].PackageName.Name;
                }

                return "";
            }

            void _OnByteAssetLoadSuccess(string path, object asset, int taskGrpID, float duration, object userData)
            {
                if(null == userData)
                {
                    Debugger.LogWarning("Byte asset load callback with a null user data!");
                    return;
                }

                AssetByteLoadCallbackContext loadContext = userData as AssetByteLoadCallbackContext;
                if (null != loadContext)
                {
                    AssetByte asetByte = m_AssetByteLoader.LoadAssetByte(asset);
                    loadContext.AsyncLoadCallbacks.OnAssetLoadSuccess(path, asetByte.AssetBytes, taskGrpID, duration, loadContext.UserData);
                }
                else
                    Debugger.LogWarning("User data type [{0}] is not a correct type [{1}]!", userData.GetType(), typeof(AssetByteLoadCallbackContext));
            }

            void _OnByteAssetLoadFailure(string path, int taskGrpID, AssetLoadErrorCode errorCode, string message, object userData)
            {
                if (null == userData)
                {
                    Debugger.LogWarning("Byte asset load callback with a null user data!");
                    return;
                }

                AssetByteLoadCallbackContext loadContext = userData as AssetByteLoadCallbackContext;
                if (null != loadContext)
                {
                    loadContext.AsyncLoadCallbacks.OnAssetLoadFailure(path, taskGrpID,errorCode, message, loadContext.UserData);
                }
                else
                    Debugger.LogWarning("User data type [{0}] is not a correct type [{1}]!", userData.GetType(), typeof(AssetByteLoadCallbackContext));
            }

            void _OnByteAssetLoadUpdate(string path, int taskGrpID, float progress, object userData)
            {
            }


            private bool _CheckAssetPathValid(string assetName)
            {
#if UNITY_EDITOR
                if (assetName.Contains("\\"))
                {
                    Debugger.LogWarning("Asset name {0} has \\, replace with /, load asset failed!!!", assetName);
                    return false;
                }
#endif
                return true;
            }
			
			protected uint _AllocHandle()
            {
                /// 前两位预留
                if (m_TaskGroupIDCnt + 1 >= uint.MaxValue >> 2)
                    m_TaskGroupIDCnt = 0;
                return (m_TaskGroupIDCnt++) | ((m_HandleType & 0x03) << 30);
            }

            private void _OnAssetLoad(string assetName, float duration)
            {
                if (null != m_OnLoadAsset)
                    m_OnLoadAsset(assetName, duration);
            }

            private void _OnAssetPackageLoad(string assetPackageName, float duration)
            {
                if (null != m_OnLoadAssetPackage)
                    m_OnLoadAssetPackage(assetPackageName, duration);
            }
        }
    }
}