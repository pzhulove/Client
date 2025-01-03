using Protocol;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ReturnExchangeActivityView : LimitTimeActivityViewCommon
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
            mTextCoinCount.text = AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_COUNTER_ENERGY_COIN).ToString();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
	        if (mModel == null)
		        return;
            mTextCoinCount.text = AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_COUNTER_ENERGY_COIN).ToString();
        }
	}
}
