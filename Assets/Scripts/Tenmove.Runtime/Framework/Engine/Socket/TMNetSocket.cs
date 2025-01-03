

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Tenmove.Runtime
{
    //[Procedure(Log = false)]
    public class NetSocket
    {
        public static readonly uint DefaultCacheSize = 32 * 1024; /// 32kB
        public static readonly uint SocketHeaderSize = sizeof(uint) * 5;
        
        private enum State
        {
            Ready = 0x00,
            Receiving = 0x01,
            Sending = 0x02,
            Shutdown = 0x04,
        }

        /// <summary>
        /// 直接作为队列输出
        /// </summary>
        protected class MessageBlockDesc : IEquatable<MessageBlockDesc>
        {
            public static readonly MessageBlockDesc Null = new MessageBlockDesc();

            private readonly uint m_MagicID;
            public uint MagicID { get { return m_MagicID; } }
            public uint MessageID { get; set; }
            public uint DataBytesTotal { get; set; }
            public uint BlockIndex { set; get; }
            public uint BlockCount { set; get; }

            public uint DataBytesDone { get; set; }
            public ITMByteBlock ByteBlock { get; set; }

            private MessageBlockDesc()
            {
                m_MagicID = ~0u;
                BlockIndex = ~0u;
                BlockCount = ~0u;
                MessageID = ~0u;
                ByteBlock = null;
                DataBytesDone = 0;
                DataBytesTotal = 0;
            }

            public MessageBlockDesc(uint magicID)
            {
                Debugger.Assert(magicID != ~0u, "Parameter 'id' can not be invalid value!");
                m_MagicID = magicID;

                BlockIndex = ~0u;
                BlockCount = ~0u;
                MessageID = ~0u;
                ByteBlock = null;
                DataBytesDone = 0;
                DataBytesTotal = 0;
            }

            public bool Continue
            {
                get { return DataBytesDone < DataBytesTotal; }
            }

            public bool Equals(MessageBlockDesc other)
            {
                return this.MagicID == other.MagicID;
            }

            public override bool Equals(object other)
            {
                return this.MagicID == ((MessageBlockDesc)other).MagicID;
            }

            public override int GetHashCode()
            {
                return (int)m_MagicID;
            }

            static public bool operator == (MessageBlockDesc left, MessageBlockDesc right)
            {
                return left.Equals(right);
            }

            static public bool operator !=(MessageBlockDesc left, MessageBlockDesc right)
            {
                return !left.Equals(right);
            }
        }

        private readonly ITMNetMessageBufferPool m_MessageBufferPool;

        protected readonly Queue<MessageBlockDesc> m_SendByteBlockQueue;
        protected readonly Queue<MessageBlockDesc> m_ReceiveByteBlockQueue;
        protected MessageBlockDesc m_CurReceiveMessage;
        protected MessageBlockDesc m_CurSendMessage;

        protected readonly Socket m_Socket;
        protected readonly uint m_CacheSize;

        protected readonly byte[] m_ReceiveCache;
        protected readonly byte[] m_SendCache;

        protected uint m_ReceiveCacheSize;
        protected uint m_SendCacheSize;

        private EnumHelper<State> m_State;

        public NetSocket(Socket socket, uint cacheSize)
            : this(socket, null, cacheSize)
        {
            m_State = new EnumHelper<State>((uint)State.Ready);
        }

        private NetSocket(Socket socket, System.Net.IPAddress ip, uint cacheSize)
        {
            Debugger.Assert(null != socket || null != ip, "Parameter 'socket' or 'ip' at last one cannot be null!");

            if (cacheSize < 32)
            {
                Debugger.LogWarning("Cache size can not less than 32 bytes!, force set to default size:{0}", DefaultCacheSize);
                cacheSize = DefaultCacheSize;
            }

            if (cacheSize > 1024 * 1024)
            {
                Debugger.LogWarning("Cache size should not larger than 1 Mbytes!, force set to default size:{0}", DefaultCacheSize);
                cacheSize = DefaultCacheSize;
            }

            m_MessageBufferPool = ModuleManager.GetModule<ITMNetMessageBufferPool>();

            m_SendByteBlockQueue = new Queue<MessageBlockDesc>();
            m_ReceiveByteBlockQueue = new Queue<MessageBlockDesc>();

            m_CacheSize = cacheSize;
            m_ReceiveCache = new byte[m_CacheSize + SocketHeaderSize];
            m_SendCache = new byte[m_CacheSize + SocketHeaderSize];

            if (null == socket)
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_Socket = socket;

            m_CurReceiveMessage = MessageBlockDesc.Null;
            m_CurSendMessage = MessageBlockDesc.Null;
        }

        public EndPoint RemoteEndPoint
        {
            get { return m_Socket.RemoteEndPoint; }
        }

        public bool IsConnected
        {
            get { return null != m_Socket ? m_Socket.Connected:false; }
        }

        public uint CacheSize
        {
            get { return m_CacheSize; }
        }

        public int SendTimeout
        {
            get { return m_Socket.SendTimeout; }
            set { m_Socket.SendTimeout = value; }
        }

        public bool HasMessage
        {
            get
            {
                lock (m_ReceiveByteBlockQueue)
                {
                    MessageBlockDesc curDesc = m_ReceiveByteBlockQueue.Peek();
                    return curDesc.BlockCount <= m_ReceiveByteBlockQueue.Count;
                }
            }
        }

        public void Bind(EndPoint localEP)
        {
            m_Socket.Bind(localEP);
        }

        public void Listen(int backlog)
        {
            m_Socket.Listen(backlog);
        }

        public IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (m_State.HasFlag((int)State.Shutdown))
                return null;

            return m_Socket.BeginConnect(remoteEP, callback, state);
        }

        public void EndConnect(IAsyncResult asyncResult)
        {
            if (m_State.HasFlag((int)State.Shutdown))
                return;

            m_Socket.EndConnect(asyncResult);
        }
        
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            if (m_State.HasFlag((int)State.Shutdown))
                return null;

            return m_Socket.BeginAccept(callback, state);
        }

        public NetSocket EndAccept(IAsyncResult asyncResult)
        {
            if (m_State.HasFlag((int)State.Shutdown))
                return null;

            Socket acceptSocket = m_Socket.EndAccept(asyncResult);
            return new NetSocket(acceptSocket, m_CacheSize);
        }

        public void ReceiveMessage()
        {
            _ExcuteReceiveMessage();
        }

        public void _ExcuteReceiveMessage()
        {
            if (m_State.HasFlag((int)State.Shutdown))
                return;

            if (m_State.HasFlag((int)State.Receiving))
                return;

            m_State += (int)State.Receiving;
            m_Socket.BeginReceive(m_ReceiveCache, 0, m_ReceiveCache.Length, SocketFlags.None, _Thread_OnReceive, this);
        }

        public void PushMessage(ByteMessage message)
        {
            /// 将所有的ByteMessage中的byteBlock放到一个list中去以每次发送的byteBlock为最小单元，每个
            /// Byteblock的前几个字节是消息号
            if (message.HasBlock)
            {
                ByteMessage.Enumerator it = message.GetEnumerator();
                uint index = 0;
                lock (m_SendByteBlockQueue)
                {
                    while (it.MoveNext())
                        m_SendByteBlockQueue.Enqueue(new MessageBlockDesc(Utility.UUID.Create32BitUUID())
                        {
                            MessageID = message.ID,
                            ByteBlock = it.Current,
                            BlockCount = message.TotalBlock,
                            BlockIndex = index++,
                            DataBytesDone = 0,
                            DataBytesTotal = it.Current.Length,
                        });
                }
            }
            else
            {
                lock (m_SendByteBlockQueue)
                    m_SendByteBlockQueue.Enqueue(new MessageBlockDesc(Utility.UUID.Create32BitUUID())
                    {
                        MessageID = message.ID,
                        ByteBlock = null ,
                        BlockIndex = 0,
                        BlockCount = 0,
                        DataBytesDone = 0,
                        DataBytesTotal = 0
                    });
            }
        }

        public ByteMessage FetchMessage()
        {
            return _ExcuteFetchMessage();
        }

        public void Update()
        {
            if (m_SendByteBlockQueue.Count > 0)
                _ExcuteSendMessage();

            //if (m_ReceiveByteBlockQueue.Count == 0)
            //    _ExcuteReceiveMessage();
        }

        public void Shutdown()
        { 
            m_State += (int)State.Shutdown;
            if (null != m_Socket)
                m_Socket.Close();
        }

        protected ITMByteBlock _AcquireBuffer(uint acquireBytes,BufferUsage usage)
        {
            return m_MessageBufferPool.AcquireBuffer(acquireBytes, usage);
        }

        protected ITMByteBlock _AcquireBuffer(byte[] rawBytes, BufferUsage usage)
        {
            return m_MessageBufferPool.AcquireBuffer(rawBytes, usage);
        }

        private void _ExcuteSendMessage()
        {
            if (m_State.HasFlag((int)State.Sending))
                return;

            if (MessageBlockDesc.Null == m_CurSendMessage)
            {
                lock (m_SendByteBlockQueue)
                {
                    if (m_SendByteBlockQueue.Count > 0)
                        m_CurSendMessage = m_SendByteBlockQueue.Dequeue();
                }

                if (MessageBlockDesc.Null != m_CurSendMessage)/// 说明有内容需要发送
                {
                    ITMByteBlock headerBlock = _AcquireBuffer(m_SendCache,BufferUsage.Write);

                    headerBlock.Write(m_CurSendMessage.MagicID);
                    headerBlock.Write(m_CurSendMessage.DataBytesTotal);
                    headerBlock.Write(m_CurSendMessage.MessageID);
                    headerBlock.Write(m_CurSendMessage.BlockIndex);
                    headerBlock.Write(m_CurSendMessage.BlockCount);

                    Debugger.LogProcedure(this,"Begin Send -> magicID:{0} DataByteTotal:{1} msgID:{2} msgBlockIdx:{3} msgBlockCnt:{4}",
                        m_CurSendMessage.MagicID, m_CurSendMessage.DataBytesTotal, m_CurSendMessage.MessageID,
                        m_CurSendMessage.BlockIndex, m_CurSendMessage.BlockCount);

                    Debugger.Assert(headerBlock.Position == SocketHeaderSize, "Send block header is out of range!");
                    headerBlock.Recycle();
                    headerBlock = null;

                    int cacheSize = 0;
                    if (null != m_CurSendMessage.ByteBlock)
                        cacheSize = (int)_WriteToCache(m_CurSendMessage.ByteBlock, 0,
                            (int)m_CurSendMessage.DataBytesTotal, m_SendCache,
                            (int)SocketHeaderSize, m_SendCache.Length - (int)SocketHeaderSize);

                    Debugger.Assert(cacheSize == m_CurSendMessage.DataBytesTotal, "Send block is out of range!");

                    m_State += (int)State.Sending;
                    m_Socket.BeginSend(m_SendCache, 0, cacheSize + (int)SocketHeaderSize,
                        SocketFlags.None, _Thread_OnSend, this);
                }
            }
        }

        private ByteMessage _ExcuteFetchMessage()
        {
            ByteMessage curReceive = null;
            lock (m_ReceiveByteBlockQueue)
            {
                if (m_ReceiveByteBlockQueue.Count > 0)
                {
                    MessageBlockDesc curDesc = m_ReceiveByteBlockQueue.Peek();
                    if (curDesc.BlockCount <= m_ReceiveByteBlockQueue.Count)
                    {
                        if (0 == curDesc.BlockIndex)
                        {
                            curReceive = ByteMessage.Acquire();
                            curReceive.Fill(curDesc.MessageID);
                            m_ReceiveByteBlockQueue.Dequeue();

                            if (curDesc.BlockCount > 0)
                            {
                                curReceive.AddBlock(curDesc.ByteBlock);
                                int index = 1;
                                while (index < curDesc.BlockCount)
                                {
                                    curDesc = m_ReceiveByteBlockQueue.Dequeue();
                                    Debugger.Assert(curDesc.BlockIndex == index, "Byte block index is missing match!");
                                    curReceive.AddBlock(curDesc.ByteBlock);

                                    ++index;
                                }
                            }
                        }
                        else
                            Debugger.LogWarning("Byte message block head index is missing!");
                    }
                }
            }

            return curReceive;
        }

        static protected void _Thread_OnReceive(IAsyncResult ar)
        {
            try
            {
                NetSocket _this = (NetSocket)ar.AsyncState;
                if (_this.m_State.HasFlag((int)State.Shutdown))
                    return;

                int receiveBytes = _this.m_Socket.EndReceive(ar);
                _this.m_State -= (int)State.Receiving;

                Debugger.LogProcedure(_this, "On Receive -> Bytes:{0}", receiveBytes);

                if (receiveBytes > 0)
                {
                    _this.m_ReceiveCacheSize += (uint)receiveBytes;
                    if (MessageBlockDesc.Null == _this.m_CurReceiveMessage)
                    {
                        if (_this.m_ReceiveCacheSize >= SocketHeaderSize)
                        {
                            ITMByteBlock headerBlock = _this._AcquireBuffer(_this.m_ReceiveCache, BufferUsage.Read);
                            uint magicID = headerBlock.ReadUInt32();
                            uint dataBytesTotal = headerBlock.ReadUInt32();
                            uint msgID = headerBlock.ReadUInt32();
                            uint msgBlockIdx = headerBlock.ReadUInt32();
                            uint msgBlockCnt = headerBlock.ReadUInt32();

                            Debugger.LogProcedure(_this,"Begin Receive -> magicID:{0} DataByteTotal:{1} msgID:{2} msgBlockIdx:{3} msgBlockCnt:{4}",
                                magicID, dataBytesTotal, msgID, msgBlockIdx, msgBlockCnt);

                            ITMByteBlock msgBlock = null;
                            if (0 != dataBytesTotal)/// 有数据的命令
                                msgBlock = _this._AcquireBuffer(_this.CacheSize, BufferUsage.Read);

                            _this.m_CurReceiveMessage = new MessageBlockDesc(magicID)
                            {
                                MessageID = msgID,
                                BlockIndex = msgBlockIdx,
                                BlockCount = msgBlockCnt,
                                DataBytesDone = 0,
                                DataBytesTotal = dataBytesTotal,
                                ByteBlock = msgBlock,
                            };
                        }
                    }

                    if(_this.m_ReceiveCacheSize == _this.m_CurReceiveMessage.DataBytesTotal + SocketHeaderSize)
                    {/// 传输完成
                        uint copyBytes = 0;
                        if (null != _this.m_CurReceiveMessage.ByteBlock)
                        {
                            copyBytes = _ReadFromCache(_this.m_ReceiveCache, (int)SocketHeaderSize,
                                (int)_this.m_CurReceiveMessage.DataBytesTotal, _this.m_CurReceiveMessage.ByteBlock,
                                0, (int)_this.m_CurReceiveMessage.DataBytesTotal);

                            Debugger.Assert(copyBytes == _this.m_CurReceiveMessage.DataBytesTotal, "Socket cache out of range!");
                            _this.m_CurReceiveMessage.ByteBlock.Seek(0, SeekOrigin.Begin);
                        }

                        /// 当前Block已经接受完毕
                        lock (_this.m_ReceiveByteBlockQueue)
                        {
                            _this.m_ReceiveByteBlockQueue.Enqueue(_this.m_CurReceiveMessage);
                        }

                        _this.m_CurReceiveMessage = MessageBlockDesc.Null;
                        _this.m_ReceiveCacheSize = 0;
                    }

                    _this.m_State += (int)State.Receiving;
                    int receivedCacheBytes = (int)_this.m_ReceiveCacheSize;
                    _this.m_Socket.BeginReceive(_this.m_ReceiveCache, receivedCacheBytes, _this.m_ReceiveCache.Length - receivedCacheBytes, SocketFlags.None, _Thread_OnReceive, _this);
                }
                else
                {
                    Debugger.LogProcedure(_this, "Receive zero bytes, Stop receive...");
                }
            }
            catch (Exception e)
            {
                Debugger.LogException("Receive data with exception:{0}\n Stack trace:{1}", e.Message, e.StackTrace);
            }
        }

        static protected void _Thread_OnSend(IAsyncResult ar)
        {
            try
            {
                NetSocket _this = (NetSocket)ar.AsyncState;
                if (_this.m_State.HasFlag((int)State.Shutdown))
                    return;

                int sendBytes = _this.m_Socket.EndSend(ar);
                _this.m_State -= (int)State.Sending;

                Debugger.LogProcedure(_this, "On Send -> Bytes:{0}", sendBytes);
                
                _this.m_SendCacheSize += (uint)sendBytes;
                if (MessageBlockDesc.Null != _this.m_CurSendMessage)
                {
                    int bytesToSend = 0;
                    if (_this.m_SendCacheSize == _this.m_CurSendMessage.DataBytesTotal + SocketHeaderSize)
                    {/// 发送完毕
                        lock (_this.m_SendByteBlockQueue)
                        {
                            if (_this.m_SendByteBlockQueue.Count > 0)
                                _this.m_CurSendMessage = _this.m_SendByteBlockQueue.Dequeue();
                            else
                                _this.m_CurSendMessage = MessageBlockDesc.Null;
                        }
                        _this.m_SendCacheSize = 0;

                        if (MessageBlockDesc.Null != _this.m_CurSendMessage)
                        {
                            ITMByteBlock headerBlock = _this._AcquireBuffer(_this.m_SendCache, BufferUsage.Write);

                            headerBlock.Write(_this.m_CurSendMessage.MagicID);
                            headerBlock.Write(_this.m_CurSendMessage.DataBytesTotal);
                            headerBlock.Write(_this.m_CurSendMessage.MessageID);
                            headerBlock.Write(_this.m_CurSendMessage.BlockIndex);
                            headerBlock.Write(_this.m_CurSendMessage.BlockCount);

                            Debugger.LogProcedure(_this, "Begin Send -> magicID:{0} DataByteTotal:{1} msgID:{2} msgBlockIdx:{3} msgBlockCnt:{4}",
                                _this.m_CurSendMessage.MagicID, _this.m_CurSendMessage.DataBytesTotal, _this.m_CurSendMessage.MessageID,
                                _this.m_CurSendMessage.BlockIndex, _this.m_CurSendMessage.BlockCount);

                            headerBlock.Recycle();
                            headerBlock = null;

                            int cacheSize = 0;
                            if (null != _this.m_CurSendMessage.ByteBlock)
                                cacheSize = (int)_WriteToCache(_this.m_CurSendMessage.ByteBlock,
                                    0, (int)_this.m_CurSendMessage.DataBytesTotal,
                                    _this.m_SendCache, (int)SocketHeaderSize,
                                    _this.m_SendCache.Length - (int)SocketHeaderSize);

                            Debugger.Assert(cacheSize == _this.m_CurSendMessage.DataBytesTotal, "Send block is out of range!");

                            bytesToSend = cacheSize + (int)SocketHeaderSize;
                        }
                    }
                    else
                        bytesToSend = (int)_this.m_CurSendMessage.DataBytesTotal + (int)SocketHeaderSize - (int)_this.m_SendCacheSize;

                    if (bytesToSend > 0)
                        _this.m_Socket.BeginSend(_this.m_SendCache, (int)_this.m_SendCacheSize, bytesToSend, SocketFlags.None, _Thread_OnSend, _this);
                }
            }
            catch (Exception e)
            {
                Debugger.LogException("Send data with exception:{0}\n Stack trace:{1}", e.Message, e.StackTrace);
            }
        }
        
        static protected uint _WriteToCache(ITMByteBlock src, int srcOffset, int srcSize, byte[] dst, int dstOffset, int dstSize)
        {
            int srcBytes = Utility.Math.Min(srcSize, (int)src.Length - srcOffset);
            int dstBytes = Utility.Math.Min(dstSize, dst.Length - dstOffset);
            int copyBytes = Utility.Math.Min(srcBytes, dstBytes);

            src.Seek(srcOffset, SeekOrigin.Begin);
            return src.ToBytes(dst, (uint)dstOffset, (uint)copyBytes);
        }

        static protected uint _ReadFromCache(byte[] src, int srcOffset, int srcSize, ITMByteBlock dst, int dstOffset, int dstSize)
        {
            int srcBytes = Utility.Math.Min(srcSize, src.Length - srcOffset);
            int dstBytes = Utility.Math.Min(dstSize, (int)dst.Length - dstOffset);
            int copyBytes = Utility.Math.Min(srcBytes, dstBytes);

            dst.Seek(dstOffset, SeekOrigin.Begin);
            return dst.FromBytes(src, (uint)srcOffset, (uint)copyBytes);
        }
    }
}