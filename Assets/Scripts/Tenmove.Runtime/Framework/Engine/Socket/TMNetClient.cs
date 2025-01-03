
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tenmove.Runtime
{
    internal class NetClient : NetNeuron,ITMNetClient
    {
        static private readonly int DefaultReconnectCount = 3;

        private readonly NetMessageHeartBeat m_HeartBeatMessage;

        private int m_ReconnectCount;
        private long m_TimeStamp;

        public NetClient(NetIPAddress ip,int port, uint cacheSize)
            :base(ip,port, cacheSize)
        {
            m_HeartBeatMessage = CreateMessage<NetMessageHeartBeat>();
            m_TimeStamp = 0;
            m_ReconnectCount = DefaultReconnectCount;
        }

        public NetClient(int port, uint cacheSize)
            : this(NetIPAddress.InvalidAddress, port, cacheSize)
        {
        }

        public void SendMessage<T>(T message) where T : NetMessage
        {
            if (null != message)
            {
                ByteMessage byteMessage = _Encode(message);
                m_Socket.PushMessage(byteMessage);
            }
            else
                Debugger.LogWarning("Parameter 'message' can not be null!");
        }

        protected override void _OnStart()
        {
            _BeginConnect();
        }

        protected override void _OnShutdown()
        {
        }

        protected override void _OnUpdate()
        {
            base._OnUpdate();

            long tickNow = Utility.Time.GetTicksNow();

            if (m_Socket.IsConnected)
            {
                if (Utility.Time.TicksToMicroseconds(tickNow - m_TimeStamp) > 10)
                {
                    SendMessage(m_HeartBeatMessage);
                    m_TimeStamp = tickNow;
                }
            }
            else
                _BeginConnect();
        }

        private void _BeginConnect()
        {
            if (State == NeuronState.Running)
            {
                IPEndPoint endPoint = new IPEndPoint(m_IPAddress, m_Port);
                m_State = NeuronState.Connecting;
                m_Socket.BeginConnect(endPoint, _Thread_OnConnected, this);
            }
        }

        static private void _Thread_OnConnected(IAsyncResult ar)
        {
            if (null != ar)
            {
                NetClient _this = (NetClient)ar.AsyncState;
                if (null != _this)
                {
                    try
                    {
                        _this.m_Socket.EndConnect(ar);
                        if (NeuronState.Shutdown == _this.State)
                            return;

                        Debugger.LogInfo("Connect to server:{0}", _this.m_Socket.RemoteEndPoint.ToString());
                        _this.m_ReconnectCount = DefaultReconnectCount;
                        _this.m_Socket.ReceiveMessage();

                        return;
                    }
                    catch (Exception e)
                    {
                        Debugger.LogException("Connect to server with exception:{0}", e.Message);
                        if (_this.m_ReconnectCount > 0)
                        {
                            System.Threading.Thread.Sleep(1000);
                            --_this.m_ReconnectCount;
                            _this._BeginConnect();
                        }
                        else
                            _this.m_State = NeuronState.Ready;
                    }
                }
                else
                    Debugger.LogWarning("Socket connection parameter 'IAsyncResult.AsyncState' is null or not a NetClient instance, Connection has Failed!");
            }
            else
                Debugger.LogWarning("Socket connection parameter 'IAsyncResult' is null, Connection has failed!");
        }    
    }
}