using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace AssetBundleTool { 
    public class StrategyByFile : StrategyBase
    {
        
        static protected string sm_OutputBundleExt = ".pck";
       
        public string[] sm_extension = new string[] { };
        public override void OnInit()
        {
            throw new System.NotImplementedException();
        }
        Vector2 displayPos = Vector2.zero;
        public override void OnDraw(Rect position, StrategyDataBase data)
        {
             StrategyByFileData sm_selectStrategyData= (StrategyByFileData)data;
            if (sm_selectStrategyData == null)
            {
                return;
            }
            EditorGUILayout.LabelField("PackStrategy   --->>>   " + sm_selectStrategyData.strategyPath, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            displayPos=EditorGUILayout.BeginScrollView(displayPos, GUILayout.Width(position.width * 0.45f), GUILayout.Height(position.height * 0.5f - 60));
            EditorGUILayout.LabelField("ExcludeFolder", EditorStyles.boldLabel);
            if (sm_selectStrategyData.excludePath != null)
            {
                for (int i = 0; i < sm_selectStrategyData.excludePath.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(sm_selectStrategyData.excludePath[i]);
                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        sm_selectStrategyData.excludePath.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (GUILayout.Button("AddExcludePath", GUILayout.Width(120), GUILayout.Height(20)))
            {
                string path = EditorUtility.OpenFolderPanel("Exclude Assets Folder", Application.dataPath.Replace("/Assets", "") + "/" + sm_selectStrategyData.strategyPath, "");
                if (path != null && path.Contains((Application.dataPath + "/Resources")))
                {
                    string tempPath = path.Substring(Application.dataPath.Length - 6);
                    if (!tempPath.Contains(sm_selectStrategyData.strategyPath) || tempPath.Equals(sm_selectStrategyData.strategyPath))
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
                    if (sm_selectStrategyData.excludePath != null)
                    {
                        for (int i = 0; i < sm_selectStrategyData.excludePath.Count; i++)
                        {
                            if (sm_selectStrategyData.excludePath[i].Equals(tempPath))
                            {
                                hasContain = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        sm_selectStrategyData.excludePath = new List<string>();
                    }
                    if (!hasContain)
                    {
                        sm_selectStrategyData.excludePath.Add(tempPath);
                    }
                }
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.Width(position.width * 0.15f), GUILayout.Height(position.height * 0.3f));

            string[] includeOrNot = new string[] { "包含", "排除" };
            sm_selectStrategyData.m_IncludeMask = EditorGUILayout.Popup(sm_selectStrategyData.m_IncludeMask, includeOrNot, GUILayout.Width(100));

            bool[] includeBoolList = new bool[sm_selectStrategyData.IncludeMask.Count];
            for (int i = 0; i < includeBoolList.Length; i++)
            {
                string currentExt = sm_selectStrategyData.IncludeMask[i].extString;
                if (sm_selectStrategyData.m_IncludeMask == 0)
                {
                    includeBoolList[i] = EditorGUILayout.Toggle(currentExt, sm_selectStrategyData.IncludeMask[i].extBool);
                    sm_selectStrategyData.IncludeMask[i].extBool = includeBoolList[i];
                }
                else if (sm_selectStrategyData.m_IncludeMask == 1)
                {
                    includeBoolList[i] = EditorGUILayout.Toggle(currentExt, !sm_selectStrategyData.IncludeMask[i].extBool);
                    sm_selectStrategyData.IncludeMask[i].extBool = !includeBoolList[i];
                }
            }
            EditorGUILayout.EndVertical();


            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
           
        }

        

        public override void OnExcute(StrategyDataBase dataBase,Dictionary<long, List<AssetPackageDesc>> sm_strategyDataDic)
        {
            StrategyByFileData data = (StrategyByFileData)dataBase;
            PackAssetByFile(data, data.strategyPath, sm_strategyDataDic);
        }

       
        /// <summary>
        /// 按文件夹记录asset到描述
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sourceDirectoryPath"></param>
        private  void PackAssetByFile(StrategyByFileData data, string sourceDirectoryPath, Dictionary<long, List<AssetPackageDesc>> sm_strategyDataDic)
        {
            sourceDirectoryPath = sourceDirectoryPath.Replace('\\', '/');
            List<AssetPackageDesc> packageDescList = null;
            if (sm_strategyDataDic.ContainsKey(data.strategyId))
            {
                packageDescList = sm_strategyDataDic[data.strategyId];
                packageDescList.Clear();
            }
            else
            {
                packageDescList = new List<AssetPackageDesc>();
                sm_strategyDataDic.Add(data.strategyId, packageDescList);
            }
            if (!Directory.Exists(sourceDirectoryPath))
            {
                return;
            }

            string[] assetPathList = Directory.GetFiles(sourceDirectoryPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < assetPathList.Length; ++i)
            {
                string curPath = assetPathList[i];
                if (!Tenmove.Runtime.Utility.File.Exists(curPath))
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
                if (data.Valid(Path.GetExtension(curPath)))
                {
                    AssetPackageDesc newPackageDesc = new AssetPackageDesc();
                    PackAssetDesc desc = new PackAssetDesc(curPath);
                    newPackageDesc.m_PackageAsset.Add(desc);
                    newPackageDesc.m_OriginSize += desc.m_OriginSize;

                    if ('/' == sourceDirectoryPath[sourceDirectoryPath.Length - 1])
                        sourceDirectoryPath = sourceDirectoryPath.Remove(sourceDirectoryPath.Length - 1);

                    string packName = "";
                    string[] subPathSplit = sourceDirectoryPath.Split('/');
                    List<string> tempPackName = new List<string>();
                    for (int j = 0, icnt = subPathSplit.Length; j < icnt; ++j)
                    {
                        if (subPathSplit[j] == "Assets" || subPathSplit[j] == "Resources")
                            continue;

                        packName += subPathSplit[j];
                        tempPackName.Add(subPathSplit[j]);
                        if (j < icnt - 1)
                        {
                            packName += '_';
                        }
                    }

                    string tempName = packName;
                    packName += '_';
                    packName += Path.GetFileNameWithoutExtension(curPath);
                    packName += '_';
                    packName += Path.GetExtension(curPath).Replace(".", "");
                    packName += sm_OutputBundleExt;
                    packName = GetValidBundleName(tempPackName, packName);

                    newPackageDesc.m_PackageName = packName;
                    packageDescList.Add(newPackageDesc);

                }
            }
        }
        /// <summary>
        /// 获得bundle名称
        /// </summary>
        /// <param name="folderNames"></param>
        /// <param name="extensionName"></param>
        /// <returns></returns>
        private string GetValidBundleName(List<string> folderNames, string extensionName)
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
    }
}
