
using System;

namespace Tenmove.Runtime
{
    [System.Serializable]
    public struct AssetDesc
    {
        public string m_AssetName;
        public string m_AssetGUIDName;
        public int m_AssetPackageID;

        public AssetDesc(string assetName, string assetGUIDName, int assetPackageID)
        {
            m_AssetName = assetName;
            m_AssetGUIDName = assetGUIDName;
            m_AssetPackageID = assetPackageID;
        }

        public string AssetName
        {
            get { return m_AssetName; }
        }

        public string AssetGUIDName
        {
            get { return m_AssetGUIDName; }
        }

        public int AssetPackageID
        {
            get { return m_AssetPackageID; }
        }
    }

}