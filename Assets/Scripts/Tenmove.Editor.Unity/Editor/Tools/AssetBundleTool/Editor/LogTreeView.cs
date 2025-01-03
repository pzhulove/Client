using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
namespace AssetBundleTool
{
    class LogTreeView : TreeView
    {
        TreeViewItem root;
        List<TreeViewItem> allItems = new List<TreeViewItem>();
        public LogTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            Reload();
        }
        public void SetTreeData(List<TreeViewItem> allItems)
        {
            this.allItems = allItems;
            BuildRows(root);
            this.Reload();
        }
        protected override TreeViewItem BuildRoot()
        {
            // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
            // are created from data. Here we just create a fixed set of items, in a real world example
            // a data model should be passed into the TreeView and the items created from the model.

            // This section illustrates that IDs should be unique and that the root item is required to 
            // have a depth of -1 and the rest of the items increment from that.
            root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            // Utility method that initializes the TreeViewItem.children and -parent for all items.
            // Return root of the tree
            return root;
        }
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            SetupParentsAndChildrenFromDepths(root, allItems);
            return base.BuildRows(root);
        }

    }
}

