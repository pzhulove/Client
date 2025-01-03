using UnityEngine;
using UnityEditor;
using GameClient;
using System;
using System.Collections;
using System.Collections.Generic;

public class AssetPackageViewer : EditorWindow
{
    static List<string> m_vecInfoList = new List<string>();
    Vector2 m_vScrollViewPos = Vector2.zero;

    [MenuItem("[打包工具]/Asset/Asset Package Viewer")]
    static void Init()
    {
        var window = GetWindow<AssetPackageViewer>();
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
        if (GUILayout.Button("抓取资源打包详情"))
            AssetPackageManager.instance.DumpAssetPackageInfo(ref m_vecInfoList);

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

                    EditorGUILayout.LabelField(m_vecInfoList[i], GUILayout.ExpandWidth(true));
                }
            }

            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndHorizontal();
    }
}


