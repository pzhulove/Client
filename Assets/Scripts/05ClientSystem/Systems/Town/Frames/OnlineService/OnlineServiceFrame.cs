using GameClient;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    public class OnlineServiceFrame : ClientFrame
    {
        private const int CanvasLayerIndex = 5;
        private bool beJumpVip = false;
        private bool isVipAuthFuncLock = false;

		#region ExtraUIBind
		private Button mBtnRefresh = null;
		private Button mBtnGoback = null;
		private Button mBtnClose = null;
		private UniWebViewUtility mWebView = null;
		private GameObject mVipEntrance = null;
		private DOTweenAnimation mVipEntranceTween = null;
		private Button mSpecialServiceBtn = null;
		private Text mSpecialServiceText = null;
		private Text mVipDescText = null;
		private Button mReturnNormal = null;
		private Text mText = null;
		
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
			mWebView = mBind.GetCom<UniWebViewUtility>("WebView");
			mVipEntrance = mBind.GetGameObject("VipEntrance");
			mVipEntranceTween = mBind.GetCom<DOTweenAnimation>("VipEntranceTween");
			mSpecialServiceBtn = mBind.GetCom<Button>("SpecialServiceBtn");
			if (null != mSpecialServiceBtn)
			{
				mSpecialServiceBtn.onClick.AddListener(_onSpecialServiceBtnButtonClick);
			}
			mSpecialServiceText = mBind.GetCom<Text>("SpecialServiceText");
			mVipDescText = mBind.GetCom<Text>("VipDescText");
			mReturnNormal = mBind.GetCom<Button>("ReturnNormal");
			if (null != mReturnNormal)
			{
				mReturnNormal.onClick.AddListener(_onReturnNormalButtonClick);
			}
			mText = mBind.GetCom<Text>("Text");
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
			mWebView = null;
			mVipEntrance = null;
			mVipEntranceTween = null;
			if (null != mSpecialServiceBtn)
			{
				mSpecialServiceBtn.onClick.RemoveListener(_onSpecialServiceBtnButtonClick);
			}
			mSpecialServiceBtn = null;
			mSpecialServiceText = null;
			mVipDescText = null;
			if (null != mReturnNormal)
			{
				mReturnNormal.onClick.RemoveListener(_onReturnNormalButtonClick);
			}
			mReturnNormal = null;
			mText = null;
		}
		#endregion

        #region Callback
        private void _onBtnRefreshButtonClick()
        {
            /* put your code in here */
            if (mWebView)
            {
                mWebView.ReLoadUrl();
            }
        }
        private void _onBtnCloseButtonClick()
        {
            /* put your code in here */
            this.Close();
        }

        private void _onBtnGobackButtonClick()
        {
            if (mWebView)
            {
                mWebView.GoBackWebView();
            }
        }

        private void _onSpecialServiceBtnButtonClick()
        {
            /* put your code in here */
            //OnlineServiceManager.GetInstance().ReqOnlineServiceUrlSign(true);
        }

        private void _onReturnNormalButtonClick()
        {
            /* put your code in here */
            //OnlineServiceManager.GetInstance().ReqOnlineServiceUrlSign();
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LimitTimeGift/OnlineService";
        }

        public static bool keyboardShowOut;
        public static void SetKeyBoardShowOut(bool isShow)
        {
            keyboardShowOut = isShow;
        }

        protected override void _OnOpenFrame()
        {
            _BindEvent();

            //bool bVipShow = TryInitSpecialServiceEntrance();

            OnlineServiceManager.GetInstance().StartReqOfflineInfos(false);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRecOnlineServiceNewNote, false);//关闭未读消息气泡提示

            string url = "";
            if (userData != null)
            {
                url = userData as string;
            }

            if (mBtnGoback)
            {
                mBtnGoback.gameObject.CustomActive(false);
            }

            if (mWebView != null)
            {
                //mWebView.PageFinished += OnGoBackBtnShow;
                mWebView.PageViewUpdate += OnWebViewUpdate;
                mWebView.PageLoadReveiveMsg += OnJumpVipService;
                mWebView.InitWebView();
            }

            //if(!bVipShow)
            //{
                ShowUrlOnWebView(url);
            //}
        }

        protected override void _OnCloseFrame()
        {
            _UnBindEvent();
            if (mWebView != null)
            {
                //mWebView.PageFinished -= OnGoBackBtnShow;
                mWebView.PageViewUpdate -= OnWebViewUpdate;
                mWebView.PageLoadReveiveMsg -= OnJumpVipService;
                mWebView.UnInitWebView();
            }
            OnlineServiceManager.GetInstance().StartReqOfflineInfos(true);
            _ClearData();
        }

        public override bool IsNeedUpdate()
        {
            return canUpdate;
        }

        private float mTickTime = 0.0f;
        private bool canUpdate = true;
        private bool dirty = false;
        protected override void _OnUpdate(float delta)
        {
            mTickTime += delta;
            if (mTickTime > 1.0f)
            {
                if (mWebView != null)
                {
                    mWebView.OnUpdate(delta);
                }
                mTickTime = 0f;
            }
        }

        void _BindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, OnNewGuideStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRecVipOnlineService, OnRecVipOnlineService);

            ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(Protocol.ServiceType.SERVICE_VIP_AUTH, OnServerFuncSwitch);
        }

        void _UnBindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnNewGuideStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRecVipOnlineService, OnRecVipOnlineService);

            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(Protocol.ServiceType.SERVICE_VIP_AUTH, OnServerFuncSwitch);
        }

        void OnNewGuideStart(UIEvent mEvent)
        {
            this.Close();
        }

        void _ClearData()
        {
            beJumpVip = false;
            isVipAuthFuncLock = false;
        }

        void OnRecVipOnlineService(UIEvent mEvent)
        {
            string url = mEvent.Param1 as string;
            bool isVipAuth = (bool)mEvent.Param2;
            ShowUrlOnWebView(url);

            /*
            ShowReturnNormalEntrance(false);
            if (!isVipAuth && mVipEntranceTween && mVipEntrance.activeSelf == true)
            {
                mVipEntranceTween.DOPlay();
            }
            else if(isVipAuth && mVipEntrance)
            {
                mVipEntrance.CustomActive(false);
                //ChangeFrameTitle();
            }
             * */
        }

        bool TryInitSpecialServiceEntrance()
        {          
            //if (!isSpecialVip)
            //{
            //    ShowVipEntrance(false);
            //    ShowReturnNormalEntrance(false);
            //    OnlineServiceManager.GetInstance().ReqOnlineServiceUrlSign();               //请求在线客服验签链接 普通客服
            //    return false;
            //}
            //ShowVipEntrance(true);
            //ShowReturnNormalEntrance(true);
          
            //if (mSpecialServiceText)
            //{ 
            //    string specialService = TR.Value("vip_online_service_entrance_button");
            //    mSpecialServiceText.text = specialService;
            //}
            //if (mVipDescText)
            //{
            //    string vipDesc = TR.Value("vip_online_service_entrance_desc");
            //    mVipDescText.text = string.Format(vipDesc,PlayerBaseData.GetInstance().VipLevel);
            //}
            return true;
        }

        private void ShowUrlOnWebView(string url)
        {
            if (mWebView != null && !string.IsNullOrEmpty(url))
            {
                mWebView.LoadUrl(url);
                mWebView.ShowWebView();
            }
        }

        private void ShowVipEntrance(bool bShow)
        {
            if (mVipEntrance)
            {
                mVipEntrance.CustomActive(bShow);
            }
        }

        private void ShowReturnNormalEntrance(bool bShow)
        {
            if (mReturnNormal)
            {
                mReturnNormal.gameObject.CustomActive(bShow);
            }
        }

        private void ChangeFrameTitle()
        {
            if (mText)
            {
                mText.text = beJumpVip ? TR.Value("vip_online_service_entrance_title") : TR.Value("vip_online_service_entrance_normal_title");
            }
        }

        private void OnGoBackBtnShow()
        {
            /*
            if (mWebView)
            {
                bool canGoBack = mWebView.CanWebViewGoBack();
                if (mBtnGoback)
                {
                    mBtnGoback.gameObject.CustomActive(canGoBack);
                }
            }
            */
        }

        private void OnWebViewUpdate()
        {
            ChangeFrameTitle();
        }

        private void OnJumpVipService(UniWebViewMessage message)
        {
            beJumpVip = false;

            if (mWebView == null)
                return;
            if (message.Equals(null))
                return;
            if (!message.Path.Equals("jumpvip"))
                return;
            if (message.Args == null)
                return;

            string reloadUrl = "";
            foreach (var arg in message.Args)
            {
                if (arg.Key.Equals("params"))
                {
                    reloadUrl = arg.Value;
                    if (!string.IsNullOrEmpty(reloadUrl))
                    {
                        beJumpVip = true;
                        //Logger.LogError("reloadUrl 1 : " + reloadUrl);
                        //string reloadUrl2 = System.Uri.EscapeUriString(reloadUrl);
                        //Logger.LogError("reloadUrl 2 : " + reloadUrl2);
                        //string reloadUrl3 = WWW.UnEscapeURL(reloadUrl);
                        //Logger.LogError("reloadUrl 3 : " + reloadUrl3);
                        mWebView.LoadUrl(reloadUrl);
                    }
                    break;
                }
            }
        }

        private void OnServerFuncSwitch(ServerSceneFuncSwitch fSwitch)
        {
            if (fSwitch.sType != Protocol.ServiceType.SERVICE_VIP_AUTH)
            {
                return;
            }
            isVipAuthFuncLock = !fSwitch.sIsOpen;
        }
    }
}
