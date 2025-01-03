using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class CoinExchangeActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField] private Text mTextFatigueCost;
        [SerializeField] private Text mTextCoinCount;

	    private ILimitTimeActivityModel mModel;

		public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
	        if (model == null)
		        return;
	        mModel = model;
			//mNote.ShowLogoText(false);
			mTextCoinCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_fatigue_coin"), CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", model.Id, CounterKeys.COUNTER_ACTIVITY_FATIGUE_COIN_NUM))));
            mTextFatigueCost.SafeSetText(string.Format(TR.Value("activity_coin_exchange_fatigue_cost"), CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", model.Id, CounterKeys.COUNTER_ACTIVITY_FATIGUE_TICKET_NUM))));
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
	        if (mModel == null)
		        return;
	        mTextCoinCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_fatigue_coin"), CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", mModel.Id, CounterKeys.COUNTER_ACTIVITY_FATIGUE_COIN_NUM))));
	        mTextFatigueCost.SafeSetText(string.Format(TR.Value("activity_coin_exchange_fatigue_cost"), CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", mModel.Id, CounterKeys.COUNTER_ACTIVITY_FATIGUE_TICKET_NUM))));
        }
	}
}
