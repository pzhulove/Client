using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
namespace AssetBundleTool
{
    public class StrategyByDirectoryLevel : StrategyBase
    {
        
        static protected string sm_OutputBundleExt = ".pck";
        public string[] sm_PackLevel = new string[]
         {
            "0","1","2","3","4","5"
         };
        
       
        public override void OnInit()
        {
            throw new System.NotImplementedException();
        }

        Vector2 m_displayPos = Vector2.zero;

        public override void OnDraw(Rect position,StrategyDataBase data)
        {
            if (data == null)
                return;
            StrategyByFileData m_selectStrategyData = (StrategyByFileData)data;
            if (m_selectStrategyData == null)
            {
                return;
            }
            EditorGUILayout.LabelField("PackStrategy   --->>>   " + m_selectStrategyData.strategyPath, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            m_displayPos = GUILayout.BeginScrollView(m_displayPos, GUILayout.Width(position.width * 0.45f), GUILayout.Height(position.height * 0.5f - 60));
            EditorGUILayout.LabelField("ExcludeFolder", EditorStyles.boldLabel);
            {
                if (m_selectStrategyData.excludePath != null)
                {
                    for (int i = 0; i < m_selectStrategyData.excludePath.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(m_selectStrategyData.excludePath[i]);
                        if (GUILayout.Button("Delete", GUILayout.Width(50)))
                        {
                            m_selectStrategyData.excludePath.RemoveAt(i);
                            i--;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                if (GUILayout.Button("AddExcludePath", GUILayout.Width(120), GUILayout.Height(20)))
                {
                    string path = EditorUtility.OpenFolderPanel("Exclude Assets Folder", Application.dataPath.Replace("/Assets", "") + "/" + m_selectStrategyData.strategyPath, "");
                    if (path != null && path.Contains((Application.dataPath + "/Resources")))
                    {
                        string tempPath = path.Substring(Application.dataPath.Length - 6);
                        if (!tempPath.Contains(m_selectStrategyData.strategyPath) || tempPath.Equals(m_selectStrategyData.strategyPath))
                        {
                            EditorUtility.DisplayDialog("Error", "该文件夹不属于子级目录", "确认");
                            return;
                        }
                        bool hasContain = false;
                        tempPath = tempPath.Replace('\\', '/');
                        if (!tempPath.EndsWith("/"))
                        {
                            tempPath += "/";
                        }
                        if (m_selectStrategyData.excludePath != null)
                        {
                            for (int i = 0; i < m_selectStrategyData.excludePath.Count; i++)
                            {
                                if (m_selectStrategyData.excludePath[i].Equals(tempPath))
                                {
                                    hasContain = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            m_selectStrategyData.excludePath = new List<string>();
                        }
                        if (!hasContain)
                        {
                            m_selectStrategyData.excludePath.Add(tempPath);
                        }
                    }
                }
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.Width(position.width * 0.15f), GUILayout.Height(position.height * 0.3f));

                    string[] includeOrNot = new string[] { "包含", "排除" };
                    m_selectStrategyData.m_IncludeMask = EditorGUILayout.Popup(m_selectStrategyData.m_IncludeMask, includeOrNot, GUILayout.Width(100));

                    bool[] includeBoolList = new bool[m_selectStrategyData.IncludeMask.Count];
                    for (int i = 0; i < includeBoolList.Length; i++)
                    {
                        string currentExt = m_selectStrategyData.IncludeMask[i].extString;
                        if (m_selectStrategyData.m_IncludeMask == 0)
                        {
                            includeBoolList[i] = EditorGUILayout.Toggle(currentExt, m_selectStrategyData.IncludeMask[i].extBool);
                            m_selectStrategyData.IncludeMask[i].extBool = includeBoolList[i];
                        }
                        else if (m_selectStrategyData.m_IncludeMask == 1)
                        {
                            includeBoolList[i] = EditorGUILayout.Toggle(currentExt, !m_selectStrategyData.IncludeMask[i].extBool);
                            m_selectStrategyData.IncludeMask[i].extBool = !includeBoolList[i];
                        }
                    }

                    EditorGUILayout.EndVertical();


                    GUILayout.Space(20);
                    GUILayout.BeginVertical();

                    m_selectStrategyData.packLevel = EditorGUILayout.Popup("打包层级", m_selectStrategyData.packLevel, sm_PackLevel, GUILayout.Width(300));
                    
                    GUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }



        public override void OnExcute(StrategyDataBase dataBase, Dictionary<long, List<AssetPackageDesc>> sm_strategyDataDic)
        {
            StrategyByFileData data = (StrategyByFileData)dataBase;
            _PackAssetByDirectoryLevel(data, data.strategyPath, sm_strategyDataDic);
        }

       
        /// <summary>
        /// 按照文件递归深度打包
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sourceDirectoryPath"></param>
        /// <param name="curLevel"></param>
        protected void _PackAssetByDirectoryLevel(StrategyByFileData data, string sourceDirectoryPath, Dictionary<long, List<AssetPackageDesc>> sm_strategyDataDic, int curLevel = 0)
        {
            sourceDirectoryPath = sourceDirectoryPath.Replace('\\', '/');
            if (!Directory.Exists(sourceDirectoryPath))
                return;

            List<AssetPackageDesc> packageDescList = null;
            if (sm_strategyDataDic.ContainsKey(data.strategyId))
            {
                packageDescList = sm_strategyDataDic[data.strategyId];
            }
            else
            {
                packageDescList = new List<AssetPackageDesc>();
                sm_strategyDataDic.Add(data.strategyId, packageDescList);
            }
            
            if (data.packLevel == curLevel)
            {
                _PackDirectory(packageDescList,data, sourceDirectoryPath, SearchOption.AllDirectories);
            }
            else
            {
                string[] directoryPath = Directory.GetDirectories(sourceDirectoryPath, "*.*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < directoryPath.Length; i++)
                {
                    _PackAssetByDirectoryLevel(data, directoryPath[i], sm_strategyDataDic, curLevel + 1);
                }
                _PackDirectory(packageDescList, data, sourceDirectoryPath, SearchOption.TopDirectoryOnly);
            }
        }
        
        /// <summary>
        /// 获得bundle名称
        /// </summary>
        /// <param name="folderNames"></param>
        /// <param name="extensionName"></param>
        /// <returns></returns>
        private  string GetValidBundleName(List<string> folderNames, string extensionName)
        {
            if (extensionName.Length < 90)
            {
                return extensionName;
            }
            string name = "";
            List<string> tempNames = new List<string>();
            if (folderNames.Count > 2)
            {
                string folderName = "";
                for (int i = 0; i < folderNames.Count; i++)
                {
                    folderName += folderNames[i];
                    if (i < folderNames.Count - 1)
                    {
                        folderName += "_";
                    }
                    if (i != folderNames.Count - 2)
                    {
                        name += folderNames[i];
                        tempNames.Add(folderNames[i]);
                        if (i < folderNames.Count - 1)
                        {
                            name += "_";
                        }
                    }
                }
                string tempName = name;
                name = tempName + extensionName.Replace(folderName, "");
                if (name.Length >= 90)
                {
                    GetValidBundleName(tempNames, name);
                }
            }
            return name;
        }

        /// <summary>
        /// 按目录记录包体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sourceDirectoryPath"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        protected  void _PackDirectory(List<AssetPackageDesc> packageDescList,StrategyByFileData data, string sourceDirectoryPath, SearchOption searchOption)
        {
            sourceDirectoryPath = sourceDirectoryPath.Replace('\\', '/');

            AssetPackageDesc newPackageDesc = new AssetPackageDesc();

            string[] assetPathList = Directory.GetFiles(sourceDirectoryPath, "*.*", searchOption);
            for (int i = 0; i < assetPathList.Length; ++i)
            {
                string curPath = assetPathList[i];
                if (!Tenmove.Runtime.Utility.File.Exists(curPath) )
                {
                    Debug.LogError("不存在或者文件名过长：" + curPath);
                    continue;
                }
                if (Path.GetExtension(curPath) == ".meta")
                    continue;
                curPath = curPath.Replace('\\', '/');
                if (data.ExcludePath(curPath))
                {
                    continue;
                }

                //if (data.Valid(Path.GetExtension(curPath)) && data.ValidLabel(curPath))
                if (data.Valid(Path.GetExtension(curPath)))
                {
                    
                    PackAssetDesc desc = new PackAssetDesc(curPath);
                    newPackageDesc.m_PackageAsset.Add(desc);
                    newPackageDesc.m_OriginSize += desc.m_OriginSize;
                    newPackageDesc.fileSet.Add(curPath);
                }
            }

            if ('/' == sourceDirectoryPath[sourceDirectoryPath.Length - 1])
                sourceDirectoryPath = sourceDirectoryPath.Remove(sourceDirectoryPath.Length - 1);

            string packName = "";
            string[] subPathSplit = sourceDirectoryPath.Split('/');
            List<string> tempPackName = new List<string>();
            for (int i = 0, icnt = subPathSplit.Length; i < icnt; ++i)
            {
                if (subPathSplit[i] == "Assets" || subPathSplit[i] == "Resources")
                    continue;

                packName += subPathSplit[i];
                tempPackName.Add(subPathSplit[i]);
                if (i + 1 < icnt)
                    packName += '_';
            }
            if (!string.IsNullOrEmpty(data.bundleExtensionName))
            {
                packName += '_' + data.bundleExtensionName;
            }
            packName += sm_OutputBundleExt;
            packName = GetValidBundleName(tempPackName, packName);

            newPackageDesc.m_PackageName = packName;
            if (newPackageDesc.m_PackageAsset.Count > 0)
                packageDescList.Add(newPackageDesc);
        }


    }
}
