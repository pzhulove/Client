using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Timers;

namespace Network
{
    public delegate void DealReceive ( int msgid , byte[] msgbytes ) ;
    public delegate void TimeOutCallback ( object source , ElapsedEventArgs e ) ;
    public delegate void PushNetErrorCallback ( int errCode , string errInfo ) ;
    public delegate void ConnectCallback(bool success);

    public enum NETERROR_CODE
    {
        NETWORK_NOT_READY       = 0 ,
        CONNECT_TIME_OUT        = 1 ,
        CONNECT_SHUTDOWN        = 2 ,
    } ;

    public class NetworkSocket
    {
        ServerType serverType = ServerType.INVALID;
        public static int TIME_OUT_SECONDS              = 10 ;
        public static byte[] cmdBuffer = new byte[UInt16.MaxValue];
        private bool mCanSend = true;
        public bool canSend
        {
            get
            {
                return mCanSend;
            }

            set
            {
                if (mCanSend != value)
                {
#if !LOGIC_SERVER && MG_TEST
                    RecordServer.instance.PushReconnectCmd(string.Format("[NetworkSocket] can send value changed {0} -> {1}", mCanSend, value));
#endif
                }

                mCanSend = value;
            }
        }

        protected bool isInited                         = false ;

        protected string serverIP                       = "" ;
        protected int serverPort                        = 0 ;

        protected NetWorkBase netWorkBase               = null ;
        protected NetInputBuffer inBuffer               = null ;
        protected NetOutputBuffer outBuffer             = null ;
        private PacketBuffer packetBuffer = new PacketBuffer();

        protected int msgRealLength                     = -1 ;
        protected int msgReceiveLength                  = -1 ;
        protected bool newMsg                           = false ;
        protected static int msgSendLength              = -1 ;
        protected static int timeOutMilliseconds        = -1 ;
        protected int lastRequestID                     = -1 ;
        

        protected DealReceive ReceiveCallBack           = null ;

        protected System.Timers.Timer timer             = null ;
        protected PushNetErrorCallback timeoutCB        = null ;

        protected int lastRecvTime                   = 0;

        public NetworkSocket(ServerType type)
        {
            serverType                  = type;
            netWorkBase                 = new NetWorkBase(type);
            //初始化缓冲区
            inBuffer                    = new NetInputBuffer() ;
            outBuffer                   = new NetOutputBuffer(netWorkBase) ;

            timer                       = new System.Timers.Timer ( TIME_OUT_SECONDS * 1000 ) ;
            timer.Elapsed               += new ElapsedEventHandler ( OnTimeOut ) ;

            timeoutCB                   = null ;
              
            msgRealLength               = 9999999 ;
            msgReceiveLength            = 0 ;
            newMsg                      = true ;
            msgSendLength               = 0 ;
            timeOutMilliseconds         = TIME_OUT_SECONDS * 1000 ;

            isInited                    = true ;
        }

        string GetSocketName()
        {
            return serverIP + serverPort;
        }

        public bool IsConnected()
        {
            return netWorkBase.IsNetworkOK();
        }

        /// <summary>
        /// 用来连接服务器
        /// </summary>
        /// <param name="serverAddr">服务器的IP地址</param>
        /// <param name="serverPort">服务器网络端口</param>
        /// <param name="maxtimeOut">超时时间</param>
        /// <returns>
        /// false  : 连接失败
        /// true   : 成功连接
        /// </returns>
        public bool                         ConnectToServer ( string addr , int port , int maxtimeOut = 10000 )
        {
            //Logger.LogProcessFormat("[Net] start to connect {0}:{1}", addr, port);

            if ( isInited == false )
            {
                //Logger.LogProcessFormat( "NetworkClient is not initialized." ) ;
                return false ;
            }

            //先断开连接
            Disconnect() ;

            //开始连接
            serverIP = addr ;
            serverPort = port ;
#if APPLE_STORE
			lastRecvTime = System.Environment.TickCount;
#endif
			if ( !netWorkBase.Connect( serverIP , serverPort , maxtimeOut ) )
            {
                //Logger.LogFormat( "与服务端{0}建立连接失败\n",GetSocketName() ) ;
                return false ;
            }

            //Logger.Log( "与服务端建立连接成功" ) ;
            //Logger.Log( "ServerAddr: " + serverIP + " ServerPort: " + serverPort ) ;

            //先清除缓冲区
            inBuffer.CleanUp() ;
            outBuffer.CleanUp() ;

            //开始接收网络消息
            netWorkBase.Receive( inBuffer.GetRawBuffer() ,
                inBuffer.GetCurrentOffset() ,
                inBuffer.GetCurrentSize() ,
                ReceiveCallback ) ;
#if APPLE_STORE
			lastRecvTime = System.Environment.TickCount;
#endif
			return true ;
        }

        public void StartRecv()
        {
            //开始接收网络消息
            netWorkBase.Receive(inBuffer.GetRawBuffer(),
                            inBuffer.GetCurrentOffset(),
                            inBuffer.GetCurrentSize(),
                            ReceiveCallback);
        }
        public bool ReSendData(uint sequence)
        {
            int len = packetBuffer.FetchPacket(sequence, cmdBuffer);
            if (len > 0)
            {
#if !LOGIC_SERVER && MG_TEST
                 RecordServer.instance.PushReconnectCmd(string.Format("start to resend command from sequence({0}) len({1})", sequence, len));
#endif
                int writeLen = outBuffer.Write(ref cmdBuffer, len);
                if (writeLen != len)
                {
                    //Logger.LogErrorFormat("ReSendData len not equal from sequence({0}) bufferlen({1}) head ({2}) tail ({3}) fetchLen({4}) ", sequence, outBuffer.m_BufferLen, outBuffer.m_Head, outBuffer.m_Tail,len);
#if !LOGIC_SERVER && MG_TEST
                    RecordServer.instance.PushReconnectCmd(string.Format("ReSendData len not equal from sequence({0}) bufferlen({1}) head ({2}) tail ({3}) fetchLen({4}) ", sequence, outBuffer.m_BufferLen, outBuffer.m_Head, outBuffer.m_Tail, len));
#endif
                    return false;
                }
            }
            else if(len < 0)
            {
                //Logger.LogErrorFormat("ReSendData from len is zero sequence({0}) len({1})", sequence, len);
#if !LOGIC_SERVER && MG_TEST
                RecordServer.instance.PushReconnectCmd(string.Format("ReSendData from len is zero sequence({0}) len({1})", sequence, len));
#endif
                return false;
            }

            return true;

        }
        public void ClearReSendData()
        {
            packetBuffer.Clear();
#if !MG_TEST_EXTENT
            canSend = true;
#endif
        }
        public void ResetResend()
        {
            canSend = true;
        }

        public void                         ConnectToServerAsync ( string addr , int port , int maxtimeOut, ConnectCallback cb, int tryCount = 0)
        {
            //Logger.LogProcessFormat("[Net] start to connect {0}:{1}", addr, port);
            if (netWorkBase.GetCurrentStatus() == NetWorkBase.NETMANAGER_STATUS.CONNECTING)
            {
                //Logger.LogProcessFormat("NetworkClient is connecting.");
                return;
            }

            if ( isInited == false )
            {
                //Logger.LogProcessFormat( "NetworkClient is not initialized." ) ;
                return ;
            }

            //先断开连接
            if(netWorkBase.IsNetworkOK())
            {
                Disconnect();
            }
            
            //开始连接
            serverIP = addr ;
            serverPort = port ;

            lastRecvTime = System.Environment.TickCount;
            netWorkBase.ConnectAsync( serverIP , serverPort , maxtimeOut, (bool isDone, string errInfo) => 
                {
                    //UnityEngine.Debug.LogWarningFormat("connect to {0}:{1} {2} {3}", serverIP, serverPort, isDone ? "success" : "failed", errInfo);
                    if(isDone)
                    {
                        //先清除缓冲区
                        inBuffer.CleanUp();
                        outBuffer.CleanUp();

                        //开始接收网络消息
                        netWorkBase.Receive(inBuffer.GetRawBuffer(),
                                        inBuffer.GetCurrentOffset(),
                                        inBuffer.GetCurrentSize(),
                                        ReceiveCallback);

                        lastRecvTime = System.Environment.TickCount;

                        cb(true);
                    }
                    else
                    {
                        if(tryCount > 0)
                        {
							//Logger.LogErrorFormat("Connect Failed {0}, Try To Connect Again.", errInfo);
                            ConnectToServerAsync(addr, port, maxtimeOut, cb, tryCount - 1);
                        }
                        else
                        {
                            //Logger.LogError("Connect Failed, " + errInfo);
                            cb(false);
                        }
                    }
                }) ;
        }

        public int SendCommandObject(object msgCmd)
        {
            Protocol.IProtocolStream msgStream = msgCmd as Protocol.IProtocolStream;
            Protocol.IGetMsgID msgId = msgCmd as Protocol.IGetMsgID;

            if (null == msgId || null == msgStream)
            {
                return -1;
            }

#if DEBUG_SETTING
            if (Global.Settings.isDebug)
            {
                string log = string.Format("Send msg {0} {1}", ProtocolHelper.instance.GetName(msgId.GetMsgID()), msgId.GetMsgID());
                //ExceptionManager.GetInstance().RecordLog(log);
            }
#endif

            int pos = 0;
            msgStream.encode(cmdBuffer, ref pos);
            return SendData(msgId.GetMsgID(), msgId.GetSequence(), cmdBuffer, pos, 0);
        }

        public int SendCommand<CommandType>(CommandType cmd) where CommandType : Protocol.IProtocolStream, Protocol.IGetMsgID
        {
#if DEBUG_SETTING
			if (Global.Settings.isDebug)
			{
				string log = string.Format("Send msg {0} {1}", ProtocolHelper.instance.GetName(cmd.GetMsgID()), cmd.GetMsgID());
				//ExceptionManager.GetInstance().RecordLog(log);
			}
#endif
            
            int pos = 0;
            cmd.encode(cmdBuffer, ref pos);
            return SendData(cmd.GetMsgID(),cmd.GetSequence(), cmdBuffer, pos, 0);
        }

		public int                        SendData ( uint msgId,uint sequence, 
                                                        byte[] msgBytes , 
                                                        int msgLen,
                                                        int timeOutSeconds , 
                                                        PushNetErrorCallback timeOutCallback = null )
		{
#if MG_TEST
            if (sequence > 0 && msgId != Protocol.GateEnterGameReq.MsgID)
#else
            if (sequence > 0 && msgId != Protocol.GateEnterGameReq.MsgID)
#endif
            {

                packetBuffer.WritePacket(msgId, sequence, msgBytes, (ushort)msgLen);
                if(!canSend)
                {
#if !LOGIC_SERVER && MG_TEST
               //     if (ClientApplication.isOpenNewReportFrameAlgo)
                    {
                        RecordServer.instance.PushReconnectCmd(string.Format("[发送帧数据]SendData can not send {0}", msgId));
                    }
#endif
                    return -1;
                }
            }

            if (netWorkBase.GetCurrentStatus() != NetWorkBase.NETMANAGER_STATUS.CONNECTED)
            {
                NetManager.Instance().Log("{0} NetworkClient is not connected.", serverType);
                NetManager.Instance().PushSocketEvent(serverType, SocketEventType.DISCONNECT);
                return -1;
            }

            if (isInited == false)
            {
                NetManager.Instance().Log("{0} NetworkClient is not initialized.", serverType);
                return -2;
            }

            if (msgBytes == null)
            {
                msgLen = 0;
            }

            timeoutCB = timeOutCallback ;

            int oldTail = outBuffer.m_Tail;
            outBuffer.WriteUShort((ushort)msgLen);
            outBuffer.WriteUint(msgId);
            outBuffer.WriteUint(sequence);
            if (sequence > 0 && msgId == Protocol.SceneDungeonRaceEndReq.MsgID)
            {
#if !LOGIC_SERVER && MG_TEST
                RecordServer.instance.PushReconnectCmd(string.Format("[发送帧数据] SendData send {0}", msgId));
#endif
            }
            if (msgLen > 0 )
            {
                if(outBuffer.Write( ref msgBytes , msgLen) != msgLen)
                {
#if UNITY_EDITOR && NET_LOG
                    Logger.LogErrorFormat("[RECON] msg:{0} sequence:{1} write to buffer failed.", msgId, sequence);
#endif
                }
            }
                
            int newTail = outBuffer.m_Tail;
#if UNITY_EDITOR && NET_LOG
            Logger.LogErrorFormat("[RECON] {0} buffer tail {1}->{2}", serverType, oldTail, newTail);
#endif

            return msgLen + (int)NET_DEFINE.HEADER_SIZE;
		}

        /// <summary>
        /// 关闭socket,断开客户端与服务器的连线
        /// </summary>
        public void                     Disconnect()
        {
            if ( isInited == false )
            {
                //Logger.Log( "NetworkClient is not initialized." ) ;
                return ;
            }

            inBuffer.CleanUp() ;
            outBuffer.CleanUp() ;
            netWorkBase.ShutDown() ;
        }
        /// <summary>
        /// 网路的轮询函数
        /// </summary>
        /// <returns>
        /// false：失败
        //	true：成功
        /// </returns>
        public void                     Tick()
        {
            if ( isInited == false )
            {
                NetManager.Instance().Log("{0} NetworkClient is not initialized.", serverType) ;
                return ;
            }

            if (IsConnected() && System.Environment.TickCount - lastRecvTime >= 60 * 1000)
            {
                // 心跳超时
                NetManager.Instance().Log("Connection({0}) heartbeat timeout.", serverType);

                Disconnect();
                NetManager.Instance().PushSocketEvent(serverType, SocketEventType.DISCONNECT);
                return;
            }

            if (NetManager.Instance().Show)
            {
                //NetManager.Instance().Log("network socket{0} tick... outbuffer len:{1} head:{2} tail:{3}", serverType, outBuffer.Length(), outBuffer.m_Head, outBuffer.m_Tail);
            }

            try
            {
                if ( outBuffer.Length() == 0 )
                {
                    return ;
                }

                outBuffer.Flush() ;
            }
            catch ( Exception e )
            {
                NetManager.Instance().Log( "NetworkClient Tick Exception: " + e.ToString() ) ;
            }

            
        }

        //Call back...
        private void                    OnTimeOut( object source , ElapsedEventArgs e )
        {
            if ( isInited == false )
            {
                //Logger.Log( "NetWorkClient is not initialized.") ;
                return ;
            }

            if ( timeoutCB != null )
            {
                timeoutCB ( ( int ) NETERROR_CODE.CONNECT_TIME_OUT , "Request " + lastRequestID + " is not responsed." ) ;
            }
        }

        protected int                   ProcessCommand ()
        {
            ushort length = 0 ;
            int process_len = 0 ;
            while ( inBuffer.GetPackLength() + (int)NET_DEFINE.HEADER_SIZE <= inBuffer.Length() )
            {
                if ( timer.Enabled )
                {
                    timer.Stop() ;
                }

                inBuffer.ReadUShort( ref length ) ;
                
                uint msgid = 0;
                inBuffer.ReadUint(ref msgid);
                
                uint sequence = 0;
                inBuffer.ReadUint(ref sequence);
                process_len += (int)NET_DEFINE.HEADER_SIZE;

                process_len += length;

                NetManager.Instance().Log("receive msg:{0}", msgid);

                if ( length > 0 )
                {
                    byte[] msgbyte = new byte[length];
                    inBuffer.Read(ref msgbyte,length);

                    MsgDATA msg = new MsgDATA(length);
                    msg.serverType = serverType;
                    msg.id = msgid & (~(int)NET_DEFINE.COMPRESS_FLAG);
                    msg.sequence = sequence;
                    if((msgid & (int)NET_DEFINE.COMPRESS_FLAG) != 0)
                    {
                        msg.bytes = CompressHelper.Uncompress(msgbyte, msgbyte.Length);
                    }
                    else
                    {
                        msg.bytes = msgbyte;
                    }
                    
                    NetProcess.Instance().PushQueue( msg );
                }
                else
                {
                    MsgDATA msg = new MsgDATA(0);
                    msg.serverType = serverType;
                    msg.id = msgid;
                    msg.sequence = sequence;
                    NetProcess.Instance().PushQueue( msg );
                }

                lastRecvTime = System.Environment.TickCount;
            }

            return process_len ;
        }

        protected void                  ConnAsyncCallback ( bool isDone , string errInfo )
        {
            if ( isDone )
            {
                //Logger.Log( "与服务端建立连接成功" ) ;
                //Logger.Log( "ServerAddr: " + serverIP + " ServerPort: " + serverPort ) ;

                //先清除缓冲区
                inBuffer.CleanUp() ;
                outBuffer.CleanUp() ;

                //开始接收网络消息
                netWorkBase.Receive( inBuffer.GetRawBuffer() ,
                    inBuffer.GetCurrentOffset() ,
                    inBuffer.GetCurrentSize() ,
                    ReceiveCallback ) ;
            }
            else
            {
                //Logger.Log( "ConnAsync 与服务端建立连接失败" ) ;
                Disconnect() ;
            }
        }

        protected void                  ReceiveCallback ( bool isDone , int bytesRead , string errInfo )
        {
            if ( bytesRead > 0 )
            {
                //Begin receiving the data from the remote device.
                //接收缓冲区
                inBuffer.m_Tail += bytesRead ;
                msgReceiveLength += bytesRead ;
                if ( newMsg && msgReceiveLength >= (int)NET_DEFINE.HEADER_SIZE )
                {
                    msgRealLength = inBuffer.GetPackLength() + (int)NET_DEFINE.HEADER_SIZE;
                    newMsg = false ;
                }
                
                if ( msgReceiveLength >= msgRealLength )
                {
                    int pro_len = ProcessCommand() ;
                    msgReceiveLength -= pro_len ;
                    newMsg = true ;
                }

                //继续接受网络消息
                netWorkBase.Receive( inBuffer.GetRawBuffer() ,
                    inBuffer.GetCurrentOffset() ,
                    inBuffer.GetCurrentSize() ,
                    ReceiveCallback ) ;
            }
            else if ( bytesRead == 0 )
            {
                //Logger.Log( "服务端主动断开连接" + GetSocketName()) ;
                netWorkBase.ShutDown() ;

                NetManager.Instance().PushSocketEvent(serverType, SocketEventType.DISCONNECT);
            }
            else
            {
                //Logger.Log("连接出错" + GetSocketName());
                netWorkBase.ShutDown();

                NetManager.Instance().PushSocketEvent(serverType, SocketEventType.DISCONNECT);
            }
        }
    }
}
