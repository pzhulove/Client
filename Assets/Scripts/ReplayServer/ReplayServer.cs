using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using GameClient;
using System.IO;

public enum ReplayErrorCode
{
	SUCCEED = 0,
	FILE_NOT_FOUND = 1,		//找不到文件
	VERSION_NOT_MATCH = 2,	//版本不匹配
	DOWNLOAD_FAILED = 3,	//下载失败
    HAS_TEAM = 4,//组队状态
    COMPRESS_ERROR = 5,//压缩错误
}

public enum ReplayPlayFrom
{
    VIDEO_FRAME = 0,
    MONEY_REWARD, //赏金联赛
}

public class ReplayServer : Singleton<ReplayServer>
{
	public RecordData recordData;

	private bool isReplay;

	private int timeAcc = 0;
	private bool started = false;
	private bool lastPlaying = false;
	private bool pause = false;
	private PVPBattle battle = null;
	private SimpleTimer pkTimer = null;
    public ReplayPlayFrom replaySource = ReplayPlayFrom.VIDEO_FRAME;
    private bool mIsLiveShow = false;
    private SockAddr mLiveShowServerAddr = null;
    public SockAddr LiveShowServerAddr
    {
        get { return mLiveShowServerAddr; }
        set { mLiveShowServerAddr = value; }
    }
    public bool IsLiveShow() { return mIsLiveShow; }
    public void SetLiveShow() { mIsLiveShow = true; }
    public void ReadyToLiveShow()
    {
        if (FrameSync.instance != null)
            FrameSync.instance.StartFrameSync((uint)ClientApplication.playerinfo.session);
    }
    public void StartLiveShow()
    {
        Logger.LogForReplay("[REPLAY]StartReplay");
        isReplay = true;
        mIsLiveShow = true;
        timeAcc = 0;
        started = false;
        replaySource = ReplayPlayFrom.MONEY_REWARD;
        lastPlaying = true;
        pause = false;
#if MG_TEST_EXTENT && !LOGIC_SERVER
        if (FrameSync.instance != null)
        {
            FrameSync.instance.PushUDPCommand("StartLiveShow ");
        }
#endif
    }
    public void SetBattle(PVPBattle newBattle)
    {
        battle = newBattle;
        if (battle != null)
            battle.isReplay = true;
    }
    private List<int> mRemoveChasingFrame = new List<int>();
    private bool _isChasingStartFrame(uint frame)
    {
        if (recordData == null) return false;
        if (recordData.chasingInfo.Count == 0) return false;
        if (recordData.chasingInfo[0].startFrame >= frame)
        {

            return true;
        }
        else
        {
            mRemoveChasingFrame.Clear();
            for (int i = 0; i < recordData.chasingInfo.Count; i++)
            {
                var curInfo = recordData.chasingInfo[i];
                //删除以前还没有执行到的元素 （很有可能同一个逻辑帧里多次追帧，所以完全可能存在某一个逻辑帧里面存在多个元素）
                if (curInfo.startFrame < frame)
                {
                    mRemoveChasingFrame.Add(i);
                }
                else
                {
                    return false;
                }
            }
            for (int i = mRemoveChasingFrame.Count - 1; i >= 0; i--)
            {
                recordData.chasingInfo.RemoveAt(mRemoveChasingFrame[i]);
            }
        }
        return false;
    }
    private bool _isChasingEndFrame(uint frame)
    {
        if (recordData.chasingInfo[0].endFrame >= frame)
        {
            recordData.chasingInfo.RemoveAt(0);
            return true;
        }
        return false;
    }
    public ReplayErrorCode StartReplay(string filename=null, ReplayPlayFrom from= ReplayPlayFrom.VIDEO_FRAME, bool isEditMode = false, bool isOpenMark = false, bool isReplayMode = false)
	{
#if MG_TEST_EXTENT && !LOGIC_SERVER
        if (FrameSync.instance != null)
        {
            FrameSync.instance.PushUDPCommand(string.Format("StartReplay {0} from {1}",filename == null ? "None": filename,from));
        }
#endif
        if (TeamDataManager.GetInstance().HasTeam())
        {
            return ReplayErrorCode.HAS_TEAM;
        }
        string fileDir = string.Empty;
        if (!isEditMode)
        {
            if (filename == null || !HasReplay(filename))
            {
                Logger.LogWarningFormat("没有这个录像文件！！！:{0}", filename);
#if DEBUG_SETTING
                if (Global.Settings.pvpDefaultSesstionID != null && Global.Settings.pvpDefaultSesstionID.Length > 1)
                    filename = Global.Settings.pvpDefaultSesstionID;
                else
#endif
                    return ReplayErrorCode.FILE_NOT_FOUND;
            }

            Logger.LogForReplay("[REPLAY]StartReplay");
            isReplay = true;
            timeAcc = 0;
            started = false;

            if (from >= ReplayPlayFrom.VIDEO_FRAME)
            {
                lastPlaying = true;
                replaySource = from;
            }

            pause = false;

            Logger.LogForReplay("[REPLAY]StartReplay - CreateData");

            ClientApplication.playerinfo.session = Convert.ToUInt64(filename);

            CreateData(filename);
        }
        else
        {
            if (filename == null || !File.Exists(filename))
            {
                return ReplayErrorCode.FILE_NOT_FOUND;
            }
            string session = Path.GetFileName(filename);
            fileDir = Path.GetDirectoryName(filename);
            isReplay = true;
            timeAcc = 0;
            started = false;

            if (from >= ReplayPlayFrom.VIDEO_FRAME)
            {
                lastPlaying = true;
                replaySource = from;
            }
            pause = false;
            Logger.LogForReplay("[REPLAY]StartReplay - CreateData");
            ClientApplication.playerinfo.session = Convert.ToUInt64(session);
            recordData = new RecordData();
            recordData.DeSerialiaction(filename);
        }

#if !UNITY_EDITOR
        if (!CheckVersion(recordData.clientVersion))
			return ReplayErrorCode.VERSION_NOT_MATCH;
#endif


        Logger.LogForReplay("[REPLAY]StartReplay - init");

		SetRacePlayers(recordData.replayFile.header.players);
        if(recordData.replayFile.header.raceType == (byte)RaceType.PK_3V3 || recordData.replayFile.header.raceType == (byte)RaceType.ScoreWar)
        {
            BattleMain.OpenBattle(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, recordData.pvpDungeonID, recordData.sessionID);
        }
        else if (recordData.replayFile.header.raceType == (byte)RaceType.PK_3V3_Melee )
        {
            BattleMain.OpenBattle(BattleType.ScufflePVP, eDungeonMode.SyncFrame, recordData.pvpDungeonID, recordData.sessionID);
        }
        else if (recordData.replayFile.header.raceType == (byte)RaceType.GuildBattle)
        {
            BattleMain.OpenBattle(BattleType.GuildPVP, eDungeonMode.SyncFrame, recordData.pvpDungeonID, recordData.sessionID);
        }
        else if (recordData.replayFile.header.raceType == (byte)RaceType.PremiumLeaguePreliminay || recordData.replayFile.header.raceType == (byte)RaceType.PremiumLeagueBattle)
        {
            BattleMain.OpenBattle(BattleType.MoneyRewardsPVP, eDungeonMode.LocalFrame, recordData.pvpDungeonID, recordData.sessionID);
        }
        else
        {
            BattleMain.OpenBattle(BattleType.MutiPlayer, eDungeonMode.SyncFrame, recordData.pvpDungeonID, recordData.sessionID);
        }
        ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();

		battle = BattleMain.instance.GetBattle() as PVPBattle;
        if (battle != null)
			battle.isReplay = true;
        var baseBattle = BattleMain.instance.GetBattle() as BaseBattle;
        if(baseBattle != null)
        {
            baseBattle.PkRaceType = recordData.replayFile.header.raceType;
        }
#if !LOGIC_SERVER
        if (recordData.replayFile.header.raceType == (byte)RaceType.PK_3V3 || recordData.replayFile.header.raceType == (byte)RaceType.ScoreWar)
        {
            RecordServer.GetInstance().StartRecord(BattleType.PVP3V3Battle, eDungeonMode.SyncFrame, "123456789", true, false, isOpenMark, isReplayMode);
        }
        else if (recordData.replayFile.header.raceType == (byte)RaceType.PK_3V3_Melee)
        {
            RecordServer.GetInstance().StartRecord(BattleType.ScufflePVP, eDungeonMode.SyncFrame, "123456789", true, false, isOpenMark, isReplayMode);
        }
        else
        {
            RecordServer.GetInstance().StartRecord(BattleType.MutiPlayer, eDungeonMode.SyncFrame, "123456789", true, false, isOpenMark, isReplayMode);
        }
        if (isReplayMode)
        {
            RecordServer.GetInstance().SetMarkFile(string.Format("{0}/{1}.mark", fileDir, ClientApplication.playerinfo.session));
        }
#endif

        return ReplayErrorCode.SUCCEED;
	}
    public ReplayErrorCode StartPVEReplay(string filename = null, ReplayPlayFrom from = ReplayPlayFrom.VIDEO_FRAME, bool isOpenMark = false, bool isReplayMode = false)
    {
        if (filename == null) return ReplayErrorCode.FILE_NOT_FOUND;
        isReplay = true;
        timeAcc = 0;
        started = false;

        if (from >= ReplayPlayFrom.VIDEO_FRAME)
        {
            lastPlaying = true;
            replaySource = from;
        }

        pause = false;

        Logger.LogForReplay("[REPLAY]StartReplay - CreateData");
        recordData = new RecordData();
        recordData.DeSerialiaction(filename);
        BattleDataManager.GetInstance().ClearBatlte();
        BattleDataManager.GetInstance().ConvertDungeonBattleInfo(recordData.dungeonInfo);


        BattleDataManager.GetInstance().PlayerInfo = recordData.dungeonInfo.players;
        ClientApplication.racePlayerInfo = recordData.dungeonInfo.players;
        for (int i = 0; i < ClientApplication.racePlayerInfo.Length; ++i)
        {
            var current = ClientApplication.racePlayerInfo[i];
            if (current.accid == ClientApplication.playerinfo.accid)
            {
                ClientApplication.playerinfo.seat = current.seat;
            }
        }
        ClientApplication.playerinfo.session = recordData.dungeonInfo.session;
        ClientApplication.relayServer.ip = recordData.dungeonInfo.addr.ip;
        ClientApplication.relayServer.port = recordData.dungeonInfo.addr.port;
        ClientApplication.playerinfo.accid = recordData.dungeonInfo.players[0].accid;

        eDungeonMode mode = eDungeonMode.None;
        if (recordData.dungeonInfo.session == 0)
        {
            mode = eDungeonMode.LocalFrame;
        }
        else
        {
            mode = eDungeonMode.SyncFrame;
        }
        BattleType type = ChapterUtility.GetBattleType((int)recordData.dungeonInfo.dungeonId);
        BattleMain.OpenBattle(type, eDungeonMode.RecordFrame, (int)recordData.dungeonInfo.dungeonId, string.Empty);
        BaseBattle battle = BattleMain.instance.GetBattle() as BaseBattle;
        battle.recordServer.sessionID = recordData.dungeonInfo.session.ToString();
        battle.recordServer.StartRecord(type, mode, recordData.dungeonInfo.session.ToString(), true, true, isOpenMark, isReplayMode);
        if (isReplayMode)
        {
            string fileDir = Path.GetDirectoryName(filename);
            battle.recordServer.SetMarkFile(string.Format("{0}/{1}.mark", fileDir, ClientApplication.playerinfo.session));
        }
        battle.SetBattleFlag(recordData.dungeonInfo.battleFlag);

        battle.SetDungeonClearInfo(recordData.dungeonInfo.clearedDungeonIds);
        var raidBattle = battle as RaidBattle;
        if (raidBattle != null)
        {
            for (int i = 0; i < recordData.dungeonInfo.clearedDungeonIds.Length; i++)
            {
                raidBattle.DungeonDestroyNotify((int)recordData.dungeonInfo.clearedDungeonIds[i]);
            }
        }

        var guildBattle = battle as GuildPVEBattle;
        if (guildBattle != null && recordData.dungeonInfo.guildDungeonInfo != null)
        {
            guildBattle.SetBossInfo(recordData.dungeonInfo.guildDungeonInfo.bossOddBlood, recordData.dungeonInfo.guildDungeonInfo.bossTotalBlood);
            guildBattle.SetBuffInfo(recordData.dungeonInfo.guildDungeonInfo.buffVec);
        }

        var finalTestBattle = battle as FinalTestBattle;
        if (finalTestBattle != null && recordData.dungeonInfo.zjslDungeonInfo != null)
        {
            finalTestBattle.SetBossInfo(recordData.dungeonInfo.zjslDungeonInfo.boss1ID, recordData.dungeonInfo.zjslDungeonInfo.boss1RemainHp, recordData.dungeonInfo.zjslDungeonInfo.boss2ID, recordData.dungeonInfo.zjslDungeonInfo.boss2RemainHp);
            finalTestBattle.SetBuffInfo(recordData.dungeonInfo.zjslDungeonInfo.buffVec);
        }
        ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
        GameFrameWork.instance.StartCoroutine(HideJoyStick());
        return ReplayErrorCode.SUCCEED;
    }

    IEnumerator HideJoyStick()
    {
	    while (InputManager.instance == null || InputManager.instance.joystick == null)
		    yield return null;
	    GameObject joyStick = InputManager.instance.GetJoyStick();
	    joyStick.CustomActive(false);
    }

    public bool CheckVersion(uint replayVersion)
	{
        return VersionManager.GetInstance().ServerVersion() == replayVersion;
     //return false;
	}

	public void SetLastPlaying(bool flag)
	{
		lastPlaying = flag;
	}

	public bool IsLastPlaying()
	{
		return lastPlaying;
	}

	public void Start()
	{
		started = true;
		Logger.LogForReplay("[REPLAY]StartReplay - start");
        if (!IsLiveShow())
        {
            FrameSync.instance.StartFrameSync(recordData.startTime);
        }

        ClientSystemManager.GetInstance().CloseFrame<PkLoadingFrame>();
        if (!IsLiveShow())
        {
			FrameSync.instance.PushNetCommand(recordData.replayFile.frames);
		}

		if (battle != null)
			battle.StartCountDown();

		BattleMain.instance.Main.RegisterEventNew(BeEventSceneType.onStartPK, (args)=>{
			if (pkTimer == null)
			{
				pkTimer = BattleMain.instance.Main.pkTimer;
			}
		});
	}

	public void Clear()
	{
		EndReplay(false,"ReplayServer Clear");
	}
    DelayCallUnitHandle mDelayResultFrameHandle;
    public void EndReplay(bool openFrame=true,string reason = "")
	{
        if(!openFrame && mDelayResultFrameHandle.IsValid())
        {
            var activeSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
            if (activeSystem != null)
                ClientSystemManager.GetInstance().CloseFrame<PkReplayResultFrame>();
            mDelayResultFrameHandle.SetRemove(true);
        }

        if (!isReplay)
			return;

		if (battle != null)
			battle._stopRobotAI();
        if (IsLiveShow())
        {
            if (Network.NetManager.instance != null)
                Network.NetManager.instance.Disconnect(Network.ServerType.RELAY_SERVER);
        }
        Logger.LogForReplay("[REPLAY]EndReplay");
		isReplay = false;
		recordData = null;
		started = false;

        #if !LOGIC_SERVER
        if (IsLiveShow())
        {
            RecordServer.GetInstance().EndLiveShowRecord();
        }
        else
        {
            RecordServer.GetInstance().EndRecord("EndReplay reason " + reason);
        }
        UnityEngine.Time.timeScale = 1.0f;
        scaleTimeIndex = 0;
        #endif

        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIReplay>();
		if (battleUI != null)
		{
            battleUI.SetReplayVisible(false);
		}
        if (openFrame && !mDelayResultFrameHandle.IsValid())
        {
            //在这里打开界面
            mDelayResultFrameHandle = ClientSystemManager.GetInstance().delayCaller.DelayCall(5000, () =>
            {
                var activeSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
                if (activeSystem != null)
                    ClientSystemManager.GetInstance().OpenFrame<PkReplayResultFrame>();
            });
        }
        mIsInPersueMode = false;
        mIsLiveShow = false;
    }
    private bool mIsInPersueMode = false;
    public bool isInPersueMode { get { return mIsInPersueMode; } }
    public void StartPersue()
    {
        mIsInPersueMode = true;
    }
    private void _UpdateLiveShow(int delta)
    {
        bool needPersue = false;
        FrameSync.instance.UpdateLiveShowFrame(mIsInPersueMode, ref needPersue);
        if (mIsInPersueMode != needPersue)
        {
            mIsInPersueMode = needPersue;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.onLiveShowPursueModeChange);
        }
    }
    /*
	public void StartRobot()
	{
		if (BattleMain.battleType == BattleType.MutiPlayer)
		{
			var curSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
			if (curSystem != null)
			{
				//curSystem.StartTimer(Global.PK_TOTAL_TIME);

				var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
				for(int i=0; i<players.Count; ++i)
				{
					var actor = players[i].playerActor;
					if (actor != null && actor.aiManager != null)
					{
						actor.pauseAI = false;
						break;
					}
				}
			}
		}
	}*/

    public void Update(int delta)
	{
		if (!started || pause)
			return;

        //Logger.LogErrorFormat("ReplayServer Update!!!!!!");
        if (IsLiveShow())
        {
            _UpdateLiveShow(delta);
            return;
        }
        if (recordData == null) return;
        if (IsDurationEnd())
		{
            string reason = "";
            if (pkTimer != null)
            {
                reason = string.Format("ReplayServer update {0} -{1}",pkTimer.GetPassTime(),recordData.duration + 10);
            }
            else
            {
                reason = "pkTimer is Null";
            }
            Stop(true, reason);
		}

		int tick = (int)FrameSync.instance.frameMs;

		timeAcc += delta;

		while (timeAcc >= tick)
		{
			timeAcc -= tick;

			if (recordData == null)
				break;
			FrameSync.instance.curFrame += (uint)Global.Settings.logicFrameStep;
#if UNITY_EDITOR
            if (!FrameSync.instance.IsInChasingMode && _isChasingStartFrame(FrameSync.instance.curFrame))
            {
                FrameSync.instance.StartInChasingMode();
            }
#endif
            FrameSync.instance.UpdateReplayFrame(tick);
#if UNITY_EDITOR
            if (FrameSync.instance.IsInChasingMode && _isChasingEndFrame(FrameSync.instance.curFrame))
            {
                FrameSync.instance.EndChasingMode();
            }
#endif
        }

        FrameSync.instance.UpdateReplayFrameGraphic((int) (Time.deltaTime * 1000) );
	}

	public bool IsDurationEnd()
	{
		if (pkTimer != null)
		{
			return pkTimer.GetPassTime() >= (recordData.duration + 10);
		}

		return false;
	}

	public bool IsReplay()
	{
		return isReplay;
	}

	private void CreateData(string filename)
	{
		recordData = new RecordData();
		recordData.DeSerialiaction(RecordData.GenPath(filename));
	}

	private void SetRacePlayers(RacePlayerInfo[] players)
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

	public void Pause()
	{
		if (!started || pause)
			return;

		pause = true;
		BattleMain.instance.GetDungeonManager().PauseFight();
	}

	public void Resume()
	{
		if (!started || !pause)
			return;

		pause = false;
		BattleMain.instance.GetDungeonManager().ResumeFight();
	}

    int scaleTimeIndex = 0;
    int[] scaleFactors = new int[] { 1, 2, 4, 8 };
    public void ScaleTime()
    {
#if !LOGIC_SERVER
        scaleTimeIndex++;
        scaleTimeIndex %= scaleFactors.Length;
        UnityEngine.Time.timeScale = scaleFactors[scaleTimeIndex];
#endif
    }
    public int timeScaler { get { return scaleFactors[scaleTimeIndex]; } }

    public void Stop(bool openFrame=false,string reason = "")
	{
		EndReplay(openFrame, "Stop reason " + reason);
	}

	public bool HasRecord()
	{
		bool ret = false;
		string name = RecordData.GenFileName();
		string path = RecordData.GenPath(name);
		var tokens = name.Split('_');
		if (CFileManager.IsFileExist(path) && 
			tokens.Length == 3 && 
			tokens[1] == VersionManager.GetInstance().ServerVersion().ToString())
			ret = true;

		return ret;
	}

	public void SwitchWatchPlayer(bool left)
	{
		var dungeonManager = BattleMain.instance.GetDungeonManager();
		var dungeonPlayerManager = BattleMain.instance.GetPlayerManager();

		var players = dungeonPlayerManager.GetAllPlayers();
        var battle = BattleMain.instance.GetBattle() as PVP3V3Battle;
        if (battle != null)
        {
            BattlePlayer player = dungeonPlayerManager.GetCurrentTeamFightingPlayer(left ? BattlePlayer.eDungeonPlayerTeamType.eTeamRed : BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
            if (player != null)
            {
                player.playerActor.isLocalActor = true;
#if !LOGIC_SERVER
                dungeonManager.GetGeScene().AttachCameraTo(player.playerActor.m_pkGeActor);
#endif
            }
        }
        else
        {
            var player = left ? players[0] : players[1];
            var otherPlayer = left ? players[1] : players[0];
            if (player != null && otherPlayer != null)
            {
                player.playerActor.isLocalActor = true;
                otherPlayer.playerActor.isLocalActor = false;
#if !LOGIC_SERVER
                dungeonManager.GetGeScene().AttachCameraTo(player.playerActor.m_pkGeActor);
#endif
            }
        }
	}

	//辅助函数
	public bool HasReplay(string filename)
	{
		return FileUtil.HasFile(RecordData.GenPath(filename));
	}

    public ReplayErrorCode CompressRecord(string fileName)
    {
        if (fileName == null || !HasReplay(fileName))
        {
            Logger.LogWarningFormat("没有这个录像文件！！！:{0}", fileName);
            return ReplayErrorCode.FILE_NOT_FOUND;
        }
        string rootPath = RecordData.GetRootPath();
        if (!Directory.Exists(rootPath))
        {
            Logger.LogErrorFormat("[UpLoadRecord] rootPath is NotExist {0}", rootPath);
            return ReplayErrorCode.FILE_NOT_FOUND;
        }
        string targetFileName = string.Format("{0}_{1}_{2}_pvp_upload.zip", Global.Settings.sdkChannel, ClientApplication.adminServer.id, fileName);
        string path = Path.Combine(rootPath, targetFileName);
        if (File.Exists(path)) return ReplayErrorCode.SUCCEED;
        string[] files = Directory.GetFiles(rootPath);
        string keywords = string.Format("s{0}", fileName);
        List<string> fitFileNames = new List<string>();
        fitFileNames.Add(Path.Combine(rootPath, fileName));
        for (int i = 0; i < files.Length; ++i)
        {
            string extName = Path.GetExtension(files[i]);
            if (extName.CompareTo(".txt") == 0)
            {
                string curFileName = Path.GetFileNameWithoutExtension(files[i]);
                if (curFileName.Contains(keywords))
                {
                    fitFileNames.Add(files[i]);
                }
            }
        }
        if (!File.Exists(path))
        {
            try
            {
                if (!LibZip.LibZipFileReader.CompressFiles(path, fitFileNames.ToArray()))
                {
                    Logger.LogErrorFormat("[UpLoadRecord] CompressFiles fail {0}", path);
                    return ReplayErrorCode.COMPRESS_ERROR;
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[UpLoadRecord Exception] CompressFiles fail {0}", e.Message);
                return ReplayErrorCode.COMPRESS_ERROR;
            }
        }
        return ReplayErrorCode.SUCCEED;
    }



}
