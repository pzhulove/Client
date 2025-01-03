using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.Text;

// 检查材质冗余Property的工具
public class MaterialPropertyClearWindow : EditorWindow
{
    private string m_SrcDir = "";
    private StringBuilder m_stringBuilder = new StringBuilder();
    private string m_Warning = "";
    private Vector2 scrollPos = new Vector2();

    private List<string> m_RepairList = new List<string>();

    private bool m_Repair;

    [MenuItem("[TM工具集]/资源规范相关/检查MaterialProperty冗余")]
    static void OpenWindow()
    {
        // Get existing open window or if none, make a new one:
        MaterialPropertyClearWindow window = (MaterialPropertyClearWindow)EditorWindow.GetWindow(typeof(MaterialPropertyClearWindow));
        window.titleContent = new GUIContent("MaterialPropertyClear");
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
        EditorGUILayout.LabelField("Warning Materials:");
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
            string logName = "Bug(修复材质)：提交修复材质冗余Property。";
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
        var assets = AssetDatabase.FindAssets("t:material", searchFolder);

        float fProgress = 0;

        try
        {
            for (int i = 0; i < assets.Length; ++i)
            {
                var path = AssetDatabase.GUIDToAssetPath(assets[i]);

                fProgress += 1.0F;
                string title = "正在检查( " + i + " of " + assets.Length + " )";

                EditorUtility.DisplayProgressBar(title, path, fProgress / assets.Length);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (mat)
                {
                    if(CheckAsset(path, mat))
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

    private bool CheckAsset(string matName, Material mat)
    {
        bool bRet = false;

        // 通过GetTexturePropertyNames能获取到隐藏的纹理Properties，但跟当前Shader不一致的通过HasProperty判断会返回false。
        SerializedObject so = new SerializedObject(mat);
        SerializedProperty pp = so.GetIterator();
        //       prop.Reset();

        while (pp.Next(true))
        {
            if (pp != null)
            {
                if(pp.name == "m_TexEnvs" && pp.isArray)
                {
                    int arrayIndex = pp.arraySize;
                    for(int i = 0; i < arrayIndex; ++i)
                    {
                        SerializedProperty dataProperty = pp.GetArrayElementAtIndex(i);

                        if(dataProperty != null)
                        {
                            SerializedProperty texNameProperty = dataProperty.FindPropertyRelative("first");
                            if(texNameProperty != null)
                            {
                                string texName = texNameProperty.stringValue;
                                if(!mat.HasProperty(texName) && mat.GetTexture(texName) != null)
                                {
                                    m_stringBuilder.AppendFormat("{0} : {1}{2}", matName, texName, Environment.NewLine);

                                    if (m_Repair)
                                    {
                                        mat.SetTexture(texName, null);
                                        bRet = true;
                                    }
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }

        return bRet;
    }
}