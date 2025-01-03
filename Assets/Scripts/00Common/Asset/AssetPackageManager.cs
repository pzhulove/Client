using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class AssetPackageManager : Singleton<AssetPackageManager>
{
    protected List<AssetPackage> m_AssetPackageList = new List<AssetPackage>();
    protected string m_DependencyFileName = "packageinfo.pak";

    protected string m_PackageInfoPath = "Base/Version/PackageInfo.asset";
    protected string m_AssetTreeDataPath = "Base/Version/AssetTreeData.asset";

    /// <summary>
    /// AssetBundle依赖管理
    /// </summary>
    class AssetSearchCompare:IComparer<DAssetPackageMapDesc>
    {
        public int Compare(DAssetPackageMapDesc x, DAssetPackageMapDesc y)
        {
            return x.assetPathKey.CompareTo(y.assetPathKey);
        }
    }
    AssetSearchCompare SEARCH_COMPARE = new AssetSearchCompare();
    DAssetPackageMapDesc DUMMY_MAPDESC = new DAssetPackageMapDesc();

    AssetBundle m_PackageDependencyBundle = null;
    DAssetPackageDependency m_PackageDependencyDesc = null;
    Tenmove.Runtime.Unity.TMUnityAssetTreeData m_AssetTreeData = null;

    protected int m_QureyCnt = 0;
    protected readonly int QUREY_STEP = 2;
    /// <summary>
    ///  内存中的Bundle管理
    /// </summary>
    protected class AssetPackageBundle
    {
        public int m_AccessCnt = 0;
        public float m_FirstAccessTime = 0;
        public AssetPackage m_AssetPackage = null;
    }

    protected Dictionary<string, AssetPackageBundle> m_AssetPackageCache = new Dictionary<string, AssetPackageBundle>();
    protected List<AssetPackageBundle> m_AssetPackageCacheList = new List<AssetPackageBundle>();
    protected long m_TotalSizeInMemory = 0;
    protected long m_MemoryUpperLimit = 5 * 1024 * 1024; /// AssetBundle内存上限50MB

    #region 异步加载管理
    protected class AssetPackageAsyncDesc
    {
        public AssetPackage m_AssetPackage;
        public bool m_AsDependency = true;
        public int m_RefCount = 0;
    }

    protected List<AssetPackageAsyncDesc> m_AsyncLoadPackageDescList = new List<AssetPackageAsyncDesc>();
    protected List<AssetPackageAsyncDesc> m_AsyncIdlePackageDescList = new List<AssetPackageAsyncDesc>();

    #endregion

    public override void Init()
    {
        /// Load version data
        /// 
        _Clear();
        Logger.LogAsset( "Initializing asset package manager.");
#if UNITY_EDITOR
        AddPackageDependency();
#endif
    }

    public override void UnInit()
    {
        _Clear();
    }

    public AssetPackage GetResPackage(string resPath, System.Type type,out string GUIDName)
    {
        GUIDName = null;
        if (null != m_PackageDependencyDesc)
        {
            if(null != m_PackageDependencyDesc.assetDescPackageMap)
            {
                DUMMY_MAPDESC.assetPathKey = resPath.ToLower();
                int idx = m_PackageDependencyDesc.assetDescPackageMap.BinarySearch(DUMMY_MAPDESC, SEARCH_COMPARE);
                if(0 <= idx && idx < m_PackageDependencyDesc.assetDescPackageMap.Count)
                {
                    DAssetPackageMapDesc curDesc = m_PackageDependencyDesc.assetDescPackageMap[idx];
                    if(0<= curDesc.packageDescIdx  && curDesc.packageDescIdx < m_PackageDependencyDesc.packageDescArray.Length)
                    {
                        DAssetPackageDesc curPackageDesc = m_PackageDependencyDesc.packageDescArray[curDesc.packageDescIdx];
                        if(null != curPackageDesc.packageAsset)
                        {
                            GUIDName = curDesc.assetPackageGUID;
                            return curPackageDesc.assetPackage;
                        }
                    }
                }
            }
        }

        return null;
    }


    public bool LoadPackage(AssetPackage resPackage, bool isDependent = false)
    {
        if(null == resPackage)
        {
            Logger.LogAsset( "Package can not be null!");
            return false;
        }

        //if (m_MemoryUpperLimit < m_TotalSizeInMemory && false == isDependent)
        //{/// 超过上限释放长时间未用的AssetBundle
        //    float timeNow = Time.time;
        //
        //    Logger.LogAssetFormat("Asset package memory size is more than {0} MB:{1} MB", ((float)m_MemoryUpperLimit / (1024 * 1024)).ToString(), ((float)m_TotalSizeInMemory / (1024 * 1024)).ToString());
        //    m_AssetPackageCacheList.Sort(
        //        (left, right) =>
        //        {
        //            float leftWeight = left.m_AccessCnt / (timeNow - left.m_FirstAccessTime);
        //            float rightWeight = right.m_AccessCnt / (timeNow - right.m_FirstAccessTime);
        //            if (leftWeight > rightWeight)
        //                return -1;
        //            else if (leftWeight < rightWeight)
        //                return 1;
        //            else
        //                return 0;
        //        });
        //
        //    int removeCnt = 0;
        //    long removeSize = 0;
        //    for (; removeCnt < m_AssetPackageCacheList.Count; ++removeCnt)
        //    {
        //        if (removeSize + m_MemoryUpperLimit > m_TotalSizeInMemory)
        //            break;
        //
        //        AssetPackageBundle curPackageDesc = m_AssetPackageCacheList[removeCnt];
        //        if (curPackageDesc.m_AssetPackage.CanUnload())
        //            removeSize += curPackageDesc.m_AssetPackage.packageBytes;
        //    }
        //
        //    List<AssetPackageBundle> removeList = new List<AssetPackageBundle>();
        //    for (int i = 0; i < removeCnt; ++i)
        //    {
        //        AssetPackageBundle curPackageDesc = m_AssetPackageCacheList[i];
        //
        //        if (curPackageDesc.m_AssetPackage.CanUnload())
        //        {
        //            curPackageDesc.m_AssetPackage.UnloadPackage(false);
        //            m_TotalSizeInMemory -= curPackageDesc.m_AssetPackage.packageBytes;
        //
        //            Logger.LogAssetFormat("Unload asset package \"{0}\"({1}MB)", curPackageDesc.m_AssetPackage.packageFullPath, ((float)curPackageDesc.m_AssetPackage.packageBytes / (1024 * 1024)).ToString());
        //            m_AssetPackageCache.Remove(curPackageDesc.m_AssetPackage.packageFullPath);
        //
        //            removeList.Add(curPackageDesc);
        //        }
        //    }
        //
        //    for (int i = 0; i < removeList.Count; ++i)
        //        m_AssetPackageCacheList.Remove(removeList[i]);
        //}

        AssetPackageBundle packageDesc = null;
        if (m_AssetPackageCache.TryGetValue(resPackage.packageFullPath, out packageDesc))
        {
            Logger.LogAssetFormat("Reloading package \"{0}\"!", resPackage.packageFullPath);
            packageDesc.m_AssetPackage.ReloadPackage( isDependent);
            ++packageDesc.m_AccessCnt;
            return true;
        }

        Logger.LogAssetFormat("Loading package \"{0}\"!", resPackage.packageFullPath);
        if (resPackage.ReloadPackage( isDependent))
        {
            AssetPackageBundle newPackageDesc = new AssetPackageBundle();

            newPackageDesc.m_FirstAccessTime = Time.time;
            newPackageDesc.m_AccessCnt = 1;
            newPackageDesc.m_AssetPackage = resPackage;

            m_TotalSizeInMemory += resPackage.packageBytes;

            m_AssetPackageCache.Add(resPackage.packageFullPath, newPackageDesc);

            if (m_AssetPackageCacheList.Contains(newPackageDesc))
                Logger.LogErrorFormat("Package \"{0}\" has already in manager cache, there is bug in program!");
            m_AssetPackageCacheList.Add(newPackageDesc);

            Logger.LogAssetFormat("Load asset package \"{0}\"({1}MB)", resPackage.packageFullPath, ((float)resPackage.packageBytes / (1024 * 1024)).ToString());
            return true;
        }

        return false;
    }

    public bool LoadPackageAsync(AssetPackage resPackage,bool highPriority, bool isDependent = false)
    {
        if (null == resPackage)
        {
            Logger.LogAsset("Package can not be null!");
            return false;
        }

        resPackage.ReloadPackageAsync(isDependent, highPriority);

        for (int i = 0, icnt = m_AsyncLoadPackageDescList.Count; i < icnt; ++i)
        {
            AssetPackageAsyncDesc curAsyncDesc = m_AsyncLoadPackageDescList[i];
            if (curAsyncDesc.m_AssetPackage.packageNameHashCode == resPackage.packageNameHashCode && curAsyncDesc.m_AssetPackage.packageName == resPackage.packageName)
            {
                ++curAsyncDesc.m_RefCount;
                curAsyncDesc.m_AsDependency &= isDependent;
                return true;
            }
        }

        AssetPackageAsyncDesc asyncDesc = _AllocPackageAsyncDesc();
        if (null != asyncDesc)
        {
            asyncDesc.m_AsDependency = isDependent;
            asyncDesc.m_AssetPackage = resPackage;
            asyncDesc.m_RefCount = 1;
            m_AsyncLoadPackageDescList.Add(asyncDesc);
        }

        return true;
    }

    public void UpdateAsync()
    {
        ++m_QureyCnt;
        if (m_QureyCnt >= QUREY_STEP)
            m_QureyCnt = 0;
        else
            return;

        for (int i = 0,icnt = m_AsyncLoadPackageDescList.Count;i<icnt;++i)
        {
            AssetPackageAsyncDesc asyncDesc = m_AsyncLoadPackageDescList[i];
            if(null == asyncDesc)
            {
                m_AsyncLoadPackageDescList.RemoveAt(i);
                break;
            }

            asyncDesc.m_AssetPackage.UpdateAsyncPackageLoad();

            /// AssetPackage 加载还未完成
            if (!asyncDesc.m_AssetPackage.IsPackageLoadFinish(asyncDesc.m_AsDependency))
                continue;

            /// AssetPackage 加载完成
            AssetPackageBundle newPackageDesc = new AssetPackageBundle();
            newPackageDesc.m_FirstAccessTime = Time.time;
            newPackageDesc.m_AccessCnt = 1;
            newPackageDesc.m_AssetPackage = asyncDesc.m_AssetPackage;

            m_TotalSizeInMemory += asyncDesc.m_AssetPackage.packageBytes;

            if(!m_AssetPackageCache.ContainsKey(asyncDesc.m_AssetPackage.packageFullPath))
                m_AssetPackageCache.Add(asyncDesc.m_AssetPackage.packageFullPath, newPackageDesc);

            bool alreadyExist = false ;
            for(int j = 0,jcnt = m_AssetPackageCacheList.Count;j<jcnt;++j)
            {
                AssetPackageBundle curDesc = m_AssetPackageCacheList[j];
                if(curDesc.m_AssetPackage.packageNameHashCode == asyncDesc.m_AssetPackage.packageNameHashCode &&
                    curDesc.m_AssetPackage.packageName == asyncDesc.m_AssetPackage.packageName)
                {
                    alreadyExist = true;
                    break;
                }
            }

            if(!alreadyExist)
                m_AssetPackageCacheList.Add(newPackageDesc);

            -- asyncDesc.m_RefCount;
            if (asyncDesc.m_RefCount <= 0)
            {
                asyncDesc.m_AsDependency = true;
                asyncDesc.m_AssetPackage = null;
                asyncDesc.m_RefCount = 0;

                m_AsyncLoadPackageDescList.RemoveAt(i);
                m_AsyncIdlePackageDescList.Add(asyncDesc);
            }

            break;
        }

        /// for(int i = 0,icnt = m_AssetPackageCacheList.Count;i<icnt;++i)
        /// {
        ///     AssetPackageBundle curPackageDesc = m_AssetPackageCacheList[i];
        ///     if(null == curPackageDesc || null == curPackageDesc.m_AssetPackage) continue;
        /// 
        ///     curPackageDesc.m_AssetPackage.UpdateAsyncAssetLoad();
        /// }

        //AssetAsyncTaskAllocator<AssetPackageResRequest>.instance.Update();
    }

    public void UnloadPackage(AssetPackage package, bool bFroceUnload = false)
    {
        bool bUnload = package.CanUnload();
        if (!bUnload && bFroceUnload)
            Logger.LogWarningFormat("Package [{0}] has been force unload while it has't ready to be unload yet!", package.packageFullPath);

        if (bUnload)
        {
            m_AssetPackageCacheList.RemoveAll(
                p =>
                {
                    if (p.m_AssetPackage == package)
                    {
                        if(m_AssetPackageCache.Remove(package.packageFullPath))
                        {
                            p.m_AssetPackage.UnloadPackage(false);

                            m_TotalSizeInMemory -= p.m_AssetPackage.packageBytes;
                            Logger.LogAssetFormat( "Unload asset package \"{0}\"({1}MB)", package.packageFullPath, ((float)p.m_AssetPackage.packageBytes / (1024 * 1024)).ToString());
                            return true;
                        }
                    }

                    return false;
                });
        }
    }

    public void UnloadUnusedPackage(bool bForceClear = false)
    {
        _ClearAllPackage(bForceClear);

        //GC.Collect();
    }

    public void AddPackageDependency()
    {
        if (!Global.Settings.loadFromPackage)
            return;

        EngineConfig.instance.LoadConfig();

        string dependency = null;
        if (EngineConfig.useTMEngine)
            dependency = m_AssetTreeDataPath;
        else
            dependency = m_PackageInfoPath;
        dependency = PathUtil.EraseExtension(dependency);

        _Clear();
        Logger.LogAsset( "Load dependent file with path:" + _GetPlatformUrlHead() + "/" + m_DependencyFileName);

        float timePos = Time.realtimeSinceStartup * 1000.0f;

        string hotfixDependPath = Path.Combine(Path.Combine(Application.persistentDataPath, "AssetBundles"), m_DependencyFileName);


        if(EngineConfig.useTMEngine)
        {
            /// 先从persistentDataPath下加载AssetBundle依赖文件
            Tenmove.Runtime.Unity.TMUnityAssetTreeData assetTree = null;

            m_PackageDependencyBundle = AssetBundle.LoadFromFile(hotfixDependPath);
            if (null != m_PackageDependencyBundle)
            {
                //Logger.LogErrorFormat("Load hotfix caches dependent file!");
                assetTree = m_PackageDependencyBundle.LoadAsset(Path.GetFileNameWithoutExtension(dependency)) as Tenmove.Runtime.Unity.TMUnityAssetTreeData;

                //if(packageDedpend.patchVersion < VersionManager.instance.clientShortVersion)
                if (VersionManager.instance.m_IsLocalNewer)
                {
                    Logger.LogErrorFormat("Fully package update remove hotfix caches!");
                    assetTree = null;
                    m_PackageDependencyBundle.Unload(true);
                    m_PackageDependencyBundle = null;

                    _ClearHotFixBundle();
                }
            }

            if (null == assetTree)
            {
                /// 如果找不到则从Resource加载最初的依赖文件
                //Logger.LogErrorFormat("Load local dependent file!");
                string streamDependPath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "AssetBundles"), m_DependencyFileName);
                m_PackageDependencyBundle = AssetBundle.LoadFromFile(streamDependPath);
                if (null != m_PackageDependencyBundle)
                    assetTree = m_PackageDependencyBundle.LoadAsset(Path.GetFileNameWithoutExtension("AssetTreeData.asset")) as Tenmove.Runtime.Unity.TMUnityAssetTreeData;
            }
            //else
            //	Logger.LogErrorFormat( "Load dependency file in hot-update resource directory!");

            timePos = Time.realtimeSinceStartup * 1000.0f - timePos;
            //Logger.LogErrorFormat("Load asset dependect file with {0} ms!", timePos);

            if (null != assetTree)
            {
                //Logger.LogErrorFormat( "Load dependency file from base successfully!");

                m_AssetTreeData = assetTree;

                Tenmove.Runtime.ITMAssetManager assetManager = Tenmove.Runtime.ModuleManager.GetModule<Tenmove.Runtime.ITMAssetManager>();
                if (null != assetManager)
                    assetManager.BuildAssetTree(assetTree);
            }
            else
                Logger.LogErrorFormat("Load dependency file has failed!");
        }
        else
        {        /// 先从persistentDataPath下加载AssetBundle依赖文件
            DAssetPackageDependency packageDedpend = null;

            //Logger.LogErrorFormat("Loading asset package dependency file from path \"{0}\"!", hotfixDependPath);

            m_PackageDependencyBundle = AssetBundle.LoadFromFile(hotfixDependPath);
            if (null != m_PackageDependencyBundle)
            {
                //Logger.LogErrorFormat("Load hotfix caches dependent file!");
                packageDedpend = m_PackageDependencyBundle.LoadAsset(Path.GetFileNameWithoutExtension(dependency)) as DAssetPackageDependency;

                //if(packageDedpend.patchVersion < VersionManager.instance.clientShortVersion)
                if (VersionManager.instance.m_IsLocalNewer)
                {
                    Logger.LogErrorFormat("Fully package update remove hotfix caches!");
                    packageDedpend = null;
                    m_PackageDependencyBundle.Unload(true);
                    m_PackageDependencyBundle = null;

                    _ClearHotFixBundle();
                }
            }

            if (null == packageDedpend)
            {
                /// 如果找不到则从Resource加载最初的依赖文件
                //Logger.LogErrorFormat("Load local dependent file!");
                string streamDependPath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "AssetBundles"), m_DependencyFileName);
                m_PackageDependencyBundle = AssetBundle.LoadFromFile(streamDependPath);
                if (null != m_PackageDependencyBundle)
                    packageDedpend = m_PackageDependencyBundle.LoadAsset(Path.GetFileNameWithoutExtension(dependency)) as DAssetPackageDependency;
            }
            //else
            //	Logger.LogErrorFormat( "Load dependency file in hot-update resource directory!");

            timePos = Time.realtimeSinceStartup * 1000.0f - timePos;
            //Logger.LogErrorFormat("Load asset dependect file with {0} ms!", timePos);

            if (null != packageDedpend)
            {
                //Logger.LogErrorFormat( "Load dependency file from base successfully!");

                m_PackageDependencyDesc = packageDedpend;
                _BuildPackageDependency(m_PackageDependencyDesc);
            }
            else
                Logger.LogErrorFormat("Load dependency file has failed!");
        }
    }

    protected void _Clear()
    {
        DOTween.Clear();
        AssetLoader.instance.ClearAll(true);
        _ClearAllPackage(true);
        m_AssetPackageCacheList.Clear();
        m_AssetPackageCache.Clear();
        Resources.UnloadUnusedAssets();

        if (null != m_PackageDependencyBundle)
        {
            m_PackageDependencyBundle.Unload(true);
            m_PackageDependencyBundle = null;
            m_PackageDependencyDesc = null;
            m_AssetTreeData = null;
        }
       
    }

    protected void _BuildPackageDependency(DAssetPackageDependency dependecy)
    {
        float lastTimeMS = Time.realtimeSinceStartup * 1000.0f;
        for (int i = 0, icnt = dependecy.packageDescArray.Length; i < icnt; ++i)
            dependecy.packageDescArray[i].assetPackage = new AssetPackage();

        Logger.LogAsset("Initialize asset packages!");
        List<AssetPackage> assetPackageList = new List<AssetPackage>();
        for (int i = 0, icnt = dependecy.packageDescArray.Length; i < icnt; ++i)
        {
            assetPackageList.Clear();
            DAssetPackageDesc curAssetPackageDesc = dependecy.packageDescArray[i];
            for (int j = 0; j < curAssetPackageDesc.packageAutoDependIdx.Length; ++j)
            {
                int idx = curAssetPackageDesc.packageAutoDependIdx[j];
                if(0 <= idx && idx < icnt)
                    assetPackageList.Add(dependecy.packageDescArray[idx].assetPackage);
            }
            curAssetPackageDesc.assetPackage.Init(curAssetPackageDesc.packagePath, curAssetPackageDesc.packageName, assetPackageList.ToArray(), curAssetPackageDesc.packageAsset, curAssetPackageDesc.packageFlag);
        }

        lastTimeMS = Time.realtimeSinceStartup * 1000.0f - lastTimeMS;
        Logger.LogProfileFormat("Init package dependency with {0} (ms)!", lastTimeMS);
    }

    protected void _ClearHotFixBundle()
    {
        //string platformUrlHead = _GetPlatformUrlHead();

        Debug.LogWarning("Clean hot-fix bundles!");
        string dstDictory = Path.Combine(Application.persistentDataPath, "AssetBundles");
        if (Directory.Exists(dstDictory))
            Directory.Delete(dstDictory, true);

        string verFile1 = Path.Combine(Application.persistentDataPath, "version.config");
        if (File.Exists(verFile1))
            File.Delete(verFile1);

        string verFile2 = Path.Combine(Application.persistentDataPath, "version.json");
        if (File.Exists(verFile2))
            File.Delete(verFile2);

#if UNITY_ANDROID
        string verFile3 = Path.Combine(Application.persistentDataPath, "Assembly-CSharp.dll");
        if (File.Exists(verFile3))
            File.Delete(verFile3);
        string verFile4 = Path.Combine(Application.persistentDataPath, "Assembly-CSharp-firstpass.dll");
        if (File.Exists(verFile4))
            File.Delete(verFile4);
#endif

        // string verFile5 = Path.Combine(Application.persistentDataPath, "versionfile.json");
        // if (File.Exists(verFile5))
        //     File.Delete(verFile5);
    }
    protected string _GetPlatformUrlHead()
    {
        Logger.LogAsset("Platform:" + Application.platform.ToString());

        if (Application.platform == RuntimePlatform.Android)
            return "file://" + Application.persistentDataPath;
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return "file://" + Application.persistentDataPath;
        else if (Application.platform == RuntimePlatform.WindowsEditor)
            return "file:///" + Application.persistentDataPath;
        else
            return Application.persistentDataPath;
    }

    public void _ClearAllPackage(bool bForceClear = false,bool bForceUnloadAsset = false)
    {
        m_AssetPackageCacheList.RemoveAll(
            p =>
            {
                bool bUnload = p.m_AssetPackage.CanUnload();
                if (!bUnload && bForceClear)
                    Logger.LogAssetFormat("Package [{0}] has been force unload while it has't ready to be unload yet!", p.m_AssetPackage.packageFullPath);

                if (bUnload || bForceClear)
                {
                    if (null != p.m_AssetPackage)
                    {
                        if (m_AssetPackageCache.Remove(p.m_AssetPackage.packageFullPath))
                        {
                            p.m_AssetPackage.UnloadPackage(bForceUnloadAsset);
                            m_TotalSizeInMemory -= p.m_AssetPackage.packageBytes;
                            Logger.LogAssetFormat( "Unload asset package \"{0}\"({1}MB)", p.m_AssetPackage.packageFullPath, ((float)p.m_AssetPackage.packageBytes / (1024 * 1024)).ToString());
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            });
    }

    protected AssetPackageAsyncDesc _AllocPackageAsyncDesc()
    {
        if(m_AsyncIdlePackageDescList.Count > 0)
        {
            int lastIdx = m_AsyncIdlePackageDescList.Count - 1;
            AssetPackageAsyncDesc assetPackage = m_AsyncIdlePackageDescList[lastIdx];
            m_AsyncIdlePackageDescList.RemoveAt(lastIdx);
            return assetPackage;
        }

        return new AssetPackageAsyncDesc();
    }

    public void DumpAssetPackageInfo(ref List<string> assetList)
    {
        assetList.Clear();
        for (int i = 0,icnt = m_AssetPackageCacheList.Count;i<icnt;++i)
        {
            string content = "";
            AssetPackageBundle curPackageBundle = m_AssetPackageCacheList[i];
            if (null == curPackageBundle || null == curPackageBundle.m_AssetPackage) continue;

            content += curPackageBundle.m_AssetPackage.packageName + "　(" + curPackageBundle.m_AssetPackage.denpendentRefCnt + ")　";
            AssetPackage[] depends = curPackageBundle.m_AssetPackage.dependPackages;
            if(null != depends && depends.Length > 0)
            {
                content += "　Depends　[　";
                for (int j = 0,jcnt = depends.Length;j<jcnt;++j)
                {
                    AssetPackage curDepend = depends[j];
                    if (null == curDepend) continue;

                    content += curDepend.packageName + "　|　";
                }

                content += "　]　";
            }

            int[] assetHash = curPackageBundle.m_AssetPackage.loadAssetHashes;
            if (null != assetHash && assetHash.Length > 0)
            {
                content += "Asset　[　";
                for (int j = 0, jcnt = assetHash.Length; j < jcnt; ++j)
                {
                    content += assetHash[j] + "　|　";
                }

                content += "　]";
            }

            assetList.Add(content);
        }
    }

    public int GetLoadedAssetPackageCount()
    {
        int alivePackageCnt = 0;
        for(int i = 0 ,icnt = m_AssetPackageCacheList.Count;i<icnt;++i)
        {
            AssetPackageBundle curPackage = m_AssetPackageCacheList[i];
            if (null != curPackage && curPackage.m_AssetPackage.packageLoaded)
                ++alivePackageCnt;
        }

        return alivePackageCnt;
    }

    protected void LoadAssetPackageTest()
    {

    }
}
