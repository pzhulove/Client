using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class HellTicketItem : ActivityItemBase
    {
        [SerializeField] private Text mTextDescription;
        [SerializeField] private Text mTextProgress;
        [SerializeField] private RectTransform mRewardItemRoot;
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] GameObject mHasTakenReward;
        [SerializeField] GameObject mNotFinishGO;
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
                    break;
                default:
                    mNotFinishGO.CustomActive(true);
                    mHasTakenReward.CustomActive(false);
                    break;
            }
            mTextDescription.SafeSetText(data.Desc);
            mTextProgress.SafeSetText(string.Format("{0}/{1}", data.DoneNum, data.TotalNum));
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
        }

    }
}
