using Protocol;
using System.Collections.Generic;
using ProtoTable;
using System;
using Network;

namespace GameClient
{
    //public enum ActivityNoticeType
    //{
    //    GuildBattle = 1, // 公会战
    //    Budo, // 武道大会
    //    Jar, // 罐子
    //}

    //public struct NotifyInfo
    //{
    //    public ActivityNoticeType type;
    //}

    public class ActivityNoticeDataManager : DataManager<ActivityNoticeDataManager>
    {
        List<NotifyInfo> m_arrNotices = new List<NotifyInfo>();

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public override void Initialize()
        {
            Clear();

            NetProcess.AddMsgHandler(SceneUpdateNotifyList.MsgID, _OnAddNotify);
        }

        public override void Clear()
        {
            m_arrNotices.Clear();

            NetProcess.RemoveMsgHandler(SceneUpdateNotifyList.MsgID, _OnAddNotify);
        }

        public void AddActivityNotice(NotifyInfo a_info)
        {
            if(DeadLineReminderDataManager.IsDeadlineReminderType((NotifyType)a_info.type))
            {
                return;
            }

            bool bFind = false;

            for(int i = 0; i < m_arrNotices.Count; i++)
            {
                if(m_arrNotices[i].type == a_info.type && m_arrNotices[i].param == a_info.param)
                {
                    m_arrNotices[i] = a_info;
                    bFind = true;

                    break;
                }
            }

            if(!bFind)
            {
                m_arrNotices.Add(a_info);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityNoticeUpdate);
        }

        public void DeleteActivityNotice(NotifyInfo a_info)
        {
            for (int i = 0; i < m_arrNotices.Count; i++)
            {
                if (m_arrNotices[i].type == a_info.type && m_arrNotices[i].param == a_info.param)
                {
                    m_arrNotices.RemoveAt(i);
                    _SendMsgRemoveNotice(a_info);
                    break;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityNoticeUpdate);
        }

        public List<NotifyInfo> GetActivityNoticeDataList()
        {
#if APPLE_STORE
            //add by mjx for ios appstore
            m_arrNotices = NoticesFilterByJarType();
#endif
            return m_arrNotices;
        }

        //add by mjx for ios appstore
        List<NotifyInfo> NoticesFilterByJarType()
        {
            List<NotifyInfo> notifyInfos = new List<NotifyInfo>();
            if (m_arrNotices != null && notifyInfos != null)
            {
                for (int i = 0; i < m_arrNotices.Count; i++)
                {
                    var note = m_arrNotices[i];
                    if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR))
                    {
                        if (note.type == (uint)NotifyType.NT_JAR_OPEN || note.type == (uint)NotifyType.NT_JAR_SALE_RESET)
                        {
                            if (notifyInfos.Contains(note))
                            {
                                notifyInfos.Remove(note);
                            }
                        }
                    }
                    else
                    {
                        notifyInfos.Add(note);
                    }
                }
            }
            return notifyInfos;
        }

        void _OnAddNotify(MsgDATA msg)
        {
            SceneUpdateNotifyList msgData = new SceneUpdateNotifyList();
            msgData.decode(msg.bytes);

            AddActivityNotice(msgData.notify);
        }

        void _SendMsgRemoveNotice(NotifyInfo a_info)
        {
            if (a_info != null)
            {
                SceneDeleteNotifyList msg = new SceneDeleteNotifyList();
                msg.notify = a_info;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        //[EnterGameMessageHandle(SceneInitNotifyList.MsgID)]
        void _OnInitNotifyInfos(MsgDATA data)
        {
            SceneInitNotifyList msgData = new SceneInitNotifyList();
            msgData.decode(data.bytes);

            //m_arrNotices.Clear();

            for (int i = 0; i < msgData.notifys.Length; i++)
            {
                if (DeadLineReminderDataManager.IsDeadlineReminderType((NotifyType)msgData.notifys[i].type))
                {
                    continue;
                }

                m_arrNotices.Add(msgData.notifys[i]);
            }

            //m_arrNotices.AddRange(msgData.notifys);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityNoticeUpdate);
        }

        public override void OnBindEnterGameMsg()
        {
            EnterGameBinding eb = new EnterGameBinding();
            eb.id = SceneInitNotifyList.MsgID;

            try
            {
                eb.method = _OnInitNotifyInfos;
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("错误!! 绑定消息{0}(ID:{1})到方法", ProtocolHelper.instance.GetName(eb.id), eb.id);
            }

            m_arrEnterGameBindings.Add(eb);
        }
    }
}
