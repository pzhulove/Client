using UnityEditor;
using UnityEngine;
using Scripts.UI;

[CustomEditor(typeof(ComUIListScript), true)]
public class ComUIListScriptInspector : UnityEditor.Editor
{
    protected virtual void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }
}