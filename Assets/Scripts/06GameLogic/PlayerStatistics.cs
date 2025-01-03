using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using Protocol;

public class PlayerStatistics
{
    public static uint skFullHPRate = 10000;

    public class FightStatistics
    {
        /// <summary>
        /// 被击数目
        /// </summary>
        public int beHitCount
        {
            get 
            {
                return mBeHitCount;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mBeHitCount, {0} -> {1}", mBeHitCount, value);
                mBeHitCount = value;
            }
        }
        private int mBeHitCount= 0;
        private int mBossDamage = 0;
        public int BossDamage
        {
            get { return mBossDamage; }
            set { mBossDamage = value; }
        }
        /// <summary>
        /// 死亡次数
        /// </summary>
        public int deadCount
        {
            get 
            {
                return mDeadCount;
            }

            set 
            {
                Logger.LogProcessFormat("[战斗统计数据] mDeadCount, {0} -> {1}", mDeadCount, value);
                mDeadCount = value;
            }
        }
        private int mDeadCount= 0;

        public int rebornCount
        {
            get 
            {
                return mRebornCount;
            }

            set 
            {
                Logger.LogProcessFormat("[战斗统计数据] mRebornCount, {0} -> {1}", mRebornCount, value);
                mRebornCount = value;
            }
        }
        private int mRebornCount= 0;

        /// <summary>
        /// 最高连击
        /// </summary>
        public int maxCombo
        {
            get
            {
                return mMaxCombo;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mMaxCombo, {0} -> {1}", mMaxCombo, value);
                mMaxCombo = value;
            }
        }
        private int mMaxCombo = 0;

        /// <summary>
        /// 最大伤害
        /// </summary>
        public int maxHurt
        {
            get
            {
                return mMaxHurt;
            }

            set 
            {
                Logger.LogProcessFormat("[战斗统计数据] mMaxHurt, {0} -> {1}", mMaxHurt, value);
                mMaxHurt = value;
            }
        }
        private int mMaxHurt = 0;

    }

    /// <summary>
    /// 轮次比赛结果
    /// </summary>
    private class MatchRoundPKResoult
    {
        /// <summary>
        /// 比赛场次
        /// </summary>
        public int      roundIndex
        {
            get
            {
                return mRoundIndex;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mRoundIndex, {0} -> {1}", mRoundIndex, value);
                mRoundIndex = value;
            }
        }
        private int mRoundIndex;

        public byte     roundTargetSeat
        {
            get 
            {
                return mRoundTargetSeat;
            }
            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mRoundTargetSeat, {0} -> {1}", mRoundTargetSeat, value);
                mRoundTargetSeat = value;
            }
        }
        private byte mRoundTargetSeat;

        /// <summary>
        /// 轮次比赛结果
        /// </summary>
        public PKResult roundResult
        {
            get 
            {
                return mRoundResult;
            }

            set 
            {
                Logger.LogProcessFormat("[战斗统计数据] mRoundResult, {0} -> {1}", mRoundResult, value);
                mRoundResult = value;
            }
        }
        private PKResult mRoundResult;

        public ePVPBattleEndType endType
        {
            get 
            {
                return mEndType;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mEndType, {0} -> {1}", mEndType, value);
                mEndType = value;
            }
        }
        private ePVPBattleEndType mEndType = ePVPBattleEndType.onNone;


        public uint SelfHPRate
        {
            get
            {
                return mSelfHPRate;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mSelfHPRate, {0} -> {1}",  mSelfHPRate, value);
                mSelfHPRate = value;
            }
        }

        private UInt32 mSelfHPRate = 0;
    }

    public class MatchRacePlayerResult
    {
        /// <summary>
        /// pk值
        /// </summary>
        public UInt32 pkValue
        {
            get 
            {
                return mPkValue;
            }

            set 
            {
                Logger.LogProcessFormat("[战斗统计数据] mPkValue, {0} -> {1}", mPkValue, value);
                mPkValue = value;
            }
        }
        private UInt32 mPkValue;

        /// <summary>
        /// 匹配值 
        /// </summary>
        public UInt32 matchScore
        {
            get 
            {
                return mMatchScore;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mMatchScore, {0} -> {1}", mMatchScore, value);
                mMatchScore = value;
            }
        }
        private UInt32 mMatchScore;

        /// <summary>
        ///  初始决斗币数量
        /// </summary>
        public UInt32 pkCoin
        {
            get 
            {
                return mPkCoin;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mPkCoin, {0} -> {1}", mPkCoin, value);
                mPkCoin = value;
            }
        }
        private UInt32 mPkCoin;

        /// <summary>
        ///  原段位
        /// </summary>
        public UInt32 seasonLevel
        {
            get {
                return mSeasonLevel;
            }
            set {
                Logger.LogProcessFormat("[战斗统计数据] mSeasonLevel, {0} -> {1}", mSeasonLevel, value);
                mSeasonLevel = value;
            }
        }
        private UInt32 mSeasonLevel;


        /// <summary>
        /// 原星
        /// </summary>
        public UInt32 seasonStar
        {
            get {
                return mSeasonStar;
            }
            set {
                Logger.LogProcessFormat("[战斗统计数据] mSeasonStar, {0} -> {1}", mSeasonStar, value);
                mSeasonStar = value;
            }
        }
        private UInt32 mSeasonStar;

        /// <summary>
        /// 原经验
        /// </summary>
        public UInt32 seasonExp
        {
            get
            {
                return mSeasonExp;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mSeasonExp, {0} -> {1}", mSeasonExp, value);
                mSeasonExp = value;
            }
        }

        private UInt32 mSeasonExp;

        private UInt32 mScoreWarContriScore = UInt32.MaxValue;
        public uint scoreWarContriScore
        {
            get
            {
                return mScoreWarContriScore;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mScoreWarContriScore, {0} -> {1}", mScoreWarContriScore, value);
                mScoreWarContriScore = value;
            }
        }

        public uint scoreWarBaseScore
        {
            get
            {
                return mScoreWarBaseScore;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mScoreWarBaseScore, {0} -> {1}", mScoreWarBaseScore, value);
                mScoreWarBaseScore = value;
            }
        }

        private UInt32 mScoreWarBaseScore = UInt32.MaxValue;

        /// <summary>
        /// 获得的荣誉值
        /// </summary>
        public UInt32 changeGlory
        {
            get
            {
                return mChangeGlory;
            }
            set
            {
                Logger.LogProcessFormat("[] mChangeGlory, {0} -> {1}", mChangeGlory, value);
                mChangeGlory = value;
            }
        }
        private UInt32 mChangeGlory = 0;
    }

    /// <summary>
    /// 整场比赛结果
    /// </summary>
    public class MatchRacePKResult
    {
        /// <summary>
        /// 比赛结果
        /// </summary>
        public PKResult raceResult
        {
            get
            {
                return mRaceResult;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mRaceResult, {0} -> {1}", mRaceResult, value);
                mRaceResult = value;
            }
        }
        private PKResult mRaceResult = PKResult.INVALID;


        /// <summary>
        /// 房间ID
        /// </summary>
        public UInt32   roomId
        {
            get 
            {
                return mRoomId;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mRoomId, {0} -> {1}", mRoomId, value);

                mRoomId = value;
            }
        }
        private UInt32 mRoomId;

        /// <summary>
        /// 房间类型
        /// </summary>
        public RoomType roomType 
        {
            get 
            {
                return mRoomType;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mRoomType, {0} -> {1}", mRoomType, value);

                mRoomType = value;
            }
        } 
        private RoomType mRoomType = RoomType.ROOM_TYPE_INVALID;

        /// <summary>
        ///  战斗获得的决斗币
        /// </summary>
        public UInt32   addPkCoinFromRace
        {
            get 
            {
                return mAddPkCoinFromRace;
            }

            set 
            {
                Logger.LogProcessFormat("[战斗统计数据] mAddPkCoinFromRace, {0} -> {1}", mAddPkCoinFromRace, value);
                mAddPkCoinFromRace = value;
            }
        }
        private UInt32 mAddPkCoinFromRace;

        /// <summary>
        ///  今日战斗获得的全部决斗币
        /// </summary>  
        public UInt32   totalPkCoinFromRace
        {
            get 
            {
                return mTotalPkCoinFromRace;
            }

            set 
            {
                Logger.LogProcessFormat("[战斗统计数据] mTotalPkCoinFromRace, {0} -> {1}", mTotalPkCoinFromRace, value);
                mTotalPkCoinFromRace = value;
            }
        }
        private UInt32 mTotalPkCoinFromRace;

        /// <summary>
        ///  是否在PVP活动期间
        /// </summary>
        public byte     isInPvPActivity
        {
            get
            {
                return mIsInPvPActivity;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mIsInPvPActivity, {0} -> {1}", mIsInPvPActivity, value);
                mIsInPvPActivity = value;
            }
        }
        private byte mIsInPvPActivity;

        /// <summary>
        ///  活动额外获得的决斗币
        /// </summary>
        public UInt32   addPkCoinFromActivity
        {
            get
            {
                return mAddPkCoinFromActivity;
            }

            set 
            {
                Logger.LogProcessFormat("[战斗统计数据] mAddPkCoinFromActivity, {0} -> {1}", mAddPkCoinFromActivity, value);
                mAddPkCoinFromActivity = value;
            }
        }
        private UInt32 mAddPkCoinFromActivity;

        /// <summary>
        ///  今日活动获得的全部决斗币
        /// </summary>
        public UInt32   totalPkCoinFromActivity
        {
            get
            {
                return mTotalPkCoinFromActivity;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mTotalPkCoinFromActivity, {0} -> {1}", mTotalPkCoinFromActivity, value);
                mTotalPkCoinFromActivity = value;
            }
        }
        private UInt32 mTotalPkCoinFromActivity;

		/// <summary>
		/// 改变的经验
		/// </summary>
		public Int32    changeSeasonExp
        {
            get 
            {
                return mChangeSeasonExp;
            }

            set
            {
                Logger.LogProcessFormat("[战斗统计数据] mChangeSeasonExp, {0} -> {1}", mChangeSeasonExp, value);
                mChangeSeasonExp = value;
            }
        }
        private Int32 mChangeSeasonExp;


        public MatchRacePlayerResult beforeRaceResult = new MatchRacePlayerResult();
        public MatchRacePlayerResult afterRaceResult  = null;
    }

    public MatchRacePKResult raceResult     = new MatchRacePKResult();

    /// <summary>
    /// PK战斗结束之后的排名
    /// </summary>
    public int               raceResultRank = 1;
   
    /// <summary>
    /// 道具使用量
    /// 药品buff， 复活币等
    /// </summary>
    List<int> mUsedItem = new List<int>();

    /// <summary>
    /// 技能使用
    /// </summary>
    List<int> mUsedSkill = new List<int>();

    /// <summary>
    /// 杀死的怪物
    /// </summary>
    List<int> mKilledMonster = new List<int>();

    /// <summary>
    /// 战斗数据
    /// </summary>
    public FightStatistics data = new FightStatistics();

    /// <summary>
    /// PK战斗结果
    /// </summary>
    private List<MatchRoundPKResoult> pkResults    = new List<MatchRoundPKResoult>();

    public int killPlayers = 0;

    private void _notify()
    {
        //Logger.LogError(ObjectDumper.Dump(data));
        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonScoreChanged);
    }

    public void Reset()
    {
        mUsedItem.Clear();
        mUsedSkill.Clear();
        mKilledMonster.Clear();
        data = new FightStatistics();
        pkResults = new List<MatchRoundPKResoult>();
    }

    public void AddPKResult(int roundIndex, byte roundTargetSeat, PKResult roundResult, ePVPBattleEndType endType, uint selfHpRate)
    {
        int lastRoundIndex = GetLastPKRoundIndex();

        if (roundIndex <= lastRoundIndex)
        {
            Logger.LogProcessFormat("[战斗] 无法新加比赛结果 对手{0}, 上一轮 {1}, 新一轮 {2}, 结束类型 {3}",  roundTargetSeat, lastRoundIndex, roundIndex, endType);
            return ;
        }

        MatchRoundPKResoult matchRes  = new MatchRoundPKResoult();

        matchRes.roundIndex      = roundIndex;
        matchRes.roundResult     = roundResult;

        matchRes.roundTargetSeat = roundTargetSeat;
        matchRes.endType = endType;
        matchRes.SelfHPRate = selfHpRate;

        Logger.LogProcessFormat("[战斗] 添加比赛结果 对手 {0}, 第 {1} 轮, 结果 {2}, 结束类型 {3}", roundTargetSeat, roundIndex, roundResult, endType);

        pkResults.Add(matchRes);
    }

    public int GetLastPKRoundIndex()
    {
        if (pkResults.Count <= 0)
        {
            return -1;
        }

        return pkResults[pkResults.Count - 1].roundIndex;
    }

    public uint GetLastHurtedRate()
    {
        if (pkResults.Count <= 0)
        {
            return 0;
        }

        if (pkResults.Count == 1 )
        {
            return PlayerStatistics.skFullHPRate - pkResults[pkResults.Count - 1].SelfHPRate;
        }
        else
        {
            uint smallerRate = pkResults[pkResults.Count - 1].SelfHPRate;
            uint biggerRate = pkResults[pkResults.Count - 2].SelfHPRate;

            if (smallerRate > biggerRate)
            {
                return 0;
            }
            else
            {
                return biggerRate - smallerRate;
            }
        }
    }

    public int GetKillMatchPlayerCount()
    {
        int count = 0;

        for (int i = 0; i < pkResults.Count; ++i)
        {
            if (ePVPBattleEndType.onAllEnemyDied == pkResults[i].endType
             && PKResult.WIN == pkResults[i].roundResult)
            {
                count++;
            }
        }

        return count;
    }

    public PKResult GetLastPKResult()
    {
        if (pkResults.Count <= 0)
        {
            return PKResult.INVALID;
        }

        return pkResults[pkResults.Count - 1].roundResult;
    }

    public void UseItem(int id, int num)
    {
        for (int i = 0; i < num; ++i)
        {
            mUsedItem.Add(id);
        }

        _notify();
    }

    public void UseSkill(int skill)
    {
        mUsedSkill.Add(skill);
        _notify();
    }

    public void OnHit()
    {
        data.beHitCount++;
        _notify();
    }

    public void Reborn()
    {
        data.rebornCount++;
        _notify();
    }

    public void Dead()
    {
        data.deadCount++;
        _notify();
    }
}
