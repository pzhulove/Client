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
    public class OnLineGiftGivingItem : ActivityItemBase
    {
        [SerializeField] private Text mTaskName;
        [SerializeField] private Image mItemBackground;
        [SerializeField] private Image mItemIcon;
        [SerializeField] private Text mItemCount;
        [SerializeField] private Button mReceiveBtn;
        [SerializeField] private Button mItemIconBtn;
        [SerializeField] private GameObject mArrowItemGo;
        [SerializeField] private GameObject mReceiveGrayItemGo;//已领取
        [SerializeField] private GameObject mNotReachItemGo;//未达成

        private ILimitTimeActivityTaskDataModel mData;

        private bool mIsLeftMinus0 = false;

        private bool isNumberDayActivity = false;
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public sealed override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (!mIsLeftMinus0)
            {
                mReceiveGrayItemGo.CustomActive(false);
                mNotReachItemGo.CustomActive(false);
                mReceiveBtn.CustomActive(false);
                switch (data.State)
                {
                    case OpActTaskState.OATS_UNFINISH:
                        mNotReachItemGo.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_OVER:
                        mReceiveGrayItemGo.CustomActive(true);
                        break;
                    case OpActTaskState.OATS_FINISHED:
                        mReceiveBtn.CustomActive(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                mReceiveGrayItemGo.CustomActive(true);
                mNotReachItemGo.CustomActive(false);
                mReceiveBtn.CustomActive(false);
            }
        }

        public sealed override void Dispose()
        {
            mIsLeftMinus0 = false;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
        }
        
        protected sealed override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (mTaskName != null)
            {
                mTaskName.text = data.Desc;
            }

            if (data.AwardDataList.Count == 1)
            {
                OpTaskReward reward = data.AwardDataList[0];

                if (reward != null)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)reward.id);
                    if (itemData != null)
                    {
                        if (mItemBackground != null)
                        {
                            ETCImageLoader.LoadSprite(ref mItemBackground, itemData.GetQualityInfo().Background);
                        }

                        if (mItemIcon != null)
                        {
                            ETCImageLoader.LoadSprite(ref mItemIcon, itemData.Icon);
                        }

                        if (reward.num > 1)
                        {
                            if (mItemCount != null)
                            {
                                mItemCount.text = reward.num.ToString();
                            }
                        }
                        else
                        {
                            if (mItemCount != null)
                            {
                                mItemCount.text = string.Empty;
                            }
                        }

                        if (mItemIconBtn != null)
                        {
                            mItemIconBtn.SafeRemoveAllListener();
                            mItemIconBtn.SafeAddOnClickListener(()=> { ItemTipManager.GetInstance().ShowTip(itemData); });
                        }
                    }
                }
            }

            if (mReceiveBtn != null)
            {
                mReceiveBtn.SafeRemoveAllListener();
                mReceiveBtn.SafeAddOnClickListener(_OnMyItemClick);
            }

            mData = data;
            ShowHaveUsedNumState(data);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
        }

        public void OnSetArrowIsShow(bool isFlag)
        {
            if (mArrowItemGo != null)
            {
                mArrowItemGo.CustomActive(isFlag);
            }
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
                    mReceiveGrayItemGo.CustomActive(true);
                    mNotReachItemGo.CustomActive(false);
                    mReceiveBtn.CustomActive(false);
                    mIsLeftMinus0 = true;
                    leftNum = 0;
                }
            }
        }
    }
}
