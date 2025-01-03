using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //大阶段翻牌奖励
    public class TeamDuplicationFinalStageCardFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Result/TeamDuplicationFinalStageCardFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFinalStageCardView != null)
                mTeamDuplicationFinalStageCardView.Init();
        }

        #region ExtraUIBind
        private TeamDuplicationFinalStageCardView mTeamDuplicationFinalStageCardView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFinalStageCardView = mBind.GetCom<TeamDuplicationFinalStageCardView>("TeamDuplicationFinalStageCardView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFinalStageCardView = null;
        }
        #endregion
        

    }
}
