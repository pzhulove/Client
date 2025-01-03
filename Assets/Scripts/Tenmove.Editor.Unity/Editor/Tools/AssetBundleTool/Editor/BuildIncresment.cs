using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace AssetBundleTool
{
    public static partial class BuildPackage
    {
        private static void IncreseMentBuild(AssetBundleManifest assetManifestGrp)
        {
            //初始化数据
            List<AssetPackageDesc> buildPackageDesc = new List<AssetPackageDesc>();//需要打包的资源合集
            List<string> logPackageName = new List<string>();
            CheckDeleteBundelName(sm_StrategyDataDic, sm_AssetFileMd5, sm_treeDataBuilder);
            Dictionary<long, List<AssetPackageDesc>>.Enumerator it = sm_StrategyDataDic.GetEnumerator();
            int progress = 0;
            //前操作
            if (_PreActions != null)
            {
                _PreActions();
            }
            float t3 = Time.realtimeSinceStartup;

            while (it.MoveNext())
            {
                progress++;

                List<AssetPackageDesc> list = it.Current.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Ready to pack", "Gathering packaging resources..." + list[i].m_PackageName, (i + 0f) / list.Count);
                    SetBundleType(buildPackageDesc, logPackageName, list[i]);
                }
            }
            EditorUtility.ClearProgressBar();
            float t4 = Time.realtimeSinceStartup - t3;
            Debug.Log("[AB] Time to collect packaging resources：" + t4 / 60);
            Debug.Log("[AB] CurData" + System.DateTime.Now);

            List<string> updateTypeAssetBundleList = new List<string>();//有更改的AssetBundle 列表
            float t5 = Time.realtimeSinceStartup;
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            for (int i = 0; i < buildPackageDesc.Count; i++)
            {
                //设置打包的详细包体信息
                EditorUtility.DisplayProgressBar("Setting the enclave", "Set package details" + buildPackageDesc[i].m_PackageName, (i + 0f) / buildPackageDesc.Count);

                PrepareBuildsDetail(builds, buildPackageDesc[i], updateTypeAssetBundleList);
            }
            EditorUtility.ClearProgressBar();
            float t6 = Time.realtimeSinceStartup - t5;
            Debug.Log("[AB] Set the time of packing detailed package information：" + t6 / 60);
            Debug.Log("[AB] CurData" + System.DateTime.Now);
            List<string> logList = new List<string>();

            Dictionary<string, string> oldBundleMd5Dic = new Dictionary<string, string>();
            Dictionary<string, string> newBundleMd5Dic = new Dictionary<string, string>();
            //删除所有有更新的AssetBundle
            DeleteOldBundleAndManifest(updateTypeAssetBundleList, logList, oldBundleMd5Dic);
            if (builds.Count == 0 && checkBuildSet())
            {
                //EditorUtility.DisplayDialog("Error", "File changes and packaging output settings changes not found, no repackaging required", "Determine");
                Debug.Log("[AB] No builds change found");
                Debug.Log("[AB] CurData" + System.DateTime.Now);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                return;
            }
            BuildPipelineExcute(assetManifestGrp, builds);
            //打印打包情况
            GeneratePackAfterLog(logPackageName, updateTypeAssetBundleList, logList, oldBundleMd5Dic, newBundleMd5Dic, assetManifestGrp);
        }
        /// <summary>
        /// 根据有用的bundle名称对比之前的清除废弃的bundle包
        /// </summary>
        /// <param name="it1">有用的</param>
        /// <param name="it2">需要删除的</param>
        /// <param name="treeDataBuilder"></param>
        private static void CheckDeleteBundelName(Dictionary<long, List<AssetPackageDesc>> it1, AssetBundelFileMd5 it2, Tenmove.Editor.Unity.AssetTreeDataBuilder treeDataBuilder)
        {
            Dictionary<string, AssetPackageDesc> apdDic = new Dictionary<string, AssetPackageDesc>();
            var it = it1.GetEnumerator();
            while (it.MoveNext())
            {
                List<AssetPackageDesc> apd = it.Current.Value;
                for (int i = 0; i < apd.Count; i++)
                {
                    if (!apdDic.ContainsKey(apd[i].m_PackageName))
                    {
                        apdDic.Add(apd[i].m_PackageName, apd[i]);
                    }
                }
            }
            for (int i = 0; i < it2.packFileMd5List.Count; i++)
            {
                if (!apdDic.ContainsKey(it2.packFileMd5List[i].zPackageName))
                {
                    string tempPath = Path.Combine(sm_UserData.m_OutputPath, it2.packFileMd5List[i].zPackageName);
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                    if (File.Exists(tempPath + ".manifest"))
                    {
                        File.Delete(tempPath + ".manifest");
                    }
                    Debug.LogFormat("Remove scrap AssetBundel--->>>>{0}", it2.packFileMd5List[i].zPackageName);
                    treeDataBuilder.RemoveAssetPackageData(it2.packFileMd5List[i].zPackageName, "");
                    it2.packFileMd5List.RemoveAt(i);
                    i--;
                }
            }
        }
        
        private static void SetBundleType(List<AssetPackageDesc> buildPackageDesc, List<string> logPackageName, AssetPackageDesc desc)
        {
            PackageFileMd5 allFileMd5 = GetPackageAllFileMd5(desc.m_PackageName, sm_AssetFileMd5);
            Dictionary<string, string> zOldFileMd5 = new Dictionary<string, string>();//旧的资源与md5
            List<FileAssetInfo> zNewFileMd5 = new List<FileAssetInfo>();//文件和文件MD5 


            //准备资源的新旧md6等待校验
            PrepareMd5OfAssets(desc, allFileMd5, zOldFileMd5, zNewFileMd5);
            //设置bundle包类别：新建或者更新 
            SetBundleToUpdateOrNew(buildPackageDesc, desc, allFileMd5, zOldFileMd5, zNewFileMd5, logPackageName);
        }
        private static void PrepareBuildsDetail(List<AssetBundleBuild> builds, AssetPackageDesc buildPackageDesc, List<string> updateTypeAssetBundleList)
        {
            AssetPackageDesc desc = buildPackageDesc;
            AssetBundleBuild build = new AssetBundleBuild();
            List<string> packageAssets = new List<string>();
            BuildDetail(buildPackageDesc, out desc, out build, out packageAssets);
            //更改包名与地址
            if (desc.m_UpdateType == AssetBundelType.Update)
            {
                updateTypeAssetBundleList.Add(desc.m_PackageName);
                sm_treeDataBuilder.ChangeAssetPacakgeData(desc.m_PackageName, "", packageAssets, 0);
            }
            //新建
            if (desc.m_UpdateType == AssetBundelType.New)
            {
                sm_treeDataBuilder.AddAssetPackageData(desc.m_PackageName, "", packageAssets, 0);
            }
            builds.Add(build);
        }
        private static void DeleteOldBundleAndManifest(List<string> updateTypeAssetBundleList, List<string> logList, Dictionary<string, string> oldBundleMd5Dic)
        {
            for (int i = 0; i < updateTypeAssetBundleList.Count; i++)
            {
                string filePath = Path.Combine(sm_UserData.m_OutputPath, updateTypeAssetBundleList[i]);
                if (File.Exists(filePath))
                {
                    oldBundleMd5Dic.Add(updateTypeAssetBundleList[i], FileUtil.GetFileMD5(filePath));
                    File.Delete(filePath);
                    //logList.Add(string.Format("Delete old AssetBundle package{0}", filePath));
                }
                if (File.Exists(filePath + ".manifest"))
                {
                    File.Delete(filePath + ".manifest");
                    //logList.Add(string.Format("Delete the old AssetBundle package manifest file{0}", filePath + ".manifest"));
                }
                //logList.Add(string.Format("Update AssetBundle package{0}", filePath));
            }
        }
        private static void GeneratePackAfterLog(List<string> logPackageName, List<string> updateTypeAssetBundleList, List<string> logList, Dictionary<string, string> oldBundleMd5Dic, Dictionary<string, string> newBundleMd5Dic, AssetBundleManifest assetManifestGrp)
        {
            if (assetManifestGrp == null)
            {
                Debug.Log("Package failed -- > interrupt please check the console error message");

                for (int i = 0; i < logPackageName.Count; i++)
                {
                    Debug.LogFormat("AssetBundle   List  {0}   ------->>>>>>>>>   {1}", i, logPackageName[i]);
                }
            }
            for (int i = 0; i < updateTypeAssetBundleList.Count; i++)
            {
                string filePath = Path.Combine(sm_UserData.m_OutputPath, updateTypeAssetBundleList[i]);
                if (File.Exists(filePath))
                {
                    newBundleMd5Dic.Add(updateTypeAssetBundleList[i], FileUtil.GetFileMD5(filePath));
                }
            }
        }
        static private void PostBuildAssetPackage()
        {
            Resources.UnloadUnusedAssets();
            _GenerateCheckListAndCalculateMD5();
        }
        /// <summary>
        /// 生成检查列表计算md5值
        /// </summary>
        /// <param name="exportCheckList"></param>
        static private void _GenerateCheckListAndCalculateMD5(bool exportCheckList = true)
        {
            string[] allPackages = Directory.GetFiles(sm_UserData.m_OutputPath, "*.*");
            List<string> allPackagesList = new List<string>(1024);
            for (int i = 0, icnt = allPackages.Length; i < icnt; ++i)
            {
                string curPath = allPackages[i];
                if (curPath.EndsWith(".pck", System.StringComparison.OrdinalIgnoreCase) ||
                    curPath.EndsWith(".pak", System.StringComparison.OrdinalIgnoreCase))
                {
                    allPackagesList.Add(Tenmove.Runtime.Utility.Path.Normalize(curPath));
                }
            }
            //结果按名称排序
            allPackagesList.Sort((string str1, string str2) =>
            {
                int str1Len = str1.Length;
                int str2Len = str2.Length;

                int compareLen = Tenmove.Runtime.Utility.Math.Min(str1Len, str2Len);
                for (int i = 0; i < compareLen; ++i)
                {
                    if (str1[i] != str2[i])
                        return str1[i] - str2[i];
                }

                if (str1Len < str2Len)
                    return 0 - str2[compareLen];
                else if (str1Len > str2Len)
                    return str1[compareLen];
                else
                    return 0;
            });

            //所有package的md5信息集合
            List<string> allPackageListMD5 = new List<string>(allPackagesList.Count);
            if (exportCheckList)
            {
                using (Stream file = Tenmove.Runtime.Utility.File.OpenWrite(sm_checkFile, true))
                {
                    StreamWriter sw = new StreamWriter(file);
                    for (int i = 0, icnt = allPackagesList.Count; i < icnt; ++i)
                    {
                        string curPackage = allPackagesList[i];
                        string md5 = FileUtil.GetFileMD5(curPackage);
                        long size = FileUtil.GetFileBytes(curPackage);
                        string fileName = Tenmove.Runtime.Utility.Path.GetFileName(allPackagesList[i]);

                        allPackageListMD5.Add(md5);
                        sw.WriteLine(string.Format("{0},{1},{2}", fileName, md5, size));
                    }

                    sw.Flush();
                    file.Flush();
                }
            }
            else
            {
                for (int i = 0, icnt = allPackagesList.Count; i < icnt; ++i)
                    allPackageListMD5.Add(FileUtil.GetFileMD5(allPackagesList[i]));
            }

            using (Stream stream = Tenmove.Runtime.Utility.Memory.OpenStream(allPackageListMD5.Count * 32))
            {
                byte[] byteBuf = null;
                stream.Seek(0, SeekOrigin.Begin);
                for (int i = 0, icnt = allPackageListMD5.Count; i < icnt; ++i)
                {
                    byteBuf = System.Text.Encoding.ASCII.GetBytes(allPackageListMD5[i]);
                    stream.Write(byteBuf, 0, byteBuf.Length);
                }
                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                Tenmove.Runtime.MD5Verifier verifier = new Tenmove.Runtime.MD5Verifier();
                if (verifier.BeginVerify(stream, 128 * 1024, 0.1f))
                {
                    bool isEnd = false;
                    do
                    {/// 危险需要加入熔断措施 异常捕获退出 或者时间机制
                        isEnd = verifier.EndVerify();
                    }
                    while (!isEnd);
                }

                using (Stream verifacation = Tenmove.Runtime.Utility.File.OpenWrite("Assets/StreamingAssets/AssetBundles/verification.md5", true))
                {
                    StreamWriter sw = new StreamWriter(verifacation);
                    sw.WriteLine(verifier.GetVerifySum());
                    sw.Flush();
                }
            }
        }
        private static void SetBundleToUpdateOrNew(List<AssetPackageDesc> buildPackageDesc, AssetPackageDesc desc, PackageFileMd5 allFileMd5, Dictionary<string, string> zOldFileMd5, List<FileAssetInfo> zNewFileMd5, List<String> logPackageName)
        {
            if (allFileMd5 == null)
            {
                PackageFileMd5 md5 = new PackageFileMd5();
                md5.zPackageName = desc.m_PackageName;
                md5.zFileMd5 = zNewFileMd5;
                md5.zDependBundleList = desc.m_DependPackage;
                sm_AssetFileMd5.packFileMd5List.Add(md5);
                desc.m_UpdateType = AssetBundelType.New;
                buildPackageDesc.Add(desc);
            }
            else
            {
                //md5相同 检测资源是否更换了文件夹 如果资源被打到自己或者其他AssetBundle包里 则重新打包该资源
                bool hasChange = BundleIfUpate(desc, allFileMd5, zOldFileMd5);
                if (hasChange)
                {
                    allFileMd5.zDependBundleList = desc.m_DependPackage;
                    allFileMd5.zFileMd5 = zNewFileMd5;
                    desc.m_UpdateType = AssetBundelType.Update;
                }
            }

            if (desc.m_UpdateType == AssetBundelType.Update)
            {
                SetUpdateBundleRecursive(desc, buildPackageDesc, logPackageName, sm_AssetFileMd5);
            }
        }

        private static bool BundleIfUpate(AssetPackageDesc desc, PackageFileMd5 allFileMd5, Dictionary<string, string> zOldFileMd5)
        {
            //判断依赖bundle包数量是否一样
            if (allFileMd5.zDependBundleList.Count != desc.m_DependPackage.Count)
            {
                return true;
            }
            //判断具体bundle包是否一致都拥有
            for (int j = 0; j < allFileMd5.zDependBundleList.Count; j++)
            {
                if (desc.m_DependPackage.Count != 0 && !desc.m_DependPackage.Contains(allFileMd5.zDependBundleList[j]))
                {
                    return true;
                }
            }
            //仔细比对资源md5
            if (CheckPackageFileMd5(zOldFileMd5, desc.m_DependAsset))
            {
                return true;
            }
            if (!File.Exists(sm_UserData.m_OutputPath + "/" + desc.m_PackageName))
            {
                Debug.LogWarning("Fail To Find Bundle:" + sm_UserData.m_OutputPath + "/" + desc.m_PackageName);
                return true;
            }

            return false;
        }
        /// <summary>
        /// 检查新旧md5不同
        /// </summary>
        /// <param name="oldDic"></param>
        /// <param name="newDic"></param>
        /// <returns></returns>
        private static bool CheckPackageFileMd5(Dictionary<string, string> oldDic, Dictionary<string, string> newDic)
        {
            bool changeFile = false;
            //新旧资源数量差异
            if (oldDic.Count != newDic.Count)
            {
                changeFile = true;
                Debug.Log("存在增删----" );
                //return changeFile;
            }
            //差集旧的有，新的没
            foreach (var item in oldDic)
            {
                if (!newDic.ContainsKey(item.Key))
                {
                    changeFile = true;
                    Debug.Log("删------" + item.Key);
                    //return changeFile;
                }
            }
            //差集新的有，旧的没
            foreach (var item in newDic)
            {
                if (!oldDic.ContainsKey(item.Key))
                {
                    changeFile = true;
                    Debug.Log("增------" + item.Key);
                    //return changeFile;
                }
            }
            //文件没有增删，内容发生变化
            var it = oldDic.GetEnumerator();
            while (it.MoveNext())
            {
                if (newDic.ContainsKey(it.Current.Key) && !it.Current.Value.Equals(newDic[it.Current.Key]))
                {
                    changeFile = true;
                    Debug.Log("首个更改------" + it.Current.Key);
                    //return changeFile;
                }
            }
            return changeFile;
        }

        private static void InitAssetBundelFileMd5()
        {
            sm_AssetFileMd5 = AssetDatabase.LoadAssetAtPath<AssetBundelFileMd5>(sm_fileMd5Path);
            if (sm_AssetFileMd5 == null)
            {
                sm_AssetFileMd5 = ScriptableObject.CreateInstance<AssetBundelFileMd5>();
                AssetDatabase.CreateAsset(sm_AssetFileMd5, sm_fileMd5Path);
                AssetDatabase.SaveAssets();
            }
        }
        /// <summary>
        /// 递归设置打包
        /// </summary>
        /// <param name="package"></param>
        /// <param name="list"></param>
        /// <param name="m_PackageName"></param>
        /// <param name="zAsset"></param>
        private static void SetUpdateBundleRecursive(AssetPackageDesc package, List<AssetPackageDesc> list, List<string> m_PackageName, AssetBundelFileMd5 zAsset)
        {
            if (!m_PackageName.Contains(package.m_PackageName) && package.m_UpdateType != AssetBundelType.New && GetPackageAllFileMd5(package.m_PackageName, zAsset) != null)
            {
                package.m_UpdateType = AssetBundelType.Update;
                list.Add(package);
                m_PackageName.Add(package.m_PackageName);
            }
            for (int i = 0; i < package.m_DependPackage.Count; i++)
            {
                if (!list.Contains(sm_packageMapDic[package.m_DependPackage[i]]))
                {
                    SetUpdateBundleRecursive(sm_packageMapDic[package.m_DependPackage[i]], list, m_PackageName, zAsset);
                }
            }
        }
        private static void PrepareMd5OfAssets(AssetPackageDesc desc, PackageFileMd5 allFileMd5, Dictionary<string, string> zOldFileMd5, List<FileAssetInfo> zNewFileMd5)
        {
            foreach (var item in desc.m_DependAsset)
            {
                FileAssetInfo info = new FileAssetInfo();
                info.name = item.Key;
                info.md5 = item.Value;
                zNewFileMd5.Add(info);
            }
            if (allFileMd5 != null)
            {
                for (int j = 0; j < allFileMd5.zFileMd5.Count; j++)
                {
                    FileAssetInfo info = allFileMd5.zFileMd5[j];
                    zOldFileMd5.Add(info.name, info.md5);
                }
            }
        }
    }
}