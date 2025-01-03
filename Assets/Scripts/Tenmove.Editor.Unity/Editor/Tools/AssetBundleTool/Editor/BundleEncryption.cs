using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace AssetBundleTool
{
    public static class BundleEncryption
    {
        public static void Encryption(AssetBundleManifest manifest, string outputPath)
        {
            string[] bundles = manifest.GetAllAssetBundles();
            foreach (string bundleName in bundles)
            {
                string filepath = outputPath + "/" + bundleName;
                byte[] filedata = File.ReadAllBytes(filepath);
                var offset = bundleName.Length;
                int filelen = (offset + filedata.Length);
                byte[] buffer = new byte[filelen];
                copyByteOffset(filedata, buffer, offset);
                FileStream fs = File.OpenWrite(filepath);
                fs.Write(buffer, 0, filelen);
                fs.Close();
            }
        }

        private static void copyByteOffset(byte[] filedata, byte[] buffer, int v)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (i < v)
                {
                    buffer[i] = filedata[i];
                }
                else
                {
                    buffer[i] = filedata[i - v];
                }
            }

        }
    }
}
