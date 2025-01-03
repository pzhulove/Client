using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using GameClient;
using Protocol;
using ProtoTable;
using Network;

/// <summary>
/// 玩家数据
/// </summary>
public class DungeonPlayerDataManager : IDungeonPlayerDataManager
{
    /// <summary>
    /// 玩家数据
    /// 不删除或者修改任何数据
    /// </summary>
    protected List<BattlePlayer> mBattlePlayers    = new List<BattlePlayer>();

    protected eDungeonMode       mDungeonMode      = eDungeonMode.None;
    protected BattleType         mBattleType       = BattleType.None;

    protected BeDungeon          mBeDungeon        = null;

    private List<BattlePlayer>   mCacheTeamPlayers = new List<BattlePlayer>();

    /// <summary>
    /// 构造此对象的时候需要保证BattleDataManager.GetInstance().PlayerInfo数据正确
    /// </summary>
    public DungeonPlayerDataManager(BattleType type, eDungeonMode mode, BeDungeon beDungeon)
    {
        mDungeonMode = mode;
        mBattleType  = type;
        mBeDungeon   = beDungeon;

        _prepareDebugData(mode);

        mBattlePlayers.Clear();
        for (int i = 0; i < BattleDataManager.GetInstance().PlayerInfo.Length; ++i)
        {
            var playerInfo = BattleDataManager.GetInstance().PlayerInfo[i];
            BattlePlayer player = new BattlePlayer
            {
                playerActor = null,
                playerInfo = playerInfo,
                state = BattlePlayer.EState.Normal,
                netState = BattlePlayer.eNetState.Online,
                netQuality = BattlePlayer.eNetQuality.Best,
                teamType = GetPlayerTeamType(playerInfo.seat)
            };


            Logger.LogProcessFormat("[战斗玩家] 添加玩家:{0} 座位号:{1}, accid {2}, roleID {3}", 
                    player.playerInfo.name,
                    player.playerInfo.seat,
                    player.playerInfo.accid,
                    player.playerInfo.roleId);

            for (int j = 0; j < player.playerInfo.potionPos.Length; ++j)
            {
                Logger.LogProcessFormat("[战斗玩家] 玩家:{0} 座位号:{1}, 药配置 {2}", 
                        player.playerInfo.name,
                        player.playerInfo.seat,
                        player.playerInfo.potionPos[j]);
            }

            mBattlePlayers.Add(player);
        }

        _bindNetMessage();

        Logger.LogProcessFormat("[战斗玩家] 初始化完成");
    }

    public void Clear()
    {
        mDungeonMode = eDungeonMode.None;
        _unbindNetMessage();
        mBattlePlayers.Clear();

        Logger.LogProcessFormat("[战斗玩家] 反初始化完成");
    }

	RacePlayerInfo GetPlayerTemplate(int seat)
	{
        RacePlayerInfo info = new RacePlayerInfo
        {
            level = 50,//PlayerBaseData.GetInstance().Level;
            name = PlayerBaseData.GetInstance().Name,
            seat = (byte)seat,
            occupation = (byte)PlayerBaseData.GetInstance().JobTableID,
            accid = seat == 0 ? ClientApplication.playerinfo.accid : 999
        };
#if DEBUG_SETTING
        if (Global.Settings.isDebug && Global.Settings.isGiveEquips)
		{
			var tokens = Global.Settings.equipList.Split(',');
			List<int> equipIDs = new List<int>();
			for(int i=0; i<tokens.Length; ++i)
			{
				int eid = Convert.ToInt32(tokens[i]);
				equipIDs.Add(eid);
			}

			List<RaceEquip> equips = BeUtility.GetEquips(equipIDs.ToArray());
			info.equips = equips.ToArray();
		}
#endif
        List<RacePet> listPet = new List<RacePet>
        {
            new RacePet()
            {
                dataId = (uint)Global.Settings.petID,
                level = (ushort)Global.Settings.petLevel,
                hunger = (ushort)Global.Settings.petHunger,
                skillIndex = (byte)Global.Settings.petSkillIndex,
            }
        };

        info.pets = listPet.ToArray ();

		GameClient.NewbieGuideBattle.InitNewbieGuidePlayerInfo(ref info);

		{
            var list = new List<RaceSkillInfo>();
            if (Global.Settings.startSystem == GameClient.EClientSystem.Battle)
            {
                var kv = TableManager.instance.GetSkillInfoByPid(PlayerBaseData.GetInstance().JobTableID);
                var iter = kv.GetEnumerator();
                while (iter.MoveNext())
                {
                    var data = TableManager.instance.GetTableItem<SkillTable>(iter.Current.Key);
                    if (data != null && iter.Current.Key <= 10000)
                    {
                        list.Add(new RaceSkillInfo()
                        {
                            id = (ushort)iter.Current.Key,
                            level = (byte)iter.Current.Value,
                            slot = 0,
                        });
                    }
                }
                info.skills = list.ToArray();
            }
            else
            {
                var skillList = GameClient.NewbieGuideBattle.GetNewbieGuidePlayerSkills();
                for (int i = 0; i < skillList.Count; ++i)
                {
                    list.Add(new RaceSkillInfo()
                    {
                        id = (ushort)skillList[i],
                        level = 1,
                        slot = 0,
                    });
                }

                info.skills = list.ToArray();
            }
        }

		return info;
	}

    //添加数据初始化
    //[Conditional("UNITY_EDITOR")]
    private void _prepareDebugData(eDungeonMode mode)
    {
        if (eDungeonMode.Test == mode)
        {
			RacePlayerInfo[] playerInfos = new RacePlayerInfo[Global.Settings.testPlayerNum];
			for(int i=0; i<Global.Settings.testPlayerNum; ++i)
			{
				playerInfos[i] = GetPlayerTemplate(i);
			}

			BattleDataManager.GetInstance().PlayerInfo = playerInfos;
        }
    }

    public List<BattlePlayer> GetAllPlayers()
    {
        if (null == mBattlePlayers)
        {
            mBattlePlayers = new List<BattlePlayer>();
        }

        return mBattlePlayers;
    }


    public BattlePlayer GetPlayerBySeat(byte seat)
    {
        BattlePlayer player = null;

        for (int i = 0; i < mBattlePlayers.Count; ++i)
        {
            if (null != mBattlePlayers[i].playerInfo && seat == mBattlePlayers[i].playerInfo.seat)
            {
                player = mBattlePlayers[i];
                break;
            }
        }

        return player;
    }

    public BattlePlayer GetPlayerByRoleID(ulong roleId)
    {
        BattlePlayer player = null;

        for (int i = 0; i < mBattlePlayers.Count; ++i)
        {
            if (null != mBattlePlayers[i].playerInfo && roleId == mBattlePlayers[i].playerInfo.roleId)
            {
                player = mBattlePlayers[i];
                break;
            }
        }

        return player;
    }

    public BattlePlayer GetMainPlayer()
    {
        BattlePlayer player = null;

		if (ReplayServer.GetInstance().IsReplay())
        {
            player = mBattlePlayers[0];
        }
        else
        {
            for (int i = 0; i < mBattlePlayers.Count; ++i)
            {
                if (null != mBattlePlayers[i].playerInfo && ClientApplication.playerinfo.accid == mBattlePlayers[i].playerInfo.accid)
                {
                    player = mBattlePlayers[i];
                    break;
                }
            }
        }

        return player;
    }

    public bool IsPlayerDeadByBattlePlayer(BattlePlayer player)
    {
        return null != player && null != player.playerActor && player.playerActor.IsDead();
    }

    public bool IsPlayerDead(byte seat)
    {
        var player = GetPlayerBySeat(seat);
        return IsPlayerDeadByBattlePlayer(player);
    }

    public bool IsAllPlayerDead()
    {
        for (int i = 0; i < mBattlePlayers.Count; ++i)
        {
            if (!IsPlayerDeadByBattlePlayer(mBattlePlayers[i]))
            {
                return false;
            }
        }

        return true;
    }

    public BattlePlayer GetFirstAlivePlayer()
    {
        for (int i = 0; i < mBattlePlayers.Count; ++i)
        {
            if (!IsPlayerDeadByBattlePlayer(mBattlePlayers[i]))
            {
                return mBattlePlayers[i];
            }
        }

        return null;
    }

    public void SendMainPlayerLoadRate(int rate)
    {
		if (ReplayServer.GetInstance().IsReplay() && !ReplayServer.GetInstance().IsLiveShow())
			return;

        if (eDungeonMode.SyncFrame == mDungeonMode)
        {
            byte convertRate = _getFinalRate(rate);

            Logger.LogProcessFormat("[战斗玩家] 通知服务器主玩家加载进度 {0}", convertRate);

            RelaySvrReportLoadProgress req = new RelaySvrReportLoadProgress
            {
                progress = convertRate
            };
            NetManager.Instance().SendCommand(ServerType.RELAY_SERVER, req);

            _updateMainPlayerProcess(convertRate);
        }
    }

    private byte _getFinalRate(int rate)
    {
        if (rate < 0)
        {
            return 0;
        }

        if (rate > BattlePlayer.kFullLoadRate)
        {
            return BattlePlayer.kFullLoadRate;
        }

        return (byte)rate;
    }

    private void _updateMainPlayerProcess(byte progress)
    {
        BattlePlayer mainPlayer = GetMainPlayer();
        if (null != mainPlayer)
        {
            _updatePlayerLoadProcess(mainPlayer.playerInfo.seat, progress);
        }
    }

    private void _updatePlayerLoadProcess(byte seat, byte progress)
    {
        BattlePlayer player = GetPlayerBySeat(seat);
        if (null != player)
        {
            Logger.LogProcessFormat("[战斗玩家] 更新玩家{0}:{1} 进度{2}", seat, player.playerInfo.name, progress);
            player.loadRate = progress;
        }
        else
        {
            Logger.LogProcessFormat("[战斗玩家] 更新玩家{0} 失败", seat);
        }
    }

    public string GetMainPlayerLoadInfo()
    {
        return string.Empty;
    }

    public void SetMainPlayerLoadInfo(string info)
    {
    }

    public byte GetPlayerLoadRate(byte seat)
    {
        BattlePlayer player = GetPlayerBySeat(seat);

        if (null != player)
        {
            return player.loadRate;
        }

        return 0;
    }


    private void _bindNetMessage()
    {
        NetProcess.AddMsgHandler(RelaySvrNotifyLoadProgress.MsgID, _dungeonPlayerDataMsgBinder);
    }

    private void _unbindNetMessage()
    {
        NetProcess.RemoveMsgHandler(RelaySvrNotifyLoadProgress.MsgID, _dungeonPlayerDataMsgBinder);
    }

    private void _dungeonPlayerDataMsgBinder(MsgDATA data)
    {
        if (RelaySvrNotifyLoadProgress.MsgID == data.id)
        {
            RelaySvrNotifyLoadProgress res = new RelaySvrNotifyLoadProgress();
            res.decode(data.bytes);

            _onRelaySvrNotifyLoadProgress(res);
        }
    }

    private void _onRelaySvrNotifyLoadProgress(RelaySvrNotifyLoadProgress res)
    {
        _updatePlayerLoadProcess(res.pos, res.progress);
    }

    public BattlePlayer.eDungeonPlayerTeamType GetPlayerTeamType(byte seat)
    {
        if ((int)seat < _getTeamCount())
        {
            return BattlePlayer.eDungeonPlayerTeamType.eTeamRed;
        }
        else
        {
            return BattlePlayer.eDungeonPlayerTeamType.eTeamBlue;
        }
    }

    public int GetPlayerCount()
    {
        if (null == mBattlePlayers)
        {
            return 0;
        }

        return mBattlePlayers.Count;
    }

    private const int kPK3V3SeatCount  = 6;
    private const int kSingleSeatCount = 2;
    private const int kPK2V2SeatCount = 4;
    public int GetSeatCount()
    {
        if (mDungeonMode != eDungeonMode.SyncFrame)
        {
            return kSingleSeatCount;
        }

        if (mBattleType == BattleType.PVP3V3Battle)
        {
            return kPK3V3SeatCount;
        }

        if (mBattleType == BattleType.ScufflePVP)
        {
            return kPK2V2SeatCount;
        }

        return kSingleSeatCount;
    }

    private int _getTeamCount()
    {
        return GetSeatCount() / 2;
    }

    
    public BattlePlayer GetCurrentTeamFightingPlayer(BattlePlayer.eDungeonPlayerTeamType type)
    {
        for (int i = 0; i < mBattlePlayers.Count; ++i)
        {
            if (mBattlePlayers[i].teamType == type)
            {
                if (mBattlePlayers[i].isFighting)
                {
                    return mBattlePlayers[i];
                }
            }
        }

        return null;
    }

    public byte GetCurrentTeamFightingPlayerSeat(BattlePlayer.eDungeonPlayerTeamType type)
    {
        BattlePlayer player = GetCurrentTeamFightingPlayer(type);

        if (!BattlePlayer.IsDataValidBattlePlayer(player))
        {
            return byte.MaxValue;
        }
        
        return player.playerInfo.seat;
    }

    public void SetPlayerVoteFightState(byte seat, bool isFight)
    {
        BattlePlayer player = GetPlayerBySeat(seat);
        if (null == player)
        {
            return ;
        }

        player.isVote = isFight;
    }

    public bool GetPlayerVoteFightState(byte seat)
    {
        BattlePlayer player = GetPlayerBySeat(seat);

        if (null == player)
        {
            return false;
        }

        return player.isVote;
    }

    public bool IsTeamPlayerAllFighted(BattlePlayer.eDungeonPlayerTeamType type)
    {
        for (int i = 0; i < GetPlayerCount(); ++i)
        {
            if (mBattlePlayers[i].teamType == type && !mBattlePlayers[i].isPassedInRound)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsTeamPlayerAllDead(BattlePlayer.eDungeonPlayerTeamType type)
    {
        for (int i = 0; i < GetPlayerCount(); ++i)
        {
            if (mBattlePlayers[i] != null && mBattlePlayers[i].playerActor != null)
            {
                if (mBattlePlayers[i].teamType == type && !mBattlePlayers[i].playerActor.IsDead())
                {
                    return false;
                }
            }
        }

        return true;
    }

    public BattlePlayer GetTargetPlayer(byte seat)
    {
        BattlePlayer curPlayer = GetPlayerBySeat(seat);

        if (!BattlePlayer.IsDataValidBattlePlayer(curPlayer))
        {
            return null;
        }

        BattlePlayer.eDungeonPlayerTeamType targetTeamType = BattlePlayer.GetTargetTeamType(curPlayer.teamType);
        return GetCurrentTeamFightingPlayer(targetTeamType);
    }

    private int _getTeamCount(BattlePlayer.eDungeonPlayerTeamType type)
    {
        int count = 0;

        for (int i = 0; i < mBattlePlayers.Count; ++i)
        {
            if (mBattlePlayers[i].teamType == type)
            {
                count++;
            }
        }

        return count;
    }

    public bool IsKillEnemyMatchPlayer(byte seat)
    {
        BattlePlayer curPlayer = GetPlayerBySeat(seat);

        if (!BattlePlayer.IsDataValidBattlePlayer(curPlayer))
        {
            return false;
        }

        int killedNumber = curPlayer.GetKillMatchPlayerCount();

        if (killedNumber <= 0)
        {
            return false;
        }

        int enemyTeamCount = _getTeamCount(BattlePlayer.GetTargetTeamType(curPlayer.teamType));

        return killedNumber >= enemyTeamCount;
    }

    public bool GetTeamVotePlayers(List<BattlePlayer> votePlayers, BattlePlayer.eDungeonPlayerTeamType type)
    {
        if (null == votePlayers)
        {
            return false;
        }

        List<BattlePlayer> battlePlayers = GetAllPlayers();

        for (int i = 0; i < battlePlayers.Count; ++i)
        {
            if (battlePlayers[i].teamType != type)
            {
                continue;
            }

            if (battlePlayers[i].hasFighted)
            {
                continue;
            }

            if (battlePlayers[i].isFighting)
            {
                continue;
            }

            if (battlePlayers[i].isVote)
            {
                votePlayers.Add(battlePlayers[i]);
            }
        }

        return true;
    }

    public bool GetTeamNotVotePlayers(List<BattlePlayer> notVotePlayers, BattlePlayer.eDungeonPlayerTeamType type)
    {
        if (null == notVotePlayers)
        {
            return false;
        }

        List<BattlePlayer> battlePlayers = GetAllPlayers();

        for (int i = 0; i < battlePlayers.Count; ++i)
        {
            if (battlePlayers[i].teamType != type)
            {
                continue;
            }

            if (battlePlayers[i].hasFighted)
            {
                continue;
            }

            if (battlePlayers[i].isFighting)
            {
                continue;
            }

            if (!battlePlayers[i].isVote)
            {
                notVotePlayers.Add(battlePlayers[i]);
            }
        }

        return true;
    }


    public int GetTeamPlayerCount(BattlePlayer.eDungeonPlayerTeamType type)
    {
        int cnt = 0;

        for (int i = 0; i < mBattlePlayers.Count; ++i)
        {
            if (mBattlePlayers[i].teamType == type)
            {
                cnt++;
            }
        }

        return cnt;
    }

    public int GetMainPlayerTeamPlayerCount()
    {
        BattlePlayer player = GetMainPlayer();

        if (!BattlePlayer.IsDataValidBattlePlayer(player))
        {
            return 0;
        }

        return GetTeamPlayerCount(player.teamType);
    }
}
