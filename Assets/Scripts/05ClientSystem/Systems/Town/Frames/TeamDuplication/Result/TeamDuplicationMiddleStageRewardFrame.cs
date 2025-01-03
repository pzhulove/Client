using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //阶段奖励
    public class TeamDuplicationMiddleStageRewardFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Result/TeamDuplicationMiddleStageRewardFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationMiddleStageRewardView != null)
            {
                int stageId = (int)userData;
                mTeamDuplicationMiddleStageRewardView.Init(stageId);
            }
        }

        #region ExtraUIBind
        private TeamDuplicationMiddleStageRewardView mTeamDuplicationMiddleStageRewardView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationMiddleStageRewardView = mBind.GetCom<TeamDuplicationMiddleStageRewardView>("TeamDuplicationMiddleStageRewardView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationMiddleStageRewardView = null;
        }
        #endregion

    }
}
