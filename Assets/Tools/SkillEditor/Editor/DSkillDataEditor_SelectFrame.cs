using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public struct FramePickData
{
    public int minValue;
    public int maxValue;
    public int curValue;
    public int totalFrames;

    public delegate void CopyFrame(FramePickData data);
    public delegate void ClearFrame(FramePickData data);

    public CopyFrame  dCopy;
    public ClearFrame dClear;
}
public class PickFramePopupWindow : PopupWindowContent
{
    static PickFramePopupWindow _pickFramePopupWindow;

    // private string titleStyle;
    // private string removeButtonStyle;
    // private string addButtonStyle;
    private string borderBarStyle;
    // private string rootGroupStyle;
    // private string subGroupStyle;
    // private string arrayElementStyle;
    // private string subArrayElementStyle;
    // private string toggleStyle;
    // private string foldStyle;
    // private string enumStyle;
    // private string fillBarStyle1;
    private string fillBarStyle2;
    // private string fillBarStyle3;
    // private string fillBarStyle4;
    private GUIStyle labelStyle;
      
//     private int _minValue = 1;
//     private int _maxValue = 88;

    public FramePickData pickdata;

    void Init()   
    {
        //titleStyle = "MeTransOffRight";
        //removeButtonStyle = "TL SelectionBarCloseButton";
        //addButtonStyle = "CN CountBadge";
        //rootGroupStyle = "GroupBox";
        //subGroupStyle = "ObjectFieldThumb";
        //arrayElementStyle = "flow overlay box";
        //subArrayElementStyle = "HelpBox";
        //foldStyle = "Foldout";
        //enumStyle = "MiniPopup";
        //toggleStyle = "BoldToggle";
        borderBarStyle = "ProgressBarBack";
        //fillBarStyle1 = "ProgressBarBar";
        fillBarStyle2 = "flow node 2 on";

        labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = Color.white;
    }

    public static void ShowPopup(Rect boundRect,FramePickData _pick)
    {
        if(_pickFramePopupWindow == null)
        {
            _pickFramePopupWindow = new PickFramePopupWindow();
            _pickFramePopupWindow.Init();
        }
        _pickFramePopupWindow.pickdata = _pick;
        PopupWindow.Show(boundRect, _pickFramePopupWindow);
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(400, 80);
    }
 

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginVertical();
        {
            StyledMinMaxSlider("Frames", ref pickdata.minValue, ref pickdata.maxValue, 0, pickdata.totalFrames - 1, EditorGUI.indentLevel);

            GUILayout.BeginHorizontal();
            {
                if (StyledButton("当前帧"))
                {
                    pickdata.minValue = pickdata.curValue;
                    pickdata.maxValue = pickdata.curValue;
                }

                if (StyledButton("复制帧"))
                {
                    if(pickdata.dCopy != null)
                    {
                        pickdata.dCopy(pickdata);
                    }
                }

                if (StyledButton("清除帧"))
                {
                    if (pickdata.dClear != null)
                    {
                        pickdata.dClear(pickdata);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    public override void OnOpen()
    {
        Debug.Log("Popup opened: " + this);
    }

    public override void OnClose()
    {
        Debug.Log("Popup closed: " + this);
    }

    public bool StyledButton(string label)
    {
        EditorGUILayout.Space();
        GUILayoutUtility.GetRect(1, 20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool clickResult = GUILayout.Button(label, "Button");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        return clickResult;
    }
    public void StyledMinMaxSlider(string label, ref int minValue, ref int maxValue, int minLimit, int maxLimit, int indentLevel)
    {
        indentLevel = 1;
        int indentSpacing = 10 * indentLevel;
        //indentSpacing += 30;
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (minValue < 0) minValue = 0;
        if (maxValue < 0) maxValue = 0;
        if (maxValue > maxLimit) maxValue = maxLimit;

        float minValueFloat = (float)minValue;
        float maxValueFloat = (float)maxValue;
        float minLimitFloat = (float)minLimit;
        float maxLimitFloat = (float)maxLimit;

        Rect tempRect = GUILayoutUtility.GetRect(1, 10);

        Rect rect = new Rect(indentSpacing, tempRect.y, Screen.width - indentSpacing - 100, 20);
        //Rect rect = new Rect(indentSpacing + 15,tempRect.y, Screen.width - indentSpacing - 70, 20);
        float fillLeftPos = ((rect.width / maxLimitFloat) * minValueFloat) + rect.x;
        float fillRightPos = ((rect.width / maxLimitFloat) * maxValueFloat) + rect.x;
        float fillWidth = fillRightPos - fillLeftPos;

        //fillWidth += (rect.width / maxLimitFloat);
        //fillLeftPos -= (rect.width / maxLimitFloat);

        // Border
        GUI.Box(rect, "", borderBarStyle);

        // Overlay
        GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);

        // Text
        //GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        //centeredStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.alignment = TextAnchor.UpperCenter;
        GUI.Label(rect, label + " between " + Mathf.Floor(minValueFloat) + " and " + Mathf.Floor(maxValueFloat), labelStyle);
        labelStyle.alignment = TextAnchor.MiddleCenter;

        // Slider
        rect.y += 10;
        rect.x = indentLevel * 10;
        rect.width = (Screen.width - (indentLevel * 10) - 100);

        EditorGUI.MinMaxSlider(rect, ref minValueFloat, ref maxValueFloat, minLimitFloat, maxLimitFloat);
        minValue = (int)minValueFloat;
        maxValue = (int)maxValueFloat;

        tempRect = GUILayoutUtility.GetRect(1, 20);
    }
}
 
