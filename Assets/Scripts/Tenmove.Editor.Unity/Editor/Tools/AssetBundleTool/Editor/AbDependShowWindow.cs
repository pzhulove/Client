using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Tenmove.Editor.Unity.Widgets;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
    public class AbDependShowWindow : EditorWindow
{
    #region Params
    private const string EndFlag = "**endOnce";
    private const string UnityBuiltinFlag = "unity_builtin_extra";
    private static string _cacheFilePath;
    private static string _abRootPath;
    private Dictionary<string, AbDependData> _abDependDic = new Dictionary<string, AbDependData>();
    private Dictionary<string, List<string>> _abBeDependDic = new Dictionary<string, List<string>>();
    #endregion

    #region Params_GUI
    private string _lastSelect;
    private float _topHeight = 45;
    private List<TreeViewItem> _allAbItem;
    private List<TreeViewItem> _selectAbDepend;
    private List<TreeViewItem> _selectAbBeDepend;
    private static AbDependTreeView _myAllAbTree;
    private static AbDependTreeView _selectDependTree;
    private static AbDependTreeView _selectBeDependTree;
    private static TMSearchField _myAllAbSearch;
    private static TMSearchField _myDependSearch;
    private static TMSearchField _myBeDependSearch;
    #endregion

    #region Init

#if UNITY_5 || UNITY_2017
    [MenuItem("[TM工具集]/资源规范相关/Ab Depend Show")]
#endif
    public static void AbDependShow()
    {
        AbDependShowWindow window = GetWindow<AbDependShowWindow>();
        window.titleContent = new GUIContent("显示Ab依赖关系");
        window.Show();
    }
    #endregion

    #region Mono
    private void OnEnable()
    {
        _allAbItem = new List<TreeViewItem>();
        _selectAbDepend = new List<TreeViewItem>();
        _selectAbBeDepend = new List<TreeViewItem>();
        _myAllAbSearch = new TMSearchField(this);
        _myDependSearch = new TMSearchField(this);
        _myAllAbTree = new AbDependTreeView(new TreeViewState());
        _myBeDependSearch = new TMSearchField(this);
        SetAllAbTreeEvents();
        _selectDependTree = new AbDependTreeView(new TreeViewState()) { CurSelectDoubleClick = TreeDoubleClickHandle };
        _selectBeDependTree = new AbDependTreeView(new TreeViewState()) { CurSelectDoubleClick = TreeDoubleClickHandle };

        string dataPath = Application.dataPath;
        int count = 2;
        int i;
        for (i = dataPath.Length - 1; i >= 0; i--)
        {
            if (dataPath[i] == '/')
                count--;
            if (count == 0)
                break;
        }
        string abDependToolPath = (i > 0 ? dataPath.Substring(0, i) : dataPath) + "/ExternalTool/AbDependTool";
        if (!Directory.Exists(abDependToolPath))
            _cacheFilePath = Application.dataPath;
        else
            _cacheFilePath = abDependToolPath;
    }
    // ReSharper disable once InconsistentNaming
    private void OnGUI()
    {
        // TopGUI
        GUILayout.BeginArea(new Rect(0, 0, position.width, _topHeight));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("加载缓存文件: ", GUILayout.Width(80));
        EditorGUILayout.TextField(_cacheFilePath);
        if (GUILayout.Button("AnalysisFile", GUILayout.Width(100)))
        {
            _cacheFilePath = EditorUtility.OpenFilePanel("Open File Dialog", _cacheFilePath, "");
            ShowAbDependFile();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ab包路径: ", GUILayout.Width(80));
        EditorGUILayout.TextField(_abRootPath);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
            _abRootPath = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/StreamingAssets/AssetBundles", "");
        if (GUILayout.Button("LoadAllBundle", GUILayout.Width(150)))
        {
            AnalysisAllAbAsset();
        }
        EditorGUILayout.EndHorizontal();
        float treeViewWidth = position.width * 0.5f;
        GUILayout.EndArea();
        // LeftGUI
        GUILayout.BeginArea(new Rect(0, _topHeight, treeViewWidth, position.height - _topHeight));
        EditorGUILayout.LabelField("所有Ab列表: ");
        GUILayout.BeginArea(new Rect(0, 20, treeViewWidth, 20), EditorStyles.toolbar);
        if (_myAllAbSearch.OnToolbarGUI())
        {
            if (_lastSelect != "" && _myAllAbSearch.text == "")
                _myAllAbTree.SetFocusAndEnsureSelectedItem();
            _myAllAbTree.searchString = _myAllAbSearch.text;
            _lastSelect = _myAllAbTree.searchString;
        }
        GUILayout.EndArea();
        _myAllAbTree.OnGUI(new Rect(0, 40, treeViewWidth, position.height - _topHeight - 40));
        GUILayout.EndArea();
        // RightGUI1
        GUILayout.BeginArea(new Rect(treeViewWidth, _topHeight, treeViewWidth, (position.height - _topHeight) * 0.5f));
        EditorGUILayout.LabelField("选中资源依赖Ab: ");
        GUILayout.BeginArea(new Rect(0, 20, treeViewWidth, 20), EditorStyles.toolbar);
        if (_myDependSearch.OnToolbarGUI())
        {
            _selectDependTree.searchString = _myDependSearch.text;
        }
        GUILayout.EndArea();
        _selectDependTree.OnGUI(new Rect(0, 40, treeViewWidth, (position.height - _topHeight) * 0.5f - 40));
        GUILayout.EndArea();
        // RightGUI2
        GUILayout.BeginArea(new Rect(treeViewWidth, (position.height + _topHeight) * 0.5f, treeViewWidth,
            (position.height - _topHeight) * 0.5f));
        EditorGUILayout.LabelField("依赖选中资源Ab: ");
        GUILayout.BeginArea(new Rect(0, 20, treeViewWidth, 20), EditorStyles.toolbar);
        if (_myBeDependSearch.OnToolbarGUI())
        {
            _selectBeDependTree.searchString = _myBeDependSearch.text;
        }
        GUILayout.EndArea();
        _selectBeDependTree.OnGUI(new Rect(0, 40, treeViewWidth, (position.height - _topHeight) * 0.5f - 40));
        GUILayout.EndArea();
    }
    #endregion

    #region Func
    private void ShowAbDependFile()
    {
        if (!File.Exists(_cacheFilePath))
            return;
        if (!_cacheFilePath.EndsWith(".txt"))
            return;
        _abDependDic.Clear();
        using (StreamReader sr = new StreamReader(File.Open(_cacheFilePath, FileMode.Open)))
        {
            string curLine;
            while ((curLine = sr.ReadLine()) != null)
            {
                if (curLine.StartsWith("--") || curLine.StartsWith("**"))
                    continue;
                AbDependData abData = new AbDependData
                {
                    AbPath = curLine.Substring(curLine.IndexOf("Assets", StringComparison.Ordinal)),
                    AbName = Path.GetFileName(curLine),
                    ChildObj = new List<AbChildObjRefData>()
                };
                while ((curLine = sr.ReadLine()) != EndFlag)
                {
                    if (curLine == null)
                        break;
                    string[] childInfo = curLine.Substring(2).Split(' ');
                    if (childInfo.Length == 0)
                        continue;
                    AbChildObjRefData childData = new AbChildObjRefData { ObjName = childInfo[0] };
                    if (childInfo.Length > 1)
                    {
                        int index = childInfo[1].IndexOf("Assets", StringComparison.Ordinal);
                        childData.DependAbPath = index < 0 ? childInfo[1] : childInfo[1].Substring(index);
                        List<string> beDepend;
                        if (!_abBeDependDic.TryGetValue(childData.DependAbPath, out beDepend))
                        {
                            beDepend = new List<string>();
                            _abBeDependDic.Add(childData.DependAbPath, beDepend);
                        }
                        beDepend.Add(abData.AbPath);
                    }
                    if (childInfo.Length > 2)
                        childData.DependObjName = childInfo[2];
                    abData.ChildObj.Add(childData);
                }
                _abDependDic.Add(abData.AbPath, abData);
            }

            int count = 1;
            _allAbItem.Clear();
            foreach (KeyValuePair<string, AbDependData> abDependData in _abDependDic)
            {
                TreeViewItem fatherItem = new TreeViewItem { depth = 0, displayName = abDependData.Key, id = count };
                _allAbItem.Add(fatherItem);
                count++;
                foreach (AbChildObjRefData childData in abDependData.Value.ChildObj)
                {
                    TreeViewItem child = new TreeViewItem { depth = 1, displayName = childData.ObjName, id = count };
                    _allAbItem.Add(child);
                    count++;
                }
            }
            _myAllAbTree.SetTreeViewData(_allAbItem);
        }
    }
    private void AnalysisAllAbAsset()
    {
        if (string.IsNullOrEmpty(_abRootPath))
            return;
        if (!Directory.Exists(_abRootPath))
            return;
        string dataPath = Application.dataPath;
        int count = 2;
        int i;
        for (i = dataPath.Length - 1; i >= 0; i--)
        {
            if (dataPath[i] == '/')
                count--;
            if (count == 0)
                break;
        }
        string externalToolParentPath = i > 0 ? dataPath.Substring(0, i) : dataPath;
        string exePath = externalToolParentPath + "/ExternalTool/AbDependTool/MyAssetsStudio.exe";
        if (!File.Exists(exePath))
            return;
        string logPath = externalToolParentPath + "/ExternalTool/AbDependTool/abDependResult.txt";
        try
        {
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = exePath,
                    Arguments = string.Format("{0} {1}", _abRootPath, logPath)
                },
                EnableRaisingEvents = true,
            };
            process.Start();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        if (File.Exists(logPath))
        {
            _cacheFilePath = logPath;
            ShowAbDependFile();
        }

        #region 单个文件分析_代码调用
        //string writePath = @"E:\MyJob\920\Program\Client\Assets\StreamingAssets\Test\Temp\AbDepend.txt";
        //AbDependAnalysis abDepend = new AbDependAnalysis();
        //LoadAssetBundle loadAb = new LoadAssetBundle();
        //abDepend.WritePath = writePath;
        //loadAb.SetAbDependAnalysis(abDepend);
        //loadAb.LoadFolder(_abRootPath);
        //abDepend.GenerateAbDependFile();
        //if (File.Exists(writePath))
        //{
        //    _cacheFilePath = writePath;
        //    AnalysisAbRefFile();
        //}
        #endregion
    }
    #endregion

    #region Func_Evnets
    private void SetAllAbTreeEvents()
    {
        _myAllAbTree.AssetSelect = (abPathList, objNameList) =>
        {
            _selectAbDepend.Clear();
            int count = 1;
            // 将依赖项解析为以依赖ab名为key的字典，若选中Ab子项，则仅匹配子项引用
            Dictionary<string, List<string>> dependAbDic = new Dictionary<string, List<string>>();
            for (int i = 0; i < abPathList.Count; i++)
            {
                string abPath = abPathList[i];
                string objName = objNameList[i];
                foreach (AbChildObjRefData childData in _abDependDic[abPath].ChildObj)
                {
                    if (childData.DependAbPath == null)
                        continue;
                    List<string> dependInfo;
                    if (!dependAbDic.TryGetValue(childData.DependAbPath, out dependInfo))
                        dependInfo = new List<string>();
                    bool needAdd = false;// 针对未加入过，且匹配成功的依赖才会加入
                    if (objName == null)
                    {
                        if (childData.DependObjName != null)
                        {
                            dependInfo.Add(childData.ObjName + "->" + childData.DependObjName);
                            needAdd = true;
                        }
                    }
                    else
                    {
                        if (childData.DependObjName != null && childData.ObjName == objName)
                        {
                            dependInfo.Add(childData.ObjName + "->" + childData.DependObjName);
                            needAdd = true;
                        }
                    }
                    if (needAdd && dependInfo.Count == 1)// 未加入过且需加入
                    {
                        dependAbDic.Add(childData.DependAbPath, dependInfo);
                    }
                }
            }

            foreach (KeyValuePair<string, List<string>> dependAb in dependAbDic)
            {
                TreeViewItem item = new TreeViewItem() { id = count, depth = 0, displayName = dependAb.Key };
                _selectAbDepend.Add(item);
                count++;
                foreach (string dependObjName in dependAb.Value)
                {
                    TreeViewItem itemChild = new TreeViewItem() { id = count, depth = 1, displayName = dependObjName };
                    _selectAbDepend.Add(itemChild);
                    count++;
                }
            }
            _selectDependTree.SetTreeViewData(_selectAbDepend);

            _selectAbBeDepend.Clear();
            int count2 = 1;

            for (int i = 0; i < abPathList.Count; i++)
            {
                string abPath = abPathList[i];
                List<string> beDepend;
                if (_abBeDependDic.TryGetValue(abPath, out beDepend))
                {
                    foreach (string beDependName in _abBeDependDic[abPath])
                    {
                        TreeViewItem item = _selectAbBeDepend.Find(p => p.displayName == beDependName);
                        if (item != null)
                            continue;  // 此处可以引用+1
                        TreeViewItem beDependItem = new TreeViewItem() { id = count2, depth = 0, displayName = beDependName };
                        _selectAbBeDepend.Add(beDependItem);
                        count2++;
                        AbDependData dependData = _abDependDic[beDependName];
                        foreach (AbChildObjRefData childData in dependData.ChildObj)
                        {
                            if (childData.DependAbPath != null && childData.DependAbPath == abPath &&
                                childData.DependObjName != null)
                            {
                                TreeViewItem childItem = new TreeViewItem() { id = count2, depth = 1, displayName = childData.ObjName };
                                _selectAbBeDepend.Add(childItem);
                                count2++;
                            }
                        }
                    }
                }
            }
            _selectBeDependTree.SetTreeViewData(_selectAbBeDepend);
        };
        _myAllAbTree.CurSelectDoubleClick = TreeDoubleClickHandle;
    }
    private void TreeDoubleClickHandle(string abName)
    {
        if (!abName.StartsWith("Assets"))
            return;
        string filePath = Application.dataPath + abName.Substring(6);
        if (File.Exists(filePath))
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(abName);
    }
    #endregion
}



public class AbDependData
{
    public string AbPath;
    public string AbName;  // 存在
    public List<AbChildObjRefData> ChildObj;
    public List<string> BeDependList;
    public AbDependData()
    {
        ChildObj = new List<AbChildObjRefData>();
        BeDependList = new List<string>();
    }
}

public class AbChildObjRefData
{
    public string ObjName;  // 存在
    public string DependAbPath;  // 空 unity内置 存在且载入 存在但未载入
    public string DependObjName;  // 存在  不存在
}