using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComGraphicControl))]
public class ComGraphicControlInspector : ComEditor
{
    
    void EnumToggle(ref ComGraphicControl.GraphicControlEnum value,ComGraphicControl.GraphicControlEnum key,string nameshow)
    {
       bool bHave = (value == key);
       bool bNew = EditorGUILayout.Toggle(nameshow,bHave);

       if(bNew != bHave)
       {
           if(bNew)
           {
                value = key;
           }
       }
    }
    public override void OnInspectorGUI()
    {
        OnBaseInspectorGUI();
        ComGraphicControl data = this.target as ComGraphicControl;
        EditorGUILayout.LabelField("图形品质开关", fontStyle);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EnumToggle(ref data.controlEnum,ComGraphicControl.GraphicControlEnum.High,"高配关闭");
        EnumToggle(ref data.controlEnum,ComGraphicControl.GraphicControlEnum.Mid,"中配关闭");
        EnumToggle(ref data.controlEnum,ComGraphicControl.GraphicControlEnum.Low,"低配关闭");
        EnumToggle(ref data.controlEnum,ComGraphicControl.GraphicControlEnum.VeryLow,"超低配关闭"); 
        EditorGUILayout.EndVertical();
    }
}