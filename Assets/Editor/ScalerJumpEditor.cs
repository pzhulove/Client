using UnityEngine;
using System.Collections;
using UnityEditor;

namespace GameClient
{
    [CustomEditor(typeof(ScalerJump))]
    class ScalerJumpEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("play"))
            {
                (target as ScalerJump).StartJumpNormal(1);
            }
            if (GUILayout.Button("play_continue"))
            {
                (target as ScalerJump).StartJumpContinue(1,10);
            }
        }
    }
}