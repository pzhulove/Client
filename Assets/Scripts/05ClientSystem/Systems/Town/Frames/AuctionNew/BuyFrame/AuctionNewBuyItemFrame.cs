using System;
using Protocol;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine;
using Scripts.UI;
using System.Collections.Generic;

namespace GameClient
{

    class AuctionNewBuyItemFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AuctionNew/AuctionNewBuyItemFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            var buyItemDataModel = userData as AuctionNewBuyItemDataModel;
            
            if (mAuctionNewBuyItemView != null)
                mAuctionNewBuyItemView.Init(buyItemDataModel);
        }

        protected sealed override void _OnCloseFrame()
        {

        }

        #region ExtraUIBind
        private AuctionNewBuyItemView mAuctionNewBuyItemView = null;

        protected override void _bindExUI()
        {
            mAuctionNewBuyItemView = mBind.GetCom<AuctionNewBuyItemView>("AuctionNewBuyItemView");
        }

        protected override void _unbindExUI()
        {
            mAuctionNewBuyItemView = null;
        }
        #endregion
        
    }
}

