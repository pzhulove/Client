using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ComAnimatorAutoClose : MonoBehaviour
{
    public float mDefTimeLen = 1.0f;
    public Animator mAnimator;
    public bool loadFromPool = false;

    private float mSumTime  = 0.0f;
    private float mLeftTime = 0.0f;

    [Range(0, 1)]
    public float mRate = 1.0f;
	// Use this for initialization

    public void SetRate(float rate)
    {
        mRate = Mathf.Clamp01(rate);
    }

    enum AnimaterState
    {
        None,
        Play,
        Finish,
    }

    private AnimaterState mState = AnimaterState.None;

    public UnityEvent mOnFinishCallback = new UnityEvent();

    void OnEnable()
    {
        mLeftTime = _getTime();
        mSumTime  = mLeftTime;
        mState    = AnimaterState.Play;
        mIsInvoke = false;
	}

    private float _getTime()
    {
        var t = 0.0f;
        if (null == mAnimator)
        {
            t = mDefTimeLen;
        }
        else
        {
            var infos = mAnimator.GetCurrentAnimatorClipInfo(0);
            for (int i = 0, icnt = infos.Length; i < icnt; ++i)
            {
                AnimatorClipInfo item = infos[i];
                t += item.clip.length;
            }
        }

        return t;
    }

    private bool mIsInvoke = false;
    
    private void _invoke()
    {
        if (!mIsInvoke)
        {
            mIsInvoke = true;

            mOnFinishCallback.Invoke();

            _destroyGameObject();

            mState = AnimaterState.Finish;
        }
    }

    private void _destroyGameObject()
    {
        if (loadFromPool)
        {
            CGameObjectPool.instance.RecycleGameObject(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
	
	void Update () 
    {
        if (mState == AnimaterState.Play)
        {
            mLeftTime -= Time.deltaTime;

            var rate = 1.0f - mLeftTime / mSumTime;

            if (rate >= mRate)
            {
                _invoke();
            }

            if (mLeftTime <= 0)
            {
                _invoke();
            }
        }
	}
}
