using System;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using UnityEngine.Rendering;

public enum SceneUINodeType
{
    ActorRoot,
    ActorRoot_InfoHead,
    ActorRoot_DungeonHpBar,
    ActorRoot_Shadow,
    ActorRoot_DungeonBar,
    ActorRoot_FootEffect,

    DropItemRoot,
    DropItem_TextBg,
    DropItem_Icon,
    DropItem_Name,
    DropItem_SpecialDesc,
    DropItem_Effect,

    Count,
}

public partial class GeSceneEx
{
    private GameObject _sceneUIRootGo = null;
    private Dictionary<int, GameObject> _sceneUIRootDic = new Dictionary<int, GameObject>();

    private void _LoadSceneUIRoot(GameObject root)
    {
        if (_sceneUIRootGo != null) return;

        _sceneUIRootGo = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/SceneUIRoot");
        _sceneUIRootGo.transform.SetParent(root.transform, false);
        for (int i = 0; i < (int)SceneUINodeType.Count; i++)
        {
            _CreateSceneUIRoot((SceneUINodeType)i);
        }
    }

    private void _UnLoadSceneUIRoot()
    {
        if (_sceneUIRootGo == null) return;

        GameObject.Destroy(_sceneUIRootGo);
        _sceneUIRootGo = null;
        _sceneUIRootDic.Clear();
    }

    public void AttachUIRoot(GameObject go, SceneUINodeType type)
    {
        var root = GetUIRootByType(type);
        if (root == null) return;
        Battle.GeUtility.AttachTo(go, root);
    }

    private GameObject GetUIRootByType(SceneUINodeType type)
    {
        int key = (int)type;
        if (!_sceneUIRootDic.ContainsKey(key)) return null;
        return _sceneUIRootDic[key];
    }

    private void _CreateSceneUIRoot(SceneUINodeType type)
    {
        int key = (int)type;
        if(_sceneUIRootDic.ContainsKey(key))return;

        switch (type)
        {
            case SceneUINodeType.ActorRoot:
            case SceneUINodeType.DropItemRoot:
                CreateSingleUI(type, _sceneUIRootGo, true);
                break;
            case SceneUINodeType.ActorRoot_DungeonBar:
            case SceneUINodeType.ActorRoot_DungeonHpBar:
            case SceneUINodeType.ActorRoot_FootEffect:
            case SceneUINodeType.ActorRoot_InfoHead:
            case SceneUINodeType.ActorRoot_Shadow:
                CreateSingleUI(type, GetUIRootByType(SceneUINodeType.ActorRoot));
                break;
            case SceneUINodeType.DropItem_Effect:
            case SceneUINodeType.DropItem_Icon:
            case SceneUINodeType.DropItem_Name:
            case SceneUINodeType.DropItem_TextBg:
            case SceneUINodeType.DropItem_SpecialDesc:
                CreateSingleUI(type, GetUIRootByType(SceneUINodeType.DropItemRoot));
                break;
        }
    }

    private void CreateSingleUI(SceneUINodeType type, GameObject parent, bool needAddSortingGroup = false)
    {
        if(parent==null)return;

        int key = (int)type;
        string name = Enum.GetName(typeof(SceneUINodeType), key);

        var obj = new GameObject();
        obj.name = name;
        if (needAddSortingGroup)
            obj.AddComponent<SortingGroup>();
        Battle.GeUtility.AttachTo(obj, parent);
        _sceneUIRootDic.Add(key, obj);
    }
}