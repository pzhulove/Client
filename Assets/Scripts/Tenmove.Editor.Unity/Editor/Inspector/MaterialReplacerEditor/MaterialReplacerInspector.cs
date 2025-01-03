using Tenmove.Runtime.Unity;
using UnityEngine;
namespace Tenmove.Editor.Unity
{
    using UnityEditor;
    [CustomEditor(typeof(MaterialReplacerComponent))]
    public class MaterialReplacerInspector : Editor
    {
        MaterialReplacerComponent m_Mytarget;
        Material m_Material;
        public override void OnInspectorGUI()
        {
            m_Mytarget = (MaterialReplacerComponent)target;
            GUI.enabled = false;
            for (int i = 0; i < m_Mytarget.m_Renders.Length; i++)
            {
                EditorGUILayout.ObjectField("render" + i, m_Mytarget.m_Renders[i], typeof(Renderer), true);
            }
            GUI.enabled = true;
            if (GUILayout.Button("查找renders"))
            {
                FindRenders(m_Mytarget.gameObject);
            }
            m_Material = (Material)EditorGUILayout.ObjectField("材质", m_Material, typeof(Material), true);
            if (GUILayout.Button("替换材质"))
            {
                m_Mytarget.SetMaterial(m_Material);
            }
        }

        public void FindRenders(GameObject gameObject)
        {
            m_Mytarget.m_Renders = gameObject.GetComponentsInChildren<Renderer>();
        }
    }
}