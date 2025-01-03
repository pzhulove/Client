using UnityEngine;
using System;
using System.Collections;
using XUPorterJSON;

public enum AssetGCTickType
{
    Asset,
    SceneActor,
    UIFrame,

    MaxTypeNum,
}

public class AssetGabageCollectorHelper : Singleton<AssetGabageCollectorHelper>
{
    protected readonly int UPDATE_STEP = 5;
    protected readonly int GC_FRAME_CNT = 1500;
    protected int m_CurUpdateCnt = 0;
    protected int m_FrameCount = 0;

    protected int[] m_CurTickCount = new int[(int)AssetGCTickType.MaxTypeNum];
    protected int[] m_AutoTickCount = new int[(int)AssetGCTickType.MaxTypeNum];
    protected bool[] m_AutoTickEnable = new bool[(int)AssetGCTickType.MaxTypeNum];

    // Use this for initialization
    override public void Init()
    {
        _ResetConfig();
    }


    // Update is called once per frame
    public void Update ()
    {
        ++m_CurUpdateCnt;
        ++m_FrameCount;
        if (UPDATE_STEP == m_CurUpdateCnt)
        {
            bool bProcessedGC = false;
            int length = m_AutoTickCount.Length < m_CurTickCount.Length ? m_AutoTickCount.Length : m_CurTickCount.Length;
            for (int i = 0,icnt = length; i<icnt;++i)
            {
                if(0 == m_AutoTickCount[i] || !m_AutoTickEnable[i]) continue;

                if(m_AutoTickCount[i] <= m_CurTickCount[i] && m_FrameCount > GC_FRAME_CNT)
                {
                    bProcessedGC = true;
                    AssetGabageCollector.instance.ClearUnusedAsset();
                }
            }

            if(bProcessedGC)
            {
                for (int i = 0, icnt = m_CurTickCount.Length; i < icnt; ++i)
                    m_CurTickCount[i] = 0;

                m_FrameCount = 0;
            }

            m_CurUpdateCnt = 0;
        }
    }

    public void AddGCPurgeTick(AssetGCTickType tickType)
    {
        int curTickType = (int)tickType;
        if (curTickType < m_CurTickCount.Length)
        {
            m_CurTickCount[curTickType] ++;
        }
    }

    public void SetGCPurgeEnable(AssetGCTickType tickType,bool enable)
    {
        int curTickType = (int)tickType;
        if (curTickType < m_AutoTickEnable.Length)
        {
            m_AutoTickEnable[curTickType] = enable;
        }
    }

    public void _ResetConfig()
    {
        for (int i = 0, icnt = m_CurTickCount.Length; i < icnt; ++i)
            m_CurTickCount[i] = 0;
        for (int i = 0, icnt = m_AutoTickEnable.Length; i < icnt; ++i)
            m_AutoTickEnable[i] = false;
    }

    public void LoadGCConfig()
    {
        _ResetConfig();

        string filePath = "GCConfig.json";
        byte[] configData = _LoadConfig(filePath);

        string content = System.Text.ASCIIEncoding.Default.GetString(configData);
        var AutoPurgeList = MiniJSON.jsonDecode(content) as Hashtable;
        try
        {
            int autoPurgeCnt = 300;
            string autoPurgeString = null;

            autoPurgeString = AutoPurgeList["Asset"].ToString();
            if (!string.IsNullOrEmpty(autoPurgeString))
            {
                if (int.TryParse(autoPurgeString, out autoPurgeCnt))
                    m_AutoTickCount[(int)AssetGCTickType.Asset] = autoPurgeCnt;
            }

            autoPurgeString = AutoPurgeList["SceneActor"].ToString();
            if (!string.IsNullOrEmpty(autoPurgeString))
            {
                if (int.TryParse(autoPurgeString, out autoPurgeCnt))
                    m_AutoTickCount[(int)AssetGCTickType.SceneActor] = autoPurgeCnt;
            }

            autoPurgeString = AutoPurgeList["UIFrame"].ToString();
            if (!string.IsNullOrEmpty(autoPurgeString))
            {
                if (int.TryParse(autoPurgeString, out autoPurgeCnt))
                    m_AutoTickCount[(int)AssetGCTickType.UIFrame] = autoPurgeCnt;
            }
        }
        catch (Exception e)
        {
            Logger.LogError("读取CGConfig.json出错" + e.ToString());
        }
    }

    private byte[] _LoadConfig(string configFile)
    {
        byte[] data = null;

        try
        {
            if (!FileArchiveAccessor.LoadFileInPersistentFileArchive(configFile, out data))
            {
                Debug.LogError("Load GC config file from persistent path has failed!");
                if (!FileArchiveAccessor.LoadFileInLocalFileArchive(configFile, out data))
                    Debug.LogError("Load GC config file from streaming path has failed!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogErrorFormat("Load GC config file exception:{0}!", e.Message);
        }

        return data;
    }
}
