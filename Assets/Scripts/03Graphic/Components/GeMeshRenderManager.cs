using UnityEngine;
using System.Collections.Generic;

public class GeMeshRenderManager : Singleton<GeMeshRenderManager>
{
    public class GeMeshRenderDesc
    {
        public Renderer m_Renderer = null;
        public Canvas m_Canvas = null;
        public Vector3 m_Center = Vector3.zero;
        public float m_ZDepth = 0.0f;
        public int m_unOriginOrder = 0;
        public GameObject m_RootObject = null;
    }

    public class GeMeshObjectDesc
    {
        public GameObject m_RootObject = null;
    }

    #region 方法

    public bool Init(GameObject mainCamera)
    {
		//因为之前AddMeshObject里面已经Return了
        return true;
        if (null == mainCamera)
            return false;

        m_CameraNode = mainCamera;

        m_Camera = m_CameraNode.GetComponent<Camera>();
        if (null == m_Camera)
            return false;

        Camera.onPreRender += CameraPreRender;

        return true;
    }

    public void Deinit()
    {
		//因为之前AddMeshObject里面已经Return了
        return;
        Camera.onPreRender -= CameraPreRender;
        m_MeshRenderList.RemoveAll(
            e =>
            {
                if (null != e.m_Renderer)
                    e.m_Renderer.sortingOrder = e.m_unOriginOrder;
                if (null != e.m_Canvas)
                    e.m_Canvas.sortingOrder = e.m_unOriginOrder;

                return true;
            }
            );
        m_MeshObjectList.Clear();
    }

    public void AddMeshObject(GameObject gameObject)
    {
		//先注掉（不用了）
		return;

        if(null == gameObject)
        {
            Logger.LogWarning("gameObject can not be null for transparent sorting");
            return;
        }

        if (null == m_CameraNode)
            return;

        bool bMeshRenderDirty = false;
        m_MeshObjectList.RemoveAll(
            e =>
            {
                if(gameObject == e.m_RootObject)
                {
                    bMeshRenderDirty = true;
                    return true;
                }
                else
                    return false;
            }
            );

        if(bMeshRenderDirty)
        {
            m_MeshRenderList.RemoveAll(
                e =>
                {
                    if (e.m_RootObject == gameObject)
                    {
                        if (null != e.m_Renderer)
                            e.m_Renderer.sortingOrder = e.m_unOriginOrder;
                        if (null != e.m_Canvas)
                            e.m_Canvas.sortingOrder = e.m_unOriginOrder;

                        return true;
                    }
                    else
                        return false;
                }
                );
        }

        int unCount = 0;
        Renderer[] amr = gameObject.GetComponentsInChildren<Renderer>();
        for(int i = 0; i < amr.Length; ++ i)
        {
            GeMeshRenderDesc newMeshRendDesc = new GeMeshRenderDesc();

            if(amr[i].material.renderQueue < 3000)
                continue;

            newMeshRendDesc.m_Renderer = amr[i];
            newMeshRendDesc.m_RootObject = gameObject;
            newMeshRendDesc.m_unOriginOrder = newMeshRendDesc.m_Renderer.sortingOrder;

            ++unCount;

            m_MeshRenderList.Add(newMeshRendDesc);
        }

        Canvas[] ac = gameObject.GetComponentsInChildren<Canvas>();
        for (int i = 0; i < ac.Length; ++i)
        {
            GeMeshRenderDesc newMeshRendDesc = new GeMeshRenderDesc();

            newMeshRendDesc.m_Canvas = ac[i];
            newMeshRendDesc.m_RootObject = gameObject;
            newMeshRendDesc.m_unOriginOrder = newMeshRendDesc.m_Canvas.sortingOrder;

            ++unCount;

            m_MeshRenderList.Add(newMeshRendDesc);
        }

        if (m_MeshRenderList.Count > 0)
        {
            GeMeshObjectDesc newMeshObjDesc = new GeMeshObjectDesc();
            newMeshObjDesc.m_RootObject = gameObject;
            m_MeshObjectList.Add(newMeshObjDesc);
        }
    }

    public void Sort()
    {
        return;

        for (int i = 0; i < m_MeshRenderList.Count; ++i)
        {
            GeMeshRenderDesc curMeshRendDesc = m_MeshRenderList[i];
            if(null != curMeshRendDesc.m_Canvas)
            {
                curMeshRendDesc.m_Center = curMeshRendDesc.m_Canvas.gameObject.transform.position;
            }
            if (null != curMeshRendDesc.m_Renderer)
            {
                curMeshRendDesc.m_Center = curMeshRendDesc.m_Renderer.bounds.center;
            }

            curMeshRendDesc.m_ZDepth = curMeshRendDesc.m_Center.z - m_CameraNode.transform.position.z;
        }

        m_MeshRenderList.Sort(
            (_Left,_Right) =>
            {
                if (_Left.m_ZDepth < _Right.m_ZDepth)
                    return 1;
                else if (_Left.m_ZDepth == _Right.m_ZDepth)
                    return 0;
                else
                    return -1;
            }
            );

        for (int i = 0; i < m_MeshRenderList.Count; ++i)
        {
            if(null != m_MeshRenderList[i].m_Renderer)
                m_MeshRenderList[i].m_Renderer.sortingOrder = i + m_MeshRenderList[i].m_unOriginOrder;
            if(null != m_MeshRenderList[i].m_Canvas)
                m_MeshRenderList[i].m_Canvas.sortingOrder = i + m_MeshRenderList[i].m_unOriginOrder;
        }
    }

    protected void CameraPreRender(Camera cam)
    {
        if (null != m_Camera && cam == m_Camera)
            Sort();
    }

    #endregion

    #region 成员

    protected List<GeMeshObjectDesc> m_MeshObjectList = new List<GeMeshObjectDesc>();
    protected List<GeMeshRenderDesc> m_MeshRenderList = new List<GeMeshRenderDesc>();
    protected GameObject m_CameraNode = null;
    protected Camera m_Camera = null;

    #endregion
}
