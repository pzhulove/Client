using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(MirroEffect))]
public class MirroEffectInspector : Editor {

    public override void OnInspectorGUI()
    {
        MirroEffect mirroEffect = this.target as MirroEffect;

        mirroEffect.mtype = (MirroEffect.MirroType)EditorGUILayout.EnumPopup("镜像类型:", mirroEffect.mtype);

        mirroEffect.fValue.x = EditorGUILayout.FloatField("正向角度:", mirroEffect.fValue.x);
        mirroEffect.fValue.y = EditorGUILayout.FloatField("镜像角度:", mirroEffect.fValue.y);

        EditorGUILayout.LabelField("镜像测试");

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("正向", "miniButton"))
        {
            mirroEffect.Apply(false);
        }

        if (GUILayout.Button("镜像", "miniButton"))
        {
            mirroEffect.Apply(true);
        }

        EditorGUILayout.EndHorizontal();
    }
}
