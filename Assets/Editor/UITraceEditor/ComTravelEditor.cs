using UnityEngine;
using System.Collections;
using UnityEditor;

namespace GameClient
{
    [CustomEditor(typeof(ComTravel))]
    public class ComTravelEditor : Editor
    {
        public void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("play"))
            {
                (target as ComTravel).StartCurve(null);
            }
        }
    }
}
