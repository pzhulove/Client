using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class TraceFrame
    {
        ClientSystemTownFrame clientFrame;
        GameObject root;

        enum TraceFrameType
        {
            TFT_NONE = -1,

            TFT_MISSION = 0,
            TFT_TEAM,
            TFT_COUNT,
        }

        Toggle[] m_akToggles = new Toggle[(int)TraceFrameType.TFT_COUNT];
        GameObject[] m_akCheckNormals = new GameObject[(int)TraceFrameType.TFT_COUNT];
        GameObject[] m_akCheckMarks = new GameObject[(int)TraceFrameType.TFT_COUNT];

        /// <summary>
        /// 是否根据消息打开
        /// </summary>
        private bool mOpenByMessage = false;

        private TraceFrameType mLastSelectFrameType = TraceFrameType.TFT_NONE;

        public void InitTraceFrame(ClientSystemTownFrame clientFrame, GameObject root)
        {
            mLastSelectFrameType = TraceFrameType.TFT_NONE;

            this.clientFrame = clientFrame;
            this.root = root;
            GameObject goPrefabs = Utility.FindChild(root, "toggle");
            goPrefabs.CustomActive(false);
            string[] names = new string[] { TR.Value("traceframe_mission"), TR.Value("traceframe_team") };
            for (int i = 0; i < m_akToggles.Length; ++i)
            {
                GameObject goCurrent = GameObject.Instantiate(goPrefabs) as GameObject;

                if(i == 0)
                {
                    goCurrent.name = "mission";
                }
                else if(i == 1)
                {
                    goCurrent.name = "team";
                }

                Utility.AttachTo(goCurrent, root);
                goCurrent.CustomActive(true);

                m_akCheckNormals[i] = Utility.FindChild(goCurrent, "CheckNormal");
                m_akCheckMarks[i] = Utility.FindChild(goCurrent, "CheckMark");

                TraceFrameType eTraceFrameType = (TraceFrameType)i;

                m_akToggles[i] = goCurrent.GetComponent<Toggle>();
                m_akToggles[i].onValueChanged.AddListener((bool bValue) =>
                {
                    _OnToggleChanged(bValue, eTraceFrameType);
                });

                Text name = Utility.FindComponent<Text>(goCurrent, "CheckNormal/Text");
                name.text = names[i];
                name = Utility.FindComponent<Text>(goCurrent, "CheckMark/Text");
                name.text = names[i];
            }

            m_akToggles[(int)TraceFrameType.TFT_MISSION].isOn = true;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamCreateSuccess, _OnCreateTeamSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OutPkWaitingScene, _OnSwitchToMission);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SwitchToMission, _OnSwitchToMission);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamJoinSuccess, _OnJoinTeamSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, _OnRemoveMemberSuccess);
        }

        public void UnInitTraceFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamCreateSuccess, _OnCreateTeamSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OutPkWaitingScene, _OnSwitchToMission);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SwitchToMission, _OnSwitchToMission);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamJoinSuccess, _OnJoinTeamSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, _OnRemoveMemberSuccess);

            for (int i = 0; i < m_akToggles.Length; ++i)
            {
                m_akToggles[i].onValueChanged.RemoveAllListeners();
                m_akToggles[i] = null;
                m_akCheckNormals[i] = null;
                m_akCheckMarks[i] = null;
            }
            root = null;
            clientFrame = null;

            ClientSystemManager.GetInstance().CloseFrame<TeamMainMenuFrame>();
            ClientSystemManager.GetInstance().CloseFrame<TeamMainFrame>();
            ClientSystemManager.GetInstance().CloseFrame<FunctionFrame>();
        }

        void _OnCreateTeamSuccess(UIEvent uiEvent)
        {
            _SwitchPage((int)TraceFrameType.TFT_TEAM);
        }

        void _OnSwitchToMission(UIEvent uiEvent)
        {
            _SwitchPage((int)TraceFrameType.TFT_MISSION);
        }

        void _OnJoinTeamSuccess(UIEvent ievent)
        {
            _SwitchPage((int)TraceFrameType.TFT_TEAM);
        }

        void _OnRemoveMemberSuccess(UIEvent ievent)
        {
            _SwitchPage((int)TraceFrameType.TFT_TEAM);
        }


        void _SwitchPage(int iIndex)
        {
            for (int i = 0; i < m_akToggles.Length; i++)
            {
                if (i == iIndex)
                {
                    mOpenByMessage = true;

                    m_akToggles[i].isOn = false;
                    m_akToggles[i].isOn = true;

                    mOpenByMessage = false;
                    
                    break;
                }
            }
        }

        void _OnToggleChanged(bool bValue, TraceFrameType eTraceFrameType)
        {
            if (bValue)
            {
                bool openRelateFrame = !mOpenByMessage && mLastSelectFrameType == eTraceFrameType;

                if (eTraceFrameType == TraceFrameType.TFT_MISSION)
                {
                    _OnMissionTrace(openRelateFrame);
                }
                else if (eTraceFrameType == TraceFrameType.TFT_TEAM)
                {
                    _OnTeam(openRelateFrame/* && TeamDataManager.GetInstance().HasTeam()*/ && !NewbieGuideManager.GetInstance().IsGuidingTask((NewbieGuideTable.eNewbieGuideTask.TeamGuide)));
                }

                mLastSelectFrameType = eTraceFrameType;
            }

            m_akCheckMarks[(int)eTraceFrameType].CustomActive(bValue);
            m_akCheckNormals[(int)eTraceFrameType].CustomActive(!bValue);
        }

        void _OnMissionTrace(bool openMissionFrame)
        {
            ClientSystemManager.GetInstance().CloseFrame<TeamMainMenuFrame>();
            ClientSystemManager.GetInstance().CloseFrame<TeamMainFrame>();

            if (!ClientSystemManager.GetInstance().IsFrameOpen<FunctionFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<FunctionFrame>(FrameLayer.Bottom);
            }

            TeamUtility.OnMissionTraceSelectTeam(false);

            if (openMissionFrame)
            {
                MissionFrameNewData data = new MissionFrameNewData
                {
                    iFirstFilter = (int)MissionFrameNew.FilterFirstType.FFT_MAIN_OR_BRANCH
                };

                ClientSystemManager.GetInstance().OpenFrame<MissionFrameNew>(FrameLayer.Middle, data);
            }
        }

        void _OnTeam(bool openTeamListFrame)
        {
            ClientSystemManager.GetInstance().CloseFrame<FunctionFrame>();

            TeamUtility.OnMissionTraceSelectTeam(true);

            if (openTeamListFrame)
            {
                TeamListFrame.TryOpenTeamListOrTeamMyFrame();
                //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle);
            }
        }
    }
}