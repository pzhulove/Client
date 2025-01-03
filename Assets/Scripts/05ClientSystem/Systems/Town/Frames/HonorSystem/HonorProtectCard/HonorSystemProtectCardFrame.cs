using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorSystemProtectCardFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/HonorSystem/HonorSystemProtectCardFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mHonorSystemProtectCardView != null)
                mHonorSystemProtectCardView.InitView();

        }

        protected override void _OnCloseFrame()
        {
        }

        #region ExtraUIBind
        private HonorSystemProtectCardView mHonorSystemProtectCardView = null;

        protected override void _bindExUI()
        {
            mHonorSystemProtectCardView = mBind.GetCom<HonorSystemProtectCardView>("HonorSystemProtectCardView");
        }

        protected override void _unbindExUI()
        {
            mHonorSystemProtectCardView = null;
        }
        #endregion

    }

}
