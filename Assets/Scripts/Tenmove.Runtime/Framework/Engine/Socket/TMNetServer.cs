

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tenmove.Runtime
{
    internal partial class NetServer : NetNeuron,ITMNetServer
    {
        private readonly int m_Backlog;
        private readonly List<Connection> m_ConnectionList;
        private readonly Dictionary<string, Connection> m_ConnetionTable;

        public NetServer( NetIPAddress ip, int port,uint cacheSize, int backlog)
            : base(ip, port, cacheSize)
        {
            Debugger.Assert(port < 65536, string.Format("Invalid port value '{0}', Valid port range [0,65535]!", port));
            Debugger.Assert(port > 1023, string.Format("Port:{0} is reserve port for system, You can not use it!", port));

            if (backlog < 0)
            {
                Debugger.LogWarning("Parameter 'backlog' can not be a negative value, auto set to zero!");
                backlog = 0;
            }

            m_Backlog = backlog;
            m_ConnectionList = new List<Connection>();
            m_ConnetionTable = new Dictionary<string, Connection>();
        }

        public NetServer(int port, uint cacheSize, int backlog)
            : this(NetIPAddress.InvalidAddress, port, cacheSize, backlog)
        {
        }

        public NetServer(int port, uint cacheSize)
            : this(NetIPAddress.InvalidAddress, port, cacheSize, 10)
        {
        }

        public bool HasConnection
        {
            get { return m_ConnectionList.Count > 0; }
        }

        public void GetConnections(List<ITMNetConnection> connectionsList)
        {
            int connectionCount = m_ConnectionList.Count;
            if (connectionsList.Capacity < connectionCount)
                connectionsList.Capacity = connectionCount;

            for (int i = 0, icnt = connectionCount; i < icnt; ++i)
                connectionsList.Add(m_ConnectionList[i]);
        }

        public void SendMessage<T>(T message, string ip = "") where T : NetMessage
        {
            if (null != message)
            {
                ByteMessage byteMessage = _Encode(message);
                _SendByteMessageTo(ip, byteMessage);
            }
            else
                Debugger.LogWarning("Parameter 'message' can not be null!");
        }        

        protected override void _OnStart()
        {
            IPEndPoint endPoint = new IPEndPoint(m_IPAddress, m_Port);
            m_Socket.Bind(endPoint);
            m_Socket.Listen(m_Backlog);
            m_Socket.SendTimeout = 3000;
            m_Socket.BeginAccept(_Thread_OnAccept, this);
        }

        protected sealed override void _OnUpdate()
        {
            base._OnUpdate();

            for (int i = 0, icnt = m_ConnectionList.Count; i < icnt; ++i)
            {
                Connection cur = m_ConnectionList[i];
                cur.Update();

                if (NetConnectState.Disconnected == cur.State)
                {
                    cur.Shutdown();
                    _RemoveConnect(cur.IP);
                }
            }
        }

        protected override void _OnShutdown()
        {
            for (int i = 0, icnt = m_ConnectionList.Count; i < icnt; ++i)
                m_ConnectionList[i].Shutdown();

            m_ConnectionList.Clear();
            m_ConnetionTable.Clear();
        }

        private Connection _AddConnection(string ip, NetSocket socket)
        {
            Connection connect = null;
            if (m_ConnetionTable.TryGetValue(ip, out connect))
                return connect;

            connect = new Connection(this, socket,ip);
            m_ConnectionList.Add(connect);
            m_ConnetionTable.Add(ip, connect);
            return connect;
        }

        private void _RemoveConnect(string ip)
        {
            Connection connect = null;
            if (m_ConnetionTable.TryGetValue(ip, out connect))
            {
                for(int i = 0,icnt = m_ConnectionList.Count;i<icnt;++i)
                {
                    Connection cur = m_ConnectionList[i];
                    if(ip == cur.IP)
                    {
                        m_ConnectionList.RemoveAt(i);
                        break;
                    }
                }

                m_ConnetionTable.Remove(ip);
            }
        }

        private void _SendByteMessageTo(string ip,ByteMessage byteMessage)
        {
            if (null != byteMessage)
            {
                if (string.IsNullOrEmpty(ip))
                {
                    for (int i = 0, icnt = m_ConnectionList.Count; i < icnt; ++i)
                        m_ConnectionList[i].SendMessage(byteMessage);
                }
                else
                {
                    Connection connetion = null;
                    if (m_ConnetionTable.TryGetValue(ip, out connetion))
                        connetion.SendMessage(byteMessage);
                    else
                        Debugger.LogWarning("Can not find connection with ip:{0}", ip);
                }
            }
        }

        static private void _Thread_OnAccept(IAsyncResult ar)
        {
            try
            {
                NetServer _this = (NetServer)ar.AsyncState;
                if (NeuronState.Shutdown == _this.State)
                    return;

                NetSocket socket = _this.m_Socket.EndAccept(ar);
                if (null == socket)
                    return;

                string[] ip = socket.RemoteEndPoint.ToString().Split(':');
                if(null != ip)
                {
                    Connection connection = _this._AddConnection(ip[0], socket);
                    connection.ReceiveMessage();
                }

                _this.m_Socket.BeginAccept(_Thread_OnAccept, _this);
            }
            catch (Exception e)
            {
                Debugger.LogException("Accept client with exception:{0}", e.Message);
            }
        }
    }
}