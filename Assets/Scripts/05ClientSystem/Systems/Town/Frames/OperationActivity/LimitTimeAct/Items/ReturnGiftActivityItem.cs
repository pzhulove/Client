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
    public class ReturnGiftActivityItem : ActivityItemBase
    {
        [SerializeField]
        private Text mTextDescription;
        [SerializeField]
        private RectTransform mRewardItemRoot;
        [SerializeField]
        Button mButtonTakeRewardFree;
        [SerializeField]
        Button mButtonTakeReward;
        [SerializeField]
        GameObject mHasTakenReward;
        [SerializeField]
        GameObject mUnTakeReward;
        [SerializeField]
        GameObject mCanTakeRewardFree;
        [SerializeField]
        GameObject mCanTakeReward;
        [SerializeField]
        private ScrollRect mAwardsScrollRect;
        [SerializeField]
        private Text mGoldNum;
        [SerializeField]
        private Image mGoldIcon;
        [SerializeField]
        private Text mBuyDes;
        [SerializeField]
        Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField]
        private int mScrollCount = 5;//超过多少时才能滑动

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
                    mCanTakeRewardFree.CustomActive(false);
                    //mUnTakeReward.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                    break;
                case OpActTaskState.OATS_FINISHED:
                    //mUnTakeReward.CustomActive(false);
                    mHasTakenReward.CustomActive(false);
                    mCanTakeReward.CustomActive(true);
                    mCanTakeRewardFree.CustomActive(true);
                    break;
                default:
                    //mUnTakeReward.CustomActive(true);
                    mHasTakenReward.CustomActive(false);
                    mCanTakeReward.CustomActive(false);
                    mCanTakeRewardFree.CustomActive(false);
                    break;
            }
            if (data.ParamNums2.Count > 1)
            {
                if (data.ParamNums2[1] == 0)
                {
                    mGoldIcon.CustomActive(false);
                    mGoldNum.CustomActive(false);
                    mCanTakeRewardFree.CustomActive(true);
                    mCanTakeReward.CustomActive(false);
                }
                else
                {
                    mGoldIcon.CustomActive(true);
                    mGoldNum.CustomActive(true);
                    mCanTakeRewardFree.CustomActive(false);
                    mCanTakeReward.CustomActive(true);
                }
            }
            if (data.ParamNums[0] == 1)
            {
                mBuyDes.text = string.Format(TR.Value("activity_coin_exchange_item_exchange_count_account"), data.DoneNum, data.TotalNum);
            }
            else
            {
                mBuyDes.text = string.Format(TR.Value("activity_coin_exchange_item_exchange_count_role"), data.DoneNum, data.TotalNum);
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
            mButtonTakeRewardFree.SafeRemoveOnClickListener(_OnItemClick);
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
                        comItem.labCount.fontSize = 28;
                    }
                }
                mAwardsScrollRect.enabled = data.AwardDataList.Count > mScrollCount;
            }
            mTextDescription.SafeSetText(data.Desc);
            mButtonTakeReward.SafeAddOnClickListener(_OnItemClick);
            mButtonTakeRewardFree.SafeAddOnClickListener(_OnItemClick);
            
            if(data.ParamNums2.Count > 1)
            {
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)data.ParamNums2[0]);
                if (itemTableData != null)
                {
                    ETCImageLoader.LoadSprite(ref mGoldIcon, itemTableData.Icon);

                }
                mGoldNum.text = data.ParamNums2[1].ToString();
                if (data.ParamNums2[1] == 0)
                {
                    mGoldIcon.CustomActive(false);
                    mGoldNum.CustomActive(false);
                    mCanTakeRewardFree.CustomActive(true);
                    mCanTakeReward.CustomActive(false);
                }
                else
                {
                    mGoldIcon.CustomActive(true);
                    mGoldNum.CustomActive(true);
                    mCanTakeRewardFree.CustomActive(false);
                    mCanTakeReward.CustomActive(true);
                }
            }
            //if(data.CountParamNums.Count > 0)
            //{
            //    mGoldNum.text = data.CountParamNums[0].value.ToString();
            //    var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)data.CountParamNums[0].currencyId);
            //    if(itemTableData != null)
            //    {
            //        ETCImageLoader.LoadSprite(ref mGoldIcon, itemTableData.Icon);
            //    }
            //}
        }

    }
}
