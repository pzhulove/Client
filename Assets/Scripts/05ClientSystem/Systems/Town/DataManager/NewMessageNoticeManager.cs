using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace GameClient
{
    public class NewMessageNoticeData
    {
        public NewMessageNoticeData(string tag)
        {
            this.tag = tag;
        }
        private string tag;

        public string mTag {
            get {return tag;}
        }
        public string mTitle;
        public string mMessage;
        public object mUserdata;
        public Action<NewMessageNoticeData> mForwardAction;

        public bool bReaded = false;

        private bool bNotUse;

        public bool mNotUse
        {
            get{return bNotUse;}
            set{
                bNotUse = value;
            }
        }
    }
    public class NewMessageNoticeManager : DataManager<NewMessageNoticeManager>
    {
        public override void Initialize()
        {
            mDataVersion = UInt64.MaxValue;
            if(newMessageNoticesList == null)
            {
                newMessageNoticesList = new List<NewMessageNoticeData>();
            }
            SetDataDirty();
        }

        public override void Clear()
        {
            ClearNewMessageNotice();
            SetDataDirty();
        }
        
        private UInt64 mDataVersion;
        private bool   mDataDirty;
        
        public UInt64 DataVersion
        {
            get{
                return mDataVersion;
            }
        }

        public bool DataDirty
        {
            get{
                return mDataDirty;
            }
        }

        
        public void SetDataDirty()
        {
            mDataVersion++;
            mDataDirty = true;
        }

        protected NewMessageNoticeData AddNewMessageNotice(string tag,string title,string message,object userdata,Action<NewMessageNoticeData> forwordAction,bool bUnique)
        {
            if(string.IsNullOrEmpty(tag))
            {
                Logger.LogError("[NewMessageNoticeManager] AddNewMessageNotice - Tag Can not be Null or Empty");
                return null;
            }
            if(bUnique)
            {
               RemoveNewMessageNoticeByTag(tag);
            }

            var data = new NewMessageNoticeData(tag){mTitle = title,mMessage = message,mUserdata = userdata,mForwardAction = forwordAction};
            newMessageNoticesList.Insert(0,data);

            SetDataDirty();
            return data;
        }

        public NewMessageNoticeData AddNewMessageNotice(string tag,object userdata,Action<NewMessageNoticeData> forwordAction,bool bUnique)
        {
            return AddNewMessageNotice(tag,TR.Value(tag+"_title"),TR.Value(tag+"_message"),userdata,forwordAction,bUnique);
        }

        public NewMessageNoticeData AddNewMessageNoticeWhenNoExist(string tag,object userdata,Action<NewMessageNoticeData> forwordAction)
        {
            if(IsNewMessageNoticeExsits(tag))
                return null;
            return AddNewMessageNotice(tag,userdata,forwordAction,true);
        }

        public void MarkAllRead()
        {
            for(int i = 0; i < mNewMessageNoticeCount; ++i)
            {
                var item = GetNewMessageNoticeByIndex(i);
                if(item != null)
                {
                    item.bReaded = true;
                }
            }

            SetDataDirty();
        }

        public int GetUnReadCount()
        {
            int iCount = 0;
            for(int i = 0; i < mNewMessageNoticeCount; ++i)
            {
                var item = GetNewMessageNoticeByIndex(i);
                if(item != null && item.bReaded == false)
                {
                   iCount++;
                }
            }

            return iCount;
        }
        public void RemoveNewMessageNotice(NewMessageNoticeData data)
        {
            SetDataDirty();
            newMessageNoticesList.Remove(data);
        }

        public void RemoveNewMessageNoticeByTag(string tag)
        {
            List<NewMessageNoticeData> items = newMessageNoticesList.FindAll(current => { return current.mTag.Equals(tag); });
            for (int i = 0; i < items.Count; ++i)
            {
                newMessageNoticesList.Remove(items[i]);
                SetDataDirty();
            }
        }

        public void ClearNewMessageNotice()
        {
            newMessageNoticesList.Clear();
            SetDataDirty();
        }


        public int mNewMessageNoticeCount
        {
            get{ return newMessageNoticesList.Count; }
        }

        public NewMessageNoticeData GetNewMessageNoticeByIndex(int index)
        {
            if(index < 0 || index >= mNewMessageNoticeCount)
            {
                return null;
            }
            
            return newMessageNoticesList[index];
        }

        public void Update()
        {
             newMessageNoticesList.RemoveAll(item => {return item.mNotUse;});
        }

        public List<NewMessageNoticeData> GetNewMessageNoticeByTag(string tag)
        {
            return newMessageNoticesList.FindAll(current => { return current.mTag.Equals(tag); });
        }

        public bool IsNewMessageNoticeExsits(string tag)
        {
            return newMessageNoticesList.Exists(current => { return current.mTag.Equals(tag); });
        }

        protected List<NewMessageNoticeData> newMessageNoticesList = new List<NewMessageNoticeData>();

    }
} 