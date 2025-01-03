using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
namespace GameClient
{
    public class FinalTestBattle : MouBattle
    {
        private uint bossID01;
        private uint bossHp01;
        private uint bossID02;
        private uint bossHp02;
        private int currentIndex = 0;
        protected bool alreadySendResult = false;
        ZjslDungeonBuff[] mPlayerBuffers = null;
        private int totalTime = 120000;
        private int countDown = 5000;
        public  UltimateChallengeDungeonTable tableData;
        public BossInfo bossInfo01 = null;
        public BossInfo bossInfo02 = null;
        public FinalPlayerInfo playerInfo = null;
        private List<BeEvent.BeEventHandleNew> m_HandleNewList = new List<BeEvent.BeEventHandleNew>();

        public FinalTestBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {
            tableData = TableManager.instance.GetFinalTestTime(id);
            if (tableData != null)
            {
                hpRecoverPercent = tableData.MaxhpRecover;
                mpRecoverPercent = tableData.MaxmpRecover;
                hpRecoverTotal = tableData.hpRecover;
                mpRecoverTotal = tableData.mpRecover;
                   totalTime = tableData.limitTime * 1000;
            }
        }

        protected override void _onPostStart()
        {
            base._onPostStart();
            _showCurrentTips();
          //  StartCountDownEffect();
        }



        public void SetBossInfo(uint bossID01, uint bossHp01, uint bossID02, uint bossHp02)
        {
            this.bossID01 = bossID01;
            this.bossHp01 = bossHp01;
            this.bossID02 = bossID02;
            this.bossHp02 = bossHp02;
            if (this.recordServer != null && recordServer.IsProcessRecord())
            {
                this.recordServer.RecordProcess(string.Format("BossID01{0} curHP01 {1},BossID02{2} curHP02 {3}", bossID01, bossHp01, bossID02, bossHp02));
                this.recordServer.MarkInt(0xa142441, (int)bossID01, (int)bossHp01, (int)bossID02, (int)bossHp02);
                // Mark:0xa142441 BossID01{0} curHP01 {1},BossID02{2} curHP02 {3}
            }
        }
        public void SetBuffInfo(ZjslDungeonBuff[] playerBuffs)
        {
            mPlayerBuffers = playerBuffs;
            if (this.recordServer != null && recordServer.IsProcessRecord())
            {
                string outputInfo = string.Empty;
                if (playerBuffs != null)
                {
                    for (int i = 0; i < playerBuffs.Length; i++)
                    {
                        outputInfo += string.Format("[buffId ({0}) lv ({1}) target({2})]", playerBuffs[i].buffId, playerBuffs[i].buffLvl,playerBuffs[i].buffTarget);
                    }
                }
                this.recordServer.RecordProcess(string.Format("PlayerBuffs {0}", outputInfo));
                this.recordServer.MarkString(0xb141442, outputInfo);
                // Mark:0xb141442 PlayerBuffs {0}
            }
        }

        private int hpRecover = 0;
        private int mpRecover = 0;
        private int hpRecoverTotal = 5;
        private int mpRecoverTotal = 5;
        private int hpRecoverPercent = 50;
        private int mpRecoverPercent = 50;
        protected override void _createPlayers()
        {
            base._createPlayers();

            var dungeonData = mDungeonManager.GetDungeonDataManager();
            var players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                var attribute = players[i].playerActor.GetEntityData();
                if (players[i].playerInfo.remainHp != 0)
                {
                    var remainHPRate = players[i].playerInfo.remainHp / (float)(GlobalLogic.VALUE_100000);
                    remainHPRate =  Mathf.Clamp01(remainHPRate);
                    float hp = remainHPRate * attribute.GetMaxHP();
                    attribute.SetHP(IntMath.Float2Int(hp));
                    players[i].playerActor.m_pkGeActor.SetHPValue(remainHPRate);
                }

                if (players[i].playerInfo.remainMp != 0)
                {
                    var remainMpRate = players[i].playerInfo.remainMp / (float)(GlobalLogic.VALUE_1000);
                    remainMpRate = Mathf.Clamp01(remainMpRate);
                    attribute.SetMP(IntMath.Float2Int(remainMpRate * attribute.GetMaxMP()));
                    players[i].playerActor.m_pkGeActor.SetMpValue(remainMpRate);
                }
                players[i].playerActor.SetPosition(dungeonData.CurrentBirthPosition());
                if (mPlayerBuffers != null && players[i].playerActor.buffController != null)
                {
                    for (int j = 0; j < mPlayerBuffers.Length; j++)
                    {
                        if (mPlayerBuffers[j] != null && mPlayerBuffers[j].buffTarget==1 && mPlayerBuffers[j].buffId!=0)
                            players[i].playerActor.buffController.TryAddBuff((int)mPlayerBuffers[j].buffId, 99999999, (int)mPlayerBuffers[j].buffLvl);
                    }
                }
                //堕落之塔只能有一个玩家进入 这边保存的数据也是针对一个玩家的  如果多个玩家进入会有问题  但是因为这块代码以前就有了  所以暂时不改
                RecordRecoverData(players[i].playerActor);
                m_HandleNewList.Add(players[i].playerActor.RegisterEventNew(BeEventType.onChangeEquipEnd, ChangeEquipEnd));
            }          
            mDungeonManager.GetBeScene().InitFriendActor(dungeonData.CurrentBirthPosition());
        }

        protected void ChangeEquipEnd(BeEvent.BeEventParam param)
        {
            var actor = param.m_Obj as BeActor;
            if (actor == null)
                return;
            RecordRecoverData(actor);
        }

        protected void RecordRecoverData(BeActor actor)
        {
            if (actor == null)
                return;
            hpRecover = actor.attribute.battleData.fHpRecoer;
            mpRecover = actor.attribute.battleData.fMpRecover;
            actor.attribute.SetAttributeValue(AttributeType.hpRecover, 0, false);
            actor.GetEntityData().battleData.hpRecover = actor.GetEntityData().battleData.fHpRecoer;
            actor.attribute.SetAttributeValue(AttributeType.mpRecover, 0, false);
            actor.GetEntityData().battleData.mpRecover = actor.GetEntityData().battleData.fMpRecover;
        }

        private void _showCurrentTips()
        {
#if !LOGIC_SERVER
            SystemNotifyManager.SysDungeonAnimation(
                string.Format("{0}", mDungeonManager.GetDungeonDataManager().table.Name )
            );
            SetFloorName();
#endif
        }
        void SetFloorName()
        {
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDeadTower>();
            if (battleUI != null && tableData!=null)
                battleUI.SetFloor(tableData.ID);
        }

       
        public override void FrameUpdate(int delta)
        {
            base.FrameUpdate(delta);
            if (mDungeonManager == null || mDungeonManager.GetBeScene() == null) return;
           // CountDown(delta);
            if (startCountDown)
            {
                if (alreadySendResult || mDungeonManager.IsFinishFight() || mDungeonManager.GetBeScene().state == BeSceneState.onPause)
                {
                    return;
                }
                UpdateTotalTime(delta);
                if (timeStop)
                {
                    _onTimeUp();
                }
            }
            else
            {
                UpdateTotalTime(0);
            }
        }

        private int timer = 0;
        private bool flag = true;
        private bool startCountDown = true;
        private void CountDown(int delta)
        {
            if (!flag)
            {
                timer += delta;
                if (timer >= countDown)
                {
                    startCountDown = true;
                    flag = true;
                    timer = 0;
                    SetAIState(false);
                    HideSettingButton(false);
                }
            }
        }

        GoblinKingdomFrame uiFrame = null;
        private void OpenFrame()
        {
            uiFrame = ClientSystemManager.instance.OpenFrame<GoblinKingdomFrame>() as GoblinKingdomFrame;
            if (uiFrame != null)
            {
                uiFrame.SetRoom();

            }
        }
        private bool timeStop = false;
        void UpdateTotalTime(int deltaTime)
        {
            totalTime -= deltaTime;
            if (totalTime > 0)
            {
                if (uiFrame != null)
                {
                    int m = totalTime / 60000;
                    int s = (totalTime % 60000) / 1000;
                    int ms = (totalTime % 1000) / 10;
                    string text = string.Format("{0}.{1:D2}.{2:D2}", m, s, ms);
                    if (uiFrame != null)
                        uiFrame.SetTime(text);
                }
            }
            else
            {
                string text = string.Format("{0}.{1:D2}.{2:D2}", 0, 0, 0);
                if (uiFrame != null)
                    uiFrame.SetTime(text);
                timeStop = true;
            }

        }
        protected void _onTimeUp()
        {
            if (alreadySendResult)
                return;

            bool isAllEnemyDead = mDungeonManager.GetBeScene()._isAllBossDead();
            if (!isAllEnemyDead)
            {
                RaceFail();
            }
        }

        protected override void _onEnd()
        {
            base._onEnd();
            RemoveHandle();
        }

        private void RaceFail()
        {
            alreadySendResult = true;
            mDungeonManager.FinishFight();

#if !LOGIC_SERVER
            SetBossInfo();
            GameFrameWork.instance.StartCoroutine(_failEnd());
#endif
#if LOGIC_SERVER
            var req = getDungeonRaceEndTeamReq(false);
            LogicServer.ReportRaceEndToLogicServer(req);
#endif
        }
        private void SetBossInfo()
        {
            var beDungeon = mDungeonManager as BeDungeon;
            if (beDungeon != null)
            {
                if (beDungeon.bossID01 != 0)
                {                  
                    BeActor monster = beDungeon.GetBeScene().FindMonsterByID((int)beDungeon.bossID01);
                    if (monster != null)
                    {
                        bossInfo01 = new BossInfo();
                        SetBossInfo(bossInfo01, monster);
                    }
                }
                if (beDungeon.bossID02 != 0)
                {                  
                    BeActor monster = beDungeon.GetBeScene().FindMonsterByID((int)beDungeon.bossID02);
                    if (monster != null)
                    {
                        bossInfo02 = new BossInfo();
                        SetBossInfo(bossInfo02, monster);
                    }
                }
            }
        }
        private void SetBossInfo(BossInfo bossInfo,BeActor monster)
        {
            if (monster != null)
            {
                GeActorEx.GeActorDesc m_ActorDesc = monster.m_pkGeActor.m_ActorDesc;
                var entityHeadIconAsset = AssetLoader.instance.LoadRes(m_ActorDesc.portraitIconRes, typeof(Sprite));
                var entityHeadIcon = entityHeadIconAsset.obj as Sprite;
                bossInfo.icon = entityHeadIcon;
                var entityHeadIconMaterial = ETCImageLoader.LoadMaterialFromSpritePath(m_ActorDesc.portraitIconRes);
                bossInfo.material = entityHeadIconMaterial;
                bossInfo.level = monster.GetEntityData().level;
                bossInfo.name = monster.GetEntityData().name;
                bossInfo.hpRate = monster.attribute.GetHPRate().single;
            }
        }

        protected  RelaySvrDungeonRaceEndReq getDungeonRaceEndTeamReq(bool isSuccess)
        {
            RelaySvrDungeonRaceEndReq msg = new RelaySvrDungeonRaceEndReq();
            //msg.roleId = PlayerBaseData.GetInstance().RoleID;
#if LOGIC_SERVER
            msg.raceEndInfo.sessionId = recordServer != null ? Convert.ToUInt64(recordServer.sessionID) : 0UL;
#else
            msg.raceEndInfo.sessionId = ClientApplication.playerinfo.session;
#endif
            msg.raceEndInfo.dungeonId = (uint)mDungeonManager.GetDungeonDataManager().id.dungeonID;
            msg.raceEndInfo.usedTime = (uint)mDungeonStatistics.AllFightTime(true);

            var m_battlePlayers = mDungeonPlayers.GetAllPlayers();

            msg.raceEndInfo.infoes = new DungeonPlayerRaceEndInfo[m_battlePlayers.Count];
            var beDungeon = mDungeonManager as BeDungeon;
            uint _bossHp01 = bossHp01;
            if (beDungeon != null)
            {
                _bossHp01 = bossHp01 - beDungeon.bossDamage01;
                if (beDungeon.bossDamage01 >= bossHp01) _bossHp01 = 0;
            }
            
            uint _bossHp02 = bossHp02;
            if (beDungeon != null)
            {
                _bossHp02 = bossHp02 - beDungeon.bossDamage02;
                if (beDungeon.bossDamage02 >= bossHp02) _bossHp02 = 0;
            }
            
            for (int i = 0; i < m_battlePlayers.Count; ++i)
            {
                RacePlayerInfo source = m_battlePlayers[i].playerInfo;
                DungeonPlayerRaceEndInfo target = new DungeonPlayerRaceEndInfo
                {
                    roleId = source.roleId,
                    pos = source.seat,
                    score = (byte)mDungeonStatistics.FinalDungeonScore(),
                    beHitCount = (ushort)mDungeonStatistics.HitCount(source.seat),
                    bossDamage = mDungeonStatistics.GetBossDamage(source.seat),
                    boss1ID = bossID01,
                    boss1RemainHp = beDungeon != null ? _bossHp01 : 0,
                    boss2ID = beDungeon != null ? beDungeon.bossID02 : 0,
                    boss2RemainHp = _bossHp02,
                    playerRemainHp = m_battlePlayers[i].playerActor != null ? GetHpRate(m_battlePlayers[i].playerActor) : 0,
                    playerRemainMp = m_battlePlayers[i].playerActor != null ? GetMpRate(m_battlePlayers[i].playerActor) : 0,
                };
                if (!isSuccess) target.score = (byte)DungeonScore.C;
                target.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", target.score, target.beHitCount, msg.raceEndInfo.usedTime));
                msg.raceEndInfo.infoes[i] = target;
            }

            return msg;
        }

        protected override void _createMonsters()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var beDungeon = mDungeonManager as BeDungeon;
            if (beDungeon == null) return ;
            var monsters = mDungeonData.CurrentMonsters();
            int monsterCreatedCount = 0;
            monsterCreatedCount = mBeScene.CreateMonsterList(monsters, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition());

            for (int i = 0; i < mBeScene.GetPendingEntities().Count; i++)
            {
                BeEntity entity = mBeScene.GetPendingEntities()[i];
                if (entity.IsBoss())
                {
                    int hp = 0;
                    uint monsterID = (uint)entity.GetEntityData().monsterID;
                    if (bossID01 != 0 && bossID01==monsterID)
                    {
                        beDungeon.bossID01 = monsterID;
                        hp = (int)bossHp01;
                    }
                    else if (bossID02 != 0 && bossID02 == monsterID)
                    {
                        beDungeon.bossID02 = monsterID;
                        hp = (int)bossHp02;
                    }
                    else  if (bossID01 == 0 && beDungeon.bossID01 ==0)
                    {
                            beDungeon.bossID01 = monsterID;
                            hp = entity.attribute.GetMaxHP();
                            bossHp01 = (uint)hp;
                    }
                    else if (bossID02 == 0 && beDungeon.bossID02 == 0)
                    {
                        beDungeon.bossID02 = monsterID;
                        hp = entity.attribute.GetMaxHP();
                        bossHp02 = (uint)hp;
                    }
                    if (hp != 0)
                    {
                        entity.attribute.SetHP(hp);
#if !LOGIC_SERVER
                        if (entity.m_pkGeActor != null)
                        {
                            entity.m_pkGeActor.isSyncHPMP = true;
                            entity.m_pkGeActor.SyncHPBar();
                            entity.m_pkGeActor.isSyncHPMP = false;
                        }
#endif
                    }
                    else
                    {
                        entity.DoDead();
                    }
                    BeActor boss = entity as BeActor;
                    if (boss !=null && boss.buffController != null && mPlayerBuffers!=null)
                    {
                        for (int j = 0; j < mPlayerBuffers.Length; j++)
                        {
                            if (mPlayerBuffers[j] != null && mPlayerBuffers[j].buffTarget == 2)
                                boss.buffController.TryAddBuff((int)mPlayerBuffers[j].buffId, 99999999, (int)mPlayerBuffers[j].buffLvl);
                        }
                    }
                }
            }
            mBeScene.RegisterEventNew(BeEventSceneType.onBossDead, (args) => 
            {
                OnBossDead();
            });

            this.thisRoomMonsterCreatedCount = monsterCreatedCount;
        }

        private void OnBossDead()
        {
            if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
            {
                List<BattlePlayer> list =  mDungeonPlayers.GetAllPlayers();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].playerActor.isMainActor)
                    {
                        list[i].playerActor.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA,-1);
                    }
                }
                if (mDungeonManager.IsFinishFight()) return;
                if (mDungeonManager.GetDungeonDataManager().IsBossArea())
                {

                    //召唤师觉醒通关血量问题
                    List<BeEntity> tempList = mDungeonManager.GetBeScene().GetSaveTempList();
                    if (tempList.Count > 0)
                    {
                        BeActor actor = tempList[0] as BeActor;
                        if (actor != null && list.Count>0)
                        {
                            list[0].playerActor = actor;
                        }
                    }
#if !LOGIC_SERVER                   
                    SetPlayerInfo();
                    _sendDungeonKillMonsterReq();
                    _sendDungeonRewardReq();
                    _sendDungeonBossRewardReq();
#endif
                    mDungeonManager.FinishFight();

#if !LOGIC_SERVER
                    GameFrameWork.instance.StartCoroutine(_successEnd());
#endif
#if LOGIC_SERVER
            var req = getDungeonRaceEndTeamReq(true);
            LogicServer.ReportRaceEndToLogicServer(req);
#endif
                }
            }           
        }

        private void SetPlayerInfo()
        {
            BeActor actor = BattleMain.instance.GetLocalPlayer().playerActor;
            playerInfo = new FinalPlayerInfo();
            if (hpRecover <= 0) hpRecover = 0;
            playerInfo.addHp = hpRecover*hpRecoverTotal + (int)(hpRecoverPercent / 1000.0f * actor.GetEntityData().GetMaxHP());
            if (mpRecover <= 0) mpRecover = 0;
            playerInfo.addMp = mpRecover * mpRecoverTotal + (int)(mpRecoverPercent / 1000.0f * actor.GetEntityData().GetMaxMP());
            playerInfo.hp = actor.GetEntityData().GetHP();
            playerInfo.mp = actor.GetEntityData().GetMP();
            playerInfo.maxHp = actor.GetEntityData().GetMaxHP();
            playerInfo.maxMp = actor.GetEntityData().GetMaxMP();
            GeActorEx.GeActorDesc m_ActorDesc = actor.m_pkGeActor.m_ActorDesc;
            var entityHeadIconAsset = AssetLoader.instance.LoadRes(m_ActorDesc.portraitIconRes, typeof(Sprite));
            var entityHeadIcon = entityHeadIconAsset.obj as Sprite;
            playerInfo.icon = entityHeadIcon;
            var entityHeadIconMaterial = ETCImageLoader.LoadMaterialFromSpritePath(m_ActorDesc.portraitIconRes);
            playerInfo.material = entityHeadIconMaterial;
            playerInfo.level = actor.GetEntityData().level;
            playerInfo.name = actor.GetEntityData().name;
        }
        protected override void _onGameStartFrame(BattlePlayer player)
        {
            base._onGameStartFrame(player);
#if !LOGIC_SERVER
            OpenFrame();
#endif                    
          //  StartCountDown();
        }

        public void StartCountDown()
        {
            if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
            {
                SetAIState(true);
                flag = false;
            }
        }

        private void SetAIState(bool pauseAI)
        {
            List<BeEntity> entityList = new List<BeEntity>();
            if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
            {
                mDungeonManager.GetBeScene().GetEntitys2(entityList);
                for (int i = 0; i < entityList.Count; i++)
                {
                    BeActor actor = entityList[i] as BeActor;
                    if (actor != null && !actor .isMainActor)
                    {
                        actor.pauseAI = pauseAI;
                    }
                       
                }
            }
        }

        private void HideSettingButton(bool hide)
        {
#if !SERVER_LOGIC
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPauseBtn>();
            if (battleUI != null)
            {
                battleUI.HidePauseButton(hide);
            }
            if (blackMask != null)
                GameObject.Destroy(blackMask);
#endif
        }

        GameObject blackMask = null;
        public void StartCountDownEffect()
        {
#if !SERVER_LOGIC
            HideSettingButton(true);
            blackMask = GameObject.Instantiate(LeanTween.instance.frameBlackMask);
            if (blackMask != null)
            {
                UnityEngine.UI.Image image = blackMask.GetComponent<UnityEngine.UI.Image>();
                if (image != null)
                {
                    image.color = new Color(0, 0, 0, 0.2f);
                }
                Utility.AttachTo(blackMask, ClientSystemManager.instance.TopLayer);
            }
            var obj = ClientSystemManager.instance.PlayUIEffect(FrameLayer.Top, "UIFlatten/Prefabs/Pk/StartFight");
#endif
        }

        protected override void _onSceneStart()
        {
            base._onSceneStart();
            mDungeonManager.GetBeScene().isUseBossShow = false;
        }


        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
            if (mDungeonManager.IsFinishFight()) return;
            base._onAreaClear(args);
        }

        protected override SceneDungeonRaceEndReq _getDungeonRaceEndReq()
        {
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            if (mainPlayer == null) return null;
            var beDungeon = mDungeonManager as BeDungeon;
            if (beDungeon == null) return null;
            uint _bossHp01 = bossHp01 - beDungeon.bossDamage01;
            if (beDungeon.bossDamage01 >= bossHp01) _bossHp01 = 0;
            uint _bossHp02 = bossHp02 - beDungeon.bossDamage02;
            if (beDungeon.bossDamage02 >= bossHp02) _bossHp02 = 0;
            var msg = new SceneDungeonRaceEndReq
            {
                beHitCount = (ushort)mDungeonStatistics.HitCount(mainPlayer.playerInfo.seat),
                usedTime = (uint)mDungeonStatistics.AllFightTime(true),
                score = (byte)mDungeonStatistics.FinalDungeonScore(),
                maxDamage = mDungeonStatistics.GetAllMaxHurtDamage(),
                skillId = mDungeonStatistics.GetAllMaxHurtSkillID(),
                param = mDungeonStatistics.GetAllMaxHurtID(),
                totalDamage = mDungeonStatistics.GetAllHurtDamage(),
                playerRemainHp = GetHpRate(mainPlayer.playerActor),
                playerRemainMp = GetMpRate(mainPlayer.playerActor),
                lastFrame = mDungeonManager.GetDungeonDataManager().GetFinalFrameDataIndex(),
                boss1ID = beDungeon.bossID01,
                boss1RemainHp = _bossHp01,
                boss2ID = beDungeon.bossID02,
                boss2RemainHp = _bossHp02

            };
            if (timeStop)
                msg.score = (byte)DungeonScore.C;
            msg.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", msg.score, msg.beHitCount, msg.usedTime));
            return msg;
        }

        private uint GetHpRate(BeActor actor)
        {
            if (actor.GetEntityData().GetHP() <= 0) return 0;
            int recover = hpRecover * hpRecoverTotal;
            recover = recover <= 0 ? 0 : recover;
            int hp = actor.GetEntityData().GetHP() + recover +  (int)(hpRecoverPercent/1000.0f* actor.GetEntityData().GetMaxHP());            
            int maxHp = actor.GetEntityData().GetMaxHP();
            float rate = (float)hp / (float)maxHp;            
            return (uint)((rate* GlobalLogic.VALUE_100000));
        }

        private uint GetMpRate(BeActor actor)
        {
            int recover = mpRecover * mpRecoverTotal;
            recover = recover <= 0 ? 0 : recover;
            int mp = actor.GetEntityData().GetMP() + recover + (int)(mpRecoverPercent / 1000.0f * actor.GetEntityData().GetMaxMP());
            int maxMp = actor.GetEntityData().GetMaxMP();
            float rate = (float)mp / (float)maxMp;
            return (uint)((rate * GlobalLogic.VALUE_1000));
        }       

        protected override void _onPlayerCancelReborn(BattlePlayer player)
        {
         
        }

        protected override void _onPlayerDead(BattlePlayer player)
        {
            if (mDungeonManager == null || mDungeonManager.GetBeScene() == null) return;
            if (alreadySendResult || mDungeonManager.IsFinishFight())
            {
                return;
            }
            RaceFail();
        }

        protected void RemoveHandle()
        {
            for(int i=0;i< m_HandleNewList.Count; i++)
            {
                if (m_HandleNewList[i] != null)
                {
                    m_HandleNewList[i].Remove();
                    m_HandleNewList[i] = null;
                }
            }
        }

    }
}