
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public enum AssetPackageUsage
    {
        LoadFromFile = 0x00,
        LoadAssetWithGUIDName = 0x01,
    }

    [System.Serializable]
    public struct AssetPackageDesc
    {
        public AssetPackageName m_PackageName;
        public int m_PackageID;
        public int[] m_DependencyPackageIDs;
        public uint m_PackageUsage;
        public int m_PackageBytes;
        public int m_PackageCRC32;
        public string m_PackageMD5;

        public AssetPackageDesc(AssetPackageName packageName,int packageID, int[] dependencyPackageIDs, uint packageUsage, int packageByes, int packageCRC32, string packageMD5)
        {
            Debugger.Assert(null != dependencyPackageIDs, "Dependency package can not be null!");

            m_PackageName = packageName;
            m_PackageID = packageID;
            m_PackageUsage = new EnumHelper<AssetPackageUsage>(packageUsage);
            m_PackageBytes = packageByes;
            m_PackageCRC32 = packageCRC32;
            m_PackageMD5 = packageMD5;

            m_DependencyPackageIDs = dependencyPackageIDs;
        }

        public AssetPackageName PackageName
        {
            get
            {
                return m_PackageName;
            }
        }

        public int PackageID
        {
            get
            {
                return m_PackageID;
            }
        }

        public int[] DependencyPackageIDs
        {
            get
            {
                return m_DependencyPackageIDs;
            }
        }

        public EnumHelper<AssetPackageUsage> AssetPackageUsage
        {
            get
            {
                return new EnumHelper<AssetPackageUsage>(m_PackageUsage);
            }
        }

        public int PackageBytes
        {
            get
            {
                return m_PackageBytes;
            }
        }

        public int PackageCRC32
        {
            get
            {
                return m_PackageCRC32;
            }
        }

        public string PackageMD5
        {
            get
            {
                return m_PackageMD5;
            }
        }

        public bool StorageInReadOnly
        {
            get
            {
                return true;
            }
        }
    }

}