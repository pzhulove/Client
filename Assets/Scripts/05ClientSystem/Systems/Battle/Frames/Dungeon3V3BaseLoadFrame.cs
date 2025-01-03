using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;
using DG.Tweening;

namespace GameClient
{
    public class Dungeon3V3BaseLoadFrame : ClientFrame 
    {
        protected enum eAnimateType
        {
            eIn,
            eOut,
            eSelected,
        }

        protected const int kMaxPlayerSeatCount = 6;

        protected class MatchUnit
        {
            public ComCommonBind board   = null;
            public ComCommonBind unit    = null;
            public GameObject    root    = null;
            public byte          playerSeat = byte.MaxValue;

            public void Clear()
            {
                if (null != board)
                {
                    board.ClearAllCacheBinds();
                }

                if (null != unit)
                {
                    unit.ClearAllCacheBinds();
                }

                board      = null;
                unit       = null;
                root       = null;
                playerSeat = byte.MaxValue;
            }
        }

        protected MatchUnit[] mBoards = new MatchUnit[kMaxPlayerSeatCount];

        private float[] mBoardsAlpha = new float[] { 0.88f, 0.60f, 0.3f };

        protected List<List<DOTweenAnimation>> mAllTypeAnimation = new List<List<DOTweenAnimation>>();

#region ExtraUIBind
        protected Button mApplyBtn = null;
        protected Image mFightVS = null;
        protected ImageNumber mCountDownNum = null;
        protected GameObject mCountDownRoot = null;
        protected GameObject mRedTeamRoot = null;
        protected GameObject mCancelFightRoot = null;
        protected GameObject mJoinFightRoot = null;
        protected GameObject mLeftTimeRoot = null;
        protected GameObject mSeat0 = null;
        protected GameObject mSeat1 = null;
        protected GameObject mSeat2 = null;
        protected GameObject mSeat3 = null;
        protected GameObject mSeat4 = null;
        protected GameObject mSeat5 = null;
        protected GameObject mFightTips = null;
        protected UIAudioProxy mCountEffect = null;
        protected ComTimeLimitButton mApplyLimitButton = null;
        protected DOTweenAnimation mDt03_01 = null;
        protected DOTweenAnimation mDt03_02 = null;
        protected DOTweenAnimation mDt03_03 = null;
        protected DOTweenAnimation mDt03_04 = null;
        protected DOTweenAnimation mDt03_05 = null;
        protected DOTweenAnimation mDt02_01 = null;
        protected DOTweenAnimation mDt02_02 = null;
        protected DOTweenAnimation mDt02_03 = null;
        protected DOTweenAnimation mDt02_04 = null;
        protected DOTweenAnimation mDt02_05 = null;
        protected DOTweenAnimation mDt01_01 = null;
        protected DOTweenAnimation mDt01_02 = null;
        protected DOTweenAnimation mDt01_03 = null;
        protected DOTweenAnimation mDt01_04 = null;
        protected DOTweenAnimation mDt01_05 = null;
        protected DOTweenAnimation mDt06_01 = null;
        protected DOTweenAnimation mDt06_02 = null;
        protected DOTweenAnimation mDt06_03 = null;
        protected DOTweenAnimation mDt06_04 = null;
        protected DOTweenAnimation mDt06_05 = null;
        protected DOTweenAnimation mDt05_01 = null;
        protected DOTweenAnimation mDt05_02 = null;
        protected DOTweenAnimation mDt05_03 = null;
        protected DOTweenAnimation mDt05_04 = null;
        protected DOTweenAnimation mDt05_05 = null;
        protected DOTweenAnimation mDt04_01 = null;
        protected DOTweenAnimation mDt04_02 = null;
        protected DOTweenAnimation mDt04_03 = null;
        protected DOTweenAnimation mDt04_04 = null;
        protected DOTweenAnimation mDt04_05 = null;
        protected GameObject mNextRoundRoot = null;
        protected GameObject mPerpareRoot = null;
        protected Image mNextRoundImage = null;
        protected GameObject mBlueTeamRoot = null;
        protected Button mPvp3v3MicRoomBtn = null;
        protected Image mPvp3v3MicRoomBtnBg = null;
        protected Image mPvp3v3MicRoomBtnClose = null;
        protected Button mPvp3v3PlayerBtn = null;
        protected Image mPvp3v3PlayerBtnBg = null;
        protected Image mPvp3v3PlayerBtnClose = null;

        protected override void _bindExUI()
        {
            mApplyBtn = mBind.GetCom<Button>("applyBtn");
            mApplyBtn.onClick.AddListener(_onApplyBtnButtonClick);
            mFightVS = mBind.GetCom<Image>("fightVS");
            mCountDownNum = mBind.GetCom<ImageNumber>("countDownNum");
            mCountDownRoot = mBind.GetGameObject("countDownRoot");
            mRedTeamRoot = mBind.GetGameObject("redTeamRoot");
            mCancelFightRoot = mBind.GetGameObject("cancelFightRoot");
            mJoinFightRoot = mBind.GetGameObject("joinFightRoot");
            mLeftTimeRoot = mBind.GetGameObject("leftTimeRoot");
            mSeat0 = mBind.GetGameObject("seat0");
            mSeat1 = mBind.GetGameObject("seat1");
            mSeat2 = mBind.GetGameObject("seat2");
            mSeat3 = mBind.GetGameObject("seat3");
            mSeat4 = mBind.GetGameObject("seat4");
            mSeat5 = mBind.GetGameObject("seat5");
            mFightTips = mBind.GetGameObject("fightTips");
            mCountEffect = mBind.GetCom<UIAudioProxy>("countEffect");
            mApplyLimitButton = mBind.GetCom<ComTimeLimitButton>("applyLimitButton");
            mDt03_01 = mBind.GetCom<DOTweenAnimation>("dt03_01");
            mDt03_02 = mBind.GetCom<DOTweenAnimation>("dt03_02");
            mDt03_03 = mBind.GetCom<DOTweenAnimation>("dt03_03");
            mDt03_04 = mBind.GetCom<DOTweenAnimation>("dt03_04");
            mDt03_05 = mBind.GetCom<DOTweenAnimation>("dt03_05");
            mDt02_01 = mBind.GetCom<DOTweenAnimation>("dt02_01");
            mDt02_02 = mBind.GetCom<DOTweenAnimation>("dt02_02");
            mDt02_03 = mBind.GetCom<DOTweenAnimation>("dt02_03");
            mDt02_04 = mBind.GetCom<DOTweenAnimation>("dt02_04");
            mDt02_05 = mBind.GetCom<DOTweenAnimation>("dt02_05");
            mDt01_01 = mBind.GetCom<DOTweenAnimation>("dt01_01");
            mDt01_02 = mBind.GetCom<DOTweenAnimation>("dt01_02");
            mDt01_03 = mBind.GetCom<DOTweenAnimation>("dt01_03");
            mDt01_04 = mBind.GetCom<DOTweenAnimation>("dt01_04");
            mDt01_05 = mBind.GetCom<DOTweenAnimation>("dt01_05");
            mDt06_01 = mBind.GetCom<DOTweenAnimation>("dt06_01");
            mDt06_02 = mBind.GetCom<DOTweenAnimation>("dt06_02");
            mDt06_03 = mBind.GetCom<DOTweenAnimation>("dt06_03");
            mDt06_04 = mBind.GetCom<DOTweenAnimation>("dt06_04");
            mDt06_05 = mBind.GetCom<DOTweenAnimation>("dt06_05");
            mDt05_01 = mBind.GetCom<DOTweenAnimation>("dt05_01");
            mDt05_02 = mBind.GetCom<DOTweenAnimation>("dt05_02");
            mDt05_03 = mBind.GetCom<DOTweenAnimation>("dt05_03");
            mDt05_04 = mBind.GetCom<DOTweenAnimation>("dt05_04");
            mDt05_05 = mBind.GetCom<DOTweenAnimation>("dt05_05");
            mDt04_01 = mBind.GetCom<DOTweenAnimation>("dt04_01");
            mDt04_02 = mBind.GetCom<DOTweenAnimation>("dt04_02");
            mDt04_03 = mBind.GetCom<DOTweenAnimation>("dt04_03");
            mDt04_04 = mBind.GetCom<DOTweenAnimation>("dt04_04");
            mDt04_05 = mBind.GetCom<DOTweenAnimation>("dt04_05");
            mNextRoundRoot = mBind.GetGameObject("nextRoundRoot");
            mPerpareRoot = mBind.GetGameObject("perpareRoot");
            mNextRoundImage = mBind.GetCom<Image>("nextRoundImage");
            mBlueTeamRoot = mBind.GetGameObject("blueTeamRoot");
            // mPvp3v3MicRoomBtn = mBind.GetCom<Button>("pvp3v3MicRoomBtn");
            // mPvp3v3MicRoomBtn.onClick.AddListener(_onPvp3v3MicRoomBtnButtonClick);
            // mPvp3v3MicRoomBtnBg = mBind.GetCom<Image>("pvp3v3MicRoomBtnBg");
            // mPvp3v3MicRoomBtnClose = mBind.GetCom<Image>("pvp3v3MicRoomBtnClose");
            // mPvp3v3PlayerBtn = mBind.GetCom<Button>("pvp3v3PlayerBtn");
            // mPvp3v3PlayerBtn.onClick.AddListener(_onPvp3v3PlayerBtnButtonClick);
            // mPvp3v3PlayerBtnBg = mBind.GetCom<Image>("pvp3v3PlayerBtnBg");
            // mPvp3v3PlayerBtnClose = mBind.GetCom<Image>("pvp3v3PlayerBtnClose");
        }

        protected override void _unbindExUI()
        {
            mApplyBtn.onClick.RemoveListener(_onApplyBtnButtonClick);
            mApplyBtn = null;
            mFightVS = null;
            mCountDownNum = null;
            mCountDownRoot = null;
            mRedTeamRoot = null;
            mCancelFightRoot = null;
            mJoinFightRoot = null;
            mLeftTimeRoot = null;
            mSeat0 = null;
            mSeat1 = null;
            mSeat2 = null;
            mSeat3 = null;
            mSeat4 = null;
            mSeat5 = null;
            mFightTips = null;
            mCountEffect = null;
            mApplyLimitButton = null;
            mDt03_01 = null;
            mDt03_02 = null;
            mDt03_03 = null;
            mDt03_04 = null;
            mDt03_05 = null;
            mDt02_01 = null;
            mDt02_02 = null;
            mDt02_03 = null;
            mDt02_04 = null;
            mDt02_05 = null;
            mDt01_01 = null;
            mDt01_02 = null;
            mDt01_03 = null;
            mDt01_04 = null;
            mDt01_05 = null;
            mDt06_01 = null;
            mDt06_02 = null;
            mDt06_03 = null;
            mDt06_04 = null;
            mDt06_05 = null;
            mDt05_01 = null;
            mDt05_02 = null;
            mDt05_03 = null;
            mDt05_04 = null;
            mDt05_05 = null;
            mDt04_01 = null;
            mDt04_02 = null;
            mDt04_03 = null;
            mDt04_04 = null;
            mDt04_05 = null;
            mNextRoundRoot = null;
            mPerpareRoot = null;
            mNextRoundImage = null;
            mBlueTeamRoot = null;
            //mPvp3v3MicRoomBtn.onClick.RemoveListener(_onPvp3v3MicRoomBtnButtonClick);
            //mPvp3v3MicRoomBtn = null;
            //mPvp3v3MicRoomBtnBg = null;
            //mPvp3v3MicRoomBtnClose = null;
            // mPvp3v3PlayerBtn.onClick.RemoveListener(_onPvp3v3PlayerBtnButtonClick);
            // mPvp3v3PlayerBtn = null;
            // mPvp3v3PlayerBtnBg = null;
            // mPvp3v3PlayerBtnClose = null;
        }
#endregion  

#region Callback
        private void _onApplyBtnButtonClick()
        {
            _onApplyBtnButtonClickEvent();
        }
        private void _onPvp3v3MicRoomBtnButtonClick()
        {
            ComVoiceTalk.ControlMic();
        }
        private void _onPvp3v3PlayerBtnButtonClick()
        {
            ComVoiceTalk.ControlPlayer();
        }
#endregion

        protected virtual void _onApplyBtnButtonClickEvent()
        {

        }

        protected float _playProcessAnimateByType(eAnimateType type, byte seat)
        {
            float time = 0.0f;


            _killAll((int)seat);
        
            switch(type)
            {
                case eAnimateType.eIn:
                    //_playAnimation(_findPlayerIndexBySeat(seat), 0);
                    break;
                case eAnimateType.eOut:
                    _playAnimation(_findPlayerIndexBySeat(seat), 3);
                    _playAnimation(_findPlayerIndexBySeat(seat), 4);
                    break;
                case eAnimateType.eSelected:
                    _playAnimation(_findPlayerIndexBySeat(seat), 1);
                    _playAnimation(_findPlayerIndexBySeat(seat), 2);
                    break;
            }

            return time;
        }

        private void _killAll(int seat)
        {
            if (seat < 0 || seat >= mBoards.Length)
            {
                return;
            }

            if (null == mBoards[seat] || null == mBoards[seat].root)
            {
                return ;
            }

            //mBoards[seat].root.Kill();
            //mBoards[seat].root.CreateTween();
        }

        private float _playAnimation(int fst, int snd)
        {
            if (fst < 0 || fst >= mAllTypeAnimation.Count)
            {
                return 0.0f;
            }

            List<DOTweenAnimation> curSeatAniamtes = mAllTypeAnimation[fst];

            if (null == curSeatAniamtes)
            {
                return 0.0f;
            }

            if (snd < 0 || snd >= curSeatAniamtes.Count)
            {
                return 0.0f;
            }

            DOTweenAnimation animate = curSeatAniamtes[snd];

            if (null == animate)
            {
                return 0.0f;
            }

            animate.tween.Rewind();
            animate.tween.Kill();

            if (animate.isValid) 
            {
                animate.CreateTween();
                animate.tween.Play();
            }

            if (animate.loops < 0)
            {
                return -1.0f;
            }


            return animate.delay + animate.duration * animate.loops;
        }


        protected MatchUnit _findBoardBySeat(byte seat)
        {
            int idx = _findPlayerIndexBySeat(seat);

            if (idx < 0 || idx >= mBoards.Length)
            {
                return null;
            }

            return mBoards[idx];
        }

        protected int _findPlayerIndexBySeat(byte seat)
        {
            for (int i = 0; i < mBoards.Length; ++i)
            {
                if (mBoards[i].playerSeat == seat)
                {
                    return i;
                }
            }

            return -1;
        }

        protected void _initBoards()
        {
            _initBoardsArray();
            _initDTAnimateArray();
           
            string blueBoardPath = mBind.GetPrefabPath("blueboardUnit");
            string redBoardPath = mBind.GetPrefabPath("redboardUnit");

            mBind.ClearCacheBinds(blueBoardPath);
            mBind.ClearCacheBinds(redBoardPath);

            for (int i = 0; i < kMaxPlayerSeatCount; ++i)
            {
                if (i < kMaxPlayerSeatCount / 2)
                {
                    mBoards[i].board = mBind.LoadExtraBind(redBoardPath);
                }
                else
                {
                    mBoards[i].board = mBind.LoadExtraBind(blueBoardPath);
                }

                ComCommonBind bind = mBoards[i].board;

                Color alphaWhite = new Color(1.0f, 1.0f, 1.0f, mBoardsAlpha[i % mBoardsAlpha.Length]);

                Image top = bind.GetCom<Image>("top");
                Image bottom = bind.GetCom<Image>("bottom");

                if (null != top)
                {
                    top.color    *= alphaWhite;
                }

                if (null != bottom)
                {
                    bottom.color *= alphaWhite;
                }
                
                if (null == bind)
                {
                    return ;
                }

                Utility.AttachTo(bind.gameObject, mBoards[i].root);
            }
        }

        private void _initDTAnimateArray()
        {
            mAllTypeAnimation.Clear();
            for (int i = 0; i < kMaxPlayerSeatCount; ++i)
            {
                List<DOTweenAnimation> animations = new List<DOTweenAnimation>();
                mAllTypeAnimation.Add(animations);
            }

            mAllTypeAnimation[0].Clear();
            mAllTypeAnimation[0].Add(mDt01_01);
            mAllTypeAnimation[0].Add(mDt01_02);
            mAllTypeAnimation[0].Add(mDt01_03);
            mAllTypeAnimation[0].Add(mDt01_04);
            mAllTypeAnimation[0].Add(mDt01_05);

            mAllTypeAnimation[1].Clear();
            mAllTypeAnimation[1].Add(mDt02_01);
            mAllTypeAnimation[1].Add(mDt02_02);
            mAllTypeAnimation[1].Add(mDt02_03);
            mAllTypeAnimation[1].Add(mDt02_04);
            mAllTypeAnimation[1].Add(mDt02_05);

            mAllTypeAnimation[2].Clear();
            mAllTypeAnimation[2].Add(mDt03_01);
            mAllTypeAnimation[2].Add(mDt03_02);
            mAllTypeAnimation[2].Add(mDt03_03);
            mAllTypeAnimation[2].Add(mDt03_04);
            mAllTypeAnimation[2].Add(mDt03_05);

            mAllTypeAnimation[3].Clear();
            mAllTypeAnimation[3].Add(mDt04_01);
            mAllTypeAnimation[3].Add(mDt04_02);
            mAllTypeAnimation[3].Add(mDt04_03);
            mAllTypeAnimation[3].Add(mDt04_04);
            mAllTypeAnimation[3].Add(mDt04_05);

            mAllTypeAnimation[4].Clear();
            mAllTypeAnimation[4].Add(mDt05_01);
            mAllTypeAnimation[4].Add(mDt05_02);
            mAllTypeAnimation[4].Add(mDt05_03);
            mAllTypeAnimation[4].Add(mDt05_04);
            mAllTypeAnimation[4].Add(mDt05_05);

            mAllTypeAnimation[5].Clear();
            mAllTypeAnimation[5].Add(mDt06_01);
            mAllTypeAnimation[5].Add(mDt06_02);
            mAllTypeAnimation[5].Add(mDt06_03);
            mAllTypeAnimation[5].Add(mDt06_04);
            mAllTypeAnimation[5].Add(mDt06_05);
        }

        private void _initBoardsArray()
        {
            for (int i = 0; i < kMaxPlayerSeatCount; ++i)
            {
                mBoards[i] = new MatchUnit();
            }

            mBoards[0].root = mSeat0;
            mBoards[1].root = mSeat1;
            mBoards[2].root = mSeat2;
            mBoards[3].root = mSeat3;
            mBoards[4].root = mSeat4;
            mBoards[5].root = mSeat5;

            for (int i = 0; i < kMaxPlayerSeatCount; ++i)
            {
                mBoards[i].root.CustomActive(false);
            }
        }

        protected void _uninitBoards()
        {
            for (int i = 0; i < mBoards.Length; ++i)
            {
                if (null != mBoards[i])
                {
                    mBoards[i].Clear();
                }
            }

            mAllTypeAnimation.Clear();
        }

        private int mRedTeamIndex  = 0;
        private int mBlueTeamIndex = kMaxPlayerSeatCount / 2;

        protected void _initPlayers()
        {
            string bluePath = mBind.GetPrefabPath("blueUnit");
            string redPath = mBind.GetPrefabPath("redUnit");
            mBind.ClearCacheBinds(bluePath);
            mBind.ClearCacheBinds(redPath);

            _initBlueRedTeamIndex();
            _initPlayersSeat();

            List<BattlePlayer> players = BattleMain.instance.GetPlayerManager().GetAllPlayers();

            for (int i = 0; i < players.Count; ++i)
            {
                _initOnePlayer(players[i]);
            }
        }

        private void _initPlayersSeat()
        {
            List<BattlePlayer> players = BattleMain.instance.GetPlayerManager().GetAllPlayers();

            for (int i = 0; i < players.Count; ++i)
            {
                int seatIdx = _generatePlayerIndex(players[i]);

                if (seatIdx >= 0 && seatIdx < kMaxPlayerSeatCount)
                {
                    mBoards[seatIdx].playerSeat = players[i].GetPlayerSeat();
                }
                else
                {
                    Logger.LogErrorFormat("[3v3] {0} 座位号索引超过范围", seatIdx);
                }
            }
        }

        private void _initBlueRedTeamIndex()
        {
            mRedTeamIndex  = 0;
            mBlueTeamIndex = kMaxPlayerSeatCount / 2;
        }

        private int _generatePlayerIndex(BattlePlayer player)
        {
            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return -1;
            }

            if (player.IsTeamRed())
            {
                return mRedTeamIndex++;
            }
            else
            {
                return mBlueTeamIndex++;
            }
        }

        private void _initOnePlayer(BattlePlayer player)
        {
            //string unitPath     = mBind.GetPrefabPath("unit");

            string bluePath = mBind.GetPrefabPath("blueUnit");
            string redPath = mBind.GetPrefabPath("redUnit");

            string unitPath = player.IsTeamRed() ? redPath : bluePath;


            MatchUnit matchUnit = _findBoardBySeat(player.playerInfo.seat);

            if (null == matchUnit)
            {
                return ;
            }

            ComCommonBind board = matchUnit.board;
            ComCommonBind bind  = mBind.LoadExtraBind(unitPath);

            if (null == bind)
            {
                return ;
            }

            GameObject root = board.GetGameObject("unitRoot");

            matchUnit.root.CustomActive(true);

            matchUnit.unit       = bind;

            Utility.AttachTo(bind.gameObject, root);

            ComDungeonPlayerLoadProgress matchPlayerLoading = bind.GetCom<ComDungeonPlayerLoadProgress>("matchPlayerLoading");
            GameObject charactorRoot                        = bind.GetGameObject("charactorRoot");
            Image      num                                  = board.GetCom<Image>("num");

            if (null != num && null != num.transform.parent)
            {
                num.transform.parent.SetAsLastSibling();
            }

            mBind.GetSprite(string.Format("num{0}", (int)_findPlayerIndexBySeat(matchUnit.playerSeat) % (kMaxPlayerSeatCount / 2)), ref num);


            matchPlayerLoading.SetSeat(player.playerInfo.seat);

            _loadCharactorMatchPrefab(player, charactorRoot);
        }

        private bool _loadCharactorMatchPrefab(BattlePlayer player, GameObject parent)
        {
            if (!_checkBattlePlayerIsValid(player))
            {
                return false;
            }

            int id = (int)player.playerInfo.occupation;

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(id);
            if (null == jobData)
            {
                return false;
            }

            GameObject root = AssetLoader.instance.LoadResAsGameObject(jobData.PKMatchShowPrefab);
            if (null == root)
            {
                return false;
            }

            Utility.AttachTo(root, parent);

            ComPK3V3LoadingCharactorPosition position = root.GetComponentInChildren<ComPK3V3LoadingCharactorPosition>();
            if (null == position)
            {
                return false;
            }

            position.SetTeamType(player.teamType);
            return true;
        }

        protected bool _checkBattlePlayerIsValid(BattlePlayer player)
        {
            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return false;
            }

            return null != _findBoardBySeat(player.playerInfo.seat);
        }
    }
}
