using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;

namespace GameClient
{
    public class LimitTimeHellItem : ActivityItemBase
    {
        [SerializeField]
        private Text mDes;
        [SerializeField]
        private Text mCount;
        [SerializeField]
        private Text mTaskCount;
        [SerializeField]
        private RectTransform mRewardItemRoot;
        [SerializeField]
        private ScrollRect mAwardsScrollRect;
        [SerializeField]
        private Button mButtonExchange;
        [SerializeField]
        private UIGray mButtonExchangeGray;
        [SerializeField]
        Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField]
        private int mScrollCount = 5;//超过多少时才能滑动


        private List<ComItem> mComItems = new List<ComItem>();
        ILimitTimeActivityModel mActivityData;
        ILimitTimeActivityModel mActivityDataNew;
        public override void InitFromMode(ILimitTimeActivityModel data, OnActivityItemClick<int> onItemClick)
        {
            mActivityData = data;
            mOnItemClick = onItemClick;
            for (int i = 0;i<data.TaskDatas.Count;i++)
            {
                if(data.TaskDatas[i].State == OpActTaskState.OATS_OVER)
                {
                    continue;
                }
                InitTaskItem(data.TaskDatas[i]);
                break;
            }
            if(data.TaskDatas.Count > 0 && data.TaskDatas[data.TaskDatas.Count - 1].State == OpActTaskState.OATS_OVER)//如果所有任务都处于完成状态，则显示最后一个任务
            {
                InitTaskItem(data.TaskDatas[data.TaskDatas.Count - 1]);
            }
        }

        private void InitTaskItem(ILimitTimeActivityTaskDataModel taskData)
        {
            mId = taskData.DataId;
            if (taskData != null && taskData.AwardDataList != null)
            {
                if (mComItems != null)
                {
                    for (int i = this.mComItems.Count - 1; i >= 0; --i)
                    {
                        ComItemManager.Destroy(mComItems[i]);
                    }
                    mComItems.Clear();
                }
                for (int i = 0; i < taskData.AwardDataList.Count; ++i)
                {
                    var comItem = ComItemManager.Create(mRewardItemRoot.gameObject);
                    if (comItem != null)
                    {
                        ItemData item = ItemDataManager.CreateItemDataFromTable((int)taskData.AwardDataList[i].id);
                        item.Count = (int)taskData.AwardDataList[i].num;
                        comItem.Setup(item, Utility.OnItemClicked);
                        mComItems.Add(comItem);
                        (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                    }
                }
                mAwardsScrollRect.enabled = taskData.AwardDataList.Count > mScrollCount;
            }

            mDes.text = taskData.Desc;
            if(taskData.DoneNum > mActivityData.Param)
            {
                mCount.text = string.Format("{0}/{1}", mActivityData.Param, mActivityData.Param);
            }
            else
            {
                mCount.text = string.Format("{0}/{1}", taskData.DoneNum, mActivityData.Param);
            }
            mTaskCount.text = GetTaskCount();
            mButtonExchange.SafeRemoveOnClickListener(_OnItemClick);
            mButtonExchange.SafeAddOnClickListener(_OnItemClick);
            if(taskData.State != OpActTaskState.OATS_FINISHED)
            {
                mButtonExchangeGray.enabled = true;
                mButtonExchange.interactable = false;
            }
            else
            {
                mButtonExchangeGray.enabled = false;
                mButtonExchange.interactable = true;
            }
        }

        private string GetTaskCount()
        {
            int finishedTask = 0;
            for (int i = 0; i < mActivityData.TaskDatas.Count; i++)
            {
                if(mActivityData.TaskDatas[i].State == OpActTaskState.OATS_OVER)
                {
                    finishedTask++;
                }
            }
            return string.Format("{0}/{1}", mActivityData.TaskDatas.Count - finishedTask, mActivityData.TaskDatas.Count);
        }

        //任务刷新时，尝试筛选此时需要显示的任务
        private void tryUpdateTask(ILimitTimeActivityModel data)
        {
            if(mActivityData == null)
            {
                mActivityData = data;
            }
            //bool flag = false;
            //for (int i = 0; i < mActivityData.TaskDatas.Count; i++)
            //{
            //    if (mActivityData.TaskDatas[i].State == OpActTaskState.OATS_OVER)
            //    {
            //        flag = true;
            //        continue;
            //    }
            //    InitTaskItem(mActivityData.TaskDatas[i]);
            //    break;
            //}
            //if (!flag && mActivityData.TaskDatas.Count > 0)//如果所有任务都处于完成状态，则显示最后一个任务
            //{
            //    InitTaskItem(mActivityData.TaskDatas[mActivityData.TaskDatas.Count - 1]);
            //}
            for (int i = 0; i < mActivityData.TaskDatas.Count; i++)
            {
                if (mActivityData.TaskDatas[i].State == OpActTaskState.OATS_OVER)
                {
                    continue;
                }
                InitTaskItem(mActivityData.TaskDatas[i]);
                break;
            }
            if (mActivityData.TaskDatas.Count > 0 && mActivityData.TaskDatas[mActivityData.TaskDatas.Count - 1].State == OpActTaskState.OATS_OVER)//如果所有任务都处于完成状态，则显示最后一个任务
            {
                InitTaskItem(mActivityData.TaskDatas[mActivityData.TaskDatas.Count - 1]);
            }
        }
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            Logger.LogErrorFormat("old updateData");
            //tryUpdateTask();
        }
        public void UpdateFromMode(ILimitTimeActivityModel data)
        {
            //Logger.LogErrorFormat("new updateData");
            tryUpdateTask(data);
        }
        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {

        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }
            mButtonExchange.SafeRemoveOnClickListener(_OnItemClick);
        }
        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }
        
        //protected override void _OnItemClick()
        //{
        //    if (mOnItemClick != null)
        //    {
        //        mOnItemClick((int)mId, 0, mCurSelectFashionUid);
        //    }
        //}
        
    }
}
