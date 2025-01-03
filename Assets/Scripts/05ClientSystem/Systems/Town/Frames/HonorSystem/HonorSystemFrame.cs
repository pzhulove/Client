using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    
    public class HonorSystemFrame : ClientFrame
    {
        
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/HonorSystem/HonorSystemFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();        
            
            if(mHonorSystemView != null)
                mHonorSystemView.InitView();

        }

        protected override void _OnCloseFrame()
        {
        }

        #region ExtraUIBind
        private HonorSystemView mHonorSystemView = null;

        protected override void _bindExUI()
        {
            mHonorSystemView = mBind.GetCom<HonorSystemView>("HonorSystemView");
        }

        protected override void _unbindExUI()
        {
            mHonorSystemView = null;
        }
        #endregion
        
    }

}
