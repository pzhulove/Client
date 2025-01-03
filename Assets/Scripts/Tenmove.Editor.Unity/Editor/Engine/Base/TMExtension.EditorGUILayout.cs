

namespace Tenmove.Editor.Unity
{
    using Tenmove.Runtime;
    using UnityEditor;
    using UnityEngine;

    public static partial class Extension
    {
        public static class EditorGUILayout
        {
            private static readonly Texture[] m_Icon = new Texture[3];
            private static GUIContent m_TempHelpContent = new GUIContent();

            public static void HelpBox(string message, MessageType type, GUIStyle style)
            {
                m_TempHelpContent.text = message;
                m_TempHelpContent.image = _GetHelpIcon(type);
                m_TempHelpContent.tooltip = null;
                UnityEditor.EditorGUILayout.LabelField(GUIContent.none, m_TempHelpContent, style);
            }

            private static Texture _GetHelpIcon(MessageType type)
            {
                Texture res = null;
                if (MessageType.None < type)
                {
                    if (null == m_Icon[(int)type - 1])
                        m_Icon[(int)type - 1] = Reflector.Type<EditorGUIUtility>().Methods["GetHelpIcon"].InvokeR<Texture, MessageType>(type);

                    res = m_Icon[(int)type - 1];
                }

                return res;
            }
        }
    }
}