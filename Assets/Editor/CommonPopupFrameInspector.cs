using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CommonPopupFrame))]
public class FrameBgInspector : Editor
{
    CommonPopupFrame.SizeType sizeType;
    string frameTitle = "";
    bool needHelpBtn = false;
    bool needCloseBtn = false;

    private void OnEnable()
    {
        CommonPopupFrame data = target as CommonPopupFrame;
        needHelpBtn = data.GetHelpBtnShowState();
        needCloseBtn = data.GetCloseBtnShowState();
    }

    public override void OnInspectorGUI()
    {
        CommonPopupFrame data = target as CommonPopupFrame;

        base.OnInspectorGUI();  

        EditorGUI.BeginChangeCheck();
        GUILayout.Label("设置界面名称");
        frameTitle = EditorGUILayout.TextField(frameTitle);
        if(EditorGUI.EndChangeCheck())
        {
            data.SetFrameTitle(frameTitle);
        }

        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("是否显示帮助按钮");
        needHelpBtn = EditorGUILayout.Toggle(needHelpBtn);
        if (EditorGUI.EndChangeCheck())
        {
            data.ShowHelpBtn(needHelpBtn);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("是否显示关闭按钮");
        needCloseBtn = EditorGUILayout.Toggle(needCloseBtn);
        if (EditorGUI.EndChangeCheck())
        {
            data.ShowCloseBtn(needCloseBtn);
        }
        GUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();        
        sizeType = (CommonPopupFrame.SizeType)EditorGUILayout.EnumPopup("选择大小类型", sizeType);
        if(EditorGUI.EndChangeCheck())
        {
            data.SetSize(sizeType);
        }      

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);            
        }        
    }
}
