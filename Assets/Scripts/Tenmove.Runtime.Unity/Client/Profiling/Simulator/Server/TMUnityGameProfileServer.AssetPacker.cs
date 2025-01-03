


namespace Tenmove.Runtime.Unity
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Tenmove.Runtime;
    using Tenmove.Runtime.Unity;
    using UnityEditor;
    using UnityEngine;

    internal partial class UnityGameProfileServer
    {
        public class AssetPackageDesc : ITMGameProfileAssetPackageDesc, ITMGameProfileAssetPackageDescFiller, IEquatable<AssetPackageDesc>
        {
            private readonly List<string> m_PackageAssets;

            public AssetPackageDesc(string name, string md5, long size, IList<string> assets)
            {
                m_PackageAssets = new List<string>();
                PackageName = string.Empty;
                PackageMD5 = string.Empty;
                PackageBytesSize = 0;

                Fill(name, md5, size, assets);
            }

            public string PackageName { private set; get; }
            public string PackageMD5 { private set; get; }
            public long PackageBytesSize { private set; get; }
            public List<string> PackageAssets { get { return m_PackageAssets; } }

            public bool HasContent
            {
                get { return PackageAssets.Count > 0; }
            }

            public bool Equals(AssetPackageDesc other)
            {
                return PackageBytesSize == other.PackageBytesSize &&
                    PackageMD5 == other.PackageMD5 &&
                    PackageName == other.PackageName;
            }

            public override bool Equals(object obj)
            {
                return base.Equals((AssetPackageDesc)obj);
            }

            public void Fill(string packageName, string md5, long size, IList<string> assets)
            { 
                m_PackageAssets.AddRange(assets);
                PackageName = packageName;
                PackageMD5 = md5;
                PackageBytesSize = size;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}