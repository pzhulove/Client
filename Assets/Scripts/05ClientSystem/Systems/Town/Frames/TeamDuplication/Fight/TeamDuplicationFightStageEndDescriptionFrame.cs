using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightStageEndDescriptionFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/TeamDuplicationFightStageEndDescriptionFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightStageEndDescriptionView != null)
            {
                TeamDuplicationFightStageDescriptionDataModel fightStageDescriptionDataModel = userData as TeamDuplicationFightStageDescriptionDataModel;
                if (fightStageDescriptionDataModel != null)
                {
                    mTeamDuplicationFightStageEndDescriptionView.Init(fightStageDescriptionDataModel.StageNumber);
                }
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightStageEndDescriptionView mTeamDuplicationFightStageEndDescriptionView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightStageEndDescriptionView = mBind.GetCom<TeamDuplicationFightStageEndDescriptionView>("TeamDuplicationFightStageEndDescriptionView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightStageEndDescriptionView = null;
        }
        #endregion

    }
}
