using System;
using System.Collections.Generic;
using Network;
using Protocol;

namespace GameClient
{
    public class WaitNetMessageManager : DataManager<WaitNetMessageManager>
    {
        public class NetMessages
        {
            class Message
            {
                public uint ID;
                public List<MsgDATA> DataList;
            }
            List<Message> m_messages = new List<Message>();

            public void AddMessage(uint msgID)
            {
                if (_GetMessage(msgID) == null)
                {
                    _AddMessage(msgID);
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

            public bool IsAllMessageReceived()
            {
                for (int i = 0; i < m_messages.Count; ++i)
                {
                    if (m_messages[i].DataList == null)
                    {
                        return false;
                    }
                }
                return true;
            }

            public string GetUnReceivedMessageDesc()
            {
                string str = "[WaitNetMessageManager] 等待消息";
                for (int i = 0; i < m_messages.Count; ++i)
                {
                    Message message = m_messages[i];
                    if (message.DataList == null)
                    {
                        str += " ";
                        str += ProtocolHelper.GetInstance().GetName(message.ID);
                    }
                }
                str += "超时";
                return str;
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

            void _AddMessage(uint id)
            {
                Message message = new Message();
                message.ID = id;
                message.DataList = null;
                m_messages.Add(message);
            }
        }

        public interface IWaitData
        {

        }

        public abstract class WaitData : IWaitData
        {
            public uint[] nMsgIDs;
            public float fWaitTime;
            public bool bWaitForever;
            public bool bTimeOutQuit;
            public bool bModal;
            public Action timeOutHandle;
            public WaitNetMessageManager manager;

            public virtual void InitMsgIDs(params uint[] a_arrMsgIDs)
            {
                nMsgIDs = a_arrMsgIDs;
            }

            public abstract void HandleTimeOut();
            public abstract void HandleMsg(MsgDATA a_msgData);
        }

        public class WaitBase : WaitData
        {
            public Action<MsgDATA> handle;

            public override void HandleTimeOut()
            {
                Logger.LogProcessFormat("[WaitNetMessageManager] 等待消息 {0} 超时", ProtocolHelper.GetInstance().GetName(nMsgIDs[0]));

                if (manager != null)
                {
                    manager._UnRegister(this);
                }

                if (timeOutHandle != null)
                {
                    timeOutHandle.Invoke();
                }
            }

            public override void HandleMsg(MsgDATA a_msgData)
            {
                Logger.LogProcessFormat("[WaitNetMessageManager] 收到消息 {0}", ProtocolHelper.GetInstance().GetName(nMsgIDs[0]));

                if (manager != null)
                {
                    manager._UnRegister(this);
                }
                if (handle != null)
                {
                    handle(a_msgData);
                }
            }
        };

        public class WaitSpecial<T> : WaitData where T : IProtocolStream, new()
        {
            public Action<T> handle;
            public T newData = new T();

            public override void HandleTimeOut()
            {
                Logger.LogProcessFormat("[WaitNetMessageManager] 等待消息 {0} 超时", ProtocolHelper.GetInstance().GetName(nMsgIDs[0]));

                if (manager != null)
                {
                    manager._UnRegister(this);
                }

                if (timeOutHandle != null)
                {
                    timeOutHandle.Invoke();
                }
            }

            public override void HandleMsg(MsgDATA a_msgData)
            {
                Logger.LogProcessFormat("[WaitNetMessageManager] 收到消息 {0}", ProtocolHelper.GetInstance().GetName(nMsgIDs[0]));

                if (manager != null)
                {
                    manager._UnRegister(this);
                }
                if (handle != null)
                {
                    if (a_msgData == null)
                    {
                        handle(default(T));
                    }
                    else
                    {
                        newData.decode(a_msgData.bytes);
                        handle(newData);
                    }
                }
            }
        };

        public class WaitMulti : WaitData
        {
            public Action<NetMessages> handle;

            public NetMessages m_netMessage = new NetMessages();

            public override void InitMsgIDs(params uint[] a_arrMsgIDs)
            {
                nMsgIDs = a_arrMsgIDs;
                for (int i = 0; i < nMsgIDs.Length; ++i)
                {
                    m_netMessage.AddMessage(nMsgIDs[i]);
                }
            }

            public override void HandleTimeOut()
            {
                Logger.LogProcessFormat(m_netMessage.GetUnReceivedMessageDesc());

                if (manager != null)
                {
                    manager._UnRegister(this);
                }
                if (timeOutHandle != null)
                {
                    timeOutHandle.Invoke();
                }
            }

            public override void HandleMsg(MsgDATA a_msgData)
            {
                Logger.LogProcessFormat("[WaitNetMessageManager] 收到消息 {0}", ProtocolHelper.GetInstance().GetName(a_msgData.id));

                m_netMessage.SetMessageData(a_msgData.id, a_msgData);

                if (m_netMessage.IsAllMessageReceived())
                {
                    if (manager != null)
                    {
                        manager._UnRegister(this);
                    }

                    if (handle != null)
                    {
                        handle(m_netMessage);
                    }
                }
            }
        }

        List<WaitData> m_arrWaitMsgData = new List<WaitData>();

        public override void Initialize()
        {

        }

        public override void Clear()
        {
            while (m_arrWaitMsgData.Count > 0)
            {
                _UnRegister(m_arrWaitMsgData[0]);
            }
        }

        public void Update(float a_fElapsed)
        {
            for (int nIdx = 0; nIdx < m_arrWaitMsgData.Count; ++nIdx)
            {
                WaitData waitData = m_arrWaitMsgData[nIdx];
                if (waitData.bWaitForever == false)
                {
                    waitData.fWaitTime -= a_fElapsed;
                    if (waitData.fWaitTime <= 0.0f)
                    {
                        waitData.HandleTimeOut();
                        nIdx--;
                    }
                }
            }
        }

        public IWaitData Wait(uint a_nMsgID, Action<MsgDATA> a_handle, bool a_bModal = true, float a_fWaitTime = 15, Action a_timeOutHandle = null)
        {
            WaitBase waitMsgData = new WaitBase();
            waitMsgData.InitMsgIDs(a_nMsgID);
            waitMsgData.fWaitTime = a_fWaitTime;
            waitMsgData.bWaitForever = a_fWaitTime <= 0;
            waitMsgData.bModal = a_bModal;
            waitMsgData.handle = a_handle;
            waitMsgData.timeOutHandle = a_timeOutHandle;
            waitMsgData.manager = this;
            _Register(waitMsgData);

            return waitMsgData;
        }

        public IWaitData Wait<T>(Action<T> a_handle, bool a_bModal = true, float a_fWaitTime = 15, Action a_timeOutHandle = null) where T : IProtocolStream, IGetMsgID, new()
        {
            WaitSpecial<T> waitMsgData = new WaitSpecial<T>();
            waitMsgData.InitMsgIDs(waitMsgData.newData.GetMsgID());
            waitMsgData.fWaitTime = a_fWaitTime;
            waitMsgData.bWaitForever = a_fWaitTime <= 0;
            waitMsgData.bModal = a_bModal;
            waitMsgData.handle = a_handle;
            waitMsgData.timeOutHandle = a_timeOutHandle;
            waitMsgData.manager = this;
            _Register(waitMsgData);

            return waitMsgData;
        }

		public WaitMulti Wait(uint[] a_arrMsgIDs, Action<NetMessages> a_handle, bool a_bModal = true, float a_fWaitTime = 15, Action a_timeOutHandle = null)
        {
            WaitMulti waitMsgData = new WaitMulti();
            waitMsgData.InitMsgIDs(a_arrMsgIDs);
            waitMsgData.fWaitTime = a_fWaitTime;
            waitMsgData.bWaitForever = a_fWaitTime <= 0;
            waitMsgData.bModal = a_bModal;
            waitMsgData.handle = a_handle;
            waitMsgData.timeOutHandle = a_timeOutHandle;
            waitMsgData.manager = this;
            _Register(waitMsgData);

            return waitMsgData;
        }

        public void CancelWait(IWaitData a_waitData)
        {
            if (a_waitData == null)
            {
                return;
            }

            _UnRegister(a_waitData as WaitData);
        }

        void _Register(WaitData a_waitData)
        {
            m_arrWaitMsgData.Add(a_waitData);
            // TODO 有时间看看能不能减少一层注册
            for (int i = 0; i < a_waitData.nMsgIDs.Length; ++i)
            {
                NetProcess.AddMsgHandler(a_waitData.nMsgIDs[i], a_waitData.HandleMsg);
            }

            if (a_waitData.bModal)
            {
                WaitNetMessageFrame.TryOpen();
            }
        }

        void _UnRegister(WaitData a_waitData)
        {
            if (m_arrWaitMsgData.Contains(a_waitData))
            {
                m_arrWaitMsgData.Remove(a_waitData);
                // TODO 有时间看看能不能减少一层注册
                for (int i = 0; i < a_waitData.nMsgIDs.Length; ++i)
                {
                    NetProcess.RemoveMsgHandler(a_waitData.nMsgIDs[i], a_waitData.HandleMsg);
                }
                if (a_waitData.bModal)
                {
                    WaitNetMessageFrame.TryClose();
                }
            }
        }
    }
}
