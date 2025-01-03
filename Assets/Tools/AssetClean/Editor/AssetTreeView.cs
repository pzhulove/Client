using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace CleanAssetsTool
{
    public class AssetTreeViewItem : TreeViewItem
    {
        public string path;
        public int iconID;
        public int parentNum;   // 依赖这个资源的数量
        public AssetTreeViewItem(int id, int depth, string displayName, string path) : base(id, depth, displayName)
        {
            this.path = path;

            if(AssetGraph.Instance != null)
            {
                BaseAsset baseAsset = AssetGraph.Instance.GetAssetInfo(path);
                if(baseAsset != null)
                {
                    if(baseAsset.DependentByRoot || baseAsset.IsRoot)
                        iconID = 0;
                    else if (baseAsset.MarkAsDelete)
                        iconID = 2;
                    else 
                        iconID = 1;

                    // 计算使用中的资源被引用的次数
                    if(baseAsset.DependentByRoot || baseAsset.IsRoot)
                    {
                        HashSet<string> explicitList = new HashSet<string>();
                        HashSet<string> implicitList = new HashSet<string>();
                        GetParentAsset(baseAsset, true, false, explicitList, implicitList);

                        parentNum = explicitList.Count + implicitList.Count;
                    }
                }
            }
        }

        private void GetParentAsset(BaseAsset asset, bool recursive, bool bImplicit, HashSet<string> explicitList, HashSet<string> implicitList)
        {
            foreach (var dependentName in asset.m_Parents)
            {
                if (explicitList.Contains(dependentName) || implicitList.Contains(dependentName))
                    continue;

                BaseAsset depAsset = AssetGraph.Instance.GetAssetInfo(dependentName);
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
                    if (implicitList.Contains(dependentName))
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
    }

    public class AssetTreeView : TreeView
    {
        const float kRowHeights = 20f;
        const float kToggleWidth = 18f;
        public bool showControls = true;
        private List<AssetTreeViewItem> treeViewAssetItems = new List<AssetTreeViewItem>();

        private List<string> data = new List<string>();
        private int colorChangeIndex = -1;
        private Color changeColor;

        Action<string> assetSelect = null;
        Action<string> assetSelectDoubleClick = null;

        public static Texture2D[] s_Icons;

        public int ColorChangeIndex
        {
            set { colorChangeIndex = value; }
        }

        public Color ColorChange
        {
            set { changeColor = value; }
        }

        public AssetTreeView(TreeViewState state) : base(state)
        {
            rowHeight = kRowHeights;
            //columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            extraSpaceBeforeIconAndLabel = kToggleWidth;

            if(s_Icons == null)
            {
                s_Icons = new Texture2D[3];
                s_Icons[0] =  EditorGUIUtility.FindTexture ("PrefabNormal Icon"); // used
                s_Icons[1] =  AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Tools/AssetClean/Editor/Res/unused.png");
                s_Icons[2] =  AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Tools/AssetClean/Editor/Res/delete.png");
            }
        }

        public List<string> GetData()
        {
            return data;
        }

        public bool ShowHorizontalScrollbar()
        {
            return showingHorizontalScrollBar;
        }

        public void SetTreeViewData(List<string> data)
        {
            this.data = data;

            Reload();
        }

        public void SetTreeViewClickCallback(Action<string> click = null, Action<string> dounbleClick = null)
        {
            this.assetSelect = click;
            this.assetSelectDoubleClick = dounbleClick;
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            treeViewAssetItems.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                AssetTreeViewItem item = new AssetTreeViewItem(i, 0, data[i], data[i]);
                //item.icon = s_Icons[item.iconID];

                treeViewAssetItems.Add(item);
                root.AddChild(item);
            }

            //SetTreeViewData(treeViewEffectVarientItems);
            return root;
        }

        /*
                protected override void SingleClickedItem(int id)
                {
                    base.SingleClickedItem(id);
                    string assetName = data[id];

                    if (this.assetSelect != null)
                    {
                        assetSelect(assetName);
                    }
                }*/

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            if (selectedIds.Count > 0)
            {
                string assetName = data[selectedIds[0]];
                if (this.assetSelect != null)
                {
                    assetSelect(assetName);
                }
            }
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            string assetName = data[id];

            if (this.assetSelectDoubleClick != null)
            {
                assetSelectDoubleClick(assetName);
            }
        }

        public string GetSelected()
        {
            if(HasSelection())
            {
                for(int i = 0; i < data.Count; ++i)
                {
                    if(IsSelected(i))
                    {
                        return data[i];
                    }
                }
            }

            return "";
        }

        public void SetSelected(string assetName)
        {
            for(int i = 0; i < data.Count; ++i)
            {
                if(data[i] == assetName)
                {
                    SetSelection(new List<int>{i});
                }
            }
        }

        public bool HasAsset(string assetName)
        {
            return data.Contains(assetName);
        }


        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (AssetTreeViewItem)args.item;
            CellGUI(args.rowRect, item, ref args);
        }

        void CellGUI(Rect cellRect, AssetTreeViewItem item, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);
            /*
                        if (item.isFolder)
                        {
                            Rect toggleRect = cellRect;
                            toggleRect.x += GetContentIndent(item);
                            toggleRect.width = kToggleWidth;
                            GUI.DrawTexture(toggleRect, s_TestIcons[0], ScaleMode.ScaleToFit);
                            args.rowRect = cellRect;
                            base.RowGUI(args);
                        }
                        else*/
            {
                Rect toggleRect = cellRect;
                toggleRect.x += GetContentIndent(item);
                toggleRect.width = kToggleWidth;
                GUI.DrawTexture(toggleRect, s_Icons[item.iconID], ScaleMode.ScaleToFit);
                cellRect.x += kToggleWidth;

                // 使用中的资源显示被引用的次数
                if(item.iconID == 0)
                {
                    Rect parentNumRect = cellRect;
                    int parentNumWidth = 50;
                    parentNumRect.width = parentNumWidth;
                    GUI.Label(parentNumRect, new GUIContent(item.parentNum.ToString()));
                    //cellRect.x += parentNumWidth;
                }

                args.rowRect = cellRect;

                bool bColorChange = (colorChangeIndex >= 0) && (item.id >= colorChangeIndex);
                Color preColor = GUI.color;

                if (bColorChange)
                {
                    GUI.color = changeColor;
                }

                base.RowGUI(args);

                if (bColorChange)
                {
                    GUI.color = preColor;
                }
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }
    }
}
