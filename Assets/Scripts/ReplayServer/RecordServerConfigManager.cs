using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using LitJson;

public class RecordServerConfigManager : Singleton<RecordServerConfigManager>
{
    private class RecordServerConfig
    {
        public bool isRecordProcess = false;
        public bool isRecordReplay  = true;
        public bool isPrintStack = false;
    }

    private RecordServerConfig mConfig = new RecordServerConfig();
    private string mVersion = string.Empty;
    public bool isPrintStack()
    {
        if (null == mConfig)
        {
            return false;
        }

        return mConfig.isPrintStack;
    }

    public string GetVersion()
    {
        return mVersion;
    }


    public bool isRecordProcess()
    {
        if (null == mConfig)
        {
            return false;
        }

        return mConfig.isRecordProcess;
    }

    public bool isRecordReplay()
    {
        if (null == mConfig)
        {
            return false;
        }

        return mConfig.isRecordReplay;
    }

    public override void Init()
    {
        _loadRecordServerConfig();
#if LOGIC_SERVER
        try
        {
            string filepath = Path.Combine(Utility.kRawDataPrefix, kVersionFileName).ToLower();
            if (File.Exists(filepath))
            {
                Logger.LogProcessFormat("[recordServerconfig] 加载文件路径 {0}", filepath);

                mVersion = System.IO.File.ReadAllText(filepath);
            }
            else
            {
                Logger.LogErrorFormat("[recordServerconfig] 加载文件路径 {0} 不存在", filepath);
            }
        }
        catch(Exception e)
        {
            Logger.LogErrorFormat("[recordServerconfig] 加载版本文件路径 错误 原因 {0}", e.Message);
        }
#endif
    }

    private const string kConfigFileName = "recordserver.json";
    private const string kVersionFileName = "version.cfg";

    private void _loadRecordServerConfig()
    {
        byte[] data = _loadData();

        if (null == data)
        {
            return ;
        }

        try 
        {
            string content = System.Text.ASCIIEncoding.Default.GetString(data);
            mConfig = LitJson.JsonMapper.ToObject<RecordServerConfig>(content);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("[recordServerconfig] 解析JSON失败 {0}", e.ToString());
        }

        Logger.LogProcessFormat("[recordServerconfig] 是否记录日志 {0}， 是否记录录像 {1}", mConfig.isRecordProcess, mConfig.isRecordReplay);
    }

    private byte[] _loadData()
    {
#if LOGIC_SERVER
        string filepath = Path.Combine(Utility.kRawDataPrefix, kConfigFileName).ToLower();

        if (File.Exists(filepath))
        {
            Logger.LogProcessFormat("[recordServerconfig] 加载文件路径 {0}", filepath);

            return System.IO.File.ReadAllBytes(filepath);
        }
        else 
        {
            Logger.LogErrorFormat("[recordServerconfig] 加载文件路径 {0} 不存在", filepath);
        }

#endif
        return null;
    }
}

