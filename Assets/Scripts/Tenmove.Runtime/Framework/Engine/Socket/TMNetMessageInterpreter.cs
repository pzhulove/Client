

using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal class NetMessageInterpreter : BaseModule,ITMNetMessageInterpreter
    {
        /// <summary>
        /// 这个表的初始化顺序必须客户端服务端要一致，关系到消息ID的分配
        /// </summary>
        static public readonly NetMessageDesc[] NetMessageDescriptions = new NetMessageDesc[]
            {
                NetMessageDesc<NetMessageHeartBeat>.GetDesc(),
                NetMessageDesc<NetMessageTransmitFile>.GetDesc(),
            };

        private readonly Dictionary<uint, NetMessageDesc> m_NetMessageIDDescTable;
        private readonly Dictionary<string, NetMessageDesc> m_NetMessageNameDescTable;
        private readonly NetMessageAllocator m_Allocator;
        private readonly ITMNetMessageBufferPool m_BufferPool;

        public NetMessageInterpreter()
        {     
            m_Allocator = new NetMessageAllocator();
            m_BufferPool = ModuleManager.GetModule<ITMNetMessageBufferPool>();
            m_NetMessageIDDescTable = new Dictionary<uint, NetMessageDesc>();
            m_NetMessageNameDescTable = new Dictionary<string, NetMessageDesc>();
            for (int i = 0,icnt = NetMessageDescriptions.Length;i<icnt;++i)
            {
                NetMessageDesc cur = NetMessageDescriptions[i];
                uint id = m_Allocator.AcquireMessageID(cur.Name);
                if (m_Allocator.InvalidHandle != id)
                {
                    cur.AllocID(id);
                    m_NetMessageIDDescTable.Add(id, cur);
                    m_NetMessageNameDescTable.Add(cur.Name, cur);
                }
            }
        }

        internal override int Priority
        {
            get { return 0; }
        }

        public void RegisterMessage<T>() where T : NetMessage,new()
        {
            System.Type messageType = typeof(T);
            string messageKey = messageType.Name;
            if (!m_NetMessageNameDescTable.ContainsKey(messageKey))
            {
                uint id = m_Allocator.AcquireMessageID(messageKey);
                if (m_Allocator.InvalidHandle != id)
                {
                    NetMessageDesc newDesc = NetMessageDesc<T>.GetDesc();
                    newDesc.AllocID(id);
                    m_NetMessageIDDescTable.Add(id, newDesc);
                    m_NetMessageNameDescTable.Add(newDesc.Name, newDesc);
                }   
            }
        }

        public T CreateMessage<T>() where T : NetMessage
        { 
            return CreateMessage(typeof(T)) as T;
        }

        public NetMessage CreateMessage(System.Type netMessageType)
        {
            NetMessage newMessage = null;
            if (null != netMessageType)
            {
                string messageTypeName = netMessageType.Name;
                NetMessageDesc desc = null;
                if (m_NetMessageNameDescTable.TryGetValue(messageTypeName, out desc))
                {
                    newMessage = desc.Create();
                    return newMessage;
                }
                else
                    Debugger.LogWarning("Net message type:{0} does not register yet!", messageTypeName);
            }
            else
                Debugger.LogWarning("Parameter 'netMessageType' can not be null!");

            return newMessage;
        }

        public uint QureyMessageID<T>() where T : NetMessage
        {
            string messageTypeName = typeof(T).Name;
            NetMessageDesc desc = null;
            if (m_NetMessageNameDescTable.TryGetValue(messageTypeName, out desc))
                return desc.ID;
            else
                Debugger.LogWarning("Net message type:{0} does not register yet!", messageTypeName);

            return Utility.Handle.InvalidHandle;
        }

        public NetMessage Decode(ByteMessage byteMessage)
        {
            if (null == byteMessage)
            {
                Debugger.LogWarning("Parameter 'byteMessage' can not be null!");
                return null;
            }

            NetMessageDesc desc = null;
            if (m_NetMessageIDDescTable.TryGetValue(byteMessage.ID, out desc))
            {
                NetMessage newMessage = desc.Create();
                ByteMessage.Enumerator it = byteMessage.GetEnumerator();
                while (it.MoveNext())
                {
                    ITMByteBlock buf = it.Current;
                    newMessage.DecodeContent(buf);
                }

                return newMessage;
            }
            else
                Debugger.LogWarning("Can not find net message with ID:{0}", byteMessage.ID);

            return null;
        }

        public ByteMessage Encode(NetMessage message,uint cacheSize)
        {
            if (null == message)
            {
                Debugger.LogWarning("Parameter 'message' can not be null!");
                return null;
            }

            if (Utility.Handle.InvalidHandle == message.ID)
            {
                Debugger.LogWarning("Parameter 'message' type:{0} is not valid net message, you can not create it by new operator, use net message interpreter!", message.GetType());
                return null;
            }

            ByteMessage byteMessage = ByteMessage.Acquire();
            byteMessage.Fill(message.ID);

            if(message.HasContent)
            {
                ITMByteBlock buf = null;
                do
                {
                    buf = m_BufferPool.AcquireBuffer(cacheSize,BufferUsage.Write);
                    byteMessage.AddBlock(buf);
                }
                while (!message.EncodeContent(buf));
            }

            return byteMessage;
        }

        internal override void Shutdown()
        {
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}