using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AuctionNewMagicCardStrengthenLevelFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AuctionNew/AuctionNewMagicCardStrengthenLevelFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            AuctionNewMagicCardDataModel dataModel = null;
            if(userData != null)
                dataModel = userData as AuctionNewMagicCardDataModel;

            if (mAuctionNewMagicCardStrengthenLevelView != null)
                mAuctionNewMagicCardStrengthenLevelView.Init(dataModel);
        }

        #region ExtraUIBind
        private AuctionNewMagicCardStrengthenLevelView mAuctionNewMagicCardStrengthenLevelView = null;

        protected override void _bindExUI()
        {
            mAuctionNewMagicCardStrengthenLevelView = mBind.GetCom<AuctionNewMagicCardStrengthenLevelView>("AuctionNewMagicCardStrengthenLevelView");
        }

        protected override void _unbindExUI()
        {
            mAuctionNewMagicCardStrengthenLevelView = null;
        }
        #endregion
        
    }

}
