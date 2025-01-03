using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CleanAssetsTool
{
    public class AssetRelationWindow : EditorWindow
    {
        enum TreeType
        {
            ParentAsset = 0,
            ShareChildAsset,
            UniqueChildAsset,

            TreeTypeNum
        };

        private string m_AssetName;
        private string m_AssetLabelName;
        private BaseAsset m_BaseAsset;
        private AssetTreeViewGroup[] m_AssetTreeViews = new AssetTreeViewGroup[(int)TreeType.TreeTypeNum];
        private AssetGraph m_AssetGraph;
        private bool m_ShowAll = true;
        private CleanAssetsEditor m_ParentWindow;

        private int ManualAddRelation = 99;
        private string m_CurrentSelectChild = "";

        internal static AssetRelationWindow ShowRelationWindow(string assetName, AssetGraph assetGraph, CleanAssetsEditor parent)
        {
            // Get existing open window or if none, make a new one:
            AssetRelationWindow window = (AssetRelationWindow)EditorWindow.GetWindow(typeof(AssetRelationWindow));
            window.titleContent = new GUIContent("资源引用关系");
            window.Show();
            window.Init(assetName, assetGraph, parent);

            return window;
        }

        private void Init(string assetName, AssetGraph assetGraph, CleanAssetsEditor parent)
        {
            m_ParentWindow = parent;
            m_AssetName = assetName;
            m_AssetGraph = assetGraph;
            m_CurrentSelectChild = "";

            m_AssetLabelName = m_AssetName;
            m_BaseAsset = m_AssetGraph.GetAssetInfo(m_AssetName);
            if(m_BaseAsset != null)
            {
                if(m_BaseAsset.DependentByRoot) m_AssetLabelName += "(使用中)";
                else if(m_BaseAsset.MarkAsDelete)  m_AssetLabelName += "(未使用，待删除列表中)";
                else  m_AssetLabelName += "(未使用)";
            }

            if (m_AssetTreeViews[0] == null)
            {
                for (int i = 0; i < (int)TreeType.TreeTypeNum; ++i)
                {
                    m_AssetTreeViews[i] = new AssetTreeViewGroup(this, i);
                    m_AssetTreeViews[i].ColorChange = Color.yellow;

                    // if(i > (int)TreeType.ParentAsset)
                    {
                        m_AssetTreeViews[i].SetTreeViewClickCallback(SelTreeItemChanged, SelTreeItemDoubleChanged);
                    }
                }
            }

            RefreshTreeViewList(assetName);
        }

        private void SelTreeItemChanged(string assetName)
        {
            m_CurrentSelectChild = assetName;
            AssetUtility.SetSelectionInProjectWindow(assetName);
        }

        private void SelTreeItemDoubleChanged(string assetName)
        {
            string preAssetName = m_AssetName;

            Init(assetName, m_AssetGraph, m_ParentWindow);

            for (int i = 0; i < (int)TreeType.TreeTypeNum; ++i)
            {
                if(m_AssetTreeViews[i].HasAsset(preAssetName))
                {
                    m_AssetTreeViews[i].SetSelected(preAssetName);
                }
            }

            m_ParentWindow.SetSelected(assetName);

            AssetUtility.SetSelectionInProjectWindow(assetName);
        }

        void OnGUI()
        {
            if (Event.current.commandName == "ObjectSelectorUpdated"
            && EditorGUIUtility.GetObjectPickerControlID() == ManualAddRelation)
            {
                var path = AssetDatabase.GetAssetPath(EditorGUIUtility.GetObjectPickerObject());
                if (path != null)
                {
                    AddChildManual(path);
                }
            }

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(m_AssetLabelName);
            bool showALL = GUILayout.Toggle(m_ShowAll, "显示所有依赖(否则只显示直接依赖, 比如一个Prefab依赖一个Material, Material依赖一张Texture, 则Prefab不直接依赖Texture)");
            if(showALL != m_ShowAll)
            {
                m_ShowAll = showALL;
                RefreshTreeViewList(m_AssetName);
            }

            GUILayout.Label("说明: 黄色表示该依赖是通过脚本中的字符串标记的，不能通过Unity的依赖系统查找到");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("手动添加依赖关系"))
            {
                EditorGUIUtility.ShowObjectPicker<UnityEngine.Object>(null, false, null, ManualAddRelation);
            }

            if (m_BaseAsset.DummyRoot)
            {
                if (GUILayout.Button("将选中的未使用资源设置为依赖"))
                {
                    AddUnusedChild();
                }
            }

            if (GUILayout.Button("手动删除依赖关系"))
            {
                DelChildManual();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            // Parent区域
            m_AssetTreeViews[(int)TreeType.ParentAsset].OnGUI(new Rect(0, 100, position.width / 2, position.height - 100));
            // 共享Child区域
            m_AssetTreeViews[(int)TreeType.ShareChildAsset].OnGUI(new Rect(position.width / 2 + 10, 100, position.width / 2 - 10, position.height / 2));
            // 独占Child区域
            m_AssetTreeViews[(int)TreeType.UniqueChildAsset].OnGUI(new Rect(position.width / 2 + 10, position.height / 2 + 100, position.width / 2 - 10, position.height / 2 - 100));
        }

        private void AddUnusedChild()
        {
            string assetName = m_ParentWindow.GetSelectedAsset(CleanAssetsEditor.TreeType.UnUsedAssetTree);
            if (string.IsNullOrEmpty(assetName))
            {
                EditorUtility.DisplayDialog("", "你需要先在未使用列表中选中一个资源", "确认");
                return;
            }

            AddChildManual(assetName);
        }

        private void DelChildManual()
        {
            if(m_CurrentSelectChild == "")
            {
                EditorUtility.DisplayDialog("", "你需要先选中一个依赖资源", "确认");
                return;
            }

            if(m_BaseAsset.m_Parents.Contains(m_CurrentSelectChild))
            {
                m_AssetGraph.UnLinkAsset(m_CurrentSelectChild, m_AssetName, true);
            }
            else
            {
                if(!m_BaseAsset.m_Children.Contains(m_CurrentSelectChild))
                {
                    EditorUtility.DisplayDialog("", "你只能删除直接依赖资源（将显示所有依赖勾去掉）", "确认");
                    return;
                }

                m_AssetGraph.UnLinkAsset(m_AssetName, m_CurrentSelectChild, true);
            } 

            m_ParentWindow.RefreshTreeViewList();
            RefreshTreeViewList(m_AssetName);
        }

        private void AddChildManual(string childName)
        {
            m_AssetGraph.LinkAsset(m_AssetName, childName, false, true);

            m_ParentWindow.RefreshTreeViewList();
            RefreshTreeViewList(m_AssetName);
        }

        private void GetParentAsset(BaseAsset asset, bool recursive, bool bImplicit, HashSet<string> explicitList, HashSet<string> implicitList)
        {
            foreach(var dependentName in asset.m_Parents)
            {
                if (explicitList.Contains(dependentName) || implicitList.Contains(dependentName))
                    continue;

                BaseAsset depAsset = m_AssetGraph.GetAssetInfo(dependentName);
                if (depAsset == null)
                {
                    WarningWindow.PushError("No AssetInfo Found in AssetGraph: {0}", dependentName);
                }

                if (!depAsset.m_Children.Contains(asset.Name))
                {
                    WarningWindow.PushError("LinkInfo Error between: \"{0}\" and \"{1}\"", dependentName, asset.Name);
                }

                if (bImplicit || depAsset.m_ImplicitChildren.Contains(asset.Name))
                {
                    implicitList.Add(dependentName);
                    bImplicit = true;
                }
                else
                {
                    explicitList.Add(dependentName);
                    if(implicitList.Contains(dependentName))
                    {
                        implicitList.Remove(dependentName);
                    }
                }

                if (recursive)
                {
                    GetParentAsset(depAsset, recursive, bImplicit, explicitList, implicitList);
                }
            }
        }

        private void GetChildAsset(BaseAsset asset, bool recursive, bool bImplicit, HashSet<string> explicitList
            , HashSet<string> implicitList, HashSet<string> UniquetList)
        {
            if(asset is ScriptAsset)
            {
                ScriptAsset scriptAsset = asset as ScriptAsset;
                foreach(string combinePath in scriptAsset.m_CombilePath)
                {
                    implicitList.Add(combinePath);
                }
            }

            foreach (var dependentName in asset.m_Children)
            {
                if (explicitList.Contains(dependentName) || implicitList.Contains(dependentName))
                    continue;

                BaseAsset depAsset = m_AssetGraph.GetAssetInfo(dependentName);
                if (depAsset == null)
                {
                    WarningWindow.PushError("No AssetInfo Found in AssetGraph: {0}", dependentName);
                }

                if (!depAsset.m_Parents.Contains(asset.Name))
                {
                    WarningWindow.PushError("LinkInfo Error between: \"{0}\" and \"{1}\"", asset.Name, dependentName);
                }

                bool bUnique = true;
                foreach (var childParent in depAsset.m_Parents)
                {
                    if(!UniquetList.Contains(childParent))
                    {
                        bUnique = false;
                        break;
                    }
                }

                if(bUnique)
                {
                    UniquetList.Add(dependentName);
                }

                if (bImplicit || asset.m_ImplicitChildren.Contains(dependentName))
                {
                    implicitList.Add(dependentName);
                    bImplicit = true;
                }
                else
                {
                    explicitList.Add(dependentName);
                    if (implicitList.Contains(dependentName))
                    {
                        implicitList.Remove(dependentName);
                    }
                }

                if (recursive)
                {
                    GetChildAsset(depAsset, recursive, bImplicit, explicitList, implicitList, UniquetList);
                }
            }
        }

        private void RefreshTreeViewList(string assetName)
        {
            m_CurrentSelectChild = "";

            for (int i = 0; i < (int)TreeType.TreeTypeNum; ++i)
            {
                m_AssetTreeViews[i].TreeViewValid = false;
            }

            int parentNum = 0;
            int shareChildNum = 0;
            int uniqueChildNum = 0;

            BaseAsset asset = m_AssetGraph.GetAssetInfo(assetName);
            if (asset != null)
            {
                if (asset.m_Parents.Count > 0)
                {
                    HashSet<string> explicitList = new HashSet<string>();
                    HashSet<string> implicitList = new HashSet<string>();
                    GetParentAsset(asset, m_ShowAll, false, explicitList, implicitList);

                    List<string> dependentAsset = new List<string>();
                    dependentAsset.AddRange(explicitList);
                    dependentAsset.AddRange(implicitList);

                    m_AssetTreeViews[(int)TreeType.ParentAsset].SetTreeViewData(dependentAsset);
                    m_AssetTreeViews[(int)TreeType.ParentAsset].SetColorChangeIndex(explicitList.Count);
                    parentNum = dependentAsset.Count;
                }

                // 如果是ScriptAsset，有可能没有Child，但有合成路径
                if (asset is ScriptAsset || asset.m_Children.Count > 0)
                {
                    HashSet<string> explicitList = new HashSet<string>();
                    HashSet<string> implicitList = new HashSet<string>();
                    HashSet<string> uiquetList = new HashSet<string>();
                    uiquetList.Add(assetName);

                    GetChildAsset(asset, m_ShowAll, false, explicitList, implicitList, uiquetList);

                    List<string> shareChild = new List<string>();
                    List<string> UniqueChild = new List<string>();
                    foreach (var dependentName in explicitList)
                    {
                        if (uiquetList.Contains(dependentName))
                            UniqueChild.Add(dependentName);
                        else
                            shareChild.Add(dependentName);
                    }

                    m_AssetTreeViews[(int)TreeType.ShareChildAsset].SetColorChangeIndex(shareChild.Count);
                    m_AssetTreeViews[(int)TreeType.UniqueChildAsset].SetColorChangeIndex(UniqueChild.Count);

                    foreach (var dependentName in implicitList)
                    {
                        if (uiquetList.Contains(dependentName))
                            UniqueChild.Add(dependentName);
                        else
                            shareChild.Add(dependentName);
                    }

                    if (shareChild.Count > 0)
                    {
                        m_AssetTreeViews[(int)TreeType.ShareChildAsset].SetTreeViewData(shareChild);

                        shareChildNum = shareChild.Count;
                    }

                    if (UniqueChild.Count > 0)
                    {
                        m_AssetTreeViews[(int)TreeType.UniqueChildAsset].SetTreeViewData(UniqueChild);

                        uniqueChildNum = UniqueChild.Count;
                    }
                }
            }
            else
            {
                WarningWindow.PushError("No AssetInfo Found in AssetGraph: {0}", assetName);
            }

            m_AssetTreeViews[(int)TreeType.ParentAsset].TreeViewName = string.Format("依赖它的资源，{0}个：", parentNum);
            m_AssetTreeViews[(int)TreeType.ShareChildAsset].TreeViewName = string.Format("它依赖的资源（共享），{0}个：", shareChildNum);
            m_AssetTreeViews[(int)TreeType.UniqueChildAsset].TreeViewName = string.Format("它依赖的资源（独占），{0}个：", uniqueChildNum);
        }
    }
}
