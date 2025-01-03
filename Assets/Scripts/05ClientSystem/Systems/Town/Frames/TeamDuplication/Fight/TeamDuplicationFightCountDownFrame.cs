using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightCountDownFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/TeamDuplicationFightCountDownFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightCountDownView != null)
            {
                mTeamDuplicationFightCountDownView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightCountDownView mTeamDuplicationFightCountDownView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightCountDownView = mBind.GetCom<TeamDuplicationFightCountDownView>("TeamDuplicationFightCountDownView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightCountDownView = null;
        }
        #endregion
        
    }
}
