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
    class OperateAdsBoardFrame : ClientFrame
    {
        private float mTickTime = 0.0f;

        #region ExtraUIBind
        private Button mBtnRefresh = null;
        private Button mBtnClose = null;
        private UniWebViewUtility mWebView = null;
        private Text titleName = null;
        private Button mBtnGoback = null;

        protected override void _bindExUI()
        {
            mBtnRefresh = mBind.GetCom<Button>("BtnRefresh");
            mBtnRefresh.onClick.AddListener(_onBtnRefreshButtonClick);
            mBtnRefresh.gameObject.CustomActive(false);
            mBtnClose = mBind.GetCom<Button>("BtnClose");
            mBtnClose.onClick.AddListener(_onBtnCloseButtonClick);
            mWebView = mBind.GetCom<UniWebViewUtility>("WebView");
            titleName = mBind.GetCom<Text>("Title");
            mBtnGoback = mBind.GetCom<Button>("BtnGoback");
            mBtnGoback.onClick.AddListener(_onBtnGobackButtonClick);
        }

        protected override void _unbindExUI()
        {
            mBtnRefresh.onClick.RemoveListener(_onBtnRefreshButtonClick);
            mBtnRefresh = null;
            mBtnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
            mBtnClose = null;
            mWebView = null;
            titleName = null;
            mBtnGoback.onClick.RemoveListener(_onBtnGobackButtonClick);
            mBtnGoback = null;
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

        private void _onBtnGobackButtonClick()
        {
            if (mWebView)
            {
                mWebView.GoBackWebView();
            }
        }
        #endregion
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/OperationAds/OperationAdsBoard";
        }

        protected override void _OnOpenFrame()
        {
            string url = "";
            if (userData != null)
            {
                url = userData as string;
            }
            if (!string.IsNullOrEmpty(mFrameName))
            {
                if (titleName)
                {
                    titleName.text = mFrameName;
                }
            }
            if (mBtnGoback)
            {
                mBtnGoback.gameObject.CustomActive(false);
            }

            if (mWebView != null)
            {
                if (string.IsNullOrEmpty(url) == false)
                {
                    //mWebView.PageFinished += OnGoBackBtnShow;

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
               //mWebView.PageFinished -= OnGoBackBtnShow;

                mWebView.UnInitWebView();               
            }
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            mTickTime += timeElapsed;

            if (mTickTime > 3.0f)
            {
                //CheckCurrAutoShowFrame();
                OnGoBackBtnShow();
                mTickTime = 0f; ;
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        private void OnGoBackBtnShow()
        {
            if (mWebView)
            {
                bool canGoBack = mWebView.CanWebViewGoBack();
                if (mBtnGoback)
                {
                    mBtnGoback.gameObject.CustomActive(canGoBack);
                }
            }
        }
    }
}
