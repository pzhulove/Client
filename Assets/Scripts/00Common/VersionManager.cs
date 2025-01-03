using UnityEngine;
using System.Collections;
using XUPorterJSON;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;

public class VersionManager : Singleton<VersionManager> {

    // release 
    public int serverVersion = 0;
    public int serverShortVersion = 0;
    public int clientVersion = 0;
    public int clientShortVersion = 0;

    private string mCommitTime;
    public string commitTime
    {
        get
        {
            return mCommitTime;
        }

        private set
        {
            mCommitTime = value;
        }
    }

    // debug
    public string commitMessage;
    public string commitAuthor;
    public string commitID;

    public bool m_ProceedHotUpdate = false;
    public bool m_IsLastest = false;
    public bool m_IsLocalNewer = false;

    private string GetFilePath(bool persistentPath)
    {
#if LOGIC_SERVER
        return "version.config";
#else
        string path = null;
        if (persistentPath)
        {
            path = Utility.FormatString(Path.Combine(Application.persistentDataPath, "version.config"));
            if (CFileManager.IsFileExist(path))
                return path;
        }
        else
        {
            path = Utility.FormatString(Path.Combine(Application.streamingAssetsPath, "version.config"));
            if (CFileManager.IsFileExist(path))
                return path;
        }

        Logger.LogErrorFormat("{0} 不存在", path);
        return "";
#endif
    }

    private byte[] GetFileData(bool peresistent)
    {
        byte[] data = null;

#if LOGIC_SERVER
        data = new byte[0];
        //File.ReadAllBytes("version.config");
#elif UNITY_EDITOR
        string filePath = null;
        filePath = GetFilePath(peresistent);
        if(!string.IsNullOrEmpty(filePath))
        {
            data = CFileManager.ReadFile(filePath);
        }
#elif UNITY_ANDROID
        if (!peresistent)
            data = CFileManager.ReadFileFromZip("version.config");
        else
        {
            FileArchiveAccessor.LoadFileInPersistentFileArchive("version.config", out data);
        }
#else
        string filePath = null;
        filePath = GetFilePath(peresistent);
        if(!string.IsNullOrEmpty(filePath))
        {
            data = CFileManager.ReadFile(filePath);
        }
#endif
        return data;
    }

    public override void Init()
    {
        UpdateAssInfo(this.GetType().Assembly);
        
        /// 先读本地版本
        byte[] dataLocal = GetFileData(false);
        int localVersion = 0;
        int localShortVersion = 0;
        _ParsePatchVersion(dataLocal,out localShortVersion, out localVersion);
//        Debug.LogWarning("Local Version:"+ localVersion);
        byte[] dataHotFix = GetFileData(true);
        int hotfixVersion = 0;
        int hotfixShortVersion = 0;
        _ParsePatchVersion(dataHotFix,out hotfixShortVersion,out hotfixVersion);
 //       Debug.LogWarning("Hotfix Version:" + hotfixVersion);

        if (hotfixShortVersion > localShortVersion && hotfixVersion >= localVersion)
            m_IsLocalNewer = false;
        else
            m_IsLocalNewer = true;
        if (m_IsLocalNewer)
            _ParseVersionFromData(dataLocal);
        else
            _ParseVersionFromData(dataHotFix);
    }

    public void _ParsePatchVersion(byte[] data,out int cientShortVersion,out int clientVersion)
    {
        cientShortVersion = 0;
        clientVersion = 0;
        if(null != data)
        {
            string json = System.Text.UTF8Encoding.UTF8.GetString(data);
            var setting = MiniJSON.jsonDecode(json) as Hashtable;
            try
            {
                cientShortVersion = int.Parse(setting["clientShortVersion"].ToString());
                clientVersion = int.Parse(setting["clientVersion"].ToString());
            }
            catch (Exception e)
            {
                Logger.LogError("读version.config出错" + e.ToString());
            }
        }

    }

    public void _ParseVersionFromData(byte[] data)
    {
        string json = System.Text.UTF8Encoding.UTF8.GetString(data);
        var setting = MiniJSON.jsonDecode(json) as Hashtable;
        try
        {
            serverVersion = int.Parse(setting["serverVersion"].ToString());
            serverShortVersion = int.Parse(setting["serverShortVersion"].ToString());
            clientVersion = int.Parse(setting["clientVersion"].ToString());
            clientShortVersion = int.Parse(setting["clientShortVersion"].ToString());

            commitTime = setting["commitTime"].ToString().Trim();

            {
                long ltime = 0;
                if (long.TryParse(commitTime, out ltime))
                {
                    var dataTime = Utility.ToUtcTime2Local(ltime);
                    commitTime = dataTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }

            commitMessage = setting["commitMessage"].ToString();
            commitAuthor = setting["commitAuthor"].ToString();
            commitID = setting["commitID"].ToString();
            
        }
        catch (Exception e)
        {
            Logger.LogError("读version.config出错");
        }
    }

    public override void UnInit()
    {
    }

    public string Version()
    {
        return string.Format("{0}.{1}.{2}.{3}", serverVersion, serverShortVersion, clientVersion, clientShortVersion);
    }
    public int ClientShortVersion()
    {
        return clientShortVersion;
    }

    public string VersionCommitTime()
    {

        return commitTime;
    }

    public UInt32 ServerVersion()
    {
        UInt32 sVersion = (UInt32)serverVersion;
        sVersion = sVersion << 8;

        sVersion += (UInt32)serverShortVersion;
        sVersion = sVersion << 16;

        sVersion += (UInt32)clientVersion;

        return sVersion;
    }

    public string Comment()
    {
        return string.Format("{0}.{1}.{2}", commitMessage, commitAuthor, commitID);
    }
    
    
    public uint DllVersion
    {
        get; private set;
    }

    static T GetAssemblyAttribute<T>(Assembly assembly)
        where T : Attribute
    {
        // Get attributes of this type.
        object[] attributes =
            assembly.GetCustomAttributes(typeof(T), true);

        // If we didn't get anything, return null.
        if ((attributes == null) || (attributes.Length == 0))
            return null;

        // Convert the first attribute value into
        // the desired type and return it.
        return (T)attributes[0];
    }

    void UpdateAssInfo(Assembly assembly)
    {
        if (null == assembly)
        {
            return;
        }

        AssemblyDescriptionAttribute assemblyAttr = GetAssemblyAttribute<AssemblyDescriptionAttribute>(assembly);
        if (assemblyAttr != null)
        {
            try 
            {
                string[] vers = assemblyAttr.Description.Split('.');
                if (vers.Length >= 2)
                {
                    uint dllVer = 0;
                    uint.TryParse(vers[1], out dllVer);
                    DllVersion = dllVer;
                    Logger.LogFormat("Dll版本号 更新 {0}", DllVersion);
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("加载Dll版本号粗错 {0}", e.ToString());
            }
        }
    }

}
