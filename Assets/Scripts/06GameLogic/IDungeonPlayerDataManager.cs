using UnityEngine;
using System.Collections.Generic;
using GameClient;


/// <summary>
/// 战斗中玩家数据获取接口
/// </summary>
public interface IDungeonPlayerDataManager
{
    List<BattlePlayer> GetAllPlayers();

    BattlePlayer GetPlayerBySeat(byte seat = 0);

    BattlePlayer GetPlayerByRoleID(ulong roleId = 0);

    BattlePlayer GetMainPlayer();

    BattlePlayer GetFirstAlivePlayer();

    bool IsPlayerDead(byte seat);

    bool IsPlayerDeadByBattlePlayer(BattlePlayer player);

    bool IsAllPlayerDead();

    void SendMainPlayerLoadRate(int rate);

    string GetMainPlayerLoadInfo();

    void SetMainPlayerLoadInfo(string info);

    byte GetPlayerLoadRate(byte seat);

    /// <summary>
    /// 获得当前队伍类型
    /// </summary>
    BattlePlayer.eDungeonPlayerTeamType GetPlayerTeamType(byte seat);

    int GetPlayerCount();

    int GetSeatCount();

    //void SetSetCount(int count);

    /// <summary>
    /// 是否所有队员都战斗过了
    /// </summary>
    bool IsTeamPlayerAllFighted(BattlePlayer.eDungeonPlayerTeamType type);

    /// <summary>
    /// 是否所有队员都死了了
    /// </summary>
    bool IsTeamPlayerAllDead(BattlePlayer.eDungeonPlayerTeamType type);

    /// <summary>
    /// 根据队伍类型获取当前出站的队员
    /// </summary>
    byte GetCurrentTeamFightingPlayerSeat(BattlePlayer.eDungeonPlayerTeamType type);

    /// <summary>
    /// 根据队伍类型获取当前出站的队员
    /// </summary>
    BattlePlayer GetCurrentTeamFightingPlayer(BattlePlayer.eDungeonPlayerTeamType type);

    /// <summary>
    /// 设置队员预备出战
    /// </summary>
    void SetPlayerVoteFightState(byte seat, bool isFight);

    /// <summary>
    /// 获得对战玩家
    /// </summary>
    BattlePlayer GetTargetPlayer(byte seat);

    bool IsKillEnemyMatchPlayer(byte seat);

    /// <summary>
    /// 获取没有投票的可以出战玩家列表
    /// </summary>
    bool GetTeamNotVotePlayers(List<BattlePlayer> notVotePlayers, BattlePlayer.eDungeonPlayerTeamType type);

    /// <summary>
    /// 获取投票的可以出战玩家列表
    /// </summary>
    bool GetTeamVotePlayers(List<BattlePlayer> notVotePlayers, BattlePlayer.eDungeonPlayerTeamType type);


    /// <summary>
    /// 获取队伍人数
    /// </summary>
    int GetTeamPlayerCount(BattlePlayer.eDungeonPlayerTeamType type);

    /// <summary>
    /// 获取本地玩家队伍人数
    /// </summary>
    int GetMainPlayerTeamPlayerCount();
}

