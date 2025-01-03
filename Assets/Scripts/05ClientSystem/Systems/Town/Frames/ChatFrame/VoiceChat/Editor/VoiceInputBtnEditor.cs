using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;


namespace GameClient
{

    [CustomEditor(typeof(VoiceInputBtn))]
    public class VoiceInputBtnEditor : Editor
    {
        SerializedProperty voiceInputType;
        SerializedProperty chatType;

        private void OnEnable() 
        {
            voiceInputType = serializedObject.FindProperty("voiceInputType");
            chatType = serializedObject.FindProperty("chatType");
        }

        public override void OnInspectorGUI()
        {
            if((VoiceInputBtn.VoiceInputType)voiceInputType.enumValueIndex == VoiceInputBtn.VoiceInputType.SingleChannel)
            {
                EditorGUILayout.PropertyField(chatType);
            }           
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
