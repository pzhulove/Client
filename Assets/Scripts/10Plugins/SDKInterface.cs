using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using SDKClient;



/// <summary>
/// sdk 支付接口信息
/// </summary>
public class SDKPayInfo
{
    public string requestId;                //本地构建的订单号
    public string price;
    public string mallType;                 //商城类型
    public string productId;                //商品ID
    public string productName;              //商品名称
    public string productShortName;         //商品其他名称
    public string productDesc;              //商品描述
    public string productSDKId;             //商品在SDK设置的Id，苹果是内购档位最后一位字段（前面是bundleid），像酷派联想就是后台配置的id
    public string extra;                    //支付的透传 

    public string serverId;
    public string serverName;
    public string roleId;
    public string roleName;

    /// <summary>
    /// 账号openuid
    /// </summary>
    public string openUid;
    public string payCallbackUrl;           //支付回调地址
}
public class SDKReportRoleInfo
{
    public SDKInterface.ReportSceneType sceneType;
    protected uint serverId;
    protected string serverName;
    protected string roleId;
    protected string roleName;
    protected int roleLevel;
    protected int jobId;
    protected string jobName;
    protected int vipLevel;
    protected int couponNum;
}

public class SDKLoginVerifyUrl
{
    public string serverUrl;
    public string serverId;
    public string channelParam;
    public string ext;
}

/// <summary>
/// Apk 信息
/// </summary>
public class ApkInfo
{
    public string appName;
    public string companyName;
    public string appBundleId;
}

/// <summary>
/// sdk 获取的用户信息
/// </summary>
public class SDKUserInfo
{
    public string openUid;
    public string token;
    public string ext;
}

/// <summary>
/// 设备 信息
/// </summary>
public class SDKDeviceInfo
{
    public string deviceId;
    public string modeltype;
    public string osInfo;
}
public class SDKInterface
{
    public enum ReportSceneType
    {
        None = 0,
        Login = 1,
        CreateRole = 2,
        LevelUp = 3,
        Logout = 4,
    }
    protected static SDKChannelInfo m_SDKChannelInfo = new SDKChannelInfo();
    public static SDKChannelInfo SDKChannelInfo
    {
        get
        {
            if (m_SDKChannelInfo == null)
            {
                m_SDKChannelInfo = new SDKChannelInfo();
            }
            return m_SDKChannelInfo;
        }
    }
    protected SDKDeviceInfo m_SDKDeviceInfo = new SDKDeviceInfo();
    public SDKDeviceInfo SDKDeviceInfo
    {
        get
        {
            if (m_SDKDeviceInfo == null)
            {
                m_SDKDeviceInfo = new SDKDeviceInfo();
            }
            return m_SDKDeviceInfo;
        }
    }

    protected ApkInfo m_ApkInfo = new ApkInfo();
    public ApkInfo GetApkInfo
    {
        get
        {
            if (m_ApkInfo == null)
            {
                m_ApkInfo = new ApkInfo();
            }
            return m_ApkInfo;
        }
    }
    protected SDKUserInfo m_SDKUserInfo = new SDKUserInfo();
    public SDKUserInfo SDKUserInfo
    {
        get
        {
            if (m_SDKUserInfo == null)
            {
                m_SDKUserInfo = new SDKUserInfo();
            }
            return m_SDKUserInfo;
        }
        set
        {
            m_SDKUserInfo = value;
        }
    }

    public static string STR_SERVERLIST = "serverList.xml";
    public static string STR_SERVERLIST_CORE = "serverList_core.xml";
    public static string STR_SERVERLIST_CORE2 = "serverList_core2.xml";
    public static string STR_SERVERLIST_CORE_915 = "serverList_core3.xml";
    public static string STR_SERVERLIST_CORE_JUNHAI = "serverList_core4.xml";
    public static string STR_SERVERLIST_IOS_APPSTORE = "serverList_ios_appstore.xml";
    public static string STR_SERVERLIST_IOS_APPSTORE_CORE_CC = "serverList_ios_appstore_core_cc.xml";
    public enum DebugType
    {
        NormalMask,
        WarningMask,
        ErrorMask,
        NormalNoMask,
        WardingNoMask,
        ErrorNoMask,
    }
    /// <summary>
    /// 渠道SDK对应游戏使用logo类型
    /// </summary>
    public enum SDKLogoType
    {
        VersionUpdate,
        SelectRole,
        LoginLogo,
        LoadingLogo
    }

    /// <summary>
    /// 功能SDK 类型
    /// </summary>
    public enum FuncSDKType
    {
        FSDK_UniWebView = 0
    }
    public enum LoginFailCode
    {
        Unkonw = 0,
        LoginFail = 1001,
        LoginCancel = 1002,
        AppIDNotFound = 1003,
        NotInit = 2001
    }
    public delegate void LoginCallback(SDKUserInfo userInfo);
    public delegate void LoginFailCallback(LoginFailCode code);
    public delegate void LogoutCallback();

    public delegate void PayResultCallback(string result);
    public delegate void BindPhoneCallBack(string bindedPhoneNum);
    public delegate void KeyboardShowOut(string result);
    public delegate void SmallGameLoad(string startSceneName);
    public delegate void AdultCheakcallback(int flag, int realNameFlag, int age);

    public LoginCallback loginCallbackGame;
    public LoginFailCallback loginFailCallbackGame;
    public LogoutCallback logoutCallbackGame;

    public PayResultCallback payResultCallbackGame;
    public BindPhoneCallBack bindPhoneCallbackGame;
    public KeyboardShowOut keyboardShowCallbackGame;
    public SmallGameLoad smallGameLoadCallbackGame;
    public AdultCheakcallback adultCheakcallback;

    public const string SDK_CHANNEL_CONFIG_FILENAME = "sdkchannel.json";
    public const string SDK_NO_CHANNEL_TYPE_NAME = "NONE";//沒有渠道默认值

    private static SDKInterface _instance;


    private static SDKInterface _GetSDKInstance(string currChannelType)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (currChannelType != SDK_NO_CHANNEL_TYPE_NAME)
            {
                return new SDKInterfaceAndroidChannel();
            } 
            else
            {
                return new SDKInterfaceAndroid();
            }
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            if (currChannelType != SDK_NO_CHANNEL_TYPE_NAME)
            {
                return new SDKInterfaceIOSChannel();
            }
            else
            {
                return new SDKInterfaceIOS();
            }
#endif
        return new SDKInterfaceDefault();
    }

    public static SDKInterface Instance {
        get {
            if (_instance == null) {
                //保证只需要解析一次
                string currChannelType = _LoadSDKChannelConfig();
                _instance = _GetSDKInstance(currChannelType);
            }
            return _instance;
        }
    }
    public static SDKInterface instance
    {
        get
        {
            return Instance;
        }
    }
    private static string _LoadSDKChannelConfig()
    {
        string sdkChannelType = SDK_NO_CHANNEL_TYPE_NAME;
        try
        {
            byte[] data = _LoadConfigData();
            if (null != data)
            {
                string content = System.Text.ASCIIEncoding.Default.GetString(data);
                if (string.IsNullOrEmpty(content))
                {
                    SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "load config content is null");
                }
                else
                {
                    m_SDKChannelInfo = LitJson.JsonMapper.ToObject<SDKChannelInfo>(content);
                    if (m_SDKChannelInfo == null)
                    {
                        m_SDKChannelInfo = new SDKChannelInfo();
                    }
                    else
                    {
                        sdkChannelType = m_SDKChannelInfo.channelType;
                    }
                }
            }
            return sdkChannelType;
        }
        catch (Exception e)
        {
            SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
            return sdkChannelType;
        }
    }
    private static byte[] _LoadConfigData()
    {
        byte[] data = null;
        try
        {
            if (!FileArchiveAccessor.LoadFileInLocalFileArchive(SDK_CHANNEL_CONFIG_FILENAME, out data))
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "can not load file in streamingPath: {0}", SDK_CHANNEL_CONFIG_FILENAME);
            }
        }
        catch (Exception e)
        {
            SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
        }

        return data;
    }


    /// <summary>
    /// 区别 10月前后接入的 SDK
    /// </summary>
    /// <returns></returns>
    public static bool IsNewSDKChannelPay()
    {
   //     if (Global.Settings.sdkChannel == SDKChannel.NONE || Global.Settings.sdkChannel == SDKChannel.COUNT ||
   //         Global.Settings.sdkChannel == SDKChannel.MG || Global.Settings.sdkChannel == SDKChannel.SN79 ||
   //         Global.Settings.sdkChannel == SDKChannel.TB || Global.Settings.sdkChannel == SDKChannel.AISI ||
   //         Global.Settings.sdkChannel == SDKChannel.XY || Global.Settings.sdkChannel == SDKChannel.TestMG ||
   //         Global.Settings.sdkChannel == SDKChannel.MGYingYB || Global.Settings.sdkChannel == SDKChannel.ZY ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYOL || Global.Settings.sdkChannel == SDKChannel.ZYYH ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYHD || Global.Settings.sdkChannel == SDKChannel.ZYWZ ||
   //         Global.Settings.sdkChannel == SDKChannel.MGOther || Global.Settings.sdkChannel == SDKChannel.ZYYZ ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYYS || Global.Settings.sdkChannel == SDKChannel.ZYLK ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYGB || Global.Settings.sdkChannel == SDKChannel.ZYMG ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYYY || Global.Settings.sdkChannel == SDKChannel.ZYNL ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYSY ||
   //         Global.Settings.sdkChannel == SDKChannel.MGOtherHC ||
   //         Global.Settings.sdkChannel == SDKChannel.TestInMG || Global.Settings.sdkChannel == SDKChannel.DYCC ||
			//Global.Settings.sdkChannel == SDKChannel.MGDY || Global.Settings.sdkChannel == SDKChannel.DYAY)
   //     {
   //         return false;
   //     }
        return true;
    }

    public bool IsMGChannel()
    {
   //     if (Global.Settings.sdkChannel == SDKChannel.MG || Global.Settings.sdkChannel == SDKChannel.ZY ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYHD || Global.Settings.sdkChannel == SDKChannel.ZYOL ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYWZ || Global.Settings.sdkChannel == SDKChannel.ZYYH ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYYZ || Global.Settings.sdkChannel == SDKChannel.ZYYS ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYLK || Global.Settings.sdkChannel == SDKChannel.ZYGB ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYMG || Global.Settings.sdkChannel == SDKChannel.ZYYY ||
   //         Global.Settings.sdkChannel == SDKChannel.ZYNL || Global.Settings.sdkChannel == SDKChannel.ZYSY ||
   //         Global.Settings.sdkChannel == SDKChannel.DYCC || Global.Settings.sdkChannel == SDKChannel.MGDY ||
			//Global.Settings.sdkChannel == SDKChannel.DYAY)
        if (m_SDKChannelInfo.channelID == "mg")
        {
            return true;
        }
        return false;
    }

    public bool IsIOSOtherChannel()
    {
        if (m_SDKChannelInfo.channelType == "IOSOther")
        {
            return true;
        }
        //if (Global.Settings.sdkChannel == SDKChannel.XY || Global.Settings.sdkChannel == SDKChannel.AISI)
        //{
        //    return true;
        //}
        return false;
    }

    public bool HasVIPAuth()
    {
        //bool iscur = false;
        //if (null != Global.VipAuthSDKChannel && Global.VipAuthSDKChannel.Length > 0) 
        //{
        //    foreach (var item in Global.VipAuthSDKChannel)
        //    {
        //        if (item == Global.Settings.sdkChannel)
        //        {
        //            iscur = true;
        //            break;
        //        }
        //    }
        //}        
        //if (IsMGChannel() || iscur)
        //{
        //    return true;
        //}
        return false;
    }

    /// <summary>
    /// version 实名开关
    /// </summary>
    /// <returns></returns>
    public bool IsRealNameAuth()
    {
        //if (null != Global.RealNamePopWindowsChannel && Global.RealNamePopWindowsChannel.Length > 0)
        //{
        //    foreach (var item in Global.RealNamePopWindowsChannel)
        //    {
        //        if (item == Global.Settings.sdkChannel)
        //        {
        //            return true;
        //        }
        //    }
        //}
        return false;
    }


	public virtual void Init (bool debug){
		//var sdkCallback = SDKCallback.instance;
        SDKUtility.OPEN_SDK_LOG_WHOLE = debug;

        var sdkCallback = SDKCallback.instance;

        if (debug && null != m_SDKChannelInfo)
        {
            LogSDKChannelInfo(m_SDKChannelInfo, DebugType.NormalNoMask);
        }

        if (null != m_SDKDeviceInfo)
        {
            m_SDKDeviceInfo.deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            m_SDKDeviceInfo.modeltype = GetDeviceModelType();
            m_SDKDeviceInfo.osInfo = GetOperationSystemInfo();
        }
        if (null != m_ApkInfo)
        {
            m_ApkInfo.appBundleId = UnityEngine.Application.identifier;
            m_ApkInfo.appName = UnityEngine.Application.productName;
            m_ApkInfo.companyName = UnityEngine.Application.companyName;
        }

    }
    public static void LogSDKChannelInfo(SDKChannelInfo channelInfo, SDKInterface.DebugType debugType)
    {
        if (null != channelInfo)
        {
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:channelID:{0}", channelInfo.channelID);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:channelName:{0}", channelInfo.channelName);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:channelParam:{0}", channelInfo.channelParam);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:channelType:{0}", channelInfo.channelType);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:platformType:{0}", channelInfo.platformType);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:payCallbackUrlFormat:{0}", channelInfo.payCallbackUrlFormat);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needPayToken:{0}", channelInfo.needPayToken);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needPayResultNotify:{0}", channelInfo.needPayResultNotify);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:serverListName:{0}", channelInfo.serverListName);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needUriEncodeOpenUid:{0}", channelInfo.needUriEncodeOpenUid);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needShowChannelRankBtn:{0}", channelInfo.needShowChannelRankBtn);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needLocalNotification:{0}", channelInfo.needLocalNotification);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needBindMobilePhone:{0}", channelInfo.needBindMobilePhone);
        }
    }


    public virtual void Login (){}
	public virtual void Logout (){}

    #region 新接口新接口新接口新接口新接口新接口新接口
    public virtual void Pay(SDKPayInfo payInfo) { }
    public virtual void ReportRoleInfo(SDKReportRoleInfo roleInfo) { }
    //服务器登录验证URL
    public virtual string LoginVerifyUrl(SDKLoginVerifyUrl verifyUrl)
    {
        if (verifyUrl == null || m_SDKChannelInfo == null || m_SDKUserInfo == null)
        {
            return "";
        }
        return string.Format("http://{0}/login?id={1}&openid={2}&token={3}&did={4}&platform={5}&model={6}&device_version={7}",
            verifyUrl.serverUrl, verifyUrl.serverId, m_SDKUserInfo.openUid, m_SDKUserInfo.token,
            m_SDKDeviceInfo.deviceId, verifyUrl.channelParam, m_SDKDeviceInfo.modeltype, m_SDKDeviceInfo.osInfo);
    }
    /// <summary>
    /// 设置特殊的支付时角色信息
    /// 服务器返回帐号信息 传回给SDK
    /// </summary>
    /// <param name="openid"></param>
    /// <param name="accesstoken"></param>
    public virtual void SetSpecialPayUserInfo(string openUid, string token)
    {
        if (m_SDKUserInfo != null)
        {
            m_SDKUserInfo.openUid = openUid;
            m_SDKUserInfo.token = token;
        }
    }

    public virtual bool NeedSDKBindPhoneOpen()
    {
        if (m_SDKChannelInfo != null)
        {
            return m_SDKChannelInfo.needBindMobilePhone;
        }
        return false;
    }

    public virtual void OpenBindPhone() { }
    public virtual void CheckIsPhoneBind() { }

    #endregion



    #region 本地推送
    public virtual void ResetBadge(){}
	public virtual void SetNotification(int nid, string content, string title, int hour){}
	public virtual void SetNotificationWeekly(int nid, string content, string title, int weekday, int hour, int minute){}
    public virtual void SetNotificationWithTsp(int nid, string content, string title, long tsp_unix) { }
    public virtual void RemoveNotification(int nid){}
	public virtual void RemoveAllNotification(){}
    #endregion

    public virtual void SetScreenBrightness(float value){}
	public virtual float GetScreenBrightness(){ return 0.5f;}
    public virtual void KeepScreenOn(bool isOn) { }
	public virtual void Exit(){}
    public virtual float GetBatteryLevel(){ return 1.00f;}
    public virtual bool IsBatteryCharging() { return true; }
    public virtual string GetSystemTimeHHMM(){ DateTime nowTime = DateTime.Now; return string.Format("{0:HH:mm}", nowTime);}
    public virtual bool RequestAudioAuthorization(){return false;}
    public virtual void SetAudioSessionActive(){}
    public virtual string GetSystemTimeStamp()
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        uint timeStamp = (uint)(DateTime.Now - startTime).TotalSeconds;
        return timeStamp.ToString();
    }
    public virtual void QuitApplicationOnEsc() { }
    #region 旧渠道接口
    public virtual void Pay(string price, string extra = "", int serverID = 0, string openuid = "")
    {
        Pay(new SDKPayInfo()
        {
            price = price,
            extra = extra,
            serverId = serverID.ToString(),
            openUid = openuid
        });
    }
    public virtual void Pay(string price, string extra = "", int serverID = 0, string openuid = "", string productName = "", string productDesc = "")
    {
        Pay(new SDKPayInfo()
        {
            price = price,
            extra = extra,
            serverId = serverID.ToString(),
            openUid = openuid,
            productName = productName,
            productDesc = productDesc
        });
    }
    public virtual void Pay(string requestId, string price, int serverId, string openUid, string roleId, int mallType, int productId,
        string productName, string productDesc, string extra = "")
    {

        Pay(new SDKPayInfo()
        {
            price = price,
            extra = extra,
            serverId = serverId.ToString(),
            openUid = openUid,
            productName = productName,
            productDesc = productDesc,
            requestId = requestId,
            roleId = roleId,
            mallType = mallType.ToString(),
            productId = productId.ToString(),

        });
    }

    public virtual string LoginVerifyUrl(string serverUrl, string serverId, string openuid, string token, string deviceId,
        string sdkChannel, string ext)
    {
        return string.Format("http://{0}/login?id={1}&openid={2}&token={3}&did={4}&platform={5}",
            serverUrl, serverId, openuid, token, deviceId, sdkChannel);
    }

    /// <summary>
    /// 获得商品内购挡位
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual string GetProductId(int id)
    {
        var ProductPearsTableData = TableManager.GetInstance().GetTable<ProtoTable.ChargeGearTable>();
        if (ProductPearsTableData != null)
        {
            var enumerator = ProductPearsTableData.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ProtoTable.ChargeGearTable item = enumerator.Current.Value as ProtoTable.ChargeGearTable;
                if (Global.Settings.sdkChannel.ToString().Equals(item.Channel))
                {
                    if (id > 0 && item.ProductIds.Count >= id)
                        return item.ProductIds[id - 1];
                }
            }
        }
        return string.Empty;
    }
    public virtual string GetClipboardText() { return ""; }

    /* android impl */
    
    //public virtual bool NeedSDKBindPhoneOpen() { return false;}
    //public virtual void OpenBindPhone(){ }
    //public virtual void CheckIsPhoneBind(){ }

    /* android impl for record role info to sdk */

    // 7977旧sdk需求，没用了
    //public virtual bool CanSetCreateRoleInfoInFiveMin() { return false; }
    public virtual void SetCreateRoleInfo(string accid, string roleid) { }
    // 7977旧sdk需求，没用了
    //public virtual void SetCreateRoleInFiveInfo(string accid,string roleid) { }
    public virtual void SetRoleUpLevelInfo(string accid,string roleid,string roleLevel) { }
    public virtual void SetCreateRoleInfo(string accid, string roleid,string roleName, string roleLevel ,string serverName, string roleRank, string beSociaty) { }
	public virtual void SetCreateRoleInfo(string roleName, int serverId) { }

    public virtual void UpdateRoleInfo(int scene, uint serverID, string serverName,
        string roleID, string roleName, int proID, int roleLevel=1, int vip=0,
        int dianquanNum=0)
    { 
    }
    #endregion
    /* sdk push ads */
    public virtual void InitServicePush(string gameObjectName) { }
    public virtual void TurnOnServicePush() { }
    public virtual void TurnOffServicePush() { }
    public virtual bool BindOtherName(string alias) { return false; }
    public virtual bool UnBindOtherName(string alias) { return false; }
	/* UniWebView util*/
    public virtual int GetKeyboardSize() { return 0; }
    public virtual int TryGetCurrVersionAPI() { return 0; }

	/*LB Hot Update*/
    public virtual void LBRequestUpdate() { }

    public virtual void LBDownloadFullRes() { }

    public virtual bool LBIsAfterUpdate() { return false; }

    public virtual string LBGetFullResPath() { return string.Empty; }

    public virtual void SaveDoc(string content, string parentPathName, string fileName, bool isAppend = true) { }
    public virtual string ReadDoc(string parentPathName, string fileName) { return ""; }
    public virtual float GetCpuTemperature() { return 0; }
    public virtual float GetBatteryTemperature() { return 0; }
    public virtual string GetAvailMemory() { return string.Empty; }
    public virtual string GetCurrentProcessMemory() { return string.Empty; }
    public virtual string GetMonoMemory()
    {
        //long totalMemory = System.GC.GetTotalMemory(false);
        //return totalMemory.ToString();
        return null;
    }
	
	public virtual bool IsSimulator() { return false; }
    public virtual string GetSimulatorName() { return string.Empty; }

    #region 实名认证
    public virtual bool HasSDKAdultCheakAuth()
    {
        return false;
    }
    public virtual void GetRealNameInfo()
    {
        SDKCallback.instance.OnAdultCheakRet("0" + "," + ((int)(Protocol.AuthIDType.AUTH_ADULT)).ToString() + "," + "0");
    }
    public virtual void GetRealNameInfo_NotLogin()
    {

    }
    public virtual bool CanOpenAdultCheakWindow()
    {
        return false;
    }
    public virtual void OpenAdultCheakWindow()
    {

    }
    #endregion
    #region Extension
    public virtual string ExtendLoginVerifyUrl()
    {
        string extendUrl = "";
        extendUrl = string.Format("&model={0}&device_version={1}&age={2}&is_id_auth={3}", GetDeviceModelType(), GetOperationSystemInfo(),ClientApplication.playerinfo.age,(int)ClientApplication.playerinfo.authType);
        return extendUrl;
    }

    /// <summary>
    /// 获取系统版本信息
    /// </summary>
    /// <returns></returns>
    public string GetOperationSystemInfo()
    {
        return UrlFormatFilter(SystemInfo.operatingSystem);
    }
    /// <summary>
    /// 获取设备型号
    /// </summary>
    /// <returns></returns>
    public string GetDeviceModelType()
    {
#if BANGCLE_EVERISK
        string deviceMode = string.Empty;
        if (PluginManager.GetInstance().IsEmulator())
        {
            deviceMode = PluginManager.GetInstance().EmulatorType();
        }
        else
        {
            deviceMode = UrlFormatFilter(SystemInfo.deviceModel);
        }
        return deviceMode;
#else
        return UrlFormatFilter(SystemInfo.deviceModel);
#endif
    }

    private string UrlFormatFilter(string originStr)
    {
        return originStr.Replace(" ", "_").Replace("&", "_").Replace("|", "_").Replace(";", "_");
    }

    /// <summary>
    /// 获得平台id android : 1  /  ios : 2
    /// </summary>
    /// <returns></returns>
    public virtual string GetBuildPlatformId()
    {
        return "0";
    }

    /// <summary>
    /// 获得在线客服平台id android : 0 /  ios : 1 /  ios越狱 : 2 / 其他 : 3
    /// </summary>
    /// <returns></returns>
    public virtual string GetOnlineServicePlatformId()
    {
        return "3";
    }

    /// <summary>
    /// 获得在线客服平台name android : 安卓 /  ios : ios /  ios越狱 : ios越狱 / 其他 : 其他
    /// </summary>
    /// <returns></returns>
    public virtual string GetOnlineServicePlatformName()
    {
        return "other";
    }

    public virtual void MobileVibrate()
    {
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public virtual void ScanFile(string path)
    {
        
    }

    public virtual void InitAlartText(string title, string message, string btnText)
    {

    }

    public virtual bool NeedShowBanQuanMsg()
    {
        if (Global.Settings.sdkChannel == SDKChannel.DYCC ||
            Global.Settings.sdkChannel == SDKChannel.MGDY ||
			Global.Settings.sdkChannel == SDKChannel.DYAY)
        {
            return true;
        }
        return false;
    }

    public virtual bool NeedHideLoginFrameLogoImg()
    {
        if (Global.Settings.sdkChannel == SDKChannel.DYCC ||
            Global.Settings.sdkChannel == SDKChannel.MGDY ||
			Global.Settings.sdkChannel == SDKChannel.DYAY)
        {
            return true;
        }
        return false;
    }
#endregion

#region Android Channels SDK

    /// <summary>
    /// 订单号生成 (字母数字)
    /// </summary>
    /// <param name="roleId">角色id</param>
    /// <param name="timeStamp">时间戳</param>
    /// <param name="ext">其他</param>
    /// <param name="limitLenght">限制字符数</param>
    /// <returns></returns>
    public virtual string GenerateRequestPayID(string roleId, string timeStamp = "", string ext="", int limitLenght=-1) 
    {
        string reqId = "";
        if (timeStamp == "")
        {
            timeStamp = TransLocalNowDateToStamp() + "";
        }
        reqId = roleId + timeStamp + ext;
        if (limitLenght > 0 && string.IsNullOrEmpty(reqId))
        {
            reqId = reqId.Substring(0,limitLenght);
        }
        return reqId;
    }
    public virtual uint TransLocalNowDateToStamp()
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        uint timeStamp = (uint)(DateTime.Now - startTime).TotalSeconds;
        return timeStamp;
    }
    public virtual string MD5CreateNormal(string STR)
    {
        string res = "";
        if (string.IsNullOrEmpty(STR))
            return res;
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        res = BitConverter.ToString(md5.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(STR)));
        res = res.Replace("-", "");
        return res.ToLowerInvariant();
    }

    public virtual void SetAccInfo(string openid, string accesstoken)
    {
    }

    public virtual string[] GetSplashResourcePath()
    {
#if APPLE_STORE
		string splash1 = "Base/UI/Image/logo";
#else
        string splash1 = "Base/UI/Image/aladezhinu";
#endif

        string[] splashResPaths = new string[1]{splash1};
        return splashResPaths;
	}
    public virtual string GetServerListName() {

        if (Global.Settings.sdkChannel == SDKChannel.MGOther)
        {
            return SDKInterface.STR_SERVERLIST_CORE2; // 商店服务器列表
        }

        if (Global.Settings.sdkChannel == SDKChannel.MGOtherHC)
        {
            return SDKInterface.STR_SERVERLIST_CORE; // 硬核服务器列表
        }

        if (!IsNewSDKChannelPay())
        {
#if APPLE_STORE
            if (Global.Settings.sdkChannel == SDKChannel.DYCC ||
				Global.Settings.sdkChannel == SDKChannel.MGDY ||
			Global.Settings.sdkChannel == SDKChannel.DYAY)
			{
                return STR_SERVERLIST_IOS_APPSTORE_CORE_CC;
			}
			else
			{
				return STR_SERVERLIST_IOS_APPSTORE;
			}
#else
            return STR_SERVERLIST;
#endif
        }
        if (Global.Settings.sdkChannel == SDKChannel.BaiDu ||
            Global.Settings.sdkChannel == SDKChannel.XiaoMi ||
            Global.Settings.sdkChannel == SDKChannel.SanXing ||
            Global.Settings.sdkChannel == SDKChannel.M4399 ||
            Global.Settings.sdkChannel == SDKChannel.M360 ||
            Global.Settings.sdkChannel == SDKChannel.MGOther ||
            Global.Settings.sdkChannel == SDKChannel.JoyLand ||
            Global.Settings.sdkChannel == SDKChannel.MGOtherHC)

            return SDKInterface.STR_SERVERLIST_CORE2;

        if (Global.Settings.sdkChannel == SDKChannel.M915)
            return SDKInterface.STR_SERVERLIST_CORE_915;

        if (Global.Settings.sdkChannel == SDKChannel.JUNHAI)
            return SDKInterface.STR_SERVERLIST_CORE_JUNHAI;

        return SDKInterface.STR_SERVERLIST_CORE;
    }

    public virtual bool IsPayResultNotify()
    {
        if (Global.Settings.sdkChannel == SDKChannel.M915 ||
            Global.Settings.sdkChannel == SDKChannel.JUNHAI ||
            Global.Settings.sdkChannel == SDKChannel.MGOther ||
			Global.Settings.sdkChannel == SDKChannel.MG ||
			Global.Settings.sdkChannel == SDKChannel.MGYingYB ||
			Global.Settings.sdkChannel == SDKChannel.JoyLand ||
            Global.Settings.sdkChannel == SDKChannel.MGOtherHC)
        {
            return true;
        }
        return false;
    }


    public virtual string GetSystemIMEI() { return ""; }

    public virtual bool IsStartFromGameCenter()
    {
        return false;
    }
    public virtual bool IsOppoPlatform()
    {
        if (Global.Settings.sdkChannel == SDKChannel.OPPO)
        {
            return true;
        }
        return false;
    }

    public virtual void GotoSDKChannelCommunity()
    { 
    }

    public virtual bool IsVivoPlatForm()
    {
        if (Global.Settings.sdkChannel == SDKChannel.VIVO)
        {
            return true;
        }
        return false;
    }


    public virtual void OpenGameCenter()
    {
    }

    public virtual string GetPlatformNameByChannel()
    {
        string platformName = "none";

        var sdkChannelNames = Global.SDKChannelName;
        if (sdkChannelNames != null)
        {
            if (sdkChannelNames.Length == (int)SDKChannel.COUNT)
            {
                platformName = sdkChannelNames[(int)Global.Settings.sdkChannel];
            }
        }
        return platformName;
    }

    /// <summary>
    /// 渠道 sdk支付时是否需要传入登录验证的token
    /// 
    /// 会调用 SetAccInfo(openid , token) 方法
    /// </summary>
    /// <returns></returns>
    public virtual bool NeedPayToken()
    {
        if (Global.Settings.sdkChannel == SDKChannel.KuPai ||
            Global.Settings.sdkChannel == SDKChannel.JUNHAI)
        {
            return true;
        }
        return false;
    }

    public static bool BeOtherChannel()
    {
        if (Global.Settings.sdkChannel == SDKChannel.M915 || Global.Settings.sdkChannel == SDKChannel.JUNHAI)
        {
            return true;
        }
        return false;
    }

    public static bool NeedChannelHideVersionUpdateProgress()
    {
        if (Global.Settings.sdkChannel == SDKChannel.DYCC ||
			Global.Settings.sdkChannel == SDKChannel.MGDY ||
			Global.Settings.sdkChannel == SDKChannel.DYAY)
        {
            return true;
        }
        return false;
    }

    public virtual void NeedLogoutSDK()
    {
        if (Global.Settings.sdkChannel == SDKChannel.JoyLand ||
            Global.Settings.sdkChannel == SDKChannel.DYCC ||
            Global.Settings.sdkChannel == SDKChannel.MGDY ||
            Global.Settings.sdkChannel == SDKChannel.DYAY)
        {
            Logout();
        }
    }

    public virtual string NeedUriEncodeOpenid(string openid)
    {
        if (string.IsNullOrEmpty(openid))
            return "";
        if (Global.Settings.sdkChannel == SDKChannel.JoyLand)
        {
            openid = Uri.EscapeDataString(openid);
        }
        return openid;
    }

    /// <summary>
    /// 根据渠道枚举值 判断当前是否是对应渠道平台
    /// </summary>
    /// <param name="sdkChannel"></param>
    /// <returns></returns>
    public virtual bool CheckPlatformBySDKChannel(SDKChannel sdkChannel)
    {
        if (Global.Settings.sdkChannel == sdkChannel)
        {
            return true;
        }
        return false;
    }

    public virtual SDKChannel GetCurrentSDKChannel()
    {
        return Global.Settings.sdkChannel;
    }
	
	#endregion
	#region ios appstore
    public virtual void GetNewVersionInAppstore() { }

    public virtual string GetIOSAppstoreSmallGameLoadSceneName() { return "select"; }

    public virtual bool IsIOSAppstoreLoadSmallGame() { return false; }

    /// <summary>
    /// 新增 2020 01 29    尝试请求IOS内购参数信息
    /// </summary>
    public virtual void TryGetIOSAppstoreProductIds() { }

    #endregion
	
	#region GET IMEI
    private UnityEngine.Coroutine imeiCoroutine;
    public virtual void SendGameServerSystemIMEI()
    {
        if (imeiCoroutine != null)
        {
            GameClient.GameFrameWork.instance.StopCoroutine(imeiCoroutine);
        }
        imeiCoroutine = GameClient.GameFrameWork.instance.StartCoroutine(WaitToReportIMEI(5000));
    }

    IEnumerator WaitToReportIMEI(int timeout)
    {
        string serverAdd = "";

#if UNITY_ANDROID
        serverAdd = Global.ANDROID_MG_CHARGE;
#elif UNITY_IOS || UNITY_IPHONE
        serverAdd = "";
#endif
        if (string.IsNullOrEmpty(serverAdd))
            yield break;

        GameClient.BaseWaitHttpRequest wt = new GameClient.BaseWaitHttpRequest();
        string sendUrl = string.Format("http://{0}/report?openid={1}&imei={2}&platform={3}",
            serverAdd, ClientApplication.playerinfo.openuid, GetSystemIMEI(), Global.SDKChannelName[(int)(Global.Settings.sdkChannel)]);

        wt.url = sendUrl;
        wt.timeout = timeout;
        wt.gaptime = 0;
        wt.reconnectCnt = 0;

        yield return wt;

        if (GameClient.BaseWaitHttpRequest.eState.Success == wt.GetResult())
        {
            string resText = wt.GetResultString();
            int retCode = -1;
            if (string.IsNullOrEmpty(resText))
            {
                //Logger.LogError("report device :  ret json is null ");
                yield return null;
            }

            Hashtable ret = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(resText);
            try
            {
                if (Int32.TryParse(ret["error"].ToString(), out retCode))
                {
                    if (retCode != 0)
                    {
                        string msg = ret["msg"].ToString();
                        //Logger.LogErrorFormat("report device : decode ret json error {0}", msg);
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.LogError("report device : decode ret json error " + e.ToString());
            }
        }
    }
    public Hashtable JsontoHashtable(string jsonData)
    {
        Hashtable hashtable = null;
        try
        {
            if (string.IsNullOrEmpty(jsonData))
                return hashtable;
            hashtable = XUPorterJSON.MiniJSON.jsonDecode(jsonData) as Hashtable;
        }
        catch (Exception e)
        {
            Debug.LogError("JsontoHashtablehas failed! Exception:" + e.ToString());
        }
        return hashtable;
    }
    #endregion
    #region smallpackage

    public virtual bool IsResourceDownloadFinished()
    {
        return true;
    }

    public virtual bool IsSmallPackage()
    {
        return false;
    }

    public virtual long GetResourceDownloadedSize()
    {
        return 0;
    }

    public virtual long GetTotalResourceDownloadSize()
    {
        return 0;
    }

    public virtual void OpenDownload()
    {

    }

    public virtual void CloseDownload()
    {

    }

    public virtual bool IsDownload()
    {
        return false;
    }

    #endregion
    #region 刘海屏
    public enum NotchDebugType
    {
        None,
        Android,
        IOS
    }
    /// <summary>
    /// Android才有效果
    /// </summary>
    public virtual void InitNotch()
    {
    }
    public virtual bool HasNotch(bool debug = false)
    {
        if (debug)
            return true;
        return false;
    }
    /// <summary>
    /// opengl数据,left bottom right top
    /// </summary>
    /// <param name="debug"></param>
    /// <returns></returns>
    public virtual int[] GetNotchSize(NotchDebugType debug = NotchDebugType.None, ScreenOrientation screenOrientation = ScreenOrientation.LandscapeLeft)
    {
        int[] n;
        switch (debug)
        {
            case NotchDebugType.None:
                n = new int[4] { 0, 0, 0, 0 };
                break;
            case NotchDebugType.Android:
                if (screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.Landscape)
                    n = new int[4] { 0, Screen.height / 2 + 150, 80, Screen.height / 2 - 150 };
                else
                    n = new int[4] { Screen.width - 80, Screen.height / 2 + 150, Screen.width, Screen.height / 2 - 150 };
                break;
            case NotchDebugType.IOS:
                if (screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.Landscape)
                    n = new int[4] { 0, Screen.height, 132, 0 };
                else
                    n = new int[4] { Screen.width - 132, Screen.height, Screen.width, 0 };
                break;
            default:
                n = new int[4] { 0, 0, 0, 0 };
                break;
        }
        return n;
    }
    #endregion
}
