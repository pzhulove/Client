using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationTeamInviteListFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Build/TeamDuplicationTeamInviteListFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationTeamInviteListView != null)
            {
                mTeamDuplicationTeamInviteListView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationTeamInviteListView mTeamDuplicationTeamInviteListView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationTeamInviteListView = mBind.GetCom<TeamDuplicationTeamInviteListView>("TeamDuplicationTeamInviteListView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationTeamInviteListView = null;
        }
        #endregion
    }
}
