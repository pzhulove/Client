using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationTeamListFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Build/TeamDuplicationTeamListFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationTeamListView != null)
                mTeamDuplicationTeamListView.Init();
        }

        protected override void _OnCloseFrame()
        {
            base._OnOpenFrame();

            TeamDuplicationDataManager.GetInstance().ResetTeamRequestJoinInEndTimeDic();
        }

        #region ExtraUIBind
        private TeamDuplicationTeamListView mTeamDuplicationTeamListView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationTeamListView = mBind.GetCom<TeamDuplicationTeamListView>("TeamDuplicationTeamListView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationTeamListView = null;
        }
        #endregion

    }
}
