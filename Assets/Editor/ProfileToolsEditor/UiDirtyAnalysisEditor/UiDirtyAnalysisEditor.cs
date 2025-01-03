using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UiDirtyAnalysis))]
public class UiDirtyAnalysisEditor : Editor
{
    private UiDirtyAnalysis dirtyAnalysis;
    private void OnEnable()
    {
        dirtyAnalysis = target as UiDirtyAnalysis;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginVertical();
        if (GUILayout.Button("监听Canvas刷新"))
        {
            dirtyAnalysis.RegisterAll();
        }
        if (GUILayout.Button("取消Canvas监听"))
        {
            dirtyAnalysis.CancelAllRegister();
        }
        GUILayout.EndVertical();
    }
}