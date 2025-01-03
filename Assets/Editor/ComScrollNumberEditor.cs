using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace GameClient
{
    [CustomEditor(typeof(ComScrollNumber))]
    class ComScrollNumberEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("play")))
            {
                (target as ComScrollNumber).Run(5,9);
            }
        }
    }
}