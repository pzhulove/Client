using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections;
using System.Net;
using System.IO;
using XUPorterJSON;
using ProtoTable;

namespace GameClient
{
    public class UserAgreementdata
    {
        public bool err { get; set; }

        public bool agree { get; set; }

    }

    class ClientSystemLogin : ClientSystem
    {
#region ExtraUIBind
        private GameObject mAccountRoot = null;
        private GameObject mPassWordRoot = null;
        private Text mVersionCode = null;
        private ComCommonBind mCurServerBind = null;
        private Text mPackedInfo = null;
        private InputField mPassword = null;
        private InputField mAccount = null;
        private Button mSelectServer = null;
        private Button mPublish = null;

        private Button mGuanwang = null;

        private Button mLogin = null;

        private Button mRegister = null;

        private GameObject mRootServerLst = null;
        private GameObject mRootClickEnter = null;
        private Text mClickEnterTips = null;
        private Button mBtnClickEnter = null;
        private Toggle mTgSelUserAgree = null;
        private Button mUserAgreement = null;
        private Button mUploadCompress = null;
        private GeObjectRenderer mLoginSpineRender = null;
        private Button mSecretAgreement = null;


        private GameObject mImage_bg1 = null;
        private GameObject mImage_bg2 = null;
        private GameObject mTishiwenzi = null;
        private GameObject mVr = null;
        private GameObject mIosDescRoot = null;
        private Text mTishiwenzi_ios = null;
        private Text mVersion_ios = null;
        private Text mPackageInfo_ios = null;
        private GameObject miosDescBottom = null;

        private GameObject mLoginPanel = null;
        private GameObject mRegisterPanel = null;

        private Button mFinishRegisterEnter = null;
        private Button mBackEnter = null;

        private InputField mRegisterAccountInput = null;
        private InputField mRegisterPasswordInput = null;
        private InputField mRegisterConfirmPasswordInput = null;
        private InputField mRegisterInvateCodeInput = null;

        private Text mInvateCode = null;
        private Text mInvateCodePlaceholder = null;


        private void SetBanQuan(bool isAppStore)
        {
            if (isAppStore)
            {
                mTishiwenzi_ios.CustomActive(true);
                mIosDescRoot.CustomActive(true);
                mVersion_ios.CustomActive(true);
                mPackageInfo_ios.CustomActive(true);
                mImage_bg1.CustomActive(false);
                //mImage_bg2.CustomActive(false);
                mTishiwenzi.CustomActive(false);
                mVr.CustomActive(false);
            }
            else
            {
                mTishiwenzi_ios.CustomActive(false);
                mIosDescRoot.CustomActive(false);
                mVersion_ios.CustomActive(false);
                mPackageInfo_ios.CustomActive(false);
                mImage_bg1.CustomActive(true);
                //mImage_bg2.CustomActive(true);
                mTishiwenzi.CustomActive(true);
                mVr.CustomActive(true);
            }
        }

        public static bool mswitchrole = false;
        public static bool mSwitchRole
        {
            get
            {
                return mswitchrole;
            }
            set
            {
                mswitchrole = value;
            //    Logger.LogErrorFormat("mswitchrole = {0}", mswitchrole);
            }
        }

        protected sealed override void _bindExUI()
        {
            mLoginPanel = mBind.GetGameObject("LoginPanel");
            mRegisterPanel = mBind.GetGameObject("RegisterPanel");

            mBackEnter = mBind.GetCom<Button>("BackEnter");
            mBackEnter.onClick.AddListener(_onBackEnterButtonClick);

            mFinishRegisterEnter = mBind.GetCom<Button>("FinishRegisterEnter");
            mFinishRegisterEnter.onClick.AddListener(_onFinishRegisterEnterButtonClick);

            mRegisterAccountInput = mBind.GetCom<InputField>("RegisterAccountInput");
            mRegisterPasswordInput = mBind.GetCom<InputField>("RegisterPasswordInput");
            mRegisterConfirmPasswordInput = mBind.GetCom<InputField>("RegisterConfirmPasswordInput");
            mRegisterInvateCodeInput = mBind.GetCom<InputField>("RegisterInvateCodeInput");

            mAccountRoot = mBind.GetGameObject("accountRoot");
            mPassWordRoot = mBind.GetGameObject("passWordRoot");
            mVersionCode = mBind.GetCom<Text>("VersionCode");
            mCurServerBind = mBind.GetCom<ComCommonBind>("CurServerBind");
            mPackedInfo = mBind.GetCom<Text>("PackedInfo");
            mPassword = mBind.GetCom<InputField>("password");
            mAccount = mBind.GetCom<InputField>("account");
            mSelectServer = mBind.GetCom<Button>("SelectServer");
            mSelectServer.onClick.AddListener(_onSelectServerButtonClick);
            mPublish = mBind.GetCom<Button>("Publish");
            mPublish.onClick.AddListener(_onPublishButtonClick);
            mLogin = mBind.GetCom<Button>("Login");

            mGuanwang = mBind.GetCom<Button>("Guanwang");
            mGuanwang.onClick.AddListener(_onGuanwangButtonClick);

            mRootServerLst = mBind.GetGameObject("rootServerLst");
            mRootClickEnter = mBind.GetGameObject("rootClickEnter");
            mClickEnterTips = mBind.GetCom<Text>("clickEnterTips");
            mBtnClickEnter = mBind.GetCom<Button>("btnClickEnter");
            mBtnClickEnter.onClick.AddListener(_onBtnClickEnterButtonClick);
            mLogin = mBind.GetCom<Button>("Login");
            mLogin.onClick.AddListener(_onLoginButtonClick);

            mRegister = mBind.GetCom<Button>("Register");
            mRegister.onClick.AddListener(_onRegisterButtonClick);

            mTgSelUserAgree = mBind.GetCom<Toggle>("tgSelUserAgree");
            mTgSelUserAgree.onValueChanged.AddListener(_onTgSelUserAgreeToggleValueChange);
            mUserAgreement = mBind.GetCom<Button>("UserAgreement");
            mUserAgreement.onClick.AddListener(_onUserAgreementButtonClick);
            mUploadCompress = mBind.GetCom<Button>("UploadCompress");
            mUploadCompress.onClick.AddListener(_onUploadCompressButtonClick);
            mLoginSpineRender = mBind.GetCom<GeObjectRenderer>("LoginSpineRender");
            mSecretAgreement = mBind.GetCom<Button>("SecretAgreement");
            mSecretAgreement.onClick.AddListener(_onSecretAgreementButtonClick);

            mImage_bg1 = mBind.GetGameObject("Image_bg1");
            //mImage_bg2 = mBind.GetGameObject("Image_bg2");
            mTishiwenzi = mBind.GetGameObject("tishiwenzi");
            mVr = mBind.GetGameObject("vr");
            mIosDescRoot = mBind.GetGameObject("iosDescRoot");
            mTishiwenzi_ios = mBind.GetCom<Text>("tishiwenzi_ios");
            mVersion_ios = mBind.GetCom<Text>("Version_ios");
            mPackageInfo_ios = mBind.GetCom<Text>("PackageInfo_ios");
            miosDescBottom = mBind.GetGameObject("iosDescBottom");

            mInvateCode = mBind.GetCom<Text>("InvateCode");
            mInvateCode.text = Global.AGENT_TEXT;

            mInvateCodePlaceholder = mBind.GetCom<Text>("InvateCodePlaceholder");
            mInvateCodePlaceholder.text = "请输入" + Global.AGENT_TEXT;

            if (null != mUploadCompress)
            {
                bool isShow = false;
#if MG_TEST
                isShow = true;
#endif
                mUploadCompress.gameObject.CustomActive(isShow);
            }
        }

        protected sealed override void _unbindExUI()
        {
            mLoginPanel = null;
            mRegisterPanel = null;

            mRegisterAccountInput = null;
            mRegisterPasswordInput = null;
            mRegisterConfirmPasswordInput = null;
            mRegisterInvateCodeInput = null;

            mAccountRoot = null;
            mPassWordRoot = null;
            mVersionCode = null;
            mCurServerBind = null;
            mPackedInfo = null;
            mPassword = null;
            mAccount = null;
            mSelectServer.onClick.RemoveListener(_onSelectServerButtonClick);
            mSelectServer = null;
            mPublish.onClick.RemoveListener(_onPublishButtonClick);
            mPublish = null;

            mGuanwang.onClick.RemoveListener(_onGuanwangButtonClick);
            mGuanwang = null;

            mRootServerLst = null;
            mRootClickEnter = null;
            mBtnClickEnter.onClick.RemoveListener(_onBtnClickEnterButtonClick);
            mBtnClickEnter = null;
            mClickEnterTips = null;
            mLogin.onClick.RemoveListener(_onLoginButtonClick);
            mLogin = null;

            mRegister.onClick.RemoveListener(_onRegisterButtonClick);
            mRegister = null;

            mBackEnter.onClick.RemoveListener(_onBackEnterButtonClick);
            mBackEnter = null;

            mFinishRegisterEnter.onClick.RemoveListener(_onFinishRegisterEnterButtonClick);
            mFinishRegisterEnter = null;

            mTgSelUserAgree.onValueChanged.RemoveListener(_onTgSelUserAgreeToggleValueChange);
            mTgSelUserAgree = null;
            mUserAgreement.onClick.RemoveListener(_onUserAgreementButtonClick);
            mUserAgreement = null;
            mUploadCompress.onClick.RemoveListener(_onUploadCompressButtonClick);
            mUploadCompress = null;
            mLoginSpineRender = null;
            mSecretAgreement.onClick.RemoveListener(_onSecretAgreementButtonClick);
            mSecretAgreement = null;

            mImage_bg1 = null;
            //mImage_bg2 = null;
            mTishiwenzi = null;
            mVr = null;
            mIosDescRoot = null;
            mTishiwenzi_ios = null;
            mVersion_ios = null;
            mPackageInfo_ios = null;
            miosDescBottom = null;

            mInvateCode = null;
            mInvateCodePlaceholder = null;

        }
#endregion   

#region Callback
        private void _onSelectServerButtonClick()
        {
            /* put your code in here */
            _onServerList();
        }
        private void _onPublishButtonClick()
        {
            /* put your code in here */
            _onPublish();

        }

        private void _onGuanwangButtonClick()
        {
            /* put your code in here */
            if(!Global.GUAN_WANG.Equals("NONE"))
            {
                Application.OpenURL(Global.GUAN_WANG);
            }
        }

        private void _onBtnClickEnterButtonClick()
        {
            /* put your code in here */
            DoClickEnter();
        }

        private void _onLoginButtonClick()
        {
            /* put your code in here */

            if(!mTgSelUserAgree.isOn)
            {
                SystemNotifyManager.SystemNotify(9301);
                return;
            }

            Logger.LogProcessFormat("[登录 登录按钮]");
            _onLogin();
        }

        private void _onRegisterButtonClick()
        {
            /* put your code in here */
            Logger.LogError("注册按钮点击111");
            if(mLoginPanel != null)
            {
                mLoginPanel.CustomActive(false);
                mLogin.CustomActive(false);
                mRegister.CustomActive(false);
            }

            if(mRegisterPanel != null)
            {
                mRegisterPanel.CustomActive(true);
            }
            
            if(Global.isShowLoginUserAgree)
            {
                mTgSelUserAgree.isOn = false;
                PlayerLocalSetting.SetValue("UserAgreeToggleValue", "false");
                PlayerLocalSetting.SaveConfig();
            }
        }

        private void _onBackEnterButtonClick()
        {
            /* put your code in here */
            Logger.LogError("返回按钮点击");
            if(mLoginPanel != null)
            {
                mLoginPanel.CustomActive(true);
                mLogin.CustomActive(true);
                mRegister.CustomActive(true);
            }

            if(mRegisterPanel != null)
            {
                mRegisterPanel.CustomActive(false);
            }

            if(Global.isShowLoginUserAgree)
            {
                mTgSelUserAgree.isOn = false;
                PlayerLocalSetting.SetValue("UserAgreeToggleValue", "false");
                PlayerLocalSetting.SaveConfig();
            }
        }

        private void _onFinishRegisterEnterButtonClick()
        {
            /* put your code in here */
            Logger.LogError("真正注册按钮点击");
            if(!mTgSelUserAgree.isOn)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请先勾选同意用户协议!");
                return;
            }

            string strAcc = mRegisterAccountInput.text;
            string strPass1 = mRegisterPasswordInput.text;
            string strPass2 = mRegisterConfirmPasswordInput.text;
            string strCPS = mRegisterInvateCodeInput.text;

            Debug.LogWarning("_onRegButtonClick" + strAcc + "|"+ strPass1 + "|"+ strPass2 + "|" + strCPS);

            if (strAcc.Trim().Equals(""))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请输入账号!");
                return;
            }
            if(strPass1.Trim().Equals(""))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请输入密码!");
                return;
            }
            if(!strPass1.Trim().Equals(strPass2.Trim()))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("两次密码不一致!");
                return;
            }

            if(!Global.DEFAULT_AGENT.Equals("DEFAULT_AGENT"))
            {
                strCPS = Global.DEFAULT_AGENT;
            }

            Logining = true;
            GameFrameWork.instance.StartCoroutine(_register(strAcc, strPass1, strCPS));
        }

        IEnumerator _register(string username, string password, string agent)
        {
            BaseWaitHttpRequest registerWt = new BaseWaitHttpRequest();

            registerWt.url = Global.REGISTER_ADDRESS + "?username=" + username.Trim() + "&password=" + password.Trim() + "&agent=" + agent.Trim();

            yield return registerWt;

            if (BaseWaitHttpRequest.eState.Success == registerWt.GetResult())
            {
                string resText = registerWt.GetResultString();
                Hashtable ret = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(resText);
                int retValue = Int32.Parse(ret["ret"].ToString());
                if(retValue != 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(ret["msg"].ToString());
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("注册成功，请返回登录!");

                    // var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    // var context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    // context.Call("reportDYRegister");
                }
                Logining = false;
                yield break;
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("注册失败，请重试!");
                Logining = false;
                yield break;
            }
        }


        private void _onTgSelUserAgreeToggleValueChange(bool changed)
        {
            if(changed)
            {
                PlayerLocalSetting.SetValue("UserAgreeToggleValue", "true");
            }
            else
            {
                PlayerLocalSetting.SetValue("UserAgreeToggleValue", "false");
            }
            PlayerLocalSetting.SaveConfig();
        }

        private void _onUserAgreementButtonClick()
        {
            UserAgreementFrameData data = new UserAgreementFrameData();
            data.frameType = UserAgreementFrameType.LookInfo;

            ClientSystemManager.GetInstance().OpenFrame<UserAgreementFrame>(FrameLayer.Middle, data);
        }

        private void _onUploadCompressButtonClick()
        {
            SystemNotifyManager.BaseMsgBoxOkCancel("是否上传数据", () =>
            {
                ClientSystemManager.instance.OpenFrame<UploadingCompressFrame>();
            }, null, "确定", "取消");
        }

        private void _onSecretAgreementButtonClick()
        {
            UserAgreementFrameData data = new UserAgreementFrameData();
            data.frameType = UserAgreementFrameType.LookInfo;

            ClientSystemManager.GetInstance().OpenFrame<SecretAgreementFrame>(FrameLayer.Middle, data);
        }
        #endregion
        protected string currentServerName;

        protected uint BkgSoundHandle = uint.MaxValue;

        protected List<string> HistoryAccount = new List<string>();
        protected string newActorName = "";

        private bool mIsLogining = false;
        private bool mAutoEnterToRoleSelect = false;

        Coroutine mGetUserAgreementInfo = null;
        public static bool mOpenUserAgreement = false;

        private enum eLoginStatus
        {
            None,
            Logining,
            WaitQueue,
            Fail,
            Success,
        }


        private eLoginStatus mLoginStatus = eLoginStatus.None;

        protected bool Logining 
        {
            get 
            {
                return mIsLogining;
            }

            set
            {
                mIsLogining = value;

                _enableLoginGray(mIsLogining);

                if (mIsLogining)
                {
                    WaitNetMessageFrame.TryOpen();
                }
                else
                {
                    ClientSystemManager.instance.CloseFrame<WaitNetMessageFrame>();
                }
            }
        }

        private void _enableLoginGray(bool enable)
        {
            if (null == mLogin)
            {
                return ;
            }

            UIGray gray = mLogin.gameObject.SafeAddComponent<UIGray>(false);
            if (null != gray)
            {
                gray.enabled = enable;
            }
        }


        protected static bool HasPreLoadRoles = false;

        public void MarkNewActor(string name)
        {
            newActorName = name;

        }

        public void OnReturnToLogin()
        {
            if (uint.MaxValue == BkgSoundHandle)
                BkgSoundHandle = AudioManager.instance.PlaySound("Sound/Login", AudioType.AudioStream, Global.Settings.bgmStart, true);
        }

        public sealed override string GetMainUIPrefabName()
        {
            return "UIFlatten/Prefabs/Login/LoginFrame";
        }

        public void _onServerList()
        {
#if APPLE_STORE
			if (mLoadingServerStatus == eLoadingServerState.Error)
			{
				Logger.LogErrorFormat("[登录] 是错误状态，重新拉取");
				_loadPlayerSetting();
				_forceloadSavedServer();
				return;
			}
			
            if (mLoadingServerStatus == eLoadingServerState.Loading
            || mLoadingServerStatus == eLoadingServerState.None)
            {
                SystemNotifyManager.SystemNotify(8001);
                return;
            }
#else
			if (mLoadingServerStatus == eLoadingServerState.Error
            || mLoadingServerStatus == eLoadingServerState.Loading
            || mLoadingServerStatus == eLoadingServerState.None)
            {
                SystemNotifyManager.SystemNotify(8001);
                return;
            }
#endif

            ClientSystemManager.instance.OpenFrame<ServerListFrame>();
        }

        private void _onPublish()
        {
            ClientSystemManager.instance.OpenFrame<PublishContentFrame>();
        }
        
#if ROBOT_TEST
        public void LoginWithAccount(string acc)
        {
            mAccount.text = acc;
            _onLogin();
        }
#endif
        private void _onLogin()
        {
            //每次点开启就启动乐变检查更新！！
            PluginManager.GetInstance().LeBianRequestUpdate();

            Logger.LogProcessFormat("[登录] 登录按钮");

            if (Logining)
            {
                Logger.LogProcessFormat("[登录] 正在登录中");
                return;
            }

            if (Global.Settings.isUsingSDK)
            {
                if (ClientApplication.playerinfo.state != PlayerState.LOGIN)
                {
                    Logger.LogProcessFormat("[登录], 不在登录状态, 打开登录XY界面");

                    PluginManager.GetInstance().OpenXYLogin();
                    Logger.LogWarning("当前SDK不是在登陆状态");
                    return;
                }
            }

#if APPLE_STORE
			if (mLoadingServerStatus == eLoadingServerState.Error)
			{
				Logger.LogErrorFormat("[登录] 是错误状态，重新拉取");
				_loadPlayerSetting();
				_forceloadSavedServer();
				return;
			}

            if (mLoadingServerStatus == eLoadingServerState.Loading
            || mLoadingServerStatus == eLoadingServerState.None)
            {
                SystemNotifyManager.SystemNotify(8001);
                Logger.LogErrorFormat("[登录], 错误状态，提示玩家 {0}", mLoadingServerStatus);
                return;
            }
#else
			if (mLoadingServerStatus == eLoadingServerState.Error
            || mLoadingServerStatus == eLoadingServerState.Loading
            || mLoadingServerStatus == eLoadingServerState.None)
            {
                SystemNotifyManager.SystemNotify(8001);
                Logger.LogErrorFormat("[登录], 错误状态，提示玩家 {0}", mLoadingServerStatus);
                return;
            }
#endif

            // if(VersionManager.instance.m_ProceedHotUpdate)
            // {
            //     Logger.LogProcessFormat("[登录], 热更新更新");
            // 
            //     if(!VersionManager.instance.m_IsLastest)
            //     {
            //         GameClient.SystemNotifyManager.SysNotifyMsgBoxOK("客户端版本号偏低，请重启客户端更新！");
            //         Logger.LogProcessFormat("[登录], 热更新更新, 重启客户端版本过低");
            //         return;
            //     }
            // 
            // }

            SockAddr adr = new SockAddr();
            adr.ip = ClientApplication.adminServer.ip;
            adr.port = (ushort)ClientApplication.adminServer.port;


            if (string.IsNullOrEmpty(adr.ip) || adr.port == 0)
            {
                ClientSystemManager.instance.OpenFrame<ServerListFrame>();
                Logger.LogProcessFormat("[登录], 打开服务器列表界面");
                return ;
            }

			ClientApplication.playerinfo.serverID = ClientApplication.adminServer.id;

            if (mAccount == null || mPassword == null)
            {
                //Logger.LogError("Account or Password InputField do not exist! \n");
                Logger.LogProcessFormat("[登录], 输入输出框为空");
                return;
            }

            if (uint.MaxValue != BkgSoundHandle)
            {
                AudioManager.instance.Stop(BkgSoundHandle);
                BkgSoundHandle = uint.MaxValue;
            }

            if (mAccount.text.Trim().Equals(""))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请输入账号!");
                return;
            }

            if (mPassword.text.Trim().Equals(""))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请输入密码!");
                return;
            }
            
            Logger.LogError("[登录] mAccount.text = " + mAccount.text);
            Logger.LogError("[登录] mPassword.text = " + mAccount.text);

            Logining = true;
            GameFrameWork.instance.StartCoroutine(_login(adr));
        }

        IEnumerator _login(SockAddr addr)
        {

#if UNITY_IOS && !APPLE_STORE
            if (Global.Settings.enableHotFix)
            {
                HotUpdateDownloader.instance.CheckHotUpdateVersion();
                while (VersionCheckResult.None == HotUpdateDownloader.instance.GetVersionCheckRes())
                {
                    yield return Yielders.EndOfFrame;
                }
            
                if (VersionCheckResult.NeedHotUpdate == HotUpdateDownloader.instance.GetVersionCheckRes())
                {
                    Logining = false;
                    ClientSystemManager.instance.SwitchSystem<ClientSystemVersion>();
                    yield return Yielders.EndOfFrame;
                    yield break;
                }
            }
#endif

            mIsGetGateSendRoleInfo = false;

            string param = "";

            BaseWaitHttpRequest loginWt = new BaseWaitHttpRequest();

            loginWt.url = Global.LOGIN_ADDRESS + "?username=" + mAccount.text.Trim() + "&password=" + mPassword.text.Trim() + "&version=" +  Global.LOGIN_VERSION;

            yield return loginWt;

            if (BaseWaitHttpRequest.eState.Success == loginWt.GetResult())
            {
                string resText = loginWt.GetResultString();
                Hashtable ret = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(resText);
                int retValue = Int32.Parse(ret["ret"].ToString());
                if(retValue != 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(ret["msg"].ToString());
                    Logining = false;
                    yield break;
                }
                else
                {
                    
                    Global.BXY_INFO = ret["bxyInfo"].ToString();
                    Logger.LogError("BXY_INFO = " + Global.BXY_INFO);
                    Global.USERNAME = mAccount.text.Trim();
                    Global.PASSWORD = mPassword.text.Trim();
                    param = "userid=" + ret["uid"].ToString() + "&openid=1&token=" + ret["token"].ToString() + "&pf=" + Global.Settings.defualtChannel;
                    param += "&timestamp=" + ret["timestamp"].ToString();
                    Logger.LogError("param = " + param);
                }
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("登录失败，请重试!");
                Logining = false;
                yield break;
            }

            

            // if(mAccount != null)
            // {
            //     param = "userid=" + mAccount.text + "&openid=1";
            // }
            // else
            // {
            //     Logger.LogError("mAccount is null in [_login]");
            // }

#if UNITY_EDITOR
            param += "&pf=" + Global.Settings.defualtChannel;
#endif

            if (Global.Settings.isUsingSDK)
			{
				if(ClientApplication.playerinfo.state != PlayerState.LOGIN)
				{
                    Logger.LogProcessFormat("[登录], 不在登录状态, 打开登录XY界面");

					PluginManager.GetInstance().OpenXYLogin();
					Logger.LogWarning("当前SDK不是在登陆状态");

                    mLoginStatus = eLoginStatus.Fail;
                    Logining = false;
                    yield break;
				}


				int isAccountValidateRet = -1;
                string errorMsg = "";

	            byte[] hashval = new byte[20];

                string url = PluginManager.instance.LoginVerifyUrl(
                            Global.LOGIN_SERVER_ADDRESS,
                            ClientApplication.playerinfo.serverID+"",
                            ClientApplication.playerinfo.openuid,
                            ClientApplication.playerinfo.token,
                            SystemInfo.deviceUniqueIdentifier,
                            Global.SDKChannelName[(int)(Global.Settings.sdkChannel)],
                            ClientApplication.playerinfo.sdkLoginExt
                        );

                Logger.LogProcessFormat("[登录] 验证 {0}", url);

                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                Logger.LogProcessFormat("[登录] 创建WebReq {0}", url);

                BaseWaitHttpRequest wt = new BaseWaitHttpRequest();

                wt.url = url;

                yield return wt;

                Logger.LogProcessFormat("[登录] 发送完毕 {0}", url);

                if (BaseWaitHttpRequest.eState.Success == wt.GetResult())
                {
                    string resText = wt.GetResultString();
                   
                    if (resText == null)
                    {
                        isAccountValidateRet = 1;
                    }
                    else
                    {
                        Logger.LogProcessFormat("[登录] WebReq收到 {0} 消息 {1}", url, resText);

                        Hashtable ret = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(resText);
                        isAccountValidateRet = Int32.Parse(ret["result"].ToString());
                        param = ret["params"].ToString();
                        if (ret.ContainsKey("error"))
                            errorMsg = ret["error"].ToString();
#if UNITY_ANDROID
                        ClientApplication.playerinfo.openuid = ret["openid"].ToString();

                        if (SDKInterface.Instance.NeedPayToken())
                        {
                            ClientApplication.playerinfo.token = ret["token"].ToString();
                            SDKInterface.Instance.SetAccInfo(ClientApplication.playerinfo.openuid, ClientApplication.playerinfo.token);
                        }
                        if (isAccountValidateRet == (int)Protocol.ProtoErrorCode.LOGIN_VERIFY_ERROR)
                        {
                            PluginManager.GetInstance().OpenXYLogin();
                        }
#endif
#if UNITY_IPHONE || UNITY_IOS
                        if (ret.ContainsKey("openid"))
                        {
                            ClientApplication.playerinfo.openuid = ret["openid"].ToString();
                        }
#endif
                        string hashHex = ret["hashval"].ToString();
                        for (int i = 0; i < hashHex.Length / 2; i++)
                        //for(int i = 0; i < hashHex.Count() / 2; i++)
                        {
                            hashval[i] = Convert.ToByte(hashHex.Substring(i * 2, 2), 16);
                        }
						#if EXCEPTION_UPLOAD
                        if (ret.ContainsKey("logPath"))
                        {
                            ExceptionUploaderManager.instance.SetUploadUrlAndOpenID(
                                    ret["logPath"].ToString(),
                                    ClientApplication.playerinfo.openuid.ToString()
                            );
                        }
						#endif
                    }
                }

                if(isAccountValidateRet != 0)
                {
                    Logining = false;
                    Logger.LogProcessFormat("[登录] 请求url {0}, {1}", url, wt.GetResult());
                    mLoginStatus = eLoginStatus.Fail;

#if MG_TEST || MG_TEST2
                    SystemNotifyManager.SysNotifyMsgBoxOK(errorMsg);
#endif

                    if (BaseWaitHttpRequest.eState.TimeOut == wt.GetResult() ||
                        BaseWaitHttpRequest.eState.Error   == wt.GetResult())
                    {
                        // 验证登录超时，提示网络环境有问题
                        SystemNotifyManager.SystemNotify(8522);
                    }

                    yield break;
                }

                ClientApplication.playerinfo.hashValue = hashval;
			}
            else 
            {
#if UNITY_EDITOR_OSX
                ExceptionUploaderManager.instance.SetUploadUrlAndOpenID("http://39.108.138.140:59965", mAccount.text);
#endif
            }

            ClientApplication.playerinfo.param     = param;

            /// 此行代码之前是SDK相关的服务器验证 流程
            ClientSystemLoginUtility.StartLoginAfterVerify();
        }

        private UnityEngine.Coroutine mWaitRolesProcessCo = null;

        private void _startWaitRolesProcess()
        {
            _stopWaitRolesProcess();
            GameFrameWork.instance.StartCoroutine(_waitLoginSuccessAndRolesData());
        }

        private void _stopWaitRolesProcess()
        {
            if (null != mWaitRolesProcessCo)
            {
                GameFrameWork.instance.StopCoroutine(mWaitRolesProcessCo);
                mWaitRolesProcessCo = null;
            }
        }

        private IEnumerator _waitLoginSuccessAndRolesData()
        {
            mIsGetGateSendRoleInfo = false;

            while (mLoginStatus == eLoginStatus.Logining)
            {
                yield return null;
            }

            while (mLoginStatus == eLoginStatus.WaitQueue)
            {
                yield return null;
            }

            if (mLoginStatus == eLoginStatus.Fail)
            {
                Logining = false;
                Logger.LogProcessFormat("[登录] 用户取消等待");
                yield break;
            }

            //// 这里已经成功登录了 =。=
    
            // TODO 这里需要改进一下=。=
            while (!mIsGetGateSendRoleInfo)
            {
                yield return Yielders.EndOfFrame;
            }

            Logining = false;
        }

        /// <summary>
        /// 这个东西有点不好，需要改进下=。=最好结合到协程里面
        /// </summary>
        private bool mIsGetGateSendRoleInfo = false;


        [MessageHandle(GateNotifyAllowLogin.MsgID)]
        void NotifyAllowLogin(MsgDATA data)
        {
            Logger.LogProcessFormat("[登录] 排队的可以进游戏啦阿拉啦啦啦 ");

            if (mLoginStatus == eLoginStatus.WaitQueue)
            {
                mLoginStatus = eLoginStatus.Logining;
            }
            else
            {
                Logger.LogErrorFormat("[登录] 不在排队状态，通知进游戏？？？ {0} -> {1}", mLoginStatus, eLoginStatus.Logining);
                mLoginStatus = eLoginStatus.Logining;
            }
        }

        [MessageHandle(GateSendRoleInfo.MsgID)]
        void OnGateSendRoleInfo(MsgDATA msg)
        {
            Logger.Log("OnGateSendRoleInfo ..\n");

            if(mSwitchRole)
            {
                return;
            }

            GateSendRoleInfo ret = new GateSendRoleInfo();
            ret.decode(msg.bytes);

            foreach (var info in ret.roles)
            {
                Logger.Log("Role info:" + info.ToString() + "\n");
            }

            //this code must be place before Code:A
            Int32 iSelectedIndex = GetNewCreateActorIndex(ret);
            //Code:A
            ClientApplication.playerinfo.roleinfo = ret.roles;

            ClientApplication.playerinfo.apponintmentOccus = ret.appointmentOccus;

            ClientApplication.playerinfo.appointmentRoleNum = ret.appointmentRoleNum;

            ClientApplication.playerinfo.baseRoleFieldNum = ret.baseRoleField;
            ClientApplication.playerinfo.extendRoleFieldNum = ret.extensibleRoleField;
            ClientApplication.playerinfo.unLockedExtendRoleFieldNum = ret.unlockedExtensibleRoleField;

            ServerListManager.instance.SaveUserData(ret.roles);

            //分包在这里判断是否是老玩家,资源没下载完成
            if (SDKInterface.Instance.IsSmallPackage() && !SDKInterface.Instance.IsResourceDownloadFinished())
            {
                if (ret.roles != null)
                {
                    for(int i=0; i<ret.roles.Length; ++i)
                    {
                        var role = ret.roles[i];
                        PluginManager.instance.LeBianJudgeLevelAndDownload(role.level);
                    }
                }
            }


#if !LOGIC_SERVER
            if(!HasPreLoadRoles)
            {
                RoleSceneLoadingFrame loadingFrame = null;
                if (!ClientSystemManager.GetInstance().IsFrameOpen<RoleSceneLoadingFrame>())
                    loadingFrame = ClientSystemManager.instance.OpenFrame<RoleSceneLoadingFrame>(FrameLayer.Top) as RoleSceneLoadingFrame;

                if (null != loadingFrame)
                {
                    for (int i = 0, icnt = ret.roles.Length; i < icnt; ++i)
                    {
                        RoleInfo data = ret.roles[i];
                        ProtoTable.JobTable jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(data.occupation);
                        if (null == jobData) continue;

                        ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(jobData.Mode);
                        if (null == res) continue;

                        //string avatarModelRes = GeDemoActor.GetAvatarResPath(res.ModelPath);
                        string avatarModelRes = res.ModelPath;
                        AssetInst modelDataInst = AssetLoader.instance.LoadRes(avatarModelRes, typeof(ScriptableObject), false);
                        if (null != modelDataInst)
                        {
                            DModelData modelDataAsset = modelDataInst.obj as DModelData;
                            if (modelDataAsset != null)
                            {
                                loadingFrame.AddLoadingTask(modelDataAsset.modelAvatar.m_AssetPath);

                                for (int j = 0, jcnt = modelDataAsset.partsChunk.Length; j < jcnt; ++j)
                                    loadingFrame.AddLoadingTask(modelDataAsset.partsChunk[j].partAsset.m_AssetPath);
                            }
                        }

                        for (int j = 0, jcnt = data.avatar.equipItemIds.Length; j < jcnt; ++j)
                            loadingFrame.AddLoadingTask(GameClient.PlayerBaseData.GetInstance().GetWeaponResFormID((int)data.avatar.equipItemIds[j]));
                    }
                }

                loadingFrame.ProcessLoading();
                HasPreLoadRoles = true;
            }
#endif

            if (ret.roles.Length <= 0)
            {
                //删除角色后不需要直接进入创建角色界面
                if(!ClientSystemManager.GetInstance().IsFrameOpen<SelectRoleFrame>())
                {
                    if (!ClientSystemManager.GetInstance().IsFrameOpen<CreateRoleFrame>())
                    {
                        ClientSystemManager.instance.OpenFrame<CreateRoleFrame>(FrameLayer.Bottom);
                    }
                }
            }
            else
            {
                if (ClientSystemManager.instance.IsFrameOpen<CreateRoleFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<CreateRoleFrame>();
                }


                if (-1 != iSelectedIndex && newActorName == ret.roles[iSelectedIndex].name)
                {
                    ClientApplication.playerinfo.curSelectedRoleIdx = iSelectedIndex;
                    GameFrameWork.instance.StartCoroutine(StartEnterGame());
                    //ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
                    mIsGetGateSendRoleInfo = true;
                    ComCreateRoleScene.ClearDemoRole();
                    ComCreateRoleScene.ClearSelectRole();

                    GameStatisticManager.GetInstance().DoStatClientData();

                    //added by mjx on 170814
                    var currentRole = ClientApplication.playerinfo.roleinfo[iSelectedIndex];
                    if (currentRole != null)
                    {
                        SDKInterface.Instance.SetCreateRoleInfo(ClientApplication.playerinfo.openuid,
                                                                currentRole.roleId.ToString(),
                                                                currentRole.name,
                                                                currentRole.level + "",
                                                                 ClientApplication.adminServer.name,
                                                                "赛季段位", "角色公会");
                        SDKInterface.Instance.SetCreateRoleInfo(ClientApplication.playerinfo.openuid, ClientApplication.playerinfo.roleinfo[iSelectedIndex].roleId.ToString());

                        //创角
                        SDKInterface.Instance.UpdateRoleInfo(3,
                            ClientApplication.adminServer.id, ClientApplication.adminServer.name,
                            currentRole.strRoleId, currentRole.name, (int)currentRole.occupation);
                    }
                    return;
                }
                else
                {
                    //登录游戏 打开选角界面 重置新解锁角色栏位数
                    ClientApplication.playerinfo.newUnLockExtendRoleFieldNum = 0;

                    SecurityLockDataManager.GetInstance().InitiallizeSystem();
                    ClientSystemManager.instance.OpenFrame<SelectRoleFrame>(FrameLayer.Bottom);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RoleInfoUpdate);

            if(iSelectedIndex != -1)
            {
                UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                uiEvent.EventID = EUIEventID.SetDefaultSelectedID;
                uiEvent.EventParams.CurrentSelectedID = iSelectedIndex;
                UIEventSystem.GetInstance().SendUIEvent(uiEvent);
            }

            mIsGetGateSendRoleInfo = true;
        }

        int GetNewCreateActorIndex(GateSendRoleInfo ret)
        {
            int iSelectedIndex = -1;
            for (int i = 0; i < ret.roles.Length && ClientApplication.playerinfo.roleinfo != null; ++i)
            {
                bool bFind = false;
                for (int j = 0; j < ClientApplication.playerinfo.roleinfo.Length; ++j)
                {
                    if (ret.roles[i].roleId == ClientApplication.playerinfo.roleinfo[j].roleId)
                    {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind)
                {
                    iSelectedIndex = i;
                    break;
                }
            }
            return iSelectedIndex;
        }

        public sealed override void OnStart(SystemContent systemContent)
        {
            AssetAsyncLoader.instance.SetLoadingLimit(16);
            SystemSwitchEventManager.GetInstance().TriggerEvent(SystemEventType.SYSETM_EVENT_SELECT_ROLE);

            //_LoadZhounianSpineAnimation();
        }

        public static IEnumerator StartEnterGame()
        {
            ClientReconnectManager.instance.canRelogin = false;
#if MG_TEST_EXTENT
            if (NetManager.Instance() != null)
            {
                NetManager.Instance().ClearReSendData();
                NetManager.Instance().ResetResend();
            }
#endif
            var roleInfo = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin();
            PlayerBaseData.GetInstance().Name = roleInfo.name;
            PlayerBaseData.GetInstance().Level = roleInfo.level;
            PlayerBaseData.GetInstance().RoleID = roleInfo.roleId;
            PlayerBaseData.GetInstance().JobTableID = roleInfo.occupation;
            PlayerBaseData.GetInstance().PreChangeJobTableID = roleInfo.preOccu;
            //清除修炼场其他角色保存的数据
            if (SkillDamageStorage.GetInstance() != null)
                SkillDamageStorage.GetInstance().ResetData();

            if (Global.Settings.isGuide && !ClientApplication.playerinfo.GetSelectRoleHasPassFirstFight())
            {
                // 关闭第一场战斗引导 ckm
                // GateFinishNewbeeGuide nbguide = new GateFinishNewbeeGuide();
                // nbguide.roleId = roleInfo.roleId;
                // nbguide.id = (UInt32)(NewbieGuideTable.eNewbieGuideTask.FirstFight);

                // NetManager.instance.SendCommand(ServerType.GATE_SERVER, nbguide);

                // BattleMain.OpenBattle(BattleType.NewbieGuide,eDungeonMode.LocalFrame,0,"20000");

                // yield return Yielders.EndOfFrame;

                // ClientSystemManager.instance.SwitchSystem<ClientSystemBattle>();
                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
            }
            else 
            {
                //初始化登录广告推送读表 
                //xzl
                //AdsPush.AdsPushServerDataManager.GetInstance().SetAdsPushTableData();

                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
            }

            yield return Yielders.EndOfFrame;
        }

        public class OptionServerItem : Dropdown.OptionData
        {
            public GlobalSetting.Address addr;
            public OptionServerItem(GlobalSetting.Address addr)
            {
                this.addr = addr;
                this.text = addr.ToString();
            }
        }

        protected override string _GetLevelName()
        {
            return "Start";
        }

        private void _updateVersion()
        {
            if (mVersionCode != null)
            {
                mVersionCode.text = string.Format("{0} {1}", VersionManager.instance.Version(), Global.Settings.loadFromPackage ? "" : "res");
            }

            if(mPackedInfo != null)
            {
                mPackedInfo.text = "";
            }

            if(mVersion_ios != null)
            {
                mVersion_ios.text = string.Format("{0} {1}", VersionManager.instance.Version(), Global.Settings.loadFromPackage ? "" : "res");
            }

            if(mPackageInfo_ios != null)
            {
                mPackageInfo_ios.text = "";
            } 
        }

        private void _LoadHistoryAccount()
        {
            if (!Global.Settings.isUsingSDK)
            {
                string accountDefault = PlayerLocalSetting.GetValue("AccountDefault") as string;
                mAccount.text = accountDefault;

                string passwordDefault = PlayerLocalSetting.GetValue("PasswordDefault") as string;
                mPassword.text = passwordDefault;
            }
        }

        private void _SaveHistoryAccount()
        {
        }

        private void _onAccountUpdate(string st)
        {
        }

        private void _onPasswordUpdate(string st)
        {
        }

        private void _onAccountEndEdit(string st)
        {
            if (!Global.Settings.isUsingSDK)
            {
                if (!string.IsNullOrEmpty(st))
                {
                    PlayerLocalSetting.SetValue("AccountDefault", st);
                    PlayerLocalSetting.SaveConfig();
                    //_forceloadSavedServer();
                }
            }
        }

        private void _onPasswordEndEdit(string st)
        {
            if (!Global.Settings.isUsingSDK)
            {
                if (!string.IsNullOrEmpty(st))
                {
                    PlayerLocalSetting.SetValue("PasswordDefault", st);
                    PlayerLocalSetting.SaveConfig();
                    //_forceloadSavedServer();
                }
            }
        }


        private UnityEngine.Coroutine mCurrentLoadServerCo = null;
        private void _forceloadSavedServer()
        {
            if (null != mCurrentLoadServerCo) GameFrameWork.instance.StopCoroutine(mCurrentLoadServerCo);
            mCurrentLoadServerCo = GameFrameWork.instance.StartCoroutine(_loadSavedServer());
        }

        public void DoClickEnter()
        {
            //if (ClientApplication.playerinfo.state != PlayerState.LOGIN)
            {
                OpenLogin();
            }
        }

        public void OpenLogin()
        {
            //在这里才打开xy的登陆界面
            if (Global.Settings.isUsingSDK)
            {
                mAccountRoot.SetActive(false);
                mPassWordRoot.SetActive(false);

                if (ClientApplication.playerinfo.state == PlayerState.LOGIN)
                {
                    ShowLoginButton();
                }
                else
                {
                    if (!mSwitchRole)
                        PluginManager.GetInstance().OpenXYLogin();
                }
            }
            else
            {
                ShowLoginButton();
            }
        }


        public void ShowLoginButton()
        {
            if (mPublish != null)
            {
                mPublish.gameObject.CustomActive(true);
                
                GameFrameWork.instance.StartCoroutine(ShowRealName());

                if (!mSwitchRole)
                {
                    ClientSystemManager.instance.OpenFrame<PublishContentFrame>(FrameLayer.Middle);

                    if(Global.isShowFCMDialog)
                    {
                        ClientSystemManager.instance.OpenFrame<AntiAddicitionContentFrame>(FrameLayer.Middle);
                    }
                }
            }

            if (mRootServerLst != null)
                mRootServerLst.CustomActive(true);
            if (mRootClickEnter != null)
                mRootClickEnter.CustomActive(false);
        }

        private IEnumerator ShowRealName()
        {
            BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
            wt.url = string.Format("http://{0}/realname", Global.MAIN_ADDRESS);

            yield return wt;

            if (wt.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                if("true" == wt.GetResultString())
                {
                    string RealNameReg =  PlayerLocalSetting.GetValue("RealNameReg") as string;
                    if((RealNameReg == null || RealNameReg == "false"))
                    {
                        ClientSystemManager.instance.OpenFrame<RealNameRegContentFrame>(FrameLayer.Middle);
                    }
                }
            }
        }

        public override void GetEnterCoroutine(AddCoroutine enter)
        {
            base.GetEnterCoroutine(enter);

            enter(_init);
        }

        Coroutine banquan_Cot;
        private void SetBanQuanMsg()
        {
            SetBanQuan(true);

            if (banquan_Cot != null)
            {
                GameFrameWork.instance.StopCoroutine(banquan_Cot);
                banquan_Cot = null;
            }

            banquan_Cot = GameFrameWork.instance.StartCoroutine(_banquanMsg());

        }
        private IEnumerator _banquanMsg()
        {
            BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
            wt.reconnectCnt = 3;
            wt.timeout = 2000;
            string channelName = SDKInterface.Instance.GetPlatformNameByChannel();
            wt.url = string.Format("http://{0}/copyright_txt", Global.IOS_BANQUAN_ADDRESS);
            yield return wt;

            if (mTishiwenzi_ios != null && BaseWaitHttpRequest.eState.Success == wt.GetResult())
            {
                var banquan = wt.GetResultString();
                if (string.IsNullOrEmpty(banquan))
                {
                    miosDescBottom.CustomActive(false);

                }
                else
                {
                    miosDescBottom.CustomActive(true);
                    mTishiwenzi_ios.text = banquan;
                }

            }

        }

        private IEnumerator _init(IASyncOperation op)
        {
            _bindEvents();
            HasPreLoadRoles = false;
            yield return Yielders.EndOfFrame;

            if (!Global.Settings.isUsingSDK)
            {
                if (null != mAccount)
                {
                    mAccount.onValueChanged.AddListener(_onAccountUpdate);
                    mAccount.onEndEdit.AddListener(_onAccountEndEdit);
                }

                if (null != mPassword)
                {
                    mPassword.onValueChanged.AddListener(_onPasswordUpdate);
                    mPassword.onEndEdit.AddListener(_onPasswordEndEdit);
                }
            }

            mAccountRoot.SetActive(true);
            mPassWordRoot.SetActive(true);

            InitEnterTips();

            InitLogoImage();

            OpenLogin();

            // if (mOpenUserAgreement && Global.Settings.isUsingSDK)
            // {
            //     mTgSelUserAgree.gameObject.CustomActive(true);
            // }
            // else
            // {
            //     mTgSelUserAgree.gameObject.CustomActive(false);
            // }

            if(Global.isShowLoginUserAgree)
            {
                mTgSelUserAgree.gameObject.CustomActive(true);
                string userAgreeToggleValue = PlayerLocalSetting.GetValue("UserAgreeToggleValue") as string;
                if(userAgreeToggleValue != null && userAgreeToggleValue.Equals("true"))
                {
                    mTgSelUserAgree.isOn = true;
                }
                else
                {
                    mTgSelUserAgree.isOn = false;
                }
            }
            else
            {
                mTgSelUserAgree.isOn = true;
            }
            
            // 如果只是切换一下角色，那么走切换角色的流程，否则走正常登录流程
            if (mSwitchRole)
            {
                SecurityLockDataManager.GetInstance().InitiallizeSystem();
                ClientSystemManager.instance.OpenFrame<SelectRoleFrame>(FrameLayer.Bottom);

                AudioManager.instance.Stop(BkgSoundHandle);
                BkgSoundHandle = uint.MaxValue;
            }
            else
            {
                BkgSoundHandle = AudioManager.instance.PlaySound("Sound/Login", AudioType.AudioStream, Global.Settings.bgmStart, true);
            }

            // register all global net message
            GlobalNetMessage.instance.Unload();
            GlobalNetMessage.instance.Load();

            _SetServerSelection();
            _updateVersion();

            //#if UNITY_EDITOR
            _LoadHistoryAccount();
            //#endif
            //重新初始化新手引导
            NewbieGuideManager.instance.Reset();

            // 若在登录前
            if (!Global.Settings.isUsingSDK || ClientApplication.playerinfo.state == PlayerState.LOGIN)
            {
                //if (!mSwitchRole)
                {
                    _loadPlayerSetting();
                    _forceloadSavedServer();
                }
            }

            GameFrameWork.instance.StartCoroutine(_GetGuanwang());

#if APPLE_STORE
            if (SDKInterface.instance.NeedShowBanQuanMsg())
            {
                SetBanQuanMsg();
            }
#endif
        }

        private IEnumerator _GetGuanwang()
        {
            string imei = "";
#if UNITY_EDITOR
            imei = "";          
#elif UNITY_ANDROID        
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var telephoneyManager = context.Call<AndroidJavaObject>("getSystemService", "phone");
            imei = telephoneyManager.Call<string>("getImei", 0);//如果手机双卡 双待  就会有两个MIEI号
            if(string.IsNullOrEmpty(imei))
            {
                imei = telephoneyManager.Call<string>("getImei", 1);
            }
            if(string.IsNullOrEmpty(imei))
            {
                imei = telephoneyManager.Call<string>("getMeid");//电信的手机 是MEID
            }
#endif
            BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
            wt.url = string.Format("http://{0}/guanwang?iemi={1}", Global.MAIN_ADDRESS, imei);

            yield return wt;

            if (wt.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                Global.GUAN_WANG = wt.GetResultString();
            }
        }

        private void _LoadZhounianSpineAnimation()
        {
            if (mLoginSpineRender != null)
            {
                mLoginSpineRender.gameObject.CustomActive(true);
                mLoginSpineRender.LoadObject("UIFlatten/Animation/denglu_2zhounian/denglu_2zhounian_spine", 31);
            }
        }

        protected override void _OnUpdate(float timeElapsed)
        {
        }

        private IEnumerator _UserAgreementReq()
        {
            BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
            wt.url = string.Format("http://{0}/agreement_info?pf={1}&id={2}", Global.USER_AGREEMENT_SERVER_ADDRESS, SDKInterface.Instance.GetPlatformNameByChannel(), ClientApplication.playerinfo.openuid);

            yield return wt;

            if (wt.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                UserAgreementdata UserAgreementInfo = wt.GetResultJson<UserAgreementdata>();

                if (UserAgreementInfo != null)
                {
                    TryOpenUserAgreementInfoFrame(UserAgreementInfo);
                }
            }
        }

        private void TryOpenUserAgreementInfoFrame(UserAgreementdata tb)
        {
            if (tb == null)
            {
                return;
            }

            if(tb.err == true)
            {
                Logger.LogError("Return UserAgreementInfo Protcol error !");
                return;
            }

            if(tb.agree == true)
            {
                return;
            }

            UserAgreementFrameData data = new UserAgreementFrameData();

            data.frameType = UserAgreementFrameType.FirstOpen;
            data.PlatFormType = SDKInterface.Instance.GetPlatformNameByChannel();
            data.OpenUid = ClientApplication.playerinfo.openuid;

            ClientSystemManager.instance.OpenFrame<UserAgreementFrame>(FrameLayer.Middle, data);
        }

        private void _stopGetUserAgreementInfo()
        {
            if (mGetUserAgreementInfo != null)
            {
                GameFrameWork.instance.StopCoroutine(mGetUserAgreementInfo);
            }

            mGetUserAgreementInfo = null;
        }

        private void _loadPlayerSetting()
        {
            var accountDefault = PlayerLocalSetting.GetValue("AccountDefault");
            if (!Global.Settings.isUsingSDK)
            {
                if (accountDefault != null)
                {
                    if (null != mAccount)
                    {
                        mAccount.text = accountDefault as string;
                    }
                }
            }

            var passwordDefault = PlayerLocalSetting.GetValue("PasswordDefault");
            if (!Global.Settings.isUsingSDK)
            {
                if (passwordDefault != null)
                {
                    if (null != mPassword)
                    {
                        mPassword.text = passwordDefault as string;
                    }
                }
            }

            var serverDefault = PlayerLocalSetting.GetValue("ServerDefault");
            if (serverDefault != null)
            {
                currentServerName = serverDefault as string;
            }

            var serverid = PlayerLocalSetting.GetValue("ServerID");
            if (serverid != null)
            {
                ClientApplication.adminServer.id = uint.Parse(serverid.ToString());
            }
        }

        protected void _SetServerSelection()
        {
            int curselect = 0;
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();

            if (false)
            {
            }
            else
            {
#if LOCAL_TEST_ETOND_SERVER
                var addr = new GlobalSetting.Address();
                addr.ip = "192.168.2.231";
                addr.port = 8444;
                addr.name = "etond";
                optionList.Add(new OptionServerItem(addr));
#endif
                var slist = Global.Settings.serverList;
                for (int i = 0; i < slist.Length; ++i)
                {
                    var cur = slist[i];
                    var item = new OptionServerItem(cur);
                    optionList.Add(item);
                    if (item.text == currentServerName)
                    {
                        curselect = optionList.IndexOf(item);
                    }
                }
            }

        }

        /// <summary>
        /// 进入
        /// </summary>
        public sealed override void OnEnter()
        {
            UWAProfilerUtility.Mark("[tm]SysLogin_OnEnter");

            base.OnEnter();
            NetManager.instance.AllowForceReconnect = true;
#if STAT_EXTRA_INFO

            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ||
                SDKInterface.instance.IsDownload())
            {
                GameStatisticManager.GetInstance().RecordDownloadStartTime();
            }

            SDKCallback.instance.SetNeedCheckTempure(false);
            GameStatisticManager.GetInstance().DoStatSmallPackageInfo_Net();
            SDKCallback.instance.ReportValues();
#endif

            UWAProfilerUtility.DoDump();
/*
            //分包在这里判断4G玩家
            if (SDKInterface.instance.IsSmallPackage() && !SDKInterface.instance.IsResourceDownloadFinished())
            {
                PluginManager.instance.LeBianPromote4GStart(PluginManager.PromoteType.LOGIN);
            }
*/            
            #if ROBOT_TEST
            tm.tool.AutoFight.instance.Gogogo();
            #endif
        }

        // 销毁
        public override void OnExit()
        {
            UWAProfilerUtility.Mark("[tm]SysLogin_OnExit");

            AssetAsyncLoader.instance.SetLoadingLimit(4);

            _unBindEvents();

            if (!Global.Settings.isUsingSDK)
            {
                if (null != mAccount)
                {
                    mAccount.onValueChanged.RemoveListener(_onAccountUpdate);
                    mAccount.onEndEdit.RemoveListener(_onAccountEndEdit);
                }

                if (null != mPassword)
                {
                    mPassword.onValueChanged.RemoveListener(_onPasswordUpdate);
                    mPassword.onEndEdit.RemoveListener(_onPasswordEndEdit);
                }
            }

            newActorName = string.Empty;

            if (!Global.Settings.isUsingSDK)
            {
                string accountName = "";
                if (null != mAccount)
                {
                    accountName = mAccount.text;
                }

                if (!string.IsNullOrEmpty(accountName))
                {
                    PlayerLocalSetting.SetValue("AccountDefault", accountName);
                }
                
                string password = "";
                if (null != mPassword)
                {
                    password = mPassword.text;
                }

                if (!string.IsNullOrEmpty(password))
                {
                    PlayerLocalSetting.SetValue("PasswordDefault", password);
                }

            }

            if (ClientApplication.adminServer.id > 0)
            {
                PlayerLocalSetting.SetValue("ServerID", ClientApplication.adminServer.id);
            }

            if (!string.IsNullOrEmpty(currentServerName))
            {
                PlayerLocalSetting.SetValue("ServerDefault", currentServerName);
            }

            PlayerLocalSetting.SaveConfig();

            ulong  accid          = 0;
            string loginRoleName  = "";
            ushort loginRoleLevel = 0;

            if (null != ClientApplication.playerinfo)
            {
                accid = ClientApplication.playerinfo.accid;
                RoleInfo info = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin();
                if (null != info)
                {
                    loginRoleName = info.name;
                    loginRoleLevel = info.level;
                }
            }

            BuglyAgent.SetUserId(string.Format("{0}_{1}",loginRoleName, loginRoleLevel));
#if MG_TEST || MG_TEST2 || MGSPTIYAN
            BuglyAgent.SetUserId(string.Format("{0}{1}{2}",PluginManager.GetBuglyFileVerInfo(), loginRoleName, loginRoleLevel));
#endif

            if (uint.MaxValue != BkgSoundHandle)
                AudioManager.instance.Stop(BkgSoundHandle);
            if (SystemManager.TargetSystem != null)
            {
                Type systemType = SystemManager.TargetSystem.GetType();
                if (systemType != null)
                {
                    if (systemType == typeof(ClientSystemBattle))
                    {
                        ManualPoolCollector.instance.Clear();
                    }
                }
            }

            base.OnExit();
        }

        private void _bindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ServerListSelectChanged,  _updateSelectServer);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.onSDKLoginSuccess,        _onSDKLoginSuccess);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ServerLoginStart,         _onServerLoginStart);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ServerLoginFail,          _onServerLoginFail);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ServerLoginFailWithServerConnect,          _onServerLoginFailWithServerConnect);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ServerLoginSuccess,       _onServerLoginSuccess);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ServerLoginQueueWait,     _onServerLoginQueueWait);
        }

        private void _unBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ServerListSelectChanged, _updateSelectServer);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.onSDKLoginSuccess,       _onSDKLoginSuccess);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ServerLoginStart,        _onServerLoginStart);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ServerLoginFail,         _onServerLoginFail);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ServerLoginFailWithServerConnect,          _onServerLoginFailWithServerConnect);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ServerLoginSuccess,      _onServerLoginSuccess);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ServerLoginQueueWait,    _onServerLoginQueueWait);
        }

        private void _onServerLoginStart(UIEvent ui)
        {
            Logger.LogProcessFormat("[登录] 开始登录 {0}", mLoginStatus);

            Logining     = true;
            mLoginStatus = eLoginStatus.Logining;

            _startWaitRolesProcess();
        }

        private void _onServerLoginFailWithServerConnect(UIEvent ui)
        {
            switch (ClientApplication.adminServer.state)
            {
                case eAdminServerStatus.Offline:
                    SystemNotifyManager.SystemNotify(100001);
                    break;
                default:
                    SystemNotifyManager.SystemNotify(8522);
                    break;
            }
        }

        private void _onServerLoginFail(UIEvent ui)
        {
            Logger.LogProcessFormat("[登录] 登录失败 {0}", mLoginStatus);

            ClientReconnectManager.instance.canRelogin = false;
            ClientReconnectManager.instance.canReconnectGate = false;

            NetManager.instance.Disconnect(ServerType.ADMIN_SERVER);
            NetManager.instance.Disconnect(ServerType.GATE_SERVER);

            ClientSystemManager.instance.CloseFrame<CreateRoleFrame>();
            ClientSystemManager.instance.CloseFrame<SelectRoleFrame>();
            ClientSystemManager.instance.CloseFrame<ServerWaitQueueUp>();
            ClientSystemManager.instance.CloseFrame<ServerListFrame>();

            Logining     = false;
            mLoginStatus = eLoginStatus.Fail;
            _stopWaitRolesProcess();
        }

        private void _onServerLoginQueueWait(UIEvent ui)
        {
            uint waitPlayerNum = (uint)ui.Param1;
            Logger.LogProcessFormat("[登录] 登录排队 上一个状态{0}, 等待玩家数目 {1}", mLoginStatus, waitPlayerNum);

            ClientSystemManager.instance.CloseFrame<CreateRoleFrame>();
            ClientSystemManager.instance.CloseFrame<SelectRoleFrame>();
            ClientSystemManager.instance.CloseFrame<ServerListFrame>();

            if (ClientSystemManager.instance.IsFrameOpen<ServerWaitQueueUp>())
            {
                Logger.LogProcessFormat("[登录] 登录排队界面已经打开");
            }
            else
            {
                ClientSystemManager.instance.OpenFrame<ServerWaitQueueUp>(FrameLayer.Middle, waitPlayerNum);
            }


            Logining     = false;
            mLoginStatus = eLoginStatus.WaitQueue;
        }

        private void _onServerLoginSuccess(UIEvent ui)
        {
            Logger.LogProcessFormat("[登录] 登录成功 要等待角色消息 上一个状态{0}", mLoginStatus);

            ClientSystemManager.instance.CloseFrame<ServerWaitQueueUp>();
            ClientSystemManager.instance.CloseFrame<ServerListFrame>();

            Logining     = false;
            mLoginStatus = eLoginStatus.Success;
        }

        private void _onSDKLoginSuccess(UIEvent ui)
        {
            ShowLoginButton();

            if (mOpenUserAgreement && Global.Settings.isUsingSDK && !mSwitchRole)
            {
                _stopGetUserAgreementInfo();
                mGetUserAgreementInfo = GameFrameWork.instance.StartCoroutine(_UserAgreementReq());
            }

            _loadPlayerSetting();
            _forceloadSavedServer();
        }

        private void _updateSelectServer(UIEvent ui)
        {
            if (null == mCurServerBind)
            {
                return ;
            }

            Image status = mCurServerBind.GetCom<Image>("status");
            Text name = mCurServerBind.GetCom<Text>("name");

            name.text = ClientApplication.adminServer.name;

            switch(ClientApplication.adminServer.state)
            {
                case eAdminServerStatus.Full:
                    // status.sprite = mCurServerBind.GetSprite("statusfull");
                    mCurServerBind.GetSprite("statusfull", ref status);
                    break;
                case eAdminServerStatus.Buzy:
                    // status.sprite = mCurServerBind.GetSprite("statusbuzy");
                    mCurServerBind.GetSprite("statusbuzy", ref status);
                    break;
                case eAdminServerStatus.Offline:
                    // status.sprite = mCurServerBind.GetSprite("statusoffline");
                    mCurServerBind.GetSprite("statusoffline", ref status);
                    break;
                case eAdminServerStatus.Ready:
                    // status.sprite = mCurServerBind.GetSprite("statusready");
                    mCurServerBind.GetSprite("statusready", ref status);
                    break;
            }
        }

        private void _updateServer(Hashtable tb)
        {
            uint id = uint.Parse(tb["id"].ToString());
            string ip = tb["ip"].ToString();
            ushort port = ushort.Parse(tb["port"].ToString());
            int cstatus = int.Parse(tb["status"].ToString());
            string cname = tb["name"].ToString();

            Logger.LogProcessFormat("[服务器列表] 选择服务器 {0}:{1}, id:{2}", ip, port, id);

            ClientApplication.adminServer.ip    = ip;
            ClientApplication.adminServer.port  = port;
            ClientApplication.adminServer.id    = id;
            ClientApplication.adminServer.state = (eAdminServerStatus)cstatus;
            ClientApplication.adminServer.name  = cname;

            _updateSelectServer(null);
        }

        private enum eLoadingServerState
        {
            None,
            Loading,
            Success,
            Error,
            /// 使用表格里面的配置
            Default,
        }

        private eLoadingServerState mLoadingServerStatus = eLoadingServerState.None;

        private IEnumerator _loadSavedServer()
        {
            bool flag = false;

            mLoadingServerStatus = eLoadingServerState.Loading;
			
#if APPLE_STORE
			WaitNetMessageFrame.TryOpen ();
#endif

			if (PlayerLocalSetting.GetValue("AccountDefault") != null)
			{
#if APPLE_STORE
                int cnt = LoginConfigManager.instance.GetUserDataReconnectCount();
#else
				int cnt = 30;
#endif
				do
				{
					yield return ServerListManager.instance.SendHttpReqCharactorUnit();

					if (ServerListManager.instance.IsAllUserReady())
					{
						uint serverId = 0; 

                        bool isCheckIdMap = false;
                        do
                        {
                            serverId = ClientApplication.adminServer.id;

                            Logger.LogProcessFormat("[服务器列表] 保存的服务器ID {0}", serverId);

                            ArrayList allUser = ServerListManager.instance.allusers;
                            for (int i = 0; i < allUser.Count; ++i)
                            {
                                Hashtable tb = allUser[i] as Hashtable;
                                if (null != tb)
                                {
                                    uint id = uint.Parse(tb["id"].ToString());

                                    if (id == serverId || 0 == serverId)
                                    {
                                        _updateServer(tb);
                                        mLoadingServerStatus = eLoadingServerState.Success;
                                        flag = true;
                                        break;
                                    }
                                }
                            }

                            if (isCheckIdMap)
                            {
                                break;
                            }

                            if (!flag)
                            {
                                isCheckIdMap = true;

                                yield return ServerListManager.instance.SendHttpReqNodeMap((int)serverId);

                                Logger.LogProcessFormat("[服务器列表] 映射的服务器ID {0}", ServerListManager.instance.newServerID);

                                if (ServerListManager.instance.newServerID != -1 &&
                                    ServerListManager.instance.newServerID != (int)serverId)
                                {
                                    ClientApplication.adminServer.id = (uint)ServerListManager.instance.newServerID;

                                    Logger.LogProcessFormat("[服务器列表] 更新服务器ID {0}", ServerListManager.instance.newServerID);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        while ( true );

                        // 假设这里用户列表为空
                        if (!flag)
                        {
                            string param = string.Format("nodes?ids={0}", serverId);

                            WaitHttpRequest lastLoginServer = new WaitHttpRequest(param);

                            yield return lastLoginServer;

                            if (lastLoginServer.GetResult() == BaseWaitHttpRequest.eState.Success)
                            {
                                ArrayList lastLoginServerData = lastLoginServer.GetResultJson();

                                if (lastLoginServerData.Count > 0)
                                {
                                    _updateServer(lastLoginServerData[0] as Hashtable);
                                    mLoadingServerStatus = eLoadingServerState.Success;
                                    flag = true;
#if APPLE_STORE
									WaitNetMessageFrame.TryClose ();
#endif
                                }
                            }
                        }
                    }
                    else 
					{
						yield return Yielders.GetWaitForSeconds(1.5f);
					}
				}
				while (!ServerListManager.instance.IsAllUserReady() && cnt-- > 0);
			}
				
            //if (
            {
                Logger.LogProcessFormat("[服务器列表] 请求Tab");
                yield return ServerListManager.instance.SendHttpReqTab();

                bool hasGetServer = false;

                if (ServerListManager.instance.IsTabsReady())
                {
                    ArrayList tabs = ServerListManager.instance.tabs;

                    Logger.LogProcessFormat("[服务器列表] Tab {0}", tabs.Count);

                    if (tabs.Count > 0)
                    {
                        yield return ServerListManager.instance.SendHttpReqRecommondServer();

                        if (ServerListManager.instance.IsRecommendServerReady())
                        {
                            ArrayList units = ServerListManager.instance.recommendServer;

                            if (units.Count > 0)
                            {
                                Hashtable sertb = null;

                                if (!Global.Settings.isUsingSDK)
                                {
                                    sertb = _GetTableDefaultServerInfo();
                                }
                                else
                                {
                                    int idx = UnityEngine.Random.Range(0, units.Count);

                                    Logger.LogProcessFormat("[服务器列表] 选中 {0}/{1}", idx, units.Count);

                                    sertb = units[idx] as Hashtable;
                                }

                                if (null != sertb)
                                {
                                    hasGetServer = true;

                                    if (!flag)
                                    {
                                        _updateServer(sertb);
                                        mLoadingServerStatus = eLoadingServerState.Success;
#if APPLE_STORE
										WaitNetMessageFrame.TryClose ();
#endif
                                    }
                                }
                            }
                        }
                    }
                }

                if (!hasGetServer && !flag)
                {
				
#if APPLE_STORE
					mLoadingServerStatus = eLoadingServerState.Error;
					SystemNotifyManager.SystemNotify(8935);
#else
					
					mLoadingServerStatus = eLoadingServerState.Default;

                    Hashtable defaultServer = _GetTableDefaultServerInfo();
                    if (defaultServer != null)
                    {
                        _updateServer(defaultServer);
                    }
#endif
                }
				
#if APPLE_STORE
				WaitNetMessageFrame.TryClose ();
#endif
            }
        }

        private Hashtable _GetTableDefaultServerInfo()
        {
            Hashtable table = null;
            DefaultAdminServerTable tb = null;
            if (Global.Settings.isUsingSDK)
            {
                tb = TableManager.instance.GetTableItem<DefaultAdminServerTable>(2);
            }
            else
            {
                tb = TableManager.instance.GetTableItem<DefaultAdminServerTable>(1);
            }

            if (null != tb)
            {
                table = new Hashtable();
                table.Add("id", tb.ServerID);
                table.Add("ip", tb.ServerIP);
                table.Add("port", tb.ServerPort);
                table.Add("name", tb.ServerName);
                table.Add("status", tb.ServerStaus);
            }
            return table;
        }

        private void InitEnterTips()
        {
            if (mClickEnterTips != null)
            {
                string tipText = TR.Value("ComGameEnterTip");

                if (Global.Settings.sdkChannel == SDKChannel.HuaWei)
                {
                    tipText = TR.Value("ComGameEnterTipHuawei");

                    mClickEnterTips.verticalOverflow = VerticalWrapMode.Overflow;
                    mClickEnterTips.alignment = TextAnchor.UpperCenter;
                }

                mClickEnterTips.text = tipText;
            }

            if (mBtnClickEnter)
            {
                mBtnClickEnter.gameObject.CustomActive(true);
            }
        }


        [UIControl("Image (4)")]
        Image mLogoImg;
        private void InitLogoImage()
        {
            if (mLogoImg != null)
            {
#if APPLE_STORE
                if (SDKInterface.instance.NeedHideLoginFrameLogoImg())
                {
                    mLogoImg.CustomActive(false);
                }
                else
                {
                    mLogoImg.CustomActive(true);
                    mLogoImg.SetNativeSize();
                    mLogoImg.transform.position = new Vector2(mLogoImg.transform.position.x, mLogoImg.transform.position.y - 20);
                }
#endif
                string imgPath = PluginManager.GetSDKLogoPath(SDKInterface.SDKLogoType.LoginLogo);
                if (string.IsNullOrEmpty(imgPath))
                    return;
                ETCImageLoader.LoadSprite(ref mLogoImg, imgPath);
            }
        }
		
#region 接收冒险队信息
        [MessageHandle(AdventureTeamInfoSync.MsgID)]
        void OnSyncAdventureTeamInfo(MsgDATA msg)
        {
            if (msg == null)
            {
                return;
            }
            var syncInfo = new AdventureTeamInfoSync();
            syncInfo.decode(msg.bytes);
            ClientApplication.playerinfo.adventureTeamInfo = syncInfo.info;
        }
#endregion
		
#region IOS 提审 - 功能开关
        [MessageHandle(GateNotifySysSwitch.MsgID)]
        void SyncIOSFunctionSwitchRes(MsgDATA data)
        {
            GateNotifySysSwitch ret = new GateNotifySysSwitch();
            ret.decode(data.bytes);

            IOSFunctionSwitchManager.GetInstance().AddClosedFunctions(ret);
        }
#endregion

    }

}
