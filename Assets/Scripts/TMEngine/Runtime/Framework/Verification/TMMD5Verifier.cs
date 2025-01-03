
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tenmove.Runtime
{
    public class MD5Verifier : IDataVerifier<string>
    {
        private const int CONST_DEFAULT_CACHE_SIZE = 4 * 1024;

        private byte[] m_DataCache = null;
        private Stream m_Stream = null;
        private long m_OriginStreamPosition = 0;
        private MD5 m_MD5 = null;
        private string m_VerifySum = "";
        private long m_TimeSlice = 0;

        private float m_Progress = 0.0f;

        public float Progress
        {
            get { return m_Progress; }
        }

        public bool BeginVerify(Stream dataStream, int cacheSize,float timeSlice)
        {
            if(null == dataStream)
            {
                Debugger.LogError("Data stream can not be null!");
                return false;
            }

            if (null != m_MD5)
            {
                Debugger.LogError("Verifier already exist, you must finish last verify operation first, try call EndVerify()!");
                return false;
            }

            if(timeSlice < 0.0f)
            {
                Debugger.LogWarning("Time slice can not be negative value!");
                timeSlice = 0;
            }

            m_TimeSlice = Utility.Time.SecondsToTicks(timeSlice);
            m_Progress = 0;
            m_VerifySum = string.Empty;
            m_MD5 = MD5.Create();
            m_MD5.Initialize();

            if (cacheSize <= 0)
                cacheSize = CONST_DEFAULT_CACHE_SIZE;

            if (null == m_DataCache || m_DataCache.Length < cacheSize)
                m_DataCache = new byte[cacheSize];

            m_Stream = dataStream;
            m_OriginStreamPosition = m_Stream.Position;
            m_Stream.Seek(0, SeekOrigin.Begin);
            return true;
        }

        public bool EndVerify()
        {
            if (null != m_Stream)
            {
                //long timeSlice = m_TimeSlice;
                //long ticksNow = Utility.Time.GetTicksNow();
                //
                //do
                //{
                //    long timeThen = ticksNow;
                //    int readBytes = m_Stream.Read(m_DataCache, 0, m_DataCache.Length);
                //    if (_StepVerifiy(m_DataCache, readBytes))
                //        return true;
                //
                //    ticksNow = Utility.Time.GetTicksNow();
                //    timeSlice -= ticksNow - timeThen;
                //}
                //while (timeSlice > 0);
                
                long ticksNow = Utility.Time.GetTicksNow();
                do
                {
                    int readBytes = m_Stream.Read(m_DataCache, 0, m_DataCache.Length);
                    if (_StepVerifiy(m_DataCache, readBytes))
                        return true;                    
                }
                while (Utility.Time.GetTicksNow() - ticksNow < m_TimeSlice);

                return false;
            }
            else
                return true;
        }

        public string GetVerifySum()
        {
            if (null != m_Stream)
                Debugger.LogWarning("Data verification is in processing!");

            return m_VerifySum;
        }

        private bool _StepVerifiy(byte[] data,int length)
        {
            m_Progress = Utility.Math.Clamp( m_Stream.Position * 1.0f / m_Stream.Length,0,1);

            if (null != data && length > 0)
            {
                m_MD5.TransformBlock(data, 0, length, data, 0);
                return false;
            }
            else
            {
                m_MD5.TransformFinalBlock(data, 0, 0);

                StringBuilder md5 = new StringBuilder(32);
                byte[] md5Data = m_MD5.Hash;
                for (int i = 0; i < md5Data.Length; i++)
                {
                    md5.Append(md5Data[i].ToString("x2"));
                }

                m_MD5.Clear();
                m_MD5 = null;
                m_VerifySum = md5.ToString();
                
                m_Stream.Seek(m_OriginStreamPosition, SeekOrigin.Begin);
                m_Stream = null;
                return true;
            }
        }
    }
}