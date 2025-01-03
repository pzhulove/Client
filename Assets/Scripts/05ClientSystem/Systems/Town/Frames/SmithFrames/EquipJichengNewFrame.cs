using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EquipJichengNewFrame : ClientFrame
    {
        #region ExtraUIBind
        private EquipJichengView mEquipJichengView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mEquipJichengView = mBind.GetCom<EquipJichengView>("EquipJichengView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mEquipJichengView = null;
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
            return "UIFlatten/Prefabs/SmithShop/FunctionPrefab/EquipJichengNewFrame";
        }

        protected override void _OnOpenFrame()
        {
            if (mEquipJichengView != null)
                mEquipJichengView.InitView();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }
    }
}