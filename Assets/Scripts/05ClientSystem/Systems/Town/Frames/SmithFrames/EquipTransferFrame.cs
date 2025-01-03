using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EquipTransferFrame : ClientFrame
    {
        #region ExtraUIBind
        private EquipTransferView mEquipTransferView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mEquipTransferView = mBind.GetCom<EquipTransferView>("EquipTransferView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mEquipTransferView = null;
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

        private SmithShopNewLinkData linkData = null;

        public static void OpenLinkFrame(string parms)
        {
            ClientSystemManager.GetInstance().OpenFrame<EquipSealFrame>(FrameLayer.Middle);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FunctionPrefab/EquipTransferFrame";
        }

        protected override void _OnOpenFrame()
        {
            linkData = userData as SmithShopNewLinkData;
            if (mEquipTransferView != null)
                mEquipTransferView.InitView(linkData);
        }

        protected override void _OnCloseFrame()
        {
            linkData = null;
        }
    }
}