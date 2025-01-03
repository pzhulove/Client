using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

namespace GameClient
{
    public class ChangeJobFinish : ClientFrame
    {
        const int MaxSkillNum = 3;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ChangeJob/ChangeJobFinish";
        }

        protected override void _OnOpenFrame()
        {
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeJobFinishFrameOpen);
            mView.OnInit();
        }

        protected override void _OnCloseFrame()
        {
            mView.OnUninit();
        }

        private ChangeJobFinishView mView;

        protected sealed override void _bindExUI()
        {
            mView = mBind.GetCom<ChangeJobFinishView>("View");
        }

        protected sealed override void _unbindExUI()
        {
            mView = null;
        }
    }
}
