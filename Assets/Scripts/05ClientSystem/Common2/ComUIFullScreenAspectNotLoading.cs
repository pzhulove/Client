using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ComUIFullScreenAspectNotLoading : MonoBehaviour 
{
	// Use this for initialization
	void Start ()
	{
		_updateSize ();
	}

	private void _updateSize()
	{
		RectTransform rect = this.GetComponent<RectTransform>();

		if (null == rect) {
			return ;
		}


        CameraAspectAdjust.eScreenType type = CameraAspectAdjust.GetScreenType ();

        Vector2 size = ComUIFullScreenAspectUtility.GetScreenDeltaSizeEachSizeX(type);
        rect.sizeDelta = new Vector2(size.y, size.x);
	}

#if UNITY_EDITOR
	void Update()
	{
		_updateSize ();
	}
#endif
}
