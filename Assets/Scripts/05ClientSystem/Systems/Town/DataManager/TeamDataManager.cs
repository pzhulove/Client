//using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public enum eTeammateFlag
    {
        /// <summary>
        /// 陌生人
        /// </summary>
        None = 0,

        /// <summary>
        /// 好友
        /// </summary>
        Friend = 1,

        /// <summary>
        /// 工会
        /// </summary>
        Guild = 2,

        /// <summary>
        /// 助战
        /// </summary>
        HelpFight = 4,
        
        /// <summary>
        /// 师傅
        /// </summary>
        Master = 8,

        /// <summary>
        /// 师傅
        /// </summary>
        Disciple = 16,
    }

    class DiffInfo
    {
        public List<int> secteamDungeonID = new List<int>();
        public string dungeonName;
        public int iMinLv;
        public int iMaxLv;
    }
    class TeamMember
    {
        //public byte pos;
        //public bool isOpened;
        public ulong id;

        /// <summary>
        /// 工会ID 
        /// </summary>
        public ulong guildid;
        public string name;
        public ushort level;

        /// <summary>
        /// 工会ID 
        /// </summary>
        public ushort viplevel;

        public byte occu;


        public bool isOnline;
        public bool isPrepared;

        /// <summary>
        /// 是否繁忙，是否在战斗中
        /// </summary>
        public bool isBuzy;

        /// <summary>
        /// 是否助战
        /// </summary>
        public bool isAssist;

        /// <summary>
        /// 地下城剩余次数
        /// </summary>
        public int dungeonLeftCount;

		public PlayerAvatar avatarInfo;

        //抗魔值
        public uint resistMagicValue;

        public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();
        public void ClearPlayerInfo()
        {
            id = 0;
            guildid = 0;
            name = "";
            level = 0;
            viplevel = 0;
            dungeonLeftCount = 0;
            occu = 0;
            isOnline = false;
            isPrepared = false;
            isAssist = false;
            isBuzy = false;
            avatarInfo = null;
            resistMagicValue = 0;
        }

        public void Debug()
        {
            Logger.LogProcessFormat("[组队数据] 成员ID:{0} 是否准备:{1} 是否在线:{2} 名字:{3} 等级:{4} 职业:{5}", 
                 id, isPrepared, isOnline, name, level, occu);
        }
    }

    class Team
    {
        public uint teamID;
        public TeammemberBaseInfo leaderInfo;
        public byte currentMemberCount;
        public byte maxMemberCount;
        public uint autoAgree;

        public uint teamDungeonID;

        public TeamMember[] members = new TeamMember[3];

        public void Debug()
        {
            Logger.LogProcessFormat("[组队数据] 开始输出组队数据");
//             Logger.LogProcessFormat(
//                 "teamID:{0} teamName:{1} chapterID:{2} dungeonID:{3} hardType:{4} leaderID:{5} count:{6}/{7} passward:{8}",
//                 id, teamName, chapterID, dungeonID, hardType, leaderID, currentMemberCount, maxMemberCount, hasPassword
//                 );
            for (int i = 0; i < members.Length; ++i)
            {
                TeamMember member = members[i];
                if (member != null)
                {
                    member.Debug();
                }
            }

            Logger.LogProcessFormat("[组队数据] 结束输出组队数据");
        }
    }

    class TeamSearchInfo
    {
        public uint teamID;
        public string teamName;
        public uint chapterID;
        public byte hardType;

        public TeamSearchInfo()
        {
            Reset();
        }

        public uint[] GetTargetTeamList(uint teamDungeonID)
        {
            List<uint> list = new List<uint>();

            TeamDungeonTable table = TableManager.instance.GetTableItem<TeamDungeonTable>((int)teamDungeonID);
            if (null != table)
            {
                if (TeamDungeonTable.eType.DUNGEON == table.Type 
                    || TeamDungeonTable.eType.CityMonster == table.Type
                    || 1 == table.ShowIndependent)
                {
                    list.Add(teamDungeonID);
                }
                else
                {
                    Dictionary<int, object> kv = TableManager.instance.GetTable<ProtoTable.TeamDungeonTable>();
                    var iter = kv.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        TeamDungeonTable tb = iter.Current.Value as TeamDungeonTable;
                        if (null != tb)
                        {
                            if(tb.MenuID == teamDungeonID)
                            {
                                list.Add((uint)tb.ID);
                            }
                        }
                    }
                }
            }

            return list.ToArray();
        }

        public void Reset()
        {
            teamID = 0;
            teamName = "";
            chapterID = 0;
            hardType = 0xFF;
        }

        public void Debug()
        {
            Logger.LogProcessFormat("[组队数据] 搜索队伍 章节ID:{0} 难度类型:{1} 队伍名字:{2} 队伍ID:{3}",
                chapterID, hardType, teamName, teamID);
        }
    }

    class TeamCreateInfo
    {
        public byte DungeonType;
        public uint ChapterID;
        public byte HardType;
        public string TeamName;
        public byte MemberCount;
        public string Passward;

        public void Reset()
        {
            DungeonType = (byte)Protocol.TeamTargetType.Dungeon;
            ChapterID = 0;
            HardType = 0xFF;
            TeamName = GetRandomTeamName();
            MemberCount = 4;
            Passward = "";
        }

        public string GetRandomTeamName()
        {
            ProtoTable.TeamNameTable table1 = TableManager.GetInstance().GetTableItem<ProtoTable.TeamNameTable>(Random.Range(5001, 5660));
            ProtoTable.TeamNameTable table2 = TableManager.GetInstance().GetTableItem<ProtoTable.TeamNameTable>(Random.Range(1001, 2398));

            return string.Format("{0}{1}", table1.Name, table2.Name);
        }

        public void Debug()
        {
            Logger.LogProcessFormat("[组队数据] 创建队伍 章节ID:{0}  难度类型:{1} 队伍名字:{2} 成员数量:{3} 密码:{4}",
                ChapterID, HardType, TeamName, MemberCount, Passward);
        }
    }

    class TeamChangeInfo
    {
        public byte dungeonType;
        public uint chapterID;
        public uint dungeonID;
        public byte hardType;
        public string teamName;
        public byte memberCount;
        public bool hasPassward;
        public bool passwardChanged;
        public string passward;

        public string GetRandomTeamName()
        {
            ProtoTable.TeamNameTable table1 = TableManager.GetInstance().GetTableItem<ProtoTable.TeamNameTable>(Random.Range(5001, 5660));
            ProtoTable.TeamNameTable table2 = TableManager.GetInstance().GetTableItem<ProtoTable.TeamNameTable>(Random.Range(1001, 2398));

            return string.Format("{0}{1}", table1.Name, table2.Name);
        }

        public void Debug()
        {
            Logger.LogProcessFormat("[组队数据] 改变队伍信息 关卡ID:{0} 地下城ID:{1} 难度:{2} 队伍名字:{3} 成员数目:{4} 密码改变:{5} 密码:{6}",
                chapterID, dungeonID, hardType, teamName, memberCount, passwardChanged, passward);
        }
    }

    class TeamDungeon
    {
        public uint id;
        public bool isOpened;
        public byte maxHardType; // 开启的最高难度
        public string name;
    }

    class TeamChapter
    {
        public uint id;
        public bool isOpened;
        public string name;
        public string dataPath;
        public List<TeamDungeon> dungeons = new List<TeamDungeon>();
    }

    class NewTeamInviteList
    {
        public TeamBaseInfo baseinfo;
        public float fTimeCount;
    }

    class TeamDataManager : DataManager<TeamDataManager>
    {
        List<TeamChapter> m_arrChapters = new List<TeamChapter>();
        List<Team> m_teamList = new List<Team>();
        List<Team> m_teamListForTeamMainUI = new List<Team>(); // 这个是专门给组队主界面用的队伍列表数据
        List<NewTeamInviteList> m_InviteTeamList = new List<NewTeamInviteList>();
        TeamSearchInfo m_searchInfo = new TeamSearchInfo();
        TeamCreateInfo m_createInfo = new TeamCreateInfo();
        TeamChangeInfo m_changeInfo = new TeamChangeInfo();
        Team m_myTeam;
        int InviteTeamID = 0;
        bool bHasNewRequester = false;
        bool bAutoAgree = false;

        bool m_bNetBind = false;

        // 堕落深渊关卡本身是普通难度的，但是策划要求在界面显示成王者并且不显示其他难度
        public const int nDuoLuoDungeonID = 6502000;

        /// <summary>
        /// 自动退队是否开启倒计时
        /// </summary>
        public static bool AutomaticBackIsStart = false;

        public static bool bIsRefreshTime = false;//是否刷新自动退队倒计时UI

        public static uint StartServerTime = 0;

        public static int iAutoMaticBackRemainTime = 0;//剩余时间

        /// <summary>
        /// 退队提示时间数组
        /// </summary>
        public static int[] AutoMaticBackTimes = new int[] { 60, 45, 30, 15, 10, 5 };
        //public static int AutoMaticBackFirstTime = 0; // 记录第一次倒计时的时间，避免TeamMyFrame界面未打开时，就通知界面更新的问题
        private uint AutoMaticBackEndTime = 60;//退队总时间60秒
        private float AutoMaticBackTimer = 0;
        private int AutonMaticBackRemainTime = 0;//退队剩余时间
        private bool bStateChanged = false;
        #region public
        public TeamSearchInfo SearchInfo { get { return m_searchInfo; } }
        public TeamCreateInfo CreateInfo { get { return m_createInfo; } }
        public TeamChangeInfo ChangeInfo { get { return m_changeInfo; } }
        public ushort PageTeamCount { get; private set; }
        public ushort CurrentTeamIndex { get; private set; }
        public ushort MaxTeamCount { get; private set; }

        public uint nTeamGuildDungeonID = 0;
        private uint mTeamDungeonID = 0;
        public uint TeamDungeonID
        {
            get
            {
                return mTeamDungeonID;
            }

            set
            {
                Logger.LogProcessFormat("[组队数据] 当前目标TeamDungeonID {0} => {1}", mTeamDungeonID, value);
                mTeamDungeonID = value;
            }
        }
        public bool NotPopUpTeamList
        {
            set;
            get;
        }
        public int CreateTeamDungeonID
        {
            set;
            get;
        }

        // 是否是堕落深渊的组队地下城id
        public  bool IsDuoLuoTeamDungeonID(int iTeamDungeonID)
        {
            TeamDungeonTable table = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(iTeamDungeonID);
            if(table == null)
            {
                return false;
            }

            DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(table.DungeonID);
            if (dungeonTable == null)
            {
                return false;
            }

            if (dungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL || dungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL)
            {
                return true;
            }

            return false;
        }
        public int GetTargetTeamDungeonID(int iID,int iHard)
        {
            List<int> FliterFirstMenuList = new List<int>();
            Dictionary<int, List<int>> FliterSecondMenuDict = new Dictionary<int, List<int>>();
            FliterSecondMenuDict.Clear();
            FliterFirstMenuList = Utility.GetTeamDungeonMenuFliterList(ref FliterSecondMenuDict);
            if(iID == 1)
            {
                return 1;
            }
            if(FliterFirstMenuList.Contains(iID))
            {
                return 0;
            }
            if(iHard == -1)
            {
                return 0;
            }
            int iTeamDungeonID = iID;
            do
            {
                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(iID);
                if (teamDungeonTable == null)
                {
                    continue;
                }
                DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                if (table == null)
                {
                    if (teamDungeonTable.Type == TeamDungeonTable.eType.CityMonster)
                    {
                        iTeamDungeonID = iID;
                    }
                    continue;
                }
                foreach (var menu in FliterSecondMenuDict)
                {
                    List<int> data = menu.Value as List<int>;
                    if(data != null && data.Contains(iID))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            TeamDungeonTable teamDungeonTable1 = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(data[i]);
                            if (teamDungeonTable1 == null)
                            {
                                continue;
                            }
                            DungeonTable table1 = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable1.DungeonID);
                            if (table1 == null)
                            {
                                continue;
                            }
                            if (table1.Hard == (DungeonTable.eHard)iHard)
                            {
                                iTeamDungeonID = data[i];
                                break;
                            }
                        }
                        break;
                    }
                }               
            }
            while (false);
            return iTeamDungeonID;
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.TeamDataManager;
        }

        public override void Initialize()
        {
            nTeamGuildDungeonID = GetTeamGuildDungeonID();
            Clear();
            NetProcess.AddMsgHandler(WorldSyncTeamInfo.MsgID, _OnNetSyncMyTeamInfo);
        }

        public override void Clear()
        {
            TeamDungeonID = 0;
            _ClearTeamData();
            _UnBindNetMsg();
            NetProcess.RemoveMsgHandler(WorldSyncTeamInfo.MsgID, _OnNetSyncMyTeamInfo);
            AutomaticBackIsStart = false;
            AutoMaticBackEndTime = 60;
            AutoMaticBackTimer = 0;
            AutonMaticBackRemainTime = 0;
            iAutoMaticBackRemainTime = 0;
            bStateChanged = false;
            bIsRefreshTime = false;
        }

        public void CreateTeam(uint teamDungeonID)
        {
            WorldCreateTeam msg = new WorldCreateTeam();
            msg.target = teamDungeonID;

            TeamDungeonID = teamDungeonID;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        public void ChangeTeamInfo(TeamOptionOperType OperType, int param1)
        {
            WorldSetTeamOption msg = new WorldSetTeamOption();

            msg.type = (byte)OperType;
            msg.param1 = (uint)param1;
            msg.str = "";

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        public void SendSceneSaveOptionsReq(SaveOptionMask saveOptionMask, bool bSet)
        {
            SceneSaveOptionsReq msg = new SceneSaveOptionsReq();
            uint gameOptions = PlayerBaseData.GetInstance().gameOptions;
            if(bSet)
            {
                gameOptions |= (uint)saveOptionMask;
            }
            else
            {
                gameOptions &= ~((uint)saveOptionMask);
            }
            msg.options = gameOptions;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        public void JoinTeam(ulong teamLeaderID)
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS && scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossGuildBattle)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("跨服公会战场景下无法加入队伍");
                        return;
                    }
                }
            }

            Logger.LogProcessFormat("[组队数据] 加入队伍, 队长ID:{0}", teamLeaderID);

            SceneRequest msg = new SceneRequest();
            msg.type = (byte)RequestType.JoinTeam;
            msg.target = teamLeaderID;
            msg.param = 0;
            msg.targetName = "";
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldJoinTeamRes>(msgRet =>
            {
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    Logger.LogProcessFormat("[组队数据] 加入队伍成功, 队长ID:{0}", teamLeaderID);

                    //通知加入语音房间
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamGlobalJoined);

                    if (msgRet.inTeam == 0)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("队长已收到你的组队请求，请等待处理");
                    }
                    else
                    {
                    }
                }
            });
        }

        public bool HasPassward(ulong teamLeaderID)
        {
            Team team = m_teamList.Find(data => { return data.leaderInfo.id == teamLeaderID; });
            if (team != null)
            {
                //return team.hasPassword;
            }
            return false;
        }

        public void QuitTeam(ulong id)
        {
            if (HasTeam() == true)
            {
                Logger.LogProcessFormat("[组队数据] 离开队伍, 成员ID:{0}", id);

                WorldLeaveTeamReq msg = new WorldLeaveTeamReq();
                msg.id = id;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        public void KickTeamMember(ulong id)
        {
            if (IsTeamLeader() == true)
            {
                Logger.LogProcessFormat("[组队数据] 提出队伍, 成员ID:{0}", id);

                WorldLeaveTeamReq msg = new WorldLeaveTeamReq();
                msg.id = id;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        public void ChangeTeamLeader(ulong id)
        {
            if (IsTeamLeader() == true)
            {
                Logger.LogProcessFormat("[组队数据] 队长更换, 成员ID:{0}", id);

                WorldTransferTeammaster msg = new WorldTransferTeammaster();
                msg.id = id;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        public void ChangeTeamPosState(byte pos, byte opened)
        {
            if (IsTeamLeader() == true)
            {
                Logger.LogProcessFormat("change team pos state! pos:{0}  state:{1}", pos, opened);
                WorldTeamChangePosStatusReq msg = new WorldTeamChangePosStatusReq();
                msg.pos = pos;
                msg.open = opened;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        public void ChangeMainPlayerAssitState()
        {
            TeamMember main = GetTeamMemberByMemberID(PlayerBaseData.GetInstance().RoleID);

            if (null != main)
            {
                Logger.LogProcessFormat("[组队数据] 主玩家更改助战类型: {0} -> {1}", main.isAssist, !main.isAssist);

                WorldChangeAssistModeReq req = new WorldChangeAssistModeReq();
                req.isAssist = main.isAssist ? (byte)0 : (byte)1;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }
        }

        public void ChangePrepareState()
        {
            if (IsTeamLeader() == false)
            {
                Logger.LogProcessFormat("change prepare state");
                WorldTeamReadyReq msg = new WorldTeamReadyReq();
                msg.ready = (byte)(GetTeamMemberByMemberID(PlayerBaseData.GetInstance().RoleID).isPrepared ? 0 : 1);
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        public List<Team> GetTeamList()
        {
            return m_teamList;
        }

        public List<Team> GetTeamListForTeamMainUI()
        {
            return m_teamListForTeamMainUI;
        }
        public List<NewTeamInviteList> GetInviteTeamList()
        {
            return m_InviteTeamList;
        }

        public Team GetMyTeam()
        {
            return m_myTeam;
        }

        public bool HasTeam()
        {
            return m_myTeam != null;
        }

        public bool IsTeamLeader()
        {
            return IsTeamLeaderByRoleID(PlayerBaseData.GetInstance().RoleID);
        }

        public bool IsTeamLeaderByRoleID(ulong roleId)
        {
            return m_myTeam != null && m_myTeam.leaderInfo.id == roleId;
        }

        public bool HasNewRequester()
        {
            return bHasNewRequester;
        }

        public void ClearNewRequesterMark()
        {
            bHasNewRequester = false;
        }

        public bool IsTeammateFriend(ulong memberID)
        {
            if (!IsTeammateMainPlayer(memberID))
            {
                TeamMember member = GetTeamMemberByMemberID(memberID);

                if (null != member)
                {
                    RelationData relation = RelationDataManager.GetInstance().GetRelationByRoleID(member.id);
                    return null != relation && relation.IsFriend();
                }
            }

            return false;
        }

        public bool IsTeammateMaster(ulong memberID)
        {
            if (!IsTeammateMainPlayer(memberID))
            {
                TeamMember member = GetTeamMemberByMemberID(memberID);

                if (null != member)
                {
                    RelationData relation = RelationDataManager.GetInstance().GetRelationByRoleID(member.id);
                    return null != relation && relation.IsMater();
                }
            }

            return false;
        }

        public bool IsTeammateDisciple(ulong memberID)
        {
            if (!IsTeammateMainPlayer(memberID))
            {
                TeamMember member = GetTeamMemberByMemberID(memberID);

                if (null != member)
                {
                    RelationData relation = RelationDataManager.GetInstance().GetRelationByRoleID(member.id);
                    return null != relation && relation.IsDisciple();
                }
            }

            return false;
        }

        public bool IsTeammateGuild(ulong memberID)
        {
            if (!IsTeammateMainPlayer(memberID))
            {
                TeamMember main = GetTeamMemberByMemberID(PlayerBaseData.GetInstance().RoleID);
                TeamMember member = GetTeamMemberByMemberID(memberID);

                if (null != member && null != main)
                {
                    return main.guildid == member.guildid && main.guildid != 0;
                }
            }

            return false;
        }

        public bool IsTeammateHelpFight(ulong memberID)
        {
            TeamDungeonTable table = TableManager.instance.GetTableItem<TeamDungeonTable>((int)TeamDungeonID);
            if (null != table)
            {
                DungeonTable duntable = TableManager.instance.GetTableItem<DungeonTable>(table.DungeonID);

                if (null != duntable)
                {
                    if (duntable.SubType != DungeonTable.eSubType.S_TEAM_BOSS)
                    {
                        return false;
                    }
                }
            }

            TeamMember member = GetTeamMemberByMemberID(memberID);
            return null != member && member.isAssist;
        }

        public bool IsTeammateMainPlayer(ulong memberID)
        {
            TeamMember member = GetTeamMemberByMemberID(memberID);
            return null != member && member.id == PlayerBaseData.GetInstance().RoleID;
        }

        public eTeammateFlag GetTeammateFlag(ulong memberID)
        {
            eTeammateFlag relation = eTeammateFlag.None;

            TeamMember member = GetTeamMemberByMemberID(memberID);

            if (null != member)
            {
                if (IsTeammateGuild(memberID))
                {
                    Logger.LogProfileFormat("[组队数据] {0}是工会", memberID);
                    relation |= eTeammateFlag.Guild;
                }

                if (IsTeammateHelpFight(memberID))
                {
                    Logger.LogProfileFormat("[组队数据] {0}是助战", memberID);
                    relation |= eTeammateFlag.HelpFight;
                }

                if (IsTeammateFriend(memberID))
                {
                    Logger.LogProfileFormat("[组队数据] {0}是好友", memberID);
                    relation |= eTeammateFlag.Friend;
                }

                if (IsTeammateMaster(memberID))
                {
                    Logger.LogProfileFormat("[组队数据] {0}是师傅", memberID);
                    relation |= eTeammateFlag.Master;
                }

                if (IsTeammateDisciple(memberID))
                {
                    Logger.LogProfileFormat("[组队数据] {0}是徒弟", memberID);
                    relation |= eTeammateFlag.Disciple;
                }
            }

            return relation;
        }


        public TeamMember GetNewTeamMember()
        {
            if (m_myTeam != null)
            {
                for (int i = 0; i < m_myTeam.members.Length; ++i)
                {
                    if (m_myTeam.members[i].id <= 0)
                    {
                        return m_myTeam.members[i];
                    }
                }
            }

            return null;
        }

        public TeamMember GetTeamMemberByMemberID(ulong id)
        {
            if (m_myTeam != null)
            {
                for (int i = 0; i < m_myTeam.members.Length; ++i)
                {
                    if (m_myTeam.members[i].id == id)
                    {
                        return m_myTeam.members[i];
                    }
                }
            }

            return null;
        }

        public int GetMemberNum()
        {
            if (m_myTeam != null)
            {
                int iNum = 0;

                for (int i = 0; i < m_myTeam.members.Length; ++i)
                {
                    if (m_myTeam.members[i].id != 0)
                    {
                        iNum++;
                    }
                }

                return iNum;
            }

            return 0;
        }

        public void RequestSearchTeam(uint teamDungeonID)
        {
            TeamSearchInfo searchInfo = TeamDataManager.GetInstance().SearchInfo;
            searchInfo.Reset();

            TeamDungeonID = teamDungeonID;

            TeamDataManager.GetInstance().RequestTeamList(0);
        }

        public void RequestSearchTeam(uint teamDungeonID,List<int> IDs)
        {
            TeamSearchInfo searchInfo = TeamDataManager.GetInstance().SearchInfo;
            searchInfo.Reset();
            TeamDungeonID = teamDungeonID;
            if(IDs == null || IDs.Count == 0)
            {
                TeamDataManager.GetInstance().RequestTeamList(0);
                return;
            }
            WorldQueryTeamList msg = new WorldQueryTeamList();
            msg.teamId = (ushort)m_searchInfo.teamID;
            uint[] tempIds = new uint[IDs.Count];
            for(int i = 0;i < IDs.Count;i++)
            {
                tempIds[i] = (uint)IDs[i];
            }
            msg.targetList = tempIds;
            msg.startPos = 0;
            msg.num = (byte)PageTeamCount;
            msg.targetId = (uint)TeamDungeonID;          
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
//             Debug.LogWarningFormat("request teamlist,targetId = {0}", msg.targetId);
//             for (int i = 0; i < msg.targetList.Length; i++)
//             {
//                 Debug.LogWarningFormat("Id = {0}", msg.targetList[i]);
//             }
            return;
        }
        public void RequestTeamListForTeamMainUI(ushort startIndex,uint TeamDungeonID)
        {
            WorldQueryTeamList msg = new WorldQueryTeamList();
            msg.teamId = 0; 
            msg.targetList = m_searchInfo.GetTargetTeamList(TeamDungeonID);
            msg.startPos = startIndex;
            msg.num = (byte)PageTeamCount;
            msg.targetId = (uint)TeamDungeonID;
            msg.param = 1; // param为1的时候是请求用于显示在组队界面上的队伍数据            
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            return;
        }

        public void RequestTeamList(ushort startIndex)
        {
            WorldQueryTeamList msg = new WorldQueryTeamList();
            msg.teamId = (ushort)m_searchInfo.teamID;
            //msg.name = searchInfo.teamName;
            //msg.areaId = searchInfo.chapterID;
            msg.targetList = m_searchInfo.GetTargetTeamList(TeamDungeonID);
            //msg.hardType = searchInfo.hardType;
            msg.startPos = startIndex;
            msg.num = (byte)PageTeamCount;
            msg.targetId = (uint)TeamDungeonID;
            if(TeamDungeonID == 1)
            {
                msg.targetId = 0;
            }
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
//             Debug.LogFormat("request teamlist,targetId = {0}", msg.targetId);
//             for(int i = 0;i < msg.targetList.Length;i++)
//             {
//                 Debug.LogFormat("Id = {0}", msg.targetList[i]);
//             }
        }

        private void _onWorldQueryTeamListRet(MsgDATA data)
        {
            WorldQueryTeamListRet msgRet = new WorldQueryTeamListRet();
            msgRet.decode(data.bytes);

            if(GetMyTeam() != null) // 已经有队伍了就不用处理队伍列表消息了
            {
                return;
            }

            if(msgRet.param == 1)
            {
                m_teamListForTeamMainUI.Clear();               
                for (int i = 0; i < msgRet.teamList.Length; ++i)
                {
                    TeamBaseInfo baseInfo = msgRet.teamList[i];
                    Team team = new Team();
                    team.teamID = baseInfo.teamId;
                    team.teamDungeonID = baseInfo.target;
                    team.leaderInfo = baseInfo.masterInfo;
                    team.maxMemberCount = baseInfo.maxMemberNum;
                    team.currentMemberCount = baseInfo.memberNum;
                    m_teamListForTeamMainUI.Add(team);
                }                
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamListRequestSuccessForTeamMainUI);
            }
            else
            {
            m_teamList.Clear();

            TeamDungeonID = msgRet.targetId;
            if(TeamDungeonID == 0)
            {
                TeamDungeonID = 1;
            }

            for (int i = 0; i < msgRet.teamList.Length; ++i)
            {
                TeamBaseInfo baseInfo = msgRet.teamList[i];

                Team team = new Team();

                team.teamID = baseInfo.teamId;
                team.teamDungeonID = baseInfo.target;
                team.leaderInfo = baseInfo.masterInfo;
                team.maxMemberCount = baseInfo.maxMemberNum;
                team.currentMemberCount = baseInfo.memberNum;

                m_teamList.Add(team);
            }
                m_teamList.Sort((a, b) => 
                {
                    if(a == null || b == null)
                    {
                        return 0;
                    }
                    if(a.leaderInfo == null || b.leaderInfo == null)
                    {
                        return 0;
                    }
                    if(a.leaderInfo.playerLabelInfo == null || b.leaderInfo.playerLabelInfo == null)
                    {
                        return 0;
                    }
                    RelationData relationData = null;                    
                    bool aIsMyFriend = RelationDataManager.GetInstance().FindPlayerIsRelation(a.leaderInfo.id, ref relationData);
                    bool bIsMyFriend = RelationDataManager.GetInstance().FindPlayerIsRelation(b.leaderInfo.id, ref relationData);
                    bool aIsMyGuild = GuildDataManager.GetInstance().IsSameGuild(a.leaderInfo.playerLabelInfo.guildId);
                    bool bIsMyGuild = GuildDataManager.GetInstance().IsSameGuild(b.leaderInfo.playerLabelInfo.guildId);
                    if(a.leaderInfo.playerLabelInfo.returnStatus != b.leaderInfo.playerLabelInfo.returnStatus)
                    {
                    return b.leaderInfo.playerLabelInfo.returnStatus - a.leaderInfo.playerLabelInfo.returnStatus;
                    }
                    if(aIsMyFriend != bIsMyFriend)
                    {
                        return bIsMyFriend.CompareTo(aIsMyFriend);
                    }
                    return bIsMyGuild.CompareTo(aIsMyGuild);
                });

            MaxTeamCount = msgRet.maxNum;
            CurrentTeamIndex = msgRet.pos;

            Logger.LogProcessFormat("team list >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            for (int i = 0; i < m_teamList.Count; ++i)
            {
                m_teamList[i].Debug();
            }
            Logger.LogProcessFormat("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamListRequestSuccess);
            }
        }

        void _onWorldTeamInviteClearNotifyRet(MsgDATA data)
        {
            WorldTeamInviteClearNotify msgRet = new WorldTeamInviteClearNotify();
            msgRet.decode(data.bytes);

            for (int i = 0; i < m_InviteTeamList.Count; i++)
            {
                if (m_InviteTeamList[i].baseinfo.teamId == msgRet.teamId)
                {
                    m_InviteTeamList.RemoveAt(i);
               
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);

                    break;
                }
            }
        }

        void _onWorldTeamInviteSyncAttrRet(MsgDATA data)
        {
            WorldTeamInviteSyncAttr msgRet = new WorldTeamInviteSyncAttr();
            msgRet.decode(data.bytes);

            for (int i = 0; i < m_InviteTeamList.Count; i++)
            {
                if (m_InviteTeamList[i].baseinfo.teamId == msgRet.teamId)
                {
                    m_InviteTeamList[i].baseinfo.target = (uint)msgRet.target;

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);

                    break;
                }
            }
        }

        private void _onWorldNotifyTeamKick(MsgDATA data)
        {
            WorldNotifyTeamKick msgRet = new WorldNotifyTeamKick();
            msgRet.decode(data.bytes);
            
            if (msgRet.endTime != 0)
            {
                StartServerTime = TimeManager.GetInstance().GetServerTime();
                AutoMaticBackEndTime = (uint)(msgRet.endTime / 1000);
                AutonMaticBackRemainTime = (int)(AutoMaticBackEndTime - StartServerTime);
                iAutoMaticBackRemainTime = AutonMaticBackRemainTime;
            }
            else
            {
                AutomaticBackIsStart = false;
            }

            if (AutonMaticBackRemainTime <= 60)
            {
                AutomaticBackIsStart = msgRet.endTime != 0;
                UpdateStateChanged();
            }
        }

        private void UpdateStateChanged()
        {
            if (bStateChanged != AutomaticBackIsStart)
            {
                bStateChanged = AutomaticBackIsStart;

                AutoMaticBackTimer = 0;

                if (AutomaticBackIsStart)
                {
                    iAutoMaticBackRemainTime = AutonMaticBackRemainTime;
                }
                else
                {
                    //停止自动退队,界面倒计时消失
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamTimeChanged, 0);
                }
            }
        }

        public List<TeamChapter> GetChapterList()
        {
            if (m_arrChapters.Count <= 0)
            {
                TeamChapter chapterNone = new TeamChapter();
                chapterNone.id = 0;
                chapterNone.isOpened = true;
                chapterNone.name = TR.Value("team_chapter_nolimit");
                chapterNone.dataPath = "";
                m_arrChapters.Add(chapterNone);

                Dictionary<int, object> cityTable = TableManager.GetInstance().GetTable<ProtoTable.CitySceneTable>();
                var iter = cityTable.GetEnumerator();
                while (iter.MoveNext())
                {
                    ProtoTable.CitySceneTable data = iter.Current.Value as ProtoTable.CitySceneTable;
                    if (data.SceneType == ProtoTable.CitySceneTable.eSceneType.DUNGEON_ENTRY)
                    {
                        TeamChapter chapter = new TeamChapter();
                        chapter.id = (uint)data.ID;
                        chapter.isOpened = true;
                        chapter.name = data.Name;
                        // TODO 组队的时候要注意
                        chapter.dataPath = data.ChapterData[0];
                        m_arrChapters.Add(chapter);
                    }
                }
            }

            return m_arrChapters;
        }

        public List<TeamDungeon> GetDungeonList(uint chapterID)
        {
            TeamChapter chapter = m_arrChapters.Find((data) => { return data.id == chapterID; });
            if (chapter != null)
            {
                if (chapter.dungeons.Count <= 0)
                {
                    TeamDungeon dungeonNone = new TeamDungeon();
                    dungeonNone.id = 0;
                    dungeonNone.isOpened = true;
                    dungeonNone.maxHardType = (byte)ProtoTable.DungeonTable.eHard.KING;
                    dungeonNone.name = TR.Value("team_dungeon_nolimit");
                    chapter.dungeons.Add(dungeonNone);

                    DChapterData chapterData = AssetLoader.instance.LoadRes(chapter.dataPath).obj as DChapterData;
                    if (chapterData != null)
                    {
                        for (int i = 0; i < chapterData.chapterList.Length; ++i)
                        {
                            uint dungeonID = (uint)(chapterData.chapterList[i].dungeonID);
                            ProtoTable.DungeonTable table = TableManager.GetInstance().GetTableItem<ProtoTable.DungeonTable>((int)dungeonID);
                            if (table != null)
                            {
                                TeamDungeon dungeon = new TeamDungeon();
                                dungeon.id = dungeonID;
                                bool isOpen;
                                int hard;
                                if (BattleDataManager.GetInstance().ChapterInfo.IsOpen((int)dungeonID, out isOpen, out hard))
                                {
                                    dungeon.isOpened = isOpen;
                                    dungeon.maxHardType = (byte)hard;
                                }
                                else
                                {
                                    dungeon.isOpened = false;
                                    dungeon.maxHardType = (byte)ProtoTable.DungeonTable.eHard.NORMAL;
                                }
                                //                                 dungeon.isOpened = true;
                                //                                 dungeon.maxHardType = (byte)ProtoTable.DungeonTable.eHard.KING;
                                dungeon.name = table.Name;
                                chapter.dungeons.Add(dungeon);
                            }
                        }
                    }
                }
                return chapter.dungeons;
            }
            else
            {
                return null;
            }
        }

        public string GetTeamChapterName(uint chapterID)
        {
            List<TeamChapter> arrChapter = GetChapterList();
            if (arrChapter != null && arrChapter.Count > 0)
            {
                TeamChapter chapter = arrChapter.Find(data => { return data.id == chapterID; });
                if (chapter != null)
                {
                    return chapter.name;
                }
            }
            return "";
        }

        public string GetTeamDungeonName(uint chapterID, uint dungeonID)
        {
            if (chapterID == 0)
            {
                return TR.Value("team_dungeon_nolimit");
            }

            List<TeamDungeon> arrDungeon = GetDungeonList(chapterID);
            if (arrDungeon != null && arrDungeon.Count > 0)
            {
                TeamDungeon dungeon = arrDungeon.Find(data => { return data.id == dungeonID; });
                if (dungeon != null)
                {
                    return dungeon.name;
                }
            }
            return "";
        }

        public static string GetTeamDungeonName(uint teamDungeonID)
        {
            TeamDungeonTable table = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)teamDungeonID);
            if(table != null)
            {
                return table.Name;
            }
            return "";
        }

        // 根据地下城id获取组队地下城id
        public static int GetTeamDungeonIDByDungeonID(int dungeonID)
        {
            Dictionary<int, object> dicts = TableManager.instance.GetTable<TeamDungeonTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    TeamDungeonTable adt = iter.Current.Value as TeamDungeonTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.DungeonID == dungeonID)
                    {
                        return adt.ID;                       
                    }
                }
            }

            return 0;
        }

        private uint GetTeamGuildDungeonID()
        {
            List<int> FliterFirstMenuList = new List<int>();
            Dictionary<int, List<int>> FliterSecondMenuDict = new Dictionary<int, List<int>>();
            if(FliterFirstMenuList == null || FliterSecondMenuDict == null)
            {
                return 0;
            }
            FliterSecondMenuDict.Clear();
            FliterFirstMenuList = Utility.GetTeamDungeonMenuFliterList(ref FliterSecondMenuDict);
            for (int i = 0; i < FliterFirstMenuList.Count; i++)
            {
                if (FliterSecondMenuDict.ContainsKey(FliterFirstMenuList[i]))
                {
                    List<int> ids = FliterSecondMenuDict[FliterFirstMenuList[i]];
                    for (int j = 0; j < ids.Count; j++)
                    {
                        TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(ids[j]);
                        if (teamDungeonTable == null)
                        {
                            continue;
                        }
                        DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                        if (dungeonTable == null)
                        {
                            continue;
                        }
                        if (dungeonTable.SubType == DungeonTable.eSubType.S_GUILD_DUNGEON)
                        {
                            return (uint)FliterFirstMenuList[i];
                        }
                    }
                }
            }
            return 0;
        }
        public string GeDungeontHardName(byte hard)
        {
            if (hard == 0xFF)
            {
                return TR.Value("team_target_diff_all");
            }
            else
            {
                ProtoTable.DungeonTable.eHard ehard = (ProtoTable.DungeonTable.eHard)(hard);
                switch (ehard)
                {
                    case ProtoTable.DungeonTable.eHard.NORMAL:
                        {
                            return TR.Value("team_target_diff_normal");
                        }
                    case ProtoTable.DungeonTable.eHard.RISK:
                        {
                            return TR.Value("team_target_diff_risk");
                        }
                    case ProtoTable.DungeonTable.eHard.WARRIOR:
                        {
                            return TR.Value("team_target_diff_warrior");
                        }
                    case ProtoTable.DungeonTable.eHard.KING:
                        {
                            return TR.Value("team_target_diff_king");
                        }
                }
            }

            return "";
        }
        #endregion

        #region private
        void _BindNetMsg()
        {
            if (m_bNetBind == false)
            {
                //NetProcess.AddMsgHandler(SceneDungeonStartRes.MsgID, _OnSceneDungeonStartRes);
                //NetProcess.AddMsgHandler(WorldNotifyRaceStart.MsgID, _OnWorldNotifyRaceStart);
                NetProcess.AddMsgHandler(WorldCreateTeamRes.MsgID, _OnNetSyncCreateTeamSuccess);
                NetProcess.AddMsgHandler(WorldNotifyNewTeamMember.MsgID, _OnNetSyncAddTeamMember);
                NetProcess.AddMsgHandler(WorldNotifyMemberLeave.MsgID, _OnTeamMemberLeave);
                NetProcess.AddMsgHandler(WorldSyncTeammaster.MsgID, _OnTeamLeaderChanged);
                NetProcess.AddMsgHandler(WorldTeamChangePosStatusSync.MsgID, _OnNetSyncTeamPosState);
                NetProcess.AddMsgHandler(WorldSyncTeamMemberStatus.MsgID, _OnNetSyncMemberState);
                NetProcess.AddMsgHandler(WorldSyncTeamOption.MsgID, _OnNetSyncChangeTeamInfo);
                NetProcess.AddMsgHandler(WorldTeamRequestResultNotify.MsgID, _OnRequestResultNotify);
                NetProcess.AddMsgHandler(SceneSyncChat.MsgID, _OnNetSyncChat);
                NetProcess.AddMsgHandler(WorldTeamRaceVoteNotify.MsgID, _OnNetSyncTeamEnterDungeonVoteNotify);
                NetProcess.AddMsgHandler(WorldTeamInviteRes.MsgID, _OnNetSyncTeamInviteRes);
                NetProcess.AddMsgHandler(WorldTeamInviteNotify.MsgID, _OnNetSyncTeamInviteNotify);
                NetProcess.AddMsgHandler(WorldTeamNotifyNewRequester.MsgID, _OnNotifyNewRequester);
                NetProcess.AddMsgHandler(WorldTeamVoteChoiceNotify.MsgID, _OnNotifyVoteChoice);
                NetProcess.AddMsgHandler(SceneTeamMatchStartRes.MsgID, _OnTeamMatchStartRes);
                NetProcess.AddMsgHandler(WorldTeamMatchCancelRes.MsgID, _OnTeamMatchCancelRes);
                NetProcess.AddMsgHandler(WorldTeamMatchResultNotify.MsgID, _OnTeamMatchResultNotify);
                NetProcess.AddMsgHandler(WorldSyncTeamMemberProperty.MsgID, _onWorldSyncTeamMemberProperty);
                NetProcess.AddMsgHandler(WorldSyncTeammemberAvatar.MsgID, _onWorldSyncTeammemberAvatar);
                NetProcess.AddMsgHandler(WorldQueryTeamListRet.MsgID, _onWorldQueryTeamListRet);
                NetProcess.AddMsgHandler(WorldTeamInviteClearNotify.MsgID, _onWorldTeamInviteClearNotifyRet);
                NetProcess.AddMsgHandler(WorldTeamInviteSyncAttr.MsgID, _onWorldTeamInviteSyncAttrRet);
                NetProcess.AddMsgHandler(WorldNotifyTeamKick.MsgID, _onWorldNotifyTeamKick);

                m_bNetBind = true;
            }
        }

        void _UnBindNetMsg()
        {
            //NetProcess.RemoveMsgHandler(SceneDungeonStartRes.MsgID, _OnSceneDungeonStartRes);
            //NetProcess.RemoveMsgHandler(WorldNotifyRaceStart.MsgID, _OnWorldNotifyRaceStart);
            NetProcess.RemoveMsgHandler(WorldCreateTeamRes.MsgID, _OnNetSyncCreateTeamSuccess);
            NetProcess.RemoveMsgHandler(WorldNotifyNewTeamMember.MsgID, _OnNetSyncAddTeamMember);
            NetProcess.RemoveMsgHandler(WorldNotifyMemberLeave.MsgID, _OnTeamMemberLeave);
            NetProcess.RemoveMsgHandler(WorldSyncTeammaster.MsgID, _OnTeamLeaderChanged);
            NetProcess.RemoveMsgHandler(WorldTeamChangePosStatusSync.MsgID, _OnNetSyncTeamPosState);
            NetProcess.RemoveMsgHandler(WorldSyncTeamMemberStatus.MsgID, _OnNetSyncMemberState);
            NetProcess.RemoveMsgHandler(WorldSyncTeamOption.MsgID, _OnNetSyncChangeTeamInfo);
            NetProcess.RemoveMsgHandler(WorldTeamRequestResultNotify.MsgID, _OnRequestResultNotify);
            NetProcess.RemoveMsgHandler(SceneSyncChat.MsgID, _OnNetSyncChat);
            NetProcess.RemoveMsgHandler(WorldTeamRaceVoteNotify.MsgID, _OnNetSyncTeamEnterDungeonVoteNotify);
            NetProcess.RemoveMsgHandler(WorldTeamInviteRes.MsgID, _OnNetSyncTeamInviteRes);
            NetProcess.RemoveMsgHandler(WorldTeamInviteNotify.MsgID, _OnNetSyncTeamInviteNotify);
            NetProcess.RemoveMsgHandler(WorldTeamNotifyNewRequester.MsgID, _OnNotifyNewRequester);
            NetProcess.RemoveMsgHandler(WorldTeamVoteChoiceNotify.MsgID, _OnNotifyVoteChoice);
            NetProcess.RemoveMsgHandler(SceneTeamMatchStartRes.MsgID, _OnTeamMatchStartRes);
            NetProcess.RemoveMsgHandler(WorldTeamMatchCancelRes.MsgID, _OnTeamMatchCancelRes);
            NetProcess.RemoveMsgHandler(WorldTeamMatchResultNotify.MsgID, _OnTeamMatchResultNotify);
            NetProcess.RemoveMsgHandler(WorldSyncTeamMemberProperty.MsgID, _onWorldSyncTeamMemberProperty);
            NetProcess.RemoveMsgHandler(WorldSyncTeammemberAvatar.MsgID, _onWorldSyncTeammemberAvatar);
            NetProcess.RemoveMsgHandler(WorldQueryTeamListRet.MsgID, _onWorldQueryTeamListRet);
            NetProcess.RemoveMsgHandler(WorldTeamInviteClearNotify.MsgID, _onWorldTeamInviteClearNotifyRet);
            NetProcess.RemoveMsgHandler(WorldTeamInviteSyncAttr.MsgID, _onWorldTeamInviteSyncAttrRet);
            NetProcess.RemoveMsgHandler(WorldNotifyTeamKick.MsgID, _onWorldNotifyTeamKick);

            m_bNetBind = false;
        }



        void _OnNetSyncMyTeamInfo(MsgDATA msg)
        {
            _BindNetMsg();

            WorldSyncTeamInfo msgData = new WorldSyncTeamInfo();
            msgData.decode(msg.bytes);

            if (msgData.id <= 0)
            {
                return;
            }

            m_myTeam = new Team();

            m_myTeam.teamID = msgData.id;
            m_myTeam.leaderInfo = new TeammemberBaseInfo();
            m_myTeam.leaderInfo.id = msgData.master;
            m_myTeam.autoAgree = msgData.autoAgree;

            TeamDungeonID = msgData.target;
            bAutoAgree = false;

            IsAutoReturnFormHell = ((msgData.options & (uint)TeamOption.HellAutoClose) == (uint)TeamOption.HellAutoClose);
            for (int i = 0; i < m_myTeam.members.Length; ++i)
            {
                TeamMember target = new TeamMember();
                target.ClearPlayerInfo();

                m_myTeam.members[i] = target;
            }

            if (msgData.members.Length > m_myTeam.members.Length)
            {
                string log = string.Format("队伍成员数量<{0}>超过设计的数量<{1}>,检查服务端数据，队伍id{2}, 队伍目标id{3} 队伍成员:\n",
                    msgData.members.Length, m_myTeam.members.Length - 1, msgData.id, msgData.target);
                for (int j = 0; j < msgData.members.Length; ++j)
                {
                    var member = msgData.members[j];
                    if (member != null)
                    {
                        log += string.Format("id:{0}, 名字{1}\n", member.id, member.name);
                    }
                    else
                    {
                        log += string.Format("下标为{0}的数组成员为空", j);
                    }
                }
            }

            int iIndex = 0;
            for (int i = 0; i < msgData.members.Length; ++i)
            {
                if (msgData.members[i].id <= 0)
                {
                    continue;
                }

                if (i >= m_myTeam.members.Length)
                {
                    continue;
                }

                _ParseMemberData(msgData.members[i], m_myTeam.members[iIndex]);
                iIndex++;
            }

            _UpdateMemberCountInfo();

            Logger.LogProcessFormat("sync team info...");
            _DebugTeamData();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateGameOptions);
        }

        void _OnNetSyncChangeTeamInfo(MsgDATA msg)
        {
            if (m_myTeam != null)
            {
                WorldSyncTeamOption msgData = new WorldSyncTeamOption();
                msgData.decode(msg.bytes);

                if (msgData.type == (byte)TeamOptionOperType.Target)
                {
                    TeamDungeonID = msgData.param1;
                }
                else if (msgData.type == (byte)TeamOptionOperType.AutoAgree)
                {
                    m_myTeam.autoAgree = msgData.param1;
                }
                else if(msgData.type == (byte)TeamOptionOperType.HellAutoClose)
                {
                    IsAutoReturnFormHell = msgData.param1 == (uint)TeamOption.HellAutoClose;
                }

                Logger.LogProcessFormat("sync ChangeTeamInfo:");
                _DebugTeamData();

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamInfoUpdateSuccess);
            }
        }

        void _onWorldSyncTeammemberAvatar(MsgDATA msg)
        {
            WorldSyncTeammemberAvatar msgData = new WorldSyncTeammemberAvatar();
            msgData.decode(msg.bytes);

            TeamMember member = GetTeamMemberByMemberID(msgData.memberId);

            if (null != member)
            {
                Logger.LogProcessFormat("[组队] 更新Avatar {0}", msgData.memberId);

                member.avatarInfo = msgData.avatar;

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamMemberStateChanged);
            }
        }


        void _onWorldSyncTeamMemberProperty(MsgDATA msg)
        {
            WorldSyncTeamMemberProperty msgData = new WorldSyncTeamMemberProperty();

            msgData.decode(msg.bytes);

            TeamMember member = GetTeamMemberByMemberID(msgData.memberId);

            if (null != member)
            {

                TeamMemberProperty prop = (TeamMemberProperty)msgData.type;
                Logger.LogProcessFormat("[组队] 类型 {0}, 值 {1}", prop, msgData.value);
                switch (prop)
                {
                    case TeamMemberProperty.GuildID:
                        member.guildid = (ulong)msgData.value;
                        break;
                    case TeamMemberProperty.Level:
                        member.level = (ushort)msgData.value;
                        break;
                    case TeamMemberProperty.VipLevel:
                        member.viplevel = (ushort)msgData.value;
                        break;
                    case TeamMemberProperty.Occu:
                        member.occu = (byte)msgData.value;
                        break;
                    case TeamMemberProperty.RemainTimes:
                        member.dungeonLeftCount = (int)msgData.value;
                        break;
                    case TeamMemberProperty.StatusMask:
                        _updateTeammateStatus(member, (byte)msgData.value);
                        break;
                    case TeamMemberProperty.ResistMagic:
                        member.resistMagicValue = (uint) msgData.value;
                        break;
                    case TeamMemberProperty.PlayerLabelInfo:
                        member.playerLabelInfo = msgData.playerLabelInfo;
                        break;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamMemberStateChanged);
            }
        }

        void _OnRequestResultNotify(MsgDATA msg)
        {
            WorldTeamRequestResultNotify msgData = new WorldTeamRequestResultNotify();
            msgData.decode(msg.bytes);

            if (msgData.agree == 1)
            {            
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamJoinSuccess);

                SystemNotifyManager.SystemNotify(9205);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateGameOptions);
            }
            else
            {
            }
        }

        void _OnNetSyncTeamEnterDungeonVoteNotify(MsgDATA msg)
        {
            WorldTeamRaceVoteNotify msgData = new WorldTeamRaceVoteNotify();
            msgData.decode(msg.bytes);

            DungeonTable data = TableManager.GetInstance().GetTableItem<DungeonTable>((int)msgData.dungeonId);
            if (data == null)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK("组队队长发起进入副本ID错误");
            }

            DeviceVibrateManager.GetInstance().TriggerDeviceVibrateByType(VibrateSwitchType.Team);

            // 加个控制开关，这个开关不经服务器控制的
//             var ControlData = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(50);
//             if(ControlData != null && ControlData.Open)
//             {
//                 //ClientSystemTown targetSystem = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
//                 //if (targetSystem != null && ClientSystemManager.GetInstance().isSwitchSystemLoading)
// 
//                 if (ClientSystemManager.GetInstance().isSwitchSystemLoading)
//                 {
//                     WorldTeamReportVoteChoice msgRes = new WorldTeamReportVoteChoice();
//                     msgRes.agree = 0;
// 
//                     NetManager netMgr = NetManager.Instance();
//                     netMgr.SendCommand(ServerType.GATE_SERVER, msgRes);
// 
//                     SystemNotifyManager.SysNotifyFloatingEffect("当前正在切换系统,无法接受进入副本");
// 
//                     return;
//                 }
//             }

            ClientSystemManager.GetInstance().OpenFrame<TeamVoteEnterDungeonFrame>(FrameLayer.HorseLamp, (int)msgData.dungeonId);
        }

        void _OnNetSyncTeamInviteRes(MsgDATA msg)
        {
            WorldTeamInviteRes msgData = new WorldTeamInviteRes();
            msgData.decode(msg.bytes);

            if (msgData.result != 0)
            {
                if ((int)Protocol.ProtoErrorCode.TEAM_INVITE_FREQUENTLY != msgData.result)
                {
                    SystemNotifyManager.SystemNotify((int)msgData.result);
                }
                return;
            }

            SystemNotifyManager.SysNotifyFloatingEffect("邀请组队已发送");
        }

        void _OnNetSyncTeamInviteNotify(MsgDATA msg)
        {
            WorldTeamInviteNotify msgData = new WorldTeamInviteNotify();
            msgData.decode(msg.bytes);

            if (HasTeam())
            {
                return;
            }

            TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)msgData.info.target);
            if (data == null)
            {
                return;
            }

            bool bFind = false;
            bool bNeedNotice = false;

            for (int i = 0; i < m_InviteTeamList.Count; i++)
            {
                if (m_InviteTeamList[i].baseinfo.masterInfo.id == msgData.info.masterInfo.id)
                {
                    if (m_InviteTeamList[i].baseinfo.target != msgData.info.target)
                    {
                        bNeedNotice = true;
                    }

                    m_InviteTeamList[i].baseinfo = msgData.info;
                    bFind = true;

                    break;
                }
            }

            if (!bFind)
            {
                NewTeamInviteList newmem = new NewTeamInviteList();
                newmem.baseinfo = msgData.info;
                newmem.fTimeCount = 0.0f;

                m_InviteTeamList.Add(newmem);
                bNeedNotice = true;
            }

            if (bNeedNotice)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);
                //ClientSystemManager.GetInstance().OpenFrame<TeamBeInvitedFrame>();
            }

            //             InviteTeamID = msgData.info.teamId;
            //            
            //             object[] args = new object[2];
            //             args[0] = msgData.info.masterInfo.name;
            //             args[1] = data.Name;
            // 
            // SystemNotifyManager.SystemNotify(1102, _AgreeJoinTeam, _RejectJoinTeam, 30.0f, args);
        }

        void _OnNotifyNewRequester(MsgDATA msg)
        {
            WorldTeamNotifyNewRequester msgData = new WorldTeamNotifyNewRequester();
            msgData.decode(msg.bytes);

            bHasNewRequester = true;

            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.TeamNewRequester);


            NewMessageNoticeManager.GetInstance().AddNewMessageNoticeWhenNoExist("TeamNewRequest", null, data =>
            {
                if (!ClientSystemManager.instance.IsFrameOpen<TeamListFrame>())
                {
                    TeamListFrame.TryOpenTeamListOrTeamMyFrame();
                    //ClientSystemManager.instance.OpenFrame<TeamListFrame>(FrameLayer.Middle);
                }

                RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.TeamNewRequester);

                NewMessageNoticeManager.GetInstance().RemoveNewMessageNotice(data);
            });
        }

        void _OnNotifyVoteChoice(MsgDATA msg)
        {
            WorldTeamVoteChoiceNotify msgData = new WorldTeamVoteChoiceNotify();
            msgData.decode(msg.bytes);

            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamVoteEnterDungeonFrame>())
            {
                TeamVoteEnterDungeonFrame Voteframe = ClientSystemManager.GetInstance().GetFrame(typeof(TeamVoteEnterDungeonFrame)) as TeamVoteEnterDungeonFrame;
                Voteframe.UpdateMemVoteState(msgData);
            }
        }

        void _OnTeamMatchStartRes(MsgDATA msg)
        {
            SceneTeamMatchStartRes msgData = new SceneTeamMatchStartRes();
            msgData.decode(msg.bytes);

            if (msgData.result != 0)
            {
                if ((ProtoErrorCode)msgData.result == ProtoErrorCode.TEAM_MATCH_START_FAILED)
                {
                    return;
                }

                SystemNotifyManager.SystemNotify((int)msgData.result);

                if ((ProtoErrorCode)msgData.result != ProtoErrorCode.TEAM_MATCH_START_IN_MATCHING)
                {
                    return;
                }

                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamMatchStartSuccess);
        }

        void _OnTeamMatchCancelRes(MsgDATA msg)
        {
            WorldTeamMatchCancelRes msgData = new WorldTeamMatchCancelRes();
            msgData.decode(msg.bytes);

            if (msgData.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);

                if ((ProtoErrorCode)msgData.result != ProtoErrorCode.TEAM_MATCH_CANCEL_NOT_IN_MATCHING)
                {
                    return;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamMatchCancelSuccess);
        }

        void _OnTeamMatchResultNotify(MsgDATA msg)
        {
            WorldTeamMatchResultNotify msgData = new WorldTeamMatchResultNotify();
            msgData.decode(msg.bytes);

            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamMatchPlayersFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<TeamMatchPlayersFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<TeamMatchPlayersFrame>(FrameLayer.Middle, msgData);
        }

        //         void _AgreeJoinTeam()
        //         {
        //             SceneReply msg = new SceneReply();
        //             msg.type = (byte)RequestType.InviteTeam;
        //             msg.requester = (uint)InviteTeamID;
        //             msg.result = 1;
        // 
        //             NetManager netMgr = NetManager.Instance();
        //             netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        //         }

        //         void _RejectJoinTeam()
        //         {
        //             SceneReply msg = new SceneReply();
        //             msg.type = (byte)RequestType.InviteTeam;
        //             msg.requester = (uint)InviteTeamID;
        //             msg.result = 0;
        // 
        //             NetManager netMgr = NetManager.Instance();
        //             netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        //         }

        void _OnNetSyncCreateTeamSuccess(MsgDATA msg)
        {
            WorldCreateTeamRes msgData = new WorldCreateTeamRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamCreateSuccess);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateGameOptions);
            }
        }

        void _OnNetSyncAddTeamMember(MsgDATA msg)
        {
            WorldNotifyNewTeamMember msgData = new WorldNotifyNewTeamMember();
            msgData.decode(msg.bytes);

            TeamMember target = GetNewTeamMember();
            _ParseMemberData(msgData.info, target);

            Logger.LogProcessFormat("sync add member:");
            _DebugMemberData(target);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamAddMemberSuccess);
        }

        void _OnTeamMemberLeave(MsgDATA msg)
        {
            WorldNotifyMemberLeave msgLevel = new WorldNotifyMemberLeave();
            msgLevel.decode(msg.bytes);

            if (msgLevel.id == PlayerBaseData.GetInstance().RoleID)
            {
                _ClearTeamData();

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamGlobalLeaved);
                // 自己离开时更新下 gameOptions相关的UI
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateGameOptions);
            }
            else
            {
                List<TeamMember> NewmembersList = new List<TeamMember>();

                for (int i = 0; i < m_myTeam.members.Length; ++i)
                {
                    if (m_myTeam.members[i].id != msgLevel.id)
                    {
                        TeamMember data = new TeamMember();

                        _ParseMemberData(m_myTeam.members[i], data);

                        NewmembersList.Add(data);
                    }
                }

                for (int i = 0; i < m_myTeam.members.Length; i++)
                {
                    m_myTeam.members[i].ClearPlayerInfo();
                }

                for (int i = 0; i < NewmembersList.Count; i++)
                {
                    m_myTeam.members[i] = NewmembersList[i];
                    _ParseMemberData(NewmembersList[i], m_myTeam.members[i]);
                }
            }

            Logger.LogProcessFormat("sync member leave:");
            _DebugTeamData();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamRemoveMemberSuccess);
        }

        void _OnNetSyncTeamPosState(MsgDATA msg)
        {
            WorldTeamChangePosStatusSync msgData = new WorldTeamChangePosStatusSync();
            msgData.decode(msg.bytes);

            _UpdateMemberCountInfo();

            Logger.LogProcessFormat("sync pos state:");
            _DebugTeamData();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamPosStateChanged);
        }

        void _OnNetSyncMemberState(MsgDATA msg)
        {
            WorldSyncTeamMemberStatus msgData = new WorldSyncTeamMemberStatus();
            msgData.decode(msg.bytes);

            TeamMember member = GetTeamMemberByMemberID(msgData.id);
            if (member != null)
            {
                _updateTeammateStatus(member, msgData.statusMask);
            }

            Logger.LogProcessFormat("sync member state:");
            _DebugMemberData(member);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamMemberStateChanged);
        }

        void _OnNetSyncChat(MsgDATA msg)
        {
            SceneSyncChat msgData = new SceneSyncChat();
            msgData.decode(msg.bytes);

            if (msgData.channel == (byte)ChanelType.CHAT_CHANNEL_TEAM_SPECIAL)
            {
                //msgData.objid         -> 发送者ID
                //msgData.receiverId    -> 发送表情ID
                if (msgData.objid != PlayerBaseData.GetInstance().RoleID)
                {
                    int faceID = (int)msgData.receiverId;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNotifyChat, msgData.objid, faceID);
                }
            }
            else if (msgData.channel == (byte)ChanelType.CHAT_CHANNEL_TEAM)
            {
                //if (msgData.objid != PlayerBaseData.GetInstance().RoleID)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNotifyChatMsg, msgData.objid, msgData.word);
                }
            }
        }

        public int DiffHard
        {
            get;
            set;
        }
        public bool IsAutoAgree
        {
            get { return bAutoAgree; }
            set { bAutoAgree = value; }
        }       
        public bool IsAutoReturnFormHell
        {
            get;
            private set;
        }
        public bool IsNotCostFatigueInEliteDungeon
        {
            get
            {
                if(GetMemberNum() == 0)
                {
                    return false;
                }

                uint gameOptions = PlayerBaseData.GetInstance().gameOptions;
                return (gameOptions & (int)SaveOptionMask.SOM_NOT_COUSUME_EBERGY) == (int)SaveOptionMask.SOM_NOT_COUSUME_EBERGY;
            }
        }
        Dictionary<string, DiffInfo> secteamDungeons = new Dictionary<string, DiffInfo>();
        public Dictionary<string, DiffInfo> GetDiffInfo()
        {
            return secteamDungeons;
        }
        void _ClearTeamData()
        {
            m_teamList.Clear();
            m_InviteTeamList.Clear();
            PageTeamCount = 40;
            CurrentTeamIndex = 0;
            MaxTeamCount = 0;
            m_myTeam = null;
            m_searchInfo = new TeamSearchInfo();
            m_createInfo = new TeamCreateInfo();
            m_changeInfo = new TeamChangeInfo();
            m_arrChapters.Clear();
            InviteTeamID = 0;
            bHasNewRequester = false;
            bAutoAgree = false;
            IsAutoReturnFormHell = false;
            DiffHard = -1;
            secteamDungeons.Clear();
        }

        void _OnTeamLeaderChanged(MsgDATA msg)
        {
            WorldSyncTeammaster msgLeader = new WorldSyncTeammaster();
            msgLeader.decode(msg.bytes);

            if (m_myTeam != null)
            {
                m_myTeam.leaderInfo.id = msgLeader.master;

                Logger.LogProcessFormat("sync team leader changed...");
                _DebugTeamData();

                for (int i = 0; i < m_myTeam.members.Length; i++)
                {
                    if (m_myTeam.members[i].id == msgLeader.master)
                    {
                        m_myTeam.leaderInfo.name = m_myTeam.members[i].name;
                        m_myTeam.leaderInfo.level = m_myTeam.members[i].level;
                        m_myTeam.leaderInfo.occu = m_myTeam.members[i].occu;
                        break;
                    }
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamChangeLeaderSuccess);

                if (msgLeader.master != PlayerBaseData.GetInstance().RoleID)
                {
                    bHasNewRequester = false;
                }
            }
        }

        private bool _isStatusMaskValid(byte statusMask, TeamMemberStatusMask mask)
        {
            return (statusMask & (byte)mask) != 0;
        }

        private void _updateTeammateStatus(TeamMember member, byte status)
        {
            member.isOnline = _isStatusMaskValid(status, TeamMemberStatusMask.Online);
            member.isPrepared = _isStatusMaskValid(status, TeamMemberStatusMask.Ready);
            member.isAssist = _isStatusMaskValid(status, TeamMemberStatusMask.Assist);
            member.isBuzy = _isStatusMaskValid(status, TeamMemberStatusMask.Racing);

            Logger.LogProcessFormat(
                    "[组队数据] 更新队员{0}状态，在线{1}, 准备{2}, 助战{3}",
                    member.id,
                    member.isOnline,
                    member.isPrepared,
                    member.isAssist);
        }

        void _ParseMemberData(TeammemberInfo source, TeamMember target)
        {
            if (target != null && source != null)
            {
                target.id = source.id;
                target.name = source.name;
                target.occu = source.occu;
                target.level = source.level;
                target.avatarInfo = source.avatar;
                target.guildid = source.guildId;
                target.viplevel = source.vipLevel;
                target.resistMagicValue = source.resistMagic;
                target.playerLabelInfo = source.playerLabelInfo;
                _updateTeammateStatus(target, source.statusMask);
            }
        }

        void _ParseMemberData(TeamMember source, TeamMember target)
        {
            if (target != null && source != null)
            {
                target.id = source.id;
                target.name = source.name;
                target.occu = source.occu;
                target.level = source.level;
                target.viplevel = source.viplevel;
                target.guildid = source.guildid;

                target.isOnline = source.isOnline;
                target.isPrepared = source.isPrepared;
                target.isAssist = source.isAssist;

                target.avatarInfo = source.avatarInfo;
                target.resistMagicValue = source.resistMagicValue;
                target.playerLabelInfo = source.playerLabelInfo;
            }
        }

        void _UpdateMemberCountInfo()
        {
            m_myTeam.maxMemberCount = 0;
            m_myTeam.currentMemberCount = 0;
            for (int i = 0; i < m_myTeam.members.Length; ++i)
            {
                TeamMember member = m_myTeam.members[i];
                //if (member.isOpened)
                {
                    m_myTeam.maxMemberCount++;
                    if (member.id > 0)
                    {
                        m_myTeam.currentMemberCount++;
                    }
                }
            }
        }

        public void TeamInviteOtherPlayer(ulong RoleId)
        {
            if(!HasTeam())
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("no_team_can_not_invite"));
                return;
            }

            SceneRequest req = new SceneRequest();

            req.type = (byte)RequestType.InviteTeam;
            req.target = RoleId;
            req.targetName = "";
            req.param = 0;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void JoinOtherPlayerTeam(ulong roleID)
        {
            JoinTeam(roleID);
        }

        void _DebugTeamData()
        {
            if (m_myTeam == null)
            {
                Logger.LogProcessFormat("my team data is null!!");
            }
            else
            {
                m_myTeam.Debug();
            }
        }

        void _DebugMemberData(TeamMember data)
        {
            if (data != null)
            {
                data.Debug();
            }
            else
            {
                Logger.LogProcessFormat("member data is null!");
            }
        }
        #endregion

        //[EnterGameMessageHandle(WorldSyncTeamInfo.MsgID)]
        void _OnInitMyTeamInfo(MsgDATA msg)
        {
            if (msg != null)
            {
                _OnNetSyncMyTeamInfo(msg);
            }
        }

        public void OnUpdate(float timeElapsed)
        {
            for(int i = 0; i < m_InviteTeamList.Count; i++)
            {
                if(m_InviteTeamList[i].fTimeCount > 30.0f)
                {
                    m_InviteTeamList.RemoveAt(i);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);

                    i--;
                }
                else
                {
                    m_InviteTeamList[i].fTimeCount += timeElapsed;
                }
            }

            if (AutomaticBackIsStart)
            {
                Team myTeam = GetMyTeam();

                if (myTeam == null)
                {
                    AutomaticBackIsStart = false;
                    UpdateStateChanged();
                    return;
                }

                AutoMaticBackTimer += timeElapsed;
                if (AutoMaticBackTimer >= 0.2f)
                {
                    AutoMaticBackTimer = 0;
                    StartServerTime = TimeManager.GetInstance().GetServerTime();
                }
                
                iAutoMaticBackRemainTime = (int)(AutoMaticBackEndTime - StartServerTime);

                SystemNotify(iAutoMaticBackRemainTime);
            }
        }

        public void SystemNotify(int time)
        {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null || myTeam.members == null || myTeam.leaderInfo == null)
            {
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamTimeChanged, time);

            //队长已退队
            if (time <= 0)
            {
                string content = string.Empty;
                //队员
                if (myTeam.leaderInfo.id != PlayerBaseData.GetInstance().RoleID)
                {
                    content = TR.Value("teamdissolve_member_tip");
                }
                else
                {
                    //队长
                    content = TR.Value("teamdissolve_leader_tip");
                }
                
                SystemNotifyManager.SysNotifyTextAnimation(content);
                AutomaticBackIsStart = false;
                UpdateStateChanged();
                return;
            }
            
            for (int i = 0; i < AutoMaticBackTimes.Length; i++)
            {
                if (AutoMaticBackTimes[i] != time)
                {
                    continue;
                }

                string content = string.Empty;
                //队员
                if (myTeam.leaderInfo.id != PlayerBaseData.GetInstance().RoleID)
                {
                    content = TR.Value("teamdissolve_member_time_tip", time);
                }
                else
                {
                    //队长
                    content = TR.Value("teamdissolve_leader_time_tip", time);
                }

                SystemNotifyManager.SysNotifyTextAnimation(content);

                //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamTimeChanged, time);
            }
        }
    }
}
