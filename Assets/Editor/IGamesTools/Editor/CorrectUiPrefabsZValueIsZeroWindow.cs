using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CorrectUiPrefabsZValueIsZeroWindow : EditorWindow
{
    #region Params
    private static string _rootPath;
    private static Vector3 _vec3Scroll;
    private static Dictionary<GameObject, List<RectTransform>> _allZNotZero = new Dictionary<GameObject, List<RectTransform>>();  //预制体, 子Ui列表
    private static List<string> _preblemPrefabPaths = new List<string>();
    #endregion

    #region Init
    [MenuItem("[TM工具集]/UI相关/检查Ui预制体,Z值不为0")]
    private static void InitWindow()
    {
        CorrectUiPrefabsZValueIsZeroWindow window = GetWindow<CorrectUiPrefabsZValueIsZeroWindow>();
        window.titleContent = new GUIContent("查找问题Ui预制体");
        window.Show();
    }
    #endregion

    #region Mono
    private List<GameObject> _allKey;
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("检查路径", GUILayout.Width(80));
        EditorGUILayout.TextField(_rootPath);
        if (GUILayout.Button("Open", GUILayout.Width(60)))
            _rootPath = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath + "/Resources/UI/UiProject", "");
        if (GUILayout.Button("Check", GUILayout.Width(60)))
            CheckAllPrefabs();
        EditorGUILayout.EndHorizontal();
        int count = _allZNotZero.Count;
        if (count == 0)
        {
            GUILayout.TextField("选择路径下未检测到问题");
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.TextField("共" + _allZNotZero.Count + "个问题");
            if (GUILayout.Button("一键修正所有预制体,暂时关闭此功能"))
            {
                AutoCorrectAllPrefab();
            }
            EditorGUILayout.EndHorizontal();
            _vec3Scroll = EditorGUILayout.BeginScrollView(_vec3Scroll);
            _allKey = _allZNotZero.Keys.ToList();
            for (int i = 0; i < _allZNotZero.Count; i++)
            {
                GameObject prefab = _allKey[i];
                List<RectTransform> allChildRTrans = _allZNotZero[prefab];
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(prefab, typeof(GameObject), true, GUILayout.Width(200));
                EditorGUILayout.LabelField("下列子级Ui可能存在问题：",GUILayout.Width(160));
                if (GUILayout.Button("选中Prefab"))
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(_preblemPrefabPaths[i]);
                    EditorGUIUtility.PingObject(go);
                    Selection.activeGameObject = go;
                }
                if (GUILayout.Button("一键修正此预制体，慎用"))
                    AutoCorrectThisPrefab(prefab, ref i);
                if (GUILayout.Button("当前预制体处理完毕"))
                    CurrentPrefabIsDone(prefab, ref i);
                EditorGUILayout.EndHorizontal();
                for (int j = 0; j < allChildRTrans.Count; j++)
                {
                    RectTransform rTrans = allChildRTrans[j];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(rTrans, typeof(RectTransform), true, GUILayout.Width(200));
                    if (GUILayout.Button("处理完毕"))
                    {
                        allChildRTrans.Remove(rTrans);
                        j--;
                        if (allChildRTrans.Count == 0)
                        {
                            _allZNotZero.Remove(prefab);
                            PrefabUtility.SaveAsPrefabAsset(prefab, _preblemPrefabPaths[i]);
                            DestroyImmediate(prefab);
                            _preblemPrefabPaths.RemoveAt(i);
                            _allKey.RemoveAt(i);
                            i--;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }
    #endregion

    #region Func
    private void CheckAllPrefabs()
    {
        if (string.IsNullOrEmpty(_rootPath))
            return;
        if (!Directory.Exists(_rootPath))
            return;

        foreach (GameObject obj in _allZNotZero.Keys)
        {
            if (obj != null)
                DestroyImmediate(obj);
        }
        _allZNotZero.Clear();
        _preblemPrefabPaths.Clear();

        string[] searchFolder = { _rootPath.Substring(_rootPath.IndexOf("Assets", StringComparison.Ordinal)) };
        string[] allAssetsGuid = AssetDatabase.FindAssets("t:prefab", searchFolder);
        int corrCount = UITools.RemoveRepeatGuid(ref allAssetsGuid);
        float fProgress = 0;
        string strAll = "/" + corrCount;
        for (int i = 0; i < corrCount; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
            GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            fProgress++;
            EditorUtility.DisplayProgressBar("正在查找" + fProgress.ToString() + strAll, "******等等等等******", fProgress / corrCount);
            if (gameObj == null)
                continue;
            CheckPrefabUiZValueNotZero(gameObj, assetPath);
        }
        EditorUtility.ClearProgressBar();
    }
    private static void CheckPrefabUiZValueNotZero(GameObject gameObj, string prefabPath)
    {
        GameObject newObj = PrefabUtility.InstantiatePrefab(gameObj) as GameObject;
        if (newObj == null)
            return;
        RectTransform[] allRTrans = newObj.GetComponentsInChildren<RectTransform>();
        List<RectTransform> allObj = null;
        bool isPrefabOk = true;
        foreach (RectTransform rTrans in allRTrans)
        {
            if (Math.Abs(rTrans.anchoredPosition3D.z) > 0.01f)
            {
                if (allObj == null)
                {
                    allObj = new List<RectTransform>();
                    _allZNotZero.Add(newObj, allObj);
                    isPrefabOk = false;
                }
                allObj.Add(rTrans);
            }
        }
        if (!isPrefabOk)
            _preblemPrefabPaths.Add(prefabPath);
        else
            DestroyImmediate(newObj);
    }
    private void AutoCorrectAllPrefab()
    {
        _allKey = _allZNotZero.Keys.ToList();
        for (int i = 0; i < _allZNotZero.Count; i++)
        {
            AutoCorrectThisPrefab(_allKey[i], ref i);
        }
    }
    private void AutoCorrectThisPrefab(GameObject obj, ref int index)
    {
        // 异常预制体处理
        List<RectTransform> allChildRTrans = _allZNotZero[obj];
        Vector3 posTemp;
        foreach (RectTransform rTrans in allChildRTrans)
        {
            posTemp = rTrans.anchoredPosition3D;
            posTemp.z = 0;
            rTrans.anchoredPosition3D = posTemp;
        }
        PrefabUtility.SaveAsPrefabAsset(obj, _preblemPrefabPaths[index]);
        
        CurrentPrefabIsDone(obj, ref index);
    }
    private void CurrentPrefabIsDone(GameObject obj, ref int index)
    {
        _allZNotZero.Remove(obj);
        DestroyImmediate(obj);
        _preblemPrefabPaths.RemoveAt(index);
        _allKey.RemoveAt(index);
        index--;
    }
    #endregion
}
