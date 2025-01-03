
namespace Tenmove.Runtime
{
    using System;

    internal interface ITMNetMessageInterpreter
    {
        void RegisterMessage<T>() where T : NetMessage, new();

        T CreateMessage<T>() where T : NetMessage;
        NetMessage CreateMessage(Type netMessageType);
        uint QureyMessageID<T>() where T : NetMessage;

        NetMessage Decode(ByteMessage byteMessage);
        ByteMessage Encode(NetMessage netMessage,uint cacheSize);
    }
}