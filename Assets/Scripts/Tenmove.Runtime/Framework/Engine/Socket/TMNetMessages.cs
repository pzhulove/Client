

namespace Tenmove.Runtime
{
    using System.Collections.Generic;
    using System.IO;

    public abstract class NetMessageLite : NetMessage
    {
        public override bool HasContent { get { return false; } }

        protected override void _OnDecodeContent(ITMByteBlock block) { }
        protected override bool _OnEncodeContent(ITMByteBlock block) { return true; }
    }
    
    public class NetMessageHeartBeat : NetMessageLite { }

    public class NetMessageTransmitFile : NetMessage
    {
        private string m_FileName;
        private Stream m_FileStream;
        private int m_FileLength;

        private long m_OriginOffset;
        private long m_BytesDone;

        public NetMessageTransmitFile()
            :base()
        {
            m_FileName = null;
            m_FileStream = null;
            m_OriginOffset = ~0;
            m_BytesDone = 0;
            m_FileLength = 0;
        }

        public string FileName
        {
            get { return m_FileName; }
        }

        public void Fill(string dstFilePath, Stream stream)
        {
            Debugger.Assert(!string.IsNullOrEmpty(dstFilePath), "Parameter 'dstFilePath' can not be null or empty string!");
            Debugger.Assert(null != stream, "Parameter 'stream' can not be null!");

            m_FileName = dstFilePath;
            m_FileStream = stream;

            m_OriginOffset = ~0;
            m_BytesDone = 0;
        }

        public override bool HasContent { get { return true; } }

        public virtual string GetNativeFilePath(string fileName)
        {
            return fileName;
        }

        protected override void _OnDecodeContent(ITMByteBlock block)
        {
            uint originPos = block.Position;
            block.Seek(0, SeekOrigin.Begin);
            if (null == m_FileStream)
            {
                int charsLen = block.ReadInt32();
                char[] chars = new char[charsLen];
                if (charsLen == block.ReadChars(chars))
                {
                    m_FileName = new string(chars);
                    m_FileLength = block.ReadInt32();
                    m_FileStream = Utility.File.OpenWrite(GetNativeFilePath(m_FileName), true) ;
                }
                else
                    Debugger.LogWarning("Decode file transmit message content has failed!");
            }

            uint dataBytes = block.ReadUInt32();
            block.ToStream(m_FileStream, dataBytes);
            block.Seek((int)originPos, SeekOrigin.Begin);

            m_BytesDone += dataBytes;

            if (m_BytesDone < m_FileLength)
                m_FileStream.Flush();
            else
                m_FileStream.Close();
        }

        protected override bool _OnEncodeContent(ITMByteBlock block)
        {
            if (~0 == m_OriginOffset)
            {/// 传输头
                m_OriginOffset = m_FileStream.Position;
                m_FileLength = (int)m_FileStream.Length;
                m_FileStream.Seek(0, SeekOrigin.Begin);

                char[] data = m_FileName.ToCharArray();
                block.Write(data.Length);
                block.Write(data);
                block.Write(m_FileLength);
            }

            uint blockBytes = (uint)((int)block.Length - (int)block.Position - sizeof(uint));
            uint streamBytes = (uint)(m_FileStream.Length - m_BytesDone);
            uint dataBytes = Utility.Math.Min(blockBytes, streamBytes);
            block.Write(dataBytes);
            m_BytesDone += block.FromStream(m_FileStream);
            if (m_BytesDone < m_FileStream.Length)
                return false;
            else
            {
                m_FileStream.Seek(m_OriginOffset, SeekOrigin.Begin);
                return true;
            }
        }

        protected virtual Stream _OpenWriteStream(string fileName)
        {
            return Utility.File.OpenWrite(fileName, true);
        }
    }
}