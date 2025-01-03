using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SinanNewFrame : ClientFrame
    {
        #region ExtraUIBind
        private SinanView mSinanView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mSinanView = mBind.GetCom<SinanView>("SinanView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mSinanView = null;
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
            return "UIFlatten/Prefabs/SmithShop/FunctionPrefab/SinanNewFrame";
        }

        protected override void _OnOpenFrame()
        {
            if (mSinanView != null)
                mSinanView.InitView();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }
    }
}