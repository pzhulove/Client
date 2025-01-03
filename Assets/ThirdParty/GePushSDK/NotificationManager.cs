using System;
using UnityEngine;


namespace Assets.SimpleAndroidNotifications
{
    public static class NotificationManager
    {
        private static string MainActivityClassName = "com.example.administrator.myapplication.BaseActivity";
        private const string ChannelName = "aldzn";
        private const string ChannelID = "3378";
        private const string GroupName = "";
        private const string GroupSummary = "";

        private const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";
        //MainActivityClassName = "com.example.administrator.myapplication.BaseActivity";  //"com.unity3d.player.UnityPlayerActivity";


        /// <summary>
        /// Schedule simple notification without app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public static int Send(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
        {
            return SendCustom2(new NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = delay,
                Title = title,
                Message = message,
                Ticker = message,
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = smallIconColor,
                LargeIcon = ""
            });
        }

        /// <summary>
        /// Schedule notification with app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public static int SendWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
        {
            return SendCustom(new NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = delay,
                Title = title,
                Message = message,
                Ticker = message,
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = smallIconColor,
                LargeIcon = "app_icon"
            });
        }

        public static int SendCustom2(NotificationParams notificationParams)
        {

            var p = notificationParams;
            var delay = (long)p.Delay.TotalMilliseconds;


            AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
            if (targetClass != null)
            {
                int soundFlag = 0;
                int vibrateFlag = 0;
                int lightFlag = 0;
                if (notificationParams.Sound)
                    soundFlag = 1;
                if (notificationParams.Vibrate)
                    vibrateFlag = 1;
                if (notificationParams.Light)
                    lightFlag = 1;



                targetClass.CallStatic("SetNotification", p.Id, GroupName, GroupSummary, ChannelID, ChannelName, delay, 0, 0L, p.Title, p.Message, p.Ticker,
                0, soundFlag, null, vibrateFlag, "1000,1000", lightFlag, 3000, 3000, -16711936, p.LargeIcon, GetSmallIconName(p.SmallIcon), ColotToInt(p.SmallIconColor), 1, 4, "callback", MainActivityClassName);

            }


            return notificationParams.Id;

        }


        /// <summary>
        /// Schedule customizable notification.
        /// </summary>
        public static int SendCustom(NotificationParams notificationParams)
        {


            var p = notificationParams;
            var delay = (long)p.Delay.TotalMilliseconds;

            long[] vibration = new long[] { 1000L, 1000L };

            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (targetClass != null)
                {
                    int soundFlag = 0;
                    int vibrateFlag = 0;
                    int lightFlag = 0;
                    if (notificationParams.Sound)
                        soundFlag = 1;
                    if (notificationParams.Vibrate)
                        vibrateFlag = 1;
                    if (notificationParams.Light)
                        lightFlag = 1;

                    targetClass.CallStatic("SetNotification", p.Id, GroupName, GroupSummary, ChannelID, ChannelName, delay, 1, 60 * 60 * 24 * 1000L, p.Title, p.Message, p.Ticker,
        0, soundFlag, null, vibrateFlag, "1000,1000", lightFlag, 3000, 3000, -16711936, p.LargeIcon, GetSmallIconName(p.SmallIcon), ColotToInt(p.SmallIconColor), 2, 4, "callback", MainActivityClassName);

                }
            }
            catch (Exception e)
            {
                throw e;
            }





            return notificationParams.Id;
        }


        public static int SendCustom3(NotificationParams notificationParams)
        {
            var p = notificationParams;
            long[] vibration = new long[] { 1000L, 1000L };

            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (targetClass != null)
                {
                    int soundFlag = 0;
                    int vibrateFlag = 0;
                    int lightFlag = 0;
                    if (notificationParams.Sound)
                        soundFlag = 1;
                    if (notificationParams.Vibrate)
                        vibrateFlag = 1;
                    if (notificationParams.Light)
                        lightFlag = 1;


                    
                    targetClass.CallStatic("SetNotification", p.Id, GroupName, GroupSummary, ChannelID, ChannelName, GetDelayDurTime(), 1, 24 * 60 * 24 * 1000L, p.Title, p.Message, p.Ticker,
0, soundFlag, null, vibrateFlag, "1000,1000", lightFlag, 3000, 3000, -16711936, p.LargeIcon, GetSmallIconName(p.SmallIcon), ColotToInt(p.SmallIconColor), 2, 4, "callback", MainActivityClassName);
                    

                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return notificationParams.Id;
        }

        public static int SendCustomWeekly(NotificationParams notificationParams)
        {

            Logger.LogErrorFormat("[SetCustomWeekly] - notification param large icon : {0}", notificationParams.LargeIcon);

            var p = notificationParams;
            //var delay = (long) p.Delay.TotalMilliseconds;

            long[] vibration = new long[] { 1000L, 1000L };

            AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
            if (targetClass != null)
            {
                int soundFlag = 0;
                int vibrateFlag = 0;
                int lightFlag = 0;
                if (notificationParams.Sound)
                    soundFlag = 1;
                if (notificationParams.Vibrate)
                    vibrateFlag = 1;
                if (notificationParams.Light)
                    lightFlag = 1;

                targetClass.CallStatic("SetNotification", p.Id, GroupName, GroupSummary, ChannelID, ChannelName, GetNearestWeekdayTime(weekDay, weekHour, weekMinute), 0, 7 * 60 * 24 * 1000L, p.Title, p.Message, p.Ticker,
0, soundFlag, null, vibrateFlag, "1000,1000", lightFlag, 3000, 3000, -16711936, p.LargeIcon, GetSmallIconName(p.SmallIcon), ColotToInt(p.SmallIconColor), 2, 4, "callback", MainActivityClassName);

            }


            return notificationParams.Id;
        }

        /// <summary>
        /// Cancel notification by id.
        /// </summary>
        public static void Cancel(int id)
        {
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (targetClass != null)
                {
                    targetClass.CallStatic("CancelNotification", id);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Cancel all notifications.
        /// </summary>
        public static void CancelAll()
        {
            try
            {
                AndroidJavaClass targetClass = new AndroidJavaClass(FullClassName);
                if (targetClass != null)
                {

                    targetClass.CallStatic("CancelAllNotifications");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private static int ColotToInt(Color color)
        {
            var smallIconColor = (Color32)color;

            return smallIconColor.r * 65536 + smallIconColor.g * 256 + smallIconColor.b;
        }

        private static string GetSmallIconName(NotificationIcon icon)
        {
            return "anp_" + icon.ToString().ToLower();
        }

        private static DateTime GetCurrentDateTime()
        {
#if !TEST_PACKAGE
            return DateTime.Now;
#else
            return GameClient.Function.ConvertIntDateTime(0);
#endif
        }


        private static long GetDelayDurTime()
        {
            int delayDurTime = 0;
            int currDateSecond = GetCurrentDateTime().Hour * 60 * 60 + GetCurrentDateTime().Minute * 60 + GetCurrentDateTime().Second;
            int customTimeSecond = hour * 60 * 60;
            int del = customTimeSecond - currDateSecond;
            Logger.LogErrorFormat("[GetDelayDurTime] - del time second is {0}", del);
            if (del <= 0)
            {
                delayDurTime = 24 * 60 * 60 + del;
            }
            else
            {
                delayDurTime = del;
            }

            Logger.LogErrorFormat("[GetDelayDurTime] - final delayDurTime second is {0}", delayDurTime * 1000L);

            return delayDurTime * 1000L;
        }

        private static long GetNearestWeekdayTime(int weekday, int hour, int minute)
        {
            //Logger.LogErrorFormat("[GetNearestWeekdayTime] - input weekday : {0}, hour : {1}, minute : {2}", weekday, hour, minute);

            long delayDurTime = 0;
            int weekdayNow = (int)GetCurrentDateTime().DayOfWeek + 1;

            //Logger.LogErrorFormat("[GetNearestWeekdayTime] - weekday now : {0}", weekdayNow);

            int addDay = (7 - weekdayNow + weekday) % 7;

            //Logger.LogErrorFormat("[GetNearestWeekdayTime] - addDay now : {0}", addDay);

            DateTime tempTime = GetCurrentDateTime().AddDays(addDay);

            //Logger.LogErrorFormat("[GetNearestWeekdayTime] - tempTime now : {0}", tempTime.ToString());

            DateTime goalTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day, hour, minute, 0);

            //Logger.LogErrorFormat("[GetNearestWeekdayTime] - goalTime now : {0}", goalTime.ToString());

            delayDurTime = (long)goalTime.Subtract(GetCurrentDateTime()).TotalMilliseconds;

            //Logger.LogErrorFormat("[GetNearestWeekdayTime] - delayDurTime second now : {0}", delayDurTime.ToString());

            if (delayDurTime < 0) delayDurTime += 7 * 24 * 60 * 60 * 1000L;

            Logger.LogErrorFormat("[GetNearestWeekdayTime] - final delayDurTime : {0}", delayDurTime);

            return delayDurTime;
        }

        public static long GetNearestYearDayTime(int year, int yearMonth, int yearDay, int yearHour, int yearMinute)
        {
            long delayDurtime = 0;

            int nowYear = (int)GetCurrentDateTime().Year;
            int nowMonth = (int)GetCurrentDateTime().Month;
            int nowYearDay = (int)GetCurrentDateTime().DayOfYear;


            return delayDurtime;
        }


        private static int hour;
        public static void SetCustomHour(int outHour)
        {
            hour = outHour;
        }

        private static int weekDay;
        private static int weekHour;
        private static int weekMinute;
        public static void SetCustomWeekly(int outWeekday, int outHour, int outMinute)
        {
            weekDay = outWeekday;
            weekHour = outHour;
            weekMinute = outMinute;

            Logger.LogErrorFormat("[SetCustomWeekly] - weekDay : {0}, weekHour : {1}, weekMinute : {2}", weekDay, weekHour, weekMinute);
        }


        private static int year;
        private static int yearMonth;
        private static int yearDay;
        private static int yearDayHour;
        private static int yearDayMinute;
        public static void SetCustomYearly(int outYear, int outYearMonth, int outYearDay, int outYearDayHour, int outYearDayMinute)
        {
            year = outYear;
            yearMonth = outYearMonth;
            yearDay = outYearDay;
            yearDayHour = outYearDayHour;
            yearDayMinute = outYearDayMinute;

            Logger.LogErrorFormat("[SetCustomYearly] - year : {0}, year month : {1}, year day : {2}, year day hour : {3}, year day minute : {4}", year, yearMonth, yearDay, yearDayHour, yearDayMinute);
        }


        public static void SetIntentActivityForSDK(string mainActivityClass)
        {
            MainActivityClassName = mainActivityClass;
            Logger.LogErrorFormat("[SetIntentActivityForSDK] - mainActivityClass is {0}", mainActivityClass);
        }
    }
    public class PartyDayModel
    {
        private int[] year;
        private int[] month;
        private int[] day;
        private int minter;
        private int hour;

        private bool isEveryYear = false;

        public PartyDayModel(int minter, int hour, int[] day, int[] month, int[] year = null)
        {
            if (day == null || month == null)
            {
                return;
            }
            this.year = year;
            this.hour = hour;
            this.minter = minter;
            this.day = day;
            this.month = month;

            if (year == null)
            {
                isEveryYear = true;
            }



        }
        public DateTime GetNextPartyDay(DateTime now)
        {
            //Console.WriteLine("start party day");
            if (day == null || month == null)
            {
                return default(DateTime);
            }

            bool isnextday = false;
            if (hour < now.Hour || (hour == now.Hour && minter < now.Minute))
            {
                isnextday = true;
            }
            bool isnextmonth = false;
            if (day[day.Length - 1] < now.Day ||
               (day[day.Length - 1] == now.Day && isnextday))
            {
                isnextmonth = true;
            }
            bool isnextyear = false;

            if (month[month.Length - 1] < now.Month ||
               (month[month.Length - 1] == now.Month && isnextmonth))
            {
                isnextyear = true;
            }


            var daycls = FindVaildvalue(day, now.Day, isnextday);
            if (!daycls.HasValue())
            {
                daycls.Index = 0;
            }

            var monthcls = FindVaildvalue(month, now.Month, isnextmonth);
            if (!monthcls.HasValue())
            {
                daycls.Index = 0;
                monthcls.Index = 0;
            }
            ValueWithIndex<int> yearcls;
            if (isEveryYear)
            {
                year = new int[] { now.Year - 1, now.Year, now.Year + 1 };
                //daycls.Index = 0;
                //monthcls.Index = 0;
                //yearcls.Index = 0;
            }

            yearcls = FindVaildvalue(year, now.Year, isnextyear);
            if (!yearcls.HasValue())
            {
                //Console.WriteLine("没有活动了");
                return default(DateTime);
            }

            if (yearcls.Value == 0 || monthcls.Value == 0 || daycls.Value == 0)
            {
                //Console.WriteLine("ERROR");
                //throw new Exception("EEEEE");
                return default(DateTime);
            }

            //校验日期是否合法
            var vailddays = DateTime.DaysInMonth(yearcls.Value, monthcls.Value);

            if (vailddays < daycls.Value)
            {
                //Console.WriteLine("invaild");
                daycls.Index = 0;

                if (monthcls.IsEndIndex())
                {
                    monthcls.Index = 0;
                    if (isEveryYear)
                    {
                        yearcls = new ValueWithIndex<int>(new int[] { now.Year + 1 });
                        yearcls.Index = 0;
                    }
                    else
                    {
                        if (yearcls.IsEndIndex())
                        {
                            //Console.WriteLine("没有活动了");
                            return default(DateTime);
                        }
                        else
                        {
                            yearcls.MoveNext();
                        }
                    }
                }
                else
                {
                    monthcls.MoveNext();
                }
            }

            return new DateTime(yearcls.Value, monthcls.Value, daycls.Value, hour, minter, 0);



        }
        public class ValueWithIndex<T>
        {
            public T Value
            {
                get
                {
                    return Arr[Index];
                }
                set
                {
                    Arr[Index] = value;
                }
            }
            private int _index;
            public int Index
            {
                get
                {
                    return _index;
                }
                set
                {
                    _index = value;
                }
            }
            public T[] Arr { get; private set; }
            public ValueWithIndex(T[] arr)
            {
                Index = -1;
                Arr = arr;
            }
            public bool IsEndIndex()
            {
                if (Index == Arr.Length - 1)
                {
                    return true;
                }
                return false;
            }
            public bool MoveNext()
            {
                if (IsEndIndex())
                {
                    return true;
                }
                Index += 1;
                return false;
            }
            public bool HasValue()
            {
                if (Arr == null || Index == -1)
                {
                    return false;
                }
                return true;
            }
        }
        private ValueWithIndex<int> FindVaildvalue(int[] array, int curvalue, bool isnext)
        {
            var ret = new ValueWithIndex<int>(array);


            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == curvalue)
                {
                    if (isnext)
                    {
                        if (i != array.Length - 1)
                        {
                            ret.Index = i + 1;
                            break;
                        }
                    }
                    else
                    {
                        ret.Index = i;
                        break;
                    }
                }
                else if (array[i] > curvalue)
                {
                    ret.Index = i;
                    break;
                }
            }
            return ret;

        }

    }


}