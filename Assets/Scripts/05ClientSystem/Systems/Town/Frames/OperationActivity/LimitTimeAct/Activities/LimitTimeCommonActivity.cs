using System;
using System.Collections.Generic;
using ActivityLimitTime;
using GameClient;
using Protocol;
using UnityEngine;

namespace GameClient
{

    public interface IDungeonBuffView
    {
         void Init(ILimitTimeActivityModel model, UnityEngine.Events.UnityAction callBack);
         void Close();
          void Dispose();
    }
    public interface IGiftPackActivityView
    {
        void Init(LimitTimeGiftPackModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick);
        void UpdateData(LimitTimeGiftPackModel model);
        void Close();
    }
    public interface IActivityView : IDisposable
    {
        void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick);
        void UpdateData(ILimitTimeActivityModel data);
        void Close();
        void Show();
        void Hide();
    }

    //基础活动类型
    public class LimitTimeCommonActivity : IActivity
    {
        protected GameObject mGameObject;
        protected bool mIsInit = false;
        protected IActivityView mView;
        //活动数据model
        protected ILimitTimeActivityModel mDataModel;

        public virtual void Init(uint activityId)
        {
            //将服务器数据结构转为客户端数据结构
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            if (data != null)
            {
                mDataModel = new LimitTimeActivityModel(data, _GetItemPrefabPath());
            }
        }

        public virtual void Show(Transform root)
        {
            if (mDataModel == null)
            {
                return;
            }

            //已经初始化了直接展示
            if (mIsInit)
            {
                mGameObject.CustomActive(true);

                if (mView != null)
                {
                    mView.Show();
                }
            }
            else
            {
                if (this.mGameObject == null)
                {
                    this.mGameObject = AssetLoader.instance.LoadResAsGameObject(_GetPrefabPath());
                }

                if (this.mGameObject != null)
                {
                    this.mGameObject.transform.SetParent(root, false);
                    this.mGameObject.CustomActive(true);
                }
                else
                {
                    Logger.LogError("加载活动预制体失败，路径:" + _GetPrefabPath());
                    return;
                }

                mView = mGameObject.GetComponent<IActivityView>();

                if (mView != null)
                {
                    mView.Init(mDataModel, _OnItemClick);
                    this.mIsInit = true;
                }
            }
        }

        public void Hide()
        {
            if (mGameObject != null)
            {
                mGameObject.CustomActive(false);
            }

            if (mView != null)
            {
                mView.Hide();
            }
        }

        public void Close()
        {
            mIsInit = false;
            if (mView != null)
            {
                mView.Close();
            }
            mView = null;
            mGameObject = null;
        }

        //活动数据更新
        public virtual void UpdateData()
        {
	        if (mDataModel == null)
		        return;
            //去datamanager获取最新活动数据
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(mDataModel.Id);
            if (data != null)
            {
                mDataModel = new LimitTimeActivityModel(data, _GetItemPrefabPath());
                if (mView != null)
                {
                    mView.UpdateData(mDataModel);
                }
            }
        }

        //任务更新
        public void UpdateTask(int taskId)
        {
            //数据更新
            mDataModel.UpdateTask(taskId);

            //View刷新
            if (mView != null)
            {
                mView.UpdateData(mDataModel);
            }
        }

        //判断是否有红点
        public virtual bool IsHaveRedPoint()
        {
            //if (mDataModel.TaskDatas == null || mDataModel.State != OpActivityState.OAS_IN)
            //    return false;
            //for (int i = 0; i < mDataModel.TaskDatas.Count; ++i)
            //{
            //    if (mDataModel.TaskDatas[i].State == OpActTaskState.OATS_FINISHED)
            //    {
            //        return true;
            //    }
            //}
            //return false;
            if (mDataModel.TaskDatas == null || mDataModel.State != OpActivityState.OAS_IN)
                return false;

            for (int i = 0; i < mDataModel.TaskDatas.Count; ++i)
            {
                if (mDataModel.TaskDatas[i].State == OpActTaskState.OATS_FINISHED)
                {
                    ILimitTimeActivityTaskDataModel taskDataModel = mDataModel.TaskDatas[i];
                    int totalSubmitlimit = mDataModel.TaskDatas[i].AccountTotalSubmitLimit;
                    int dailySubmitlimit = mDataModel.TaskDatas[i].AccountDailySubmitLimit;
					int WeeklySubmitLimit = mDataModel.TaskDatas [i].AccountWeeklySubmitLimit;
                    if (totalSubmitlimit > 0)
                    {
                        if (ActivityDataManager.GetInstance().GetActivityConunter(taskDataModel.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK) < taskDataModel.AccountTotalSubmitLimit)
                        {
                            return true;
                        }
                    }
                    else if (dailySubmitlimit > 0)
                    {
                        if (ActivityDataManager.GetInstance().GetActivityConunter(taskDataModel.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK) < taskDataModel.AccountDailySubmitLimit)
                        {
                            return true;
                        }
                    }
					else if(WeeklySubmitLimit > 0)
					{
						if (ActivityDataManager.GetInstance ().GetActivityConunter (taskDataModel.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_WEEKLY_SUBMIT_TASK) < taskDataModel.AccountWeeklySubmitLimit) 
						{
							return true;
						}
				   }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //获取活动id
        public uint GetId()
        {
            return mDataModel.Id;
        }

        //获取活动名称
        public string GetName()
        {
            return mDataModel.Name;
        }

        //获取活动状态 结束/开始/准备
        public OpActivityState GetState()
        {
            return mDataModel.State;
        }

        public virtual void Dispose()
        {
            this.mGameObject = null;
            this.mIsInit = false;
            if (this.mView != null)
            {
                this.mView.Dispose();
            }
            this.mView = null;
        }

        //点击事件
        protected virtual void _OnItemClick(int taskId, int param,ulong param2)
        {
            ActivityDataManager.GetInstance().RequestOnTakeActTask(mDataModel.Id, (uint)taskId);
        }

        //活动任务对应的预制体路径
        protected virtual string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/DailyLoginItem";
        }

        //活动界面对应的预制体
        protected virtual string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/CommonActivity";
        }
    }
}