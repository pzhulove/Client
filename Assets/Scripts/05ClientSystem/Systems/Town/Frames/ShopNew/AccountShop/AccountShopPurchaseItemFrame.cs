using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AccountShopPurchaseItemFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ShopNew/AccountShopPurchaseItemFrame";
        }
        
        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            var accShopItemInfo = userData as AccountShopPurchaseItemInfo;

            if (mAccShopPurchaseItemView != null)
                mAccShopPurchaseItemView.InitShop(accShopItemInfo);
        }

        #region ExtraUIBind
        private AccountShopPurchaseItemView mAccShopPurchaseItemView = null;

        protected override void _bindExUI()
        {
            mAccShopPurchaseItemView = mBind.GetCom<AccountShopPurchaseItemView>("AccShopPurchaseItemView");
        }

        protected override void _unbindExUI()
        {
            mAccShopPurchaseItemView = null;
        }
        #endregion
        
    }

}
