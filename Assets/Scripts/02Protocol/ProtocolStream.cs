using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace Protocol
{
    public interface IProtocolStream
    {
        void encode(byte[] buffer, ref int pos);
        void decode(byte[] buffer, ref int pos);
    }

    public interface IGetMsgID
    {
        UInt32 GetMsgID();
        UInt32 GetSequence();
        void SetSequence(UInt32 sequence);
    }

    public static class MsgTools
    {
        public static void decode(this Protocol.IProtocolStream stream, byte[] buffer)
        {
            int pos = 0;
            stream.decode(buffer, ref pos);
        }
        
        public static T decode<T>(this Network.MsgDATA msg) where T : Protocol.IProtocolStream,new()
        {
            T protocol = new T();
            protocol.decode(msg.bytes);
            return protocol;
        }

        public static void encode(this Protocol.IProtocolStream stream, byte[] buffer)
        {
            int pos = 0;
            stream.encode(buffer, ref pos);
        }

        public static string toString(this SockAddr addr)
        {
            StringBuilder builder = new StringBuilder();
            builder = builder.AppendFormat("Server IP:{0},Port:{1}", addr.ip, addr.port);
            return builder.ToString();
        }
    }
}
