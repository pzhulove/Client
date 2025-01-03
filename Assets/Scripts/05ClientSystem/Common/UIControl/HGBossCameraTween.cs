using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class HGBossCameraTween : MonoBehaviour
{
	public delegate void HGBossCameraTweenFinishCallBack (bool from);

    public AnimationCurve positionCurve;
    public AnimationCurve scaleCurve;

    private Vector3        targetPos;
    public float          targetSize;

    public float          timeLength = 3.0f;
	public string 		  bgPrefab = "";

	public float blackTime = 0.6f;
	public float blackFadeOutTime = 0.5f;

    private Camera        cachedCamera;
    private Transform     cachedTransform;

    private Vector3       cachedPosition;
    private float         cachedSize;

    private float         beginTime;

    private bool          bUpdate;
	private bool 		  from;

	public HGBossCameraTweenFinishCallBack callback = null;


    void Awake()
    {
        cachedCamera = gameObject.GetComponent<Camera>();
        cachedTransform = gameObject.transform;
        cachedPosition  = Vector3.zero;//cachedTransform.localPosition;
        cachedSize      = 3.0f;
    }   

    IEnumerator UpdateTween()
    {
        float time = Time.realtimeSinceStartup - beginTime;

        while(time < timeLength)
        {
            time = Mathf.Repeat(time,timeLength) / timeLength;
            
			if (from)
			{
				float t1 = positionCurve.Evaluate(time);
				cachedTransform.localPosition = Vector3.Lerp(cachedPosition, targetPos,t1);
				float t2 = scaleCurve.Evaluate(time);
				cachedCamera.orthographicSize  = Mathf.Lerp(cachedSize, targetSize,t2);
			}
			else {

				float t1 = positionCurve.Evaluate(time);
				cachedTransform.localPosition = Vector3.Lerp(targetPos,cachedPosition,t1);
				float t2 = scaleCurve.Evaluate(time);
				cachedCamera.orthographicSize  = Mathf.Lerp(targetSize,cachedSize,t2);

			}
            
            yield return Yielders.EndOfFrame;
            time = Time.realtimeSinceStartup - beginTime;

			Logger.LogErrorFormat("camera:({0},{1},{2})", cachedTransform.localPosition.x, cachedTransform.localPosition.y, cachedTransform.localPosition.z);
        }

		if (callback != null)
			callback(from);

        cachedTransform.localPosition = cachedPosition;
        cachedCamera.orthographicSize = cachedSize;

        bUpdate = false;
    }

	public void StartTween(bool from=true)
    {
        //return;

		this.from = from;
        
        if(bUpdate == false)
        {
            bUpdate = true;
            beginTime = Time.realtimeSinceStartup;
            cachedSize  = cachedCamera.orthographicSize;
            StartCoroutine(UpdateTween());
        }
    }

	public void SetStartPos(Vector3 targetPos)
	{
		this.targetPos = targetPos;
		cachedTransform.localPosition = targetPos;
		cachedCamera.orthographicSize = targetSize;
	}

	public void RestorePos()
	{
		cachedTransform.localPosition = cachedPosition;
		cachedCamera.orthographicSize = cachedSize;
	}
}