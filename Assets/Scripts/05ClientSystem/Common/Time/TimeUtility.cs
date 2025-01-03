using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;
using static System.TimeZone;

namespace GameClient
{
    //时间助手类
    public static class TimeUtility
    {

        //判断两个时间戳是否为同一天
        public static bool IsSameDayOfTwoTime(ulong beginTime, ulong endTime)
        {
            if (endTime <= beginTime)
                return true;

            var beginDateTime = GetDateTimeByTimeStamp((int) beginTime);
            var endDateTime = GetDateTimeByTimeStamp((int) endTime);

            return beginDateTime.ToString("yyyy-MM-dd") == endDateTime.ToString("yyyy-MM-dd");
        }

        //获得两个时间戳内的周数
        public static int GetWeekNumberBetweenTime(ulong beginTime, ulong endTime)
        {
            if (endTime <= beginTime)
                return 0;

            int week = 0;

            var beginDataTime = GetDateTimeByTimeStamp((int) beginTime);
            var endDataTime = GetDateTimeByTimeStamp((int) endTime);

            while (beginDataTime < endDataTime)
            {
                if (beginDataTime.DayOfWeek == DayOfWeek.Sunday)
                    week++;

                beginDataTime = beginDataTime.AddDays(1);
            }

            if (endDataTime.DayOfWeek != DayOfWeek.Sunday)
                week++;

            return week;
        }

        //根据时间轴，得到时间
        public static DateTime GetDateTimeByTimeStamp(int time)
        {
            System.DateTime getTime = CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime thisTime = getTime.AddSeconds(time);
            return thisTime;
        }

        //得到今天是周几
        //1-7分别代表周一到周日
        public static int GetTodayTimeInWeekDay()
        {
            System.DateTime zeroTime = Function.GetTodayZeroTime();

            switch (zeroTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                case DayOfWeek.Sunday:
                    return 7;
            }
            return 7;
        }

        //得到昨天是周几；1-7：周一到周日
        public static int GetYesterdayTimeInWeekDay()
        {
            var todayTime = GetTodayTimeInWeekDay();
            //今天是周一，则昨日是周天
            if (todayTime == 1)
                return 7;

            //周二到周日，昨天以此递减
            return todayTime - 1;
        }


        public static string GetTimeFormatByYearMonthDay(UInt32 timeStamp)
        {
            System.DateTime time = CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            var result = string.Format("{0}年{1}月{2}日{3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
            return result;
        }

        public static string GetTimeFormatByYearMonthDay(UInt32 timeStamp,Func<string> foramt)
        {
            System.DateTime time = CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            var result = string.Format(foramt(), dt.Year, dt.Month, dt.Day);
            return result;
        }

        //2010-10-11
        public static string GetDateTimeByCommonType(int time)
        {
            System.DateTime getTime = CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            var dateTime = getTime.AddSeconds(time);
            return dateTime.ToString("yyyy-MM-dd");
        }

        //天数
        public static string GetDayNumberStr(int time)
        {
            var dayNumber = time / (24 * 3600);
            return dayNumber.ToString();
        }

    }
}