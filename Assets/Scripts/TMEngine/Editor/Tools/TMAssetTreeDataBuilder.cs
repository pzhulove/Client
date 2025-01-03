using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Tenmove.Runtime;

namespace Tenmove.Editor.Unity
{
    public class LinearMapForEditor<K, V> : LinearMap<K,V> where K : IComparable<K>, IComparable, IEquatable<K>
    {
        public LinearMapForEditor(bool ordered)
            : base(0, ordered)
        {
        }

        public LinearMapForEditor(int capacity, bool ordered)
            : base(capacity, ordered)
        {
        }

        public List<KeyValuePair<K, V>> Extract()
        {
            return m_ObjectMap;
        }

        public void EnsureOrder()
        {
            _EnsureOrder(true);
        }

        public bool TryGetIndex(K key,out int index)
        {
            _EnsureOrder(m_AutoSorting);
            index = ~0;
            m_TargetItem.Key = key;
            int idx = m_ObjectMap.BinarySearch(m_TargetItem, m_Comparer);
            if (0 <= idx && idx < m_ObjectMap.Count)
            {
                index = idx;
                return true;
            }
            
            return false;
        }

        public bool SetValueAt(int index, V value)
        {
            if (0 <= index && index < m_ObjectMap.Count)
            {
                m_ObjectMap[index].Value = value;
                return true;
            }

            return false;
        }
    }


    public class AssetTreeDataBuilder
    {
        private class AssetData
        {
            public AssetData(string assetPath,AssetPackageName assetPacakgeName)
            {
                AssetPath = assetPath;
                AssetPathHash = assetPath.GetHashCode();
                AssetPackageName = assetPacakgeName;
                AssetGUID = AssetDatabase.AssetPathToGUID(assetPath);

                string filePath = Runtime.Utility.Path.Combine("Assets/Resources/", assetPath);
                if(Runtime.Utility.File.Exists(filePath))
                    AssetBytes = Runtime.Utility.File.GetByteSize(filePath);
            }

            public string AssetPath { set; get; }
            public int AssetPathHash { set; get; }
            public AssetPackageName AssetPackageName { set; get; }
            public string AssetGUID { set; get; }
            public long AssetBytes { set; get; }
        }

        private class AssetPackageData
        {
            public AssetPackageData(string packageName,string packageVariant,List<string> packageAssets,uint packageUsage,long packageBytes,int packageCRC32,string packageMD5)
            {
                PackageName = packageName;
                PackageVariant = packageVariant;
                PackageAsset = packageAssets;
                PackageUsage = new EnumHelper<AssetPackageUsage>(packageUsage);
                PackageBytes = packageBytes;
                PackageCRC32 = packageCRC32;
                PackageMD5 = packageMD5;
                DependentPackage = new List<AssetPackageName>();
            }

            public string PackageName { set; get; }
            public string PackageVariant { set; get; }
            public List<string> PackageAsset { set; get; }
            public EnumHelper<AssetPackageUsage> PackageUsage { set; get; }
            public long PackageBytes { set; get; }
            public int PackageCRC32 { set; get; }
            public string PackageMD5 { set; get; }
            public List<AssetPackageName> DependentPackage { set; get; }
        }

        private readonly LinearMapForEditor<string, Runtime.AssetDesc> m_AssetDescList = new LinearMapForEditor<string, Runtime.AssetDesc>(true);
        private readonly LinearMapForEditor<AssetPackageName, Runtime.AssetPackageDesc> m_AssetPackageDescList = new LinearMapForEditor<AssetPackageName, Runtime.AssetPackageDesc>(true);

        private readonly List<AssetData> m_Assets = new List<AssetData>();
        private readonly HashSet<string> m_AssetSet = new HashSet<string>();
        private readonly List<AssetPackageData> m_AssetPackages = new List<AssetPackageData>();
        private readonly HashSet<string> m_AssetPackageSet = new HashSet<string>();

        private class AssetDiffDesc
        {
            public AssetDiffDesc(string assetPath,int flag)
            {
                AssetPath = assetPath;
                AssetFlag = flag;
            }

            public string AssetPath { set; get; }
            public int AssetFlag { set; get; }
        }

        public AssetTreeDataBuilder(string assetTreeDataPath)
        {
            _Init(assetTreeDataPath);
        }

        public void AddAssetPackageData(string packageName,string packageVariant,List<string> packageAssets,uint packageUsage)
        {
            List<string> normalizedList;
            _NormalizedAssetList(packageAssets,out normalizedList);
            _AddAssetPackageData(packageName, packageVariant, normalizedList,  packageUsage);
        }

        public bool ChangeAssetPacakgeData(string packageName,string packageVariant, List<string> packageAssets, uint packageUsage)
        {
            List<string> normalizedList;
            _NormalizedAssetList(packageAssets, out normalizedList);
            return _ChangeAssetPacakgeData(packageName, packageVariant, normalizedList, packageUsage);
        }

        public void RemoveAssetPackageData(string packageName,string packageVariant)
        {
            _RemoveAssetPackageData(packageName, packageVariant);
        }

        public Runtime.Unity.TMUnityAssetTreeData Build(AssetBundleManifest assetBundleManifest)
        {
            if (null == assetBundleManifest)
            {
                Debugger.LogError("Asset bundle manifest can not be null!");
                return null;
            }
            
            int[] defaultDependencyPackageIDs = new int[0];
            for (int i = 0, icnt = m_AssetPackages.Count; i < icnt; ++i)
            {
                AssetPackageData curPackageData = m_AssetPackages[i];
                if (null == curPackageData)
                {
                    Debugger.LogError("Asset tree data is crashed!");
                    _ClearAssetTree();
                    return null;
                }

                AssetPackageName packageName = new AssetPackageName(curPackageData.PackageName, curPackageData.PackageVariant);
                EnumHelper<AssetPackageUsage> assetPackageUsage = curPackageData.PackageUsage;
                m_AssetPackageDescList.Add(packageName, new AssetPackageDesc(packageName, ~0, defaultDependencyPackageIDs, assetPackageUsage, (int)curPackageData.PackageBytes, curPackageData.PackageCRC32, curPackageData.PackageMD5));
            }
            m_AssetPackageDescList.EnsureOrder();

            for ( int i = 0,icnt = m_AssetPackageDescList.Count;i<icnt;++i)
            {
                AssetPackageDesc curAssetPackageDesc;
                int[] dependPackageIDs = defaultDependencyPackageIDs;
                if (m_AssetPackageDescList.TryGetValueAt(i,out curAssetPackageDesc))
                {
                    if(_IsAssetPackageInManifest(curAssetPackageDesc.m_PackageName.Name, assetBundleManifest))
                        dependPackageIDs = _GetDependentIDsFromManifest(curAssetPackageDesc.m_PackageName.Name, assetBundleManifest) ?? defaultDependencyPackageIDs;
                    else
                        dependPackageIDs = _GetDependentIDsFromLastData(curAssetPackageDesc.m_PackageName.Name) ?? defaultDependencyPackageIDs;

                    AssetPackageDesc newAssetPackageDesc = new AssetPackageDesc(curAssetPackageDesc.PackageName, i, dependPackageIDs, curAssetPackageDesc.AssetPackageUsage, curAssetPackageDesc.PackageBytes, curAssetPackageDesc.PackageCRC32, curAssetPackageDesc.PackageMD5);
                    m_AssetPackageDescList.SetValueAt(i, newAssetPackageDesc);
                }
            }

            for (int i = 0, icnt = m_Assets.Count; i < icnt; ++i)
            {
                AssetData curAssetData = m_Assets[i];
                int packageIdx = ~0;
                if (m_AssetPackageDescList.TryGetIndex(curAssetData.AssetPackageName, out packageIdx))
                    m_AssetDescList.Add(curAssetData.AssetPath, new Runtime.AssetDesc(curAssetData.AssetPath, curAssetData.AssetGUID, packageIdx));
            }
            m_AssetDescList.EnsureOrder();
            
            Runtime.Unity.TMUnityAssetTreeData assetTreeData = ScriptableObject.CreateInstance<Runtime.Unity.TMUnityAssetTreeData>();
            
            List<Runtime.AssetDesc> assetDescList = new List<Runtime.AssetDesc>();
            for (int i = 0, icnt = m_AssetDescList.Count; i < icnt; ++i)
            {
                Runtime.AssetDesc curAssetDesc;
                if (m_AssetDescList.TryGetValueAt(i, out curAssetDesc))
                {
                    assetDescList.Add(curAssetDesc);
                    continue;
                }
            
                Debugger.LogError("Asset tree data is crashed!");
                _ClearAssetTree();
                return null;
            }
            
            List<Runtime.AssetPackageDesc> assetPackageDescList = new List<Runtime.AssetPackageDesc>();
            for (int i = 0, icnt = m_AssetPackageDescList.Count; i < icnt; ++i)
            {
                Runtime.AssetPackageDesc curAssetPackageDesc;
                if (m_AssetPackageDescList.TryGetValueAt(i, out curAssetPackageDesc))
                {
                    assetPackageDescList.Add(curAssetPackageDesc);
                    continue;
                }
            
                Debugger.LogError("Asset tree data is crashed!");
                _ClearAssetTree();
                return null;
            }
            
            assetTreeData.Fill(assetDescList, assetPackageDescList);
            return assetTreeData;
        }

        private void _AddAssetPackageData(string packageName, string packageVariant, List<string> packageAssets, uint packageUsage)
        {
            packageName = packageName.ToLower();
            if (!m_AssetPackageSet.Contains(packageName))
            {
                m_AssetPackageSet.Add(packageName);
                m_AssetPackages.Add(new AssetPackageData(packageName, packageVariant, packageAssets, packageUsage, 0, ~0, ""));

                for (int i = 0, icnt = packageAssets.Count; i < icnt; ++i)
                    _AddAssetData(packageAssets[i], new AssetPackageName(packageName, packageVariant));
            }
            else
                Debugger.LogWarning("Asset package with name '{0}' has already in list!", packageName);
        }

        private bool _ChangeAssetPacakgeData(string packageName, string packageVariant, List<string> packageAssets, uint packageUsage)
        {
            packageName = packageName.ToLower();
            if (m_AssetPackageSet.Contains(packageName))
            {
                AssetPackageName assetPackageName = new AssetPackageName(packageName, packageVariant);

                for (int i = 0, icnt = m_AssetPackages.Count; i < icnt; ++i)
                {
                    AssetPackageData curAssetPackageData = m_AssetPackages[i];
                    if (curAssetPackageData.PackageName == packageName
                        && curAssetPackageData.PackageVariant == packageVariant)
                    {
                        List<AssetDiffDesc> assetDiffList = new List<AssetDiffDesc>();
                        for (int j = 0, jcnt = curAssetPackageData.PackageAsset.Count; j < jcnt; ++j)
                            assetDiffList.Add(new AssetDiffDesc(curAssetPackageData.PackageAsset[j], -1));
                        for (int j = 0, jcnt = packageAssets.Count; j < jcnt; ++j)
                        {
                            AssetDiffDesc assetDesc = assetDiffList.Find(
                                (a) =>
                                {
                                    return a.AssetPath == packageAssets[j];
                                });

                            if (default(AssetDiffDesc) == assetDesc)
                                assetDiffList.Add(new AssetDiffDesc(packageAssets[j], 1));
                            else
                                assetDesc.AssetFlag += 1;
                        }

                        for (int j = 0, jcnt = assetDiffList.Count; j < jcnt; ++j)
                        {
                            AssetDiffDesc assetDesc = assetDiffList[j];
                            if (assetDesc.AssetFlag > 0)
                                _AddAssetData(assetDesc.AssetPath, assetPackageName);
                            else if (assetDesc.AssetFlag < 0)
                                _RemoveAssetData(assetDesc.AssetPath);
                            else
                                continue;
                        }

                        curAssetPackageData.PackageAsset.Clear();
                        curAssetPackageData.PackageAsset.AddRange(packageAssets);
                        curAssetPackageData.PackageUsage = new EnumHelper<AssetPackageUsage>(packageUsage);
                        curAssetPackageData.PackageBytes = 0;
                        curAssetPackageData.PackageCRC32 = ~0;
                        curAssetPackageData.PackageMD5 = "";

                        return true;
                    }
                }
            }

            Debugger.LogWarning("Can not find asset package with name '{0}' and variant name '{1}',Change package data has failed!", packageName, packageVariant);
            return false;
        }

        private void _RemoveAssetPackageData(string packageName, string packageVariant)
        {
            packageName = packageName.ToLower();
            if (m_AssetPackageSet.Contains(packageName))
            {

                for (int i = 0, icnt = m_AssetPackages.Count; i < icnt; ++i)
                {
                    AssetPackageData curAssetPackageData = m_AssetPackages[i];
                    if (curAssetPackageData.PackageName == packageName
                        && curAssetPackageData.PackageVariant == packageVariant)
                    {
                        for (int j = 0, jcnt = curAssetPackageData.PackageAsset.Count; j < jcnt; ++j)
                            _RemoveAssetData(curAssetPackageData.PackageAsset[j]);

                        m_AssetPackageSet.Remove(packageName);
                        m_AssetPackages.RemoveAt(i);
                        return;
                    }
                }
            }
            Debugger.LogWarning("Can not find asset package with name '{0}' and variant name '{1}',Remove package data has failed!", packageName, packageVariant);
        }

        protected void _AddAssetData(string assetPath,AssetPackageName assetPackageName)
        {
            if (!m_AssetSet.Contains(assetPath))
            {
                m_AssetSet.Add(assetPath);
                m_Assets.Add(new AssetData(assetPath,assetPackageName));
            }
        }

        protected void _RemoveAssetData(string assetPath)
        {
            if (m_AssetSet.Contains(assetPath))
            {
                m_AssetSet.Remove(assetPath);
                for(int i = 0,icnt = m_Assets.Count;i<icnt;++i)
                {
                    if(m_Assets[i].AssetPath == assetPath)
                    {
                        m_Assets.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        protected bool _IsAssetPackageInManifest(string assetPackageName, AssetBundleManifest assetBundleManifest)
        {
            string[] assetBundles = assetBundleManifest.GetAllAssetBundles();
            for(int i = 0,icnt = assetBundles.Length;i<icnt;++i)
            {
                if (assetPackageName == assetBundles[i])
                    return true;
            }

            return false;
        }

        protected int[] _GetDependentIDsFromList(string assetPackageName,string[] dependents)
        {
            int[] dependPackageIDs = null;
            if (null != dependents)
            {
                List<int> dependPackageIDList = new List<int>();
                List<LinearMap<AssetPackageName, AssetPackageDesc>.KeyValuePair<AssetPackageName, AssetPackageDesc>> findTable = m_AssetPackageDescList.Extract();

                for (int j = 0, jcnt = dependents.Length; j < jcnt; ++j)
                {
                    string curDependent = dependents[j];
                    if (curDependent == assetPackageName)
                        continue;

                    for (int k = 0, kcnt = findTable.Count; k < kcnt; ++k)
                    {
                        if (findTable[k].Key.Name == curDependent)
                        {
                            dependPackageIDList.Add(k);
                            break;
                        }
                    }
                }

                dependPackageIDs = dependPackageIDList.ToArray();
            }

            return dependPackageIDs;
        }


        protected int[] _GetDependentIDsFromManifest(string assetPackageName,AssetBundleManifest assetBundleManifest)
        {
            int[] dependPackageIDs = null;
            string[] dependents = assetBundleManifest.GetAllDependencies(assetPackageName);
            if (null != dependents)
            {
                dependPackageIDs = _GetDependentIDsFromList(assetPackageName, dependents);
            }

            return dependPackageIDs;
        }

        protected int[] _GetDependentIDsFromLastData(string assetPackageName)
        {
            int[] dependPackageIDs = null;
            for (int i = 0,icnt = m_AssetPackages.Count; i<icnt;++i)
            {
                AssetPackageData curPackage = m_AssetPackages[i];
                if(curPackage.PackageName == assetPackageName)
                {
                    string[] dependents = new string[0];
                    if (null != curPackage.DependentPackage)
                    {
                        dependents = new string[curPackage.DependentPackage.Count];
                        for (int j = 0, jcnt = curPackage.DependentPackage.Count; j < jcnt; ++j)
                            dependents[j] = curPackage.DependentPackage[j].Name;
                    }

                    dependPackageIDs = _GetDependentIDsFromList(assetPackageName, dependents);
                }
            }

            return dependPackageIDs;
        }

        private void _Init(string assetTreeDataPath)
        {
            _ClearAssetTree();
            Runtime.Unity.TMUnityAssetTreeData assetTreeData = AssetDatabase.LoadAssetAtPath<Runtime.Unity.TMUnityAssetTreeData>(assetTreeDataPath);
            if (null != assetTreeData)
            {
                List<Runtime.AssetDesc> assetDescList = assetTreeData.GetAssetDescMap();
                List<AssetPackageDesc> assetPacakgeDescList = assetTreeData.GetAssetPackageDescMap();

                for (int i = 0, icnt = assetPacakgeDescList.Count; i < icnt; ++i)
                {
                    AssetPackageDesc assetPackageDesc = assetPacakgeDescList[i];

                    List<string> assets = new List<string>();
                    for (int j = 0, jcnt = assetDescList.Count; j < jcnt; ++j)
                    {
                        Runtime.AssetDesc curAsset = assetDescList[j];
                        if (curAsset.AssetPackageID == assetPackageDesc.m_PackageID)
                            assets.Add(curAsset.AssetName);
                    }

                    List<AssetPackageName> assetPackages = new List<AssetPackageName>();
                    for (int j = 0, jcnt = assetPackageDesc.DependencyPackageIDs.Length; j < jcnt; ++j)
                    {
                        int curID = assetPackageDesc.DependencyPackageIDs[j];
                        if (0 <= curID && curID< assetPacakgeDescList.Count)
                            assetPackages.Add(assetPacakgeDescList[curID].PackageName);
                    }

                    AddAssetPackageData(assetPackageDesc.PackageName.Name, assetPackageDesc.PackageName.Variant, assets, assetPackageDesc.m_PackageUsage);
                    m_AssetPackages[m_AssetPackages.Count - 1].DependentPackage = assetPackages;
                }
            }
        }


        public void Clear()
        {
            _ClearAssetTree();
        }
        
        private void _ClearAssetTree()
        {
            m_AssetDescList.Clear();
            m_AssetPackageDescList.Clear();

            m_Assets.Clear();
            m_AssetSet.Clear();
            m_AssetPackages.Clear();
            m_AssetPackageSet.Clear();
        }

        private void _NormalizedAssetList(List<string> originList, out List<string> normlaizedList)
        {
            normlaizedList = new List<string>(originList.Count);
            for (int i = 0, icnt = originList.Count; i < icnt; ++i)
            {
                string newPath = Runtime.Utility.Path.ChangeExtension(originList[i], null).Replace("Assets/Resources/", null).ToLower();
                if (!_IsInList(newPath, normlaizedList))
                    normlaizedList.Add(newPath);
            }
        }

        private bool _IsInList(string path,List<string> list)
        {
            for (int i = 0, icnt = list.Count; i < icnt; ++i)
                if (list[i] == path)
                    return true;

            return false;
        }
    }

    public class AssetTreeDataConverter
    {
        static LinearMapForEditor<string, Runtime.AssetDesc> m_AssetDescList = new LinearMapForEditor<string, Runtime.AssetDesc>(false);
        static LinearMapForEditor<AssetPackageName, Runtime.AssetPackageDesc> m_AssetPackageDescList = new LinearMapForEditor<AssetPackageName, Runtime.AssetPackageDesc>(false);

        static public Runtime.Unity.TMUnityAssetTreeData Convert(DAssetPackageDependency assetPackageDependency)
        {
            if (null == assetPackageDependency)
            {
                Debugger.LogError("Asset package dependency can not be null!");
                return null;
            }

            int[] defaultDependencyPackageIDs = new int[0];
            if (null != assetPackageDependency.packageDescArray)
            {
                for (int i = 0, icnt = assetPackageDependency.packageDescArray.Length; i < icnt; ++i)
                {
                    DAssetPackageDesc curAssetPackageDesc = assetPackageDependency.packageDescArray[i];
                    if (null == curAssetPackageDesc)
                    {
                        Debugger.LogError("Asset tree data is crashed!");
                        _ClearAssetTree();
                        return null;
                    }

                    int[] dependencyPackageIDs = defaultDependencyPackageIDs;
                    if (null != curAssetPackageDesc.packageAutoDependIdx)
                    {
                        dependencyPackageIDs = new int[curAssetPackageDesc.packageAutoDependIdx.Length];
                        for (int j = 0, jcnt = curAssetPackageDesc.packageAutoDependIdx.Length; j < jcnt; ++j)
                        {
                            int idx = curAssetPackageDesc.packageAutoDependIdx[j];
                            if (0 <= idx && idx < icnt)
                            {
                                dependencyPackageIDs[j] = idx;
                                continue;
                            }

                            Debugger.LogError("Asset tree data is crashed!");
                            _ClearAssetTree();
                            return null;
                        }
                    }

                    AssetPackageName packageName = new AssetPackageName(curAssetPackageDesc.packageName, "");
                    EnumHelper<AssetPackageUsage> assetPackageUsage = new EnumHelper<AssetPackageUsage>(AssetPackageUsage.LoadFromFile);
                    if (0 != (curAssetPackageDesc.packageFlag & (uint)DAssetPackageFlag.UsingGUIDName))
                        assetPackageUsage.AddFlag(AssetPackageUsage.LoadAssetWithGUIDName);
                    m_AssetPackageDescList.Add(packageName, new AssetPackageDesc(packageName, ~0, dependencyPackageIDs, assetPackageUsage, 0, ~0, curAssetPackageDesc.packageMD5));
                }

                m_AssetPackageDescList.EnsureOrder();
            }

            for (int i = 0, icnt = m_AssetPackageDescList.Count; i < icnt; ++i)
            {
                AssetPackageDesc curAssetPackageDesc;
                int[] dependPackageIDs = defaultDependencyPackageIDs;
                if (m_AssetPackageDescList.TryGetValueAt(i, out curAssetPackageDesc))
                {
                    if (null != curAssetPackageDesc.DependencyPackageIDs)
                    {
                        List<int> dependPackageIDList = new List<int>();
                        for (int j = 0, jcnt = curAssetPackageDesc.DependencyPackageIDs.Length; j < jcnt; ++j)
                        {
                            int idx = curAssetPackageDesc.DependencyPackageIDs[j];
                            if (0 <= idx && idx < icnt)
                            {
                                DAssetPackageDesc curDependentPackageDesc = assetPackageDependency.packageDescArray[idx];
                                AssetPackageName packageName = new AssetPackageName(curDependentPackageDesc.packageName, "");
                                if (packageName == curAssetPackageDesc.m_PackageName)
                                    continue;

                                int packageIdx = ~0;
                                if (m_AssetPackageDescList.TryGetIndex(packageName, out packageIdx))
                                {
                                    dependPackageIDList.Add(packageIdx);
                                    continue;
                                }
                            }

                            Debugger.LogError("Asset tree data is crashed!");
                            _ClearAssetTree();
                            return null;
                        }

                        dependPackageIDs = dependPackageIDList.ToArray();
                    }

                    AssetPackageDesc newAssetPackageDesc = new AssetPackageDesc(curAssetPackageDesc.PackageName, i, dependPackageIDs, curAssetPackageDesc.AssetPackageUsage, curAssetPackageDesc.PackageBytes, curAssetPackageDesc.PackageCRC32, curAssetPackageDesc.PackageMD5);
                    m_AssetPackageDescList.SetValueAt(i, newAssetPackageDesc);
                }
            }

            if (null != assetPackageDependency.assetDescPackageMap)
            {
                for (int i = 0, icnt = assetPackageDependency.assetDescPackageMap.Count; i < icnt; ++i)
                {
                    DAssetPackageMapDesc curAssetMapDesc = assetPackageDependency.assetDescPackageMap[i];
                    if (0 <= curAssetMapDesc.packageDescIdx && curAssetMapDesc.packageDescIdx < assetPackageDependency.packageDescArray.Length)
                    {
                        DAssetPackageDesc curPackageDesc = assetPackageDependency.packageDescArray[curAssetMapDesc.packageDescIdx];
                        AssetPackageName packageName = new AssetPackageName(curPackageDesc.packageName, "");
                        int packageIdx = ~0;
                        if (m_AssetPackageDescList.TryGetIndex(packageName, out packageIdx))
                        {
                            m_AssetDescList.Add(curAssetMapDesc.assetPathKey, new Runtime.AssetDesc(curAssetMapDesc.assetPathKey, curAssetMapDesc.assetPackageGUID, packageIdx));
                        }
                    }
                    else
                    {
                        Debugger.LogError("Asset tree data is crashed!");
                        _ClearAssetTree();
                        return null;
                    }
                }
            }

            m_AssetDescList.EnsureOrder();

            Runtime.Unity.TMUnityAssetTreeData assetTreeData = ScriptableObject.CreateInstance<Runtime.Unity.TMUnityAssetTreeData>();

            List<Runtime.AssetDesc> assetDescList = new List<Runtime.AssetDesc>();
            for (int i = 0, icnt = m_AssetDescList.Count; i < icnt; ++i)
            {
                Runtime.AssetDesc curAssetDesc;
                if (m_AssetDescList.TryGetValueAt(i, out curAssetDesc))
                {
                    assetDescList.Add(curAssetDesc);
                    continue;
                }

                Debugger.LogError("Asset tree data is crashed!");
                _ClearAssetTree();
                return null;
            }

            List<Runtime.AssetPackageDesc> assetPackageDescList = new List<Runtime.AssetPackageDesc>();
            for (int i = 0, icnt = m_AssetPackageDescList.Count; i < icnt; ++i)
            {
                Runtime.AssetPackageDesc curAssetPackageDesc;
                if (m_AssetPackageDescList.TryGetValueAt(i, out curAssetPackageDesc))
                {
                    assetPackageDescList.Add(curAssetPackageDesc);
                    continue;
                }

                Debugger.LogError("Asset tree data is crashed!");
                _ClearAssetTree();
                return null;
            }

            assetTreeData.Fill(assetDescList, assetPackageDescList);
            return assetTreeData;
        }

        static private void _ClearAssetTree()
        {
            m_AssetDescList.Clear();
            m_AssetPackageDescList.Clear();
        }
    }
}