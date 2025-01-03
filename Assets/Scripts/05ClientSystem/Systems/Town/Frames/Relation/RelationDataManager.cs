using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using System.Reflection;
///////删除linq
using ProtoTable;

namespace GameClient
{
    public enum WorldQueryPlayerType
    {
        WQPT_WATCH_PLAYER_INTO = 0,
        WQPT_FRIEND,
        WQPT_TEACHER,
        WQPT_Query_ON_SHELF_ITEM_OWNER_INFO,     //道具的拥有者的信息
    }

    /// <summary>
    /// 好友度区间名称数据
    /// </summary>
    public class FriendlyDegreesIntervalNameModel
    {
        public int minLevel { get; set; }
        public int maxLevel { get; set; }
        public string name { get; set; }
        public int addPercent { get; set; }
        public FriendlyDegreesIntervalNameModel(int minLevel,int maxLevel,string name,int addPercent)
        {
            this.minLevel = minLevel;
            this.maxLevel = maxLevel;
            this.name = name;
            this.addPercent = addPercent;
        }
    }
    public class RelationDataManager : DataManager<RelationDataManager>
    {
        #region var
        
        protected Dictionary<ulong, RelationData> m_relationsDict;
        protected Dictionary<RelationType, List<ulong>> m_typeRelations;
        protected List<InviteFriendData> m_inviteFriends;
        protected List<RelationData> m_recommand_teachres = new List<RelationData>();
        protected List<RelationData> m_recommand_pupils = new List<RelationData>();
        protected List<RelationData> m_apply_pupils = new List<RelationData>();
        protected List<RelationData> m_apply_teachers = new List<RelationData>();
        protected List<FriendlyDegreesIntervalNameModel> m_friendDegreesIntervalList = new List<FriendlyDegreesIntervalNameModel>();//好友度区间名称集合
        /// <summary>
        /// 好友切磋邀请列表
        /// </summary>
        protected List<SceneSyncRequest> m_friendsPlayInviteList = new List<SceneSyncRequest>();

        public List<SceneSyncRequest> FriendsPlayInviteList
        {
            get
            {
                return m_friendsPlayInviteList;
            }
        }

        public List<RelationData> ApplyPupils
        {
            get
            {
                return m_apply_pupils;
            }
        }

        public List<RelationData> ApplyTeachers
        {
            get
            {
                return m_apply_teachers;
            }
        }

        public List<RelationData> SearchedTeacherList
        {
            get
            {
                m_recommand_teachres.RemoveAll(x =>
                {
                    var rd = RelationDataManager.GetInstance().GetRelationByRoleID(x.uid);
                    return null != rd && (rd.type == (byte)RelationType.RELATION_DISCIPLE || rd.type == (byte)RelationType.RELATION_MASTER);
                });
                return m_recommand_teachres;
            }
        }

        public void SetQueryedTeacherInfo(RelationData info)
        {
            m_recommand_teachres.Clear();
            m_recommand_teachres.Add(info);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSearchedTeacherListChanged);
        }

        public List<RelationData> SearchedPupilList
        {
            get
            {
                m_recommand_pupils.RemoveAll(x =>
                {
                    var rd = RelationDataManager.GetInstance().GetRelationByRoleID(x.uid);
                    return null != rd && (rd.type == (byte)RelationType.RELATION_DISCIPLE || rd.type == (byte)RelationType.RELATION_MASTER);
                });
                return m_recommand_pupils;
            }
        }

        const int m_inviteFriendListNum = 30;
        const int m_priChatListNum = 50;
        private ulong m_curPriChatUid;

        //私聊玩家列表
        protected List<PrivateChatPlayerData> m_priChatList;
        #endregion
        
        public int GetFriendlyDegreesAddPercent(int intimacy)
        {
            for (int i = 0; i < m_friendDegreesIntervalList.Count; i++)
            {
                var data = m_friendDegreesIntervalList[i];
                if (intimacy >= data.minLevel && intimacy <= data.maxLevel)
                {
                    return data.addPercent;
                }

                if (i == m_friendDegreesIntervalList.Count - 1)
                {
                    if (intimacy > data.minLevel)
                    {
                        return data.addPercent;
                    }
                }
            }
            return 0;
        }

        public string GetFriendlyDegreesIntervalName(int intimacy)
        {
            for (int i = 0; i < m_friendDegreesIntervalList.Count; i++)
            {
                var data = m_friendDegreesIntervalList[i];
                if (intimacy >= data.minLevel && intimacy <= data.maxLevel)
                {
                    return data.name;
                }

                if (i == m_friendDegreesIntervalList.Count - 1)
                {
                    if (intimacy > data.minLevel)
                    {
                        return data.name;
                    }
                }
            }
            return "";
        }
        void _InitFriendWelfareAddTable()
        {
            var table = TableManager.GetInstance().GetTable<FriendWelfareAddTable>().GetEnumerator();
            while (table.MoveNext())
            {
                var item = table.Current.Value as FriendWelfareAddTable;
                if (item == null)
                {
                    return;
                }

                int minLevel = 0;
                int maxLevel = 0;
                string name = "";
                int addPercent = 0;
                if (item.IntimacySpan.Length > 0)
                {
                    string[] levels = item.IntimacySpan.Split('|');
                    if (levels == null)
                    {
                        return;
                    }

                    int.TryParse(levels[0], out minLevel);
                    int.TryParse(levels[1], out maxLevel);
                    name = item.IntimacyName;
                    addPercent = item.ExpAddProb;
                }

                FriendlyDegreesIntervalNameModel model = new FriendlyDegreesIntervalNameModel(minLevel, maxLevel, name,addPercent);
                m_friendDegreesIntervalList.Add(model);
            }
        }

        #region net
        void _BindNetMessage()
        {
            NetProcess.AddMsgHandler(WorldSyncRelationList.MsgID, _OnSyncRelationList);
            NetProcess.AddMsgHandler(WorldUpdatePlayerOnlineRes.MsgID, OnRecvPlayerOnLineStatusChanged);

            NetProcess.AddMsgHandler(WorldSyncRelationData.MsgID, _OnSyncRelationData);
            NetProcess.AddMsgHandler(WorldNotifyNewRelation.MsgID, _OnNewRelation);
            NetProcess.AddMsgHandler(WorldNotifyDelRelation.MsgID, _OnDelRelation);
            NetProcess.AddMsgHandler(WorldRelationFindPlayersRet.MsgID, _OnRecommendFindRet);
            NetProcess.AddMsgHandler(WorldSyncOnOffline.MsgID, _OnSyncOffline);
            NetProcess.AddMsgHandler(SceneSyncRequest.MsgID, _OnSyncRequest);
            NetProcess.AddMsgHandler(WorldSetPlayerRemarkRes.MsgID, _OnSetPlayerRemarkRet);

            

        }

        void _UnBindNetMessage()
        {
            NetProcess.RemoveMsgHandler(WorldSyncRelationList.MsgID, _OnSyncRelationList);
            NetProcess.RemoveMsgHandler(WorldUpdatePlayerOnlineRes.MsgID, OnRecvPlayerOnLineStatusChanged);

            NetProcess.RemoveMsgHandler(WorldSyncRelationData.MsgID, _OnSyncRelationData);
            NetProcess.RemoveMsgHandler(WorldNotifyNewRelation.MsgID, _OnNewRelation);
            NetProcess.RemoveMsgHandler(WorldNotifyDelRelation.MsgID, _OnDelRelation);
            NetProcess.RemoveMsgHandler(WorldRelationFindPlayersRet.MsgID, _OnRecommendFindRet);
            NetProcess.RemoveMsgHandler(WorldSyncOnOffline.MsgID, _OnSyncOffline);
            NetProcess.RemoveMsgHandler(SceneSyncRequest.MsgID, _OnSyncRequest);
            NetProcess.RemoveMsgHandler(WorldSetPlayerRemarkRes.MsgID, _OnSetPlayerRemarkRet);
        }

        //[MessageHandle(WorldSyncRelationList.MsgID)]
        void _OnSyncRelationList(MsgDATA msg)
        {
            int pos = 0;
            byte type = 0;

            //BaseDLL.decode_int8(msg.bytes, ref pos, ref type);
            List<Relation> relations = RelationDecoder.DecodeList(msg.bytes, ref pos, msg.bytes.Length);
            for (int j = 0; j < relations.Count; ++j)
            {
                RelationData localData = _CreateLocalData(relations[j]);
                
                _AddRelation(localData);
                _UpdatePriChatListRelation(localData);
                type = relations[j].type;

                if (m_typeRelations.ContainsKey((RelationType)type) == false)
                {
                    m_typeRelations.Add((RelationType)type, new List<ulong>());
                }
                m_typeRelations[(RelationType)type].Add(localData.uid);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRelationChanged, localData,false);
            }
        }
        protected void _OnSyncRelationData(MsgDATA msg)
        {
            int pos = 0;
            Relation relation = RelationDecoder.DecodeData(msg.bytes, ref pos, msg.bytes.Length);
            RelationData localData = null;
            m_relationsDict.TryGetValue(relation.uid, out localData);
            if (localData == null)
            {
                Logger.LogErrorFormat("_OnSyncRelationData recv data is null! {0}", relation.uid);
            }
            else
            {
                _UpdateLocalData(relation, ref localData);
                _UpdatePriChatListRelation(localData);

                if(localData.type == (byte)RelationType.RELATION_MASTER ||
                    localData.type == (byte)RelationType.RELATION_DISCIPLE)
                {
                    TAPNewDataManager.GetInstance().RemoveQueryInfo(localData.uid);
                    TAPNewDataManager.GetInstance().RemoveApplyedPupil(localData.uid);
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRelationChanged, localData,false);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshFriendList);
        }

        protected void _OnNewRelation(MsgDATA msg)
        {
            int pos = 0;
            Relation relation = RelationDecoder.DecodeNew(msg.bytes, ref pos, msg.bytes.Length);
            RelationData localData = null;
            m_relationsDict.TryGetValue(relation.uid, out localData);
            if (localData == null)
            {
                localData = _CreateLocalData(relation);

                _AddRelation(localData);
                DelInviter(localData.uid);

                if (m_typeRelations.ContainsKey((RelationType)localData.type) == false)
                {
                    m_typeRelations.Add((RelationType)localData.type, new List<ulong>());
                }
                m_typeRelations[(RelationType)localData.type].Add(localData.uid);
                _UpdatePriChatListRelation(localData);

                if (localData.type == (byte)RelationType.RELATION_MASTER ||
                    localData.type == (byte)RelationType.RELATION_DISCIPLE)
                {
                    TAPNewDataManager.GetInstance().RemoveQueryInfo(localData.uid);
                    TAPNewDataManager.GetInstance().RemoveApplyedPupil(localData.uid);
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshFriendList);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRelationChanged, localData,false);
            }
            else
            {
                Logger.LogError("_OnNewRelation recv data is not new!");
            }
        }

        protected void _OnDelRelation(MsgDATA msg)
        {
            int pos = 0;
            WorldNotifyDelRelation msgRet = new WorldNotifyDelRelation();
            msgRet.decode(msg.bytes, ref pos);

            _RemoveRelation(msgRet.id);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshFriendList);
        }

        protected void _OnRecommendFindRet(MsgDATA msg)
        {
            int pos = 0;
            WorldRelationFindPlayersRet msgRet = new WorldRelationFindPlayersRet();
            msgRet.decode(msg.bytes, ref pos);
            RelationFindType eRelationFindType = (RelationFindType)msgRet.type;
            switch(eRelationFindType)
            {
                case RelationFindType.Friend:
                    {
                        RelationData[] recList = new RelationData[msgRet.friendList.Length];
                        for (int i = 0; i < msgRet.friendList.Length; ++i)
                        {
                            recList[i] = _CreateLocalData(msgRet.friendList[i]);
                        }

                        UIEventSystem.GetInstance().SendUIEvent(new UIEventRecievRecommendFriend(recList));
                    }
                    break;
                case RelationFindType.Master:
                    {
                        m_recommand_teachres.Clear();
                        for (int i = 0; i < msgRet.friendList.Length; ++i)
                        {
                            var teacher = _CreateLocalData(msgRet.friendList[i]);
                            if(null != teacher)
                            {
                                m_recommand_teachres.Add(teacher);
                            }
                        }
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSearchedTeacherListChanged);
                    }
                    break;
                case RelationFindType.Disciple:
                    {
                        m_recommand_pupils.Clear();
                        for (int i = 0; i < msgRet.friendList.Length; ++i)
                        {
                            var pupil = _CreateLocalData(msgRet.friendList[i]);
                            if (null != pupil)
                            {
                                m_recommand_pupils.Add(pupil);
                            }
                        }
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSearchedPupilListChanged);
                    }
                    break;
            }
        }

        protected void _OnSyncOffline(MsgDATA msg)
        {
            int pos = 0;
            WorldSyncOnOffline msgRet = new WorldSyncOnOffline();
            msgRet.decode(msg.bytes, ref pos);

            RelationData localData;
            m_relationsDict.TryGetValue(msgRet.id, out localData);
            if (localData == null)
            {
                Logger.LogWarning("_OnSyncOffline recv data is new!");
            }
            else
            {
                localData.isOnline = msgRet.isOnline;
                _UpdatePriChatListRelation(localData);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRelationChanged, localData,false);

                if (localData.type == (byte)RelationType.RELATION_MASTER ||
                    localData.type == (byte)RelationType.RELATION_DISCIPLE)
                {
                    SystemNotifyManager.SystemNotify((localData.isOnline == 0 ? 8924 : 8921),
                        RelationDataManager.GetRelationDesc((RelationType)localData.type),
                        localData.name);
                }
            }
        }

        protected void _OnSetPlayerRemarkRet(MsgDATA msg)
        {
            WorldSetPlayerRemarkRes msgRet = new WorldSetPlayerRemarkRes();
            msgRet.decode(msg.bytes);

            if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
            {
                var table = TableManager.GetInstance().GetTableItem<CommonTipsDesc>((int)msgRet.code);
                if (table != null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(table.Descs);
                }
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SetNoteNameSuccess);
            }
        }

        protected void _OnSyncRequest(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncRequest ==>> msg is nil");
                return;
            }

            SceneSyncRequest req = new SceneSyncRequest();
            req.decode(msg.bytes);

            switch ((RequestType)req.type)
            {
                case RequestType.JoinTeam:
                    break;
                case RequestType.RequestFriend:
                    {
                        AddInviteFriendNotify(req);

                        break;
                    }
                case RequestType.Request_Challenge_PK:
                    {
                        ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                        if (systemTown == null)
                        {
                            break;
                        }

                        CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                        if (scenedata == null)
                        {
                            break;
                        }

                        // 不在传统决斗场不接受挑战，3v3房间，武道大会，赏金联赛等等 都不接受邀请
                        if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.TRADITION)
                        {
                            break;
                        }

                        m_friendsPlayInviteList.Add(req);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3InviteRoomListUpdate);
                        //// 如果玩家正在看录像就不再接受挑战邀请
                        //if (ReplayServer.GetInstance().IsReplay())
                        //{
                        //    break;
                        //}

                        ////如果玩家正在自由练习不再接受挑战邀请
                        //if (BattleMain.battleType == BattleType.Training || BattleMain.battleType == BattleType.TrainingPVE)
                        //{
                        //    ReplyRequest(req, 0);
                        //    break;
                        //}

                        //string msgCtx = String.Format("是否接受玩家{0}的挑战请求！", req.requesterName);
                        //SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgCtx, () => { ReplyRequest(req, 1); }, () => { ReplyRequest(req, 0); });
                        break;
                    }
                case RequestType.InviteTeam:
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("邀请发送成功");
                        break;
                    }
                case RequestType.RequestMaster:
                    {
                        AddApplyPupilNotify(req);
                        break;
                    }
                case RequestType.RequestDisciple:
                    {
                        AddApplyTeacherNotify(req);
                        break;
                    }
                case RequestType.Request_Equal_PK:
                    FairDueleRequest(req);
                    break;
            }
        }
        /// <summary>
        /// 公平竞技场PK邀请提示面板
        /// </summary>
        /// <param name="req"></param>
        private void FairDueleRequest(SceneSyncRequest req)
        {
            if (req == null) return;
            ClientSystemTown systemTown2 = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown2 == null)
            {
                return;
            }

            CitySceneTable scenedata2 = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown2.CurrentSceneID);
            if (scenedata2 == null)
            {
                return;
            }

            // 不在公平竞技场，3v3房间，武道大会，赏金联赛等等 都不接受邀请
            if (scenedata2.SceneSubType != CitySceneTable.eSceneSubType.FairDuelPrepare)
            {
                return;
            }

            // 如果玩家正在看录像就不再接受挑战邀请
            if (ReplayServer.GetInstance().IsReplay())
            {
                return;
            }

            //如果玩家正在自由练习不再接受挑战邀请
            if (BattleMain.battleType == BattleType.Training || BattleMain.battleType == BattleType.TrainingPVE || BattleMain.battleType ==  BattleType.InputSetting|| BattleMain.battleType ==  BattleType.ChangeOccu)
            {
                ReplyRequest(req, 0);
                return;
            }

            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = string.Format(TR.Value("fairduel_pkfriend_content"), req.requesterName),
                IsShowNotify = false,
                LeftButtonText = TR.Value("fairduel_pkfriend_cancel"),
                RightButtonText = TR.Value("fairduel_pkfriend_ok"),
                OnRightButtonClickCallBack = () => { ReplyRequest(req, 1); },
                OnLeftButtonClickCallBack = () => { ReplyRequest(req, 0); }
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
            //  string msgCtx2 = String.Format("是否接受玩家{0}的挑战请求！", req.requesterName);
            // SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgCtx2, () => { ReplyRequest(req, 1); }, () => { ReplyRequest(req, 0); });

        }
        public void ReplyRequest(SceneSyncRequest req, byte reply)
        {
            SceneReply sendMsg = new SceneReply();
            sendMsg.type = req.type;
            sendMsg.requester = req.requester;
            sendMsg.result = reply;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);
        }

        #endregion

        #region public function
        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.RelationDataManager;
        }

        public override void Initialize()
        {
            Clear();

            _BindNetMessage();
            m_relationsDict = new Dictionary<ulong, RelationData>();
            m_typeRelations = new Dictionary<RelationType, List<ulong>>();
            m_inviteFriends = new List<InviteFriendData>();
            m_priChatList = new List<PrivateChatPlayerData>();
            _InitFriendWelfareAddTable();
        }

        public override void Clear()
        {
            _UnBindNetMessage();
            m_relationsDict = null;
            m_typeRelations = null;
            m_inviteFriends = null;
            m_priChatList = null;
            m_akQueriedIds.Clear();
            m_recommand_teachres.Clear();
            m_recommand_pupils.Clear();
            m_apply_pupils.Clear();
            m_apply_teachers.Clear();
            m_bHasNewApply = false;
            m_friendDegreesIntervalList.Clear();
            m_friendsPlayInviteList.Clear();
        }

        List<ulong> m_akQueriedIds = new List<ulong>();

        public void AddQueryInfo(ulong uid)
        {
            if(!m_akQueriedIds.Contains(uid))
            {
                m_akQueriedIds.Add(uid);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnQueryIntervalChanged, uid);
        }

        public void RemoveQueryInfo(ulong uid)
        {
            if(m_akQueriedIds.Contains(uid))
            {
                m_akQueriedIds.Remove(uid);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnQueryIntervalChanged, uid);
        }

        public void ClearQueryInfo()
        {
            m_akQueriedIds.Clear();
        }

        public bool CanQuery(ulong uid)
        {
            return !m_akQueriedIds.Contains(uid);
        }

        void _UpdatePriChatListRelation(RelationData rd)
        {
            var find = m_priChatList.Find(x => { return x.relationData.uid == rd.uid; });
            if(find != null)
            {
                find.relationData = rd;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdatePrivate);
        }

        public void OnAddPriChatList(RelationData rd, bool recv)
        {
//             RelationData data = GetRelationByRoleID(uid);
//             if (data == null)
//             {
//                 return;
//             }

            AddPriChatList(ref rd, recv);
        }

        public void SetCurPriChatUid(ulong uid)
        {
            m_curPriChatUid = uid;
        }

        public void MarkDirty(ulong uid,bool bDirty)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RoleChatDirtyChanged, uid, bDirty);
        }

        public bool GetPriDirty()
        {
            if(m_priChatList != null)
            {
                for (int i = 0; i < m_priChatList.Count; ++i)
                {

                    if (m_priChatList[i].chatNum > 0)
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        public bool GetFriendPriDirty()
        {
            if (m_priChatList != null)
            {
                for (int i = 0; i < m_priChatList.Count; ++i)
                {
                    if(m_priChatList[i].chatNum <= 0)
                    {
                        continue;
                    }

                    var curRelation = GetRelationByRoleID(m_priChatList[i].relationData.uid);
                    if(null == curRelation || curRelation.type != (int)RelationType.RELATION_FRIEND)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool GetPriDirtyByUid(ulong uid)
        {
            if (m_priChatList != null)
            {
                for (int i = 0; i < m_priChatList.Count; ++i)
                {
                    if(uid == m_priChatList[i].relationData.uid &&
                        m_priChatList[i].chatNum > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void ClearPriChatDirty(ulong uid)
        {
            m_curPriChatUid = uid;
            for (int i = 0; i < m_priChatList.Count; ++i)
            {
                if (m_priChatList[i].relationData.uid == uid)
                {
                    m_priChatList[i].chatNum = 0;
                    MarkDirty(uid, false);
                    return; 
                }
            }
        }

        public void DelPriChat(ulong uid)
        {
            var find = m_priChatList.Find(x => { return x.relationData.uid == uid; });
            if(find != null)
            {
                //m_priChatList.Remove(find);
                ChatRecordManager.GetInstance().RemoveChatRecords(PlayerBaseData.GetInstance().RoleID, uid);
                ChatManager.GetInstance().RemovePrivateChatData(uid);
                UIEventSystem.GetInstance().SendUIEvent(new UIEventDelPrivate(uid));
               
            }
        }

        public static string GetRelationDesc(RelationType eRelation)
        {
            switch(eRelation)
            {
                case RelationType.RELATION_BLACKLIST:
                    {
                        return TR.Value("relation_desc_black");
                    }
                case RelationType.RELATION_FRIEND:
                    {
                        return TR.Value("relation_desc_friend");
                    }
                case RelationType.RELATION_MASTER:
                    {
                        return TR.Value("relation_desc_teacher");
                    }
                case RelationType.RELATION_DISCIPLE:
                    {
                        return TR.Value("relation_desc_pupil");
                    }
                case RelationType.RELATION_STRANGER:
                    {
                        return TR.Value("relation_desc_stranger");
                    }
            }
            return TR.Value("relation_desc_none");
        }

        private void AddPriChatList(ref RelationData rd, bool recv)
        {
            if(rd == null)
            {
                return;
            }

            bool bFind = false;
            int idx = 0;
            for (int i = 0; i < m_priChatList.Count; ++i)
            {
                if (m_priChatList[i].relationData.uid == rd.uid)
                {
                    m_priChatList[i].relationData = rd;
                    if(recv == true)
                    {
                        m_priChatList[i].chatNum++;
                        MarkDirty(rd.uid, true);
                    }

                    bFind = true;
                    idx = i;
                    break;
                }
            }

            if (!bFind)
            {
                if (m_priChatList.Count >= m_priChatListNum)
                {
                    m_priChatList.RemoveAt(0);
                }

                PrivateChatPlayerData data = new PrivateChatPlayerData();
                data.relationData = rd;
                if(recv == true)
                {
                    data.chatNum++;
                    MarkDirty(rd.uid, true);
                }

                m_priChatList.Add(data);

                if(m_priChatList.Count >= m_priChatListNum)
                {
                    data.iOrder = m_priChatList[m_priChatList.Count - 2].iOrder + 1;
                }
                else
                {
                    data.iOrder = m_priChatList.Count;
                }
            }
            else
            {
                int iOrder = m_priChatList[m_priChatList.Count - 1].iOrder + 1;
                m_priChatList[idx].iOrder = iOrder;
                //swap
                var tmp = m_priChatList[idx];
                m_priChatList[idx] = m_priChatList[0];
                m_priChatList[0] = tmp;
            }

            if(recv && rd.uid != PlayerBaseData.GetInstance().RoleID)
            {
                UIEventSystem.GetInstance().SendUIEvent(new UIEventPrivateChat(rd, true));
            }
            
        }
        
        public List<PrivateChatPlayerData> GetPriChatList()
        {
            //for(int i = 0; i < m_priChatList.Count; ++i)
            //{
            //    if(m_priChatList != null && m_priChatList[i].relationData != null)
            //    {
            //        var targetRelation = RelationDataManager.GetInstance().GetRelationByRoleID(m_priChatList[i].relationData.uid);
            //        if(targetRelation != null)
            //        {
            //            m_priChatList[i].relationData = targetRelation;
            //        }
            //        else
            //        {
            //            m_priChatList[i].relationData.type = (byte)RelationType.RELATION_STRANGER;
            //        }
            //    }
            //}
            //for (int i = 0; i < m_priChatList.Count; i++)
            //{
            //    if (m_priChatList[i].relationData.type == (byte)RelationType.RELATION_BLACKLIST)
            //    {
            //        m_priChatList.Remove(m_priChatList[i]);
            //    }
            //}
            return m_priChatList;
        }

        public bool HasTeacher()
        {
            var datas = GetRelation((byte)RelationType.RELATION_MASTER);
            if(null != datas && datas.Count > 0)
            {
                return null != datas[0];
            }
            return false;
        }

        public RelationData GetTeacher()
        {
            var datas = GetRelation((byte)RelationType.RELATION_MASTER);
            if (null != datas && datas.Count > 0)
            {
                return datas[0];
            }
            return null;
        }

        public void RemoveApplyPupil(ulong guid)
        {
            var find = m_apply_pupils.Find((x) =>
            {
                return (x as RelationData).uid == guid;
            });

            if (null != find)
            {
                m_apply_pupils.Remove(find);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnApplyPupilListChanged);
            }
        }

        public void RemoveApplyTeacher(ulong guid)
        {
            var find = m_apply_teachers.Find((x) =>
            {
                return (x as RelationData).uid == guid;
            });

            if(null != find)
            {
                m_apply_teachers.Remove(find);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnApplyTeacherListChanged);
            }
        }

        public void AcceptApplyPupils(ulong uid)
        {
            SceneReply sendMsg = new SceneReply();
            sendMsg.type = (byte)RequestType.RequestMaster;
            sendMsg.requester = uid;
            sendMsg.result = 1;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);
        }

        public static void AcceptApplyTeachers(string param)
        {
            if(string.IsNullOrEmpty(param))
            {
                return;
            }

            ulong guid = 0;
            if(!ulong.TryParse(param,out guid))
            {
                return;
            }

            RelationDataManager.GetInstance()._AcceptApplyTeachers(guid);
        }

        public void _AcceptApplyTeachers(ulong uid)
        {
            SceneReply sendMsg = new SceneReply();
            sendMsg.type = (byte)RequestType.RequestDisciple;
            sendMsg.requester = uid;
            sendMsg.result = 1;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);
        }

        public void RefuseApplyPupils(ulong[] uids)
        {
            if (null != uids)
            {
                for(int i = 0; i < uids.Length; ++i)
                {
                    SceneReply sendMsg = new SceneReply();
                    sendMsg.type = (byte)RequestType.RequestDisciple;
                    sendMsg.requester = uids[i];
                    sendMsg.result = 0;
                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);
                }
            }
        }

        public void RefuseApplyTeachers(ulong[] uids)
        {
            if (null != uids)
            {
                for (int i = 0; i < uids.Length; ++i)
                {
                    SceneReply sendMsg = new SceneReply();
                    sendMsg.type = (byte)RequestType.RequestMaster;
                    sendMsg.requester = uids[i];
                    sendMsg.result = 0;
                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);
                }
            }
        }

        public void RefuseAllApplyPupils()
        {
            List<ulong> uids = GamePool.ListPool<ulong>.Get();
            for (int i = 0; i < m_apply_pupils.Count; ++i)
            {
                var current = m_apply_pupils[i] as RelationData;
                if (null != current)
                {
                    uids.Add(current.uid);
                }
            }
            RefuseApplyPupils(uids.ToArray());
            GamePool.ListPool<ulong>.Release(uids);
            m_apply_pupils.Clear();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnApplyPupilListChanged);
        }
        public void RefuseAllApplyTeachers()
        {
            List<ulong> uids = GamePool.ListPool<ulong>.Get();
            for (int i = 0; i < m_apply_teachers.Count; ++i)
            {
                var current = m_apply_teachers[i] as RelationData;
                if (null != current)
                {
                    uids.Add(current.uid);
                }
            }
            RefuseApplyTeachers(uids.ToArray());
            GamePool.ListPool<ulong>.Release(uids);
            m_apply_teachers.Clear();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnApplyTeacherListChanged);
        }

        public void MakeDebugSearchTeacherListData()
        {
            SearchedTeacherList.Clear();
            SearchedTeacherList.Add(new RelationData
            {
                uid = 15888,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "make a nice life,forget passed life!",
            });

            SearchedTeacherList.Add(new RelationData
            {
                uid = 15889,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "how pretty the dog is !",
            });

            SearchedTeacherList.Add(new RelationData
            {
                uid = 15890,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "what a nice girl!",
            });
        }

        public void MakeDebugApplyPupilDatas()
        {
            m_apply_pupils.Clear();
            m_apply_pupils.Add(new RelationData
            {
                uid = 15888,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "make a nice life,forget passed life!",
            });

            m_apply_pupils.Add(new RelationData
            {
                uid = 15889,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "how pretty the dog is !",
            });

            m_apply_pupils.Add(new RelationData
            {
                uid = 15890,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "what a nice girl!",
            });
        }

        public void MakeDebugSearchedPupilListData()
        {
            m_recommand_pupils.Clear();
            m_recommand_pupils.Add(new RelationData
            {
                uid = 15888,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "make a nice life,forget passed life!",
            });

            m_recommand_pupils.Add(new RelationData
            {
                uid = 15889,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "how pretty the dog is !",
            });

            m_recommand_pupils.Add(new RelationData
            {
                uid = 15890,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
                announcement = "what a nice girl!",
            });
        }

        public void MakeDebugPupilDatas()
        {
            _AddRelation(new RelationData
            {
                uid = 15888,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
            });

            _AddRelation(new RelationData
            {
                uid = 15889,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
            });

            _AddRelation(new RelationData
            {
                uid = 15890,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
            });

            _AddRelation(new RelationData
            {
                uid = 15888,
                name = PlayerBaseData.GetInstance().Name,
                seasonLv = (uint)SeasonDataManager.GetInstance().seasonLevel,
                level = (ushort)PlayerBaseData.GetInstance().Level,
                occu = (byte)PlayerBaseData.GetInstance().JobTableID,
                vipLv = (byte)PlayerBaseData.GetInstance().VipLevel,
                type = (byte)RelationType.RELATION_DISCIPLE,
                status = 0,
            });
        }

        public List<RelationData> GetRelation(byte type)
        {
            List<RelationData> relationList = new List<RelationData>();

            if (m_relationsDict != null)
            {
                var m_relationsDictEnum = m_relationsDict.GetEnumerator();
                while (m_relationsDictEnum.MoveNext())
                {
                    RelationData data = m_relationsDictEnum.Current.Value;
                    if (data != null)
                    {
                        if (data.type != type)
                        {
                            continue;
                        }
                        else
                        {
                            relationList.Add(data);
                        }
                    }
                }
            }

            relationList.Sort(SortList);
            
            return relationList;
        }

        /// <summary>
        /// 查找组队玩家是否是我的好友
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool FindPlayerIsRelation(ulong uid,ref RelationData relationData)
        {
            var datas_friends = GetRelation((byte)RelationType.RELATION_FRIEND);
            var datas_teachers = GetRelation((byte)RelationType.RELATION_MASTER);
            var datas_pupils = GetRelation((byte)RelationType.RELATION_DISCIPLE);
            List<RelationData> kRelationDtatas = new List<RelationData>();
            kRelationDtatas.AddRange(datas_teachers);
            kRelationDtatas.AddRange(datas_pupils);
            kRelationDtatas.AddRange(datas_friends);

            for (int i = 0; i < kRelationDtatas.Count; i++)
            {
                if (kRelationDtatas[i].uid != uid)
                {
                    continue;
                }

                relationData = kRelationDtatas[i];
                return true;
            }

            return false;
        }

        private int SortList(RelationData a, RelationData b)
        {
            if(a.isOnline > b.isOnline)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public RelationData GetRelationByRoleID(ulong uid)
        {
            RelationData data = null;

            if(m_relationsDict != null)
            {
                if (m_relationsDict.TryGetValue(uid, out data))
                {
                    return data;
                }
            }

            return data;
        }

        public void AddInviteFriendNotify(SceneSyncRequest req)
        {
            InviteFriendData inviteFriend = new InviteFriendData();
            inviteFriend.requester = req.requester;
            inviteFriend.requesterName = req.requesterName;
            inviteFriend.requesterLevel = req.requesterLevel;
            inviteFriend.requesterOccu = req.requesterOccu;
            inviteFriend.vipLv = req.requesterVipLv;

            bool bFind = false;
            for (int i = 0; i < m_inviteFriends.Count; ++i)
            {
                if (m_inviteFriends[i].requester == inviteFriend.requester)
                {
                    m_inviteFriends[i] = inviteFriend;
                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                if (m_inviteFriends.Count >= m_inviteFriendListNum)
                {
                    m_inviteFriends.RemoveAt(0);
                }
                m_inviteFriends.Add(inviteFriend);
            }

            //好友邀请消息，临时添加
//             NewMessageNoticeManager.GetInstance().AddNewMessageNoticeWhenNoExist("InviteFriendNotify",null,
//                 data=>{
//                             var frame = ClientSystemManager.GetInstance().OpenFrame<RelationFrame>(FrameLayer.Middle) as RelationFrame;
//                                 frame.ChangeTabChange(1);                          
//                                 NewMessageNoticeManager.GetInstance().RemoveNewMessageNotice(data);
//                       });

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FriendRequestNoticeUpdate); 

            //通知UI
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshInviteList);
        }

        bool m_bHasNewApply = false;
        public bool HasNewApply
        {
            get
            {
                return m_bHasNewApply;
            }
        }
        public void AddApplyPupilNotify(SceneSyncRequest req)
        {
            RelationData inviteFriend = new RelationData();
            inviteFriend.uid = req.requester;
            inviteFriend.name = req.requesterName;
            inviteFriend.level = req.requesterLevel;
            inviteFriend.occu = req.requesterOccu;
            inviteFriend.vipLv = req.requesterVipLv;
            inviteFriend.type = (byte)RelationType.RELATION_STRANGER;

            inviteFriend.avatar = req.avatar;
            inviteFriend.activeTimeType = req.activeTimeType;
            inviteFriend.masterType = req.masterType; 
            inviteFriend.regionId = req.regionId;
            inviteFriend.declaration = req.declaration;
            //inviteFriend.dailyTaskState = req.

            bool bFind = false;
            for (int i = 0; i < m_apply_pupils.Count; ++i)
            {
                if (m_apply_pupils[i].uid == inviteFriend.uid)
                {
                    m_apply_pupils[i] = inviteFriend;
                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                if (m_apply_pupils.Count >= m_inviteFriendListNum)
                {
                    m_apply_pupils.RemoveAt(0);
                }
                m_apply_pupils.Add(inviteFriend);
                m_bHasNewApply = true;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnNewPupilApplyRecieved);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnApplyPupilListChanged);
        }
        public void AddApplyTeacherNotify(SceneSyncRequest req)
        {
            RelationData inviteFriend = new RelationData();
            inviteFriend.uid = req.requester;
            inviteFriend.name = req.requesterName;
            inviteFriend.level = req.requesterLevel;
            inviteFriend.occu = req.requesterOccu;
            inviteFriend.vipLv = req.requesterVipLv;
            inviteFriend.type = (byte)RelationType.RELATION_STRANGER;

            inviteFriend.avatar = req.avatar;
            inviteFriend.activeTimeType = req.activeTimeType;
            inviteFriend.masterType = req.masterType;
            inviteFriend.regionId = req.regionId;
            inviteFriend.declaration = req.declaration;

            //string playerLink = string.Format("{{P {0} {1} {2} {3}}}", inviteFriend.uid, inviteFriend.name, inviteFriend.occu, inviteFriend.level);

            ChatManager.GetInstance().AddAskForPupilInvite(inviteFriend,TR.Value("tap_invite_msg",inviteFriend.uid));

            bool bFind = false;
            for (int i = 0; i < m_apply_teachers.Count; ++i)
            {
                if (m_apply_teachers[i].uid == inviteFriend.uid)
                {
                    m_apply_teachers[i] = inviteFriend;
                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                if (m_apply_teachers.Count >= m_inviteFriendListNum)
                {
                    m_apply_teachers.RemoveAt(0);
                }
                m_apply_teachers.Add(inviteFriend);
                m_bHasNewApply = true;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnNewTeacherApplyRecieved);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnApplyTeacherListChanged);
        }

        public void RemoveApplyPupilNotify()
        {
            m_bHasNewApply = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnNewPupilApplyRecieved);
        }

        public int maxFriendCount
        {
            get
            {
                int value = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TAP_SYSTEM_FRIEND_MAX_COUNT);
                if(null != systemValue)
                {
                    value = systemValue.Value;
                }
                return value;
            }
        }

        public List<InviteFriendData> GetInviteFriendData()
        {
            return m_inviteFriends;
        }

        public void DelInviter(ulong uid)
        {
            for (int i = 0; i < m_inviteFriends.Count; ++i)
            {
                if (m_inviteFriends[i].requester == uid)
                {
                    m_inviteFriends.RemoveAt(i);
                    break;
                }
            }


            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshInviteList);

        }

        public void DelAllInviter()
        {
            m_inviteFriends.Clear();
        }

        public void OnPrivateChat(RelationData data)
        {
            RelationData rd = GetRelationByRoleID(data.uid);
            if(rd == null)
            {
                rd = data;
                rd.type = (byte)RelationType.RELATION_STRANGER;
            }

            OnAddPriChatList(rd, false);

            ChatManager.GetInstance().OpenPrivateChatFrame(data);
        }

        public void AddRelationByID(ulong uid, RequestType eRequestType)
        {
            switch(eRequestType)
            {
                case RequestType.RequestMaster:
                case RequestType.RequestDisciple:
                    {
                        SceneRequest req = new SceneRequest();
                        req.type = (byte)eRequestType;
                        req.targetName = "";
                        req.target = uid;
                        NetManager netMgr = NetManager.Instance();
                        netMgr.SendCommand(ServerType.GATE_SERVER, req);
                    }
                    break;
                default:
                    Logger.LogErrorFormat("this request has not been suported !! {0}", eRequestType);
                    break;
            }
        }

        public void AddFriendByName(string name)
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_add_friend_need_lv", functionData.FinishLevel));
                return;
            }

            if (name != PlayerBaseData.GetInstance().Name)
            {
                SceneRequest req = new SceneRequest();
                req.type = (byte)RequestType.RequestFriendByName;
                req.targetName = name;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            }
        }

        /// <summary>
        /// 请求添加好友
        /// </summary>
        /// <param name="param"></param>
        public static void RequestAddRelation(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return;
            }

            ulong guid = 0;
            if (!ulong.TryParse(param, out guid))
            {
                return;
            }

            RelationDataManager.GetInstance().AddFriendByID(guid);
        }

        public void AddFriendByID(ulong uid)
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_add_friend_need_lv", functionData.FinishLevel));
                return;
            }

            if (uid != PlayerBaseData.GetInstance().RoleID)
            {
                SceneRequest req = new SceneRequest();
                req.type = (byte)RequestType.RequestFriend;
                req.target = uid;
                req.targetName = "";
                req.param = 0;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            }
            else
            {
                SystemNotifyManager.SystemNotify(3049);
            }
        }

        public void DelFriend(ulong uid)
        {
            RelationData data = GetRelationByRoleID(uid);
            if(data == null)
            {
                return;
            }

            if(data.type != (byte)RelationType.RELATION_FRIEND)
            {
                return;
            }

            WorldRemoveRelation req = new WorldRemoveRelation();
            req.type = (byte)RelationType.RELATION_FRIEND;
            req.uid = uid;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void DelRelation(ulong uid, RelationType eRelationType)
        {
            RelationData data = GetRelationByRoleID(uid);
            if (data == null)
            {
                return;
            }

            WorldRemoveRelation req = new WorldRemoveRelation();
            req.type = (byte)eRelationType;
            req.uid = uid;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void DelBlack(ulong uid)
        {
            RelationData data = GetRelationByRoleID(uid);
            if (data == null)
            {
                return;
            }

            if (data.type != (byte)RelationType.RELATION_BLACKLIST)
            {
                return;
            }

            WorldRemoveRelation req = new WorldRemoveRelation();
            req.type = (byte)RelationType.RELATION_BLACKLIST;
            req.uid = uid;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void AddBlackList(ulong uid)
        {
            RelationData data = GetRelationByRoleID(uid);

            //已有黑名单
            if (data != null && data.type == (byte)RelationType.RELATION_BLACKLIST)
            {
                SystemNotifyManager.SystemNotify(3118);
                return;
            }

            WorldAddToBlackList req = new WorldAddToBlackList();
            req.tarUid = uid;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void QueryPlayerOnlineStatus(ulong[] uids)
        {
            WorldUpdatePlayerOnlineReq kSend = new WorldUpdatePlayerOnlineReq();
            kSend.uids = uids;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }

        public void QueryPlayerOnlineStatus()
        {
            List<ulong> uids = GamePool.ListPool<ulong>.Get();
            uids.Clear();
            var list = GetPriChatList();
            for (int i = 0; i < list.Count; ++i)
            {
                uids.Add(list[i].relationData.uid);
            }
            QueryPlayerOnlineStatus(uids.ToArray());
            GamePool.ListPool<ulong>.Release(uids);
        }

        //[MessageHandle(WorldUpdatePlayerOnlineRes.MsgID)]
        void OnRecvPlayerOnLineStatusChanged(MsgDATA msg)
        {
            WorldUpdatePlayerOnlineRes res = new WorldUpdatePlayerOnlineRes();
            res.decode(msg.bytes);

            for(int i = 0; i < res.playerStates.Length; ++i)
            {
                var current = res.playerStates[i];
                var find = m_priChatList.Find(x => { return x.relationData.uid == current.uid; });
                if(find != null)
                {
                    find.relationData.isOnline = current.online;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPlayerOnLineStatusChanged);
        }
        #endregion

        #region private function

        protected void _AddPriChatPlayer()
        {

        }

        protected bool _AddRelation(RelationData data)
        {
            if (data != null && m_relationsDict.ContainsKey(data.uid) == false)
            {
                m_relationsDict.Add(data.uid, data);
                return true;
            }
            return false;
        }

        public void SendUpdateRelation()
        {
            WorldUpdateRelation req = new WorldUpdateRelation();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        protected bool _RemoveRelation(ulong uid)
        {
            RelationData data = null;
            m_relationsDict.TryGetValue(uid, out data);

            if (data != null)
            {
                m_relationsDict.Remove(uid);
                if(m_typeRelations.ContainsKey((RelationType)data.type))
                {
                    m_typeRelations[(RelationType)data.type].Remove(uid);
                }
                data.type = (byte)RelationType.RELATION_STRANGER;
                _UpdatePriChatListRelation(data);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRelationChanged, data,true);
                return true;
            }


            return false;
        }

        protected void _UpdateLocalData(Relation net, ref RelationData localData)
        {
            for (int i = 0; i < net.dirtyFields.Count; ++i)
            {
                if (net.dirtyFields[i] == (int)RelationAttr.RA_NAME)
                {
                    localData.name = net.name;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_SEASONLV)
                {
                    localData.seasonLv = net.seasonLv;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_LEVEL)
                {
                    localData.level = net.level;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_OCCU)
                {
                    localData.occu = net.occu;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_DAYGIFTNUM)
                {
                    localData.dayGiftNum = net.dayGiftNum;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_TYPE)
                {
                    if(m_typeRelations.ContainsKey((RelationType)localData.type))
                    {
                        ulong uid = localData.uid;
                        m_typeRelations[(RelationType)localData.type].RemoveAll(x =>
                        {
                            return x == uid;
                        });
                    }

                    if(!m_typeRelations.ContainsKey((RelationType)net.type))
                    {
                        m_typeRelations[(RelationType)net.type] = new List<ulong>();
                    }

                    m_typeRelations[(RelationType)net.type].Add(localData.uid);

                    localData.type = net.type;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_CREATETIME)
                {
                    localData.createTime = net.createTime;
                }
                else if(net.dirtyFields[i] == (int)RelationAttr.RA_STATUS)
                {
                    localData.status = net.status;
                }
                else if(net.dirtyFields[i] == (int)RelationAttr.RA_MASTER_DAYGIFTNUM)
                {
                    localData.tapDayGiftTimes = net.dailyGiftTimes;
                }
                else if(net.dirtyFields[i] == (int)RelationAttr.RA_MASTER_DAYGIFTNUM)
                {
                    localData.tapDayGiftTimes = net.dailyGiftTimes;
                }
                else if(net.dirtyFields[i] == (int)RelationAttr.RA_OFFLINE_TIME)
                {
                    localData.offlineTime = net.offlineTime;
                }
                else if(net.dirtyFields[i] == (int)RelationAttr.RA_DAILY_MASTERTASK_STATE)
                {
                    localData.dailyTaskState = net.dailyMasterTaskState;
                }
			    else if (net.dirtyFields[i] == (int)RelationAttr.RA_REMARKS)
                {
                    localData.remark = net.remark;
                }
                else if (net.dirtyFields[i]== (int)RelationAttr.RA_INTIMACY)
                {
                    localData.intimacy = net.intimacy;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_IS_REGRESS)
                {
                    localData.isRegress = net.isRegress;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_MARK)
                {
                    localData.mark = net.mark;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_HEAD_FRAME)
                {
                    localData.headFrame = net.headFrame;
                }
                else if (net.dirtyFields[i] == (int)RelationAttr.RA_RETURN_YEAR_TITLE)
                {
                    localData.returnYearTitle = net.returnYearTitle;
                }
            }
        }

        protected RelationData _CreateLocalData(Relation net)
        {
            RelationData localData = new RelationData();
            localData.uid = net.uid;
            localData.type = net.type;
            localData.name = net.name;
            localData.seasonLv = net.seasonLv;
            localData.level = net.level;
            localData.occu = net.occu;
            localData.dayGiftNum = net.dayGiftNum;
            localData.isOnline = net.isOnline;
            localData.createTime = net.createTime;
            localData.vipLv = net.vipLv;
            localData.status = net.status;
            localData.tapDayGiftTimes = net.dailyGiftTimes;
            localData.announcement = string.Empty;
            localData.offlineTime = net.offlineTime;
            localData.intimacy = net.intimacy;
			localData.remark = net.remark;
            localData.dailyTaskState = net.dailyMasterTaskState;
            localData.isRegress = net.isRegress;
            localData.mark = net.mark;
            localData.headFrame = net.headFrame;
            localData.returnYearTitle = net.returnYearTitle;
            return localData;
        }

        protected RelationData _CreateLocalData(QuickFriendInfo net)
        {
            RelationData localData = new RelationData();
            localData.uid = net.playerId;
            localData.name = net.name;
            localData.seasonLv = net.seasonLv;
            localData.level = net.level;
            localData.occu = net.occu;
            localData.vipLv = net.vipLv;
            localData.announcement = net.masterNote;
            localData.avatar = net.avatar;
            localData.activeTimeType = net.activeTimeType;
            localData.masterType = net.masterType;
            localData.regionId = net.regionId;
            localData.declaration = net.declaration;
            localData.playerLabelInfo = net.playerLabelInfo;
            return localData;
        }
        #endregion

    }
}
