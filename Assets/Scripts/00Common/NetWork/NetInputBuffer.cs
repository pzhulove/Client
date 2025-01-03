using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Network
{
    public class NetInputBuffer
    {
		//初始化的接收缓存长度
        //临时增加BUFFER长度，以后修改回来     罗亚
        public readonly int DEFAULTSOCKETOUTPUTBUFFERSIZE = 1024 * 1024 ;

        public int m_BufferLen = 0;
        public int m_MaxBufferLen = 0;
        public byte[] m_Buffer;
        public int m_Head = 0;
        public int m_Tail = 0;
		
		public NetInputBuffer ()
		{
			m_Head = 0;
            m_Tail = 0;
            m_Buffer = new byte[DEFAULTSOCKETOUTPUTBUFFERSIZE];
            m_BufferLen = DEFAULTSOCKETOUTPUTBUFFERSIZE;
            m_MaxBufferLen = DEFAULTSOCKETOUTPUTBUFFERSIZE;
		}

        public byte[] GetRawBuffer()
        {
            return m_Buffer ;
        }

        public int GetCurrentOffset ()
        {
            return m_Tail ;
        }

        public int GetCurrentSize ()
        {
            return m_BufferLen - m_Tail ;
        }

        // 返回false表示读取失败 [12/13/2011 ZL]
        public bool Peek(ref byte[] buf, int len)
        {
            if (len == 0)
            {
                Logger.LogError( "len == 0" ) ;
                return false;
            }
                
            int resLen = Length() ;
            if (len > resLen)
            {
                Logger.LogError( "len > Length(): " + len + " > " + resLen ) ;
                return false;
            }
                
            int j = m_Head;
            for ( int i = 0; i < len; ++i )
            {
                buf[i] = m_Buffer[j++];
                if (j == m_BufferLen)
                    j = 0;
            }
            return true;
        }

        public int GetPackLength()
        {
            ushort outBuf = 0 ;
            if ( Length() < (int)NET_DEFINE.HEADER_SIZE) return 0;
            outBuf = (ushort)(m_Buffer[m_Head] | (m_Buffer[m_Head + 1] << 8));
            //Skip(4);
            outBuf = (ushort)IPAddress.NetworkToHostOrder( (short)outBuf );
            return outBuf ;
        }

        /// <summary>
        /// 从接收缓冲区里得到len长度的字符串
        /// </summary>
        /// <param name="buf">得到的buff</param>
        /// <param name="len">要得到的buff的长度</param>
        /// <returns>
        /// 0：没有读到数据
        /// >0：得到数据
        /// </returns>
        public int Read(ref byte[] buf, int len)
        {
            if (!Peek(ref buf, len))
                return 0;
            Skip(len);
            return len;
        }
        
        /// <summary>
        /// 从接收缓冲区里得到一个byte值
        /// </summary>
        /// <param name="buf">得到的byte值</param>
        /// <returns>
        ///  0：没有读到数据
        ///  1：得到数据
        /// </returns>
        public int ReadByte(ref byte buf)
        {
            if (Length() < 1) return 0;
            buf = m_Buffer[m_Head];
            Skip(1);
            return 1;
        }
        public int ReadSByte(ref sbyte buf)
        {
            if (Length() < 1) return 0;
            byte b;
            b = m_Buffer[m_Head];
            Skip(1);
            buf = (sbyte)b;
            return 1;
        }

        /// <summary>
        /// 从接收缓冲区里得到一个short值
        /// </summary>
        /// <param name="buf">得到的short值</param>
        /// <returns>
        ///  0：没有读到数据
        ///  2：得到数据
        /// </returns>
        public int ReadShort(ref short buf)
        {
            if (Length() < 2) return 0;
            buf = (short)((m_Buffer[m_Head]) | (m_Buffer[m_Head + 1] << 8));
            Skip(2);
            buf = IPAddress.NetworkToHostOrder(buf);
            return 2;
        }

        /// <summary>
        /// 从接收缓冲区里得到一个ushort值
        /// </summary>
        /// <param name="buf">得到的ushort值</param>
        /// <returns>
        ///  0：没有读到数据
        ///  2：得到数据
        /// </returns>
        public int ReadUShort(ref ushort buf)
        {
            short temp = 0;
            if (ReadShort(ref temp) == 0)
                return 0;

            buf = (ushort)temp;
            return 2;
        }
        

        /// <summary>
        /// 从接收缓冲区里得到一个int值
        /// </summary>
        /// <param name="buf">要得到的int值</param>
        ///  0：没有读到数据
        ///  4：得到数据
        /// </returns>
        public int ReadInt(ref int buf)
        {
            if (Length() < 4) return 0;
            buf = (m_Buffer[m_Head]) | (m_Buffer[m_Head + 1] << 8) | (m_Buffer[m_Head + 2] << 16) | (m_Buffer[m_Head + 3] << 24);
            Skip(4);
            buf = IPAddress.NetworkToHostOrder(buf);
            return 4;
        }

        /// <summary>
        /// 从接收缓冲区里得到一个Uint值 
        /// </summary>
        /// <param name="buf">要得到的Uint值</param>
        ///  0：没有读到数据
        ///  4：得到数据
        /// </returns>
        public int ReadUint(ref uint buf)
        {
            int temp = 0;
            if (ReadInt(ref temp) == 0)
                return 0;

            buf = (uint)temp;

            return 4;
        }
        /// <summary>
        /// 从接收缓冲区里得到一个float值
        /// </summary>
        /// <param name="buf">要得到的float值</param>
        ///  0：没有读到数据
        ///  4：得到数据
        /// </returns>
        public int ReadFloat(ref float buf)
        {
            if (Length() < 4) return 0;
            buf = BitConverter.ToSingle(m_Buffer, m_Head);
            Skip(4);
            return 4;
        }

        /// <summary>
        /// 读取结构体（网页版本的不能使用！）
        /// </summary>
        /// <param name="OutType">返回的object</param>
        /// <returns>
        /// 0：出错
        /// > 0 :等于object的大小：正常
        /// </returns>
        public int ReadStruct(ref object OutObject)
        {
            int size = Marshal.SizeOf(OutObject);

            byte[] bytes = new byte[size];
            if ( Read(ref bytes, size) == 0)
            {
                return 0;
            }
            if (size > bytes.Length)
            {
                return 0;
            }

            //test begin
            /*WriteFiles.WritFile.Log(LogerType.INFO,"SocketInputStream::ReadStructl( ):" + size + "\n");
            for (int i = 0; i < size; ++i)
            {
                WriteFiles.WritFile.Log(LogerType.INFO, "bytes:" + bytes[i]);
            }*/
            //test end

            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将字节拷贝到内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构
            OutObject = Marshal.PtrToStructure(structPtr, OutObject.GetType());
            //释放内存
            Marshal.FreeHGlobal(structPtr);

            return size;
        }

        /// <summary>
        /// 读取结构体
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>
        /// null：出错
        /// 不为null： 正常
        /// </returns>
        public object ReadStruct(Type type )
        {
            int size = Marshal.SizeOf(type);

            byte[] bytes = new byte[size];
            if (Read(ref bytes, size) == 0)
            {
                return null;
            }
            if (size > bytes.Length)
            {
                return null;
            }

            ////test begin
            //string msg = string.Empty;

            //WriteFiles.Log("SocketInputStream::ReadStructl( ):" + size);
            //for (int i = 0/*m_Head*/; i < size; ++i)
            //{
            //    msg += "," + BitConverter.ToString(BitConverter.GetBytes(bytes[i]));
                
            //}
            //WriteFiles.Log( msg);
            ////test end

            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将字节拷贝到内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存
            Marshal.FreeHGlobal(structPtr);

            return obj;
        }

        /// <summary>
        /// 查找包的分隔符
        /// </summary>
        /// <param name="buf">分隔符串</param>
        /// <returns>
        /// true ：成功找到
        /// false：没有找到
        /// </returns>
        public bool Find(ref byte[] buf)
        {
            /*int len = Length();
            for (int i = 0; i < len; ++i)
            {
                int j = 0;
                if (m_Buffer[m_Head] == NET_DEFINE.PACK_COMPART[j])
                {
                    for (++j; j < NET_DEFINE.PACK_COMPART_SIZE; ++j)
                    {
                        if (m_Buffer[(m_Head + j) % m_BufferLen] != NET_DEFINE.PACK_COMPART[j])
                            break;
                    }
                }
                // 找到包分隔符,并跳过包分隔符 [12/13/2011 ZL]
                if (j == NET_DEFINE.PACK_COMPART_SIZE)
                {
                    m_Head += NET_DEFINE.PACK_COMPART_SIZE;
                    if (m_Head >= m_BufferLen)
                        m_Head -= m_BufferLen;
                    return true;
                }
                ++m_Head;
                if (m_Head == m_BufferLen)
                {
                    m_Head -= m_BufferLen;
                }
            }*/
            return false;
        }

        public bool Skip(int len)
        {
            if (len == 0)
                return false;

            if (len > Length())
                return false;

            m_Head = (m_Head + len) % m_BufferLen;

            //Edit by luoya, for leave away from end of the buffer...
            if ( m_Head == m_Tail )
            {
                m_Head = m_Tail = 0 ;
            }

            return true;
        }

        public void Initsize()
        {
            m_Head = m_Tail = 0;
            m_Buffer = new byte[DEFAULTSOCKETOUTPUTBUFFERSIZE];
            m_BufferLen = DEFAULTSOCKETOUTPUTBUFFERSIZE;
            m_MaxBufferLen = DEFAULTSOCKETOUTPUTBUFFERSIZE;
        }
		
        public bool Resize(int size)
        {
            return false;
        }

        public int Capacity()
        {
            return m_BufferLen;
        }

        public int Length()
        {
            if (m_Head < m_Tail)
                return m_Tail - m_Head;

            else if (m_Head > m_Tail)
                return m_BufferLen - m_Head + m_Tail;

            return 0;
        }

        public void resetBuffer( bool isNotEnough)
        {
            try
            {
                //如果出现粘包现象，则把上次余留的TCP包拷到缓冲区的头部，否则清空缓冲区
                if (isNotEnough)
                    Array.Copy(m_Buffer, m_Head, m_Buffer, 0, Length());
                else
                    Array.Clear(m_Buffer, 0, m_Buffer.Length);
                
                //重新设置总大小与 当前偏移
                m_Tail = Length();
                m_Head = 0;
            }
            catch (Exception e)
            {
                //Network.NetWork.ErrorString = "resetBuffer: " + e.ToString();
                //WriteFiles.WritFile.Log(LogerType.ERROR, "resetBuffer: " + e.ToString());
            }
        }

        public bool IsEmpty()
        {
            return m_Head == m_Tail;
        }

        public void CleanUp()
        {
            m_Head = m_Tail = 0;
            Array.Clear(m_Buffer, 0, m_Buffer.Length);
        }
	}
}

