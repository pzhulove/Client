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
    public class InspectCheckRedundancy
    {
        private Rect m_RectPos;
        private string assetBundlePath = "";
        private List<string> bundlePath = new List<string>();
        private Dictionary<string, List<string>> mapDic = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> redundancyDic = new Dictionary<string, List<string>>();
        private List<string> bundleFile = new List<string>();
        private InspectTreeView inspectTreeView;
        private string dataPath = "";
        private int buttonHeight = 25;
        private int offest;
        Vector2 m_ScrollPosition;
        string path = "";
        /// <summary>
        /// 去除资源冗余文件
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="assetBundlePath"></param>
        /// <param name="inspectTreeView"></param>
        public void OnEnable(Rect pos, string assetBundlePath, InspectTreeView inspectTreeView)
        {
            m_RectPos = pos;
            offest = ((int)InspectModel.End + 1) / 3;
            this.assetBundlePath = assetBundlePath;
            dataPath = Application.dataPath;
            this.inspectTreeView = inspectTreeView;
            bundlePath.Clear();
            mapDic.Clear();
            redundancyDic.Clear();
            bundleFile.Clear();
            if (!string.IsNullOrEmpty(this.assetBundlePath))
            {
                string[] files = Directory.GetFiles(this.assetBundlePath, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]);
                    if (extension.Equals(".meta") || extension.Equals(".manifest") || extension.Equals(""))
                        continue;
                    bundlePath.Add(files[i].Replace('\\', '/').Replace(dataPath, "Assets"));
                }
            }
            for (int i = 0; i < bundlePath.Count; i++)
            {
                EditorUtility.DisplayProgressBar("正在检视资源冗余", "检视中", i * 1.0f / bundlePath.Count);
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath[i]);
                string[] bundleAssets = bundle.GetAllAssetNames();
                bundle.Unload(true);
                for (int j = 0; j < bundleAssets.Length; j++)
                {
                    bundleFile.Add(bundleAssets[j]);
                    string[] dependencies = AssetDatabase.GetDependencies(bundleAssets[j]);
                    for (int k = 0; k < dependencies.Length; k++)
                    {
                        var ext = Path.GetExtension(dependencies[k]);
                        var notAllowedExtensions = new string[] { ".meta", ".manifest", ".dll", ".cs", ".exe", ".js" };
                        if (!notAllowedExtensions.Contains(ext))
                        {
                            if (mapDic.ContainsKey(dependencies[k]))
                            {
                                if (!mapDic[dependencies[k]].Contains(bundlePath[i]))
                                {
                                    mapDic[dependencies[k]].Add(bundlePath[i]);
                                }
                            }
                            else
                            {
                                List<string> list = new List<string>();
                                list.Add(bundlePath[i]);
                                mapDic.Add(dependencies[k], list);
                            }
                        }
                    }
                }
            }
            var it = mapDic.GetEnumerator();
            while (it.MoveNext())
            {
                if (it.Current.Value.Count > 1 && !bundleFile.Contains(it.Current.Key.ToLower()))
                {
                    redundancyDic.Add(it.Current.Key, it.Current.Value);
                }
            }
            List<InspectTreeItem> treeViewItems = new List<InspectTreeItem>();
            var it1 = redundancyDic.GetEnumerator();
            while (it1.MoveNext())
            {
                InspectTreeItem item = new InspectTreeItem(it1.Current.Key, 0);
                treeViewItems.Add(item);
            }
            inspectTreeView.SetTreeViewData(treeViewItems, ShowSelectFileBundle);
            EditorUtility.ClearProgressBar();
        }
        public void OnGUI(Rect pos)
        {
            m_RectPos = pos;
            if (redundancyDic.Count > 0)
            {
                inspectTreeView.OnGUI(new Rect(m_RectPos.x, m_RectPos.y + buttonHeight * offest, m_RectPos.width / 2, m_RectPos.height - buttonHeight * offest));
            }
            if (redundancyDic.Count > 0 && redundancyDic.ContainsKey(path))
            {
                GUILayout.BeginArea(new Rect(m_RectPos.width / 2 + 10, m_RectPos.y + buttonHeight * offest, m_RectPos.width / 2 - 10, m_RectPos.height - buttonHeight * offest));
                EditorGUILayout.BeginVertical("box");
                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
                for (int i = 0; i < redundancyDic[path].Count; i++)
                {
                    GUILayout.Label(redundancyDic[path][i]);
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
        public void ShowSelectFileBundle(string path)
        {
            this.path = path;
        }
        public void OnDestroy()
        {

        }
    }
}
