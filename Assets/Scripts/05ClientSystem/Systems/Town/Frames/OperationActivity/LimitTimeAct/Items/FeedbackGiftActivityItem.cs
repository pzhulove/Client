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
    public class FeedbackGiftActivityItem : ActivityItemBase
    {
        [SerializeField]private Button mUnFinishBtn;
        [SerializeField]private Button mFinishBtn;
        [SerializeField]private Button mOverBtn;
        [SerializeField]private Image mItemBackground;
        [SerializeField]private Image mItemIcon;
        [SerializeField]private Text mItemNum;
        [SerializeField]private Text mTaskCount;
        [SerializeField]private UIGray mGray;
        [SerializeField]private GameObject mOverText;

        private ILimitTimeActivityTaskDataModel mData;

        private bool mIsLeftMinus0 = false;
        private ItemData itemData;
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public sealed override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (!mIsLeftMinus0)
            {
                mUnFinishBtn.CustomActive(false);
                mFinishBtn.CustomActive(false);
                mOverBtn.CustomActive(false);
                mOverText.CustomActive(false);
                mGray.enabled = false;
                switch (data.State)
                {
                    case OpActTaskState.OATS_UNFINISH:
                        mUnFinishBtn.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_OVER:
                        mOverBtn.CustomActive(true);
                        mOverText.CustomActive(true);
                        mGray.enabled = true;
                        break;
                    case OpActTaskState.OATS_FINISHED:
                        mFinishBtn.CustomActive(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                mOverBtn.CustomActive(true);
                mOverText.CustomActive(true);
                mGray.enabled = true;
                mUnFinishBtn.CustomActive(false);
                mFinishBtn.CustomActive(false);
            }
        }

        public sealed override void Dispose()
        {
            base.Dispose();
            
            mIsLeftMinus0 = false;
            itemData = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
        }
        
        protected sealed override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data == null)
            {
                return;
            }

            mData = data;

            if (mData.AwardDataList.Count > 0)
            {
                itemData = ItemDataManager.CreateItemDataFromTable((int)mData.AwardDataList[0].id);

                if (mItemBackground != null)
                {
                    ETCImageLoader.LoadSprite(ref mItemBackground, itemData.GetQualityInfo().Background);
                }

                if (mItemIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mItemIcon, itemData.Icon);
                }

                mItemNum.SafeSetText(mData.AwardDataList[0].num.ToString());
            }

            mTaskCount.SafeSetText(string.Format("{0}积分", mData.TotalNum));

            mUnFinishBtn.SafeRemoveAllListener();
            mUnFinishBtn.SafeAddOnClickListener(OmItemShowTips);

            mFinishBtn.SafeRemoveAllListener();
            mFinishBtn.SafeAddOnClickListener(_OnMyItemClick);

            mOverBtn.SafeRemoveAllListener();
            mOverBtn.SafeAddOnClickListener(OmItemShowTips);

            ShowHaveUsedNumState(data);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
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

        private void OmItemShowTips()
        {
            ItemTipManager.GetInstance().ShowTip(itemData);
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
                    mOverBtn.CustomActive(true);
                    mFinishBtn.CustomActive(false);
                    mUnFinishBtn.CustomActive(false);
                    mIsLeftMinus0 = true;
                    leftNum = 0;
                }
            }
        }
    }
}
