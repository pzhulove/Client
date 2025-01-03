


using System.IO;

namespace Tenmove.Runtime
{
    internal partial class NetMessageBufferPool
    {
        private class ByteBuffer : RecycleBinObject<ByteBuffer> ,ITMByteBlock
        {

            public enum State
            {
                Acquired,
                Recycled,
            }

            private NetMessageBufferPool m_Pool;
            private BufferBlockDesc m_Desc;
            private byte[] m_Buffer;
            private MemoryStream m_Stream;
            private BinaryWriter m_Writer;
            private BinaryReader m_Reader;
            private State m_State;

            public ByteBuffer()
            {
                m_Buffer = null;
                m_Stream = null;
                m_Writer = null;
                m_Reader = null;
                m_State = State.Recycled;
            }

            public void Fill(NetMessageBufferPool pool,uint capacity, BufferUsage usage)
            {
                Debugger.Assert(null != pool, "Parameter 'pool' can not be null!");

                m_Pool = pool;
                m_Desc = pool._AcquireBuffer(capacity);
                m_Buffer = m_Desc.Buffer;
                m_Stream = new MemoryStream(m_Buffer);
                switch(usage)
                {
                    case BufferUsage.Read: m_Reader = new BinaryReader(m_Stream); break;
                    case BufferUsage.Write: m_Writer = new BinaryWriter(m_Stream); break;
                }
            }

            public void Fill(byte[] bytes, BufferUsage usage)
            {
                m_Pool = null;
                m_Desc = null;
                m_Buffer = bytes;
                m_Stream = new MemoryStream(m_Buffer);
                switch (usage)
                {
                    case BufferUsage.Read: m_Reader = new BinaryReader(m_Stream); break;
                    case BufferUsage.Write: m_Writer = new BinaryWriter(m_Stream); break;
                }
            }

            public uint Capacity
            {
                get { return (uint)m_Buffer.Length; }
            }

            public uint Length
            {
                get { return (uint)m_Stream.Length; }
            }

            public uint Position
            {
                get { return (uint)m_Stream.Position; }
            }

            public sealed override bool IsValid
            {
                get { return null != m_Buffer; }
            }

            public sealed override void OnCreate()
            {
                base.OnCreate();
                m_State = State.Acquired;
            }

            public sealed override void OnRelease()
            {
                _Clear();
            }

            public sealed override void OnRecycle()
            {
                base.OnRecycle();

                _Clear();
               m_State = State.Recycled;
            }

            public sealed override void OnReuse()
            {
                base.OnReuse();
                m_State = State.Acquired;
            }

            public uint FromBytes(byte[] bytes, uint offset = 0, uint length = 0)
            {
                if (null != bytes)
                {
                    uint bufferLen = (uint)bytes.Length;
                    if (offset < bufferLen)
                    {
                        if (0 == length)
                            length = bufferLen;

                        int srcFillBytes = Utility.Math.Max(0, Utility.Math.Min((int)bufferLen - (int)offset, (int)length));
                        int dstFillBytes = (int)Length - (int)Position;
                        uint fillBytes = (uint)Utility.Math.Min(srcFillBytes, dstFillBytes);

                        m_Stream.Write(bytes, (int)offset, (int)length);
                        return fillBytes;
                    }
                    else
                        Debugger.LogWarning("Parameter 'offset' must less than buffer length:{0}!", bufferLen);
                }
                else
                    Debugger.LogWarning("Parameter 'bytes' can not be null!");

                return 0;
            }

            public uint ToBytes(byte[] bytes, uint offset = 0, uint length = 0)
            {
                if (null != bytes)
                {
                    uint bufferLen = (uint)bytes.Length;
                    if (offset < bufferLen)
                    {
                        if (0 == length)
                            length = bufferLen;

                        uint copiedBytes = (uint)m_Stream.Read(bytes, (int)offset, (int)length);
                        return copiedBytes;
                    }
                    else
                        Debugger.LogWarning("Parameter 'offset' must less than buffer length:{0}!", bufferLen);
                }
                else
                    Debugger.LogWarning("Parameter 'bytes' can not be null!");

                return 0;
            }

            public uint FromStream(Stream stream, uint dataBytes)
            {
                if (null != stream)
                {
                    uint copyBytes = (uint)((int)Length - (int)Position);
                    if (dataBytes > 0)
                        copyBytes = Utility.Math.Min(dataBytes, copyBytes);
                    return (uint)stream.Read(m_Buffer, (int)Position, (int)copyBytes);
                }
                else
                    Debugger.LogWarning("Parameter 'stream' can not be null!");

                return 0u;
            }

            public void ToStream(Stream stream, uint dataBytes)
            {
                if (null != stream)
                {
                    uint copyBytes = (uint)((int)Length - (int)Position);
                    if (dataBytes > 0)
                        copyBytes = Utility.Math.Min(dataBytes, copyBytes);
                    stream.Write(m_Buffer, (int)Position, (int)copyBytes);
                }
                else
                    Debugger.LogWarning("Parameter 'stream' can not be null!");
            }

            public float ReadFloat32()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                float res = m_Reader.ReadSingle();
                return res;
            }

            public double ReadFloat64()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                double res = m_Reader.ReadDouble();
                return res;
            }

            public bool ReadBool()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                bool res = m_Reader.ReadBoolean();
                return res;
            }

            public short ReadInt16()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                short res = m_Reader.ReadInt16();
                return res;
            }

            public int ReadInt32()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                int res = m_Reader.ReadInt32();
                return res;
            }

            public long ReadInt64()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                long res = m_Reader.ReadInt64();
                return res;
            }

            public ushort ReadUInt16()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                ushort res = m_Reader.ReadUInt16();
                return res;
            }

            public uint ReadUInt32()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                uint res = m_Reader.ReadUInt32();
                return res;
            }

            public ulong ReadUInt64()
            {
                Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                ulong res = m_Reader.ReadUInt64();
                return res;
            }

            public uint ReadChars(char[] chars)
            {
                if (null != chars)
                {
                    Debugger.Assert(null != m_Reader, "Binary reader can not be null!");
                    return (uint)m_Reader.Read(chars, 0, chars.Length);
                }

                return 0;
            }

            public void Seek(int offset, SeekOrigin origin)
            {
                _Check();
                m_Stream.Seek(offset, origin);
            }

            public void Write(float value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(double value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(bool value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(short value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(int value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(long value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(ushort value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(uint value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(ulong value)
            {
                Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                m_Writer.Write(value);
            }

            public void Write(char[] value)
            {
                if (null != value)
                {
                    Debugger.Assert(null != m_Writer, "Binary writer can not be null!");
                    m_Writer.Write(value);
                }
            }

            private void _Check(long offset = 0, long needSize = 0)
            {
                if (m_State != State.Acquired)
                    throw new EngineException("Byte block has recycled already, But you still using it!");
            }

            private void _Clear()
            {
                if (null != m_Writer)
                {
                    m_Writer.Close();
                    m_Writer = null;
                }

                if (null != m_Reader)
                {
                    m_Reader.Close();
                    m_Reader = null;
                }

                if (null != m_Stream)
                {
                    m_Stream.Close();
                    m_Stream = null;
                }

                if (null != m_Pool)
                {
                    m_Pool._RecycleBuffer(m_Desc);
                    m_Desc = null;
                    m_Pool = null;
                }

                m_Buffer = null;
            }
        }
    }
}