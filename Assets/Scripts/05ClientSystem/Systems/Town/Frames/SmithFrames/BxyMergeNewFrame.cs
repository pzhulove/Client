using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BxyMergeNewFrame : ClientFrame
    {
        #region ExtraUIBind
        private BxyMergeView mBxyMergeView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mBxyMergeView = mBind.GetCom<BxyMergeView>("BxyMergeView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mBxyMergeView = null;
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
            return "UIFlatten/Prefabs/SmithShop/FunctionPrefab/BxyMergeNewFrame";
        }

        protected override void _OnOpenFrame()
        {
            if (mBxyMergeView != null)
                mBxyMergeView.InitView();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }
    }
}