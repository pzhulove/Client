using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationTeamSettingFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Build/TeamDuplicationTeamSettingFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationTeamSettingView != null)
            {
                TeamDuplicationTeamBuildDataModel teamBuildDataModel = userData as TeamDuplicationTeamBuildDataModel;

                var teamType = TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE;
                uint teamDifficultyLevel = 0;
                bool isResetEquipmentScore = false;
                int ownerEquipmentScore = 0;
                if (teamBuildDataModel != null)
                {
                    teamType = (TeamCopyTeamModel) teamBuildDataModel.TeamModelType;
                    teamDifficultyLevel = teamBuildDataModel.TeamDifficultyLevel;
                    isResetEquipmentScore = teamBuildDataModel.IsResetEquipmentScore;
                    ownerEquipmentScore = teamBuildDataModel.OwnerEquipmentScore;
                }

                mTeamDuplicationTeamSettingView.Init(teamType, teamDifficultyLevel,
                    isResetEquipmentScore,
                    ownerEquipmentScore);
            }
        }

        #region ExtraUIBind
        private TeamDuplicationTeamSettingView mTeamDuplicationTeamSettingView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationTeamSettingView = mBind.GetCom<TeamDuplicationTeamSettingView>("TeamDuplicationTeamSettingView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationTeamSettingView = null;
        }
        #endregion

    }
}
