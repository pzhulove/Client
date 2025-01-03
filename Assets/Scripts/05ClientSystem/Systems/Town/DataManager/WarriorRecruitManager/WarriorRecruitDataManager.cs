using Network;
using Protocol;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 招募身份
    /// </summary>
    [Flags]
    public enum RecruitIdentify
    {
        //无身份
        RI_INVAILD = 1 << 0,//1
        //新人
        RI_NEWBIE = 1 << 1,//2
        //老人
        RI_OLDMAN = 1 << 2,//4
        //回归
        RI_RETURNMAN = 1 << 3,//8
    }

    /// <summary>
    /// 招募玩家信息
    /// </summary>
    public class RecruitPlayerInfo
    {
        /// <summary>
		/// 角色id
		/// </summary>
		public UInt64 userId;
        /// <summary>
        /// 姓名
        /// </summary>
        public string name;
        /// <summary>
        /// 职业
        /// </summary>
        public byte occu;
        /// <summary>
        /// 在线状态
        /// </summary>
        public byte online;

        /// <summary>
        /// 等级
        /// </summary>
        public int level;
    }

    /// <summary>
    /// 勇士招募任务数据
    /// </summary>
    public class WarriorRecruitTaskDataModel
    {
        public int taskId;
        /// <summary>
        /// 任务类型
        /// </summary>
        public int taskType;
        /// <summary>
        /// 总数
        /// </summary>
        public int fullcnt;
        /// <summary>
        /// 当前计数
        /// </summary>
        public int cnt;
        /// <summary>
        /// 奖励
        /// </summary>
        public List<ItemSimpleData> rewards;
        /// <summary>
        /// 任务描述
        /// </summary>
        public string taskDesc;
        /// <summary>
        /// 活动状态
        /// </summary>
        public int state;
        /// <summary>
        /// 身份
        /// </summary>
        public int identify;
        /// <summary>
        /// 跳转链接id
        /// </summary>
        public int linkId;
    }

    /// <summary>
    /// 勇士招募活动管理器
    /// </summary>
    public class WarriorRecruitDataManager : DataManager<WarriorRecruitDataManager>
    {
        /// <summary>
        /// 身份
        /// </summary>
        public static byte identify = 0;
        /// <summary>
        /// 邀请码
        /// </summary>
        public static string inviteCode = string.Empty;

        /// <summary>
        /// 是否绑定邀请码
        /// </summary>
        public static bool isBindInviteCode = false;

        /// <summary>
        /// 在别的服是否绑定
        /// </summary>
        public static bool isHireAlreadyBind = false;

        /// <summary>
        /// 是否有别人绑定我
        /// </summary>
        public static bool isOtherBindMe = false;

        /// <summary>
        /// 所有任务
        /// </summary>
        List<WarriorRecruitTaskDataModel> mAllTaskDataModelList = new List<WarriorRecruitTaskDataModel>();

        /// <summary>
        /// 招募同伴任务
        /// </summary>
        public List<WarriorRecruitTaskDataModel> mRecruitCompanionsTaskList = new List<WarriorRecruitTaskDataModel>();

        public int warriorRecruitActiveID = 8800;

        /// <summary>
        /// 老玩家奖励预览
        /// </summary>
        public List<int> mRecruitmentBonusPreview_OldPlayerList = new List<int>();

        /// <summary>
        /// 新玩家奖励预览
        /// </summary>
        public List<int> mRecruitmentBonusPreview_NewPlayerList = new List<int>();

        public sealed override void Clear()
        {
            UnRegisterNetHandler();
           
            inviteCode = string.Empty;
            identify = 0;
            isBindInviteCode = false;
            isOtherBindMe = false;
            isHireAlreadyBind = false;

            if (mAllTaskDataModelList != null)
                mAllTaskDataModelList.Clear();

            if (mRecruitCompanionsTaskList != null)
                mRecruitCompanionsTaskList.Clear();

            if (mRecruitmentBonusPreview_OldPlayerList != null)
                mRecruitmentBonusPreview_OldPlayerList.Clear();

            if (mRecruitmentBonusPreview_NewPlayerList != null)
                mRecruitmentBonusPreview_NewPlayerList.Clear();
        }

        public sealed override void Initialize()
        {
            InitHireTaskTable();
            RegisterNetHandler();
            InitRecruitmentBonusPreviewData("RecruitmentBonusPreview_OldPlayer", ref mRecruitmentBonusPreview_OldPlayerList);
            InitRecruitmentBonusPreviewData("RecruitmentBonusPreview_NewPlayer", ref mRecruitmentBonusPreview_NewPlayerList);
        }

        private void RegisterNetHandler()
        {
            // 勇士招募活动协议
            NetProcess.AddMsgHandler(WorldQueryHireInfoRes.MsgID, OnQueryHireInfoRes);
            NetProcess.AddMsgHandler(WorldUseHireCodeRes.MsgID, OnUseHireCodeRes);
            NetProcess.AddMsgHandler(WorldQueryTaskStatusRes.MsgID, OnQueryTaskStatusRes);
            NetProcess.AddMsgHandler(WorldQueryHireTaskAccidListRes.MsgID, OnQueryHireTaskAccidListRes);
            NetProcess.AddMsgHandler(WorldQueryHireListRes.MsgID, OnQueryHireListRes);
            NetProcess.AddMsgHandler(WorldSubmitHireTaskRes.MsgID, OnSubmitHireTaskRes);
            NetProcess.AddMsgHandler(WorldQueryHireAlreadyBindRes.MsgID, OnQueryHireAlreadyBindRes);
        }

        private void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(WorldQueryHireInfoRes.MsgID, OnQueryHireInfoRes);
            NetProcess.RemoveMsgHandler(WorldUseHireCodeRes.MsgID, OnUseHireCodeRes);
            NetProcess.RemoveMsgHandler(WorldQueryTaskStatusRes.MsgID, OnQueryTaskStatusRes);
            NetProcess.RemoveMsgHandler(WorldQueryHireTaskAccidListRes.MsgID, OnQueryHireTaskAccidListRes);
            NetProcess.RemoveMsgHandler(WorldQueryHireListRes.MsgID, OnQueryHireListRes);
            NetProcess.RemoveMsgHandler(WorldSubmitHireTaskRes.MsgID, OnSubmitHireTaskRes);
            NetProcess.RemoveMsgHandler(WorldQueryHireAlreadyBindRes.MsgID, OnQueryHireAlreadyBindRes);
        }

        private void OnQueryHireInfoRes(MsgDATA msgData)
        {
            WorldQueryHireInfoRes res = new WorldQueryHireInfoRes();
            res.decode(msgData.bytes);

            identify = res.identity;
            inviteCode = res.code;
            isBindInviteCode = res.isBind == 1;
            isOtherBindMe = res.isOtherBindMe == 1;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WarriorRecruitQueryIdentitySuccessed);
        }

        private void OnUseHireCodeRes(MsgDATA msgData)
        {
            WorldUseHireCodeRes res = new WorldUseHireCodeRes();
            res.decode(msgData.bytes);

            if (res.errorCode != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.errorCode);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation("绑定成功");

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WarriorRecruitBindInviteCodeSuccessed);
            }
        }

        private void OnQueryTaskStatusRes(MsgDATA msgData)
        {
            WorldQueryTaskStatusRes res = new WorldQueryTaskStatusRes();
            res.decode(msgData.bytes);

            for (int i = 0; i < res.hireTaskInfoList.Length; i++)
            {
                var taskInfo = res.hireTaskInfoList[i];
                if (taskInfo == null)
                {
                    continue;
                }

                for (int j = 0; j < mAllTaskDataModelList.Count; j++)
                {
                    var taskDataModel = mAllTaskDataModelList[j];

                    if (taskDataModel == null)
                    {
                        continue;
                    }

                    if (taskDataModel.taskId != taskInfo.taskID)
                    {
                        continue;
                    }

                    taskDataModel.state = taskInfo.status;
                    taskDataModel.cnt = (int)taskInfo.cnt;
                    break;
                }
            }

            mRecruitCompanionsTaskList = FilterRecruiIdentifyTask((int)RecruitIdentify.RI_OLDMAN);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WarriorRecruitQueryTaskSuccessed);
        }

        private void OnQueryHireTaskAccidListRes(MsgDATA msgData)
        {
            WorldQueryHireTaskAccidListRes res = new WorldQueryHireTaskAccidListRes();
            res.decode(msgData.bytes);
            
            List<string> nameList = new List<string>();
            for (int i = 0; i < res.nameList.Length; i++)
            {
                nameList.Add(res.nameList[i]);
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<CompleteQuestPlayerLlistFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CompleteQuestPlayerLlistFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<CompleteQuestPlayerLlistFrame>(FrameLayer.Middle, nameList);
        }

        private void OnQueryHireListRes(MsgDATA msgData)
        {
            WorldQueryHireListRes res = new WorldQueryHireListRes();
            res.decode(msgData.bytes);

            List<RecruitPlayerInfo> playerInfoList = new List<RecruitPlayerInfo>();

            for (int i = 0; i < res.hireList.Length; i++)
            {
                var hire = res.hireList[i];
                if (hire == null)
                {
                    continue;
                }

                RecruitPlayerInfo info = new RecruitPlayerInfo();
                info.name = hire.name;
                info.occu = hire.occu;
                info.online = hire.online;
                info.userId = hire.userId;
                info.level = (int)hire.lv;

                playerInfoList.Add(info);
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<RecruitListFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<RecruitListFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<RecruitListFrame>(FrameLayer.Middle, playerInfoList);
        }

        private void OnSubmitHireTaskRes(MsgDATA msgData)
        {
            WorldSubmitHireTaskRes res = new WorldSubmitHireTaskRes();
            res.decode(msgData.bytes);
            
            if (res.errorCode != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.errorCode);
            }
            else
            {
                for (int j = 0; j < mAllTaskDataModelList.Count; j++)
                {
                    var taskDataModel = mAllTaskDataModelList[j];

                    if (taskDataModel == null)
                    {
                        continue;
                    }

                    if (taskDataModel.taskId != res.taskId)
                    {
                        continue;
                    }

                    taskDataModel.state = (int)OpActTaskState.OATS_OVER;
                    break;
                }

                for (int i = 0; i < mRecruitCompanionsTaskList.Count; i++)
                {
                    var taskDataModel = mRecruitCompanionsTaskList[i];

                    if (taskDataModel == null)
                    {
                        continue;
                    }

                    if (taskDataModel.taskId != res.taskId)
                    {
                        continue;
                    }

                    taskDataModel.state = (int)OpActTaskState.OATS_OVER;
                    break;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WarriorRecruitReceiveRewardSuccessed);
            }
        }

        private void OnQueryHireAlreadyBindRes(MsgDATA msgData)
        {
            WorldQueryHireAlreadyBindRes res = new WorldQueryHireAlreadyBindRes();
            res.decode(msgData.bytes);

            isHireAlreadyBind = res.errorCode <= 0 ? false : true;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WarriorRecruitQueryHireAlreadyBindSuccessed);
        }

        /// <summary>
        /// 查询招募信息
        /// </summary>
        public void SendHireInfoReq()
        {
            WorldQueryHireInfoReq req = new WorldQueryHireInfoReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 使用邀请码
        /// </summary>
        /// <param name="code"></param>
        public void SendUseHireCodeReq(string code)
        {
            WorldUseHireCodeReq req = new WorldUseHireCodeReq();
            req.code = code;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 招募任务完成情况
        /// </summary>
        public void SendQueryTaskStatusReq()
        {
            WorldQueryTaskStatusReq req = new WorldQueryTaskStatusReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 指定id的任务完成名单
        /// </summary>
        /// <param name="taskId"></param>
        public void SendQueryHireTaskAccidListReq(int taskId)
        {
            WorldQueryHireTaskAccidListReq req = new WorldQueryHireTaskAccidListReq();
            req.taskId = (UInt32)taskId;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 查询招募列表
        /// </summary>
        public void SendQueryHireListReq()
        {
            WorldQueryHireListReq req = new WorldQueryHireListReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 提交招募任务
        /// </summary>
        /// <param name="taskId"></param>
        public void SendSubmitHireTaskReq(int taskId)
        {
            WorldSubmitHireTaskReq req = new WorldSubmitHireTaskReq();
            req.taskId = (UInt32)taskId;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 查询招募硬币
        /// </summary>
        public void SendWorldQueryHireCoinReq()
        {
            WorldQueryHireCoinReq req = new WorldQueryHireCoinReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        ///  查询或设置招募是否已推送
        /// </summary>
        /// <param name="type">0：查询1：设置</param>
        public void SendWorldQueryHirePushReq(byte type)
        {
            WorldQueryHirePushReq req = new WorldQueryHirePushReq();
            req.type = type;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 招募红点
        /// </summary>
        public void SendWorldQueryHireRedPointReq()
        {
            SceneQueryHireRedPointReq req = new SceneQueryHireRedPointReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 查询有没有在别的服绑定
        /// </summary>
        public void SendWorldQueryHireAlreadyBindReq()
        {
            WorldQueryHireAlreadyBindReq req = new WorldQueryHireAlreadyBindReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 根据身份过滤任务
        /// </summary>
        public List<WarriorRecruitTaskDataModel> FilterRecruiIdentifyTask(int identify)
        {
            List<WarriorRecruitTaskDataModel> taskList = new List<WarriorRecruitTaskDataModel>();

            for (int i = 0; i < mAllTaskDataModelList.Count; i++)
            {
                var taskData = mAllTaskDataModelList[i];
                if (taskData == null)
                {
                    continue;
                }

                if (taskData.identify != identify)
                {
                    continue;
                }

                taskList.Add(taskData);
            }

            return taskList;
        }

        private void InitHireTaskTable()
        {
            var enumer = TableManager.GetInstance().GetTable<HireTask>().GetEnumerator();
            while (enumer.MoveNext())
            {
                var table = enumer.Current.Value as HireTask;
                if (table == null)
                {
                    continue;
                }

                WarriorRecruitTaskDataModel model = new WarriorRecruitTaskDataModel();
                model.fullcnt = table.FullCnt;
                model.identify = table.Identify;
                model.taskId = table.ID;
                model.taskType = table.Type;
                model.taskDesc = table.Describe;
                model.rewards = GetRewards(table.Rewards);
                model.linkId = table.Link;

                mAllTaskDataModelList.Add(model);
            }
        }

        private List<ItemSimpleData> GetRewards(string path)
        {
            List<ItemSimpleData> rewards = new List<ItemSimpleData>();

            string[] strs = path.Split(',');

            for (int i = 0; i < strs.Length; i++)
            {
                var str = strs[i];
                if (str == string.Empty)
                {
                    continue;
                }

                string[] items = str.Split('_');
                if (items.Length >= 2)
                {
                    int itemId = 0;
                    int itemNum = 0;

                    int.TryParse(items[0], out itemId);
                    int.TryParse(items[1], out itemNum);
                    
                    rewards.Add(new ItemSimpleData(itemId, itemNum));
                }
            }

            return rewards;
        }

        /// <summary>
        /// 检查勇士招募活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckWarriorRecruitActiveIsOpen()
        {
            if (ActiveManager.GetInstance().allActivities.ContainsKey(warriorRecruitActiveID))
            {
                var info = ActiveManager.GetInstance().allActivities[warriorRecruitActiveID];
                if (info.state == (byte)StateType.Running)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 初始化奖励预览数据
        /// </summary>
        private void InitRecruitmentBonusPreviewData(string key,ref List<int> idList)
        {
            if (idList == null)
            {
                idList = new List<int>();
            }

            idList.Clear();

            string sRecruitmentBonusPreview = TR.Value(key);
            string[] sIds = sRecruitmentBonusPreview.Split('|');
            for (int i = 0; i < sIds.Length; i++)
            {
                var str = sIds[i];
                if (str == string.Empty)
                {
                    continue;
                }

                int id = 0;
                int.TryParse(str, out id);

                idList.Add(id);
            }
        }

        /// <summary>
        /// 红点
        /// </summary>
        /// <returns></returns>
        public bool IsRedPointShow()
        {
            int count = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_HIRE_RED_POINT);

            if (count <= 0)
            {
                return true;
            }

            for (int i = 0; i < mAllTaskDataModelList.Count; i++)
            {
                var data = mAllTaskDataModelList[i];
                if (data == null)
                {
                    continue;
                }

                if (data.state == (int)OpActTaskState.OATS_FINISHED)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 接受招募页签是否显示
        /// </summary>
        /// <returns></returns>
        public bool IsAcceptRecruitTabShow()
        {
            bool isShow = ((WarriorRecruitDataManager.identify & (int)RecruitIdentify.RI_NEWBIE) != 0 && isBindInviteCode) ||
                          (!ClientApplication.playerinfo.CheckAllRoleLevelIsContainsParamLevel(20) && isHireAlreadyBind == false) ||
                          ((WarriorRecruitDataManager.identify & (int)RecruitIdentify.RI_RETURNMAN) != 0) ||
                          ((WarriorRecruitDataManager.identify & (int)RecruitIdentify.RI_RETURNMAN) != 0 && !((WarriorRecruitDataManager.identify & (int)RecruitIdentify.RI_NEWBIE) != 0));

            return isShow;
        }
    }
}
