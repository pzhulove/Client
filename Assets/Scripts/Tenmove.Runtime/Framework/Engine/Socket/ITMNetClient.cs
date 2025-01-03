
namespace Tenmove.Runtime
{
    public interface ITMNetClient : ITMNetNeuron
    {
        void SendMessage<T>(T message) where T : NetMessage;
    }
}