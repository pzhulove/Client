using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AssetArchive
{
    public string m_FullPath = null;
    public string m_HashName = null;
}

public enum AssetPackageState
{
    Unload,
    Loading,
    Loaded,
}

[System.Serializable]
public class AssetPackage
{
    [SerializeField]
    protected string m_BundlePath;
    [SerializeField]
    protected string m_BundleName;
    [SerializeField]
    protected string m_BundleFullPath;
    [SerializeField]
    protected string m_BundleHotfixPath;

    [SerializeField]
    protected List<AssetPackage> m_DependencyList = new List<AssetPackage>();
    //protected List<AssetArchive> m_AssetArchiveList = new List<AssetArchive>();

    [SerializeField]
    protected uint m_PackageFlag;
    [SerializeField]
    protected int m_PackageSize = 0;
    [SerializeField]
    protected long m_cbBytes = 0;

    [System.NonSerialized]
    protected AssetBundle m_AssetBundle = null;
    [System.NonSerialized]
    protected AssetPackageState m_PackageState;
    [System.NonSerialized]
    protected int m_DenpendentRefCnt = 0;

    protected struct AssetItem
    {
        public int assetInstID;
        public bool isGameObjAsset;
    }

    [System.NonSerialized]
    protected List<AssetItem> m_LoadedAssetList = new List<AssetItem>();
    [System.NonSerialized]
    protected int m_AssetResCount = 0;

    [System.NonSerialized]
    protected bool m_LoadAsDependent = true;/// 必须为true
    [System.NonSerialized]
    protected bool m_DependentLoaded = false;

    [System.NonSerialized]
    protected int m_LoadingCount = 0;
    [System.NonSerialized]
    protected int m_PackageNameHashCode = ~0;

    public AssetBundleRequest OnBeginLoadAssetAsync(string assetNameInPackage, Type assetType, bool subAsset)
    {
        if(null != m_AssetBundle)
        {
            // ///////////////////////////////////////////////////////////////////////
            // for (int i = 0,icnt = m_DependencyList.Count;i<icnt;++i)
            // {
            //     AssetPackage cur = m_DependencyList[i];
            //     if(null == cur)
            //     {
            //         Debug.LogError("######### Dependency is null!");
            //         continue;
            //     }
            // 
            //     if(!cur.IsPackageLoadFinish(true))
            //     {
            //         Debug.LogError("######### Dependency is not ready!");
            //         continue;
            //     }
            // 
            // 
            //     if (!cur.IsPackageLoadFinish(true))
            //     {
            //         Debug.LogError("######### Dependency is not ready!");
            //         continue;
            //     }
            // 
            // }
            // 
            // ///////////////////////////////////////////////////////////////////////

            OnBeginLoadAsset();
            if (subAsset)
                return m_AssetBundle.LoadAssetAsync(assetNameInPackage, assetType);
            else
                return m_AssetBundle.LoadAssetWithSubAssetsAsync(assetNameInPackage, assetType);
        }

        return null;
    }

    public void OnBeginLoadAsset()
    {
        ++m_LoadingCount;
    }

    public void OnFinishLoadAsset()
    {
        --m_LoadingCount;
    }

    public void TryUnloadPackage()
    {
        if (0 == m_LoadingCount)
            UnloadBundle();
    }

    #region Async load
    [System.NonSerialized]
    //protected IAssetPackageRequest m_AssetBundleRequest = null;
    protected IAsyncLoadRequest<AssetBundle> m_AssetBundleRequest = null;

    // protected class AssetAsyncLoadCommand
    // {
    //     public string m_AssetName;
    //     public System.Type m_AssetType;
    //     public string m_SubRes;
    // }
    // 
    // protected List<AssetAsyncLoadCommand> m_AssetAsyncLoadCommandList = new List<AssetAsyncLoadCommand>();
    // protected List<AssetAsyncLoadCommand> m_AssetAsyncIdleCommandList = new List<AssetAsyncLoadCommand>();

    // protected AssetAsyncLoadCommand _AllocAsyncLoadCommand()
    // {
    //     if(m_AssetAsyncIdleCommandList.Count > 0)
    //     {
    //         int lastIdx = m_AssetAsyncIdleCommandList.Count - 1;
    //         AssetAsyncLoadCommand idleCommand = m_AssetAsyncIdleCommandList[lastIdx];
    //         m_AssetAsyncIdleCommandList.RemoveAt(lastIdx);
    //         return idleCommand;
    //     }
    // 
    //     return new AssetAsyncLoadCommand();
    // }

    public void UpdateAsyncPackageLoad()
    {
        if(!m_LoadAsDependent)
        {
            m_DependentLoaded = true;
            for (int i = 0, icnt = m_DependencyList.Count; i < icnt; ++i)
            {
                if (null != m_DependencyList[i])
                {
                    if (this == m_DependencyList[i] || m_BundleName == m_DependencyList[i].m_BundleName)
                        continue;

                    if (!m_DependencyList[i].IsPackageLoadFinish(true))
                    {
                        m_DependentLoaded = false;
                        break;
                    }
                }
            }

        }

        if (null != m_AssetBundleRequest)
        {
            if (m_AssetBundleRequest.IsDone())
            {
                _AssignAssetBundle( m_AssetBundleRequest.Extract());
                m_AssetBundleRequest = null;
                m_PackageState = AssetPackageState.Loaded;
            }
        }

    }

    private void _AssignAssetBundle(AssetBundle assetBundle)
    {
        if (null == assetBundle)
            return;

        if (null == m_AssetBundle)
            m_AssetBundle = assetBundle;
        else
            assetBundle.Unload(false);
    }

    #endregion

    public bool Init(string bundlePath, string bundleName, AssetPackage[] dependency, DPackAssetDesc[] assets, uint packageFlag)
    {
        if (string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(bundlePath))
        {
            Logger.LogAsset( "Package bundle must has a name and path!");
            return false;
        }

        m_BundleName = bundleName.ToLower();
        m_PackageNameHashCode = m_BundleName.GetHashCode();
        m_BundlePath = bundlePath;
        m_PackageFlag = packageFlag;
        m_BundleFullPath = Path.Combine(m_BundlePath, m_BundleName);
        m_BundleHotfixPath = Path.Combine(Path.Combine(m_BundlePath,"hotfix"), m_BundleName);

        if (null != dependency)
        {
            m_DependencyList.AddRange(dependency);
            //for (int i = 0; i < dependency.Length; ++i)
            //{
            //    if (null != dependency[i])
            //        m_DependencyList.Add(dependency[i]);
            //}
        }

        //for (int i = 0; i < assets.Length; ++i)
        //{
        //    AssetArchive newArchive = new AssetArchive();
        //    newArchive.m_FullPath = assets[i].packageAsset;
        //    newArchive.m_HashName = assets[i].packageGUID;
        //    m_AssetArchiveList.Add(newArchive);
        //}

        return true;
    }

    public bool ReloadPackage( bool isDependent = false)
    {
        if (!isDependent && !m_DependentLoaded)
        {
            for (int i = 0; i < m_DependencyList.Count; ++i)
            {
                if (null != m_DependencyList[i])
                {
                    if (this == m_DependencyList[i] || m_BundleName == m_DependencyList[i].m_BundleName)
                        continue;

                    /// 2017-04-07 这种写法有点问题 改成下面的写法
                    //if (AssetPackageManager.instance.LoadPackage(m_DependencyList[i], bAsync, true))
                    //    m_DependencyList[i].AddDependentRef();

                    AssetPackageManager.instance.LoadPackage(m_DependencyList[i], true);
                    m_DependencyList[i].AddDependentRef();
                }
            }

            m_DependentLoaded = true;
        }

        /// 2017-04-07 这种写法有点问题 改成下面的写法
        m_LoadAsDependent = isDependent & m_LoadAsDependent;

        if (null != m_AssetBundle)
        {
            m_PackageState = AssetPackageState.Loaded;
            return true;
        }

        if(null != m_AssetBundleRequest)
        {
            if (m_AssetBundleRequest.IsDone())
            {
                _AssignAssetBundle(m_AssetBundleRequest.Extract());
                m_PackageState = AssetPackageState.Loaded;
                m_AssetBundleRequest = null;
                return true;
            }
            else
            {
                m_AssetBundleRequest.Abort();
                m_AssetBundleRequest = null;
            }
        }

        /// 2017-04-07 这种写法有点问题 改成下面的写法
        //m_LoadAsDependent = isDependent & m_LoadAsDependent;
        m_PackageState = AssetPackageState.Loading;
        /// Debug.LogWarningFormat("Syncload asset bundle '{0}'!", m_BundleName);
        _AssignAssetBundle(AssetPackageLoader.instance.LoadPackage(this));

        if (null != m_AssetBundle)
        {
            m_PackageState = AssetPackageState.Loaded;
            return true;
        }
        else
        {
            m_PackageState = AssetPackageState.Unload;
            return false;
        }
    }

    public bool IsPackageNeedLoad(bool isDependent = false)
    {
        if(!isDependent)
        {
            return !m_DependentLoaded;
        }
        else
        {
            return null != m_AssetBundle;
        }
    }

    public bool IsPackageInLoading()
    {
        return null != m_AssetBundleRequest;
    }

    public bool IsDependecyLoaded()
    {
        if(!m_LoadAsDependent)
        {
            return m_DependentLoaded;
        }

        return true;
    }

    public bool IsPackageLoadFinish(bool asDependent)
    {
        if (!asDependent)
        {
            for (int i = 0, icnt = m_DependencyList.Count; i < icnt; ++i)
            {
                if (null != m_DependencyList[i])
                {
                    if (this == m_DependencyList[i] || m_BundleName == m_DependencyList[i].m_BundleName)
                        continue;

                    if (!m_DependencyList[i].IsPackageLoadFinish(true))
                        return false;
                }
            }
        }

        return null != m_AssetBundle;
    }

    public void ReloadPackageAsync(bool isDependent = false,bool highPriority = false)
    {
        if (!isDependent && !m_DependentLoaded)
        {
            for (int i = 0; i < m_DependencyList.Count; ++i)
            {
                if (null != m_DependencyList[i])
                {
                    if (this == m_DependencyList[i] || m_BundleName == m_DependencyList[i].m_BundleName)
                        continue;

                    AssetPackageManager.instance.LoadPackageAsync(m_DependencyList[i], highPriority, true);
                    m_DependencyList[i].AddDependentRef();
                }
            }
        }

        m_LoadAsDependent = isDependent & m_LoadAsDependent;
        if (null != m_AssetBundle)
        {
            m_PackageState = AssetPackageState.Loaded;
        }
        else
        {
            //m_AssetBundleRequest = AssetPackageLoader.instance.LoadPackageAsync(this);
            if (null == m_AssetBundleRequest)
            {
                /// Debug.LogWarningFormat("Asyncload asset bundle '{0}'!", m_BundleName);
                m_AssetBundleRequest = AssetPackageLoader.instance.LoadPackageAsync(this, highPriority);
                m_PackageState = AssetPackageState.Loading;
            }
            else
            {
                if(m_AssetBundleRequest.IsDone())
                {
                    _AssignAssetBundle(m_AssetBundleRequest.Extract());
                    m_PackageState = AssetPackageState.Loaded;
                    m_AssetBundleRequest = null;
                }
            }
        }
    }

    public void UnloadPackage(bool bUnloadAllAsset = false)
    {
        //AssetPackageLoader.instance.UnloadPackage(this, bFroceUnload);
        Logger.LogAssetFormat( "Unloading package \"{0}\"!", m_BundleName);

        if (!m_LoadAsDependent && m_DependentLoaded)
        {
            for (int i = 0; i < m_DependencyList.Count; ++i)
            {
                if (null != m_DependencyList[i] && this != m_DependencyList[i])
                    m_DependencyList[i].RemoveDependentRef();
            }

            m_DependentLoaded = false;
        }

        UnloadBundle(bUnloadAllAsset);

        m_LoadAsDependent = true;
    }

    public UnityEngine.Object LoadResFromPackage(string resName,System.Type type,bool bAsync = false,string subRes = "")
    {
        float lastTimeMS = Time.realtimeSinceStartup * 1000.0f;
        AssetPackageManager.instance.LoadPackage(this);
        lastTimeMS = Time.realtimeSinceStartup * 1000.0f - lastTimeMS;
        Logger.LogProfileFormat("#### Load dependent bundle for package {0} with {1}!", m_BundleName, lastTimeMS);

        if (null != m_AssetBundle)
        {
            int nameIdx = resName.LastIndexOf('/');
            if (0 <= nameIdx && nameIdx < resName.Length)
                resName = resName.Substring(nameIdx + 1).ToLower();

            UnityEngine.Object assetOut = null;
            if (type == typeof(Sprite))
            {
                OnBeginLoadAsset();
                Sprite[] spriteArray = m_AssetBundle.LoadAssetWithSubAssets<Sprite>(CFileManager.EraseExtension(resName));
                for (int i = 0; i < spriteArray.Length; ++i)
                {
                    if (spriteArray[i].name == subRes)
                        assetOut = spriteArray[i];
                }
                OnFinishLoadAsset();
            }

            if (null == assetOut)
            {
                OnBeginLoadAsset();
                lastTimeMS = Time.realtimeSinceStartup * 1000.0f;
                assetOut = m_AssetBundle.LoadAsset(resName, type);
                lastTimeMS = Time.realtimeSinceStartup * 1000.0f - lastTimeMS;
                OnFinishLoadAsset();
                //UnityEngine.Debug.LogWarningFormat("#### Load Res {0} from package {1} with {2}!", resName, m_BundleName, lastTimeMS);
            }

            if (null != assetOut)
            {
                // if(typeof(GameObject) == type && CanUnload())
                //     UnloadPackage();
                // else
                // {
                //     if (!m_LoadedAssetList.Contains(assetOut.GetInstanceID()))
                //         m_LoadedAssetList.Add(assetOut.GetInstanceID());
                //     else
                //         Logger.LogAssetFormat("Asset \"{0}\" has be loaded multiple times, Maybe there is bug in asset load system!", resName);
                // }

                bool isGameObjAsset = assetOut is GameObject;
                _AddAssetRecord(assetOut.GetInstanceID(), isGameObjAsset, resName);
                if (isGameObjAsset && 0 == m_AssetResCount && m_DenpendentRefCnt <= 0)
                {
                    /// Debug.LogWarningFormat("Unload assetBundle:'{0}'!", m_BundleName);
                    TryUnloadPackage();
                }
            }

            return assetOut;
        }

        return null;
    }

    void _AddAssetRecord(int instID,bool isGameObj,string resName)
    {
        for(int i = 0,icnt = m_LoadedAssetList.Count;i<icnt;++i)
        {
            if(m_LoadedAssetList[i].assetInstID == instID)
            {
                Logger.LogAssetFormat("Asset \"{0}\" has be loaded multiple times, Maybe there is bug in asset load system!", resName);
                return;
            }
        }

        AssetItem newItem = new AssetItem();
        newItem.assetInstID = instID;
        newItem.isGameObjAsset = isGameObj;
        m_LoadedAssetList.Add(newItem);

        if (!isGameObj)
            ++m_AssetResCount;
    }

    void _RemoveAssetRecord(int instID)
    {
        for (int i = 0, icnt = m_LoadedAssetList.Count; i < icnt; ++i)
        {
            if (m_LoadedAssetList[i].assetInstID == instID)
            {
                if (!m_LoadedAssetList[i].isGameObjAsset)
                    --m_AssetResCount;

                m_LoadedAssetList.RemoveAt(i);
                return;
            }
        }
    }

    protected void _OnAsyncExtractCallback(UnityEngine.Object asset,string resName)
    {
        if (null != asset)
        {
            //if (asset is GameObject && CanUnload())
            //    UnloadPackage();
            //else
            //{
            //    if (!m_LoadedAssetList.Contains(asset.GetInstanceID()))
            //    {
            //        m_LoadedAssetList.Add(asset.GetInstanceID());
            //    }
            //    else
            //        Logger.LogAssetFormat("Asset \"{0}\" has be loaded multiple times, Maybe there is bug in asset load system!", resName);
            //}

            bool isGameObjAsset = asset is GameObject;
            _AddAssetRecord(asset.GetInstanceID(), isGameObjAsset, resName);
            if(isGameObjAsset && 0 == m_AssetResCount && m_DenpendentRefCnt <= 0)
            {
                TryUnloadPackage();
                /// Debug.LogWarningFormat("Unload assetBundle:'{0}'!", m_BundleName);
            }
        }
    }

    public IAsyncLoadRequest<UnityEngine.Object> LoadResFromPackageAsync(string resName, System.Type type, string subRes = "",bool highPriority = false)
    {
        if(EngineConfig.asyncPackageLoad)
        {
            if (null == m_AssetBundleRequest)
                AssetPackageManager.instance.LoadPackageAsync(this, highPriority);
            else
            {
                if (m_AssetBundleRequest.IsDone())
                {
                    _AssignAssetBundle(m_AssetBundleRequest.Extract());
                    m_AssetBundleRequest = null;
                    m_PackageState = AssetPackageState.Loaded;
                }
            }

            return AsyncLoadTaskAllocator<AssetBundleResquestWrapper, UnityEngine.Object>.instance.AllocAsyncTask(resName, new AssetBundleResquestData(type, subRes, this, _OnAsyncExtractCallback), highPriority);
        }
        else
        {
            Logger.LogProfileFormat("#### Async load dependent bundle for package {0}!", m_BundleName);
            AssetPackageManager.instance.LoadPackage(this);
            return AsyncLoadTaskAllocator<AssetBundleResquestWrapper, UnityEngine.Object>.instance.AllocAsyncTask(resName, new AssetBundleResquestData(type, subRes, this, _OnAsyncExtractCallback), highPriority);
        }
    }

    public void UnloadAsset(int assetInstID)
    {
        //m_LoadedAssetList.Remove(assetInstID);

        //for (int i = 0; i < m_LoadedAssetList.Count; ++i)
        //    Logger.LogAssetFormat("   Asset {0} in package {1}", m_LoadedAssetList[i],m_BundleName);

        _RemoveAssetRecord(assetInstID);
    }

    public bool CanUnload()
    {
        return 0 == m_LoadedAssetList.Count && 0 >= m_DenpendentRefCnt && 0 >= m_LoadingCount;
    }

    /// <summary>
    /// 外部不要调用
    /// </summary>
    public void UnloadBundle(bool unloadAllLoadedObjects = false)
    {
        if (m_LoadingCount > 0)
            throw new Exception("You can not unload this asset-bundle while asset in loading with this bundle!");

        if (null != m_AssetBundleRequest)
        {
            m_AssetBundleRequest.Abort();
            m_AssetBundleRequest = null;
        }

        if (null != m_AssetBundle)
        {
            /// Debug.LogWarningFormat("Unload asset bundle '{0}'!", m_BundleName);
            AssetBundleRegiester.instance.ReleaseAssetBundle(m_AssetBundle, unloadAllLoadedObjects);
            m_AssetBundle = null;
            m_PackageState = AssetPackageState.Unload;
        }
    }

    public void AddDependentRef()
    {
        ++m_DenpendentRefCnt;
        Logger.LogAssetFormat("AssetBundle [{0}] add reference count to {1}", m_AssetBundle, m_DenpendentRefCnt);
    }

    public void RemoveDependentRef()
    {
        --m_DenpendentRefCnt;
        Logger.LogAssetFormat("AssetBundle [{0}] release reference count to {1}", m_AssetBundle, m_DenpendentRefCnt);
        if (m_DenpendentRefCnt < 0)
            Logger.LogErrorFormat( "AssetBundle [{0}] released count is more than created count", m_BundleName);
    }

    public string packageFullPath
    {
        get
        {
            return m_BundleFullPath;
        }
    }
    public string packageHotfixPath
    {
        get
        {
            return m_BundleHotfixPath;
        }
    }

    public string packageName
    {
        get { return m_BundleName; }
    }

    public int packageNameHashCode
    {
        get { return m_PackageNameHashCode; }
    }

    protected bool _IsResInNative(string resPath)
    {
        return false;
    }

    protected bool _IsResInServer(string resPath)
    {
        return false;
    }
    protected string _GetServerResourcePath()
    {
        return "";
    }

    protected string _GetNativeResourcePath()
    {
        return "";
    }

    public long packageBytes
    {
        get
        {
            return m_cbBytes;
        }
    }

    public bool usingHashName
    {
        get
        {
            return (m_PackageFlag & (uint)DAssetPackageFlag.UsingGUIDName) != 0;
        }
    }

    public AssetPackage[] dependPackages
    {
        get
        {
            return m_DependencyList.ToArray();
        }
    }

    public int[] loadAssetHashes
    {
        get
        {
            int[] hash = new int[m_LoadedAssetList.Count];
            for (int i = 0, icnt = m_LoadedAssetList.Count; i < icnt; ++i)
                hash[i] = m_LoadedAssetList[i].assetInstID;

            return hash;
        }
    }

    public int denpendentRefCnt
    {
        get { return m_DenpendentRefCnt; }
    }

    public bool packageLoaded
    {
        get { return null != m_AssetBundle; }
    }

    public AssetBundle assetBundle
    {
        get { return m_AssetBundle; }
    }
}


