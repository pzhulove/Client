using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace Network
{
    enum UdpEventType
    {
        Connect,
        Msg,
        Disconnect
    }

    class UdpEvent
    {
        public UdpEventType type;

    }
    class UdpClient
    {
        ServerType          serverType;
        private byte[]      buffer = new byte[65535];
        IntPtr              handle;
        uint                accountId;
        string              serverIp;
        ushort              serverPort;
        bool                isConnected;
        byte[]              logFileName;

        public static FramePerformance    perf = new FramePerformance();

        Thread networkThread;
        Mutex mutex = new Mutex();
        Queue<UdpEvent> eventList = new Queue<UdpEvent>();

        void Lock()
        {
            mutex.WaitOne();
        }

        void Unlock()
        {
            mutex.ReleaseMutex();
        }

        public UdpClient(ServerType type)
        {
            serverType = type;
            isConnected = false;
            Initialize();

            networkThread = new Thread(new ParameterizedThreadStart(NetworkdThreadLoop));
            networkThread.Start(this);

            perf = new FramePerformance();
        }

        void Initialize()
        {
            Lock();

            string logFileNameStr = UnityEngine.Application.persistentDataPath + "/udp2.log";

            logFileName = StringHelper.StringToUTF8Bytes(logFileNameStr);

            handle = UdpDLL.avalon_udpconnector_initialize(logFileName);

            if (handle == null)
            {
                Logger.LogError("initialize udp failed.");
            }
            Unlock();
        }

        static void NetworkdThreadLoop(Object obj)
        {
            UdpClient client = (UdpClient)obj;
            byte[] buffer = new byte[65535];

            while(true)
            {
                client.Lock();
                if(client.handle == null)
                {
                    client.Unlock();
                    Thread.Sleep(10);
                    continue;
                }

                while (true)
                {
                    uint bufferLen = (uint)buffer.Length;
                    int eventType = UdpDLL.avalon_udpconnector_checkdata(client.handle, buffer, ref bufferLen);
                    if (eventType <= 0)
                    {
                        break;
                    }

                    if (eventType == 2)
                    {
                        // Disconnected
                        UdpEvent ev = new UdpEvent();
                        ev.type = UdpEventType.Disconnect;
                        client.eventList.Enqueue(ev);
                    }
                    else if (eventType == 3)
                    {
                        // Recv Data
                        client.OnReceived(buffer, bufferLen);
                    }
                    else
                    {
                        // Unknown Event Type
                    }
                }
                client.Unlock();

                Thread.Sleep(3);
            }
        }

        public bool IsConnected()
        {
            return isConnected;
        }

        public void SendLog2Http()
        {
            UdpDLL.avalon_udpconnector_save_log(logFileName);

			string url = string.Format("http://{0}:9527/udp?accid={1}", Global.GLOBAL_SERVER_ADDRESS, ClientApplication.playerinfo.accid);
            Http.UploadFile(url, StringHelper.BytesToString(logFileName));
            UdpDLL.avalon_udpconnector_open_log(logFileName);
        }

        ServerType GetServerType()
        {
            return serverType;
        }

        public bool Connect(string ip, ushort port, uint accid, uint timeout)
        {
            byte[] ipBytes = StringHelper.StringToUTF8Bytes(ip);

            Lock();
            int ret = UdpDLL.avalon_udpconnector_connect(handle, ipBytes, port, accid, timeout);
            Unlock();

            if (ret != 0)
            {
                Logger.LogErrorFormat("Connect To {0}:{1} failed, ret:{2}.", ip, port, ret);
                Reset();
                return false;
            }

            accountId = accid;
            serverIp = ip;
            serverPort = port;
            isConnected = true;

            return true;
        }

        public void Disconnect()
        {
            Lock();
            UdpDLL.avalon_udpconnector_disconnect(handle, accountId);
            Unlock();

            Reset();
        }

        public int SendCommandObject(object msgCmd)
        {
            Protocol.IProtocolStream msgStream = msgCmd as Protocol.IProtocolStream;
            Protocol.IGetMsgID       msgId     = msgCmd as Protocol.IGetMsgID;

            if (null == msgId || null == msgStream)
            {
                return -1;
            }

            // 先encode消息本体
            int pos = (int)NET_DEFINE.HEADER_SIZE;
            msgStream.encode(buffer, ref pos);

            // 先是2字节的包长度，再是4字节的消息号
            uint msgLen = (uint)pos - (uint)NET_DEFINE.HEADER_SIZE;
            pos = 0;
            BaseDLL.encode_uint16(buffer, ref pos, (ushort)IPAddress.HostToNetworkOrder((short)msgLen));
            BaseDLL.encode_uint32(buffer, ref pos, (uint)IPAddress.HostToNetworkOrder((int)msgId.GetMsgID()));

            return SendData(buffer, msgLen + (uint)NET_DEFINE.HEADER_SIZE);
        }

        public int SendCommand<CommandType>(CommandType cmd) where CommandType : Protocol.IProtocolStream, Protocol.IGetMsgID
        {
            return SendCommandObject(cmd);
        }

        public int SendData(byte[] data, uint len)
        {
            Lock();
            int ret = UdpDLL.avalon_udpconnector_senddata(handle, data, len, 1);
            Unlock();
            return ret;
        }

        public int Ping()
        {
            Lock();
            int ping = UdpDLL.avalon_udpconnector_ping(handle);
            Unlock();
            return ping;
        }

        public void Tick()
        {
            if(handle == null)
            {
                return;
            }

            
            while (true)
            {
                Lock();
                if (eventList.Count == 0)
                {
                    Unlock();
                    break;
                }

                UdpEvent ev = eventList.Dequeue();
                Unlock();

                if(ev.type == UdpEventType.Disconnect)
                {
                    // Disconnected
                    OnDisconnected();
                }
            }
        }

        protected void OnDisconnected()
        {
            NetManager.Instance().Log("Disconnected from {0}:{1}.", serverIp, serverPort);

            NetManager.Instance().PushSocketEvent(ServerType.RELAY_SERVER, SocketEventType.DISCONNECT);
            
            Reset();
        }

        protected void OnReceived(byte[] data, uint dataLen)
        {
            /*byte[] data = new byte[msgLen];
            for (int i = 0; i < msgLen; i++)
            {
                data[i] = msgData[i];
            }

            string msg = Encoding.Default.GetString(data);
            System.Console.WriteLine("Recv Data " + msg);*/

            if(dataLen < (uint)NET_DEFINE.HEADER_SIZE)
            {
                Logger.LogErrorFormat("Recveived Error Data, DataLen = " + dataLen);
                return;
            }

            ushort msgLen = 0;
            uint msgId = 0;
            int pos = 0;
            BaseDLL.decode_uint16(data, ref pos, ref msgLen);
            msgLen = (ushort)IPAddress.NetworkToHostOrder((short)msgLen);

            BaseDLL.decode_uint32(data, ref pos, ref msgId);
            msgId = (uint)IPAddress.NetworkToHostOrder((int)msgId);

            uint needLen = msgLen + (uint)NET_DEFINE.HEADER_SIZE;
            if (needLen != dataLen)
            {
                Logger.LogErrorFormat("Recveived Error Data, DataLen = {0}, NeedLen = {1}",dataLen, needLen);
                return;
            }

            MsgDATA msg = new MsgDATA(msgLen);
            msg.serverType = serverType;
            msg.id = msgId;
            for(int i = 0; i < msgLen; i++)
            {
                msg.bytes[i] = data[i + (uint)NET_DEFINE.HEADER_SIZE];
            }

            if(msg.id == Protocol.RelaySvrFrameDataNotify.MsgID)
            {
                Protocol.RelaySvrFrameDataNotify notify = new Protocol.RelaySvrFrameDataNotify();
                int pos_ = 0;
                notify.decode(msg.bytes, ref pos_);

                UInt32 nowTime = (UInt32)System.Environment.TickCount;
                for (int i = 0; i < notify.frames.Length; i++)
                {
                    var frame = notify.frames[i];
                    for(int j = 0; j < frame.data.Length; j++)
                    {
                        var input = frame.data[j];
                        if (input.seat == ClientApplication.playerinfo.seat && input.input.sendTime != 0)
                        {
                            uint delay = nowTime - input.input.sendTime;
                            perf.AddDelay(delay);
                        }
                    }
                }
            }

            NetProcess.Instance().PushQueue(msg);
        }

        public void Reset()
        {
            Lock();
            accountId = 0;
            serverIp = "";
            serverPort = 0;
            isConnected = false;

            if(handle != null)
            {
                UdpDLL.avalon_udpconnector_deinitialize(handle);
            }
            
            handle = UdpDLL.avalon_udpconnector_initialize(logFileName);
            if (handle == null)
            {
                Logger.LogError("initialize udp failed.");
            }

            Logger.LogProcessFormat("reset udp client.");

            Unlock();
        }
    }
}
