using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RenderTextureWrapper : MonoBehaviour
{
    public string renderTextureName = "";

	// Use this for initialization
	void Start ()
    {
        _Binding();
    }

    
    void OnEnable()
    {
        _Binding();
    }

    public void Rebind()
    {
        _Binding();
    }

    protected void _Binding()
    {

        IGeRenderTexture rendTex = GeRenderTextureManager.instance.FindRenderTextureByName(renderTextureName);
        if (null != rendTex)
        {
            RawImage image = gameObject.transform.GetComponentInChildren<RawImage>();
            if (null != image)
                image.texture = rendTex.GetRenderTexture();
            else
                Logger.LogErrorFormat("Prefab {0} does not contain an RawImage component!",gameObject.name);
        }
    }
}
