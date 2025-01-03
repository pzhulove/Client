using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;

public class SubMeshChecker : Editor
{
    static string RES_PATH = "Assets/Resources/Actor";

    protected class SubMeshResDesc
    {
        public string m_ResPath;
        public int m_SubMesh;
    }
    static List<SubMeshResDesc> m_MeshDescList = new List<SubMeshResDesc>();

    [MenuItem("[TM工具集]/ArtTools/统计多材质网格")]
    static void StatisticSubMeshRes()
    {
        _Clear();

        string[] pathList = null;
        List<string> fileList = new List<string>();
        pathList = Directory.GetFiles(RES_PATH, "*.prefab", SearchOption.AllDirectories);
        fileList.AddRange(pathList);

        for (int i = 0; i < fileList.Count; ++i)
        {
            EditorUtility.DisplayProgressBar("检查网格资源", "正在检查第" + i + "个资源...", ((float)i / fileList.Count));
            GameObject curObject = AssetDatabase.LoadAssetAtPath<GameObject>(fileList[i]);
            if (null != curObject)
            {
                SubMeshResDesc kNewMeshResDesc = new SubMeshResDesc();
                kNewMeshResDesc.m_ResPath = fileList[i].Replace('\\', '/');
                kNewMeshResDesc.m_SubMesh = 0;
                Renderer mr = curObject.GetComponentInChildren<Renderer>();
                if(null != mr)
                    kNewMeshResDesc.m_SubMesh = mr.sharedMaterials.Length;

                Destroy(curObject);

                m_MeshDescList.Add(kNewMeshResDesc);
            }
        }

        /// Dump
        /// 
        string date = DateTime.Now.ToString("yyyy_MM_dd");
        FileStream streamW = new FileStream("MeshResStatistic_" + date + ".txt", FileMode.Create, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(streamW);

        sw.WriteLine(m_MeshDescList.Count.ToString() + " Effect Total");
        for (int i = 0; i < m_MeshDescList.Count; ++i)
        {
            SubMeshResDesc kCurMeshResDesc = m_MeshDescList[i];

            sw.WriteLine(i.ToString() + ":" + Path.GetFileName(kCurMeshResDesc.m_ResPath));
            sw.WriteLine("    - Sub Mesh Num:" + kCurMeshResDesc.m_SubMesh.ToString());
            sw.WriteLine("");
        }

        streamW.Flush();
        sw.Close();
        streamW.Close();

        streamW = new FileStream("MeshResStatistic_" + date + ".csv", FileMode.Create, FileAccess.Write, FileShare.Write);
        sw = new StreamWriter(streamW);

        sw.WriteLine("Prefab Name,Sub Mesh Render Num");
        for (int i = 0; i < m_MeshDescList.Count; ++i)
        {
            SubMeshResDesc kCurMeshResDesc = m_MeshDescList[i];

            string content = "";

            content += Path.GetFileName(kCurMeshResDesc.m_ResPath);
            content += ",";

            content += kCurMeshResDesc.m_SubMesh.ToString();
            sw.WriteLine(content);
        }

        streamW.Flush();
        sw.Close();
        streamW.Close();
        EditorUtility.ClearProgressBar();
    }

    static void _Clear()
    {
        m_MeshDescList.Clear();
    }
}
