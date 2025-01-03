

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Tenmove.Runtime
{
    /// <summary>
    /// Raw message
    /// Ŀǰ��ģʽÿ�����Ӷ������messageȻ�󷢳�ȥ����ʵÿ����Ϣֻ��Ҫ����һ�飬
    /// ��Ϣ����������Ϣ������Ҫ�ֿ�����
    /// </summary>
    public class ByteMessage : RecycleBinObject<ByteMessage>, IEnumerable<ITMByteBlock>
    {
        public static readonly uint InvalidID = Utility.Handle.InvalidHandle;

        public struct Enumerator : IEnumerator<ITMByteBlock>, IEnumerator
        {
            private readonly ByteMessage m_Message;
            private LinkedList<ITMByteBlock>.Enumerator m_Enumerator;

            public ITMByteBlock Current
            {
                get { return m_Enumerator.Current; }
            }
            object IEnumerator.Current
            {
                get { return m_Enumerator.Current; }
            }

            public Enumerator(ByteMessage message)
            {
                Debugger.Assert(null != message, "Parameter 'message' can not be null!");

                m_Message = message;
                m_Enumerator = _GetEnumerator();
            }

            public bool MoveNext()
            {
                return m_Enumerator.MoveNext();
            }

            public void Reset()
            {
                m_Enumerator = _GetEnumerator();
            }

            public void Dispose()
            {
            }

            private LinkedList<ITMByteBlock>.Enumerator _GetEnumerator()
            {
                return m_Message.m_Blocks.GetEnumerator();
            }
        }
            
        private readonly LinkedList<ITMByteBlock> m_Blocks;
        private uint m_MessageID;

        public ByteMessage()
        {
            m_Blocks = new LinkedList<ITMByteBlock>();
            m_MessageID = InvalidID;
        }

        public uint ID { get { return m_MessageID; } }
        public sealed override bool IsValid { get { return true; } }
        public bool HasBlock { get { return m_Blocks.Count > 0; } }
        public uint TotalBlock { get { return (uint)m_Blocks.Count; } }

        public void Fill(uint messageID)
        {
            Debugger.Assert(InvalidID != messageID, "Parameter 'messageID' is not a valid value!");
            m_MessageID = messageID;
        }

        public void AddBlock(ITMByteBlock block)
        {
            m_Blocks.AddLast(block);
        }

        IEnumerator<ITMByteBlock> IEnumerable<ITMByteBlock>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            m_MessageID = InvalidID;
            m_Blocks.Clear();
        }

        public sealed override void OnRelease()
        {
        }
    }
}