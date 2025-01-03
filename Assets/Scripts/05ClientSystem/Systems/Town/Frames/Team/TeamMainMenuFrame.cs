using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System.Collections.Generic;

namespace GameClient
{
    class TeamMainMenuData
    {
        public Vector2 uiPos;
    }

    class TeamMainMenuFrame : ClientFrame
    {
        #region val
        const int maxListCount = 10;
        object requestListObj = null;
        bool bRequestGuildDungeonTeamList = false;
        const float fRequestInterval = 5.0f;
        #endregion
        #region ui bind
        private GameObject guildDungeonTeams = null;
        private GameObject teamListParent = null;
        private GameObject teamItemTemplate = null;
        private Text joinTips = null;
        #endregion
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamMainMenu";
        }

        protected override void _OnOpenFrame()
        {
            bRequestGuildDungeonTeamList = false;
            requestListObj = new object();
            _Initialize();
            _BindUIEvent();
            _RequestGuildDungeonTeamList();
        }

        protected override void _OnCloseFrame()
        {
            bRequestGuildDungeonTeamList = false;
            InvokeMethod.RmoveInvokeIntervalCall(requestListObj);
            requestListObj = null;
            _Clear();

//             if (!ClientSystemManager.instance.IsFrameOpen<FunctionFrame>())
//             {
//                 ClientSystemManager.instance.OpenFrame<FunctionFrame>(FrameLayer.Bottom);
//             }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }
        protected override void _bindExUI()
        {
            guildDungeonTeams = mBind.GetGameObject("guildDungeonTeams");
            teamListParent = mBind.GetGameObject("teamListParent");
            teamItemTemplate = mBind.GetGameObject("teamItemTemplate");
            joinTips = mBind.GetCom<Text>("joinTips");
        }
        protected override void _unbindExUI()
        {
            guildDungeonTeams = null;
            teamListParent = null;
            teamItemTemplate = null;
            joinTips = null;
        }
        protected override void _OnUpdate(float timeElapsed)
        {
            m_btTeamList.CustomActive(ClientSystemManager.GetInstance().IsFrameOpen<GuildArenaFrame>());
            if(GuildDataManager.IsGuildDungeonMapScence() || GuildDataManager.IsInGuildAreanScence())
            {
                guildDungeonTeams.CustomActive(true);
                joinTips.CustomActive(false);
                if (!bRequestGuildDungeonTeamList)
                {
                    bRequestGuildDungeonTeamList = true;
                    InvokeMethod.RmoveInvokeIntervalCall(requestListObj);
                    InvokeMethod.InvokeInterval(requestListObj, 0.0f, fRequestInterval, float.MaxValue, null, _RequestGuildDungeonTeamList, null);
                }
            }
            else
            {
                guildDungeonTeams.CustomActive(false);
                joinTips.CustomActive(true);
                bRequestGuildDungeonTeamList = false;
                InvokeMethod.RmoveInvokeIntervalCall(requestListObj);
            }
        }
        void _RequestGuildDungeonTeamList()
        {
            TeamDataManager.GetInstance().RequestTeamListForTeamMainUI(0,TeamDataManager.GetInstance().nTeamGuildDungeonID);
        }
        void _Clear()
        {
            _UnBindUIEvent();
        }

        void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamCreateSuccess, _OnCreateTeamSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, _OnQuitSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamListRequestSuccessForTeamMainUI, OnTeamListRequestSuccess);
        }

        void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamCreateSuccess, _OnCreateTeamSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, _OnQuitSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamListRequestSuccessForTeamMainUI, OnTeamListRequestSuccess);
        }

        void _OnCreateTeamSuccess(UIEvent uiEvent)
        {
            TeamListFrame.TryOpenTeamListOrTeamMyFrame();
            //ClientSystemManager.instance.OpenFrame<TeamListFrame>();
            //frameMgr.CloseFrame(this);
        }

        void _OnQuitSuccess(UIEvent uiEvent)
        {
            _SetupFuncBtns();
        }
        void OnTeamListRequestSuccess(UIEvent uiEvent)
        {
            _UpdateGuildDungeonTeamList();
        }

        [UIEventHandle("Content/funcs/CreateTeam/Button")]
        void _OnCreateTeamClicked()
        {
            //frameMgr.OpenFrame<TeamCreateFrame>(FrameLayer.Middle);

            FunctionUnLock data = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Team);
            if(data == null)
            {
                return;
            }

            if (PlayerBaseData.GetInstance().Level < data.FinishLevel)
            {
                SystemNotifyManager.SystemNotify(1300031);
                return;
            }

            if(GuildDataManager.IsInGuildAreanScence())
            {
                TeamListFrame.TryOpenTeamListOrTeamMyFrame();
                //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle);
                return;
            }
            TeamCreateInfo createInfo = TeamDataManager.GetInstance().CreateInfo;
            createInfo.Debug();
            createInfo.Reset();

            TeamDataManager.GetInstance().CreateTeam(TeamUtility.kDefaultTeamDungeonID);
            //frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Content/funcs/MyTeam/Button")]
        void _OnMyTeamClicked()
        {
            TeamListFrame.TryOpenTeamListOrTeamMyFrame();
            //ClientSystemManager.instance.OpenFrame<TeamListFrame>();
            //frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Content/funcs/TeamList/Button")]
        void _OnTeamListClicked()
        {
            ActiveManager.OnTeamListClicked(string.Empty);
        }
        [UIEventHandle("btTeamList")]
        void _OnClickTeamList()
        {
            ActiveManager.OnTeamListClicked(string.Empty);
        }

        void _Initialize()
        {
            //             TeamMainMenuData uiData = userData as TeamMainMenuData;
            //             if (uiData != null)
            //             {
            _SetupFuncBtns();
            //                 _SetupFramePosition(uiData.uiPos);
            //             }
            //             else
            //             {
            //                 Logger.LogError("TeamMainMenuData is null!!");
            //             }
        }

        void _SetupFuncBtns()
        {
            bool hasMyTeam = TeamDataManager.GetInstance().HasTeam();
            Utility.FindGameObject(m_content, "funcs/CreateTeam").SetActive(hasMyTeam == false);
            Utility.FindGameObject(m_content, "funcs/MyTeam").SetActive(hasMyTeam);
        }

        void _SetupFramePosition(Vector2 pos)
        {
            RectTransform rectParent = frame.GetComponent<RectTransform>();
            RectTransform rectContent = m_content.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectContent);
            float xMin = 0.0f;
            float xMax = rectParent.rect.size.x - rectContent.rect.size.x;
            float yMin = rectContent.rect.size.y - rectParent.rect.size.y;
            float yMax = 0.0f;

            Vector2 localPos;
            bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, pos, ClientSystemManager.GetInstance().UICamera, out localPos);
            if (success)
            {
                if (localPos.x < xMin)
                {
                    localPos.x = xMin;
                }
                else if (localPos.x > xMax)
                {
                    localPos.x = localPos.x - rectContent.rect.size.x;
                }

                if (localPos.y < yMin)
                {
                    localPos.y = localPos.y + rectContent.rect.size.y;
                }
                else if (localPos.y > yMax)
                {
                    localPos.y = yMax;
                }

                rectContent.anchoredPosition = localPos;
            }
        }

        void _UpdateGuildDungeonTeamList()
        {
            if(guildDungeonTeams == null)
            {
                return;
            }
            if(!guildDungeonTeams.activeInHierarchy)
            {
                return;
            }
            if (teamListParent != null && teamItemTemplate != null)
            {
                List<Team> teamDatas = TeamDataManager.GetInstance().GetTeamListForTeamMainUI();
                int iNum = 0;
                for (int i = 0; i < teamListParent.transform.childCount; ++i)
                {
                    GameObject go = teamListParent.transform.GetChild(i).gameObject;
                    GameObject.Destroy(go);
                }
                for (int i = 0; i < teamDatas.Count && iNum < maxListCount; i++)
                {
                    Team info = teamDatas[i];
                    if (info == null)
                    {
                        continue;
                    }
                    if(info.currentMemberCount >= info.maxMemberCount)
                    {
                        continue;
                    }
                    GameObject goCurrent = GameObject.Instantiate(teamItemTemplate.gameObject);
                    Utility.AttachTo(goCurrent, teamListParent);
                    goCurrent.CustomActive(true);
                    ComCommonBind bind = goCurrent.GetComponent<ComCommonBind>();
                    if (bind != null)
                    {
                        StaticUtility.SafeSetText(bind, "leader", info.leaderInfo.name);                        
                        StaticUtility.SafeSetText(bind, "count", string.Format("{0}/{1}",info.currentMemberCount,info.maxMemberCount));
                        StaticUtility.SafeSetText(bind, "target", TeamDataManager.GetTeamDungeonName(info.teamDungeonID));
                        ulong iTeamID = teamDatas[i].leaderInfo.id;
                        StaticUtility.SafeSetBtnCallBack(bind, "join", () => 
                        {
                            OnClickJoinTeam(iTeamID);
                        });
                    }
                    iNum++;
                }               
            }          
        }
        void OnClickJoinTeam(ulong iTeamID)
        {
            int iIndex = -1;
            List<Team> teamDatas = TeamDataManager.GetInstance().GetTeamListForTeamMainUI();
            for (int i = 0; i < teamDatas.Count; i++)
            {
                if (iTeamID == teamDatas[i].leaderInfo.id)
                {
                    iIndex = i;
                    break;
                }
            }
            if (iIndex < 0 || teamDatas == null || iIndex >= teamDatas.Count)
            {
                return;
            }
            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_already_has_team"));
                return;
            }
            int iNotifyID = -1;
            Team data = teamDatas[iIndex];
            if (!Utility.CheckTeamCondition((int)data.teamDungeonID, ref iNotifyID))
            {
                if (iNotifyID != -1)
                {
                    SystemNotifyManager.SystemNotify(iNotifyID);
                }
                return;
            }
            if (teamDatas[iIndex].currentMemberCount >= teamDatas[iIndex].maxMemberCount)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("队伍人数已满");
                return;
            }
            TeamDataManager.GetInstance().JoinTeam(teamDatas[iIndex].leaderInfo.id);
        }
        [UIControl("Content/funcs/CreateTeam/Button")]
        Button m_funCreateTeam;

        [UIControl("Content/funcs/MyTeam/Button")]
        Button m_funMyTeam;

        [UIControl("Content/funcs/TeamList/Button")]
        Button m_funTeamList;

        [UIObject("Content")]
        GameObject m_content;

        [UIObject("btTeamList")]
        GameObject m_btTeamList;
        //         [UIEventHandle("BG")]
        //         void _OnCloseClicked()
        //         {
        //             frameMgr.CloseFrame(this);
        //         }
    }
}
