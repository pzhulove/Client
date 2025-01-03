using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System;
namespace AssetBundleTool
{
    public class InspectTreeItem : TreeViewItem
    {
        public string bundlePath { get; private set; }

        public InspectTreeItem(string path, int depth) : base(path.GetHashCode(), depth, path)
        {
            bundlePath = path;
        }
    }
    public class InspectTreeView : TreeView
    {
        TreeViewItem root;
        List<TreeViewItem> treeViewItems = new List<TreeViewItem>();
        InspectTreeItem currentItem;
        Action<string> ClickItem;
        public InspectTreeView(TreeViewState s) : base(s)
        {
            showBorder = true;
            Reload();
        }
        public void SetTreeViewData(List<InspectTreeItem> treeViewItems, Action<string> clickItem)
        {
            this.treeViewItems.Clear();
            for (int i = 0; i < treeViewItems.Count; i++)
            {
                this.treeViewItems.Add(treeViewItems[i]);
            }
            BuildRows(root);
            ClickItem = clickItem;
            this.Reload();
        }
        protected override TreeViewItem BuildRoot()
        {
            root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            return root;
        }
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            SetupParentsAndChildrenFromDepths(root, treeViewItems);
            return base.BuildRows(root);
        }
        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                SetSelection(new int[0], TreeViewSelectionOptions.FireSelectionChanged);
            }
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            currentItem = FindItem(id, root) as InspectTreeItem;
            if (currentItem != null && ClickItem != null)
            {
                ClickItem(currentItem.bundlePath);
            }
        }
        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }
    }
}