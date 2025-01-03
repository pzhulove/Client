using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace AssetBundleTool
{
    public static partial class BuildPackage
    {
        private static void NonIncreseMentBuild(AssetBundleManifest assetManifestGrp)
        {
            //初始化数据
            Resources.UnloadUnusedAssets();
            List<AssetPackageDesc> buildPackageDesc = new List<AssetPackageDesc>();//需要打包的资源合集
            List<string> logPackageName = new List<string>();


            Dictionary<long, List<AssetPackageDesc>>.Enumerator it = sm_StrategyDataDic.GetEnumerator();
            int progress = 0;
            float t3 = Time.realtimeSinceStartup;

            while (it.MoveNext())
            {
                progress++;

                List<AssetPackageDesc> list = it.Current.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Ready to pack", "Gathering packaging resources..." + list[i].m_PackageName, (i + 0f) / list.Count);
                    List<FileAssetInfo> zNewFileMd5 = new List<FileAssetInfo>();//文件和文件MD5 
                    //记录下新md5
                    PrepareMd5OfNewAssets(list[i], zNewFileMd5);
                    //设置bundle包类别：新建或者更新 
                    SetBundleToNew(buildPackageDesc, list[i], zNewFileMd5);
                }
            }
            EditorUtility.ClearProgressBar();
            float t4 = Time.realtimeSinceStartup - t3;
            Debug.Log("[AB] Time to collect packaging resources：" + t4 / 60);
            Debug.Log("[AB] CurData" + System.DateTime.Now);

            List<string> newAssetBundleList = new List<string>();//有更改的AssetBundle 列表
            float t5 = Time.realtimeSinceStartup;
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            for (int i = 0; i < buildPackageDesc.Count; i++)
            {
                //设置打包的详细包体信息
                EditorUtility.DisplayProgressBar("Setting the enclave", "Set package details" + buildPackageDesc[i].m_PackageName, (i + 0f) / buildPackageDesc.Count);

                PrepareNewBuildsDetail(builds, buildPackageDesc[i], newAssetBundleList);
            }
            EditorUtility.ClearProgressBar();
            float t6 = Time.realtimeSinceStartup - t5;
            Debug.Log("[AB] Set the time of packing detailed package information：" + t6 / 60);
            Debug.Log("[AB] CurData" + System.DateTime.Now);

            if (builds.Count == 0 && checkBuildSet())
            {
                Debug.Log("[AB] No builds change found");
                Debug.Log("[AB] CurData" + System.DateTime.Now);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                return;
            }
            BuildPipelineExcute(assetManifestGrp, builds);
        }

        private static void PrepareMd5OfNewAssets(AssetPackageDesc desc, List<FileAssetInfo> zNewFileMd5)
        {
            foreach (var item in desc.m_DependAsset)
            {
                FileAssetInfo info = new FileAssetInfo();
                info.name = item.Key;
                info.md5 = item.Value;
                zNewFileMd5.Add(info);
            }
        }

        private static void SetBundleToNew(List<AssetPackageDesc> buildPackageDesc, AssetPackageDesc desc, List<FileAssetInfo> zNewFileMd5)
        {
            PackageFileMd5 md5 = new PackageFileMd5();
            md5.zPackageName = desc.m_PackageName;
            md5.zFileMd5 = zNewFileMd5;
            md5.zDependBundleList = desc.m_DependPackage;
            sm_AssetFileMd5.packFileMd5List.Add(md5);
            desc.m_UpdateType = AssetBundelType.New;
            buildPackageDesc.Add(desc);
        }

        private static void PrepareNewBuildsDetail(List<AssetBundleBuild> builds, AssetPackageDesc buildPackageDesc, List<string> updateTypeAssetBundleList)
        {
            AssetPackageDesc desc;
            AssetBundleBuild build;
            List<string> packageAssets;
            BuildDetail(buildPackageDesc, out desc, out build, out packageAssets);

            sm_treeDataBuilder.AddAssetPackageData(desc.m_PackageName, "", packageAssets, 0);
            builds.Add(build);
        }

    }
}