using UnityEngine;
using System.Collections.Generic;

public class AsyncLoadData
{
}

/// <summary>
/// 相当于一个
/// </summary>
public interface IAsyncLoadWrapper<T> where T : class
{
    bool IsReady();
    void Prepare(string loadPath, AsyncLoadData asyncLoadData);
    void DoLoad();
    bool IsDone();
    T Extract();
    bool InLoading();
    void OnAbort();
    void Reset();
}

public class ResourceResquestData : AsyncLoadData
{
    public ResourceResquestData(System.Type type,string subRes = "")
    {
        m_AssetType = type;
        m_SubAsset = subRes;
    }

    public string m_SubAsset = null;
    public System.Type m_AssetType = null;
}

public class ResourceRequestWrapper : IAsyncLoadWrapper<Object>
{
    public ResourceRequest m_Request = null;
    ResourceResquestData m_LoadData = null;
    string m_LoadPath = null;
    public bool IsReady()
    {
        return true;
    }

    public void Prepare(string loadPath, AsyncLoadData asyncLoadData)
    {
        if(null == m_LoadData)
        {
            m_LoadData = asyncLoadData as ResourceResquestData;
            m_LoadPath = loadPath;
        }
    }

    public void DoLoad()
    {
        if (null != m_LoadData)
            m_Request = Resources.LoadAsync(m_LoadPath, m_LoadData.m_AssetType);
        else
            Logger.LogErrorFormat("Async load data is invalid with res path:{0}!", m_LoadPath);
    }


    public bool IsDone()
    {
        return m_Request.isDone;
    }

    public Object Extract()
    {
        return m_Request.asset;
    }

    public bool InLoading()
    {
        return null != m_Request;
    }

    public void OnAbort()
    {
    }

    public void Reset()
    {
        m_Request = null;
        m_LoadData = null;
        m_LoadPath = null;
    }
}

public delegate void onExtractCallback(Object asset, string resName);
public class AssetBundleResquestData : AsyncLoadData
{
    public AssetBundleResquestData(System.Type type,string subAsset, AssetPackage assetPackage, onExtractCallback callback)
    {
        m_SubAsset = subAsset;
        m_AssetType = type;
        m_OnExtractCallback = callback;
        m_AssetPackage = assetPackage;
    }

    public string m_SubAsset = null;
    public System.Type m_AssetType = null;
    public onExtractCallback m_OnExtractCallback = null;
    public AssetPackage m_AssetPackage = null;
}

public class AssetBundleResquestWrapper : IAsyncLoadWrapper<Object>
{
    public AssetBundleRequest m_Request = null;
    AssetBundleResquestData m_LoadData = null;
    protected string m_LoadPath = null;

    public bool IsReady()
    {
        return null != m_LoadData && null != m_LoadData.m_AssetPackage && null != m_LoadData.m_AssetPackage.assetBundle;
    }

    public void Prepare(string loadPath, AsyncLoadData asyncLoadData)
    {
        if (null == m_LoadData)
        {
            m_LoadData = asyncLoadData as AssetBundleResquestData;
            m_LoadPath = loadPath;
        }
    }

    public void DoLoad()
    {
        if (null != m_LoadData)
        {
            /// Debug.LogWarningFormat("Async load with res path:{0}!", m_LoadPath);

            /// string assetNameInPackage = m_LoadPath;
            string assetNameInPackage = null;
            if (AssetLoader.AsyncLoadPackageRes)
            {
                int assetNameInPackageIdx = m_LoadPath.LastIndexOf('/');
                if (assetNameInPackageIdx + 1 < m_LoadPath.Length)
                    assetNameInPackage = m_LoadPath.Substring(assetNameInPackageIdx + 1);
                else
                    assetNameInPackage = m_LoadPath;
            }
            else
                assetNameInPackage = m_LoadPath;

            m_Request = m_LoadData.m_AssetPackage.OnBeginLoadAssetAsync(assetNameInPackage, m_LoadData.m_AssetType, string.IsNullOrEmpty(m_LoadData.m_SubAsset));
        }
        else
            Logger.LogErrorFormat("Async load data is invalid with res path:{0}!", m_LoadPath);
    }

    public bool IsDone()
    {
        return m_Request.isDone;
    }

    public void OnAbort()
    {
    }

    public Object Extract()
    {
        if (null == m_Request)
            return null;

        Object asset = null;
        if (string.IsNullOrEmpty(m_LoadData.m_SubAsset))
        {
            asset = m_Request.asset;
        }
        else
        {
            Object[] subresArray = m_Request.allAssets;
            for (int i = 0, icnt = subresArray.Length; i < icnt; ++i)
            {
                if (subresArray[i].name == m_LoadData.m_SubAsset)
                {
                    asset = subresArray[i];
                    break;
                }
            }
        }

        m_LoadData.m_AssetPackage.OnFinishLoadAsset();
        /// Debug.LogWarningFormat("Finish async load with res path:{0}!", m_LoadPath);
        if (null != asset)
            m_LoadData.m_OnExtractCallback(asset, m_LoadPath);

        return asset;
    }

    public bool InLoading()
    {
        return null != m_Request;
    }

    public void Reset()
    {
        m_Request = null;
        m_LoadData = null;
        m_LoadPath = null;
    }
}

public class AssetBundleCreateRequestData : AsyncLoadData
{
    public AssetBundleCreateRequestData(AssetPackage assetPackage)
    {
        m_AssetPackage = assetPackage;
    }

    public AssetPackage m_AssetPackage = null;
}

public class AssetBundleCreateRequestWrapper : IAsyncLoadWrapper<AssetBundle>
{
    public AssetBundleCreateRequest m_Request = null;
    AssetBundleCreateRequestData m_LoadData = null;
    protected string m_LoadPath = null;
    private AssetBundle m_AassetBundle = null;

    public bool IsReady()
    {
        return m_LoadData.m_AssetPackage.IsDependecyLoaded();
    }

    public void Prepare(string loadPath, AsyncLoadData asyncLoadData)
    {
        if (null == m_LoadData)
        {
            m_LoadData = asyncLoadData as AssetBundleCreateRequestData;
            m_LoadPath = loadPath;
        }
    }

    public void DoLoad()
    {
        if (null != m_LoadData)
        {
            m_AassetBundle = AssetBundleRegiester.instance.AquireAssetBundle(m_LoadPath);
            if (null == m_AassetBundle)
            {
                m_Request = AssetBundle.LoadFromFileAsync(m_LoadPath);
            }
        }
        else
            Logger.LogErrorFormat("Async load data is invalid with res path:{0}!", m_LoadPath);
    }

    public bool IsDone()
    {
        if (null != m_AassetBundle)
			return true;

        if (m_Request.isDone)
        {
            if (null != m_Request.assetBundle)
            {
                m_AassetBundle = m_Request.assetBundle;
                AssetBundleRegiester.instance.RegiesterAssetBundle(m_LoadPath, m_AassetBundle);
                m_Request = null;
            }
            return true;
        }
        else
            return false;
    }

    public AssetBundle Extract()
    {
		if (null != m_AassetBundle)
            return m_AassetBundle;

        if (null == m_Request)
            return null;

        return m_Request.assetBundle;
    }

    public bool InLoading()
    {
        return null != m_Request || null != m_AassetBundle;
    }

    public void OnAbort()
    {
        if (null != m_AassetBundle)
        {
            AssetBundleRegiester.instance.ReleaseAssetBundle(m_AassetBundle, false);
            m_AassetBundle = null;
        }

        if (null != m_Request && m_Request.isDone)
            m_Request.assetBundle.Unload(false);
    }

    public void Reset()
    {
        m_Request = null;
        m_AassetBundle = null;
        m_LoadData = null;
    }
}



