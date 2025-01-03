using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class CommonHelpNewFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/CommonHelpNew/CommonHelpNewFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mCommonHelpNewView != null)
            {
                var helpId = 0;
                if (userData != null)
                    helpId = (int) userData;

                mCommonHelpNewView.InitView(helpId);
            }
        }

        #region ExtraUIBind
        private CommonHelpNewView mCommonHelpNewView = null;

        protected override void _bindExUI()
        {
            mCommonHelpNewView = mBind.GetCom<CommonHelpNewView>("CommonHelpNewView");
        }

        protected override void _unbindExUI()
        {
            mCommonHelpNewView = null;
        }
        #endregion
        
    }

}
