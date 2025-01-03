using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class Dungeon3v3LoadingFrame : Dungeon3V3BaseLoadFrame
    {
        protected enum eLoadingStatus
        {
            /// <summary>
            /// 加载
            /// </summary>
            Loading,

            /// <summary>
            /// 投票
            /// </summary>
            Vote,

            /// <summary>
            /// 对战动画
            /// </summary>
            VsAnimate,
        }

        protected eLoadingStatus mLoadingStatus = eLoadingStatus.Loading;

        //protected StringBuilder strBuilder;
        protected int _UpdateSpeed = 10;
        protected int _targetProgress = 0;
        protected int _currentProgress = -1;

        protected override void _onApplyBtnButtonClickEvent()
        {
            /* put your code in here */
            BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

            if (null == mainPlayer)
            {
                return ;
            }

            if (ReplayServer.instance.IsReplay())
            {
                mJoinFightRoot.CustomActive(false);
                mCancelFightRoot.CustomActive(false);
                mLeftTimeRoot.CustomActive(false);
            }
            else
            {
                MatchRoundVote cmd = new MatchRoundVote
                {
                    isVote = !mainPlayer.isVote
                };
                FrameSync.instance.FireFrameCommand(cmd);
                mJoinFightRoot.CustomActive(!cmd.isVote);
                mCancelFightRoot.CustomActive(cmd.isVote);
                mLeftTimeRoot.CustomActive(!cmd.isVote);
            }
        }

        protected override bool _IsLoadingFrame()
        {
            return true;
        }

        public override IEnumerator LoadingOpenPost()
        {
            float maxTime = 0.0f;
            if (null != mAllTypeAnimation)
            {
                for (int i = 0; i < mAllTypeAnimation.Count; ++i)
                {
                    if (null != mAllTypeAnimation[i] && mAllTypeAnimation[i].Count > 0)
                    {
                        maxTime = Mathf.Max(maxTime, mAllTypeAnimation[i][0].duration + mAllTypeAnimation[i][0].delay);
                    }
                }
            }

            yield return Yielders.GetWaitForSeconds(maxTime);
        }

        protected override void _OnOpenFrame()
        {

#if UNITY_EDITOR
            _UpdateSpeed = Global.Settings.loadingProgressStepInEditor;
#else
            _UpdateSpeed = Global.Settings.loadingProgressStep;
#endif
            mApplyLimitButton.ResetCount();

            GameObject.DontDestroyOnLoad(frame);
            //strBuilder = StringBuilderCache.Acquire();
            _targetProgress = 0;
            _currentProgress = -1;
            StartCoroutine(UpdateProgress());

            _initBoards();
            _initPlayers();
            _updateAllPlayersStatus();

            mLoadingStatus = eLoadingStatus.Loading;

            _updateWithLoadingStatus();

            for (int i = 0;i < kMaxPlayerSeatCount; ++i)
            {
                _playProcessAnimateByType(eAnimateType.eIn, (byte)i);
            }

            //for (int i = 0; i < mBoards.Count; ++i)
            //{
            //    DG.Tweening.DOTween.Play(mBoards[i].root, "beizan02");

            //    var list = DG.Tweening.GetTweensById("beizan02");

            //    for (int j = 0; j < list.Count; ++i)
            //    {
            //        list[j].DoPlay();
            //    }
            //}

            _bindEvent();

            if (BattleDataManager.GetInstance().PkRaceType == RaceType.PK_3V3_Melee)
                SetFightTitle();
        }

        private void SetFightTitle()
        {
            Image image = mPerpareRoot.GetComponent<Image>();
            ETCImageLoader.LoadSprite(ref image, "UI/Image/Packed/p_UI_3V3PK.png:UI_3v3_Beizhan_Wenzi_Duizhan");
        }

        private new bool _checkBattlePlayerIsValid(BattlePlayer player)
        {
            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return false;
            }

            return null != _findBoardBySeat(player.playerInfo.seat);
        }

        private void _bindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged,   _onPK3V3VoteForFightStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3StartVoteForFight,           _onPK3V3StartVoteForFight);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3FinishVoteForFight,          _onPK3V3FinishVoteForFight);
        }

        private void _unbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3VoteForFightStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3StartVoteForFight,         _onPK3V3StartVoteForFight);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3FinishVoteForFight,        _onPK3V3FinishVoteForFight);
        }

        private void _onPK3V3FinishVoteForFight(UIEvent ui)
        {
            mLoadingStatus = eLoadingStatus.VsAnimate;
            _updateWithLoadingStatus();
        }

        private void _onPK3V3StartVoteForFight(UIEvent ui)
        {
            mLoadingStatus = eLoadingStatus.Vote;
            _updateWithLoadingStatus();
        }

        private void _updateWithLoadingStatus()
        {
            mApplyBtn.gameObject.CustomActive(false);
            mFightVS.gameObject.CustomActive(false);
            mCountDownRoot.CustomActive(false);
            mLeftTimeRoot.CustomActive(false);
            mFightTips.CustomActive(false);
            mNextRoundRoot.CustomActive(false);

            _setUnitLoadingBarVisible(false);

            switch (mLoadingStatus)
            {
                case eLoadingStatus.Loading:
                    _setUnitLoadingBarVisible(true);
                    break;
                case eLoadingStatus.Vote:
                    //mPerpareRoot.CustomActive(true);
                    mCountDownRoot.CustomActive(true);
                    if (!ReplayServer.instance.IsReplay())
                    {
                        mApplyBtn.gameObject.CustomActive(true);
                    }
                    break;
                case eLoadingStatus.VsAnimate:

                    if (BattleDataManager.GetInstance().PkRaceType == RaceType.PK_3V3_Melee)
                    {
                        //DG.Tweening.DOTween.Play(mPerpareRoot);
                        GameObject mFightVS2 = mBind.GetGameObject("3v3Tips2");
                        if (mFightVS2 != null)
                        {
                            mFightVS2.gameObject.CustomActive(true);
                        }

                        GameObject mFightTips2 = mBind.GetGameObject("VS2");
                        if (mFightTips2 != null)
                        {
                            mFightTips2.gameObject.CustomActive(true);
                        }  
                    }
                    else
                    {
                        DG.Tweening.DOTween.Play(mPerpareRoot);
                        mFightVS.gameObject.CustomActive(true);
                        mNextRoundRoot.CustomActive(true);

                        _doVSAnimate();

                        BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
                        if (BattlePlayer.IsDataValidBattlePlayer(mainPlayer))
                        {
                            if (mainPlayer.isFighting)
                            {
                                mFightTips.CustomActive(true);
                            }
                        }
                    }
                    
                    break;
            }
        }

        private void _setUnitLoadingBarVisible(bool isVisible)
        {
            for (int i = 0; i < mBoards.Length; ++i)
            {
                ComCommonBind bind = mBoards[i].unit;

                if (null == bind)
                {
                    continue;
                }

                GameObject progressRoot = bind.GetGameObject("progressRoot");

                if (null == progressRoot)
                {
                    continue;
                }

                progressRoot.CustomActive(isVisible);
            }
        }

        private void _doVSAnimate()
        {
            _doTeamVSAnimate(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
            _doTeamVSAnimate(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
        }

        private void _doTeamVSAnimate(BattlePlayer.eDungeonPlayerTeamType type)
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetCurrentTeamFightingPlayer(type);

            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return ;
            }

            List<BattlePlayer> players = BattleMain.instance.GetPlayerManager().GetAllPlayers();

            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i].teamType == player.teamType)
                {
                    if (players[i].playerInfo.seat != player.playerInfo.seat)
                    {
                        _doOnePlayerVSAnimate((int)players[i].playerInfo.seat);
                        _playProcessAnimateByType(eAnimateType.eOut, players[i].playerInfo.seat);
                    }
                    else
                    {
                        _playProcessAnimateByType(eAnimateType.eSelected, players[i].playerInfo.seat);
                    }
                }
            }
        }

        private void _doOnePlayerVSAnimate(int seat)
        {
            MatchUnit board = _findBoardBySeat((byte)seat);
            if (null == board)
            {
                return ;
            }

            if (null == board.unit)
            {
                return ;
            }

            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat((byte)seat);

            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return ;
            }

            ComCommonBind bind = board.unit;

            Image battleImage  = bind.GetCom<Image>("battleImage");
            Image appliedImage = bind.GetCom<Image>("appliedImage");

            appliedImage.gameObject.CustomActive(false);

            battleImage.gameObject.SetActive(player.isFighting);

        }

        private void _onPK3V3VoteForFightStatusChanged(UIEvent ui)
        {
            _updateAllPlayersStatus();
        }

        private void _updateAllPlayersStatus()
        {
            List<BattlePlayer> players = BattleMain.instance.GetPlayerManager().GetAllPlayers();

            for (int i = 0; i < players.Count; ++i)
            {
                _updateOnePlayerStatus(players[i]);
            }
        }

        private void _updateOnePlayerStatus(BattlePlayer player)
        {
            if (!_checkBattlePlayerIsValid(player))
            {
                return ;
            }

            MatchUnit board = _findBoardBySeat(player.playerInfo.seat);

            if (null == board)
            {
                return ; 
            }

            ComCommonBind bind = board.unit;

            if (null == bind)
            {
                return ;
            }

            Image battleImage  = bind.GetCom<Image>("battleImage");
            Image appliedImage = bind.GetCom<Image>("appliedImage");

            battleImage.gameObject.CustomActive(false);

            bool isShowVote = false;

            BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

            if (BattlePlayer.IsDataValidBattlePlayer(mainPlayer))
            {
                if (player.teamType == mainPlayer.teamType)
                {
                    isShowVote = player.isVote;
                }
                else
                {
                    isShowVote = false;
                }
            }

            appliedImage.gameObject.CustomActive(isShowVote);
        }

        protected override void _OnCloseFrame()
        {
            _uninitBoards();
            _unbindEvent();
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        private float mLocalTickTime = 1.0f;

        private int mLastLeftTime = -1;

        protected override void _OnUpdate(float delta)
        {
            if (eLoadingStatus.Vote == mLoadingStatus)
            {
                mLocalTickTime += delta;

                if (mLocalTickTime > 1.0f)
                {
                    int leftTime = BattleMain.instance.GetDungeonStatistics().GetMatchPlayerVoteTimeLeft() / 1000 + 1;

                    if (leftTime >= 9)
                    {
                        leftTime = 9;
                    }

                    if (mLastLeftTime != leftTime)
                    {
                        mCountEffect.TriggerAudio(12);
                        mCountDownNum.SetTextNumber(leftTime); 
                        mLastLeftTime = leftTime;
                    }
                    
                    mLocalTickTime -= 1.0f;
                }
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Loading/PVPLoading/DungeonPVPLoadingFrame";
        }

        public IEnumerator UpdateProgress()
        {
            while (_targetProgress <= 100)
            {
                while (_currentProgress < _targetProgress)
                {
                    _currentProgress += _UpdateSpeed;
                    if (_currentProgress > _targetProgress)
                    {
                        _currentProgress = _targetProgress;
                    }

                    _SetProgress(_currentProgress);
                    yield return Yielders.EndOfFrame;
                }

                if (_targetProgress == 100)
                {
                    break;
                }

                yield return Yielders.EndOfFrame;

                _targetProgress = (int)(ClientSystemManager.GetInstance().SwitchProgress * 100.0f);
            }
        }

        protected void _SetProgress(int progress)
        {
            if (progress < 0)
            {
                progress = 0;
            }
            if (progress > 100)
            {
                progress = 100;
            }
        }
    }
}
