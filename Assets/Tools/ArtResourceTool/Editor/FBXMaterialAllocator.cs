using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;

public class FBXMaterialAllocator : Editor
{
    class ReimportFileDesc
    {
        public ReimportFileDesc(string asset, int count, int itemID)
        {
            m_Asset = asset;
            m_Count = count;
            m_ItemID = itemID;
        }

        public int m_Count;
        public int m_ItemID;
        public string m_Asset;
    }

    class ReimportFileDescComparer : IComparer<ReimportFileDesc>
    {
        public int Compare(ReimportFileDesc x, ReimportFileDesc y)
        {
            return y.m_Count - x.m_Count;
        }
    }

    static List<ReimportFileDesc> m_ReimportFileDescList = new List<ReimportFileDesc>();

    //[MenuItem("Assets/Replace Standard Shader")]
    static public void ReplaceStandardShader()
    {
        m_ReimportFileDescList.Clear();
        FileStream file = File.OpenRead("Assets/Resources/reimporter.csv");
        using (StreamReader tr = new StreamReader(file))
        {
            while (!tr.EndOfStream)
            {
                string line = tr.ReadLine();
                string[] context = line.Split(',');
                if (context.Length == 2)
                {
                    int ID;
                    if (int.TryParse(context[0].Trim(), out ID))
                    {
                        int count;
                        if (int.TryParse(context[1].Trim(), out count))
                        {
                            m_ReimportFileDescList.Add(new ReimportFileDesc(null, count, ID));
                        }
                    }
                }
                tr.Close();
            }
        }
        file.Close();
        XlsxDataUnit itemtbl = XlsxDataManager.Instance().GetXlsxByName("ItemTable");
        XlsxDataUnit restbl = XlsxDataManager.Instance().GetXlsxByName("ResTable");
        if (null != itemtbl)
        {
            for (int j = 0,jcnt = m_ReimportFileDescList.Count;j<jcnt;++j)
            {
                ReimportFileDesc curDesc = m_ReimportFileDescList[j];
                bool hasFind = false;
                for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < itemtbl.RowCount; ++i)
                {
                    Dictionary<string, ICell> tbl = itemtbl.GetRowDataByLine(i);
                    ICell cellID = tbl["ID"];
                    if (cellID.CustomToInt() == curDesc.m_ItemID)
                    {
                        ICell cellRes = tbl["ResID"];
                        for (int k = XlsxDataUnit.XLS_DATA_INDEX; k < restbl.RowCount; ++k)
                        {
                            Dictionary<string, ICell> tblRes = restbl.GetRowDataByLine(k);
                            ICell cellResID = tblRes["ID"];
                            if(cellRes.CustomToInt() == cellResID.CustomToInt())
                            {
                                curDesc.m_Asset = tblRes["ModelPath"].CustomToString();
                                hasFind = true;
                                break;
                            }
                        }

                        if (hasFind)
                            break;
                    }
                }
            }
        }

        m_ReimportFileDescList.Sort(new ReimportFileDescComparer());

        FileStream fileW = File.OpenWrite("Assets/Resources/reimporter_dump.txt");
        using (StreamWriter tw = new StreamWriter(fileW))
        {
            for (int j = 0, jcnt = m_ReimportFileDescList.Count; j < jcnt; ++j)
            {
                ReimportFileDesc curDesc = m_ReimportFileDescList[j];
                if (string.IsNullOrEmpty(curDesc.m_Asset))
                    continue;

                tw.WriteLine(string.Format("{0},{1}", curDesc.m_Asset, curDesc.m_Count));
            }
            tw.Flush();
            tw.Close();
        }
        fileW.Close();
        int maxImportCount = 300;
        for (int i = 0, icnt = maxImportCount < m_ReimportFileDescList.Count ? maxImportCount : m_ReimportFileDescList.Count; i < icnt; ++i)
        {
            if (string.IsNullOrEmpty(m_ReimportFileDescList[i].m_Asset))
                continue;

            string asset = "Assets/Resources/" + Path.ChangeExtension(m_ReimportFileDescList[i].m_Asset, ".prefab");
            string[] dependency = AssetDatabase.GetDependencies(asset);

            for (int j = 0; j < dependency.Length; ++j)
            {
                if (dependency[j].EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                {
                    AssetDatabase.ImportAsset(dependency[j], ImportAssetOptions.ForceUpdate);
                    break;
                }
            }
        }
    }
}