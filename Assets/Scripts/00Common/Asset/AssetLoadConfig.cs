using UnityEngine;
using System;
using System.Collections;
using XUPorterJSON;

public enum AssetLoadConfigItem
{
    AsyncLoad,

    MaxTypeNum,
}

public class AssetLoadConfig : Singleton<AssetLoadConfig>
{
    bool m_AsyncLoad = true;

    // Use this for initialization
    override public void Init()
    {
        m_AsyncLoad = true;

        string filePath = "Environment/LoadConfig.json";
        UnityEngine.Object obj = AssetLoader.instance.LoadRes(filePath).obj;
        if (obj == null)
        {
            return;
        }

        string content = System.Text.ASCIIEncoding.Default.GetString((obj as TextAsset).bytes);
        var assetLoadConfigList = MiniJSON.jsonDecode(content) as Hashtable;
        try
        {
            int configValue = 1;
            string configString = null;

            configString = assetLoadConfigList["AsyncLoad"].ToString();
            if(!string.IsNullOrEmpty(configString))
            {
                if (int.TryParse(configString, out configValue))
                {
                    if (configValue == 0)
                        m_AsyncLoad = false;
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError("读取LoadConfig.json出错" + e.ToString());
        }
    }

    public bool asyncLoad
    {
        get
        {
            return m_AsyncLoad;
        }
    }
}
