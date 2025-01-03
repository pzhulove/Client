using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ComControlFrame : ClientFrame
    {
        
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ComControl/ComControlFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if(mComControlView != null)
                mComControlView.InitView();
           
        }

        protected override void _OnCloseFrame()
        {

        }

        #region ExtraUIBind
        private ComControlView mComControlView = null;

        protected override void _bindExUI()
        {
            mComControlView = mBind.GetCom<ComControlView>("ComControlView");
        }

        protected override void _unbindExUI()
        {
            mComControlView = null;
        }
        #endregion
        
    }

}
