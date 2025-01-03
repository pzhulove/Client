using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMEgnine.Runtime.Unity;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    public class UnityResAsyncLoader : TMResAsyncLoader,IDisposable
    {
        private string m_PackageFullPath = null;
        private string m_PackageUpdateUrl = null;
        private string m_AssetNameInPackage = null;
        private string m_AssetSubName = null;
        private string m_AssetFullPath = null;
        private string m_FileFullPath = null;
        
        private AssetBundleRequest m_PackageAssetLoadReq = null;
        private ResourceRequest m_AssetLoadReq = null;
        private WWW m_WWW = null;
        private ResAsyncLoadCallback m_AsyncLoadCallback = null;
        private long m_FileLoadStamp = 0;
        private bool m_ChangedToSyncLoad = false;

        private bool m_Disposed = false;
        private bool m_HasSyncRequested = false;  // 有同步请求强制该异步返回了


        event EventHandler<LoadResourceUpdateEventArgs> m_LoadResourceUpdateEventHandler = null;
        event EventHandler<LoadResourceCompleteEventArgs> m_LoadResourceCompleteEventHandler = null;
        event EventHandler<LoadResourceFailedEventArgs> m_LoadResourceFailedEventHandler = null;
        event EventHandler<LoadPackageCompleteEventArgs> m_LoadPackageCompleteEventHandler = null;

        public UnityResAsyncLoader()
        {
        }

        public override event EventHandler<LoadResourceUpdateEventArgs> UpdateResourceEventHandler
        {
            add
            {
                m_LoadResourceUpdateEventHandler += value;
            }

            remove
            {
                m_LoadResourceUpdateEventHandler -= value;
            }
        }

        public override event EventHandler<LoadResourceCompleteEventArgs> LoadResourceCompleteEventHandler
        {
            add
            {
                m_LoadResourceCompleteEventHandler += value;
            }

            remove
            {
                m_LoadResourceCompleteEventHandler -= value;
            }
        }

        public override event EventHandler<LoadResourceFailedEventArgs> LoadResourceFailedEventHandler
        {
            add
            {
                m_LoadResourceFailedEventHandler += value;
            }

            remove
            {
                m_LoadResourceFailedEventHandler -= value;
            }
        }

        public override event EventHandler<LoadPackageCompleteEventArgs> LoadPackageCompleteEventHandler
        {
            add
            {
                m_LoadPackageCompleteEventHandler += value;
            }

            remove
            {
                m_LoadPackageCompleteEventHandler -= value;
            }
        }

        public sealed override bool ChangedToSyncLoad
        {
            get { return m_ChangedToSyncLoad; }
        }

        /// <summary>
        /// 强制同步完成资源加载
        /// </summary>
        /// <returns>是否加载成功</returns>
        public sealed override bool ForceSyncLoadAsset()
        {
            m_ChangedToSyncLoad = true;

            /// 异步如果刚刚加载完毕 可以进行同步加载。（如果Agent已经发起但是处于等待依赖包加载的阶段，还没有真正向Unity
            /// 请求加载，这时候不能启动同步加载，因为这样的话会导致这个异步加载请求失败，Unity相同的包不能够加载两次,如果
            /// 出现这种情况，说明AssetLoader里面同步模式下加载依赖包和主包的逻辑有BUG）

            if (m_PackageAssetLoadReq != null)
            {/// 从AssetBundle中加载资源的方式
                if (null != m_PackageAssetLoadReq.asset)
                {
                    UnityEngine.Object asset = null;
                    if (!string.IsNullOrEmpty(m_AssetSubName))
                    {
                        UnityEngine.Object[] subresArray = m_PackageAssetLoadReq.allAssets;
                        for (int i = 0, icnt = subresArray.Length; i < icnt; ++i)
                        {
                            if (subresArray[i].name == m_AssetSubName)
                            {
                                asset = subresArray[i];
                                break;
                            }
                        }
                    }

                    if (null == asset)
                        asset = m_PackageAssetLoadReq.asset;

                    GameObject go = asset as GameObject;
                    m_LoadResourceCompleteEventHandler(this, new LoadResourceCompleteEventArgs(
                        new UnityAssetObject(asset), null == go));

                    m_PackageAssetLoadReq = null;
                    m_AssetNameInPackage = null;

                    return true;
                }
                else
                {
                    m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                        AssetLoadErrorCode.NotExist,
                        string.Format("Can not load asset named '{0}' from asset bundle which does not exist in bundle '{1}'!",
                        m_AssetNameInPackage, m_PackageFullPath), m_AssetNameInPackage));

                    return false;
                }
            }

            if (m_AssetLoadReq != null)
            {
                if (null != m_AssetLoadReq.asset)
                {
                    UnityEngine.Object asset = null;
                    /// if (!string.IsNullOrEmpty(m_AssetSubName))
                    /// {
                    /// }
                    if (null == asset)
                        asset = m_AssetLoadReq.asset;

                    GameObject go = m_AssetLoadReq.asset as GameObject;
                    m_LoadResourceCompleteEventHandler(this,
                        new LoadResourceCompleteEventArgs(new UnityAssetObject(
                            m_AssetLoadReq.asset), null == go));
                    m_AssetLoadReq = null;
                    m_AssetFullPath = null;
                    return true;
                }
                else
                {
                    m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                        AssetLoadErrorCode.NotExist,
                        string.Format("Can not load asset named '{0}'!", m_AssetFullPath), m_AssetFullPath));
                    return false;
                }
            }

            return false;
        }

        public sealed override void LoadPackage(string packageFullpath)
        {
            if (null == m_LoadResourceFailedEventHandler ||
                null == m_LoadResourceCompleteEventHandler || 
                null == m_LoadPackageCompleteEventHandler)
            {
                Debugger.AssertFailed("Load asset agent loader handler is invalid!");
                return;
            }

            if (string.IsNullOrEmpty(packageFullpath))
            {
                m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                    AssetLoadErrorCode.PackageError,
                    "Asset bundle path is invalid!",
                    packageFullpath));
                return;
            }

            /// Debugger.LogInfo("Async-loading:'{0}'", packageFullpath);
            m_PackageFullPath = packageFullpath;
            if(!UnityAssetBundleKeeper.LoadAssetBundleAsync(packageFullpath,_OnAssetBundleLoadSuccess,_OnAssetBundleLoadFailure,_OnAssetBundleLoadUpdate))
            {
                m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                    AssetLoadErrorCode.PackageError,
                    "Asset bundle path is invalid!",
                    packageFullpath));
                return;
            }
        }

        public sealed override void LoadAsset(object resource, string assetName, string subResName, Type assetType)
        {
            if (null == m_LoadResourceFailedEventHandler ||
                null == m_LoadResourceCompleteEventHandler)
            {
                Debugger.AssertFailed("Load asset agent loader handler is invalid!");
                return;
            }

            if (null == resource)
                _LoadAssetFromResource(assetName, subResName ,assetType);
            else
                _LoadAssetFromPackage(resource, assetName,subResName, assetType);
        }

        protected void _LoadAssetFromPackage(object package,string assetNameInPackage, string subResName, Type assetType)
        {
            AssetBundle assetBundle = package as AssetBundle;
            if (null == assetBundle)
            {
                m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                    AssetLoadErrorCode.TypeError,
                    "Cant not load asset from loaded resource which is not a unity asset bundle!",
                    assetNameInPackage));
                return;
            }

            if (string.IsNullOrEmpty(assetNameInPackage))
            {
                m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                    AssetLoadErrorCode.InvalidParam,
                    "Can not load asset form asset bundle cause asset name in package is invalid!",
                    assetNameInPackage));
                return;
            }

            /// Debugger.LogInfo("Async-loading:'{0}'", assetNameInPackage);

            m_AssetSubName = subResName;
            m_AssetNameInPackage = assetNameInPackage;
            
            if (string.IsNullOrEmpty(subResName))
                m_PackageAssetLoadReq = assetBundle.LoadAssetAsync(m_AssetNameInPackage, assetType);
            else
                m_PackageAssetLoadReq = assetBundle.LoadAssetWithSubAssetsAsync(m_AssetNameInPackage, assetType);
        }

        protected void _LoadAssetFromResource(string assetFullPath, string subResName, Type assetType)
        {
            if (string.IsNullOrEmpty(assetFullPath))
            {
                m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                    AssetLoadErrorCode.InvalidParam,
                    "Asset path is invalid!",
                    assetFullPath));
                return;
            }

            m_AssetSubName = subResName;
            m_AssetFullPath = assetFullPath;
            m_AssetLoadReq = Resources.LoadAsync(Utility.Path.ChangeExtension( m_AssetFullPath,null), assetType);
        }

        public sealed override void LoadFile(string filepath,bool readWritePath, ResAsyncLoadCallback fileLoadCallback )
        {
            if (null == fileLoadCallback)
            {
                Debugger.AssertFailed("Load file callback is can not be null!");
                return;
            }

            if (string.IsNullOrEmpty(filepath))
            {
                fileLoadCallback(filepath, null, 0, "File path is invalid!");
                return;
            }

            m_AsyncLoadCallback = fileLoadCallback;
            m_FileFullPath = _GetAccessFileURLProtocol(filepath,readWritePath);
            m_WWW = new WWW(m_FileFullPath);
            m_FileLoadStamp = DateTime.Now.Ticks;
        }

        private string _GetAccessFileURLProtocol(string filepath,bool readWritePath)
        {
            if(readWritePath)
            {
#if UNITY_IOS
                return Utility.Path.Combine("file://" + Application.persistentDataPath,filepath);
#elif UNITY_ANDROID
                return Utility.Path.Combine("file://" + Application.persistentDataPath,filepath);
#else
                return Utility.Path.Combine("file:///" + Application.persistentDataPath, filepath);
#endif
            }
            else
            {
#if UNITY_IOS
                return Utility.Path.Combine("file://" + Application.streamingAssetsPath,filepath);
#elif UNITY_ANDROID
                return Utility.Path.Combine("jar:file://" + Application.dataPath + "!/assets/",filepath);
#else
                return Utility.Path.Combine("file:///" + Application.streamingAssetsPath, filepath);
#endif
            }
        }

        public sealed override void UnloadPackage(string packagePath)
        {
            /// AssetBundle assetBundle = package as AssetBundle;
            /// if (null == assetBundle)
            /// {
            ///     Debugger.LogError("Can not unload asset from loaded resource which is not a unity asset bundle!");
            ///     return;
            /// }
            /// 
            /// assetBundle.Unload(true);
            
            UnityAssetBundleKeeper.UnloadAssetBundle(packagePath);
        }

        public sealed override void Update()
        {
            _UpdateWWWLoading();
            _UpdatePackageFetchRequest();
            _UpdatePackageAssetLoadRequest();
            _UpdateResourceLoadRequest();
        }

        public sealed override void Reset()
        {
            m_PackageFullPath = null;
            m_PackageUpdateUrl = null;
            m_AssetNameInPackage = null;
            m_AssetSubName = null;
            m_AssetFullPath = null;
            m_FileFullPath = null;
            
            m_PackageAssetLoadReq = null;
            m_AssetLoadReq = null;
            m_FileLoadStamp = 0;
            m_ChangedToSyncLoad = false;

            if (null != m_WWW)
            {
                m_WWW.Dispose();
                m_WWW = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            if(disposing)
            {
                if(null != m_WWW)
                {
                    m_WWW.Dispose();
                    m_WWW = null;
                }
            }

            m_Disposed = true;
        }

        private void _UpdateWWWLoading()
        {
            if(null != m_WWW)
            {
                if(m_WWW.isDone)
                {
                    if(string.IsNullOrEmpty(m_WWW.error))
                    {
                        m_AsyncLoadCallback(m_FileFullPath, m_WWW.bytes,Utility.Time.TicksToSeconds(DateTime.Now.Ticks - m_FileLoadStamp), null);
                    }
                    else
                    {
                        m_AsyncLoadCallback(m_FileFullPath, null,
                            Utility.Time.TicksToSeconds(DateTime.Now.Ticks - m_FileLoadStamp),
                            string.Format("Can not load file '{0}' with WWW! (internal error:{1})", m_FileFullPath, m_WWW.error));
                    }

                    m_WWW.Dispose();
                    m_WWW = null;
                    m_FileFullPath = null;
                    m_FileLoadStamp = 0;
                    m_AsyncLoadCallback = null;
                }
            }
        }

        private void _UpdatePackageFetchRequest()
        {
        }
        
        private void _UpdatePackageAssetLoadRequest()
        {
            if(null != m_PackageAssetLoadReq)
            {
                if(m_PackageAssetLoadReq.isDone)
                {
                    if(null != m_PackageAssetLoadReq.asset)
                    {
                        UnityEngine.Object asset = null;
                        if(!string.IsNullOrEmpty(m_AssetSubName))
                        {
                            UnityEngine.Object[] subresArray = m_PackageAssetLoadReq.allAssets;
                            for (int i = 0, icnt = subresArray.Length; i < icnt; ++i)
                            {
                                if (subresArray[i].name == m_AssetSubName)
                                {
                                    asset = subresArray[i];
                                    break;
                                }
                            }
                        }

                        if (null == asset)
                            asset = m_PackageAssetLoadReq.asset;                        

                        GameObject go = m_PackageAssetLoadReq.asset as GameObject;
                        m_LoadResourceCompleteEventHandler(this, new LoadResourceCompleteEventArgs(
                            new UnityAssetObject(m_PackageAssetLoadReq.asset), null == go));
                        m_PackageAssetLoadReq = null;
                        m_AssetNameInPackage = null;
                    }
                    else
                    {
                        m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                            AssetLoadErrorCode.NotExist,
                            string.Format("Can not load asset named '{0}' from asset bundle which does not exist in bundle '{1}'!", m_AssetNameInPackage, m_PackageFullPath),
                            m_AssetNameInPackage));
                    }
                }
                else
                {
                    m_LoadResourceUpdateEventHandler(this, new LoadResourceUpdateEventArgs(ResourceLoadMode.LoadAsset, m_PackageAssetLoadReq.progress));
                }
            }
        }

        private void _UpdateResourceLoadRequest()
        {
            if (null != m_AssetLoadReq)
            {
                if (m_AssetLoadReq.isDone)
                {
                    if (null != m_AssetLoadReq.asset)
                    {
                        UnityEngine.Object asset = null;
                        /// if (!string.IsNullOrEmpty(m_AssetSubName))
                        /// {
                        /// }
                        if (null == asset)
                            asset = m_AssetLoadReq.asset;
                        GameObject go = m_AssetLoadReq.asset as GameObject;
                        m_LoadResourceCompleteEventHandler(this, new LoadResourceCompleteEventArgs(new UnityAssetObject(m_AssetLoadReq.asset), null == go));
                        m_AssetLoadReq = null;
                        m_AssetFullPath = null;
                    }
                    else
                    {
                        m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                            AssetLoadErrorCode.NotExist,
                            string.Format("Can not load asset named '{0}'!", m_AssetFullPath),
                            m_AssetFullPath));
                    }
                }
                else
                {
                    m_LoadResourceUpdateEventHandler(this, new LoadResourceUpdateEventArgs(ResourceLoadMode.LoadAsset, m_AssetLoadReq.progress));
                }
            }
        }

        private void _OnAssetBundleLoadSuccess(AssetBundle assetBundle)
        {
            m_LoadPackageCompleteEventHandler(this, new LoadPackageCompleteEventArgs(assetBundle,m_PackageFullPath));
        }

        private void _OnAssetBundleLoadFailure(string path)
        {
            m_LoadResourceFailedEventHandler(this, new LoadResourceFailedEventArgs(
                AssetLoadErrorCode.NotExist,
                string.Format("Can not load asset bundle from file '{0}' which is not a valid asset bundle resource!", path),
                path));
        }

        private void _OnAssetBundleLoadUpdate(float progress)
        {
            m_LoadResourceUpdateEventHandler(this, new LoadResourceUpdateEventArgs(ResourceLoadMode.LoadPackage, progress));
        }
    }
}

