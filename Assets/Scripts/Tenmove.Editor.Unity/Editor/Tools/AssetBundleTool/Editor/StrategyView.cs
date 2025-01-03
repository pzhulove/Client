using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace AssetBundleTool
{
    public static class StrategyView 
    {
        public static string sm_strategyPath = Application.dataPath + "/Scripts/Tenmove.Editor.Unity/Editor/Tools/AssetBundleTool/PackStrategy.xml";//策略XML存储位置
        
        
        //策略列表
        public static List<StrategyDataBase> sm_strategyDataList {
            get {
                return BuildPackage.sm_strategyDataList;
            }
            set {
                BuildPackage.sm_strategyDataList = value;
            }
        }
        public static StrategyDataBase sm_selectStrategyData;//当前选择的策略
        public static Dictionary<long, List<AssetPackageDesc>> sm_strategyDataDic
        {
            get {
                return BuildPackage.sm_StrategyDataDic;
            }
        }//策略Bundel信息
        public static string sm_selectStrategy = typeof(StrategyByDirectoryLevel).FullName;
        private static string sm_selectBundleName = "";//当前选择的Bundel
        private static List<string> sm_logConsoleStr;//打印日志
        [SerializeField] static  TreeViewState m_TreeViewState;
        // The TreeView is not serializable it should be reconstructed from the tree data.
        private static LogTreeView m_TreeView;
        private static SearchField m_SearchField;
        private static bool sm_isConsole = false;
        private static Rect position;
        public static void OnInit() {
            BuildPackage.InitStrategy();
            if (sm_strategyDataList.Count > 0)
            {
                sm_selectStrategyData = sm_strategyDataList[0];
                foreach (var i in sm_strategyDataList)
                {
                    i.Init();
                }
                sm_selectStrategyData.LoadSetting();
            }

            sm_logConsoleStr = new List<string>();
            #region  初始化打印面板数据
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            m_TreeView = new LogTreeView(m_TreeViewState);
            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            #endregion

        }


        public static void OnDraw(Rect pos)
        {
            position = pos;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f), GUILayout.Height(position.height * 0.5f));
            {
                GUILayout.Space(5f);
                _PackBundleFolder();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space(5f);
                _PackBundleStrategy();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                var back = GUI.backgroundColor;
                GUI.backgroundColor = sm_isConsole ? back : Color.blue;
                //deToggleButtonStyle.normal.textColor = isConsole ? Color.white : Color.blue;
                if (GUILayout.Button("ShowBundleDetailInfo", "toolbarButton", GUILayout.Width(150)))
                {
                    sm_isConsole = false;
                }
                GUI.backgroundColor = !sm_isConsole ? back : Color.blue;

                if (GUILayout.Button("LogConsole", "toolbarButton", GUILayout.Width(100)))
                {
                    sm_isConsole = true;
                }
                GUI.backgroundColor = back;
                EditorGUILayout.EndHorizontal();
                if (sm_isConsole)
                {
                    _LogConsoleInfo();
                }
                else
                {
                    _BundleDisplayView();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            {
                if (!sm_isConsole)
                    _ShowBundleDetailInfo();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        static Vector2 sm_bundleFolderV2 = Vector2.zero;
        /// <summary>
        /// 设置打包目录文件
        /// </summary>
        private static void _PackBundleFolder()
        {
            EditorGUILayout.LabelField("SelectFolder", EditorStyles.boldLabel);

            sm_bundleFolderV2=GUILayout.BeginScrollView(sm_bundleFolderV2, GUILayout.Width(position.width * 0.5f), GUILayout.Height(position.height * 0.5f - 60));
            EditorGUILayout.BeginVertical("box");
            if (sm_strategyDataList != null)
            {
                for (int i = 0; i < sm_strategyDataList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    sm_strategyDataList[i].enable = GUILayout.Toggle(sm_strategyDataList[i].enable, "开启", GUILayout.Width(50));

                    Color originColor = GUI.backgroundColor;
                    Color contentColor = GUI.contentColor;

                    if (!sm_strategyDataList[i].enable)
                    {
                        GUI.backgroundColor = Color.gray;
                        GUI.contentColor = Color.gray;
                    }

                    if (sm_selectStrategyData != null && sm_selectStrategyData == sm_strategyDataList[i])
                    {
                        GUIStyle style = new GUIStyle();
                        style = GUI.skin.GetStyle("box");
                        style.alignment = TextAnchor.MiddleLeft;
                        style.normal.textColor = Color.yellow;
                        style.clipping = TextClipping.Overflow;
                        GUILayout.Label(sm_strategyDataList[i].strategyPath, style, GUILayout.Height(20), GUILayout.Width(position.width * 0.5f - 250));
                    }
                    else
                    {
                        if (GUILayout.Button(sm_strategyDataList[i].strategyPath, "label", GUILayout.Height(20), GUILayout.Width(position.width * 0.5f -250)))
                        {
                            sm_selectStrategyData = sm_strategyDataList[i];
                            sm_selectStrategyData.LoadSetting();
                            if (sm_selectStrategyData.defaultBool) {
                                sm_selectStrategyData.defaultBool=false;
                            }
                        }
                    }
                    //sm_strategyDataList[i].m_EAssetType = (EAssetType)EditorGUILayout.EnumPopup(sm_strategyDataList[i].m_EAssetType, GUILayout.Width(60));
                    if (sm_strategyDataList[i].defaultBool)
                    {
                        GUIStyle style = new GUIStyle();
                        style = GUI.skin.GetStyle("box");
                        style.alignment = TextAnchor.MiddleLeft;
                        style.normal.textColor = Color.red;
                        style.clipping = TextClipping.Overflow;
                        GUILayout.Label("未定制的默认策略", style, GUILayout.Height(20), GUILayout.Width(100));
                    }


                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {

                        if (sm_selectStrategyData != null && sm_strategyDataDic.ContainsKey(sm_selectStrategyData.strategyId))
                        {
                            sm_strategyDataDic.Remove(sm_selectStrategyData.strategyId);
                        }
                        if (sm_selectStrategyData != null && sm_selectStrategyData == sm_strategyDataList[i])
                        {
                            sm_selectStrategyData = null;
                        }
                        sm_strategyDataList.RemoveAt(i);
                        i--;
                    }

                    GUI.backgroundColor = originColor;
                    GUI.contentColor = contentColor;
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
           
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Path", GUILayout.Width(100), GUILayout.Height(20)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Assets Folder", Application.dataPath + "/Resources", "");
                if (path != null && path.Contains((Application.dataPath + "/Resources")))
                {
                    string tempPath = path.Substring(Application.dataPath.Length - 6);
                    CheckAndAddPath(tempPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "请选中Resources下面的目录", "确认");
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("保存策略", GUILayout.Width(100), GUILayout.Height(20)))
            {
                SaveStrategy();
            }

            EditorGUILayout.EndHorizontal();
        }

        public static void CheckAndAddPath(string tempPath)
        {
            bool hasContain = false;
            for (int i = 0; i < sm_strategyDataList.Count; i++)
            {
                if (sm_strategyDataList[i].strategyPath.Equals(tempPath))
                {
                    hasContain = true;
                    break;
                }
            }

            StrategyByFileData data = new StrategyByFileData();
            data.strategyPath = tempPath;
            data.strategyId = SmallFunc.GetTimeStamp();
            if (hasContain)
            {
                data.bundleExtensionName = data.strategyId.ToString();
            }
            sm_strategyDataList.Add(data);
            sm_selectStrategyData = data;
            sm_selectStrategyData.Init();
            sm_selectStrategyData.LoadSetting();
        }

        static StrategyBase sm_currentStrategy;
        static StrategyDataBase sm_prevDataBase;
        /// <summary>
        /// 打包策略
        /// </summary>
        private static void _PackBundleStrategy()
        {
            if (sm_selectStrategyData == null)
            {
                return;
            }
            //从列表选择对应策略，默认为层级文件夹打包
            if (sm_currentStrategy==null || sm_selectStrategyData.strateggy.Draw("选择策略:") || sm_prevDataBase!=sm_selectStrategyData)
            {
                sm_prevDataBase = sm_selectStrategyData;
                sm_currentStrategy = CreateInstance<StrategyBase>(sm_selectStrategyData.strateggy.m_TypeName);
            }
            sm_currentStrategy.OnDraw(position, sm_selectStrategyData);
        }

        /// <summary>
        /// 保存策略
        /// </summary>
        public static void SaveStrategy()
        {
            if (!File.Exists(sm_strategyPath))
            {
                FileStream fs = File.Create(sm_strategyPath);
                fs.Close();
                fs.Dispose();
            }
            if (SmallFunc.CreateXML(sm_strategyDataList, sm_strategyPath))
            {
                Debug.LogError("保存成功");
            }
            
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullName">命名空间.类型名</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string fullName)
        {
            string path = fullName;//命名空间.类型名,程序集
            Type o = Type.GetType(path);//加载类型
            object obj = Activator.CreateInstance(o, true);//根据类型创建实例
            return (T)obj;//类型转换并返回
        }

        static Vector2 sm_bundleDisplayV2 = Vector2.zero;
        /// <summary>
        /// bundle展示
        /// </summary>
        private static void _BundleDisplayView()
        {
            EditorGUILayout.BeginVertical("box");
            long bundelS = 0;
            sm_bundleDisplayV2=GUILayout.BeginScrollView(sm_bundleDisplayV2, GUILayout.Width(position.width * 0.5f), GUILayout.Height(position.height * 0.5f - 100));
            if (sm_selectStrategyData != null && sm_strategyDataDic != null && sm_strategyDataDic.ContainsKey(sm_selectStrategyData.strategyId))
            {
                List<AssetPackageDesc> selectPackageDescList = sm_strategyDataDic[sm_selectStrategyData.strategyId];
                if (selectPackageDescList != null)
                {
                    for (int i = 0; i < selectPackageDescList.Count; i++)
                    {
                        bundelS = bundelS + selectPackageDescList[i].m_OriginSize;
                        string bundleSize = Math.Round(selectPackageDescList[i].m_OriginSize / 1024.0f, 2) + "Kb";
                        if (sm_selectBundleName == selectPackageDescList[i].m_PackageName)
                        {
                            if (GUILayout.Button(selectPackageDescList[i].m_PackageName + "   " + bundleSize, "box", GUILayout.Height(20)))
                            {
                                sm_selectBundleName = selectPackageDescList[i].m_PackageName;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button(selectPackageDescList[i].m_PackageName + "   " + bundleSize, "label", GUILayout.Height(20)))
                                sm_selectBundleName = selectPackageDescList[i].m_PackageName;
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (sm_selectStrategyData != null && sm_strategyDataDic != null && sm_strategyDataDic.ContainsKey(sm_selectStrategyData.strategyId))
            {
                List<AssetPackageDesc> selectPackageDescList = sm_strategyDataDic[sm_selectStrategyData.strategyId];
                if (selectPackageDescList != null && selectPackageDescList.Count > 0)
                {
                    GUILayout.Label(string.Format("Bundel资源总大小 {0}Kb   Bundel总数量{1} ", Math.Round(bundelS / 1024.0f, 2), selectPackageDescList.Count));
                }
            }
        }

        /// <summary>
        /// log日志展示与保存
        /// </summary>
        private static void _LogConsoleInfo()
        {
            //GUILayout.BeginHorizontal(GUILayout.Width(position.width - 30), GUILayout.Height(position.height * 0.5f - 100));

            List<TreeViewItem> items = new List<TreeViewItem>();
            for (int i = 0; i < sm_logConsoleStr.Count; i++)
            {
                TreeViewItem item = new TreeViewItem { id = i, depth = 0, displayName = sm_logConsoleStr[i] };
                items.Add(item);
            }
            m_TreeView.SetTreeData(items);
            _DoToolbar();
            _DoTreeView();
            //GUILayout.EndVertical();
            GUILayout.Label(string.Format("总日志数  {0} ", sm_logConsoleStr.Count));
        }
        static void  _DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(100);
            GUILayout.FlexibleSpace();
            m_TreeView.searchString = m_SearchField.OnToolbarGUI(m_TreeView.searchString);
            GUILayout.EndHorizontal();
        }

        static void _DoTreeView()
        {
            Rect rect = GUILayoutUtility.GetRect(0, position.width - 30, 0, position.height * 0.5f - 100);
            m_TreeView.OnGUI(rect);
        }

        static Vector2 sm_bundleDetailInfo = Vector2.zero;
        /// <summary>
        /// 选中包的具体asset信息
        /// </summary>
        private static void _ShowBundleDetailInfo()
        {
            EditorGUILayout.BeginVertical("box");
            int assetNum = 0;//资源数量
            sm_bundleDetailInfo=GUILayout.BeginScrollView(sm_bundleDetailInfo, GUILayout.Width(position.width * 0.45f), GUILayout.Height(position.height * 0.5f - 100));
            if (sm_selectBundleName != null && sm_selectStrategyData != null && sm_strategyDataDic != null && sm_strategyDataDic.ContainsKey(sm_selectStrategyData.strategyId))
            {
                List<AssetPackageDesc> selectPackageDescList = sm_strategyDataDic[sm_selectStrategyData.strategyId];
                if (selectPackageDescList != null)
                {
                    for (int i = 0; i < selectPackageDescList.Count; i++)
                    {
                        if (sm_selectBundleName == selectPackageDescList[i].m_PackageName)
                        {
                            List<PackAssetDesc> m_PackageAsset = selectPackageDescList[i].m_PackageAsset;

                            for (int j = 0; j < m_PackageAsset.Count; j++)
                            {
                                string bundleSize = Math.Round(m_PackageAsset[j].m_OriginSize / 1024.0f, 2) + "Kb";
                                GUILayout.Label(m_PackageAsset[j].m_AssetPath + "    " + bundleSize);
                            }
                            assetNum = m_PackageAsset.Count;
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            if (assetNum != 0)
            {
                GUILayout.Label(string.Format("选中Bundel内所有资源总数  {0} ", assetNum));
            }
        }
        public static void OnLeave() {
        }
    }
}
