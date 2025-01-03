using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorSystemPreviewFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/HonorSystem/HonorSystemPreviewFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mHonorSystemPreviewView != null)
                mHonorSystemPreviewView.InitView();

        }

        protected override void _OnCloseFrame()
        {
        }

        #region ExtraUIBind
        private HonorSystemPreviewView mHonorSystemPreviewView = null;

        protected override void _bindExUI()
        {
            mHonorSystemPreviewView = mBind.GetCom<HonorSystemPreviewView>("HonorSystemPreviewView");
        }

        protected override void _unbindExUI()
        {
            mHonorSystemPreviewView = null;
        }
        #endregion
        
    }

}
