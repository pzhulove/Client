using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Tenmove.Editor.Unity.Widgets
{
    public class TMSearchField : SearchField
    {
        private EditorWindow m_EditorWindow;

        private float m_FinishTime;

        private string m_SearchString = "";

        public float delay
        {
            get;
            set;
        }

        public string text
        {
            get;
            private set;
        }

        public TMSearchField(EditorWindow editorWindow)
        {
            base.autoSetFocusOnFindCommand = false;
            this.delay = 0.5f;
            this.text = this.m_SearchString;
            this.m_EditorWindow = editorWindow;
        }

        public bool OnToolbarGUI()
        {
            bool flag = base.HasFocus() && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
            bool flag2 = base.HasFocus() && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
            string text = base.OnToolbarGUI(this.m_SearchString);
            if (text != this.m_SearchString)
            {
                this.m_SearchString = text;
                this.m_FinishTime = Time.realtimeSinceStartup + this.delay;
            }
            if (flag | flag2)
            {
                this.m_FinishTime = 0f;
            }
            if (this.m_FinishTime > Time.realtimeSinceStartup)
            {
                this.m_EditorWindow.Repaint();
                return false;
            }
            if (this.m_SearchString != this.text)
            {
                this.text = this.m_SearchString;
                this.m_EditorWindow.Repaint();
                return true;
            }
            return false;
        }
    }
}
