using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Tenmove.Editor.Unity
{
    public class SceneEffectiveAreaTreeViewItem : TreeViewItem
    {
        public string name;
        public Renderer renderer;
        public float effectiveRate;
        public int effectiveArea;
        public int totalArea;
        public int sceneID;

        public SceneEffectiveAreaTreeViewItem(int id, string name, Renderer renderer, 
            float effectiveRate, int effectiveArea, int totalArea, int sceneID)
            : base(id)
        {
            this.name = name;
            this.renderer = renderer;
            this.effectiveRate = effectiveRate;
            this.effectiveArea = effectiveArea;
            this.totalArea = totalArea;
            this.sceneID = sceneID;
        }
    }

    public class SceneEffectiveAreaTreeView : TreeView
    {
        private static TreeViewState m_TreeViewState = new TreeViewState();

        private bool m_IsReadyToShow;
        private List<SceneEffectiveAreaObject> m_Data;
        private List<SceneEffectiveAreaTreeViewItem> m_Items = new List<SceneEffectiveAreaTreeViewItem>();

        public SceneEffectiveAreaTreeView(MultiColumnHeader multiColumnHeader)
            : base(m_TreeViewState, multiColumnHeader)
        {
            columnIndexForTreeFoldouts = 1;
            extraSpaceBeforeIconAndLabel = 18f;
            float kRowHeights = 20f;
            rowHeight = kRowHeights;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            showBorder = true;
            showAlternatingRowBackgrounds = true;
        }

        protected override TreeViewItem BuildRoot()
        {
            m_Items.Clear();
            SceneEffectiveAreaTreeViewItem root = new SceneEffectiveAreaTreeViewItem(0, "", null, 0f, 0, 0, 0);
            root.depth = -1;
            m_Items.Add(root);

            if (m_Data != null)
            {
                foreach (SceneEffectiveAreaObject sceneObject in m_Data)
                {
                    SceneEffectiveAreaTreeViewItem item = new SceneEffectiveAreaTreeViewItem(m_Items.Count, sceneObject.name,
                        sceneObject.renderer, sceneObject.effectiveRate, sceneObject.effectiveArea, sceneObject.totalArea, sceneObject.id);
                    m_Items.Add(item);
                    root.AddChild(item);
                }
            }

            SetupDepthsFromParentsAndChildren(root);

            return root;
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            SceneEffectiveAreaTreeViewItem item;
            if (id > 0 && id < m_Items.Count)
                item = m_Items[id];
            else
                item = FindItem(id, rootItem) as SceneEffectiveAreaTreeViewItem;

            if (item != null)
            {
                if (item.renderer != null)
                {
                    Selection.objects = new Object[] { item.renderer.gameObject };
                    //SceneView.lastActiveSceneView.FrameSelected();
                }
            }
        }

        public void SetData(List<SceneEffectiveAreaObject> sceneObjects)
        {
            m_Data = sceneObjects;
            m_IsReadyToShow = sceneObjects.Count > 0;
            Reload();
        }

        public override void OnGUI(Rect rect)
        {
            if (m_IsReadyToShow)
            {
                base.OnGUI(rect);
            }
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            SceneEffectiveAreaTreeViewItem item = (SceneEffectiveAreaTreeViewItem)args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
            }
        }

        private void CellGUI(Rect cellRect, SceneEffectiveAreaTreeViewItem item, int column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                // Name
                case 0:
                    GUI.Label(cellRect, item.name);
                    break;
                // Rate
                case 1:
                    if(SceneEffectiveAreaWindow.CONST_DEBUG)
                        GUI.Label(cellRect, string.Format("Rate:[{0}], EffectiveArea:[{1}], TotalArea:[{2}], ID:[{3}]", item.effectiveRate, 
                            item.effectiveArea, item.totalArea, item.sceneID));
                    else
                        GUI.Label(cellRect, (item.effectiveRate * 100).ToString() + "%");
                    break;
            }
        }

        public static MultiColumnHeaderState DefaultHeaderState()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    //contextMenuText = "Asset",
                    headerTextAlignment = UnityEngine.TextAlignment.Center,
                    sortedAscending = true,
                    sortingArrowAlignment = UnityEngine.TextAlignment.Right,
                    width = 400,
                    minWidth = 80,
                    maxWidth = 80,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("EffectiveRate"),
                    //contextMenuText = "Type",
                    headerTextAlignment = UnityEngine.TextAlignment.Center,
                    sortedAscending = true,
                    sortingArrowAlignment = UnityEngine.TextAlignment.Right,
                    width = 100,
                    minWidth = 400,
                    maxWidth = 1000,
                    autoResize = false,
                    allowToggleVisibility = true
                },
            };


            var state = new MultiColumnHeaderState(columns);
            return state;
        }
    }
}


