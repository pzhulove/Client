using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetBundleTool
{
    public static class BuildView
    {
        public static Dictionary<long, List<AssetPackageDesc>> sm_StrategyDataDic {
            get {
                return BuildPackage.sm_StrategyDataDic;
            }
        }
        public static BuildData sm_UserData {
            get {
                return BuildPackage.sm_UserData;
            }
        }
        public static BuildAssetBundleOptions sm_Opt = BuildAssetBundleOptions.None;
        private static bool sm_IsSelect = false;
        public static void OnInit() {
            BuildPackage.InitBuildData();
            //BuildPackage.InitAssetProcesses();
        }
        /// <summary>
        /// 设定打包版面的ui
        /// </summary>
        public static void OnDraw(Rect position)
        {
            EditorGUILayout.Space();
            sm_IsSelect = EditorGUILayout.BeginToggleGroup("Advanced", sm_IsSelect);
            GUILayout.BeginHorizontal();
            sm_UserData.m_OutputPath = EditorGUILayout.TextField("Output Path", sm_UserData.m_OutputPath);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
            {
                sm_UserData.m_OutputPath = EditorUtility.OpenFolderPanel("Bundle Folder", sm_UserData.m_OutputPath, string.Empty);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUIContent[] m_CompressionOptions =
            {
                   new GUIContent("No Compression"),
                   new GUIContent("Standard Compression (LZMA)"),
                   new GUIContent("Chunk Based Compression (LZ4)")
            };
            int[] m_CompressionValues = { 0, 1, 2 };
            sm_UserData.m_Compression = (CompressOptions)EditorGUILayout.IntPopup(
                                 new GUIContent("Compression"),
                                 (int)sm_UserData.m_Compression,
                                 m_CompressionOptions,
                                 m_CompressionValues);

            sm_UserData.m_DisableWriteTypeTree = EditorGUILayout.Toggle("DisableWriteTypeTree", sm_UserData.m_DisableWriteTypeTree);
            
            sm_UserData.m_DisableEncryption = EditorGUILayout.Toggle("加密", sm_UserData.m_DisableEncryption);
            

            smBuildIncreasement = EditorGUILayout.Toggle("增量打包", smBuildIncreasement);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //EditorGUILayout.LabelField("选择资源打包输出类型----------");
            //foreach (int v in Enum.GetValues(typeof(EAssetType)))
            //{
            //    string strName = Enum.GetName(typeof(EAssetType), v);
            //    bool ifHasType= EditorGUILayout.Toggle(strName, sm_UserData.m_AssetTypes.Contains(strName));
            //    if (ifHasType && !sm_UserData.m_AssetTypes.Contains(strName))
            //    {
            //        sm_UserData.m_AssetTypes.Add(strName);
            //    }
            //    else if (!ifHasType && sm_UserData.m_AssetTypes.Contains(strName))
            //    {
            //        sm_UserData.m_AssetTypes.Remove(strName);
            //    }
            //}
            EditorGUILayout.EndToggleGroup();
            if (GUILayout.Button("Build"))
            {
                Build();
            }

        }
        public static bool smBuildIncreasement=false;
        private static void Build()
        {
           BuildPackage.IncrementBuildBundle(smBuildIncreasement);
        }

       

        public static void OnLeave()
        {
            BuildPackage.SaveBuildData();
        }

       

    }
}