using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DG.Tweening;

public class ControlButtonAnimationInside : MonoBehaviour {

	// Use this for initialization
	private ETCButton UIOpress;
	private RectTransform UItrans;
	public Vector3 PressScale = new Vector3(0.8f,0.8f,0.8f);
	public Vector3 ReleaseScale = new Vector3(1.0f,1.0f,1.0f);
	public float PressTime = 0.15f;
	public float ReleaseTime = 0.1f;
	void Start () {

		/*
		UIOpress = this.gameObject.GetComponent<ETCButton>();
		UItrans = this.gameObject.GetComponent<RectTransform> ();

		PressScale 	 = new Vector3(0.85f,0.85f,0.85f);
		ReleaseScale = new Vector3(1.0f,1.0f,1.0f);

		UIOpress.onDown.AddListener (skillpress);
		UIOpress.onUp.AddListener (skillup);
		*/
	}
	public void skillpress()
	{
		//DOTween.To(() => UItrans.localScale, x => UItrans.localScale = x, PressScale,PressTime);
	}
	public void skillup()
	{
		//Tweener doTweener = DOTween.To(() => UItrans.localScale, x => UItrans.localScale = x, ReleaseScale,ReleaseTime);
		//doTweener.SetEase(Ease.OutBack);
	}
    
    public void OnDestroy()
    {
        if(null != UIOpress)
        {
            UIOpress.onDown.RemoveListener(skillpress);
            UIOpress.onUp.RemoveListener(skillup);

            UIOpress = null;
            UItrans = null;
        }
    }
}
