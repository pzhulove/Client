using UnityEngine;
using System.Collections.Generic;

using DAssetPackageFlags = System.Int32;
public enum DAssetPackageFlag
{
    None            = 0x00,
    LeaveWithAsset  = 0x01, /// 标志该Bundle和其所依赖的Bundle驻留于内存 知道依赖它们的所有资源释放完毕
    UsingGUIDName   = 0x02, /// 标志该Bundle中的资源打包是使用GUID作为资源索引名 
}

[System.Serializable]
public class DPackAssetDesc
{
    public DPackAssetDesc(string asset,string guid)
    {
        packageAsset = asset;
        packageGUID = guid;
    }

    public string packageAsset;
    public string packageGUID;
}


[System.Serializable]
public class DAssetPackageDesc
{
    [System.NonSerialized]
    public string[] packageDependency = new string[0];
    [System.NonSerialized]
    public DPackAssetDesc[] packageAsset = new DPackAssetDesc[0];
    [System.NonSerialized]
    public AssetPackage assetPackage = null;
    [System.NonSerialized]
    public string packageMD5 = "";
    [System.NonSerialized]
    public string packageKey = "";
    [System.NonSerialized]
    public string[] packageAutoDepend = new string[0];

    public int[] packageAutoDependIdx = new int[0];
    public string packagePath = "";
    public string packageName = "";
    public int packageVer = 0;
    public uint packageFlag = (int)DAssetPackageFlag.None;
}

public enum DAssetPackageDescType
{
    Invalid,
    Native,
    Remote,
}

[System.Serializable]
public struct DAssetPackageMapDesc
{
    public string assetPathKey;
    public string assetPackageGUID;
    public int packageDescIdx;
}

public class DAssetPackageDependency : ScriptableObject
{
    public int patchVersion = 0;
    public DAssetPackageDescType packageDescType = DAssetPackageDescType.Invalid;
    public DAssetPackageDesc[] packageDescArray = new DAssetPackageDesc[0];

    public List<DAssetPackageMapDesc> assetDescPackageMap = new List<DAssetPackageMapDesc>();

    static public int Comparison(DAssetPackageMapDesc x, DAssetPackageMapDesc y)
    {
        return x.assetPathKey.CompareTo(y.assetPathKey);
    }
}
