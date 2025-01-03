using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace CleanAssetsTool
{
    public struct ProgressRange
    {
        public ProgressRange(float fStart, float fEnd)
        {
            Start = fStart;
            End = fEnd;
        }

        public float Step()
        {
            return End - Start;
        }

        public float Start;
        public float End;
    }

    public class BaseAsset
    {
        enum Mask
        {
            DirectDependentByRoot   = 1,
            IndirectDependentByRoot = 2,
            DependentByRoot = DirectDependentByRoot | IndirectDependentByRoot,
            MarkAsDelete = 16,
            DummyRoot = 32,
        };

        private AssetGraph m_Owner;

        public string m_Name;
        public int    m_uniqueID;
        public int    m_Mask;
        public Type   m_AssetType;
        public bool   m_IsRoot;
        public bool   m_IsTarget;

        // 所有依赖的资源，包括显式和隐式。
        public HashSet<string> m_Children = new HashSet<string>();

        // 直接依赖它（不管是显式还是隐式）的资源。
        public HashSet<string> m_Parents = new HashSet<string>();

        // 通过字符串隐式依赖的资源，单独记录一下
        public HashSet<string> m_ImplicitChildren = new HashSet<string>();

        public string Name
        {
            get { return m_Name; }
        }

        public bool IsRoot
        {
            get { return m_IsRoot; }
            set { m_IsRoot = value; }
        }

        public bool IsTarget
        {
            get { return m_IsTarget; }
            set { m_IsTarget = value; }
        }

        public Type AssetType
        {
            get { return m_AssetType; }
        }

        public bool DependentChecked
        {
            get;
            set;
        }

        public bool StatusUpdated
        {
            get;
            set;
        }

        /// <summary>
        /// 是否被根（表格或者代码）依赖。无论是直接还是间接。
        /// </summary>
        public bool DependentByRoot
        {
            get { return (m_Mask & (int)Mask.DependentByRoot) != 0; }
        }

        public bool DummyRoot
        {
            get { return (m_Mask & (int)Mask.DummyRoot) != 0; }
            set
            {
                if (value)
                    m_Mask |= (int)Mask.DummyRoot;
                else
                    m_Mask &= ~(int)Mask.DummyRoot;
            }
        }

        /// <summary>
        /// 是否被根（表格或者代码）直接依赖。
        /// </summary>
        public bool DirectDependentByRoot
        {
            get { return (m_Mask & (int)Mask.DirectDependentByRoot) != 0; }
            set
            {
                if(value)
                {
                    m_Mask |= (int)Mask.DirectDependentByRoot;
                    MarkAsDelete = false;
                }
                else
                {
                    m_Mask &= ~(int)Mask.DirectDependentByRoot;
                }
            }
        }

        /// <summary>
        /// 是否被根（表格或者代码）间接依赖。
        /// </summary>
        public bool InDirectDependentByRoot
        {
            get { return (m_Mask & (int)Mask.IndirectDependentByRoot) != 0; }
            set
            {
                if (value)
                {
                    m_Mask |= (int)Mask.IndirectDependentByRoot;
                    MarkAsDelete = false;
                }
                else
                {
                    m_Mask &= ~(int)Mask.IndirectDependentByRoot;
                }
            }
        }

        public bool MarkAsDelete
        {
            get { return (m_Mask & (int)Mask.MarkAsDelete) != 0; }
            set
            {
                if (value)
                {
                    if (DependentByRoot || IsRoot)
                    {
                        EditorUtility.DisplayDialog("", "该资源被表格或者代码引用，不能删除。\n确定不用，先手动解除引用，再删除。", "确认");
                        return;
                    }

                    m_Mask |= (int)Mask.MarkAsDelete;
                }
                else
                {
                    m_Mask &= ~(int)Mask.MarkAsDelete;
                }
            }
        }

        // 用于反序列化时创建对象
        public BaseAsset(AssetGraph owner)
        {
            m_Owner = owner;
        }

        public BaseAsset(string name, int uniqueID, AssetGraph owner, Type assetType)
        {
            m_Name = name;
            m_uniqueID = uniqueID;
            m_Owner = owner;
            m_AssetType = assetType;
        }

        // 添加一个依赖资源
        public void AddChild(string childName, bool bImplicit)
        {
            if (bImplicit)
            {
                if (m_Children.Add(childName))
                {
                    m_ImplicitChildren.Add(childName);
                }
            }
            else
            {
                m_Children.Add(childName);

                if (m_ImplicitChildren.Contains(childName))
                {
                    m_ImplicitChildren.Remove(childName);
                }
            }
        }

        public void AddParent(string parentName)
        {
            m_Parents.Add(parentName);
        }

        public void CheckDependent()
        {
            if (DependentChecked)
                return;

            string[] dependents = AssetUtility.GetDependencies(m_Name, false);

            for (int i = 0, iMax = dependents.Length; i < iMax; ++i)
            {
                if (!dependents[i].Equals(m_Name))
                {
                    m_Owner.LinkAsset(m_Name, dependents[i], false);
                }
            }

            DependentChecked = true;
        }

        // 迭代所有child，执行某个操作，func返回false，终止迭代。
        public void IterateChildren(bool recursive, Func<BaseAsset, bool> func)
        {
            HashSet<string> dealed = new HashSet<string>();
            dealed.Add(m_Name);

            IterateChildren(this, recursive, func, dealed);
        }

        private static bool IterateChildren(BaseAsset baseAsset, bool recursive, Func<BaseAsset, bool> func, HashSet<string> dealed)
        {
            foreach (string childAsset in baseAsset.m_Children)
            {
                BaseAsset child = baseAsset.m_Owner.GetAssetInfo(childAsset);

                if (!dealed.Add(child.Name))
                    continue;

                // 返回false，中断迭代
                if (!func(child))
                    return false;

                if(recursive)
                {
                    // 返回false，中断迭代
                    if(!IterateChildren(child, recursive, func, dealed))
                        return false;
                }
            }

            return true;
        }

        // 迭代所有parent，执行某个操作，func返回false，终止迭代。
        public void IterateParents(bool recursive, Func<BaseAsset, bool> func)
        {
            HashSet<string> dealed = new HashSet<string>();
            dealed.Add(m_Name);

            IterateParents(this, recursive, func, dealed);
        }

        private static bool IterateParents(BaseAsset baseAsset, bool recursive, Func<BaseAsset, bool> func, HashSet<string> dealed)
        {
            foreach (string parentAsset in baseAsset.m_Parents)
            {
                BaseAsset parent = baseAsset.m_Owner.GetAssetInfo(parentAsset);

                if(!dealed.Add(parent.Name))
                    continue;

                // 返回false，中断迭代
                if (!func(parent))
                    return false;

                if(recursive)
                {
                    // 返回false，中断迭代
                    if(!IterateParents(parent, recursive, func, dealed))
                        return false;
                }
            }

            return true;
        }

        // 获取只被该资源引用的Child资源
        public HashSet<string> GetUniqueChild()
        {
            HashSet<string> dealed = new HashSet<string>();
            HashSet<string> UniquetList = new HashSet<string>();

            dealed.Add(m_Name);
            UniquetList.Add(m_Name);

            GetUniqueChild(this, dealed, UniquetList);

            UniquetList.Remove(m_Name);

            return UniquetList;
        }

        private static void GetUniqueChild(BaseAsset baseAsset, HashSet<string> dealed, HashSet<string> UniquetList)
        {
            foreach (var dependentName in baseAsset.m_Children)
            {
                if (dealed.Contains(dependentName))
                    continue;

                BaseAsset depAsset = baseAsset.m_Owner.GetAssetInfo(dependentName);
                bool bUnique = true;
                foreach (var childParent in depAsset.m_Parents)
                {
                    // 如果子节点有一个父节点不在UniqueList里面，则该子节点就不是检测节点独有的。
                    if(!UniquetList.Contains(childParent))
                    {
                        bUnique = false;
                        break;
                    }
                }

                if(bUnique)
                {
                    UniquetList.Add(dependentName);
                    GetUniqueChild(depAsset, dealed, UniquetList);
                }          
            }
        }
    }

    public class PrefabAsset : BaseAsset
    {
        public PrefabAsset(AssetGraph owner) : base(owner)
        {
        }

        public PrefabAsset(string name, int uniqueID, AssetGraph owner, Type assetType) : base(name, uniqueID, owner, assetType)
        {
        }
    }

    public class ScriptableAsset : BaseAsset
    {
        public ScriptableAsset(AssetGraph owner) : base(owner)
        {
        }

        public ScriptableAsset(string name, int uniqueID, AssetGraph owner, Type assetType) : base(name, uniqueID, owner, assetType)
        {
        }
    }

    public class XlsAsset : BaseAsset
    {
        public XlsAsset(AssetGraph owner) : base(owner)
        {
            IsRoot = true;
        }

        public XlsAsset(string name, int uniqueID, AssetGraph owner, Type assetType) : base(name, uniqueID, owner, assetType)
        {
            IsRoot = true;
        }
    }

    public class ScriptAsset : BaseAsset
    {
        public bool m_HasCombinePath = false;
        public HashSet<string> m_CombilePath = new HashSet<string>();

        public ScriptAsset(AssetGraph owner) : base(owner)
        {
            IsRoot = true;
        }

        public ScriptAsset(string name, int uniqueID, AssetGraph owner, Type assetType) : base(name, uniqueID, owner, assetType)
        {
            IsRoot = true;
        }

        public void PushCombinePath(string path)
        {
            m_CombilePath.Add(path);
            m_HasCombinePath = true;
        }
    }

    public class AssetGraph
    {
        private Dictionary<string, BaseAsset> m_AllAssets = new Dictionary<string, BaseAsset>();
        private Dictionary<Type, Dictionary<string, BaseAsset>> m_TypedAssets = new Dictionary<Type, Dictionary<string, BaseAsset>>();

        private Dictionary<string, BaseAsset> m_RootAssets = new Dictionary<string, BaseAsset>();

        private int m_lastID;
        private bool m_LockCreation;
        private List<string> m_CacheAssetNames = new List<string>();

        public static AssetGraph Instance;

        public AssetGraph()
        {
            Instance = this;
        }

        public Dictionary<string, BaseAsset> RootAssets
        {
            get { return m_RootAssets; }
        }

        public string RootAssetDir
        {
            get;
            set;
        }

        public List<string> SingleRootAssets
        {
            get;
            set;
        }

        public string TargetAssetDir
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是需要检测的资源。如果是全局模式（以表格和脚本为根资源，则所有资源都是目标检测资源，否则指定目录下的才是目标资源）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsTargetAsset(string name)
        {
            if (string.IsNullOrEmpty(RootAssetDir) && SingleRootAssets.Count == 0)
                return true;
            else if (name.Contains(TargetAssetDir))
                return true;

            return false;
        }

        #region BuildAssetGraph

        /// <summary>
        /// 清空资源图
        /// </summary>
        public void ClearGraph()
        {
            m_AllAssets.Clear();
            m_TypedAssets.Clear();
            m_RootAssets.Clear();

            m_LockCreation = false;
            m_lastID = 0;
        }

        private bool IsIgnore(string assetName)
        {
            if (AssetDatabase.IsValidFolder(assetName))
                return true;

            // 忽略二进制文件和spriteatlas
            if (assetName.EndsWith("bytes") || assetName.EndsWith("spriteatlas"))
                return true;

            return false;
        }

        /// <summary>
        /// 收集所有资源信息并分析显式依赖
        /// </summary>
        /// <param name="progressRange"></param>
        public IEnumerator CollecRootAssets(string dir, ProgressRange progressRange)
        {
            string[] allAssets = AssetUtility.FindAllAssets(dir);
            for (int i = 0; i < allAssets.Length; ++i)
            {
                float fProgress = progressRange.Start + i * progressRange.Step() / allAssets.Length;
                string info = string.Format("正在收集根资源（{0}/{1}）", i, allAssets.Length);

                EditorUtility.DisplayProgressBar("", info, fProgress);

                if (IsIgnore(allAssets[i]))
                    continue;

                BaseAsset baseAsset = GetOrCreateAssetInfo(allAssets[i], true);
                if (baseAsset != null)
                {
                    baseAsset.IsRoot = true;
                    baseAsset.CheckDependent();
                }

                yield return null;
            }
        }

        /// <summary>
        /// 添加单独的Root资源,比如GlobalSetting
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public IEnumerator AddSingleRootAssets(string assetPath)
        {
            BaseAsset baseAsset = GetOrCreateAssetInfo(assetPath, true);
            if (baseAsset != null)
            {
                baseAsset.IsRoot = true;
                baseAsset.CheckDependent();
            }

            yield return null;
        }

        /// <summary>
        /// 收集所有资源信息并分析显式依赖
        /// </summary>
        /// <param name="progressRange"></param>
        public IEnumerator CollecNonRootAssets(string dir, ProgressRange progressRange)
        {
            string[] allAssets = AssetUtility.FindAllAssets(dir);
            for (int i = 0; i < allAssets.Length; ++i)
            {
                float fProgress = progressRange.Start + i * progressRange.Step() / allAssets.Length;
                string info = string.Format("正在收集待检测资源（{0}/{1}）", i, allAssets.Length);

                EditorUtility.DisplayProgressBar("", info, fProgress);

                if (IsIgnore(allAssets[i]))
                    continue;

                BaseAsset baseAsset = GetOrCreateAssetInfo(allAssets[i]);
                if (baseAsset != null)
                {
                    baseAsset.CheckDependent();
                }

                yield return null;
            }
        }

        /// <summary>
        /// 收集所有资源信息并分析显式依赖
        /// </summary>
        /// <param name="progressRange"></param>
        public void CollecAllAssetAndAnalyseExplicitDependency(string dir, ProgressRange progressRange)
        {
            string[] allAssets = AssetUtility.FindAllAssets(dir);
            for (int i = 0; i < allAssets.Length; ++i)
            {
                float fProgress = progressRange.Start + i * progressRange.Step() / allAssets.Length;
                string info = string.Format("正在收集资源并统计显式依赖（{0}/{1}）", i, allAssets.Length);

                EditorUtility.DisplayProgressBar("", info, fProgress);

                if (IsIgnore(allAssets[i]))
                    continue;

                BaseAsset baseAsset = GetOrCreateAssetInfo(allAssets[i]);
                if (baseAsset != null)
                {
                    baseAsset.CheckDependent();
                }
            }
        }

        /// <summary>
        /// 分析隐式依赖（脚本中的字符串，TextAsset中的字符串）
        /// </summary>
        /// <returns></returns>
        public IEnumerator AnalyseImplicitDependency(ProgressRange progressRange)
        {
            m_LockCreation = true;

            Dictionary<string, BaseAsset> prefabAssets = GetAssetGroup(typeof(PrefabAsset));
            Dictionary<string, BaseAsset> scriptableAssets = GetAssetGroup(typeof(ScriptableAsset));
            Dictionary<string, BaseAsset> textAssets = GetAssetGroup(typeof(TextAsset));

            int count = prefabAssets.Count + scriptableAssets.Count + textAssets.Count;
            int current = 0;

            // Prefab
            {
                var itr = prefabAssets.GetEnumerator();
                while (itr.MoveNext())
                {
                    // 显式进度条
                    {
                        if (current % 50 == 0)
                        {
                            Resources.UnloadUnusedAssets();
                        }

                        float fProgress = progressRange.Start + current * progressRange.Step() / count;
                        string info = string.Format("正在分析隐式依赖（{0}/{1}）", current, count);
                        current++;

                        EditorUtility.DisplayProgressBar("", info, fProgress);
                    }

                    BaseAsset baseAsset = itr.Current.Value;

                    WarningWindow.PushInfo("正在分析隐式依赖: ", baseAsset.Name);

                    // 检测脚本上的字符串依赖
                    {
                        GameObject prefabAsset = null;
                        List<string> outAssetPathes = new List<string>();

                        foreach (var child in baseAsset.m_Children)
                        {
                            BaseAsset childAsset = GetAssetInfo(child);

                            if (childAsset != null && childAsset.AssetType == typeof(MonoScript))
                            {
                                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(child);
                                if (monoScript == null)
                                {
                                    WarningWindow.PushWarning("AssetDatabase.LoadAssetAtPath加载资源失败：\"{0}\" in \"{1}\"", child, baseAsset.Name);
                                    continue;
                                }

                                if (monoScript.GetClass() == null)
                                {
                                    WarningWindow.PushWarning("脚本中类型缺失：\"{0}\" in \"{1}\"", child, baseAsset.Name);
                                    continue;
                                }

                                // 只检测继承至MonoBehavior的组件
                                if (monoScript.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
                                {
                                    Console.WriteLine(child);

                                    TypeStringInfo typeInfo = ScriptAnalysis.GetClassStringInfo(monoScript);
                                    if (typeInfo != null)
                                    {
                                        if (prefabAsset == null)
                                        {
                                            prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(baseAsset.Name);

                                            if (prefabAsset == null)
                                            {
                                                WarningWindow.PushError("AssetDatabase.LoadAssetAtPath加载资源失败：\"{0}\"", baseAsset.Name);
                                            }
                                        }

                                        Component[] components = prefabAsset.GetComponentsInChildren(monoScript.GetClass());
                                        foreach (var component in components)
                                        {
                                            CheckStringField(component, typeInfo, outAssetPathes, new HashSet<object>());
                                        }
                                    }
                                }
                            }
                        }

                        for (int j = 0; j < outAssetPathes.Count; ++j)
                        {
                            LinkAsset(baseAsset.Name, outAssetPathes[j], true);
                        }
                    }

                    yield return null;
                }
            }

            // ScriptableObject
            {
                var itr = scriptableAssets.GetEnumerator();
                while (itr.MoveNext())
                {
                    // 显式进度条
                    {
                        if (current % 50 == 0)
                        {
                            Resources.UnloadUnusedAssets();
                        }

                        float fProgress = progressRange.Start + current * progressRange.Step() / count;
                        string info = string.Format("正在分析隐式依赖（{0}/{1}）", current, count);
                        current++;

                        EditorUtility.DisplayProgressBar("", info, fProgress);
                    }

                    BaseAsset baseAsset = GetAssetInfo(itr.Current.Key);

                    WarningWindow.PushInfo("正在分析隐式依赖: ", baseAsset.Name);

                    // 检测脚本上的字符串依赖
                    {
                        ScriptableObject sciptObjectAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(baseAsset.Name);
                        if (sciptObjectAsset == null)
                        {
                            WarningWindow.PushWarning("AssetDatabase.LoadAssetAtPath加载资源失败：\"{0}\"", baseAsset.Name);
                            continue;
                        }

                        bool mainScriptChecked = false;
                        List<string> outAssetPathes = new List<string>();
                        foreach (var child in baseAsset.m_Children)
                        {
                            BaseAsset childAsset = GetAssetInfo(child);

                            if (childAsset != null && childAsset.AssetType == typeof(MonoScript))
                            {
                                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(child);
                                if (monoScript == null)
                                {
                                    WarningWindow.PushWarning("AssetDatabase.LoadAssetAtPath加载资源失败：\"{0}\"", child);
                                    continue;
                                }

                                // ScriptableObject只检测主脚本，通过prefab等依赖的其他脚本不用检测，在检测prefab的时候会检测。
                                if (monoScript.GetClass() == sciptObjectAsset.GetType())
                                {
                                    TypeStringInfo typeInfo = ScriptAnalysis.GetClassStringInfo(monoScript);
                                    if (typeInfo != null)
                                    {
                                        CheckStringField(sciptObjectAsset, typeInfo, outAssetPathes, new HashSet<object>());
                                    }

                                    mainScriptChecked = true;
                                    break;
                                }
                            }
                        }

                        // ScriptableObject未发现主脚本，异常
                        if (!mainScriptChecked)
                        {
                            WarningWindow.PushWarning("ScriptableObject未检测主脚本：\"{0}\"", baseAsset.Name);
                        }

                        for (int j = 0; j < outAssetPathes.Count; ++j)
                        {
                            LinkAsset(baseAsset.Name, outAssetPathes[j], true);
                        }
                    }

                    yield return null;
                }
            }

            // TextAsset
            {
                Dictionary<string, BaseAsset> textAsset;
                if(m_TypedAssets.TryGetValue(typeof(TextAsset), out textAsset))
                {
                    foreach (var itr in textAsset)
                    {
                        // SkillData的FileList.json文件
                        if (itr.Value.Name.EndsWith("_FileList.json"))
                        {
                            AnalyseFileListJson(itr.Value.Name);
                        }

                        // ShaderList文件
                    }
                }

                yield return null;
            }

            m_LockCreation = false;

            Resources.UnloadUnusedAssets();
        }

        private void AnalyseFileListJson(string jsonName)
        {
            if (CleanAssetsEditor.projectionVersion == CleanAssetsEditor.ProjectionVersion.One)
                _AnalyseFileListJson1(jsonName);
            else if (CleanAssetsEditor.projectionVersion == CleanAssetsEditor.ProjectionVersion.Two)
                _AnalyseFileListJson2(jsonName);
        }

        /// <summary>
        /// 分析SkillData的json文件:1.0
        /// </summary>
        /// <param name="jsonName"></param>
        private void _AnalyseFileListJson1(string jsonName)
        {
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonName);
            if (textAsset == null)
                return;

            string path = Path.GetDirectoryName(jsonName);

            string content = textAsset.text;
            ArrayList list = XUPorterJSON.MiniJSON.jsonDecode(content) as ArrayList;

            for (int i = 0; i < list.Count; ++i)
            {
                string skillfn = path + "/" + list[i];
                if (AssetUtility.IsAssetPath(ref skillfn))
                {
                    LinkAsset(jsonName, skillfn, false);
                }
            }
        }

        /// <summary>
        /// 分析SkillData的json文件:2.0
        /// </summary>
        /// <param name="jsonName"></param>
        private void _AnalyseFileListJson2(string jsonName)
        {
            List<string> assetCandidate = CodeAnalyser.GetAllCandidateAssetPath(jsonName);

            for (int i = 0; i < assetCandidate.Count; ++i)
            {
                LinkAsset(jsonName, assetCandidate[i], false);
            }
        }

        /// <summary>
        /// 分析表格对资源的依赖
        /// </summary>
        /// <param name="progressRange"></param>
        public IEnumerator AnalyseXlsDependent(ProgressRange progressRange)
        {
            CreateDummyRoot();

            string[] allXls = XlsUtil.GetXLSFileList();
            for (int i = 0; i < allXls.Length; ++i)
            {
                XlsxDataUnit unit = new XlsxDataUnit(allXls[i]);

                float fProgress = progressRange.Start + i * progressRange.Step() / allXls.Length;
                string info = string.Format("正在收集表格资源（{0}/{1}：{2}）", i, allXls.Length, unit.ProtoName);

                EditorUtility.DisplayProgressBar("", info, fProgress);


                List<string> assetNames = unit.GetAllAssetNames();
                if (assetNames.Count > 0)
                {
                    BaseAsset xmlAsset = CreateXlsAsset(unit.XlsName);

                    foreach (string assetName in assetNames)
                    {
                        LinkAsset(xmlAsset.Name, assetName, false);
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// 分析代码对资源的依赖
        /// </summary>
        /// <param name="progressRange"></param>
        public IEnumerator AnalyseScriptDependent(ProgressRange progressRange)
        {
            string[] allScripts = AssetUtility.FindAllScripts("Assets");
            for (int i = 0; i < allScripts.Length; ++i)
            {
                // 不检测Editor下的脚本
                if (allScripts[i].Contains("/Editor/"))
                    continue;

                float fProgress = progressRange.Start + i * progressRange.Step() / allScripts.Length;
                string info = string.Format("正在收集代码中资源（{0}/{1}：{2}）", i, allScripts.Length, allScripts[i]);

                EditorUtility.DisplayProgressBar("", info, fProgress);

                List<string> assetCandidate = CodeAnalyser.GetAllCandidateAssetPath(allScripts[i]);
                if (assetCandidate.Count > 0)
                {
                    // 避免与prefab上找到的script名称同名
                    string showName = "Script: " + allScripts[i];

                    ScriptAsset scriptAsset = CreatScriptAsset(showName);

                    foreach (string candidateName in assetCandidate)
                    {
                        int bracePos = candidateName.IndexOf("{");
                        if (bracePos > 0)
                        {
                            // 合成路径。
                            scriptAsset.PushCombinePath(candidateName);
                        }
                        else
                        {
                            LinkAsset(showName, candidateName, false);
                        }
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// 更新资源树状态
        /// </summary>
        /// <param name="progressRange"></param>
        public IEnumerator UpdateTreeStatus(ProgressRange progressRange)
        {
            m_LockCreation = true;

            int count = m_RootAssets.Count;
            int currentIndex = 0;

            var itr = m_RootAssets.GetEnumerator();
            while (itr.MoveNext())
            {
                BaseAsset baseAsset = itr.Current.Value;

                float fProgress = progressRange.Start + currentIndex * progressRange.Step() / count;
                string info = string.Format("正在生成资源依赖信息（{0}/{1}：{2}）", currentIndex, count, baseAsset.Name);
                currentIndex++;

                EditorUtility.DisplayProgressBar("", info, fProgress);

                UpdateRootStatus(baseAsset);
            }

            m_LockCreation = false;

            yield return null;
        }

        #endregion

        #region GetTreeViewAsset

        public void GetAssetNames(CleanAssetsEditor.TreeType treeType, AssetFilter.AssetFilterResult filterResult, List<string> outList, List<string> outList2)
        {
            switch(treeType)
            {
                case CleanAssetsEditor.TreeType.RootAssetTree:
                    GetRootAssets(filterResult, outList, outList2);
                    break;
                case CleanAssetsEditor.TreeType.UsedAssetTree:
                    GetUsedAssets(filterResult, outList);
                    break;
                case CleanAssetsEditor.TreeType.UnUsedAssetTree:
                    GetUnUsedAssets(filterResult, outList);
                    break;
                case CleanAssetsEditor.TreeType.DeleteAssetTree:
                    GetDeleteAssets(filterResult, outList);
                    break;
            }
        }

        private void GetRootAssets(AssetFilter.AssetFilterResult filterResult, List<string> outList, List<string> outList2)
        {
            var itr = m_RootAssets.GetEnumerator();
            while (itr.MoveNext())
            {
                ScriptAsset scriptAsset = itr.Current.Value as ScriptAsset;

                // 有combinePath的脚本放在最后黄色显示
                if (scriptAsset != null && scriptAsset.m_HasCombinePath)
                    outList2.Add(itr.Current.Key);
                else
                    outList.Add(itr.Current.Key);
            }
        }

        /*private void GetXlsAndScriptAssets(AssetFilter.AssetFilterResult filterResult, List<string> outList, List<string> outList2)
        {
            bool includeXls = true;
            bool includeScript = true;

            if (filterResult != null && !filterResult.ShowAll)
            {
                if (filterResult.Inclusive)
                {
                    includeXls = false;
                    includeScript = false;

                    for (int i = 0; i < filterResult.Types.Count; ++i)
                    {
                        if (filterResult.Types[i] == typeof(XlsAsset))
                        {
                            includeXls = true;
                        }
                        else if (filterResult.Types[i] == typeof(ScriptAsset))
                        {
                            includeScript = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < filterResult.Types.Count; ++i)
                    {
                        if (filterResult.Types[i] == typeof(XlsAsset))
                        {
                            includeXls = false;
                        }
                        else if (filterResult.Types[i] == typeof(ScriptAsset))
                        {
                            includeScript = false;
                        }
                    }
                }
            }

            if (includeXls)
            {
                var itr = m_XlsAssets.GetEnumerator();
                while (itr.MoveNext())
                {
                    outList.Add(itr.Current.Key);
                }
            }

            if (includeScript)
            {
                var itr = m_ScriptAssets.GetEnumerator();
                while (itr.MoveNext())
                {
                    // 有combinePath的脚本放在最后黄色显示
                    if (itr.Current.Value.m_HasCombinePath)
                        outList2.Add(itr.Current.Key);
                    else
                        outList.Add(itr.Current.Key);
                }
            }
        }*/

        private void GetUsedAssets(AssetFilter.AssetFilterResult filterResult, List<string> outList)
        {
            GetAssets(filterResult, outList, true, false);
        }

        private void GetUnUsedAssets(AssetFilter.AssetFilterResult filterResult, List<string> outList)
        {
            GetAssets(filterResult, outList, false, false);
        }

        private void GetDeleteAssets(AssetFilter.AssetFilterResult filterResult, List<string> outList)
        {
            GetAssets(filterResult, outList, false, true);
        }

        private void GetAssets(AssetFilter.AssetFilterResult filterResult, List<string> outList, bool bUsed, bool bDelete, bool bRoot = false)
        {
            var itr = m_TypedAssets.GetEnumerator();
            while (itr.MoveNext())
            {
                if (filterResult != null && !filterResult.ShowAll)
                {
                    bool bHasType = false;
                    for (int i = 0; i < filterResult.Types.Count; ++i)
                    {
                        // 当用TextAsset做为filter的时候，MonoScript不算在里面
                        if(itr.Current.Key == typeof(MonoScript) && filterResult.Types[i] == typeof(TextAsset))
                        {
                            continue;
                        }

                        if (filterResult.Types[i].IsAssignableFrom(itr.Current.Key))
                        {
                            bHasType = true;
                            break;
                        }
                    }

                    // Incluseive，不在列表中。或者Exclusive，在列表中。都忽略。
                    if (filterResult.Inclusive != bHasType)
                    {
                        continue;
                    }
                }

                Dictionary<string, BaseAsset> assetGroup = itr.Current.Value;
                var itr2 = assetGroup.GetEnumerator();
                while (itr2.MoveNext())
                {
                    BaseAsset baseAsset = itr2.Current.Value;
                    if (!baseAsset.IsTarget || baseAsset.IsRoot != bRoot)
                        continue;

                    if (bDelete)
                    {
                        if(baseAsset.MarkAsDelete)
                        {
                            outList.Add(baseAsset.Name);
                        }
                    }
                    else
                    {
                        if (baseAsset.DependentByRoot == bUsed && !baseAsset.MarkAsDelete)
                        {
                            outList.Add(baseAsset.Name);
                        }
                    }
                }
            }
        }

        #endregion

        private void UpdateRootStatus(BaseAsset baseAsset)
        {
            baseAsset.DirectDependentByRoot = true;
            baseAsset.StatusUpdated = true;

            foreach (var child in baseAsset.m_Children)
            {
                BaseAsset childsset = GetAssetInfo(child);

                childsset.DirectDependentByRoot = true;
                childsset.StatusUpdated = true;

                childsset.IterateChildren(true, grandChild =>
                {
                    if (!grandChild.StatusUpdated)
                    {
                        grandChild.InDirectDependentByRoot = true;
                        grandChild.StatusUpdated = true;
                    }

                    return true;
                }
                );
            }
        }

        private BaseAsset CreateDummyRoot()
        {
            BaseAsset dummyRoot = CreateXlsAsset("手动标记使用资源根节点");
            dummyRoot.DummyRoot = true;

            return dummyRoot;
        }

        public XlsAsset CreateXlsAsset(string assetName)
        {
            if (m_LockCreation)
            {
                WarningWindow.PushError("m_LockCreation = true in GetOrCreateAssetInfo: {0}", assetName);
                return null;
            }

            m_LockCreation = true;

            BaseAsset ret;
            if (!m_AllAssets.TryGetValue(assetName, out ret))
            {
                ret = new XlsAsset(assetName, m_lastID++, this, typeof(XlsAsset));

                m_AllAssets.Add(assetName, ret);
                m_RootAssets.Add(assetName, ret);
            }

            m_LockCreation = false;

            return ret as XlsAsset;
        }

        public ScriptAsset CreatScriptAsset(string assetName)
        {
            if (m_LockCreation)
            {
                WarningWindow.PushError("m_LockCreation = true in GetOrCreateAssetInfo: {0}", assetName);
                return null;
            }

            m_LockCreation = true;

            BaseAsset ret;
            if (!m_AllAssets.TryGetValue(assetName, out ret))
            {
                ret = new ScriptAsset(assetName, m_lastID++, this, typeof(ScriptAsset));

                m_AllAssets.Add(assetName, ret);
                m_RootAssets.Add(assetName, ret);
            }

            m_LockCreation = false;

            return ret as ScriptAsset;
        }

        public Dictionary<string, BaseAsset> GetAssetGroup(Type type)
        {
            Dictionary<string, BaseAsset> ret;
            if (m_TypedAssets.TryGetValue(type, out ret))
                return ret;
            else
                return new Dictionary<string, BaseAsset>(); 
        }

        public BaseAsset GetAssetInfo(string assetName)
        {
            BaseAsset ret = null;
            m_AllAssets.TryGetValue(assetName, out ret);

            return ret;
        }

        private Type CheckAssetType(Type type)
        {
            if (type.IsSubclassOf(typeof(Texture)))
            {
                return typeof(Texture);
            }
/*
            else if (type == typeof(TextAsset))
            {
                return typeof(TextAsset);
            }*/

            return type;
        }

        /// <summary>
        /// 创建一个资源信息，并登记到上面几个表中
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public BaseAsset GetOrCreateAssetInfo(string assetName, bool isRoot = false)
        {
            BaseAsset baseAsset = null;
            if (m_AllAssets.TryGetValue(assetName, out baseAsset))
            {
                if(isRoot && !m_RootAssets.ContainsKey(assetName))
                {
                    m_RootAssets.Add(assetName, baseAsset);
                }

                return baseAsset;
            }

            if(m_LockCreation)
            {
                //WarningWindow.PushError("m_LockCreation = true in GetOrCreateAssetInfo: {0}", assetName);
                m_CacheAssetNames.Add(assetName);
                return null;
            }

            m_LockCreation = true;

            Type assetType = null;
            if (assetName.EndsWith(".prefab"))
            {
                assetType = typeof(PrefabAsset);
                baseAsset = new PrefabAsset(assetName, m_lastID++, this, assetType);
            }
            else if (assetName.EndsWith(".asset"))
            {
                assetType = typeof(ScriptableAsset);
                baseAsset = new ScriptableAsset(assetName, m_lastID++, this, assetType);
            }
            else
            {
                UnityEngine.Object unityAsset = AssetDatabase.LoadMainAssetAtPath(assetName);
                if (unityAsset == null)
                {
                    WarningWindow.PushWarning("Cannot Load UnityAsset in GetOrCreateAssetInfo: {0}", assetName);
                }

                assetType = CheckAssetType(unityAsset != null ? unityAsset.GetType() : typeof(UnityEngine.Object));

                baseAsset = new BaseAsset(assetName, m_lastID++, this, assetType);
            }

            baseAsset.IsTarget = IsTargetAsset(assetName);

            Dictionary<string, BaseAsset> assetGroup;
            if (!m_TypedAssets.TryGetValue(assetType, out assetGroup))
            {
                assetGroup = new Dictionary<string, BaseAsset>();
                m_TypedAssets.Add(assetType, assetGroup);
            }

            assetGroup.Add(assetName, baseAsset);
            m_AllAssets.Add(assetName, baseAsset);
            if(isRoot)
            {
                m_RootAssets.Add(assetName, baseAsset);
            }

            m_LockCreation = false;

            return baseAsset;
        }

        public void LinkAsset(string parent, string child, bool bImplicit, bool bNeedUpdateStatus = false)
        {
            BaseAsset parentAsset = GetOrCreateAssetInfo(parent);
            BaseAsset childAsset = GetOrCreateAssetInfo(child);

            if (parentAsset != null && childAsset != null)
            {
                childAsset.AddParent(parent);
                parentAsset.AddChild(child, bImplicit);

                if(bNeedUpdateStatus)
                {
                    UpdateNodeStatus(childAsset);
                }
            }
        }

        public void UnLinkAsset(string parent, string child, bool bNeedUpdateStatus = false)
        {
            BaseAsset parentAsset = GetAssetInfo(parent);
            BaseAsset childAsset = GetAssetInfo(child);
            if (parentAsset != null && childAsset != null)
            {
                parentAsset.m_Children.Remove(child);
                parentAsset.m_ImplicitChildren.Remove(child);

                childAsset.m_Parents.Remove(parent);

                if (bNeedUpdateStatus)
                {
                    UpdateNodeStatus(childAsset);
                }
            }
        }

        public void RemoveAssetFromGraph(string assetName)
        {  
            BaseAsset baseAsset = GetAssetInfo(assetName);
            if (baseAsset != null)
            {
                // 去除删除文件在AssetGraph中的链接
                HashSet<string> tmpSet = new HashSet<string>();
                tmpSet.UnionWith(baseAsset.m_Parents);
                foreach (string parent in tmpSet)
                {
                    UnLinkAsset(parent, baseAsset.Name);
                }

                tmpSet.Clear();
                tmpSet.UnionWith(baseAsset.m_Children);
                foreach (string child in tmpSet)
                {
                    UnLinkAsset(baseAsset.Name, child);
                }

                // 去除asset登记
                m_AllAssets.Remove(assetName);

                foreach (var assetGroup in m_TypedAssets)
                {
                    assetGroup.Value.Remove(assetName);
                }
            }
        }

        private void GetAllParent(BaseAsset baseAsset, HashSet<BaseAsset> parents)
        {
            foreach (string parentAsset in baseAsset.m_Parents)
            {
                BaseAsset parent = GetAssetInfo(parentAsset);
                if(parents.Add(parent))
                {
                    GetAllParent(parent, parents);
                }
            }
        }

        private void UpdateNodeStatus(BaseAsset baseAsset)
        {
            // 第一个节点单独处理，避免循环引用导致问题。比如A和B相互引用，A之前被root间接引用，现在没有了。如果A直接进入UpdateNodeStatusRecursive
            // 判断，将被判断为被root间接引用，执行完后A和B还是被认为Root间接引用。
            {
                baseAsset.DirectDependentByRoot = false;
                baseAsset.InDirectDependentByRoot = false;

                foreach (string parentAsset in baseAsset.m_Parents)
                {
                    BaseAsset parent = GetAssetInfo(parentAsset);
                    if (parent.IsRoot)
                    {
                        baseAsset.DirectDependentByRoot = true;
                        break;
                    }
                }

                if(!baseAsset.DependentByRoot)
                {
                    HashSet<BaseAsset> parents = new HashSet<BaseAsset>();
                    GetAllParent(baseAsset, parents);

                    foreach(var p in parents)
                    {
                        if(p.IsRoot)
                        {
                            baseAsset.InDirectDependentByRoot = true;
                            break;
                        }
                    }
                }
            }

            HashSet<string> dealed = new HashSet<string>();
            dealed.Add(baseAsset.Name);

            foreach (string childAsset in baseAsset.m_Children)
            {
                BaseAsset child = GetAssetInfo(childAsset);
                UpdateNodeStatusRecursive(child, dealed, baseAsset.DependentByRoot);
            }
        }

        private void UpdateNodeStatusRecursive(BaseAsset baseAsset, HashSet<string> dealed, bool bIndirectDepByRoot)
        {
            if (dealed.Contains(baseAsset.Name))
                return;

            dealed.Add(baseAsset.Name);
            baseAsset.DirectDependentByRoot = false;
            baseAsset.InDirectDependentByRoot = false;


            foreach (string parentAsset in baseAsset.m_Parents)
            {
                BaseAsset parent = GetAssetInfo(parentAsset);

                // 如果有一个parent是root，设置DirectDependentByRoot
                if (parent.IsRoot)
                {
                    baseAsset.DirectDependentByRoot = true;

                    if(bIndirectDepByRoot)
                    {
                        baseAsset.InDirectDependentByRoot = true;
                        break;
                    }
                }
                // 如果有一个parent被root依赖，设置InDirectDependentByRoot
                else if (parent.DependentByRoot)
                {
                    baseAsset.InDirectDependentByRoot = true;
                }

                if(baseAsset.DirectDependentByRoot && baseAsset.InDirectDependentByRoot)
                {
                    break;
                }
            }

            foreach (string childAsset in baseAsset.m_Children)
            {
                BaseAsset child = GetAssetInfo(childAsset);
                UpdateNodeStatusRecursive(child, dealed, baseAsset.DependentByRoot);
            }
        }

        private void CheckStringField(object obj, TypeStringInfo typeInfo, List<string> outAssetPathes, HashSet<object> checkedObject)
        {
            if (obj == null)
                return;

            foreach (var itr in typeInfo.m_CandidateField)
            {
                FieldInfo fieldInfo = itr.Key;

                object fieldObj = fieldInfo.GetValue(obj);

                // 如果该字段为空，忽略。
                if (fieldObj == null)
                    continue;

                // 如果已经检测过，忽略，避免循环引用导致栈溢出。
                if (checkedObject.Contains(fieldObj))
                    continue;

                if (fieldObj is string)
                {
                    string strValue = fieldObj as string;

                    if (AssetUtility.IsAssetPath(ref strValue))
                    {
                        outAssetPathes.Add(strValue);
                    }
                }
                else if ((fieldObj is string[]) || (fieldObj is List<string>))
                {
                    foreach (var element in (fieldObj as System.Collections.IList))
                    {
                        string strValue = element as string;                     

                        if (AssetUtility.IsAssetPath(ref strValue))
                        {
                            outAssetPathes.Add(strValue);
                        }
                    }
                }
                else
                {
                    Type fieldType = fieldInfo.FieldType;

                    // 如果是List<T>或者数组类型。它们都继承至IList，可以用foreach直接迭代元素。
                    if (fieldType.IsGenericType || fieldType.IsArray)
                    {
                        foreach (var element in (fieldObj as System.Collections.IList))
                        {
                            if (element == null)
                                continue;

                            CheckStringField(element, itr.Value, outAssetPathes, checkedObject);
                        }
                    }
                    else
                    {
                        CheckStringField(fieldObj, itr.Value, outAssetPathes, checkedObject);
                    }

                }
            }
        }

        private void LoadAssetInfo(BaseAsset asset, XmlNode node)
        {
            XmlElement xmlElement = node as XmlElement;

            asset.m_Name = xmlElement.GetAttribute("name");
            asset.m_uniqueID = int.Parse(xmlElement.GetAttribute("id"));
            asset.m_Mask = int.Parse(xmlElement.GetAttribute("mask"));
            asset.IsTarget = bool.Parse(xmlElement.GetAttribute("istarget"));

            string parentIDs = xmlElement.GetAttribute("parents");
            string childIDs = xmlElement.GetAttribute("children"); 
            string impchildIDs = xmlElement.GetAttribute("impchildren");

            // 先存在这里，之后所有Asset都加载进来了再建立连接。
            if (parentIDs.Length > 0)
                asset.m_Parents.Add(parentIDs);

            if (childIDs.Length > 0)
                asset.m_Children.Add(childIDs);

            if (impchildIDs.Length > 0)
                asset.m_ImplicitChildren.Add(impchildIDs);


            if (asset is ScriptAsset)
            {
                ScriptAsset scriptAsset = asset as ScriptAsset;
                foreach (XmlNode xmlAsset in xmlElement.ChildNodes)
                {
                    string name = (xmlAsset as XmlElement).GetAttribute("name");
                    scriptAsset.PushCombinePath(name);
                }
            }

            /*
                        char[] spliter = new char[] { ',' };

                        if (parentIDs.Length > 0)
                        {
                            string[] pID = parentIDs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);

                            for(int i = 0; i < pID.Length; ++i)
                            {
                                int pUniqueID = int.Parse(pID[i]);
                            }
                        }*/
        }

        private void SaveAssetInfo(BaseAsset asset, XmlDocument xmlDoc, XmlElement parentNode, string name)
        {
            XmlElement xmlNode = xmlDoc.CreateElement(name);
            xmlNode.SetAttribute("name", asset.m_Name);
            xmlNode.SetAttribute("id", asset.m_uniqueID.ToString());
            xmlNode.SetAttribute("mask", asset.m_Mask.ToString());
            xmlNode.SetAttribute("istarget", asset.IsTarget.ToString());
            xmlNode.SetAttribute("type", asset.GetType().ToString());

            string parentIDs = "";
            foreach (var parent in asset.m_Parents)
            {
                BaseAsset passet;
                if (m_AllAssets.TryGetValue(parent, out passet))
                {
                    parentIDs += passet.m_uniqueID.ToString() + ",";
                }
            }

            xmlNode.SetAttribute("parents", parentIDs);

            string childIDs = "";
            foreach (var child in asset.m_Children)
            {
                BaseAsset casset;
                if (m_AllAssets.TryGetValue(child, out casset))
                {
                    childIDs += casset.m_uniqueID.ToString() + ",";
                }
            }

            xmlNode.SetAttribute("children", childIDs);

            string impchildIDs = "";
            foreach (var child in asset.m_ImplicitChildren)
            {
                BaseAsset casset;
                if (m_AllAssets.TryGetValue(child, out casset))
                {
                    impchildIDs += casset.m_uniqueID.ToString() + ",";
                }
            }

            xmlNode.SetAttribute("impchildren", impchildIDs);

            if (asset is ScriptAsset)
            {
                ScriptAsset scriptAsset = asset as ScriptAsset;
                foreach (string combinePath in scriptAsset.m_CombilePath)
                {
                    XmlElement xmlCombineNode = xmlDoc.CreateElement("combinePath");
                    xmlCombineNode.SetAttribute("name", combinePath);

                    xmlNode.AppendChild(xmlCombineNode);
                }
            }

            parentNode.AppendChild(xmlNode);
        }


        public void LoadFromXML(string fileName)
        {
            ClearGraph();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            Dictionary<int, BaseAsset> tmpDic = new Dictionary<int, BaseAsset>();
            var xmlRoot = xmlDoc.SelectSingleNode("Root");
            if (xmlRoot != null)
            {
                foreach (XmlNode xmlAssetType in xmlRoot.ChildNodes)
                {
                    /*// XLSs
                    if (xmlAssetType.LocalName == "XLSs")
                    {
                        int count = int.Parse((xmlAssetType as XmlElement).GetAttribute("count"));

                        foreach (XmlNode xmlAsset in xmlAssetType.ChildNodes)
                        {
                            XlsAsset asset = new XlsAsset(this);
                            asset.m_AssetType = typeof(XlsAsset);

                            LoadAssetInfo(asset, xmlAsset);

                            m_XlsAssets.Add(asset.Name, asset);
                            m_AllAssets.Add(asset.Name, asset);

                            tmpDic.Add(asset.m_uniqueID, asset);
                        }

                        Debug.Assert(m_XlsAssets.Count == count);
                    }

                    // Scripts
                    if (xmlAssetType.LocalName == "Scripts")
                    {
                        int count = int.Parse((xmlAssetType as XmlElement).GetAttribute("count"));

                        foreach (XmlNode xmlAsset in xmlAssetType.ChildNodes)
                        {
                            ScriptAsset asset = new ScriptAsset(this);
                            asset.m_AssetType = typeof(ScriptAsset);

                            LoadAssetInfo(asset, xmlAsset);

                            m_ScriptAssets.Add(asset.Name, asset);
                            m_AllAssets.Add(asset.Name, asset);

                            tmpDic.Add(asset.m_uniqueID, asset);
                        }

                        Debug.Assert(m_ScriptAssets.Count == count);
                    }*/

                    // Roots 
                    if (xmlAssetType.LocalName == "RootAssets")
                    {
                        int count = int.Parse((xmlAssetType as XmlElement).GetAttribute("count"));

                        foreach (XmlNode xmlAsset in xmlAssetType.ChildNodes)
                        {
                            string typeName = (xmlAsset as XmlElement).GetAttribute("type");
                            Type type = CleanAssetAssemblyUtility.GetType(typeName);

                            if (type == null)
                                type = typeof(BaseAsset);

                            BaseAsset asset = (BaseAsset)Activator.CreateInstance(type, this);
                            asset.m_AssetType = type;
                            asset.IsRoot = true;

                            LoadAssetInfo(asset, xmlAsset);

                            m_RootAssets.Add(asset.Name, asset);
                            m_AllAssets.Add(asset.Name, asset);

                            tmpDic.Add(asset.m_uniqueID, asset);
                        }

                        Debug.Assert(m_RootAssets.Count == count);
                    }

                    // PureAssets
                    if (xmlAssetType.LocalName == "PureAssets")
                    {
                        foreach (XmlNode xmlType in xmlAssetType.ChildNodes)
                        {
                            string typeName = (xmlType as XmlElement).GetAttribute("name");
                            Type type = CleanAssetAssemblyUtility.GetType(typeName);

                            Dictionary<string, BaseAsset> assetGroup = new Dictionary<string, BaseAsset>();
                            m_TypedAssets.Add(type, assetGroup);

                            foreach (XmlNode xmlAssets in xmlType.ChildNodes)
                            {
                                BaseAsset asset = null;
                                if(type == typeof(PrefabAsset))
                                {
                                    asset = new PrefabAsset(this);
                                }
                                else if (type == typeof(ScriptableAsset))
                                {
                                    asset = new ScriptableAsset(this);
                                }
                                else
                                {
                                    asset = new BaseAsset(this);
                                }

                                asset.m_AssetType = type;

                                LoadAssetInfo(asset, xmlAssets);

                                assetGroup.Add(asset.Name, asset);
                                m_AllAssets.Add(asset.Name, asset);

                                tmpDic.Add(asset.m_uniqueID, asset);
                            }
                        }
                    }
                }
            }

            char[] spliter = new char[] { ',' };

            foreach (var itr in tmpDic)
            {
                BaseAsset asset = itr.Value;
                if (m_lastID <= asset.m_uniqueID)
                    m_lastID = asset.m_uniqueID + 1;

                if (asset.m_Parents.Count > 0)
                {
                    string parentIDs = asset.m_Parents.ElementAt(0);
                    asset.m_Parents.Clear();

                    string[] pID = parentIDs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < pID.Length; ++i)
                    {
                        int pUniqueID = int.Parse(pID[i]);

                        BaseAsset parentAsset = tmpDic[pUniqueID];
                        asset.m_Parents.Add(parentAsset.m_Name);
                    }
                }

                if (asset.m_Children.Count > 0)
                {
                    string childIDs = asset.m_Children.ElementAt(0);
                    asset.m_Children.Clear();

                    string[] pID = childIDs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < pID.Length; ++i)
                    {
                        int pUniqueID = int.Parse(pID[i]);

                        BaseAsset childAsset = tmpDic[pUniqueID];
                        asset.m_Children.Add(childAsset.m_Name);
                    }
                }

                if (asset.m_ImplicitChildren.Count > 0)
                {
                    string childIDs = asset.m_ImplicitChildren.ElementAt(0);
                    asset.m_ImplicitChildren.Clear();

                    string[] pID = childIDs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < pID.Length; ++i)
                    {
                        int pUniqueID = int.Parse(pID[i]);

                        BaseAsset childAsset = tmpDic[pUniqueID];
                        asset.m_ImplicitChildren.Add(childAsset.m_Name);
                    }
                }
            }

            return;

 /*           {
                AnalyseXlsDependent(new ProgressRange(0.7f, 0.95f));

                // TextAsset
                {
                    Dictionary<string, BaseAsset> textAsset;
                    if (m_TypedAssets.TryGetValue(typeof(TextAsset), out textAsset))
                    {
                        foreach (var itr in textAsset)
                        {
                            // SkillData的FileList.json文件
                            if (itr.Value.Name.EndsWith("_FileList.json"))
                            {
                                AnalyseFileListJson(itr.Value.Name);
                            }

                            // ShaderList文件
                        }
                    }
                }

                UpdateTreeStatus(new ProgressRange(0.95f, 1.0f));
                EditorUtility.ClearProgressBar();
            }*/
        }  

        public void SaveToXML(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlRoot = xmlDoc.CreateElement("Root");

            Dictionary<string, int> nameToID = new Dictionary<string, int>();

            if (xmlRoot != null)
            {
                /*// 表格
                {
                    XmlElement xmlRootNodes = xmlDoc.CreateElement("XLSs");
                    xmlRootNodes.SetAttribute("count", m_XlsAssets.Count.ToString());
                    xmlRoot.AppendChild(xmlRootNodes);

                    foreach (var xlsAsset in m_XlsAssets)
                    {
                        SaveAssetInfo(xlsAsset.Value, xmlDoc, xmlRootNodes, "xls");
                    }
                }

                // 代码
                {
                    XmlElement xmlRootNodes = xmlDoc.CreateElement("Scripts");
                    xmlRootNodes.SetAttribute("count", m_ScriptAssets.Count.ToString());
                    xmlRoot.AppendChild(xmlRootNodes);

                    foreach (var scriptAsset in m_ScriptAssets)
                    {
                        SaveAssetInfo(scriptAsset.Value, xmlDoc, xmlRootNodes, "Script");
                    }
                }
*/
                // Roots 
                {
                    XmlElement xmlRootNodes = xmlDoc.CreateElement("RootAssets");
                    xmlRootNodes.SetAttribute("count", m_RootAssets.Count.ToString());
                    xmlRoot.AppendChild(xmlRootNodes);

                    foreach (var rootAsset in m_RootAssets)
                    {
                        SaveAssetInfo(rootAsset.Value, xmlDoc, xmlRootNodes, "RootAsset");
                    }
                }

                {
                    XmlElement xmlRootNodes = xmlDoc.CreateElement("PureAssets");
                    xmlRoot.AppendChild(xmlRootNodes);

                    foreach (var itr in m_TypedAssets)
                    {
                        XmlElement xmlTypeNode = xmlDoc.CreateElement("Type");
                        xmlTypeNode.SetAttribute("name", itr.Key.FullName);
                        xmlRootNodes.AppendChild(xmlTypeNode);

                        Dictionary<string, BaseAsset> assetGroup = itr.Value;

                        foreach (var usedPureAsset in assetGroup)
                        {
                            if(!usedPureAsset.Value.IsRoot)
                                SaveAssetInfo(usedPureAsset.Value, xmlDoc, xmlTypeNode, "PureAsset");
                        }
                    }
                }
            }

            xmlDoc.AppendChild(xmlRoot);

            xmlDoc.Save(fileName);
        }
    }
}
