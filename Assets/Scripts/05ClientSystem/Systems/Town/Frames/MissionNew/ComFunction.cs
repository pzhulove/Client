using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public enum FunctionType
    {
        FT_MISSION = 0,
        FT_TEAM,
        FT_COUNT,
    }

    class ComFunction : MonoBehaviour
    {
        public GameObject goTogglePrefab;
        bool m_bInit = false;
        Toggle[] toggles = new Toggle[(int)FunctionType.FT_COUNT];
        FunctionType m_eFunctionType = FunctionType.FT_COUNT;
        string[] m_names = new string[(int)FunctionType.FT_COUNT] { "任务", "组队" };
        string[] m_toggle_names = new string[(int)FunctionType.FT_COUNT] { "mission", "team" };
        string[] m_iconPath = new string[(int)FunctionType.FT_COUNT] { "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Renwu2", "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Duiwu2" };
        string[] m_iconUnselectPath = new string[(int)FunctionType.FT_COUNT] { "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Renwu", "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Duiwu" };
        TeamMemberCountShow memberCountShow = null;

        public GameObject goFunctionRoot;
        string m_mission_prefab_path = "UIFlatten/Prefabs/FunctionFrame/FunctionFrame";
        GameObject goMission;
        ComUIListScript comListScript;
        bool m_bFromEvent = false;

        public ComMissionRedBinder comRedBinder = null;
        void _BinderMissionRedPoint(GameObject goLocal, FunctionType eFunctionType)
        {
            if (null != goLocal)
            {
                if (eFunctionType == FunctionType.FT_MISSION)
                {
                    GameObject goRedPoint = Utility.FindChild(goLocal, "RedPoint");
                    if (null != comRedBinder)
                    {
                        if (null != comRedBinder.onSucceed)
                        {
                            comRedBinder.onSucceed.RemoveAllListeners();
                            comRedBinder.onSucceed.AddListener(() =>
                            {
                                goRedPoint.CustomActive(true);
                            });
                        }
                        if (null != comRedBinder.onFailed)
                        {
                            comRedBinder.onFailed.RemoveAllListeners();
                            comRedBinder.onFailed.AddListener(() =>
                            {
                                goRedPoint.CustomActive(false);
                            });
                        }
                    }
                }
            }
        }

        void _InitToggles()
        {
            if(m_bInit)
            {
                return;
            }
            m_bInit = true;
            if (null == goTogglePrefab)
            {
                return;
            }
            for(int i = 0; i < toggles.Length; ++i)
            {
                GameObject goLocal = GameObject.Instantiate(goTogglePrefab) as GameObject;
                if(null == goLocal)
                {
                    continue;
                }
                Utility.AttachTo(goLocal, gameObject);
                goLocal.CustomActive(true);

                ComCommonBind bind = goLocal.GetComponent<ComCommonBind>();
                if(bind != null)
                {
                    var go = bind.GetCom<TeamMemberCountShow>("memberCntRoot");
                    if((FunctionType)i != FunctionType.FT_TEAM)
                    {
                        GameObject.DestroyImmediate(go.gameObject);            
                        go = null;
                    }
                    else
                    {
                        memberCountShow = go;
                    }
                }

                toggles[i] = goLocal.GetComponent<Toggle>();

                if(null != goLocal)
                {
                    goLocal.name = m_toggle_names[i];
                }

                _BinderMissionRedPoint(goLocal, (FunctionType)i);

                if (null != toggles[i])
                {
                    Text Name = Utility.FindComponent<Text>(goLocal,"CheckNormal/Text");
                    if(null != Name)
                    {
                        Name.text = m_names[i];
                    }
                    Text CheckName = Utility.FindComponent<Text>(goLocal,"CheckMark/Text");
                    if (null != CheckName)
                    {
                        CheckName.text = m_names[i];
                    }

                    Image Icon = Utility.FindComponent<Image>(goLocal, "CheckNormal/Icon");
                    if (Icon != null)
                    {
                        ETCImageLoader.LoadSprite(ref Icon, m_iconUnselectPath[i]);
                    }

                    Image CheckIcon = Utility.FindComponent<Image>(goLocal, "CheckMark/CheckIcon");
                    if (CheckIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref CheckIcon, m_iconPath[i]);
                    }

                    FunctionType eFunctionType = (FunctionType)i;
                    switch(eFunctionType)
                    {
                        case FunctionType.FT_MISSION:
                            {
                                toggles[i].onValueChanged.RemoveListener(_OnMission);
                                toggles[i].onValueChanged.AddListener(_OnMission);
                            }
                            break;
                        case FunctionType.FT_TEAM:
                            {
                                toggles[i].onValueChanged.RemoveListener(_OnTeam);
                                toggles[i].onValueChanged.AddListener(_OnTeam);
                            }
                            break;
                    }
                }
            }
            goTogglePrefab.CustomActive(false);
        }

        void _RegisterEvent()
        {
            MissionManager.GetInstance().onAddNewMission += OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission += OnDeleteMission;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _UpdateMission);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FunctionFrameUpdate, _UpdateMission);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamCreateSuccess, _OnCreateTeamSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OutPkWaitingScene, _OnSwitchToMission);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SwitchToMission, _OnSwitchToMission);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamJoinSuccess, _OnJoinTeamSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, _OnRemoveMemberSuccess);
        }

        void _UnRegisterEvent()
        {
            MissionManager.GetInstance().onAddNewMission -= OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission -= OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission -= OnDeleteMission;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _UpdateMission);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FunctionFrameUpdate, _UpdateMission);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamCreateSuccess, _OnCreateTeamSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OutPkWaitingScene, _OnSwitchToMission);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SwitchToMission, _OnSwitchToMission);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamJoinSuccess, _OnJoinTeamSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, _OnRemoveMemberSuccess);
        }

        public void Initialize()
        {
            _InitToggles();
            m_bFromEvent = true;
            _SetToggle(FunctionType.FT_MISSION);
            m_bFromEvent = false;
            _RegisterEvent();
            m_bDirty = false;
            _UpdateMission();
        }

        void _UpdateMission(UIEvent uiEvent)
        {
            _UpdateMission();
        }

        void OnAddNewMission(uint iTaskID)
        {
            _UpdateMission();
        }

        void OnUpdateMission(uint iTaskID)
        {
            _UpdateMission();
        }

        void OnDeleteMission(uint iTaskID)
        {
            _UpdateMission();
        }

        void _OnCreateTeamSuccess(UIEvent uiEvent)
        {
            m_bFromEvent = true;
            _SetToggle(FunctionType.FT_TEAM);
            m_bFromEvent = false;
        }

        void _OnSwitchToMission(UIEvent uiEvent)
        {
            m_bFromEvent = true;
            _SetToggle(FunctionType.FT_MISSION);
            m_bFromEvent = false;
        }

        void _OnJoinTeamSuccess(UIEvent ievent)
        {
            m_bFromEvent = true;
            _SetToggle(FunctionType.FT_TEAM);
            m_bFromEvent = false;
        }

        void _OnRemoveMemberSuccess(UIEvent ievent)
        {
            m_bFromEvent = true;
            _SetToggle(FunctionType.FT_TEAM);
            m_bFromEvent = false;
        }

        public void UnInitialize()
        {
            goTogglePrefab = null;
            for (int i = 0; i < toggles.Length; ++i)
            {
                var toggle = toggles[i];
                if(null != toggle)
                {
                    switch ((FunctionType)i)
                    {
                        case FunctionType.FT_MISSION:
                            {
                                toggle.onValueChanged.RemoveListener(_OnMission);
                            }
                            break;
                        case FunctionType.FT_TEAM:
                            {
                                toggle.onValueChanged.RemoveListener(_OnTeam);
                            }
                            break;
                    }
                }
                toggles[i] = null;
            }
            m_eFunctionType = FunctionType.FT_COUNT;
            goFunctionRoot = null;
            goMission = null;
            if(null != comListScript)
            {
                comListScript.onBindItem = null;
                comListScript.onItemVisiable = null;
                comListScript.onItemSelected = null;
                comListScript = null;
            }

            _UnRegisterEvent();
        }

        void _SetToggle(FunctionType eFunctionType)
        {
           // if(m_eFunctionType != eFunctionType)
            {
                m_eFunctionType = eFunctionType;
                switch (eFunctionType)
                {
                    case FunctionType.FT_MISSION:
                        {
                            var toggle = toggles[(int)FunctionType.FT_MISSION];
                            if(null != toggle)
                            {
                                toggle.onValueChanged.RemoveListener(_OnMission);
                                toggle.isOn = true;
                                toggle.onValueChanged.AddListener(_OnMission);
                            }
                            _OnMission(true);
                        }
                        break;
                    case FunctionType.FT_TEAM:
                        {
                            var toggle = toggles[(int)FunctionType.FT_TEAM];
                            if (null != toggle)
                            {
                                toggle.onValueChanged.RemoveListener(_OnTeam);
                                toggle.isOn = true;
                                toggle.onValueChanged.AddListener(_OnTeam);
                            }
                            _OnTeam(true);
                        }
                        break;
                }
            }
        }

        void _OnMission(bool bValue)
        {
            if(bValue)
            {
                ClientSystemManager.GetInstance().CloseFrame<TeamMainMenuFrame>();
                ClientSystemManager.GetInstance().CloseFrame<TeamMainFrame>();

                TeamUtility.OnMissionTraceSelectTeam(false);

                //UpdateMemberCountGrayState(FunctionType.FT_MISSION);

                if (m_eFunctionType == FunctionType.FT_MISSION)
                {
                    if(!m_bFromEvent)
                    {
                        MissionFrameNewData data = new MissionFrameNewData();
                        data.iFirstFilter = (int)MissionFrameNew.FilterFirstType.FFT_MAIN_OR_BRANCH;

                        ClientSystemManager.GetInstance().OpenFrame<MissionFrameNew>(FrameLayer.Middle, data);
                        GameStatisticManager.GetInstance().DoStartUIButton("Mission");
                    }
                }
                else
                {
                    m_eFunctionType = FunctionType.FT_MISSION;
                }

                if (null == comListScript)
                {
                    goMission = AssetLoader.instance.LoadRes(m_mission_prefab_path, typeof(GameObject)).obj as GameObject;
                    if (null != goMission)
                    {
                        Utility.AttachTo(goMission, goFunctionRoot);
                        comListScript = Utility.FindComponent<ComUIListScript>(goMission, "ScrollView");
                    }

                    if (null != comListScript)
                    {
                        comListScript.Initialize();
                        comListScript.onBindItem = (GameObject go) =>
                        {
                            if(null != go)
                            {
                                return go.GetComponent<ComFunctionItem>();
                            }
                            return null;
                        };
                        comListScript.onItemVisiable = (ComUIListElementScript item)=>
                        {
                            var items = MissionManager.GetInstance().TraceList;
                            if(null != item && item.m_index >= 0 && item.m_index < MissionManager.GetInstance().ListCnt)
                            {
                                var script = item.gameObjectBindScript as ComFunctionItem;
                                if(null != script)
                                {
                                    script.OnItemVisible(items[item.m_index]);
                                }
                            }
                        };
                    }
                }
            }

            if(null != goMission)
            {
                goMission.CustomActive(bValue);
            }
        }

        bool m_bDirty = false;
        void _OnConfirmToUpdate()
        {
            if (null != comListScript)
            {
                MissionManager.GetInstance().GetTraceTaskList();
                comListScript.SetElementAmount(MissionManager.GetInstance().ListCnt);
            }
            m_bDirty = false;
        }

        void _UpdateMission()
        {
            if(!m_bDirty)
            {
                m_bDirty = true;
                InvokeMethod.Invoke(this,0.33f, _OnConfirmToUpdate);
            }
        }

        void UpdateMemberCountGrayState(FunctionType functionType)
        {
            if(memberCountShow == null)
            {
                return;
            }

            bool bGray = functionType != FunctionType.FT_TEAM;

            var gray = memberCountShow.gameObject.SafeAddComponent<UIGray>(false);
            if(gray != null)
            {
                gray.SetEnable(false);
                gray.SetEnable(bGray);
            }
        }

        void _OnTeam(bool bValue)
        {
            if(!bValue)
            {
                return;
            }

            //UpdateMemberCountGrayState(FunctionType.FT_TEAM);

            if (!Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Team))
            {
                var functionUnLockTable = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Team);
                if (functionUnLockTable != null)
                {
                    SystemNotifyManager.SystemNotify(functionUnLockTable.CommDescID);
                }
               
                m_bFromEvent = true;
                _SetToggle(FunctionType.FT_MISSION);
                m_bFromEvent = false;
                return;
            }

            TeamUtility.OnMissionTraceSelectTeam(true);

            if (m_eFunctionType == FunctionType.FT_TEAM)
            {
                if (!m_bFromEvent && !NewbieGuideManager.GetInstance().IsGuidingTask((ProtoTable.NewbieGuideTable.eNewbieGuideTask.TeamGuide)))
                {
                    TeamListFrame.TryOpenTeamListOrTeamMyFrame();
                    //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle);
                    GameStatisticManager.GetInstance().DoStartUIButton("Team");
                }
            }
            else
            {
                m_eFunctionType = FunctionType.FT_TEAM;
            }
        }

        void OnDestroy()
        {
            InvokeMethod.RemoveInvokeCall(this);
        }
    }
}
