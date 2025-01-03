
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public interface ITMAssetPackage
    {
        string Name
        {
            get;
        }

        int NameHashCode
        {
            get;
        }

        List<ITMAssetPackage> ItDependentPackages
        {
            get;
        }

        List<ITMAssetPackage> PackagesDependentIt
        {
            get;
        }
    }

    internal sealed partial class AssetManager
    {
        private class AssetPackage : Referable, ITMAssetPackage
        {
            private readonly string m_PackageName;
            private readonly int m_PackageNameHashCode;
            private readonly ITMResourceLoader m_ResourceLoader;
            private readonly object m_PackageObject;
            private readonly ITMReferencePool<AssetPackage> m_AssetPackagePool;
            private List<AssetPackage> m_DependecyPackages;
            private int m_DependencyCount;
            private int m_AsyncRequestCount;
            private int m_AssetCount;
            
            private long m_TimeStamp;
            
            private class PackageDenpendentItDesc
            {
                public int Count;
                public AssetPackage AssetPackage;
            }
            private readonly List<PackageDenpendentItDesc> m_PackagesDependentIt;

            public AssetPackage(string name,object packageObject,ITMResourceLoader resourceLoader, ITMReferencePool<AssetPackage> assetPackagePool)
            {
                Debugger.Assert(!string.IsNullOrEmpty(name), "Package name can not be null or empty string!");
                Debugger.Assert(null != resourceLoader, "Resource loader can not be null!");
                Debugger.Assert(null != packageObject, "Package object can not be null!");
                Debugger.Assert(null != assetPackagePool, "Asset package pool object can not be null!");

                m_PackageName = name;
                m_PackageNameHashCode = name.GetHashCode();
                m_PackageObject = packageObject;
                m_ResourceLoader = resourceLoader;
                m_AssetPackagePool = assetPackagePool;
                m_DependencyCount = 0;
                m_AsyncRequestCount = 0;
                m_AssetCount = 0;

                m_PackagesDependentIt = new List<PackageDenpendentItDesc>();
            }

            public override string Name
            {
                get { return m_PackageName; }
            }

            public override int NameHashCode
            {
                get { return m_PackageNameHashCode; }
            }

            public override bool IsInUse
            {
                get { return SpawnCount > 0; }
            }

            public override bool IsLocked
            {
                get { return false; }
            }

            public override long LastUseTime
            {
                get { return m_TimeStamp; }
            }

            public override int SpawnCount
            {
                get { return m_AssetCount + m_DependencyCount + m_AsyncRequestCount; }
            }

            public override void Lock(bool bLock) { }

            public object Object
            {
                get { return m_PackageObject; }
            }

            public bool AsDependency
            {
                get { return null == m_DependecyPackages; }
            }

            public List<ITMAssetPackage> ItDependentPackages
            {
                get
                {
                    List< ITMAssetPackage > itDependentPackages =  new List<ITMAssetPackage>();
                    if (null != m_DependecyPackages)
                    {
                        for (int i = 0, icnt = m_DependecyPackages.Count; i < icnt; ++i)
                            itDependentPackages.Add(m_DependecyPackages[i]);
                    }

                    return itDependentPackages;
                }
            }

            public List<ITMAssetPackage> PackagesDependentIt
            {
                get
                {
                    List<ITMAssetPackage> packagesDependentIt = new List<ITMAssetPackage>();
                    if (null != m_PackagesDependentIt)
                    {
                        for (int i = 0, icnt = m_PackagesDependentIt.Count; i < icnt; ++i)
                            packagesDependentIt.Add(m_PackagesDependentIt[i].AssetPackage);
                    }

                    return packagesDependentIt;
                }
            }

            public void AddDependency(List<AssetPackage> dependencyPackages)
            {
                if (null == m_DependecyPackages)
                {
                    m_DependecyPackages = dependencyPackages;
                    if (null != m_DependecyPackages)
                    {
                        for (int i = 0, icnt = m_DependecyPackages.Count; i < icnt; ++i)
                        {
                            m_DependecyPackages[i]._AddDependencyCount();
                            m_DependecyPackages[i]._RegisterPackageDependentOnIt(this);
                        }
                    }
                }
            }


            public void SpawmAsset()
            {
                ++m_AssetCount;
            }

            public void UnspawnAsset()
            {
                --m_AssetCount;
                Debugger.Assert(m_AssetCount >= 0, "Asset count can not less than zero!");
            }

            public void AddRequestCount()
            {
                ++m_AsyncRequestCount;
                //Debugger.LogInfo("### Add Request Count '{0}':{1} [TaskID:{2}]", Name, m_AsyncRequestCount, taskID);
            }

            public void ReleaseRequestCount()
            {
                --m_AsyncRequestCount;
                //Debugger.LogInfo("### Release Request Count '{0}':{1}", Name, m_AsyncRequestCount);
                Debugger.Assert(m_AsyncRequestCount >= 0, "Request count can not less than zero!");
            }

            public sealed override void OnSpawn()
            {
                m_TimeStamp = Utility.Time.GetTicksNow();
            }

            public sealed override void OnUnspawn()
            {
            }

            public sealed override void OnRelease()
            {
                if (null != m_DependecyPackages)
                {
                    for (int i = 0, icnt = m_DependecyPackages.Count; i < icnt; ++i)
                    {
                        AssetPackage dependency = m_DependecyPackages[i];
                        dependency._ReleaseDependencyCount();
                        dependency._DeregisterPackageDependentOnIt(this);
                    }
                }

                m_ResourceLoader.UnloadPackage(m_PackageName);
            }

            private void _AddDependencyCount()
            {
                ++m_DependencyCount;
                //Debugger.LogInfo("### Add Dependent Count '{0}':{1} [MainPackage:{2}]", Name, m_DependencyCount, mainPackage);
            }

            private void _ReleaseDependencyCount()
            {
                --m_DependencyCount;
                //Debugger.LogInfo("### Release Dependent Count '{0}':{1} [MainPackage:{2}]", Name, m_DependencyCount, mainPackage);
                Debugger.Assert(m_DependencyCount >= 0, "Dependent count can not less than zero!");
            }

            private void _RegisterPackageDependentOnIt(AssetPackage assetPackage)
            {
                for (int i = 0, icnt = m_PackagesDependentIt.Count; i < icnt; ++i)
                {
                    PackageDenpendentItDesc cur = m_PackagesDependentIt[i];
                    if (cur.AssetPackage.NameHashCode == assetPackage.NameHashCode &&
                        cur.AssetPackage.Name == assetPackage.Name)
                    {
                        ++cur.Count;
                        return;
                    }
                }

                PackageDenpendentItDesc newDesc = new PackageDenpendentItDesc();
                newDesc.AssetPackage = assetPackage;
                newDesc.Count = 1;
                m_PackagesDependentIt.Add(newDesc);
            }

            private void _DeregisterPackageDependentOnIt(ITMAssetPackage assetPackage)
            {
                for (int i = 0, icnt = m_PackagesDependentIt.Count; i < icnt; ++i)
                {
                    PackageDenpendentItDesc cur = m_PackagesDependentIt[i];
                    if (cur.AssetPackage.NameHashCode == assetPackage.NameHashCode &&
                        cur.AssetPackage.Name == assetPackage.Name)
                    {
                        --cur.Count;

                        if (cur.Count <= 0)
                            m_PackagesDependentIt.RemoveAt(i);

                        return;
                    }
                }

                Debugger.LogWarning("Can not find package '{0}', Deregister has failed!", assetPackage.Name);
            }
        }
    }
}