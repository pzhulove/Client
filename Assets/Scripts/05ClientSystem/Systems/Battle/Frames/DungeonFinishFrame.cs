using UnityEngine;
using System;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using Protocol;
using Network;
using System.Diagnostics;
using DG.Tweening;
using System.Collections.Generic;
using ActivityLimitTime;

namespace GameClient
{
    public class DungeonFinishFrame : ClientFrame
    {
        private const string kOpenFrameSoundPath = "Sound/SE/result_list";
        protected GameObject mEffect;

#region Override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Finish/DungeonNormalFinish";
        }

        protected override bool _isLoadFromPool()
        {
            return true;
        }
#endregion

#region ExtraUIBind
        private ComDungeonFinishScore mFinishScore = null;
        private ComItemList mFiledNameList = null;
        private UIAudioProxy mAudioProxy = null;
        private DOTweenAnimation[] mDt = new DOTweenAnimation[24];
        private GameObject mFatigueCombustionRoot = null;

        protected override void _bindExUI()
        {
            mFinishScore = mBind.GetCom<ComDungeonFinishScore>("FinishScore");
            mFiledNameList = mBind.GetCom<ComItemList>("FiledNameList");
            mAudioProxy = mBind.GetCom<UIAudioProxy>("AudioProxy");
            mDt[0] = mBind.GetCom<DOTweenAnimation>("dt0");
            mDt[1] = mBind.GetCom<DOTweenAnimation>("dt1");
            mDt[2] = mBind.GetCom<DOTweenAnimation>("dt2");
            mDt[3] = mBind.GetCom<DOTweenAnimation>("dt3");
            mDt[4] = mBind.GetCom<DOTweenAnimation>("dt4");
            mDt[5] = mBind.GetCom<DOTweenAnimation>("dt5");
            mDt[6] = mBind.GetCom<DOTweenAnimation>("dt6");
            mDt[7] = mBind.GetCom<DOTweenAnimation>("dt7");
            mDt[8] = mBind.GetCom<DOTweenAnimation>("dt8");
            mDt[9] = mBind.GetCom<DOTweenAnimation>("dt9");
            mDt[10] = mBind.GetCom<DOTweenAnimation>("dt10");
            mDt[11] = mBind.GetCom<DOTweenAnimation>("dt11");
            mDt[12] = mBind.GetCom<DOTweenAnimation>("dt12");
            mDt[13] = mBind.GetCom<DOTweenAnimation>("dt13");
            mDt[14] = mBind.GetCom<DOTweenAnimation>("dt14");
            mDt[15] = mBind.GetCom<DOTweenAnimation>("dt15");
            mDt[16] = mBind.GetCom<DOTweenAnimation>("dt16");
            mDt[17] = mBind.GetCom<DOTweenAnimation>("dt17");
            mDt[18] = mBind.GetCom<DOTweenAnimation>("dt18");
            mDt[19] = mBind.GetCom<DOTweenAnimation>("dt19");
            mDt[20] = mBind.GetCom<DOTweenAnimation>("dt20");
            mDt[21] = mBind.GetCom<DOTweenAnimation>("dt21");
            mDt[22] = mBind.GetCom<DOTweenAnimation>("dt22");
            mDt[23] = mBind.GetCom<DOTweenAnimation>("dt23");
            mFatigueCombustionRoot = mBind.GetGameObject("FatigueCombustionRoot");
        }

        protected override void _unbindExUI()
        {
            mFinishScore = null;
            mFiledNameList = null;
            mAudioProxy = null;

            for (int i = 0; i < mDt.Length; i++)
            {
                mDt[i] = null;
            }

            mFatigueCombustionRoot = null;
        }
#endregion   

        protected override void _OnOpenFrame()
        {
            //AudioManager.instance.PlaySound(kOpenFrameSoundPath, AudioType.AudioEffect);
            if (BattleMain.battleType == BattleType.GuildPVE)
            {
                GuildPVEBattle battle = null;
                if (BattleMain.instance != null)
                {
                    battle = BattleMain.instance.GetBattle() as GuildPVEBattle;
                }

                if (mFinishScore != null)
                {
                    for (int i = 0; i < mFinishScore.infos.Length; i++)
                    {
                        if (mFinishScore.infos[i] == null) continue;

                        if (battle == null || battle.ValidTable == null) continue;
                        if (battle.ValidTable.dungeonLvl > 1)
                        {
                            mFinishScore.infos[i].mScoreType = ComDungeonScoreInfo.eScoreType.StandardDamage;
                            if (i == 0)
                            {
                                mFinishScore.infos[i].scoreStandard = battle.ValidTable.oneStarDamage;
                                mFinishScore.infos[i].scoreLevel = 3;//S
                            }
                            else if (i == 1)
                            {
                                mFinishScore.infos[i].scoreStandard = battle.ValidTable.twoStarDamage;
                                mFinishScore.infos[i].scoreLevel = 4;//SS
                            }
                            else if (i == 2)
                            {
                                mFinishScore.infos[i].scoreStandard = battle.ValidTable.threeStarDamage;
                                mFinishScore.infos[i].scoreLevel = 5;//SSS
                            }
                            mFinishScore.infos[i].Init();
                        }
                        else
                        {
                            if (i == 0)
                            {
                                mFinishScore.infos[0].mScoreType = ComDungeonScoreInfo.eScoreType.HitCount;
                                mFinishScore.infos[0].Init();
                            }
                            if (i == 1)
                            {
                                mFinishScore.infos[1].mScoreType = ComDungeonScoreInfo.eScoreType.FightTime;
                                mFinishScore.infos[1].Init();
                            }
                            if (i == 2)
                            {
                                mFinishScore.infos[2].mScoreType = ComDungeonScoreInfo.eScoreType.ReborCount;
                                mFinishScore.infos[2].Init();
                            }
                        }
                    }
                }
            }
            else
            {
                if (mFinishScore != null)
                {
                    if (mFinishScore.infos[0] != null)
                    {
                        mFinishScore.infos[0].mScoreType = ComDungeonScoreInfo.eScoreType.HitCount;
                        mFinishScore.infos[0].Init();
                    }
                    if (mFinishScore.infos[1] != null)
                    {
                        mFinishScore.infos[1].mScoreType = ComDungeonScoreInfo.eScoreType.FightTime;
                        mFinishScore.infos[1].Init();
                    }
                    if (mFinishScore.infos[2])
                    {
                        mFinishScore.infos[2].mScoreType = ComDungeonScoreInfo.eScoreType.ReborCount;
                        mFinishScore.infos[2].Init();
                    }
                }
            }
            GameFrameWork.instance.StartCoroutine(_addCallback());

            _playAnimate();

            _playBgm();

            
           
        }

        private void _playAnimate()
        {
            for (int i = 0; i < mDt.Length; ++i)
            {
                if (null != mDt[i] && mDt[i].isActive)
                {
                    mDt[i].DORestart();
                }
            }
        }

        private void _playBgm()
        {
            if (null != mAudioProxy)
            {
                mAudioProxy.TriggerAudio(1);
            }
        }

        protected override void _OnCloseFrame()
        {
            if (null != mFinishScore)
            {
                mFinishScore.Uninit();
                mFinishScore = null;
            }

            if (null == BattleMain.instance)
            {
                return ;
            }

            if (null == BattleMain.instance.GetDungeonManager())
            {
                return ;
            }

            if (null == BattleMain.instance.GetDungeonManager().GetDungeonDataManager())
            {
                return;
            }

            GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.CLICK_RESULT,
                    BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID,
                    BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentAreaID(),
                    "");
        }

        private IEnumerator _addCallback()
        {
            yield return Yielders.GetWaitForSeconds(5);
            frameMgr.CloseFrame(this);
        }

        public void SetData(SceneDungeonRaceEndRes res)
        {
            int monsterExp          = (int)res.killMonsterTotalExp;
            int dungeonExp          = (int)res.raceEndExp;
            int monthCardGold       = (int)res.monthcartGoldReward + (int)res.goldTitleGoldReward;
            UInt32[] addExp         = res.addition.addition;
            if (ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.fatigueBurnType = res.fatigueBurnType;
            GuildDataManager.GetInstance().chestDoubleFlag = res.chestDoubleFlag;
            DungeonScore finalScore = (DungeonScore)res.score;

            var sum = monsterExp + dungeonExp;

            mFinishScore.Init(sum, finalScore);

            int totalCount = System.Enum.GetNames(typeof(DungeonAdditionType)).Length;;

            if (addExp.Length >= totalCount)
            {
                int drugExp  = (int)addExp[(int)DungeonAdditionType.EXP_BUFF];
                int scoreExp = (int)addExp[(int)DungeonAdditionType.EXP_SCORE];
                int diffExp  = (int)addExp[(int)DungeonAdditionType.EXP_HARD];
                int vipExp   = (int)addExp[(int)DungeonAdditionType.EXP_VIP];
                int tapExp   = (int)addExp[(int)DungeonAdditionType.EXP_TAP];
                int relationExp = (int)addExp[(int)DungeonAdditionType.EXP_FRIEND];

                int baseExp = 0;
                if (ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(Protocol.ServiceType.SERVICE_NEW_RACE_END_EXP))
                {
                    baseExp = (int)addExp[(int)DungeonAdditionType.DUNGEON_EXP_BASE];
                }
                else
                {
                    baseExp = dungeonExp - drugExp - scoreExp - diffExp - vipExp - tapExp - relationExp;
                }
                
                int vipGold  = (int)addExp[(int)DungeonAdditionType.GOLD_VIP];

                mFinishScore.SetExpSplit(baseExp, drugExp, scoreExp, diffExp, vipExp, vipGold, monthCardGold, tapExp, relationExp);
            }

			if (ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
				mFatigueCombustionRoot.CustomActive(ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.fatigueBurnType == (int)FatigueBurnType.FBT_COMMON || ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.fatigueBurnType == (int)FatigueBurnType.FBT_ADVANCED);
        }

        public void SetDrops(ComItemList.Items[]  drops)
        {
            mFiledNameList.SetItems(drops);
        }

        private void _onHandle()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
