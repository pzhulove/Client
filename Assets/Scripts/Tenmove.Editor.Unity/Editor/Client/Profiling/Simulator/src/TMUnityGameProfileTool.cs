

namespace Tenmove.Editor.Unity
{
    using System;
    using System.Collections.Generic;
    using Tenmove.Runtime;
    using Tenmove.Runtime.Unity;
    using UnityEditor;

    internal partial class UnityGameProfileTool : UnityEditorBase<UnityGameProfileTool>
    {
        static private readonly string Title = "Game Profile Tool";

        private readonly Dictionary<System.Type, System.Type> m_ProfilerMessageGUITable;

        public UnityGameProfileTool()
        {
            m_ProfilerMessageGUITable = new Dictionary<Type, Type>();
        }

        [MenuItem("Tenmove/Game Profile Tool")]
        public static void Open()
        {
            UnityGameProfileTool _this = _CreateInstance(Title, true);

            _this.RegisterSimulatorMessageGUI<GameProfileCreateAsset, GameProfileCreateAssetGUI>();
            _this.RegisterSimulatorMessageGUI<GameProfileClearAllAssets, GameProfileClearAllAssetsGUI>();
        }

        public void RegisterSimulatorMessageGUI<T, TGUI>()
            where T : ITMNetMessageGameProfile
            where TGUI : GameProfileMessageGUI<T>
        {
            System.Type messageType = typeof(T);
            System.Type messageGUIType = typeof(TGUI);
            if (!m_ProfilerMessageGUITable.ContainsKey(messageType))
                m_ProfilerMessageGUITable.Add(messageType, messageGUIType);
            else
                Debugger.LogWarning("Simulator message [type:{0}] has already reigister a GUI!", messageType.Name);
        }

        protected override bool _OnInit()
        {
            if (base._OnInit())
            {
                return true;
            }
            else
                return false;
        }

        protected override void _OnDeinit()
        {
            base._OnDeinit();
        }

        protected override void _OnActive()
        {
            OpenPanelPage<MainPage>(
                new MainPageParam()
                {
                });
        }

        protected override void _OnDeactive()
        {
        }

        private System.Type _AcquireMessageGUIType(System.Type messageType)
        {
            Type guiType = null;
            if (m_ProfilerMessageGUITable.TryGetValue(messageType, out guiType))
                return guiType;

            return null;
        }

        protected override void _OnEndEditMode()
        {
            base._OnEndEditMode();
            Close();
        }
    }
}