using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

public class ResNameTool : Editor
{
    class SameNameResDesc
    {
        public string name;
        public List<string> resPathDetail = new List<string>();
    }

    static List<SameNameResDesc> m_sameResPathList = new List<SameNameResDesc>();

    static readonly string ASSET_PATH = "Assets/Resources/";

    [MenuItem("[TM工具集]/ArtTools/导出场景同名资源列表")]
    static void SelectSameNameRes()
    {
        string[] fileLst = Directory.GetFiles(ASSET_PATH, "*",SearchOption.AllDirectories);

        m_sameResPathList.Clear();
        for (int i = 0, icnt = fileLst.Length; i < icnt; ++i)
        {
            if(Path.GetExtension(fileLst[i]).Contains("meta"))
                continue;

            if (!Path.GetExtension(fileLst[i]).ToLower().Contains("prefab"))
                continue;

            _AddNameResDesc(fileLst[i]);
        }

        string date = DateTime.Now.ToString("yyyy_MM_dd");
        FileStream streamW = new FileStream("Assets/AssetInfoDump_" + date + ".txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));
        for (int i = 0,icnt = m_sameResPathList.Count; i < icnt; ++i)
        {
            if(m_sameResPathList[i].resPathDetail.Count < 2)
                continue;

            EditorUtility.DisplayProgressBar("正在统计", "正在写入统计信息...", ((float)i / icnt));
            sw.WriteLine(m_sameResPathList[i].name);
            for (int j = 0,jcnt = m_sameResPathList[i].resPathDetail.Count; j < jcnt; ++j)
                sw.WriteLine("- Resource Path:" + m_sameResPathList[i].resPathDetail[j]);
        }
        streamW.Flush();
        sw.Close();
        streamW.Close();
        EditorUtility.ClearProgressBar();
    }

    static protected void _AddNameResDesc(string assetPath)
    {
        string assetName = Path.GetFileName(assetPath).ToLower();
        for(int i = 0,icnt = m_sameResPathList.Count; i<icnt;++i)
        {
            SameNameResDesc cur = m_sameResPathList[i];
            if (cur.name == assetName)
            {
                cur.resPathDetail.Add(assetPath);
                break;
            }
        }

        SameNameResDesc newDesc = new SameNameResDesc();
        newDesc.name = assetName;
        newDesc.resPathDetail.Add(assetPath);

        m_sameResPathList.Add(newDesc);
    }
}
