using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using UnityEngine.Assertions;

namespace GameClient
{

    class ChannelRanklistFrame : ClientFrame
    {
        public const int CanvasLayerIndex = 5;

        #region ExtraUIBind
        private Button mBtnRefresh = null;
        private Button mBtnClose = null;
        private UniWebViewUtility mWebView = null;

        protected override void _bindExUI()
        {
            mBtnRefresh = mBind.GetCom<Button>("BtnRefresh");
            mBtnRefresh.onClick.AddListener(_onBtnRefreshButtonClick);
            mBtnRefresh.gameObject.CustomActive(false);
            mBtnClose = mBind.GetCom<Button>("BtnClose");
            mBtnClose.onClick.AddListener(_onBtnCloseButtonClick);
            mWebView = mBind.GetCom<UniWebViewUtility>("WebView");
        }

        protected override void _unbindExUI()
        {
            mBtnRefresh.onClick.RemoveListener(_onBtnRefreshButtonClick);
            mBtnRefresh = null;
            mBtnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
            mBtnClose = null;
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
        private void _onBtnCloseButtonClick()
        {
            this.Close();
        }
        #endregion
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Ranklist/ChannelRanklist";
        }

        protected override void _OnOpenFrame()
        {
            string url = "";
            if (userData != null)
            {
                url = userData as string;
            }

            if (mWebView != null)
            {
                if (string.IsNullOrEmpty(url) == false)
                {
                    mWebView.InitWebView();
                    mWebView.LoadUrl(url);
                    mWebView.ShowWebView();
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            if (mWebView != null)
            {
                mWebView.UnInitWebView();
            }
        }
    }
}
