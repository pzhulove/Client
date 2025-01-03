
namespace Tenmove.Runtime
{
    internal class NetManager : BaseModule,ITMNetManager
    {
        private readonly ITMNetMessageInterpreter m_NetMessageInterpreter;

        public NetManager()
        {
            m_NetMessageInterpreter = ModuleManager.GetModule<ITMNetMessageInterpreter>();
        }

        public void RegisterMessage<T>() where T : NetMessage, new()
        {
            if (null != m_NetMessageInterpreter)
                m_NetMessageInterpreter.RegisterMessage<T>();
        }

        public ITMNetClient CreateClient(NetIPAddress ip, int port, uint cacheSize)
        {
            return new NetClient(ip, port, cacheSize);
        }

        public ITMNetClient CreateClient(int port, uint cacheSize)
        {
            return CreateClient(NetIPAddress.InvalidAddress, port, cacheSize);
        }

        public ITMNetServer CreateServer(NetIPAddress ip, int port, uint cacheSize, int backlog)
        {
            return new NetServer(ip, port, cacheSize, backlog);
        }

        public ITMNetServer CreateServer(int port, uint cacheSize, int backlog)
        {
            return CreateServer(NetIPAddress.InvalidAddress, port, cacheSize, backlog);
        }

        internal override int Priority { get { return 0; } }
        internal override void Shutdown() { }
        internal override void Update(float elapseSeconds, float realElapseSeconds) { }
    }
}