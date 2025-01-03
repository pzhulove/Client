
using UnityEngine;

public class EngineConfig : Singleton<EngineConfig>
{
    public class EngineConfigData
    {
        public bool asyncPackageLoad = false;
        public bool useTMEngine = true;
        public bool useNewHitText = true;
        public bool useAsyncLoadAnim = true;
        public bool usePrewarmFrame = true;
	    public bool usePackageAsyncLoad = true;
		public int logLevel = 3;
        public bool enableAvatarPartFallback = true;
        public int lowLevelAgentCount = 8;
        public int mediumLevelAgentCount = 8;
        public int highLevelAgentCount = 8;
    }

    private EngineConfigData m_EngineConfigData = new EngineConfigData();
    private const string kConfigFileName = "engineconfig.json";
    private bool useAsyncLoadAnimRuntimeSwitch = false;  // runtime开关是否启用动画异步加载

    private readonly int[] graphicLevelAgentCount = new int[(int)GraphicLevel.Num];

    static public bool asyncPackageLoad
    {
        get { return false; }
    }

    static public bool useTMEngine
    {
        get
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return false;
            else
                return instance.m_EngineConfigData.useTMEngine;
#else
                return instance.m_EngineConfigData.useTMEngine;
#endif
        }
    }

    static public int logLevel
    {
        get
        {
            return instance.m_EngineConfigData.logLevel;
        }
    }

    static public bool asyncLoadAnimRuntimeSwitch
    {
        get { return instance.useAsyncLoadAnimRuntimeSwitch; }
        set { instance.useAsyncLoadAnimRuntimeSwitch = value; }
    }

    static public bool useAsyncLoadAnim
    {
        get { return instance.m_EngineConfigData.useAsyncLoadAnim && instance.useAsyncLoadAnimRuntimeSwitch; }
    }

    static public bool enableAvatarPartFallback
    {
        get { return instance.m_EngineConfigData.enableAvatarPartFallback; }
    }


    static public bool useNewHitText
    {
        get { return instance.m_EngineConfigData.useNewHitText; }
    }

    static public bool usePrewarmFrame
    {
        get { return instance.m_EngineConfigData.usePrewarmFrame; }
        set { instance.m_EngineConfigData.usePrewarmFrame = value; }
    }

	static public bool usePackageAsyncLoad
	{
        get { return instance.m_EngineConfigData.usePackageAsyncLoad; }
	}

    static public int GetAgentCountByGraphicLevel(int graphicLevel)
    {
        if (graphicLevel < instance.graphicLevelAgentCount.Length)
            return instance.graphicLevelAgentCount[graphicLevel];
        else
            return 8;
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
                m_EngineConfigData = LitJson.JsonMapper.ToObject<EngineConfigData>(content);
            }
            else
            {
                UnityEngine.Debug.LogFormat("Engine config load failed!");
            }
        }
        catch(System.Exception e)
        {
            UnityEngine.Debug.LogFormat("Engine config load failed with exception:{0}!",e.Message);
        }

        Debug.LogFormat("EngineConfig:[asyncPackageLoad:{0}] ", m_EngineConfigData.asyncPackageLoad);
        Debug.LogFormat("EngineConfig:[useTMEngine:{0}] ", m_EngineConfigData.useTMEngine);
        Debug.LogFormat("EngineConfig:[useNewHitText:{0}] ", m_EngineConfigData.useNewHitText);
        Debug.LogFormat("EngineConfig:[useAsyncLoadAnim:{0}] ", m_EngineConfigData.useAsyncLoadAnim);
        Debug.LogFormat("EngineConfig:[usePrewarmFrame:{0}] ", m_EngineConfigData.usePrewarmFrame);
        Debug.LogFormat("EngineConfig:[usePackageAsyncLoad:{0}] ", m_EngineConfigData.usePackageAsyncLoad);
        Debug.LogFormat("EngineConfig:[logLevel:{0}] ", m_EngineConfigData.logLevel);
        Debug.LogFormat("EngineConfig:[enableAvatarPartFallback:{0}] ", m_EngineConfigData.enableAvatarPartFallback);
        Debug.LogFormat("EngineConfig:[lowLevelAgentCount:{0}] ", m_EngineConfigData.lowLevelAgentCount);
        Debug.LogFormat("EngineConfig:[mediumLevelAgentCount:{0}] ", m_EngineConfigData.mediumLevelAgentCount);
        Debug.LogFormat("EngineConfig:[highLevelAgentCount:{0}] ", m_EngineConfigData.highLevelAgentCount);

        if(graphicLevelAgentCount.Length >= (int)GraphicLevel.Num)
        {
            graphicLevelAgentCount[(int)GraphicLevel.NORMAL] = m_EngineConfigData.highLevelAgentCount;
            graphicLevelAgentCount[(int)GraphicLevel.MIDDLE] = m_EngineConfigData.mediumLevelAgentCount;
            graphicLevelAgentCount[(int)GraphicLevel.LOW] = m_EngineConfigData.lowLevelAgentCount;
        }
    }

    private byte[] _loadData()
    {
        byte[] data = null;

        try
        {            
            if(!FileArchiveAccessor.LoadFileInPersistentFileArchive(kConfigFileName, out data))
            {
                Debug.LogError("Load engine config file from persistent path has failed!");
                if (!FileArchiveAccessor.LoadFileInLocalFileArchive(kConfigFileName, out data))
                    Debug.LogError("Load engine config file from streaming path has failed!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogErrorFormat("Load engine config file exception:{0}!",e.Message);
        }

        return data;
    }
}
