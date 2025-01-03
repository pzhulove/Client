using UnityEngine;
using System.Collections;
using System;
using Protocol;

public interface IDungeonStatistics
{
    #region Score
    int RebornCountScore();

    int HitCountScore();

    int AllFightTimeScore(bool withClear);

    int FinalScore();

    DungeonScore FinalDungeonScore();

    int FightTimeSplit(int level);
    #endregion

    int AllDeadCount();

    int AllRebornCount();

    int AllHitCount();

    int RebornCount(byte seat);

    int DeadCount(byte seat);

    int HitCount(byte seat);

    int ItemUseCount(byte seat, int id);

    int SkillUseCount(byte seat, int id);

    int AllFightTime(bool withClear);

    int CurrentFightTime(bool withClear);

    /// <summary>
    /// 最近打过的房间的时间统计
    /// </summary>
    int LastFightTimeWithCount(bool withClear, int count);

    /// <summary>
    /// 最大伤害结构 
    /// </summary>
    DungeonHurtData GetCurrentMaxHurtData();

    DungeonHurtData GetAllMaxHurtData();

    UInt64 GetAllHurtDamage();

    /// <summary>
    /// 最大伤害 
    /// </summary>
    uint GetAllMaxHurtDamage();
    uint GetAllMaxHurtSkillID();
    uint GetAllMaxHurtID();
    uint GetBossDamage(byte seat);
    uint GetTotalBossDamage();
    /// <summary>
    /// 设置伤害数据
    /// </summary>
    void AddHurtData(int skillId, int hurtId, int damage);
    void AddBossHurtData(int attakcerId, int damage, int monsterID);

    int GetMatchPlayerVoteTimeLeft();

    void SetMatchPlayerVoteTimeLeft(int leftTime);
}

