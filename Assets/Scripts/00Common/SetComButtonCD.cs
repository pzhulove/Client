using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetComButtonCD : MonoBehaviour {

    [SerializeField]
    private float mButtonCD = 0.5f;
    [SerializeField]
    private bool mBtIsWork = false;
    [SerializeField]
    private UIGray mUiGray = null;
    [SerializeField]
    private Button mBtn = null;
    [SerializeField]
    private Text mText = null;
    [SerializeField]
    private string mFormatStr = "s";
    [SerializeField]
    private bool needBtnInteractable = false;
    [SerializeField]
    private bool needBtnGray = false;

    private float timer = 0f;
    private string oldText = "";
    private bool bDirty = false;
	void Start ()
    {
        mBtIsWork = true;
        timer = 0f;
        bDirty = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (bDirty == false)
        {
            return;
        }
        if (mBtIsWork == true)
        {
            timer = 0f;
            if (needBtnGray && mUiGray != null)
            {
                mUiGray.enabled = false;
            }
            if (needBtnInteractable && mBtn != null)
            {
                mBtn.interactable = true;
            }
            bDirty = false;
            return;
        }
        if (timer < mButtonCD)
        {
            float mTime = mButtonCD - timer;
            int m_ITime = (int)mTime + 1;
            if(mText != null)
            {
                mText.text = oldText + m_ITime.ToString() + mFormatStr;
            }
            timer += Time.deltaTime;
        }
        else
        {
            mBtIsWork = true;
            if (mText)
            {
                mText.text = oldText;
            }
        }
    }

    public void StartBtCD()
    {
        if (needBtnGray && mUiGray != null)
        {
            mUiGray.enabled = true;
        }
        if (needBtnInteractable && mBtn != null)
        {
            mBtn.interactable = false;
        }
        if (mText)
        {
            oldText = mText.text;
        }
        mBtIsWork = false;
        bDirty = true;
    }

    public void StopBtCD()
    {
        if (needBtnGray && mUiGray != null)
        {
            mUiGray.enabled = false;
        }
        if (needBtnInteractable && mBtn != null)
        {
            mBtn.interactable = true;
        }
        if (mText)
        {
            oldText = mText.text;
        }
        mBtIsWork = true;
        bDirty = false;
    }

    public bool IsBtnWork()
    {
        return mBtIsWork;
    }
}
