using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AssetBundleTool
{
    public class AssetBundelFileMd5 : ScriptableObject
    {
        //[SerializeField]
        public List<PackageFileMd5> packFileMd5List = new List<PackageFileMd5>();
    }
    [System.Serializable]
    public class PackageFileMd5
    {
        public string zPackageName = "";
        public string bundleMd5 = "";
        public List<string> zDependBundleList = new List<string>();//所有依赖的Bundle 文件的列表  添加或者移除时都要重新打这个AssetBundle 
        public List<FileAssetInfo> zFileMd5 = new List<FileAssetInfo>();//文件和文件MD5 
    }
    [System.Serializable]
    public class FileAssetInfo
    {
        public string name = "";
        public string md5 = "";
    }
}
