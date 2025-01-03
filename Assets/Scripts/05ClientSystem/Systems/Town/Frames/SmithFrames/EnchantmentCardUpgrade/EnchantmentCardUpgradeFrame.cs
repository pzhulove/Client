using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EnchantmentCardUpgradeFrame : ClientFrame
    {
        #region ExtraUIBind
        private EnchantmentCardUpgradeView mEnchantmentCardUpgradeView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mEnchantmentCardUpgradeView = mBind.GetCom<EnchantmentCardUpgradeView>("EnchantmentCardUpgradeView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mEnchantmentCardUpgradeView = null;
            mCloseBtn.onClick.RemoveListener(_onCloseBtnButtonClick);
            mCloseBtn = null;
        }
        #endregion

        #region Callback
        private void _onCloseBtnButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/EnchantmentCardUpgrade/EnchantmentCardUpgradeFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(mEnchantmentCardUpgradeView != null)
            {
                mEnchantmentCardUpgradeView.InitView();
            }
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }
    }
}