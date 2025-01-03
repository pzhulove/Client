using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GeUIEffectAttach))]
public class GeUIEffectAttachInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GeUIEffectAttach uiEffectAttach = target as GeUIEffectAttach;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Dummys");
        GUILayout.Label("UIEffects");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        for (int i = 0; i < uiEffectAttach.m_Dummys.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            uiEffectAttach.m_Dummys[i] = EditorGUILayout.ObjectField(uiEffectAttach.m_Dummys[i], typeof(RectTransform), true) as RectTransform;
            uiEffectAttach.m_UIEffects[i] = EditorGUILayout.ObjectField(uiEffectAttach.m_UIEffects[i], typeof(RectTransform), true) as RectTransform;
            if(GUILayout.Button("Remove"))
            {
                uiEffectAttach.DelItem(i); 
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Add"))
        {
            uiEffectAttach.AddNewItem();
        }

        EditorGUILayout.EndVertical();
    }
}
