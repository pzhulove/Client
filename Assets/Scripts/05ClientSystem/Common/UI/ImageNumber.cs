using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class ImageNumber : MonoBehaviour {

    public enum eImageNumberState
    {
        None,
        Animating,
        End,
    }

    [System.Serializable]
    public class ImageNode
    {
        public DOTweenAnimation[] allAnimate = new DOTweenAnimation[0];
        public Image image;

        private eImageNumberState mState;

        public eImageNumberState state
        {
            get
            {
                return mState;
            }

            set
            {
                mState = value;
            }
        }

        public float animateTime
        {
            get; set;
        }

        private int mNumber = 0;
        public int number
        {
            get {
                return mNumber;
            }

            set
            {
                if (value != mNumber)
                {
                    mNumber = value;
                    isDirty = true;
                }
            }
        }

        public bool isDirty
        {
            get; private set;
        }

        public void Play()
        {
            state = eImageNumberState.Animating;

            image.transform.localScale = Vector3.one;

            for (int i = 0; i < allAnimate.Length; ++i)
            {
                if (null != allAnimate[i])
                {
                    allAnimate[i].DORestart();
                }
            }

            _resetAnimateTime();

            isDirty = false;
        }

        private void _resetAnimateTime()
        {
            float maxTime = 0.0f;
            for (int i = 0; i < allAnimate.Length; ++i)
            {
                if (null != allAnimate[i] && allAnimate[i].loops > 0)
                {
                    maxTime = Mathf.Max(maxTime, allAnimate[i].delay + allAnimate[i].duration * allAnimate[i].loops);
                }
            }

            animateTime = maxTime;
        }
    }

    public Sprite[] numberList = new Sprite[10];

    public ImageNode[] allImageNodes = new ImageNode[0];
	
    public int number;

	public float offsetX = 0.0f;
	public float offsetXStep = 0.0f;
	public float offsetY = 0.0f;
	public float offsetYStep = 0.0f;

	void OnEnable()
	{
		UpdateNumber();
	}

	void OnDisable()
	{
        for (int i = 0; i < allImageNodes.Length; ++i)
        {
            allImageNodes[i].image.enabled = false;
        }
	}

    public void SetTextNumber(int num, bool isObjEnable = false)
    {
        number = num;
        UpdateNumber(isObjEnable);
    }

    void UpdateNumber(bool isObjEnable = false)
    {
        int showNum = 0;
        int leftNum = number;

        Logger.LogProcessFormat("[数字更新] num: {0}", number);

        for (int i = 0; i < allImageNodes.Length; i++)
        {
            showNum = leftNum % 10;
            leftNum = leftNum / 10;

            if (leftNum > 0 || showNum > 0)
            {
                allImageNodes[i].image.sprite = numberList[showNum];
                allImageNodes[i].image.enabled = true;
                allImageNodes[i].number = showNum;
                if(isObjEnable)
                    allImageNodes[i].image.CustomActive(true);
            }
            else 
            {
                allImageNodes[i].image.enabled = false;
                allImageNodes[i].number = 0;
                if(isObjEnable)
                    allImageNodes[i].image.CustomActive(false);
            }
        }
    }

    public void Update()
    {
        for (int i = 0; i < allImageNodes.Length; ++i)
        {
            ImageNode curNode = allImageNodes[i];

            switch (curNode.state)
            {
                case eImageNumberState.Animating:
                {
                    if (curNode.animateTime > 0)
                    {
                        curNode.animateTime -= Time.deltaTime;
                    }
                    else
                    {
                        curNode.state = eImageNumberState.End;
                    }
                    break;
                }
                case eImageNumberState.End:
                {
                    curNode.state = eImageNumberState.None;
                    break;
                }
                case eImageNumberState.None:
                {
                    if (curNode.isDirty)
                    {
                        curNode.Play();
                    }
                    break;
                }
            }
        }
    }
}
