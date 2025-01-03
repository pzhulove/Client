using Protocol;
using System.Collections.Generic;
using ProtoTable;
using Network;
using System;
using UnityEngine;

namespace GameClient
{
    public class OtherPlayerDetailItem
    {
        public UInt64 guid;
        public UInt32 itemTypeId;
        public UInt32 num;
    }

    public class ChijiOtherPlayerItems
    {
        public UInt32 battleID;
        public UInt64 playerID;
        public List<OtherPlayerDetailItem> detailItems = new List<OtherPlayerDetailItem>();

        public void ClearData()
        {
            battleID = 0;
            playerID = 0;
            detailItems.Clear();
        }
    }

    public class JoinPlayerInfo
    {
        public UInt32 accId;
        public UInt64 playerId;
        public string playerName;
        public byte occu;  // 选角前的原本职业
    }

    public class PlayerDeathReason
    {
        public UInt64 playerID;
        public UInt64 killerID;
        public UInt32 reason;
        public UInt32 kills; // 这是killer的kills，不是Deather的kills
    }

    public class PoisonRingInfo
    {
        public Vector2 center = Vector2.zero;
        public float radius = 0.0f;
        public float durTime = 0.0f;
        public float shrinkTime = 0.0f;
        public float lastRadius = 0.0f;
        public Vector2 lastCenter = Vector2.zero;
        public Vector2 nextStageCenter = Vector2.zero;
        public float nextStageRadius = 0.0f;

        public void Reset()
        {
            center = Vector2.zero;
            radius = 0.0f;
            durTime = 0.0f;
            shrinkTime = 0.0f;
            lastRadius = 0.0f;
            lastCenter = Vector2.zero;
            nextStageCenter = Vector2.zero;
            nextStageRadius = 0.0f;
        }
    };

    public class ChijiDataManager : DataManager<ChijiDataManager>
    {
        private bool m_bNetBind = false;

        private int m_PrepareScenePlayerNum = 0;
        private int m_PrepareSceneMaxPlayerNum = 0;
        private UInt32 m_ChijiBattleID = 0;
        private ChiJiTimeTable.eBattleStage m_CurBattleStage = ChiJiTimeTable.eBattleStage.BS_NONE;   // 一局吃鸡当前所处的阶段
        private uint[] m_StageStartTimeList = null;  // 每个阶段的起始时间
        private SettlementInfo m_Settlementinfo = new SettlementInfo(); // 结算数据(失败者要退回到主城后才显示结算信息，所以该数据在退出吃鸡场景时还不能清掉，只能单独维护生命周期)
        private bool m_bGuardForSettlement = false;    // 时序问题，收到协议后，在进入城镇后再弹界面(失败者要退回到主城后才显示结算信息，所以该数据在退出吃鸡场景时还不能清掉，只能单独维护生命周期)
        private bool m_bGuardForPickUpOtherPlayerItems = false;
        private bool m_bGuardForPkEndData = false;
        private List<ulong> m_otherPlayerDead = new List<ulong>();
        private ChijiOtherPlayerItems m_OtherPlayerItems = new ChijiOtherPlayerItems();
        private int m_SurvivePlayerNum = 0;
        private uint m_SceneNodeID = 0;
        private PoisonRingInfo m_poisonRing = new PoisonRingInfo();
        private UInt32 m_BattleDungeonId = 0;
        private byte m_BattleRaceType = 0;
        private UInt64 m_SessionId = 0;
        private bool m_isMainPlayerDead = false;
        private List<JoinPlayerInfo> m_JoinPlayerInfoList = new List<JoinPlayerInfo>();
        private List<PlayerDeathReason> m_ShowDeathPlayerList = new List<PlayerDeathReason>();
        private List<BattleNpc> m_NpcDataList = new List<BattleNpc>();
        private PlayerDeathReason m_KillInfo = new PlayerDeathReason();
        private UInt64 m_RecentKillPlayerID;
        private UInt32 m_KillNum;
        private SceneMatchPkRaceEnd m_pkEndData = null;
        private bool m_IsMatching = false;
        private bool m_IsToPrepareScene = false;
        private bool m_SwitchingTownToPrepare = false;     // 从主城切到吃鸡准备场景
        private bool m_SwitchingPrepareToTown = false;     // 从吃鸡准备场景切回主城(这个变量比较特殊，在切回城镇时还需要用到它，所以不能统一在退回城镇时清除，要单独维护生命周期)
        private bool m_SwitchingPrepareToChijiScene = false; // 从吃鸡准备场景切到吃鸡场景
        private bool m_SwitchingChijiSceneToPrepare = false;  // 从吃鸡场景切回准备场景(这个变量比较特殊，在切回城镇时还需要用到它，所以不能统一在退回城镇时清除，要单独维护生命周期)
        private bool m_IsReadyPk = false;
        private int m_BestRank = 0;

        public int PrepareScenePlayerNum { get { return m_PrepareScenePlayerNum; } set { m_PrepareScenePlayerNum = value; } }
        public int PrepareSceneMaxPlayerNum { get { return m_PrepareSceneMaxPlayerNum; } }
        public bool SwitchingTownToPrepare { get { return m_SwitchingTownToPrepare; } set { m_SwitchingTownToPrepare = value; } }
        public bool SwitchingPrepareToTown { get { return m_SwitchingPrepareToTown; } set { m_SwitchingPrepareToTown = value; } }
        public bool SwitchingPrepareToChijiScene { get { return m_SwitchingPrepareToChijiScene; } set { m_SwitchingPrepareToChijiScene = value; } }
        public bool SwitchingChijiSceneToPrepare { get { return m_SwitchingChijiSceneToPrepare; } set { m_SwitchingChijiSceneToPrepare = value; } }
        public List<ulong> OtherDeadPlayers { get { return m_otherPlayerDead; } }
        public UInt32 ChijiBattleID { get { return m_ChijiBattleID; } }
        public ChiJiTimeTable.eBattleStage CurBattleStage { get { return m_CurBattleStage; } }
        public uint[] StageStartTimeList { get { return m_StageStartTimeList; } }
        public SettlementInfo Settlementinfo { get { return m_Settlementinfo; } set { m_Settlementinfo = value; } }
        public bool GuardForSettlement { get { return m_bGuardForSettlement; } set { m_bGuardForSettlement = value; } }
        public bool GuardForPickUpOtherPlayerItems { get { return m_bGuardForPickUpOtherPlayerItems; } set { m_bGuardForPickUpOtherPlayerItems = value; } }
        public ChijiOtherPlayerItems OtherPlayerItems { get { return m_OtherPlayerItems; } }
        public int SurvivePlayerNum { get { return m_SurvivePlayerNum; } }
        public UInt32 BattleDungeonId { get { return m_BattleDungeonId; } set { m_BattleDungeonId = value; } }
        public byte BattleRaceType { get { return m_BattleRaceType; } set { m_BattleRaceType = value; } }
        public UInt64 RecentKillPlayerID { get { return m_RecentKillPlayerID; } set { m_RecentKillPlayerID = value; } }
        public UInt64 SessionId { get { return m_SessionId; } set { m_SessionId = value; } }
        public PoisonRingInfo PoisonRing { get { return m_poisonRing; } }
        public bool IsMainPlayerDead { get { return m_isMainPlayerDead; } }
        public List<JoinPlayerInfo> JoinPlayerInfoList { get { return m_JoinPlayerInfoList; } }
        public List<PlayerDeathReason> ShowDeathPlayerList { get { return m_ShowDeathPlayerList; } }
        public List<BattleNpc> NpcDataList { get { return m_NpcDataList; } }
        public SceneMatchPkRaceEnd PkEndData { get { return m_pkEndData; } set { m_pkEndData = value; } }
        public bool GuardForPkEndData { get { return m_bGuardForPkEndData; } set { m_bGuardForPkEndData = value; } }
        public bool IsMatching { get { return m_IsMatching; } set { m_IsMatching = value; } }
        public bool IsToPrepareScene { get { return m_IsToPrepareScene; } set { m_IsToPrepareScene = value; } }
        public UInt32 KillNum { get { return m_KillNum; } }
        public uint SceneNodeId { get { return m_SceneNodeID; } }
        public bool IsReadyPk { get { return m_IsReadyPk; } set { m_IsReadyPk = value; } }
        public int BestRank { get { return m_BestRank; } set { m_BestRank = value; } }

        private const string HonorBattleFieldStr = "HonorBattleField";
        public int[] ChijiActivityIDs = new int[] { 2019, 2020, 2021, 2022, 2023, 2024, 2025 };//吃鸡活动ID
        public int CurrentUseDrugId { get; set; }
        public ISceneData sceneData { get; set; }
        public void DoDead()
        {
            m_isMainPlayerDead = true;
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public override void Initialize()
        {
            Clear();
            _BindNetMsg();
            _BindUIEvent();

            var tableData = TableManager.GetInstance().GetTable<ChiJiTimeTable>();
            if (tableData != null && m_StageStartTimeList == null)
            {
                m_StageStartTimeList = new uint[tableData.Count];
            }
        }

        public override void Clear()
        {
            _UnBindNetMsg();
            _UnBindUIEvent();
            ClearChijiSceneData();
            ClearPrepareSceneData();
        }

        // 吃鸡管理器管理的是整个吃鸡系统的数据，不同数据的生命周期不一样，
        // 部分数据在离开单局战斗时需要重置，有的在离开吃鸡场景后重置，有的在退出整个吃鸡玩法的准备场景后重置
        public void ClearBattleData()  // 退出单局需要清空的数据
        {
            m_BattleDungeonId = 0;
            m_BattleRaceType = 0;
            if (m_pkEndData != null)
            {
                m_pkEndData = null;
            }
            m_bGuardForPkEndData = false;
            m_SessionId = 0;
            m_IsReadyPk = false;
        }

        public void ClearChijiSceneData() // 退出吃鸡场景需要清空的数据
        {
            ClearBattleData();

            m_ChijiBattleID = 0;
            m_otherPlayerDead.Clear();
            m_isMainPlayerDead = false;
            PlayerBaseData.GetInstance().BuffMgr.Clear();
            m_CurBattleStage = ChiJiTimeTable.eBattleStage.BS_NONE;
            m_SurvivePlayerNum = 0;
            m_poisonRing.Reset();
            if (m_StageStartTimeList != null)
            {
                for (int i = 0; i < m_StageStartTimeList.Length; i++)
                {
                    m_StageStartTimeList[i] = 0;
                }
            }
            m_bGuardForPickUpOtherPlayerItems = false;
            m_OtherPlayerItems.ClearData();
            m_JoinPlayerInfoList.Clear();
            m_ShowDeathPlayerList.Clear();
            m_NpcDataList.Clear();
            m_RecentKillPlayerID = 0;
            m_KillNum = 0;
            m_IsMatching = false;
            m_IsReadyPk = false;
        }

        public void ClearAllRelatedSystemData()  // 退出吃鸡场景需要清空的所有相关系统的相关数据
        {
            ClearChijiSceneData();

            BattleDataManager.GetInstance().PkRaceType = RaceType.Dungeon;
            PlayerBaseData.GetInstance().ClearChijiData();
            ItemDataManager.GetInstance().ClearChijiData();
            SkillDataManager.GetInstance().ClearChijiSkill();
            PetDataManager.GetInstance().ClearChijiPetData();
            SystemNotifyManager.Clear();
        }

        public void ClearPrepareSceneData() // 退出吃鸡准备场景，因为服务器在退出吃鸡场景的时候就已经同步玩家原有数据了，此时不能再清空各相关系统的数据
        {
            m_PrepareScenePlayerNum = 0;
            m_PrepareSceneMaxPlayerNum = 0;
            m_IsToPrepareScene = false;
            m_BestRank = 0;
            this.sceneData = null;
        }

        public void ClearPlayerIntrinsicData()  // 匹配进入吃鸡场景时需要清空玩家本身一些原有的数据
        {
            ItemDataManager.GetInstance().ClearChijiData();
            SkillDataManager.GetInstance().ClearChijiSkill();
            PetDataManager.GetInstance().ClearChijiPetData();
            SystemNotifyManager.Clear();
        }

        private void _BindNetMsg()
        {
            if (m_bNetBind == false)
            {
                NetProcess.AddMsgHandler(BattleNotifyPrepareInfo.MsgID, _OnNotifyChijiPrepareInfo); // 同步吃鸡准备场景人数
                NetProcess.AddMsgHandler(BattleEnrollRes.MsgID, _OnBattleEnrollRes); // 吃鸡报名返回
                NetProcess.AddMsgHandler(SceneBattleMatchSync.MsgID, _OnSceneBattleMatchSync); // 匹配成功，进入吃鸡场景，同步吃鸡战斗id 
                NetProcess.AddMsgHandler(SceneBattleOccuListRes.MsgID, _OnOccuListRes);
                NetProcess.AddMsgHandler(SceneBattleSelectOccuRes.MsgID, _OnRecvSelectOccuRes);
                NetProcess.AddMsgHandler(SceneBattleBirthTransferNotify.MsgID, _OnRecvBirthTransfer);
                NetProcess.AddMsgHandler(SceneBattleSyncPoisonRing.MsgID, _OnRecvPoisionRing);
                NetProcess.AddMsgHandler(SceneBattleUseItemRes.MsgID, _OnPickUpBuffItemRes);
                NetProcess.AddMsgHandler(SceneItemAdd.MsgID, _OnSyncSceneItemAdd);
                NetProcess.AddMsgHandler(SceneItemDel.MsgID, _OnSyncSceneItemDel);
                NetProcess.AddMsgHandler(SceneBattleNotifyWaveInfo.MsgID, _OnSyncBattleWaveInfo);  // 同步当前游戏进度状态
                NetProcess.AddMsgHandler(SceneBattleBalanceEnd.MsgID, _OnBattleBalanceEnd);  // 游戏结束，大吉大利，今晚吃鸡
                NetProcess.AddMsgHandler(SceneBattleThrowSomeoneItemRes.MsgID, _OnBattleThrowSomeOne);
                NetProcess.AddMsgHandler(SceneBattleNotifySomeoneDead.MsgID, _OnBattleSomeOneDead); // 通知玩家死亡，每个人死亡都会通知给其他所有人
                NetProcess.AddMsgHandler(SceneBattleNotifySpoilsItem.MsgID, _OnPickUpOthersItem); // 捡取别人死后道具
                NetProcess.AddMsgHandler(SceneBattlePickUpSpoilsRes.MsgID, _OnPickUpSpoilsRes);
                //NetProcess.AddMsgHandler(BattleLockSomeOneRes.MsgID, _OnBattleLockSomeOneRes);   // 准备挑战别人请求
                NetProcess.AddMsgHandler(BattlePkSomeOneRes.MsgID, _OnPkSomeOneRes); // 吃鸡挑战某个玩家返回
                NetProcess.AddMsgHandler(SceneBattleNpcNotify.MsgID, _OnNpcNotify);  //通知刷新npc 
                NetProcess.AddMsgHandler(SceneBattleNpcTradeRes.MsgID, _OnNpcTradeRes);
                NetProcess.AddMsgHandler(SceneBattleOpenBoxRes.MsgID, _OnOpenBoxRes);
                NetProcess.AddMsgHandler(SceneBattlePlaceTrapsRes.MsgID, _OnTrapPlaced);//放置陷阱消息
                NetProcess.AddMsgHandler(SceneBattleNotifyBeTraped.MsgID, _OnTrapTriggered);//陷阱被踩中通知
                NetProcess.AddMsgHandler(BattleNotifyBestRank.MsgID, _OnNotifyBestRank);
                NetProcess.AddMsgHandler(BattleSkillChoiceListNotify.MsgID, _OnNotifySkillChoiceList);
                NetProcess.AddMsgHandler(BattleChoiceSkillRes.MsgID, _OnChoiceSkillRes);
                NetProcess.AddMsgHandler(BattleEquipChoiceListNotify.MsgID, _OnEquipChoiceListRes);
                NetProcess.AddMsgHandler(BattleChoiceEquipRes.MsgID, _OnChoiceEquipRes);
                NetProcess.AddMsgHandler(SceneBattleNoWarOption.MsgID, _OnNotifyNoWarOption);
                NetProcess.AddMsgHandler(SceneBattleNoWarWait.MsgID, _OnNotifyNoWarWait);
                NetProcess.AddMsgHandler(SceneBattleNoWarChoiceRes.MsgID, _OnNotifyNoWarChoiceRes);
                NetProcess.AddMsgHandler(SceneBattleNoWarNotify.MsgID, _OnNoWarNotify);
                
                m_bNetBind = true;
            }
        }

        private void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(BattleNotifyPrepareInfo.MsgID, _OnNotifyChijiPrepareInfo);
            NetProcess.RemoveMsgHandler(BattleEnrollRes.MsgID, _OnBattleEnrollRes);
            NetProcess.RemoveMsgHandler(SceneBattleMatchSync.MsgID, _OnSceneBattleMatchSync);
            NetProcess.RemoveMsgHandler(SceneBattleOccuListRes.MsgID, _OnOccuListRes);
            NetProcess.RemoveMsgHandler(SceneBattleSelectOccuRes.MsgID, _OnRecvSelectOccuRes);
            NetProcess.RemoveMsgHandler(SceneBattleBirthTransferNotify.MsgID, _OnRecvBirthTransfer);
            NetProcess.RemoveMsgHandler(SceneBattleSyncPoisonRing.MsgID, _OnRecvPoisionRing);
            NetProcess.RemoveMsgHandler(SceneBattleUseItemRes.MsgID, _OnPickUpBuffItemRes);
            NetProcess.RemoveMsgHandler(SceneItemAdd.MsgID, _OnSyncSceneItemAdd);
            NetProcess.RemoveMsgHandler(SceneItemDel.MsgID, _OnSyncSceneItemDel);
            NetProcess.RemoveMsgHandler(SceneBattleNotifyWaveInfo.MsgID, _OnSyncBattleWaveInfo);
            NetProcess.RemoveMsgHandler(SceneBattleBalanceEnd.MsgID, _OnBattleBalanceEnd);
            NetProcess.RemoveMsgHandler(SceneBattleThrowSomeoneItemRes.MsgID, _OnBattleThrowSomeOne);
            NetProcess.RemoveMsgHandler(SceneBattleNotifySomeoneDead.MsgID, _OnBattleSomeOneDead);
            NetProcess.RemoveMsgHandler(SceneBattleNotifySpoilsItem.MsgID, _OnPickUpOthersItem);
            NetProcess.RemoveMsgHandler(SceneBattlePickUpSpoilsRes.MsgID, _OnPickUpSpoilsRes);
            //NetProcess.RemoveMsgHandler(BattleLockSomeOneRes.MsgID, _OnBattleLockSomeOneRes);
            NetProcess.RemoveMsgHandler(BattlePkSomeOneRes.MsgID, _OnPkSomeOneRes);
            NetProcess.RemoveMsgHandler(SceneBattleNpcNotify.MsgID, _OnNpcNotify);
            NetProcess.RemoveMsgHandler(SceneBattleNpcTradeRes.MsgID, _OnNpcTradeRes);
            NetProcess.RemoveMsgHandler(SceneBattleOpenBoxRes.MsgID, _OnOpenBoxRes);
            NetProcess.RemoveMsgHandler(SceneBattlePlaceTrapsRes.MsgID, _OnTrapPlaced);//放置陷阱消息
            NetProcess.RemoveMsgHandler(SceneBattleNotifyBeTraped.MsgID, _OnTrapTriggered);//陷阱被踩中通知
            NetProcess.RemoveMsgHandler(BattleNotifyBestRank.MsgID, _OnNotifyBestRank);
            NetProcess.RemoveMsgHandler(BattleSkillChoiceListNotify.MsgID, _OnNotifySkillChoiceList);
            NetProcess.RemoveMsgHandler(BattleChoiceSkillRes.MsgID, _OnChoiceSkillRes);
            NetProcess.RemoveMsgHandler(BattleEquipChoiceListNotify.MsgID, _OnEquipChoiceListRes);
            NetProcess.RemoveMsgHandler(BattleChoiceEquipRes.MsgID, _OnChoiceEquipRes);
            NetProcess.RemoveMsgHandler(SceneBattleNoWarOption.MsgID, _OnNotifyNoWarOption);
            NetProcess.RemoveMsgHandler(SceneBattleNoWarWait.MsgID, _OnNotifyNoWarWait);
            NetProcess.RemoveMsgHandler(SceneBattleNoWarChoiceRes.MsgID, _OnNotifyNoWarChoiceRes);
            NetProcess.RemoveMsgHandler(SceneBattleNoWarNotify.MsgID, _OnNoWarNotify);

            m_bNetBind = false;
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiPkReadyFinish, _OnChijiPkReadyFinish);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiPkReadyFinish, _OnChijiPkReadyFinish);
        }
        private void _OnTrapPlaced(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnTrapPlaced ==>> msg is nil");
                return;
            }

            var res = new SceneBattlePlaceTrapsRes();
            res.decode(msg.bytes);
            if (res.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.retCode);
                return;
            }
        }
        private void _OnTrapTriggered(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnTrapTriggered ==>> msg is nil");
                return;
            }

            var res = new SceneBattleNotifyBeTraped();
            res.decode(msg.bytes);

            if (res.battleID != m_ChijiBattleID)
            {
                Logger.LogErrorFormat(string.Format("吃鸡battleid不一致，Cur ChijiBattleID = {0}, msgData.battleID = {1},协议:[SceneItemAdd]", m_ChijiBattleID, res.battleID));
                return;
            }

            if (res.ownerID == PlayerBaseData.GetInstance().RoleID)
            {
                JoinPlayerInfo info = m_JoinPlayerInfoList.Find(x => { return x.playerId == res.playerID; });

                if (info != null)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("{0}踩中了你的陷阱，受到当前血量30%伤害并定身", info.playerName));
                }
            }
            else
            {
                JoinPlayerInfo info = m_JoinPlayerInfoList.Find(x => { return x.playerId == res.ownerID; });

                if (info != null)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("你踩中了{0}的陷阱，受到当前血量30%伤害并定身", info.playerName));
                }
            }
            var current = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }
            current.DoTrapEffect(res.x, res.y);
        }

        private void _OnPickUpBuffItemRes(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnPickUpItemRes ==>> msg is nil");
                return;
            }

            SceneBattleUseItemRes res = new SceneBattleUseItemRes();
            res.decode(msg.bytes);

            if (res.code != 0)
            {
                Logger.LogErrorFormat("[SceneBattleUseItemRes] error,error code= {0}", res.code);
            }
        }

        private void _OnSyncSceneItemAdd(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncSceneItemAdd ==>> msg is nil");
                return;
            }

            var res = new SceneItemAdd();
            res.decode(msg.bytes);

            if (res.battleID != m_ChijiBattleID)
            {
                Logger.LogErrorFormat(string.Format("吃鸡battleid不一致，Cur ChijiBattleID = {0}, msgData.battleID = {1},协议:[SceneItemAdd]", m_ChijiBattleID, res.battleID));
                return;
            }

            var current = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            current.OnRecvSyncSceneItemAdd(res);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NearItemsChanged);
        }

        private void _OnSyncSceneItemDel(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncSceneItemAdd ==>> msg is nil");
                return;
            }

            var res = new SceneItemDel();
            res.decode(msg.bytes);

            if (res.battleID != m_ChijiBattleID)
            {
                Logger.LogErrorFormat(string.Format("吃鸡battleid不一致，Cur ChijiBattleID = {0}, msgData.battleID = {1},协议:[SceneItemDel]", m_ChijiBattleID, res.battleID));
                return;
            }

            var current = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }
            current.OnRecvSceneItemDel(res);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NearItemsChanged);
        }

        void _OnNotifyChijiPrepareInfo(MsgDATA msg)
        {
            BattleNotifyPrepareInfo msgData = new BattleNotifyPrepareInfo();
            msgData.decode(msg.bytes);

            m_PrepareScenePlayerNum = (int)msgData.playerNum;
            m_PrepareSceneMaxPlayerNum = (int)msgData.totalNum;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateChijiPrepareScenePlayerNum);
        }

        void _OnBattleEnrollRes(MsgDATA msg)
        {
            BattleEnrollRes msgData = new BattleEnrollRes();
            msgData.decode(msg.bytes);

            if (msgData.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.retCode);
                return;
            }
            else
            {
                if (msgData.isMatch != 0)
                {
                    //SystemNotifyManager.SystemNotify(4200005);

                    ClientSystemManager.GetInstance().OpenFrame<ChijiSeekWaitingFrame>(FrameLayer.Middle);
                }
                else
                {
                    ClientSystemManager.GetInstance().CloseFrame<ChijiSeekWaitingFrame>();
                }
            }
        }

        private void _OnOccuListRes(MsgDATA msg)
        {
            SceneBattleOccuListRes res = new SceneBattleOccuListRes();
            res.decode(msg.bytes);

            ClientSystemManager.GetInstance().OpenFrame<SelectOccupationFrame>(FrameLayer.Middle, res.occuList);
        }

        private void _OnRecvSelectOccuRes(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("onRecvSelectOccuRes ==>> msg is nil");
                return;
            }
            var res = new SceneBattleSelectOccuRes();
            res.decode(msg.bytes);

            if (res.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format("[onRecvSelectOccuRes] error Code: {0}", res.retCode));
                return;
            }
            SystemNotifyManager.SysNotifyFloatingEffect("选择职业成功");
        }

        private void _OnRecvBirthTransfer(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("onRecvBirthTransfer ==>> msg is nil");
                return;
            }
            var res = new SceneBattleBirthTransferNotify();
            res.decode(msg.bytes);
            //   var current = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            var current = GameClient.ClientSystemManager.GetInstance().GetCurrentSystem() as GameClient.ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            if (current.MainPlayer == null)
            {
                return;
            }
            float AxisScale = PlayerBaseData.GetInstance().AxisScale;
            var birthPos = new Vector3(res.birthPosX / AxisScale, 0.0f, res.birthPosY / AxisScale);
            if (res.playerID == current.MainPlayer.ActorData.GUID)
            {
                PlayerBaseData.GetInstance().Pos = birthPos;
                current.MainPlayer.ActorData.MoveData.ServerPosition = PlayerBaseData.GetInstance().Pos;
                SystemNotifyManager.SysNotifyFloatingEffect("传送成功");
            }
            else
            {
                //if(current.TownOtherPlayers.ContainsKey(res.playerID))
                //{
                //    var player = current.TownOtherPlayers[res.playerID];
                //    if(player != null)
                //    {
                //        player.ActorData.MoveData.ServerPosition = birthPos;
                //    }
                //}
                var fighter = current.OtherFighters.GetFighter(res.playerID);
                if (fighter != null)
                {
                    fighter.ActorData.MoveData.ServerPosition = birthPos;
                }
            }
        }

        void _OnRecvPoisionRing(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("onRecvPoisionRing ==>> msg is nil");
                return;
            }
            //var current = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            var current = GameClient.ClientSystemManager.GetInstance().GetCurrentSystem() as GameClient.ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            SceneBattleSyncPoisonRing msgData = new SceneBattleSyncPoisonRing();
            msgData.decode(msg.bytes);
            if (msgData.battleID != m_ChijiBattleID)
            {
                return;
            }
            Vector2 radiuCenter = new Vector2(msgData.x / current.AxisScale, msgData.y / current.AxisScale);
            Vector2 nextStageCenter = new Vector2(msgData.x1 / current.AxisScale, msgData.y1 / current.AxisScale);
            float nextStageRadius = msgData.radius1 / current.AxisScale;
            m_poisonRing.lastCenter = m_poisonRing.center;
            m_poisonRing.lastRadius = m_poisonRing.radius;
            m_poisonRing.center = radiuCenter;
            m_poisonRing.durTime = msgData.beginTimestamp;
            m_poisonRing.radius = msgData.radius / current.AxisScale;
            m_poisonRing.shrinkTime = msgData.interval;
            m_poisonRing.nextStageCenter = nextStageCenter;
            m_poisonRing.nextStageRadius = nextStageRadius;
            Logger.LogErrorFormat("_OnRecvPoisionRing Posison Ring x {0} y {1} radius {2} timeStamp {3} durTime {4} nextStage x {5} y {6} nextRadius {7}", msgData.x, msgData.y, msgData.radius, msgData.beginTimestamp, msgData.interval, msgData.x1, msgData.y1, msgData.radius1);
            current.SetPoisonRing((int)msgData.x, (int)msgData.y, (int)msgData.radius, msgData.beginTimestamp, (int)msgData.interval, m_poisonRing.lastCenter, m_poisonRing.lastRadius);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BlueCircleChange, radiuCenter, msgData.radius / current.AxisScale, (float)msgData.beginTimestamp, (float)msgData.interval);

        }

        void _OnSceneBattleMatchSync(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnBattleMatchSync ==>> msg is nil");
                return;
            }

            SceneBattleMatchSync msgData = new SceneBattleMatchSync();
            msgData.decode(msg.bytes);

            m_ChijiBattleID = msgData.battleID;
            m_SurvivePlayerNum = (int)msgData.suvivalNum;
            m_SceneNodeID = msgData.sceneNodeID;

            m_JoinPlayerInfoList.Clear();

            for (int i = 0; i < msgData.players.Length; i++)
            {
                JoinPlayerInfo info = new JoinPlayerInfo();

                info.accId = msgData.players[i].accId;
                info.playerId = msgData.players[i].playerId;
                info.playerName = msgData.players[i].playerName;
                info.occu = msgData.players[i].occu;

                m_JoinPlayerInfoList.Add(info);
            }

            Utility.SwitchToChiJiRoom();
        }

        private void _OnSyncBattleWaveInfo(MsgDATA msg)
        {
            SceneBattleNotifyWaveInfo msgData = new SceneBattleNotifyWaveInfo();
            msgData.decode(msg.bytes);

            if (msgData.battleID != m_ChijiBattleID)
            {
                Logger.LogErrorFormat(string.Format("吃鸡battleid不一致，Cur ChijiBattleID = {0}, msgData.battleID = {1},协议:[SceneBattleNotifyWaveInfo]", m_ChijiBattleID, msgData.battleID));
                return;
            }

            m_CurBattleStage = (ChiJiTimeTable.eBattleStage)msgData.waveID;

            if (m_StageStartTimeList != null && (int)m_CurBattleStage < m_StageStartTimeList.Length)
            {
                m_StageStartTimeList[(int)m_CurBattleStage] = TimeManager.GetInstance().GetServerTime();
            }

            //Logger.LogErrorFormat("吃鸡阶段测试----接收服务器数据,当前波次 : m_CurBattleStage = {0},记下服务器时间 GetServerTime = {1}", m_CurBattleStage, TimeManager.GetInstance().GetServerTime());

            if (m_CurBattleStage == ChiJiTimeTable.eBattleStage.BS_PREPARE_TIME)
            {
                SystemNotifyManager.SystemNotify(11000);
            }
            else if (m_CurBattleStage == ChiJiTimeTable.eBattleStage.BS_START_PK)
            {
                SystemNotifyManager.SystemNotify(11002);
            }
            else if (m_CurBattleStage == ChiJiTimeTable.eBattleStage.BS_PUT_ITEM_1 || m_CurBattleStage == ChiJiTimeTable.eBattleStage.BS_PUT_ITEM_2 || m_CurBattleStage == ChiJiTimeTable.eBattleStage.BS_PUT_ITEM_3)
            {
                SystemNotifyManager.SystemNotify(11001);
            }
            else if (m_CurBattleStage == ChiJiTimeTable.eBattleStage.BS_SAFE_AREA_1 || m_CurBattleStage == ChiJiTimeTable.eBattleStage.BS_SAFE_AREA_2 || m_CurBattleStage == ChiJiTimeTable.eBattleStage.BS_SAFE_AREA_3)
            {
                SystemNotifyManager.SystemNotify(11003);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiBattleStageChanged);
        }

        private void _OnBattleBalanceEnd(MsgDATA msg)
        {
            SceneBattleBalanceEnd msgData = new SceneBattleBalanceEnd();
            msgData.decode(msg.bytes);

#if UNITY_EDITOR
            Logger.LogErrorFormat("吃鸡时序测试----收到服务器结算消息[SceneBattleBalanceEnd] battleID {0} ChijiBattleID {1}", msgData.battleID, m_ChijiBattleID);
#endif
            if (msgData.battleID != m_ChijiBattleID && m_ChijiBattleID != 0)
            {
                Logger.LogErrorFormat("吃鸡时序测试----收到服务器结算消息ID不一致 battleID {0} ChijiBattleID {1}", msgData.battleID, m_ChijiBattleID);
                return;
            }
            m_Settlementinfo.ClearData();

            m_Settlementinfo.rank = msgData.rank;
            m_Settlementinfo.playerNum = msgData.playerNum;
            m_Settlementinfo.kills = msgData.kills;
            m_Settlementinfo.survivalTime = msgData.survivalTime;
            m_Settlementinfo.score = msgData.score;
            m_Settlementinfo.glory = msgData.getHonor;

            bool bCanOpenDirectly = false;

            var systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (systemTown != null)
            {
                CitySceneTable townTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (townTableData != null)
                {
                    if (townTableData.SceneSubType == CitySceneTable.eSceneSubType.Battle /*|| townTableData.SceneSubType == CitySceneTable.eSceneSubType.BattlePrepare*/)
                    {
                        bCanOpenDirectly = true;
                    }
                }
            }

            if (bCanOpenDirectly && !SwitchingChijiSceneToPrepare)
            {
                OpenSettlementFrame();
            }
            else
            {
                m_bGuardForSettlement = true;
            }
        }

        private void _OnBattleThrowSomeOne(MsgDATA msg)
        {
            SceneBattleThrowSomeoneItemRes msgData = new SceneBattleThrowSomeoneItemRes();
            msgData.decode(msg.bytes);

            if (msgData.retCode != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgData.retCode);
                return;
            }

            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }
            if (current.MainPlayer != null && current.MainPlayer.ActorData.GUID == msgData.attackID)
            {

                var chijiItemTable = TableManager.GetInstance().GetTableItem<ChijiItemTable>((int)msgData.itemDataID);
                if (chijiItemTable != null)
                {
                    string mContent = string.Empty;
                    //如果是手雷
                    if (chijiItemTable.SubType == ChijiItemTable.eSubType.ChijiGrenade)
                    {
                        mContent = TR.Value("Chiji_UseHandGrenades", msgData.targetName, msgData.param);
                    }
                    else if (chijiItemTable.SubType == ChijiItemTable.eSubType.ChijiMoveSpeed)
                    {
                        //如果是毒镖
                        mContent = TR.Value("Chiji_UsePoisonDart", msgData.targetName, msgData.param);
                    }

                    SystemNotifyManager.SysNotifyTextAnimation(mContent);
                }
                //主角不做表现
                return;
            }
            //   var targetPlayer = current.GetTownPlayer(msgData.targetID);
            //  var attackerPlayer = current.GetTownPlayer(msgData.attackID);
            var targetPlayer = current.GetPlayer(msgData.targetID);
            var attackerPlayer = current.GetPlayer(msgData.attackID);
            if (attackerPlayer != null)
            {
                attackerPlayer.CreateBullet(msgData.targetID, (int)msgData.itemDataID);
            }
        }

        private void _OnBattleSomeOneDead(MsgDATA msg)
        {
            var res = new SceneBattleNotifySomeoneDead();
            res.decode(msg.bytes);

            if (SwitchingChijiSceneToPrepare)
            {
                Logger.LogErrorFormat(string.Format("正在SwitchingChijiSceneToPrepare, 没有刷新m_SurvivePlayerNum，res.suvivalNum = {0}, m_SurvivePlayerNum = {1}", res.suvivalNum, m_SurvivePlayerNum));
                return;
            }

            if (res.battleID != m_ChijiBattleID || m_ChijiBattleID == 0)
            {
                Logger.LogErrorFormat(string.Format("吃鸡SomeDead battleid不一致，Cur ChijiBattleID = {0}, msgData.battleID = {1},协议:[SceneBattleNotifySomeoneDead]", m_ChijiBattleID, res.battleID));
                return;
            }

#if MG_TEST || UNITY_EDITOR
            JoinPlayerInfo deathinfo = JoinPlayerInfoList.Find(x => { return x.playerId == res.playerID; });
            if(deathinfo != null)
            {
                Logger.LogErrorFormat("吃鸡时序测试----some one dead, BattleID = {0}, name = {1}, id = {2}, reason = {3} from [_OnBattleSomeOneDead]", m_ChijiBattleID, deathinfo.playerName, res.playerID, res.reason);
            }
#endif

            m_SurvivePlayerNum = (int)res.suvivalNum;

            // 0:掉线，1：被杀，2：毒死，3：结算，4：超时
            if (res.reason == 1 || res.reason == 2)
            {
                if (PlayerBaseData.GetInstance().RoleID == res.playerID)
                {
                    m_isMainPlayerDead = true;
                }
                else
                {
                    m_otherPlayerDead.Add(res.playerID);
                }

                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                if (current != null) // 从战斗退出
                {
                    var player = current.GetPlayer(res.playerID);
                    if (player != null)
                    {
                        player.SetDead();
#if MG_TEST || UNITY_EDITOR
                        Logger.LogErrorFormat("吃鸡时序测试----收到其他玩家死亡消息, BattleID = {0}, playerid = {1}", m_ChijiBattleID, res.playerID);
#endif
                    }
                }

                if (res.killerID == PlayerBaseData.GetInstance().RoleID)
                {
                    m_RecentKillPlayerID = res.playerID;
                    m_KillNum = res.kills;
                }

                PlayerDeathReason Deathdata = new PlayerDeathReason();

                Deathdata.playerID = res.playerID;
                Deathdata.killerID = res.killerID;
                Deathdata.reason = res.reason;
                Deathdata.kills = res.kills;

                if (m_ShowDeathPlayerList.Count >= 3)
                {
                    m_ShowDeathPlayerList.RemoveAt(0);
                }

                m_ShowDeathPlayerList.Add(Deathdata);
            }
            else if (res.reason == 0)
            {
                JoinPlayerInfo playerinfo = JoinPlayerInfoList.Find(x => { return x.playerId == res.playerID; });
                if (playerinfo != null)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("玩家{0}已掉线", playerinfo.playerName));
                }
            }
            else if (res.reason == 4)
            {
                JoinPlayerInfo playerinfo = JoinPlayerInfoList.Find(x => { return x.playerId == res.playerID; });
                if (playerinfo != null)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("玩家{0}连接超时", playerinfo.playerName));
                }
            }
            else if (res.reason == 3)
            {
                JoinPlayerInfo playerinfo = JoinPlayerInfoList.Find(x => { return x.playerId == res.playerID; });
                if (playerinfo != null)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("玩家{0}已结算", playerinfo.playerName));
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiPlayerDead);
        }

        private void _OnPickUpOthersItem(MsgDATA msg)
        {
            SceneBattleNotifySpoilsItem msgData = new SceneBattleNotifySpoilsItem();
            msgData.decode(msg.bytes);

            if (msgData.battleID != m_ChijiBattleID)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(string.Format("吃鸡battleid不一致，Cur ChijiBattleID = {0}, msgData.battleID = {1}, 协议名: [SceneBattleNotifySpoilsItem]", m_ChijiBattleID, msgData.battleID));
                return;
            }

            m_OtherPlayerItems.battleID = msgData.battleID;
            m_OtherPlayerItems.playerID = msgData.playerID;

            m_OtherPlayerItems.detailItems.Clear();

            if (msgData.detailItems.Length > 0)
            {
                for (int i = 0; i < msgData.detailItems.Length; i++)
                {
                    OtherPlayerDetailItem item = new OtherPlayerDetailItem();

                    item.guid = msgData.detailItems[i].guid;
                    item.itemTypeId = msgData.detailItems[i].itemTypeId;
                    item.num = msgData.detailItems[i].num;

                    m_OtherPlayerItems.detailItems.Add(item);
                }

                bool bCanOpenDirectly = false;

                //  ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                var systemTown = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemGameBattle;
                if (systemTown != null)
                {
                    CitySceneTable townTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                    if (townTableData != null)
                    {
                        if (townTableData.SceneSubType == CitySceneTable.eSceneSubType.Battle)
                        {
                            bCanOpenDirectly = true;
                        }
                    }
                }

                if (!bCanOpenDirectly)
                {
                    m_bGuardForPickUpOtherPlayerItems = true;
                }
                else
                {
                    ClientSystemManager.GetInstance().OpenFrame<PlayerItemFrame>(FrameLayer.Middle);
                }
            }
        }

        private void _OnPickUpSpoilsRes(MsgDATA msg)
        {
            SceneBattlePickUpSpoilsRes msgData = new SceneBattlePickUpSpoilsRes();
            msgData.decode(msg.bytes);

            if (msgData.retCode != 0)
            {
                Logger.LogErrorFormat("[SceneBattlePickUpSpoilsRes] error, msgData.retCode = {0}", msgData.retCode);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PickUpLoserItem, msgData.itemGuid);
        }

        //         void _OnBattleLockSomeOneRes(MsgDATA msg)
        //         {
        //             BattleLockSomeOneRes msgData = new BattleLockSomeOneRes();
        //             msgData.decode(msg.bytes);
        // 
        //             if (msgData.retCode != 0)
        //             {
        //                 Logger.LogErrorFormat("[BattleLockSomeOneRes] error, msgData.retCode = {0}", msgData.retCode);
        //             }
        //         }

        void _OnPkSomeOneRes(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnPkSomeOneRes ==>> msg is nil");
                return;
            }

            BattlePkSomeOneRes msgData = new BattlePkSomeOneRes();
            msgData.decode(msg.bytes);

            if (msgData.retCode != 0)
            {
                ClientSystemGameBattle systemChiji = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                if (systemChiji == null)
                {
                    return;
                }

                CitySceneTable tableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemChiji.CurrentSceneID);
                if (tableData == null)
                {
                    return;
                }

                if (tableData.SceneType != CitySceneTable.eSceneType.BATTLE)
                {
                    return;
                }

                if (msgData.retCode == 4200002)
                {
                    SystemNotifyManager.SystemNotify(4200002);
                }
                else if (msgData.retCode == 4200003)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("对方处于无敌保护状态，你无法发起挑战");
                }
                else if (msgData.retCode == 4200004)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("对方正在pk中，你无法发起挑战");
                }
                else if (msgData.retCode == 4200005)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("自己处于无敌状态，你无法发起挑战");
                }
                else if (msgData.retCode == 4200006)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("自己正在pk中，你无法发起挑战");
                }
                else if(msgData.retCode == 4200012)
                {
                    //SystemNotifyManager.SysNotifyFloatingEffect("对方使用了免战令，30秒内无法继续挑战该玩家");
                    SystemNotifyManager.SystemNotify(4200012);
                }
                else if(msgData.retCode == 4200013)
                {
                    SystemNotifyManager.SystemNotify(4200013);

                    IsReadyPk = false;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiPKButtonChange);
                }
                else
                {
                    Logger.LogErrorFormat("_OnPkSomeOneRes Error, error id = {0}", msgData.retCode);
                }

                return;
            }
        }

        void _OnNpcNotify(MsgDATA msg)
        {
            SceneBattleNpcNotify msgData = new SceneBattleNpcNotify();
            msgData.decode(msg.bytes);

            for (int i = 0; i < msgData.battleNpcList.Length; i++)
            {
                int index = m_NpcDataList.FindIndex(x => { return msgData.battleNpcList[i].npcGuid == x.npcGuid; });

                if (index >= 0)
                {
                    m_NpcDataList[index] = msgData.battleNpcList[i];
                }
                else
                {
                    if (msgData.battleNpcList[i].opType == 1)
                    {
                        m_NpcDataList.Add(msgData.battleNpcList[i]);
                    }
                }

#if MG_TEST || UNITY_EDITOR
                //Logger.LogErrorFormat("吃鸡时序测试----收到NPC数据, BattleID = {0}, npcId = {1}, npcGuid = {2}, opType = {3}", m_ChijiBattleID, msgData.battleNpcList[i].npcId, msgData.battleNpcList[i].npcGuid, msgData.battleNpcList[i].opType);
#endif
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateChijiNpcData);
        }

        void _OnNpcTradeRes(MsgDATA msg)
        {
            SceneBattleNpcTradeRes msgData = new SceneBattleNpcTradeRes();
            msgData.decode(msg.bytes);

            if (msgData.retCode != 0)
            {
                if (msgData.retCode == 4200008)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("出手慢了，已被其他人击败！");
                }
                else
                {
                    SystemNotifyManager.SystemNotify((int)msgData.retCode);
                }

                return;
            }

            NpcTable tableData = TableManager.GetInstance().GetTableItem<NpcTable>((int)msgData.npcId);
            if (tableData != null && tableData.SubType == NpcTable.eSubType.ShopNpc)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("兑换成功");
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ExchangeSuccess);
        }

        void _OnOpenBoxRes(MsgDATA msg)
        {
            SceneBattleOpenBoxRes msgData = new SceneBattleOpenBoxRes();
            msgData.decode(msg.bytes);

            if (msgData.retCode != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgData.retCode);
                return;
            }

            if (msgData.param == 1)
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                if (current == null)
                {
                    return;
                }

                if (current.MainPlayer == null)
                {
                    return;
                }

                int pickUpTime = 0;

                List<BeItem> nearitems = current.MainPlayer.FindNearestTownItems();
                if (nearitems != null)
                {
                    for (int i = 0; i < nearitems.Count; i++)
                    {
                        if (nearitems[i].ActorData.GUID != msgData.itemGuid)
                        {
                            continue;
                        }

                        var chijiItemTable = TableManager.GetInstance().GetTableItem<ChijiItemTable>(nearitems[i].ItemID);
                        if (chijiItemTable == null)
                        {
                            continue;
                        }

                        pickUpTime = chijiItemTable.TimeLeft;

                        break;
                    }
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.StartOpenChijiItem, msgData.openTime, pickUpTime);
            }
            else if (msgData.param == 2)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CancelOpenChijiItem);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinishOpenChijiItem);
            }
        }

        void _OnNotifyBestRank(MsgDATA msg)
        {
            BattleNotifyBestRank msgData = new BattleNotifyBestRank();
            msgData.decode(msg.bytes);

            m_BestRank = (int)msgData.rank;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiBestRank);
        }

        void _OnNotifySkillChoiceList(MsgDATA msg)
        {
            BattleSkillChoiceListNotify msgData = new BattleSkillChoiceListNotify();
            msgData.decode(msg.bytes);

            var systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (systemTown != null)
            {
                CitySceneTable townTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (townTableData != null)
                {
                    if (townTableData.SceneType == CitySceneTable.eSceneType.BATTLE && townTableData.SceneSubType == CitySceneTable.eSceneSubType.Battle)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OpenChijiSkillChooseFrame, PickUpType.PickUpSkill, msgData.skillList);
                    }
                }
            }
        }

        void _OnChoiceSkillRes(MsgDATA msg)
        {
            BattleChoiceSkillRes msgData = new BattleChoiceSkillRes();
            msgData.decode(msg.bytes);

            SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>((int)msgData.skillId);

            if (skillData == null)
            {
                return;
            }

            SystemNotifyManager.SysNotifyFloatingEffect(string.Format("捡取技能 [{0}] Lv.{1}", skillData.Name, msgData.skillLvl));
        }

        void _OnEquipChoiceListRes(MsgDATA msg)
        {
            BattleEquipChoiceListNotify msgData = new BattleEquipChoiceListNotify();
            msgData.decode(msg.bytes);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OpenChijiSkillChooseFrame, PickUpType.PickUpItem, msgData.equipList);
        }

        void _OnChoiceEquipRes(MsgDATA msg)
        {
            BattleChoiceEquipRes msgData = new BattleChoiceEquipRes();
            msgData.decode(msg.bytes);

            if(msgData.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.retCode);
                return;
            }
        }

        void _OnNotifyNoWarOption(MsgDATA msg)
        {
            SceneBattleNoWarOption msgData = new SceneBattleNoWarOption();
            msgData.decode(msg.bytes);

            object[] args = new object[1];
            args[0] = msgData.enemyName;

            SystemNotifyManager.SystemNotify(6004, _OnClickAgree, _OnClickReject, 10.0f, args);
        }

        void _OnNotifyNoWarWait(MsgDATA msg)
        {
            SceneBattleNoWarWait msgData = new SceneBattleNoWarWait();
            msgData.decode(msg.bytes);

            SystemNotifyManager.SysNotifyFloatingEffect("发起挑战成功，请等待对手接受挑战!");

            IsReadyPk = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiPKButtonChange);
        }

        private void _OnClickAgree()
        {
            SceneBattleNoWarChoiceReq req = new SceneBattleNoWarChoiceReq();
            req.isNoWar = 1;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        private void _OnClickReject()
        {
            SceneBattleNoWarChoiceReq req = new SceneBattleNoWarChoiceReq();
            req.isNoWar = 0;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        void _OnNotifyNoWarChoiceRes(MsgDATA msg)
        {
            SceneBattleNoWarChoiceRes msgData = new SceneBattleNoWarChoiceRes();
            msgData.decode(msg.bytes);

            if (msgData.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.retCode);
                return;
            }
        }

        void _OnNoWarNotify(MsgDATA msg)
        {
            SceneBattleNoWarNotify msgData = new SceneBattleNoWarNotify();
            msgData.decode(msg.bytes);

            SystemNotifyManager.SysNotifyFloatingEffect("对方使用了免战令，30秒内无法对其发起挑战");
        }

        private void _OnChijiPkReadyFinish(UIEvent iEvent)
        {
            BattleMain.OpenBattle(BattleType.ChijiPVP, eDungeonMode.SyncFrame, (int)m_BattleDungeonId, m_SessionId.ToString());

            BaseBattle battle = BattleMain.instance.GetBattle() as BaseBattle;
            if (battle != null)
            {
                battle.PkRaceType = m_BattleRaceType;
            }

#if !LOGIC_SERVER
            if (ReplayServer.GetInstance().IsLiveShow())
            {
                ReplayServer.GetInstance().SetBattle(BattleMain.instance.GetBattle() as PVPBattle);
            }
#endif
            var current = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemGameBattle;
            if (current != null)
            {
                this.sceneData = current.LevelData;
            }
            ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();

            m_BattleDungeonId = 0;
            m_BattleRaceType = 0;
            m_SessionId = 0;
        }

        public void CreateBullet(ulong itemGuid, int itemID, ulong targetId)
        {
            var current = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            if (current.MainPlayer == null)
            {
                //主角不做表现
                return;
            }

            var req = new SceneBattleThrowSomeoneItemReq
            {
                battleID = m_ChijiBattleID,
                targetID = targetId,
                itemGuid = itemGuid
            };

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            current.MainPlayer.CreateBullet(targetId, itemID);
        }

        public void SendChijiBattleID()
        {
            var req = new SceneItemSync();
            req.battleID = m_ChijiBattleID;
            Logger.LogError("SendChijiBattleID");
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendSelectJobId(int jobid)
        {
            var req = new SceneBattleSelectOccuReq();
            req.occu = (byte)jobid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendSelectAreaId(int areaID)
        {
            SceneBattleBirthTransfer req = new SceneBattleBirthTransfer();
            req.regionID = (uint)areaID;
            req.battleID = m_ChijiBattleID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendPickUpBuffItems(UInt64 guid)
        {
            // 这是个捡东西的协议，协议名字与实际功能不符
            SceneBattleUseItemReq req = new SceneBattleUseItemReq();

            req.battleID = m_ChijiBattleID;
            req.uid = guid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

         public void SendPickUpMapBoxs(UInt64 guid)
         {
            SceneBattleOpenBoxReq req = new SceneBattleOpenBoxReq();

            req.itemGuid = guid;
            req.param = 1;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
         }

        public void SendPickUpOtherPlayerItems(UInt64 guid, UInt64 playerID)
        {
            SceneBattlePickUpSpoilsReq req = new SceneBattlePickUpSpoilsReq();

            req.battleID = m_ChijiBattleID;
            req.playerID = playerID;
            req.itemGuid = guid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendBattleLockSomeOneReq(UInt64 dstRoleID)
        {
            BattleLockSomeOneReq req = new BattleLockSomeOneReq();

            req.battleID = m_ChijiBattleID;
            req.roleId = PlayerBaseData.GetInstance().RoleID;
            req.dstId = dstRoleID;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendBattlePkSomeOneReq(UInt64 dstRoleID, int dungeonId)
        {
            BattlePkSomeOneReq req = new BattlePkSomeOneReq();

            req.battleID = m_ChijiBattleID;
            req.roleId = PlayerBaseData.GetInstance().RoleID;
            req.dstId = dstRoleID;
            req.dungeonID = (uint)dungeonId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendDelItemReq(UInt64 itemguid)
        {
            SceneBattleDelItemReq req = new SceneBattleDelItemReq();
            req.itemGuid = itemguid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendNpcTradeReq(UInt32 npcId, UInt64 npcGuid, List<UInt64> paramVec = null)
        {
            SceneBattleNpcTradeReq req = new SceneBattleNpcTradeReq();

            req.npcGuid = npcGuid;
            req.npcId = npcId;

            if (paramVec != null)
            {
                req.paramVec = new ulong[paramVec.Count];

                for (int i = 0; i < paramVec.Count; i++)
                {
                    req.paramVec[i] = paramVec[i];
                }
            }

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendSelectSkillReq(UInt32 skillid)
        {
            BattleChoiceSkillReq req = new BattleChoiceSkillReq();
            req.skillId = skillid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendSelectItemReq(UInt32 itemid)
        {
            BattleChoiceEquipReq req = new BattleChoiceEquipReq();
            req.equipId = itemid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public int GetPrepareScenePlayerNum()
        {
            return m_PrepareScenePlayerNum;
        }
        
        /// <summary>
        /// //判断字符串是否为吃鸡荣耀战场字符串，用于判断活动副本
        /// </summary>
        /// <param name="judgeStr"></param>
        /// <returns></returns>
        public bool IsHonorBattleFieldStr(string judgeStr)
        {
            if (string.IsNullOrEmpty(judgeStr) == true)
                return false;
            if (judgeStr == HonorBattleFieldStr)
                return true;
            return false;
        }

        /// <summary>
        /// 判断吃鸡活动是否开启
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public bool MainFrameChijiButtonIsShow()
        {
            bool isShow = false;

            for (int i = 0; i < ChijiActivityIDs.Length; i++)
            {
                int activityId = ChijiActivityIDs[i];

                if (!ActiveManager.GetInstance().allActivities.ContainsKey(activityId))
                {
                    continue;
                }

                var activityInfo = ActiveManager.GetInstance().allActivities[activityId];

                if (activityInfo.state == (byte)StateType.Running)
                {
                    isShow = true;
                }
            }
           
            return isShow;
        }

        /// <summary>
        /// 得到吃鸡开放等级
        /// </summary>
        /// <returns></returns>
        public int GetChijiOpenLevel()
        {
            for (int i = 0; i < ChijiActivityIDs.Length; i++)
            {
                int activityId = ChijiActivityIDs[i];

                if (!ActiveManager.GetInstance().allActivities.ContainsKey(activityId))
                {
                    continue;
                }

                var activityInfo = ActiveManager.GetInstance().allActivities[activityId];

                if (activityInfo.state == (byte)StateType.Running)
                {
                    return activityInfo.level;
                }
            }

            return 0;
        }

        /// <summary>
        /// 打开结算界面，根据排名打开胜利和失败界面
        /// </summary>
        public void OpenSettlementFrame()
        {
            SettlementInfo settlementInfo = ChijiDataManager.GetInstance().Settlementinfo;
            if (settlementInfo.rank == 1)
            {
                ClientSystemManager.GetInstance().OpenFrame<ChijiSettlementFrame>();
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<ChijiSettlementLoseFrame>();
            }
        }

        public void ResponseBattleEnd()
        {
            IsToPrepareScene = false;
            SwitchingChijiSceneToPrepare = false;

            // 失败退出到城镇要清掉吃鸡相关数据
            if ((PKResult)m_pkEndData.result == PKResult.WIN)
            {
                ClearBattleData();
                ClientSystemManager.instance.SwitchSystem<ClientSystemGameBattle>();
            }
            else
            {
                ClearAllRelatedSystemData();
                IsToPrepareScene = true;
                SwitchingChijiSceneToPrepare = true; // 从战斗退出到准备场景和从吃鸡场景退出到准备场景要做的事情一样

                ClientSystemManager.instance.SwitchSystem<ClientSystemGameBattle>();
            }
        }

        /// <summary>
        /// 检查当前场景是否是吃鸡场景
        /// </summary>
        /// <returns></returns>
        public bool CheckCurrentSystemIsClientSystemGameBattle()
        {
            var currentSystem = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (currentSystem == null)
            {
                return false;
            }

            return true;
        }

        public void SendOccuListReq()
        {
            SceneBattleOccuListReq req = new SceneBattleOccuListReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        public int _GetWeeklyTotalGlory()
        {
            int tempValue = 0;
            if (CountDataManager.GetInstance() != null)
            {
                tempValue = CountDataManager.GetInstance().GetCount("chi_ji_honor");
            }
            return tempValue;
        }
        public int _GetWeeklyMaxPVPGlory()
        {
            int tempValue = 0;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_PK_CHIJI_HONOR_MAX);
            if (SystemValueTableData != null)
            {
                tempValue = SystemValueTableData.Value;
            }
            return tempValue;
        }
    }
}
