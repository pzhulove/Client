using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {
            private sealed class AssetTask : LoadTaskBase
            {
                readonly AssetLoadCallbacks<object> m_Callbacks;
                readonly bool m_LoadFromPackage;
                readonly AssetObjectDesc m_AssetObjectDesc;
                AssetPackage m_AssetPackage = null;

                public AssetTask(AssetLoader assetLoader,ITMReferencePool<Asset> assetPool,ITMReferencePool<AssetPackage> assetPackagePool,string assetName, string mainResPath, string subResName, Type assetType,AssetObjectDesc desc, AssetLoadCallbacks<object> callbacks, AssetPackageDesc packageDesc, string assetNameInPackage, int groupID, object userData)
                    : base(assetLoader, assetPool, assetPackagePool, assetName, packageDesc, groupID, userData)
                {
                    Debugger.Assert(!string.IsNullOrEmpty(mainResPath), "Asset name can not be null or empty string!");
                    Debugger.Assert(null != callbacks, "Asset load call back can not be null!");
                    
                    m_MainResPath = mainResPath;
                    m_SubResName = subResName;
                    m_AssetNameInPackage = assetNameInPackage;
                    m_LoadFromPackage = !string.IsNullOrEmpty(assetNameInPackage);

                    m_Callbacks = callbacks;

                    m_AssetType = assetType;
                    m_AssetObjectDesc = desc;
                }

                public sealed override void OnStart(LoadTaskAgent agent)
                {
                    if (m_AssetLoader._IsAssetInRecord(AssetName))
                    {
                        agent.SetAgentState(LoadState.WaitForTarget);
                        return;
                    }

                    Asset asset = m_AssetPool.Spawn(AssetName);
                    if (null != asset)
                    {
                        OnLoadSuccess(agent, asset, Utility.Time.TicksToSeconds(DateTime.Now.Ticks - StartTimeStamp));                     
                        return;
                    }

                    m_IsTaskInLoading = true;
                    m_AssetLoader._RegisterRecordAsset(AssetName, agent);
                    agent.SetAgentState(LoadState.WaitForDependency);
                    OnWaitDependency(agent);
                }

                public sealed override void OnWaitDependency(LoadTaskAgent agent)
                {
                    if (m_LoadFromPackage)
                    {
                        string packageName = m_AssetPackageDesc.PackageName.Name;

                        m_AssetPackage = m_AssetPackagePool.Spawn(packageName);
                        if (null == m_AssetPackage)
                        {
                            if (!m_AssetLoader._IsPackageInRecord(packageName))
                            {
                                OnLoadFailure(agent, AssetLoadErrorCode.DependencyLoadError, string.Format("Can not find dependency package with name '{0}' in loading!", packageName));                                
                                return;
                            }

                            agent.SetAgentState(LoadState.WaitForDependency);
                            return;
                        }
                    }

                    OnDependencyLoadReady(agent);
                }

                public sealed override void OnDependencyLoadReady(LoadTaskAgent agent)
                {
                    agent.SetAgentState(LoadState.WaitForTarget);

                    if (agent.ChangedToSyncLoad)
                    {
                        Asset asset = m_AssetPool.Spawn(AssetName);
                        if (null != asset)
                        {
                            agent.SetAgentState(LoadState.None);
                            OnLoadSuccess(agent, asset, Utility.Time.TicksToSeconds(DateTime.Now.Ticks - StartTimeStamp));
                            return;
                        }
                    }

                    if (m_LoadFromPackage)
                    {
                        if (null != m_AssetPackage)
                        {
                            agent.LoadAssetAsync(m_AssetPackage, AssetNameInPackage, SubResName, AssetType);
                        }
                        else
                        {
                            OnLoadFailure(agent, AssetLoadErrorCode.DependencyLoadError, string.Format("Dependency package with asset name '{0}' is null!", AssetName));
                            return;
                        }
                    }
                    else
                    {
                        agent.LoadAssetAsync(null, MainResPath, SubResName, AssetType);
                    }
                }

                public sealed override void OnWaitTarget(LoadTaskAgent agent)
                {
                    string assetName = AssetName;
                    if (m_AssetLoader._IsAssetInRecord(assetName))
                        return;

                    agent.SetAgentState(LoadState.None);
                    Asset asset = m_AssetPool.Spawn(assetName);
                    if (null == asset)
                    {
                        OnLoadFailure(agent,AssetLoadErrorCode.NotExist,string.Format("Asset '{0}' has been destroyed, or does not exist!", assetName));
                        return;
                    }

                    OnTargetLoadReady(agent,asset);
                }

                public sealed override void OnTargetLoadReady(LoadTaskAgent agent,object asset)
                {
                    OnLoadSuccess(agent, asset, Utility.Time.TicksToSeconds(DateTime.Now.Ticks -StartTimeStamp));
                }

                public sealed override void OnLoadSuccess(LoadTaskAgent agent, object asset, float duration)
                {
                    _ReleaseDependecyRequestCount();
                    if (m_IsTaskInLoading)
                    {
                        m_IsTaskInLoading = false;
                        m_AssetLoader._UnregisterRecordAsset(AssetName);
                    }

                    agent.SetAgentState(LoadState.None);
                    base.OnLoadSuccess(agent, asset, duration);
                    Asset assetObj = asset as Asset;
                    m_StateCode = LoadTaskState.Done;
                    if (null != assetObj)
                        m_Callbacks.OnAssetLoadSuccess(AssetName, assetObj.CreateAssetInst(m_AssetObjectDesc), m_TaskGroupID, duration, m_UserData);
                    else
                        m_Callbacks.OnAssetLoadFailure(AssetName, m_TaskGroupID, AssetLoadErrorCode.NullAsset, "Asset loaded is null!", m_UserData);
                }

                public sealed override void OnLoadFailure(LoadTaskAgent agent, AssetLoadErrorCode errorCoude, string message)
                {
                    _ReleaseDependecyRequestCount();

                    agent.SetAgentState(LoadState.None);
                    base.OnLoadFailure(agent, errorCoude, message);
                    if (m_IsTaskInLoading)
                    {
                        m_IsTaskInLoading = false;
                        m_AssetLoader._UnregisterRecordAsset(AssetName);
                    }
                    m_StateCode = LoadTaskState.Done;
                    m_Callbacks.OnAssetLoadFailure(AssetName,m_TaskGroupID, errorCoude, message, m_UserData);
                }

                public sealed override void OnLoadUpdate(LoadTaskAgent agent, float progress)
                {
                    base.OnLoadUpdate(agent, progress);
                    if (null != m_Callbacks.OnAssetLoadUpdate)
                        m_Callbacks.OnAssetLoadUpdate(AssetName,m_TaskGroupID, progress, m_UserData);
                }

                public sealed override void OnLoadDependencyDone(LoadTaskAgent agent, object dependency)
                {
                    base.OnLoadDependencyDone(agent, dependency);

                    AssetPackage assetPackageObject = dependency as AssetPackage;
                    if (null != assetPackageObject)
                    {
                        m_AssetPackage = assetPackageObject;
                        m_AssetPackage.AddRequestCount();
                    }
                    else
                    {
                        string errorMsg = string.Format("Dependency package '{0}' type cast error source object type '{1}' needed type:{2}",
                            AssetName, null == dependency ? "null" : dependency.GetType().Name , typeof(AssetPackage).Name);
                        OnLoadFailure(agent, AssetLoadErrorCode.DependencyLoadError, errorMsg);
                    }
                }

                public sealed override List<AssetPackage> GetDependencyPackages()
                {
                    List<AssetPackage> assetPackageList = new List<AssetPackage>(1);
                    assetPackageList.Add(m_AssetPackage);
                    return assetPackageList;
                }

                private void _ReleaseDependecyRequestCount()
                {
                    if (null != m_AssetPackage)
                    {
                        m_AssetPackage.ReleaseRequestCount();
                    }
                }

                public sealed override AssetPackage GetMainPackage()
                {
                    return m_AssetPackage;
                }

                public string MainResPath
                {
                    get { return m_MainResPath; }
                }

                public string SubResName
                {
                    get { return m_SubResName; }
                }

                public Type AssetType
                {
                    get { return m_AssetType; }
                }

                public string AssetNameInPackage
                {
                    get { return m_AssetNameInPackage; }
                }

                readonly private string m_MainResPath;
                readonly private string m_SubResName;
                readonly private string m_AssetGUIDName;
                readonly private string m_AssetNameInPackage;
                readonly private Type m_AssetType;

            }
        }
    }
}