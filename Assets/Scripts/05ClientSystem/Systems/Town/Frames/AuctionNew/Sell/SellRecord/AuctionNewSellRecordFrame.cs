using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AuctionNewSellRecordFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AuctionNew/AuctionNewSellRecordFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if(mAuctionNewSellRecordView != null)
                mAuctionNewSellRecordView.InitView();
        }

        #region ExtraUIBind
        private AuctionNewSellRecordView mAuctionNewSellRecordView = null;

        protected override void _bindExUI()
        {
            mAuctionNewSellRecordView = mBind.GetCom<AuctionNewSellRecordView>("AuctionNewSellRecordView");
        }

        protected override void _unbindExUI()
        {
            mAuctionNewSellRecordView = null;
        }
        #endregion

    }

}
