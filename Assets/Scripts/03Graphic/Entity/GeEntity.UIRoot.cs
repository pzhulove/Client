using UnityEngine;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// UI相关
/// </summary>
public partial class GeEntity 
{
    private List<GameObject> _entityUIList = new List<GameObject>();

    public void SetActorUIRootPos(Vector3 pos)
    {
        for(int i=0;i< _entityUIList.Count; i++)
        {
            if (_entityUIList[i] == null)
                continue;
            _entityUIList[i].transform.position = pos;
        }
    }

    protected void AttachToRootNode(GameObject go, SceneUINodeType type)
    {
#if !LOGIC_SERVER
        if (go == null) return;
        //添加一个父节点
        var nameGo = new GameObject();
        if (m_EntitySpaceDesc.rootNode != null)
        {
            nameGo.transform.position = m_EntitySpaceDesc.rootNode.transform.position;
            nameGo.name = m_EntitySpaceDesc.rootNode.name;
        } 
        Battle.GeUtility.AttachTo(go, nameGo);

        _RealAttach(nameGo, type);
#endif
    }

    protected Transform GetShadowParentNodeAndAttach()
    {
#if !LOGIC_SERVER
        var nameGo = new GameObject();
        if (m_EntitySpaceDesc.rootNode != null)
        {
            nameGo.transform.position = m_EntitySpaceDesc.rootNode.transform.position;
            nameGo.name = m_EntitySpaceDesc.rootNode.name;
        }

        _RealAttach(nameGo, SceneUINodeType.ActorRoot_Shadow);

        return nameGo.transform;
#else
        return null;
#endif

    }

    private void _RealAttach(GameObject obj, SceneUINodeType type)
    {
#if !LOGIC_SERVER
        if (m_Scene == null) return;
        m_Scene.AttachUIRoot(obj, type);
        _entityUIList.Add(obj);
#endif
    }

    protected void RemoveEntityUIRoot()
    {
        for(int i=0;i< _entityUIList.Count; i++)
        {
            var obj = _entityUIList[i];
            if (obj == null)
                continue;
            GameObject.Destroy(obj);
        }
        _entityUIList.Clear();
    }

    /// <summary>
    /// 显示或隐藏UI
    /// </summary>
    public void SetUIVisible(bool isShow)
    {
        for (int i = 0; i < _entityUIList.Count; i++)
        {
            var obj = _entityUIList[i];
            if (obj == null)
                continue;
            obj.CustomActive(isShow);
        }
    }
}
