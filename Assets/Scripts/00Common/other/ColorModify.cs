using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
    [ExecuteAlways]
#endif
public class ColorModify : MonoBehaviour {

    public Color tintColor = Color.white;
    [HideInInspector]
    private MeshRenderer mesh_renderer;
    // Use this for initialization
    void Start () {
        mesh_renderer = gameObject.GetComponent<MeshRenderer>();
       
	}
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        mesh_renderer.sharedMaterial.SetColor("_TintColor", tintColor);
#else
        mesh_renderer.sharedMaterial.SetColor("_TintColor", tintColor);
#endif
    }
}
