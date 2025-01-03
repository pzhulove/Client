using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    public class ChijiFullMapFrame : ClientFrame
    {
        ChijiMapFrame m_mapFrame;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiFullMapFrame";
        }
        public ChijiMapFrame MapFrame { get { return m_mapFrame; } }
        protected override void _OnOpenFrame()
        {
            m_mapFrame = frameMgr.OpenFrame<ChijiMapFrame>(mContent, ChijiMapState.Full_Map, "chiji_full_map") as ChijiMapFrame;
        }

        protected override void _OnCloseFrame()
        {
            frameMgr.CloseFrame(m_mapFrame);
        }

        #region ExtraUIBind
        private GameObject mContent = null;
        private Button mClose = null;

        protected override void _bindExUI()
        {
            mContent = mBind.GetGameObject("content");
            mClose = mBind.GetCom<Button>("close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
        }

        protected override void _unbindExUI()
        {
            mContent = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
