using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

using Network;
using Protocol;
using DG.Tweening;
using ProtoTable;
using Tenmove.Runtime;

/// <summary>
/// Battle类
/// </summary>
namespace GameClient
{
    public enum ServerNotifyMessageId
    {
        None = 0,
        TeamTailPass = 1,       //团本尾部关卡通关
        CthyllaNightmare = 2,   //最终boss克西拉进入噩梦状态

        CthyllaSweetDream = 3,  //最终boss克西拉进入美梦状态
        HARDTeamTailPass = 4,       //HARD团本尾部关卡通关
        HARDCthyllaNightmare = 5,   //HARD最终boss克西拉进入噩梦状态
        HARDCthyllaSweetDream = 6,  //HARD最终boss克西拉进入美梦状态
    }

    public enum BattleFlagType
    {
        EntityRemoveInPassDoor = 1 << 0,  //力法组队过门时 AI释放豪龙破军导致自己被移除
        Eff_Devilddom_hidden_room = 1 << 7,
        PendingArray_Dont_Remove = 1 << 8, //boss死亡时是否删除pengdingarray开关
        BossTranform_Disable_GC = 1 <<11, //boss转换的时候是否关闭GC
		SKILL_LOADING_TYPE = 1<<13,          //人物技能加载优化
        Mechanism2055CancelSkill = 1 << 17, //录像反馈机制2055取消技能切idle开关
        ZhanshaMechanismFlag = 1 << 19,   //斩杀机制使用第二套造成伤害并且会飘字的方案
		Buff7Finish = 1 << 20,   //睡眠Buff结束开关
		SkillSpecialBug = 1 << 25,     //修正后跳取消功能异常buf（这个一直没上线）
        SceneOnReadyStartPetAI = 1<<26, //修正宠物AI被某些战斗异常开启
        GamePursuit = 1 << 29,   //追帧开启

    }

    /// <summary>
    /// 战斗结果类型
    /// </summary>
    public enum BattleResult
    {
        None,   //默认
        Fail,   //通关
        Success,    //失败
    }

#if MG_TEST && LOGIC_SERVER
    public partial class BaseBattle : GameBindSystem, IBattle, IOnExecCommand, IDungeonAudio,IDisposable
#else
    public partial class BaseBattle : GameBindSystem, IBattle, IOnExecCommand, IDungeonAudio
#endif
    {

        protected InputManager mInputManager = null;

        private eDungeonMode mMode = eDungeonMode.None;
        private BattleType mBattleType = BattleType.None;

        protected IDungeonPlayerDataManager mDungeonPlayers = null;
        protected IDungeonManager mDungeonManager = null;
        protected IDungeonStatistics mDungeonStatistics = null;
        protected IDungeonEnumeratorManager mDungeonEnumeratorManager = null;

        protected FrameRandomImp mframeRandom = new FrameRandomImp();
        protected TrailManagerImp mTrialManager;
        protected LogicTrailManager mTrailManager;
        protected BeProjectilePoolImp mProjectilePool;
        protected BeAICommandPoolImp mAICommandPool;		

        public BeBuffPoolImp BuffPool
        {
            get { return mBuffPool; }
        }
        protected BeBuffPoolImp mBuffPool = new BeBuffPoolImp();

        public BeMechanismPoolImp MechanismPool {get {return mMechanismPool;}}
        protected BeMechanismPoolImp mMechanismPool = new BeMechanismPoolImp();
		protected List<int> playerRoleIDList = new List<int>();
        protected int pkRaceType = -1;
        protected LevelManager mLevelMgr = null;

        public bool NeedPreload = true;
        public bool NeedCreateInput = true;
        public bool NeedSendMsg = true;
        public bool NeedShowHitFloat = true;
        public bool NeedPlaySound = true;

        public MessageSender MessageSender = new MessageSender();
        /// <summary>
        /// 用于战斗开关的位掩码
        /// </summary>
        protected uint _battleFlag;

        /// <summary>
        /// 设置战斗开关 位掩码
        /// </summary>
        /// <param name="flag"></param>
        public void SetBattleFlag(uint flag)
        {
            _battleFlag = flag;
        }

        /// <summary>
        /// 判断服务器提供的战斗开关是否打开
        /// 该接口不好理解 请使用下面的新接口FunctionIsOpen
        /// </summary>
        /// <param name="iFlag"></param>
        /// <returns></returns>
        public bool HasFlag(BattleFlagType iFlag)
        {
            uint tmp = (_battleFlag & (uint)iFlag);
            return tmp != 0;
        }

        /// <summary>
        /// 服务器开关是否打开 默认是关闭状态 返回False
        /// 该开关是用于做保护 如果当前使用的代码 出现问题 可以立即开启开关
        /// </summary>
        public bool FunctionIsOpen(BattleFlagType iFlag)
        {
            uint tmp = (_battleFlag & (uint)iFlag);
            return tmp != 0;
        }

        public int PkRaceType { get { return pkRaceType; } set { pkRaceType = value; } }

#if !LOGIC_SERVER
        /// <summary>
        /// 用于存储战斗结果
        /// </summary>
        private BattleResult _PveBattleResult;
        public BattleResult PveBattleResult
        {
            get { return _PveBattleResult; }
            set { _PveBattleResult = value; }
        }
#endif

        

        public TrailManagerImp TrailManager
        {
            get
            {
                return mTrialManager;
            }
        }
        public LogicTrailManager LogicTrailManager
        {
            get
            {
                return mTrailManager;
            }
        }
        public BeProjectilePoolImp BeProjectilePool
        {
            get
            {
                return mProjectilePool;
            }
        }

        public BeAICommandPoolImp BeAICommandPool
        {
            get
            {
                return mAICommandPool;
            }
        }
        public FrameRandomImp FrameRandom
        {
            get { return mframeRandom; }
        }

        public LevelManager LevelMgr
        {
            get { return mLevelMgr; }
            set { mLevelMgr = value; }
        }

#if LOGIC_SERVER
        protected RecordServer mRecordServer = new RecordServer();
#else
        protected RecordServer mRecordServer = RecordServer.GetInstance();
#endif


        private LogicServer mLogicServer = null;

        public LogicServer logicServer
        {
            get
            {
                return mLogicServer;
            }
        }
#if MG_TEST  && LOGIC_SERVER
        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);

        }
        ~BaseBattle()
        {
            Dispose(false);
        }
        private bool m_disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    // Release managed resources            
                }

                // Release unmanaged resources            

                m_disposed = true;
            }
            LogicApplication.OnLogicServerDisposed(mRecordServer != null ? Convert.ToUInt64(mRecordServer.sessionID) : 0);
            Logger.LogErrorFormat("[Dispose BaseBattle] session {0} dippose {1} stack {2} ", mRecordServer != null ? Convert.ToUInt64(mRecordServer.sessionID) : 0, disposing, RecordServer.GetStackTraceModelName());
        }
#endif

        public BaseBattle(BattleType type, eDungeonMode mode, int dungeonID)
        {
            mBattleType = type;
            mMode = mode;
#if !LOGIC_SERVER
            _PveBattleResult = BattleResult.None;
#endif
            var dungeon = new BeDungeon(type, mode, dungeonID)
            {
                mBattle = this
            };
            mDungeonStatistics         = dungeon;
            mDungeonManager            = dungeon;
            mDungeonEnumeratorManager  = dungeon;
            mDungeonPlayers            = dungeon.GetDungeonPlayerDataManager();

            mTrialManager              = new TrailManagerImp();
            mTrailManager              = new LogicTrailManager();
            mProjectilePool            = new BeProjectilePoolImp();
            mProjectilePool.Init();
            mAICommandPool             = new BeAICommandPoolImp();
            mAICommandPool.Init();
            mframeRandom.mRecordServer = mRecordServer;

#if LOGIC_SERVER
            mRecordServer.Init();
#endif

            mRecordServer.battleType   = type;
            mRecordServer.FrameRandom  = FrameRandom;
        }

        public IDungeonPlayerDataManager dungeonPlayerManager
        {
            get
            {
                return mDungeonPlayers;
            }
        }

        public IDungeonManager dungeonManager
        {
            get
            {
                return mDungeonManager;
            }
        }

        public IDungeonStatistics dungeonStatistics
        {
            get
            {
                return mDungeonStatistics;
            }
        }

        public RecordServer recordServer
        {
            get
            {
                return mRecordServer;
            }
        }

#region IBattle
        public BattleType GetBattleType()
        {
            return mBattleType;
        }

        public eDungeonMode GetMode()
        {
            return mMode;
        }

        public void End(bool needEndRecord = true)
        {
            //Log2File.Flush();

#if !SERVER_LOGIC
            _doStatFinishBattle();
#endif

#if SERVER_LOGIC && MG_TEST
            LogicApplication.TrySaveRecordData(logicServer);
#endif
            if (mLevelMgr != null)
            {
                mLevelMgr.DeInit();
                mLevelMgr = null;
            }
            if (needEndRecord)
                recordServer.EndRecord("BaseBattleEnd");
            _unbindEvents();
            _unloadInputManger();
            BeUtility.ResetCamera();
            
            ClearBgm();

            _onEnd();
            mDungeonManager.DestoryBeScene();
            mDungeonManager = null;
            mDungeonPlayers = null;
            mDungeonStatistics = null;
            mDungeonEnumeratorManager = null;
            mLogicServer = null;

            if (mTrailManager != null)
            {
                mTrailManager.ClearAll();
                mTrailManager = null;
            }

            if (MessageSender != null)
            {
                MessageSender.Clear();
                MessageSender = null;
            }
#if !LOGIC_SERVER
            GBCommandPoolSystem.GetInstance().Clear();
            GBControllerAllocator.GetInstance().Clear();
#endif

            //recordServer.EndRecord();
        }

        private void _doStatFinishBattle()
        {
            if (!NeedSendMsg)
                return;

#if !SERVER_LOGIC
            try 
            {
                GameStatisticManager.instance.DoStatFinishBattle(mDungeonManager.GetDungeonDataManager().id.dungeonID);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[战斗数据] 结束战斗统计 {0}", e.ToString());
            }
#endif
        }



        public IEnumerator Start(IASyncOperation op)
        {
            ITMStopWatch stopWatch01 = TMBattleAssetLoadRecord.instance.CreateStopWatch("BaseBattleStart");
            TMBattleAssetLoadRecord.instance.AssetLoadFlag = BattleAssetLoadFlag.PreLoad;

            //!!!!!!!
            GeEffectPool.GetInstance().ClearAll();


            if (eDungeonMode.SyncFrame == mMode)
            {
                /*				if (RecordServer.GetInstance().NeedRecord())
                                {
                                    RecordServer.GetInstance().StartRecord(RecordMode.TEAM, ClientApplication.playerinfo.session.ToString(), true);
                                }*/
            }

            FrameSync.instance.isFire = true;
            FrameSync.instance.SetDungeonMode(mMode);
            if (eDungeonMode.SyncFrame == mMode)
            {
                /// Connect to the RelayServer

                if (!ReplayServer.GetInstance().IsReplay())
                {
                    WaitServerConnected waitConnect = new WaitServerConnected(ServerType.RELAY_SERVER,
                            ClientApplication.relayServer.ip,
                            ClientApplication.relayServer.port,
                            ClientApplication.playerinfo.accid
                            );

                    yield return waitConnect;

                    if (waitConnect.GetResult() == WaitServerConnected.eResult.Success)
                    {
                        GameClient.ClientReconnectManager.instance.canReconnectRelay = true;

                        /*                        if (RecordServer.GetInstance().NeedRecord())
                                                {
                                                    RecordServer.GetInstance().StartRecord(RecordMode.TEAM, ClientApplication.playerinfo.session.ToString(), true);
                                                }*/
                    }
                    else
                    {
                        yield return new NormalCustomEnumError("[战斗] 连接服务器失败", eEnumError.ProcessError);
                        yield break;
                    }
                }

                TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart 连接服务器完成", stopWatch01);
                //所有加载同步随机数种子
                FrameRandom.ResetSeed((uint)ClientApplication.playerinfo.session);
                OnAfterSeedInited();
                FrameSync.instance.Init();
                FrameSync.instance.SetStartTick();
                UWAProfilerUtility.Mark("[tm]Battle_FrameInit_Sync");

                if (!ReplayServer.GetInstance().IsReplay())
                {
                    var msgEvents = new MessageEvents();
                    var req = new RelaySvrLoginReq();
                    var res = new RelaySvrNotifyGameStart();
                    var mainPlayer = mDungeonPlayers.GetMainPlayer();

                    req.accid = mainPlayer.playerInfo.accid;
                    req.roleid = mainPlayer.playerInfo.roleId;
                    req.seat = mainPlayer.playerInfo.seat;
                    req.session = ClientApplication.playerinfo.session;

                    Logger.LogProcessFormat("[战斗] RelaySvrLoginReq accid {0} roleid {1} seatid {2} sessionid {3} \n", req.accid, req.roleid, req.seat, req.session);

                    Network.NetManager.instance.SendCommand(ServerType.RELAY_SERVER, req);
                    if (op != null) op.SetProgress(0.05f);
                    if (op != null) op.SetProgressInfo("login");
                }
                else
                {
                    // ReplayServer.GetInstance().Start();
                }
            }
            else
            {
                FrameRandom.ResetSeed((uint)ClientApplication.playerinfo.session);
                OnAfterSeedInited();
                FrameSync.instance.Init();
                FrameSync.instance.SetStartTick();
                UWAProfilerUtility.Mark("[tm]Battle_FrameInit_Local");
            }


            if (op != null) op.SetProgress(0.1f);
            mDungeonPlayers.SendMainPlayerLoadRate(4);

            yield return Yielders.EndOfFrame;

            mDungeonManager.GetDungeonDataManager().FirstArea();

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart _onStart 之前", stopWatch01);
            _onStart();
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart _onStart 完成", stopWatch01);

            if (op != null) op.SetProgress(0.15f);
            if (op != null) op.SetProgressInfo("_onStart");

            _createBase();
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart _createBase 完成", stopWatch01);

            if (op != null) op.SetProgress(0.2f);
            if (op != null) op.SetProgressInfo("_createBase");

            mDungeonPlayers.SendMainPlayerLoadRate(20);
            yield return Yielders.EndOfFrame;

            _createEntitys();
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart _createEntitys 完成", stopWatch01);

            while (!_isBattleLoadFinish())
            {
                yield return Yielders.EndOfFrame;
            }

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart BattleLoadFinish 完成", stopWatch01);

            if (op != null) op.SetProgress(0.5f);
            if (op != null) op.SetProgressInfo("_createEntitys");
            mDungeonPlayers.SendMainPlayerLoadRate(50);

            yield return Yielders.EndOfFrame;

            if (NeedCreateInput)
                _createInput();
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart _createInput 完成", stopWatch01);

            _onStartResourceLoaded();
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart _onStartResourceLoaded 完成", stopWatch01);

            if (op != null) op.SetProgress(0.6f);
            if (op != null) op.SetProgressInfo("_onStartResourceLoaded");

            yield return Yielders.EndOfFrame;

#if !LOGIC_SERVER
            if (NeedPreload)
                PreparePreloadRes();
#endif
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart PreparePreloadRes 完成", stopWatch01);

            if (op != null) op.SetProgress(0.7f);
            if (op != null) op.SetProgressInfo("PreparePreloadRes");

            mDungeonPlayers.SendMainPlayerLoadRate(55);
            yield return Yielders.EndOfFrame;


            _onSceneStart();
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart _onSceneStart 完成", stopWatch01);

            if (op != null) op.SetProgress(0.8f);
            if (op != null) op.SetProgressInfo("_onSceneStart");
            mDungeonPlayers.SendMainPlayerLoadRate(60);
            yield return Yielders.EndOfFrame;

            if (NeedPreload)
                yield return ClientSystemManager._PreloadRes(op);
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart _PreloadRes 完成", stopWatch01);

            TMBattleAssetLoadRecord.instance.AssetLoadFlag = BattleAssetLoadFlag.Fighting;

            mDungeonPlayers.SendMainPlayerLoadRate(90);



            if (op != null) op.SetProgress(0.95f);
            if (op != null) op.SetProgressInfo("RelaySvrNotifyGameStart2");
            mDungeonPlayers.SendMainPlayerLoadRate(100);

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart 所有资源加载 完成", stopWatch01);

            _bindEvents();
            if (ReplayServer.GetInstance().IsLiveShow())
            {
                ReplayServer.GetInstance().ReadyToLiveShow();
            }
            if (!ReplayServer.GetInstance().IsReplay() || ReplayServer.GetInstance().IsLiveShow())
            {
                float timeOutCount = 0.0f;
                while (!FrameSync.instance.isGetStartFrame)
                {
                    if (timeOutCount >= 130.0f)
                    {
                        // 超时
                        break;
                    }
                    else
                    {
                        yield return Yielders.EndOfFrame;
                        timeOutCount += Time.unscaledDeltaTime;
                    }
                }

                if (!FrameSync.instance.isGetStartFrame)
                {
                    yield return new NormalCustomEnumError("[战斗] 进入游戏消息超时", eEnumError.ProcessError);
                    yield break;
                }
            }

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart 收到战斗开始帧 完成", stopWatch01);

            if (ReplayServer.GetInstance().IsReplay())
            {
                ClientSystemManager.GetInstance().delayCaller.DelayCall(1200, () =>
                {
                    ReplayServer.GetInstance().Start();
                });
            }

            mDungeonManager.StartFight();

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("BaseBattleStart mDungeonManager.StartFight 完成", stopWatch01);

            if (op != null) op.SetProgress(1.0f);
            if (op != null) op.SetProgressInfo("Loading...");

            if (!ReplayServer.GetInstance().IsReplay())
            {
                if (NeedSendMsg) 
                    mDungeonManager.GetDungeonDataManager().SendDungeonEnterRace();
            }

            PushBgm(mDungeonManager.GetDungeonDataManager().table.BGMPath, mDungeonManager.GetDungeonDataManager().table.EnvironmentBGMPath);
            
            TMBattleAssetLoadRecord.instance.CloseAndSaveWatch(stopWatch01);
#if STAT_EXTRA_INFO
#if !LOGIC_SERVER
	            var battleCreateDuration = Time.realtimeSinceStartup - startTime;
	            //Logger.LogErrorFormat("battleCreateDuration:{0}", battleCreateDuration);
                if (NeedSendMsg)
	                GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.DUNGEON_LOADING, battleCreateDuration.ToString());
#endif
#endif
        }

        public BeScene Restart()
        {
            _onRestart();

            return mDungeonManager.GetBeScene();
        }
#endregion
        public virtual void OnCriticalElementDisappear()
        {

        }
        public virtual void OnAfterSeedInited()
        {

        }
        public virtual bool CanReborn()
        {
            return true;
        }
        public virtual bool IsRebornCountLimit()  //是否副本有总复活数量限制
        {
            return false;
        }
        public virtual int GetLeftRebornCount()  //剩余总复活数量
        {
            return 0;
        }
        public virtual int GetMaxRebornCount()
        {
            return 0;
        }
        protected virtual void PreparePreloadRes()
        {

            UWAProfilerUtility.Mark("[tm]Battle_PreparePreloadRes");

#if DEBUG_REPORT_ROOT
			if (DebugSettings.instance.DisablePreload)
				return;
#endif

            var players = mDungeonPlayers.GetAllPlayers();

            //低画质个人不做预加载，组队要预加载
#if TEST_SILLFILE_LOAD        
			if (GeGraphicSetting.instance.IsLowLevel() && players.Count <= 1)
				return;
#endif
            

            PreloadManager.ClearCache();
            PreloadManager.battleType = mBattleType;
            

#if !LOGIC_SERVER
            var dungeonDataManager = mDungeonManager.GetDungeonDataManager();
            if (null != dungeonDataManager)
            {
                CResPreloader.instance.AddRes(dungeonDataManager.PreloadPath(), false, 1, dungeonDataManager.Preload);
            }
#endif
            
            playerRoleIDList.Clear();
            for (int i = 0; i < players.Count; ++i)
            {
                var actor = players[i].playerActor;
                if (actor != null)
                {
                    bool useCube = false;


#if !LOGIC_SERVER
                    //画质设置：低配不预加载
                    if (GeGraphicSetting.instance.IsLowLevel() && !BattleMain.IsModePvP(mBattleType))
					{
						if (!players[i].playerActor.isLocalActor)
							useCube = true;
					}
                    //技能资源文件预加载
                    //if (((BeClientSwitch.FunctionIsOpen(ClientSwitchType.SkillPre)
                    //    && !BattleMain.IsModePvP(mBattleType)) || mBattleType == BattleType.ChijiPVP)
                    //    && !GeGraphicSetting.instance.IsLowLevel()
                    //    && !playerRoleIDList.Contains(players[i].playerActor.professionID))
                    //if(!playerRoleIDList.Contains(players[i].playerActor.professionID))
                    //{
                    //    playerRoleIDList.Add(players[i].playerActor.professionID);
                    //    PreloadJobRes(players[i]);
                    //}
#endif
#if LOGIC_SERVER
                    if(mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                    {
                        GameClient.PreloadManager.PreloadActor(actor, mDungeonManager.GetBeScene().ActionFrameMgr, mDungeonManager.GetBeScene().SkillFileCache, useCube);
                    }
                    else
                    {                    
                        GameClient.PreloadManager.PreloadActor(actor, null, null, useCube);
                    }
#else

                    PreloadManager.PreloadActor(actor,null,null, useCube);
                    //预加载宠物
                    if (actor.pet != null)
                        PreloadManager.PreloadActor(actor.pet, null, null, useCube);
#endif
                }
            }
            
            PreloadEnemies();


            if (Global.PRELOAD_MODE == PreloadMode.ALL)
            {
                PreloadAllMonsters();
            }
            
            _PreloadHell();
        }

        protected void PreloadAllMonsters()
        {
#if !LOGIC_SERVER
            if (this.GetBattleType() == BattleType.TreasureMap) return;
			var mBeScene = mDungeonManager.GetBeScene();
			var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var areaList = mDungeonData.battleInfo.areas;
			for(int i=0; i<areaList.Count; ++i)
			{
				var cnt = areaList[i].MonsterCount;
				for(int j=0;j<cnt; ++j)
				{
                    var unit = areaList[i].GetMonsterAt(j);

					PreloadManager.PreloadMonsterID(unit.typeId,null,null);
					var summonMonsterList = unit.summonerMonsters;
					if (summonMonsterList != null)
					{
						for(int k=0; k<summonMonsterList.Count; ++k)
						{
							PreloadManager.PreloadMonsterID(summonMonsterList[k].typeId,null,null);
						}
					}
				}
			}
#endif
        }

        protected void PreloadEnemies()
        {
#if !LOGIC_SERVER
            var beScene = mDungeonManager.GetBeScene();
            if (beScene == null)
                return;

            for (int k = 0; k < 2; ++k)
            {
                List<BeEntity> entities = null;
                if (k == 0)
                    entities = beScene.GetEntities();
                else if (k == 1)
                    entities = beScene.GetPendingEntities();
                for (int i = 0; i < entities.Count; ++i)
                {
                    var actor = entities[i] as BeActor;
                    if (actor != null && actor.IsMonster())
                    {
                        GameClient.PreloadManager.PreloadActor(actor,null,null);
                    }
					if ((entities[i] as BeObject) != null)
					{
						if (entities[i].m_iResID == 60111)
						{
							GameClient.PreloadManager.PreloadResID(entities[i].m_iResID,null,null);
						}

					}
                }
            }
#endif
        }

        protected void PreloadMonster()
        {

#if !LOGIC_SERVER

#if DEBUG_REPORT_ROOT
			if (DebugSettings.instance.DisablePreload)
				return;
#endif

            CResPreloader.instance.Clear();
			PreloadManager.ClearCache();
            PreloadEnemies();
            CResPreloader.instance.DoPreLoadAsync(false, true);
#endif
        }

        protected void PreloadHell()
        {
#if !LOGIC_SERVER
			CResPreloader.instance.Clear();
			PreloadManager.ClearCache();
			_PreloadHell();
			CResPreloader.instance.DoPreLoadAsync(false, true);
#endif
        }

        protected void _PreloadHell()
        {
#if !LOGIC_SERVER
			CResPreloader.instance.SetTag(CResPreloader.PreloadTag.HELL);

			var mBeScene = mDungeonManager.GetBeScene();
			var mDungeonData = mDungeonManager.GetDungeonDataManager();

			var info = mDungeonData.battleInfo.dungeonHealInfo;

			//var info = mDungeonData.CurrentHellDestructs();
			if (null != info)
			{
				for (int i = 0; i < info.waveInfos.Count; ++i)
				{
					for (int j = 0; j < info.waveInfos[i].monsters.Count; ++j)
					{
						//Logger.LogErrorFormat("preload hell:{0}", info.waveInfos[i].monsters[j].typeId);
						GameClient.PreloadManager.PreloadMonsterID(info.waveInfos[i].monsters[j].typeId,null,null/*, true*/);
					}
				}
			}

			CResPreloader.instance.SetTag(CResPreloader.PreloadTag.NONE);
#endif
        }
#region 私有

        private void _bindEvents()
        {
#if !LOGIC_SERVER


            FrameSync.instance.SetMainLogic(mDungeonManager);

            InitBindSystem(null);

#endif


            BeScene beScene = mDungeonManager.GetBeScene();
            if (null != beScene)
            {
                beScene.RegisterEventNew(BeEventSceneType.onClear, _onAreaClearEvent);
                beScene.RegisterEventNew(BeEventSceneType.onDoorStateChange, _onDoorStateChange);
                beScene.RegisterEventNew(BeEventSceneType.onMonsterRemoved, _onMonsterRemoved);
                beScene.RegisterEventNew(BeEventSceneType.onMonsterDead, _onMonsterDead);
                beScene.RegisterEventNew(BeEventSceneType.onEggDead, _onEggDead);

            }
        }

        private void _unbindEvents()
        {
#if !LOGIC_SERVER

            if (mMode == eDungeonMode.SyncFrame)
            {
                FrameSync.instance.UnInit();
            }

            GameClient.ClientReconnectManager.instance.canReconnectRelay = false;

            FrameSync.instance.ClearMainLogic();

            ExistBindSystem();

#endif


            BeScene beScene = mDungeonManager.GetBeScene();
            if (null != beScene)
            {
                beScene.UnRegisterEventNew(BeEventSceneType.onClear, _onAreaClearEvent);
                beScene.UnRegisterEventNew(BeEventSceneType.onDoorStateChange, _onDoorStateChange);
                beScene.UnRegisterEventNew(BeEventSceneType.onMonsterRemoved, _onMonsterRemoved);
                beScene.UnRegisterEventNew(BeEventSceneType.onMonsterDead, _onMonsterDead);
                beScene.UnRegisterEventNew(BeEventSceneType.onEggDead, _onEggDead);
            }
        }

#region Input
        protected void _createInput()
        {
#if !LOGIC_SERVER && UNITY_EDITOR
            if (ReplayServer.GetInstance().IsReplay() && BattleMain.IsModePvP(GetBattleType()))
                return;
#else
            if (ReplayServer.GetInstance().IsReplay())
                return;
#endif

            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            if (null != mainPlayer && null != mainPlayer.playerActor)
            {
                _loadInputManager(mainPlayer.playerActor);
            }
            else
            {

            }
        }

        protected void _reLoadSkillButton()
        {

#if !LOGIC_SERVER
            if (ReplayServer.GetInstance().IsReplay())
                return;
            if (mDungeonPlayers == null) return;
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            if (null != mainPlayer)
            {
                //_loadInputManager(mainPlayer.playerActor);
				mInputManager.LoadJoystick(SettingManager.GetInstance().GetJoystickMode());
                mInputManager.LoadSkillButton(mainPlayer.playerActor);
                mInputManager.LoadInputSettingBattleProgram();
                mInputManager.ResetSkillJoystick();
            }
#endif
        }


        protected void _unloadInputManger()
        {

#if !LOGIC_SERVER
            if (mInputManager != null)
            {
                mInputManager.Unload();
                mInputManager = null;
            }
#endif

        }

        protected void _hiddenInputManagerJump()
        {
            if (null != mInputManager)
            {
                mInputManager.HiddenJump();
            }
        }

        protected void _hiddenAllInput()
        {
            if (null != mInputManager)
            {
                mInputManager.SetVisible(false);
            }
        }

        protected void _showAllInput()
        {
            if (null != mInputManager)
            {
                mInputManager.SetVisible(true);
            }
        }

        private void _loadInputManager(BeActor actor)
        {

#if !LOGIC_SERVER
            _unloadInputManger();

            if (mInputManager == null)
            {
                mInputManager = new InputManager();
            }

            if (mInputManager != null)
            {
				mInputManager.LoadJoystick(SettingManager.GetInstance().GetJoystickMode());
                mInputManager.LoadSkillButton(actor);
                mInputManager.LoadInputSettingBattleProgram();
                mInputManager.InitState();

                InputManager.instance = mInputManager;
            }
#else
            mInputManager = null;
#endif

        }
#endregion

#endregion

        protected void _createBase()
        {
            UWAProfilerUtility.Mark("[tm]Battle_createBase");
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]_createBase {0}", GetBattleType());
                recordServer.MarkString(0x8779808, GetBattleType().ToString());
                // Mark:0x8779808 [BATTLE]_createBase {0}
            }
            mDungeonManager.CreateBeScene();
            var beScene = mDungeonManager.GetBeScene();
            if (null != beScene)
            {
                beScene.CreateDecorator();
                beScene.ClearBossDeadBody();
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonAreaChanged);
            _onCreateScene(null);
        }

        protected virtual void _createEntitys()
        {
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]_createEntitys");
                recordServer.Mark(0x888888);
                // Mark:0x888888 [BATTLE]_createEntitys
            }

            UWAProfilerUtility.Mark("[tm]Battle_createDestruct");
            _createHealDestruct();

            UWAProfilerUtility.Mark("[tm]Battle_createDoor");
            _createDoors();

            UWAProfilerUtility.Mark("[tm]Battle_createPlayer");
            _createPlayers();

            UWAProfilerUtility.Mark("[tm]Battle_createRegions");
            _createRegions();

            UWAProfilerUtility.Mark("[tm]Battle_createMonster");
            _createMonsters();

            UWAProfilerUtility.Mark("[tm]Battle_createDestructs");
            _createDestructs();
        }

        protected virtual bool _isBattleLoadFinish()
        {
            return true;
        }

        protected bool _isNeedSendNet()
        {
#if !LOGIC_SERVER

            switch (mMode)
            {
                case eDungeonMode.LocalFrame:
                case eDungeonMode.SyncFrame:
                    return true;
            }


#endif

            return false;
        }

        public void PostStart()
        {
            _onPostStart();

#if !LOGIC_SERVER
            if (GlobalEventSystem.GetInstance() != null)
            {
                GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleInitFinished);
            }

            if (NeedSendMsg && GameStatisticManager.instance != null && mDungeonManager.GetDungeonDataManager() != null && mDungeonManager.GetDungeonDataManager().id != null)
            {
                GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.ENTER,
                                mDungeonManager.GetDungeonDataManager().id.dungeonID,
                                mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                                "");
            }
#if UNITY_EDITOR
            if (dungeonManager != null && dungeonManager.GetGeScene() != null && dungeonManager.GetBeScene() != null)
            {
                dungeonManager.GetGeScene().SetDoorData(dungeonManager.GetBeScene().GetDoorList());
            }
#endif

#endif
        }

        private void _onAreaClearEvent(BeEvent.BeEventParam args)
        {
            UWAProfilerUtility.Mark("[tm]Battle_AreaClear");

            Logger.LogProcessFormat("OnAreaClear\n");
            _onAreaClear(args);


#if !LOGIC_SERVER
            if (NeedSendMsg)
            {
                GameStatisticManager.instance.DoStatInBattleEx(
                    StatInBattleType.CLEAR_ROOM,
                    mDungeonManager.GetDungeonDataManager().id.dungeonID,
                    mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                    null);
            }
            
#endif


            if (!mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                _playDungeonClear();
            }
        }

#region PlayVoice
        private int _getMainPlayerResourceID()
        {
            BattlePlayer player = mDungeonPlayers.GetMainPlayer();
            if (null != player)
            {
                return (int)player.playerInfo.occupation;
            }

            return -1;
        }

        protected void _playDungeonClear()
        {
#if !SERVER_LOGIC

            if (mBattleType != BattleType.MutiPlayer &&
                mBattleType != BattleType.GuildPVP &&
                mBattleType != BattleType.PVP3V3Battle &&
                mBattleType != BattleType.MoneyRewardsPVP &&
                mBattleType != BattleType.ScufflePVP &&
                mBattleType != BattleType.ChijiPVP)
            {
                VoiceManager.instance.PlayVoiceByOccupation(ProtoTable.VoiceTable.eVoiceType.DUNGEONCLEARROOM, _getMainPlayerResourceID());
            }

#endif

        }

        protected void _playDungeonDead()
        {
            //VoiceManager.instance.PlayVoiceByOccupation(ProtoTable.VoiceTable.eVoiceType.DUNGEONDEAD, _getMainPlayerResourceID());
        }

        protected void _playDungeonFinish()
        {
#if !SERVER_LOGIC

            VoiceManager.instance.PlayVoiceByOccupation(ProtoTable.VoiceTable.eVoiceType.DUNGEONFINISH, _getMainPlayerResourceID());

#endif

        }

        protected void _playDungeonUsePowerSkill()
        {
#if !SERVER_LOGIC

            VoiceManager.instance.PlayVoiceByOccupation(ProtoTable.VoiceTable.eVoiceType.DUNGEONPOWERSKILL, _getMainPlayerResourceID());

#endif

        }

        protected void _playDungeonSkillPower()
        {
#if !SERVER_LOGIC

           VoiceManager.instance.PlayVoiceByOccupation(ProtoTable.VoiceTable.eVoiceType.DUNGEONKILLPOWER, _getMainPlayerResourceID());

#endif

        }
#endregion

#region 虚函数们儿


        protected virtual void _onAreaClear(BeEvent.BeEventParam args) { }

        protected virtual void _onCreateScene(BeEvent.BeEventParam args) { }

        protected virtual void _onDoorStateChange(BeEvent.BeEventParam args) { }

        protected virtual void _onMonsterRemoved(BeEvent.BeEventParam args) { }

        protected virtual void _onMonsterDead(BeEvent.BeEventParam args) { }

        protected virtual void _onEggDead(BeEvent.BeEventParam args) { }

        protected virtual void _onStart() { }

        protected virtual void _onStartResourceLoaded() { }

        protected virtual void _onPostStart() { }

        protected virtual void _onEnd() { }

        protected virtual void _onRestart() { }

        protected virtual void _createArea() { }

        protected virtual void _createRegions() { }

        protected virtual void _createDecorators() { }

        protected virtual void _createHealDestruct() { }

        protected virtual void _createMonsters() { }

        protected virtual void _createDestructs() { }

        protected virtual void _createDoors() { }

        protected virtual void _createPlayers() { }

        protected virtual void _onSceneStart() { }

        protected virtual void _onUpdate(int delta) { }

#endregion

#region IDungeonAudio implementation
        private class BgmNode
        {
            public string path = string.Empty;
            public uint handle = uint.MaxValue;

            public string envPath = string.Empty;
            public uint envHandle = uint.MaxValue;
        }

        private Stack<BgmNode> mBgmStack = new Stack<BgmNode>();

        private bool _playBgm(BgmNode node)
        {
#if !LOGIC_SERVER
            if (null != node)
            {
                node.handle  = AudioManager.instance.PlaySound(node.path, AudioType.AudioStream, Global.Settings.bgmBattle, true);
                node.envHandle = AudioManager.instance.PlaySound(node.envPath, AudioType.AudioEnvironment, Global.Settings.bgmBattle, true);

                Logger.LogProcessFormat("[DungeonAudio] 播放 {0}, {1} env:{2}", node.path, node.handle, node.envPath);
                return true;
            }
#endif

            return false;
        }

        private bool _stopBgm(BgmNode node)
        {
#if !LOGIC_SERVER

            if (null != node)
            {
                if (node.handle != uint.MaxValue)
                {
                    AudioManager.instance.Stop(node.handle);
                    node.handle = uint.MaxValue;

                    if (node.envHandle != uint.MaxValue)
                    {
                        AudioManager.instance.Stop(node.envHandle);
                        node.envHandle = uint.MaxValue;
                    }

                    Logger.LogProcessFormat("[DungeonAudio] 停止 {0}, {1}", node.path, node.handle);

                    return true;
                }
            }
#endif
            return false;
        }

        public bool PushBgm(string path, string envPath = null)
        {
            if (!NeedPlaySound)
                return false;

            if (!string.IsNullOrEmpty(path))
            {
                BgmNode node = new BgmNode()
                {
                    path = path,
                    envPath = envPath
                };

                if (_playBgm(node))
                {
                    if (mBgmStack.Count > 0)
                    {
                        _stopBgm(mBgmStack.Peek());
                    }

                    mBgmStack.Push(node);

                    return true;
                }
            }

            return false;
        }

        public void PopBgm()
        {
            if (!NeedPlaySound)
                return;

            if (mBgmStack.Count > 0)
            {
                _stopBgm(mBgmStack.Pop());
            }

            if (mBgmStack.Count > 0)
            {
                _playBgm(mBgmStack.Peek());
            }
        }

        public void ClearBgm()
        {
            if (!NeedPlaySound)
                return;
                
            while (mBgmStack.Count > 0)
            {
                _stopBgm(mBgmStack.Pop());
            }
        }

        public uint PlaySound(int tableID)
        {
            if (!NeedPlaySound)
                return uint.MaxValue;

            if (tableID <= 0)
                return uint.MaxValue;
            
            return AudioManager.instance.PlaySound(tableID);
        }
#endregion


#region IOnExecCommand implementation
        public virtual void BeforeExecFrameCommand(byte seat, IFrameCommand cmd)
        {
        }

        public virtual void AfterExecFrameCommand(byte seat, IFrameCommand cmd)
        {
            BattlePlayer player = null;

            switch (cmd.GetID())
            {
                case FrameCommandID.Leave:
                    {
                        if (null != mDungeonPlayers)
                        {
                            player = mDungeonPlayers.GetPlayerBySeat(seat);

                            if (null != player)
                            {
                                player.onPlayerLeave();
                            }
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattlePlayerLeave);
                        _onPlayerLeave(player);
                    }
                    break;
                case FrameCommandID.ReconnectEnd:
                    {
                        if (null != mDungeonPlayers)
                        {
                            player = mDungeonPlayers.GetPlayerBySeat(seat);
                            player.onPlayerBack();
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattlePlayerBack);
                        _onPlayerBack(player);
                    }
                    break;
                case FrameCommandID.Reborn:
                    {
                        if (null != mDungeonPlayers)
                        {
                            player = mDungeonPlayers.GetPlayerBySeat(seat);
                        }
                        _onReborn(player);
                    }
                    break;
                case FrameCommandID.GameStart:
                    {
                        if (null != mDungeonPlayers)
                        {
                            player = mDungeonPlayers.GetPlayerBySeat(seat);
                        }
                        
                        if(mBattleType == BattleType.ScufflePVP)
                        {
                            _onGameStartFrame(player);
                        }
                        else if (this.mBattleType == BattleType.PVP3V3Battle || !ReplayServer.GetInstance().IsReplay())
                        {
                            _onGameStartFrame(player);
                        }

                    }
                    break;
                case FrameCommandID.RaceEnd:
                    {

                        if (mDungeonManager != null)
                        {
                            mDungeonManager.FinishFight();
                        }
#if !LOGIC_SERVER
                        if (FrameSync.instance.IsInChasingMode)
                            FrameSync.instance.EndChasingMode();
#endif
                        FrameSync.instance.SetDungeonMode(eDungeonMode.LocalFrame);

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleFrameSyncEnd);
                    }
                    break;
                case FrameCommandID.NetQuality:
                    {
                        if (null != mDungeonPlayers)
                        {
                            player = mDungeonPlayers.GetPlayerBySeat(seat);
                        }

                        NetQualityCommand frame = cmd as NetQualityCommand;
                        if (null != frame && null != player)
                        {
                            player.netQuality = (BattlePlayer.eNetQuality)frame.quality;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattlePlayerInfoChange);
                        }
                    }
                    break;
                case FrameCommandID.AutoFight:
                    {
                        Logger.LogProcessFormat("[战斗帧] 自动战斗 {0}", seat);

                        if (null != mDungeonPlayers)
                        {
                            player = mDungeonPlayers.GetPlayerBySeat(seat);
                        }

                        AutoFightCommand frame = cmd as AutoFightCommand;

                        if (null != frame && null != player)
                        {
                            player.isAutoFight = frame.openAutoFight;
                        }
                        else
                        {

                        }
                    }
                    break;
                case FrameCommandID.PlayerQuit:
                    {
                        //player = mDungeonPlayers.GetPlayerBySeat(seat);
                    }
                    break;
                case FrameCommandID.SceneChangeArea:
                    {
                        Logger.LogProcessFormat("[战斗帧] 场景切换");

                        _onSceneAreaChange();
                    }
                    break;
                case FrameCommandID.MatchRoundVote:
                    {
                        Logger.LogProcessFormat("[投票帧]");

                        if (null != mDungeonPlayers)
                        {
                            player = mDungeonPlayers.GetPlayerBySeat(seat);
                        }
                        _onMatchRoundVote(player);
                    }
                    break;
                case FrameCommandID.TeamCopyRaceEnd:
                    {
                        _onTeamCopyRaceEnd();

                        if (mDungeonManager != null)
                        {
                            mDungeonManager.FinishFight();
                        }
#if !LOGIC_SERVER
                        if (FrameSync.instance.IsInChasingMode)
                            FrameSync.instance.EndChasingMode();
#endif
                        if (!ReplayServer.GetInstance().IsLiveShow())
                        {
                            FrameSync.instance.SetDungeonMode(eDungeonMode.LocalFrame);
                        }


                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleFrameSyncEnd);
                    }
                    break;
            }
        }
#endregion

        private void _fireRaceEndOnLocalFrame()
        {
            if (eDungeonMode.LocalFrame == mMode)
            {
                RaceEndCommand raceEnd = new RaceEndCommand();
                FrameSync.instance.FireFrameCommand(raceEnd);
            }
        }

        protected IEnumerator _fireRaceEndOnLocalFrameIter()
        {
            _fireRaceEndOnLocalFrame();

            yield return null;

        }

        protected virtual void _onMatchRoundVote(BattlePlayer player)
        {

        }

        protected virtual void _onRaceEnd()
        {
        }

        protected virtual void _onTeamCopyRaceEnd()
        {

        }

        protected virtual void _onSceneAreaChange()
        {

        }

        protected virtual void _onPlayerLeave(BattlePlayer player)
        {
        }

        protected virtual void _onPlayerBack(BattlePlayer player)
        {
        }

        protected virtual void _onGameStartFrame(BattlePlayer player)
        {

        }
        protected virtual void _onReborn(BattlePlayer player)
        {

        }

        protected IEnumerator _sendMsgWithResend<T0, T1>(ServerType serverType, MessageEvents msgEvents, T0 req, T1 res, bool isShowWaitFrame = true, float timeout = 2.0f, int msgWaitCount = 3, int msgResendCount = 3)

                where T0 : IProtocolStream, IGetMsgID
                where T1 : IProtocolStream, IGetMsgID
        {
            int sendRet = -1;

            if (req != null)
            {
                Logger.LogProcessFormat("[_sendMsgWithResend] 发送消息 {0}", ObjectDumper.Dump(req));
                sendRet = Network.NetManager.Instance().SendCommand(serverType, req);
            }

            if (res != null)
            {
                uint msgId = res.GetMsgID();

                msgEvents.AddMessage(msgId);

                Action<MsgDATA> handle = (MsgDATA recivedMsgData) =>
                {
                    Logger.LogProcessFormat("[_sendMsgWithResend] 回调收到消息 {0}", recivedMsgData.id);
                    msgEvents.SetMessageData(recivedMsgData.id, recivedMsgData);
                };

                NetProcess.AddMsgHandler(msgId, handle);

                int waitCount = 0;
                int resendCount = 0;
                float tmpTimeout = timeout;

                while (!msgEvents.IsAllMessageReceived())
                {
                    if (tmpTimeout > 0.0f)
                    {
                        tmpTimeout -= Time.unscaledDeltaTime;

                        yield return Yielders.EndOfFrame;
                    }
                    else
                    {
                        tmpTimeout = timeout;

                        waitCount++;

                        Logger.LogProcessFormat("[_sendMsgWithResend] 消息超时 {0}", res.GetType());

                        if (waitCount >= msgWaitCount)
                        {
                            waitCount = 0;

                            resendCount++;

                            if (resendCount > msgResendCount)
                            {
                                break;
                            }

                            if (req != null)
                            {
                                Logger.LogProcessFormat("[_sendMsgWithResend] 重新发送消息 {0}", ObjectDumper.Dump(req));
                                sendRet = Network.NetManager.Instance().SendCommand(serverType, req);
                            }
                        }
                    }
                }

                NetProcess.RemoveMsgHandler(msgId, handle);

                if (msgEvents.IsAllMessageReceived())
                {
                    Logger.LogProcessFormat("[_sendMsgWithResend] 收到消息 {0}", msgId);

                    var data = msgEvents.GetMessageData(msgId);
                    res.decode(data.bytes);
                }
            }
        }

        /// <summary>
        /// 这个Update不受帧数据的影响
        /// </summary>
        /// <param name="delta"></param>
        public virtual void Update(int delta)
        {
            if (null != mDungeonManager && null != mDungeonManager.GetGeScene())
            {
                mDungeonManager.GetGeScene().Update(delta);
            }

            if (null != mInputManager)
            {
                mInputManager.Update(delta);
            }

            if(null != MessageSender)
            {
                MessageSender.Update(delta);
            }

            _onUpdate(delta);
        }

        /// <summary>
        /// 帧Update
        /// </summary>
        public virtual void FrameUpdate(int delta)
        {
            if (null != mTrailManager)
            {
                mTrailManager.Update(delta);
            }
        }
        

        //预加载职业资源
        private void PreloadJobRes(BattlePlayer player)
        {
            List<int> skillList = BattlePlayer.GetSkillList(player.playerInfo);
            for (int j = 0; j < skillList.Count; j++)
            {
                SkillPreTable skillPreloadData = TableManager.GetInstance().GetTableItem<SkillPreTable>(skillList[j]);
                if (skillPreloadData == null)
                    continue;
                if (player.IsLocalPlayer() && skillPreloadData.LocalInfoID.Count > 0)
                {
                    for (int k = 0; k < skillPreloadData.LocalInfoID.Length; k++)
                    {
                        SkillPreInfoTable preInfoData = TableManager.GetInstance().GetTableItem<SkillPreInfoTable>(skillPreloadData.LocalInfoID[k]);
                        if(preInfoData != null)
                            CResPreloader.instance.AddRes(preInfoData.Path, false, preInfoData.Count);
                    }
                }

                if (skillPreloadData.InfoID.Count > 0)
                {
                    for (int k = 0; k < skillPreloadData.InfoID.Length; k++)
                    {
                        SkillPreInfoTable preInfoData = TableManager.GetInstance().GetTableItem<SkillPreInfoTable>(skillPreloadData.InfoID[k]);
                        if(preInfoData != null)
                            CResPreloader.instance.AddRes(preInfoData.Path, false, preInfoData.Count);
                    }
                }
            }
        }


        /// <summary>
        /// 初始化关卡编辑器
        /// </summary>
        public void InitLevelManager()
        {
            if (dungeonManager == null)
                return;
            if (dungeonManager.GetDungeonDataManager() == null)
                return;
            if (dungeonManager.GetDungeonDataManager().table == null)
                return;
            if (string.IsNullOrEmpty(dungeonManager.GetDungeonDataManager().table.dungeonLevelPath))
                return;
            mLevelMgr = new LevelManager();
            mLevelMgr.Init(dungeonManager.GetDungeonDataManager().table.dungeonLevelPath,this);
        }

        /// <summary>
        /// 在验证服务器日志中记录关卡通关信息
        /// </summary>
        /// <param name="IdArr"></param>
        public void SetDungeonClearInfo(uint[] IdArr)
        {
            string info = string.Empty;
            for (int i=0;i< IdArr.Length; i++)
            {
                info += IdArr[i];
            }

            if (this.recordServer != null && recordServer.IsProcessRecord())
            {
                this.recordServer.RecordProcess(string.Format("ClearedDungeonIds:{0}", info));
                this.recordServer.MarkString(0x88778766, info);
                // Mark:0x88778766 ClearedDungeonIds:{0}
            }

        }

        private void _OnLoadAsset(string asset)
        {
            Log2File.Log("Load asset:{0}", asset);
        }

        private void _OnLoadAssetPackage(string assetPackage)
        {
            Log2File.Log("Load asset package:{0}", assetPackage);
        }
        
        public bool isDialogFrameOpen = false;
        public void OpenDialog(int dialogId)
        {
            isDialogFrameOpen = true;
#if !LOGIC_SERVER
            var task = new GameClient.TaskDialogFrame.OnDialogOver();
            UnityEngine.Events.UnityAction action = () =>
            {
                var cmd = new CloseFilmCommand();
                cmd.closeDialog = true;
                FrameSync.instance.FireFrameCommand(cmd);
            };
            if (action != null)
            {
                task.AddListener(action);
            }
            GameClient.MissionManager.GetInstance().CreateDialogFrame(dialogId, 0, task);
#endif
        }

        public void CloseDialog()
        {
            if (!isDialogFrameOpen)
            {
                return;
            }

            isDialogFrameOpen = false;
#if !LOGIC_SERVER
            GameClient.MissionManager.GetInstance().CloseAllDialog();
#endif
        }

        protected long GetTimeStamp()
        {
            // //这种方式得到的时间戳更长一点,包含了小数点
            // //TotalSeconds 属性 ： 获取以整秒数和秒的小数部分表示的当前 System.TimeSpan 结构的值
            // //返回：158946540194438
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            // return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public bool isDungeonFail = false;
        protected long startTime = 0;

    }
}
