using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComTipsUnit : MonoBehaviour {

    public Image mFg;
    public Image mIcon;
    public Text mCount;

    public void SetPercent(float percent)
    {
        if (null != mFg)
        {
            mFg.fillAmount = Mathf.Clamp01(1.0f - percent);
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (null != mIcon)
        {
            mIcon.sprite = sprite;
        }
    }

    public void SetCount(int count)
    {
        if (null != mCount)
        {
            mCount.text = count.ToString();
        }
    }

    public void SetType()
    {

    }
}
