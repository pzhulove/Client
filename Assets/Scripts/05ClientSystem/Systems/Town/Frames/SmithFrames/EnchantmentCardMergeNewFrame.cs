using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EnchantmentCardMergeNewFrame : ClientFrame
    {
        #region ExtraUIBind
        private MagicCardMergeView mEnchantmentCardMergeView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mEnchantmentCardMergeView = mBind.GetCom<MagicCardMergeView>("EnchantmentCardMergeView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mEnchantmentCardMergeView = null;
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
            return "UIFlatten/Prefabs/SmithShop/FunctionPrefab/EnchantmentCardMergeNewFrame";
        }

        protected override void _OnOpenFrame()
        {
            if (mEnchantmentCardMergeView != null)
                mEnchantmentCardMergeView.InitView();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }
    }
}