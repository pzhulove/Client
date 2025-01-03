using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public delegate void PostLoadProcess(Image img);

public class ImageAsyncLoadProxy : MonoBehaviour
{
    uint m_Handle = ~0u;
    public Image m_Image = null;
    uint m_CheckTick = 0;
    PostLoadProcess m_PostProcess = null;

    public void LoadResAsync(string res,PostLoadProcess postProcess = null)
    {
        if (~0u != m_Handle)
            AssetLoader.instance.AbortRequest(m_Handle);

        if (null == m_Image)
            m_Image = gameObject.GetComponent<Image>();

        if (null != m_Image)
        {
            if(!enabled)
                this.enabled = true;

            m_Handle = AssetLoader.instance.LoadResAync(res, typeof(Sprite));
            m_PostProcess = postProcess;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (~0u != m_Handle)
        {
            if (m_CheckTick < 3)
            {
                ++m_CheckTick;
                return;
            }
            else
                m_CheckTick = 0;

            if (AssetLoader.instance.IsRequestDone(m_Handle))
            {
                Sprite sp = AssetLoader.instance.Extract(m_Handle).obj as Sprite;
                if (null != m_Image)
                {
                    m_Image.sprite = sp;
                    if (null != m_PostProcess)
                        m_PostProcess(m_Image);
                }
                m_PostProcess = null;
                m_Handle = ~0u;
            }
        }
        else
            this.enabled = false;
    }

    void OnDestroy()
    {
        if(~0u != m_Handle)
        {
            AssetLoader.instance.AbortRequest(m_Handle);
            m_Handle = ~0u;
            m_PostProcess = null;
        }

        m_Image = null;
    }

    static public void AddAsyncLoadResWithGameObject(GameObject go,string res, PostLoadProcess postProcess = null)
    {
        if(null != go)
        {
            ImageAsyncLoadProxy imageAsync = go.GetComponent<ImageAsyncLoadProxy>();
            if (null == imageAsync)
                imageAsync = go.AddComponent<ImageAsyncLoadProxy>();
            if (null != imageAsync)
            {
                if (null == imageAsync.m_Image)
                    imageAsync.m_Image = go.GetComponent<Image>();

                imageAsync.LoadResAsync(res, postProcess);
            }
        }
    }
}
