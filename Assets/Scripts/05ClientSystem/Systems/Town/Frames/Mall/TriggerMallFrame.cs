using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;

namespace GameClient
{
    public class TriggerMallFrame : ClientFrame
    {        
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mall/TriggerMallFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (mView != null)
            {
                mView.OnInit();
            }
            //购买完成后 刷新列表
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed,OnMallListUpdate);
        }

        protected sealed override void _OnCloseFrame()
        {
            if (mView != null)
            {
                mView.OnUninit();
            }
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed,OnMallListUpdate);
        }

        private void OnMallListUpdate(UIEvent uiEvent)
        {
            if (null != mView)
            {
                mView.OnUpdateMallList();
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

            int mMoneyID = 1;//ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            int mCount = pushItemData.disCountPrice;

            CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = mMoneyID, nCount = mCount }, ()=> 
            {
                TopUpPushDataManager.GetInstance().OnSendWorldBuyRechargePushItemsReq(pushItemData.pushId);
            });
        }

        #region ExtraUIBind
        private TriggerMallFrameView mView = null;

        protected sealed override void _bindExUI()
        {
            mView = mBind.GetCom<TriggerMallFrameView>("View");
        }

        protected sealed override void _unbindExUI()
        {
            mView = null;
        }
        #endregion
    }
}
