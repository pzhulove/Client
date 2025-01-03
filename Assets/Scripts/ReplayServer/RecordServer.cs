using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System.Text;
using System.IO;
using GameClient;
using System.Diagnostics;
public enum RecordMode
{
	PVP = 0,
	TEAM,
	SINGLE
}

#if LOGIC_SERVER
public class RecordServer /*: Singleton<RecordServer>*/
#else
public class RecordServer : Singleton<RecordServer>
#endif
{
	protected FrameRandomImp mframeRandom;
	public FrameRandomImp FrameRandom{
		set{
			mframeRandom = value;
		}

		get{
			return mframeRandom;
		}
	}
	private RecordData recordData;
	private bool mIsRecordProcess;
 
	private int timeAcc = 0;
	private StringBuilder contentRecorder;
	private StringBuilder contentRecorder2;
	private bool mIsRecordReplay = false;
	public string sessionID = string.Empty;
 
	protected BattleType type;
	protected eDungeonMode mode;

    private bool bNeedUploadRecordFile = false;

	private bool openRecordFunction = true;
    private bool mIsRecording = false;
    private List<string> cacheFilesList = new List<string>();

	public BattleType battleType = BattleType.None;
    private StringBuilder reconnectRecorder;

    private bool mIsLogicServerSaveRecordInTheEnd = false;
    private LogicServer mLogicServer = null;

    protected bool m_HaveSaveRecordBattle = false;
    private RecordMarkSystem m_markSys = null;
    public bool HaveSaveRecordBattle { get { return m_HaveSaveRecordBattle; } set { m_HaveSaveRecordBattle = value; } }

    public void SetLogicServer(LogicServer logic)
    {
        mLogicServer = logic;
        if (m_markSys != null)
        {
            m_markSys.SetLogicServer(logic);
        }
    }
    public void Mark(uint id)
    {
        if (m_markSys != null)
        {
            m_markSys.Mark(id);
        }
    }

    public void MarkInt(uint id, params int[] paramData)
    {
        if (m_markSys == null) return;
        m_markSys.MarkInt(id, paramData);
    }
    public void MarkString(uint id, params string[] paramData)
    {
        if (m_markSys == null) return;
        m_markSys.MarkString(id, paramData);
    }
    public void Mark(uint id, int[] paramData, params string[] paramDataStr)
    {
        if (m_markSys == null) return;
        m_markSys.Mark(id, paramData, paramDataStr);
    }
    public void Mark(uint id, string[] paramDataStr, params int[] paramData)
    {
        if (m_markSys == null) return;
        m_markSys.Mark(id, paramDataStr, paramData);
    }
    public void SetMarkFile(string fileName)
    {
        if (m_markSys != null)
        {
            m_markSys.Load(fileName);
        }
    }
    public void BeginUpdate()
    {
        if (m_markSys != null)
        {
            m_markSys.BeginUpdate();
        }
    }
    public void EndUpdate()
    {
        if (m_markSys != null)
        {
            m_markSys.EndUpdate();
        }
    }
    public bool isLogicServerSaveRecordInTheEnd
    {
        get 
        {
#if !LOGIC_SERVER
            return mIsLogicServerSaveRecordInTheEnd;
//#elif LOGIC_SERVER && MG_TEST
//            return true;
#else
            return false;
#endif
        }
        
        set 
        {
            mIsLogicServerSaveRecordInTheEnd = value;
        }
    }

    public RecordData GetRecordData()
    {
        return recordData;
    }


	#if LOGIC_SERVER
	public /*override*/  void Init ()
	{
        /*base.Init ();*/
#if !LOGIC_SERVER
        openRecordFunction = GameClient.SwitchFunctionUtility.IsOpen(15);
#else
        openRecordFunction = true;
#endif

    }
#else
	public override  void Init ()
	{
		base.Init ();

		openRecordFunction = GameClient.SwitchFunctionUtility.IsOpen(15);
	}
#endif

    public void StartRecord(BattleType type, eDungeonMode mode,string sessionID, bool IsRecordProcess,bool IsRecordReplay, bool needMark = true, bool isReplayMode = false)
	{
		Logger.LogForReplay("[RECORD]StartRecord");
		Clear();

        this.type             = type;
        this.mode             = mode;
        this.sessionID        = sessionID;
        this.mIsRecordProcess = IsRecordProcess;
        this.mIsRecordReplay  = IsRecordReplay;


#if LOGIC_SERVER
        mIsRecordProcess      = RecordServerConfigManager.instance.isRecordProcess();
        mIsRecordReplay       = RecordServerConfigManager.instance.isRecordReplay();
#elif UNITY_EDITOR
        // pceditor客户端 打开日志，打开录像
        if (Global.Settings.isDebug)
        {
            mIsRecordProcess = true;
            mIsRecordReplay = true;
        }
        //else
        //{
        //    mIsRecordProcess = false;
        //    mIsRecordReplay  = false;
        //}
#endif
        mIsRecording = true;
        bNeedUploadRecordFile = false;

        if (mIsRecordReplay)
		{
			recordData = new RecordData();
			recordData.sessionID = sessionID;
#if !SERVER_LOGIC
            recordData.clientVersion = VersionManager.GetInstance().ServerVersion();
#else
			recordData.clientVersion = 0;
#endif
        }
			
		timeAcc = 0;
        if (mIsRecordProcess)
        {
#if RECORD_FILE
            contentRecorder = StringBuilderCache.Acquire(1024 * 1024);
#endif
#if !SERVER_LOGIC
#if RECORD_FILE
            contentRecorder.AppendFormat("[Normal Battle Log]Version:{0} SessionID:{1} BattleType:{2} DungeonMode:{3}\n", VersionManager.GetInstance().Version(), sessionID, type, mode);
#else
            if (needMark)
            {
                if (isReplayMode)
                {
                    m_markSys = new RecordMarkSystem(RECORD_MODE.REPLAY, FrameSync.instance, VersionManager.GetInstance().Version(), sessionID, type, mode);
                    m_markSys.SetRandom(FrameRandom);
                }
                else
                {
                    m_markSys = new RecordMarkSystem(RECORD_MODE.RECORD, FrameSync.instance, VersionManager.GetInstance().Version(), sessionID, type, mode);
                    m_markSys.SetRandom(FrameRandom);
                }
            }
#endif
            PushReconnectCmd(string.Format("StartRecord mIsRecordProcess {0} mIsRecordReplay {1}", mIsRecordProcess, mIsRecordReplay));

#if !LOGIC_SERVER && MG_TEST
            _SaveSkillBar();
#endif

#else

#if RECORD_FILE
            contentRecorder.AppendFormat("[Server Battle Log]Version:{0} SessionID:{1} BattleType:{2} DungeonMode:{3}\n", RecordServerConfigManager.instance.GetVersion(), sessionID, type, mode);
#else
            if (needMark)
            {
                if (isReplayMode)
                {
                    m_markSys = new RecordMarkSystem(RECORD_MODE.REPLAY, mLogicServer,RecordServerConfigManager.instance.GetVersion(),sessionID, type, mode);
                    m_markSys.SetRandom(FrameRandom);
                }
                else
                {
                    m_markSys = new RecordMarkSystem(RECORD_MODE.RECORD, mLogicServer,RecordServerConfigManager.instance.GetVersion(),sessionID, type, mode);
                    m_markSys.SetRandom(FrameRandom);
                }
            }
#endif
#endif
        }
        else
        {
            contentRecorder = null;
        }

    }

    public void MarkRecordFileNeedUpload()
    {
        bNeedUploadRecordFile = true;
    }

	public void StartRecordProcess()
	{
		mIsRecordProcess = true;
		mIsRecordReplay = false;
		contentRecorder = StringBuilderCache.Acquire(1024*1024);
#if !SERVER_LOGIC
			contentRecorder.AppendFormat("[Normal Battle Log]SessionID:{0} \n", sessionID);
#else
			contentRecorder.AppendFormat("[Server Battle Log]SessionID:{0} \n", sessionID);
#endif
	}
    //保存开始追帧的逻辑帧号
    public void RecordChasingStartFrame(uint frame, string outputDebugStr)
    {
        PushReconnectCmd(string.Format("StartChasing {0} at {1}", outputDebugStr, frame));
        if (!mIsRecordReplay || recordData == null) return;
        recordData.PushChasingStartFrame(frame);
    }
    // 保存结束追帧的逻辑帧号
    public void RecordChasingEndFrame(uint frame, string outputDebugStr)
    {
        PushReconnectCmd(string.Format("EndChasing {0} at {1}", outputDebugStr, frame));
        if (!mIsRecordReplay || recordData == null) return;
        recordData.PushChasingEndFrame(frame);
    }
    public void RecordSequence(uint sequence)
    {
        if(contentRecorder != null)
        {
            contentRecorder.AppendFormat("[{0}]", sequence);
        }
    }
	public void RecordProcessPlayerInfo(IDungeonPlayerDataManager playMgr)
	{
		var players = playMgr.GetAllPlayers();
		var mainPlayer = playMgr.GetMainPlayer();

        //contentRecorder.AppendFormat("self seat:{0} name:{1}", mainPlayer.playerInfo.seat, mainPlayer.playerInfo.name);
        //contentRecorder2.AppendFormat("self seat:{0} name:{1}", mainPlayer.playerInfo.seat, mainPlayer.playerInfo.name);
        if (m_markSys != null)
        {
            StringBuilder playerInfo = new StringBuilder();
            for (int i = 0; i < players.Count; ++i)
            {
                var player = players[i];
                playerInfo.AppendFormat("Seat:{0} Name:{1} Lv:{2} occ:{3} zoneId:{4} accid:{5}", player.playerInfo.seat, player.playerInfo.name, player.playerInfo.level, ((ActorOccupation)player.playerInfo.occupation), player.playerInfo.zoneId, player.playerInfo.accid);
            }
            m_markSys.MarkString(0xeeeeee, playerInfo.ToString());
        }

        if (contentRecorder != null)
        {
            contentRecorder.Append("F[-]");
            for (int i = 0; i < players.Count; ++i)
            {
                var player = players[i];
                contentRecorder.AppendFormat("Seat:{0} Name:{1} Lv:{2} occ:{3} zoneId:{4} accid:{5}", player.playerInfo.seat, player.playerInfo.name, player.playerInfo.level, ((ActorOccupation)player.playerInfo.occupation), player.playerInfo.zoneId, player.playerInfo.accid);
            }
            contentRecorder.Append("\n");
        }
	}
    private void FlushFile(string path, ref StringBuilder content)
    {
        if (content == null) return;
        StreamWriter sw = null;
        if (File.Exists(path))
        {

            try
            {
                sw = new StreamWriter(path, true);
                sw.Write(content.ToString());
                sw.Flush();
                sw.Close();
            }
            catch (System.Exception e)
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
                Logger.LogErrorFormat("Flush file failed!!!!!!:{0} reason {1} \n", path, e.ToString());
            }
        }
        else
        {
            try
            {
                sw = new StreamWriter(path);
                sw.Write(content.ToString());
                sw.Flush();
                sw.Close();
            }
            catch (System.Exception e)
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
                Logger.LogErrorFormat("Flush file failed!!!!!!:{0} reason {1} \n", path, e.ToString());
            }
        }
        content.Clear();
    }
    public void FlushProcess()
    {
        if (!mIsRecordProcess) return;
        if (contentRecorder != null)
        {
            string fileName = string.Format("s{0}_{1}_{2}process.txt", sessionID, type, mode);
            string name = string.Format("{0}_{1}", VersionManager.GetInstance() != null ? VersionManager.GetInstance().ClientShortVersion():0, fileName);
            string path = RecordData.GenPath(name);
            FlushFile(path, ref contentRecorder);
        }
        if (contentRecorder2 != null)
        {
            string fileName = string.Format("s{0}_{1}_{2}process_receive_package.txt", sessionID, type, mode);
            string name = string.Format("{0}_{1}", VersionManager.GetInstance() != null ? VersionManager.GetInstance().ClientShortVersion() : 0, fileName);
            string path2 = RecordData.GenPath(name);
            FlushFile(path2, ref contentRecorder2);
        }
        if (m_markSys != null)
        {
            string path = RecordData.GenPath(BeUtility.Format("{0}.mark", sessionID));
            m_markSys.Flush(path, RecordData.buffer);
        }
    }

    void SafeRelease(ref StringBuilder s)
	{
		if(s != null)
		{
			StringBuilderCache.Release(s);
			s = null;
		}
	}
	public void CreateCR2()
	{
		if (!mIsRecordProcess)
			return;
		if (contentRecorder2 == null)
		{
			contentRecorder2 = StringBuilderCache.Acquire(1024*1024);

#if !LOGIC_SERVER
            contentRecorder2.AppendFormat("Version:{0} sessionID:{1} \n", VersionManager.GetInstance().Version(), sessionID);
#else
            contentRecorder2.AppendFormat("Version:{0} sessionID:{1} \n", RecordServerConfigManager.instance.GetVersion() , sessionID);
#endif

        }
    }

	public bool IsProcessRecord()
	{
#if !LOGIC_SERVER
		
        if (!openRecordFunction)
			return false;
		
		return mIsRecordProcess;

#else
        return true;
#endif
    }

    public bool IsReplayRecord(bool force=false)
	{
		if (!openRecordFunction)
			return false;
		if(force)
			mIsRecordReplay = true;

		return mIsRecordReplay;
	}

	public void EndRecord(string reason)
	{
#if LOGIC_SERVER
        if (!isLogicServerSaveRecordInTheEnd)
        {
            Clear();
            return ;
        }
#endif
#if !LOGIC_SERVER && MG_TEST
        SaveSkillBtnClick();
         _SaveSkillBar();
        PushReconnectCmd(string.Format("EndRecord mIsRecordProcess {0} mIsRecordReplay {1} reason {2}", mIsRecordProcess, mIsRecordReplay,reason));
#endif
        Logger.LogForReplay("[RECORD]EndRecord");
        bool bRecordReplay = mIsRecordReplay;
        bool bRecordProcess = mIsRecordProcess;
        if (mIsRecordReplay)
		{
            _saveRecordReplay();
            mIsRecordReplay = false;
		}
        EndRecordProcess();
        mIsRecording = false;
        SaveReconnectCmd();
        if(mLogicServer != null)
        {
            mLogicServer = null;
        }
#if !LOGIC_SERVER
        if (bRecordReplay || bRecordProcess)
        {
            _CompressAndDeleteOldFile();
        }
#endif
        _compressFilsAndUpload();

      
    }
    public void EndLiveShowRecord()
    {
#if !LOGIC_SERVER && MG_TEST
        SaveSkillBtnClick();
        
        PushReconnectCmd(string.Format("EndLiveShowRecord mIsRecordProcess {0} mIsRecordReplay {1}", mIsRecordProcess, mIsRecordReplay));
#endif
        Logger.LogForReplay("[RECORD]EndRecord");
        bool bRecordReplay = mIsRecordReplay;
        bool bRecordProcess = mIsRecordProcess;
        if (mIsRecordReplay)
        {
            _saveRecordReplay(true);
            mIsRecordReplay = false;
        }
        EndRecordProcess();
        mIsRecording = false;
        SaveReconnectCmd();
    }
    private void _SaveSkillBar()
    {
        List<Protocol.SkillBarGrid> skillList = SkillDataManager.GetInstance().GetPveSkillBar();
        PushReconnectCmd(BeUtility.Format("Save SkillBar Begin {0} {1} {2} {3}", BattleMain.battleType, BattleDataManager.GetInstance().PkRaceType, SkillDataManager.GetInstance().CurPVESKillPage, SkillDataManager.GetInstance().CurPVPSKillPage));
        PushReconnectCmd(BeUtility.Format("PVP SkillBarCount :{0}", skillList.Count));
        if (skillList.Count >= 0)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                if (skillList[i] != null)
                {
                    PushReconnectCmd(BeUtility.Format("PVP SkillBar:{0} id:{1} slot {2}", i, skillList[i].id, skillList[i].slot));
                }
            }
        }
        skillList = SkillDataManager.GetInstance().GetPvpSkillBar();
        PushReconnectCmd(BeUtility.Format("PVE SkillBarCount :{0}", skillList.Count));
        if (skillList.Count >= 0)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                if (skillList[i] != null)
                {
                    PushReconnectCmd(BeUtility.Format("PVE SkillBar:{0} id:{1} slot {2}", i, skillList[i].id, skillList[i].slot));
                }
            }
        }
        PushReconnectCmd("Save SkillBar End");
    }
    /// <summary>
    /// 将用户技能按键按下的消息存入Reconnect文件中
    /// </summary>
    private void SaveSkillBtnClick()
    {
#if MG_TEST_EXTENT && !LOGIC_SERVER
        if (InputManager.instance == null)
            return;
        int count = InputManager.instance.skillOnClickQue.Count;
        for (int i=0;i< count; i++)
        {
            string str = InputManager.instance.skillOnClickQue.Dequeue().ToString();
            PushReconnectCmd(str);
        }
#endif
    }

    string mLastRecordTimeStamp = string.Empty;
    public int GetCurrentRecordSize()
    {
        string rootPath = RecordData.GetRootPath();
        if (!Directory.Exists(rootPath))
        {
            Logger.LogErrorFormat("[_CompressAndDeleteOldFile] rootPath is NotExist {0}", rootPath);
            return 0;
        }
        string sessionId = this.sessionID.ToString();
        string targetFileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_upload.zip", Global.Settings.sdkChannel, ClientApplication.adminServer.id, sessionId, type, mode, mLastRecordTimeStamp);
        string path = Path.Combine(rootPath, targetFileName);
        if (!File.Exists(path))
        {
            //模糊查找
            string[] files = Directory.GetFiles(rootPath);
            string keywords = string.Format("{0}_{1}_{2}_{3}_{4}_", Global.Settings.sdkChannel, ClientApplication.adminServer.id, sessionId, type, mode);
            for (int i = 0; i < files.Length; ++i)
            {
                string extName = Path.GetExtension(files[i]);
                if (extName.CompareTo(".zip") == 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(files[i]);
                    if (fileName.Contains(keywords))
                    {
                        var rightFileInfo = new FileInfo(files[i]);
                        return (int)rightFileInfo.Length;
                    }
                }
            }
            return 0;
        }
        FileInfo fileInfo = new FileInfo(path);
        return (int)fileInfo.Length;
    }
    public int GetPkRecordSize(string sessionId)
    {
        string targetFileName = string.Format("{0}_{1}_{2}_pvp_upload.zip", Global.Settings.sdkChannel, ClientApplication.adminServer.id, sessionId);
        string rootPath = RecordData.GetRootPath();
        string path = Path.Combine(rootPath, targetFileName);
        if (!File.Exists(path))
        {
            return 0;
        }
        FileInfo fileInfo = new FileInfo(path);
        return (int)fileInfo.Length;
    }
    public class UploadInfo
    {
        public string platform;
        public string channel;
        public string zone_id;
        public string zone_name;
        public string player_name;
        public string player_id;
        public string version;
        public string replay;
        public string record_type;
        public string content;
    };
    public bool UpLoadCurrentRecordFile(ref string errorReason, int type, string desc)
    {
        string rootPath = RecordData.GetRootPath();
        if (!Directory.Exists(rootPath))
        {
            Logger.LogErrorFormat("[_CompressAndDeleteOldFile] rootPath is NotExist {0}", rootPath);
            errorReason = "找不到文件路径";
            return false;
        }
        string sessionId = this.sessionID.ToString();
        string targetFileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_upload.zip", Global.Settings.sdkChannel, ClientApplication.adminServer.id, sessionId, this.type, mode, mLastRecordTimeStamp);
        string path = Path.Combine(rootPath, targetFileName);
        if (!File.Exists(path))
        {

            string[] files = Directory.GetFiles(rootPath);
            string keywords = string.Format("{0}_{1}_{2}_{3}_{4}_", Global.Settings.sdkChannel, ClientApplication.adminServer.id, sessionId, this.type, mode);
            bool bFinded = false;
            for (int i = 0; i < files.Length; ++i)
            {
                string extName = Path.GetExtension(files[i]);
                if (extName.CompareTo(".zip") == 0)
                {
                    string findFileName = Path.GetFileNameWithoutExtension(files[i]);
                    if (findFileName.Contains(keywords))
                    {
                        path = files[i];
                        targetFileName = findFileName;
                        bFinded = true;
                        break;
                    }
                }
            }
            if (!bFinded)
            {
                errorReason = "找不到文件 或者已经上传过了";
                return false;
            }
        }

        string fileName = Path.GetFileName(path);
        try
        {

            Http.UploadFile(string.Format("http://{0}/replay?file={1}", Global.RECORDLOG_GET_ADDRESS, fileName), path);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("UploadLastFile occur error reason {0}", e.Message);
            errorReason = "上传失败";
            return false;
        }
        UploadInfo uploadInfo = null;
#if UNITY_IOS || UNITY_IPHONE
        uploadInfo = new UploadInfo
        {
            platform = "IOS",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            version = VersionManager.GetInstance().Version(),
            replay = targetFileName,
            record_type = type.ToString(),
            content = desc
        };
#elif UNITY_ANDROID
        uploadInfo = new UploadInfo
        {
            platform = "Android",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            version = VersionManager.GetInstance().Version(),
            replay = targetFileName,
            record_type = type.ToString(),
            content = desc
        };
#else
        uploadInfo = new UploadInfo
        {
            platform = "PC",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            replay = targetFileName,
            record_type = type.ToString(),
#if !LOGIC_SERVER
            version = VersionManager.GetInstance().Version(),
#else
            version = "",
#endif
            content = desc
        };
#endif
        if (uploadInfo != null)
        {
            try
            {
                var jsonText = LitJson.JsonMapper.ToJson(uploadInfo);
                Http.SendPostRequest(string.Format("http://{0}/desc", Global.RECORDLOG_POST_ADDRESS), jsonText);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("UploadLastFile occur error reason {0}", e.Message);
                errorReason = "输入用户信息失败";
                return false;
            }
        }
        try
        {
            File.Delete(path);
        }
        catch (Exception e)
        {
            return true;
        }
        return true;
    }

    public bool UpLoadRecordFile(string fileName, ref string errorReason, int type, string desc)
    {
        string targetFileName = string.Format("{0}_{1}_{2}_pvp_upload.zip", Global.Settings.sdkChannel, ClientApplication.adminServer.id, fileName);
        string rootPath = RecordData.GetRootPath();
        string path = Path.Combine(rootPath, targetFileName);
        if (!File.Exists(path))
        {
            errorReason = "找不到文件";
            return false;
        }
        string upLoadFileName = Path.GetFileName(path);
        try
        {

            Http.UploadFile(string.Format("http://{0}/replay?file={1}", Global.RECORDLOG_GET_ADDRESS, upLoadFileName), path);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("UploadLastFile occur error reason {0}", e.Message);
            errorReason = "上传失败";
            return false;
        }
        UploadInfo uploadInfo = null;
#if UNITY_IOS || UNITY_IPHONE
        uploadInfo = new UploadInfo
        {
            platform = "IOS",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            replay = targetFileName,
            record_type = type.ToString(),
            version = VersionManager.GetInstance().Version(),
            content = desc
        };
#elif UNITY_ANDROID
          uploadInfo = new UploadInfo
        {
            platform = "Android",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            replay = targetFileName,
            version = VersionManager.GetInstance().Version(),
            record_type = type.ToString(),
            content = desc
        };
#else
        uploadInfo = new UploadInfo
        {
            platform = "PC",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            replay = targetFileName,
            record_type = type.ToString(),
#if !LOGIC_SERVER
            version = VersionManager.GetInstance().Version(),
#else
            version = "",
#endif
            content = desc
        };
#endif
        if (uploadInfo != null)
        {
            var jsonText = LitJson.JsonMapper.ToJson(uploadInfo);
            try
            {
                Http.SendPostRequest(string.Format("http://{0}/desc", Global.RECORDLOG_POST_ADDRESS), jsonText);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("UpLoadRecordFile occur error reason {0}", e.Message);
                errorReason = "输入用户信息失败";
                return false;
            }
        }
        try
        {
            File.Delete(path);
        }
        catch (Exception e)
        {
            return true;
        }
        return true;
    }
    private void _CompressAndDeleteOldFile()
    {

        string rootPath = RecordData.GetRootPath();

        if (!Directory.Exists(rootPath))
        {
            Logger.LogErrorFormat("[_CompressAndDeleteOldFile] rootPath is NotExist {0}", rootPath);
            return;
        }

        string[] files = Directory.GetFiles(rootPath);

        string sessionId = this.sessionID.ToString();
        string playerId = Utility.GetCurrentTimeUnix().ToString();
        mLastRecordTimeStamp = playerId;
        cacheFilesList.Clear();
        List<string> zipFileList = new List<string>();

        for (int i = 0; i < files.Length; ++i)
        {
            if (!files[i].EndsWith(".zip") && files[i].Contains(sessionId))
            {
                cacheFilesList.Add(files[i]);
            }
            string extName = Path.GetExtension(files[i]);
            if (extName.CompareTo(".zip") == 0)
            {
                zipFileList.Add(files[i]);
            }
        }

        try
        {
            string targetFileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_upload.zip", Global.Settings.sdkChannel, ClientApplication.adminServer.id, sessionId, type, mode, playerId);
            string path = Path.Combine(rootPath, targetFileName);
            if (!LibZip.LibZipFileReader.CompressFiles(path, cacheFilesList.ToArray()))
            {
                Logger.LogErrorFormat("[_CompressAndDeleteOldFile] CompressFiles fail {0}", path);
            }
        }
        catch(Exception e)
        {
            Logger.LogErrorFormat("[_CompressAndDeleteOldFile Exception] CompressFiles fail {0}",e.Message);
        }

        try
        {
            _DeleteFileList(zipFileList);
        }
        catch(Exception e)
        {
            Logger.LogErrorFormat("[_CompressAndDeleteOldFile Exception] DeleteZipFile fail ");
        }
    }
    private void _saveRecordReplay(bool isInBattleRecord = false)
    {
        if (recordData == null) return;
        recordData.Serialization(isInBattleRecord);
        if (isInBattleRecord) return;
        recordData.playerInfo = null;
        recordData.resultInfo = null;
        recordData.endReq = null;
        recordData.frameInfo.Clear();
        recordData.replayFile = null;
        recordData = null;
    }

    public void LogicServerSaveRecordAndReplay()
    {
#if LOGIC_SERVER
        _saveRecordReplay();
        _saveRecordProcess();
        mIsRecordProcess = false;
        mIsRecordReplay = false;
#endif
    }

    private void _compressFilsAndUpload()
    {
        if (!bNeedUploadRecordFile)
        {
            Logger.LogProcessFormat("[_compressFilsAndUpload] bNeedUploadRecordFile is false");
            return ;
        }

        string rootPath  = RecordData.GetRootPath();

        if (!Directory.Exists(rootPath))
        {
            Logger.LogErrorFormat("[_compressFilsAndUpload] rootPath is NotExist {0}", rootPath);
            return ;
        }

        string[] files   = Directory.GetFiles(rootPath);

        string sessionId = this.sessionID.ToString();
		string playerId = Utility.GetCurrentTimeUnix().ToString ();

        cacheFilesList.Clear();

        //UnityEngine.Debug.LogFormat("[_compressFilsAndUpload] session {0}, playerid {1}, findPath {2}", sessionId, playerId, rootPath);

        for (int i = 0; i < files.Length; ++i)
        {
            //UnityEngine.Debug.LogFormat("[_compressFilsAndUpload] try add File {0}", files[i]);

            if (!files[i].EndsWith(".zip") && files[i].Contains(sessionId))
            {
                //UnityEngine.Debug.LogFormat("[_compressFilsAndUpload] add File {0}", files[i]);
                cacheFilesList.Add(files[i]);
            }
        }

        string targetFileName = string.Format("{0}_{1}_{2}_{3}.zip", sessionId, type, mode, playerId);
        string path           = Path.Combine(rootPath, targetFileName);

        UnityEngine.Debug.LogFormat("[_compressFilsAndUpload] try compress File {0}", path);

        if (LibZip.LibZipFileReader.CompressFiles(path, cacheFilesList.ToArray()))
        {
            _uploadHttpFile(path);
            
            _DeleteFileList(cacheFilesList);
        }
        else
        {
            Logger.LogErrorFormat("[_compressFilsAndUpload] CompressFiles fail {0}", path);
        }
    }
    
    private void _DeleteFileList(IList<string> files)
    {
#if !ROBOT_TEST
        for (int i = 0; i < files.Count; ++i)
        {
            if (File.Exists(files[i]))
            {
                File.Delete(files[i]);
            }
        }
#endif
    }
    

	public void EndRecordProcess()
	{
#if LOGIC_SERVER
        if (!isLogicServerSaveRecordInTheEnd)
        {
            return ;
        }
#endif

		if(mIsRecordProcess)
		{
            _saveRecordProcess();
            mIsRecordProcess = false;
		}
	}

    /// <summary>
    /// 真正*纯粹保存
    /// </summary>
    private void _saveRecordProcess()
    {
        if (m_markSys != null)
        {
            string path = RecordData.GenPath(BeUtility.Format("{0}.mark", sessionID));
            m_markSys.Save(path, RecordData.buffer);
            m_markSys = null;
        }
        SaveProcess();
        SafeRelease(ref contentRecorder);
        SafeRelease(ref contentRecorder2);
    }

	public void RecordStartTime(uint startTime)
	{
        if (!mIsRecordReplay)
        {
#if !LOGIC_SERVER
            PushReconnectCmd(string.Format("RecordStartTime {0}", mIsRecordReplay));
#endif
            return;
        }

		recordData.startTime = startTime;
		Logger.LogForReplay("[RECORD]RecordStartTime:{0}", startTime);
	}

	public void RecordPlayerInfo(WorldNotifyRaceStart info)
	{
        if (!mIsRecordReplay)
        {
#if !LOGIC_SERVER
            PushReconnectCmd(string.Format("RecordPlayerInfo {0}", mIsRecordReplay));
#endif
            return;
        }

		recordData.playerInfo = info;
        recordData.raceType = (RaceType)info.raceType;
        Logger.LogForReplay("[RECORD]RecordPlayerInfo");
	}

	public void RecordDungoenInfo(SceneDungeonStartRes info)
	{
        if (!mIsRecordReplay)
        {
#if !LOGIC_SERVER
            PushReconnectCmd(string.Format("RecordDungoenInfo {0}", mIsRecordReplay));
#endif
            return;
        }
		
		recordData.dungeonInfo = info;
	}
	public void RecordDungeonID(int dungeonID)
	{
        if (!mIsRecordReplay)
        {
#if !LOGIC_SERVER
            PushReconnectCmd(string.Format("RecordDungoenInfo {0}", mIsRecordReplay));
#endif
            return;
        }
		recordData.pvpDungeonID = dungeonID;

		Logger.LogForReplay("[RECORD]RecordDungeonID:{0}", recordData.pvpDungeonID);
	}

	public void RecordStartFrame()
	{
		if (!mIsRecordReplay)
			return;

		recordData.startFrame = FrameSync.instance.curFrame;
		Logger.LogForReplay("[RECORD]RecordStartFrame:{0}", recordData.startFrame);
	}

	public void RecordServerFrames(Frame[] frames)
	{
        if (!mIsRecordReplay)
        {
#if !LOGIC_SERVER
            PushReconnectCmd(string.Format("RecordServerFrames {0}", mIsRecordReplay));
#endif
            return;
        }

        if (frames == null || frames.Length <= 0)
            return;

		var frameData = new RecordFrameData(frames, timeAcc);
        if (frameData == null)
        {
            Logger.LogErrorFormat("[ERROR]create RecordFrameData failed!!!!");
            return;
        }
		recordData.frameInfo.Add(frameData);

		Logger.LogForReplay("[RECORD]RecordServerFrames tickTime:{0}", timeAcc);
	}

	public void RecordResult(SceneMatchPkRaceEnd ret)
	{
        if (!mIsRecordReplay)
        {
#if !LOGIC_SERVER
            PushReconnectCmd(string.Format("RecordResult {0}", mIsRecordReplay));
#endif
            return;
        }

		//recordData.resultInfo = ret;

		Logger.LogForReplay("[RECORD]RecordResult");
	}

	public void RecordEndReq(RelaySvrEndGameReq req, int duration)
	{
        if (!mIsRecordReplay)
        {
#if !LOGIC_SERVER
            PushReconnectCmd(string.Format("RecordEndReq {0}", mIsRecordReplay));
#endif
            return;
        }
		recordData.endReq = req;
		recordData.duration = duration;
		Logger.LogForReplay("[RECORD]RecordEndReq");
	}

	public void Update(int delta)
	{
		if (mIsRecordProcess)
		{
			timeAcc += delta;
		}
	}

	public void Clear()
	{
		mIsRecordProcess = false;
		mIsRecordReplay = false;
        mIsRecording = false;
        recordData = null;
        if(mLogicServer != null)
        {
            mLogicServer = null;
        }
		SafeRelease(ref contentRecorder);
		SafeRelease(ref contentRecorder2);
        SafeRelease(ref reconnectRecorder);
	}
    [Conditional("RECORD_FILE")]
    public void RecordProcess(string content)
	{
        if (!mIsRecordProcess)
        {
#if !LOGIC_SERVER
          //  PushReconnectCmd(string.Format("RecordProcess {0} {1} {2}", mIsRecordProcess,GetFrameInfo(),content));
#endif
            return;
        }

        if (contentRecorder == null)
        {
            //Logger.LogErrorFormat("[ERROR]contentRecorder is null sid:{0}", sessionID);
            return;
        }

		contentRecorder.Append(GetFrameInfo());
		contentRecorder.Append(content);
		contentRecorder.Append("\n");
	}
    [Conditional("RECORD_FILE")]
    public void RecordProcess(string format, params object[] args)
	{
        if (!mIsRecordProcess)
        {
#if !LOGIC_SERVER
        //    PushReconnectCmd(string.Format("RecordProcess args {0} {1} {2}", mIsRecordProcess,GetFrameInfo(),string.Format(format,args)));
#endif
            return;
        }

        if (contentRecorder == null)
        {
            //Logger.LogErrorFormat("[ERROR]contentRecorder is null sid:{0}", sessionID);
            return;
        }
            

		contentRecorder.Append(GetFrameInfo());
		contentRecorder.AppendFormat(format, args);
		contentRecorder.Append("\n");
	}

	public void RecordProcess2(string content)
	{
        if (!mIsRecordProcess)
        {
#if !LOGIC_SERVER
         //   PushReconnectCmd(string.Format("RecordProcess2 {0} {1} {2}", mIsRecordProcess,GetFrameInfo(),content));
#endif
            return;
        }

		if (contentRecorder2 == null)
			CreateCR2();

		contentRecorder2.Append(GetFrameInfo());
		contentRecorder2.Append(content);
		contentRecorder2.Append("\n");
	}

	public void RecordProcess2(string format, params object[] args)
	{
        if (!mIsRecordProcess)
        {
#if !LOGIC_SERVER
          //  PushReconnectCmd(string.Format("RecordProcess2 args {0} {1}", mIsRecordProcess,string.Format(format,args)));
#endif
            return;
        }

		if (contentRecorder2 == null)
			CreateCR2();

		//contentRecorder2.Append(GetFrameInfo());
		contentRecorder2.AppendFormat(format, args);
		contentRecorder2.Append("\n");
	}

	public string GetFrameInfo()
	{
       
#if !SERVER_LOGIC

        if (FrameSync.instance == null)
            return string.Format("T[{0}]F[-]R[{1},{2}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), FrameRandom.callNum, FrameRandom.GetSeed());
        else
        {

            if(FrameSync.instance.svrFrame != FrameSync.instance.endFrame ||
               FrameSync.instance.svrFrame != FrameSync.instance.lastSvrFrame)
            {
                return string.Format("T[{0}]S[{1}-{2}-{3}]F[{4}]R[{5},{6}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), FrameSync.instance.lastSvrFrame, FrameSync.instance.svrFrame, FrameSync.instance.endFrame, FrameSync.instance.curFrame, FrameRandom.callNum, FrameRandom.GetSeed());
            }
            return string.Format("T[{0}]S[{1}]F[{2}]R[{3},{4}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), FrameSync.instance.lastSvrFrame, FrameSync.instance.curFrame, FrameRandom.callNum, FrameRandom.GetSeed());
        }
#else
		 return string.Format("T[{0}]S[{1}]F[{2}]R[{3},{4}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), mLogicServer != null ? mLogicServer.GetSvrFrame(): 9999999999u,FrameRandom.callFrame, FrameRandom.callNum, FrameRandom.GetSeed());
#endif

        return "";
	}
    public static string GetStackTraceModelName()
    {
        //当前堆栈信息
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
        System.Diagnostics.StackFrame[] sfs = st.GetFrames();
        //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
        string _filterdName = "ResponseWrite,ResponseWriteError,";
        string _fullName = string.Empty, _methodName = string.Empty;
        for (int i = 1; i < sfs.Length; ++i)
        {
            //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
            if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset()) break;
            _methodName = sfs[i].GetMethod().Name;//方法名称
            //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0
            if (_filterdName.Contains(_methodName)) continue;
            _fullName = sfs[i].GetMethod().ReflectedType.FullName + "." + _methodName + "()->" + _fullName;
        }
        st = null;
        sfs = null;
        _filterdName = _methodName = null;
        return _fullName.TrimEnd('-', '>');
    }
    public void CopyProcess(ref string dstLogger)
    {
        dstLogger = string.Empty;
        if (contentRecorder != null)
        {
            dstLogger = contentRecorder.ToString();
        }
    }
    public void CopyProcess2(ref string dstLogger)
    {
        dstLogger = string.Empty;
        if (contentRecorder2 != null)
        {
            dstLogger = contentRecorder2.ToString();
        }
    }
    public static void SaveProcessWithProfiler(string sessionID,string logContent, string logContent2)
    {
        if(string.IsNullOrEmpty(sessionID) || logContent == null || logContent2 == null)
        {
            return;
        }
        string folder = RecordData.GetRootPath();
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        string path = folder + "Profiler/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        DateTime Dt = DateTime.Now;
        string strCurTime = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", Dt.Year, Dt.Month, Dt.Day, Dt.Hour, Dt.Minute, Dt.Second);
        string fileName = Path.Combine(path,string.Format("s{0}_{1}_process.txt", sessionID, strCurTime));
        string fileName2 = Path.Combine(path,string.Format("s{0}_{1}_process_receive_package.txt", sessionID, strCurTime));
#if RECORD_FILE
        SaveLogFile(logContent, fileName);
#endif
        SaveLogFile(logContent2, fileName2);
    }
    public static void SaveLogFile(string log,string path)
    {
        if (log == null) return;
        StreamWriter sw = null;
        try
        {
            sw = new StreamWriter(path);
            sw.Write(log);
            sw.Flush();
            sw.Close();
            sw = null;
        }
        catch (System.Exception e)
        {
            Logger.LogErrorFormat("save file failed!!!!!!:{0} reason {1} \n", path, e.ToString());
            if (sw != null)
            {
                sw.Close();
            }
            sw = null;
        }
    }

    private void SaveProcess()
	{
#if LOGIC_SERVER
		DateTime Dt = DateTime.Now;
		string strCurTime = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", Dt.Year, Dt.Month, Dt.Day, Dt.Hour, Dt.Minute, Dt.Second);

		string path = RecordData.GenPath(string.Format("s{2}_{0}_{1}_process.txt", sessionID, type, strCurTime));
#else
		string path = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}process.txt", sessionID, type,mode)));
#endif
#if LOGIC_SERVER
	    if(mIsRecordProcess)
        {
            RecordProcess2(GetStackTraceModelName());
        }
#endif

        if (contentRecorder != null)
		{
#if !LOGIC_SERVER
            string fileName = string.Format("s{0}_{1}_{2}process.txt", sessionID, type, mode);
            string name = string.Format("{0}_{1}", VersionManager.GetInstance() != null ? VersionManager.GetInstance().ClientShortVersion() : 0, fileName);
            string savepath = RecordData.GenPath(name);
            FlushFile(savepath, ref contentRecorder);
#else
            saveToFile(contentRecorder, path);
#endif
            SafeRelease(ref contentRecorder);

            
		}


#if LOGIC_SERVER
		string path2 = RecordData.GenPath(string.Format("s{2}_{0}_{1}_process_receive_package.txt", sessionID, type, strCurTime));
#else
		string path2 = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}process_receive_package.txt", sessionID, type,mode)));
#endif

		if (contentRecorder2 != null)
		{
#if !LOGIC_SERVER
            string fileName = string.Format("s{0}_{1}_{2}process_receive_package.txt", sessionID, type, mode);
            string name = string.Format("{0}_{1}", VersionManager.GetInstance() != null ? VersionManager.GetInstance().ClientShortVersion() : 0, fileName);
            string savepath2 = RecordData.GenPath(name);
            FlushFile(savepath2, ref contentRecorder2);
#else
            saveToFile(contentRecorder2, path2);
#endif
            SafeRelease(ref contentRecorder2);
		}

	}

    /// <summary>
    /// 战斗内 在出现报错的时候保存录像
    /// </summary>
    public void EndRecordInBattleOnError()
    {
        _saveRecordReplay(true);
        SaveProcessInBattle();
    }

    public bool EndRecordInBattle(ref string reason, int type, string desc)
    {
        if (!mIsRecordReplay && !mIsRecordProcess)
        {
            reason = "受限";
            return false;
        }
        SaveSkillBtnClick();
        _saveRecordReplay(true);
        SaveProcessInBattle();
        SaveReconnectCmdInBattle();
        string sessionId = this.sessionID.ToString();
        string timeStamp = Utility.GetCurrentTimeUnix().ToString();
        cacheFilesList.Clear();
        List<string> zipFileList = new List<string>();
        string rootPath = RecordData.GetRootPath();
        if (!Directory.Exists(rootPath))
        {
            Logger.LogErrorFormat("[EndRecordInBattle] rootPath is NotExist {0}", rootPath);
            return false;
        }
        //  SaveReconnectCmd(false);
        string[] files = Directory.GetFiles(rootPath);
        string zipExtName = ".zip";
        string inBattleKeywords = "_InBattle";
        for (int i = 0; i < files.Length; ++i)
        {
            if (!files[i].EndsWith(zipExtName) && files[i].Contains(inBattleKeywords))
            {
                cacheFilesList.Add(files[i]);
            }
        }
        string targetFileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_InBattle_upload.zip", Global.Settings.sdkChannel, ClientApplication.adminServer.id, sessionId, this.type, mode, timeStamp);
        string path = Path.Combine(rootPath, targetFileName);
        if (LibZip.LibZipFileReader.CompressFiles(path, cacheFilesList.ToArray()))
        {
            _DeleteFileList(cacheFilesList);
            cacheFilesList.Clear();
        }
        else
        {
            Logger.LogErrorFormat("[EndRecordInBattle] CompressFiles fail {0}", path);
            reason = "压缩失败";
            _DeleteFileList(cacheFilesList);
            cacheFilesList.Clear();
            return false;
        }
        if (!File.Exists(path))
        {
            string[] zipfiles = Directory.GetFiles(rootPath);
            string keywords = "_InBattle_upload.zip";
            bool bFinded = false;
            for (int i = 0; i < files.Length; ++i)
            {
                string extName = Path.GetExtension(files[i]);
                if (extName.CompareTo(".zip") == 0)
                {
                    string findFileName = Path.GetFileNameWithoutExtension(files[i]);
                    if (findFileName.Contains(keywords))
                    {
                        path = files[i];
                        targetFileName = findFileName;
                        bFinded = true;
                        break;
                    }
                }
            }
            if (!bFinded)
            {
                reason = "找不到文件 或者已经上传过了";
                return false;
            }
        }
        string fileName = Path.GetFileName(path);
        try
        {

            Http.UploadFile(string.Format("http://{0}/replay?file={1}", Global.RECORDLOG_GET_ADDRESS, fileName), path);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("UploadLastFile occur error reason {0}", e.Message);
            reason = "上传失败";
            return false;
        }
        UploadInfo uploadInfo = null;
#if UNITY_IOS || UNITY_IPHONE
        uploadInfo = new UploadInfo
        {
            platform = "IOS",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            replay = targetFileName,
            record_type = type.ToString(),
            version = VersionManager.GetInstance().Version(),
            content = desc
        };
#elif UNITY_ANDROID
          uploadInfo = new UploadInfo
        {
            platform = "Android",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            replay = targetFileName,
            version = VersionManager.GetInstance().Version(),
            record_type = type.ToString(),
            content = desc
        };
#else
        uploadInfo = new UploadInfo
        {
            platform = "PC",
            channel = Global.Settings.sdkChannel.ToString(),
            zone_id = PlayerBaseData.GetInstance().ZoneID.ToString(),
            zone_name = ClientApplication.adminServer.name,
            player_name = PlayerBaseData.GetInstance().Name,
            player_id = PlayerBaseData.GetInstance().RoleID.ToString(),
            replay = targetFileName,
            record_type = type.ToString(),
#if !LOGIC_SERVER
            version = VersionManager.GetInstance().Version(),
#else
            version = "",
#endif
            content = desc
        };
#endif
        if (uploadInfo != null)
        {
            var jsonText = LitJson.JsonMapper.ToJson(uploadInfo);
            try
            {
                Http.SendPostRequest(string.Format("http://{0}/desc", Global.RECORDLOG_POST_ADDRESS), jsonText);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("UploadBattleInFightFile occur error reason {0}", e.Message);
                reason = "输入用户信息失败";
                return false;
            }
        }
        try
        {
            File.Delete(path);
        }
        catch (Exception e)
        {
            return true;
        }
        return true;
    }
    /// <summary>
    /// 战斗中间保存
    /// </summary>
    public void SaveProcessInBattle()
    {
        if (m_markSys != null)
        {
            string srcpath = RecordData.GenPath(BeUtility.Format("{0}.mark", sessionID));
            string dstpath = RecordData.GenPath(BeUtility.Format("{0}_InBattle.mark", sessionID));
            m_markSys.SaveInBattle(srcpath, dstpath, RecordData.buffer);
        }
        string path = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}_InBattle_process.txt", sessionID, type, mode)));
        if (contentRecorder != null)
        {
            //  SaveFileInBattle(contentRecorder, path);
            string curPath = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}process.txt", sessionID, type, mode)));
            FlushFile(curPath, ref contentRecorder);
            CopyFileInBattle(curPath, path);
        }

        string path2 = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}_InBattle_process_receive_package.txt", sessionID, type, mode)));
        if (contentRecorder2 != null)
        {
            //  SaveFileInBattle(contentRecorder2, path2);
            string curPath2 = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}process_receive_package.txt", sessionID, type, mode)));
            FlushFile(curPath2, ref contentRecorder2);
            CopyFileInBattle(curPath2, path2);
        }
    }
    public void SaveRecordReplayInBattle()
    {
        _saveRecordReplay(true);
    }
    private void CopyFileInBattle(string path, string pathInBattle)
    {
        if (File.Exists(path))
        {
            File.Copy(path, pathInBattle, true);
        }
    }
    private void SaveReconnectCmdInBattle()
    {
        if (reconnectRecorder != null)
        {
            string path = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}_InBattle_ReconnectPackage.txt", sessionID, type, mode)));
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(reconnectRecorder.ToString());
                writer.Flush();
                writer.Close();
                fs.Close();
                writer = null;
                fs = null;
#if UNITY_EDITOR
                Logger.LogErrorFormat("SaveReconnectCmdInBattle save file succeed!!!!!!:{0}\n", path);
#endif
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("SaveReconnectCmdInBattle save file failed!!!!!!:{0} reason {1}\n", path,e.ToString());
            }
        }
    }

    private void SaveFileInBattle(StringBuilder contentRecorder, string path)
    {
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryWriter sw = new BinaryWriter(fs);
        sw.Write(contentRecorder.ToString());
        sw.Flush();
        fs.Flush();
        sw.Close();
        fs.Close();
        fs.Dispose();
    }

    private void saveToFile(StringBuilder contentRecorder, string path)
	{
        try 
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryWriter sw = new BinaryWriter(fs);
            sw.Write(contentRecorder.ToString());
            sw.Flush();
            fs.Flush();
            sw.Close();
            fs.Close();
            fs.Dispose();
#if UNITY_EDITOR
            Logger.LogErrorFormat("save file succeed!!!!!!:{0}\n", path);
#endif
        }
        catch (System.Exception e)
        {
            Logger.LogErrorFormat("save file failed!!!!!!:{0} reason {1} \n", path,e.ToString());
        }
	}

    private string _getUploadUrl()
    {
        byte seat     = 0xFF;
        int  allCount = -1;

        if (null != BattleMain.instance)
        {
            IDungeonPlayerDataManager allPlayer = BattleMain.instance.GetPlayerManager();

            if (null != allPlayer)
            {
                BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

                if (null != mainPlayer)
                {
                    seat = mainPlayer.playerInfo.seat;
                }

                allCount = allPlayer.GetAllPlayers().Count;
            }
        }

        return string.Format("http://192.168.2.22:60000/race?session={0}&seat={1}&type={2}&allseat={3}", sessionID, seat, type, allCount);
    }


    private void _uploadHttpFile(string path)
    {
        string fileName = Path.GetFileName(path);

#if MG_TEST
      Http.UploadFile("http://39.108.138.140:59969?file="+fileName,path);
       
#else
      Http.UploadFile("http://39.108.138.140:59965?file="+fileName, path);
#endif
        //Http.UploadFile("http://192.168.2.22:9965?file="+fileName, path);
    }
    public void SaveReconnectCmd()
    {
        if (reconnectRecorder != null)
        {
            string path = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}ReconnectPackage.txt", sessionID, type, mode)));
            if (reconnectWriter == null)
            {
                try
                {
                    reconnectWriter = new StreamWriter(path);
                    reconnectWriter.Write(reconnectRecorder.ToString());
                    reconnectWriter.Flush();
                    reconnectWriter.Close();
                    reconnectWriter = null;

#if UNITY_EDITOR
                    Logger.LogErrorFormat("SaveReconnectCmd save file succeed!!!!!!:{0}\n", path);
#endif
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("SaveReconnectCmd save file failed!!!!!!:{0} Reason {1}\n", path, e.Message);
                    if (reconnectWriter != null)
                    {
                        reconnectWriter.Close();
                    }
                    reconnectWriter = null;
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                    fileStream = null;
                }
                SafeRelease(ref reconnectRecorder);
            }
            else
            {
                try
                {
                    reconnectWriter.Write(reconnectRecorder.ToString());
                    reconnectWriter.Flush();
                    fileStream.Flush();
                    reconnectWriter.Close();
                    fileStream.Close();
                    fileStream.Dispose();
                    reconnectWriter = null;
                    fileStream = null;
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("SaveReconnectCmd2 save file failed!!!!!!:{0} Reason {1}\n", path,e.Message);
                    if (reconnectWriter != null)
                    {
                        reconnectWriter.Close();
                    }
                    reconnectWriter = null;
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                    fileStream = null;
                }
                SafeRelease(ref reconnectRecorder);
            }
        }
    }
    StreamWriter reconnectWriter = null;
    FileStream fileStream = null;
    [Conditional("MG_TEST")]
    public void PushReconnectCmd(string cmdStr)
    {
        if (!mIsRecordProcess) return;
        if (!mIsRecording) return;
        if (cmdStr.Equals(string.Empty)) return;
        if (reconnectRecorder == null)
        {
            reconnectRecorder = StringBuilderCache.Acquire(16 * 1024);
            reconnectRecorder.AppendFormat("sessionID:{0} \n", sessionID);
        }
        reconnectRecorder.Append(string.Format("[{0}]",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")));
        reconnectRecorder.Append(cmdStr);
        reconnectRecorder.Append("\n");
        if (reconnectRecorder.Length > 100000)
        {
            string path = RecordData.GenPath(RecordData.GenFileName(string.Format("s{0}_{1}_{2}ReconnectPackage.txt", sessionID, type, mode)));
            if (reconnectWriter == null)
            {
                try
                {
                    fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    reconnectWriter = new StreamWriter(fileStream);
                    reconnectWriter.Write(reconnectRecorder.ToString());
                    reconnectWriter.Flush();
#if UNITY_EDITOR
                    Logger.LogErrorFormat("PushReconnectCmd save file succeed!!!!!!:{0}\n", path);
#endif
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("PushReconnectCmd save file failed!!!!!!:{0} reason {1}\n ", path,e.ToString());
                    if (reconnectWriter != null)
                    {
                        reconnectWriter.Close();
                    }
                    reconnectWriter = null;
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                    fileStream = null;
                }
                SafeRelease(ref reconnectRecorder);
            }
            else
            {
                try
                {
                    reconnectWriter.Write(reconnectRecorder.ToString());
                    reconnectWriter.Flush();
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("PushReconnectCmd2 save file failed!!!!!!:{0} reason {1}\n", path, e.ToString());
                    if (reconnectWriter != null)
                    {
                        reconnectWriter.Close();
                    }
                    reconnectWriter = null;
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                    fileStream = null;
                }
                SafeRelease(ref reconnectRecorder);
            }
        }
    }
}
