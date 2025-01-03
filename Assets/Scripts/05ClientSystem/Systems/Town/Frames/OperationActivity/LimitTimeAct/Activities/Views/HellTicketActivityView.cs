using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class HellTicketActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField] private Text mTextLotteryCount;
        [SerializeField] private Button mButtonLottery;
        [SerializeField] private Button mButtonPreviewAwards;

        public UnityAction OnLotteryClick { set { mButtonLottery.SafeRemoveAllListener(); mButtonLottery.SafeAddOnClickListener(value); } }
        public UnityAction OnPreviewAwardsClick { set { mButtonPreviewAwards.SafeRemoveAllListener(); mButtonPreviewAwards.SafeAddOnClickListener(value); } }

        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
            mTextLotteryCount.SafeSetText(string.Format(TR.Value("activity_hell_ticket_lottery_count"), CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_TIME)));
        }

        public void UpdateLotteryCount()
        {
            mTextLotteryCount.SafeSetText(string.Format(TR.Value("activity_hell_ticket_lottery_count"), CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_TIME)));
        }
    }
}
