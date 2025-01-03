using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightStagePanelFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/TeamDuplicationFightStagePanelFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightStagePanelView != null)
            {
                var fightStageId = (int) userData;
                mTeamDuplicationFightStagePanelView.Init(fightStageId);
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightStagePanelView mTeamDuplicationFightStagePanelView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightStagePanelView = mBind.GetCom<TeamDuplicationFightStagePanelView>("TeamDuplicationFightStagePanelView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightStagePanelView = null;
        }
        #endregion
        

    }
}
