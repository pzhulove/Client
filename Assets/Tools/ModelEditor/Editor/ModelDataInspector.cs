using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(DModelData))]
public class DModelDataInspector : Editor
{
#if !LOGIC_SERVER
    [MenuItem("[TM工具集]/ArtTools/模型编辑器", false, 1)]
    public static void CreateAsset()
    {
        ModelEditorPanel.Init(null,"");
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Editor"))
            ModelEditorPanel.Init(null,"");
    }
#endif
}
