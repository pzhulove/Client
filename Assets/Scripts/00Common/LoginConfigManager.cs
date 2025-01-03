using UnityEngine;
using System.Collections;

using LitJson;


public class LoginConfigManager : Singleton<LoginConfigManager>
{
    private class LoginConfig
    {
        ///<summary>
        /// 登录已有角色重试次数
        ///</summary>
        public int userDataReconnectCount    = 5;

        ///<summary>
        /// BaseWaitHTTP单次的超时时间
        ///</summary>
        public int httpDefaultTimeOut        = 5000;

        ///<summary>
        /// BaseWaitHTTP超时之后重试等待时间
        ///</summary>
        public int httpDefaultGapTimeOut     = 1000;

        ///<summary>
        /// BaseWaitHTTP超时之后重试次数
        ///</summary>
        public int httpDefaultReconnectCount = 3;

        public ServerAddress defaultServer   = new ServerAddress();
    }

    private class ServerAddress
    {
        public uint   id     = 1;
        public string ip     = "0.0.0.0";
        public ushort port   = 0;
        public string name   = "";
        public int    status = 0;
    }

    private const string kConfigFileName = "baseloginconfig.conf";

    private LoginConfig  mConfig         = new LoginConfig();

    public uint GetServerAddressID()
    {
        return mConfig.defaultServer.id;
    }

    public string GetServerAddressIP()
    {
        return mConfig.defaultServer.ip;
    }

    public ushort GetServerAddressPort()
    {
        return mConfig.defaultServer.port;
    }

    public string GetServerAddressName()
    {
        return mConfig.defaultServer.name;
    }

    public int GetServerAddressStatus()
    {
        return mConfig.defaultServer.status;
    }

    public int GetUserDataReconnectCount()
    {
        return mConfig.userDataReconnectCount;
    }

    public int GetHttpDefaultTimeOut()
    {
        return mConfig.httpDefaultTimeOut;
    }

    public int GetHttpDefaultGapTimeOut()
    {
        return mConfig.httpDefaultGapTimeOut;
    }

    public int GetHttpDefaultReconnectCount ()
    {
        return mConfig.httpDefaultReconnectCount;
    }

    public void LoadBaseLoginConfig()
    {
        string data = _loadData();

		if (!string.IsNullOrEmpty(data))
        {
            try 
            {
                LoginConfig config        = LitJson.JsonMapper.ToObject<LoginConfig>(data);

                mConfig = config;

                UnityEngine.Debug.LogFormat("[LoginConfig] {0}", ObjectDumper.Dump(config));
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogErrorFormat("[LoginConfig] fail");
            }
        }
        else
        {
            UnityEngine.Debug.LogErrorFormat("[LoginConfig] 加载失败");
        }
    }

    private string _loadData()
    {
        string data = null;

        try 
        {
            if (!FileArchiveAccessor.LoadFileInPersistentFileArchive(kConfigFileName, out data))
            {
                FileArchiveAccessor.LoadFileInLocalFileArchive(kConfigFileName, out data);
            }
        } 
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogErrorFormat("[LoginConfig] 加载失败 {0}", e.ToString());
        }

        return data;
    }
}
