using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 脚本建议挂在UIRoot
/// </summary>
public class UiDirtyAnalysis : MonoBehaviour
{
    #region Params
    private List<UiChildDirtyCheck> _allChild;
    private Dictionary<Canvas, List<string>> _dirtyCanvasToObjNameMap;
    private bool _isRegister;
    #endregion

    #region Singeleton
    public static UiDirtyAnalysis Instance { get; private set; }
    private void Start()
    {
        if (Instance != null)
        {
            Instance.CancelAllRegister();
            Destroy(Instance);
        }
        Instance = this;
        _allChild = new List<UiChildDirtyCheck>();
        _dirtyCanvasToObjNameMap = new Dictionary<Canvas, List<string>>();
        _isRegister = false;
    }
    #endregion

    #region Mono
    private void Update()
    {
        if (_isRegister)
        {
            if (_dirtyCanvasToObjNameMap.Count == 0)
                return;
            Debug.Log("UGUI刷新帧号:" + Time.frameCount);
            //Debug.Log("UGUI刷新帧号:");
            foreach (KeyValuePair<Canvas, List<string>> keyValuePair in _dirtyCanvasToObjNameMap)
            {
                Debug.Log("Canvas: " + (keyValuePair.Key == null ? "空" : keyValuePair.Key.name) + "因以下组件被刷新：");
                foreach (string objPath in keyValuePair.Value)
                {
                    Debug.Log(objPath);
                }
            }
            _dirtyCanvasToObjNameMap.Clear();
        }
    }
    #endregion

    #region InspectorEvent
    public void RegisterAll()
    {
        // 为防止动态增删Ui强制刷新
        CancelAllRegister();
        Graphic[] allGraphics = GetComponentsInChildren<Graphic>();
        int graphicCount = allGraphics.Length;
        _allChild = new List<UiChildDirtyCheck>();
        for (int i = 0; i < graphicCount; i++)
        {
            UiChildDirtyCheck child = allGraphics[i].gameObject.AddComponent<UiChildDirtyCheck>();
            _allChild.Add(child);
            child.RegisterDirtyEvent();
        }
        _isRegister = true;
    }
    public void CancelAllRegister()
    {
        _isRegister = false;
        if (_allChild.Count == 0)
            return;
        foreach (UiChildDirtyCheck child in _allChild)
        {
            if (child == null)
                continue;
            child.CancelDirtyEvent();
            Destroy(child);
        }
        _allChild.Clear();
    }
    #endregion

    #region Func_UpdateData
    public void UpdateDirtyCanvasInfo(Canvas canvas, string objPath)
    {
        List<string> pathList;
        if (!_dirtyCanvasToObjNameMap.TryGetValue(canvas, out pathList))
        {
            pathList = new List<string>();
            _dirtyCanvasToObjNameMap.Add(canvas, pathList);
        }
        if (!pathList.Contains(objPath))
            pathList.Add(objPath);
    }
    #endregion
}
