using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //阶段奖励
    public class TeamDuplicationGameResultFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/Result/TeamDuplicationGameResultFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mTeamDuplicationGameResultView != null)
            {
                var isSucceed = (bool) userData;
                mTeamDuplicationGameResultView.Init(isSucceed);
            }
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();

            //战斗结束，展示大阶段翻牌
            var isSucceed = (bool) userData;
            if (isSucceed == true)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFinalResultCloseMessage);
            }
        }

        #region ExtraUIBind
        private TeamDuplicationGameResultView mTeamDuplicationGameResultView = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationGameResultView = mBind.GetCom<TeamDuplicationGameResultView>("TeamDuplicationGameResultView");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationGameResultView = null;
        }
        #endregion

    }
}
