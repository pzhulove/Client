using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMEgnine.Runtime.Unity;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    public class UnityResSyncLoader : TMResSyncLoader
    {
        private string m_PackageFullPath = null;
        private string m_PackageUpdateUrl = null;
        private string m_AssetNameInPackage = null;
        private string m_AssetFullPath = null;

        public UnityResSyncLoader()
        {

        }

        public sealed override object LoadPackage(string packageFullpath)
        {
            if (string.IsNullOrEmpty(packageFullpath))
            {
                Debugger.LogError("Can not load asset bundle cause package path is invalid!");
                return null;
            }

            m_PackageFullPath = packageFullpath;
            //AssetBundle assetBundle = AssetBundle.LoadFromFile(m_PackageFullPath);
            //if(null == assetBundle)
            //    Debugger.LogError("Can not load asset bundle from file '{0}' which is not a valid asset bundle resource!", m_PackageFullPath);
            //
            //return assetBundle;

            AssetBundle assetBundle = UnityAssetBundleKeeper.LoadAssetBundleSync(packageFullpath);
            if(null == assetBundle)
                Debugger.LogError("Can not load asset bundle from file '{0}' which is not a valid asset bundle resource!", m_PackageFullPath);
            
            return assetBundle;
        }

        public sealed override object LoadAsset(object package, string assetName, string subResName, Type assetType)
        {
            if (null == package)
                return _LoadAssetFromResource(assetName,subResName, assetType);
            else
                return _LoadAssetFromPackage(package, assetName,subResName ,assetType);
        }

        protected object _LoadAssetFromPackage(object package,string assetNameInPackage, string subResName, Type assetType)
        {
            AssetBundle assetBundle = package as AssetBundle;
            if (null == assetBundle)
            {
                Debugger.LogError("Cant not load asset from loaded resource which is not a unity asset bundle!");
                return null;
            }

            if (string.IsNullOrEmpty(assetNameInPackage))
            {
                Debugger.LogError("Can not load asset form asset bundle cause asset name in package is invalid!");
                return null;
            }

            m_AssetNameInPackage = assetNameInPackage;

            UnityEngine.Object asset = null;
            if (!String.IsNullOrEmpty(subResName))
            {
                if (assetType == typeof(Sprite))
                {
                    Sprite[] spriteArray = assetBundle.LoadAssetWithSubAssets<Sprite>(CFileManager.EraseExtension(m_AssetNameInPackage));
                    for (int i = 0; i < spriteArray.Length; ++i)
                    {
                        if (subResName == spriteArray[i].name)
                        {
                            asset = spriteArray[i];
                            break;
                        }
                    }
                }
            }

            if (null == asset)
                asset = assetBundle.LoadAsset(m_AssetNameInPackage, assetType);

            if(null == asset)
                Debugger.LogError("Can not load asset named '{0}' from asset bundle which does not exist in bundle '{1}'!", m_AssetNameInPackage,m_PackageFullPath);
            
            return new UnityAssetObject(asset);
        }

        protected object _LoadAssetFromResource(string assetFullPath, string subResName, Type assetType)
        {
            if (string.IsNullOrEmpty(assetFullPath))
            {
                Debugger.LogError("Asset path is invalid!");
                return null;
            }

            m_AssetFullPath = Utility.Path.ChangeExtension(assetFullPath, null) ;

            UnityEngine.Object asset = null;
            if (!String.IsNullOrEmpty(subResName))
            {
                if (assetType == typeof(Sprite))
                {
                    Sprite[] spriteArray = Resources.LoadAll<Sprite>(CFileManager.EraseExtension(m_AssetFullPath));
                    subResName = Utility.Path.ChangeExtension(subResName, null);

                    for (int i = 0; i < spriteArray.Length; ++i)
                    {
                        if (spriteArray[i].name == subResName)
                        {
                            asset = spriteArray[i];
                            break;
                        }
                    }
                }
            }

            if (null == asset)
                asset = Resources.Load(m_AssetFullPath, assetType);

            if (null == asset)
                Debugger.LogWarning("Can not load asset named '{0}'!", m_AssetFullPath);

            return new UnityAssetObject(asset);
        }

        public sealed override bool LoadFile(string filepath,bool readWritePath,out byte[] filedata)
        {
            filedata = null;
            if (string.IsNullOrEmpty(filepath))
            {
                Debugger.LogError("file path is invalid!");
                return false;
            }

            if(!Utility.File.Exists(filepath))
            {
                Debugger.LogError("File with file path '{0}' does not exist!", filepath);
                return false;
            }

            using (FileStream fileStream = Utility.File.OpenRead(filepath))
            {
                int fileBytes = (int)fileStream.Length;
                filedata = new byte[fileBytes];
                fileStream.Seek(0, SeekOrigin.Begin);
                int readBytes = fileStream.Read(filedata, 0, fileBytes);
                fileStream.Close();
                if (readBytes != fileBytes)
                {
                    Debugger.LogError("Read file '{0}' has failed!(total bytes:{1}, read bytes:{2})", filepath, fileBytes, readBytes);
                    return false;
                }

                return true;
            }
        } 

        public sealed override void UnloadPackage(string packagePath)
        {
            /// AssetBundle assetBundle = package as AssetBundle;
            /// if (null == assetBundle)
            /// {
            ///     Debugger.LogError("Cant not unload asset from loaded resource which is not a unity asset bundle!");
            ///     return;
            /// }
            /// 
            /// assetBundle.Unload(true);
            UnityAssetBundleKeeper.UnloadAssetBundle(packagePath);
        }

        public sealed override void Reset()
        {
            m_PackageFullPath = null;
            m_PackageUpdateUrl = null;
            m_AssetNameInPackage = null;
            m_AssetFullPath = null;
        }
    }
}

