using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

using Network;
using Protocol;
using DG.Tweening;

public enum PKVideoResult
{
    WIN = 1,
    LOSE = 2,
    DRAW = 3,
    INVALID = 4,
}

public enum PKResult
{
    WIN = 1,
    LOSE = 2,
    DRAW = 4,
    INVALID = 8,
}

/// <summary>
/// Battle类
/// </summary>
namespace GameClient
{
    public enum ePVPBattleEndType
    {
        onNone,

        onTimeOut,

        onAllEnemyDied
    }

    public class PVPBattle : BaseBattle
    {

        protected bool alreadySendResult = false;
        private IBeEventHandle handler = null;

        protected RelaySvrEndGameReq savedReq = null;
        SimpleTimer2 gameTimer = new SimpleTimer2();
        public bool isReplay = false;
        protected ePVPBattleEndType mEndType = ePVPBattleEndType.onNone;

        public PVPBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        { }

        public override void FrameUpdate(int delta)
        {
            base.FrameUpdate(delta);
            if (gameTimer != null)
            {
                gameTimer.UpdateTimer(delta);
            }
            if (isReplay)
                return;
            if (mDungeonManager == null || mDungeonManager.IsFinishFight()) return;
            if (alreadySendResult)
            {
                return;
            }

            //var curSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
            //if (curSystem != null)
            //{
            if (gameTimer != null && gameTimer.IsTimeUp())
            {
                _onTimeUp();
            }
            //}
        }
        protected bool IsTimeUp()
        {
            if (gameTimer != null)
            {
                if (gameTimer.IsTimeUp())
                    return true;
                return false;
            }
            return true;
        }
        protected /*sealed*/ override void _onGameStartFrame(BattlePlayer player)
        {
#if !SERVER_LOGIC 

            ClientSystemManager.instance.CloseFrame<PkLoadingFrame>();
            ClientSystemManager.instance.CloseFrame<Dungeon3v3LoadingFrame>();
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam != null)
            {
                TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);
            }

#endif

            StartCountDown();

            /*
            var obj = ClientSystemManager.instance.PlayUIEffect(FrameLayer.Top, "UIFlatten/Prefabs/Pk/StartFight");

			//同步的倒计时同步
			BattleMain.instance.GetDungeonManager().GetBeScene().StartPKCountDown(Global.PK_COUNTDOWN_TIME, ()=>{
				if (obj != null)
				{
					GameObject.Destroy(obj);
					obj = null;

					var curSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
					if (curSystem != null)
					{
						curSystem.StartTimer(Global.PK_TOTAL_TIME);
					}

					var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
					for(int i=0; i<players.Count; ++i)
					{
						var actor = players[i].playerActor;
						if (actor != null)
							actor.pkRestrainPosition = false;

						if (actor != null && actor.aiManager != null)
						{
							actor.pauseAI = false;

							if (RecordServer.GetInstance().IsRecord())
							{
								RecordServer.GetInstance().RecordProcess("start frame!!!");
							}

							if (RecordServer.GetInstance().NeedRecord())
							{
								RecordServer.GetInstance().RecordStartFrame();
							}

							break;
						}
					}

					BattleMain.instance.Main.TriggerEvent(BeEventSceneType.onStartPK);
				}
			});
*/
            /*
            //if (BattleMain.battleType == BattleType.MutiPlayer || BattleMain.battleType == BattleType.GuildPVP)
            {
				//机器人开启AI！！！！
				ClientSystemManager.instance.delayCaller.DelayCall(Global.PK_COUNTDOWN_TIME * 1000, () =>
                {
					var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
					for(int i=0; i<players.Count; ++i)
					{
						var actor = players[i].playerActor;
						if (actor != null && actor.aiManager != null)
						{
							actor.pauseAI = false;

							if (RecordServer.GetInstance().IsRecord())
							{
								RecordServer.GetInstance().RecordProcess("start frame!!!");
							}

							if (RecordServer.GetInstance().NeedRecord())
							{
								RecordServer.GetInstance().RecordStartFrame();
							}

							break;
						}
					}
                });
            }*/
        }


        public void StartCountDown(bool startAI = true)
        {
#if !SERVER_LOGIC

            var obj = ClientSystemManager.instance.PlayUIEffect(FrameLayer.Top, "UIFlatten/Prefabs/Pk/StartFight");

#endif


            //同步的倒计时同步
            mDungeonManager.GetBeScene().StartPKCountDown(Global.PK_COUNTDOWN_TIME, () =>
            {
                //if (obj != null)
                {
                    //GameObject.Destroy(obj);
                    //obj = null;

#if !LOGIC_SERVER
                    var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPvp>();
                    if (battleUI != null)
                        battleUI.StartTimer(Global.PK_TOTAL_TIME);
#endif
                    gameTimer.SetCountdown(Global.PK_TOTAL_TIME);
                    gameTimer.StartTimer();
                    var players = mDungeonPlayers.GetAllPlayers();
                    for (int i = 0; i < players.Count; ++i)
                    {
                        var actor = players[i].playerActor;
                        if (actor != null)
                        {
                            actor.skillController.StartInitCDForSkill();
                            actor.pkRestrainPosition = false;
                        }
                        if (startAI)
                        {
                            if (actor != null && actor.aiManager != null)
                            {
                                actor.pauseAI = false;

                                if (recordServer != null && recordServer.IsProcessRecord())
                                {
                                    recordServer.RecordProcess("start frame!!!");
                                    recordServer.Mark(0xb8422741);
                                    // Mark:0xb8422741 start frame!!!
                                }

                                //Logger.LogErrorFormat("start robot frame:{0}", FrameSync.instance.curFrame);

#if !LOGIC_SERVER
                                if (RecordServer.GetInstance().IsReplayRecord())
                                {
                                    RecordServer.GetInstance().RecordStartFrame();
                                }
#endif
                                //break;
                            }
                        }
                    }

                    mDungeonManager.GetBeScene().TriggerEventNew(BeEventSceneType.onStartPK);

                }
            });
        }
        protected Coroutine mfinishBattle = null;
        protected virtual void _onTimeUp()
        {
            if (alreadySendResult)
                return;
            //if (mDungeonManager != null)
            //{
            //    if (mDungeonManager.IsFinishFight())
            //    {
            //        return;
            //    }
            //    mDungeonManager.FinishFight();
            //}
            mEndType = ePVPBattleEndType.onTimeOut;
#if !LOGIC_SERVER
            if (!ReplayServer.GetInstance().IsReplay())
            {
                if(mfinishBattle != null)
                {
                    return;// GameFrameWork.instance.StopCoroutine(mfinishBattle);
                }
                mfinishBattle = GameFrameWork.instance.StartCoroutine(_FinishBattle(ePVPBattleEndType.onTimeOut));
            }
#else
            LogicServerSendResult(ePVPBattleEndType.onTimeOut);
#endif
        }

        public void _stopRobotAI()
        {
            if (mDungeonPlayers == null)
                return;
            var players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                var actor = players[i].playerActor;
                if (actor != null && !actor.IsDead() && !actor.pauseAI && actor.aiManager != null)
                    actor.aiManager.Stop();
            }
        }

        protected virtual void _onPlayerDead(BattlePlayer player)
        {
            if (alreadySendResult)
                return;

            _stopRobotAI();
            mDungeonManager.FinishFight();

#if !LOGIC_SERVER
            if (!ReplayServer.GetInstance().IsReplay())
            {
                _saveResult(ePVPBattleEndType.onAllEnemyDied);
                if (mfinishBattle != null)
                {
                    return;// GameFrameWork.instance.StopCoroutine(mfinishBattle);
                }
                mfinishBattle = GameFrameWork.instance.StartCoroutine(_FinishBattle(ePVPBattleEndType.onAllEnemyDied));
            }
#else
            LogicServerSendResult(ePVPBattleEndType.onAllEnemyDied);
#endif
        }
        public void LogicServerSendResult(ePVPBattleEndType a_eEndType)
        {
            if (GetBattleType() !=  BattleType.ScufflePVP && GetBattleType() != BattleType.PVP3V3Battle)
            {
                if (savedReq == null)
                    _saveResult(a_eEndType);

                if (savedReq != null)
                    LogicServer.ReportPkRaceEndToLogicServer(savedReq);

                alreadySendResult = true;
            }
        }
        protected IEnumerator _FinishBattle(ePVPBattleEndType a_eEndType)
        {
#if !LOGIC_SERVER
            if (RecordServer.instance != null)
            {
                RecordServer.instance.PushReconnectCmd("Start _FinishBattle");
            }
#endif
            yield return Yielders.GetWaitForSeconds(Global.Settings.bullteTime * Global.Settings.bullteScale / 1000.0f);
            yield return Yielders.GetWaitForSeconds(0.5f);

         //   _PlayWinAction(a_eEndType);

            _ShowResultEffect(a_eEndType);

            yield return Yielders.GetWaitForSeconds(2.0f);

            _sendResult(a_eEndType);
        }

        protected virtual PKResult _getPKResulte(byte seat, ePVPBattleEndType a_eEndType)
        {
            var player = mDungeonPlayers.GetPlayerBySeat(seat);

            if (a_eEndType == ePVPBattleEndType.onTimeOut)
            {
                var players = mDungeonPlayers.GetAllPlayers();
                for (int i = 0; i < players.Count; ++i)
                {
                    if (players[i].playerInfo.seat != seat)
                    {
                        VFactor selfHpResult = player.playerActor.GetEntityData().GetHPRate();
                        VFactor targetHpResult = players[i].playerActor.GetEntityData().GetHPRate();

                        PKResult result = PKResult.DRAW;

                        if (selfHpResult == targetHpResult)
                            result = PKResult.DRAW;
                        else if (selfHpResult > targetHpResult)
                            result = PKResult.WIN;
                        else
                            result = PKResult.LOSE;

                        return result;
                    }
                }
            }
            else if (a_eEndType == ePVPBattleEndType.onAllEnemyDied)
            {
                if (player.playerActor.IsDead())
                {
                    return PKResult.LOSE;
                }
                else
                {
                    return PKResult.WIN;
                }
            }

            return PKResult.INVALID;
        }

        public void OnPlayWinAction(ePVPBattleEndType a_eEndType)
        {
            if (null == mDungeonPlayers)
            {
                return ;
            }

            List<BattlePlayer> players = mDungeonPlayers.GetAllPlayers();

            for (int i = 0; i < players.Count; ++i)
            {
                if (!BattlePlayer.IsDataValidBattlePlayer(players[i]))
                {
                    continue;
                }

                PKResult result = _getPKResulte(players[i].playerInfo.seat, a_eEndType);

                if (PKResult.WIN == result)
                {
                    BeActor actor = players[i].playerActor;

                    if (actor != null)
                    {
                        handler = BeUtility.ShowWin(actor, handler);
                    }
                }
            }
        }

        protected virtual void _ShowResultEffect(ePVPBattleEndType a_eEndType)
        {
#if !LOGIC_SERVER
            var system = GameClient.ClientSystemManager.instance.CurrentSystem as GameClient.ClientSystemBattle;
#else
			GameClient.ClientSystemBattle system = null;
#endif
            if (system != null && mDungeonPlayers != null)
            {
                var player = mDungeonPlayers.GetMainPlayer();

                if (player != null && player.playerInfo != null)
                {
                    PKResult pkResult = _getPKResulte(player.playerInfo.seat, a_eEndType);

                    var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPvp>();
                    if (battleUI != null)
                        battleUI.ShowPkResult(pkResult);

                    if (pkResult == PKResult.WIN)
                    {
                        PlaySound(19);
                    }
                    else if (pkResult == PKResult.LOSE)
                    {
                        PlaySound(20);
                    }
                }
            }
        }

        protected uint _getVideoScore(PkPlayerRaceEndInfo[] playerInfos)
        {
            var players = mDungeonPlayers.GetAllPlayers();
            if (players.Count < 2)
                return 0;

            if (players[0] == null || players[1] == null)
                return 0;

            if (players[0].playerInfo == null || players[1].playerInfo == null)
                return 0;

            uint hitScore = 0;
            uint seasonScore = 0;
            uint extraScore = 0;

            hitScore += _getHitScore(players[0].playerActor.actorData);
            hitScore += _getHitScore(players[1].playerActor.actorData);

            seasonScore += _getSeasonScore(players[0].playerInfo.seasonLevel,
                                            (PKResult)playerInfos[0].result);
            seasonScore += _getSeasonScore(players[1].playerInfo.seasonLevel,
                                            (PKResult)playerInfos[1].result);
            extraScore += _getExtraScore(players[0].playerActor.actorData);
            extraScore += _getExtraScore(players[1].playerActor.actorData);


            float timeFactor = 0;
            if (mDungeonManager != null)
            {
                var pkTimer = mDungeonManager.GetBeScene().pkTimer;
                if (pkTimer != null)
                {
                    timeFactor = _getTimeFactor(pkTimer.GetPassTime());
                }
            }

            uint replayScore = (uint)((hitScore + extraScore + seasonScore) * timeFactor);

#if UNITY_EDITOR && !LOGIC_SERVER
            Logger.LogError("录像得分------" + replayScore + "（连击得分：" + hitScore + " + 段位得分：" + seasonScore + "额外得分：" + extraScore + "时间系数：" + timeFactor + "）");
#endif

            return replayScore;
        }

        float _getTimeFactor(int passTime)
        {
            int time = Mathf.Abs(150 - passTime);
            if (time < 10)
                return 1;
            if (time < 30)
                return 0.9f;
            if (time < 60)
                return 0.85f;
            if (time < 90)
                return 0.8f;
            if (time < 240)
                return 0.7f;
            return 0.7f;
        }

        uint _getHitScore(BeActorData data)
        {
            return data.GetDamageScore() + data.GetExtraScore();
        }

        uint _getExtraScore(BeActorData data)
        {
            return data.GetExtraScore();
        }

        uint _getSeasonScore(uint seasonLevel, PKResult result)
        {
            uint score = 0;
            uint[] seasonScoreArray = new uint[8] { 0, 105, 150, 255, 270, 300, 360, 450 };//段位分
            ProtoTable.SeasonLevelTable data = TableManager.GetInstance().GetTableItem<ProtoTable.SeasonLevelTable>((int)seasonLevel);
            if (data != null)
            {
                int level = (int)data.MainLevel;
                if (level < seasonScoreArray.Length)
                {
                    score = seasonScoreArray[level];
                }
            }

            if (result == PKResult.WIN)
                score = (uint)(score * 0.7f);
            else if (result == PKResult.LOSE)
                score = (uint)(score * 0.3f);
            else
                score = 0;

            return score;
        }

        protected virtual void _saveResult(ePVPBattleEndType a_eEndType)
        {
            if (savedReq != null)
                return;
            if (mDungeonPlayers == null) return;
            mEndType = a_eEndType;
            RelaySvrEndGameReq req = new RelaySvrEndGameReq();
#if !LOGIC_SERVER
            PkRaceEndInfo info = new PkRaceEndInfo
            {
                gamesessionId = ClientApplication.playerinfo.session
            };
#else
            PkRaceEndInfo info = new PkRaceEndInfo
            {
                gamesessionId =  this.recordServer != null ? Convert.ToUInt64(recordServer.sessionID) : 0UL
            };
#endif

            var players = mDungeonPlayers.GetAllPlayers();
            
            PkPlayerRaceEndInfo[] playerInfos = new PkPlayerRaceEndInfo[players.Count];
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i] != null && players[i].playerInfo != null)
                {
                    var seat = players[i].playerInfo.seat;
                    var tmp = new PkPlayerRaceEndInfo
                    {
                        roleId = players[i].playerInfo.roleId,
                        pos = seat,
                        remainHp = (UInt32)(Mathf.Max(0f, players[i].playerActor.GetEntityData().GetHPRate().single) * 1000),
                        remainMp = (UInt32)(Mathf.Max(0f, players[i].playerActor.GetEntityData().GetMPRate().single) * 1000),
                        result = (byte)_getPKResulte(seat, a_eEndType)//(players[i].playerInfo.accid == ClientApplication.playerinfo.accid?(byte)result:(byte)GetOpposite(result));
                    };
                    //!! 这里是什么意思

                    playerInfos[i] = tmp;
                }
            }
            info.infoes = playerInfos;

            info.replayScore = _getVideoScore(playerInfos);

            req.end = info;

            savedReq = req;
        }

        void _sendResult(ePVPBattleEndType a_eEndType)
        {
            if (savedReq == null)
                _saveResult(a_eEndType);

#if !LOGIC_SERVER
            try
            {
                if (RecordServer.GetInstance().IsReplayRecord())
                {
                    int duration = 0;
                    if (mDungeonManager != null)
                    {
                        var pkTimer = mDungeonManager.GetBeScene().pkTimer;
                        if (pkTimer != null)
                        {
                            duration = pkTimer.GetPassTime();
                        }
                    }
                    else
                    {
                        duration = Global.PK_TOTAL_TIME;
                    }
                    RecordServer.GetInstance().RecordEndReq(savedReq, duration);

                }
            }
            catch(Exception e)
            {
                if(mDungeonManager != null)
                    Logger.LogErrorFormat("_sendResult IsReplayRecord is corrupt! {0} record server is null {1} dungeon scene is null {2}", e.ToString(), RecordServer.GetInstance() == null, mDungeonManager.GetBeScene() == null);
                else
                    Logger.LogErrorFormat("_sendResult IsReplayRecord is corrupt! {0} record server is null {1} dungeon is null {2}", e.ToString(), RecordServer.GetInstance() == null, mDungeonManager == null);
                return;
            }
#endif
            try
            { 
                if (savedReq == null)
                {
                    if (mfinishBattle != null)
                    {
                        GameFrameWork.instance.StopCoroutine(mfinishBattle);
                    }
                    ClientReconnectManager.instance.canReconnectRelay = false;
                    mfinishBattle = null;
                    alreadySendResult = true;
                    return;
                }
                else
                {
                    NetManager.instance.SendCommand(ServerType.RELAY_SERVER, savedReq);
#if !LOGIC_SERVER
                    if (RecordServer.instance != null)
                    {
                        RecordServer.instance.PushReconnectCmd("Start SendResult");
                    }
#endif
                }
            }
            catch(Exception e)
            {
                Logger.LogErrorFormat("_sendResult savedReq is corrupt! {0} NetManager.instance is null {1} GameFrameWork is null {2}  ClientReconnectManager is null {3}", e.ToString(), NetManager.instance == null, GameFrameWork.instance == null, ClientReconnectManager.instance == null);
                return;
            }

            try
            {
                ClientReconnectManager.instance.canReconnectRelay = false;

            // 			var scene = mDungeonManager.GetBeScene();
            // 			if(scene != null)
            // 			{
            // 				scene.state = BeSceneState.onBulletTime;
            // 			}
                if(BattleMain.instance != null)
                    BattleMain.instance.WaitForResult();
            
                mfinishBattle = null;
                alreadySendResult = true;
            }
            catch(Exception e)
            {
                Logger.LogErrorFormat("_sendResult this point is corrupt reason {0}! or  ClientReconnectManager is null {1} BattleMain is null {2}", e.ToString(), ClientReconnectManager.instance == null, BattleMain.instance == null);
                return;
            }


            //GameFrameWork.instance.StartCoroutine(_ShowBattleFinish(result));


            //Logger.LogErrorFormat("Racing End,Send RelaySvrEndGameReq {0} {1} {2} {3}\n", req.resultFlag, req.roldid, req.seat, req.session);
        }

        protected override void _createPlayers()
        {
            int maxLevel = -1;
            var main = mDungeonManager.GetBeScene();
            var players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                var currentBattlePlayer = players[i];
                var current = players[i].playerInfo;

                var skillInfo = BattlePlayer.GetSkillInfo(current);
                var equipsInfo = BattlePlayer.GetEquips(current,true);
                var sideEquipInfo = BattlePlayer.GetSideEquips(current,true);
                var strengthenLevel = BattlePlayer.GetWeaponStrengthenLevel(current);
                var rankBuff = BattlePlayer.GetRankBuff(current);
                var petData = BattlePlayer.GetPetData(current,true);
                var avatarData = BattlePlayer.GetAvatar(current);
                var buffList = BattlePlayer.GetBuffList(current);
                bool isLocalActor = current.accid == ClientApplication.playerinfo.accid;

                bool isShowFashionWeapon = current.avatar.isShoWeapon == 1 ? true : false;
                bool isAIRobot = current.robotAIType > 0 ? true : false;

                var actor = main.CreateCharacter(
                    isLocalActor,
                    current.occupation,
                    current.level,
                    (int)ProtoTable.UnitTable.eCamp.C_HERO + current.seat,
                    skillInfo,
                    equipsInfo,
                    PkRaceType == (int)RaceType.ChiJi ? buffList : null,
                    0,
                    "",
                    strengthenLevel,
                    rankBuff,
                    petData,
                    sideEquipInfo,
                    avatarData,
                    isShowFashionWeapon,
                    isAIRobot
                );

                actor.InitChangeEquipData(current.equips, current.equipScheme);
                if (actor.GetEntityData() != null)
                    actor.GetEntityData().SetCrystalNum(BattlePlayer.GetCrsytalNum(current));

                actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(current);

                players[i].playerActor = actor;

                //临时代码 设置位置，朝向，移除时间
                VInt3 pos = new VInt3(i == 0 ? -4 * (int)IntMath.kIntDen : 4 * (int)IntMath.kIntDen, 0, 0);
                actor.SetPosition(pos, true);
                actor.SetFace(i != 0);
                actor.m_iRemoveTime = GlobalLogic.VALUE_1000 * 120;

#region 空气墙
                //只能上下移动
                /*actor.stateController.SetAbilityEnable(BeAbilityType.X_MOVE, false);
				actor.stateController.SetAbilityEnable(BeAbilityType.Z_MOVE, false);*/
                actor.stateController.SetAbilityEnable(BeAbilityType.ATTACK, false);

                //string effectName = "";
                //Vec3 effectPos = Vec3.zero;
                //左边的玩家
                if (i == 0)
                {
                    //effectName = "Effects/Scene_effects/Door/Prefab/Eff_kongqiqiang_left";
                    //effectPos = new Vec3(-2.5f, 0, 0);

                    actor.pkRestrainPosition = true;
                    actor.pkRestrainRangeX = new VInt2(-4.4f, -3.6f);
                }
                //右边的玩家
                else
                {
                    //effectName = "Effects/Scene_effects/Door/Prefab/Eff_kongqiqiang_right";
                    //effectPos = new Vec3(2.5f, 0, 0);

                    actor.pkRestrainPosition = true;
                    actor.pkRestrainRangeX = new VInt2(3.6f, 4.4f);
                }

                //var effect = main.currentGeScene.CreateEffect(effectName, 100000, effectPos);

                main.RegisterEventNew(BeEventSceneType.onStartPK, (args) =>
                {
                    //main.currentGeScene.DestroyEffect(effect);
                    /*					actor.stateController.SetAbilityEnable(BeAbilityType.X_MOVE, true);
                                        actor.stateController.SetAbilityEnable(BeAbilityType.Z_MOVE, true);*/
                    actor.stateController.SetAbilityEnable(BeAbilityType.ATTACK, true);

                    actor.pkRestrainPosition = false;
                });

                if (!FunctionIsOpen(BattleFlagType.SceneOnReadyStartPetAI))
                {
                    main.RegisterEventNew(BeEventSceneType.AfterOnReady, args =>
                    {
                        if (actor.pet != null && actor.pet.GetEntityData() != null && actor.pet.GetEntityData().isPet && actor.pet.aiManager != null)
                        {
                            actor.pet.aiManager.Stop();
                        }
                    });
                }
                #endregion

#if !LOGIC_SERVER
                //actor.RegisterEvent(BeEventType.onBeKilled, args =>
                //{
                //ShowBattleResult(ReplayServer.GetInstance().recordData.resultInfo);
                //ReplayServer.GetInstance().Stop(true);
                //});
                if (!ReplayServer.GetInstance().IsReplay())
                    actor.RegisterEventNew(BeEventType.onBeKilled, args => { _onPlayerDead(currentBattlePlayer); });
                //actor.RegisterEvent(BeEventType.onBeKilled, args => { _onPlayerDead(currentBattlePlayer); });
#else
                actor.RegisterEventNew(BeEventType.onBeKilled, args => { _onPlayerDead(currentBattlePlayer); });
				//actor.RegisterEvent(BeEventType.onBeKilled, args =>{ _onPlayerDead(currentBattlePlayer); });
#endif

                if (current.accid == ClientApplication.playerinfo.accid)
                {
                    actor.isLocalActor = true;
                    actor.m_pkGeActor.AddTittleComponent(BattlePlayer.GetTitleID(current), current.name, 0, "", current.level, (int)current.seasonLevel, PlayerInfoColor.PK_PLAYER);
                    actor.m_pkGeActor.CreateInfoBar(current.name, PlayerInfoColor.PK_PLAYER, current.level);
                    actor.m_pkGeActor.CreatePKHPBar(i == 0 ? CPKHPBar.PKBarType.Left : CPKHPBar.PKBarType.Right, current.name, PlayerInfoColor.PK_PLAYER);
                }
                else
                {
                    if (ReplayServer.GetInstance().IsReplay())
                    {
                        if (i == 0)
                            actor.isLocalActor = true;
                    }

                    actor.m_pkGeActor.AddTittleComponent(BattlePlayer.GetTitleID(current), current.name, 0, "", current.level, (int)current.seasonLevel, PlayerInfoColor.PK_OTHER_PLAYER);
                    actor.m_pkGeActor.CreateInfoBar(current.name, PlayerInfoColor.PK_OTHER_PLAYER, current.level);
                    actor.m_pkGeActor.CreatePKHPBar(i == 0 ? CPKHPBar.PKBarType.Left : CPKHPBar.PKBarType.Right, current.name, PlayerInfoColor.PK_OTHER_PLAYER);
                    actor.m_pkGeActor.CreateFootIndicator();
                }

                if (null != actor.m_pkGeActor)
                {
                    actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
                }

                //TODO 加机器人判定
                if (current.robotAIType > 0)
                {
                    //BattlePlayer.DebugPrint(current);
                    InitAIForRobot(actor, (int)current.robotAIType, current.robotHard);
                }


                if (petData != null)
                    actor.SetPetData(petData);
                actor.CreateFollowMonster();

                maxLevel = Math.Max(maxLevel, current.level);

                actor.isMainActor = true;

                actor.UseProtect();

                actor.UseActorData();
            }


            //var players = mDungeonPlayers.GetAllPlayers();
            //这里确保只要1v1！！！
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].playerActor.UseAdjustBalance();
                var targetPlayer = players[i == 0 ? 1 : 0];
                players[i].playerActor.GetEntityData().AdjustHPForPvP(
                    players[i].playerInfo.level,
                    targetPlayer.playerInfo.level,
                    players[i].playerInfo.occupation,
                    targetPlayer.playerInfo.occupation,
                    PkRaceType);
            }

            _postCreatePlayer();

#if !LOGIC_SERVER
            mDungeonManager.GetGeScene().AttachCameraTo(mDungeonPlayers.GetMainPlayer().playerActor.m_pkGeActor);
#endif
        }

        public virtual void _postCreatePlayer()
        {
#if DEBUG_SETTING
			if (Global.Settings.isDebug)
			{
				var players = mDungeonPlayers.GetAllPlayers();
				var localPlayer = mDungeonPlayers.GetMainPlayer();
				for(int i=0; i<players.Count; ++i)
				{
					var actor = players[i].playerActor;
					if (Global.Settings.playerHP > 0 && players[i]==localPlayer || 
						Global.Settings.monsterHP > 0 && players[i]!=localPlayer)
					{
						int hp = 0;
						if (Global.Settings.playerHP > 0 && players[i]==localPlayer)
							hp = Global.Settings.playerHP;
						if (Global.Settings.monsterHP > 0 && players[i]!=localPlayer)
							hp = Global.Settings.monsterHP;
						
						//actor.GetEntityData().battleData.hp = hp;
						//actor.GetEntityData().battleData.maxHp = hp;
						actor.GetEntityData().SetHP(hp);
						actor.GetEntityData().SetMaxHP(hp);
						actor.m_pkGeActor.ResetHPBar();
					}
						
				}
			}
#endif
        }

        protected sealed override bool _isBattleLoadFinish()
        {
            bool loadFinish = true;
            var players = mDungeonPlayers.GetAllPlayers();
            if(null != players)
            {
                for (int i = 0; i < players.Count; ++i)
                {
                    var currentBattlePlayer = players[i];
                    if(null != currentBattlePlayer)
                    {
                        BeActor beActor = currentBattlePlayer.playerActor;
                        if (null != beActor && null != beActor.m_pkGeActor)
                            loadFinish = loadFinish ? beActor.m_pkGeActor.IsAvatarLoadFinished():loadFinish;
                    }
                }
            }

            return loadFinish;
        }

        void InitAIForRobot(BeActor actor, int aiLevel, int matchNum = 0)
        {
            var tables = TableManager.GetInstance().GetTable<ProtoTable.AIConfigTable>();
            ProtoTable.AIConfigTable aiData = null;
            foreach (var item in tables)
            {
                var data = item.Value as ProtoTable.AIConfigTable;
                if (data.JobID == actor.GetEntityData().professtion && data.AIType == aiLevel)
                {
                    aiData = data;
                }
            }
            actor.isPkRobot = true;
            if (aiData != null)
            {
                actor.InitAutoFight(
                    aiData.AIActionPath,
                    aiData.AIDestinationSelectPath,
                    aiData.AIEventPath,
                    aiData.AIAttackDelay,
                    aiData.AIDestinationChangeTerm,
                    aiData.AIThinkTargetTerm,
                    aiData.AIKeepDistance
                );
            }
            else
            {
                Logger.LogErrorFormat("找不到机器人AI难度:{0}", aiLevel);
            }

            //1-50, 2-70,3-70
            float[] hpAdjustRate = { 0.35f, 0.5f, 0.6f };
            if (matchNum >= 1 && matchNum <= 3)
            {
                var attribute = actor.GetEntityData();
                if (attribute != null)
                {
                    float hpRate = hpAdjustRate[matchNum - 1];
                    int originHP = attribute.GetMaxHP();
                    int newhp = IntMath.Float2Int(attribute.GetMaxHP() * hpRate);

                    attribute.SetMaxHP(newhp);
                    attribute.SetHP(newhp);
                    actor.m_pkGeActor.ResetHPBar();

                    //Logger.LogErrorFormat("match:{0} originHP:{1} rate:{2} newHP:{3}", matchNum, originHP, hpRate, newhp);
                }
            }
        }

        [MessageHandle(SceneMatchPkRaceEnd.MsgID)]
        void _OnReceivePkEndData(MsgDATA msg)
        {

            SceneMatchPkRaceEnd pkEndData = new SceneMatchPkRaceEnd();
            pkEndData.decode(msg.bytes);

            Logger.LogWarningFormat("ReceivePkEndData {0}\n", ObjectDumper.Dump(pkEndData));

#if !LOGIC_SERVER
            if (RecordServer.GetInstance().IsReplayRecord())
            {
                RecordServer.GetInstance().RecordResult(pkEndData);
            }
            if (RecordServer.GetInstance() != null)
            {
                RecordServer.GetInstance().PushReconnectCmd("PkEndData");
            }

#endif

                //mDungeonManager.FinishFight();
                BattleMain.instance.End();


            ClientSystemManager.instance.OpenFrame<PKBattleResultFrame>(FrameLayer.Middle, pkEndData);
#if !LOGIC_SERVER
            if (RecordServer.GetInstance() != null)
            {
                RecordServer.GetInstance().PushReconnectCmd("BattleResultEnd");
            }

            if (RecordServer.GetInstance().IsReplayRecord())
            {
                RecordServer.GetInstance().EndRecord("PkEnd");
            }
#endif
        }


        //         [MessageHandle(RelaySvrGameResultNotify.MsgID)]
        //         void OnRelaySvrGameResultNotify(MsgDATA msg)
        //         {
        //             RelaySvrGameResultNotify ret = new RelaySvrGameResultNotify();
        //             ret.decode(msg.bytes);

        // TODO
        // 			if (RecordServer.GetInstance().NeedRecord())
        // 			{
        // 				RecordServer.GetInstance().RecordResult(ret);
        // 				RecordServer.GetInstance().EndRecord();
        //             }

        //             var curSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
        //             if (curSystem != null)
        //             {
        //                 curSystem.StopTimer();
        //             }

        //FrameSync.instance.isFire = false;

        // 			if (IsWin(ret))
        // 			{
        // 				var actor = BattleMain.instance.GetLocalPlayer().playerActor;
        // 				if (actor != null)
        // 				{
        // 					handler = BeUtility.ShowWin(actor, handler);
        // 				}
        // 			}
        // 				
        //             ShowBattleResult(ret);
        //         }
        // 
        // 		public bool IsWin(RelaySvrGameResultNotify ret)
        // 		{
        // 			for(int i = 0; i < ret.results.Length; ++i)
        // 			{
        // 				var cur = ret.results[i];
        // 				if (cur.roldid == PlayerBaseData.GetInstance().RoleID)
        // 					return cur.flag == 1;
        // 			}
        // 
        // 			return false;
        // 		}

        // 		public void ShowBattleResult(RelaySvrGameResultNotify ret)
        // 		{
        //FrameSync.instance.StopFrameSync();
        //Logger.LogWarningFormat("RelaySvrGameResultNotify {0}\n",ret.reason);

        // 			bool win = false;
        // 			string winName = "";
        // 			string loseName = "";
        // 			for(int i = 0; i < ret.results.Length; ++i)
        // 			{
        // 				var cur = ret.results[i];
        // 
        // 				if (cur.roldid == PlayerBaseData.GetInstance().RoleID)
        // 				{
        // 					win = cur.flag == 1;
        // 					PKResultFrame.SetResult(cur.flag);
        //                     PKBattleResultFrame.SetBattleResult(cur.flag);
        // 				}
        // 
        // 				if(cur.flag == 1)
        // 				{
        // 					winName = mDungeonPlayers.GetPlayerByRoleID(cur.roldid).playerInfo.name;
        // 				}
        // 				else
        // 				{
        // 					loseName = mDungeonPlayers.GetPlayerByRoleID(cur.roldid).playerInfo.name;
        // 				}
        // 			}
        // 
        // 			string str = string.Format("<color=yellow>{0}</color> 战胜了 <color=yellow>{1}</color>",winName,loseName);
        // 			PKResultFrame.SetResultText(str);

        // 			FrameSync.instance.SetDungeonMode(eDungeonMode.LocalFrame);
        // 			ClientSystemManager.GetInstance().delayCaller.DelayCall(2000, ()=>{
        // 				mDungeonManager.FinishFight();
        // 				BattleMain.instance.End();
        // 				GameFrameWork.instance.StartCoroutine(DelayOpenFrame());		
        // 			});
        // 		}

        //         IEnumerator DelayOpenFrame()
        //         {
        //             bool bInLoading = false;
        //             if(ClientApplication.ops != null)
        //             {
        //                 bInLoading = true;
        //                 ClientApplication.ops.allowSceneActivation = true;
        //                 yield return ClientApplication.ops;
        //             }
        // 
        //             yield return Yielders.EndOfFrame;
        // 
        //             if(false == bInLoading)
        //             {
        //                 if (ClientSystemManager.instance.IsFrameOpen<PkLoadingFrame>())
        //                 {
        //                     ClientSystemManager.instance.CloseFrame<PkLoadingFrame>();
        //                 }
        // 
        //                 //ClientSystemManager.instance.OpenFrame<PKResultFrame>(FrameLayer.Middle);
        //                 ClientSystemManager.instance.OpenFrame<PKBattleResultFrame>(FrameLayer.Middle);
        //             }
        //         }

        //         [ProtocolHandle(typeof(RelaySvrRaceEndNotify))]
        //         void OnRelaySvrNotifyGameEnd(RelaySvrRaceEndNotify msg)
        //         {
        // 
        //             FrameSync.instance.StopFrameSync();
        // 
        // 
        //             if(msg.reason != (int)RaceEndReason.Normal)
        //             {
        //                 RaceEndReason ret = (RaceEndReason)msg.reason;
        //                 string message = "RelaySvrNotifyGameEnd: " + ret.GetDescription(); 
        //                 SystemNotifyManager.SysNotifyMsgBoxOK(message, ()=>{ ClientSystemManager.instance.SwitchSystem<ClientSystemTown>(); } );
        //             }
        //             Logger.LogProcessFormat("stop frame sync!!!");
        //         }

    }
}
