using System.Collections;
using System.Collections.Generic;
using System.IO;

using XUPorterJSON;
using UnityEngine;

public class RemoteConfig : MonoSingleton<RemoteConfig>
{
    protected readonly string REMOTE_CONFIG_FILE_NAME = "remote_config.json";
    protected readonly string UPDATA_SERVER_FILE_NAME = "updateserver.json";
    protected readonly string UPDATA_SERVER_HACK_FILE_NAME = "updateserver-hack.json";
    protected ArrayList m_ServerList = null;
    protected bool m_ConfigLoaded = false;

    List<string> m_BuglyBlackRoleList = new List<string>();
    List<string> m_BuglyBlackAndroidList = new List<string>();

    public IEnumerator FetchRemoteConfig()
    {
        m_ConfigLoaded = false;
        if (null == m_ServerList)
            yield return _LoadServerList();
        
        if(null != m_ServerList)
        {
            int serverIdx = UnityEngine.Random.Range(0, m_ServerList.Count);
            string serverRoot = m_ServerList[serverIdx] as string;

            if (!string.IsNullOrEmpty(serverRoot))
            {
                int retryCnt = 0;
                WWW serverWWW = null;
                do
                {
                    string configPath = Path.Combine(serverRoot, REMOTE_CONFIG_FILE_NAME);
                    configPath = configPath.Replace("\\", "/");

                    if (configPath.Contains("?"))
                        configPath = string.Format("{0}&tsp={1}", configPath, Utility.GetCurrentTimeUnix() / 300);
                    else
                        configPath = string.Format("{0}?tsp={1}", configPath, Utility.GetCurrentTimeUnix() / 300);

                    Debug.Log("URL:[" + configPath + "]!");
                    ///serverWWW = new WWW(configPath, form);
                    ///
                    float time = Time.realtimeSinceStartup;
                    serverWWW = new WWW(configPath);
                    yield return serverWWW;

                    time = Time.realtimeSinceStartup - time;
                    if (time * 1000 > 1.0f)
                        Debug.LogFormat("### RemoteConfig fetchTime:{0}",time);

                    if (null == serverWWW.error)
                    {
                        _ParseRemoveConfig(System.Text.UTF8Encoding.Default.GetString(serverWWW.bytes));
                        m_ConfigLoaded = true;
                        yield break;
                    }
                    else
                    {
                        if (_IsFileNotFound(serverWWW.error))
                        {
                            yield break;
                        }

                        Logger.LogErrorFormat("连接资源服务器失败[{0}]!", serverWWW.error);
                        ++retryCnt;
                    }
                } while (retryCnt < 3);
            }
        }
    }

    public bool IsRoleNameInBlackList(string roleName)
    {
        if (null != m_BuglyBlackRoleList)
            return m_BuglyBlackRoleList.Contains(roleName);
        return false;
    }

    public bool IsAndroidInBlackList(string androidAPI)
    {
        Debug.LogFormat("### OS API Level:{0}",androidAPI);
        if (null != m_BuglyBlackAndroidList)
            return m_BuglyBlackAndroidList.Contains(androidAPI);
        return false;
    }

    public string GetCurrentOSAndroidAPILevel()
    {
        string op = SystemInfo.operatingSystem;
        Debug.LogFormat("### Current OS:{0}", op);
        if (op.Contains("Android OS", System.StringComparison.OrdinalIgnoreCase))
        {
            int level = op.IndexOf("API-");
            if(0 <= level && level < op.Length)
            {
                int cnt = 0;
                while (cnt + 4 + level < op.Length)
                {
                    char curChar = op[level + 4 + cnt];
                    if ('9' < curChar || curChar < '0')
                        break;

                    ++cnt;
                }

                return op.Substring(level + 4, cnt);
            }
        }

        return "";
    }

    private bool _IsFileNotFound(string error)
    {
        return error.Contains("404");
    }

    protected void _ParseRemoveConfig(string jsonData)
    {
        Hashtable remoteConfig = MiniJSON.jsonDecode(jsonData) as Hashtable;
        if(null != remoteConfig)
        {
            Hashtable bugly = remoteConfig["Bugly"] as Hashtable;
            if(null != bugly)
            {
                ArrayList blackNameList = bugly["RoleName"] as ArrayList;
                if(null != blackNameList)
                {
                    m_BuglyBlackRoleList.Clear();
                    m_BuglyBlackRoleList.Capacity = blackNameList.Count;
                    for (int i = 0, icnt = blackNameList.Count; i < icnt; ++i)
                        m_BuglyBlackRoleList.Add(blackNameList[i] as string);
                }

                ArrayList blackAndroidList = bugly["AndroidAPI"] as ArrayList;
                if(null != blackAndroidList)
                {
                    m_BuglyBlackAndroidList.Clear();
                    m_BuglyBlackAndroidList.Capacity = blackAndroidList.Count;
                    for (int i = 0, icnt = blackAndroidList.Count; i < icnt; ++i)
                        m_BuglyBlackAndroidList.Add(blackAndroidList[i] as string);
                }
            }
        }
    }

    protected IEnumerator _LoadServerList()
    {
        byte[] fileData = null;
        string path = Path.Combine(_GetHotfixAccessUrlProtocol(), UPDATA_SERVER_HACK_FILE_NAME);
        WWW www = new WWW(path);
        yield return www;

        if (null == www.error)
            fileData = www.bytes;

        if(null == fileData)
        {
            if (null != www)
                www.Dispose();

            path = Path.Combine(_GetLocalAccessUrlProtocol(), UPDATA_SERVER_FILE_NAME);
            www = new WWW(path);
            yield return www;

            if (null == www.error)
                fileData = www.bytes;
            else
                Debug.LogWarning(www.error);

            if (null != fileData)
                m_ServerList = _GetUpdateServerFromJson(System.Text.UTF8Encoding.Default.GetString(fileData));
            else
                Logger.LogErrorFormat("Can not open server config file with path:{0}",path);
        }

        if (null != www)
            www.Dispose();
    }

    protected ArrayList _GetUpdateServerFromJson(string jsonData)
    {
        ArrayList serverList = new ArrayList();
        try
        {
            Hashtable table = MiniJSON.jsonDecode(jsonData) as Hashtable;

            if (Global.Settings.hotFixUrlDebug)
            {
                serverList = table["debug"] as ArrayList;
            }
            else
            {
                serverList = table["release"] as ArrayList;
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

    protected string _GetLocalAccessUrlProtocol()
    {
#if UNITY_IOS
        return "file://" + Application.streamingAssetsPath;
#elif UNITY_ANDROID
        return "jar:file://" + Application.dataPath + "!/assets/";
#else
        return "file:///" + Application.streamingAssetsPath;
#endif
    }

    protected string _GetHotfixAccessUrlProtocol()
    {
#if UNITY_IOS
        return "file://" + Application.persistentDataPath;
#elif UNITY_ANDROID
        return "file://" + Application.persistentDataPath;
#else
        return "file:///" + Application.persistentDataPath;
#endif
    }
}
