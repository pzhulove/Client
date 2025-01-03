using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    internal sealed partial class AssetManager : BaseModule, ITMAssetManager
    {
        private readonly List<string> m_ResourceOnlyPath = new List<string>();

        private string m_ReadOnlyPath;
        private string m_ReadWritePath;
        private string m_CurrentVariant;
        private EnumHelper<AssetMode> m_AssetMode = new EnumHelper<AssetMode>(AssetMode.None);

        private AssetLoader m_AssetLoader;
        private ITMReferencePool<Asset> m_AssetPool;
        private float m_AssetPoolAutoPurgeInterval;
        private ITMReferencePool<AssetPackage> m_AssetPackagePool;
        private float m_AssetPackagePoolAutoPurgeInterval;

        private AssetTree m_AssetTree;
        private string m_PackageRootFolder;

        private string m_ApplicationVersion;
        private int m_InternalResourceVersion;
        private string m_RemoteResServerURI;

        private bool m_AssetDescTableIsReady = false;
        private readonly LinearMap<string, AssetDesc> m_AssetDescTable;
        private bool m_AssetPackageDescTableIsReady = false;
        private readonly LinearMap<AssetPackageName, AssetPackageDesc> m_AssetPackageDescTable;

        public AssetManager()
        {
            m_ReadOnlyPath = null;
            m_ReadWritePath = null;
            m_CurrentVariant = null;
            m_AssetMode = new EnumHelper<AssetMode>(AssetMode.None);

            m_ApplicationVersion = null;
            m_InternalResourceVersion = 0;
            m_RemoteResServerURI = null;
            m_PackageRootFolder = "";

            m_AssetPool = null;
            m_AssetPackagePool = null;
            m_AssetLoader = null;

            m_AssetInitCompleteEventHandler = null;
            m_VersionListUpdateSuccessEventHandler = null;
            m_VersionListUpdateFailureEventHandler = null;
            m_AssetCheckCompleteEventHandler = null;
            m_AssetUpdateStartEventHandler = null;
            m_AssetUpdateChangedEventHandler = null;
            m_AssetUpdateFailureEventHandler = null;
            m_AssetUpdateFinishEventHandler = null;

            m_AssetDescTableIsReady = false;
            m_AssetPackageDescTableIsReady = false;
            m_AssetDescTable = new LinearMap<string, AssetDesc>(true);
            m_AssetPackageDescTable = new LinearMap<AssetPackageName, AssetPackageDesc>(true);
        }

        public event Function<string,float> OnLoadAsset
        {
            add
            {
                if(null != m_AssetLoader)
                    m_AssetLoader.OnLoadAsset += value;
            }
            remove
            {
                if (null != m_AssetLoader)
                    m_AssetLoader.OnLoadAsset -= value;
            }
        }

        public event Function<string, float> OnLoadAssetPackage
        {
            add
            {
                if (null != m_AssetLoader)
                    m_AssetLoader.OnLoadAssetPackage += value;
            }
            remove
            {
                if (null != m_AssetLoader)
                    m_AssetLoader.OnLoadAssetPackage -= value;
            }
        }

        public bool IsAssetLoaderReady
        {
            get
            {
                return m_AssetDescTableIsReady && m_AssetPackageDescTableIsReady;
            }
        }
	
        public EnumHelper<AssetMode> AssetRunMode
        {
            get
            {
                return m_AssetMode;
            }
        }

        /// <summary>
        /// 获取资源只读区路径。
        /// </summary>
        public string ReadOnlyPath
        {
            get
            {
                return m_ReadOnlyPath;
            }
        }

        /// <summary>
        /// 获取资源读写区路径。
        /// </summary>
        public string ReadWritePath
        {
            get
            {
                return m_ReadWritePath;
            }
        }

        public string PackageRootFolder
        {
            get
            {
                return m_PackageRootFolder;
            }
        }

        /// <summary>
        /// 获取当前变体。
        /// </summary>
        public string CurrentVariant
        {
            get
            {
                return m_CurrentVariant;
            }
        }

        public string ApplicationVersion
        {
            get { return m_ApplicationVersion; }
        }

        /// <summary>
        /// 获取当前资源内部版本号。
        /// </summary>
        public int InternalResourceVersion
        {
            get
            {
                return m_InternalResourceVersion;
            }
        }
        /// <summary>
        /// 获取已准备完毕资源数量。
        /// </summary>
        public int AssetCount
        {
            get
            {
                return m_AssetDescTable.Count;
            }
        }

        /// <summary>
        /// 获取已准备完毕资源数量。
        /// </summary>
        public int AssetPackageCount
        {
            get
            {
                return m_AssetPackageDescTable.Count;
            }
        }

        /// <summary>
        /// 获取等待更新资源数量。
        /// </summary>
        public int UpdateWaitingCount
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取正在更新资源数量。
        /// </summary>
        public int UpdatingCount
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取加载资源代理总数量。
        /// </summary>
        public int LoadAgentTotalCount
        {
            get { return m_AssetLoader.LoadAgentTotalCount; }
        }

        /// <summary>
        /// 获取基础加载资源代理总数量。
        /// </summary>
        public int LoadAgentBaseCount
        {
            get { return m_AssetLoader.LoadAgentBaseCount; }
        }

        /// <summary>
        /// 获取额外每优先级级加载资源代理数量。
        /// </summary>
        public int LoadAgentExtraCount
        {
            get { return m_AssetLoader.LoadAgentExtraCount; }
        }

        /// <summary>
        /// 获取可用加载资源代理的数量。
        /// </summary>
        public int LoadAgentFreeCount
        {
            get { return m_AssetLoader.FreeAgentCount; }
        }

        /// <summary>
        /// 获取正在加载资源代理的数量。
        /// </summary>
        public int LoadAgentWorkingCount
        {
            get { return m_AssetLoader.WorkingAgentCount; }
        }

        /// <summary>
        /// 获取等待加载资源任务数量。
        /// </summary>
        public int LoadTaskWaitingCount
        {
            get { return m_AssetLoader.WaitingTaskCount; }
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float AssetPoolAutoPurgeInterval
        {
            get
            {
                return m_AssetPoolAutoPurgeInterval;
            }
        }

        /// <summary>
        /// 获取资源对象池过期时间（单位：秒）。
        /// </summary>
        public float AssetExpireTime
        {
            get
            {
                return m_AssetPool.ExpireTime;
            }
        }

        /// <summary>
        /// 获取已加载的资源的数量。
        /// </summary>
        public int AssetLoadedCount
        {

            get
            {
                return m_AssetPool.ObjectCount;
            }
        }

        /// <summary>
        /// 获取可释放的资源的数量。
        /// </summary>
        public int AssetCanReleaseCount
        {
            get
            {
                return m_AssetPool.CanReleasedCount;
            }
        }

        /// <summary>
        /// 获取资源对象池优先级。
        /// </summary>
        public int AssetPoolPriority
        {
            get
            {
                return m_AssetPool.Priority;
            }
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float AssetPackagePoolAutoPurgeInterval
        {
            get
            {
                return m_AssetPackagePoolAutoPurgeInterval;
            }
        }

        /// <summary>
        /// 获取资源包对象池过期时间（单位：秒）。
        /// </summary>
        public float AssetPackageExpireTime
        {
            get
            {
                return m_AssetPackagePool.ExpireTime;
            }
        }

        /// <summary>
        /// 获取已加载的资源包的数量。
        /// </summary>
        public int AssetPackageLoadedCount
        {
            get
            {
                return m_AssetPackagePool.ObjectCount;
            }
        }

        /// <summary>
        /// 获取可释放的资源包的数量。
        /// </summary>
        public int AssetPackageCanReleaseCount
        {
            get
            {
                return m_AssetPackagePool.CanReleasedCount;
            }
        }

        /// <summary>
        /// 获取资源包对象池优先级。
        /// </summary>
        public int AssetPackagePoolPriority
        {
            get
            {
                return m_AssetPackagePool.Priority;
            }
        }

        public bool IsAsyncInLoading
        {
            get { return m_AssetLoader.IsAsyncInLoading; }
        }

        public string RemoteResServerURI
        {
            get { return m_RemoteResServerURI; }
        }

        public ObjectDesc[] AssetPoolObjectInfos
        {
            get { return m_AssetPool.GetAllObjectInfos(); }
        }
        
        public ObjectDesc[] AssetPackagePoolObjectInfos
        {
            get { return m_AssetPackagePool.GetAllObjectInfos(); }
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

        public void CreateAssetLoader(string asyncResLoaderTypeName, string syncResLoaderTypeName,string assetByteLoaderTypeName)
        {
            m_AssetPool = new ReferencePoolManager.ReferencePool<Asset>("Asset pool", true, 60, 30, 256, 300);
            m_AssetPackagePool = new ReferencePoolManager.ReferencePool<AssetPackage>("Asset package pool", true, 120, 25, 128, 300);
            m_AssetLoader = new AssetLoader(this, m_AssetPool, m_AssetPackagePool, asyncResLoaderTypeName, syncResLoaderTypeName,assetByteLoaderTypeName);

            m_AssetTree = new AssetTree(this);
        }

        /// <summary>
        /// 设置资产加载代理器数量。
        /// </summary>
        public void SetAssetLoadAgentCount(int baseCount, int extraCountPerPriority)
        {
            if (null == m_AssetLoader)
            {
                Debugger.LogWarning("Asset loader is not create yet!");
                return;
            }

            m_AssetLoader.SetAssetLoadAgentCount(baseCount, extraCountPerPriority);
        }

        /// <summary>
        /// 设置资源只读区路径。
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径。</param>
        public void SetReadOnlyPath(string readOnlyPath)
        {
            m_ReadOnlyPath = readOnlyPath;
        }

        /// <summary>
        /// 设置资源读写区路径。
        /// </summary>
        /// <param name="readWritePath">资源读写区路径。</param>
        public void SetReadWritePath(string readWritePath)
        {
            m_ReadWritePath = readWritePath;
        }

        public void AddResourceOnlyPath(string resourceOnlyPath)
        {
            if (string.IsNullOrEmpty(resourceOnlyPath))
            {
                Debugger.LogWarning("Resource only path can not be null or empty!");
                return;
            }

            resourceOnlyPath = Utility.Path.Normalize(resourceOnlyPath);
            if ('/' != resourceOnlyPath[resourceOnlyPath.Length - 1])
                resourceOnlyPath += '/';
            for (int i = 0, icnt = m_ResourceOnlyPath.Count; i < icnt; ++i)
            {
                if (resourceOnlyPath == m_ResourceOnlyPath[i])
                    return;
            }

            m_ResourceOnlyPath.Add(resourceOnlyPath);
        }

        /// <summary>
        /// 设置资源资源模式。
        /// </summary>
        /// <param name="assetmode">资源资源模式。</param>
        public void SetAssetMode(uint assetmode)
        {
            m_AssetMode = new EnumHelper<AssetMode>(assetmode);
        }

        /// <summary>
        /// 设置当前变体。
        /// </summary>
        /// <param name="currentVariant">当前变体。</param>
        public void SetCurrentVariant(string currentVariant)
        {
            m_CurrentVariant = currentVariant;
        }

        /// <summary>
        /// 设置资源包跟目录。
        /// </summary>
        /// <param name="packageRoot">资源包根目录（应用工作目录的相对路径）。</param>
        public void SetPackageRootFolder(string packageRoot)
        {
            m_PackageRootFolder = packageRoot;
        }

        /// <summary>
        /// 设置资源对象池优先级。
        /// </summary>
        /// <param name="priority">优先级。</param>
        public void SetAssetPoolPriority(int priority)
        {
            m_AssetPool.SetPriority(priority);
        }

        /// <summary>
        /// 设置资源池自动清洗时间间隔。
        /// </summary>
        /// <param name="intervalInSeconds">间隔时间（单位：秒）。</param>
        public void SetAssetPoolAutoPurgeInterval(float intervalInSeconds)
        {
            m_AssetPoolAutoPurgeInterval = intervalInSeconds;
        }

        /// <summary>
        /// 设置资源对象过期时间。
        /// </summary>
        /// <param name="expireTime">过期时间（单位：秒）。</param>
        public void SetAssetExpireTime(float expireTime)
        {
            m_AssetPool.SetExpireTime(expireTime);
        }

        /// <summary>
        /// 设置资源包对象池优先级。
        /// </summary>
        /// <param name="priority">优先级。</param>
        public void SetAssetPackagePoolPriority(int priority)
        {
            m_AssetPackagePool.SetPriority(priority);
        }

        /// <summary>
        /// 设置资源包池自动清洗时间间隔。
        /// </summary>
        /// <param name="intervalInSeconds">间隔时间（单位：秒）。</param>
        public void SetAssetPackagePoolAutoPurgeInterval(float intervalInSeconds)
        {
            m_AssetPackagePoolAutoPurgeInterval = intervalInSeconds;
        }

        /// <summary>
        /// 设置资源包对象过期时间。
        /// </summary>
        /// <param name="expireTime">过期时间（单位：秒）。</param>
        public void SetAssetPackageExpireTime(float expireTime)
        {
            m_AssetPackagePool.SetExpireTime(expireTime);
        }

        /// <summary>
        /// 查询资源是否存在
        /// </summary>
        /// <param name="assetName">资源路径</param>
        /// <param name="assetType">资源类型</param>
        /// <param name="loadFromPackage">是否从包内加载</param>
        /// <returns>资源是否存在</returns>
        public bool IsAssetExist(string assetName, System.Type assetType, bool loadFromPackage)
        {
            loadFromPackage = m_AssetMode.HasFlag((int)AssetMode.Package);
            return m_AssetLoader.IsAssetExist(assetName, assetType, loadFromPackage);
        }

        /// <summary>
        /// 查询一个资源所在的资源包的名字
        /// </summary>
        /// <param name="assetName">资源路径</param>
        /// <returns>返回资源所在的资源包的名字，如果资源不在资源包内则返回string.Empty</returns>
        public void QurreyAssetPackage(string assetName,List<string> packages)
        {
            m_AssetLoader.QurreyAssetPackage(assetName,packages);
        }

        /// <summary>
        /// 同步预加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>是否加载成功</returns>
        public bool PreLoadAsset(string assetName, System.Type assetType, object userData, uint uFlag = 0u)
        {
            bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
            if ((m_AssetDescTableIsReady && m_AssetPackageDescTableIsReady) || !isLoadFromPackage)
            {
                return m_AssetLoader.PreLoadAsset(assetName, assetType, userData, isLoadFromPackage, uFlag);
            }
            else
                Debugger.LogError("Can not load asset '{0}' since asset desc table is not ready!", assetName);

            return false;
        }
		
        /// <summary>
        /// 同步加载字节资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源内容</returns>
        public byte[] LoadAssetByte(string assetName, object userData, uint uFlag = 0u)
        {
            bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
            return m_AssetLoader.LoadAssetByte(assetName, userData, isLoadFromPackage, uFlag);
        }

        /// <summary>
        /// 同步加载字节资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="fileLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        public int LoadAssetByteSync(string assetName, AssetLoadCallbacks<byte[]> asyncLoadCallback, object userData)
        {
            if (null != asyncLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetByteSync(assetName, asyncLoadCallback, userData, isLoadFromPackage);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

        /// <summary>
        /// 异步加载字节资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="fileLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        public int LoadAssetByteAsync(string assetName, AssetLoadCallbacks<byte[]> asyncLoadCallback, object userData, int priorityLevel = 0)
        {
            if (null != asyncLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetByteAsync(assetName, asyncLoadCallback, userData, isLoadFromPackage, priorityLevel);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

		
        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        public object LoadAsset(string assetName, System.Type assetType, object userData, uint uFlag = 0u)
        {
            bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
            return m_AssetLoader.LoadAsset(assetName, assetType, userData, isLoadFromPackage, uFlag);            
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        public object LoadAsset(string assetName, System.Type assetType, Transform transform, object userData, uint uFlag = 0u)
        {
            bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
            return m_AssetLoader.LoadAsset(assetName, assetType, transform, userData, isLoadFromPackage, uFlag);
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="parent">初始父节点。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        public object LoadAsset(string assetName, System.Type assetType, object parent, object userData, uint uFlag = 0u)
        {
            bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
            return m_AssetLoader.LoadAsset(assetName, assetType, parent, userData, isLoadFromPackage, uFlag);
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="parent">初始父节点。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        public object LoadAsset(string assetName, System.Type assetType,object parent, Transform transform, object userData, uint uFlag = 0u)
        {
            bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
            return m_AssetLoader.LoadAsset(assetName, assetType, parent, transform, userData, isLoadFromPackage, uFlag);
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        public int LoadAssetSync(string assetName, System.Type assetType, AssetLoadCallbacks<object> assetLoadCallback, object userData)
        {
            if (null != assetLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetSync(assetName, assetType, assetLoadCallback, userData, isLoadFromPackage);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        public int LoadAssetSync(string assetName, System.Type assetType, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData)
        {
            if (null != assetLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetSync(assetName, assetType, transform, assetLoadCallback, userData, isLoadFromPackage);            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="parent">初始父节点。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        public int LoadAssetSync(string assetName, System.Type assetType, object parent, AssetLoadCallbacks<object> assetLoadCallback, object userData)
        {
            if (null != assetLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetSync(assetName, assetType, parent, assetLoadCallback, userData, isLoadFromPackage);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="parent">初始父节点。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        public int LoadAssetSync(string assetName, System.Type assetType, object parent, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData)
        {
            if (null != assetLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetSync(assetName, assetType, parent, transform, assetLoadCallback, userData, isLoadFromPackage);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

        /// <summary>
        /// 异步步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="assetLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        public int LoadAssetAsync(string assetName, System.Type assetType, AssetLoadCallbacks<object> assetLoadCallback, object userData, int priorityLevel = 0)
        {
            if (null != assetLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetAsync(assetName, assetType, assetLoadCallback, userData, isLoadFromPackage, priorityLevel);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;           
        }
		
		
        /// <summary>
        /// 异步步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="assetLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        public int LoadAssetAsync(string assetName, System.Type assetType, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, int priorityLevel = 0)
        {
            if (null != assetLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetAsync(assetName, assetType, transform, assetLoadCallback, userData, isLoadFromPackage, priorityLevel);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

        /// <summary>
        /// 异步步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="assetLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        public int LoadAssetAsync(string assetName, System.Type assetType, object parent, AssetLoadCallbacks<object> assetLoadCallback, object userData, int priorityLevel = 0)
        {
            if (null != assetLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetAsync(assetName, assetType, parent, assetLoadCallback, userData, isLoadFromPackage, priorityLevel);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

        /// <summary>
        /// 异步步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="assetLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        public int LoadAssetAsync(string assetName, System.Type assetType, object parent ,Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, int priorityLevel = 0)
        {
            if (null != assetLoadCallback)
            {
                bool isLoadFromPackage = _CheckAssetIsLoadFromResource(assetName);
                return m_AssetLoader.LoadAssetAsync(assetName, assetType, parent, transform, assetLoadCallback, userData, isLoadFromPackage, priorityLevel);
            }
            else
                Debugger.LogError("Asset load callback can not be null!");

            return ~0;
        }

        public void BeginClearUnusedAssets(bool releaseAll)
        {
            m_AssetPool.ReleaseUnusedObject(releaseAll);
            m_AssetPackagePool.ReleaseUnusedObject(releaseAll);
        }

        public bool EndClearUnusedAssets()
        {
            return true;
        }

        /// <summary>
        /// 当GC管理器准备清理资源的注册回调
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="args">事件参数</param>
        public void OnUnloadUnusedAsset(object sender, UnloadUnusedAssetEventArgs args)
        {
            BeginClearUnusedAssets(false);
            EndClearUnusedAssets();
        }

        public void OnUnloadAssetStateChanged(bool isPurgingAsset)
        {
            if (null != m_AssetLoader)
                m_AssetLoader.OnUnloadAssetStateChanged(isPurgingAsset);
        }

        public bool BuildAssetTree(object assetTreeData)
        {
            if (null == m_AssetTree)
            {
                Debugger.LogError("Asset tree is null!");
                return false;
            }

            if(null == assetTreeData)
            {
                Debugger.LogError("Asset tree data can not be null!");
                return false;
            }


            ITMAssetTreeData assetTree = assetTreeData as ITMAssetTreeData;
            if (null != assetTree)
            {
                m_AssetTree.BuildAssetTree(assetTree);
                return true;
            }

            DAssetPackageDependency assetPackageData = assetTreeData as DAssetPackageDependency;
            if (null != assetPackageData)
            {
                m_AssetTree.BuildAssetTree(assetPackageData);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (null != m_AssetLoader)
                m_AssetLoader.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理游戏框架模块。
        /// </summary>
        internal override void Shutdown()
        {
            BeginClearUnusedAssets(true);
            EndClearUnusedAssets();

            m_AssetDescTableIsReady = false;
            m_AssetDescTable.Clear();
            m_AssetPackageDescTableIsReady = false;
            m_AssetPackageDescTable.Clear();

            m_AssetLoader.Shutdown();
        }

        private AssetDesc? _GetAssetDescWithAssetName(string assetName)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                Debugger.AssertFailed("Asset name is invalid!");
#endif

            AssetDesc assetDesc = default(AssetDesc);
            if (m_AssetDescTable.TryGetValue(assetName, out assetDesc))
                return assetDesc;

            return null;
        }

        private AssetPackageDesc? _GetAssetPackageDescWithPackageName(AssetPackageName packageName)
        {
            AssetPackageDesc assetPackageDesc = default(AssetPackageDesc);
            if (m_AssetPackageDescTable.TryGetValue(packageName, out assetPackageDesc))
                return assetPackageDesc;

            return null;
        }

        private void _OnAssetTreeBuildComplete()
        {
            m_AssetPackageDescTableIsReady = true;
            m_AssetDescTableIsReady = true;
        }

        private bool _CheckAssetIsLoadFromResource(string assetName)
        {
            if (!m_AssetMode.HasFlag(AssetMode.Package))
                return false;
            else
            {
                List<string> resPathList = m_ResourceOnlyPath;
                for (int i = 0, icnt = resPathList.Count; i < icnt; ++i)
                {
                    if (assetName.StartsWith(resPathList[i]))
                        return false;
                }

                return true;
            }
        }

        private static bool _IsAssetExistWithPath(string assetPath)
        {
            return Utility.File.Exists(assetPath);
        }

        public void LockAsset(string assetName, bool bLock)
        {
            m_AssetPool.Lock(assetName, bLock);
        }
    }
}