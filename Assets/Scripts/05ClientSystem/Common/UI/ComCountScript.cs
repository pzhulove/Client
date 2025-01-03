using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class ComCountScript : MonoBehaviour {

    public UnityEvent mCallback = new UnityEvent();

    //public Text mText;

    public int mLeftTime;

    public Image mTimeImage;

    public Sprite[] mNumberSprite;
    public Material mNumberMaterial;

    [SerializeField] private bool mIsUseImage = true;
    [SerializeField] private TextEx mTextCountDown;

    public enum CountState
    {
        None,
        Pause,
        Count
    }
    
	public void StartCount(UnityAction callback, int leftTime=0)
    {
        mTime = 0.0f;
        mState = CountState.Count;
        //mText.enabled = true;
        //mText.text = mLeftTime.ToString();
		if (leftTime > 0)
			mLeftTime = leftTime;

        if (mTimeImage != null)
        {
            mTimeImage.enabled = mIsUseImage;
        }
        mTextCountDown.CustomActive(!mIsUseImage);
        _updateLeftTime();

        if (null != callback)
        {
            mCallback.AddListener(callback);
        }
    }

    private void _updateLeftTime()
    {
        if (mLeftTime >= 0)
        {
            if (mIsUseImage)
            {
                if (mTimeImage != null)
                {
                    mTimeImage.sprite = mNumberSprite[mLeftTime % 10];
                    mTimeImage.material = mNumberMaterial;
                }
            }
            else
            {
                mTextCountDown.SafeSetText((mLeftTime % 10).ToString());
            }
        }
    }

    private const float kTimeStep = 1.0f;
    private float mTime = 0.0f;

    private CountState mState;
    public CountState State { get { return mState; } }

	public void Show(bool flag)
	{
        if(mTimeImage != null)
        mTimeImage.enabled = flag;
	}
	public void SetMTimeImage(int leftSecond)
	{
        if (mTimeImage != null && mNumberSprite != null && leftSecond  >= 0 && leftSecond <  mNumberSprite.Length)
        {
            mTimeImage.sprite = mNumberSprite[leftSecond];
            mTimeImage.material = mNumberMaterial;
        }
	}

    public void Decrease()
    {
        mLeftTime--;
        //mText.text = mLeftTime.ToString();
        _updateLeftTime();

        if (mTime > 0)
        {
            mTime = 0;
        }

        if (mLeftTime <= 0)
        {
            if (mCallback != null)
            {
                try
                {
                    mCallback.Invoke();
                }
                catch (System.Exception e)
                {
                }
            }

            if (null != mTimeImage)
            {
                mTimeImage.enabled = false;
            }

            mTextCountDown.CustomActive(false);

            mState = CountState.None;
        }
    }

    public void StopCount()
    {
        mState = CountState.None;
        if (mTimeImage != null)
        {
            mTimeImage.enabled = false;
        }
        mTextCountDown.CustomActive(false);
        //mText.enabled = false;
    }

    public void PauseCount()
    {
        mState = CountState.Pause;
        if (mTimeImage != null)
        {
            mTimeImage.enabled = false;
        }
        mTextCountDown.CustomActive(false);
        //mText.enabled = false;
    }

    public void ResumeCount()
    {
        mState = CountState.Count;
        if (mTimeImage != null)
        {
            mTimeImage.enabled = mIsUseImage;
        }
        mTextCountDown.CustomActive(!mIsUseImage);
        //mText.enabled = true;
    }

    // Update is called once per frame
    void Update ()
    {
        if (mState != CountState.Count)
        {
            return;
        }

        mTime += Time.deltaTime;
        if (mTime > kTimeStep)
        {
            mTime -= kTimeStep;

            Decrease();
        }
	}

    public void RebornFail()
    {
        if (mCallback == null)
            return;
        mCallback.Invoke();
    }
}
