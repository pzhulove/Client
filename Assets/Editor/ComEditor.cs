using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System.Diagnostics;
using System.Reflection;

public class ComEditor : Editor
{

    protected List<string> mCacheString = new List<string>();


    protected Vector2 mScrollSprites = Vector2.zero;
    
    protected GUIStyle fontstyleWarnning;
    protected GUIStyle fontstyleUnitInfo;
    protected GUIStyle fontstyleUnitInfoSelect;
    protected GUIStyle headerStyle;
    protected GUIStyle radioButtonStyle;
    protected GUIStyle fontStyle;

    void CreateFontStyle()
    {
        if (fontStyle == null)
        {
            fontStyle = new GUIStyle();
            fontStyle.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontStyle.fontSize = 14;
            fontStyle.alignment = TextAnchor.UpperLeft;
            fontStyle.normal.textColor = Color.green;
            fontStyle.hover.textColor = Color.green;
        }

        if (fontstyleWarnning == null)
        {
            fontstyleWarnning = new GUIStyle(EditorStyles.label);
            fontstyleWarnning.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleWarnning.fontSize = 12;
            fontstyleWarnning.alignment = TextAnchor.LowerLeft;
            fontstyleWarnning.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.hover.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.active.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleWarnning.focused.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
        }

        if (fontstyleUnitInfo == null)
        {
            fontstyleUnitInfo = new GUIStyle(EditorStyles.label);
            fontstyleUnitInfo.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleUnitInfo.fontSize = 12;
            //fontstyleUnitInfo.fontStyle = FontStyle.Bold;
            fontstyleUnitInfo.alignment = TextAnchor.MiddleLeft;
            fontstyleUnitInfo.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            fontstyleUnitInfo.hover.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
        }

        if(fontstyleUnitInfoSelect == null)
        {
            fontstyleUnitInfoSelect = new GUIStyle(EditorStyles.label);
            fontstyleUnitInfoSelect.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontstyleUnitInfoSelect.fontSize = 12;
            //fontstyleUnitInfo.fontStyle = FontStyle.Bold;
            fontstyleUnitInfoSelect.alignment = TextAnchor.MiddleLeft;
            fontstyleUnitInfoSelect.normal.textColor = Color.green;
            fontstyleUnitInfoSelect.hover.textColor = Color.green;
        }

        // if(deStylePalette == null)
        // {
        //     deStylePalette = new DG.DemiEditor.DeStylePalette();
        //     var type = deStylePalette.GetType();
        //     MethodInfo info = type.GetMethod("Init",
        //         BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

        //     info.Invoke(deStylePalette,new object[0]);
        // }

        if(deToggleButtonStyle == null)
        {
            deToggleButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
            deToggleButtonStyle.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
        }

        if(headerStyle == null)
        {
            headerStyle = "RL Header";
        }
 
        if(radioButtonStyle == null)
        {
			radioButtonStyle = new GUIStyle(EditorStyles.miniButton);//new GUIStyle(DG.DemiEditor.DeGUI.styles.button.def);
        }
    }
 

    //protected  DG.DemiEditor.DeStylePalette    deStylePalette;
    protected GUIStyle                        deToggleButtonStyle;

  
    public bool ToggleButton(bool bCheck,GUIContent content,GUIStyle style)
    {
        var back = GUI.backgroundColor;
        var colorPalete = DG.DemiEditor.DeGUI.colors;
        if(colorPalete == null)
        {
            Repaint();
            //return bCheck;
        }
        else
        {
            GUI.backgroundColor = bCheck ? Color.green : (Color)colorPalete.bg.toggleOff;
            style.normal.textColor =  bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
            style.active.textColor =  bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
        } 
       
        bool flag = GUILayout.Button(content,style);
        if(flag)
        {
            bCheck = !bCheck;
            GUI.changed = true;
        }
        GUI.backgroundColor = back;

        return bCheck;
    }
    public bool ToggleButton(bool bCheck,GUIContent content)
    {
        var back = GUI.backgroundColor;
        var colorPalete = DG.DemiEditor.DeGUI.colors;
        if(colorPalete == null)
        {
            Repaint();
            //return bCheck; 
        }
        else
        {
            GUI.backgroundColor = bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
            deToggleButtonStyle.normal.textColor =  bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
            deToggleButtonStyle.active.textColor =  bCheck ?  Color.green : (Color)colorPalete.bg.toggleOff;
        }
       
        deToggleButtonStyle.fontSize = 14;
        bool flag = GUILayout.Button(content,deToggleButtonStyle);
        if(flag)
        {
            bCheck = !bCheck;
            GUI.changed = true;
        }
        GUI.backgroundColor = back;

        return bCheck;
    }
 
    public  void DrawBox(Color c,int height)
    {
        var back = GUI.backgroundColor;
        GUI.backgroundColor = c;
        Rect rt = GUILayoutUtility.GetRect(1,height);
        rt.height = height;
        GUI.Box(rt,"",headerStyle);
        GUI.backgroundColor = back;
    }

 
    public void OnBaseInspectorGUI()
    {        
        GUIControls.UIStyles.UpdateEditorStyles();
        CreateFontStyle();
    }
}
 