using UnityEngine;
using UnityEngine.UI;

public class UiChildDirtyCheck : MonoBehaviour
{
    #region Params
    private Graphic _selfGraph;
    private string _selfPath;
    private Canvas _parentCanvas;
    #endregion

    #region Func_Event
    // 暂不合并，以后可能会打印详细信息
    private void DirtyLayoutCallBack()
    {
        //Debug.Log("DirtyLayoutCallBack   " + _selfPath);
        //UiDirtyAnalysis.Instance.UpdateDirtyCanvasInfo(_parentCanvas, _selfPath);
        UiDirtyAnalysis.Instance.UpdateDirtyCanvasInfo(_parentCanvas, "DirtyLayoutCallBack   " + _selfPath);
    }
    private void DirtyVerticesCallBack()
    {
        //Debug.Log("DirtyVerticesCallBack   " + _selfPath);
        //UiDirtyAnalysis.Instance.UpdateDirtyCanvasInfo(_parentCanvas, _selfPath);
        UiDirtyAnalysis.Instance.UpdateDirtyCanvasInfo(_parentCanvas, "DirtyVerticesCallBack   " + _selfPath);
    }
    private void DirtyMaterialCallBack()
    {
        //Debug.Log("DirtyMaterialCallBack   " + _selfPath);
        //UiDirtyAnalysis.Instance.UpdateDirtyCanvasInfo(_parentCanvas, _selfPath);
        UiDirtyAnalysis.Instance.UpdateDirtyCanvasInfo(_parentCanvas, "DirtyMaterialCallBack   " + _selfPath);
    }
    #endregion

    #region Func
    public void RegisterDirtyEvent()
    {
        if (_selfGraph == null)
        {
            _selfGraph = GetComponent<Graphic>();
            _selfPath = GetSelfPath(transform);
        }
        _selfGraph.RegisterDirtyLayoutCallback(DirtyLayoutCallBack);
        _selfGraph.RegisterDirtyVerticesCallback(DirtyVerticesCallBack);
        _selfGraph.RegisterDirtyMaterialCallback(DirtyMaterialCallBack);
    }
    public void CancelDirtyEvent()
    {
        _selfGraph.UnregisterDirtyLayoutCallback(DirtyLayoutCallBack);
        _selfGraph.UnregisterDirtyVerticesCallback(DirtyVerticesCallBack);
        _selfGraph.UnregisterDirtyMaterialCallback(DirtyMaterialCallBack);
    }
    private string GetSelfPath(Transform transObj)
    {
        string path = "";
        if (transObj == null)
        {
            _parentCanvas = null;
            return path;
        }
        path = transObj.name;
        _parentCanvas = transObj.GetComponent<Canvas>();
        Transform parent = transObj.parent;
        while (parent && parent.GetComponent<UiDirtyAnalysis>() == null)
        {
            if (_parentCanvas == null)
                _parentCanvas = parent.GetComponent<Canvas>();
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        if (_parentCanvas == null && parent != null)
            _parentCanvas = parent.GetComponent<Canvas>();
        return path;
    }
    #endregion
}
