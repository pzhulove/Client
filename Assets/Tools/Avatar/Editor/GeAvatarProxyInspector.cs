using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(GeAvatarProxy), true)]
public class GeAvatarProxyInspector : Editor
{
    private SerializedObject m_Object;
    
    private SerializedProperty m_Avatar;
    private SerializedProperty m_SkelRemapOffest;
    private SerializedProperty m_SkelRemapTable;

    protected void OnEnable()
    {
        m_Object = new SerializedObject(target);

        m_Avatar = m_Object.FindProperty("m_Avatar");
        m_SkelRemapOffest = m_Object.FindProperty("m_SkelRemapOffest");
        m_SkelRemapTable = m_Object.FindProperty("m_SkelRemapTable");
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
        EditorGUILayout.PropertyField(m_Avatar, new GUIContent("Avatar:"));
        if (EditorGUI.EndChangeCheck())
            m_Object.ApplyModifiedProperties();

        if (GUILayout.Button("重新生成骨骼索引"))
        {
            GeAvatarProxy targAvatarProxy = (GeAvatarProxy)target;

            if (null == targAvatarProxy)
                return;

            List<int> newRemapTable = new List<int>();
            List<int> newOffsetTable = new List<int>();
            if (null != targAvatarProxy)
            {
                targAvatarProxy.RefreshAvatarDesc();
                m_Object.ApplyModifiedProperties();
            }
        }
    }


    [MenuItem("[TM工具集]/ArtTools/Refresh Avatar Desc")]
    static public void GenerateAvatarDescProxy()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor" });
        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if(path.Contains("_Head_") ||
               path.Contains("_Body_") ||
               path.Contains("_Pant_") )
            {
                string pathJob = path.Replace("Assets/Resources/Actor/", null);

                GameObject avatar = null;
                string pathAvatar = null;
                int idx = pathJob.IndexOf('/');
                if(idx < pathJob.Length)
                {
                    pathJob = pathJob.Substring(0,idx);
                    pathAvatar = Path.Combine("Assets/Resources/Actor/" + pathJob, Path.Combine("Animations/" , pathJob  +  "_Avatar.prefab"));
                    avatar = AssetDatabase.LoadAssetAtPath(pathAvatar,typeof(GameObject)) as GameObject;

                    if(null != avatar)
                    {
                        GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
                    
                        GeAvatarProxy curAvatar = temp.GetComponent<GeAvatarProxy>();
                        if (null == curAvatar)
                            curAvatar = temp.AddComponent<GeAvatarProxy>();
                    
                        if (null != curAvatar)
                        {
                            curAvatar.avatar = avatar;
                            curAvatar.RefreshAvatarDesc();
                        }
                    
                        AssetDatabase.SaveAssets();
                        PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
                        GameObject.DestroyImmediate(temp);
                    }
                }
            }

        }
    }
}
