using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.ComponentModel;

#if UNITY_EDITOR
public class SearchFilesEditorWindow : EditorWindow
{
    public enum SearchFileType
    {
        [Description("包含事件的.anim文件")]
        ANIMATIONCLIP,
        [Description("包含事件的.prefab文件")]
        PREFAB_ANIMATIONCLIP,
        [Description("有Collider的.prefab文件")]
        PREFAB_COLLIDER,
    }
    public static SearchFilesEditorWindow mEditorWindow;

    private string mEffectPath;
    private string mResourcePath;
    private string mSearchFilesPath;
    private string mOutLogPath;

    private List<string> mOutLogList;

    private SearchFileType mSearchFileType;
    private string[] mSearchFileTypeStringArr = null;
    private Array mSearchFileTypeValueArr = null;

    private bool mSaveLog = true;
    private bool mPrintLog = true;

    [MenuItem("[TM工具集]/过滤资源文件")]
    public static void OpenWindow()
    {
        if(mEditorWindow != null)
        {
            mEditorWindow.Close();
        }
        mEditorWindow = GetWindow<SearchFilesEditorWindow>();
        mEditorWindow.Show();
    }

    private void Awake()
    {
        InitData();
    }

    private void OnGUI()
    {
        if (EditorApplication.isCompiling)
        {
            EditorGUILayout.HelpBox(string.Format("正在编译中\n"), MessageType.Warning);
            return;
        }

        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox(string.Format("游戏正在运行\n"), MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("路径为绝对路径，可以直接复制文件夹里的路径");
            EditorGUILayout.BeginHorizontal();
            {
                if (mSearchFilesPath == null)
                {
                    mSearchFilesPath = string.Empty;
                }
                var str = EditorGUILayout.TextField("文件搜索路径：", mSearchFilesPath);
                if (str != mSearchFilesPath)
                {
                    mSearchFilesPath = str;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                if (StyledButton("资源总路径"))
                {
                    mSearchFilesPath = mResourcePath;
                }

                if (StyledButton("特效路径"))
                {
                    mSearchFilesPath = mEffectPath;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                if(mSearchFileTypeValueArr == null || mSearchFileTypeValueArr == null)
                {
                    var t = typeof(SearchFileType);
                    mSearchFileTypeStringArr = t.GetDescriptions();
                    mSearchFileTypeValueArr = t.GetValues();
                }
                int index = Array.BinarySearch(mSearchFileTypeValueArr, mSearchFileType);
                index = EditorGUILayout.Popup("搜索方式：", index, mSearchFileTypeStringArr);
                mSearchFileType = (SearchFileType)index;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                if (StyledButton("搜索一下"))
                {
                    DoSearch();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    void DoSearch()
    {
        if (!Directory.Exists(mSearchFilesPath) || !mSearchFilesPath.Contains(Application.dataPath)) 
        {
            Debug.LogError("请检查路径的正确性，以及是否是当前项目的资源路径");
            return;
        }
        mOutLogList.Clear();
        string noFileNotify = string.Empty;
        string logFileName = string.Empty;
        switch (mSearchFileType)
        {
            case SearchFileType.ANIMATIONCLIP:
                DFSFiles(mSearchFilesPath, _animFilter, _doAnimFile);
                noFileNotify = string.Format("{0}路径下的所有动画文件都没有添加事件", mSearchFilesPath);
                logFileName = "含有事件的动画文件.txt";
                break;
            case SearchFileType.PREFAB_ANIMATIONCLIP:
                DFSFiles(mSearchFilesPath, _prefabFilter, _doAnimEventPrefab);
                noFileNotify = string.Format("{0}路径下的所有预制体上都没有关联有动画事件的动画文件", mSearchFilesPath);
                logFileName = "含有动画事件动画的预制体.txt";
                break;
            case SearchFileType.PREFAB_COLLIDER:
                DFSFiles(mSearchFilesPath, _prefabFilter, _doColliderPrefab);
                noFileNotify = string.Format("{0}路径下的所有预制体上都没有Collider", mSearchFilesPath);
                logFileName = "含有Collider的预制体.txt";
                break;
            default:
                break;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        if (mSaveLog)
        {
            if (mOutLogList.Count == 0)
            {
                Logger.LogError(noFileNotify);
            }
            else
            {
                if (Directory.Exists(mOutLogPath))
                {
                    try
                    {
                        var p = mOutLogPath + logFileName;
                        File.WriteAllLines(p, mOutLogList.ToArray());
                        Logger.LogError("日志存放路径" + Path.GetFullPath(p));
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.Message);
                    }
                }
            }
        }
    }

    void DFSFiles(string path, Func<FileInfo,bool> filter_, Action<UnityEngine.Object, string> func_)
    {
        DirectoryInfo folder = new DirectoryInfo(path);
        foreach (FileInfo nextfile in folder.GetFiles())
        {
            var fullPath = nextfile.FullName;
            if (filter_ != null && !filter_(nextfile))
            {
                continue;
            }
            var prefabPath = fullPath.Substring(fullPath.IndexOf("Assets"));
            var go = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath);
            func_(go, prefabPath);
        }
        foreach (DirectoryInfo nextFolder in folder.GetDirectories())
        {
            DFSFiles(nextFolder.FullName, filter_, func_);
        }
    }

    private void InitData()
    {
        mResourcePath = Application.dataPath + "/Resources";
        mEffectPath = mResourcePath + "/Effects";
        mSearchFilesPath = string.Empty;
        mOutLogPath = Application.dataPath + "/../../";
        var t = typeof(SearchFileType);
        mSearchFileTypeStringArr = t.GetDescriptions();
        mSearchFileTypeValueArr = t.GetValues();

        mOutLogList = new List<string>();
    }

    public static bool StyledButton(string label)
    {
        EditorGUILayout.Space();
        GUILayoutUtility.GetRect(1, 20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool clickResult = GUILayout.Button(label, "miniButton");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        return clickResult;
    }

    #region Filter
    bool _animFilter(FileInfo file)
    {
        var fileName = file.Name;
        if (fileName.Contains(".anim") && !fileName.Contains(".meta")) 
        {
            return true;
        }
        return false;
    }

    bool _prefabFilter(FileInfo file)
    {
        var fileName = file.Name;
        if (fileName.Contains(".prefab") && !fileName.Contains(".meta"))
        {
            return true;
        }
        return false;
    }
    #endregion
    #region Process File
    void _doAnimFile(UnityEngine.Object obj,string path)
    {
        if(obj == null)
        {
            return;
        }
        AnimationClip c = obj as AnimationClip;
        if(c == null)
        {
            return;
        }
        int num = logAnimationEvent(c);

        if (num > 0)
        {
            string log = string.Format("AnitionClip Path:{0},has Event Num:{1}", path, num);
            if (mPrintLog)
            {
                Logger.LogError(log);
            }
            mOutLogList.Add(log);
        }
    }
    int logAnimationEvent(AnimationClip a)
    {
        if (a != null && a.events != null && a.events.Length > 0)
        {
            return a.events.Length;
        }
        return 0;
    }
    void _doAnimEventPrefab(UnityEngine.Object obj, string path)
    {
        if(obj == null)
        {
            return;
        }
        GameObject go = obj as GameObject;
        int pa = 0;
        GeAnimDescProxy[] ga = go.GetComponentsInChildren<GeAnimDescProxy>();
        if (ga != null)
        {
            for (int i = 0; i < ga.Length; ++i)
            {
                for (int j = 0; j < ga[i].animDescArray.Length; ++j)
                {
                    if (ga[i].animDescArray[j] != null && ga[i].animDescArray[j].animClipData != null)
                        pa += logAnimationEvent(ga[i].animDescArray[j].animClipData);
                }

            }
        }

        GeEffectProxy[] ge = go.GetComponentsInChildren<GeEffectProxy>();
        if (ge != null)
        {
            for (int i = 0; i < ge.Length; ++i)
            {
                for (int j = 0; j < ge[i].m_Animations.Length; ++j)
                {
                    if (ge[i].m_Animations[j] != null && ge[i].m_Animations[j].clip != null)
                        pa += logAnimationEvent(ge[i].m_Animations[j].clip);
                }
                for (int j = 0; j < ge[i].m_Animators.Length; ++j)
                {
                    if (ge[i].m_Animators[j] != null && ge[i].m_Animators[j].runtimeAnimatorController != null
                        && ge[i].m_Animators[j].runtimeAnimatorController.animationClips != null)
                    {
                        for (int k = 0; k < ge[i].m_Animators[j].runtimeAnimatorController.animationClips.Length; ++k)
                        {
                            pa += logAnimationEvent(ge[i].m_Animators[j].runtimeAnimatorController.animationClips[k]);
                        }
                    }
                }
            }
        }
        if (pa > 0)
        {
            var o = string.Format("Path: {0},has Animation Event num:{1}", path, pa);
            mOutLogList.Add(o);
            Logger.LogErrorFormat(o);
        }
    }
    void _doColliderPrefab(UnityEngine.Object obj, string path)
    {
        if (obj == null)
        {
            return;
        }
        GameObject go = obj as GameObject;
        if (go == null)
        {
            return;
        }
        int nun = 0;
        if (go != null)
        {
            var c = go.GetComponentsInChildren<Collider>();
            if (c != null)
            {
                nun += c.Length;
            }
            var c2d = go.GetComponentsInChildren<Collider2D>();
            if (c2d != null)
            {
                nun += c2d.Length;
            }
        }
        if (nun > 0)
        {
            var o = string.Format("Path: {0},has Collider num:{1}", path, nun);
            mOutLogList.Add(o);
            Logger.LogErrorFormat(o);
        }
    }
    void _doColliderPrefabRemove(UnityEngine.Object obj,string path)
    {
        if(obj == null)
        {
            return;
        }
        GameObject go = obj as GameObject;
        if(go == null)
        {
            return;
        }
        int nun = 0;
        if (go != null)
        {
            var c = go.GetComponentsInChildren<Collider>(true);
            if (c != null && c.Length > 0) 
            {
                nun += c.Length;
                for(int i = 0; i < c.Length; ++i)
                {
                    DestroyImmediate(c[i],true);
                }
            }
            var c2d = go.GetComponentsInChildren<Collider2D>(true);
            if (c2d != null && c2d.Length > 0) 
            {
                nun += c2d.Length;
                for(int i = 0; i < c2d.Length; ++i)
                {
                    DestroyImmediate(c2d[i],true);
                }
            }
            if (nun > 0)
            {
                EditorUtility.SetDirty(go);
                var o = string.Format("Path: {0},has Collider num:{1}", path, nun);
                mOutLogList.Add(o);
                Logger.LogErrorFormat(o);
            }
        }
        
    }
    #endregion
}
#endif
