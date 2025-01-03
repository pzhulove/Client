


namespace Tenmove.Editor.Unity
{
    using UnityEditor;
    using UnityEngine;

    internal static partial class Styles
    {
        public static class Default
        {
            static private GUISkin m_Skin = null;
            static public GUISkin Skin
            {
                get
                {
                    if (null == m_Skin)
                        m_Skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Scripts/Tenmove.Editor.Unity/Res/Skin/tenmove.guiskin");
                    return m_Skin;
                }
            }

            static public GUIStyle Button = new GUIStyle((GUIStyle)"button")
            {
                fontSize = 15,
                font = Skin.font,
                fontStyle = FontStyle.Normal,
                fixedHeight = 0.0f,
                padding = new RectOffset(0, 0, 10, 10),
                alignment = TextAnchor.MiddleCenter
            };

            static public GUIStyle Label = new GUIStyle((GUIStyle)"label")
            {
                fontSize = 15,
                font = Skin.font,
                fontStyle = FontStyle.Normal,
                fixedHeight = 0.0f,
                padding = new RectOffset(0, 0, 10, 10),
                alignment = TextAnchor.MiddleLeft
            };

            static public GUIStyle ObjectField = new GUIStyle(EditorStyles.objectField)
            {
                fontSize = 15,
                font = Skin.font,
                fontStyle = FontStyle.Normal,
                fixedHeight = 0.0f,
                padding = new RectOffset(0, 0, 10, 10),
                alignment = TextAnchor.MiddleLeft
            };

            static public GUIStyle Slider = new GUIStyle("slider")
            {
                fontSize = 15,
                font = Skin.font,
                fontStyle = FontStyle.Normal,
                fixedHeight = 0.0f,
                padding = new RectOffset(0, 0, 10, 10),
                alignment = TextAnchor.MiddleLeft
            };

            static public GUIStyle HelpBox = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 15,
                font = Skin.font,
                fontStyle = FontStyle.Normal,
                fixedHeight = 0.0f,
                padding = new RectOffset(0, 0, 10, 10),
                alignment = TextAnchor.MiddleLeft
            };

            static public GUIStyle Notification = new GUIStyle((GUIStyle)"NotificationBackground")
            {
            };
        }
    }
}