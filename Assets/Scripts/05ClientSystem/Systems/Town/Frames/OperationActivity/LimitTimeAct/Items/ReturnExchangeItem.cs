using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ReturnExchangeItem : ActivityItemBase
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

            //if (data.State == OpActTaskState.OATS_OVER)
            //{
            //    mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_exchange_count"), 0, data.TotalNum));
            //}
            //else
            //{
            //    mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_exchange_count"), data.DoneNum, data.TotalNum));
            //}

            if(data.ParamNums2.Count > 0)
            {
                if (data.ParamNums2[0] == 0)
                {
                    mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_exchange_count_role"), data.DoneNum, data.TotalNum));
                }
                else
                {
                    mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_exchange_count_account"), data.DoneNum, data.TotalNum));
                }
            }

			ulong coinNum = AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_COUNTER_ENERGY_COIN);
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
            mButtonExchange.SafeAddOnClickListener(_OnItemClick);
            mButtonExchangeBlue.SafeAddOnClickListener(_OnItemClick);
        }

    }
}
