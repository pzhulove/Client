using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChijiHelpFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiHelpFrame";
        }

        protected override void _OnOpenFrame()
        {
            
        }

        protected override void _OnCloseFrame()
        {
            
        }

        #region ExtraUIBind
        private Button mClose = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            Close();
        }
        #endregion
    }
}
