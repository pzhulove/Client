using UnityEditor;
using UnityEditor.UI;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;



namespace UnityEditor.UI
{
    //	[ExecuteInEditMode]
    [CustomEditor(typeof(GeUISwitchButton), true)]
    public class GeUISwitchButtonEditor : SelectableEditor
    {
        private SerializedObject m_Object;

        SerializedProperty m_Direction;
        SerializedProperty m_FillRect;
        SerializedProperty m_HandleRect;
        SerializedProperty m_HandleImage;
        SerializedProperty m_HandleText;


        SerializedProperty m_OnImage;
        SerializedProperty m_OnText;
        SerializedProperty m_States;
        SerializedProperty m_OnValueChanged;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Object = new SerializedObject(target);

            //editor update
            //m_editorTime = m_Object.FindProperty("m_editorEmitterTime");

            //read only
            m_Direction = m_Object.FindProperty("m_Direction");
            m_FillRect = m_Object.FindProperty("m_FillRect");
            m_HandleRect = m_Object.FindProperty("m_HandleRect");
            m_HandleImage = m_Object.FindProperty("m_HandleImage");
            m_HandleText = m_Object.FindProperty("m_HandleText");
            m_OnImage = m_Object.FindProperty("m_OnImage");
            m_OnText = m_Object.FindProperty("m_OnText");
            m_States = m_Object.FindProperty("m_States");
            m_OnValueChanged = m_Object.FindProperty("m_OnValueChanged");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override bool HasPreviewGUI() { return true; }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            serializedObject.Update();

            GeUISwitchButton switchBtn = target as GeUISwitchButton;
            EditorGUI.BeginChangeCheck();
            
            switchBtn.handleImage = EditorGUILayout.ObjectField("Handle Image", switchBtn.handleImage, typeof(Image)) as Image;
            switchBtn.handleText = EditorGUILayout.ObjectField("Handle Text", switchBtn.handleText, typeof(Text)) as Text;

            switchBtn.onImage = EditorGUILayout.ObjectField("On Image", switchBtn.onImage, typeof(Sprite)) as Sprite;
            switchBtn.onText = EditorGUILayout.TextField("On Text", switchBtn.onText);
            switchBtn.offImage = EditorGUILayout.ObjectField("Off Image", switchBtn.offImage, typeof(Sprite)) as Sprite;
            switchBtn.offText = EditorGUILayout.TextField("Off Text", switchBtn.offText);

            m_States.boolValue = EditorGUILayout.Toggle("IsOn", m_States.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                switchBtn = target as GeUISwitchButton;
                switchBtn.SetSwitch(m_States.boolValue);
                switchBtn.RefreshUI();
            }

            EditorGUILayout.PropertyField(m_FillRect);
            EditorGUILayout.PropertyField(m_HandleRect);
            if (m_FillRect.objectReferenceValue != null || m_HandleRect.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Direction);
                if (EditorGUI.EndChangeCheck())
                {
                    GeUISwitchButton.Direction direction = (GeUISwitchButton.Direction)m_Direction.enumValueIndex;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        switchBtn = obj as GeUISwitchButton;
                        switchBtn.SetDirection(direction, true);
                    }
                }

                bool warning = false;
                foreach (var obj in serializedObject.targetObjects)
                {
                    switchBtn = obj as GeUISwitchButton;
                    GeUISwitchButton.Direction dir = switchBtn.direction;
                    if (dir == GeUISwitchButton.Direction.LeftToRight || dir == GeUISwitchButton.Direction.RightToLeft)
                        warning = (switchBtn.navigation.mode != Navigation.Mode.Automatic && (switchBtn.FindSelectableOnLeft() != null || switchBtn.FindSelectableOnRight() != null));
                    else
                        warning = (switchBtn.navigation.mode != Navigation.Mode.Automatic && (switchBtn.FindSelectableOnDown() != null || switchBtn.FindSelectableOnUp() != null));
                }

                if (warning)
                    EditorGUILayout.HelpBox("The selected slider direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);

                // Draw the event notification options
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the slider fill or the slider handle or both. Each must have a parent RectTransform that it can slide within.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
