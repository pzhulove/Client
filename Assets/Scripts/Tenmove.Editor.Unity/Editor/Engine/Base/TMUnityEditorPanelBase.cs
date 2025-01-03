

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tenmove.Editor.Unity
{
    public abstract partial class UnityEditorPanelBase : EditorWindow
    {
        private bool m_IsCompiling = false;
        private readonly LinkedList<UnityEditorPageBase> m_EditorPageList;
        private UnityEditorPageBase m_CurrentPage;
        private uint m_EditorPageAllocCount;

        public static TEditorPanel GetPanel<TEditorPanel>(string title, bool isDockablePanel,params Type[] desiredDockNextTo) where TEditorPanel : EditorWindow
        {
            TEditorPanel panel = null;
            if(isDockablePanel)
                panel = EditorWindow.GetWindow<TEditorPanel>( title,desiredDockNextTo);
            else
                panel = EditorWindow.GetWindow<TEditorPanel>(true, title);
            panel.Show();
            panel.autoRepaintOnSceneChange = true;
            panel.ShowTab();
            return panel;
        }

        public UnityEditorPanelBase()
        {
            m_IsCompiling = false;
            m_EditorPageList = new LinkedList<UnityEditorPageBase>();
            m_EditorPageAllocCount = 0;
        }

        private void Awake()
        {
            _OnInit();
        }

        private void OnEnable()
        {
            _OnActive();
        }

        private void OnDisable()
        {
            _OnDeactive();
        }

        private void OnDestroy()
        {
            _OnDeinit();
        }

        protected abstract bool _OnInit();
        protected abstract void _OnDeinit();

        protected abstract void _OnActive();
        protected abstract void _OnDeactive();

        protected virtual void _OnUpdate()
        {
            LinkedListNode<UnityEditorPageBase> it = m_EditorPageList.First;
            while (null != it)
            {
                UnityEditorPageBase cur = it.Value;
                if (cur.NeedUpdate)
                    cur.Update();
                it = it.Next;
            }
        }

        /// <summary>
        /// 绘制事件。
        /// </summary>
        public void OnGUI()
        {
            if (m_IsCompiling && !EditorApplication.isCompiling)
            {
                m_IsCompiling = false;
                _OnCompileComplete();
            }
            else if (!m_IsCompiling && EditorApplication.isCompiling)
            {
                m_IsCompiling = true;
                _OnCompileStart();
            }

            _OnGUI();
             
            if (m_IsCompiling)
                _OnCompiling();
        }

        public void OnPlayModeChanged(PlayModeStateChange playMode)
        {
            switch(playMode)
            {
                case PlayModeStateChange.EnteredEditMode: _OnBeginEditMode(); break;
                case PlayModeStateChange.ExitingEditMode: _OnEndEditMode(); break;
                case PlayModeStateChange.EnteredPlayMode: _OnBeginPlayMode(); break;
                case PlayModeStateChange.ExitingPlayMode: _OnEndPlayMode(); break;
            }
        }

        private void _OnGUI()
        {
            if (null != m_CurrentPage)
                m_CurrentPage.DrawPageGUI();
        }

        /// <summary>
        /// 编译开始事件。
        /// </summary>
        protected virtual void _OnCompileStart() { }

        /// <summary>
        /// 正在编译。
        /// </summary>
        protected virtual void _OnCompiling()
        {
            if (null != m_CurrentPage)
                m_CurrentPage.OnCompiling();
        }

        /// <summary>
        /// 编译完成事件。
        /// </summary>
        protected virtual void _OnCompileComplete()
        {
            if (null != m_CurrentPage)
                m_CurrentPage.OnCompileComplete();
        }

        protected virtual void _OnBeginPlayMode() { }
        protected virtual void _OnEndPlayMode() { }
        protected virtual void _OnBeginEditMode() { }
        protected virtual void _OnEndEditMode() { }

        protected T _CreatePanelPage<T>() where T : UnityEditorPageBase
        {
            T panelPage = Runtime.Utility.Assembly.CreateInstance(typeof(T),this, _AllocatePageID()) as T;
            panelPage.Init();
            m_EditorPageList.AddLast(panelPage);
            return panelPage;
        }

        protected void DestroyPanelPage<T>() where T : UnityEditorPageBase
        {
            Type destroyType = typeof(T);
            if (m_CurrentPage.GetType() == destroyType)
            {
                m_CurrentPage.Deactive();
                m_CurrentPage = null;
            }

            LinkedListNode<UnityEditorPageBase> it = m_EditorPageList.First;
            while (null != it)
            {
                UnityEditorPageBase cur = it.Value;
                if (cur.GetType() == destroyType)
                {
                    cur.Deinit();
                    m_EditorPageList.Remove(it);
                    break;
                }

                it = it.Next;
            }
        }

        protected void OpenPanelPage<T>(PageParam param = null) where T : UnityEditorPageBase
        {
            Type pageType = typeof(T);
            UnityEditorPageBase targetPage = null;
            LinkedListNode<UnityEditorPageBase> it = m_EditorPageList.First;
            while (null != it)
            {
                UnityEditorPageBase cur = it.Value;
                if (cur.GetType() == pageType)
                {
                    targetPage = cur;
                    break;
                }
                it = it.Next;
            }

            if (null == targetPage)
                targetPage = _CreatePanelPage<T>();

            if (null != targetPage)
            {
                if (null != m_CurrentPage)
                    m_CurrentPage.Deactive();

                m_CurrentPage = targetPage;
                m_CurrentPage.Active(param);
            }
        }

        private uint _AllocatePageID()
        {
            return m_EditorPageAllocCount++;
        }
    }

    public abstract class UnityEditorMainPanelBase : UnityEditorPanelBase
    {
        public UnityEditorMainPanelBase()
        {
        }
    }
}