using UnityEngine;
using System.Collections.Generic;

public class GeOffscreenRenderer
{
    protected IGeRenderTexture m_RenderTexture = null;
    protected Light m_DirectionalLight = null;
    protected Camera m_AvatarCamera = null;
    protected List<GameObject> m_RenderObjList = new List<GameObject>();
    protected GameObject m_OffscreenRoot = null;
    protected int m_RenderLayer = 0;

    public bool Init(string rendTextureNode, int width, int height, int layer = 12)
    {
        if (null == m_OffscreenRoot)
            m_OffscreenRoot = new GameObject("OffscreenRender");

        m_RenderTexture = GeRenderTextureManager.instance.CreateRenderTexture(rendTextureNode, layer, width, height);
        if (null != m_RenderTexture)
        {
            m_DirectionalLight = m_RenderTexture.GetLight();

            m_AvatarCamera = m_RenderTexture.GetCamera();
            if (null != m_AvatarCamera)
            {
                m_RenderLayer = layer;
                return true;
            }
            else
                Logger.LogWarning("Get camera form render texture has failed!");
        }
        else
            Logger.LogWarning("create render texture has failed!");

        return false;
    }

    public void Deinit()
    {
        ClearAll();

        if (null != m_RenderTexture)
        {
            GeRenderTextureManager.instance.DestroyRenderTexture(m_RenderTexture);

            m_RenderTexture = null;
            m_DirectionalLight = null;
            m_AvatarCamera = null;
        }

        if (m_OffscreenRoot)
        {
            GameObject.Destroy(m_OffscreenRoot);
            m_OffscreenRoot = null;
        }
    }

    public void AddRenderObject(GameObject go)
    {
        if(!m_RenderObjList.Contains(go))
        {
            go.layer = m_RenderLayer;
            Renderer[] ar = go.GetComponentsInChildren<Renderer>();
            for(int i = 0; i < ar.Length; ++i)
            {
                if (null != ar[i])
                    ar[i].gameObject.layer = m_RenderLayer;
            }

            m_RenderObjList.Add(go);
        }
    }

    public void RemoveRenderObject(GameObject go)
    {
        m_RenderObjList.RemoveAll(
            f =>
            {
                return go == f;
            });
    }

    public void ClearAll()
    {
        m_RenderObjList.Clear();
    }
}
