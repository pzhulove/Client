
using UnityEditor;
using Tenmove.Runtime.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;

namespace Tenmove.Editor.Unity
{
    [CustomEditor(typeof(TMGameProfileModuleComponent))]
    internal sealed class GameProfileModuleComponentInspector : ComponentInspector
    {
        private SerializedProperty m_EnableProfiler = null;

        private void OnEnable()
        {
            m_EnableProfiler = serializedObject.FindProperty("m_EnableProfiler");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            TMGameProfileModuleComponent t = (TMGameProfileModuleComponent)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField( m_EnableProfiler,new GUIContent("Enable Profiler:"));

            if (EditorGUI.EndChangeCheck())
            {
                if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                    EditorSceneManager.MarkSceneDirty(PrefabStageUtility.GetCurrentPrefabStage().scene);
                else
                    EditorUtility.SetDirty(t);
                serializedObject.ApplyModifiedProperties();
            }

            if (EditorApplication.isPlaying)
                EditorGUILayout.HelpBox("Simulator can not enable or disable during runtime.", MessageType.Info);
        }
    }
}