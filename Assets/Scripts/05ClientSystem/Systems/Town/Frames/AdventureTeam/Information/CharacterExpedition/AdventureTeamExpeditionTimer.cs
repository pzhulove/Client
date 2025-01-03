using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{


    public class AdventureTeamExpeditionTimer : MonoBehaviour
    {

        public enum FormatType
        {
            ADVENTURE_TEAM_EXPEDITION, //佣兵团远征倒计时
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
        int countdown = 0;
        bool run = false;
        int seconds = 0;

        public FormatType formatType = FormatType.ADVENTURE_TEAM_EXPEDITION;
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
            if (formatType == FormatType.ADVENTURE_TEAM_EXPEDITION)
            {
                if (time4.day > 0)
                {
                    componetText.text = string.Format("剩余{0}天", time4.day);
                }
                else
                {
                    if (time4.hour <= 0 && time4.min > 0)
                    {
                        componetText.text = TR.Value("adventure_team_expedition_timer_tips") + string.Format("\n{0:00}分{1:00}秒", time4.min, time4.sec);
                    }
                    else if (time4.hour <= 0 && time4.min <= 0 && time4.sec > 0)
                    {
                        componetText.text = TR.Value("adventure_team_expedition_timer_tips") + string.Format("\n{0:00}秒", time4.sec);
                    }
                    else if (time4.hour <= 0 && time4.min <= 0 && time4.sec <= 0)
                    {
                        
                    }
                    else
                    {
                        componetText.text = TR.Value("adventure_team_expedition_timer_tips") + string.Format("\n{0:00}时{1:00}分{2:00}秒", time4.hour, time4.min, time4.sec);
                    }
                }
            }

        }

#endif
        public void ConverSeconds(int seconds, ref Time4 time)
        {
            time.day = seconds / (24 * 60 * 60);

            seconds -= time.day * (24 * 60 * 60);

            time.hour = seconds / (60 * 60);

            seconds -= time.hour * (60 * 60);

            time.min = seconds / 60;
            time.sec = seconds % 60;
        }
    }
}