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
    public class MsgBuffer
    {
        public byte[] data = new byte[NetOutputBuffer.DEFAULTSOCKETOUTPUTBUFFERSIZE];
        public int length;
    }

	public class NetOutputBuffer
	{
		//初始化的发送缓存长度
		static public readonly int DEFAULTSOCKETOUTPUTBUFFERSIZE = 64 * 1024;//8*1024
		public int m_BufferLen = 0;
		public int m_MaxBufferLen = 0;
        static private GamePool.MutexObjectPool<MsgBuffer> msgPool = new GamePool.MutexObjectPool<MsgBuffer>();

        public static void Init(int maxCacheObjNum)
        {
            msgPool.Init(maxCacheObjNum);
        }

        public static GamePool.MutexObjectPool<MsgBuffer> GetMsgPool()
        {
            return msgPool;
        }

        //发送缓冲区
        public byte[] m_Buffer;
        // 多线程发送数据缓冲区，有好的方法，可以去除，不然用的时候就得拷贝！！ [3/12/2012 adomi]
        public byte[] m_SendData;
        public int m_Head;
        public int m_Tail;

        protected NetWorkBase netWorkBase = null;
		
		public NetOutputBuffer (NetWorkBase netWork)
		{
            netWorkBase = netWork;
            m_Head = 0;
            m_Tail = 0;
            m_Buffer = new byte[DEFAULTSOCKETOUTPUTBUFFERSIZE];
            m_BufferLen = DEFAULTSOCKETOUTPUTBUFFERSIZE;
            m_MaxBufferLen = DEFAULTSOCKETOUTPUTBUFFERSIZE;
            m_SendData = new byte[DEFAULTSOCKETOUTPUTBUFFERSIZE];
		}
		
		/// <summary>
        /// 将buff写入缓存区
        /// </summary>
        /// <param name="buf">要写入的buff</param>
        /// <param name="len">要写入的buff的长度</param>
        /// <returns>
        /// 0： 失败
        /// 返回len：则成功
        /// </returns>
        public int Write(ref byte[] buf, int len)
        {

            int nFree = ((m_Head <= m_Tail) ? (m_BufferLen - m_Tail + m_Head - 1) : (m_Head - m_Tail - 1));

            if (len >= nFree)
            {
                if (!Resize(len - nFree + 1))
                    return 0;
            }
            for (int i = 0; i < len; ++i)
            {
                m_Buffer[m_Tail++] = buf[i];
                if (m_Tail == m_BufferLen)
                    m_Tail -= m_BufferLen;
            }

            return len;
        }

        /// <summary>
        /// 把一个byte值写放发送缓冲区
        /// </summary>
        /// <param name="buf">要写入的byte值</param>
        /// <returns>
        /// 0： 失败
        /// 1：则成功
        /// </returns>
        public int WriteByte(byte buf)
        {

            int nFree = ((m_Head <= m_Tail) ? (m_BufferLen - m_Tail + m_Head - 1) : (m_Head - m_Tail - 1));

            if (1 >= nFree)
            {
                if (!Resize(1 - nFree + 1))
                    return 0;
            }
            m_Buffer[m_Tail++] = buf;
            if (m_Tail == m_BufferLen)
                m_Tail -= m_BufferLen;
            return 1;
        }
		
        public int WriteSByte(sbyte buf)
        {
            return WriteByte((byte)buf);
        }
		/// <summary>
        /// 把一个Short值写放发送缓冲区
        /// </summary>
        /// <param name="buf">要写入的Short值</param>
        /// <returns>
        /// 0： 失败
        /// 2：则成功
        /// </returns>
        public int WriteShort(short buf)
        {

            int nFree = ((m_Head <= m_Tail) ? (m_BufferLen - m_Tail + m_Head - 1) : (m_Head - m_Tail - 1));

            if (2 >= nFree)
            {
                if (!Resize(2 - nFree + 1))
                    return 0;
            }

            buf = IPAddress.HostToNetworkOrder(buf);
            m_Buffer[m_Tail++] = (byte)((buf >> 0) & 0xff);
            if (m_Tail == m_BufferLen) m_Tail -= m_BufferLen;
            m_Buffer[m_Tail++] = (byte)((buf >> 8) & 0xff);
            if (m_Tail == m_BufferLen) m_Tail -= m_BufferLen;

            return 2;
        }

        /// <summary>
        /// 把一个ushort值写放发送缓冲区
        /// </summary>
        /// <param name="buf">要写入的ushort值</param>
        /// <returns>
        /// 0： 失败
        /// 2：则成功
        /// </returns>
        public int WriteUShort(ushort buf)
        {
            return WriteShort( (short)buf );
        }

        /// <summary>
        /// 把一个Int值写放发送缓冲区
        /// </summary>
        /// <param name="buf">要写入的Int值</param>
        /// <returns>
        /// 0： 失败
        /// 4：则成功
        /// </returns>
        public int WriteInt(int buf)
        {

            int nFree = ((m_Head <= m_Tail) ? (m_BufferLen - m_Tail + m_Head - 1) : (m_Head - m_Tail - 1));

            if (4 >= nFree)
            {
                if (!Resize(4 - nFree + 1))
                    return 0;
            }

            buf = IPAddress.HostToNetworkOrder(buf);
            m_Buffer[m_Tail++] = (byte)((buf >> 0) & 0xff);
            if (m_Tail == m_BufferLen) m_Tail -= m_BufferLen;
            m_Buffer[m_Tail++] = (byte)((buf >> 8) & 0xff);
            if (m_Tail == m_BufferLen) m_Tail -= m_BufferLen;
            m_Buffer[m_Tail++] = (byte)((buf >> 16) & 0xff);
            if (m_Tail == m_BufferLen) m_Tail -= m_BufferLen;
            m_Buffer[m_Tail++] = (byte)((buf >> 24) & 0xff);
            if (m_Tail == m_BufferLen) m_Tail -= m_BufferLen;

            return 4;
        }

        /// <summary>
        /// 把一个uint值写放发送缓冲区
        /// </summary>
        /// <param name="buf">要写入的uint值</param>
        /// <returns>
        /// 0： 失败
        /// 4：则成功
        /// </returns>
        public int WriteUint(uint buf)
        {
            return WriteInt((int)buf);
        }

        /// <summary>
        /// 把一个float值写放发送缓冲区
        /// </summary>
        /// <param name="buf">要写入的float值</param>
        /// <returns>
        /// 0： 失败
        /// 4：则成功
        /// </returns>
        public int WriteFloat(float buf)
        {
            byte[] bytes = BitConverter.GetBytes(buf);
            return Write(ref bytes, bytes.Length);
        }

        /// <summary>
        /// 写结构体到发送缓冲区
        /// </summary>
        /// <param name="InType">传入的object</param>
        /// <returns>
        /// 0:出错
        /// > 0 且 为object 大小：正常
        /// </returns>
        public int WriteStruct(object InType)
        {
            int size = Marshal.SizeOf(InType);
            byte[] bytes = new byte[size];
            //分配结构体大小的内存
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷贝到分配好的空间
            Marshal.StructureToPtr(InType, structPtr, false);
            //从内存拷贝到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);

            if ( Write(ref bytes, bytes.Length) == 0)
            {
                return 0;
            }
            return size;
        }
		
		/// <summary>
        /// 发送缓冲区里的所有逻辑包
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="buffLen">发送长度</param>
        /// <returns></returns>
        protected bool Send ( int buffLen )
        {
         //   if (ClientApplication.isOpenNewReconnectAlgo)
            {
                // 拷贝要发送的 [3/12/2012 adomi]
                MsgBuffer buffer = NetOutputBuffer.GetMsgPool().Get();
                buffer.length = buffLen;
                byte[] sendData = buffer.data;
                Array.Clear(sendData, 0, sendData.Length);
                Array.Copy(m_Buffer, m_Head, sendData, 0, buffLen);

#if UNITY_EDITOR && NET_LOG
            Logger.LogErrorFormat("[RECON] output buffer send data:{0}->{1} len:{2}", m_Head, m_Head + buffLen, buffLen);
#endif

                netWorkBase.SendByPool(buffer);
            }
            //else
            //{
            //    Array.Clear(m_SendData, 0, m_SendData.Length);
            //    Array.Copy(m_Buffer, m_Head, m_SendData, 0, buffLen);
            //    netWorkBase.Send(m_SendData, 0, buffLen);
            //}

            return true;
        }
		
		/// <summary>
        /// 发送缓冲区里的消息
        /// </summary>
        /// <returns>
        /// -1：出错
        /// >=0：正常
        /// </returns>
        public int Flush()
        {
            int nFlushed = 0;
            int nSent = 0;
            int nLeft;

            if (NetManager.Instance().Show)
            {
                NetManager.Instance().Log("output buffer flushing... head:{0} tail:{1}", m_Head, m_Tail);
            }

            if (m_BufferLen > m_MaxBufferLen)
            {
                NetManager.Instance().Log("output buffer error, len:{0} max:{1}.", m_BufferLen, m_MaxBufferLen);
                //如果单个客户端的缓存太大，则重新设置缓存，并将此客户端断开连接
                Initsize();
                return -1;
            }

            if (m_Head < m_Tail)
            {
                nLeft = m_Tail - m_Head;

                while (nLeft > 0)
                {
                    //发送包
                    Send( nLeft ) ;

                    nSent = nLeft;
                    nFlushed += nSent;
                    nLeft -= nSent;
                    m_Head += nSent;
                }

            }
            else if (m_Head > m_Tail)
            {
                nLeft = m_BufferLen - m_Head;

                while (nLeft > 0)
                {
                    //发送包
                    Send( nLeft ) ;

                    nSent = nLeft;
                    nFlushed += nSent;
                    nLeft -= nSent;
                    m_Head += nSent;
                }

                m_Head = 0;

                nLeft = m_Tail;

                while (nLeft > 0)
                {
                    //发送包
                    Send( nLeft ) ;

                    nSent = nLeft;
                    nFlushed += nSent;
                    nLeft -= nSent;
                    m_Head += nSent;
                }
            }

            m_Head = m_Tail = 0;

            return nFlushed;
        }
		
		public void Initsize()
        {
            m_Head = m_Tail = 0;
            m_Buffer = new byte[DEFAULTSOCKETOUTPUTBUFFERSIZE];
            m_BufferLen = DEFAULTSOCKETOUTPUTBUFFERSIZE;
            m_MaxBufferLen = DEFAULTSOCKETOUTPUTBUFFERSIZE;
        }

        // 暂时不做内存的重新分配 [12/13/2011 ZL]
        public bool Resize(int size)
        {
            NetManager.Instance().Log("output buffer resize{0} failed.", size);
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

        public bool IsEmpty()
        {
            return m_Head == m_Tail;
        }

        public void CleanUp()
        {
            m_Head = m_Tail = 0;
            Array.Clear(m_Buffer, 0, m_Buffer.Length);
            Array.Clear(m_SendData, 0, m_SendData.Length);
        }
		
	}
}

