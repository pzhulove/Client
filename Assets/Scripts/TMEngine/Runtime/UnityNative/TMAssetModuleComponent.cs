using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMEgnine.Runtime.Unity;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tenmove/AssetModule")]
    public class TMAssetModuleComponent : TMModuleComponent
    {
        private ITMAssetManager m_AssetManager = null;
        
        [SerializeField]
        private uint m_AssetMode = 0;
        [SerializeField]
        private bool m_UseTempPath = false;

        [SerializeField]
        private float m_UnloadUnusedAssetsInterval = 60f;

        [SerializeField]
        private float m_AssetPoolAutoPurgeInterval = 60f;
        [SerializeField]
        private float m_AssetExpireTime = 30f;
        [SerializeField]
        private int m_AssetPoolPriority = 30;

        [SerializeField]
        private float m_AssetPackagePoolAutoPurgeInterval = 60f;
        [SerializeField]
        private float m_AssetPackageExpireTime = 30f;
        [SerializeField]
        private int m_AssetPackagePoolPriority = 25;

        [SerializeField]
        private int m_UpdateRetryCount = 3;
        [SerializeField]
        private int m_AssetLoadAgentCount = 3;
        [SerializeField]
        private int m_AssetLoadAgentExtraCount = 3; // 每级优先级额外允许使用Agent数量

        [SerializeField]
        private string m_ResAsyncLoaderTypeName = typeof(UnityResAsyncLoader).FullName;
        [SerializeField]
        private string m_ResSyncLoaderTypeName = typeof(UnityResSyncLoader).FullName;
        [SerializeField]
        private string m_AssetByteLoaderTypeName = typeof(UnityAssetByteLoader).FullName;

        [SerializeField]
        private string m_AssetPackageRootFolder = "AssetBundles";
        [SerializeField]
        private string m_ResourceOnlyRootFolder = "Base";

        public string ReadOnlyPath
        {
            get { return null != m_AssetManager ? m_AssetManager.ReadOnlyPath:string.Empty; }
        }

        public string ReadWritePath
        {
            get { return null != m_AssetManager ? m_AssetManager.ReadWritePath : string.Empty; }
        }

        public uint AssetMode
        {
            get { return m_AssetMode; }
        }

        public bool UseTempPath
        {
            get { return m_UseTempPath; }
        }

        public string CurrentVariant
        {
            get { return null != m_AssetManager ? m_AssetManager.CurrentVariant : string.Empty; }
        }

        public float UnloadUnusedAssetsInterval
        {
            get { return m_UnloadUnusedAssetsInterval; }
            set { m_UnloadUnusedAssetsInterval = value; }
        }

        public string ApplicationVersion
        {
            get { return null != m_AssetManager ? m_AssetManager.ApplicationVersion : string.Empty; }
        }

        public int InternalResourceVersion
        {
            get { return null != m_AssetManager ? m_AssetManager.InternalResourceVersion : -1; }
        }

        public int AssetCount
        {
            get { return null != m_AssetManager ? m_AssetManager.AssetCount:0; }
        }
        public int AssetPackageCount
        {
            get { return null != m_AssetManager ? m_AssetManager.AssetPackageCount:0; }
        }

        public string RemoteResServerURL
        {
            get { return null != m_AssetManager ? m_AssetManager.RemoteResServerURI:string.Empty; }
        }

        /// <summary>
        /// 获取等待更新资源数量。
        /// </summary>
        public int UpdateWaitingCount
        {
            get { return null != m_AssetManager ? m_AssetManager.UpdateWaitingCount : -1; }
        }
        /// <summary>
        /// 获取正在更新资源数量。
        /// </summary>
        public int UpdatingCount
        {
            get { return null != m_AssetManager ? m_AssetManager.UpdatingCount : -1; }
        }

        /// <summary>
        /// 总加载资源代理数量。
        /// </summary>
        public int LoadAgentTotalCount
        {
            get { return null != m_AssetManager ? m_AssetManager.LoadAgentTotalCount : -1; }
        }

        /// <summary>
        /// 基础加载资源代理数量（Normal Priority优先级可用的代理数量）。
        /// </summary>
        public int LoadAgentBaseCount
        {
            get { return null != m_AssetManager ? m_AssetManager.LoadAgentBaseCount : -1; }
        }

        /// <summary>
        /// 获取可用加载资源代理数量。
        /// </summary>
        public int LoadAgentFreeCount
        {
            get { return null != m_AssetManager ? m_AssetManager.LoadAgentFreeCount : -1; }
        }

        /// <summary>
        /// 获取工作中加载资源代理数量。
        /// </summary>
        public int LoadAgentWorkingCount
        {
            get { return null != m_AssetManager ? m_AssetManager.LoadAgentWorkingCount : -1; }
        }

        /// <summary>
        /// 获取等待加载资源任务数量。
        /// </summary>
        public int LoadWaitingTaskCount
        {
            get { return null != m_AssetManager ? m_AssetManager.LoadTaskWaitingCount : -1; }
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float AssetPoolAutoPurgeInterval
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetPoolAutoPurgeInterval : -1;
            }

            set
            {
                if (null != m_AssetManager)
                {
                    m_AssetPoolAutoPurgeInterval = value;
                    m_AssetManager.SetAssetPoolAutoPurgeInterval(value);
                }
                else
                    Debug.LogWarning("Asset manager is not initialize yet!");
            }
        }

        /// <summary>
        /// 设置资源对象暂存时间。
        /// </summary>
        public float AssetExpireTime
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetExpireTime : -1;
            }

            set
            {
                if (null != m_AssetManager)
                {
                    m_AssetExpireTime = value;
                    m_AssetManager.SetAssetExpireTime(value);
                }
                else
                    Debug.LogWarning("Asset manager is not initialize yet!");
            }
        }

        /// <summary>
        /// 获取已加载的资源的数量。
        /// </summary>
        public int AssetLoadedCount
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetLoadedCount : -1;
            }
        }

        /// <summary>
        /// 获取可释放的资源的数量。
        /// </summary>
        public int AssetCanReleaseCount
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetCanReleaseCount : -1;
            }
        }

        /// <summary>
        /// 设置资源对象暂存时间。
        /// </summary>
        public int AssetPoolPriority
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetPoolPriority : -1;
            }

            set
            {
                if (null != m_AssetManager)
                {
                    m_AssetPoolPriority = value;
                    m_AssetManager.SetAssetPoolPriority(value);
                }
                else
                    Debug.LogWarning("Asset manager is not initialize yet!");
            }
        }

        /// <summary>
        /// 获取或设置资源包对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float AssetPackagePoolAutoPurgeInterval
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetPackagePoolAutoPurgeInterval : -1;
            }

            set
            {
                if (null != m_AssetManager)
                {
                    m_AssetPackagePoolAutoPurgeInterval = value;
                    m_AssetManager.SetAssetPackagePoolAutoPurgeInterval(value);
                }
                else
                    Debug.LogWarning("Asset manager is not initialize yet!");
            }
        }

        /// <summary>
        /// 设置资源对象暂存时间。
        /// </summary>
        public float AssetPackageExpireTime
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetPackageExpireTime : -1;
            }

            set
            {
                if (null != m_AssetManager)
                {
                    m_AssetPackageExpireTime = value;
                    m_AssetManager.SetAssetPackageExpireTime(value);
                }
                else
                    Debug.LogWarning("Asset manager is not initialize yet!");
            }
        }


        /// <summary>
        /// 获取已加载的资源包的数量。
        /// </summary>
        public int AssetPackageLoadedCount
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetPackageLoadedCount:-1;
            }
        }

        /// <summary>
        /// 获取可释放的资源包的数量。
        /// </summary>
        public int AssetPackageCanReleaseCount
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetPackageCanReleaseCount : -1;
            }
        }

        /// <summary>
        /// 设置资源包对象暂存时间。
        /// </summary>
        public int AssetPackagePoolPriority
        {
            get
            {
                return null != m_AssetManager ? m_AssetManager.AssetPackagePoolPriority:-1;
            }

            set
            {
                if (null != m_AssetManager)
                {
                    m_AssetPackagePoolPriority = value;
                    m_AssetManager.SetAssetPackagePoolPriority(value);
                }
                else
                    Debug.LogWarning("Asset manager is not initialize yet!");
            }
        }

        public ObjectDesc[] AssetPoolObjectInfos
        {
            get { return null != m_AssetManager ? m_AssetManager.AssetPoolObjectInfos:new ObjectDesc[0]; }
        }

        public ObjectDesc[] AssetPackagePoolObjectInfos
        {
            get { return null != m_AssetManager ? m_AssetManager.AssetPackagePoolObjectInfos : new ObjectDesc[0]; }
        }

        protected sealed override void Awake()
        {
            base.Awake();
        }

        public void SetAssetLoadAgentCount(int baseCount, int extraCountPerPriority)
        {
            m_AssetLoadAgentCount = baseCount;
            m_AssetLoadAgentExtraCount = extraCountPerPriority;
            if (null != m_AssetManager)
            {
                m_AssetManager.SetAssetLoadAgentCount(m_AssetLoadAgentCount, m_AssetLoadAgentExtraCount);
            }
            else
                Debugger.LogWarning("AssetManager is not create yet!");
        }

        private void Start()
        {
            m_AssetManager = ModuleManager.GetModule<ITMAssetManager>();
            if(null == m_AssetManager)
            {
                Debugger.LogError("Resource manager is invalid!");
                return;
            }

            m_AssetManager.AssetInitComplete += _OnAssetInitComplete;
            m_AssetManager.VersionListUpdateSuccess += _OnVersionListUpdateSuccess;
            m_AssetManager.VersionListUpdateFailure += _OnVersionListUpdateFailure;
            m_AssetManager.AssetCheckComplete += _OnAssetCheckComplete;
            m_AssetManager.AssetUpdateStart   += _OnAssetUpdateStart;
            m_AssetManager.AssetUpdateChanged +=_OnAssetUpdateChanged;
            m_AssetManager.AssetUpdateSuccess +=_OnAssetUpdateSuccess;
            m_AssetManager.AssetUpdateFailure +=_OnAssetUpdateFailure;
            m_AssetManager.AssetUpdateFinish  +=_OnAssetUpdateFinish ;

            m_AssetManager.SetReadOnlyPath(Application.streamingAssetsPath);
            string readWriteRootPath = null;
            if (m_UseTempPath)
                readWriteRootPath = Application.temporaryCachePath;
            else
                readWriteRootPath = Application.persistentDataPath;
            m_AssetManager.SetReadWritePath(readWriteRootPath);

            EnumHelper<AssetMode> assetMode = new EnumHelper<AssetMode>(m_AssetMode);
            if (Global.Settings.loadFromPackage)
                assetMode.AddFlag(Runtime.AssetMode.Package);
            else
                assetMode.RemoveFlag(Runtime.AssetMode.Package);

            m_AssetManager.SetAssetMode(assetMode);

            m_AssetManager.SetPackageRootFolder(m_AssetPackageRootFolder);
            m_AssetManager.CreateAssetLoader(m_ResAsyncLoaderTypeName, m_ResSyncLoaderTypeName, m_AssetByteLoaderTypeName);

            m_AssetManager.AddResourceOnlyPath(m_ResourceOnlyRootFolder);
            m_AssetManager.SetAssetLoadAgentCount(m_AssetLoadAgentCount, m_AssetLoadAgentExtraCount);

            m_AssetManager.SetAssetExpireTime(m_AssetExpireTime);
            m_AssetManager.SetAssetPoolPriority(m_AssetPoolPriority);
            m_AssetManager.SetAssetPoolAutoPurgeInterval(m_AssetPoolAutoPurgeInterval);
            m_AssetManager.SetAssetPackageExpireTime(m_AssetPackageExpireTime);
            m_AssetManager.SetAssetPackagePoolPriority(m_AssetPackagePoolPriority);
            m_AssetManager.SetAssetPackagePoolAutoPurgeInterval(m_AssetPackagePoolAutoPurgeInterval);
        }

        private void Update()
        { 
            UnityAssetBundleKeeper.Update();
        }

        private void _OnAssetInitComplete(object sender,AssetInitCompleteEventArgs e)
        {

        }

        private void _OnVersionListUpdateSuccess(object sender, VersionListUpdateSuccessEventArgs e)
        {

        }

        private void _OnVersionListUpdateFailure(object sender, VersionListUpdateFailureEventArgs e)
        {

        }

        private void _OnAssetCheckComplete(object sender, AssetCheckCompleteEventArgs e)
        {

        }

        private void _OnAssetUpdateStart  (object sender, AssetUpdateStartEventArgs e)
        {

        }

        private void _OnAssetUpdateChanged(object sender, AssetUpdateChangedEventArgs e)
        {

        }

        private void _OnAssetUpdateSuccess(object sender, AssetUpdateSuccessEventArgs e)
        {

        }

        private void _OnAssetUpdateFailure(object sender, AssetUpdateFailureEventArgs e)
        {

        }

        private void _OnAssetUpdateFinish(object sender, AssetUpdateFinishEventArgs e)
        {

        }

    }
}

