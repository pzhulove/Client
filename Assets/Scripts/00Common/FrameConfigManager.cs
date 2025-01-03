using UnityEngine;
using System.Collections;

using LitJson;

public class FrameConfigManager : Singleton<FrameConfigManager>
{
    public class FrameConfig
    {
        public uint logicUpdateStep = 16;
        public uint logicFrameStep  = 1;
        public int  logicFrameStepDelta = 1;
    }

    private const string kConfigFileName = "frameconfig.conf";

    public void LoadFrameConfig()
    {
        byte[] data = _loadData();

        if (null != data)
        {
            string content            = System.Text.ASCIIEncoding.Default.GetString(data);

            FrameConfig config        = LitJson.JsonMapper.ToObject<FrameConfig>(content);

            //FrameSync.logicFrameStep  = config.logicFrameStep;
            //FrameSync.logicUpdateStep = config.logicUpdateStep;
            FrameSync.logicUpdateStep = 32;
            FrameSync.logicFrameStepDelta = config.logicFrameStepDelta;

            UnityEngine.Debug.LogFormat("[FrameConfig] 更新设置 {0} , {1}, {2}", Global.Settings.logicFrameStep, FrameSync.logicUpdateStep,FrameSync.logicFrameStepDelta);
        }
        else
        {
            UnityEngine.Debug.LogFormat("[FrameConfig] 加载失败");
        }
    }

    private byte[] _loadData()
    {
        byte[] data = null;

        try {
#if UNITY_EDITOR
            data = CFileManager.ReadFile(System.IO.Path.Combine(Application.streamingAssetsPath, kConfigFileName));
#else
            FileArchiveAccessor.LoadFileInPersistentFileArchive(kConfigFileName, out data);

            if (null == data || data.Length <= 0)
            {
#if UNITY_ANDROID
                //data = CFileManager.ReadFileFromZip(kConfigFileName);
#else 
                data = CFileManager.ReadFile(System.IO.Path.Combine(Application.streamingAssetsPath, kConfigFileName));
#endif
            }
#endif
        } 
        catch (System.Exception e)
        {

        }

        return data;
    }
}
