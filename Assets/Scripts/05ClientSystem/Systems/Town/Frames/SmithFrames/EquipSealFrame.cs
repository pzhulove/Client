using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class EquipSealData
    {
        public ItemData item;
        public ItemData material;
    }

    public class EquipSealFrame : ClientFrame
    {
        #region ExtraUIBind
        private ComEquipSealView mEquipSealView = null;
        private ButtonEx mCloseBtn = null;

        protected override void _bindExUI()
        {
            mEquipSealView = mBind.GetCom<ComEquipSealView>("EquipSealView");
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mEquipSealView = null;
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
            return "UIFlatten/Prefabs/SmithShop/FunctionPrefab/EquipSealFrame";
        }

        protected override void _OnOpenFrame()
        {
            linkData = userData as SmithShopNewLinkData;
            if (mEquipSealView != null)
            {
                mEquipSealView.InitView(linkData);
            }
        }

        protected override void _OnCloseFrame()
        {
            linkData = null;
        }
    }
}