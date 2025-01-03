using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(HGButton), true)]
    [CanEditMultipleObjects]
    public class HGButtonEditor : ButtonEditor
    {
        SerializedProperty m_OnUpDownProperty;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnUpDownProperty = serializedObject.FindProperty("m_OnUpDown");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(m_OnUpDownProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
