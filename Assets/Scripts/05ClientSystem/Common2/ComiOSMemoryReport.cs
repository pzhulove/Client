using UnityEngine;
using UnityEngine.UI;

public class ComiOSMemoryReport : MonoBehaviour 
{
    public Text curMemoryInMB;
    public float time2Update = 1.0f;

    private float mTickTime = 0.0f;

	void Update () 
    {
        mTickTime += Time.unscaledDeltaTime;
        if (mTickTime > time2Update)
        {
            mTickTime -= time2Update;

            curMemoryInMB.text = _getMemorySize();
        }
	}

    string _getMemorySize()
    {
		uint useMonoSize = UnityEngine.Profiling.Profiler.GetMonoUsedSize() / 1024 / 1024;
		uint heapMonoSize = UnityEngine.Profiling.Profiler.GetMonoHeapSize() / 1024 / 1024;

#if UNITY_IOS
        return string.Format("mono:{0}/{1} mem:{2}/{3} package:{4} pool:{5}",
                useMonoSize,
                heapMonoSize,
                iOSUtility.MemoryReport.GetMemoryUsedInMB(),
                iOSUtility.MemoryReport.GetMaxMemoryUsedInMB(),
                AssetPackageManager.instance.GetLoadedAssetPackageCount(),
                CGameObjectPool.instance.GetPooledGameObjectNum());
#else 
		return string.Format("mono:{0}/{1} package:{2} pool:{3} fps:{4} lastFps:{5} promot:{6}",
                useMonoSize,
                heapMonoSize,
                AssetPackageManager.instance.GetLoadedAssetPackageCount(),
                CGameObjectPool.instance.GetPooledGameObjectNum(), 
			ComponentFPS.instance.GetFPS(),
			ComponentFPS.instance.GetLastAverageFPS(),
			GeGraphicSetting.instance.needPromoted);

#endif
    }
}
