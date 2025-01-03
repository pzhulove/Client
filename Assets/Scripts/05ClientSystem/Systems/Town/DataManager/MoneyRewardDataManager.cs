using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    //Events
    //OnMoneyRewardsStatusChanged
    public enum MoneyRewardsStatus
    {
        MRS_INVALID = -1,
        MRS_READY = 0,//t1
        MRS_8_RACE,//t1+40
        MRS_PRE_4_RACE,//t1 + 5 + 40
        MRS_4_RACE,//t1 + 5 + 40 + 5,
        MRS_2_RACE,//t1 + 5 + 40 + 5 + 5,
        MRS_RACE,//t1 + 5 + 40 + 5,
        MRS_END,
    }

    sealed class MoneyRewardsDataManager : DataManager<MoneyRewardsDataManager>
    {
        int m_iActiveID = 8888;
        public int ActiveID
        {
            get
            {
                return m_iActiveID;
            }
        }

        int m_iSceneID = -5005;
        public int SceneID
        {
            get
            {
                if (m_iSceneID > 0)
                {
                    return m_iSceneID;
                }

                var sceneTable = TableManager.GetInstance().GetTable<ProtoTable.CitySceneTable>();
                if (null != sceneTable)
                {
                    var enumerator = sceneTable.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var sceneItem = enumerator.Current.Value as CitySceneTable;
                        if (null != sceneItem && sceneItem.SceneSubType == CitySceneTable.eSceneSubType.MoneyRewards)
                        {
                            m_iSceneID = sceneItem.ID;
                            break;
                        }
                    }
                }

                if (m_iSceneID < 0)
                {
                    m_iSceneID = -m_iSceneID;
                }

                return m_iSceneID;
            }
        }

        public int MoneyID
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_MR_ADD_COST_MONEY_ID);
                if(null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        public int SecondMoneyID
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_PREMIUM_LEAGUE_REENROLL_COST_ITEM);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        public int EnrollItemID
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_PREMIUM_LEAGUE_ENROLL_REWARD_ITEM);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        public int EnrollCount
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_PREMIUM_LEAGUE_ENROLL_REWARD_ITEM_NUM);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        public int MaxAwardEachVS
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_PREMIUM_LEAGUE_TOTAL_RACE_MAX_REWARD_NUM);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        public int FixedAwardEachVS
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_PREMIUM_LEAGUE_EVERY_RACE_MAX_REWARD_NUM);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        public string MoneyIcon
        {
            get
            {
                var id = MoneyID;
                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
                if(null != item)
                {
                    return item.Icon;
                }
                return string.Empty;
            }
        }

        public bool IsMoneyEnough
        {
            get
            {
                int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(MoneyID);
                bool bEnough = iOwnedCount >= MoneyCount;
                return bEnough;
            }
        }

        public int MoneyCount
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_MR_ADD_COST_MONEY_COUNT);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        int _playerCount = 0;
        public int playerCount
        {
            get
            {
                return _playerCount;
            }
            private set
            {
                _playerCount = value;
            }
        }

        int _addPratyTimes = 0;
        public int addPartyTimes
        {
            get
            {
                return _addPratyTimes;
            }
            private set
            {
                _addPratyTimes = value;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsAddPartyTimesChanged);
            }
        }

        int _moneys = 0;
        public int moneysInPool
        {
            private set
            {
                _moneys = value;
            }
            get
            {
                return _moneys;
            }
        }

        int _vsAwards = 0;
        public int vsAwards
        {
            get
            {
                return _vsAwards;
            }
            private set
            {
                _vsAwards = value;
            }
        }

        public int needLevel
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_MR_MIN_PLAYER_LEVEL);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        public string MoneyName
        {
            get
            {
                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(MoneyID);
                if(null != item)
                {
                    return item.Name;
                }
                return string.Empty;
            }
        }

        public string SecondMoneyName
        {
            get
            {
                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(SecondMoneyID);
                if (null != item)
                {
                    return item.Name;
                }
                return string.Empty;
            }
        }

        public int secondMatchCost
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PREMIUM_LEAGUE_REENROLL_COST);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        public bool IsSecondMoneyEnough
        {
            get
            {
                int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(SecondMoneyID);
                bool bEnough = iOwnedCount >= secondMatchCost;
                return bEnough;
            }
        }

        public bool bHasParty
        {
            get
            {
                return _addPratyTimes > 0;
            }
        }

        public bool bCanAddOnceParty
        {
            get
            {
                return _addPratyTimes == 1;
            }
        }

        int iDefChampAward = -1;
        public int DefChampAward
        {
            get
            {
                if(-1 != iDefChampAward)
                {
                    return iDefChampAward;
                }
                iDefChampAward = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PREMIUM_LEAGUE_CHAMP_DEF_VALUE);
                if(null != systemValue)
                {
                    iDefChampAward = systemValue.Value;
                }
                return iDefChampAward;
            }
        }

        public int ChampAward
        {
            get
            {
                int iValue = 0;
                if (MoneyRewardsDataManager.GetInstance().awardsList.Count > 0)
                {
                    iValue = MoneyRewardsDataManager.GetInstance().awardsList[0];
                }
                int iDefValue = DefChampAward;
                if(iValue <= 0)
                {
                    iValue = iDefValue;
                }
                return iValue;
            }
        }

        List<int> _awardsList = new List<int>();
        public List<int> awardsList
        {
            get
            {
                return _awardsList;
            }
        }

        int[] _stages = new int[4] { 0, 0, 0, 0 };

        int _winTimes = 0;
        public int WinTimes
        {
            private set
            {
                _winTimes = value;
            }
            get
            {
                return _winTimes;
            }
        }

        int _loseTimes = 0;
        public int LoseTime
        {
            private set
            {
                _loseTimes = value;
            }
            get
            {
                return _loseTimes;
            }
        }

        int _rank = 0;
        public int Rank
        {
            set
            {
                _rank = value;
            }
            get
            {
                return _rank;
            }
        }

        int _score = 0;
        public int Score
        {
            set
            {
                _score = value;
            }
            get
            {
                return _score;
            }
        }

        int _maxScore = 0;
        public int MaxScore
        {
            set
            {
                _maxScore = value;
            }
            get
            {
                return _maxScore;
            }
        }

        public bool CanMatch
        {
            get
            {
                return Status == PremiumLeagueStatus.PLS_PRELIMINAY;
            }
        }

        public int matchMaxTimes
        {
            get
            {
                int iValue = 0;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PREMIUM_LEAGUE_ENROLL_MAX_COUNT);
                if (null != systemValue)
                {
                    iValue = systemValue.Value;
                }
                return iValue;
            }
        }

        MoneyRewaradsRankItemData[] _rankItems = new MoneyRewaradsRankItemData[100];
        public MoneyRewaradsRankItemData[] RankItems
        {
            get
            {
                return _rankItems;
            }
        }

        MoneyRewaradsRankItemData _rankItemSelf = new MoneyRewaradsRankItemData();
        public MoneyRewaradsRankItemData RankItemSelf
        {
            get
            {
                return _rankItemSelf;
            }
        }

        void _clearSelfRank()
        {
            _rankItemSelf.name = string.Empty;
            _rankItemSelf.score = 0;
            _rankItemSelf.maxScore = 0;
            _rankItemSelf.rank = 999;
        }

        public void ClearRankItems()
        {
            for (int i = 0; i < _rankItems.Length; ++i)
            {
                _rankItems[i] = null;
            }
        }

        public enum RecordDownLoadType
        {
            RDT_INIT = 0,
            RDT_LOADING,
            RDT_FINISH,
            RDT_FAILED,
        }

        public class RecordDownLoad
        {
            public UInt64 a_raceId;
            public UnityAction<UInt64> cb;
            public RecordDownLoadType eRecordDownLoadType = RecordDownLoadType.RDT_INIT;
            public string sessionID;
            public ReplayWaitHttpRequest req;
        };
        List<RecordDownLoad> _records = new List<RecordDownLoad>();
        public bool isRcdInQueue(UInt64 a_raceId)
        {
            for(int i = 0; i < _records.Count; ++i)
            {
                if(null != _records[i] && _records[i].a_raceId == a_raceId)
                {
                    return true;
                }
            }
            return false;
        }

        public void downloadRcd(UInt64 a_raceId, UnityAction<UInt64> cb)
        {
            if(!isRcdInQueue(a_raceId))
                _records.Add(new RecordDownLoad { a_raceId = a_raceId, cb = cb });
        }

        public static void ShowErrorNotify(ReplayErrorCode code)
        {
            if (code == ReplayErrorCode.FILE_NOT_FOUND)
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("pk_video_file_not_found"));
            else if (code == ReplayErrorCode.VERSION_NOT_MATCH)
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("pk_video_version_not_match"));
            else if (code == ReplayErrorCode.DOWNLOAD_FAILED)
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("pk_video_download_failed"));
            else if (code == ReplayErrorCode.HAS_TEAM)
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("pk_video_has_team"));
        }

        public override void Update(float a_fTime)
        {
            bool bFlag = false;
            for(int i = 0; i < _records.Count; ++i)
            {
                var record = _records[i];
                if(null != record)
                {
                    switch(record.eRecordDownLoadType)
                    {
                        case RecordDownLoadType.RDT_INIT:
                            {
                                record.sessionID = record.a_raceId.ToString();
                                record.req = new ReplayWaitHttpRequest(record.sessionID);
                                record.eRecordDownLoadType = RecordDownLoadType.RDT_LOADING;
                            }
                            break;
                        case RecordDownLoadType.RDT_LOADING:
                            {
                                if(!record.req.MoveNext())
                                {
                                    var result = record.req.GetResult();
                                    if (result == BaseWaitHttpRequest.eState.Success)
                                    {
                                        var contents = record.req.GetResultBytes();
                                        if (contents == null || contents.Length <= 0)
                                        {
                                            ShowErrorNotify(ReplayErrorCode.DOWNLOAD_FAILED);
                                            record.eRecordDownLoadType = RecordDownLoadType.RDT_FAILED;
                                        }
                                        else
                                        {
                                            var decompressedContents = contents;//CompressHelper.Uncompress(contents, contents.Length);

                                            RecordData.SaveReplayFile(record.sessionID, decompressedContents, decompressedContents.Length);

                                            decompressedContents = null;
                                            contents = null;

                                            record.eRecordDownLoadType = RecordDownLoadType.RDT_FINISH;

                                            if (null != record.cb)
                                            {
                                                record.cb.Invoke(record.a_raceId);
                                            }
                                        }
                                    }
                                    else if (result == BaseWaitHttpRequest.eState.TimeOut)
                                    {
                                        record.eRecordDownLoadType = RecordDownLoadType.RDT_FAILED;
                                        ShowErrorNotify(ReplayErrorCode.DOWNLOAD_FAILED);
                                    }
                                    else if (result == BaseWaitHttpRequest.eState.Error)
                                    {
                                        record.eRecordDownLoadType = RecordDownLoadType.RDT_FAILED;
                                        ShowErrorNotify(ReplayErrorCode.DOWNLOAD_FAILED);
                                    }

                                    bFlag = true;
                                }
                            }
                            break;
                    }
                }
            }

            _records.RemoveAll(x =>
            {
                return null == x || (x.eRecordDownLoadType == RecordDownLoadType.RDT_FAILED ||
                x.eRecordDownLoadType == RecordDownLoadType.RDT_FINISH);
            });

            if(bFlag)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsRcdStatusChanged);
            }
        }

        public int getValidRankCount
        {
            get
            {
                int iCnt = 0;
                for(int i = 0; i < _rankItems.Length; ++i)
                {
                    if(null != _rankItems[i])
                    {
                        ++iCnt;
                    }
                    else
                    {
                        break;
                    }
                }
                return iCnt;
            }
        }

        public void TryCallEnterMsgBox(int iMsgID,UnityEngine.Events.UnityAction onOk)
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            if(null != onOk)
            {
                onOk.Invoke();
            }

            SystemNotifyManager.SystemNotify(iMsgID,()=>
            {
                if(ClientSystemManager.GetInstance().IsFrameOpen<MoneyRewardsEnterFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<MoneyRewardsEnterFrame>();
                }
                MoneyRewardsDataManager.GetInstance().GotoPvpFight();
            });
        }

        public void SendAddParty(UnityAction callback)
        {
            SendAddMoneyRewards();

            WaitNetMessageManager.GetInstance().Wait<WorldPremiumLeagueEnrollRes>(msgRet =>
            {
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    if(null != callback)
                    {
                        callback.Invoke();
                    }
                  //  Logger.LogErrorFormat("报名成功!");
                }
            });
        }

        public void SendAddMoneyRewards()
        {
            WorldPremiumLeagueEnrollReq kSend = new WorldPremiumLeagueEnrollReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER,kSend);
        }

        //[MessageHandle(WorldPremiumLeagueEnrollRes.MsgID)]
        void OnRecvWorldPremiumLeagueEnrollRes(MsgDATA msg)
        {
            WorldPremiumLeagueEnrollRes recv = new WorldPremiumLeagueEnrollRes();
            recv.decode(msg.bytes);

            if(recv.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)recv.result);
            }
            else
            {
               // Logger.LogErrorFormat("报名成功!");
            }
        }

        //[MessageHandle(WorldPremiumLeagueSelfInfo.MsgID)]
        void OnRecvWorldPremiumLeagueSelfInfo(MsgDATA msg)
        {
            WorldPremiumLeagueSelfInfo recv = new WorldPremiumLeagueSelfInfo();
            recv.decode(msg.bytes);

            //Rank = (int)recv.info.ranking;
            Score = (int)recv.info.score;
            //MaxScore = (int)recv.info.score;//TODO:MAX_RANK_SCORE
            WinTimes = (int)recv.info.winNum;
            LoseTime = (int)recv.info.loseNum;
            addPartyTimes = (int)recv.info.enrollCount;
            vsAwards = (int)recv.info.preliminayRewardNum;

            //_rankItemSelf.rank = Rank;
            _rankItemSelf.score = Score;
            _rankItemSelf.maxScore = MaxScore;
            _rankItemSelf.name = PlayerBaseData.GetInstance().Name;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsSelfResultChanged);
        }

        public override void Initialize()
        {
            RegisterNetHandler();
            InvokeMethod.InvokeInterval(this, 3.0f, 1.0f, 999999.0f, null,_UpdateMsgHint, null);
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(WorldPremiumLeagueSelfInfo.MsgID, OnRecvWorldPremiumLeagueSelfInfo);
            NetProcess.AddMsgHandler(WorldPremiumLeagueBattleInfoInit.MsgID, OnRecvWorldPremiumLeagueBattleInfoInit);
            NetProcess.AddMsgHandler(WorldPremiumLeagueBattleInfoUpdate.MsgID, OnRecvWorldPremiumLeagueBattleInfoUpdate);
            NetProcess.AddMsgHandler(WorldPremiumLeagueSyncStatus.MsgID, OnRecvWorldPremiumLeagueSyncStatus);
            NetProcess.AddMsgHandler(WorldPremiumLeagueBattleGamerUpdate.MsgID, OnRecvWorldPremiumLeagueBattleGamerUpdate);
            NetProcess.AddMsgHandler(WorldPremiumLeagueBattleGamerInit.MsgID, OnRecvWorldPremiumLeagueBattleGamerInit);
            NetProcess.AddMsgHandler(WorldPremiumLeagueRewardPoolRes.MsgID, OnRecvWorldPremiumLeagueRewardPoolRes);
            NetProcess.AddMsgHandler(WorldPremiumLeagueBattleRecordSync.MsgID, OnRecvWorldPremiumLeagueBattleRecordSync);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(WorldPremiumLeagueSelfInfo.MsgID, OnRecvWorldPremiumLeagueSelfInfo);
            NetProcess.RemoveMsgHandler(WorldPremiumLeagueBattleInfoInit.MsgID, OnRecvWorldPremiumLeagueBattleInfoInit);
            NetProcess.RemoveMsgHandler(WorldPremiumLeagueBattleInfoUpdate.MsgID, OnRecvWorldPremiumLeagueBattleInfoUpdate);
            NetProcess.RemoveMsgHandler(WorldPremiumLeagueSyncStatus.MsgID, OnRecvWorldPremiumLeagueSyncStatus);
            NetProcess.RemoveMsgHandler(WorldPremiumLeagueBattleGamerUpdate.MsgID, OnRecvWorldPremiumLeagueBattleGamerUpdate);
            NetProcess.RemoveMsgHandler(WorldPremiumLeagueBattleGamerInit.MsgID, OnRecvWorldPremiumLeagueBattleGamerInit);
            NetProcess.RemoveMsgHandler(WorldPremiumLeagueRewardPoolRes.MsgID, OnRecvWorldPremiumLeagueRewardPoolRes);
            NetProcess.RemoveMsgHandler(WorldPremiumLeagueBattleRecordSync.MsgID, OnRecvWorldPremiumLeagueBattleRecordSync);
        }

        List<CLPremiumLeagueBattle> m_battles = new List<CLPremiumLeagueBattle>();

        //[MessageHandle(WorldPremiumLeagueBattleInfoInit.MsgID)]
        void OnRecvWorldPremiumLeagueBattleInfoInit(MsgDATA msg)
        {
            WorldPremiumLeagueBattleInfoInit recv = new WorldPremiumLeagueBattleInfoInit();
            recv.decode(msg.bytes);

            m_battles.Clear();
            for (int i = 0; i < recv.battles.Length; ++i)
            {
                m_battles.Add(recv.battles[i]);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsBattleInfoChanged);
        }

        //[MessageHandle(WorldPremiumLeagueBattleInfoUpdate.MsgID)]
        void OnRecvWorldPremiumLeagueBattleInfoUpdate(MsgDATA msg)
        {
            WorldPremiumLeagueBattleInfoUpdate recv = new WorldPremiumLeagueBattleInfoUpdate();
            recv.decode(msg.bytes);

            m_battles.RemoveAll(x => { return x.raceId == recv.battle.raceId; });
            m_battles.Add(recv.battle);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsBattleInfoChanged);

        //    Logger.LogErrorFormat("OnRecvWorldPremiumLeagueBattleInfoUpdate <color=#00ff00>race id = {0} type={1}</color>", recv.battle.raceId,recv.battle.type);
        }

        public CLPremiumLeagueBattle GetChampionRelationInfo()
        {
            for (int i = 0; i < m_battles.Count; ++i)
            {
                var battle = m_battles[i];
                if (null != battle && battle.raceId != 0)
                {
                    if(battle.type == (byte)PremiumLeagueStatus.PLS_FINAL)
                    {
                        if (null != battle.fighter1 && null != battle.fighter2)
                        {
                            return battle;
                        }
                    }
                }
            }

            return null;
        }

        public CLPremiumLeagueBattle GetRelationBattleInfo(ulong roleId, PremiumLeagueStatus ePremiumLeagueStatus)
        {
            //if(!(ePremiumLeagueStatus == PremiumLeagueStatus.PLS_FINAL_EIGHT || ePremiumLeagueStatus == PremiumLeagueStatus.PLS_FINAL_FOUR))
            //{
            //    return null;
            //}

            for(int i = 0; i < m_battles.Count; ++i)
            {
                var battle = m_battles[i];
                if(null != battle && battle.raceId != 0)
                {
                    if(null != battle.fighter1 && null != battle.fighter2)
                    {
                        if(battle.type == (byte)ePremiumLeagueStatus)
                        {
                            if(battle.fighter1.id == roleId || battle.fighter2.id == roleId)
                            {
                                return battle;
                            }
                        }
                    }
                }
            }

            return null;
        }

        void _resetAwards()
        {
            if (_awardsList.Count == 5)
            {
                for (int i = 0; i < _awardsList.Count; ++i)
                {
                    _awardsList[i] = 0;
                }
            }
            else
            {
                _awardsList.Clear();
                for (int i = 0; i < 5; ++i)
                {
                    _awardsList.Add(0);
                }
            }
        }

        public override void Clear()
        {
            _resetAwards();

            for (int i = 0; i < _rankItems.Length; ++i)
            {
                _rankItems[i] = null;
            }
            records.Clear();
            _totalRecordsCount = 0;
            activeInfo.status = (byte)PremiumLeagueStatus.PLS_INIT;
            for(int i = 0; i < resultDatas.Length; ++i)
            {
                resultDatas[i] = null;
            }
            moneysInPool = 0;
            Rank = 999;
            WinTimes = 0;
            LoseTime = 0;
            Score = 0;
            MaxScore = 0;
            vsAwards = 0;
            _clearSelfRank();

            m_battles.Clear();
            System.Array.Clear(_stages, 0, _stages.Length);

            InvokeMethod.RmoveInvokeIntervalCall(this);

            UnRegisterNetHandler();
        }

        void _OnConfirmToPvpFight()
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<MoneyRewardsEnterFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MoneyRewardsEnterFrame>();
            }

            MoneyRewardsDataManager.GetInstance().GotoPvpFight();
        }

       void _UpdateMsgHint()
        {
            if(addPartyTimes <= 0)
            {
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if(null == systemTown)
            {
                return;
            }

            if(systemTown.CurrentSceneID == SceneID)
            {
                return;
            }

            if(null != systemTown)
            {
                uint delta = MoneyRewardsDataManager.GetInstance().StatusEndTime >= TimeManager.GetInstance().GetServerTime() ? MoneyRewardsDataManager.GetInstance().StatusEndTime - TimeManager.GetInstance().GetServerTime() : 0;

                if (Status == PremiumLeagueStatus.PLS_ENROLL)
                {
                    if(_stages[0] == 0)
                    {
                        if(delta <= 300)
                        {
                            _stages[0] = 1;
                            SystemNotifyManager.SystemNotify(7024, _OnConfirmToPvpFight, null, new object[] { 0 });
                        }
                    }
                }
                else if (Status == PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE)
                {
                    if (_stages[1] == 0)
                    {
                        if (delta <= 300)
                        {
                            if(Rank <= 8 && Rank >= 1)
                            {
                                _stages[1] = 1;
                                var timeDesc = Utility.ToUtcTime2Local(MoneyRewardsDataManager.GetInstance().StatusEndTime).ToString("HH:mm", Utility.cultureInfo);
                                SystemNotifyManager.SystemNotify(7025, _OnConfirmToPvpFight, null, new object[] { timeDesc });
                            }
                        }
                    }
                }
                else if (Status == PremiumLeagueStatus.PLS_FINAL_EIGHT)
                {
                    if (_stages[2] == 0)
                    {
                        if (delta <= 60)
                        {
                            if(WinTimes == 1)
                            {
                                _stages[2] = 1;
                                var timeDesc = Utility.ToUtcTime2Local(MoneyRewardsDataManager.GetInstance().StatusEndTime).ToString("HH:mm", Utility.cultureInfo);
                                SystemNotifyManager.SystemNotify(7026, _OnConfirmToPvpFight, null, new object[] { timeDesc });
                            }
                        }
                    }
                }
                else if (Status == PremiumLeagueStatus.PLS_FINAL_FOUR)
                {
                    if (_stages[3] == 0)
                    {
                        if (delta <= 60)
                        {
                            if (WinTimes == 2)
                            {
                                _stages[3] = 1;
                                var timeDesc = Utility.ToUtcTime2Local(MoneyRewardsDataManager.GetInstance().StatusEndTime).ToString("HH:mm", Utility.cultureInfo);
                                SystemNotifyManager.SystemNotify(7027, _OnConfirmToPvpFight, null, new object[] { timeDesc });
                            }
                        }
                    }
                }
            }
        }

        PremiumLeagueStatusInfo activeInfo = new PremiumLeagueStatusInfo();
        public PremiumLeagueStatus Status
        {
            get
            {
                if(activeInfo.status >= (byte)PremiumLeagueStatus.PLS_INIT && activeInfo.status <= (byte)PremiumLeagueStatus.PLS_FINAL_WAIT_CLEAR)
                {
                    return (PremiumLeagueStatus)activeInfo.status;
                }
                return PremiumLeagueStatus.PLS_INIT;
            }
        }

        public uint StatusEndTime
        {
            get
            {
                return activeInfo.endTime;
            }
        }

        //[MessageHandle(WorldPremiumLeagueSyncStatus.MsgID)]
        void OnRecvWorldPremiumLeagueSyncStatus(MsgDATA msg)
        {
            activeInfo.decode(msg.bytes);

            if (activeInfo.status == (byte)PremiumLeagueStatus.PLS_INIT)
            {
                for (int i = 0; i < resultDatas.Length; ++i)
                {
                    resultDatas[i] = null;
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsResultChanged);

                m_battles.Clear();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsBattleInfoChanged);

                System.Array.Clear(_stages, 0, _stages.Length);
                vsAwards = 0;
            }

            NotifyInfo NoticeData = new NotifyInfo
            {
                type = (uint)NotifyType.NT_MONEY_REWARDS
            };

            if (needEnterance)
            {
                ActivityNoticeDataManager.GetInstance().AddActivityNotice(NoticeData);
                DeadLineReminderDataManager.GetInstance().AddActivityNotice(NoticeData);
            }
            else
            {
                ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(NoticeData);
                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(NoticeData);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsStatusChanged);
        }

        //[MessageHandle(WorldPremiumLeagueBattleGamerUpdate.MsgID)]
        void OnRecvWorldPremiumLeagueBattleGamerUpdate(MsgDATA msg)
        {
            WorldPremiumLeagueBattleGamerUpdate recv = new WorldPremiumLeagueBattleGamerUpdate();
            recv.decode(msg.bytes);

            for(int i = 0; i < resultDatas.Length; ++i)
            {
                if(null != resultDatas[i] && resultDatas[i].recordId == recv.roleId)
                {
                    resultDatas[i].winTimes = (int)recv.winNum;
                    resultDatas[i].losed = recv.isLose != 0;
                    break;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsResultChanged);
        }

        //[MessageHandle(WorldPremiumLeagueBattleGamerInit.MsgID)]
        void OnRecvWorldPremiumLeagueBattleGamerInit(MsgDATA msg)
        {
            WorldPremiumLeagueBattleGamerInit recv = new WorldPremiumLeagueBattleGamerInit();
            recv.decode(msg.bytes);

            for (int i = 0; i < resultDatas.Length; ++i)
            {
                resultDatas[i] = null;
            }

            for (int i = 0; i < recv.gamers.Length; ++i)
            {
                var current = recv.gamers[i];
                if(null != current)
                {
                    if(current.ranking >= 1 && current.ranking <= resultDatas.Length)
                    {
                        ComMoneyRewardsResultData data = new ComMoneyRewardsResultData
                        {
                            recordId = current.roleId,
                            name = current.name,
                            occu = current.occu,
                            winTimes = (int)current.winNum,
                            rank = (int)current.ranking,
                            losed = current.isLose != 0
                        };
                        resultDatas[current.ranking-1] = data;

                     //   Logger.LogErrorFormat("<color=#00ff00>roleId = {0},name = {1},occu = {2},winTimes ={3},rank={4}</color>", data.recordId, data.name, data.occu, data.winTimes, data.rank);
                    }
                    else
                    {
                        Logger.LogProcessFormat("ranking error = {0} must in range[1,8]", current.ranking);
                    }
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsResultChanged);
        }

        //[MessageHandle(WorldPremiumLeagueRewardPoolRes.MsgID)]
        void OnRecvWorldPremiumLeagueRewardPoolRes(MsgDATA msg)
        {
            WorldPremiumLeagueRewardPoolRes recv = new WorldPremiumLeagueRewardPoolRes();
            recv.decode(msg.bytes);

            moneysInPool = (int)recv.money;
            playerCount = (int)recv.enrollPlayerNum;

            _resetAwards();
            for (int i = 0; i < recv.rewards.Length && i < _awardsList.Count; ++i)
            {
                _awardsList[i] = (int)recv.rewards[i];
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsPoolsMoneyChanged);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsPlayerCountChanged);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsAwardListChanged);
        }

        int _totalRecordsCount = 0;
        public int TotalRecordsCount
        {
            get
            {
                return _totalRecordsCount;
            }

            private set
            {
                _totalRecordsCount = value;
            }
        }

        //[MessageHandle(WorldPremiumLeagueBattleRecordSync.MsgID)]
        void OnRecvWorldPremiumLeagueBattleRecordSync(MsgDATA msg)
        {
            WorldPremiumLeagueBattleRecordSync recv = new WorldPremiumLeagueBattleRecordSync();
            recv.decode(msg.bytes);

            _AddRecords(recv.record);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsRecordsChanged);
        }

        void _AddRecords(PremiumLeagueRecordEntry current)
        {
            if (null != current)
            {
                var find = Records.Find(x => { return x.iIndex == current.index; });
                if (null == find)
                {
                    var data = new ComMoneyRewardsRecordsData
                    {
                        iIndex = (int)current.index,
                        measured = false,
                        time = current.time,
                        srcId = current.winner.id,
                        tarId = current.loser.id,
                        srcName = current.winner.name,
                        tarName = current.loser.name,
                        srcBeatCount = current.winner.winStreak,
                        dstBeatCount = current.loser.winStreak,
                        scoreChanged = current.winner.gotScore
                    };
                    Records.Add(data);
                }
            }
        }

        public void RequestRecords(bool isSelf,int start,int count,UnityAction onAction)
        {
            WorldPremiumLeagueBattleRecordReq kSend = new WorldPremiumLeagueBattleRecordReq
            {
                isSelf = isSelf ? (byte)1 : (byte)0,
                startIndex = (uint)start,
                count = (uint)count
            };

            //     Logger.LogErrorFormat("<color=#00ff00>RequestRecords isSelf={0} start={1} count={2}</color>", isSelf, start, count);

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);

            WaitNetMessageManager.GetInstance().Wait<WorldPremiumLeagueBattleRecordRes>(recv =>
            {
                TotalRecordsCount = (int)recv.totalCount;
             //   Logger.LogErrorFormat("<color=#00ff00>WorldPremiumLeagueBattleRecordRes TotalRecordsCount={0} </color>", TotalRecordsCount);

                for (int i = 0; i < recv.records.Length; ++i)
                {
                    _AddRecords(recv.records[i]);
                }

                Records.Sort((x, y) =>
                {
                    return x.iIndex - y.iIndex;
                });

                if(null != onAction)
                {
                    onAction.Invoke();
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsRecordsChanged);
            });
        }

        ComMoneyRewardsResultData[] resultDatas = new ComMoneyRewardsResultData[8];
        public ComMoneyRewardsResultData[] ResultDatas
        {
            get
            {
                return resultDatas;
            }
        }

        int[,] m_pairs = new int[8, 2] { { 0, 7 }, { 1, 6 }, { 2, 5 }, { 3, 4 }, { 4, 3 }, { 5, 2 }, { 6, 1 }, { 7, 0 } };

        public ComMoneyRewardsResultData GetOtherResultData(ComMoneyRewardsResultData value)
        {
            if(null != value)
            {
                for (int i = 0; i < m_pairs.Length; ++i)
                {
                    int srcIndex = value.rank - 1;
                    int dstIndex = -1;
                    if (srcIndex == m_pairs[srcIndex, 0])
                    {
                        dstIndex = m_pairs[srcIndex, 1];
                    }
                    else if (srcIndex == m_pairs[srcIndex, 1])
                    {
                        dstIndex = m_pairs[srcIndex, 0];
                    }
                    if (dstIndex >= 0 && dstIndex < ResultDatas.Length)
                    {
                        return ResultDatas[dstIndex];
                    }
                }
            }
            return null;
        }

        public ComMoneyRewardsResultData GetLocalResultData()
        {
            for(int i = 0; i < ResultDatas.Length; ++i)
            {
                if(null != ResultDatas[i] && ResultDatas[i].recordId == PlayerBaseData.GetInstance().RoleID)
                {
                    return ResultDatas[i];
                }
            }
            return null;
        }

        public int getIndexByRoleId(ulong roleId)
        {
            for (int i = 0; i < ResultDatas.Length; ++i)
            {
                if(null != ResultDatas[i] && ResultDatas[i].recordId == roleId)
                {
                    return i;
                }
            }
            return -1;
        }
        //八强关系查找表
        int[] m_map_pairs = new int[8]
        {
            (0<<24)|(7<<16)|(3<<8)|4,
            (7<<24)|(0<<16)|(3<<8)|4,
            (3<<24)|(4<<16)|(0<<8)|7,
            (4<<24)|(3<<16)|(0<<8)|7,
            (1<<24)|(6<<16)|(2<<8)|5,
            (6<<24)|(1<<16)|(2<<8)|5,
            (2<<24)|(5<<16)|(1<<8)|6,
            (5<<24)|(2<<16)|(1<<8)|6,
        };

        public ComMoneyRewardsResultData GetNextVsData(int iIndex, PremiumLeagueStatus status,ref bool hasNext)
        {
            hasNext = false;
            if (iIndex >= 0 && iIndex < ResultDatas.Length)
            {
                for (int i = 0; i < m_map_pairs.Length; ++i)
                {
                    if ((m_map_pairs[i] >> 24) == iIndex)
                    {
                        var local = ResultDatas[iIndex];
                        if(null == local)
                        {
                            hasNext = false;
                            return null;
                        }

                        if(status == PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE)
                        {
                            var r1 = getResultByIndex((m_map_pairs[i] >> 16) & 0xFF);
                            hasNext = true;
                            return r1;
                        }

                        if (status == PremiumLeagueStatus.PLS_FINAL_EIGHT)
                        {
                            if (local.winTimes > 0)
                            {
                                hasNext = true;
                                var r1 = getResultByIndex((m_map_pairs[i] >> 8) & 0xFF);
                                if (null != r1 && r1.winTimes > 0)
                                {
                                    return r1;
                                }
                                var r2 = getResultByIndex(m_map_pairs[i] & 0xFF);
                                if (null != r2 && r2.winTimes > 0)
                                {
                                    return r2;
                                }
                            }
                            else
                            {
                                var r1 = getResultByIndex((m_map_pairs[i] >> 16) & 0xFF);
                                if(null == r1)
                                {
                                    hasNext = true;
                                }
                                else if(r1.winTimes <= 0)
                                {
                                    hasNext = true;
                                    return r1;
                                }
                                else
                                {
                                    hasNext = false;
                                }
                            }
                            return null;
                        }

                        if(status == PremiumLeagueStatus.PLS_FINAL_FOUR)
                        {
                            if (local.winTimes < 1)
                            {
                                hasNext = false;
                                return null;
                            }

                            if(local.winTimes == 1)
                            {
                                var r1 = getResultByIndex((m_map_pairs[i] >> 8) & 0xFF);
                                var r2 = getResultByIndex(m_map_pairs[i] & 0xFF);
                                if (null == r1 && null == r2)
                                {
                                    hasNext = true;
                                    return null;
                                }

								if (r1 == null || r2 == null) {
									hasNext = false;
									return null;
								}

                                var r = r1.winTimes > r2.winTimes ? r1 : r2;
                                if (r.winTimes > 1)
                                {
                                    hasNext = false;
                                    return null;
                                }
                                else
                                {
                                    hasNext = true;
                                    return r1;
                                }
                            }
                            else if(local.winTimes > 1)
                            {
                                hasNext = true;
                                for (int j = 0; j < resultDatas.Length; ++j)
                                {
                                    if(null != resultDatas[j] && resultDatas[j].recordId != local.recordId && resultDatas[j].winTimes == 2)
                                    {
                                        return resultDatas[j];
                                    }
                                }
                            }
                            return null;
                        }

                        if (status == PremiumLeagueStatus.PLS_FINAL)
                        {
                            if (local.winTimes < 2)
                            {
                                hasNext = false;
                                return null;
                            }

                            if (local.winTimes == 2)
                            {
                                for (int j = 0; j < resultDatas.Length; ++j)
                                {
                                    if (null != resultDatas[j] && resultDatas[j].recordId != local.recordId && resultDatas[j].winTimes == 2)
                                    {
                                        hasNext = true;
                                        return resultDatas[j];
                                    }
                                }
                            }

                            return null;
                        }
                        break;
                    }
                }
            }
            return null;
        }

        public ComMoneyRewardsResultData getResultByIndex(int iIndex)
        {
            if (iIndex >= 0 && iIndex < ResultDatas.Length)
            {
                return ResultDatas[iIndex];
            }
            return null;
        }


        public ComMoneyRewardsResultData getChampionData
        {
            get
            {
                ComMoneyRewardsResultData ret = null;
                for(int i = 0; i < resultDatas.Length; ++i)
                {
                    if(null != resultDatas[i] && resultDatas[i].winTimes == 3)
                    {
                        ret = resultDatas[i];
                        break;
                    }
                }
                return ret;
            }
        }

        public bool isLevelFit
        {
            get
            {
                return needLevel <= PlayerBaseData.GetInstance().Level;
            }
        }

        public bool isOpen
        {
            get
            {
                return Status != PremiumLeagueStatus.PLS_INIT;
            }
        }

        public bool needEnterance
        {
            get
            {
                return Status != PremiumLeagueStatus.PLS_INIT && Status != PremiumLeagueStatus.PLS_FINAL_WAIT_CLEAR;
            }
        }

        public MoneyRewardsStatus eMoneyRewardsStatus
        {
            get
            {
                switch(Status)
                {
                    case PremiumLeagueStatus.PLS_INIT:
                        {
                            return MoneyRewardsStatus.MRS_INVALID;
                        }
                    case PremiumLeagueStatus.PLS_ENROLL:
                        {
                            return MoneyRewardsStatus.MRS_READY;
                        }
                    case PremiumLeagueStatus.PLS_PRELIMINAY:
                        {
                            return MoneyRewardsStatus.MRS_8_RACE;
                        }
                    case PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE:
                        {
                            return MoneyRewardsStatus.MRS_PRE_4_RACE;
                        }
                    case PremiumLeagueStatus.PLS_FINAL_EIGHT:
                        {
                            return MoneyRewardsStatus.MRS_4_RACE;
                        }
                    case PremiumLeagueStatus.PLS_FINAL_FOUR:
                        {
                            return MoneyRewardsStatus.MRS_2_RACE;
                        }
                    case PremiumLeagueStatus.PLS_FINAL:
                        {
                            return MoneyRewardsStatus.MRS_RACE;
                        }
                    case PremiumLeagueStatus.PLS_FINAL_WAIT_CLEAR:
                        {
                            return MoneyRewardsStatus.MRS_END;
                        }
                }
                return MoneyRewardsStatus.MRS_INVALID;
            }
        }

        List<ComMoneyRewardsRecordsData> records = new List<ComMoneyRewardsRecordsData>();
        public List<ComMoneyRewardsRecordsData> Records
        {
            get
            {
                return records;
            }
        }

        public int getDataCount(bool bSelfRelation)
        {
            int cnt = 0;
            for (int i = 0; i < records.Count; ++i)
            {
                if (!bSelfRelation || records[i].HasSelfInfo)
                {
                    ++cnt;
                }
            }
            return cnt;
        }

        public ComMoneyRewardsRecordsData getData(int iIndex, bool bSelfRelation)
        {
            if (iIndex >= 0 && iIndex < records.Count)
            {
                if (!bSelfRelation)
                {
                    return records[iIndex];
                }

                int cnt = -1;
                for (int i = 0; i < records.Count; ++i)
                {
                    if (records[i].HasSelfInfo)
                    {
                        ++cnt;
                        if (iIndex == cnt)
                        {
                            return records[i];
                        }
                    }
                }
            }
            return null;
        }

        void _TryStartSecondMatch()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsTrySecondMatch);
        }

        public void SendMatchParty()
        {
            if (!CanMatch)
            {
                return;
            }

            WorldMatchStartReq kSend = new WorldMatchStartReq
            {
                type = (byte)PkType.Pk_Premium_League_Preliminay
            };
            NetManager.Instance().SendCommand<WorldMatchStartReq>(ServerType.GATE_SERVER, kSend);

            WaitNetMessageManager.GetInstance().Wait<WorldMatchStartRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.result != 0)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchFailed);

                    if (msgRet.result == (int)ProtoErrorCode.PREMIUM_LEAGUE_PRELIMINAY_ALREADY_LOSE)
                    {
                        if(addPartyTimes == 1)
                        {
                            _TryStartSecondMatch();
                        }
                        else
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.result);
                    }
                    return;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchStartSuccess);
            });
        }

        public void GotoPvpFight()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            ProtoTable.CitySceneTable TownTableData = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(MoneyRewardsDataManager.GetInstance().SceneID);
            if (TownTableData == null)
            {
                return;
            }

            var sceneId = MoneyRewardsDataManager.GetInstance().SceneID;

            if (systemTown.CurrentSceneID != sceneId && sceneId > 0)
            {
                GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
                    new SceneParams
                    {
                        currSceneID = systemTown.CurrentSceneID,
                        currDoorID = 0,
                        targetSceneID = sceneId,
                        targetDoorID = 0,
                    }));

                ClientSystemManager.GetInstance().CloseFrame<PkWaitingRoom>();
            }
        }
    }
}