using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using UnityEngine.UI;

namespace GameClient
{
    public class ChijiEntranceFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiEntranceFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }

        #region ExtraUIBind
        private Button mHelp = null;
        private Button mClose = null;
        private Button mGo = null;

        protected override void _bindExUI()
        {
            mHelp = mBind.GetCom<Button>("Help");
            if (null != mHelp)
            {
                mHelp.onClick.AddListener(_onHelpButtonClick);
            }
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
            mGo = mBind.GetCom<Button>("Go");
            if (null != mGo)
            {
                mGo.onClick.AddListener(_onGoButtonClick);
            }
        }

        protected override void _unbindExUI()
        {
            if (null != mHelp)
            {
                mHelp.onClick.RemoveListener(_onHelpButtonClick);
            }
            mHelp = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
            if (null != mGo)
            {
                mGo.onClick.RemoveListener(_onGoButtonClick);
            }
            mGo = null;
        }
        #endregion

        #region Callback
        private void _onHelpButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<ChijiHelpFrame>(FrameLayer.Middle);
        }
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _onGoButtonClick()
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Chiji_has_team"));
                return;
            }

            ChijiDataManager.GetInstance().SwitchingTownToPrepare = true;
            Utility.SwitchToChijiWaittingRoom();
        }
        #endregion
    }
}
