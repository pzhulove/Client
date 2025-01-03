using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets
{
    public class FindResMissing : EditorWindow
    {
        #region Params
        private static Dictionary<Object, List<Object>> _allMissingResToCmptDic = new Dictionary<Object, List<Object>>();
        private static Dictionary<Object, string> _allMissingResNameDic = new Dictionary<Object, string>();
        private static string _dirPath;
        private Vector2 _vec3Scroll;
        private string _strWait = "****等等等等****等等等等****";
        private string _waitTitle = "正在查找";
        private StringBuilder _strLog = new StringBuilder();
        private string _searchOnecCount = "100";
        private bool _myRepair = false;

        #region PresetTypeInfo
        private bool _showAll = true;
        private bool _showMay = false;
        private bool _showSuggest = false;
        private string _lastShow = "All";  // All May Suggest
        private bool _isRefresh = true;
        private List<ResTypeCheckData> _curInfo = new List<ResTypeCheckData>();
        private List<ResTypeCheckData> _curShow = new List<ResTypeCheckData>();
        private List<ResTypeCheckData> _presetInfo = new List<ResTypeCheckData>
        {
            new ResTypeCheckData{EndWith = ".jpg", Lv = -1, Desc = "jpg图片"},
            new ResTypeCheckData{EndWith = ".anim", Lv = -1, Desc = "模型动画文件"},
            new ResTypeCheckData{EndWith = ".tga", Lv = -1, Desc = "材质贴图"},
            new ResTypeCheckData{EndWith = ".psd", Lv = -1, Desc = "ps导出图片"},
            new ResTypeCheckData{EndWith = ".png", Lv = -1, Desc = "png图片"},
            new ResTypeCheckData{EndWith = ".controller", Lv = -1, Desc = "动画状态机"},
            new ResTypeCheckData{EndWith = ".ttf", Lv = -1, Desc = "字体相关"},
            new ResTypeCheckData{EndWith = ".tpsheet", Lv = -1, Desc = "UI图集合并信息"},
            new ResTypeCheckData{EndWith = ".xml", Lv = -1, Desc = "常为配置文件"},
            new ResTypeCheckData{EndWith = ".json", Lv = -1, Desc = "常为配置文件"},
            new ResTypeCheckData{EndWith = ".txt", Lv = -1, Desc = "文本文档"},
            new ResTypeCheckData{EndWith = ".shader", Lv = -1, Desc = "材质shader"},
            new ResTypeCheckData{EndWith = ".bytes", Lv = -1, Desc = "二进制文件"},
            new ResTypeCheckData{EndWith = ".unity", Lv = -1, Desc = "unity场景文件"},
            new ResTypeCheckData{EndWith = ".cginc", Lv = -1, Desc = "unity内置shader"},
            new ResTypeCheckData{EndWith = ".dll", Lv = -1, Desc = "代码程序集"},
            new ResTypeCheckData{EndWith = ".ogg", Lv = -1, Desc = "有损音频"},
            new ResTypeCheckData{EndWith = ".wav", Lv = -1, Desc = "无损音频"},
            new ResTypeCheckData{EndWith = ".fnt", Lv = -1, Desc = "字体文件"},
            new ResTypeCheckData{EndWith = ".skel", Lv = -1, Desc = "spine导出"},
            new ResTypeCheckData{EndWith = ".atlas", Lv = -1, Desc = "spine导出"},
            new ResTypeCheckData{EndWith = ".cs", Lv = -1, Desc = "代码文件"},
            new ResTypeCheckData{EndWith = ".shadervariants", Lv = -1, Desc = "Shader变体"},

            new ResTypeCheckData{EndWith = ".fbx", Lv = 0, Desc = "模型文件"},
            new ResTypeCheckData{EndWith = ".physicmaterial", Lv = 0, Desc = "物理材质"},
            new ResTypeCheckData{EndWith = ".fontsettings", Lv = 0, Desc = "字体配置"},
            new ResTypeCheckData{EndWith = ".asset", Lv = 0, Desc = "AB资源包"},

            new ResTypeCheckData{EndWith = ".mat", Lv = 1, Desc = "材质文件"},
            new ResTypeCheckData{EndWith = ".prefab", Lv = 1, Desc = "预制体"},
        };
        #endregion
        #endregion

        #region Init
        [MenuItem("[TM工具集]/Find Missing Res")]
        public static void FindMissingRes()
        {
            FindResMissing window = GetWindow<FindResMissing>();
            window.titleContent = new GUIContent("查找所有Missing资源_新");
            window.Show();
        }
        #endregion

        #region Mono
        private bool haveShow = false;
        // ReSharper disable once InconsistentNaming
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("检查路径", GUILayout.Width(150));
            EditorGUILayout.TextField(_dirPath);
            if (GUILayout.Button("Open", GUILayout.Width(60)))
                _dirPath = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources", "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("--------------------华丽的分割线--------------------");
            if (GUILayout.Button("查找文件类型"))
                FindAllResType();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            string cur = "";
            _showAll = EditorGUILayout.Toggle("显示所有", _showAll);
            if (_showAll)
            {
                _showMay = false;
                _showSuggest = false;
                cur = "All";
            }
            _showMay = EditorGUILayout.Toggle("显示可能", _showMay);
            if (_showMay)
            {
                _showAll = false;
                _showSuggest = false;
                cur = "May";
            }
            _showSuggest = EditorGUILayout.Toggle("显示建议", _showSuggest);
            if (_showSuggest)
            {
                _showAll = false;
                _showMay = false;
                cur = "Suggest";
            }
            EditorGUILayout.EndHorizontal();
            if (_isRefresh || cur != _lastShow)
            {
                _curShow.Clear();
                int lv;
                if (_showAll)
                    lv = -1;
                else if (_showMay)
                    lv = 0;
                else
                    lv = 1;
                foreach (ResTypeCheckData data in _curInfo)
                {
                    if (data.Lv >= lv)
                        _curShow.Add(new ResTypeCheckData { EndWith = data.EndWith, Lv = data.Lv, Desc = data.Desc, IsOpen = data.Lv >= 0 });
                }
                _isRefresh = false;
            }
            _lastShow = cur;
            int curIndex = 0;
            int lastLv = 2;
            for (int i = 0; i < _curShow.Count; i++)
            {
                ResTypeCheckData data = _curShow[i];
                if (data.Lv != lastLv)
                {
                    lastLv = data.Lv;
                    curIndex = 0;
                    if (i > 0)
                        EditorGUILayout.EndHorizontal();
                    string str;
                    if (data.Lv == 1)
                        str = "****以下类型建议查找****";
                    else if (data.Lv == 0)
                        str = "****以下类型可能需要查找****";
                    else
                        str = "****以下类型不建议查找****";
                    EditorGUILayout.LabelField(str, GUILayout.Width(170));
                }
                if (curIndex % 3 == 0)
                    EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(data.EndWith + ": " + data.Desc, GUILayout.Width(150));
                data.IsOpen = EditorGUILayout.Toggle("", data.IsOpen);
                if (curIndex % 3 == 2)
                    EditorGUILayout.EndHorizontal();
                curIndex++;
            }
            if (_curShow.Count > 0 && curIndex % 3 != 2)
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("--------------------华丽的分割线--------------------");
            EditorGUILayout.LabelField("可输入较大数值或负数以查找所有");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("单次检测次数:", GUILayout.Width(100));
            _searchOnecCount = EditorGUILayout.TextField(_searchOnecCount);
            EditorGUILayout.EndHorizontal();
            _myRepair = EditorGUILayout.Toggle("自动修复", _myRepair);
            if (GUILayout.Button("查找"))
                Find();
            _vec3Scroll = EditorGUILayout.BeginScrollView(_vec3Scroll);
            foreach (var res in _allMissingResToCmptDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(res.Key, res.GetType(), true, GUILayout.Width(200));
                EditorGUILayout.BeginVertical();
                foreach (Object cmpt in res.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (!cmpt)
                        GUILayout.Label("存在Component缺失，请查找");
                    else
                    {
                        EditorGUILayout.ObjectField(cmpt, cmpt.GetType(), true, GUILayout.Width(200));
                        string resPath;
                        if (_allMissingResNameDic.TryGetValue(cmpt, out resPath))
                            GUILayout.Label(resPath);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            if (!haveShow && _allMissingResToCmptDic.Count > 0)
            {
                EditorUtility.DisplayDialog("查找完毕", "共找到: " + _allMissingResToCmptDic.Count + "个对象存在文件丢失", "确认");
                haveShow = true;
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Func
        private void AddLog(string exceptFilePath)
        {
            _strLog.AppendLine(exceptFilePath);
        }
        private void FindAllResType()
        {
            if (string.IsNullOrEmpty(_dirPath))
                return;
            string[] searchFolder = { _dirPath.Substring(_dirPath.IndexOf("Assets", StringComparison.Ordinal)) };
            //检索所有资源并去重
            string[] allAssetsGuid = AssetDatabase.FindAssets("*", searchFolder);
            int corrCount = RemoveRepeatGuid(ref allAssetsGuid);
            _curInfo.Clear();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < corrCount; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
                if (Directory.Exists(assetPath))
                    continue;
                int index = assetPath.LastIndexOf('.');
                if (index < 0)
                    continue;
                string endName = assetPath.Substring(index).ToLower();
                if (IsTypeExists(endName, _curInfo))
                    continue;
                _curInfo.Add(new ResTypeCheckData { EndWith = endName, FirstPath = assetPath });
                sb.AppendLine(endName + "***" + Application.dataPath + assetPath.Substring(assetPath.IndexOf('/')));
            }
            CorrectCurInfo();
            _isRefresh = true;
        }
        private bool IsTypeExists(string endName, List<ResTypeCheckData> info)
        {
            foreach (ResTypeCheckData data in info)
            {
                if (data.EndWith == endName)
                    return true;
            }
            return false;
        }
        private void CorrectCurInfo()
        {
            foreach (ResTypeCheckData data in _curInfo)
            {
                foreach (ResTypeCheckData info in _presetInfo)
                {
                    if (data.EndWith == info.EndWith)
                    {
                        data.Desc = info.Desc;
                        data.Lv = info.Lv;
                        break;
                    }
                }
            }

            _curInfo = _curInfo.OrderByDescending(p => p.Lv).ToList();
        }
        private void Find()
        {
            if (string.IsNullOrEmpty(_dirPath))
                return;
            List<ResTypeCheckData> curCheck = null;
            if (!_showAll)
            {
                curCheck = new List<ResTypeCheckData>();
                foreach (ResTypeCheckData data in _curShow)
                {
                    if (data.IsOpen)
                        curCheck.Add(data);
                }
                if (curCheck.Count == 0)
                    return;
            }
            haveShow = false;
            _allMissingResToCmptDic.Clear();
            _allMissingResNameDic.Clear();
            _strLog.Length = 0;
            int onceCount;
            if (!int.TryParse(_searchOnecCount, out onceCount))
            {
                onceCount = 100;
            }
            string[] searchFolder = { _dirPath.Substring(_dirPath.IndexOf("Assets", StringComparison.Ordinal)) };
            string[] allAssetsGuid = AssetDatabase.FindAssets("*", searchFolder);
            int corrCount = RemoveRepeatGuid(ref allAssetsGuid);
            float fProgress = 0;
            for (int i = 0; i < corrCount; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
                if (!_showAll)
                {
                    int index = assetPath.LastIndexOf('.');
                    if (index < 0)
                        continue;
                    string endName = assetPath.Substring(index);
                    if (!IsTypeExists(endName, curCheck))
                        continue;
                }
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                fProgress++;
                EditorUtility.DisplayProgressBar(_waitTitle, _strWait, fProgress / corrCount);
                if (obj == null)
                {
                    continue;
                }
                GameObject gameObj = obj as GameObject;
                if (gameObj != null)
                {
                    if (SearchGameObjSelf(gameObj))
                        AddLog(assetPath);
                }
                else
                {
                    if (SearchWholeObj(obj))
                        AddLog(assetPath);
                }
                if (onceCount >= 0 && _allMissingResToCmptDic.Count >= onceCount)
                    break;
            }
            EditorUtility.ClearProgressBar();
            //string filePath = Path.Combine(Application.dataPath, "FindResMissing.txt");
            //using (StreamWriter sw = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate)))
            //{
            //    sw.Write(_strLog);
            //}
        }
        // 目前发现AssetDatabase.FindAssets("*", searchFolder);查找到的Guid有重复
        private int RemoveRepeatGuid(ref string[] allGuid)
        {
            int curIndex = 0;
            for (int i = 0; i < allGuid.Length; i++)
            {
                while (true)
                {
                    if (i + 1 >= allGuid.Length)
                        break;
                    if (allGuid[i + 1] != allGuid[curIndex])
                    {
                        curIndex++;
                        allGuid[curIndex] = allGuid[i + 1];
                        break;
                    }
                    i++;
                }
            }
            curIndex++;
            return curIndex;
        }
        // 继承自GameObject类，需递归
        private bool SearchGameObjSelf(GameObject gameObj)
        {
            bool haveExcept = false;
            Component[] cmptAry = gameObj.GetComponents<Component>();
            int index = -1;
            foreach (Component cmpt in cmptAry)
            {
                index++;
                if (cmpt is Transform || cmpt is CanvasRenderer)
                    continue;
                if (!cmpt)
                {
                    List<Object> objList;
                    if (!_allMissingResToCmptDic.TryGetValue(gameObj, out objList))
                    {
                        objList = new List<Object>();
                        _allMissingResToCmptDic.Add(gameObj, objList);
                    }
                    objList.Add(cmpt);
                    if (!haveExcept)
                        haveExcept = true;
                    continue;
                }
                if (SearchCmpt(cmpt, gameObj))
                    if (!haveExcept)
                        haveExcept = true;
            }
            for (int i = 0; i < gameObj.transform.childCount; ++i)
            {
                if (SearchGameObjSelf(gameObj.transform.GetChild(i).gameObject))
                    if (!haveExcept)
                        haveExcept = true;
            }
            return haveExcept;
        }
        private bool SearchCmpt(Component cmpt, Object obj)
        {
            bool haveExcept = false;
            SerializedObject serObj = new SerializedObject(cmpt);
            SerializedProperty iterator = serObj.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                    continue;
                if (iterator.objectReferenceValue != null || iterator.objectReferenceInstanceIDValue == 0)
                    continue;
                if (!haveExcept)
                    haveExcept = true;
                if (!_allMissingResNameDic.ContainsKey(cmpt))
                    _allMissingResNameDic.Add(cmpt, iterator.propertyPath);
                else
                    _allMissingResNameDic[cmpt] += " | " + iterator.propertyPath;
                if (_allMissingResToCmptDic.ContainsKey(obj))
                {
                    if (!_allMissingResToCmptDic[obj].Contains(cmpt))
                        _allMissingResToCmptDic[obj].Add(cmpt);
                }
                else
                    _allMissingResToCmptDic.Add(obj, new List<Object> { cmpt });
            }
            return haveExcept;
        }
        // Material之类，继承自Object
        private bool SearchWholeObj(Object obj)
        {
            bool haveExcept = false;
            SerializedObject serObj = new SerializedObject(obj);
            SerializedProperty iterator = serObj.GetIterator();
            bool isMatRep = false;
            while (iterator.NextVisible(true))
            {
                if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                    continue;
                if (iterator.objectReferenceValue != null || iterator.objectReferenceInstanceIDValue == 0)
                    continue;

                #region IgnoreBlock
                // 针对shader: HeroGo/PBR/Surface/General的忽略模块
                Material mat = obj as Material;
                if (mat != null && mat.shader != null && (mat.shader.name == "HeroGo/PBR/Surface/General" || mat.shader.name == "HeroGo/PBR/Surface/Weapon_Tint"))
                    continue;
                if (mat == null)
                    isMatRep = true;
                if (_myRepair)
                {
                    if (mat != null && !isMatRep)
                    {
                        string missingPath = iterator.propertyPath;
                        bool isNeedContinue = false;
                        SerializedObject matObj = new SerializedObject(obj);
                        SerializedProperty matIterator = matObj.GetIterator();
                        while (matIterator.NextVisible(true))
                        {
                            if (matIterator.name != "m_TexEnvs" ||
                                !matIterator.isArray)
                                continue;
                            for (int i = 0; i < matIterator.arraySize; i++)
                            {
                                SerializedProperty dataProperty = matIterator.GetArrayElementAtIndex(i);
                                if (dataProperty != null)
                                {
                                    SerializedProperty firstProperty = dataProperty.FindPropertyRelative("first");
                                    SerializedProperty secondProperty = dataProperty.FindPropertyRelative("second");
                                    SerializedProperty texProperty = null;
                                    if (secondProperty != null)
                                        texProperty = secondProperty.FindPropertyRelative("m_Texture");
                                    if (firstProperty != null && texProperty != null)
                                    {
                                        string texName = firstProperty.stringValue;
                                        if (!mat.HasProperty(texName))
                                        {
                                            if (texProperty.propertyPath == missingPath)
                                                isNeedContinue = true;
                                            if (texProperty.objectReferenceValue == null && texProperty.objectReferenceInstanceIDValue != 0)
                                                mat.SetTexture(texName, null);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        isMatRep = true;
                        if (isNeedContinue)
                            continue;
                    }
                }
                #endregion

                if (!haveExcept)
                    haveExcept = true;
                if (!_allMissingResNameDic.ContainsKey(obj))
                    _allMissingResNameDic.Add(obj, iterator.propertyPath);
                else
                    _allMissingResNameDic[obj] += " | " + iterator.propertyPath;
                if (_allMissingResToCmptDic.ContainsKey(obj))
                {
                    if (!_allMissingResToCmptDic[obj].Contains(obj))
                        _allMissingResToCmptDic[obj].Add(obj);
                }
                else
                    _allMissingResToCmptDic.Add(obj, new List<Object> { obj });
            }
            return haveExcept;
        }
        #endregion
    }

    public class ResTypeCheckData
    {
        public int Lv;
        public string EndWith;
        public string Desc;
        public bool IsOpen = false;
        // debug使用
        public string FirstPath;
    }
}