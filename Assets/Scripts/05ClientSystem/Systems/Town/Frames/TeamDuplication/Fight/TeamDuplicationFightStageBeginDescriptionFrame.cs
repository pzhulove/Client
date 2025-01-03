using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightStageBeginDescriptionFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/TeamDuplicationFightStageBeginDescriptionFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightStageBeginDescriptionView != null)
            {
                TeamDuplicationFightStageDescriptionDataModel fightStageDescriptionDataModel = userData as TeamDuplicationFightStageDescriptionDataModel;
                if (fightStageDescriptionDataModel != null)
                {
                    mTeamDuplicationFightStageBeginDescriptionView.Init(fightStageDescriptionDataModel.StageNumber);
                }
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightStageBeginDescriptionView mTeamDuplicationFightStageBeginDescriptionView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightStageBeginDescriptionView = mBind.GetCom<TeamDuplicationFightStageBeginDescriptionView>("TeamDuplicationFightStageBeginDescriptionView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightStageBeginDescriptionView = null;
        }
        #endregion

    }
}
