using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GameClient;
using Protocol;
#if MG_TEST && LOGIC_SERVER
public class LogicServer : IDisposable
#else
public class LogicServer
#endif
{
    private bool bRun = false;
    private int timeAcc = 0;
    private int timeEndOut = 0;

    private int timeEndOutMS = 20 * 1000;

    private uint curFrame = 0;

    private uint keyFrameRate = 1;
    private uint serverSeed = 0;
    private uint frameSpeed = 1;
    private uint svrFrameLater = 0;
    private bool isGetStartFrame = false;

    private RecordServer record;
    private ulong session;
   

    static public LogicServer sm_Root;
#if MG_TEST && LOGIC_SERVER
    private int dungeonId = -1;
    public double curConsumeTime = 0;
    public double maxConsumeTime = 0;
    public double curInitConsumeTime = 0;
    public double startTimeStamp = 0;
    public int maxUpdateFrame = 0;
#endif
    public uint CurrentFrame
    {
        get { return curFrame; }
    }
    
    public uint GetCurFrame() { return curFrame; }
    public LogicServer()
    {
        sm_Root = this;
        bufferCache = new byte[65535];
        rootLogic = this;
        //Logger.LogError("LogicServer Ctor\n");
        frameQueue = new Queue<GameClient.IFrameCommand>();
        keyFrameRate = 1;
        isGetStartFrame = false;
    }
    public ulong GetSession() { return session; }

    private bool mIsFire = true;
    public bool isFire
    {
        get
        {
            return mIsFire;
        }

        set
        {
            mIsFire = value;
        }
    }

    public RecordServer recordServer
    {
        get { return record; }
    }
    public GameClient.BaseBattle GetBattle() { return battle; }

#if MG_TEST && LOGIC_SERVER
    public void Dispose()
    {
        Dispose(true);
        //  GC.SuppressFinalize(this);

    }
    ~LogicServer()
    {
        Dispose(false);
    }
    private bool m_disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!m_disposed)
        {
            if (disposing)
            {
                // Release managed resources            
            }

            // Release unmanaged resources            

            m_disposed = true;
        }
        // LogicApplication.OnLogicServerDisposed(session);
        Logger.LogErrorFormat("[Dispose LogicServer] session {0} dippose {1} stack {2} ", session.ToString(), disposing, RecordServer.GetStackTraceModelName());
    }
    public int DungeonID
    {
        get
        {

            if (dungeonId == -1 && battle != null &&
                battle.dungeonManager != null &&
                 battle.dungeonManager.GetDungeonDataManager() != null &&
                 battle.dungeonManager.GetDungeonDataManager().id != null)
            {
                dungeonId = battle.dungeonManager.GetDungeonDataManager().id.dungeonID;
            }
            return dungeonId;
        }
    }
#endif

    private GameClient.BaseBattle battle;

    private static uint logicUpdateStep = 32;
    private static uint logicFrameStep = 2;
    private static int logicFrameStepDelta = 0;

    public static LogicServer rootLogic;

    protected Queue<GameClient.IFrameCommand> frameQueue = new Queue<GameClient.IFrameCommand>();
    byte[] bufferCache = new byte[65535];
    void Init()
    {
        bRun = false;
        timeAcc = 0;
        curFrame = 0;
        //keyFrameRate = 1;
        serverSeed = 0;
        frameSpeed = 1;
        svrFrameLater = 0;
        isGetStartFrame = false;

        isFire = true;
        //record;
        //session;

        //record = new RecordServer();
        bufferCache = new byte[65535];
        frameQueue = new Queue<GameClient.IFrameCommand>();
        logicUpdateStep = 32;
        logicFrameStep = 2;
        logicFrameStepDelta = 0;

        bCanForceUpdateEnd = false;
        bIsEnd = false;
    }

    public enum LogicServerLogType
    {
        Debug,
        Info,
        Warning,
        Error,
        Exception,
    }

    public void OnRelayGameStart(UInt32 randomseed)
    {
        timeAcc = 0;
        curFrame = 2;
        //currentEndFrame = 0;
        battle.FrameRandom.callFrame = curFrame;
        serverSeed = randomseed;
        record.RecordProcessPlayerInfo(battle.dungeonPlayerManager);
        if (record.IsReplayRecord() &&
           battle.dungeonManager != null &&
           battle.dungeonManager.GetDungeonDataManager() != null &&
           battle.dungeonManager.GetDungeonDataManager().id != null)
        {
            record.RecordStartTime(randomseed);
            int dungeonID = battle.dungeonManager.GetDungeonDataManager().id.dungeonID;
            record.RecordDungeonID(dungeonID);
            record.RecordSequence(currentEndFrame);
        }
    }
    
    #if LOGIC_SOURCE_DEBUG
    public delegate void OnServerCheckSumDel(ulong session, uint frame, uint callnum);
    public static OnServerCheckSumDel OnServerCheckSum;
    public delegate void LogConsoleDel(LogicServerLogType type, string message);
    public static LogConsoleDel LogConsole;
    public delegate void OnReportRaceEndDel(ulong session, byte[] raceend, int len);
    public static OnReportRaceEndDel OnReportRaceEnd;
    #else
    
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern void OnServerCheckSum(ulong session, uint frame, uint callnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern void LogConsole(LogicServerLogType type, string message);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern void OnReportRaceEnd(ulong session, byte[] raceend, int len);
    #endif

    static byte[] raceEndBuffer = new byte[65536];
    public static void ReportRaceEndToLogicServer(RelaySvrDungeonRaceEndReq raceend)
    {
        for (int i = 0; i < raceend.raceEndInfo.infoes.Length; i++)
        {
            var info = raceend.raceEndInfo.infoes[i];
            for (int j = 0; j < info.md5.Length; j++)
            {
                info.md5[j] = 0;
            }
        }
        string raceEndInfo = string.Empty;
        string commonInfo = string.Format("sessionid : {0} dungeonId : {1} usedTime : {2}", raceend.raceEndInfo.sessionId, raceend.raceEndInfo.dungeonId, raceend.raceEndInfo.usedTime);
        string subDetail = string.Empty;
        for (int i = 0; i < raceend.raceEndInfo.infoes.Length; i++)
        {
            var info = raceend.raceEndInfo.infoes[i];
            subDetail += string.Format("<roleId : {0} pos : {1} score : {2} hitCount : {3} bossDamage : {4} bossId1 : {5} bossRemainHP1 : {6} bossId2 : {7} bossRemainHP2 : {8}>", info.roleId, info.pos, info.score, info.beHitCount, info.bossDamage,info.boss1ID,info.boss1RemainHp,info.boss2ID,info.boss2RemainHp);
        }
        Logger.LogErrorFormat("[ReportRaceEndToLogicServer] {0} {1} ", commonInfo, subDetail);

        for (int i = 0; i < raceend.raceEndInfo.infoes.Length; i++)
        {
            var info = raceend.raceEndInfo.infoes[i];
            for(int j = 0; j < info.md5.Length; j++)
            {
                info.md5[j] = 0;
            }
        }

        int pos = 0;
        raceend.encode(raceEndBuffer, ref pos);
        OnReportRaceEnd(raceend.raceEndInfo.sessionId, raceEndBuffer, pos);
    }
    public static void ReportPkRaceEndToLogicServer(RelaySvrEndGameReq raceEnd)
    {
        var info = raceEnd.end;
        string raceEndInfo = string.Empty;
        string commonInfo = string.Format("sessionid : {0} replayScore : {1}", info.gamesessionId, info.replayScore);
        string subDetail = string.Empty;
        for (int i = 0; i < info.infoes.Length; i++)
        {
            var subInfo = info.infoes[i];
            subDetail += string.Format("<roleId : {0} pos : {1} remainHp : {2} remainMp : {3} result : {4} damagePercent : {5}>", subInfo.roleId, subInfo.pos, subInfo.remainHp, subInfo.remainMp, subInfo.result, subInfo.damagePercent);
        }
        Logger.LogErrorFormat("[ReportPkRaceEndToLogicServer] {0} {1} ", commonInfo, subDetail);
        int pos = 0;
        raceEnd.encode(raceEndBuffer, ref pos);
        OnReportRaceEnd(raceEnd.end.gamesessionId, raceEndBuffer, pos);
    }

    public static LogicServer NewGameLogic()
    {
        var logic = new LogicServer();
        logic.Init();
        rootLogic = logic;
        return logic;
    }

    public static void LogicServerInit()
    {
        try
        {
            TableManager.CreateInstance(false);                     //初始化表格 (热更新调整)
            TableManager.GetInstance().LogicServerInit();
            EquipMasterDataManager.GetInstance().Initialize();
            //ItemDataManager.GetInstance().InitData();
            //OnServerCheckSum(0,1,2);
            //LogConsole("LogicServerInit");
            //System.GC.Collect();
        }
        catch (Exception e)
        {
            Logger.LogError(e.ToString() + "\n");
        }

    }
    static public void DumpMemory()
    {
        string dumpFileName = string.Format("dump-{0}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        //UnityHeapDump.Create(dumpFileName);
    }
    public void StartPVE(ulong s, System.IntPtr buff, int bufflen)
    {
         System.GC.Collect();
#if LOGIC_SERVER && MG_TEST
         var preTimeNow = Utility.GetTimeStamp();
#endif
        CopyBufferCache(buff, bufflen);
        session = s;

        SceneDungeonStartRes rep = new SceneDungeonStartRes();
        rep.decode(bufferCache);

        BattleDataManager.GetInstance().ClearBatlte();
        BattleDataManager.GetInstance().ConvertDungeonBattleInfo(rep);

        _setRacePlayers(rep.players);
        _setRaceRelayServer(rep.session, rep.addr);

        eDungeonMode mode = eDungeonMode.None;
        if (rep.session == 0)
        {
            mode = eDungeonMode.LocalFrame;
        }
        else
        {
            mode = eDungeonMode.SyncFrame;
        }

        session = rep.session;
        BattleType type = ChapterUtility.GetBattleType((int)rep.dungeonId);
        //BattleMain.OpenBattle(type, mode, (int)rep.dungeonId);
        battle = BattleFactory.CreateBattle(type, mode, (int)rep.dungeonId);

        battle.SetBattleFlag(rep.battleFlag);
        record = battle.recordServer;
      
        record.sessionID = rep.session.ToString();
        battle.recordServer.StartRecord(type, mode, rep.session.ToString(), true, false);
        record.SetLogicServer(this);
        battle.SetDungeonClearInfo(rep.clearedDungeonIds);
        var raidBattle = battle as RaidBattle;
        if (raidBattle != null)
        {
        for (int i = 0; i < rep.clearedDungeonIds.Length; i++)
        {
                raidBattle.DungeonDestroyNotify((int)rep.clearedDungeonIds[i]);
            }
        }
        var guildBattle = battle as GuildPVEBattle;
        if (guildBattle != null && rep.guildDungeonInfo != null)
        {
            guildBattle.SetBossInfo(rep.guildDungeonInfo.bossOddBlood, rep.guildDungeonInfo.bossTotalBlood);
            guildBattle.SetBuffInfo(rep.guildDungeonInfo.buffVec);
        }
        else
        {
            var finalTestBattle = battle as FinalTestBattle;
            if (finalTestBattle != null && rep.zjslDungeonInfo != null)
            {
                finalTestBattle.SetBossInfo(rep.zjslDungeonInfo.boss1ID, rep.zjslDungeonInfo.boss1RemainHp, rep.zjslDungeonInfo.boss2ID, rep.zjslDungeonInfo.boss2RemainHp);
                finalTestBattle.SetBuffInfo(rep.zjslDungeonInfo.buffVec);
            }
            else
            {
                var pveBattle = battle as PVEBattle;
                if (pveBattle != null && rep.hellAutoClose == 1)
                {
                    pveBattle.OpenHellClose();
                }
            }
        }
        battle.StartLogicServer(this);
        record.RecordDungeonID((int)rep.dungeonId);
        record.RecordDungoenInfo(rep);
      
#if LOGIC_SERVER && MG_TEST
          var afterTimeNow = Utility.GetTimeStamp();
            curInitConsumeTime = afterTimeNow - preTimeNow;
            startTimeStamp = Utility.GetTimeStamp();
#endif
    }


    public void StartPVE(ulong s, byte[] buff)
    {
         System.GC.Collect();
#if LOGIC_SERVER && MG_TEST
          var preTimeNow = Utility.GetTimeStamp();
#endif
        bufferCache = buff;
        session = s;

        SceneDungeonStartRes rep = new SceneDungeonStartRes();
        rep.decode(bufferCache);

        BattleDataManager.GetInstance().ClearBatlte();
        BattleDataManager.GetInstance().ConvertDungeonBattleInfo(rep);

        _setRacePlayers(rep.players);
        _setRaceRelayServer(rep.session, rep.addr);

        eDungeonMode mode = eDungeonMode.None;

        if (rep.session == 0)
        {
            mode = eDungeonMode.LocalFrame;
        }
        else
        {
            mode = eDungeonMode.SyncFrame;
        }

        session = rep.session;

        BattleType type = ChapterUtility.GetBattleType((int)rep.dungeonId);
        //BattleMain.OpenBattle(type, mode, (int)rep.dungeonId);
        battle = BattleFactory.CreateBattle(type, mode, (int)rep.dungeonId);


        battle.SetBattleFlag(rep.battleFlag);
       

        record = battle.recordServer;
        record.sessionID = rep.session.ToString();
        battle.recordServer.StartRecord(type, mode, rep.session.ToString(), true, false);
        record.SetLogicServer(this);
        battle.SetDungeonClearInfo(rep.clearedDungeonIds);
        var raidBattle = battle as RaidBattle;
        if (raidBattle != null)
        {
        for (int i = 0; i < rep.clearedDungeonIds.Length; i++)
        {
                raidBattle.DungeonDestroyNotify((int)rep.clearedDungeonIds[i]);
            }
        }
        var guildBattle = battle as GuildPVEBattle;
        if (guildBattle != null && rep.guildDungeonInfo != null)
        {
            guildBattle.SetBossInfo(rep.guildDungeonInfo.bossOddBlood, rep.guildDungeonInfo.bossTotalBlood);
            guildBattle.SetBuffInfo(rep.guildDungeonInfo.buffVec);
        }
        else
        {
            var finalTestBattle = battle as FinalTestBattle;
            if (finalTestBattle != null && rep.zjslDungeonInfo != null)
            {
                finalTestBattle.SetBossInfo(rep.zjslDungeonInfo.boss1ID, rep.zjslDungeonInfo.boss1RemainHp, rep.zjslDungeonInfo.boss2ID, rep.zjslDungeonInfo.boss2RemainHp);
                finalTestBattle.SetBuffInfo(rep.zjslDungeonInfo.buffVec);
            }
            else
            {
                var pveBattle = battle as PVEBattle;
                if (pveBattle != null && rep.hellAutoClose == 1)
                {
                    pveBattle.OpenHellClose();
                }
            }
        }
        battle.StartLogicServer(this);

        record.RecordDungeonID((int)rep.dungeonId);
        record.RecordDungoenInfo(rep);
#if LOGIC_SERVER && MG_TEST
        var afterTimeNow = Utility.GetTimeStamp();
        curInitConsumeTime = afterTimeNow - preTimeNow;
        startTimeStamp = Utility.GetTimeStamp();
#endif
    }

    public void StartPVP(ulong s, System.IntPtr buff, int bufflen)
    {
         System.GC.Collect();
#if LOGIC_SERVER && MG_TEST
         var preTimeNow = Utility.GetTimeStamp();
#endif
        Init();
        CopyBufferCache(buff, bufflen);
        session = s;

        WorldNotifyRaceStart rep = new WorldNotifyRaceStart();
        rep.decode(bufferCache);

        //if (rep.raceType == (byte)RaceType.PK || rep.raceType == (byte)RaceType.GuildBattle)
        {
            _setRacePlayers(rep.players);
            _setRaceRelayServer(rep.sessionId, rep.addr);
            /*
           if (rep.raceType == (byte)RaceType.GuildBattle)
            {
                BattleMain.OpenBattle(BattleType.GuildPVP,eDungeonMode.SyncFrame, (int)rep.dungeonId, rep.sessionId.ToString());
            }
            else if(rep.raceType == (byte)RaceType.PremiumLeaguePreliminay || rep.raceType == (byte)RaceType.PremiumLeagueBattle)
            {
                BattleMain.OpenBattle(BattleType.MoneyRewardsPVP,eDungeonMode.LocalFrame, (int)rep.dungeonId, rep.sessionId.ToString());
            }
            else if (rep.raceType == (byte)RaceType.PK_3V3 || rep.raceType == (byte)RaceType.ScoreWar)
            {
                if(rep.raceType == (byte)RaceType.ScoreWar)
                {
                    Pk3v3CrossDataManager.GetInstance().bMatching = false;

                    WorldNotifyRaceStart rep2 = new WorldNotifyRaceStart();
                    rep2 = rep;

                    GameFrameWork.instance.StartCoroutine(_PK3v3CrossMatchOK(rep2));
                    return;
                }


                BattleMain.OpenBattle(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, (int)rep.dungeonId, rep.sessionId.ToString());
            }
            else
            {
                BattleMain.OpenBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, (int)rep.dungeonId,rep.sessionId.ToString());
            }
            */

            if (rep.raceType == (byte)RaceType.GuildBattle)
            {
                //BattleMain.OpenBattle(BattleType.GuildPVP);
                battle = BattleFactory.CreateBattle(BattleType.GuildPVP, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                record = battle.recordServer;
                record.sessionID = rep.sessionId.ToString();
                battle.recordServer.StartRecord(BattleType.MutiPlayer, eDungeonMode.SyncFrame, rep.sessionId.ToString(), true, false);
                record.SetLogicServer(this);
                battle.recordServer.RecordPlayerInfo(rep);
                battle.StartLogicServer(this);
            }
            else if (rep.raceType == (byte)RaceType.PremiumLeaguePreliminay || rep.raceType == (byte)RaceType.PremiumLeagueBattle)
            {
                battle = BattleFactory.CreateBattle(BattleType.MoneyRewardsPVP, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                record = battle.recordServer;
                record.sessionID = rep.sessionId.ToString();
                battle.recordServer.StartRecord(BattleType.MoneyRewardsPVP, eDungeonMode.SyncFrame, rep.sessionId.ToString(), true, false);
                record.SetLogicServer(this);
                battle.recordServer.RecordPlayerInfo(rep);
                battle.StartLogicServer(this);
            }
            else if (rep.raceType == (byte)RaceType.PK_3V3 ||
                     rep.raceType == (byte)RaceType.ScoreWar)
            {
                battle = BattleFactory.CreateBattle(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                record = battle.recordServer;
               
                record.sessionID = rep.sessionId.ToString();
                battle.recordServer.StartRecord(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, rep.sessionId.ToString(), true, false);
                record.SetLogicServer(this);
                battle.recordServer.RecordPlayerInfo(rep);
                battle.StartLogicServer(this);
            }
            else if (rep.raceType == (byte)RaceType.PK_3V3_Melee || rep.raceType == (byte)RaceType.PK_2V2_Melee)
            {
                battle = BattleFactory.CreateBattle(BattleType.ScufflePVP, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                record = battle.recordServer;
                record.sessionID = rep.sessionId.ToString();
                battle.recordServer.StartRecord(BattleType.ScufflePVP, eDungeonMode.SyncFrame, rep.sessionId.ToString(), true, false);
                record.SetLogicServer(this);
                battle.recordServer.RecordPlayerInfo(rep);
                battle.StartLogicServer(this);
            }
            else if (rep.raceType == (byte)RaceType.ChiJi)
            {
                battle = BattleFactory.CreateBattle(BattleType.ChijiPVP, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                record = battle.recordServer;
                
                record.sessionID = rep.sessionId.ToString();
                battle.recordServer.StartRecord(BattleType.ChijiPVP, eDungeonMode.SyncFrame, rep.sessionId.ToString(), true, false);
                record.SetLogicServer(this);
                battle.recordServer.RecordPlayerInfo(rep);
                battle.StartLogicServer(this);
            }
            else
            {
                //BattleMain.OpenBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                battle = BattleFactory.CreateBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                record = battle.recordServer;
                record.sessionID = rep.sessionId.ToString();
                battle.recordServer.StartRecord(BattleType.MutiPlayer, eDungeonMode.SyncFrame, rep.sessionId.ToString(), true, false);
                record.SetLogicServer(this);
                battle.recordServer.RecordPlayerInfo(rep);
                battle.StartLogicServer(this);
            }
            if (battle != null)
            {
                battle.PkRaceType = (int)rep.raceType;
            }
        }
#if LOGIC_SERVER && MG_TEST
            var afterTimeNow = Utility.GetTimeStamp();
            curInitConsumeTime = afterTimeNow - preTimeNow;
            startTimeStamp = Utility.GetTimeStamp();
#endif
        //record = battle.recordServer;
        //record.sessionID = rep.sessionId.ToString();
        //record.recordMode = RecordMode.PVP;
    }

    bool bCanForceUpdateEnd = false;
    bool bIsEnd = false;

    public void StartPVPRecord(System.IntPtr buff, int bufflen)
    {
        try
        {
             System.GC.Collect();
#if LOGIC_SERVER && MG_TEST
          var preTimeNow = Utility.GetTimeStamp();
#endif
            Init();
            //!! 录像才强制为ForceUpdateEnd
            bCanForceUpdateEnd = true;
            CopyBufferCache(buff, bufflen);
            //buff = System.IntPtr.Zero;

            ReplayFile rep = new ReplayFile();
            int pos = 0;
            rep.decode(bufferCache, ref pos);



            if (rep.header.raceType == 0)
            {

                session = rep.header.sessionId;
                //rep.header.raceType 
                //if (rep.header.raceType == (byte)RaceType.PK || rep.header.raceType == (byte)RaceType.GuildBattle)
                //{

                _setRacePlayers(rep.header.players);
                _setRaceRelayServer(rep.header.sessionId, new SockAddr());

                if (rep.header.raceType == (byte)RaceType.GuildBattle)
                {
                    //BattleMain.OpenBattle(BattleType.GuildPVP);
                    battle = BattleFactory.CreateBattle(BattleType.GuildPVP,eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;
                    record.isLogicServerSaveRecordInTheEnd = true;
                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.MutiPlayer, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    record.SetLogicServer(this);
                    battle.StartLogicServer(this);
                }
                else if (rep.header.raceType == (byte)RaceType.PremiumLeaguePreliminay || rep.header.raceType == (byte)RaceType.PremiumLeagueBattle)
                {
                    battle = BattleFactory.CreateBattle(BattleType.MoneyRewardsPVP, eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;
                   
                    record.isLogicServerSaveRecordInTheEnd = true;
                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.MoneyRewardsPVP, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    record.SetLogicServer(this);
                    battle.StartLogicServer(this);
                }
                else if (rep.header.raceType == (byte)RaceType.PK_3V3 ||
                 rep.header.raceType == (byte)RaceType.ScoreWar)
                {
                    battle = BattleFactory.CreateBattle(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;

                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    record.SetLogicServer(this);
                    battle.StartLogicServer(this);
                }
                else if (rep.header.raceType == (byte)RaceType.PK_3V3_Melee || rep.header.raceType == (byte)RaceType.PK_2V2_Melee)
                {
                    battle = BattleFactory.CreateBattle(BattleType.ScufflePVP, eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;
                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.ScufflePVP, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    record.SetLogicServer(this);
                    battle.StartLogicServer(this);
                }
                else if (rep.header.raceType == (byte)RaceType.ChiJi)
                {
                    battle = BattleFactory.CreateBattle(BattleType.ChijiPVP, eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;
                    record.isLogicServerSaveRecordInTheEnd = true;
                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.ChijiPVP, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    record.SetLogicServer(this);
                    battle.StartLogicServer(this);
                }
                else
                {
                    //BattleMain.OpenBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                    battle = BattleFactory.CreateBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;
                    record.isLogicServerSaveRecordInTheEnd = true;
                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.MutiPlayer, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    record.SetLogicServer(this);
                    battle.StartLogicServer(this);
                }
                if(battle != null)
                {
                    battle.PkRaceType = rep.header.raceType;
                }


                //record.recordMode = RecordMode.PVP;
                //frameQueue = new Queue<GameClient.IFrameCommand>();

                //}
            }
            else
            {
                SceneDungeonStartRes r = new SceneDungeonStartRes();
                r.decode(bufferCache, ref pos);

                BattleDataManager.GetInstance().ClearBatlte();
                BattleDataManager.GetInstance().ConvertDungeonBattleInfo(r);

                

                eDungeonMode mode = eDungeonMode.None;
                if (r.session == 0)
                {
                    mode = eDungeonMode.LocalFrame;
                }
                else
                {
                    mode = eDungeonMode.SyncFrame;
                }

                session = r.session;

                BattleType type = ChapterUtility.GetBattleType((int)r.dungeonId);
                _setRacePlayers(r.players);
                _setRaceRelayServer(r.session, r.addr);
                battle = BattleFactory.CreateBattle(type, mode, (int)r.dungeonId);
                record = battle.recordServer;
                record.isLogicServerSaveRecordInTheEnd = true;
                record.sessionID = r.session.ToString();
                battle.SetBattleFlag(r.battleFlag);
              
                record = battle.recordServer;
               
                record.sessionID = r.session.ToString();
                battle.recordServer.StartRecord(type, mode, r.session.ToString(), true, false);
                record.SetLogicServer(this);
                battle.SetDungeonClearInfo(r.clearedDungeonIds);
                var raidBattle = battle as RaidBattle;
                if (raidBattle != null)
                {
                for (int i = 0; i < r.clearedDungeonIds.Length; i++)
                {
                        raidBattle.DungeonDestroyNotify((int)r.clearedDungeonIds[i]);
                    }
                }
                var guildBattle = battle as GuildPVEBattle;
                if (guildBattle != null && r.guildDungeonInfo != null)
                {
                    guildBattle.SetBossInfo(r.guildDungeonInfo.bossOddBlood,r.guildDungeonInfo.bossTotalBlood);
                    guildBattle.SetBuffInfo(r.guildDungeonInfo.buffVec);
                }
                else
                {
                    var finalTestBattle = battle as FinalTestBattle;
                    if (finalTestBattle != null && r.zjslDungeonInfo != null)
                    {
                        finalTestBattle.SetBossInfo(r.zjslDungeonInfo.boss1ID, r.zjslDungeonInfo.boss1RemainHp, r.zjslDungeonInfo.boss2ID, r.zjslDungeonInfo.boss2RemainHp);
                        finalTestBattle.SetBuffInfo(r.zjslDungeonInfo.buffVec);
                    }
                    else
                    {
                        var pveBattle = battle as PVEBattle;
                        if (pveBattle != null && r.hellAutoClose == 1)
                        {
                            pveBattle.OpenHellClose();
                        }
                    }
                }
                battle.StartLogicServer(this);
                record.RecordDungeonID((int)r.dungeonId);
                record.RecordDungoenInfo(r);
            }

            _PushFrameCommand(rep.frames);

            if (rep.header.raceType == 0)
            {
                Logger.LogErrorFormat(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Start PVP Mode!!");
            }
            else
            {
                Logger.LogErrorFormat(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Start PVE Mode!!");
            }
#if LOGIC_SERVER && MG_TEST
            var afterTimeNow = Utility.GetTimeStamp();
            curInitConsumeTime = afterTimeNow - preTimeNow;
            startTimeStamp = Utility.GetTimeStamp();
#endif
        }
        catch (Exception e)
        {
            Logger.LogError(e.ToString() + "\n");
        }
    }

    public void StartPVPRecord(byte[] buff)
    {
        try
        {
#if LOGIC_SERVER && MG_TEST
          var preTimeNow = Utility.GetTimeStamp();
#endif
            //     System.GC.Collect();
            Init();
            //!! 录像才强制为ForceUpdateEnd
            bufferCache = buff;
            bCanForceUpdateEnd = true;

            ReplayFile rep = new ReplayFile();
            int pos = 0;
            rep.decode(bufferCache, ref pos);



            if (rep.header.raceType == 0)
            {

                session = rep.header.sessionId;
                //rep.header.raceType 
                //if (rep.header.raceType == (byte)RaceType.PK || rep.header.raceType == (byte)RaceType.GuildBattle)
                //{

                _setRacePlayers(rep.header.players);
                _setRaceRelayServer(rep.header.sessionId, new SockAddr());

                if (rep.header.raceType == (byte)RaceType.GuildBattle)
                {
                    //BattleMain.OpenBattle(BattleType.GuildPVP);
                    battle = BattleFactory.CreateBattle(BattleType.GuildPVP, eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;
                    record.isLogicServerSaveRecordInTheEnd = true;
                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.MutiPlayer, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    battle.StartLogicServer(this);
                }
                else if (rep.header.raceType == (byte)RaceType.ChiJi)
                {
                    battle = BattleFactory.CreateBattle(BattleType.ChijiPVP, eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;
                    record.isLogicServerSaveRecordInTheEnd = true;
                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.ChijiPVP, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    battle.StartLogicServer(this);
                }
                else
                {
                    //BattleMain.OpenBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, (int)rep.dungeonId);
                    battle = BattleFactory.CreateBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, (int)rep.header.pkDungeonId);
                    record = battle.recordServer;
                    record.isLogicServerSaveRecordInTheEnd = true;
                    record.sessionID = rep.header.sessionId.ToString();
                    battle.recordServer.StartRecord(BattleType.MutiPlayer, eDungeonMode.SyncFrame, rep.header.sessionId.ToString(), true, false);
                    battle.StartLogicServer(this);
                }


                //record.recordMode = RecordMode.PVP;
                //frameQueue = new Queue<GameClient.IFrameCommand>();

                //}
            }
            else
            {
                SceneDungeonStartRes r = new SceneDungeonStartRes();
                r.decode(bufferCache, ref pos);

                BattleDataManager.GetInstance().ClearBatlte();
                BattleDataManager.GetInstance().ConvertDungeonBattleInfo(r);

                _setRacePlayers(r.players);
                _setRaceRelayServer(r.session, r.addr);

                eDungeonMode mode = eDungeonMode.None;
                if (r.session == 0)
                {
                    mode = eDungeonMode.LocalFrame;
                }
                else
                {
                    mode = eDungeonMode.SyncFrame;
                }

                session = r.session;

                BattleType type = ChapterUtility.GetBattleType((int)r.dungeonId);
                //BattleMain.OpenBattle(type, mode, (int)rep.dungeonId);
                battle = BattleFactory.CreateBattle(type, mode, (int)r.dungeonId);
                record = battle.recordServer;
                record.isLogicServerSaveRecordInTheEnd = true;
                record.sessionID = r.session.ToString();
                battle.recordServer.StartRecord(type, mode, r.session.ToString(), true, false);
                battle.StartLogicServer(this);
            }

            _PushFrameCommand(rep.frames);

            if (rep.header.raceType == 0)
            {
                Logger.LogErrorFormat(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Start PVP Mode!!");
            }
            else
            {
                Logger.LogErrorFormat(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Start PVE Mode!!");
            }
#if LOGIC_SERVER && MG_TEST
           var afterTimeNow = Utility.GetTimeStamp();
            curInitConsumeTime = afterTimeNow - preTimeNow;
             startTimeStamp = Utility.GetTimeStamp();
#endif
        }
        catch (Exception e)
        {
            Logger.LogError(e.ToString() + "\n");
        }
    }


    public void Update(int delta)
    {


        try
        {

//             if (isEnd())
//             {
//                 EndBattle();
//                 return;
//             }
//             battle.dungeonManager.FinishFight();
// 
//             return;
            //if(!bRun)
            //{
            //    return;
            //}
            if (isEnd())
            {
                EndBattle();
                return;
            }

            if (currentEndFrame < logicFrameStep)
            {
                return;
            }

            int tick = (int)logicUpdateStep;

            timeAcc = delta;

            if (null != battle && null != battle.recordServer)
            {
                //battle.recordServer.RecordProcess("curFrame {0} Update {1} endFrame {2}", curFrame, delta, currentEndFrame);
            }

            while (/*!isEnd() && */timeAcc >= tick)
            {
                //!!这里是逐步压帧的方式，录像直接设置为更新到最后，验证服务器是收到结束帧后
                if (bCanForceUpdateEnd == false)
                {
                    // 1110 > (1111 - 2 + 1)
                    // 1110 > 1110
                    if (curFrame > ((currentEndFrame) - logicFrameStep + 1))
                    {
                        break;
                    }
                }


                timeAcc -= tick;

                curFrame += logicFrameStep;

                if (null != battle && null != battle.recordServer)
                {
                    //battle.recordServer.RecordProcess("curFrame {0} tick {1} endFrame {2}", curFrame, tick, currentEndFrame);
                }
                
                UpdateSendChecksum();
                //战斗结束了就直接退出执行帧命令了 
                if (!bIsEnd)
                {
                    UpdateReplayFrame(tick);
                }
                else
                {
                    break;
                }
            }

            if (frameQueue.Count <= 0 && bCanForceUpdateEnd)
            {
                //battle.dungeonManager.FinishFight();
                timeEndOut += delta;

                if (timeEndOut > timeEndOutMS)
                {
                    battle.dungeonManager.FinishFight();
                }
            }

            //Logger.LogErrorFormat("time acc current is {0}",timeAcc);
        }
        catch (Exception e)
        {
            try
            {
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("LogicServer Save Error Session {0} Begin", session));
                BeUtility.SaveBattleRecord(GetBattle());
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("LogicServer Save Error Session {0} End", session));
            }
            catch (Exception e2)
            {
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("LogicServer Save Session {0} Failed reason {1}", session, e2.ToString()));
            }
            finally
            {
                Logger.LogError(e.ToString() + " session:" + session.ToString() + "\n");
            }
            
        }
    }

    public bool isEnd()
    {
        if (battle == null)
        {
            return true;
        }

        if (battle.dungeonManager == null)
        {
            return true;
        }
        bool bEnd = battle.dungeonManager.IsFinishFight();

        if (bEnd)
        {
            EndBattle();
        }

        return bEnd;
    }

    public void PushFrameCommand(System.IntPtr buff, int bufflen)
    {

        CopyBufferCache(buff, bufflen);

        RelaySvrFrameDataNotify ntf = new RelaySvrFrameDataNotify();
        ntf.decode(bufferCache);
        _PushFrameCommand(ntf.frames);

        _recordRelayFileFrame(ntf);
    }

    List<Frame> mCacheFrame = new List<Frame>();

    private void _recordRelayFileFrame(RelaySvrFrameDataNotify ntf)
    {
        if (null == ntf)
        {
            return ;
        }

        if (null == ntf.frames)
        {
            return ;
        }

        if (null == record)
        {
            return ;
        }

        if (!record.IsReplayRecord())
        {
            return ;
        }
      
        mCacheFrame.Clear();

        for (int i = 0; i < ntf.frames.Length; ++i)
        {
            if (null != ntf.frames[i].data && ntf.frames[i].data.Length > 0)
            {
                mCacheFrame.Add(ntf.frames[i]);
            }
        }

        if (mCacheFrame.Count > 0)
        {
            ntf.frames = mCacheFrame.ToArray();
            record.RecordServerFrames(ntf.frames);
        }

        mCacheFrame.Clear();
    }

    protected void _PushFrameCommand(Frame[] frames)
    {
        if (record != null && record.IsProcessRecord())
        {
            for (int i = 0; i < frames.Length; ++i)
            {
                var frame = frames[i];
                for (int j = 0; j < frame.data.Length; ++j)
                {
                    var cur = frame.data[j];
                    byte seat = cur.seat;
                    var data = cur.input;
                    IFrameCommand frameCmd = FrameCommandFactory.CreateCommand(data.data1);


                    if (frameCmd != null)
                    {
                        FrameCommandID id = (FrameCommandID)data.data1;

                        if (id == FrameCommandID.NetQuality)
                            continue;

                        frameCmd.SetValue(frame.sequence, seat, data);
                        //record.RecordProcess2("receive cmd {0}", frameCmd.GetString());
                    }
                }
            }
        }

        {
            _pushNetCommand(frames);
        }
    }

    private void _setRacePlayers(RacePlayerInfo[] players)
    {
        BattleDataManager.GetInstance().PlayerInfo = players;
        ClientApplication.racePlayerInfo = players;
        for (int i = 0; i < ClientApplication.racePlayerInfo.Length; ++i)
        {
            var current = ClientApplication.racePlayerInfo[i];
            if (current.accid == ClientApplication.playerinfo.accid)
            {
                ClientApplication.playerinfo.seat = current.seat;
            }
        }
    }

    private void _setRaceRelayServer(ulong session, SockAddr addr)
    {
        ClientApplication.playerinfo.session = session;
        ClientApplication.relayServer.ip = addr.ip;
        ClientApplication.relayServer.port = addr.port;
    }

    uint currentEndFrame;
    public uint GetSvrFrame()
    {
        return currentEndFrame;
    }
    void SetSvrFrame(uint svrNum)
    {
        svrNum = svrNum >> 1;
        svrNum = svrNum << 1;

        currentEndFrame = svrNum;

        //if (record != null && record.IsProcessRecord())
        //{
        //    record.RecordProcess("[CMD]update current End Frame ---------------- :{0}", currentEndFrame);
        //}
    }

    void CopyBufferCache(System.IntPtr buff, int bufflen)
    {
        /*if(bufferCache == null || bufferCache.Length < bufflen)
        {
            bufferCache = new byte[bufflen];
        } */

        /*if(bufferCache == null)
        {
            Console.Write("!!!! bufferCache == null keyFrameRate " +keyFrameRate);
        }*/

        Logger.LogProcessFormat("!!!!keyFrameRate " + keyFrameRate);
        bufferCache = new byte[bufflen];
        Marshal.Copy(buff, bufferCache, 0, bufflen);
    }


    private void _pushNetCommand(Frame[] frames)
    {
        for (int i = 0; i < frames.Length; ++i)
        {
            var frame = frames[i];
            SetSvrFrame(frame.sequence);
            for (int j = 0; j < frame.data.Length; ++j)
            {
                var cur = frame.data[j];
                byte seat = cur.seat;
                var data = cur.input;

                IFrameCommand frameCmd = FrameCommandFactory.CreateCommand(data.data1);
                if (frameCmd == null)
                {
                    Logger.LogErrorFormat("Seat{0} Data Id {1}FrameCommand is Null!! \n", seat, data.data1);
                }
                else
                {
                    Logger.LogFormat("{0}Recive Cmd {1} \n", System.DateTime.Now.ToLongTimeString(), frameCmd.GetString());

                    frameCmd.SetValue(frame.sequence, seat, data);

                    BaseFrameCommand baseframeCmd = frameCmd as BaseFrameCommand;

                    if (baseframeCmd != null)
                    {
                        baseframeCmd.sendTime = data.sendTime;
                    }

                    FrameCommandID frameCmdID = (FrameCommandID)frameCmd.GetID();

                    Logger.LogProcessFormat("[帧同步] 收到 数据类型 {0}", frameCmdID);

                    if (!isGetStartFrame)
                    {
                        if (frameCmdID == FrameCommandID.GameStart)
                        {
                            isGetStartFrame = true;
                            ClearCmdQueue();
                        }
                    }
                    //else
                    //{
                    //    if (frameCmdID == FrameCommandID.RaceEnd)
                    //    {
                    //        ClientReconnectManager.instance.canReconnectRelay = false;
                    //    }
                    //}

                    if (frameCmdID == FrameCommandID.RaceEnd)
                    {
                        bCanForceUpdateEnd = true;
                    }

                    frameQueue.Enqueue(frameCmd);
//                     if ( record != null && record.IsProcessRecord())
//                     {
//                         Logger.LogErrorFormat("[SessionID]: {0} [CMD]PUSH CMD:{1}\n", record.sessionID, frameCmd.GetString());
//                     }
//                     else
//                     {
//                         Logger.LogErrorFormat("[CMD]PUSH CMD:{0}\n", frameCmd.GetString());
//                     }
                    if (record != null && record.IsProcessRecord())
                    {
                        record.RecordProcess2("T[{0}][CMD]PUSH CMD:{1} FrameSequence:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), frameCmd.GetString(), frame.sequence);
                        //record.RecordProcess( "[CMD]PUSH CMD:{0}", frameCmd.GetString());
                    }
                }
            }
        }
    }

    private void ClearCmdQueue()
    {
        while (frameQueue.Count > 0)
        {
            frameQueue.Dequeue();
        }
    }

    private void UpdateReplayFrame(int delta)
    {
#if LOGIC_SERVER && MG_TEST
       var preTimeNow = Utility.GetTimeStamp();
#endif
        while (frameQueue.Count > 0)
        {
            GameClient.IFrameCommand command = frameQueue.Peek();
            uint curframe = (command.GetFrame() + this.svrFrameLater) * keyFrameRate;
            if (curframe > this.curFrame)
            {
                break;
            }

            //Logger.LogForReplay("{0} Frame:{1} ExecCommand {2} \n", System.DateTime.Now.ToLongTimeString(), this.curFrame, command.GetString());
            GameClient.IFrameCommand cmd = frameQueue.Dequeue();
            if (null != cmd)
            {
                var cmdbase = cmd as GameClient.BaseFrameCommand;
                cmdbase.battle = battle;

                //Console.Write(string.Format("Frame{2} Exec Commond {0} {1} \n",cmd.GetType().Name,cmd.ToString(),curFrame));
                if (null != battle && null != battle.recordServer)
                {
                    battle.recordServer.RecordProcess("[CMD]ExecCommand:{0}", cmd.GetString());
                    battle.recordServer.MarkString(0x8779807, cmd.GetString());
                    // Mark:0x8779807 ExecCommand cmd:{0}
                }
                //                 if (null != battle && null != battle.recordServer)
                //                 {
                //                     Logger.LogErrorFormat("SessionId:[{0}] {1} Frame:{2} ExecCommand {3} \n", battle.recordServer.sessionID, System.DateTime.Now.ToLongTimeString(), this.curFrame, command.GetString());
                //                 }
                //                 else
                //                 {
                //                     Logger.LogErrorFormat("{0} Frame:{1} ExecCommand {2} \n", System.DateTime.Now.ToLongTimeString(), this.curFrame, command.GetString());
                //                 }

                cmd.ExecCommand();
                //FrameCommandPool.instance.RecycleFrameCommand(cmd);
            }

            if (frameQueue.Count <= 0)
            {
                timeEndOut = 0;
            }

        }

        if (null != battle.dungeonManager)
        {
            battle.dungeonManager.Update(delta);
        }

#if LOGIC_SERVER && MG_TEST
            var afterTimeNow = Utility.GetTimeStamp();
            curConsumeTime = afterTimeNow - preTimeNow;
            if(curConsumeTime > maxConsumeTime)
            {
                maxConsumeTime = curConsumeTime;
                maxUpdateFrame = (int)curFrame;
            }
#endif
    }


    void SendChecksum()
    {
        //!!游戏结束就不上报随机数了
        if (isEnd())
        {
            return;
        }
#if !LOGIC_SERVER_TEST
        OnServerCheckSum(session, curFrame, battle.FrameRandom.callNum);
#endif
    }

    void UpdateSendChecksum()
    {
        battle.FrameRandom.callFrame = curFrame;
        if (curFrame > 0 && curFrame % 50 == 0)
        {
            SendChecksum();
        }
    }

    void EndBattle()
    {
        try
        {
            if (bIsEnd == false)
            {
                bIsEnd = true;
                battle.End();
            }
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat(e.ToString() + "\n");
        }

    }
    public void GiveUpVerify()
    {
        if(isEnd() == false)
        {
            battle.dungeonManager.FinishFight();
        }
        EndBattle();
    }

    public void UnsyncVerify()
    {
        
    }

    public void SaveRecord()
    {
        record.LogicServerSaveRecordAndReplay();
    }

    public bool HaveFrameInQueue()
    {
        return frameQueue.Count > 0;
    }
    
        
    public static void UnsyncTest(ulong s)
    {
        Logger.LogErrorFormat("start unsync test with {0}",s.ToString(), RecordData.GetRootPath());
        tm.tool.UnsyncChecker ck = new tm.tool.UnsyncChecker();
        ck.PushUnsync(s.ToString(), RecordData.GetRootPath(), s.ToString());
        ck.UpdateUnsycnNode(10000);
        ck.UpdateUnsycnNode(10000);
    }

}
