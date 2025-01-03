using System;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using Network;
using Protocol;
using System.Collections;
using UnityEngine.Events;
using System.IO;
using System.Text;
public class FramePerformance
{
    public uint totalDelay = 0;
    public uint averageDelay = 0;
    public uint maxDelay = 0;
    public uint cmdNum = 0;
    public uint recentDelay = 0;

    protected uint[] recentDelays = new uint[10];
    protected int recentPos = 0;


    public void AddDelay(UInt32 delay)
    {
        cmdNum++;
        totalDelay += delay;
        maxDelay = delay > maxDelay ? delay : maxDelay;
        averageDelay = totalDelay / cmdNum;
        recentDelays[recentPos++] = delay;
        if (recentPos >= recentDelays.Length)
        {
            recentPos = 0;
        }
        recentDelay = GetRecentDelay();
    }

    private uint GetRecentDelay()
    {
        uint total = 0;
        uint num = 0;
        for (int i = 0; i < recentDelays.Length; i++)
        {
            if (recentDelays[i] != 0)
            {
                total += recentDelays[i];
            }
            num++;
        }

        return num > 0 ? total / num : 0;
    }
}
public class FrameSync : GameBindSystem
{
    private static FrameSync s_instance;

    public enum eFrameSyncState
    {
        onCreate,
        onStart,
        onTick,
        onReconnect,
        onEnd,
    }


    private eDungeonMode mMode = eDungeonMode.LocalFrame;

    private eFrameSyncState mState = eFrameSyncState.onCreate;

    public static FrameSync instance
    {
        get
        {
            if (FrameSync.s_instance == null)
            {
                FrameSync.s_instance = new FrameSync();
            }
            return FrameSync.s_instance;
        }
    }

    private IDungeonManager mMainLogic;
    public void SetMainLogic(IDungeonManager logic)
    {
        mMainLogic = logic;
    }

    public void ClearMainLogic()
    {
        mMainLogic = null;
        mLastTimeDifference = 0.0f;
        _reset();
    }

    public string GetDebugString()
    {
        return "\n" + "curFrame:" + curFrame + " endFrame:" + endFrame + " curFrameDelay:" + curFrameDelay + " curframeNeedUpdate:" + curframeNeedUpdate;
    }
    protected Queue<IFrameCommand> frameQueue = new Queue<IFrameCommand>();

    public uint curFrame;
    public uint endFrame;
    public uint frameMs;
    public uint svrFrame;
    public uint svrFrameMs;
    public int avgFrameDelay = 66;
    public int curFrameDelay;
    public int driftFactor;
    public float timeStart;
    public uint backupSvrFrame;
    public uint keyFrameRate = 1;
    public uint serverSeed = 0;
    public uint frameSpeed = 1;
    public uint svrFrameLater = 0;
    public bool logicLoaded = false;
    public uint lastSvrFrame = 1;

    public FramePerformance recvCmdPerf = new FramePerformance();
    public FramePerformance execCmdPerf = new FramePerformance();
    private FrameCountPerformance mChasingModeMoniter = new FrameCountPerformance(4, 1, 10);

    private bool mIsGetStartFrame = false;
    public bool isGetStartFrame
    {
        get
        {
            return mIsGetStartFrame;
        }

        private set
        {
            mIsGetStartFrame = value;
        }
    }

    public bool bInRunMode;
    public int nDegree;
    public bool bInMoveMode;

    public float fLocalUpdateAcc = 0;
    public static uint GetTicksNow()
    {
        if (FrameSync.instance != null)
        {
            return FrameSync.instance.curFrame * FrameSync.instance.svrFrameMs;
        }
        return 0;
    }
    public string GetChasingOutputStr()
    {
        return BeUtility.Format("{0} chasing {1}", mChasingModeMoniter.OutputString(), mIsInChasingMode);
    }

#if MG_TEST_EXTENT && !LOGIC_SERVER
    private FileStream udpLogFile = null;
    private StreamWriter udpLogWriter = null;
    private StringBuilder contentRecorder = null;
#endif
    public bool isOpenChasingMode()
    {
        if (BattleMain.IsModePvP(BattleMain.battleType) || BattleMain.instance == null) return false;
        var battle = BattleMain.instance.GetBattle() as BaseBattle;
        if (battle == null) return false;
        return GeGraphicSetting.instance.IsLowLevel()
            && battle.FunctionIsOpen(BattleFlagType.GamePursuit)
            && BeClientSwitch.FunctionIsOpen(ClientSwitchType.OpenChasingMode);
    }
    private bool mIsInChasingMode = false;
    public bool IsInChasingMode { get { return mIsInChasingMode; } }
    public void StartInChasingMode()
    {
        mIsInChasingMode = true;
#if !LOGIC_SERVER
        string outputDebugStr = string.Empty;
#if MG_TEST
            if(mChasingModeMoniter != null)
                outputDebugStr = mChasingModeMoniter.OutputString();
#endif
        if (BattleMain.IsModePvP(BattleMain.battleType) || BattleMain.instance == null) return;
        RecordServer.GetInstance().RecordChasingStartFrame(curFrame, outputDebugStr);
#endif

    }
    public void EndChasingMode()
    {
        mIsInChasingMode = false;
        if (mMainLogic != null)
        {
            mMainLogic.DoGraphicBackToFront();
        }
#if !LOGIC_SERVER
        if (RecordServer.GetInstance() != null)
        {
            string outputDebugStr = string.Empty;
#if MG_TEST
            if(mChasingModeMoniter != null)
                outputDebugStr = mChasingModeMoniter.OutputString();
#endif
            if (BattleMain.IsModePvP(BattleMain.battleType) || BattleMain.instance == null) return;
            RecordServer.GetInstance().RecordChasingEndFrame(curFrame, outputDebugStr);
        }
#endif
    }
    public void SetChasingMoniterParam(int addCount, int maxCount, int maxTime, int maxCall)
    {
        mChasingModeMoniter.ReInit(addCount, maxCount, maxTime, maxCall);
    }
    private void _reset()
    {
        curFrame = 0;
        endFrame = 0;
        frameMs = 0;
        svrFrame = 0;
        svrFrameMs = 0;
        avgFrameDelay = 66;
        curFrameDelay = 0;
        driftFactor = 0;
        timeStart = 0.0f;
        backupSvrFrame = 0;
        keyFrameRate = 1;
        serverSeed = 0;
        frameSpeed = 1;
        svrFrameLater = 0;
        logicLoaded = false;
        frameQueue.Clear();
        isGetStartFrame = false;
        fLocalUpdateAcc = 0;
        lastSvrFrame = 1;
#if !LOGIC_SERVER && MG_TEST
        if (RecordServer.GetInstance() != null)
            RecordServer.GetInstance().PushReconnectCmd(string.Format("FrameSync._reset {0}",svrFrame));
#endif
#if !LOGIC_SERVER
        if (IsInChasingMode)
        {
            EndChasingMode();
        }
#endif
    }

    public void ResetMove()
    {
        bInRunMode = false;
        nDegree = 0;

        if (bInMoveMode)
        {
            if (InputManager.instance != null)
                InputManager.instance.FireStopCommand();
        }

        bInMoveMode = false;

        //frameQueue.Clear();
    }

    public void Init()
    {
        _reset();

        mState = eFrameSyncState.onStart;
        InitBindSystem(null);
#if !LOGIC_SERVER && MG_TEST
        if (RecordServer.GetInstance() != null)
            RecordServer.GetInstance().PushReconnectCmd(string.Format("FrameSync.Init() {0} {1} {2} {3}", svrFrame, lastSvrFrame,frameQueue.Count,curFrame));
#endif
        frameQueue.Clear();

#if MG_TEST_EXTENT && !LOGIC_SERVER
        try
        {
            if (contentRecorder != null)
            {
                if(udpLogWriter != null)
                {    
                    udpLogWriter.Write(contentRecorder.ToString());
                    udpLogWriter.Flush();    
                }
                contentRecorder.Clear();
            }
            contentRecorder = StringBuilderCache.Acquire(1024 * 1024);
           
            if (udpLogWriter != null)
            {
                udpLogWriter.Close();
            }
            if (udpLogFile != null)
            {
                udpLogFile.Dispose();
                udpLogFile.Close();
            }
        }
        catch(Exception e)
        {
            Logger.LogErrorFormat("Try rewrite log Error reason {0}",e.Message);
        }

        try
        {
            string filePath = string.Format("{0}/udp_track_{1}.log", UnityEngine.Application.persistentDataPath, DateTime.Now.ToString("yyyy-MM-dd"));
            if (File.Exists(filePath))
            {
                udpLogFile = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                udpLogWriter = new StreamWriter(udpLogFile);
            }
            else
            {
                udpLogFile = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                udpLogWriter = new StreamWriter(udpLogFile);
            }
        }
        catch(Exception e)
        {
             Logger.LogErrorFormat("Try CreateFile log Error reason {0}",e.Message);
        }
#endif
    }

    public void SetDungeonMode(eDungeonMode mode)
    {
        mMode = mode;

        if (eDungeonMode.SyncFrame != mMode)
        {
            ClientReconnectManager.instance.canReconnectRelay = false;
        }

        Logger.LogProcessFormat("[帧同步] 设置同步模式 {0}", mMode);
    }


    public void SetStartTick()
    {
        mState = eFrameSyncState.onTick;

        if (eDungeonMode.SyncFrame == mMode)
        {
            ClientReconnectManager.instance.canReconnectRelay = true;
        }
        else
        {
            isGetStartFrame = true;
            GameStartFrame cmd = new GameStartFrame
            {
                startTime = (UInt32)(Time.time * GlobalLogic.VALUE_1000)
            };
            FireFrameCommand(cmd);
        }

        fLocalUpdateAcc = 0;
        Logger.LogProcessFormat("[帧同步] 开始接受帧同步消息 frameQueue.Count {0}", frameQueue.Count);
    }

    public void UnInit()
    {
        isFire = true;

        mState = eFrameSyncState.onCreate;
        SetDungeonMode(eDungeonMode.LocalFrame);
        ExistBindSystem();
        if (mLastTimeScaler != 1)
        {
            //  UnityEngine.Time.timeScale = 1.0f;
            mLastTimeScaler = 1;
        }
        _reset();
#if MG_TEST_EXTENT && !LOGIC_SERVER
        try
        {
            if (udpLogWriter != null)
            {
                udpLogWriter.Write(contentRecorder.ToString());
                udpLogWriter.Flush();
                udpLogWriter.Close();
                udpLogWriter = null;
            }

            if (udpLogFile != null)
            {
                udpLogFile.Close();        
                udpLogFile.Dispose();
                udpLogFile = null;
            }

            if (contentRecorder != null)
            {
                contentRecorder.Clear();
                contentRecorder = null;
            }
        }
        catch(Exception e)
        {
            Logger.LogErrorFormat("Try writeFile log Error reason {0}",e.Message);
        }
#endif
    }

    public static uint logicUpdateStep = 32;
    //public static uint logicFrameStep = 1;
    public static int logicFrameStepDelta = 0;

    public void PushUDPCommand(string str)
    {
#if MG_TEST_EXTENT && !LOGIC_SERVER
        if (contentRecorder != null)
        {
            contentRecorder.Append(string.Format("[{0}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")));
            contentRecorder.Append(str);
            contentRecorder.Append("\n");
        }
#endif
    }
    public void StartFrameSync(uint serverSeed, bool initSvr = true)
    {
        Logger.LogProcessFormat("[帧同步] 开始帧同步 随机数种子 {0}, frameQueue.Count {1}", serverSeed, frameQueue.Count);
#if !LOGIC_SERVER && MG_TEST
        if (RecordServer.GetInstance() != null)
            RecordServer.GetInstance().PushReconnectCmd(string.Format("StartFrameSynnc {0} {1} {2} {3}", initSvr, svrFrame, lastSvrFrame,curFrame));
#endif
        if (svrFrame != 0)
        {
            lastSvrFrame = svrFrame;
        }
        else
        {
            if (initSvr)
            {
                lastSvrFrame = 1;
            }
        }
        curFrame = 0;
        if (initSvr)
        {
            //endFrame = 0;
            //svrFrame = 1;
            mLastTimeDifference = 0.0f;
        }
        frameMs = logicUpdateStep;
        svrFrameMs = 16;
        keyFrameRate = 1;
        avgFrameDelay = 0;
        curFrameDelay = 0;
        driftFactor = 2;
        frameSpeed = 1;
        fLocalUpdateAcc = 0;
        mLastTimeScaler = 1;
        //frameQueue.Clear();
        //FrameRandom.ResetSeed(serverSeed);
        this.serverSeed = serverSeed;

#if !SERVER_LOGIC

        timeStart = Time.realtimeSinceStartup;
        logicLoaded = true;
        //sendCount   = 0;
        Application.runInBackground = true;

#endif


        //mState = eFrameSyncState.onTick;
        ResetMove();
    }

    public void StartSingleFrame()
    {
        //mState = eFrameSyncState.onTick;
        ResetMove();
        fLocalUpdateAcc = 0;
    }
   
    public void StopFrameSync()
    {
        //sendCount = 0;
    }
   
    public void ResetBattle()
    {
        ResetMove();
    }
    float mLastTimeDifference = 0.0f;
    public void UpdateLiveShowFrame(bool isInPersuitMode, ref bool needPersue)
    {
        if (isGetStartFrame &&
            mState != eFrameSyncState.onTick &&
            mState != eFrameSyncState.onReconnect)
        {
            return;
        }

        int curSec = (int)Time.realtimeSinceStartup;
        if (preSec != curSec)
        {
            preSec = curSec;
        }

        driftFactor = 2;
        float drift = 2.0f;
        int framesNeedUpdate = (int)((endFrame / ((int)Global.Settings.logicFrameStep) - curFrame / (int)Global.Settings.logicFrameStep) / drift);
        if (isInPersuitMode)
        {

            uint needScaler = 1;
            if (framesNeedUpdate < 16)
            {
                needPersue = false;
                if (mLastTimeDifference != 0.0f && timeStart != mLastTimeDifference)
                {
                    timeStart = mLastTimeDifference;
                }
                if (needScaler != mLastTimeScaler)
                {
                    mLastTimeScaler = needScaler;
                    frameSpeed = mLastTimeScaler;
                }

            }
            else
            {
                needPersue = true;
                needScaler = 16;
                if (needScaler != mLastTimeScaler)
                {
                    mLastTimeScaler = needScaler;
                    frameSpeed = mLastTimeScaler;
                }
            }

        }
        framesNeedUpdate = Mathf.Clamp(framesNeedUpdate, cFrameMin / (int)Global.Settings.logicFrameStep, 100);
        curframeNeedUpdate = framesNeedUpdate;

        int curFrameNeedUpate = framesNeedUpdate;

        long curClientTimeMs = (long)(((Time.realtimeSinceStartup - timeStart) * (float)GlobalLogic.VALUE_1000) * frameSpeed);
        long delayMs = curClientTimeMs - ((svrFrame / Global.Settings.logicFrameStep * Global.Settings.logicFrameStep + logicFrameStepDelta) * svrFrameMs);
        int jitterDelay = CalculateJitterDelay(delayMs / frameSpeed);
        while (curFrameNeedUpate > 0)
        {
            long curFrameMs = curFrame * svrFrameMs;
            long aheadTimeMs = curClientTimeMs - curFrameMs / frameSpeed;
            aheadTimeMs -= jitterDelay;
            UInt32 nowTime = (UInt32)(System.Environment.TickCount);
            if (aheadTimeMs >= frameMs)
            {
                if (curFrame >= (endFrame - Global.Settings.logicFrameStep + 1))
                {
                    curFrameNeedUpate = 0;
                    continue;
                }
                curFrame += Global.Settings.logicFrameStep;

                UpdateSendChecksum();

                //this.LogicFrameTick += this.FrameDelta;
                while (frameQueue.Count > 0)
                {
                    IFrameCommand command = frameQueue.Peek();
                    uint curframe = (command.GetFrame() + this.svrFrameLater) * keyFrameRate;
                    if (curframe > this.curFrame)
                    {
                        break;
                    }

                    BaseFrameCommand frameCmd = command as BaseFrameCommand;
                    if (frameCmd != null && frameCmd.seat == ClientApplication.playerinfo.seat && frameCmd.sendTime > 0)
                    {
                        execCmdPerf.AddDelay(nowTime - frameCmd.sendTime);
                    }

                    Logger.LogFormat("{0} Frame:{1} ExecCommand {2} \n", System.DateTime.Now.ToLongTimeString(), this.curFrame, command.GetString());

#if !LOGIC_SERVER
                    if (RecordServer.GetInstance().IsProcessRecord())
                    {
                        RecordServer.GetInstance().RecordProcess("[CMD]ExecCommand:{0}", command.GetString());
                        RecordServer.GetInstance().MarkString(0x8779714, command.GetString());
                        // Mark:0x8779714 [CMD]ExecCommand:{0}
                    }
#endif

                    IFrameCommand cmd = frameQueue.Dequeue();
                    if (null != cmd)
                    {

                        Logger.LogProcessFormat("[帧同步] 执行 数据类型 {0}", (FrameCommandID)cmd.GetID());

                        var cmdbase = cmd as BaseFrameCommand;
                        cmdbase.battle = BattleMain.instance.GetBattle() as BaseBattle;
                        cmd.ExecCommand();
                        //FrameCommandPool.instance.RecycleFrameCommand(cmd);
                    }
                }

                if (null != mMainLogic)
                {
                    mMainLogic.Update((int)frameMs);
                }

                curFrameNeedUpate--;
                continue;
            }
            curFrameNeedUpate = 0;
        }

        mMainLogic.UpdateGraphic((int)(Time.deltaTime * GlobalLogic.VALUE_1000));
    }
    public void SetSvrFrame(uint svrNum)
    {
#if !LOGIC_SERVER && MG_TEST
        if (svrFrame >= svrNum)
        {
            if (RecordServer.GetInstance() != null)
                RecordServer.GetInstance().PushReconnectCmd(string.Format("SetSvrFrame larger than current {0} --- {1}", svrFrame, svrNum));
        }
#endif
        if (svrFrame != endFrame || svrFrame != lastSvrFrame)
        {
#if !LOGIC_SERVER && MG_TEST

            if (RecordServer.GetInstance() != null)
                RecordServer.GetInstance().PushReconnectCmd(string.Format("SetSvrFrame Begin frame not equal {0} --- {1} --- {2} -- {3}", lastSvrFrame, svrFrame, endFrame, svrNum));
#endif
        }

        lastSvrFrame = svrNum;
        svrFrame = svrNum;
        endFrame = (svrNum + svrFrameLater) * keyFrameRate;
        if(svrFrame != endFrame || svrFrame != lastSvrFrame)
        {
#if !LOGIC_SERVER && MG_TEST

            if (RecordServer.GetInstance() != null)
                RecordServer.GetInstance().PushReconnectCmd(string.Format("SetSvrFrame End frame not equal {0} --- {1} --- {2} -- {3}", lastSvrFrame, svrFrame, endFrame, svrNum));
#endif
        }
        //Logger.LogErrorFormat("svrFrame:{0} endFrame:{1}", svrFrame, endFrame);

        if (backupSvrFrame != svrFrame)
        {
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            ulong svrRealMs = (svrFrame) * this.svrFrameMs;
            float timeDiffrent = realtimeSinceStartup - (svrRealMs * 0.001f);
            float offset = timeDiffrent - this.timeStart;
            if (offset < 0f)
            {
                if (!ReplayServer.GetInstance().IsReplay())
                    this.timeStart = timeDiffrent;
                 mLastTimeDifference = timeDiffrent;
            }
            backupSvrFrame = svrFrame;
        }
    }
    public int CalculateJitterDelay(long nDelayMs)
    {
        this.curFrameDelay = (nDelayMs <= 0L) ? 0 : ((int)nDelayMs);
        if (this.avgFrameDelay < 0)
        {
            this.avgFrameDelay = this.curFrameDelay;
        }
        else
        {
            this.avgFrameDelay = ((29 * this.avgFrameDelay) + this.curFrameDelay) / 30;
        }

        return this.avgFrameDelay;
    }

    public static int sendCount;
    public static int preSec = 0;

    public bool isFire = true;


    public void FireFrameCommand(IFrameCommand cmd, bool force=false)
    {
        if (!isFire && !force)
        {
            return;
        }

        switch (mMode)
        {
#if LOCAL_RECORD_FILE_REPLAY
			case eDungeonMode.RecordFrame:
				
				break;
#endif
            case eDungeonMode.Test:
            case eDungeonMode.LocalFrame:
                //Logger.LogErrorFormat("t:{0}  Push Local FrameCommand {1} \n", Time.realtimeSinceStartup, cmd.GetString());
                frameQueue.Enqueue(cmd);

                BaseFrameCommand baseFrame = cmd as BaseFrameCommand;
                if (null != baseFrame)
                {
                    baseFrame.sendTime = (UInt32)System.Environment.TickCount;
                    if (BattleMain.battleType == BattleType.MutiPlayer || BattleMain.IsTeamMode(BattleMain.battleType, BattleMain.mode))
                    {
                        baseFrame.seat = ClientApplication.playerinfo.seat;
                    }
                    else
                    {
                        baseFrame.seat = 0;
                    }
                }
                break;
            case eDungeonMode.SyncFrame:
                if (mState != eFrameSyncState.onTick)
                {
                    return;
                }

                if (ReplayServer.GetInstance().IsReplay())
                {
                    return;
                }

                var cmddata = cmd.GetInputData();
                cmddata.sendTime = (UInt32)System.Environment.TickCount;
                RelaySvrPlayerInputReq req = new RelaySvrPlayerInputReq
                {
                    session = ClientApplication.playerinfo.session,
                    seat = ClientApplication.playerinfo.seat,
                    roleid = PlayerBaseData.GetInstance().RoleID,
                    input = cmddata
                };
                int iRet = NetManager.Instance().SendCommand(ServerType.RELAY_SERVER, req);

                //sendCount++;

                if (iRet != 0)
                {
                    Logger.LogWarning("InputData SendCommand Error!\n");
                }
                break;
            default:
                Logger.LogError("invalid dungeon mode");
                break;
        }
    }

    void SendChecksum()
    {
#if MG_TEST_EXTENT && !LOGIC_SERVER
        if (contentRecorder != null)
        {
            contentRecorder.Append(string.Format("[{0}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")));
            contentRecorder.Append(string.Format("check sum {0} frame {1}", BattleMain.instance.FrameRandom.callNum, curFrame));
            contentRecorder.Append("\n");
        }
#endif
        RelaySvrFrameChecksumRequest req = new RelaySvrFrameChecksumRequest
        {
            checksum = BattleMain.instance.FrameRandom.callNum,
            frame = curFrame
        };

        int iRet = NetManager.Instance().SendCommand(ServerType.RELAY_SERVER, req);

        if (iRet != 0)
        {
#if MG_TEST_EXTENT && !LOGIC_SERVER
            PushUDPCommand(string.Format("SendChecksum Error! {0}", iRet));
#endif
            Logger.LogWarning("SendChecksum Error!\n");
        }
        else
        {
            //Logger.LogWarningFormat("SendChecksum frame:{0}, checksum:{1}", req.frame, req.checksum);
        }
    }

    void UpdateSendChecksum()
    {
        if (ReplayServer.GetInstance().IsReplay())
        {
            PushUDPCommand("IsInReplay");
            return;
        }
        //Logger.LogErrorFormat("UpdateSendChecksum curFrame:{0}", curFrame);
        if (curFrame > 0 && curFrame % 50 == 0)
        {
            SendChecksum();
        }
    }

    //[MessageHandle(RelaySvrNotifyGameStart.MsgID)]
    //void OnRelaySvrNotifyGameStart(MsgDATA msg)
    //{
    //	if (BattleMain.IsModeMultiplayer())
    //	{
    //		RelaySvrNotifyGameStart ret = new RelaySvrNotifyGameStart();
    //		ret.decode(msg.bytes);
    //		OnRelaySvrNotifyGameStart(ret);
    //		BattleMain.instance.GetDungeonManager().StartFight();
    //	}
    //}

    //[MessageHandle(RelaySvrNotifyGameStart.MsgID)]
    public void OnRelayGameStart(uint startTime)
    {
        StartFrameSync(startTime, !ReplayServer.GetInstance().IsLiveShow());
        curFrame = 2;
#if !LOGIC_SERVER
        if (RecordServer.GetInstance().IsReplayRecord())
        {
            RecordServer.GetInstance().RecordStartTime(startTime);
            int dungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            RecordServer.GetInstance().RecordDungeonID(dungeonID);
            RecordServer.GetInstance().RecordSequence(lastSvrFrame);
        }
#endif

        BattlePlayer battlePlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        if (battlePlayer != null)
        {
            var actor = battlePlayer.playerActor;
            bool isAttackReplaceLigui = false;
            if (actor != null && actor.GetEntityData() != null && actor.GetEntityData().professtion == 11)
                isAttackReplaceLigui = SettingManager.GetInstance().GetLiGuiValue(SettingManager.STR_LIGUI);

            string key = SettingManager.STR_CHASER_PVE;
            if(BattleMain.instance != null && BattleMain.instance.GetBattle() != null)
                key = BattleMain.IsModePvP(BattleMain.instance.GetBattle().GetBattleType()) ? SettingManager.STR_CHASER_PVP: SettingManager.STR_CHASER_PVE;
            byte chaserSetting = (byte) SettingManager.GetInstance().GetChaserSetting(key);
                
            DoublePressConfigCommand cmd = new DoublePressConfigCommand
            {
                hasDoublePress = Global.Settings.hasDoubleRun,
                hasRunAttackConfig = SettingManager.GetInstance().GetRunAttackMode() == InputManager.RunAttackMode.QTE,
                attackReplaceLigui = isAttackReplaceLigui,
                backHitConfig = SettingManager.GetInstance().GetValue(SettingManager.STR_BACKHIT),
                autoHitConfig = SettingManager.GetInstance().GetValue(SettingManager.STR_AUTOHIT),
                canUseCrystalSkill = BeUtility.CheckVipAutoUseCrystalSkill(),
                paladinAttackCharge = SettingManager.GetInstance().GetPaladinAttack() == InputManager.PaladinAttack.OPEN,
                chaserSwitch = chaserSetting
            };
            FrameSync.instance.FireFrameCommand(cmd);
        }

        Logger.LogProcessFormat("[帧同步] 在{0} 开始帧同步 ", mMode);

#if !LOGIC_SERVER
        if (RecordServer.GetInstance().IsProcessRecord())
        {
            RecordServer.GetInstance().RecordProcessPlayerInfo(BattleMain.instance.GetPlayerManager());
        }
#endif
    }

    public void OnRelaySvrNotifyGameStart(RelaySvrNotifyGameStart ret)
    {
        OnRelayGameStart((uint)ret.startTime);
    }

    public void ClearCmdQueue()
    {
        frameQueue.Clear();
    }

    float timepre = 0;
    private void _pushNetCommand(Frame[] frames)
    {
        UInt32 nowTime = (UInt32)System.Environment.TickCount;
        float delta = Time.realtimeSinceStartup * (float)(GlobalLogic.VALUE_1000);
        timepre = delta - timepre;
        //Logger.LogError("RelaySvrFrameDataNotify Frame: " + timepre + " \n");
        timepre = delta;

        for (int i = 0; i < frames.Length; ++i)
        {
            var frame = frames[i];
            uint preLastFrame = lastSvrFrame;
            if (lastSvrFrame > frame.sequence)
            {
                continue;
            }
            SetSvrFrame(frame.sequence);
            for (int j = 0; j < frame.data.Length; ++j)
            {
                var cur = frame.data[j];
                byte seat = cur.seat;
                var data = cur.input;

                if (seat == ClientApplication.playerinfo.seat && data.sendTime > 0)
                {
                    uint delay = nowTime - data.sendTime;
                    recvCmdPerf.AddDelay(delay);
                }

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
#if !LOGIC_SERVER && MG_TEST

                        if (RecordServer.GetInstance() != null)
                            RecordServer.GetInstance().PushReconnectCmd(string.Format("ClearCmdQueue {0} {1} {2} {3}", svrFrame, lastSvrFrame, frameQueue.Count,curFrame));

#endif
                            ClearCmdQueue();
                        }
                    }
                    else
                    {
                        if (frameCmdID == FrameCommandID.RaceEnd)
                        {
                            ClientReconnectManager.instance.canReconnectRelay = false;
                            if(baseframeCmd != null && Global.Settings.logicFrameStep != 0)
                            {
                                uint mode = baseframeCmd.frame % Global.Settings.logicFrameStep;
                                if (mode > 0)
                                {
                                    endFrame += mode;
                                }
                            }
                        }
                    }
                    if (frameQueue.Count > 0)
                    {
                        var cmd = frameQueue.Peek();
                        if (cmd != null)
                        {
                            if (cmd.GetFrame() > frameCmd.GetFrame())
                            {
#if !LOGIC_SERVER && MG_TEST

                                if (RecordServer.GetInstance() != null)
                                    RecordServer.GetInstance().PushReconnectCmd(string.Format("_pushNetCommand {0} {1} {2} {3}", preLastFrame, lastSvrFrame, frameQueue.Count, curFrame));

#endif

                            }
                        }
                    }
                    frameQueue.Enqueue(frameCmd);

#if !LOGIC_SERVER
                    if (RecordServer.GetInstance().IsProcessRecord())
                    {
                        RecordServer.GetInstance().RecordProcess2("T[{0}][CMD]PUSH CMD:{1} FrameSequence:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), frameCmd.GetString(), frame.sequence);
                    }
#endif
                }
            }
        }
    }

    [ProtocolHandle(typeof(RelaySvrReconnectFrameData))]
    private void _onRelaySvrReconnectFrameData(RelaySvrReconnectFrameData rep)
    {
        if (mState == eFrameSyncState.onTick || mState == eFrameSyncState.onReconnect)
        {
         
            if (rep.finish == 0)
            {
                mState = eFrameSyncState.onReconnect;
            }
            else
            {
                mState = eFrameSyncState.onTick;
                Logger.LogProcessFormat("recv all reconnect frame data.");
            }

            if (rep.frames.Length > 0)
            {
#if !LOGIC_SERVER
                if (RecordServer.GetInstance().IsReplayRecord())
                {
                    bool hasOPFrame = false;
                    for (int i = 0; i < rep.frames.Length; ++i)
                    {
                        var frame = rep.frames[i];
                        for (int j = 0; j < frame.data.Length; ++j)
                        {
                            var cur = frame.data[j];
                            var data = cur.input;
                            FrameCommandID id = (FrameCommandID)data.data1;
                            if (id != FrameCommandID.NetQuality)
                                hasOPFrame = true;
                        }
                    }

                    if (hasOPFrame)
                        RecordServer.GetInstance().RecordServerFrames(rep.frames);
                }
#endif
                _pushNetCommand(rep.frames);
            }
#if !LOGIC_SERVER && MG_TEST
            if (RecordServer.GetInstance() != null)
                RecordServer.GetInstance().PushReconnectCmd(string.Format("_onRelaySvrReconnectFrameData {0} - {1} - {2} - {3} - {4}", svrFrame, endFrame, lastSvrFrame, curFrame, mState));
#endif
        }
        else
        {
            Logger.LogErrorFormat("reconnect with {0}", mState);
        }
    }

    [MessageHandle(RelaySvrFrameDataNotify.MsgID)]
    void OnRelaySvrFrameDataNotify(MsgDATA msg)
    {
        RelaySvrFrameDataNotify ntf = new RelaySvrFrameDataNotify();
        ntf.decode(msg.bytes);


#if !LOGIC_SERVER
        //bool hasOPFrame = false;
        if (RecordServer.GetInstance().IsProcessRecord())
        {
            for (int i = 0; i < ntf.frames.Length; ++i)
            {
                var frame = ntf.frames[i];
                for (int j = 0; j < frame.data.Length; ++j)
                {
                    var cur = frame.data[j];
                    byte seat = cur.seat;
                    var data = cur.input;
                    IFrameCommand frameCmd = FrameCommandFactory.CreateCommand(data.data1);


                    if (frameCmd != null)
                    {
                        FrameCommandID id = (FrameCommandID)data.data1;

                        /*if (id != FrameCommandID.NetQuality)
							hasOPFrame = true;
						*/
                        frameCmd.SetValue(frame.sequence, seat, data);

                        //RecordServer.GetInstance().RecordProcess2("receive cmd {0}", frameCmd.GetString());
                    }
                }
            }
        }

        if (RecordServer.GetInstance().IsReplayRecord())
        {
            bool hasOPFrame = false;
            for (int i = 0; i < ntf.frames.Length; ++i)
            {
                var frame = ntf.frames[i];
                for (int j = 0; j < frame.data.Length; ++j)
                {
                    var cur = frame.data[j];
                    var data = cur.input;
                    FrameCommandID id = (FrameCommandID)data.data1;
                    if (id != FrameCommandID.NetQuality)
                        hasOPFrame = true;
                }
            }

           if (hasOPFrame)
            RecordServer.GetInstance().RecordServerFrames(ntf.frames);
        }
#endif

        {
            _pushNetCommand(ntf.frames);
        }
        //else
        //{
        //Logger.LogErrorFormat("tick with {0}", mState);
        //}
    }

    public void PushNetCommand(Frame[] frames)
    {
        //if (mState == eFrameSyncState.onTick)
        {
            _pushNetCommand(frames);
        }
        //else
        //{
        //Logger.LogErrorFormat("tick with {0}", mState);
        //}
    }

    List<_inputData> inputdatas = new List<_inputData>();

    protected void UpdateLocalFrameRandNum()
    {
        UInt32 callNum = 0;

        if (BattleMain.instance == null)
            return;
        

        BaseBattle basebattle = BattleMain.instance.GetBattle() as BaseBattle;

        if (null != basebattle)
        {
            callNum = basebattle.FrameRandom.callNum;
        }

        if (null != mMainLogic)
        {
            mMainLogic.GetDungeonDataManager().PushLocalFrameRandNum(curFrame, callNum);
        }
    }

    protected void UpdateLocalFrameCmd()
    {
        while (frameQueue.Count > 0)
        {
            IFrameCommand cmd = frameQueue.Dequeue();
            Logger.LogFormat("{0}  Local ExecCommand {1} \n", System.DateTime.Now.ToLongTimeString(), cmd.GetString());
            // Logger.LogFormat("{0}  Local ExecCommand {1} \n", System.DateTime.Now.ToLongTimeString(), cmd.GetString());
            var cmdbase = cmd as BaseFrameCommand;
            cmdbase.battle = BattleMain.instance.GetBattle() as BaseBattle;

            if (null != cmdbase)
            {
                cmdbase.frame = curFrame;
                if(cmdbase.frame == 0)
                {
                    Logger.LogErrorFormat("ExecCommand:{0}",cmd.GetString());
                }
            }

            if(cmd.GetSendTime() > 0)
            {
                uint delay = (UInt32)System.Environment.TickCount - cmd.GetSendTime();
                execCmdPerf.AddDelay(delay);
            }
            
            if (null != mMainLogic)
            {
                mMainLogic.GetDungeonDataManager().PushLocalFrameData(cmd);
#if !SERVER_LOGIC
                if (RecordServer.GetInstance().IsReplayRecord())
                {
                    inputdatas.Add(cmd.GetInputData());
                }

                //      RecordServer.GetInstance().RecordProcess2("[CMD]PUSH CMD:{0}", cmd.GetString());
                if (RecordServer.GetInstance().IsProcessRecord())
                {
                    RecordServer.GetInstance().RecordProcess2("T[{0}][CMD]PUSH CMD:{1} FrameSequence:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), cmd.GetString(), cmd.GetFrame());
                    RecordServer.GetInstance().RecordProcess("[CMD]ExecCommand:{0}", cmd.GetString());
                    RecordServer.GetInstance().MarkString(0x8779805, cmd.GetString());
                    // Mark:0x8779805 ExecCommand:{0}
                }
#endif
            }

            cmd.ExecCommand();
            //FrameCommandPool.instance.RecycleFrameCommand(cmd);
        }

#if !SERVER_LOGIC
        if (RecordServer.GetInstance().IsReplayRecord())
        {
            if (inputdatas.Count > 0)
            {
                RelaySvrFrameDataNotify ntf = new RelaySvrFrameDataNotify
                {
                    frames = new Frame[1]
                };
                ntf.frames[0] = new Frame
                {
                    sequence = curFrame,
                    data = new _fighterInput[inputdatas.Count]
                };
                for (int k = 0; k < inputdatas.Count; ++k)
                {
                    ntf.frames[0].data[k] = new _fighterInput
                    {
                        seat = 0,
                        input = inputdatas[k]
                    };
                }
                inputdatas.Clear();
                RecordServer.GetInstance().RecordServerFrames(ntf.frames);
            }
        }
#endif
    }
    protected void UpdateLocalFrame()
    {
        //long delta = IntMath.Float2Int(Time.deltaTime,GlobalLogic.VALUE_1000); //(long)(Time.deltaTime * (float)GlobalLogic.VALUE_1000);
        //delta = Mathf.Clamp((int)delta, 1, GlobalLogic.VALUE_100);
        float delta = Time.deltaTime;
        delta = Mathf.Clamp(delta,0,100f);
        fLocalUpdateAcc += delta;

        float frameRate = logicUpdateStep / 1000f;

        UInt32 nowTime = (UInt32)(Time.time * GlobalLogic.VALUE_1000);

        //Logger.LogErrorFormat("UpdateLocalFrame {0} {1} {2}",Time.deltaTime,delta,nLocalUpdateAcc);
        while (fLocalUpdateAcc >= frameRate)
        {
            curFrame += Global.Settings.logicFrameStep;

#if LOCAL_RECORD_FILE_REPLAY
            _updatePushRecordFrame();
#endif

            UpdateLocalFrameRandNum();
            UpdateLocalFrameCmd();
            if (null != mMainLogic)
            {
                mMainLogic.Update((int)logicUpdateStep);
            }
            fLocalUpdateAcc -= frameRate;
        }
        if (mMainLogic != null)
            mMainLogic.UpdateGraphic((int)(Time.deltaTime * GlobalLogic.VALUE_1000));
    }

    const int cFrameMin = 2;

    public void UpdateReplayFrameGraphic(int delta)
    {
        if (null != mMainLogic)
        {
            mMainLogic.UpdateGraphic(delta);
        }
    }

    public void UpdateReplayFrame(int delta)
    {
        while (frameQueue.Count > 0)
        {
            IFrameCommand command = frameQueue.Peek();
            uint curframe = (command.GetFrame() + this.svrFrameLater) * keyFrameRate;
            if (curframe > this.curFrame)
            {
                break;
            }

            //Logger.LogForReplay("{0} Frame:{1} ExecCommand {2} \n", System.DateTime.Now.ToLongTimeString(), this.curFrame, command.GetString());
            IFrameCommand cmd = frameQueue.Dequeue();
            if (null != cmd)
            {
                var cmdbase = cmd as BaseFrameCommand;
                cmdbase.battle = BattleMain.instance.GetBattle() as BaseBattle;
                cmd.ExecCommand();
                //FrameCommandPool.instance.RecycleFrameCommand(cmd);
            }

        }

        if (null != mMainLogic)
        {
            mMainLogic.Update(delta);
        }
    }

    static int curframeNeedUpdate = 0;
    private uint mLastTimeScaler = 1;

    private bool _IsRunOutOfFrame()
    {
        if (endFrame == 0)
        {
            return true;
        }

        return curFrame >= (endFrame - Global.Settings.logicFrameStep + 1);
    }

    protected void UpdateSyncFrame()
    {
        if (isGetStartFrame &&
            mState != eFrameSyncState.onTick &&
            mState != eFrameSyncState.onReconnect)
        {
            return;
        }

        int curSec = (int)Time.realtimeSinceStartup;
        if (preSec != curSec)
        {
            //sendCount = 0;
            preSec = curSec;
        }

        driftFactor = 2;
        // Global.Settings.drift = 3
        int framesNeedUpdate = (int)((endFrame - curFrame) / Global.Settings.drift);

        // 这里最多只执行 8=(默认) 帧数据 by wdd
        framesNeedUpdate = Mathf.Clamp(framesNeedUpdate, 1, Global.Settings.frameSyncMaxUpdateCount);

        //framesNeedUpdate /= (int)logicFrameStep;
        curframeNeedUpdate = framesNeedUpdate;

        int curFrameNeedUpate = framesNeedUpdate;

        // frameSpeed = 1
        // timeStart为游戏开始的时间
        // 可能会因为观看直播而计算出新的额时间
        // curClientTimeMs 就是 客户端经过的时间
        long curClientTimeMs = (long)(((Time.realtimeSinceStartup - timeStart) * (float)GlobalLogic.VALUE_1000) * frameSpeed);

        // svrFrame 就是客户端收到的服务器最后的帧标号
        // delayMs 就是客户端比服务器迟的时间
        long delayMs = curClientTimeMs - ((svrFrame / Global.Settings.logicFrameStep * Global.Settings.logicFrameStep + logicFrameStepDelta) * svrFrameMs);

        // 根据以往的延迟时间，计算出来的平均延迟时间
        int jitterDelay = CalculateJitterDelay(delayMs);
        if (isOpenChasingMode())
        {
            //优化追帧时卡顿
            mChasingModeMoniter.Update((int)(Time.deltaTime * 1000.0f));
            mChasingModeMoniter.RefreshCurCount(curframeNeedUpdate);
            if (mChasingModeMoniter.IsFit())
            {
                if (!this.IsInChasingMode)
                {
                    StartInChasingMode();
                }
            }
            else if (this.IsInChasingMode)
            {
                EndChasingMode();
            }
        }
        while (curFrameNeedUpate > 0)
        {
            long curFrameMs = curFrame * svrFrameMs;
            long aheadTimeMs = curClientTimeMs - curFrameMs;
            aheadTimeMs -= jitterDelay;
            UInt32 nowTime = (UInt32)System.Environment.TickCount;

            if (aheadTimeMs >= frameMs)
            {
                if (_IsRunOutOfFrame())
                {
                    //this.EndBlockWaitNum++;
                    curFrameNeedUpate = 0;
                    continue;
                }
                //this.EndBlockWaitNum = 0;
                curFrame += Global.Settings.logicFrameStep;

                UpdateSendChecksum();

                //this.LogicFrameTick += this.FrameDelta;
                while (frameQueue.Count > 0)
                {
                    IFrameCommand command = frameQueue.Peek();
                    uint curframe = (command.GetFrame() + this.svrFrameLater) * keyFrameRate;
                    if (curframe > this.curFrame)
                    {
                        break;
                    }

                    BaseFrameCommand frameCmd = command as BaseFrameCommand;
                    if (frameCmd != null && frameCmd.seat == ClientApplication.playerinfo.seat && frameCmd.sendTime > 0)
                    {
                        execCmdPerf.AddDelay(nowTime - frameCmd.sendTime);
                    }

                    Logger.LogFormat("{0} Frame:{1} ExecCommand {2} \n", System.DateTime.Now.ToLongTimeString(), this.curFrame, command.GetString());
#if !LOGIC_SERVER
                    if (RecordServer.GetInstance().IsProcessRecord())
                    {
                        RecordServer.GetInstance().RecordProcess("[CMD]ExecCommand:{0}", command.GetString());
                        RecordServer.GetInstance().MarkString(0x8779806, command.GetString());
                        // Mark:0x8779806 ExecCommand cmd:{0}
                    }
#endif

                    IFrameCommand cmd = frameQueue.Dequeue();
                    if (null != cmd)
                    {

                        Logger.LogProcessFormat("[帧同步] 执行 数据类型 {0}", (FrameCommandID)cmd.GetID());

                        var cmdbase = cmd as BaseFrameCommand;
                        cmdbase.battle = BattleMain.instance.GetBattle() as BaseBattle;
                        cmd.ExecCommand();
                        //FrameCommandPool.instance.RecycleFrameCommand(cmd);
                    }
                }

                if (null != mMainLogic)
                {
                    mMainLogic.Update((int)frameMs);
                }

                curFrameNeedUpate--;
                continue;
            }
            curFrameNeedUpate = 0;
        }

        if (null != mMainLogic)
        {
            mMainLogic.UpdateGraphic((int)(Time.deltaTime * GlobalLogic.VALUE_1000));
        }
    }

#if LOCAL_RECORD_FILE_REPLAY
    private ReplayFile mRelayFile = null;
	private int index = 0;
	private List<Frame> mCacheFrame = new List<Frame> ();
	public void SetRelayFile(ReplayFile file)
	{
        mRelayFile = file;
		index = 0;
	}

	private void _updatePushRecordFrame()
	{
        if (mMode != eDungeonMode.RecordFrame)
        {
            //return ;
        }

        if (null == mRelayFile)
        {
            return ;
        }

        for (int i = index; i < mRelayFile.frames.Length; ++i) 
        {
            if (mRelayFile.frames[i].sequence <= curFrame)
            {
                mCacheFrame.Add(mRelayFile.frames[i]);
                index = i + 1;
            }
        }

        if (mCacheFrame.Count > 0)
        {
            _pushNetCommand(mCacheFrame.ToArray());
        }

        mCacheFrame.Clear();

        if (index >= mRelayFile.frames.Length)
        {
        #if !LOGIC_SERVER
            RecordServer.GetInstance().EndRecord("_updatePushRecordFrame");
        #endif
			mRelayFile = null;
        }
	}
#endif

    public void UpdateFrame()
    {
        //Logger.LogProcessFormat("[帧同步] 更新 {0}", mMode);

        switch (mMode)
        {
            case eDungeonMode.LocalFrame:
            case eDungeonMode.Test:
#if LOCAL_RECORD_FILE_REPLAY
            case eDungeonMode.RecordFrame:
#endif
                UpdateLocalFrame();
                break;
            case eDungeonMode.SyncFrame:
                if (!ReplayServer.GetInstance().IsReplay())
                    UpdateSyncFrame();
                break;
        }
    }
}
