using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace GameClient
{
    [CustomEditor(typeof(ComRollNumber))]
    class ComRollNumberEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("play")))
            {
                (target as ComRollNumber).SetEditorValue();
            }
        }
    }
}