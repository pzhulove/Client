using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class DungeonReadyFightFrame : Dungeon3V3BaseLoadFrame
    {
        public class MatchedFighters 
        {
            public byte redTeamSeat = byte.MaxValue;
            public byte blueTeamSeat = byte.MaxValue;
            public int  roundIndex = -1;
        }

        private MatchedFighters mCurrentFighters = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Loading/PVPLoading/DungeonPVPLoadingFrame";
        }

        protected override void _OnOpenFrame()
        {
            //mLeftProgress.value  = 1.0f;
            //mLeftProgressText.text = string.Empty;
            //mRightProgress.value = 1.0f;
            //mRightProgressText.text = string.Empty;

            _updateCurrentFighters();

            _initBoards();
            _initPlayers();

            mApplyBtn.gameObject.CustomActive(false);
            mCountDownRoot.CustomActive(false);
            mLeftTimeRoot.CustomActive(false);
            mFightTips.CustomActive(false);
            // mPvp3v3MicRoomBtn.gameObject.CustomActive(false);
            // mPvp3v3PlayerBtn.gameObject.CustomActive(false);

            _hiddenOther();

            if (null != mBind)
            {
                mBind.StartCoroutine(_delay1sCall());
            }
        }

        private void _hiddenOther()
        {
            if (null == mCurrentFighters)
            {
                return ;
            }

            for (int i = 0; i < mBoards.Length; ++i)
            {
                if (mBoards[i].playerSeat == byte.MaxValue)
                {
                    continue;
                }

                if (mBoards[i].playerSeat == mCurrentFighters.redTeamSeat)
                {
                    continue;
                }

                if (mBoards[i].playerSeat == mCurrentFighters.blueTeamSeat)
                {
                    continue;
                }

                if (null != mBoards[i].root)
                {
                    mBoards[i].root.CustomActive(false);
                }
            }

        }

        protected override void _OnCloseFrame()
        {
            _uninitBoards();
        }

        protected IEnumerator _delay1sCall()
        {
            yield return Yielders.GetWaitForSeconds(1.0f);

            _init2MatchPlayers();
        }

        //private class UINode
        //{
        //    public Text  name;
        //    public Text  job;
        //    public Image persion;
        //    public Text  pkLevel;
        //    public Text  guildName;
        //    public Text  serverName;
        //}

        private void _updateCurrentFighters()
        {
            mCurrentFighters = this.userData as MatchedFighters;

            if (null == mCurrentFighters)
            {
                Logger.LogErrorFormat("[战斗] 获取 userdata 失败");
            }
        }
        
        protected bool _init2MatchPlayers()
        {
            //DG.Tweening.DOTween.Play(
            //if (!_initOnePlayerWithType(BattlePlayer.eDungeonPlayerTeamType.eTeamRed))
            //{
            //    Logger.LogErrorFormat("[战斗] 对战展示 红方数据异常");
            //    return false;
            //}

            //if (!_initOnePlayerWithType(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue))
            //{
            //    Logger.LogErrorFormat("[战斗] 对战展示 蓝方数据异常");
            //    return false;
            //}
            //
            
            if (null == mBind)
            {
                return false;
            }
        
            mNextRoundRoot.CustomActive(true);
            mFightVS.gameObject.CustomActive(true);

            DG.Tweening.DOTween.Play(mPerpareRoot);

            if (null == mCurrentFighters)
            {
                Logger.LogErrorFormat("[战斗] [loading] mCurrentFighters 为空");
                return false;
            }

            mBind.GetSprite(string.Format("rnum{0}", mCurrentFighters.roundIndex), ref mNextRoundImage);
            mNextRoundImage.SetNativeSize();

            _playProcessAnimateByType(eAnimateType.eSelected, mCurrentFighters.redTeamSeat);
            _playProcessAnimateByType(eAnimateType.eSelected, mCurrentFighters.blueTeamSeat);

         
            //if (!_playAnimationWith(mCurrentFighters.redTeamSeat))
            //{
            //    Logger.LogErrorFormat("[战斗] [loading] red 播放失败 {0}", mCurrentFighters.redTeamSeat);
            //    return false;
            //}

            //if (!_playAnimationWith(mCurrentFighters.blueTeamSeat))
            //{
            //    Logger.LogErrorFormat("[战斗] [loading] blue 播放失败 {0}", mCurrentFighters.blueTeamSeat);
            //    return false;
            //}
            
            return true;
        }

        private bool  _playAnimationWith(byte seat)
        {
            MatchUnit unit = _findBoardBySeat(seat);

            if (null == unit)
            {
                Logger.LogErrorFormat("[战斗] [loading] unit 为空 {0}", seat);
                return false;
            }

            if (null == unit.root)
            {
                Logger.LogErrorFormat("[战斗] [loading] root 为空 {0}", seat);
                return false;
            }

            Logger.LogProcessFormat("[战斗] [loading] 播放 {0}", seat);

            //DG.Tweening.DOTween.Play(unit.root, "beizan02");

           // DOTweenAnimation list = unit.root.GetComponents<DOTweenAnimation>();

            //var list = DG.Tweening.GetTweensById("beizan02");

            //for (int i = 0; i < list.Count; ++i)
            //{
            //    list[i].DoPlay();
            //}
        

            return true;
        }

        //protected bool _initOnePlayerWithType(BattlePlayer.eDungeonPlayerTeamType type)
        //{
        //    BattlePlayer player = _getBattlePlayer(type);

        //    if (null == player)
        //    {
        //        return false;
        //    }

        //    UINode node = new UINode();

        //    switch(type)
        //    {
        //        case BattlePlayer.eDungeonPlayerTeamType.eTeamRed:
        //            node.name       = mLeftName;
        //            node.job        = mLeftJob;
        //            node.persion    = mLeftPerson;
        //            node.pkLevel    = mLeftPkLv;
        //            node.guildName  = mLeftGuild;
        //            node.serverName = mLeftServerText;
        //            break;
        //        case BattlePlayer.eDungeonPlayerTeamType.eTeamBlue:
        //            node.name       = mRightName;
        //            node.job        = mRightJob;
        //            node.persion    = mRightPerson;
        //            node.pkLevel    = mRightPkLv;
        //            node.guildName  = mRightGuild;
        //            node.serverName = mRightServerText;
        //            break;
        //    }

        //    return _initOnePlayerInfo(player, node);
        //}

        //private byte _getBattlePlayerByType(BattlePlayer.eDungeonPlayerTeamType type)
        //{
        //    MatchedFighters mf = this.userData as MatchedFighters;

        //    if (null == mf)
        //    {
        //        Logger.LogErrorFormat("[战斗] 获取 userdata {0}", type);
        //        return null;
        //    }
        //    
        //    Logger.LogProcessFormat("[战斗] 获得队伍 {0} 的玩家", type);

        //    switch(type)
        //    {
        //        case BattlePlayer.eDungeonPlayerTeamType.eTeamRed:
        //            return mf.redTeamSeat;
        //        case BattlePlayer.eDungeonPlayerTeamType.eTeamBlue:
        //            return BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mf.blueTeamSeat);
        //    }

        //    Logger.LogErrorFormat("[战斗] 传入类型有误 {0}", type);

        //    return null;
        //}

        //private bool _initOnePlayerInfo(BattlePlayer player, UINode node)
        //{
        //    if (!BattlePlayer.IsDataValidBattlePlayer(player))
        //    {
        //        return false;
        //    }

        //    if (null == node)
        //    {
        //        return false;
        //    }

        //    if (null != node.name)
        //    {
        //        node.name.text = player.GetPlayerName();
        //    }

        //    if (null != node.job)
        //    {
        //        node.job.text = Utility.GetJobName(player.playerInfo.occupation,0);
        //    }

        //    if (null != node.serverName)
        //    {
        //        if (ClientApplication.adminServer.name != player.GetPlayerServerName())
        //        {
        //            node.serverName.text = player.GetPlayerServerName();
        //        }
        //        else
        //        {
        //            node.serverName.text = string.Empty;
        //        }
        //    }

        //    if (null != node.persion)
        //    {
        //        var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(player.playerInfo.occupation);

        //        if (jobData != null && jobData.JobPortrayal != "" && jobData.JobPortrayal != "-")
        //        {
        //            ETCImageLoader.LoadSprite(ref node.persion, jobData.JobPortrayal);
        //        }
        //    }

        //    if (null != node.pkLevel)
        //    {
        //        node.pkLevel.text = SeasonDataManager.GetInstance().GetRankName((int)player.playerInfo.seasonLevel);
        //    }

        //    if (null != node.guildName)
        //    {
        //        if (string.IsNullOrEmpty(player.playerInfo.guildName))
        //        {
        //            node.guildName.text = string.Empty;
        //        }
        //        else
        //        {
        //            node.guildName.text = string.Format("公会:{0}", player.playerInfo.guildName);
        //        }
        //    }

        //    return true;
        //}
    }
}
