

using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {

            private sealed class PackageTask : LoadTaskBase
            {
                readonly LoadTaskBase m_MainTask;
                readonly List<AssetPackage> m_DependecyPackages;
                readonly bool m_AsDependency;
                AssetPackage m_TargetPackage;

                public PackageTask(AssetLoader assetLoader, ITMReferencePool<Asset> assetPool, ITMReferencePool<AssetPackage> assetPackagePool, string packageName, LoadTaskBase mainTask, AssetPackageDesc packageDesc, object userData,bool asDependecy)
                    : base(assetLoader, assetPool, assetPackagePool, packageName, packageDesc, mainTask.TaskGroupID, userData)
                {
                    Debugger.Assert(null != mainTask, "Main task can not be null!");

                    m_MainTask = mainTask;
                    m_MainTask.AddDependencyCount();
                    m_AsDependency = asDependecy;
                    m_DependecyPackages = asDependecy ? null:new List<AssetPackage>();
                    m_TargetPackage = null;
                }

                public sealed override void OnStart(LoadTaskAgent agent)
                {
                    if (m_AssetLoader._IsPackageInRecord(AssetName))
                    {
                        agent.SetAgentState(LoadState.WaitForTarget);
                        return;
                    }

                    m_TargetPackage = m_AssetPackagePool.Spawn(AssetName);
                    if (null != m_TargetPackage)
                    {
                        /// 如果之前作为依赖包加载过，现在又做为非依赖包加载，这时候需要把依赖包的计数加上去
                        if (m_AsDependency || !m_TargetPackage.AsDependency)
                        {
                            OnTargetLoadReady(agent, m_TargetPackage);
                            return;
                        }
                    }
                    else
                    {
                        m_IsTaskInLoading = true;
                        m_AssetLoader._RegisterRecordPackage(AssetName, agent);
                    }

                    if (m_TotalDependentCount > 0)
                    {
                        int[] dependencyPackageIDs = m_AssetPackageDesc.DependencyPackageIDs;
                        for (int i = 0, icnt = dependencyPackageIDs.Length; i < icnt; ++i)
                        {
                            string curDependentPackage = m_AssetLoader._GetAssetPackageNameByPackageID(dependencyPackageIDs[i]);
                            if (curDependentPackage == m_AssetPackageDesc.PackageName.Name)
                                continue;

                            if (!m_AssetPackagePool.CanSpawn(curDependentPackage))
                            {
                                if (!m_AssetLoader._IsPackageInRecord(curDependentPackage))
                                {
                                    OnLoadFailure(agent, AssetLoadErrorCode.DependencyLoadError,
                                        string.Format("Can not find dependency package with name '{0}' in loading!", curDependentPackage));
                                    return;
                                }

                                m_DependencyPackageWaitingList.AddLast(curDependentPackage);
                            }
                        }

                        if (m_DependencyPackageWaitingList.Count > 0)
                        {
                            agent.SetAgentState(LoadState.WaitForDependency);
                            return;
                        }
                    }

                    OnDependencyLoadReady(agent);
                }

                public sealed override void OnWaitDependency(LoadTaskAgent agent)
                {
                    LinkedListNode<string> cur = m_DependencyPackageWaitingList.First;
                    while (null != cur)
                    {
                        if (!m_AssetLoader._IsPackageInRecord(cur.Value))
                        {
                            LinkedListNode<string> next = cur.Next;
                            if (!m_AssetPackagePool.CanSpawn(cur.Value))
                            {
                                OnLoadFailure(agent, AssetLoadErrorCode.DependencyLoadError,
                                    string.Format("Can not find dependency package with name '{0}' in loading!", cur.Value));
                                return;
                            }

                            m_DependencyPackageWaitingList.Remove(cur);
                            cur = next;
                            continue;
                        }

                        cur = cur.Next;
                    }

                    if (m_DependencyPackageWaitingList.Count > 0)
                        return;
                    
                    OnDependencyLoadReady(agent);
                }

                public sealed override void OnDependencyLoadReady(LoadTaskAgent agent)
                {                   
                    agent.SetAgentState(LoadState.WaitForTarget);

                    if (agent.ChangedToSyncLoad)
                        m_TargetPackage = m_AssetPackagePool.Spawn(AssetName);

                    if (null == m_TargetPackage)
                    {
                        /// string packageLoadPath = Utility.Path.Combine(
                        ///     m_AssetPackageDesc.StorageInReadOnly ? m_AssetLoader.ReadOnlyPath : m_AssetLoader.ReadWritePath,
                        ///     Utility.Path.Combine(m_AssetLoader.PackageRootFolder, m_AssetPackageDesc.PackageName.FullName));
                        /// agent.LoadPackageAsync(packageLoadPath);

                        string packageLoadPath = Utility.Path.Combine(m_AssetLoader.ReadWritePath,
                            Utility.Path.Combine(m_AssetLoader.PackageRootFolder, m_AssetPackageDesc.PackageName.FullName));
                        if(!_IsAssetExistWithPath(packageLoadPath))
                        {
                            packageLoadPath = Utility.Path.Combine(m_AssetLoader.ReadOnlyPath,
                            Utility.Path.Combine(m_AssetLoader.PackageRootFolder, m_AssetPackageDesc.PackageName.FullName));
                        }
                        agent.LoadPackageAsync(packageLoadPath);
                    }
                    else
                        OnTargetLoadReady(agent, m_TargetPackage);
                }

                public sealed override void OnWaitTarget(LoadTaskAgent agent)
                {
                    string packageName = m_AssetPackageDesc.PackageName.Name;
                    if (m_AssetLoader._IsPackageInRecord(packageName))
                        return;

                    m_TargetPackage = m_AssetPackagePool.Spawn(packageName);
                    if (null == m_TargetPackage)
                    {
                        OnLoadFailure(agent, AssetLoadErrorCode.DependencyLoadError,
                           string.Format("Can not find package named '{0}'!", packageName));
                        return;
                    }

                    OnTargetLoadReady(agent, m_TargetPackage);
                }

                public sealed override void OnTargetLoadReady(LoadTaskAgent agent, object asset)
                {
                    agent.SetAgentState(LoadState.None);
                    OnLoadSuccess(agent, asset, Utility.Time.TicksToSeconds(Utility.Time.GetTicksNow() - StartTimeStamp));
                }

                public sealed override void OnLoadSuccess(LoadTaskAgent agent, object asset, float duration)
                {
                    m_TargetPackage = asset as AssetPackage;

                    m_TargetPackage.AddDependency(GetDependencyPackages());

                    _ReleaseDependecyRequestCount();

                    if (m_IsTaskInLoading)
                    {
                        m_IsTaskInLoading = false;
                        m_AssetLoader._UnregisterRecordPackage(AssetName);
                    }

                    base.OnLoadSuccess(agent, asset, duration);
                    m_MainTask.OnLoadDependencyDone(agent, asset);
                    m_StateCode = LoadTaskState.Done;
                }

                public sealed override void OnLoadFailure(LoadTaskAgent agent, AssetLoadErrorCode errorCode, string message)
                {
                    _ReleaseDependecyRequestCount();

                    base.OnLoadFailure(agent, errorCode, message);
                    if (m_IsTaskInLoading)
                    {
                        m_IsTaskInLoading = false;
                        m_AssetLoader._UnregisterRecordPackage(AssetName);
                    }
                    m_StateCode = LoadTaskState.Done;
                    m_MainTask.OnLoadDependencyDone(agent,null);
                }

                public sealed override void OnLoadDependencyDone(LoadTaskAgent agent, object dependency)
                {
                    base.OnLoadDependencyDone(agent, dependency);

                    AssetPackage assetPackageObject = dependency as AssetPackage;
                    if (null != assetPackageObject)
                    {
                        assetPackageObject.AddRequestCount();
                        m_DependecyPackages.Add(assetPackageObject);
                    }
                    else
                    {
                        string errorMsg = string.Empty;
                        if (null == dependency)
                            errorMsg = string.Format("Dependency package '{0}' is null!", AssetName);
                        else
                            errorMsg = string.Format("Dependency package '{0}' type cast error source object type '{1}' needed type:{2}", AssetName, dependency.GetType().Name, typeof(AssetPackage).Name);
                        OnLoadFailure(agent, AssetLoadErrorCode.DependencyLoadError, errorMsg);
                    }
                }

                public sealed override List<AssetPackage> GetDependencyPackages()
                {
                    return m_DependecyPackages;
                }

                public sealed override AssetPackage GetMainPackage()
                {
                    return m_TargetPackage;
                }

                private void _ReleaseDependecyRequestCount()
                {
                    if (null != m_DependecyPackages)
                    {
                        for (int i = 0, icnt = m_DependecyPackages.Count; i < icnt; ++i)
                            m_DependecyPackages[i].ReleaseRequestCount();
                    }
                }
            }
        }
    }
}