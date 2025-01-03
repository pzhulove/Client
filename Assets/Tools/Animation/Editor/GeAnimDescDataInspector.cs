using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;

[CustomEditor(typeof(GeAnimDescData), true)]
public class GeAnimDescDataInspector : Editor
{
    private GeAnimDescData m_Data;

    // 动画的源文件，可以是AnimationClip、FBX
    private List<Object> m_AnimDataResObjList = new List<Object>();
    // AnimationClip文件
    private List<AnimationClip> m_AnimClipList = new List<AnimationClip>();

    private int m_AnimResNum = 0;

    private void OnEnable()
    {
        m_Data = target as GeAnimDescData;

        _Reload();
    }

    private void _Reload()
    {
        m_AnimResNum = m_Data.animDataResFile.Length;
        m_AnimDataResObjList.Clear();
        m_AnimClipList.Clear();

        foreach (string animRes in m_Data.animDataResFile)
        {
            m_AnimDataResObjList.Add(AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", animRes), typeof(Object)) as Object);
        }

        foreach (GeAnimDesc animDesc in m_Data.animDescArray)
        {
            AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine("Assets/Resources/", animDesc.animClipPath)) as AnimationClip;
            m_AnimClipList.Add(animationClip);
        }
    }

    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("刷新数据"))
        {
            string dataPath = AssetDatabase.GetAssetPath(m_Data);
            string dataDirectory = dataPath.Substring(0, dataPath.LastIndexOf("/"));

            // 收集同目录下的动画文件
            m_Data.animDataResFile = GetAnimFiles(dataDirectory);
            m_Data.GenAnimDesc();

            _Reload();

            EditorUtility.SetDirty(m_Data);
            AssetDatabase.SaveAssets();
        }

        GUILayout.Space(10);

        EditorGUI.BeginChangeCheck();
        {
            m_AnimResNum = EditorGUILayout.IntField("动画资源数量：", m_AnimResNum);

            if(m_AnimDataResObjList.Count < m_AnimResNum)
            {
                int numToAdd = m_AnimResNum - m_AnimDataResObjList.Count;
                for(int i = 0;i < numToAdd;++i)
                {
                    m_AnimDataResObjList.Add(null);
                }
            }
            if(m_AnimDataResObjList.Count > m_AnimResNum)
            {
                int numToRemove = m_AnimDataResObjList.Count - m_AnimResNum;
                m_AnimDataResObjList.RemoveRange(m_AnimDataResObjList.Count - numToRemove, numToRemove);
            }

            for(int i = 0;i < m_AnimDataResObjList.Count;++i)
            {
                m_AnimDataResObjList[i] = EditorGUILayout.ObjectField("动画文件" + i + ":", m_AnimDataResObjList[i], typeof(Object), false) as Object;
            }
        }
        if(EditorGUI.EndChangeCheck())
        {
            // 保存动画源文件
            m_Data.animDataResFile = new string[m_AnimDataResObjList.Count];
            for(int i = 0;i < m_AnimDataResObjList.Count;++i)
            {
                m_Data.animDataResFile[i] = AssetDatabase.GetAssetPath(m_AnimDataResObjList[i]).Replace("Assets/Resources/", null);
            }

            m_Data.GenAnimDesc();
            _UpdateAnimClip();
        }

        GUILayout.Space(10);

        EditorGUILayout.IntField("动画数量：", m_AnimClipList.Count);

        for(int i = 0;i < m_AnimClipList.Count;++i)
        {
            EditorGUILayout.ObjectField("动画序列" + i + ":", m_AnimClipList[i], typeof(AnimationClip), false);
        }
    }

    private void _UpdateAnimClip()
    {
        m_AnimClipList.Clear();
        foreach (GeAnimDesc animDesc in m_Data.animDescArray)
        {
            AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine("Assets/Resources/", animDesc.animClipPath)) as AnimationClip;
            m_AnimClipList.Add(animationClip);
        }
    }

    // 查找动画文件
    private static string[] GetAnimFiles(string path)
    {
        string[] files = Directory.GetFiles(path);
        List<string> animFiles = new List<string>();

        for (int m = 0, mcnt = files.Length; m < mcnt; ++m)
        {
            string ext = Path.GetExtension(files[m]);
            if (ext.Contains("meta")) continue;

            //if (ext.Contains("fbx") || ext.Contains("FBX"))
            //{
            //    GameObject curFBX = AssetDatabase.LoadAssetAtPath(files[m], typeof(GameObject)) as GameObject;
            //    if (null != curFBX)
            //    {
            //        animFiles.Add(files[m]);
            //        //DestroyImmediate(curFBX);
            //    }
            //}

            if (ext.Contains("anim") || ext.Contains("ANIM"))
            {
                AnimationClip curAnimClip = AssetDatabase.LoadAssetAtPath(files[m], typeof(AnimationClip)) as AnimationClip;
                if (null != curAnimClip)
                {
                    animFiles.Add(files[m]);
                    //DestroyImmediate(curAnimClip);
                }
            }
        }

        for (int i = 0; i < animFiles.Count; ++i)
            animFiles[i] = animFiles[i].Replace('\\', '/').Replace("Assets/Resources/", null);

        return animFiles.ToArray();
    }


    [MenuItem("Assets/Create/武器动画文件")]
    public static void CreateGeAnimDescData()
    {
        if (Selection.objects == null || Selection.objects.Length < 1 || AssetDatabase.GetAssetPath(Selection.objects[0]) == string.Empty)
        {
            UnityEngine.Debug.LogError("请选择要处理的文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        string absolutePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + path;
        // 不是个path
        if (!System.IO.Directory.Exists(absolutePath))
        {
            path = path.Substring(0, path.LastIndexOf("/"));
        }


        GeAnimDescData animDescData = ScriptableObject.CreateInstance<GeAnimDescData>();
        animDescData.animDataResFile = GetAnimFiles(path);
        animDescData.GenAnimDesc();

        AssetDatabase.CreateAsset(animDescData, path + "/GeAnimDescData.asset");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Assets/角色工具/35、Refresh Anim Desc(Weapon)", false, 35)]
    public static void GenerateAnimDescProxy()
    {
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);

        for (int i = 0; i < selection.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("烘焙动画资源", "正在烘焙第" + i + "个资源...", (i / selection.Length));
            GameObject prefab = selection[i] as GameObject;
            if (null == prefab)
                continue;

            string path = AssetDatabase.GetAssetPath(prefab);
            if (path.EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
                continue;
            if (path.Contains("_camera")) continue;

            if (null != prefab)
            {
                GeAnimDescProxy proxy = prefab.GetComponent<GeAnimDescProxy>();

                if (null == proxy)
                    proxy = prefab.AddComponent<GeAnimDescProxy>();

                if (path.Contains("_Pifu/"))
                {
                    path = path.Substring(0, path.IndexOf("_Pifu/") + 6);
                    path = path.Replace("_Pifu", "");
                }
                GeAnimDescData animData = _GetNearestAnimationData(path);


                Animation anim = prefab.GetComponent<Animation>();
                if (anim == null)
                    anim = prefab.AddComponent<Animation>();

                anim.cullingType = AnimationCullingType.BasedOnRenderers;
                proxy.animInstance = anim;


                System.Type type = typeof(GeAnimDescProxy);
                var fieldInfo = type.GetField("m_ExternAnimDescData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                fieldInfo.SetValue(proxy, animData);
            }

            PrefabUtility.SavePrefabAsset(prefab);
        }

        EditorUtility.ClearProgressBar();
    }


    static GeAnimDescData _GetNearestAnimationData(string path)
    {
        path = path.Replace('\\', '/');
        string[] dicList = path.Split('/');

        for (int i = dicList.Length - 1; i >= 0; --i)
        {
            string serchPath = "";
            int j = 0;
            while (j < i)
            {
                serchPath += dicList[j++];
                serchPath += '/';
            }

            string[] animPath = null;
            if (!string.IsNullOrEmpty(serchPath))
            {
                try
                {
                    animPath = Directory.GetDirectories(serchPath, "Animations");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("#####################:" + serchPath + "[" + ex.Message + "]");
                }
            }
            if (null != animPath && animPath.Length > 0)
            {
                for (int k = 0, kcnt = animPath.Length; k < kcnt; ++k)
                {
                    string[] animDataGUID = AssetDatabase.FindAssets("t:GeAnimDescData", new string[] { animPath[k] });
                    if(animDataGUID != null && animDataGUID.Length > 0)
                    {
                        string animDataPath = AssetDatabase.GUIDToAssetPath(animDataGUID[0]);
                        GeAnimDescData animData = AssetDatabase.LoadAssetAtPath<GeAnimDescData>(animDataPath);

                        if (animData != null)
                            return animData;
                    }

                    
                }
            }
        }

        return null;
    }
}
