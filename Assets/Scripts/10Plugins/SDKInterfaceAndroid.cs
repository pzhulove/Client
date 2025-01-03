using UnityEngine;
using System.Collections;
using System;
using Assets.SimpleAndroidNotifications;


public class SDKInterfaceAndroid : SDKInterface
{

    private const string PERMISSION_WRITE = "android.permission.WRITE_EXTERNAL_STORAGE";

    protected AndroidJavaObject currentActivity;
    protected AndroidJavaObject GetActivity()
    {
        if (currentActivity == null)
        {
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        }

        return currentActivity;
    }

    protected AndroidJavaObject applicationContext;

    protected AndroidJavaObject GetContext()
    {
        if (applicationContext == null)
        {
            applicationContext = GetActivity().Call<AndroidJavaObject>("getApplicationContext");
        }
        return applicationContext;
    }

    protected AndroidJavaObject lebianJavaObj;
    protected AndroidJavaObject GetLebianJavaObject()
    {
        if (lebianJavaObj == null)
        {
            lebianJavaObj = new AndroidJavaObject("com.excelliance.open.LeBianSDKImpl");
            lebianJavaObj.CallStatic("SetCommonContext", GetActivity());
        }
        return lebianJavaObj;
    }

    protected AndroidJavaObject commonJavaObj;
    protected AndroidJavaObject GetCommonJavaObject()
    {
        if (commonJavaObj == null)
        {
            commonJavaObj = new AndroidJavaObject("com.tm.commonlib.AndroidCommonImpl");
            commonJavaObj.CallStatic("SetCommonContext", GetActivity());
        }
        return commonJavaObj;
    }

    #region 安卓原生方法 java工程common
    public override float GetBatteryLevel()
    {
        return GetCommonJavaObject().CallStatic<float>("GetBatteryLevel");
        /*
                try{
                    int level = 0;
                    string capacityStr = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
                    if(int.TryParse(capacityStr,out level))
                       return level+0.00f;
                    else
                       return -1.00f;
                }catch(Exception e)
                {
                     Logger.LogError("Failed to read battery power txt in android"+e.Message);
                }
                return -1.00f
        */
    }

    public override bool IsBatteryCharging()
    {
        return GetCommonJavaObject().CallStatic<bool>("IsBatteryCharging");
    }


    public override string GetClipboardText()
    {
        return GetCommonJavaObject().CallStatic<string>("GetClipboardText");
    }

    public override string GetSystemIMEI()
    {
        string androidIMEI = "";
        try
        {
            androidIMEI = GetCommonJavaObject().CallStatic<string>("GetSystemIMEI");
        }
        catch (Exception e)
        {
            //Logger.LogError("can not find the method 'GetSystemIMEI' in Android Common Jar");
        }
        return androidIMEI;
    }
    public static void KeepScreenOn(bool isOn)
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            if (currentActivity != null)
            {
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject jo = currentActivity.Call<AndroidJavaObject>("getWindow");
                    if (jo == null)
                        return;
                    if (isOn)
                    {
                        jo.Call("addFlags", 128);
                    }
                    else
                    {
                        jo.Call("clearFlags", 128);
                    }
                }));
            }
        }
        catch (Exception e)
        {
            //Logger.LogError("try KeepScreenOn failed :"+e.ToString());
        }
#endif
    }


    // 2.0没有
    public override float GetCpuTemperature()
    {
        return 0f;
        // 先注释掉
        //try
        //{
        //    return GetCommonJavaObject().CallStatic<float>("GetCpuTemperature");
        //}
        //catch (Exception e)
        //{
        //    Logger.LogError("GetCpuTemperature falied!" + e.ToString());
        //    return base.GetCpuTemperature();
        //}

    }
    // 2.0没有
    public override float GetBatteryTemperature()
    {
        return 0f;
        // 先注释掉
        //try
        //{
        //    //return GetCommonJavaObject().GetStatic<int>("mtemperature");
        //    return GetCommonJavaObject().CallStatic<float>("GetBatteryTemp");
        //}
        //catch (Exception e)
        //{
        //    Logger.LogError("GetBatteryTemperature falied!" + e.ToString());
        //    return base.GetBatteryTemperature();
        //}

    }

    public override string GetAvailMemory()
    {
        try
        {
            return GetCommonJavaObject().CallStatic<string>("GetAvailMemory");
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("[SDKInterfaceAndroid] - GetAvailMemory falied!, {0}", e.ToString());
            return base.GetAvailMemory();
        }
    }
    public override string GetCurrentProcessMemory()
    {
        try
        {
            return GetCommonJavaObject().CallStatic<string>("GetCurrentProcessMemory");
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("[SDKInterfaceAndroid] - GetCurrentProcessMemory falied!, {0}", e.ToString());
            return base.GetCurrentProcessMemory();
        }
    }

    public override bool IsSimulator()
    {
        try
        {
            return GetCommonJavaObject().CallStatic<bool>("isSimulator");
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("[SDKInterfaceAndroid] - check IsSimulator falied!, {0}", e.ToString());
            return base.IsSimulator();
        }
    }

    public override string GetSimulatorName()
    {
        try
        {
            return GetCommonJavaObject().CallStatic<string>("getSimulatorName");
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("[SDKInterfaceAndroid] - GetSimulatorName falied!, {0}", e.ToString());
            return base.GetSimulatorName();
        }
    }


    #endregion


    #region UniWebView Util in Android

    public override int GetKeyboardSize()
    {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = null;

            try
            {
                if (UnityClass == null)
                    return 0;
                AndroidJavaObject activity = UnityClass.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity == null)
                    return 0;
                AndroidJavaObject unityPlayer = activity.Get<AndroidJavaObject>("mUnityPlayer");
                if (unityPlayer == null)
                    return 0;
                View = unityPlayer.Call<AndroidJavaObject>("getView");
            }
            catch (Exception e)
            {
                //Logger.LogError("try GetUnityPlayerView in Android failed : "+e.ToString());
                return 0;
            }

            if (View == null)
                return 0;

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                if (Rct == null)
                    return 0;
                try
                {
                    View.Call("getWindowVisibleDisplayFrame", Rct);
                }
                catch (Exception e)
                {
                    //Logger.LogError("Platform Android GetKeyboardSize failed :"+e.ToString());
                    return 0;
                }
                return Screen.height - Rct.Call<int>("height");
            }
        }
    }

    public override int TryGetCurrVersionAPI()
    {
        string op = SystemInfo.operatingSystem;
        //Logger.LogProcessFormat("operating system api info : {0}", op);

        int res = 0;
        try
        {
            string headPlat = "Android OS";
            string headStr = "API-";
            int headStrLength = headStr.Length;
            int apiCount = 2;

            if (op.Contains(headPlat))
            {
                if (op.Contains(headStr) == false)
                    return res;
                int index = op.IndexOf(headStr);

                //Logger.LogProcessFormat("try index = {0}", index);

                string api = op.Substring(index + headStrLength, apiCount);

                //Logger.LogProcessFormat("try api = {0}", api);

                if (int.TryParse(api, out res))
                {
                    return res;
                }
            }

            return res;

        }
        catch (Exception e)
        {
            //Logger.LogErrorFormat("Get Android Api error : {0}",e.ToString());
            return res;
        }

        //int sdkLevel = 0;
        //try
        //{
        //    var clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
        //    if (clazz != null)
        //    {
        //        var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
        //        sdkLevel = AndroidJNI.GetStaticIntField(clazz, fieldID);
        //    }
        //    return sdkLevel;
        //}
        //catch (Exception e)
        //{
        //    Logger.LogError("TryGetCurrVersionAPI error : " + e.ToString());
        //    return sdkLevel;
        //}
    }

    #endregion
    public override string GetSystemTimeHHMM()
    {
        return base.GetSystemTimeHHMM();
    }
    public override bool RequestAudioAuthorization()
    {
        return true;
    }

    public override void QuitApplicationOnEsc()
    {
        if (GameClient.ClientSystemManager.GetInstance().CurrentSystem == null)
            return;
        if (GameClient.ClientSystemManager.GetInstance().CurrentSystem is GameClient.ClientSystemLogin)
        {
            if ((Input.GetKeyDown(KeyCode.Escape)))
            {
                GameClient.SystemNotifyManager.SystemNotifyOkCancel(8519,
                    () =>
                    {
                        Application.Quit();
                    },
                    () =>
                    {
                    }
                );
            }
        }
    }

    public override void InitServicePush(string gameObjectName)
    {
        //GTPush.GTPushBinding.initPush(gameObjectName);
        //GTPush.GTPushBinding.setPushMode(true);
    }
    public override void TurnOnServicePush()
    {
        //GTPush.GTPushBinding.turnOnPush();
    }
    public override void TurnOffServicePush()
    {
        //GTPush.GTPushBinding.turnOffPush();
    }

    public override bool BindOtherName(string alias)
    {
        return true;
        //return GTPush.GTPushBinding.bindAlias(alias);
    }

    public override bool UnBindOtherName(string alias)
    {
        return true;
        //return GTPush.GTPushBinding.unBindAlias(alias);
    }

    public override void SetNotification(int nid, string content, string title, int hour)
    {
        string MainActivityClassName = "";
        var notificationParams = new NotificationParams
        {
            Id = nid,
            Delay = System.TimeSpan.FromSeconds(1),
            Title = title,
            Message = content,
            Ticker = "通知",
            Sound = true,
            Vibrate = true,
            Light = true,
            SmallIcon = NotificationIcon.Bell,
            SmallIconColor = new Color(0, 0.5f, 0),
            LargeIcon = "app_icon"
        };

        MainActivityClassName = "com.tm.dnl15.MainActivity";


        if (string.IsNullOrEmpty(MainActivityClassName))
            return;

        NotificationManager.SetCustomHour(hour);

        NotificationManager.SetIntentActivityForSDK(MainActivityClassName);
        NotificationManager.SendCustom3(notificationParams);
    }

    public override void SetNotificationWeekly(int nid, string content, string title, int weekday, int hour, int minute)
    {
        string MainActivityClassName = "";
        var notificationParamsWeekly = new NotificationParams
        {
            Id = ResetNidWeekly(nid, weekday),
            Delay = new TimeSpan(),
            Title = title,
            Message = content,
            Ticker = "通知",
            Sound = true,
            Vibrate = true,
            Light = true,
            SmallIcon = NotificationIcon.Bell,
            SmallIconColor = new Color(0, 0.5f, 0),
            LargeIcon = "app_icon"
        };

        MainActivityClassName = "com.tm.dnl15.MainActivity";

        if (string.IsNullOrEmpty(MainActivityClassName))
        {
            return;
        }

        NotificationManager.SetCustomWeekly(weekday, hour, minute);
        NotificationManager.SetIntentActivityForSDK(MainActivityClassName);
        NotificationManager.SendCustomWeekly(notificationParamsWeekly);
    }
    public override void SetNotificationWithTsp(int nid, string content, string title, long tsp_unix)
    {
        string MainActivityClassName = "";
        var notificationParamsWeekly = new NotificationParams
        {
            Id = nid,
            Delay = new TimeSpan(tsp_unix * 10000),
            Title = title,
            Message = content,
            Ticker = "通知",
            Sound = true,
            Vibrate = true,
            Light = true,
            SmallIcon = NotificationIcon.Bell,
            SmallIconColor = new Color(0, 0.5f, 0),
            LargeIcon = "app_icon"
        };

        MainActivityClassName = "com.tm.dnl15.MainActivity";
        if (string.IsNullOrEmpty(MainActivityClassName))
        {
            return;
        }
        NotificationManager.SetIntentActivityForSDK(MainActivityClassName);
        NotificationManager.SendCustom2(notificationParamsWeekly);
    }

    private int ResetNidWeekly(int nid, int weekly)
    {
        return nid * 10 + 100000 + weekly;
    }

    public override void RemoveNotification(int nid)
    {
        NotificationManager.Cancel(nid);
        for (int i = 1; i <= 7; i++)
        {
            NotificationManager.Cancel(ResetNidWeekly(nid, i));
        }
    }

    public override void RemoveAllNotification()
    {
        NotificationManager.CancelAll();
    }

    public override string GetBuildPlatformId()
    {
        return TR.Value("zymg_plat_id_android");
    }

    public override string GetOnlineServicePlatformId()
    {
        return TR.Value("zymg_onlineservice_plat_id_android");
    }

    public override string GetOnlineServicePlatformName()
    {
        return TR.Value("zymg_onlineservice_plat_name_android");
    }

    public override void MobileVibrate()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// 动态获取android权限 暂时用不上
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    public bool RequestPermission(string permission)
    {
        using (AndroidJavaClass ContextCompat = new AndroidJavaClass("android.support.v4.content.ContextCompat"))
        {
            if (ContextCompat.CallStatic<int>("checkSelfPermission", GetContext(), permission) != 0)
            {
                using (AndroidJavaClass ActivityCompat = new AndroidJavaClass("android.support.v4.app.ActivityCompat"))
                {
                    ActivityCompat.CallStatic("requestPermissions", GetActivity(), new string[] { permission }, 100);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// 使图片显示在相册
    /// </summary>
    /// <param name="path">图片路径</param>
    public override void ScanFile(string path)
    {
        using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", GetContext(), null))
        {
            Conn.CallStatic("scanFile", GetContext(), new string[] { path }, null, null);
        }
    }

    public override void SaveDoc(string content, string parentPathName, string fileName, bool isAppend = true)
    {
        if (string.IsNullOrEmpty(content))
        {
            Logger.LogError("SaveDoc failed, content is null !");
            return;
        }
        if (string.IsNullOrEmpty(parentPathName))
        {
            Logger.LogError("SaveDoc failed, parentPathName is null !");
            return;
        }
        if (string.IsNullOrEmpty(fileName))
        {
            Logger.LogError("SaveDoc failed, fileName is null !");
            return;
        }
        try
        {
            GetCommonJavaObject().CallStatic("SaveDoc", content, parentPathName, fileName, isAppend);
        }
        catch (Exception e)
        {
            Logger.LogError("SaveDoc failed" + e.ToString());
        }
    }

    public override string ReadDoc(string parentPathName, string fileName)
    {
        if (string.IsNullOrEmpty(parentPathName))
        {
            Logger.LogError("ReadDoc failed, parentPathName is null !");
            return base.ReadDoc(parentPathName, fileName);
        }
        if (string.IsNullOrEmpty(fileName))
        {
            Logger.LogError("ReadDoc failed, fileName is null !");
            return base.ReadDoc(parentPathName, fileName);
        }
        try
        {
            return GetCommonJavaObject().CallStatic<string>("ReadFile", parentPathName, fileName);
        }
        catch (Exception e)
        {
            Logger.LogError("SaveDoc failed" + e.ToString());
            return base.ReadDoc(parentPathName, fileName);
        }
    }


    #region 实名认证
    public override bool HasSDKAdultCheakAuth()
    {
        try
        {
            return GetActivity().Call<bool>("HasAdultCheakAuth");
        }
        catch (Exception e)
        {
            Logger.LogError("HasSDKAdultCheakAuth falied!" + e.ToString());
            return false;
        }
    }
    public override void GetRealNameInfo()
    {
        try
        {
            //没开游戏实名就直接给他玩
            if (!IsRealNameAuth())
            {
                SDKCallback.instance.OnAdultCheakRet("0" + "," + ((int)(Protocol.AuthIDType.AUTH_ADULT)).ToString() + "," + "0");
                return;
            }

            //sdk没实名通道就直接给他玩
            if (!HasSDKAdultCheakAuth())
            {
                //华为依赖外部华为移动应用服务
                if (Global.Settings.sdkChannel == SDKChannel.HuaWei)
                {
                    GameClient.SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("download_huawei_service"));
                    SDKCallback.instance.OnAdultCheakRet("0" + "," + ((int)(Protocol.AuthIDType.AUTH_NO_ID)).ToString() + "," + "0");
                    return;
                }
                SDKCallback.instance.OnAdultCheakRet("0" + "," + ((int)(Protocol.AuthIDType.AUTH_ADULT)).ToString() + "," + "0");
                return;
            }

            GetActivity().Call("GetRealNameInfo");
        }
        catch (Exception e)
        {
            Logger.LogError("GetRealNameInfo falied!" + e.ToString());
        }
    }
    public override bool CanOpenAdultCheakWindow()
    {
        try
        {

            return GetActivity().Call<bool>("CanOpenAdultCheakWindow");
        }
        catch (Exception e)
        {
            Logger.LogError("CanOpenAdultCheakWindow falied!" + e.ToString());
            return false;
        }
    }
    public override void OpenAdultCheakWindow()
    {
        try
        {
            if (!CanOpenAdultCheakWindow())
                return;
            GetActivity().Call("OpenAdultCheakWindow");
        }
        catch (Exception e)
        {
            Logger.LogError("OpenAdultCheakWindow falied!" + e.ToString());
        }
    }
    #endregion

    #region LeBian sdk

    //老乐变
    public override void LBRequestUpdate()
    {
        try
        {
            GetLebianJavaObject().CallStatic("RequestUpdate");
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("[SDKInterfaceAndroid] - LBRequestUpdate invoke failed : {0}", e.ToString());
        }
    }

    public override void LBDownloadFullRes()
    {
#if USE_SMALLPACKAGE
        GetLebianJavaObject().CallStatic("DownloadFullResNotify");
#endif
    }

    //这个接口没调到
    public override bool LBIsAfterUpdate()
    {
        bool isAfterUpdate = false;
#if USE_SMALLPACKAGE
        try
        {
            isAfterUpdate = GetLebianJavaObject().CallStatic<bool>("IsAfterUpdate");
            Debug.Log("LBIsAfterUpdate return value --->>> " + isAfterUpdate);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
#endif
        return isAfterUpdate;
    }

    public override string LBGetFullResPath()
    {
        string fullResPath = "";
#if USE_SMALLPACKAGE
        try
        {
            fullResPath = GetLebianJavaObject().CallStatic<string>("GetFullResPath");
            Debug.Log("LBGetFullResPath return value --->>> " + fullResPath);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
#endif
        return fullResPath;
    }


    protected AndroidJavaObject lebianSMApi;
    protected AndroidJavaObject GetLebianSMObject()
    {
#if USE_SMALLPACKAGE
        if (lebianSMApi == null)
        {
            lebianSMApi = new AndroidJavaObject("com.excelliance.lebianapi.LebianApi");
        }
        return lebianSMApi;
#endif
        return lebianSMApi;
    }

    bool isResourceDownloaded = false;
    public override bool IsResourceDownloadFinished()
    {
        bool ret = false;
#if USE_SMALLPACKAGE

        if (isResourceDownloaded)
            ret = true;
        else
        {
            if (SDKCallback.instance.CanRead)
            {
                SDKCallback.instance.CanRead = false;
                var javaObj = GetLebianSMObject();
                if (javaObj != null)
                {
                    ret = javaObj.CallStatic<bool>("isDownloadFinished", GetContext());
                    if (ret)
                        isResourceDownloaded = true;
                }
            }
        }

        
#endif
        return ret;
    }

    bool firstCheckIsSmallPackage = true;
    bool isSmallPackage = false;
    public override bool IsSmallPackage()
    {
        bool ret = false;
#if USE_SMALLPACKAGE
        if (!firstCheckIsSmallPackage)
            ret = isSmallPackage;
        else
        {
            firstCheckIsSmallPackage = false;
            var javaObj = GetLebianSMObject();
            if (javaObj != null)
            {
                ret = javaObj.CallStatic<bool>("isSmallPkg", GetContext());
                isSmallPackage = ret;
            }
        }

        
#endif
        return ret;
    }

    public override long GetResourceDownloadedSize()
    {
        long ret = 0;
#if USE_SMALLPACKAGE
        var javaObj = GetLebianSMObject();
        if (javaObj != null)
        {
            ret = javaObj.CallStatic<long>("getCurrentDlSize", GetContext());
        }
#endif
        return ret;
    }

    public override long GetTotalResourceDownloadSize()
    {
        long ret = 0;
#if USE_SMALLPACKAGE
        var javaObj = GetLebianSMObject();
        if (javaObj != null)
        {
            ret = javaObj.CallStatic<long>("getTotalSize", GetContext());
        }
#endif
        return ret;
    }

    public override void OpenDownload()
    {
#if USE_SMALLPACKAGE
        var javaObj = GetLebianSMObject();
        if (javaObj != null)
        {
            javaObj.CallStatic("openDownload", GetContext());
            isDownload = true;
        }
#endif
    }

    public override void CloseDownload()
    {
#if USE_SMALLPACKAGE
        var javaObj = GetLebianSMObject();
        if (javaObj != null)
        {
            javaObj.CallStatic("closeDownload", GetContext());
            isDownload = false;
        }
#endif
    }

    bool isDownload = false;
    public override bool IsDownload()
    {
        return isDownload;
    }

    #endregion

    #region 刘海屏
    public override void InitNotch()
    {
        try
        {
            GetCommonJavaObject().CallStatic("initNotch",GetActivity());
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("error : {0}", e.ToString());
        }
    }
    public override bool HasNotch(bool debug = false)
    {
        if (debug)
            return base.HasNotch(debug);

        try
        {
            return GetCommonJavaObject().CallStatic<bool>("hasNotch");
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("error : {0}", e.ToString());
            return base.HasNotch(debug);
        }
    }
    public override int[] GetNotchSize(NotchDebugType debug = NotchDebugType.None, ScreenOrientation screenOrientation = ScreenOrientation.LandscapeLeft)
    {
        if (debug != NotchDebugType.None)
            return base.GetNotchSize(debug, screenOrientation);
        try
        {
            var i = GetCommonJavaObject().CallStatic<int[]>("getNotchsize");
            return i;
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("error : {0}", e.ToString());
            return base.GetNotchSize(debug);
        }

    }

    #endregion
}
