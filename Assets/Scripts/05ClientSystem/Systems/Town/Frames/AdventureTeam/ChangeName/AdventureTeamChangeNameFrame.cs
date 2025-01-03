using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamChangeNameFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventureTeam/AdventureTeamChangeNameFrame";
        }
        
        protected override void _OnOpenFrame()
        {
            if (userData == null)
            {
                Logger.LogError("AdventureTeamChangeNameFrame out param data is null");
                return;
            }
            AdventureTeamRenameModel renameModel = userData as AdventureTeamRenameModel;
            if (renameModel == null)
            {
                Logger.LogError("AdventureTeamChangeNameFrame renameModel is null");
                return;
            }
            
            if(mAdventureTeamChangeNameView != null)
                mAdventureTeamChangeNameView.InitData(renameModel);
        }       

        protected override void _OnCloseFrame()
        {
            if (mAdventureTeamChangeNameView != null)
                mAdventureTeamChangeNameView.Clear();
        }

        #region ExtraUIBind
        private AdventureTeamChangeNameView mAdventureTeamChangeNameView = null;

        protected override void _bindExUI()
        {
            mAdventureTeamChangeNameView = mBind.GetCom<AdventureTeamChangeNameView>("AdventureTeamChangeNameView");
        }

        protected override void _unbindExUI()
        {
            mAdventureTeamChangeNameView = null;
        }
        #endregion
        
    }

}
