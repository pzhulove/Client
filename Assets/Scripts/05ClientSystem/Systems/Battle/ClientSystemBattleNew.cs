using ActivityLimitTime;
using GameClient;
using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using Tenmove.Runtime;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 战斗系统
    /// </summary>
    public partial class ClientSystemBattle : ClientSystem
    {
        #region ExtraUIBind
        private GameObject mMonsterBossRoot = null;
        private GameObject mPlayerInfoRoot = null;
        private GameObject mPlayerPKLeftRoot = null;
        private GameObject mPlayerPKRightRoot = null;
        private GameObject m3v3HpRoot = null;

        protected override void _bindExUI()
        {
            mMonsterBossRoot = mBind.GetGameObject("MonsterBossRoot");
            mPlayerInfoRoot = mBind.GetGameObject("PlayerInfoRoot");
            mPlayerPKLeftRoot = mBind.GetGameObject("PlayerPKLeftRoot");
            mPlayerPKRightRoot = mBind.GetGameObject("PlayerPKRightRoot");
            m3v3HpRoot = mBind.GetGameObject("3v3HpRoot");
        }

        protected override void _unbindExUI()
        {
            mMonsterBossRoot = null;
            mPlayerInfoRoot = null;
            mPlayerPKLeftRoot = null;
            mPlayerPKRightRoot = null;
            m3v3HpRoot = null;
        }
        #endregion

        /// <summary>
        /// UI组件管理器
        /// </summary>
        private BattleUIComponentManager _battleUIComponentManager = new BattleUIComponentManager();
        public BattleUIComponentManager BattleUIComponentManager { get { return _battleUIComponentManager; } }

        /// <summary>
        /// 获取怪物血条根节点
        /// </summary>
        public GameObject MonsterBossRoot { get { return mMonsterBossRoot; } }
        public GameObject Pvp3v3LeftHpBarRoot { get { return m3v3HpRoot; } }
        public GameObject Pvp3v3RightHpBarRoot { get { return m3v3HpRoot; } }
        public GameObject PlayerSelfInfoRoot { get { return mPlayerInfoRoot; } }
        public GameObject PlayerOtherInfoRoot { get { return mPlayerInfoRoot; } }
        public GameObject PlayerPKLeftRoot { get { return mPlayerPKLeftRoot; } }
        public GameObject PlayerPKRightRoot { get { return mPlayerPKRightRoot; } }

        //临时使用，用于打开组队面板
        public static bool bNeedOpenTeamFrame = false;
        public BeActionManager simpleActionManager = new BeActionManager();
        public DebugBattleStatisCompnent comDebugBattleStatis = new DebugBattleStatisCompnent();

        // 剧情任务对话框弹出记录 包括三种 进入房间 房间怪物清除 关卡boss被击杀
        // 对话弹出后做下记录，防止反复弹出对话框
        Dictionary<int, List<int>> storyTaskDlgPopUpRecords = new Dictionary<int, List<int>>();
        TaskStatus storyTaskOldState = TaskStatus.TASK_INIT; // 这个用来记录玩家刚进入地下城的时候剧情任务的状态 是否触发剧情对话以这个状态为准

        public override string GetMainUIPrefabName()
        {
            return "UIFlatten/Prefabs/BattleUI/ClientSystemBattleMainNew";
        }

        protected override void _OnMainFrameOpen()
        {
            base._OnMainFrameOpen();
            _battleUIComponentManager.Init(_mainFrame);
        }

        public sealed override void GetEnterCoroutine(AddCoroutine enter)
        {
            base.GetEnterCoroutine(enter);
            OpenBattle();
            enter(_3stepStart);
        }

        private IEnumerator _3stepStart(IASyncOperation op)
        {
#if UNITY_EDITOR
            if (Global.Settings.aiHotReload)
            {
                if(behaviac.Workspace.Instance.IsInited)
                    behaviac.Workspace.Instance.Cleanup();
            }
            BehaviorTreeMechanism.BehaviorTreeLogData.Instance.Clear();
#endif
            
#if !LOGIC_SERVER
            BeActionFrameMgr.Start();
            SkillFileListCache.Start();
#endif
            ThreeStepProcess _3step = new ThreeStepProcess(
                    "BattleStart",
                    ClientSystemManager.instance.enumeratorManager,
                    BattleMain.instance.Start(op));

            _3step.SetErrorProcessHandle(_errorHandle);

            return _3step;
        }

        private IEnumerator _errorHandle(eEnumError type, string msg)
        {
            Logger.LogErrorFormat("[战斗] {0} {1}", type, msg);

            OnSystemError();

            yield break;
        }

        public sealed override void GetExitCoroutine(AddCoroutine exit)
        {
            base.GetExitCoroutine(exit);
        }

        public sealed override void OnStart(SystemContent systemContent)
        {
            if (state == eClientSystemState.onError)
            {
                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
                ClientReconnectManager.instance.Clear();
            }
            else
            {
                var battle = BattleMain.instance.GetBattle() as BaseBattle;

                if (battle != null)
                {
                    battle.PostStart();
                }

                ClientSystemManager.instance.delayCaller.DelayCall(100, () =>
                {
                    bool hasDialog = MissionManager.GetInstance().TryOpenTaskGuideInBattle();
                    if (!hasDialog)
                        ShowTraceAnimation();
                });
                _battleUIComponentManager.Start();
            }
        }

        public override void OnEnter()
        {
            UWAProfilerUtility.Mark("[tm]SysBattle_OnEnter");

            base.OnEnter();

            if (Global.Settings.aiHotReload)
                behaviac.Workspace.Instance.TryInit();

            _battleUIComponentManager.Enter();

            OnEnterOld();
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            base._OnUpdate(timeElapsed);
            _battleUIComponentManager.Update(timeElapsed);
            _OnUpdateSecond(timeElapsed);
        }

        public override void OnExit()
        {
            UWAProfilerUtility.Mark("[tm]SysBattle_OnExit");

            base.OnExit();
            OnExitOld();
            _DeInitData();
        }

        /// <summary>
        /// 函数内部会将时间增量转换成秒
        /// </summary>
        private void _OnUpdateSecond(float timeElapsed)
        {
            //逻辑上面的Update
            int deltaTime = (int)(timeElapsed * GlobalLogic.VALUE_1000);
            if (ReplayServer.GetInstance().IsReplay())
                ReplayServer.GetInstance().Update(deltaTime);
            if (BattleMain.instance != null)
                BattleMain.instance.Update();
        }

        private void _DeInitData()
        {
            SavePveBattleResult();
            simpleActionManager.Deinit();
            _battleUIComponentManager.DeInit();
#if !LOGIC_SERVER
            BeAttachFramesProxy.AttachFrameDataPool.Clear();
#endif
        }

        /// <summary>
        /// 创建战斗
        /// </summary>
        private void OpenBattle()
        {
#if DEBUG_SETTING
            if (Global.Settings.startSystem == EClientSystem.Battle)
            {
                if (Global.Settings.isLocalDungeon)
                {
#if LOCAL_RECORD_FILE_REPLAY
					EquipMasterDataManager.GetInstance().Initialize();

					string path = Global.Settings.scenePath;

					byte[] buff = System.IO.File.ReadAllBytes (path);
                    //Logger.LogForReplay("[RECORD]Start DeSerialiaction:{0} buffLen:{1}", path, buff.Length);

					ReplayFile replayFile = new ReplayFile ();


                    int pos = 0;
                    replayFile.decode(buff,ref pos);

                    string content = ObjectDumper.Dump(replayFile);
                    System.IO.File.WriteAllText(path+"_RelayFile_log.txt", content);

                    //if(replayFile.header.raceType == 255)
                    //{
                    //    dungeonInfo = new SceneDungeonStartRes();
                    //    dungeonInfo.decode(buff);
                    //}
		

                    SceneDungeonStartRes rep = new SceneDungeonStartRes();
					rep.decode(buff, ref pos);

                    content = ObjectDumper.Dump(rep);
                    System.IO.File.WriteAllText(path + "_SceneDungeonStartRes_log.txt", content);

                    BattleDataManager.GetInstance().ClearBatlte();
                    BattleDataManager.GetInstance().ConvertDungeonBattleInfo(rep);


                    _setRacePlayers(rep.players);
					ClientApplication.playerinfo.session = rep.session;

                    Logger.LogErrorFormat("ClientApplication.playerinfo.session = {0},[GetEnterCoroutine]", ClientApplication.playerinfo.session);

					ClientApplication.relayServer.ip = rep.addr.ip;
					ClientApplication.relayServer.port = rep.addr.port;
					ClientApplication.playerinfo.accid = rep.players[0].accid;

                    eDungeonMode mode = eDungeonMode.None;
                    if (rep.session == 0)
                    {
                        mode = eDungeonMode.LocalFrame;
                    }
                    else
                    {
                        mode = eDungeonMode.SyncFrame;
                    }

                    //session = rep.session;

                    BattleType type = ChapterUtility.GetBattleType((int)rep.dungeonId);
                    //BattleMain.OpenBattle(type, mode, (int)rep.dungeonId);
					BattleMain.OpenBattle(type, eDungeonMode.RecordFrame, (int)rep.dungeonId, string.Empty);
					BaseBattle battle = BattleMain.instance.GetBattle () as BaseBattle;
                    //var battle = BattleFactory.CreateBattle(type, mode, (int)rep.dungeonId);
                    //record = battle.recordServer;
                    battle.recordServer.sessionID = rep.session.ToString();
                    battle.recordServer.StartRecord(type, mode, rep.session.ToString(), true, true);

                    battle.SetBattleFlag(rep.battleFlag);

                    battle.SetDungeonClearInfo(rep.clearedDungeonIds);
                    var raidBattle = battle as RaidBattle;
                    if(raidBattle!= null)
                    {
                    for (int i = 0; i < rep.clearedDungeonIds.Length; i++)
                    {
                            raidBattle.DungeonDestroyNotify((int)rep.clearedDungeonIds[i]);
                        }
                    }

                    var guildBattle = battle as GuildPVEBattle;
                    if (guildBattle != null && rep.guildDungeonInfo != null)
                    {
                        guildBattle.SetBossInfo(rep.guildDungeonInfo.bossOddBlood,rep.guildDungeonInfo.bossTotalBlood);
                        guildBattle.SetBuffInfo(rep.guildDungeonInfo.buffVec);
                    }

                      FinalTestBattle finalBattle = battle as FinalTestBattle;
                if (finalBattle != null && rep.zjslDungeonInfo!=null)
                {
                    finalBattle.SetBossInfo(rep.zjslDungeonInfo.boss1ID, rep.zjslDungeonInfo.boss1RemainHp, rep.zjslDungeonInfo.boss2ID, rep.zjslDungeonInfo.boss2RemainHp);
                    finalBattle.SetBuffInfo(rep.zjslDungeonInfo.buffVec);
                }
                    FrameSync.instance.SetRelayFile(replayFile);
                    //battle.StartLogicServer(this);
#else
                    // local model
                    BattleType type = ChapterUtility.GetBattleType(Global.Settings.localDungeonID);
                    BattleMain.OpenBattle(type, eDungeonMode.Test, Global.Settings.localDungeonID, string.Empty);
#endif
                }
                else
                {
                    BattleMain.OpenBattle(BattleType.Single, eDungeonMode.None, 0, string.Empty);
                    FrameSync.instance.StartSingleFrame();
                }
            }
#endif
        }

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

        public void ShowTraceAnimation()
        {
            if (ClientSystemManager.instance.IsFrameOpen<MissionDungenFrame>())
            {
                var frame = ClientSystemManager.instance.GetFrame(typeof(MissionDungenFrame));
                if (frame != null)
                {
                    MissionDungenFrame missionFrame = (frame as MissionDungenFrame);
                    missionFrame.Move(true);

                    ClientSystemManager.instance.delayCaller.DelayCall(3000, () => {
                        missionFrame.Move(false);
                    });
                }
            }
        }

        private void SavePveBattleResult()
        {
#if!LOGIC_SERVER
            if (BattleMain.instance == null)
                return;
            var baseBattle = BattleMain.instance.GetBattle() as BaseBattle;
            if (baseBattle == null)
                return;
            PlayerLocalSetting.SetValue("BattleResult", (int)baseBattle.PveBattleResult);
            PlayerLocalSetting.SaveConfig();
#endif
        }

        /// <summary>
        /// 老的OnEnter函数中的调用 从原来代码中移植过来 避免出现Bug
        /// </summary>
        private void OnEnterOld()
        {
            if (Global.Settings.aiHotReload)
                behaviac.Workspace.Instance.TryInit();

            if (Global.Settings.displayHUD)
                HUDInfoViewer.instance.Init();

            if (null != simpleActionManager)
                simpleActionManager.Init();

            if (state == eClientSystemState.onError)
            {
                //ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
                return;
            }

            if (BattleMain.IsTeamMode(BattleMain.battleType, BattleMain.mode))
            {
                bNeedOpenTeamFrame = true;
            }

            DungeonRebornFrame.isLocal = false;

            storyTaskDlgPopUpRecords = new Dictionary<int, List<int>>();
            storyTaskDlgPopUpRecords.SafeAdd((int)EUIEventID.OnEnterDungeonArea, new List<int>());
            storyTaskDlgPopUpRecords.SafeAdd((int)EUIEventID.OnClearDungeonArea, new List<int>());
            storyTaskDlgPopUpRecords.SafeAdd((int)EUIEventID.OnDungeonBossKilled, new List<int>());

            storyTaskOldState = TaskStatus.TASK_INIT;
            storyTaskOldState = GetStoryTaskState();

            _loadBattleUI();
            _bindDungeonEvent();

            if (Global.Settings.startSystem != EClientSystem.Battle)
            {
                if (BattleMain.instance.battleState == BattleState.End)
                {
                    if (BattleMain.battleType != BattleType.ChijiPVP)
                    {
                        ClientSystemManager.instance.OpenFrame<PKBattleResultFrame>(FrameLayer.Middle);
                    }
                }
            }

            //_bindUIEvent();

            //add by mjx on 170821 for limitTimeGift when first dead
            if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager != null)
                ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.RegisterPlayerDead();

            //重置录像状态
            if (!ReplayServer.GetInstance().IsReplay())
                ReplayServer.GetInstance().SetLastPlaying(false);
            //PVP模式调整UI3DRoot
            if (BattleMain.instance != null)
            {
                if (BattleMain.IsModePvP(BattleMain.battleType))
                {
                    SetUI3DRoot(true);
                }
                else
                {
                    SetUI3DRoot(false);
                }
            }

            _updatePrechangeJobSkillButton();

            EngineConfig.asyncLoadAnimRuntimeSwitch = true;
            if (BattleMain.battleType == BattleType.ScufflePVP)
            {
                GameStatisticManager.instance.dataStatistics.Init();
            }
#if ROBOT_TEST
            if (BattleMain.IsModeMultiplayer(BattleMain.mode))
            {
                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
                if (battleUI != null)
                {
                    battleUI.SetAutoFight(true);
                    battleUI.SetAutoFightIsOn(true);
                }
            }
            AutoFightTestDataManager.instance.RecordBattleData(true);
#endif
        }

        private void _loadBattleUI()
        {
            GameFrameWork.instance.SwithTouchInput();

            if (Global.Settings.showBattleInfoPanel)
            {
                comDebugBattleStatis._loadBattleStatisticsUI();
            }
        }

        protected void _bindDungeonEvent()
        {
            if (BattleMain.instance == null)
            {
                return;


            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleDoubleBossTips, _tipDoubleBoss);

            switch (BattleMain.battleType)
            {
                case BattleType.DeadTown:
                case BattleType.YuanGu:
                case BattleType.Dungeon:
                case BattleType.ChampionMatch:
                case BattleType.GuildPVE:
                case BattleType.FinalTestBattle:
                    if (BattleMain.mode != eDungeonMode.Test)
                    {
                        MissionManager.GetInstance().TriggerDungenBegin();
                        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonRewardFinish, _chapterFinish);
                    }
                    break;
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEnterDungeonArea, _OnEnterDungeonArea);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnClearDungeonArea, _OnClearDungeonArea);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDungeonBossKilled, _OnDungeonBossKilled);
        }
        

        //根据战斗场景类型 设置3DRoot
        protected void SetUI3DRoot(bool isPvp)
        {
#if !LOGIC_SERVER
            if (ClientSystemManager.GetInstance() != null)
            {
                RectTransform ui3drootRect = ClientSystemManager.GetInstance().Layer3DRoot.GetComponent<RectTransform>();
                Vector3 orignalPos = ui3drootRect.localPosition;
                ui3drootRect.localPosition = isPvp ? new Vector3(orignalPos.x, orignalPos.y, -6.5f) : new Vector3(orignalPos.x, orignalPos.y, -1f);
            }
#endif
        }

        private void _updatePrechangeJobSkillButton()
        {
            if (BattleMain.battleType == BattleType.NewbieGuide)
            {
                return;
            }

            bool isShow = PlayerBaseData.GetInstance().Level > 1;

            JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().PreChangeJobTableID);
            if (jobTable == null)
            {
                if (PlayerBaseData.GetInstance().Level < 15)
                {
                    Logger.LogErrorFormat("预先转职职业 {0}", PlayerBaseData.GetInstance().PreChangeJobTableID);
                }
                return;
            }

            if (null == InputManager.instance)
            {
                Logger.LogErrorFormat("预先转职职业 按钮为空空");
                return;
            }

            ETCButton preChangeJob = InputManager.instance.GetETCButton(jobTable.ProJobSkills);
            if (null != preChangeJob)
            {
                preChangeJob.gameObject.CustomActive(isShow);
            }
        }

        private bool mIsInit3v3AllPendingCharactor = false;

        /// <summary>
        /// 老的OnExit函数中的调用 从原来代码中移植过来 避免出现Bug
        /// </summary>
        private void OnExitOld()
        {
#if ROBOT_TEST
            AutoFightTestDataManager.instance.RecordBattleData();
#endif
            SavePveBattleResult();
            simpleActionManager.Deinit();


            storyTaskDlgPopUpRecords = null;
            storyTaskOldState = TaskStatus.TASK_INIT;

            _unloadBattleUI();

            _tryPushFrameBeforeBattleMainClose();

            mIsInit3v3AllPendingCharactor = false;
            
            var system = ClientSystemManager.GetInstance().TargetSystem as ClientSystemBattle;
            SaveSkillDamageData();
            if (BattleMain.instance != null)
            {
                if (system != null)
                {
                    BattleMain.CloseBattle(false);
                }
                else
                {
                    BattleMain.CloseBattle();
                }
            }

            if (ReplayServer.GetInstance() != null)
            {
                ReplayServer.GetInstance().EndReplay(false, "SystemBattleExit");
            }
            ExceptionManager.GetInstance().PrintLogToFile(true);

            ClientSystemManager.GetInstance().Clear3DUIRoot();

            EngineConfig.asyncLoadAnimRuntimeSwitch = false;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEnterDungeonArea, _OnEnterDungeonArea);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnClearDungeonArea, _OnClearDungeonArea);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDungeonBossKilled, _OnDungeonBossKilled);
        }

        private void _unloadBattleUI()
        {
            GameFrameWork.instance.SwithTouchInput(false);
        }

        private void _tryPushFrameBeforeBattleMainClose()
        {
            if (null == BattleMain.instance)
            {
                return;
            }

            //修炼场退出后不打开活动关卡页面	
            if (BattleMain.battleType == BattleType.TrainingPVE)
            {
                if (InstituteFrame.IsEnterFromYanWUYuan)
                {
                    ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(InstituteFrame)));
                }
                return;
            }


            int dungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            dungeonID = ChapterUtility.GetOriginDungeonId(dungeonID);

            var table = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(dungeonID);
            if (null != table)
            {
#if !LOGIC_SERVER
                // 终极试炼关卡结算失败后会弹出一个获得buf的界面
                if (BattleMain.battleType == BattleType.FinalTestBattle)
                {
                    var baseBattle = BattleMain.instance.GetBattle() as BaseBattle;
                    if (baseBattle != null)
                    {
                        if (baseBattle.PveBattleResult == BattleResult.Fail && ActivityDataManager.GetInstance().GetUltimateChallengeInspireBufID() > 0)
                        {
                            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(GetUltimateChallengeBufTipFrame)));
                        }
                    }

                    ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(UltimateChallengeFrame)));
                }
#endif

                if (!TeamDataManager.GetInstance().HasTeam())
                {
                    //深渊和远古，进入到挑战关卡
                    if (table.SubType == DungeonTable.eSubType.S_YUANGU
                        || table.SubType == DungeonTable.eSubType.S_HELL_ENTRY
                        || table.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL
                        || table.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL
                        || table.SubType == DungeonTable.eSubType.S_WEEK_HELL
                        || table.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY
                        || table.SubType == DungeonTable.eSubType.S_WEEK_HELL_PER
                        || table.SubType == DungeonTable.eSubType.S_NANBUXIGU)
                    {
                        ChallengeUtility.OnOpenClientFrameStackCmd(dungeonID, table);
                    }
                    else
                    {
                        if (table.SubType != ProtoTable.DungeonTable.eSubType.S_NORMAL &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_WUDAOHUI &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_NEWBIEGUIDE &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_PK &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_COMBOTRAINING &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_CITYMONSTER &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_DEVILDDOM &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_GUILD_DUNGEON &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_RAID_DUNGEON &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_ANNIVERSARY_HARD &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_ANNIVERSARY_NORMAL &&
                            table.SubType != ProtoTable.DungeonTable.eSubType.S_TREASUREMAP)
                        {
                            ActiveParams data = new ActiveParams
                            {
                                param0 = (ulong)dungeonID
                            };
                            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(ActivityDungeonFrame), data));
                        }

                    }
                }

                if (BattleMain.battleType == BattleType.TrainingSkillCombo || BattleMain.battleType == BattleType.Training)
                {
                    ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(InstituteFrame)));
                }
            }
        }

        //protected void _bindUIEvent()
        //{
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _onLevelChanged);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, _onExpChanged);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKRankChanged, _onPkRankChanged);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonScoreChanged, _onScoreUpdate);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonUnlockDiff, _onDiffHardUpdate);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketOpenSuccess, _OnRedPacketOpenSuccess);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketGet, _OnNewRedPacketGet);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketDelete, _OnDeleteRedPacket);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, _OnGuideStart);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideFinish, _OnGuideFinish);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleFrameSyncEnd, _OnBattleFrameSyncEnd);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3StartRedyFightCount);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3StartRedyFightCount);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3GetRoundEndResult, _onPK3V3GetRoundEndResult);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VanityBonusAnimationEnd, onUpdateVanityBonusIconStatus);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, _OnSystemSwitchFinished);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRollItemEnd, _OnRollItemEnd);
        //    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureMapSizeChange, _OnTreasureMapSizeChanged);
        //}

        //protected void _unBindUIEvent()
        //{
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _onLevelChanged);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, _onExpChanged);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKRankChanged, _onPkRankChanged);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonScoreChanged, _onScoreUpdate);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonUnlockDiff, _onDiffHardUpdate);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketOpenSuccess, _OnRedPacketOpenSuccess);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketGet, _OnNewRedPacketGet);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketDelete, _OnDeleteRedPacket);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, _OnGuideStart);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideFinish, _OnGuideFinish);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleFrameSyncEnd, _OnBattleFrameSyncEnd);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3StartRedyFightCount);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3StartRedyFightCount);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3GetRoundEndResult, _onPK3V3GetRoundEndResult);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VanityBonusAnimationEnd, onUpdateVanityBonusIconStatus);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemSwitchFinished);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRollItemEnd, _OnRollItemEnd);
        //    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureMapSizeChange, _OnTreasureMapSizeChanged);
        //}

        private void _tipDoubleBoss(UIEvent ui)
        {
            int delayInMS = (int)ui.Param1;
            int tipId = (int)ui.Param2;
            CommonTipsDesc tb = TableManager.instance.GetTableItem<CommonTipsDesc>(tipId);

            Logger.LogProcessFormat("[双boss提示] {0}", delayInMS);

            if (null != tb)
            {
                SystemNotifyManager.SysDungeonSkillTip(tb.Descs, delayInMS / 1000.0f);
            }
        }

        // 各种判空检查
        bool CheckBattleMainDataValid()
        {
            if (BattleMain.instance == null)
            {
                return false;
            }

            if (BattleMain.instance.GetDungeonManager() == null)
            {
                return false;
            }

            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager() == null)
            {
                return false;
            }

            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id == null)
            {
                return false;
            }

            return true;
        }

        TaskStatus GetStoryTaskState()
        {
            if (!CheckBattleMainDataValid())
            {
                return TaskStatus.TASK_INIT;
            }          

            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;            

            DungeonUIConfigTable dungeonUIConfigTable = TableManager.GetInstance().GetTableItem<DungeonUIConfigTable>(id);
            if (dungeonUIConfigTable == null)
            {
                return TaskStatus.TASK_INIT;
            }

            var missionInfo = MissionManager.GetInstance().GetMission((uint)dungeonUIConfigTable.TaskID);
            if (missionInfo == null)
            {
                return TaskStatus.TASK_INIT;
            }

            return (TaskStatus)missionInfo.status;
        }

        private void _OnEnterDungeonArea(UIEvent ui)
        {
            if(!CheckBattleMainDataValid())
            {
                return;
            }

            if(TeamDataManager.GetInstance().HasTeam())
            {
                return;
            }           

            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            int areaIdx = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentIndex();           

            if (storyTaskDlgPopUpRecords == null)
            {
                return;
            }

            var indexs = storyTaskDlgPopUpRecords.SafeGetValue((int)EUIEventID.OnEnterDungeonArea);
            if (indexs == null)
            {
                return;
            }

            if (indexs.IndexOf(areaIdx) != -1)
            {
                return;
            }
            indexs.Add(areaIdx);

            DungeonUIConfigTable dungeonUIConfigTable = TableManager.GetInstance().GetTableItem<DungeonUIConfigTable>(id);
            if(dungeonUIConfigTable == null)
            {
                return;
            }           

            if(storyTaskOldState != TaskStatus.TASK_UNFINISH)
            {
                return;
            }

            if(dungeonUIConfigTable.AreaDialog == null)
            {
                return;
            }

            int count = dungeonUIConfigTable.AreaDialog.Length;
            for(int i = 0;i < count;i++)
            {
                string idx2DlgID = dungeonUIConfigTable.AreaDialog[i];
                if(string.IsNullOrEmpty(idx2DlgID))
                {
                    continue;
                }

                var kv = idx2DlgID.Split(':');
                if(kv == null)
                {
                    continue;
                }

                if (kv.Length < 2)
                {
                    continue;                                       
                }

                int idx = Utility.ToInt(kv[0]);
                int dlgID = Utility.ToInt(kv[1]);

                if(idx != areaIdx)
                {
                    continue;                   
                }             

                AddDialog(dlgID); 
                break;
            }
        }

        private void _OnClearDungeonArea(UIEvent ui)
        {
            if (!CheckBattleMainDataValid())
            {
                return;
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                return;
            }            

            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsBossArea())
            {
                return;
            }

            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            int areaIdx = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentIndex();

            if (storyTaskDlgPopUpRecords == null)
            {
                return;
            }

            var indexs = storyTaskDlgPopUpRecords.SafeGetValue((int)EUIEventID.OnClearDungeonArea);
            if (indexs == null)
            {
                return;
            }

            if (indexs.IndexOf(areaIdx) != -1)
            {
                return;
            }     
            indexs.Add(areaIdx);

            DungeonUIConfigTable dungeonUIConfigTable = TableManager.GetInstance().GetTableItem<DungeonUIConfigTable>(id);
            if (dungeonUIConfigTable == null)
            {
                return;
            }

            if (storyTaskOldState != TaskStatus.TASK_UNFINISH)
            {
                return;
            }

            if (dungeonUIConfigTable.AreaAfterDialog == null)
            {
                return;
            }

            int count = dungeonUIConfigTable.AreaAfterDialog.Length;
            for (int i = 0; i < count; i++)
            {
                string idx2DlgID = dungeonUIConfigTable.AreaAfterDialog[i];
                if (string.IsNullOrEmpty(idx2DlgID))
                {
                    continue;
                }

                var kv = idx2DlgID.Split(':');
                if (kv == null)
                {
                    continue;
                }

                if (kv.Length < 2)
                {
                    continue;
                }

                int idx = Utility.ToInt(kv[0]);
                int dlgID = Utility.ToInt(kv[1]);

                if (idx != areaIdx)
                {
                    continue;
                }               

                AddDialog(dlgID);               
                break;
            }
        }

        private void _OnDungeonBossKilled(UIEvent ui)
        {
            if (!CheckBattleMainDataValid())
            {
                return;
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                return;
            }           

            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            int areaIdx = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentIndex();

            if (storyTaskDlgPopUpRecords == null)
            {
                return;
            }

            var indexs = storyTaskDlgPopUpRecords.SafeGetValue((int)EUIEventID.OnDungeonBossKilled);
            if (indexs == null)
            {
                return;
            }

            if (indexs.IndexOf(areaIdx) != -1)
            {
                return;
            }
            indexs.Add(areaIdx);

            DungeonUIConfigTable dungeonUIConfigTable = TableManager.GetInstance().GetTableItem<DungeonUIConfigTable>(id);
            if (dungeonUIConfigTable == null)
            {
                return;
            }

            if (storyTaskOldState != TaskStatus.TASK_UNFINISH)
            {
                return;
            }

            AddDialog(dungeonUIConfigTable.BossKilledDialog);
        }

        private void AddDialog(int id)
        {
            if (!CheckBattleMainDataValid())
            {
                return;
            }

            BattleMain.instance.GetDungeonManager().PauseFight(false);

            var task = new GameClient.TaskDialogFrame.OnDialogOver();
            if (task != null)
            {
                task.AddListener(() =>
                {
                    BattleMain.instance.GetDungeonManager().ResumeFight(false);
                });
            }

            GameClient.MissionManager.GetInstance().CreateDialogFrame(id, 0, task);
        }

        private void _chapterFinish(UIEvent uiEvent)
        {
            if (BattleMain.mode != eDungeonMode.Test)
            {
                Logger.Log("chapter finish callback");
                MissionManager.GetInstance().TriggerDungenEnd();
            }
        }

        //保存技能伤害数据
        protected void SaveSkillDamageData()
        {
#if !LOGIC_SERVER
            if (BattleMain.instance == null
                || BattleMain.instance.GetPlayerManager() == null
                || BattleMain.instance.GetPlayerManager().GetMainPlayer() == null)
                return;
            if (BattleMain.IsModePvP(BattleMain.battleType))
                return;
            BeActor actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
            string dungeonName = BeUtility.GetDungeonName();
            if (actor == null)
                return;
            if (actor.skillDamageManager != null)
                actor.skillDamageManager.SaveSkillDamageData(actor, dungeonName);
#endif
        }
    }
}
