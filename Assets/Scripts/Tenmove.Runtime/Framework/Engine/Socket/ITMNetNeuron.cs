

namespace Tenmove.Runtime
{
    using System;

    public delegate void MessageProcessor<T, U>(T netMessage, U userData) where T : NetMessage;
    public delegate void MessageProcessor<T>(T netMessage) where T : NetMessage;

    public interface ITMNetNeuron
    {
        string IPEndPoint { get; }

        NetMessage CreateMessage(Type netMessageType);
        T CreateMessage<T>() where T : NetMessage;

        bool RegisterMessageProcessor<T, U>(MessageProcessor<T, U> processor, U userData) where T : NetMessage where U : IEquatable<U>;
        bool RegisterMessageProcessor<T>(MessageProcessor<T> processor) where T : NetMessage;
        void UnregisterMessageProcessor<T>() where T : NetMessage;

        void Start();
        void Update();
        void Shutdown();
    }
}