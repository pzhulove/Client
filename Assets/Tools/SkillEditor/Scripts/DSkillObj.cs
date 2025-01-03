#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class DEditorObj
{
    protected GameObject _rootObject;
    protected GameObject _gameObjectPrefab;
    protected GameObject _gameObject;
    protected bool _bCreate = false;

    public float timelength = 1.0f;

    public bool visible = true;
    protected DSkillFrameGrap _data;
    public DEditorObj(DSkillFrameGrap data)
    {
        _data = data;
    }

    public GameObject RootObject
    {
        get { return _rootObject; }
    }

    public void Show(bool bShow)
    {
        if (!_bCreate)
        {
            Create();
        }

        if (bShow && visible)
            _rootObject.CustomActive(true);
        else
            _rootObject.CustomActive(false);

    }

    public void Destroy()
    {
        if (_rootObject)
        {
            _rootObject.transform.SetParent(null, true);
            Editor.DestroyImmediate(_rootObject);
        }
        _gameObject = null;
        _rootObject = null;
        _bCreate = false;
    }

    void Create()
    {
        if (_data.op != DSFGrapOp.GRAP_CHANGE_TARGETPOS)
            return;

        if (true)
        {
            if (_rootObject == null)
            {
                _rootObject = new GameObject();
                _rootObject.name = "Grab_prefab";

                var obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tools/SkillEditor/Cube.prefab");

                _gameObject = (GameObject)PrefabUtility.InstantiatePrefab(obj);
                _gameObject.transform.SetParent(_rootObject.transform, true);
                _rootObject.transform.SetParent(DSkillData.actor.transform, false);
            }
        }
        _bCreate = true;
    }
    public void Sampler(float fTime)
    {
        if (_bCreate == false)
        {
            Create();
        }

        if (_rootObject)
        {
            _rootObject.transform.localPosition = _data.targetPos.vector3();
        }
    }
}

#endif