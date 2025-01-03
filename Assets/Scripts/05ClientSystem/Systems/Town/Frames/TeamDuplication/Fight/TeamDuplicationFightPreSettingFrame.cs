using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationFightPreSettingFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Fight/TeamDuplicationFightPreSettingFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationFightPreSettingView != null)
            {
                mTeamDuplicationFightPreSettingView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationFightPreSettingView mTeamDuplicationFightPreSettingView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationFightPreSettingView = mBind.GetCom<TeamDuplicationFightPreSettingView>("TeamDuplicationFightPreSettingView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationFightPreSettingView = null;
        }
        #endregion
        

    }
}
