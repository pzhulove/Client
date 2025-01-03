using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class Pk3v3RoomSettingData
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

    public class Pk3v3DataManager : DataManager<Pk3v3DataManager>
    {
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

            InitRoomSettingData();
        }

        public override void Clear()
        {
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
        }

        public void BindNetMsg()
        {
            _BindNetMsg();
        }

        public void UnBindNetMsg()
        {
            _UnBindNetMsg();
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

            Pk3v3RoomSettingData meleeData = null;
            if (!roomSettingData.TryGetValue(RoomType.ROOM_TYPE_MELEE, out meleeData))
            {
                meleeData = new Pk3v3RoomSettingData();
                meleeData.DefaultInit();
                roomSettingData.Add(RoomType.ROOM_TYPE_MELEE, meleeData);
            }
            Pk3v3RoomSettingData MatchData = null;
            if (!roomSettingData.TryGetValue(RoomType.ROOM_TYPE_THREE_MATCH, out MatchData))
            {
                MatchData = new Pk3v3RoomSettingData();
                MatchData.DefaultInit();

                roomSettingData.Add(RoomType.ROOM_TYPE_THREE_MATCH, MatchData);
            }

//             {
//                 Pk3v3RoomSettingData MatchData1 = null;
//                 if (!roomSettingData.TryGetValue(RoomType.ROOM_TYPE_THREE_SCORE_WAR, out MatchData1))
//                 {
//                     MatchData1 = new Pk3v3RoomSettingData();
//                     MatchData1.DefaultInit();
// 
//                     roomSettingData.Add(RoomType.ROOM_TYPE_THREE_MATCH, MatchData1);
//                 }
//             }

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

            {
                Pk3v3RoomSettingData MatchData1 = null;
                if (!roomSettingData.TryGetValue(RoomType.ROOM_TYPE_THREE_SCORE_WAR, out MatchData1))
                {
                    return;
                }
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
        }

        void _BindNetMsg()
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

                m_bNetBind = true;
            }
        }

        void _UnBindNetMsg()
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

            m_bNetBind = false;
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

            if(berror)
            {
                Logger.LogError("3v3房间掉线重连后,玩家初始化数据时,服务器同步的玩家列表有问题，全是空数据");
            }
        }

        // 创建或更新房间返回
        void _OnWorldUpdateRoomRes(MsgDATA msg)
        {
            WorldUpdateRoomRes msgData = new WorldUpdateRoomRes();
            msgData.decode(msg.bytes);

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            roomInfo = msgData.info;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomInfoUpdate);

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.Pk3v3)
                    {
                        SwitchToPk3v3Scene();
                    }                  
                }
            }
        }

        void _OnSyncRoomSimpleInfo(MsgDATA msg)
        {
            WorldSyncRoomSimpleInfo msgData = new WorldSyncRoomSimpleInfo();
            msgData.decode(msg.bytes);

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [_OnSyncRoomSimpleInfo]");
            }

            if(msgData.info.roomType != (byte)RoomType.ROOM_TYPE_THREE_FREE && msgData.info.roomType != (byte)RoomType.ROOM_TYPE_MELEE)
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

                    SystemNotifyManager.SystemNotify(9214, args);
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

                for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                {
                    if (roomInfo.roomSlotInfos[i].playerId > 0)
                    {
                        iCount++;
                        uplayerid = roomInfo.roomSlotInfos[i].playerId;
                    }
                }

                if (iCount == 1 && uplayerid == roomInfo.roomSimpleInfo.ownerId)
                {
                    bOpen = false;
                }

                if (bOpen)
                {
                    ClientSystemManager.GetInstance().OpenFrame<Pk3v3VoteEnterPkFrame>(FrameLayer.Middle);
                }
            }
            else if(msgData.info.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH)
            {
                ClientSystemManager.GetInstance().CloseFrame<Pk3v3VoteEnterPkFrame>();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3BeginMatch);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSimpleInfoUpdate);
        }

        void _OnSyncRoomPasswordInfo(MsgDATA msg)
        {
            WorldSyncRoomPasswordInfo msgData = new WorldSyncRoomPasswordInfo();
            msgData.decode(msg.bytes);

            bHasPassword = msgData.password != "";
            PassWord = msgData.password;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Set3v3RoomPassword);
        }

        void _OnSyncRoomSlotInfo(MsgDATA msg)
        {
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

        // 玩家自己主动加入房间的返回
        void _OnJoinRoomRes(MsgDATA msg)
        {
            WorldJoinRoomRes msgData = new WorldJoinRoomRes();
            msgData.decode(msg.bytes);          

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                if(msgData.result == (uint)ProtoErrorCode.ROOM_SYSTEM_QUIT_JOIN_ROOM_NOT_EXIST)
                {
                    SystemNotifyManager.SystemNotify(9217, AcceptCreateAmuseRoom);
                }
                else if(msgData.result == (uint)ProtoErrorCode.ROOM_SYSTEM_QUIT_JOIN_MATCH_NOT_EXIST)
                {
                    SystemNotifyManager.SystemNotify(9217, AcceptCreateMatchRoom);
                }
                else if(msgData.result == (uint)ProtoErrorCode.ROOM_SYSTEM_PASSWORD_NOT_EMPTY)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RefreshRoomList);
                    SystemNotifyManager.SysNotifyFloatingEffect("该房间已设置密码");
                }
                else
                {
                    if (msgData.info != null && msgData.info.roomSimpleInfo != null 
                        && (msgData.info.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || msgData.info.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE))
                    {
                        SystemNotifyManager.SystemNotify((int)msgData.result);
                    }                    
                }
               
                return;
            }

            if (msgData.info.roomSimpleInfo.roomType != (byte)RoomType.ROOM_TYPE_THREE_FREE && msgData.info.roomSimpleInfo.roomType != (byte)RoomType.ROOM_TYPE_MELEE)
            {
                return;
            }

            roomInfo = msgData.info;

            if(roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [_OnJoinRoomRes]");
                return;
            }

            SwitchToPk3v3Scene();
        }

        // 给邀请者的返回
        void _OnInviteJoinRoomRes(MsgDATA msg)
        {
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

            if(msgData.roomType != (byte)RoomType.ROOM_TYPE_THREE_FREE && msgData.roomType != (byte)RoomType.ROOM_TYPE_MELEE)
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

            if(msgData.roomInfo.roomSimpleInfo.roomType != (byte)RoomType.ROOM_TYPE_THREE_FREE && msgData.roomInfo.roomSimpleInfo.roomType != (byte)RoomType.ROOM_TYPE_MELEE)
            {
                return;
            }

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS && msgData.result != (uint)ProtoErrorCode.ROOM_SYSTEM_BE_INVITE_REFUSE)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }

            roomInfo = msgData.roomInfo;

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [_OnJoinRoomRes]");
                return;
            }

            if (msgData.result != (uint)ProtoErrorCode.ROOM_SYSTEM_BE_INVITE_REFUSE)
            {
                SwitchToPk3v3Scene();
            }
        }

        // 通知被踢出房间的人
        void _OnSyncKickOutInfo(MsgDATA msg)
        {
            WorldSyncRoomKickOutInfo msgData = new WorldSyncRoomKickOutInfo();
            msgData.decode(msg.bytes);

            ClearRoomInfo();

            SystemNotifyManager.SysNotifyMsgBoxOK(string.Format("你被玩家{0}踢出了{1}号房间(10秒内无法再次进入)", msgData.kickPlayerName, msgData.roomId), OnClickOkAcceptBeKickedOut);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3KickOut);
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

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.Pk3v3)
                    {
                        return;
                    }
                }
            }

            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
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
            SendCreateRoomReq(RoomType.ROOM_TYPE_MELEE);
        }

        public void SendCreateRoomReq(RoomType roomtype)
        {
            Pk3v3RoomSettingData setdata = null;

            if (!roomSettingData.TryGetValue(roomtype, out setdata))
            {
                //                 Logger.LogErrorFormat("3v3 {0} setting data is null", roomtype);
                roomSettingData.TryGetValue(RoomType.ROOM_TYPE_THREE_FREE, out setdata);
            }                   
            if(setdata == null)
            {
                return;
            }

            WorldUpdateRoomReq req = new WorldUpdateRoomReq();

            req.roomId = 0;
            req.roomType = (byte)roomtype;
            req.name = TR.Value("pk_create_room_name", PlayerBaseData.GetInstance().Name);
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

        public void SwitchToPk3v3Scene()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata != null && scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3)
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
                    targetSceneID = 5006,
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
                SystemNotifyManager.SysNotifyFloatingEffect("房间正在匹配中,无法进行操作");
                return true;
            }

            return false;
        }

        public static bool checkCanJump()
        {
            Logger.LogErrorFormat("checkCanJump");
            return true;
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

            if (HasInPk3v3Room())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("你已经在房间里了");
                return;
            }

            // 积分赛场景不支持点击娱乐模式房间链接
            if (Pk3v3CrossDataManager.GetInstance().CheckPk3v3CrossScence())
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

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.BUDO)
            {
                SystemNotifyManager.SystemNotify(9307);
                return;
            }

            if (iRoomid <= 0)
            {
                return;
            }

            SendJoinRoomReq((uint)iRoomid, RoomType.ROOM_TYPE_THREE_FREE, "", (uint)lStamp);
        }

        public static void AcceptJoinPk3v3MeleeRoomLink(string param)
        {
            var tokens = param.Split('|');
            if (null == tokens || tokens.Length != 2)
            {
                return;
            }
            int iRoomid = 0;
            long lStamp = 0;
            if (!int.TryParse(tokens[0], out iRoomid) || !long.TryParse(tokens[1], out lStamp))
            {
                return;
            }
#if UNITY_EDITOR
            Logger.LogErrorFormat("AcceptJoinPk3v3MeleeRoomLink roomId = {0} stamp = {1} realtime = {2}", iRoomid, lStamp, Utility.ToUtcTime2Local(lStamp).ToString("tt yyMMdd hh:mm:ss", Utility.cultureInfo));
#endif
            if (HasInPk3v3Room())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("你已经在房间里了");
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
            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.BUDO)
            {
                SystemNotifyManager.SystemNotify(9307);
                return;
            }
            if (iRoomid <= 0)
            {
                return;
            }
            SendJoinRoomReq((uint)iRoomid, RoomType.ROOM_TYPE_MELEE, "", (uint)lStamp);
        }
        public string GetRoomState(RoomStatus roomstatus)
        {
            if(roomstatus == RoomStatus.ROOM_STATUS_BATTLE || roomstatus == RoomStatus.ROOM_STATUS_MATCH || roomstatus == RoomStatus.ROOM_STATUS_READY)
            {
                return "<color=#FFD800FF>决斗中</color>";
            }
            else if(roomstatus == RoomStatus.ROOM_STATUS_OPEN)
            {
                return "<color=#E84A42FF>未开始</color>";
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
            else if(roomtype == RoomType.ROOM_TYPE_MELEE)
            {
                return "乱斗";
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
            return Random.Range(1000, 9999);
        }

        public string GetPk3v3LocalDataKey(RoomType roomType, string key)
        {
            return string.Format("{0}_3v3_{1}_{2}", PlayerBaseData.GetInstance().RoleID, roomType, key);
        }
    }
}
