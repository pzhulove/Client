using Protocol;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TuanBenSupportActivityView : MonoBehaviour, IActivityView
    {

        [SerializeField] private GameObject mHaveTakenGo;
        [SerializeField] private GameObject mNotTakenGo;
        [SerializeField] private GameObject mCanTakenGo;
        [SerializeField] private Button mTakeRewardBtn;
        [SerializeField] private Text mTimeTxt;
        [SerializeField] private Text mRuleDesTxt;
        [SerializeField] private Text mItemName;
        [SerializeField] private Text mItemCount;
        [SerializeField] private Image mItemBackGround;
        [SerializeField] private Image mItemIcon;
        [SerializeField] private Button mItemIconBtn;
        [SerializeField] private Text mAccountWeeklyLimitDesc;
        [SerializeField] private Text mRoleWeeklyLimitDesc;
        [SerializeField] private string mWeeklyDesc = "账号每周限领次数:{0}/{1}";
        [SerializeField] private string mRoleWeeklyDesc = "角色每周限领次数:{0}/1";


        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private ILimitTimeActivityTaskDataModel mTaskData = null;
        private bool mIsLeftMinus0 = false;
		private ILimitTimeActivityModel mModel;
        public  void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

			mModel = model;
			mIsLeftMinus0 = false;
            mOnItemClick = onItemClick;
            mTakeRewardBtn.SafeAddOnClickListener(_OnTakeRewardBtnClick);

            //mTimeTxt.SafeSetText(string.Format("{0}~{1}", Function._TransTimeStampToStr(model.StartTime), Function._TransTimeStampToStr(model.EndTime)));
            mTimeTxt.SafeSetText(Function.GetTimeWithMonthDayHour((int)model.StartTime, (int)model.EndTime));
            mRuleDesTxt.SafeSetText(model.RuleDesc);

            UpdateData(model);
            _InitItems();
            ShowWeeklyUseNumState(mTaskData);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);

			if (mTaskData != null && mTaskData.AccountWeeklySubmitLimit > 0)
			{
				ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mTaskData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_WEEKLY_SUBMIT_TASK);
			}
        }
        
        public void UpdateData(ILimitTimeActivityModel data)
        {
            if (data.Id == 0 || data.TaskDatas == null || data.TaskDatas.Count < 1)
            {
                return;
            }

            mHaveTakenGo.CustomActive(false);
            mNotTakenGo.CustomActive(false);
            mCanTakenGo.CustomActive(false);

            mTaskData = data.TaskDatas[0];
            if (mTaskData == null) return;

            if (!mIsLeftMinus0)
            {
				UpdateRoleWeeklyLimitDesc (1);

                switch (mTaskData.State)
                {
                    case Protocol.OpActTaskState.OATS_UNFINISH:
                        mNotTakenGo.CustomActive(true);
                        break;
                    case Protocol.OpActTaskState.OATS_FINISHED:
                        mCanTakenGo.CustomActive(true);
                        break;
				case Protocol.OpActTaskState.OATS_OVER:
					    mHaveTakenGo.CustomActive (true);
					    UpdateRoleWeeklyLimitDesc (0);
                        break;
                }
            }
            else
            {
                mHaveTakenGo.CustomActive(true);
                mNotTakenGo.CustomActive(false);
                mCanTakenGo.CustomActive(false);
            }
        }

        public  void Close()
        {
            mIsLeftMinus0 = false;
            mTaskData = null;
            mOnItemClick = null;
            mTakeRewardBtn.SafeRemoveOnClickListener(_OnTakeRewardBtnClick);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
            Destroy(gameObject);
        }

        public void Show()
        {
            gameObject.CustomActive(true);
        }

        public void Hide()
        {
            gameObject.CustomActive(false);
        }

        public void Dispose()
        {

        }
        private void _OnTakeRewardBtnClick()
        {
            if (mOnItemClick != null && mTaskData!=null)
            {
                mOnItemClick((int)mTaskData.DataId, 0, 0);

                if (mTaskData.AccountWeeklySubmitLimit > 0)
                {
                    ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mTaskData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_WEEKLY_SUBMIT_TASK);
                }
            }
        }
        
        private void _InitItems()
        {
            if (mTaskData != null)
            {
                if (mTaskData.AwardDataList != null && mTaskData.AwardDataList.Count >= 1)
                {
                    OpTaskReward taskReward = mTaskData.AwardDataList[0];
                    if (taskReward == null) return;
                  
                    ItemData itemData= ItemDataManager.CreateItemDataFromTable((int)taskReward.id);
                    if (itemData == null) return;
                    itemData.Count =(int)taskReward.num;
                   
                    if (mItemName != null)
                    {
                        mItemName.text = itemData.GetColorName();
                    }

                    if (mItemBackGround != null)
                    {
                        ETCImageLoader.LoadSprite(ref mItemBackGround, itemData.GetQualityInfo().Background);
                    }

                    if (mItemIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref mItemIcon, itemData.Icon);
                    }

                    if (mItemCount != null)
                    {
                        if (itemData.Count > 1)
                            mItemCount.text = itemData.Count.ToString();
                    }

                    if (mItemIconBtn != null)
                    {
                        mItemIconBtn.onClick.RemoveAllListeners();
                        mItemIconBtn.onClick.AddListener(() => 
                        {
                            ItemTipManager.GetInstance().ShowTip(itemData);
                        });
                    }
                }
            }
        }

        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if (mTaskData != null)
            {
                if ((uint)uiEvent.Param1 == mTaskData.DataId)
                {
                    ShowWeeklyUseNumState(mTaskData);
                }

            }
        }

        private void UpdateRoleWeeklyLimitDesc(int num)
        {
            if(mRoleWeeklyLimitDesc != null)
            {
                mRoleWeeklyLimitDesc.text = string.Format(mRoleWeeklyDesc, num);
            }
        }

        private void ShowWeeklyUseNumState(ILimitTimeActivityTaskDataModel data)
        {
			mIsLeftMinus0 = false;

            if (data != null)
            {
                int totalNum = 0;
                int haveNum = 0;

                if (data.AccountWeeklySubmitLimit > 0)
                {
                    haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(data.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_WEEKLY_SUBMIT_TASK);
                    totalNum = data.AccountWeeklySubmitLimit;
                }

                int leftNum = totalNum - haveNum;
				if (leftNum <= 0 && totalNum > 0) {
					mHaveTakenGo.CustomActive (true);
					mNotTakenGo.CustomActive (false);
					mCanTakenGo.CustomActive (false);
					mIsLeftMinus0 = true;
					leftNum = 0;
					UpdateRoleWeeklyLimitDesc (0);
				}

                if(mAccountWeeklyLimitDesc != null)
                {
                    mAccountWeeklyLimitDesc.text = string.Format(mWeeklyDesc, leftNum, totalNum);
                }

				UpdateData(mModel);
            }
        }
    }
}
