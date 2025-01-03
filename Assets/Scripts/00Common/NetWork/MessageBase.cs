using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace Network
{
    enum NET_DEFINE
    {
        HEADER_LENGTH_PART_SIZE = 2,    // 包头中长度字段的字节数
        HEADER_MSGID_PART_SIZE = 4,     // 包头中消息号字段的字节数
        HEADER_SEQUENCE_PART_SIZE = 4,  // 包头中消息序号字段的字节数
        HEADER_SIZE = HEADER_LENGTH_PART_SIZE + HEADER_MSGID_PART_SIZE + HEADER_SEQUENCE_PART_SIZE,                // 包头的字节数
        COMPRESS_FLAG = 1 << 31,

    }
    public class MsgDATA
    {
        public ServerType serverType;
        public uint id;
        public uint sequence;
        public byte[] bytes;

        public MsgDATA(int length)
        {
            bytes = new byte[length];
        }
    }
}

