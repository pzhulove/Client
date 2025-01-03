

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tenmove.Runtime
{
    /// <summary>
    /// @Description: 
    ///     Minimum unit for net function.
    /// @Author:Simon.King
    /// @Date:2020.04.04
    /// </summary>
    internal abstract class NetNeuron : ITMNetNeuron
    {
        public enum NeuronState
        {
            Ready,
            Running,
            Connecting,
            Connected,
            Shutdown,
        }

        protected interface IProcessorDesc
        {
            void Process(NetMessage message);
            bool Equals(IProcessorDesc other);
        }

        private class ProcessorDesc<T, U> : IProcessorDesc where T : NetMessage where U : IEquatable<U>
        {
            private readonly MessageProcessor<T, U> m_Processor;
            private readonly U m_UserData;

            public ProcessorDesc(MessageProcessor<T, U> processor,U userData)
            {
                Debugger.Assert(null != processor, "Parameter 'processor' can not be null!");
                m_Processor = processor;
                m_UserData = userData;
            }

            public void Process(NetMessage message)
            {
                if (null != m_Processor)
                    m_Processor(message as T, m_UserData);
                else
                    Debugger.LogWarning("Processor delegate instance for message '{0}' is null!", typeof(T));
            }

            public bool Equals(IProcessorDesc other)
            {
                if (null != other)
                {
                    if(this.GetType() == other.GetType())
                    {
                        ProcessorDesc<T, U> otherInst = other as ProcessorDesc<T, U>;
                        return m_Processor == otherInst.m_Processor && m_UserData.Equals(otherInst.m_UserData);
                    }
                }

                return false;
            }
        }

        private class ProcessorDesc<T> : IProcessorDesc where T : NetMessage
        {
            private readonly MessageProcessor<T> m_Processor;

            public ProcessorDesc(MessageProcessor<T> processor)
            {
                Debugger.Assert(null != processor, "Parameter 'processor' can not be null!");
                m_Processor = processor;
            }

            public void Process(NetMessage message)
            {
                if (null != m_Processor)
                    m_Processor(message as T);
                else
                    Debugger.LogWarning("Processor delegate instance for message '{0}' is null!", typeof(T));
            }

            public bool Equals(IProcessorDesc other)
            {
                if (null != other)
                {
                    if (this.GetType() == other.GetType())
                        return m_Processor == (other as ProcessorDesc<T>).m_Processor;
                }

                return false;
            }
        }

        private readonly ITMNetMessageInterpreter m_MessageInterpreter;
        private readonly string m_IPEndPoint;
        protected readonly System.Net.IPAddress m_IPAddress;
        protected readonly NetSocket m_Socket;
        protected readonly int m_Port;

        protected readonly Dictionary<uint, List<IProcessorDesc>> m_ProcessorRegisterTable;

        protected NeuronState m_State;
        private readonly uint m_CacheSize;

        public NetNeuron(NetIPAddress ipAddress,int port,uint cacheSize)
            //: base(new System.Net.NetIPAddress(ipAddress.Value), cacheSize)
        {
            Debugger.Assert(port < 65536, string.Format("Invalid port value '{0}', Valid port range [0,65535]!", port));
            Debugger.Assert(port > 1023, string.Format("Port:{0} is reserve port for system, You can not use it!", port));

            m_ProcessorRegisterTable = new Dictionary<uint, List<IProcessorDesc>>();

            m_MessageInterpreter = ModuleManager.GetModule<ITMNetMessageInterpreter>();

            if(NetIPAddress.InvalidAddress == ipAddress)
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
                {
                    if (ipHostInfo.AddressList[i].AddressFamily.Equals(AddressFamily.InterNetwork))
                        ipAddress = new NetIPAddress(ipHostInfo.AddressList[i].GetAddressBytes());
                }
            }

            m_IPAddress = new System.Net.IPAddress(ipAddress.Value);
            m_Socket = new NetSocket(new Socket(m_IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp), cacheSize);
            m_CacheSize = m_Socket.CacheSize;
            m_Port = port;
            m_IPEndPoint = string.Format("{0}:{1}", m_IPAddress.ToString(), m_Port);
            m_State = NeuronState.Ready;
        }

        ~NetNeuron()
        {
            Shutdown();
        }

        public NeuronState State
        {
            get { return m_State; }
        }

        public string IPEndPoint
        {
            get { return m_IPEndPoint; }
        }

        public bool IsConnected
        {
            get { return m_Socket.IsConnected; }
        }

        public T CreateMessage<T>() where T : NetMessage
        {
            return m_MessageInterpreter.CreateMessage<T>();
        }

        public NetMessage CreateMessage(Type netMessageType)
        {
            return m_MessageInterpreter.CreateMessage(netMessageType);
        }

        public bool RegisterMessageProcessor<T,U>(MessageProcessor<T,U> processor,U userData) 
            where T:NetMessage where U : IEquatable<U>
        {
            if (null != processor)
            {
                ProcessorDesc<T, U> processorDesc = new ProcessorDesc<T, U>(processor, userData);
                _AddRegisterMessageDesc<T>(processorDesc);
            }
            else
                Debugger.LogWarning("Parameter 'processor' can not be null!");

            return false;
        }

        public bool RegisterMessageProcessor<T>(MessageProcessor<T> processor) where T : NetMessage
        {
            if (null != processor)
            {
                ProcessorDesc<T> processorDesc = new ProcessorDesc<T>(processor);
                _AddRegisterMessageDesc<T>(processorDesc);
            }
            else
                Debugger.LogWarning("Parameter 'processor' can not be null!");

            return false;
        }

        public void UnregisterMessageProcessor<T>() where T : NetMessage
        {
            uint netMsgID = m_MessageInterpreter.QureyMessageID<T>();
            if (m_ProcessorRegisterTable.ContainsKey(netMsgID))
                m_ProcessorRegisterTable.Remove(netMsgID);
        }

        public void Start()
        {
            m_State = NeuronState.Running;
            _OnStart();
        }

        public void Update()
        {
            m_Socket.Update();
            _OnUpdate();
        }

        public void Shutdown()
        {
            _OnShutdown();
            m_Socket.Shutdown();
            m_State = NeuronState.Shutdown;
        }

        private void _AddRegisterMessageDesc<T>(IProcessorDesc desc) where T: NetMessage
        {
            uint netMsgID = m_MessageInterpreter.QureyMessageID<T>();
            List<IProcessorDesc> descList = null;
            if (m_ProcessorRegisterTable.TryGetValue(netMsgID, out descList))
            {
                for (int i = 0, icnt = descList.Count; i < icnt; ++i)
                {
                    if (descList[i].Equals(desc))
                        return;
                }
            }
            else
            {
                descList = new List<IProcessorDesc>();
                m_ProcessorRegisterTable.Add(netMsgID, descList);
            }

            descList.Add(desc);
        }

        protected T _Decode<T>(ByteMessage byteMessage) where T: NetMessage
        {
            return m_MessageInterpreter.Decode(byteMessage) as T;
        }

        protected ByteMessage _Encode<T>(T netMessage) where T : NetMessage
        {
            return m_MessageInterpreter.Encode(netMessage, m_CacheSize);
        }

        protected bool _ProcessOneMessage()
        {
            ByteMessage byteMessage = m_Socket.FetchMessage();
            if (null != byteMessage)
            {
                NetMessage netMessage = m_MessageInterpreter.Decode(byteMessage);
                if (null != netMessage)
                {
                    List<IProcessorDesc> descList = null;
                    if (m_ProcessorRegisterTable.TryGetValue(netMessage.ID, out descList))
                    {
                        for(int i = 0,icnt = descList.Count;i<icnt;++i)
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

        protected virtual void _OnUpdate()
        {
            long ticks = Utility.Time.GetTicksNow();
            do
            {
                if (!_ProcessOneMessage())
                    break;
            }
            while (Utility.Time.TicksToMicroseconds(Utility.Time.GetTicksNow() - ticks) < 2);
        }

        protected abstract void _OnStart();
        protected abstract void _OnShutdown();
    }
}