using System;
using System.Collections.Generic;
using Network;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Protocol;
using System.Diagnostics;

namespace GameClient
{
    public class MessageEvents
    {
        public class Message
        {
            public uint ID;
            public List<MsgDATA> DataList;
            public bool MustWait;
        }
        List<Message> m_messages = new List<Message>();


        public MessageEvents(uint msgID, bool mustWait = true)
        {
            AddMessage(msgID, mustWait);
        }

        public MessageEvents()
        {

        }

        public void AddMessage(uint msgID, bool mustWait = true)
        {
            if (_GetMessage(msgID) == null)
            {
                _AddMessage(msgID, mustWait);
            }
        }

        public void SetMessageData(uint msgID, MsgDATA data)
        {
            Message message = _GetMessage(msgID);
            if (message != null)
            {
                if (message.DataList == null)
                {
                    message.DataList = new List<MsgDATA>();
                }

                message.DataList.Add(data);
            }
        }

        public MsgDATA GetMessageData(uint msgID)
        {
            Message message = _GetMessage(msgID);
            if (message != null)
            {
                if (message.DataList != null)
                {
                    return message.DataList[0];
                }
            }
            return null;
        }

        public List<MsgDATA> GetMessageDatas(uint msgID)
        {
            Message message = _GetMessage(msgID);
            if (message != null)
            {
                return message.DataList;
            }
            return null;
        }

        public List<Message> GetMessages()
        {
            return m_messages;
        }

        public bool IsAllMessageReceived()
        {
            for (int i = 0; i < m_messages.Count; ++i)
            {
                if (m_messages[i].MustWait == true && m_messages[i].DataList == null)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetNotReceivedMessageID()
        {
            for (int i = 0; i < m_messages.Count; ++i)
            {
                if (m_messages[i].MustWait == true && m_messages[i].DataList == null)
                {
                    return (int)m_messages[i].ID;
                }
            }

            return -1;
        }

        public string GetUnreceivedMessageDesc()
        {
            string errorStr = "unreceived message:";
            for (int i = 0; i < m_messages.Count; ++i)
            {
                if (m_messages[i].DataList == null && m_messages[i].MustWait == true)
                {
                    errorStr = errorStr + ProtocolHelper.ID2Name(m_messages[i].ID) + " ";
                }
            }
            return errorStr;
        }

        Message _GetMessage(uint id)
        {
            for (int i = 0; i < m_messages.Count; ++i)
            {
                if (m_messages[i].ID == id)
                {
                    return m_messages[i];
                }
            }
            return null;
        }

        void _AddMessage(uint id, bool mustWait)
        {
            Message message = new Message();
            message.ID = id;
            message.DataList = null;
            message.MustWait = mustWait;
            m_messages.Add(message);
        }
    }
    public class MessageWait<T1,T2> where T1 : IProtocolStream, IGetMsgID
                                    where T2 : IProtocolStream, IGetMsgID
    {
        private Queue<T1> mReq;
        private T2 mRes;
        private Action<MsgDATA> _handle;
        private float _durTime;
        private float _timeOut;
        private ServerType _serverType;

        public delegate bool Comparor(T1 req, T2 res);
        public Comparor comparor = null;
        public MessageWait(ServerType serverType,T1 req,T2 res,float timeout = 20.0f)
        {
            _durTime = Time.time;
            _serverType = serverType;
            _timeOut = timeout;
            mRes = res;
            _handle =  new Action<MsgDATA>(_OnMessageReceived);
            mReq = new Queue<T1>();
            NetProcess.AddMsgHandler(mRes.GetMsgID(), _handle);
            int sendRet = NetManager.Instance().SendCommand(serverType, req);
            mReq.Enqueue(req);
        }
        public void DeInit()
        {
            NetProcess.RemoveMsgHandler(mRes.GetMsgID(), _handle);
            _handle = null;
            mReq.Clear();
            mRes = default(T2);
            mReq = null;
            comparor = null;
        }
        public void PushRequestQueue(T1 req)
        {
            mReq.Enqueue(req);
        }
        public bool IsWaitting()
        {
            if (mReq.Count <= 0)
            {
                return false;
            }
            float curTime = Time.time;
            float deltaTime = curTime - _durTime;
            if(deltaTime > _timeOut)
            {
                T1 sendData = mReq.Peek();
                _durTime = curTime;
                int sendRet = NetManager.Instance().SendCommand(_serverType, sendData);
            }
            return true;
        }
        private void _OnMessageReceived(MsgDATA data)
        {
            mRes.decode(data.bytes);
            T1 queueData = mReq.Peek();
            if (comparor != null && comparor(queueData, mRes))
            {
                mReq.Dequeue();
            }
        }
    }
    public class MessageUtility
    {
        /// <summary>
        /// 发送消息，并等待消息返回
        ///
        /// 例子：
        /// 
        ///  SceneDungeonStartReq req = new SceneDungeonStartReq();
        ///  req.dungeonId = 101000;
        ///
        ///  var msg = new MessageEvents();
        ///  var res = new SceneDungeonStartRes();
        ///
        ///  yield return MessageUtility.Wait<SceneDungeonStartReq, SceneDungeonStartRes>(msg, req, res, true);
        ///
        ///  if (msg.IsAllMessageReceived())
        ///  {
        ///      // 正常收到消息
        ///  }
        ///  else
        ///  {
        ///      // 超时
        ///  }
        /// </summary>
        ///
        /// <param name="msgEvents">发送消息对象</param>
        /// <typeparam name="T0">发送消息类型</typeparam>
        /// <typeparam name="T1">等待消息类型</typeparam>
        /// <param name="req">发送消息对象</param>
        /// <param name="res">等待消息对象</param>
        /// <param name="isShowWaitFrame">是否开启等待界面</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public static IEnumerator Wait<T0, T1>(ServerType serverType, MessageEvents msgEvents, T0 req, T1 res, bool isShowWaitFrame = false, float timeout = 20.0f)
                where T0 : IProtocolStream, IGetMsgID
                where T1 : IProtocolStream, IGetMsgID
        {
            if (msgEvents == null)
            {
                yield break;
            }

            if (req != null)
            {
                Logger.LogProcessFormat("[WaitNetMessage] 发送消息 {0}", ObjectDumper.Dump(req));
                NetManager.Instance().SendCommand(serverType, req);
            }

            if (res != null)
            {
                msgEvents.AddMessage(res.GetMsgID());

                yield return new WaitNetMessage(msgEvents, timeout, isShowWaitFrame);

                if (!msgEvents.IsAllMessageReceived())
                {
                    Logger.LogProcessFormat("[WaitNetMessage] 消息超时 {0}", res.GetType());
                    yield break;
                }

                var data = msgEvents.GetMessageData(res.GetMsgID());
                res.decode(data.bytes);

                Logger.LogProcessFormat("[WaitNetMessage] 收到消息 {0}", ObjectDumper.Dump(res));
            }
        }


        private const int kWaitCount       = 1;
        private const int kResendCount     = 2;
        private const float kResendTimeout = 3.5f;
        
        public static IEnumerator WaitWithResend<T0, T1>(ServerType serverType, MessageEvents msgEvents, T0 req, T1 res, bool isShowWaitFrame = true, float timeout = kResendTimeout)
                where T0 : IProtocolStream, IGetMsgID
                where T1 : IProtocolStream, IGetMsgID
        {
            Logger.LogProcessFormat("[WaitWithReconnect] 发送消息");

            if (msgEvents == null)
            {
                yield break;
            }

            int sendRet = -1;

            if (req != null)
            {
                Logger.LogProcessFormat("[WaitWithReconnect] 发送消息 {0}", ObjectDumper.Dump(req));
                sendRet = NetManager.Instance().SendCommand(serverType, req);
            }

            if (res != null)
            {
                msgEvents.AddMessage(res.GetMsgID());

                WaitNetMessage        waitNet     = new WaitNetMessage(msgEvents, -1, isShowWaitFrame);
                UnityEngine.Coroutine waitCo      = GameFrameWork.instance.StartCoroutine(waitNet);

                int                   waitCount   = 0;
                int                   resendCount = 0;
                float                 tmpTimeout  = timeout < 0 ? kResendTimeout : timeout;

                while (!msgEvents.IsAllMessageReceived())
                {
                    if (tmpTimeout > 0.0f)
                    {
                        tmpTimeout -= Time.unscaledDeltaTime;

                        //Logger.LogErrorFormat("等一阵");
                        yield return Yielders.EndOfFrame;

                    }
                    else
                    {
                        tmpTimeout = timeout < 0 ? kResendTimeout : timeout;;
                        //Logger.LogErrorFormat("等超市");

                        waitCount++;

                        Logger.LogProcessFormat("[WaitWithReconnect] 消息超时 {0}", res.GetType());

                        if (waitCount >= kWaitCount)
                        {
                            waitCount = 0;

                            // resend
                            resendCount++;

                            if (resendCount > kResendCount && timeout > 0)
                            {
                                // TODO 连接失败，退出到登录
                                break;
                            }

                            if (req != null && -1 == sendRet)
                            {
                                Logger.LogProcessFormat("[WaitWithReconnect] 重新发送消息 {0}", ObjectDumper.Dump(req));
                                sendRet = NetManager.Instance().SendCommand(serverType, req);
                            }
                        }
                    }
                }

                if (null != waitNet)
                {
                    waitNet.OnRemove();
                }

                if (null != waitCo)
                {
                    GameFrameWork.instance.StopCoroutine(waitCo);
                }

                if (msgEvents.IsAllMessageReceived())
                {
                    Logger.LogProcessFormat("[WaitWithReconnect] 收到消息 {0}", ObjectDumper.Dump(res));
                    var data = msgEvents.GetMessageData(res.GetMsgID());
                    res.decode(data.bytes);
                }
            }
        }
    }


    public class WaitNetMessage : IEnumerator, IEnumeratorLifeCycle
    {
#region IEnumeratorLifeCycle implementation
        public void OnAdd()
        {
        }

        public void OnRemove()
        {
            if (m_bShowWaitFrame && mHasOpenWaitNetMessageFrame)
            {
                WaitNetMessageFrame.TryClose();
                mHasOpenWaitNetMessageFrame = false;
            }
        }
#endregion

        private MessageEvents _msgEvnets;
        private Action<MsgDATA> _handle;
        private float m_time;
        private bool m_waitForever;
        private bool m_bShowWaitFrame;

        private bool mHasOpenWaitNetMessageFrame = true;

        public WaitNetMessage(MessageEvents msgEvents, float waitTime = 0, bool bShowWaitFrame = true)
        {
            Logger.LogProcessFormat("[WaitNetMessage] 等待时长 {0}", waitTime);
            _msgEvnets = msgEvents;
            _handle = new Action<MsgDATA>(_OnMessageReceived);
            m_time = waitTime;
            if (m_time <= 0.0f)
            {
                m_waitForever = true;
            }
            else
            {
                m_waitForever = false;
            }

            List<MessageEvents.Message> messages = _msgEvnets.GetMessages();
            for (int i = 0; i < messages.Count; ++i)
            {
                NetProcess.AddMsgHandler(messages[i].ID, _handle);
                Logger.LogProcessFormat("[WaitNetMessage] 等待消息 {0}", messages[i].ID);
            }

            m_bShowWaitFrame = bShowWaitFrame;

            if (m_bShowWaitFrame)
            {
                WaitNetMessageFrame.TryOpen();
                mHasOpenWaitNetMessageFrame = true;
            }
        }

        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            bool needWait = false;
            if (m_waitForever == false)
            {
                m_time -= Time.deltaTime;
                if (m_time > 0.0f)
                {
                    needWait = true;
                }
            }
            else
            {
                needWait = true;
            }

            if (needWait)
            {
                if (_msgEvnets.IsAllMessageReceived() == false)
                {
                    //Logger.LogProcessFormat("waitting.........");
                    return true;
                }
                else
                {
                    Logger.LogProcessFormat("[WaitNetMessage] 成功收到消息");
                    _RemoveMsgHandlers();
                    if (m_bShowWaitFrame)
                    {
                        WaitNetMessageFrame.TryClose();
                        mHasOpenWaitNetMessageFrame = false;
                    }
                    return false;
                }
            }
            else
            {
                Logger.LogProcessFormat("[WaitNetMessage] 等待消息超时");
                _RemoveMsgHandlers();
                _LogMessageUnReceived();
                if (m_bShowWaitFrame)
                {
                    WaitNetMessageFrame.TryClose();
                    mHasOpenWaitNetMessageFrame = false;
                }

                return false;
            }
        }

        public void Reset()
        {

        }

        protected void _OnMessageReceived(MsgDATA data)
        {
            if (data == null)
            {
                Logger.LogError("WaitNetMessage._OnMessageReceived ==>> data is null!!");
                return;
            }

            Logger.LogProcessFormat("[WaitNetMessage] 收到消息 {0}", data.GetType());

            _msgEvnets.SetMessageData(data.id, data);
        }

        protected void _RemoveMsgHandlers()
        {
            List<MessageEvents.Message> messages = _msgEvnets.GetMessages();
            for (int i = 0; i < messages.Count; ++i)
            {
                NetProcess.RemoveMsgHandler(messages[i].ID, _handle);
            }
        }

        protected void _LogMessageUnReceived()
        {
            List<MessageEvents.Message> messages = _msgEvnets.GetMessages();
            string errorStr = "time out !! unreceive message: ";
            for (int i = 0; i < messages.Count; ++i)
            {
                if (messages[i].DataList == null && messages[i].MustWait == true)
                {
                    errorStr = errorStr + ProtocolHelper.ID2Name(messages[i].ID) + " ";
                }
            }
            Logger.LogError(errorStr);
        }
    }

    class WaitNetMessageFrame : ClientFrame
    {
        //[UIObject("Content")]
        GameObject m_content;

        DelayCallUnitHandle m_delayCallUnit;

        protected override bool _isLoadFromPool()
        {
            return true;
        }

        protected override void _OnOpenFrame()
        {
			if (m_content == null)
				m_content = mBind.GetGameObject("root");

            m_content.SetActive(false);

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_delayCallUnit);

            m_delayCallUnit = ClientSystemManager.GetInstance().delayCaller.DelayCall(1500, _ShowContent);
            ms_count = 0;

            _OnReopenFrame(null);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WaitMessageReopen, _OnReopenFrame);
        }

        protected override void _OnCloseFrame()
        {
            ClientSystemManager.GetInstance().delayCaller.StopItem(m_delayCallUnit);
            ms_count = 0;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WaitMessageReopen, _OnReopenFrame);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/WaitingConnectFrame";
        }

        void _OnReopenFrame(UIEvent a_event)
        {
            if (frame != null)
            {
                frame.transform.SetAsFirstSibling();
            }
        }

        //public override bool IsNeedUpdate()
        //{
        //    return true;
        //}

        //protected override void _OnUpdate(float dt)
        //{
        //    if (null != frame)
        //    {
        //        frame.transform.SetAsFirstSibling();
        //    }
        //}

        static int ms_count = 0;

        static public void TryOpen()
        {
            Logger.LogProcessFormat("[WaitNetMessage] 尝试打开等待界面 目前累计计数 {0}, {1}", ms_count, ClientSystemManager.GetInstance().IsFrameOpen<WaitNetMessageFrame>() == false ? "打开" : "增加计数");
            if (ClientSystemManager.GetInstance().IsFrameOpen<WaitNetMessageFrame>() == false)
            {
                if (ms_count > 0)
                {
                    Logger.LogProcessFormat("[WaitNetMessage] 尝试打开等待界面 累计计数 {0} > 0 !!!!", ms_count);
                }

                ClientSystemManager.GetInstance().OpenFrame<WaitNetMessageFrame>(FrameLayer.TopMost);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WaitMessageReopen);
            }

            ms_count++;

            Logger.LogProcessFormat("[WaitNetMessage] 尝试打开等待界面 累计计数 {0}", ms_count);
        }

        static public void TryClose()
        {
            ms_count--;

            Logger.LogProcessFormat("[WaitNetMessage] 尝试关闭等待界面 目前累计计数 {0}, {1}", ms_count, ms_count <= 0 ? "关闭" : "减少计数");

            if (ms_count <= 0)
            {
                ClientSystemManager.GetInstance().CloseFrame<WaitNetMessageFrame>();
            }

            Logger.LogProcessFormat("[WaitNetMessage] 尝试关闭等待界面 累计计数 {0}", ms_count);
        }

        void _ShowContent()
        {
            if (null != m_content)
            {
                m_content.SetActive(true);
            }
        }
    }
}
