using System;
//Create Time : 2020-4-15
//Author By: Shen si
//Description : 水印文件结构描述

/// <summary>
/// 水印详细信息
/// </summary>
public class NodeData
{
    public UInt32 callNum;
    public UInt32 totalCallNum;
    public UInt64 timeStamp;
    public UInt32 randCallNum;
    public UInt32 randSeed;
    public int[] paramDatas = new int[0];
    public string[] paramStrings = new string[0];

    public NodeData Add(params int[] args)
    {
        var list = GamePool.ListPool<int>.Get();
        list.AddRange(paramDatas);
        list.AddRange(args);
        paramDatas = list.ToArray();
        GamePool.ListPool<int>.Release(list);
        return this;
    }
    
    public NodeData Add(params string[] args)
    {
        var list = GamePool.ListPool<string>.Get();
        list.AddRange(paramStrings);
        list.AddRange(args);
        paramStrings = list.ToArray();
        GamePool.ListPool<string>.Release(list);
        return this;
    }
    public void Recycle()
    {
        callNum = 0;
        totalCallNum = 0;
        timeStamp = 0;
        randCallNum = 0;
        randSeed = 0;
        paramDatas = Array.Empty<int>();
        paramStrings = Array.Empty<string>();
    }

    #region METHOD


    public void encode(byte[] buffer, ref int pos_)
    {
        BaseDLL.encode_uint32(buffer, ref pos_, callNum);
        BaseDLL.encode_uint32(buffer, ref pos_, totalCallNum);
        BaseDLL.encode_uint64(buffer, ref pos_, timeStamp);
        BaseDLL.encode_uint32(buffer, ref pos_, randCallNum);
        BaseDLL.encode_uint32(buffer, ref pos_, randSeed);
        BaseDLL.encode_int8(buffer, ref pos_, (byte)paramDatas.Length);
        for (int i = 0; i < paramDatas.Length; i++)
        {
            BaseDLL.encode_int32(buffer, ref pos_, paramDatas[i]);
        }
        BaseDLL.encode_int8(buffer, ref pos_, (byte)paramStrings.Length);
        for (int i = 0; i < paramStrings.Length; i++)
        {
            if (paramStrings[i] == null)
            {
                paramStrings[i] = string.Empty;
            }
            byte[] nameBytes = StringHelper.StringToUTF8Bytes(paramStrings[i]);
            BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
        }
    }

    public void decode(byte[] buffer, ref int pos_)
    {
        BaseDLL.decode_uint32(buffer, ref pos_, ref callNum);
        BaseDLL.decode_uint32(buffer, ref pos_, ref totalCallNum);
        BaseDLL.decode_uint64(buffer, ref pos_, ref timeStamp);
        BaseDLL.decode_uint32(buffer, ref pos_, ref randCallNum);
        BaseDLL.decode_uint32(buffer, ref pos_, ref randSeed);
        byte paramDatasCnt = 0;
        BaseDLL.decode_int8(buffer, ref pos_, ref paramDatasCnt);
        paramDatas = new int[paramDatasCnt];
        for (int i = 0; i < paramDatas.Length; i++)
        {
            BaseDLL.decode_int32(buffer, ref pos_, ref paramDatas[i]);
        }
        byte paramStringCnt = 0;
        BaseDLL.decode_int8(buffer, ref pos_, ref paramStringCnt);
        paramStrings = new string[paramStringCnt];
        for(int i = 0; i < paramStrings.Length;i++)
        {
            UInt16 nameLen = 0;
            BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
            byte[] nameBytes = new byte[nameLen];
            for (int j = 0; j < nameLen; j++)
            {
                BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[j]);
            }
            paramStrings[i] = StringHelper.BytesToString(nameBytes);
        }
    }

    public void encode(MapViewStream buffer, ref int pos_)
    {
        BaseDLL.encode_uint32(buffer, ref pos_, callNum);
        BaseDLL.encode_uint32(buffer, ref pos_, totalCallNum);
        BaseDLL.encode_uint64(buffer, ref pos_, timeStamp);
        BaseDLL.encode_uint32(buffer, ref pos_, randCallNum);
        BaseDLL.encode_uint32(buffer, ref pos_, randSeed);
        BaseDLL.encode_int8(buffer, ref pos_, (byte)paramDatas.Length);
        for (int i = 0; i < paramDatas.Length; i++)
        {
            BaseDLL.encode_int32(buffer, ref pos_, paramDatas[i]);
        }
        BaseDLL.encode_int8(buffer, ref pos_, (byte)paramStrings.Length);
        for (int i = 0; i < paramStrings.Length; i++)
        {
            if (paramStrings[i] == null)
            {
                paramStrings[i] = string.Empty;
            }
            byte[] nameBytes = StringHelper.StringToUTF8Bytes(paramStrings[i]);
            BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
        }
    }

    public void decode(MapViewStream buffer, ref int pos_)
    {
        BaseDLL.decode_uint32(buffer, ref pos_, ref callNum);
        BaseDLL.decode_uint32(buffer, ref pos_, ref totalCallNum);
        BaseDLL.decode_uint64(buffer, ref pos_, ref timeStamp);
        BaseDLL.decode_uint32(buffer, ref pos_, ref randCallNum);
        BaseDLL.decode_uint32(buffer, ref pos_, ref randSeed);
        byte paramDatasCnt = 0;
        BaseDLL.decode_int8(buffer, ref pos_, ref paramDatasCnt);
        paramDatas = new int[paramDatasCnt];
        for (int i = 0; i < paramDatas.Length; i++)
        {
            BaseDLL.decode_int32(buffer, ref pos_, ref paramDatas[i]);
        }
        byte paramStringCnt = 0;
        BaseDLL.decode_int8(buffer, ref pos_, ref paramStringCnt);
        paramStrings = new string[paramStringCnt];
        for (int i = 0; i < paramStrings.Length; i++)
        {
            UInt16 nameLen = 0;
            BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
            byte[] nameBytes = new byte[nameLen];
            for (int j = 0; j < nameLen; j++)
            {
                BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[j]);
            }
            paramStrings[i] = StringHelper.BytesToString(nameBytes);
        }
    }
    #endregion

}
/// <summary>
/// 水印信息
/// </summary>
/// 
public class MarkData
{
    public UInt32 id;
    public NodeData[] markDatas = Array.Empty<NodeData>();
    public void Recycle()
    {
        id = 0;
        for(int i = 0; i < markDatas.Length;i++)
        {
            GameClient.MarkNodeDataPool.Release(markDatas[i]);
        }
        markDatas = Array.Empty<NodeData>();
    }

    #region METHOD


    public void encode(byte[] buffer, ref int pos_)
    {
        BaseDLL.encode_uint32(buffer, ref pos_, id);
        BaseDLL.encode_uint32(buffer, ref pos_, (UInt32)markDatas.Length);
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i].encode(buffer, ref pos_);
        }
    }

    public void decode(byte[] buffer, ref int pos_)
    {
        BaseDLL.decode_uint32(buffer, ref pos_, ref id);
        UInt32 callDatasCnt = 0;
        BaseDLL.decode_uint32(buffer, ref pos_, ref callDatasCnt);
        markDatas = new NodeData[callDatasCnt];
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i] = new NodeData();
            markDatas[i].decode(buffer, ref pos_);
        }
    }

    public void encode(MapViewStream buffer, ref int pos_)
    {
        BaseDLL.encode_uint32(buffer, ref pos_, id);
        BaseDLL.encode_uint32(buffer, ref pos_, (UInt32)markDatas.Length);
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i].encode(buffer, ref pos_);
        }
    }

    public void decode(MapViewStream buffer, ref int pos_)
    {
        BaseDLL.decode_uint32(buffer, ref pos_, ref id);
        UInt32 callDatasCnt = 0;
        BaseDLL.decode_uint32(buffer, ref pos_, ref callDatasCnt);
        markDatas = new NodeData[callDatasCnt];
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i] = new NodeData();
            markDatas[i].decode(buffer, ref pos_);
        }
    }
    #endregion

}
/// <summary>
/// 当前帧水印信息
/// </summary>
public class FrameMarkData
{
    public UInt32 frame;
    public UInt32 sequence;
    public MarkData[] markDatas = new MarkData[0];
    public void Recycle()
    {
        frame = 0;
        sequence = 0;
        for(int i = 0; i < markDatas.Length;i++)
        {
            GameClient.MarkDataPool.Release(markDatas[i]);
        }
        markDatas = Array.Empty<MarkData>();
    }

    #region METHOD


    public void encode(byte[] buffer, ref int pos_)
    {
        BaseDLL.encode_uint32(buffer, ref pos_, frame);
        BaseDLL.encode_uint32(buffer, ref pos_, sequence);
        BaseDLL.encode_uint32(buffer, ref pos_, (UInt32)(markDatas.Length));
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i].encode(buffer, ref pos_);
        }
    }

    public void decode(byte[] buffer, ref int pos_)
    {
        BaseDLL.decode_uint32(buffer, ref pos_, ref frame);
        BaseDLL.decode_uint32(buffer, ref pos_, ref sequence);
        UInt32 markDatasCnt = 0;
        BaseDLL.decode_uint32(buffer, ref pos_, ref markDatasCnt);
        markDatas = new MarkData[markDatasCnt];
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i] = new MarkData();
            markDatas[i].decode(buffer, ref pos_);
        }
    }

    public void encode(MapViewStream buffer, ref int pos_)
    {
        BaseDLL.encode_uint32(buffer, ref pos_, frame);
        BaseDLL.encode_uint32(buffer, ref pos_, sequence);
        BaseDLL.encode_uint32(buffer, ref pos_, (UInt32)markDatas.Length);
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i].encode(buffer, ref pos_);
        }
    }

    public void decode(MapViewStream buffer, ref int pos_)
    {
        BaseDLL.decode_uint32(buffer, ref pos_, ref frame);
        BaseDLL.decode_uint32(buffer, ref pos_, ref sequence);
        UInt32 markDatasCnt = 0;
        BaseDLL.decode_uint32(buffer, ref pos_, ref markDatasCnt);
        markDatas = new MarkData[markDatasCnt];
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i] = new MarkData();
            markDatas[i].decode(buffer, ref pos_);
        }
    }
    #endregion

}

public class MarkFileData
{
    public UInt64 sessionId = 0;
    public byte battleType = 0;
    public byte dungeonMode = 0;
    public string version = string.Empty;
    public FrameMarkData[] markDatas = new FrameMarkData[0];
    public void Recycle()
    {
        sessionId = 0;
        battleType = 0;
        dungeonMode = 0;
        version = string.Empty;
        for (int i = 0;i < markDatas.Length;i++)
        {
            GameClient.FrameMarkDataPool.Release(markDatas[i]);
        }
        markDatas = Array.Empty<FrameMarkData>();
    }
    
    #region METHOD


    public void encode(byte[] buffer, ref int pos_)
    {
        BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
        BaseDLL.encode_int8(buffer, ref pos_, battleType);
        BaseDLL.encode_int8(buffer, ref pos_, dungeonMode);
        byte[] nameBytes = StringHelper.StringToUTF8Bytes(version);
        BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
        BaseDLL.encode_uint32(buffer, ref pos_, (UInt32)markDatas.Length);
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i].encode(buffer, ref pos_);
        }
    }

    public void decode(byte[] buffer, ref int pos_)
    {
        BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
        BaseDLL.decode_int8(buffer, ref pos_,ref battleType);
        BaseDLL.decode_int8(buffer, ref pos_,ref dungeonMode);
        UInt16 nameLen = 0;
        BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
        byte[] nameBytes = new byte[nameLen];
        for (int j = 0; j < nameLen; j++)
        {
            BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[j]);
        }
        version = StringHelper.BytesToString(nameBytes);
        UInt32 markDatasCnt = 0;
        BaseDLL.decode_uint32(buffer, ref pos_, ref markDatasCnt);
        markDatas = new FrameMarkData[markDatasCnt];
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i] = new FrameMarkData();
            markDatas[i].decode(buffer, ref pos_);
        }
    }

    public void encode(MapViewStream buffer, ref int pos_)
    {
        BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
        BaseDLL.encode_int8(buffer, ref pos_, battleType);
        BaseDLL.encode_int8(buffer, ref pos_, dungeonMode);
        byte[] nameBytes = StringHelper.StringToUTF8Bytes(version);
        BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
        BaseDLL.encode_uint32(buffer, ref pos_, (UInt32)markDatas.Length);
        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i].encode(buffer, ref pos_);
        }
    }

    public void decode(MapViewStream buffer, ref int pos_)
    {
        BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
        BaseDLL.decode_int8(buffer, ref pos_, ref battleType);
        BaseDLL.decode_int8(buffer, ref pos_, ref dungeonMode);
        UInt16 nameLen = 0;
        BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
        byte[] nameBytes = new byte[nameLen];
        for (int j = 0; j < nameLen; j++)
        {
            BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[j]);
        }
        version = StringHelper.BytesToString(nameBytes);
        UInt32 markDatasCnt = 0;
        BaseDLL.decode_uint32(buffer, ref pos_, ref markDatasCnt);
        markDatas = new FrameMarkData[markDatasCnt];

        for (int i = 0; i < markDatas.Length; i++)
        {
            markDatas[i] = new FrameMarkData();
            markDatas[i].decode(buffer, ref pos_);
        }
    }
    #endregion

}
