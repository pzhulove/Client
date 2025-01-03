using Network;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class PVPScuffleBattle : PVPBattle
    {
        public PVPScuffleBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        { }

        protected override void _createPlayers()
        {
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

                bool isLocalActor = current.accid == ClientApplication.playerinfo.accid;
                int camp = (int)currentBattlePlayer.teamType;
                bool isShowFashionWeapon = current.avatar.isShoWeapon == 1 ? true : false;
                bool isAIRobot = current.robotAIType > 0 ? true : false;

                var actor = main.CreateCharacter(
                    isLocalActor,
                    current.occupation,
                    current.level,
                    camp,
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

                actor.InitChangeEquipData(current.equips, current.equipScheme);
                if (actor == null || actor.m_pkGeActor == null) return;
                if (actor.GetEntityData() != null)
                    actor.GetEntityData().SetCrystalNum(BattlePlayer.GetCrsytalNum(current));

                actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(current);
                players[i].playerActor = actor;

                actor.SetFace(camp != 0);
                actor.m_iRemoveTime = GlobalLogic.VALUE_1000 * 240;

                #region 空气墙

                actor.stateController.SetAbilityEnable(BeAbilityType.ATTACK, false);
                actor.pkRestrainPosition = true;
                if (camp == 0)
                {

                    actor.pkRestrainRangeX = new VInt2(-6f, -3.6f);
                }
                else
                {
                    actor.pkRestrainRangeX = new VInt2(3.6f, 6f);
                }
                #endregion
                main.RegisterEventNew(BeEventSceneType.onStartPK, (args) =>
                {
                    actor.stateController.SetAbilityEnable(BeAbilityType.ATTACK, true);
                });

                actor.RegisterEventNew(BeEventType.onBeKilled, args => { _onPlayerDead(currentBattlePlayer); });
                //actor.RegisterEvent(BeEventType.onBeKilled, args => { _onPlayerDead(currentBattlePlayer); });
                actor.RegisterEventNew(BeEventType.onKill, args => { OnKill(currentBattlePlayer, args); });
                //actor.RegisterEvent(BeEventType.onKill, args => { OnKill(currentBattlePlayer, args); });
                actor.RegisterEventNew(BeEventType.onSummon, args => { OnSummon(currentBattlePlayer, args); });
                PlayerInfoColor playerInfoColor = camp == 1 ? PlayerInfoColor.PK_PLAYER : PlayerInfoColor.PK_OTHER_PLAYER;
                actor.m_pkGeActor.AddTittleComponent(BattlePlayer.GetTitleID(current), current.name, 0, "", current.level, (int)current.seasonLevel, playerInfoColor);
                actor.m_pkGeActor.CreateInfoBar(current.name, playerInfoColor, current.level);
                actor.m_pkGeActor.CreatePKHPBar(camp == 0 ? CPKHPBar.PKBarType.Left : CPKHPBar.PKBarType.Right, current.name, playerInfoColor);
                if (current.accid == ClientApplication.playerinfo.accid)
                {
                    actor.isLocalActor = true;
                }
                else
                {
                    if (ReplayServer.GetInstance().IsReplay())
                    {
                        if (i == 0)
                            actor.isLocalActor = true;
                    }
                }
                actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
                if (petData != null)
                    actor.SetPetData(petData);
                actor.CreateFollowMonster();

                actor.isMainActor = true;

                actor.UseProtect();

                actor.UseActorData();
            }

            for (int i = 0; i < players.Count; ++i)
            {
#if !LOGIC_SERVER
                if (players[i].teamType == BattlePlayer.eDungeonPlayerTeamType.eTeamRed)
                {
                    players[i].playerActor.m_pkGeActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_hong");
                }
                else
                {
                    players[i].playerActor.m_pkGeActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_lan");
                }
#endif
                players[i].playerActor.UseAdjustBalance();
                players[i].playerActor.GetEntityData().AdjustHPForScufflePVP(
                    players[i].playerInfo.level,
                    players[i].playerInfo.occupation
                    );
            }

            _postCreatePlayer();
            SetActorPos(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
            SetActorPos(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
#if !LOGIC_SERVER
            mDungeonManager.GetGeScene().AttachCameraTo(mDungeonPlayers.GetMainPlayer().playerActor.m_pkGeActor);
#endif
        }

        private void SetActorPos(BattlePlayer.eDungeonPlayerTeamType type)
        {
            List<BattlePlayer> list = mDungeonPlayers.GetAllPlayers().FindAll(x => { return x.GetPlayerCamp() == (int)type; });
            switch (list.Count)
            {
                case 1:
                    for (int i = 0; i < list.Count; i++)
                    {
                        var pos = new VInt3(type == BattlePlayer.eDungeonPlayerTeamType.eTeamRed ? -4 : 4, 0.0f, 0);

                        list[i].playerActor.SetPosition(pos);
                    }
                    break;
                case 2:

                    for (int i = 0; i < list.Count; i++)
                    {
                        var pos = new VInt3(type == BattlePlayer.eDungeonPlayerTeamType.eTeamRed ? -4 : 4, -2 + 4 * i, 0.0f);

                        list[i].playerActor.SetPosition(pos);
                    }
                    break;
                case 3:
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i <= 1)
                        {
                            var pos = new VInt3(type == BattlePlayer.eDungeonPlayerTeamType.eTeamRed ? -4 : 4, -2 + 4 * i, 0.0f);

                            list[i].playerActor.SetPosition(pos);
                        }
                        else
                        {
                            var pos = new VInt3(type == BattlePlayer.eDungeonPlayerTeamType.eTeamRed ? -6 : 6, 0, 0.0f);

                            list[i].playerActor.SetPosition(pos);
                        }
                    }
                    break;
            }
        }

        protected override void _onEnd()
        {
            _unbindNetMessage();
#if !LOGIC_SERVER

            List<BattlePlayer> list = mDungeonPlayers.GetAllPlayers();
            string job = "";
            for (int i = 0; i < list.Count; i++)
            {
                BeActor actor = list[i].playerActor;
                if (actor != null)
                {
                    job += actor.GetName() + ";";
                }
            }
            GameStatisticManager.instance.DoStatFinishMeleeBattle(job);
#endif
        }

        private void _bindNetMessage()
        {
            Logger.LogProcessFormat("[战斗] [3v3] 开始绑定 消息");
            NetProcess.AddMsgHandler(SceneRoomMatchPkRaceEnd.MsgID, _onSceneRoomMatchPkRaceEnd);
        }

        private void _unbindNetMessage()
        {
            Logger.LogProcessFormat("[战斗] [3v3] 解除绑定 消息");
            NetProcess.RemoveMsgHandler(SceneRoomMatchPkRaceEnd.MsgID, _onSceneRoomMatchPkRaceEnd);
        }

        /// <summary> 
        /// 结算消息中的数据来之后，更新界面数据，并显示玩家自己的积分变化
        /// </summary>
        private void _onSceneRoomMatchPkRaceEnd(MsgDATA data)
        {
            SceneRoomMatchPkRaceEnd res = new SceneRoomMatchPkRaceEnd();
            res.decode(data.bytes);
            _convert2BattlePlayer(res);
            _openDungeon3v3FinishFrame(res);
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3GetRaceEndResult);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK2V2CrossScoreGetRaceEndResult);
        }

        private void _convert2BattlePlayer(SceneRoomMatchPkRaceEnd res)
        {
            if (null == res)
            {
                Logger.LogErrorFormat("[战斗] [3v3] raceEnd 为空");
                return;
            }

            for (int i = 0; i < res.slotInfos.Length; ++i)
            {
                RoomSlotBattleEndInfo cur = res.slotInfos[i];

                if (null == cur)
                {
                    continue;
                }

                BattlePlayer player = mDungeonPlayers.GetPlayerBySeat(cur.seat);

                if (null == player)
                {
                    continue;
                }

                if (player.IsLocalPlayer())
                {
                    player.ConvertSceneRoomMatchPkRaceEnd2LocalBattlePlayer(res);
                }

                player.ConvertRoomSlotBattleEndInfo2BattlePlayer(cur);
            }
        }

        private void _openDungeon3v3FinishFrame(SceneRoomMatchPkRaceEnd res)
        {
#if !LOGIC_SERVER
            var activeSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;

            if (activeSystem != null)
            {
                if (ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_RACE_ID_CHECK))
                {
                    if (ClientApplication.playerinfo != null)
                    {
                        if (res.raceId != 0 && res.raceId == ClientApplication.playerinfo.session)
                        {
                            ClientSystemManager.instance.OpenFrame<Dungeon3v3FinishFrame>();
                        }
                    }
                }
                else
                {
                    ClientSystemManager.instance.OpenFrame<Dungeon3v3FinishFrame>();
                }
            }    
#endif
        }

        private enum ePVP3V3ProcessStatus
        {
            /// <summary>
            /// 加载
            /// </summary>
            None,
            /// <summary>
            /// 等待投票VS出现
            /// </summary>
            WaitFirstVoteVSFlag,
            /// <summary>
            /// 开始战斗
            ///
            /// 播放3，2，1倒计时，
            ///
            /// 限制玩家移动
            /// </summary>
            RoundStart,
        }

        private ePVP3V3ProcessStatus mCurPVP3V3ProcessStatus = ePVP3V3ProcessStatus.None;
        private int mTickTime = 0;

        private ePVP3V3ProcessStatus curPVP3V3ProcessStatus
        {
            get
            {
                return mCurPVP3V3ProcessStatus;
            }

            set
            {
                Logger.LogProcessFormat("[战斗] [3v3] 状态 {0} -> {1}", mCurPVP3V3ProcessStatus, value);
                mCurPVP3V3ProcessStatus = value;
            }
        }

        /// <summary>
        /// 计算时间
        /// </summary>
        private bool _isTimeUp(int delta)
        {
            mTickTime -= delta;

            return !_isTickTimeUp();
        }

        private bool _isTickTimeUp()
        {
            return mTickTime <= 0;
        }

        BattlePlayer mPlayer = null;

        protected override void _onGameStartFrame(BattlePlayer player)
        {
            mPlayer = player;
            mTickTime = 2000;

            curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.WaitFirstVoteVSFlag;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3FinishVoteForFight);
        }

        public override void FrameUpdate(int delta)
        {
            base.FrameUpdate(delta);
            if (mDungeonManager == null || mDungeonManager.IsFinishFight()) return;
            if (alreadySendResult)
            {
                return;
            }
            if (_isTimeUp(delta) || IsTimeUp()) return;
            switch (curPVP3V3ProcessStatus)
            {
                case ePVP3V3ProcessStatus.None:
                    break;

                case ePVP3V3ProcessStatus.WaitFirstVoteVSFlag:
                    if (!_isTimeUp(delta))
                    {
                        mTickTime = 0;
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.RoundStart;
                    }
                    break;

                case ePVP3V3ProcessStatus.RoundStart:
                    curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.None;
                    base._onGameStartFrame(mPlayer);
                    break;
            }
        }

        protected override void _onStart()
        {
            base._onStart();
            _bindNetMessage();
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDungeonMap>();

            if (null != battleUI)
            {
                battleUI.SetDungeonMapActive(false);
            }
#endif
        }

        protected override void _onPlayerDead(BattlePlayer player)
        {
            if (alreadySendResult)
                return;
            if(mDungeonManager == null || mDungeonPlayers==null)
            {
                return;
            }
            if (mDungeonManager.IsFinishFight()) return;

            if (player.playerActor != null)
            {
                player.playerActor.ClearProtect();
            }

            if (mDungeonPlayers.IsTeamPlayerAllDead(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue) ||
                mDungeonPlayers.IsTeamPlayerAllDead(BattlePlayer.eDungeonPlayerTeamType.eTeamRed))
            {
                mDungeonManager.FinishFight();
#if !LOGIC_SERVER

                if (!ReplayServer.GetInstance().IsReplay())
                {
                    _saveResult(ePVPBattleEndType.onAllEnemyDied);
                    if (mfinishBattle != null)
                    {
                        return;
                        //GameFrameWork.instance.StopCoroutine(mfinishBattle);
                    }
                    mfinishBattle = GameFrameWork.instance.StartCoroutine(_FinishBattle(ePVPBattleEndType.onAllEnemyDied));
                }
#else
                _LogicServerSendResult(ePVPBattleEndType.onAllEnemyDied);
#endif
            }
            if (player != null && player.playerActor != null)
                player.playerActor.SetAttackButtonState(ButtonState.RELEASE);
#if !LOGIC_SERVER
            List<BattlePlayer> list = BattleMain.instance.GetPlayerManager().GetAllPlayers().FindAll(x => { return x.GetPlayerCamp() == player.GetPlayerCamp() && !x.playerActor.IsDead(); });
            if (player.IsLocalPlayer())
            {
                if (list.Count > 0)
                    mDungeonManager.GetGeScene().AttachCameraTo(list[0].playerActor.m_pkGeActor);
                _unloadInputManger();
            }
#endif

        }
        private void _LogicServerSendResult(ePVPBattleEndType a_eEndType)
        {
            if (savedReq == null)
                _saveResult(a_eEndType);

            if (savedReq != null)
                LogicServer.ReportPkRaceEndToLogicServer(savedReq);

            alreadySendResult = true;
        }

        protected override void _saveResult(ePVPBattleEndType a_eEndType)
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
            bool isTeamBlueLost = mDungeonPlayers.IsTeamPlayerAllDead(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
            bool isTeamRedLost = mDungeonPlayers.IsTeamPlayerAllDead(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i] != null && players[i].playerInfo != null)
                {
                    var seat = players[i].playerInfo.seat;
                    bool isRed = players[i].IsTeamRed();
                    
                    var tmp = new PkPlayerRaceEndInfo
                    {
                        roleId = players[i].playerInfo.roleId,
                        pos = seat,
                        remainHp = (UInt32)(Mathf.Max(0f, players[i].playerActor.GetEntityData().GetHPRate().single) * 1000),
                        remainMp = (UInt32)(Mathf.Max(0f, players[i].playerActor.GetEntityData().GetMPRate().single) * 1000),
                    };
                    if(a_eEndType == ePVPBattleEndType.onTimeOut)
                    {
                        tmp.result = (byte)PKResult.LOSE;
                    }
                    else
                    {
                        if (isTeamBlueLost)
                        {
                            if(isRed)
                                tmp.result = (byte)PKResult.WIN;
                            else
                                tmp.result = (byte)PKResult.LOSE;

                        }
                        else
                        {
                            if (isRed)
                                tmp.result = (byte)PKResult.LOSE;
                            else
                                tmp.result = (byte)PKResult.WIN;
                        }
                    }

                    //!! 这里是什么意思

                    playerInfos[i] = tmp;
                }
            }
            info.infoes = playerInfos;

            info.replayScore = _getVideoScore(playerInfos);

            req.end = info;

            savedReq = req;
        }
        protected override void _onTimeUp()
        {
            if (alreadySendResult)
                return;
            mEndType = ePVPBattleEndType.onTimeOut;
#if !LOGIC_SERVER
            if (!ReplayServer.GetInstance().IsReplay())
            {
                if (mfinishBattle != null)
                {
                    return;
                }
                mfinishBattle = GameFrameWork.instance.StartCoroutine(_FinishBattle(ePVPBattleEndType.onTimeOut));
            }
#else
            _LogicServerSendResult(ePVPBattleEndType.onTimeOut);
#endif
        }
        private void OnKill(BattlePlayer player, BeEvent.BeEventParam param)
        {
            BeEntity entity = param.m_Obj as BeEntity;
            if (entity != null)
            {
                BeActor actor = entity as BeActor;
                if (actor != null && actor.isMainActor)
                    player.statistics.killPlayers++;
            }

        }

        private void OnSummon(BattlePlayer player, BeEvent.BeEventParam args)
        {
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null)
            {
                if (actor.GetEntityData().isSummonMonster)
                {
                    actor.RegisterEventNew(BeEventType.onKill, obj =>
                    //actor.RegisterEvent(BeEventType.onKill, obj =>
                    {
                        BeEntity entity = obj.m_Obj as BeEntity;
                        if (entity != null)
                        {
                            BeActor beActor = entity as BeActor;
                            if (beActor != null && beActor.isMainActor)
                                player.statistics.killPlayers++;
                        }
                    });
                }
            }

        }

        protected override PKResult _getPKResulte(byte seat, ePVPBattleEndType a_eEndType)
        {
            var player = mDungeonPlayers.GetPlayerBySeat(seat);

            if (a_eEndType == ePVPBattleEndType.onTimeOut)
            {
                return PKResult.LOSE;
                // return GetRaceEndResult(player.teamType);

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

        protected override void _ShowResultEffect(ePVPBattleEndType a_eEndType)
        {
#if !LOGIC_SERVER
            var activeSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
            if (activeSystem != null)
            {
                ClientSystemManager.instance.OpenFrame<Dungeon3v3FinishFrame>();
            }
#endif
        }


        public PKResult GetRaceEndResult(BattlePlayer.eDungeonPlayerTeamType type)
        {
            if (mEndType == ePVPBattleEndType.onTimeOut)
            {
                return PKResult.LOSE;
            }

            if (mDungeonPlayers.IsTeamPlayerAllDead(type) && !mDungeonPlayers.IsTeamPlayerAllDead(BattlePlayer.GetTargetTeamType(type)))
            {
                return PKResult.LOSE;
            }

            if (!mDungeonPlayers.IsTeamPlayerAllDead(type) && mDungeonPlayers.IsTeamPlayerAllDead(BattlePlayer.GetTargetTeamType(type)))
            {
                return PKResult.WIN;
            }
            return PKResult.DRAW;
        }

    }




}