
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    [System.Serializable]
    public struct AssetPackageName : IComparable, IComparable<AssetPackageName>, IEquatable<AssetPackageName>
    {
        public string m_PackageName;
        public string m_Variant;

        public AssetPackageName(string name, string variant)
        {
            if (string.IsNullOrEmpty(name))
                Debugger.AssertFailed("Resource name can not be null or empty!");

            m_PackageName = name;
            m_Variant = variant;
        }

        public string Name
        {
            get
            {
                return m_PackageName;
            }
        }

        public string Variant
        {
            get
            {
                return m_Variant;
            }
        }

        public bool IsVariant
        {
            get
            {
                return !string.IsNullOrEmpty(m_Variant);
            }
        }

        public string FullName
        {
            get
            {
                return IsVariant ? string.Format("{0}.{1}", m_PackageName, m_Variant) : m_PackageName;
            }
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public override bool Equals(object value)
        {
            return (value is AssetPackageName) && (this == (AssetPackageName)value);
        }

        public bool Equals(AssetPackageName assetPackageName)
        {
            return (this == assetPackageName);
        }

        public static bool operator ==(AssetPackageName assetPackageName1, AssetPackageName assetPackageName2)
        {
            return assetPackageName1.CompareTo(assetPackageName2) == 0;
        }

        public static bool operator !=(AssetPackageName assetPackageName1, AssetPackageName assetPackageName2)
        {
            return assetPackageName1.CompareTo(assetPackageName2) != 0;
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            if (!(value is AssetPackageName))
            {
                throw new EngineException("Type of value is invalid.");
            }

            return CompareTo((AssetPackageName)value);
        }

        public int CompareTo(AssetPackageName assetPackageName)
        {
            int result = string.Compare(m_PackageName, assetPackageName.m_PackageName);
            if (result != 0)
            {
                return result;
            }

            return string.Compare(m_Variant, assetPackageName.m_Variant);
        }
    }

    /// <summary>
    /// 资源名称比较器。Struct作为Dictionary的key在Full AOT下有问题，需要自己实现IEqualityComparer
    /// </summary>
    public sealed class AssetPackageNameComparer : IComparer<AssetPackageName>, IEqualityComparer<AssetPackageName>
    {
        public int Compare(AssetPackageName x, AssetPackageName y)
        {
            return x.CompareTo(y);
        }

        public bool Equals(AssetPackageName x, AssetPackageName y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(AssetPackageName obj)
        {
            return obj.GetHashCode();
        }
    }

}