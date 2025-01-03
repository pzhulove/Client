using System;
using System.Net;

namespace Network
{
    public class PacketBuffer
    {
        // 一个环形缓冲器，[startPos, endPos)
        private byte[] m_Buffer = new byte[UInt16.MaxValue + 1];
        private int m_StartPos;
        private int m_EndPos;
        private int m_BufferLength = 0;
        private int m_LengthMask;

        public PacketBuffer()
        {
            m_BufferLength = m_Buffer.Length;
            m_LengthMask = m_BufferLength - 1;
        }

        public void WritePacket(UInt32 msgId, UInt32 sequence, byte[] data, UInt16 size)
        {
            // 空间不足，需要空出足够的空间
            int freeSize = GetFreeSize();
            int needSize = size + (int)NET_DEFINE.HEADER_SIZE;
            int start = m_StartPos;
            int end = m_EndPos;
            if (freeSize < needSize)
            {
                Free(needSize);
#if !LOGIC_SERVER 
                freeSize = GetFreeSize();
                //如果缓冲区已经满，不能在往缓冲区中写入数据了
                if (GetFreeSize() < needSize)
                {
                    string str = string.Format("Try Free {0} Size Failure {1} In {2} Seq {3}",freeSize,needSize,msgId,sequence);
                    Logger.LogError(str);
                    RecordServer.instance.PushReconnectCmd(str);
                    return;
                }
                RecordServer.instance.PushReconnectCmd(string.Format("[WritePacket Free] ---- store msg:{0} sequence:{1} size:{2} freesize:{7} needsize:{8}, ({3},{4}) -> ({5},{6})", msgId, sequence, size, start, end, m_StartPos, m_EndPos, freeSize, needSize));

#endif
            }
            start = m_StartPos;
            end = m_EndPos;

            WriteShort((short)size);
            WriteInt32((int)msgId);
            WriteInt32((int)sequence);
            WriteArray(data, size);
#if !LOGIC_SERVER && MG_TEST
          //  if (ClientApplication.isOpenNewReportFrameAlgo)
            {
                RecordServer.instance.PushReconnectCmd(string.Format("[WritePacket] ---- store msg:{0} sequence:{1} size:{2}  freesize:{7} needsize:{8}, ({3},{4}) -> ({5},{6})", msgId, sequence, size, start, end, m_StartPos, m_EndPos, freeSize, needSize));
                RecordServer.instance.PushReconnectCmd("[WritePacket] PUSH CMD Begin -----");
                if (msgId == Protocol.WorldDungeonReportFrameReq.MsgID)
                {
                    var req = new Protocol.WorldDungeonReportFrameReq();
                    int pos = 0;
                    req.decode(data, ref pos);
                    for (int i = 0; i < req.frames.Length; i++)
                    {
                        for (int j = 0; j < req.frames[i].data.Length; j++)
                        {
                            var cmd = GameClient.FrameCommandFactory.CreateCommand(req.frames[i].data[j].input.data1);
                            cmd.SetValue(req.frames[i].sequence, req.frames[i].data[j].seat, req.frames[i].data[j].input);
                            RecordServer.instance.PushReconnectCmd(string.Format("[WritePacket] PUSH CMD:{0}", cmd.GetString()));
                        }
                    }
                }
                RecordServer.instance.PushReconnectCmd("[WritePacket] PUSH CMD End -----");
            }
#endif

#if UNITY_EDITOR && NET_LOG
            Logger.LogErrorFormat("store msg:{0} sequence:{1} size:{2}, ({3},{4}) -> ({5},{6})", msgId, sequence, size, start, end, m_StartPos, m_EndPos);
#endif
        }

        public int FetchPacket(UInt32 startSequence, byte[] data)
        {
#if !LOGIC_SERVER && MG_TEST
            RecordServer.instance.PushReconnectCmd(string.Format("Reconnect Begin sequence {0} UsedSize {1} -------", startSequence,GetUsedSize()));
#endif
            if (startSequence == 0 || GetUsedSize() == 0)
            {
                return 0;
            }

            int len = 0;
            int start = m_StartPos;
            int end = m_EndPos < m_StartPos ? m_EndPos + m_BufferLength : m_EndPos;

            bool hasSequence = false;

            while (start < end)
            {
                int tmpStart = start;
                short msgLen = ReadShort(start);
                int msgId = ReadInt32(start + 2);
                int sequence = ReadInt32(start + 6);

                if(sequence <= startSequence)
                {
                    start += msgLen + (int)NET_DEFINE.HEADER_SIZE;
                    if (sequence == startSequence)
                    {
                        hasSequence = true;
                    }
                    continue;
                }

                if (sequence == startSequence + 1)
                {
                    hasSequence = true;
                }

#if !LOGIC_SERVER && MG_TEST
                //  if (ClientApplication.isOpenNewReportFrameAlgo)
                {
                    if (msgId == Protocol.WorldDungeonEnterRaceReq.MsgID)
                    {

                    }
                    else if(msgId == Protocol.SceneDungeonEnterNextAreaReq.MsgID)
                    {

                    }
                    else if (msgId == Protocol.WorldDungeonReportFrameReq.MsgID)
                    {

                        var req = new Protocol.WorldDungeonReportFrameReq();
                        int pos = start + 10;
                        req.decode(m_Buffer, ref pos);
                        for (int i = 0; i < req.frames.Length; i++)
                        {
                            for (int j = 0; j < req.frames[i].data.Length; j++)
                            {
                                if (req.frames[i].data[j] == null || req.frames[i].data[j].input == null)
                                {
                                    Logger.LogErrorFormat("input data is invalid {0}--{1}--{2}--{3}", start, end, pos, i, j);
                                    continue;
                                }
                                var cmd = GameClient.FrameCommandFactory.CreateCommand(req.frames[i].data[j].input.data1);
                                if (cmd == null)
                                {
                                    Logger.LogErrorFormat("cmd type is error :{0}", req.frames[i].data[j].input.data1);
                                    continue;
                                }
                                cmd.SetValue(req.frames[i].sequence, req.frames[i].data[j].seat, req.frames[i].data[j].input);
                                RecordServer.instance.PushReconnectCmd(string.Format("[CMD]PUSH CMD:{0}", cmd.GetString()));

                            }
                        }
                    }
                    else if(msgId == Protocol.SceneDungeonClearAreaMonsters.MsgID)
                    {

                    }
                    else if(msgId == Protocol.RelaySvrDungeonRaceEndReq.MsgID)
                    {

                    }
                }
#endif

#if UNITY_EDITOR && NET_LOG
                Logger.LogErrorFormat("[RECON] fetch msg:{0} sequence:{1} len:{2}", msgId, sequence, msgLen);
#endif
                                for (int i = 0; i < msgLen + (int)NET_DEFINE.HEADER_SIZE; i++)
                {
                    data[len++] = ReadByte(start++);
                    if(len >= data.Length)
                    {
                        return -1;
                    }
                }
            }
#if !LOGIC_SERVER && MG_TEST
          //  if (ClientApplication.isOpenNewReportFrameAlgo)
            {
                RecordServer.instance.PushReconnectCmd("Reconnect End-------");
            }
#endif
            if (hasSequence)
            {
                return len;
            }
            else
            {
                return -1;
            }
        }

        private void WriteInt32(int value)
        {
            value = IPAddress.HostToNetworkOrder(value);
            for (int i = 0; i < 4; i++)
            {
                WriteByte((byte)((value >> (i * 8)) & 0xff));
            }
        }

        private void WriteShort(short value)
        {
            value = IPAddress.HostToNetworkOrder(value);
            WriteByte((byte)((value >> 0) & 0xff));
            WriteByte((byte)((value >> 8) & 0xff));
        }

        private void WriteByte(byte value)
        {
            m_Buffer[m_EndPos++] = value;
            m_EndPos = m_EndPos & m_LengthMask;
        }

        private void WriteArray(byte[] data, UInt16 size)
        {
            for (int i = 0; i < size; i++)
            {
                WriteByte(data[i]);
            }
        }

        private int ReadInt32(int pos)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                value |= ReadByte(pos++) << (i * 8);
            }

            value = IPAddress.HostToNetworkOrder(value);
            return value;
        }

        private short ReadShort(int pos)
        {
            short value = 0;
            for (int i = 0; i < 2; i++)
            {
                value |= (short)(ReadByte(pos++) << (i * 8));
            }

            value = IPAddress.HostToNetworkOrder(value);
            return value;
        }

        private byte ReadByte(int pos)
        {
            pos &= m_LengthMask;
            byte value = m_Buffer[pos];
            return value;
        }

        // 获取已经使用了的空间大小
        public int GetUsedSize()
        {
            if (m_EndPos < m_StartPos)
            {
                return m_EndPos + m_BufferLength - m_StartPos;
            }
            else
            {
                return m_EndPos - m_StartPos;
            }
        }

        // 获取剩余的空间大小
        private int GetFreeSize()
        {
            return m_BufferLength - GetUsedSize();
        }

        // 清理出最起码size大小的空间
        private void Free(int size)
        {
            int start = m_StartPos;
            int end = m_EndPos < m_StartPos ? m_EndPos + m_BufferLength : m_EndPos;

            // 这里加个容错，防止死循环。。。
            // 照理说是不可能的
            // 尝试100次之后就把整个缓冲区清空

            int tryCount = 1000;
            while (GetFreeSize() < size)
            {
                if (--tryCount <= 0)
                {
                    break;
                }

                int tmpStart = start;
                short msgLen = ReadShort(m_StartPos);
                m_StartPos = (m_StartPos + msgLen + (int)NET_DEFINE.HEADER_SIZE) & m_LengthMask;
            }

            if (GetFreeSize() < size)
            {
                Clear();
            }
        }

        public void Clear()
        {
            m_StartPos = m_EndPos = 0;
        }
    }
}

