

namespace Tenmove.Editor.Unity
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Tenmove.Editor.Unity.Widgets;
    using Tenmove.Runtime;
    using Tenmove.Runtime.Unity;
    using UnityEditor;
    using UnityEngine;

    internal partial class UnityGameProfileTool
    {
        private class MainPageParam : PageParam
        {
        }

        private partial class MainPage : UnityEditorPageBase,IEquatable<MainPage>
        {
            private class AssetDesc
            {
                private string m_AssetPath;
                private UnityEngine.Object m_Asset;

                public string AssetPath
                {
                    get { return m_AssetPath; }
                }

                public UnityEngine.Object Asset
                {
                    set
                    {
                        if(m_Asset != value)
                        {
                            m_Asset = value;
                            m_AssetPath = AssetDatabase.GetAssetPath(m_Asset);
                            if (null != m_AssetPath)
                                m_AssetPath.Replace("Assets/Resources", null);
                            else
                                m_AssetPath = string.Empty;
                        }
                    }

                    get
                    {
                        if (null == m_Asset)
                        {
                            if (!string.IsNullOrEmpty(m_AssetPath))
                                m_Asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                                    Utility.Path.Combine("Assets/Resources", m_AssetPath));
                        }

                        return m_Asset;
                    }
                }
            }

            private class NetMessageGUIInstance
            {
                private readonly MainPage m_MainPage;
                private readonly ITMGameProfileMessageGUI m_MessageGUI;
                private bool m_IsRemoved;

                public NetMessageGUIInstance(MainPage mainPage ,ITMGameProfileMessageGUI messageGUI)
                {
                    Debugger.Assert(null != mainPage, "Parameter 'mainPage' can not be null!");
                    Debugger.Assert(null != messageGUI, "Parameter 'messageGUI' can not be null!");

                    m_MainPage = mainPage;
                    m_MessageGUI = messageGUI;

                    m_IsRemoved = false;
                }

                public bool IsRemoved { get { return m_IsRemoved; } }
                public bool FoldOut { get; set; }
                public bool IsValid
                {
                    get { return m_MessageGUI.CheckValid(); }
                }

                public void OnGUI()
                {
                    if(FoldOut)
                    {
                        EditorGUILayout.BeginVertical("box");
                        if(GUILayout.Button(string.Format("－ 消息{0}", m_MessageGUI.Message.GetType().Name), Styles.Default.Label))
                            FoldOut = !FoldOut;

                        EditorGUILayout.BeginVertical("box");
                        m_MessageGUI.OnGUI();
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        if (GUILayout.Button(string.Format("＋ 消息{0}", m_MessageGUI.Message.GetType().Name), Styles.Default.Label))
                            FoldOut = !FoldOut;

                        EditorGUI.BeginDisabledGroup(!m_MainPage._NoConnections && !IsValid);
                        if (GUILayout.Button("发送到设备", Styles.Default.Button, GUILayout.Width(100)))
                        {
                            if (m_MainPage.m_ProfilerServer.IsServerOnline)
                                m_MainPage.m_ProfilerServer.SendMessage(m_MessageGUI.Message);
                        }
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("删除", Styles.Default.Button, GUILayout.Width(40)))
                        {
                            m_MessageGUI.Recycle();
                            m_IsRemoved = true;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            private readonly ITMUnityGameProfileServer m_ProfilerServer;
            private readonly List<AssetDesc> m_AssetPackList;

            private readonly UnityTypeDropButton m_MessageDropList;
            private readonly List<NetMessageGUIInstance> m_MessageGUIList;
            private int m_CurrentFoldOutGUI;

            private ITMGameProfileMessageGUI m_CurrentMessage;

            private bool m_TurnOnServer;
            private Vector2 m_ClientScroll;
            private Vector2 m_AssetPackScroll;
            private Vector2 m_NetMessageScroll;

            public MainPage(UnityEditorPanelBase parentWindow, uint pageID)
                : base(parentWindow, pageID)
            {
                m_TurnOnServer = false;
                m_AssetPackList = new List<AssetDesc>();
                m_MessageDropList = new UnityTypeDropButton();
                m_CurrentMessage = null;
                m_CurrentFoldOutGUI = -1;
                m_MessageGUIList = new List<NetMessageGUIInstance>();
                m_ProfilerServer = ModuleManager.GetModule<ITMUnityGameProfileServer>();
            }

            public sealed override bool NeedUpdate
            {
                get { return true; }
            }

            protected bool _NoConnections
            {
                get { return !m_TurnOnServer || null == m_ProfilerServer || !m_ProfilerServer.HasConnection; }
            }

            public bool Equals(MainPage other)
            {
                return ID == other.ID;
            }

            public override void OnCompiling()
            {
                base.OnCompiling();
                _DisplayNotification("正在编译，请稍后……", Styles.Default.Notification);
            }

            public override void OnCompileComplete()
            {
                base.OnCompileComplete();
                _RefreshAssetPackList();
            }

            protected sealed override void _OnUpdate()
            {
                base._OnUpdate();
            }

            protected sealed override void _OnPageGUI()
            {
                GUILayout.BeginArea(new Rect(0, 20, PanelRect.width, PanelRect.height / 2.0f), EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                m_TurnOnServer = GUILayout.Toggle(m_TurnOnServer, "WIFI", Styles.Default.Button, GUILayout.Width(50));
                string label = string.Empty;
                if (m_TurnOnServer)
                {
                    m_ProfilerServer.StartServer();
                    m_ProfilerServer.RegisterMessageProcessor<GameProfileTransmitFileEnd,MainPage>(_OnTransmitFileFinished,this);
                    label = string.Format("本机IP：{0}", m_ProfilerServer.ServerIP);
                }
                else
                {
                    m_ProfilerServer.ShutdownServer();
                    label = "OFFLINE";
                }
                GUILayout.Label(label, Styles.Default.Button);

                EditorGUILayout.EndHorizontal();

                _DrawConnectedClientList(m_ProfilerServer, m_TurnOnServer);
                _DrawAssetPackConfigure(_NoConnections);

                GUILayout.EndArea();

                float heightPos = 20 + (PanelRect.height / 2.0f);
                Rect rc = new Rect(0, heightPos, PanelRect.width, PanelRect.height - heightPos);
                GUILayout.BeginArea(rc, EditorStyles.helpBox);
                _DrawNetMessagePanel(rc, _NoConnections);
                GUILayout.EndArea();
            }

            protected override void _OnInit()
            {
                m_MessageDropList.Init(Utility.Assembly.GetTypesOf(typeof(ITMNetMessageGameProfile)), _OnDropListSelected);
                _RefreshAssetPackList();
            }

            protected override void _OnDeinit()
            {
            }

            protected override void _OnActive(PageParam param)
            {
                base._OnActive(param);
                MainPageParam mainPageParam = param as MainPageParam;
                if (null != mainPageParam)
                {

                }
            }

            private void _DrawAssetPackConfigure( bool noConnections)
            {
                GUILayout.Label("需要打包的资源列表：", Styles.Default.Label);

                EditorGUILayout.BeginVertical("box");
                m_AssetPackScroll = EditorGUILayout.BeginScrollView(m_AssetPackScroll);
                for (int i = 0, icnt = m_AssetPackList.Count; i < icnt; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    m_AssetPackList[i].Asset = EditorGUILayout.ObjectField(m_AssetPackList[i].Asset, typeof(GameObject), false, GUILayout.Width(PanelRect.width - 95));
                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        m_AssetPackList.RemoveAt(i);
                        --i;
                        --icnt;
                    } 
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("+"))
                    m_AssetPackList.Add(new AssetDesc());
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                if (GUILayout.Button("打包选中资源", Styles.Default.Button))
                {
                    List<string> assetList = FrameStackList<string>.Acquire();
                    for(int i = 0,icnt = m_AssetPackList.Count;i<icnt;++i)
                    {
                        string assetPath = m_AssetPackList[i].AssetPath;
                        if (!string.IsNullOrEmpty(assetPath))
                            assetList.Add(assetPath);
                    }

                    m_ProfilerServer.PackAssets(assetList);
                    FrameStackList<string>.Recycle(assetList);
                }
                EditorGUI.EndDisabledGroup();

                bool hasNoContent = !m_ProfilerServer.CurrentPackageDesc.HasContent;
                EditorGUI.BeginDisabledGroup(noConnections || hasNoContent);
                if (GUILayout.Button("同步到设备", Styles.Default.Button))
                {
                    if (null != m_ProfilerServer.CurrentPackageDesc)
                    {
	                    using (Stream stream = Utility.File.OpenRead(m_ProfilerServer.CurrentPackageDesc.PackageName))
	                    {
	                        NetMessageTransmitFile msg = m_ProfilerServer.CreateMessage<GameProfileTransmitFile>();
	                        string fileName = Utility.Path.GetFileName(m_ProfilerServer.CurrentPackageDesc.PackageName);
	                        msg.Fill(fileName, stream);
	                        m_ProfilerServer.SendMessage(msg);
	                    }
					}
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
            }

            private void _DrawConnectedClientList(ITMUnityGameProfileServer server,bool enableServer)
            {
                GUILayout.Label("已连接的设备：", Styles.Default.Label);

                EditorGUILayout.BeginVertical("box");
                m_ClientScroll = EditorGUILayout.BeginScrollView(m_ClientScroll, GUILayout.Height(60));

                List<ITMNetConnection> connections = FrameStackList<ITMNetConnection>.Acquire();
                if (null != server)
                    server.GetConnections(connections);
                if (connections.Count > 0)
                {
                    for (int i = 0, icnt = connections.Count; i < icnt; ++i)
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        GUILayout.Label(connections[i].IP, Styles.Default.Label);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    if(enableServer)
                        Extension.EditorGUILayout.HelpBox("正在等待设备链接...", MessageType.Info, Styles.Default.HelpBox);
                    else
                        Extension.EditorGUILayout.HelpBox("请先开启WIFI链接。", MessageType.Warning, Styles.Default.HelpBox);
                }

                FrameStackList<ITMNetConnection>.Recycle(connections);

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            private void _DrawNetMessagePanel(Rect rc, bool noConnections)
            {
                GUILayout.Label("网络消息配置:", Styles.Default.Label);

                EditorGUILayout.BeginVertical("box");
                m_NetMessageScroll = EditorGUILayout.BeginScrollView(m_NetMessageScroll);
                bool isNetMessageValid = false;
                if (m_ProfilerServer.IsServerOnline)
                {
                    EditorGUILayout.BeginHorizontal();

                    m_MessageDropList.Draw("创建网络消息", Styles.Default.Button);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    
                    for(int i = 0,icnt = m_MessageGUIList.Count;i<icnt;++i)
                    {
                        NetMessageGUIInstance cur = m_MessageGUIList[i];

                        cur.FoldOut = m_CurrentFoldOutGUI == i;
                        cur.OnGUI();
                        if (cur.FoldOut)
                            m_CurrentFoldOutGUI = i;
                        else
                        {
                            if (m_CurrentFoldOutGUI == i)
                                m_CurrentFoldOutGUI = ~0;
                        }

                        if (cur.IsRemoved)
                        {
                            m_MessageGUIList.RemoveAt(i);
                            --i;
                            --icnt;
                        }
                    }

                    EditorGUILayout.Space();
                }
                else
                    Extension.EditorGUILayout.HelpBox("请先开启WIFI链接。", MessageType.Warning, Styles.Default.HelpBox);

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(noConnections && !isNetMessageValid);
                if (GUILayout.Button("发送消息到设备", Styles.Default.Button))
                {
                    if (m_ProfilerServer.IsServerOnline && null != m_CurrentMessage)
                        m_ProfilerServer.SendMessage(m_CurrentMessage.Message);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }


            private void _RefreshAssetPackList()
            {
                m_ProfilerServer.RefreshPackageDesc();
                m_AssetPackList.Clear();
                if (null != m_ProfilerServer.CurrentPackageDesc)
                {
	                for (int i = 0, icnt = m_ProfilerServer.CurrentPackageDesc.PackageAssets.Count; i < icnt; ++i)
	                    m_AssetPackList.Add(new AssetDesc() { Asset = AssetDatabase.LoadAssetAtPath<GameObject>(m_ProfilerServer.CurrentPackageDesc.PackageAssets[i]) });
                }
            }

            private void _OnDropListSelected(Type messageType)
            {
                if (m_ProfilerServer.IsServerOnline)
                    m_MessageGUIList.Add(new NetMessageGUIInstance(this,_CreateSimulatorGUI(messageType)));
            }

            private static void _OnTransmitFileFinished(GameProfileTransmitFileEnd message,MainPage _this)
            {
                _this._DisplayNotification(message.Message, Styles.Default.Notification,1.0f);
            }

            private ITMGameProfileMessageGUI _CreateSimulatorGUI(System.Type type)
            {
                Type guiType = Instance._AcquireMessageGUIType(type);
                if (null != guiType)
                    return Utility.Assembly.CreateInstance(guiType, m_ProfilerServer, m_ProfilerServer.CreateMessage(type)) as ITMGameProfileMessageGUI;

                return null;
            }
        }
    }
}