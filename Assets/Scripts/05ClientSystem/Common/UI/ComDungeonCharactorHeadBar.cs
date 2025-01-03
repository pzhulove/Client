using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComDungeonCharactorHeadBar : ComBaseCharactorBar, IDungeonCharactorBar
{
    public Image[] mImageBar;
    public eDungeonCharactorBar mType;
    public Text buffTxt;
    public UIGray mCdGray;
    public Text mCDText;

    public void SetRate(float rate)
    {
        var maxBarCnt = mImageBar.Length;
        rate = Mathf.Clamp01(rate) * maxBarCnt;

        int iPart = (int)rate;
        float fPart = rate - iPart;

        for (int i = 0; i < iPart; ++i)
        {
            mImageBar[i].fillAmount = 1.0f;
        }

        if (iPart != maxBarCnt)
        {
            mImageBar[iPart].fillAmount = fPart;
        }

        for (int i = iPart + 1; i < maxBarCnt; ++i)
        {
            mImageBar[i].fillAmount = 0.0f;
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
