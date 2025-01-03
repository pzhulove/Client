


using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public delegate NetMessage NetMessageCreator();
    public class NetMessageDesc
    {
        private uint m_ID;
        private readonly string m_Name;
        private readonly NetMessageCreator m_Creator;

        public NetMessageDesc(string name, NetMessageCreator creator)
        {
            m_ID = ~0u;
            m_Name = name;
            m_Creator = creator;
        }

        public string Name { get { return m_Name; } }
        public uint ID { get { return m_ID; } }

        public bool AllocID(uint id)
        {
            if (~0u == m_ID)
            {
                if (~0u != id)
                {
                    m_ID = id;
                    return true;
                }
                else
                    Debugger.LogWarning("Parameter 'id'[value:{0}] is invalid net message ID!", id);
            }
            else
                Debugger.LogWarning("Net message '{0}' has already allocate a ID number:{1}", m_Name, m_ID);

            return false;
        }

        public NetMessage Create()
        {
            if (~0u != m_ID)
            {
                NetMessage newMsg = m_Creator();
                newMsg.AllocID(m_ID);
                return newMsg;
            }
            else
                Debugger.LogWarning("Net message '{0}' has not allocate a ID number yet!", m_Name);

            return null;
        }
    }

    public class NetMessageDesc<T> where T : NetMessage, new()
    {
        public static NetMessageDesc GetDesc()
        {
            return new NetMessageDesc(typeof(T).Name, NetMessage.Create<T>);
        }
    }

    public abstract class NetMessage
    {
        private uint m_ID;

        public NetMessage()
        {
            m_ID = Utility.Handle.InvalidHandle;
        }

        public uint ID { get { return m_ID; } }
        public void AllocID(uint id) { m_ID = id; }

        static public NetMessage Create<T>() where T : NetMessage, new()
        {
            return new T();
        }

        public void DecodeContent(ITMByteBlock block)
        {
            Debugger.Assert(Utility.Handle.InvalidHandle != m_ID, "This message is not valid net message, you can not create it by new operator, use net message interpreter!");
            Debugger.Assert(null != block, "Parameter 'block' can not be null!");

            _OnDecodeContent(block);
        }

        public bool EncodeContent(ITMByteBlock block)
        {
            Debugger.Assert(Utility.Handle.InvalidHandle != m_ID, "This message is not valid net message, you can not create it by new operator, use net message interpreter!");
            Debugger.Assert(null != block, "Parameter 'block' can not be null!");

            return _OnEncodeContent(block);
        }

        public abstract bool HasContent { get; } 
        protected abstract void _OnDecodeContent(ITMByteBlock block);
        protected abstract bool _OnEncodeContent(ITMByteBlock block);
    }
}