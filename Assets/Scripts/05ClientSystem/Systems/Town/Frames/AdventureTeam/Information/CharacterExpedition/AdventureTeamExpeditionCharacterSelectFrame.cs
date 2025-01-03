using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamExpeditionCharacterSelectFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventureTeam/AdventureTeamExpeditionCharacterSelectFrame";
        }
        
        protected override void _OnOpenFrame()
        {
            if(mAdventureTeamExpeditionCharacterSelectView != null)
                mAdventureTeamExpeditionCharacterSelectView.InitData();
        }       

        protected override void _OnCloseFrame()
        {
            if (mAdventureTeamExpeditionCharacterSelectView != null)
                mAdventureTeamExpeditionCharacterSelectView.Clear();
        }

        #region ExtraUIBind
        private AdventureTeamExpeditionCharacterSelectView mAdventureTeamExpeditionCharacterSelectView = null;

        protected override void _bindExUI()
        {
            mAdventureTeamExpeditionCharacterSelectView = mBind.GetCom<AdventureTeamExpeditionCharacterSelectView>("AdventureTeamCharacterSelectView");
        }

        protected override void _unbindExUI()
        {
            mAdventureTeamExpeditionCharacterSelectView = null;
        }
        #endregion
        
    }

}
