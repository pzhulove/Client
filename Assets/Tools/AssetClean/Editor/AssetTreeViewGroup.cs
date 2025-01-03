using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.IO;

namespace CleanAssetsTool
{
    public class AssetTreeViewGroup
    {
        public CleanAssetSearchField SearchField;
        public AssetTreeView TreeView;
        public bool TreeViewValid;
        public string TreeViewName;
        public AssetFilter assetFilter;
        public int id;
        Action<int, AssetFilter.AssetFilterResult> onFilterChange;

        public AssetTreeViewGroup(EditorWindow window, int ID, Action<int, AssetFilter.AssetFilterResult> filterChange = null)
        {
            TreeView = new AssetTreeView(new TreeViewState());
            TreeViewValid = false;
            id = ID;
            onFilterChange = filterChange;

            SearchField = new CleanAssetSearchField(window);
            SearchField.delay = 0.1f;
            SearchField.downOrUpArrowKeyPressed += TreeView.SetFocusAndEnsureSelectedItem;
        }

        public void AddAssetFilterType(bool defaultValue, string name, Type type, AssetFilter.AssetMask mask = AssetFilter.AssetMask.None)
        {
            if (assetFilter == null)
            {
                assetFilter = new AssetFilter(OnFilterChange);
            }

            assetFilter.AddAssetFilterType(defaultValue, name, type, mask);
        }

        public AssetFilter.AssetFilterResult GetFilterResult()
        {
            if (assetFilter != null)
            {
                return assetFilter.GetFilterResult();
            }

            return null;
        }

        public string GetSelected()
        {
            return TreeView.GetSelected();
        }

        public void SetSelected(string assetName)
        {
            TreeView.SetSelected(assetName);
        }

        public bool HasAsset(string assetName)
        {
            return TreeView.HasAsset(assetName);
        }

        private void OnFilterChange(AssetFilter.AssetFilterResult filterResult)
        {
            if (onFilterChange != null)
            {
                onFilterChange(id, filterResult);
            }
        }

        public void OnGUI(Rect rect)
        {
            float yMin = rect.yMin;
            GUILayout.BeginArea(new Rect(rect.xMin, yMin, rect.width, 25), EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TreeViewName);
            if (GUILayout.Button("导出数据（显示的列表）"))
            {
                SaveTreeDataToFile();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            yMin += 30;

            if (assetFilter != null)
            {
                assetFilter.OnGUI(new Rect(rect.xMin, yMin, rect.width, 20));
                yMin += 20;
            }

            GUILayout.BeginArea(new Rect(rect.xMin, yMin, rect.width, 20), EditorStyles.toolbar);
            if (SearchField.OnToolbarGUI())
            {
                TreeView.searchString = SearchField.text;
            }
            GUILayout.EndArea();

            yMin += 20;

            if (TreeViewValid)
            {
                TreeView.OnGUI(new Rect(rect.xMin, yMin, rect.width, rect.height - (yMin - rect.yMin)));
            }
        }

        public void SetTreeViewData(List<string> data)
        {
            if (data != null && data.Count > 0)
            {
                TreeView.SetTreeViewData(data);
                TreeViewValid = true;
            }
            else
            {
                TreeViewValid = false;
            }
        }

        public void SetTreeViewClickCallback(Action<string> click = null, Action<string> doubleClick = null)
        {
            TreeView.SetTreeViewClickCallback(click, doubleClick);
        }

        public void SetColorChangeIndex(int i)
        {
            TreeView.ColorChangeIndex = i;
        }

        public Color ColorChange
        {
            set { TreeView.ColorChange = value; }
        }

        private void SaveTreeDataToFile()
        {
            string fileName = EditorUtility.SaveFilePanel("导出到文件", AssetUtility.m_RecentSavePath, "", "txt");
            if (fileName.Length != 0)
            {
                AssetUtility.m_RecentSavePath = Path.GetDirectoryName(fileName);

                using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    List<string> assetNames = TreeView.GetData();

                    StreamWriter streamWriter = new StreamWriter(file);

                    for (int i = 0, icnt = assetNames.Count; i < icnt; ++i)
                        streamWriter.WriteLine(assetNames[i]);

                    streamWriter.Flush();
                    streamWriter.Close();
                    file.Close();
                }
            }
        }
    }
}
