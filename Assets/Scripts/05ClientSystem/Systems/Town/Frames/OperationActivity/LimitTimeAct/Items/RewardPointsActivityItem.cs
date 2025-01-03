using System;
using System.Collections;
using System.Collections.Generic;
using ActivityLimitTime;
using Network;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class RewardPointsActivityItem : ActivityItemBase
    {
        [SerializeField] private Text mTaskName;
        [SerializeField] private Text mIntegralCount;
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Button mReceiveBtn;
        [SerializeField] private GameObject mReceiveGrayItemGo;//已领取
        [SerializeField] private GameObject mNotReachItemGo;//未达成
        [SerializeField] private Vector2 mComItemSize = new Vector2(100f, 100f);

        private List<ComItem> mComItems = new List<ComItem>();

        private ILimitTimeActivityTaskDataModel mData;

        private bool mIsLeftMinus0 = false;

        private int totalScore = 0;
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public sealed override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (!mIsLeftMinus0)
            {
                mReceiveGrayItemGo.CustomActive(false);
                mNotReachItemGo.CustomActive(false);
                mReceiveBtn.CustomActive(false);
                switch (data.State)
                {
                    case OpActTaskState.OATS_UNFINISH:
                        mNotReachItemGo.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_OVER:
                        mReceiveGrayItemGo.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_FINISHED:
                        mReceiveBtn.CustomActive(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                mReceiveGrayItemGo.CustomActive(true);
                mNotReachItemGo.CustomActive(false);
                mReceiveBtn.CustomActive(false);
            }
        }

        public sealed override void Dispose()
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
            mIsLeftMinus0 = false;
            totalScore = 0;
        }
        
        protected sealed override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            int activityId = ActivityDataManager.GetInstance()._GetLimitTimeActivityIdByTaskId(data.DataId);

            var activityData = ActivityDataManager.GetInstance().GetLimitTimeActivityData((uint)activityId);
            if (activityData != null)
            { //挑战者俱乐部积分奖励
                if (activityData.tmpType == (int)ActivityLimitTimeFactory.EActivityType.OAT_CHALLENGEPOINTSREWARDACTIVITY)
                {
                    //特殊处理，任务id为1592001，积分读取每日积分
                    if (data.DataId == 1592001)
                    {
                        totalScore = CountDataManager.GetInstance().GetCount(CounterKeys.TOTAL_CHALLENGE_DAY_SCORE);
                    }
                    else
                    {
                        totalScore = CountDataManager.GetInstance().GetCount(CounterKeys.TOTAL_CHALLENGE_SCORE);
                    }
                    
                }//春季挑战俱乐部积分奖励
                else if (activityData.tmpType == (int)ActivityLimitTimeFactory.EActivityType.OAT_SPRINGENGEPOINTSREWARDACTIVITY)
                {
                    totalScore = CountDataManager.GetInstance().GetCount(CounterKeys.TOTAL_SPRING_SCORE);
                }
            }
           
            if (mTaskName != null)
            {
                mTaskName.text = data.Desc;
            }

            if (mIntegralCount != null)
            {
                mIntegralCount.text = string.Format("{0}/{1}", totalScore, data.ParamNums2[0]);
            }

            for (int i = 0; i < data.AwardDataList.Count; i++)
            {
                OpTaskReward reward = data.AwardDataList[i];
                if (reward == null)
                {
                    continue;
                }

                var comItem = ComItemManager.Create(mItemParent);
                if (comItem != null)
                {
                    ItemData item = ItemDataManager.CreateItemDataFromTable((int)reward.id);
                    item.Count = (int)reward.num;
                    comItem.Setup(item, Utility.OnItemClicked);
                    mComItems.Add(comItem);
                    (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                }
            }
           
            if (mReceiveBtn != null)
            {
                mReceiveBtn.SafeRemoveAllListener();
                mReceiveBtn.SafeAddOnClickListener(_OnMyItemClick);
            }

            mData = data;
            ShowHaveUsedNumState(data);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
        }

        private void _OnMyItemClick()
        {
            _OnItemClick();
            if (mData != null)
            {
                if (mData.AccountDailySubmitLimit > 0)
                {
                    ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                }
                if (mData.AccountTotalSubmitLimit > 0)
                {
                    ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                }
            }
        }

        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if (mData != null)
            {
                if ((uint)uiEvent.Param1 == mData.DataId)
                {
                    ShowHaveUsedNumState(mData);
                }

            }
        }

        /// <summary>
        /// 显示账号次数
        /// </summary>
        private void ShowHaveUsedNumState(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null)
            {
                int totalNum = 0;
                int haveNum = 0;

                if (data.AccountDailySubmitLimit > 0)
                {
                    haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(data.DataId,
                        ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                    totalNum = data.AccountDailySubmitLimit;
                }
                else if (data.AccountTotalSubmitLimit > 0)
                {
                    haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(data.DataId,
                       ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                    totalNum = data.AccountTotalSubmitLimit;
                }

                int leftNum = totalNum - haveNum;
                if (leftNum <= 0 && totalNum > 0)
                {
                    mReceiveGrayItemGo.CustomActive(true);
                    mNotReachItemGo.CustomActive(false);
                    mReceiveBtn.CustomActive(false);
                    mIsLeftMinus0 = true;
                    leftNum = 0;
                }
            }
        }
    }
}
