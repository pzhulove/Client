

namespace Tenmove.Runtime.Unity
{
    using System.Collections.Generic;
    using UnityEngine;

    internal partial class UnityGameProfileClient
    {
        private delegate void AssetPackageLoadCallback<TUserData>(string assetPackagePath,TUserData userData);

        private partial class AssetLoader
        {
            private static readonly byte SimulatorAssetRequestHandleType = 0xbe;
            public static readonly uint InvalidAssetReuqestHandle = Utility.Handle.InvalidTypeHandle(SimulatorAssetRequestHandleType);

            private class AssetBundleDesc
            {
                private readonly AssetBundle m_AssetBundle;
                private readonly List<string> m_AssetList;

                public AssetBundleDesc(AssetBundle assetBundle, List<string> assetList)
                {
                    Debugger.Assert(null != assetBundle, "Parameter 'assetBundle' can not be null!");
                    Debugger.Assert(null != assetList, "Parameter 'assetList' can not be null!");

                    m_AssetBundle = assetBundle;
                    m_AssetList = assetList;
                }

                public AssetBundle AssetBundle
                {
                    get { return m_AssetBundle; }
                }

                public List<string> AssetList
                {
                    get { return m_AssetList; }
                }
            }

            private abstract class AsyncLoadTask
            {
                protected readonly AssetLoader m_AssetLoader;
                protected readonly string m_AssetPath;

                public AsyncLoadTask(AssetLoader assetLoader,string assetPath)
                {
                    Debugger.Assert(null != assetLoader, "Parameter 'assetLoader' can not be null!");
                    Debugger.Assert(!string.IsNullOrEmpty(assetPath), "Parameter 'assetPath' can not be null or empty string!");

                    m_AssetLoader = assetLoader;
                    m_AssetPath = assetPath;
                }

                public abstract void Start();
                public abstract bool Update();
            }

            private class AssetAsyncLoadTask : AsyncLoadTask
            {
                private readonly AssetBundle m_AssetBundle;
                private readonly string m_SubAssetName;
                private readonly System.Type m_AssetType;
                private readonly Function<string,UnityEngine.Object,string> m_OnTaskFinished;
                private AssetBundleRequest m_AssetLoadRequest;

                public AssetAsyncLoadTask(AssetLoader assetLoader,AssetBundle assetBundle,string assetNameInPackage,System.Type assetType,string subAssetName, Function<string, UnityEngine.Object,string> onTaskFinished)
                    : base(assetLoader,assetNameInPackage)
                {
                    Debugger.Assert(null != assetBundle, "Parameter 'assetBundle' can not be null!");
                    Debugger.Assert(null != assetType, "Parameter 'assetType' can not be null!");
                    Debugger.Assert(null != onTaskFinished, "Parameter 'onTaskFinished' can not be null!");

                    m_AssetType = assetType;
                    m_SubAssetName = subAssetName;
                    m_OnTaskFinished = onTaskFinished;
                    m_AssetBundle = assetBundle;
                }

                public override void Start()
                {
                    if (string.IsNullOrEmpty(m_SubAssetName))
                        m_AssetLoadRequest = m_AssetBundle.LoadAssetAsync(m_AssetPath, m_AssetType);
                    else
                        m_AssetLoadRequest = m_AssetBundle.LoadAssetWithSubAssetsAsync(m_AssetPath, m_AssetType);
                }

                public override bool Update()
                {
                    if (null != m_AssetLoadRequest)
                    {
                        if (m_AssetLoadRequest.isDone)
                        {
                            if (null != m_AssetLoadRequest.asset)
                            {
                                UnityEngine.Object asset = null;
                                if (!string.IsNullOrEmpty(m_SubAssetName))
                                {
                                    UnityEngine.Object[] subresArray = m_AssetLoadRequest.allAssets;
                                    for (int i = 0, icnt = subresArray.Length; i < icnt; ++i)
                                    {
                                        if (subresArray[i].name == m_SubAssetName)
                                        {
                                            asset = subresArray[i];
                                            break;
                                        }
                                    }
                                }

                                if (null == asset)
                                    asset = m_AssetLoadRequest.asset;

                                GameObject go = m_AssetLoadRequest.asset as GameObject;
                                m_OnTaskFinished(m_AssetPath , m_AssetLoadRequest.asset,string.Empty);
                            }
                            else
                            {
                                m_OnTaskFinished(m_AssetPath, m_AssetLoadRequest.asset, 
                                    string.Format("Can not load asset named '{0}' from asset bundle '{1}'!",
                                    m_AssetPath,m_AssetBundle.name));
                            }

                            return true;
                        }

                        return false;
                    }

                    return true;
                }
            }

            private class AssetBundleAsyncLoadTask : AsyncLoadTask
            {
                private readonly Function<string, AssetBundle,string> m_OnTaskFinished;
                private AssetBundleCreateRequest m_PackageCreateRequest;
                private AssetBundle m_Target;

                public AssetBundleAsyncLoadTask(AssetLoader assetLoader, string assetPackagePath, Function<string, AssetBundle,string> onTaskFinished)
                    : base(assetLoader,assetPackagePath)
                {
                    Debugger.Assert(null != onTaskFinished, "Parameter 'onTaskFinished' can not be null!");
                    m_OnTaskFinished = onTaskFinished;
                    m_PackageCreateRequest = null;
                    m_Target = null;
                }

                public override void Start()
                {
                    AssetBundleDesc desc = null;
                    if (!m_AssetLoader.m_AssetBundleTable.TryGetValue(m_AssetPath, out desc))
                    {
                        if (!m_AssetLoader.m_AssetBundleLoadingTable.Contains(m_AssetPath))
                        {
                            m_AssetLoader.m_AssetBundleLoadingTable.Add(m_AssetPath);
                            m_PackageCreateRequest = AssetBundle.LoadFromFileAsync(m_AssetPath);
                        }
                    }
                    else
                        m_Target = desc.AssetBundle;
                }

                public override bool Update()
                {
                    string lastMessage = string.Empty;

                    if (null != m_PackageCreateRequest)
                    {/// request有值说明是该任务发起的
                        if (m_PackageCreateRequest.isDone)
                        {
                            AssetBundle assetBundle = m_PackageCreateRequest.assetBundle;
                            if (null == assetBundle)
                            {
                                lastMessage = string.Format(
                                    "Can not load asset bundle from file '{0}' which is not a valid asset bundle resource!",
                                    m_AssetPath);
                            }

                            m_Target = assetBundle;

                            if (null != assetBundle)
                            {
                                List<string> allAssets = new List<string>(assetBundle.GetAllAssetNames());
                                AssetBundleDesc assetBundleDesc = new AssetBundleDesc(assetBundle, allAssets);
                                m_AssetLoader.m_AssetBundleList.AddLast(assetBundleDesc);
                                m_AssetLoader.m_AssetBundleTable.Add(m_AssetPath, assetBundleDesc);
                                for (int i = 0, icnt = allAssets.Count; i < icnt; ++i)
                                {
                                    LinkedList<string> packagePathList = null;
                                    if (!m_AssetLoader.m_AssetPackageMapTable.TryGetValue(allAssets[i], out packagePathList))
                                    {
                                        packagePathList = new LinkedList<string>();
                                        m_AssetLoader.m_AssetPackageMapTable.Add(allAssets[i], packagePathList);
                                    }

                                    packagePathList.AddLast(m_AssetPath);
                                }
                            }

                            m_AssetLoader.m_AssetBundleLoadingTable.Remove(m_AssetPath);
                            m_PackageCreateRequest = null;
                        }
                        else
                            return false;
                    }

                    if (null == m_Target)
                    {/// 没有值说明这个任务是等待具有相同资源的其他任务执行完毕或者是这个AssetBundle包已经加载完毕
                        if (m_AssetLoader.m_AssetBundleLoadingTable.Contains(m_AssetPath))
                            return false;

                        AssetBundleDesc desc = null;
                        if (m_AssetLoader.m_AssetBundleTable.TryGetValue(m_AssetPath, out desc))
                            m_Target = desc.AssetBundle;
                        else
                            lastMessage = string.Format("Asset bundle '{0}' loading has failed in other tasks!", m_AssetPath);
                    }

                    m_OnTaskFinished(m_AssetPath, m_Target, lastMessage);
                    return true;
                }
            }

            private interface ITMAssetLoadContext
            {
                void OnAssetTaskFinished(string assetPath, UnityEngine.Object assetObject, string errorMessage);
            }

            private class AssetLoadContext<TAsset,TUserData> : ITMAssetLoadContext where TAsset:class
            {
                protected readonly uint m_RequestID;
                private readonly AssetLoadCallback<TAsset, TUserData> m_Callback;
                private readonly TUserData m_UserData;

                public AssetLoadContext(uint requestID,TUserData userData, AssetLoadCallback<TAsset, TUserData> callback)
                {
                    Debugger.Assert(InvalidAssetReuqestHandle != requestID, "Parameter 'requestID' can not be invalid value!");
                    Debugger.Assert(null != callback, "Parameter 'callback' can not be null!");

                    m_RequestID = requestID;
                    m_Callback = callback;
                    m_UserData = userData;
                }

                public void OnAssetTaskFinished(string assetPath, UnityEngine.Object assetObject, string errorMessage)
                {
                    TAsset target = null;
                    if (assetObject is GameObject)
                        target = Object.Instantiate(assetObject) as TAsset;
                    else
                        target = assetObject as TAsset;

                    m_Callback(m_RequestID, assetPath, target, m_UserData);
                }
            }

            private interface ITMAssetPackageLoadContext
            {
                void OnAssetPackageTaskFinished(string packagePath, AssetBundle assetBundle, string errorMessage);
            }

            private class AssetPackageLoadContext<TUserData> : ITMAssetPackageLoadContext
            {
                private readonly TUserData m_UserData;
                private readonly AssetPackageLoadCallback<TUserData> m_Callback;

                public AssetPackageLoadContext(TUserData userData, AssetPackageLoadCallback< TUserData> callback)
                {
                    Debugger.Assert(null != callback, "Parameter 'callback' can not be null!");

                    m_Callback = callback;
                    m_UserData = userData;
                }

                public void OnAssetPackageTaskFinished(string packagePath, AssetBundle assetBundle, string errorMessage)
                {
                    m_Callback(packagePath,m_UserData);
                }
            }

            private readonly LinkedList<AssetBundleDesc> m_AssetBundleList;
            private readonly Dictionary<string, AssetBundleDesc> m_AssetBundleTable;
            private readonly Dictionary<string, LinkedList<string>> m_AssetPackageMapTable;
            
            private readonly HashSet<string> m_AssetBundleLoadingTable;
            private readonly LinkedList<AsyncLoadTask> m_AssetTaskList;
            
            private uint m_AssetHandleAllocCount;

            public AssetLoader()
            {
                m_AssetBundleList = new LinkedList<AssetBundleDesc>();
                m_AssetBundleTable = new Dictionary<string, AssetBundleDesc>();
                m_AssetBundleLoadingTable = new HashSet<string>();
                m_AssetPackageMapTable = new Dictionary<string, LinkedList<string>>();

                m_AssetTaskList = new LinkedList<AsyncLoadTask>();

                m_AssetHandleAllocCount = 0;
            }

            public void LoadAssetPackage<TUserData>(string assetPackagePath, TUserData userData, AssetPackageLoadCallback<TUserData> callback)
            {
                if (null != callback)
                {
                    if (!string.IsNullOrEmpty(assetPackagePath))
                    {
                        ITMAssetPackageLoadContext assetPackageLoadContext = 
                            new AssetPackageLoadContext<TUserData>(userData, callback);
                        AssetBundleAsyncLoadTask task = new AssetBundleAsyncLoadTask(this, assetPackagePath, assetPackageLoadContext.OnAssetPackageTaskFinished);
                        task.Start();
                        m_AssetTaskList.AddLast(task);
                    }
                    else
                        Debugger.LogWarning("Parameter 'assetPackagePath' can not be null or empty string!");
                }
                else
                    Debugger.LogWarning("Parameter 'callback' can not be null!");
            }

            public void UnloadAssetPackage(string assetPackagePath)
            {
                if (!string.IsNullOrEmpty(assetPackagePath))
                {
                    AssetBundleDesc curDesc = null;
                    if(m_AssetBundleTable.TryGetValue(assetPackagePath,out curDesc))
                    {
                        List<string> assetLists = curDesc.AssetList;
                        for(int i = 0,icnt = assetLists.Count;i<icnt;++i)
                        {
                            LinkedList<string> packageList = null;
                            if(m_AssetPackageMapTable.TryGetValue(assetLists[i], out packageList))
                                packageList.Remove(assetPackagePath);
                        }

                        m_AssetBundleTable.Remove(assetPackagePath);
                        LinkedListNode<AssetBundleDesc> cur = m_AssetBundleList.First;
                        while(null != cur)
                        {
                            AssetBundleDesc curValue = cur.Value;
                            if (curValue == curDesc)
                            {
                                curValue.AssetBundle.Unload(true);
                                m_AssetBundleList.Remove(cur);
                                break;
                            }

                            cur = cur.Next;
                        }
                    }
                    else
                        Debugger.LogWarning("Can not find load asset package with path '{0}'!",assetPackagePath);
                }
                else
                    Debugger.LogWarning("Parameter 'assetPackagePath' can not be null or empty string!");
            }

            public uint LoadGameObject<TUserData>(string assetPath, TUserData userData, AssetLoadCallback<GameObject, TUserData> callback)
            {
                return LoadAsset(assetPath, userData, callback);
            }

            public uint LoadAsset<TAsset,TUserData>(string assetPath, TUserData userData, AssetLoadCallback<TAsset, TUserData> callback) where TAsset : class
            {
                if (!string.IsNullOrEmpty(assetPath))
                {
                    if (null != callback)
                    {
                        LinkedList<string> packagePathList = null;
                        if (m_AssetPackageMapTable.TryGetValue(assetPath.ToLower(), out packagePathList))
                        {
                            AssetBundleDesc desc = null;
                            if (m_AssetBundleTable.TryGetValue(packagePathList.Last.Value, out desc))
                            {
                                uint requestID = Utility.Handle.AllocHandle(SimulatorAssetRequestHandleType, ref m_AssetHandleAllocCount);
                                ITMAssetLoadContext assetLoadContext = new AssetLoadContext<TAsset, TUserData>(
                                    requestID, userData, callback);

                                AssetAsyncLoadTask task = new AssetAsyncLoadTask(this, desc.AssetBundle, assetPath,
                                    typeof(GameObject), string.Empty, assetLoadContext.OnAssetTaskFinished);
                                task.Start();
                                m_AssetTaskList.AddLast(task);
                                return requestID;
                            }
                            else
                                Debugger.LogWarning("Can not find package name with '{0}'!", packagePathList.Last.Value);
                        }
                        else
                            Debugger.LogWarning("Can not find asset '{0}' in loaded packages!", assetPath);
                    }
                    else
                        Debugger.LogWarning("Parameter 'callback' can not be null!");
                }
                else
                    Debugger.LogWarning("Parameter 'assetPath' can not be null or empty string!");

                return InvalidAssetReuqestHandle;
            }

            public void Update()
            {
                LinkedListNode<AsyncLoadTask> curTask = m_AssetTaskList.First, nextTask = null;
                while(null != curTask)
                {
                    nextTask = curTask.Next;
                    if (curTask.Value.Update())
                        m_AssetTaskList.Remove(curTask);

                    curTask = nextTask;
                }
            }
        }
    }
}