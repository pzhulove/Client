using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using System.Threading;
using System.Net;

namespace Network
{
    public class NetProcess
    {
        private static NetProcess instance = null;
        protected float mAnimInterval = 0.0f;
        protected uint msgProcessTimeAcc = 0;
        protected uint msgTimerTimeAcc = 0;
        protected Queue<MsgDATA> msgQueue = new Queue<MsgDATA>();
        public uint recvMaxSequence = 0;
        private Mutex msgQueueMutex = new Mutex();

        protected EventRouter<uint> msgRouter = new EventRouter<uint>();

        public EventRouter<uint> msgHandler
        {
            get { return msgRouter; } 
        }

        public static EventRouter<uint> sMsgHandler
        {
            get { return instance.msgHandler; }
        }

        public void Clear()
        {
            recvMaxSequence = 0;
            instance.msgRouter.ClearAllEvents();
            instance.msgQueue.Clear();

            NetManager.instance.RegisterBaseHandler();
        }

        public static void AddMsgHandler(uint msgID,Action<MsgDATA> handler)
        {
#if !LOGIC_SERVER			
            RemoveMsgHandler(msgID, handler);
            sMsgHandler.AddEventHandler<MsgDATA>(msgID, handler);
#endif
        }

        public static void RemoveMsgHandler(uint msgID, Action<MsgDATA> handler)
        {
#if !LOGIC_SERVER						
            sMsgHandler.RemoveEventHandler<MsgDATA>(msgID, handler);
#endif
        }
        static public NetProcess Instance()
        {
            if(instance == null)
            {
                instance = new NetProcess();
            }

            return instance;
        }

        public void Init()
        {
            mAnimInterval = 0.0f;
        }

        public void PushQueue(MsgDATA data)
        {
            msgQueueMutex.WaitOne();

            if (Global.Settings.isDebug)
            {
                NetManager.instance.RecordReceivedMsg(data);
            }

            msgQueue.Enqueue(data);
            msgQueueMutex.ReleaseMutex();
        }

        public MsgDATA PopQueue()
        {
            msgQueueMutex.WaitOne();
            if(msgQueue.Count == 0)
            {
                msgQueueMutex.ReleaseMutex();
                return null;
            }

            MsgDATA msg = msgQueue.Dequeue();
            msgQueueMutex.ReleaseMutex();

            return msg;
        }

        public void Tick(uint deltaInMillSecs)
        {
            msgProcessTimeAcc += deltaInMillSecs;
            msgTimerTimeAcc += deltaInMillSecs;

            if(NetManager.Instance().Show)
            {
                NetManager.Instance().Log("net process tick...");
            }

            //if (msgProcessTimeAcc >= 50)
            {
                MsgProcess();
                msgProcessTimeAcc = 0;
            }
        }

        protected void MsgProcess()
        {
            while (true)
            {
                MsgDATA msg = PopQueue();
                if(msg == null)
                {
                    break;
                }

                try
                {
                    Process(msg);
                }
                catch (System.Exception ex)
                {
                    Logger.LogError("MsgProcess Exception:" + ex.ToString());
                }
            }
        }

        protected void Process(MsgDATA msg)
        {
            if(msg == null || msg.bytes == null)
            {
                return;
            }

            if(msg.serverType != ServerType.RELAY_SERVER)
            {
                NetManager.Instance().Log("Process msg {0} {1}", ProtocolHelper.instance.GetName(msg.id), msg.id);

            }

            if (msg.serverType == ServerType.GATE_SERVER && msg.sequence > recvMaxSequence)
            {
                recvMaxSequence = msg.sequence;
            }


            // 地下城开始协议
            if (msg.id == Protocol.SceneDungeonStartRes.MsgID || msg.id == Protocol.SceneNotifyEnterScene.MsgID)
            {
                byte[] key = new byte[16];
                for(int j = 0; j < 4; j++)
                {
                    int seed = IPAddress.HostToNetworkOrder((int)msg.sequence);
                    for (int i = 0; i < 4; i++)
                    {
                        byte value = (byte)(seed & 0xFF);
                        key[i] = value == 0 ? (byte)255 : value;
                        seed = seed >> 8;
                    }
                }
                if(ClientApplication.isEncryptProtocol)
                {
                    msg.bytes = Xxtea.XXTEA.Decrypt(msg.bytes, key);
                }

                if (msg.bytes == null || msg.bytes.Length < 16)
                {
                    Logger.LogErrorFormat("recv invalid dungeon start response, invalid length.");
                    return;
                }

                bool checkMd5 = false;
                for(int i = msg.bytes.Length - 16; i < msg.bytes.Length; i++)
                {
                    if(msg.bytes[i] != 0)
                    {
                        checkMd5 = true;
                    }
                }

                if(checkMd5 && msg.id == Protocol.SceneDungeonStartRes.MsgID)
                {
                    // md5检测
                    byte[] srcBytes = new byte[msg.bytes.Length - 16];
                    for (int i = 0; i < srcBytes.Length; i++)
                    {
                        srcBytes[i] = msg.bytes[i];
                    }
                    byte[] md5 = GameClient.DungeonUtility.GetMD5(srcBytes);
                    for (int i = 0; i < 16; i++)
                    {
                        int index = msg.bytes.Length - 16 + i;
                        byte value = msg.bytes[index];
                        if (value != md5[i])
                        {
                            Logger.LogErrorFormat("recv invalid dungeon start response, invalid md5.");
                            return;
                        }
                    }
                }
#if MG_TEST
//                 if(msg.id == Protocol.SceneDungeonStartRes.MsgID)
//                 {
//                     ulong roleid = 0;
// 
//                     roleid = GameClient.PlayerBaseData.GetInstance().RoleID;
//                     Logger.LogErrorFormat("该日志不是报错,只出现在体验服,接收到单局开始协议,RoleId = {0}", roleid);
//                 } 
#endif
            }

            msgRouter.BroadCastEvent<MsgDATA>(msg.id, msg);

            _OnReciveMsgData(msg);
        }

        public delegate void RecivedMsgData(MsgDATA msgData);
        public static RecivedMsgData onReciveMsgData;

        private static void _OnReciveMsgData(MsgDATA msgData)
        {
#if UNITY_EDITOR
            try
            {
                if (null != onReciveMsgData)
                {
                    onReciveMsgData(msgData);
                }
            }
            catch { }
#endif
        }
    }
}
