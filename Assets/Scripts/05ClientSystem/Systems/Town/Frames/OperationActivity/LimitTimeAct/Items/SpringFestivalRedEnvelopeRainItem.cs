using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class SpringFestivalRedEnvelopeRainItem : ActivityItemBase
    {
        [SerializeField] private Text mDateDesc;
        [SerializeField] private Text mActiveDesc;
        [SerializeField] private Text mActiveValue;
        [SerializeField] private Text mOnlineTimeDesc;
        [SerializeField] private Text mOnlineTimeValue;
        [SerializeField] private Image mBackgroupImg;
        [SerializeField] private Image mIconImg;
        [SerializeField] private GameObject mGoUnReceived;
        [SerializeField] private GameObject mGoReceived;
        [SerializeField] private GameObject mGoHasReceived;
        [SerializeField] private Button mBtnReceived;
        [SerializeField] private Button mBtnItem;

        private ILimitTimeActivityTaskDataModel mData;
        private bool bIsToReceived = false; // 是否领取过
        public sealed override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            mGoUnReceived.CustomActive(false);
            mGoReceived.CustomActive(false);
            mGoHasReceived.CustomActive(false);

            if (!bIsToReceived)
            {
                switch (data.State)
                {
                    case Protocol.OpActTaskState.OATS_UNFINISH:
                        mGoUnReceived.CustomActive(true);
                        break;
                    case Protocol.OpActTaskState.OATS_FINISHED:
                        mGoReceived.CustomActive(true);
                        break;
                    case Protocol.OpActTaskState.OATS_OVER:
                        mGoHasReceived.CustomActive(true);
                        break;
                }
            }
            else
            {
                mGoUnReceived.CustomActive(false);
                mGoReceived.CustomActive(false);
                mGoHasReceived.CustomActive(true);
            }
           
            if (mOnlineTimeValue != null)
            {
                mOnlineTimeValue.text = string.Format("({0}/1)", data.ParamProgressList.Count);
            }
        }

        protected sealed override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            mData = data;
            if (mDateDesc != null)
            {
                mDateDesc.text = data.taskName;
            }

            string task1 = string.Empty;
            string task2 = string.Empty;
            string[] taskDescs = data.Desc.Split('，');
            if (taskDescs != null && taskDescs.Length >= 2)
            {
                task1 = taskDescs[0];
                task2 = taskDescs[1];
            }

            if (task1 != "" && task2 != "")
            {
                if (mActiveDesc != null)
                {
                    mActiveDesc.text = task1;
                }

                if (mOnlineTimeDesc != null)
                {
                    mOnlineTimeDesc.text = task2;
                }
            }

            if (mActiveValue != null)
            {
                mActiveValue.text = string.Format("({0}/{1})", (int)(data.DoneNum / data.TotalNum), data.TotalNum / data.TotalNum);
            }

            if (mOnlineTimeValue != null)
            {
                mOnlineTimeValue.text = string.Format("({0}/1)", data.ParamProgressList.Count);
            }

            if (data.AwardDataList != null && data.AwardDataList.Count > 0)
            {
                OpTaskReward rewad = data.AwardDataList[0];
                int itemId = (int)rewad.id;
                int count = (int)rewad.num;

                ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemId);
                if (itemData != null)
                {
                    itemData.Count = count;

                    if (mBtnItem != null)
                    {
                        mBtnItem.onClick.RemoveAllListeners();
                        mBtnItem.onClick.AddListener(() => 
                        {
                            ItemTipManager.GetInstance().ShowTip(itemData);
                        });
                    }

                    if (mBackgroupImg != null)
                    {
                        ETCImageLoader.LoadSprite(ref mBackgroupImg, itemData.GetQualityInfo().Background);
                    }

                    if (mIconImg != null)
                    {
                        ETCImageLoader.LoadSprite(ref mIconImg, itemData.Icon);
                    }
                }
            }

            if (mBtnReceived != null)
            {
                mBtnReceived.SafeAddOnClickListener(_OnItemClick);
            }

            RegisterAccountData(_OnActivityCounterUpdate);
            OnRequsetAccountData(data);
        }

        public sealed override void Dispose()
        {
            base.Dispose();

            if (mBtnReceived != null)
            {
                mBtnReceived.SafeRemoveOnClickListener(_OnItemClick);
            }

            UnRegisterAccountData(_OnActivityCounterUpdate);
            mData = null;
            bIsToReceived = false;
        }

        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if (mData != null)
            {
                if ((uint)uiEvent.Param1 == mData.DataId)
                {
                    int accountTotalSubmit = mData.AccountTotalSubmitLimit;
                    int totalNum = 0;
                    int haveNum = 0;
                   
                    if (accountTotalSubmit > 0)
                    {
                        haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                        totalNum = accountTotalSubmit;
                    }
                    if (totalNum > 0)
                    {
                        int leftNum = totalNum - haveNum;
                        if (leftNum <= 0)
                        {
                            bIsToReceived = true;
                            mGoUnReceived.CustomActive(false);
                            mGoReceived.CustomActive(false);
                            mGoHasReceived.CustomActive(true);
                            leftNum = 0;
                        }
                    }
                }
            }
        }
    }
}