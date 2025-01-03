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
    // 队伍中的成员界面
    class Pk3v3CrossTeamMyFrame : ClientFrame
    {
        int[] ModelLayers = { 15, 16, 17 };

        private bool mBisFlag = false; //判断疲劳燃烧是否显示
        private ActivityLimitTime.ActivityLimitTimeData data = null;  //疲劳燃烧活动数据
        private ActivityLimitTime.ActivityLimitTimeDetailData mData = null; //疲劳燃烧活动开启的子任务数据
        private bool mFatigueCombustionTimeIsOpen = false;//是否正在倒计时
        private Text mTime; //疲劳燃烧时间Text文本
        private int mFatigueCombustionTime = -1;//用于疲劳燃烧结束时间戳

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
            public Button             AddMemberMark;
            public Button             showMenu;
            public GameObject         modelRoot;
            public GeAvatarRendererEx avatarRenderer = null;
            public GameObject mFatigueCombustionRoot;
            public Text seasonLv;

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

                friendstatus.SetActive(false);
                guildstatus.SetActive(false);
                helpfightstatus.SetActive(false);

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
            }

            /// <summary>
            /// 设置Vip等级
            ///
            /// 若level 小于等于 0, 则隐藏
            ///
            /// </summary>
            public void SetVipLevel(int level)
            {
                if (null == bind)
                {
                    return;
                }

                Text viplevel = bind.GetCom<Text>("viplevel");
                GameObject vipRoot = bind.GetGameObject("vipRoot");

                vipRoot.SetActive(level > 0);
                viplevel.text = level.ToString();
            }
        }

        const int TeamMemberNum = 3;

        bool bStartMatch = false;
        float fAddUpTime = 0.0f;

        MemberUI[] MemberInfoUIs = new MemberUI[TeamMemberNum];

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3Cross/Pk3v3CrossTeamMy";
        }

        #region channel_dropdown_list

        [UIControl("root/ChannelSelect", typeof(MyDropDown))]
        MyDropDown m_dropDown;
        [UIObject("root/ChannelSelect")]
        GameObject m_goLeader;

        void _InitDropDown()
        {
            if (null != m_dropDown)
            {
                
                ChatType[] eChatList = new ChatType[] { ChatType.CT_ACOMMPANY,  ChatType.CT_GUILD, ChatType.CT_NORMAL };
                string[] descs = new string[] {"组队频道","公会频道","附近频道"};
                m_dropDown.BindItems(descs, eChatList, _OnDropDownValueChanged);
            }
        }

        void _UnInitDropDown()
        {
            if (null != m_dropDown)
            {

            }
        }

        void _OnDropDownValueChanged(ChatType eChatType)
        {
            //Logger.LogErrorFormat("{0}", eChatType);
            string content = _GetLeaderInviteWords(true);
            if(string.IsNullOrEmpty(content))
            {
                return;
            }

            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null && pkInfo.nCurPkCount >= Pk3v3CrossDataManager.MAX_PK_COUNT)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("挑战次数已达上限，操作失败");
                return;
            }

            if (frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
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
                        //ChatManager.GetInstance().SendChat(ChatType.CT_ACOMMPANY, content);

                        WorldRoomSendInviteLinkReq req = new WorldRoomSendInviteLinkReq();
                        req.roomId = Pk3v3CrossDataManager.GetInstance().GetRoomInfo().roomSimpleInfo.id;
                        req.channel = (byte)ChanelType.CHAT_CHANNEL_ACCOMPANY;

                        NetManager netMgr = NetManager.Instance();
                        netMgr.SendCommand(ServerType.GATE_SERVER, req);

                        //SystemNotifyManager.SysNotifyTextAnimation("消息发送成功!", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
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
                        //SystemNotifyManager.SysNotifyTextAnimation("消息发送成功!", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                        //ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, content);

                        WorldRoomSendInviteLinkReq req = new WorldRoomSendInviteLinkReq();
                        req.roomId = Pk3v3CrossDataManager.GetInstance().GetRoomInfo().roomSimpleInfo.id;
                        req.channel = (byte)ChanelType.CHAT_CHANNEL_WORLD;

                        NetManager netMgr = NetManager.Instance();
                        netMgr.SendCommand(ServerType.GATE_SERVER, req);
                    }
                    break;
                case ChatType.CT_GUILD:
                    {
                        if(PlayerBaseData.GetInstance().eGuildDuty == EGuildDuty.Invalid)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("chat_guild_talk_need_guild"), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        //SystemNotifyManager.SysNotifyTextAnimation("消息发送成功!",CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                        //ChatManager.GetInstance().SendChat(ChatType.CT_GUILD, content);

                        WorldRoomSendInviteLinkReq req = new WorldRoomSendInviteLinkReq();
                        req.roomId = Pk3v3CrossDataManager.GetInstance().GetRoomInfo().roomSimpleInfo.id;
                        req.channel = (byte)ChanelType.CHAT_CHANNEL_TRIBE;

                        NetManager netMgr = NetManager.Instance();
                        netMgr.SendCommand(ServerType.GATE_SERVER, req);
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
                        //SystemNotifyManager.SysNotifyTextAnimation("消息发送成功!", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                        //ChatManager.GetInstance().SendChat(ChatType.CT_NORMAL, content);

                        WorldRoomSendInviteLinkReq req = new WorldRoomSendInviteLinkReq();
                        req.roomId = Pk3v3CrossDataManager.GetInstance().GetRoomInfo().roomSimpleInfo.id;
                        req.channel = (byte)ChanelType.CHAT_CHANNEL_AROUND;

                        NetManager netMgr = NetManager.Instance();
                        netMgr.SendCommand(ServerType.GATE_SERVER, req);
                    }
                    break;
            }
        }
        #endregion

        protected override void _OnOpenFrame()
        {
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

// 				if (TeamDataManager.GetInstance().IsTeamLeader() && TeamDataManager.GetInstance().GetMemberNum()>=3)
// 				{
// 					ClientSystemManager.GetInstance().delayCaller.DelayCall(5000, ()=>{
// 						_onOnEnterDungeonButtonClick();
// 					});
// 				}
			}
			#endif
        }

        protected override void _OnCloseFrame()
        {
            Clear();
        }

        void Clear()
        {
            bStartMatch = false;
            fAddUpTime = 0.0f;
            mIsSendMessage = false;

            for (int i = 0; i < MemberInfoUIs.Length; i++)
            {
                MemberInfoUIs[i].AddMemberMark.onClick.RemoveAllListeners();
                MemberInfoUIs[i].showMenu.onClick.RemoveAllListeners();
            }
            MemberInfoUIs = new MemberUI[TeamMemberNum];

            UnBindUIEvent();
            _UnInitDropDown();
        }

        void BindUIEvent()
        {
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnAddMemberSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnRemoveMemberSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamChangeLeaderSuccess, OnChangeLeaderSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, OnTeamInfoChangeSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamPosStateChanged, OnTeamPosStateChanged);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMemberStateChanged, OnMemberStateChanged);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshFriendList, OnMemberStateChanged);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMatchStartSuccess, OnTeamMatchStartSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMatchCancelSuccess, OnTeamMatchCancelSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamNotifyChatMsg, _updateTeamChatMsg);
            //             if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
            //             {
            //                 ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.AddSyncTaskDataChangeListener(_OnTaskChange);
            //             }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3BeginMatch, OnBeginMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3BeginMatchRes, OnBeginMatchRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CancelMatch, OnCancelMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CancelMatchRes, OnCancelMatchRes);
        }

        void UnBindUIEvent()
        {
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnAddMemberSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnRemoveMemberSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamChangeLeaderSuccess, OnChangeLeaderSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, OnTeamInfoChangeSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamPosStateChanged, OnTeamPosStateChanged);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMemberStateChanged, OnMemberStateChanged);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshFriendList, OnMemberStateChanged);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMatchStartSuccess, OnTeamMatchStartSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMatchCancelSuccess, OnTeamMatchCancelSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamNotifyChatMsg, _updateTeamChatMsg);
            //             if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
            //                 ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RemoveSyncTaskDataChangeListener(_OnTaskChange);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3BeginMatch, OnBeginMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3BeginMatchRes, OnBeginMatchRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CancelMatch, OnCancelMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CancelMatchRes, OnCancelMatchRes);
        }

        // 刷新房间成员数据
        void OnPk3v3RoomSlotInfoUpdate(UIEvent iEvent)
        {
            // 刷新成员数据
            UpdateMemberInfo();

            UpdateBeginButton();
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
                SystemNotifyManager.SysNotifyFloatingEffect("正在匹配,无法退出");
                return;
            }

            frameMgr.CloseFrame(this);
        }

        void OnExitTeamBtnClicked()
        {
            //TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);

            if(frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                return;
            }

            WorldQuitRoomReq req = new WorldQuitRoomReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        string _GetLeaderInviteWords(bool bNotify)
        {
            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return string.Empty;
            }

//             TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
//             if (data == null)
//             {
//                 return string.Empty;
//             }

            if (Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
            {
                string content = string.Format(TR.Value("LeaderInvite"), "3v3积分争霸赛");

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

        void OnLeaderInvite()
        {
            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

//             TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
//             if (data == null)
//             {
//                 return;
//             }

            if (Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
            {
                string content = string.Format(TR.Value("LeaderInvite"), "3v3积分争霸赛");

                content += "{T ";
                content += myTeam.leaderInfo.id.ToString();
                content += "}";

                //ChatManager.GetInstance().SendChat(ChatType.CT_ACOMMPANY, content);
                ChatFrame cframe = ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle) as ChatFrame;

                cframe.SetTab(ChatType.CT_ACOMMPANY);
                cframe.SetTalkContent(content);
                //SystemNotifyManager.SysNotifyFloatingEffect("发送喊话成功");
            }
            //else
            //{
            //    ChatFrame cframe = ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle) as ChatFrame;
            //    cframe.SetTab(ChatType.CT_TEAM);
            //}
        }

        private void _onTeamChat()
        {
            ChatFrame cframe = ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle) as ChatFrame;
            cframe.SetTab(ChatType.CT_TEAM);
        }

        void OnAutoAgreeClicked(bool value)
        {
//             if(!Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
//             {
//                 return;
//             }
// 
//             if (!value)
//             {
//                 Pk3v3CrossDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.AutoAgree, 0);
//             }
//             else
//             {
//                 Pk3v3CrossDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.AutoAgree, 1);
//             }
        }

        void OnRequestListClicked()
        {
            if (!Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
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

            if(!Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            ScoreWarStatus status = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
            if (status != ScoreWarStatus.SWS_PREPARE && status != ScoreWarStatus.SWS_BATTLE)
            {
                return;
            }

            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null && pkInfo.nCurPkCount >= Pk3v3CrossDataManager.MAX_PK_COUNT)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("挑战次数已达上限，操作失败");
                return;
            }

            if (frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                return;
            }            

            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamInvitePlayerListFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<TeamInvitePlayerListFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<TeamInvitePlayerListFrame>(FrameLayer.Middle, InviteType.Pk3v3Invite);
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

            Pk3v3CrossDataManager.Team teamData = Pk3v3CrossDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }

            Pk3v3CrossDataManager.TeamMember data = null;

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
            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            if (!Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            NetManager netMgr = NetManager.Instance();

            if (!bStartMatch)
            {
                if(Pk3v3CrossDataManager.GetInstance().GetMemberNum() == TeamMemberNum)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("队伍人数已满无法匹配");
                    return;
                }

                if ((int)CurTeamDungeonTableID == 1)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("请选择一个目标副本");
                    return;
                }

                if (!Utility.CheckTeamEnterDungeonCondition((int)CurTeamDungeonTableID))
                {
                    return;
                }

                TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
                if (data == null)
                {
                    return;
                }

                SceneTeamMatchStartReq msg = new SceneTeamMatchStartReq();
                msg.dungeonId = (uint)data.DungeonID;

                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
            else
            {
                WorldTeamMatchCancelReq msg = new WorldTeamMatchCancelReq();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        void _onChangeAssisStatus()
        {
            //Pk3v3CrossDataManager.GetInstance().ChangeMainPlayerAssitState();
        }

        void OnEnterDungeon()
        {
            if (!Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            if((int)CurTeamDungeonTableID == 1)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请选择一个目标副本");
                return;
            }

            if (!Utility.CheckTeamEnterDungeonCondition((int)CurTeamDungeonTableID))
            {
                return;
            }

            TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
            if (data == null)
            {
                return;
            }

            GameFrameWork.instance.StartCoroutine(_teamStart((uint)data.DungeonID));
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
                //Logger.LogErrorFormat("[组队] 等待组队进入副本网络消息中");
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

            //mOnExit.CustomActive(Pk3v3CrossDataManager.GetInstance().IsTeamLeader());
            //mOnEnterDungeon.CustomActive(Pk3v3CrossDataManager.GetInstance().IsTeamLeader());

            UpdateBeginButton();
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
                    Text seasonLv = bind.GetCom<Text>("seasonLv");
                    MemberUI ui = new MemberUI();

                    ui.memberID       = 0;
                    ui.bind           = bind;

                    ui.contentRoot    = content;
                    ui.Level          = level;
                    ui.Job            = job;
                    ui.memberName     = name;
                    ui.leaderMark     = leaderMark;
                    ui.AddMemberMark  = addMemberMask;
                    ui.showMenu       = showMenu;
                    ui.modelRoot      = modelRoot;
                    ui.avatarRenderer = avatarRenderer;
                    ui.mFatigueCombustionRoot = mFatigueCombustionRoot;
                    ui.seasonLv = seasonLv;

                    MemberInfoUIs[i] = ui;
                }
            }
        }

        void UpdateTeamInfo()
        {
            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return;
            }

            ScoreWarStatus scoreWarStatus = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
            UInt32 nEndTime = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatusEndTime();

            mTargetDengeonName.text = "";
            return;

            TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
            if (data == null)
            {
                return;
            }

            mTargetDengeonName.text = data.Name;

            if (Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
            {
                mFuncs.SetActive(true);
                mOnInvite.gameObject.SetActive(false);
                m_goLeader.CustomActive(true);

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
                mOnInvite.gameObject.SetActive(false);
                m_goLeader.CustomActive(false);
                //mLeaderInviteText.text = "组队聊天";
            }

            UpdateBuyNum(data);
        }

        private void _updateMemberFlagInfo()
        {
            Pk3v3CrossDataManager.Team teamData = Pk3v3CrossDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }

            for (int i = 0; i < teamData.members.Length && i < TeamMemberNum; ++i)
            {
                Pk3v3CrossDataManager.TeamMember data = teamData.members[i];

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
                    eTeammateFlag flag = Pk3v3CrossDataManager.GetInstance().GetTeammateFlag(data.id);
                    //Logger.LogProcessFormat("[组队] 更新标记类型 {0}", flag);
                    ui.SetFlag(flag);
                }
            }
        }        

        void UpdateMemberInfo()
        {
            Pk3v3CrossDataManager.Team teamData = Pk3v3CrossDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }           

            int iIndex = 0;
            for (int i = 0; i < teamData.members.Length && i < TeamMemberNum; ++i)
            {
                Pk3v3CrossDataManager.TeamMember data = teamData.members[i];

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

                ui.memberID = data.id;
                ui.contentRoot.SetActive(true);
                ui.Level.text = string.Format("Lv.{0}", data.level);
                ui.memberName.text = data.name;
                ui.leaderMark.gameObject.SetActive(data.id == teamData.leaderInfo.id);
                ui.seasonLv.text = SeasonDataManager.GetInstance().GetRankName((int)data.playerSeasonLevel);

                ui.SetBuzy(data.isBuzy);
                ui.SetVipLevel(data.viplevel);

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
                        ui.avatarRenderer.LoadAvatar(res.ModelPath, ModelLayers[iIndex]);
                        

                        _FixLightDir();

                        PlayerBaseData.GetInstance().AvatarEquipFromItems(ui.avatarRenderer,
                            data.avatarInfo.equipItemIds,
                            data.occu,
                            (int) (data.avatarInfo.weaponStrengthen),
                            null,
                            false,
                            data.avatarInfo.isShoWeapon);
						ui.avatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
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
            }

            for(int i = 0; i < TeamMemberNum; i++)
            {
                MemberUI ui = MemberInfoUIs[i];

                int iIdx = i;
                ui.AddMemberMark.onClick.RemoveAllListeners();
                ui.AddMemberMark.onClick.AddListener(() => { OnClickAddMemberMark(iIdx); });
                ui.AddMemberMark.gameObject.SetActive(ui.memberID <= 0);

                int iIdx2 = i;
                ui.showMenu.onClick.RemoveAllListeners();

                if (ui.memberID > 0 && ui.memberID != PlayerBaseData.GetInstance().RoleID)
                {
                    ui.showMenu.onClick.AddListener(() => { OnClickshowMenu(iIdx2); });
                }
            }

            _updateMemberFlagInfo();
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

        void OnRemoveMemberSuccess(UIEvent iEvent)
        {
            if (Pk3v3CrossDataManager.GetInstance().HasTeam() == false)
            {
                frameMgr.CloseFrame(this);
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

            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();

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

            ClientSystemManager.GetInstance().OpenFrame<TeamMatchWaitingFrame>(FrameLayer.Middle, data);
        }

        private uint CurTeamDungeonTableID
        {
            get {
                return Pk3v3CrossDataManager.GetInstance().TeamDungeonID;
            }
        }


        void OnTeamMatchCancelSuccess(UIEvent uiEvent)
        {
            bStartMatch = false;

            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();

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

            //mCover.gameObject.SetActive(false);
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
            if (bStartMatch)
            {
                Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();

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

//             {
//                 ScoreWarStatus state = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
//                 if (state == ScoreWarStatus.SWS_PREPARE || state == ScoreWarStatus.SWS_BATTLE || state == ScoreWarStatus.SWS_WAIT_END)
//                 {
//                     if (goBattleTimeState != null)
//                     {
//                         goBattleTimeState.CustomActive(true);
//                     }
// 
//                     int nLeftDay = 0;
//                     int nLeftHour = 0;
//                     int nLeftMin = 0;
//                     int nLeftSec = 0;
// 
//                     GetLeftTime(Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatusEndTime(), TimeManager.GetInstance().GetServerTime(), ref nLeftDay, ref nLeftHour, ref nLeftMin, ref nLeftSec);
// 
//                     string strTime = "";
//                     strTime += string.Format("{0:00}", nLeftHour);
//                     strTime += string.Format("{0:00}", nLeftMin);
//                     strTime += string.Format("{0:00}", nLeftSec);
// 
//                     if (strTime.Length == arrNumbers.Count)
//                     {
//                         for (int i = 0; i < strTime.Length; i++)
//                         {
//                             Text txtNumber = arrNumbers[i];
//                             if (txtNumber != null)
//                             {
//                                 txtNumber.text = strTime[i].ToString();
//                             }
//                         }
//                     }
// 
//                 }
//                 else
//                 {
//                     if (goBattleTimeState != null)
//                     {
//                         goBattleTimeState.CustomActive(false);
//                     }
//                 }
//             }
        }

        #region ExtraUIBind
        private GameObject mMembersRoot = null;
        private Text mTargetDengeonName = null;
        private GameObject mFuncs = null;
        private Toggle mBtAutoAgree = null;
        private Image mNewRequestRedPoint = null;
        private Text mMatchText = null;
        private ComCommonBind mBtBuyNum = null;
        private Text mLeaderInviteText = null;
        private Button mOnExit = null;
        private Button mOnInvite = null;
        private Button mOnReqestList = null;
        private Button mOnMatch = null;
        private Button mOnEnterDungeon = null;
        private Toggle mOnHelpFightStatus = null;
        private Button mOnHelpFight = null;
        private Button mOnTeamTalk = null;
        private GameObject mHelpFightRoot = null;

        private Text mBtMatchText = null;
        private UIGray mOnEnterDungeonGray = null;

        GameObject goBattleTimeState = null;
        List<Text> arrNumbers = new List<Text>();

        bool bMatchLock = false;

        protected override void _bindExUI()
        {
            mMembersRoot = mBind.GetGameObject("MembersRoot");
            mTargetDengeonName = mBind.GetCom<Text>("TargetDengeonName");
            mFuncs = mBind.GetGameObject("Funcs");
            mBtAutoAgree = mBind.GetCom<Toggle>("btAutoAgree");
            mBtAutoAgree.onValueChanged.AddListener(_onBtAutoAgreeToggleValueChange);
            mNewRequestRedPoint = mBind.GetCom<Image>("NewRequestRedPoint");
            mMatchText = mBind.GetCom<Text>("MatchText");
            mBtBuyNum = mBind.GetCom<ComCommonBind>("btBuyNum");
            mLeaderInviteText = mBind.GetCom<Text>("LeaderInviteText");
            mOnExit = mBind.GetCom<Button>("onExit");
            mOnExit.onClick.AddListener(_onOnExitButtonClick);
            mOnInvite = mBind.GetCom<Button>("onInvite");
            mOnInvite.onClick.AddListener(_onOnInviteButtonClick);
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

            mBtStartImage = mBind.GetCom<Image>("btStartImage");
            mBtMatchText = mBind.GetCom<Text>("btMatchText");

            goBattleTimeState = mBind.GetGameObject("goBattleTimeState");

            mOnEnterDungeonGray = mBind.GetCom<UIGray>("btEnterDungeonGray");

            for (int i = 0; i < 6; i++)
            {
                Text txtNumber = mBind.GetCom<Text>(string.Format("txtTime{0}", i));
                if (txtNumber != null)
                {
                    arrNumbers.Add(txtNumber);
                }
            }
        }

        protected override void _unbindExUI()
        {
            mMembersRoot = null;
            mTargetDengeonName = null;
            mFuncs = null;
            mBtAutoAgree.onValueChanged.RemoveListener(_onBtAutoAgreeToggleValueChange);
            mBtAutoAgree = null;
            mNewRequestRedPoint = null;
            mMatchText = null;
            mBtBuyNum = null;
            mLeaderInviteText = null;
            mOnExit.onClick.RemoveListener(_onOnExitButtonClick);
            mOnExit = null;
            mOnInvite.onClick.RemoveListener(_onOnInviteButtonClick);
            mOnInvite = null;
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

            mBtStartImage = null;
            mBtMatchText = null;

            goBattleTimeState = null;
            arrNumbers.Clear();

            mOnEnterDungeonGray = null;
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

        private void _onOnInviteButtonClick()
        {
            /* put your code in here */
            OnLeaderInvite();
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
            //OnEnterDungeon();

            _onBtBeginButtonClick();
        }

        string StartBtnRedPath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Quren_Xuanzhong_06";
        string StartBtnBluePath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Lanse_01";
        const int MaxPlayerNum = 3;

        private Image mBtStartImage = null;

        void OnBeginMatch(UIEvent iEvent)
        {
            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

            PkSeekWaitingData data = new PkSeekWaitingData();
            data.roomtype = PkRoomType.Pk3v3Cross;

            ClientSystemManager.GetInstance().OpenFrame<PkSeekWaiting>(FrameLayer.Middle, data);

            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                ShowCancelText(roomInfo);
                //ETCImageLoader.LoadSprite(ref mBtStartImage, StartBtnBluePath);
                //mRight.CustomActive(roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
            }

            UpdateBeginButton();
        }

        void OnBeginMatchRes(UIEvent iEvent)
        {
            bMatchLock = false;
        }

        void OnCancelMatch(UIEvent iEvent)
        {
            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

            ClientSystemManager.GetInstance().CloseFrame<PkSeekWaiting>();

            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                ShowBeginText(roomInfo);
                //ETCImageLoader.LoadSprite(ref mBtStartImage, StartBtnRedPath);
                //mRight.CustomActive(roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
            }

            UpdateBeginButton();
        }

        void UpdateBeginButton()
        {
            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            ScoreWarStatus state = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();

            mOnEnterDungeonGray.SetEnable(false);
            mOnEnterDungeon.interactable = true;
            if (state != ScoreWarStatus.SWS_BATTLE || (roomInfo != null && roomInfo.roomSimpleInfo.id > 0 && roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID))
            {
                mOnEnterDungeonGray.SetEnable(true);
                mOnEnterDungeon.interactable = false;
            }

            mOnEnterDungeon.CustomActive(true);  

            if (roomInfo != null && roomInfo.roomSimpleInfo.id > 0 && roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID)
            {
                mOnEnterDungeon.CustomActive(false);
            }

            if(roomInfo != null && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH)
            {
                mBtMatchText.text = "取消匹配";
            }
            else
            {
                mBtMatchText.text = "开始匹配";
            }

            if(state == ScoreWarStatus.SWS_PREPARE || state == ScoreWarStatus.SWS_BATTLE)
            {
                m_dropDown.m_button.interactable = true;
                
                if(!Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
                {
                    m_dropDown.m_button.interactable = false;
                }
            }
            else
            {
                m_dropDown.m_button.interactable = false;
            }

            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null)
            {
                if (pkInfo.nCurPkCount >= 5)
                {
                    mOnEnterDungeonGray.SetEnable(true);
                    mOnEnterDungeon.interactable = false;
                }
            }
        }

        void OnCancelMatchRes(UIEvent iEvent)
        {
            bMatchLock = false;
        }

        void ShowBeginText(RoomInfo roomInfo)
        {
            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE)
            {
                mBtMatchText.text = "开始决斗";
            }
            else
            {
                mBtMatchText.text = "开始匹配";
            }
        }

        void ShowCancelText(RoomInfo roomInfo)
        {
            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE)
            {
                mBtMatchText.text = "取消决斗";
            }
            else
            {
                mBtMatchText.text = "取消匹配";
            }
        }

        private void _onBtBeginButtonClick()
        {
            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            if (roomInfo == null || roomInfo.roomSlotInfos == null)
            {
                return;
            }

            if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_OPEN)
            {
                if (PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你不是房主,无法开始游戏");
                    return;
                }

                for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                {
                    if (roomInfo.roomSlotInfos[i].playerId > 0 && roomInfo.roomSlotInfos[i].status == (byte)RoomSlotStatus.ROOM_SLOT_STATUS_OFFLINE)
                    {
                        SystemNotifyManager.SystemNotify(9216);
                        return;
                    }
                }
            }

            if(roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_READY)
            {
                return;
            }

            /*ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if(systemTown != null)
            {
                for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                {
                    // 不是房主且不在活动场景，不能开始匹配
                    if (roomInfo.roomSlotInfos[i].playerId > 0 && roomInfo.roomSlotInfos[i].playerId != roomInfo.roomSimpleInfo.ownerId && systemTown.GetSceneObjetByPlayerID(roomInfo.roomSlotInfos[i].playerId) == null)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(string.Format("玩家{0}不在活动场景，无法开始匹配", roomInfo.roomSlotInfos[i].playerName));
                        return;
                    }
                }
            }*/          

            if (bMatchLock)
            {
                return;
            }

            bMatchLock = true;

            if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_OPEN)
            {
                SendBeginGameReq();
            }
            else if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH)
            {
                SendCancelGameReq();
            }
            else
            {
                Logger.LogErrorFormat("Pk3v3 begin state is error, roomstate = {0}", roomInfo.roomSimpleInfo.roomStatus);
            }
        }

        void SendBeginGameReq()
        {
            WorldRoomBattleStartReq req = new WorldRoomBattleStartReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendCancelGameReq()
        {
            WorldRoomBattleCancelReq req = new WorldRoomBattleCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
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

        void GetLeftTime(uint nEndTime, uint nNowTime, ref int nLeftDay, ref int nLeftHour, ref int nLeftMin, ref int nLeftSec)
        {
            nLeftDay = 0;
            nLeftHour = 0;
            nLeftMin = 0;
            nLeftSec = 0;

            if (nEndTime <= nNowTime)
            {
                return;
            }

            uint LeftTime = nEndTime - nNowTime;

            uint Day = LeftTime / (24 * 60 * 60);
            LeftTime -= Day * 24 * 60 * 60;

            uint Hour = LeftTime / (60 * 60);
            LeftTime -= Hour * 60 * 60;

            uint Minute = LeftTime / 60;
            LeftTime -= Minute * 60;

            uint Second = LeftTime;

            nLeftDay = (int)Day;
            nLeftHour = (int)Hour;
            nLeftMin = (int)Minute;
            nLeftSec = (int)Second;

            return;
        }     
    }
}
