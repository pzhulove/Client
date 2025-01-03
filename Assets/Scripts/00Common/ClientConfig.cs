
using UnityEngine;

public class ClientConfig : Singleton<ClientConfig>
{
    public class ClientConfigData
    {
        public ClientConfigData()
        {
            appPackageFetchEnable = true;
            appMinAndroidLevel = 28;
            appMaxAndroidLevel = 28;
            appPackageFetchServer = "download.aldzn.com:59000";
            appPackageMessageLoginFailed = "当前游戏版本号过低 点击〖重新登录〗进行游戏更新，如果仍不能正常游戏， 请点击〖<color=#ff0000ff>下载客户端</color>〗下载游戏最新版本。";
            appPackageMessageAppVersionLow = "当前游戏版本号过低 点击〖尝试更新〗尝试游戏更新，如果仍不能正常游戏， 请点击〖<color=#ff0000ff>下载客户端</color>〗下载游戏最新版本。";

            appPackageButTextRetryUpdate = "尝试更新";
            appPackageButTextRetryLogin = "重新登录";
            appPackageButTextOpenURL = "下载客户端";
        }

        public bool appPackageFetchEnable;
        public int appMinAndroidLevel;
        public int appMaxAndroidLevel;
        public string appPackageFetchServer;

        public string appPackageMessageLoginFailed;
        public string appPackageMessageAppVersionLow;

        public string appPackageButTextRetryLogin;
        public string appPackageButTextRetryUpdate;
        public string appPackageButTextOpenURL;
    }

    private ClientConfigData m_ConfigData = new ClientConfigData();
    private const string kConfigFileName = "clientconfig.json";

    static public bool AppPackageFetchEnable
    {
        get { return instance.m_ConfigData.appPackageFetchEnable; }
    }

    static public int AppMinAndroidLevel
    {
        get { return instance.m_ConfigData.appMinAndroidLevel; }
    }

    static public int AppMaxAndroidLevel
    {
        get { return instance.m_ConfigData.appMaxAndroidLevel; }
    }

    static public string AppPackageFetchServer
    {
        get { return instance.m_ConfigData.appPackageFetchServer; }
    }

    static public string AppPackageMessageLoginFailed
    {
        get { return instance.m_ConfigData.appPackageMessageLoginFailed; }
    }

    static public string AppPackageMessageAppVersionLow
    {
        get { return instance.m_ConfigData.appPackageMessageAppVersionLow; }
    }

    static public string AppPackageButTextRetryLogin
    {
        get { return instance.m_ConfigData.appPackageButTextRetryLogin; }
    }

    static public string AppPackageButTextRetryUpdate
    {
        get { return instance.m_ConfigData.appPackageButTextRetryUpdate; }
    }

    static public string AppPackageButTextOpenURL
    {
        get { return instance.m_ConfigData.appPackageButTextOpenURL; }
    }

    public sealed override void Init()
    {
        LoadConfig();
    }

    public void LoadConfig()
    {
        try
        {
            byte[] data = _loadData();
            if (null != data)
            {
                string content = System.Text.ASCIIEncoding.Default.GetString(data);
                m_ConfigData = LitJson.JsonMapper.ToObject<ClientConfigData>(content);
            }
            else
            {
                UnityEngine.Debug.LogFormat("Client config load failed!");
            }
        }
        catch(System.Exception e)
        {
            UnityEngine.Debug.LogFormat("Client config load failed with exception:{0}!", e.Message);
        }

        Debug.LogFormat("Client config:[appPackageFetchEnable:'{0}'] ", m_ConfigData.appPackageFetchEnable);
        Debug.LogFormat("Client config:[appMinAndroidLevel:'{0}'] ", m_ConfigData.appMinAndroidLevel);
        Debug.LogFormat("Client config:[appMaxAndroidLevel:'{0}'] ", m_ConfigData.appMaxAndroidLevel);

        Debug.LogFormat("Client config:[appPackageFetchServer:'{0}'] ", m_ConfigData.appPackageFetchServer);

        Debug.LogFormat("Client config:[appPackageMessageLoginFailed:'{0}'] ", m_ConfigData.appPackageMessageLoginFailed);
        Debug.LogFormat("Client config:[appPackageMessageAppVersionLow:'{0}'] ", m_ConfigData.appPackageMessageAppVersionLow);

        Debug.LogFormat("Client config:[appPackageButTextRetryLogin:'{0}'] ", m_ConfigData.appPackageButTextRetryLogin);
        Debug.LogFormat("Client config:[appPackageButTextRetryUpdate:'{0}'] ", m_ConfigData.appPackageButTextRetryUpdate);
        Debug.LogFormat("Client config:[appPackageButTextOpenURL:'{0}'] ", m_ConfigData.appPackageButTextOpenURL);
    }

    private byte[] _loadData()
    {
        byte[] data = null;

        try
        {            
            if(!FileArchiveAccessor.LoadFileInPersistentFileArchive(kConfigFileName, out data))
            {
                Debug.LogError("Load client config file from persistent path has failed!");
                if (!FileArchiveAccessor.LoadFileInLocalFileArchive(kConfigFileName, out data))
                    Debug.LogError("Load client config file from streaming path has failed!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogErrorFormat("Load client config file exception:{0}!", e.Message);
        }

        return data;
    }
}
