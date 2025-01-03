using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using Tenmove.Runtime.Unity;

namespace AssetBundleTool
{
    public static partial class BuildPackage
    {
        private static string sm_versionPath = "Assets/Resources/Base/Version2";
        private static string sm_treeDataPath = "Assets/Resources/Base/Version2/AssetTreeData.asset";
        private static string sm_checkFile = "Assets/StreamingAssets/AssetBundles/checkfile.lst";
        private static string sm_dumpFile = "/PackageInfoDumpNew.txt";
        private static string sm_treeDataJsonPath = "Assets/Resources/Base/Version2/AssetTreeData.json";

        private static string sm_fileMd5Path = "Assets/Scripts/Tenmove.Editor.Unity/Editor/Tools/AssetBundleTool/EditorAssetBundleFileMd5.asset";
        private static string sm_fileMd5PathJson = "Assets/Scripts/Tenmove.Editor.Unity/Editor/Tools/AssetBundleTool/AssetBundleFileMd5Json.json";

        private static AssetBundelFileMd5 sm_AssetFileMd5;
        private static Tenmove.Editor.Unity.AssetTreeDataBuilder sm_treeDataBuilder;
        //Bundel映射表
        public static Dictionary<string, AssetPackageDesc> sm_packageMapDic = new Dictionary<string, AssetPackageDesc>();//Bundel映射表;
        public static BuildData sm_UserData;
        //策略Bundel信息
        public static Dictionary<long, List<AssetPackageDesc>> sm_StrategyDataDic;
        public static Dictionary<string, OnAssetsProcess> sm_AssetProcess_Pre = new Dictionary<string, OnAssetsProcess>();
        public static Dictionary<string, OnAssetsProcess> sm_AssetProcess_Post = new Dictionary<string, OnAssetsProcess>();

        public delegate void OnAssetsProcess();
        public static OnAssetsProcess _PreActions, _PostActions;

        public static OnAssetsProcess m_Pre_Actor, m_Pre_Shader, m_Pre_Data, m_Pre_Scene, m_Pre_Effect, m_Pre_UI, m_Pre_Sound;
        public static OnAssetsProcess m_Post_Actor, m_Post_Shader, m_Post_Data, m_Post_Scene, m_Post_Effect, m_Post_UI, m_Post_Sound;

        /// <summary>
        /// 执行打包
        /// </summary>
        public static void _ExecuteBuild(bool incrementalPacking =true)
        {
            Debug.Log("[AB] -----yangdongfeng----AssetBundleManager---"+ incrementalPacking);           
            Debug.Log("[AB] CurData" + System.DateTime.Now);
            float t1 = Time.realtimeSinceStartup;


            //检测上次打包数据完整性，不完整则执行非增量打包
            if (!incrementalPacking || !CheckIncrementalDataIntegrality())
            {
                Debug.Log("[AB] No complete packaging data detected, all data will be cleared and non incremental packaging will be performed------------->>>>>>>>>>>>>");
                Debug.Log("[AB] CurData" + System.DateTime.Now);
                incrementalPacking = false;
                ClearBundleData();
            }
            float t2 = Time.realtimeSinceStartup - t1;
            Debug.Log("[AB] Test integrity time：" + t2 / 60);
            Debug.Log("[AB] CurData" + System.DateTime.Now);
            InitData();

            
            AssetBundleManifest assetManifestGrp = null;
            if (incrementalPacking)
            {
                Debug.Log("[AB]This time is  IncreseMentBuild：");
                IncreseMentBuild(assetManifestGrp);
            }
            else {
                Debug.Log("[AB]This time is  NonIncreseMentBuild：");
                NonIncreseMentBuild(assetManifestGrp);
            }
            _PostprocessActions();
        }

        #region 打包公共调用函数
        private static void InitData()
        {
            sm_treeDataBuilder = new Tenmove.Editor.Unity.AssetTreeDataBuilder(sm_treeDataPath);
            _PostActions += PostBuildAssetPackage;
            InitAssetBundelFileMd5();
        }

        

        private static void _PostprocessActions()
        {

        }

        private static void BuildPipelineExcute(AssetBundleManifest assetManifestGrp, List<AssetBundleBuild> builds)
        {
            float t7 = Time.realtimeSinceStartup;
            var buildsarray = builds.ToArray();
            _DumpBuildArray(buildsarray);
            Debug.LogFormat("[AB] build array count {0}", buildsarray.Length);
            if (!Directory.Exists(sm_UserData.m_OutputPath))
            {
                Directory.CreateDirectory(sm_UserData.m_OutputPath);
            }
            assetManifestGrp = BuildPipeline.BuildAssetBundles(sm_UserData.m_OutputPath, buildsarray, BuildView.sm_Opt, EditorUserBuildSettings.activeBuildTarget);
            if (assetManifestGrp == null)
            {
                Debug.Log("[AB] No pack AB");
                return;
            }

            float t8 = Time.realtimeSinceStartup - t7;
            Debug.Log("[AB] Perform packaging time：" + t8 / 60);
            Debug.Log("[AB] CurData" + System.DateTime.Now);
            if (sm_UserData.m_DisableEncryption)
            {
                BundleEncryption.Encryption(assetManifestGrp, sm_UserData.m_OutputPath);
            }
            AfterBuildPipeline(assetManifestGrp);
        }
        private static void AfterBuildPipeline(AssetBundleManifest assetManifestGrp)
        {
            Debug.Log("[AB] GeneratePackAfterLog " + System.DateTime.Now);//[AB] GeneratePackAfterLog 9/7/2020 3:43:10 PM
            //根据打包清单生成树结构
            RecordTreeData(sm_treeDataPath, assetManifestGrp);
            Debug.Log("[AB] RecordTreeData " + System.DateTime.Now);//[AB] RecordTreeData 9/7/2020 3:43:57 PM
            //保存zAsset
            RecordAssetFileMd5();
            Debug.Log("[AB] RecordAssetFileMd5 " + System.DateTime.Now);//[AB] RecordAssetFileMd5 9/7/2020 3:44:06 PM
            //记录用户设置AssetTreeData
            RecordAssetTreeData();
            Debug.Log("[AB] RecordAssetTreeData " + System.DateTime.Now);//[AB] RecordAssetTreeData 9/7/2020 3:44:49 PM
            //dumpAssetbundle信息
            Debug.Log("[AB] DumpstrategyDataDicInfo " + System.DateTime.Now);
            //打包后操作
            _PostActions();
            float totalTime = (Time.realtimeSinceStartup - startTime) / 60f;
            Debug.Log("[AB] Wrap up：" + System.DateTime.Now + "totalTime" + "__" + totalTime);//[AB] Wrap up：9/7/2020 3:44:57 PMtotalTime__19.31936
            //DeleteManifestAndMeta();
        }

        private static void _DumpBuildArray(IList<AssetBundleBuild> builds)
        {
            StringBuilder builder = new StringBuilder(2048*10);
            foreach (var cur in builds)
            {
                builder.Append("assetBundleName:");
                builder.Append(cur.assetBundleName);
                builder.Append("\n");

                builder.Append("assetBundleVariant:");
                builder.Append(cur.assetBundleVariant);
                builder.Append("\n");

                builder.Append("assetNames:[\n");
                foreach (var curass in cur.assetNames)
                {
                    builder.Append("  ");
                    builder.Append(curass);
                    builder.Append("\n");
                }
                builder.Append("]\n");
                
                builder.Append("addressableNames:[\n");
                foreach (var curaddr in cur.addressableNames)
                {
                    builder.Append("  ");
                    builder.Append(curaddr);
                    builder.Append("\n");
                }
                builder.Append("]\n");
            }
            var files = Directory.GetFiles(".");
            foreach (var item in files)
            {
                if (item.Contains("build-array-dump"))
                {
                    File.Delete(item);
                }
            }
            File.WriteAllText("./build-array-dump-"+System.DateTime.Now.Ticks+".txt", builder.ToString());
        }

        private static void DumpstrategyDataDicInfo()
        {
            string date = DateTime.Now.ToString("yyyy_MM_dd");
            FileStream streamW = new FileStream(sm_UserData.m_OutputPath+ sm_dumpFile, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));

            Dictionary<long, List<AssetPackageDesc>>.Enumerator it = sm_StrategyDataDic.GetEnumerator();
            int progress = 0;

            while (it.MoveNext())
            {
                progress++;

                List<AssetPackageDesc> list = it.Current.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    AssetPackageDesc desc = list[i];
                    EditorUtility.DisplayProgressBar("Ready to write packInfo", "writting packaging infos..." + desc.m_PackageName, (i + 0f) / list.Count);

                    sw.WriteLine(desc.m_PackageName);
                    for (int j = 0; j < desc.m_PackageAsset.Count; ++j)
                        sw.WriteLine("- Asset:" + desc.m_PackageAsset[j].m_AssetPath + "(" + desc.m_PackageAsset[j].m_AssetGUID + ")");

                    for (int j = 0; j < desc.m_DependPackage.Count; ++j)
                        sw.WriteLine("- Dependency:" + desc.m_DependPackage[j]);
                }
            }
            streamW.Flush();
            sw.Close();
            streamW.Close();
            EditorUtility.ClearProgressBar();
        }
        private static bool checkBuildSet()
        {
            BuildData old = _DeserializeBuildData();
            return sm_UserData.Equals(old);
        }
        private static void DumpTreeData(Tenmove.Runtime.Unity.TMUnityAssetTreeData data) {
            StringBuilder builder = new StringBuilder(2048 * 10);
            IEnumerable<Tenmove.Runtime.AssetDesc> descMaps =  data.GetAssetDescMap();
            foreach (var cur in descMaps)
            {
                builder.Append("assetName:");
                builder.Append(cur.AssetName);
                builder.Append("\n");


                builder.Append("assetPackId:");
                builder.Append(cur.AssetPackageID);
                builder.Append("\n");
            }
            IEnumerable<Tenmove.Runtime.AssetPackageDesc> descPacks = data.GetAssetPackageDescMap() ;
            foreach (var cur in descPacks)
            {
                builder.Append("assetPackName:");
                builder.Append(cur.PackageName);
                builder.Append("\n");

                builder.Append("assetPackId:");
                builder.Append(cur.PackageID);
                builder.Append("\n");

                builder.Append("assetPackDependencyPackageIDs:");
                builder.Append(cur.DependencyPackageIDs);
                builder.Append("\n");
            }
            File.WriteAllText("./build-array-dump-" + "NewBuild" + ".txt", builder.ToString());
        }

        private static void RecordTreeData(string treeDataPath, AssetBundleManifest assetManifestGrp)
        {
            TMUnityAssetTreeData data = sm_treeDataBuilder.Build(assetManifestGrp);

            if (File.Exists(treeDataPath))
            {
                File.Delete(treeDataPath);
            }

            Debug.Log("Package successful ------ > > > > > start to update configuration file");
            if (!Directory.Exists(sm_versionPath))
                Directory.CreateDirectory(sm_versionPath);
            AssetDatabase.CreateAsset(data, treeDataPath);

            DumpTreeData(data);

            if (File.Exists(sm_treeDataJsonPath))
            {
                File.Delete(sm_treeDataJsonPath);
            }


            FileStream fs1 = File.Create(sm_treeDataJsonPath);
            fs1.Close();
            string jsonData1 = JsonUtility.ToJson(data);
            File.WriteAllText(sm_treeDataJsonPath, jsonData1);
        }

        private static void RecordAssetFileMd5()
        {
            Debug.Log("Asset.packFileMd5List.Count:  --->>>   " + sm_AssetFileMd5.packFileMd5List.Count);

            for (int i = 0; i < sm_AssetFileMd5.packFileMd5List.Count; i++)
            {
                PackageFileMd5 md5 = sm_AssetFileMd5.packFileMd5List[i];
                string path = Path.Combine(sm_UserData.m_OutputPath, md5.zPackageName);
                if (File.Exists(path))
                {
                    md5.bundleMd5 = FileUtil.GetFileMD5(path);
                }
            }
            EditorUtility.SetDirty(sm_AssetFileMd5);
            AssetDatabase.SaveAssets();

            if (File.Exists(sm_fileMd5PathJson))
            {
                File.Delete(sm_fileMd5PathJson);
            }
            FileStream fs = File.Create(sm_fileMd5PathJson);
            fs.Close();
            string jsonData = JsonUtility.ToJson(sm_AssetFileMd5);
            File.WriteAllText(sm_fileMd5PathJson, jsonData);
        }

        private static void RecordAssetTreeData()
        {
            AssetBundleBuild[] bundlePackageInfoBuild = new AssetBundleBuild[1];
            bundlePackageInfoBuild[0].assetBundleName = "packageinfo.pak";
            bundlePackageInfoBuild[0].assetNames = new string[1] { sm_treeDataPath };
            bundlePackageInfoBuild[0].assetBundleVariant = "";
            AssetBundleManifest assetManifestPackageInfo = BuildPipeline.BuildAssetBundles(sm_UserData.m_OutputPath, bundlePackageInfoBuild, BuildView.sm_Opt, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            sm_treeDataBuilder = null;
        }

        
        private static void DeleteManifestAndMeta() {
            var directory = new System.IO.DirectoryInfo(sm_UserData.m_OutputPath);
            var files = directory.GetFiles("*.manifest" , System.IO.SearchOption.AllDirectories);
            foreach (var f in files) {
                File.Delete(f.ToString());
            }
            var files2 = directory.GetFiles("*.manifest.meta", System.IO.SearchOption.AllDirectories);
            foreach (var f in files2)
            {
                File.Delete(f.ToString());
            }
        }
        

        private static void BuildDetail(AssetPackageDesc buildPackageDesc, out AssetPackageDesc desc, out AssetBundleBuild build, out List<string> packageAssets)
        {
            desc = buildPackageDesc;
            build = new AssetBundleBuild();
            packageAssets = new List<string>();
            for (int k = 0; k < desc.m_PackageAsset.Count; k++)
            {
                packageAssets.Add(desc.m_PackageAsset[k].m_AssetPath);
            }
            build.assetBundleName = desc.m_PackageName;
            build.assetBundleVariant = "";
            build.assetNames = new string[desc.m_PackageAsset.Count];
            build.addressableNames = new string[desc.m_PackageAsset.Count];
            //记录包名和对应的地址
            for (int j = 0; j < desc.m_PackageAsset.Count; ++j)
            {
                build.assetNames[j] = desc.m_PackageAsset[j].m_AssetPath;

                if (!string.IsNullOrEmpty(build.assetNames[j]))
                {
                    //addressableName去掉后缀，对应解包去后缀
                    int extentionAt = desc.m_PackageAsset[j].m_AssetPath.LastIndexOf(".");
                    string path = desc.m_PackageAsset[j].m_AssetPath.Remove(extentionAt);
                    build.addressableNames[j] = path.Replace("Assets/Resources/", null);
                }
            }
        }

        /// <summary>
        /// 获取存储在本地的上次打包的Bundel包的MD5信息
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="zAsset"></param>
        /// <returns></returns>
        private static PackageFileMd5 GetPackageAllFileMd5(string packageName, AssetBundelFileMd5 zAsset)
        {
            for (int i = 0; i < zAsset.packFileMd5List.Count; i++)
            {
                if (packageName.Equals(zAsset.packFileMd5List[i].zPackageName))
                {
                    return zAsset.packFileMd5List[i];
                }
            }
            return null;
        }
        private static void _BreakAssetLinks()
        {
            // 里面有对Scene目录下材质的引用，runtime也在用，暂时不清理。
            //_BreakSceneAssetLink();

            // 目前没有发现直接资源引用
            //_BreakDungeonAssetLink();

            _BreakSkillAssetLink();
        }

        /// <summary>
        /// 技能配置文件断链
        /// </summary>
        static void _BreakSkillAssetLink()
        {
            string[] assetPathList = Directory.GetFiles("Assets/Resources/Data", "*.asset", SearchOption.AllDirectories);
            for (int i = 0, icnt = assetPathList.Length; i < icnt; ++i)
            {
                string path = assetPathList[i];
                DSkillData data = AssetDatabase.LoadAssetAtPath<DSkillData>(path);

                if (null == data) continue;

                data.goHitEffect = null;
                data.goHitEffectAsset.m_AssetObj = null;

                data.goSFX = null;
                data.goSFXAsset.m_AssetObj = null;
                data.characterPrefab = null;
                data.characterAsset.m_AssetObj = null;

                if (null != data.attachFrames)
                {
                    for (int j = 0, jcnt = data.attachFrames.Length; j < jcnt; ++j)
                    {
                        EntityAttachFrames curFrame = data.attachFrames[j];
                        curFrame.entityPrefab = null;
                        curFrame.entityAsset.m_AssetObj = null;
                        //curFrame.avatarPartInfoData = null;
                        //curFrame.avatarAssetName = string.Empty;
                    }
                }

                if (null != data.effectFrames)
                {
                    for (int j = 0, jcnt = data.effectFrames.Length; j < jcnt; ++j)
                    {
                        EffectsFrames curFrame = data.effectFrames[j];
                        curFrame.effectGameObjectPrefeb = null;
                        curFrame.effectAsset.m_AssetObj = null;
                    }
                }

                if (null != data.entityFrames)
                {
                    for (int j = 0, jcnt = data.entityFrames.Length; j < jcnt; ++j)
                    {
                        EntityFrames curFrame = data.entityFrames[j];
                        curFrame.entityPrefab = null;
                        curFrame.entityAsset.m_AssetObj = null;
                    }
                }

                if (null != data.sfx)
                {
                    for (int j = 0, jcnt = data.sfx.Length; j < jcnt; ++j)
                    {
                        DSkillSfx curFrame = data.sfx[j];
                        curFrame.soundClip = null;
                        curFrame.soundClipAsset.m_AssetObj = null;
                    }
                }

                EditorUtility.SetDirty(data);
            }
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// 检测上次打包数据完整性，不完整则执行非增量打包
        /// </summary>
        /// <returns></returns>
        private static bool CheckIncrementalDataIntegrality()
        {
            //加载treedata
            TMUnityAssetTreeData assetTreeData = GetAssetTreeDataFromBundle();
            if (assetTreeData == null)
            {
                return false;
            }
            if (File.Exists(sm_treeDataPath))
            {
                File.Delete(sm_treeDataPath);
            }
            TMUnityAssetTreeData tmAssetTreeData = ScriptableObject.CreateInstance<TMUnityAssetTreeData>();
            tmAssetTreeData.Fill(assetTreeData.GetAssetDescMap(), assetTreeData.GetAssetPackageDescMap());
            if (!Directory.Exists(sm_versionPath))
                Directory.CreateDirectory(sm_versionPath);
            AssetDatabase.CreateAsset(tmAssetTreeData, sm_treeDataPath);
            AssetDatabase.Refresh();
            
            AssetBundelFileMd5 md5File = AssetDatabase.LoadAssetAtPath<AssetBundelFileMd5>(sm_fileMd5Path);
            if (md5File == null)
            {
                return false;
            }
            string[] allBundleFileArray = Directory.GetFiles(sm_UserData.m_OutputPath, "*.pck", SearchOption.AllDirectories);
            if (allBundleFileArray.Length != md5File.packFileMd5List.Count)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 加载treedata
        /// </summary>
        /// <returns></returns>
        private static TMUnityAssetTreeData GetAssetTreeDataFromBundle()
        {
            string assetBundlePath = sm_UserData.m_OutputPath;
            TMUnityAssetTreeData assetTreeData = null;
            string bundleName = "packageinfo.pak";
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(assetBundlePath, bundleName));
            if (assetBundle == null)
            {
                Debug.Log("Can't find AssetBundles  :" + bundleName);
                return null;
            }
            assetTreeData = assetBundle.LoadAsset<TMUnityAssetTreeData>("AssetTreeData.asset");
            if (assetTreeData == null)
            {
                assetBundle.Unload(true);
                Debug.Log("Can't find TMUnityAssetTreeData  :" + assetTreeData);
                return null;
            }
            assetBundle.Unload(false);
            return assetTreeData;
        }

        /// <summary>
        /// 清理打包缓存和校验信息和以前的bundle包和树结构体
        /// </summary>
        [MenuItem("[打包工具]/ClearBundleData")]
        public static void ClearBundleData()
        {

            if (File.Exists(sm_fileMd5Path))
            {
                AssetDatabase.DeleteAsset(sm_fileMd5Path);
            }
            if (File.Exists(sm_treeDataPath))
            {
                File.Delete(sm_treeDataPath);
            }
            if (Directory.Exists(sm_UserData.m_OutputPath))
            {
                Directory.Delete(sm_UserData.m_OutputPath, true);
                Directory.CreateDirectory(sm_UserData.m_OutputPath);
            }
            if (File.Exists(sm_treeDataJsonPath))
            {
                File.Delete(sm_treeDataJsonPath);
            }
            if (File.Exists(sm_fileMd5PathJson))
            {
                File.Delete(sm_fileMd5PathJson);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        #endregion
    }
}
