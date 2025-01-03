using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace GameClient
{

    public class Pk2v2CrossDataManager : DataManager<Pk2v2CrossDataManager>
    {
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

        public static int MAX_PK_COUNT
        {
            get
            {
                return Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_2V2_SCORE_WAR_PARTAKE_NUM);
            }
        }
        bool m_bNetBind = false;

        public class My2v2PkInfo
        {
            public int nCurPkCount = 0;
            public uint nScore;
            public byte nWinCount;
            public List<uint> arrAwardIDs = new List<uint>();
        }

        My2v2PkInfo m_pkInfo = new My2v2PkInfo();
        public My2v2PkInfo PkInfo
        {
            set
            {
                m_pkInfo = value;
            }
            get
            {
                if(m_pkInfo == null)
                {
                    m_pkInfo = new My2v2PkInfo();
                }

                return m_pkInfo;
            }
        }
   
        public ScoreListItem m_myRankInfo = new ScoreListItem();
        
        public ScoreListItem GetMyRankInfo()
        {
            return m_myRankInfo;
        }

        const string key_pvp_2v2_score = "pvp_2v2_score";
        const string key_pvp_2v2_battle_count = "pvp_2v2_battle_count";
        const string key_pvp_2v2_win_count = "pvp_2v2_win_count";
        const string key_pvp_2v2_last_battle_time = "pvp_2v2_last_battle_time";
        const string key_pvp_2v2_reward_mask = "pvp_2v2_reward_mask";

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public override void Initialize()
        {
            Clear();
            _BindNetMsg();           

            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
        }

        public override void Clear()
        {
            m_arrScoreList = null;
            m_pkInfo = null;
            bMatching = false;
            bOpenNotifyFrame = false;
            NotifyCount = 0;
            scoreWarStatus = ScoreWar2V2Status.SWS_2V2_INVALID;
            scoreWarStateEndTime = 0;

            m_myRankInfo = new ScoreListItem();

            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;

            _UnBindNetMsg();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
        }


        public override void Update(float a_fTime)
        {
            
        }   
        
        public bool CheckPk2v2CrossScence()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null && scenedata.SceneSubType == CitySceneTable.eSceneSubType.Melee2v2Cross)
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
                NetProcess.AddMsgHandler(WorldSortListRet.MsgID, _OnRankListRes);
                NetProcess.AddMsgHandler(Scene2V2SyncScoreWarInfo.MsgID, OnScene2V2SyncScoreWarInfo);
                NetProcess.AddMsgHandler(Scene2V2coreWarRewardRes.MsgID, OnScene2V2coreWarRewardRes);

                NetProcess.AddMsgHandler(WorldMatchStartRes.MsgID, _onStartBattle);
                NetProcess.AddMsgHandler(WorldMatchCancelRes.MsgID, _onCancelBattle);          

                m_bNetBind = true;
            }
        }

        public void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldSortListRet.MsgID, _OnRankListRes);
            NetProcess.RemoveMsgHandler(Scene2V2SyncScoreWarInfo.MsgID, OnScene2V2SyncScoreWarInfo);
            NetProcess.RemoveMsgHandler(Scene2V2coreWarRewardRes.MsgID, OnScene2V2coreWarRewardRes);

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

            if(!CheckPk2v2CrossScence())
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
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossBeginMatchRes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossBeginMatch);
        }

        private void _onCancelBattle(MsgDATA a_msgData)
        {
            if (null == a_msgData)
            {
                return;
            }

            if (!CheckPk2v2CrossScence())
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
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossCancelMatchRes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossCancelMatch);
        }

        void Swap<T>(ref T x, ref T y)
        {
            T temp = x;
            x = y;
            y = temp;
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

            if((SortListType)arrRecods.type.mainType != SortListType.SORTLIST_2V2_SCORE_WAR)
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

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdatePk2v2CrossRankScoreList);           
        }

        public void SwitchToPk2v2CrossScene()
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
                    targetSceneID = 5008,
                    targetDoorID = 0,
                }));
        }

        void SendCancelOnePersonMatchGameReq()
        {
            WorldMatchCancelReq req = new WorldMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
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

        ScoreWar2V2Status scoreWarStatus = ScoreWar2V2Status.SWS_2V2_INVALID;
        UInt32 scoreWarStateEndTime = 0;

        public ScoreWar2V2Status Get2v2CrossWarStatus()
        {
            return scoreWarStatus;
        }

        public UInt32 Get2v2CrossWarStatusEndTime()
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

        string m_kSavePath = "2v2CrossOpen.json";
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

        void _OnOnCountValueChange(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }

            string key = uiEvent.Param1 as string;
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (key == key_pvp_2v2_score)
            {
                Pk2v2CrossDataManager.My2v2PkInfo pkInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
                pkInfo.nScore = (uint)CountDataManager.GetInstance().GetCount(key);
                Pk2v2CrossDataManager.GetInstance().PkInfo = pkInfo;
            }
            else if (key == key_pvp_2v2_battle_count)
            {
                Pk2v2CrossDataManager.My2v2PkInfo pkInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
                pkInfo.nCurPkCount = (int)((uint)(Pk2v2CrossDataManager.MAX_PK_COUNT) - (uint)CountDataManager.GetInstance().GetCount(key));
                Pk2v2CrossDataManager.GetInstance().PkInfo = pkInfo;

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossPkAwardInfoUpdate);
            }
            else if (key == key_pvp_2v2_win_count)
            {
                Pk2v2CrossDataManager.My2v2PkInfo pkInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
                pkInfo.nWinCount = (byte)CountDataManager.GetInstance().GetCount(key);
                Pk2v2CrossDataManager.GetInstance().PkInfo = pkInfo;

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossPkAwardInfoUpdate);
            }
            else if (key == key_pvp_2v2_reward_mask)
            {
                Pk2v2CrossDataManager.My2v2PkInfo pkInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
                List<uint> arrIDs = pkInfo.arrAwardIDs;
                arrIDs.Clear();


                int maskValue = CountDataManager.GetInstance().GetCount(key);
                BitArray ba = new BitArray(BitConverter.GetBytes(maskValue));
                if (ba != null)
                {
                    int iSize = TableManager.GetInstance().GetTable<ProtoTable.ScoreWar2v2RewardTable>().Count;
                    for (uint i = 0; i < ba.Length && i < iSize; ++i)
                    {
                        int iId = (int)(i + 1);
                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ScoreWar2v2RewardTable>(iId);
                        if (null == item)
                        {
                            continue;
                        }

                        int rewardID = item.RewardId;
                        if (ba.Get(rewardID))
                        {
                            arrIDs.Add((uint)rewardID);
                        }
                    }
                }

                arrIDs.Sort();
                Pk2v2CrossDataManager.GetInstance().PkInfo = pkInfo;

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossPkAwardInfoUpdate);
            }

            return;
        }

        private void OnLevelChanged(int iPreLv, int iCurLv)
        {
            int i2v2CrossOpenLv = 0;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null)
            {
                i2v2CrossOpenLv = SystemValueTableData.Value;
                if (i2v2CrossOpenLv > 0 && iPreLv < i2v2CrossOpenLv && iCurLv >= i2v2CrossOpenLv)
                {                    
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK2V2CrossButton, (byte)Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatus());
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
                }
            }
        }

        void OnScene2V2SyncScoreWarInfo(MsgDATA msg)
        {
            Scene2V2SyncScoreWarInfo ret = new Scene2V2SyncScoreWarInfo();
            ret.decode(msg.bytes);           

            scoreWarStatus = (ScoreWar2V2Status)ret.status;
            scoreWarStateEndTime = ret.statusEndTime;

            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
            {
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);

            bool isFlag = false;
            if (ret.status >= (byte)ScoreWar2V2Status.SWS_2V2_PREPARE && ret.status < (byte)ScoreWar2V2Status.SWS_2V2_WAIT_END)
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
                    ClientSystemManager.GetInstance().OpenFrame<Pk2v2CrossOpenNotifyFrame>(FrameLayer.Middle);

                    SetIDOpened(ClientApplication.playerinfo.accid);
                }                
            }           

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK2V2CrossButton, ret.status);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK2V2CrossStatusUpdate); 

            if(scoreWarStatus == ScoreWar2V2Status.SWS_2V2_BATTLE && ClientSystemManager.GetInstance().IsFrameOpen<Pk2v2CrossWaitingRoomFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<Pk2v2CrossMatchStartNotifyFrame>();
            }

            if(scoreWarStatus >= ScoreWar2V2Status.SWS_2V2_WAIT_END)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossCancelMatchRes);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk2v2CrossCancelMatch);
            }

            if(scoreWarStatus == ScoreWar2V2Status.SWS_2V2_INVALID || scoreWarStatus == ScoreWar2V2Status.SWS_2V2_WAIT_END || scoreWarStatus == ScoreWar2V2Status.SWS_2V2_MAX)
            {
                bOpenNotifyFrame = false;
                NotifyCount = 0;
                ClearIDOpened(ClientApplication.playerinfo.accid);

                ClientSystemManager.GetInstance().CloseFrame<JoinPk2v2CrossFrame>();
            }

            if(scoreWarStatus >= ScoreWar2V2Status.SWS_2V2_WAIT_END) // 活动结束了 主动取消匹配
            {
                bMatching = false;
                SendCancelOnePersonMatchGameReq();
            }
        }

        void OnScene2V2coreWarRewardRes(MsgDATA msg)
        {
            Scene2V2coreWarRewardRes res = new Scene2V2coreWarRewardRes();
            res.decode(msg.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
            }

            return;
        }
    }
}
