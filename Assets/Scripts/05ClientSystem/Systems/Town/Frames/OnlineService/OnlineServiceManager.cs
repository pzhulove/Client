using GameClient;
using Network;
using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class OnlineServiceManager : DataManager<OnlineServiceManager>
{
    const string RequestSignNotify = "客服系统正在维护中...";

    //在线客服默认参数
    private static string appid = "138";
    private static string userId = "";
    string userName = "";
    string serverId = "";
    string serverName = "";
    string platId = "";
    string platName = "";
    string level = "";
    string jobId = "";
    string timeStap = "";
    string vipFlag = "";
    string channelName="";
	
	//VIP认证新增
    string accId = "";
    string roleId = "";
    string vipPlatId = "";
	string jobName = "";
	string vipLevel = "";

    const int urlParamCount = 10;
    //string[] urlParamStrs;

    #region Main params 

    public bool IsOnlineServiceOpen { get; private set; }

    string openServiceSignStr = "";
    string offlineInfoSignStr = "";
    string vipAuthSignStr = "";

    int reqOfflineInfoDuration = 5;
    bool isUpdateReqInfo = false;
    int unReadMsgNum = 0;

    UnityEngine.Coroutine tempCoroutine;

    float elapsedTime = 0f;

    bool isReqOpenServiceUrl = false;
    bool isReqVipAuthUrl = false;

    int reqSignWithHttpTimeOut = 3000;

    #endregion

    #region PUBLIC METHODS

    public void TryCloseOnlineServiceFrame()
    {
        if (ClientSystemManager.GetInstance().IsFrameOpen<OnlineServiceFrame>())
        {
            ClientSystemManager.GetInstance().CloseFrame<OnlineServiceFrame>();
        }
    }

    public void UpdateReqOfflineInfos(float timeElapsed)
    {
        if (isUpdateReqInfo)
        {
            elapsedTime += timeElapsed;
            if (elapsedTime > reqOfflineInfoDuration)
            {
                elapsedTime = 0f;

                ReqOfflineInfoSign();
                isUpdateReqInfo = false;
            }
        }
    }

    public void StartReqOfflineInfos(bool beUpdate)
    {
        isUpdateReqInfo = beUpdate;
        elapsedTime = 0f;
        reqOfflineInfoDuration = 5;
        unReadMsgNum = 0;
        if (tempCoroutine != null)
        {
            GameFrameWork.instance.StopCoroutine(tempCoroutine);
            tempCoroutine = null;
        }
    }

    public void RequireOfflineInfos()
    {
        if (tempCoroutine != null)
        {
            GameFrameWork.instance.StopCoroutine(tempCoroutine);
            tempCoroutine = null;
        }
        tempCoroutine = GameFrameWork.instance.StartCoroutine(WaitForOfflineUrlRet(reqSignWithHttpTimeOut));
    }
        
    public bool HasUnReadMessage()
    {
        if (unReadMsgNum > 0)
            return true;
        return false;
    }

    public bool IsSatisfiedVipLevel()
    {
        bool vipFlag = false; //false / 0为普通  |   1为vip
        int enableVipLevel = 10;
        string vipStr = TR.Value("vip_online_service_entrance_openlevel");
        if (int.TryParse(vipStr, out enableVipLevel))
        {
            vipFlag = PlayerBaseData.GetInstance().VipLevel >= enableVipLevel ? true : false;
            Logger.LogProcessFormat("vip_online_service_entrance_openlevel : {0}", enableVipLevel);
        }
        return vipFlag;
    }

    public void TryReqOnlineServiceSign()
    {
        bool isVipAuthFuncLock = ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(Protocol.ServiceType.SERVICE_VIP_AUTH);
        bool isSpecialVip = IsSatisfiedVipLevel();
        //bool isMGPlatform = PluginManager.GetInstance().IsMGSDKChannel();
        bool hasVipAuth = PluginManager.GetInstance().HasVIPAuth();

        if (isSpecialVip && !isVipAuthFuncLock && hasVipAuth)
        {
            ReqOnlineServiceUrlSign(true);               //请求在线客服验签链接 VIP认证入口
        }
        else
        {
            ReqOnlineServiceUrlSign();               //请求在线客服验签链接 普通客服
        }
    }

    public bool TryReqOnlineServiceOpen()
    {
        IsOnlineServiceOpen = !ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(Protocol.ServiceType.SERVICE_ONLINE_SERVICE);
        return IsOnlineServiceOpen;
    }

    #endregion

    public override void Initialize()
    {
        ResetUrlParams();
        NetProcess.AddMsgHandler(WorldCustomServiceSignRes.MsgID, OnSyncOnlineServiceSign);
    }

    public override void Clear()
    {
        IsOnlineServiceOpen = false;
        openServiceSignStr = "";
        offlineInfoSignStr = "";
        vipAuthSignStr = "";
        //urlParamStrs = null;
        reqOfflineInfoDuration = 5;
        isUpdateReqInfo = false;
        unReadMsgNum = 0;

        if (tempCoroutine != null)
        {
            GameFrameWork.instance.StopCoroutine(tempCoroutine);
        }
        tempCoroutine = null;

        elapsedTime = 0f;

        NetProcess.RemoveMsgHandler(WorldCustomServiceSignRes.MsgID, OnSyncOnlineServiceSign);
		
		userId = "";

        isReqOpenServiceUrl = false;
        isReqVipAuthUrl = false;
    }

    public void ReqOnlineServiceUrlSign(bool bVipAuth = false)
    {
        isReqOpenServiceUrl = true;
        isReqVipAuthUrl = bVipAuth;

        TrySetUrlParams();
        string urlInfo = "";
        if (bVipAuth)                                   //VIP认证请求验签
        {
            urlInfo = TryGetVipAuthReqInfo();
        }
        else
        {
            urlInfo = TryGetOnlineServiceReqInfo();
        }
        //Logger.LogError("req OnlineServiceUrlSign!!! = " + urlInfo);

        if (string.IsNullOrEmpty(urlInfo))
            return;
        WorldCustomServiceSignReq req = new WorldCustomServiceSignReq();
        req.info = urlInfo;
        NetManager.Instance().SendCommand(ServerType.GATE_SERVER,req);
    }

    public void ReqOfflineInfoSign()
    {
        isReqOpenServiceUrl = false;

        TrySetUrlParams();
        string urlInfo = TryGetOfflineInfoReqInfo();
        //Logger.LogError("req OfflineInfoSign!!! = " + urlInfo);

        if (string.IsNullOrEmpty(urlInfo))
            return;
        WorldCustomServiceSignReq req = new WorldCustomServiceSignReq();
        req.info = urlInfo;
        NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
    }

    void OnSyncOnlineServiceSign(MsgDATA msg)
    {
        WorldCustomServiceSignRes res = new WorldCustomServiceSignRes();
        res.decode(msg.bytes);
        //Logger.LogError("res result = " + res.result);

        if (res.result == (int)ProtoErrorCode.SUCCESS)
        {
            IsOnlineServiceOpen = true;
           
            if (isReqOpenServiceUrl)
            {
                string url = "";
                if (isReqVipAuthUrl)
                {
                    vipAuthSignStr = res.sign;
                    //Logger.LogError("vipAuthSignStr ret sign!!! = " + vipAuthSignStr);
                    url = GetReqVipCheckUrl();
                }
                else
                {
                    openServiceSignStr = res.sign;
                    //Logger.LogError("openServiceSignStr ret sign!!! = " + openServiceSignStr);
                    url = GetReqOnlineServiceUrl();
                }
                if (ClientSystemManager.GetInstance().IsFrameOpen<OnlineServiceFrame>() == false)
                {
                    ClientSystemManager.GetInstance().OpenFrame<OnlineServiceFrame>(FrameLayer.TopMoreMost, url);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRecVipOnlineService, url, isReqVipAuthUrl);
                }
            }
            else
            {
                offlineInfoSignStr = res.sign;

                if (!SDKInterface.IsNewSDKChannelPay())
                    RequireOfflineInfos();
            }
        }
        if (res.result == (int)ProtoErrorCode.CUSTOM_SERVICE_CLOSED)
        {
            IsOnlineServiceOpen = false;

            if (isReqOpenServiceUrl)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(RequestSignNotify);
            }
        }

        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MakeShowOnlineService);
    }

    IEnumerator WaitForCheckVipUrlRet(int timeout)
    {
        string url = GetReqVipCheckUrl();
        if (string.IsNullOrEmpty(url))
            yield break;
        BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
        wt.timeout = timeout;
        wt.gaptime = 0;
        wt.reconnectCnt = 0;
        wt.url = url;

        //Logger.LogError("BaseWaitHttpRequest for WaitForCheckVipUrlRet...Send Http req for sign..." + url);

        yield return wt;

        if (wt.GetResult() == BaseWaitHttpRequest.eState.Success)
        {
            //Logger.LogErrorFormat("WaitForCheckVipUrlRet - url {0} success!",url);
        }
    }

    IEnumerator WaitForOfflineUrlRet(int timeout)
    {
        string url = GetReqOfflineServiceUrl();
        if (string.IsNullOrEmpty(url))
            yield break;
        BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
        wt.timeout = timeout;
        wt.gaptime = 0;
        wt.reconnectCnt = 0;
        wt.url = url;

        //Logger.LogError("BaseWaitHttpRequest for onlineService...Send Http req for sign...");

        yield return wt;

        if (wt.GetResult() == BaseWaitHttpRequest.eState.Success)
        {
            string resText = wt.GetResultString();
            if (string.IsNullOrEmpty(resText) == false)
            {
                Hashtable ret = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(resText);
                string resCode = ret["code"] + "";
                if (resCode != null && resCode.Equals("0"))
                {
                    object hashData = ret["data"];
                    string hash1 = XUPorterJSON.MiniJSON.jsonEncode(hashData);
                    Hashtable res = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(hash1);

                    object interval = res["interval"];
                    if (interval != null)
                    {
                        string durationStr = interval.ToString();
                        //reqOfflineInfoDuration = System.Int32.Parse(durationStr);
                        if (System.Int32.TryParse(durationStr, out reqOfflineInfoDuration)) { }
                        //Logger.LogError("res reqOfflineInfoDuration!!! = " + reqOfflineInfoDuration);       
                    }

                    string number = res["number"] + "";
                    if (number != null)
                    {
                        //unReadMsgNum = System.Int32.Parse(number);
                        //Logger.LogError("res unReadMsgNum!!! = " + unReadMsgNum);

                        if(System.Int32.TryParse(number, out unReadMsgNum))
                        {
                            bool isShowNote = unReadMsgNum > 0 ? true : false;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRecOnlineServiceNewNote, isShowNote);
                        }
                    }

                    //判断是否需要请求刷新 放在最后
                    string flag = res["flag"] + "";
                    if (flag != null)
                    {
                        isUpdateReqInfo = flag.Equals("1") ? true : false;
                        //Logger.LogError("res isUpdateReqInfo!!! = " + isUpdateReqInfo);
                    }
                }
                else
                {
                    //请求失败,取消请求
                    isUpdateReqInfo = false;
                }
            }
            else
            {
                //请求失败,取消请求
                isUpdateReqInfo = false;
            }
        }
    }

    IEnumerator WaitForOfflineUrlRet()
    {
        string url = GetReqOfflineServiceUrl();
       // Logger.LogError("WaitForOfflineUrlRet URL = " + url);
        if (string.IsNullOrEmpty(url))
            yield break;
        WaitHttpRespensedService wh = new WaitHttpRespensedService(url, 5);
        yield return wh;


        //BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
        //string url = FormatUrlParams();//FormatReqOfflineInfoUrl();
        //if (string.IsNullOrEmpty(url))
        //    yield break;
        //wt.url = url;
        //yield return wt;
        //if (BaseWaitHttpRequest.eState.Success == wt.GetResult())
        //{
        string resText = wh.GetResultString();
        if(string.IsNullOrEmpty(resText)==false)
        {
            //string resText = wt.GetResultString();
            //if (string.IsNullOrEmpty(resText))
            //     yield break;
            Hashtable ret = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(resText);
            string resCode = ret["code"] + "";
            if (resCode != null && resCode.Equals("0"))
            {
                object hashData = ret["data"];
                string hash1 = XUPorterJSON.MiniJSON.jsonEncode(hashData);
                Hashtable res = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(hash1);

				object interval = res["interval"];
                if (interval != null)
                {
                	string durationStr = interval.ToString();
                    //reqOfflineInfoDuration = System.Int32.Parse(durationStr);
                    if (System.Int32.TryParse(durationStr, out reqOfflineInfoDuration)) { }
                    //Logger.LogError("res reqOfflineInfoDuration!!! = " + reqOfflineInfoDuration);       
                }

                string number = res["number"] + "";
                if (number != null)
                {
                    //unReadMsgNum = System.Int32.Parse(number);
                    //Logger.LogError("res unReadMsgNum!!! = " + unReadMsgNum);

                    if (System.Int32.TryParse(number, out unReadMsgNum))
                    {
                        bool isShowNote = unReadMsgNum > 0 ? true : false;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRecOnlineServiceNewNote, isShowNote);
                    }
                }

                //判断是否需要请求刷新 放在最后
                string flag = res["flag"] + "";
                if (flag != null)
                {
                    isUpdateReqInfo = flag.Equals("1") ? true : false;
                    //Logger.LogError("res isUpdateReqInfo!!! = " + isUpdateReqInfo);
                }
            }
            else
            {
                //请求失败,取消请求
                isUpdateReqInfo = false;
            }
        }
        else
        {
            //请求失败,取消请求
            isUpdateReqInfo = false;
        }
    }


    void ResetUrlParams()
    {
        //urlParamStrs = new string[urlParamCount] 
        //{ 
        //    userId,userName,serverId,serverName,platId,platName,level,timeStap,jobId,openServiceSignStr
        //};
    }

    private string TryGetOnlineServiceReqInfo()
    {
        return FormatReqSignUrl();
    }

    private string TryGetOfflineInfoReqInfo()
    {
        return FomatReqOfflineSignUrl();
    }

    private string TryGetVipAuthReqInfo()
    {
        return FormatReqVipCheckSignUrl();
    }

    private string FormatReqSignUrl()
    {
        string url = string.Format("user_id={0}&user_name={1}&server_id={2}&server_name={3}&plat_id={4}&plat_name={5}&level={6}&timestamp={7}&jobs_id={8}&is_special={9}&ouid={10}&app_id={11}&channel_name={12}",
                                        userId,
                                        userName,
                                        serverId,
                                        serverName,
                                        platId,
                                        platName,
                                        level,
                                        timeStap,
                                        jobId,
                                        vipFlag,
                                        accId,
                                        appid,
                                        channelName
                         );
        return url;
    }

    private string FomatReqOfflineSignUrl()
    {
        string url = string.Format("user_id={0}&timestamp={1}&app_id={2}&is_special={3}", userId, timeStap, appid, vipFlag);

        //Logger.LogErrorFormat("当前请求离线消息的角色id为 ：{0}", userId);

        return url;
    }

    private string FormatReqVipCheckSignUrl()
    {
        string url = string.Format("uid={0}&server={1}&server_id={2}&role={3}&role_id={4}&plat_id={5}&job={6}&job_id={7}&vip={8}&vip_sign={9}",

                            accId, serverName, serverId, userName, roleId, vipPlatId, jobName, jobId, vipLevel, vipLevel);
        return url;
    }
    
    private string GetReqOnlineServiceUrl()
    {
        if (string.IsNullOrEmpty(openServiceSignStr))
            return "";
#if TEST_ONLINE_SERVICE
        string url = string.Format("http://{0}/?{1}", Global.ONLINE_SERVICE_ADDRESS, openServiceSignStr);
#else
        string url = string.Format("https://{0}/?{1}", Global.ONLINE_SERVICE_ADDRESS, openServiceSignStr);
#endif
        return url;
    }

    private string GetReqOfflineServiceUrl()
    {
        if (string.IsNullOrEmpty(offlineInfoSignStr))
            return "";
        string formatUrl = string.Format("https://{0}/user/unread?{1}", Global.ONLINE_SERVICE_REQ_ADDRESS, offlineInfoSignStr);
        //Logger.LogError("FormatReqOfflineInfoUrl Req offline service unread url = " + formatUrl);
        return formatUrl;
    }

    private string GetReqVipCheckUrl()
    {
        if (string.IsNullOrEmpty(vipAuthSignStr))
            return "";
        string formatUrl = string.Format("http://{0}?{1}", Global.ONLINE_SERVICE_VIP_CHECK_ADDRESS, vipAuthSignStr);
        return formatUrl;
    }

    private void SetCurrBaseJobId()
    {
        JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
        if (jobData != null)
        {
            if (jobData.JobType == 0)
            {
                jobId = jobData.ID.ToString();
            }
            else
            {
                jobId = jobData.prejob.ToString();
            }
            jobName = jobData.Name;
        }
    }

    private void TrySetUrlParams()
    {
        SetCurrBaseJobId();
        userName = PlayerBaseData.GetInstance().Name;
        serverId = ClientApplication.adminServer.id.ToString();
        serverName = ClientApplication.adminServer.name;
        //platId = Global.SDKChannelName[(int)(Global.Settings.sdkChannel)];
        platId = PluginManager.GetInstance().GetOnlineServiceBuildPlatformId();
        //platName = Global.SDKChannelName[(int)(Global.Settings.sdkChannel)];
        platName = PluginManager.GetInstance().GetOnlineServicePlatformName();
        level = PlayerBaseData.GetInstance().VipLevel.ToString();
        //xzl
        timeStap = TimeManager.GetInstance().GetServerTime().ToString();
        channelName = PluginManager.GetInstance().GetChannelName();

        //VIP认证新增start
        #if UNITY_EDITOR
        accId = ClientApplication.playerinfo.accid.ToString();
        #else
        accId = ClientApplication.playerinfo.openuid;
        #endif
        roleId = PlayerBaseData.GetInstance().RoleID.ToString();
        vipLevel = PlayerBaseData.GetInstance().VipLevel.ToString();
        vipPlatId = PluginManager.GetInstance().GetBuildPlatformId();
        //VIP认证新增end

        //默认客户端不直接请求vip客服链接 需要先经过恺英的vip认证 ！！！
        vipFlag = "0";  
        /*
        if (bSpecialService)
        {
            vipFlag = IsSatisfiedVipLevel() ? "1" : "0";
        }
        else
        {
            vipFlag = "0";
        }
        */
        appid = TR.Value("zymg_onlineservice_app_id");
        userId = string.Format("{0}_{1}_{2}", serverId, vipFlag, roleId);
    }

    #region UnUsed
    private void UrlEncodeOnRecSign(params string[] urlParams)
    {
        if (urlParams != null)
        {
            if (urlParams.Length != urlParamCount)
            {
               // Logger.LogProcessFormat("Private Url Params length != {0} !!!", urlParamCount);
                return;
            }
            for (int i = 0; i < urlParamCount; i++)
            {
                string paramTemp = urlParams[i];
                urlParams[i] = UrlEncode(paramTemp);
            }
            //if (urlParamStrs == null || urlParamStrs.Length != urlParamCount)
            //{
            //   Logger.LogProcessFormat("Global Url Params Error !!!");
            //   return;
            //}
            //for (int i = 0; i < urlParamCount; i++)
            //{
            //    urlParamStrs[i] = urlParams[i];
            //}
        }
    }

    public string UrlEncode(string input)
    {
        StringBuilder newBytes = new StringBuilder();
        var urf8Bytes = Encoding.UTF8.GetBytes(input);
        /*
       foreach (var item in urf8Bytes)
       {
           if (IsReverseChar((char)item))
           {
               newBytes.Append('%');
               newBytes.Append(ByteToHex(item));

           }
           else
               newBytes.Append((char)item);
       }
       */
       for (int i = 0; i < urf8Bytes.Length; i++)
       {
           var item = urf8Bytes[i];
           if (IsReverseChar((char)item))
           {
               newBytes.Append('%');
               newBytes.Append(ByteToHex(item));

           }
           else
               newBytes.Append((char)item);
       }
           
        return newBytes.ToString();
    }

    private bool IsReverseChar(char c)
    {
        return !((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')
                || c == '-' || c == '_' || c == '.' || c == '~');
    }

    private string ByteToHex(byte b)
    {
        return b.ToString("x2");
    }
    #endregion
}

public class WaitHttpRespensedService : BaseCustomEnum<HTTPAsyncRequest.eState>, IEnumerator
{
    protected string url = null;
    protected HTTPAsyncRequest req = null;

    public WaitHttpRespensedService(string url, int timeout)
    {
        this.url = url;
        this.req = new HTTPAsyncRequest();
        this.req.SendHttpRequst(url, timeout);
        Logger.LogProcessFormat("[WaitHttpPublishContent] 开始 url {0}, {1}", url, req.GetState());
    }

    public string GetResultString()
    {
        Logger.LogProcessFormat("[WaitHttpPublishContent] String 获取值 url {0}, {1}", url, req.GetState());
        if (null != req && req.GetState() == HTTPAsyncRequest.eState.Success)
        {
            return req.GetResString();
        }
        return null;
    }

    public byte[] GetResultByteArray()
    {
        Logger.LogProcessFormat("[WaitHttpPublishContent] ByteArray 获取值 url {0}, {1}", url, req.GetState());
        if (null != req && req.GetState() == HTTPAsyncRequest.eState.Success)
        {
            return _Base64DecodeToBytes(req.GetResString());
        }
        return null;
    }

    public bool MoveNext()
    {
        mResult = req.GetState();
        if (mResult == HTTPAsyncRequest.eState.Error)
        {

        }
        return null != req && req.GetState() == HTTPAsyncRequest.eState.Wait;
    }

    public void Reset()
    {

    }

    public object Current
    {
        get { return null; }
    }

    private byte[] _Base64DecodeToBytes(string base64EncodeDate)
    {
        return System.Convert.FromBase64String(base64EncodeDate);
    }
}