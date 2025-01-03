using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TreasureConversionFrame : ClientFrame
    {
        #region ExtraUIBind
        private TreasureConversionView mTreasureConversionView = null;
        private Button mClose = null;

        protected sealed override void _bindExUI()
        {
            mTreasureConversionView = mBind.GetCom<TreasureConversionView>("TreasureConversionView");
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected sealed override void _unbindExUI()
        {
            mTreasureConversionView = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TreasureConversionFrame/TreasureConversionFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, OnActivityUpdate);
            if (mTreasureConversionView != null)
            {
                mTreasureConversionView.InitView();
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, OnActivityUpdate);
        }

        private void OnActivityUpdate(UIEvent a_event)
        {
            if (BeadCardManager.GetInstance().CheckTreasureConvertActivityOpon() == false)
            {
                frameMgr.CloseFrame(this);
            }
        }
    }
}