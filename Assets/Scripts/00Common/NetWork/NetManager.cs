using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using System.Threading;
using System.Diagnostics;

namespace Network
{
    
    public enum ServerType
    {
        INVALID,

        ADMIN_SERVER,
        GATE_SERVER,
        RELAY_SERVER,

        NUM,
    }

    public enum SocketEventType
    {
        CONNECTED,
        DISCONNECT,
    }
    struct SocketEvent
    {
        public ServerType serverType;
        public SocketEventType eventType;
        public UInt32 param1;
        public UInt32 param2;
    }

    public class NetManager : MonoSingleton<NetManager>
    {
        NetworkSocket cli2AdminServer;
        NetworkSocket cli2GateServer;
        UdpClient cli2RelayServer;
		NetworkSocket cli2TcpRelayServer;

        Queue<SocketEvent> socketEvents;
        Mutex socketEventsMutex = new Mutex();
        Queue<string> netLogs = new Queue<string>();
        Mutex netLogsMutex = new Mutex();
        UInt32 sequenceSeed = 0;
        Dictionary<UInt32, UInt32> reSendMsgIDList;

#region  DEBUG
         [HideInInspector]
        public List<string> recordSendedMsg = new List<string>();
        [HideInInspector]
        public List<string> recordReceivedMsg = new List<string>();

        public void RecordSendedMsg(string content)
        {
            if (recordSendedMsg.Count >= 100)
                recordSendedMsg.RemoveAt(0);

            recordSendedMsg.Add(content);
        }

        public void RecordReceivedMsg(MsgDATA data)
        {
            if (recordReceivedMsg.Count >= 100)
                recordReceivedMsg.RemoveAt(0);
            recordReceivedMsg.Add(string.Format("msg:{0}({1})", ProtocolHelper.instance.GetName(data.id), data.id));
        }

#endregion


		bool isTcpRelay = false;

        public static NetManager Instance()
        {
            return instance;
        }
        bool bInit = false;
        bool m_AllowForceReconnect = false;

        private NetworkReachability mCurNetworkState = NetworkReachability.NotReachable;

		public void SetIsTcp(bool useTcpRelay)
		{
			isTcpRelay = useTcpRelay;
		}

		public sealed override void Init() 
        {
            GameObject.DontDestroyOnLoad(gameObject);

            NetOutputBuffer.Init(10);

            cli2AdminServer = new NetworkSocket(ServerType.ADMIN_SERVER);
            cli2GateServer  = new NetworkSocket(ServerType.GATE_SERVER);
            cli2RelayServer = new UdpClient(ServerType.RELAY_SERVER);
			cli2TcpRelayServer = new NetworkSocket(ServerType.RELAY_SERVER);
			socketEvents    = new Queue<SocketEvent>();
            
            reSendMsgIDList = new Dictionary<uint, uint>();
            InitReSendMsgs();
            NetProcess.Instance().Init();
            RegisterBaseHandler();

            ProtocolHelper.CreateInstance();

            mCurNetworkState = Application.internetReachability;
            mStateChangeTime = 0;

            bInit = true;
        }
        
        public bool AllowForceReconnect
        {
            set { m_AllowForceReconnect = value; }
            get { return m_AllowForceReconnect; }
        }

        private float mEnterBackgroundTime = 0.0f;
        private float mResumeFregroundTime = 0.0f;

        private float kNeedReconnectGapTime = 6.0f;
        private float kResumeTime           = 8 * 60.0f;

        public bool Show = false;

        private bool _isTimeNeedReconnect()
        {
            return (mResumeFregroundTime - mEnterBackgroundTime) > kNeedReconnectGapTime;
        }

        private bool _isResumeTimeOut()
        {
            return (mResumeFregroundTime - mEnterBackgroundTime) > kResumeTime;
        }

        private void InitReSendMsgs()
        {
            SetMsgNeedReSend(Protocol.WorldDungeonEnterRaceReq.MsgID);
            SetMsgNeedReSend(Protocol.SceneDungeonEnterNextAreaReq.MsgID);
            SetMsgNeedReSend(Protocol.WorldDungeonReportFrameReq.MsgID);
            SetMsgNeedReSend(Protocol.SceneDungeonClearAreaMonsters.MsgID);
            SetMsgNeedReSend(Protocol.RelaySvrDungeonRaceEndReq.MsgID);
            SetMsgNeedReSend(Protocol.SceneDungeonRaceEndReq.MsgID);
//#if MG_TEST
            SetMsgNeedReSend(Protocol.GateEnterGameReq.MsgID);
//#else
//            SetMsgNeedReSend(Protocol.GateClientLoginReq.MsgID);
//#endif
            SetMsgNeedReSend(Protocol.SceneDungeonKillMonsterReq.MsgID);
            SetMsgNeedReSend(Protocol.SceneDungeonRewardReq.MsgID);
        }

        public void SetMsgNeedReSend(UInt32 msgId)
        {
            UInt32 value;
            if (reSendMsgIDList.TryGetValue(msgId, out value))
            {
                return;
            }

            reSendMsgIDList.Add(msgId, 0);
        }

        public void Awake()
        {
            Log("[NetManager] Awake");
        }

        public void OnEnable()
        {
            Log("[NetManager] OnEnable");
        }

        public void OnDisable()
        {
            Log("[NetManager] OnDisable");
        }

        protected sealed override void OnDestroy()
        {
            Log("[NetManager] OnDestroy");
        }
        public void OnApplicationPause(bool pause)
        {
            Log("[NetManager] OnApplicationPause({0}).", pause);

            if (pause)
            {
                mEnterBackgroundTime = Time.realtimeSinceStartup;
                Log("[NetManager] application pause. mEnter {0}, mResume {1}", mEnterBackgroundTime, mResumeFregroundTime);
            }
            else
            {
                mResumeFregroundTime = Time.realtimeSinceStartup;
                Log("[NetManager] application resume. mEnter {0}, mResume {1}", mEnterBackgroundTime, mResumeFregroundTime);
#if APPLE_STORE
				if(!isTcpRelay)
				{
#endif
					if (!cli2RelayServer.IsConnected())
					{
						cli2RelayServer.Reset();
					}

					if (_isResumeTimeOut())
					{
						GameClient.ClientReconnectManager.instance.ResumeTimeOut();
					}

					if (_isTimeNeedReconnect())
					{
						//_tryForceDisconnectServer();

						cli2RelayServer.Disconnect();
						Log("force disconnect relay...");
						NetManager.instance.PushSocketEvent(ServerType.RELAY_SERVER, SocketEventType.DISCONNECT);
					}
#if APPLE_STORE
				}
#endif
            }
        }

        private void _tryForceDisconnectServer()
        {
            if (_canForceDisconnectServer())
            {
                Log("force disconnect all server...");
                cli2GateServer.Disconnect();
                NetManager.instance.PushSocketEvent(ServerType.GATE_SERVER, SocketEventType.DISCONNECT);
#if APPLE_STORE
				if(isTcpRelay)
				{
					cli2TcpRelayServer.Disconnect();

				}
				else
				{
#endif
					cli2RelayServer.Disconnect();
#if APPLE_STORE
				}
#endif
                NetManager.instance.PushSocketEvent(ServerType.RELAY_SERVER, SocketEventType.DISCONNECT);
            }
        }

        public void RegisterBaseHandler()
        {
            NetProcess.AddMsgHandler(Protocol.HeartBeatMsg.MsgID, new Action<MsgDATA>(this.OnHeartBeat));
            NetProcess.AddMsgHandler(Protocol.GateSyncServerTime.MsgID, new Action<MsgDATA>(this.OnRecvGateSyncServerTime));
        }

        static long lastTime;
        static float lastShowTime;
        public void Update()
        {
            if(bInit == false)
            {
                Log("net manager init == false");
                return;
            }

            //long deltaTime = DateTime.Now.Ticks - lastTime;
            //lastTime = DateTime.Now.Ticks;
            //if (deltaTime / 10000 > 100)
            //{
            //    Log("net manager tick " + deltaTime / 10000);
            //}

            //Logger.LogProcessFormat("Net Manager Begin!");

            float now = Time.realtimeSinceStartup;
            if(now - lastShowTime >= 1.0f)
            {
                Show = true;
                //Log("net manager start tick...");
                lastShowTime = now;
            }

			uint delta = (uint)(Time.deltaTime * GlobalLogic.VALUE_1000);
            
            if (cli2AdminServer != null) cli2AdminServer.Tick();
            if (cli2GateServer != null) cli2GateServer.Tick();
#if APPLE_STORE
			if(isTcpRelay)
			{
				cli2TcpRelayServer.Tick();
			}
			else
			{
#endif          
                if (cli2RelayServer != null) cli2RelayServer.Tick();
#if APPLE_STORE
			}
#endif

            NetProcess.Instance().Tick(delta);

            _updateNetworkstate(delta);

            DispatchSocketEvent();
            RecordLogs();

            Show = false;

            //Logger.LogProcessFormat("Net Manager End!");
        }

        private long mStateChangeTime = 0;

        private const long kStateLoginChange = 2 * 1000;

        private void _updateNetworkstate(uint delta)
        {
            mStateChangeTime += delta;

            if (mStateChangeTime > kStateLoginChange)
            {
                mStateChangeTime = 0;
                NetworkReachability curStats = Application.internetReachability;

                if (curStats != mCurNetworkState)
                {
                    mCurNetworkState = curStats;

                    if(AllowForceReconnect)
                        _tryForceDisconnectServer();
                }
            }
        }

        private bool _isNetworkReachabilityValid()
        {
            return true;
            //return mCurNetworkState != NetworkReachability.NotReachable;
        }

        public void Reset()
        {
            cli2RelayServer.Disconnect();
        }

        public bool IsConnected(ServerType serverType)
        {
            if (!_isNetworkReachabilityValid())
            {
                return false;
            }

            if (serverType == ServerType.ADMIN_SERVER)
            {
                return cli2AdminServer.IsConnected();
            }
            else if (serverType == ServerType.GATE_SERVER)
            {
                return cli2GateServer.IsConnected();
            }
            else if (serverType == ServerType.RELAY_SERVER)
            {
#if APPLE_STORE
				if(isTcpRelay)
				{
					return cli2TcpRelayServer.IsConnected();
				}
				else
				{
#endif
					return cli2RelayServer.IsConnected();
#if APPLE_STORE
				}
#endif
            }

            return false;
        }
        public bool ConnectToAdmainServer(string ip, ushort port, uint timeout)
        {
            if (!_isNetworkReachabilityValid())
            {
                return false;
            }

            return cli2AdminServer.ConnectToServer(ip, port, (int)timeout);
        }

        public void ConnectToAdmainServerAsync(string ip, ushort port, uint timeout, ConnectCallback cb)
        {
            if (!_isNetworkReachabilityValid() && null != cb)
            {
                cb(false);
            }
            else
            {
                cli2AdminServer.ConnectToServerAsync(ip, port, (int)timeout, cb);
            }
        }

        public bool ConnectToGateServer(string ip, ushort port, uint timeout)
        {
            if (!_isNetworkReachabilityValid())
            {
                return false;
            }

            return cli2GateServer.ConnectToServer(ip, port, (int)timeout);
        }

        public void ConnectToGateServerAsync(string ip, ushort port, uint timeout, ConnectCallback cb)
        {
            if (!_isNetworkReachabilityValid() && null != cb)
            {
                cb(false);
            }
            else
            {
                cli2GateServer.ConnectToServerAsync(ip, port, (int)timeout, cb);
            }
        }

        public bool ConnectToRelayServer(string ip, ushort port, uint accid, uint timeout)
        {
            if (!_isNetworkReachabilityValid())
            {
                return false;
            }
#if APPLE_STORE
			if(isTcpRelay)
			{
				return cli2TcpRelayServer.ConnectToServer(ip, port, (int)timeout);
			}
			else
			{
#endif
				return cli2RelayServer.Connect(ip, port, accid, timeout);
#if APPLE_STORE
			}
#endif
        }

        public delegate void SendCommandCallback(ServerType serverType, Protocol.IGetMsgID msg, Protocol.IProtocolStream msgStream);
        public static SendCommandCallback onSendCommand;

        private void _OnSendCommand(ServerType type, Protocol.IGetMsgID msg, Protocol.IProtocolStream msgStream)
        {
#if UNITY_EDITOR
            try
            {
                if (null != onSendCommand)
                {
                    onSendCommand(type, msg, msgStream);
                }
            }
            catch { }
#endif
        }

        public int SendCommandObject(ServerType serverType, object msgCmd)
        {
            Protocol.IProtocolStream msgStream = msgCmd as Protocol.IProtocolStream;
            Protocol.IGetMsgID msgId = msgCmd as Protocol.IGetMsgID;

            if (null == msgId || null == msgStream)
            {
                return -1;
            }

            int ret = -1;

            if (Global.Settings.isDebug)
            {
                RecordSendedMsg(string.Format("{0}({1})", msgStream.GetType(), msgId.GetMsgID()));
            }


            if (serverType == ServerType.ADMIN_SERVER)
            {
                ret = cli2AdminServer.SendCommandObject(msgCmd);
            }
            else if (serverType == ServerType.GATE_SERVER)
            {
                // if (ClientApplication.isOpenNewReconnectAlgo && IsMsgNeedReSend(msgId.GetMsgID()))
                // {
                //     msgId.SetSequence(GenSequence());
                // }
                ret = cli2GateServer.SendCommandObject(msgCmd);
                Log("request to send cmd:{0}", msgId.GetMsgID());
            }
            else if (serverType == ServerType.RELAY_SERVER)
            {
                if (isTcpRelay)
                {
                    ret = cli2TcpRelayServer.SendCommandObject(msgCmd);
                }
                else
                {
                    ret = cli2RelayServer.SendCommandObject(msgCmd);
                }
            }

            _OnSendCommand(serverType, msgId, msgStream);

            //             if(ret == -1)
            //             {
            //                 Logger.LogErrorFormat("[网络] 发送消息 {0} 到 {1} 出错。错误码: {2}", cmd.GetType(), serverType.ToString(), ret);    
            //             }

            return ret;
        }

        public int SendCommand<CommandType>(ServerType serverType, CommandType cmd) where CommandType : Protocol.IProtocolStream, Protocol.IGetMsgID
        {
            int ret = -1;
            if (cmd == null) return ret;

            if (Global.Settings.isDebug)
            {
                RecordSendedMsg(string.Format("{0}({1})", cmd.GetType(), cmd.GetMsgID()));
            }

            if (serverType == ServerType.ADMIN_SERVER)
            {
                ret = cli2AdminServer.SendCommand(cmd);
            }
            else if (serverType == ServerType.GATE_SERVER)
            {
#if UNITY_EDITOR && NET_LOG
                DateTime date = DateTime.Now;
                string head = string.Format("[{0}-{1}-{2} {3}:{4}:{5}:{6}] ", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
                Logger.LogErrorFormat("[RECON] {0}send commend: {1}", head, cmd.GetType());

                if (cmd.GetMsgID() == Protocol.SceneDungeonRaceEndReq.MsgID)
                {
                    Logger.LogErrorFormat("send race end");
                }
#endif

#if APPLE_STORE
                if (ClientApplication.isOpenNewReconnectAlgo && IsMsgNeedReSend(cmd.GetMsgID()))
                {
#else
				if (IsMsgNeedReSend(cmd.GetMsgID()))
                {
#endif
                    cmd.SetSequence(GenSequence());
                }
                ret = cli2GateServer.SendCommand(cmd);
#if UNITY_EDITOR && NET_LOG
                Logger.LogErrorFormat("request to send cmd:{0} sequence:{1}", cmd.GetMsgID(), cmd.GetSequence());
#endif
            }
            else if (serverType == ServerType.RELAY_SERVER)
            {
#if APPLE_STORE
				if(isTcpRelay)
				{
					ret = cli2TcpRelayServer.SendCommand(cmd);
				}
				else
				{
#endif
					ret = cli2RelayServer.SendCommand(cmd);
#if APPLE_STORE
				}
#endif
            }

//             if(ret == -1)
//             {
//                 Logger.LogErrorFormat("[网络] 发送消息 {0} 到 {1} 出错。错误码: {2}", cmd.GetType(), serverType.ToString(), ret);    
//             }
         
            return ret;
        }

        UInt32 GenSequence()
        {
            return ++sequenceSeed;
        }
        public bool ReSendCommand(uint sequence)
        {
            if (cli2GateServer != null)
            {
#if UNITY_EDITOR && NET_LOG
                Logger.LogErrorFormat("[RECON]start to resend command from sequence({0})", sequence);
#endif
                return cli2GateServer.ReSendData(sequence);
            }

            return true;
        }
        bool IsMsgNeedReSend(UInt32 msgid)
        {
            UInt32 value;
            return reSendMsgIDList.TryGetValue(msgid, out value);
        }
        public void ClearReSendData()
        {
            if (cli2GateServer != null)
            {
                cli2GateServer.ClearReSendData();
            //    sequenceSeed = 0;
            }
        }
        public void ResetResend()
        {
            if (cli2GateServer != null)
            {
                cli2GateServer.ResetResend();
            }
        }


        public void Disconnect(ServerType serverType)
        {
            if (serverType == ServerType.ADMIN_SERVER)
            {
                cli2AdminServer.Disconnect();
            }
            else if (serverType == ServerType.GATE_SERVER)
            {
                cli2GateServer.Disconnect();
            }
            else if (serverType == ServerType.RELAY_SERVER)
            {
#if APPLE_STORE
				if(isTcpRelay)
				{
					cli2TcpRelayServer.Disconnect();
				}
				else
				{
#endif
					cli2RelayServer.Disconnect();
#if APPLE_STORE
				}
#endif
            }
            else
            {
                //Logger.LogError("Invalid ServerType " + serverType);
            }
        }

        public int GetPingToRelayServer()
        {
#if APPLE_STORE
			if(isTcpRelay)
			{
				return 0;
			}
			else
			{
#endif
				return cli2RelayServer.Ping();
#if APPLE_STORE
			}
#endif
        }

        private bool _canForceDisconnectServer()
        {
            return true;

            //return !GameClient.ClientSystemManager.instance.isSwitchSystemLoading;
        }
        public void OnCanSendData(ServerType serverType)
        {
            if (serverType == ServerType.GATE_SERVER)
            {
                cli2GateServer.canSend = true;
            }
        }

        public void OnDisconnected(ServerType serverType)
        {
            if ((serverType == ServerType.GATE_SERVER || serverType == ServerType.RELAY_SERVER))
            {
                if (serverType == ServerType.GATE_SERVER)
                {
#if !LOGIC_SERVER && MG_TEST
                    RecordServer.instance.PushReconnectCmd("[帧数据]GATE_SERVER disconnected");
#endif
                    cli2GateServer.canSend = false;
                }
                else
                {
#if !LOGIC_SERVER && MG_TEST
                    RecordServer.instance.PushReconnectCmd("[帧数据]RELAY_SERVER disconnected");
#endif
                }
                if (_canForceDisconnectServer())
                {
                    GameClient.IClientNet net = GameClient.ClientReconnectManager.instance as GameClient.IClientNet;
                    if (null != net)
                    {
                        net.OnDisconnect(serverType);
                    }
                }
            }
        }

        void OnConnected(ServerType serverType)
        {
			if (serverType == ServerType.ADMIN_SERVER)
			{
				cli2AdminServer.StartRecv();
			}
			else if (serverType == ServerType.GATE_SERVER)
			{
				cli2GateServer.StartRecv();
			}
#if APPLE_STORE
			else if (serverType == ServerType.RELAY_SERVER)
			{
				if (isTcpRelay)
				{
					cli2TcpRelayServer.StartRecv();
				}
			}
#endif
        }

        void DispatchSocketEvent()
        {
            socketEventsMutex.WaitOne();

            if (socketEvents != null)
            {
                while (socketEvents.Count > 0)
                {
                    SocketEvent socketEvent = socketEvents.Dequeue();
                    
                    if(socketEvent.eventType == SocketEventType.DISCONNECT)
                    {
                        OnDisconnected(socketEvent.serverType);
                    }
                    else if(socketEvent.eventType == SocketEventType.CONNECTED)
                    {
                        // 连接成功事件
                        // ToDo...
                    }
                    else
                    {
                        //Logger.LogError("unknown socket event:" + socketEvent.serverType.GetDescription());
                    }
                }
            }

            socketEventsMutex.ReleaseMutex();
        }

        public void PushSocketEvent(ServerType serverType, SocketEventType eventType, UInt32 param1 = 0, UInt32 param2 = 0)
        {
            SocketEvent socketEvent = new SocketEvent
            {
                serverType = serverType,
                eventType = eventType,
                param1 = param1,
                param2 = param2
            };

            socketEventsMutex.WaitOne();
            socketEvents.Enqueue(socketEvent);
            socketEventsMutex.ReleaseMutex();

            Log("socket:{0} push event:{1} param:{2},{3}", serverType, eventType, param1, param2);
        }

        public void UploadUdpLogs()
        {
            cli2RelayServer.SendLog2Http();
        }

        void OnHeartBeat(MsgDATA msg)
        {
            Log("Recv HeartBeat From " + (int)msg.serverType);

            Protocol.HeartBeatMsg heartMsg = new Protocol.HeartBeatMsg();
            NetManager.Instance().SendCommand(msg.serverType, heartMsg);
        }

        void OnRecvGateSyncServerTime(MsgDATA msgData)
        {
            GameClient.TimeManager.GetInstance().OnRecvGateSyncServerTime(msgData);
        }

        [Conditional("NET_LOG")]
        public void Log(string str, params object[] args)
        {
            DateTime date = DateTime.Now;
            string head = string.Format("[{0}-{1}-{2} {3}:{4}:{5}:{6}] ", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
            string log = string.Format(str, args);
            netLogsMutex.WaitOne();
            netLogs.Enqueue(head + log);
            netLogsMutex.ReleaseMutex();
        }

        [Conditional("NET_LOG")]
        void RecordLogs()
        {
            netLogsMutex.WaitOne();

            while(netLogs.Count > 0)
            {
                string log = netLogs.Dequeue();
#if UNITY_EDITOR
                Logger.LogErrorFormat(log);
#endif
                ExceptionManager.GetInstance().RecordLog(log);
            }

            netLogsMutex.ReleaseMutex();
        }
    }
}

