using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.Text;

// 检查特效Animator的工具
public class EffectCookCheck : EditorWindow
{
    private string m_SrcDir = "";
    private StringBuilder m_stringBuilder = new StringBuilder();
    private string m_Warning = "";
    private Vector2 scrollPos = new Vector2();
    private bool m_Repair = false;

    [MenuItem("[TM工具集]/ArtTools/检查EffectCook")]
    static void OpenWindow()
    {
        // Get existing open window or if none, make a new one:
        EffectCookCheck window = (EffectCookCheck)EditorWindow.GetWindow(typeof(EffectCookCheck));
        window.titleContent = new GUIContent("EffectCookCheck");
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("检查路径", GUILayout.Width(150));
        EditorGUILayout.TextField(m_SrcDir);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
        {
            m_SrcDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources/Effects", "");
        }
        EditorGUILayout.EndHorizontal();

        m_Repair = EditorGUILayout.Toggle(m_Repair);

        if (GUILayout.Button("开始检查"))
        {
            CheckEffectCooks();
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Warning Prefabs:");
        GUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true;
        style.fixedHeight = position.height - 30;
        m_Warning = EditorGUILayout.TextArea(m_Warning, style);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void CheckEffectCooks()
    {
        m_stringBuilder.Clear();
        m_Warning = "";
        if (m_SrcDir == "")
            return;

        string[] searchFolder = new string[] { m_SrcDir.Substring(m_SrcDir.IndexOf("Assets")) };
        var prefabs = AssetDatabase.FindAssets("t:prefab", searchFolder);

        float fProgress = 0;

        try
        {
            for (int i = 0; i < prefabs.Length; ++i)
            {
                var path = AssetDatabase.GUIDToAssetPath(prefabs[i]);

                fProgress += 1.0F;
                string title = "正在检查( " + i + " of " + prefabs.Length + " )";

                EditorUtility.DisplayProgressBar(title, path, fProgress / prefabs.Length);
                GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (root)
                {
                    bool bRepaired = CheckEffectCook(path, "", root);
                    if(bRepaired)
                    {
                        EditorUtility.SetDirty(root);
                    }
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();

            if (m_Repair)
                AssetDatabase.SaveAssets();
        }

        m_Warning = m_stringBuilder.ToString();
    }

    private bool CheckEffectCook(string prefabName, string parentName, GameObject obj)
    {
        /*
                Canvas canvas = obj.GetComponent<Canvas>();
                if(canvas != null)
                {
                    if(canvas.overrideSorting == true)
                    {
                        m_stringBuilder.AppendFormat("{0} : {1}/{2} : {3} : {4}{5}", prefabName, parentName, obj.name, 
                            canvas.sortingLayerName, canvas.sortingOrder.ToString(), Environment.NewLine);
                    }
                }*/

        bool bRepaired = false;

        GeEffectProxy effectProxy = obj.GetComponent<GeEffectProxy>();
        if (effectProxy != null)
        {
            if (!CheckComponentValid(effectProxy.m_AudioProxy, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_AudioProxy{3}", prefabName, parentName, obj.name, Environment.NewLine);

                if (m_Repair) bRepaired = true;
            }

            if (!CheckComponentValid(effectProxy.m_Animations, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_Animations{3}", prefabName, parentName, obj.name, Environment.NewLine);
                if (m_Repair) bRepaired = true;
            }

            if (!CheckComponentValid(effectProxy.m_ParticleSys, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_ParticleSys{3}", prefabName, parentName, obj.name, Environment.NewLine);
                if (m_Repair) bRepaired = true;
            }

            if (!CheckComponentValid(effectProxy.m_Trails, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_Trails{3}", prefabName, parentName, obj.name, Environment.NewLine);
                if (m_Repair) bRepaired = true;
            }

            if (!CheckComponentValid(effectProxy.m_SeqFrames, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_SeqFrames{3}", prefabName, parentName, obj.name, Environment.NewLine);
                if (m_Repair) bRepaired = true;
            }
/*
             if (!CheckComponentValid(effectProxy.m_ParticleEmitter, obj, m_Repair))
             {
                 m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_ParticleEmitter{3}", prefabName, parentName, obj.name, Environment.NewLine);
                 if (m_Repair) bRepaired = true;
             }

             if (!CheckComponentValid(effectProxy.m_ParticleAnimator, obj, m_Repair))
             {
                 m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_ParticleAnimator{3}", prefabName, parentName, obj.name, Environment.NewLine);
                 if (m_Repair) bRepaired = true;
             }
*/
            if (!CheckComponentValid(effectProxy.m_Animators, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_Animators{3}", prefabName, parentName, obj.name, Environment.NewLine);
                if (m_Repair) bRepaired = true;
            }

            if (!CheckComponentValid(effectProxy.m_MeshRenderer, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_MeshRenderer{3}", prefabName, parentName, obj.name, Environment.NewLine);
                if (m_Repair) bRepaired = true;
            }

            if (!CheckComponentValid(effectProxy.m_SpineAnimations, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_SpineAnimations{3}", prefabName, parentName, obj.name, Environment.NewLine);
                if (m_Repair) bRepaired = true;
            }

            if (!CheckComponentValid(effectProxy.m_SpineAnimationsUI, obj, m_Repair))
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2}, m_SpineAnimationsUI{3}", prefabName, parentName, obj.name, Environment.NewLine);
                if (m_Repair) bRepaired = true;
            }
        }

        for (int i = 0; i < obj.transform.childCount; ++i)
        {
            bool bRet = CheckEffectCook(prefabName, parentName + "/" + obj.name, obj.transform.GetChild(i).gameObject);

            bRepaired = bRepaired || bRet;
        }

        return bRepaired;
    }

    bool CheckComponentValid<T>(T comp, GameObject obj, bool repair) where T: Component
    {
        if (comp == null)
            return true;

        T[] components = obj.GetComponentsInChildren<T>(true);
        if (components.Length > 0)
        {
            foreach (var itr in components)
            {
                if (comp == itr)
                {
                    return true;
                }
            }
        }

        if(repair)
            comp = null;

        return false;
    }

    bool CheckComponentValid<T>(T[] comps, GameObject obj, bool repair) where T : Component
    {
        if (comps == null || comps.Length == 0)
            return true;

        T[] components = obj.GetComponentsInChildren<T>(true);
        if (components.Length == 0)
        {
            for (int i = 0; i < comps.Length; ++i)
            {
                if (comps[i] != null)
                {
                    if (repair)
                        comps[i] = null;

                    return false;
                }
            }

            return true;
        }

        for (int i = 0; i < comps.Length; ++i)
        {
            if (comps[i] == null)
                continue;

            bool bValid = false;

            foreach (var itr in components)
            {
                if (comps[i] == itr)
                {
                    bValid = true;
                }
            }

            if(!bValid)
            {
                if (repair)
                    comps[i] = null;

                return false;
            }
        }

/*
        if (repair)
        {
            for (int i = 0; i < comps.Length; ++i)
            {
                if (comps[i] == null)
                    continue;
            }
        }*/

        return true;
    }
}