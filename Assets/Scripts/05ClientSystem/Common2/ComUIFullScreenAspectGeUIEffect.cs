using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ComUIFullScreenAspectGeUIEffect : MonoBehaviour 
{
    private float originScaleX = 1.0f;

	// Use this for initialization
	void Start () 
    {
        originScaleX = this.transform.localScale.x;
        _updateSize();
	}

	
	void _updateSize () 
    {
        CameraAspectAdjust.eScreenType type = CameraAspectAdjust.GetScreenType();

        float rate = ComUIFullScreenAspectUtility.GetScreenDeltaRate(type);
        Vector3 localScale = this.transform.localScale;
        this.transform.localScale = new Vector3(originScaleX * rate, localScale.y, localScale.z);
	}

#if UNITY_EDITOR
	void Update()
	{
		_updateSize ();
	}
#endif
}
