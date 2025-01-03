using UnityEngine;
using System.Collections;

public class FadInOut : MonoBehaviour {
	
	public float life = 2.0f;
	
	float startTime;
	Material mat = null;
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if (!meshRenderer || !meshRenderer.material)
		{
			base.enabled = false;
		}
		else
		{
			mat = meshRenderer.material;
			if(mat)
			{
				mat.shader = AssetShaderLoader.Find("Shader Forge/Ghostshader");
			}
			mat.SetColor("_Color",Color.black);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float time = Time.time - startTime;
		if (time > life)
		{
			DestroyImmediate(gameObject);
		}
		else
		{
			float remainderTime = life - time;
			if (mat)
			{
				Color col = mat.GetColor("_Color");
				col = Color.black;
				col.a = remainderTime;
				mat.SetColor("_Color", col);
			}
		}
	}
}