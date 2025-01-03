using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Protocol;
using Network;
using System.Net;
using System.IO;
using System.Text;
using XUPorterJSON;
using ProtoTable;

namespace GameClient
{
    public class AuctionFreezeRemindFrame : ClientFrame
    {
        const string mPrefabPath = "UIFlatten/Prefabs/AuctionNew/FreezeRemind/AuctionFreezeRemind";

        /// <summary>
        /// 充值冻结金额倍率
        /// </summary>
        int mChargeUnFreezeMoneryRate = 0;
        /// <summary>
        /// 日常任务活跃度解冻需求值
        /// </summary>
        int mDailyTaskScoreUnFreezeRequirement = 0;
        /// <summary>
        /// 日常任务活跃度解冻天数
        /// </summary>
        int mDailyTaskScoreUnFreezeDays = 0;

        AuctionFreezeRemindData mAuctionFreezeRemindData = null;

        public sealed override string GetPrefabPath()
        {
            return mPrefabPath;
        }

        #region ExtraUIBind
        private Button mClose = null;
        private Text mText = null;
        private Text mTerminationOfBailText = null;
        private Text mExceptionTradHoursText = null;
        private Text mRemainingOnBailText = null;
        private Text mOne = null;
        private Text mForeverBlank = null;
        private Text mTwo = null;
        private Text mRuleOne = null;
        private Text mRuleTwo = null;
        private ScrollRect mScroll = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
            mText = mBind.GetCom<Text>("text");
            mTerminationOfBailText = mBind.GetCom<Text>("TerminationOfBailText");
            mExceptionTradHoursText = mBind.GetCom<Text>("ExceptionTradHoursText");
            mRemainingOnBailText = mBind.GetCom<Text>("RemainingOnBailText");
            mForeverBlank = mBind.GetCom<Text>("foreverBlank");
            mOne = mBind.GetCom<Text>("One");
            mTwo = mBind.GetCom<Text>("Two");
            mRuleOne = mBind.GetCom<Text>("RuleOne");
            mRuleTwo = mBind.GetCom<Text>("RuleTwo");
            mScroll = mBind.GetCom<ScrollRect>("scroll");
        }

        protected override void _unbindExUI()
        {
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
            mText = null;
            mTerminationOfBailText = null;
            mExceptionTradHoursText = null;
            mRemainingOnBailText = null;
            mRemainingOnBailText = null;
            mOne = null;
            mTwo = null;
            mRuleOne = null;
            mRuleTwo = null;
            mScroll = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }
        #endregion


        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                mAuctionFreezeRemindData = userData as AuctionFreezeRemindData;
            }
            _InitData();
            _Init(mAuctionFreezeRemindData);
        }

        protected sealed override void _OnCloseFrame()
        {
            mAuctionFreezeRemindData = null;
            mChargeUnFreezeMoneryRate = 0;
            mDailyTaskScoreUnFreezeRequirement = 0;
            mDailyTaskScoreUnFreezeDays = 0;
        }
        void _InitData()
        {
            var DespoitData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_CHARGE_UN_FREEZE_MONEY_RATE);
            mChargeUnFreezeMoneryRate = DespoitData.Value;
            var mSystemTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DAILY_TASK_SCORE_UN_FREEZE_REQUIREMENT);
            mDailyTaskScoreUnFreezeRequirement = mSystemTable.Value;
            var mSystemTable2 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DAILL_TASK_SCORE_UN_FREEZE_DAYS);
            mDailyTaskScoreUnFreezeDays = mSystemTable2.Value;
        }
        void _Init(AuctionFreezeRemindData data)
        {
            if (data == null)
            {
                return;
            }

            var isItemForeverFreeze = AuctionNewUtility.IsItemForeverFreeze((int)data.RemainDayNumber);

            mText.text = TR.Value("AuctionFreezeRemind_ExceptionText", data.FrozenAmount);

            var tradHourStr = TimeUtility.GetDateTimeByCommonType((int)data.AbnormalDealTimeStamp);
            mExceptionTradHoursText.text = TR.Value("AuctionFreezeRemind_ExceptionTextTradHours", tradHourStr);

            var ofBailHourStr = TimeUtility.GetDateTimeByCommonType((int)data.BailFinishedTimeStamp);
            mTerminationOfBailText.text = TR.Value("AuctionFreezeRemind_TerminationOfBail", ofBailHourStr);

            var onBailStr = TimeUtility.GetDayNumberStr((int)data.RemainDayNumber);
            mRemainingOnBailText.text = TR.Value("AuctionFreezeRemind_RemainingOnBail", onBailStr);

            mOne.text = TR.Value("AuctionFreezeRemind_One", data.BackAmount);
            mTwo.text = TR.Value("AuctionFreezeRemind_Two", data.BackDayNumber);
            mRuleOne.text = TR.Value("AuctionFreezeRemind_RuleOne", mChargeUnFreezeMoneryRate);
            mRuleTwo.text = TR.Value("AuctionFreezeRemind_RuleTwo", mDailyTaskScoreUnFreezeRequirement, mDailyTaskScoreUnFreezeDays);

            //永久冻结
            if (isItemForeverFreeze == true)
            {
                CommonUtility.UpdateTextVisible(mForeverBlank, true);

                CommonUtility.UpdateTextVisible(mTwo, false);
                CommonUtility.UpdateTextVisible(mRuleTwo, false);
            }
            else
            {
                CommonUtility.UpdateTextVisible(mForeverBlank, false);

                CommonUtility.UpdateTextVisible(mTwo, true);
                CommonUtility.UpdateTextVisible(mRuleTwo, true);
            }

        }

    }
}
