using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ChangeFashionSpecialExchangeView : LimitTimeActivityViewCommon
    {

        private ILimitTimeActivityModel mModel;
        private GameObject mThisItem;

		public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            //base.Init(model, onItemClick);
	        if (model == null)
		        return;
            mNote.Init(model);
            mModel = model;
            base.mOnItemClick = onItemClick;
			//mNote.ShowLogoText(false);
			//mTextCoinCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_fatigue_coin"), CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", model.Id, CounterKeys.CHANGE_FASHION_EXCHANGE_COUNT))));
            //mTextFatigueCost.SafeSetText(string.Format(TR.Value("activity_coin_exchange_fatigue_cost"), CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", model.Id, CounterKeys.CHANGE_FASHION_EXCHANGE_HAVE_USE_COUNT))));
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            _InitItems(model);
        }
        protected override sealed void _InitItems(ILimitTimeActivityModel data)
        {

            GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
            if (go == null)
            {
                Logger.LogError("加载预制体失败，路径:" + data.ItemPath);
                return;
            }

            if (go.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go);
                Logger.LogError("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + data.ItemPath);
                return;
            }
            mThisItem = go;
            mItems.Clear();

            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoot, false);
            item.GetComponent<IActivityCommonItem>().InitFromMode( data, mOnItemClick);
            mItems.Add(data.TaskDatas[0].DataId, item.GetComponent<IActivityCommonItem>());
            Destroy(go);
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
	        //mTextCoinCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_fatigue_coin"), CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", mModel.Id, CounterKeys.CHANGE_FASHION_EXCHANGE_COUNT))));
	        //mTextFatigueCost.SafeSetText(string.Format(TR.Value("activity_coin_exchange_fatigue_cost"), CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", mModel.Id, CounterKeys.CHANGE_FASHION_EXCHANGE_HAVE_USE_COUNT))));
        }
        public override void UpdateData(ILimitTimeActivityModel data)
        {
            mModel = data;
            for(int i = 0;i<data.TaskDatas.Count;i++)
            {
                //mThisItem.GetComponent<IActivityCommonItem>().UpdateData(data.TaskDatas[i]);
                //var tempItem = mThisItem.GetComponent<IActivityCommonItem>();
                //tempItem.UpdateData(data.TaskDatas[i]);
                var tempItem2 = mItemRoot.GetComponentInChildren<IActivityCommonItem>();
                tempItem2.UpdateData(data.TaskDatas[i]);
            }
        }
    }
}
