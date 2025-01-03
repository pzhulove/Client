using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class PersonalSettingFrame : ClientFrame
    {
        #region ExtraUIBind
        private PersonalSettingView mPersonalSettingView = null;

        protected sealed override void _bindExUI()
        {
            mPersonalSettingView = mBind.GetCom<PersonalSettingView>("PersonalSettingView");
        }

        protected sealed override void _unbindExUI()
        {
            mPersonalSettingView = null;
        }
        #endregion

        public static int mDefalutTabIndex = 0;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/PersonalSettingFrame/PersonalSettingFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                mDefalutTabIndex = (int)userData;
            }
            mPersonalSettingView.InitView();
        }

        protected sealed override void _OnCloseFrame()
        {
            mDefalutTabIndex = 0;
        }
    }
}

