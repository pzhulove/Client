// using UnityEngine;
// using UnityEditor;
// using System.Collections;
// 
// namespace UnityEngine.UI.Extensions
// {
//     [CustomEditor(typeof(GeUIRadarChart), true)]
//     public class GeUIRadarChartInspector : Editor
//     {
//         private SerializedObject m_Object;
// 
//         private SerializedProperty m_Surface_IsOpaque;
//         private SerializedProperty m_Surface_AnimatData;
// 
//         protected void OnEnable()
//         {
//             m_Object = new SerializedObject(target);
// 
//             m_Surface_IsOpaque = m_Object.FindProperty("m_Surface_IsOpaque");
//             m_Surface_AnimatData = m_Object.FindProperty("m_Surface_AnimatData");
//         }
// 
//         protected void OnDisable()
//         {
//         }
// 
//         public override bool HasPreviewGUI() { return true; }
// 
// 
//         public override void OnInspectorGUI()
//         {
//             m_Object.Update();
//         }
// 
//     }
// }
