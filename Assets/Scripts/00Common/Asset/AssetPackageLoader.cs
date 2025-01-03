using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class AssetPackageLoader : MonoSingleton<AssetPackageLoader>
{
    public AssetBundle LoadPackage(AssetPackage package)
    {
        if (null == package)
            return null;
        
        AssetBundle newAssetBundle = null;
        newAssetBundle = _LoadPackageSync(package);

        return newAssetBundle;
    }
    public IAsyncLoadRequest<AssetBundle> LoadPackageAsync(AssetPackage package,bool highPriority)
    {
        if (null == package)
            return AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.INVALID_LOAD_REQUEST;

        return _LoadPackageAsync(package, highPriority);
    }

    public AssetBundle _LoadPackageSync(AssetPackage package)
    {
        AssetBundle bundle = _LoadPackageSync(package,false,true);
        if(null == bundle)
        {
            bundle = _LoadPackageSync(package, false, false);
            if (null == bundle)
            {
                bundle = _LoadPackageSync(package,true,false);
            }
        }

        return bundle;
    }

    public IAsyncLoadRequest<AssetBundle> _LoadPackageAsync(AssetPackage package,bool highPriority)
    {
        IAsyncLoadRequest<AssetBundle> packageRequest = _LoadPackageAsync(package,false,true, highPriority);
        if (!packageRequest.IsValid())
        {
            packageRequest = _LoadPackageAsync(package, false, false, highPriority);
            if (!packageRequest.IsValid())
            {
                packageRequest = _LoadPackageAsync(package, true,false, highPriority);
            }
        }

        return packageRequest;
    }

    /// public AssetBundle _LoadPackageFromNativeSync(AssetPackage package)
    /// {
    ///     string packagePath = Path.Combine(Application.streamingAssetsPath, package.packageFullPath);
    ///     //if (!File.Exists(packagePath))
    ///     //{
    ///     //    Logger.LogAssetFormat("Asset package with Path [{0}] does not exist!" ,packagePath);
    ///     //    return null;
    ///     //}
    /// 
    ///     AssetBundle bundle = AssetBundle.LoadFromFile(packagePath);
    ///     if (null != bundle)
    ///     {
    ///         return bundle;
    ///     }
    ///     else
    ///         Logger.LogAssetFormat("Load asset bundle from file has failed![AssetBundle:{0}]", packagePath);
    ///     /*
    ///     byte[] content = File.ReadAllBytes(packagePath);
    ///     if(null != content)
    ///     {
    ///         AssetBundle bundle = AssetBundle.LoadFromMemory(content);
    ///         if (null != bundle)
    ///         {
    ///             packageBytes = content.Length;
    ///             content = null;
    /// 
    ///             //GC.Collect();
    ///             return bundle;
    ///         }
    ///         else
    ///             Logger.LogAssetFormat( "Load asset bundle from memory has failed(Maybe can not allocate no more memory) [AssetBundle:{0}]!", packagePath);
    ///     }
    ///     */
    ///     return null;
    /// }
    /// public AssetBundle _LoadPackageFromHotFixSync(AssetPackage package)
    /// {
    ///     string packagePath = Path.Combine(Application.persistentDataPath, package.packageFullPath);
    ///     if (!File.Exists(packagePath))
    ///     {
    ///         Logger.LogAssetFormat("Asset package with Path [{0}] does not exist!", packagePath);
    ///         return null;
    ///     }
    /// 
    ///     AssetBundle bundle = AssetBundle.LoadFromFile(packagePath);
    ///     if (null != bundle)
    ///     {
    ///         return bundle;
    ///     }
    ///     else
    ///         Logger.LogAssetFormat("Load asset bundle from file has failed![AssetBundle:{0}]", packagePath);
    /// 
    ///     //byte[] content = File.ReadAllBytes(packagePath);
    ///     //if (null != content)
    ///     //{
    ///     //    AssetBundle bundle = AssetBundle.LoadFromMemory(content);
    ///     //    if (null != bundle)
    ///     //    {
    ///     //        packageBytes = content.Length;
    ///     //        content = null;
    ///     //
    ///     //        //GC.Collect();
    ///     //        return bundle;
    ///     //    }
    ///     //    else
    ///     //        Logger.LogAssetFormat("Load asset bundle from memory has failed(Maybe can not allocate no more memory) [AssetBundle:{0}]!", packagePath);
    ///     //}
    /// 
    ///     return null;
    /// }

    public AssetBundle _LoadPackageSync(AssetPackage package, bool fromNative,bool fromHotfix)
    {
        /// if (package.packageName.Equals("data_table.pck", StringComparison.OrdinalIgnoreCase))
        /// {/// 临时的特殊处理
        ///     byte[] data_table = null;
        ///     if (fromNative)
        ///         FileArchiveAccessor.LoadFileInLocalFileArchive(package.packageFullPath, out data_table);
        ///     else
        ///         FileArchiveAccessor.LoadFileInPersistentFileArchive(fromHotfix ? package.packageHotfixPath : package.packageFullPath, out data_table);
        /// 
        ///     if(null != data_table)
        ///     {/// data_table.pck memory
        ///         AssetBundle bundle = AssetBundle.LoadFromMemory(data_table);
        ///         if (null != bundle)
        ///         {
        ///             return bundle;
        ///         }
        ///         else
        ///             Logger.LogAssetFormat("Load asset bundle from file has failed![AssetBundle:{0}]", package.packageFullPath);
        ///     }
        /// }
        /// else
        /// {
        ///     string assetPackagePath = null;
        ///     if (fromNative)
        ///         assetPackagePath = Application.streamingAssetsPath;
        ///     else
        ///         assetPackagePath = Application.persistentDataPath;
        /// 
        ///     assetPackagePath = Path.Combine(assetPackagePath, fromHotfix ? package.packageHotfixPath : package.packageFullPath);
        /// 
        ///     if (!fromNative)
        ///         if (!File.Exists(assetPackagePath))
        ///             return null;
        /// 
        ///     AssetBundle bundle = AssetBundle.LoadFromFile(assetPackagePath);
        ///     if (null != bundle)
        ///     {
        ///         return bundle;
        ///     }
        ///     else
        ///         Logger.LogAssetFormat("Load asset bundle from file has failed![AssetBundle:{0}]", assetPackagePath);
        /// }
        /// 
        /// return null;
        /// 

        string assetPackagePath = null;
        if (fromNative)
            assetPackagePath = Application.streamingAssetsPath;
        else
            assetPackagePath = Application.persistentDataPath;

        assetPackagePath = Path.Combine(assetPackagePath, fromHotfix ? package.packageHotfixPath : package.packageFullPath);

        if (!fromNative)
            if (!File.Exists(assetPackagePath))
                return null;

        AssetBundle bundle = AssetBundleRegiester.instance.AquireAssetBundle(assetPackagePath);
        if (null != bundle)
            return bundle;
        
        bundle = AssetBundle.LoadFromFile(assetPackagePath);
        if (null != bundle)
        {
            AssetBundleRegiester.instance.RegiesterAssetBundle(assetPackagePath, bundle);
            return bundle;
        }
        else
            Logger.LogAssetFormat("Load asset bundle from file has failed![AssetBundle:{0}]", assetPackagePath);

        return null;
    }


    public IAsyncLoadRequest<AssetBundle> _LoadPackageAsync(AssetPackage package,bool fromNative, bool fromHotfix,bool highPriority)
    {
        string assetPackagePath = null;
        if (fromNative)
        {
            assetPackagePath = Path.Combine(Application.streamingAssetsPath, package.packageFullPath);
            AssetBundle bundle = AssetBundleRegiester.instance.AquireAssetBundle(assetPackagePath);
            if (null != bundle)
                return AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.AllocAsyncTaskWithTarget(bundle, assetPackagePath, highPriority);

            return AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.AllocAsyncTask(assetPackagePath
            , new AssetBundleCreateRequestData(package), highPriority);
        }
        else
        {
            assetPackagePath = Path.Combine(Application.persistentDataPath, fromHotfix ? package.packageHotfixPath : package.packageFullPath);
            if (File.Exists(assetPackagePath))
            {
                AssetBundle bundle = AssetBundleRegiester.instance.AquireAssetBundle(assetPackagePath);
                if (null != bundle)
                    return AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.AllocAsyncTaskWithTarget(bundle, assetPackagePath, highPriority);

                return AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.instance.AllocAsyncTask(
                    assetPackagePath, new AssetBundleCreateRequestData(package), highPriority);
            }
            else
                return AsyncLoadTaskAllocator<AssetBundleCreateRequestWrapper, AssetBundle>.INVALID_LOAD_REQUEST;
        }
    }

    public AssetBundle _LoadPackageFromWWW(AssetPackage package, out long packageBytes, bool bAsync = false)
    {
        packageBytes = 0;

        return null;
    }
}
