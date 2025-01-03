using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour {

	Vector3 initPos;
	bool bUpdate = false;
	public Vector3 UpPosition;
	public AnimationCurve doCurve;
	public float time;
	float beginTime;
	// Use this for initialization
	void Start () {
		initPos = gameObject.transform.localPosition;
	}
	
	public void SetUp()
	{
		bUpdate = false;
		gameObject.transform.localPosition = UpPosition;
	}

	public void Begin()
	{
		bUpdate = true;
		beginTime = 0.0f;
	}
	// Update is called once per frame
	void Update()
	{
		if(bUpdate == false)
		{
			return;
		}

		beginTime += Time.deltaTime;

		if(beginTime > time)
		{
			bUpdate = false;
			gameObject.transform.localPosition = initPos;
		}
		else
		{
			var t =  Mathf.Repeat(beginTime,time) / time;
			t = doCurve.Evaluate(t);
			gameObject.transform.localPosition = Vector3.Lerp(UpPosition, initPos,t);
		}
	}
}
