using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationRequesterListFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Build/TeamDuplicationRequesterListFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationRequesterListView != null)
            {
                mTeamDuplicationRequesterListView.Init();
            }
        }


        #region ExtraUIBind
        private TeamDuplicationRequesterListView mTeamDuplicationRequesterListView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationRequesterListView = mBind.GetCom<TeamDuplicationRequesterListView>("TeamDuplicationRequesterListView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationRequesterListView = null;
        }
        #endregion
        


    }
}
