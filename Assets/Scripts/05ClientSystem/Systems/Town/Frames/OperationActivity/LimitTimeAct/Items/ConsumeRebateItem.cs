using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ConsumeRebateItem : ActivityItemBase
    {
        [SerializeField]
        private Text mTaskDes;
        [SerializeField]
        private GameObject mItemRoot;
        [SerializeField]
        private Vector2 mComItemSize = new Vector2(100, 100);

        [SerializeField]
        private Button mReceiveBtn;
        [SerializeField]
        private GameObject mUnFinishGo;
        [SerializeField]
        private GameObject mHaveReceiveGo;
        [SerializeField]
        private Text mAccountLimitTxt;

        private bool mIsLeftMinus0 = false;

        private ILimitTimeActivityTaskDataModel mData;
        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            mTaskDes.SafeSetText(string.Format(TR.Value("TaskDes_Content"),data.Desc,":"));
            if (data.AwardDataList != null)
            {
                for (int i = 0; i < data.AwardDataList.Count; i++)
                {
                    _CreateItem(data.AwardDataList[i]);
                }
            }
            mData = data;
            mReceiveBtn.SafeAddOnClickListener(_OnMyItemClick);
            ShowHaveUsedNumState(data);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
        }

      

        private void _OnMyItemClick()
        {
            int limitLv = 0;
            int.TryParse(TR.Value("ConsumeRebateLimitPlayerGrade"), out limitLv);
            if (PlayerBaseData.GetInstance().Level >= limitLv)
            {
                _OnItemClick();
                if(mData!=null)
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
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format("活动等级不匹配，需要等级{0},当前等级{1}", limitLv, PlayerBaseData.GetInstance().Level));
            }
        }

        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if(!mIsLeftMinus0)
            {
                switch (data.State)
                {
                    case Protocol.OpActTaskState.OATS_INIT:
                    case Protocol.OpActTaskState.OATS_UNFINISH:
                        mUnFinishGo.CustomActive(true);
                        mReceiveBtn.CustomActive(false);
                        mHaveReceiveGo.CustomActive(false);
                        break;
                    case Protocol.OpActTaskState.OATS_FINISHED:
                        mReceiveBtn.CustomActive(true);
                        mUnFinishGo.CustomActive(false);
                        mHaveReceiveGo.CustomActive(false);
                        break;
                    case Protocol.OpActTaskState.OATS_OVER:
                        mHaveReceiveGo.CustomActive(true);
                        mUnFinishGo.CustomActive(false);
                        mReceiveBtn.CustomActive(false);
                        break;
                }
            }
            else
            {
                mHaveReceiveGo.CustomActive(true);
                mUnFinishGo.CustomActive(false);
                mReceiveBtn.CustomActive(false);
            }
           
        }

        public override void Dispose()
        {
            base.Dispose();
            mReceiveBtn.SafeRemoveOnClickListener(_OnMyItemClick);
        }
        private void _CreateItem(OpTaskReward opTaskReward)
        {
            if (opTaskReward != null)
            {
                ComItem comItem = ComItemManager.Create(mItemRoot);
                ItemData item = ItemDataManager.CreateItemDataFromTable((int)opTaskReward.id);
                if (comItem != null && item != null)
                {
                    comItem.GetComponent<RectTransform>().sizeDelta = mComItemSize;
                    item.Count = (int)opTaskReward.num;
                    comItem.Setup(item, Utility.OnItemClicked);
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
                if (leftNum <= 0)
                {
                    mHaveReceiveGo.CustomActive(true);
                    mUnFinishGo.CustomActive(false);
                    mReceiveBtn.CustomActive(false);
                    mIsLeftMinus0 = true;
                    leftNum = 0;
                }
                mAccountLimitTxt.CustomActive(totalNum>0);
                mAccountLimitTxt.SafeSetText(string.Format(TR.Value("ConsumeRebate_AccountLimt_Content"), leftNum, totalNum));

            }
        }

    }
}
