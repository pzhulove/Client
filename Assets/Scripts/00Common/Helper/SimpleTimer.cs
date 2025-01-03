using UnityEngine;
using System.Collections;
using UnityEngine.UI;

 
public class SimpleTimer : MonoBehaviour {

	public enum FormatType
	{
		PK = 0,
		MALL_ITEM,
        MALL_GIFT,
        MALL_LIMITTIME,         //限时商城
        ACTIVITY_LIMIT,         //限时活动
        MONTH_CARD_LOCKERS,     //月卡寄存箱
        STRENGTHEN_TICKET_BUFF, //强化祈福
	}

	public class Time4
	{
		public int day;
		public int hour;
		public int min;
		public int sec;
	}

	public bool useSystemUpdate = false;
    public Text componetText = null;
    int timeAcc = 0;
    bool run = false;
    int seconds = 0;
	int countdown = 0;

	public FormatType formatType = FormatType.PK;
	Time4 time4 = new Time4();


    const int INTERVAL = 1000;

    public void SetVisible(bool flag)
    {
#if !SERVER_LOGIC 

        gameObject.SetActive(flag);

 #endif

    }

    public void StartTimer()
    {
        SetVisible(true);

        seconds = 0;
        run = true;
    }

	public void SetCountdown(int seconds)
	{
		countdown = seconds;
#if !SERVER_LOGIC 
		SetText(countdown);
 #endif

	}

	public void Reset()
	{
		seconds = 0;
	}

    public void StopTimer()
    {
        run = false;
    }

    public void UpdateTimer(int delta)
    {
        if (!run)
            return;

        timeAcc += delta;
        if (timeAcc >= INTERVAL)
        {
            timeAcc -= INTERVAL;
            ShowSecond(1);
        }
    }

	void Update()
	{
		if (!useSystemUpdate)
			return;
		
		float timeElapsed = Time.deltaTime * GlobalLogic.VALUE_1000;
		UpdateTimer((int)timeElapsed);
	}

    void ShowSecond(int add)
    {
        seconds += add;

#if !SERVER_LOGIC 

        if (componetText != null)
        {
			int tmp = seconds;
			if (countdown > 0)
			{
				tmp = countdown - seconds;
			}
			else
			{
			    tmp = 0;
			}
			SetText(tmp);
        }

 #endif

    }

#if !SERVER_LOGIC 

	void SetText(int tmp)
	{

		ConverSeconds(tmp, ref time4);
		if (formatType == FormatType.PK)
		{
			if (countdown > 0 && tmp <= 0)
				componetText.text = "时间到";
			else
			{
				if (countdown > 0 && tmp <= 10)
				{
					componetText.color = Color.red;
				}

				componetText.text = string.Format("{0:00}:{1:00}", time4.min, time4.sec);
			}
		}
		else if (formatType == FormatType.MALL_ITEM)
		{
            if(time4.day > 0)
            {
                componetText.text = string.Format("剩余{0}天", time4.day);
            }
            else
            {
                componetText.text = string.Format("{0}:{1:00}:{2:00}", time4.hour, time4.min, time4.sec);
            }
        }
        else if (formatType == FormatType.MALL_GIFT)
        {
            if (time4.day > 0)
            {
                componetText.text = string.Format("剩余{0}天", time4.day);
            }
            else
            {
				if(time4.hour < 0 )
					time4.hour = 0;
				if(time4.min < 0 )
					time4.min = 0;
				if(time4.sec < 0 )
					time4.sec = 0;
                componetText.text = string.Format("剩余时间：{0:00}:{1:00}:{2:00}", time4.hour, time4.min, time4.sec);
            }
        }
        else if (formatType == FormatType.MALL_LIMITTIME)
		{
		    if (time4.day > 0)
		    {
		        componetText.text = string.Format("{0}天", time4.day);
		    }
		    else
		    {
		        if (time4.hour < 0)
		            time4.hour = 0;
		        if (time4.min < 0)
		            time4.min = 0;
		        if (time4.sec < 0)
		            time4.sec = 0;
		        componetText.text = string.Format("{0:00}:{1:00}:{2:00}", time4.hour, time4.min, time4.sec);
		    }
		}
        else if (formatType == FormatType.ACTIVITY_LIMIT)
		{
		    if (time4.day > 0)
		    {
		        componetText.text = string.Format(TR.Value("activity_limit_left_time"), time4.day);
		    }
		    else
		    {
                //小时，分钟，秒均小于0的时候，活动结束
		        if (time4.hour <= 0 && time4.min <= 0 && time4.sec <= 0)
		        {
		            componetText.text = TR.Value("activity_limit_time_over");
		        }
		        else
		        {
		            if (time4.hour < 0)
		                time4.hour = 0;
		            if (time4.min < 0)
		                time4.min = 0;
		            if (time4.sec < 0)
		                time4.sec = 0;
                    var timeStr = string.Format(TR.Value("activity_limit_left_less_one_day"), time4.hour, time4.min, time4.sec);
		            componetText.text = timeStr;
		        }
		    }
        }
        else if (formatType == FormatType.MONTH_CARD_LOCKERS)
        {
            if (time4.day > 0)
            {
                if (time4.hour < 0)
                    time4.hour = 0;
                if (time4.min < 0)
                    time4.min = 0;
                if (time4.sec < 0)
                    time4.sec = 0;
                var timeStr = string.Format(TR.Value("month_card_item_expiredtime"), time4.hour + time4.day * 24, time4.min, time4.sec);
                componetText.text = timeStr;
            }
            else
            {
                if (time4.hour <= 0 && time4.min <= 0 && time4.sec <= 0)
                {
                    componetText.text = TR.Value("month_card_item_expiredtime_over");
                }
                else
                {
                    if (time4.hour < 0)
                        time4.hour = 0;
                    if (time4.min < 0)
                        time4.min = 0;
                    if (time4.sec < 0)
                        time4.sec = 0;
                    var timeStr = string.Format(TR.Value("month_card_item_expiredtime"), time4.hour, time4.min, time4.sec);
                    componetText.text = timeStr;
                }
            }
        }
        else if (formatType == FormatType.STRENGTHEN_TICKET_BUFF)
        {
            var timeStr = string.Format(TR.Value("qianghuaquanhecheng_left_time"), time4.day, time4.hour, time4.min, time4.sec);
            componetText.text = timeStr;
        }

    }
	
	#endif

	public bool IsTimeUp()
	{
		if (countdown > 0)
			return seconds >= countdown;
		return false;
	}

	public void ConverSeconds(int seconds, ref Time4 time)
	{
		time.day = seconds / (24 * 60 * 60);

		seconds -= time.day * (24 * 60 * 60);

		time.hour = seconds / (60 * 60);

		seconds -= time.hour * (60 * 60);

		time.min = seconds / 60;
		time.sec = seconds % 60;
    }
	public int GetPassTime()
	{
		return seconds;
	}

    #region Added Method

    public Time4 GetCurrTime4()
    {
        return time4;
    }

    #endregion
}
