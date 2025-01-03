using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventurerPassCardHelpFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventurerPassCard/AdventurerPassCardHelpFrame";
        }

        protected override void _OnOpenFrame()
        {
        }

        protected override void _OnCloseFrame()
        {
        }

        private Button mBtnGetActive;
        protected override void _bindExUI()
        {
            mBtnGetActive = mBind.GetCom<Button>("BtnGetActive");
            mBtnGetActive.SafeSetOnClickListener(_OnClickGetActive);
        }

        protected override void _unbindExUI()
        {
            mBtnGetActive = null;
        }

        private void _OnClickGetActive()
        {
            ClientSystemManager.GetInstance().CloseFrame<AdventurerPassCardHelpFrame>();
            ClientSystemManager.GetInstance().CloseFrame<AdventurerPassCardFrame>();
            if (ClientSystemManager.GetInstance().IsFrameOpen<ActivityDungeonFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ActivityDungeonFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<ActivityDungeonFrame>();
        }
    }
}
