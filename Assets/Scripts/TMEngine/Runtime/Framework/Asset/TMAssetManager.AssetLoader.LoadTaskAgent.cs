using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {
            private enum LoadState
            {
                None,
                WaitForDependency,
                WaitForTarget,
            }

            private class AsyncLoadRecorder
            {
                private struct AsyncLoadRecord
                {
                    public AsyncLoadRecord(string name, LoadTaskAgent agent)
                    {
                        Name = name;
                        NameHash = name.GetHashCode();
                        Agent = agent;
                    }

                    public readonly string Name;
                    public readonly int NameHash;
                    public readonly LoadTaskAgent Agent;
                }

                private readonly List<AsyncLoadRecord> m_Records;

                public AsyncLoadRecorder()
                {
                    m_Records = new List<AsyncLoadRecord>();
                }

                public int Count
                {
                    get { return m_Records.Count; }
                }

                public bool Register(string name, LoadTaskAgent agent)
                {
                    if(string.IsNullOrEmpty(name))
                    {
                        Debugger.LogWarning("Parameter 'name' can not be null or empty string!");
                        return false;
                    }

                    if(null == agent)
                    {
                        Debugger.LogWarning("Parameter 'agent' can not be null!");
                        return false;
                    }

                    int hash = name.GetHashCode();
                    for (int i = 0, icnt = m_Records.Count; i < icnt; ++i)
                    {
                        AsyncLoadRecord cur = m_Records[i];
                        if (cur.NameHash == hash && cur.Name == name)
                        {
                            if (cur.Agent == agent)
                                return true;
                            else
                            {
                                Debugger.LogWarning("More than one asynchronize loader records with same name!");
                                return false;
                            }
                        }
                    }

                    m_Records.Add(new AsyncLoadRecord(name, agent));
                    return true;
                }

                public bool Unregister(string name)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        Debugger.LogWarning("Parameter 'name' can not be null or empty string!");
                        return false;
                    }

                    int hash = name.GetHashCode();
                    for (int i = 0, icnt = m_Records.Count; i < icnt; ++i)
                    {
                        AsyncLoadRecord cur = m_Records[i];
                        if (cur.NameHash == hash && cur.Name == name)
                        {
                            m_Records.RemoveAt(i);
                            return true;
                        }
                    }

                    return false;
                }

                public LoadTaskAgent FindRecordAgent(string name)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        Debugger.LogWarning("Parameter 'name' can not be null or empty string!");
                        return null;
                    }

                    int hash = name.GetHashCode();
                    for (int i = 0, icnt = m_Records.Count; i < icnt; ++i)
                    {
                        AsyncLoadRecord cur = m_Records[i];
                        if (cur.NameHash == hash && cur.Name == name)
                            return cur.Agent;
                    }

                    return null;
                }

                public bool HasRecord(string name)
                {
                    return null != FindRecordAgent(name);
                }
            }

            private readonly AsyncLoadRecorder m_AsyncLoadRecordAsset;
            private readonly AsyncLoadRecorder m_AsyncLoadRecordPackage;

            private void _RegisterRecordAsset(string assetName, LoadTaskAgent agent)
            {
                m_AsyncLoadRecordAsset.Register(assetName, agent);
            }

            private void _UnregisterRecordAsset(string assetName)
            {
                m_AsyncLoadRecordAsset.Unregister(assetName);
            }

            private bool _IsAssetInRecord(string assetName)
            {
                return m_AsyncLoadRecordAsset.HasRecord(assetName);
            }

            private void _RegisterRecordPackage(string packageName, LoadTaskAgent agent)
            {
                if(m_AssetPackagePool.CanSpawn(packageName))
                {
                    Debugger.LogWarning("Asset package '{0}' has already loaded!", packageName);
                    return;
                }

                m_AsyncLoadRecordPackage.Register(packageName, agent);
            }

            private void _UnregisterRecordPackage(string packageName)
            {
                m_AsyncLoadRecordPackage.Unregister(packageName);
            }

            private bool _IsPackageInRecord(string packageName)
            {
                return m_AsyncLoadRecordPackage.HasRecord(packageName);
            }

            private class LoadTaskAgent
            {
                private readonly AssetLoader m_AssetLoader;
                private readonly ITMReferencePool<Asset> m_AssetPool;
                private readonly ITMReferencePool<AssetPackage> m_AssetPackagePool;

                private TMResAsyncLoader m_ResAsyncLoader;
                private LoadTaskBase m_CurrentTask = null;
                private LoadState m_State = LoadState.None;

                public LoadTaskAgent(ITMReferencePool<Asset> assetPool, ITMReferencePool<AssetPackage> assetPackagePool, AssetLoader assetLoader)
                {
                    if (null == assetPool)
                        Debugger.AssertFailed("Asset pool is null!");

                    if (null == assetPackagePool)
                        Debugger.AssertFailed("Asset package pool is null!");

                    if (null == assetLoader)
                        Debugger.AssertFailed("Asset loader is null!");

                    m_AssetPool = assetPool;
                    m_AssetPackagePool = assetPackagePool;
                    m_AssetLoader = assetLoader;
                    m_ResAsyncLoader = m_AssetLoader.AllocateResAsyncLoader();

                    m_CurrentTask = null;
                    m_State = LoadState.None;
                }

                public LoadTaskBase Task
                {
                    get { return m_CurrentTask; }
                }

                public bool ChangedToSyncLoad
                {
                    get { return null == m_ResAsyncLoader ? true : m_ResAsyncLoader.ChangedToSyncLoad; }
                }

                public void Initialize()
                {
                    m_ResAsyncLoader.UpdateResourceEventHandler += _OnAssetLoadAgentUpdate;
                    m_ResAsyncLoader.LoadResourceCompleteEventHandler += _OnAssetLoadAgentLoadAssetComplete;
                    m_ResAsyncLoader.LoadResourceFailedEventHandler += _OnAssetLoadAgentLoadAssetFailed;
                    m_ResAsyncLoader.LoadPackageCompleteEventHandler += _OnAssetLoadAgentLoadPackageComplete;
                }

                public void Shutdown()
                {
                    m_ResAsyncLoader.UpdateResourceEventHandler -= _OnAssetLoadAgentUpdate;
                    m_ResAsyncLoader.LoadResourceCompleteEventHandler -= _OnAssetLoadAgentLoadAssetComplete;
                    m_ResAsyncLoader.LoadResourceFailedEventHandler -= _OnAssetLoadAgentLoadAssetFailed;
                    m_ResAsyncLoader.LoadPackageCompleteEventHandler -= _OnAssetLoadAgentLoadPackageComplete;

                    m_AssetLoader.RecycleAsyncLoader(m_ResAsyncLoader);
                    m_ResAsyncLoader = null;
                }

                public void Start(LoadTaskBase task)
                {
                    if (null == task)
                    {
                        Debugger.AssertFailed("Task is null!");
                    }

                    m_CurrentTask = task;
                    m_State = LoadState.None;
                    m_CurrentTask.OnStart(this);
                }

                public void SetAgentState(LoadState state)
                {
                    m_State = state;
                }

                public void LoadAssetAsync(AssetPackage package,string assetName,string subResName,Type assetType)
                {
                    if (null != package)
                        m_ResAsyncLoader.LoadAsset(package.Object, assetName, subResName, assetType);
                    else
                        m_ResAsyncLoader.LoadAsset(null, assetName, subResName, assetType);
                }

                public void LoadPackageAsync(string packageFullPath)
                {
                    m_ResAsyncLoader.LoadPackage(packageFullPath);
                }

                public bool ForceSyncLoadAsset()
                {
                    if (null != m_ResAsyncLoader)
                        return m_ResAsyncLoader.ForceSyncLoadAsset();
                    else
                    {
                        Debugger.LogWarning("Resource asynchronize loader is null!");
                        return false;
                    }
                }

                public void Reset()
                {
                    m_ResAsyncLoader.Reset();
                    m_CurrentTask = null;
                    m_State = LoadState.None;
                }

                public void Update(float logicDeltaTime, float realDeltaTime)
                {
                    m_ResAsyncLoader.Update();

                    switch (m_State)
                    {
                        case LoadState.None:
                            {/// Do nothing;
                                return;
                            }

                        case LoadState.WaitForDependency:
                            {
                                m_CurrentTask.OnWaitDependency(this);
                                return;
                            }

                        case LoadState.WaitForTarget:
                            {
                                m_CurrentTask.OnWaitTarget(this);
                                return;
                            }

                        default:
                            {
                                Debugger.AssertFailed(string.Format("Invalid state code:{0}", m_State));
                                return;
                            }
                    }
                }

                private void _OnAssetLoadAgentUpdate(object sender, LoadResourceUpdateEventArgs args)
                {
                    m_CurrentTask.OnLoadUpdate(this, args.Progress);
                }

                private void _OnAssetLoadAgentLoadAssetComplete(object sender, LoadResourceCompleteEventArgs args)
                {
                    AssetPackage assetPackage = m_CurrentTask.GetMainPackage();
                    Asset newAsset = new Asset(m_CurrentTask.AssetName, args.Asset as ITMAssetObject, assetPackage, m_AssetPackagePool);
                    m_AssetPool.Register(newAsset, true);
                    m_CurrentTask.OnTargetLoadReady(this, newAsset);
                }

                private void _OnAssetLoadAgentLoadAssetFailed(object sender, LoadResourceFailedEventArgs args)
                {
                    if(null != m_CurrentTask)
                        m_CurrentTask.OnLoadFailure(this, args.ErrorCode, args.Message);
                    else
                        UnityEngine.Debug.LogErrorFormat("Asset task which load '{0}' has already be reset!", args.ResName);
                }

                private void _OnAssetLoadAgentLoadPackageComplete(object sender, LoadPackageCompleteEventArgs args)
                {
                    if (null != m_CurrentTask)
                    {
                        string packageName = m_CurrentTask.AssetPackageDesc.PackageName.Name;
                        AssetPackage newPackage = new AssetPackage(packageName,
                            args.Package, m_ResAsyncLoader, m_AssetPackagePool);

                        m_AssetPackagePool.Register(newPackage, true);
                        m_CurrentTask.OnTargetLoadReady(this, newPackage);
                    }
                    else
                        UnityEngine.Debug.LogErrorFormat("Asset task which load '{0}' has already be reset!", args.PackageName);
                }
            }
        }
    }
}