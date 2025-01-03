

namespace Tenmove.Runtime.Unity
{
    using System.IO;
    using Tenmove.Runtime.Math;
    using UnityEngine;

    public interface ITMNetMessageGameProfile
    {
        NetMessage NetMessage { get; }
    }

    public abstract class NetMessageGameProfile : NetMessage, ITMNetMessageGameProfile
    {
        public NetMessage NetMessage { get { return this; } }
    }

    public class GameProfileTransmitFile : NetMessageTransmitFile
    {
        public NetMessage NetMessage { get { return this; } }

        public override string GetNativeFilePath(string fileName)
        {
#if UNITY_EDITOR
            return Utility.Path.Combine(Application.streamingAssetsPath, "AssetBundles", fileName);
#else 
            return Utility.Path.Combine(Application.persistentDataPath, "AssetBundles", fileName);         
#endif
        }
    }

    public class GameProfileTransmitFileEnd : NetMessage
    {
        private char[] m_CharBuffer;

        public GameProfileTransmitFileEnd()
        {
            m_CharBuffer = null;
        }

        public override bool HasContent
        {
            get { return true; }
        }

        public void Fill(string message)
        {
            if (null != message)
                m_CharBuffer = message.ToCharArray();
        }

        public string Message
        {
            get
            {
                if (null != m_CharBuffer)
                    return new string(m_CharBuffer);
                else
                    return string.Empty;
            }
        }

        protected override void _OnDecodeContent(ITMByteBlock block)
        {
            int charLen = block.ReadInt32();
            if (charLen > 0)
            {
                m_CharBuffer = new char[charLen];
                block.ReadChars(m_CharBuffer);
            }
        }

        protected override bool _OnEncodeContent(ITMByteBlock block)
        {
            if (null != m_CharBuffer)
            {
                block.Write(m_CharBuffer.Length);
                block.Write(m_CharBuffer);
            }
            else
                block.Write(0);

            return true;
        }
    }

    public class GameProfileCreateAsset : NetMessageGameProfile
    {
        public string AssetName { set; get; }
        public int InstCount { set; get; }
        public Vec2 Range { set; get; }

        public override bool HasContent
        {
            get { return true; }
        }

        protected override void _OnDecodeContent(ITMByteBlock block)
        {
            int stringLen = block.ReadInt32();
            if (stringLen > 0)
            {
                char[] stringData = new char[stringLen];
                block.ReadChars(stringData);
                AssetName = new string(stringData);
            }
            InstCount = block.ReadInt32();
            Range = new Vec2() { x = block.ReadFloat32(), y = block.ReadFloat32() };
        }

        protected override bool _OnEncodeContent(ITMByteBlock block)
        {
            if (null != AssetName)
            {
                char[] stringData = AssetName.ToCharArray();
                block.Write(stringData.Length);
                block.Write(stringData);
            }
            block.Write(InstCount);
            block.Write(Range.x);
            block.Write(Range.y);
            return true;
        }
    }

    public class GameProfileClearAllAssets : NetMessageGameProfile
    {
        public override bool HasContent
        {
            get { return false; }
        }

        protected override void _OnDecodeContent(ITMByteBlock block)
        {
        }

        protected override bool _OnEncodeContent(ITMByteBlock block)
        {
            return true;
        }
    }
}