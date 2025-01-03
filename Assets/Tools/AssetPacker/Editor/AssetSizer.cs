using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;


using XUPorterJSON;
using System.Collections;

public class AssetSizer
{
    static public string[] m_PackCommands = new string[]
    {
        "Assets/Resources/Actor",
        "Assets/Resources/Effects",
        "Assets/Resources/UI",
        "Assets/Resources/Scene",
    };

    [MenuItem("[打包工具]/AssetSizer")]
    static public void MeasureAsset()
    {
        FileStream streamW = new FileStream("SizeOut.txt", FileMode.Create, FileAccess.Write, FileShare.Write);

        StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));
        for (int i = 0; i < m_PackCommands.Length; ++i)
        {
            sw.WriteLine(m_PackCommands[i] + ":");
            //             long cbSize = 0;
            //             string[] firstLvl = Directory.GetFiles(m_PackCommands[i], "*.*", SearchOption.AllDirectories);
            //             if(firstLvl.Length > 0)
            //             {
            //                 for(int j = 0; j < firstLvl.Length; ++ j)
            //                 {
            //                     string ext = PathUtil.GetExtension(firstLvl[j]);
            //                     if(ext == ".png" || ext == ".tga" || ext == ".jpg" || ext == ".bmp" || ext == ".fbx")
            //                         cbSize += FileUtil.GetFileBytes(firstLvl[j]);
            //                 }
            //             }
            // 
            //             sw.WriteLine(m_PackCommands[i] + ":" + "{0}MB\n", (float)cbSize / (1024 * 1024));

            long cbSize = _MeasureSubDir(m_PackCommands[i], 1, 0, sw);
            sw.WriteLine("{0}MB\n", (float)cbSize / (1024 * 1024));

        }
        streamW.Flush();

        sw.Close();
        streamW.Close();


        //////////////////////////////////////////////////////////////////////////

        //         ArrayList serverList = new ArrayList();
        // 
        //         serverList.Add("http://192.168.2.61/update/");
        //         serverList.Add("http://192.168.2.61/update/");
        // 
        //         string json = MiniJSON.jsonEncode(serverList);
        // 
        //         FileStream streamW_ = new FileStream("updateserver.json", FileMode.Create, FileAccess.Write, FileShare.Write);
        //         StreamWriter sw_ = new StreamWriter(streamW_, Encoding.GetEncoding(936));
        // 
        //         sw_.Write(json);
        // 
        //         streamW_.Flush();
        // 
        //         sw_.Close();
        //         streamW_.Close();

        //string[] shaderList = Directory.GetFiles("Assets/Resources/Shader", "*.shader", SearchOption.AllDirectories);
        //ArrayList shaderResList = new ArrayList();
        //
        //for(int i = 0; i < shaderList.Length; ++ i)
        //{
        //    shaderResList.Add(shaderList[i].Replace("Assets/Resources/", "").Replace('\\','/'));
        //}
        //
        //FileStream streamW_ = new FileStream(Path.Combine("Assets/Resources/Shader", "ShaderList.json"), FileMode.Create, FileAccess.Write, FileShare.Write);
        //StreamWriter sw_ = new StreamWriter(streamW_, Encoding.GetEncoding(936));
        //
        //sw_.Write(MiniJSON.jsonEncode(shaderResList));
        //
        //streamW_.Flush();
        //
        //sw_.Close();
        //streamW_.Close();
    }

    static public long _MeasureSubDir(string subDir, int dstLevel, int curLevel, StreamWriter sw)
    {
        subDir = subDir.Replace('\\', '/');
        string[] firstLvl = Directory.GetDirectories(subDir, "*.*", SearchOption.TopDirectoryOnly);

        long cbSize = 0;
        if (null != firstLvl)
        {
            for (int j = 0; j < firstLvl.Length; ++j)
            {
                if (dstLevel == curLevel)
                {
                    long cbSubSize = 0;
                    string[] subLvl = Directory.GetFiles(firstLvl[j], "*.*", SearchOption.AllDirectories);
                    if (subLvl.Length > 0)
                    {
                        for (int k = 0; k < subLvl.Length; ++k)
                        {
                            string ext = PathUtil.GetExtension(subLvl[k]);
                            if (ext == ".png" || ext == ".tga" || ext == ".jpg" || ext == ".bmp" || ext == ".fbx")
                                cbSubSize += FileUtil.GetFileBytes(subLvl[k]);
                        }
                    }

                    //sw.WriteLine(firstLvl[j] + ":" + "{0}MB\n", (float)cbSubSize / (1024 * 1024));
                    cbSize += cbSubSize;
                }
                else
                {
                    cbSize += _MeasureSubDir(firstLvl[j], dstLevel, curLevel + 1, sw);
                }
            }
        }

        if (dstLevel == curLevel)
        {
            string pad = "  ";
            int tabCnt = curLevel;
            while (0 == curLevel--)
                pad += "  ";

            sw.WriteLine(pad + subDir + ":" + "{0}MB\n", (float)cbSize / (1024 * 1024));
        }

        return cbSize;
    }


    static DAssetPackageDependency m_PackageDependencyDesc = null;

    class AssetSearchCompare : IComparer<DAssetPackageMapDesc>
    {
        public int Compare(DAssetPackageMapDesc x, DAssetPackageMapDesc y)
        {
            return x.assetPathKey.CompareTo(y.assetPathKey);
        }
    }
    static AssetSearchCompare SEARCH_COMPARE = new AssetSearchCompare();
    static DAssetPackageMapDesc DUMMY_MAPDESC = new DAssetPackageMapDesc();

    static DAssetPackageDesc _GetAssetPackageDesc(string resPath)
    {
        if (null != m_PackageDependencyDesc)
        {
            if (null != m_PackageDependencyDesc.assetDescPackageMap)
            {
                DUMMY_MAPDESC.assetPathKey = resPath.ToLower();
                int idx = m_PackageDependencyDesc.assetDescPackageMap.BinarySearch(DUMMY_MAPDESC, SEARCH_COMPARE);

                if (0 <= idx && idx < m_PackageDependencyDesc.assetDescPackageMap.Count)
                {
                    DAssetPackageMapDesc curDesc = m_PackageDependencyDesc.assetDescPackageMap[idx];
                    if (0 <= curDesc.packageDescIdx && curDesc.packageDescIdx < m_PackageDependencyDesc.packageDescArray.Length)
                    {
                        DAssetPackageDesc curPackageDesc = m_PackageDependencyDesc.packageDescArray[curDesc.packageDescIdx];
                        return curPackageDesc;
                    }
                }
            }
        }

        return null;
    }

    [MenuItem("[打包工具]/Measure Asset Size")]
    static public void MeasureAssetSize()
    { 
        FileStream streamR = new FileStream("AssetList.txt", FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(streamR);
        string buf = sr.ReadToEnd();
        sr.Close();
        streamR.Close();

        string[] lines = buf.Split(new char[] { '\n','\t'});

        List<string> pathList = new List<string>();
        if(null != lines)
        {
            for(int i = 0,icnt = lines.Length;i<icnt;++i)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                if ("-" == lines[i])  continue;

                string pathR = lines[i].Replace("\r", null);
                if (!pathList.Contains(pathR))
                    pathList.Add(pathR);
            }
        }

        List<string> allAssetsWithDepend = new List<string>();
        for (int i = 0, icnt = pathList.Count; i < icnt; ++i)
        {
            Object obj = AssetLoader.instance.LoadRes(pathList[i]).obj;
            if (null == obj) continue;

            string Path = AssetDatabase.GetAssetPath(obj);
            if(string.IsNullOrEmpty(Path))
            {
                if (obj is GameObject)
                {
                    pathList[i] = PathUtil.EraseExtension(pathList[i]);
                    Path = "Assets/Resources/" + pathList[i] + ".prefab";
                }
            }

            if (string.IsNullOrEmpty(Path))
                Debug.LogError("###################:" + pathList[i]);

            allAssetsWithDepend.Add(Path);
            allAssetsWithDepend.AddRange( AssetDatabase.GetDependencies(Path) );
        }

        List<string> allAssets = new List<string>();
        for (int i = 0, icnt = allAssetsWithDepend.Count; i < icnt; ++i)
        {
            string Path = allAssetsWithDepend[i];
            if (!allAssets.Contains(Path))
                allAssets.Add(Path);
        }

        long cbBytes = 0;

        FileStream asset_fs = new FileStream("Assets/StreamingAssets/AllAssetList.txt", FileMode.CreateNew, FileAccess.Write);
        StreamWriter asset_sw = new StreamWriter(asset_fs);
        asset_sw.Flush();
        asset_sw.BaseStream.Seek(0, SeekOrigin.End);

        for (int i = 0, icnt = allAssets.Count; i < icnt; ++i)
            asset_sw.WriteLine(allAssets[i]);
        asset_sw.Flush();
        asset_sw.Close();
        asset_fs.Close();

        for (int i = 0, icnt = allAssets.Count; i < icnt; ++i)
        {
            cbBytes += FileUtil.GetFileBytes(allAssets[i]);
        }

        long resMB = cbBytes / (1024 * 1024);

        long cbPackageBytes = 0;
        m_PackageDependencyDesc = AssetDatabase.LoadAssetAtPath<DAssetPackageDependency>("Assets/Resources/Base/Version/PackageInfo.asset") as DAssetPackageDependency;
        if(null != m_PackageDependencyDesc)
        {
            List<DAssetPackageDesc> packageListAll = new List<DAssetPackageDesc>();
            for(int i = 0,icnt = allAssets.Count;i<icnt;++i)
            {
                string ext = Path.GetExtension(allAssets[i]);
                if (ext.Equals(".meta", System.StringComparison.OrdinalIgnoreCase) || ext.Equals(".cs", System.StringComparison.OrdinalIgnoreCase)
                     || ext.Equals(".dll", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                string validPath = allAssets[i].Replace("Assets/Resources/", null).Replace('\\', '/');
                packageListAll.Add(_GetAssetPackageDesc(Path.ChangeExtension(validPath,null)));
            }

            List<string> packageList = new List<string>();
            for (int i = 0, icnt = packageListAll.Count; i < icnt; ++i)
            {
                DAssetPackageDesc curDesc = packageListAll[i];

                if(null == curDesc) continue;

                string packageFullPath = Path.Combine(curDesc.packagePath, curDesc.packageName).Replace('\\', '/');
                if (!packageList.Contains(packageFullPath))
                    packageList.Add(packageFullPath);
            }

            long packageBytes = 0;

            FileStream fs = new FileStream("FirstPackage.csv", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.End);

            for (int i = 0, icnt = packageList.Count; i < icnt; ++i)
            {
                long size = FileUtil.GetFileBytes(Path.Combine("Assets/StreamingAssets", packageList[i]));
                float sizeMB = size / (1024.0f * 1024.0f);
                packageBytes += size;

                sw.WriteLine(packageList[i] + "," + sizeMB.ToString("0.000"));
            }

            long packageMB = packageBytes / (1024 * 1024);
            sw.WriteLine((packageMB + 1).ToString());

            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
