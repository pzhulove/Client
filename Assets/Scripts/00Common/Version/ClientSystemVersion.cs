using UnityEngine;
using System.Collections;


namespace GameClient
{
    class ClientSystemVersion : ClientSystem
    {
        enum VersionCheckState
        {
            NONE,
            INIT_STATE,
            HOT_FIX,
            DONE
        };

        new VersionCheckState mState = VersionCheckState.NONE;

        float showVersionInfoSecond = 3f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public sealed override string GetMainUIPrefabName()
        {
            return "Base/Version/VersionFrame/ClientSystemVersionMainUI";
        }
        protected sealed override string _GetLevelName()
        {
            return "Start";
        }

        protected void InitHotFix()
        {
            VersionManager.instance.m_ProceedHotUpdate = true;
        }

        protected IEnumerator _CheckUpdate()
        {

//分包不在最开始提示保底方案下载整包
#if UNITY_ANDROID && !USE_SMALLPACKAGE
            //if (Client.AppPackageFetcher.NeedFetchAppPackage())
            //{/// 如果打开拉取完整包配置
            //    if (Client.AppPackageFetcher.InitNativePackageVersion())
            //    {
            //        yield return Client.AppPackageFetcher.FetchFullAppPackage();
            //        if(Client.AppPackageFetcher.IsVersionValid())
            //        {
            //            if(Client.AppPackageFetcher.IsRemoteNewer())
            //            {/// 远端版本较新，弹框提示用户下载新包
            //                bool block = true;
            //                SystemNotifyManager.BaseMsgBoxOkCancel(ClientConfig.AppPackageMessageAppVersionLow,
            //                () =>
            //                {
            //                    block = false;
            //                },
            //                () =>
            //                {
            //                    Client.AppPackageFetcher.OpenAppPackageURL();
            //                    block = false;
            //                },
            //                ClientConfig.AppPackageButTextRetryUpdate,
            //                ClientConfig.AppPackageButTextOpenURL
            //                );

            //                while (block)
            //                {
            //                    yield return Yielders.EndOfFrame;
            //                }
            //            }
            //        }

            //    }
            //}
            //Client.AppPackageFetcher.SkipFetchAgain(false);
#endif


            SplashLoadingFrame splashFrame = ClientSystemManager.GetInstance().GetFrame(typeof(SplashLoadingFrame)) as SplashLoadingFrame;
            if (splashFrame != null)
            {
                while(splashFrame.fadeFinish != true)
                {
                    yield return Yielders.EndOfFrame;
                }
            }

            if (PluginManager.isFirstStartGame)
            {
                if (CheckIsShowSplash())
                {
                    StartSplashFrame startSplashFrame = ClientSystemManager.GetInstance().OpenFrame<StartSplashFrame>(FrameLayer.Bottom) as StartSplashFrame;
                    if (startSplashFrame != null)
                    {
                        while (startSplashFrame.IsSplashDone == false)
                        {
                            yield return Yielders.EndOfFrame;
                        }
                    }
                }
                PluginManager.isFirstStartGame = false;
            }

            VersionUpdateFrame versionFrame = null;
            if (!ClientSystemManager.GetInstance().IsFrameOpen<VersionUpdateFrame>())
            {
                versionFrame = ClientSystemManager.instance.OpenFrame<VersionUpdateFrame>(FrameLayer.Bottom) as VersionUpdateFrame;

#if APPLE_STORE
                string info = "正在初始化本地资源（不消耗流量）...";                
                if (versionFrame != null && PluginManager.NeedChannelHideVersionUpdateProgress())
                {
                    versionFrame.SetSliderColorAlpha(0f);                
                }
#else
                string info = "正在初始化..";
#endif
                versionFrame.UpdateProgressState(info);
            }

            if (Global.Settings.enableHotFix)
            {
                HotUpdateDownloader.instance.DoDeleteExpirePackage();
                HotUpdateDownloader.instance.CheckHotUpdateVersion();
                while (VersionCheckResult.None == HotUpdateDownloader.instance.GetVersionCheckRes())
                {
                    yield return Yielders.EndOfFrame;
                }
                
                if (VersionCheckResult.NeedHotUpdate == HotUpdateDownloader.instance.GetVersionCheckRes())
                {
#if APPLE_STORE
                    if (versionFrame != null  && PluginManager.NeedChannelHideVersionUpdateProgress())
                    {
                        versionFrame.SetSliderColorAlpha(1f);
                    }
#endif

                    mState = VersionCheckState.HOT_FIX;
                    string info = "正在链接版本服务器..";
                    versionFrame.UpdateProgressState(info);
                    HotUpdateDownloader.instance.DoHotUpdate(versionFrame);
                
                    while (VersionUpdateState.FinishUpdate != HotUpdateDownloader.instance.updateState)
                    {
                        yield return Yielders.EndOfFrame;
                    }

#if APPLE_STORE
					if (VersionCheckResult.NeedUpGrade == HotUpdateDownloader.instance.GetVersionCheckRes ()) 
					{
						// 等到地老天荒=。=的强更流程啊啊啊 
						while (true) yield return null;
					}
#endif
                    /// 释放之前的数据
                    /// 
                    GameFrameWork.instance.DeinitLogicModule();
                }
                
                // 临时的强更流程
                // else if (VersionCheckResult.Lastest == HotUpdateDownloader.instance.GetVersionCheckRes())
                // {
                //     yield return HotUpdateDownloader.instance.ForceFullUpdate();
                //     while (VersionUpdateState.FinishUpdate != HotUpdateDownloader.instance.updateState)
                //     {
                //         yield return Yielders.EndOfFrame;
                //     }
                // }
            }

            versionFrame.ResetProgress("初始化...");

            mState = VersionCheckState.INIT_STATE;
            yield return GameFrameWork.instance._gameDataInitCoroutine(
                (process,text)=>{
                    versionFrame.UpdateProgress(process,text);
                }
            );

            mState = VersionCheckState.DONE;

			if (PluginManager.CheckDeviceMemory())
			{
				yield break;
			}

			AdsPush.LoginPushManager.GetInstance().Init();
            ServerAddressManager.GetInstance().GetServerList();

			SDKCallback.instance.StartScreenSave();

#if !APPLE_STORE
            PluginManager.GetInstance().InititalSDK();
#endif

            PluginManager.GetInstance().InitNotifications();
            
            FrameConfigManager.instance.LoadFrameConfig();
            SystemConfig.instance.LoadConfig();

#if APPLE_STORE
            LoginConfigManager.instance.LoadBaseLoginConfig();
#endif
            GameClient.ActivityDungeonPersistentDataManager.instance.LoadData();
            
            ClientSystemManager.GetInstance().SwitchSystem<ClientSystemLogin>();

            yield break;
        }

        public sealed override void OnEnter()
        {
            base.OnEnter();
            if(Global.Settings.enableHotFix == true)
            {
                InitHotFix();
            }

//             if (!SDKInterface.IsNewSDKChannelPay())
//             {
//                 PluginManager.isFirstStartGame = false;
//             }
            
            GameFrameWork.instance.StartCoroutine(_CheckUpdate());
        }

        // 销毁
        public sealed override void OnExit()
        {
            ClientSystemManager.instance.CloseFrame<VersionUpdateFrame>();
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
            /*
            if( mState == VersionCheckState.DONE )
            {
                SplashLoadingFrame splashFrame = ClientSystemManager.GetInstance().GetFrame(typeof(SplashLoadingFrame)) as SplashLoadingFrame;
                if (splashFrame.fadeFinish)
                    ClientSystemManager.GetInstance().SwitchSystem<ClientSystemLogin>();
            }
            */
        }

        //是否显示额外闪屏页
        private bool CheckIsShowSplash()
        {
            bool isShow = true;
            if (Global.Settings.sdkChannel == SDKChannel.M915 || 
                Global.Settings.sdkChannel == SDKChannel.JUNHAI)
            {
                isShow = false;
            }
            return isShow;
        }
    }
}
