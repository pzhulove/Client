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
    public class ReservationUpgradeItem : ActivityItemBase
    {
        [SerializeField] private Text mTextDescription;
        [SerializeField] private Text mTextProgress;
        [SerializeField] private RectTransform mRewardItemRoot;
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] Button mButtonTakeReward;
        [SerializeField] GameObject mHasTakenReward;
        [SerializeField] GameObject mNotFinishGO;
        [SerializeField] GameObject mFinishGO;
        [SerializeField] GameObject mCanNotTakeReward;
        [SerializeField] Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField] private int mScrollCount = 5;//超过多少时才能滑动

        private List<ComItem> mComItems = new List<ComItem>();

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (PlayerBaseData.GetInstance().appointmentOccu == 1)
            {
                if (OpActTaskState.OATS_OVER == data.State)
                {
                    mTextProgress.SafeSetText(string.Format("<color=#00FF56FF>{0}</color>/{1}", data.TotalNum, data.TotalNum));
                    mFinishGO.CustomActive(false);
                    mNotFinishGO.CustomActive(false);
                    mCanNotTakeReward.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                }
                else if (OpActTaskState.OATS_FINISHED == data.State)
                {
                    mTextProgress.SafeSetText(string.Format("<color=#00FF56FF>{0}</color>/{1}", data.TotalNum, data.TotalNum));
                    mNotFinishGO.CustomActive(false);
                    mHasTakenReward.CustomActive(false);
                    mCanNotTakeReward.CustomActive(false);
                    mFinishGO.CustomActive(true);
                }
                else
                {
                    mTextProgress.SafeSetText(string.Format("<color=#AE0000FF>{0}</color>/{1}", data.DoneNum, data.TotalNum));
                    mNotFinishGO.CustomActive(true);
                    mHasTakenReward.CustomActive(false);
                    mCanNotTakeReward.CustomActive(false);
                    mFinishGO.CustomActive(false);
                }
            }
            else
            {
                mTextProgress.SafeSetText(string.Format("{0}/{1}", data.DoneNum, data.TotalNum));
                mNotFinishGO.CustomActive(false);
                mHasTakenReward.CustomActive(false);
                mCanNotTakeReward.CustomActive(true);
                mFinishGO.CustomActive(false);
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

            mTextDescription.SafeSetText(data.Desc);
            mButtonTakeReward.SafeAddOnClickListener(_OnItemClick);

        }

    }
}
