using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

#if APPLE_STORE
//IPV6转化
public class IPV6DLL
{
    [DllImport("__Internal")]
    private static extern string getIPv6(string mHost);

    public static string GetIPv6(string mHost)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost);
		return mIPv6;
#else
        return mHost + "&&ipv4";
#endif
    }

    public static void getIPType(string serverIp, out string newServerIp, out AddressFamily mIPType)
    {
        // 是点分10进制ip不解析直接返回
        IPAddress ia;
        if (System.Net.IPAddress.TryParse(serverIp, out ia))
        {
            newServerIp = serverIp;
            mIPType = AddressFamily.InterNetwork;
            return;
        }


        mIPType = AddressFamily.InterNetwork;
		IPHostEntry hostInfo = Dns.GetHostEntry(serverIp);
		if(hostInfo.AddressList.Length == 0)
		{
			newServerIp = "";
			mIPType = AddressFamily.AppleTalk;
			return;
		}
		
		newServerIp = hostInfo.AddressList[0].ToString();

		try
        {
            string mIPv6 = GetIPv6(serverIp);
            if (!string.IsNullOrEmpty(mIPv6))
            {
                string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                if (m_StrTemp != null && m_StrTemp.Length >= 2)
                {
                    string IPType = m_StrTemp[1];
                    if (IPType == "ipv6")
                    {
                        newServerIp = m_StrTemp[0];
                        mIPType = AddressFamily.InterNetworkV6;
                    }
                    else if(IPType == "ipv4")
                    {
						mIPType = AddressFamily.InterNetwork;
					}
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError("GetIPv6 error:" + e);
        }

    }
}
#endif

//罗亚,网络基础层，不做任何上层逻辑处理，只负责
//最基本的连接服务器，发送，接受，生成网络状态和错误码

namespace Network
{
    public delegate void NetWorkStatusCallback ( bool isDone , string errInfo ) ;
    public delegate void NetWorkReceiveCallback ( bool isDone , int bytesRead , string errInfo ) ;
	
    class SendContex
    {
        public Socket       sock;
        public MsgBuffer    buffer;
    }


    public class NetWorkBase
    {
        public enum NETMANAGER_STATUS
        {
            NO_CONNECT                      = -1 , //初始状态
            CONNECTED                       = 0 , //连接成功
            CONNECTING                      = 1 ,                       
        } ;

        ServerType serverType                       = ServerType.INVALID;
        protected bool isInited                     = false ;
        protected Socket client                     = null ;

        // ManualResetEvent instances signal completion.
        private ManualResetEvent connectDone        = new ManualResetEvent(false);

        private NETMANAGER_STATUS netStatus  = NETMANAGER_STATUS.NO_CONNECT ;

        private GamePool.MutexObjectPool<SendContex> sendContexPool = new GamePool.MutexObjectPool<SendContex>();

        protected NetWorkStatusCallback connCB      = null ;
        protected NetWorkStatusCallback sendCB      = null ;
        protected NetWorkReceiveCallback recvCB     = null ;
        protected NetWorkStatusCallback toutCB      = null ;

        public NetWorkBase(ServerType type)
        {
            if ( isInited == true )
            {
                Logger.Log( "NetWorkBase Init can only called once.") ;
                return ;
            }

            SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
            isInited = true ;
            serverType = type;
        }

        public NETMANAGER_STATUS    GetCurrentStatus ()
        {
            return netStatus ;
        }

        private void                SetCurrentStatus ( NETMANAGER_STATUS status )
        {
            NetManager.Instance().Log( "Connection:{0} SetCurrentStatus {1} -> {2}", serverType, netStatus, status) ;
            netStatus = status ;
        }

        public bool                 IsNetworkReachable ()
        {
            return !( Application.internetReachability == NetworkReachability.NotReachable ) ;
        }

        public bool                 IsNetworkOK ()
        {
            //return netStatus == NETMANAGER_STATUS.CONNECT_SUCESS ;
            return ( client != null ) && ( netStatus == NETMANAGER_STATUS.CONNECTED ) ;
        }

        public bool                 Connect ( string addr , int port , int MaxtimeOut , NetWorkStatusCallback cb = null )
        {
            //注册回调函数
            connCB = cb ;

            if ( isInited == false )
            {
                Logger.Log( "NetWorkBase is not initialized." ) ;
                if ( connCB != null )
                {
                    connCB( false , "NetWorkBase is not initialized." ) ;
                }
                return false ;
            }

            try
            {
#if APPLE_STORE
                // 获得v4或v6的ip和addrFamily
                string newServerIp = "";
                AddressFamily newAddressFamily = AddressFamily.InterNetwork;
                IPV6DLL.getIPType(addr, out newServerIp, out newAddressFamily);
                if (!string.IsNullOrEmpty(newServerIp)) { addr = newServerIp; }
#endif
                Logger.Log( addr ) ;
                Logger.Log( port.ToString() ) ;
#if APPLE_STORE
				IPAddress ip = IPAddress.Parse(newServerIp) ;  
#else
				IPAddress ip = IPAddress.Parse( addr ) ;  
#endif
				IPEndPoint remoteEP = new IPEndPoint( ip , port ) ;

                // Create a TCP/IP socket.
#if APPLE_STORE
                client = new Socket(newAddressFamily, SocketType.Stream , ProtocolType.Tcp ) ;
#else
			    client = new Socket( AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp ) ;
#endif
                if ( client == null )
                {
                    SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                    if ( connCB != null )
                    {
                        connCB( false , "Create Socket Failed" ) ;
                    }
                    return false ;
                }

                connectDone.Reset() ;
                SetCurrentStatus(NETMANAGER_STATUS.CONNECTING);
                client.BeginConnect( remoteEP , new AsyncCallback( ConnectCallback ) , client ) ;

                if ( connectDone.WaitOne( MaxtimeOut , false ) )
                {
                    if ( netStatus == NETMANAGER_STATUS.CONNECTED )
                    {
                        if ( connCB != null )
                        {
                            connCB( true , "" ) ;
                        }
                        return true ;
                    }
                    else
                    {
                        SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                        throw new Exception( "Connect Failed" ) ;
                    }
                }
                else
                {
                    SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                    throw new TimeoutException ( "Connect TimeOut" ) ;
                }
            }
            catch ( Exception e )
            {
                ShutDown() ;
                connectDone.Reset() ;
                Logger.Log( e.ToString() ) ;
                if ( connCB != null )
                {
                    connCB( false , "NetworkBase Connect: " + e.ToString() ) ;
                }
                return false ;
            }
        }

        public void                 ConnectAsync ( string addr , int port , int MaxtimeOut , NetWorkStatusCallback cb = null )
        {
            //注册回调函数
            connCB = cb ;

            if ( isInited == false )
            {
                Logger.Log( "NetWorkBase is not initialized." ) ;
                if ( connCB != null )
                {
                    connCB( false , "NetWorkBase is not initialized." ) ;
                }
                return ;
            }

            try
            {
#if APPLE_STORE
                // 获得v4或v6的ip和addrFamily
                string newServerIp = "";
                AddressFamily newAddressFamily = AddressFamily.InterNetwork;
                IPV6DLL.getIPType(addr, out newServerIp, out newAddressFamily);
                if (!string.IsNullOrEmpty(newServerIp)) { addr = newServerIp; }

                Logger.LogForNet("connect to {0}:{1}", newServerIp, port);
#else
				Logger.LogForNet("connect to {0}:{1}", addr, port);
#endif

#if APPLE_STORE
                IPAddress ip = IPAddress.Parse(newServerIp) ;  
#else 			
				IPAddress ip = IPAddress.Parse( addr ) ;
#endif
                IPEndPoint remoteEP = new IPEndPoint( ip , port ) ;

                // Create a TCP/IP socket.
#if APPLE_STORE
                client = new Socket(newAddressFamily, SocketType.Stream , ProtocolType.Tcp ) ;
#else 			
				client = new Socket( AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp ) ;
#endif
                if ( client == null )
                {
                    SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                    if ( connCB != null )
                    {
                        connCB( false , "Create Socket Failed" ) ;
                    }
                    return ;
                }

                connectDone.Reset();
                SetCurrentStatus(NETMANAGER_STATUS.CONNECTING);
                client.BeginConnect( remoteEP , new AsyncCallback( ConnectAsyncCallback ) , client ) ;

                Thread thread = new Thread(_OnWaitConnectAsyncReturnThread);
                thread.Start(MaxtimeOut);
            }
            catch ( Exception e )
            {
                ShutDown() ;
                Logger.LogFormat( "IP:{0} reason {1}",addr != null ? "empty":addr,e.ToString() ) ;
                if ( connCB != null )
                {
                    connCB( false , "NetworkBase Connect: " + e.ToString() ) ;
                }
            }
        }

        protected void _OnWaitConnectAsyncReturnThread(object obj)
        {
            int timeout = int.Parse(obj.ToString());
            if (connectDone.WaitOne(timeout, false))
            {
                if (netStatus == NETMANAGER_STATUS.CONNECTED)
                {
                    Logger.LogForNet("Async Connect Return Success.");
                    if (connCB != null)
                    {
                        connCB(true, "");
                    }
                    return;
                }
                else
                {
                    Logger.LogForNet("Async Connect Return Failed. NetStatus == " + netStatus.ToString());
                    SetCurrentStatus(NETMANAGER_STATUS.NO_CONNECT);
                    if (connCB != null)
                    {
                        connCB(false, "Connect Failed");
                    }
                }
            }
            else
            {
                Logger.LogForNet("Async Connect Timeout.");
                SetCurrentStatus(NETMANAGER_STATUS.NO_CONNECT);
                if (connCB != null)
                {
                    connCB(false, "Connect TimeOut");
                }
            }
        }

        public void                 ShutDown ()
        {
			if (GetCurrentStatus() == NETMANAGER_STATUS.NO_CONNECT)
			{
				//Logger.LogErrorFormat( "NetWorkBase is NO_CONNECT.") ;
				return ;
            }

            SetCurrentStatus(NETMANAGER_STATUS.NO_CONNECT);
            //ExceptionManager.Instance().LogError( "NetWorkBase ShutDown.") ;
            connectDone.Reset();
            if ( isInited == false )
            {
                Logger.Log( "NetWorkBase is not initialized.") ;
                return ;
            }

            try
            {
                if ( client != null )
                {
                    // Release the socket.
                    client.Shutdown( SocketShutdown.Both ) ;
                    client.Close() ;
                    client = null ;
                }
            }
            catch ( Exception e )
            {
                Logger.Log( e.ToString() ) ;
            }
        }

        public bool SendByPool(MsgBuffer buffer, NetWorkStatusCallback cb = null)
        {
            sendCB = cb;

            if (isInited == false)
            {
#if UNITY_EDITOR && NET_LOG
                Logger.LogErrorFormat("[RECON]{0} NetWorkBase is not initialized.", serverType);
#endif
                if (sendCB != null)
                {
                    sendCB(false, "NetWorkBase is not initialized.");
                }
                return true;
            }

            try
            {
                if (client == null)
                {
#if UNITY_EDITOR && NET_LOG
                    Logger.LogErrorFormat("[RECON]{0} NetworkBase Send: Socket is not created.", serverType);
#endif
                    if (sendCB != null)
                    {
                        sendCB(false, "NetworkBase Send: Socket is not created.");
                    }
                    return true;
                }

                if (netStatus != NETMANAGER_STATUS.CONNECTED)
                {
#if UNITY_EDITOR && NET_LOG
                    Logger.LogErrorFormat("[RECON]{0} NetworkBase Send: netStatus is not CONNECT_SUCESS.", serverType);
#endif
                    if (sendCB != null)
                    {
                        sendCB(false, "NetworkBase Send: Socket is not created.");
                    }
                    return true;
                }

#if UNITY_EDITOR && NET_LOG
                Logger.LogErrorFormat("[RECON]{0} begin to send data len:{1}...", serverType, buffer.length);
#endif
                SocketError errorCode;
                SendContex contex = sendContexPool.Get();
                contex.sock = client;
                contex.buffer = buffer;
                client.BeginSend(buffer.data, 0, buffer.length, SocketFlags.None, out errorCode, new AsyncCallback(SendCallbackWithPool), contex);


            }
            catch (Exception e)
            {
                ShutDown();
                SetCurrentStatus(NETMANAGER_STATUS.NO_CONNECT);
                Logger.Log(e.ToString());
                if (sendCB != null)
                {
                    sendCB(false, "NetworkBase Send Exception: " + e.ToString());
                }
            }

            return true;
        }

        public void                 Send ( byte[] byteData , int offset , int buffLen , NetWorkStatusCallback cb = null )
        {
            sendCB = cb ;

            if ( isInited == false )
            {
#if UNITY_EDITOR && NET_LOG
                Logger.LogErrorFormat("[RECON]{0} NetWorkBase is not initialized.", serverType);
#endif
                if ( sendCB != null )
                {
                    sendCB( false , "NetWorkBase is not initialized." ) ;
                }
                return ;
            }

            try
            {
                if ( client == null )
                {
#if UNITY_EDITOR && NET_LOG
                    Logger.LogErrorFormat("[RECON]{0} NetworkBase Send: Socket is not created.", serverType);
#endif
                    if ( sendCB != null )
                    {
                        sendCB( false , "NetworkBase Send: Socket is not created." ) ;
                    }
                    return ;
                }

                if ( netStatus != NETMANAGER_STATUS.CONNECTED )
                {
#if UNITY_EDITOR && NET_LOG
                    Logger.LogErrorFormat("[RECON]{0} NetworkBase Send: netStatus is not CONNECT_SUCESS.", serverType);
#endif
                    if ( sendCB != null )
                    {
                        sendCB( false , "NetworkBase Send: Socket is not created." ) ;
                    }
                    return ;
                }

#if UNITY_EDITOR && NET_LOG
                Logger.LogErrorFormat("[RECON]{0} begin to send data len:{1}...", serverType, buffLen);
#endif
                //current_send_len = buffLen ;
                SocketError errorCode;
                /*client.Send(byteData, offset, buffLen, SocketFlags.None, out errorCode);
                if (errorCode != SocketError.Success)
                {
                    Logger.LogErrorFormat("[RECON]socket:{0} send error: {1}", serverType, errorCode);
                    ShutDown();
                    NetManager.instance.PushSocketEvent(serverType, SocketEventType.DISCONNECT);
                    if (cb != null)
                    {
                        cb(false, "NetworkBase SendCallback Exception: " + errorCode);
                    }
                }*/

                client.BeginSend(byteData, offset, buffLen, SocketFlags.None, out errorCode, new AsyncCallback(SendCallback), client);
            }
            catch ( Exception e )
            {
                ShutDown() ;
                SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                Logger.Log( e.ToString() ) ;
                if ( sendCB != null )
                {
                    sendCB( false , "NetworkBase Send Exception: " + e.ToString() ) ;
                }
            }
        }

        public void                  Receive ( byte[] byteData , int offset , int size , NetWorkReceiveCallback cb = null )
        {
            recvCB = cb ;

            if ( isInited == false )
            {
                NetManager.Instance().Log( "{0} NetWorkBase is not initialized.", serverType) ;
                if ( recvCB != null )
                {
                    recvCB( false , -1 , "NetWorkBase is not initialized." ) ;
                }
                return ;
            }

            try
            {
                if ( client == null )
                {
                    NetManager.Instance().Log( "{0} NetworkBase Receive: Socket is not created.", serverType) ;
                    if ( recvCB != null )
                    {
                        recvCB( false , -1 , "NetworkBase Receive: Socket is not created." ) ;
                    }
                    return ;
                }

                if ( netStatus != NETMANAGER_STATUS.CONNECTED )
                {
                    NetManager.Instance().Log( "{0} NetworkBase Receive: netStatus is not CONNECT_SUCESS.", serverType) ;
                    if ( recvCB != null )
                    {
                        recvCB( false , -1 , "NetworkBase Receive: netStatus is not CONNECT_SUCESS." ) ;
                    }
                    return ;
                }

                client.BeginReceive( byteData , offset , size , SocketFlags.None ,
                                   new AsyncCallback( ReceiveCallback ) , client ) ;
            }
            catch ( Exception e )
            {
                NetManager.Instance().Log("{0} receive exception: {1}", serverType, e.ToString());
                ShutDown() ;
                SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                Logger.Log( e.ToString() ) ;
                if ( recvCB != null )
                {
                    recvCB( false , -1 , "NetworkBase Receive Exception: " + e.ToString() ) ;
                }
            }
        }

        private void                    ConnectCallback ( IAsyncResult ar )
        {
            if ( isInited == false )
            {
                Logger.Log( "NetWorkBase is not initialized." ) ;
                if ( connCB != null )
                {
                    connCB( false , "NetWorkBase is not initialized." ) ;
                }
                return ;
            }

            try
            {
                if ( client == null )
                {
                    Logger.Log( "NetworkBase ConnectCallback: Socket is not created." ) ;
                    if ( connCB != null )
                    {
                        connCB( false , "NetworkBase ConnectCallback: Socket is not created." ) ;
                    }
                    return ;
                }

                // Retrieve the socket from the state object.
                Socket socket = ( Socket ) ar.AsyncState ;
                if ( socket != null )
                {
                    // Complete the connection.
                    socket.EndConnect( ar ) ;
                    // Signal that the connection has been made.
                    SetCurrentStatus ( NETMANAGER_STATUS.CONNECTED ) ;

                    Logger.Log( "NetworkBase Connect Done." ) ;
                }
                else
                {
                    SetCurrentStatus(NETMANAGER_STATUS.NO_CONNECT);

                    Logger.Log("NetworkBase failed.");
                }
            }
            catch ( Exception e )
            {
                SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                Logger.Log( e.ToString() ) ;
                if ( connCB != null )
                {
                    connCB( false , "NetworkBase ConnectCallback Exception: " + e.ToString() ) ;
                }
                ShutDown() ;
            }
            finally
            {
                connectDone.Set() ;
            }
        }

        private void                    ConnectAsyncCallback ( IAsyncResult ar )
        {
            if ( isInited == false )
            {
                Logger.LogForNet("NetWorkBase is not initialized.") ;
                if ( connCB != null )
                {
                    connCB( false , "NetWorkBase is not initialized." ) ;
                }
                connectDone.Set();
                return ;
            }

            try
            {
                if ( client == null )
                {
                    Logger.LogForNet( "NetworkBase ConnectAsyncCallback: Socket is not created." ) ;
                    if ( connCB != null )
                    {
                        connCB( false , "NetworkBase ConnectAsyncCallback: Socket is not created." ) ;
                    }
                    return ;
                }

                // Retrieve the socket from the state object.
                Socket socket = ( Socket ) ar.AsyncState ;
                if ( socket != null )
                {
                    // Complete the connection.
                    socket.EndConnect( ar ) ;
                    // Signal that the connection has been made.
                    SetCurrentStatus ( NETMANAGER_STATUS.CONNECTED ) ;

                    Logger.LogForNet( "NetworkBase ConnectAsync Done." ) ;
                }
            }
            catch ( Exception e )
            {
                SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                Logger.LogForNet( e.ToString() ) ;
                if ( connCB != null )
                {
                    connCB( false , "NetworkBase ConnectAsyncCallback Exception: " + e.ToString() ) ;
                }
                ShutDown() ;
            }
            finally
            {
                connectDone.Set();
            }
        }

        private void SendCallbackWithPool(IAsyncResult ar)
        {
            SendContex contex = (SendContex)ar.AsyncState;

            if (isInited == false)
            {
                NetManager.Instance().Log("NetWorkBase is not initialized.");
                if (sendCB != null)
                {
                    sendCB(false, "NetWorkBase is not initialized.");
                }
                NetOutputBuffer.GetMsgPool().Release(contex.buffer);
                sendContexPool.Release(contex);
                return;
            }

            try
            {
                if (client == null)
                {
                    NetManager.Instance().Log("NetworkBase SendCallback: Socket is not created.");
                    if (sendCB != null)
                    {
                        sendCB(false, "NetworkBase SendCallback: Socket is not created.");
                    }
                    NetOutputBuffer.GetMsgPool().Release(contex.buffer);
                    sendContexPool.Release(contex);
                    return;
                }

                if (netStatus != NETMANAGER_STATUS.CONNECTED)
                {
                    NetManager.Instance().Log("NetworkBase SendCallback: netStatus is not CONNECT_SUCESS.");
                    if (sendCB != null)
                    {
                        sendCB(false, "NetworkBase SendCallback: netStatus is not CONNECT_SUCESS.");
                    }
                    NetOutputBuffer.GetMsgPool().Release(contex.buffer);
                    sendContexPool.Release(contex);
                    return;
                }

                // Retrieve the socket from the state object.
                Socket socket = contex.sock;
                SocketError errCode = SocketError.Success;
                // Complete sending the data to the remote device.
                int bytesSent = socket.EndSend(ar, out errCode);

                if (errCode != SocketError.Success)
                {
#if UNITY_EDITOR && NET_LOG
                    NetManager.Instance().Log("[RECON]{0} socket send error: {1}", serverType, errCode);
                    throw new Exception("Connect Failed");
#endif
                }

                if (errCode != SocketError.Success && errCode != SocketError.WouldBlock && errCode != SocketError.TryAgain && errCode != SocketError.Interrupted)
                {
                    NetManager.Instance().Log("Error while sending data from server. Error Code: " + errCode);
                    throw new Exception("Connect Failed");
                }
                if (errCode != SocketError.Success)
                {

                }

                if (sendCB != null)
                {
                    sendCB(true, "");
                }
            }
            catch (Exception e)
            {
                SetCurrentStatus(NETMANAGER_STATUS.NO_CONNECT);
                NetManager.instance.PushSocketEvent(serverType, SocketEventType.DISCONNECT);
                NetManager.Instance().Log("NetworkBase SendCallback Exception: " + e.ToString());
                if (sendCB != null)
                {
                    sendCB(false, "NetworkBase SendCallback Exception: " + e.ToString());
                }
                ShutDown();
            }

            NetOutputBuffer.GetMsgPool().Release(contex.buffer);
            sendContexPool.Release(contex);
        }

        private void                    SendCallback( IAsyncResult ar )
        {
            
            if ( isInited == false )
            {
                NetManager.Instance().Log( "NetWorkBase is not initialized." ) ;
                if ( sendCB != null )
                {
                    sendCB( false , "NetWorkBase is not initialized." ) ;
                }
                return ;
            }

            try
            {
                if ( client == null )
                {
                    NetManager.Instance().Log( "NetworkBase SendCallback: Socket is not created." ) ;
                    if ( sendCB != null )
                    {
                        sendCB( false , "NetworkBase SendCallback: Socket is not created." ) ;
                    }
                    return ;
                }

                if ( netStatus != NETMANAGER_STATUS.CONNECTED )
                {
                    NetManager.Instance().Log( "NetworkBase SendCallback: netStatus is not CONNECT_SUCESS." ) ;
                    if ( sendCB != null )
                    {
                        sendCB( false , "NetworkBase SendCallback: netStatus is not CONNECT_SUCESS." ) ;
                    }
                    return ;
                }

                // Retrieve the socket from the state object.
                Socket socket = ( Socket ) ar.AsyncState ;
                SocketError errCode = SocketError.Success ;
                // Complete sending the data to the remote device.
                int bytesSent = socket.EndSend( ar , out errCode ) ;

                if (errCode != SocketError.Success)
                {
#if UNITY_EDITOR && NET_LOG
                    NetManager.Instance().Log("[RECON]{0} socket send error: {1}", serverType, errCode);
                    throw new Exception("Connect Failed");
#endif
                }

                if (errCode != SocketError.Success && errCode != SocketError.WouldBlock && errCode != SocketError.TryAgain && errCode != SocketError.Interrupted)
                {
                    NetManager.Instance().Log("Error while sending data from server. Error Code: " + errCode);
                    throw new Exception("Connect Failed");
                }
                if ( errCode != SocketError.Success )
                {
                    
                }

                if ( sendCB != null )
                {
                    sendCB( true , "" ) ;
                }
            }
            catch ( Exception e )
            {
                SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                NetManager.instance.PushSocketEvent(serverType, SocketEventType.DISCONNECT);
                NetManager.Instance().Log( "NetworkBase SendCallback Exception: " + e.ToString() ) ;
                if ( sendCB != null )
                {
                    sendCB( false , "NetworkBase SendCallback Exception: " + e.ToString() ) ;
                }
                ShutDown() ;
            }
        }

        private void                    ReceiveCallback ( IAsyncResult ar )
        {
            if ( isInited == false )
            {
                NetManager.Instance().Log( "{0} NetWorkBase is not initialized.", serverType ) ;
                if ( recvCB != null )
                {
                    recvCB( false , -1 , "NetWorkBase is not initialized." ) ;
                }
                return ;
            }

            try
            {
                if ( client == null )
                {
                    NetManager.Instance().Log( "{0} NetworkBase ReceiveCallback: Socket is not created.", serverType) ;
                    if ( recvCB != null )
                    {
                        recvCB( false , -1 , "NetworkBase ReceiveCallback: Socket is not created." ) ;
                    }
                    return ;
                }

                if ( netStatus != NETMANAGER_STATUS.CONNECTED )
                {
                    NetManager.Instance().Log("{0} NetworkBase ReceiveCallback: invalid netStatus {1}.", serverType, netStatus);
                    if ( recvCB != null )
                    {
                        recvCB( false , -1 , "NetworkBase ReceiveCallback: netStatus is not CONNECT_SUCESS." ) ;
                    }
                    return ;
                }

                //收消息的
                Socket socket = ( Socket ) ar.AsyncState ;
                SocketError errCode = SocketError.Success ;
                int bytesRead = socket.EndReceive( ar , out errCode );

                if(errCode != SocketError.Success)
                {
                    Logger.LogForNet("socket recv error: {0}", errCode);
                }
                
                if ( errCode != SocketError.Success && errCode != SocketError.WouldBlock && errCode != SocketError.TryAgain && errCode != SocketError.Interrupted )
                {
                    NetManager.Instance().Log( "Error while receive data from server. Error Code: " + errCode ) ;
                    throw new Exception( "Connect Failed" ) ;
                }

                //具体如何收取数据交给回调函数处理
                if (recvCB != null)
                {
                    recvCB( true , bytesRead , "NetworkBase ReceiveCallback: netStatus is not CONNECT_SUCESS." ) ;
                }
            }
            catch ( Exception e )
            {
                SetCurrentStatus ( NETMANAGER_STATUS.NO_CONNECT ) ;
                Logger.Log( "NetworkBase ReceiveCallback Exception: Connect is shutdown." ) ;
                if ( recvCB != null )
                {
                    recvCB( false , -1 , "NetworkBase ReceiveCallback Exception: Connect is shutdown." ) ;
                }
                //ShutDown() ;
            }
        }
    }
}
