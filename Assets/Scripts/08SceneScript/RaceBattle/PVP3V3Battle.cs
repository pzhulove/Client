using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Protocol;
using Network;

namespace GameClient
{
    public class PVP3V3Battle : BaseMatchPVPBattle
    {
        private enum ePVP3V3ProcessStatus
        {
            /// <summary>
            /// 加载
            /// </summary>
            None,

            /// <summary>
            /// 第一轮投票
            ///
            /// 投票
            ///
            /// 决定双方出战人员
            ///
            /// 加载双方出战人员
            ///
            /// 进入 RoundStart 阶段
            ///
            /// </summary>
            FirstVote,

            /// <summary>
            /// 等待投票结束的动画
            /// </summary>
            WaitFirstVote,

            /// <summary>
            /// 等待投票VS出现
            /// </summary>
            WaitFirstVoteVSFlag,

            /// <summary>
            /// 战斗开始的预备阶段
            /// </summary>
            PrepareRoundStart,

            /// <summary>
            /// 显示PKloading中
            /// </summary>
            DuringLoadingRoundStart,

            /// <summary>
            /// 显示PKloading
            /// </summary>
            LodingRoundStart,

            /// <summary>
            /// 开始一轮战斗
            ///
            /// 播放3，2，1倒计时，
            ///
            /// 限制玩家移动
            /// </summary>
            RoundStart,

            /// <summary>
            /// 限制玩家移动时期间
            /// </summary>
            WaitFight,

            /// <summary>
            /// 战斗
            /// </summary>
            Fight,

            /// <summary>
            /// 计算本轮结果
            /// </summary>
            GetRoundEndResult,

            /// <summary>
            /// 计算本轮结果
            /// </summary>
            WaitRoundEndResult,

            /// <summary>
            /// 结算动画结束后的，提示时间 
            /// </summary>
            WaitTipRoundEndResult,

            /// <summary>
            /// 一轮结束
            /// </summary>
            RoundEnd,

            /// <summary>
            /// 结算
            /// </summary>
            GetRaceEndResult,


            WaitRaceEndResult,

            /// <summary>
            /// 结算
            /// </summary>
            RaceEnd,

            Finish,
        }

        /// <summary>
        /// 投票时间
        /// </summary>
        private const int kFirstVoteTime = 10 * 1000;

        private const int kWaitFirstVoteTime                 = 8   * 1000 - kWaitFirstVoteVSTime;
        private const int kWaitFirstVoteVSTime               = kWaitVSPkLoadingTime;

        /// <summary>
        /// PKLoaindg显示角色的时间 
        /// </summary>
        private const int kWaitPkLoadingTime   = 8 * 1000 - kWaitVSPkLoadingTime;
        private const int kWaitVSPkLoadingTime = 3800;

        /// <summary>
        /// PK战斗的时间 秒数
        /// </summary>
        private const int kFightTimeInSec = Global.PK_TOTAL_TIME;

        /// <summary>
        /// PK战斗的时间 毫秒数
        /// </summary>
        private const int kFightTime = kFightTimeInSec * 1000;

        /// <summary>
        /// 积分赛时间
        /// </summary>
        private const int scoreWarFightTime = 3 * 60;


        /// <summary>
        /// 等待结果的时间
        /// </summary>
        private const int kRoundResultTime = 2 * 1000;


        private const int kTipRoundEndResultTime = 2 * 1000;

        /// <summary>
        /// 比赛结束的时间 
        /// </summary>
        private const int kEndResultTime = 2 * 1000;

        /// <summary>
        /// 开始战斗倒计时 3-2-1 的时间
        /// </summary>
        private const int kWaitForFightTimeInSec = 3;

        private ePVP3V3ProcessStatus mCurPVP3V3ProcessStatus = ePVP3V3ProcessStatus.None;
        private int mTickTime = 0;
        private ePVPBattleEndType mMatchPlayerEndType = ePVPBattleEndType.onNone;
        private PKResult mRedTeamPKResult = PKResult.INVALID;
        private PKResult mBlueTeamPKResult = PKResult.INVALID;

        private List<BattlePlayer> mCurrentRedTeamPendingFightPlayers = new List<BattlePlayer>();
        private List<BattlePlayer> mCurrentBlueTeamPendingFightPlayers = new List<BattlePlayer>();

        private BattlePlayer mNextRedTeamFighingPlayer = null;
        private BattlePlayer mNextBlueTeamFighingPlayer = null;

        private List<BattlePlayer> mDeadFightingPlayers = new List<BattlePlayer>();


        private ePVP3V3ProcessStatus curPVP3V3ProcessStatus
        {
            get
            {
                return mCurPVP3V3ProcessStatus;
            }

            set
            {
                Logger.LogProcessFormat("[战斗] [3v3] 状态 {0} -> {1}", mCurPVP3V3ProcessStatus, value);

                //try 
                //{
                //    _fromStatus2Status(mCurPVP3V3ProcessStatus, value);
                //}
                //catch (System.Exception e)
                //{
                //    Logger.LogErrorFormat("[战斗] [3v3] 出错 {0}", e.ToString());
                //}

                mCurPVP3V3ProcessStatus = value;
            }
        }


        public PVP3V3Battle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id) { }

        int mDuringLoadingCnt = 0;
        bool mHasPreloadFlag = false;
        bool mHasGCCollect = false;

        /// <summary>
        /// 这里是本地时间的Update走一下非逻辑相关的东西
        /// </summary>
        /// <param name="delta"></param>
        public override void Update(int delta)
        {
            base.Update(delta);

            switch (curPVP3V3ProcessStatus)
            {
                case ePVP3V3ProcessStatus.WaitFirstVoteVSFlag:
                case ePVP3V3ProcessStatus.LodingRoundStart:
                    if (!mHasGCCollect)
                    {
                        mHasGCCollect = true;
                        // 第二轮开始GC
                        if (_getMatchRoundIndex() > 0)
                        {
                            //CGameObjectPool.instance.ExecuteClearPooledObjects();
                            //System.GC.Collect();

                            AssetGabageCollector.instance.ClearUnusedAsset();
                            //Logger.LogError("gc");
                        }
                    }
                    mDuringLoadingCnt++;
                    // GC 要等3帧，这里多等个3帧，
                    if (mDuringLoadingCnt >= 6 && !mHasPreloadFlag)
                    {
                        mHasPreloadFlag = true;

                        CResPreloader.instance.Clear();
                        PreloadManager.ClearCache();

                        BattlePlayer blue = mDungeonPlayers.GetCurrentTeamFightingPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
                        BeActionFrameMgr frameMgr = null;
                        SkillFileListCache fileCache = null;
                        if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
                        {
                            frameMgr = mDungeonManager.GetBeScene().ActionFrameMgr;
                            fileCache = mDungeonManager.GetBeScene().SkillFileCache;
                        }
                        if (BattlePlayer.IsDataValidBattlePlayer(blue))
                        {
                            PreloadManager.PreloadActor(blue.playerActor, frameMgr, fileCache);
                            //Logger.LogError("blue");
                        }
                        BattlePlayer red = mDungeonPlayers.GetCurrentTeamFightingPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                        if (BattlePlayer.IsDataValidBattlePlayer(red))
                        {
                            PreloadManager.PreloadActor(red.playerActor, frameMgr, fileCache);
                            //Logger.LogError("red");
                        }

                        CResPreloader.instance.DoPreLoadAsync(false, true);
                    }
                    break;
            }
        }

        /// <summary>
        /// 帧同步的Update
        /// </summary>
        public override void FrameUpdate(int delta)
        {
            base.FrameUpdate(delta);
            switch (curPVP3V3ProcessStatus)
            {
                case ePVP3V3ProcessStatus.None:
                    break;
                case ePVP3V3ProcessStatus.FirstVote:

                    if (!_isTimeUp(delta))
                    {
                        if (_isRaceEnd())
                        {
                            curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.GetRaceEndResult;
                            _closePVP3V3LoadingFrame();
                            break;
                        }

                        mNextBlueTeamFighingPlayer = _pickNextTeamFightPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
                        mNextRedTeamFighingPlayer = _pickNextTeamFightPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);

                        if (!_showUpFightingCharactor())
                        {
                            Logger.LogErrorFormat("[战斗] [3v3] 创建下一场玩家失败 本伦比赛直接结束");
                            curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.GetRaceEndResult;
                        }
                        else
                        {

                            mTickTime = kWaitFirstVoteTime;
                            curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.WaitFirstVote;
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3FinishVoteForFight);
                    }
                    dungeonStatistics.SetMatchPlayerVoteTimeLeft(mTickTime);
                    break;
                case ePVP3V3ProcessStatus.WaitFirstVote:
                    if (!_isTimeUp(delta))
                    {
                        mTickTime = kWaitFirstVoteVSTime;
                        mHasPreloadFlag = false;
                        mHasGCCollect = true;
                        mDuringLoadingCnt = 6;
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.WaitFirstVoteVSFlag;
                    }
                    dungeonStatistics.SetMatchPlayerVoteTimeLeft(0);
                    break;
                case ePVP3V3ProcessStatus.WaitFirstVoteVSFlag:
                    if (!_isTimeUp(delta))
                    {
                        Logger.LogProcessFormat("[战斗] [3v3] 第 {0} 轮投票结束, 关闭loading界面, 显示对决界面（pkloading)", _getMatchRoundIndex());
                        _closePVP3V3LoadingFrame();

                        mTickTime = 0;

                        _setDeadBattlePlayerRemoveTime2Zero();
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.LodingRoundStart;
                    }
                    dungeonStatistics.SetMatchPlayerVoteTimeLeft(0);
                    break;
                case ePVP3V3ProcessStatus.PrepareRoundStart:
                    // 移除过门不带过去的实体
                    _sceneEntityRemoveAll();

                    if (!_showUpFightingCharactor())
                    {
                        Logger.LogErrorFormat("[战斗] [3v3] 创建第 {0} 轮玩家失败 本伦比赛直接结束", _getMatchRoundIndex());
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.GetRaceEndResult;
                    }
                    else
                    {
                        Logger.LogProcessFormat("[战斗] [3v3] 第 {0} 轮, 显示对决界面（pkloading)", _getMatchRoundIndex());

                        mTickTime = kWaitPkLoadingTime;
                        _setDungeonReadyFightFrame(true);
                        _setDeadBattlePlayerRemoveTime2Zero();

                        mDuringLoadingCnt = 0;
                        mHasPreloadFlag   = false;
                        mHasGCCollect     = false;

                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.DuringLoadingRoundStart;
                    }
                    break;
                case ePVP3V3ProcessStatus.DuringLoadingRoundStart:
                    // 这个状态是所有动画都播放完成了

                    if (!_isTimeUp(delta))
                    {
                        mTickTime = kWaitVSPkLoadingTime;
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.LodingRoundStart;
                    }
                    break;
                case ePVP3V3ProcessStatus.LodingRoundStart:
                    if (!_isTimeUp(delta))
                    {
                        _resetAllCharactorVoteStatus();
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3StartRedyFightCount);

                        _setDungeonReadyFightFrame(false);
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.RoundStart;
                    }
                    break;
                case ePVP3V3ProcessStatus.RoundStart:
                    curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.WaitFight;
                    mMatchPlayerEndType = ePVPBattleEndType.onNone;
                    mRedTeamPKResult = PKResult.INVALID;
                    mBlueTeamPKResult = PKResult.INVALID;
                    int fightTime = PkRaceType == (int)(Protocol.RaceType.ScoreWar) ? scoreWarFightTime : kFightTimeInSec;
                    mTickTime = fightTime * 1000;

                    Logger.LogProcessFormat("[战斗] [3v3] 开始第 {0} 轮 3-2-1 倒计时", _getMatchRoundIndex());
                    _startRoundReadyFight(kWaitForFightTimeInSec, fightTime);

                    SwitchRoundAddInvincibleBuff(mNextRedTeamFighingPlayer, true);
                    SwitchRoundAddInvincibleBuff(mNextBlueTeamFighingPlayer, true);

                    break;

                case ePVP3V3ProcessStatus.WaitFight:
                    break;
                case ePVP3V3ProcessStatus.Fight:
                    if (!_isTimeUp(delta))
                    {
                        mMatchPlayerEndType = ePVPBattleEndType.onTimeOut;
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.GetRoundEndResult;

                        _getPKResultInCurrentRound();

                        _killTeamPlayerInCurrentRoundWithTimeOut();
                    }
                    break;
                case ePVP3V3ProcessStatus.GetRoundEndResult:
                    Logger.LogProcessFormat("[战斗] [3v3] 第 {0} 轮, 有结果啦 {1}, 结果 红队:{2} 蓝队:{3} ", _getMatchRoundIndex(), mMatchPlayerEndType, mRedTeamPKResult, mBlueTeamPKResult);

                    mTickTime = kRoundResultTime;
                    curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.WaitRoundEndResult;

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3GetRoundEndResult);

                    mNextRedTeamFighingPlayer  = _pickNextTeamFightPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                    mNextBlueTeamFighingPlayer = _pickNextTeamFightPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);

                    SwitchRoundAddInvincibleBuff(mNextRedTeamFighingPlayer);
                    SwitchRoundAddInvincibleBuff(mNextBlueTeamFighingPlayer);

                    break;
                case ePVP3V3ProcessStatus.WaitRoundEndResult:
                    if (!_isTimeUp(delta))
                    {
                        mTickTime = kTipRoundEndResultTime;
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.WaitTipRoundEndResult;

                        _hiddenBattleResult();

                        if (!_isRaceEnd())
                        {
                            _showTipRoundEndResult();
                        }
                    }
                    break;
                case ePVP3V3ProcessStatus.WaitTipRoundEndResult:
                    if (!_isTimeUp(delta))
                    {
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.RoundEnd;

                        _hiddenTipRoundEndResult();
                    }
                    break;
                case ePVP3V3ProcessStatus.RoundEnd:
                    Logger.LogProcessFormat("[战斗] [3v3] 第 {0} 轮, 结束", _getMatchRoundIndex());

                    if (_isRaceEnd())
                    {
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.GetRaceEndResult;
                    }
                    else
                    {
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.PrepareRoundStart;

                        _nextMatchRoundIndex();
                    }
                    break;
                case ePVP3V3ProcessStatus.GetRaceEndResult:
                    Logger.LogProcessFormat("[战斗] [3v3] 第 {0} 轮, 最终论, 等待结算", _getMatchRoundIndex());
                    mTickTime = kEndResultTime;
                    curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.WaitRaceEndResult;

                    // 这里的结算数据都是客户端已有的
                    // 结算界面在收到服务器回复的结算消息后再打开 
                    // add by qxy 2018-09-14 22:30
                    //openDungeon3v3FinishFrame();
                    break;
                case ePVP3V3ProcessStatus.WaitRaceEndResult:
                    if (!_isTimeUp(delta))
                    {
                        curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.RaceEnd;
                    }
                    break;
                case ePVP3V3ProcessStatus.RaceEnd:
                    Logger.LogProcessFormat("[战斗] [3v3] 第 {0} 轮, 最终论, 比赛结束", _getMatchRoundIndex());

                    // 发送结算消息
                    _sendRelaySvrEndGameReq();

                    mDungeonManager.FinishFight();
                    ClientReconnectManager.instance.canReconnectRelay = false;

                    curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.Finish;
                    break;
                case ePVP3V3ProcessStatus.Finish:
                    break;
            }
        }

        private void _setDeadBattlePlayerRemoveTime2Zero()
        {
            for (int i = 0; i < mDeadFightingPlayers.Count; ++i)
            {
                if (null != mDeadFightingPlayers[i] &&
                    null != mDeadFightingPlayers[i].playerActor)
                {
                    mDeadFightingPlayers[i].playerActor.m_iRemoveTime = 0;

                    // 让本地玩家死去吧
                    mDeadFightingPlayers[i].playerActor.isLocalActor = false;
                }
            }

            mDeadFightingPlayers.Clear();
        }

        private void _showTipRoundEndResult()
        {
            BattlePlayer mainPlayer = mDungeonPlayers.GetMainPlayer();

            if (!BattlePlayer.IsDataValidBattlePlayer(mainPlayer))
            {
                return;
            }

            BattlePlayer mainPlayerTeamNextPlayer = _getNextTeamFightPlayer(mainPlayer.teamType);

            if (!BattlePlayer.IsDataValidBattlePlayer(mainPlayerTeamNextPlayer))
            {
                return;
            }

            if (!mainPlayerTeamNextPlayer.isFighting && mainPlayerTeamNextPlayer.GetPlayerSeat() == mainPlayer.GetPlayerSeat())
            {
                _showMainPlayerIsNextPlayer();
            }
        }

        private void _hiddenTipRoundEndResult()
        {
            _hiddenMainPlayerIsNextPlayer();
        }

        private void _resetAllCharactorVoteStatus()
        {
            List<BattlePlayer> players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].isVote = false;
            }
        }

        private void _setDungeonReadyFightFrame(bool isOpen)
        {
            if (isOpen)
            {
                byte redSeat = mDungeonPlayers.GetCurrentTeamFightingPlayerSeat(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                byte blueSeat = mDungeonPlayers.GetCurrentTeamFightingPlayerSeat(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);

                _openDungeonReadyFightFrame(redSeat, blueSeat);
            }
            else
            {
                _closeDungeonReadyFightFrame();
            }
        }

        private void _getPKResultInCurrentRound()
        {
            mRedTeamPKResult = _getOnePKResult(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
            mBlueTeamPKResult = _getOnePKResult(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);

            Logger.LogProcessFormat("[战斗] 获得当前队伍比赛结果 红:{0} 蓝:{1}", mRedTeamPKResult, mBlueTeamPKResult);
        }

        protected PKResult _getOnePKResult(BattlePlayer.eDungeonPlayerTeamType teamType)
        {
            BattlePlayer matchPlayer = mDungeonPlayers.GetCurrentTeamFightingPlayer(teamType);
            if (null == matchPlayer)
            {
                return PKResult.INVALID;
            }

            return _getMatchPVPResult(matchPlayer.playerInfo.seat, mMatchPlayerEndType);
        }
        public BeActor GetWinActor()
        {
            PKResult result =  _getOnePKResult(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
            if (result == PKResult.DRAW) return null;
            if(result == PKResult.LOSE)
            {
                var battlePlayer = mDungeonPlayers.GetCurrentTeamFightingPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                if (battlePlayer != null) return battlePlayer.playerActor;
                return null;
            }
            else if(result == PKResult.WIN)
            {
                var battlePlayer = mDungeonPlayers.GetCurrentTeamFightingPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
                if (battlePlayer != null) return battlePlayer.playerActor;
                return null;
            }
            //最后一次容错
            if(result == PKResult.INVALID)
            {
                result = _getOnePKResult(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                if (result == PKResult.DRAW) return null;
                if(result == PKResult.WIN)
                {
                    var battlePlayer = mDungeonPlayers.GetCurrentTeamFightingPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                    if (battlePlayer != null) return battlePlayer.playerActor;
                    return null;
                }
            }
            return null;
        }

        private bool _isRaceEnd()
        {
            bool teamRedAllFight = mDungeonPlayers.IsTeamPlayerAllFighted(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
            bool teamBlueAllFight = mDungeonPlayers.IsTeamPlayerAllFighted(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);

            Logger.LogProcessFormat("[战斗] 是否战斗已经结束了？ 红:{0} 蓝:{1}", teamRedAllFight, teamBlueAllFight);

            return teamBlueAllFight || teamRedAllFight;
        }

        private bool _showUpFightingCharactor()
        {
            if (_isRaceEnd())
            {
                return false;
            }

            _pickOneFightingCharactor2Battle(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
            _pickOneFightingCharactor2Battle(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);

            _adjustBalanceForOneFightingCharactor(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
            _adjustBalanceForOneFightingCharactor(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);


            mNextBlueTeamFighingPlayer = null;
            mNextRedTeamFighingPlayer = null;

            return true;
        }

        private bool _pickOneFightingCharactor2Battle(BattlePlayer.eDungeonPlayerTeamType type)
        {
            BattlePlayer matchPlayer = _getNextTeamFightPlayer(type);

            if (null == matchPlayer)
            {
                Logger.LogProcessFormat("[战斗] 多人匹配 无法出战下一个玩家 {0}", type);
                return false;
            }

            if (!matchPlayer.isFighting)
            {
                // TODO 待优化=。=
                _createMatchPlayer(matchPlayer);

                if (matchPlayer.IsLocalPlayer())
                {
                    // 3v3的本地玩家操作配置帧
                    _fireDoublePressConfigFrame();

                    // TODO 待优化=。=
                    _createInput();
                }

                if(matchPlayer.playerActor != null)
                {
                    matchPlayer.playerActor.SetAttackButtonState(ButtonState.RELEASE); 
                }
            }
            else
            {
                _setCharactorAtBirthPosition(matchPlayer);
                // 重置状态，移除召唤
                _resetCharactorStatus(matchPlayer);
                _limitCharactorRestrainZone(matchPlayer);
            }

            return true;
        }

        private BattlePlayer _getNextTeamFightPlayer(BattlePlayer.eDungeonPlayerTeamType type)
        {
            switch (type)
            {
                case BattlePlayer.eDungeonPlayerTeamType.eTeamBlue:
                    return mNextBlueTeamFighingPlayer;
                case BattlePlayer.eDungeonPlayerTeamType.eTeamRed:
                    return mNextRedTeamFighingPlayer;
            }

            return null;
        }

        private BattlePlayer _pickNextTeamFightPlayer(BattlePlayer.eDungeonPlayerTeamType type)
        {
            BattlePlayer matchPlayer = mDungeonPlayers.GetCurrentTeamFightingPlayer(type);

            if (null == matchPlayer)
            {
                matchPlayer = _pickNewTeamFightPlayer(type);
            }

            return matchPlayer;
        }

        private BattlePlayer _pickNewTeamFightPlayer(BattlePlayer.eDungeonPlayerTeamType type)
        {
            List<BattlePlayer> pendingPlayers = _getClearCurrentPendingFightPlayers(type);

            if (!mDungeonPlayers.GetTeamVotePlayers(pendingPlayers, type))
            {
                return null;
            }

            if (pendingPlayers.Count > 0)
            {
                return _randPendingFightPlayers2Fight(pendingPlayers);
            }

            if (!mDungeonPlayers.GetTeamNotVotePlayers(pendingPlayers, type))
            {
                return null;
            }

            if (pendingPlayers.Count > 0)
            {
                return pendingPlayers[0];
            }

            return null;
        }

        private BattlePlayer _randPendingFightPlayers2Fight(List<BattlePlayer> pendingPlayers)
        {
            if (null == pendingPlayers || pendingPlayers.Count <= 0)
            {
                return null;
            }

            int randIdx = 0;
            int cnt = pendingPlayers.Count;

            if (null != FrameRandom)
            {
                randIdx = FrameRandom.Random((uint)cnt) % cnt;
            }

            return pendingPlayers[randIdx];
        }

        private List<BattlePlayer> _getClearCurrentPendingFightPlayers(BattlePlayer.eDungeonPlayerTeamType type)
        {
            List<BattlePlayer> pendingPlayers = null;
            switch (type)
            {
                case BattlePlayer.eDungeonPlayerTeamType.eTeamBlue:
                    pendingPlayers = mCurrentBlueTeamPendingFightPlayers;
                    break;
                case BattlePlayer.eDungeonPlayerTeamType.eTeamRed:
                    pendingPlayers = mCurrentRedTeamPendingFightPlayers;
                    break;
            }

            if (null == pendingPlayers)
            {
                pendingPlayers = new List<BattlePlayer>();
            }

            pendingPlayers.Clear();

            return pendingPlayers;
        }


        private bool _adjustBalanceForOneFightingCharactor(BattlePlayer.eDungeonPlayerTeamType type)
        {
            if (_isRaceEnd())
            {
                return false;
            }

            BattlePlayer matchPlayer = mDungeonPlayers.GetCurrentTeamFightingPlayer(type);
            if (!BattlePlayer.IsDataValidBattlePlayer(matchPlayer))
            {
                return false;
            }

            if (PKResult.INVALID != matchPlayer.GetLastPKResult())
            {
                return false;
            }

            return _adjustBalanceMatchPlayer(matchPlayer);
        }

        private void _closePVP3V3LoadingFrame()
        {
#if !LOGIC_SERVER
            ClientSystemManager.instance.CloseFrame<Dungeon3v3LoadingFrame>();
#endif
        }

        private bool _killTeamPlayerInCurrentRoundWithTimeOut()
        {
            if (mMatchPlayerEndType != ePVPBattleEndType.onTimeOut)
            {
                Logger.LogErrorFormat("[战斗] 错误比赛结束状态 {0}", mMatchPlayerEndType);
                return false;
            }

            Logger.LogProcessFormat("[战斗] 第 {0} 轮 超时杀死", _getMatchRoundIndex());

            if (_isPkResultCanKillPlayerWithTimeOut(mRedTeamPKResult))
            {
                Logger.LogProcessFormat("[战斗] 强制杀死的不是比尔 =。= 结果{0}", mRedTeamPKResult);
                _killOneTeamPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
            }

            if (_isPkResultCanKillPlayerWithTimeOut(mBlueTeamPKResult))
            {
                Logger.LogProcessFormat("[战斗] 强制杀死的不是比尔 =。= 结果{0}", mBlueTeamPKResult);
                _killOneTeamPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
            }

            return true;
        }

        private bool _isPkResultCanKillPlayerWithTimeOut(PKResult result)
        {
            if (result == PKResult.DRAW)
            {
                return true;
            }

            if (result == PKResult.LOSE)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 计算时间
        /// </summary>
        private bool _isTimeUp(int delta)
        {
            mTickTime -= delta;

            return !_isTickTimeUp();
        }

        private bool _isTickTimeUp()
        {
            return mTickTime <= 0;
        }

        protected override void _onGameStartFrame(BattlePlayer player)
        {
            _hiddenAllInput();

            mTickTime = kFirstVoteTime;

            curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.FirstVote;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3StartVoteForFight);
        }

        protected override void _onMatchBattlePlayerReady2Fight()
        {
            mTickTime = PkRaceType == (int)(Protocol.RaceType.ScoreWar) ? scoreWarFightTime * 1000 : kFightTime;
            curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.Fight;
        }

        protected override void _onMatchBattlePlayerDead(BattlePlayer player)
        {
            Logger.LogProcessFormat("[战斗] 玩家死掉了哟 {0} {1} 类型 {2} ", player.GetPlayerSeat(), player.GetPlayerName(), mMatchPlayerEndType);

            switch (mMatchPlayerEndType)
            {
                case ePVPBattleEndType.onNone:
                    mMatchPlayerEndType = ePVPBattleEndType.onAllEnemyDied;
                    curPVP3V3ProcessStatus = ePVP3V3ProcessStatus.GetRoundEndResult;

                    _getPKResultInCurrentRound();
                    break;
                case ePVPBattleEndType.onTimeOut:
                    Logger.LogProcessFormat("[战斗] 玩家死掉了哟 {0} {1} 超时", player.GetPlayerSeat(), player.GetPlayerName());
                    // 此时已经计算过
                    break;
                case ePVPBattleEndType.onAllEnemyDied:
                    Logger.LogProcessFormat("[战斗] 玩家死掉了哟 {0} {1} 对方被搞死了", player.GetPlayerSeat(), player.GetPlayerName());
                    // TODO 这里可能存在 两个玩家不在同一时间死亡，但是间隔很短
                    // 处理这种平局
                    break;
            }

            _showBattleResult();

            mDeadFightingPlayers.Add(player);
            if (player != null && player.playerActor != null)
                player.playerActor.SetAttackButtonState(ButtonState.RELEASE);

            if (player.IsLocalPlayer())
            {
                _unloadInputManger();
            }
        }

        private void _showBattleResult()
        {
#if !LOGIC_SERVER
            switch (mDungeonPlayers.GetMainPlayer().teamType)
            {
                case BattlePlayer.eDungeonPlayerTeamType.eTeamBlue:
                    _showMatchResult(mBlueTeamPKResult);
                    break;
                case BattlePlayer.eDungeonPlayerTeamType.eTeamRed:
                    _showMatchResult(mRedTeamPKResult);
                    break;
            }
#endif
        }

        private void _hiddenBattleResult()
        {
            _hiddenMatchResult();
        }


        protected override void _onMatchRoundVote(BattlePlayer player)
        {
#if !LOGIC_SERVER
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3VoteForFightStatusChanged);

            _allPlayerApplyBattleTips();
#endif
        }

        private List<BattlePlayer> mCacheRoundVotePlayers = new List<BattlePlayer>();

        private void _allPlayerApplyBattleTips()
        {
            if (ePVP3V3ProcessStatus.FirstVote == curPVP3V3ProcessStatus)
            {
                return;
            }

            BattlePlayer mainPlayer = mDungeonPlayers.GetMainPlayer();
            if (!BattlePlayer.IsDataValidBattlePlayer(mainPlayer))
            {
                return;
            }

            mCacheRoundVotePlayers.Clear();

            mDungeonPlayers.GetTeamNotVotePlayers(mCacheRoundVotePlayers, mainPlayer.teamType);

            if (mCacheRoundVotePlayers.Count > 0)
            {
                return;
            }

            mDungeonPlayers.GetTeamVotePlayers(mCacheRoundVotePlayers, mainPlayer.teamType);

            for (int i = 0; i < mCacheRoundVotePlayers.Count; ++i)
            {
                if (mCacheRoundVotePlayers[i].GetPlayerSeat() == mainPlayer.GetPlayerSeat())
                {
                    SystemNotifyManager.SystemNotify(9219);
                    return;
                }
            }
        }

        protected override void _onStart()
        {
            _bindNetMessage();
        }

        protected override void _onEnd()
        {
            _unbindNetMessage();
        }

        private void _bindNetMessage()
        {
            Logger.LogProcessFormat("[战斗] [3v3] 开始绑定 消息");
            NetProcess.AddMsgHandler(SceneRoomMatchPkRaceEnd.MsgID, _onSceneRoomMatchPkRaceEnd);
        }

        private void _unbindNetMessage()
        {
            Logger.LogProcessFormat("[战斗] [3v3] 解除绑定 消息");
            NetProcess.RemoveMsgHandler(SceneRoomMatchPkRaceEnd.MsgID, _onSceneRoomMatchPkRaceEnd);
        }

        /// <summary> 
        /// 结算消息中的数据来之后，更新界面数据，并显示玩家自己的积分变化
        /// </summary>
        private void _onSceneRoomMatchPkRaceEnd(MsgDATA data)
        {
            SceneRoomMatchPkRaceEnd res = new SceneRoomMatchPkRaceEnd();
            res.decode(data.bytes);

            Logger.LogProcessFormat("[战斗] [3v3] 比赛结束 消息");
            Logger.LogProcessFormat("[战斗] Dump SceneRoomMatchPkRaceEnd {0}", ObjectDumper.Dump(res));

            _convert2BattlePlayer(res);
            _openDungeonMatchRaceResult(res);
            _openDungeon3v3FinishFrame(res);
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3GetRaceEndResult);
        }

        private void _openDungeon3v3FinishFrame(SceneRoomMatchPkRaceEnd res)
        {
#if !LOGIC_SERVER
            var activeSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;

            if(activeSystem != null)
            {
                if(ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(Protocol.ServiceType.SERVICE_RACE_ID_CHECK))
                {
                    if (ClientApplication.playerinfo != null)
                    {
                        if(res.raceId != 0 && res.raceId == ClientApplication.playerinfo.session)
                        {
                            ClientSystemManager.instance.OpenFrame<Dungeon3v3FinishFrame>();
                        }
                    } 
                }else
                {
                    ClientSystemManager.instance.OpenFrame<Dungeon3v3FinishFrame>();
                }
            }
#endif
        }

        private void _openDungeonMatchRaceResult(SceneRoomMatchPkRaceEnd pkEndData)
        {
#if !LOGIC_SERVER
            // PKBattleResultFrame 在3v3FinishFrame里面打开

            //if (RoomType.ROOM_TYPE_THREE_MATCH == mDungeonPlayers.GetMainPlayer().GetMatchRoomType())
            //{
            //    ClientSystemManager.instance.CloseFrame<Dungeon3v3FinishFrame>();
            //    ClientSystemManager.instance.OpenFrame<PKBattleResultFrame>(FrameLayer.Middle, pkEndData);
            //}
#endif
        }

        private void _convert2BattlePlayer(SceneRoomMatchPkRaceEnd res)
        {
            if (null == res)
            {
                Logger.LogErrorFormat("[战斗] [3v3] raceEnd 为空");
                return;
            }

            for (int i = 0; i < res.slotInfos.Length; ++i)
            {
                RoomSlotBattleEndInfo cur = res.slotInfos[i];

                if (null == cur)
                {
                    continue;
                }

                BattlePlayer player = mDungeonPlayers.GetPlayerBySeat(cur.seat);

                if (null == player)
                {
                    continue;
                }

                if (player.IsLocalPlayer())
                {
                    player.ConvertSceneRoomMatchPkRaceEnd2LocalBattlePlayer(res);
                }

                player.ConvertRoomSlotBattleEndInfo2BattlePlayer(cur);
            }
        }

        /// <summary>
        /// 每一轮结束的时候玩家添加一个无敌Buff 在新一轮开始的时候移除
        /// </summary>
        private void SwitchRoundAddInvincibleBuff(BattlePlayer player, bool isRestore = false)
        {
            if (player == null)
                return;
            if (player.playerActor == null || player.playerActor.IsDead())
                return;
            if (isRestore)
                player.playerActor.buffController.RemoveBuff((int)GlobalBuff.DUNFU);
            else
                player.playerActor.buffController.TryAddBuff((int)GlobalBuff.DUNFU, GlobalLogic.VALUE_10000);
        }
    }
}
