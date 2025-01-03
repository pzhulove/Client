
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Tenmove.Runtime
{
    internal partial class NetServer
    {
        private class Connection : ITMNetConnection
        {
            private readonly NetServer m_Server;
            private readonly NetSocket m_Socket;
            private readonly string m_IP;
            private NetConnectState m_State;
            private long m_LastOverTime;

            public Connection(NetServer server, NetSocket socket, string ip)
            {
                Debugger.Assert(null != server, "Parameter 'server' can not be null!");
                Debugger.Assert(null != socket, "Parameter 'socket' can not be null!");
                Debugger.Assert(!string.IsNullOrEmpty(ip), "Parameter 'ip' can not be null or empty string!");
                
                m_Server = server;
                m_Socket = socket;
                m_IP = ip;
                m_LastOverTime = Utility.Time.GetTicksNow();

                m_Server.RegisterMessageProcessor<NetMessageHeartBeat>(_OnReceiveHeartBeat);
                m_State = NetConnectState.Connecting;
            }

            public NetConnectState State
            {
                get { return m_State; }
            }

            public string IP
            {
                get { return m_IP; }
            }

            public void ReceiveMessage()
            {
                m_Socket.ReceiveMessage();
            }

            public void SendMessage(ByteMessage message)
            {
                m_Socket.PushMessage(message);
            }

            public void Update()
            {
                m_Socket.Update();

                long ticks = Utility.Time.GetTicksNow();
                do
                {
                    if (!_ProcessOneMessage())
                        break;
                }
                while (Utility.Time.TicksToMicroseconds(Utility.Time.GetTicksNow() - ticks) < 1);

                if(Utility.Time.TicksToSeconds(ticks - m_LastOverTime) > 10)
                {
                    m_LastOverTime = ticks;
                    if (m_State < NetConnectState.Disconnected)
                        ++m_State;

                    Debugger.LogWarning("Connection is overtime more than 10 seconds! Current state:{0}", m_State);
                }
            }

            public void Shutdown()
            {
                m_Socket.Shutdown();
            }

            protected bool _ProcessOneMessage()
            {
                ByteMessage byteMessage = m_Socket.FetchMessage();
                if (null != byteMessage)
                {
                    NetMessage netMessage = m_Server._Decode<NetMessage>(byteMessage);
                    if (null != netMessage)
                    {
                        List<IProcessorDesc> descList = null;
                        if (m_Server.m_ProcessorRegisterTable.TryGetValue(netMessage.ID, out descList))
                        {
                            for (int i = 0, icnt = descList.Count; i < icnt; ++i)
                                descList[i].Process(netMessage);
                        }
                        else
                            Debugger.LogWarning("Can not find message processor with message ID:{0}", netMessage.ID);
                    }
                    else
                        Debugger.LogWarning("Decode byte message with ID:{0} has failed!", byteMessage.ID);

                    return true;
                }

                return false;
            }

            protected void _OnReceiveHeartBeat(NetMessageHeartBeat heartBeat)
            {
                m_State = NetConnectState.Connecting;
                m_LastOverTime = Utility.Time.GetTicksNow();
            }
        }
    }
}