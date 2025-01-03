using Protocol;
using System.Collections.Generic;
using ProtoTable;

namespace GameClient
{
    public struct AnnounceData
    {
        public int ID;
        public string content;
        public int priority;
    }

    public class AnnouncementManager : DataManager<AnnouncementManager>
    {
        float TimeInterval = 0.08f;
        float Interval = 0f;

        List<AnnounceData> ContentList = new List<AnnounceData>();

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public override void Initialize()
        {
            Clear();
        }

        public override void Clear()
        {
            TimeInterval = 0.05f;
            Interval = 0f;
            ContentList.Clear();
        }

        // 转发服务器消息
        public void AnnounceFromServer(SysAnnouncement res)
        {
            var AnnouncementData = TableManager.GetInstance().GetTableItem<AnnounceTable>((int)res.id);
            if (AnnouncementData == null)
            {
                return;
            }

            if (ContentList.Count == 0)
            {
                Interval = (TimeInterval + 0.01f);
            }

            AnnounceData newData = new AnnounceData();

            newData.ID = (int)res.id;
            newData.content = res.word;
            newData.priority = AnnouncementData.Priority;

            bool bInsertSuccess = false;

            for(int i = 0; i < ContentList.Count; i++)
            {
                if(ContentList[i].priority >= newData.priority)
                {
                    continue;
                }

                ContentList.Insert(i, newData);
                bInsertSuccess = true;
				break;
            }

            if(!bInsertSuccess)
            {
                ContentList.Add(newData);
            }
        }

        public void OnUpdate(float timeElapsed)
        {
            Interval += timeElapsed;

            if(Interval < TimeInterval)
            {
                return;
            }

            Interval = 0f;

            ClientSystemLogin clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemLogin;
            if (clientSystem != null)
            {
                ClientSystemManager.GetInstance().CloseFrame<AnnouncementFrame>();
                return;
            }

            ClientSystemTown townSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            ClientSystemBattle battleSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemBattle;

            if (townSystem == null && battleSystem == null)
            {
                ClientSystemManager.GetInstance().CloseFrame<AnnouncementFrame>();
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen("AnnouncementFrame"))
            {
                return;
            }

            PlayStart();
        }

        void PlayStart()
        {
            if (ContentList.Count <= 0)
            {
                return;
            }

            object param = ContentList[0] as object;
            AnnouncementFrame Announcement = null;
            if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>() ||
                ClientSystemManager.GetInstance().IsFrameOpen<FirstPayFrame>() ||
                ClientSystemManager.GetInstance().IsFrameOpen<SecondPayFrame>())
            {
                Announcement = ClientSystemManager.GetInstance().OpenFrame<AnnouncementFrame>(FrameLayer.BelowMiddle, param) as AnnouncementFrame;
            }
            else
            {
                Announcement = ClientSystemManager.GetInstance().OpenFrame<AnnouncementFrame>(FrameLayer.HorseLamp, param) as AnnouncementFrame;
            }
           
            Announcement.ResPlayEnd = PlayEnd;
        }

        void PlayEnd()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<AnnouncementFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AnnouncementFrame>();

                if(ContentList != null && ContentList.Count > 0)
                {
                    ContentList.RemoveAt(0);
                }

                Interval = 0f;
            }
        }
    }
}
