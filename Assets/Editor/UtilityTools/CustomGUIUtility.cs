using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HeroGo
{
    public class CustomGUIUtility
    {
        private static GUIStyle linkLabelStyle;

        static CustomGUIUtility()
        {
            linkLabelStyle = new GUIStyle("ControlLabel")
            {
                normal = {
                    textColor = new Color(0.25f, 0.5f, 0.9f, 1f)
                },
                stretchWidth = false,
                fontSize = 10
            };
        }
        
        public static bool InitFiledAfterEnter(string label, ref int len, int left, int right)
        {
            var tlen = Mathf.Clamp(EditorGUILayout.IntField(label, len), left, right);
            if (tlen != len)
            {
                //if (Event.current.isKey && Event.current.keyCode.ToString() == "Return")
                {
                    len = tlen;
                    return true;
                }
            }
            return false;
        }

        
        public static bool LinkLabel(string content, GUIStyle style = null)
        {
            var method = LinkLabelMethod();
            GUIStyle orgStyle = null;
            orgStyle = SetStyle(linkLabelStyle);
            object[] param = {content, null};
            var res = method.Invoke(null, param);
            if(orgStyle != null)
                SetStyle(orgStyle);
            return res is bool ? (bool) res : false;
        }

        static GUIStyle SetStyle(GUIStyle style)
        {
            var curStyle = typeof(EditorStyles).GetField("s_Current", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var field = typeof(EditorStyles).GetField("m_LinkLabel", BindingFlags.Instance | BindingFlags.NonPublic);
            var orgStyle = field.GetValue(curStyle);
            field.SetValue(curStyle, style);
            return orgStyle as GUIStyle;
        }
	
        static MethodInfo LinkLabelMethod()
        {
            var methods = typeof(EditorGUILayout).GetMethods (BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                if (method.Name.Equals("LinkLabel"))
                {
                    var ps = method.GetParameters();
                    foreach (var parameterInfo in ps)
                    {
                        if (parameterInfo.ParameterType.Name == "String")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }
    }
}
