using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void OnLoadFinishCallback(UnityEngine.Object asset);

public class AssetAsyncLoader : MonoSingleton<AssetAsyncLoader>
{
    // Use this for initialization
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);

        gameObject.transform.position = new Vector3(0, -1000, 0);
    }

    public void SetLoadingLimit(int maxCount)
    {
        AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.RunningTaskLimit = maxCount;
        AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.RunningTaskLimit = maxCount;
        AsyncLoadTaskAllocator<AssetBundleResquestWrapper, UnityEngine.Object>.instance.RunningTaskLimit = maxCount;
    }

    public GameObject root
    {
        get { return gameObject; }
    }

    public bool IsAsyncInLoading
    {
        get {
            return AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.IsResAsyncLoading() ||
              AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.IsResAsyncLoading() ||
              AsyncLoadTaskAllocator<AssetBundleResquestWrapper, UnityEngine.Object>.instance.IsResAsyncLoading();
        }
    }

    public void ClearWaitingQueue()
    {
        AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.ClearWaitingQueue();
        AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.ClearWaitingQueue();
        AsyncLoadTaskAllocator<AssetBundleResquestWrapper, UnityEngine.Object>.instance.ClearWaitingQueue();
    }

    public void ClearFinishQueue()
    {
        AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.ClearFinishQueue();
        AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.ClearFinishQueue();
        AsyncLoadTaskAllocator<AssetBundleResquestWrapper, UnityEngine.Object>.instance.ClearFinishQueue();
    }

    // Update is called once per frame
    void Update ()
    {
        if (AssetGabageCollector.instance.IsUnloadingAssets)
            return;

        AsyncLoadTaskAllocator<ResourceRequestWrapper, UnityEngine.Object>.instance.Update();
        AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.Update();
        AsyncLoadTaskAllocator<AssetBundleResquestWrapper, UnityEngine.Object>.instance.Update();
        
        AssetPackageManager.instance.UpdateAsync();
        AssetLoader.instance.Update();
        CGameObjectPool.instance.Update();
    }
}
