using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AssetLoadStatistician : Singleton<AssetLoadStatistician>
{
    string m_DumpFile = "FileLoadProfile.csv";
    List<string> m_DumpBuf = new List<string>();
    int m_BufLineNum = 10;

    void _DumpToFile()
    {
        if (m_DumpBuf.Count <= 0)
            return;

        FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, m_DumpFile), FileMode.Append, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();
        sw.BaseStream.Seek(0, SeekOrigin.End);
        for (int i = 0; i < m_DumpBuf.Count; ++i)
            sw.WriteLine(m_DumpBuf[i]);

        sw.Flush();
        sw.Close();
        fs.Close();
        m_DumpBuf.Clear();
    }

    void _RecordLoadFile(string info)
    {
        if (m_DumpBuf.Count >= m_BufLineNum)
            _DumpToFile();

        m_DumpBuf.Add(info);
    }

    public void AddAssetProfile(string fileName,float ms,bool bundle,bool async)
    {
        if (!Global.Settings.profileAssetLoad)
            return;

        _RecordLoadFile(string.Format("{0},{1},{2},{3},{4}", DateTime.Now.ToString("hh:mm:ss:fff"), fileName, ms.ToString("0.000"), bundle ? "AssetBundle" : "Resource", async ? "Async" : "Sync"));
    }
}
