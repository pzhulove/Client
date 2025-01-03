using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.IMGUI.Controls;
using System.Linq;
namespace AssetBundleTool
{
    public enum InspectModel
    {
        CheckRedundancy,
        CheckUnselectFolder,
        CheckDependence,
        End,
    }
    public static class InspectView
    {
        private static InspectModel inspectModel;
        private static Rect sm_RectPos;
        private static string sm_assetBundlePath {
            get {
                return BuildView.sm_UserData.m_OutputPath;
            }
        }
        private static List<StrategyDataBase> sm_strategyDataList {
            get {
                return StrategyView.sm_strategyDataList;
            }
        }

        private static TreeViewState inspectTreeViewState;
        private static InspectTreeView inspectTreeView;
        private static SearchField m_SearchField;
        private static InspectCheckRedundancy inspectCheckRedundancy;
        private static InspectCheckUnselectFolder inspectCheckUnselectFolder;
        private static InspectCheckDependence inspectCheckDependence;

        //显示策略
        public static void OnInit()
        {
            if (inspectTreeViewState == null)
                inspectTreeViewState = new TreeViewState();
            inspectTreeView = new InspectTreeView(inspectTreeViewState);
            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += inspectTreeView.SetFocusAndEnsureSelectedItem;
            inspectCheckRedundancy = inspectCheckRedundancy == null ? new InspectCheckRedundancy() : inspectCheckRedundancy;
            inspectCheckUnselectFolder = inspectCheckUnselectFolder == null ? new InspectCheckUnselectFolder() : inspectCheckUnselectFolder;
            inspectCheckDependence = inspectCheckDependence == null ? new InspectCheckDependence() : inspectCheckDependence;
        }

        public static void OnDraw(Rect pos)
        {
            Rect m_RectPos = new Rect(0, 20, pos.width, pos.height - 20);
            Color color = GUI.backgroundColor;
            GUI.backgroundColor = Color.cyan;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检查Bundle资源冗余", EditorStyles.miniButtonMid, GUILayout.Height(24)))
            {
                inspectModel = InspectModel.CheckRedundancy;
                inspectCheckRedundancy.OnEnable(pos, sm_assetBundlePath, inspectTreeView);
            }
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("检查未添加策略文件夹", EditorStyles.miniButtonMid, GUILayout.Height(24)))
            {
                inspectModel = InspectModel.CheckUnselectFolder;
                inspectCheckUnselectFolder.OnEnable(pos, sm_strategyDataList, inspectTreeView);
            }
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("检查依赖信息", EditorStyles.miniButtonMid, GUILayout.Height(24)))
            {
                inspectModel = InspectModel.CheckDependence;
                //inspectCheckDependence.OnEnable(pos, assetBundlePath, inspectTreeView);
                AbDependShowWindow.AbDependShow();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = color;
            inspectTreeView.searchString = m_SearchField.OnToolbarGUI(inspectTreeView.searchString);
            switch (inspectModel)
            {
                case InspectModel.CheckRedundancy:
                    inspectCheckRedundancy.OnGUI(m_RectPos);
                    break;
                case InspectModel.CheckUnselectFolder:
                    inspectCheckUnselectFolder.OnGUI(m_RectPos);
                    break;
                case InspectModel.CheckDependence:
                    inspectCheckDependence.OnGUI(m_RectPos);
                    break;
                default:
                    break;
            }
        }
        public static void OnLeave()
        {
            if (inspectCheckDependence != null)
                inspectCheckDependence.OnDestroy();
            if (inspectCheckRedundancy != null)
                inspectCheckRedundancy.OnDestroy();
        }
    }
}