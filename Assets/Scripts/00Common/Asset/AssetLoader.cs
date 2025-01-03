
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tenmove.Runtime;

public enum AssetLoadFlag
{
    None = 0x00,
    HideAfterLoad = 0x01,

    HighPriority = 0x10,
}

public class AssetLoader : Singleton<AssetLoader>
{

    public enum AssetLockFlag
    {
        LockGroup_Town = 1,
    }

    /////////////////////////////////////////////////////// New API //////////////////////////////////////////////////////////
    private static Tenmove.Runtime.ITMAssetManager m_AssetManager = null;
    private static Tenmove.Runtime.ITMAssetManager _GetAssetManager()
    {
#if UNITY_EDITOR
        GameObject engineRoot = GameObject.Find("TMEngine");
        if (null == engineRoot)
        {
            engineRoot = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Base/TMEngine"));
            engineRoot.name = "TMEngine";
        }
#endif

        if (null == m_AssetManager)
            m_AssetManager = Tenmove.Runtime.ModuleManager.GetModule<Tenmove.Runtime.ITMAssetManager>();

        return m_AssetManager;
    }

    public static event Function<string, float> OnLoadAsset
    {
        add { _GetAssetManager().OnLoadAsset += value; }
        remove { _GetAssetManager().OnLoadAsset -= value; }
    }

    public static event Function<string, float> OnLoadAssetPackage
    {
        add { _GetAssetManager().OnLoadAssetPackage += value; }
        remove { _GetAssetManager().OnLoadAssetPackage -= value; }
    }

    private AssetInst _LoadRes_TMEngine(string path,System.Type type,bool isMustExist = true,uint flag = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load._LoadRes_TMEngine"))
        {
#endif
        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        object obj = null;
        if (null != assetManager)
            obj = assetManager.LoadAsset(path, type, null, 0);

        AssetInst newInst = new AssetInst(obj as UnityEngine.Object);
        return newInst;
#if ENABLE_PROFILER
        }
#endif
    }

    private bool _PreLoadRes_TMEngine(string path, System.Type type, bool isMustExist = true, uint flag = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load._PreLoadRes_TMEngine"))
        {
#endif
        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        object obj = null;
        if (null != assetManager)
            return assetManager.PreLoadAsset(path, type, null, 0);

        return false;
#if ENABLE_PROFILER
        }
#endif
    }

    private static int _LoadResAsync_TMEngine(string path,System.Type type,Tenmove.Runtime.AssetLoadCallbacks<object> callbacks,object userData,uint flag,uint waterMark)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load._LoadResAsync_TMEngine"))
        {
#endif
        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
            return assetManager.LoadAssetAsync(path, type, callbacks, userData, 1==flag ? 1:0 );

        return ~0;
#if ENABLE_PROFILER
        }
#endif
    }

    public static bool IsResExist(string path, System.Type type,bool loadFromPackage)
    {
        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
            return assetManager.IsAssetExist(path, type, loadFromPackage);

        return false;
    }

    public static void QurreyResPackage(string path, List<string> packages)
    {
        packages.Clear();
        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
        {
            assetManager.QurreyAssetPackage(path, packages);
        }
    }

    [Obsolete("Use Tenmove.Runtime 2.0 API 'LoadAsset' instead.")]
    public static uint LoadResAsync(string path, System.Type type, Tenmove.Runtime.AssetLoadCallbacks<object> callbacks, object userData, uint flag = 0u, uint waterMark = 0x0)
    {
        return (uint)_LoadResAsync_TMEngine(path, type, callbacks, userData, flag, waterMark);
    }

    [Obsolete("Use Tenmove.Runtime 2.0 API 'LoadAsset' instead.")]
    public static uint LoadResAsGameObjectAsync(string path, Tenmove.Runtime.AssetLoadCallbacks<object> callbacks, object userData, uint flag = 0u, uint waterMark = 0x0)
    {
        return (uint)_LoadResAsync_TMEngine(path, typeof(GameObject), callbacks, userData, flag, waterMark);
    }

    [Obsolete("There no necessary to invoke anymore.")]
    public static void AbortLoadRequest(uint handle)
    {

    }

    /////////////////////////////////////////////////////// New API //////////////////////////////////////////////////////////

    static public readonly uint INVILID_HANDLE = ~0u;
    static bool m_AsyncLoadPackageRes = true;

    static public bool AsyncLoadPackageRes
    {
        set { m_AsyncLoadPackageRes = value; }
        get { return m_AsyncLoadPackageRes; }
    }

    protected int m_QureyCnt = 0;
    protected readonly int QUREY_STEP = 2;

    #region 方法


    /// <summary>
    /// 预加载资源
    ///     与LoadRes的差别是对于Prefab类型资源，不会实例化Prefab到GameObject。
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">资源类型（Sprite,Texture2D等静态资源需要传入类型以正确加载资源）</param>
    /// <param name="isMustExist">必须存在 为True在资源不存在时将报错</param>
    /// <returns>资源实例</returns>
    public bool PreLoadRes(string path, System.Type type, bool isMustExist = true, uint flag = 0u)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.PreLoadRes"))
        {
#endif
        if (EngineConfig.useTMEngine)
            return _PreLoadRes_TMEngine(path, type, isMustExist, flag);

        return false;
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 加载资源
    ///     对于Prefab类型资源，会自动实例化Prefab到GameObject。
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">资源类型（Sprite,Texture2D等静态资源需要传入类型以正确加载资源）</param>
    /// <param name="isMustExist">必须存在 为True在资源不存在时将报错</param>
    /// <returns>资源实例</returns>
    [Obsolete("Use Tenmove.Runtime 2.0 API 'LoadAsset' instead.")]
    public AssetInst LoadRes(string path,System.Type type,bool isMustExist = true,uint flag = 0u)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.LoadRes"))
        {
#endif
        if (EngineConfig.useTMEngine)
            return _LoadRes_TMEngine(path, type, isMustExist, flag);

#if UNITY_EDITOR
        _ValidResPath(path);
#endif
        //Logger.LogAsset("Begin load resource from Path:" + path);
        _TickAutoPurgeCnt();

        AssetDesc assetDesc = null;
        if (_GetCachedAssetDesc(path, type,out assetDesc))
        {/// GameObject 和静态资源分开管理。
            if (null != assetDesc)
                return assetDesc.CreateRefInst();
            else
                _RemoveCacheAssetDesc(path,type);
        }

        string mainRes, subRes;
        _ParseAssetPath(path, out mainRes, out subRes);

        {
            /// 检查正在创建的AssetDesc
            //if(AssetAsyncTaskAllocator<AssetResourceRequest>.instance.IsAssetInAsyncLoading(path))
            if(AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.IsResInAsyncLoading(path))
            {
#if UNITY_EDITOR
                Logger.LogErrorFormat("Async load task is already exist,sync load has failed![res:{0}]", path);
#else
                Logger.LogWarningFormat("Async load task is already exist,sync load has failed![res:{0}]", path);
#endif
                return null;
            }
        }

        /// 创建新的AssetDesc
        assetDesc = new AssetDesc();
        if(assetDesc.Init(mainRes, type, subRes))
        {
            _RecordLoadFile(path);
            _AddCachedAssetDesc(path, type, assetDesc);
            return assetDesc.CreateRefInst();
        }
        else
        {
            if(isMustExist)
                Logger.LogErrorFormat("Can not instantiate asset with path \"{0}\"!", path);
            return null;
        }
#if ENABLE_PROFILER
        }
#endif
    }

    [Obsolete("Use Tenmove.Runtime 2.0 API 'LoadAsset' instead.")]
    public AssetInst LoadRes(string path, bool isMustExist = true,uint flag = 0u)
    {
        return LoadRes(path,typeof(UnityEngine.Object), isMustExist,flag);
    }

    /// <summary>
    /// 加载GameObject资源LoadRes的便捷方式
    /// </summary>
    /// <param name="path">Prefab路径</param>
    /// <param name="isMustExist">必须存在 为True在资源不存在时将报错</param>
    /// <returns>实例化后GameObject</returns>
    /// 
    [Obsolete("Use Tenmove.Runtime 2.0 API 'LoadAssetAsGameObject' instead.")]
    public GameObject LoadResAsGameObject(string path,bool isMustExist = true,uint flag = 0u)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.LoadResAsGameObject"))
        {
#endif
        var inst = LoadRes(path, typeof(GameObject) ,isMustExist,flag);

        if(inst == null)
        {
            Logger.LogAssetFormat( "{0} path do not exist!",path);
            return null;
        }

        if(inst.obj == null)
        {
            Logger.LogAssetFormat("{0} path Contain Empty Object!",path);
            return null;
        }

        GameObject obj =  inst.obj as GameObject;

        if(obj == null)
        {
            Logger.LogAssetFormat("{0} path Contain Error Object,{1}",path,inst.obj.GetType().Name);
        }
        
        return obj;
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 加载资源
    ///     对于Prefab类型资源，会自动实例化Prefab到GameObject。
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">资源类型（Sprite,Texture2D等静态资源需要传入类型以正确加载资源）</param>
    /// <param name="callback">资源加载完成回调</param>
    /// <param name="isMustExist">必须存在 为True在资源不存在时将报错</param>
    /// <returns>资源实例</returns>
    [Obsolete("Use Tenmove.Runtime 2.0 API 'LoadAsset' instead.")]
    public uint LoadResAync(string path, System.Type type, bool isMustExist = true,uint flag = 0u, uint waterMark = 0x0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.LoadResAync"))
        {
#endif
        if(EngineConfig.useTMEngine)
            throw new Exception("Method expired!");

        /// Debug.LogFormat("Loading asset async with path:'{0}'!", path);

#if UNITY_EDITOR
        _ValidResPath(path);
#endif
        if (string.IsNullOrEmpty(path))
            return INVILID_HANDLE;

        IAssetInstRequest assetRequest = _LoadResAync(path, type, isMustExist, flag,waterMark);
        if (null != assetRequest)
        {
            return m_AsyncRequestAllocator.AddAsyncRequest(assetRequest);
        }
        else
            Logger.LogErrorFormat("Async load asset [{0}] has failed!", path);

        return INVILID_HANDLE;
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 加载GameObject资源LoadRes的便捷方式
    /// </summary>
    /// <param name="path">Prefab路径</param>
    /// <param name="isMustExist">必须存在 为True在资源不存在时将报错</param>
    /// <returns>实例化后GameObject</returns>
    [Obsolete("Use Tenmove.Runtime 2.0 API 'LoadAssetAsGameObject' instead.")]
    public uint LoadResAsyncAsGameObject(string path, bool isMustExist = true,uint flag = 0u, uint waterMark = 0x0)
    {
        return LoadResAync(path, typeof(GameObject), isMustExist,flag,waterMark);
    }

    [Obsolete("There no necessary to invoke anymore.")]
    public bool IsRequestDone(uint handle)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.IsRequestDone"))
        {
#endif
        IAssetInstRequest request = m_AsyncRequestAllocator.GetAsyncRequestByHandle(handle);
        if (null != request)
            return request.IsDone();
        else
        {
            Logger.LogErrorFormat("Asset async-load handle [0x{0}] is invalid or expired!", handle.ToString("x"));
#if UNITY_EDITOR
            Debug.DebugBreak();
#endif
        }

        return false;
#if ENABLE_PROFILER
        }
#endif
    }

    [Obsolete("There no necessary to invoke anymore.")]
    public void AbortRequest(uint handle)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.AbortRequest"))
        {
#endif
        if (EngineConfig.useTMEngine)
            return;

        IAssetInstRequest request = m_AsyncRequestAllocator.GetAsyncRequestByHandle(handle);
        if (null != request)
        {
            m_AsyncRequestAllocator.RemoveAsyncRequest(handle);
            request.Abort();
        }
        else
            Logger.LogErrorFormat("Asset async-load handle [0x{0}] is invalid or expired!", handle.ToString("x"));
#if ENABLE_PROFILER
        }
#endif
    }

    [Obsolete("There no necessary to invoke anymore.")]
    public AssetInst Extract(uint handle)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.Extract"))
        {
#endif
        IAssetInstRequest request = m_AsyncRequestAllocator.GetAsyncRequestByHandle(handle);
        if (null != request)
        {
            if (request.IsDone())
            {
                m_AsyncRequestAllocator.RemoveAsyncRequest(handle);
                return request.Extract();
            }
        }
        else
            Logger.LogErrorFormat("Asset async-load handle [0x{0}] is invalid or expired!", handle.ToString("x"));

        return null;
#if ENABLE_PROFILER
        }
#endif
    }

    [Obsolete("There no necessary to invoke anymore.")]
    public bool IsValidHandle(uint handle)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.IsValidHandle"))
        {
#endif
        IAssetInstRequest request = m_AsyncRequestAllocator.GetAsyncRequestByHandle(handle);
        if (null == request)
        {
            Logger.LogErrorFormat("Asset async-load handle [0x{0}] is invalid or expired!", handle.ToString("x"));
            return false;
        }
        else
            return true;
#if ENABLE_PROFILER
        }
#endif
    }

    [Obsolete("There no necessary to invoke anymore.")]
    public void SetAsyncLoadStep(int step)
    {
        if (0 < step && step < 8)
            m_AsyncStep = step;
        else
            Logger.LogErrorFormat("Input async load step [{0}] is invalid value!", step);
    }

    [Obsolete("There no necessary to invoke anymore.")]
    public int GetAsyncLoadStep()
    {
        return m_AsyncStep;
    }

    public void SetPurgeTime(float timeLen)
    {
        m_PurgeTime = timeLen;
    }

    public void SetAutoPurgeCount(int cnt)
    {
        m_AutoPurgeCnt = cnt;
    }

    public void ResetPurgeTick()
    {
        m_CurPurgeCnt = 0;
    }

    [Obsolete("There no necessary to invoke anymore.")]
    public bool LockAsset(string assetName, int lockFlag)
    {
        //if (EngineConfig.useTMEngine)
        //{
        //    Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        //    if (null != assetManager)
        //    {
        //        return assetManager.LockAsset(assetName, lockFlag);
        //    }
        //}

        return false;
    }

    public void PurgeUnusedRes(bool ignoreTime = false,Type type = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.PurgeUnusedRes"))
        {
#endif
        if(EngineConfig.useTMEngine)
        {
            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
            {
                assetManager.BeginClearUnusedAssets(false);
                assetManager.EndClearUnusedAssets();
            }

            return;
        }


        Dictionary<string, List<AssetInfo>>.Enumerator enumerator = m_ResDescCacheTableEx.GetEnumerator();

        //List<string> deleteKeyList = new List<string>();
        List<AssetDelKey> deleteKeyList = GamePool.ListPool<AssetDelKey>.Get();// new List<string>();
        while (enumerator.MoveNext())
        {
            List<AssetInfo> value = enumerator.Current.Value;
            if (null == value) continue;
            for (int i = 0, icnt = value.Count; i < icnt; ++i)
            {
                AssetInfo curInfo = value[i];
                if (null == curInfo || null == curInfo.m_AssetDesc)
                {
                    AssetDelKey delKey = new AssetDelKey();
                    delKey.path = enumerator.Current.Key;
                    delKey.type = null;
                    deleteKeyList.Add(delKey);
                    continue;
                }

                if (null != type && type != curInfo.m_AssetDesc.assetType)
                    continue;

                if (curInfo.m_AssetDesc.CanBeRemoved())
                {
                    if (ignoreTime || Time.time - curInfo.m_AssetDesc.lastUseTime > m_PurgeTime)
                    {
                        curInfo.m_AssetDesc.Deinit();
                        AssetDelKey delKey = new AssetDelKey();
                        delKey.path = enumerator.Current.Key;
                        delKey.type = curInfo.m_AssetType;
                        deleteKeyList.Add(delKey);
                    }
                }
            }
        }

        for (int i = 0; i < deleteKeyList.Count; ++i)
            _RemoveCacheAssetDesc(deleteKeyList[i].path, deleteKeyList[i].type);

        GamePool.ListPool<AssetDelKey>.Release(deleteKeyList);
#if ENABLE_PROFILER
        }
#endif
    }

    public void ClearAll(bool force = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load.ClearAll"))
        {
#endif
        Dictionary<string, List<AssetInfo>>.Enumerator enumerator = m_ResDescCacheTableEx.GetEnumerator();

        //List<string> deleteKeyList = new List<string>();
        List<AssetDelKey> deleteKeyList = GamePool.ListPool<AssetDelKey>.Get();// new List<string>();
        while (enumerator.MoveNext())
        {
            List<AssetInfo> value = enumerator.Current.Value;
            if(null == value) continue;
            for(int i = 0,icnt = value.Count;i<icnt;++i)
            {
                AssetInfo curInfo = value[i];
                if (null == curInfo || null == curInfo.m_AssetDesc) continue;

                if(curInfo.m_AssetDesc.CanBeRemoved())
                {
                    curInfo.m_AssetDesc.Deinit();
                    AssetDelKey delKey = new AssetDelKey();
                    delKey.path = enumerator.Current.Key;
                    delKey.type = curInfo.m_AssetType;
                    deleteKeyList.Add(delKey);
                }
                else
                {
                    if (force)
                    {
                        Logger.LogAssetFormat("Asset [{0}] is under using!", curInfo.m_AssetDesc.m_FullPath);

                        curInfo.m_AssetDesc.Deinit();
                        AssetDelKey delKey = new AssetDelKey();
                        delKey.path = enumerator.Current.Key;
                        delKey.type = curInfo.m_AssetType;
                        deleteKeyList.Add(delKey);
                    }
                }
            }
        }

        for (int i = 0; i < deleteKeyList.Count; ++i)
            _RemoveCacheAssetDesc(deleteKeyList[i].path, deleteKeyList[i].type);

        GamePool.ListPool<AssetDelKey>.Release(deleteKeyList);
#if ENABLE_PROFILER
        }
#endif
    }

    #endregion

    #region New API 2020.09.15 Tenmove.Runtime 2.0 
    public static int INVALID_HANDLE = ~0;

    public static string ReadWritePath
    {
        /// 不是写错
        get { return _GetAssetManager().ReadWritePath.ToString(); }
    }

    public static string ReadOnlyPath
    {
        /// 不是写错
        get { return _GetAssetManager().ReadOnlyPath.ToString(); }
    }

    public static bool IsAssetManagerReady()
    {
        if (EngineConfig.useTMEngine)
        {
            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
                return assetManager.IsAssetLoaderReady;

            return false;
        }

        return true;
    }

    public static int LoadAsset(string path, System.Type type, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        if (string.IsNullOrEmpty(path))
            return INVALID_HANDLE;

        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
        {
            if (isAsync)
                return assetManager.LoadAssetAsync(path, type, callbacks, userData, (int)priority);
            else
                return assetManager.LoadAssetSync(path, type, callbacks, userData);
        }

        return INVALID_HANDLE;
    }

    public static int LoadAsset(string path, System.Type type, Tenmove.Runtime.Math.Transform transform, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        if (string.IsNullOrEmpty(path))
            return INVALID_HANDLE;

        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
        {
            if (isAsync)
                return assetManager.LoadAssetAsync(path, type, transform, callbacks, userData, (int)priority);
            else
                return assetManager.LoadAssetSync(path, type, transform, callbacks, userData);
        }

        return INVALID_HANDLE;
    }

    public static int LoadAsset(string path, System.Type type, object parent, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        if (string.IsNullOrEmpty(path))
            return INVALID_HANDLE;

        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
        {
            if (isAsync)
                return assetManager.LoadAssetAsync(path, type, parent, callbacks, userData, (int)priority);
            else
                return assetManager.LoadAssetSync(path, type, parent, callbacks, userData);
        }

        return INVALID_HANDLE;
    }

    public static int LoadAsset(string path, System.Type type, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        if (string.IsNullOrEmpty(path))
            return INVALID_HANDLE;

        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
        {
            if (isAsync)
                return assetManager.LoadAssetAsync(path, type, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData, (int)priority);
            else
                return assetManager.LoadAssetSync(path, type, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData);
        }

        return INVALID_HANDLE;
    }

    public static int LoadAsset(string path, System.Type type, Tenmove.Runtime.Math.Transform transform, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        if (string.IsNullOrEmpty(path))
            return INVALID_HANDLE;

        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
        {
            if (isAsync)
                return assetManager.LoadAssetAsync(path, type, transform, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData, (int)priority);
            else
            {
                return assetManager.LoadAssetSync(path, type, transform, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData);
            }
        }

        return INVALID_HANDLE;
    }

    public static int LoadAsset(string path, System.Type type, object parent, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        if (string.IsNullOrEmpty(path))
            return INVALID_HANDLE;

        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
        {
            if (isAsync)
                return assetManager.LoadAssetAsync(path, type, parent, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData, (int)priority);
            else
            {
                return assetManager.LoadAssetSync(path, type, parent, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData);
            }
        }

        return INVALID_HANDLE;
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <param name="userData"></param>
    /// <param name="onSuccess"></param>
    /// <returns></returns>
    public static int LoadAsset(string path, System.Type type, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, type, userData, onSuccess, _OnLoadFailureDefault, isAsync, priority);
    }

    public static int LoadAsset(string path, System.Type type, Tenmove.Runtime.Math.Transform transform, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, type, transform, userData, onSuccess, _OnLoadFailureDefault, isAsync, priority);
    }

    public static int LoadAsset(string path, System.Type type, object parent, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, type, parent, userData, onSuccess, _OnLoadFailureDefault, isAsync, priority);
    }

    /// <summary>
    /// 加载资源内容
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <param name="userData"></param>
    /// <param name="onSuccess"></param>
    /// <returns></returns>
    public static int LoadAssetByte(string path, object userData, OnAssetLoadSuccess<byte[]> onSuccess, bool isAsync = true)
    {
        return LoadAssetByte(path, userData, onSuccess, _OnLoadFailureDefault, isAsync);
    }

    public static int LoadAssetByte(string path, object userData, OnAssetLoadSuccess<byte[]> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true)
    {
        if (string.IsNullOrEmpty(path))
            return INVALID_HANDLE;

        Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
        if (null != assetManager)
        {
            if (isAsync)
                return assetManager.LoadAssetByteAsync(path, new AssetLoadCallbacks<byte[]>(onSuccess, onFailure), userData, 0);
            else
            {
                byte[] asset = assetManager.LoadAssetByte(path, userData, 0);
                if (null != asset)
                    onSuccess(path, asset, INVALID_HANDLE, 0, userData);
                else
                    return assetManager.LoadAssetByteAsync(path, new AssetLoadCallbacks<byte[]>(onSuccess, onFailure), userData, 0);
            }
        }

        return INVALID_HANDLE;
    }

    public static int LoadAssetAsGameObject(string path, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, typeof(UnityEngine.GameObject), userData, callbacks, isAsync, priority);
    }

    public static int LoadAssetAsGameObject(string path, Tenmove.Runtime.Math.Transform transform, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, typeof(UnityEngine.GameObject), transform, userData, callbacks, isAsync, priority);
    }

    public static int LoadAssetAsGameObject(string path, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, typeof(UnityEngine.GameObject), userData, onSuccess, onFailure, isAsync, priority);
    }

    public static int LoadAssetAsGameObject(string path, Tenmove.Runtime.Math.Transform transform, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, typeof(UnityEngine.GameObject), transform, userData, onSuccess, onFailure, isAsync, priority);
    }

    public static int LoadAssetAsGameObject(string path, object parent, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, typeof(UnityEngine.GameObject), parent, userData, onSuccess, onFailure, isAsync, priority);
    }

    public static int LoadAssetAsGameObject(string path, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, typeof(UnityEngine.GameObject), userData, onSuccess, isAsync, priority);
    }

    public static int LoadAssetAsGameObject(string path, object parent, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, typeof(UnityEngine.GameObject), parent, userData, onSuccess, isAsync, priority);
    }

    public static int LoadAssetAsGameObject(string path, Tenmove.Runtime.Math.Transform transform, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
    {
        return LoadAsset(path, typeof(UnityEngine.GameObject), transform, userData, onSuccess, isAsync, priority);
    }

    private static void _OnLoadFailureDefault(string path, int taskID, AssetLoadErrorCode errorCode, string message, object userData)
    {
        Debugger.LogWarning("Load asset with path '{0}' has failed!(Task ID:{1},Error msg:{2})", path, taskID, message);
    }

    #endregion

    #region 私有方法

    public override void Init()
    {
        base.Init();

        AssetAsyncLoader.instance.Init();

#if UNITY_EDITOR
        m_DumpFile = "ResLoadRecord/FileLoadTrace_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".rec";
        string dumpFileDir = Path.Combine(Application.streamingAssetsPath,Path.GetDirectoryName(m_DumpFile));
        if (Directory.Exists(dumpFileDir))
            Directory.CreateDirectory(dumpFileDir);
#endif
    }

    void _ParseAssetPath(string assetPath,out string mainRes,out string subRes)
    {
        string[] assetKeys = assetPath.Split(':');
        if (assetKeys.Length > 1)
        {
            subRes = assetKeys[1];
            mainRes = assetKeys[0];
        }
        else
        {
            subRes = "";
            mainRes = assetPath;
        }
    }

    private AssetInstRequest _CheckResInLoading(List<AsyncLoadTaskDesc> asyncLoadTaskList,string path, System.Type type, uint flag = 0u, uint waterMark = 0x0 )
    {
        AssetInstRequest instReq = null;
        for (int i = 0; i < asyncLoadTaskList.Count; ++i)
        {
            AsyncLoadTaskDesc curTask = asyncLoadTaskList[i];
            if (path == curTask.m_ResPath && type == curTask.m_ResType)
            {
                instReq = _AllocAssetInstRequest();
                instReq.m_flag = flag;
                instReq.m_waterMark = waterMark;
                curTask.m_WaitingReqList.Add(instReq);
                break;
            }
        }

        return instReq;
    }

    public IAssetInstRequest _LoadResAync(string path, System.Type type, bool isMustExist = true,uint flag = 0u, uint waterMark = 0x0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load._LoadResAync"))
        {
#endif
        _TickAutoPurgeCnt();
        AssetInstRequest instReq = null;
        AssetDesc assetDesc = null;
        if (_GetCachedAssetDesc(path, type, out assetDesc))
        {/// GameObject 和静态资源分开管理。
            instReq = _AllocAssetInstRequest();
            instReq.m_AssetInst = assetDesc.CreateRefInst(flag);
            instReq.m_IsDone = true;
            instReq.m_HasExtract = false;
            instReq.m_flag = flag;
            instReq.m_waterMark = waterMark;

            m_CompletedReqList.Add(instReq);
            return instReq;
        }

        string mainRes, subRes;
        _ParseAssetPath(path, out mainRes, out subRes);

        /// 检查正在创建的AssetDesc
        ///for (int i = 0; i < m_AsyncLoadTaskList.Count; ++i)
        ///{
        ///    AsyncLoadTaskDesc curTask = m_AsyncLoadTaskList[i];
        ///    if (path == curTask.m_ResPath && type == curTask.m_ResType)
        ///    {
        ///        instReq = _AllocAssetInstRequest();
        ///        instReq.m_flag = flag;
        ///        instReq.m_waterMark = waterMark;
        ///        curTask.m_WaitingReqList.Add(instReq);
        ///        return instReq;
        ///    }
        ///}
        ///
        instReq = _CheckResInLoading(m_HighPriorityAsyncLoadTaskList, path, type, flag, waterMark);
        if (null != instReq)
            return instReq;
        instReq = _CheckResInLoading(m_AsyncLoadTaskList, path, type, flag, waterMark);
        if (null != instReq)
            return instReq;

        /// 创建新的AssetDesc
        _RecordLoadFile(path);
        assetDesc = new AssetDesc();
        bool highPriority = (0 != (flag & (uint)AssetLoadFlag.HighPriority));

        assetDesc.InitAsync(path, type, subRes, highPriority);
        AsyncLoadTaskDesc newAsyncTask = new AsyncLoadTaskDesc();
        newAsyncTask.m_AssetDesc = assetDesc;
        newAsyncTask.m_ResPath = path;
        newAsyncTask.m_SubRes = subRes;
        newAsyncTask.m_ResType = type;
        instReq = _AllocAssetInstRequest();
        instReq.m_flag = flag;
        instReq.m_waterMark = waterMark;
        newAsyncTask.m_WaitingReqList.Add(instReq);

        if (0 != (flag & (uint)AssetLoadFlag.HighPriority))
            m_HighPriorityAsyncLoadTaskList.Add(newAsyncTask);
        else
            m_AsyncLoadTaskList.Add(newAsyncTask);
        return instReq;
        /*

        _TickAutoPurgeCnt();
        AssetInstRequest instReq = null;
        AssetDesc assetDesc = null;

        string mainRes, subRes;
        _ParseAssetPath(path, out mainRes, out subRes);

        /// 检查正在创建的AssetDesc
        for (int i = 0; i < m_AsyncLoadTaskList.Count; ++i)
        {
            AsyncLoadTaskDesc curTask = m_AsyncLoadTaskList[i];
            if (path == curTask.m_ResPath && type == curTask.m_ResType)
            {
                instReq = _AllocAssetInstRequest();				
            	instReq.m_flag = flag;
                curTask.m_WaitingReqList.Add(instReq);
                return instReq;
            }
        }


        if (_GetCachedAssetDesc(path, type, out assetDesc))
        {            
            assetDesc.SetHolding(true);
            if(null == assetDesc || null == assetDesc.m_AssetObjRef)
            {
                _RemoveCacheAssetDesc(path, type);
                assetDesc = null;
            }
        }

        if(null == assetDesc)
        {
            /// 创建新的AssetDesc
            _RecordLoadFile(path);
            assetDesc = new AssetDesc(); 
            assetDesc.InitAsync(path, type, subRes);  
        }        
        AsyncLoadTaskDesc newAsyncTask = new AsyncLoadTaskDesc();
        newAsyncTask.m_AssetDesc = assetDesc;
        newAsyncTask.m_ResPath = path;
        newAsyncTask.m_ResType = type;
        instReq = _AllocAssetInstRequest();
		instReq.m_flag = flag;
        newAsyncTask.m_WaitingReqList.Add(instReq);

        m_AsyncLoadTaskList.Add(newAsyncTask);
        return instReq; 

*/
#if ENABLE_PROFILER
        }
#endif
    }

    void _AddCachedAssetDesc(string path, System.Type type,AssetDesc assetDesc)
    {
        List<AssetInfo> assetInfoList = null;
        if (m_ResDescCacheTableEx.TryGetValue(path, out assetInfoList))
        {
            for (int i = 0, icnt = assetInfoList.Count; i < icnt; ++i)
            {
                AssetInfo assetInfo = assetInfoList[i];
                if (null == assetInfo) continue;

                if (assetInfo.m_AssetType == type)
                {
                    Logger.LogErrorFormat("Multiple asset desc with path {0} and type {1}", path, type.Name);
                    return;
                }
            }

            assetInfoList.Add(new AssetInfo(type, assetDesc));
            return;
        }

        assetInfoList = new List<AssetInfo>();
        assetInfoList.Add(new AssetInfo(type, assetDesc));
        m_ResDescCacheTableEx.Add(path, assetInfoList);
    }

    void _RemoveCacheAssetDesc(string path,Type type)
    {
        List<AssetInfo> assetInfoList = null;
        if (m_ResDescCacheTableEx.TryGetValue(path, out assetInfoList))
        {
            if(null == type)
            {
                m_ResDescCacheTableEx.Remove(path);
                return;
            }

            for (int i = 0, icnt = assetInfoList.Count; i < icnt; ++i)
            {
                AssetInfo assetInfo = assetInfoList[i];
                if (null == assetInfo) continue;

                if (assetInfo.m_AssetType == type)
                {
                    assetInfoList.RemoveAt(i);
                    if(assetInfoList.Count <= 0)
                        m_ResDescCacheTableEx.Remove(path);
                    return;
                }
            }
        }

        Debug.LogErrorFormat("########################## Path:[{0}] Type:{1}", path, type);
    }

    bool _GetCachedAssetDesc(string path, System.Type type, out AssetDesc assetDesc)
    {
        assetDesc = null;
        List<AssetInfo> assetInfoList = null;
        if (m_ResDescCacheTableEx.TryGetValue(path, out assetInfoList))
        {
            for (int i = 0, icnt = assetInfoList.Count; i < icnt; ++i)
            {
                AssetInfo assetInfo = assetInfoList[i];
                if (null == assetInfo) continue;

                if (assetInfo.m_AssetType == type)
                {
                    assetDesc = assetInfo.m_AssetDesc;
                    return true;
                }
            }
        }

        return false;
    }

    //bool _GetCachedAssetDescEx(string path,System.Type type , out string resKey, out AssetDesc assetDesc)
    //{
    //    resKey = path + "(" + type.ToString() + ")";
    //    return m_ResDescCacheTable.TryGetValue(resKey, out assetDesc);
    //}

    //int CGCnt = 0;
    void _TickAutoPurgeCnt()
    {
        AssetGabageCollectorHelper.instance.AddGCPurgeTick(AssetGCTickType.Asset);
        ///if(0 != m_AutoPurgeCnt)
        ///{
        ///    if (m_CurPurgeCnt >= m_AutoPurgeCnt)
        ///    {
        ///        //Debug.LogWarningFormat("Tick GC ...... {0}", CGCnt++);
        ///        AssetGabageCollector.instance.ClearUnusedAsset();
        ///        m_CurPurgeCnt = 0;
        ///    }
        ///
        ///    ++ m_CurPurgeCnt;
        ///}
    }

#endregion

#region 变量

    //private DictionaryView<string, AssetDesc> m_ResDescCacheTable = new DictionaryView<string, AssetDesc>();
    class AssetInfo
    {
        public AssetInfo(Type type,AssetDesc desc)
        {
            m_AssetDesc = desc;
            m_AssetType = type;
        }

        public System.Type m_AssetType;
        public AssetDesc m_AssetDesc;
    }
    private Dictionary<string, List<AssetInfo>> m_ResDescCacheTableEx = new Dictionary<string, List<AssetInfo>>();

    struct AssetDelKey
    {
        public string path;
        public Type type;
    }


    private float m_PurgeTime = 30.0f; /// 默认清理时间设置为30秒
    private int m_AutoPurgeCnt = 0;
    private int m_CurPurgeCnt = 0;

    #endregion

    #region 异步加载状态管理

    protected int m_AsyncStep = 1;

    protected List<AssetInstRequest> m_AssetInstReqPool = new List<AssetInstRequest>();
    protected List<AssetInstRequest> m_CompletedReqList = new List<AssetInstRequest>();

    protected class AsyncLoadTaskDesc
    {
        public AssetDesc m_AssetDesc;
        public string m_ResPath;
        public string m_SubRes;
        public Type m_ResType;
        public List<AssetInstRequest> m_WaitingReqList = new List<AssetInstRequest>();
    }

    protected List<AsyncLoadTaskDesc> m_AsyncLoadTaskList = new List<AsyncLoadTaskDesc>();
    protected List<AsyncLoadTaskDesc> m_HighPriorityAsyncLoadTaskList = new List<AsyncLoadTaskDesc>();

    public int CompleteTaskCount
    {
        get { return m_CompletedReqList.Count; }
    }

    public int LoadingTaskCount
    {
        get { return m_AsyncLoadTaskList.Count + m_HighPriorityAsyncLoadTaskList.Count; }
    }

    protected AssetInstRequest _AllocAssetInstRequest()
    {
        AssetInstRequest availableReq = null;
        if(m_AssetInstReqPool.Count > 0)
        {
            availableReq = m_AssetInstReqPool[0];
            m_AssetInstReqPool.RemoveAt(0);
        }

        if(null == availableReq)
            availableReq = new AssetInstRequest();

        availableReq.Reset();
        return availableReq;
    }

    protected void _CheckAsyncLoadTaskList(List<AsyncLoadTaskDesc> asyncLoadTaskList,ref int step)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]Load._CheckAsyncLoadTaskList"))
        {
#endif
        for (int i = 0, icnt = asyncLoadTaskList.Count; i < icnt; ++i)
        {
            AsyncLoadTaskDesc cur = asyncLoadTaskList[i];
            if (null == cur)
            {
                asyncLoadTaskList.RemoveAt(i);
                --i;
                --icnt;
                continue;
            }


            if (!cur.m_AssetDesc.CheckAsyncLoadComplete())
            {
                for (int j = 0, jcnt = cur.m_WaitingReqList.Count; j < jcnt; ++j)
                {
                    AssetInstRequest curReq = cur.m_WaitingReqList[j];
                    if (null != curReq && !curReq.m_IsAbort)
                        continue;

                    cur.m_WaitingReqList.RemoveAt(j);
                    --j;
                    --jcnt;
                }               

                continue;
            }
            
            if (cur.m_AssetDesc.IsDeadAsset)
            {/// 之前的资源被加载完成后还未等到处理又被释放掉了 重新尝试加载资源并调高优先级
                cur.m_AssetDesc.InitAsync(cur.m_ResPath, cur.m_ResType, cur.m_SubRes, true);
                continue;
            }

            AssetDesc dummy = null;
            if (!_GetCachedAssetDesc(cur.m_ResPath, cur.m_ResType, out dummy))
                _AddCachedAssetDesc(cur.m_ResPath, cur.m_ResType, cur.m_AssetDesc);
            
            for (int j = 0, jcnt = cur.m_WaitingReqList.Count; j < jcnt; ++j)
            {
                AssetInstRequest curReq = cur.m_WaitingReqList[j];
                if (null != curReq)
                {
                    if (!curReq.m_IsAbort)
                    {
                        curReq.m_AssetInst = cur.m_AssetDesc.CreateRefInst(curReq.m_flag);
                        curReq.m_IsDone = true;
                        m_ReqCompleteList.Add(curReq);
                        --step;
                    }
                }

                cur.m_WaitingReqList.RemoveAt(j);
                --j;
                --jcnt;
                if (step <= 0)
                    break;
            }

            if (cur.m_WaitingReqList.Count <= 0)
            {
                asyncLoadTaskList.RemoveAt(i);
                --i;
                --icnt;
            }

            if (step <= 0)
                break;            
        }
#if ENABLE_PROFILER
        }
#endif
    }


    List<AssetInstRequest> m_ReqCompleteList = new List<AssetInstRequest>();
    protected void _Update()
    {
        //++m_QureyCnt;
        //if (m_QureyCnt >= QUREY_STEP)
        //    m_QureyCnt = 0;
        //else
        //    return;
        
        /// for (int i = 0,icnt = m_AsyncLoadTaskList.Count;i<icnt;++i)
        /// {
        ///     AsyncLoadTaskDesc cur = m_AsyncLoadTaskList[i];
        ///     if(null == cur)
        ///     {
        ///         m_AsyncLoadTaskList.RemoveAt(i);
        ///         break;
        ///     }
        /// 
        ///     if (!cur.m_AssetDesc.CheckAsyncLoadComplete())
        ///         continue;
        /// 
        ///     AssetDesc dummy = null;
        ///     if (!_GetCachedAssetDesc(cur.m_ResPath, cur.m_ResType,out dummy))
        ///         _AddCachedAssetDesc(cur.m_ResPath, cur.m_ResType, cur.m_AssetDesc);
        /// 
        ///     int step = m_AsyncStep;
        ///     for (int j = 0,jcnt= cur.m_WaitingReqList.Count;j<jcnt;++j)
        ///     {
        ///         AssetInstRequest curReq = cur.m_WaitingReqList[j];
        ///         if(null != curReq)
        ///         {
        ///             if(!curReq.m_IsAbort)
        ///             {
        ///                 curReq.m_AssetInst = cur.m_AssetDesc.CreateRefInst(curReq.m_flag);
        ///                 curReq.m_IsDone = true;
        ///                 m_ReqCompleteList.Add(curReq);
        ///                 --step;
        ///             }
        ///         }
        /// 
        ///         cur.m_WaitingReqList.RemoveAt(j);
        ///         --j;
        ///         --jcnt;
        ///         if (step <= 0)
        ///             break;
        ///     }
        /// 
        ///     if(cur.m_WaitingReqList.Count <= 0)
        ///         m_AsyncLoadTaskList.RemoveAt(i);
        /// 
        ///     break;
        /// }

        int Step = m_AsyncStep;
        _CheckAsyncLoadTaskList(m_HighPriorityAsyncLoadTaskList,ref Step);
        if (Step > 0)
        {
            Step = 1;
            _CheckAsyncLoadTaskList(m_AsyncLoadTaskList, ref Step);
        }

        for (int i = 0,icnt = m_CompletedReqList.Count;i<icnt;++i)
        {
            AssetInstRequest curReq = m_CompletedReqList[i];
            if(null != curReq)
            {
                if(curReq.m_IsAbort)
                {
                    if(null != curReq.m_AssetInst && curReq.m_AssetInst.isGameObject)
                    {
                        //GameObject go = (curReq.m_AssetInst.obj as GameObject);
                        //if(null != go)
                        //    go.SetActive(true);

                        //#if UNITY_EDITOR
                        //                        Logger.LogError("AssetInstReq Destory:" + curReq.m_AssetInst.obj.name +  " " + curReq.m_waterMark.ToString("x"));
                        //#endif

                        GameObject.Destroy(curReq.m_AssetInst.obj);
                    }
                    curReq.Reset();
                    m_AssetInstReqPool.Add(curReq);
                    m_CompletedReqList.RemoveAt(i);
                    --i;
                    --icnt;
                    continue;
                }
                else
                {
                    if (!curReq.m_HasExtract)
                        continue;
                }
                
                curReq.Reset();
                m_AssetInstReqPool.Add(curReq);
            }

            m_CompletedReqList.RemoveAt(i);
            --i;
            --icnt;
        }

        if (null != m_ReqCompleteList)
        {
            m_CompletedReqList.AddRange(m_ReqCompleteList);
            m_ReqCompleteList.RemoveRange(0, m_ReqCompleteList.Count);
        }
    }

    public void Update()
    {
        _Update();
    }

    public void DumpAssetInfo(ref List<string> assetList)
    {
        assetList.Clear();
        Dictionary<string, List<AssetInfo>>.Enumerator enumerator = m_ResDescCacheTableEx.GetEnumerator();

        while (enumerator.MoveNext())
        {
            List<AssetInfo> value = enumerator.Current.Value;
            if (null == value) continue;
            for (int i = 0, icnt = value.Count; i < icnt; ++i)
            {
                AssetInfo curInfo = value[i];
                if (null == curInfo || null == curInfo.m_AssetDesc) continue;

                AssetDesc val = curInfo.m_AssetDesc;
                string info = string.Format("{0} ({1}) Ref:{2}     [Key: {3}]", Path.GetFileNameWithoutExtension(val.m_FullPath), val.assetType.ToString(), val.GetRefCount(), enumerator.Current.Key);
                assetList.Add(info);
            }
        }
    }

#endregion

#region 异步加载句柄管理
    AsyncRequestHandleAllocator<IAssetInstRequest> m_AsyncRequestAllocator = new AsyncRequestHandleAllocator<IAssetInstRequest>(0);
#endregion

#region 辅助功能：资源加载统计
    
    string m_DumpFile = "ResLoadRecord/FileLoadTrace.rec";
    List<string> m_DumpBuf = new List<string>();
    int m_BufLineNum = 10;

    void _DumpToFile()
    {
        if (m_DumpBuf.Count <= 0)
            return;
#if UNITY_EDITOR
        if(!Directory.Exists(Path.Combine(Application.streamingAssetsPath, "ResLoadRecord")))
            Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, "ResLoadRecord"));

        FileStream fs = new FileStream(Path.Combine(Application.streamingAssetsPath, m_DumpFile), FileMode.Append, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();
        sw.BaseStream.Seek(0, SeekOrigin.End);
        for (int i = 0; i < m_DumpBuf.Count; ++i)
            sw.WriteLine(m_DumpBuf[i]);

        sw.Flush();
        sw.Close();
        fs.Close();
#endif

        m_DumpBuf.Clear();
    }

    void _RecordLoadFile(string file)
    {
        if (!Global.Settings.recordResFile)
            return;

        if (m_DumpBuf.Count >= m_BufLineNum)
            _DumpToFile();

        m_DumpBuf.Add(file);
    }

    void _ValidResPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (path.Length != path.TrimEnd(' ').Length)
        {
            Logger.LogErrorFormat("路径有问题：{0}！", path);
        }
    }

#endregion
}
