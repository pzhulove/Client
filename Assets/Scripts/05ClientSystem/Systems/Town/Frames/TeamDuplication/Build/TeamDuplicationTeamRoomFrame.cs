using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationTeamRoomFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Build/TeamDuplicationTeamRoomFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationTeamRoomView != null)
            {
                var teamId = (int) userData;
                mTeamDuplicationTeamRoomView.Init(teamId);
            }
        }

        #region ExtraUIBind
        private TeamDuplicationTeamRoomView mTeamDuplicationTeamRoomView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationTeamRoomView = mBind.GetCom<TeamDuplicationTeamRoomView>("TeamDuplicationTeamRoomView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationTeamRoomView = null;
        }
        #endregion

    }
}
