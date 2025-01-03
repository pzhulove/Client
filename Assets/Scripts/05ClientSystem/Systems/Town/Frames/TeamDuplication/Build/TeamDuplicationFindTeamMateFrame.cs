using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFindTeamMateFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Build/TeamDuplicationFindTeamMateFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFindTeamMateView != null)
            {
                mTeamDuplicationFindTeamMateView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFindTeamMateView mTeamDuplicationFindTeamMateView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFindTeamMateView = mBind.GetCom<TeamDuplicationFindTeamMateView>("TeamDuplicationFindTeamMateView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFindTeamMateView = null;
        }
        #endregion

    }
}
