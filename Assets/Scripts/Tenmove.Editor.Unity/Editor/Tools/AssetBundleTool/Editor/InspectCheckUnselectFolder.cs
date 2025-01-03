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
    public class InspectCheckUnselectFolder
    {
        private Rect m_RectPos;
        private InspectTreeView inspectTreeView;
        private List<StrategyDataBase> strategyDataList;//策略列表
        private List<string> allFolderInsideResources = new List<string>();
        private List<string> allFolderInsideStrategy = new List<string>();
        private List<string> unSelectFolders = new List<string>();
        private string dataPath = "";
        private int buttonHeight = 25;
        private int offest;
        public void OnEnable(Rect pos, List<StrategyDataBase> strategyDataList, InspectTreeView inspectTreeView)
        {
            m_RectPos = pos;
            offest = ((int)InspectModel.End + 1) / 3;
            dataPath = Application.dataPath;
            this.strategyDataList = strategyDataList;
            this.inspectTreeView = inspectTreeView;
            allFolderInsideResources.Clear();
            allFolderInsideStrategy.Clear();
            unSelectFolders.Clear();
            string[] resourcesFolder = Directory.GetDirectories("Assets/Resources", "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < resourcesFolder.Length; i++)
            {
                string str = resourcesFolder[i].Replace("\\", "/");
                EditorUtility.DisplayProgressBar("正在遍历Resources文件夹", "遍历中", i * 1.0f / resourcesFolder.Length);
                if (!str.Contains("Assets/Resources/Base"))
                {
                    allFolderInsideResources.Add(str);
                }
            }
            EditorUtility.ClearProgressBar();
            for (int i = 0; i < strategyDataList.Count; i++)
            {
                if (!Directory.Exists(strategyDataList[i].strategyPath))
                    continue;
                if (!allFolderInsideStrategy.Contains(GetNormalizedPath(strategyDataList[i].strategyPath)))
                {
                    allFolderInsideStrategy.Add(GetNormalizedPath(strategyDataList[i].strategyPath));
                }
                string[] strategyFolders = Directory.GetDirectories(strategyDataList[i].strategyPath, "*.*", SearchOption.AllDirectories);
                EditorUtility.DisplayProgressBar("正在遍历策略文件夹", "遍历中", i * 1.0f / strategyDataList.Count);
                for (int j = 0; j < strategyFolders.Length; j++)
                {
                    if (!allFolderInsideStrategy.Contains(GetNormalizedPath(strategyFolders[j])))
                    {
                        allFolderInsideStrategy.Add(GetNormalizedPath(strategyFolders[j]));
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            for (int i = 0; i < allFolderInsideResources.Count; i++)
            {
                if (!allFolderInsideStrategy.Contains(allFolderInsideResources[i]))
                {
                    unSelectFolders.Add(allFolderInsideResources[i]);
                }
            }
            List<InspectTreeItem> treeViewItems = new List<InspectTreeItem>();
            for (int i = 0; i < unSelectFolders.Count; i++)
            {
                InspectTreeItem item = new InspectTreeItem(unSelectFolders[i], 0);
                treeViewItems.Add(item);
            }
            inspectTreeView.SetTreeViewData(treeViewItems, SetClickPath);
        }
        private string GetNormalizedPath(string path)
        {
            return path.Replace("\\", "/");
        }
        public void OnGUI(Rect pos)
        {
            m_RectPos = pos;
            if (unSelectFolders.Count > 0)
            {
                inspectTreeView.OnGUI(new Rect(m_RectPos.x, m_RectPos.y + buttonHeight * offest, m_RectPos.width, m_RectPos.height - buttonHeight * offest));
            }
        }
        public void SetClickPath(string path)
        {

        }
    }
}
