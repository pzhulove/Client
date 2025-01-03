using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    public class GuildPVEBattle:PVEBattle
    {
        int mInitBossHP = 1000;
        DungeonBuff[] mPlayerBuffers = null;
        GuildDungeonLvlTable mDungeonLvRecord = null;
        bool mIsCriticalElementDestroyed = false;
        private UnityEngine.Coroutine mDeadProcess = null;
        private bool mIsSyncSuccess = false; //是否联动的情况下胜利的
        public class BossInfo
        {
            public UInt64 curHP = 100;
            public UInt64 maxHP = 100;
            public string iconPath = string.Empty;
            public string bossName = string.Empty;
        }
#if !LOGIC_SERVER
        private BossInfo mBossInfo = new BossInfo();
#endif
        public GuildDungeonLvlTable ValidTable
        {
            get { return mDungeonLvRecord; }
        }
        public GuildPVEBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {
            mDungeonLvRecord = null;
 			mIsCriticalElementDestroyed = false;
            Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildDungeonLvlTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    GuildDungeonLvlTable adt = iter.Current.Value as GuildDungeonLvlTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.DungeonId != id)
                    {
                        continue;
                    }
                    mDungeonLvRecord = adt;
                    break;
                }
            }
           
        }
        public void SetBossInfo(UInt64 curHP,UInt64 maxHP)
        {
            mInitBossHP = (int)curHP;
#if !LOGIC_SERVER
            mBossInfo.curHP = curHP;
            mBossInfo.maxHP = maxHP;
#endif
            if (this.recordServer != null)
            {
                this.recordServer.RecordProcess(string.Format("Boss curHP {0}", curHP));
                this.recordServer.MarkString(0xb141440, curHP.ToString());
                // Mark:0xb141440 Boss curHP  {0}

            }
        }
        public void SetBuffInfo(DungeonBuff[] playerBuffs)
        {
            mPlayerBuffers = playerBuffs;
            if (this.recordServer != null && recordServer.IsProcessRecord())
            {
                string outputInfo = string.Empty;
                if (playerBuffs != null)
                {
                    for (int i = 0; i < playerBuffs.Length; i++)
                    {
                        outputInfo += string.Format("[buffId ({0}) lv ({1})]", playerBuffs[i].buffId, playerBuffs[i].buffLvl);
                    }
                }
                this.recordServer.RecordProcess(string.Format("PlayerBuffs {0}", outputInfo));
                this.recordServer.MarkString(0xb141480, outputInfo);
                // Mark:0xb141480 PlayerBuffs {0}
            }
        }
        protected override void _createPlayers()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var players = mDungeonPlayers.GetAllPlayers();


            #region get pos
            VInt3 bornPos = mDungeonData.GetBirthPosition();
            VInt3[] poses = new VInt3[5];
            poses[0] = bornPos;

            int count = 1;

            for (int i = 1; i <= players.Count - 1; ++i)
            {
                var tmp = bornPos;
                tmp.x += BeAIManager.DIR_VALUE2[i - 1, 0] * VInt.one.i;
                tmp.y += BeAIManager.DIR_VALUE2[i - 1, 1] * VInt.one.i;

                if (!mBeScene.IsInBlockPlayer(tmp))
                {
                    poses[count++] = tmp;
                }
            }

            for (int i = count; i <= players.Count - 1; ++i)
                poses[i] = bornPos;
            #endregion

            for (int i = 0; i < players.Count; ++i)
            {
                var battlePlayer = players[i];



                if (battlePlayer.playerActor == null)
                {
                    var racePlayer = battlePlayer.playerInfo;

                    var petData = BattlePlayer.GetPetData(racePlayer,false);

                    bool isLocalActor = racePlayer.accid == ClientApplication.playerinfo.accid;
                    bool isShowFashionWeapon = racePlayer.avatar.isShoWeapon == 1 ? true : false;
                    bool isAIRobot = racePlayer.robotAIType > 0 ? true : false;

                    var actor = mBeScene.CreateCharacter(
                        isLocalActor,
                        racePlayer.occupation,
                        racePlayer.level,
                        (int)ProtoTable.UnitTable.eCamp.C_HERO,
                        BattlePlayer.GetSkillInfo(racePlayer),
                        BattlePlayer.GetEquips(racePlayer,false),
                        BattlePlayer.GetBuffList(racePlayer),
                        racePlayer.seat,
                        racePlayer.name,
                        BattlePlayer.GetWeaponStrengthenLevel(racePlayer),
                        BattlePlayer.GetRankBuff(racePlayer),
                        petData,
                        BattlePlayer.GetSideEquips(racePlayer,false),
                        BattlePlayer.GetAvatar(racePlayer),
                        isShowFashionWeapon,
                        isAIRobot
                    );

                    actor.InitChangeEquipData(racePlayer.equips, racePlayer.equipScheme);
                    actor.SetScale(VInt.Float2VIntValue(Global.Settings.charScale));
                    if (actor.GetEntityData() != null)
                        actor.GetEntityData().SetCrystalNum(BattlePlayer.GetCrsytalNum(racePlayer));

                    battlePlayer.playerActor = actor;

                    actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(racePlayer);
                    actor.SetPosition(mDungeonData.GetBirthPosition(), true);
                    actor.isMainActor = true;
                    actor.UseProtect();
                    actor.m_iRemoveTime = Int32.MaxValue;
                    if(mPlayerBuffers != null && actor.buffController != null)
                    {
                        for(int j = 0; j < mPlayerBuffers.Length;j++)
                        {
                            if(mPlayerBuffers[j] != null )
                                actor.buffController.TryAddBuff((int)mPlayerBuffers[j].buffId,99999999, (int)mPlayerBuffers[j].buffLvl);
                        }
                    }

                    if (null != actor.m_pkGeActor)
                    {
                        actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
                    }

#if !LOGIC_SERVER
                    actor.m_pkGeActor.AddTittleComponent(BattlePlayer.GetTitleID(racePlayer), racePlayer.name, 0, "", racePlayer.level, (int)racePlayer.seasonLevel, PlayerInfoColor.LEVEL_PLAYER);
                    if (racePlayer.accid == ClientApplication.playerinfo.accid)
                        actor.m_pkGeActor.CreateInfoBar(racePlayer.name, PlayerInfoColor.TOWN_PLAYER, racePlayer.level);
                    else
                        actor.m_pkGeActor.CreateInfoBar(racePlayer.name, PlayerInfoColor.LEVEL_PLAYER, racePlayer.level);
#endif


                    if (racePlayer.accid == ClientApplication.playerinfo.accid)
                    {
#if !LOGIC_SERVER
                        mDungeonManager.GetGeScene().AttachCameraTo(actor.m_pkGeActor);
#endif
                        //初始化伤害统计数据
                        actor.skillDamageManager.InitData(mBeScene);

                        actor.isLocalActor = true;
                        actor.UseActorData();
                        actor.m_pkGeActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo");
                    }
                    else
                    {

#if !LOGIC_SERVER
                        //画质设置：低配不显示友军特效
                        if (GeGraphicSetting.instance.IsLowLevel())
                        {
                            var effectManager = actor.m_pkGeActor.GetEffectManager();
                            if (effectManager != null)
                            {
                                effectManager.useCube = true;
                            }
                        }
#endif
                    }
                    if (!FunctionIsOpen(BattleFlagType.SceneOnReadyStartPetAI))
                    {
                        mBeScene.RegisterEventNew(BeEventSceneType.AfterOnReady, args =>
                        {
                            if (actor.pet != null && actor.pet.GetEntityData() != null && actor.pet.GetEntityData().isPet && actor.pet.aiManager != null)
                            {
                                actor.pet.aiManager.Stop();
                            }
                        });
                    }
                    // TODO battle Player Dead
                    actor.RegisterEventNew(BeEventType.onAfterDead, arsg =>
                    {
                        if (battlePlayer.state != BattlePlayer.EState.Dead)
                        {
                            _onPlayerDead(battlePlayer);
                        }


#if !LOGIC_SERVER
                        GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.PLAYER_DEAD,
                                mDungeonManager.GetDungeonDataManager().id.dungeonID,
                                mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                                string.Format("{0}, {1}", battlePlayer.playerInfo.roleId, battlePlayer.statistics.data.deadCount));
#endif

                    });

                    actor.RegisterEventNew(BeEventType.onDead, arsg =>
                    {
                        if (battlePlayer.state != BattlePlayer.EState.Dead)
                        {
#if !LOGIC_SERVER && MG_TEST
                            RecordServer.instance.PushReconnectCmd(string.Format("BeEventType.onDead {0}", battlePlayer.playerActor.GetPID()));
#endif
                            if (!CanReborn()) return;
                            int rebornCount = 5;
                            if (mDungeonManager != null && mDungeonManager.GetDungeonDataManager() != null && mDungeonManager.GetDungeonDataManager().table != null)
                            {
                                rebornCount = mDungeonManager.GetDungeonDataManager().table.RebornCount;
                            }
                            else
                            {
                                Logger.LogErrorFormat("GuildPVERebornCount can not fetched!");
                            }
                            bool isAllPlayerDead = true;
                            int loopIndex = 0;
                            for (loopIndex = 0; loopIndex < players.Count; loopIndex++)
                            {
                                var player = players[loopIndex];
                                if (!player.playerActor.IsDead())
                                {
                                    isAllPlayerDead = false;
                                    break;
                                }
                            }
                            bool needCheck = true;
                            if (isAllPlayerDead)
                            {
                                for (loopIndex = 0; loopIndex < players.Count; loopIndex++)
                                {
                                    var player = players[loopIndex];
                                    if (player.playerActor.dungeonRebornCount < rebornCount)
                                    {
                                        player.playerActor.StartDeadProtect();
                                        needCheck = false;
                                    }
                                }

                                if (needCheck)
                                {
                                    _CheckGuildFightEnd();
                                }
                            }
                        }
                    });

                    actor.RegisterEventNew(BeEventType.onDeadProtectEnd, args =>
                    {
                        _CheckGuildFightEnd();
                    });

                    actor.RegisterEventNew(BeEventType.onReborn, args =>
                    {
                        _onPlayerGuildReborn(battlePlayer);


#if !LOGIC_SERVER
                        GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.PLAYER_REBORN,
                                mDungeonManager.GetDungeonDataManager().id.dungeonID,
                                mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                                string.Format("{0}, {1}", battlePlayer.playerInfo.roleId, battlePlayer.statistics.data.rebornCount));
#endif

                    });

                    actor.RegisterEventNew(BeEventType.onHit, args => {
                        //actor.RegisterEvent(BeEventType.onHit, args => {
                        if (args != null)
                        {
                            var e = args.m_Obj as BeEntity;
                            if (e != null && e.m_iCamp != battlePlayer.playerActor.m_iCamp)
                            {
                                _onPlayerHit(battlePlayer);
                            }
                        }
                        else
                        {
                            _onPlayerHit(battlePlayer);
                        }
                    });

                    actor.RegisterEventNew(BeEventType.onCastSkill, args =>
                    {
                        int skillId = args.m_Int;
                        _onPlayerUseSkill(battlePlayer, skillId);
                    });

                    SetAccompanyInfo(battlePlayer);

                    if (petData != null)
                        actor.SetPetData(petData);
                    actor.CreateFollowMonster();

                    //actor.InitAutoFight();
                    InitAutoFight(actor);
                    ChangeActorAttribute(actor);
                    //actor.forceRunMode = false;
                    actor.SetForceRunMode(false);
#if DEBUG_SETTING
                    if (Global.Settings.isDebug)
                    {
                        if (Global.Settings.playerHP > 0)
                        {
                            //actor.GetEntityData().battleData.hp = Global.Settings.playerHP;
                            //actor.GetEntityData().battleData.maxHp = Global.Settings.playerHP;
                            actor.GetEntityData().SetHP(Global.Settings.playerHP);
                            actor.GetEntityData().SetMaxHP(Global.Settings.playerHP);
                            actor.m_pkGeActor.ResetHPBar();
                        }
                    }
#endif
                }
                else
                {
                    // set transport birth position

                    battlePlayer.playerActor.ResetMoveCmd();
                    if (battlePlayer.playerActor.actionManager != null)
                        battlePlayer.playerActor.actionManager.StopAll();

                }

                battlePlayer.playerActor.SetPosition(poses[i], true);
                mBeScene.InitFriendActor(mDungeonData.GetBirthPosition());
            }
        }
        protected override void _createMonsters()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var monsters = mDungeonData.CurrentMonsters();
            BeActor boss = null;
            int monsterCreatedCount = 0;
            if (mDungeonLvRecord != null)
            {
                if(mDungeonLvRecord.dungeonLvl > 1)
                {
                    monsterCreatedCount = mBeScene.CreateMonsterList(monsters, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition(), ref boss);
                }
                else
                {
                    monsterCreatedCount = mBeScene.CreateMonsterList(monsters, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition());
                }
            }
            else
            {
                Logger.LogErrorFormat("DungeonId {0} can't find Record", mDungeonData.id.dungeonID);
            }
           
            if(boss != null)
            {
                boss.attribute.SetHP(mInitBossHP);
                if (this.recordServer != null && recordServer.IsProcessRecord())
                {
                    this.recordServer.RecordProcess(string.Format("SetBossHP {0} {1}", mInitBossHP, boss.GetInfo()));
                    recordServer.Mark(0xb88788, new int[] {mInitBossHP,
                        boss.m_iID,
                        boss.GetPosition().x,
                        boss. GetPosition().y,
                        boss.GetPosition().z,
                        boss.moveXSpeed.i,
                        boss.moveYSpeed.i,
                        boss.moveZSpeed.i,
                        boss.GetFace() ? 1 : 0,
                        boss.attribute.GetHP(),
                        boss.attribute.GetMP(),
                        boss.GetAllStatTag(),
                        boss.attribute.battleData.attack.ToInt()},boss.GetName());
                    // Mark:0xb88788 SetBossHP {0} [PID:{1} name:{13} pos:({2},{3},{4}) speed:({5},{6},{7}) face:{8} hp:{9} mp:{10} attack:{12} tag:{11}]
                }
#if !LOGIC_SERVER
                if (boss.m_pkGeActor != null)
                {
                    boss.m_pkGeActor.isSyncHPMP = true;
                    boss.m_pkGeActor.SyncHPBar();
                    boss.m_pkGeActor.isSyncHPMP = false;
                }
#endif
            }
            this.thisRoomMonsterCreatedCount = monsterCreatedCount;
        }
        public override void OnCriticalElementDisappear()
        {
            mIsCriticalElementDestroyed = true;
            _DoFightEnd(false);
        }
        public override bool CanReborn()
        {
            if (mDungeonLvRecord != null && mDungeonLvRecord.dungeonLvl > 1) return true;
            return false;
        }
        protected void _onPlayerGuildReborn(BattlePlayer player)
        {
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]mid:{0} player reborn", player.playerActor.m_iID);
                recordServer.MarkInt(0x1779600, player.playerActor.m_iID);
                // Mark:0x1779600 [BATTLE]mid:{0} player reborn
            }
            var players = mDungeonPlayers.GetAllPlayers();
            for (int loopIndex = 0; loopIndex < players.Count; loopIndex++)
            {
                var curPlayer = players[loopIndex];
                curPlayer.playerActor.EndDeadProtect();
            }
#if !LOGIC_SERVER
            byte seat = player.playerInfo.seat;
            byte mainPlayerSeat = mDungeonPlayers.GetMainPlayer().playerInfo.seat;

            if (BattleMain.IsModeMultiplayer(GetMode()) && seat == mainPlayerSeat)
            {
                mDungeonManager.GetGeScene().AttachCameraTo(player.playerActor.m_pkGeActor);
            }
#endif

            player.state = BattlePlayer.EState.Normal;
            player.statistics.Reborn();


#if !LOGIC_SERVER

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornSuccess, player.playerInfo.seat);

            if (mainPlayerSeat == seat)
            {
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }
            }
#endif
            _CheckGuildFightEnd();
        }
        private void _CheckGuildFightEnd()
        {
            if (mDungeonManager == null || mDungeonPlayers == null || mDungeonManager.GetDungeonDataManager() == null)
                return;
            if (mDungeonManager.IsFinishFight()) return;
            List<BattlePlayer> players = mDungeonPlayers.GetAllPlayers();
            bool isAllPlayerDead = true;
            bool isAllEnemyDead = mDungeonManager.GetBeScene().isAllEnemyDead();
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (!player.playerActor.IsDead() ||
                    player.playerActor.IsInDeadProtect)
                {
                    isAllPlayerDead = false;
                }
            }
#if !LOGIC_SERVER && MG_TEST
            RecordServer.instance.PushReconnectCmd(string.Format("_CheckGuildFightEnd isAllEnemyDead : {0} isAllPlayerDead: {1} IsBossArea :{2}", isAllEnemyDead, isAllPlayerDead, mDungeonManager.GetDungeonDataManager().IsBossArea()));
#endif
            if (isAllEnemyDead && mDungeonManager.GetDungeonDataManager().IsBossArea())
            {

                if (!isAllPlayerDead)
                {
                    _DoFightEnd(true);
                }
                else
                {
                    _DoFightEnd(false);
                }
            }
            else if (isAllPlayerDead)
            {
                _DoFightEnd(false);
            }
        }
        protected override void _onPlayerDead(BattlePlayer player)
        {
            if (player != null)
            {
                if (recordServer != null && recordServer.IsProcessRecord())
                {
                    recordServer.RecordProcess("[BATTLE]mid:{0} player dead", player.playerActor.m_iID);
                    recordServer.MarkInt(0x1779500, player.playerActor.m_iID);
                    // Mark:0x1779500 [BATTLE]mid:{0} player dead
                }

                _playDungeonDead();

                player.state = BattlePlayer.EState.Dead;
                player.statistics.Dead();
                byte seat = player.playerInfo.seat;
                bool canReborn = CanReborn();
                if (canReborn)
                {
                    player.playerActor.buffController.TryAddBuff((int)GlobalBuff.DEAD_PROTECT, GlobalLogic.VALUE_10000);
                }

#if !LOGIC_SERVER
                byte mainPlayerSeat = mDungeonPlayers.GetMainPlayer().playerInfo.seat;
                if (seat == mainPlayerSeat)
                {
                    if (BattleMain.IsModeMultiplayer(GetMode()))
                    {
                        BattlePlayer alivePlayer = mDungeonPlayers.GetFirstAlivePlayer();
                        if (null != alivePlayer)
                        {
                            mDungeonManager.GetGeScene().AttachCameraTo(alivePlayer.playerActor.m_pkGeActor);
                        }
                    }
                }
                if (canReborn)
                {
                    if (mDungeonPlayers.IsAllPlayerDead() || seat == mainPlayerSeat)
                    {
                        _startPlayerDeadProcess(player);
                    }
                }
#endif
           
                if (!canReborn && mDungeonPlayers.IsAllPlayerDead())
                {
                    _DoFightEnd(false);
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattlePlayerDead);
        }
        private void _startPlayerDeadProcess(BattlePlayer player)
        {
#if !LOGIC_SERVER
            _stopPlayerDeadProcess();

            if (!mDungeonManager.IsFinishFight())
            {
                mDeadProcess = GameFrameWork.instance.StartCoroutine(_playerDeadProcess(player));
            }
#endif
        }
        private void _stopPlayerDeadProcess()
        {
#if !LOGIC_SERVER
            if (null != mDeadProcess)
            {
                GameFrameWork.instance.StopCoroutine(mDeadProcess);
                mDeadProcess = null;
            }
#endif
        }
        private IEnumerator _playerDeadProcess(BattlePlayer player)
        {
#if !LOGIC_SERVER
            if (player != null && player.IsLocalPlayer())
            {
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }

                ClientSystemManager.instance.OpenFrame<DungeonRebornFrame>(FrameLayer.Middle);
            }

            yield return Yielders.EndOfFrame;
#endif

            while (DungeonRebornFrame.sState == DungeonRebornFrame.eState.None)
            {
                yield return Yielders.EndOfFrame;
            }

            if (DungeonRebornFrame.sState == DungeonRebornFrame.eState.Cancel)
            {

                _onPlayerCancelGuildReborn(player);
#if !LOGIC_SERVER
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }
#endif
            }
        }
        protected virtual void _onPlayerCancelGuildReborn(BattlePlayer player)
        {
            if (player != null)
            {
                player.playerActor.EndDeadProtect();
            }
            _CheckGuildFightEnd();
        }

        public bool isNormalLevel() //是否普通关卡
        {
            if (mDungeonLvRecord != null)
            {
                return mDungeonLvRecord.dungeonLvl <= 1;
            }
            return true;
        }
        protected sealed override void _onStart()
        {
            base._onStart();
#if !LOGIC_SERVER
            if (!isNormalLevel())
            {
                if (mDungeonLvRecord != null)
                {
                    var bossTable = TableManager.instance.GetTableItem<UnitTable>(mDungeonLvRecord.bossId);
                    if (bossTable != null)
                    {
                        mBossInfo.bossName = bossTable.Name;
                        var resTable = TableManager.instance.GetTableItem<ResTable>(bossTable.Mode);
                        if (resTable != null)
                        {
                            mBossInfo.iconPath = resTable.IconPath;
                        }

                    }
                }
                ClientSystemManager.instance.OpenFrame<GuildPVEBattleFrame>(FrameLayer.Middle,mBossInfo);
            }
#endif
        }
        private void _DoFightEnd(bool isSuccess, bool isSync = false)
        {
            if (mDungeonManager == null || mDungeonPlayers == null || mDungeonManager.GetDungeonDataManager() == null)
                return;
            if (mDungeonManager.IsFinishFight()) return;


            if (mDungeonLvRecord != null)
            {
                int hard = mDungeonLvRecord.dungeonLvl;
                switch (hard)
                {
                    case 1:
                        break;
                    case 2:
                    case 3:
                        {
                            if (!isSuccess)
                            {
                                var m_battlePlayers = mDungeonPlayers.GetAllPlayers();
                                uint bossDamage = 0;
                                for (int i = 0; i < m_battlePlayers.Count; ++i)
                                {
                                    RacePlayerInfo source = m_battlePlayers[i].playerInfo;
                                    bossDamage = mDungeonStatistics.GetBossDamage(source.seat);
                                    if (bossDamage > 0)
                                    {
                                        isSuccess = true;
                                        break;
                                    }

                                }
                            }
                        }
                        break;
                }
            }

            if (isSuccess)
            {
                if(isSync)
                    mIsSyncSuccess = true;
#if !LOGIC_SERVER
                _sendDungeonKillMonsterReq();
                _sendDungeonRewardReq();
                _sendDungeonBossRewardReq();
                _sendDungeonRaceEndReqByGuild();
#else
                var req = _getDungeonRaceEndTeamReq();
                LogicServer.ReportRaceEndToLogicServer(req);
#endif
            }
            else
            {
#if !LOGIC_SERVER
                _sendDungeonRaceEndReqByGuild();
#else
                var req = _getDungeonRaceEndTeamReq();
                LogicServer.ReportRaceEndToLogicServer(req);
#endif
            }
#if !LOGIC_SERVER && MG_TEST
            RecordServer.instance.PushReconnectCmd(string.Format("_DoFightEnd isSuccess : {0}", isSuccess));
#endif
            mDungeonManager.FinishFight();
        }
        protected void _sendDungeonRaceEndReqByGuild()
        {
#if !LOGIC_SERVER
            if (_isNeedSendNet())
            {
                if (eDungeonMode.SyncFrame == GetMode())
                {
                    GameFrameWork.instance.StartCoroutine(_sendDungeonTeamRaceEndReqIterByGuild());
                }
                else
                {
                    GameFrameWork.instance.StartCoroutine(_sendDungeonRaceEndReqIterByGuild());
                }
            }
            else
            {
                SceneDungeonRaceEndRes res = new SceneDungeonRaceEndRes
                {
                    result = 0
                };
                _onSceneDungeonRaceEndRes(res);
            }

            ClearBgm();

            _playDungeonFinish();
#endif
        }
        private IEnumerator _sendDungeonTeamRaceEndReqIterByGuild(bool modifyScore = false, Protocol.DungeonScore score = Protocol.DungeonScore.C)
        {
#if !LOGIC_SERVER
            var msgEvent = new MessageEvents();
            var res = new SceneDungeonRaceEndRes();
            var req = _getDungeonRaceEndTeamReq();

            if (modifyScore)
            {
                for (int i = 0; i < req.raceEndInfo.infoes.Length; ++i)
                {
                    req.raceEndInfo.infoes[i].score = (byte)score;
                }
            }

            BattleMain.instance.WaitForResult();

            yield return MessageUtility.Wait<RelaySvrDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.RELAY_SERVER, msgEvent, req, res, false);

            if (msgEvent.IsAllMessageReceived())
            {
                _onSceneDungeonRaceEndRes(res);
            }
#else
            yield break;
#endif
        }
        private IEnumerator _sendDungeonRaceEndReqIterByGuild(bool modifyScore = false, Protocol.DungeonScore score = Protocol.DungeonScore.C)
        {
            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            var msgEvent = new MessageEvents();
            var res = new SceneDungeonRaceEndRes();
            var req = _getDungeonRaceEndReqByGuild();

            if (modifyScore)
                req.score = (byte)score;

            yield return _sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msgEvent, req, res, true, 3, 3, 33);

            //收到结算
            if (msgEvent.IsAllMessageReceived())
            {
                _onSceneDungeonRaceEndRes(res);
            }
        }

        protected SceneDungeonRaceEndReq _getDungeonRaceEndReqByGuild()
        {
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            var msg = new SceneDungeonRaceEndReq
            {
                beHitCount = (ushort)mDungeonStatistics.HitCount(mainPlayer.playerInfo.seat),
                usedTime = (uint)mDungeonStatistics.AllFightTime(true),
                score = (byte)GetFinalDungeonScore(),
                maxDamage = mDungeonStatistics.GetAllMaxHurtDamage(),
                skillId = mDungeonStatistics.GetAllMaxHurtSkillID(),
                param = mDungeonStatistics.GetAllMaxHurtID(),
                totalDamage = mDungeonStatistics.GetAllHurtDamage(),
                lastFrame = mDungeonManager.GetDungeonDataManager().GetFinalFrameDataIndex(),
                bossDamage = mainPlayer != null ? 
                                        mDungeonStatistics.GetBossDamage(mainPlayer.GetPlayerSeat()) : 
                                        mDungeonStatistics.GetTotalBossDamage()
            };
            msg.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", msg.score, msg.beHitCount, msg.usedTime));
            return msg;
        }
        private RelaySvrDungeonRaceEndReq _getDungeonRaceEndTeamReq()
        {

            RelaySvrDungeonRaceEndReq msg = new RelaySvrDungeonRaceEndReq();
#if !LOGIC_SERVER
            msg.raceEndInfo.sessionId = ClientApplication.playerinfo.session;
#else
            msg.raceEndInfo.sessionId = this.recordServer != null ? Convert.ToUInt64(recordServer.sessionID) : 0UL;
#endif
            msg.raceEndInfo.dungeonId = (uint)mDungeonManager.GetDungeonDataManager().id.dungeonID;
            msg.raceEndInfo.usedTime = (uint)mDungeonStatistics.AllFightTime(true);

            var m_battlePlayers = mDungeonPlayers.GetAllPlayers();

            msg.raceEndInfo.infoes = new DungeonPlayerRaceEndInfo[m_battlePlayers.Count];

            for (int i = 0; i < m_battlePlayers.Count; ++i)
            {
                RacePlayerInfo source = m_battlePlayers[i].playerInfo;
                DungeonPlayerRaceEndInfo target = new DungeonPlayerRaceEndInfo
                {
                    roleId = source.roleId,
                    pos = source.seat,
                    score = (byte)GetFinalDungeonScore(),
                    beHitCount = (ushort)mDungeonStatistics.HitCount(source.seat),
                    bossDamage = mDungeonStatistics.GetBossDamage(source.seat)

                };
                target.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", target.score, target.beHitCount, msg.raceEndInfo.usedTime));
                msg.raceEndInfo.infoes[i] = target;
            }

            return msg;
        }

        public DungeonScore GetFinalDungeonScore()
        {
            if(mDungeonLvRecord != null)
            {
                if(mDungeonStatistics == null)
                {
                    Logger.LogErrorFormat("GetFinalDungeonScore mDungeonStatistics is null");
                    return DungeonScore.C;
                }
                if (mDungeonLvRecord.dungeonLvl > 1)
                {
                    if (mDungeonStatistics.GetTotalBossDamage() >= mDungeonLvRecord.threeStarDamage)
                    {
                        return DungeonScore.SSS;
                    }
                    if (mDungeonStatistics.GetTotalBossDamage() >= mDungeonLvRecord.twoStarDamage)
                    {
                        return DungeonScore.SS;
                    }
                    if (mDungeonStatistics.GetTotalBossDamage() >= mDungeonLvRecord.oneStarDamage)
                    {
                        return DungeonScore.S;
                    }
                    if (mDungeonStatistics.GetTotalBossDamage() > 0)
                    {
                        return DungeonScore.A;
                    }
                }
                else if(!mIsCriticalElementDestroyed)
                {
                    return mDungeonStatistics.FinalDungeonScore();
                }
            }
            if (mIsSyncSuccess) return DungeonScore.A;
            return DungeonScore.C;
        }

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
            if (battleUI != null)
            {
                battleUI.CloseLevelTip();
            }
#endif

            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[SCENE]_onAreaClear");
                recordServer.Mark(0xb2457246);
                // Mark:0xb2457246 [SCENE]_onAreaClear
            }

            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                _DoFightEnd(true);
            }
            else
            {
#if !LOGIC_SERVER
                if (this.thisRoomMonsterCreatedCount > 0)
                {
                    SystemNotifyManager.SystemNotify(6000);
                    PlaySound(5);
                }

      

                var index = mDungeonManager.GetDungeonDataManager().CurrentIndex();
                areaIndex = (uint)(1 << index) | areaIndex;
                _updateDungeonState(true);
#endif
            }
#if !LOGIC_SERVER
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DisplayMissionTips);
#endif
        }

        protected sealed override void _onPlayerLeave(BattlePlayer player)
        {
            if (player != null)
            {
                player.netState = BattlePlayer.eNetState.Offline;

                if (recordServer != null && recordServer.IsProcessRecord())
                {
                    recordServer.RecordProcess("[BATTLE]PID:{0} playerName:{1} _onPlayerLeave", player.playerActor.m_iID, player.playerInfo.name);
                    recordServer.Mark(0x1779511, new int[] { player.playerActor.m_iID }, player.playerInfo.name);
                    // Mark:0x1779511 [BATTLE]PID:{0} playerName:{1} _onPlayerLeave
                }
            }

            if (mDungeonPlayers.IsAllPlayerDead())
            {
                _CheckGuildFightEnd();
            }
        }
        protected override void _onTeamCopyRaceEnd()
        {
            if (mDungeonManager != null && !mDungeonManager.IsFinishFight())
            {
                 _DoFightEnd(true,true);
                if (mDungeonManager.GetBeScene() != null)
                {
                    mDungeonManager.GetBeScene().ClearAllMonsters();
                }
            }

        }

        [MessageHandle(WorldGuildDungeonEndNotify.MsgID)]
        void _OnReceiveBattleEndData(MsgDATA msg)
        {
#if !LOGIC_SERVER
            if(dungeonManager == null || 
               dungeonManager.GetDungeonDataManager() == null ||
               dungeonManager.GetDungeonDataManager().id == null)
            {
                return;
            }
            WorldGuildDungeonEndNotify pkBattleEndData = new WorldGuildDungeonEndNotify();
            pkBattleEndData.decode(msg.bytes);
            if(pkBattleEndData.dungeonId == (uint)dungeonManager.GetDungeonDataManager().id.dungeonID)
            {
                TeamCopyRaceEnd cmd = new TeamCopyRaceEnd();
                FrameSync.instance.FireFrameCommand(cmd, true);
            }
#endif
        }
        [MessageHandle(WorldGuildDungeonBossOddBlood.MsgID)]
        void _OnReceiveBossBloodData(MsgDATA msg)
        {
#if !LOGIC_SERVER
            WorldGuildDungeonBossOddBlood pkBossBloodData = new WorldGuildDungeonBossOddBlood();
            pkBossBloodData.decode(msg.bytes);
            mBossInfo.curHP = pkBossBloodData.bossOddBlood;
            mBossInfo.maxHP = pkBossBloodData.bossTotalBlood;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildBossHPRefresh, mBossInfo);
#endif
        }
    }

}
