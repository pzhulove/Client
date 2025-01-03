using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ActiveUpdate : MonoBehaviour
{
    public int iTemplateID;
    public string key;
    public float fTickInterval;
    public string funcname;
    public Text text;
    public string formatTimeString = "{0:D2}天{1:D2}时{2:D2}分{3:D2}秒";

    private int mTotalNum;
    uint time1;
    uint time2;
    uint time3;
    double recvTime;
    void OnSevenDayTimeChanged(uint time1, uint time2,uint time3, double recvTime)
    {
        this.time1 = time1;
        this.time2 = time2;
        this.time3 = time3;
        this.recvTime = recvTime;
    }

    // Use this for initialization
    void Start ()
    {
        GameClient.ActiveManager.GetInstance().onSevenDayTimeChanged += OnSevenDayTimeChanged;
        GameClient.ActiveManager.GetInstance().SendSevenDayTimeReq();
        
        if (text != null && GameClient.ActiveManager.GetInstance().ActiveDictionary.ContainsKey(iTemplateID))
        {
            var activeData = GameClient.ActiveManager.GetInstance().ActiveDictionary[iTemplateID];
            if(activeData != null && funcname != null && fTickInterval > 0)
            {
                InvokeRepeating(funcname, 0, fTickInterval);
            }
        }
        else if(text != null && funcname != null && fTickInterval > 0)
        {
            InvokeRepeating(funcname, 0, fTickInterval);
        }
    }

    void OnUpdateCloseTime()
    {
        double iOrgTime = 10;
        if (!double.TryParse(TR.Value("seven_day_last_time"),out iOrgTime))
        {
            CancelInvoke("OnUpdateCloseTime");
            return;
        }
        var roleInfo = ClientApplication.playerinfo.roleinfo;
        if(roleInfo != null)
        {
            for(int i = 0;i  < roleInfo.Length; ++i)
            {
                if(roleInfo[i].roleId == GameClient.PlayerBaseData.GetInstance().RoleID)
                {
                    uint serverTime = GameClient.TimeManager.GetInstance().GetServerTime();
                    uint time = time1 + (uint)recvTime > serverTime ? time1 + (uint)recvTime - serverTime : 0;
                    uint iDays = time / 86400;
                    uint iHours = (time / 3600) % 24;
                    uint iMinutes = (time / 60) % 60;
                    uint iSeconds = time % 60;
                    text.text = string.Format(formatTimeString, iDays, iHours, iMinutes, iSeconds);
                    break;
                }
            }
        }
    }
    void OnUpdateAwardCloseTime()
    {
        double iOrgTime = 10;
        if (!double.TryParse(TR.Value("seven_day_award_last_time"), out iOrgTime))
        {
            CancelInvoke("OnUpdateAwardCloseTime");
            return;
        }

        var roleInfo = ClientApplication.playerinfo.roleinfo;
        if (roleInfo != null)
        {
            for (int i = 0; i < roleInfo.Length; ++i)
            {
                if (roleInfo[i].roleId == GameClient.PlayerBaseData.GetInstance().RoleID)
                {
                    uint serverTime = GameClient.TimeManager.GetInstance().GetServerTime();
                    uint time = time2 + (uint)recvTime > serverTime ? time2 + (uint)recvTime - serverTime : 0;
                    uint iDays = time / 86400;
                    uint iHours = (time / 3600) % 24;
                    uint iMinutes = (time / 60) % 60;
                    uint iSeconds = time % 60;
                    text.text = string.Format(formatTimeString, iDays, iHours, iMinutes, iSeconds);
                    break;
                }
            }
        }
    }

    void OnUpdateDailyChargeReset()
    {
        uint serverTime = GameClient.TimeManager.GetInstance().GetServerTime();
        uint time = time3 + (uint)recvTime > serverTime ? time3 + (uint)recvTime - serverTime : 0;
        uint iDays = time / 86400;
        uint iHours = (time / 3600) % 24;
        uint iMinutes = (time / 60) % 60;
        uint iSeconds = time % 60;
        text.text = string.Format(formatTimeString, iDays, iHours, iMinutes, iSeconds);
    }

    void OnLineTimeAccumulate()
    {
        if (GameClient.ActiveManager.GetInstance().ActiveDictionary.ContainsKey(iTemplateID))
        {
            var activeData = GameClient.ActiveManager.GetInstance().ActiveDictionary[iTemplateID];
            if (activeData != null)
            {
                GameClient.ActiveManager.ActiveMainUpdateKey find = null;
                for(int i = 0; i < activeData.updateMainKeys.Count; ++i)
                {
                    if(activeData.updateMainKeys[i].key == key)
                    {
                        find = activeData.updateMainKeys[i];
                        break;
                    }
                }
                if(find == null)
                {
                    return;
                } 

                int iAccumulatedTime = GameClient.ActiveManager.GetInstance().GetTemplateUpdateValue(iTemplateID, find.key);
                double iPassedTime = GameClient.TimeManager.GetInstance().GetServerTime() - find.fRecievedTime;
               
                DateTime dateTime = GameClient.Function.ConvertIntDateTime(iPassedTime + iAccumulatedTime - 8*3600);
                text.text = string.Format(find.content,dateTime.ToString("HH:mm:ss"));
            }
        }
    }
    void OnLineTimeAccumulateShowMinutes()
    {
        if (GameClient.ActiveManager.GetInstance().ActiveDictionary.ContainsKey(iTemplateID))
        {
            var activeData = GameClient.ActiveManager.GetInstance().ActiveDictionary[iTemplateID];
            if (activeData != null)
            {
                GameClient.ActiveManager.ActiveMainUpdateKey find = null;
                for (int i = 0; i < activeData.updateMainKeys.Count; ++i)
                {
                    if (activeData.updateMainKeys[i].key == key)
                    {
                        find = activeData.updateMainKeys[i];
                        break;
                    }
                }
                if (find == null)
                {
                    return;
                }

                int iAccumulatedTime = GameClient.ActiveManager.GetInstance().GetTemplateUpdateValue(iTemplateID, find.key);
                double iPassedTime = GameClient.TimeManager.GetInstance().GetServerTime() - find.fRecievedTime;
                DateTime dateTime = GameClient.Function.ConvertIntDateTime(iPassedTime + iAccumulatedTime - 8 * 3600);
                DateTime dt0 = new DateTime(1970, 1, 1);
                TimeSpan ts = new TimeSpan(dateTime.Ticks - dt0.Ticks);
                int curNum = (int)Math.Floor(ts.TotalMinutes);
                if(curNum>=mTotalNum)
                {
                    curNum = mTotalNum;
                }
                text.text = string.Format(formatTimeString, curNum, mTotalNum);
            }
        }
    }

    public void SetTotlaNum(int totalNum)
    {
        mTotalNum = totalNum;
    }

    void OnDestroy()
    {
        GameClient.ActiveManager.GetInstance().onSevenDayTimeChanged -= OnSevenDayTimeChanged;
    }
}