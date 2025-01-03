using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UI;
using GameClient;


namespace Assets.Editor
{
    [CustomEditor(typeof(ComButtonEx))]
    class ButtonExEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            var button = this.target as ComButtonEx;
            button.penetrable = EditorGUILayout.Toggle("penetrable", button.penetrable);
            base.OnInspectorGUI();
        }
    }
}
