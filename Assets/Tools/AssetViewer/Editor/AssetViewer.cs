using UnityEngine;
using UnityEditor;
using GameClient;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class AssetViewer : EditorWindow
{
    static List<string> m_vecInfoList = new List<string>();
    Vector2 m_vScrollViewPos = Vector2.zero;

    [MenuItem("[打包工具]/Asset/Asset Viewer")]
    static void Init()
    {
        var window = GetWindow<AssetViewer>();
        window.position = new Rect(50, 50, 250, 60);
        window.Show();
    }

    string m_Filter = "";

    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Color originCol = GUI.color;
        GUI.color = Color.red;
        if (GUILayout.Button("强制GC"))
            AssetGabageCollector.instance.ClearUnusedAsset();
        EditorGUILayout.Space();

        GUI.color = Color.green;
        if (GUILayout.Button("抓取资源引用计数"))
            AssetLoader.instance.DumpAssetInfo(ref m_vecInfoList);

        GUI.color = Color.yellow;
        if (GUILayout.Button("导出资源信息到文件"))
        {
            string date = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
            FileStream streamW = new FileStream("AssetInfoDump_" + date + ".txt", FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));
            for (int i = 0; i < m_vecInfoList.Count; ++i)
            {
                EditorUtility.DisplayProgressBar("导出信息", "正在导出信息...", ((float)i / m_vecInfoList.Count));
                sw.WriteLine(m_vecInfoList[i]);
            }
            streamW.Flush();
            sw.Close();
            streamW.Close();
            EditorUtility.ClearProgressBar();
        }

        GUI.color = originCol;

        m_Filter = EditorGUILayout.TextField("筛选资源：", m_Filter);

        bool enableFilter = !string.IsNullOrEmpty(m_Filter);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal("ObjectFieldThumb");
        {
            m_vScrollViewPos = EditorGUILayout.BeginScrollView(m_vScrollViewPos);

            EditorGUILayout.LabelField("当前资源总数：" + m_vecInfoList.Count);
            for (int i = 0, icnt = m_vecInfoList.Count; i < icnt; ++i)
            {
                if (!string.IsNullOrEmpty(m_vecInfoList[i]))
                {
                    if(enableFilter && !m_vecInfoList[i].ToLower().Contains(m_Filter.ToLower()))
                        continue;

                    EditorGUILayout.LabelField(m_vecInfoList[i]);
                }
            }

            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndHorizontal();
    }
}


