using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ChallengeDungeonRewardFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Challenge/ChallengeDungeonRewardFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            ChallengeDungeonRewardDataModel challengeDungeonDataModel = null;
            if (userData != null)
                challengeDungeonDataModel = userData as ChallengeDungeonRewardDataModel;

            if (mChallengeDungeonRewardView != null)
            {
                mChallengeDungeonRewardView.InitRewardView(challengeDungeonDataModel);
            }
        }

        #region ExtraUIBind
        private ChallengeDungeonRewardView mChallengeDungeonRewardView = null;

        protected override void _bindExUI()
        {
            mChallengeDungeonRewardView = mBind.GetCom<ChallengeDungeonRewardView>("ChallengeDungeonRewardView");
        }

        protected override void _unbindExUI()
        {
            mChallengeDungeonRewardView = null;
        }
        #endregion


    }

}
