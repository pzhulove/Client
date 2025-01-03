using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class TimeRefresh : MonoBehaviour {
    public Text text = null;
    public enum TimeType
    {
        TT_SHORT_TIME = 0,
        TT_SECOND,
        TT_INVALID,
    }
    public TimeType m_eTimeType = TimeType.TT_SHORT_TIME;
    public UnityEvent mAction;
    //public float fTickInterval = 1.0f;
    public string formatString = "{0}";
    public float refreshInterval = 1.0f;
    bool enable = true;
    public bool Enable
    {
        get { return enable; }
        set
        {
            enable = value;
        }
    }
    public GameClient.UINumber uiNumber;
    public void SetUINumber()
    {
        if(null != uiNumber)
        {
            uiNumber.Value = (int)time;
        }
    }

    public void SetFormatString(string value)
    {
        this.formatString = value;
    }

    // Use this for initialization
    public void Initialize ()
    {
        time = 0;
        endTime = 0;
        CancelInvoke("UpdateTime");
        InvokeRepeating("UpdateTime", 0, refreshInterval);
	}

    uint time;
    uint endTime;
    public uint Time
    {
        set
        {
            time = value;
            endTime = GameClient.TimeManager.GetInstance().GetServerTime() + value;
        }
    }

    public void UpdateTime()
    {
        var curTime = GameClient.TimeManager.GetInstance().GetServerTime();
        //if (endTime > curTime)
        {
            time = endTime > curTime ? endTime - curTime : 0;
            if (text != null)
            {
                if(m_eTimeType == TimeType.TT_SHORT_TIME)
                {
                    uint iH = time / 3600;
                    uint iM = (time - 3600 * iH) / 60;
                    uint iS = (time - 3600 * iH) % 60;
                    text.text = string.Format(formatString, string.Format("{0:D2}:{1:D2}:{2:D2}", iH, iM, iS));
                    if(null != mAction)
                    {
                        mAction.Invoke();
                    }
                }
                else if(m_eTimeType == TimeType.TT_SECOND)
                {
                    uint iS = time % 60;
                    text.text = string.Format(formatString, string.Format("{0:D2}",iS));
                    if (null != mAction)
                    {
                        mAction.Invoke();
                    }
                }
            }
        }
    }
}
