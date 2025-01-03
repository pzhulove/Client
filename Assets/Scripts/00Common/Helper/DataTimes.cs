using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace GameClient
{
    public enum ShowtimeType
    {
         Normal = 0,
         Auction,
         OnlineGift,
         NewYearRedPack,
    }

    /// <summary>
    /// 时间相关函数
    /// </summary>
    public static class Function
    {
        /// <summary>
        /// 将Unix时间戳转换为[20:08]hh:mm
        /// </summary>
        public static readonly DateTime sStartTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        public static string GetShortTimeString(double d)
        {
            DateTime dateTime = ConvertIntDateTime(d);
            return dateTime.ToString("T", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="d">double 型数字</param>
        /// <returns>DateTime</returns>
        public static System.DateTime ConvertIntDateTime(double d)
        {
            var time = sStartTime.AddSeconds(d);
            return time;
        }
        
        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static double ConvertDateTimeInt(System.DateTime time)
        {
            double intResult = 0;
            intResult = (time - sStartTime).TotalSeconds;
            return intResult;
        }

        public static float CalLeftTime(double fBeginTime, double TotalLastsTime)
        {
            DateTime NowTime = DateTime.Now;
            double fNowTime = ConvertDateTimeInt(NowTime);

            double AlreadyPassedTime = fNowTime - fBeginTime;

            float LeftTime = (float)TotalLastsTime - (float)AlreadyPassedTime;

            return LeftTime;
        }

        public static float CalLeftTime(float fEndTime)
        {
            DateTime NowTime = DateTime.Now;
            double fNowTime = ConvertDateTimeInt(NowTime);

            float LeftTime = fEndTime - (float)fNowTime;

            return LeftTime;
        }

        public static string GetBeginTimeStr(double fBeginTime, ShowtimeType eShowType = ShowtimeType.Normal)
        {
            DateTime beginTime = ConvertIntDateTime(fBeginTime);

            string sBeginTime = "";

            if (eShowType == ShowtimeType.Auction)
            {

            }
            else
            {
                sBeginTime += beginTime.Year.ToString() + ".";
                sBeginTime += beginTime.Month.ToString() + ".";
                sBeginTime += beginTime.Day.ToString();
            }

            return sBeginTime;
        }

        public static string GetLeftTimeStr(float fEndTime, ShowtimeType eShowType = ShowtimeType.Normal)
        {
            string sLeftTime = "";

            DateTime NowTime = DateTime.Now;
            double fNowTime = ConvertDateTimeInt(NowTime);

            double LeftTime = fEndTime - fNowTime;

            int Day = (int)LeftTime / (24 * 60 * 60);
            LeftTime -= Day * 24 * 60 * 60;

            int Hour = (int)LeftTime / (60 * 60);
            LeftTime -= Hour * 60 * 60;

            int Minute = (int)LeftTime / 60;

            if (eShowType == ShowtimeType.Auction)
            {
                if (Day > 0)
                {
                    sLeftTime = string.Format("{0}小时", Day * 24 + Hour);
                }
                else if (Hour > 0)
                {
                    sLeftTime = string.Format("{0}小时", Hour);
                }
                else if (Minute > 0)
                {
                    sLeftTime = string.Format("{0}分钟", Minute);
                }
                else
                {
                    sLeftTime = "不足1分钟";
                }
            }
            else if(eShowType == ShowtimeType.NewYearRedPack)
            {
                if (Day > 0)
                {
                    sLeftTime = string.Format("{0}天{1}小时{2}分", Day, Hour, Minute);
                }
                else if (Hour > 0)
                {
                    sLeftTime = string.Format("{0}小时{1}分", Hour, Minute);
                }
                else if (Minute > 0)
                {
                    sLeftTime = string.Format("{0}分钟", Minute);
                }
                else
                {
                    sLeftTime = "不足1分钟";
                }
            }
            else
            {
                if (Day < 0)
                {
                    sLeftTime = "<color=#fb0e0e>";
                }

                sLeftTime += "有效期:";

                if (Day > 0)
                {
                    sLeftTime += string.Format("{0}天", Day);
                }
                else if (Hour > 0)
                {
                    sLeftTime += string.Format("{0}小时", Hour);
                }
                else if (Minute > 0)
                {
                    sLeftTime += string.Format("{0}分钟", Minute);
                }
                else
                {
                    sLeftTime += "不足1分钟";
                }

                if (Day < 0)
                {
                    sLeftTime += "</color>";
                }
            }

            return sLeftTime;
        }

        public static string GetLeftTimeStr(double fBeginTime, double TotalLastsTime, ShowtimeType eShowType = ShowtimeType.Normal)
        {
            string sLeftTime = "";

            DateTime NowTime = DateTime.Now;
            double fNowTime = ConvertDateTimeInt(NowTime);

            double AlreadyPassedTime = fNowTime - fBeginTime;

            double LeftTime = TotalLastsTime - AlreadyPassedTime;

            int Day = (int)LeftTime / (24 * 60 * 60);
            LeftTime -= Day * 24 * 60 * 60;

            int Hour = (int)LeftTime / (60 * 60);
            LeftTime -= Hour * 60 * 60;

            int Minute = (int)LeftTime / 60;

            if(eShowType == ShowtimeType.Auction)
            {
                if (Day > 0)
                {
                    sLeftTime = string.Format("{0}小时", Day * 24 + Hour);
                }
                else if (Hour > 0)
                {
                    sLeftTime = string.Format("{0}小时", Hour);
                }
                else if (Minute > 0)
                {
                    sLeftTime = string.Format("{0}分钟", Minute);
                }
                else
                {
                    sLeftTime = "不足1分钟";
                }
            }
            else
            {
                if(Day < 0)
                {
                    sLeftTime = "<color=#fb0e0e>";
                }

                sLeftTime += "有效期:";

                if (Day > 0)
                {
                    sLeftTime += string.Format("{0}天", Day);
                }
                else if (Hour > 0)
                {
                    sLeftTime += string.Format("{0}小时", Hour);
                }
                else if (Minute > 0)
                {
                    sLeftTime += string.Format("{0}分钟", Minute);
                }
                else
                {
                    sLeftTime += "不足1分钟";
                }

                if (Day < 0)
                {
                    sLeftTime += "</color>";
                }
            }

            return sLeftTime;
        }
        /// <summary>
        /// 返回一个4长度链表，分别是天，时，分，秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static List<int> GetTimeForNum(int time)
        {
            List<int> numTime = new List<int>();
            numTime.Clear();
            int SumTime = (int)TimeManager.GetInstance().GetServerTime() - time;
            if (SumTime < 0)
            {
                SumTime = 0 ;
            }
            int TimeDay = SumTime / 24 / 60 / 60;
            if (TimeDay > 0)
            {
                SumTime = SumTime - TimeDay * 24 * 60 * 60;
            }
            int TimeHour = SumTime / 60 / 60;
            int TimeMin = SumTime / 60 % 60;
            int TimeSecond = SumTime % 60;
            numTime.Add(TimeDay);
            numTime.Add(TimeHour);
            numTime.Add(TimeMin);
            numTime.Add(TimeSecond);
            return numTime;
        }

        public static string SetShowLeftTime(int NextStartTime)
        {
            int SumTime = NextStartTime - (int)TimeManager.GetInstance().GetServerTime();
            if (SumTime <= 0)
            {
                return "00:00:00";
            }

            int TimeHour = SumTime / 60 / 60;
            String TempHour = TimeHour.ToString().PadLeft(2, '0');
            int TimeMin = SumTime / 60 % 60;
            String TempMin = TimeMin.ToString().PadLeft(2, '0');
            int TimeSecond = SumTime % 60;
            String TempSec = TimeSecond.ToString().PadLeft(2, '0');
            string Hour = "";
            string Min = "";
            string Second = "";
            Hour = string.Format("{0}", TempHour);
            Min = string.Format("{0}", TempMin);
            Second = string.Format("{0}", TempSec);

            return string.Format("{0}:{1}:{2}", Hour, Min, Second);
        }

        public static string SetShowTime(int NextStartTime)
        {
            string TempDay = "";
            int SumTime = NextStartTime - (int)TimeManager.GetInstance().GetServerTime();
            if(SumTime <= 0)
            {
                return "0秒";
            }
            int TimeDay = SumTime / 24 / 60 / 60;
            if(TimeDay>0)
            {
                TempDay = TimeDay.ToString() + "天";
                SumTime = SumTime - TimeDay * 24 * 60 * 60;
            }
            int TimeHour = SumTime / 60 / 60;
            int TimeMin = SumTime / 60 % 60;
            String TempMin = TimeMin.ToString().PadLeft(2, '0');
            int TimeSecond = SumTime % 60;
            String TempSec = TimeSecond.ToString().PadLeft(2, '0');
            string Hour = "";
            string Min = "";
            string Second = "";
            if (TimeHour != 0)
            {
                Hour = string.Format("{0}小时", TimeHour);
            }
            if (TimeMin != 0 || TimeHour != 0)
            {
                Min = string.Format("{0}分钟", TempMin);
            }
            if (TimeSecond != 0 || TimeMin != 0 || TimeHour != 0)
            {
                Second = string.Format("{0}秒", TempSec);
            }
            return string.Format("{0}{1}{2}{3}", TempDay,Hour, Min, Second);
        }
        public static string SetShowTimeMin(int NextStartTime)
        {
            string TempDay = "";
            int SumTime = NextStartTime - (int)TimeManager.GetInstance().GetServerTime();
            if (SumTime <= 0)
            {
                SumTime = 0;
            }
            int TimeDay = SumTime / 24 / 60 / 60;
            if (TimeDay > 0)
            {
                TempDay = TimeDay.ToString() + "天";
                SumTime = SumTime - TimeDay * 24 * 60 * 60;
            }
            int TimeHour = SumTime / 60 / 60;
            int TimeMin = SumTime / 60 % 60;
            String TempMin = TimeMin.ToString().PadLeft(2, '0');
            int TimeSecond = SumTime % 60;
            String TempSec = TimeSecond.ToString().PadLeft(2, '0');
            string Hour = "";
            string Min = "";
            string Second = "";
            if (TimeHour != 0)
            {
                Hour = string.Format("{0}小时", TimeHour);
            }
            if (TimeMin != 0 || TimeHour != 0)
            {
                Min = string.Format("{0}分钟", TempMin);
            }
            //if (TimeSecond != 0 || TimeMin != 0 || TimeHour != 0)
            //{
            //    Second = string.Format("{0}秒", TempSec);
            //}
            return string.Format("{0}{1}{2}", TempDay, Hour, Min);
        }
        public static string SetShowTimeHour(int NextStartTime)
        {
            string TempDay = "";
            int SumTime = NextStartTime - (int)TimeManager.GetInstance().GetServerTime();
            if (SumTime < 0)
            {
                return "0秒";
            }
            int TimeDay = SumTime / 24 / 60 / 60;
            if (TimeDay > 0)
            {
                TempDay = TimeDay.ToString() + "天";
                SumTime = SumTime - TimeDay * 24 * 60 * 60;
            }
            int TimeHour = SumTime / 60 / 60;

            string Hour = "";

            if (TimeHour != 0)
            {
                Hour = string.Format("{0}小时", TimeHour);
            }

            return string.Format("{0}{1}", TempDay, Hour);
        }

        /// <summary>
        /// 头像框使用期限调用
        /// </summary>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public static string SetShowTimeDay(int EndTime)
        {
            string tempDay = "";
            int sumTime = EndTime - (int)TimeManager.GetInstance().GetServerTime();
            if (sumTime < 0)
            {
                return "0秒";
            }

            int timeDay = sumTime / 24 / 60 / 60;
            if (timeDay > 0)
            {
                tempDay = string.Format("{0}天", timeDay);
            }

            if (timeDay < 1)
            {
                tempDay = "1天";
            }

            return tempDay;
        }

        public static string GetLastsTimeStr(double fCurTime, bool mustRetainHour = false)
        {
            string sLeftTime = "";

            int iIntCurTime = (int)(fCurTime);

	        if (iIntCurTime < 0)
		        iIntCurTime = 0;

			int Hour = iIntCurTime / (60 * 60);
            iIntCurTime -= Hour * 60 * 60;

            int Minute = iIntCurTime / 60;
            iIntCurTime -= Minute * 60;

            if (Hour > 0 || mustRetainHour)
            {
                sLeftTime = string.Format("{0:00}:{1:00}:{2:00}", Hour, Minute, iIntCurTime);
            }
            else if (Minute > 0)
            {
                sLeftTime = string.Format("{0:00}:{1:00}", Minute, iIntCurTime);
            }
            else
            {
                sLeftTime = string.Format("00:{0:00}", iIntCurTime);
            }

            return sLeftTime;
        }

        public static string GetLeftTime(int fEndTime, int fNowTime, ShowtimeType eShowType = ShowtimeType.Normal)
        {
            string sLeftTime = "";

//             DateTime NowTime = DateTime.Now;
//             double fNowTime = ConvertDateTimeInt(NowTime);

            int LeftTime = fEndTime - fNowTime;

	        if (LeftTime < 0)
		        LeftTime = 0;

            int Day = LeftTime / (24 * 60 * 60);
            LeftTime -= Day * 24 * 60 * 60;

            int Hour = LeftTime / (60 * 60);
            LeftTime -= Hour * 60 * 60;

            int Minute = LeftTime / 60;
            LeftTime -= Minute * 60;

            if(eShowType == ShowtimeType.OnlineGift)
            {
                if (Minute >= 0 && Minute <= 9)
                {
                    if(LeftTime >= 0 && LeftTime <= 9)
                    {
                        sLeftTime = string.Format("0{0}:0{1}", Minute, LeftTime);
                    }
                    else
                    {
                        sLeftTime = string.Format("0{0}:{1}", Minute, LeftTime);
                    }
                }
                else
                {
                    if(LeftTime >= 0 && LeftTime <= 9)
                    {
                        sLeftTime = string.Format("{0}:0{1}", Minute, LeftTime);
                    }
                    else
                    {
                        sLeftTime = string.Format("{0}:{1}", Minute, LeftTime);
                    }    
                }
            }
            else
            {
                if (Day > 0)
                {
                    sLeftTime = string.Format("{0}小时", Day * 24 + Hour);
                }
                else if (Hour > 0)
                {
                    sLeftTime = string.Format("{0}小时", Hour);
                }
                else if (Minute > 0)
                {
                    sLeftTime = string.Format("{0}分{1}秒", Minute, LeftTime);
                }
                else
                {
                    sLeftTime = string.Format("{0}分{1}秒", Minute, LeftTime);
                }
            }

            return sLeftTime;
        }

		public static string GetLeftMinutes(int fEndTime, int fNowTime)
		{
			string sLeftTime = "";

			//             DateTime NowTime = DateTime.Now;
			//             double fNowTime = ConvertDateTimeInt(NowTime);

			int LeftTime = fEndTime - fNowTime;

			int Minute = LeftTime / 60;
			LeftTime -= Minute * 60;

			sLeftTime = string.Format("{0}分{1}秒", Minute, LeftTime);


			return sLeftTime;
		}
        //判断这个时间戳是不是当天的    当天指的是只用于6点到第二天6点
        public static bool IsTodayTimeBySpan(long num)
        {
            if (num == 0)
                return false;
            DateTime nowTime = sStartTime.AddSeconds(TimeManager.GetInstance().GetServerTime() - 6 * 60 * 60); 
            DateTime dt = sStartTime.AddSeconds(num - 6 * 60 * 60);
            if (dt.Date == nowTime.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		public static string GetDate(int startTime, int endTime)
        {
            // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return startdt.ToString("yyyy年MM月dd日") + "到" + enddt.ToString("yyyy年MM月dd日");
        }

        public static string GetMonthDate(int startTime, int endTime)
        {
         // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return startdt.ToString("MM月dd日") + "到" + enddt.ToString("MM月dd日");
        }

        public static string GetOneData(int time)
        {
            DateTime thisTime = sStartTime.AddSeconds(time);
            return thisTime.ToString("yyyy年MM月dd日");
        }
        public static string GetDateTime(int startTime, int endTime)
        {
             // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return startdt.ToString("yyyy年MM月dd日HH:mm") + "到" + enddt.ToString("yyyy年MM月dd日HH:mm");
        }

        /// <summary>
        /// 格式：年月日时分秒至年月日时分秒
        /// </summary>
        /// <returns></returns>
        public static string GetDateTimeHMS(int startTime, int endTime)
        {
            // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return startdt.ToString("yyyy年MM月dd日HH:mm:ss") + "至" + enddt.ToString("yyyy年MM月dd日HH:mm:ss");
        }

	    public static string GetTime(int time)
	    {
		   // 当地时区
		    DateTime date = sStartTime.AddSeconds(time);
		    return date.ToString("HH:mm");
	    }

		public static string GetDateTime(int time,bool needYear = true)
        {
             // 当地时区
            DateTime startdt = sStartTime.AddSeconds(time);
            if(needYear)
            {
                return startdt.ToString("yyyy年MM月dd日 HH:mm");
            }
            else
            {
                return startdt.ToString("MM月dd日");
            }
        }
        public static string GetDateTimeHaveMonthToSecond(int time)
        {
            // 当地时区
            DateTime startdt = sStartTime.AddSeconds(time);
            return startdt.ToString("MM月dd日 HH:mm");
        }

        public static string GetTime(int startTime, int endTime)
        {
             // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return startdt.ToString("HH:mm") + "-" + enddt.ToString("HH:mm");
        }

        public static string GetTimeChinese(int startTime, int endTime)
        {
            // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return startdt.ToString("HH时:mm分") + "-" + enddt.ToString("HH时:mm分");
        }

        public static string GetTimeWithoutYear(int startTime, int endTime)
        {
            // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return startdt.ToString("MM月dd日HH:mm") + "~" + enddt.ToString("MM月dd日HH:mm");
        }

        public static string GetTimeWithMonthDayHour(int startTime, int endTime)
        {
            // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return startdt.ToString("M/d H:mm") + "-" + enddt.ToString("M/d H:mm");
        }

        public static string GetTimeWithoutYearNoZero(int startTime, int endTime)
        {
            // 当地时区
            DateTime startdt = sStartTime.AddSeconds(startTime);
            DateTime enddt = sStartTime.AddSeconds(endTime);
            return string.Format("{0}月{1}日{2:HH:mm}~{3}月{4}日{5:HH:mm}", startdt.Month, startdt.Day, startdt, enddt.Month, enddt.Day, enddt);
        }

        public static int GetLeftDay(int nowTime, int endTime)
        {
            int LeftTime = endTime - nowTime;

            int day = LeftTime / (24 * 60 * 60);
            return day > 0 ? day : 0;
        }

        /**
         * 获取零点时间 和 指定时间
         * **/

        public static DateTime GetTodayZeroTime()
        {
            double currTimeStamp = TimeManager.GetInstance().GetServerDoubleTime();
            System.DateTime currDateTime = Function.ConvertIntDateTime(currTimeStamp);
            return new DateTime(currDateTime.Year, currDateTime.Month, currDateTime.Day);
        }

        public static DateTime GetYestodayZeroTime()
        {
            double currTimeStamp = TimeManager.GetInstance().GetServerDoubleTime();
            System.DateTime currDateTime = Function.ConvertIntDateTime(currTimeStamp);
            TimeSpan timespan = new TimeSpan(1, 0, 0, 0);
            DateTime yesdt = currDateTime.Subtract(timespan);
            return new DateTime(yesdt.Year, yesdt.Month, yesdt.Day);
        }

        public static DateTime GetDayBeforYestodayZeroTime()
        {
            double currTimeStamp = TimeManager.GetInstance().GetServerDoubleTime();
            System.DateTime currDateTime = Function.ConvertIntDateTime(currTimeStamp);
            TimeSpan timespan = new TimeSpan(2, 0, 0, 0);
            DateTime yesdt = currDateTime.Subtract(timespan);
            return new DateTime(yesdt.Year, yesdt.Month, yesdt.Day);
        }

        public static DateTime GetTomorrowZeroTime()
        {
            double currTimeStamp = TimeManager.GetInstance().GetServerDoubleTime();
            System.DateTime currDateTime = Function.ConvertIntDateTime(currTimeStamp);
            TimeSpan timespan = new TimeSpan(1, 0, 0, 0);
            DateTime yesdt = currDateTime.Add(timespan);
            return new DateTime(yesdt.Year, yesdt.Month, yesdt.Day);
        }

        public static DateTime GetTodayGivenHourTime(int hour)
        {
            System.DateTime zeroTime = GetTodayZeroTime();
            TimeSpan timespan = new TimeSpan(0, hour, 0, 0);
            DateTime todaySixDt = zeroTime.Add(timespan);
            return todaySixDt;
        }

        public static DateTime GetTomorrowGivenHourTime(int hour)
        {
            System.DateTime zeroTime = GetTomorrowZeroTime();
            TimeSpan timespan = new TimeSpan(0, hour, 0, 0);
            DateTime torrSixDt = zeroTime.Add(timespan);
            return torrSixDt;
        }

        public static DateTime GetYesterdayGivenHourTime(int hour)
        {
            System.DateTime zeroTime = GetYestodayZeroTime();
            TimeSpan timespan = new TimeSpan(0, hour, 0, 0);
            DateTime torrSixDt = zeroTime.Add(timespan);
            return torrSixDt;
        }

        public static DateTime GetTodayGivenHourAndMinuteTime(int hour, int minute)
        {
            System.DateTime zeroTime = GetTodayZeroTime();
            TimeSpan timespan = new TimeSpan(0, hour, minute, 0);
            DateTime todaySixDt = zeroTime.Add(timespan);
            return todaySixDt;
        }

        public static double GetTodayGivenHourAndMinuteTimestamp(int hour, int minute)
        {
            DateTime todayGetTime = GetTodayGivenHourAndMinuteTime(hour, minute);            
            double timestamp = ConvertDateTimeInt(todayGetTime);
            return timestamp;
        }

        /// <summary>
        /// 获取服务器时间，今天的周天
        /// 
        /// 返回 0 表示周日
        /// 1 表示 周一 
        /// 依次。。。
        /// </summary>
        /// <returns></returns>
        public static int GetTodayWeek()
        {
            System.DateTime zeroTime = GetTodayZeroTime();
            return (int)zeroTime.DayOfWeek;
        }


        /// <summary>
        /// 获取指定时间点的前几天的时间点
        /// </summary>
        /// <param name="dayNum"></param>
        /// <returns></returns>
        public static DateTime GetBeforeDaysDateTime(int dayNum, DateTime start)
        {
            start.AddDays(-dayNum);
            return start;
        }

        /// <summary>
        /// 获取指定时间，下一周的指定周天的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNextWeekdayDateTime(DateTime start, DayOfWeek week)
        {
            DateTime temp = new DateTime(start.Year, start.Month, start.Day);
            int startDayWeek = (int)start.DayOfWeek;
            if (startDayWeek == 0)
            {
                startDayWeek = 7;
            }
            int weekDay = (int)week;
            if (weekDay == 0)
            {
                weekDay = 7;
            }
            int daysToAdd = weekDay - startDayWeek + 7;
            return temp.AddDays(daysToAdd);
        }

        /// <summary>
        /// 获取服务器时间，下一周的指定周天的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNextWeekdayDateTime(DayOfWeek week)
        {
            DateTime now = GetCurrDateTime();
            return GetNextWeekdayDateTime(now, week);
        }

        public static DateTime GetThisWeekdayDateTime(DateTime start, DayOfWeek week)
        {
            DateTime temp = new DateTime(start.Year, start.Month, start.Day);
            int startDayWeek = (int)start.DayOfWeek;
            if (startDayWeek == 0)
            {
                startDayWeek = 7;
            }
            int weekDay = (int)week;
            if (weekDay == 0)
            {
                weekDay = 7;
            }
            int daysToAdd = weekDay - startDayWeek;
            return temp.AddDays(daysToAdd);
        }

        public static DateTime GetThisWeekdayDateTime(DayOfWeek week)
        {
            DateTime now = GetCurrDateTime();
            return GetThisWeekdayDateTime(now, week);
        }

        public static DateTime GetNextMonthdayDateTime(DateTime start, int monthday)
        {
            DateTime next = new DateTime(start.Year, start.Month, 1);
            next = next.AddMonths(1);
            int nextMonthdays = DateTime.DaysInMonth(next.Year, next.Month);
            if (monthday > nextMonthdays)
            {
                monthday = nextMonthdays;
            }
            if (monthday <= 0)
            {
                monthday = 1;
            }
            return next.AddDays(monthday - 1);
        }

        public static DateTime GetNextMonthdayDateTime(int monthday)
        {
            DateTime now = GetCurrDateTime();
            return GetNextMonthdayDateTime(now, monthday);
        }

        public static DateTime GetThisMonthdayDateTime(DateTime start, int monthday)
        {
            DateTime temp = new DateTime(start.Year, start.Month, 1);
            int tempMonthdays = DateTime.DaysInMonth(temp.Year, temp.Month);
            if (monthday > tempMonthdays)
            {
                monthday = tempMonthdays;
            }
            if (monthday <= 0)
            {
                monthday = 1;
            }
            return temp.AddDays(monthday - 1);
        }

        public static DateTime GetThisMonthdayDateTime(int monthday)
        {
            DateTime now = GetCurrDateTime();
            return GetThisMonthdayDateTime(now, monthday);
        }

        /// <summary>
        /// 获取用冒号分割的时间表示中的时，分，秒的整型数组
        /// HH:mm:ss
        /// </summary>
        /// <param name="timeWithColon">英语冒号 （:）</param>
        /// <returns></returns>
        public static int[] TransferTimeSplitByColon(string timeWithColon)
        {
            int[] times = new int[3];
            if (!string.IsNullOrEmpty(timeWithColon))
            {
                string[] timeArr = timeWithColon.Split(':');
                int timeHour = 0;
                int timeMinute = 0;
                int timeSecond = 0;
                if (null != timeArr && timeArr.Length >= 2)
                {
                    if (int.TryParse(timeArr[0], out timeHour))
                    {
                        times[0] = timeHour;
                    }
                    if (int.TryParse(timeArr[1], out timeMinute))
                    {
                        times[1] = timeMinute;
                    }
                    if (timeArr.Length >= 3)
                    {
                        if (int.TryParse(timeArr[2], out timeSecond))
                        {
                            times[2] = timeSecond;
                        }
                    }
                }
            }

            return times;
        }
        #region Current Time Utility
        public static int GetCurrTimeStamp()
        {
            //获取当前时间点
            int currTimeStamp = (int)TimeManager.GetInstance().GetServerTime();
            return currTimeStamp;
        }

        public static int GetCurrTimeHour()
        {
            double currTimeStamp = TimeManager.GetInstance().GetServerDoubleTime();
            System.DateTime currDateTime = ConvertIntDateTime(currTimeStamp);
            int currHour = currDateTime.Hour;
            return currHour;
        }

        public static System.DateTime GetCurrDateTime()
        {
            double currTimeStamp = TimeManager.GetInstance().GetServerDoubleTime();
            System.DateTime currDateTime = ConvertIntDateTime(currTimeStamp);
            return currDateTime;
        }

        /// <summary>
        /// 获取今天  指定小时的 刷新时间戳
        ///    >= 返回值 即表示 在今天
        /// </summary>
        /// <param name="refreshHour"></param>
        /// <returns></returns>
        public static int GetRefreshHourTimeStamp(int refreshHour)
        {
            int currHour = GetCurrTimeHour();
            System.DateTime currDateTime = GetCurrDateTime();
            System.DateTime refreshDateTime;
            int refreshDateTimeStamp = 0;
            // 还没到今天的刷新时间 用昨天的刷新时间判断
            if (refreshHour > currHour)
            {
                refreshDateTime = GetYesterdayGivenHourTime(refreshHour);
            }
            else //到了今天刷新时间点 用今天的时间  （刷新时刻相同时 也认为是已经到了今天 用今天的刷新时间）
            {
                refreshDateTime = GetTodayGivenHourTime(refreshHour);
            }
            refreshDateTimeStamp = System.Convert.ToInt32(ConvertDateTimeInt(refreshDateTime));
            return refreshDateTimeStamp;
        }
        #endregion

        public static string _TransTimeStampToStr(UInt32 timeStamp)
        {
            DateTime dt = sStartTime.AddSeconds(timeStamp);// unix 总秒数

            return string.Format("{0}年{1}月{2}日{3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
        }
    }
}