using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightEndVoteFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/Vote/TeamDuplicationFightEndVoteFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightEndVoteView != null)
            {
                mTeamDuplicationFightEndVoteView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightEndVoteView mTeamDuplicationFightEndVoteView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightEndVoteView = mBind.GetCom<TeamDuplicationFightEndVoteView>("TeamDuplicationFightEndVoteView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightEndVoteView = null;
        }
        #endregion

    }
}
