using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //倒计时
    public static class CountDownTimeUtility
    {
        //倒计时：小时分钟
        public static string GetCountDownTimeByHourMinute(uint endTime, uint curTime)
        {

            uint hour = 0;
            uint min = 0;
            if (endTime <= curTime)
                return string.Format("{0:D2}:{1:D2}", hour, min);

            var intervalTime = endTime - curTime;
            hour = intervalTime / 3600;
            min = (intervalTime - 3600 * hour) / 60;

            return string.Format("{0:D2}:{1:D2}", hour, min);
        }

        //小时：分钟：秒 00:00:00
        public static string GetCountDownTimeByHourMinuteSecondFormat(uint endTime, uint curTime)
        {
            uint hour = 0;
            uint minute = 0;
            uint second = 0;

            if (endTime <= curTime)
                return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);

            var intervalTime = endTime - curTime;
            hour = intervalTime / 3600;
            minute = (intervalTime - 3600 * hour) / 60;
            second = (intervalTime - 3600 * hour) % 60;

            var timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);

            return timeStr;
        }

        //倒计时：分钟和秒； 00:00
        public static string GetCountDownTimeByMinuteSecondFormat(uint endTime, uint curTime)
        {
            uint minute = 0;
            uint second = 0;
            if(endTime <= curTime)
                return string.Format("{0:D2}:{1:D2}", minute, second);

            var intervalTime = endTime - curTime;
            minute = intervalTime / 60;
            second = intervalTime - (minute * 60);

            var timeStr = string.Format("{0:D2}:{1:D2}", minute, second);

            return timeStr;
        }

        //倒计时：天小时分秒：1天03小时06分08秒
        public static string GetCountDownTimeByDayHourMinuteSecondFormat(uint endTime, uint curTime)
        {
            uint day = 0;
            uint hour = 0;
            uint minute = 0;
            uint second = 0;
            if (endTime <= curTime)
            {
                var zeroTimeStr = TR.Value("Count_Down_Time_Day_Hour_Minute_Second_Format",
                    day,
                    hour,
                    minute,
                    second);

                return zeroTimeStr;
            }
            
            var intervalTime = endTime - curTime;
            day = intervalTime / 86400;         //(3600 * 24)
            hour = (intervalTime - 86400 * day) / 3600;
            var leftTime = intervalTime - day * 86400 - hour * 3600;
            minute = leftTime / 60;
            second = leftTime % 60;

            var timeStr = TR.Value("Count_Down_Time_Day_Hour_Minute_Second_Format",
                day,
                hour,
                minute,
                second);
            return timeStr;
        }

        //秒 
        public static string GetCountDownTimeBySecondFormat(uint endTime, uint curTime)
        {
            if (endTime <= curTime)
                return "0";

            var intervalTime = endTime - curTime;
            return intervalTime.ToString();
        }

        public static string GetCoolDownTimeByDayHour(uint endTime, uint curTime)
        {
            uint day = 0;
            uint hour = 0;

            if (endTime <= curTime)
            {
                return string.Format(TR.Value("auction_new_item_trade_cool_down_time"), day, hour);
            }

            var intervalTime = endTime - curTime;
            day = intervalTime / (3600 * 24);
            hour = (intervalTime - 3600 * 24 * day) / 3600;
            return string.Format(TR.Value("auction_new_item_trade_cool_down_time"), day, hour);
        }

    }
}