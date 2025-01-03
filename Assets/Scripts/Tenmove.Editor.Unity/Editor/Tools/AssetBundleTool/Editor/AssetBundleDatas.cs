using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace AssetBundleTool
{
    public enum AssetBundelType
    {
        None,//未更新 不变
        New,//新的AssetBundel 
        Update,//更新
    }
    
       
        public class PackAssetDesc
        {
            public PackAssetDesc(string assetPath)
            {
                m_AssetPath = assetPath.Replace('\\', '/');
                m_AssetGUID = AssetDatabase.AssetPathToGUID(m_AssetPath);
                {
                    FileInfo info = new FileInfo(assetPath);
                    m_OriginSize = info.Length;
                }
            }
            public string m_AssetPath;
            public string m_AssetGUID;
            public long m_OriginSize ;//单个资源的大小
        }
    [Serializable]
    public class BuildData
    {
        public CompressOptions m_Compression = CompressOptions.ChunkBasedCompression;
        public bool m_DisableWriteTypeTree = false;
        public bool m_DisableEncryption = false;
        public string m_OutputPath = Application.streamingAssetsPath + "/AssetBundles";
        //public List<string> m_AssetTypes = new List<string>();
        public bool Equals(BuildData obj)
        {
            return obj != null &&
                   m_Compression == obj.m_Compression &&
                   m_DisableWriteTypeTree == obj.m_DisableWriteTypeTree &&
                   m_DisableEncryption == obj.m_DisableEncryption &&
                   m_OutputPath == obj.m_OutputPath;
                   //&& m_AssetTypes ==obj.m_AssetTypes;
        }
    }
    public class AssetPackageDesc
    {
        public string m_PackageName;
        public long m_OriginSize = 0;//整个Bundel包内所有资源大小
        public EAssetType m_AssetType = EAssetType.Actor;
        public List<PackAssetDesc> m_PackageAsset = new List<PackAssetDesc>();//Bundel包内所有的资源
        public List<string> m_DependAssetExceptPackage = new List<string>();//依赖的所有资源  不包含bundel包里所有资源
        public Dictionary<string, string> m_DependAsset = new Dictionary<string, string>();//包含bundel包里所有资源和其依赖的资源  以及其MD5
        public List<string> m_DependPackage = new List<string>();//依赖的AssetBundel包
        public AssetBundelType m_UpdateType = AssetBundelType.None;// assetbundel包的类型
        public HashSet<string> fileSet = new HashSet<string>();
    }
    public enum CompressOptions
    {
        Uncompressed = 0,
        StandardCompression,
        ChunkBasedCompression,
    }
    public enum EAssetType
    {
        Actor = 0,
        Shader = 1,
        Data = 2,
        Scene = 3,
        Effect = 4,
        UI = 5,
        Sound = 6,
    }

    public enum Mode
    {
        PackStrategy = 0,
        Build = 1,
        Inspect = 2
    }
}
