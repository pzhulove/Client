using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameClient;
using ProtoTable;

public class ComPVP3V3PendingCharactor : MonoBehaviour
{
    void Awake()
    {
        _bindExUI();
        _bindEvent();
    }

    void OnDestroy()
    {
        _unbindExUI();
        _unbindEvent();
    }

    private void _bindEvent()
    {
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3VoteForFightStatusChanged);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3VoteForFightStatusChanged);
    }

    private void _unbindEvent()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3VoteForFightStatusChanged);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3VoteForFightStatusChanged);
    }

    private void _onPK3V3VoteForFightStatusChanged(UIEvent ui)
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);

        if (!BattlePlayer.IsDataValidBattlePlayer(player))
        {
            return ;
        }

        BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        if (!BattlePlayer.IsDataValidBattlePlayer(mainPlayer))
        {
            return ;
        }

        if (mainPlayer.teamType != player.teamType)
        {
            return ;
        }

        _updateFightFlag();
    }

    public ComCommonBind mBind;

#region ExtraUIBind
    private RectTransform mBg = null;
    private Image mIcon = null;
    private GameObject mIconRoot = null;
    private Text mName = null;
    private GameObject mDeadFlag = null;
    private Outline mTeamFlag = null;
    private GameObject mLocalPlayerFlag = null;
    private GameObject mRedtipapplybattle = null;
    private GameObject mRedtipnextround = null;
    private GameObject mRedtiproot = null;
    private GameObject mRedtriangle = null;
    private GameObject mFightFlag = null;

    protected void _bindExUI()
    {
        mBg = mBind.GetCom<RectTransform>("bg");
        mIcon = mBind.GetCom<Image>("icon");
        mIconRoot = mBind.GetGameObject("iconRoot");
        mName = mBind.GetCom<Text>("name");
        mDeadFlag = mBind.GetGameObject("deadFlag");
        mTeamFlag = mBind.GetCom<Outline>("teamFlag");
        mLocalPlayerFlag = mBind.GetGameObject("localPlayerFlag");
        mRedtipapplybattle = mBind.GetGameObject("redtipapplybattle");
        mRedtipnextround = mBind.GetGameObject("redtipnextround");
        mRedtiproot = mBind.GetGameObject("redtiproot");
        mRedtriangle = mBind.GetGameObject("redtriangle");
        mFightFlag = mBind.GetGameObject("fightFlag");
    }

    protected void _unbindExUI()
    {
        mBg = null;
        mIcon = null;
        mIconRoot = null;
        mName = null;
        mDeadFlag = null;
        mTeamFlag = null;
        mLocalPlayerFlag = null;
        mRedtipapplybattle = null;
        mRedtipnextround = null;
        mRedtiproot = null;
        mRedtriangle = null;
        mFightFlag = null;
    }
#endregion   
 
	private byte mSeat = byte.MaxValue;

    public void InitWithSeat(byte seat)
    {
		if (mSeat == seat)
		{
			return ;
		}

        mSeat = seat;

        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(seat);

        if (!BattlePlayer.IsDataValidBattlePlayer(player))
        {
            return;
        }

		if (null != mName)
		{
			mName.text = player.GetPlayerName();
		}

		if (null != mTeamFlag)
		{
            //mTeamFlag.enabled     = true;
            //mTeamFlag.effectColor = player.IsTeamRed() ? Color.red : Color.blue;
		}

        if (null != mRedtiproot)
        {
            mRedtiproot.CustomActive(true);
        }

        if (null != mIcon)
        {
            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>((int)player.playerInfo.occupation);
            if (null != jobData)
            {

                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);

                if (null != resData)
                {
                    ETCImageLoader.LoadSprite(ref mIcon, resData.IconPath);
                }
            }
        }

		if (null != mIconRoot)
		{
			if (player.IsTeamRed())
			{
				mIconRoot.transform.SetAsFirstSibling();
			}
			else
			{
				mIconRoot.transform.SetAsLastSibling();
			}
		}

        if (null != mLocalPlayerFlag)
        {
            mLocalPlayerFlag.CustomActive(player.IsLocalPlayer());
            mLocalPlayerFlag.transform.SetAsLastSibling();
        }

		if (null != mBg)
		{
            mBg.transform.SetAsFirstSibling();

			if (player.IsTeamRed())
			{
				mBg.rectTransform().localScale = Vector3.one;
			}
			else
			{
				mBg.rectTransform().localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			}
		}

        if (null != mRedtipnextround)
        {
            if (player.IsTeamRed())
            {
                mRedtipnextround.transform.localScale = Vector3.one;
            }
            else
            {
                mRedtipnextround.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
        }

        if (null != mRedtipapplybattle)
        {
            if (player.IsTeamRed())
            {
                mRedtipapplybattle.transform.localScale = Vector3.one;
            }
            else
            {
                mRedtipapplybattle.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
        }

        Logger.LogProcessFormat("[战斗] 玩家 {0} 初始化HUD信息", player.GetPlayerName());
    }

    public void UpdateInfo()
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);

        if (!BattlePlayer.IsDataValidBattlePlayer(player))
        {
            return;
        }

        Logger.LogProcessFormat("[战斗] 玩家 {0} 更新HUD信息", player.GetPlayerName());

        _updateFightFlag();

        if (null != mDeadFlag)
        {
            mDeadFlag.CustomActive(player.isPassedInRound);
        }
    }

    private List<BattlePlayer> votePlayers    = new List<BattlePlayer>();
    private List<BattlePlayer> notVotePlayers = new List<BattlePlayer>();

    private void _updateFightFlag()
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);

        if (!BattlePlayer.IsDataValidBattlePlayer(player))
        {
            return;
        }

        BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        if (!BattlePlayer.IsDataValidBattlePlayer(mainPlayer))
        {
            return ;
        }

        if (mainPlayer.teamType != player.teamType)
        {
            _showStatusNone();
            return ;
        }

        votePlayers.Clear();
        notVotePlayers.Clear();

        BattleMain.instance.GetPlayerManager().GetTeamVotePlayers(votePlayers, player.teamType);
        BattleMain.instance.GetPlayerManager().GetTeamNotVotePlayers(notVotePlayers, player.teamType);

        Logger.LogProcessFormat("[战斗玩家] 投票人数 {0}, 未投票人数 {1}", votePlayers.Count, notVotePlayers.Count);

        // 没有可以出战的人了
        if (votePlayers.Count <= 0 && notVotePlayers.Count <= 0)
        {
            _showStatusNone();

            Logger.LogProcessFormat("[战斗玩家] 投票人数 {0}, 未投票人数 {1}, 没有可以出战的人了", votePlayers.Count, notVotePlayers.Count);
            return ;
        }

        // 所有人都没有投票
        if (votePlayers.Count <= 0)
        {
            Logger.LogProcessFormat("[战斗玩家] 投票人数 {0}, 未投票人数 {1}, 所有人都没有投票", votePlayers.Count, notVotePlayers.Count);

            if (notVotePlayers[0].GetPlayerSeat() == player.GetPlayerSeat())
            {
                Logger.LogProcessFormat("[战斗玩家] 投票人数 {0}, 未投票人数 {1}, 当前玩家是下一个出战的", votePlayers.Count, notVotePlayers.Count);
                _showStatusNextRound();
            }
            else
            {
                Logger.LogProcessFormat("[战斗玩家] 投票人数 {0}, 未投票人数 {1}, 当前玩家是不是出战的", votePlayers.Count, notVotePlayers.Count);
                _showStatusNone();
            }
            return ;
        }

        // 有人投票
        bool isShowVoted = player.isVote && !player.hasFighted;

        if (isShowVoted)
        {
            Logger.LogProcessFormat("[战斗玩家] 投票人数 {0}, 未投票人数 {1}, 显示当前玩家为申请状态", votePlayers.Count, notVotePlayers.Count);
            _showStatusApplyBattle();
        }
        else
        {
            Logger.LogProcessFormat("[战斗玩家] 投票人数 {0}, 未投票人数 {1}, 显示当前玩家为空的状态", votePlayers.Count, notVotePlayers.Count);
            _showStatusNone();
        }
    }

    private void _showStatusApplyBattle()
    {
        mRedtriangle.CustomActive(true);
        mRedtipapplybattle.CustomActive(true);
        mRedtipnextround.CustomActive(false);
    }

    private void _showStatusNextRound()
    {
        mRedtriangle.CustomActive(true);
        mRedtipapplybattle.CustomActive(false);
        mRedtipnextround.CustomActive(true);
    }

    private void _showStatusNone()
    {
        mRedtriangle.CustomActive(false);
        mRedtipapplybattle.CustomActive(false);
        mRedtipnextround.CustomActive(false);
    }
}
