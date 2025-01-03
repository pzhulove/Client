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
    public class AccumulateClearanceItem : ActivityItemBase
    {
        [SerializeField]
        private Text mTextDescription;
        [SerializeField]
        private Text mAccountNumTxt;
        [SerializeField]
        private RectTransform mRewardItemRoot;
        [SerializeField]
        Button mButtonUnFinish;
        [SerializeField]
        Button mButtonTakeReward;
        [SerializeField]
        GameObject mHasTakenReward;
        //[SerializeField]
        //GameObject mUnTakeReward;
        [SerializeField]
        GameObject mUnFinish;
        [SerializeField]
        GameObject mCanTakeReward;
        [SerializeField]
        private ScrollRect mAwardsScrollRect;
        //[SerializeField]
        //private Text mGoldNum;
        //[SerializeField]
        //private Image mGoldIcon;
        //[SerializeField]
        //private Text mBuyDes;
        [SerializeField]
        Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField]
        private int mScrollCount = 5;//超过多少时才能滑动

        private List<ComItem> mComItems = new List<ComItem>();

        private ILimitTimeActivityTaskDataModel mData;

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            mCanTakeReward.CustomActive(false);
            mButtonUnFinish.CustomActive(false);
            mHasTakenReward.CustomActive(false);
            switch (data.State)
            {
                case OpActTaskState.OATS_UNFINISH:
                    mButtonUnFinish.CustomActive(true);
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
            mButtonTakeReward.SafeRemoveOnClickListener(_OnSendMsg);
            base.UnRegisterAccountData(_OnActivityCounterUpdate);
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
            mData = data;
            mAccountNumTxt.CustomActive(false);
            mTextDescription.CustomActive(false);
            mButtonTakeReward.SafeAddOnClickListener(_OnItemClick);
            mButtonTakeReward.SafeAddOnClickListener(_OnSendMsg);

            base.RegisterAccountData(_OnActivityCounterUpdate);
            base.OnRequsetAccountData(mData);
        }

        private void _OnSendMsg()
        {
            if(mData!=null)
            {
                base.OnRequsetAccountData(mData);
            }
           
        }

        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if (mData != null)
            {
                if ((uint)uiEvent.Param1 == mData.DataId)
                {
                    ShowHaveUsedNumState();
                }
            }
        }
     
        /// <summary>
        /// 显示账号次数
        /// </summary>
        private void ShowHaveUsedNumState()
        {
            if(mData!=null)
            {
                int accountDailySubmit = mData.AccountDailySubmitLimit;
                int accountTotalSubmit = mData.AccountTotalSubmitLimit;
                int totalNum = 0;
                if(accountDailySubmit>0)
                {
                    totalNum = accountDailySubmit;
                }
                if(accountTotalSubmit>0)
                {
                    totalNum = accountTotalSubmit;
                }
                if(totalNum>0)
                {
                    int haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                    int leftNum = totalNum - haveNum;
                    if (leftNum < 0)
                    {
                        leftNum = 0;
                    }
                    if (mAccountNumTxt != null)
                    {
                        mAccountNumTxt.CustomActive(true);
                        mAccountNumTxt.text = string.Format(TR.Value("limitactivity_dailyreward_tip", leftNum, totalNum));
                    }

                    if (leftNum <= 0)
                    {
                        mHasTakenReward.CustomActive(true);

                        mCanTakeReward.CustomActive(false);
                        mButtonUnFinish.CustomActive(false);
                        mButtonUnFinish.CustomActive(false);
                    }

                    ShowtxtDescription(mData, leftNum <= 0);
                }
                else
                {
                    ShowtxtDescription(mData, true);
                }
              
            }

          

        }

        private void ShowtxtDescription(ILimitTimeActivityTaskDataModel data,bool isHideProgress)
        {
            if (mData != null)
            {
                mTextDescription.CustomActive(true);
                if (!isHideProgress)
                {
                   
                    mTextDescription.text = string.Format(TR.Value("activity_accumulate_exchange_content"), mData.Desc, string.Format(TR.Value("activity_accumulate_exchange_tips"), mData.DoneNum, mData.TotalNum));
                }
                else
                {
                    mTextDescription.text = string.Format("{0}", mData.Desc);
                }
               
            }
        }
    }
}
