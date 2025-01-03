using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CleanAssetsTool
{
    public class ScriptInfoWindow : EditorWindow
    {
        private string m_ScriptInfo;
        private Vector2 scrollPos = new Vector2();
        private GUIStyle style;

        internal static ScriptInfoWindow ShowScriptInfoWindow(string striptName)
        {
            // Get existing open window or if none, make a new one:
            ScriptInfoWindow window = (ScriptInfoWindow)EditorWindow.GetWindow(typeof(ScriptInfoWindow));
            window.Init(striptName);
            window.Show();

            return window;
        }

        private void Init(string striptName)
        {
            m_ScriptInfo = "";

            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(striptName);
            TypeStringInfo typeInfo = ScriptAnalysis.GetClassStringInfo(monoScript);

            titleContent = new GUIContent("脚本信息：" + monoScript.GetClass().Name);

            if (typeInfo != null)
            {
                m_ScriptInfo = typeInfo.GetShowInfo(null, 0);
            }
            else
            {
                m_ScriptInfo = "无string相关成员";
            }

            style = new GUIStyle(EditorStyles.textArea);
            style.wordWrap = true;

            Repaint();
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.TextArea(m_ScriptInfo, style);

            EditorGUILayout.EndScrollView();
        }

    }
}
