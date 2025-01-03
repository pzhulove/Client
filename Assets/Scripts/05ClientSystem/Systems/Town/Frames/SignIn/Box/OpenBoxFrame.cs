using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class OpenBoxFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Box/OpenBoxFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mOpenBoxView != null)
            {
                var boxData = (BoxDataModel) userData;
                mOpenBoxView.InitView(boxData);
            }
        }

        #region ExtraUIBind
        private OpenBoxView mOpenBoxView = null;

        protected override void _bindExUI()
        {
            mOpenBoxView = mBind.GetCom<OpenBoxView>("OpenBoxView");
        }

        protected override void _unbindExUI()
        {
            mOpenBoxView = null;
        }
        #endregion
        
    }

}
