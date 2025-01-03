using UnityEngine;
using System.Collections;
using UnityEditor;

namespace GameClient
{
    [CustomEditor(typeof(NewSuperLinkText))]
    class NewSuperLinkTextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}