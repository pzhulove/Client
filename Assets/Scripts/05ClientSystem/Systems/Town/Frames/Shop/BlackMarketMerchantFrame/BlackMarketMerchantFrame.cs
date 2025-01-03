using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 黑市商人界面
    /// </summary>
    public class BlackMarketMerchantFrame : ClientFrame
    {
        #region ExtraUIBind
        private BlackMarketMerchantView mBlackMarketMerchantFrame = null;

        protected sealed override void _bindExUI()
        {
            mBlackMarketMerchantFrame = mBind.GetCom<BlackMarketMerchantView>("BlackMarketMerchantFrame");
        }

        protected sealed override void _unbindExUI()
        {
            mBlackMarketMerchantFrame = null;
        }
        #endregion

        BlackMarketMerchantDataModel mDataModel = null;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Shop/BlackMarketMerchantFrame/BlackMarketMerchantFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            RegisterEvent();

            BlackMarketMerchantDataManager.GetInstance().OnSendWorldBlackMarketAuctionListReq();
        }

        private void RegisterEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BlackMarketMerchanRetSuccess, OnBlackMarketMerchanRetSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BlackMarketMerchantItemUpdate, OnBlackMarketMerchantItemUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SyncBlackMarketMerchantNPCType, OnSyncBlackMarketMerchantNPCType);
        }

        private void UnRegisterEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BlackMarketMerchanRetSuccess, OnBlackMarketMerchanRetSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BlackMarketMerchantItemUpdate, OnBlackMarketMerchantItemUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SyncBlackMarketMerchantNPCType, OnSyncBlackMarketMerchantNPCType);
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            mDataModel = null;
            UnRegisterEvent();
        }

        private void OnBlackMarketMerchanRetSuccess(UIEvent uiEvent)
        {
            mDataModel = BlackMarketMerchantDataManager.GetInstance().GetBlackMarketMerchantDataModel();

            if (mDataModel != null)
            {
                mBlackMarketMerchantFrame.InitView(mDataModel, OnApplyTradDelegate, OnCancelApplyDelegate);
            }
        }

        void OnBlackMarketMerchantItemUpdate(UIEvent uiEvent)
        {
            mDataModel = BlackMarketMerchantDataManager.GetInstance().GetBlackMarketMerchantDataModel();

            if (mDataModel != null)
            {
                mBlackMarketMerchantFrame.RefreshItemInfoList(mDataModel);
            }
        }

        void OnSyncBlackMarketMerchantNPCType(UIEvent uiEvent)
        {
            if (BlackMarketMerchantDataManager.BlackMarketType == BlackMarketType.BmtInvalid)
            {
                Close();
            }
        }

        void OnApplyTradDelegate(ApplyTradData data)
        {
            if (data == null)
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<BlackMarketMerchantTradeFrame>(FrameLayer.Middle, data);
        }

        void OnCancelApplyDelegate(BlackMarketAuctionInfo info)
        {
            if (info == null)
            {
                return;
            }

            BlackMarketMerchantDataManager.GetInstance().OnSendWorldBlackMarketAuctionCancelReq(info.guid);
        }
        
    }
}
