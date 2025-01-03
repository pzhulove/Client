using UnityEngine;
using UnityEditor;
using Protocol;

namespace GameClient
{
    public class BattleUIDungeonScore : BattleUIBase
    {
        #region ExtraUIBind
        private ComDungeonScore mFightingTimeRoot = null;
        private ComDungeonScore mDungeonScore = null;

        protected override void _bindExUI()
        {
            mFightingTimeRoot = mBind.GetCom<ComDungeonScore>("FightingTimeRoot");
            mDungeonScore = mBind.GetCom<ComDungeonScore>("DungeonScore");
        }

        protected override void _unbindExUI()
        {
            mFightingTimeRoot = null;
            mDungeonScore = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIDungeonScore";
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            _bindDungeonScore();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonScoreChanged, _onScoreUpdate);
        }

        protected override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void OnUpdate(float timeElapsed)
        {
            base.OnUpdate(timeElapsed);
            _baseUITick(timeElapsed);
        }

        protected override void OnExit()
        {
            base.OnExit();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonScoreChanged, _onScoreUpdate);
        }

        protected void _bindDungeonScore()
        {
            if (null != mDungeonScore)
            {

                mDungeonScore.Init();

                mDungeonScore.infos[0].SetScoreCallback(
                        () => { return BattleMain.instance.GetDungeonStatistics().HitCountScore(); });

                mDungeonScore.infos[1].SetScoreCallback(
                        () => { return BattleMain.instance.GetDungeonStatistics().AllFightTimeScore(true); });

                mDungeonScore.infos[1].SetTimeLimiteCallback(
                        () => { return BattleMain.instance.GetDungeonStatistics().FightTimeSplit(1); });

                mDungeonScore.infos[2].SetScoreCallback(
                        () => { return BattleMain.instance.GetDungeonStatistics().RebornCountScore(); });

                mDungeonScore.infos[0].SetCallback(
                        () => { return BattleMain.instance.GetDungeonStatistics().AllHitCount(); });

                mDungeonScore.infos[1].SetCallback(
                        () => { return BattleMain.instance.GetDungeonStatistics().AllFightTime(true); });

                mDungeonScore.infos[2].SetCallback(
                        () => { return BattleMain.instance.GetDungeonStatistics().AllRebornCount(); });
            }


            if (null != mDungeonScore)
            {
                mDungeonScore.onFadeChanged = (State) => {
                    bool bComScoreOpend = (State == ComDungeonScore.eState.Open);
                    if (mFightingTimeRoot)
                    {
                        mFightingTimeRoot.gameObject.CustomActive(!bComScoreOpend);
                    }
                };
            }

            if (null != mFightingTimeRoot && null != mFightingTimeRoot.infos && mFightingTimeRoot.infos.Length > 0)
            {
                mFightingTimeRoot.infos[0].SetCallback(
                    () => { return BattleMain.instance.GetDungeonStatistics().AllFightTime(true); });
            }
        }

        private float mTimeTick = 0;
        private const float kUITick = 0.5f;
        protected void _baseUITick(float delta)
        {
            mTimeTick += delta;
            if (mTimeTick > kUITick)
            {
                mTimeTick -= kUITick;
                _onUIUpdate();
            }
        }

        protected void _onUIUpdate()
        {
            _onScoreUpdate(null);
        }

        private void _onScoreUpdate(UIEvent uiEvent)
        {
            if (null != mDungeonScore
                && null != BattleMain.instance
                && BattleMain.instance.GetDungeonManager() != null
                && BattleMain.instance.GetDungeonManager().GetBeScene() != null
                && !BattleMain.instance.GetDungeonManager().GetBeScene().IsBossDead())
            {
                DungeonScore realScore = BattleMain.instance.GetDungeonStatistics().FinalDungeonScore();
                mDungeonScore.SetScore(realScore);

                if (mFightingTimeRoot)
                {
                    mFightingTimeRoot.SetScore(realScore);
                }
            }
        }

        public void HideFightingTime()
        {
            if (mFightingTimeRoot == null)
                return;
            mFightingTimeRoot.CustomActive(false);
        }
    }
}