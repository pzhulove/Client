using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using XUPorterJSON;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using LitJson;
namespace GameClient
{
    public enum VersionUpdateState
    {
        None,
        CheckPathPermission,
        CheckNativeVerion,
        CheckNetwork,
        CheckVersionServer,
        CheckRemoteVersion,
        CheckExpireVersion,
        FinishCheckVersion,
        GetPatchDiffList,
        DownloadPatch,
        CheckPatch,
        InstallPatch,
        FinishUpdate,
        WaitForRestart,
        WaitFullUpdate,
    }

    public enum VersionCheckResult
    {
        None,             // 无 （结果还未出）
        ErrorOccur,       // 出现错误
        Lastest,          // 最新
        NeedHotUpdate   , // 需要热更新
        NeedUpGrade     , // 需要整包更新
    }

    public class HotUpdateDownloader : MonoSingleton<HotUpdateDownloader>
    {
        delegate void OnDownloadPatchFailed();

        protected class DiffAssetDesc
        {
            public string assetUrl;
            public string assetMD5;
            public long assetBytes;
        }

		protected class HotUpdateConfig
		{
			/// <summary>
			/// "获取服务器配置文件失败，是否重试？"
			/// </summary>
			public string patchIndexFileFetchInfo  { get; set; }

			/// <summary>
			/// "读取本地版本信息失败，安装包被破坏，请重新下载安装包！"
			/// </summary>
			public string packageNotValid 		{ get; set; }

			/// <summary>
			///  "亲爱的玩家，莉娅发布了最新的游戏版本，点击[下载]获取游戏的最新版本！"
			/// </summary>
			public string forceUpdate {get; set;}

			/// <summary>
			///	string msgInfo = "本地版本高于服务器版本，请等待服务器维护完成！";
			/// </summary>
			public string packageVersionIsHigh {get;set;}

			/// <summary>
			/// string downloadWithoutWifi =string.Format( "检测到当前设备未连接到无线局域网（WiFi），本次更新共{0}MB，已下载{1}MB，是否确认更新？", totalSizeMB.ToString("G3"), downloadSizeMB.ToString("G3"));
			/// </summary>
			public string patchZipFileFetchInfo {get;set;}

			/// <summary>
			///	当前网络不可用，请检查是否链接了可用的WiFi或移动网络
			/// </summary>
			public string versionFileFetchInfo {get;set;}

			///<summary>
			/// string msgInfo = "网络连接遇到问题，是否重试？";
			/// </summary>
			public string networkErrorInfo {get; set; }

			/// <summary>
			/// "由于网络问题导致更新的压缩包损坏，是否重新下载？"
			/// </summary>
			public string pathZipFileDownloadError { get; set; }

			/// <summary>
			/// 打到包内的url地址
			/// </summary>
			/// <value>The full package URL.</value>
			public string fullPackageDownloadUrl { get; set; }
		}

		private HotUpdateConfig mHotUpdateConfig = null;

#if APPLE_STORE
		private const string kHotUpdateConfigFileName = "hotupdateconfig.conf";

		private HotUpdateConfig _loadHotUpdateConfig()
		{
            if (null != mHotUpdateConfig)
			{
				return mHotUpdateConfig;
			}

			string jsonContent = string.Empty;
			try 
			{
				if (FileArchiveAccessor.LoadFileInLocalFileArchive(kHotUpdateConfigFileName, out jsonContent))
				{
					mHotUpdateConfig = LitJson.JsonMapper.ToObject<HotUpdateConfig> (jsonContent);	
				}
			} 
			catch (Exception e)
			{
				Logger.LogErrorFormat("[] 读取或转json失败 {0}", kHotUpdateConfigFileName);
			}

			return mHotUpdateConfig;
		}
#endif

        protected class VersionDesc
        {
            public void Reset()
            {
                m_VersionString = null;
                m_VersionTbl = null;
                m_LocalNewer = false;
                m_IsVersionOK = false;

                if(null == m_VersionNumber)
                    m_VersionNumber = new int[4] { 1, 0, 1, 0 };
                else
                {
                    m_VersionNumber[0] = 1;
                    m_VersionNumber[1] = 0;
                    m_VersionNumber[2] = 1;
                    m_VersionNumber[3] = 0;
                }
            }
            
            public Hashtable m_VersionTbl = null;
            public bool m_LocalNewer = false;
            public bool m_IsVersionOK = false;
            public int[] m_VersionNumber = new int[4] { 1, 0, 1, 0 };
            public string m_VersionString = null;
        }

        protected readonly string[] PLATFORM_STRING_TABLE = new string[]
        {
            "pc",
            "ios",
            "android",
        };

        protected DiffAssetDesc[] m_DiffAssetDescArray = null;

        protected long m_TotalSize = 0;
        protected long m_DownloadedSize = 0;

        protected readonly int DOWNLOAD_CHUNK_SIZE = 1024 * 1024; /// 1M
        protected readonly int REQUEST_TIME_LIMIT = 16 * 1000;

        //protected readonly string VERSION_INFO_FILE_NAME = "version.json";
        protected readonly string VERSION_INFO_FILE_NAME = "version.json";
		/// <summary>
		/// 整包下载地址的名字
		/// </summary>
		protected readonly string FULLPACKAGE_INFO_FILE_NAME = "full-package.json";
        protected readonly string VERSION_EXPIRE_FILE_NAME = "version-expire.json";///过期版本号文件
        protected readonly string UPDATA_SERVER_FILE_NAME = "updateserver.json";
        protected readonly string UPDATA_SERVER_HACK_FILE_NAME = "updateserver-hack.json";
        protected readonly string VERSION_INFO_FILE_LOCAL_PATH = "";

        protected readonly string FORCE_UPDATE_PACKAGE_NAME = "fullupdateurl.json";
        protected string m_ForceUpdateUrl = "";

        protected readonly string DELETE_PACKAGE_LIST_NAME = "delpackagelist.json";

        protected readonly string DEFAULT_VERSION = "1.0.1.0";

        protected CustomPlatform m_Platform = CustomPlatform.PC;
        protected string PLATFORM_STRING = "";

        protected bool m_LocalHost = false;
        protected bool m_LocalNewer = false;
        protected string m_NativeVersion = "";
        protected string m_RemoteVersion = "";
        protected string m_ExpireVersion = "";
        protected VersionCheckResult m_CheckRes = VersionCheckResult.Lastest;

        protected VersionDesc m_CurNativeVerDesc = new VersionDesc();
        protected VersionDesc m_CurRemoteVerDesc = new VersionDesc();

        protected VersionUpdateFrame m_UI = null;

        protected Hashtable m_VersionTbl = null;
        protected ArrayList m_ServerList = null;
        protected VersionUpdateState m_UpdateState = VersionUpdateState.None;

        protected bool m_HasSendErrorMail = false;

        public VersionUpdateState updateState
        {
            get { return m_UpdateState; }
        }

        public enum CustomPlatform
        {
            PC,
            iOS,
            Android,

            MaxPlatformNum
        }

        enum VersionType
        {
            ServerMajor,
            ServerMinor,
            CustomMajor,
            CustomPatch,

            VersionTypeNum,
        }

        // Use this for initialization
        void Start()
        {
#if UNITY_ANDROID
            m_Platform = CustomPlatform.Android;
#elif UNITY_IOS
            m_Platform = CustomPlatform.iOS;
#else
            m_Platform = CustomPlatform.PC;
#endif

            PLATFORM_STRING = PLATFORM_STRING_TABLE[(int)m_Platform];

            if(!_LoadHackServerList())
                _LoadServerList();
        }

        public void DoHotUpdate(VersionUpdateFrame frame)
        {
            m_UI = frame;
            StartCoroutine(_ProcessHotUpdate());
        }

        public void CheckHotUpdateVersion()
        {
            StartCoroutine(_CheckHotUpdateVersion());
        }

        protected IEnumerator _CheckHotUpdateVersion()
        {
            VersionManager.instance.m_IsLastest = false;
            m_CheckRes = VersionCheckResult.None;
            yield return Yielders.EndOfFrame;

            yield return _FetchVersion();
            Debug.Log("_CheckHotUpdateVersion CustomMajor");

            int majorIdx = (int)VersionType.CustomMajor;
            if (m_CurNativeVerDesc.m_VersionNumber[majorIdx] == m_CurRemoteVerDesc.m_VersionNumber[majorIdx])
            {
                int patchIdx = (int)VersionType.CustomPatch;
                if (m_CurNativeVerDesc.m_VersionNumber[patchIdx] < m_CurRemoteVerDesc.m_VersionNumber[patchIdx])
                {
                    m_CheckRes = VersionCheckResult.NeedHotUpdate;
                    yield break;
                }
            }
            else if(m_CurNativeVerDesc.m_VersionNumber[majorIdx] < m_CurRemoteVerDesc.m_VersionNumber[majorIdx])
            {
                m_CheckRes = VersionCheckResult.NeedHotUpdate;
                yield break;
            }

            m_CheckRes = VersionCheckResult.Lastest;
            VersionManager.instance.m_IsLastest = true;
            Debug.Log("_CheckHotUpdateVersion End");
        }

        public VersionCheckResult GetVersionCheckRes()
        {
            return m_CheckRes;
        }

        static public void RestartApp()
        {
            Debug.Log("Restart application has been called!");
#if UNITY_ANDROID
            Debug.Log("Restart application step 0");
            AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            Debug.Log("Restart application step 1");
            AndroidJavaObject javaObject = javaClass.GetStatic<AndroidJavaObject> ("currentActivity");
            Debug.Log("Restart application step 2");
            javaObject.Call("restartApplication");
            Debug.Log("Restart application step 3");
#else
            Logger.LogWarning("This function can only be called by android platform!");
#endif
        }


        IEnumerator _FetchVersion()
        {
#if APPLE_STORE
			_loadHotUpdateConfig();
#endif

            yield return _GetNativeVersion();

            if (!m_CurNativeVerDesc.m_IsVersionOK)
            {
                string msg = "读取本地版本信息失败，安装包被破坏，请重新下载安装包！";
#if APPLE_STORE
				if (null != mHotUpdateConfig) 
				{
					msg = mHotUpdateConfig.packageNotValid;
				}
#endif
                GameClient.SystemNotifyManager.BaseMsgBoxOK(msg, _OnConfirmQuit_OK);
                yield break;
            }

            yield return _GetRemoteVersion();

            if (!m_CurRemoteVerDesc.m_IsVersionOK)
            {
                string info = "获取服务器配置文件失败，是否重试？";
#if APPLE_STORE
				if (null != mHotUpdateConfig) 
				{
					info = mHotUpdateConfig.patchIndexFileFetchInfo;
				}
#endif
                GameClient.SystemNotifyManager.HotUpdateMsgBoxOkCancel(info, _OnRetryHotUpdate_OK, _OnRetryHotUpdate_Cancel);
                yield break;
            }
            Debug.Log("_FetchVersion Version End");
        }

        public IEnumerator ForceFullUpdate()
        {
            yield return _ForceFullUpdate();
        }

        WaitForEndOfFrame WAIT_FOR_ENDOFFRAME = new WaitForEndOfFrame();
        IEnumerator _ForceFullUpdate()
        {
            m_UpdateState = VersionUpdateState.WaitFullUpdate;

            yield return _GetForceUpdateUrl();
            string msgInfo = "亲爱的玩家，莉娅发布了最新的游戏版本，点击[下载]获取游戏的最新版本！";
#if APPLE_STORE
			if (null != mHotUpdateConfig) 
			{
				msgInfo = mHotUpdateConfig.forceUpdate;
			}

            _openAppStoreForceUpdateFrame();
#endif
            GameClient.SystemNotifyManager.BaseMsgBoxOK(msgInfo, _ReopenUrl, "下载");
        }

        private void _openAppStoreForceUpdateFrame()
        {
#if APPLE_STORE
if (!string.IsNullOrEmpty(m_ForceUpdateUrl))
            {

                if (!ClientSystemManager.instance.IsFrameOpen(typeof(VersionAppStoreForceUpdateFrame)))
                {

                    ClientSystemManager.instance.OpenFrame<VersionAppStoreForceUpdateFrame>(FrameLayer.TopMost, m_ForceUpdateUrl);
                }
            }
#endif
        }

        void _ReopenUrl()
        {
			if (!string.IsNullOrEmpty (m_ForceUpdateUrl)) 
			{
				Application.OpenURL (m_ForceUpdateUrl);
			}
			else
			{
				Application.OpenURL ("http://ald.xy.com");
			}

            StartCoroutine(_ForceFullUpdate());
        }

        IEnumerator _GetForceUpdateUrl()
        {
#if APPLE_STORE
			_loadHotUpdateConfig ();
			
			//string forceUpdateUrlFile = Path.Combine(_GetHotfixAccessUrlProtocol(), FORCE_UPDATE_PACKAGE_NAME);
            Logger.Log("Load hot-fix version from persistent stream...");
            //WWW forceUpdateUrlFileWWW = new WWW(forceUpdateUrlFile);
            //yield return forceUpdateUrlFileWWW;

            m_ForceUpdateUrl = "";
			string configPath = Path.Combine(_GetResourceServerRoot(), FULLPACKAGE_INFO_FILE_NAME);
			configPath = configPath.Replace("\\", "/");

			BaseWaitHttpRequest waitHttp = new BaseWaitHttpRequest ();
			waitHttp.url     = configPath;
            waitHttp.gaptime = 200;
            waitHttp.reconnectCnt = 0;
            waitHttp.timeout = 1000;

            yield return waitHttp;

			if (BaseWaitHttpRequest.eState.Success == waitHttp.GetResult ()) 
			{
				m_ForceUpdateUrl = waitHttp.GetResultString ();
			} 
			else
			{
				if (null != mHotUpdateConfig) 
				{
					m_ForceUpdateUrl = mHotUpdateConfig.fullPackageDownloadUrl;
				} 
			}

			if (string.IsNullOrEmpty (m_ForceUpdateUrl)) 
            {
				m_ForceUpdateUrl = "http://ald.xy.com";
			}
#else
			
			string forceUpdateUrlFile = Path.Combine(_GetHotfixAccessUrlProtocol(), FORCE_UPDATE_PACKAGE_NAME);
            Logger.Log("Load hot-fix version from persistent stream...");
            WWW forceUpdateUrlFileWWW = new WWW(forceUpdateUrlFile);
            yield return forceUpdateUrlFileWWW;

            m_ForceUpdateUrl = "";
            if (null == forceUpdateUrlFileWWW.error)
            {
                try
                {
                    ArrayList UrlList = MiniJSON.jsonDecode(forceUpdateUrlFileWWW.text) as ArrayList;
                    if (UrlList != null && UrlList.Count > 0)
                    {
                        m_ForceUpdateUrl = UrlList[0] as string;
                    }
                }
                catch (System.Exception e)
                {
                    Logger.LogAssetFormat("Get update url form json has failed! Exception:" + e.ToString());
                }
            }
#endif
        }

        IEnumerator _ProcessHotUpdate()
        {
            yield return Yielders.EndOfFrame;

            yield return _FetchVersion();

            m_NativeVersion = m_CurNativeVerDesc.m_VersionString;
            m_VersionTbl = m_CurNativeVerDesc.m_VersionTbl;
            m_LocalNewer = m_CurNativeVerDesc.m_LocalNewer;
            m_RemoteVersion = m_CurRemoteVerDesc.m_VersionString;

            int majorIdx = (int)VersionType.CustomMajor;
            if (m_CurNativeVerDesc.m_VersionNumber[majorIdx] < m_CurRemoteVerDesc.m_VersionNumber[majorIdx])
            {///本地版本低于服务器 需要整包更新
                //string msgInfo = "检测到有全新的版本上线，请去应用商店获取最新的版本！";
                //GameClient.SystemNotifyManager.BaseMsgBoxOK(msgInfo, _OnNeedDownloadFullVersion_OK);

#if APPLE_STORE
				StartCoroutine(_ForceFullUpdate());
#else
                string msgInfo = "亲爱的玩家，艾米莉亚发布了最新的游戏版本，请获取最新版本的游戏！";
                GameClient.SystemNotifyManager.BaseMsgBoxOK(msgInfo, _OnNeedDownloadFullVersion_OK);
#endif

                ///热更新完毕
                _DisplayUpdateState("最新的游戏版本已经发布，请获取最新的游戏版本");
                _DisplayUpdateInfo(100, "祝您生活愉快！");

                VersionManager.instance.m_IsLastest = true;
                m_UpdateState = VersionUpdateState.FinishUpdate;
                m_CheckRes = VersionCheckResult.NeedUpGrade;
                yield break;
            }
            else if (m_CurNativeVerDesc.m_VersionNumber[majorIdx] > m_CurRemoteVerDesc.m_VersionNumber[majorIdx])
            {/// 本地版本高于服务器版本 提示需要等待服务器更新
                string msgInfo = "本地版本高于服务器版本，请等待服务器维护完成！";
#if APPLE_STORE
				if (null != mHotUpdateConfig) 
				{
					msgInfo = mHotUpdateConfig.packageVersionIsHigh;
				}
#endif
                GameClient.SystemNotifyManager.BaseMsgBoxOK(msgInfo, _OnWaitServerUpdate_OK);
                m_CheckRes = VersionCheckResult.Lastest ;
                yield break;
            }
            else
            {
                int patchIdx = (int)VersionType.CustomPatch;
                if (m_CurNativeVerDesc.m_VersionNumber[patchIdx] < m_CurRemoteVerDesc.m_VersionNumber[patchIdx])
                {/// 本地资源版本低于远端版本 需要热更新

                    /// 检查本地版本和服务器过期版本号
                    yield return StartCoroutine(_GetExpireVersion());

                    bool bIsExpireOK = false;
                    string[] expireVerList = null;
                    if (!string.IsNullOrEmpty(m_ExpireVersion))
                    {
                        expireVerList = m_ExpireVersion.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
                        if ((int)VersionType.VersionTypeNum == expireVerList.Length)
                            bIsExpireOK = true;
                        else
                            Logger.LogAsset("Invalid expire version string:" + m_ExpireVersion);
                    }

                    if (!bIsExpireOK)
                    {
                        string info = "获取服务器配置文件失败，是否重试？";
#if APPLE_STORE
						if (null != mHotUpdateConfig) 
						{
							info = mHotUpdateConfig.patchIndexFileFetchInfo;
						}
#endif
                        GameClient.SystemNotifyManager.HotUpdateMsgBoxOkCancel(info, _OnRetryHotUpdate_OK, _OnRetryHotUpdate_Cancel);
                        yield break;
                    }

                    int[] expireVers = new int[expireVerList.Length];
                    for (int i = 0; i < expireVerList.Length; ++i)
                        expireVers[i] = int.Parse(expireVerList[i]);

                    if(m_CurNativeVerDesc.m_VersionNumber[patchIdx] < expireVers[patchIdx])
                    {/// 本地版本小于热更新服务器支持的最低版本
                        //string msgInfo = "亲爱的玩家，莉娅发布了最新的游戏版本，请去应用商店获取最新的版本！";
                        string msgInfo = "亲爱的玩家，莉娅发布了最新的游戏版本，请获取最新版本的游戏！";
#if APPLE_STORE
						if (null != mHotUpdateConfig) 
						{
							msgInfo = mHotUpdateConfig.forceUpdate;
						}

                        _openAppStoreForceUpdateFrame();
#endif

#if APPLE_STORE
						GameClient.SystemNotifyManager.BaseMsgBoxOK(msgInfo, _ReopenUrl, "立即更新");
#else
						GameClient.SystemNotifyManager.BaseMsgBoxOK(msgInfo, _OnNeedDownloadFullVersion_OK);
#endif

                        ///热更新完毕
                        _DisplayUpdateState("最新的游戏版本已经发布，请获取最新的游戏版本");
                        _DisplayUpdateInfo(100, "祝您生活愉快！");

                        VersionManager.instance.m_IsLastest = true;
                        m_UpdateState = VersionUpdateState.FinishUpdate;
                        m_CheckRes = VersionCheckResult.NeedUpGrade;

                        yield break;
                    }
                    else
                    {/// 本地版本号在热更服务器支持的版本内
                        StartCoroutine(_ProcessUpdatePatches());
                        yield break;
                    }
                }
                else if(m_CurNativeVerDesc.m_VersionNumber[patchIdx] > m_CurRemoteVerDesc.m_VersionNumber[patchIdx])
                {/// 诡异了 本地版本居然比更新服务器版本还高
                    /// 删除Hotfix目录下的文件
                    if(!m_LocalNewer)
                    {
                        _ClearHotUpdateCaches();
                        StartCoroutine(_ProcessHotUpdate());
                        yield break;
                    }
                    else
                    {
                        string content = string.Format("当前远端版本号：{0}，玩家本地版本号：{1}", m_CurRemoteVerDesc.m_VersionNumber[patchIdx], m_CurNativeVerDesc.m_VersionNumber[patchIdx]);
                        _SendErrorEmail("检测到玩家版本高于热更版本", content);
                    }

                    m_CheckRes = VersionCheckResult.Lastest;
                }

                Debug.LogWarning("##################################### 不对");
                ///热更新完毕
                _DisplayUpdateState("当前版本为最新");
                _DisplayUpdateInfo(100, "祝您游戏愉快！");

                VersionManager.instance.m_IsLastest = true;
                m_UpdateState = VersionUpdateState.FinishUpdate;
            }
        }

        IEnumerator _ProcessUpdatePatches()
        {
            Logger.Log("Begin update path!");
            
            string msg = "正在获取更新文件列表...";
            _DisplayUpdateState(msg);
            _DisplayUpdateInfo(7, "");

            string packageFile = "package-" + m_NativeVersion + "-" + m_RemoteVersion + ".txt";

            Debug.Log("Target patch:" + packageFile);

            m_UpdateState = VersionUpdateState.GetPatchDiffList;

            string diffListData = null;
            string diffFileUrl = null;
            string lastError = "";
            WWW diffListWWW = null;
            int retryCnt = 0;
            bool isConnectOK = false;
            do
            {
                /// 首先获取DiffFile
                diffFileUrl = _GetDiffListFileUrl(packageFile).Replace('\\','/');
                diffListWWW = new WWW(diffFileUrl);
                yield return diffListWWW;

                if(null == diffListWWW.error && !string.IsNullOrEmpty(diffListWWW.text))
                {
                    diffListData = diffListWWW.text;
                    isConnectOK = true;
                }
                else
                {
                    lastError = diffListWWW.error;
                    Logger.LogAssetFormat("Download difference list file from server has failed (url:{0})!", diffFileUrl);
                }
                diffListWWW.Dispose();

                if (isConnectOK)
                    break;
                else
                    ++retryCnt;

            } while (retryCnt < 3);

            if(isConnectOK && !string.IsNullOrEmpty(diffListData))
            {
                if (_ParseDiffList(diffListData, ref m_TotalSize, ref m_DownloadedSize))
                {
                    if (!_IsWifiUsed())
                    {/// 非Wifi环境下 提示是否下载更新包
                        float totalSizeMB = m_TotalSize / (1024.0f * 1024.0f);
                        float downloadSizeMB = m_DownloadedSize / (1024.0f * 1024.0f);

                        string downloadWithoutWifi =string.Format( "检测到当前设备未连接到无线局域网（WiFi），本次更新共{0}MB，已下载{1}MB，是否确认更新？", totalSizeMB.ToString("G3"), downloadSizeMB.ToString("G3"));
#if APPLE_STORE
						if (null != mHotUpdateConfig) 
						{
							try {
								downloadWithoutWifi = string.Format(mHotUpdateConfig.patchZipFileFetchInfo, totalSizeMB.ToString("G3"), downloadSizeMB.ToString("G3"));
							} catch (Exception e) {
								Logger.LogErrorFormat ("[HotUpdate] patch with error {0}", e.ToString());
							}
						}
#endif
						GameClient.SystemNotifyManager.HotUpdateMsgBoxOkCancel(downloadWithoutWifi, _OnConfirmHotUpdate_OK, _OnConfirmHotUpdate_Cancel);
                    }
                    else
                        _OnConfirmHotUpdate_OK();
                }
            }
            else
            {
                string content = string.Format("问题流程：{0}，问题地址（URL）：{1}，请求列表{2}，错误详情：{3}", m_UpdateState.ToString(), diffFileUrl, packageFile, lastError);
                _SendErrorEmail("热更新获取差异列表失败", content);

#if !APPLE_STORE
                if (_IsFileNotFound(lastError))
                {
                    _DisplayUpdateState("热更新检查完毕");
                    _DisplayUpdateInfo(100, "未发现需要更新的文件列表！");
                    m_UpdateState = VersionUpdateState.FinishUpdate;
                    m_CheckRes = VersionCheckResult.Lastest;

                    yield break;
                }
                else
#endif
                {
                    string info = "获取服务器配置文件失败，是否重试？";
#if APPLE_STORE
					if (null != mHotUpdateConfig) 
					{
						info = mHotUpdateConfig.patchIndexFileFetchInfo;
					}
#endif
                    GameClient.SystemNotifyManager.HotUpdateMsgBoxOkCancel(info, _OnRetryHotUpdate_OK, _OnRetryHotUpdate_Cancel);
                }

            }
        }

        IEnumerator _GetRemoteVersion()
        {
            m_CurRemoteVerDesc.Reset();

            Logger.Log("Fetch server config file...");
            if(!Global.Settings.loadFromPackage)
            {
                Logger.LogWarning("To enable hot-fix please enable load from package!");
                //m_RemoteVersion = m_NativeVersion;

                m_CurRemoteVerDesc.m_VersionString = m_CurNativeVerDesc.m_VersionString;

                yield break;
            }

			float startTime = Time.realtimeSinceStartup;

            string msg = "正在获取服务器配置文件...";
            //VersionUpdateFrame versionUpdateFrame = ClientSystemManager.instance.GetFrame(typeof(VersionUpdateFrame)) as VersionUpdateFrame;
            _DisplayUpdateState(msg);
            _DisplayUpdateInfo(5, "");

            string configPath = null;
            int retryCnt = 0;

            string lastError = "";

            bool isRemoteOK = false;

            string usrAgreementString = "";
#if UNITY_IOS || UNITY_EDITOR
			while (!isRemoteOK)
			{
#endif
	            //WWWForm form = new WWWForm();
	            //form.AddField("Cache-Control", "max-age=0");
	            //WWW serverWWW = null;
	            do 
	            {
	                configPath = Path.Combine(_GetResourceServerRoot(), VERSION_INFO_FILE_NAME);
	                configPath = configPath.Replace("\\", "/");

					BaseWaitHttpRequest configReq = new BaseWaitHttpRequest();

                    if (configPath.Contains("?"))
                    {
                        configReq.url 	  = configPath + string.Format("&tsp={0}", Utility.GetCurrentTimeUnix()/300);
                    }
                    else 
                    {
                        configReq.url 	  = configPath + string.Format("?tsp={0}", Utility.GetCurrentTimeUnix()/300);
                    }

					configReq.reconnectCnt = 0;

	                Debug.Log("URL:[" + configReq.url + "]!");
	                m_UpdateState = VersionUpdateState.CheckNetwork;
	                ///serverWWW = new WWW(configPath, form);
	                //serverWWW = new WWW(configPath);

					yield return configReq;

	                //yield return serverWWW;

					if (BaseWaitHttpRequest.eState.Success == configReq.GetResult())
	                {
	                    Hashtable verTbl = null;
                        //m_RemoteVersion = _GetVersionFromJson(serverWWW.text, ref verTbl);
                        usrAgreementString = configReq.GetResultString();

                        m_CurRemoteVerDesc.m_VersionString = _GetVersionFromJson(usrAgreementString, ref verTbl);
	                    m_UpdateState = VersionUpdateState.CheckRemoteVersion;

	                    isRemoteOK = true;
	                }
	                else
	                {
	                    //lastError = configReq.error;
	                    Logger.LogAsset("严重错误:连接资源服务器失败[" + lastError + "]!");
	                }

	                //serverWWW.Dispose();

	                if (isRemoteOK)
	                    break;
	                else
	                    ++retryCnt;

	            } while (retryCnt < 3);
#if UNITY_IOS || UNITY_EDITOR
				if (!isRemoteOK)
				{
					bool isUserClick = false;
					string info = "当前网络不可用，请检查是否链接了可用的WiFi或移动网络";
#if APPLE_STORE
					if (null != mHotUpdateConfig && !string.IsNullOrEmpty(mHotUpdateConfig.versionFileFetchInfo)) 
					{
						info = mHotUpdateConfig.versionFileFetchInfo;
					}
#endif
					GameClient.SystemNotifyManager.HotUpdateMsgBoxOk(info, ()=>{ isUserClick = true; });
					while (!isUserClick) { yield return null; }
				}
			}
#endif

			float fetchTime = (Time.realtimeSinceStartup - startTime) * 1000;
			if (fetchTime >= 5000)
			{
				// 统计卡5秒以上的情况
				Logger.LogErrorFormat("version fetchTime:{0}", fetchTime);
			}


            if(!isRemoteOK)
            {
                /// 将服务器远端版本置为本地版本 为的是当玩家版本为新版本的情况下 
                /// 版本服务器不能连接的状态下可以正常登陆游戏
                //m_RemoteVersion = m_NativeVersion;
                m_CurRemoteVerDesc.m_VersionString = m_CurNativeVerDesc.m_VersionString;

                string content = string.Format("问题流程：{0}，问题地址（URL）：{1}，错误详情：{2}", m_UpdateState.ToString(), configPath, lastError);

                _SendErrorEmail("热更新连接版本服务器失败", content);
            }

            //Debug.Log("Remote Version:[" + m_RemoteVersion + "]!");
            Debug.Log("Remote Version:[" + m_CurRemoteVerDesc.m_VersionString + "]!");

            m_CurRemoteVerDesc.m_IsVersionOK = false;
            string[] remoteVerList = null;
            if (!string.IsNullOrEmpty(m_CurRemoteVerDesc.m_VersionString))
            {
                remoteVerList = m_CurRemoteVerDesc.m_VersionString.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
                if ((int)VersionType.VersionTypeNum == remoteVerList.Length)
                    m_CurRemoteVerDesc.m_IsVersionOK = true;
                else
                    Logger.LogAsset("Invalid remote version string:" + m_RemoteVersion);
            }

            if(m_CurRemoteVerDesc.m_IsVersionOK)
            {
                for (int i = 0; i < m_CurRemoteVerDesc.m_VersionNumber.Length; ++i)
                    m_CurRemoteVerDesc.m_VersionNumber[i] = int.Parse(remoteVerList[i]);
            }
            Debug.Log("_GetUserAgreementFromJson");
            /// 用户协议
            bool needAgreement = _GetUserAgreementFromJson(usrAgreementString);
            ClientSystemLogin.mOpenUserAgreement = needAgreement;
            //越狱的客服开关，由于xy和爱思使用同一热更地址，所以需要使用这个区分
            SDKChannel[] plt = _GetIOSOtherVIPAuth(usrAgreementString);
            Global.VipAuthSDKChannel = plt;
            //是否开启实名弹窗的开关
            SDKChannel[] cls = _GetUseRealNamePopWindowsFromJson(usrAgreementString);
            Global.RealNamePopWindowsChannel = cls;
            //是否开启sdk推送的开关（目前只有vivo）
            SDKChannel[] cls2 = _GetUseSDKPuchFromJson(usrAgreementString);
            Global.RealNamePopWindowsChannel = cls;
        }

        IEnumerator _GetExpireVersion()
        {
            Logger.Log("Fetch server config file...");
            if (!Global.Settings.loadFromPackage)
            {
                Logger.LogWarning("To enable hot-fix please enable load from package!");
                m_ExpireVersion = DEFAULT_VERSION;
                yield break;
            }

            string msg = "正在获取服务器配置文件...";
            _DisplayUpdateState(msg);

            string configPath = null;
            int retryCnt = 0;

            string lastError = "";

            bool isExpireOK = false;
            WWWForm form = new WWWForm();
            form.AddField("Cache-Control", "max-age=0");
            WWW serverWWW = null;
            do
            {
                configPath = Path.Combine(_GetResourceServerRoot(), VERSION_EXPIRE_FILE_NAME);
                configPath = configPath.Replace("\\", "/");

                Debug.Log("URL:[" + configPath + "]!");
                ///serverWWW = new WWW(configPath, form);
                serverWWW = new WWW(configPath);

                yield return serverWWW;

                if (null == serverWWW.error && !string.IsNullOrEmpty(serverWWW.text))
                {
                    Hashtable verTbl = null;
                    m_ExpireVersion = _GetVersionFromJson(serverWWW.text, ref verTbl);
                    m_UpdateState = VersionUpdateState.CheckExpireVersion;

                    isExpireOK = true;
                }
                else
                {
                    lastError = serverWWW.error;
                    Logger.LogAsset("严重错误:连接资源服务器失败[" + lastError + "]!");
                }

                serverWWW.Dispose();

                if (isExpireOK)
                    break;
                else
                    ++retryCnt;

            } while (retryCnt < 3);

            if (!isExpireOK)
            {
                /// 将服务器远端版本置为本地版本 为的是当玩家版本为新版本的情况下 
                /// 版本服务器不能连接的状态下可以正常登陆游戏
                m_ExpireVersion = DEFAULT_VERSION;
                string content = string.Format("问题流程：{0}，问题地址（URL）：{1}，错误详情：{2}", m_UpdateState.ToString(), configPath, lastError);

                _SendErrorEmail("热更新连接版本服务器失败", content);
            }

            Debug.Log("Expire Version:[" + m_ExpireVersion+ "]!");
        }

        IEnumerator _GetNativeVersion()
        {
            m_CurNativeVerDesc.Reset();

            Logger.Log("Fetch local version config file...");
            _DisplayUpdateState("读取本地版本信息... ");

            m_UpdateState = VersionUpdateState.CheckPathPermission;

            string localPersistentVerFile = Path.Combine(_GetHotfixAccessUrlProtocol(), VERSION_INFO_FILE_NAME);
            Logger.Log("Load hot-fix version from persistent stream...");
            WWW localPersistentVerFileWWW = new WWW(localPersistentVerFile);
            yield return localPersistentVerFileWWW;

            string verHotfixJson = null;
            Hashtable verTblHotfix = null;
            if (null == localPersistentVerFileWWW.error)
            {
                verHotfixJson = _GetVersionFromJson(localPersistentVerFileWWW.text, ref verTblHotfix);
                Debug.LogWarning("### Native hot-fix version:" + verHotfixJson);
            }
            else
            {/// 大版本更新 还未进行过Patch更新
                verHotfixJson = DEFAULT_VERSION;
            }
            localPersistentVerFileWWW.Dispose();

            /// 在未进行过热更的版本前 版本号文件应该来自于Resource下的Version文件
            string localStreamVerFile = Path.Combine(_GetLocalAccessUrlProtocol(), VERSION_INFO_FILE_NAME);
            Logger.Log("Load local version from asset stream...");
            WWW localStreamVerFileWWW = new WWW(localStreamVerFile);
            yield return localStreamVerFileWWW;

            string verLocalJson = null;
            Hashtable verTblLocal = null;
            if (null == localStreamVerFileWWW.error)
            {
                verLocalJson = _GetVersionFromJson(localStreamVerFileWWW.text, ref verTblLocal);
                Debug.LogWarning("### Native local version:" + verLocalJson);
            }
            else
            {
                // localStreamVerFile = Path.Combine(_GetLocalAccessUrlProtocol(), "version.json");
                // Logger.Log("Load local old version from asset stream...");
                // localStreamVerFileWWW = new WWW(localStreamVerFile);
                // yield return localStreamVerFileWWW;
                // if (null == localStreamVerFileWWW.error)
                // {
                //     verLocalJson = _GetVersionFromJson(localStreamVerFileWWW.text, ref verTblLocal);
                //     Debug.LogWarning("### Native local version:" + verLocalJson);
                // }
                // else
                // {
                    /// 这绝壁是严重错误
                    Logger.LogAsset("Oh crap! Load local version from persistent has failed!");
                    verLocalJson = DEFAULT_VERSION;
                    _SendErrorEmail("热更新检查包内版本文件错误", localStreamVerFileWWW.error + "版本号文件路径：" + localStreamVerFile);
                // }
            }
            localStreamVerFileWWW.Dispose();

            string[] hotfixVerList = verHotfixJson.Split('.');
            string[] localVerList = verLocalJson.Split('.');
            int hotfixPatchVersion = 0;
            int localPatchVersion = 0;
            int hotfixClientVersion = 0;
            int localClientVersion = 0;
            if ((int)VersionType.VersionTypeNum == hotfixVerList.Length)
            {
                hotfixClientVersion = int.Parse(hotfixVerList[2]);
                hotfixPatchVersion = int.Parse(hotfixVerList[3]);
            }
            else
                Debug.LogWarning("Hot-fix version string is invalid(while split version string to sub versions)!");
            if ((int)VersionType.VersionTypeNum == localVerList.Length)
            {
                localClientVersion = int.Parse(localVerList[2]);
                localPatchVersion = int.Parse(localVerList[3]);
            }
            else
                Debug.LogWarning("Local version string is invalid(while split version string to sub versions)!");
            
            if(hotfixPatchVersion > localPatchVersion && hotfixClientVersion >= localClientVersion)
            {
                //m_NativeVersion = verHotfixJson;
                //m_VersionTbl = verTblHotfix;
                //m_LocalNewer = false;

                m_CurNativeVerDesc.m_VersionString = verHotfixJson;
                m_CurNativeVerDesc.m_VersionTbl = verTblHotfix;
                m_CurNativeVerDesc.m_LocalNewer = false;
            }
            else
            {/// 如果本地包版本大于热更目录包 清理热更目录
                //m_NativeVersion = verLocalJson;
                //m_VersionTbl = verTblLocal;
                //m_LocalNewer = true;

                m_CurNativeVerDesc.m_VersionString = verLocalJson;
                m_CurNativeVerDesc.m_VersionTbl = verTblLocal;
                m_CurNativeVerDesc.m_LocalNewer = true;

                _ClearHotUpdateCaches();
            }

            m_UpdateState = VersionUpdateState.CheckNativeVerion;

            m_CurNativeVerDesc.m_IsVersionOK = false;
            string[] nativeVerList = null;
            if (!string.IsNullOrEmpty(m_CurNativeVerDesc.m_VersionString))
            {
                nativeVerList = m_CurNativeVerDesc.m_VersionString.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
                if ((int)VersionType.VersionTypeNum == nativeVerList.Length)
                    m_CurNativeVerDesc.m_IsVersionOK = true;
                else
                    Logger.LogAsset("Invalid native version string:" + m_CurNativeVerDesc.m_VersionString);
            }

            if (m_CurNativeVerDesc.m_IsVersionOK)
            {
                for (int i = 0; i < m_CurNativeVerDesc.m_VersionNumber.Length; ++i)
                    m_CurNativeVerDesc.m_VersionNumber[i] = int.Parse(nativeVerList[i]);
            }
        }

        protected void _OnConfirmHotUpdate_OK()
        {
            StartCoroutine(_UpdatePatches());
        }

        protected void _OnConfirmHotUpdate_Cancel()
        {
            Logger.Log("_OnConfirmHotUpdate_Cancel");
            Application.Quit();
        }

        protected void _OnRetryHotUpdate_OK()
        {
            Logger.Log("_OnRetryHotUpdate_OK");

            _ClearHotUpdateCaches();
            StartCoroutine(_ProcessHotUpdate());
        }
        protected void _OnRetryHotUpdate_Cancel()
        {
            Logger.Log("_OnRetryHotUpdate_Cancel");

            //string msg = "更新版本失败！";
            //string info = "请检查网络并确保游戏客户端没有被非法篡改！";
            //_DisplayUpdateState(msg);
            //m_UI.ResetProgress(info);
            //Application.Quit();

            VersionManager.instance.m_IsLastest = false;
            m_UpdateState = VersionUpdateState.FinishUpdate;
        }

        protected void _OnWaitServerUpdate_OK()
        {/// 连接到官网

        }
        protected void _OnNeedDownloadFullVersion_OK()
        {/// 连接到官网
            /// 没有官网暂时Unity3D官网代替
            //Application.OpenURL("http://unity3d.com/");
        }

        protected void _OnConfirmQuit_OK()
        {
            Application.Quit();
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        { // Always accept
            return true; //总是接受
        }

        IEnumerator _UpdatePatches()
        {
            Logger.Log("Begin downlad patches ...");

            string msg = "开始下载更新文件...";
            _DisplayUpdateState(msg);
            _DisplayUpdateInfo(9, "");

            ServicePointManager.DefaultConnectionLimit = 30;

            byte[] downloadCache = new byte[DOWNLOAD_CHUNK_SIZE];

            string assetFullUrl = "";
            string patchAssetName = "";

            m_UpdateState = VersionUpdateState.DownloadPatch;
            int nReadSize = 0;
            for (int i = 0; i < m_DiffAssetDescArray.Length; i++)
            {
                DiffAssetDesc assetDesc = m_DiffAssetDescArray[i];

                assetFullUrl = _GetDiffListFileUrl(assetDesc.assetUrl);// Path.Combine(_GetResourceServerRoot(), assetDesc.assetUrl);
                patchAssetName = Path.GetFileName(assetFullUrl);          // package_2.5.24.28_2.5.24.29.zip

                Logger.Log("[Download] patch asset url: " + assetDesc.assetUrl);// resource/common/package_2.5.24.28_2.5.24.29.zip
                Logger.Log("[Download] full asset url : " + assetFullUrl);
                Logger.Log("[Download] asset MD5 : " + assetDesc.assetMD5);     // 24652a56791d9d96c03a1e444df5ceec 
                Logger.Log("[Download] asset size : " + assetDesc.assetBytes);  // 10240000

                System.GC.Collect();

                // TODO if sDiffUrl is wrong or the resource is not valid
                long responseCountLength = 0;
                HttpWebRequest requestHttp = null;
                WebResponse webResponse = null;

                ///try
                ///{
                ///    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                ///}
                ///catch (Exception e)
                ///{
                ///    Logger.LogAsset("Get https download request count length has failed! Exception:" + e.ToString());
                ///}

                try
                {
                    requestHttp = HttpWebRequest.Create(assetFullUrl) as HttpWebRequest;
                    //pRequestHttp.Timeout = 10;

                    webResponse = requestHttp.GetResponse();

                    responseCountLength = webResponse.ContentLength;
                    requestHttp.KeepAlive = false;

                    webResponse.Close();
                    webResponse = null;

                    requestHttp.Abort();
                    requestHttp = null;
                }
                catch (Exception e)
                {
                    //BaseMsgBox.m_instance.Initialize(BROKEN_NET_READ_TIPS, ReDownloadNewAssetbundleByPoint);
                    string msgInfo = "网络连接遇到问题，是否重试？";
#if APPLE_STORE
					if (null != mHotUpdateConfig && !string.IsNullOrEmpty(mHotUpdateConfig.patchIndexFileFetchInfo)) 
					{
						msgInfo = mHotUpdateConfig.patchIndexFileFetchInfo;
					}
#endif
                    GameClient.SystemNotifyManager.HotUpdateMsgBoxOkCancel(msgInfo, _OnRetryHotUpdate_OK, _OnRetryHotUpdate_Cancel);

                    Logger.LogError("### Get http download request count length has failed! Exception:" + e.ToString());
                    yield break;
                }

                string assetDownloadPath = Path.Combine(Application.persistentDataPath, patchAssetName);
                long fileOffset = FileUtil.GetFileBytes(assetDownloadPath);

                Logger.Log("[Download] patch asset : " + patchAssetName + " save to " + assetDownloadPath);

                if (fileOffset < responseCountLength)
                {
                    FileStream fs;
                    if (-1 != fileOffset)
                    {/// 文件存在 操作文件指针到上次未写完的位置
                        fs = File.OpenWrite(assetDownloadPath);
                        fs.Seek(fileOffset, SeekOrigin.Current);
                    }
                    else
                    {
                        Logger.LogAsset("[Download] patch asset's url is not valid : " + assetFullUrl);
                        fs = new FileStream(assetDownloadPath, FileMode.Create);
                    }

                    requestHttp = null;
                    try
                    {
                        requestHttp = HttpWebRequest.Create(assetFullUrl) as HttpWebRequest;
                    }
                    catch (System.Exception e)
                    {
                        /// 
                        /// BaseMsgBox.m_instance.Initialize(BROKEN_NET_READ_TIPS, ReDownloadNewAssetbundleByPoint);

                        string msgInfo = "网络连接遇到问题，是否重试？";
#if APPLE_STORE
						if (null != mHotUpdateConfig && !string.IsNullOrEmpty(mHotUpdateConfig.patchIndexFileFetchInfo)) 
						{
							msgInfo = mHotUpdateConfig.patchIndexFileFetchInfo;
						}
#endif
                        GameClient.SystemNotifyManager.HotUpdateMsgBoxOkCancel(msgInfo, _OnRetryHotUpdate_OK, _OnRetryHotUpdate_Cancel);

                        Logger.LogError("### [Download] Create http request for download offset has failed! Exception: " + e.ToString());
                        yield break;
                    }

                    if (fileOffset > 0)
                        requestHttp.AddRange((int)fileOffset);

                    webResponse = requestHttp.GetResponse();

                    Stream ns = webResponse.GetResponseStream();
                    ns.ReadTimeout = REQUEST_TIME_LIMIT;

                    while (true)
                    {
                        try
                        {
                            nReadSize = ns.Read(downloadCache, 0, DOWNLOAD_CHUNK_SIZE);

                            if (nReadSize <= 0)
                                break;

                            fs.Write(downloadCache, 0, nReadSize);

                            int curProgress = (int)(((float)(fs.Length * 100) / m_TotalSize) * 0.69);
                            msg = "正在下载文件：";
                            _DisplayUpdateState(msg);

                            float MBDiv = 1 / (1024.0f * 1024.0f);
                            string sizeInfo = string.Format("  已下载{0}MB（共{1}MB）", (fs.Length * MBDiv).ToString("G3"), (m_TotalSize * MBDiv).ToString("G3"));
                            _DisplayUpdateInfo(curProgress + 10, patchAssetName + sizeInfo);
                            //DownloadWnd.Instance().Update((int)fs.Length);
                        }
                        catch (System.Exception e)
                        {
                            //BaseMsgBox.m_instance.Initialize(BROKEN_NET_READ_TIPS, ReDownloadNewAssetbundleByPoint);
                            Logger.LogAsset("[Download] Read http request data stream has failed! Exception: " + e.ToString());
                            yield break;
                        }

                        yield return nReadSize;
                    }

                    ns.Close();
                    fs.Close();

                    webResponse.Close();
                    webResponse = null;

                    requestHttp.Abort();
                    requestHttp = null;
                }
                else
                {
                    Logger.Log("[Download] Already Download the full patch");
                }

                m_UpdateState = VersionUpdateState.CheckPatch;
                string downloadedAssetMD5 = FileUtil.GetFileMD5(assetDownloadPath);
                if (downloadedAssetMD5.ToLower().Equals(assetDesc.assetMD5.ToLower()))
                {
                    msg = "正在解压文件...";
                    _DisplayUpdateState(msg);
                    _DisplayUpdateInfo(80, "");

                    m_UpdateState = VersionUpdateState.InstallPatch;
                    // uncompress all 
                    if(LibZip.LibZipFileReader.UncompressAll(Path.Combine( Application.persistentDataPath , patchAssetName)))
                    {
                        Logger.LogAsset("Uncompress complete!");
                        File.Delete(Path.Combine(Application.persistentDataPath, patchAssetName));
                    }
                    else
                    {
                        GameClient.SystemNotifyManager.BaseMsgBoxOK("解压更新文件失败，请清理设备存储空间后重试！", null);
                        VersionManager.instance.m_IsLastest = false;
                        m_UpdateState = VersionUpdateState.FinishUpdate;
                        _DisplayUpdateInfo(0, "解压更新文件失败，请清理设备存储空间后重试！");
                        yield break;
                    }
                }
                else
                {
                    //BaseMsgBox.m_instance.Initialize(BROKEN_NET_TIPS, ReDownloadNewAssetbundleByPoint);
                    // TODO break the loop
                    Logger.Log("[Download] Patch asset MD5 miss match!");
                    Logger.Log("    Downloaded asset MD5: " + downloadedAssetMD5);
                    Logger.Log("    Expected asset MD5: " + assetDesc.assetMD5);

                    /// 清理状态 重新进行下载
                    /// 下载的压缩包有问题
                    
                    /// 清理有问题的压缩包
                    File.Delete(Path.Combine(Application.persistentDataPath, patchAssetName));
                    string msgInfo = "由于网络问题导致更新的压缩包损坏，是否重新下载？";
#if APPLE_STORE
					if (null != mHotUpdateConfig && !string.IsNullOrEmpty(mHotUpdateConfig.networkErrorInfo)) 
					{
						msgInfo = mHotUpdateConfig.networkErrorInfo;
					}
#endif
                    GameClient.SystemNotifyManager.HotUpdateMsgBoxOkCancel(msgInfo, _OnRetryHotUpdate_OK, _OnRetryHotUpdate_Cancel);

                    yield break;
                }
            }

            Logger.LogAsset("[Download] End");

            /// 保存最新的版本号
            Logger.LogAsset("Re-save new version");
            //string verPath = Path.Combine(Application.persistentDataPath, VERSION_INFO_FILE_NAME);
            string verPath = Path.Combine(Application.persistentDataPath, VERSION_INFO_FILE_NAME);
            FileStream streamW = new FileStream(verPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            streamW.SetLength(0);
            Logger.LogAsset("Create file stream :" + verPath + "!");
            StreamWriter sw = new StreamWriter(streamW);
            Logger.LogAsset("Create stream write!");

            m_NativeVersion = m_RemoteVersion;
            m_VersionTbl[PLATFORM_STRING] = m_NativeVersion;
            Logger.LogAsset("Write version json!");
            sw.Write( MiniJSON.jsonEncode(m_VersionTbl) );

            Logger.LogAsset("Flush!");
            streamW.Flush();
            Logger.LogAsset("Close stream writer!");
            sw.Close();
            Logger.LogAsset("Close stream!");
            streamW.Close();

            VersionManager.instance.Init();

            ///热更新完毕
            string info = "祝您游戏愉快！";
            string state = "更新完毕";
            _DisplayUpdateState(state);
            _DisplayUpdateInfo(100, info);
            m_UpdateState = VersionUpdateState.FinishUpdate;
            VersionManager.instance.m_IsLastest = true;

#if UNITY_ANDROID
            _DisplayUpdateInfo(100, "正在重新启动游戏...");
            m_UpdateState = VersionUpdateState.WaitForRestart;

            yield return Yielders.GetWaitForSeconds(1.0f);
            RestartApp();
            //GameClient.SystemNotifyManager.BaseMsgBoxOK("更新完毕，请点击确定重启游戏！", RestartApp);
#endif

            Debug.Log("Hot fix completed!");

            yield return Yielders.EndOfFrame;
        }

        public void DoDeleteExpirePackage()
        {
            string delPackageListFile = Path.Combine(_GetHotfixAccessPath(), DELETE_PACKAGE_LIST_NAME);
            Debug.LogWarningFormat("Hot fix delete package list file[{0}]!", delPackageListFile);

            string delPackageListData = "";
            if(FileArchiveAccessor.LoadFileInPersistentFileArchive(DELETE_PACKAGE_LIST_NAME, out delPackageListData))
            {
                ArrayList UrlList = MiniJSON.jsonDecode(delPackageListData) as ArrayList;
                if (null != UrlList)
                {
                    for (int i = 0, icnt = UrlList.Count; i < icnt; ++i)
                    {
                        string delPackage = UrlList[i] as string;
                        if (!string.IsNullOrEmpty(delPackage))
                        {
                            string dstDelPath = Path.Combine(_GetHotfixAccessPath(), Path.Combine("AssetBundles", delPackage));
                            Debug.LogWarningFormat("Hot fix delete package file[{0}]!", dstDelPath);
                            if (File.Exists(dstDelPath))
                                File.Delete(dstDelPath);
                        }
                    }

                }

                if (File.Exists(delPackageListFile))
                {
                    Debug.LogWarningFormat("Hot fix delete package list file is exist[{0}]!", delPackageListFile);
                    File.Delete(delPackageListFile);
                }
                else
                    Debug.LogWarningFormat("Hot fix delete package list file is not exist[{0}]!", delPackageListFile);
            }
            else
            {
                Debug.LogWarningFormat("Hot fix can't open file [{0}]!", delPackageListFile);
            }
        }

        protected string _GetDiffListFileUrl(string packageFile)
        {
            ///if (m_LocalHost)
            ///    return "http://localhost/DNF/" + PLATFORM_STRING + "/" + m_LastestVersion + "/" + packageFile;
            ///else/// TODO: 读配置查询
            ///    return "";

            return _GetResourceServerRoot() + PLATFORM_STRING + "/zip/" + m_RemoteVersion + "/" + packageFile;
        }

        bool _ParseDiffList(string diffData, ref long totalSize, ref long downloadedSize)
        {
            string diffListData = diffData.Trim();

            string[] pAllFiles = diffListData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            m_DiffAssetDescArray = new DiffAssetDesc[pAllFiles.Length];
            totalSize = 0;
            downloadedSize = 0;

            for (int i = 0; i < pAllFiles.Length; i++)
            {
                string[] curAssetDesc = pAllFiles[i].Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                DiffAssetDesc curDiffAssetDesc = new DiffAssetDesc();

                try
                {
                    curDiffAssetDesc.assetUrl = curAssetDesc[0].Replace("zip:",null);
                    curDiffAssetDesc.assetMD5 = curAssetDesc[1].Replace("md5:", null);
                    curDiffAssetDesc.assetBytes = long.Parse(curAssetDesc[2].Replace(" bytes",null));

                    m_DiffAssetDescArray[i] = curDiffAssetDesc;
                    totalSize += curDiffAssetDesc.assetBytes;

                    string dstDownloadPath = Path.Combine(Application.persistentDataPath, Path.GetFileName(curDiffAssetDesc.assetUrl));
                    long lFileOffSet = FileUtil.GetFileBytes(dstDownloadPath);
                    if (lFileOffSet >= 0)
                        downloadedSize += lFileOffSet;
                }
                catch (System.Exception e)
                {
                    Logger.LogAssetFormat("Parse difference file info [{0}] has failed: {1}", pAllFiles[i], e.ToString());
                    return false;
                }
            }

            return true;
        }

        protected string _GetResourceServerRoot()
        {
            if (m_LocalHost)
                return "http://localhost/DNF/";
            else/// TODO: 读配置查询
            {
                if (null != m_ServerList)
                {
                    int serverIdx = UnityEngine.Random.Range(0, m_ServerList.Count);
                    return m_ServerList[serverIdx] as string;
                }
                else
                {
                    Logger.LogWarning("Load server list has failed!");
                    return "";
                }
            }
        }

        protected void _DisplayUpdateState(string updateState)
        {
            if(null != m_UI && m_UI.IsOpen())
                m_UI.UpdateProgressState(updateState);
        }

        protected void _DisplayUpdateInfo(int progress, string info )
        {
            if (null != m_UI && m_UI.IsOpen())
                m_UI.UpdateProgress(progress, info);
        }

        protected bool _LoadHackServerList()
        {
            string jsonData = null;
            if (FileArchiveAccessor.LoadFileInPersistentFileArchive(UPDATA_SERVER_HACK_FILE_NAME, out jsonData))
            {
                m_ServerList = MiniJSON.jsonDecode(jsonData) as ArrayList;
                if (null != m_ServerList)
                    return true;
            }

            return false;
        }

        protected void _LoadServerList()
        {
            //string path = Path.Combine(Application.streamingAssetsPath, UPDATA_SERVER_FILE_NAME);
            string path = Path.Combine(_GetLocalAccessUrlProtocol(), UPDATA_SERVER_FILE_NAME);
            byte[] fileData = null;

            //CFileManager.ReadFile(path);
            WWW www = new WWW(path);
            while (!www.isDone) { }
            if (null == www.error)
            {
                List<byte> byteList = new List<byte>();
                //List<byte> byteList = GamePool.ListPool<byte>.Get();
                byteList.AddRange(www.bytes);
                fileData = byteList.ToArray();

                //GamePool.ListPool<byte>.Release(byteList);
            }
            else
                Debug.LogWarning(www.error);

            www.Dispose();

            if (null != fileData)
                m_ServerList = _GetUpdateServerFromJson(System.Text.UTF8Encoding.Default.GetString(fileData));
            else
                Logger.LogAsset("Can not open server config file with path:" + path);
        }

        protected string _GetLocalAccessUrlProtocol()
        {
            switch(m_Platform)
            {
                case CustomPlatform.PC:
                    return "file:///" + Application.streamingAssetsPath;
                case CustomPlatform.iOS:
                    return "file://" + Application.streamingAssetsPath;
                case CustomPlatform.Android:
                    return "jar:file://" + Application.dataPath + "!/assets/";
                default:
                    return "file:///" + Application.streamingAssetsPath;
            }
        }
        protected string _GetHotfixAccessUrlProtocol()
        {
            switch (m_Platform)
            {
                case CustomPlatform.PC:
                    return "file:///" + Application.persistentDataPath;
                case CustomPlatform.iOS:
                    return "file://" + Application.persistentDataPath;
                case CustomPlatform.Android:
                    return "file://" + Application.persistentDataPath;
                default:
                    return "file:///" + Application.persistentDataPath;
            }
        }
        protected string _GetHotfixAccessPath()
        {
            return Application.persistentDataPath;
        }

        protected string _GetVersionFromJson(string jsonData,ref Hashtable versionTable)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    versionTable = MiniJSON.jsonDecode(jsonData) as Hashtable;
                    if (versionTable != null)
                    {
                        return versionTable[PLATFORM_STRING] as string;
                    }
                    else
                    {
                        Debug.Log("[_GetVersionFromJson] versionTable is null");
                    }
                }
                else
                {
                    Debug.Log("[_GetVersionFromJson] jsonData is null");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("Get version form json has failed! Exception:" + e.ToString());
                Logger.LogAssetFormat("Get version form json has failed! Exception:" + e.ToString());
                versionTable = null;
            }

            return DEFAULT_VERSION;
        }

        protected bool _GetUserAgreementFromJson(string jsonData)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    Hashtable versionTable = MiniJSON.jsonDecode(jsonData) as Hashtable;
                    if (versionTable != null)
                    {
                        string agreement = versionTable["agreement"] as string;
                        if (!string.IsNullOrEmpty(agreement))
                        {
                            if (agreement.Equals("true", StringComparison.OrdinalIgnoreCase))
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            Debug.Log("[_GetUserAgreementFromJson] agreement is null");
                        }
                    }
                    else
                    {
                        Debug.Log("[_GetUserAgreementFromJson] versionTable is null");
                    }
                }
                else
                {
                    Debug.Log("[_GetUserAgreementFromJson] jsonData is null");
                }
            }
            catch (System.Exception e)
            {
			    Debug.LogErrorFormat("Get version form json has failed! Exception:" + e.ToString());
                Logger.LogAssetFormat("Get version form json has failed! Exception:" + e.ToString());
            }

            return false;
        }

        protected SDKChannel[] _GetIOSOtherVIPAuth(string jsonData)
        {
            SDKChannel[] a = new SDKChannel[0];
            try
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    Hashtable versionTable = MiniJSON.jsonDecode(jsonData) as Hashtable;
                    if (versionTable != null && versionTable.ContainsKey("vipauth"))
                    {
                        string vipauth = versionTable["vipauth"] as string;
                        if (string.IsNullOrEmpty(vipauth))
                        {
                            return a;
                        }
                        var auths = vipauth.Split(',');
                        if (auths == null || auths.Length <= 0)
                        {
                            return a;
                        }
                        a = new SDKChannel[auths.Length];
                        for (int i = 0; i < auths.Length; i++)
                        {
                            try
                            {
                                a[i] = (SDKChannel)Enum.Parse(typeof(SDKChannel), auths[i]);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("[_GetIOSOtherVIPAuth] VIPAuth is invaild!!!"+e.ToString());
                                Logger.LogError("[_GetIOSOtherVIPAuth] VIPAuth is invaild!!!"+e.ToString());
                                return new SDKChannel[0];
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("[_GetIOSOtherVIPAuth] jsonData is null");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("Get version form json has failed! Exception:" + e.ToString());
                Logger.LogAssetFormat("Get version form json has failed! Exception:" + e.ToString());
            }
            return a;
        }

        protected SDKChannel[] _GetUseRealNamePopWindowsFromJson(string jsonData)
        {
            SDKChannel[] a = new SDKChannel[0];
            try
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    Hashtable versionTable = MiniJSON.jsonDecode(jsonData) as Hashtable;
                    if (versionTable != null && versionTable.ContainsKey("realnameauth"))
                    {
                        string realnameauth = versionTable["realnameauth"] as string;
                        if (string.IsNullOrEmpty(realnameauth))
                        {
                            return a;
                        }
                        var auths = realnameauth.Split(',');
                        if (auths == null || auths.Length <= 0)
                        {
                            return a;
                        }
                        a = new SDKChannel[auths.Length];
                        for (int i = 0; i < auths.Length; i++)
                        {
                            try
                            {
                                a[i] = (SDKChannel)Enum.Parse(typeof(SDKChannel), auths[i]);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("[_GetUseRealNamePopWindowsFromJson] realnameauth is invaild!!!" + e.ToString());
                                return new SDKChannel[0];
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("[_GetUseRealNamePopWindowsFromJson] jsonData is null");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Get version form json has failed! Exception" + e.ToString());
            }
            return a;
        }
        protected SDKChannel[] _GetUseSDKPuchFromJson(string jsonData)
        {
            SDKChannel[] a = new SDKChannel[0];
            try
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    Hashtable versionTable = MiniJSON.jsonDecode(jsonData) as Hashtable;
                    if (versionTable != null && versionTable.ContainsKey("sdkpush"))
                    {
                        string realnameauth = versionTable["sdkpush"] as string;
                        if (string.IsNullOrEmpty(realnameauth))
                        {
                            return a;
                        }
                        var auths = realnameauth.Split(',');
                        if (auths == null || auths.Length <= 0)
                        {
                            return a;
                        }
                        a = new SDKChannel[auths.Length];
                        for (int i = 0; i < auths.Length; i++)
                        {
                            try
                            {
                                a[i] = (SDKChannel)Enum.Parse(typeof(SDKChannel), auths[i]);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("[_GetUseSDKPuchFromJson] sdkpush is invaild!!!" + e.ToString());
                                return new SDKChannel[0];
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("[_GetUseSDKPuchFromJson] jsonData is null");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Get version form json has failed! Exception" + e.ToString());
            }
            return a;
        }
        protected ArrayList _GetUpdateServerFromJson(string jsonData)
        {
            ArrayList serverList = new ArrayList();
            try
            {
                Hashtable table = MiniJSON.jsonDecode(jsonData) as Hashtable;

                if (table != null)
                {
                    if (Global.Settings.hotFixUrlDebug)
                    {
                        serverList = table["debug"] as ArrayList;
                    }
                    else
                    {
                        serverList = table["release"] as ArrayList;
                    }
                }
                else
                {
                    Debug.Log("[_GetUpdateServerFromJson] table is null");
                }

                return serverList;
            }
            catch (System.Exception e)
            {
                Logger.LogAssetFormat("Get update server list form json has failed! Exception:" + e.ToString());
                serverList.Clear();
                return serverList;
            }
        }

        private bool _IsWifiUsed()
        {
            return (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
        }

        private bool _IsFileNotFound(string error)
        {
            return error.Contains("404");
        }
        
        protected bool _SendErrorEmail(string title,string content)
        {
            return MailSender.SetEmail(title, content);
        }

        protected void _ClearHotUpdateCaches()
        {
            //string platformUrlHead = _GetPlatformUrlHead();
            Debug.LogWarning("Clean hot-fix bundles!");
            string dstDictory = Path.Combine(_GetHotfixAccessPath(), "AssetBundles");
            if (Directory.Exists(dstDictory))
                Directory.Delete(dstDictory, true);

            string verFile1 = Path.Combine(_GetHotfixAccessPath(), "version.config");
            if (File.Exists(verFile1))
                File.Delete(verFile1);

            string verFile2 = Path.Combine(_GetHotfixAccessPath(), VERSION_INFO_FILE_NAME);
            if (File.Exists(verFile2))
                File.Delete(verFile2);

#if UNITY_ANDROID
            string verFile3 = Path.Combine(_GetHotfixAccessPath(), "Assembly-CSharp.dll");
            if (File.Exists(verFile3))
                File.Delete(verFile3);
            string verFile4 = Path.Combine(_GetHotfixAccessPath(), "Assembly-CSharp-firstpass.dll");
            if (File.Exists(verFile4))
                File.Delete(verFile4);
#endif
        }

        protected void _ProcessQuitProcedural()
        {
            Application.Quit();
        }

        //////////////////////////////////////////////////////////////////////////
        /// 下载分包逻辑
        public enum PackageDownloadState
        {
            None,
            CheckLocalVerion,
            CheckNetwork,
            GetPackageList,
            DownloadPackage,
            VerifyPackage,
            UnzipPackage,
            Finish,
        }

        protected class PackageDesc
        {
            public string assetUrl;
            public string assetMD5;
            public long assetBytes;
        }

        protected PackageDesc[] m_PackageDescArray = null;
        protected PackageDownloadState m_DownloadState = PackageDownloadState.None;
        protected string m_LocalPatchVersion = "";
        protected string m_PackageListFile = "packagelist.json";

        public IEnumerator ProcessPackageDownload()
        {
            yield return _GetLocalPackageVersion();

            /// 判断是否需要获取补包
            string packageListFile = Path.Combine(_GetHotfixAccessUrlProtocol(), m_PackageListFile);
            WWW packageListFileWWW = new WWW(packageListFile);
            yield return packageListFileWWW;
            
            Debug.LogWarningFormat("Local version:{0}", m_LocalPatchVersion);

            bool needFetchPackage = true;
            if (null == packageListFileWWW.error)
            {
                ArrayList UrlList = MiniJSON.jsonDecode(packageListFileWWW.text) as ArrayList;
                if(null != UrlList)
                {
                    if (UrlList.Count > 0)
                    {
                        string verCode = UrlList[0] as string;
                        int persistentVer = 0;
                        int.TryParse(verCode, out persistentVer);
                        int localVer = 1;
                        int.TryParse(m_LocalPatchVersion,out localVer);

                        if(persistentVer >= localVer)
                            needFetchPackage = false;
                    }
                }
            }
            packageListFileWWW.Dispose();

            if(needFetchPackage)
                yield return _FetchAssetPackage();
        }

        IEnumerator _GetLocalPackageVersion()
        {
            string localStreamVerFile = Path.Combine(_GetLocalAccessUrlProtocol(), VERSION_INFO_FILE_NAME);
            Logger.Log("Load local version from asset stream...");
            WWW localStreamVerFileWWW = new WWW(localStreamVerFile);
            yield return localStreamVerFileWWW;

            string verLocalJson = null;
            Hashtable verTblLocal = null;
            if (null == localStreamVerFileWWW.error)
            {
                verLocalJson = _GetVersionFromJson(localStreamVerFileWWW.text, ref verTblLocal);
                Debug.LogWarning("### Native local version:" + verLocalJson);
            }
            else
            {
                /// 这绝壁是严重错误
                Logger.LogAsset("Oh crap! Load local version from persistent has failed!");
            }
            localStreamVerFileWWW.Dispose();

            if (!string.IsNullOrEmpty(verLocalJson))
            {
                string[] nativeVerList = null;
                if (!string.IsNullOrEmpty(verLocalJson))
                {
                    nativeVerList = verLocalJson.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if ((int)VersionType.VersionTypeNum == nativeVerList.Length)
                    {
                        m_LocalPatchVersion = nativeVerList[(int)VersionType.CustomPatch];
                        yield break;
                    }
                }
            }

            m_LocalPatchVersion = null;
        }

        protected string _GetPackageListFileUrl(string packageFile)
        {
            return _GetResourceServerRoot() + "package/" + PLATFORM_STRING + "/" + packageFile;
        }

        bool _ParsePackageList(string diffData, ref long totalSize, ref long downloadedSize)
        {
            string diffListData = diffData.Trim();

            string[] pAllFiles = diffListData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            m_PackageDescArray = new PackageDesc[pAllFiles.Length];
            totalSize = 0;
            downloadedSize = 0;

            for (int i = 0; i < pAllFiles.Length; i++)
            {
                string[] curAssetDesc = pAllFiles[i].Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                PackageDesc curPackageDesc = new PackageDesc();

                try
                {
                    curPackageDesc.assetUrl = curAssetDesc[0].Replace("zip:", null);
                    curPackageDesc.assetMD5 = curAssetDesc[1].Replace("md5:", null);
                    curPackageDesc.assetBytes = long.Parse(curAssetDesc[2].Replace(" bytes", null));

                    m_PackageDescArray[i] = curPackageDesc;
                    totalSize += curPackageDesc.assetBytes;

                    string dstDownloadPath = Path.Combine(Application.persistentDataPath, Path.GetFileName(curPackageDesc.assetUrl));
                    long lFileOffSet = FileUtil.GetFileBytes(dstDownloadPath);
                    if (lFileOffSet >= 0)
                        downloadedSize += lFileOffSet;
                }
                catch (System.Exception e)
                {
                    Logger.LogAssetFormat("Parse package file info [{0}] has failed:{1}", pAllFiles[i],e.ToString());
                    return false;
                }
            }

            return true;
        }

        IEnumerator _FetchAssetPackage()
        {
            Logger.Log("Begin update path!");

            string packageFile = "package-" + m_LocalPatchVersion + ".txt";

            Debug.Log("Target patch:" + packageFile);

            m_DownloadState = PackageDownloadState.GetPackageList;

            string diffListData = null;
            string packageListFileUrl = null;
            string lastError = "";
            WWW packageListWWW = null;
            int retryCnt = 0;
            bool isConnectOK = false;
            do
            {
                /// 首先获取DiffFile
                packageListFileUrl = _GetPackageListFileUrl(packageFile).Replace('\\', '/');
                packageListWWW = new WWW(packageListFileUrl);
                yield return packageListWWW;

                if (null == packageListWWW.error && !string.IsNullOrEmpty(packageListWWW.text))
                {
                    diffListData = packageListWWW.text;
                    isConnectOK = true;
                }
                else
                {
                    lastError = packageListWWW.error;
                    Logger.LogAssetFormat("Download package list file from server has failed (url:{0})!", packageListFileUrl);
                }
                packageListWWW.Dispose();

                if (isConnectOK)
                    break;
                else
                    ++retryCnt;

            } while (retryCnt < 3);

            if (isConnectOK && !string.IsNullOrEmpty(diffListData))
            {
                if (_ParsePackageList(diffListData, ref m_TotalSize, ref m_DownloadedSize))
                {
                    if (!_IsWifiUsed())
                    {/// 非Wifi环境下 提示是否下载更新包
                        //float totalSizeMB = m_TotalSize / (1024.0f * 1024.0f);
                        //float downloadSizeMB = m_DownloadedSize / (1024.0f * 1024.0f);
                        //
                        //string downloadWithoutWifi = string.Format("检测到当前设备未连接到无线局域网（WiFi），本次更新共{0}MB，已下载{1}MB，是否确认更新？", totalSizeMB.ToString("G3"), downloadSizeMB.ToString("G3"));
                        //GameClient.SystemNotifyManager.HotUpdateMsgBoxOkCancel(downloadWithoutWifi, _OnConfirmHotUpdate_OK, _OnConfirmHotUpdate_Cancel);

                        yield break;
                    }
                    else
                    {/// wifi 环境下执行下载
                        yield return _DownloadPacakges();
                    }
                }
            }
            else
            {/// 获取服务器配置文件失败
                
            }
        }

        IEnumerator _DownloadPacakges()
        {
            Logger.Log("Begin downlad patches ...");
            ServicePointManager.DefaultConnectionLimit = 30;

            byte[] downloadCache = new byte[DOWNLOAD_CHUNK_SIZE];

            string assetFullUrl = "";
            string patchAssetName = "";

            m_DownloadState = PackageDownloadState.DownloadPackage;
            int nReadSize = 0;
            for (int i = 0; i < m_PackageDescArray.Length; i++)
            {
                PackageDesc assetDesc = m_PackageDescArray[i];

                assetFullUrl = _GetPackageListFileUrl(assetDesc.assetUrl);// Path.Combine(_GetResourceServerRoot(), assetDesc.assetUrl);
                patchAssetName = Path.GetFileName(assetFullUrl);          // package_2.5.24.28_2.5.24.29.zip

                Logger.Log("[Download] patch asset url: " + assetDesc.assetUrl);// resource/common/package-2.5.24.28.zip
                Logger.Log("[Download] full asset url : " + assetFullUrl);
                Logger.Log("[Download] asset MD5 : " + assetDesc.assetMD5);     // 24652a56791d9d96c03a1e444df5ceec 
                Logger.Log("[Download] asset size : " + assetDesc.assetBytes);  // 10240000

                System.GC.Collect();

                // TODO if sDiffUrl is wrong or the resource is not valid
                long responseCountLength = 0;
                HttpWebRequest requestHttp = null;
                WebResponse webResponse = null;

                try
                {
                    requestHttp = HttpWebRequest.Create(assetFullUrl) as HttpWebRequest;
                    //pRequestHttp.Timeout = 10;

                    webResponse = requestHttp.GetResponse();

                    responseCountLength = webResponse.ContentLength;
                    requestHttp.KeepAlive = false;

                    webResponse.Close();
                    webResponse = null;

                    requestHttp.Abort();
                    requestHttp = null;
                }
                catch (Exception e)
                {/// 网络连接遇到问题，是否重试
                    //BaseMsgBox.m_instance.Initialize(BROKEN_NET_READ_TIPS, ReDownloadNewAssetbundleByPoint);

                    Logger.LogError("Get http download request count length has failed! Exception:" + e.ToString());
                    yield break;
                }

                string assetDownloadPath = Path.Combine(Application.persistentDataPath, patchAssetName);
                long fileOffset = FileUtil.GetFileBytes(assetDownloadPath);

                Logger.Log("[Download] patch asset : " + patchAssetName + " save to " + assetDownloadPath);

                if (fileOffset < responseCountLength)
                {
                    FileStream fs;
                    if (-1 != fileOffset)
                    {/// 文件存在 操作文件指针到上次未写完的位置
                        fs = File.OpenWrite(assetDownloadPath);
                        fs.Seek(fileOffset, SeekOrigin.Current);
                    }
                    else
                    {
                        Logger.LogAsset("[Download] patch asset's url is not valid : " + assetFullUrl);
                        fs = new FileStream(assetDownloadPath, FileMode.Create);
                    }

                    requestHttp = null;
                    try
                    {
                        requestHttp = HttpWebRequest.Create(assetFullUrl) as HttpWebRequest;
                    }
                    catch (System.Exception e)
                    {/// "网络连接遇到问题，重试？"
                     /// 
                     /// BaseMsgBox.m_instance.Initialize(BROKEN_NET_READ_TIPS, ReDownloadNewAssetbundleByPoint);

                        StartCoroutine(_FetchAssetPackage());
                        Logger.LogAsset("[Download] Create http request for download offset has failed! Exception: " + e.ToString());
                        yield break;
                    }

                    if (fileOffset > 0)
                        requestHttp.AddRange((int)fileOffset);

                    webResponse = requestHttp.GetResponse();

                    Stream ns = webResponse.GetResponseStream();
                    ns.ReadTimeout = REQUEST_TIME_LIMIT;

                    while (true)
                    {
                        try
                        {
                            nReadSize = ns.Read(downloadCache, 0, DOWNLOAD_CHUNK_SIZE);

                            if (nReadSize <= 0)
                                break;

                            fs.Write(downloadCache, 0, nReadSize);

                            int curProgress = (int)(((float)(fs.Length * 100) / m_TotalSize) * 0.69);

                            float MBDiv = 1 / (1024.0f * 1024.0f);
                            string sizeInfo = string.Format("  已下载{0}MB（共{1}MB）", (fs.Length * MBDiv).ToString("G3"), (m_TotalSize * MBDiv).ToString("G3"));
                            Logger.LogError(sizeInfo);
                        }
                        catch (System.Exception e)
                        {
                            Logger.LogAsset("[Download] Read http request data stream has failed! Exception: " + e.ToString());
                            yield break;
                        }

                        yield return nReadSize;
                    }

                    ns.Close();
                    fs.Close();

                    webResponse.Close();
                    webResponse = null;

                    requestHttp.Abort();
                    requestHttp = null;
                }
                else
                {
                    Logger.Log("[Download] Already Download the full patch");
                }

                m_DownloadState = PackageDownloadState.VerifyPackage;
                //string downloadedAssetMD5 = FileUtil.GetFileMD5(assetDownloadPath);

                yield return FileUtil.GetFileMD5Async(assetDownloadPath);
                string downloadedAssetMD5 = FileUtil.md5;

                if (downloadedAssetMD5.ToLower().Equals(assetDesc.assetMD5.ToLower()))
                {
                    m_DownloadState = PackageDownloadState.UnzipPackage;
                    // uncompress all 
                    yield return LibZip.LibZipFileReader.UncompressAllAsync(Path.Combine(Application.persistentDataPath, patchAssetName));
                    Logger.LogAsset("Uncompress complete!");
                }
                else
                {
                    //BaseMsgBox.m_instance.Initialize(BROKEN_NET_TIPS, ReDownloadNewAssetbundleByPoint);
                    // TODO break the loop
                    Logger.Log("[Download] Patch asset MD5 miss match!");
                    Logger.Log("    Downloaded asset MD5: " + downloadedAssetMD5);
                    Logger.Log("    Expected asset MD5: " + assetDesc.assetMD5);

                    /// 清理状态 重新进行下载
                    /// 下载的压缩包有问题
                    Debug.LogWarning("MD5效验失败，包体下载失败！");

                    yield break;
                }
            }

            Logger.LogAsset("[Download] End");

            yield return Yielders.EndOfFrame;
            File.Delete(Path.Combine(Application.persistentDataPath, patchAssetName));

            ///热更新完毕
            m_DownloadState = PackageDownloadState.Finish;
            Debug.Log("Asset package download completed!");

            yield return Yielders.EndOfFrame;
        }
    }
}
