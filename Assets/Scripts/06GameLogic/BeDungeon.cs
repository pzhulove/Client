using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Network;
using System;
using System.Diagnostics;


using GameClient;
using ProtoTable;

/// <summary>
/// 管理整个地下城
/// 
/// 提供各种创建，查询，删除接口
/// </summary>
public class BeDungeon : IDungeonManager, IDungeonStatistics, IDungeonEnumeratorManager
{
    
    
    #region 数据
    /// <summary>
    /// 地下城数据
    /// </summary>
    protected DungeonDataManager mDungeonData;

    /// <summary>
    /// 玩家数据
    /// </summary>
    protected DungeonPlayerDataManager mDungeonPlayers;

    /// <summary>
    /// 当前游戏场景
    /// 
    /// 如果逻辑表现分离，这里需要有一个 `mBeScenes`
    /// </summary>
    protected BeScene mBeScene;

    /// <summary>
    /// 当前模式
    /// </summary>
    protected eDungeonMode mMode = eDungeonMode.None;

    /// <summary>
    /// 切换场景GC上限
    /// </summary>
    private const int kSwitchLimit = 5;

    /// <summary>
    /// 切换场景的次数统计
    /// </summary>
    protected int mSwitchCount = 0;

    /// <summary>
    /// 暂停时的，上一个状态
    /// </summary>
    protected BeSceneState mPreBeSceneState    = BeSceneState.onNone;

    protected string       mPrePauseBeSceneTag = "";

    private int mDelta = 0;

    protected int mMatchPKLeftVoteTime = 0;
    private int mBossDamage = 0;
    public uint bossID01 = 0;
    public uint bossID02 = 0;
    public uint bossDamage01 = 0;
    public uint bossDamage02 = 0;
    private IEnumeratorManager mEnumertorManager = new EnumeratorProcessManager();

    // 嵌套 
    private List<List<IEnumerator>> mStackEnumerators = new List<List<IEnumerator>>();

    #endregion

    /// <summary>
    /// 根据这个去决定是否需要使用服务端下发的数据
    /// </summary>
    public BeDungeon(BattleType type, eDungeonMode mode, int dungeonID)
    {
        mMode           = mode;
        mDungeonData    = new DungeonDataManager(type, mode, dungeonID);
        mDungeonPlayers = new DungeonPlayerDataManager(type, mode, this);
        mSwitchCount    = 0;
        mBossDamage = 0;
        bossID01 = 0;
        bossID02 = 0;
        bossDamage01 = 0;
        bossDamage02 = 0;
        Logger.LogProcessFormat("[战斗地下城] 初始化完成");
    }

    public BaseBattle mBattle
    {
        get;set;
    }

    public DungeonPlayerDataManager GetDungeonPlayerDataManager()
    {
        return mDungeonPlayers;
    }

    #region IDungeonEnumeratorManager implementation

    public IEnumerator AddEnumerator(IEnumerator iter, int priority = int.MaxValue)
    {
        if (null != mEnumertorManager)
        {
            return mEnumertorManager.AddEnumerator(iter, priority);
        }

        return null;
    }

    public void RemoveEnumerator(IEnumerator iter)
    {
        if (null != mEnumertorManager)
        {
            mEnumertorManager.RemoveEnumerator(iter);
        }
    }

    public void ClearAllEnumerators()
    {
        if (null != mEnumertorManager)
        {
            mEnumertorManager.ClearAllEnumerators();
        }
    }
    #endregion

    #region DungeonManager
    public BeScene CreateBeScene()
    {
        if (null != mBeScene)
        {
            mBeScene.ReloadScene(mDungeonData.CurrentScenePath());
        }
        else
        {
            mBeScene = BeScene.CreateScene(mDungeonData.CurrentScenePath(), mBattle);
            mBeScene.singleBloodBarCount = mDungeonData.table.SingleBarValue;
        }

        if (null != mBeScene)
        {
            mBeScene.TriggerEventNew(BeEventSceneType.onCreate);
        }

        return mBeScene;
    }

    public void DestoryBeScene()
    {
        mSwitchCount = 0;

        ClearAllEnumerators();

        if (null != mDungeonData)
        {
            mDungeonData.Clear();
            mDungeonData = null;
        }

        if (null != mDungeonPlayers)
        {
            mDungeonPlayers.Clear();
            mDungeonPlayers = null;
        }

        if (null != mBeScene)
        {
            mBeScene.UnRegisterAll();
            mBeScene.Unload();
            mBeScene = null;
        }

        Logger.LogProcessFormat("[战斗地下城] 反初始化完成");
    }

    public BeScene GetBeScene()
    {
        return mBeScene;
    }

    public GeSceneEx GetGeScene()
    {
        if (null != mBeScene)
        {
            return mBeScene.currentGeScene;
        }

        return null;
    }

    public DungeonDataManager GetDungeonDataManager()
    {
        return mDungeonData;
    }

    public void StartFight(bool isFinishFight = false)
    {
		if (mBattle != null && mBattle.recordServer!=null && mBattle.recordServer.IsProcessRecord())
		{
			mBattle.recordServer.RecordProcess("[BATTLE]StartFight dungeon id:{0}", GetDungeonDataManager().id.dungeonID);
            mBattle.recordServer.MarkInt(0x8779794, GetDungeonDataManager().id.dungeonID);
            // Mark:0x8779794 [BATTLE]StartFight dungeon id:{0}
        }

        if (null != mBeScene)
        {
            //针对于团本联动 服务器发送其他团队结算达到自己胜利条件后，自己结算的判断保护
            if (!isFinishFight)
            {
                mBeScene.state = BeSceneState.onReady;
                mBeScene.OnReady();
            }
            else
            {
                mBeScene.state = BeSceneState.onFinish;
            }
        }
    }
    public void DoGraphicBackToFront()
    {
#if !SERVER_LOGIC
        if (null != mBeScene)
        {
            if (mBeScene.currentGeScene != null)
                mBeScene.currentGeScene.DoBackToFront();
            for (int i = 0; i < mBeScene.GetEntityCount(); i++)
            {
                var curEntity = mBeScene.GetEntityAt(i) as BeActor;
                if (curEntity != null)
                {
                    curEntity.TriggerEventNew(BeEventType.onBackModeEnd);
                }
            }
            var pendingEntities = mBeScene.GetPendingEntities();
            for (int i = 0; i < pendingEntities.Count; i++)
            {
                var curEntity = pendingEntities[i] as BeActor;
                if (curEntity != null)
                {
                    curEntity.TriggerEventNew(BeEventType.onBackModeEnd);
                }
            }
        }
#endif
    }

    public void FinishFight()
    {
        mDungeonData.FinishUpdateFrameData();

#if !SERVER_LOGIC 
        GameClient.ClientReconnectManager.instance.canReconnectRelay = false;
        if (FrameSync.instance.IsInChasingMode)
            FrameSync.instance.EndChasingMode();
        FrameSync.instance.SetDungeonMode(eDungeonMode.LocalFrame);
        if (ReplayServer.GetInstance().IsLiveShow())
        {
            if (Network.NetManager.instance != null)
                Network.NetManager.instance.Disconnect(ServerType.RELAY_SERVER);
        }
#endif

        if (mBattle != null && mBattle.recordServer!=null && mBattle.recordServer.IsProcessRecord())
		{
			mBattle.recordServer.RecordProcess("[BATTLE]FinishFight");
            mBattle.recordServer.Mark(0x6666666);
            // Mark:0x6666666 [BATTLE]FinishFight
        }
        if (null != mBeScene)
        {
            mBeScene.state = BeSceneState.onFinish;
        }


        if(mBattle != null && mBattle.recordServer != null)
        {
            //!!验证服务器，暂时不记录录像
            mBattle.recordServer.EndRecordProcess();
        }

#if !SERVER_LOGIC
        TMBattleAssetLoadRecord.instance.SaveInfoToFile();
#endif

        //mBattle.End();
    }

    public bool IsFinishFight()
    {
        if (null != mBeScene)
        {
            return BeSceneState.onFinish == mBeScene.state;
        }

        return true;
    }

	public void PauseFight(bool pauseAnimation=true, string tag = "", bool force = false)
    {
        if (string.IsNullOrEmpty(mPrePauseBeSceneTag) || force)
        {
            mPrePauseBeSceneTag = tag;
        }

        //if (mBattle != null && mBattle.recordServer!=null && mBattle.recordServer.IsProcessRecord())
        //{
        //	mBattle.recordServer.RecordProcess("[BATTLE]PauseFight");
        //}

        if (null != mBeScene && (mMode != eDungeonMode.SyncFrame || ReplayServer.GetInstance().IsReplay())&& mBeScene.state != BeSceneState.onFinish)
        {
            Logger.EditorLogWarning("PauseFight");

            RacePuaseFrame pauseFrame = FrameCommandFactory.CreateCommand((UInt32)FrameCommandID.RacePause) as RacePuaseFrame;
            pauseFrame.isPauseLogic = true;
            FrameSync.instance.FireFrameCommand(pauseFrame);

			mBeScene.Pause(pauseAnimation);
        }
    }

	public void ResumeFight(bool pauseAnimation=true, string tag = "", bool force = false)
	{
        if (tag == mPrePauseBeSceneTag || force)
        {
            mPrePauseBeSceneTag = "";

			//if (mBattle != null && mBattle.recordServer!=null && mBattle.recordServer.IsProcessRecord())
            //{
			//	mBattle.recordServer.RecordProcess("[BATTLE]ResumeFight");
            //}

			if (null != mBeScene && (mMode != eDungeonMode.SyncFrame || ReplayServer.GetInstance().IsReplay()) && mBeScene.state == BeSceneState.onPause)
            {
                Logger.EditorLogWarning("ResumeFight");
                RacePuaseFrame pauseFrame = FrameCommandFactory.CreateCommand((UInt32)FrameCommandID.RacePause) as RacePuaseFrame;
                pauseFrame.isPauseLogic = false;
                FrameSync.instance.FireFrameCommand(pauseFrame);
                mBeScene.Resume(pauseAnimation);
            }
        }
	}


    private bool bInFade = false;
    private uint mSum = 0;
    private DungeonFadeCallback mLoadcb = null;
	private DungeonFadeCallback mFadeoutcb = null;
	private uint mFadein;
    private uint mFadeout;
    public bool IsInFade { get { return bInFade; } }
    public void OpenFade(DungeonFadeCallback fadein, DungeonFadeCallback load, DungeonFadeCallback fadeout, uint intime, uint outtime)
    {
        if (null != fadein)
        {
            fadein.Invoke();
            fadein = null;
        }

        bInFade     = true;
        mSum     	= 0;
        mLoadcb  	= load;
        mFadeoutcb  = fadeout;
        mFadein     = intime;
        mFadeout    = outtime;

#if !SERVER_LOGIC 

        GameFrameWork.instance.OpenFadeFrame(
                        () => { }, null,
                        () => { return false; },
                        intime / 1000.0f,
                        outtime / 1000.0f);

 #endif

    }

    private void _updateFadeIter(int delta)
    {
        if (bInFade)
        {
            uint all = mFadein + mFadeout;
            bool bFadeinFinish = false;

            if (mSum < all)
            {
                if (!bFadeinFinish && mSum >= mFadein)
                {
                    bFadeinFinish = true;

                    if (null != mLoadcb)
                    {
                        mLoadcb.Invoke();
                        mLoadcb = null;
                    }
                }
            }
            else
            {
                if (null != mFadeoutcb)
                {
                    mFadeoutcb.Invoke();
                    mFadeoutcb = null;
                }

                bInFade = false;
            }

            mSum += (uint)delta;
        }
    }

    #endregion

    #region Update

    /// <summary>
    /// 如果多个Scene的管理
    /// 
    /// </summary>
    /// <param name="delta"></param>
    public void Update(int delta)
    {
        mDelta = delta;
        if (mBattle != null && mBattle.recordServer != null)
        {
            mBattle.recordServer.BeginUpdate();
        }
        _updateFadeIter(delta);

        if (null != mBeScene)
        {
            mBeScene.Update(delta);
        }

        if (null != mEnumertorManager)
        {
            mEnumertorManager.UpdateEnumerators();
        }
        
        mBattle.FrameUpdate(delta);
        
        if (null != mDungeonData)
        {
            mDungeonData.Update(delta);
        }

        _dungeonTimeStatistics(delta);
        if (mBattle != null && mBattle.recordServer != null)
        {
            mBattle.recordServer.EndUpdate();
        }
    }

    public void UpdateGraphic(int delta)
    {
#if !SERVER_LOGIC 

     if (null != mBeScene)
        {
            mBeScene.UpdateGraphic(delta);
        }

 #endif

    }
    private void _dungeonTimeStatistics(int delta)
    {
        if (null != mBeScene && !mBeScene.IsBossDead())
        {
            DungeonStatistics statist = mDungeonData.CurrentDungeonStatistics();

            if (null != statist)
            {
                switch (mBeScene.state)
                {
                    case BeSceneState.onNone:
                    case BeSceneState.onPause:
                        break;
                    case BeSceneState.onCreate:
                    case BeSceneState.onReady:
                    case BeSceneState.onFight:
                    case BeSceneState.onBulletTime:
                        statist.areaFightTime += delta;

                        if (statist.lastVisitFrame == uint.MaxValue)
                        {
                            statist.lastVisitFrame = FrameSync.instance.curFrame;
                        }

                        break;
                    case BeSceneState.onClear:
                        statist.areaClearTime += delta;

                        
                        break;
                    case BeSceneState.onFinish:
                        break;
                    default:
                        break;
                }
            }
        }
    }
    #endregion

    public UInt64 GetAllHurtDamage()
    {
        UInt64 allDamage = 0;

        List<DungeonStatistics> statist = mDungeonData.AllDungeonStatistics();
        for (int i = 0; i < statist.Count; ++i)
        {
            allDamage += statist[i].GetSumDamage();
        }

        Logger.LogProcessFormat("[战斗] 总伤害 {0}", allDamage);
        return allDamage;
    }

    public uint GetAllMaxHurtDamage()
    {
        DungeonHurtData data = GetAllMaxHurtData();

        if (null == data || data.damage < 0)
        {
            return 0;
        }

        Logger.LogProcessFormat("[战斗] 最大伤害 {0}", data.damage);
        return (uint)data.damage;
    }

    public uint GetAllMaxHurtSkillID()
    {
        DungeonHurtData data = GetAllMaxHurtData();

        if (null == data)
        {
            return 0;
        }

        Logger.LogProcessFormat("[战斗] 最大伤害SKILLID {0}", data.skillId);
        return (uint)data.skillId;
    }

    public uint GetAllMaxHurtID()
    {
        DungeonHurtData data = GetAllMaxHurtData();

        if (null == data)
        {
            return 0;
        }

        Logger.LogProcessFormat("[战斗] 最大伤害HurtID {0}", data.hurtId);

        return (uint)data.hurtId;
    }

    public DungeonHurtData GetAllMaxHurtData()
    {
        List<DungeonStatistics> statist = mDungeonData.AllDungeonStatistics();

        int iMaxDamage                  = -1;

        DungeonHurtData data            = null;
        DungeonHurtData maxData         = null;

        for (int i = 0; i < statist.Count; ++i)
        {
            if (null == statist[i])
            {
                continue;
            }

            data = statist[i].GetMaxHurtData();

            if (iMaxDamage < data.damage)
            {
                iMaxDamage = data.damage;
                maxData = data;
            }
        }

        return maxData;
    }

    public DungeonHurtData GetCurrentMaxHurtData()
    {
        if (null == mBeScene)
        {
            return new DungeonHurtData();
        }

        DungeonStatistics statist = mDungeonData.CurrentDungeonStatistics();
        if (null == statist)
        {
            return new DungeonHurtData();
        }

        return statist.GetMaxHurtData();
    }

    public void AddHurtData(int skillId, int hurtId, int damage)
    {
        if (null == mBeScene)
        {
            return ;
        }

        DungeonStatistics statist = mDungeonData.CurrentDungeonStatistics();
        if (null == statist)
        {
            return ;
        }

        Logger.LogProcessFormat("[战斗] 设置伤害数据 SKID {0}, HID {1}, D {2}",skillId, hurtId, damage);

        statist.AddHurtData(skillId, hurtId, damage);
    }
    public void AddBossHurtData(int pid, int damage,int monsterID)
    {
        mBossDamage += damage;
        AddBossDamage((uint)monsterID, (uint)damage);
        var allPlayers = mDungeonPlayers.GetAllPlayers();
        for (int i = 0; i < allPlayers.Count; ++i)
        {
            if (allPlayers[i] != null && allPlayers[i].playerActor != null && allPlayers[i].playerActor.GetPID() == pid)
            {
                allPlayers[i].statistics.data.BossDamage += damage;
                return;
            }
        }
        if (mBeScene != null)
        {
            var attacker = mBeScene.GetEntityByPID(pid);
            if (attacker != null && attacker.GetEntityData() != null)
            {
                Logger.LogWarningFormat("AddBossHurtData {0} 找不到对应的玩家id {1} {2} {3}",mDungeonData != null ? mDungeonData.id.dungeonID : 0,pid, damage, attacker.GetEntityData().name);
                return;
            }
        }
        Logger.LogWarningFormat("AddBossHurtData {0} 找不到对应的玩家id {1} {2}", mDungeonData != null ? mDungeonData.id.dungeonID : 0, pid, damage);
    }

    private void AddBossDamage(uint monsterID, uint damage)
    {
        if (bossID01 == monsterID)
        {
            bossDamage01 += damage;
        }
        else if (bossID02 == monsterID)
        {
            bossDamage02 += damage;
        }
    }
    public uint GetBossDamage(byte seat)
    {
        var allPlayers = mDungeonPlayers.GetAllPlayers();

        for (int i = 0; i < allPlayers.Count; ++i)
        {
            if (allPlayers[i] != null && allPlayers[i].GetPlayerSeat() == seat)
            {
                return (uint)(allPlayers[i].statistics.data.BossDamage);
            }
        }
        Logger.LogErrorFormat("GetBossDamage 找不到座位的玩家seat {0}", seat);
        return 0u;
    }
    public uint GetTotalBossDamage()
    {
        return (uint)mBossDamage;
    }
    public int AllDeadCount()
    {
        int sum = 0;

        var allPlayers = mDungeonPlayers.GetAllPlayers();

        for(int i = 0; i < allPlayers.Count; ++i)
        {
            sum += allPlayers[i].statistics.data.deadCount;
        }

        return sum;
    }

    public int AllRebornCount()
    {
        var allPlayers = mDungeonPlayers.GetAllPlayers();
        int sum = 0;

        for(int i = 0; i < allPlayers.Count; ++i)
        {
            sum += allPlayers[i].statistics.data.rebornCount;
        }

        return sum;
    }

    public int AllHitCount()
    {
        int sum = 0;
        var allPlayers = mDungeonPlayers.GetAllPlayers();
        for(int i=0; i<allPlayers.Count; ++i)
        {
            sum += allPlayers[i].statistics.data.beHitCount;
        }
        return sum;
    }

    public int RebornCount(byte seat)
    {
		var player = mDungeonPlayers.GetPlayerBySeat(seat);
        return player.statistics.data.rebornCount;
    }

    public int DeadCount(byte seat)
    {
        var player = mDungeonPlayers.GetPlayerBySeat(seat);
        return player.statistics.data.deadCount;
    }

    public int HitCount(byte seat)
    {
		var player = mDungeonPlayers.GetPlayerBySeat(seat);
        return player.statistics.data.beHitCount;
    }

    public int ItemUseCount(byte seat, int id)
    {
        throw new NotImplementedException();
    }

    public int SkillUseCount(byte seat, int id)
    {
        throw new NotImplementedException();
    }

    public int AllFightTime(bool withClear)
    {
        return mDungeonData.AllFightTime(withClear);
    }

    public int CurrentFightTime(bool withClear)
    {
        return mDungeonData.CurrentFightTime(withClear);
    }

    public int LastFightTimeWithCount(bool withClear, int count)
    {
        return mDungeonData.LastFightTimeWithCount(withClear, count);
    }

    private int _score(UnionCell split, int max, int score)
    {
        int i;

        for (i = 1; i <= max; ++i)
        {
            var value = TableManager.GetValueFromUnionCell(split, i);
            if (score >= value)
            {
                return i - 1;
            }
        }

        return max - 1;
    }

    public int RebornCountScore()
    {
        var split = mDungeonData.table.RebornSplitArg;
        var cnt = AllRebornCount();
        return _score(split, split.eValues.everyValues.Count, cnt);
    }

    public int HitCountScore()
    {
        var split = mDungeonData.table.HitSplitArg;
        var cnt = AllHitCount();
        return _score(split, split.eValues.everyValues.Count, cnt);
    }

    public int AllFightTimeScore(bool withClear)
    {
        var split = mDungeonData.table.TimeSplitArg;
        var cnt = AllFightTime(withClear) / 1000;
        return _score(split, split.eValues.everyValues.Count, cnt);
    }

    public int FinalScore()
    {
        return HitCountScore() + AllFightTimeScore(true) + RebornCountScore();
    }

    public DungeonScore FinalDungeonScore()
    {
		if (mDungeonPlayers.IsAllPlayerDead() && !mBeScene.IsBossDead())
			return DungeonScore.C; 

        var score = FinalScore();
		return GetDungeonScoreByValue(score);
    }

	private DungeonScore GetDungeonScoreByValue(int score)
	{
		var realScore = DungeonScore.SSS;
		if (score >= 3) { realScore = DungeonScore.SSS; }
		else if (score >= 2) { realScore = DungeonScore.SS; }
		else if (score >= 1) { realScore = DungeonScore.S; }
		else { realScore = DungeonScore.A; }
		return realScore;
	}

    public int FightTimeSplit(int level)
    {
        var split = mDungeonData.table.TimeSplitArg;
        return TableManager.GetValueFromUnionCell(split, level);
    }

    public void SetMatchPlayerVoteTimeLeft(int leftTime)
    {
        mMatchPKLeftVoteTime = leftTime;
    }

    public int GetMatchPlayerVoteTimeLeft()
    {
        return mMatchPKLeftVoteTime;
    }
}
