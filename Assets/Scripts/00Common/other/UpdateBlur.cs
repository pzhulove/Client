using UnityEngine;
using UnityEngine.UI;

public class UpdateBlur : MonoBehaviour {

	// Use this for initialization
	public float DurationTime = 0.15f;
	public float DelayTime = 0.1f;
	void Start () {
		this.GetComponent<Image> ().material.SetFloat ("_Distance",DurationTime);
	}
	
	// Update is called once per frame
	void Update () {
		DelayTime -= Time.deltaTime;
		if(DelayTime <= 0)
		{
			DurationTime -= Time.deltaTime;
			if (DurationTime <= 0) {
				DurationTime = 0;
			}
			this.GetComponent<Image> ().material.SetFloat ("_Distance",DurationTime);
		}

	}
}
