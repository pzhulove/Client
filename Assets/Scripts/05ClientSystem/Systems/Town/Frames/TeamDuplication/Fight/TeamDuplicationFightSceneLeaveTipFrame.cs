using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightSceneLeaveTipFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/TeamDuplicationFightSceneLeaveTipFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightSceneLeaveTipView != null)
            {
                mTeamDuplicationFightSceneLeaveTipView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightSceneLeaveTipView mTeamDuplicationFightSceneLeaveTipView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightSceneLeaveTipView = mBind.GetCom<TeamDuplicationFightSceneLeaveTipView>("TeamDuplicationFightSceneLeaveTipView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightSceneLeaveTipView = null;
        }
        #endregion

    }
}
