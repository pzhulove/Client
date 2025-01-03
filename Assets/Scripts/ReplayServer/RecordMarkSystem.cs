using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

//Create Time : 2020-4-15
//Author By: Shen si
//Description : 水印机制，用以替代ProcessFile日志，节省内存，直接保存成二进制文件
//每一个水印下保存运行时参数，战斗完毕后，把运行时的参数保存在文件中
//在播放录像的时候运行时参数值和保存在文件的参数值进行比较
//如果不相等则中断执行


//当前水印信息
public class MarkDataRunTime
{
    public uint curCallNum = 0;    //当前markid下已经调用的次数
    public List<NodeData> m_CallData = new List<NodeData>();
    public void Recycle()
    {
        //写入文件需要的数据和本数据是同源所以不能放入pool
        curCallNum = 0;
        m_CallData.Clear();
    }
};

//当前帧数下的所有水印信息 
public class FrameMarkDataRunTime
{
    public uint curFrame = 0;       //当前逻辑帧
    public uint sequence = 0;     //当前update索引
    public Dictionary<uint, MarkDataRunTime> m_FrameDatas = new Dictionary<uint, MarkDataRunTime>();
    public void Recycle()
    {
        curFrame = 0;
        sequence = 0;
        var iter = m_FrameDatas.GetEnumerator();
        while (iter.MoveNext())
        {
            GameClient.MarkDataRunTimePool.Release(iter.Current.Value);
        }
        m_FrameDatas.Clear();
    }
};

//水印详细信息
public class NodeRunTimeData
{
    public uint markId = 0;   //水印id
    public uint totalCallNum = 0; //总调用次数
    public uint callNum = 0; //当前水印 下的调用次数
    public UInt64 timeStamp = 0;  //时间戳
    public uint curFrame = 0;   //当前帧数
    public uint randCallNum = 0;
    public uint randSeed = 0;
    public int[] paramDatas = null;  //当前整型的参数
    public string[] paramStr = null;   //当前字符串型参数
};
public enum RECORD_MODE   //播放模式
{
    RECORD,   //记录模式
    REPLAY     //播放录像模式
};
//水印管理器
public class RecordMarkSystem
{
    private FrameMarkDataRunTime m_curFrameData = null;   //当前帧水印数据
    private List<FrameMarkDataRunTime> m_FrameDatas = null;//  //所有帧水印数据 存储
    private Dictionary<uint, FrameMarkDataRunTime> m_ReplayFrameDatas = null; //所有帧水印数据 读取
    private int m_LastFrameCount = 0; //已经写入的条目数量
    private uint m_totalCallNum = 0;
    private RECORD_MODE m_Mode;
    private uint m_frameDataIndex = 0;
    private string m_version = string.Empty;
    private int m_versionStrLen = 0;
    private BattleType m_battleType;
    private eDungeonMode m_DungoneMode;
    private string m_sessionId = string.Empty;
    private FrameRandomImp m_frameRandom = null;
#if LOGIC_SERVER
    private LogicServer m_Inst = null;
#else 
    private FrameSync m_Inst = null;
#endif
    public void SetRandom(FrameRandomImp rand)
    {
        m_frameRandom = rand;
    }
    public void SetLogicServer(LogicServer inst)
    {
#if LOGIC_SERVER
           m_Inst = inst;
#endif
    }
    public RecordMarkSystem(RECORD_MODE mode, object inst, string version, string sessionId, BattleType type, eDungeonMode dungeonMode)
    {
        m_Mode = mode;
        m_DungoneMode = dungeonMode;
        m_battleType = type;
        m_sessionId = sessionId;
        m_version = version;
        byte[] nameBytes = StringHelper.StringToUTF8Bytes(m_version);
        m_versionStrLen = 2;
        if (nameBytes.Length > 0)
        {
            m_versionStrLen += nameBytes.Length - 1;
        }
#if LOGIC_SERVER
        m_Inst = inst as LogicServer;
#else
        m_Inst = inst as FrameSync;
#endif

    }
    public void BeginUpdate()
    {
        if (m_Mode != RECORD_MODE.REPLAY) return;
        if (m_ReplayFrameDatas == null)
        {
            return;
        }
        if (m_ReplayFrameDatas.ContainsKey(m_frameDataIndex))
        {
            m_curFrameData = m_ReplayFrameDatas[m_frameDataIndex];
        }
    }
    //一个逻辑帧结束时调用
    public void EndUpdate()
    {
        m_frameDataIndex++;
        if (m_Mode != RECORD_MODE.RECORD)
        {
            m_curFrameData = null;
            return;
        }
        if (m_FrameDatas == null)
        {
            m_FrameDatas = new List<FrameMarkDataRunTime>();
        }
        if (m_curFrameData != null)
        {
            m_FrameDatas.Add(m_curFrameData);
        }
        m_curFrameData = null;

    }
    public uint GetCurFrame()
    {
        if (m_Inst == null) return 0;
#if LOGIC_SERVER
        if(m_frameRandom != null)
        {
            return m_frameRandom.callFrame;
        }
        return m_Inst.GetCurFrame();
#else
        return m_Inst.curFrame;
#endif
    }

    //导出成可阅读的日志文件,通过配表文件生成格式化后的可阅读的日志
    public void DumpProcessFileFromMark(string path)
    {
        Load(path);
        var descDic = GetMarkReadableDic();
        var nodeDataList = new List<NodeRunTimeData>();
        var frameiter = m_ReplayFrameDatas.GetEnumerator();
        while (frameiter.MoveNext())
        {
            var curFrame = frameiter.Current.Value;
            var iter = curFrame.m_FrameDatas.GetEnumerator();

            while (iter.MoveNext())
            {
                var markDataList = iter.Current.Value;
                for (int j = 0; j < markDataList.m_CallData.Count; j++)
                {
                    var curData = markDataList.m_CallData[j];
                    nodeDataList.Add(new NodeRunTimeData
                    {
                        totalCallNum = curData.totalCallNum,
                        markId = iter.Current.Key,
                        callNum = curData.callNum,
                        timeStamp = curData.timeStamp,
                        curFrame = curFrame.curFrame,
                        randSeed = curData.randSeed,
                        randCallNum = curData.randCallNum,
                        paramDatas = curData.paramDatas,
                        paramStr = curData.paramStrings
                    });
                }
            }
        }
        nodeDataList.Sort(SortByCallNum); //按照总调用次数来排序
        //生成格式化文件
        StringBuilder builder = new StringBuilder(string.Format("[Normal Battle Log]Version:{0} SessionID:{1} BattleType:{2} DungeonMode:{3}\r\n", m_version, m_sessionId, m_battleType, m_DungoneMode));
        StringBuilder buildParam = new StringBuilder();
        for (int i = 0; i < nodeDataList.Count; i++)
        {
            buildParam.Clear();
            var curNode = nodeDataList[i];
            string descFormat = null;
            if (descDic.TryGetValue(curNode.markId, out descFormat))
            {
                List<object> argsList = new List<object>();
                for (int argsi = 0; argsi < curNode.paramDatas.Length; argsi++)
                {
                    argsList.Add(curNode.paramDatas[argsi]);
                }

                for (int argsi = 0; argsi < curNode.paramStr.Length; argsi++)
                {
                    argsList.Add(curNode.paramStr[argsi]);
                }

                object[] args = argsList.ToArray();
                Logger.LogErrorFormat("markid {0} format {1}", curNode.markId, descFormat);
                buildParam.AppendFormat(descFormat, args);
            }
            else
            {
                for (int j = 0; j < curNode.paramDatas.Length; j++)
                {
                    buildParam.Append(curNode.paramDatas[j]);
                    buildParam.Append("|");
                }
                for (int j = 0; j < curNode.paramStr.Length; j++)
                {
                    buildParam.Append(curNode.paramStr[j]);
                    buildParam.Append("|");
                }
            }
            var dateTime = Utility.GetDateTimeByUnixTime(curNode.timeStamp);
            string prefix = string.Format("[{0}][{5}]T[{1}]C[{2}][0x{4:X}]F[{3}]R[{6},{7}]",
                                            dateTime.ToString("yyyy-MM-dd HH:mm:ss:ms"),
                                            curNode.totalCallNum,
                                            curNode.callNum,
                                            curNode.curFrame,
                                            curNode.markId, curNode.timeStamp, curNode.randCallNum, curNode.randSeed);

            builder.AppendLine(prefix + buildParam.ToString());
        }
        TextWriter sw = null;
        string dumpPath = Path.GetFileNameWithoutExtension(path);
        string dumpDir = Path.GetDirectoryName(path);
        dumpPath = string.Format("{0}/{1}_Process.txt", dumpDir, dumpPath);
        try
        {
            sw = File.CreateText(dumpPath);
            sw.Write(builder.ToString());
            sw.Flush();
            sw.Close();
        }
        catch (Exception e)
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
            Logger.LogErrorFormat("[DumpMark] {0} failed {1}", dumpPath, e.ToString());
        }
    }

    private Dictionary<uint, string> GetMarkReadableDic()
    {
        Dictionary<uint, string> markReadableDic = new Dictionary<uint, string>();
        StreamReader file = null;
        try
        {
            string path = Application.dataPath + "/Editor/RecordMarkDesc/ReadableMarkMap.txt";
            if (!File.Exists(path))
            {
                Logger.LogErrorFormat("请先生成水印描述问题件");
                return markReadableDic;
            }
            file = new StreamReader(path);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                var splitStr = line.Split(new[] { "|@@|" }, StringSplitOptions.RemoveEmptyEntries);//line.Split(new []{'|','@','@','|'});
                if (splitStr.Length >= 2)
                {
                    uint id = Convert.ToUInt32(splitStr[0], 16);
                    if (markReadableDic.ContainsKey(id))
                    {
                        Logger.LogErrorFormat("has same mark id:{0} in desc file", id);
                    }
                    else
                    {
                        markReadableDic.Add(id, splitStr[splitStr.Length - 1]);
                    }
                }
            }
            file.Close();
        }
        catch (Exception e)
        {
            Logger.LogError(e.ToString());
            if (file != null)
            {
                file.Close();
            }
        }

        return markReadableDic;
    }

    //按照总调用次数升序排序
    private int SortByCallNum(NodeRunTimeData a, NodeRunTimeData b)
    {
        int compVal = a.totalCallNum.CompareTo(b.totalCallNum);
        if (compVal == 0)
        {
            return a.callNum.CompareTo(b.callNum);
        }
        return compVal;
    }

    public NodeData Mark(uint id)
    {
        NodeData nodeData = null;
        if (m_Mode == RECORD_MODE.RECORD)
        {
            if (m_curFrameData == null)
            {
                m_curFrameData = GameClient.FrameMarkDataRunTimePool.Get();
                m_curFrameData.sequence = m_frameDataIndex;
                m_curFrameData.curFrame = GetCurFrame();
            }
            MarkDataRunTime curMarkData = null;
            m_totalCallNum++;
            if (!m_curFrameData.m_FrameDatas.ContainsKey(id))
            {
                m_curFrameData.m_FrameDatas.Add(id, GameClient.MarkDataRunTimePool.Get());
            }
            curMarkData = m_curFrameData.m_FrameDatas[id];
            curMarkData.curCallNum++;

            nodeData = GameClient.MarkNodeDataPool.Get();
            nodeData.callNum = curMarkData.curCallNum;
            nodeData.totalCallNum = m_totalCallNum;
            nodeData.randCallNum = m_frameRandom != null ? m_frameRandom.callNum : 0;
            nodeData.randSeed = m_frameRandom != null ? m_frameRandom.GetSeed() : 0;
            nodeData.timeStamp = (UInt64)Utility.GetTimeStamp();
            curMarkData.m_CallData.Add(nodeData);
        }
        else
        {
            nodeData = GameClient.MarkNodeDataPool.Get();
        }
        return nodeData;
    }

    public static void TryBreak()
    {
    #if UNITY_EDITOR && !LOGIC_SERVER
        UnityEngine.Debug.Break();
    #endif
    }

    public void Check(uint id, NodeData data)
    {
#if UNITY_EDITOR
        if (data == null)
            return;

        if (m_Mode != RECORD_MODE.REPLAY)
            return;

        NodeData curNodeData = null;
        bool bRet = _CommonCheck(id, ref curNodeData);
        if (!bRet) return;
        if (curNodeData.paramDatas.Length != data.paramDatas.Length)
        {
            Logger.LogErrorFormat("paramDatas Len is not equal {0} {1} {2} {3} {4}", m_totalCallNum, id, m_frameDataIndex, curNodeData.paramDatas.Length, data.paramDatas.Length);
            TryBreak();
            return;
        }

        for (int i = 0; i < curNodeData.paramDatas.Length; i++)
        {
            if (curNodeData.paramDatas[i] != data.paramDatas[i])
            {
                Logger.LogErrorFormat("paramDatas value is not equal {0} {1} {2} {3} {4} {5}", m_totalCallNum, id, m_frameDataIndex, i, curNodeData.paramStrings[i], data.paramDatas[i]);
                TryBreak();
                return;
            }
        }

        if (curNodeData.paramStrings.Length != data.paramStrings.Length)
        {
            Logger.LogErrorFormat("paramStrings Len is not equal {0} {1} {2} {3} {4}", m_totalCallNum, id, m_frameDataIndex, curNodeData.paramDatas.Length, data.paramStrings.Length);
            TryBreak();
            return;
        }

        for (int i = 0; i < curNodeData.paramStrings.Length; i++)
        {
            if (curNodeData.paramStrings[i] != data.paramStrings[i])
            {
                Logger.LogErrorFormat("paramStrings value is not equal {0} {1} {2} {3} {4} {5}", m_totalCallNum, id, m_frameDataIndex, i, curNodeData.paramStrings[i], data.paramStrings[i]);
                TryBreak();
                return;
            }
        }
#endif
    }

    private void _MarkAll(uint id, int[] datas, string[] datasStr)
    {
        if (m_Mode == RECORD_MODE.RECORD)
        {
            if (m_curFrameData == null)
            {
                m_curFrameData = GameClient.FrameMarkDataRunTimePool.Get();
                m_curFrameData.sequence = m_frameDataIndex;
                m_curFrameData.curFrame = GetCurFrame();
            }
            MarkDataRunTime curMarkData = null;
            m_totalCallNum++;
            if (!m_curFrameData.m_FrameDatas.ContainsKey(id))
            {
                m_curFrameData.m_FrameDatas.Add(id, GameClient.MarkDataRunTimePool.Get());
            }
            curMarkData = m_curFrameData.m_FrameDatas[id];
            curMarkData.curCallNum++;
            var newNodeData = GameClient.MarkNodeDataPool.Get();
            newNodeData.callNum = curMarkData.curCallNum;
            newNodeData.totalCallNum = m_totalCallNum;
            newNodeData.timeStamp = (UInt64)Utility.GetTimeStamp();
            newNodeData.randCallNum = m_frameRandom != null ? m_frameRandom.callNum : 0;
            newNodeData.randSeed = m_frameRandom != null ? m_frameRandom.GetSeed() : 0;
            newNodeData.paramDatas = datas;
            newNodeData.paramStrings = datasStr;
            curMarkData.m_CallData.Add(newNodeData);
        }
        else
        {
#if UNITY_EDITOR
            NodeData curNodeData = null;
            bool bRet = _CommonCheck(id, ref curNodeData);
            if (!bRet) return;
            if (curNodeData.paramDatas.Length != datas.Length)
            {
                Logger.LogErrorFormat("paramDatas Len is not equal {0} {1} {2} {3} {4}", m_totalCallNum, id, m_frameDataIndex, curNodeData.paramDatas.Length, datas.Length);
                TryBreak();
                return;
            }

            for (int i = 0; i < curNodeData.paramDatas.Length; i++)
            {
                if (curNodeData.paramDatas[i] != datas[i])
                {
                    Logger.LogErrorFormat("paramDatas value is not equal {0} {1} {2} {3} {4} {5}", m_totalCallNum, id, m_frameDataIndex, i, curNodeData.paramStrings[i], datas[i]);
                    TryBreak();
                    return;
                }
            }

            if (curNodeData.paramStrings.Length != datasStr.Length)
            {
                Logger.LogErrorFormat("paramStrings Len is not equal {0} {1} {2} {3} {4}", m_totalCallNum, id, m_frameDataIndex, curNodeData.paramDatas.Length, datas.Length);
                TryBreak();
                return;
            }

            for (int i = 0; i < curNodeData.paramStrings.Length; i++)
            {
                if (curNodeData.paramStrings[i] != datasStr[i])
                {
                    Logger.LogErrorFormat("paramStrings value is not equal {0} {1} {2} {3} {4} {5}", m_totalCallNum, id, m_frameDataIndex, i, curNodeData.paramStrings[i], datas[i]);
                    TryBreak();
                    return;
                }
            }
#endif
        }
    }
    public void Mark(uint id, string[] dataStr, params int[] datas)
    {
        _MarkAll(id, datas, dataStr);
    }
    public void Mark(uint id, int[] datas, params string[] datasStr)
    {
        _MarkAll(id, datas, datasStr);
    }
    //水印参数全部都是字符串类型
    public void MarkString(uint id, params string[] datas)
    {
        if (m_Mode == RECORD_MODE.RECORD)
        {
            if (m_curFrameData == null)
            {
                m_curFrameData = GameClient.FrameMarkDataRunTimePool.Get();
                m_curFrameData.sequence = m_frameDataIndex;
                m_curFrameData.curFrame = GetCurFrame();
            }
            MarkDataRunTime curMarkData = null;
            m_totalCallNum++;
            if (!m_curFrameData.m_FrameDatas.ContainsKey(id))
            {
                m_curFrameData.m_FrameDatas.Add(id, GameClient.MarkDataRunTimePool.Get());
            }
            curMarkData = m_curFrameData.m_FrameDatas[id];
            curMarkData.curCallNum++;
            var newNodeData = GameClient.MarkNodeDataPool.Get();
            newNodeData.callNum = curMarkData.curCallNum;
            newNodeData.totalCallNum = m_totalCallNum;
            newNodeData.timeStamp = (UInt64)Utility.GetTimeStamp();
            newNodeData.randCallNum = m_frameRandom != null ? m_frameRandom.callNum : 0;
            newNodeData.randSeed = m_frameRandom != null ? m_frameRandom.GetSeed() : 0;
            newNodeData.paramStrings = datas;
            curMarkData.m_CallData.Add(newNodeData);
        }
        else
        {
#if UNITY_EDITOR
            NodeData curNodeData = null;
            bool bRet = _CommonCheck(id, ref curNodeData);
            if (!bRet) return;
            if (curNodeData.paramStrings.Length != datas.Length)
            {
                Logger.LogErrorFormat("paramStrings Len is not equal {0} {1} {2} {3} {4}", m_totalCallNum, id, m_frameDataIndex, curNodeData.paramDatas.Length, datas.Length);
                TryBreak();
                return;
            }

            for (int i = 0; i < curNodeData.paramStrings.Length; i++)
            {
                if (curNodeData.paramStrings[i] != datas[i])
                {
                    Logger.LogErrorFormat("paramStrings value is not equal {0} {1} {2} {3} {4} {5}", m_totalCallNum, id, m_frameDataIndex, i, curNodeData.paramStrings[i], datas[i]);
                    TryBreak();
                    return;
                }
            }
#endif
        }
    }
    private bool _CommonCheck(uint id, ref NodeData curNodeData)
    {
        if (m_curFrameData == null)
        {
            if (m_ReplayFrameDatas.ContainsKey(m_frameDataIndex))
            {
                m_curFrameData = m_ReplayFrameDatas[m_frameDataIndex];
            }

            if (m_curFrameData == null)
            {
                Logger.LogErrorFormat("m_curFrameData is null {0} {1} {2}", id, m_frameDataIndex, m_totalCallNum);
                TryBreak();
                return false;
            }
        }
        m_totalCallNum++;
        if (!m_curFrameData.m_FrameDatas.ContainsKey(id))
        {
            Logger.LogErrorFormat("MarkId can not be finded {0} {1} {2}", id, m_frameDataIndex, m_totalCallNum);
            TryBreak();
            return false;
        }

        var curMarkData = m_curFrameData.m_FrameDatas[id];
        curMarkData.curCallNum++;
        if (curMarkData.curCallNum > curMarkData.m_CallData.Count)
        {
            Logger.LogErrorFormat("curCallNum larger {0} {1} {2} {3} {4}", id, m_frameDataIndex, m_totalCallNum, curMarkData.curCallNum, curMarkData.m_CallData.Count);
            TryBreak();
            return false;
        }

        curNodeData = curMarkData.m_CallData[(int)(curMarkData.curCallNum - 1)];
        if (curNodeData.callNum != curMarkData.curCallNum)
        {
            Logger.LogErrorFormat("callNum is not equal {0} {1} {2} {3} {4}", id, m_frameDataIndex, m_totalCallNum, curNodeData.callNum, curMarkData.curCallNum);
            TryBreak();
            return false;
        }

        if (curNodeData.totalCallNum != m_totalCallNum)
        {
            Logger.LogErrorFormat("totalCallNum is not equal {0} {1} {2} {3} {4}", m_totalCallNum, curNodeData.totalCallNum, curNodeData.callNum, m_frameDataIndex, id);
            TryBreak();
            return false;
        }
        return true;
    }

    //水印参数为整型
    public void MarkInt(uint id, params int[] datas)
    {
        if (m_Mode == RECORD_MODE.RECORD)
        {
            if (m_curFrameData == null)
            {
                m_curFrameData = GameClient.FrameMarkDataRunTimePool.Get();
                m_curFrameData.sequence = m_frameDataIndex;
                m_curFrameData.curFrame = GetCurFrame();
            }
            MarkDataRunTime curMarkData = null;
            m_totalCallNum++;
            if (!m_curFrameData.m_FrameDatas.ContainsKey(id))
            {
                m_curFrameData.m_FrameDatas.Add(id, GameClient.MarkDataRunTimePool.Get());
            }
            curMarkData = m_curFrameData.m_FrameDatas[id];
            curMarkData.curCallNum++;
            var newNodeData = GameClient.MarkNodeDataPool.Get();
            newNodeData.callNum = curMarkData.curCallNum;
            newNodeData.totalCallNum = m_totalCallNum;
            newNodeData.timeStamp = Utility.GetCurrentTimeUnix();
            newNodeData.randCallNum = m_frameRandom != null ? m_frameRandom.callNum : 0;
            newNodeData.randSeed = m_frameRandom != null ? m_frameRandom.GetSeed() : 0;
            newNodeData.paramDatas = datas;
            curMarkData.m_CallData.Add(newNodeData);
        }
        else
        {
#if UNITY_EDITOR

            NodeData curNodeData = null;
            bool bRet = _CommonCheck(id, ref curNodeData);
            if (!bRet) return;
            if (curNodeData.paramDatas.Length != datas.Length)
            {
                Logger.LogErrorFormat("paramDatas Len is not equal {0} {1} {2} {3} {4}", m_totalCallNum, id, m_frameDataIndex, curNodeData.paramDatas.Length, datas.Length);
                TryBreak();
                return;
            }

            for (int i = 0; i < curNodeData.paramDatas.Length; i++)
            {
                if (curNodeData.paramDatas[i] != datas[i])
                {
                    Logger.LogErrorFormat("paramDatas value is not equal {0} {1} {2} {3} {4} {5}", m_totalCallNum, id, m_frameDataIndex, i, curNodeData.paramDatas[i], datas[i]);
                    TryBreak();
                    return;
                }
            }
#endif
        }

    }

    public void _WriteLeftFrameData(string path, byte[] buffer, List<FrameMarkDataRunTime> frameDatas, bool isInBattle)
    {
        //    Logger.LogErrorFormat("_WriteLeftFrameData {0}", path);
        FileStream fs = null;
        BinaryWriter sw = null;
        int fileLen = 0;
        try
        {
            fs = new FileStream(path, FileMode.Open, FileAccess.Write);
            sw = new BinaryWriter(fs);
            int filePosition = 8 + 2 + m_versionStrLen;
            sw.Seek(filePosition, SeekOrigin.Begin);
            fileLen = (int)fs.Length;
            sw.Write(m_LastFrameCount + frameDatas.Count);
            sw.Flush();
            fs.Flush();
            sw.Close();
            fs.Close();
        }
        catch (Exception e)
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            Logger.LogErrorFormat("Flush file Failed {0}", e.ToString());
            return;
        }
        MapViewStream st = new MapViewStream(path, buffer, MapViewStream.FileAccessMode.Write, fileLen);
        int pos = 0;
        for (int i = 0; i < frameDatas.Count; i++)
        {
            var curFrameInfo = frameDatas[i];
            var newFrameInfo = isInBattle ? new FrameMarkData() : GameClient.FrameMarkDataPool.Get();
            newFrameInfo.frame = curFrameInfo.curFrame;
            newFrameInfo.sequence = curFrameInfo.sequence;

            newFrameInfo.markDatas = new MarkData[curFrameInfo.m_FrameDatas.Count];
            var iter = curFrameInfo.m_FrameDatas.GetEnumerator();
            int curMarkIndex = 0;
            while (iter.MoveNext())
            {
                var newMark = isInBattle ? new MarkData() : GameClient.MarkDataPool.Get();
                newMark.id = iter.Current.Key;
                newFrameInfo.markDatas[curMarkIndex] = newMark;
                var curMark = iter.Current.Value;
                newMark.markDatas = curMark.m_CallData.ToArray();
                curMarkIndex++;
            }
            newFrameInfo.encode(st, ref pos);
            if(!isInBattle)
                GameClient.FrameMarkDataPool.Release(newFrameInfo);
        }
        bool bRet = st.Save();
        if (bRet)
        {
            if (!isInBattle)
            {
                m_LastFrameCount += frameDatas.Count;
                for (int i = 0; i < frameDatas.Count; i++)
                {
                    GameClient.FrameMarkDataRunTimePool.Release(frameDatas[i]);
                }
                frameDatas.Clear();
            }
        }
    }
    private void _WriteWholeFrameData(string path, byte[] buffer, List<FrameMarkDataRunTime> frameDatas, bool isInBattle)
    {
        //    Logger.LogErrorFormat("_WriteWholeFrameData {0}", path);
        MarkFileData markFileData = new MarkFileData();
        markFileData.markDatas = new FrameMarkData[frameDatas.Count];
        markFileData.version = m_version;
        markFileData.battleType = (byte)m_battleType;
        markFileData.dungeonMode = (byte)m_DungoneMode;
        markFileData.sessionId = Convert.ToUInt64(m_sessionId);
        for (int i = 0; i < frameDatas.Count; i++)
        {
            var curFrameInfo = frameDatas[i];

            var newFrameInfo = isInBattle ? new FrameMarkData() : GameClient.FrameMarkDataPool.Get();
            newFrameInfo.frame = curFrameInfo.curFrame;
            newFrameInfo.sequence = curFrameInfo.sequence;
            markFileData.markDatas[i] = newFrameInfo;
            newFrameInfo.markDatas = new MarkData[curFrameInfo.m_FrameDatas.Count];
            var iter = curFrameInfo.m_FrameDatas.GetEnumerator();
            int curMarkIndex = 0;
            while (iter.MoveNext())
            {
                var newMark = isInBattle ? new MarkData() : GameClient.MarkDataPool.Get();
                newMark.id = iter.Current.Key;
                newFrameInfo.markDatas[curMarkIndex] = newMark;
                var curMark = iter.Current.Value;
                newMark.markDatas = curMark.m_CallData.ToArray();
                curMarkIndex++;
            }
        }
        MapViewStream st = new MapViewStream(path, buffer, MapViewStream.FileAccessMode.Write);
        int pos = 0;
        markFileData.encode(st, ref pos);
        bool bRet = st.Save();
        if (bRet)
        {
            if (!isInBattle)
            {
                markFileData.Recycle();
                m_LastFrameCount = frameDatas.Count;
                for (int i = 0; i < frameDatas.Count; i++)
                {
                    GameClient.FrameMarkDataRunTimePool.Release(frameDatas[i]);
                }
                frameDatas.Clear();
            }
        }
    }
    public void Flush(string path, byte[] buffer)
    {
        if (m_Mode == RECORD_MODE.REPLAY) return;
        if (m_FrameDatas == null || m_FrameDatas.Count <= 0) return;
        if (m_LastFrameCount > 0)
        {
            _WriteLeftFrameData(path, buffer, m_FrameDatas, false);
        }
        else
        {
            _WriteWholeFrameData(path, buffer, m_FrameDatas, false);
        }
    }
    //保存水印文件
    public void Save(string path, byte[] buffer)
    {
        if (m_Mode == RECORD_MODE.REPLAY) return;
        //把最后一帧的数据放入m_FrameDatas中
        EndUpdate();
        if (m_FrameDatas == null || m_FrameDatas.Count <= 0) return;
        if (m_LastFrameCount > 0)
        {
            _WriteLeftFrameData(path, buffer, m_FrameDatas, false);
        }
        else
        {
            _WriteWholeFrameData(path, buffer, m_FrameDatas, false);
        }

        GameClient.FrameMarkDataRunTimePool.Clear();
        GameClient.FrameMarkDataPool.Clear();
        GameClient.MarkDataRunTimePool.Clear();
        GameClient.MarkDataPool.Clear();
        GameClient.MarkNodeDataPool.Clear();
    }
    public void SaveInBattle(string srcPath, string dstPath, byte[] buffer)
    {
        var tempFrameDatas = new List<FrameMarkDataRunTime>();
        if (m_FrameDatas != null)
        {
            tempFrameDatas.AddRange(m_FrameDatas);
        }

        if (m_curFrameData != null)
        {
            tempFrameDatas.Add(m_curFrameData);
        }
        try
        {
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }
            if (File.Exists(srcPath))
            {
                File.Copy(srcPath, dstPath);
            }
            if (tempFrameDatas == null || tempFrameDatas.Count <= 0) return;
            if (m_LastFrameCount > 0)
            {
                _WriteLeftFrameData(dstPath, buffer, tempFrameDatas, true);
            }
            else
            {
                _WriteWholeFrameData(dstPath, buffer, tempFrameDatas, true);
            }
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("Save MarkFile In Battle Failed: {0}", e.ToString());
        }
    }
    //读取水印文件
    public void Load(string path)
    {
        if (!File.Exists(path)) return;
        m_ReplayFrameDatas = new Dictionary<uint, FrameMarkDataRunTime>();
        FileStream fs = null;
        BinaryReader br = null;
        try
        {
            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs);
            var buff = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            br = null;
            fs = null;
            MarkFileData markFileData = new MarkFileData();
            int pos = 0;
            markFileData.decode(buff, ref pos);
            m_battleType = (BattleType)markFileData.battleType;
            m_sessionId = markFileData.sessionId.ToString();
            m_DungoneMode = (eDungeonMode)markFileData.dungeonMode;
            m_version = markFileData.version;
            for (int i = 0; i < markFileData.markDatas.Length; i++)
            {
                var curFrameData = markFileData.markDatas[i];
                var curFrameRunTimeData = new FrameMarkDataRunTime
                {
                    curFrame = curFrameData.frame,
                    sequence = curFrameData.sequence
                };
                m_ReplayFrameDatas.Add(curFrameData.sequence, curFrameRunTimeData);
                for (int j = 0; j < curFrameData.markDatas.Length; j++)
                {
                    var curMarkData = curFrameData.markDatas[j];
                    var curMarkRunTimeData = new MarkDataRunTime();
                    curFrameRunTimeData.m_FrameDatas.Add(curMarkData.id, curMarkRunTimeData);
                    for (int k = 0; k < curMarkData.markDatas.Length; k++)
                    {
                        curMarkRunTimeData.m_CallData.Add(curMarkData.markDatas[k]);
                    }
                }
            }
            markFileData = null;
            buff = null;
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("Read MarkData failed reason :{0}", e.ToString());
            if (br != null)
            {
                br.Close();
                br = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
        }
    }
}
