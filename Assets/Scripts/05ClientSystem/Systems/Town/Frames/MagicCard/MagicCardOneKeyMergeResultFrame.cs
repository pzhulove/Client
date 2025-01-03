using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MagicCardOneKeyMergeResultFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MagicCard/MagicCardOneKeyMergeResultFrame";
        }


        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mMagicCardOneKeyMergeResultView != null)
                mMagicCardOneKeyMergeResultView.InitData();
        }

        #region ExtraUIBind
        private MagicCardOneKeyMergeResultView mMagicCardOneKeyMergeResultView = null;

        protected override void _bindExUI()
        {
            mMagicCardOneKeyMergeResultView = mBind.GetCom<MagicCardOneKeyMergeResultView>("MagicCardOneKeyMergeResultView");
        }

        protected override void _unbindExUI()
        {
            mMagicCardOneKeyMergeResultView = null;
        }
        #endregion
        

    }
}
