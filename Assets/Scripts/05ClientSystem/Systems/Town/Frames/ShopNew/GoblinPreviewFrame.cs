using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinPreviewFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ShopNew/GoblinPreviewFrame";
        }
        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, _updatePreviewView);
            MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(9);
            if (mGoblinPreviewView != null)
                mGoblinPreviewView.InitPreviewView();
        }

        #region ExtraUIBind
        private GoblinPreviewView mGoblinPreviewView = null;

        protected override void _bindExUI()
        {
            mGoblinPreviewView = mBind.GetCom<GoblinPreviewView>("GoblinPreviewView");
        }

        protected override void _unbindExUI()
        {
            mGoblinPreviewView = null;
        }
        #endregion


        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, _updatePreviewView);
        }

        private void _updatePreviewView(UIEvent uiEvent)
        {
            int type = (int)uiEvent.Param1;
            if(type == 6)
            {
                if (mGoblinPreviewView != null)
                    mGoblinPreviewView.InitPreviewView();
            }
        }
    }

}
