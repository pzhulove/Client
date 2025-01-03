using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using ProtoTable;

namespace GameClient
{

    /// <summary>
    /// 邮件类型
    /// </summary>
    public enum MailTabType
    {
       MTT_NONE = - 1,
       MTT_ANNOUNCEMENT, //公告
       MTT_REWARD,       //奖励
       MTT_GUILD,        //公会
    }

    /// <summary>
    /// 邮件数据
    /// </summary>
    public class MailDataModel
    {
        public MailTitleInfo info;
        public string content;

        // 物品
        public List<ItemReward> items = new List<ItemReward>();
        public List<Item> detailItems = new List<Item>();
    }


    public class MailDataManager : DataManager<MailDataManager>
    {
        /// <summary>
        /// 所有邮件
        /// </summary>
        private List<MailTitleInfo> mAllMailTilteInfo = new List<MailTitleInfo>();
        /// <summary>
        /// 公告邮件标题信息
        /// </summary>
        public List<MailTitleInfo> mAnnouncementMailTitleInfoList = new List<MailTitleInfo>();

        /// <summary>
        /// 奖励邮件标题信息
        /// </summary>
        public List<MailTitleInfo> mRewardMailTitleInfoList = new List<MailTitleInfo>();

        /// <summary>
        /// 公会邮件标题信息
        /// </summary>
        public List<MailTitleInfo> mGuildMailTitleInfoList = new List<MailTitleInfo>();

        /// <summary>
        /// 公告邮件
        /// </summary>
        public Dictionary<UInt64, MailDataModel> mAnnouncementMailDataModelDict = new Dictionary<UInt64, MailDataModel>();

        /// <summary>
        /// 奖励邮件
        /// </summary>
        public Dictionary<UInt64, MailDataModel> mRewardMailDataModelDict = new Dictionary<UInt64, MailDataModel>();

        /// <summary>
        /// 公会邮件
        /// </summary>
        public Dictionary<UInt64, MailDataModel> mGuildMailDataModelDict = new Dictionary<UInt64, MailDataModel>();
        

        private static int announcementMailOneKeyDeleteNum;
        /// <summary>
        /// 公告可删除邮件数
        /// </summary>
        public static int AnnouncementMailOneKeyDeleteNum
        {
            get { return announcementMailOneKeyDeleteNum; }
            set { announcementMailOneKeyDeleteNum = value; }
        }

        private static int rewardMailOneKeyDeleteNum;
        /// <summary>
        /// 奖励可删除邮件数
        /// </summary>
        public static int RewardMailOneKeyDeleteNum
        {
            get { return rewardMailOneKeyDeleteNum; }
            set { rewardMailOneKeyDeleteNum = value; }
        }

        private static int announcementMailOneKeyReceiveNum;
        /// <summary>
        /// 公告邮件邮件附件数
        /// </summary>
        public static int AnnouncmentMailOneKeyReceiveNum
        {
            get { return announcementMailOneKeyReceiveNum; }
            set { announcementMailOneKeyReceiveNum = value; }
        }

        private static int rewardMailOneKeyReceiveNum;
        /// <summary>
        /// 奖励附件邮件数
        /// </summary>
        public static int RewardMailOneKeyReceiveNum
        {
            get { return rewardMailOneKeyReceiveNum; }
            set { rewardMailOneKeyReceiveNum = value; }
        }

        private static int guildMailOneKeyReceiveNum;
        /// <summary>
        /// 公会邮件附件数
        /// </summary>
        public static int GuildMailOneKeyReceiveNum
        {
            get { return guildMailOneKeyReceiveNum; }
            set { guildMailOneKeyReceiveNum = value; }
        }

        private static int guildMailOneKeyDeleteNum;
        /// <summary>
        /// 公会可删除邮件数
        /// </summary>
        public static int GuildMailOneKeyDeleteNum
        {
            get { return guildMailOneKeyDeleteNum; }
            set { guildMailOneKeyDeleteNum = value; }
        }

        private static MailTabType mCurrentSelectMailTabType;

        /// <summary>
        /// 当前操作的页签
        /// </summary>
        public static MailTabType CurrentSelectMailTabType
        {
            get { return mCurrentSelectMailTabType; }
            set { mCurrentSelectMailTabType = value; }
        }

        private static int mMailItemNum;

        /// <summary>
        /// 邮件附带物品数量上限
        /// </summary>
        public static int MailItemNum
        {
            get { return mMailItemNum; }
            set { mMailItemNum = value; }
        }

      
        private static int mUnReadMailNum;
        /// <summary>
        /// 未读邮件数
        /// </summary>
        public static int UnReadMailNum
        {
            get { return mUnReadMailNum; }
            set { mUnReadMailNum = value; }
        }

        private static int mOneKeyReceiveNum;
        /// <summary>
        /// 邮件有附件数
        /// </summary>
        public static int OneKeyReceiveNum
        {
            get { return mOneKeyReceiveNum; }
            set { mOneKeyReceiveNum = value; }
        }

        private static int mOneKeyDeleteNum;
        /// <summary>
        /// 邮件无附件数
        /// </summary>
        public static int OneKeyDeleteNum
        {
            get { return mOneKeyDeleteNum; }
            set { mOneKeyDeleteNum = value; }
        }

        private static int mAnnouncementUnReadMailNum;
        /// <summary>
        /// 公告 未读邮件数
        /// </summary>
        public static int AnnouncementUnReadMailNum
        {
            get { return mAnnouncementUnReadMailNum; }
            set { mAnnouncementUnReadMailNum = value; }
        }

        private static int mRewardUnReadMailNum;
        /// <summary>
        /// 奖励 未读邮件数
        /// </summary>
        public static int RewardUnReadMailNum
        {
            get { return mRewardUnReadMailNum; }
            set { mRewardUnReadMailNum = value; }
        }

        private static int mGuildUnReadMailNum;
        /// <summary>
        /// 公会 未读邮件数
        /// </summary>
        public static int GuildUnReadMailNum
        {
            get { return mGuildUnReadMailNum; }
            set { mGuildUnReadMailNum = value; }
        }

        private static int mDefaultMailMainTabIndex = 0;
        /// <summary>
        /// 打开邮件默认选中的页签
        /// </summary>
        public static int DefaultMailMainTabIndex
        {
            get { return mDefaultMailMainTabIndex; }
            set { mDefaultMailMainTabIndex = value; }
        }

        /// <summary>
        /// 查找邮件标题信息
        /// </summary>
        /// <param name="mailInfoList"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public MailTitleInfo FindMailTitleInfo(List<MailTitleInfo> mailInfoList,UInt64 id)
        {
            //return mailInfoList.Find(x => { return x.id == id; });
            MailTitleInfo info = null;

            for (int i = 0; i < mailInfoList.Count; i++)
            {
                UInt64 mailId = mailInfoList[i].id;

                if (mailId != id)
                {
                    continue;
                }

                return mailInfoList[i];
            }

#if MG_TEST_EXTENT
            if (info == null)
            {
                System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

                //打日志查MailTitleInfo 为什么为Null  把传进来的集合所有ID 和服务器返回ID全部打印出来 检查是否会有服务器返回过来的ID 在集合里面找不到 导致MailTitleInfo 为Null
                for (int i = 0; i < mailInfoList.Count; i++)
                {
                    strBuilder.Append(mailInfoList[i].id.ToString());
                    strBuilder.Append(",");
                }

                Logger.LogErrorFormat("集合mailInfoList所有邮件id为{0}[WorldReadMailRet] 阅读邮件协议 返回的邮件id = {1}", strBuilder.ToString(), id);
            }
#endif
            return info;
        }

        /// <summary>
        /// 查找邮件数据
        /// </summary>
        /// <param name="mailDataModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public MailDataModel FindMailDataModel(Dictionary<UInt64,MailDataModel> mailDataModel , UInt64 id )
        {
            MailDataModel model;

            if (mailDataModel.TryGetValue(id,out model))
            {
                return model;
            }

            return null;
        }

        /// <summary>
        /// 得到当前要显示的邮件列表
        /// </summary>
        /// <returns></returns>
        public List<MailTitleInfo> GetCurrentShowMailTitleInfoList()
        {
            List<MailTitleInfo> mCurrentMailTitleInfoList = new List<MailTitleInfo>();
            
            switch (CurrentSelectMailTabType)
            {
                case MailTabType.MTT_NONE:
                    break;
                case MailTabType.MTT_ANNOUNCEMENT:
                    mCurrentMailTitleInfoList = MailDataManager.GetInstance().mAnnouncementMailTitleInfoList;
                    break;
                case MailTabType.MTT_REWARD:
                    mCurrentMailTitleInfoList = MailDataManager.GetInstance().mRewardMailTitleInfoList;
                    break;
                case MailTabType.MTT_GUILD:
                    mCurrentMailTitleInfoList = MailDataManager.GetInstance().mGuildMailTitleInfoList;
                    break;
                default:
                    break;
            }

            return mCurrentMailTitleInfoList;
        }

        public sealed override EEnterGameOrder GetOrder()
        {
            return base.GetOrder();
        }

        public sealed override void Clear()
        {
            mAllMailTilteInfo.Clear();
            mAnnouncementMailTitleInfoList.Clear();
            mRewardMailTitleInfoList.Clear();
            mGuildMailTitleInfoList.Clear();
            mAnnouncementMailDataModelDict.Clear();
            mRewardMailDataModelDict.Clear();
            mGuildMailDataModelDict.Clear();

            announcementMailOneKeyDeleteNum = 0;
            rewardMailOneKeyDeleteNum = 0;
            announcementMailOneKeyReceiveNum = 0;
            rewardMailOneKeyReceiveNum = 0;
            guildMailOneKeyReceiveNum = 0;
            guildMailOneKeyDeleteNum = 0;
            mMailItemNum = 0;
            mUnReadMailNum = 0;
            mOneKeyReceiveNum = 0;
            mOneKeyDeleteNum = 0;
            mAnnouncementUnReadMailNum = 0;
            mRewardUnReadMailNum = 0;
            mGuildUnReadMailNum = 0;

            UnRegisterNetHandler();
        }

        public sealed override void Initialize()
        {
            InitLocalData();
            RegisterNetHandler();
        }

        private void InitLocalData()
        {
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_MAIL_ITEM_MAXNUM);
            mMailItemNum = SystemValueTableData.Value;
        }

        /// <summary>
        /// 检查页签红点是否显示
        /// </summary>
        /// <param name="mailTabType"></param>
        /// <returns></returns>
        public bool CheckRedPoint(MailTabType mailTabType)
        {
            int unReadMailNum = 0;

            switch (mailTabType)
            {
                case MailTabType.MTT_NONE:
                    break;
                case MailTabType.MTT_ANNOUNCEMENT:
                    unReadMailNum = AnnouncementUnReadMailNum;
                    break;
                case MailTabType.MTT_REWARD:
                    unReadMailNum = RewardUnReadMailNum;
                    break;
                case MailTabType.MTT_GUILD:
                    unReadMailNum = GuildUnReadMailNum;
                    break;
            }

            if (unReadMailNum > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 更新邮件未读数量和附加奖励邮件数
        /// </summary>
        public void UpdateMailUnReadNumAndOneKeyReceiveNum()
        {
            mUnReadMailNum = 0;
            mOneKeyReceiveNum = 0;
            mOneKeyDeleteNum = 0;
            mAnnouncementUnReadMailNum = 0;
            mRewardUnReadMailNum = 0;
            mGuildUnReadMailNum = 0;
            for (int i = 0; i < mAllMailTilteInfo.Count; i++)
            {
                if (mAllMailTilteInfo[i].status == 0)
                {
                    mUnReadMailNum++;
                }

                if (mAllMailTilteInfo[i].hasItem == 1)
                {
                    mOneKeyReceiveNum++;
                }
                else
                {
                    mOneKeyDeleteNum++;
                }
            }

            for (int i = 0; i < mAnnouncementMailTitleInfoList.Count; i++)
            {
                if (mAnnouncementMailTitleInfoList[i].status == 0)
                {
                    mAnnouncementUnReadMailNum++;
                }
            }

            for (int i = 0; i < mRewardMailTitleInfoList.Count; i++)
            {
                if (mRewardMailTitleInfoList[i].status == 0)
                {
                    mRewardUnReadMailNum++;
                }
            }

            for (int i = 0; i < mGuildMailTitleInfoList.Count; i++)
            {
                if (mGuildMailTitleInfoList[i].status == 0)
                {
                    mGuildUnReadMailNum++;
                }
            }
        }

        /// <summary>
        /// 更新打开邮件默认选中的页签
        /// </summary>
        public void UpdateOpenMailTabType()
        {
            if (mRewardMailTitleInfoList.Count > 0)
            {
                mDefaultMailMainTabIndex = 1;
            }
            else if (mGuildMailTitleInfoList.Count > 0 && mRewardMailTitleInfoList.Count <= 0)
            {
                mDefaultMailMainTabIndex = 2;
            }
            else
            {
                mDefaultMailMainTabIndex = 0;
            }
        }

        private void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(WorldMailListRet.MsgID, OnWorldMailListRet);
            NetProcess.AddMsgHandler(WorldReadMailRet.MsgID, OnWorldReadMailRet);
            NetProcess.AddMsgHandler(WorldSyncMailStatus.MsgID, OnWorldSyncMailStatusRes);
            NetProcess.AddMsgHandler(WorldNotifyDeleteMail.MsgID, OnWorldNotifyDeleteMailRes);
            NetProcess.AddMsgHandler(WorldNotifyNewMail.MsgID, OnWorldNotifyNewMailRes);
        }

        private void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(WorldMailListRet.MsgID, OnWorldMailListRet);
            NetProcess.RemoveMsgHandler(WorldReadMailRet.MsgID, OnWorldReadMailRet);
            NetProcess.RemoveMsgHandler(WorldSyncMailStatus.MsgID, OnWorldSyncMailStatusRes);
            NetProcess.RemoveMsgHandler(WorldNotifyDeleteMail.MsgID, OnWorldNotifyDeleteMailRes);
            NetProcess.RemoveMsgHandler(WorldNotifyNewMail.MsgID, OnWorldNotifyNewMailRes);
        }

        /// <summary>
        /// 请求邮件列表返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnWorldMailListRet(MsgDATA msg)
        {
            WorldMailListRet res = new WorldMailListRet();
            res.decode(msg.bytes);

            mAnnouncementMailTitleInfoList.Clear();
            mRewardMailTitleInfoList.Clear();
            mGuildMailTitleInfoList.Clear();
            mAllMailTilteInfo.Clear();

            for (int i = 0; i < res.mails.Length; i++)
            {
                if (res.mails[i].type == (int)MailType.MAIL_TYPE_GM)
                {
                    mAnnouncementMailTitleInfoList.Add(res.mails[i]);
                }
                else if(res.mails[i].type == (int)MailType.MAIL_TYPE_GUILD)
                {
                    mGuildMailTitleInfoList.Add(res.mails[i]);
                }
                else
                {
                    mRewardMailTitleInfoList.Add(res.mails[i]);
                }

                mAllMailTilteInfo.Add(res.mails[i]);
            }

            SortAnnouncementMailList();
            UpdateAnnouncementOneKeyNum();

            SortRewardMailList();
            UpdateRewardOneKeyNum();

            SortGuildMailList();
            UpdateGuildMailOneKeyNum();

            UpdateMailUnReadNumAndOneKeyReceiveNum();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewMailNotify);

        }

        /// <summary>
        /// 阅读邮件请求返回详细信息
        /// </summary>
        /// <param name="msg"></param>
        private void OnWorldReadMailRet(MsgDATA msg)
        {
            WorldReadMailRet res = new WorldReadMailRet();
            res.decode(msg.bytes);

            MailDataModel mMailDataModel = new MailDataModel();
            mMailDataModel.content = res.content;
            mMailDataModel.items = new List<ItemReward>(res.items);
            mMailDataModel.detailItems = res.detailItems;

            switch (CurrentSelectMailTabType)
            {
                case MailTabType.MTT_NONE:
                    break;
                case MailTabType.MTT_ANNOUNCEMENT:

                    mMailDataModel.info = FindMailTitleInfo(mAnnouncementMailTitleInfoList, res.id);

                    MailDataModel mailData = FindMailDataModel(mAnnouncementMailDataModelDict, res.id);
                    if (mailData == null)
                    {
                        mAnnouncementMailDataModelDict.Add(res.id, mMailDataModel);
                    }
                    break;
                case MailTabType.MTT_REWARD:

                    mMailDataModel.info = FindMailTitleInfo(mRewardMailTitleInfoList, res.id);
                    MailDataModel mailData2 = FindMailDataModel(mRewardMailDataModelDict, res.id);
                    if (mailData2 == null)
                    {
                        mRewardMailDataModelDict.Add(res.id, mMailDataModel);
                    }
                    break;
                case MailTabType.MTT_GUILD:
                    mMailDataModel.info = FindMailTitleInfo(mGuildMailTitleInfoList, res.id);
                    MailDataModel mailData3 = FindMailDataModel(mGuildMailDataModelDict, res.id);
                    if (mailData3 == null)
                    {
                        mGuildMailDataModelDict.Add(res.id, mMailDataModel);
                    }
                    break;
            }


            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReadMailResSuccess, mMailDataModel);
        }

        /// <summary>
        /// 同步邮件状态
        /// </summary>
        /// <param name="msg"></param>
        private void OnWorldSyncMailStatusRes(MsgDATA msg)
        {
            WorldSyncMailStatus res = new WorldSyncMailStatus();
            res.decode(msg.bytes);

            switch (CurrentSelectMailTabType)
            {
                case MailTabType.MTT_NONE:
                    break;
                case MailTabType.MTT_ANNOUNCEMENT:
                    UpdateMailStatue(mAnnouncementMailTitleInfoList, mAnnouncementMailDataModelDict, res);
                    break;
                case MailTabType.MTT_REWARD:
                    UpdateMailStatue(mRewardMailTitleInfoList, mRewardMailDataModelDict, res);
                    break;
                case MailTabType.MTT_GUILD:
                    UpdateMailStatue(mGuildMailTitleInfoList, mGuildMailDataModelDict, res);
                    break;
            }

            UpdateMailUnReadNumAndOneKeyReceiveNum();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateMailStatus);
        }

        /// <summary>
        /// 删除邮件返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnWorldNotifyDeleteMailRes(MsgDATA msg)
        {
            WorldNotifyDeleteMail res = new WorldNotifyDeleteMail();
            res.decode(msg.bytes);

            switch (CurrentSelectMailTabType)
            {
                case MailTabType.MTT_NONE:
                    break;
                case MailTabType.MTT_ANNOUNCEMENT:

                    for (int i = 0; i < res.ids.Length; i++)
                    {
                        DeleteMailTitleInfo(mAnnouncementMailTitleInfoList, res.ids[i]);
                        DeleteMailDataModle(mAnnouncementMailDataModelDict, res.ids[i]);

                        DeleteMailTitleInfo(mAllMailTilteInfo, res.ids[i]);
                    }

                    SortAnnouncementMailList();
                    UpdateAnnouncementOneKeyNum();
                    break;
                case MailTabType.MTT_REWARD:

                    for (int i = 0; i < res.ids.Length; i++)
                    {
                        DeleteMailTitleInfo(mRewardMailTitleInfoList, res.ids[i]);
                        DeleteMailDataModle(mRewardMailDataModelDict, res.ids[i]);

                        DeleteMailTitleInfo(mAllMailTilteInfo, res.ids[i]);
                    }

                    SortRewardMailList();
                    UpdateRewardOneKeyNum();
                    break;
                case MailTabType.MTT_GUILD:

                    for (int i = 0; i < res.ids.Length; i++)
                    {
                        DeleteMailTitleInfo(mGuildMailTitleInfoList, res.ids[i]);
                        DeleteMailDataModle(mGuildMailDataModelDict, res.ids[i]);

                        DeleteMailTitleInfo(mAllMailTilteInfo, res.ids[i]);
                    }

                    SortGuildMailList();
                    UpdateGuildMailOneKeyNum();
                    break;
                default:
                    break;
            }

            UpdateMailUnReadNumAndOneKeyReceiveNum();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMailDeleteSuccess);
            SystemNotifyManager.SystemNotify(1026);
        }

        /// <summary>
        /// 新邮件
        /// </summary>
        /// <param name="msg"></param>
        private void OnWorldNotifyNewMailRes(MsgDATA msg)
        {
            WorldNotifyNewMail res = new WorldNotifyNewMail();
            res.decode(msg.bytes);

            if (res.info.type == (int)MailType.MAIL_TYPE_GM)
            {
                mAnnouncementMailTitleInfoList.Insert(0, res.info);
                UpdateAnnouncementOneKeyNum();
            }
            else if (res.info.type == (int)MailType.MAIL_TYPE_GUILD)
            {
                mGuildMailTitleInfoList.Insert(0, res.info);
                UpdateGuildMailOneKeyNum();
            }
            else
            {
                mRewardMailTitleInfoList.Insert(0, res.info);
                UpdateRewardOneKeyNum();
            }

            mAllMailTilteInfo.Insert(0, res.info);

            UpdateMailUnReadNumAndOneKeyReceiveNum();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewMailNotify);
        }

        /// <summary>
        /// 请求邮件
        /// </summary>
        public static void OnSendMailListReq()
        {
            if (ClientSystemManager.GetInstance().CurrentSystem.GetType() != typeof(ClientSystemTown))
            {
                return;
            }

            // 向服务器请求邮件列表
            WorldMailListReq req = new WorldMailListReq();
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);

            Logger.Log("[MailSystem]Send WorldMailListReq....");
        }


        // 阅读邮件请求（邮件状态的返回走同步邮件状态消息）
        public void OnSendReadMailReq(UInt64 id)
        {
            WorldReadMailReq req = new WorldReadMailReq();
            req.id = id;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 领取附件请求（领取返回走删除）
        /// </summary>
        /// <param name="bReceivaAll"></param>
        /// <param name="curSelMailID"></param>
        public void OnSendReceiveMailReq(bool bReceivaAll, UInt64 curSelMailID)
        {
            WorldGetMailItems req = new WorldGetMailItems();
            if (bReceivaAll)
            {
                req.type = 1;
                req.id = 0;

                switch (CurrentSelectMailTabType)
                {
                    case MailTabType.MTT_NONE:
                        break;
                    case MailTabType.MTT_ANNOUNCEMENT:
                        req.mailType = (int)MailType.MAIL_TYPE_GM;
                        break;
                    case MailTabType.MTT_REWARD:
                        req.mailType = (int)MailType.MAIL_TYPE_SYSTEM;
                        break;
                    case MailTabType.MTT_GUILD:
                        req.mailType = (int)MailType.MAIL_TYPE_GUILD;
                        break;
                }
            }
            else
            {
                req.type = 0;
                req.id = curSelMailID;
            }

            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 删除邮件请求
        /// </summary>
        /// <param name="bDeleteAll">是否全部删除</param>
        /// <param name="mailTabType">邮件类型</param>
        /// <param name="curSelMailID">当前选择的邮件ID</param
        public void OnSendDeleteMailReq(bool bDeleteAll,UInt64 curSelMailID)
        {
            WorldDeleteMail req = new WorldDeleteMail();
            
            List<UInt64> mDeleteMailList = null;
            
            if (bDeleteAll)
            {
                switch (CurrentSelectMailTabType)
                {
                    case MailTabType.MTT_NONE:
                        break;
                    case MailTabType.MTT_ANNOUNCEMENT:
                        mDeleteMailList = GetDeleteMailList(mAnnouncementMailTitleInfoList);
                        break;
                    case MailTabType.MTT_REWARD:
                        mDeleteMailList = GetDeleteMailList(mRewardMailTitleInfoList);
                        break;
                    case MailTabType.MTT_GUILD:
                        mDeleteMailList = GetDeleteMailList(mGuildMailTitleInfoList);
                        break;
                    default:
                        break;
                }

                req.ids = new UInt64[mDeleteMailList.Count];

                for (int i = 0; i < mDeleteMailList.Count; i++)
                {
                    req.ids[i] = mDeleteMailList[i];
                }
            }
            else
            {
                req.ids = new UInt64[1];
                req.ids[0] = curSelMailID;
            }

            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 得到删除的邮件列表
        /// </summary>
        /// <param name="mailList"></param>
        /// <returns></returns>
        private List<UInt64> GetDeleteMailList(List<MailTitleInfo> mailList)
        {
            List<UInt64> DeleteMailList = new List<UInt64>();

            for (int i = 0; i < mailList.Count; i++)
            {
                if (mailList[i].hasItem == 0)
                {
                    DeleteMailList.Add(mailList[i].id);
                }
            }

            return DeleteMailList;
        }

        /// <summary>
        /// 删除邮件信息
        /// </summary>
        /// <param name="id"></param>
        private void DeleteMailTitleInfo(List<MailTitleInfo> mailTitleInfo,UInt64 id)
        {
            for (int i = 0; i < mailTitleInfo.Count; i++)
            {
                if (mailTitleInfo[i].id == id)
                {
                    mailTitleInfo.RemoveAt(i);
                    break;
                }
            }
        }
        
        /// <summary>
        /// 删除邮件数据信息
        /// </summary>
        /// <param name="mailDataModle"></param>
        /// <param name="id"></param>
        private void DeleteMailDataModle(Dictionary<UInt64,MailDataModel> mailDataModle,UInt64 id)
        {
            if (mailDataModle.ContainsKey(id))
            {
                mailDataModle.Remove(id);
            }
        }
        
        /// <summary>
        /// 更新公告可删除邮件数
        /// </summary>
        private void UpdateAnnouncementOneKeyNum()
        {
            announcementMailOneKeyDeleteNum = 0;
            announcementMailOneKeyReceiveNum = 0;

            for (int i = 0; i < mAnnouncementMailTitleInfoList.Count; i++)
            {
                if (mAnnouncementMailTitleInfoList[i].hasItem == 0)
                {
                    announcementMailOneKeyDeleteNum++;
                }
                else
                {
                    announcementMailOneKeyReceiveNum++;
                }
            }
        }

        /// <summary>
        /// 更新奖励可删除邮件数
        /// </summary>
        private void UpdateRewardOneKeyNum()
        {
            rewardMailOneKeyDeleteNum = 0;
            rewardMailOneKeyReceiveNum = 0;

            for (int i = 0; i < mRewardMailTitleInfoList.Count; i++)
            {
                if (mRewardMailTitleInfoList[i].hasItem == 0)
                {
                    rewardMailOneKeyDeleteNum++;
                }
                else
                {
                    rewardMailOneKeyReceiveNum++;
                }
            }
        }

        /// <summary>
        /// 更新公会可删邮件数
        /// </summary>
        private void UpdateGuildMailOneKeyNum()
        {
            guildMailOneKeyDeleteNum = 0;
            guildMailOneKeyReceiveNum = 0;
            for (int i = 0; i < mGuildMailTitleInfoList.Count; i++)
            {
                if (mGuildMailTitleInfoList[i].hasItem == 0)
                {
                    guildMailOneKeyDeleteNum++;
                }
                else
                {
                    guildMailOneKeyReceiveNum++;
                }
            }
        }

        /// <summary>
        /// 更新邮件状态信息
        /// </summary>
        /// <param name="mailInfoList"></param>
        /// <param name="mailDataDict"></param>
        private void UpdateMailStatue(List<MailTitleInfo> mailInfoList, Dictionary<UInt64, MailDataModel> mailDataDict, WorldSyncMailStatus res)
        {
            for (int i = 0; i < mailInfoList.Count; i++)
            {
                if (mailInfoList[i].id != res.id)
                {
                    continue;
                }

                if (mailInfoList[i].status != res.status)
                {
                    mailInfoList[i].status = res.status;

                    MailDataModel mMailDataModel = FindMailDataModel(mailDataDict, res.id);
                    if (mMailDataModel != null)
                    {
                        mMailDataModel.info.status = res.status;
                    }
                }
            }
        }

        private void SortAnnouncementMailList()
        {
            List<MailTitleInfo> UnReadList = new List<MailTitleInfo>();
            List<MailTitleInfo> ReadList = new List<MailTitleInfo>();

            for (int i = 0; i < mAnnouncementMailTitleInfoList.Count; i++)
            {
                if (mAnnouncementMailTitleInfoList[i].status == 0)
                {
                    UnReadList.Add(mAnnouncementMailTitleInfoList[i]);
                }
                else
                {
                    ReadList.Add(mAnnouncementMailTitleInfoList[i]);
                }
            }

            mAnnouncementMailTitleInfoList = UnReadList;

            for (int i = 0; i < ReadList.Count; i++)
            {
                mAnnouncementMailTitleInfoList.Add(ReadList[i]);
            }
        }

        private void SortRewardMailList()
        {
            List<MailTitleInfo> UnReadList = new List<MailTitleInfo>();
            List<MailTitleInfo> ReadList = new List<MailTitleInfo>();

            for (int i = 0; i < mRewardMailTitleInfoList.Count; i++)
            {
                if (mRewardMailTitleInfoList[i].status == 0)
                {
                    UnReadList.Add(mRewardMailTitleInfoList[i]);
                }
                else
                {
                    ReadList.Add(mRewardMailTitleInfoList[i]);
                }
            }
            
            mRewardMailTitleInfoList = UnReadList;

            for (int i = 0; i < ReadList.Count; i++)
            {
                mRewardMailTitleInfoList.Add(ReadList[i]);
            }
        }

        private void SortGuildMailList()
        {
            List<MailTitleInfo> UnReadList = new List<MailTitleInfo>();
            List<MailTitleInfo> ReadList = new List<MailTitleInfo>();

            for (int i = 0; i < mGuildMailTitleInfoList.Count; i++)
            {
                if (mGuildMailTitleInfoList[i].status == 0)
                {
                    UnReadList.Add(mGuildMailTitleInfoList[i]);
                }
                else
                {
                    ReadList.Add(mGuildMailTitleInfoList[i]);
                }
            }

            mGuildMailTitleInfoList = UnReadList;

            for (int i = 0; i < ReadList.Count; i++)
            {
                mGuildMailTitleInfoList.Add(ReadList[i]);
            }
        }

        public void SortMailList()
        {
            SortAnnouncementMailList();
            SortRewardMailList();
            SortGuildMailList();
        }
    }
}
