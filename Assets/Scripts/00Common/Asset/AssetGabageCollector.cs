using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum AssetCollectType
{
    None            = 0x00,
    Spirte          = 0x01,
    AudioClip       = 0x02,
    AnimationClip   = 0x04,
    GameObject      = 0x08,
    All             = Spirte | AudioClip | AnimationClip | GameObject,
}

public class AssetGabageCollector : MonoSingleton<AssetGabageCollector>
{
    private readonly Tenmove.Runtime.Unity.AssetGabageCollector m_GabageCollector;
    
    bool m_LockForUnloading = false;

    public AssetGabageCollector()
    {
        m_GabageCollector = new Tenmove.Runtime.Unity.AssetGabageCollector();
        m_GabageCollector.Init();
    }

    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    public bool IsUnloadingAssets
    {
        get { return false; }
    }

    public void ClearUnusedAsset(List<string> reserveAssets = null,bool isForce = false)
    {
#if TEST_SILLFILE_LOAD        
        reserveAssets = CResPreloader.instance.priorityGameObjectKeys;
#endif        

        int level = 0;
        GeGraphicSetting.instance.GetSetting("GraphicLevel", ref level);
        if (level == 2 || isForce)
        {
#if !LOGIC_SERVER
            BeActionFrameMgr.Clear();
#endif
        }

        HashSet<string> reserveAssetsSet = new HashSet<string>();
        if(null != reserveAssets)
        {
            for (int i = 0, icnt = reserveAssets.Count; i < icnt; ++i)
                reserveAssetsSet.Add(reserveAssets[i]);
        }

        StartCoroutine(m_GabageCollector.ClearUnusedAssetAsync(reserveAssetsSet));
    }

    private void Update()
    {
        m_GabageCollector.Update();
    }

    ///public void ClearUnusedAsset(List<string> keys=null)
    ///{
    ///    //return;
    ///
    ///    if (m_NeedClearAsset)
    ///        return;
    ///
	///	//低画质clear skill cache
	///	int level = 0;
	///	GeGraphicSetting.instance.GetSetting("GraphicLevel", ref level);
	///	if (level == 2)
	///	{
	///		BeActionFrameMgr.Clear();
	///	}
    ///
    ///    Logger.LogProcessFormat( "################## Begin purge unused res!#####################");
    ///
    ///    AssetLoader.instance.ResetPurgeTick();
    ///
	///	if (keys == null)
    ///    	CGameObjectPool.instance.ExecuteClearPooledObjects();
	///	else
	///		CGameObjectPool.instance.ExecuteClearPooledObjects(keys);
    ///
    ///    GC.Collect();
    ///
    ///    m_NeedClearAsset = true;
    ///}

    ///protected IEnumerator _CleanSync()
    ///{
    ///    yield return Resources.UnloadUnusedAssets();
    ///
    ///    ObjectLeakDitector.DumpObjectRef();
    ///    yield break;
    ///}   
}
