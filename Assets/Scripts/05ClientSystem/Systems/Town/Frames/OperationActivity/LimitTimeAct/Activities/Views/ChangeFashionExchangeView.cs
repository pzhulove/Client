using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ChangeFashionExchangeView : LimitTimeActivityViewCommon
    {
        [SerializeField] private Text mTextFatigueCost;
        [SerializeField] private Text mTextCoinCount;
        [SerializeField] private Button mGetScoreBtn;
        [SerializeField] private int mItemTableId;


	    private ILimitTimeActivityModel mModel;
        private string countKey = null;
        private string countCostKey = null;
        public delegate void GetScoreCallBack(int id);
        private GetScoreCallBack mGetScoreCallBack;
        public void setGetScoreCallBack(GetScoreCallBack callBack)
        {
            mGetScoreCallBack = callBack;
        }
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
	        if (model == null)
		        return;
	        mModel = model;
            var countStr = model.CountParam.Split('|');
            if (countStr.Length != 2)
            {
                mTextFatigueCost.CustomActive(false);
                mTextCoinCount.CustomActive(false);
            }
            else
            {
                mTextFatigueCost.CustomActive(true);
                mTextCoinCount.CustomActive(true);
                countKey = countStr[0];
                countCostKey = countStr[1];
            }
            //mNote.ShowLogoText(false);
            if(countKey != null && countCostKey != null)
            {
                mTextCoinCount.SafeSetText(string.Format(TR.Value("activity_change_fashion_count"), CountDataManager.GetInstance().GetCount(countKey)));
                mTextFatigueCost.SafeSetText(string.Format(TR.Value("activity_change_fashion_cost"), CountDataManager.GetInstance().GetCount(countCostKey)));
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            mGetScoreBtn.SafeAddOnClickListener(() =>
            {
                //ItemComeLink.OnLink(204, 0);
                if(mGetScoreCallBack != null)
                {
                    mGetScoreCallBack(mItemTableId);
                }
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            mGetScoreBtn.SafeRemoveAllListener();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
	        if (mModel == null)
		        return;
            if (countKey != null && countCostKey != null)
            {
                mTextCoinCount.SafeSetText(string.Format(TR.Value("activity_change_fashion_count"), CountDataManager.GetInstance().GetCount(countKey)));
                mTextFatigueCost.SafeSetText(string.Format(TR.Value("activity_change_fashion_cost"), CountDataManager.GetInstance().GetCount(countCostKey)));
            }
        }
	}
}
