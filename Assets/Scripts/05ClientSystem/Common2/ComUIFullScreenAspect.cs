using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ComUIFullScreenAspect : MonoBehaviour 
{
	// Use this for initialization
	void Start ()
	{
		_updateSize ();
	}

	private static Vector2 half = new Vector2(0.5f, 0.5f);
		
	private void _updateSize()
	{
		RectTransform rect = this.GetComponent<RectTransform>();

		if (null == rect) {
			return ;
		}

		rect.anchorMin = half;
		rect.anchorMax = half;
		rect.pivot     = half;

		CameraAspectAdjust.eScreenType type = CameraAspectAdjust.GetScreenType ();
        rect.sizeDelta = ComUIFullScreenAspectUtility.GetScreenDeltaSize(type);
	}

#if UNITY_EDITOR
	void Update()
	{
		_updateSize ();
	}
#endif
}
