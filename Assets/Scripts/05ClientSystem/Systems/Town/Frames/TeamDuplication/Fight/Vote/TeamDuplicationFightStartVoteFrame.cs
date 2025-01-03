using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightStartVoteFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/Vote/TeamDuplicationFightStartVoteFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightStartVoteView != null)
            {
                mTeamDuplicationFightStartVoteView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightStartVoteView mTeamDuplicationFightStartVoteView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightStartVoteView = mBind.GetCom<TeamDuplicationFightStartVoteView>("TeamDuplicationFightStartVoteView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightStartVoteView = null;
        }
        #endregion
        
    }
}
