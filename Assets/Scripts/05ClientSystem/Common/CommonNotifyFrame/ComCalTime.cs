using UnityEngine;
using System;

public class ComCalTime : MonoBehaviour
{
    bool bBeginCal = false;
    bool bAddPosy = false;

    double BeginTime = 0.0f;
    double timeInterval = 0.0f;

    public void BeginCalTime(bool bBegin)
    {
        bBeginCal = bBegin;

        if(bBeginCal)
        {
            BeginTime = ConvertDateTimeInt(DateTime.Now);
        }      
    }

    public void SetPosy(bool bSet)
    {
        bAddPosy = bSet;
    }

    public bool GetPosyIsAdded()
    {
        return bAddPosy;
    }

    public double GetPassedTime()
    {
        return timeInterval;
    }

    void Update()
    {
        if(bBeginCal)
        {
            double NowTime = ConvertDateTimeInt(DateTime.Now);

            timeInterval = NowTime - BeginTime;
        }     
    }

    double ConvertDateTimeInt(DateTime time)
    {
        double result = 0.0f;
        result = (time - GameClient.Function.sStartTime).TotalSeconds;

        return result;
    }
}