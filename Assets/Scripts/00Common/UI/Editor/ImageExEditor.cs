using System.Linq;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Sprites.
    /// </summary>
     
    [CustomEditor(typeof(ImageEx), true)]
    [CanEditMultipleObjects]
    public class ImageExEditor : ImageEditor
    {
        SerializedProperty m_IsEnableWhiteImage;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_IsEnableWhiteImage = serializedObject.FindProperty("m_IsEnableWhiteImage"); 
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_IsEnableWhiteImage, new GUIContent("空图模式"));
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    
    }  
}
