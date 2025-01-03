using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemConfig : Singleton<SystemConfig>
{
    protected readonly string mConfigPath = "system.conf";

    protected class SystemConfigData
    {
        public bool removeRefOnClose = false;
    }

    protected SystemConfigData mConfigData = new SystemConfigData();

    public void LoadConfig()
    {
        byte[] data = _loadData();

        if (null != data)
        {
            string content = System.Text.ASCIIEncoding.Default.GetString(data);

            mConfigData = LitJson.JsonMapper.ToObject<SystemConfigData>(content);
            GameClient.ClientSystemManager.sRemoveRefOnClose = mConfigData.removeRefOnClose;
        }
        else
        {
            UnityEngine.Debug.LogFormat("[SystemConfig] 加载失败");
        }
    }


    private byte[] _loadData()
    {
        byte[] data = null;

        try
        {
#if UNITY_EDITOR
            data = CFileManager.ReadFile(System.IO.Path.Combine(Application.streamingAssetsPath, mConfigPath));
#else
            FileArchiveAccessor.LoadFileInPersistentFileArchive(mConfigPath, out data);

            if (null == data || data.Length <= 0)
            {
#if UNITY_ANDROID
                //data = CFileManager.ReadFileFromZip(mConfigPath);
#else 
                data = CFileManager.ReadFile(System.IO.Path.Combine(Application.streamingAssetsPath, mConfigPath));
#endif
            }
#endif
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogFormat("[SystemConfig] 加载失败:" + e.ToString());
        }

        return data;
    }
}
