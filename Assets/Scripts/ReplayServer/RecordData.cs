using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Protocol;

public class RecordFrameData
{
    public Frame[] serverFrames;
    public int tickTime;

	public RecordFrameData(Frame[] frames, int tick)
	{
		serverFrames = frames;
		tickTime = tick;
	}
}

public class ChasingData
{
    public uint startFrame;
    public uint endFrame;
}

public class RecordData
{
	public static int BUFF_SIZE = 256 * 1024;
	public static byte[] buffer = new byte[BUFF_SIZE];//256k
	public static string RECORD_FILE_NAME = "record.bin";

	public string sessionID;
	public uint startTime;
	public uint clientVersion;
	public int pvpDungeonID;
	public int duration = 30;
	public WorldNotifyRaceStart playerInfo;
	public SceneDungeonStartRes	dungeonInfo;
    public RaceType raceType = RaceType.Dungeon;
	public uint startFrame;
	public List<RecordFrameData> frameInfo = new List<RecordFrameData>();
    public List<ChasingData> chasingInfo = new List<ChasingData>();//追帧用的数据
    public SceneMatchPkRaceEnd resultInfo;
	public RelaySvrEndGameReq endReq;

	public ReplayFile replayFile;

	public RecordData()
	{
		replayFile = new ReplayFile();
	}
    public void CopyData(RecordData data)
    {
        data.sessionID = sessionID;
        data.startTime = startTime;
        data.clientVersion = clientVersion;
        data.pvpDungeonID = pvpDungeonID;
        data.playerInfo = playerInfo;
        data.dungeonInfo = dungeonInfo;
        data.raceType = raceType;
        data.startFrame = startFrame;
        data.frameInfo = frameInfo;
        data.resultInfo = resultInfo;
        data.endReq = endReq;
        data.duration = duration;
        data.replayFile = replayFile;
    }
    public void PushChasingStartFrame(uint frame)
    {
        ChasingData data = new ChasingData
        {
            startFrame = frame,
            endFrame = 0
        };
        chasingInfo.Add(data);
    }
    public void PushChasingEndFrame(uint frame)
    {
        if (chasingInfo.Count == 0)
        {
            Logger.LogErrorFormat("Unreasonable Push End Frame {0}", frame);
            return;
        }
        var curInfo = chasingInfo[chasingInfo.Count - 1];
        if (curInfo.startFrame > frame)
        {
            Logger.LogErrorFormat("Unreasonable Push End Frame Compare to Start {0} {1}", curInfo.startFrame, frame);
            return;
        }
        curInfo.endFrame = frame;
    }
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(sessionID))
        {
            return false;
        }
        return true;
    }
    public void SerializationWithProfile()
    {
        if (!IsValid()) return;
        replayFile.header.version = clientVersion;
        replayFile.header.startTime = startTime;
        replayFile.header.sessionId = Convert.ToUInt64(sessionID);

        if (dungeonInfo != null)
        {
            replayFile.header.raceType = 255;//resultInfo.pkType;	
        }
        else if (raceType == RaceType.PK_3V3 || raceType == RaceType.ScoreWar || raceType == RaceType.PK_3V3_Melee)
        {
            replayFile.header.raceType = (byte)raceType;
        }
        else
        {
            replayFile.header.raceType = (byte)raceType;
        }

        replayFile.header.pkDungeonId = (uint)pvpDungeonID;
        replayFile.header.players = playerInfo == null ? new RacePlayerInfo[0] : playerInfo.players;
        replayFile.header.duration = (uint)duration;

        int count = 0;
        for (int i = 0; i < frameInfo.Count; ++i)
        {
            var frameData = frameInfo[i];
            count += frameData.serverFrames.Length;
        }

        replayFile.frames = new Frame[count];
        int index = 0;
        for (int i = 0; i < frameInfo.Count; ++i)
        {
            var frameData = frameInfo[i].serverFrames;
            for (int j = 0; j < frameData.Length; ++j)
            {
                replayFile.frames[index++] = frameData[j];
            }
        }

        replayFile.results = new ReplayRaceResult[2];
        for (int i = 0; i < 2; ++i)
        {
            ReplayRaceResult result = new ReplayRaceResult();

            if (endReq != null)
            {
                result.pos = endReq.end.infoes[i].pos;
                result.result = endReq.end.infoes[i].result;
                replayFile.results[i] = result;
            }
            else
            {
                result.result = 1;
                result.pos = (byte)i;
                replayFile.results[i] = result;
            }

        }
        if (replayFile.frames.Length > int.MaxValue)
        {
            Logger.LogErrorFormat("RecordData Serialization sessionid : {0} ,dungeonId : {1},raceType :{2} clientversion :{3} replay frameLength : {4} is out of MaxRange",
                replayFile.header.sessionId,
                replayFile.header.pkDungeonId,
                replayFile.header.raceType,
                replayFile.header.version,
                replayFile.frames.Length);
        }
        int pos = 0;
        try
        {
            string folder = GetRootPath();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string path = folder + "Profiler/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var Dt = DateTime.Now;
            string strCurTime = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", Dt.Year, Dt.Month, Dt.Day, Dt.Hour, Dt.Minute, Dt.Second);
            string fileName = string.Format("{0}{1}_{2}",path,sessionID, strCurTime);
            MapViewStream st = new MapViewStream(fileName, buffer, MapViewStream.FileAccessMode.Write);
            replayFile.encode(st, ref pos);

            if (dungeonInfo != null)
            {
                dungeonInfo.encode(st, ref pos);
            }
            st.Save();
        }
        catch (System.Exception e)
        {
            Logger.LogErrorFormat("[RecordData] save file failed Exception {0}", e.Message);
        }
    }

    public void Serialization(bool isInBattle = false)
	{
		Logger.LogForReplay("[RECORD]Start Serialization");


		replayFile.header.version = clientVersion;
		replayFile.header.startTime = startTime;
		replayFile.header.sessionId = Convert.ToUInt64(sessionID);

		if(dungeonInfo != null)
		{
			replayFile.header.raceType = 255;//resultInfo.pkType;	
		}
        else if(raceType == RaceType.PK_3V3 || raceType == RaceType.ScoreWar || raceType == RaceType.PK_3V3_Melee)
        {
            replayFile.header.raceType = (byte)raceType;
        }
        else
        {
            replayFile.header.raceType = (byte)raceType;
        }
		
		replayFile.header.pkDungeonId = (uint)pvpDungeonID;
		replayFile.header.players = playerInfo == null ? new RacePlayerInfo[0] : playerInfo.players;
		replayFile.header.duration = (uint)duration;

		int count = 0;
		for(int i=0; i<frameInfo.Count; ++i)
		{
			var frameData = frameInfo[i];
            count += frameData.serverFrames.Length;
		}

		replayFile.frames = new Frame[count];
		int index = 0;
		for(int i=0; i<frameInfo.Count; ++i)
		{
			var frameData = frameInfo[i].serverFrames;
			for(int j=0; j<frameData.Length; ++j)
			{
				replayFile.frames[index++] = frameData[j];
			}
		}

		replayFile.results = new ReplayRaceResult[2];
		for(int i=0; i<2; ++i)
		{
			ReplayRaceResult result = new ReplayRaceResult();

			if (endReq != null) {
				result.pos = endReq.end.infoes [i].pos;
				result.result = endReq.end.infoes [i].result;
				replayFile.results [i] = result;
			}
			else {
				result.result = 1;
				result.pos = (byte)i;
				replayFile.results [i] = result;
			}

		}
        if (replayFile.frames.Length > int.MaxValue)
        {
            Logger.LogErrorFormat("RecordData Serialization sessionid : {0} ,dungeonId : {1},raceType :{2} clientversion :{3} replay frameLength : {4} is out of MaxRange",
                replayFile.header.sessionId,
                replayFile.header.pkDungeonId,
                replayFile.header.raceType,
                replayFile.header.version,
                replayFile.frames.Length);
        }
        int pos = 0;
        //   replayFile.encode(buffer, ref pos);
        string path = isInBattle ? GenPath(sessionID + "_InBattle") : GenPath(sessionID);
        try
        {
            MapViewStream st = new MapViewStream(path, buffer, MapViewStream.FileAccessMode.Write);
            replayFile.encode(st, ref pos);

            if (dungeonInfo != null)
            {
                dungeonInfo.encode(st, ref pos);
            }
            //保存追帧时的数据
            BaseDLL.encode_uint32(st, ref pos, (uint)chasingInfo.Count);
            for (int i = 0; i < chasingInfo.Count; i++)
            {
                var curInfo = chasingInfo[i];
                BaseDLL.encode_uint32(st, ref pos, curInfo.startFrame);
                BaseDLL.encode_uint32(st, ref pos, curInfo.endFrame);
            }
            st.Save();
        }
        catch (System.Exception e)
        {
            Logger.LogErrorFormat("[RecordData] save file failed {0},Exception {1}", path, e.Message);
        }
       
        //pos = 0;
        //replayFile.encode(buffer, ref pos);
        //if (dungeonInfo != null)
        //{
        //    dungeonInfo.encode(buffer, ref pos);
        //}

        //SaveReplayFile(sessionID, buffer, pos,"extra");

        Logger.LogForReplay("[RECORD]End Serialization {0} pos:{1}", sessionID, pos);
	}

	public void DeSerialiaction(string path)
	{
		

		FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
		BinaryReader br = new BinaryReader(fs);
		var buff = br.ReadBytes((int)fs.Length);

		Logger.LogForReplay("[RECORD]Start DeSerialiaction:{0} buffLen:{1}", path, buff.Length);

		int pos = 0;
		replayFile.decode(buff,ref pos);

		if(replayFile.header.raceType == 255)
		{
			dungeonInfo = new SceneDungeonStartRes();
			dungeonInfo.decode(buff,ref pos);
		}
        //读取追帧数据
        if (fs.Length > pos)
        {
            uint count = 0;
            BaseDLL.decode_uint32(buff, ref pos, ref count);
            for (int i = 0; i < count; i++)
            {
                var curInfo = new ChasingData();
                uint startFrame = 0;
                uint endFrame = 0;
                BaseDLL.decode_uint32(buff, ref pos, ref startFrame);
                BaseDLL.decode_uint32(buff, ref pos, ref endFrame);
                curInfo.startFrame = startFrame;
                curInfo.endFrame = endFrame;
                chasingInfo.Add(curInfo);
            }
        }

        br.Close();
		fs.Close();

		clientVersion = replayFile.header.version;
		startTime = replayFile.header.startTime;
		sessionID = replayFile.header.sessionId.ToString();
		pvpDungeonID = (int)replayFile.header.pkDungeonId;
		duration = (int)replayFile.header.duration;

		Logger.LogForReplay("[RECORD]End DeSerialiaction");
	}

	public static string GenFileName(string subname=null)
	{		
		DateTime Dt = DateTime.Now;
		string strCurTime = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", Dt.Year, Dt.Month, Dt.Day, Dt.Hour, Dt.Minute, Dt.Second);
	
		string name = "";
		if (subname == null)
			name = string.Format("{3}", strCurTime, GameClient.PlayerBaseData.GetInstance().RoleID, VersionManager.GetInstance().ServerVersion(), subname==null?RECORD_FILE_NAME:subname);
		else
			name = string.Format("{0}_{1}_{2}_{3}", strCurTime, GameClient.PlayerBaseData.GetInstance().RoleID, VersionManager.GetInstance().ServerVersion(), subname==null?RECORD_FILE_NAME:subname);

		return name;
	}

    public static string GetRootPath()
    {
		#if LOGIC_SERVER
		string folder = "./RecordLog/";
		#elif UNITY_ANDROID || UNITY_IOS
        string folder = Utility.GetWriteablePath() + "RecordLog/";
		#elif !UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
		string folder = "./RecordLog/";
		#else 
		string folder = Utility.GetWriteablePath() + "../../RecordLog/";
		#endif

        return folder;
    }

	public static string GenPath(string fileName)
	{
        string folder = GetRootPath();
		if (!Directory.Exists(folder))
			Directory.CreateDirectory(folder);
		return folder + fileName;
	}

	public static void SaveReplayFile(string sessionID, byte[] contents, int count, string format="")
	{
        string path = GenPath(sessionID+format/*GenFileName()*/);

        try 
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryWriter sw = new BinaryWriter(fs);

            sw.Write(contents, 0, count);

            sw.Flush();
            sw.Close();

            fs.Close();
        }
        catch (System.Exception e)
        {
            Logger.LogErrorFormat("[RecordData] save file failed {0}", path);
        }
	}
}
