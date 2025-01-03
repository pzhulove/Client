
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal sealed partial class AssetManager
    {
        internal delegate void OnAssetTreeBuildComplete();

        internal class AssetTree
        {
            readonly AssetManager m_AssetManager;
            readonly OnAssetTreeBuildComplete m_OnAssetTreeBuildComplete; 

            public AssetTree(AssetManager assetManager)
            {
                Debugger.Assert(null != assetManager, "Asset manager object can not be null!");
                m_AssetManager = assetManager;
                m_OnAssetTreeBuildComplete = m_AssetManager._OnAssetTreeBuildComplete;
            }

            public void BuildAssetTree(DAssetPackageDependency assetTree)
            {
                if (null == assetTree)
                {
                    Debugger.LogError("Asset tree data can not be null!");
                    return;
                }

                _ClearAssetTree();
                long timeStampBegin = Utility.Time.GetTicksNow();
                                
                if (null != assetTree.packageDescArray)
                {
                    int[] defaultDependencyPackageIDs = new int[0];
                    for (int i= 0,icnt = assetTree.packageDescArray.Length;i<icnt;++i)
                    {
                        DAssetPackageDesc curAssetPackageDesc = assetTree.packageDescArray[i];
                        if (null == curAssetPackageDesc)
                        {
                            Debugger.LogError("Asset tree data is crashed!");
                            _ClearAssetTree();
                            return;
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
                                return;
                            }
                        }

                        AssetPackageName packageName = new AssetPackageName(curAssetPackageDesc.packageName, "");
                        EnumHelper<AssetPackageUsage> assetPackageUsage = new EnumHelper<AssetPackageUsage>(AssetPackageUsage.LoadFromFile);
                        if (0 != (curAssetPackageDesc.packageFlag & (uint)DAssetPackageFlag.UsingGUIDName))
                            assetPackageUsage.AddFlag(AssetPackageUsage.LoadAssetWithGUIDName);
                        m_AssetManager.m_AssetPackageDescTable.Add(packageName, new AssetPackageDesc(packageName,i, dependencyPackageIDs, assetPackageUsage, 0, ~0, curAssetPackageDesc.packageMD5));
                    }
                }

                if(null != assetTree.assetDescPackageMap)
                {
                    for (int i = 0, icnt = assetTree.assetDescPackageMap.Count; i < icnt; ++i)
                    {
                        DAssetPackageMapDesc curAssetMapDesc = assetTree.assetDescPackageMap[i];
                        if (0 <= curAssetMapDesc.packageDescIdx && curAssetMapDesc.packageDescIdx < assetTree.packageDescArray.Length)
                            m_AssetManager.m_AssetDescTable.Add(curAssetMapDesc.assetPathKey, new AssetDesc(curAssetMapDesc.assetPathKey, curAssetMapDesc.assetPackageGUID, curAssetMapDesc.packageDescIdx));
                        else
                        {
                            Debugger.LogError("Asset tree data is crashed!");
                            _ClearAssetTree();
                            return;
                        }
                    }
                }

                if (null != m_OnAssetTreeBuildComplete)
                    m_OnAssetTreeBuildComplete();
                Debugger.LogInfo("Init asset tree {0} (ms)!", Utility.Time.TicksToMicroseconds(Utility.Time.GetTicksNow() - timeStampBegin));
            }


            public void BuildAssetTree(ITMAssetTreeData assetTree)
            {
                if (null == assetTree)
                {
                    Debugger.LogError("Asset tree data can not be null!");
                    return;
                }
           
                List<AssetDesc> assetDescList = assetTree.GetAssetDescMap();
                List<AssetPackageDesc> assetPackageDescList = assetTree.GetAssetPackageDescMap();

                List<LinearMap<string, AssetDesc>.KeyValuePair<string, AssetDesc>> assetDescTable = new List<LinearMap<string, AssetDesc>.KeyValuePair<string, AssetDesc>>();
                for(int i = 0,icnt = assetDescList.Count;i<icnt;++i)
                {
                    AssetDesc curAssetDesc = assetDescList[i];
                    assetDescTable.Add(new LinearMap<string, AssetDesc>.KeyValuePair<string, AssetDesc>(curAssetDesc.AssetName, curAssetDesc));
                }

                List<LinearMap<AssetPackageName, AssetPackageDesc>.KeyValuePair<AssetPackageName, AssetPackageDesc>> assetPackageDescTable = new List<LinearMap<AssetPackageName, AssetPackageDesc>.KeyValuePair<AssetPackageName, AssetPackageDesc>>();
                for (int i = 0, icnt = assetPackageDescList.Count; i < icnt; ++i)
                {
                    AssetPackageDesc curAssetPackageDesc = assetPackageDescList[i];
                    assetPackageDescTable.Add(new LinearMap<AssetPackageName, AssetPackageDesc>.KeyValuePair<AssetPackageName, AssetPackageDesc>(curAssetPackageDesc.PackageName, curAssetPackageDesc));
                }

                m_AssetManager.m_AssetDescTable.Fill(assetDescTable, false);
                m_AssetManager.m_AssetPackageDescTable.Fill(assetPackageDescTable, false);

                if (null != m_OnAssetTreeBuildComplete)
                    m_OnAssetTreeBuildComplete();
            }

            private void _ClearAssetTree()
            {
                m_AssetManager.m_AssetDescTableIsReady = false;
                m_AssetManager.m_AssetDescTable.Clear();
                m_AssetManager.m_AssetPackageDescTableIsReady = false;
                m_AssetManager.m_AssetPackageDescTable.Clear();
            }
        }
    }
}