using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MagicCardMergeData
    {
        public Action MagicCardMergeAction;
    }

    public class MagicCardOneKeyMergeTipFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MagicCard/MagicCardOneKeyMergeTipFrame";
        }


        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            var magicCardMergeData = userData as MagicCardMergeData;

            if (mMagicCardOneKeyMergeTipView != null)
                mMagicCardOneKeyMergeTipView.InitData(magicCardMergeData);

        }

        #region ExtraUIBind
        private MagicCardOneKeyMergeTipView mMagicCardOneKeyMergeTipView = null;

        protected override void _bindExUI()
        {
            mMagicCardOneKeyMergeTipView = mBind.GetCom<MagicCardOneKeyMergeTipView>("MagicCardOneKeyMergeTipView");
        }

        protected override void _unbindExUI()
        {
            mMagicCardOneKeyMergeTipView = null;
        }
        #endregion
        
    }
}
