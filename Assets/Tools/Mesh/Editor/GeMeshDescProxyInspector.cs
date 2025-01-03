using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(GeMeshDescProxy), true)]
public class GeMeshDescProxyInspector : Editor
{

#if !LOGIC_SERVER
    private SerializedObject m_Object;
    
    private SerializedProperty m_Surface_IsOpaque;
    private SerializedProperty m_Surface_AnimatRes;
    private SerializedProperty m_ShadeModel;

    private Object m_ModelData = null;

    protected void OnEnable()
    {
        m_Object = new SerializedObject(target);
        
        m_Surface_IsOpaque = m_Object.FindProperty("m_Surface_IsOpaque");
        m_Surface_AnimatRes = m_Object.FindProperty("m_Surface_AnimatRes");
        m_ShadeModel = m_Object.FindProperty("m_ShadeModel");

        m_ModelData = AssetDatabase.LoadAssetAtPath<DModelData>(Path.Combine("Assets/Resources/", m_Surface_AnimatRes.stringValue)) as DModelData;
    }

    protected void OnDisable()
    {
    }

    public override bool HasPreviewGUI() { return true; }


    public override void OnInspectorGUI()
    {
        m_Object.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_Surface_IsOpaque, new GUIContent("不透明网格:"));
        EditorGUILayout.PropertyField(m_ShadeModel, new GUIContent("着色模型:"));

        m_ModelData = EditorGUILayout.ObjectField("动画材质:", m_ModelData,typeof(DModelData));

        if (EditorGUI.EndChangeCheck())
        {
            if (null != m_ModelData)
                m_Surface_AnimatRes.stringValue = AssetDatabase.GetAssetPath(m_ModelData).Replace("Assets/Resources/", null);
            
            m_Object.ApplyModifiedProperties();
        }
        if (GUILayout.Button("重新生成动画材质"))
        {
            GeMeshDescProxy targMeshProxy = (GeMeshDescProxy)target;

            if (null == targMeshProxy)
                return;
            
            if (null != targMeshProxy)
            {
                targMeshProxy.RebakeAnimat();
                m_Object.ApplyModifiedProperties();
            }
        }
    }


    [MenuItem("Assets/Refresh Mesh Desc")]
    static public void GenerateMeshDescProxyAlt()
    {
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);

        for (int i = 0; i < selection.Length; ++i)
        {
            GameObject curPrefab = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null == curPrefab)
                continue;

            GameObject temp = PrefabUtility.InstantiatePrefab(curPrefab) as GameObject;
            SkinnedMeshRenderer[] asmr = temp.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (null != asmr)
            {
                for (int j = 0, icnt = asmr.Length; j < icnt; ++j)
                {
                    if (null == asmr[j]) continue;

                    GeMeshDescProxy newProxy = asmr[j].gameObject.GetComponent<GeMeshDescProxy>();
                    if (null == newProxy)
                    {
                        newProxy = asmr[j].gameObject.AddComponent<GeMeshDescProxy>();
                        newProxy.m_Surface_IsOpaque = true;
                    }
                }
            }

            MeshRenderer[] amr = curPrefab.GetComponentsInChildren<MeshRenderer>();
            if (null != amr)
            {
                for (int j = 0, jcnt = amr.Length; j < jcnt; ++j)
                {
                    if (null == amr[j]) continue;

                    GeMeshDescProxy newProxy = amr[j].gameObject.GetComponent<GeMeshDescProxy>();
                    if (null == newProxy)
                        newProxy = amr[j].gameObject.AddComponent<GeMeshDescProxy>();

                    if (null != newProxy)
                        newProxy.m_Surface_IsOpaque = true;
                }
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, curPrefab, ReplacePrefabOptions.ConnectToPrefab);

            Editor.DestroyImmediate(curPrefab, false);
        }

        AssetDatabase.SaveAssets();

    }


    [MenuItem("[TM工具集]/ArtTools/Refresh Mesh Desc")]
    static public void GenerateMeshDescProxy()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor", "Assets/Resources/Scene" });
        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);
    
            if(null != data)
            {
                GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
                SkinnedMeshRenderer[] asmr = temp.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (null != asmr)
                {
                    for (int i = 0, icnt = asmr.Length; i < icnt; ++i)
                    {
                        if (null == asmr[i]) continue;

                        GeMeshDescProxy newProxy = asmr[i].gameObject.GetComponent<GeMeshDescProxy>();
                        if (null == newProxy)
                        {
                            newProxy = asmr[i].gameObject.AddComponent<GeMeshDescProxy>();
                            newProxy.m_Surface_IsOpaque = true;
                        }
                    }
                }

                MeshRenderer[] amr = data.GetComponentsInChildren<MeshRenderer>();
                if (null != amr)
                {
                    for (int i = 0, icnt = amr.Length; i < icnt; ++i)
                    {
                        if (null == amr[i]) continue;

                        GeMeshDescProxy newProxy = amr[i].gameObject.GetComponent<GeMeshDescProxy>();
                        if (null == newProxy)
                            newProxy = amr[i].gameObject.AddComponent<GeMeshDescProxy>();

                        if (null != newProxy)
                            newProxy.m_Surface_IsOpaque = true;
                    }
                }

                AssetDatabase.SaveAssets();
                PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
                GameObject.DestroyImmediate(temp);
            }
        }
    }


#if UNITY_EDITOR
    [MenuItem("[TM工具集]/ArtTools/Refresh Mesh Desc (PBR)")]
    static public void GenerateMeshDescProxyPBR()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor" });
        //var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Gunman" });
        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (null != data)
            {
                GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
                SkinnedMeshRenderer[] asmr = temp.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (null != asmr)
                {
                    for (int i = 0, icnt = asmr.Length; i < icnt; ++i)
                    {
                        if (null == asmr[i]) continue;

                        GeMeshDescProxy newProxy = asmr[i].gameObject.GetComponent<GeMeshDescProxy>();
                        if (null == newProxy)
                        {
                            newProxy = asmr[i].gameObject.AddComponent<GeMeshDescProxy>();
                            newProxy.m_Surface_IsOpaque = true;
                        }

                        Material[] am = asmr[i].sharedMaterials;
                        for (int j = 0, jcnt = am.Length; j < jcnt; ++j)
                        {
                            Material m = am[j];
                            if (null == m) continue;

                            if (m.shader.name.Contains("PBR"))
                            {
                                newProxy.surfaceAnimatRes = "Animat/CommonBuffEffectPBR.asset";
                                break;
                            }
                        }
                    }
                }


				MeshRenderer[] amr = temp.GetComponentsInChildren<MeshRenderer>();
				if (null != amr)
				{
					for (int i = 0, icnt = amr.Length; i < icnt; ++i)
					{
						if (null == amr[i]) continue;

						GeMeshDescProxy newProxy = amr[i].gameObject.GetComponent<GeMeshDescProxy>();
						if (null == newProxy)
						{
							newProxy = amr[i].gameObject.AddComponent<GeMeshDescProxy>();
							newProxy.m_Surface_IsOpaque = true;
						}

						Material[] am = amr[i].sharedMaterials;
						for (int j = 0, jcnt = am.Length; j < jcnt; ++j)
						{
							Material m = am[j];
							if (null == m) continue;

							if (m.shader.name.Contains("PBR"))
							{
								newProxy.surfaceAnimatRes = "Animat/CommonBuffEffectPBR.asset";
								break;
							}
						}
					}
				}

                AssetDatabase.SaveAssets();
                PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
                GameObject.DestroyImmediate(temp);
            }
        }
    }
#endif
#endif
}
