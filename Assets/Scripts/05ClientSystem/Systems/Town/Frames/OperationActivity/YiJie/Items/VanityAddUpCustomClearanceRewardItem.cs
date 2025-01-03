using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public class VanityAddUpCustomClearanceRewardItem : ActivityItemBase
    {
        [SerializeField]
        private Text mTextDescription;
        [SerializeField]
        private GameObject mItemRoot;
        [SerializeField]
        private Button mGoButton;
        [SerializeField]
        Button mButtonTakeReward;
        [SerializeField]
        private GameObject mHasTakenReward;
        [SerializeField]
        private Vector2 mComItemSize = new Vector2();
        [SerializeField]
        private Text mOwnNum;
        [SerializeField]
        private Text mTatleNum;
        private List<ComItem> mComItems = new List<ComItem>();
        private ILimitTimeActivityTaskDataModel mTaskData;
        [SerializeField]
        private GameObject mProgressGo;
        [SerializeField]
        private GameObject mAccountNumGo;
        [SerializeField]
        private Text mCanNumTxt;
        [SerializeField]
        private Text mTotalNumTxt;
        [SerializeField]
        private Vector2 mAccountGoPos = new Vector2();
        private ILimitTimeActivityTaskDataModel mData;

        private bool isLeftMinusThan0 = true;
        public sealed override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if(isLeftMinusThan0)
            {
                if (data.State == OpActTaskState.OATS_UNFINISH ||
               data.State == OpActTaskState.OATS_INIT)
                {

                    mGoButton.CustomActive(true);
                    mButtonTakeReward.CustomActive(false);
                    mHasTakenReward.CustomActive(false);


                }
                else if (data.State == OpActTaskState.OATS_FINISHED)
                {
                    mGoButton.CustomActive(false);
                    mButtonTakeReward.CustomActive(true);
                    mHasTakenReward.CustomActive(false);
                }
                else if (data.State == OpActTaskState.OATS_OVER)
                {
                    mGoButton.CustomActive(false);
                    mButtonTakeReward.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                }
            }
            else
            {
                mGoButton.CustomActive(false);
                mButtonTakeReward.CustomActive(false);
                mHasTakenReward.CustomActive(true);
            }
           
        }

        protected sealed override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            mTaskData = data;
            if (data.AwardDataList.Count <= 0)
            {
                Logger.LogError("data.AwardDataList 数组数量为0");
                return;
            }
            
            for (int i = 0; i < data.AwardDataList.Count; i++)
            {
                var comItem = ComItemManager.Create(this.mItemRoot.gameObject);
                if (comItem != null)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[i].id);
                    itemData.Count = (int)data.AwardDataList[i].num;
                    comItem.Setup(itemData, Utility.OnItemClicked);
                    (comItem.transform as RectTransform).sizeDelta = this.mComItemSize;
                    mComItems.Add(comItem);
                }
            }

            if (mTextDescription != null)
            {
                mTextDescription.SafeSetText(data.Desc);
            }
            
            if (mTatleNum != null)
            {
                mTatleNum.SafeSetText(data.TotalNum.ToString());
            }

            if (mOwnNum != null)
            {
                mOwnNum.SafeSetText(data.DoneNum.ToString());
            }

            if (mGoButton != null)
            {
                mGoButton.SafeAddOnClickListener(OnGoBtnClick);
            }

            if (mButtonTakeReward != null)
            {
                mButtonTakeReward.SafeAddOnClickListener(_OnItemClick);
                mButtonTakeReward.SafeAddOnClickListener(_OnSendMsg);
            }
            if(data.AccountTotalSubmitLimit>0)
            {
                mAccountNumGo.CustomActive(true);
            }
            else
            {
                mAccountNumGo.CustomActive(false);
            }
            mData = data;
            base.RegisterAccountData(_OnActivityCounterUpdate);
            base.OnRequsetAccountData(data);
        }
        
        public sealed override void Dispose()
        {
            base.Dispose();

            for (int i = this.mComItems.Count - 1; i >= 0; --i)
            {
                ComItemManager.Destroy(mComItems[i]);
            }
            mComItems.Clear();

            if (mButtonTakeReward != null)
            {
                mButtonTakeReward.SafeRemoveOnClickListener(_OnItemClick);
                mButtonTakeReward.SafeRemoveOnClickListener(_OnSendMsg);
            }
            
            if (mGoButton != null)
            {
                mGoButton.SafeRemoveOnClickListener(OnGoBtnClick);
            }
            base.UnRegisterAccountData(_OnActivityCounterUpdate);
            mTaskData = null;
        }

        private void OnGoBtnClick()
        {
            if(mTaskData!=null)
            {
                if (mTaskData.ParamNums2.Count == 1)
                {
                    if (mTaskData.ParamNums2[0] == (int)DungeonTable.eSubType.S_DEVILDDOM)
                    {
                        Utility.PathfindingYiJieMap();
                    }
                    
                }
                else if (mTaskData.ParamNums2.Count == 3)
                {
                    if(mTaskData.ParamNums2[0]==(int)DungeonTable.eSubType.S_WEEK_HELL&& mTaskData.ParamNums2[1] == (int)DungeonTable.eSubType.S_WEEK_HELL_ENTRY
                        && mTaskData.ParamNums2[2] == (int)DungeonTable.eSubType.S_WEEK_HELL_PER)
                    {
                        ChallengeUtility.OnOpenChallengeMapFrame(ProtoTable.DungeonModelTable.eType.WeekHellModel, 0);
                        if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
                        {
                            ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
                        }

                    }

                }
            }
        }
        private void _OnSendMsg()
        {
            if (mData != null)
            {
                base.OnRequsetAccountData(mData);
            }
        }
        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if (mData != null)
            {
                if ((uint)uiEvent.Param1 == mData.DataId)
                {
                    int accountDailySubmit = mData.AccountDailySubmitLimit;
                    int accountTotalSubmit = mData.AccountTotalSubmitLimit;
                    int totalNum = 0;
                    int haveNum = 0;
                    if (accountDailySubmit > 0)
                    {
                        haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                        totalNum = accountDailySubmit;
                    }
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
                            isLeftMinusThan0 = false;
                            mGoButton.CustomActive(false);
                            mButtonTakeReward.CustomActive(false);
                            mHasTakenReward.CustomActive(true);

                            mProgressGo.CustomActive(false);
                            mAccountNumGo.GetComponent<RectTransform>().anchoredPosition = mAccountGoPos;
                            leftNum = 0;
                        }
                        mCanNumTxt.SafeSetText(leftNum.ToString());
                        mTotalNumTxt.SafeSetText(totalNum.ToString());
                    }
                }
            }
        }


    }
}

