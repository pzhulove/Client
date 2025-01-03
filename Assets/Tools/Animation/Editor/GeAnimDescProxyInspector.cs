using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[CustomEditor(typeof(GeAnimDescProxy), true)]
public class GeAnimDescProxyInspector : Editor
{
    static readonly string m_BackupPath = "Assets/AnimBackUp/";

    private SerializedObject m_Object;
    private SerializedProperty m_AnimDataResFile;
    private SerializedProperty m_AnimDescArray;
    private SerializedProperty m_Animaion;
    private SerializedProperty m_ExternAnimDescData;

    private Transform[] m_AvatarSkeleton = null;

    private int m_AnimResNum = 0;
    private List<Object> m_AnimDataResObjList = new List<Object>();
    private int m_AnimClipNum = 0;
    private List<AnimationClip> m_AnimClipList = new List<AnimationClip>();

    protected void OnEnable()
    {
        m_Object = new SerializedObject(target);

        m_AnimDataResFile = m_Object.FindProperty("m_AnimDataResFile");
        m_AnimDescArray = m_Object.FindProperty("m_AnimDescArray");
        m_Animaion = m_Object.FindProperty("m_Animaion");
        m_ExternAnimDescData = m_Object.FindProperty("m_ExternAnimDescData");

        _Reload();
    }

    protected void OnDisable()
    {
    }

    public override bool HasPreviewGUI() { return true; }

    public override void OnInspectorGUI()
    {
        m_Object.Update();

        GeAnimDescProxy targAvatarProxy = (GeAnimDescProxy)target;
        if (null != targAvatarProxy)
        {
            if (GUILayout.Button("刷新数据"))
            {
                targAvatarProxy.GenAnimDesc();
                m_Object.Update();
                _Reload();
            }

            EditorGUILayout.ObjectField("动画控制器：", targAvatarProxy.animInstance, typeof(Animation));
        }

        EditorGUILayout.PropertyField(m_ExternAnimDescData);
        m_Object.ApplyModifiedProperties();

        EditorGUILayout.Space();
        _OnDrawAnimClipList();
        EditorGUILayout.Space();
    }

    protected void _Reload()
    {
        m_AnimResNum = m_AnimDataResFile.arraySize;
        for (int i = 0; i < m_AnimResNum; ++i)
        {
            SerializedProperty curResObj = m_AnimDataResFile.GetArrayElementAtIndex(i);
            m_AnimDataResObjList.Add(AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", curResObj.stringValue), typeof(Object)) as Object);
        }

        GeAnimDescProxy targAvatarProxy = (GeAnimDescProxy)target;
        if(null != targAvatarProxy)
        {
            m_AnimClipList.Clear();
            GeAnimDesc[] animDescArray = targAvatarProxy.animDescArray;
            if(null != animDescArray)
            {
                m_AnimClipNum = animDescArray.Length;
                for (int i = 0; i < m_AnimClipNum; ++i)
                {
                    animDescArray[i].animClipData = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", animDescArray[i].animClipPath), typeof(AnimationClip)) as AnimationClip;
                    m_AnimClipList.Add(animDescArray[i].animClipData);
                }
            }
        }
    }

    protected void _OnDrawAnimClipList()
    {
        bool bChanged = false;
        m_AnimResNum = EditorGUILayout.IntField("动画资源数量：", m_AnimResNum);
        if (m_AnimResNum > 0)
        {
            for (int i = 0; i < m_AnimResNum; ++i)
            {
                if (m_AnimDataResObjList.Count <= i)
                {
                    bChanged = true;
                    m_AnimDataResObjList.Add(EditorGUILayout.ObjectField("动画文件" + i + ":", null, typeof(Object)) as Object);
                }
                else
                {
                    Object oldAnimRes = m_AnimDataResObjList[i];
                    m_AnimDataResObjList[i] = EditorGUILayout.ObjectField("动画文件" + i + ":", oldAnimRes, typeof(Object)) as Object;
                    if (oldAnimRes != m_AnimDataResObjList[i])
                        bChanged = true;
                }
            }
        }

        if(bChanged)
        {
            m_AnimDataResFile.ClearArray();
            for (int i = 0; i < m_AnimResNum; ++i)
            {
                m_AnimDataResFile.InsertArrayElementAtIndex(i);
                SerializedProperty curAnimRes = m_AnimDataResFile.GetArrayElementAtIndex(i);
                curAnimRes.stringValue = AssetDatabase.GetAssetPath(m_AnimDataResObjList[i]).Replace("Assets/Resources/", null);
            }

            m_Object.ApplyModifiedProperties();
            GeAnimDescProxy targAvatarProxy = (GeAnimDescProxy)target;
            if (null != targAvatarProxy)
            {
                targAvatarProxy.GenAnimDesc();
            }
            m_Object.Update();
            _Reload();

            bChanged = false;
        }
        EditorGUILayout.Space();

        EditorGUILayout.IntField("动画数量：", m_AnimClipNum);
        if (m_AnimClipNum > 0)
        {
            for (int i = 0; i < m_AnimClipNum; ++i)
            {
                EditorGUILayout.ObjectField("动画序列" + i + ":", m_AnimClipList[i], typeof(AnimationClip));
            }
        }
    }

    [MenuItem("[TM工具集]/ArtTools/Test Test")]
    static public void GenerateTestProxy()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor/Hero_Gungirl" });

        if (!Directory.Exists(m_BackupPath))
            AssetDatabase.CreateFolder("Assets", "AnimBackUp");

        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;

            Animation anim = temp.GetComponentInChildren<Animation>();
            if (null != anim)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(anim);
                GameObject go = new GameObject();

                Animation  newAnim = go.AddComponent<Animation>();
                ComponentUtility.PasteComponentValues(newAnim);

                PrefabUtility.CreatePrefab(Path.Combine(m_BackupPath, Path.GetFileName(path)), go);
                EditorUtility.SetDirty(go);
            }
            
            GameObject.DestroyImmediate(temp);
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Assets/角色工具/30、Refresh Anim Desc", false, 30)]
    static public void GenerateAnimDescProxy()
    {
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);

        for (int i = 0; i < selection.Length; ++i)
        { 
            EditorUtility.DisplayProgressBar("烘焙动画资源", "正在烘焙第" + i + "个资源...", (i / selection.Length));
            GameObject data = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null == data)
                continue;

            string  path =  AssetDatabase.GetAssetPath(data);
            if (path.EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
                continue;
            if (path.Contains("_camera")) continue;
            //if (path.Contains("/Weapon/") || path.Contains("/WeaponShow/")) continue;
            if (path.EndsWith(".FBX", StringComparison.OrdinalIgnoreCase))
                continue;
            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (null != temp)
            {
                GeAnimDescProxy proxy = temp.GetComponent<GeAnimDescProxy>();

                if (null == proxy)
                    proxy = temp.AddComponent<GeAnimDescProxy>();

                List<string> fbxLst = new List<string>();
                _GetNearestAnimationPath(path, ref fbxLst);

                for (int j = 0, jcnt = fbxLst.Count; j < jcnt; ++j)
                    fbxLst[j] = fbxLst[j].Replace('\\', '/').Replace("Assets/Resources/", null);

                Animation anim = temp.GetComponent<Animation>();
                if (null != anim)
                {
                    AnimationClip defClip = anim.clip;

                    Object.DestroyImmediate(anim);
                    anim = temp.AddComponent<Animation>();
                    if (null != anim)
                    {
                        anim.clip = defClip;
                        anim.cullingType = AnimationCullingType.BasedOnRenderers;
                    }
                }

                if (fbxLst.Count > 0)
                {
                    proxy.animDataResFile = fbxLst.ToArray();
                    proxy.GenAnimDesc();
                }

                string prefabDirPath = Path.GetDirectoryName(path);
                if (prefabDirPath.EndsWith("Animations", StringComparison.Ordinal))
                {  // 如果Avatar Prefab上无SkinMeshRenderer，则添加之
                    SkinnedMeshRenderer smr = temp.GetComponent<SkinnedMeshRenderer>();
                    if (smr == null)
                    {
                        smr = temp.AddComponent<SkinnedMeshRenderer>();
                        smr.lightProbeUsage = LightProbeUsage.Off;
                        smr.reflectionProbeUsage = ReflectionProbeUsage.Off;
                        smr.shadowCastingMode = ShadowCastingMode.Off;
                        smr.sharedMaterials = new Material[0];
                    }
                }
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
            //GameObject.DestroyImmediate(data);

        }

        EditorUtility.ClearProgressBar();
    }



    [MenuItem("[TM工具集]/ArtTools/Refresh All Anim Desc")]
    static public void GenerateAllAnimDescProxy()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor" });
        //var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor/Monster_Emo" });
        int cnt = 0;
        float total = str.Length;
        foreach (var guid in str)
        {
            EditorUtility.DisplayProgressBar("烘焙动画资源", "正在烘焙第"+ cnt + "个资源...", ((cnt++) / total));

            var path = AssetDatabase.GUIDToAssetPath(guid);

            Debug.Log("######" + path);

            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (path.Contains("_camera")) continue;
            if (path.Contains("/Weapon/") || path.Contains("/WeaponShow/")) continue;

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if(null != temp)
            {
                GeAnimDescProxy proxy = temp.GetComponent<GeAnimDescProxy>();

                if(null == proxy)
                    proxy = temp.AddComponent<GeAnimDescProxy>();

                List<string> fbxLst = new List<string>();
                _GetNearestAnimationPath(path,ref fbxLst);

                for(int i = 0,icnt = fbxLst.Count;i<icnt;++i)
                {
                    fbxLst[i] = fbxLst[i].Replace('\\', '/').Replace("Assets/Resources/", null);
                }

                Animation anim = temp.GetComponent<Animation>();
                if (null != anim)
                {
                    AnimationClip defClip = anim.clip;

                    Object.DestroyImmediate(anim);
                    anim = temp.AddComponent<Animation>();
                    if (null != anim)
                    {
                        anim.clip = defClip;
                        anim.cullingType = AnimationCullingType.BasedOnRenderers;
                    }
                }

                if (fbxLst.Count > 0)
                {
                    proxy.animDataResFile = fbxLst.ToArray();
                    proxy.GenAnimDesc();
                }
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(data);
        }

        EditorUtility.ClearProgressBar();
    }

    static void _GetNearestAnimationPath(string path,ref List<string> outFileList)
    {
        path = path.Replace('\\', '/');
        string[] dicList = path.Split('/');

        for (int i = dicList.Length - 1;i>=0;--i)
        {
            string serchPath = "";
            int j = 0;
            while (j < i)
            {
                serchPath += dicList[j++];
                serchPath += '/';
            }

            string[] animPath = null;
            if(!string.IsNullOrEmpty(serchPath))
            {
                try
                {
                    animPath = Directory.GetDirectories(serchPath, "Animations");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("#####################:" + serchPath + "["+ ex.Message + "]");
                }
            }
            if(null != animPath && animPath.Length > 0)
            {
                for(int k = 0,kcnt = animPath.Length;k<kcnt;++k)
                {
                    string[] animFile = Directory.GetFiles(animPath[k]);
                    for(int m = 0,mcnt = animFile.Length;m<mcnt;++m)
                    {
                        string ext = Path.GetExtension(animFile[m]);
                        if (ext.Contains("meta")) continue;

                        if(ext.Contains("fbx") || ext.Contains("FBX"))
                        {
                            continue;
                            GameObject curFBX = AssetDatabase.LoadAssetAtPath(animFile[m], typeof(GameObject)) as GameObject;
                            if (null != curFBX)
                            {
                                outFileList.Add(animFile[m]);
                                DestroyImmediate(curFBX);
                            }
                        }

                        if(ext.Contains("anim") || ext.Contains("ANIM"))
                        {
                            AnimationClip curAnimClip = AssetDatabase.LoadAssetAtPath(animFile[m], typeof(AnimationClip)) as AnimationClip;
                            if (null != curAnimClip)
                            {
                                outFileList.Add(animFile[m]);
                                //DestroyImmediate(curAnimClip);
                            }
                        }
                    }
                }
            }

            if(outFileList.Count > 0)
                return;
        }
    }

    [MenuItem("[TM工具集]/ArtTools/Modify Anim Update Mode")]
    static public void ModifyAnimtionUpdateMode()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor" });

        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            Animation anim = temp.GetComponentInChildren<Animation>();
            if (null != anim)
                anim.cullingType = AnimationCullingType.BasedOnRenderers;

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
        }
    }
}
