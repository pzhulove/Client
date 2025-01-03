using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationMainFightFrame : ClientFrame
    {
        ComTalk m_miniTalk;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/TeamDuplicationMainFightFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mTeamDuplicationMainFightView != null)
                mTeamDuplicationMainFightView.Init();

            InitComTalk();
        }

        protected override void _OnCloseFrame()
        {
            if (m_miniTalk != null)
            {
                ComTalk.Recycle();
                m_miniTalk = null;
            }
        }

        void InitComTalk()
        {
            m_miniTalk = ComTalk.Create(mTalkParent);
        }

        #region ExtraUIBind
        private TeamDuplicationMainFightView mTeamDuplicationMainFightView = null;
        private GameObject mTalkParent = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationMainFightView = mBind.GetCom<TeamDuplicationMainFightView>("TeamDuplicationMainFightView");
            mTalkParent = mBind.GetGameObject("TalkParent");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationMainFightView = null;
            mTalkParent = null;
        }
        #endregion

    }
}
