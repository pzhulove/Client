using UnityEngine;
using System.Collections;

public class TilingChange : MonoBehaviour {

	public Material material;
	public Vector2 offStart = new Vector2(0.7f, 1.0f);
	public Vector2 offFinal = new Vector2(2.5f, 1.0f);
	public Vector2 speed =  new Vector2(0.0004f, 0f);
	private Vector2 startOffset = new Vector2();

	void Start()
	{
		material = GetComponent<Renderer>().material;
		startOffset = offStart;
	}

	void Update()
	{
		startOffset.x += speed.x * (offFinal.x - offStart.x);
		startOffset.y += speed.y * (offFinal.y - offStart.y);

		if (startOffset.x >= offFinal.x)
			return;

		material.SetVector("_MainTex_ST", startOffset);
	}
}
