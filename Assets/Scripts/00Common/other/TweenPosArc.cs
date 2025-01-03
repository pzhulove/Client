using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class TweenPosArc : MonoBehaviour {

	public GameObject targetPosition;
	public bool NeedFindTarget = true;
	private GameObject goPosition;

	public UnityEvent onFinish = new UnityEvent();
	public float delayTime = 0f;//0.2f;

	private float countDelayTime = 0;

	private List<Vector3> point = new List<Vector3>();
	private Vector3 gamePointInit;
	private Vector3 gamePoint1;
	private Vector3 gamePoint2;
	private Vector3 gamePoint3;
	private Vector3 gamePoint4;
	private Vector3 gamePoint5;
	private Transform goldPosition;
	public float TimeSoeed = 0.001f;
	private float Timer = 0;
	private bool isPlay = true;
	private bool isRemove = true;
	private float percent = 0.35f;
	private float radius1 = 2.0f;
	private float radius2 = 3.0f;
	private float radiusX1;
	private float radiusY1;
	private float radiusX2;
	private float radiusY2;
	private float heigaht2;
	private float randomAngle1;
	private float randomAngle2;
	private float randomAngle;
	public float conmat = 15.0f;

	int i = 1;

	void Start()
	{
		countDelayTime = delayTime;

		goldPosition = this.transform;
		gamePointInit = goldPosition.transform.position;

		randomAngle1 = Random.Range (60,120);
		randomAngle2 = Random.Range (-60,-120);
		float[] floatNum = new float[]{randomAngle1,randomAngle2};
		randomAngle = floatNum[Random.Range(0,floatNum.Length)];

		radiusX1 = Mathf.Sin (randomAngle * Mathf.PI/180) * radius1;
		radiusY1 = Mathf.Cos (randomAngle * Mathf.PI/180) * radius1;
		radiusX2 = Mathf.Sin (randomAngle * Mathf.PI/180) * radius2;
		radiusY2 = Mathf.Cos (randomAngle * Mathf.PI/180) * radius2;
		heigaht2 = targetPosition.transform.position.y * percent;

		if (targetPosition) {
			Init ();
		} else {
			Debug.LogWarning("没有查到追寻物体");
		}

	}

	public void Init()
	{
		goPosition = NeedFindTarget ? Utility.FindThatChild("Body", targetPosition) : targetPosition;
		if (goPosition == null)
			return;

		countDelayTime = delayTime;
		mState = eTweenPosState.None;

		gamePoint1 = gamePointInit;
		gamePoint2 = new Vector3 (gamePointInit.x + radiusX1,gamePointInit.y,gamePointInit.z + radiusY1);
		gamePoint3 = new Vector3 (gamePointInit.x + radiusX2,gamePointInit.y + heigaht2,gamePointInit.z + radiusY2);
		gamePoint4 = goPosition.transform.position;
		gamePoint5 = goPosition.transform.position;
		point = new List<Vector3>();
		for (int i = 0; i < 200; i++)
		{           
			Vector3 pos1 = Vector3.Lerp(gamePoint1, gamePoint2, i / conmat);
			Vector3 pos2 = Vector3.Lerp(gamePoint2, gamePoint3, i / conmat);
			Vector3 pos3 = Vector3.Lerp(gamePoint3, gamePoint4, i / conmat);
			Vector3 pos4 = Vector3.Lerp(gamePoint4, gamePoint5, i / conmat);

			var pos1_0 = Vector3.Lerp(pos1, pos2, i / conmat);
			var pos1_1 = Vector3.Lerp(pos2, pos3, i / conmat);
			var pos1_2 = Vector3.Lerp(pos3, pos4, i / conmat);

			var pos2_0 = Vector3.Lerp(pos1_0, pos1_1, i / conmat);
			var pos2_1 = Vector3.Lerp(pos1_1, pos1_2, i / conmat);

			Vector3 find = Vector3.Lerp(pos2_0, pos2_1, i / conmat);

			point.Add(find);            
		}

	}

	//    	void OnDrawGizmos()
	//    	{
	//    		Gizmos.color = Color.yellow;
	//    		for (int i = 0; i < point.Count-1; i++)
	//    		{
	//    			Gizmos.DrawLine(point[i], point[i + 1]);
	//    			
	//    		}
	//    	}

	enum eTweenPosState
	{
		None,
		Played,
		Finish,
	}

	private eTweenPosState mState = eTweenPosState.None;


	void OnDisable()
	{
		if (mState != eTweenPosState.Played)
		{
			isPlay = false;
			onFinish.Invoke();
			mState = eTweenPosState.Played;
		}
	}

	void Update()
	{
		Init();
		if (goPosition == null)
			return;


		Timer += Time.deltaTime;
		if (Timer > TimeSoeed && isPlay)
		{
			Timer = 0;
			goldPosition.transform.position = Vector3.Lerp(point[i - 1], point[i], 1f);
			i++;
			if (i >= point.Count) i = 1;
			if(Vector3.Distance(goldPosition.transform.position, gamePoint5) < 0.01f)
			{
				isRemove = false;
			}
		} 
		if(!isRemove)
		{
			delayTime -= Time.deltaTime;
			if(delayTime < 0)
			{
				isPlay = false;
				mState = eTweenPosState.Played;
				onFinish.Invoke();
			}
		}

	}

}