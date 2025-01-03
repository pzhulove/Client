using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ControlUIAlpha : MonoBehaviour {
	
	// Use this for initialization
	private CanvasGroup cg;
	public float delayTime = 0.0f;
	public float toAlpha = 1.0f;
	public float alphaTime = 1.0f;
	void Start () {
		cg = this.GetComponent<CanvasGroup> ();
	}
	
	void TweenShow ()
	{
		DOTween.To(() => cg.alpha, x => cg.alpha = x, toAlpha, alphaTime);
		cg .DOFade( toAlpha, alphaTime);
	}
	
	// Update is called once per frame
	void Update () {
		delayTime -= Time.deltaTime;
		if(delayTime <= 0)
		{
			delayTime = 0;
			TweenShow();
		}
		
	}
}
