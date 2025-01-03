using Protocol;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ExpGetBackItem : MonoBehaviour
    {
        [SerializeField] private Image mImageIcon;
        [SerializeField] private GameObject mRewardGO2;
        [SerializeField] private Image mImageRewardIcon1;
        [SerializeField] private Image mImageRewardIcon2;
        [SerializeField] private Text mTextCount;
        [SerializeField] private Text mTextReward1;
        [SerializeField] private Text mTextReward2;
        [SerializeField] private GameObject mNormalGO;
        [SerializeField] private GameObject mPerfectGO;
        [SerializeField] private Image mImageHasTaken;
        [SerializeField] private Image mImageNormalCostIcon;
        [SerializeField] private Image mImagePerfectCostIcon;
        [SerializeField] private Image mImageCompleteMask;
        [SerializeField] private Text mTextNoRewards;

        private ExpGetBackActive.ActiveData mData;
        private Action<ExpGetBackActive.ActiveData> mOnNormalClick;
        private Action<ExpGetBackActive.ActiveData> mOnPerfectClick;

        public void Init(ExpGetBackActive.ActiveData data, Action<ExpGetBackActive.ActiveData> onNormalClick, Action<ExpGetBackActive.ActiveData> onPerfectClick)
        {
            if (data != null)
            {
                mData = data;
                mOnNormalClick = onNormalClick;
                mOnPerfectClick = onPerfectClick;
                if (data.ActiveType == ExpGetBackActive.EActiveType.Fatigue)
                {
                    if (data.PerfectRewardNums != null && data.PerfectRewardNums.Count > 0)
                    {
                        mTextCount.SafeSetText(TR.Value("activity_fatigue_get_back_count", data.PerfectRewardNums[0]));
                    }
                }
                else
                {
                    mTextCount.SafeSetText(TR.Value("activity_reward_get_back_count", data.Count));
                }

                ETCImageLoader.LoadSprite(ref mImageIcon, data.Icon);
                //奖励
                if (data.PerfectRewardItemIds.Count > 0 && data.PerfectRewardNums.Count == data.PerfectRewardItemIds.Count)
                {
                    var rewardItem = TableManager.GetInstance().GetTableItem<ItemTable>(data.PerfectRewardItemIds[0]);
                    if (rewardItem != null)
                    {
                        ETCImageLoader.LoadSprite(ref mImageRewardIcon1, rewardItem.Icon);
                    }
                    _SetRewardText(mTextReward1, data.PerfectRewardNums[0].ToString());
                    //mTextReward1.SafeSetText(data.PerfectRewardNums[0].ToString());

                    if (data.PerfectRewardItemIds.Count > 1)
                    {
                        mRewardGO2.CustomActive(true);
                        rewardItem = TableManager.GetInstance().GetTableItem<ItemTable>(data.PerfectRewardItemIds[1]);
                        if (rewardItem != null)
                        {
                            ETCImageLoader.LoadSprite(ref mImageRewardIcon2, rewardItem.Icon);
                        }
                        //mTextReward2.SafeSetText(data.PerfectRewardNums[1].ToString());
                        _SetRewardText(mTextReward2, data.PerfectRewardNums[1].ToString());
                    }
                    else
                    {
                        mRewardGO2.CustomActive(false);
                    }
                }

                //消耗
                if (data.Count > 0)
                {
                    mNormalGO.CustomActive(true);
                    mPerfectGO.CustomActive(true);
                    mImageHasTaken.enabled = false;
                    mImageCompleteMask.enabled = false;
                    mImageHasTaken.enabled = false;
                    mTextNoRewards.enabled = false;

                    var item = TableManager.GetInstance().GetTableItem<ItemTable>(data.NormalCostItemId);
                    if (item != null)
                    {
                        ETCImageLoader.LoadSprite(ref mImageNormalCostIcon, item.Icon);
                    }

                    item = TableManager.GetInstance().GetTableItem<ItemTable>(data.PerfectCostItemId);
                    if (item != null)
                    {
                        ETCImageLoader.LoadSprite(ref mImagePerfectCostIcon, item.Icon);
                    }
                }
                else
                {
                    mNormalGO.CustomActive(false);
                    mPerfectGO.CustomActive(false);
                    if (data.OriginData.status == (int)TaskStatus.TASK_OVER)
                    {
                        mTextNoRewards.enabled = false;
                        mImageHasTaken.enabled = true;
                        mImageCompleteMask.enabled = true;
                    }
                    else
                    {
                        mImageCompleteMask.enabled = false;
                        mImageHasTaken.enabled = false;
                        mTextNoRewards.enabled = true;
                    }
                }
            }
        }

        private void _SetRewardText(Text text, string str)
        {
            var size = text.rectTransform.sizeDelta;
            float width = size.x;
            size.x = StaticUtility.GetTextPreferredWidth(text, str);
            text.rectTransform.sizeDelta = size;
            text.SafeSetText(str);
            width = size.x - width;

            size = (text.transform.parent as RectTransform).sizeDelta;
            size.x += width;
            (text.transform.parent as RectTransform).sizeDelta = size;
        }

        public void OnNormalClick()
        {
            mOnNormalClick?.Invoke(mData);
        }

        public void OnPerfectClick()
        {
            mOnPerfectClick?.Invoke(mData);
        }
    }
}