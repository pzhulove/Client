using System.Collections.Generic;
using Protocol;
using System;
using System.Collections;
using Network;
using System.Diagnostics;
using GameClient;
using UnityEngine;
using ProtoTable;

[LoggerModel("GlobalNotify")]
public class GlobalNetMessage : GameClient.GameBindSystem
{
    #region Instance
    private static GlobalNetMessage mInstance;
    public static GlobalNetMessage instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new GlobalNetMessage();
            }

            return mInstance;
        }
    }

    public void Load()
    {
        if (mInstance != null)
        {
            mInstance.InitBindSystem(null);
        }
    }

    public void Unload()
    {
        if (mInstance != null)
        {
            mInstance.ExistBindSystem();
            mInstance = null;
        }
    }
    #endregion

    #region DungeonGlobalNotify
    //[ProtocolHandle(typeof(SceneDungeonSyncNewOpenedList))]
    //private void _onSceneDungeonSyncNewOpenedList(SceneDungeonSyncNewOpenedList rep)
    //{
    //    Logger.LogProcessFormat(ObjectDumper.Dump(rep));
    //    BattleDataManager.GetInstance().DungeonOpenList(rep);
    //}

    //[ProtocolHandle(typeof(SceneDungeonInit))]
    //private void _onSceneDungeonInit(SceneDungeonInit rep)
    //{
    //    Logger.LogProcessFormat(ObjectDumper.Dump(rep));
    //    BattleDataManager.GetInstance().DungeonInit(rep);
    //}

    //[ProtocolHandle(typeof(SceneDungeonUpdate))]
    //private void _onSceneDungeonUpdate(SceneDungeonUpdate rep)
    //{
    //    Logger.LogProcessFormat(ObjectDumper.Dump(rep));
    //    BattleDataManager.GetInstance().DungeonUpdate(rep);
    //}

    //[ProtocolHandle(typeof(SceneDungeonHardInit))]
    //private void _onSceneDungeonHardInit(SceneDungeonHardInit rep)
    //{
    //    Logger.LogProcessFormat(ObjectDumper.Dump(rep));
    //    BattleDataManager.GetInstance().DungeonHardInit(rep);
    //}

    //[ProtocolHandle(typeof(SceneDungeonHardUpdate))]
    //private void _onSceneDungeonHardUpdate(SceneDungeonHardUpdate rep)
    //{
    //    Logger.LogProcessFormat(ObjectDumper.Dump(rep));

    //    BattleDataManager.GetInstance().DungeonHardUpdate(rep);

    //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonUnlockDiff, (int)rep.info.id, (int)rep.info.unlockedHardType);
    //}

    #endregion

    #region Buff Notify

    [ProtocolHandle(typeof(SceneNotifyRemoveBuff))]
    private void _onSceneNotifyRemoveBuff(SceneNotifyRemoveBuff rep)
    {
        Logger.LogProcessFormat("[buffdrug] 服务器通知删除buff {0}", rep.buffId);

        var buffList = GameClient.PlayerBaseData.GetInstance().buffList;
        var removeBuffList = GameClient.PlayerBaseData.GetInstance().removedBuffList;

        var countFromList = buffList.RemoveAll(buff => { return buff.id == rep.buffId; });
        if (countFromList > 0)
        {
            Logger.LogProcessFormat("[buffdrug] 从bufflist中移除 buff.uid {0} with count {1}", rep.buffId, countFromList);
        }

        var countFromRemoveList = removeBuffList.RemoveAll(buff => { return buff.id == rep.buffId; });
        if (countFromRemoveList > 0)
        {
            Logger.LogProcessFormat("[buffdrug] 从removedbufflist中移除 buff.uid {0} with count {1}", rep.buffId, countFromRemoveList);
        }

        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BuffRemoved, (int)rep.buffId);

        //var frame = GameClient.ClientSystemManager.instance.GetFrame(typeof(GameClient.DungeonTipListFrame)) as GameClient.DungeonTipListFrame;
        //if (frame != null)
        //{
        //    frame.RemoveBuff((int)rep.buffId);
        //}

        //var curSystem = GameClient.ClientSystemManager.instance.CurrentSystem as GameClient.ClientSystemBattle;
        //if (curSystem != null)
        //{
        //    //curSystem.dungeonTipList.RemoveBuff((int)rep.buffId);
        //}
    }
    #endregion

    [ProtocolHandle(typeof(SceneDungeonChestNotify))]
    private void _onSceneDungeonChestNotify(SceneDungeonChestNotify res)
    {
        Logger.LogProcessFormat(ObjectDumper.Dump(res));

        GameClient.BattleDataManager.GetInstance().chestInfo.count  = res.payChestCost;
        GameClient.BattleDataManager.GetInstance().chestInfo.itemId = res.payChestCostItemId;
    }

    //[ProtocolHandle(typeof(SceneDungeonAddMonsterDropItemNotify))]
    //private void _onSceneDungeonAddMonsterDropItemNotify(SceneDungeonAddMonsterDropItemNotify res)
    //{
    //    Logger.Log(ObjectDumper.Dump(res));
    //    BattleDataManager.GetInstance().DungeonAddMonsterDropItemNotify(res);
    //}


    #region SysNotify

    [ProtocolHandle(typeof(SysNotify))]
    private void _onSysNotify(SysNotify rep)
    {
        Logger.LogProcessFormat(ObjectDumper.Dump(rep));

        SystemNotifyManager.SysNotifyFromServer(rep);
    }

    #endregion

    #region SysAnnouncement

    [ProtocolHandle(typeof(SysAnnouncement))]
    private void _onAnnouncement(SysAnnouncement rep)
    {
        //Logger.LogProcessFormat(ObjectDumper.Dump(rep));
        AnnouncementManager.GetInstance().AnnounceFromServer(rep);
    }

    #endregion

    #region FuncUnlock

    [ProtocolHandle(typeof(SceneSyncFuncUnlock))]
    private void _onNewFuncUnlock(SceneSyncFuncUnlock rep)
    {
        Logger.LogProcessFormat(ObjectDumper.Dump(rep));

        FunctionUnLock table = TableManager.GetInstance().GetTableItem<FunctionUnLock>(rep.funcId);
        if (table == null)
        {
            return;
        }

        if (table.IsOpen == false)
        {
            return;
        }

        //过滤 帐号绑定功能解锁类型
        if (table.BindType == FunctionUnLock.eBindType.BT_AccBind)
        {
            return;
        }

        if (table.Type == FunctionUnLock.eType.Area)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewAreaUnlock, table.AreaID);
        }
        else if (table.Type == FunctionUnLock.eType.Func)
        {
            //PlayerBaseData.GetInstance().NewUnlockFuncList.Add(rep.funcId);
            //PlayerBaseData.GetInstance().UnlockFuncList.Add(rep.funcId);

            if (table.bPlayAnim == 1)
            {
                if (!PlayerBaseData.GetInstance().IsFlyUpState)
                {
                    if(!PlayerBaseData.GetInstance().NewUnlockFuncList.Contains(rep.funcId))
                    {
                    PlayerBaseData.GetInstance().NewUnlockFuncList.Add(rep.funcId);
                    }
                    if(table.FuncType == FunctionUnLock.eFuncType.AdventurePassSeason)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassButtonRedPoint, true);
                    }                    
                }
            }
            else
            {
                PlayerBaseData.GetInstance().NewUnlockNotPlayFuncList.Add(rep.funcId);             
                if (table.FuncType == FunctionUnLock.eFuncType.AdventurePassSeason)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateAventurePassButtonRedPoint, true);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewFuncUnlock, rep.funcId, table.FuncType);
        }
    }

    [ProtocolHandle(typeof(WorldSyncFuncUnlock))]
    private void _OnAccountFuncUnlock(WorldSyncFuncUnlock res)
    {
        if (res == null) return;
        Logger.LogProcessFormat(ObjectDumper.Dump(res));

        FunctionUnLock table = TableManager.GetInstance().GetTableItem<FunctionUnLock>(res.funcId);
        if (table == null)
        {
            return;
        }

        if (table.IsOpen == false)
        {
            return;
        }

        //过滤 角色绑定功能解锁类型
        if (table.BindType == FunctionUnLock.eBindType.BT_RoleBind)
        {
            return;
        }

        if (table.Type == FunctionUnLock.eType.Func)
        {
            if (table.bPlayAnim == 1)
            {
                if(!PlayerBaseData.GetInstance().NewUnlockFuncList.Contains(res.funcId))
                {
                PlayerBaseData.GetInstance().NewUnlockFuncList.Add(res.funcId);
                }                
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewAccountFuncUnlock, res.funcId);
        }
    }
    #endregion

    #region 秘药飞升状态同步
    [ProtocolHandle(typeof(SceneSyncFlyUpStatus))]
    private void _OnSyncFlyUpStatus(SceneSyncFlyUpStatus rep)
    {
        if(rep == null)
        {
            return;
        }

        if(rep.status == 0)
        {
            if (PlayerBaseData.GetInstance().IsFlyUpState)
            {
                // 飞升结束要把新手引导数据都跳过发给服务器
                // 只处理强制引导，由任务触发的引导不需要处理，因为服务器已经把对应跨越等级段的任务都按完成算了
                NewbieGuideManager.GetInstance().SendSaveBoot(NewbieGuideTable.eNewbieGuideTask.AncientGuide);
            }

            // 要等新手引导的服务器返回后再置位
            //PlayerBaseData.GetInstance().IsFlyUpState = false;
        }
        else
        {
            PlayerBaseData.GetInstance().IsFlyUpState = true;

            // 在点击使用秘药的时候客户端就已经触发引导了，所以这里在收到消息的时候要处理一下
            NewbieGuideManager.GetInstance().ManagerFinishGuide(NewbieGuideManager.GetInstance().GetCurTaskID());
        }
    }
    #endregion

    #region 开始战斗

    private void _setRacePlayers(RacePlayerInfo[] players)
    {
        BattleDataManager.GetInstance().PlayerInfo = players;
        ClientApplication.racePlayerInfo = players;
        for (int i = 0; i < ClientApplication.racePlayerInfo.Length; ++i)
        {
            var current = ClientApplication.racePlayerInfo[i];
            if (current.accid == ClientApplication.playerinfo.accid)
            {
                ClientApplication.playerinfo.seat = current.seat;
            }
        }
    }

    private void _setRaceRelayServer(ulong session, SockAddr addr)
    {
        ClientApplication.playerinfo.session = session;
        ClientApplication.relayServer.ip = addr.ip;
        ClientApplication.relayServer.port = addr.port;
    }

    //public static bool ReceiveEnterDungeonMsgWhenQuitDungeon = false;
    //public static uint NeedEnterDungeonIdWhenQuitDungeon = 0;

    /// <summary>
    /// 关卡开始通知
    /// </summary>
    /// <param name="rep"></param>
    [ProtocolHandle(typeof(SceneDungeonStartRes))]
    private void _onSceneDungeonStartRes(SceneDungeonStartRes rep)
    {
        Logger.LogProcessFormat(ObjectDumper.Dump(rep));

        var system = ClientSystemManager.instance.CurrentSystem;

        bool isCreateFromBattle = false;
        if (null != system && system.IsSystem<ClientSystemBattle>())
        {
            isCreateFromBattle = true;
        }

        {
            if (0 == rep.result)
            {
                // 测试代码注释
                //                 ClientSystemTown systemTown = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                // 
                //                 if (ClientSystemManager.GetInstance().isSwitchSystemLoading && systemTown != null)
                //                 {
                //                     ReceiveEnterDungeonMsgWhenQuitDungeon = true;
                //                     NeedEnterDungeonIdWhenQuitDungeon = rep.dungeonId;
                //                 }
                BattleDataManager.GetInstance().PkRaceType = (byte)RaceType.Dungeon;
                BattleDataManager.GetInstance().ClearBatlte();
                BattleDataManager.GetInstance().ConvertDungeonBattleInfo(rep);

                BattleDataManager.GetInstance().originExp = GameClient.PlayerBaseData.GetInstance().Exp;

                Global.Settings.canUseAutoFight = rep.openAutoBattle > 0;
                if (rep.openAutoBattle == 2)
                    Global.Settings.canUseAutoFightFirstPass = true;
                else
                    Global.Settings.canUseAutoFightFirstPass = false;

                if(rep.session == 0)
                {
                    rep.session = 151248698618683818;
                }

                _setRacePlayers(rep.players);
                _setRaceRelayServer(rep.session, rep.addr);

                eDungeonMode mode = eDungeonMode.None;
                if (rep.addr.port == 0)
                {
                    mode = eDungeonMode.LocalFrame;
                }
                else
                {
                    mode = eDungeonMode.SyncFrame;
                }

                BattleType type = ChapterUtility.GetBattleType((int)rep.dungeonId);
                var battleMain = BattleMain.OpenBattle(type, mode, (int)rep.dungeonId,rep.session.ToString());
                
#if !SERVER_LOGIC
                bool isRecordLog    = rep.isRecordLog != 0;
                bool isRecordReplay = rep.isRecordReplay != 0;
                RecordServer.GetInstance().StartRecord(type, mode, rep.session.ToString(), isRecordLog, isRecordReplay);
                RecordServer.GetInstance().RecordDungoenInfo(rep);

                //提供给Bugly使用的接口
                string dungeonInfo = string.Format("{0}_{1}", rep.dungeonId, rep.session);
                if (PluginManager.GetInstance() != null)
                {
                    PluginManager.GetInstance().SetBuglyVerIdInfo(dungeonInfo);
                }
#endif
                if (battleMain != null)
                {
                    var baseBattle = battleMain.GetBattle() as BaseBattle;

                    //存储队友其他关卡通关消息
                    if (baseBattle != null)
                    {
						baseBattle.SetBattleFlag(rep.battleFlag);

                        baseBattle.SetDungeonClearInfo(rep.clearedDungeonIds);
                        var raidBattle = baseBattle as RaidBattle;
                        if(raidBattle != null)
                        {
                        for (int i = 0; i < rep.clearedDungeonIds.Length; i++)
                        {
                                raidBattle.DungeonDestroyNotify((int)rep.clearedDungeonIds[i]);
                            }
                        }
                    }
                    var guildBattle = battleMain.GetBattle() as GuildPVEBattle;
                    if (guildBattle != null)
                    {
                        if (rep.guildDungeonInfo != null)
                        {
                            guildBattle.SetBossInfo(rep.guildDungeonInfo.bossOddBlood, rep.guildDungeonInfo.bossTotalBlood);
                            guildBattle.SetBuffInfo(rep.guildDungeonInfo.buffVec);
                        }
                        else
                        {
                            Logger.LogErrorFormat("[PVE] GuildPveBattle sessionid {0} is Null", rep.session);
                        }
                    }
                    else
                    {
                        var finalTestBattle = battleMain.GetBattle() as FinalTestBattle;
                        if (finalTestBattle != null && rep.zjslDungeonInfo != null)
                        {
                            finalTestBattle.SetBossInfo(rep.zjslDungeonInfo.boss1ID, rep.zjslDungeonInfo.boss1RemainHp, rep.zjslDungeonInfo.boss2ID, rep.zjslDungeonInfo.boss2RemainHp);
                            finalTestBattle.SetBuffInfo(rep.zjslDungeonInfo.buffVec);
                        }
                        else
                        {
                            var pveBattle = battleMain.GetBattle() as PVEBattle;
                            if (pveBattle != null && rep.hellAutoClose == 1)
                            {
                                pveBattle.OpenHellClose();
                            }
                        }
                    }
                }
                ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>(null, null, true);
#if ROBOT_TEST
                AutoFightRunTime.GetInstance().OnDungeonStart();
#endif
                //GameClient.GameFrameWork.instance.StartCoroutine(_startBattle(mode, rep.addr, rep.session, ()=>
                //{
                //    if (mode != eDungeonMode.None)
                //    {
                //        BattleType type = ChapterUtility.GetBattleType((int)rep.dungeonId);
                //        BattleMain.OpenBattle(type, mode, (int)rep.dungeonId);
                //        ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
                //    }
                //}));
            }
            else
            {
                if (rep.result != (UInt32)ProtoErrorCode.DUNGEON_TEAM_START_RACE_FAILED && rep.result != (UInt32)ProtoErrorCode.DUNGEON_TEAM_WAIT_OTHER_VOTE)
                {
                    SystemNotifyManager.SystemNotify((int)rep.result);
                }
            }
        }
    }

    public delegate void ConnnectRelayCallback();

    private IEnumerator _startBattle(eDungeonMode mode, SockAddr addr, ulong session, ConnnectRelayCallback cb)
    {
        if (eDungeonMode.SyncFrame == mode)
        {
            ClientApplication.playerinfo.session = session;
            ClientApplication.relayServer.ip = addr.ip;
            ClientApplication.relayServer.port = addr.port;

            WaitServerConnected waitConnect = new WaitServerConnected(ServerType.RELAY_SERVER,
                ClientApplication.relayServer.ip,
                ClientApplication.relayServer.port,
                ClientApplication.playerinfo.accid
            );

            yield return waitConnect;

            if (waitConnect.GetResult() == WaitServerConnected.eResult.Success)
            {
                GameClient.ClientReconnectManager.instance.canReconnectRelay = true;

                if (null != cb)
                {
                    cb.Invoke();
                }
            }
        }
        else
        {
            if (null != cb)
            {
                cb.Invoke();
            }
        }
    }

    [ProtocolHandle(typeof(GateNotifyKickoff))]
    private void _onGateNotifyKickoff(GateNotifyKickoff res)
    {
        Logger.LogProcessFormat("[登录] 被顶号");

        ClientReconnectManager.instance.canRelogin = false;
        ClientReconnectManager.instance.canReconnectGate = false;

#if !LOGIC_SERVER
        if (null != BattleMain.instance && res.errorCode == 8932)
        {
            RecordServer.GetInstance().MarkRecordFileNeedUpload();
        }
#endif

        ClientSystemManager.instance.QuitToLoginSystem((int)res.errorCode);
        //SystemNotifyManager.SystemNotify((int)res.errorCode, ()=>
        //{
        //    ClientSystemManager.instance._QuitToLoginImpl();
        //});
        //IClientNet net = ClientSystemManager.instance.CurrentSystem as IClientNet;
        //if (null != net)
        //{
        //    net.OnKickOff();
        //}
    }

    /// <summary>
    ///  PK开始通知
    /// </summary>
    /// <param name="rep"></param>
    [ProtocolHandle(typeof(WorldNotifyRaceStart))]
    private void _onPkRaceStart(WorldNotifyRaceStart rep)
    {
        Logger.LogProcessFormat(ObjectDumper.Dump(rep));

        if (rep.raceType == (byte)RaceType.PK || rep.raceType == (byte)RaceType.GuildBattle || rep.raceType == (byte)RaceType.PremiumLeagueBattle || rep.raceType == (byte)RaceType.PremiumLeaguePreliminay || rep.raceType == (byte)RaceType.PK_3V3
            || rep.raceType == (byte)RaceType.ScoreWar ||rep.raceType==(byte)RaceType.PK_3V3_Melee || rep.raceType == (byte)RaceType.PK_2V2_Melee || rep.raceType == (byte)RaceType.ChiJi || rep.raceType == (byte)RaceType.PK_EQUAL_1V1)
        {
#if !LOGIC_SERVER
            //if (RecordServer.GetInstance().IsReplayRecord(true))
            //{
            //
			bool isRecordProcess = rep.isRecordLog != 0;
			bool isRecordReplay  = rep.isRecordReplay != 0;

            //if (rep.raceType == (byte)RaceType.PK_3V3)
            //{
            //    // pk3v3  关日志关录像
            //    isRecordProcess = false;
            //    isRecordReplay  = false;
            //}
            //else
            //{
            //    // 其他pk
            //    isRecordProcess = false;
            //    isRecordReplay  = true;
            //}
#if !MG_TEST
            isRecordProcess = false;
            isRecordReplay = true;
#endif

            RecordServer.GetInstance().StartRecord(BattleType.MutiPlayer, eDungeonMode.SyncFrame, rep.sessionId.ToString(), isRecordProcess, isRecordReplay);
            RecordServer.GetInstance().RecordPlayerInfo(rep);
            if (ReplayServer.GetInstance().IsLiveShow())
            {
                ReplayServer.GetInstance().StartLiveShow();
            }
            //}
#endif

            _setRacePlayers(rep.players);
            if (ReplayServer.GetInstance() != null && ReplayServer.GetInstance().IsLiveShow())
            {
                if (ReplayServer.GetInstance().LiveShowServerAddr == null)
                {
                    Logger.LogErrorFormat("no available live show address");
#if !LOGIC_SERVER
                    RecordServer.GetInstance().EndRecord("Address Error");
                    ReplayServer.GetInstance().EndReplay();
#endif
                    return;
                }
                _setRaceRelayServer(rep.sessionId, ReplayServer.GetInstance().LiveShowServerAddr);
            }
            else
            {
                _setRaceRelayServer(rep.sessionId, rep.addr);
            }

            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.PKMatched, rep.raceType);

            try
            {
                BattleDataManager.GetInstance().PkRaceType = (RaceType)rep.raceType;
            }
            catch (Exception e)
            {

            }
           
            if (rep.raceType == (byte)RaceType.GuildBattle)
            {
                BattleMain.OpenBattle(BattleType.GuildPVP,eDungeonMode.SyncFrame, (int)rep.dungeonId, rep.sessionId.ToString());
            }
            else if(rep.raceType == (byte)RaceType.PremiumLeaguePreliminay || rep.raceType == (byte)RaceType.PremiumLeagueBattle)
            {
                BattleMain.OpenBattle(BattleType.MoneyRewardsPVP,eDungeonMode.LocalFrame, (int)rep.dungeonId, rep.sessionId.ToString());
            }
            else if (rep.raceType == (byte)RaceType.PK_3V3 || rep.raceType == (byte)RaceType.ScoreWar || rep.raceType == (byte)RaceType.PK_3V3_Melee 
                || rep.raceType == (byte)RaceType.PK_2V2_Melee)
            {
                if(rep.raceType == (byte)RaceType.ScoreWar)
                {
                    Pk3v3CrossDataManager.GetInstance().bMatching = false;

                    WorldNotifyRaceStart rep2 = new WorldNotifyRaceStart();
                    rep2 = rep;

                    GameFrameWork.instance.StartCoroutine(_PK3v3CrossMatchOK(rep2));
                    return;
                }
                if (rep.raceType == (byte)RaceType.PK_2V2_Melee)
                {
                    Pk2v2CrossDataManager.GetInstance().bMatching = false;
                    WorldNotifyRaceStart rep2 = new WorldNotifyRaceStart();
                    rep2 = rep;
                    GameFrameWork.instance.StartCoroutine(_PK2v2CrossMatchOK(rep2));
                    return;
                }


                if (rep.raceType == (byte)RaceType.PK_3V3_Melee)
                {
                    BattleMain.OpenBattle(BattleType.ScufflePVP, eDungeonMode.SyncFrame, (int)rep.dungeonId, rep.sessionId.ToString());
                }
                else
                {
                    BattleMain.OpenBattle(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, (int)rep.dungeonId, rep.sessionId.ToString());
                }
            }
            else if (rep.raceType == (byte)RaceType.ChiJi)
            {
                ChijiDataManager.GetInstance().BattleDungeonId = rep.dungeonId;
                ChijiDataManager.GetInstance().BattleRaceType = rep.raceType;
                ChijiDataManager.GetInstance().SessionId = rep.sessionId;
                ChijiDataManager.GetInstance().IsReadyPk = false;

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiPkReady);

                // 吃鸡单局因为要做假的等待，所以不能即使执行后面的流程
                //BattleMain.OpenBattle(BattleType.ChijiPVP, eDungeonMode.SyncFrame, (int)rep.dungeonId, rep.sessionId.ToString());
            }
            else
            {
                BattleMain.OpenBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, (int)rep.dungeonId,rep.sessionId.ToString());
            }
            if (rep.raceType != (byte)RaceType.ChiJi)
            {
                BaseBattle battle = BattleMain.instance.GetBattle() as BaseBattle;
                if (battle != null)
                {
                    battle.PkRaceType = (int)rep.raceType;
                }

#if !LOGIC_SERVER
                if (ReplayServer.GetInstance().IsLiveShow())
                {
                    ReplayServer.GetInstance().SetBattle(BattleMain.instance.GetBattle() as PVPBattle);
                }
#endif
                ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
            }
        }
    }

    public IEnumerator _PK3v3CrossMatchOK(WorldNotifyRaceStart rep)
    {
        DeviceVibrateManager.GetInstance().TriggerDeviceVibrateByType(VibrateSwitchType.Pk3v3);
        ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMatchOKFrame>();

        yield return Yielders.GetWaitForSeconds(3.0f);


        BattleMain.OpenBattle(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, (int)rep.dungeonId, rep.sessionId.ToString());
        BaseBattle battle = BattleMain.instance.GetBattle() as BaseBattle;
        if (battle != null)
        {
            battle.PkRaceType = (int)rep.raceType;
        }
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsLiveShow())
        {
            ReplayServer.GetInstance().SetBattle(BattleMain.instance.GetBattle() as PVPBattle);
        }
#endif
        ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
        yield return 0;
    }
    public IEnumerator _PK2v2CrossMatchOK(WorldNotifyRaceStart rep)
    {
        ClientSystemManager.GetInstance().OpenFrame<Pk2v2CrossMatchOKFrame>();
        yield return Yielders.GetWaitForSeconds(3.0f);
        BattleMain.OpenBattle(BattleType.ScufflePVP, eDungeonMode.SyncFrame, (int)rep.dungeonId, rep.sessionId.ToString());
        BaseBattle battle = BattleMain.instance.GetBattle() as BaseBattle;
        if (battle != null)
        {
            battle.PkRaceType = (int)rep.raceType;
        }
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsLiveShow())
        {
            ReplayServer.GetInstance().SetBattle(BattleMain.instance.GetBattle() as PVPBattle);
        }
#endif
        ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();

        yield return 0;
    }

#endregion

#region Mail
    [ProtocolHandle(typeof(WorldNotifyNewMail))]
    private void _onWorldNotifyNewMail(WorldNotifyNewMail rep)
    {
        Logger.LogProcessFormat(ObjectDumper.Dump(rep));

        if ((MailType)rep.info.type == MailType.MAIL_TYPE_GM)
        {
            PlayerBaseData.GetInstance().mails.mailList.Insert(0, rep.info);
            PlayerBaseData.GetInstance().mails.UpdateOneKeyNum();
            PlayerBaseData.GetInstance().mails.UnreadNum += 1;
        }
        else
        {
            PlayerBaseData.GetInstance().mails.rewardMailList.Insert(0, rep.info);
            PlayerBaseData.GetInstance().mails.UpdateOneKeyRewardNum();
            PlayerBaseData.GetInstance().mails.UnreadRewardNum += 1;
        }

       

        //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewMailNotify);
    }
#endregion

#region PK Data
    [MessageHandle(SceneSyncPkStatisticInfo.MsgID)]
    void _OnInitPkDataStatistics(MsgDATA msg)
    {
        if (msg == null)
        {
            Logger.LogError("_OnInitPkDataStatistics ==>> msg is nil");
            return;
        }

        int pos = 0;
        Dictionary<byte, PkStatistic> PkStatisticObj = PkStatisticDecoder.DecodeSyncPkStatisticInfoMsg(msg.bytes, ref pos, msg.bytes.Length);

        if (PkStatisticObj == null)
        {
            return;
        }

        Logger.LogProcessFormat(ObjectDumper.Dump(PkStatisticObj));

        GameClient.PlayerBaseData.GetInstance().PkStatisticsData.Clear();
        GameClient.PlayerBaseData.GetInstance().PkStatisticsData = PkStatisticObj;
    }

    [MessageHandle(SceneSyncPkStatisticProperty.MsgID)]
    void _OnUpdatePkDataStatistics(MsgDATA msg)
    {
        if (msg == null)
        {
            Logger.LogError("_OnUpdatePkDataStatistics ==>> msg is nil");
            return;
        }

        int pos = 0;
        byte DataTypeId = 0;
        BaseDLL.decode_int8(msg.bytes, ref pos, ref DataTypeId);

        PkStatistic StatisticData = null;
        if (!GameClient.PlayerBaseData.GetInstance().PkStatisticsData.TryGetValue(DataTypeId, out StatisticData))
        {
            Logger.LogWarningFormat("_OnUpdatePkDataStatistics ==>> StatisticDataId {0} isn't in PkStatisticsData", DataTypeId);
        }

        if (StatisticData == null)
        {
            StatisticData = new PkStatistic();
        }

        // 这里已经把客户端数据PkStatisticsData更新过了
        PkStatisticDecoder.DecodeSyncPkStatisticPropertyMsg(ref StatisticData, msg.bytes, ref pos, msg.bytes.Length);

        Logger.LogProcessFormat(ObjectDumper.Dump(StatisticData));

        if (!GameClient.PlayerBaseData.GetInstance().PkStatisticsData.ContainsKey(DataTypeId))
        {
            GameClient.PlayerBaseData.GetInstance().PkStatisticsData.Add(DataTypeId, StatisticData);
        }
        else
        {
            for (int i = 0; i < StatisticData.dirtyFields.Count; i++)
            {
                PkStatisticProperty DataAttr = (PkStatisticProperty)StatisticData.dirtyFields[i];

                if (DataAttr == PkStatisticProperty.PKIA_CUR_WIN_STEAK)
                {
                    GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.PkCurWinSteak);
                    continue;
                }
            }
        }
    }

    //     [MessageHandle(SceneMatchPkRaceEnd.MsgID)]
    //     void _OnReceivePkEndData(MsgDATA msg)
    //     {
    //         SceneMatchPkRaceEnd ret = new SceneMatchPkRaceEnd();
    //         ret.decode(msg.bytes);
    // 
    //         Logger.LogWarningFormat("ReceivePkEndData {0}\n", ObjectDumper.Dump(ret));
    // 
    //         PlayerBaseData.GetInstance().PkEndData = ret;
    // 
    //         PKRankDataManager.GetInstance().pkRankID = (int)ret.newSeasonLevel;
    //         PKRankDataManager.GetInstance().pkOldRankID = (int)ret.oldSeasonLevel;
    //         PKRankDataManager.GetInstance().pkStarCount = (int)ret.newSeasonStar;
    //         PKRankDataManager.GetInstance().pkOldStarCount = (int)ret.oldSeasonStar;
    // 
    //         UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkEndData);
    //     }
#endregion

#region 切换角色
    [ProtocolHandle(typeof(RoleSwitchRes))]
    private void _onSwitchRoleRes(RoleSwitchRes rep)
    {
        ClientSystemManager.GetInstance()._QuitToSelectRoleImpl();
    }
#endregion
    [MessageHandle(GateSendLoginPushInfo.MsgID)]
    void OnGateSendLoginPushInfo(MsgDATA msg)
    {
        GateSendLoginPushInfo ret = new GateSendLoginPushInfo();
        ret.decode(msg.bytes);

        if(ret == null)
        {
            AdsPush.LoginPushManager.GetInstance().ClearPushList();
            return;
        }
        
        List<AdsPush.LoginPushManager.LoginPushData> PushList = new List<AdsPush.LoginPushManager.LoginPushData>();
        PushList.Clear();
        for (int i = 0; i < ret.infos.Length; i++)
        {
            AdsPush.LoginPushManager.LoginPushData loginPushData = new AdsPush.LoginPushManager.LoginPushData
            {
                name = ret.infos[i].name,
                iconPath = ret.infos[i].iconPath,
                linkInfo = ret.infos[i].linkInfo,
                loadingIconPath = ret.infos[i].loadingIconPath,
                sortNum = ret.infos[i].sortNum,
                unlockLevel = ret.infos[i].unlockLevel,
                startTime = (int)ret.infos[i].startTime,
                endTime = (int)ret.infos[i].endTime,
                needTime = ret.infos[i].isShowTime == 0 ? false : true,
                IsSetNative = (int)ret.infos[i].isSetNative,
            };
            PushList.Add(loginPushData);
        }
        AdsPush.LoginPushManager.GetInstance().SetLoginPushList(PushList);
    }

    [ProtocolHandle(typeof(RelaySvrObserveRes))]
    private void _onRelaySvrObserveRes(RelaySvrObserveRes res)
    {
        if (res == null) return;
        if (res.result != (uint)ProtoErrorCode.SUCCESS)
        {
            SystemNotifyManager.SystemNotify((int)res.result);
            return;
        }
        ReplayServer.GetInstance().SetLiveShow();
    }
#region 接收角色列表
    [MessageHandle(GateSendRoleInfo.MsgID)]
    void OnGateSendRoleInfo(MsgDATA msg)
    {
        // 这里只刷新切换角色时候的角色列表数据，正常登录流程不在这里接收数据
        if (!ClientSystemLogin.mSwitchRole)
        {
            return;
        }

        GateSendRoleInfo ret = new GateSendRoleInfo();
        ret.decode(msg.bytes);

        foreach (var info in ret.roles)
        {
            Logger.Log("Role info:" + info.ToString() + "\n");
        }

        ClientApplication.playerinfo.apponintmentOccus = ret.appointmentOccus;

        ClientApplication.playerinfo.appointmentRoleNum = ret.appointmentRoleNum;
        ClientApplication.playerinfo.roleinfo = ret.roles;

        ClientApplication.playerinfo.baseRoleFieldNum = ret.baseRoleField;
        ClientApplication.playerinfo.extendRoleFieldNum = ret.extensibleRoleField;
        ClientApplication.playerinfo.unLockedExtendRoleFieldNum = ret.unlockedExtensibleRoleField;

        ServerListManager.instance.SaveUserData(ret.roles);    
    }
#endregion
    
    #region 接收冒险队信息

    [MessageHandle(AdventureTeamInfoSync.MsgID)]
    void OnSyncAdventureTeamInfo(MsgDATA msg)
    {
        if (msg == null)
        {
            return;
        }
        var syncInfo = new AdventureTeamInfoSync();
        syncInfo.decode(msg.bytes);
        ClientApplication.playerinfo.adventureTeamInfo = syncInfo.info;
    }

    #endregion
}
