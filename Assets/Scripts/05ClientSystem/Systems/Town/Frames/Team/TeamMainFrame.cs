using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class TeamMainFrame : ClientFrame
    {
        const int MemberNum = 3;

        List<TeamMember> TeamDatas = new List<TeamMember>();

        #region val
        const int maxListCount = 10;
        object requestListObj = null;   
        const float fRequestInterval = 5.0f;
        #endregion
        #region ui bind
        Toggle btGuildDungeonTeamList = null;
        Toggle btTeamList = null;
        GameObject myTeamPlayers = null;
        GameObject guildDungeonTeams = null;    
        private GameObject teamListParent = null;
        private GameObject teamItemTemplate = null;
        #endregion
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamMain";
        }

        protected override void _OnOpenFrame()
        {
            InitInterface();
            BindUIEvent();
            requestListObj = new object();
        }

        protected override void _OnCloseFrame()
        {
            Clear();
            InvokeMethod.RmoveInvokeIntervalCall(requestListObj);
            requestListObj = null;
        }

        void Clear()
        {
            TeamDatas.Clear();
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            btGuildDungeonTeamList = mBind.GetCom<Toggle>("btGuildDungeonTeamList");
            btGuildDungeonTeamList.SafeAddOnValueChangedListener((bool value) => 
            {
                if(value)
                {
                    myTeamPlayers.CustomActive(false);
                    guildDungeonTeams.CustomActive(true);
                    _RequestGuildDungeonTeamList();
                    InvokeMethod.RmoveInvokeIntervalCall(requestListObj);
                    InvokeMethod.InvokeInterval(requestListObj, 0.0f, fRequestInterval, float.MaxValue, null, _RequestGuildDungeonTeamList, null);
                }               
            });
            btTeamList = mBind.GetCom<Toggle>("btTeamList");
            btTeamList.SafeAddOnValueChangedListener((bool value) => 
            {
                if(value)
                {
                    _OnClickTeamList();
                }
            });
            myTeamPlayers = mBind.GetGameObject("myTeamPlayers");
            guildDungeonTeams = mBind.GetGameObject("guildDungeonTeams");
            teamListParent = mBind.GetGameObject("teamListParent");
            teamItemTemplate = mBind.GetGameObject("teamItemTemplate");
        }
        protected override void _unbindExUI()
        {
            btGuildDungeonTeamList = null;
            btTeamList = null;
            myTeamPlayers = null;
            guildDungeonTeams = null;
            teamListParent = null;
            teamItemTemplate = null;
        }
        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnAddMemberSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamChangeLeaderSuccess, OnChangeLeaderSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMemberStateChanged, OnChangeLeaderSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SetNoteNameSuccess, OnSetNoteNameSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamListRequestSuccessForTeamMainUI, OnTeamListRequestSuccess);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnAddMemberSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamChangeLeaderSuccess, OnChangeLeaderSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMemberStateChanged, OnChangeLeaderSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SetNoteNameSuccess, OnSetNoteNameSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamListRequestSuccessForTeamMainUI, OnTeamListRequestSuccess);
        }
        public override bool IsNeedUpdate()
        {
            return true;
        }
        protected override void _OnUpdate(float timeElapsed)
        {
            btTeamList.CustomActive(ClientSystemManager.GetInstance().IsFrameOpen<GuildArenaFrame>());
            btGuildDungeonTeamList.CustomActive(ClientSystemManager.GetInstance().IsFrameOpen<GuildArenaFrame>());
        }

        void _RequestGuildDungeonTeamList()
        {
            TeamDataManager.GetInstance().RequestTeamListForTeamMainUI(0, TeamDataManager.GetInstance().nTeamGuildDungeonID);
        }
        void _UpdateGuildDungeonTeamList()
        {
            if (guildDungeonTeams == null)
            {
                return;
            }
            if (!guildDungeonTeams.activeInHierarchy)
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
                    if (info.currentMemberCount >= info.maxMemberCount)
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
                        StaticUtility.SafeSetText(bind, "count", string.Format("{0}/{1}", info.currentMemberCount, info.maxMemberCount));
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
        [UIEventHandle("myTeamPlayers/btQuit")]
        void OnQuit()
        {
            TeamDataManager.GetInstance().NotPopUpTeamList = true;
            TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);
        }

        [UIEventHandle("myTeamPlayers/Player{0}/Icon", typeof(Button), typeof(UnityEngine.Events.UnityAction<int>), 1, MemberNum)]
        void OnClickMember(int iIndex)
        {
            if (iIndex < 0)
            {
                return;
            }

            if(TeamDatas == null || TeamDatas.Count <= 0 || iIndex >= TeamDatas.Count)
            {
                return;
            }

            if(TeamDatas[iIndex].id == PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            TeamMenuData menuData = new TeamMenuData();

            menuData.index = (byte)iIndex;
            menuData.memberID = TeamDatas[iIndex].id;
            menuData.name = TeamDatas[iIndex].name;
            menuData.occu = TeamDatas[iIndex].occu;
            menuData.level = TeamDatas[iIndex].level;

            menuData.Pos = new Vector3();

            if (frameMgr.IsFrameOpen<TeamMemberMenuFrame>())
            {
                frameMgr.CloseFrame<TeamMemberMenuFrame>();
            }

            frameMgr.OpenFrame<TeamMemberMenuFrame>(FrameLayer.Middle, menuData);
        }

        [UIEventHandle("myTeamPlayers/Player{0}/btOpenTeamMy", typeof(Button), typeof(UnityEngine.Events.UnityAction<int>), 1, MemberNum)]
        void OnOpenTeamMy(int iIndex)
        {
            TeamListFrame.TryOpenTeamListOrTeamMyFrame();
            //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>();
        }
        
        void _OnClickTeamList()
        {
            TeamListFrame.TryOpenTeamListOrTeamMyFrame();
            //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>();
            myTeamPlayers.CustomActive(true);
            guildDungeonTeams.CustomActive(false);
            InvokeMethod.RmoveInvokeIntervalCall(requestListObj);
        }

        void InitInterface()
        {
            UpdateInterface();
        }

        void UpdateInterface()
        {
            Team teamData = TeamDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }

            TeamDatas.Clear();

            int iIndex = 0;
            for(int i = 0; i < teamData.members.Length; i++)
            {
                if(teamData.members[i].id <= 0)
                {
                    continue;
                }

                TeamMember memeberdata = new TeamMember();
                memeberdata = teamData.members[i];

                TeamDatas.Add(memeberdata);

                JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(teamData.members[i].occu);
                if (jobData != null)
                {
                    ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                    if (resData != null)
                    {
                        //Sprite spr = AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
                        //if (spr != null)
                        //{
                        //    Icons[iIndex].sprite = spr;
                        //}
                        ETCImageLoader.LoadSprite(ref Icons[iIndex], resData.IconPath);
                    }
                }

                MemberNames[iIndex].text = teamData.members[i].name;
                RelationData relationData = null;
                RelationDataManager.GetInstance().FindPlayerIsRelation(teamData.members[i].id, ref relationData);
                if (relationData != null)
                {
                    if (relationData.remark != null && relationData.remark != "")
                    {
                        MemberNames[iIndex].text = relationData.remark;
                    }
                }
               
                Levels[iIndex].text = string.Format("Lv.{0}", teamData.members[i].level);

                //HPTexts[i].text = PlayerBaseData.GetInstance().MaxHP.ToString();

                if (teamData.leaderInfo.id == teamData.members[i].id)
                {
                    LeaderMarks[iIndex].gameObject.SetActive(true);
                }
                else
                {
                    LeaderMarks[iIndex].gameObject.SetActive(false);
                }

                Players[iIndex].gameObject.SetActive(true);

                iIndex++;
            }

            for(int i = iIndex; i < MemberNum; i++)
            {
                Players[i].gameObject.SetActive(false);
            }
        }

        void OnAddMemberSuccess(UIEvent iEvent)
        {
            UpdateInterface();
        }

        void OnChangeLeaderSuccess(UIEvent iEvent)
        {
            UpdateInterface();
        }

        void OnSetNoteNameSuccess(UIEvent iEvent)
        {
            UpdateInterface();
        }

        void OnTeamListRequestSuccess(UIEvent uiEvent)
        {
            _UpdateGuildDungeonTeamList();
        }
        [UIControl("myTeamPlayers/Player{0}", typeof(Image), 1)]
        protected Image[] Players = new Image[MemberNum];

        [UIControl("myTeamPlayers/Player{0}/Icon", typeof(Image), 1)]
        protected Image[] Icons = new Image[MemberNum];

        [UIControl("myTeamPlayers/Player{0}/Name", typeof(Text), 1)]
        protected Text[] MemberNames = new Text[MemberNum];

        [UIControl("myTeamPlayers/Player{0}/Level", typeof(Text), 1)]
        protected Text[] Levels = new Text[MemberNum];

        [UIControl("myTeamPlayers/Player{0}/LeaderMark", typeof(Image), 1)]
        protected Image[] LeaderMarks = new Image[MemberNum];

        [UIControl("myTeamPlayers/Player{0}/HP/Text", typeof(Text), 1)]
        protected Text[] HPTexts = new Text[MemberNum];

        [UIControl("myTeamPlayers/Player{0}/MP/Text", typeof(Text), 1)]
        protected Text[] MPTexts = new Text[MemberNum];

        [UIControl("myTeamPlayers/btQuit")]
        protected Button btQuit;
    }
}
