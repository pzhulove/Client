


using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class NetMessageBufferPool :BaseModule, ITMNetMessageBufferPool
    {
        private readonly static uint MaxBufferSize = 1024 * 1024; /// Max buffer block limit size 1MB.
        private readonly static uint MinBufferSize = 32; /// Max buffer block limit size 32 bytes.

        private class BufferBlockDesc
        {
            private readonly byte[] m_BlockBuffer;

            public BufferBlockDesc(uint bufferSize)
            {
                m_BlockBuffer = new byte[bufferSize];
            }

            public byte[] Buffer { get { return m_BlockBuffer; } }
        }
        
        private readonly Dictionary<uint, Stack<BufferBlockDesc>> m_BufferBlockTable;

        public NetMessageBufferPool()
        {
            m_BufferBlockTable = new Dictionary<uint, Stack<BufferBlockDesc>>();
        }

        internal  override int Priority
        {
            get { return 0; }
        }

        internal override void Shutdown()
        {
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        public ITMByteBlock AcquireBuffer(uint size, BufferUsage usage)
        {
            ByteBuffer byteBuf = ByteBuffer.Acquire();
            byteBuf.Fill(this, size, usage);
            return byteBuf;
        }

        public ITMByteBlock AcquireBuffer(byte[] rawBuffer, BufferUsage usage)
        {
            ByteBuffer byteBuf = ByteBuffer.Acquire();
            byteBuf.Fill(rawBuffer,usage);
            return byteBuf;
        }

        private BufferBlockDesc _AcquireBuffer(uint size)
        {
            if (size > MaxBufferSize)
            {
                Debugger.LogWarning("Parameter can not larger than {0} bytes", MaxBufferSize);
                size = MaxBufferSize;
            }

            if (size < MinBufferSize)
            {
                Debugger.LogWarning("Parameter can not less than {0} bytes", MinBufferSize);
                size = MinBufferSize;
            }

            uint sizePowerOf2 = _GetPowerOf2Value(size);
            if (sizePowerOf2 > 0)
            {
                Stack<BufferBlockDesc> target = null;
                if (!m_BufferBlockTable.TryGetValue(sizePowerOf2, out target))
                {
                    target = new Stack<BufferBlockDesc>();
                    m_BufferBlockTable.Add(sizePowerOf2, target);
                }

                if (target.Count > 0)
                    return target.Pop();

                return new BufferBlockDesc(sizePowerOf2);
            }
            else
                return null;
        }

        private void _RecycleBuffer(BufferBlockDesc desc)
        {
            Stack<BufferBlockDesc> target = null;
            if (m_BufferBlockTable.TryGetValue((uint)desc.Buffer.Length, out target))
                target.Push(desc);
        }

        private uint _GetPowerOf2Value(uint value)
        {
            uint XOR = ~0u ^ ~value;
            for (int i = 0, icnt = sizeof(uint) * 8; i < icnt; ++i)
            {
                int curBit = icnt - i - 1;
                if (1 == XOR >> curBit)
                {
                    int index = 0 != (value & (value - 1)) ? curBit + 1 : curBit;
                    return 1u << index;
                }
            }

            return 0;
        }
    }
}