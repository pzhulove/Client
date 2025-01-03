using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightSettingDifficultyFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/TeamDuplicationFightSettingDifficultyFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightSettingDifficultyView != null)
            {
                mTeamDuplicationFightSettingDifficultyView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightSettingDifficultyView mTeamDuplicationFightSettingDifficultyView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightSettingDifficultyView = mBind.GetCom<TeamDuplicationFightSettingDifficultyView>("TeamDuplicationFightSettingDifficultyView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightSettingDifficultyView = null;
        }
        #endregion
        

    }
}
