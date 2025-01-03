using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.Text;

// 检查ScriptMissing的工具
public class ScriptMissingCheckWindow : EditorWindow
{
    private string m_SrcDir = "";
    private StringBuilder m_stringBuilder = new StringBuilder();
    private string m_Warning = "";
    private Vector2 scrollPos = new Vector2();

    private List<string> m_RepairList = new List<string>();

    private bool m_Repair;

    [MenuItem("[TM工具集]/资源规范相关/检查ScriptMissing")]
    static void OpenWindow()
    {
        // Get existing open window or if none, make a new one:
        ScriptMissingCheckWindow window = (ScriptMissingCheckWindow)EditorWindow.GetWindow(typeof(ScriptMissingCheckWindow));
        window.titleContent = new GUIContent("ScriptMissingCheck");
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
            m_SrcDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources", "");
        }
        EditorGUILayout.EndHorizontal();

        m_Repair = EditorGUILayout.Toggle("修复", m_Repair);

        if (GUILayout.Button("开始检查"))
        {
            CheckAssets();
        }

        if (GUILayout.Button("提交修改到SVN"))
        {
            CommitSVN();
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

    private void CommitSVN()
    {
        if(m_RepairList.Count > 0)
        {
            string commitFileNames = m_RepairList[0];
            string logName = "Bug(修复脚本缺失)：提交修复脚本缺失文件。";
            if (m_RepairList.Count > 1)
            {
                for (int i = 1; i < m_RepairList.Count; ++i)
                {
                    commitFileNames += "*" + m_RepairList[i];
                }
            }

            SvnTool.SvnCommit(commitFileNames, logName);
        }
    }

    private void CheckAssets()
    {
        m_RepairList.Clear();
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
                    if(CheckAsset(path, "", root))
                    {
                        m_RepairList.Add(Application.dataPath + path.Replace("Assets/", "/"));
                    }
                }
            }
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            throw e;
        }

        if(m_Repair)
        {
			AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        EditorUtility.ClearProgressBar();

        m_Warning = m_stringBuilder.ToString();
    }

    // https://blog.csdn.net/linxinfa/article/details/78047996
    private bool CheckAsset(string prefabName, string parentName, GameObject obj)
    {
        bool bRet = false; 

        if(m_Repair)
        {
            SerializedObject so = new SerializedObject(obj);
            var soProperties = so.FindProperty("m_Component");
            var components = obj.GetComponents<Component>();
            int propertyIndex = 0;
            foreach (var c in components)
            {
                if (c == null)
                {
                    soProperties.DeleteArrayElementAtIndex(propertyIndex);
                    m_stringBuilder.AppendFormat("{0} : {1}/{2}{3}", prefabName, parentName, obj.name, Environment.NewLine);
                    bRet = true;
                }
                else
                {
                    ++propertyIndex; 
                }
            }

            if(bRet)
                so.ApplyModifiedProperties();
        }
        else
        {
            var components = obj.GetComponents<Component>();

            foreach (var c in components)
            {
                if (c == null)
                {
                    m_stringBuilder.AppendFormat("{0} : {1}/{2}{3}", prefabName, parentName, obj.name, Environment.NewLine);
                    bRet = true;
                }
            }
        }

        for (int i = 0; i < obj.transform.childCount; ++i)
        {
            if(CheckAsset(prefabName, parentName + "/" + obj.name, obj.transform.GetChild(i).gameObject))
            {
                bRet = true;
            }
        }

        return bRet;
    }
}