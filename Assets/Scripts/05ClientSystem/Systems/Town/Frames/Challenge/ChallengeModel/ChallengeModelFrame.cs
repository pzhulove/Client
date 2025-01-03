using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ChallengeModelFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Challenge/ChallengeModelFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mChallengeModelView != null)
            {
                mChallengeModelView.InitView();
            }
        }

        #region ExtraUIBind
        private ChallengeModelView mChallengeModelView = null;

        protected override void _bindExUI()
        {
            mChallengeModelView = mBind.GetCom<ChallengeModelView>("ChallengeModelView");
        }

        protected override void _unbindExUI()
        {
            mChallengeModelView = null;
        }
        #endregion
        
    }

}
