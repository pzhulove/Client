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
    public class AccumulateLoginItem : ActivityItemBase
    {
        [SerializeField] private Text mTextDescription;
        [SerializeField] private RectTransform mRewardItemRoot;
        [SerializeField] Button mButtonTakeReward;
        [SerializeField] GameObject mHasTakenReward;
        [SerializeField] GameObject mUnTakeReward;
        [SerializeField] GameObject mCanTakeReward;
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] private Text mCount;
        [SerializeField] private Text mAccountNumTxt;
        [SerializeField] Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField] private int mScrollCount = 5;//超过多少时才能滑动

        private List<ComItem> mComItems = new List<ComItem>();
        private ILimitTimeActivityTaskDataModel mData;

        private bool mIsLeftMinus0 = false;
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (!mIsLeftMinus0)
            {
                mCanTakeReward.CustomActive(false);
                mCount.CustomActive(false);
                mHasTakenReward.CustomActive(false);
                switch (data.State)
                {
                    case OpActTaskState.OATS_UNFINISH:
                        mCount.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_OVER:
                        mHasTakenReward.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_FINISHED:
                        mCanTakeReward.CustomActive(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                mCanTakeReward.CustomActive(false);
                mCount.CustomActive(false);
                mHasTakenReward.CustomActive(true);
            }
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
            mButtonTakeReward.SafeRemoveOnClickListener(_OnItemClick);
        }

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
	        if (data.AwardDataList != null)
	        {
		        for (int i = 0; i < data.AwardDataList.Count; ++i)
		        {
			        var comItem = ComItemManager.Create(mRewardItemRoot.gameObject);
			        if (comItem != null)
			        {
				        ItemData item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[i].id);
				        item.Count = (int)data.AwardDataList[i].num;
				        comItem.Setup(item, Utility.OnItemClicked);
				        mComItems.Add(comItem);
				        (comItem.transform as RectTransform).sizeDelta = mComItemSize;
			        }
		        }
		        mAwardsScrollRect.enabled = data.AwardDataList.Count > mScrollCount;
	        }
			mTextDescription.SafeSetText(data.Desc);
            mButtonTakeReward.SafeAddOnClickListener(_OnMyItemClick);
            mCount.text = string.Format(TR.Value("activity_accumulate_login_tips"),data.DoneNum,data.TotalNum);

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
                    mCanTakeReward.CustomActive(false);
                    mCount.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                    mIsLeftMinus0 = true;
                    leftNum = 0;
                }

                mAccountNumTxt.CustomActive(totalNum > 0);
                mAccountNumTxt.SafeSetText(string.Format(TR.Value("ConsumeRebate_AccountLimt_Content"), leftNum, totalNum));
            }
        }
    }
}
