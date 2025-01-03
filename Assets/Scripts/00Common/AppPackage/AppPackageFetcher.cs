

using GameClient;
using System;
using System.Collections;
using UnityEngine;
using XUPorterJSON;

namespace Client
{
    public class AppPackageFetchResult
    {
        public string version;
        public string result;
        public string url;
    }

    public static class AppPackageFetcher
    {
        static private readonly string DEFAULT_VERSION = "1.0.1.0";

        static private string m_FullAppPackageURL = "";
        static private bool m_IsFetchURLFinished = false;

        static private string m_RemoteFullPackageVersion = DEFAULT_VERSION;
        static private string m_NativeFullPackageVersion = DEFAULT_VERSION;

        static private int m_RemoteClientMajor = ~0;
        static private int m_RemoteClientPatch = ~0;
        static private int m_NativeClientMajor = ~0;
        static private int m_NativeClientPatch = ~0;

        static private bool m_SkipFetchAgain = false;

        public static IEnumerator FetchFullAppPackage()
        {
            m_RemoteClientMajor = ~0;
            m_RemoteClientPatch = ~0;

            m_IsFetchURLFinished = false;
            /// 读配置文件
            string fetchURL = PluginManager.instance.FullPackageFetchURL(ClientConfig.AppPackageFetchServer, Global.Settings.sdkChannel.ToString(), VersionManager.instance.Version());
            Debug.LogFormat("### Application package SDK channel name:[{0}] fetch URL:'{1}'.", Global.SDKChannelName[(int)(Global.Settings.sdkChannel)], fetchURL);

            BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
            wt.reconnectCnt = 3;
            wt.timeout = 2000;
            wt.url = fetchURL;
            yield return wt;

            Debug.LogFormat("### Application package URL [{0}] fetch request has send, waiting response...", fetchURL);
            if (BaseWaitHttpRequest.eState.Success == wt.GetResult())
            {
                AppPackageFetchResult res = wt.GetResultJson<AppPackageFetchResult>();
                if (null != res)
                {
                    if (res.result.Equals("success", StringComparison.OrdinalIgnoreCase))
                    {
                        m_FullAppPackageURL = res.url;
                        m_RemoteFullPackageVersion = res.version;

                        Debug.LogFormat("### Application package remote version string:{0}", m_RemoteFullPackageVersion);
                        Debug.LogFormat("### Application package download url:{0}", m_FullAppPackageURL);

                        _GetVersionFromVersionString(m_RemoteFullPackageVersion, ref m_RemoteClientMajor, ref m_RemoteClientPatch);
                    }
                    else/// url拉取失败
                        Debug.LogWarningFormat("Fetch application package URL has trouble! return with:{0}.", res.result);
                }
                else
                    Debug.LogWarningFormat("BaseWaitHttpRequest GetResultJson parse failed! [Fetch URL:{0}]", fetchURL);
            }
            else
                Debug.LogWarningFormat("Fetch application package URL has failed!");

            m_IsFetchURLFinished = true;
        }

        public static bool InitNativePackageVersion()
        {
            m_NativeClientMajor = ~0;
            m_NativeClientPatch = ~0;   
            /// 读取包内的版本号
            string jsonData;
            if (FileArchiveAccessor.LoadFileInLocalFileArchive("version.json", out jsonData))
            {
                m_NativeFullPackageVersion = _GetVersionFromJson(jsonData);
                Debug.LogFormat("### Application package native version string:{0}", m_NativeFullPackageVersion);
                return _GetVersionFromVersionString(m_NativeFullPackageVersion, ref m_NativeClientMajor,ref m_NativeClientPatch);                    
            }

            return false;
        }

        public static bool IsVersionValid()
        {
            return ~0 != m_NativeClientMajor && ~0 != m_NativeClientPatch && ~0 != m_RemoteClientMajor && ~0 != m_RemoteClientPatch;
        }

        public static bool IsRemoteNewer()
        {
            if (m_RemoteClientMajor > m_NativeClientMajor)
                return true;
            else if (m_RemoteClientMajor == m_NativeClientMajor)
            {
                if (m_RemoteClientPatch > m_NativeClientPatch)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static void SkipFetchAgain(bool skip)
        {
            m_SkipFetchAgain = skip;
        }

        public static void OpenAppPackageURL()
        {
            if(!m_IsFetchURLFinished)
            {
                Debug.LogWarning("### Application package URL fetch is not finish yet, You should begin fetch first.");
                return;
            }

            if(string.IsNullOrEmpty(m_FullAppPackageURL))
            {
                Debug.LogWarning("### Application package URL is empty string.");
                return;
            }

            Debug.LogFormat("### Application package URL fetch success, open application target url:'{0}'.", m_FullAppPackageURL);
            Application.OpenURL(m_FullAppPackageURL);
        }

        public static bool NeedFetchAppPackage()
        {
            if (ClientConfig.AppPackageFetchEnable)
            {/// 如果打开拉取完整包配置
                int curAPILevel = (int)OSInfo.GetAndroidOSAPILevel();
                if (ClientConfig.AppMinAndroidLevel <= curAPILevel && curAPILevel <= ClientConfig.AppMaxAndroidLevel)
                {/// 如果是规定范围内的API-Level
                    return !m_SkipFetchAgain;
                }
            }

            return false;
        }

        private static bool _GetVersionFromVersionString(string versionString,ref int majorClient,ref int patchClient)
        {
            majorClient = ~0;
            patchClient = ~0;
            string[] verList = versionString.Split('.');

            if (4 == verList.Length)
            {
                if(int.TryParse(verList[2],out majorClient))
                {
                    if (int.TryParse(verList[3], out patchClient))
                        return true;
                    else
                        Debug.LogError("Client application package fetch parse patch version has failed!");
                }
                else
                    Debug.LogError("Client application package fetch parse major version has failed!");
            }
            else
                Debug.LogWarning("Local version string is invalid(while split version string to sub versions)!");

            return false;
        }

        private static string _GetVersionFromJson(string jsonData)
        {
            try
            {
                Hashtable versionTable = MiniJSON.jsonDecode(jsonData) as Hashtable;
                return versionTable[PlatformString] as string;
            }
            catch (System.Exception e)
            {
                Logger.LogAssetFormat("Get version form json has failed! Exception:" + e.ToString());
            }

            return "1.0.1.0";
        }

        public static string PlatformString
        {
            get
            {
#if UNITY_ANDROID
                 return "android";
#elif UNITY_IOS
                 return "ios";
#else
                return "pc";
#endif
            }
        }


        private static IEnumerator _OpenAppPackageUrl()
        {
            yield return Client.AppPackageFetcher.FetchFullAppPackage();
            Client.AppPackageFetcher.OpenAppPackageURL();
        }

    }
}

