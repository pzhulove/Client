using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class InscriptionComposeFrame : ClientFrame
    {
        #region ExtraUIBind
        private InscriptionComposeFrameView mInscriptionComposeFrameView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mInscriptionComposeFrameView = mBind.GetCom<InscriptionComposeFrameView>("InscriptionComposeFrameView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mInscriptionComposeFrameView = null;
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
            return "UIFlatten/Prefabs/SmithShop/InscriptionFrame/InscriptionComposeFrame";
        }

        protected override void _OnOpenFrame()
        {
            if (mInscriptionComposeFrameView != null)
                mInscriptionComposeFrameView.InitView();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }
    }
}