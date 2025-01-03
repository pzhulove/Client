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
    public class DailyLoginItem : ActivityItemBase
    {
        [SerializeField] private Text mTextDescription;
        [SerializeField] private RectTransform mRewardItemRoot;
        [SerializeField] Button mButtonTakeReward;
        [SerializeField] GameObject mHasTakenReward;
        [SerializeField] GameObject mUnTakeReward;
        [SerializeField] GameObject mCanTakeReward;
        [SerializeField] private ScrollRect mAwardsScrollRect;
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
                    mCanTakeReward.CustomActive(false);
                    mUnTakeReward.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                    break;
                case OpActTaskState.OATS_FINISHED:
                    mUnTakeReward.CustomActive(false);
                    mHasTakenReward.CustomActive(false);
                    mCanTakeReward.CustomActive(true);
                    break;
                default:
                    mUnTakeReward.CustomActive(true);
                    mHasTakenReward.CustomActive(false);
                    mCanTakeReward.CustomActive(false);
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
            mButtonTakeReward.SafeAddOnClickListener(_OnItemClick);
        }

    }
}
