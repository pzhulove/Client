using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CleanAssetsTool
{
    public class CleanAssetException : Exception
    {

    }

    class WarningWindow : EditorWindow
    {
        private static StringBuilder m_stringBuilder = new StringBuilder();
        private Vector2 scrollPos = new Vector2();
        private GUIStyle style;
        private string m_Warning = "";

        internal static WarningWindow ShowWarningWindow()
        {
            // Get existing open window or if none, make a new one:
            WarningWindow window = (WarningWindow)EditorWindow.GetWindow(typeof(WarningWindow));
            window.titleContent = new GUIContent("Warning");
            window.Init();
            window.Show();

            return window;
        }

        public static bool HasWarning()
        {
            return m_stringBuilder.Length > 0;
        }

        public static void ClearError()
        {
            m_stringBuilder.Clear();
        }

        public static void PushInfo(string format, params object[] args)
        {
            //Debug.LogFormat(format, args);
        }

        public static void PushWarning(string format, params object[] args)
        {
            //Debug.LogWarningFormat(format, args);
            m_stringBuilder.AppendFormat(format, args);
            m_stringBuilder.AppendLine();
        }

        public static void PushError(string format, params object[] args)
        {
            m_stringBuilder.AppendFormat(format, args);
            m_stringBuilder.AppendLine();

            Debug.LogErrorFormat(format, args);
            throw new Exception();
        }

        private void Init()
        {
            style = new GUIStyle(EditorStyles.textArea);
            style.wordWrap = true;

            m_Warning = m_stringBuilder.ToString();
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.TextArea(m_Warning, style);

            EditorGUILayout.EndScrollView();
        }
    }
}
