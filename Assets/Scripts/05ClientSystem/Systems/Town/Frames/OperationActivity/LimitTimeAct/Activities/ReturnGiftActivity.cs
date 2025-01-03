using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class ReturnGiftActivity : LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            base.Init(activityId);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ReturnGiftActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ReturnGiftActivityItem";
        }

        public override bool IsHaveRedPoint()
        {
            if (mDataModel.TaskDatas == null || mDataModel.State != OpActivityState.OAS_IN)
                return false;
            

            for (int i = 0; i < mDataModel.TaskDatas.Count; ++i)
            {
                uint currentTime = TimeManager.GetInstance().GetServerTime();
                uint startTime = 0;
                uint endTime = 0;
                if (mDataModel.TaskDatas[i].DataId == 11013006)
                {
                    startTime = mDataModel.TaskDatas[i].ParamNums[3];
                    endTime = mDataModel.TaskDatas[i].ParamNums[4];

                    if (currentTime < startTime || currentTime >= endTime)
                    {
                        continue;
                    }
                }
                else if (mDataModel.TaskDatas[i].DataId == 11017003)
                {
                    startTime = mDataModel.TaskDatas[i].ParamNums[2];
                    endTime = mDataModel.TaskDatas[i].ParamNums[3];

                    if (currentTime < startTime || currentTime >= endTime)
                    {
                        continue;
                    }
                }
                if (mDataModel.TaskDatas[i].State == OpActTaskState.OATS_FINISHED)
                {

                    return true;
                }
            }

            return false;
        }
        protected override void _OnItemClick(int taskId, int param, ulong param2)
        {
            for(int i = 0;i< mDataModel.TaskDatas.Count;i++)
            {
                if(taskId != mDataModel.TaskDatas[i].DataId)
                {
                    continue;
                }
                if(mDataModel.TaskDatas[i].ParamNums2.Count > 1 && mDataModel.TaskDatas[i].ParamNums2[1] > 0)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(string.Format("是否确定花费{0}点券赎回该奖励", mDataModel.TaskDatas[i].ParamNums2[1]), () =>
                    {
                        base._OnItemClick(taskId, param, param2);
                    }, () =>
                    {
                        return;
                    });
                }
                else
                {
                    base._OnItemClick(taskId, param, param2);
                }
            }
        }
    }
}