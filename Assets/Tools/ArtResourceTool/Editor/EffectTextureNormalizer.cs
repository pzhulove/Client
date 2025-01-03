using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EffectTextureNormalizer : Editor
{
    static protected readonly string RES_PATH = "Assets/Resources/Effects";
    static protected List<GameObject> m_EffectLIst = new List<GameObject>();

    [MenuItem("[TM工具集]/ArtTools/Normalize Effect Texture")]
    static public void NormalizeMaxParticles()
    {
        string[] prefabAll = AssetDatabase.FindAssets("t:texture", new string[] { RES_PATH });
        Dictionary<string, List<string>> effResMap = new Dictionary<string, List<string>>();
        for (int i = 0, icnt = prefabAll.Length; i < icnt; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabAll[i]);
            effResMap.Add(path, new List<string>());
        }

        List<string> nullTexture = new List<string>();
        string[] allprefab = AssetDatabase.FindAssets("t:prefab", new string[] { RES_PATH });
        for (int i = 0, icnt = allprefab.Length; i < icnt; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(allprefab[i]);
            UnityEngine.GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            string[] deps = AssetDatabase.GetDependencies(path);
            for (int j = 0, jcnt = deps.Length; j < jcnt; ++j)
            {
                string deppath = deps[j];
                string ext = Path.GetExtension(deppath);
                if( ext.Equals(".jpg",System.StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(".tga", System.StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(".png", System.StringComparison.OrdinalIgnoreCase))
                {
                    List<string> curList = null;
                    if (effResMap.TryGetValue(deppath, out curList))
                        curList.Add(path);
                    else
                        nullTexture.Add(deppath);
                }
            }
        }


        string m_DumpFile = "EffectTexture.csv";
        FileStream fs = new FileStream(m_DumpFile, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();
        sw.BaseStream.Seek(0, SeekOrigin.End);

        Dictionary<string, List<string>>.Enumerator it = effResMap.GetEnumerator();
        while (it.MoveNext())
        {
            List<string> prefabLst = it.Current.Value;
            string texRes = it.Current.Key;

            string buf = texRes;
            buf += ",";
            buf += FileUtil.GetFileBytes(texRes);
            buf += ",";
            buf += (FileUtil.GetFileBytes(texRes) / (1024.0f * 1024.0f)).ToString("0.000");
            buf += ",";
            buf += prefabLst.Count;
            buf += ",";
            for (int i = 0, icnt = prefabLst.Count; i<icnt;++i)
            {
                buf += prefabLst[i] + "|";
            }
            sw.WriteLine(buf.Substring(0, buf.Length - 1));
        }

        for (int i = 0, icnt = nullTexture.Count; i < icnt; ++i)
        {
            sw.WriteLine(nullTexture[i]);
        }

        sw.Flush();
        sw.Close();
        fs.Close();

        List<string> assetPackRes = new List<string>();
        it = effResMap.GetEnumerator();
        while(it.MoveNext())
        {
            List<string> prefabLst = it.Current.Value;
            if(prefabLst.Count > 1)
                assetPackRes.Add(it.Current.Key);
        }

        AssetBundleBuild[] bundlePackageInfoBuild = new AssetBundleBuild[1];
        bundlePackageInfoBuild[0].assetBundleName = "EffectTexture.pak";
        bundlePackageInfoBuild[0].assetNames = assetPackRes.ToArray();// new string[1] { "Assets/Resources/Base/Version/PackageInfo.asset" };
        bundlePackageInfoBuild[0].assetBundleVariant = "";

        AssetBundleManifest assetManifestPackageInfo = BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles/", bundlePackageInfoBuild, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

        //Dictionary<string, List<string>>.Enumerator it = effResMap.GetEnumerator();
        //while(it.MoveNext())
        //{
        //    List<string> prefabLst = it.Current.Value;
        //    string oldPath = it.Current.Key;
        //    string textFileName = Path.GetFileName(oldPath);
        //    if(prefabLst.Count < 10)
        //    {
        //        string newDir = Path.GetDirectoryName(prefabLst[0]);
        //        string newPath = Path.Combine(newDir, textFileName).Replace('\\', '/');
        //        AssetDatabase.MoveAsset(oldPath, newPath);
        //    }
        //}
    }
}

