using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    public enum InviteType
    {
        None = 0,
        TeamInvite,
        Pk3v3Invite,
    }

    public enum InviteUIType
    {
        ByLeader, // 队长发起的邀请
        ByMember, // 队员发起的邀请
    }
    class TeamInvitePlayerListFrame : ClientFrame
    {
        InviteType invitetype = InviteType.None;
        static InviteUIType inviteUIType = InviteUIType.ByLeader;

        public class PlayersData
        {
            public ulong uid;
            public string name;
            public byte occu;
            public ushort level;
            public bool IsInvite;
            public string remark;//备注名称
            public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();
        }

        class PlayersUI
        {
            public Image Icon;
            public Text Name;
            public Text Level;
            public Text Job;
            public Button Invite;
            public Text InviteText;
            public GameObject returnPlayer;
            public GameObject myFriend;
            public GameObject myGuild;
        }

        const int TabNum = 3;

        int CurTabIndex = 0;

        List<GameObject> PlayersObjList = new List<GameObject>();
        List<PlayersUI> PlayersUIInfo = new List<PlayersUI>();

        List<PlayersData> NearList = new List<PlayersData>();
        List<PlayersData> FriendsList = new List<PlayersData>();
        List<PlayersData> GuildMemberList = new List<PlayersData>();

        bool bIsIn3v3Cross = false;

        int iNeedMinLv = 1;
        bool canInvate = true;
        float curTimeUpdateTime = 0;
        float lastTimeUpdateTime = 0;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamInvitePlayerList";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                invitetype = (InviteType)userData;
            }

            InitInterface();
            BindUIEvent();
            canInvate = true;

            if(tabs != null)
            {
                tabs.InitComTab((tabData) => 
                {
                    CurTabIndex = tabData.id;
                    UpdateInterface();
                });

                togNearby = tabs.GetToggle(0);
                togFriend = tabs.GetToggle(1);
                togGuild = tabs.GetToggle(2);
            }

            bIsIn3v3Cross = frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>();
            // 队员进行普通组队邀请相关UI要稍微调整
            if(invitetype == InviteType.TeamInvite && !TeamDataManager.GetInstance().IsTeamLeader())
            {
                mBtInviteAll.CustomActive(false);
                togFriend.SafeSetToggleOnState(true);
                togNearby.CustomActive(false);
            }
        }

        protected override void _OnCloseFrame()
        {
            bIsIn3v3Cross = false;
            Clear();
            UnBindUIEvent();
        }

        void Clear()
        {
            CurTabIndex = 0;

            for(int i = 0; i < PlayersUIInfo.Count; i++)
            {
                PlayersUIInfo[i].Invite.onClick.RemoveAllListeners();
            }
            PlayersUIInfo.Clear();

            PlayersObjList.Clear();

            NearList.Clear();
            FriendsList.Clear();
            GuildMemberList.Clear();
            canInvate = true;
            iNeedMinLv = 1;
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildMembersUpdated, OnGuildMembersUpdated);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildMembersUpdated, OnGuildMembersUpdated);
        }

//         [UIEventHandle("middle/Title/btClose")]
//         void OnClose()
//         {
//             frameMgr.CloseFrame(this);
//         }

//         [UIEventHandle("middle/Tab/Func{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, TabNum)]
//         void OnSwitchLabel(int iIndex, bool value)
//         {
//             if (iIndex < 0 || !value)
//             {
//                 return;
//             }
// 
//             CurTabIndex = iIndex;
// 
//             UpdateInterface();
//         }

        void OnClickInvite(int iIndex)
        {
            if (frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("当前状态无法进行该操作");
                return;
            }

            if (iIndex < 0)
            {
                return;
            }

            ulong uid = 0;
            int level = 1;

            if (CurTabIndex == 0)
            {
                if (iIndex >= NearList.Count || NearList[iIndex].IsInvite)
                {
                    return;
                }

                uid = NearList[iIndex].uid;
                level = NearList[iIndex].level;
            }
            else if(CurTabIndex == 1)
            {
                if (iIndex >= FriendsList.Count || FriendsList[iIndex].IsInvite)
                {
                    return;
                }

                uid = FriendsList[iIndex].uid;
                level = FriendsList[iIndex].level;
            }
            else if(CurTabIndex == 2)
            {
                if (iIndex >= GuildMemberList.Count || GuildMemberList[iIndex].IsInvite)
                {
                    return;
                }
                if(iIndex >= NearList.Count)
                {
                    return;
                }

                if(!GuildDataManager.GetInstance().HasSelfGuild())
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你尚未加入一个公会");
                    return;
                }

                uid = GuildMemberList[iIndex].uid;
                level = NearList[iIndex].level;
            }

            if(bIsIn3v3Cross)
            {
                if (invitetype == InviteType.Pk3v3Invite)
                {
                    if (level < Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("对方尚未解锁决斗场");
                        return;
                    }

                    if (Pk3v3CrossDataManager.GetInstance().CheckIsInMyRoom(uid))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("对方已经在你队伍中");
                        return;
                    }

                    Pk3v3CrossDataManager.GetInstance().Pk3v3RoomInviteOtherPlayer(uid);
                }
                else
                {
                    if (level < Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Team))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("对方尚未解锁组队功能");
                        return;
                    }

                    if (level < iNeedMinLv)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("对方等级无法进入当前组队副本");
                        return;
                    }

                    if (CheckIsInMyTeam(uid))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("对方已经在你队伍中");
                        return;
                    }

                    TeamDataManager.GetInstance().TeamInviteOtherPlayer(uid);
                }
            }
            else
            {
                if (invitetype == InviteType.Pk3v3Invite)
                {
                    if (level < Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("对方尚未解锁决斗场");
                        return;
                    }

                if(Pk3v3DataManager.GetInstance().CheckIsInMyRoom(uid))
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("对方已经在你房间中");
                    return;
                }

                Pk3v3DataManager.GetInstance().Pk3v3RoomInviteOtherPlayer(uid);
            }
            else
            {
                if (level < Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Team))
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("对方尚未解锁组队功能");
                    return;
                }

                if (level < iNeedMinLv)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("对方等级无法进入当前组队副本");
                    return;
                }

                if (CheckIsInMyTeam(uid))
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("对方已经在你队伍中");
                    return;
                }

                    TeamDataManager.GetInstance().TeamInviteOtherPlayer(uid);
                }
            }

            if (CurTabIndex == 0)
            {
                if(iIndex < NearList.Count)
                NearList[iIndex].IsInvite = true;
            }
            else if (CurTabIndex == 1)
            {
                if (iIndex < FriendsList.Count)
                    FriendsList[iIndex].IsInvite = true;
            }
            else if (CurTabIndex == 2)
            {
                if (iIndex < GuildMemberList.Count)
                    GuildMemberList[iIndex].IsInvite = true;
            }

            UpdateInterface();
        }

        void InitInterface()
        {
            mBtInviteAll.gameObject.CustomActive(invitetype == InviteType.TeamInvite);
            m3v3Root.CustomActive(invitetype == InviteType.Pk3v3Invite);

            if(invitetype == InviteType.TeamInvite)
            {
                TeamDungeonTable TeamDungeonData = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)TeamDataManager.GetInstance().TeamDungeonID);
                if (TeamDungeonData == null)
                {
                    return;
                }

                iNeedMinLv = TeamDungeonData.MinLevel;
            }

//             for (int i = 0; i < FuncTab.Length; i++)
//             {
//                 if (i == 0)
//                 {
//                     FuncTab[i].isOn = true;
//                 }
//             }

            SendFindNearPlayersReq();
            InitFriendsData();

            if(GuildDataManager.GetInstance().HasSelfGuild())
            {
                GuildDataManager.GetInstance().RequestGuildMembers();
            }           
        }

        void UpdateInterface()
        {
            int RealNum = 0;

            if(CurTabIndex == 0)
            {
                RealNum = NearList.Count;
            }
            else if(CurTabIndex == 1)
            {
                RealNum = FriendsList.Count;
            }
            else if(CurTabIndex == 2)
            {
                RealNum = GuildMemberList.Count;
            }

            if (RealNum > PlayersObjList.Count)
            {
                int iOriNum = PlayersObjList.Count;
                int iDifference = RealNum - PlayersObjList.Count;

                for (int i = 0; i < iDifference; i++)
                {
                    GameObject EleObj = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Team/TeamInviteEle");
                    if (EleObj == null)
                    {
                        continue;
                    }

                    Utility.AttachTo(EleObj, PlayersRoot);
                    PlayersObjList.Add(EleObj);

                    PlayersUI ui = new PlayersUI();

                    Image[] imgs = EleObj.GetComponentsInChildren<Image>();
                    for(int j = 0; j < imgs.Length; j++)
                    {
                        if(imgs[j].name == "Icon")
                        {
                            ui.Icon = imgs[j];
                            break;
                        }
                    }

                    Text[] texts = EleObj.GetComponentsInChildren<Text>();
                    for (int j = 0; j < texts.Length; j++)
                    {
                        if (texts[j].name == "Name")
                        {
                            ui.Name = texts[j];
                        }
                        else if(texts[j].name == "Job")
                        {
                            ui.Job = texts[j];
                        }
                        else if(texts[j].name == "Level")
                        {
                            ui.Level = texts[j];
                        }
                        else if(texts[j].name == "content")
                        {
                            ui.InviteText = texts[j];
                        }
                    }

                    ui.Invite = EleObj.GetComponentInChildren<Button>();
                    ui.Invite.gameObject.AddComponent<UIGray>();

                    ComCommonBind bind = EleObj.GetComponent<ComCommonBind>();
                    if(bind != null)
                    {
                        ui.returnPlayer = bind.GetGameObject("returnPlayer");
                        ui.myFriend = bind.GetGameObject("myFriend");
                        ui.myGuild = bind.GetGameObject("myGuild");
                    }
                    PlayersUIInfo.Add(ui);
                }
            }

            for (int i = 0; i < PlayersObjList.Count; i++)
            {
                if (i < RealNum)
                {
                    PlayersUI ui = PlayersUIInfo[i];
                    PlayersData playerdata = null;

                    if (CurTabIndex == 0)
                    {
                        playerdata = NearList[i];        
                    }
                    else if(CurTabIndex == 1)
                    {
                        playerdata = FriendsList[i];
                    }
                    else if(CurTabIndex == 2)
                    {
                        playerdata = GuildMemberList[i];
                    }

                    if(playerdata == null)
                    {
                        PlayersObjList[i].SetActive(false);
                        continue;
                    }

                    if (playerdata.remark != null && playerdata.remark != "")
                    {
                        ui.Name.text = playerdata.remark;
                    }
                    else
                    {
                        ui.Name.text = playerdata.name;
                    }
                    ui.Level.text = string.Format("Lv.{0}", playerdata.level);

                    ui.returnPlayer.CustomActive(false);
                    ui.myFriend.CustomActive(false);
                    ui.myGuild.CustomActive(false);
                    RelationData relationData = null;
                    bool isMyFriend = RelationDataManager.GetInstance().FindPlayerIsRelation(playerdata.uid, ref relationData);
                    bool isMyGuild = GuildDataManager.GetInstance().IsSameGuild(playerdata.playerLabelInfo.guildId);
                    if (CurTabIndex == 0)// 附近
                    {
                        if (playerdata.playerLabelInfo.returnStatus == 1)
                        {
                            ui.returnPlayer.CustomActive(true);
                        }
                        else if (isMyFriend)
                        {
                            ui.myFriend.CustomActive(true);
                        }
                        else if (isMyGuild)
                        {
                            ui.myGuild.CustomActive(true);
                        }
                    }
                    else if (CurTabIndex == 1) // 好友
                    {
                        if (playerdata.playerLabelInfo.returnStatus == 1)
                        {
                            ui.returnPlayer.CustomActive(true);
                        }                  
                        else if (isMyGuild)
                        {
                            ui.myGuild.CustomActive(true);
                        }
                    }
                    else if (CurTabIndex == 2) // 公会
                    {
                        if (playerdata.playerLabelInfo.returnStatus == 1)
                        {
                            ui.returnPlayer.CustomActive(true);
                        }
                        else if (isMyFriend)
                        {
                            ui.myFriend.CustomActive(true);
                        }
                    }                    
                    JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(playerdata.occu);
                    if (jobData != null)
                    {
                        ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                        if (resData != null)
                        {
                            //Sprite spr = AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
                            //if (spr != null)
                            //{
                            //    ui.Icon.sprite = spr;
                            //}
                            ETCImageLoader.LoadSprite(ref ui.Icon, resData.IconPath);
                        }

                        ui.Job.text = jobData.Name;
                    }

                    int iIndex = i;
                    ui.Invite.onClick.RemoveAllListeners();
                    ui.Invite.onClick.AddListener(() => { OnClickInvite(iIndex); });

                    if(playerdata.IsInvite)
                    {
                        ui.InviteText.text = "已邀请";

                        ui.Invite.gameObject.GetComponent<UIGray>().enabled = true;
                        ui.Invite.interactable = false;
                    }
                    else
                    {
                        ui.InviteText.text = "邀请";

                        ui.Invite.gameObject.GetComponent<UIGray>().enabled = false;
                        ui.Invite.interactable = true;
                    }

                    PlayersObjList[i].SetActive(true);
                }
                else
                {
                    PlayersObjList[i].SetActive(false);
                }
            }
        }

        private int SortPlayersData(PlayersData a, PlayersData b)
        {
            if(a == null || b == null)
            {
                return 0;
            }
            if(a.playerLabelInfo == null || b.playerLabelInfo == null)
            {
                return 0;
            }
            RelationData relationData = null;
            bool aIsMyFriend = RelationDataManager.GetInstance().FindPlayerIsRelation(a.uid, ref relationData);
            bool bIsMyFriend = RelationDataManager.GetInstance().FindPlayerIsRelation(b.uid, ref relationData);
            bool aIsMyGuild = GuildDataManager.GetInstance().IsSameGuild(a.playerLabelInfo.guildId);
            bool bIsMyGuild = GuildDataManager.GetInstance().IsSameGuild(b.playerLabelInfo.guildId);
            if (a.playerLabelInfo.returnStatus != b.playerLabelInfo.returnStatus)
            {
            return b.playerLabelInfo.returnStatus - a.playerLabelInfo.returnStatus;        
            }
            if (aIsMyFriend != bIsMyFriend)
            {
                return bIsMyFriend.CompareTo(aIsMyFriend);
            }
            return bIsMyGuild.CompareTo(aIsMyGuild);
        }
        void InitFriendsData()
        {
            List<RelationData> temp = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_FRIEND);

            // 师徒关系也要加入到好友里
            List<RelationData> Teacher = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_MASTER);
            List<RelationData> Student = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);

            temp.InsertRange(temp.Count, Teacher);
            temp.InsertRange(temp.Count, Student);

            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].isOnline == 1)
                {
                    PlayersData data = new PlayersData();

                    data.uid = temp[i].uid;
                    data.name = temp[i].name;
                    data.occu = temp[i].occu;
                    data.level = temp[i].level;
                    data.remark = temp[i].remark;
                    data.IsInvite = false;

                    data.playerLabelInfo.awakenStatus = temp[i].playerLabelInfo.awakenStatus;
                    data.playerLabelInfo.returnStatus = temp[i].playerLabelInfo.returnStatus;

                    data.playerLabelInfo.returnStatus = temp[i].isRegress; // 好友数据里面用另外一个字段表示回归标志 属于历史遗留问题
                    FriendsList.Add(data);
                }
            }
            if(FriendsList != null)
            {
                //FriendsList[FriendsList.Count - 1].playerLabelInfo.returnStatus = 1;
                FriendsList.Sort(SortPlayersData);
            }
        }

        void SendFindNearPlayersReq()
        {
            WorldRelationFindPlayersReq req = new WorldRelationFindPlayersReq();
            if(invitetype == InviteType.TeamInvite)
            {
                req.type = (byte)RelationFindType.Team;
            }
            else if(invitetype == InviteType.Pk3v3Invite)
            {
                req.type = (byte)RelationFindType.Room;
            }
            
            req.name = "";
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendOnekeyInviteRoomReq(uint roomid,byte channel = (byte)ChanelType.CHAT_CHANNEL_ACCOMPANY)
        {
            WorldRoomSendInviteLinkReq req = new WorldRoomSendInviteLinkReq();
            req.roomId = roomid;
            req.channel = channel;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        [MessageHandle(WorldRelationFindPlayersRet.MsgID)]
        void OnFindNearPlayersRes(MsgDATA msg)
        {
            WorldRelationFindPlayersRet res = new WorldRelationFindPlayersRet();

            res.decode(msg.bytes);
            if (res.type != (byte)RelationFindType.Room && res.type != (byte)RelationFindType.Team)
            {
                return;
            }

            NearList.Clear();
            for (int i = 0; i < res.friendList.Length; i++)
            {
                PlayersData data = new PlayersData();

                data.uid = res.friendList[i].playerId;
                data.name = res.friendList[i].name;
                data.occu = res.friendList[i].occu;
                data.level = res.friendList[i].level;
                data.IsInvite = false;
                data.playerLabelInfo.awakenStatus = res.friendList[i].playerLabelInfo.awakenStatus;
                data.playerLabelInfo.returnStatus = res.friendList[i].playerLabelInfo.returnStatus;
                RelationData relationData = null;
                RelationDataManager.GetInstance().FindPlayerIsRelation(res.friendList[i].playerId, ref relationData);
                if (relationData != null)
                {
                    if (relationData.remark != null && relationData.remark != "")
                    {
                        data.remark = relationData.remark;
                    }
                }

                NearList.Add(data);
            }
            if(NearList != null)
            {
                NearList.Sort(SortPlayersData);
            }

            UpdateInterface();
        }

        void OnGuildMembersUpdated(UIEvent iEvent)
        {
            GuildMemberList.Clear();

            List<GuildMemberData> Memberdata = GuildDataManager.GetInstance().GetMembers();

            for (int i = 0; i < Memberdata.Count; i++)
            {
                PlayersData data = new PlayersData();

                if(Memberdata[i].uGUID == PlayerBaseData.GetInstance().RoleID || Memberdata[i].uOffLineTime != 0)
                {
                    continue;
                }

                data.uid = Memberdata[i].uGUID;
                data.name = Memberdata[i].strName;
                data.level = (ushort)Memberdata[i].nLevel;
                data.occu = (byte)Memberdata[i].nJobID;
                data.remark = Memberdata[i].remark;
                data.playerLabelInfo = Memberdata[i].playerLabelInfo;
                data.IsInvite = false;

                GuildMemberList.Add(data);
            }
            if(GuildMemberList != null)
            {
                GuildMemberList.Sort(SortPlayersData);
            }
        }

        bool CheckIsInMyTeam(ulong uId)
        {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return false;
            }

            for (int i = 0; i < myTeam.members.Length; i++)
            {
                if (myTeam.members[i].id == uId)
                {
                    return true;
                }
            }

            return false;
        }

//         [UIControl("middle/Tab/Func{0}", typeof(Toggle), 1)]
//         protected Toggle[] FuncTab = new Toggle[TabNum];

        [UIObject("middle/Scroll View/Viewport/Content")]
        protected GameObject PlayersRoot;

        #region ExtraUIBind
        private Button mBtInviteAll = null;
        private GameObject m3v3Root = null;
        private Button mOneKeyPropaganda = null;
        private Button mBt3v3InviteAll = null;
        private Text mInviteTime = null;
        private UIGray mInviteGray = null;
        private Toggle togNearby = null;
        private Toggle togFriend = null;
        private Toggle togGuild = null;
        private CommonTabToggleGroup tabs = null;

        protected override void _bindExUI()
        {
            mBtInviteAll = mBind.GetCom<Button>("btInviteAll");
            mBtInviteAll.onClick.AddListener(_onBtInviteAllButtonClick);
            m3v3Root = mBind.GetGameObject("3v3Root");
            mOneKeyPropaganda = mBind.GetCom<Button>("OneKeyPropaganda");
            mOneKeyPropaganda.onClick.AddListener(_onOneKeyPropagandaButtonClick);
            mBt3v3InviteAll = mBind.GetCom<Button>("bt3v3InviteAll");
            mBt3v3InviteAll.onClick.AddListener(_onBt3v3InviteAllButtonClick);
            mInviteTime = mBind.GetCom<Text>("InviteTime");
            mInviteGray = mBind.GetCom<UIGray>("InviteGray");
            tabs = mBind.GetCom<CommonTabToggleGroup>("tabs");
        }

        protected override void _unbindExUI()
        {
            mBtInviteAll.onClick.RemoveListener(_onBtInviteAllButtonClick);
            mBtInviteAll = null;
            m3v3Root = null;
            mOneKeyPropaganda.onClick.RemoveListener(_onOneKeyPropagandaButtonClick);
            mOneKeyPropaganda = null;
            mBt3v3InviteAll.onClick.RemoveListener(_onBt3v3InviteAllButtonClick);
            mBt3v3InviteAll = null;
            mInviteTime = null;
            mInviteGray = null;
            togNearby = null;
            togFriend = null;
            togGuild = null;
            tabs = null;
        }
        #endregion

        #region Callback
        private void _onBtInviteAllButtonClick()
        {
            if (!TeamDataManager.GetInstance().IsTeamLeader())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("你不是队长没有权限邀请");
                return;
            }

            List<PlayersData> List = null;

            if (CurTabIndex == 0)
            {
                List = NearList;
            }
            else if (CurTabIndex == 1)
            {
                List = FriendsList;
            }
            else if (CurTabIndex == 2)
            {
                if (!GuildDataManager.GetInstance().HasSelfGuild())
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你尚未加入一个公会");
                    return;
                }

                List = GuildMemberList;
            }

            if (List == null)
            {
                return;
            }

            bool bHasSend = false;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].level < Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Team))
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("{0}尚未解锁组队功能", List[i].name));
                    List[i].IsInvite = true;

                    continue;
                }

                if (List[i].level < iNeedMinLv)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("{0}等级无法进入当前组队副本", List[i].name));
                    List[i].IsInvite = true;

                    continue;
                }

                if (List[i].IsInvite)
                {
                    continue;
                }

                if (CheckIsInMyTeam(List[i].uid))
                {
                    continue;
                }

                TeamDataManager.GetInstance().TeamInviteOtherPlayer(List[i].uid);
                bHasSend = true;

                List[i].IsInvite = true;
            }

            if (bHasSend)
            {
                UpdateInterface();
            }
        }

        private void _onOneKeyPropagandaButtonClick()
        {
            if(invitetype != InviteType.Pk3v3Invite)
            {
                return;
            }

            if(frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("当前状态无法进行该操作");
                return;
            }

            if(bIsIn3v3Cross)
            {
                RoomInfo roominfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
                if (roominfo == null)
                {
                    return;
                }

                SendOnekeyInviteRoomReq(roominfo.roomSimpleInfo.id,(byte)ChanelType.CHAT_CHANNEL_AROUND);
                canInvate = true;
                Pk3v3CrossDataManager.GetInstance().SimpleInviteLastTime = 30;
            }
            else
            {
                RoomInfo roominfo = Pk3v3DataManager.GetInstance().GetRoomInfo();
                if (roominfo == null)
                {
                    return;
                }

                SendOnekeyInviteRoomReq(roominfo.roomSimpleInfo.id);
                canInvate = true;
                Pk3v3DataManager.GetInstance().SimpleInviteLastTime = 30;
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }
        protected override void _OnUpdate(float LastTime)
        {
           if(bIsIn3v3Cross)
            {
                if (canInvate)
                {
                    curTimeUpdateTime = Time.time;
                    if (curTimeUpdateTime - lastTimeUpdateTime >= 1)
                    {
                        if (Pk3v3CrossDataManager.GetInstance().SimpleInviteLastTime > 0)
                        {
                            mInviteTime.text = string.Format("一键喊话{0}s", Pk3v3CrossDataManager.GetInstance().SimpleInviteLastTime);
                            mInviteGray.enabled = true;
                            mOneKeyPropaganda.interactable = false;
                        }
                        else
                        {
                            canInvate = false;
                            mInviteTime.text = string.Format("一键喊话", Pk3v3CrossDataManager.GetInstance().SimpleInviteLastTime);
                            mInviteGray.enabled = false;
                            mOneKeyPropaganda.interactable = true;
                        }
                        lastTimeUpdateTime = curTimeUpdateTime;
                    }
                }
            }
           else
            {
                if (canInvate)
                {
                    curTimeUpdateTime = Time.time;
                    if (curTimeUpdateTime - lastTimeUpdateTime >= 1)
                    {
                        if (Pk3v3DataManager.GetInstance().SimpleInviteLastTime > 0)
                        {
                            mInviteTime.text = string.Format("一键喊话{0}s", Pk3v3DataManager.GetInstance().SimpleInviteLastTime);
                            mInviteGray.enabled = true;
                            mOneKeyPropaganda.interactable = false;
                        }
                        else
                        {
                            canInvate = false;
                            mInviteTime.text = string.Format("一键喊话", Pk3v3DataManager.GetInstance().SimpleInviteLastTime);
                            mInviteGray.enabled = false;
                            mOneKeyPropaganda.interactable = true;
                        }
                        lastTimeUpdateTime = curTimeUpdateTime;
                    }
                }
            }
        }

        private void _onBt3v3InviteAllButtonClick()
        {
            if (frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("当前状态无法进行该操作");
                return;
            }

            List<PlayersData> List = null;

            if (CurTabIndex == 0)
            {
                List = NearList;
            }
            else if (CurTabIndex == 1)
            {
                List = FriendsList;
            }
            else if (CurTabIndex == 2)
            {
                if (!GuildDataManager.GetInstance().HasSelfGuild())
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你尚未加入一个公会");
                    return;
                }

                List = GuildMemberList;
            }

            if (List == null)
            {
                return;
            }

            bool bHasSend = false;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].level < Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel))
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("{0}尚未解锁决斗场", List[i].name));
                    List[i].IsInvite = true;

                    continue;
                }

                if (List[i].IsInvite)
                {
                    continue;
                }

                if(bIsIn3v3Cross)
                {
                    if (Pk3v3CrossDataManager.GetInstance().CheckIsInMyRoom(List[i].uid))
                    {
                        continue;
                    }

                    Pk3v3CrossDataManager.GetInstance().Pk3v3RoomInviteOtherPlayer(List[i].uid);
                    bHasSend = true;
                }
                else
                {
                    if (Pk3v3DataManager.GetInstance().CheckIsInMyRoom(List[i].uid))
                    {
                        continue;
                    }

                    Pk3v3DataManager.GetInstance().Pk3v3RoomInviteOtherPlayer(List[i].uid);
                    bHasSend = true;
                }

                List[i].IsInvite = true;
            }

            if (bHasSend)
            {
                UpdateInterface();
            }
        }
        #endregion
    }
}
