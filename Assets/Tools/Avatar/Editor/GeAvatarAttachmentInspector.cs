using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(GeAvatarAttachment), true)]
public class GeAvatarAttachmentInspector : Editor
{
    private SerializedObject m_Object;
    private GameObject m_RefAvatar = null;

    protected class GeAttachCache
    {
        public GeAttachCache(GameObject goAttach,string attachNode)
        {
            m_AttachmentObj = goAttach;
            m_AttachNode = attachNode;
        }

        public GameObject m_AttachmentObj = null;
        public string m_AttachNode = null;
    }

    protected List<GeAttachCache> m_AttachCacheList = new List<GeAttachCache>();

    protected string[] m_AttachNodeArray = null;
    protected List<string> m_AttachNodeList = new List<string>();

    protected void OnEnable()
    {
        m_Object = new SerializedObject(target);
        _Reload();
    }

    protected void OnDisable()
    {
    }

    public override bool HasPreviewGUI() { return true; }

    public override void OnInspectorGUI()
    {
        m_Object.Update();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        GameObject refAvatar = EditorGUILayout.ObjectField("参考挂件模型：", m_RefAvatar, typeof(GameObject)) as GameObject;
        if(GUILayout.Button("重新加载挂点", GUILayout.Width(80)) || refAvatar != m_RefAvatar)
        {
            m_AttachNodeList.Clear();
            for (int j = 0, jcnt = m_AttachCacheList.Count; j < jcnt; ++j)
            {
                GeAttachCache cur = m_AttachCacheList[j];
                if (null == cur) continue;

                if (!m_AttachNodeList.Contains(cur.m_AttachNode))
                    m_AttachNodeList.Add(cur.m_AttachNode);
            }

            _AddAttachNodeList(m_RefAvatar, ref m_AttachNodeList);
            m_AttachNodeArray = m_AttachNodeList.ToArray();
        }
        m_RefAvatar = refAvatar;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        for (int i = 0, icnt = m_AttachCacheList.Count; i < icnt; ++i)
        {
            GeAttachCache cur = m_AttachCacheList[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label("挂件对象");
            cur.m_AttachmentObj = EditorGUILayout.ObjectField(cur.m_AttachmentObj, typeof(GameObject)) as GameObject;
            GUILayout.EndHorizontal();

            if(null != m_AttachNodeArray)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("挂件挂点");
                int idx = _GetAttachNodeIdx(cur.m_AttachNode);
                int newIdx = EditorGUILayout.Popup(idx, m_AttachNodeArray);
                if (idx != newIdx)
                    cur.m_AttachNode = m_AttachNodeArray[newIdx];
                GUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("添加挂件"))
            m_AttachCacheList.Add(new GeAttachCache(null, ""));

        if (EditorGUI.EndChangeCheck())
        {
            GeAvatarAttachment targAvatarProxy = (GeAvatarAttachment)target;
            if (null != targAvatarProxy)
            {
                targAvatarProxy.refAvatar = AssetDatabase.GetAssetPath(m_RefAvatar).Replace("Assets/Resources/", null);

                GeAttachDesc[] attachDesc = new GeAttachDesc[m_AttachCacheList.Count];
                for (int i = 0,icnt = m_AttachCacheList.Count;i<icnt;++i)
                {
                    GeAttachDesc curDesc = new GeAttachDesc();
                    if (null == curDesc) continue;

                    GeAttachCache curCache = m_AttachCacheList[i];
                    if(null == curCache) continue;

                    curDesc.m_AttachmentRes = AssetDatabase.GetAssetPath(curCache.m_AttachmentObj).Replace("Assets/Resources/", null);
                    curDesc.m_AttachNode = curCache.m_AttachNode;
                    attachDesc[i] = curDesc;
                }

                targAvatarProxy.attachDescArray = attachDesc;
            }
            
            m_Object.ApplyModifiedProperties();
        }
    }

    protected void _AddAttachNodeList(GameObject go,ref List<string> attachNodeList)
    {
        if (null == go)
            return;

        Transform[] childList = go.GetComponentsInChildren<Transform>();
        if(null != childList)
        {
            for(int i = 0,icnt = childList.Length;i<icnt;++i)
            {
                Transform cur = childList[i];
                if(null == cur) continue;

                if(cur.gameObject.CompareTag("Dummy"))
                {
                    string name = "[actor]" + cur.gameObject.name;
                    if (!attachNodeList.Contains(name))
                        attachNodeList.Add(name);
                }
            }
        }
    }

    protected int _GetAttachNodeIdx(string attachNode)
    {
        for(int i = 0,icnt = m_AttachNodeList.Count;i<icnt;++i)
        {
            if (attachNode.Equals(m_AttachNodeList[i]))
                return i;
        }

        return -1;
    }


    protected void _Reload()
    {

        m_AttachNodeList.Clear();
        GeAvatarAttachment targAvatarProxy = (GeAvatarAttachment)target;
        if (null != targAvatarProxy)
        {
            if (string.IsNullOrEmpty(targAvatarProxy.refAvatar))
                m_RefAvatar = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", targAvatarProxy.refAvatar), typeof(GameObject)) as GameObject;

            m_AttachCacheList.Clear();
            GeAttachDesc[] attachDescArray = targAvatarProxy.attachDescArray;
            if (null != attachDescArray)
            {
                for (int i = 0,icnt = attachDescArray.Length; i < icnt; ++i)
                {
                    GeAttachDesc curDesc = attachDescArray[i];
                    if(null == curDesc) continue;

                    if(string.IsNullOrEmpty(curDesc.m_AttachNode))
                        continue;

                    if (!m_AttachNodeList.Contains(curDesc.m_AttachNode))
                        m_AttachNodeList.Add(curDesc.m_AttachNode);

                    GameObject goAttach = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/", curDesc.m_AttachmentRes), typeof(GameObject)) as GameObject;
                    if (null != goAttach)
                        m_AttachCacheList.Add(new GeAttachCache(goAttach, curDesc.m_AttachNode));
                }
            }
        }

        if(null != m_RefAvatar)
        {
            _AddAttachNodeList(m_RefAvatar, ref m_AttachNodeList);
        }

        m_AttachNodeArray = m_AttachNodeList.ToArray();
    }

    //protected void _OnDrawAnimClipList()
    //{
    //    bool bChanged = false;
    //    m_AnimResNum = EditorGUILayout.IntField("动画资源数量：", m_AnimResNum);
    //    if (m_AnimResNum > 0)
    //    {
    //        for (int i = 0; i < m_AnimResNum; ++i)
    //        {
    //            if (m_AnimDataResObjList.Count <= i)
    //            {
    //                bChanged = true;
    //                m_AnimDataResObjList.Add(EditorGUILayout.ObjectField("动画文件" + i + ":", null, typeof(Object)) as Object);
    //            }
    //            else
    //            {
    //                Object oldAnimRes = m_AnimDataResObjList[i];
    //                m_AnimDataResObjList[i] = EditorGUILayout.ObjectField("动画文件" + i + ":", oldAnimRes, typeof(Object)) as Object;
    //                if (oldAnimRes != m_AnimDataResObjList[i])
    //                    bChanged = true;
    //            }
    //        }
    //    }
    //
    //    if(bChanged)
    //    {
    //        m_AnimDataResFile.ClearArray();
    //        for (int i = 0; i < m_AnimResNum; ++i)
    //        {
    //            m_AnimDataResFile.InsertArrayElementAtIndex(i);
    //            SerializedProperty curAnimRes = m_AnimDataResFile.GetArrayElementAtIndex(i);
    //            curAnimRes.stringValue = AssetDatabase.GetAssetPath(m_AnimDataResObjList[i]).Replace("Assets/Resources/", null);
    //        }
    //
    //        m_Object.ApplyModifiedProperties();
    //        GeAnimDescProxy targAvatarProxy = (GeAnimDescProxy)target;
    //        if (null != targAvatarProxy)
    //        {
    //            targAvatarProxy.GenAnimDesc();
    //        }
    //        m_Object.Update();
    //        _Reload();
    //
    //        bChanged = false;
    //    }
    //    EditorGUILayout.Space();
    //
    //    EditorGUILayout.IntField("动画数量：", m_AnimClipNum);
    //    if (m_AnimClipNum > 0)
    //    {
    //        for (int i = 0; i < m_AnimClipNum; ++i)
    //        {
    //            EditorGUILayout.ObjectField("动画序列" + i + ":", m_AnimClipList[i], typeof(AnimationClip));
    //        }
    //    }
    //}
    

    // [MenuItem("[TM工具集]/ArtTools/Refresh Anim Desc")]
    // static public void GenerateAnimDescProxy()
    // {
    //     var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor" });
    //     //var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor/Monster_Emo" });
    //     int cnt = 0;
    //     float total = str.Length;
    //     foreach (var guid in str)
    //     {
    //         EditorUtility.DisplayProgressBar("烘焙动画资源", "正在烘焙第"+ cnt + "个资源...", ((cnt++) / total));
    // 
    //         var path = AssetDatabase.GUIDToAssetPath(guid);
    // 
    //         Debug.Log("######" + path);
    // 
    //         var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);
    // 
    //         if (path.Contains("_camera")) continue;
    //         if (path.Contains("/Weapon/") || path.Contains("/WeaponShow/")) continue;
    // 
    //         GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
    //         if(null != temp)
    //         {
    //             GeAnimDescProxy proxy = temp.GetComponent<GeAnimDescProxy>();
    // 
    //             if(null == proxy)
    //                 proxy = temp.AddComponent<GeAnimDescProxy>();
    // 
    //             List<string> fbxLst = new List<string>();
    //             _GetNearestAnimationPath(path,ref fbxLst);
    // 
    //             for(int i = 0,icnt = fbxLst.Count;i<icnt;++i)
    //             {
    //                 fbxLst[i] = fbxLst[i].Replace('\\', '/').Replace("Assets/Resources/", null);
    //             }
    // 
    //             Animation anim = temp.GetComponent<Animation>();
    //             if (null != anim)
    //             {
    //                 AnimationClip defClip = anim.clip;
    // 
    //                 Object.DestroyImmediate(anim);
    //                 anim = temp.AddComponent<Animation>();
    //                 if (null != anim)
    //                 {
    //                     anim.clip = defClip;
    //                     anim.cullingType = AnimationCullingType.BasedOnRenderers;
    //                 }
    //             }
    // 
    //             if (fbxLst.Count > 0)
    //             {
    //                 proxy.animDataResFile = fbxLst.ToArray();
    //                 proxy.GenAnimDesc();
    //             }
    //         }
    // 
    //         AssetDatabase.SaveAssets();
    //         PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
    //         GameObject.DestroyImmediate(temp);
    //         GameObject.DestroyImmediate(data);
    //     }
    // 
    //     EditorUtility.ClearProgressBar();
    // }
    // 
    // static void _GetNearestAnimationPath(string path,ref List<string> outFileList)
    // {
    //     path = path.Replace('\\', '/');
    //     string[] dicList = path.Split('/');
    // 
    //     for (int i = dicList.Length - 1;i>=0;--i)
    //     {
    //         string serchPath = "";
    //         int j = 0;
    //         while (j < i)
    //         {
    //             serchPath += dicList[j++];
    //             serchPath += '/';
    //         }
    // 
    //         string[] animPath = null;
    //         if(!string.IsNullOrEmpty(serchPath))
    //         {
    //             try
    //             {
    //                 animPath = Directory.GetDirectories(serchPath, "Animations");
    //             }
    //             catch (System.Exception ex)
    //             {
    //                 Debug.LogError("#####################:" + serchPath + "["+ ex.Message + "]");
    //             }
    //         }
    //         if(null != animPath && animPath.Length > 0)
    //         {
    //             for(int k = 0,kcnt = animPath.Length;k<kcnt;++k)
    //             {
    //                 string[] animFile = Directory.GetFiles(animPath[k]);
    //                 for(int m = 0,mcnt = animFile.Length;m<mcnt;++m)
    //                 {
    //                     string ext = Path.GetExtension(animFile[m]);
    //                     if (ext.Contains("meta")) continue;
    // 
    //                     if(ext.Contains("fbx") || ext.Contains("FBX"))
    //                     {
    //                         GameObject curFBX = AssetDatabase.LoadAssetAtPath(animFile[m], typeof(GameObject)) as GameObject;
    //                         if (null != curFBX)
    //                         {
    //                             outFileList.Add(animFile[m]);
    //                             DestroyImmediate(curFBX);
    //                         }
    //                     }
    // 
    //                     if(ext.Contains("anim") || ext.Contains("ANIM"))
    //                     {
    //                         AnimationClip curAnimClip = AssetDatabase.LoadAssetAtPath(animFile[m], typeof(AnimationClip)) as AnimationClip;
    //                         if (null != curAnimClip)
    //                         {
    //                             outFileList.Add(animFile[m]);
    //                             DestroyImmediate(curAnimClip);
    //                         }
    //                     }
    //                 }
    //             }
    //         }
    // 
    //         if(outFileList.Count > 0)
    //             return;
    //     }
    // }
    // 
    // [MenuItem("[TM工具集]/ArtTools/Modify Anim Update Mode")]
    // static public void ModifyAnimtionUpdateMode()
    // {
    //     var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Actor" });
    // 
    //     foreach (var guid in str)
    //     {
    //         var path = AssetDatabase.GUIDToAssetPath(guid);
    //         var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);
    // 
    //         GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
    //         Animation anim = temp.GetComponentInChildren<Animation>();
    //         if (null != anim)
    //             anim.cullingType = AnimationCullingType.BasedOnRenderers;
    // 
    //         AssetDatabase.SaveAssets();
    //         PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
    //         GameObject.DestroyImmediate(temp);
    //     }
    // }
}
