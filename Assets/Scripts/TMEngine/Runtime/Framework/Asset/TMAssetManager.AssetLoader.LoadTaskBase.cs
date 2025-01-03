using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {
            internal enum LoadTaskState
            {
                None,
                Ready,
                Loading,
                Done,
            }

            private class LoadTaskBase
            {
                internal LoadTaskBase(AssetLoader assetLoader, ITMReferencePool<Asset> assetPool, ITMReferencePool<AssetPackage> assetPackagePool, string assetName,AssetPackageDesc assetPackageDesc,int groupID, object userData)
                {
                    Debugger.Assert(!string.IsNullOrEmpty(assetName), "Asset name can not be null or empty string!");

                    m_AssetLoader = assetLoader;
                    m_AssetPool = assetPool;
                    m_AssetPackagePool = assetPackagePool;
                    m_AssetName = assetName;
                    m_AssetPackageDesc = assetPackageDesc;

                    m_TaskID = ms_TaskIDAllocCount++;
                    m_TaskGroupID = groupID;

                    m_UserData = userData;
                    m_TimeStamp = System.DateTime.Now.Ticks;
                    m_StateCode = LoadTaskState.None;
                    m_DependencyPackageWaitingList = new LinkedList<string>();
                    m_IsTaskInLoading = false;
                }

                public virtual void OnStart(LoadTaskAgent agent)
                {
                }

                public virtual void OnWaitDependency(LoadTaskAgent agent)
                {
                }

                public virtual void OnDependencyLoadReady(LoadTaskAgent agent)
                {
                }

                public virtual void OnWaitTarget(LoadTaskAgent agent)
                {
                }

                public virtual void OnTargetLoadReady(LoadTaskAgent agent, object asset)
                {
                }

                public virtual void OnLoadSuccess(LoadTaskAgent agent, object asset, float duration)
                {
                }

                public virtual void OnLoadFailure(LoadTaskAgent agent, AssetLoadErrorCode errorCoude, string message)
                {
                }

                public virtual void OnLoadUpdate(LoadTaskAgent agent, float progress)
                {
                }

                public virtual void OnLoadDependencyDone(LoadTaskAgent agent, object dependency)
                {
                }

                public void AddDependencyCount()
                {
                    ++m_TotalDependentCount;
                }

                public virtual List<AssetPackage> GetDependencyPackages()
                {
                    return null;
                }

                public virtual AssetPackage GetMainPackage()
                {
                    return null;
                }

                public virtual bool LoadFromPackage()
                {
                    return false;
                }

                public string AssetName
                {
                    get { return m_AssetName; }
                }

                public int TaskID
                {
                    get { return m_TaskID; }
                }

                public int TaskGroupID
                {
                    get { return m_TaskGroupID; }
                }

                public long TimeStamp
                {
                    get { return m_TimeStamp; }
                }

                public object UserData
                {
                    get { return m_UserData; }
                }

                public int TotalDependencyCount
                {
                    get { return m_TotalDependentCount; }
                }

                public bool IsDone
                {
                    get { return LoadTaskState.Done == m_StateCode; }
                }

                public long StartTimeStamp
                {
                    get { return m_TimeStamp; }
                }

                public AssetPackageDesc AssetPackageDesc
                {
                    get { return m_AssetPackageDesc; }
                }

                static int ms_TaskIDAllocCount = 0;

                readonly protected AssetLoader m_AssetLoader;
                readonly protected ITMReferencePool<Asset> m_AssetPool;
                readonly protected ITMReferencePool<AssetPackage> m_AssetPackagePool;
                readonly protected int m_TaskID;
                readonly protected int m_TaskGroupID;
                readonly protected LinkedList<string> m_DependencyPackageWaitingList;

                readonly protected string m_AssetName;
                readonly protected AssetPackageDesc m_AssetPackageDesc;
                readonly protected long m_TimeStamp;
                readonly protected object m_UserData;

                protected LoadTaskState m_StateCode = LoadTaskState.None;
                protected int m_TotalDependentCount = 0;
                protected bool m_IsTaskInLoading = false;
            }
        }
    }
}