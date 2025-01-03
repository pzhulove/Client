

namespace Tenmove.Runtime
{
    public interface ITMNetManager
    {
        void RegisterMessage<T>() where T : NetMessage, new();

        ITMNetClient CreateClient(NetIPAddress ip, int port, uint cacheSize);
        ITMNetClient CreateClient(int port, uint cacheSize);
        ITMNetServer CreateServer(NetIPAddress ip, int port, uint cacheSize, int backlog = 10);
        ITMNetServer CreateServer(int port, uint cacheSize, int backlog = 10);
    }
}