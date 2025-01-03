using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battle;
using DG.Tweening;
using Protocol;
using Network;

namespace GameClient
{
    public class Dungeon3v3FinishFrame : ClientFrame
    {
        private class ResultUnit
        {
            public byte seat = byte.MaxValue;
            public ComCommonBind bind = null;
            public GameObject root = null;
        }

        private List<ResultUnit> mResults = new List<ResultUnit>();

        private enum eState
        {
            /// <summary>
            /// 初始化状态
            /// </summary>
            eNone,

            /// <summary>
            /// 展示结果
            /// </summary>
            eShowResult,

            /// <summary>
            /// 等待用户关闭
            /// </summary>
            eWaitClose,

            /// <summary>
            /// 去他妈的关闭界面
            /// </summary>
            eClose,
        }

        private eState mState = eState.eNone;

        private const float kShowResultTime = 5.0f;
        private const float kWaitCloseTime = 10.0f;

        private float mTickTime = 0.0f;

        private RoomType mCurrentRoomType = RoomType.ROOM_TYPE_INVALID;
        private SceneRoomMatchPkRaceEnd mSceneRoomMatchPkRaceEnd = null;

        #region ExtraUIBind
        private Text mBestPlayerName = null;
        private GameObject mBestPlayerIcon = null;
        private CanvasGroup mTipsCanvasGroup = null;
        private ComCountScript mCount2Close = null;
        private GameObject mSeat0 = null;
        private GameObject mSeat1 = null;
        private GameObject mSeat2 = null;
        private GameObject mSeat3 = null;
        private GameObject mSeat4 = null;
        private GameObject mSeat5 = null;
        private Image mResult = null;
        private GameObject mRankTitle = null;
        private GameObject mBaseScoreTitle = null;
        private GameObject mContributeTitle = null;
        private GameObject scoreTitle = null;
        private GameObject mGloryTitle = null;
        private Button mCloseButton = null;

        protected override void _bindExUI()
        {
            mBestPlayerName = mBind.GetCom<Text>("bestPlayerName");
            mBestPlayerIcon = mBind.GetGameObject("bestPlayerIcon");
            mTipsCanvasGroup = mBind.GetCom<CanvasGroup>("tipsCanvasGroup");
            mCount2Close = mBind.GetCom<ComCountScript>("count2Close");
            mSeat0 = mBind.GetGameObject("seat0");
            mSeat1 = mBind.GetGameObject("seat1");
            mSeat2 = mBind.GetGameObject("seat2");
            mSeat3 = mBind.GetGameObject("seat3");
            mSeat4 = mBind.GetGameObject("seat4");
            mSeat5 = mBind.GetGameObject("seat5");
            mResult = mBind.GetCom<Image>("result");
            mRankTitle = mBind.GetGameObject("rankTitle");
            scoreTitle = mBind.GetGameObject("scoreTitle");
            mBaseScoreTitle = mBind.GetGameObject("baseScoreTitle");
            mContributeTitle = mBind.GetGameObject("contributeTitle");
            mCloseButton = mBind.GetCom<Button>("closeButton");
            if (null != mCloseButton)
            {
                mCloseButton.onClick.AddListener(_onCloseButtonButtonClick);
            }
            mGloryTitle = mBind.GetGameObject("gloryTitle");
        }

        protected override void _unbindExUI()
        {
            mBestPlayerName = null;
            mBestPlayerIcon = null;
            mTipsCanvasGroup = null;
            mCount2Close = null;
            mSeat0 = null;
            mSeat1 = null;
            mSeat2 = null;
            mSeat3 = null;
            mSeat4 = null;
            mSeat5 = null;
            mResult = null;
            mRankTitle = null;
            mBaseScoreTitle = null;
            mContributeTitle = null;
            scoreTitle = null;
            if (null != mCloseButton)
            {
                mCloseButton.onClick.RemoveListener(_onCloseButtonButtonClick);
            }
            mCloseButton = null;
            mGloryTitle = null;
        }
        #endregion


        #region Callback
        private void _onCloseButtonButtonClick()
        {
            /* put your code in here */
            if (ReplayServer.instance.IsReplay())
            {
                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
                return;
            }
            if (mCurrentRoomType == RoomType.ROOM_TYPE_THREE_MATCH)
            {
                switch (mState)
                {
                    case eState.eWaitClose:
                        if (_isSceneRoomMatchPkRaceEndRecied())
                        {
                            _openDungeonMatchRaceResult(mSceneRoomMatchPkRaceEnd);
                        }
                        else
                        {
                            Logger.LogErrorFormat("[战斗] [3v3] 打开匹配结果界面错误哦 还没收到比赛结束消息");
                        }
                        break;
                    case eState.eClose:
                        // 此时倒计时结束，还没收到消息的情况下响应点击事件 直接返回城镇在
                        ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
                        break;
                    default:
                        Logger.LogErrorFormat("[战斗] [3v3] 其他状态 {0}", mState);
                        break;
                }
            }
            else
            {
                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
            }
        }
        #endregion

        /// <summary>
        /// 自动关闭的时候
        /// </summary>
        private void _onAutoClose()
        {
            if (mCurrentRoomType == RoomType.ROOM_TYPE_THREE_MATCH)
            {
                switch (mState)
                {
                    case eState.eClose:
                        if (_isSceneRoomMatchPkRaceEndRecied())
                        {
                            _openDungeonMatchRaceResult(mSceneRoomMatchPkRaceEnd);
                        }
                        else
                        {
                            Logger.LogErrorFormat("[战斗] [3v3] 打开匹配结果界面错误哦 还没收到比赛结束消息");
                        }
                        break;
                    default:
                        Logger.LogErrorFormat("[战斗] [3v3] 其他状态 {0}", mState);
                        break;
                }
            }
            else
            {
                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
            }
        }

        private bool _isSceneRoomMatchPkRaceEndRecied()
        {
            return null != mSceneRoomMatchPkRaceEnd;
        }

        private void _openDungeonMatchRaceResult(SceneRoomMatchPkRaceEnd pkEndData)
        {
            if (RoomType.ROOM_TYPE_THREE_MATCH != mCurrentRoomType)
            {
                Logger.LogErrorFormat("[战斗] [3v3] 打开匹配结果界面错误哦 当前比赛类型 {0}", mCurrentRoomType);
                return;
            }

            // TODO 这里隐藏特效 ?
            ClientSystemManager.instance.CloseFrame(this);

            ClientSystemManager.instance.OpenFrame<PKBattleResultFrame>(FrameLayer.Middle, mSceneRoomMatchPkRaceEnd);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3/Pk3v3ResultFrame";
        }

        protected override void _OnOpenFrame()
        {
            _initFrameState();

            _clearAllBoards();
            _initBoardTitle();
            _initBoards();
            _initAllPlayersFromBattleInfo();
            _initBestPlayer();
            _updateResultImage();
            _updateAllPlayersFromResultInfo();

            _bindEvents();
            _bindNetMessage();
        }

        void UpdateBattleResultInfo()
        {            
            
        }

        protected override void _OnCloseFrame()
        {
            _clearFrameState();
            _unbindEvents();
            _clearAllBoards();

            _unbindNetMessage();
        }

        private void _bindNetMessage()
        {
            Logger.LogProcessFormat("[战斗] [3v3] Finish界面 开始绑定 消息");
            NetProcess.AddMsgHandler(SceneRoomMatchPkRaceEnd.MsgID, _onSceneRoomMatchPkRaceEnd);
        }

        private void _unbindNetMessage()
        {
            Logger.LogProcessFormat("[战斗] [3v3] Finish界面 接触绑定 消息");
            NetProcess.RemoveMsgHandler(SceneRoomMatchPkRaceEnd.MsgID, _onSceneRoomMatchPkRaceEnd);
        }

        private void _onSceneRoomMatchPkRaceEnd(MsgDATA data)
        {
            SceneRoomMatchPkRaceEnd res = new SceneRoomMatchPkRaceEnd();

            res.decode(data.bytes);

            mSceneRoomMatchPkRaceEnd = res;

            // 倒计时结束之后收到网络消息，直接打开界面
            if (mState == eState.eClose)
            {
                _openDungeonMatchRaceResult(res);
            }
        }

        private void _bindEvents()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3GetRaceEndResult,   _onPK3V3GetRaceEndResult);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK2V2CrossScoreGetRaceEndResult, _onPK2V2GetRaceEndResult);
        }

        private void _unbindEvents()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3GetRaceEndResult, _onPK3V3GetRaceEndResult);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK2V2CrossScoreGetRaceEndResult, _onPK2V2GetRaceEndResult);
        }

        private void _onPK3V3GetRaceEndResult(UIEvent ui)
        {
            _updateAllPlayersFromResultInfo();
        }
        private void _onPK2V2GetRaceEndResult(UIEvent ui)
        {
            _updateAllPlayersFromResultInfo();
        }

        private void _updateResultImage()
        {
            if (null == mResult)
            {
                return ;
            }

            BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

            if (null == mainPlayer)
            {
                mResult.enabled = false;
                return;
            }

            PKResult result = _getTeamRaceEndResult(mainPlayer.teamType);

            switch (result)
            {
                case PKResult.DRAW:
                    mBind.GetSprite("draw", ref mResult);
                    break;
                case PKResult.WIN:
                    mBind.GetSprite("win", ref mResult);
                    break;
                case PKResult.LOSE:
                    mBind.GetSprite("lose", ref mResult);
                    break;
                default:
                    mResult.enabled = false;
                    break;
            }
        }

        /// <summary>
        /// 这里是显示战斗结果的数据
        /// </summary>
        private void _updateAllPlayersFromResultInfo()
        {
            for (int i = 0; i < mResults.Count; ++i)
            {
                BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mResults[i].seat);
                ComCommonBind bind = mResults[i].bind;

                if (null == bind || null == player || _is3v3Scuffle())
                {
                    continue;
                }

                _updateOnePlayerSeasonInfo(bind, player.GetRaceEndResultSeasonLevel(), player);
            }
        }

        private void _clearAllBoards()
        {
            for (int i = 0; i < mResults.Count; ++i)
            {
                mResults[i].bind.ClearAllCacheBinds();
                mResults[i].bind = null;
                mResults[i].root = null;
                mResults[i].seat = byte.MaxValue;
            }

            mResults.Clear();
        }

        private void _initBestPlayer()
        {
            BattlePlayer player = _getRaceEndTopRankPlayer();
            if (null == player)
            {
                return ;
            }

            mBestPlayerName.text = player.GetPlayerName();
            
            var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(player.playerInfo.occupation);
            if (jobData != null && jobData.PKResultPrefab != "" && jobData.PKResultPrefab != "-")
            {
                GameObject obj = AssetLoader.instance.LoadResAsGameObject(jobData.PKResultPrefab);
                Utility.AttachTo(obj, mBestPlayerIcon);
                //ETCImageLoader.LoadSprite(ref mBestPlayerIcon, jobData.JobPortrayal);
            }
        }

        private BattlePlayer _getRaceEndTopRankPlayer()
        {
            List<BattlePlayer> battlePlayers = BattleMain.instance.GetPlayerManager().GetAllPlayers();
            PKResult result = _getTeamRaceEndResult(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);

            switch (result)
            {
                case PKResult.WIN:
                    return _getRaceEndTeamTopRankPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                case PKResult.LOSE:
                    return _getRaceEndTeamTopRankPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
                case PKResult.DRAW:
                    {
                        if (_is3v3Scuffle() || _is2v2ScuffleScore())
                        {
                            List<BattlePlayer> list = BattleMain.instance.GetPlayerManager().GetAllPlayers();
                            list.Sort(_teamPlayerScoreCmp);
                            return list[0];
                        }
                        else
                        {
                            BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

                            if (null == mainPlayer)
                            {
                                break;
                            }

                            if (mainPlayer.IsTeamRed())
                            {
                                return _getRaceEndTeamTopRankPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                            }
                            else
                            {
                                return _getRaceEndTeamTopRankPlayer(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
                            }
                        }
                    }
                    break;
            }

            Logger.LogErrorFormat("[战斗] 战斗结果异常 {0}", result);

            if (battlePlayers.Count > 0)
            {
                // TODO get best playername
                return battlePlayers[0];
            }

            return null;
        }

        public PKResult _getTeamRaceEndResult(BattlePlayer.eDungeonPlayerTeamType type)
        {
            if (_is3v3Scuffle() || _is2v2ScuffleScore())
            {
                if (BattleMain.instance != null && BattleMain.instance.GetBattle() != null)
                {
                    var battle = BattleMain.instance.GetBattle() as PVPScuffleBattle;

                    return battle.GetRaceEndResult(type);
                }
                else
                {
                    return PKResult.INVALID;
                }
            }
            else
            {
                // TODO BaseMatchPVPBattle.cs: 108 _getRaceEndResult
                //  我方队伍有人还没参战
                if (!BattleMain.instance.GetPlayerManager().IsTeamPlayerAllFighted(type))
                {
                    return PKResult.WIN;
                }

                // 对方队伍有人还没参战
                if (!BattleMain.instance.GetPlayerManager().IsTeamPlayerAllFighted(BattlePlayer.GetTargetTeamType(type)))
                {
                    return PKResult.LOSE;
                }

                // 所有场上玩家都经理战斗
                List<BattlePlayer> allPlayers = BattleMain.instance.GetPlayerManager().GetAllPlayers();

                PKResult pKResult = PKResult.INVALID;
                int maxIndex = -1;

                for (int i = 0; i < allPlayers.Count; ++i)
                {
                    if (allPlayers[i].teamType == type)
                    {
                        // 查找最后一轮玩家的战绩
                        int roundIdx = allPlayers[i].GetLastPKRoundIndex();

                        if (roundIdx > maxIndex)
                        {
                            pKResult = allPlayers[i].GetLastPKResult();
                            maxIndex = roundIdx;
                        }
                    }
                }

                return pKResult;
            }
        }

        public BattlePlayer _getRaceEndTeamTopRankPlayer(BattlePlayer.eDungeonPlayerTeamType type)
        {
            List<BattlePlayer> teamPlayers = _getRaceEndTeamResultsInCacheList(type);

            if (null == teamPlayers || teamPlayers.Count <= 0)
            {
                return null;
            }

            return teamPlayers[0];
        }

        private List<BattlePlayer> _getRaceEndTeamResultsInCacheList(BattlePlayer.eDungeonPlayerTeamType type)
        {
            List<BattlePlayer> teamPlayers = _getTeamPlayers(type);

            if (null == teamPlayers || teamPlayers.Count <= 0)
            {
                return new List<BattlePlayer>();
            }

            if(!_is3v3Score())
            {
                teamPlayers.Sort(_teamPlayerScoreCmp);
            }
            else
            {
                teamPlayers.Sort(_teamPlayerScoreCmp3v3Cross);
            }
            

            return teamPlayers;
        }

        private List<BattlePlayer> _getTeamPlayers(BattlePlayer.eDungeonPlayerTeamType type)
        {
            List<BattlePlayer> battlePlayers = BattleMain.instance.GetPlayerManager().GetAllPlayers();
            List<BattlePlayer> cacheTeamPlayers = new List<BattlePlayer>();

            for (int i = 0; i < battlePlayers.Count; ++i)
            {
                if (battlePlayers[i].teamType == type)
                {
                    cacheTeamPlayers.Add(battlePlayers[i]);
                }
            }

            return cacheTeamPlayers;
        }

        private int _teamPlayerScoreCmp(BattlePlayer fst, BattlePlayer snd)
        {
            int fstKilledNum = fst.GetKillMatchPlayerCount();
            int sndKilledNum = snd.GetKillMatchPlayerCount();
            if (fstKilledNum > sndKilledNum)
            {
                return -1;
            }
            else if (fstKilledNum == sndKilledNum)
            {
                // TODO 根据伤害血量排布
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private int _teamPlayerScoreCmp3v3Cross(BattlePlayer fst, BattlePlayer snd)
        {
            uint fstScore = fst.GetRaceEndResultScoreWarBaseScore() + fst.GetRaceEndResultScoreWarContriScore();
            uint sndScore = snd.GetRaceEndResultScoreWarBaseScore() + snd.GetRaceEndResultScoreWarContriScore();

            Logger.LogErrorFormat("fstScore = {0},sndScore = {1}", fstScore, sndScore);

            if (fstScore > sndScore)
            {
                return -1;
            }
            else if (fstScore == sndScore)
            {
                // TODO 根据基础分+贡献分排序
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private void _initBoardTitle()
        {
            bool isNormal = !_is3v3Score();

            mRankTitle.CustomActive(isNormal);
            mContributeTitle.CustomActive(!isNormal);
            mBaseScoreTitle.CustomActive(!isNormal);
            mGloryTitle.CustomActive(!isNormal);
            scoreTitle.CustomActive(false);
            if(_is2v2ScuffleScore())
            {          
                mRankTitle.CustomActive(false);
                mContributeTitle.CustomActive(false);
                mBaseScoreTitle.CustomActive(false);
                scoreTitle.CustomActive(true);
                mGloryTitle.CustomActive(true);
            }
        }

        private void _initBoards()
        {
            string unitPath = mBind.GetPrefabPath("unit");
            mBind.ClearCacheBinds(unitPath);

            BattlePlayer[] allPlayers = _getAllPlayerRaceEndRanks();

            for (int i = 0; i < allPlayers.Length; ++i)
            {
                BattlePlayer player = allPlayers[i];

                ComCommonBind bind  = mBind.LoadExtraBind(unitPath);

                ResultUnit unit     = new ResultUnit();

                unit.seat           = player.GetPlayerSeat();
                unit.bind           = bind;
                unit.root           = _getBoardRoot(i);

                mResults.Add(unit);

                Utility.AttachTo(bind.gameObject, _getBoardRoot(i));
            }

        }

        public BattlePlayer[] _getAllPlayerRaceEndRanks()
        {
            List<BattlePlayer> allBattlePlayer = new List<BattlePlayer>();

            PKResult result = _getTeamRaceEndResult(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);

            switch (result)
            {
                case PKResult.WIN:
                    _getRaceEndRank(allBattlePlayer, BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                    _getRaceEndRank(allBattlePlayer, BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
                    break;
                case PKResult.LOSE:
                    _getRaceEndRank(allBattlePlayer, BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
                    _getRaceEndRank(allBattlePlayer, BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
                    break;
                case PKResult.DRAW:
                    // 平局的显示
                    BattlePlayer[] teamRed = _getRaceEndTeamResultsInCacheList(BattlePlayer.eDungeonPlayerTeamType.eTeamRed).ToArray();
                    BattlePlayer[] teamBlue = _getRaceEndTeamResultsInCacheList(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue).ToArray();

                    int i = 0;
                    int j = 0;

                    while (i < teamRed.Length)
                    {
                        teamRed[i].SetPKResultRank(i + 1);
                        allBattlePlayer.Add(teamRed[i++]);
                    }

                    while (j < teamBlue.Length)
                    {
                        teamBlue[j].SetPKResultRank(j + 1);
                        allBattlePlayer.Add(teamBlue[j++]);
                    }

                    //while (i < teamRed.Length || j < teamBlue.Length)
                    //{
                    //    if (i < teamRed.Length)
                    //    {
                    //        teamRed[i].SetPKResultRank(i + 1);
                    //        allBattlePlayer.Add(teamRed[i++]);
                    //    }

                    //    if (j < teamBlue.Length)
                    //    {
                    //        teamBlue[j].SetPKResultRank(j + 1);
                    //        allBattlePlayer.Add(teamBlue[j++]);
                    //    }
                    //}
                    break;
            }

            return allBattlePlayer.ToArray();
        }

        private bool _getRaceEndRank(List<BattlePlayer> list, BattlePlayer.eDungeonPlayerTeamType type)
        {
            if (null == list)
            {
                return false;
            }

            int startIndex = list.Count;

            List<BattlePlayer> teamPlayers = _getRaceEndTeamResultsInCacheList(type);

            for (int i = 0; i < teamPlayers.Count; ++i)
            {
                list.Add(teamPlayers[i]);

                teamPlayers[i].SetPKResultRank(startIndex + i + 1);
            }

            return true;
        }

        private GameObject _getBoardRoot(int idx)
        {
            switch (idx)
            {
                case 0:
                    return mSeat0;
                case 1:
                    return mSeat1;
                case 2:
                    return mSeat2;
                case 3:
                    return mSeat3;
                case 4:
                    return mSeat4;
                case 5:
                    return mSeat5;
            }

            return null;
        }

        /// <summary>
        /// 这里是显示战斗已有的数据，以及客户端统计的数据
        /// </summary>
        private void _initAllPlayersFromBattleInfo()
        {
            for (int i = 0; i < mResults.Count; ++i)
            {
                _initOnePlayerInfoFromBattleInfo(mResults[i]);
            }
        }

        private bool _initOnePlayerInfoFromBattleInfo(ResultUnit resultUnit)
        {
            if (null == resultUnit)
            {
                return false;
            }

            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(resultUnit.seat);
            ComCommonBind bind  = resultUnit.bind;

            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return false;
            }

            if (null == bind)
            {
                return false;
            }

            GameObject selectObject = bind.GetGameObject("selectObject");
            Text killNumber         = bind.GetCom<Text>("killNumber");
            Text gloryNumber        = bind.GetCom<Text>("glory");
            Text playerName         = bind.GetCom<Text>("playerName");
            Image playerIcon        = bind.GetCom<Image>("playerIcon");
            Image teamFlag          = bind.GetCom<Image>("teamFlag");
            Image rankNumber        = bind.GetCom<Image>("rankNumber");
            Image playerResulte     = bind.GetCom<Image>("playerResulte");

            if (null != resultUnit.root)
            {
                DG.Tweening.DOTweenAnimation animate = resultUnit.root.GetComponent<DG.Tweening.DOTweenAnimation>();
                if (null != animate)
                {
                    animate.DOPlay();
                }
            }

            
            mBind.GetSprite(_getTeamFlagTag(player.teamType), ref teamFlag);
            mBind.GetSprite(_getTeamRankTag(player.GetPKResultRank()), ref rankNumber);

            string iconPath = _getTeamIconPath(player);


            if (!string.IsNullOrEmpty(iconPath) && "-" != iconPath)
            {
                Logger.LogProcessFormat("[战斗] [3v3] 结算头像路径 {0}", iconPath);
                
                ETCImageLoader.LoadSprite(ref playerIcon, iconPath);
            }

            playerResulte.gameObject.CustomActive(BattleMain.instance.GetPlayerManager().IsKillEnemyMatchPlayer(player.GetPlayerSeat()));

            selectObject.CustomActive(player.IsLocalPlayer());

            bind.transform.localScale = player.IsLocalPlayer() ? Vector3.one * 1.03f : Vector3.one;

            playerName.text = player.GetPlayerName();
            killNumber.text = player.GetKillMatchPlayerCount().ToString();
            if(gloryNumber != null)
            {
                gloryNumber.text = "+" + player.GetRaceEndResultAddGlory().ToString();
            }

            _updateOnePlayerSeasonInfo(bind, player.playerInfo.seasonLevel, player);

            return true;
        }

        private string _getTeamIconPath(BattlePlayer player)
        {
            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return string.Empty;
            }

            var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>((int)player.playerInfo.occupation);
            if (jobData != null)
            {
                return jobData.PKMatchResultHeadIconPath;
            }

            return string.Empty;
        }

        private string _getTeamRankTag(int rank)
        {
            return string.Format("n{0}", rank);
        }

        private string _getTeamFlagTag(BattlePlayer.eDungeonPlayerTeamType type)
        {
            switch(type)
            {
                case BattlePlayer.eDungeonPlayerTeamType.eTeamRed:
                    return "redTeamBoard";
                case BattlePlayer.eDungeonPlayerTeamType.eTeamBlue:
                    return "blueTeamBoard";
            }
            return "redTeamBoard";
        }

        private bool _updateOnePlayerSeasonInfo(ComCommonBind bind, UInt32 seasonLevel, BattlePlayer player)
        {   
            if (null == bind)
            {
                return false;
            }

            byte occupation = player.GetPlayerSeat();
            //UInt32 seasonLevel = ;
            //player.playerInfo.seasonLevel;

            Text rankLevelNum   = bind.GetCom<Text>("rankLevelNum");
            Image rankLevelIcon = bind.GetCom<Image>("rankLevelIcon");
            Image rankIconNumber = bind.GetCom<Image>("rankIconNumber");

            Text baseScoreNumber = bind.GetCom<Text>("baseScoreNumber");
            Text contributeNumber = bind.GetCom<Text>("contributeNumber");
            GameObject rankLevelRoot = bind.GetGameObject("rankLevelRoot");
            GameObject waitScoreRoot = bind.GetGameObject("waitScoreRoot");

            Text gloryNumber = bind.GetCom<Text>("glory");
            bool isTypeNormal = !_is3v3Score();

            rankLevelRoot.CustomActive(isTypeNormal);
            baseScoreNumber.CustomActive(!isTypeNormal);
            contributeNumber.CustomActive(!isTypeNormal);
            if (null != gloryNumber)
            {
                gloryNumber.CustomActive(!isTypeNormal);
            }

            if (_is2v2ScuffleScore())
            {
                rankLevelRoot.CustomActive(false);
                contributeNumber.CustomActive(false);
                baseScoreNumber.CustomActive(true);
                if(null != gloryNumber)
                {
                    gloryNumber.CustomActive(true);
                }
            }
            if (!isTypeNormal)
            {
                if (player.GetRaceEndResultScoreWarBaseScore() == UInt32.MaxValue ||
                    player.GetRaceEndResultScoreWarContriScore() == UInt32.MaxValue)
                {
                    waitScoreRoot.CustomActive(true);

                    baseScoreNumber.CustomActive(false);
                    contributeNumber.CustomActive(false);
                    if (gloryNumber != null)
                    {
                        gloryNumber.CustomActive(false);
                    }
                }
                else
                {
                    waitScoreRoot.CustomActive(false);
 
                    baseScoreNumber.text = player.GetRaceEndResultScoreWarBaseScore().ToString();
                    contributeNumber.text = player.GetRaceEndResultScoreWarContriScore().ToString();
                }
            }
            if (_is2v2ScuffleScore())
            {
                //baseScoreNumber.text = player.GetRaceEndResultScoreWarBaseScore().ToString();

                // 在2v2积分赛中 baseScoreNumber表示为战斗积分
                // 胜利100 失败50
                int score = 0;
                PKResult pKResult = _getTeamRaceEndResult(player.teamType);
                if (pKResult == PKResult.WIN)
                {
                    score = Utility.GetSystemValueFromTable(ProtoTable.SystemValueTable.eType3.SVT_2V2_SCORE_WAR_WIN_SCORE);
                }
                else if (pKResult == PKResult.LOSE)
                {
                    score = Utility.GetSystemValueFromTable(ProtoTable.SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LOSE_SCORE);
                }
                baseScoreNumber.text = "+" + score.ToString();
            }

            if(_is2v2ScuffleScore()|| _is3v3Score())
            {
                if (gloryNumber != null)
                {
                    PKResult pKResult = _getTeamRaceEndResult(player.teamType);
                    gloryNumber.text = "+" + _getGloryCount(pKResult);
                }
            }

            rankLevelNum.text   = SeasonDataManager.GetInstance().GetRankName((int)seasonLevel);

            string path = SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon((int)seasonLevel);
            ETCImageLoader.LoadSprite(ref rankLevelIcon, path);

            path = SeasonDataManager.GetInstance().GetSubSeasonLevelIcon((int)seasonLevel);
            ETCImageLoader.LoadSprite(ref rankIconNumber, path);
            rankIconNumber.SetNativeSize();

            return true;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        private void _initFrameState()
        {
            mTickTime            = kShowResultTime;
            mState               = eState.eShowResult;

            mCloseButton.enabled = false;
            // hident tips
            mTipsCanvasGroup.alpha = 0.0f;

            mSceneRoomMatchPkRaceEnd = null;

            // 房间类型
            try
            {
                mCurrentRoomType = (RoomType)Pk3v3DataManager.GetInstance().GetRoomInfo().roomSimpleInfo.roomType;
            }
            catch (Exception e)
            {

            }
        }

        private bool _is3v3Score()
        {
            return BattleDataManager.GetInstance().PkRaceType == RaceType.ScoreWar;
        }

        private bool _is3v3Scuffle()
        {
            return BattleDataManager.GetInstance().PkRaceType == RaceType.PK_3V3_Melee;
        }

        private bool _is2v2ScuffleScore()
        {
            return BattleDataManager.GetInstance().PkRaceType == RaceType.PK_2V2_Melee;
        }

        private void _clearFrameState()
        {
            mState = eState.eNone;
        }

        protected override void _OnUpdate(float delta)
        {
            switch (mState)
            {
                case eState.eShowResult:
                    if (_isTickTimeEnd(delta))
                    {
                        mTickTime = kWaitCloseTime;
                        mState    = eState.eWaitClose;

                        // show tips
                        mCloseButton.enabled = true;
                        // tips fade in
                        DOTween.To(() => { return 0.0f; },
                                (x) =>
                                {
                                    if (null != mTipsCanvasGroup)
                                    {
                                        mTipsCanvasGroup.alpha = x;
                                    }
                                },
                                1.0f,
                                2.0f)
                        .SetEase(Ease.InQuad);

                        mCount2Close.StartCount(null, (int)kWaitCloseTime);
                    }
                    break;
                case eState.eWaitClose:
                    if (_isTickTimeEnd(delta))
                    {
                        mState    = eState.eClose;
                        _onAutoClose();
                    }
                    else
                    {

                    }
                    break;
                case eState.eClose:
                    break;

            }
        }

        private bool _isTickTimeEnd(float delta)
        {
            mTickTime -= delta;

            return mTickTime <= 0.0f;
        }

        private int _getGloryCount(PKResult result)
        {
            int tempValue = 0;
            var item = _getHonorRewardTable();
            if (item != null)
            {
                if (result== PKResult.WIN)
                {
                    tempValue = item.VictoryReward;
                }
                else if(result == PKResult.LOSE)
                {
                    tempValue = item.LostReward;
                }
                else if(result == PKResult.DRAW)
                {
                    tempValue = item.LostReward;
                }
            }
            return tempValue;
        }

        private ProtoTable.HonorRewardTable _getHonorRewardTable()
        {
            var table = TableManager.GetInstance().GetTable<ProtoTable.HonorRewardTable>();

            foreach (var item in table)
            {
                var tableItem = item.Value as ProtoTable.HonorRewardTable;
                if (_is2v2ScuffleScore() && tableItem.PvpType == 15)
                {
                    return tableItem;
                }
                if (_is3v3Score() && tableItem.PvpType == 9)
                {
                    return tableItem;
                }
            }
            return null;
        }
    }
}
