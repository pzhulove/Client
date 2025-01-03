using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CoinExchangeItem : ActivityItemBase
    {
        [SerializeField] private Text mTextExchangeCount;
        [SerializeField] private Image mImageCoinIcon;
        [SerializeField] private Text mTextCoinCount;
        [SerializeField] private Text mTextCoinOwnNum;
        [SerializeField] private Button mButtonExchange;
        [SerializeField] private Button mButtonExchangeBlue;
        [SerializeField] private RectTransform mRewardItemRoot;
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] GameObject mHasTakenReward;
        [SerializeField] GameObject mNotFinishGO;
        [SerializeField] GameObject mFinishedGO;

        [SerializeField] Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField] private int mScrollCount = 5;//超过多少时才能滑动

        private List<ComItem> mComItems = new List<ComItem>();

        private ILimitTimeActivityTaskDataModel mData;

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            switch (data.State)
            {
                case OpActTaskState.OATS_OVER:
                    mNotFinishGO.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                    mFinishedGO.CustomActive(false);
                    break;
                case OpActTaskState.OATS_FINISHED:
                    mFinishedGO.CustomActive(true);
                    mNotFinishGO.CustomActive(false);
                    mHasTakenReward.CustomActive(false);
                    break;
                case OpActTaskState.OATS_UNFINISH:
                    mFinishedGO.CustomActive(false);
                    mNotFinishGO.CustomActive(true);
                    mHasTakenReward.CustomActive(false);
                    break;
            }

            if (data.State == OpActTaskState.OATS_OVER)
            {
                if(data.AccountDailySubmitLimit==0&&data.AccountTotalSubmitLimit==0)
                {
                    mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_count_role"), 0, data.TotalNum));
                }
              
               
            }
            else
            {
                if (data.AccountDailySubmitLimit == 0 && data.AccountTotalSubmitLimit == 0)
                {
                    mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_count_role"), data.DoneNum, data.TotalNum));
                }
                
            }

			int coinNum = CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", mActivityId, CounterKeys.COUNTER_ACTIVITY_FATIGUE_COIN_NUM));
			var paramNumList = data.ParamNums;
            if (paramNumList.Count == 0)
            {
                return;
            }
            mTextCoinCount.SafeSetText(string.Format("/{0}", paramNumList[0]));
            mTextCoinOwnNum.SafeSetText(coinNum.ToString());
            if (coinNum < paramNumList[0])
            {
                mTextCoinOwnNum.color = Color.red;
            }
            else
            {
                mTextCoinOwnNum.color = Color.green;
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
            mButtonExchange.SafeRemoveOnClickListener(_OnItemClick);
            mButtonExchange.SafeRemoveOnClickListener(_OnSendMsg);
            mButtonExchangeBlue.SafeRemoveOnClickListener(_OnItemClick);
        }

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null && data.AwardDataList != null)
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
            mButtonExchange.SafeAddOnClickListener(_OnItemClick);
            mButtonExchangeBlue.SafeAddOnClickListener(_OnItemClick);
            mButtonExchange.SafeAddOnClickListener(_OnSendMsg);
            base.RegisterAccountData(_OnActivityCounterUpdate);
            base.OnRequsetAccountData(data);
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
            if(mData!=null)
            {
                if ((uint)uiEvent.Param1 == mData.DataId)
                {
                    int accountDailySubmit = mData.AccountDailySubmitLimit;
                    int accountTotalSubmit = mData.AccountTotalSubmitLimit;
                    int totalNum = 0;
                    int haveNum = 0;
                    if (accountDailySubmit > 0)
                    {
                        haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                        totalNum = accountDailySubmit;
                    }
                    if (accountTotalSubmit > 0)
                    {
                        haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                        totalNum = accountTotalSubmit;
                    }
                    if(totalNum>0)
                    {
                       
                        int leftNum = totalNum - haveNum;
                        if (leftNum <= 0)
                        {
                            leftNum = 0;
                        }
                        mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_count_account"), leftNum, totalNum));
                    }
                }
            }
        }
    }
}
