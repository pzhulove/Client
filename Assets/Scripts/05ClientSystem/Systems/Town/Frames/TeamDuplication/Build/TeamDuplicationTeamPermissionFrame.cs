using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationTeamPermissionFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Build/TeamDuplicationTeamPermissionFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationTeamPermissionView != null)
            {
                var permissionDataModel = userData as TeamDuplicationPermissionDataModel;
                mTeamDuplicationTeamPermissionView.Init(permissionDataModel);
            }
        }

        #region ExtraUIBind
        private TeamDuplicationTeamPermissionView mTeamDuplicationTeamPermissionView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationTeamPermissionView = mBind.GetCom<TeamDuplicationTeamPermissionView>("TeamDuplicationTeamPermissionView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationTeamPermissionView = null;
        }
        #endregion

    }
}
