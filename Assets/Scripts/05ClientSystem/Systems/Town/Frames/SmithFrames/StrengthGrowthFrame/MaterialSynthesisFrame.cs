using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MaterialSynthesisFrame : ClientFrame
    {
        #region ExtraUIBind
        private MaterialSynthesisView mMaterialsSynthesisView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mMaterialsSynthesisView = mBind.GetCom<MaterialSynthesisView>("MaterialsSynthesisView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mMaterialsSynthesisView = null;
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
            return "UIFlatten/Prefabs/SmithShop/StrengthenGrowth/MaterialsSynthesis";
        }

        protected override void _OnOpenFrame()
        {
            if (mMaterialsSynthesisView != null)
                mMaterialsSynthesisView.InitView();
        }

        protected override void _OnCloseFrame()
        {
           
        }
    }
}