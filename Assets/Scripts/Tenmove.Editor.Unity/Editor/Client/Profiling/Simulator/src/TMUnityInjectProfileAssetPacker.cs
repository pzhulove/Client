

namespace Tenmove.Editor.Unity
{
    using System.Collections.Generic;
    using System.IO;
    using Tenmove.Runtime;
    using Tenmove.Runtime.Unity;
    using UnityEditor;
    using UnityEngine;

    internal partial class DLUnityInjectProfileAssetPacker : ITMGameProfileAssetPacker
    {
        /// <summary>
        /// 打包资源
        /// </summary>
        /// <param name="targetFolder"></param>
        /// <param name="packageName"></param>
        /// <param name="assetList"></param>
        /// <returns></returns>
        public bool PackAsset(string targetFolder, string packageName, List<string> assetList, ITMGameProfileAssetPackageDescFiller packageDescFiller)
        {
            if (!string.IsNullOrEmpty(targetFolder))
            {
                if (!string.IsNullOrEmpty(packageName))
                {
                    if (null != assetList && assetList.Count > 0)
                    {
                        if (!Runtime.Utility.Directory.Exists(targetFolder))
                            Runtime.Utility.Directory.CreateDirectory(targetFolder);

                        string packageFileName = Runtime.Utility.Path.ChangeExtension(packageName, "pck");
                        string packageFilePath = Runtime.Utility.Path.Combine(targetFolder, packageFileName);
                        if (Runtime.Utility.Directory.Exists(targetFolder))
                        {
                            string[] files = Runtime.Utility.Directory.GetFiles(targetFolder, "*.*", false);
                            for (int i = 0, icnt = files.Length; i < icnt; ++i)
                                Runtime.Utility.File.Delete(files[i]);
                        }

                        AssetBundleBuild assetBundlebuild = new AssetBundleBuild();
                        assetBundlebuild.assetBundleName = packageFileName;
                        assetBundlebuild.assetNames = new string[assetList.Count];
                        assetBundlebuild.addressableNames = new string[assetList.Count];
                        for (int i = 0, icnt = assetList.Count; i < icnt; ++i)
                        {
                            string assetPath = assetList[i];
                            assetBundlebuild.assetNames[i] = assetPath;
                            assetBundlebuild.addressableNames[i] = assetPath.Replace("Assets/Resources/", null);
                        }

                        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(targetFolder, new AssetBundleBuild[] { assetBundlebuild }, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

                        if (null != manifest)
                        {
                            string packageMD5 = string.Empty;
                            long fileLength = 0;
                            using (Stream packageFile = Runtime.Utility.File.OpenRead(packageFilePath))
                            {
                                fileLength = packageFile.Length;

                                IDataVerifier<string> md5Verifier = new MD5Verifier();
                                md5Verifier.BeginVerify(packageFile, 256 * 1024, 0.1f);
                                while (!md5Verifier.EndVerify())
                                    EditorUtility.DisplayProgressBar("Packing package", "Generating package MD5...", md5Verifier.Progress);

                                packageMD5 = md5Verifier.GetVerifySum();
                                packageFile.Close();

                                EditorUtility.ClearProgressBar();
                            }

                            packageDescFiller.Fill(packageFilePath, packageMD5, fileLength, assetList);
                        }
                    }
                    else
                        Debugger.LogWarning("Parameter 'assetList' can not be null or empty!");
                }
                else
                    Debugger.LogWarning("Parameter 'packageName' can not be null or empty string!");
            }
            else
                Debugger.LogWarning("Parameter 'targetFolder' can not be null or empty string!");

            return false;
        }
    }
}