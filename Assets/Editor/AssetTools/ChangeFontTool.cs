using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFontTool : EditorWindow
{
    private string m_FindRootDir;
    private string m_StrStartFontSize = "0";
    private string m_StrEndFontSize = "100";
    private int m_StartFontSize = 0;
    private int m_EndFontSize = 100;
    private Font m_TargetFont;
    private Vector2 m_ScrollPos;
    private bool m_IgnoreSameFont = false;
    private GameObject m_LastActiveGameObject = null;
    private List<GameObject> m_AllPrefabList = new List<GameObject>();
    private List<List<FontUseInfo>> m_AllPrefabUseFontInfoList = new List<List<FontUseInfo>>();
    private List<GameObject> m_FilterPrefabList = new List<GameObject>();
    private List<List<FontUseInfo>> m_FilterPrefabUseFontInfoList = new List<List<FontUseInfo>>();

    #region Init
    [MenuItem("[TM工具集]/ArtTools/新_字体替换工具")]
    private static void _NewChangeFontTool()
    {
        ChangeFontTool window = GetWindow<ChangeFontTool>();
        window.titleContent = new GUIContent("替换字体资源");
        window.Show();
    }
    #endregion

    #region GUI Method
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("建议处理路径: Resources/UIFlatten");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("替换路径: ", GUILayout.Width(80));
        EditorGUILayout.TextField(m_FindRootDir);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
            m_FindRootDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources/UIFlatten", "");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("目标字体: ", GUILayout.Width(80));
        m_TargetFont = EditorGUILayout.ObjectField(m_TargetFont, typeof(Font), true) as Font;
        bool last = m_IgnoreSameFont;
        m_IgnoreSameFont = EditorGUILayout.Toggle("忽略相同字体", m_IgnoreSameFont, GUILayout.Width(200));
        if (m_IgnoreSameFont != last)
            _UpdateFilterPrefabUseFontInfo();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("字体范围: ", GUILayout.Width(80));
        m_StrStartFontSize = EditorGUILayout.TextField(m_StrStartFontSize);
        EditorGUILayout.LabelField(" ~ ", GUILayout.Width(30));
        m_StrEndFontSize = EditorGUILayout.TextField(m_StrEndFontSize);
        if (GUILayout.Button("设置字体范围"))
        {
            _SetFontSizeRange();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("收集字体使用信息"))
        {
            _CollectFontUseInfo();
        }
        if (GUILayout.Button("一键替换为目标字体"))
        {
            _ReplaceAllFontToTargetFont();
        }
        if (GUILayout.Button("重置工具"))
        {
            _ResetTool();
        }
        _DrawGuiScrollView();
        EditorGUILayout.EndVertical();
    }

    private void _DrawGuiScrollView()
    {
        int filterCount = m_FilterPrefabList.Count;
        if (filterCount <= 0)
            return;
        if (Selection.activeGameObject != null)
        {
            _ChangeActiveGameObject(Selection.activeGameObject);
        }
        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
        for (int i = 0; i < filterCount; i++)
        {
            List<FontUseInfo> filterInfo = m_FilterPrefabUseFontInfoList[i];
            int infoCount = filterInfo.Count;
            for (int j = 0; j < infoCount; j++)
            {
                FontUseInfo info = filterInfo[j];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(info.UseComponent, info.ComponentType, true);
                EditorGUILayout.ObjectField(info.UseFont, typeof(Font), true);
                EditorGUILayout.LabelField(info.FontSize.ToString());
                if (GUILayout.Button("修改为目标字体"))
                {
                    if (info.UseFont != m_TargetFont)  // 此处与一键处理所有预制体存在代码冗余
                    {
                        if (info.ComponentType == typeof(Text))
                        {
                            Text text = info.UseComponent as Text;
                            text.font = m_TargetFont;
                            info.UseFont = m_TargetFont;
                            PrefabUtility.SaveAsPrefabAsset(info.Prefab, info.PrefabPath);
                            AssetDatabase.Refresh();
                            if (m_IgnoreSameFont)
                            {
                                filterInfo.RemoveAt(j);
                                infoCount--;
                                j--;
                            }
                        }
                        else if (info.ComponentType == typeof(TextMesh))
                        {
                            TextMesh textMesh = info.UseComponent as TextMesh;
                            textMesh.font = m_TargetFont;
                            info.UseFont = m_TargetFont;
                            PrefabUtility.SaveAsPrefabAsset(info.Prefab, info.PrefabPath);
                            AssetDatabase.Refresh();
                            if (m_IgnoreSameFont)
                            {
                                filterInfo.RemoveAt(j);
                                infoCount--;
                                j--;
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            if (m_IgnoreSameFont && filterInfo.Count == 0)
            {
                m_FilterPrefabList.RemoveAt(i);
                m_FilterPrefabUseFontInfoList.RemoveAt(i);
                filterCount--;
                i--;
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void _ChangeActiveGameObject(GameObject obj)
    {
        if (obj == m_LastActiveGameObject)
            return;
        GameObject nearPrefabObj = PrefabUtility.GetNearestPrefabInstanceRoot(obj);
        if (nearPrefabObj == m_LastActiveGameObject)
            return;
        if (m_AllPrefabList.Contains(nearPrefabObj))
        {
            int prefabCount = m_AllPrefabList.Count;
            for (int i = 0; i < prefabCount; i++)
            {
                m_AllPrefabList[i].SetActive(nearPrefabObj.GetInstanceID() == m_AllPrefabList[i].GetInstanceID());
            }
            m_LastActiveGameObject = nearPrefabObj;
        }
    }
    #endregion

    #region GUI Button Click
    private void _SetFontSizeRange()
    {
        if (!int.TryParse(m_StrStartFontSize, out m_StartFontSize))
        {
            EditorUtility.DisplayDialog("字体大小有误", "最小字体应设置为整数", "知道啦！");
            m_StartFontSize = -1;
            return;
        }
        if (!int.TryParse(m_StrEndFontSize, out m_EndFontSize))
        {
            EditorUtility.DisplayDialog("字体大小有误", "最大字体应设置为整数", "知道啦！");
            m_EndFontSize = -1;
            return;
        }
        if (m_StartFontSize > m_EndFontSize)
        {
            EditorUtility.DisplayDialog("字体范围有误", "最小字体不应大于最大字体", "知道啦！");
            m_StartFontSize = -1;
            m_EndFontSize = -1;
            return;
        }
        _UpdateFilterPrefabUseFontInfo();
    }

    private void _CollectFontUseInfo()
    {
        if (string.IsNullOrEmpty(m_FindRootDir))
        {
            EditorUtility.DisplayDialog("路径有误", "请点击上方Open并选择欲处理目录", "知道啦！");
            return;
        }
        if (!Directory.Exists(m_FindRootDir))
        {
            EditorUtility.DisplayDialog("路径有误", "当前路径不存在，请重新选择", "知道啦！");
            return;
        }
        string[] searchFolder = new string[1] { m_FindRootDir.Substring(m_FindRootDir.IndexOf("Assets/")) };
        string[] prefabIds = AssetDatabase.FindAssets("t:prefab", searchFolder);
        int idCount = prefabIds.Length;
        List<Text> allTextList = new List<Text>();
        List<TextMesh> allTextMeshList = new List<TextMesh>();
        _ResetListInfo();
        for (int i = 0; i < idCount; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabIds[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            // 处理UI.Text
            prefab.GetComponentsInChildren(true, allTextList);
            // 处理TextMesh
            prefab.GetComponentsInChildren(true, allTextMeshList);
            int textCount = allTextList.Count;
            int textMeshCount = allTextMeshList.Count;
            if (textCount == 0 && textMeshCount == 0)
                continue;
            GameObject insPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            //GameObject insPrefab = prefab;
            int fontUseCount = textCount + textMeshCount;
            List<FontUseInfo> allFontUseInfo = new List<FontUseInfo>(fontUseCount);
            if (textCount > 0)
            {
                insPrefab.GetComponentsInChildren(true, allTextList);
                for (int j = 0; j < textCount; j++)
                {
                    allFontUseInfo.Add(new FontUseInfo
                    {
                        Prefab = insPrefab,
                        UseComponent = allTextList[j],
                        ComponentType = typeof(Text),
                        UseFont = allTextList[j].font,
                        FontSize = allTextList[j].fontSize,
                        PrefabPath = prefabPath,
                    });
                }
            }
            if (textMeshCount > 0)
            {
                insPrefab.GetComponentsInChildren(true, allTextMeshList);
                for (int j = 0; j < textCount; j++)
                {
                    allFontUseInfo.Add(new FontUseInfo
                    {
                        Prefab = insPrefab,
                        UseComponent = allTextMeshList[j],
                        ComponentType = typeof(Text),
                        UseFont = allTextMeshList[j].font,
                        FontSize = allTextMeshList[j].fontSize,
                        PrefabPath = prefabPath,
                    });
                }
            }
            m_AllPrefabList.Add(insPrefab);
            m_AllPrefabUseFontInfoList.Add(allFontUseInfo);
        }
        _UpdateFilterPrefabUseFontInfo();
    }

    private void _ReplaceAllFontToTargetFont()
    {
        int filterCount = m_FilterPrefabList.Count;
        if (filterCount == 0)
            return;
        for (int i = 0; i < filterCount; i++)
        {
            List<FontUseInfo> filterInfo = m_FilterPrefabUseFontInfoList[i];
            int infoCount = filterInfo.Count;
            bool haveChanged = false;
            string prefabPath = null;
            for (int j = 0; j < infoCount; j++)
            {
                FontUseInfo info = filterInfo[j];
                if (info.UseFont != m_TargetFont)
                {
                    if (info.ComponentType == typeof(Text))
                    {
                        Text text = info.UseComponent as Text;
                        text.font = m_TargetFont;
                        info.UseFont = m_TargetFont;
                        if (m_IgnoreSameFont)
                        {
                            filterInfo.RemoveAt(j);
                            infoCount--;
                            j--;
                        }
                        if (!haveChanged)
                        {
                            haveChanged = true;
                            prefabPath = info.PrefabPath;
                        }
                    }
                    else if (info.ComponentType == typeof(TextMesh))
                    {
                        TextMesh textMesh = info.UseComponent as TextMesh;
                        textMesh.font = m_TargetFont;
                        info.UseFont = m_TargetFont;
                        if (m_IgnoreSameFont)
                        {
                            filterInfo.RemoveAt(j);
                            infoCount--;
                            j--;
                        }
                        if (!haveChanged)
                        {
                            haveChanged = true;
                            prefabPath = info.PrefabPath;
                        }
                    }
                }
            }// for j

            if (haveChanged)
            {
                PrefabUtility.SaveAsPrefabAsset(m_FilterPrefabList[i], prefabPath);
                AssetDatabase.Refresh();
            }
            if (m_IgnoreSameFont && filterInfo.Count == 0)
            {
                m_FilterPrefabList.RemoveAt(i);
                m_FilterPrefabUseFontInfoList.RemoveAt(i);
                filterCount--;
                i--;
            }
        }
    }

    private void _ResetTool()
    {
        m_FindRootDir = "";
        m_StrStartFontSize = "0";
        m_StrEndFontSize = "100";
        m_StartFontSize = 0;
        m_EndFontSize = 100;
        m_TargetFont = null;
        m_ScrollPos = Vector2.zero;
        m_IgnoreSameFont = false;
        m_LastActiveGameObject = null;
        _ResetListInfo();
    }

    private void _ResetListInfo()
    {
        int allPrefabCount = m_AllPrefabList.Count;
        if (allPrefabCount > 0)
        {
            for (int i = 0; i < allPrefabCount; i++)
            {
                DestroyImmediate(m_AllPrefabList[i]);
            }
        }
        m_AllPrefabList.Clear();
        m_AllPrefabUseFontInfoList.Clear();
        m_FilterPrefabList.Clear();
        m_FilterPrefabUseFontInfoList.Clear();
        m_ScrollPos = Vector2.zero;
    }
    #endregion

    #region Common Method
    private void _UpdateFilterPrefabUseFontInfo()
    {
        int allPrefabCount = m_AllPrefabList.Count;
        if (allPrefabCount == 0)
            return;
        m_FilterPrefabList.Clear();
        m_FilterPrefabUseFontInfoList.Clear();
        for (int i = 0; i < allPrefabCount; i++)
        {
            List<FontUseInfo> filterFontUseInfo = new List<FontUseInfo>();
            int alreadyAddCount = 0;
            foreach (FontUseInfo info in m_AllPrefabUseFontInfoList[i])
            {
                if (_IsComponentFontAtRange(info.FontSize))
                {
                    if (!m_IgnoreSameFont)
                    {
                        filterFontUseInfo.Add(info);
                        alreadyAddCount++;
                    }
                    else if (m_IgnoreSameFont && info.UseFont != m_TargetFont)
                    {
                        filterFontUseInfo.Add(info);
                        alreadyAddCount++;
                    }
                }
            }
            if (alreadyAddCount > 0)
            {
                m_FilterPrefabList.Add(m_AllPrefabList[i]);
                m_FilterPrefabUseFontInfoList.Add(filterFontUseInfo);
            }
        }
    }

    private bool _IsComponentFontAtRange(int fontSize)
    {
        if (m_StartFontSize < 0 || m_EndFontSize <= 0)
            return false;
        if (fontSize >= m_StartFontSize && fontSize <= m_EndFontSize)
            return true;
        return false;
    }
    #endregion

    private class FontUseInfo
    {
        public GameObject Prefab;
        public Component UseComponent;
        public Type ComponentType;
        public Font UseFont;
        public int FontSize;
        public string PrefabPath;
    }
}