using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationTeamBuildFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Build/TeamDuplicationTeamBuildFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            //设置背景遮罩的透明度,特殊处理
            if (blackMask != null)
            {
                var blackMaskImage = blackMask.GetComponent<Image>();
                if (blackMaskImage != null)
                    blackMaskImage.color = new Color(0f, 0f, 0f, 210.0f / 255.0f);
            }

            //TeamBuildView初始化
            if (mTeamDuplicationTeamBuildView != null)
            {
                mTeamDuplicationTeamBuildView.Init();
            }
        }

        #region ExtraUIBind
        private TeamDuplicationTeamBuildView mTeamDuplicationTeamBuildView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationTeamBuildView = mBind.GetCom<TeamDuplicationTeamBuildView>("TeamDuplicationTeamBuildView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationTeamBuildView = null;
        }
        #endregion
    }
}
