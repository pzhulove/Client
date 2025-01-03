using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameClient
{
    class TeamMyFrame : ClientFrame
    {
        int[] ModelLayers = { 15, 16, 17 };

        private bool mBisFlag = false; //判断疲劳燃烧是否显示
        private ActivityLimitTime.ActivityLimitTimeData data = null;  //疲劳燃烧活动数据
        private ActivityLimitTime.ActivityLimitTimeDetailData mData = null; //疲劳燃烧活动开启的子任务数据
        private bool mFatigueCombustionTimeIsOpen = false;//是否正在倒计时
        private Text mTime; //疲劳燃烧时间Text文本
        private int mFatigueCombustionTime = -1;//用于疲劳燃烧结束时间戳

        private const int returnPlayerBufCount = 3;
        public class MemberUI
        {
            public ulong         memberID;
            public ComCommonBind bind;

            // TODO 到时候把这些变量都去了
            public GameObject         contentRoot;
            public Text               Level;
            public Text               Job;
            public Image              leaderMark;
            public Text               memberName;
            public GameObject resistMagicRoot;
            public Text resistMagicValue;
            public Button             AddMemberMark;
            public Button             showMenu;
            public GameObject         modelRoot;
            public GeAvatarRendererEx avatarRenderer = null;
            public GameObject mFatigueCombustionRoot;
            public Button rewardPreview;
            public Button searchBtn;
            public GameObject countDownRoot;
            public Image tenImg;
            public Image bitsImg;

            public void SetBuzy(bool isBuzy)
            {
                if (null == bind) 
                {
                    return;
                }

                GameObject buzystatus = bind.GetGameObject("buzystatus");
                GameObject normalstatus = bind.GetGameObject("normalstatus");

                buzystatus.SetActive(isBuzy);
                normalstatus.SetActive(!isBuzy);
            }


            public void SetFlag(eTeammateFlag relation)
            {
                if (null == bind) 
                {
                    return;
                }

                GameObject friendstatus = bind.GetGameObject("friendstatus");
                GameObject guildstatus = bind.GetGameObject("guildstatus");
                GameObject helpfightstatus = bind.GetGameObject("helpfightstatus");
                GameObject masterstatus = bind.GetGameObject("masterstatus");
                GameObject disciplestatus = bind.GetGameObject("disciplestatus");

                friendstatus.SetActive(false);
                guildstatus.SetActive(false);
                helpfightstatus.SetActive(false);
                masterstatus.SetActive(false);
                disciplestatus.SetActive(false);

                if (((int)(relation & eTeammateFlag.Friend)) != 0)
                {
                    friendstatus.SetActive(true);
                }

                if (((int)(relation & eTeammateFlag.Guild)) != 0)
                {
                    guildstatus.SetActive(true);
                }

                if (((int)(relation & eTeammateFlag.HelpFight)) != 0)
                {
                    helpfightstatus.SetActive(true);
                }

                if (((int)(relation & eTeammateFlag.Master)) != 0)
                {
                    masterstatus.SetActive(true);
                }

                if (((int)(relation & eTeammateFlag.Disciple)) != 0)
                {
                    disciplestatus.SetActive(true);
                }
            }

            public const string ICON_VIP_RES_PATH_FORMAT = "UI/Image/Packed/p_UI_Vip.png:UI_Chongzhi_Zi_Guizu_0{0}";
            const string VIP_DES_ELEMENT_PATH = "UIFlatten/Prefabs/Vip/VipElement";
            /// <summary>
            /// 设置Vip等级
            ///
            /// 若level 小于等于 0, 则隐藏
            ///
            /// </summary>
            public void SetVipLevel(int level,ulong playerID)
            {
                if (null == bind)
                {
                    return;
                }

                GameObject vipRoot = bind.GetGameObject("vipRoot");

                ComPlayerVipLevelShow comPlayerVipLevelShow = bind.GetCom<ComPlayerVipLevelShow>("comPlayerVipLevelShow");
                if(comPlayerVipLevelShow != null)
                {
                    ShowMode showMode = (playerID == PlayerBaseData.GetInstance().RoleID ? ShowMode.MySelf : ShowMode.Other);
                    comPlayerVipLevelShow.SetUp(showMode, playerID == PlayerBaseData.GetInstance().RoleID);
                    comPlayerVipLevelShow.SetVipLevel(level);                    
                }

                vipRoot.SetActive(level > 0); 
            }
        }

        const int TeamMemberNum = 3;

        bool bStartMatch = false;
        float fAddUpTime = 0.0f;

        MemberUI[] MemberInfoUIs = new MemberUI[TeamMemberNum];
        private List<int> FliterFirstMenuList = new List<int>();
        private Dictionary<int, List<int>> FliterSecondMenuDict = new Dictionary<int, List<int>>();

        bool bToggleInit = false; // 用来解决toggle在界面打开的时候会调用一次回调的问题
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamMy";
        }

        #region channel_dropdown_list

        void _InitDropDown()
        {
            if(ChannelSelect != null)
            {
                List<ComControlData> comControlDatas = new List<ComControlData>();
                if(comControlDatas != null)
                {
                    ChatType[] eChatList = new ChatType[] { ChatType.CT_ACOMMPANY, ChatType.CT_GUILD, ChatType.CT_NORMAL };
                    string[] descs = new string[] { "组队频道", "公会频道", "附近频道" };
                    if (GuildDataManager.IsGuildTeamDungeonID((int)TeamDataManager.GetInstance().TeamDungeonID))
                    {
                        eChatList = new ChatType[] { ChatType.CT_GUILD };
                        descs = new string[] { "公会频道" };
                    }

                    for(int i = 0;i < eChatList.Length && i < descs.Length;i++)
                    {
                        ComControlData comControlData = new ComControlData()
                        {
                            Index = i,
                            Id = (int)eChatList[i],
                            Name = descs[i],
                        };

                        if(comControlData != null)
                        {
                            comControlDatas.Add(comControlData);
                        }
                    }

                    ChannelSelect.InitComDropDownControl(comControlDatas.Count > 0 ? new ComControlData()
                    {
                        Index = 0,
                        Id = (int)eChatList[0],
                        Name = TR.Value("chat_team_leader_talk"),
                    } : null, comControlDatas, (data) => 
                    {
                        if(data != null)
                        {
                            _OnDropDownValueChanged((ChatType)data.Id);
                        }
                        
                    }, null);
                }
            }
        }

        void _UnInitDropDown()
        {
           
        }

        void _OnDropDownValueChanged(ChatType eChatType)
        {
            //Logger.LogErrorFormat("{0}", eChatType);
            string content = _GetLeaderInviteWords(true);
            if(string.IsNullOrEmpty(content))
            {
                return;
            }

            switch (eChatType)
            {
                case ChatType.CT_ACOMMPANY:
                    {
                        var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TEAM_CHAT_LV_LIMIT);
                        if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_team_talk_need_level"), systemValue.Value), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        if (!ChatManager.accompany_chat_try_enter_cool_down())
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_accompany_talk_need_interval"), ChatManager.accompany_cool_time), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        ChatManager.GetInstance().SendChat(ChatType.CT_ACOMMPANY, content);
                        SystemNotifyManager.SysNotifyTextAnimation("消息发送成功!", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                    }
                    break;
                case ChatType.CT_WORLD:
                    {
                        var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WORLD_CHAT_LV_LIMIT);
                        if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_world_talk_need_level"), systemValue.Value), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        int iFreeTimes = ChatManager.GetInstance().FreeWorldChatLeftTimes;
                        if (iFreeTimes <= 0)
                        {
                            if (!ChatManager.GetInstance().CheckWorldActivityValueEnough())
                            {
                                SystemNotifyManager.SystemNotify(7006, () =>
                                {
                                    ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.VIP);
                                });
                                break;
                            }
                        }
                        if (!ChatManager.world_chat_try_enter_cool_down())
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_world_talk_need_interval"), ChatManager.world_cool_time), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        SystemNotifyManager.SysNotifyTextAnimation("消息发送成功!", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, content);
                    }
                    break;
                case ChatType.CT_GUILD:
                    {
                        if(PlayerBaseData.GetInstance().eGuildDuty == EGuildDuty.Invalid)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("chat_guild_talk_need_guild"), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        SystemNotifyManager.SysNotifyTextAnimation("消息发送成功!",CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                        ChatManager.GetInstance().SendChat(ChatType.CT_GUILD, content);
                    }
                    break;
                case ChatType.CT_NORMAL:
                    {
                        var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_AROUND_CHAT_LV_LIMIT);
                        if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_normal_talk_need_level"), systemValue.Value), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        if (!ChatManager.arround_chat_try_enter_cool_down())
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_normal_talk_need_interval"), ChatManager.arround_cool_time), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        SystemNotifyManager.SysNotifyTextAnimation("消息发送成功!", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                        ChatManager.GetInstance().SendChat(ChatType.CT_NORMAL, content);
                    }
                    break;
            }
        }
        #endregion

        protected override void _OnOpenFrame()
        {
            bToggleInit = true;
            returnPlayerBufs = null;
            FliterSecondMenuDict.Clear();
            FliterFirstMenuList = Utility.GetTeamDungeonMenuFliterList(ref FliterSecondMenuDict);
            InitInterface();
            mIsSendMessage = false;
           
#if DEBUG_SETTING
            if (Global.Settings.IsTestTeam())
			{
				//_onOnEnterDungeonButtonClick
				BeUtility.SendGM("!!additem id=200000002 num=1000");
				BeUtility.SendGM("!!additem id=600000006 num=1000");
				BeUtility.SendGM("!!addfatigue num=150");

				Global.Settings.forceUseAutoFight = true;

				if (TeamDataManager.GetInstance().IsTeamLeader() && TeamDataManager.GetInstance().GetMemberNum()>=3)
				{
					ClientSystemManager.GetInstance().delayCaller.DelayCall(5000, ()=>{
						_onOnEnterDungeonButtonClick();
					});
				}
			}
			#endif           
        }

        protected override void _OnCloseFrame()
        {
            bToggleInit = false;
            returnPlayerBufs = null;
            Clear();
        }

        void Clear()
        {
            //TeamDataManager.bIsRefreshTime = false;
            bStartMatch = false;
            fAddUpTime = 0.0f;
            mIsSendMessage = false;

            for (int i = 0; i < MemberInfoUIs.Length; i++)
            {
                MemberUI mem = MemberInfoUIs[i];

                if (mem != null)
                {
                    if(mem.AddMemberMark != null)
                    {
                        mem.AddMemberMark.onClick.RemoveAllListeners();
                    }

                    if(mem.showMenu != null)
                    {
                        mem.showMenu.onClick.RemoveAllListeners();
                    }
                }
            }

            MemberInfoUIs = new MemberUI[TeamMemberNum];

            UnBindUIEvent();
            _UnInitDropDown();

            _UnInitVoiceChatModule();
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnAddMemberSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnRemoveMemberSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamChangeLeaderSuccess, OnChangeLeaderSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, OnTeamInfoChangeSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamPosStateChanged, OnTeamPosStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMemberStateChanged, OnMemberStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshFriendList, OnMemberStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMatchStartSuccess, OnTeamMatchStartSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMatchCancelSuccess, OnTeamMatchCancelSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamNotifyChatMsg, _updateTeamChatMsg);
            if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
            {
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.AddSyncTaskDataChangeListener(_OnTaskChange);
            }
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SetNoteNameSuccess, OnSetNoteNameSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamTimeChanged, OnTeamTimeChanged);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnAddMemberSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnRemoveMemberSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamChangeLeaderSuccess, OnChangeLeaderSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, OnTeamInfoChangeSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamPosStateChanged, OnTeamPosStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMemberStateChanged, OnMemberStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshFriendList, OnMemberStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMatchStartSuccess, OnTeamMatchStartSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMatchCancelSuccess, OnTeamMatchCancelSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamNotifyChatMsg, _updateTeamChatMsg);
            if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RemoveSyncTaskDataChangeListener(_OnTaskChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SetNoteNameSuccess, OnSetNoteNameSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamTimeChanged, OnTeamTimeChanged);
        }

        private void _updateTeamChatMsg(UIEvent ui)
        {
            UInt64 roleID = (UInt64)ui.Param1;
            string word = (string)ui.Param2;

            for (int i = 0; i < MemberInfoUIs.Length; ++i)
            {
                if (MemberInfoUIs[i].memberID == roleID)
                {
                    ComTeamChatMessage chatMessage = MemberInfoUIs[i].bind.GetCom<ComTeamChatMessage>("chatMessage");
                    chatMessage.SetMessage(word);
                    break;
                }
            }

        }

        void OnClickClose()
        {
            if (bStartMatch)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_dungeon_is_matching"));
                return;
            }

            frameMgr.CloseFrame(this);
        }

        void OnExitTeamBtnClicked()
        {
            TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);
        }

        string _GetLeaderInviteWords(bool bNotify)
        {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return string.Empty;
            }

            TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
            if (data == null)
            {
                return string.Empty;
            }

            if (TeamDataManager.GetInstance().IsTeamLeader())
            {
                string content = string.Format(TR.Value("LeaderInvite"), data.Name);

                content += "{T ";
                content += myTeam.leaderInfo.id.ToString();
                content += "}";

                return content;
            }
            else
            {
                if(bNotify)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("只有队长才能发送组队邀请公告!");
                }
            }

            return string.Empty;
        } 

        private void _onTeamChat()
        {
            ChatFrame cframe = ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle) as ChatFrame;
            cframe.SetTab(ChatType.CT_TEAM);
        }

        void OnAutoAgreeClicked(bool value)
        {
            if(!TeamDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            if (!value)
            {
                TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.AutoAgree, 0);
            }
            else
            {
                TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.AutoAgree, 1);
            }
        }

        void OnRequestListClicked()
        {
            if (!TeamDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            WorldTeamRequesterListReq msg = new WorldTeamRequesterListReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        void OnClickAddMemberMark(int index)
        {
            if (index < 0)
            {
                return;
            }

            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();
            if(myTeam == null)
            {
                return;
            }

            // 队长勾选了自动同意的情况下，队员可以打开邀请界面
            if(!TeamDataManager.GetInstance().IsTeamLeader() && myTeam.autoAgree != 1)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_leader_forbid_team_member_to_invite"));
                return;
            }

            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamInvitePlayerListFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<TeamInvitePlayerListFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<TeamInvitePlayerListFrame>(FrameLayer.Middle, InviteType.TeamInvite);
        }

        void OnClickshowMenu(int index)
        {
            if(index < 0)
            {
                return;
            }

            if(MemberInfoUIs[index].memberID <= 0 || MemberInfoUIs[index].memberID == PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            Team teamData = TeamDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }

            TeamMember data = null;

            for (int i = 0; i < teamData.members.Length; ++i)
            {
                if(teamData.members[i].id == MemberInfoUIs[index].memberID)
                {
                    data = teamData.members[i];
                    break;
                }
            }

            if(data == null)
            {
                return;
            }

            TeamMenuData menuData = new TeamMenuData();

            menuData.index = (byte)index;
            menuData.memberID = data.id;
            menuData.name = data.name;
            menuData.occu = data.occu;
            menuData.level = data.level;
            menuData.Pos = MemberInfoUIs[index].modelRoot.GetComponent<RectTransform>().position;

            if (frameMgr.IsFrameOpen<TeamMemberMenuFrame>())
            {
                frameMgr.CloseFrame<TeamMemberMenuFrame>();
            }

            frameMgr.OpenFrame<TeamMemberMenuFrame>(frame, menuData);
        }

        void OnMatchDugeon()
        {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            if (!TeamDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            NetManager netMgr = NetManager.Instance();

            if (!bStartMatch)
            {
                if(TeamDataManager.GetInstance().GetMemberNum() == TeamMemberNum)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_dungeon_team_number_overflow"));
                    return;
                }

                if ((int)CurTeamDungeonTableID == 1)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_dungeon_select_one_duplication"));
                    return;
                }

                TeamDungeonTable data = TableManager.GetInstance()
                    .GetTableItem<TeamDungeonTable>((int) CurTeamDungeonTableID);
                if (data == null)
                    return;


                //攻城怪物类型,炫耀传送TeamDungeonTableID 和 DungeonID
                if(data.Type == TeamDungeonTable.eType.CityMonster)
                {
                    //todo 根据协议，发送 TeamDungeonTableID 和 DungeonID
                    var msg = new SceneTeamMatchStartReq();
                    msg.dungeonId = (uint)data.DungeonID;
                    netMgr.SendCommand(ServerType.GATE_SERVER, msg);
                }
                else
                {
                    if (!Utility.CheckTeamEnterDungeonCondition((int) CurTeamDungeonTableID))
                    {
                        return;
                    }

                    SceneTeamMatchStartReq msg = new SceneTeamMatchStartReq();
                    msg.dungeonId = (uint) data.DungeonID;
                    netMgr.SendCommand(ServerType.GATE_SERVER, msg);
                }
            }
            else
            {
                WorldTeamMatchCancelReq msg = new WorldTeamMatchCancelReq();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        void _onChangeAssisStatus()
        {
            TeamDataManager.GetInstance().ChangeMainPlayerAssitState();
        }

        void OnEnterDungeon()
        {
            if (!TeamDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            if((int)CurTeamDungeonTableID == 1)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_dungeon_select_one_duplication"));
                return;
            }

            TeamDungeonTable data = TableManager.GetInstance()
                .GetTableItem<TeamDungeonTable>((int) CurTeamDungeonTableID);
            if(data == null)
                return;

            //攻城怪物类型:进入到自动寻找怪物的流程
            if (data.Type == TeamDungeonTable.eType.CityMonster)
            {
                AttackCityMonsterDataManager.GetInstance().EnterFindPathProcessByTeamDuplication();
                return;
            }
            
            if (!Utility.CheckTeamEnterDungeonCondition((int)CurTeamDungeonTableID))
            {
                return;
            }

            if (GuildDataManager.GetInstance().IsGuildDungeonMap(data.DungeonID))
            {
                if(!Utility.CheckTeamEnterGuildDungeon())
                {
                    return;
                }
                if (!GuildDataManager.GetInstance().IsGuildDungeonOpen(data.DungeonID))
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildDungeonNotOpenNow"));
                    return;
                }
            }
            //显示抗魔值不足的提示信息
            var isShowEnterDuplicationTips = IsShowEnterDuplicationTips(data.DungeonID);
            if(isShowEnterDuplicationTips == true)
                return;

            //显示配置技能的提示框
            var isShowSkillConfigTipFrame =
                SkillDataManager.GetInstance().IsShowSkillTreeFrameTipBySkillConfig(OnEnterGame);
            if (isShowSkillConfigTipFrame == true)
                return;

            if(ChallengeUtility.isYunShangChangAn(data.DungeonID))
            {
                SystemNotifyManager.SysNotifyTextAnimation("未央幻境不支持直接进入!");
                return;
            }
            GameFrameWork.instance.StartCoroutine(_teamStart((uint) data.DungeonID));
            
        }

        //显示进入地下城之前抗魔值不足的提示信息
        private bool IsShowEnterDuplicationTips(int dungeonId)
        {
            var isShowResistMagicLessTip = false;
            var resistMagicTipContent = string.Empty;

            isShowResistMagicLessTip =
                DungeonUtility.IsShowDungeonResistMagicValueTip(dungeonId, ref resistMagicTipContent);

            if (isShowResistMagicLessTip == true)
            {
                SystemNotifyManager.SysNotifyMsgBoxCancelOk(resistMagicTipContent,
                    null,
                    OnEnterGame);

                return true;
            }

            return false;
        }

        private void OnEnterGame()
        {
            TeamDungeonTable teamDungeonTable = TableManager.GetInstance()
                .GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
            if (teamDungeonTable == null)
                return;

            GameFrameWork.instance.StartCoroutine(_teamStart((uint)teamDungeonTable.DungeonID));
        }
        
		private bool mIsSendMessage = false;
        protected IEnumerator _teamStart(uint dungeonID)
        {
            if (!mIsSendMessage)
            {
                MessageEvents        msg = new MessageEvents();
                SceneDungeonStartReq req = new SceneDungeonStartReq();
                SceneDungeonStartRes res = new SceneDungeonStartRes();

                req.dungeonId = dungeonID;

                mIsSendMessage = true;

                yield return (MessageUtility.Wait<SceneDungeonStartReq, SceneDungeonStartRes>(ServerType.GATE_SERVER, msg, req, res, true, 2));

                if (res.result == 0)
                {
                    //BattleDataManager.GetInstance().originDungeonId = (int)dungeonID;
                }


                mIsSendMessage = false;
            }
            else
            {
                Logger.LogErrorFormat("[组队] 等待组队进入副本网络消息中");
            }
        }


        void InitInterface()
        {
            InitMemberInfoUI();
            UpdateTeamInfo();
            UpdateMemberInfo();
            //_InitFatigueCombustionRoot();
            _InitDropDown();       
            BindUIEvent();

            _InitVoiceChatModule();
        }

        void UpdateReturnPlayerBufItems(ComCommonBind bind, PlayerLabelInfo playerLabelInfo)
        {
            if(bind == null || playerLabelInfo == null)
            {
                return;
            }

            GameObject mContent = bind.GetGameObject("BuffContent");
            if (mContent != null)
            {
                for (int i = 0; i < mContent.GetComponentsInChildren<ComItem>().Length; i++)
                {
                    GameObject.Destroy(mContent.GetComponentsInChildren<ComItem>()[i].gameObject);
                }
            }
            
            List<int> OpActivityTaskTableIds = new List<int>()
            {
                11017000,11017001,11017002
            };

            //判断该成员是否穿戴了周年庆称号，不等于0代表穿戴了周年庆称号
            if (playerLabelInfo.returnAnniversaryTitle != 0)
            {
                OpActivityTaskTableIds.Add(11017003);
            }

            if(OpActivityTaskTableIds == null)
            {
                return;
            }

            for (int i = 0; i < OpActivityTaskTableIds.Count; i++)
            {
                OpActivityTaskTable table = TableManager.GetInstance().GetTableItem<OpActivityTaskTable>(OpActivityTaskTableIds[i]);
                if(table == null)
                {
                    continue;
                }
                string strReward = table.TaskReward;
                string[] reward = strReward.Split('_');
                if (reward.Length >= 2)
                {
                    int id = int.Parse(reward[0]);
                    int iCount = int.Parse(reward[1]);
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(id);
                    if (itemData != null)
                    {
                        itemData.Count = iCount;
                        ComItem item = bind.GetCom<ComItem>("Item");
                        ComItem comItem = GameObject.Instantiate(item);
                        if (comItem != null)
                        {
                            comItem.CustomActive(true);
                            comItem.transform.SetParent(bind.GetGameObject("BuffContent").transform,false);
                            comItem.Setup(itemData, (var1, var2) =>
                            {
                                ItemTipManager.GetInstance().CloseAll();
                                ItemTipManager.GetInstance().ShowTip(var2);
                            });
                        }
                    }
                }
            }
        }
        void InitMemberInfoUI()
        {
            string path = mBind.GetPrefabPath("memberunit");

            mBind.ClearCacheBinds(path);

            for (int i = 0; i < TeamMemberNum; ++i)
            {
                ComCommonBind bind = mBind.LoadExtraBind(path);

                if (null != bind)
                {
                    Utility.AttachTo(bind.gameObject, mMembersRoot);

                    Text level = bind.GetCom<Text>("level");
                    Text job = bind.GetCom<Text>("job");
                    Text name = bind.GetCom<Text>("name");
                    GameObject resistMagicRoot = bind.GetGameObject("ResistMagicRoot");
                    Text resistMagicValue = bind.GetCom<Text>("resistMagicValue");
                    Image leaderMark = bind.GetCom<Image>("leaderMark");
                    Button addMemberMask = bind.GetCom<Button>("addMemberMask");
                    Button showMenu = bind.GetCom<Button>("showMenu");
                    GameObject modelRoot = bind.GetGameObject("modelRoot");
                    GeAvatarRendererEx avatarRenderer = bind.GetCom<GeAvatarRendererEx>("avatarRenderer");
                    GameObject content = bind.GetGameObject("content");
                    GameObject friendstatus = bind.GetGameObject("friendstatus");
                    GameObject guildstatus = bind.GetGameObject("guildstatus");
                    GameObject helpfightstatus = bind.GetGameObject("helpfightstatus");
                    Text viplevel = bind.GetCom<Text>("viplevel");
                    GameObject vipRoot = bind.GetGameObject("vipRoot");
                    GameObject buzystatus = bind.GetGameObject("buzystatus");
                    GameObject normalstatus = bind.GetGameObject("normalstatus");
                    GameObject mFatigueCombustionRoot = bind.GetGameObject("FatigueCombustionRoot");
                    Button rewardPreview = bind.GetCom<Button>("RewardPreview");
                    Button rearchBtn = bind.GetCom<Button>("Search");
                    GameObject countDownRoot = bind.GetGameObject("CountDownRoot");
                    Image tenImg = bind.GetCom<Image>("Ten");
                    Image bitsImg = bind.GetCom<Image>("Bits");
                    MemberUI ui = new MemberUI();

                    ui.memberID       = 0;
                    ui.bind           = bind;

                    ui.contentRoot    = content;
                    ui.Level          = level;
                    ui.Job            = job;
                    ui.memberName     = name;
                    ui.resistMagicRoot = resistMagicRoot;
                    ui.resistMagicValue = resistMagicValue;
                    ui.leaderMark     = leaderMark;
                    ui.AddMemberMark  = addMemberMask;
                    ui.showMenu       = showMenu;
                    ui.modelRoot      = modelRoot;
                    ui.avatarRenderer = avatarRenderer;
                    ui.mFatigueCombustionRoot = mFatigueCombustionRoot;
                    ui.rewardPreview = rewardPreview;
                    ui.searchBtn = rearchBtn;
                    ui.countDownRoot = countDownRoot;
                    ui.tenImg = tenImg;
                    ui.bitsImg = bitsImg;

                    //StaticUtility.SafeSetBtnCallBack(ui.bind, "returnPlayer", () => 
                    //{
                    //    StaticUtility.SafeSetVisible(ui.bind, "returnPlayerBuffInfo", true);
                    //    UpdateReturnPlayerBufItems(ui.bind);

                    //    returnPlayerBufs = ui.bind.GetGameObject("returnPlayerBuffInfo");
                    //    if (returnPlayerBufs != null)
                    //    {
                    //        returnPlayerBufs.transform.SetParent(GetFrame().transform);
                    //        returnPlayerBufs.transform.SetAsLastSibling();
                    //    }

                    //    returnPlayerBufsBk.CustomActive(true);
                    //});

                    MemberInfoUIs[i] = ui;
                }
            }
        }

        void UpdateTeamInfo()
        {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
            if (data == null)
            {
                return;
            }

            mTargetDengeonName.text = data.Name;

            //更新关卡自身的抗魔值和玩家的抗魔值
            UpdateTargetDungeonInfo(data.DungeonID);
            UpdateMemberResistMagicInfoByTeamChanged();

            if (TeamDataManager.GetInstance().IsTeamLeader())
            {
                mFuncs.SetActive(true);
      
                ChannelSelect.CustomActive(true);

                //mLeaderInviteText.text = "队长喊话";

                if (myTeam.autoAgree == 1)
                {
                    mBtAutoAgree.isOn = true;
                }
                else
                {
                    mBtAutoAgree.isOn = false;
                }

                if (!bStartMatch)
                {
                    if (data.MatchType == TeamDungeonTable.eMatchType.QUICK_MATCH)
                    {
                        mMatchText.text = "快速匹配";
                    }
                    else
                    {
                        mMatchText.text = "快速组队";
                    }
                }


                mNewRequestRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.TeamNewRequester));
            }
            else
            {
                mFuncs.SetActive(false);
           
                ChannelSelect.CustomActive(false);
        
            }

            if(togAutoAgreeEnter != null)
            {
                togAutoAgreeEnter.isOn = TeamDataManager.GetInstance().IsAutoAgree;
            }
            if (goAutoAgree != null)
            {
                goAutoAgree.CustomActive(TeamDataManager.GetInstance().IsTeamLeader());
            }
            if (goAutoAgreeEnter != null)
            {
                goAutoAgreeEnter.CustomActive(!TeamDataManager.GetInstance().IsTeamLeader());
            }
            if (btSelectTarget != null)
            {
                btSelectTarget.CustomActive(TeamDataManager.GetInstance().IsTeamLeader());
            }
            UpdateBuyNum(data);
            UpdateAutoReturnFromHellUI();
            UpdateEliteNotCostFatigueUI();
        }

        private void UpdateTargetDungeonInfo(int dungeonId)
        {
            if(mTargetDungeonResistMagicRoot == null)
                return;
            if (mTargetDengeonResistMagicValue == null)
                return;

            int resistMagicValue = DungeonUtility.GetDungeonResistMagicValueById(dungeonId);
            if (resistMagicValue <= 0)
            {
                mTargetDungeonResistMagicRoot.CustomActive(false);
            }
            else
            {
                mTargetDungeonResistMagicRoot.CustomActive(true);
                mTargetDengeonResistMagicValue.text = resistMagicValue.ToString();
            }
        }

        private void _updateMemberFlagInfo()
        {
            Team teamData = TeamDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }

            for (int i = 0; i < teamData.members.Length && i < TeamMemberNum; ++i)
            {
                TeamMember data = teamData.members[i];

                if (data.id <= 0)
                {
                    continue;
                }

                MemberUI ui = null;
                
                for (int j = 0; j < MemberInfoUIs.Length; ++j)
                {
                    if (MemberInfoUIs[j].memberID == data.id)
                    {
                        ui = MemberInfoUIs[j];
                        break;
                    }
                }

                if (null != ui)
                {
                    eTeammateFlag flag = TeamDataManager.GetInstance().GetTeammateFlag(data.id);
                    Logger.LogProcessFormat("[组队] 更新标记类型 {0}", flag);
                    ui.SetFlag(flag);
                }
            }
        }

        private void UpdateMemberResistMagicInfoByTeamChanged()
        {
            Team teamData = TeamDataManager.GetInstance().GetMyTeam();
            if(teamData == null)
                return;

            var iIndex = 0;
            for (var i = 0; i < teamData.members.Length && i < TeamMemberNum; i++)
            {
                TeamMember curTeamData = teamData.members[i];

                if (curTeamData == null || curTeamData.id <= 0)
                {
                    continue;
                }

                if (iIndex >= MemberInfoUIs.Length)
                {
                    continue;
                }

                MemberUI ui = MemberInfoUIs[iIndex];

                if (ui == null)
                {
                    continue;
                }

               
                //同步的玩家抗魔值
                UpdatePlayerResistMagicValue(ui.resistMagicValue, curTeamData.resistMagicValue, ui.resistMagicRoot);

                iIndex++;
            }

        }

        void UpdateMemberInfo()
        {
            Team teamData = TeamDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }

            for (int i = 0; i < teamData.members.Length && i < MemberInfoUIs.Length; ++i)
            {
                TeamMember data = teamData.members[i];
                MemberUI ui = MemberInfoUIs[i];
                if(data != null && ui != null)
                {
                    ui.SetVipLevel(data.viplevel, data.id);
                }
            }


            int iIndex = 0;
            for (int i = 0; i < teamData.members.Length && i < TeamMemberNum; ++i)
            {
                TeamMember data = teamData.members[i];

                if (data == null || data.id <= 0)
                {
                    continue;
                }

                if (iIndex >= MemberInfoUIs.Length)
                {
                    continue;
                }

                MemberUI ui = MemberInfoUIs[iIndex];

                if (ui == null)
                {
                    continue;
                }

                if (TeamDataManager.AutomaticBackIsStart /*&& TeamDataManager.bIsRefreshTime*/)
                {
                    if (teamData.leaderInfo.id == data.id)
                    {
                        ui.countDownRoot.CustomActive(true);
                    }
                }
                else
                {
                    ui.countDownRoot.CustomActive(false);
                }

                ui.memberID = data.id;
                ui.contentRoot.SetActive(true);
                ui.Level.text = string.Format("Lv.{0}", data.level);
                ui.memberName.text = data.name;
//                 if(ui.memberID == PlayerBaseData.GetInstance().RoleID)
//                 {
//                     ui.memberName.color = new Color32(0x2a,0xe8,0xa7,0xff);                  
//                 }
//                 else
//                 {
//                     ui.memberName.color = new Color32(0xcc,0xdd,0xef,0xff);
//                 }
                RelationData relationData = null;
                RelationDataManager.GetInstance().FindPlayerIsRelation(data.id, ref relationData);
                if (relationData != null)
                {
                    if (relationData.remark != null && relationData.remark != "")
                    {
                        ui.memberName.text = relationData.remark;
                    }
                }
               
                ui.leaderMark.gameObject.SetActive(data.id == teamData.leaderInfo.id);
                StaticUtility.SafeSetVisible(ui.bind, "selfFlag", data.id == PlayerBaseData.GetInstance().RoleID);

                //同步的玩家抗魔值
                UpdatePlayerResistMagicValue(ui.resistMagicValue,data.resistMagicValue,ui.resistMagicRoot);

                ui.SetBuzy(data.isBuzy);                

                // 回归玩家标志刷新
                StaticUtility.SafeSetVisible<Button>(ui.bind, "returnPlayer", data.playerLabelInfo.returnStatus == 1);

                PlayerLabelInfo playerLabelInfo = data.playerLabelInfo;

                StaticUtility.SafeSetBtnCallBack(ui.bind, "returnPlayer", () =>
                {
                    StaticUtility.SafeSetVisible(ui.bind, "returnPlayerBuffInfo", true);
                    UpdateReturnPlayerBufItems(ui.bind, playerLabelInfo);

                    returnPlayerBufs = ui.bind.GetGameObject("returnPlayerBuffInfo");
                    if (returnPlayerBufs != null)
                    {
                        returnPlayerBufs.transform.SetParent(GetFrame().transform);
                        returnPlayerBufs.transform.SetAsLastSibling();
                    }

                    returnPlayerBufsBk.CustomActive(true);
                });

                JobTable job = TableManager.instance.GetTableItem<JobTable>(data.occu);
                if (job == null)
                {
                    Logger.LogErrorFormat("can not find JobTable with id:{0}", data.occu);
                }
                else
                {
                    ui.Job.text = job.Name;
                    ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

                    if (res == null)
                    {
                        Logger.LogErrorFormat("can not find ResTable with id:{0}", job.Mode);
                    }
                    else
                    {
                        ui.avatarRenderer.LoadAvatar(res.ModelPath);

                        _FixLightDir();

                        PlayerBaseData.GetInstance().AvatarEquipFromItems(ui.avatarRenderer,
                            data.avatarInfo.equipItemIds,
                            data.occu,
                            (int) (data.avatarInfo.weaponStrengthen),
                            null,
                            false,
                            data.avatarInfo.isShoWeapon);
						ui.avatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
                        //ui.avatarRenderer.RotateY(80);
                    }

                }

                if (data.id == PlayerBaseData.GetInstance().RoleID)
                {
                    mOnHelpFightStatus.isOn = data.isAssist;
                }

                iIndex++;
            }

            for(int i = iIndex; i < TeamMemberNum; i++)
            {
                MemberUI ui = MemberInfoUIs[i];

                ui.memberID = 0;
                ui.contentRoot.SetActive(false);
                StaticUtility.SafeSetVisible<Button>(ui.bind, "returnPlayer", false);
            }

            for(int i = 0; i < TeamMemberNum; i++)
            {
                MemberUI ui = MemberInfoUIs[i];

                int iIdx = i;
                ui.AddMemberMark.onClick.RemoveAllListeners();
                ui.AddMemberMark.onClick.AddListener(() => { OnClickAddMemberMark(iIdx); });
                ui.AddMemberMark.gameObject.SetActive(ui.memberID <= 0);
                StaticUtility.SafeSetVisible(ui.bind, "RewardPreviewRoot", ui.memberID <= 0);

                int iIdx2 = i;
                ui.showMenu.onClick.RemoveAllListeners();

                if (ui.memberID > 0 && ui.memberID != PlayerBaseData.GetInstance().RoleID)
                {
                    ui.showMenu.onClick.AddListener(() => { OnClickshowMenu(iIdx2); });
                }
                
                //注册奖励预览按钮
                if (ui.rewardPreview != null)
                {
                    ui.rewardPreview.onClick.RemoveAllListeners();
                    ui.rewardPreview.onClick.AddListener(OpenTeamRewardPreviewFrame);
                }

                if (ui.searchBtn != null)
                {
                    ui.searchBtn.onClick.RemoveAllListeners();
                    ui.searchBtn.onClick.AddListener(OpenTeamRewardPreviewFrame);
                }
            }

            //刷新倒计时
            if (TeamDataManager.AutomaticBackIsStart)
            {
                UpdateCountDownInfo(TeamDataManager.iAutoMaticBackRemainTime);
            }

            _updateMemberFlagInfo();
        }


        private void OpenTeamRewardPreviewFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<TeamRewardPreviewFrame>();
        }

        private void UpdatePlayerResistMagicValue(Text magicValueText, uint playerResistMagicValue, GameObject resistMagicRoot)
        {
            if (magicValueText == null)
                return;
            if(resistMagicRoot == null)
                return;

            resistMagicRoot.CustomActive(false);
            
            var teamDungeonData =
                TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
            if (teamDungeonData == null)
                return;

            var resistMagicValue = DungeonUtility.GetDungeonResistMagicValueById(teamDungeonData.DungeonID);
            if (resistMagicValue <= 0)
                return;

            //关卡抗魔值存在，则显示player抗魔值
            resistMagicRoot.CustomActive(true);
            if (resistMagicValue <= playerResistMagicValue)
            {
                magicValueText.text = string.Format(TR.Value("resist_magic_value_normal"), playerResistMagicValue);

            }
            else
            {
                magicValueText.text = string.Format(TR.Value("resist_magic_value_less"), playerResistMagicValue);
            }
        }


        void UpdateBuyNum(TeamDungeonTable data)
        {
            _updateMemberFlagInfo();

            // TODO 显示助战

            DungeonTable dungeonData = TableManager.GetInstance().GetTableItem<DungeonTable>(data.DungeonID);

            if(dungeonData == null)
            {
                mBtBuyNum.gameObject.SetActive(false);
                mHelpFightRoot.SetActive(false);
                return;
            }

            if(dungeonData.SubType == DungeonTable.eSubType.S_TEAM_BOSS)
            {
                mBtBuyNum.gameObject.SetActive(true);
                mHelpFightRoot.SetActive(true);
            }
            else
            {
                mBtBuyNum.gameObject.SetActive(false);
                mHelpFightRoot.SetActive(false);
            }
        }

        void OnAddMemberSuccess(UIEvent iEvent)
        {
            UpdateMemberInfo();
        }

        void  OnSetNoteNameSuccess(UIEvent iEvent)
        {
            UpdateMemberInfo();
        }
        void OnRemoveMemberSuccess(UIEvent iEvent)
        {
            if (TeamDataManager.GetInstance().HasTeam() == false)
            {
                frameMgr.CloseFrame(this);

                if(ClientSystemManager.instance.IsFrameOpen<TeamMatchWaitingFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<TeamMatchWaitingFrame>();
                }
            }
            else
            {
                UpdateMemberInfo();
            }
        }

        void OnChangeLeaderSuccess(UIEvent iEvent)
        {
            UpdateTeamInfo();
            UpdateMemberInfo();
        }

        void OnTeamInfoChangeSuccess(UIEvent iEvent)
        {
            UpdateTeamInfo();
        }

        void OnTeamPosStateChanged(UIEvent iEvent)
        {
            UpdateMemberInfo();
        }

        void OnMemberStateChanged(UIEvent iEvent)
        {
            UpdateMemberInfo();
        }

        void OnRedPointChanged(UIEvent iEvent)
        {
            ERedPoint RedPointType = (ERedPoint)iEvent.Param1;

            if (RedPointType == ERedPoint.TeamNewRequester)
            {
                mNewRequestRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.TeamNewRequester));
            }
        }

        void OnTeamMatchStartSuccess(UIEvent uiEvent)
        {
            fAddUpTime = 0.0f;
            bStartMatch = true;

            //mCover.gameObject.SetActive(true);

            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            TeamDungeonTable tdData = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);

            if (tdData == null)
            {
                return;
            }

            TeamMatchWaitingData data = new TeamMatchWaitingData();

            if (tdData.MatchType == TeamDungeonTable.eMatchType.QUICK_MATCH)
            {
                data.matchState = MatchState.TeamMatch;
            }
            else
            {
                data.matchState = MatchState.TeamJoin;
            }

            data.TeamDungeonTableID = (int)CurTeamDungeonTableID;
            ClientSystemManager.GetInstance().OpenFrame<TeamMatchWaitingFrame>(FrameLayer.Middle, data);
        }

        private uint CurTeamDungeonTableID
        {
            get {
                return TeamDataManager.GetInstance().TeamDungeonID;
            }
        }


        void OnTeamMatchCancelSuccess(UIEvent uiEvent)
        {
            bStartMatch = false;

            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            TeamDungeonTable tdData = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);

            if (tdData == null)
            {
                return;
            }

            if (tdData.MatchType == TeamDungeonTable.eMatchType.QUICK_MATCH)
            {
                mMatchText.text = "快速匹配";
            }
            else
            {
                mMatchText.text = "快速组队";
            }

            ClientSystemManager.GetInstance().CloseFrame<TeamMatchWaitingFrame>(); // 队长取消匹配后应该关闭匹配界面
            //mCover.gameObject.SetActive(false);
        }

        private void OnTeamTimeChanged(UIEvent uiEvent)
        {
            int time = (int)uiEvent.Param1;

            UpdateCountDownInfo(time);
        }

        [MessageHandle(WorldTeamRequesterListRes.MsgID)]
        void OnTeamRequestersListRes(MsgDATA msg)
        {
            WorldTeamRequesterListRes res = new WorldTeamRequesterListRes();
            res.decode(msg.bytes);

            List<TeammemberBaseInfo> requesters = new List<TeammemberBaseInfo>();

            for (int i = 0; i < res.requesters.Length; i++)
            {
                requesters.Add(res.requesters[i]);
            }

            ClientSystemManager.GetInstance().OpenFrame<TeamRequesterListFrame>(FrameLayer.Middle, requesters);
        }

        void _FixLightDir()
        {
            while (global::Global.Settings.avatarLightDir.x > 360)
                global::Global.Settings.avatarLightDir.x -= 360;
            while (global::Global.Settings.avatarLightDir.x < 0)
                global::Global.Settings.avatarLightDir.x += 360;

            while (global::Global.Settings.avatarLightDir.y > 360)
                global::Global.Settings.avatarLightDir.y -= 360;
            while (global::Global.Settings.avatarLightDir.y < 0)
                global::Global.Settings.avatarLightDir.y += 360;

            while (global::Global.Settings.avatarLightDir.z > 360)
                global::Global.Settings.avatarLightDir.z -= 360;
            while (global::Global.Settings.avatarLightDir.z < 0)
                global::Global.Settings.avatarLightDir.z += 360;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            //_SetFatigueCombustionTime();

            if (bStartMatch)
            {
                Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

                if (myTeam == null)
                {
                    return;
                }

                TeamDungeonTable tdData = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);

                if (tdData == null)
                {
                    return;
                }

                fAddUpTime += timeElapsed;
                int iInt = (int)(fAddUpTime);

                if (tdData.MatchType == TeamDungeonTable.eMatchType.QUICK_MATCH)
                {
                    mMatchText.text = string.Format("匹配中({0}秒)", iInt);
                }
                else
                {
                    mMatchText.text = string.Format("组队中({0}秒)", iInt);
                }
            }
        }

        private string teamNumberPath = "UI/Image/NewPacked/Team.png:Team_Text_{0}";

        private void UpdateCountDownInfo(int time)
        {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            if (myTeam == null || myTeam.members == null || myTeam.leaderInfo == null)
            {
                return;
            }

            for (int i = 0; i < myTeam.members.Length && i < TeamMemberNum; ++i)
            {
                TeamMember data = myTeam.members[i];

                if (data == null || data.id <= 0)
                {
                    continue;
                }
                
                if (i >= MemberInfoUIs.Length)
                {
                    continue;
                }

                MemberUI ui = MemberInfoUIs[i];

                if (ui == null)
                {
                    continue;
                }

                if (data.id != myTeam.leaderInfo.id)
                {
                    ui.countDownRoot.CustomActive(false);
                    continue;
                }

                TeamDataManager.bIsRefreshTime = true;

                ui.countDownRoot.CustomActive(TeamDataManager.AutomaticBackIsStart);

                if (time >= 10)
                {
                    ui.tenImg.CustomActive(true);
                    ui.bitsImg.CustomActive(true);
                    int tenNumber = time / 10;
                    int bitsNumber = time % 10;

                    ETCImageLoader.LoadSprite(ref ui.tenImg, string.Format(teamNumberPath, tenNumber));
                    ETCImageLoader.LoadSprite(ref ui.bitsImg, string.Format(teamNumberPath, bitsNumber));
                    ui.tenImg.SetNativeSize();
                    ui.bitsImg.SetNativeSize();
                }
                else
                {
                    ui.tenImg.CustomActive(false);
                    ui.bitsImg.CustomActive(true);

                    ETCImageLoader.LoadSprite(ref ui.bitsImg, string.Format(teamNumberPath, time));
                    ui.bitsImg.SetNativeSize();
                }
            }
        }

        #region Voice SDK

        void _InitVoiceChatModule()
        {
            if(mComVoiceChat != null)
            {
                mComVoiceChat.Init(ComVoiceChat.ComVoiceChatType.Global);
            }
        }

        void _UnInitVoiceChatModule()
        {
            if(mComVoiceChat != null)
            {
                mComVoiceChat.UnInit();
                mComVoiceChat = null;
            }
        }

        #endregion

        #region ExtraUIBind
        private GameObject mMembersRoot = null;
        private Text mTargetDengeonName = null;
        private GameObject mTargetDungeonResistMagicRoot = null;
        private Text mTargetDengeonResistMagicValue = null;
        private GameObject mFuncs = null;
        private Toggle mBtAutoAgree = null;
        private Image mNewRequestRedPoint = null;
        private Text mMatchText = null;
        private ComCommonBind mBtBuyNum = null;
        private Text mLeaderInviteText = null;
        private Button mOnExit = null;

        private Button mOnReqestList = null;
        private Button mOnMatch = null;
        private Button mOnEnterDungeon = null;
        private Toggle mOnHelpFightStatus = null;
        private Button mOnHelpFight = null;
        private Button mOnTeamTalk = null;
        private GameObject mHelpFightRoot = null;

        private ComVoiceChat mComVoiceChat = null;

        private Toggle togAutoAgreeEnter = null;
        private GameObject goAutoAgree = null;
        private GameObject goAutoAgreeEnter = null;
        private Button mOnClose = null;
        private Button btSelectTarget = null;
        private Button returnPlayerBufsBk = null; // 这个按钮是用来将回归玩家buf信息界面隐藏掉
        private GameObject returnPlayerBufs = null; // 当前显示的回归玩家的buf信息
        private Toggle togAutoReturn = null;
        private GameObject goAutoReturn = null;
        private GameObject mNotCostFatigue = null;
        private Toggle mBtNotCostFatigue = null;
        private ComDropDownControl ChannelSelect = null;

        protected override void _bindExUI()
        {
            mMembersRoot = mBind.GetGameObject("MembersRoot");
            mTargetDengeonName = mBind.GetCom<Text>("TargetDengeonName");
            mTargetDungeonResistMagicRoot = mBind.GetGameObject("TargetDengeonResistMagicValueRoot");
            mTargetDengeonResistMagicValue = mBind.GetCom<Text>("TargetDengeonResistMagicValue");
            mFuncs = mBind.GetGameObject("Funcs");
            mBtAutoAgree = mBind.GetCom<Toggle>("btAutoAgree");
            mBtAutoAgree.onValueChanged.AddListener(_onBtAutoAgreeToggleValueChange);
            mNewRequestRedPoint = mBind.GetCom<Image>("NewRequestRedPoint");
            mMatchText = mBind.GetCom<Text>("MatchText");
            mBtBuyNum = mBind.GetCom<ComCommonBind>("btBuyNum");
            //mLeaderInviteText = mBind.GetCom<Text>("LeaderInviteText");
            mOnExit = mBind.GetCom<Button>("onExit");
            mOnExit.onClick.AddListener(_onOnExitButtonClick);
      
            mOnReqestList = mBind.GetCom<Button>("onReqestList");
            mOnReqestList.onClick.AddListener(_onOnReqestListButtonClick);
            mOnMatch = mBind.GetCom<Button>("onMatch");
            mOnMatch.onClick.AddListener(_onOnMatchButtonClick);
            mOnEnterDungeon = mBind.GetCom<Button>("onEnterDungeon");
            mOnEnterDungeon.onClick.AddListener(_onOnEnterDungeonButtonClick);
            mOnHelpFightStatus = mBind.GetCom<Toggle>("onHelpFightStatus");
            mOnHelpFightStatus.onValueChanged.AddListener(_onOnHelpFightStatusToggleValueChange);
            mOnHelpFight = mBind.GetCom<Button>("onHelpFight");
            mOnHelpFight.onClick.AddListener(_onOnHelpFightButtonClick);
            mOnTeamTalk = mBind.GetCom<Button>("onTeamTalk");
            mOnTeamTalk.onClick.AddListener(_onOnTeamTalkButtonClick);
            mHelpFightRoot = mBind.GetGameObject("helpFightRoot");
            
            mComVoiceChat = mBind.GetCom<ComVoiceChat>("comVoiceChat");

            togAutoAgreeEnter = mBind.GetCom<Toggle>("togAutoAgreeEnter");
            if(togAutoAgreeEnter != null)
            {
                togAutoAgreeEnter.onValueChanged.RemoveAllListeners();
                togAutoAgreeEnter.onValueChanged.AddListener((bool bIsOn) => 
                {
                    TeamDataManager.GetInstance().IsAutoAgree = bIsOn;
                });
            }
            goAutoAgree = mBind.GetGameObject("goAutoAgree");
            goAutoAgreeEnter = mBind.GetGameObject("goAutoAgreeEnter");
            mOnClose = mBind.GetCom<Button>("onClose");
            mOnClose.onClick.AddListener(_onOnCloseButtonClick);
            btSelectTarget = mBind.GetCom<Button>("btSelectTarget");
            if(btSelectTarget != null)
            {
                btSelectTarget.onClick.RemoveAllListeners();
                btSelectTarget.onClick.AddListener(() => 
                {
                    ClientSystemManager.GetInstance().OpenFrame<TeamTargetSelect>(FrameLayer.Middle);
                });
            }
            returnPlayerBufsBk = mBind.GetCom<Button>("returnPlayerBufsBk");
            returnPlayerBufsBk.SafeRemoveAllListener();
            returnPlayerBufsBk.SafeSetOnClickListener(() => 
            {
                returnPlayerBufs.CustomActive(false);
                returnPlayerBufsBk.CustomActive(false);
            });
            togAutoReturn = mBind.GetCom<Toggle>("togAutoReturn");
            togAutoReturn.SafeAddOnValueChangedListener((value) => 
            {
                TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.HellAutoClose, value ? (int)TeamOption.HellAutoClose : 0);
                if (value == true)
                {
                    GameStatisticManager.GetInstance().DoStartBreakThePitPillarAndReturnAutoMatically();
                }
            });
            goAutoReturn = mBind.GetGameObject("goAutoReturn");
            mNotCostFatigue = mBind.GetGameObject("NotCostFatigue");
            mBtNotCostFatigue = mBind.GetCom<Toggle>("btNotCostFatigue");
            mBtNotCostFatigue.SafeSetOnValueChangedListener((value) => 
            {
                if (bToggleInit)
                {
                    bToggleInit = false;
                    return;
                }
                if (value)
                {
                    if(TeamDataManager.GetInstance().GetMemberNum() == 0)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("elite_dungeon_can_not_set_toggle_state_with_one_player"));
                        mBtNotCostFatigue.SafeSetToggleOnState(false);
                    }
                    else
                    {
                        LoginToggleMsgBoxOKCancelFrame.TryShowMsgBox(LoginToggleMsgBoxOKCancelFrame.LoginToggleMsgType.NotCostFatigue,
                        TR.Value("elite_dungeon_not_cost_fatigu_have_no_award"),
                        () =>
                        {
                            TeamDataManager.GetInstance().SendSceneSaveOptionsReq(SaveOptionMask.SOM_NOT_COUSUME_EBERGY, true);
                        },
                        () =>
                        {
                            mBtNotCostFatigue.SafeSetToggleOnState(false);
                        });
                    }                    
                }
                else
                {
                    TeamDataManager.GetInstance().SendSceneSaveOptionsReq(SaveOptionMask.SOM_NOT_COUSUME_EBERGY, false);
                }
            });

            ChannelSelect = mBind.GetCom<ComDropDownControl>("ChannelSelect");            
        }

        protected override void _unbindExUI()
        {
            mMembersRoot = null;
            mTargetDengeonName = null;
            mTargetDungeonResistMagicRoot = null;
            mTargetDengeonResistMagicValue = null;
            mFuncs = null;
            mBtAutoAgree.onValueChanged.RemoveListener(_onBtAutoAgreeToggleValueChange);
            mBtAutoAgree = null;
            mNewRequestRedPoint = null;
            mMatchText = null;
            mBtBuyNum = null;
            //mLeaderInviteText = null;
            mOnExit.onClick.RemoveListener(_onOnExitButtonClick);
            mOnExit = null;
        
            mOnReqestList.onClick.RemoveListener(_onOnReqestListButtonClick);
            mOnReqestList = null;
            mOnMatch.onClick.RemoveListener(_onOnMatchButtonClick);
            mOnMatch = null;
            mOnEnterDungeon.onClick.RemoveListener(_onOnEnterDungeonButtonClick);
            mOnEnterDungeon = null;
            mOnHelpFightStatus.onValueChanged.RemoveListener(_onOnHelpFightStatusToggleValueChange);
            mOnHelpFightStatus = null;
            mOnHelpFight.onClick.RemoveListener(_onOnHelpFightButtonClick);
            mOnHelpFight = null;
            mOnTeamTalk.onClick.RemoveListener(_onOnTeamTalkButtonClick);
            mOnTeamTalk = null;
            mHelpFightRoot = null;

            mComVoiceChat = null;
        
            togAutoAgreeEnter = null;
            goAutoAgree = null;
            goAutoAgreeEnter = null;
            btSelectTarget = null;
            returnPlayerBufsBk = null;
            togAutoReturn = null;
            goAutoReturn = null;
            mNotCostFatigue = null;            
            mBtNotCostFatigue = null;
            ChannelSelect = null;
        }
        #endregion
        #region method    
        // 30 50 55级深渊可以配置是否打完柱子直接返回
        // 60级的深渊也可以配置了 add by qxy 2019-06-14
        bool IsHellTeamDungeonID(int iTeamDungeonID)
        {
            TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(iTeamDungeonID);
            if(teamDungeonTable == null)
            {
                return false;
            }
            DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
            if(dungeonTable == null)
            {
                return false;
            }
            if(dungeonTable.SubType != DungeonTable.eSubType.S_HELL_ENTRY)
            {
                return false;
            }
            int[] dungeonLvs = new int[] {30,50,55,60 };
            if(dungeonLvs == null)
            {
                return false;
            }
            for(int i = 0;i < dungeonLvs.Length;i++)
            {
                if(dungeonTable.Level == dungeonLvs[i])
                {
                    return true;
                }
            }
            return false;
        }
        void UpdateAutoReturnFromHellUI()
        {
            goAutoReturn.CustomActive(IsHellTeamDungeonID((int)TeamDataManager.GetInstance().TeamDungeonID));
            togAutoReturn.SafeSetToggleOnState(TeamDataManager.GetInstance().IsAutoReturnFormHell);
            if(togAutoReturn != null)
            {
                UIGray uiGray = togAutoReturn.gameObject.SafeAddComponent<UIGray>(false);
                if (uiGray != null)
                {
                    uiGray.SetEnable(false);
                    uiGray.SetEnable(!TeamDataManager.GetInstance().IsTeamLeader());
                }
                togAutoReturn.interactable = TeamDataManager.GetInstance().IsTeamLeader();
                togAutoReturn.image.raycastTarget = TeamDataManager.GetInstance().IsTeamLeader();
            }           
        }
        void UpdateEliteNotCostFatigueUI()
        {
            mNotCostFatigue.CustomActive(TeamUtility.IsEliteTeamDungeonID((int)TeamDataManager.GetInstance().TeamDungeonID));
            mBtNotCostFatigue.SafeSetToggleOnState(TeamDataManager.GetInstance().IsNotCostFatigueInEliteDungeon);
            bToggleInit = false;
            mBtNotCostFatigue.SafeSetGray(TeamDataManager.GetInstance().GetMemberNum() == 0,false);            
        }
#endregion   

#region Callback
        private void _onBtAutoAgreeToggleValueChange(bool changed)
        {
            /* put your code in here */
            OnAutoAgreeClicked(changed);
        }

        private void _onOnCloseButtonClick()
        {
            /* put your code in here */
            OnClickClose();
        }

        private void _onOnExitButtonClick()
        {
            /* put your code in here */
            OnExitTeamBtnClicked();
        }



        private void _onOnReqestListButtonClick()
        {
            /* put your code in here */
            OnRequestListClicked();
        }

        private void _onOnMatchButtonClick()
        {
            /* put your code in here */
            OnMatchDugeon();
        }

        private void _onOnEnterDungeonButtonClick()
        {
            /* put your code in here */
            OnEnterDungeon();
        }

        private void _onOnHelpFightStatusToggleValueChange(bool changed)
        {
            /* put your code in here */

        }

        private void _onOnHelpFightButtonClick()
        {
            /* put your code in here */
            _onChangeAssisStatus();
        }

        private void _onOnTeamTalkButtonClick()
        {
            /* put your code in here */

            _onTeamChat();
        }
        #endregion
        #region 精力燃烧
        private void _InitFatigueCombustionRoot()
        {
            int iIndex = _GetRoleIDIndex();
            if (iIndex >= MemberInfoUIs.Length || iIndex < 0)
            {
                Team teamData = TeamDataManager.GetInstance().GetMyTeam();
                if (teamData == null)
                {
                    Logger.LogErrorFormat("GetMyTeam is not exist!");
                    return;
                }
                string errorInfo = "";
                for (int i = 0; i < teamData.members.Length && i < TeamMemberNum; ++i)
                {
                    TeamMember data = teamData.members[i];
                    errorInfo += data.id.ToString() + ";";
                }
                Logger.LogErrorFormat("Can not find roleid {0} is not exist {1}!", PlayerBaseData.GetInstance().RoleID, errorInfo);
                return;
            }
            MemberUI ui = MemberInfoUIs[iIndex];

            if (ui == null)
            {
                return;
            }

            _InitFatigueCombustionGameObject(ui.mFatigueCombustionRoot);

        }
        /// <summary>
        /// 初始化疲劳燃烧是否显示
        /// </summary>
        private void _InitFatigueCombustionGameObject(GameObject mFatigueCombustionRoot)
        {
            ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.FindFatigueCombustionActivityIsOpen(ref mBisFlag, ref data);

            if (mBisFlag == true && data != null)
            {
                mFatigueCombustionRoot.CustomActive(true);

                _InitFatigueCombustionInfo(mFatigueCombustionRoot, data);

            }
            else
            {
                mFatigueCombustionRoot.CustomActive(false);
            }

        }

        /// <summary>
        /// 初始化疲劳燃烧信息
        /// </summary>
        /// <param name="go"></param>
        /// <param name="activityData"></param>
        private void _InitFatigueCombustionInfo(GameObject go, ActivityLimitTime.ActivityLimitTimeData activityData)
        {
            ComCommonBind mBind = go.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            var activityId = activityData.DataId;
            Text mTime = mBind.GetCom<Text>("Time");
            Button mOpen = mBind.GetCom<Button>("Open");
            Button mStop = mBind.GetCom<Button>("Stop");
            GameObject mOrdinaryName = mBind.GetGameObject("OrdinaryName");
            GameObject mSeniorName = mBind.GetGameObject("SeniorName");
            SetButtonGrayCD mCDGray = mBind.GetCom<SetButtonGrayCD>("CDGray");
            mOrdinaryName.CustomActive(false);
            mSeniorName.CustomActive(false);

            for (int i = 0; i < activityData.activityDetailDataList.Count; i++)
            {
                if (activityData.activityDetailDataList[i].ActivityDetailState == ActivityLimitTime.ActivityTaskState.Init ||
                    activityData.activityDetailDataList[i].ActivityDetailState == ActivityLimitTime.ActivityTaskState.UnFinish)
                {
                    continue;
                }

                mData = activityData.activityDetailDataList[i];

                var mTaskId = mData.DataId;

                string mStrID = mTaskId.ToString();
                string mStr = mStrID.Substring(mStrID.Length - 1);
                int mIndex = 0;

                if (int.TryParse(mStr, out mIndex))
                {
                    if (mIndex == 1)
                    {
                        mOrdinaryName.CustomActive(true);
                        mSeniorName.CustomActive(false);
                    }
                    else
                    {
                        mSeniorName.CustomActive(true);
                        mOrdinaryName.CustomActive(false);
                    }
                }

                mOpen.onClick.RemoveAllListeners();
                mOpen.onClick.AddListener(() =>
                {
                    ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RequestOnTakeActTask(activityId, mTaskId);
                    mCDGray.StartGrayCD();
                });

                mStop.onClick.RemoveAllListeners();
                mStop.onClick.AddListener(() =>
                {
                    ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RequestOnTakeActTask(activityId, mTaskId);
                    mCDGray.StartGrayCD();
                });

                _UpdateFatigueCombustionData(go, mData);
                if (activityData.activityDetailDataList[i].ActivityDetailState != ActivityLimitTime.ActivityTaskState.Failed)
                    return;
            }
            
        }

        /// <summary>
        /// 更新疲劳燃烧数据状态
        /// </summary>
        /// <param name="go"></param>
        /// <param name="activityData"></param>
        private void _UpdateFatigueCombustionData(GameObject go, ActivityLimitTime.ActivityLimitTimeDetailData activityData)
        {
            if (go==null || activityData == null)
            {
                return;
            }

            var mBind = go.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }

            mTime = mBind.GetCom<Text>("Time");
            Button mOpen = mBind.GetCom<Button>("Open");
            Button mStop = mBind.GetCom<Button>("Stop");
            mOpen.CustomActive(false);
            mStop.CustomActive(false);
            //是否在燃烧
            mFatigueCombustionTimeIsOpen = false;
            switch (activityData.ActivityDetailState)
            {
                case ActivityLimitTime.ActivityTaskState.Init:
                case ActivityLimitTime.ActivityTaskState.UnFinish:
                    mOpen.CustomActive(true);
                    mTime.text = Function.GetLastsTimeStr(mData.DoneNum);
                    break;
                case ActivityLimitTime.ActivityTaskState.Finished:
                    mStop.CustomActive(true);
                    mFatigueCombustionTimeIsOpen = true;
                    mFatigueCombustionTime = mData.DoneNum;
                    break;
                case ActivityLimitTime.ActivityTaskState.Failed:
                    mTime.text = Function.GetLastsTimeStr(mData.DoneNum);
                    mOpen.CustomActive(true);
                    break;
            }
        }

        /// <summary>
        /// 设置疲劳燃烧的时间
        /// </summary>
        private void _SetFatigueCombustionTime()
        {
            if (mFatigueCombustionTimeIsOpen && mTime != null)
            {
                if (mFatigueCombustionTime - (int)TimeManager.GetInstance().GetServerTime() > 0)
                {
                    mTime.text = Function.GetLastsTimeStr(mFatigueCombustionTime - (int)TimeManager.GetInstance().GetServerTime());
                }
                else
                {
                    int iIndex = _GetRoleIDIndex();

                    MemberUI ui = MemberInfoUIs[iIndex];

                    if (ui == null)
                    {
                        return;
                    }

                    ui.mFatigueCombustionRoot.CustomActive(false);
                }
            }
        }

        /// <summary>
        /// 疲劳燃烧活动任务变化
        /// </summary>
        private void _OnTaskChange()
        {
            int iIndex = _GetRoleIDIndex();

            MemberUI ui = MemberInfoUIs[iIndex];

            if (ui == null)
            {
                return;
            }
            _UpdateFatigueCombustionData(ui.mFatigueCombustionRoot, mData);
        }
        private int _GetRoleIDIndex()
        {
            Team teamData = TeamDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return -1;
            }
            
            for (int i = 0; i < teamData.members.Length && i < TeamMemberNum; ++i)
            {
                TeamMember data = teamData.members[i];

                if (data == null || data.id <= 0)
                {
                    continue;
                }

                if (i >= MemberInfoUIs.Length)
                {
                    continue;
                }

                if (data.id != PlayerBaseData.GetInstance().RoleID)
                {
                    continue;
                }

                int mIndex = i;

                return mIndex;
            }

            return -1;
        }
        #endregion
    }
}
