using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using Tenmove.Runtime.Unity;
using Tenmove.Runtime;
using Tenmove.Runtime.Unity;

namespace AssetBundleTool
{
    public class InspectCheckDependence
    {
        private Rect m_RectPos;
        private string assetBundlePath = "";
        private List<BundleInfo> bundleInfoList = new List<BundleInfo>();
        private Dictionary<string, List<string>> bundleDependenceDic = new Dictionary<string, List<string>>();
        private InspectTreeView inspectTreeView;
        private TMUnityAssetTreeData assetTreeData;
        private string dataPath = "";
        private int buttonHeight = 25;
        private int offest;
        private AssetBundle assetBundle;
        Vector2 m_ScrollPosition;
        string path = "";
        public void OnEnable(Rect pos, string assetBundlePath, InspectTreeView inspectTreeView)
        {
            m_RectPos = pos;
            offest = ((int)InspectModel.End + 1) / 3;
            this.assetBundlePath = assetBundlePath;
            dataPath = Application.dataPath;
            this.inspectTreeView = inspectTreeView;
            bundleInfoList.Clear();
            bundleDependenceDic.Clear();
            assetTreeData = null;
            string bundleName = "packageinfo.pak";
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
            }
            assetBundle = AssetBundle.LoadFromFile(Path.Combine(assetBundlePath, bundleName));
            if (assetBundle == null)
            {
                Debug.LogError("找不到 AssetBundles  :" + bundleName);
                return;
            }
            assetTreeData = assetBundle.LoadAsset<TMUnityAssetTreeData>("AssetTreeData.asset");
            if (assetTreeData == null)
            {
                assetBundle.Unload(true);
                Debug.LogError("找不到 TMUnityAssetTreeData  :" + assetTreeData);
                return;
            }
            assetBundle.Unload(false);
            List<Tenmove.Runtime.AssetPackageDesc> packageList = assetTreeData.GetAssetPackageDescMap();
            for (int i = 0; i < packageList.Count; i++)
            {
                EditorUtility.DisplayProgressBar("正在检视依赖信息", "检视中", i * 1.0f / packageList.Count);
                string bundlePath = Path.Combine(assetBundlePath, packageList[i].m_PackageName.m_PackageName);
                if (!File.Exists(bundlePath))
                    continue;
                BundleInfo bundleInfo = new BundleInfo();
                bundleInfo.bundleName = packageList[i].m_PackageName.m_PackageName;
                bundleInfo.bundleSize = File.ReadAllBytes(bundlePath).Length;
                bundleInfo.bundleSizeStr = GetNormalizedValue(bundleInfo.bundleSize);
                bundleInfo.id = packageList[i].PackageID;
                bundleInfo.dependPackageID = packageList[i].m_DependencyPackageIDs;
                bundleInfoList.Add(bundleInfo);
            }
            EditorUtility.ClearProgressBar();

            SortBySize();
            SetTreeList();
        }
        private void SetTreeList()
        {
            bundleDependenceDic.Clear();
            List<InspectTreeItem> treeViewItems = new List<InspectTreeItem>();
            for (int i = 0; i < bundleInfoList.Count; i++)
            {
                int[] dependPackageID = bundleInfoList[i].dependPackageID;
                List<string> bundleInfoDependenceList = new List<string>();
                for (int j = 0; j < dependPackageID.Length; j++)
                {
                    BundleInfo info = GetBundleInfo(dependPackageID[j]);
                    if (info == null)
                        continue;
                    string keyName1 = string.Format("{0}   {1}", info.bundleName, info.bundleSizeStr);
                    bundleInfoDependenceList.Add(keyName1);
                }
                string keyName = string.Format("{0}   {1}", bundleInfoList[i].bundleName, bundleInfoList[i].bundleSizeStr);
                bundleDependenceDic.Add(keyName, bundleInfoDependenceList);
                InspectTreeItem item = new InspectTreeItem(keyName, 0);
                treeViewItems.Add(item);
            }
            inspectTreeView.SetTreeViewData(treeViewItems, ShowSelectFileBundle);
        }
        public void OnGUI(Rect pos)
        {
            m_RectPos = pos;
            if (bundleInfoList.Count > 0)
            {
                inspectTreeView.OnGUI(new Rect(m_RectPos.x, m_RectPos.y + buttonHeight * offest, m_RectPos.width / 2, m_RectPos.height - buttonHeight * offest - 20));
                GUILayout.BeginArea(new Rect(m_RectPos.x, m_RectPos.height, m_RectPos.width / 2, 20));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Bundle大小"))
                {
                    SortBySize();
                    SetTreeList();
                }
                if (GUILayout.Button("A-Z"))
                {
                    SortByAZ();
                    SetTreeList();
                }
                if (GUILayout.Button("依赖个数"))
                {
                    SortByCount();
                    SetTreeList();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            if (bundleInfoList.Count > 0 && bundleDependenceDic.ContainsKey(path))
            {
                GUILayout.BeginArea(new Rect(m_RectPos.width / 2 + 10, m_RectPos.y + buttonHeight * offest, m_RectPos.width / 2 - 10, m_RectPos.height - buttonHeight * offest));
                EditorGUILayout.BeginVertical("box");
                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
                for (int i = 0; i < bundleDependenceDic[path].Count; i++)
                {
                    GUILayout.Label(bundleDependenceDic[path][i]);
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
        private void SortBySize()
        {
            bundleInfoList.Sort((BundleInfo info1, BundleInfo info2) =>
            {
                if (info1.bundleSize > info2.bundleSize)
                {
                    return -1;
                }
                else if (info1.bundleSize < info2.bundleSize)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
        }
        private void SortByAZ()
        {
            bundleInfoList.Sort((BundleInfo info1, BundleInfo info2) =>
            {
                return String.Compare(info1.bundleName, info2.bundleName, true);
            });
        }
        private int GetAscii(string name)
        {
            if (name.Length > 0)
            {
                char c = name[0];
                return (int)c;
            }
            return 0;
        }
        private void SortByCount()
        {
            bundleInfoList.Sort((BundleInfo info1, BundleInfo info2) =>
            {
                if (info1.dependPackageID.Length > info2.dependPackageID.Length)
                {
                    return -1;
                }
                else if (info1.dependPackageID.Length < info2.dependPackageID.Length)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
        }
        public void ShowSelectFileBundle(string path)
        {
            this.path = path;
        }
        private BundleInfo GetBundleInfo(int id)
        {
            BundleInfo bundleInfo = bundleInfoList.Find((BundleInfo info) =>
              {
                  if (info.id.Equals(id))
                  {
                      return true;
                  }
                  else
                  {
                      return false;
                  }
              });
            return bundleInfo;
        }
        private string GetNormalizedValue(long bytes)
        {
            string value = "";
            if (bytes < 1024L)
            {
                value = bytes.ToString() + "B";
            }
            else if (bytes < 1024L * 1024L)
            {
                value = (bytes / 1024L).ToString() + "KB";
            }
            else
            {
                value = ((double)bytes / 1024F / 1024F).ToString("0.0") + "MB";
            }
            return value;
        }
        public void OnDestroy()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
            }
        }
        public class BundleInfo
        {
            public string bundleName;
            public int id;
            public int bundleSize;
            public string bundleSizeStr;
            public int[] dependPackageID;
        }
    }
}
