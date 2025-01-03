using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.IO;
using System.Collections;

namespace CleanAssetsTool
{
    /// <summary>
    /// 清理无用资源的工具，也可以用来检查资源引用关系
    /// 工具实现的原理：
    /// 1：收集指定目录（默认Assets/Resources）下所有资源，建立起资源表，并根据Unity的AssetDatabase.GetDependencies接口建立各资源显式的依赖连接（只统计直接依赖，不recursive）。
    /// 2：建立资源间隐式的依赖关系。两种情况：一是Prefab和ScriptableObject上脚本中的string类型引用的资源路径。二是TextAsset如json文件（不包括MonoScript）中的资源路径。
    /// 3：上面两步就建立起项目中所有资源的相互依赖关系了。下面统计Root资源，Root资源是指引用资源路径的表格和代码资源。分别从表格和代码中统计资源路径，建立起Root资源和其他资源的引用关系。
    /// 4：有了以上的关系后，就可以决定出每个资源的状态：直接被Root资源引用，间接被Root资源引用，没有被Root资源直接或间接引用。
    /// 5：决定出每个资源的状态后。没有被Root资源直接或间接引用的资源就是可以删除的资源。
    /// 
    /// 项目中的特殊处理：
    /// 1：1.0项目中表格中同一个cell中可能有多个路径，是用'|'分割的。
    /// 2：1.0项目中SkillData的引用在表格中是"Data/SkillData/APC/Huozhimonvtala"这样的一个路径，在程序中进行了处理，
    ///    合成一个"Data/SkillData/APC/Huozhimonvtala/Huozhimonvtala_FileList.json"这样的路径，这个json文件中记录了使用的SkillData文件。
    /// 3: 1.0和2.0技能配置文件保存的格式不同，导致分析需要做不同的处理
    /// </summary>
    public class CleanAssetsEditor : EditorWindow
    {
        public enum ProjectionVersion
        {
            // 1.0
            One,
            // 2.0
            Two
        }

        public enum TreeType
        {
            RootAssetTree = 0,
            UsedAssetTree,
            UnUsedAssetTree,
            DeleteAssetTree,

            TreeTypeNum
        };

        public const ProjectionVersion projectionVersion = ProjectionVersion.Two;

        private string m_RootAssetDir = "";                         // 根资源目录
        private List<string> m_SingleRootAsset = new List<string>();// 单独的根资源
        private string m_AssetDir = "Assets/Resources";             // 检测资源目录

        private Vector2 m_RootAssetScrollPos = new Vector2();
        private AssetTreeViewGroup[] m_AssetTreeViews = new AssetTreeViewGroup[(int)TreeType.TreeTypeNum];
        private AssetGraph m_AssetGraph = new AssetGraph();
        private string m_selectPureAsset;
        private string[] m_AssetTreeViewNames = new string[]
        {
            "根资源，{0}个：",

            "使用中的资源（被根资源直接或间接引用），{0}个：",
            "未使用的资源，{0}个：",
            "准备删除的资源，{0}个：",
        };

        // 需要检查的纯资源类型，以及它是否可能引用其他资源
        private Dictionary<Type, bool> m_PureAssetType = new Dictionary<Type, bool>
        {
            {typeof(Material), true},
        //    {typeof(Shader), false},
            {typeof(Texture), false},
        //    {typeof(AudioClip), false},
        };

        [MenuItem("[TM工具集]/资源规范相关/资源引用关系查找")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            CleanAssetsEditor window = (CleanAssetsEditor)EditorWindow.GetWindow(typeof(CleanAssetsEditor));
            window.titleContent = new GUIContent("资源引用关系");
            window.Show();
        }

        public static void InitExternal(string rootDir, string assetDir)
        {
            CleanAssetsEditor window = (CleanAssetsEditor)EditorWindow.GetWindow(typeof(CleanAssetsEditor));
            window.titleContent = new GUIContent("资源引用关系");
            window.Show();

            window.m_RootAssetDir = rootDir;
            window.m_AssetDir = assetDir;
        }

        private void OnEnable()
        {
            for(int i = 0; i < (int)TreeType.TreeTypeNum; ++i)
            {
                m_AssetTreeViews[i] = new AssetTreeViewGroup(this, i, OnFilterChanged);
                m_AssetTreeViews[i].ColorChange = (i == (int)TreeType.RootAssetTree) ? Color.yellow : Color.red;

                if (i == (int)TreeType.UsedAssetTree || i == (int)TreeType.UnUsedAssetTree || i == (int)TreeType.DeleteAssetTree)
                {
                    m_AssetTreeViews[i].SetTreeViewClickCallback(SelPureAssetChange, SelPureAssetDoubleClick);

                    m_AssetTreeViews[i].AddAssetFilterType(false, "Prefab", typeof(PrefabAsset));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "ScriptableObject", typeof(ScriptableAsset));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "文本", typeof(TextAsset));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "贴图", typeof(Texture));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "材质", typeof(Material));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "Shader", typeof(Shader));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "动画", typeof(AnimationClip));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "声音", typeof(AudioClip));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "脚本", typeof(MonoScript));

                    m_AssetTreeViews[i].AddAssetFilterType(false, "其他", null, AssetFilter.AssetMask.Other);
                    m_AssetTreeViews[i].AddAssetFilterType(true,  "所有", null, AssetFilter.AssetMask.All);
                }
                else if (i == (int)TreeType.RootAssetTree)
                {
                    m_AssetTreeViews[i].SetTreeViewClickCallback(SelXlsAssetChange, SelXlsAssetDoubleClick);

                    m_AssetTreeViews[i].AddAssetFilterType(false, "表格", typeof(XlsAsset));
                    m_AssetTreeViews[i].AddAssetFilterType(false, "脚本", typeof(ScriptAsset));
                    m_AssetTreeViews[i].AddAssetFilterType(true, "所有", null, AssetFilter.AssetMask.All);
                }
            }

            RefreshTreeViewList();

            EditorApplication.update += UpdateEditor;
        }

        private void OnDisable() 
        {
            AssetGraph.Instance = null;
            EditorApplication.update -= UpdateEditor;
        }

        private bool m_Errored = false;
        private Stack<IEnumerator> m_Coroutines = new Stack<IEnumerator>();
        private void UpdateEditor()
        {
            if(m_Coroutines.Count > 0)
            {
                try
                { 
                    _UpdateCoroutine(m_Coroutines.Peek());
                }
                catch (Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("", "检查发生错误，联系程序", "确认");
                    m_Coroutines.Clear();

                    throw e;
                }
            }
        }

        private void _UpdateCoroutine(IEnumerator _coroutine)
        {
            bool bRet = _coroutine.MoveNext();
            if (!bRet)
            {
                m_Coroutines.Pop();
                return;
            }

            var current = _coroutine.Current;
            if(current is IEnumerator)
            {
                m_Coroutines.Push(current as IEnumerator);
            }
        }

        private IEnumerator StartAnalyse()
        {
            ScriptAnalysis.Clear();
            WarningWindow.ClearError();

            m_AssetGraph.ClearGraph();
            m_AssetGraph.RootAssetDir = m_RootAssetDir;
            m_AssetGraph.SingleRootAssets = m_SingleRootAsset;
            m_AssetGraph.TargetAssetDir = m_AssetDir;

            // 收集根资源
            if (!string.IsNullOrEmpty(m_RootAssetDir))
                yield return m_AssetGraph.CollecRootAssets(m_RootAssetDir, new ProgressRange(0, 0.3f));
            foreach(var singleRootAsset in m_SingleRootAsset)
            {
                yield return m_AssetGraph.AddSingleRootAssets(singleRootAsset);
            }

            yield return m_AssetGraph.AnalyseXlsDependent(new ProgressRange(0, 0.2f));
            yield return m_AssetGraph.AnalyseScriptDependent(new ProgressRange(0.2f, 0.4f));
            
            // 收集待检测根资源
            yield return m_AssetGraph.CollecNonRootAssets(m_AssetDir, new ProgressRange(0.4f, 0.65f));

            // 收集Prefab上脚本，ScriptableObject以及TextAsset中的字符串方式依赖
            yield return m_AssetGraph.AnalyseImplicitDependency(new ProgressRange(0.65f, 0.95f));

            yield return m_AssetGraph.UpdateTreeStatus(new ProgressRange(0.95f, 1.0f));

            EditorUtility.ClearProgressBar();

            RefreshTreeViewList();

            if (WarningWindow.HasWarning())
            {
                WarningWindow.ShowWarningWindow();
            }

            EditorUtility.DisplayDialog("", "检查完成", "确认");

/*
            if (false)
            {
                m_AssetGraph.CollecAllAssetAndAnalyseExplicitDependency(m_AssetDir, new ProgressRange(0, 0.4f));
                m_AssetGraph.AnalyseImplicitDependency(new ProgressRange(0.4f, 0.6f));
                m_AssetGraph.AnalyseXlsDependent(new ProgressRange(0.6f, 0.8f));

                m_AssetGraph.AnalyseScriptDependent(new ProgressRange(0.8f, 0.95f));
                m_AssetGraph.UpdateTreeStatus(new ProgressRange(0.95f, 1.0f));
            }*/
        }


        void OnGUI()
        { 
            float xStart = 150;
            float yStart = 0;

            GUILayout.BeginArea(new Rect(0, 0, xStart, position.height));

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("选择根资源目录"))
            {
                m_RootAssetDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources", "");
                int pos = m_RootAssetDir.IndexOf("Assets/Resources");
                if (pos > 0)
                    m_RootAssetDir = m_RootAssetDir.Substring(pos, m_RootAssetDir.Length - pos);
            }
            EditorGUILayout.TextField(m_RootAssetDir);


            if (GUILayout.Button("添加单独的根资源"))
            {
                string newRootAsset = EditorUtility.OpenFilePanel("Open File Dialog", Application.dataPath + "/Resources", "");
                int pos = newRootAsset.IndexOf("Assets/Resources");
                if (pos > 0)
                {
                    m_SingleRootAsset.Add(newRootAsset.Substring(pos, newRootAsset.Length - pos));
                }
            }
            for (int i = 0;i < m_SingleRootAsset.Count;i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextField(m_SingleRootAsset[i]);
                if(GUILayout.Button("移除"))
                {
                    m_SingleRootAsset.RemoveAt(i);
                    break;
                }
                GUILayout.EndHorizontal();
            }


            if (GUILayout.Button("选择检测资源目录)"))
            {
                m_AssetDir = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources", "");
                int pos = m_AssetDir.IndexOf("Assets/Resources");
                if (pos > 0)
                    m_AssetDir = m_AssetDir.Substring(pos, m_AssetDir.Length - pos);
            }
            EditorGUILayout.TextField(m_AssetDir);

            //EditorGUILayout.Space();
            if (GUILayout.Button("开始检查"))
            {
                m_Coroutines.Clear();
                m_Coroutines.Push(StartAnalyse());

                /*try
                {
                    ScriptAnalysis.Clear();
                    WarningWindow.ClearError();

                    m_AssetGraph.ClearGraph();
                    m_AssetGraph.RootAssetDir = m_RootAssetDir;
                    m_AssetGraph.TargetAssetDir = m_AssetDir;

                    if (true)
                    { 
                        // 收集根资源
                        if(string.IsNullOrEmpty(m_RootAssetDir))
                        {
                            m_AssetGraph.AnalyseXlsDependent(new ProgressRange(0, 0.2f));
                            m_AssetGraph.AnalyseScriptDependent(new ProgressRange(0.2f, 0.4f));
                        }
                        else
                        {
                            m_AssetGraph.CollecRootAssets(m_RootAssetDir, new ProgressRange(0, 0.3f));
                        }

                        // 收集待检测根资源
                        m_AssetGraph.CollecNonRootAssets(m_AssetDir, new ProgressRange(0.4f, 0.65f));

                        // 收集Prefab上脚本，ScriptableObject以及TextAsset中的字符串方式依赖
                        m_AssetGraph.AnalyseImplicitDependency(new ProgressRange(0.65f, 0.95f));

                        m_AssetGraph.UpdateTreeStatus(new ProgressRange(0.95f, 1.0f));
                    }

                }
                catch(Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("", "检查发生错误，联系程序", "确认");
                    throw e;
                }

                EditorUtility.ClearProgressBar();

                RefreshTreeViewList();

                if(WarningWindow.HasWarning())
                {
                    WarningWindow.ShowWarningWindow();
                }

                EditorUtility.DisplayDialog("", "检查完成", "确认");*/
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("导入资源关系图"))
            {
                string[] allAsset = AssetUtility.FindAllAssets("Assets/Resources");

                ImportAssetGraph();
                RefreshTreeViewList();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("保存资源关系图"))
            {
                ExportAssetGraph();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("显示Warning窗口"))
            {
                WarningWindow.ShowWarningWindow();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("显示MonoScript信息"))
            {
                ShowMonosciptInfo();
            }

            EditorGUILayout.EndVertical();

            GUILayout.EndArea();

            xStart += 10;

            m_AssetTreeViews[(int)TreeType.RootAssetTree].OnGUI(new Rect(xStart, yStart, position.width / 2 - xStart / 2, (position.height - yStart) * 1.0f / 3.0f));
            m_AssetTreeViews[(int)TreeType.UsedAssetTree].OnGUI(new Rect(xStart, yStart + (position.height - yStart) * 1.0f / 3.0f, position.width / 2 - xStart/2, (position.height - yStart) * 2.0f / 3.0f));
            m_AssetTreeViews[(int)TreeType.UnUsedAssetTree].OnGUI(new Rect(position.width / 2 + xStart / 2 + 10, yStart, position.width / 2 - xStart / 2 - 10, position.height/2 - yStart - 30));
            m_AssetTreeViews[(int)TreeType.DeleteAssetTree].OnGUI(new Rect(position.width / 2 + xStart / 2 + 10, position.height / 2, position.width / 2 - xStart / 2 - 10, position.height/2));
       
            GUILayout.BeginArea(new Rect(position.width / 2 + xStart / 2 + 10, position.height/2 - yStart - 30, position.width / 2 - xStart / 2 - 10, 30));
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("添加该资源到删除列表"))
            {
                AddSelToDeleteList(false);
            }
            if(GUILayout.Button("添加该资源及相关依赖资源到删除列表"))
            {
                AddSelToDeleteList(true);
            }
            if(GUILayout.Button("从删除列表中移除"))
            {
                RemoveFromDeleteList();
            }
            if(GUILayout.Button("删除删除列表中的资源"))
            {
                DeleteListAssets();
            }
            if (GUILayout.Button("提交SVN"))
            {
                CommitSVN();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public string GetSelectedAsset(CleanAssetsEditor.TreeType treeType)
        {
            return m_AssetTreeViews[(int)treeType].GetSelected();
        }

        public void AddSelToDeleteList(bool includeDep)
        {
            string delAssetName = m_AssetTreeViews[(int)TreeType.UnUsedAssetTree].GetSelected();
            if(string.IsNullOrEmpty(delAssetName))
            {
                EditorUtility.DisplayDialog("", "你需要先在未使用列表中选中一个资源", "确认");
                return;
            }

            BaseAsset asset = m_AssetGraph.GetAssetInfo(delAssetName);
            if(asset != null)
            {
                if(asset.DependentByRoot)
                {
                    EditorUtility.DisplayDialog("", "你不能删除一个被表格或代码引用的资源\n若真需要删除，请先删除对它的引用", "确认");
                    return; 
                }

                string lowerName = asset.Name.ToLower();
                if(lowerName.EndsWith(".cs") || lowerName.EndsWith(".dll"))
                {
                    EditorUtility.DisplayDialog("", "你不能删除一个脚本或者DLL", "确认");
                    return; 
                }

                if(!includeDep)
                {
                    foreach(var parent in asset.m_Parents)
                    {
                        BaseAsset parentAsset = m_AssetGraph.GetAssetInfo(parent);
                        if(!parentAsset.MarkAsDelete)
                        {
                            EditorUtility.DisplayDialog("", "你需要先删除依赖它的资源，或者使用\n\"添加该资源及相关依赖资源到删除列表\"方式", "确认");
                            return; 
                        }
                    }
                }
                
                asset.MarkAsDelete = true;

                // 如果删除与该资源相关的未使用资源，则同时删除该资源的Child资源以及Parent资源。
                if(includeDep)
                {
                    asset.IterateParents(true, AddParentToDeleteList);

                    HashSet<string> uniqueChild = asset.GetUniqueChild();
                    foreach(var child in uniqueChild)
                    {
                        AddToDeleteList(m_AssetGraph.GetAssetInfo(child));
                    }
                }

                RefreshTreeViewList();
            }
        }

        private bool AddParentToDeleteList(BaseAsset baseAsset)
        {
            if(!baseAsset.DependentByRoot && !baseAsset.IsRoot)
            {
                string lowerName = baseAsset.Name.ToLower();
                if(lowerName.EndsWith(".cs") || lowerName.EndsWith(".dll"))
                {
                    return true; 
                }

                // 将该资源标记为删除，同时删除其UniqueChild
                baseAsset.MarkAsDelete = true;

                /*
                HashSet<string> uniqueChild = baseAsset.GetUniqueChild();
                foreach(var child in uniqueChild)
                {
                    AddToDeleteList(m_AssetGraph.GetAssetInfo(child));
                }
                 */
            }

            return true;
        }

        private bool AddToDeleteList(BaseAsset baseAsset)
        {
            if(!baseAsset.DependentByRoot && !baseAsset.IsRoot)
            {
                string lowerName = baseAsset.Name.ToLower();
                if(lowerName.EndsWith(".cs") || lowerName.EndsWith(".dll"))
                {
                    return true; 
                }

                baseAsset.MarkAsDelete = true;
            }

            return true;
        }

        private void RemoveFromDeleteList()
        {
            string delAssetName = m_AssetTreeViews[(int)TreeType.DeleteAssetTree].GetSelected();

            if(string.IsNullOrEmpty(delAssetName))
            {
                EditorUtility.DisplayDialog("", "你需要先在删除列表中选中一个资源", "确认");
                return;
            }

            BaseAsset asset = m_AssetGraph.GetAssetInfo(delAssetName);
            if(asset != null)
            {
                asset.MarkAsDelete = false;
                asset.IterateChildren(true, childAsset => 
                    {
                        childAsset.MarkAsDelete = false;
                        return true;
                    }
                );
            }

            RefreshTreeViewList();
        }

        private void DeleteListAssets()
        {
            string fileName = EditorUtility.SaveFilePanel("导出删除文件备份", AssetUtility.m_RecentSavePath, "", "unitypackage");
            if (fileName.Length != 0)
            {
                AssetUtility.m_RecentSavePath = Path.GetDirectoryName(fileName);

                List<string> delList = new List<string>();

                try
                {
                    m_AssetGraph.GetAssetNames(TreeType.DeleteAssetTree, new AssetFilter.AssetFilterResult() { ShowAll = true }, delList, null);

                    // 导出unityPackage
                    {
                        AssetDatabase.ExportPackage(delList.ToArray(), fileName, ExportPackageOptions.Default);
                    }

                    // 保存删除文件列表和AssetGraph到文件
                    {
                        string delfileListName = Path.ChangeExtension(fileName, "txt");
                        using (FileStream file = new FileStream(delfileListName, FileMode.Create, FileAccess.Write))
                        {
                            StreamWriter streamWriter = new StreamWriter(file);

                            for (int i = 0, icnt = delList.Count; i < icnt; ++i)
                                streamWriter.WriteLine(delList[i]);

                            streamWriter.Flush();
                            streamWriter.Close();
                            file.Close();
                        }

                        string assetGraphName = Path.ChangeExtension(fileName,null);
                        assetGraphName += "_before.xml";
                        m_AssetGraph.SaveToXML(assetGraphName);
                    }

                    // 从AssetGraph中删除
                    {
                        for (int i = 0, icnt = delList.Count; i < icnt; ++i)
                        {
                            m_AssetGraph.RemoveAssetFromGraph(delList[i]);
                        }

                        string assetGraphName = Path.ChangeExtension(fileName, null);
                        assetGraphName += "_after.xml";
                        m_AssetGraph.SaveToXML(assetGraphName);
                    }
                }
                catch
                {
                    EditorUtility.DisplayDialog("", "删除发生错误，联系程序。\n放心，实际资源并没有删除。", "确认");
                    throw;
                }

                // 从Unity工程中删除
                {
                    for (int i = 0, icnt = delList.Count; i < icnt; ++i)
                    {
                        AssetDatabase.DeleteAsset(delList[i]);
                    }

                    AssetDatabase.Refresh();
                }

                RefreshTreeViewList();

                if(EditorUtility.DisplayDialog("", "删除成功。可通过Import Package菜单\n重新导入删除的资源。若需要将删除提交SVN，\n选择 提交， 也可以之后再提交。", "提交", "不提交"))
                {
                    CommitDelAssetToSVN(delList);
                }
            }
        }

        private void CommitDelAssetToSVN(List<string> delList)
        {
            if (delList.Count == 0)
                return;

            string prefix = Application.dataPath;
            int pos = prefix.LastIndexOf("/Assets");
            if(pos < 0)
            {
                pos = prefix.LastIndexOf("\\Assets");
            }

            if (pos > 0)
            {
                prefix = prefix.Substring(0, pos + 1);
            }

            string commitFileNames = "";
            string logName = "Bug(提交资源清理，删除资源)：提交资源清理，删除资源!!!!!!!!!!";

            for (int i = 0; i < delList.Count; ++i)
            {
                commitFileNames += prefix + delList[i];
                commitFileNames += "*" + prefix + delList[i] + ".meta";

                if(i != delList.Count - 1)
                {
                    commitFileNames += "*";
                }
            }

            SvnTool.SvnCommit(commitFileNames, logName);
        }

        private void CommitSVN()
        {
            List<string> delList = new List<string>();
            string fileName = EditorUtility.OpenFilePanelWithFilters("选择删除资源列表文件", AssetUtility.m_RecentSavePath, new string[] { "TXT", "txt" });
            if (fileName.Length != 0)
            {
                using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    StreamReader streamReader = new StreamReader(file);

                    while (!streamReader.EndOfStream)
                    {
                        string delName = streamReader.ReadLine();
                        delList.Add(delName);
                    }
                    
                    streamReader.Close();
                    file.Close();
                }

                CommitDelAssetToSVN(delList);
            }
        }

        private void ShowMonosciptInfo()
        {
            BaseAsset asset = m_AssetGraph.GetAssetInfo(m_selectPureAsset) as BaseAsset;
            if(asset != null && asset.AssetType == typeof(MonoScript))
            {
                ScriptInfoWindow.ShowScriptInfoWindow(m_selectPureAsset);
            }
        }

        public void SetSelected(string assetName)
        {
            for(int i = 0; i < (int)TreeType.TreeTypeNum; ++i)
            {
                if(m_AssetTreeViews[i] != null && m_AssetTreeViews[i].HasAsset(assetName))
                    m_AssetTreeViews[i].SetSelected(assetName);
            }
        }

        private void ImportAssetGraph()
        {
            string fileName = EditorUtility.OpenFilePanelWithFilters("导入文件", AssetUtility.m_RecentSavePath, new string[] { "XML", "xml" });
            if(fileName.Length != 0)
            {
                m_AssetGraph.LoadFromXML(fileName);
                AssetUtility.m_RecentSavePath = Path.GetDirectoryName(fileName);
            }
        }

        private void ExportAssetGraph()
        {
            string fileName = EditorUtility.SaveFilePanel("导出到文件", AssetUtility.m_RecentSavePath, "", "xml");
            m_AssetGraph.SaveToXML(fileName);
            AssetUtility.m_RecentSavePath = Path.GetDirectoryName(fileName);
        }

        private void RefreshTreeView(int treeType)
        {
            List<string> outList1 = new List<string>();
            List<string> outList2 = new List<string>();

            m_AssetGraph.GetAssetNames((TreeType)treeType, m_AssetTreeViews[treeType].GetFilterResult(), outList1, outList2);

            int colorChangeIndex = outList1.Count;
            List<string> allAssets = outList1;
            if(outList2.Count > 0)
            {
                allAssets.AddRange(outList2);
            }

            m_AssetTreeViews[treeType].SetTreeViewData(allAssets);
            m_AssetTreeViews[treeType].SetColorChangeIndex(colorChangeIndex);
            m_AssetTreeViews[treeType].TreeViewName = string.Format(m_AssetTreeViewNames[treeType], allAssets.Count);
        }

        public void RefreshTreeViewList()
        {
            for(int i = 0; i < (int)TreeType.TreeTypeNum; ++i)
            {
                RefreshTreeView(i);
            }

            m_selectPureAsset = "";
        }

        private void OnFilterChanged(int treeID, AssetFilter.AssetFilterResult filterResult)
        {
            RefreshTreeView(treeID);
        }

        private void SelXlsAssetChange(string assetName)
        {
            AssetUtility.SetSelectionInProjectWindow(assetName);
        }

        private void SelXlsAssetDoubleClick(string assetName)
        {
            AssetUtility.SetSelectionInProjectWindow(assetName);
            AssetRelationWindow.ShowRelationWindow(assetName, m_AssetGraph, this);
        }

        private void SelPureAssetChange(string assetName)
        {
            m_selectPureAsset = assetName;
            AssetUtility.SetSelectionInProjectWindow(assetName);
        }

        private void SelPureAssetDoubleClick(string assetName)
        {
            m_selectPureAsset = assetName;
            AssetUtility.SetSelectionInProjectWindow(assetName);

            AssetRelationWindow.ShowRelationWindow(assetName, m_AssetGraph, this);
        }
    }    
}