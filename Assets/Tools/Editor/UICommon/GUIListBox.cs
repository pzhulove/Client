using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace EditorUI
{
    public class UIListItem
    {
        public UIListItem(string[] texts)
        {
            this.texts = texts;
        }

        public UIListItem(string  text)
        {
            this.texts = new string[] { text };
        }

        public void Remove()
        {
            if(container != null)
            {
                container.RemoveItems(this);
            }
        }

        public string getText(int iIndex)
        {

            if(texts == null)
            {
                return string.Empty;
            }

            if(iIndex < 0 || iIndex >= texts.Length)
            {
                return string.Empty;
            }

            return texts[iIndex];
        }

        public bool hasChildren
        {
            get { return childrenroot != null;  }
        }
        public string[]             texts;
        public bool                 isexpand = true;
        public object               userdata;
        public int                  depth  = 0;
        public UIListItem           parent = null;
        public UIListItem           next   = null;
        public UIListItem           pre    = null;
        public UIListItem           childrenroot      = null;
        public UIListItem           childrenend       = null;
        public UIListBox            container;

        public void AddChildren(UIListItem item)
        {
            if(childrenroot == null)
            {
                childrenroot = item;
                childrenend = item;
                item.parent = this;
                item.depth = depth + 1;
            }
            else
            {
                childrenend.next = item;
                item.pre = childrenend;
                item.next = null;
                childrenend = item;
                item.parent = this;
                item.depth = depth + 1;
            }
        }

        public void AddBefore(UIListItem item,UIListItem before)
        {
            if(before == null)
            {
                AddChildren(item);
            }
            else
            {
                item.next = before;
                item.pre = before.pre;
                before.pre = item;

                if(item.pre != null)
                {
                    item.pre.next = item;
                }

                if (childrenroot == before)
                {
                    childrenroot = item;
                }

                item.parent = this;
                item.depth = depth + 1;
            }
        }
        
        bool Has(UIListItem item)
        {
            bool bHas = false;

            UIListItem root = childrenroot;
            while(root != null)
            {
                if(item == root)
                {
                    bHas = true;
                    break;
                }
                root = root.next;
            }

            return bHas;
        }

        public bool RemoveChildren(UIListItem item)
        {
            if (item.parent != this)
            {
                return false;
            }

            if (Has(item) == false)
            {
                return false;
            }

            
            item.parent = null;

            if (item == childrenroot)
            {
                childrenroot = childrenroot.next;

                if (childrenroot != null)
                {
                    childrenroot.pre = null;
                }
                else
                {
                    childrenend = childrenroot = null;
                }
            }
            else if(item == childrenend)
            {
                childrenend = childrenend.pre;

                if(childrenend != null)
                {
                    childrenend.next = null;
                }
                else
                {
                    childrenend = childrenroot = null;
                }
            }
            else
            {
                item.pre.next = item.next;
                item.next.pre = item.pre;
            }

            return true;
        }

        public UIListItem Next(bool bEnterChildren)
        {
            if(bEnterChildren)
            {
                if (childrenroot != null)
                {
                    return childrenroot;
                }
                else
                {
                    return Next(false);
                }
            }
            else
            {
                return next;
            }
        }

        static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        void UpdateChildRootEnd(UIListItem item)
        {
            if(item.pre == null)
            {
                childrenroot = item;
            }

            if(item.next == null)
            {
                childrenend = item;
            }
        }
        private void SwapChildItem(UIListItem left,UIListItem right)
        {
            Swap(ref left.texts,ref right.texts);
            Swap(ref left.userdata,ref right.userdata);
        }

        int DefaultComparison(UIListItem left,UIListItem right)
        {
            if(left.texts.Length < 1 || right.texts.Length < 1)
            {
                return 0;
            }

            return left.texts[0].CompareTo(left.texts[1]);
        }

        int Comparison(UIListItem left,UIListItem right,System.Comparison<UIListItem> comparison)
        {
            if(comparison == null)
            {
                comparison = DefaultComparison;
            }

            return comparison(left,right);
        }

        public void SortChild(System.Comparison<UIListItem> comparison = null)
        {
            var left = childrenroot;
            while(left != null)
            {
                left.SortChild(comparison);
                
                var right = left.next;
                while(right != null)
                {

                    if(Comparison(left,right,comparison) == 1)
                    {
                        SwapChildItem(left,right);
                        //Swap(ref left,ref right);
                    }

                    right = right.next;
                }

                left = left.next;
            }   
        }
    }

    public class UIListBox
    {
        protected GUIStyle background;
        protected GUIStyle header;
        protected GUIStyle label;
        protected GUIStyle entryEven;
        protected GUIStyle entryOdd;
        protected GUIStyle foldout;
        protected GUIStyle numberLabel;
        protected GUIStyle profilerGraphBackground;

        public string s_background = "Box";
        public string s_header = "OL title";
        public string s_label = "OL label";
        public string s_entryEven = "OL EntryBackEven";
        public string s_entryOdd = "OL EntryBackOdd";
        public string s_foldout = "IN foldout";
        public string s_numberLabel = "OL Label";
        public string s_profilerGraphBackground = "ProfilerScrollviewBackground";

        public bool     styleDirty = true;
        public bool     treeStyle = true;

        protected Rect titleRect;
        protected Rect viewRect;
        protected Rect groupRect;
        protected float scrollMargin = 14;
        protected UIListItem root_items = new UIListItem("root");
        protected List<UIListItem> visible_rows = new List<UIListItem>();
        protected bool             visibledirty = true;
        protected EditorWindow     editorWindow;
       // protected UIListItem       selectedItem;
        protected List<UIListItem> selectedItems = new List<UIListItem>();
        protected int              iSelected;
        public OnDragCallback      dragCallBack;
        public OnDeleteCallback     deleteCallBack;
         
        public OnDuplicate          duplicateCallBack;
        
        public delegate void OnDragCallback(UIListItem item);
        public delegate void OnDeleteCallback(UIListItem item);
        public delegate void OnDuplicate(UIListItem item);
        
        protected bool keyBoardFocus = false;
        public UIListItem selected
        {
            get
            {
                if (selectedItems.Count > 0)
                {
                    return selectedItems[selectedItems.Count - 1];
                }
                return null;
            }
            set
            {
                if(value != null && selected != value)
                {
                    selectedItems.Clear();
                    selectedItems.Add(value);
                    MarkVisibleDirty();
                    GetVisibleRows();
                    for (int i = 0; i < visible_rows.Count; ++i)
                    {
                        if (value == visible_rows[i])
                        {
                            iSelected = i;
                            break;
                        }
                    }
                    
                }
            }
        }
        
        public void SortChild(System.Comparison<UIListItem> comparison = null)
        {
            if(root_items != null)
            {
                root_items.SortChild(comparison);
            }
        }
        
        protected void UpdateStyle()
        {
            if(styleDirty)
            {
                styleDirty = false;

                 background = s_background;
                 header = s_header;
                 label = s_label;
                 entryEven = s_entryEven;
                 entryOdd = s_entryOdd;
                 foldout = s_foldout;
                 numberLabel = s_numberLabel;
                 numberLabel.richText = true;
                 profilerGraphBackground = s_profilerGraphBackground;
             }   
        }
        protected bool IsExpanded(UIListItem item)
        {
            if(item == root_items)
            {
                return true;
            }

            return bExpandAll ? true : item.isexpand;
        }

        public delegate bool UIListItemFilter(UIListItem item);


        protected void GetVisibleItemsRecursive(UIListItem item, List<UIListItem> items, UIListItemFilter filter = null)
        {
            if (item != root_items)
            {
                if (null != filter)
                {
                    if (filter(item))
                    {
                        items.Add(item);
                    }
                }
                else
                {
                    items.Add(item);
                }
            }

            if (item.hasChildren && this.IsExpanded(item))
            {
                //foreach (TreeViewItem current in item.children)
                UIListItem current = item.childrenroot;
                while (current != null)
                {
                    this.GetVisibleItemsRecursive(current, items, filter);
                    current = current.next;
                }
            }
        }

        private UIListItemFilter mFilter = null;
        public UIListItemFilter filter
        {
            set
            {
                mFilter = value;
            }
        }

        public List<UIListItem> GetVisibleRows()
        {
            if(visible_rows == null || visibledirty)
            {
                visibledirty = false;

                visible_rows = new List<UIListItem>();
                GetVisibleItemsRecursive(root_items, visible_rows, mFilter);
            }
            return visible_rows;  
        }

        public bool bExpandAll = false;
        public delegate void OnSelectionsChangeCallBack(UIListItem[] item);

        GUIStyle GetRowBackgroundStyle(int iRow)
        {
            return iRow % 2 == 0 ? entryEven : entryOdd;
        }
        public delegate void OnClickTitle(UIListBox box,int iIndex);
        public OnClickTitle onClickTitle;

        public UIListBox(EditorWindow editorWindow,int iTitleHeight = 22,int iItemHeight = 16)
        {
            itemHeight = iItemHeight;
            titleHeight = iTitleHeight;
            this.editorWindow = editorWindow;
        }

        public void SetTitles(string[] titles,float[] relativeSizes)
        {
            buttonTitles = titles;
            state = new SplitterState(relativeSizes);
        }
        
        public OnSelectionsChangeCallBack onSelectionsChange;

        public int itemHeight;
        public int showHeight;
        public int titleHeight;
        public int ScrollViewHeight;
        protected Vector2 scrollPos;
     

        public void MarkVisibleDirty()
        {
            visibledirty = true;
            Repaint();
            GUI.changed = true;
        }
        public void Clear()
        {
            MarkVisibleDirty();
            root_items = new UIListItem("root");
        }

        public UIListItem AddItems(string[] texts,UIListItem parent = null)
        {
            UIListItem item = new UIListItem(texts);

            if(parent == null)
            {
                parent = root_items;
            }
            item.container = this;
            parent.AddChildren(item);
            MarkVisibleDirty();

            return item;
        }
        public delegate bool CanInsert(UIListItem current,UIListItem needInsert); 
        public UIListItem InsertItems(string[] texts,UIListItem parent, CanInsert func)
        {
            UIListItem item = new UIListItem(texts);

            if (parent == null)
            {
                parent = root_items;
            }

            item.container = this;

            var current = parent.childrenroot;
            bool bCanInsert = false;

            if (func != null && current != null)
            {  
                while (current != null)
                {
                    bCanInsert = func(current, item);
                    if (bCanInsert)
                    {
                        parent.AddBefore(item, current);
                        break;
                    }
                    current = current.next;
                }
            }

            if(bCanInsert == false)
            {
                parent.AddChildren(item);
            }
           
            MarkVisibleDirty();

            return item;
        }

        public UIListItem AddItems(string  text,UIListItem parent = null)
        {
            UIListItem item = new UIListItem(text);
            if (parent == null)
            {
                parent = root_items;
            }
            item.container = this;
            parent.AddChildren(item);

            MarkVisibleDirty();

            return item;
        }

        public void RemoveItems(UIListItem item)
        {
            if(item.parent != null)
            {
                item.parent.RemoveChildren(item);
            }
            else
            {
                root_items.RemoveChildren(item);
            }
            item.container = null;
            MarkVisibleDirty();
        }

        UIListItem GetNext(bool bEnterChildren)
        {
            return root_items.Next(bEnterChildren);
        }

        protected void Repaint()
        {
            if(editorWindow)
            {
                editorWindow.Repaint();
            }
        }
        private static int ListBoxHash = "EditorUIListBox".GetHashCode();
        private int ID;
        protected string[] buttonTitles;
        protected EditorUI.SplitterState state;
 

        private Rect GetRowRect(int rowIndex,float width)
        {
            return new Rect(1f, viewRect.y + itemHeight * (float)rowIndex, width, itemHeight);
        }

        protected void DrawTextColumn(ref Rect currentRect, string text, int index, float margin, bool selected)
        {
            if (index != 0)
            {
                currentRect.x += (float)this.state.realSizes[index - 1];
            }
            currentRect.x += margin;
            currentRect.width = (float)this.state.realSizes[index] - margin;
            numberLabel.Draw(currentRect, text, false, false, false, selected);
            currentRect.x -= margin;
        }

        protected void DrawText(Rect currentRect, string text, float margin, bool selected)
        {
            currentRect.x += margin;
            currentRect.width -= margin;
            numberLabel.Draw(currentRect, text, false, false, false, selected);
        }

        private void DrawItem(UIListItem item,int rowCount,float width, bool isSelected, int id)
        {
            Event current = Event.current;
            Rect rowRect = this.GetRowRect(rowCount, width);
            Rect position = rowRect;
            GUIStyle rowBackgroundStyle = this.GetRowBackgroundStyle(rowCount);
            bool bSelectedChange = false;

            if (current.type == EventType.Repaint)
            {
                rowBackgroundStyle.Draw(position, GUIContent.none, false, false, isSelected, false);
            }

            float num = (float)item.depth * 16f + 4f;

            if (item.hasChildren)
            {
                bool flag = IsExpanded(item);
                bool changed = GUI.changed;
                GUI.changed = false;
                num -= 14f;
                Rect position2 = new Rect(num, position.y, 14f, itemHeight);
                flag = GUI.Toggle(position2, flag, GUIContent.none, foldout);
                if (GUI.changed)
                {
                    item.isexpand = flag;
                    visibledirty = true;
                    Repaint();
                    return;
                }
                GUI.changed |= changed;
                num += 16f;
            }
            
            if (current.type == EventType.MouseDown && EditorWindow.focusedWindow == editorWindow)
            {

                if (rowRect.Contains(current.mousePosition))
                {
                    if (current.modifiers == EventModifiers.Control)
                    {
                        if (!selectedItems.Contains(item))
                        {
                            selectedItems.Add(item);
                            if (onSelectionsChange != null)
                            {
                                onSelectionsChange(selectedItems.ToArray());
                            }
                        }
                        else
                        {
                            selectedItems.Remove(item);
                            if (onSelectionsChange != null)
                            {
                                onSelectionsChange(selectedItems.ToArray());
                            }
                        }
                        bSelectedChange = true;
                        current.Use();
                    }
                    else if(current.modifiers == EventModifiers.Shift && selectedItems.Count > 0)
                    {
                        var firstItem = selectedItems[0];
                        if (firstItem != null)
                        {
                            selectedItems.Clear();
                            bool isStart= false;
                            for (int i = 0; i < visible_rows.Count; ++i)
                            {
                                bool isChange = false;
                                if (item == visible_rows[i] || firstItem == visible_rows[i])
                                {
                                    isChange = true;
                                    isStart = !isStart;
                                }
                                if (isStart || isChange)
                                {
                                    selectedItems.Add(visible_rows[i]);
                                }
                            }
                            
                            if (onSelectionsChange != null)
                            {
                                onSelectionsChange(selectedItems.ToArray());
                            }
                        }
                    }
                    else
                    {
                        if (iSelected != rowCount
                            || selected != item)
                        {
                            selectedItems.Clear();
                            selectedItems.Add(item);
                            if (onSelectionsChange != null)
                            {
                                onSelectionsChange(selectedItems.ToArray());
                            }
                            bSelectedChange = true;
                            iSelected = rowCount;
                            selected = item;
                            current.Use();
                        } 
                    }
                }
            }
            if (Event.current.type == EventType.MouseDrag && EditorWindow.focusedWindow == editorWindow)
            {
                if (rowRect.Contains(Event.current.mousePosition))
                {
                    if(dragCallBack != null)
                    {
                        dragCallBack(item);
                    }
                }
            }

            
            if (bSelectedChange)
            {
                Repaint();
                return;
            }

            if (current.type == EventType.Repaint)
            {
                numberLabel.alignment = TextAnchor.MiddleLeft;
                if(state != null)
                {
                    this.DrawTextColumn(ref position, visible_rows[rowCount].getText(0), 0, treeStyle ? num : 4, isSelected);
                    int num2 = 1;
                    for (int i = 1; i < buttonTitles.Length; i++)
                    {
                        position.x += (float)state.realSizes[num2 - 1];
                        position.width = (float)state.realSizes[num2] - 4f;
                        num2++;
                        numberLabel.Draw(position, visible_rows[rowCount].getText(i), false, false, false, isSelected);
                    }
                }
                else
                {
                    this.DrawText(position, visible_rows[rowCount].getText(0),treeStyle ? num : 4, isSelected);
                }
            }
        }

 
        bool IsRowVisible(int num)
        {
            return (this.ScrollViewHeight == 0 || (
                ((float)num * itemHeight <= (float)this.ScrollViewHeight + this.scrollPos.y) && 
                ((float)(num + 1) * itemHeight > this.scrollPos.y) )
                );
        }

        bool IsFullVisible(int num)
        {
            return (this.ScrollViewHeight == 0 
                || (
                ((float)(num + 1) * itemHeight <= (float)this.ScrollViewHeight + this.scrollPos.y)
                && ((float)(num) * itemHeight >= this.scrollPos.y))
                );
        }

        void MoveSelection(bool bUp)
        {
            // 多选状态下禁用
            if(selectedItems.Count > 1)
                return;
            
            int iPreSelected = iSelected;
            if (iSelected < 0 || iSelected >= visible_rows.Count)
            {
                return;
            }

            if(visible_rows.Count <= 0)
            {
                return;
            }

            if (CheckSelectedItem() == false)
            {
                iSelected = 0;
            }
            else
            {
                if (bUp)
                {
                    iSelected--;
                }
                else
                {
                    iSelected++;
                }
            }

            iSelected = Mathf.Max(Mathf.Min(iSelected, visible_rows.Count - 1), 0);
            selected = visible_rows[iSelected];
            EnsureSelectedShow();

            if(iPreSelected != iSelected)
            {
                if(onSelectionsChange != null)
                {
                    onSelectionsChange(selectedItems.ToArray());
                }
            }
        }
        bool selectedshowdirty = false;

        public void EnsureSelectedShow()
        {
            selectedshowdirty = true;
        }

        public void MultiSelect(UIListItem item)
        {
            if (!selectedItems.Contains(item))
            {
                selectedItems.Add(item);
            }
            else
            {
                selectedItems.Remove(item);
            }
            
            if (onSelectionsChange != null)
            {
                onSelectionsChange(selectedItems.ToArray());
            }
            Repaint();
        }
        
        public void UpdateSelectedShow()
        {
            if(selectedshowdirty && Event.current.type == EventType.Repaint)
            {
                selectedshowdirty = false;

                if (iSelected >= 0 && iSelected < visible_rows.Count)
                {
                    if (!IsFullVisible(iSelected))
                    {
                        if ((float)(iSelected) * itemHeight < this.scrollPos.y)
                        {
                            this.scrollPos.y = (float)(iSelected) * itemHeight;
                        }
                        else if (((float)(iSelected + 1) * itemHeight > (float)this.ScrollViewHeight + this.scrollPos.y))
                        {
                            this.scrollPos.y = (float)(iSelected + 1) * itemHeight - (float)this.ScrollViewHeight;
                        }
                    }
                }
            }
        }
        void HandleMouse(Event current,Rect pos)
        {
            if (current.type == EventType.MouseDown && EditorWindow.focusedWindow == editorWindow)
            {
                if (pos.Contains(current.mousePosition))
                {
                    //GUIUtility.keyboardControl = ID;
                    //GUIUtility.hotControl = ID;
                    keyBoardFocus = true;
                    GUI.FocusControl("");
                }
                else
                {
                    keyBoardFocus = false;
                }
            }
            else if(current.type == EventType.MouseUp &&  EditorWindow.focusedWindow == editorWindow)
            {
                if(GUIUtility.hotControl == ID)
                {
                   GUIUtility.hotControl = 0;
                }
            }
        }

        private void HandleCommandEvents(int ControlID)
		{
			if (GUIUtility.keyboardControl != ControlID)
			{
				return;
			}
			EventType type = Event.current.type;
			if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
			{
				bool flag = type == EventType.ExecuteCommand;
				if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
				{
					Event.current.Use();
					if (flag)
					{
						//ProjectBrowser.DeleteSelectedAssets(true);
					}
				}
				else
				{
					if (Event.current.commandName == "Duplicate")
					{
						Event.current.Use();
						if (flag)
						{
							//ProjectWindowUtil.DuplicateSelectedAssets();
						}
					}
				}
			}
		}
        
        void HandleKeyboard(Event current)
        {
            if (current.type == EventType.KeyDown )
            {
                if(keyBoardFocus == false)
                {
                    return;
                }

                if (current.keyCode == KeyCode.UpArrow)
                {
                    MoveSelection(true);
                    current.Use();
                    return;
                }
                else if (current.keyCode == KeyCode.DownArrow)
                {
                    MoveSelection(false);
                    current.Use();
                    return;
                }


                if (Event.current.keyCode == KeyCode.Delete)
                {
                    if (deleteCallBack != null)
                    {
                        deleteCallBack(selected);
                        current.Use();
                        return;
                    }
                }

                if (Event.current.keyCode == KeyCode.D && Event.current.shift)
                {
                    if(duplicateCallBack != null)
                    {
                        duplicateCallBack(selected);
                        current.Use();
                        return;
                    }
                }
            }
        }
        
        public void OnLayoutGUI(float height = 0.0f,float width = 0.0f)
        {
            Rect rt = GUILayoutUtility.GetRect(1, height);

            if(height != 0.0f)
            {
                rt.height = height;
            }
            
            if(width != 0.0f)
            {
                rt.width = width;
            }

            OnGUI(rt);
        }
        bool CheckSelectedItem()
        {
            return selected == visible_rows[iSelected];
        }

        public void OnGUI(Rect pos)
        {
            UpdateStyle();

            Event current = Event.current;

            ID = GUIUtility.GetControlID(ListBoxHash, FocusType.Passive);

            HandleMouse(current, pos);

            if (current.type == EventType.Repaint)
            {
                background.Draw(pos, GUIContent.none, false, false, GUIUtility.hotControl == ID, false);
            }

            GetVisibleRows();

            pos.x += 1;
            pos.y += 1;
            pos.width -= 2;
            pos.height -= 2;

            groupRect = pos;
            groupRect.width -= scrollMargin;
            titleRect = groupRect;
            titleRect.height = titleHeight;
            viewRect = groupRect;
            viewRect.y = titleHeight;
      
            if (state != null)
            {
                EditorUI.SplitterGUILayout.BeginHorizontalSplit(ref state, pos, titleHeight);
                int x = 0;
                for (int i = 0; i < buttonTitles.Length; ++i)
                {
                    if( GUI.Button(new Rect(x, 0, state.realSizes[i], titleHeight), buttonTitles[i], EditorStyles.toolbarButton) )
                    {
                        if(onClickTitle != null)
                        {
                            onClickTitle(this,i);
                        }
                    }
                    x += state.realSizes[i];
                }
            }
            else
            {
                titleHeight = 0;
                GUI.BeginGroup(pos);
            }
           
             
            Rect itemRect = groupRect;
            itemRect.x = 0;
            itemRect.width += scrollMargin;
            itemRect.y = titleHeight + 1;
            itemRect.height = groupRect.height - titleHeight - 1;
            Rect itemViewRect = itemRect;
            itemViewRect.width -= (scrollMargin + 2);
            itemViewRect.height = Mathf.Max(visible_rows.Count * itemHeight, itemRect.height);
            ScrollViewHeight = (int)itemRect.height;
            HandleKeyboard(current);
            if (visibledirty)
            {
                goto end2;
            }
            UpdateSelectedShow();

          

            scrollPos = GUI.BeginScrollView(itemRect, scrollPos, itemViewRect,false,true);

          
            for (int i = 0; i < visible_rows.Count; ++i)
            {
                if (IsRowVisible(i))
                {
                    var item = visible_rows[i];
                    DrawItem(item, i, groupRect.width, selectedItems.Contains(item), ID);

                    if( visibledirty )
                    {
                        goto end;
                    }
                }
            }

       end:
            GUI.EndScrollView();
       end2:
            if (state != null)
            {
                EditorUI.SplitterGUILayout.EndHorizontalSplit();
            }
            else
            {
                GUI.EndGroup();
            }
        }
    }
}
