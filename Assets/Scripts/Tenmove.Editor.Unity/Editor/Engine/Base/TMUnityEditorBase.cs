

using System;
using System.Collections.Generic;
using Tenmove.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tenmove.Editor.Unity
{
    public enum DockSide
    {
        Top,
        Bottom,
        Left,
        Right,
    }

    public abstract partial class UnityEditorBase : UnityEditorMainPanelBase
    {
        private readonly LinkedList<EditorWindow> m_ChildPanelList;
        private readonly FrameTrigger m_FrameTrigger;

        private UnityEngine.Object m_StageNavigationManagerInstance;
        private UnityEngine.SceneManagement.Scene m_EditorScene;
        private string m_OriginScene;
        private uint m_FrameCount;
        private GameObject m_PrefabModeTarget;


        public UnityEditorBase()
        {
            m_ChildPanelList = new LinkedList<EditorWindow>();
            m_FrameTrigger = new FrameTrigger();
            m_FrameCount = 0;

        }

        private UnityEngine.Object StageNavigationManagerInstance
        {
            get
            {
                if(null == m_StageNavigationManagerInstance)
                {
                    //这个类继承自单例泛型类ScriptableSingleton<T>
                    Type type = Runtime.Utility.Assembly.GetType("UnityEditor.SceneManagement.StageNavigationManager");
                    m_StageNavigationManagerInstance = Reflector.Type(type.BaseType).Properties["instance"].GetValue<UnityEngine.Object>();
                }

                return m_StageNavigationManagerInstance;
            }
        }

        protected bool _IsMainStage
        {
            get
            {
                if (null != StageNavigationManagerInstance)
                {
                    object currentStageNavigationItem = Reflector.Type("UnityEditor.SceneManagement.StageNavigationManager", m_StageNavigationManagerInstance).Properties["currentItem"].GetValue<object>();
                    if(null != currentStageNavigationItem)
                        return Reflector.Type("UnityEditor.SceneManagement.StageNavigationItem", currentStageNavigationItem).Properties["isMainStage"].GetValue<bool>();
                }

                return true;
            }
        }

        protected uint _AddFrameTrigger<T,U>(uint triggerAfterFrame, T initiator, U userData, Function<T, U> callback, bool isLoop = false)
        {
            return m_FrameTrigger.AddTrigger(m_FrameCount, triggerAfterFrame, initiator, userData, callback, isLoop);
        }

        protected void _RemoveFrameTrigger(uint handle)
        {
            m_FrameTrigger.RemoveTrigger(handle);
        }

        protected T _CreateChildPanel<T>(string title, bool isDockablePanel) where T : EditorWindow
        {
            T newPanel = UnityEditorPanelBase.GetPanel<T>(title, isDockablePanel);
            if (null != newPanel)
                m_ChildPanelList.AddLast(newPanel);
            return newPanel;
        }

        protected void _OpenIsolateScene()
        {
            if (string.IsNullOrEmpty(m_OriginScene))
            {
                m_OriginScene = EditorSceneManager.GetActiveScene().path;
                m_EditorScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            }
        }

        protected GameObject _OpenPrefabScene(string prefabPath)
        {
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debugger.LogWarning("Parameter 'prefabPath' can not be null or empty string!");
                return null;
            }

            m_PrefabModeTarget = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (null != m_PrefabModeTarget)
                UnityEditor.AssetDatabase.OpenAsset(m_PrefabModeTarget);
            else
                Debugger.LogWarning("Can not open prefab with path '{0}'!", prefabPath);

            return m_PrefabModeTarget;
        }

        protected override bool _OnInit()
        {
            EditorApplication.playModeStateChanged += _PlayModeChanged;
            return true;
        }

        protected override void _OnDeinit()
        {
            EditorApplication.playModeStateChanged -= _PlayModeChanged;
            if (!string.IsNullOrEmpty(m_OriginScene))
            {
                EditorSceneManager.OpenScene(m_OriginScene, OpenSceneMode.Single);
                m_OriginScene = string.Empty;
            }

            m_FrameTrigger.Shutdown();
        }

        protected override void _OnCompileStart()
        {
            EditorApplication.playModeStateChanged -= _PlayModeChanged;
            base._OnCompileStart();
        }

        protected override void _OnCompileComplete()
        {
            base._OnCompileComplete();
            EditorApplication.playModeStateChanged += _PlayModeChanged;
        }

        static protected EditorWindow _CreateUnityInternalPanel(string panelTypeName, bool isDockablePanel)
        {
            return Extractor.CreateInternalPanel(panelTypeName, isDockablePanel);
        }

        protected static string _OpenFolderPanel(string title, string folder, string defaultName)
        {
            return EditorUtility.OpenFolderPanel(title, folder, defaultName);
        }

        protected static string _OpenFilePanel(string title, string directory, string extension)
        {
            return EditorUtility.OpenFilePanel(title, directory, extension);
        }

        protected static string _OpenFilePanelWithFilters(string title, string directory, string[] filters)
        {
            return EditorUtility.OpenFilePanelWithFilters(title, directory, filters);
        }

        protected static string _SaveFilePanel(string title, string directory, string defaultName, string extension)
        {
            return EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
        }

        protected static string _SaveFilePanelInProject(string title, string defaultName, string extension, string message, string path)
        {
            return EditorUtility.SaveFilePanelInProject(title, defaultName, extension, message, path);
        }

        protected static string _SaveFilePanelInProject(string title, string defaultName, string extension, string message)
        {
            return EditorUtility.SaveFilePanelInProject(title, defaultName, extension, message);
        }

        protected static string _SaveFolderPanel(string title, string folder, string defaultName)
        {
            return EditorUtility.SaveFolderPanel(title, folder, defaultName);
        }

        /// <summary>
        /// 退出prefab模式时强制打开当前编辑的Prefab
        /// </summary>
        private void _LockInPrefabStage()
        {
            if (_IsMainStage)
            {
                if (null != m_PrefabModeTarget)
                    AssetDatabase.OpenAsset(m_PrefabModeTarget);
            }
        }

        private void Update()
        {
            ++m_FrameCount;
            _OnUpdate();

            m_FrameTrigger.Update(m_FrameCount);

            _LockInPrefabStage();
        }
         
        private void _PlayModeChanged(PlayModeStateChange state)
        {
            LinkedListNode<EditorWindow> cur = m_ChildPanelList.First;
            while (null != cur)
            {
                UnityEditorPanelBase curPanel = cur.Value as UnityEditorPanelBase;
                if (null != curPanel)
                    curPanel.OnPlayModeChanged(state);

                cur = cur.Next;
            }

            OnPlayModeChanged(state);
        }
    }

    public abstract class UnityEditorBase<TEditor> : UnityEditorBase where TEditor : UnityEditorBase
    {
        static private TEditor sm_Instance;

        public UnityEditorBase()
        {
        }

        protected override void _OnDeinit()
        {
            base._OnDeinit();
            sm_Instance = null;
        }

        static protected TEditor Instance
        {
            get  { return sm_Instance; }
        }

        static protected TEditor _CreateInstance(string title, bool isDockablePanel)
        {
            if (null == sm_Instance)
                sm_Instance = UnityEditorPanelBase.GetPanel<TEditor>(title, isDockablePanel);

            return sm_Instance;
        }
    }
}