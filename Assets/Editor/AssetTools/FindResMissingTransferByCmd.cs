using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class FindResMissingTransferByCmd
{
    #region Params
    private static Dictionary<Object, List<Object>> _allMissingResToCmptDic = new Dictionary<Object, List<Object>>();
    private static Dictionary<Object, string> _allMissingResNameDic = new Dictionary<Object, string>();
    private static string _rootPath;
    private static StringBuilder _strLog = new StringBuilder();
    private static string _logPath;
    #endregion

    #region Init
    private static void FindMissingResTransferByCmd()
    {
        string[] allParams = Environment.GetCommandLineArgs();
        if (allParams.Length > 2)
        {
            _logPath = allParams[allParams.Length - 2] + "\\LogDir";
            _rootPath = allParams[allParams.Length - 1];
        }
        else
        {
            _logPath = Application.dataPath + "\\LogDir";
            _rootPath = Path.Combine(Application.dataPath, "Resources/Actor");
        }
        if (!Directory.Exists(_logPath))
            Directory.CreateDirectory(_logPath);
        Find();
    }
    #endregion

    #region Mono
    private static bool haveShow = false;
    #endregion

    #region Func
    private static void AddLog(string exceptFilePath)
    {
        _strLog.AppendLine(exceptFilePath);
    }
    private static void Find()
    {
        if (string.IsNullOrEmpty(_rootPath))
            return;
        Debug.Log("开始查找: " + DateTime.Now);
        haveShow = false;
        _allMissingResToCmptDic.Clear();
        _allMissingResNameDic.Clear();
        _strLog.Length = 0;
        string[] searchFolder = { _rootPath.Substring(_rootPath.IndexOf("Assets", StringComparison.Ordinal)) };
        string[] allAssetsGuid = AssetDatabase.FindAssets("*", searchFolder);
        int corrCount = RemoveRepeatGuid(ref allAssetsGuid);
        for (int i = 0; i < corrCount; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
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
        }
        Debug.Log("查找结束: " + DateTime.Now);
        string filePath = Path.Combine(_logPath, DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "资源missing检测.txt");
        using (StreamWriter sw = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate)))
        {
            sw.Write(_strLog);
        }
    }
    // 目前发现AssetDatabase.FindAssets("*", searchFolder);查找到的Guid有重复
    private static int RemoveRepeatGuid(ref string[] allGuid)
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
    private static bool SearchGameObjSelf(GameObject gameObj)
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
    private static bool SearchCmpt(Component cmpt, Object obj)
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
    private static bool SearchWholeObj(Object obj)
    {
        bool haveExcept = false;
        SerializedObject serObj = new SerializedObject(obj);
        SerializedProperty iterator = serObj.GetIterator();
        while (iterator.NextVisible(true))
        {
            if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                continue;
            if (iterator.objectReferenceValue != null || iterator.objectReferenceInstanceIDValue == 0)
                continue;
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