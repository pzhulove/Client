using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AssetBundleRegiester : Singleton<AssetBundleRegiester>
{
    class AssetBundleDesc
    {
        public AssetBundleDesc(AssetBundle assetBundle)
        {
            m_AssetBundle = assetBundle;
            m_RefCount = 0;
        }

        public int m_RefCount;
        public AssetBundle m_AssetBundle;
    }

    private readonly Dictionary<string, AssetBundleDesc> m_AssetBundleRegTable = new Dictionary<string, AssetBundleDesc>();

    public void RegiesterAssetBundle(string assetPackage, AssetBundle assetBundle)
    {
        if (null == assetBundle)
            return;

        AssetBundleDesc assetBundleDesc = null;
        if (m_AssetBundleRegTable.TryGetValue(assetPackage,out assetBundleDesc))
        {
            if (null != assetBundleDesc.m_AssetBundle)
                assetBundleDesc.m_AssetBundle.Unload(false);
            assetBundleDesc.m_AssetBundle = assetBundle;
            assetBundleDesc.m_RefCount = 1;
            return;
        }

        assetBundleDesc = new AssetBundleDesc(assetBundle);
        assetBundleDesc.m_RefCount = 1;
        m_AssetBundleRegTable.Add(assetPackage, assetBundleDesc);
    }

    public AssetBundle AquireAssetBundle(string assetPackage)
    {
        AssetBundleDesc assetBundleDesc = null;
        if (m_AssetBundleRegTable.TryGetValue(assetPackage, out assetBundleDesc))
        {
            ++assetBundleDesc.m_RefCount;
            return assetBundleDesc.m_AssetBundle;
        }

        return null;
    }

    public void ReleaseAssetBundle(AssetBundle assetBundle,bool forceUnload = false)
    {
        Dictionary<string, AssetBundleDesc>.Enumerator it = m_AssetBundleRegTable.GetEnumerator();
        while (it.MoveNext())
        {
            AssetBundleDesc curDesc = it.Current.Value;
            if(curDesc.m_AssetBundle.GetInstanceID() == assetBundle.GetInstanceID())
            {
                --curDesc.m_RefCount;
                if (curDesc.m_RefCount <= 0)
                {
                    curDesc.m_AssetBundle.Unload(forceUnload);
                    m_AssetBundleRegTable.Remove(it.Current.Key);
                }
                return;
            }
        }
    }
}