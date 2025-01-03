using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class SummerVacationGrandTotalItem : ActivityItemBase
    {
        /// <summary>
        /// Item描述
        /// </summary>
        [SerializeField]
        private Text mItemDesTxt;
        /// <summary>
        /// Item数量
        /// </summary>
        [SerializeField]
        private Text mItemCountTxt;
        /// <summary>
        /// 完成任务领取奖励Btn
        /// </summary>
        [SerializeField]
        private Button mReciveBtn;
        /// <summary>
        /// 为完成任务Btn
        /// </summary>
        [SerializeField]
        private Button mUnFinishBtn;
        /// <summary>
        /// 已经领取奖励
        /// </summary>
        [SerializeField]
        private GameObject mHaveReciveGo;
        /// <summary>
        /// 在其他角色领取
        /// </summary>
        [SerializeField]
        private GameObject mOtherReceiveGo;

        [SerializeField]
        private GameObject mRewardRoot;

        /// <summary>
        /// 任务奖励的集合
        /// </summary>
        private List<ComItem> mComItems = new List<ComItem>();
        [SerializeField]
        private ScrollRect mAwardsScrollRect;
        /// <summary>
        /// 超过多少时才能滑动
        /// </summary>
        [SerializeField]
        private int mScrollCount = 5;
        [SerializeField]
        Vector2 mComItemSize = new Vector2(100f, 100f);


        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (mItemDesTxt != null)
            {
                mItemDesTxt.text = data.Desc;
            }
            if (mItemCountTxt != null)
            {
                mItemCountTxt.text = string.Format(TR.Value("limitactivity_shuqigrandtotal_accounttip"), data.DoneNum, data.TotalNum);
            }
            if (mReciveBtn != null)
            {
                mReciveBtn.SafeAddOnClickListener(_OnItemClick);
            }
            if (data.AwardDataList != null)
            {
                for (int i = 0; i < data.AwardDataList.Count; i++)
                {
                    if (mRewardRoot != null)
                    {
                        var comItem = ComItemManager.Create(mRewardRoot.gameObject);
                        if (comItem != null)
                        {
                            ItemData item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[i].id);
                            item.Count = (int)data.AwardDataList[i].num;
                            item.EquipType = (EEquipType)data.AwardDataList[i].equipType;
                            item.StrengthenLevel = data.AwardDataList[i].strenth;
                            comItem.Setup(item, Utility.OnItemClicked);
                            mComItems.Add(comItem);
                            (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                        }
                    }
                }
                mAwardsScrollRect.enabled = data.AwardDataList.Count > mScrollCount;

            }
        }
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
           switch(data.State)
            {
                case OpActTaskState.OATS_UNFINISH:
                    OnlyShowUnFinishTask();
                    break;
                case OpActTaskState.OATS_FINISHED:
                    OnlyShowRecive();
                    break;
                case OpActTaskState.OATS_OVER:
                    OnlyShowSelfHaveRecive();
                    break;
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            if (mReciveBtn != null)
            {
                mReciveBtn.SafeRemoveOnClickListener(_OnItemClick);
            }
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }

        }
        

        /// <summary>
        /// 显示完成任务 领取奖励
        /// </summary>
        private void OnlyShowRecive()
        {
           mReciveBtn.CustomActive(true);
      
           mUnFinishBtn.CustomActive(false);
           mHaveReciveGo.CustomActive(false);
           mOtherReceiveGo.CustomActive(false);
        }
        /// <summary>
        /// 显示未完成任务
        /// </summary>
        private void OnlyShowUnFinishTask()
        {
            mUnFinishBtn.CustomActive(true);

            mReciveBtn.CustomActive(false);
            mHaveReciveGo.CustomActive(false);
            mOtherReceiveGo.CustomActive(false);

        }

        /// <summary>
        /// 显示自己已领取奖励
        /// </summary>
        public void OnlyShowSelfHaveRecive()
        {
            mHaveReciveGo.CustomActive(true);

            mUnFinishBtn.CustomActive(false);
            mReciveBtn.CustomActive(false);
            mOtherReceiveGo.CustomActive(false);
        }
        /// <summary>
        /// 显示在其他角色领取奖励
        /// </summary>
        public void OnlyShowOtherHaveRecive()
        {
            mOtherReceiveGo.CustomActive(true);

            mHaveReciveGo.CustomActive(false);
            mUnFinishBtn.CustomActive(false);
            mReciveBtn.CustomActive(false);
           
        }
    }
}
