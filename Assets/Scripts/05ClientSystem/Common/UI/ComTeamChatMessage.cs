using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ComTeamChatMessage : MonoBehaviour
{
    public float mDelay = 5.0f;

    public GameObject mRoot;
    public Text mContent;
    public GameObject mLinkRoot;

    private GameClient.LinkParse mLink;

    private enum eState 
    {
        onShow,
        onHidden,
    }

    private eState mState = eState.onHidden;

	// Use this for initialization
	void Start () 
    {
        _hiddenRoot(false);
        
        if (null != mLinkRoot)
        {
            mLink = mLinkRoot.GetComponent<GameClient.LinkParse>();
        }
	}

    private void _hiddenRoot(bool isShow)
    {
        mState = isShow ? eState.onShow : eState.onHidden;

        if (null != mRoot)
        {
            mRoot.SetActive(isShow);
        }
    }

    private void _setMsg(string msg)
    {
        if (null != mContent)
        {
            mContent.text = msg;
        }

        if (null != mLink)
        {
            mLink.SetText(msg);
        }
    }

    private float mCal = 0.0f;

    public void SetMessage(string msg)
    {
        _hiddenRoot(true);
        _setMsg(msg);
        mCal = mDelay;
    }

    void Update()
    {
        if (mState == eState.onShow)
        {
            if (mCal > 0)
            {
                mCal -= Time.deltaTime;
            }
            else
            {
                _hiddenRoot(false);
            }
        }
    }
}
