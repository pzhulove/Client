using UnityEngine;
using System.Collections;

public class RenderTextureLoader : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Camera cam = gameObject.transform.GetComponentInChildren<Camera>();
        if(null != cam)
        {
            RenderTexture rendTarget = AssetLoader.instance.LoadRes("UI/Material/" + gameObject.name, typeof(RenderTexture)).obj as RenderTexture;
            if(null != rendTarget)
                cam.targetTexture = rendTarget;
        }
    }
}
