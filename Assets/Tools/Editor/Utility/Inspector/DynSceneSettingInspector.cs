using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DynSceneSetting))]
public class DynSceneSettingInspector : UnityEditor.Editor
{
    [MenuItem("[关卡编辑器]/区域/生成光照信息数据")]
    public static void Create()
    {
        FileTools.CreateAsset<DynSceneSetting>("SceneSetting");
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Snap"))
        {
            DynSceneSetting setting = target as DynSceneSetting;
            setting.Snap();
        }

        if (GUILayout.Button("Apply"))
        {
            DynSceneSetting setting = target as DynSceneSetting;
            setting.Apply();
        }
    }
}