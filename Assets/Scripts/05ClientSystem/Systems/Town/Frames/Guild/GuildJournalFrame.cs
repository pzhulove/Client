using Protocol;
using ProtoTable;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum EJournalType
    {
        Date,
        Info
    }

    public class JournalDataItem
    {
        public EJournalType JournalType;
        public string Contnet;
    }
    public class GuildJournalFrame : ClientFrame
    {


        private ComUIListScript mList;
        private Button mCloseBtn;


        private List<JournalDataItem> mJournalItems = new List<JournalDataItem>();
        private int mRecordShowCount = 100;
        private int mRecordRefreshInterval = 10000;
        private float mDeltTime = 0f;
        private uint mUpTime = 0;

        private List<GuildEvent> mGuildEventList = new List<GuildEvent>();
        private Dictionary<string, List<JournalDataItem>> mAllDataItemDic = new Dictionary<string, List<JournalDataItem>>();
        protected override void _OnOpenFrame()
        {
            _BindEvt();
            _Init();

        }


        protected override void _OnCloseFrame()
        {
            _ClearData();
            _UnBindEvt();
        }


        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildJournalFrame";
        }

        protected override void _bindExUI()
        {
            mList = mBind.GetCom<ComUIListScript>("ScrollView");
            mCloseBtn = mBind.GetCom<Button>("CloseBtn");
            mCloseBtn.SafeAddOnClickListener(_OnCloseBtnClick);
        }
        
        protected override void _unbindExUI()
        {
            mList = null;
            mCloseBtn.SafeRemoveOnClickListener(_OnCloseBtnClick);
            mCloseBtn = null;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            mDeltTime += Time.deltaTime;
            if(mDeltTime * 1000 >= mRecordRefreshInterval)
            {
                mDeltTime = 0;
                GuildDataManager.GetInstance().RequsetGuildEvent(mUpTime);
            }
            
        }
        private void _Init()
        {
            mDeltTime = 0f;
            mUpTime = 0;
            mRecordShowCount = Utility.GetSystemValueFromTable(SystemValueTable.eType.SVT_GUILD_EVENT_SHOW_MAX);
            if (mList!=null&&!mList.IsInitialised())
            {
                mList.Initialize();
                mList.onItemVisiable = OnItemVisiable;
            }
            GuildDataManager.GetInstance().RequsetGuildEvent(mUpTime);
        }

        private void OnItemVisiable(ComUIListElementScript item)
        {
            if (item == null) return;
            if (item.m_index < 0 || item.m_index >= mJournalItems.Count) return;
            JournalItem journalItem = item.GetComponent<JournalItem>();
            if (journalItem == null) return;
            journalItem.Init(mJournalItems[item.m_index]);
        }
       
        private void _OnCloseBtnClick()
        {
            frameMgr.CloseFrame<GuildJournalFrame>();
        }

        private void _UpdateJournalList(GuildEvent[] guildEvents,uint upTime)
        {
            if (guildEvents == null) return;
            //取最新的100条数据
            for (int i = 0; i < guildEvents.Length; i++)
            {
                mGuildEventList.Add(guildEvents[i]);
            }
            mGuildEventList.Sort(_Sort);
            bool flag = mGuildEventList.Count > mRecordShowCount;
            if(flag)
            {
                List<GuildEvent> tmpEventList = new List<GuildEvent>();
                for (int i = 0; i < mRecordShowCount; i++)
                {
                    tmpEventList.Add(mGuildEventList[i]);
                }
                mGuildEventList = tmpEventList;
            }

            //将在相同的天的日志放到一起
            mAllDataItemDic.Clear();
            for (int i = 0; i < mGuildEventList.Count; i++)
            {
               GuildEvent guildEvent = mGuildEventList[i];
               if (guildEvent == null) continue;
               string keyTimeStr= TimeUtility.GetTimeFormatByYearMonthDay(guildEvent.addTime, _OnFormat);
                JournalDataItem journalDataItem = null;
               if (!mAllDataItemDic.ContainsKey(keyTimeStr))
               {
                    mAllDataItemDic.Add(keyTimeStr, new List<JournalDataItem>());
                    journalDataItem = new JournalDataItem() {JournalType=EJournalType.Date,Contnet=keyTimeStr};
                    mAllDataItemDic[keyTimeStr].Add(journalDataItem);
                }
                journalDataItem= new JournalDataItem() { JournalType = EJournalType.Info, Contnet = guildEvent.eventInfo};
                mAllDataItemDic[keyTimeStr].Add(journalDataItem);
            }

            mJournalItems.Clear();
            var iter= mAllDataItemDic.GetEnumerator();
            while(iter.MoveNext())
            {
                mJournalItems.AddRange(iter.Current.Value);
            }

            if(mList!=null)
            {
                mList.SetElementAmount(mJournalItems.Count);
            }

            mUpTime = upTime;
        }

        private void _BindEvt()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildEventListRes, _OnGuildEvenListRes);
        }
        
        private void _UnBindEvt()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildEventListRes, _OnGuildEvenListRes);
        }


        private void _OnGuildEvenListRes(UIEvent uiEvent)
        {
            WorldGuildEventListRes data = (WorldGuildEventListRes)uiEvent.Param1;
            if (data == null) return;
            _UpdateJournalList(data.guildEvents, data.uptime);
        }
        private int _Sort(GuildEvent x, GuildEvent y)
        {
            if(x.addTime>y.addTime)
            {
                return -1;
            }else if(x.addTime<y.addTime)
            {
                return 1;
            }
            return 0;
        }

        private string _OnFormat()
        {
            return "{0}.{1}.{2}";
        }

        private void _ClearData()
        {
            mDeltTime = 0f;
            mUpTime = 0;
            mJournalItems.Clear();
            mAllDataItemDic.Clear();
            mGuildEventList.Clear();
        }

    }
}
