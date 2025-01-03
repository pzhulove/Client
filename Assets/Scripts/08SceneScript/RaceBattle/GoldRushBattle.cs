using System.Collections.Generic;
using System.Collections;
using Battle;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine;

/// <summary>
/// Battle类
/// </summary>
namespace GameClient
{
    public class GoldRushBattle : ActivityBattle
    {
        enum GoldRushState
        {
            Ready,
            Fight,
            Finish,
        }
        public GoldRushBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {}

        private GoldRushState mCurState;
        public GoldRushTable GoldRushData { get; private set; }
        public int EndTime { get; private set; }
        public int CurTime { get; private set; }
        private int curTimer;
        public int NextMonsterWaveTime { get; private set; }
        private int mCurWave;
        private bool mWaveTipFlag;
        private BeActor mBossActor;
        private int mBossLastHp;
        private BattleUIGoldRush mBattleUI;

        protected override void _onPostStart()
        {
            int id = mDungeonManager.GetDungeonDataManager().id.dungeonID;
            GoldRushData = TableManager.instance.GetTableItem<GoldRushTable>(id);
    
            mCurState = GoldRushState.Ready;
            mCurWave = 0;
            CurTime = 0;
            curTimer = 0;
            EndTime = GoldRushData.TimeLimit;
            NextMonsterWaveTime = TableManager.GetValueFromUnionCell(GoldRushData.WaveInterval, mCurWave);
            mWaveTipFlag = true;
            _findBossActor();
            mBossLastHp = mBossActor.attribute.GetHP();
            mBattleUI = BattleUIHelper.GetBattleUIComponent<BattleUIGoldRush>();

            mDungeonManager.PauseFight();
            
#if !LOGIC_SERVER
            mDungeonManager.GetBeScene().RegisterEventNew(BeEventSceneType.onPickGold, (eventParam) =>
            {
                mBattleUI.AddGold(eventParam.m_Int, eventParam.m_Vector);
            });
                
            if (InputManager.instance != null && InputManager.instance.isAttackButtonOnly)
            {
                InputManager.instance.ResetButtonState();
            }

            InputManager.instance.SetEnable(false);

            BattleUIHelper.GetBattleUIComponent<BattleUICommon>().SwitchCombo(false);

            var startEff = ClientSystemManager.instance.PlayUIEffect(FrameLayer.Middle, "UIFlatten/Prefabs/Battle/Start/StartCountDown");
#endif
            ClientSystemManager.instance.delayCaller.DelayCall(3000, () =>
            {
#if !LOGIC_SERVER
                InputManager.instance.SetEnable(true);
                if (startEff != null)
                    GameObject.Destroy(startEff);
#endif          
                if (mDungeonManager != null)
                    mDungeonManager.ResumeFight();
                
                mCurState = GoldRushState.Fight;
            });
        }

        protected override void _onUpdate(int delta)
        {
            base._onUpdate(delta);
            if (mCurState != GoldRushState.Fight)
                return;
            curTimer += delta;
            CurTime = curTimer / GlobalLogic.VALUE_1000;
            if (CurTime > EndTime) //time out
            {
                if (!mDungeonPlayers.GetMainPlayer().playerActor.IsDead())
                    _onAreaClear(null);
                else
                    _onPlayerCancelReborn(null);
                return;
            }
            
            //next wave tip
            if (CurTime == NextMonsterWaveTime - GoldRushData.waveTipTime && mWaveTipFlag)
            {
                ShowWaveTip(true);
                
            }
            
            //next wave
            if (CurTime >= NextMonsterWaveTime)
            {
                ShowWaveTip(false);
                
                mCurWave++;
                NextMonsterWaveTime = int.MaxValue;
                if (GoldRushData.WaveInterval.eValues.everyValuesLength > mCurWave)
                {
                    int nextWaveInterval = TableManager.GetValueFromUnionCell(GoldRushData.WaveInterval, mCurWave);
                    NextMonsterWaveTime = mCurWave + nextWaveInterval;
                }

                if (mDungeonManager.GetDungeonDataManager().CurrentMonsterGroupCount() > mCurWave)
                {
                    var waveMonsters = mDungeonManager.GetDungeonDataManager().CurrentMonsters(mCurWave);
                    var mBeScene = mDungeonManager.GetBeScene();
                    var mDungeonData = mDungeonManager.GetDungeonDataManager();
                    var monsterCreatedCount =
                        mBeScene.CreateMonsterList(waveMonsters, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition(), false);
                    thisRoomMonsterCreatedCount += monsterCreatedCount;

                    List<BeEntity> list = GamePool.ListPool<BeEntity>.Get();
                    mBeScene.GetEntitys2(list);
                    for (int i = 0; i < list.Count; i++)
                    {
                        var actor = list[i] as BeActor;
                        if (actor != null && actor.aiManager != null)
                        {
                            actor.StartAI(null);
                        }
                    }
                    GamePool.ListPool<BeEntity>.Release(list);
                }
            }
#if !LOGIC_SERVER
             //drop
             int curBossHp = mBossActor.attribute.GetHP();
             int bossHpDropStep = (int)(mBossActor.attribute.GetMaxHP() * GoldRushData.BossDamageRatio * 0.001);
             if (mBossLastHp - curBossHp >= bossHpDropStep)
             {
                 List<DungeonDropItem> dropItems = new List<DungeonDropItem>
                 {
                     new DungeonDropItem()
                     {
                         id = mBossLastHp,
                         typeId = Global.GOLD_ITEM_ID2,
                         num = GoldRushData.BossDamageGold,
                     }
                 };
                 mDungeonManager.GetBeScene().DropItems(dropItems, mBossActor.GetPosition());
                 mBossLastHp -= bossHpDropStep;
             }
#endif
        }

        private void ShowWaveTip(bool bActive)
        {
            mWaveTipFlag = !bActive;
#if !LOGIC_SERVER
            mBattleUI.ShowWaveTip(bActive);
#endif
        }

        private void _findBossActor()
        {
            List<BeActor> bossList = GamePool.ListPool<BeActor>.Get();
            mDungeonManager.GetBeScene().FindBoss(bossList);
            if (bossList.Count > 0)
            {
                mBossActor = bossList[0];
            }

            GamePool.ListPool<BeActor>.Release(bossList);
        }

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
            mCurState = GoldRushState.Finish;
            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
#if !LOGIC_SERVER
		        _sendDungeonKillMonsterReq();
                _sendDungeonRewardReq();
                _sendDungeonBossRewardReq();
#endif
                mDungeonManager.FinishFight();

#if !LOGIC_SERVER
                GameFrameWork.instance.StartCoroutine(_successEnd());
#endif
            }
        }

        protected override void _onPlayerCancelReborn(BattlePlayer player)
        {
            mCurState = GoldRushState.Finish;
#if !LOGIC_SERVER
            GameFrameWork.instance.StartCoroutine(_failEnd());
#endif
        }

        protected override SceneDungeonRaceEndReq _getDungeonRaceEndReq()
        {
            var msg = base._getDungeonRaceEndReq();
            float ratio = ((float) mBossActor.attribute.GetMaxHP() - mBossActor.attribute.GetHP()) / mBossActor.attribute.GetMaxHP();
            msg.bossDamageRatio = (uint) (ratio * 1000);
            return msg;
        }

        protected IEnumerator _failEnd()
        {
#if !LOGIC_SERVER
            //存储战斗结果
            PveBattleResult = BattleResult.Fail;
#endif
            _openFinishFrame<DungeonCommonFailFrame>();
            _setFinish(false);

            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            yield return Yielders.EndOfFrame;

            if (_isNeedSendNet())
            {
                var req = _getDungeonRaceEndReq();
                var res = new SceneDungeonRaceEndRes();
                var msg = new MessageEvents();

                yield return (_sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msg, req, res));

                if (msg.IsAllMessageReceived())
                {
                    _setExp(res.killMonsterTotalExp + res.raceEndExp);
                    _setDrop(null);
                }
            }
            else
            {
                _setExp(0);
                _setDrop(null);
            }
        }

        protected IEnumerator _successEnd()
        {
#if !LOGIC_SERVER
            //存储战斗结果
            PveBattleResult = BattleResult.Success;
            var actor = mDungeonPlayers.GetMainPlayer().playerActor;
            if (actor != null && !actor.IsDead())
                mDungeonManager.GetBeScene().ForcePickUpDropItem(actor);
#endif
            _openFinishFrame<DungeonGoldRushFinishFrame>();
            _setFinish(true);

            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            yield return Yielders.EndOfFrame;

            if (_isNeedSendNet())
            {
                var req = _getDungeonRaceEndReq();
                var res = new SceneDungeonRaceEndRes();
                var msg = new MessageEvents();

                yield return (_sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msg, req, res));

                if (msg.IsAllMessageReceived())
                {
                    if (0 != res.hasRaceEndDrop)
                    {
                        yield return _requestRaceEndDrops(res.raceEndDropBaseMulti);
                    }
                    (mFinishFrame as DungeonGoldRushFinishFrame).SetExtraGoldReward(res.bossDamageGoldReward);
                    _setExp(res.killMonsterTotalExp + res.raceEndExp);
                    _setDrop(_getAllRewardItems(res).ToArray());
                    //Logger.LogErrorFormat("bossDamageGoldReward:{0}", res.bossDamageGoldReward);
                }
            }
            else
            {
                _setExp(0);
                _setDrop(null);
            }
        }
    }
}
