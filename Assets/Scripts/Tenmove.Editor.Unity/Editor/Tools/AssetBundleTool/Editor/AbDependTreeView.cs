using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

public class AbDependTreeView : TreeView
{
    #region Params
    private const float KRowHeights = 15;
    private const float KToggleWidth = 10f;
    private readonly List<TreeViewItem> _allItem;
    private TreeViewItem _rootItem;
    public Action<string> CurSelectDoubleClick;
    public Action<List<string>, List<string>> AssetSelect;
    #endregion

    #region Init
    public AbDependTreeView(TreeViewState state) : base(state)
    {
        _allItem = new List<TreeViewItem>();
        _rootItem = new TreeViewItem { depth = -1, id = 0, displayName = "Root" };
        rowHeight = KRowHeights;
        extraSpaceBeforeIconAndLabel = KToggleWidth;
        showAlternatingRowBackgrounds = true;
        showBorder = true;
        Reload();
    }
    protected override TreeViewItem BuildRoot()
    {
        SetupParentsAndChildrenFromDepths(_rootItem, _allItem);
        return _rootItem;
    }
    #endregion

    #region SetTreeViewData
    public void SetTreeViewData(List<TreeViewItem> allItem)
    {
        ValidateAllItem(allItem);
        Reload();
    }
    private void ValidateAllItem(List<TreeViewItem> allItem)
    {
        _rootItem = null;
        _allItem.Clear();
        foreach (TreeViewItem item in allItem)
        {
            if (item.depth == -1)
                _rootItem = item;
            else
                _allItem.Add(item);
        }
        if (_rootItem == null)
            _rootItem = new TreeViewItem { depth = -1, id = 0, displayName = "Root" };
    }
    #endregion

    #region Func_Override
    protected override void SelectionChanged(IList<int> selectedIds)
    {
        base.SelectionChanged(selectedIds);
        if (selectedIds.Count == 0)
            return;
        if (AssetSelect == null)
            return;
        List<string> assetNameList = new List<string>();
        List<string> objNameList = new List<string>();
        foreach (int id in selectedIds)
        {
            TreeViewItem item = _allItem[id - 1];
            if (item.depth == 0)
            {
                assetNameList.Add(item.displayName);
                objNameList.Add(null);
            }
            else
            {
                assetNameList.Add(GetParentItemByItem(item).displayName);
                objNameList.Add(item.displayName);
            }
        }
        AssetSelect(assetNameList, objNameList);
    }
    protected override void DoubleClickedItem(int id)
    {
        base.DoubleClickedItem(id);
        if (CurSelectDoubleClick == null)
            return;
        TreeViewItem item = _allItem[id - 1];
        if (item.depth == 0)
            CurSelectDoubleClick(item.displayName);
        else
            CurSelectDoubleClick(GetParentItemByItem(item).displayName);
    }
    #endregion

    #region Func_Aid
    private TreeViewItem GetParentItemByItem(TreeViewItem childItem)
    {
        for (int i = childItem.id - 2; i >= 0; i--)
        {
            if (_allItem[i].depth < childItem.depth)
                return _allItem[i];
        }
        return null;
    }
    #endregion
}