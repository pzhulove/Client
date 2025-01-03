


using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal class NetMessageAllocator
    {
        private static readonly uint Invalid = Utility.Handle.InvalidTypeHandle(NetMessageHandleType);

        private enum MessageID
        {
            None = 0,
            HeartBeat,
            
            MaxCount,
            Custom = MaxCount,
        }

        private readonly Dictionary<string, uint> m_NetMessageTable;
        private readonly Dictionary<uint,string > m_NetMessageRevertTable;

        private uint m_NetMessageIDAllocCount;
        private static readonly byte NetMessageHandleType = 128;

        public NetMessageAllocator()
        {
            m_NetMessageIDAllocCount = 0;
            m_NetMessageTable = new Dictionary<string, uint>();
            m_NetMessageRevertTable = new Dictionary<uint, string>();

            for (MessageID i = MessageID.None, icnt = MessageID.MaxCount; i < icnt; ++i)
            {
                string msgName = i.ToString();
                uint msgID = Utility.Handle.AllocHandle( NetMessageHandleType, ref m_NetMessageIDAllocCount);

                m_NetMessageTable.Add(msgName, msgID);
                m_NetMessageRevertTable.Add(msgID, msgName);

                Debugger.LogInfo("Register message:[ID:{0} Name:{1}]", msgID, msgName);
            }
        }

        public uint InvalidHandle
        {
            get { return Invalid; }
        }

        public uint AcquireMessageID(string msgName)
        {
            uint handle = Invalid;
            if (!string.IsNullOrEmpty(msgName))
            {
                if (m_NetMessageTable.TryGetValue(msgName, out handle))
                    return handle;

                handle = Utility.Handle.AllocHandle(NetMessageHandleType, ref m_NetMessageIDAllocCount);
                if (Utility.Handle.InvalidHandle != handle)
                {
                    m_NetMessageTable.Add(msgName, handle);
                    m_NetMessageRevertTable.Add(handle, msgName);
                    return handle;
                }
                else
                    Debugger.LogWarning("No more net message ID slot to allocate!");
            }
            else
                Debugger.LogWarning("Parameter 'msgName' can not be null or empty string!");

            return handle;
        }

        public string GetMessageName(uint msgID)
        {
            string msgName = string.Empty;
            if (Invalid != msgID)
            {
                if (m_NetMessageRevertTable.TryGetValue(msgID, out msgName))
                    return msgName;
            }
            else
                Debugger.LogWarning("Parameter 'msgID' [value:0x{0:x}] is not valid net message ID!", msgID);

            return msgName;
        }
    }
}