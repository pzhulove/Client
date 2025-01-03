using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteAlways]
public class ComTime : MonoBehaviour {

    public int mTimeInMS = (60 * 60 * 3 + 60 * 3 + 3) * 1000 + 20;

    public string mTimeHoursUnit = "H";
    public string mTimeMiniteUnit = "M";
    public string mTimeSecondUnit = "S";

    public bool mIsHiddenZero = true;
    public bool mIsHiddenMs = false;
    public bool mIsTripSpace = false;

    public int mTimeInLimit  = 1000 * 10;
    public bool mIsTimeLimit = false;
    public string mTimeLimitSign = ">";

    public string mToStringFormat = "0";


    public Text mCurrentText;

    private void _updateText()
    {
        bool isOverLimit = mTimeInLimit < mTimeInMS;

        if (mIsTimeLimit && isOverLimit)
        {
            mTimeInMS = mTimeInLimit;
        }

        int ms = mTimeInMS % 1000;
        int mTimeInS = mTimeInMS / 1000;
        int s = mTimeInS % 60;
        int m = mTimeInS / 60 % 60;
        int h = mTimeInS / 60 / 60;

        if (mIsHiddenZero)
        {
            mCurrentText.text = string.Format("{0,2}{1}{2,2}{3}{4,2}{5}{6,2}",
                h == 0 ? "" : h.ToString(mToStringFormat), h == 0 ? "" : mTimeHoursUnit.ToString(), 
                (h == 0 && m == 0) ? "" : m.ToString(mToStringFormat), (h == 0 && m == 0) ? "" : mTimeMiniteUnit.ToString(),
                ((h != 0 || m != 0) && s == 0) ?  "" : s.ToString(mToStringFormat), ((h != 0 || m != 0) && s == 0) ? "" : mTimeSecondUnit,
                mIsHiddenMs ? "" : (ms / 10).ToString());
        }
        else
        {
            mCurrentText.text = string.Format("{0,2}{1}{2,2}{3}{4,2}{5}{6,2}",
                h.ToString(mToStringFormat), mTimeHoursUnit,
                m.ToString(mToStringFormat), mTimeMiniteUnit,
                s.ToString(mToStringFormat), mTimeSecondUnit,
                mIsHiddenMs ? "" : (ms / 10).ToString());
        }


        if (mIsTripSpace)
        {
            var str = mCurrentText.text;
            mCurrentText.text = str.Trim();
        }

        if (mIsTimeLimit && isOverLimit)
        {
            var str = mCurrentText.text;
            mCurrentText.text = mTimeLimitSign + str;
        }
    }

    public virtual void SetTime(int time)
    {
        mTimeInMS = time;
        _updateText();
    }

#if UNITY_EDITOR
    public void Update()
    {
        if (!Application.isPlaying)
        {
            SetTime(mTimeInMS);
        }
    }
#endif
}
