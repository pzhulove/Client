using Protocol;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class OldPlayerFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SelecteRoleNew/OldPlayerFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            _InitData();
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {

        }

        void _UnRegisterUIEvent()
        {

        }

        void _InitData()
        {
            ClientApplication.veteranReturn = 0;
        }

        void _ClearData()
        {

        }

        #region ExtraUIBind
        private Button mReStart = null;
        private Button mOk = null;
        private Button mClose = null;

        protected override void _bindExUI()
        {
            mReStart = mBind.GetCom<Button>("reStart");
            if (null != mReStart)
            {
                mReStart.onClick.AddListener(_onReStartButtonClick);
            }
            mOk = mBind.GetCom<Button>("ok");
            if (null != mOk)
            {
                mOk.onClick.AddListener(_onOkButtonClick);
            }
            mClose = mBind.GetCom<Button>("close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
        }

        protected override void _unbindExUI()
        {
            if (null != mReStart)
            {
                mReStart.onClick.RemoveListener(_onReStartButtonClick);
            }
            mReStart = null;
            if (null != mOk)
            {
                mOk.onClick.RemoveListener(_onOkButtonClick);
            }
            mOk = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onReStartButtonClick()
        {
            /* put your code in here */
            ClientApplication.DisconnectGateServerAtLogin();
            if(ClientSystemManager.GetInstance().IsFrameOpen<SelectRoleFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<SelectRoleFrame>();
            }
            frameMgr.CloseFrame(this);
        }
        private void _onOkButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}