using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BaseWebViewFrame : ClientFrame
    {
        BaseWebViewParams param = null;
        private float mTickTime = 0.0f;
        private bool needUpdate = true;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/BaseWebView/BaseWebViewFrame";
        }

        protected override void _OnOpenFrame()
        {
            _BindUIEvent();
            _InitFrameView();
        }

        protected override void _OnCloseFrame()
        {
            _UnInitFrameView();
            _UnBindUIEvent();
        }

        public override bool IsNeedUpdate()
        {
            return needUpdate;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            mTickTime += timeElapsed;
            if (mTickTime > 1.0f)
            {
                if (null != mWebView)
                {
                    mWebView.OnUpdate(timeElapsed);
                }
                mTickTime = 0f;
            }
        }

        void _BindUIEvent()
        {

        }

        void _UnBindUIEvent()
        {

        }

        private void _InitFrameView()
        {
            if (userData != null)
            {
                param = userData as BaseWebViewParams;
            }
            if (param == null)
            {
                return;
            }

            BaseWebViewType type = param.type;
            if (mComTitle != null)
            {
                mComTitle.SetTitleByType(type);
            }
            string url = param.fullUrl;
            if (!string.IsNullOrEmpty(url))
            {
                if (mWebView != null)
                {
                    mWebView.PageViewUpdate += OnWebViewUpdate;
                    mWebView.PageLoadReveiveMsg += OnPageReceiveMsg;

                    mWebView.InitWebView();
                    mWebView.LoadUrl(url);
#if UNITY_EDITOR
                    Application.OpenURL(url);
#endif
                    mWebView.ShowWebView();
                }
            }
            needUpdate = param.needFrameUpdate;
            mBtnGoback.CustomActive(param.needGobackBtn);
            mBtnRefresh.CustomActive(param.needRefreshBtn);
        }

        private void _UnInitFrameView()
        {
            if (mWebView != null)
            {
                mWebView.PageViewUpdate -= OnWebViewUpdate;
                mWebView.PageLoadReveiveMsg -= OnPageReceiveMsg;

                mWebView.UnInitWebView();
            }

            mTickTime = 0f;
            if (param != null)
            {
                param.Clear();
                param = null;
            }
        }

        private void OnWebViewUpdate()
        {

        }

        private void OnPageReceiveMsg(UniWebViewMessage message)
        {
            if (mWebView == null)
                return;
            if (message.Equals(null))
                return;
            if (string.IsNullOrEmpty(message.Path))
                return;
            for (int i = 0; param != null && param.uniWebViewMsgs != null && i < param.uniWebViewMsgs.Count; i++)
            {
                var msg = param.uniWebViewMsgs[i];
                if (msg.scheme == message.Scheme &&
                    msg.path == message.Path)
                {
                    if (msg.onReceiveWebViewMsg != null)
                    {
                        msg.onReceiveWebViewMsg(message.Args, mWebView);
                    }
                }
            }
        }

        #region ExtraUIBind
        private Button mBtnRefresh = null;
        private Button mBtnGoback = null;
        private Button mBtnClose = null;
        private ComBaseWebViewTitle mComTitle = null;
        private UniWebViewUtility mWebView = null;

        protected override void _bindExUI()
        {
            mBtnRefresh = mBind.GetCom<Button>("BtnRefresh");
            if (null != mBtnRefresh)
            {
                mBtnRefresh.onClick.AddListener(_onBtnRefreshButtonClick);
            }
            mBtnGoback = mBind.GetCom<Button>("BtnGoback");
            if (null != mBtnGoback)
            {
                mBtnGoback.onClick.AddListener(_onBtnGobackButtonClick);
            }
            mBtnClose = mBind.GetCom<Button>("BtnClose");
            if (null != mBtnClose)
            {
                mBtnClose.onClick.AddListener(_onBtnCloseButtonClick);
            }
            mComTitle = mBind.GetCom<ComBaseWebViewTitle>("ComTitle");
            mWebView = mBind.GetCom<UniWebViewUtility>("WebView");
        }

        protected override void _unbindExUI()
        {
            if (null != mBtnRefresh)
            {
                mBtnRefresh.onClick.RemoveListener(_onBtnRefreshButtonClick);
            }
            mBtnRefresh = null;
            if (null != mBtnGoback)
            {
                mBtnGoback.onClick.RemoveListener(_onBtnGobackButtonClick);
            }
            mBtnGoback = null;
            if (null != mBtnClose)
            {
                mBtnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
            }
            mBtnClose = null;
            mComTitle = null;
            mWebView = null;
        }
        #endregion

        #region Callback
        private void _onBtnRefreshButtonClick()
        {
            if (mWebView)
            {
                mWebView.ReLoadUrl();
            }
        }
        private void _onBtnGobackButtonClick()
        {
            if (mWebView)
            {
                mWebView.GoBackWebView();
            }
        }
        private void _onBtnCloseButtonClick()
        {
            this.Close();
        }
        #endregion
    }
}
