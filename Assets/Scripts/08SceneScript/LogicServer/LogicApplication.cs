using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using tm.tool;

public class LogicApplication : Singleton<LogicApplication>
{
    static Dictionary<UInt64, LogicServer> m_Logics = new Dictionary<ulong, LogicServer>();
#if MG_TEST && LOGIC_SERVER
    static Dictionary<UInt64, UInt32> m_waitDesposeLogics = new Dictionary<ulong, uint>();
    static KeyValuePair<UInt64, double>[] m_maxConsumeTimeLogics = new KeyValuePair<UInt64, double>[5];      //当前活跃战斗update 时间消耗 排名
    static KeyValuePair<UInt64, double>[] m_maxInitConsumeTimeLogics = new KeyValuePair<UInt64, double>[5]; //当前活跃战斗初始化 时间消耗 排名
    static KeyValuePair<UInt64, double>[] m_maxDurationTimeLogics = new KeyValuePair<UInt64, double>[5];    //当前活跃战斗存活时间最长  排名
    static KeyValuePair<UInt64, double>[] m_curMaxUpdateTimeLogics = new KeyValuePair<UInt64, double>[5];   //当前活跃战斗最大update时间消耗 排名
    static KeyValuePair<UInt64,double>[] m_durationTopList = new KeyValuePair<UInt64, double>[5];           //历史战斗存活时间最长 排名
    static Dictionary<int,int>  m_curAddBattle = new Dictionary<int,int>();
    static Dictionary<int,int>  m_curDecBattle = new Dictionary<int,int>();
    static Dictionary<int,int>  m_prevAddBattle = null;
    static Dictionary<int,int>  m_prevDecBattle = null;
    const int capacity = 5; //默认大于2！！！

    //历史战斗最大update时间消耗
    static UInt64[] m_maxUpdateSessionId = new UInt64[capacity];
    static double[] m_maxUpdateTime = new double[capacity];
    static int[] m_maxUpdateFrame = new int[capacity];

    //历史战斗存活时间最长时间消耗 
    static UInt64[] m_maxDurationSessionId = new UInt64[capacity];
    static double[] m_maxDurationTime = new double[capacity];

    //历史战斗初始化时间最大时间消耗
    static UInt64[] m_maxInitSessionId = new UInt64[capacity];
    static double[] m_maxInitTime = new double[capacity];

    static bool m_DirtyInitConsume = false;
    static double m_timeStamp = 0;
    static double averageUpdateTime = 0;
    static double curUpdateTime = 0;
    static double maxUpdateTime = 0;
    static int updateCount = 0;
    static long curMem = 0;
    static readonly object disposelocker = new object();
    static int m_MainThreadID = ~0;
    public static RecordData[] longestRecordData = new RecordData[capacity*3];
    public static string[] longestLogBuilder = new string[capacity*3];
    public static string[] longestLogBuilder2 = new string[capacity*3];
    static double timeSpan = 0;
#endif

    // Use this for initialization
    static int WRITE_TAG = 1;
    static int CHECK_TAG = 2;
    static UInt64 curSessionID = 0;
    static string zeroStr = 0.ToString();
    static int curSessionId = -1;

    static void GuardThread()
    {
        while (true)
        {
            Thread.Sleep(2000);
            string switchOn = string.Empty;
            string filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "guard.cfg");

            if (System.IO.File.Exists(filePath))
            {
                switchOn = System.IO.File.ReadAllText(filePath);
            }
            else
            {
                Logger.LogErrorFormat("filePath not found {0}", filePath);
            }
            int option = 0;
            int.TryParse(switchOn, out option);
            if (option == WRITE_TAG)
            {
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.WriteAllText(filePath, zeroStr);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat("WRITE_TAG Save Guard Log {0} is failed reason {1}", filePath, e.ToString());
                }
                if (m_Logics.ContainsKey(curSessionID))
                {
                    var curBattle = m_Logics[curSessionID];
                    if (curBattle != null)
                    {
                        if (curBattle.isEnd())
                        {
                            Logger.LogErrorFormat("curSessionID is End {0}", curSessionID);
                        }
                        else
                        {
                            if (curBattle.recordServer != null)
                            {
                                try
                                {
                                    curBattle.recordServer.SaveProcessInBattle();
                                    curBattle.recordServer.SaveRecordReplayInBattle();
                                    Logger.LogErrorFormat("curSessionID {0} is Success", curSessionID);
                                }
                                catch (Exception e)
                                {
                                    Logger.LogErrorFormat("curSessionID {0} is Exception :{1}", curSessionID, e.ToString());
                                }
                            }
                        }
                    }
                }
            }
            else if (option == CHECK_TAG)
            {
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.WriteAllText(filePath, zeroStr);
                    }
                    Logger.LogErrorFormat("curSessionID is {0}", curSessionID);
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat("CHECK_TAG Save Guard Log {0} is failed reason {1}", filePath, e.ToString());
                }
            }
        }
    }
    new static public bool Init()
    {
#if MG_TEST && LOGIC_SERVER
        Thread t = new Thread(GuardThread);
        t.Start();
#endif
        try
        {
#if MG_TEST && LOGIC_SERVER
            m_MainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            for (int i = 0; i < longestRecordData.Length; i++)
            {
                if (longestRecordData[i] == null)
                    longestRecordData[i] = new RecordData();
            }
            for(int i = 0; i < longestLogBuilder.Length;i++)
            {
                longestLogBuilder[i] = string.Empty;
            }
            for (int i = 0; i < longestLogBuilder2.Length; i++)
            {
                longestLogBuilder2[i] = string.Empty;
            }
            timeSpan = Utility.GetTimeStamp();
#endif
            LogicServer.LogicServerInit();
            return true;
        }
        catch (Exception e)
        {
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, e.ToString());
            return false;
        }
    }
    static public void ShowConsumeTime()
    {
#if MG_TEST && LOGIC_SERVER
        Logger.LogError("------------------华丽的开始分割线------------------------------------------");
        for (int i = 0; i < m_maxConsumeTimeLogics.Length;i++)
        {
            var timeinfo = m_maxConsumeTimeLogics[i];
            if(timeinfo.Key > 0)
                Logger.LogErrorFormat(string.Format("[Peak Update ConsumeTime RankList] No{0} sessionId : {1} time {2}", i,timeinfo.Key,timeinfo.Value));
        }

        for (int i = 0; i < m_curMaxUpdateTimeLogics.Length; i++)
        {
            var timeinfo = m_curMaxUpdateTimeLogics[i];
            if(timeinfo.Key > 0)
                Logger.LogErrorFormat(string.Format("[Current Update ConsumeTime RankList] No{0} sessionId : {1} time {2}", i, timeinfo.Key, timeinfo.Value));
        }
            
        for (int i = 0; i < m_maxInitConsumeTimeLogics.Length;i++)
        {
            var timeinfo = m_maxInitConsumeTimeLogics[i];
            if(timeinfo.Key > 0)   
                Logger.LogErrorFormat(string.Format("[Init ConsumeTime RankList] No{0} sessionId : {1} time {2}", i, timeinfo.Key,timeinfo.Value));
        }

        for(int i = 0; i < m_maxDurationTimeLogics.Length;i++)
        {
            var timeinfo = m_maxDurationTimeLogics[i];
            if(timeinfo.Key > 0)
                Logger.LogErrorFormat(string.Format("[Duration ConsumeTime RankList] No{0} sessionId : {1} time {2}", i, timeinfo.Key, timeinfo.Value));
        }
        for (int i = 0; i < m_durationTopList.Length; i++)
        {
            var timeinfo = m_durationTopList[i];
            if(timeinfo.Key > 0)
                Logger.LogErrorFormat(string.Format("[Duration ConsumeTime TopList] No{0} sessionId : {1} time {2}", i, timeinfo.Key, timeinfo.Value));
        }

        Logger.LogErrorFormat(string.Format("[Update Total ConsumeTime] curTime {0} averageTime {1} peakTime {2}", curUpdateTime, averageUpdateTime / updateCount, maxUpdateTime));
        for(int i =0;i< capacity; ++i)
        {
            if(m_maxUpdateSessionId[i] > 0 ||  m_maxDurationSessionId[i] > 0 || m_maxInitSessionId[i] > 0)
            {    
                Logger.LogErrorFormat("[Session ConsumeTime Detail]: MaxUpdate: SessionId {0} ConsumeTime {1} Frame {2}\r\n MaxDuration: SessionId {3}  ConsumeTime {4} \r\n MaxInit: SessionId {5} ConsumeTime {6}",
                                                     m_maxUpdateSessionId[i], m_maxUpdateTime[i], m_maxUpdateFrame[i],
                                                     m_maxDurationSessionId[i], m_maxDurationTime[i],
                                                     m_maxInitSessionId[i], m_maxInitTime[i]);
            }
        }

        long mem = 0;//GC.GetTotalMemory(false);
        Logger.LogErrorFormat("[MemInfo] : prevMem {0} curMem {1} usedMem {2} raceCount {3}", curMem, mem, mem - curMem, m_Logics.Count);
        curMem = mem;
         Logger.LogError("------------------华丽的结束分割线------------------------------------------");
#endif
    }
    static public int LogicCount
    {
        get { return m_Logics.Count; }
    }

    static public void DeInit()
    {
        m_Logics.Clear();
    }
    static public void ExceptionCallBack()
    {
#if MG_TEST && LOGIC_SERVER
        try
        {
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("[ExceptionCallBack]: curSessionID {0} Saving....", curSessionID));
            bool bFind = false;
            if (m_Logics.ContainsKey(curSessionID))
            {
                var logic = m_Logics[curSessionID];
                if (logic != null)
                {
                    BeUtility.SaveBattleRecord(logic.GetBattle());
                    bFind = true;
                }
            }
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("[ExceptionCallBack] : curSessionID {0} Saved {1}.", curSessionID,bFind));
        }
        catch(Exception e)
        {
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("[ExceptionCallBack] : curSessionID {0} occur Error.", curSessionID));
        }
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("[ExceptionCallBack] : curSessionID {0} occur Error.", curSessionID));
#endif
    }
    // Update is called once per frame
    static public void Update(int delta)
    {
#if MG_TEST && LOGIC_SERVER
        var timeStamp = Utility.GetTimeStamp();
#endif
        List<UInt64> needDelLogics = new List<ulong>();
        foreach (var itr in m_Logics)
        {
            var logic = itr.Value;
            curSessionID = itr.Key;

            try
            {
                logic.Update(delta);
#if MG_TEST && LOGIC_SERVER
                int minTimeIndex = 0;
                int sameSessionIndex = -1;
                for (int i = 0;i< m_maxUpdateTime.Length; ++i)
                {
                    if(m_maxUpdateSessionId[i] == logic.GetSession())
                    {
                        sameSessionIndex = i;
                        break;
                    }
                    if(m_maxUpdateTime[minTimeIndex] > m_maxUpdateTime[i])
                    {
                        minTimeIndex = i;
                    }
                }
                if(m_maxUpdateTime[minTimeIndex] < logic.maxConsumeTime && sameSessionIndex == -1)//如果存在相同的sessionid就不做要随意更新了
                {
                    m_maxUpdateTime[minTimeIndex] = logic.maxConsumeTime;
                    m_maxUpdateSessionId[minTimeIndex] = logic.GetSession();
                    m_maxUpdateFrame[minTimeIndex] = logic.maxUpdateFrame;
                }
                else if(sameSessionIndex != -1)//这里可能存在问题，默认每个logic的最大时长都比之前大，所以才能直接覆盖，下面的两次遍历同理
                {
                    m_maxUpdateTime[sameSessionIndex] = logic.maxConsumeTime;
                    m_maxUpdateSessionId[sameSessionIndex] = logic.GetSession();
                    m_maxUpdateFrame[sameSessionIndex] = logic.maxUpdateFrame;
                }
                SortListWithFindOrInsert(m_maxConsumeTimeLogics, logic.maxConsumeTime, logic.GetSession());
                SortListWithFindOrInsert(m_curMaxUpdateTimeLogics, logic.curConsumeTime, logic.GetSession());
                var durTime = timeStamp - logic.startTimeStamp;
                minTimeIndex = 0;
                sameSessionIndex = -1;
                for(int i = 0; i < m_maxDurationTime.Length; ++i)
                {
                    if(m_maxDurationSessionId[i] == logic.GetSession())
                    {
                        sameSessionIndex = i;
                        break;
                    }
                    if(m_maxDurationTime[minTimeIndex] > m_maxDurationTime[i])
                    {
                        minTimeIndex = i;
                    }
                }
                if (m_maxDurationTime[minTimeIndex] < durTime && sameSessionIndex == -1) 
                {
                    m_maxDurationTime[minTimeIndex] = durTime;
                    m_maxDurationSessionId[minTimeIndex] = logic.GetSession();
                }
                else if(sameSessionIndex != -1)
                {
                    m_maxDurationTime[sameSessionIndex] = durTime;
                    m_maxDurationSessionId[sameSessionIndex] = logic.GetSession();
                }

                minTimeIndex = 0;
                sameSessionIndex = -1;
                for (int i = 0; i < m_maxInitTime.Length; ++i) 
                {
                    if(m_maxInitSessionId[i] == logic.GetSession())
                    {
                        sameSessionIndex = -1;
                        break;
                    }
                    if (m_maxInitTime[minTimeIndex] > m_maxInitTime[i])
                    {
                        minTimeIndex = i;
                    }
                }
                if (m_maxInitTime[minTimeIndex] < logic.curInitConsumeTime && sameSessionIndex == -1) 
                {
                    m_maxInitTime[minTimeIndex] = logic.curInitConsumeTime;
                    m_maxInitSessionId[minTimeIndex] = logic.GetSession();
                }
                else if(sameSessionIndex != -1)
                {
                    m_maxInitTime[sameSessionIndex] = logic.curInitConsumeTime;
                    m_maxInitSessionId[sameSessionIndex] = logic.GetSession();
                }

                SortListWithFindOrInsert(m_maxDurationTimeLogics, durTime, logic.GetSession());
                SortListWithFindOrInsert(m_durationTopList, durTime, logic.GetSession());
#endif
                if (logic.isEnd())
                {
                    Logger.LogErrorFormat("race:{0} is end...", logic.GetSession());
#if LOGIC_SERVER_TEST
                    needDelLogics.Add(itr.Key);
#else
                    needDelLogics.Add(logic.GetSession());
#endif
                }
            }
            catch (Exception e)
            {
#if !LOGIC_SERVER_TEST
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, e.ToString());
#else
                Logger.LogErrorFormat(e.ToString());
#endif
                // 验证服务器 添加 异常的记录
                if (null != logic && null != logic.recordServer)
                {
                    logic.recordServer.RecordProcess(e.ToString());
                }
            }
        }


        foreach(var id in needDelLogics)
        {
            DelLogicInstance(id);
        }
#if MG_TEST && LOGIC_SERVER
        if(m_DirtyInitConsume)
        {
            SortList(m_maxInitConsumeTimeLogics);
        }
        m_DirtyInitConsume = false;

        ShowLongWaitDisposeInfo();
#endif
        unsyncChecker.UpdateUnsycnNode(delta / 1000.0f);
       
#if MG_TEST && LOGIC_SERVER
        var curTime = Utility.GetTimeStamp();
        var deltaTime = curTime - timeStamp;
        curUpdateTime = deltaTime;
        averageUpdateTime += curUpdateTime;
        if (curUpdateTime > maxUpdateTime)
        {
            maxUpdateTime = curUpdateTime;
        }
        updateCount++;
        if (curTime - m_timeStamp >= 5)
        {
            m_timeStamp = curTime;
            ShowConsumeTime();
            averageUpdateTime = 0;
            updateCount = 0;
        }

        if (curTime - timeSpan > 3600)
        {
            timeSpan = curTime;
            SerilizeRecordData();
        }
#endif
    }
    private static tm.tool.UnsyncChecker unsyncChecker = new UnsyncChecker();
    
#if MG_TEST && LOGIC_SERVER
    private static void SerilizeRecordData()
    {
        for (int k = 0; k < capacity; ++k)
        {
            if (m_Logics.ContainsKey(m_maxUpdateSessionId[k]))
            {
                var logic = m_Logics[m_maxUpdateSessionId[k]];
                SaveRecordData(logic, longestRecordData[0 + 3 * k]);
                if (logic != null && logic.recordServer != null)
                {
                    logic.recordServer.CopyProcess(ref longestLogBuilder[0 + 3 * k]);
                    logic.recordServer.CopyProcess2(ref longestLogBuilder2[0 + 3 * k]);
                }
            }
            if (m_Logics.ContainsKey(m_maxDurationSessionId[k]))
            {
                var logic = m_Logics[m_maxDurationSessionId[k]];
                SaveRecordData(m_Logics[m_maxDurationSessionId[k]], longestRecordData[1 + 3 * k]);
                if (logic != null && logic.recordServer != null)
                {
                    logic.recordServer.CopyProcess(ref longestLogBuilder[1 + 3 * k]);
                    logic.recordServer.CopyProcess2(ref longestLogBuilder2[1 + 3 * k]);
                }
            }
            if (m_Logics.ContainsKey(m_maxInitSessionId[k]))
            {
                var logic = m_Logics[m_maxInitSessionId[k]];
                SaveRecordData(m_Logics[m_maxInitSessionId[k]], longestRecordData[2 + 3 * k]);
                if (logic != null && logic.recordServer != null)
                {
                    logic.recordServer.CopyProcess(ref longestLogBuilder[2 + 3 * k]);
                    logic.recordServer.CopyProcess2(ref longestLogBuilder2[2 + 3 * k]);
                }
            }
        }
        try
        {
            for (int i = 0; i < longestRecordData.Length; i++)
            {
                if (longestRecordData[i] != null)
                {
                    longestRecordData[i].SerializationWithProfile();
                }
            }
            for (int i = 0; i < longestLogBuilder.Length && i < longestLogBuilder2.Length && i < longestRecordData.Length; i++)
            {
                RecordServer.SaveProcessWithProfiler(longestRecordData[i].sessionID, longestLogBuilder[i], longestLogBuilder2[i]);
            }
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("SaveRecordData occur error reason {0}:", e.ToString());
        }
        System.IO.FileStream fs = null;
        System.IO.StreamWriter sw = null;
        try
        {
            string folder = RecordData.GetRootPath();
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);
            string path = folder + "Profiler/";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            string fileName = System.IO.Path.Combine(path, "WriteFileLog.txt");
            if (!System.IO.File.Exists(fileName))
            {
                fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            }
            else
            {
                fs = new System.IO.FileStream(fileName, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            }
            sw = new System.IO.StreamWriter(fs);
            sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
            for (int k = 0; k < capacity; ++k)
            {
                if(m_maxUpdateSessionId[k] > 0)
                    sw.WriteLine(string.Format("[{0}]index:{4}[MaxUpdate session] {1} consumeTime {2} Frame {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), m_maxUpdateSessionId[k], m_maxUpdateTime[k], m_maxUpdateFrame[k], k));
                if(m_maxDurationSessionId[k] > 0)
                    sw.WriteLine(string.Format("[{0}]index:{3}[MaxDuration session] {1} consumeTime {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), m_maxDurationSessionId[k], m_maxDurationTime[k], k));
                if(m_maxInitSessionId[k] > 0)
                    sw.WriteLine(string.Format("[{0}]index:{3}[MaxInit session] {1} consumeTime {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), m_maxInitSessionId[k], m_maxInitTime[k], k));
                sw.WriteLine("");
            }
            ShowBattleStatistic(sw);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        catch (Exception e)
        {
            if (sw != null)
            {
                sw.Close();
            }
            if (fs != null)
            {
                fs.Close();
            }
            Logger.LogErrorFormat("Write Log occur error reason {0}:", e.ToString());
        }
    }
#endif
#if MG_TEST && LOGIC_SERVER
    public static void SaveRecordData(LogicServer logic,RecordData dstData)
    {
        if (logic != null && logic.recordServer != null && logic.recordServer.GetRecordData() != null && dstData != null)
        {
            logic.recordServer.GetRecordData().CopyData(dstData);
        }
        else if(dstData != null)
        {
            dstData.sessionID = string.Empty;
        }


    }
    public static void TrySaveRecordData(LogicServer logic)
    {
        if (logic == null) return;
        for(int i = 0; i < capacity; ++i)
        {
            if (logic.GetSession() == m_maxUpdateSessionId[i])
            {
                SaveRecordData(logic, longestRecordData[0]);
                if (logic.recordServer != null)
                {
                    logic.recordServer.CopyProcess(ref longestLogBuilder[0]);
                    logic.recordServer.CopyProcess2(ref longestLogBuilder2[0]);
                }
            }
            else if (logic.GetSession() == m_maxDurationSessionId[i])
            {
                SaveRecordData(logic, longestRecordData[1]);
                if (logic.recordServer != null)
                {
                    logic.recordServer.CopyProcess(ref longestLogBuilder[1]);
                    logic.recordServer.CopyProcess2(ref longestLogBuilder2[1]);
                }
            }
            else if (logic.GetSession() == m_maxInitSessionId[i])
            {
                SaveRecordData(logic, longestRecordData[2]);
                if (logic.recordServer != null)
                {
                    logic.recordServer.CopyProcess(ref longestLogBuilder[2]);
                    logic.recordServer.CopyProcess2(ref longestLogBuilder2[2]);
                }
            }
        }
    }
#endif
    static public bool StartPVE(ulong s, System.IntPtr buff, int bufflen)
    {
        try
        {
            var logic = FindLogicInstance(s);
            if (logic != null)
            {
#if !LOGIC_SERVER_TEST
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("race({0}) start pve failed, already has this race.", s));
#else
                Logger.LogErrorFormat("race({0}) start pve failed, already has this race.", s);
#endif
                return false;
            }
#if !LOGIC_SERVER_TEST && TEST_LOGIC_SERVER
            GC.Collect();
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("StartPVE id : {0} totalMemory : {1}", s,/*GC.GetTotalMemory(true))*/0);
#endif
            logic = LogicServer.NewGameLogic();
            logic.StartPVE(s, buff, bufflen);
#if MG_TEST && LOGIC_SERVER
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("StartPVE id: {0} stack :{1}",s, RecordServer.GetStackTraceModelName()));
            int id = logic.DungeonID;
            if(m_curAddBattle.ContainsKey(id))
            {
                m_curAddBattle[id]++;
            }
            else
            {
                m_curAddBattle.Add(id,1);
            }

#endif
            m_Logics[s] = logic;

#if LOGIC_SERVER && MG_TEST
            SortListWithFindOrInsert(m_maxInitConsumeTimeLogics,logic.curInitConsumeTime,logic.GetSession());
#endif
            return true;
        }
        catch (Exception e)
        {
#if !LOGIC_SERVER_TEST
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, e.ToString());
#else
            Logger.LogErrorFormat(e.ToString());
#endif
            return false;
        }
    }

    static public bool StartPVP(ulong s, System.IntPtr buff, int bufflen)
    {
        try
        {
            var logic = FindLogicInstance(s);  
            if (logic != null)
            {
#if !LOGIC_SERVER_TEST
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("race({0}) start pvp failed, already has this race.", s));
#else
                Logger.LogErrorFormat("race({0}) start pvp failed, already has this race.", s);
#endif
                return false;
            }
#if !LOGIC_SERVER_TEST && TEST_LOGIC_SERVER
            GC.Collect();
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("startPVP id : {0} totalMemory : {1}", s,0/*GC.GetTotalMemory(false))*/);
#endif
            logic = LogicServer.NewGameLogic();
            logic.StartPVP(s, buff, bufflen);
#if MG_TEST && LOGIC_SERVER
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("StartPVP id: {0} stack :{1}",s, RecordServer.GetStackTraceModelName()));
            int id = logic.DungeonID;
            if(m_curAddBattle.ContainsKey(id))
            {
                m_curAddBattle[id]++;
            }
            else
            {
                m_curAddBattle.Add(id,1);
            }
#endif
            m_Logics[s] = logic;
#if LOGIC_SERVER && MG_TEST
             SortListWithFindOrInsert(m_maxInitConsumeTimeLogics,logic.curInitConsumeTime,logic.GetSession());
#endif
            return true;
        }
        catch (Exception e)
        {
#if !LOGIC_SERVER_TEST
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, e.ToString());
#else
                Logger.LogErrorFormat(e.ToString());
#endif
            return false;
        }
    }

    static public bool PushFrameCommand(ulong s, System.IntPtr buff, int bufflen)
    {
        try
        {
            var logic = FindLogicInstance(s);
            if (logic == null)
            {
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("can't find race({0})", s));
                return false;
            }

            logic.PushFrameCommand(buff, bufflen);
            return true;
        }
        catch (Exception e)
        {
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, e.ToString());
            return false;
        }

    }
    static public void DumpMemory()
    {
        string dumpFileName = string.Format("dump-{0}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        //UnityHeapDump.Create(dumpFileName);
    }

    static public void StartPVPRecord(System.IntPtr buff, int bufflen)
    {
        var logic = LogicServer.NewGameLogic();
        logic.StartPVPRecord(buff, bufflen);
#if MG_TEST && LOGIC_SERVER
         LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("StartPVPRecord id:{0} stack :{1}",logic.GetSession(), RecordServer.GetStackTraceModelName()));
#endif
        m_Logics[logic.GetSession()] = logic;
#if LOGIC_SERVER && MG_TEST
       SortListWithFindOrInsert(m_maxInitConsumeTimeLogics,logic.curInitConsumeTime,logic.GetSession());
#endif
    }
    static ulong s = 0;
    static public void StartPVPRecord(byte[] buff)
    {
        var logic = LogicServer.NewGameLogic();
#if MG_TEST && LOGIC_SERVER
         LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("StartPVPRecord1 id: {0} stack :{1}",s+1, RecordServer.GetStackTraceModelName()));
#endif
        logic.StartPVPRecord(buff);
        m_Logics[++s] = logic;
#if LOGIC_SERVER && MG_TEST
        SortListWithFindOrInsert(m_maxInitConsumeTimeLogics,logic.curInitConsumeTime,logic.GetSession());
#endif
    }
    static public bool StartPVERecord(byte[] buff)
    {
        try
        {
            var logic = FindLogicInstance(s);
            if (logic != null)
            {
                Logger.LogErrorFormat("race({0}) start pve failed, already has this race.", s);
                return false;
            }
#if MG_TEST && LOGIC_SERVER
         LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("StartPVERecord1 id: {0} stack :{1}", s+ 1,RecordServer.GetStackTraceModelName()));
#endif
            logic = LogicServer.NewGameLogic();
            m_Logics[++s] = logic;
            logic.StartPVE(s, buff);
#if LOGIC_SERVER && MG_TEST
            SortListWithFindOrInsert(m_maxInitConsumeTimeLogics,logic.curInitConsumeTime,logic.GetSession());
#endif
            return true;
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat(e.ToString());
            return false;
        }
    }

    static public void GiveUpVerify(ulong s)
    {
        try
        {
            var logic = FindLogicInstance(s);
            if (logic == null)
            {
#if !LOGIC_SERVER_TEST
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("can't find race({0})", s));
#else
            Logger.LogErrorFormat("can't find race({0})", s);
#endif
                return;
            }

            logic.GiveUpVerify();
            DelLogicInstance(s);
            return;
        }
        catch (Exception e)
        {
#if !LOGIC_SERVER_TEST
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, e.ToString());
#else
            Logger.LogErrorFormat(e.ToString());
#endif
            return;
        }
    }

    /// <summary>
    /// 验证服务器c++层调用
    /// </summary>
    /// <param name="s"></param>
    static public void UnsyncVerify(ulong s)
    {
        try
        {
            unsyncChecker.PushUnsync("logicserver", RecordData.GetRootPath(), s.ToString());
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat(e.ToString());
        } 
    }

    static public UInt32 GetRaceNum()
    {
        return (UInt32)m_Logics.Count;
    }

    static public bool IsRaceFinish(ulong s)
    {
        try
        {
            var logic = FindLogicInstance(s);
            if (logic == null)
            {
                return true;
            }
            
            return logic.isEnd();
        }
        catch (Exception e)
        {
#if !LOGIC_SERVER_TEST
            LogicServer.LogConsole( LogicServer.LogicServerLogType.Error, e.ToString());
#else
            Logger.LogErrorFormat(e.ToString());
#endif
            return false;
        }
    }

    static public bool IsRunToLastFrame(ulong s)
    {
        try
        {
            var logic = FindLogicInstance(s);
            if (logic == null)
            {
                return true;
            }

            return !logic.HaveFrameInQueue();
        }
        catch (Exception e)
        {
#if !LOGIC_SERVER_TEST
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, e.ToString());
#else
            Logger.LogErrorFormat(e.ToString());
#endif
            return false;
        }
    }

    static public void SaveRecord(ulong s)
    {
         try
        {
            var logic = FindLogicInstance(s);
            if (logic == null)
            {
#if !LOGIC_SERVER_TEST
                LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("can't find race({0})", s));
#else
            Logger.LogErrorFormat("can't find race({0})", s);
#endif
                return;
            }

            logic.SaveRecord();
            return;
        }
        catch (Exception e)
        {
#if !LOGIC_SERVER_TEST
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, e.ToString());
#else
            Logger.LogErrorFormat(e.ToString());
#endif
            return;
        }
    }

    static public bool HaveFrameInQueue(ulong s)
    {
        try
        {
            var logic = FindLogicInstance(s);
            if (logic == null)
            {
                Logger.LogErrorFormat("can't find race({0})", s);
                return false;
            }

             return logic.HaveFrameInQueue();
        }
        catch (Exception e)
        {
            Logger.LogProcessFormat(e.ToString() + "\n");
            return false;
        }
    }
    
    static protected LogicServer FindLogicInstance(UInt64 id)
    {
        LogicServer logic;
        m_Logics.TryGetValue(id, out logic);
        return logic;
    }
    static public void CleanUpWorkSpace()
    {
        if (behaviac.Workspace.Instance != null)
        {
            behaviac.Workspace.Instance.Cleanup();
        }
    }
    static protected void DelLogicInstance(UInt64 id)
    {
#if MG_TEST && LOGIC_SERVER
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("DelLogicInstance:{0} stack :{1}", id, RecordServer.GetStackTraceModelName()));
        int dungeonId = -1;
#endif

        if (m_Logics.ContainsKey(id))
        {
#if MG_TEST && LOGIC_SERVER
            if (m_Logics[id] != null)
            {
                dungeonId = m_Logics[id].DungeonID;
            }
#endif
            m_Logics[id] = null;
        }
        m_Logics.Remove(id);

#if MG_TEST && LOGIC_SERVER
         if(m_curDecBattle.ContainsKey(dungeonId))
        {
            m_curDecBattle[dungeonId]++;
        }
        else
        {
            m_curDecBattle.Add(dungeonId,1);
        }
        for (int i = 0; i < m_maxConsumeTimeLogics.Length; i++)
        {
            var timeinfo = m_maxConsumeTimeLogics[i];
            if(timeinfo.Key == id)
            {
                m_maxConsumeTimeLogics[i] = new KeyValuePair<ulong, double>(0, 0);
                break;
            }
        }
        for (int i = 0; i < m_curMaxUpdateTimeLogics.Length; i++)
        {
            var timeinfo = m_curMaxUpdateTimeLogics[i];
            if (timeinfo.Key == id)
            {
                m_curMaxUpdateTimeLogics[i] = new KeyValuePair<ulong, double>(0, 0);
                break;
            }
        }
        for (int i = 0; i < m_maxInitConsumeTimeLogics.Length; i++)
        {
            var timeinfo = m_maxInitConsumeTimeLogics[i];
            if (timeinfo.Key == id)
            {
                m_maxInitConsumeTimeLogics[i] = new KeyValuePair<ulong, double>(0, 0);
                m_DirtyInitConsume = true;
                break;
            }
        }
        for (int i = 0; i < m_maxDurationTimeLogics.Length; i++)
        {
            var timeinfo = m_maxDurationTimeLogics[i];
            if (timeinfo.Key == id)
            {
                m_maxDurationTimeLogics[i] = new KeyValuePair<ulong, double>(0, 0);
                break;
            }
        }

        WaitDispose(id);
#endif

#if !LOGIC_SERVER_TEST && TEST_LOGIC_SERVER
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, "for test");
        GC.Collect();
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("DelLogicInstance:{0} totalMemory:{1}", id, 0;/*GC.GetTotalMemory(false))*/);
#endif
    }

#if MG_TEST && LOGIC_SERVER
    static public void WaitDispose(UInt64 id)
    {
        lock(disposelocker)
        {       
            if (m_waitDesposeLogics.ContainsKey(id))
            {
                Logger.LogErrorFormat("repeat wait dispose race({0})", id);
                return;
            }

            var now = Utility.GetCurrentTimeUnix();
            m_waitDesposeLogics.Add(id, now);

            Logger.LogErrorFormat("race({0}) start to wait dispose...", id);
        }
    }
    static public void OnLogicServerDisposed(UInt64 id)
    {
        bool needlocker = false;
        if(~0 == m_MainThreadID)    
        {
            Logger.LogError("MainThread is not set");
        }
        else if(m_MainThreadID != System.Threading.Thread.CurrentThread.ManagedThreadId)
        {
            Logger.LogError("WorkThread run OnLogicServerDisposed");
            needlocker = true;
        }
        if(needlocker)
        {
            lock(disposelocker)
            {
                if (m_waitDesposeLogics.ContainsKey(id) == false)
                {
                    Logger.LogErrorFormat("repeat dispose race({0})", id);
                    return;
                }

                var startTime = m_waitDesposeLogics[id];
                var now = Utility.GetCurrentTimeUnix();

                m_waitDesposeLogics.Remove(id);

                Logger.LogErrorFormat("race({0}) disposed delay({1})s", id, now - startTime);
            }
        }
        else
        {
            if (m_waitDesposeLogics.ContainsKey(id) == false)
            {
                Logger.LogErrorFormat("repeat dispose race({0})", id);
                return;
            }

            var startTime = m_waitDesposeLogics[id];
            var now = Utility.GetCurrentTimeUnix();

            m_waitDesposeLogics.Remove(id);

            Logger.LogErrorFormat("race({0}) disposed delay({1})s", id, now - startTime);
        }
    }

    static UInt32 lastShowTime = 0;
    static public void ShowLongWaitDisposeInfo()
    {
        var now = Utility.GetCurrentTimeUnix();

        if (now - lastShowTime <= 60)
        {
            return;
        }

        lock(disposelocker)
        {
            foreach (var e in m_waitDesposeLogics)
            {
                var delay = now - e.Value;
                if (delay >= 10)
                {
                    Logger.LogErrorFormat("race({0}) wait dispose too long, current delay({1})s", e.Key, delay);
                }
            }
        }
        lastShowTime = now;
    }

   static private void SortList(KeyValuePair<UInt64, double>[] list)
    {
        //排序
        for (int i = 0; i < list.Length; i++)
        {
            for (int j = i + 1; j < list.Length; j++)
            {
                var b = list[j];
                var a = list[i];
                if (a.Value < b.Value)
                {
                    list[i] = b;
                    list[j] = a;
                }
            }
        }
    }
    static private void SortListWithFindOrInsert(KeyValuePair<UInt64, double>[] list,double value, UInt64 sessionId)
    {
        //查找
        bool find = false;
        for (int i = 0; i < list.Length; i++)
        {
            var timeinfo = list[i];
            if (timeinfo.Key == sessionId)
            {
                list[i] = new KeyValuePair<ulong, double>(sessionId, value);
                find = true;
                break;
            }
        }
        SortList(list);
        //插入
        if (!find)
        {
            for (int i = 0; i < list.Length; i++)
            {
                var timeInfo = list[i];
                if (timeInfo.Value < value)
                {
                    for (int j = list.Length - 1; j > i; j--)
                    {
                        list[j] = list[j - 1];
                    }
                    list[i] = new KeyValuePair<ulong, double>(sessionId, value);
                    break;
                }
                else if (timeInfo.Value == value)
                {
                    break;
                }
            }
        }
    }
    static private void ShowBattleStatistic(System.IO.StreamWriter sw)
    {
          sw.WriteLine("===============Statistic Begin===============");
          var iter = m_curAddBattle.GetEnumerator();
          int curAddBattleCount = 0; 
          int preAddDungeonCount = 0;
          while(iter.MoveNext())
          {
              curAddBattleCount += iter.Current.Value;
              int curDungeonCount = iter.Current.Value;
              int dungeonId       = iter.Current.Key;
              int preDungonCount = 0;
              if(m_prevAddBattle != null)
              {
                 if(m_prevAddBattle.ContainsKey(dungeonId))
                {
                    preDungonCount = m_prevAddBattle[dungeonId];
                    preAddDungeonCount += preDungonCount;
                }
              }
              sw.WriteLine(string.Format("DungoneID {0} AddCount {1}",dungeonId,curDungeonCount - preDungonCount));
          }

          iter = m_curDecBattle.GetEnumerator();
          int curDecBattleCount = 0;
          int preDecDungeonCount = 0;
          while(iter.MoveNext())
          {
              curDecBattleCount += iter.Current.Value;
              int curDungeonCount = iter.Current.Value;
              int dungeonId       = iter.Current.Key;
              int preDungeonCount = 0;
              if(m_prevDecBattle != null)
              {
                 if(m_prevDecBattle.ContainsKey(dungeonId))
                {
                    preDungeonCount = m_prevDecBattle[dungeonId];
                    preDecDungeonCount += preDungeonCount;
                }
              }
               sw.WriteLine(string.Format("DungoneID {0} DecCount {1}",dungeonId,curDungeonCount - preDungeonCount));
          }
           sw.WriteLine(string.Format("TotalBattleAddCount {0} TotalBattleDecCount {1} CurBattleAddCount {2} CurBattleDecCount {3}",
                                 curAddBattleCount,curDecBattleCount,curAddBattleCount - preAddDungeonCount,curDecBattleCount - preDecDungeonCount));
       
          sw.WriteLine("===============Statistic End===============");
          m_prevDecBattle = new Dictionary<int,int>(m_curDecBattle);
          m_prevAddBattle = new Dictionary<int,int>(m_curAddBattle);
     
    }
#endif
}
