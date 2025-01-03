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
    public class VanityDailyCustomClearanceRewardItem : ActivityItemBase
    {
        [SerializeField] private Text mTextDescription;
        [SerializeField] private Image mImageBg;
        [SerializeField] private RectTransform mItemRoot;
        [SerializeField] Vector2 mComItemSize;
        [SerializeField] Button mButtonTakeReward;
        [SerializeField] GameObject mHasTakenReward;
        [SerializeField]
        Button mButtonGo;
        [SerializeField]
        Text mTaskCount;

        private List<ComItem> mComItems = new List<ComItem>();

        private ILimitTimeActivityTaskDataModel mData;

        protected sealed override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data == null)
            {
                Logger.LogError("data is null");
                return;
            }
            mData = data;
            mTextDescription.text = data.Desc;
            if (mButtonTakeReward == null)
            {
                Logger.LogError("按钮为空了，检查预制体DailyRewardItem");
            }


            UpdateData(data);
            mButtonTakeReward.SafeAddOnClickListener(_OnItemClick);
            mButtonGo.SafeAddOnClickListener(OnGoBtnClick);
            InitItems(data.AwardDataList);
        }

        public sealed override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (data.State == OpActTaskState.OATS_FINISHED)
            {
                mButtonTakeReward.gameObject.CustomActive(true);
                mHasTakenReward.CustomActive(false);
                mButtonGo.CustomActive(false);
            }
            else if (data.State == OpActTaskState.OATS_OVER)
            {
                mButtonTakeReward.gameObject.CustomActive(false);
                mHasTakenReward.CustomActive(true);
                mButtonGo.CustomActive(false);
            }
            else
            {
                mButtonTakeReward.gameObject.CustomActive(false);
                mHasTakenReward.CustomActive(false);
                mButtonGo.CustomActive(true);
            }
            if(data.DoneNum <= data.TotalNum)
            {
                mTaskCount.text = string.Format("已完成{0}/{1}", data.DoneNum, data.TotalNum);
            }
            else
            {
                mTaskCount.text = string.Format("已完成{0}/{1}", data.TotalNum, data.TotalNum);
            }
        }

        public sealed override void Dispose()
        {
            base.Dispose();
           
            for (int i = mComItems.Count - 1; i >= 0; --i)
            {
                ComItemManager.Destroy(mComItems[i]);
            }
            mComItems.Clear();

            if (mButtonTakeReward != null)
            {
                mButtonTakeReward.SafeRemoveOnClickListener(_OnItemClick);
            }

            if (mButtonGo != null)
            {
                mButtonGo.SafeRemoveOnClickListener(OnGoBtnClick);
            }
        }
        
        void InitItems(List<OpTaskReward> awards)
        {
            if (awards == null || awards.Count == 0)
            {
                return;
            }

            for (int i = 0; i < awards.Count; ++i)
            {
                var comItem = ComItemManager.Create(mItemRoot.gameObject);
                if (comItem != null)
                {
                    ItemData data = ItemDataManager.CreateItemDataFromTable((int)awards[i].id);
                    data.Count = (int)awards[i].num;
                    comItem.Setup(data, Utility.OnItemClicked);
                    mComItems.Add(comItem);
                    (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                }

            }
        }


        private void OnGoBtnClick()
        {
            if (mData != null)
            {
                for (int i = 0; i < mData.ParamNums.Count; i++)
                {
                    if (mData.ParamNums[i] == (int)DungeonTable.eSubType.S_DEVILDDOM)
                    {
                        Utility.PathfindingYiJieMap();
                        return;
                    }

                    if (mData.ParamNums[i] == (int)DungeonTable.eSubType.S_WEEK_HELL ||
                        mData.ParamNums[i] == (int)DungeonTable.eSubType.S_WEEK_HELL_ENTRY ||
                        mData.ParamNums[i] == (int)DungeonTable.eSubType.S_WEEK_HELL_PER)
                    {
                        ChallengeUtility.OnOpenChallengeMapFrame(ProtoTable.DungeonModelTable.eType.WeekHellModel, 0);
                        if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
                        {
                            ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
                        }

                        return;
                    }
                }
            }

        }
    }
}
