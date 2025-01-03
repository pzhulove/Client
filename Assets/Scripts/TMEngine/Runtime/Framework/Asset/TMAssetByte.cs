


namespace Tenmove.Runtime
{
    public class AssetByte
    {
        private readonly byte[] m_AssetBytes;

        public AssetByte(byte[] bytes)
        {
            m_AssetBytes = bytes;
        }

        public byte[] AssetBytes
        {
            get { return m_AssetBytes; }
        }

    }
}