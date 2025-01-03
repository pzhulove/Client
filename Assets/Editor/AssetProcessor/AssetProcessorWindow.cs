using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net;
using System.IO;
using System;

public class AssetProcessorWindow : EditorWindow
{
    #region Display Variables
    //Scroll
    private Vector2 AssetTypeScroll;
    private Vector2 StrategyScroll;
    private Vector2 IncludesScroll;
    private Vector2 IgnoresScroll;
    private Vector2 SettingScroll;

    //用来实现拖拽
    private Vector2 IncludeEventMouse = new Vector2(0, 0);
    private Vector2 ExcludeEventMouse = new Vector2(0, 0);
    private bool IncludeDragUpdate;
    private bool ExcludeDragUpdate;
    private int IncludeObjectPickerID = 10;
    private int ExcludeObjectPickerID = 11;

    private string stragyName;
    #endregion

    private AssetProcessProfile.AssetType m_DisplayType;

    private List<BaseAssetProcessorData> m_Strategies;
    public static BaseAssetProcessorData m_CurrentDisplayStrategy;

    // 用来Copy Paste路径
    public List<string> m_TempPaths;
    public List<string> m_TempFiles;

    private void OnEnable()
    {
        //启用AssetProcessor
        AssetProcessor.Enable = true;

        AssetProcessProfile.LoadAssetProfile();

        //先加载texture的第一个策略
        SwithToAssetType(AssetProcessProfile.AssetType.Texture);

        IncludeEventMouse = new Vector2(0, 0);
        ExcludeEventMouse = new Vector2(0, 0);
        IncludeDragUpdate = false;
        ExcludeDragUpdate = false;

        stragyName = string.Empty;
    }

    private void SwithToAssetType(AssetProcessProfile.AssetType assetType)
    {
        m_DisplayType = assetType;
        m_Strategies = AssetProcessProfile.GetAssetProfile(m_DisplayType);

        m_CurrentDisplayStrategy = m_Strategies.Count == 0 ? null : m_Strategies[0];
    }

    private void OnDisable()
    {
        //禁用AssetProcessor
        AssetProcessor.Enable = false;
    }

    [MenuItem("[TM工具集]/资源规范相关/资源导入规范设置")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<AssetProcessorWindow>(false, "AssetProcessor", true);
        window.minSize = new Vector2(1400f,800f);
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width), GUILayout.Height(position.height));
        {
            GUILayout.Space(2f);
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.15f));
            {
                GUILayout.Space(5f);
                //AssetType
                DrawAssetType();

                //Asset Strategy
                DrawAssetStrategy();
                
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(2f);
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.45f));
            {
                GUILayout.Space(5f);
                //Include Exclude
                DrawIncludeExclude();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(2f);
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.39f));
            {
                GUILayout.Space(5f);
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

                DrawSettings();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawAssetType()
    {
        //选择资源类型
        EditorGUILayout.LabelField("Asset Type", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height * 0.3f), GUILayout.Width(position.width * 0.18f));
        {
            AssetTypeScroll = EditorGUILayout.BeginScrollView(AssetTypeScroll);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUI.backgroundColor = m_DisplayType == AssetProcessProfile.AssetType.Texture ? Color.green : Color.white;
            if (GUILayout.Button("Texture2D", GUILayout.Width(100)))
            {
                SwithToAssetType(AssetProcessProfile.AssetType.Texture);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUI.backgroundColor = m_DisplayType == AssetProcessProfile.AssetType.Mesh ? Color.green : Color.white;
            if (GUILayout.Button("Mesh", GUILayout.Width(100)))
            {
                SwithToAssetType(AssetProcessProfile.AssetType.Mesh);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUI.backgroundColor = m_DisplayType == AssetProcessProfile.AssetType.Audio ? Color.green : Color.white;
            if (GUILayout.Button("Audio", GUILayout.Width(100)))
            {
                SwithToAssetType(AssetProcessProfile.AssetType.Audio);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUI.backgroundColor = m_DisplayType == AssetProcessProfile.AssetType.Animation ? Color.green : Color.white;
            if (GUILayout.Button("Animation", GUILayout.Width(100)))
            {
                SwithToAssetType(AssetProcessProfile.AssetType.Animation);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawAssetStrategy()
    {
        //选择策略
        EditorGUILayout.LabelField("Asset Strategy", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height * 0.63f), GUILayout.Width(position.width * 0.18f));
        {
            EditorGUILayout.BeginHorizontal();
            stragyName = GUILayout.TextField(stragyName, GUILayout.Width(100));
            if (GUILayout.Button("创建策略", GUILayout.Width(80)))
            {
                //创建策略
                if (stragyName != null && stragyName.Length > 0)
                {
                    bool repeat = false;
                    foreach (var strategy in m_Strategies)
                    {
                        if (strategy.StrategyName == stragyName)
                        {
                            repeat = true;
                            //弹出对话框 提示名字重复
                            EditorUtility.DisplayDialog("策略名字重复", "换个名字呗", "好的");
                            break;
                        }
                    }
                    if (repeat == false)
                    {
                        switch (m_DisplayType)
                        {
                            case AssetProcessProfile.AssetType.Texture:
                                var texProcessorData = new TextureProcessorData();
                                texProcessorData.StrategyName = stragyName;
                                m_Strategies.Add(texProcessorData);
                                break;
                            case AssetProcessProfile.AssetType.Mesh:
                                break;
                            case AssetProcessProfile.AssetType.Audio:
                                var audioProcessorData = new AudioProcessorData();
                                audioProcessorData.StrategyName = stragyName;
                                m_Strategies.Add(audioProcessorData);
                                break;
                            case AssetProcessProfile.AssetType.Animation:
                                break;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            StrategyScroll = GUILayout.BeginScrollView(StrategyScroll);
            {
                for (int i = 0; i < m_Strategies.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    if (m_CurrentDisplayStrategy == m_Strategies[i])
                    {
                        GUI.backgroundColor = Color.green;
                        if (GUILayout.Button(m_Strategies[i].StrategyName, GUILayout.Width(200)))
                        {
                            //切换到显示该策略
                            m_CurrentDisplayStrategy = m_Strategies[i];
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    else
                    {
                        if (GUILayout.Button(m_Strategies[i].StrategyName, GUILayout.Width(200)))
                        {
                            //切换到显示该策略
                            m_CurrentDisplayStrategy = m_Strategies[i];
                        }
                    }



                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("删除", GUILayout.Width(50)))
                    {
                        //删除该策略
                        //删除文件
                        //如果当前显示的是这个策略 释放它
                        if (m_CurrentDisplayStrategy == m_Strategies[i])
                        {
                            m_CurrentDisplayStrategy = null;
                        }
                        m_Strategies.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawIncludeExclude()
    {
        #region Includes
        EditorGUILayout.LabelField("Includes(包含资源或目录)", EditorStyles.boldLabel);

        //添加通过Object Picker选择的路径
        if (Event.current.commandName == "ObjectSelectorUpdated"
             && EditorGUIUtility.GetObjectPickerControlID() == IncludeObjectPickerID)
        {
            var path = AssetDatabase.GetAssetPath(EditorGUIUtility.GetObjectPickerObject());
            if (path != null && m_CurrentDisplayStrategy != null)
            {
                m_CurrentDisplayStrategy.AddPathOrFile(path, true);
                Repaint();
            }
        }
        if (Event.current.commandName == "ObjectSelectorUpdated"
            && EditorGUIUtility.GetObjectPickerControlID() == ExcludeObjectPickerID)
        {
            var path = AssetDatabase.GetAssetPath(EditorGUIUtility.GetObjectPickerObject());
            if (path != null && m_CurrentDisplayStrategy != null)
            {
                m_CurrentDisplayStrategy.AddPathOrFile(path, false);
                Repaint();
            }
        }

        using (var IncludePanel = new EditorGUILayout.VerticalScope("box", GUILayout.Height(position.height * 0.5f - 60)))
        {
            IncludesScroll = GUILayout.BeginScrollView(IncludesScroll, GUILayout.Height(position.height * 0.5f - 60));
            {
                if (m_CurrentDisplayStrategy != null)
                {
                    for (int i = 0; i < m_CurrentDisplayStrategy.IncludePaths.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(m_CurrentDisplayStrategy.IncludePaths[i]);
                        if (GUILayout.Button("删除", GUILayout.Width(50)))
                        {
                            m_CurrentDisplayStrategy.IncludePaths.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    for (int i = 0; i < m_CurrentDisplayStrategy.IncludeFiles.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(m_CurrentDisplayStrategy.IncludeFiles[i]);
                        if (GUILayout.Button("删除", GUILayout.Width(50)))
                        {
                            m_CurrentDisplayStrategy.IncludeFiles.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            Rect includeRect = new Rect(0, 0, IncludePanel.rect.width, IncludePanel.rect.height);
            if (Event.current.type == EventType.DragUpdated)
            {
                if (includeRect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
                IncludeEventMouse = Event.current.mousePosition;

                IncludeDragUpdate = true;
                ExcludeDragUpdate = false;
            }
            if (Event.current.type == EventType.DragExited && IncludeDragUpdate)
            {
                if (includeRect.Contains(IncludeEventMouse))
                {
                    foreach (var item in DragAndDrop.paths)
                    {
                        if (m_CurrentDisplayStrategy != null)
                        {
                            m_CurrentDisplayStrategy.AddPathOrFile(item, true);
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }


        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Add Path", GUILayout.Width(100), GUILayout.Height(20)))
            {
                var path = EditorUtility.OpenFolderPanel("Select Include Path", "", "");
                if (path != null)
                {
                    var assetFolderPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);
                    var index = path.IndexOf(assetFolderPath);
                    var length = assetFolderPath.Length;
                    path = path.Remove(index, length);
                    if (m_CurrentDisplayStrategy != null)
                    {
                        m_CurrentDisplayStrategy.AddPathOrFile(path, true);
                    }
                }

            }
            if (GUILayout.Button("Add File", GUILayout.Width(100), GUILayout.Height(20)))
            {
                EditorGUIUtility.ShowObjectPicker<Texture>(null, false, null, IncludeObjectPickerID);
            }
            if(GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if (m_CurrentDisplayStrategy != null)
                    m_CurrentDisplayStrategy.ClearIncludeOrExclude(true);
            }
            if (GUILayout.Button("Copy", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if(m_CurrentDisplayStrategy != null)
                {
                    m_TempFiles = m_CurrentDisplayStrategy.IncludeFiles;
                    m_TempPaths = m_CurrentDisplayStrategy.IncludePaths;
                }
            }
            if (GUILayout.Button("Paste", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if (m_TempFiles != null && m_TempPaths != null && m_CurrentDisplayStrategy != null)
                    m_CurrentDisplayStrategy.PasteIncludeOrExclude(m_TempPaths, m_TempFiles, true);
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Exclude
        EditorGUILayout.LabelField("Exclude(从Includes中排除目标文件或目录)", EditorStyles.boldLabel);
        using (var ExcludePanel = new EditorGUILayout.VerticalScope("box", GUILayout.Height(position.height * 0.5f - 60)))
        {
            IgnoresScroll = GUILayout.BeginScrollView(IgnoresScroll, GUILayout.Height(position.height * 0.5f - 60));
            {
                if (m_CurrentDisplayStrategy != null)
                {
                    for (int i = 0; i < m_CurrentDisplayStrategy.ExcludePaths.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(m_CurrentDisplayStrategy.ExcludePaths[i]);
                        if (GUILayout.Button("删除", GUILayout.Width(50)))
                        {
                            m_CurrentDisplayStrategy.ExcludePaths.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    for (int i = 0; i < m_CurrentDisplayStrategy.ExcludeFiles.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(m_CurrentDisplayStrategy.ExcludeFiles[i]);
                        if (GUILayout.Button("删除", GUILayout.Width(50)))
                        {
                            m_CurrentDisplayStrategy.ExcludeFiles.RemoveAt(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            Rect excludeRect = new Rect(0, 0, ExcludePanel.rect.width, ExcludePanel.rect.height);
            if (Event.current.type == EventType.DragUpdated)
            {
                if (excludeRect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
                ExcludeEventMouse = Event.current.mousePosition;

                ExcludeDragUpdate = true;
                IncludeDragUpdate = false;
            }
            if (Event.current.type == EventType.DragExited && ExcludeDragUpdate)
            {
                if (excludeRect.Contains(ExcludeEventMouse))
                {
                    foreach (var item in DragAndDrop.paths)
                    {
                        if (m_CurrentDisplayStrategy != null)
                        {
                            m_CurrentDisplayStrategy.AddPathOrFile(item, false);
                        }
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Add Path", GUILayout.Width(100), GUILayout.Height(20)))
            {
                var path = EditorUtility.OpenFolderPanel("Select Exclude Path", "", "");
                if (path != null)
                {
                    var assetFolderPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);
                    var index = path.IndexOf(assetFolderPath);
                    var length = assetFolderPath.Length;
                    path = path.Remove(index, length);
                    if (m_CurrentDisplayStrategy != null)
                    {
                        m_CurrentDisplayStrategy.AddPathOrFile(path, false);
                    }
                }
            }
            if (GUILayout.Button("Add File", GUILayout.Width(100), GUILayout.Height(20)))
            {
                EditorGUIUtility.ShowObjectPicker<Texture>(null, false, null, ExcludeObjectPickerID);
            }
            if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if (m_CurrentDisplayStrategy != null)
                    m_CurrentDisplayStrategy.ClearIncludeOrExclude(false);
            }
            if (GUILayout.Button("Copy", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if (m_CurrentDisplayStrategy != null)
                {
                    m_TempFiles = m_CurrentDisplayStrategy.ExcludeFiles;
                    m_TempPaths = m_CurrentDisplayStrategy.ExcludePaths;
                }
            }
            if (GUILayout.Button("Paste", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if (m_TempFiles != null && m_TempPaths != null && m_CurrentDisplayStrategy != null)
                    m_CurrentDisplayStrategy.PasteIncludeOrExclude(m_TempPaths, m_TempFiles, false);
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion
    }

    private void DrawSettings()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(position.width * 0.36f - 10), GUILayout.Height(position.height - 50));
        {
            SettingScroll = EditorGUILayout.BeginScrollView(SettingScroll);
            if (m_CurrentDisplayStrategy != null)
            {
                switch (m_DisplayType)
                {
                    case AssetProcessProfile.AssetType.Texture:
                        m_CurrentDisplayStrategy.DisplayAndChangeData();
                        break;
                    case AssetProcessProfile.AssetType.Mesh:
                        break;
                    case AssetProcessProfile.AssetType.Audio:
                        m_CurrentDisplayStrategy.DisplayAndChangeData();
                        break;
                    case AssetProcessProfile.AssetType.Animation:
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(20)))
            {
                /* if (m_CurrentDisplayStrategy != null)
                 {
                     m_CurrentDisplayStrategy.Save(m_DisplayType);
                 }*/

                AssetProcessProfile.SaveAssetProfile();

                ShowNotification(new GUIContent("保存成功"));
            }
            if (GUILayout.Button("Apply", GUILayout.Width(100), GUILayout.Height(20)))
            {
                AssetProcessProfile.SaveAssetProfile();

                if (m_CurrentDisplayStrategy != null)
                {
                    m_CurrentDisplayStrategy.ReImportAsset();
                }

                ShowNotification(new GUIContent("应用成功"));
            }

            GUILayout.Space(30);

            string buttonName = "提交Config到svn";
            if (m_CurrentDisplayStrategy != null)
                buttonName = string.Format("提交Config和\"{0}\"到svn", m_CurrentDisplayStrategy.StrategyName);

            if (GUILayout.Button(buttonName, GUILayout.Height(20)))
            {
                AssetProcessProfile.CommitConfigToSVN(m_CurrentDisplayStrategy);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
