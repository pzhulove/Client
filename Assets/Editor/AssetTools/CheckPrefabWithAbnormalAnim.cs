using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CheckPrefabWithAbnormalAnim : EditorWindow
{
    #region Params
    private static string _rootPath;
    private static List<PrefabWithAbnormalData> _allAbnormalData = new List<PrefabWithAbnormalData>();
    private static Vector3 _vec3Scroll;
    #endregion

    #region Init
    /// <summary>
    /// 目前先检测动画绑定对象不存在问题
    /// 后续检测动画绑定Component不存在问题
    /// </summary>
    [MenuItem("[TM工具集]/资源规范相关/查找预制体不规范动画")]
    private static void CheckAllPrefabWithAbnormalAnim()
    {
        CheckPrefabWithAbnormalAnim window = GetWindow<CheckPrefabWithAbnormalAnim>();
        window.titleContent = new GUIContent("检查预制体不规范动画使用");
        window.Show();
    }
    #endregion

    #region Mono
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("检查路径", GUILayout.Width(150));
        EditorGUILayout.TextField(_rootPath);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
            _rootPath = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources", "");
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("检查当前路径下预制体不规范Anim使用"))
            CheckAllPrefabWithAbnormalAnimAtThisPath();
        int abnormalCount = _allAbnormalData.Count;
        if (abnormalCount > 0)
        {
            GUILayout.Label("共找到：" + abnormalCount + "个问题预制体");
            _vec3Scroll = EditorGUILayout.BeginScrollView(_vec3Scroll);
            for (int i = 0; i < abnormalCount; i++)
            {
                PrefabWithAbnormalData prefabData = _allAbnormalData[i];
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("预制体： ", GUILayout.Width(50));
                EditorGUILayout.ObjectField(prefabData.Prefab, typeof(GameObject), true, GUILayout.Width(170));
                GUILayout.Label("以下子级上动画存在问题：", GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
                foreach (KeyValuePair<GameObject, ObjWithAbnormalData> objToObjAbnormalData in prefabData.AllAbnormalObjDic)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("  ", GUILayout.Width(6));
                    EditorGUILayout.ObjectField(objToObjAbnormalData.Value.AnimObj, typeof(GameObject), true, GUILayout.Width(170));
                    EditorGUILayout.EndHorizontal();
                    foreach (KeyValuePair<AnimationClip, List<string>> clipToMissObj in objToObjAbnormalData.Value.AbnormalClipToObjMap)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("    使用了AnimationClip", GUILayout.Width(130));
                        EditorGUILayout.ObjectField(clipToMissObj.Key, typeof(AnimationClip), true, GUILayout.Width(200));
                        GUILayout.Label("用到以下GameObject Missing");
                        EditorGUILayout.EndHorizontal();
                        foreach (string missObjName in clipToMissObj.Value)
                        {
                            GUILayout.Label("      " + missObjName);
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }
    #endregion

    #region Func
    private void CheckAllPrefabWithAbnormalAnimAtThisPath()
    {
        EditorUtility.ClearProgressBar();
        if (string.IsNullOrEmpty(_rootPath))
            return;
        if (!Directory.Exists(_rootPath))
            return;
        _allAbnormalData.Clear();

        string[] searchFolder = { _rootPath.Substring(_rootPath.IndexOf("Assets", StringComparison.Ordinal)) };
        string[] allAssetsGuid = AssetDatabase.FindAssets("t:prefab", searchFolder);
        int corrCount = RemoveRepeatGuid(ref allAssetsGuid);
        float fProgress = 0;
        for (int i = 0; i < corrCount; i++)
        {
            fProgress++;
            EditorUtility.DisplayProgressBar("正在查找", fProgress.ToString() + "/" + corrCount.ToString(), fProgress / corrCount);
            string path = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
            GameObject objPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (objPrefab == null)
                continue;
            Animation[] allAnimation = objPrefab.GetComponents<Animation>();
            Animator[] allAnimator = objPrefab.GetComponents<Animator>();
            int animationCount = allAnimation.Length;
            int animatorCount = allAnimator.Length;
            if (animationCount == 0 && animatorCount == 0)
                continue;
            PrefabWithAbnormalData prefabData = new PrefabWithAbnormalData { PrefabPath = path, Prefab = objPrefab };
            if (animationCount > 0)  //Animation
            {
                for (int j = 0; j < animationCount; j++)
                {
                    Animation anim = allAnimation[j];
                    AnimationClip[] allClips = AnimationUtility.GetAnimationClips(anim.gameObject);
                    int clipCount = allClips.Length;
                    if (clipCount == 0)
                        continue;
                    for (int k = 0; k < clipCount; k++)
                    {
                        AnimationClip clip = allClips[k];
                        EditorCurveBinding[] bindings1 = AnimationUtility.GetCurveBindings(clip);
                        EditorCurveBinding[] bindings2 = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                        int bind1Count = bindings1.Length;
                        int bind2Count = bindings2.Length;
                        if (bind1Count == 0 && bind2Count == 0)
                            continue;
                        if (bind1Count > 0)
                        {
                            for (int l = 0; l < bind1Count; l++)
                            {
                                EditorCurveBinding bind = bindings1[l];
                                Transform tranTarget = anim.transform.Find(bind.path);
                                if (tranTarget == null)
                                {
                                    // 问题预制体，记录
                                    ObjWithAbnormalData objData;
                                    if (!prefabData.AllAbnormalObjDic.TryGetValue(anim.gameObject, out objData))
                                    {
                                        objData = new ObjWithAbnormalData { AnimObj = anim.gameObject, ObjName = anim.name };
                                        prefabData.AllAbnormalObjDic.Add(anim.gameObject, objData);
                                    }
                                    List<string> allAbnormalObj;
                                    if (!objData.AbnormalClipToObjMap.TryGetValue(clip, out allAbnormalObj))
                                    {
                                        allAbnormalObj = new List<string>();
                                        objData.AbnormalClipToObjMap.Add(clip, allAbnormalObj);
                                    }
                                    if (!allAbnormalObj.Contains(bind.path))
                                        allAbnormalObj.Add(bind.path);
                                }
                            }
                        }
                        if (bind2Count > 0)
                        {
                            for (int l = 0; l < bind2Count; l++)
                            {
                                EditorCurveBinding bind = bindings2[l];
                                Transform tranTarget = anim.transform.Find(bind.path);
                                if (tranTarget == null)
                                {
                                    // 问题预制体，记录
                                    ObjWithAbnormalData objData;
                                    if (!prefabData.AllAbnormalObjDic.TryGetValue(anim.gameObject, out objData))
                                    {
                                        objData = new ObjWithAbnormalData { AnimObj = anim.gameObject, ObjName = anim.name };
                                        prefabData.AllAbnormalObjDic.Add(anim.gameObject, objData);
                                    }
                                    List<string> allAbnormalObj;
                                    if (!objData.AbnormalClipToObjMap.TryGetValue(clip, out allAbnormalObj))
                                    {
                                        allAbnormalObj = new List<string>();
                                        objData.AbnormalClipToObjMap.Add(clip, allAbnormalObj);
                                    }
                                    if (!allAbnormalObj.Contains(bind.path))
                                        allAbnormalObj.Add(bind.path);
                                }
                            }
                        }
                    }
                }
            }

            if (animatorCount > 0)  //Animator
            {
                for (int j = 0; j < animatorCount; j++)
                {
                    Animator anim = allAnimator[j];
                    if (anim == null || anim.runtimeAnimatorController == null)
                        continue;
                    AnimationClip[] allClips = anim.runtimeAnimatorController.animationClips;
                    int clipCount = allClips.Length;
                    if (clipCount == 0)
                        continue;
                    for (int k = 0; k < clipCount; k++)
                    {
                        AnimationClip clip = allClips[k];
                        EditorCurveBinding[] bindings1 = AnimationUtility.GetCurveBindings(clip);
                        EditorCurveBinding[] bindings2 = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                        int bind1Count = bindings1.Length;
                        int bind2Count = bindings2.Length;
                        if (bind1Count == 0 && bind2Count == 0)
                            continue;
                        if (bind1Count > 0)
                        {
                            for (int l = 0; l < bind1Count; l++)
                            {
                                EditorCurveBinding bind = bindings1[l];
                                Transform tranTarget = anim.transform.Find(bind.path);
                                if (tranTarget == null)
                                {
                                    // 问题预制体，记录
                                    ObjWithAbnormalData objData;
                                    if (!prefabData.AllAbnormalObjDic.TryGetValue(anim.gameObject, out objData))
                                    {
                                        objData = new ObjWithAbnormalData { AnimObj = anim.gameObject, ObjName = anim.name };
                                        prefabData.AllAbnormalObjDic.Add(anim.gameObject, objData);
                                    }
                                    List<string> allAbnormalObj;
                                    if (!objData.AbnormalClipToObjMap.TryGetValue(clip, out allAbnormalObj))
                                    {
                                        allAbnormalObj = new List<string>();
                                        objData.AbnormalClipToObjMap.Add(clip, allAbnormalObj);
                                    }
                                    if (!allAbnormalObj.Contains(bind.path))
                                        allAbnormalObj.Add(bind.path);
                                }
                            }
                        }// if (bind1Count > 0)
                        if (bind2Count > 0)
                        {
                            for (int l = 0; l < bind2Count; l++)
                            {
                                EditorCurveBinding bind = bindings2[l];
                                Transform tranTarget = anim.transform.Find(bind.path);
                                if (tranTarget == null)
                                {
                                    // 问题预制体，记录
                                    ObjWithAbnormalData objData;
                                    if (!prefabData.AllAbnormalObjDic.TryGetValue(anim.gameObject, out objData))
                                    {
                                        objData = new ObjWithAbnormalData { AnimObj = anim.gameObject, ObjName = anim.name };
                                        prefabData.AllAbnormalObjDic.Add(anim.gameObject, objData);
                                    }
                                    List<string> allAbnormalObj;
                                    if (!objData.AbnormalClipToObjMap.TryGetValue(clip, out allAbnormalObj))
                                    {
                                        allAbnormalObj = new List<string>();
                                        objData.AbnormalClipToObjMap.Add(clip, allAbnormalObj);
                                    }
                                    if (!allAbnormalObj.Contains(bind.path))
                                        allAbnormalObj.Add(bind.path);
                                }
                            }
                        }// if (bind2Count > 0)
                    }
                }
            }

            if (prefabData.AllAbnormalObjDic.Count > 0)
            {
                _allAbnormalData.Add(prefabData);
            }
        }
        EditorUtility.ClearProgressBar();
    }
    private int RemoveRepeatGuid(ref string[] allGuid)
    {
        int curIndex = 0;
        for (int i = 0; i < allGuid.Length; i++)
        {
            while (true)
            {
                if (i + 1 >= allGuid.Length)
                    break;
                if (allGuid[i + 1] != allGuid[curIndex])
                {
                    curIndex++;
                    allGuid[curIndex] = allGuid[i + 1];
                    break;
                }
                i++;
            }
        }
        curIndex++;
        return curIndex;
    }
    #endregion
}

public class PrefabWithAbnormalData
{
    public string PrefabPath;
    public GameObject Prefab;
    public Dictionary<GameObject, ObjWithAbnormalData> AllAbnormalObjDic;

    public PrefabWithAbnormalData()
    {
        AllAbnormalObjDic = new Dictionary<GameObject, ObjWithAbnormalData>();
    }
}

public class ObjWithAbnormalData
{
    public GameObject AnimObj;
    public string ObjName;
    public Dictionary<AnimationClip, List<string>> AbnormalClipToObjMap;

    public ObjWithAbnormalData()
    {
        AbnormalClipToObjMap = new Dictionary<AnimationClip, List<string>>();
    }
}