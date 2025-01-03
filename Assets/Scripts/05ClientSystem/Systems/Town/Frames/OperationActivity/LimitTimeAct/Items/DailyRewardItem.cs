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
    public interface IDailyRewardGiftItem : IDisposable
    {
        void Init(List<DailyRewardDetailData> giftList);
    }

    public class DailyRewardDetailData
    {
        public ItemSimpleData mSimpleData { get; private set; }
        public string Desc { get; private set; }

        public DailyRewardDetailData(GiftSyncInfo info)
        {
            mSimpleData = new ItemSimpleData((int)info.itemId, (int)info.itemNum);
            string description = TR.Value("activity_summer_watermelon_deatil_description").Replace("\\n", "\n");
            if (info.validLevels.Length != 2)
            {
                Logger.LogError("等级配置不正确!");
                return;
            }
            int dataMin = Mathf.Min((int)info.validLevels[0], (int)info.validLevels[1]);
            int dataMax = Mathf.Max((int)info.validLevels[0], (int)info.validLevels[1]);
            Desc = string.Format(description, dataMin, dataMax);
        }
    }

    public class DailyRewardItem : ActivityItemBase
    {
        [SerializeField] private Text mTextDescription;
        [SerializeField] private ComItemNew mMainItem;
        [SerializeField] private ComUIListScript mRewardDetailListScript;
        [SerializeField] Button mButtonTakeReward;
        [SerializeField] GameObject mHasTakenReward;
        [SerializeField] private Text mAccountNumTxt;
        private uint mGiftId;
        private List<List<DailyRewardDetailData>> mGiftDataList = new List<List<DailyRewardDetailData>>();
        private List<int> itemIdList = new List<int>();
        private List<int> itemNumList = new List<int>();
        private ILimitTimeActivityTaskDataModel mData;

        protected sealed override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
	        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftPackInfoResp);
            mButtonTakeReward.SafeAddOnClickListener(_OnItemClick);
            mButtonTakeReward.SafeAddOnClickListener(_OnSendMsg);
            base.RegisterAccountData(_OnActivityCounterUpdate);
            if (data == null)
            {
                Logger.LogError("data is null");
                return;
            }
            if (null != mRewardDetailListScript)
            {
                mRewardDetailListScript.Initialize();
                mRewardDetailListScript.onItemVisiable = (item) =>
                {
                    if (null == item)
                        return;
                    var script = item.GetComponent<ComItemNew>();
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemIdList[item.m_index]);
                    itemData.Count = itemNumList[item.m_index];
                    script.Setup(itemData, null, true);
                };
            }
            mData = data;
            mTextDescription.SafeSetText(TR.Value("activity_daily_reward_cur_level_desp", PlayerBaseData.GetInstance().Level));
            mAccountNumTxt.CustomActive(false);
            UpdateData(data);
            _InitItems(data.AwardDataList);
            base.OnRequsetAccountData(mData);
        }

        public sealed override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (data.State != OpActTaskState.OATS_FINISHED)
            {
                if (mButtonTakeReward != null)
                {
                    mButtonTakeReward.CustomActive(false);
                }

                mHasTakenReward.CustomActive(true);
            }
            else
            {
                if (mButtonTakeReward != null)
                {
                    mButtonTakeReward.CustomActive(true);
                }
                mHasTakenReward.CustomActive(false);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
	        mGiftDataList.Clear();
			mGiftId = 0;
            mButtonTakeReward.SafeRemoveOnClickListener(_OnItemClick);
            mButtonTakeReward.SafeRemoveOnClickListener(_OnSendMsg);
            mOnItemClick = null;
	        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftPackInfoResp);
            base.UnRegisterAccountData(_OnActivityCounterUpdate);
            //GameObject.Destroy(gameObject);
        }

        void _OnGetGiftPackInfoResp(UIEvent param)
        {
	        if (param == null || param.Param1 == null)
	        {
		        Logger.LogError("礼包数据为空");
		        return;
	        }
	        GiftPackSyncInfo data = param.Param1 as GiftPackSyncInfo;

            if (data.id == mGiftId)
            {
	            mGiftDataList.Clear();
				_InitAwardItemDetails(data.gifts);
            }
        }

        void _InitItems(List<OpTaskReward> awards)
        {
            if (awards == null || awards.Count == 0)
            {
                return;
            }
	        mGiftId = awards[0].id;
            ItemData data = ItemDataManager.CreateItemDataFromTable((int)mGiftId);
            mMainItem.Setup(data, null, true);
			GiftPackDataManager.GetInstance().GetGiftPackItem((int)awards[0].id);
        }

        void _InitAwardItemDetails(GiftSyncInfo[] gifts)
        {
            if (gifts == null || gifts.Length == 0)
            {
                return;
            }

            itemIdList.Clear();
            itemNumList.Clear();
            int curLevel = PlayerBaseData.GetInstance().Level;
            for (int i = 0; i < gifts.Length; ++i)
            {
                int dataMin = Mathf.Min((int)gifts[i].validLevels[0], (int)gifts[i].validLevels[1]);
                int dataMax = Mathf.Max((int)gifts[i].validLevels[0], (int)gifts[i].validLevels[1]);
                if (curLevel >= dataMin && curLevel <= dataMax)
                {
                    itemIdList.Add((int)gifts[i].itemId);
                    itemNumList.Add((int)gifts[i].itemNum);
                }
            }
            mRewardDetailListScript.SetElementAmount(itemIdList.Count);
        }

        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if (mData != null)
            {
                if ((uint)uiEvent.Param1 ==mData.DataId)
                {
                    ShowHaveUsedNumState();
                }
                
            }
        }
        private void _OnSendMsg()
        {
            if(mData!=null)
            {
                base.OnRequsetAccountData(mData);
            }
          
        }
        /// <summary>
        /// 显示账号次数
        /// </summary>
        private void ShowHaveUsedNumState()
        {
            if(mData!=null)
            {
                int accountDailySubmit= mData.AccountDailySubmitLimit;
                int accountTotalSubmit = mData.AccountTotalSubmitLimit;
                int totalNum = 0;
                if(accountDailySubmit>0)
                {
                    totalNum = accountDailySubmit;
                }
                if (accountTotalSubmit > 0)
                {
                    totalNum = accountTotalSubmit;
                }
                if (totalNum>0)
                {
                    int haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(mData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                    int leftNum = totalNum - haveNum;
                    if (leftNum < 0)
                    {
                        leftNum = 0;
                    }
                    if (mAccountNumTxt != null)
                    {
                        mAccountNumTxt.CustomActive(true);
                        mAccountNumTxt.SafeSetText(string.Format(TR.Value("limitactivity_dailyreward_tip", leftNum, totalNum)));
                    }
                    if (leftNum <= 0)
                    {
                        mButtonTakeReward.CustomActive(false);
                        mHasTakenReward.CustomActive(true);
                    }
                }
                else
                {
                    mAccountNumTxt.CustomActive(false);
                }
              

            }

        }
    }
}
