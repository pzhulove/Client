

using UnityEditor;
using UnityEngine;

namespace Tenmove.Editor.Unity
{
    using System.Collections.Generic;
    using Tenmove.Runtime;
    using Tenmove.Runtime.Unity;

    public interface ITMGameProfileMessageGUI
    {
        NetMessage Message { get; }
        void OnGUI();
        bool CheckValid();
        void Recycle();
    }

    public abstract class GameProfileMessageGUI<T> : ITMGameProfileMessageGUI where T : ITMNetMessageGameProfile
    {
        protected readonly ITMUnityGameProfileServer m_ProfileServer;
        protected readonly T m_Message;

        public GameProfileMessageGUI(ITMUnityGameProfileServer server, T message)
        {
            Debugger.Assert(null != server, "Parameter 'server' can not be null!");
            Debugger.Assert(null != message, "Parameter 'message' can not be null!");

            m_ProfileServer = server;
            m_Message = message;
        }

        public NetMessage Message
        {
            get { return m_Message.NetMessage; }
        }

        public virtual void OnGUI()
        {
            Extension.EditorGUILayout.HelpBox(string.Format("该网络消息【类型：{0}】没有任何可配置的参数!", GetType().Name), MessageType.Info, Styles.Default.HelpBox);
        }

        public abstract bool CheckValid();

        public void Recycle()
        {
        }
    }

    internal partial class UnityGameProfileTool
    {
        private class GameProfileCreateAssetGUI : GameProfileMessageGUI<GameProfileCreateAsset>
        {
            readonly private List<string> m_AssetList;
            private string[] m_AssetNames;
            private int m_InstCount;
            private Vector2 m_Range;

            private string m_MD5;
            private int m_CurSelected;

            public GameProfileCreateAssetGUI(ITMUnityGameProfileServer server, GameProfileCreateAsset message)
                : base(server, message)
            {
                m_AssetNames = null;
                m_AssetList = new List<string>();
                m_CurSelected = -1;
                m_MD5 = string.Empty;
            }

            public override bool CheckValid()
            {
                return !string.IsNullOrEmpty(m_Message.AssetName);
            }

            public override void OnGUI()
            {
                if (null != m_ProfileServer.CurrentPackageDesc)
                {
                    List<string> packageAssetList = m_ProfileServer.CurrentPackageDesc.PackageAssets;
                    if (null == m_AssetNames || m_AssetNames.Length != packageAssetList.Count)
                        m_AssetNames = new string[packageAssetList.Count];

                    if (m_MD5 != m_ProfileServer.CurrentPackageDesc.PackageMD5)
                    {
                        m_AssetList.Clear();
                        for (int i = 0, icnt = m_AssetNames.Length; i < icnt; ++i)
                        {
                            m_AssetNames[i] = Runtime.Utility.Path.GetFileNameWithoutExtension(packageAssetList[i]);
                            m_AssetList.Add(packageAssetList[i]);
                        }
                    }
                }

                if (null != m_AssetNames && m_AssetNames.Length > 0)
                {
                    int curSelected = EditorGUILayout.Popup("要播放的特效：", m_CurSelected, m_AssetNames);

                    if (0 <= curSelected && curSelected < m_AssetList.Count)
                    {
                        if (curSelected != m_CurSelected)
                        {
                            m_CurSelected = curSelected;
                            m_Message.AssetName = m_AssetList[m_CurSelected].Replace("Assets/Resources/", null);
                        }

                        m_InstCount = EditorGUILayout.IntSlider("创建的实例数量：", m_InstCount, 1, 10);
                        m_Range = EditorGUILayout.Vector2Field("实例创建范围：", m_Range);

                        m_Message.InstCount = m_InstCount;
                        m_Message.Range = m_Range.ToVec2();
                    }
                }
                else
                {
                    Extension.EditorGUILayout.HelpBox("There is not asset in current asset package", MessageType.Warning, Styles.Default.HelpBox);
                }
            }
        }

        private class GameProfileClearAllAssetsGUI : GameProfileMessageGUI<GameProfileClearAllAssets>
        {
            public GameProfileClearAllAssetsGUI(ITMUnityGameProfileServer server, GameProfileClearAllAssets message)
                : base(server, message)
            {
            }

            public override bool CheckValid()
            {
                return true;
            }
        }
    }
}