using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;


namespace GameClient
{
    public class FriendLinessSpecificationFrame : ClientFrame
    {
        #region ExtraUIBind
        private Text mText = null;
        private Button mClose = null;

        protected override void _bindExUI()
        {
            mText = mBind.GetCom<Text>("text");
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
        }

        protected override void _unbindExUI()
        {
            mText = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
        }
        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RelationFrame/FriendLinessSpecificationFrame";
        }
        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }
    }
}

