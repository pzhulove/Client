

namespace Tenmove.Runtime
{
    using System.Collections.Generic;

    public enum NetConnectState
    {
        Connecting,
        HeartBeatOverTime,
        Disconnected,
        Closed,
    }

    public interface ITMNetConnection
    {
        NetConnectState State { get; }
        string IP { get; }
    }

    public interface ITMNetServer : ITMNetNeuron
    {
        bool HasConnection { get; }

        void SendMessage<T>(T message, string ip = "") where T : NetMessage;
        void GetConnections(List<ITMNetConnection> connectionsList);
    }
}