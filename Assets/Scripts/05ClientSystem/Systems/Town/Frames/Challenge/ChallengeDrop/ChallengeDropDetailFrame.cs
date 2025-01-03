using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ChallengeDropDetailFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Challenge/ChallengeDropDetailFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mChallengeDropDetailView != null)
            {
                var dungeonId = (int) userData;
                mChallengeDropDetailView.InitView(dungeonId);
            }
        }

        #region ExtraUIBind
        private ChallengeDropDetailView mChallengeDropDetailView = null;

        protected override void _bindExUI()
        {
            mChallengeDropDetailView = mBind.GetCom<ChallengeDropDetailView>("ChallengeDropDetailView");
        }

        protected override void _unbindExUI()
        {
            mChallengeDropDetailView = null;
        }
        #endregion
        
    }

}
