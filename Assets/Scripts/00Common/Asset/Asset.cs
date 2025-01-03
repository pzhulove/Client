using UnityEngine;
using System.Collections.Generic;

using WeakRef = System.WeakReference;

public delegate void AssetDescCreateCallback(AssetDesc assetDesc);


public class AssetDesc
{
    public string m_FullPath = null;
    public int m_FullPathHash = ~0;
    public string m_Extension = null;
	public string m_SubResource = null;
    public GameObject m_GameObject = null;
    public int m_AssetObjInstID = ~0;
    public AssetPackage m_AssetPackage = null;
    public WeakRef m_AssetObjRef = null;
    public IAsyncLoadRequest<UnityEngine.Object> m_AsyncRequest = null;
    System.Type m_AssetType = typeof(void);
    protected bool m_IsInstAsset = false;
    protected List<GameObject> m_ObjectList = new List<GameObject>();
    protected float m_LastUseTime = 0;
    protected bool m_initActive = false;

    public bool Init(string fullPath, System.Type type,string subRes = "")
    {
#if LOG_CREATEASSET
        string log = string.Format("CreateAsset {1} {0}",fullPath,type.Name);
        UnityEngine.Debug.LogError(log);
        //ExceptionManager.GetInstance().RecordLog(log);
#endif

        /// Debug.LogWarningFormat("Sync load with res path:{0}!", fullPath);

        float lastTimeMS = Time.realtimeSinceStartup * 1000.0f;
        UnityEngine.Object assetObj = _CreateAsset(PathUtil.EraseExtension(fullPath), type, subRes);

        /// 对于GameObject直接先实例化资源出来
        // if (assetObj is GameObject)
        // {
        //     assetObj = GameObject.Instantiate(assetObj);
        //     GameObject go = assetObj as GameObject;
        //     if (null != go)
        //     {
        //         go.name = assetObj.name.Replace("(Clone)", null);
        //         m_initActive = go.activeSelf;
        //         go.SetActive(false);
        //     }
        // }

        m_AssetObjRef = new WeakRef(assetObj,false);/// 短弱引用
        lastTimeMS = Time.realtimeSinceStartup * 1000.0f - lastTimeMS;
        Logger.LogProfileFormat("Load asset {0} with {1} (ms)!", fullPath, lastTimeMS);

        if (null != m_AssetObjRef)
        {
            m_AssetType = type;
            m_FullPath = fullPath;
            m_Extension = PathUtil.GetExtension(fullPath);
			m_SubResource = subRes;
            //m_AssetObject = m_AssetObjRef.Target as Object;
            if (null != m_AssetObjRef.Target)
            {
                m_AssetObjInstID = assetObj.GetInstanceID();
                m_IsInstAsset = m_AssetObjRef.Target is GameObject;
                if (m_IsInstAsset)
                {
                    m_GameObject = m_AssetObjRef.Target as GameObject;
                    //if (null == m_GameObject.GetComponent<AssetProxy>())
                    //    m_GameObject.AddComponent<AssetProxy>();
                }
            }

            // _AddAssetDescRegTable(this);
            return true;
        }
        else
            Logger.LogAssetFormat( "Can't load the resource with path {0}!", fullPath);

        return false;
    }

    public void InitAsync(string fullPath, System.Type type, string subRes = "",bool highPriority = false)
    {
        #if LOG_CREATEASSET
        string log = string.Format("CreateAssetAsync {1} {0}",fullPath,type.Name);
        UnityEngine.Debug.LogError(log);
        //ExceptionManager.GetInstance().RecordLog(log);
        #endif
        
 
        m_AssetType = type;
        m_FullPath = fullPath;
        m_Extension = PathUtil.GetExtension(fullPath);
        m_SubResource = subRes;

        m_AsyncRequest = _CreateAssetAsync(fullPath, type, subRes,highPriority);
    }

    public bool CheckAsyncLoadComplete()
    {
        if (null != m_AsyncRequest)
        {
            if (null != m_AssetPackage)
            {
                if(null == m_AssetPackage.assetBundle)
                {
                    if (EngineConfig.asyncPackageLoad)
                    {
                        if (!m_AssetPackage.IsPackageInLoading())
                            AssetPackageManager.instance.LoadPackageAsync(m_AssetPackage, false);
                    }
                    else
                        AssetPackageManager.instance.LoadPackage(m_AssetPackage, false);

                }
            }

            if(m_AsyncRequest.IsDone())
            {
                UnityEngine.Object assetObj = m_AsyncRequest.Extract();

                // /// 对于GameObject直接先实例化资源出来
                // if (assetObj is GameObject)
                // {
                //     assetObj = GameObject.Instantiate(assetObj);
                //     GameObject go = assetObj as GameObject;
                //     if (null != go)
                //     {
                //         go.name = assetObj.name.Replace("(Clone)",null);
                //         m_initActive = go.activeSelf;
                //         go.SetActive(false);
                //     }
                // }

                m_AssetObjRef = new WeakRef(assetObj,false);
                if (null != m_AssetObjRef.Target)
                {
                    m_AssetObjInstID = assetObj.GetInstanceID();
                    m_IsInstAsset = m_AssetObjRef.Target is GameObject;
                    if (m_IsInstAsset)
                    {
                        m_GameObject = m_AssetObjRef.Target as GameObject;
                        //if(null == m_GameObject.GetComponent<AssetProxy>())
                        //    m_GameObject.AddComponent<AssetProxy>();
                    }
                }
                m_AsyncRequest = null;
                return true;
            }

            return false;
        }

        return true;
    }

    protected IAsyncLoadRequest<UnityEngine.Object> _CreateAssetAsync(string assetPath, System.Type type, string subRes = "",bool highPriority = false)
    {
        IAsyncLoadRequest<UnityEngine.Object> res = null;
        if (!Global.Settings.loadFromPackage || assetPath.Contains("Base/"))
        {
            float lastTimeMS = 0;
            if (type == typeof(Sprite))
            {
                if (!string.IsNullOrEmpty(subRes))
                {
                    lastTimeMS = (Time.realtimeSinceStartup * 1000.0f);
                    //res = AssetAsyncLoader.instance.LoadAllAsyncEx<Sprite>(assetPath,type,subRes);
                    res = AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.AllocAsyncTask(assetPath, new ResourceResquestData(type, subRes),highPriority);
                    lastTimeMS = (Time.realtimeSinceStartup * 1000.0f) - lastTimeMS;
                    AssetLoadStatistician.instance.AddAssetProfile(assetPath, lastTimeMS,false,true);

                    return res;
                }
            }

            lastTimeMS = (Time.realtimeSinceStartup * 1000.0f);
            //res = AssetAsyncLoader.instance.LoadResAsyncEx(PathUtil.EraseExtension(assetPath), type,subRes);
            res = AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.AllocAsyncTask(assetPath, new ResourceResquestData(type, subRes), highPriority);
            lastTimeMS = (Time.realtimeSinceStartup * 1000.0f) - lastTimeMS;
            AssetLoadStatistician.instance.AddAssetProfile(assetPath, lastTimeMS, false, true);
            return res;
        }
        else
        {
            string guidName = null;
            m_AssetPackage = AssetPackageManager.instance.GetResPackage(assetPath, type, out guidName);
            if (null != m_AssetPackage)
            {
                string loadName = assetPath;
                if (m_AssetPackage.usingHashName && !string.IsNullOrEmpty(guidName))
                    loadName = guidName;

                float lastTimeMS = (Time.realtimeSinceStartup * 1000.0f);

                /// /// 原方式
                /// Object asset = m_AssetPackage.LoadResFromPackage(loadName, type, false, subRes);
                /// if (null == asset)
                ///     Logger.LogAssetFormat("Load asset [{0}] from package has failed!", assetPath);
                /// 
                /// AssetResourceRequest dummy = null;
                /// //res = AssetAsyncTaskAllocator<AssetResourceRequest>.instance.AllocAsyncTaskWithTarget(asset,assetPath,out dummy);
                /// res = AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.AllocAsyncTaskWithTarget(asset,assetPath);
                /// 
                /// lastTimeMS = (Time.realtimeSinceStartup * 1000.0f) - lastTimeMS;
                /// AssetLoadStatistician.instance.AddAssetProfile(assetPath, lastTimeMS, true, true);
                /// 
                /// return res;
                /// /// 原方式

                /// 新方式 （AssetBundle加载完全异步化）
                return m_AssetPackage.LoadResFromPackageAsync(loadName, type, subRes,highPriority);

                /// Object asset = m_AssetPackage.LoadResFromPackage(loadName, type, false, subRes);
                /// if (null == asset)
                ///     Logger.LogAssetFormat("Load asset [{0}] from package has failed!", assetPath);
                /// 
                /// res = AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.AllocAsyncTaskWithTarget(asset, assetPath);
                /// 
                /// lastTimeMS = (Time.realtimeSinceStartup * 1000.0f) - lastTimeMS;
                /// AssetLoadStatistician.instance.AddAssetProfile(assetPath, lastTimeMS, true, true);
                /// 
                /// return res;
            }
#if UNITY_EDITOR
            else
                Logger.LogErrorFormat("Load asset [{0}] from package has failed(asset is not include in package)!", assetPath);
#endif

            //return AssetAsyncTaskAllocator<AssetResourceRequest>.INVALID_ASYNC_REQUEST;
            return AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.INVALID_LOAD_REQUEST;
        }
    }

    public void Deinit()
    {
        // _RemoveAssetDescReg(this);

        Logger.LogAssetFormat( "Dispose asset \"{0}\"!", m_FullPath);
        if(null == m_AssetObjRef)
            Logger.LogErrorFormat("Bad asset \"{0}\"!", m_FullPath);
        else if (m_AssetObjRef.IsAlive)
            Logger.LogAssetFormat("Asset \"{0}\" object reference is not be release!", m_FullPath);
        _DestroyAsset();
    }

    public AssetInst CreateRefInst(uint flag = 0u)
    {
        AssetInst newInst = null;
        if (Global.Settings.enableAssetInstPool)
        {
            newInst = AssetInstPool.instance.GetAssetInst(this,null, flag);
        }
        else
        {
            newInst = new AssetInst();
            newInst.Init(this, flag);
        }

        m_LastUseTime = Time.time;
        return newInst;
    }

    public void AddRefInst(AssetInst assetInst)
    {
        if (null != assetInst.obj)
            m_ObjectList.Add(assetInst.obj as GameObject);
            
    }

    public void RemoveRefInst(AssetInst assetInst)
    {
        if(null != assetInst.obj)
        {
            bool hasRemoved = false;
            m_ObjectList.RemoveAll(
                f =>
                {
                    if (f == (assetInst.obj as GameObject))
                        hasRemoved = true;
                    return f == (assetInst.obj as GameObject);
                }
                );

            if (!hasRemoved)
                Logger.LogErrorFormat("Asset [{0}] inst is not in list!", assetInst.assetDesc.m_FullPath);//
        }

        //for(int i = 0; i < m_ObjectList.Count; ++ i)
        //{
        //    Logger.LogAssetFormat("->Remain game object:{0}", m_ObjectList[i].obj.name);
        //}
    }

    protected void _CheckDeadObj()
    {
        m_ObjectList.RemoveAll(
            f =>
            {
                return null == f;
            }
            );
    }

    public int GetRefCount()
    {
        if (isInstAsset)
        {
            _CheckDeadObj();
            if (null != m_ObjectList)
                return m_ObjectList.Count;
            else
                return 0;
        }
        else
        {
            //m_AssetObject = null;
            if (null != m_AssetObjRef)
            {
                if (null != m_AssetObjRef.Target as Object)
                {
                    Debug.Assert(m_AssetObjRef.Target is Object);
                    //m_AssetObject = m_AssetObjRef.Target as Object;
                    return 1;
                }
            }
            return 0;
        }
    }

    public bool CanBeRemoved()
    {
        // //return (!m_AssetObjRef.IsAlive) && 0 == GetRefCount();
        // if (isInstAsset)
        //     return 0 == GetRefCount();
        // else
        //     return !m_AssetObjRef.IsAlive;
        return 0 == GetRefCount();
    }

    public float lastUseTime
    {
        get { return m_LastUseTime; }
    }

    public bool isInstAsset
    {
        get { return m_IsInstAsset; }
    }

    public bool initActive
    {
        get { return m_initActive; }
    }

    public System.Type assetType
    {
        get { return m_AssetType; }
    }

    public bool IsDeadAsset
    {
        get { return null == m_AssetObjRef;}
    }

    protected Object _CreateAsset(string assetPath, System.Type type,string subRes = "")
    {
        Object asset = null;
        if (!Global.Settings.loadFromPackage || assetPath.Contains("Base/"))
        {
            float lastTimeMS = (Time.realtimeSinceStartup * 1000.0f);

            if (type == typeof(Sprite))
            {
                if (!string.IsNullOrEmpty(subRes))
                {
                    Sprite[] spriteArray = Resources.LoadAll<Sprite>(CFileManager.EraseExtension(assetPath));
                    for (int i = 0; i < spriteArray.Length; ++i)
                    {
                        if (spriteArray[i].name == subRes)
                            return spriteArray[i];
                    }
                }
            }
            else if (type == typeof(AnimationClip))
            {
                if (!string.IsNullOrEmpty(subRes))
                {
                    AnimationClip[] animClipArray = Resources.LoadAll<AnimationClip>(CFileManager.EraseExtension(assetPath));
                    for (int i = 0; i < animClipArray.Length; ++i)
                    {
                        if (animClipArray[i].name == subRes)
                            return animClipArray[i];
                    }
                }
            }

            asset = Resources.Load(PathUtil.EraseExtension(assetPath), type);

            lastTimeMS = (Time.realtimeSinceStartup * 1000.0f) - lastTimeMS;
            AssetLoadStatistician.instance.AddAssetProfile(assetPath, lastTimeMS, false, false);
        }
        else
        {
            float lastTimeMS = (Time.realtimeSinceStartup * 1000.0f);
            string guidName = null;
            m_AssetPackage = AssetPackageManager.instance.GetResPackage(assetPath, type, out guidName);

            if (null != m_AssetPackage)
            {
                string loadName = assetPath;
                if (m_AssetPackage.usingHashName && !string.IsNullOrEmpty(guidName))
                    loadName = guidName;

                //UnityEngine.Debug.LogWarningFormat("####################BEGIN  AssetPackage assetPath:{0} LoadName:{1} Type:{2} subRes:{3}",assetPath,loadName,type,subRes);
                asset = m_AssetPackage.LoadResFromPackage(loadName, type, false, subRes);
                if (null == asset)
                    Logger.LogAssetFormat("Load asset [{0}] from package has failed!", assetPath);
                //UnityEngine.Debug.LogWarningFormat("Asset is packageFullPath:{0} packageName:{1} assetPath:{2} guidName:{3}",m_AssetPackage.packageFullPath,m_AssetPackage.packageName,assetPath,guidName);    
                //UnityEngine.Debug.LogWarningFormat("####################END  AssetPackage assetPath:{0} LoadName:{1} Type:{2} subRes:{3}",assetPath,loadName,type,subRes);
            }
            else
            {
                //Logger.LogErrorFormat("#####################################Load asset!! [{0}] from package has failed(asset is not include in package)!################################", assetPath);

                //{/// 兼容措施 走正常流程加载资源
                //    if (type == typeof(Sprite))
                //    {
                //        if (!string.IsNullOrEmpty(subRes))
                //        {
                //            Sprite[] spriteArray = Resources.LoadAll<Sprite>(CFileManager.EraseExtension(assetPath));
                //            for (int i = 0; i < spriteArray.Length; ++i)
                //            {
                //                if (spriteArray[i].name == subRes)
                //                    return spriteArray[i];
                //            }
                //        }
                //    }
                //    else if (type == typeof(AnimationClip))
                //    {
                //        if (!string.IsNullOrEmpty(subRes))
                //        {
                //            AnimationClip[] animClipArray = Resources.LoadAll<AnimationClip>(CFileManager.EraseExtension(assetPath));
                //            for (int i = 0; i < animClipArray.Length; ++i)
                //            {
                //                if (animClipArray[i].name == subRes)
                //                    return animClipArray[i];
                //            }
                //        }
                //    }
                //
                //    asset = Resources.Load(PathUtil.EraseExtension(assetPath), type);
                //}
            }

            lastTimeMS = (Time.realtimeSinceStartup * 1000.0f) - lastTimeMS;
            AssetLoadStatistician.instance.AddAssetProfile(assetPath, lastTimeMS, true, false);
        }

        return asset;
    }

    protected void _DestroyAsset()
    {
        if(null != m_AsyncRequest)
        {
            m_AsyncRequest.Abort();
            m_AsyncRequest = null;
        }

        if(null == m_AssetObjRef)
            return;
        if(null != m_AssetPackage)
        {/// AssetBundle卸载
            //m_AssetPackage.UnloadAsset(m_AssetObject);
            // Debug.LogWarningFormat("Destroy asset [{0}] inst ID:{1}",m_FullPath, m_AssetObjInstID);
            m_AssetPackage.UnloadAsset(m_AssetObjInstID);
            m_AssetPackage = null;
        }
        else
        {
            //if (!isInstAsset)
            //    Resources.UnloadAsset(m_AssetObject);
            if (!isInstAsset)
                Resources.UnloadAsset(m_AssetObjRef.Target as Object);
        }

        //m_AssetObject = null;
        m_AssetObjRef = null;
        //if(null != m_GameObject)
        //{
        //    MonoBehaviour.Destroy(m_GameObject.GetComponent<AssetProxy>());
        //    m_GameObject = null;
        //}
        m_GameObject = null;
        m_AssetObjInstID = ~0;
    }

     // static List<AssetDesc> m_RegTable = new List<AssetDesc>();
     // static protected void _RemoveAssetDescReg(AssetDesc asset)
     // {
     //     for (int i = 0, icnt = m_RegTable.Count; i < icnt; ++i)
     //     {
     //         if (m_RegTable[i].m_AssetObjInstID == asset.m_AssetObjInstID)
     //         {
     //             m_RegTable.RemoveAt(i);
     //             return;
     //         }
     //     }
     // 
     //     Debug.LogWarningFormat("Asset [{0}] is not in global reg-table!",asset.m_FullPath);
     // }
     // 
     // static protected void _AddAssetDescRegTable(AssetDesc asset)
     // {
     //     m_RegTable.Add(asset);
     // }
     // 
     // static public void DumpAssetDescReg(ref List<string> dumpList)
     // {
     //     dumpList.Clear();
     //     for (int i = 0, icnt = m_RegTable.Count; i < icnt; ++i)
     //     {
     //         AssetDesc assetDesc = m_RegTable[i];
     //         if(null == assetDesc) continue;
     // 
     //         string info = string.Format("AssetDesc:[{0}] ID:{1} (Ref Count:{2}) \n", assetDesc.m_FullPath, assetDesc.m_AssetObjInstID, assetDesc.GetRefCount());
     //         dumpList.Add(info);
     //     }
     // }
}

public class AssetInst
{
    public AssetInst()
    {
        m_AssetDesc = null;
        m_Object = null;
        m_Removed = false;
        m_InstanceID = ~0;
    }

    public AssetInst(Object obj)
    {
        m_AssetDesc = null;
        m_Object = obj;
        m_Removed = false;
        m_InstanceID = ~0;
    }

    public void Init(AssetDesc ad, Object obj, uint flag = 0u)
    {
        if (null == ad || null == ad.m_AssetObjRef)
        {
            Logger.LogErrorFormat("Bad asset desc! [{0}]", ad.m_FullPath);

            m_AssetDesc = null;
            m_Object = null;

            return;
        }

        if (null == obj)
        {
            Logger.LogErrorFormat("Bad asset object! [{0}]", ad.m_FullPath);

            m_AssetDesc = null;
            m_Object = null;

            return;
        }

        m_Removed = false;
        m_AssetDesc = ad;
        if (null == m_AssetDesc.m_AssetObjRef.Target as Object)
            m_AssetDesc.Init(m_AssetDesc.m_FullPath, m_AssetDesc.assetType, m_AssetDesc.m_SubResource);

        if (m_AssetDesc.isInstAsset)
        {
            m_Object = obj;
            //m_Object.name = string.Format("asset_inst[{0}]", m_Object.name);
            GameObject thisObj = m_Object as GameObject;
            if (1u == flag)
                thisObj.SetActive(false);
            if (null != thisObj)
            {
                AssetProxy assetProxy = thisObj.GetComponent<AssetProxy>();
                if (null != assetProxy)
                {
                    if (assetProxy.Init(this))
                    {
                        m_InstanceID = thisObj.GetInstanceID();
                        m_AssetDesc.AddRefInst(this);
                        return;
                    }
                }
                else
                    Logger.LogError("Get asset proxy component has failed!");
            }
            else
                Logger.LogWarningFormat("Asset object is not a GameObject(prefab) but loaded as a gameobject!");

            m_AssetDesc = null;
            m_Object = null;
            thisObj = null;
        }
        else
        {
            m_AssetDesc = null;
            m_Object = null;
        }
    }

    public void Init(AssetDesc ad,uint flag)
    {

        if (null == ad.m_AssetObjRef)
        {
            Logger.LogErrorFormat("Bad asset desc! [{0}]", ad.m_FullPath);

            m_AssetDesc = null;
            m_Object = null;

            return;
        }

        m_Removed = false;
        m_AssetDesc = ad;

        if (null == m_AssetDesc.m_AssetObjRef.Target as Object)
        {
            //Debug.LogWarningFormat("target object [{0}] is dead!", m_AssetDesc.m_FullPath);
            m_AssetDesc.Init(m_AssetDesc.m_FullPath, m_AssetDesc.assetType, m_AssetDesc.m_SubResource);
        }

        if (m_AssetDesc.isInstAsset)
        {
            //m_Object = GameObject.Instantiate(m_AssetDesc.m_AssetObject);

            // GameObject go = m_AssetDesc.m_AssetObjRef.Target as GameObject;
            // if(null != go)
            // {
            //     if (m_AssetDesc.initActive)
            //         go.SetActive(true);
            // }

            //m_Object = GameObject.Instantiate(m_AssetDesc.m_AssetObjRef.Target as Object);
            //Logger.LogWarningFormat("Instantiate Gamobject! [{0}]", ad.m_FullPath);
            m_Object = GameObject.Instantiate(m_AssetDesc.m_GameObject);

            // if (null != go)
            //     go.SetActive(false);

            //m_Object.name = string.Format("asset_inst[{0}]", m_Object.name);
            //GameObject thisObj = m_Object as GameObject;
            GameObject thisObj = m_Object as GameObject;
            if(1u == flag)
                thisObj.SetActive(false);
            if (null != thisObj)
            {
                AssetProxy assetProxy = thisObj.GetComponent<AssetProxy>();
                if (null == assetProxy)
                    assetProxy = thisObj.AddComponent<AssetProxy>();

                if (null != assetProxy)
                {
                    if (assetProxy.Init(this))
                    {
                        m_InstanceID = m_Object.GetInstanceID();
                        m_AssetDesc.AddRefInst(this);
                    }
                    else
                    {
                        GameObject.Destroy(m_Object);
                        m_Object = null;
                        m_AssetDesc = null;
                        m_InstanceID = ~0;
                    }
                }
                else
                    Logger.LogWarning("Add asset proxy component has failed!");
            }
            else
                Logger.LogWarningFormat("Asset object is not a GameObject(prefab) but loaded as a gameobject!");

            thisObj = null;
        }
        else
        {
            //m_Object = m_AssetDesc.m_AssetObject;
            m_Object = m_AssetDesc.m_AssetObjRef.Target as Object;
        }
    }

    ~AssetInst()
    {
        Logger.LogAssetFormat("Asset instance [{0}] has be disposed!", m_AssetDesc.m_FullPath);

        _Dispose();
        m_Object = null;
    }

    public void Release()
    {
        if (m_Removed)
            return;

        _Dispose();
        m_Object = null;
        m_Removed = true;

        if (Global.Settings.enableAssetInstPool)
            AssetInstPool.instance.RecycleAssetInst(this);

    }

    void _Dispose()
    {
        if (null != m_AssetDesc)
        {
            if (m_AssetDesc.isInstAsset)
                m_AssetDesc.RemoveRefInst(this);

            /// Logger.LogAssetFormat("Release asset instance \"{0}\"(Type:{1})!", m_AssetDesc.m_FullPath, m_AssetDesc.assetType.ToString());

            m_AssetDesc = null;
        }
        else
            Logger.LogAsset("Game object has been released two times!");

        m_InstanceID = ~0;
    }

    public bool isGameObject
    {
        get
        {
            if(null != m_Object)
            {
                return null != (m_Object as GameObject);
            }

            return false;
        }
    }

    public int instanceID
    {
        get { return m_InstanceID; }
    }

    /// public void CopyInstance(GameObject go)
    /// {
    ///     if (Global.Settings.enableAssetInstPool)
    ///     {
    ///         AssetInstPool.instance.GetAssetInst(m_AssetDesc, go);
    ///     }
    ///     else
    ///     {
    ///         AssetInst _this = new AssetInst();
    ///         _this.Init(m_AssetDesc, go);
    ///     }
    /// 
    ///     //AssetInst _this = new AssetInst();
    ///     //_this.Init(m_AssetDesc, go);
    ///     ////AssetInst _this = AssetInstPool.instance.GetAssetInst(m_AssetDesc, go);
    /// }

    public UnityEngine.Object obj
    {
        get { return m_Object; }
    }

    public System.Type assetType
    {
        get { return m_AssetDesc.assetType; }
    }

    public AssetDesc assetDesc
    {
        get { return m_AssetDesc; }
    }

    public void Reset()
    {
        m_AssetDesc = null;
        m_Object = null;
        m_Removed = false;
        m_InstanceID = ~0;
    }

    protected AssetDesc m_AssetDesc = null;
    protected Object m_Object = null;

    protected bool m_Removed = false;
    protected int m_InstanceID = ~0;
}







