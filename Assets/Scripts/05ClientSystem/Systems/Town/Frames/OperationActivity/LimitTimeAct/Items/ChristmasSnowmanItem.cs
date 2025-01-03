using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChristmasSnowmanItem : ActivityItemBase
    {
        [SerializeField]
        private Button mUnFinishBtn;
        [SerializeField]
        private Button mFinishBtn;
        [SerializeField]
        private Button mOverBtn;
        [SerializeField]
        private Image mIconImage;
        [SerializeField]
        private Text mItemCount;
        ItemData mItemData;
        public override sealed void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            switch (data.State)
            {
                case OpActTaskState.OATS_INIT:
                case OpActTaskState.OATS_UNFINISH:
                    mUnFinishBtn.CustomActive(true);
                    mFinishBtn.CustomActive(false);
                    mOverBtn.CustomActive(false);
                    break;
                case OpActTaskState.OATS_FINISHED:
                    mUnFinishBtn.CustomActive(false);
                    mFinishBtn.CustomActive(true);
                    mOverBtn.CustomActive(false);
                    break;
                case OpActTaskState.OATS_FAILED:
                    break;
                case OpActTaskState.OATS_SUBMITTING:
                    break;
                case OpActTaskState.OATS_OVER:
                    mUnFinishBtn.CustomActive(false);
                    mFinishBtn.CustomActive(false);
                    mOverBtn.CustomActive(true);
                    break;
                default:
                    break;
            }
        }

        protected override sealed void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            SetItemInfo(data);
            if (mUnFinishBtn != null)
            {
                mUnFinishBtn.onClick.RemoveAllListeners();
                mUnFinishBtn.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(mItemData); });
            }
            if (mFinishBtn != null)
            {
                mFinishBtn.onClick.RemoveAllListeners();
                mFinishBtn.onClick.AddListener(_OnItemClick);
            }
            if (mOverBtn != null)
            {
                mOverBtn.onClick.RemoveAllListeners();
                mOverBtn.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(mItemData); });
            }
        }

        public override sealed void Dispose()
        {
            base.Dispose();
            if (mUnFinishBtn != null)
            {
                mUnFinishBtn.onClick.RemoveAllListeners();
            }
            if (mFinishBtn != null)
            {
                mFinishBtn.onClick.RemoveAllListeners();
            }
            if (mOverBtn != null)
            {
                mOverBtn.onClick.RemoveAllListeners();
            }
            mItemData = null;
        }

        void SetItemInfo(ILimitTimeActivityTaskDataModel data)
        {
            if (data.AwardDataList.Count > 0)
            {
                int itemId = (int)data.AwardDataList[0].id;
                int num = (int)data.AwardDataList[0].num;
                mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(itemId);
                if (mItemData == null)
                {
                    return;
                }

                ETCImageLoader.LoadSprite(ref mIconImage, mItemData.Icon);
                if (num > 1)
                {
                    if (mItemCount != null)
                    {
                        mItemCount.text = num.ToString();
                    }
                }
                
            }
        }
    }
}
