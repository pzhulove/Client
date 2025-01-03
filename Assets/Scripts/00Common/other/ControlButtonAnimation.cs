using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DG.Tweening;

[RequireComponent(typeof(UISelectableExtension))]
public class ControlButtonAnimation : MonoBehaviour {

	// Use this for initialization
	private UISelectableExtension UIOpress;
	public RectTransform UItrans;
	public Vector3 mPressScale = new Vector3(0.7f,0.7f,0.7f);
	public bool    bUseGlobalScale = true;
	private Vector3 mReleaseScale = new Vector3(1.0f,1.0f,1.0f);
	private AnimationCurve mEaseCurve = new AnimationCurve (new Keyframe(0,0),new Keyframe(1,1));
	private float mPressTime = 0.08f;
	private float mReleaseTime = 0.3f;
	void Start () {

		if(LeanTween.instance != null)
		{
			if(bUseGlobalScale) mPressScale = LeanTween.instance.buttonPressScale;
			mEaseCurve = LeanTween.instance.buttonTween;
		}

		UIOpress = this.gameObject.GetComponent<UISelectableExtension>();
		if(UItrans == null)
		UItrans = this.gameObject.GetComponent<RectTransform> ();
		if(UIOpress && UItrans)
		{
			UIOpress.OnButtonPress.AddListener (item =>{
				DOTween.To(() => UItrans.localScale, x => UItrans.localScale = x, mPressScale,mPressTime);
			});
			UIOpress.OnButtonRelease.AddListener (item => {
				Tweener doTweener = DOTween.To(() => UItrans.localScale, x => UItrans.localScale = x, mReleaseScale,mReleaseTime);
				doTweener.SetEase(mEaseCurve);
			});
		}
	}

    void OnDestroy()
    {
        if (null != UIOpress)
        {
            UIOpress.OnButtonPress.RemoveAllListeners();
            UIOpress.OnButtonRelease.RemoveAllListeners();
        }
    }

}
