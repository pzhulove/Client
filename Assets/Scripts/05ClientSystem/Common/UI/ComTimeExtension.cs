using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComTimeExtension : ComTime
{
    public bool mIsHideHour = false;
    public override void SetTime(int time)
    {
        mTimeInMS = time;
        _UpdateText();
    }

    private void _UpdateText()
    {
        bool isOverLimit = mTimeInLimit < mTimeInMS;

        if (mIsTimeLimit && isOverLimit)
        {
            mTimeInMS = mTimeInLimit;
        }

        int ms = mTimeInMS % 1000;
        int mTimeInS = mTimeInMS / 1000;
        int s = mTimeInS % 60;
        int m = mTimeInS / 60;
        int h = 0;
        //int m = mTimeInS / 60 % 60;
        //int h = mTimeInS / 60 / 60;

        if (mIsHiddenZero)
        {
            if (mIsHideHour)
            {
                mCurrentText.text = string.Format("{0,2}{1}{2,2}{3}{4,2}",
                   (h == 0 && m == 0) ? "" : m.ToString(mToStringFormat), (h == 0 && m == 0) ? "" : mTimeMiniteUnit.ToString(),
                   ((h != 0 || m != 0) && s == 0) ? "" : s.ToString(mToStringFormat), ((h != 0 || m != 0) && s == 0) ? "" : mTimeSecondUnit,
                   mIsHiddenMs ? "" : (ms / 10).ToString());
            }
            else
            {
                mCurrentText.text = string.Format("{0,2}{1}{2,2}{3}{4,2}{5}{6,2}",
                    h == 0 ? "" : h.ToString(mToStringFormat), h == 0 ? "" : mTimeHoursUnit.ToString(),
                    (h == 0 && m == 0) ? "" : m.ToString(mToStringFormat), (h == 0 && m == 0) ? "" : mTimeMiniteUnit.ToString(),
                    ((h != 0 || m != 0) && s == 0) ? "" : s.ToString(mToStringFormat), ((h != 0 || m != 0) && s == 0) ? "" : mTimeSecondUnit,
                    mIsHiddenMs ? "" : (ms / 10).ToString());
            }
        }
        else
        {
            if (mIsHideHour)
            {
                mCurrentText.text = string.Format("{0,2}{1}{2,2}{3}{4,2}",
                    m.ToString(mToStringFormat), mTimeMiniteUnit,
                    s.ToString(mToStringFormat), mTimeSecondUnit,
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
}
