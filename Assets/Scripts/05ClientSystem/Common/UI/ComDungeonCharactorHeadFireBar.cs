using UnityEngine;
using UnityEngine.UI;
using System.Collections;

class ComDungeonCharactorHeadFireBar : ComBaseCharactorBar, IDungeonCharactorBar
{
    public Image[] mImageBarFull;
    public Image[] mImageBarEmpty;
    public eDungeonCharactorBar mType;
    public Text buffTxt;
    public UIGray mCdGray;
    public Text mCDText;
    
    public void SetRate(float rate)
    {
        rate = Mathf.Clamp01(rate);
        int n = (int)(rate / 0.2f);
        for (int i = 0; i < mImageBarFull.Length; i++)
        {
            mImageBarFull[i].color = i < n ? Color.white : Color.clear;
            //mImageBarFull[i].gameObject.CustomActive(i < n);
        }
    }

    public void SetBarType(eDungeonCharactorBar type)
    {
        mType = type;
    }

    public eDungeonCharactorBar GetBarType()
    {
        return mType;
    }

    public void Show(bool isShow)
    {
        if (mCdGray != null)
        {
            mCdGray.enabled = false;
        }

        _SetVisible(isShow);

        if (!isShow)
        {
            if (mCDText != null)
            {
                mCDText.text = "";
            }
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void SetText(string content)
    {
        if (buffTxt != null)
            buffTxt.text = content;
    }

    public void SetCdText(float cdTime)
    {
        if (mCdGray != null)
        {
            mCdGray.enabled = cdTime != 0 ? true : false;
        }

        if (mCDText != null)
        {
            mCDText.text = cdTime.ToString("#0.0");
        }
    }
}
