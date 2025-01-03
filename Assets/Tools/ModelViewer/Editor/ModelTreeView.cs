using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.IO;
public class ModelTreeViewItem : TreeViewItem
{
    public bool isFolder;
    public string path;
    public string guid;
    public ModelTreeViewItem(int id, int depth, string displayName, bool isFolder, string path) : base(id, depth, displayName)
    {
        this.isFolder = isFolder;
        this.path = path;
    }
    public ModelTreeViewItem(int id, int depth, string displayName, bool isFolder, string path, string guid) : base(id, depth, displayName)
    {
        this.isFolder = isFolder;
        this.path = path;
        this.guid = guid;
    }
}
public class ModelTreeView : TreeView
{
    const float kRowHeights = 20f;
    const float kToggleWidth = 18f;
    public bool showControls = true;
    private List<TreeViewItem> treeViewItems = new List<TreeViewItem>();
    private List<ModelTreeViewItem> treeViewModelItems = new List<ModelTreeViewItem>();
    private List<AssetFolder> data = new List<AssetFolder>();
    private ModelViewerWindow modelViewerWindow;
    private bool isAssetLabel = false;
    Action<bool, string, string> modelSelect = null;
    Texture2D[] s_TestIcons =
    {
            EditorGUIUtility.FindTexture ("Folder Icon"),
            EditorGUIUtility.FindTexture ("PrefabNormal Icon"),
    };
    public ModelTreeView(TreeViewState state, ModelViewerWindow varientSetWindow) : base(state)
    {
        rowHeight = kRowHeights;
        //columnIndexForTreeFoldouts = 0;
        showAlternatingRowBackgrounds = true;
        showBorder = true;
        extraSpaceBeforeIconAndLabel = kToggleWidth;
        this.modelViewerWindow = varientSetWindow;
    }
    public void SetTreeViewData(List<AssetFolder> data, Action<bool, string, string> a,bool isAssetLabel=false)
    {
        this.data = data;
        this.modelSelect = a;
        this.isAssetLabel = isAssetLabel;
        Reload();
    }

    private void SetTreeViewItemRecursive(TreeViewItem parent, AssetFolder folder)
    {
        if (!folder.hasPrefabRecursive)
        {
            return;
        }

        ModelTreeViewItem item = new ModelTreeViewItem(treeViewModelItems.Count + 1, folder.depth, folder.name, true, folder.path);
        treeViewModelItems.Add(item);
        parent.AddChild(item);

        for (int i = 0; i < folder.childPrefab.Count; i++)
        {
            ModelTreeViewItem item1 = new ModelTreeViewItem(treeViewModelItems.Count + 1, folder.childPrefab[i].depth, folder.childPrefab[i].name, false, folder.childPrefab[i].path, folder.childPrefab[i].guid);
            item.AddChild(item1);
            treeViewModelItems.Add(item1);
        }
        for (int i = 0; i < folder.childFolder.Count; i++)
        {
            SetTreeViewItemRecursive(item, folder.childFolder[i]);
        }
    }
    public void SetTreeViewData(List<ModelTreeViewItem> treeViewItems)
    {
        this.treeViewItems.Clear();
        for (int i = 0; i < treeViewItems.Count; i++)
        {
            this.treeViewItems.Add(treeViewItems[i]);
        }
        this.Reload();
    }
    protected override TreeViewItem BuildRoot()
    {
        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
        treeViewModelItems.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            SetTreeViewItemRecursive(root, data[i]);
        }
        return root;
    }
    protected override void DoubleClickedItem(int id)
    {
        base.DoubleClickedItem(id);
        ModelTreeViewItem item = FindItem(id, rootItem) as ModelTreeViewItem;
        if (this.modelSelect != null)
        {
            modelSelect(item.isFolder, item.path, item.guid);
        }
    }
    public override void OnGUI(Rect rect)
    {
        base.OnGUI(rect);
    }
    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (ModelTreeViewItem)args.item;
        CellGUI(args.rowRect, item, ref args);
    }

    void CellGUI(Rect cellRect, ModelTreeViewItem item, ref RowGUIArgs args)
    {
        // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
        CenterRectUsingSingleLineHeight(ref cellRect);
        if (item.isFolder)
        {
            Rect toggleRect = cellRect;
            toggleRect.x += GetContentIndent(item);
            toggleRect.width = kToggleWidth;
            GUI.DrawTexture(toggleRect, s_TestIcons[0], ScaleMode.ScaleToFit);
            args.rowRect = cellRect;
            base.RowGUI(args);
        }
        else
        {
            Rect toggleRect = cellRect;
            toggleRect.x += GetContentIndent(item);
            toggleRect.width = kToggleWidth;
            GUI.DrawTexture(toggleRect, s_TestIcons[1], ScaleMode.ScaleToFit);
            args.rowRect = cellRect;
            base.RowGUI(args);
        }
        if (args.item.depth == 0 && item.isFolder&&!isAssetLabel)
        {
            var width = 16;
            var edgeRect = new Rect(args.rowRect.xMax - width, args.rowRect.y, width, args.rowRect.height);
            if (GUI.Button(edgeRect, "-"))
            {
                if (EditorUtility.DisplayDialog("Delete Folder", String.Format("是否删除 {0}", item.path), "确定"))
                {
                    modelViewerWindow.RemoveFolder(item.path);
                }
            }
        }
    }
    protected override bool CanMultiSelect(TreeViewItem item)
    {
        return false;
    }
}
