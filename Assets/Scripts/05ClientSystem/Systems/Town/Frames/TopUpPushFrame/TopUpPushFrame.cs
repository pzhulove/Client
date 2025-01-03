using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using ProtoTable;

namespace GameClient
{

    public class TopUpPushFrame : ClientFrame
    {
        string mPrefabPath = "UIFlatten/Prefabs/TopUpPushFrame/TopUpPushFrame";

        #region ExtraUIBind
        private TopUpPushView mTopUpPushView = null;

        protected sealed override void _bindExUI()
        {
            mTopUpPushView = mBind.GetCom<TopUpPushView>("TopUpPushView");
        }

        protected sealed override void _unbindExUI()
        {
            mTopUpPushView = null;
        }
        #endregion


        public sealed override string GetPrefabPath()
        {
            return mPrefabPath;
        }

        protected sealed override void _OnOpenFrame()
        {
            TopUpPushDataModel mTopUpPushDataModel = TopUpPushDataManager.GetInstance().GetTopUpPushDataModel();

            if (mTopUpPushView != null)
            {
                mTopUpPushView.InitView(mTopUpPushDataModel, OnBuyClick);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TopUpPushBuySuccess, OnTopUpPushBuySuccess);
        }

        protected sealed override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TopUpPushBuySuccess, OnTopUpPushBuySuccess);

            FollowingOpenQueueManager.GetInstance().NotifyCurrentOrderClosed();
        }
        
        private void OnTopUpPushBuySuccess(UIEvent uiEvent)
        {
            TopUpPushDataModel mTopUpPushDataModel = TopUpPushDataManager.GetInstance().GetTopUpPushDataModel();
            if (mTopUpPushView != null)
            {
                mTopUpPushView.RefreshItems(mTopUpPushDataModel);
            }
        }

        /// <summary>
        /// 购买回调
        /// </summary>
        /// <param name="pushId"></param>
        private void OnBuyClick(TopUpPushItemData pushItemData)
        {
            if (pushItemData == null)
            {
                Logger.LogErrorFormat("选中购买的道具数据为Null");
                return;
            }

            //剩余购买次数
            int remainNum = pushItemData.maxTimes - pushItemData.buyTimes;
            if (remainNum <= 0)
            {
                return;
            }

            int mMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            int mCount = pushItemData.disCountPrice;

            CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = mMoneyID, nCount = mCount }, ()=> 
            {
                TopUpPushDataManager.GetInstance().OnSendWorldBuyRechargePushItemsReq(pushItemData.pushId);
            });
        }
    }
}
