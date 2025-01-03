using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ActiveVarBinder))]
public class UIActionEventEditor : Editor
{
    SerializedProperty m_serializedOnSuccessTrigger;
    ConditionTrigger m_OnSuccessTrigger;

    SerializedProperty m_serializedOnFailedTrigger;
    ConditionTrigger m_OnFailedTrigger;

    void OnEnable()
    {
        m_serializedOnSuccessTrigger = serializedObject.FindProperty("m_OnSuccessTrigger");
        m_serializedOnFailedTrigger = serializedObject.FindProperty("m_OnFailedTrigger");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //serializedObject.Update();

        //ActiveVarBinder plug = target as ActiveVarBinder;

        //plug.m_iDataID = EditorGUILayout.IntField("m_iDataID", plug.m_iDataID);
        //plug.m_kKey = EditorGUILayout.TextField("m_kKey", plug.m_kKey);
        //plug.m_iCompareValue = EditorGUILayout.IntField("m_iCompareValue", plug.m_iCompareValue);

        //EditorGUILayout.PropertyField(m_serializedOnSuccessTrigger, true);
        //EditorGUILayout.PropertyField(m_serializedOnFailedTrigger, true);

        //serializedObject.ApplyModifiedProperties();
    }
}