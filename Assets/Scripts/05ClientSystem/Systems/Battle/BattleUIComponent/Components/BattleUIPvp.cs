using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    /// <summary>
    /// Pvp相关UI
    /// </summary>
    public class BattleUIPvp : BattleUIBase
    {
        public BattleUIPvp() : base() { }

        #region ExtraUIBind
        private GameObject mPkResult = null;
        private SimpleTimer mTimerController = null;

        protected override void _bindExUI()
        {
            mPkResult = mBind.GetGameObject("PkResult");
            mTimerController = mBind.GetCom<SimpleTimer>("TimerController");
        }

        protected override void _unbindExUI()
        {
            mPkResult = null;
            mTimerController = null;
        }
        #endregion
        public SimpleTimer TimerController { get { return mTimerController; } }

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIPvp";
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleFrameSyncEnd, _OnBattleFrameSyncEnd);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKRankChanged, _onPkRankChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleFrameSyncEnd, _OnBattleFrameSyncEnd);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, _OnSystemSwitchFinished);
        }

        protected override void OnExit()
        {
            base.OnExit();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleFrameSyncEnd, _OnBattleFrameSyncEnd);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKRankChanged, _onPkRankChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleFrameSyncEnd, _OnBattleFrameSyncEnd);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemSwitchFinished);
        }

        public void StartTimer(int countdown = 0)
        {
            if (mTimerController != null)
            {
                if (countdown > 0)
                    mTimerController.SetCountdown(countdown);

                mTimerController.StartTimer();
            }
        }

        void _OnBattleFrameSyncEnd(UIEvent a_event)
        {
            StopTimer();
        }

        public void StopTimer()
        {
            if (mTimerController != null)
            {
                mTimerController.StopTimer();
            }
        }

        public void ShowPkResult(PKResult result)
        {
            if (mPkResult == null) return;
            mPkResult.CustomActive(true);

            ComCommonBind pkBind = mPkResult.GetComponent<ComCommonBind>();

            if (null == pkBind)
            {
                return;
            }

            GameObject winObj = pkBind.GetGameObject("objWin");
            GameObject loseObj = pkBind.GetGameObject("objLost");
            GameObject drawObj = pkBind.GetGameObject("objDual");

            winObj.CustomActive(false);
            loseObj.CustomActive(false);
            drawObj.CustomActive(false);

            switch (result)
            {
                case PKResult.DRAW:
                    drawObj.CustomActive(true);
                    break;
                case PKResult.LOSE:
                    loseObj.CustomActive(true);
                    break;
                case PKResult.WIN:
                    winObj.CustomActive(true);
                    break;
            }
        }

        public void HiddenPkResult()
        {
            if (null == mPkResult)
            {
                return;
            }

            mPkResult.CustomActive(false);
        }

        protected void _onPkRankChanged(UIEvent uiEvent)
        {
            if (null != BattleMain.instance)
            {
                switch (BattleMain.battleType)
                {
                    case BattleType.Mou:
                    case BattleType.North:
                    case BattleType.DeadTown:
                    case BattleType.Dungeon:
                    case BattleType.ChampionMatch:
                    case BattleType.GuildPVE:
                    case BattleType.FinalTestBattle:
                        var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
                        if (mainPlayer != null && mainPlayer.playerActor != null && mainPlayer.playerActor.m_pkGeActor != null)
                        {
                            mainPlayer.playerActor.m_pkGeActor.UpdatePkRank(SeasonDataManager.GetInstance().seasonLevel, SeasonDataManager.GetInstance().seasonStar);
                        }
                        break;
                }
            }
        }

        private void _OnSystemSwitchFinished(UIEvent uiEvent)
        {
            if (ChijiDataManager.GetInstance().GuardForPkEndData && BattleMain.battleType == BattleType.ChijiPVP)
            {
                ChijiDataManager.GetInstance().GuardForPkEndData = false;

                Logger.LogErrorFormat("吃鸡结算异常测试----PKResult = {0},[_OnSystemSwitchFinished]", ChijiDataManager.GetInstance().PkEndData.result);

                // 失败退出到城镇要清掉吃鸡相关数据
                ChijiDataManager.GetInstance().ResponseBattleEnd();
            }
        }
    }
}