


using Tenmove.Runtime;
using UnityEngine;

namespace Tenmove.Editor.Unity
{
    public class PageParam { }
    public abstract class UnityEditorPageBase
    {
        private static readonly uint InvalidID = ~0u;

        private class NotificationContext
        {
            private readonly UnityEditorPageBase m_NotifyPage;
            private long m_TimeStamp;
            private string m_Message;
            private GUIStyle m_Style;
            private long m_TimeLength;
            private bool m_Expired;

            public NotificationContext(UnityEditorPageBase notifyPage)
            {
                Runtime.Debugger.Assert(null != notifyPage, "Parameter 'notifyPage' can not be null!");
                m_NotifyPage = notifyPage;
                m_TimeStamp = 0;
                m_Message = string.Empty;
                m_Style = null;
                m_TimeLength = 0;
                m_Expired = true;
            }

            public void Show(string message,GUIStyle style,float timeSeconds)
            {
                m_TimeStamp = Runtime.Utility.Time.GetTicksNow();
                m_TimeLength = Runtime.Utility.Time.SecondsToTicks(timeSeconds);
                m_Message = message;
                m_Style = style ?? "NotificationBackground";
                m_Expired = false;
            }

            public void OnGUI()
            {
                if (m_Expired)
                    return;

                long ticks = Runtime.Utility.Time.GetTicksNow();
                if (ticks < m_TimeStamp + m_TimeLength)
                    m_NotifyPage._DisplayNotification(m_Message, m_Style);
                else
                    m_Expired = true;
            }

            public void OnCompiling()
            {
                m_Expired = true;
            }
        }

        private readonly UnityEditorPanelBase m_ParentPanel;
        private readonly uint m_ID;

        private readonly NotificationContext m_NotificationContext;

        public UnityEditorPageBase(UnityEditorPanelBase parentPanel, uint pageID)
        {
            Runtime.Debugger.Assert(InvalidID != pageID, "Parameter 'pageID' can not be invalid value!");
            Runtime.Debugger.Assert(null != parentPanel, "Parameter 'parentPanel' can not be null!");

            m_ID = pageID;
            m_ParentPanel = parentPanel;

            m_NotificationContext = new NotificationContext(this);
        }

        public uint ID
        {
            get { return m_ID; }
        }

        public Rect PanelRect
        {
            get { return m_ParentPanel.position; }
        }

        public UnityEditorPanelBase Panel
        {
            get { return m_ParentPanel; }
        }

        public abstract bool NeedUpdate { get; }

        public void Update()
        {
            _OnUpdate();
        }

        public void DrawPageGUI()
        {
            _OnPageGUI();

            m_NotificationContext.OnGUI();
        }

        public void Init()
        {
            _OnInit();
        }

        public void Deinit()
        {
            _OnDeinit();
        }

        public void Deactive()
        {
            _OnDeactive();
        }

        public void Active(PageParam param)
        {
            _OnActive(param);
        }

        protected virtual void _OnUpdate() { }

        public virtual void OnCompiling() { m_NotificationContext.OnCompiling(); }
        public virtual void OnCompileComplete() { }

        protected abstract void _OnPageGUI();

        protected abstract void _OnInit();
        protected abstract void _OnDeinit();

        protected virtual void _OnActive(PageParam param) { }
        protected virtual void _OnDeactive() { }

        protected void _DisplayNotification(string message, GUIStyle style,float timeSeconds)
        {
            m_NotificationContext.Show(message, style, timeSeconds);
        }

        protected void _DisplayNotification(string message,GUIStyle style)
        {
            GUILayout.BeginArea(new Rect(0, 0, PanelRect.width, PanelRect.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(message), style);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}