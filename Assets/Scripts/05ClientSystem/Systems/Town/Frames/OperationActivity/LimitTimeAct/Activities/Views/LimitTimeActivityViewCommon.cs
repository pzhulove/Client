using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public interface IActivityCommonItem : IDisposable
    {
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="id">数组id，应该对应数据在数组中的id</param>
		/// <param name="activityId">活动id</param>
        /// <param name="data">数据</param>
		/// <param name="onItemClick">点击事件</param>
		void Init(uint id, uint activityId, ILimitTimeActivityTaskDataModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick);

        void InitFromMode(ILimitTimeActivityModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick);

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        void UpdateData(ILimitTimeActivityTaskDataModel data);

        /// <summary>
        /// 销毁
        /// </summary>
        void Destroy();
    }

    //基础活动界面
    public class LimitTimeActivityViewCommon : MonoBehaviour, IActivityView
    {
        [SerializeField] protected RectTransform mItemRoot = null;
        [SerializeField] protected ActivityNote mNote;
        //用来替换note
        [SerializeField] protected ActivityCommonInfo mComInfo;
        
        protected readonly Dictionary<uint, IActivityCommonItem> mItems = new Dictionary<uint, IActivityCommonItem>();

        protected ActivityItemBase.OnActivityItemClick<int> mOnItemClick;

        protected ILimitTimeActivityModel mModel;
        protected bool mRetunGiftIsUpdate = false;
        protected bool mRetunPrivilegeIsUpdate = false;
        protected bool mActivityIsStart = false;
        //传入活动数据与点击对应事件
        public virtual void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            mModel = model;
            mOnItemClick = onItemClick;
            _InitItems(model);
            if (null != mNote)
                mNote.Init(model);

            if (null != mComInfo)
                mComInfo.OnInit(model);
 
            uint currentTime = TimeManager.GetInstance().GetServerTime();
            uint startTime = 0;
            uint endTime = 0;
            //特殊处理，回归赠礼，周年赠礼任务，不在活动时间内不显示在界面中
            if (mModel.Id == 11013)
            {
                ILimitTimeActivityTaskDataModel taskData = mModel.TaskDatas.Find(x => { return x.DataId == 11013006; });
                if (taskData != null)
                {
                    startTime = taskData.ParamNums[3];
                    endTime = taskData.ParamNums[4];
                    if (currentTime < endTime)
                    {
                        mRetunGiftIsUpdate = true;
                    }
                    
                }
            }//特殊处理，回归特权，周年buff，不在活动时间内不显示在界面中
            else if (mModel.Id == 11017)
            {
                ILimitTimeActivityTaskDataModel taskData = mModel.TaskDatas.Find(x => { return x.DataId == 11017003; });
                if (taskData != null)
                {
                    startTime = taskData.ParamNums[2];
                    endTime = taskData.ParamNums[3];
                    if (currentTime < endTime)
                    {
                        mRetunPrivilegeIsUpdate = true;
                    }
                        
                }
            }
        }

        private void Update()
        {
            if (mRetunGiftIsUpdate)
            {
                ILimitTimeActivityTaskDataModel taskData = mModel.TaskDatas.Find(x => { return x.DataId == 11013006; });
                if (taskData != null)
                {
                    uint currentTime = TimeManager.GetInstance().GetServerTime();
                    uint startTime = taskData.ParamNums[3];
                    uint endTime = taskData.ParamNums[4];

                    if (currentTime >= endTime)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, mModel.Id);
                        mRetunGiftIsUpdate = false;
                    }

                    if (mActivityIsStart == false)
                    {
                        if (currentTime >= startTime && currentTime < endTime)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, mModel.Id);
                            mActivityIsStart = true;
                        }
                    }
                }
                else
                {
                    mRetunGiftIsUpdate = false;
                }
            }

            if (mRetunPrivilegeIsUpdate)
            {
                ILimitTimeActivityTaskDataModel taskData = mModel.TaskDatas.Find(x => { return x.DataId == 11017003; });
                if (taskData != null)
                {
                    uint currentTime = TimeManager.GetInstance().GetServerTime();
                    uint startTime = taskData.ParamNums[2];
                    uint endTime = taskData.ParamNums[3];

                    if (currentTime >= endTime)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, mModel.Id);
                        mRetunPrivilegeIsUpdate = false;
                    }

                    if (mActivityIsStart == false)
                    {
                        if (currentTime >= startTime && currentTime < endTime)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, mModel.Id);
                            mActivityIsStart = true;
                        }
                    }
                }
                else
                {
                    mRetunPrivilegeIsUpdate = false;
                }
            }
        }

        public virtual void UpdateData(ILimitTimeActivityModel data)
	    {
	        if (data.Id == 0 || data.TaskDatas == null || mItems == null)
	        {
	            Logger.LogError("ActivityLimitTimeData data is null");
	            return;
	        }
	        GameObject go = null;

            for (int i = 0; i < data.TaskDatas.Count; ++i)
	        {
                var taskData = data.TaskDatas[i];
                if (taskData == null)
                {
                    continue;
                }

                uint currentTime = TimeManager.GetInstance().GetServerTime();
                uint startTime = 0;
                uint endTime = 0;
               
                if (mItems.ContainsKey(taskData.DataId))
                {
                    mItems[taskData.DataId].UpdateData(taskData);

                    //特殊处理，回归活动周年庆奖励
                    if (taskData.DataId == 11013006)
                    {
                        startTime = taskData.ParamNums[3];
                        endTime = taskData.ParamNums[4];

                        if (currentTime >= endTime)
                        {
                            mItems.Remove(taskData.DataId);
                        }
                    }//特殊处理，周年庆称号属性
                    else if (taskData.DataId == 11017003)
                    {
                        startTime = taskData.ParamNums[2];
                        endTime = taskData.ParamNums[3];

                        if (currentTime >= endTime)
                        {
                            mItems.Remove(taskData.DataId);
                        }
                    }
	            }
	            else
	            {
	                if (go == null)
	                {
	                    go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
	                }

	                _AddItem(go, i, data);
	            }
            }

	        //遍历删除多余的数据
	        List<uint> dataIdList = new List<uint>(mItems.Keys);
	        for (int i = 0; i < dataIdList.Count; ++i)
	        {
	            bool isHave = false;
	            for (int j = 0; j < data.TaskDatas.Count; ++j)
	            {
	                if (dataIdList[i] == data.TaskDatas[j].DataId)
	                {
	                    isHave = true;
	                    break;
	                }
	            }

	            if (!isHave)
	            {
	                var item = mItems[dataIdList[i]];
	                mItems.Remove(dataIdList[i]);
	                item.Destroy();
	            }
	        }

            if (go != null)
	        {
	            Destroy(go);
	        }
        }

	    public virtual void Dispose()
	    {
	        foreach (var item in mItems.Values)
	        {
	            item.Dispose();
	        }

	        mItems.Clear();
            mOnItemClick = null;
	        if (mNote != null)
	        {
	            mNote.Dispose();
	        }
            mModel = null;
            mRetunGiftIsUpdate = false;
            mRetunPrivilegeIsUpdate = false;
            mActivityIsStart = false;
        }

        public virtual void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
        }
        
        protected virtual void _InitItems(ILimitTimeActivityModel data)
        {

            GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
            if (go == null)
            {
                Logger.LogError("加载预制体失败，路径:" + data.ItemPath);
                return;
            }

            if (go.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go);
                Logger.LogError("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + data.ItemPath);
                return;
            }

            mItems.Clear();

            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                var taskData = data.TaskDatas[i];
                if (taskData == null)
                {
                    continue;
                }
                
                _AddItem(go, i, data);
            }
            Destroy(go);
        }

        protected void _AddItem(GameObject go, int id, ILimitTimeActivityModel data)
        {
            uint currentTime = TimeManager.GetInstance().GetServerTime();
            uint startTime = 0;
            uint endTime = 0;
            if (data.TaskDatas[id].DataId == 11013006)
            {
                startTime = data.TaskDatas[id].ParamNums[3];
                endTime = data.TaskDatas[id].ParamNums[4];

                if (currentTime < startTime || currentTime >= endTime)
                {
                    return;
                }
            }
            else if (data.TaskDatas[id].DataId == 11017003)
            {
                startTime = data.TaskDatas[id].ParamNums[2];
                endTime = data.TaskDatas[id].ParamNums[3];

                if (currentTime < startTime || currentTime >= endTime)
                {
                    return;
                }
            }
            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoot, false);
            item.GetComponent<IActivityCommonItem>().Init(data.TaskDatas[id].DataId, data.Id, data.TaskDatas[id], mOnItemClick);
            mItems.Add(data.TaskDatas[id].DataId, item.GetComponent<IActivityCommonItem>());
        }

    }
}
