using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;
using GameClient;

//[ExecuteInEditMode]
public class ComExpBar : MonoBehaviour
{
    private int mMaxBarCount = 10;

    public float mDelayTime = 0.0f;
    public float mTweenTime = 0.5f;

    public Color mImageColor = new Color(1f, 71f / 255f, 0f);

    public delegate KeyValuePair<UInt64, UInt64> ExpBarCallback(UInt64 exp);
    private ExpBarCallback mExpBarCallback;

    public delegate string TextFormatter(ulong a_uExp);
    public TextFormatter TextFormat { get; set; }

    [System.Serializable]
    public class ExpBarRate : UnityEvent<float> { }
    [SerializeField]
    public ExpBarRate[] mExpBarRateCallbacks = new ExpBarRate[0];

    public Image[] mImageBar = new Image[0];

    public Text mExpText;

    private Tweener mTweener;

    [System.NonSerialized]
    [Range(0, 1)]
    /// <summary>
    /// the test variable in editor
    /// </summary>
    public float mRate;

    private UInt64 mTotalExp;

    public void Awake()
    {
        mTotalExp = 0;
        mMaxBarCount = Mathf.Max(mImageBar.Length, mExpBarRateCallbacks.Length);
    }

    public void OnDestroy()
    {
        mExpBarCallback = null;
        this.mTweener = null;
    }

    private void _updateText(UInt64 curExp, UInt64 curSum, ulong a_uTotalExp)
    {
        if (null != mExpText)
        {
            if (TextFormat != null)
            {
                mExpText.text = TextFormat.Invoke(a_uTotalExp);
            }
            else
            {
                if (curSum == 0)
                {
                    mExpText.text = "满级";
                }
                else
                {
                    mExpText.text = string.Format("{0}/{1}", curExp, curSum);
                }
            }
        }
    }

    private void _callCB(int idx, float rate)
    {
        if (idx >= 0 && idx < mExpBarRateCallbacks.Length)
        {
            mExpBarRateCallbacks[idx].Invoke(rate);
        }
    }

    /// <summary>
    /// 兼容之前已经使用的预制体
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="rate"></param>
    private void _callCBImage(int idx, float rate)
    {
        if (idx >= 0 && idx < mImageBar.Length)
        {
            mImageBar[idx].fillAmount = rate;
        }
    }

    public void SetRate(float rate)
    {
        rate = Mathf.Clamp01(rate) * mMaxBarCount;

        int iPart = (int)rate;
        float fPart = rate - iPart;

        for (int i = 0; i < iPart; ++i)
        {
            _callCB(i, 1.0f);
            _callCBImage(i, 1.0f);
        }

        if (iPart != mMaxBarCount)
        {
            _callCB(iPart, fPart);
            _callCBImage(iPart, fPart);
        }
        else 
        {
            //_updateColor(mImageColor);
        }

        for (int i = iPart + 1; i < mMaxBarCount; ++i)
        {
            _callCB(i, 0.0f);
            _callCBImage(i, 0.0f);
        }
    }


    private void _updateExp(UInt64 exp)
    {
        var kv = new KeyValuePair<UInt64, UInt64>(0, 0);
        if (null != mExpBarCallback)
        {
            try
            {
                kv = mExpBarCallback(exp);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("exp bar error {0}", e.ToString());
            }
        }

        _updateText(kv.Key, kv.Value, exp);

        if (kv.Key == 0 && kv.Value == 0)
        {
            SetRate(1.0f);
        }
        else 
        {
            SetRate(1.0f * kv.Key / kv.Value);
        }

        mTotalExp = exp;
    }

    public void SetExp(UInt64 exp, bool force = true, ExpBarCallback cb = null)
    {
        mExpBarCallback = cb;

        /// FIX
        /// 反正这里有个bug
        /// 1级没有经验的时候，没有动画
        if (force || mTotalExp == 0 || mTotalExp > exp)
        {
            _updateExp(exp);
        }
        else
        {
            if (this.mTweener != null && this.mTweener.IsActive())
            {
                this.mTweener.ChangeStartValue(this.mTotalExp);
                this.mTweener.ChangeEndValue(exp);
                this.mTweener.Restart();
            }
            else
            {
                this.mTweener = DOTween.To(
                    () => this.mTotalExp,
                    _updateExp,
                    exp, this.mTweenTime);

                this.mTweener.SetDelay(this.mDelayTime);
            }
        }
    }
}
