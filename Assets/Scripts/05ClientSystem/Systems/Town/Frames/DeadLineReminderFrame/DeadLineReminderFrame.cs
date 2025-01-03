using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class DeadLineReminderFrame : ClientFrame
    {
        #region ExtraUIBind
        private DeadLineReminderView mView = null;
        private Button mBtClose = null;

        protected override void _bindExUI()
        {
            mView = mBind.GetCom<DeadLineReminderView>("View");
            mBtClose = mBind.GetCom<Button>("btClose");
            if (null != mBtClose)
            {
                mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            }
        }

        protected override void _unbindExUI()
        {
            mView = null;
            if (null != mBtClose)
            {
                mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            }
            mBtClose = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            Close();
        }
        #endregion
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/DeadLineReminderFrame/DeadLineReminderFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (mView != null)
            {
                mView.InitView();
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            
        }
    }
}