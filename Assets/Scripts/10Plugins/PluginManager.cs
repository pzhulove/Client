using UnityEngine;
using System.Collections.Generic;
#if BANGCLE_EVERISK
using Assets.BangBangEveRisk;
#endif

public class PluginManager : Singleton<PluginManager>
{
    static string BUGLY_IOS_APPID = "ffa5a11f5e";
    static string BUGLY_ANDROID_APPID = "fac8cca1e6";
    static string BUGLY_ANDROID_MG_TEST = "fc1ccbfe45";
    private bool isSDKInited = false;
    //！第一次开始游戏！
    public static bool isFirstStartGame = true;

    public override void Init()
    {
        //Debug.Log("start baseinit");
        base.Init();
        //原生刘海屏配置初始化
        InitNotch();

        //InitBugly();
        //Debug.Log("start LeBianRequestUpdate");
        LeBianRequestUpdate();
        //Debug.Log("start LeBianIsAfterUpdate");
        //LeBianIsAfterUpdate();
        //InitVoiceChatSDK ();

        //InitServiceAdsPush(SDKCallback.instance.gameObject.name);
#if BANGCLE_EVERISK
        BangBangEveRisk.GetInstance().RegisterServiceEMULATOR();
        //BangBangEveRisk.GetInstance().Init();
#endif
    }

    public void InititalSDK()
    {
        if (!isSDKInited)
        {
            InitSDK();
            isSDKInited = true;
        }
    }

    public bool IsLargeMemoryDevice()
    {
        //大于2G的设备
#if UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID
		int systemMemory = SystemInfo.systemMemorySize;
		return systemMemory > 2000;
#endif

        return true;
    }

    public static bool CheckDeviceMemory()
    {
        if (!GameClient.SwitchFunctionUtility.IsOpen(6))
            return false;

        int systemMemory = SystemInfo.systemMemorySize;
#if UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID


		if (systemMemory <= 512)
		{
			GameClient.SystemNotifyManager.SysNotifyMsgBoxOK("设备内存不足!", ()=>{
				SDKInterface.Instance.Exit();
			});
			return true;
		}
#endif
        return false;
    }

    public static bool IsTargetDevice()
    {
        int systemMemory = SystemInfo.systemMemorySize;
#if UNITY_IPHONE || UNITY_IOS

		if (systemMemory <= 1024)
			return true;
#endif

        return false;
    }

    public static void InitBugly()
    {
        Debug.Log("### Init Bugly!!!!");
#if DEBUG_SETTING
        bool isSDKDebug = Global.Settings.isDebug;
#else
		bool isSDKDebug = false;
#endif
        BuglyAgent.ConfigDebugMode(isSDKDebug);

#if UNITY_IPHONE || UNITY_IOS
		BuglyAgent.InitWithAppId (BUGLY_IOS_APPID);
#elif UNITY_ANDROID
		//BuglyAgent.InitWithAppId (BUGLY_ANDROID_APPID);

#if MG_TEST || MG_TEST2
					   SetBuglyFileVerInfo();
                       BuglyAgent.InitWithAppId (BUGLY_ANDROID_MG_TEST);
#else
						BuglyAgent.InitWithAppId (BUGLY_ANDROID_APPID);
#endif
        
#endif

        //BuglyAgent.RegisterLogCallback ();

        BuglyAgent.EnableExceptionHandler();
    }

    private static readonly string BuglySetUseIdInfoFileName = "bugly_userid_info.conf";
    /// <summary>
    /// bugly上报存的id
    /// </summary>
    /// <param name="versioninfo"></param>
    public static void SetBuglyFileVerInfo()
    {
        string buglyuseridinfo = GetBuglyFileVerInfo();
        if (buglyuseridinfo != "none")
        {
            BuglyAgent.ConfigDefault("", "", buglyuseridinfo, 0);
        }
    }

    public static string GetBuglyFileVerInfo()
    {
        string buglyuseridinfo = "none";
        if (FileArchiveAccessor.LoadFileInPersistentFileArchive(BuglySetUseIdInfoFileName, out buglyuseridinfo))
        {
            try
            {
                Logger.LogErrorFormat("[SDKConfig] 读取 {0} : {1}", BuglySetUseIdInfoFileName, buglyuseridinfo);
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("[SDKConfig] 解析 {0} 失败, {1}", BuglySetUseIdInfoFileName, e.ToString());
            }
        }
        else
        {
            Logger.LogErrorFormat("[SDKConfig] 加载 {0} 失败", BuglySetUseIdInfoFileName);
        }
        return buglyuseridinfo;
    }


    public string BuglySceneInfo = string.Empty;
    private bool _buglyInfoDebug = false;
    private readonly object buglyinfoLock = new object();
    /// <summary>
    /// 传过来存的bugly上报 id bugly只能存100字符
    /// </summary>
    /// <param name="veridinfo"></param>
    public void SetBuglyVerIdInfo(string sceneinfo = "")
    {
#if MG_TEST || MG_TEST2 || MGSPTIYAN
        if (!string.IsNullOrEmpty(sceneinfo))
        {
            BuglySceneInfo = sceneinfo;
        }
        if (!string.IsNullOrEmpty(BuglySceneInfo))
        {
            lock (buglyinfoLock)
            {
                //long totalMemory = System.GC.GetTotalMemory(false);
                string simulatorInfo = IsSimulator() ? GetSimulatorName() : "MP";
                string memoryInfo = string.Format("{0}_{1}", GetAvailMemory(), GetCurrentProcessMemory());
                string info = string.Format("{0}_{1}_{2}", simulatorInfo, memoryInfo, BuglySceneInfo);

                FileArchiveAccessor.SaveFileInPersistentFileArchive(BuglySetUseIdInfoFileName, info);
                if (_buglyInfoDebug)
                {
                    Debug.LogFormat("[PluginManager] - SetBuglyVerIdInfo - info: {0}", info);
                }
            }
        }
#endif
    }

    /// <summary>
    /// 保持屏幕常亮
    /// </summary>
    public static void KeepScreenOn()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        SDKInterfaceAndroid.KeepScreenOn(true);
#elif UNITY_IPHONE || UNITY_IOS
        SDKInterfaceIOS.CommonInit();
#endif

    }


    void InitSDK()
    {
#if DEBUG_SETTING
        bool isSDKDebug = Global.Settings.isDebug;
#else
		bool isSDKDebug = false;

        byte[] data = null;
        string SPEICY_FILE_NAME = "DGameSDK.xml";
        if (FileArchiveAccessor.LoadFileInPersistentFileArchive(SPEICY_FILE_NAME, out data))
        {
            isSDKDebug = true;
        }

#endif

        _buglyInfoDebug = isSDKDebug;

        SDKInterface.Instance.Init(isSDKDebug);

        SDKInterface.Instance.loginCallbackGame = (SDKUserInfo userInfo) =>
        {
            string token = userInfo.token;
            string openuid = userInfo.openUid;
            string ext = userInfo.ext;

            Logger.LogProcessFormat("[Process]login callback token:{0} uid:{1}", token, openuid);
            ClientApplication.playerinfo.state = PlayerState.LOGIN;
            ClientApplication.playerinfo.token = token;
            ClientApplication.playerinfo.openuid = openuid;
            ClientApplication.playerinfo.sdkLoginExt = ext;

            if (Global.Settings.isUsingSDK)
            {
                string newAccount = Global.SDKChannelName[(int)Global.Settings.sdkChannel] + openuid;

                if (SDKChannel.SN79 == Global.Settings.sdkChannel)
                {
                    newAccount = openuid;
                }

                var originAccount = GameClient.PlayerLocalSetting.GetValue("AccountDefault");
                string originAccountName = originAccount == null ? string.Empty : originAccount.ToString();
                if (newAccount != originAccountName)
                {
                    GameClient.PlayerLocalSetting.SetValue("ServerID", null);
                    ClientApplication.adminServer.id = 0;
                }

                GameClient.PlayerLocalSetting.SetValue("AccountDefault", newAccount);
                GameClient.PlayerLocalSetting.SaveConfig();
            }


            SDKInterface.Instance.GetRealNameInfo();
            //新增 SDK登录成功 刷新屏幕适配 
            CameraAspectAdjust.MarkDirty();
        };

        SDKInterface.Instance.logoutCallbackGame = () =>
        {
            Logger.LogProcessFormat("[Process]logout callback");
            ClientApplication.playerinfo.state = PlayerState.LOGOUT;


            var manager = GameClient.ClientSystemManager.GetInstance();
            //if ((manager.CurrentSystem as GameClient.ClientSystemLogin) == null)
            manager._QuitToLoginImpl();

            GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.onSDKLogoutSuccess);

            var currSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem;
            if (currSystem != null)
            {
                if (currSystem is GameClient.ClientSystemLogin)
                {
                    SDKInterface.Instance.Login();
                }
            }
        };

        SDKInterface.Instance.adultCheakcallback = (int flag, int realNameFlag, int age) =>
        {
            if (flag == 0)
            {
                ClientApplication.playerinfo.age = age;
                ClientApplication.playerinfo.authType = (Protocol.AuthIDType)realNameFlag;
            }
            GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.onSDKLoginSuccess);
        };

        SDKInterface.Instance.payResultCallbackGame = (string result) =>
        {
            Logger.LogProcessFormat("[Process]payResultCallbackGame:{0}", result);

            GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnPayResultNotify, result);
        };

        SDKInterface.Instance.bindPhoneCallbackGame = (string bindPhoneNum) =>
        {
            Logger.LogProcessFormat("发送手机绑定验证到服务器 1-sdkCB,{0} = ", bindPhoneNum);
            MobileBind.MobileBindManager.GetInstance().SDKBindPhoneSucc(bindPhoneNum);
        };

        SDKInterface.Instance.smallGameLoadCallbackGame = (string sceneName) =>
        {
            var go = GameObject.Find("UIRoot");
            if (go != null)
                GameObject.DestroyImmediate(go);

            go = GameObject.Find("Environment");
            if (go != null)
                GameObject.DestroyImmediate(go);

            Application.LoadLevel(sceneName);
        };
    }

    public void OpenXYLogin()
    {
        SDKInterface.Instance.Login();
    }

    public void AddNotification(int nID, bool isWeeklyOpen = false)
    {
        var data = TableManager.GetInstance().GetTableItem<ProtoTable.NotificationTable>(nID);
        if (data != null)
        {
            if (isWeeklyOpen)
            {
                if (data.weekday == "0")
                {
                    try
                    {
                        int min = data.minute;
                        int hour = data.hour;
                        string[] rawday = data.day.Split(',');
                        int[] day;
                        string[] rawmonth = data.month.Split(',');
                        int[] month;
                        if (!Str2Int(rawday, out day))
                        {
                            Logger.LogError("day format invaild " + data.day);
                            return;
                        }
                        if (!Str2Int(rawmonth, out month))
                        {
                            Logger.LogError("month format invaild" + data.month);
                            return;
                        }
                        int[] year = null;
                        if (data.year != "0")
                        {
                            string[] rawyear = data.year.Split(',');
                            if (!Str2Int(rawyear, out year))
                            {
                                Logger.LogError("year format invaild" + data.year);
                                return;
                            }
                        }


                        var md = new Assets.SimpleAndroidNotifications.PartyDayModel(min, hour, day, month, year);
                        var deltatime = md.GetNextPartyDay(System.DateTime.Now);
                        if (deltatime == default(System.DateTime))
                        {
                            Logger.LogError("deltatime == default");
                            return;
                        }
                        var tsp_unix = System.Convert.ToInt64((deltatime - System.DateTime.Now).Ticks / 10000);

                        SDKInterface.instance.SetNotificationWithTsp(data.ID, data.Content, data.Content, tsp_unix);
                    }
                    catch (System.Exception e)
                    {
                        Logger.LogError(e.ToString());
                    }

                }
                else
                {
                    string[] strWeeks = data.weekday.Split(',');
                    List<int> weeks = new List<int>();
                    if (strWeeks == null)
                    {
                        Logger.LogErrorFormat("周推送数据有误！！");
                        return;
                    }

                    for (int i = 0; i < strWeeks.Length; i++)
                    {
                        int value;
                        if (System.Int32.TryParse(strWeeks[i], out value))
                        {
                            //1代表星期一转为 1代表星期日
                            value = value + 1;
                            if (value > 7) value = value % 7;

                            weeks.Add(value);
                        }
                        else
                        {
                            Logger.LogProcessFormat("Int32.TryParse could not parse '{0}' to an int.\n", strWeeks[i]);
                        }
                    }
                    for (int i = 0; i < weeks.Count; i++)
                    {
                        SDKInterface.Instance.SetNotificationWeekly(nID, data.Content, "", weeks[i], data.hour, data.minute);
                    }
                }
            }
            else
            {
                SDKInterface.Instance.SetNotification(nID, data.Content, "", data.hour);
            }
        }
    }
    private bool Str2Int(string[] vs, out int[] outints)
    {
        try
        {
            outints = new int[vs.Length];
            for (int i = 0; i < vs.Length; i++)
            {
                outints[i] = System.Convert.ToInt32(vs[i]);
            }
        }
        catch (System.Exception e)
        {
            Logger.LogError(e.ToString());
            outints = new int[0];
            return false;
        }
        return true;
    }
    public void RemoveNotification(int nID)
    {
        SDKInterface.Instance.RemoveNotification(nID);
    }

    public void RemoveAllNotification()
    {
        SDKInterface.Instance.RemoveAllNotification();
    }

    public bool GetAudioAuthorization()
    {
        return SDKInterface.Instance.RequestAudioAuthorization();
    }

    public void InitNotifications()
    {
#if APPLE_STORE
        if (SDKInterface.instance.IsIOSAppstoreLoadSmallGame())
        {
            return;
        }
#endif
        bool isWeeklyOpen = false;
        var clientfunctabe = TableManager.GetInstance().GetTableItem<ProtoTable.SwitchClientFunctionTable>(60);
        if (clientfunctabe != null)
        {
            isWeeklyOpen = clientfunctabe.Open;
        }

        RemoveAllNotification();

        //去掉右上角的数字
        SDKInterface.Instance.ResetBadge();

        var dic = TableManager.GetInstance().GetTable<ProtoTable.NotificationTable>();
        Dictionary<int, object>.Enumerator enumerator = dic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var item = (ProtoTable.NotificationTable)enumerator.Current.Value;
            if (item.NeedClose > 0)
                RemoveNotification(item.ID);
            else
                AddNotification(item.ID, isWeeklyOpen);
        }
    }

    public float GetBatteryPower()
    {
        return SDKInterface.Instance.GetBatteryLevel();
    }

    public bool IsBatteryCharging()
    {
        return SDKInterface.Instance.IsBatteryCharging();
    }

    public string GetSystemTime_HHMM()
    {
        return SDKInterface.Instance.GetSystemTimeHHMM();
    }

    public string GetSystemTimeStamp()
    {
        return SDKInterface.Instance.GetSystemTimeStamp();
    }

    public void SetAudioSessionActive()
    {
        SDKInterface.Instance.SetAudioSessionActive();
    }

    public string LoginVerifyUrl(string serverUrl, string serverId, string openuid, string token, string deviceId, string sdkChannel, string ext)
    {
        string extendUrl = SDKInterface.Instance.ExtendLoginVerifyUrl();
        ext = ext.Replace(" ", "");
        string finalUrl = SDKInterface.Instance.LoginVerifyUrl(serverUrl, serverId, openuid, token, deviceId, sdkChannel, ext) + extendUrl;
        return finalUrl;
    }

    public virtual string FullPackageFetchURL(string serverName, string sdkChannel, string version)
    {
        return string.Format("http://{0}?channel={1}&version={2}", serverName, sdkChannel, version);
    }

    public string GetClipboardText()
    {
        return SDKInterface.Instance.GetClipboardText();
    }

    public void AddKeyboardShowListener(SDKInterface.KeyboardShowOut OnKeyboardres)
    {
        if (OnKeyboardres != null)
        {
            SDKInterface.Instance.keyboardShowCallbackGame = OnKeyboardres;
        }
    }

    public void RemoveKeyboardShowListener()
    {
        SDKInterface.Instance.keyboardShowCallbackGame = null;
    }

    public int GetCurrVersionApi()
    {
        return SDKInterface.Instance.TryGetCurrVersionAPI();
    }

    public bool IsSDKEnableSystemVersion(SDKInterface.FuncSDKType funcSdkType)
    {
        int api = GetCurrVersionApi();
        switch (funcSdkType)
        {
            case SDKInterface.FuncSDKType.FSDK_UniWebView:
                if (api <= 19)
                {
                    return false;
                }
                return true;
        }
        return true;
    }

    public bool IsMGSDKChannel()
    {
        return SDKInterface.Instance.IsMGChannel();
    }

    public static bool NeedChannelHideVersionUpdateProgress()
    {
        return SDKInterface.NeedChannelHideVersionUpdateProgress();
    }

    public void LeBianRequestUpdate()
    {
#if USE_LEBIAN
        SDKInterface.instance.LBRequestUpdate();
        //Logger.LogError("[LBRequestUpdate] !!! ");
#endif
    }

    public void LeBianJudgeLevelAndDownload(int level)
    {
#if USE_LEBIAN
        var data = TableManager.GetInstance().GetTableItem<ProtoTable.SwitchClientFunctionTable>(100);
        if (data != null && data.Open && level >= data.ValueA)
        {
            LeBianDownloadFullRes();
        }
#endif
    }

    public void LeBianDownloadFullRes()
    {
#if USE_LEBIAN
        SDKInterface.instance.LBDownloadFullRes();
#endif

    }

    public bool LeBianIsAfterUpdate()
    {
#if USE_LEBIAN
        return SDKInterface.instance.LBIsAfterUpdate();
#endif
        return false;
    }

    public string LeBianGetFullResPath()
    {
#if USE_LEBIAN
        return SDKInterface.instance.LBGetFullResPath();
#endif

        return string.Empty;
    }

    public void ReportDeviceId()
    {
        if (Global.Settings.sdkChannel == SDKChannel.MG || Global.Settings.sdkChannel == SDKChannel.TestMG)
            SDKInterface.Instance.SendGameServerSystemIMEI();
    }

    public string GetBuildPlatformId()
    {
        return SDKInterface.Instance.GetBuildPlatformId();
    }

    public bool HasVIPAuth()
    {
        return SDKInterface.Instance.HasVIPAuth();
    }

    public bool IsRealNameAuth()
    {
        return SDKInterface.Instance.IsRealNameAuth();
    }

    public string GetOnlineServiceBuildPlatformId()
    {
        return SDKInterface.Instance.GetOnlineServicePlatformId();
    }

    public string GetOnlineServicePlatformName()
    {
        return SDKInterface.Instance.GetOnlineServicePlatformName();
    }

    public string GetChannelName()
    {
        return SDKInterface.Instance.GetPlatformNameByChannel();
    }

    public void TriggerMobileVibrate()
    {
        SDKInterface.Instance.MobileVibrate();
    }

#if BANGCLE_EVERISK
    public bool IsEmulator()
    {
        return BangBangEveRisk.GetInstance().IsEnumlator;
    }

    public string EmulatorType()
    {
        if (string.IsNullOrEmpty(BangBangEveRisk.GetInstance().EmulatorType))
        {
            return "EMULATOR";
        }
        else
        {
            return BangBangEveRisk.GetInstance().EmulatorType;
        }
    }
#endif

    public void ScanFile(string path)
    {
        SDKInterface.Instance.ScanFile(path);
    }

    public void SaveDoc(string content, string parentPathName, string fileName, bool isAppend = true)
    {
        SDKInterface.Instance.SaveDoc(content, parentPathName, fileName, isAppend);
    }

    public string ReadDoc(string parentPathName, string fileName)
    {
        return SDKInterface.Instance.ReadDoc(parentPathName, fileName);
    }

    public float GetCpuTemperature()
    {
        return SDKInterface.Instance.GetCpuTemperature();
    }

    public float GetBatteryTemperature()
    {
        return SDKInterface.Instance.GetBatteryTemperature();
    }

    public void TryGetIOSAppstoreProductIds()
    {
        SDKInterface.Instance.TryGetIOSAppstoreProductIds();
    }

    public string GetAvailMemory()
    {
        return SDKInterface.Instance.GetAvailMemory();
    }

    public string GetCurrentProcessMemory()
    {
        return SDKInterface.Instance.GetCurrentProcessMemory();
    }

    public string GetMonoMemory()
    {
        return SDKInterface.Instance.GetMonoMemory();
    }

    public bool IsSimulator()
    {
        return SDKInterface.Instance.IsSimulator();
    }

    public string GetSimulatorName()
    {
        return SDKInterface.Instance.GetSimulatorName();
    }

    #region 实名认证
    public bool HasSDKAdultCheakAuth()
    {
        return SDKInterface.Instance.HasSDKAdultCheakAuth();
    }


    public bool CanOpenAdultCheakWindow()
    {
        return SDKInterface.Instance.CanOpenAdultCheakWindow();

    }
    public void OpenAdultCheakWindow()
    {
        SDKInterface.Instance.OpenAdultCheakWindow();
    }
    #endregion
    #region 外部调用静态方法

    public static string GetSDKLogoPath(SDKInterface.SDKLogoType sdkLogoType)
    {
        string iconPath = "";
        if (SDKInterface.BeOtherChannel())
        {
            string sdkChannelName = Global.Settings.sdkChannel.ToString();
            string resHeadPath = string.Format("Base/Version/VersionFrame/SDK_Logos/{0}/", sdkChannelName);
            string assetHeadPath = string.Format("UI/Image/Background/SDK_Logos/{0}/", sdkChannelName);
            string versionUpdateName = string.Format("version_update_frame_background_{0}", sdkChannelName);
            string selectRoleName = string.Format("version_update_frame_background_{0}.jpg:version_update_frame_background_{1}", sdkChannelName, sdkChannelName);
            string loginLogoName = string.Format("UI_Beijing_Dixiacheng_{0}.png:UI_Beijing_Dixiacheng_{1}", sdkChannelName, sdkChannelName);
            string loadingLogoName = string.Format("Loading-logo_{0}.png:Loading-logo_{1}", sdkChannelName, sdkChannelName);
            switch (sdkLogoType)
            {
                case SDKInterface.SDKLogoType.VersionUpdate:
                    iconPath = resHeadPath + versionUpdateName;
                    break;
                case SDKInterface.SDKLogoType.SelectRole:
                    iconPath = assetHeadPath + selectRoleName;
                    break;
                case SDKInterface.SDKLogoType.LoginLogo:
                    iconPath = assetHeadPath + loginLogoName;
                    break;
                case SDKInterface.SDKLogoType.LoadingLogo:
                    iconPath = assetHeadPath + loadingLogoName;
                    break;
            }
            if (string.IsNullOrEmpty(iconPath))
            {
                Logger.LogError("GetSDKLogoPath - iconPath is null");
            }
            return iconPath;
        }
        return iconPath;
    }

    #endregion

    #region Push Ads
    public void InitServiceAdsPush(string gameObjectName)
    {
        SDKInterface.Instance.InitServicePush(gameObjectName);
    }

    public void TurnOnServiceAdsPush(bool on)
    {
        if (on)
        {
            SDKInterface.Instance.TurnOnServicePush();
        }
        else
        {
            SDKInterface.Instance.TurnOffServicePush();
        }
    }

    public bool BindOtherNameForServicePush(string alias)
    {
        return SDKInterface.Instance.BindOtherName(alias);
    }

    public bool UnBindOtherNameForServicePush(string alias)
    {
        return SDKInterface.Instance.UnBindOtherName(alias);
    }

    #endregion


    #region 刘海屏
    /// <summary>
    /// Android才有效果
    /// </summary>
    public void InitNotch()
    {
        SDKInterface.Instance.InitNotch();
    }
    /// <summary>
    /// 设备是否存在刘海
    /// </summary>
    /// <param name="debug">在编辑器模拟刘海</param>
    /// <returns></returns>
    public bool HasNotch(bool debug = false)
    {
        return SDKInterface.Instance.HasNotch(debug);
    }


    /// <summary>
    /// 设备的刘海左下点(x y)右上点(z w)，注意会因设备旋转处于左右状态，返回的数值为屏幕分辨率缩放比为1的情况下，若要适配当前分辨率，需要乘以手机原始分辨率与当前分辨率的比值，具体数值在GraphicSettings.OnResolutionChanged
    /// </summary>
    /// <param name="debug">(使用前设置显示分辨率为1920*1080)在编辑器模拟刘海</param>
    /// <param name="screenOrientation">(使用前设置显示分辨率为1920*1080)在编辑器模拟刘海所处位置</param>
    /// <returns>左下点(x y)右上点(z w)</returns>
    public Vector4 GetNotchSize(SDKInterface.NotchDebugType debug = SDKInterface.NotchDebugType.None, ScreenOrientation screenOrientation = ScreenOrientation.LandscapeLeft)
    {
        var n = SDKInterface.Instance.GetNotchSize(debug, screenOrientation);
        if (debug != SDKInterface.NotchDebugType.None)
        {
            n = OpenGL2DirectX(n);
            n = DirectX2UGUI(n);
            n = Resort(n);

            Debug.DrawLine(new Vector3(n[0], n[1]), new Vector3(n[2], n[3]), Color.green);
            Debug.DrawLine(new Vector3(n[0], n[3]), new Vector3(n[2], n[1]), Color.blue);
            return new Vector4(n[0], n[1], n[2], n[3]);
        }
        if (n.Length == 2)//SDK24-28厂商提供的接口只返回刘海长宽，无位置信息
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
            {
                //转换成左刘海矩形
                n = new int[4] { 0, (Screen.height + n[1]) / 2, n[0], (Screen.height - n[1]) / 2 };
                //转换坐标系到UGUI坐标系
                n = DirectX2UGUI(OpenGL2DirectX(n));
                //重新排序，使x y 为左下点坐标，z w 为右上点坐标
                n = Resort(n);
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                //转换成右刘海矩形
                n = new int[4] { Screen.width - n[0], (Screen.height + n[1]) / 2, Screen.width, (Screen.height - n[1]) / 2 };
                //转换坐标系到UGUI坐标系
                n = DirectX2UGUI(OpenGL2DirectX(n));
                //重新排序，使x y 为左下点坐标，z w 为右上点坐标
                n = Resort(n);
            }
        }
        else if (n.Length == 4)//SDK28及以上会返回刘海所在矩形
        {
            n = DirectX2UGUI(OpenGL2DirectX(n));
            n = Resort(n);
        }
        else
        {
            return new Vector4(0, 0, 0, 0);
        }

        return new Vector4(n[0], n[1], n[2], n[3]);
    }
    private int[] OpenGL2DirectX(int[] n)
    {
        return new int[4] { n[0], Screen.height - n[1], n[2], Screen.height - n[3] };
    }
    private int[] DirectX2UGUI(int[] n)
    {
        return new int[4] { n[0] - Screen.width / 2, n[1] - Screen.height / 2, n[2] - Screen.width / 2, n[3] - Screen.height / 2 };

    }
    private int[] Resort(int[] n)
    {
        int tmp = 0;
        if (n[0] > n[2])
        {
            tmp = n[0];
            n[0] = n[2];
            n[2] = tmp;
        }

        if (n[1] > n[3])
        {
            tmp = n[1];
            n[1] = n[3];
            n[3] = tmp;
        }
        return n;
    }
    #endregion
}
