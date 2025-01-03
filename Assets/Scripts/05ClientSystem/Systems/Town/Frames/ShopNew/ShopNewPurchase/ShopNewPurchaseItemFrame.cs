using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ShopNewPurchaseItemFrame : ClientFrame
    {
     

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ShopNew/ShopNewPurchaseItemFrame";
        }
        
        protected override void _OnOpenFrame()
        {
           
            base._OnOpenFrame();

            var shopNewShopItemTable = userData as ShopNewShopItemInfo;

            if (mShopNewPurchaseItemView != null)
                mShopNewPurchaseItemView.InitShop(shopNewShopItemTable);
        }

        #region ExtraUIBind
        private ShopNewPurchaseItemView mShopNewPurchaseItemView = null;

        protected override void _bindExUI()
        {
            mShopNewPurchaseItemView = mBind.GetCom<ShopNewPurchaseItemView>("ShopNewPurchaseItemView");
        }

        protected override void _unbindExUI()
        {
            mShopNewPurchaseItemView = null;
        }
        #endregion
        
    }

}
