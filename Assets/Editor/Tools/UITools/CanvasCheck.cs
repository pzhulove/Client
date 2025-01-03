using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.Text;

// 检查Canvas的工具，只有主城和UIRoot的canvas允许设置sortingorder
public class CanvasCheck : EditorWindow
{
    private string m_SrcDir = "";
    private StringBuilder m_stringBuilder = new StringBuilder();
    private string m_Warning = "";
    private Vector2 scrollPos = new Vector2();

    [MenuItem("[TM工具集]/ArtTools/检查Canvas")]
    static void CreateChenghao()
    {
        // Get existing open window or if none, make a new one:
        CanvasCheck window = (CanvasCheck)EditorWindow.GetWindow(typeof(CanvasCheck));
        window.titleContent = new GUIContent("CanvasCheck");
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
            m_SrcDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "../../UIPacker/称号", "");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("开始检查"))
        {
            CheckCanvas();
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

    private void CheckCanvas()
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
                    CheckCanvas(path, "", root);
                }
            }
        }
        catch(Exception e)
        {
            EditorUtility.ClearProgressBar();
            throw e;
        }

        EditorUtility.ClearProgressBar();

        m_Warning = m_stringBuilder.ToString();
    }

    private void CheckCanvas(string prefabName, string parentName, GameObject obj)
    {
        Canvas canvas = obj.GetComponent<Canvas>();
        if(canvas != null)
        {
            if(canvas.overrideSorting == true)
            {
                m_stringBuilder.AppendFormat("{0} : {1}/{2} : {3} : {4}{5}", prefabName, parentName, obj.name, 
                    canvas.sortingLayerName, canvas.sortingOrder.ToString(), Environment.NewLine);
            }
        }

        for(int i = 0; i < obj.transform.childCount; ++i)
        {
            CheckCanvas(prefabName, parentName + "/" + obj.name, obj.transform.GetChild(i).gameObject);
        }
    }
}