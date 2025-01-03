using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

using Network;
using Protocol;
using DG.Tweening;
using ProtoTable;

namespace GameClient
{
    public class BaseNewbieGuideBattle : PVEBattle
    {
        public BaseNewbieGuideBattle(BattleType type, eDungeonMode mode, int id) : base(type, eDungeonMode.Test, id)
        {
            FrameRandom.ResetSeed(0x1314555);
        }

        private IEnumerator _save()
        {
            yield break;
            //if (Global.Settings.startSystem == EClientSystem.Login)
            //{
            //    SceneNotifyNewBoot req = new SceneNotifyNewBoot();
            //    req.id = (UInt32)eNewbieGuideTask.FirstFight;
            //    NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);

            //    int cnt = 0;
            //    while (cnt < 10 && !Utility.IsNewbieGuideFinish(eNewbieGuideTask.FirstFight))
            //    {
            //        yield return Yielders.GetWaitForSeconds(1);
            //        cnt++;
            //    }

            //    if (!Utility.IsNewbieGuideFinish(eNewbieGuideTask.FirstFight))
            //    {
            //        Logger.LogError("first fight save failed");
            //    }
            //}
        }


        private IEnumerator _closeFrame(Type type)
        {
            while (!ClientSystemManager.instance.IsFrameOpen(type))
            {
                yield return Yielders.EndOfFrame;
            }

			ClientSystemManager.instance.CloseFrameByType(type, true);
        }

        private UnityEngine.Coroutine mCoroutine = null;

        AnimationCurve skillButtonShow = new AnimationCurve(
            new Keyframe[]
            {
                new Keyframe(0.0f,0.0f),
                new Keyframe(0.75f,1.5f),
                new Keyframe(1.0f,1.0f)
            }
        );

        protected IEnumerator _GuideShowSkill(GameObject obj, float timelen)
        {
            timelen = 0.15f;
            /* */
            /*float time = 0;
            var rect = obj.transform.GetComponent<RectTransform>();
            time += Time.deltaTime;

            while (time <= timelen)
            {
                float t = time / timelen;
                t = skillButtonShow.Evaluate(t);
                rect.localScale = new Vector3(t, t, t);

                yield return Yielders.EndOfFrame;
                time += Time.deltaTime;
            }

            rect.localScale = Vector3.one;
            */
            var rect = obj.transform.GetComponent<RectTransform>();
            DOTween.To(()=>rect.localScale,(value)=>{rect.localScale = value;},Vector3.one,timelen).SetEase(skillButtonShow);
            yield return Yielders.GetWaitForSeconds(timelen);
        }

        int step = 0;
        protected IEnumerator _DoStateNewbieGuideBegin()
        {

#if !LOGIC_SERVER
            step = 0;
            if (mDungeonPlayers == null) yield break;
            BattlePlayer player = mDungeonPlayers.GetMainPlayer();
            if (player != null)
            {
                var actor = mDungeonPlayers.GetMainPlayer().playerActor;
                if (actor != null)
                {
                    if (actor.m_pkGeActor != null && actor.m_pkGeActor.titleComponent != null)
                    {
                        actor.m_pkGeActor.titleComponent.SetPKEnable(false);
                    }
                    int initBuffID = 1112;
                    if (initBuffID > 0)
                    {
                        actor.buffController.TryAddBuff(initBuffID, -1);
                    }

                }
            }
#endif
            yield break;
        }
        protected IEnumerator _DoStateNewbieGuideIter(string key)
        {
            _DoStateNewbieGuideFunc(key);
             yield break;
        }
        protected void _DoStateNewbieGuideFunc(string key)
        {
             GameStatisticManager.GetInstance().DoStatNewBieGuide(key);
             step++;
        }

        protected override void _onStartResourceLoaded()
        {
            return;
/*
            var mainPlayer = mDungeonPlayers.GetMainPlayer();

            GeActorEx geActor = mainPlayer.playerActor.m_pkGeActor;
            ActorOccupation newJob = (ActorOccupation)PlayerBaseData.GetInstance().JobTableID;

            switch (newJob)
            {
                case ActorOccupation.SwordMan:
                    {
#if LOGIC_SERVER
                        if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Swordman/Swordman_Kuangzhan", mainPlayer.playerActor.m_cpkEntityInfo,
                                mDungeonManager.GetBeScene().ActionFrameMgr, mDungeonManager.GetBeScene().SkillFileCache);
                        }
                        else
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Swordman/Swordman_Kuangzhan", mainPlayer.playerActor.m_cpkEntityInfo,null,null);
                        }
#else
                         geActor.LoadOneSkillConfig("Data/SkillData/Swordman/Swordman_Kuangzhan", mainPlayer.playerActor.m_cpkEntityInfo);
#endif
                    }
                    break;
                case ActorOccupation.MagicMan:
                    {
#if LOGIC_SERVER
                        if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Mage/Mage_zhaohuan", mainPlayer.playerActor.m_cpkEntityInfo, 
                                mDungeonManager.GetBeScene().ActionFrameMgr, mDungeonManager.GetBeScene().SkillFileCache);
                        }
                        else
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Mage/Mage_zhaohuan", mainPlayer.playerActor.m_cpkEntityInfo, null,null);
                        }
#else
                        geActor.LoadOneSkillConfig("Data/SkillData/Mage/Mage_zhaohuan", mainPlayer.playerActor.m_cpkEntityInfo);
#endif
                    }
                    break;
                case ActorOccupation.Gungirl:
                    {
#if LOGIC_SERVER
                        if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Gungirl/Gungirl_Manyou", mainPlayer.playerActor.m_cpkEntityInfo,
                                mDungeonManager.GetBeScene().ActionFrameMgr, mDungeonManager.GetBeScene().SkillFileCache);
                        }
                        else
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Gungirl/Gungirl_Manyou", mainPlayer.playerActor.m_cpkEntityInfo, null,null);
                        }
#else
                        geActor.LoadOneSkillConfig("Data/SkillData/Gungirl/Gungirl_Manyou", mainPlayer.playerActor.m_cpkEntityInfo);
#endif
                    }
                    // geActor.LoadOneSkillConfig("Data/SkillData/Gungirl/Gungirl_Manyou_1", mainPlayer.playerActor.m_cpkEntityInfo);
                    break;
                case ActorOccupation.GunMan:
                    {
#if LOGIC_SERVER
                        if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Gunman/Gunman_Manyou", mainPlayer.playerActor.m_cpkEntityInfo, 
                                mDungeonManager.GetBeScene().ActionFrameMgr, mDungeonManager.GetBeScene().SkillFileCache);
                        }
                        else
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Gunman/Gunman_Manyou", mainPlayer.playerActor.m_cpkEntityInfo, null,null);
                        }
#else
                        geActor.LoadOneSkillConfig("Data/SkillData/Gunman/Gunman_Manyou", mainPlayer.playerActor.m_cpkEntityInfo);
#endif
                    }
                    // geActor.LoadOneSkillConfig("Data/SkillData/Gunman/Gunman_Manyou_1", mainPlayer.playerActor.m_cpkEntityInfo);
                    break;
                default:
                    break;
            }

            GameFrameWork.instance.StartCoroutine(_addExSkill(new int[] { }, 0, true));
             */
        }

        protected override void _onEnd()
        {
            if (null != mCoroutine)
            {
                GameFrameWork.instance.StopCoroutine(mCoroutine);
                mCoroutine = null;
            }

            mBossActor = null;
            mEliteMonster = null;
        }

        protected override void _onPlayerUseSkill(BattlePlayer player, int skill)
        {
            var seat = player.playerInfo.seat;
            if (seat == ClientApplication.playerinfo.seat)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonPlayerUseSkill, skill);
            }
        }

        protected override void _onPlayerDead(BattlePlayer player)
        {
            RebornFrameCommand cmd = new RebornFrameCommand();
            cmd.frame = 0;
            cmd.seat = player.playerInfo.seat;
            cmd.targetSeat = player.playerInfo.seat;
            FrameSync.instance.FireFrameCommand(cmd);
        }

        protected void _createMonsterExceptType(int type, bool revert = false)
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var monsters = mDungeonData.CurrentMonsters();

            var list = new List<Battle.DungeonMonster>(monsters.ToArray());

            list.RemoveAll(x =>
            {
                if (revert)
                {
                    return !(type == (x.typeId % 100 / 10));
                }

                return type == (x.typeId % 100 / 10);
            });

            mBeScene.CreateMonsterList(list, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition());
        }

        protected override void _createMonsters()
        {
            /*if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                _createMonsterExceptType(3);
            }
            else*/
            {
                base._createMonsters();
            }
        }

        protected void Log(string str, params object[] args)
        {
            Logger.EditorLogWarning("[NewbieGuideBatttle] {0}\n",string.Format(str, args));
        }

        protected void DungeonPause(bool bPause)
        {
            if(bPause)
            {
                Log("PauseFight");
                mDungeonManager.PauseFight();
            }
            else
            {
                Log("ResumeFight");
                mDungeonManager.ResumeFight();
            }
        }
        protected IEnumerator _waitForTime(float time, bool isPause = false)
        {   
            Log("_waitForTime Begin {0} Pause {1}",time,isPause);
            if (mDungeonManager == null) yield break;
            if (isPause) mDungeonManager.PauseFight();

            yield return Yielders.GetWaitForSeconds(time);
            if (mDungeonManager == null) yield break;
            if (isPause) mDungeonManager.ResumeFight();
            Log("_waitForTime End");
        }

        protected IEnumerator _startEliteIntroduce(int skillID, float time)
        {
            var mons = _findNoneBossActor();

            yield return Yielders.EndOfFrame;

            if (null != mons)
            {
                bool flag = false;
                mons.RegisterEventNew(BeEventType.onHurt, args =>
                //mons.RegisterEvent(BeEventType.onHurt, args =>
                {
                    if (!flag && mons.GetEntityData().GetHP() <= 0)
                    {
                        flag = true;
                        _createMonsterExceptType(3, true);
                        mDungeonManager.GetBeScene().state = BeSceneState.onFight;
                        mEliteMonster = null;
                    }
                });
            }

            yield return Yielders.EndOfFrame;

            mons.hasAI = false;

            yield return Yielders.EndOfFrame;

            mons.UseSkill(skillID, true);

            yield return _waitForTime(time);


            mons.hasAI = true;
        }

        protected IEnumerator _waitForBossBrith()
        {
            while (mEliteMonster != null)
            {
                yield return Yielders.EndOfFrame;
            }
        }

		protected IEnumerator _DoLogic(UnityAction logic)
		{
            Log("_DoLogic   {0} ",logic);
			if (logic != null)
				logic();
			yield break;
		}

        protected IEnumerator _startBossIntroduce(float time)
        {
            yield return Yielders.EndOfFrame;

            mInputManager.SetVisible(false);

            mDungeonManager.GetBeScene().state = BeSceneState.onFight;

            //boss.Reset();
            //boss.hasAI = false;
            //Vec3 bossPos = boss.GetPosition();
            ////boss.SetPosition(new Vec3(bossPos.x - 100, bossPos.y, bossPos.z - 50));

            //yield return _waitForState(BeSceneState.onFight);

            _createMonsterExceptType(3, true);
            yield return Yielders.EndOfFrame;
            BeActor boss = _findBossActor();
            boss.Reset();
            boss.hasAI = false;
            boss.SetCanBeAttacked(false);

            boss.m_pkGeActor.ChangeAction("Anim_Chuchang", 1.0f, false, true);
            boss.m_pkGeActor.SetMaterial("");
            boss.needHitShader = true;
            boss.m_pkGeActor.SetHeadInfoVisible(false);
            boss.m_pkGeActor.SetFootIndicatorVisible(false);
            mBossHurtHandler = boss.RegisterEventNew(BeEventType.onHurt, _onBossHurt);
            //mBossHurtHandler = boss.RegisterEvent(BeEventType.onHurt, _onBossHurt);

            yield return _waitForTime(time);

            mInputManager.SetVisible(true);
            boss.hasAI = true;
            boss.Reset();
            boss.StartAI(null);
            boss.SetCanBeAttacked(true);
        }

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
            mDungeonManager.GetBeScene().SetTransportDoorEnable(TransportDoorType.Left, false);
            mDungeonManager.GetBeScene().SetTransportDoorEnable(TransportDoorType.Right, false);

            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                mDungeonManager.FinishFight();
            }
            else
            {
                //SystemNotifyManager.SysDungeonAnimation2("怪物已清除");
            }
        }

        protected override void _onPostStart()
        {
            GameFrameWork.instance.StartCoroutine(_save());

            mCoroutine = GameFrameWork.instance.StartCoroutine(_startGuide());


            FrameRandom.ResetSeed(0x1314555);

            CameraAspectAdjust.MarkDirty();
        }


        protected IEnumerator _waitForDialog(int id)
        {
            Log("_waitForDialog {0}",id);
            Logger.LogProcessFormat("[start] wait for dialog {0}", id);
            _DoStateNewbieGuideFunc("Dialog " + id + "Begin");
            if (mDungeonManager == null) yield break;
            mDungeonManager.PauseFight();

            bool isFinish = false;

            var task = new GameClient.TaskDialogFrame.OnDialogOver();
            task.AddListener(() =>
            {
                isFinish = true;
            });
            MissionManager.GetInstance().CreateDialogFrame(id, 0, task);

            while (!isFinish)
            {
                yield return Yielders.EndOfFrame;
            }
            if (mDungeonManager == null) yield break;
            mDungeonManager.ResumeFight();
            _DoStateNewbieGuideFunc("Dialog " + id + "End");
            Logger.LogProcessFormat("[end] wait for dialog {0}", id);
        }

        protected IEnumerator _waitForState(BeSceneState state)
        {
            Log("wait for state {0} begin", state);
            Logger.LogProcessFormat("[start] wait for state {0}", state);
            if (mDungeonManager == null) yield break;
            while (null != mDungeonManager.GetBeScene() && mDungeonManager.GetBeScene().state != state)
            {
                yield return Yielders.EndOfFrame;
                if (mDungeonManager == null) yield break;
            }
             Log("wait for state {0} end", state);
            Logger.LogProcessFormat("[end] wait for state {0}", state);
        }

        protected IEnumerator _waitForStateEX(BeSceneState state)
        {
            Logger.LogProcessFormat("[start] wait for state {0}", state);

            while (null != mDungeonManager.GetBeScene() && mDungeonManager.GetBeScene().state < state)
            {
                yield return Yielders.EndOfFrame;
            }

            Logger.LogProcessFormat("[end] wait for state {0}", state);
        }

        private VFactor mBackupZDimFactor = VFactor.one;
        private bool mBackupBossCamera = false;

        protected IEnumerator _waitForBossRoom()
        {
            Logger.LogProcessFormat("[start] wait for boss room");
            //mBackupZDimFactor = Global.Settings.zDimFactor;
            //mBackupBossCamera = Global.Settings.openBossShow;
            //Global.Settings.zDimFactor = 3.0f;
            //Global.Settings.openBossShow = false;

            while (!mDungeonManager.GetDungeonDataManager().CurrentDataConnect().IsBoss())
            {
                yield return Yielders.EndOfFrame;
            }

            Logger.LogProcessFormat("[end] wait for boss room");
        }

//         protected IEnumerator _revertZDimFactor()
//         {
//             Global.Settings.zDimFactor = mBackupZDimFactor;
//             //Global.Settings.openBossShow = mBackupBossCamera;
//             yield return Yielders.EndOfFrame;
//         }

        protected IEnumerator _openFrame(Type type, bool isPause = false)
        {
            if (isPause) mDungeonManager.PauseFight();

            if (!ClientSystemManager.instance.IsFrameOpen(type))
            {
                ClientSystemManager.instance.OpenFrame(type);
            }

            if (isPause) mDungeonManager.ResumeFight();

            yield return Yielders.EndOfFrame;
        }


        protected IEnumerator _waitFrameClose(Type type)
        {
            if (mDungeonManager == null) yield break;
            mDungeonManager.PauseFight();

            Logger.LogProcessFormat("[start] wait for frame close {0}", type);

            if (!ClientSystemManager.instance.IsFrameOpen(type))
            {
                ClientSystemManager.instance.OpenFrame(type);
            }

            while (ClientSystemManager.instance.IsFrameOpen(type))
            {
                yield return Yielders.EndOfFrame;
            }

            if(mDungeonManager != null)
            {
                mDungeonManager.ResumeFight();
            }

            Logger.LogProcessFormat("[end] wait for frame close {0}", type);
        }

        protected IEnumerator _waitFrame(Type type, float delay, bool pauseGame = true)
        {
            if (pauseGame) mDungeonManager.PauseFight();

            Logger.LogProcessFormat("[start] wait for frame {0}, {1}", type, delay);

            ClientSystemManager.instance.OpenFrame(type);

            yield return Yielders.GetWaitForSeconds(delay);

			ClientSystemManager.instance.CloseFrameByType(type, false);

            if (pauseGame) mDungeonManager.ResumeFight();

            Logger.LogProcessFormat("[end] wait for frame {0}, {1}", type, delay);
        }

        //protected IEnumerator _waitTips(string msg, float delay)
        //{
        //    Logger.LogProcessFormat("[start] wait for tips {0}, {1}", msg, delay);
        //    SystemNotifyManager.SysDungeonAnimation(msg);
        //    yield return Yielders.GetWaitForSeconds(delay);
        //    Logger.LogProcessFormat("[end] wait for tips {0}, {1}", msg, delay);
        //}

        protected IEnumerator _waitTips(string msg, float delay)
        {
            Logger.LogProcessFormat("[start] wait for newbie tips {0}, {1}", msg, delay);

            var frame = ClientSystemManager.instance.OpenFrame<NewbieGuideMsgTipsFrame>() as NewbieGuideMsgTipsFrame;

            frame.SetMessage(msg);

            yield return Yielders.GetWaitForSeconds(delay);

            ClientSystemManager.instance.CloseFrame<NewbieGuideMsgTipsFrame>();

            Logger.LogProcessFormat("[end] wait for newbie tips {0}, {1}", msg, delay);
        }

        protected IEnumerator _waitPlayerLowHp(float rate)
        {
            Logger.LogProcessFormat("[start] wait for low hp rate: {0}", rate);

            var player = mDungeonPlayers.GetMainPlayer();
            var data = player.playerActor.GetEntityData();

            while (data.GetHPRate().single >= rate)
            {
                yield return Yielders.EndOfFrame;
            }

            yield return Yielders.EndOfFrame;
            player.playerActor.DoHPChange((int)(data.GetMaxHP() * rate), false);

            yield return Yielders.EndOfFrame;
            player.playerActor.Reset();

            Logger.LogProcessFormat("[end] wait for low hp rate: {0}", rate);
        }

        protected IEnumerator _setPlayerHp(float rate)
        {
            Logger.LogProcessFormat("[start] wait for low hp rate: {0}", rate);

            var player = mDungeonPlayers.GetMainPlayer();
            var data = player.playerActor.GetEntityData();

            yield return Yielders.EndOfFrame;

            if (data.GetHPRate().single <= rate)
            {
                player.playerActor.DoHPChange((int)(data.GetMaxHP() * rate), false);
            }

            yield return Yielders.EndOfFrame;

            player.playerActor.Reset();

            Logger.LogProcessFormat("[end] wait for low hp rate: {0}", rate);
        }

        protected BeActor mBossActor = null;

        private BeActor _findBossActor()
        {
            if (mBossActor == null)
            {
				List<BeActor> bossList = GamePool.ListPool<BeActor>.Get();
				mDungeonManager.GetBeScene().FindBoss(bossList);
                if (bossList.Count > 0)
                {
                    mBossActor = bossList[0];
                }

				GamePool.ListPool<BeActor>.Release(bossList);
            }

            return mBossActor;
        }

        protected BeActor mEliteMonster = null;
        private BeActor _findNoneBossActor()
        {
            if (mEliteMonster == null)
            {
                var monsterlist = mDungeonManager.GetBeScene().GetFullEntities();
                for (int i = 0; i < monsterlist.Count; ++i)
                {
                    var em = monsterlist[i] as BeActor;
                    if (em.m_iCamp != (int)ProtoTable.UnitTable.eCamp.C_HERO &&
                       (em.GetEntityData().type == (int)ProtoTable.UnitTable.eType.ELITE ||
                        em.GetEntityData().type == (int)ProtoTable.UnitTable.eType.MONSTER))
                    {
                        mEliteMonster = em;
                    }
                }
            }

            return mEliteMonster;
        }


        /// <summary>
        /// 让boss使用技能
        /// </summary>
        /// <param name="skill">技能ID</param>
        /// <param name="time">等待时间</param>
        /// <param name="skilltime">技能释放时间</param>
        /// <returns></returns>
        protected IEnumerator _waitBossFinalSkill(int skill, float time, float skilltime, int dialogId)
        {
            Logger.LogProcessFormat("[start] wait for boss final skill: {0}", skill);

            var boss = _findBossActor();

            yield return Yielders.GetWaitForSeconds(time);

            if (null != boss)
            {
                boss.hasAI = false;
                boss.Reset();

                yield return Yielders.EndOfFrame;

                if (!boss.UseSkill(skill, true))
                {
                    Logger.LogErrorFormat("[middle] wait for boss skill {0} faild", skill);
                }

                yield return Yielders.EndOfFrame;

                yield return _waitForDialog(dialogId);

                boss.hasAI = true;
            }
            else
            {
                Logger.LogError("boss is nil");
            }



            yield return Yielders.GetWaitForSeconds(skilltime);

            Logger.LogProcessFormat("[end] wait for boss final skill: {0}", skill);
        }

        private void _onBossHurt(BeEvent.BeEventParam param)
        {
            var boss = _findBossActor();

            if (boss.GetEntityData().GetHPRate().single < 0.3f)
            {
                boss.DoHeal(param.m_Int);
            }
        }

        private IBeEventHandle mBossHurtHandler = null;
        //private const string kBossMoveAnimatePrefab = "UI/Prefabs/Battle/BossAnimate";

        protected IEnumerator _waitBossIntroduce(float time)
        {
            Logger.LogProcessFormat("[start] wait for boss introduce");

            var boss = _findBossActor();

            if (null != boss)
            {
                yield return Yielders.GetWaitForSeconds(time);

                boss.hasAI = true;
                boss.Reset();
            }

            yield return Yielders.EndOfFrame;

            Logger.LogProcessFormat("[end] wait for boss introduce");
        }

        protected IEnumerator _waitBossDead(float time)
        {
            Logger.LogProcessFormat("[start] wait for boss dead");

            int cnt = 20;
            var boss = _findBossActor();
            var hurthp = boss.GetEntityData().GetHP() / cnt + 1;

            if (null != mBossHurtHandler)
            {
                //boss.RemoveEvent(mBossHurtHandler);
                mBossHurtHandler.Remove();
                mBossHurtHandler = null;
            }

            for (int i = 0; i < cnt; i++)
            {
                boss.DoHurt(hurthp);
                yield return Yielders.GetWaitForSeconds(0.1f);
            }

            //boss.DoDead();

            yield return Yielders.GetWaitForSeconds(time);

            Logger.LogProcessFormat("[end] wait for boss dead");
        }

        protected IEnumerator _returnToTown()
        {
            Log("_returnToTown");
            //string str = string.Format("[GuideBattle]End");
            //GameStatisticManager.GetInstance().DoStatistic(str);
            var currentSystem = ClientSystemManager.instance.GetCurrentSystem();
            if (currentSystem != null && currentSystem != typeof(ClientSystemTown))
            {
                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
            }
            yield break;
        }

        protected IEnumerator _setVisibleInputmanager(bool isShow)
        {
            yield return Yielders.EndOfFrame;

            if (null != mInputManager)
            {
                mInputManager.SetVisible(isShow);
            }
        }

        protected IEnumerator _setPlayerFrameCommand(bool enable)
        {
            Log("EnablePlayerFrameCommand {0}",enable);
            InputManager.instance.isLock = !enable;
            FrameSync.instance.isFire = enable;
            yield break;
        }

        protected void _initExSkill(int[] skills)
        {
            SkillBarGrid[] bklist = SkillDataManager.GetInstance().GetSkillConfiguration(false).ToArray();

            List<SkillBarGrid> list = new List<SkillBarGrid>();// PlayerBaseData.GetInstance().skillBar;
            for (int i = 0; i < skills.Length; ++i)
            {
                SkillBarGrid grid = new SkillBarGrid();
                grid.id = (ushort)skills[i];
                grid.slot = (byte)(i + 1);
                list.Add(grid);
            }

            SkillDataManager.GetInstance().SetPvePage1SkillBar(list);
            _reLoadSkillButton();
            _hiddenInputManagerJump();
            if (mDungeonPlayers == null) return;
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            for (int i = 0; i < skills.Length; ++i)
            {
                if (!mainPlayer.playerActor.HasSkill(skills[i]))
                {
                    mainPlayer.playerActor.skillController.LoadOneSkill(skills[i], _getSkillLevel());
                }
            }
            
            foreach (var kv in mainPlayer.playerActor.GetSkills())
            {
                kv.Value.level = _getSkillLevel();
                kv.Value.cdReduceRate = new VRate(_getSkillCDReduceRate());
            }

            SkillDataManager.GetInstance().SetPvePage1SkillBar(new List<SkillBarGrid>(bklist));
        }

        protected void _hideExSkill(int[] skills, int start, int end)
        {
            if (mInputManager == null) return;
            for (int i = start; i <= end && i <skills.Length; ++i)
            {
                mInputManager.GetETCButton(skills[i]).gameObject.transform.localScale = Vector3.zero;
                //yield return showSkill(mInputManager.GetETCButton(skills[i]).gameObject,0.1f);
            }
        }
        public const float waitTime = 0.0f;
        protected IEnumerator _addExSkillEx(int[] skills, int start, int end)
        {
            if (mDungeonManager == null) yield break;
            mDungeonManager.PauseFight();

            List<GameObject> objList = new List<GameObject>();
            yield return Yielders.EndOfFrame;
            if (mInputManager == null) yield break;
            if (skills != null && skills.Length > start && skills.Length > end)
            {
                for (int i = start; i <= end; ++i)
                {
                    //var effect = AssetLoader.instance.LoadResAsGameObject("Effects/Scene_effects/EffectUI/EffUI_Autoskill_chixu_guo");
                    //objList.Add(effect);
                    //Utility.AttachTo(effect,mInputManager.GetETCButton(skills[i]).gameObject);
                    yield return _GuideShowSkill(mInputManager.GetETCButton(skills[i]).gameObject, 0.1f);
                    yield return Yielders.GetWaitForSeconds(0.05f);
                }
            }

            yield return Yielders.GetWaitForSeconds(waitTime);

            for (int i = 0; i < objList.Count; ++i)
            {
                GameObject.Destroy(objList[i]);
            }
            objList.Clear();
            if (mDungeonManager == null) yield break;
            mDungeonManager.ResumeFight();
        }

        protected virtual int _getSkillLevel()
        {
            return 1;
        }

        protected virtual float _getSkillCDReduceRate()
        {
            return 0.0f;
        }

        protected IEnumerator _addExSkill(int[] skills, float wait = 2, bool isHidden = false)
        {
            Log("_addExSkill {0} {1} {2}", skills,wait,isHidden);;
            if (mDungeonManager == null) yield break;
            mDungeonManager.PauseFight();
            //if (skills.Length == 2)
            //{
            //    mDungeonManager.PauseFight();

            //    var frame = ClientSystemManager.instance.OpenFrame(typeof(NewbieGuideNewSkillTipsFrame)) as NewbieGuideNewSkillTipsFrame;
            //    frame.SetSkill(skills);

            //    yield return Yielders.GetWaitForSeconds(wait);

            //    ClientSystemManager.instance.CloseFrame(typeof(NewbieGuideNewSkillTipsFrame), false);

            //    mDungeonManager.ResumeFight();
            //}

            SkillBarGrid[] bklist = SkillDataManager.GetInstance().GetSkillConfiguration(false).ToArray();

            List<SkillBarGrid> list = new List<SkillBarGrid>();// PlayerBaseData.GetInstance().skillBar;
            for (int i = 0; i < skills.Length; ++i)
            {
                SkillBarGrid grid = new SkillBarGrid();
                grid.id = (ushort)skills[i];
                grid.slot = (byte)(i + 1);
                list.Add(grid);
            }

            SkillDataManager.GetInstance().SetPvePage1SkillBar(list) ;
            _reLoadSkillButton();
            _hiddenInputManagerJump();
            if (mInputManager != null)
            {
                for (int i = 0; i < skills.Length; ++i)
                {
                    mInputManager.GetETCButton(skills[i]).gameObject.transform.localScale = Vector3.zero;
                    //yield return showSkill(mInputManager.GetETCButton(skills[i]).gameObject,0.1f);
                }
                if (isHidden) mInputManager.SetVisible(false);
            }
            yield return Yielders.EndOfFrame;
            if (mDungeonPlayers == null) yield break;
            var mainPlayer = mDungeonPlayers.GetMainPlayer();

            for (int i = 0; i < skills.Length; ++i)
            {
                if (!mainPlayer.playerActor.HasSkill(skills[i]))
                {
                    mainPlayer.playerActor.skillController.LoadOneSkill(skills[i], _getSkillLevel());
                }
            }

            foreach (var kv in mainPlayer.playerActor.GetSkills())
            {
                kv.Value.level = _getSkillLevel();
                kv.Value.cdReduceRate = new VRate(_getSkillCDReduceRate());
            }

            yield return Yielders.EndOfFrame;

            // revert the skill bar
            SkillDataManager.GetInstance().SetPvePage1SkillBar(new List<SkillBarGrid>(bklist)) ;
            //var jumpbutton = mInputManager.GetETCButton(-1);
            //jumpbutton.gameObject.SetActive(false);

            List<GameObject> objList = new List<GameObject>();
            if (mInputManager != null)
            {
                for (int i = 0; i < skills.Length; ++i)
                {
                    //var effect = AssetLoader.instance.LoadResAsGameObject("Effects/Scene_effects/EffectUI/EffUI_Autoskill_chixu_guo");
                    //objList.Add(effect);
                    //Utility.AttachTo(effect,mInputManager.GetETCButton(skills[i]).gameObject);
                    yield return _GuideShowSkill(mInputManager.GetETCButton(skills[i]).gameObject, 0.1f);
                    yield return Yielders.GetWaitForSeconds(0.05f);
                }
            }

            yield return Yielders.GetWaitForSeconds(waitTime);

            for (int i = 0; i < objList.Count; ++i)
            {
                GameObject.Destroy(objList[i]);
            }
            objList.Clear();

            mDungeonManager.ResumeFight();
        }

        private void _createSkillFrameCommand(int skillID, SkillFrameCommand.SkillFrameData data)
        {
            SkillFrameCommand cmd = new SkillFrameCommand();
            cmd.frame = 0;
            cmd.skillSolt = (uint)skillID;

            cmd.skillSlotUp = SkillFrameCommand.Assemble(data);
            FrameSync.instance.FireFrameCommand(cmd);
        }


        protected IEnumerator _useFinalSkill(int skill, float time, ActorOccupation newJob)
        {
            var mainPlayer = mDungeonPlayers.GetMainPlayer();

            GeActorEx geActor = mainPlayer.playerActor.m_pkGeActor;
            /*
            switch (newJob)
            {
                case ActorOccupation.SwordMan:
                    {
#if LOGIC_SERVER
                        if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Swordman/Swordman_Kuangzhan", mainPlayer.playerActor.m_cpkEntityInfo, 
                                mDungeonManager.GetBeScene().ActionFrameMgr, mDungeonManager.GetBeScene().SkillFileCache);
                        }
                        else
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Swordman/Swordman_Kuangzhan", mainPlayer.playerActor.m_cpkEntityInfo, null,null);
                        }
#else
                    geActor.LoadOneSkillConfig("Data/SkillData/Swordman/Swordman_Kuangzhan", mainPlayer.playerActor.m_cpkEntityInfo);
#endif
                    }
                    break;
                case ActorOccupation.MagicMan:
                    {
#if LOGIC_SERVER
                        if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Mage/Mage_zhaohuan", mainPlayer.playerActor.m_cpkEntityInfo, 
                                mDungeonManager.GetBeScene().ActionFrameMgr, mDungeonManager.GetBeScene().SkillFileCache);
                        }
                        else
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Mage/Mage_zhaohuan", mainPlayer.playerActor.m_cpkEntityInfo, null,null);
                        }
#else
                        geActor.LoadOneSkillConfig("Data/SkillData/Mage/Mage_zhaohuan", mainPlayer.playerActor.m_cpkEntityInfo);
#endif
                    }
                    break;
                case ActorOccupation.Gungirl:
                    {
#if LOGIC_SERVER
                        if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Gungirl/Gungirl_Manyou", mainPlayer.playerActor.m_cpkEntityInfo, 
                                mDungeonManager.GetBeScene().ActionFrameMgr, mDungeonManager.GetBeScene().SkillFileCache);
                        }
                        else
                        {
                            geActor.LoadOneSkillConfig("Data/SkillData/Gungirl/Gungirl_Manyou", mainPlayer.playerActor.m_cpkEntityInfo, null,null);
                        }
#else
                        geActor.LoadOneSkillConfig("Data/SkillData/Gungirl/Gungirl_Manyou", mainPlayer.playerActor.m_cpkEntityInfo);
#endif
                    }
                    break;
                default:
                    break;
            }
             */
            yield return Yielders.EndOfFrame;


            mainPlayer.playerActor.skillController.LoadOneSkill(skill, 35);

            yield return Yielders.EndOfFrame;

            mDungeonManager.PauseFight();

            Type type = typeof(NewbieGuideFinalSkillTipsFrame);

            if (!ClientSystemManager.instance.IsFrameOpen(type))
            {
                var frame = ClientSystemManager.instance.OpenFrame(type) as NewbieGuideFinalSkillTipsFrame;
                frame.SetSkill(skill);
            }

            while (ClientSystemManager.instance.IsFrameOpen(type))
            {
                yield return Yielders.EndOfFrame;
            }

            mDungeonManager.ResumeFight();

            yield return Yielders.EndOfFrame;

            var elist = mDungeonManager.GetBeScene().GetFullEntities();
            for (int i = 0; i < elist.Count; ++i)
            {
                elist[i].Reset();

                if (elist[i].m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY)
                {
                    elist[i].hasAI = false;
                }
            }

            yield return Yielders.EndOfFrame;

            mainPlayer.playerActor.Reset();

            yield return Yielders.EndOfFrame;

            mainPlayer.playerActor.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, (int)(time * 1000));

            yield return Yielders.EndOfFrame;

            _createSkillFrameCommand(skill, new SkillFrameCommand.SkillFrameData());

            yield return Yielders.EndOfFrame;

            _createSkillFrameCommand(skill, new SkillFrameCommand.SkillFrameData{isUp = true});

            yield return Yielders.GetWaitForSeconds(time);
        }

        protected IEnumerator _createAPC(int id, int level, int skillID)
        {
            var mainPlayer = mDungeonPlayers.GetMainPlayer();

            AccompanyData data;
            data.id = id;
            data.level = level;
            data.skillID = skillID;

            mainPlayer.playerActor.SummonHelp(data);

            yield break;
        }

        protected IEnumerator _resetCamera(float time)
        {
            Log("_resetCamera time{0}",time);
            yield return _moveCameraTo(-mMoveX, time);
        }

        protected float mMoveX = 0.0f;

        protected IEnumerator _moveCameraTo(float x, float time, Ease type = Ease.Linear)
        {
            Log("_moveCameraTo x{0} time {1} type {2}",x,time,type);
            mMoveX += x;
            if (mDungeonManager == null) yield break;
            GeSceneEx scene = mDungeonManager.GetBeScene().currentGeScene;
            GeCamera camera = scene.GetCamera();

            Vector3 originPos = camera.localPosition;
            Vector3 toPos = new Vector3(originPos.x + x, originPos.y, originPos.z);

            if (time <= 0)
            {
                camera.SetLocalPosition(toPos);
                yield break;
            }
            else
            {
                yield return null;

                var dt = DOTween.To(
                        () => originPos,
                        r => camera.SetLocalPosition(r),
                        toPos,
                        time).SetEase(type);

                yield return Yielders.GetWaitForSeconds(time);
            }
        }

        protected IEnumerator _waitSpriteTips(string path, float delay, bool pauseGame)
        {
            if (pauseGame) mDungeonManager.PauseFight();

            NewbieGuideImageTips frame = ClientSystemManager.instance.OpenFrame<NewbieGuideImageTips>() as NewbieGuideImageTips;

            frame.SetSprite(path);

            while (delay > 0.0f && ClientSystemManager.instance.IsFrameOpen(typeof(NewbieGuideImageTips)))
            {
                delay -= Time.deltaTime;

                yield return Yielders.EndOfFrame;
            }

            //yield return Yielders.GetWaitForSeconds(delay);

            ClientSystemManager.instance.CloseFrame<NewbieGuideImageTips>();

            if (pauseGame) mDungeonManager.ResumeFight();
        }

        protected class ComboSkillUnit
        {
            public ComboSkillUnit(int skill, float time, string text = "")
            {
                this.skill = skill;
                this.time = time;
                this.text = text;
            }

            public int skill = 1;
            public float time = 0.5f;

            public string text;
        }

        public const string fingerPath = "UIFlatten/Prefabs/NewbieGuide/FingerMove";
        public const string fingerDoublePath = "UIFlatten/Prefabs/NewbieGuide/FingerDoubleMove";
		public const string buttonTips = "UIFlatten/Prefabs/NewbieGuide/ButtonTipsNoButton_newbiebattle";

        public GameObject LoadUIEffect(GameObject parent, string path, bool bKeep = true)
        {
            var go = AssetLoader.instance.LoadResAsGameObject(path);
            if (bKeep)
            {
                Utility.AttachTo(go, parent);
            }
            else
            {
                Utility.AttachTo(go, parent.transform.parent.gameObject, false);
                var p = parent.GetComponent<RectTransform>();
                var l = go.GetComponent<RectTransform>();
                l.position = p.position;
            }
            go.transform.SetAsLastSibling();
            return go;
        }

        protected IEnumerator _addComboSkillTips(string path, ComboSkillUnit[] skills)
        {
            mInputManager.SetEnable(true);
            var mainPlayers = mDungeonPlayers.GetMainPlayer();
            //NewbieGuidTipsFrame tips = ClientSystemManager.instance.OpenFrame<NewbieGuidTipsFrame>(FrameLayer.High) as NewbieGuidTipsFrame;
            yield return Yielders.EndOfFrame;

            for (int i = 0; i < skills.Length; ++i)
            {
                SkillTable table = TableManager.instance.GetTableItem<SkillTable>(skills[i].skill);
                if (table != null)
                {
                    ETCButton etcbutton = null;
                    if (table.SkillCategory == 1)
                    {
                        etcbutton = mInputManager.GetSpecialETCButton(SpecialSkillID.NORMAL_ATTACK);
                    }
                    else
                    {
                        etcbutton = mInputManager.GetETCButton(skills[i].skill);
                    }

                    if (etcbutton != null)
                    {
                        mDungeonManager.PauseFight();

                        yield return Yielders.EndOfFrame;

                        NewbieGuideUseSkillFrame useskill = ClientSystemManager.instance.OpenFrame<NewbieGuideUseSkillFrame>() as NewbieGuideUseSkillFrame;

                        Vector3 pos = Vector3.zero;

                        pos = etcbutton.transform.position;

                        GameObject useskillShow = LoadUIEffect(etcbutton.gameObject, buttonTips);
                        useskill.SetSkill(skills[i].skill, pos, etcbutton.GetComponent<RectTransform>().sizeDelta);

                        while (ClientSystemManager.instance.IsFrameOpen(typeof(NewbieGuideUseSkillFrame)))
                        {
                            yield return Yielders.EndOfFrame;
                        }
                        GameObject.Destroy(useskillShow);
                        useskillShow = null;

                        mDungeonManager.ResumeFight();

                        yield return Yielders.EndOfFrame;
                        //if(String.IsNullOrEmpty(skills[i].text) == false)
                        {
                            ClientSystemManager.instance.CloseFrame<NewbieGuidTipsFrame>();
                            //NewbieGuidTipsFrame t = ClientSystemManager.instance.OpenFrame<NewbieGuidTipsFrame>(FrameLayer.High) as NewbieGuidTipsFrame;
                            //t.SetTipsText(skills[i].text);
                        }

                        BeSkill skill = mainPlayers.playerActor.GetSkill(skills[i].skill);

                        skill.ResetCoolDown();

                        yield return Yielders.EndOfFrame;

                        //string str = string.Format("[GuideBattle]ComboSkill{0}", i);
                        //GameStatisticManager.GetInstance().DoStatistic(str);

                        mainPlayers.playerActor.UseSkill(skills[i].skill, true);

                        yield return Yielders.GetWaitForSeconds(skills[i].time);


                        //yield return _waitFrameClose(typeof(NewbieGuideUseSkillFrame));
                    }
                }
            }
        }

        protected IEnumerator _removeComboSkillTips()
        {
            var mainPlayers = mDungeonPlayers.GetMainPlayer();
            mainPlayers.playerActor.m_pkGeActor.RemoveComboTips();
            yield return Yielders.EndOfFrame;
        }

        protected virtual IEnumerator _startGuide()
        {
            return null;
        }
    }
}
