using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class ComArrayListDrawer
{
    public GUIContent iconToolbarPlus;
    public GUIContent iconToolbarPlusMore;
    public GUIContent iconToolbarMinus;
    public GUIStyle draggingHandle;
    public GUIStyle headerBackground;
    public GUIStyle footerBackground;
    public GUIStyle boxBackground;
    public GUIStyle preButton;
    public GUIStyle elementBackground;
    public GUIStyle elementBackgourndFocus;

    private bool bInit = false;

    Editor mEditor;
    public void Init() {
        CreateFontStyle();
        if(bInit == true) return;
        bInit = true;
        iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
        iconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");
        iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
        elementBackground = new GUIStyle("RL Element");
        draggingHandle = "RL DragHandle";
        headerBackground = "RL Header";
        footerBackground = "RL Footer";
        boxBackground = "RL Background";
        preButton = "RL FooterButton";
        elementBackgourndFocus = new GUIStyle("RL Element");
        elementBackgourndFocus.normal.background = elementBackgourndFocus.focused.background;
    }

    GUIStyle fontstyleUnitInfo;

    void CreateFontStyle()
    {
    
        if (fontstyleUnitInfo == null)
        {
            fontstyleUnitInfo = new GUIStyle();
            fontstyleUnitInfo.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleUnitInfo.fontSize = 18;
            //fontstyleUnitInfo.fontStyle = FontStyle.Bold;
            fontstyleUnitInfo.alignment = TextAnchor.MiddleLeft;
            fontstyleUnitInfo.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleUnitInfo.hover.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
        }
 
    }

    private Rect BodyRect;
    private Rect HeadRect;
    private Rect[] elemRects;

    private int   currentSelected = -1;
    private int   currentIndex = -1;

    public int selected{
        get{
            return currentSelected;
        }
    }

    public Rect[] ElemRects
    {
        get { return elemRects; }
    }

    public bool dirty = false;

    public void DrawHeader(string Title,int count,Editor editor)
    {
        Init();
        mEditor = editor;

        if (string.IsNullOrEmpty(Title) == false)
        {
            EditorGUILayout.BeginVertical();
            /*if (Event.current.type == EventType.Layout)
            {
                HeadRect = GUILayoutUtility.GetRect(1,1);
                //this.boxBackground.Draw(BodyRect, false, true, true, false);
            }

            if (Event.current.type == EventType.Repaint)
            {
                this.headerBackground.Draw(HeadRect, false, true, true, false);
            }*/

            EditorGUILayout.LabelField(Title, fontstyleUnitInfo);
            EditorGUILayout.EndVertical();
        }
        

        
        EditorGUILayout.BeginVertical(boxBackground);
        EditorGUI.indentLevel+=1;

        if(elemRects == null || elemRects.Length != count)
        {
            elemRects = new Rect[count];
        }

        if(BodyRect.size == Vector2.zero || dirty)
        {
            //dirty = false;
            //editor.Repaint();
        }
    }

    public void BeginDrawElement(int index)
    {
        currentIndex = index;

        if (Event.current.button == 0 && Event.current.clickCount == 1)
        {
            if (elemRects[currentIndex].Contains(Event.current.mousePosition))
            {
                if(currentSelected != currentIndex) mEditor.Repaint();
                 currentSelected = currentIndex;
                 //Event.current.Use();
            }
            else if(BodyRect.Contains(Event.current.mousePosition) == false)
            {
                currentSelected = -1;
            }
        }

        if (Event.current.type == EventType.Repaint && currentIndex == currentSelected)
		{
			this.elementBackground.Draw(elemRects[currentIndex], false, true, true,false);
		}
        if(index != 0)EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        
    }

    public void FocusElementBackground(Rect eleRect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (eleRect == null)
            {
                return;
            }
            this.elementBackground.Draw(eleRect, false, false, false, true);
        }
    }

    enum EditType
    {
        none,
        add,
        remove,
        moveup,
        movedown
    }

    EditType mEditType;
    public void EndDrawElement()
    {
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        
        if(Event.current.type == EventType.Repaint)
        {
            elemRects[currentIndex] = GUILayoutUtility.GetLastRect();
        }
        

    }

    private static int IDHash = "ComArrayListDrawer".GetHashCode();
    public delegate T0 UnityActionEx<T0>(T0 arg0);
    public bool DrawFooter<T>(ref T[] array,UnityActionEx<T> onNew = null) where T :  new()
    {
        
        if (Event.current.type == EventType.ContextClick && Event.current.button == 1 && Event.current.clickCount == 1)
        {
            for(int i = 0; i < array.Length; ++i)
            if (elemRects[i].Contains(Event.current.mousePosition))
            {
                 currentSelected = i;
                 GUIUtility.hotControl = IDHash;
                 GenericMenu menu = new GenericMenu();
                 menu.AddItem(new GUIContent("Add"), false, (value)=>{mEditType = EditType.add;}, null);
                 menu.AddItem(new GUIContent("Remove"), false, (value)=>{mEditType = EditType.remove;}, null);
                 menu.AddSeparator("");
                 menu.AddItem(new GUIContent("MoveUp"), false, (value)=>{mEditType = EditType.moveup;}, null);
                 menu.AddItem(new GUIContent("MoveDown"), false, (value)=>{mEditType = EditType.movedown;}, null);
                 menu.ShowAsContext();
                 Event.current.Use();
                 break;
            }
            
        }  


        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button(iconToolbarPlus))
        {
            mEditType = EditType.add;
        }
        
        if(GUILayout.Button(iconToolbarMinus))
        {
           mEditType = EditType.remove;
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;
        if(Event.current.type == EventType.Repaint)
        {
            BodyRect = GUILayoutUtility.GetLastRect();
        }
        
        switch(mEditType)
        {
            case EditType.add:
                {
                    T newData = new T();
                    if(onNew != null)
                    {
                        newData = onNew(newData);
                    }

                    if (null == array || array.Length <= 0)
                    {
                        array =  new T[] { newData };
                        currentSelected = 0;
                    }
                    else
                    {
                        if (currentSelected < 0 || currentSelected >= array.Length) 
                        {
                            currentSelected = array.Length - 1;
                        }

                        currentSelected = currentSelected + 1;
                        array = DArrayTools.InsertElement(array, newData, currentSelected);                       
                    }
                    //mEditor.Repaint();
                }
            break;
            case EditType.remove:
                {
                    if (currentSelected >= 0 && currentSelected < array.Length)
                        ArrayUtility.RemoveAt(ref array, currentSelected);
                    currentSelected = currentSelected - 1;
                    if (currentSelected < 0) currentSelected = 0;
                    //mEditor.Repaint();
                }
                break;
            case EditType.moveup:
                {
                    array = DArrayTools.MoveElement(array,ref currentSelected,true);
                }
                break;
            case EditType.movedown:
                {
                    array = DArrayTools.MoveElement(array,ref currentSelected,false);
                }
                break;
            
        }

        bool result = mEditType != EditType.none;
        mEditType = EditType.none;

        return result;
    }
    public void DrawFooter<T>(ref List<T> list,UnityAction OnAdd,UnityAction<int> OnDelete)
    {
         if (Event.current.type == EventType.ContextClick && Event.current.button == 1 && Event.current.clickCount == 1)
        {
            for(int i = 0; i < list.Count; ++i)
            if (elemRects[i].Contains(Event.current.mousePosition))
            {
                 currentSelected = i;
                 GUIUtility.hotControl = IDHash;
                 GenericMenu menu = new GenericMenu();
                 menu.AddItem(new GUIContent("Add"), false, (value)=>{mEditType = EditType.add;}, null);
                 menu.AddItem(new GUIContent("Remove"), false, (value)=>{mEditType = EditType.remove;}, null);
                 menu.AddSeparator("");
                 menu.AddItem(new GUIContent("MoveUp"), false, (value)=>{mEditType = EditType.moveup;}, null);
                 menu.AddItem(new GUIContent("MoveDown"), false, (value)=>{mEditType = EditType.movedown;}, null);
                 menu.ShowAsContext();
                 Event.current.Use();
                 break;
            }
            
        }  


        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button(iconToolbarPlus))
        {
            mEditType = EditType.add;
        }
        
        if(GUILayout.Button(iconToolbarMinus))
        {
           mEditType = EditType.remove;
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;
        if(Event.current.type == EventType.Repaint)
        {
            BodyRect = GUILayoutUtility.GetLastRect();
        }

        switch(mEditType)
        {
            case EditType.add:
                {
                    OnAdd();
                }
            break;
            case EditType.remove:
                {
                    if (currentSelected >= 0 && currentSelected < list.Count)
                         OnDelete(currentSelected);
                    currentSelected = currentSelected - 1;
                    if (currentSelected < 0) currentSelected = 0;
                }
                break;
        }
        mEditType = EditType.none;
    }
}
