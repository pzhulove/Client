


using System;
using UnityEditor;
using Tenmove.Runtime;

namespace Tenmove.Editor.Unity
{
    public static class Extractor
    {
        public static EditorWindow CreateInternalPanel(string panelTypeName,bool isDockablePanel)
        {
            EditorWindow panel = null;
            Type panelType = Runtime.Utility.Assembly.GetType(panelTypeName);
            if (null != panelType)
            {
                EditorWindow.CreateInstance(panelType);
                panel = EditorWindow.GetWindow(panelType, !isDockablePanel);
            }
            return panel;           
        }
    }

}