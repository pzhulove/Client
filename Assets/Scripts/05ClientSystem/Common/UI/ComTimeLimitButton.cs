using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComTimeLimitButton : MonoBehaviour {

    public int     maxCount = 1;

    public Button  mButton;
    public UIGray  mGray;
    public ComTime mLeftTime;

    public float mDelayResume = 5.0f;

    private int  mCurrentCount = 0;

    public enum eState 
    {
        onWait,
        onNormal,
    }

    private float mCalDelay = 0.0f;

    private eState mState = eState.onNormal;
    private eState state 
    {
        get {
            return mState;
        }

        set 
        {
            if (mState != value)
            {
                mState = value;
                _onStateChange(mState == eState.onNormal);
            }
        }
    }

    private void _onStateChange(bool isNormal)
    {
        if (null != mButton)
        {
            mButton.interactable = isNormal;
        }

        if (null != mGray)
        {
            mGray.enabled = !isNormal;
        }

        if (null != mLeftTime)
        {
            mLeftTime.gameObject.SetActive(!isNormal);
        }
    }
    
    void Start()
    {
        state = eState.onNormal;
    }

    void Awake()
    {
        if (null != mButton)
        {
            mButton.onClick.AddListener(_onButtonClick);
        }
        ResetCount();
    }

    void OnDestroy()
    {
        if (null != mButton)
        {
            mButton.onClick.RemoveListener(_onButtonClick);
        }
    }

	// Update is called once per frame
	void Update () 
    {
        if (state == eState.onWait)
        {
            mCalDelay -= Time.deltaTime;

            if (mCalDelay < 0)
            {
                state = eState.onNormal;
            }
            else
            {
                if (null != mLeftTime)
                {
                    mLeftTime.SetTime((int)(mCalDelay * 1000));
                }
            }
        }
	}

    public void ResetCount()
    {
        mCurrentCount = 0;
    }

    private void _onButtonClick()
    {
        if (state == eState.onNormal)
        {
            mCurrentCount++;

            if (mCurrentCount >= maxCount)
            {
                ResetCount();

                state = eState.onWait;
                mCalDelay = mDelayResume;
            }
        }
    }
}
