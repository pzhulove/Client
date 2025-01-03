using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamDuplicationMainBuildFrame : ClientFrame
    {
        ComTalk m_miniTalk;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TeamDuplication/TeamDuplicationMainBuildFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();
            
            if (mTeamDuplicationMainBuildView != null)
                mTeamDuplicationMainBuildView.Init();

            InitComTalk();
        }

        protected override void _OnCloseFrame()
        {

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame townFrame = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                if (townFrame != null)
                {
                    // marked by ckm
                    var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                    var townTableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                    if(townTableData == null || townTableData.SceneType != CitySceneTable.eSceneType.TEAMDUPLICATION)
                    {
                        townFrame.SetForbidFadeIn(false);
                    }
                }
            }

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
        private TeamDuplicationMainBuildView mTeamDuplicationMainBuildView = null;
        private GameObject mTalkParent = null;

        protected override void _bindExUI()
        {
            mTeamDuplicationMainBuildView = mBind.GetCom<TeamDuplicationMainBuildView>("TeamDuplicationMainBuildView");
            mTalkParent = mBind.GetGameObject("TalkParent");
        }

        protected override void _unbindExUI()
        {
            mTeamDuplicationMainBuildView = null;
            mTalkParent = null;
        }
        #endregion


    }
}
