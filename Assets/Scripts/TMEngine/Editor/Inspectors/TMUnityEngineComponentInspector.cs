
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Tenmove.Runtime.Unity;

namespace Tenmove.Editor.Unity
{
    [CustomEditor(typeof(TMUnityEngineComponent))]
    internal sealed class TMUnityEngineComponentInspector : ComponentInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        private void OnEnable()
        {

        }
    }
}
