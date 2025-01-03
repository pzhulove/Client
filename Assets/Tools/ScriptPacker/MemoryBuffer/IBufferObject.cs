
using System;
using System.IO;
using Tenmove.Runtime;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MemoryWriteReaderAnimation
{
    public interface IBinaryable
    {
        IBufferObject ToBufferObject();
    }

    public interface IBufferObject
    {
        void ReadFrom(MemoryBufferReader memoryReader);
        void WriteTo(MemoryBufferWriter memoryWriter);
    }

    public class TMemoryBufferReaderWriter<T> where T : IBufferObject
    {
        private T m_Object;

        public TMemoryBufferReaderWriter(T _obj)
        {
            m_Object = _obj;
        }

        /// <summary>
        /// 从文件加载，自动解析是否LZ4压缩
        /// </summary>
        /// <param name="fileName">相对于Resources目录下的路径</param>
        /// <returns></returns>
        public bool LoadFrom(string fileName)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                TextAsset textAsset = Resources.Load<TextAsset>(Path.ChangeExtension(fileName, null));
                OnLoad(textAsset != null ? textAsset.bytes : null);
            }
            else
#endif
            {
                //AssetLoader.LoadAsset(fileName, typeof(TextAsset), null, (string path, object asset, int taskID, float duration, object userData) =>
                //{
                //    TextAsset textAsset = asset as TextAsset;
                //    OnLoad(textAsset != null ? textAsset.bytes : null);
                //},
                //false);
            }

            return true;
        }

        private void OnLoad(byte[] scriptData)
        {
            if (scriptData == null)
                return;

            DeSerializeFrom(scriptData);
        }

        /// <summary>
        /// 从内存加载，自动解析是否LZ4压缩
        /// </summary>
        /// <param name="inBuffer"></param>
        public void DeSerializeFrom(byte[] inBuffer, int startIndex = 0, int length = 0)
        {
            if (inBuffer[startIndex] > 0)
            {
                int oriLengh = BitConverter.ToInt32(inBuffer, startIndex + 1);

                byte[] uncompressedBuffer = ScriptHelper.DecodeLZ4(inBuffer, startIndex + 5, length - 5, oriLengh);

                MemoryBufferReader memReader = new MemoryBufferReader(uncompressedBuffer);
                m_Object.ReadFrom(memReader);
            }
            else
            {
                MemoryBufferReader memReader = new MemoryBufferReader(inBuffer, startIndex, length);

                Byte bCompressed = 0;
                memReader.Read(ref bCompressed);

                m_Object.ReadFrom(memReader);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 序列化成byte[]，支持压缩。返回的byte[] 通过DeSerializeFrom可反序列化成该对象。
        /// </summary>
        /// <param name="bCompress">是否LZ4压缩</param>
        /// <returns></returns>
        public byte[] SerializeTo(bool bCompress = true)
        {
            MemoryBufferWriter memWriter = new MemoryBufferWriter();
            m_Object.WriteTo(memWriter);

            byte[] binaryData = memWriter.GetBufferData();
            if (binaryData == null)
                return null;

            byte[] retBuffer = null;

            if (bCompress)
            {
                byte[] lengthByte = BitConverter.GetBytes(binaryData.Length);
                byte[] packedBuffer = ScriptHelper.EncodeLZ4(binaryData, 0, binaryData.Length);

                retBuffer = new byte[packedBuffer.Length + 5];
                retBuffer[0] = (byte)1;
                for (int i = 0; i < 4; ++i)
                {
                    retBuffer[1 + i] = lengthByte[i];
                }

                Buffer.BlockCopy(packedBuffer, 0, retBuffer, 5, packedBuffer.Length);
            }
            else
            {
                retBuffer = new byte[binaryData.Length + 1];
                retBuffer[0] = (byte)0;

                Buffer.BlockCopy(binaryData, 0, retBuffer, 1, binaryData.Length);
            }

            return retBuffer;
        }

        /// <summary>
        /// 序列化成byte[]，支持压缩。跟上面函数相比不用每次函数内new一个byte[]数组。
        /// </summary>
        /// <param name="buffer">接收数据数组</param>
        /// <param name="bCompress">是否LZ4压缩</param>
        /// <returns>数据长度</returns>
        public int SerializeTo(byte[] buffer, bool bCompress = true)
        {
            MemoryBufferWriter memWriter = new MemoryBufferWriter();
            m_Object.WriteTo(memWriter);

            byte[] binaryData = memWriter.GetBufferData();
            if (binaryData == null)
                return 0;

            int length;
            if (bCompress)
            {
                byte[] lengthByte = BitConverter.GetBytes(binaryData.Length);
                byte[] packedBuffer = ScriptHelper.EncodeLZ4(binaryData, 0, binaryData.Length);

                buffer[0] = (byte)1;
                for (int i = 0; i < 4; ++i)
                {
                    buffer[1 + i] = lengthByte[i];
                }

                Buffer.BlockCopy(packedBuffer, 0, buffer, 5, packedBuffer.Length);
                length = packedBuffer.Length + 5;
            }
            else
            {
                buffer[0] = (byte)0;
                Buffer.BlockCopy(binaryData, 0, buffer, 1, binaryData.Length);
                length = binaryData.Length + 1;
            }

            return length;
        }

        /// <summary>
        /// 序列化到文件，支持LZ4压缩
        /// </summary>
        /// <param name="fileName">相对于Resources目录下的路径</param>
        /// <returns></returns>
        public bool SaveTo(string fileName, bool bCompress = true)
        {
            MemoryBufferWriter memWriter = new MemoryBufferWriter();
            m_Object.WriteTo(memWriter);

            byte[] binaryData = memWriter.GetBufferData();
            if (binaryData == null)
                return false;

            fileName = "Assets/Resources/" + fileName;

           // AssetDatabase.DeleteAsset(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                byte[] packedBuffer = binaryData;

                fs.WriteByte(bCompress ? (byte)1 : (byte)0);
                if (bCompress)
                {
                    byte[] lengthByte = BitConverter.GetBytes(binaryData.Length);
                    fs.Write(lengthByte, 0, lengthByte.Length);
                    packedBuffer = ScriptHelper.EncodeLZ4(binaryData, 0, binaryData.Length);
                }

                fs.Write(packedBuffer, 0, packedBuffer.Length);
            }

            return true;
        }
#endif
    }
}

