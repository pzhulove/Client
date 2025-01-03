using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using DG.Tweening;
using GameClient;

[ExecuteAlways]
public class ComDungeonScore : ComBaseComponet 
{
    public enum eState
    {
        Close,
        Open,
    }
    
    public float mTime  = 0.5f;

    public float mDelay = 2.0f;
    public int   mCnt = 2;

    private int mCountCnt = 2;

    public ComDungeonScoreInfo[] infos;
    public Sprite[] mImages;
    public DungeonScore mRealScore = DungeonScore.C;
    public Image mScoreImage;

    public GameObject mRoot;
    public RectMask2D mRootMask;

    public eState mState = eState.Open;

    public delegate void OnFadeChanged(eState state);
    public OnFadeChanged onFadeChanged;

    public void Init()
    {
        for (int i = 0; i < infos.Length; ++i)
        {
            infos[i].Init();
        }

        _setScore(DungeonScore.SSS);

        mCountCnt = mCnt;
		mState = eState.Close;

		if (mState == eState.Close)
		{
			mRoot.transform.localPosition = new Vector3(172, 0, 0);
		}
		else 
		{
			mRoot.transform.localPosition = new Vector3(-172, 0, 0);
		}

		//UpdateRoot();
    }

    public void UpdateRoot()
    {

        if (mState == eState.Close)
        {
            mState = eState.Open;
            _updateInfos();
        }
        else
        {
            mState = eState.Close;
        }

        if (mCountCnt > 0)
        {
            GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.ClientBattleMainFadeInFadeOut, (object)(mState == eState.Open));
        }
        _setRootState(mState == eState.Open);

        if (onFadeChanged != null)
        {
            onFadeChanged(mState);
        }
    }

    private void _setRootState(bool isOpen)
    {
        if (isOpen)
        {
            var anim = DOTween.To(
                    () => mRoot.transform.localPosition,
                    r => mRoot.transform.localPosition = r,
                    new Vector3(-172, 0, 0),
                    mTime);
            if(anim != null)
            {
                anim.onComplete = DoRootMaskActive;
                anim.SetEase(Ease.OutQuad);
            }
        }
        else 
        {
            DoRootMaskActive();
            DOTween.To(
                    () => mRoot.transform.localPosition,
                    r => mRoot.transform.localPosition = r,
                    new Vector3(172, 0, 0),
                    mTime).SetEase(Ease.OutQuad);
        }

    }

    public void SetScore(DungeonScore score)
    {
        _setScore(score);

        if (mState == eState.Open)
        {
            _updateInfos();
        }
    }

#region ExtraUIBind
    private GameObject mScoreImageRoot0 = null;
    private GameObject mScoreImageRoot1 = null;
    private GameObject mScoreImageRoot2 = null;
    private Image mScoreImage0 = null;
    private Image mScoreImage1 = null;
    private Image mScoreImage2 = null;

    protected override void _bindExUI()
    {
        mScoreImageRoot0 = mBind.GetGameObject("scoreImageRoot0");
        mScoreImageRoot1 = mBind.GetGameObject("scoreImageRoot1");
        mScoreImageRoot2 = mBind.GetGameObject("scoreImageRoot2");
        mScoreImage0 = mBind.GetCom<Image>("scoreImage0");
        mScoreImage1 = mBind.GetCom<Image>("scoreImage1");
        mScoreImage2 = mBind.GetCom<Image>("scoreImage2");
    }

    protected override void _unbindExUI()
    {
        mScoreImageRoot0 = null;
        mScoreImageRoot1 = null;
        mScoreImageRoot2 = null;
        mScoreImage0 = null;
        mScoreImage1 = null;
        mScoreImage2 = null;
    }
#endregion   

    private void DoRootMaskActive()
    {
        if (mRootMask != null)
        {
            if (mState == eState.Open)
            {
                mRootMask.enabled = false;
            }
            else
            {
                mRootMask.enabled = true;
            }
        }
    }

    private void _setScore(DungeonScore score)
    {
        if (mRealScore != score)
        {
            mRealScore = score;

            if (!isInited)
            {
                return ;
            }

            mScoreImageRoot0.CustomActive(false);
            mScoreImageRoot1.CustomActive(false);
            mScoreImageRoot2.CustomActive(false);

            //mScoreImage0.sprite = mBind.GetSprite("s");
            //mScoreImage1.sprite = mBind.GetSprite("s");
            //mScoreImage2.sprite = mBind.GetSprite("s");
            mBind.GetSprite("s", ref mScoreImage0);
            mBind.GetSprite("s", ref mScoreImage1);
            mBind.GetSprite("s", ref mScoreImage2);

            switch (score)
            {
                case Protocol.DungeonScore.SSS:
                    mScoreImageRoot0.CustomActive(true);
                    mScoreImageRoot1.CustomActive(true);
                    mScoreImageRoot2.CustomActive(true);
                    break;
                case Protocol.DungeonScore.SS:
                    mScoreImageRoot0.CustomActive(true);
                    mScoreImageRoot1.CustomActive(true);
                    break;
                case Protocol.DungeonScore.S:
                    mScoreImageRoot2.CustomActive(true);
                    break;
                case Protocol.DungeonScore.A:
                case Protocol.DungeonScore.B:
                case Protocol.DungeonScore.C:
                    mScoreImageRoot2.CustomActive(true);
                    // mScoreImage0.sprite = mBind.GetSprite("a");
                    mBind.GetSprite("a", ref mScoreImage2);
                    break;
            }

        }
    }

    private void _updateInfos()
    {
        for (int i = 0; i < infos.Length; ++i)
        {
            infos[i].UpdateInfo();
        }
    }

    private float mTickTime = 0.0f;

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SetScore(mRealScore);
        }
#endif
        
        /*if (mTickTime > 0)
        {
            mTickTime -= Time.deltaTime;
        }
        else 
        {
            if (mCountCnt > 0)
            {
                mTickTime = mDelay;
                UpdateRoot();
                mCountCnt--;
            }
        }*/
        
    }
}
