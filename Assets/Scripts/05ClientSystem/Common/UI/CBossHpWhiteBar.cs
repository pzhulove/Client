using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[ExecuteInEditMode]
public class CBossHpWhiteBar : MonoBehaviour 
{
    public RectTransform mLeft;
    public RectTransform mRight;
    public CanvasGroup   mGroup;

    public AnimationCurve fadeOutCurve;

    private float mMinValue = 0.0f;
    private float mMaxValue = 0.0f;

    public void SetValue(float minValue, float maxValue)
    {
        mMaxValue = maxValue = Mathf.Clamp01(maxValue);
        mMinValue = minValue = Mathf.Clamp01(minValue);

        if (null != mLeft)
        {
            mLeft.offsetMin = Vector2.zero;
            mLeft.offsetMax = Vector2.zero;
        }
        
        if (null != mRight)
        {
            mRight.offsetMin = Vector2.zero;
            mRight.offsetMax = Vector2.zero;
        }

        if (minValue > maxValue)
        {
            if (null != mLeft)
            {
                mLeft.anchorMin = new Vector2(0,        0.0f);
                mLeft.anchorMax = new Vector2(maxValue, 1.0f);
            }

            if (null != mRight)
            {
                mRight.anchorMin = new Vector2(minValue, 0.0f); 
                mRight.anchorMax = new Vector2(1.0f,     1.0f); 
            }
        }
        else 
        {
            if (null != mLeft)
            {
                mLeft.anchorMin = new Vector2(minValue, 0.0f);
                mLeft.anchorMax = new Vector2(maxValue, 1.0f);
            }

            if (null != mRight)
            {
                mRight.anchorMin = new Vector2(0.0f,    0.0f); 
                mRight.anchorMax = new Vector2(0.0f,    1.0f);
            }
        }
    }

    public void SetMinValue(float minValue)
    {
        if (mMinValue < mMaxValue && mMinValue > minValue)
        {
            mMinValue = minValue;
            
            if (null != mLeft)
            {
                mLeft.anchorMin = new Vector2(minValue, 0.0f);
            }
        }
    }


    private float mTimeOut = 0.0f;
    private float mTimeCount = 0.0f;
    private UnityAction mTimeOutCallback = null;

    private enum eState
    {
        None,
        Start,
        End,
    }

    private eState mState = eState.None;

    public void StartFadeOut(float timeout, UnityAction fn)
    {
        mState = eState.Start;
        mTimeOut = timeout;
        mTimeCount = timeout;
        mTimeOutCallback = fn;

        if (null != mGroup)
        {
            mGroup.alpha = 1.0f;
        }

        //var dt = DOTween.To(() => 1.0f, x =>
        //        { 
        //            if (null != mGroup)
        //            {
        //                mGroup.alpha = x; 
        //            }
        //        }, 0.0f, timeout); 
        //dt.SetEase(Ease.OutQuad);
        //dt.OnComplete( ()=>{
        //    if (null != fn)
        //    {
        //        fn.Invoke();
        //    }
        //} );
    }

    public void Update()
    {
        if (eState.Start == mState)
        {
            if (mTimeCount > 0.0f)
            {
                mTimeCount -= Time.deltaTime;

                float originRate = mTimeCount / mTimeOut;

                float rate = fadeOutCurve.Evaluate(originRate);

                if (null != mGroup)
                {
                    mGroup.alpha = rate * 1.0f;
                }
            }
            else 
            {
                if (null != mGroup)
                {
                    mGroup.alpha = 0.0f;
                }

                if (null != mTimeOutCallback)
                {
                    mTimeOutCallback.Invoke();
                    mTimeOutCallback = null;
                }

                mState = eState.End;
            }
        }
    }
}
