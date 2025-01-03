using UnityEngine;
using System;
using System.Collections.Generic;
using Protocol;
using Battle;
using Network;

namespace GameClient
{
    public class BaseMatchPVPBattle : BaseBattle
    {
        public BaseMatchPVPBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {
        }


        // TODO 这个数据移动到数据管理类之中去?
        private int mRoundIndex = 0;

        protected int _getMatchRoundIndex()
        {
            return mRoundIndex;
        }

        protected void _nextMatchRoundIndex()
        {
            mRoundIndex++;
        }

        protected void _openDungeonReadyFightFrame(byte redSeat, byte blueSeat)
        {
#if !LOGIC_SERVER
            DungeonReadyFightFrame.MatchedFighters match = new DungeonReadyFightFrame.MatchedFighters();
            match.redTeamSeat                            = redSeat;
            match.blueTeamSeat                           = blueSeat;
            match.roundIndex                             = _getMatchRoundIndex();

            ClientSystemManager.instance.OpenFrame<DungeonReadyFightFrame>(FrameLayer.Middle, match);
#endif
        }

        protected void _closeDungeonReadyFightFrame()
        {
#if !LOGIC_SERVER
            ClientSystemManager.instance.CloseFrame<DungeonReadyFightFrame>();
#endif
        }

        protected bool _sendRelaySvrEndGameReq()
        {
#if !LOGIC_SERVER
            RelaySvrEndGameReq req = _getRelaySvrEndGameReq();

            if (null == req)
            {
                return false;
            }
    
            NetManager.instance.SendCommand(ServerType.RELAY_SERVER, _getRelaySvrEndGameReq());

            return true;
#else
            return false;
#endif
        }

        protected RelaySvrEndGameReq _getRelaySvrEndGameReq()
        {
            List<BattlePlayer> players        = mDungeonPlayers.GetAllPlayers();

            PkPlayerRaceEndInfo[] playerInfos = new PkPlayerRaceEndInfo[players.Count];

            PkRaceEndInfo info                = new PkRaceEndInfo();
            info.gamesessionId                = ClientApplication.playerinfo.session;
            info.infoes                       = playerInfos;

            RelaySvrEndGameReq req            = new RelaySvrEndGameReq();
            req.end                           = info;

            for (int i = 0; i < players.Count; ++i)
            {
                PkPlayerRaceEndInfo tmp = new PkPlayerRaceEndInfo();
                playerInfos[i]          = tmp;

                tmp.roleId              = players[i].playerInfo.roleId;
                tmp.pos                 = players[i].playerInfo.seat;
                tmp.result              = (byte)_getRaceEndResult(players[i].teamType);

                if (null != players[i].playerActor)
                {
                    tmp.remainHp = (UInt32)(Mathf.Max(0f, players[i].playerActor.GetEntityData().GetHPRate().single) * 1000);
                    tmp.remainMp = (UInt32)(Mathf.Max(0f, players[i].playerActor.GetEntityData().GetMPRate().single) * 1000);

                    tmp.damagePercent = (UInt32)players[i].GetPKAllDamageRate();
                }
            }

            return req;
        }

        protected PKResult _getRaceEndResult(BattlePlayer.eDungeonPlayerTeamType type)
        {
            // TODO Dungeon3v3FinishFrame.cs: 241 _getTeamRaceEndResult
            //  我方队伍有人还没参战
            if (!mDungeonPlayers.IsTeamPlayerAllFighted(type))
            {
                return PKResult.WIN;
            }

            // 对方队伍有人还没参战
            if (!mDungeonPlayers.IsTeamPlayerAllFighted(BattlePlayer.GetTargetTeamType(type)))
            {
                return PKResult.LOSE;
            }

            // 所有场上玩家都经理战斗
            List<BattlePlayer> allPlayers = mDungeonPlayers.GetAllPlayers();

            PKResult pKResult = PKResult.INVALID;
            int maxIndex = -1;

            for (int i = 0; i < allPlayers.Count; ++i)
            {
                if (allPlayers[i].teamType == type)
                {
                    // 查找最后一轮玩家的战绩
                    int roundIdx = allPlayers[i].GetLastPKRoundIndex();
                    if (roundIdx > maxIndex)
                    {
                        pKResult = allPlayers[i].GetLastPKResult();
                        maxIndex = roundIdx;
                    }
                }
            }

            return pKResult;
        }

        /// <summary>
        /// 显示匹配结果
        /// </summary>
        protected void _showMatchResult(PKResult pkResult)
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPvp>();
            if (battleUI != null)
            {
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
#endif
        }

        /// <summary>
        /// 隐藏匹配结果
        /// </summary>
        protected void _hiddenMatchResult()
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPvp>();
            if (battleUI != null)
            {
                battleUI.HiddenPkResult();
            }
#endif
        }

        protected void _showMainPlayerIsNextPlayer()
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUI3V3>();
            if (battleUI != null)
            {
                battleUI.ShowPVP3V3Tips();
            }
#endif
        }

        protected void _hiddenMainPlayerIsNextPlayer()
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUI3V3>();
            if (battleUI != null)
            {
                battleUI.HiddenPVP3V3Tips();
            }
#endif
        }

        /// <summary>
        /// 获得比赛结果
        /// </summary>
        protected PKResult _getMatchPVPResult(byte seat, ePVPBattleEndType a_eEndType)
        {
            BattlePlayer player = mDungeonPlayers.GetPlayerBySeat(seat);

            if (!_isValidCharacter(player))
            {
                Logger.LogErrorFormat("[战斗] 传入座位号 {0} 玩家为空", seat);
                return PKResult.INVALID;
            }

            BattlePlayer targetPlayer = mDungeonPlayers.GetTargetPlayer(player.playerInfo.seat);
            if (!_isValidCharacter(targetPlayer))
            {
                Logger.LogErrorFormat("[战斗] 传入座位号 {0} 玩家敌对玩家为空", seat);
                return PKResult.INVALID;
            }

            PKResult result = PKResult.INVALID;

            if (a_eEndType == ePVPBattleEndType.onTimeOut)
            {
                VFactor selfHpResult = player.playerActor.GetEntityData().GetHPRate();
                VFactor targetHpResult = targetPlayer.playerActor.GetEntityData().GetHPRate();

                if (selfHpResult == targetHpResult)
                {
                    result = PKResult.DRAW;
                }
                else if (selfHpResult > targetHpResult)
                {
                    result = PKResult.WIN;
                }
                else
                {
                    result = PKResult.LOSE;
                }
            }
            else if (a_eEndType == ePVPBattleEndType.onAllEnemyDied)
            {
                if (mDungeonPlayers.IsPlayerDeadByBattlePlayer(player) && mDungeonPlayers.IsPlayerDeadByBattlePlayer(targetPlayer))
                {
                    result = PKResult.DRAW;
                }
                else if (mDungeonPlayers.IsPlayerDeadByBattlePlayer(player))
                {
                    result = PKResult.LOSE;
                }
                else if (mDungeonPlayers.IsPlayerDeadByBattlePlayer(targetPlayer))
                {
                    result = PKResult.WIN;
                }
            }

            Logger.LogProcessFormat("[战斗] 战斗结果 {0} 获取玩家 {1} -VS- 玩家 {2}", result, player.GetPlayerName(), targetPlayer.GetPlayerName());

            _setMatchPVPResultForBattlePlayer(player, targetPlayer, result, a_eEndType);

            return result;
        }

        private void _setMatchPVPResultForBattlePlayer(BattlePlayer player, BattlePlayer targetPlayer, PKResult result, ePVPBattleEndType endType)
        {
            if (null == player || null == targetPlayer)
            {
                Logger.LogErrorFormat("[战斗] 保存战斗结果出错，因为传入玩家对象为空");
                return ;
            }

            uint selfHpResult = (uint)(player.playerActor.GetEntityData().GetHPRate().single * 10000);
            uint targetHpResult = (uint)(targetPlayer.playerActor.GetEntityData().GetHPRate().single * 10000);

            player.AddPKResult(_getMatchRoundIndex(), targetPlayer.GetPlayerSeat(), result, endType, selfHpResult);
            targetPlayer.AddPKResult(_getMatchRoundIndex(), player.GetPlayerSeat(), _getTargetPkResult(result), endType, targetHpResult);

            player.AddDamageData(targetPlayer.GetPlayerSeat(), targetPlayer.GetLastHurtedRate());
            targetPlayer.AddDamageData(player.GetPlayerSeat(), player.GetLastHurtedRate());
        }

        private PKResult _getTargetPkResult(PKResult pkResult)
        {
            switch (pkResult)
            {
                case PKResult.INVALID:
                case PKResult.DRAW:
                    return pkResult;
                case PKResult.WIN:
                    return PKResult.LOSE;
                case PKResult.LOSE:
                    return PKResult.WIN;
                default:
                    Logger.LogErrorFormat("[战斗] 错误pk结果类型 {0}", pkResult);
                    return PKResult.INVALID;
            }
        }

        /// <summary>
        /// 双击跑的配置帧
        /// </summary>
        protected void _fireDoublePressConfigFrame()
        {
            BattlePlayer battlePlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            if (!BattlePlayer.IsDataValidBattlePlayer(battlePlayer))
            {
                return;
            }

            bool isAttackReplaceLigui = false;
            var actor = battlePlayer.playerActor;
            if (actor != null && actor.GetEntityData()!= null && actor.GetEntityData().professtion == 11)
                isAttackReplaceLigui = SettingManager.GetInstance().GetLiGuiValue(SettingManager.STR_LIGUI);

            string key = BattleMain.IsModePvP(GetBattleType()) ? SettingManager.STR_CHASER_PVP: SettingManager.STR_CHASER_PVE;
            byte chaserSetting = (byte) SettingManager.GetInstance().GetChaserSetting(key);
            
            DoublePressConfigCommand cmd = new DoublePressConfigCommand();
            cmd.attackReplaceLigui = isAttackReplaceLigui;
            cmd.hasDoublePress = Global.Settings.hasDoubleRun;
            cmd.hasRunAttackConfig = SettingManager.GetInstance().GetRunAttackMode() == InputManager.RunAttackMode.QTE;
            cmd.canUseCrystalSkill = BeUtility.CheckVipAutoUseCrystalSkill();
            cmd.backHitConfig = SettingManager.GetInstance().GetValue(SettingManager.STR_BACKHIT);
            cmd.autoHitConfig = SettingManager.GetInstance().GetValue(SettingManager.STR_AUTOHIT);
            cmd.paladinAttackCharge = SettingManager.GetInstance().GetPaladinAttack() == InputManager.PaladinAttack.OPEN;
            cmd.chaserSwitch = chaserSetting;
            FrameSync.instance.FireFrameCommand(cmd);
        }

        /// <summary>
        /// 添加相机视角到玩家
        /// </summary>
        protected bool _updateCameraForBattlePlayer(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                Logger.LogError("[战斗] 设置跟随相机的目标玩家为空");
                return false;
            }

            GeSceneEx geScene = mDungeonManager.GetGeScene();
            if (null == geScene)
            {
                Logger.LogError("[战斗] 设置跟随相机的 geScene 为空");
                return false;
            }

            geScene.AttachCameraTo(player.playerActor.m_pkGeActor);

            return true;
        }

        /// <summary>
        /// 开启本剧战斗的准备倒计时
        /// </summary>
        protected void _startRoundReadyFight(int countTimeInSec, int fightTimeInSec, bool startAI = true)
        {
#if !SERVER_LOGIC
            var obj = ClientSystemManager.instance.PlayUIEffect(FrameLayer.Top, "UIFlatten/Prefabs/Pk/StartFight");
#endif
            Logger.LogProcessFormat("[战斗] 开始本场战斗倒计时 时长:{0}, 是否有AI:{1}", countTimeInSec, startAI);
            
            mDungeonManager.GetBeScene().TriggerEventNew(BeEventSceneType.on3v3SwitchNext);    //3v3比赛开始

            List<BattlePlayer> players = mDungeonPlayers.GetAllPlayers();
           

            //同步的倒计时同步
            mDungeonManager.GetBeScene().StartPKCountDown(countTimeInSec, () =>
            {
                {
#if !LOGIC_SERVER
                    var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPvp>();
                    if(battleUI != null)
                        battleUI.StartTimer(fightTimeInSec);
#endif
                    for (int i = 0; i < players.Count; ++i)
                    {
                        if (!_isValidCharacter(players[i]))
                        {
                            continue;
                        }

                        BeActor actor = players[i].playerActor;
                        if (players[i].isFighting)
                        {
                            actor.skillController.StartInitCDForSkill();
                            actor.pkRestrainPosition = false;
                        }

                        _unlimitCharactorRestrainZoneWithBattlePlayer(players[i]);

                        if (startAI)
                        {
                            if (actor.aiManager != null)
                            {
                                actor.pauseAI = false;

                                if (recordServer != null && recordServer.IsProcessRecord())
                                {
                                    recordServer.RecordProcess("start frame!!!");
                                    recordServer.Mark(0xa141440);
                                    // Mark:0xa141440 start frame!!!
                                }

#if !LOGIC_SERVER
                                if (RecordServer.GetInstance().IsReplayRecord())
                                {
                                    RecordServer.GetInstance().RecordStartFrame();
                                }
#endif
                            }
                        }
                    }

                    Logger.LogProcessFormat("[战斗] 准备时间结束");

                    _onMatchBattlePlayerReady2Fight();
                }
            });
        }

        protected bool _adjustBalanceMatchPlayer(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            BattlePlayer targetPlayer = mDungeonPlayers.GetTargetPlayer(player.GetPlayerSeat());

            if (!_isValidCharacter(targetPlayer))
            {
                Logger.LogErrorFormat("[战斗] 多人匹配 天平调整 {0} 对手为空", player.GetPlayerSeat());
                return false;
            }

            player.playerActor.UseAdjustBalance();
            player.playerActor.GetEntityData().AdjustHPForPvP(
                    player.playerInfo.level,      targetPlayer.playerInfo.level,
                    player.playerInfo.occupation, targetPlayer.playerInfo.occupation,
                    PkRaceType);

            Logger.LogProcessFormat("[战斗] 多人匹配 天平调整 {0} vs {1}", player.GetPlayerName(), targetPlayer.GetPlayerName());

            return true;
        }

        /// <summary>
        /// 创建一个pk玩家
        /// </summary>
        protected BeActor _createMatchPlayer(BattlePlayer player)
        {
            BeActor actor = _createCharacterForBattlePlayer(player);
            if (null == actor)
            {
                return null;
            }
            
            _initCharactorData(player);
            _initFollowCameara(player);
            _createCharacterExtraUI(player);
            _bindCharactorEvents(player);

            Logger.LogProcessFormat("[战斗] 多人匹配 创建匹配玩家 {0}", player.GetBattlePlayerInfo());

            return actor;
        }

        private BeActor _createCharacterForBattlePlayer(BattlePlayer player)
        {
            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return null;
            }

            BeScene main = mDungeonManager.GetBeScene();
            if (null == main)
            {
                return null;
            }

            var serverData      = player.playerInfo;

            var skillInfo       = BattlePlayer.GetSkillInfo(serverData);
            var equipsInfo      = BattlePlayer.GetEquips(serverData,true);
            var sideEquipInfo   = BattlePlayer.GetSideEquips(serverData,true);
            var strengthenLevel = BattlePlayer.GetWeaponStrengthenLevel(serverData);
            var rankBuff        = BattlePlayer.GetRankBuff(serverData);
            var petData         = BattlePlayer.GetPetData(serverData,true);
            var avatarData      = BattlePlayer.GetAvatar(serverData);

            bool isShowFashionWeapon = serverData.avatar.isShoWeapon == 1 ? true : false;
            bool isAIRobot = serverData.robotAIType > 0 ? true : false;

            var actor = main.CreateCharacter
            (
                player.IsLocalPlayer(),
                serverData.occupation,
                serverData.level,
                player.GetPlayerCamp(),
                skillInfo,
                equipsInfo,
                null, 0, "",
                strengthenLevel,
                rankBuff,
                petData,
                sideEquipInfo,
                avatarData,
                isShowFashionWeapon,
                isAIRobot
            );

            actor.InitChangeEquipData(serverData.equips, serverData.equipScheme);
            if (petData != null)
            {
                actor.SetPetData(petData);
                actor.CreateFollowMonster();
            }

            if (null != actor.m_pkGeActor)
            {
                actor.m_pkGeActor.PushPostLoadCommand(()=>
                {
                    Logger.LogProcessFormat("[战斗] 创建 加载完成 完成玩家实体 {0}", player.GetBattlePlayerInfo());
                });
            }

            player.playerActor = actor;
            player.isFighting  = true;

            Logger.LogProcessFormat("[战斗] 创建一个玩家实体 {0}", player.GetBattlePlayerInfo());
            return actor;
        }

        private bool _initFollowCameara(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            BattlePlayer localPlayer = mDungeonPlayers.GetMainPlayer();
            if (null == localPlayer)
            {
                return false;
            }

            if (localPlayer.teamType == player.teamType)
            {
                _updateCameraForBattlePlayer(player);
            }

            return true;
        }

        private bool _initCharactorData(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            BeActor        actor            = player.playerActor;
            RacePlayerInfo serverPlayerData = player.playerInfo;

            if (actor.GetEntityData() != null)
            {
                actor.GetEntityData().SetCrystalNum(BattlePlayer.GetCrsytalNum(serverPlayerData));
            }

            actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(serverPlayerData);

            _setCharactorAtBirthPosition(player);
            _limitCharactorRestrainZone(player);

            //TODO 加机器人判定
            if (serverPlayerData.robotAIType > 0)
            {
                //BattlePlayer.DebugPrint(current);
                _initCharactorAIForRobot(actor, (int)serverPlayerData.robotAIType, serverPlayerData.robotHard);
            }

            actor.isMainActor = true;
            actor.UseProtect();
            actor.UseActorData();

            return true;
        }

        /// <summary>
        /// 设置玩家位置
        /// </summary>
        protected bool _setCharactorAtBirthPosition(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            Logger.LogProcessFormat("[战斗] 多人匹配 设置玩家出生位置 {0}", player.GetBattlePlayerInfo()); 

            BeActor actor = player.playerActor;

            //临时代码 设置位置，朝向，移除时间
            VInt3 pos = new VInt3(player.IsTeamRed() ? -4 * (int)IntMath.kIntDen : 4 * (int)IntMath.kIntDen, 0, 0);
            actor.SetPosition(pos, true);
            actor.SetFace(!player.IsTeamRed());
            actor.m_iRemoveTime = GlobalLogic.VALUE_1000 * 120;

            return true;
        }

        protected bool _resetCharactorStatus(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            BeActor actor = player.playerActor;

            actor.ResetActorStatus();
           
            //  删除所有召唤兽
            BeScene scene = mDungeonManager.GetBeScene();

            if (null != scene)
            {
                scene.ForceDoDeadEntityByOwner(actor);
            }
           
            return true;
        }

        /// <summary>
        /// 移除过门不带过去的实体
        /// </summary>
        protected void _sceneEntityRemoveAll()
        {
            BeScene scene = mDungeonManager.GetBeScene();
            if (null != scene)
            {
                scene.Pvp3v3EntityRemoveAll();
            }
        }

        /// <summary>
        /// 空气墙
        /// </summary>
        protected bool _limitCharactorRestrainZone(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            BeActor actor = player.playerActor;

            Logger.LogProcessFormat("[战斗] 多人匹配 开启空气墙 {0}", player.GetBattlePlayerInfo()); 

            //只能上下移动
            actor.stateController.SetAbilityEnable(BeAbilityType.ATTACK, false);

            //左边的玩家
            if (player.IsTeamRed())
            {
                actor.pkRestrainPosition = true;
                actor.pkRestrainRangeX = new VInt2(-4.4f, -3.6f);
            }
            else //右边的玩家
            {
                actor.pkRestrainPosition = true;
                actor.pkRestrainRangeX = new VInt2(3.6f, 4.4f);
            }

            return true;
        }

        private bool _unlimitCharactorRestrainZoneWithBattlePlayer(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            Logger.LogProcessFormat("[战斗] 多人匹配 解除空气墙 {0}", player.GetBattlePlayerInfo()); 

            BeActor actor            = player.playerActor;

            actor.pkRestrainPosition = false;
            actor.stateController.SetAbilityEnable(BeAbilityType.ATTACK, true);

            return true;
        }

        private void _initCharactorAIForRobot(BeActor actor, int aiLevel, int matchNum = 0)
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


        private bool _createCharacterExtraUI(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            BeActor        actor            = player.playerActor;
            RacePlayerInfo serverPlayerData = player.playerInfo;
           
            if (player.IsLocalPlayer())
            {
                actor.isLocalActor = true;
                actor.m_pkGeActor.CreatePKHPBar(player.IsTeamRed() ? CPKHPBar.PKBarType.Left : CPKHPBar.PKBarType.Right, serverPlayerData.name, PlayerInfoColor.PK_PLAYER);
            }
            else
            {
                if (ReplayServer.GetInstance().IsReplay())
                {
                    // TODO 这里的逻辑是录像选择一个玩家跟随
                    if (player.IsTeamRed() && player.playerInfo.seat == 0)
                    {
                        actor.isLocalActor = true;
                    }
                }

                actor.m_pkGeActor.CreatePKHPBar(player.IsTeamRed() ? CPKHPBar.PKBarType.Left : CPKHPBar.PKBarType.Right, serverPlayerData.name, PlayerInfoColor.PK_OTHER_PLAYER);
            }

            if (_isSameTeam(player))
            {
                actor.m_pkGeActor.AddTittleComponent(BattlePlayer.GetTitleID(serverPlayerData), serverPlayerData.name, 0, "",
                    serverPlayerData.level, (int)serverPlayerData.seasonLevel, PlayerInfoColor.PK_PLAYER);
                actor.m_pkGeActor.CreateInfoBar(serverPlayerData.name, PlayerInfoColor.PK_PLAYER, serverPlayerData.level);
            }
            else
            {
                actor.m_pkGeActor.AddTittleComponent(BattlePlayer.GetTitleID(serverPlayerData), serverPlayerData.name, 0, "",
                    serverPlayerData.level, (int)serverPlayerData.seasonLevel, PlayerInfoColor.PK_OTHER_PLAYER);
                actor.m_pkGeActor.CreateInfoBar(serverPlayerData.name, PlayerInfoColor.PK_OTHER_PLAYER, serverPlayerData.level);
                actor.m_pkGeActor.CreateFootIndicator();
            }

            return true;
        }

        private bool _isSameTeam(BattlePlayer player)
        {
            BattlePlayer mainplayer = mDungeonPlayers.GetMainPlayer();

            if (BattlePlayer.IsDataValidBattlePlayer(mainplayer))
            {
                if (mainplayer.teamType == player.teamType)
                {
                    return true;
                }
            }

            return false;
        }

        private bool _bindCharactorEvents(BattlePlayer player)
        {
            if (!_isValidCharacter(player))
            {
                return false;
            }

            BattlePlayer currentBattlePlayer = player;
            BeActor      currentActor        = currentBattlePlayer.playerActor;
#if !LOGIC_SERVER
            //if (ReplayServer.GetInstance().IsReplay())
            //{

            //}
            //else
            {
                //currentActor.RegisterEvent(BeEventType.onBeKilled, args =>{ _onMatchBattlePlayerDeadEvent(currentBattlePlayer); });
                currentActor.RegisterEventNew(BeEventType.onBeKilled, args => { _onMatchBattlePlayerDeadEvent(currentBattlePlayer); });
            }
#else
            currentActor.RegisterEventNew(BeEventType.onBeKilled, args =>{ _onMatchBattlePlayerDeadEvent(currentBattlePlayer); });
#endif

            return true;
        }

        private bool _unbindEventsForBattlePlayer(BattlePlayer player)
        {
            return true;
        }

        protected bool _killOneTeamPlayer(BattlePlayer.eDungeonPlayerTeamType type)
        {
            BattlePlayer teamPlayer = mDungeonPlayers.GetCurrentTeamFightingPlayer(type);

            if (!_isValidCharacter(teamPlayer))
            {
                return false;
            }

            if (!teamPlayer.isFighting)
            {
                Logger.LogProcessFormat("[战斗] 强制杀死 失败 {0} {1} 不在战斗中", teamPlayer.GetPlayerSeat(), teamPlayer.GetPlayerName());
                return false;
            }

            if (teamPlayer.playerActor.IsDead())
            {
                Logger.LogProcessFormat("[战斗] 强制杀死 失败 {0} {1} 已经死了", teamPlayer.GetPlayerSeat(), teamPlayer.GetPlayerName());
                return false;
            }

            Logger.LogProcessFormat("[战斗] 强制杀死 {0} {1}", teamPlayer.GetPlayerSeat(), teamPlayer.GetPlayerName());

            teamPlayer.playerActor.DoDead();

            _onMatchBattlePlayerDeadEvent(teamPlayer);

            return true;
        }

        private bool _isValidCharacter(BattlePlayer player)
        {
            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return false;
            }

            BeActor actor = player.playerActor;
            if (null == actor)
            {
                Logger.LogProcessFormat("[战斗] BattlePlayer 中的 playerActor 为空");
                return false;
            }

            return true;
        }

        private void _onMatchBattlePlayerDeadEvent(BattlePlayer player)
        {
            if (null != player)
            {
                Logger.LogProcessFormat("[战斗] 玩家终于死了 {0} {1}", player.GetPlayerSeat(), player.GetPlayerName());
            }

            if (null != player)
            {
                player.state      = BattlePlayer.EState.Dead;
            }

            
            _onMatchBattlePlayerDead(player);

            if (null != player)
            {
                player.isFighting     = false;
            }
        }

        protected virtual void _onMatchBattlePlayerDead(BattlePlayer player)
        {

        }

        protected virtual void _onMatchBattlePlayerReady2Fight()
        {

        }
    }
}




