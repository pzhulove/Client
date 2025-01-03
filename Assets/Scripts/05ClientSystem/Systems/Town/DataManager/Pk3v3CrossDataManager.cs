using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameClient
{
    public class Pk3v3CrossRoomSettingData
    {
        public bool bSetMinLv;
        public bool bSetMinRankLv;

        public int MinLv;
        public int MinRankLv;

        public void DefaultInit()
        {
            bSetMinLv = false;
            bSetMinRankLv = false;

            MinLv = Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel);
            MinRankLv = SeasonDataManager.GetInstance().GetMinRankID();
        }
    }

    public class Pk3v3CrossDataManager : DataManager<Pk3v3CrossDataManager>
    {
        public static uint MAX_PK_COUNT = 5;
        bool m_bNetBind = false;

        Dictionary<RoomType, Pk3v3RoomSettingData> roomSettingData = new Dictionary<RoomType, Pk3v3RoomSettingData>();

        public bool bHasPassword = false;
        public string PassWord = "";

        WorldSyncRoomSwapSlotInfo SwapSlotInfo = new WorldSyncRoomSwapSlotInfo();
        float fChangePosLastTime = 0.0f;
        float fAddUpTime = 0.0f;
        float fChangePosLocationTime = 0.0f;
        public int iInt = 0;

        bool m_bNotify = false;
        public bool isNotify { get { return m_bNotify; } set { m_bNotify = value; } }
        
        private int simpleInviteLastTime = -1;//用于判断现在能不能进行一键邀请的剩余时间
        public int SimpleInviteLastTime
        {
            set
            {
                simpleInviteLastTime = value;
            }
            get
            {
                return simpleInviteLastTime;
            }
        }
        public Dictionary<RoomType, Pk3v3RoomSettingData> RoomSettingData
        {
            get
            {
                return roomSettingData;
            }

            set
            {
                if (roomSettingData != value)
                {
                    roomSettingData = value;
                }
            }
        }

        static RoomInfo roomInfo = new RoomInfo();
        List<WorldSyncRoomInviteInfo> InviteRoomList = new List<WorldSyncRoomInviteInfo>();

        public class TeamMember
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
            public UInt32 playerSeasonLevel;

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
            }

            public void Debug()
            {
                //Logger.LogProcessFormat("[组队数据] 成员ID:{0} 是否准备:{1} 是否在线:{2} 名字:{3} 等级:{4} 职业:{5}",
                     //id, isPrepared, isOnline, name, level, occu);
            }
        }

        public class Team
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
                //Logger.LogProcessFormat("[组队数据] 开始输出组队数据");
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

                //Logger.LogProcessFormat("[组队数据] 结束输出组队数据");
            }
        }

        Team m_myTeam = null;


        public class ScoreListItem
        {
            public UInt64 nPlayerID;
            public string strPlayerName;
            public UInt64 nPlayerScore;
            public string strServerName;
            public UInt16 nRank;
        }

        List<ScoreListItem> m_arrScoreList = null;

        public List<ScoreListItem> GetScoreList()
        {
            return m_arrScoreList;
        }

        public class My3v3PkInfo
        {
            public int nCurPkCount = (int)MAX_PK_COUNT; // 默认值为5，也就是默认认为玩家已经没有战斗次数
            public uint nScore;
            public byte nWinCount;
            public List<uint> arrAwardIDs = new List<uint>();
        }

        My3v3PkInfo m_pkInfo = new My3v3PkInfo();
        public My3v3PkInfo PkInfo
        {
            set
            {
                m_pkInfo = value;
            }
            get
            {
                if(m_pkInfo == null)
                {
                    m_pkInfo = new My3v3PkInfo();
                }

                return m_pkInfo;
            }
        }
   
        public ScoreListItem m_myRankInfo = new ScoreListItem();
        
        public ScoreListItem GetMyRankInfo()
        {
            return m_myRankInfo;
        }

        public bool HasTeam()
        {
            return m_myTeam != null;
        }

        public Team GetMyTeam()
        {
            return m_myTeam;
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

        private uint mTeamDungeonID = 0;
        public uint TeamDungeonID
        {
            get
            {
                return mTeamDungeonID;
            }

            set
            {
                //Logger.LogProcessFormat("[组队数据] 当前目标TeamDungeonID {0} => {1}", mTeamDungeonID, value);
                mTeamDungeonID = value;
            }
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
            }

            return relation;
        }

        public bool IsTeamLeaderByRoleID(ulong roleId)
        {
            return m_myTeam != null && m_myTeam.leaderInfo.id == roleId;
        }

        public bool IsTeamLeader()
        {
            return IsTeamLeaderByRoleID(PlayerBaseData.GetInstance().RoleID);
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public override void Initialize()
        {
            Clear();
            _BindNetMsg();

            var data = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_ROOM_REJECT_CHANGEPOS_TIME);
            if(data != null)
            {
                fChangePosLastTime = data.Value;
            }

            // 3v3积分赛模式不需要读取或者存储本地配置文件
            //InitRoomSettingData();

            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
        }

        public override void Clear()
        {
            m_arrScoreList = null;
            m_pkInfo = null;
            bMatching = false;
            bOpenNotifyFrame = false;
            NotifyCount = 0;
            scoreWarStatus = ScoreWarStatus.SWS_INVALID;
            scoreWarStateEndTime = 0;            

            ClearRoomInfo();     

            if(roomSettingData != null)
            {
                var data = roomSettingData.GetEnumerator();

                while(data.MoveNext())
                {
                    var sdata = data.Current.Value;

                    if(sdata != null)
                    {
                        sdata.DefaultInit();
                    }
                }
            }

            fChangePosLastTime = 0.0f;
            fAddUpTime = 0.0f;
            fChangePosLocationTime = 0.0f;
            _UnBindNetMsg();
            iInt = 0;
            m_bNotify = false;
            SimpleInviteLastTime = -1;

            m_myRankInfo = new ScoreListItem();

            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
        }

        void InitRoomSettingData()
        {
            if (roomSettingData == null)
            {
                roomSettingData = new Dictionary<RoomType, Pk3v3RoomSettingData>();
            }

            Pk3v3RoomSettingData amuseData = null;
            if (!roomSettingData.TryGetValue(RoomType.ROOM_TYPE_THREE_FREE, out amuseData))
            {
                amuseData = new Pk3v3RoomSettingData();
                amuseData.DefaultInit();

                roomSettingData.Add(RoomType.ROOM_TYPE_THREE_FREE, amuseData);
            }

            Pk3v3RoomSettingData MatchData = null;
            if (!roomSettingData.TryGetValue(RoomType.ROOM_TYPE_THREE_MATCH, out MatchData))
            {
                MatchData = new Pk3v3RoomSettingData();
                MatchData.DefaultInit();

                roomSettingData.Add(RoomType.ROOM_TYPE_THREE_MATCH, MatchData);
            }

            SetLocalSave();
        }

        void SetLocalSave()
        {
            Pk3v3RoomSettingData AmuseData = null;
            if (!roomSettingData.TryGetValue(RoomType.ROOM_TYPE_THREE_FREE, out AmuseData))
            {
                return;
            }

            Pk3v3RoomSettingData MatchData = null;
            if (!roomSettingData.TryGetValue(RoomType.ROOM_TYPE_THREE_MATCH, out MatchData))
            {
                return;
            }

            string Amuse_bSetMinLv_Key = GetPk3v3LocalDataKey(RoomType.ROOM_TYPE_THREE_FREE, "bSetMinLv");
            string Amuse_bSetMinRankLv_Key = GetPk3v3LocalDataKey(RoomType.ROOM_TYPE_THREE_FREE, "bSetMinRankLv");
            string Amuse_MinLv_Key = GetPk3v3LocalDataKey(RoomType.ROOM_TYPE_THREE_FREE, "MinLv");
            string Amuse_MinRankLv_Key = GetPk3v3LocalDataKey(RoomType.ROOM_TYPE_THREE_FREE, "MinRankLv");
            string Match_bSetMinLv_Key = GetPk3v3LocalDataKey(RoomType.ROOM_TYPE_THREE_MATCH, "bSetMinLv");
            string Match_bSetMinRankLv_Key = GetPk3v3LocalDataKey(RoomType.ROOM_TYPE_THREE_MATCH, "bSetMinRankLv");
            string Match_MinLv_Key = GetPk3v3LocalDataKey(RoomType.ROOM_TYPE_THREE_MATCH, "MinLv");
            string Match_MinRankLv_Key = GetPk3v3LocalDataKey(RoomType.ROOM_TYPE_THREE_MATCH, "MinRankLv");

            string Amuse_bSetMinLv_Value = PlayerLocalSetting.GetValue(Amuse_bSetMinLv_Key) as string;
            string Amuse_bSetMinRankLv_Value = PlayerLocalSetting.GetValue(Amuse_bSetMinRankLv_Key) as string;
            string Amuse_MinLv_Value = PlayerLocalSetting.GetValue(Amuse_MinLv_Key) as string;
            string Amuse_MinRankLv_Value = PlayerLocalSetting.GetValue(Amuse_MinRankLv_Key) as string;
            string Match_bSetMinLv_Value = PlayerLocalSetting.GetValue(Match_bSetMinLv_Key) as string;
            string Match_bSetMinRankLv_Value = PlayerLocalSetting.GetValue(Match_bSetMinRankLv_Key) as string;
            string Match_MinLv_Value = PlayerLocalSetting.GetValue(Match_MinLv_Key) as string;
            string Match_MinRankLv_Value = PlayerLocalSetting.GetValue(Match_MinRankLv_Key) as string;

            if(Amuse_bSetMinLv_Value == null || Amuse_bSetMinLv_Value == "")
            {
                PlayerLocalSetting.SetValue(Amuse_bSetMinLv_Key, AmuseData.bSetMinLv.ToString());
            }
            else
            {
                if(Amuse_bSetMinLv_Value == "True")
                {
                    AmuseData.bSetMinLv = true;
                }
                else
                {
                    AmuseData.bSetMinLv = false;
                }             
            }

            if (Amuse_bSetMinRankLv_Value == null || Amuse_bSetMinRankLv_Value == "")
            {
                PlayerLocalSetting.SetValue(Amuse_bSetMinRankLv_Key, AmuseData.bSetMinRankLv.ToString());
            }
            else
            {
                if (Amuse_bSetMinRankLv_Value == "True")
                {
                    AmuseData.bSetMinRankLv = true;
                }
                else
                {
                    AmuseData.bSetMinRankLv = false;
                }
            }

            if (Amuse_MinLv_Value == null || Amuse_MinLv_Value == "")
            {
                PlayerLocalSetting.SetValue(Amuse_MinLv_Key, AmuseData.MinLv.ToString());
            }
            else
            {
                AmuseData.MinLv = Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel);

                int iMinLv = 0;
                if (int.TryParse(Amuse_MinLv_Value, out iMinLv))
                {
                    AmuseData.MinLv = iMinLv;
                }
            }

            if (Amuse_MinRankLv_Value == null || Amuse_MinRankLv_Value == "")
            {
                PlayerLocalSetting.SetValue(Amuse_MinRankLv_Key, AmuseData.MinRankLv.ToString());
            }
            else
            {
                AmuseData.MinRankLv = SeasonDataManager.GetInstance().GetMinRankID();

                int iMinRankLv = 0;
                if (int.TryParse(Amuse_MinRankLv_Value, out iMinRankLv))
                {
                    AmuseData.MinRankLv = iMinRankLv;
                }
            }

            if (Match_bSetMinLv_Value == null || Match_bSetMinLv_Value == "")
            {
                PlayerLocalSetting.SetValue(Match_bSetMinLv_Key, MatchData.bSetMinLv.ToString());
            }
            else
            {
                if (Match_bSetMinLv_Value == "True")
                {
                    MatchData.bSetMinLv = true;
                }
                else
                {
                    MatchData.bSetMinLv = false;
                }
            }

            if (Match_bSetMinRankLv_Value == null || Match_bSetMinRankLv_Value == "")
            {
                PlayerLocalSetting.SetValue(Match_bSetMinRankLv_Key, MatchData.bSetMinRankLv.ToString());
            }
            else
            {
                if (Match_bSetMinRankLv_Value == "True")
                {
                    MatchData.bSetMinRankLv = true;
                }
                else
                {
                    MatchData.bSetMinRankLv = false;
                }
            }

            if (Match_MinLv_Value == null || Match_MinLv_Value == "")
            {
                PlayerLocalSetting.SetValue(Match_MinLv_Key, MatchData.MinLv.ToString());
            }
            else
            {
                MatchData.MinLv = Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel);

                int iMinLv = 0;
                if (int.TryParse(Match_MinLv_Value, out iMinLv))
                {
                    MatchData.MinLv = iMinLv;
                }
            }

            if (Match_MinRankLv_Value == null || Match_MinRankLv_Value == "")
            {
                PlayerLocalSetting.SetValue(Match_MinRankLv_Key, MatchData.MinRankLv.ToString());
            }
            else
            {
                MatchData.MinRankLv = SeasonDataManager.GetInstance().GetMinRankID();

                int iMinRankLv = 0;
                if (int.TryParse(Match_MinRankLv_Value, out iMinRankLv))
                {
                    MatchData.MinRankLv = iMinRankLv;
                }
            }

            PlayerLocalSetting.SaveConfig();
        }

        public override void Update(float a_fTime)
        {
            if (isNotify || SimpleInviteLastTime>0)
            {
                fAddUpTime += a_fTime;
                if (fAddUpTime > 1.0f)
                {
                    if(SimpleInviteLastTime>0)
                    {
                        SimpleInviteLastTime -= 1;
                    }
                    if(isNotify)
                    {
                        fChangePosLocationTime -= 1.0f;
                        iInt = (int)(fChangePosLocationTime);
                    }
                    fAddUpTime = 0.0f;
                    
                }
                
            }
        }

        public void SetCountDownTime(float fTime)
        {
            if (fTime > 0.0f)
            {
                fChangePosLocationTime = fTime;
                iInt = (int)fTime;
                m_bNotify = true;
            }
        }
        public void ClearRoomInfo()
        {
            if(roomInfo != null)
            {
                roomInfo.roomSimpleInfo.id = 0;
                roomInfo.roomSimpleInfo.isPassword = 0;
                roomInfo.roomSimpleInfo.ownerId = 0;
                roomInfo.roomSimpleInfo.roomStatus = 0;
                roomInfo.roomSimpleInfo.roomType = 0;

                for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                {
                    roomInfo.roomSlotInfos[i].playerId = 0;
                    roomInfo.roomSlotInfos[i].group = 0;
                    roomInfo.roomSlotInfos[i].index = 0;
                    roomInfo.roomSlotInfos[i].playerOccu = 0;
                }
            }

            InviteRoomList.Clear();

            bHasPassword = false;
            PassWord = "";

            m_myTeam = null;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);
        }
        
        public bool CheckPk3v3CrossScence()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null && scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3)
                {
                    return true;
                }
            }

            return false;
        }

        public void _BindNetMsg()
        {
            if (m_bNetBind == false)
            {
                NetProcess.AddMsgHandler(WorldSyncRoomInfo.MsgID, _OnWorldSyncReLoginRoomInfo);
                NetProcess.AddMsgHandler(WorldUpdateRoomRes.MsgID, _OnWorldUpdateRoomRes);           
                NetProcess.AddMsgHandler(WorldSyncRoomSimpleInfo.MsgID, _OnSyncRoomSimpleInfo);              
                NetProcess.AddMsgHandler(WorldSyncRoomPasswordInfo.MsgID, _OnSyncRoomPasswordInfo);
                NetProcess.AddMsgHandler(WorldSyncRoomSlotInfo.MsgID, _OnSyncRoomSlotInfo);      
                NetProcess.AddMsgHandler(WorldJoinRoomRes.MsgID, _OnJoinRoomRes);
                NetProcess.AddMsgHandler(WorldInviteJoinRoomRes.MsgID, _OnInviteJoinRoomRes);
                NetProcess.AddMsgHandler(WorldSyncRoomInviteInfo.MsgID, _OnSyncInviteInfo);
                NetProcess.AddMsgHandler(WorldBeInviteRoomRes.MsgID, _OnRoomInviteReply);
                NetProcess.AddMsgHandler(WorldSyncRoomKickOutInfo.MsgID, _OnSyncKickOutInfo);
                NetProcess.AddMsgHandler(WorldKickOutRoomRes.MsgID, _OnKickOutRoomRes);
                NetProcess.AddMsgHandler(WorldChangeRoomOwnerRes.MsgID, _OnChangeRoomOwnerRes);
                NetProcess.AddMsgHandler(WorldRoomCloseSlotRes.MsgID, _OnRoomCloseSlotRes);
                NetProcess.AddMsgHandler(WorldRoomBattleStartRes.MsgID, _OnRoomBeginGameRes);            
                NetProcess.AddMsgHandler(WorldRoomBattleCancelRes.MsgID, _OnRoomBattleCancelRes);      
                NetProcess.AddMsgHandler(WorldRoomBattleReadyRes.MsgID, _OnVoteReadyRes);
                NetProcess.AddMsgHandler(WorldRoomSendInviteLinkRes.MsgID, _OnRoomSendInviteLinkRes);
                NetProcess.AddMsgHandler(WorldRoomSwapSlotRes.MsgID, _OnRoomSwapSlotRes);
                NetProcess.AddMsgHandler(WorldSyncRoomSwapSlotInfo.MsgID, _OnSyncRoomSwapSlotInfo);
                NetProcess.AddMsgHandler(WorldRoomResponseSwapSlotRes.MsgID, _OnRoomResponseSwapSlotRes);
                NetProcess.AddMsgHandler(WorldSyncRoomSwapResultInfo.MsgID, _OnSyncRoomSwapResultInfo);

                NetProcess.AddMsgHandler(WorldSortListRet.MsgID, _OnRankListRes);
                NetProcess.AddMsgHandler(SceneSyncScoreWarInfo.MsgID, OnWorldSyncScoreWarInfo);

                NetProcess.AddMsgHandler(WorldMatchStartRes.MsgID, _onStartBattle);
                NetProcess.AddMsgHandler(WorldMatchCancelRes.MsgID, _onCancelBattle);

                NetProcess.AddMsgHandler(WorldQuitRoomRes.MsgID, _OnQuitRoomRes);

                m_bNetBind = true;
            }
        }

        public void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldSyncRoomInfo.MsgID, _OnWorldSyncReLoginRoomInfo);
            NetProcess.RemoveMsgHandler(WorldUpdateRoomRes.MsgID, _OnWorldUpdateRoomRes);
            NetProcess.RemoveMsgHandler(WorldSyncRoomSimpleInfo.MsgID, _OnSyncRoomSimpleInfo);
            NetProcess.RemoveMsgHandler(WorldSyncRoomPasswordInfo.MsgID, _OnSyncRoomPasswordInfo);
            NetProcess.RemoveMsgHandler(WorldSyncRoomSlotInfo.MsgID, _OnSyncRoomSlotInfo);
            NetProcess.RemoveMsgHandler(WorldJoinRoomRes.MsgID, _OnJoinRoomRes);
            NetProcess.RemoveMsgHandler(WorldInviteJoinRoomRes.MsgID, _OnInviteJoinRoomRes);
            NetProcess.RemoveMsgHandler(WorldSyncRoomInviteInfo.MsgID, _OnSyncInviteInfo);
            NetProcess.RemoveMsgHandler(WorldBeInviteRoomRes.MsgID, _OnRoomInviteReply);
            NetProcess.RemoveMsgHandler(WorldSyncRoomKickOutInfo.MsgID, _OnSyncKickOutInfo);
            NetProcess.RemoveMsgHandler(WorldKickOutRoomRes.MsgID, _OnKickOutRoomRes);
            NetProcess.RemoveMsgHandler(WorldChangeRoomOwnerRes.MsgID, _OnChangeRoomOwnerRes);
            NetProcess.RemoveMsgHandler(WorldRoomCloseSlotRes.MsgID, _OnRoomCloseSlotRes);
            NetProcess.RemoveMsgHandler(WorldRoomBattleStartRes.MsgID, _OnRoomBeginGameRes);
            NetProcess.RemoveMsgHandler(WorldRoomBattleCancelRes.MsgID, _OnRoomBattleCancelRes);
            NetProcess.RemoveMsgHandler(WorldRoomBattleReadyRes.MsgID, _OnVoteReadyRes);
            NetProcess.RemoveMsgHandler(WorldRoomSendInviteLinkRes.MsgID, _OnRoomSendInviteLinkRes);
            NetProcess.RemoveMsgHandler(WorldRoomSwapSlotRes.MsgID, _OnRoomSwapSlotRes);
            NetProcess.RemoveMsgHandler(WorldSyncRoomSwapSlotInfo.MsgID, _OnSyncRoomSwapSlotInfo);
            NetProcess.RemoveMsgHandler(WorldRoomResponseSwapSlotRes.MsgID, _OnRoomResponseSwapSlotRes);
            NetProcess.RemoveMsgHandler(WorldSyncRoomSwapResultInfo.MsgID, _OnSyncRoomSwapResultInfo);

            NetProcess.RemoveMsgHandler(WorldSortListRet.MsgID, _OnRankListRes);
            NetProcess.RemoveMsgHandler(SceneSyncScoreWarInfo.MsgID, OnWorldSyncScoreWarInfo);

            NetProcess.RemoveMsgHandler(WorldMatchStartRes.MsgID, _onStartBattle);
            NetProcess.RemoveMsgHandler(WorldMatchCancelRes.MsgID, _onCancelBattle);

            m_bNetBind = false;
        }

        public bool bMatching = false;

        public bool IsMathcing()
        {
            return bMatching;
        }

        private void _onStartBattle(MsgDATA a_msgData)
        {
            if (null == a_msgData)
            {
                return;
            }

            WorldMatchStartRes msgRet = new WorldMatchStartRes();
            msgRet.decode(a_msgData.bytes);

            if (msgRet == null)
            {
                return;
            }

            if (msgRet.result != 0)
            {
                if (msgRet.result == (int)ProtoErrorCode.PREMIUM_LEAGUE_PRELIMINAY_ALREADY_LOSE)
                {
                    //none
                }
                else
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchFailed);
                return;
            }

            bMatching = true;

            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchStartSuccess);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3BeginMatchRes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3BeginMatch);
        }

        private void _onCancelBattle(MsgDATA a_msgData)
        {
            if (null == a_msgData)
            {
                return;
            }

            WorldMatchCancelRes msgRet = new WorldMatchCancelRes();
            msgRet.decode(a_msgData.bytes);

            if (msgRet == null)
            {
                return;
            }

            if (msgRet.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelFailed);
                return;
            }

            bMatching = false;
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelSuccess);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3BeginMatchRes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CancelMatch);
        }

        // 断线重连后同步房间信息
        void _OnWorldSyncReLoginRoomInfo(MsgDATA msg)
        {
            WorldSyncRoomInfo msgData = new WorldSyncRoomInfo();
            msgData.decode(msg.bytes);

            roomInfo = msgData.info;

            if(roomInfo.roomSimpleInfo.id <= 0)
            {
                Logger.LogError("3v3房间掉线重连后,玩家初始化数据时,服务器同步的房间id有问题");
            }

            bool berror = true;
            for(int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                if(roomInfo.roomSlotInfos[i].playerId != 0)
                {
                    berror = false;
                }
            }

            GetTeamDataFromRoomData();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);

            if (berror)
            {
                Logger.LogError("3v3房间掉线重连后,玩家初始化数据时,服务器同步的玩家列表有问题，全是空数据");
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
                target.playerSeasonLevel = source.playerSeasonLevel;

                target.isOnline = source.isOnline;
                target.isPrepared = source.isPrepared;
                target.isAssist = source.isAssist;

                target.avatarInfo = source.avatarInfo;
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

        void _DebugTeamData()
        {
            if (m_myTeam == null)
            {
                //Logger.LogProcessFormat("my team data is null!!");
            }
            else
            {
                //m_myTeam.Debug();
            }
        }

        // 创建或更新房间返回
        void _OnWorldUpdateRoomRes(MsgDATA msg)
        {
            //Logger.LogErrorFormat("_OnWorldUpdateRoomRes");

//             {
//                 ClientSystemTown systemTown1 = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
//                 if (systemTown1 != null)
//                 {
//                     CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown1.CurrentSceneID);
//                     if (scenedata != null && scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3 || scenedata.SceneSubType == CitySceneTable.eSceneSubType.TRADITION)
//                     {
//                         return ;
//                     }
//                 }
//             }

//             if (!CheckPk3v3CrossScence())
//             {
//                 return;
//             }

            WorldUpdateRoomRes msgData = new WorldUpdateRoomRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            roomInfo = msgData.info;

            if(roomInfo.roomSimpleInfo.roomType != (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomInfoUpdate);

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossTeamListFrame>())
                    {
                        ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossTeamListFrame>();
                    }

                    {                       
                        GetTeamDataFromRoomData();

                        //Logger.LogProcessFormat("sync team info...");
                        _DebugTeamData();
                    }

                    if (!ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossMyTeamFrame>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMyTeamFrame>();
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);
                }
            }
        }

        void _OnSyncRoomSimpleInfo(MsgDATA msg)
        {
            //Logger.LogErrorFormat("_OnSyncRoomSimpleInfo");

//             if (!CheckPk3v3CrossScence())
//             {
//                 return;
//             }

            WorldSyncRoomSimpleInfo msgData = new WorldSyncRoomSimpleInfo();
            msgData.decode(msg.bytes);

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [_OnSyncRoomSimpleInfo]");
            }
            if(msgData.info.roomType != (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                return;
            }

            if (roomInfo != null)
            {
                if (roomInfo.roomSimpleInfo.ownerId != msgData.info.ownerId)
                {
                    string ownername = "";

                    for(int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                    {
                        if(roomInfo.roomSlotInfos[i].playerId == msgData.info.ownerId)
                        {
                            ownername = roomInfo.roomSlotInfos[i].playerName;
                            break;
                        }
                    }

                    object[] args = new object[1];
                    args[0] = ownername;

                    SystemNotifyManager.SystemNotify(9220, args);
                }
            }

            roomInfo.roomSimpleInfo = msgData.info;            

            if(msgData.info.roomStatus == (byte)RoomStatus.ROOM_STATUS_OPEN)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CancelMatch);
            }
            else if(msgData.info.roomStatus == (byte)RoomStatus.ROOM_STATUS_READY)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3VoteEnterPkFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<Pk3v3VoteEnterPkFrame>();
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3VoteEnterBattle);

                bool bOpen = true;

                int iCount = 0;
                ulong uplayerid = 0;

                for(int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                {
                    if(roomInfo.roomSlotInfos[i].playerId > 0)
                    {
                        iCount++;
                        uplayerid = roomInfo.roomSlotInfos[i].playerId;
                    }
                }

                if (iCount == 1 && uplayerid == roomInfo.roomSimpleInfo.ownerId)
                {
                    bOpen = false;
                }

                /*if(!CheckPk3v3CrossScence())
                {
                    bOpen = false;
                }*/

                if(bOpen)
                {
                    ClientSystemManager.GetInstance().OpenFrame<Pk3v3VoteEnterPkFrame>(FrameLayer.Middle);
                }          
            }
            else if(msgData.info.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH)
            {
                ClientSystemManager.GetInstance().CloseFrame<Pk3v3VoteEnterPkFrame>();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3BeginMatch);
            }

            GetTeamDataFromRoomData();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSimpleInfoUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSlotInfoUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);
        }

        void _OnSyncRoomPasswordInfo(MsgDATA msg)
        {
//             if (!CheckPk3v3CrossScence())
//             {
//                 return;
//             }

            WorldSyncRoomPasswordInfo msgData = new WorldSyncRoomPasswordInfo();
            msgData.decode(msg.bytes);

            bHasPassword = msgData.password != "";
            PassWord = msgData.password;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Set3v3RoomPassword);
        }

        void GetTeamDataFromRoomData()
        {
            if(roomInfo == null)
            {
                return;
            }

            if (roomInfo.roomSimpleInfo.id <= 0)
            {
                return;
            }

            m_myTeam = new Team();

            m_myTeam.teamID = roomInfo.roomSimpleInfo.id;
            m_myTeam.leaderInfo = new TeammemberBaseInfo();
            m_myTeam.leaderInfo.id = roomInfo.roomSimpleInfo.ownerId;
            m_myTeam.autoAgree = 1;

            //TeamDungeonID = msgData.target;

            for (int i = 0; i < m_myTeam.members.Length; ++i)
            {
                TeamMember target = new TeamMember();
                target.ClearPlayerInfo();

                m_myTeam.members[i] = target;
            }

            int iIndex = 0;
            for (int i = 0; i < roomInfo.roomSlotInfos.Length; ++i)
            {
                if (roomInfo.roomSlotInfos[i].playerId <= 0)
                {
                    continue;
                }

                TeamMember member = new TeamMember();
                member.id = roomInfo.roomSlotInfos[i].playerId;
                member.name = roomInfo.roomSlotInfos[i].playerName;
                member.occu = roomInfo.roomSlotInfos[i].playerOccu;
                member.avatarInfo = roomInfo.roomSlotInfos[i].avatar;
                member.level = roomInfo.roomSlotInfos[i].playerLevel;
                member.playerSeasonLevel = roomInfo.roomSlotInfos[i].playerSeasonLevel;
                member.viplevel = roomInfo.roomSlotInfos[i].playerVipLevel;

                _ParseMemberData(member, m_myTeam.members[iIndex]);
                iIndex++;
            }

            int iLeaderIndex = -1;
            for (int i = 0; i < m_myTeam.members.Length; i++)
            {
                Pk3v3CrossDataManager.TeamMember data = m_myTeam.members[i];
                if (data.id == m_myTeam.leaderInfo.id)
                {
                    iLeaderIndex = i;
                    break;
                }
            }

            if (iLeaderIndex != -1 && iLeaderIndex != 0)
            {
                Swap(ref m_myTeam.members[0], ref m_myTeam.members[iLeaderIndex]);
            }

            _UpdateMemberCountInfo();

            return;
        }

        void Swap<T>(ref T x, ref T y)
        {
            T temp = x;
            x = y;
            y = temp;
        }


        // 同步房间成员消息
        void _OnSyncRoomSlotInfo(MsgDATA msg)
        {
//             if (!CheckPk3v3CrossScence())
//             {
//                 return;
//             }

            WorldSyncRoomSlotInfo msgData = new WorldSyncRoomSlotInfo();
            msgData.decode(msg.bytes);

            if(roomInfo != null)
            {
                if(roomInfo.roomSlotInfos != null)
                {
                    for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                    {
                        if (roomInfo.roomSlotInfos[i].group != msgData.slotInfo.group)
                        {
                            continue;
                        }

                        if (roomInfo.roomSlotInfos[i].index != msgData.slotInfo.index)
                        {
                            continue;
                        }

                        if(roomInfo.roomSlotInfos[i].playerId > 0 && msgData.slotInfo.playerId <= 0)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3PlayerLeave, msgData.slotInfo.group, msgData.slotInfo.index);
                        }

                        if(msgData.slotInfo.playerId > 0 && msgData.slotInfo.playerId != roomInfo.roomSlotInfos[i].playerId)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3ChangePosition, msgData.slotInfo.playerId, msgData.slotInfo.group);
                        }

                        roomInfo.roomSlotInfos[i] = msgData.slotInfo;

                        GetTeamDataFromRoomData();

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSlotInfoUpdate);

                        if (msgData.slotInfo.status == (byte)RoomSlotStatus.ROOM_SLOT_STATUS_OFFLINE)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect(string.Format("玩家{0}已断开连接", msgData.slotInfo.playerName));
                        }

                        if (msgData.slotInfo.readyStatus == (byte)RoomSlotReadyStatus.ROOM_SLOT_READY_STATUS_REFUSE || 
                           msgData.slotInfo.readyStatus == (byte)RoomSlotReadyStatus.ROOM_SLOT_READY_STATUS_TIMEOUT)
                        {                           
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RefuseBeginMatch);

                            if(msgData.slotInfo.readyStatus == (byte)RoomSlotReadyStatus.ROOM_SLOT_READY_STATUS_REFUSE)
                            {
                                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("pk_room_beginfight_refuse", msgData.slotInfo.playerName));
                            }
                            else
                            {
                                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("pk_room_beginfight_timeout", msgData.slotInfo.playerName));
                            }                    
                        }

                        break;
                    }                
                }
                else
                {
                    Logger.LogError("roomInfo.roomSlotInfos is null in [_OnSyncRoomSlotInfo]");
                }
            }
            else
            {
                Logger.LogError("roomInfo is null in [_OnSyncRoomSlotInfo]");
            }      
        }

        // 玩家自己主动退出房间的返回
        void _OnQuitRoomRes(MsgDATA msg)
        {
            WorldQuitRoomRes res = new WorldQuitRoomRes();
            res.decode(msg.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
            }

            Pk3v3CrossDataManager.GetInstance().ClearRoomInfo();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSimpleInfoUpdate);

            ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossMyTeamFrame>();
        }

        // 玩家自己主动加入房间的返回
        void _OnJoinRoomRes(MsgDATA msg)
        {
//             if (!CheckPk3v3CrossScence())
//             {
//                 return;
//             }

            WorldJoinRoomRes msgData = new WorldJoinRoomRes();
            msgData.decode(msg.bytes);            
            if(msgData.info != null && msgData.info.roomSimpleInfo != null && msgData.info.roomSimpleInfo.roomType != (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                return;
            }

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                if(msgData.result == (uint)ProtoErrorCode.ROOM_SYSTEM_QUIT_JOIN_ROOM_NOT_EXIST)
                {
                    SystemNotifyManager.SystemNotify(9217, AcceptCreateAmuseRoom);
                }
                else if(msgData.result == (uint)ProtoErrorCode.ROOM_SYSTEM_QUIT_JOIN_MATCH_NOT_EXIST)
                {
                    SystemNotifyManager.SystemNotify(9218, AcceptCreateMatchRoom);
                }
                else if(msgData.result == (uint)ProtoErrorCode.ROOM_SYSTEM_PASSWORD_NOT_EMPTY)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RefreshRoomList);
                    SystemNotifyManager.SysNotifyFloatingEffect("该房间已设置密码");
                }
                else
                {
                    if (msgData.info != null && msgData.info.roomSimpleInfo != null && msgData.info.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
                    {
                        SystemNotifyManager.SystemNotify((int)msgData.result);
                    }                        
                }
               
                return;
            }

            if (msgData.info.roomSimpleInfo.roomType != (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                return;
            }

            roomInfo = msgData.info;

            if(roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [_OnJoinRoomRes]");
                return;
            }

            //SwitchToPk3v3CrossScene();

            {               
                GetTeamDataFromRoomData();
                //Logger.LogProcessFormat("sync team info...");
                _DebugTeamData();
            }

            if (!CheckPk3v3CrossScence())
            {
                SwitchToPk3v3CrossScene();
            }
            else
            {
                if (!ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossMyTeamFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMyTeamFrame>();
                }
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossTeamListFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossTeamListFrame>();
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSimpleInfoUpdate);
        }

        // 给邀请者的返回
        void _OnInviteJoinRoomRes(MsgDATA msg)
        {
//             if (!CheckPk3v3CrossScence())
//             {
//                 return;
//             }

            WorldInviteJoinRoomRes msgData = new WorldInviteJoinRoomRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
        }

        // 同步给被邀请者的消息
        void _OnSyncInviteInfo(MsgDATA msg)
        {          
            WorldSyncRoomInviteInfo msgData = new WorldSyncRoomInviteInfo();
            msgData.decode(msg.bytes);

            if(msgData.roomType != (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                return;
            }

            bool bFind = false;
            for (int i = 0; i < InviteRoomList.Count; i++)
            {
                if (InviteRoomList[i].inviterId == msgData.inviterId)
                {
                    InviteRoomList[i] = msgData;

                    bFind = true;
                    break;
                }
            }

            if(!bFind)
            {
                InviteRoomList.Insert(0, msgData);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3InviteRoomListUpdate,msgData.roomType);
        }

        // 被邀请者同意或拒绝加入房间
        void _OnRoomInviteReply(MsgDATA msg)
        {  
            WorldBeInviteRoomRes msgData = new WorldBeInviteRoomRes();
            msgData.decode(msg.bytes);
           

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS && msgData.result != (uint)ProtoErrorCode.ROOM_SYSTEM_BE_INVITE_REFUSE)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            if(msgData.result == (uint)ProtoErrorCode.ROOM_SYSTEM_BE_INVITE_REFUSE)
            {
                return;
            }

            if (msgData.roomInfo.roomSimpleInfo.roomType != (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                return;
            }

            roomInfo = msgData.roomInfo;

            GetTeamDataFromRoomData();

            if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossTeamListFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossTeamListFrame>();
            }

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [_OnJoinRoomRes]");
                return;
            }

            if (msgData.result != (uint)ProtoErrorCode.ROOM_SYSTEM_BE_INVITE_REFUSE)
            {
                if(!CheckPk3v3CrossScence())
                {
                    SwitchToPk3v3CrossScene();
                }
                else
                {
                    if (!ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossMyTeamFrame>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMyTeamFrame>();
                    }
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);
        }

        // 通知被踢出房间的人
        void _OnSyncKickOutInfo(MsgDATA msg)
        {
            WorldSyncRoomKickOutInfo msgData = new WorldSyncRoomKickOutInfo();
            msgData.decode(msg.bytes);

            ClearRoomInfo();

            SystemNotifyManager.SysNotifyMsgBoxOK(string.Format("你被玩家{0}踢出了队伍", msgData.kickPlayerName), OnClickOkAcceptBeKickedOut);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3KickOut);

            if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossMyTeamFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossMyTeamFrame>();
            }            
        }

        // 通知发起踢人的人
        void _OnKickOutRoomRes(MsgDATA msg)
        {
            WorldKickOutRoomRes msgData = new WorldKickOutRoomRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
        }

        void _OnChangeRoomOwnerRes(MsgDATA msg)
        {
            WorldChangeRoomOwnerRes msgData = new WorldChangeRoomOwnerRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
        }

        void _OnRoomCloseSlotRes(MsgDATA msg)
        {
            WorldRoomCloseSlotRes msgData = new WorldRoomCloseSlotRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
        }

        // 房主发起进入战斗返回，只返回给房主
        void _OnRoomBeginGameRes(MsgDATA msg)
        {
            WorldRoomBattleStartRes msgData = new WorldRoomBattleStartRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                if(msgData.result != 3600003)
                {
                    SystemNotifyManager.SystemNotify((int)msgData.result);
                }                
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3BeginMatchRes);
        }

        // 房主发起取消进入战斗返回，只返回给房主
        void _OnRoomBattleCancelRes(MsgDATA msg)
        {
            WorldRoomBattleCancelRes msgData = new WorldRoomBattleCancelRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CancelMatchRes);
        }

        // 返回给投票者自己的协议
        void _OnVoteReadyRes(MsgDATA msg)
        {
            WorldRoomBattleReadyRes msgData = new WorldRoomBattleReadyRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
        }

        // 返回给发送邀请的玩家
        void _OnRoomSendInviteLinkRes(MsgDATA msg)
        {
            WorldRoomSendInviteLinkRes msgData = new WorldRoomSendInviteLinkRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("消息已发送...");
            }
        }

        // 返回给请求交换位置的人
        void _OnRoomSwapSlotRes(MsgDATA msg)
        {
            WorldRoomSwapSlotRes msgData = new WorldRoomSwapSlotRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            if (msgData.playerId > 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("交换位置请求已发送,请等待...");
            }      
        }

        // 通知玩家交换位置信息
        void _OnSyncRoomSwapSlotInfo(MsgDATA msg)
        {
            WorldSyncRoomSwapSlotInfo msgData = new WorldSyncRoomSwapSlotInfo();
            msgData.decode(msg.bytes);

            SwapSlotInfo = msgData;

            object[] args = new object[1];
            args[0] = msgData.playerName;

            SystemNotifyManager.SystemNotify(9215, SwapPosOK, SwapPosCancel, fChangePosLastTime, args);
        }

        // 返回给接受或拒绝交换位置的人
        void _OnRoomResponseSwapSlotRes(MsgDATA msg)
        {
            WorldRoomResponseSwapSlotRes msgData = new WorldRoomResponseSwapSlotRes();
            msgData.decode(msg.bytes);
        }

        void _OnRankListRes(MsgDATA msg)
        {
            WorldSortListRet res = new WorldSortListRet();
            res.decode(msg.bytes);

            int pos = 0;
            BaseSortList arrRecods = SortListDecoder.Decode(msg.bytes, ref pos, msg.bytes.Length);

            if (arrRecods == null)
            {
                Logger.LogError("arrRecods decode fail");
                return;
            }

            if((SortListType)arrRecods.type.mainType != SortListType.SORTLIST_SCORE_WAR)
            {
                // 红包排行榜和这个排行榜绑定了同一个消息id，所以会出现类型不匹配的问题 这里去掉日志打印就好了
                //Logger.LogError("arrRecods.type error!!!");
                return;
            }

            if(m_arrScoreList == null)
            {
                m_arrScoreList = new List<ScoreListItem>();
                if(m_arrScoreList == null)
                {
                    Logger.LogErrorFormat("new List<ScoreListItem>() error!!!");
                    return;
                }
            }
            m_arrScoreList.Clear();

            for(int i = 0;i < arrRecods.entries.Count;i++)
            {
                ScoreWarSortListEntry entry = arrRecods.entries[i] as ScoreWarSortListEntry;
                if(entry == null)
                {
                    Logger.LogErrorFormat("arrRecods.entries[{0}] error!!!",i);
                    continue;
                }

                ScoreListItem item = new ScoreListItem();
                if(item == null)
                {
                    Logger.LogErrorFormat("new ScoreListItem() error!!!");
                    continue;
                }

                item.nPlayerID = entry.id;
                item.nPlayerScore = entry.score;
                item.strPlayerName = entry.name;
                item.strServerName = entry.serverName;
                item.nRank = entry.ranking;

                m_arrScoreList.Add(item);
            }

            if(arrRecods.selfEntry == null)
            {
                Logger.LogErrorFormat("arrRecods.selfEntry is null!!!");
            }
            else
            {
                ScoreWarSortListEntry entry = arrRecods.selfEntry as ScoreWarSortListEntry;
                if(entry != null)
                {
                    if(m_myRankInfo != null)
                    {
                        m_myRankInfo.nPlayerID = entry.id;
                        m_myRankInfo.nPlayerScore = entry.score;
                        m_myRankInfo.strPlayerName = entry.name;
                        m_myRankInfo.strServerName = entry.serverName;
                        m_myRankInfo.nRank = entry.ranking;                     
                    }
                }
                else
                {
                    Logger.LogErrorFormat("arrRecods.selfEntry as ScoreWarSortListEntry error!!!");
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdatePk3v3CrossRankScoreList);           
        }

        void _OnSyncRoomSwapResultInfo(MsgDATA msg)
        {
            WorldSyncRoomSwapResultInfo msgData = new WorldSyncRoomSwapResultInfo();
            msgData.decode(msg.bytes);

            if(msgData.result == (byte)RoomSwapResult.ROOM_SWAP_RESULT_REFUSE)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format("{0}拒绝了你交换位置的请求", msgData.playerName));
            }
            else if(msgData.result == (byte)RoomSwapResult.ROOM_SWAP_RESULT_CANCEL)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("交换位置请求取消");
                ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxOKCancel>();
            }       
        }

        void OnClickOkAcceptBeKickedOut()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.Pk3v3)
            {
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3KickOut);
        }

        void SwapPosOK()
        {
            SendAgreeChangePosReq(true);
        }

        void SwapPosCancel()
        {
            SendAgreeChangePosReq(false);
        }

        void AcceptCreateAmuseRoom()
        {
            SendCreateRoomReq(RoomType.ROOM_TYPE_THREE_FREE);     
        }

        void AcceptCreateMatchRoom()
        {
            SendCreateRoomReq(RoomType.ROOM_TYPE_THREE_SCORE_WAR);
        }

        public void SendCreateRoomReq(RoomType roomtype)
        {
            Pk3v3RoomSettingData setdata = null;

            //             if (!roomSettingData.TryGetValue(roomtype, out setdata))
            //             {
            //                 Logger.LogErrorFormat("3v3 {0} setting data is null", roomtype);
            //                 return;
            //             }

            setdata = new Pk3v3RoomSettingData();
            setdata.DefaultInit();

            WorldUpdateRoomReq req = new WorldUpdateRoomReq();

            req.roomId = 0;
            req.roomType = (byte)roomtype;
            req.name = TR.Value("pkcross_create_room_name", PlayerBaseData.GetInstance().Name);
            req.password = PassWord;
            req.limitPlayerLevel = (ushort)setdata.MinLv;
            req.limitPlayerSeasonLevel = (uint)setdata.MinRankLv;

            if (setdata.bSetMinLv)
            {
                req.isLimitPlayerLevel = 1;
            }
            else
            {
                req.isLimitPlayerLevel = 0;
            }

            if(setdata.bSetMinRankLv)
            {
                req.isLimitPlayerSeasonLevel = 1;
            }
            else
            {
                req.isLimitPlayerSeasonLevel = 0;
            }

            req.isLimitPlayerLevel = 1;
            req.isLimitPlayerSeasonLevel = 1;
            req.limitPlayerLevel = 40;
            req.limitPlayerSeasonLevel = (uint)SeasonDataManager.GetInstance().GetMinRankID();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public static void SendJoinRoomReq(uint roomid, RoomType roomtype = RoomType.ROOM_TYPE_INVALID, string password = "", uint createTime = 0)
        {
            WorldJoinRoomReq req = new WorldJoinRoomReq();

            req.roomId = roomid;

            if (roomtype != RoomType.ROOM_TYPE_INVALID)
            {
                req.roomType = (byte)roomtype;
            }

            req.password = password;
            req.createTime = createTime;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void Pk3v3RoomInviteOtherPlayer(ulong RoleId)
        {
            WorldInviteJoinRoomReq req = new WorldInviteJoinRoomReq();
            req.playerId = RoleId;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendPk3v3ChangePosReq(uint roomId, RoomSlotInfo TargetSlotInfo)
        {
            WorldRoomSwapSlotReq req = new WorldRoomSwapSlotReq();

            req.roomId = roomId;
            req.slotGroup = TargetSlotInfo.group;
            req.index = TargetSlotInfo.index;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendClosePosReq(byte group, byte index)
        {
            WorldRoomCloseSlotReq req = new WorldRoomCloseSlotReq();

            req.slotGroup = group;
            req.index = index;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendAgreeChangePosReq(bool bAgree)
        {
            WorldRoomResponseSwapSlotReq req = new WorldRoomResponseSwapSlotReq();

            if(bAgree)
            {
                req.isAccept = 1;
            }
            else
            {
                req.isAccept = 0;
            }

            req.slotGroup = SwapSlotInfo.slotGroup;
            req.slotIndex = SwapSlotInfo.slotIndex;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SwitchToPk3v3CrossScene()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable TownTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (TownTableData == null)
            {
                return;
            }

            ClientSystemManager.GetInstance().CloseFrame<PkWaitingRoom>();

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(true);
            }

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
                new SceneParams
                {
                    currSceneID = systemTown.CurrentSceneID,
                    currDoorID = 0,
                    targetSceneID = 5007,
                    targetDoorID = 0,
                }));
        }

        public RoomInfo GetRoomInfo()
        {
            return roomInfo;
        }

        public List<WorldSyncRoomInviteInfo> GetInviteRoomList()
        {
            return InviteRoomList;
        }

        public static bool HasInPk3v3Room()
        {
            if(roomInfo == null)
            {
                return false;
            }

            if(roomInfo.roomSlotInfos == null)
            {
                return false;
            }

            for(int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                if(roomInfo.roomSlotInfos[i].playerId == PlayerBaseData.GetInstance().RoleID)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckIsInMyRoom(ulong uId)
        {
            if (roomInfo == null || roomInfo.roomSlotInfos == null)
            {
                return false;
            }

            for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                if (roomInfo.roomSlotInfos[i].playerId == uId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckRoomIsMatching()
        {
            if (roomInfo == null)
            {
                return false;
            }

            if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("队伍正在匹配中,无法进行操作");
                return true;
            }

            return false;
        }

        static void SendCancelOnePersonMatchGameReq()
        {
            WorldMatchCancelReq req = new WorldMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public static void AcceptJoinPk3v3RoomLink(string param)
        {
            var tokens = param.Split('|');
            if(null == tokens || tokens.Length !=2)
            {
                return;
            }

            int iRoomid = 0;
            long lStamp = 0;

            if (!int.TryParse(tokens[0], out iRoomid) || !long.TryParse(tokens[1],out lStamp))
            {
                return;
            }

#if UNITY_EDITOR
            Logger.LogErrorFormat("AcceptJoinPk3v3RoomLink roomId = {0} stamp = {1} realtime = {2}", iRoomid, lStamp, Utility.ToUtcTime2Local(lStamp).ToString("tt yyMMdd hh:mm:ss", Utility.cultureInfo));
#endif

            //             if (HasInPk3v3Room())
            //             {
            //                 SystemNotifyManager.SysNotifyFloatingEffect("你已经在房间里了");
            //                 return;
            //             }

            // 娱乐模式场景不支持点击积分赛模式房间链接
            if (Pk3v3DataManager.HasInPk3v3Room())
            {
                //SystemNotifyManager.SysNotifyFloatingEffect("你已经在房间里了");
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneType == CitySceneTable.eSceneType.PK_PREPARE && PkWaitingRoom.bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            if(scenedata.SceneSubType == CitySceneTable.eSceneSubType.BUDO)
            {
                SystemNotifyManager.SystemNotify(9307);
                return;
            }

            if(iRoomid <= 0)
            {
                return;
            }

            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format("该活动需要达到{0}级后才能加入", SystemValueTableData.Value));
                return;
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel("进入积分赛场景会退出当前所在队伍，是否确认进入？", () =>
                {
                    TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);
                    SendJoinRoomReq((uint)iRoomid, RoomType.ROOM_TYPE_THREE_SCORE_WAR, "", (uint)lStamp);
                });

                return;
            }

            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null && pkInfo.nCurPkCount >= Pk3v3CrossDataManager.MAX_PK_COUNT)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("挑战次数已达上限，操作失败");
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                return;
            }

            SendJoinRoomReq((uint)iRoomid, RoomType.ROOM_TYPE_THREE_SCORE_WAR, "", (uint)lStamp);
        }

        public string GetRoomState(RoomStatus roomstatus)
        {
            if(roomstatus == RoomStatus.ROOM_STATUS_BATTLE || roomstatus == RoomStatus.ROOM_STATUS_MATCH || roomstatus == RoomStatus.ROOM_STATUS_READY)
            {
                return "<color=#f0cd0dff>决斗中</color>";
            }
            else if(roomstatus == RoomStatus.ROOM_STATUS_OPEN)
            {
                return "<color=#ffffffff>未开始</color>";
            }

            return "异常";
        }

        public string GetRoomType(RoomType roomtype)
        {
            if (roomtype == RoomType.ROOM_TYPE_THREE_FREE)
            {
                return "娱乐";
            }
            else if (roomtype == RoomType.ROOM_TYPE_THREE_MATCH)
            {
                return "段位";
            }

            return "异常";
        }

        public int GetRankLvByIndex(int iIndex)
        {
            if (iIndex == 0)
            {
                return SeasonDataManager.GetInstance().GetMinRankID();
            }
            else if (iIndex == 1)
            {
                return 24501;
            }
            else if (iIndex == 2)
            {
                return 34501;
            }
            else if (iIndex == 3)
            {
                return 44501;
            }
            else if (iIndex == 4)
            {
                return 54501;
            }
            else
            {
                return SeasonDataManager.GetInstance().GetMaxRankID();
            }
        }

        public int RandPassWord()
        {
            return UnityEngine.Random.Range(1000, 9999);
        }

        public string GetPk3v3LocalDataKey(RoomType roomType, string key)
        {
            return string.Format("{0}_3v3_{1}_{2}", PlayerBaseData.GetInstance().RoleID, roomType, key);
        }

        bool bOpenNotifyFrame = false;
        public int NotifyCount
        {
            set;
            get;
        }

        public bool IsOpenNotifyFrame
        {
            set { bOpenNotifyFrame = value; }
            get { return bOpenNotifyFrame; }
        }

        ScoreWarStatus scoreWarStatus = ScoreWarStatus.SWS_INVALID;
        UInt32 scoreWarStateEndTime = 0;

        public ScoreWarStatus Get3v3CrossWarStatus()
        {
            return scoreWarStatus;
        }

        public UInt32 Get3v3CrossWarStatusEndTime()
        {
            return scoreWarStateEndTime;
        }

        public override void OnApplicationStart()
        {  
            FileArchiveAccessor.LoadFileInPersistentFileArchive(m_kSavePath, out jsonText);
            if (jsonText == null)
            {
                FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, "");
                jsonText = "";
                return;
            }

            return;
        }

        string m_kSavePath = "3v3CrossOpen.json";
        string jsonText = null;

        public bool IsIDOpened(UInt64 id)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                return false;
            }

            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if(data == null)
            {
                return false;
            }         

            bool value = false;
            string keyName = id.ToString();

            if(data.ContainsKey(keyName) && data[keyName].IsBoolean)
            {
                return (bool)data[keyName];
            }

            return false;
            try
            {
                value = (bool)data[keyName];
            }
            catch (Exception e)
            {
                int iValue = (int)data[keyName];
                if (0 == iValue)
                {
                    value = false;
                }
                else
                {
                    value = true;
                }

            }
            finally
            {
                //Debug.Log("finally = ================");
            }

            return value;         
        }

        public void ClearIDOpened(UInt64 id)
        {
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return;
            }

            data[id.ToString()] = false;

            try
            {
                jsonText = data.ToJson();
                if (!string.IsNullOrEmpty(jsonText))
                {
                    FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, jsonText);
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(e.ToString());
            }

            return;
        }

        public void SetIDOpened(UInt64 id)
        {
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return;
            }

            data[id.ToString()] = true;

            try
            {
                jsonText = data.ToJson();
                if (!string.IsNullOrEmpty(jsonText))
                {
                    FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, jsonText);
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(e.ToString());
            }

            return;
        }

        private void OnLevelChanged(int iPreLv, int iCurLv)
        {
            int i3v3CrossOpenLv = 0;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null)
            {
                i3v3CrossOpenLv = SystemValueTableData.Value;
                if (i3v3CrossOpenLv > 0 && iPreLv < i3v3CrossOpenLv && iCurLv >= i3v3CrossOpenLv)
                {                    
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3CrossButton, (byte)Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus());
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
                }
            }
        }

        void OnWorldSyncScoreWarInfo(MsgDATA msg)
        {
            SceneSyncScoreWarInfo ret = new SceneSyncScoreWarInfo();
            ret.decode(msg.bytes);           

            scoreWarStatus = (ScoreWarStatus)ret.status;
            scoreWarStateEndTime = ret.statusEndTime;

            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
            {
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);

            bool isFlag = false;
            if (ret.status >= (byte)ScoreWarStatus.SWS_PREPARE && ret.status < (byte)ScoreWarStatus.SWS_WAIT_END)
            {
                isFlag = true;
            }
            else
            {
                isFlag = false;
            }

            if(isFlag)
            {
                NotifyCount++;
            }
            else
            {
                NotifyCount = 0;
                bOpenNotifyFrame = false;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                if (NotifyCount > 0 && !bOpenNotifyFrame && !IsIDOpened(ClientApplication.playerinfo.accid))
                {
                    bOpenNotifyFrame = true;
                    ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossOpenNotifyFrame>(FrameLayer.Middle);

                    SetIDOpened(ClientApplication.playerinfo.accid);
                }                
            }           

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3CrossButton, ret.status);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSimpleInfoUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSlotInfoUpdate);

            if(scoreWarStatus == ScoreWarStatus.SWS_BATTLE && ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMatchStartNotifyFrame>();
            }

            if(scoreWarStatus >= ScoreWarStatus.SWS_WAIT_END)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CancelMatchRes);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CancelMatch);
            }

            if(scoreWarStatus == ScoreWarStatus.SWS_INVALID || scoreWarStatus == ScoreWarStatus.SWS_WAIT_END || scoreWarStatus == ScoreWarStatus.ROOM_TYPE_MAX)
            {
                bOpenNotifyFrame = false;
                NotifyCount = 0;
                ClearIDOpened(ClientApplication.playerinfo.accid);

                ClientSystemManager.GetInstance().CloseFrame<JoinPK3v3CrossFrame>();
            }
        }
    }
}
