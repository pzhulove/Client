using System.Collections.Generic;
using UnityEngine;

namespace Tenmove.Runtime.Unity
{    
    public class TMUnityAssetTreeData : ScriptableObject, ITMAssetTreeData
    {
        [UnityEngine.SerializeField]
        List<Runtime.AssetDesc> m_AssetDescList;
        [UnityEngine.SerializeField]
        List<Runtime.AssetPackageDesc> m_AssetPackageDescList;

        public void Fill(List<Runtime.AssetDesc> assetDescList,
            List<Runtime.AssetPackageDesc> assetPackageList)
        {
            m_AssetDescList = assetDescList;
            m_AssetPackageDescList = assetPackageList;
        }

        public List<Runtime.AssetDesc> GetAssetDescMap()
        {
            return m_AssetDescList;
        }

        public List<Runtime.AssetPackageDesc> GetAssetPackageDescMap()
        {
            return m_AssetPackageDescList;
        }
    }
}