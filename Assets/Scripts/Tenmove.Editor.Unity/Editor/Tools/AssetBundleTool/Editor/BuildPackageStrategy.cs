using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AssetBundleTool
{
    public static partial class BuildPackage
    {
        public static List<StrategyDataBase> sm_strategyDataList = new List<StrategyDataBase>();//策略列表
        private static List<string> sm_allPath=new List<string>(); //resource下的所有文件夹(除了base)
        private static List<string> sm_allStrategtPath=new List<string>(); //resource下的所有文件夹(除了base)

        public static void InitStrategy()
        {
            sm_StrategyDataDic = new Dictionary<long, List<AssetPackageDesc>>();

            //加载策略文件
            string sm_strategyPath = StrategyView.sm_strategyPath;//策略XML存储位置

            //编辑器模式下初始化过不用初始化，直接返回
            if (sm_strategyDataList == null)
            {
                sm_strategyDataList = new List<StrategyDataBase>();
            }
            if ( sm_strategyDataList.Count != 0)
            {
                return;
            }

            //非编辑器模式下需要初始化
            sm_strategyDataList = SmallFunc.FromXmlString(sm_strategyPath, typeof(List<StrategyDataBase>)) as List<StrategyDataBase>;
            if (sm_strategyDataList == null)
            {
                sm_strategyDataList = new List<StrategyDataBase>();
            }
            _CheckNotIncludePath();

        }

       
        public static void InitBuildData()
        {
            sm_UserData = _DeserializeBuildData();
        }
        public static void SaveBuildData()
        {
            var dataPath = Application.dataPath + "/Scripts/Tenmove.Editor.Unity/Editor/Tools/AssetBundleTool/AssetBundleBuild.xml";

            if (sm_UserData != null)
            {
                SmallFunc.CreateXML(sm_UserData, dataPath);
            }
        }
        public static float startTime = 0;
        private static void _PreprocessActions()
        {
            SceneSprAtlasPacker.UpdateAllSpriteAtlasForScene();
        }
        public static void IncrementBuildBundle(bool incrementBuildBundle = false)
        {
            Debug.Log("[AB] StartBuild：" + System.DateTime.Now);
            startTime = Time.realtimeSinceStartup;
            _PreprocessActions();

            try
            {
                InitStrategy();
                
                if (sm_UserData == null)
                {
                    InitBuildData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("InitialFailure：" + e.Message);
            }
            _writeStrategyPath_Assets(incrementBuildBundle);
        }
        
        private static void _CheckNotIncludePath()
        {
            //1.遍历resource下除了base的所有子目录

            string rootPath = Application.dataPath + "/Resources";
            _ListDirectory(rootPath, sm_allPath, true);
            //2.排除base
            sm_allPath.Remove("Assets/Resources/Base");
            foreach (var i in sm_strategyDataList)
            {
                if (!sm_allStrategtPath.Contains(i.strategyPath))
                {
                    sm_allStrategtPath.Add(i.strategyPath);
                }
            }
            //3.删掉allpath中存在的策略路径，加入排除的和未添加的
            _LoopAddNotInclude();
            foreach (var i in sm_allPath)
            {
                StrategyView.CheckAndAddPath(i);
            }
            foreach (var i in sm_strategyDataList)
            {
                i.Init();
            }
            StrategyView.SaveStrategy();
        }

        private static void _LoopAddNotInclude()
        {
            int currentCountCheck = sm_allStrategtPath.Count;
            for (int i =0;i<sm_strategyDataList.Count;i++)
            {
                string strategyPath = sm_strategyDataList[i].strategyPath;
                if (sm_allPath.Contains(strategyPath))
                {
                    sm_allPath.Remove(strategyPath);
                    sm_allStrategtPath.Remove(strategyPath);
                    if (sm_strategyDataList[i].excludePath.Count != 0)
                    {
                        foreach (var k in sm_strategyDataList[i].excludePath) {
                            if (!sm_allPath.Contains(k))
                            {
                                string  temp = k.Substring(0, k.Length - 1);
                                sm_allPath.Add(temp);
                            }
                        }
                    }
                }
            }
            if (sm_allStrategtPath.Count == currentCountCheck && sm_allStrategtPath.Count != 0)
            {
                List<string> tempAllPath = new List<string>();
                for (int i = 0; i < sm_allPath.Count; i++)
                {
                    string tempPath = sm_allPath[i];
                    bool continueExpand = false;
                    foreach (var j in sm_allStrategtPath)
                    {
                        if (j.Contains(tempPath))
                            continueExpand = true;
                    }
                    if (continueExpand)
                    {
                        string path = Application.dataPath + tempPath.Substring(6);
                        _ListDirectory(path, tempAllPath, true);
                        sm_allPath.Remove(tempPath);
                    }
                }
                sm_allPath.AddRange(tempAllPath);
            }
            if (sm_allStrategtPath.Count != 0)
            {
                _LoopAddNotInclude();
            }
            return;
        }

        private static BuildData _DeserializeBuildData()
        {
            var dataPath = Application.dataPath + "/Tools/AssetBundleTool/AssetBundleBuild.xml";
            if (!File.Exists(dataPath))
            {
                return new BuildData();
            }
            BuildData data = SmallFunc.FromXmlString(dataPath, typeof(BuildData)) as BuildData;
            if (data == null)
            {
                return new BuildData();
            }
            else
            {
                return data;
            }
        }

        /// <summary>
        /// 按路径记录bundle包以及其中依赖的资源
        /// </summary>
        /// <returns></returns>
        private static void _writeStrategyPath_Assets(bool incrementBuildBundle = true)
        {
            sm_packageMapDic.Clear();
            sm_StrategyDataDic.Clear();
            float t1 = Time.realtimeSinceStartup;
            Debug.Log("[AB] start Collect strategy " + System.DateTime.Now);
            //foreach (var i in sm_UserData.m_AssetTypes)
            //{
            //    if (sm_AssetProcess_Pre[i]!=null)
            //    {
            //        Debug.Log(i);
            //        sm_AssetProcess_Pre[i]();
            //    }
            //}
            EditorUtility.DisplayProgressBar("准备", "shaderMap生成，技能断链", 0.5f);
            SmallFunc.BuildShaderMap();
            ScriptPacker.TransScriptData();
            _BreakAssetLinks();
            EditorUtility.ClearProgressBar();

            Debug.Log("[AB] end Process before " + System.DateTime.Now);
            try
            {
                for (int i = 0; i < StrategyView.sm_strategyDataList.Count; i++)
                {
                    if (!StrategyView.sm_strategyDataList[i].enable)
                    {
                        continue;
                    }
                    EditorUtility.DisplayProgressBar("ParkStrategy", "..."+ StrategyView.sm_strategyDataList[i].strategyPath, ((float)i /StrategyView.sm_strategyDataList.Count));
                    var tempStrategy = StrategyView.CreateInstance<StrategyBase>(StrategyView.sm_strategyDataList[i].strateggy.m_TypeName);
                    tempStrategy.OnExcute(StrategyView.sm_strategyDataList[i], sm_StrategyDataDic);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[AB] Collect strategy failure:" + e.Message);
                Debug.Log("[AB] CurData:" + System.DateTime.Now);
                throw;
            }
            float t2 = Time.realtimeSinceStartup - t1;
            Debug.Log("[AB] Pack strategy cost time：" + t2 / 60);
            Debug.Log("[AB] CurData" + System.DateTime.Now);
            EditorUtility.ClearProgressBar();
            if (sm_UserData.m_Compression == CompressOptions.Uncompressed)
                BuildView.sm_Opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
            else if (sm_UserData.m_Compression == CompressOptions.ChunkBasedCompression)
                BuildView.sm_Opt |= BuildAssetBundleOptions.ChunkBasedCompression;
            if (sm_UserData.m_DisableWriteTypeTree)
                BuildView.sm_Opt |= BuildAssetBundleOptions.DisableWriteTypeTree;
            
            if (_RecordBundle())
            {
                _ExecuteBuild(incrementBuildBundle);
            }

        }
        // AssetPath --> PackageName的映射
        private static bool _RecordBundle()
        {
            Debug.Log("StartBuild");
            float t1 = Time.realtimeSinceStartup;
            Dictionary<string, string> bundelFileMap = new Dictionary<string, string>();


            foreach (var item in sm_StrategyDataDic)
            {
                List<AssetPackageDesc> desc = item.Value;
                for (int i = 0; i < desc.Count; i++)
                {
                    EditorUtility.DisplayProgressBar(" OutFolderAssetPath --> PackageName Mapping", desc[i].m_PackageName, (i + 0f) / desc.Count);

                    if (!sm_packageMapDic.ContainsKey(desc[i].m_PackageName))
                    {
                        sm_packageMapDic.Add(desc[i].m_PackageName, desc[i]);
                        for (int j = 0; j < desc[i].m_PackageAsset.Count; j++)
                        {
                            bundelFileMap.Add(desc[i].m_PackageAsset[j].m_AssetPath, desc[i].m_PackageName);
                        }
                    }
                }
            }
            float t2 = Time.realtimeSinceStartup - t1;
            Debug.Log(" AssetPath --> PackageName Mapping cost Time：" + t2 / 60);
            EditorUtility.ClearProgressBar();
            float t3 = Time.realtimeSinceStartup;
            int progress = 0;
            foreach (var item in sm_StrategyDataDic)
            {
                List<AssetPackageDesc> descs = item.Value;
                progress++;
                for (int i = 0; i < descs.Count; i++)
                {
                    AssetPackageDesc desc = descs[i];
                    // 获取该Package依赖的不属于该包的资源
                    _FindDepend(desc);
                    List<string> dependFiles = desc.m_DependAssetExceptPackage;
                    EditorUtility.DisplayProgressBar("set依赖信息", descs[i].m_PackageName+progress+"----"+sm_StrategyDataDic.Count+"#"+ i+"----" + descs.Count, (i + 0f) / descs.Count);
                    // 统计Package依赖的Package
                    for (int j = 0; j < dependFiles.Count; j++)
                    {
                        if (bundelFileMap.ContainsKey(dependFiles[j]))
                        {
                            if (!desc.m_DependPackage.Contains(bundelFileMap[dependFiles[j]]))
                            {
                                desc.m_DependPackage.Add(bundelFileMap[dependFiles[j]]);
                            }
                        }
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            float t4 = Time.realtimeSinceStartup - t3;
            Debug.Log("cost time ：" + t4 / 60);
            Debug.Log("end collect");
            return true;
        }

        private static void _FindDepend(AssetPackageDesc desc)
        {
            desc.m_DependAssetExceptPackage.Clear();
            Dictionary<string, string> packsDic = new Dictionary<string, string>();
            for (int i = 0; i < desc.m_PackageAsset.Count; i++)
            {
                var packasset = desc.m_PackageAsset[i];
                packsDic.Add(packasset.m_AssetPath, packasset.m_AssetGUID);
            }
            var it = packsDic.GetEnumerator();
            while (it.MoveNext())
            {
                string path = it.Current.Key;
                string[] depends = AssetDatabase.GetDependencies(path);
                for (int i = 0; i < depends.Length; i++)
                {
                    if (!packsDic.ContainsKey(depends[i]) && Path.GetExtension(depends[i]).ToLower() != ".cs")
                    {
                        desc.m_DependAssetExceptPackage.Add(depends[i]);
                    }
                    if (!desc.m_DependAsset.ContainsKey(depends[i]) && Path.GetExtension(depends[i]).ToLower() != ".cs")
                    {
                        string s = FileUtil.GetFileMD5(depends[i]);
                        string s1 = FileUtil.GetFileMD5(depends[i] + ".meta");
                        desc.m_DependAsset.Add(depends[i],s+s1);
                    }
                }
            }
        }

        /// <summary>
        /// 列出path路径对应的文件夹中的子文件夹和文件
        /// 然后再递归列出子文件夹内的文件和文件夹
        /// </summary>
        /// <param name="path">需要列出内容的文件夹的路径</param>
        private static void _ListDirectory(string path,List<string> childrenPaths,bool moveHead=false)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(path);
            foreach (string subdirectory in subdirectoryEntries)
            {
                string temp = subdirectory;
                if (moveHead)
                {
                    temp = temp.Substring(Application.dataPath.Length - 6);
                    temp = temp.Replace(@"\", @"/");
                }
                if (!childrenPaths.Contains(temp))
                {
                    childrenPaths.Add(temp);
                }
            }
        }
    }
}
