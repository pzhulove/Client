using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

namespace GameClient
{
    class ChampionMatchBattle : ActivityBattle
    {
        private int currentIndex = 0;

        public ChampionMatchBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id) { }

        #region 重写虚函数们儿

        protected override void _onCreateScene(BeEvent.BeEventParam args)
        {

        }

        protected override void _onPostStart()
        {
            _setMatchName();

#if !LOGIC_SERVER
            ChampionMatchFrame.inited = false;
            ClientSystemManager.instance.OpenFrame<ChampionMatchFrame>(FrameLayer.Top);
#endif
            mDungeonManager.PauseFight();
        }

        private void _setMatchName()
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIChampionName>();
            if (battleUI != null)
            {
                var id = mDungeonManager.GetDungeonDataManager().id;
                string diff = id.prestoryID > 0 ? "" : ChapterUtility.GetHardString(id.diffID);
                var str = string.Format("<color=#14c5ff>{0}</color>{1}", diff, mDungeonManager.GetDungeonDataManager().asset.GetName());

                battleUI.SetChampionMatchName(str);
            }
#endif
        }

        private bool mIsProcessAreaClear = false;

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
#if !LOGIC_SERVER
            if (!mIsProcessAreaClear)
            {
                mIsProcessAreaClear = true;
                GameFrameWork.instance.StartCoroutine(_processAreaClear());
            }
#else
#endif
        }

        private IEnumerator _processAreaClear()
        {
            yield return _sendDungeonReportDataIter();

            mIsProcessAreaClear = false;

            var mCurBeScene = mDungeonManager.GetBeScene();
            var mainPlayer = mDungeonPlayers.GetMainPlayer();

            mCurBeScene.ForcePickUpDropItem(mainPlayer.playerActor);

#if !LOGIC_SERVER
            _showWinEffect();

            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                _CheckFightEnd();
            }
            else
            {
                ClientSystemManager.instance.delayCaller.DelayCall(1500, () =>
                {
                    FireSceneChangeAreaCmd();
                });
            }
#endif
        }

        private void _showWinEffect()
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPvp>();
            if (battleUI != null)
            {
                battleUI.ShowPkResult(PKResult.WIN);
                PlaySound(19);

                ClientSystemManager.instance.delayCaller.DelayCall(2000, () =>
                {
                    battleUI.HiddenPkResult();
                });
            }
#endif
        }

        public void FireSceneChangeAreaCmd()
        {
#if !LOGIC_SERVER
            SceneChangeArea changeArea = new SceneChangeArea();
            FrameSync.instance.FireFrameCommand(changeArea);
#endif  
        }

        public void ResumeGameCmd()
        {
            if (mDungeonManager != null)
            {
                mDungeonManager.ResumeFight();
            }
        }

        protected override void _onSceneAreaChange()
        {
            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                mDungeonManager.FinishFight();
                _resetPlayerActor(false);
            }
            else
            {
                _changeAreaByIdx();
            }
        }

        protected override void _createPlayers()
        {
            base._createPlayers();

            var dungeonData = mDungeonManager.GetDungeonDataManager();
            var players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].playerActor.SetPosition(dungeonData.CurrentBirthPosition());
            }

            mDungeonManager.GetBeScene().InitFriendActor(dungeonData.CurrentBirthPosition());
        }

        protected override void _onStart()
        {
            _hiddenDungeonMap(true);
            currentIndex = mDungeonManager.GetDungeonDataManager().CurrentIndex();
        }

        protected override void _createDoors()
        {
            // keep empty
        }

        protected override void _onDoorStateChange(BeEvent.BeEventParam args)
        {
            // keep empty
        }

        #endregion

        private void _changeAreaByIdx()
        {
            _changeAreaFade(600, 300, () =>
            {
                if (mDungeonManager.GetDungeonDataManager().NextAreaByIndexBaseOnServerData(currentIndex + 1))
                {
                    currentIndex++;

                    _createBase();
                    _createEntitys();
#if !LOGIC_SERVER
                    PreloadMonster();
#endif
                    _onSceneStart();
                    mDungeonManager.StartFight();

                    _sendSceneDungeonEnterNextAreaReq(mDungeonManager.GetDungeonDataManager().CurrentAreaID());
                    _sendDungeonRewardReq();
                }
            }, () => 
            {
#if !LOGIC_SERVER
                ClientSystemManager.instance.OpenFrame<ChampionMatchFrame>(FrameLayer.Top);
#endif
                if (mDungeonManager != null)
                {
                    var scene = mDungeonManager.GetBeScene();
                    if (scene != null)
                    {
                        scene.Pause(true);
                        scene.PauseLogic();
                    }
                }
            });
        }

    }
}
