using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class QiXiQueQiaoActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField] private Text mTextTime;
        [SerializeField] private Text mTextRule;
        [SerializeField] private Text mTextTotalProgress;
        [SerializeField] private Image mImageProgress;
        [SerializeField] private List<Text> mTextTargets;
        [SerializeField] private Text mTextLastTarget;
        [SerializeField] private Button mButtonTryOn;
        [SerializeField] private Color mFinishedColor;
        [SerializeField] private Color mUnFinishedColor;
        [SerializeField] private Transform mCurrentTaskItemRoot;
        [SerializeField] private Button mButtonTakeAward;
        [SerializeField] private Button mButtonFinalReward;
        [SerializeField] private GameObject mFinishedGO;
        [SerializeField] private GameObject mUnFinishedGO;
        [SerializeField] private GameObject mEndGO;
        [SerializeField] private GameObject mEffectCompleteAllTask;
        [SerializeField] private Color mTaskCompleteColor = Color.red;
        [SerializeField] private Color mTaskUnCompleteColor = Color.green;

        [SerializeField] private Transform mRoot;
        private ComItem mItemComItem;

        private ComItem mComItem;
        private int mCurrentTaskId = -1;
        private int mLastRewardItemId = -1;
        private int mLastGiftPackItemId = -1;
        private bool mIsInitAvatar = false;

        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mOnItemClick = onItemClick;
            _InitItems(model);
            UpdateData(model);
            mButtonTakeAward.SafeAddOnClickListener(_OnButtonTakeAwardClick);
            mButtonTryOn.SafeAddOnClickListener(_OnButtonTryOnClick);
            mButtonFinalReward.SafeAddOnClickListener(_OnButtonFinalRewardClick);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            if (model.TaskDatas != null && model.TaskDatas.Count > 0)
            {
                var awards = model.TaskDatas[model.TaskDatas.Count - 1].AwardDataList;
                if (awards != null && awards.Count > 0)
                {
                    mLastGiftPackItemId = (int)awards[0].id;
                    GiftPackDataManager.GetInstance().GetGiftPackItem(mLastGiftPackItemId);
                }

            }
            if (mRoot != null)
            {
                if (mItemComItem != null)
                {
                    ComItemManager.Destroy(mItemComItem);
                    mItemComItem = null;
                }
                OpTaskReward reward=  model.TaskDatas[model.TaskDatas.Count - 1].AwardDataList[0];
                if(reward!=null)
                {
                    mItemComItem = ComItemManager.Create(mRoot.gameObject);
                    ItemData data = ItemDataManager.CreateItemDataFromTable((int)reward.id, 100, reward.strenth);
                    data.Count = (int)reward.num;
                    mItemComItem.Setup(data, Utility.OnItemClicked);
                }
              
            }
        }

        public override void UpdateData(ILimitTimeActivityModel model)
        {
            if (model == null)
            {
                return;
            }
            base.UpdateData(model);
            _UpdateTitleInfo(model);
            _UpdateProgressInfo(model);
            _UpdateCurrentTaskInfo(model);
        }

        void _UpdateTitleInfo(ILimitTimeActivityModel model)
        {
            mTextTime.SafeSetText(string.Format(TR.Value("activity_qi_xi_que_qiao_time"),
                Function.GetTimeWithoutYearNoZero((int) model.StartTime, (int) model.EndTime))); // string.Format(TR.Value("activity_qi_xi_que_qiao_time"), startTime.Month, startTime.Day, endTime.Month, endTime.Day));
        }

        void _UpdateProgressInfo(ILimitTimeActivityModel model)
        {
            if (model.TaskDatas != null)
            {
                for (int i = 0; i < model.TaskDatas.Count; ++i)
                {
                    if (i >= mTextTargets.Count)
                    {
                        break;
                    }

                    if (mTextTargets[i] == null)
                    {
                        continue;
                    }

                    switch (model.TaskDatas[i].State)
                    {
                        case OpActTaskState.OATS_FINISHED:
                        case OpActTaskState.OATS_OVER:
                            mTextTargets[i].color = mFinishedColor;
                            break;
                        default:
                            mTextTargets[i].color = mUnFinishedColor;
                            break;
                    }
                    //mTextTargets[i].SafeSetText(model.TaskDatas[i].Desc);
                }

                mTextLastTarget.SafeSetText(string.Format(TR.Value("activity_qi_xi_que_qiao_item_reward"), model.TaskDatas[model.TaskDatas.Count - 1].Desc));

                if (mImageProgress != null && model.ParamArray != null && model.ParamArray.Length > 1)
                {
                    float finishedValue = 0f;
                    for (int i = 0; i < model.TaskDatas.Count; ++i)
                    {
                        finishedValue += model.TaskDatas[i].DoneNum;
                    }
                    mImageProgress.fillAmount = finishedValue / model.ParamArray[0];
                    string color = Utility.GetHexColor(mTaskUnCompleteColor);
                    if (finishedValue >= model.ParamArray[0])
                    {
                        color = Utility.GetHexColor(mTaskCompleteColor);
                    }
                    mTextTotalProgress.SafeSetText(string.Format(TR.Value("activity_qi_xi_que_qiao_total_progress"), color, finishedValue, "</color>", model.ParamArray[0]));
                }
            }
            //今日完成度
            if (model.ParamArray != null && model.ParamArray.Length >= 2)
            {
                string countKey = string.Format("{0}{1}", model.Id, CounterKeys.OPACT_MAGPIE_BRIDGE_DAILY_PROGRESS);
                var completeValue = CountDataManager.GetInstance().GetCount(countKey);
                var maxValue = model.ParamArray[1];
                string color = Utility.GetHexColor(mTaskUnCompleteColor);
                if (completeValue >= maxValue)
                {
                    color = Utility.GetHexColor(mTaskCompleteColor);
                }
                mTextRule.SafeSetText(model.RuleDesc + string.Format(TR.Value("activity_qi_xi_que_qiao_today_progress"), color, completeValue, "</color>", model.ParamArray[1]));
                //mTextTodayProgress.SafeSetText();
            }
        }

        void _UpdateCurrentTaskInfo(ILimitTimeActivityModel model)
        {
            if (model.TaskDatas == null || model.TaskDatas.Count == 0)
            {
                return;
            }

            ILimitTimeActivityTaskDataModel showTask = null;

            for (int i = 0; i < model.TaskDatas.Count; ++i)
            {
                if (model.TaskDatas[i].State == OpActTaskState.OATS_UNFINISH)
                {
                    showTask = model.TaskDatas[i];
                    break;
                }
                else if (model.TaskDatas[i].State == OpActTaskState.OATS_FINISHED)
                {
                    showTask = model.TaskDatas[i];
                    break;
                }
            }

            bool isLastTask = false;
            if (showTask == null)
            {
                showTask = model.TaskDatas[model.TaskDatas.Count - 1];
                isLastTask = true;
            }

            //如果不是当前显示的任务
            if (showTask.DataId != mCurrentTaskId)
            {
                mCurrentTaskId = (int)showTask.DataId;
                if (mComItem != null)
                {
                    ComItemManager.Destroy(mComItem);
                    mComItem = null;
                }

                mComItem = ComItemManager.Create(mCurrentTaskItemRoot.gameObject);
                ItemData data = ItemDataManager.CreateItemDataFromTable((int)showTask.AwardDataList[0].id, 100, showTask.AwardDataList[0].strenth);
                data.Count = (int)showTask.AwardDataList[0].num;
                mComItem.Setup(data, Utility.OnItemClicked);
            }

            switch (showTask.State)
            {
                case OpActTaskState.OATS_FINISHED:
                    mFinishedGO.CustomActive(true);
                    mUnFinishedGO.CustomActive(false);
                    mEndGO.CustomActive(false);
                    break;
                case OpActTaskState.OATS_OVER:
                    mFinishedGO.CustomActive(false);
                    mUnFinishedGO.CustomActive(false);
                    mEndGO.CustomActive(true);
                    if (isLastTask)
                    {
                        mEffectCompleteAllTask.CustomActive(true);
                    }
                    break;
                default:
                    mFinishedGO.CustomActive(false);
                    mUnFinishedGO.CustomActive(true);
                    mEndGO.CustomActive(false);
                    break;
            }

        }

        void _OnButtonTakeAwardClick()
        {
            if (mOnItemClick != null)
            {
                mOnItemClick((int)mCurrentTaskId, 0);
            }
        }

        void _OnButtonTryOnClick()
        {
            if (mLastGiftPackItemId > 0)
            {
                _ShowAvartar();
            }
        }

        void _OnButtonFinalRewardClick()
        {
            if (mLastRewardItemId > 0)
            {
                ItemData data = ItemDataManager.CreateItemDataFromTable(mLastRewardItemId);
                if (data != null)
                {
                    Utility.OnItemClicked(null, data);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItem != null)
            {
                ComItemManager.Destroy(mComItem);
                mComItem = null;
            }
            mButtonTakeAward.SafeRemoveOnClickListener(_OnButtonTakeAwardClick);
            mButtonTryOn.SafeRemoveOnClickListener(_OnButtonTryOnClick);
            mButtonFinalReward.SafeRemoveOnClickListener(_OnButtonFinalRewardClick);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            mIsInitAvatar = false;
            mCurrentTaskId = -1;
            mLastRewardItemId = -1;
        }

        public override void Show()
        {
            if (mIsInitAvatar)
            {
                _ShowAvartar();
            }
        }

        void _ShowAvartar()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<PlayerTryOnFrame>())
            {
                var tryOnFrame = ClientSystemManager.GetInstance().GetFrame(typeof(PlayerTryOnFrame)) as PlayerTryOnFrame;
                if (tryOnFrame != null)
                {
                    tryOnFrame.Reset(mLastRewardItemId);
                }
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, mLastRewardItemId);
            }
        }

        void _OnGetGiftData(UIEvent param)
        {
            if (param == null || param.Param1 == null)
            {
                Logger.LogError("礼包数据为空");
                return;
            }
            GiftPackSyncInfo data = param.Param1 as GiftPackSyncInfo;
            if (data != null && data.id == mLastGiftPackItemId)
            {
                for (int j = 0; j < data.gifts.Length; ++j)
                {
                    GiftPackItemData giftTable = GiftPackDataManager.GetGiftDataFromNet(data.gifts[j]);
                    if (giftTable.ItemID > 0 && giftTable.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                    {
                        mLastRewardItemId = giftTable.ItemID;
                        if (mIsInitAvatar)
                        {
                            _ShowAvartar();
                        }
                        break;
                    }
                }
            }

        }
    }
}
