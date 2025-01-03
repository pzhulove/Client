using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AuctionNewMsgBoxOkCancelFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/CommonSystemNotify/AuctionNewMsgBoxOkCancelFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            var commonMsgBoxOkCancelParamData = userData as CommonMsgBoxOkCancelNewParamData;

            if (mAuctionNewMsgBoxOkCancelView != null)
                mAuctionNewMsgBoxOkCancelView.InitData(commonMsgBoxOkCancelParamData);

        }

        #region ExtraUIBind
        private AuctionNewMsgBoxOkCancelView mAuctionNewMsgBoxOkCancelView = null;

        protected override void _bindExUI()
        {
            mAuctionNewMsgBoxOkCancelView = mBind.GetCom<AuctionNewMsgBoxOkCancelView>("AuctionNewMsgBoxOkCancelView");
        }

        protected override void _unbindExUI()
        {
            mAuctionNewMsgBoxOkCancelView = null;
        }
        #endregion
    }
}
