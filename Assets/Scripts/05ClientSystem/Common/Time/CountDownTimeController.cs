using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace GameClient
{

    public delegate void OnCountDownTimeCallback();

    public class CountDownTimeController : MonoBehaviour
    {
        public enum CountDownTimeType
        {
            InValid = -1,
            TimeHourMinute = 0,             //显示小时和分钟 00:00
            TimeSecond = 1,                 //显示s
            TimeHourMinuteSecond = 2,       //显示小时，分钟和秒 00:00:00
            TimeMinuteSecond = 3,           //显示分钟和秒 00:00
        }

        [SerializeField] private Text countDownTimeLabel;
        [SerializeField] private CountDownTimeType countDownTimeType;
        [SerializeField] private float refreshInterval = 1.0f;

        private OnCountDownTimeCallback _onCountDownTimeCallback;
        private uint _endTime;

        //倒计时开始：设置时间，进行初始化操作
        //倒计时关闭：直接重置

        //设置结束时间
        public uint EndTime
        {
            set { _endTime = value; }
        }

        //开启倒计时
        public void InitCountDownTimeController()
        {
            CancelCountDownTime();
            InvokeRepeating("OnUpdateCountDownTime", 0, refreshInterval);
        }

        public void SetCountDownTimeCallback(OnCountDownTimeCallback onCountDownTimeCallback)
        {
            _onCountDownTimeCallback = onCountDownTimeCallback;
        }

        public void ResetCountDownTimeController()
        {
            CancelCountDownTime();
        }

        private void OnUpdateCountDownTime()
        {
            if (countDownTimeType == CountDownTimeType.InValid)
            {
                CancelCountDownTime();
                return;
            }

            switch (countDownTimeType)
            {
                case CountDownTimeType.TimeHourMinute:
                    UpdateCountDownTimeByHourMinute();
                    break;
                case CountDownTimeType.TimeSecond:
                    UpdateCountDownTimeBySecond();
                    break;
                case CountDownTimeType.TimeHourMinuteSecond:
                    UpdateCountDownTimeByHourMinuteSecond();
                    break;
                case CountDownTimeType.TimeMinuteSecond:
                    UpdateCountDownTimeByMinuteSecond();
                    break;
                default:
                    break;
            }
        }

        private void UpdateCountDownTimeByMinuteSecond()
        {
            if (countDownTimeLabel != null)
            {
                countDownTimeLabel.text = CountDownTimeUtility.GetCountDownTimeByMinuteSecondFormat(
                    _endTime,
                    TimeManager.GetInstance().GetServerTime());
            }

            if (TimeManager.GetInstance().GetServerTime() >= _endTime)
            {
                CancelCountDownTime();
                if (_onCountDownTimeCallback != null)
                    _onCountDownTimeCallback();
            }

        }


        //小时，分钟
        private void UpdateCountDownTimeByHourMinute()
        {
            if (countDownTimeLabel != null)
            {
                countDownTimeLabel.text = CountDownTimeUtility.GetCountDownTimeByHourMinute(_endTime,
                    TimeManager.GetInstance().GetServerTime());
            }

            if (TimeManager.GetInstance().GetServerTime() >= _endTime)
            {
                CancelCountDownTime();
                if (_onCountDownTimeCallback != null)
                {
                    _onCountDownTimeCallback();
                }
            }
        }

        //小时分钟和秒
        private void UpdateCountDownTimeByHourMinuteSecond()
        {
            if (countDownTimeLabel != null)
            {
                countDownTimeLabel.text = CountDownTimeUtility.GetCountDownTimeByHourMinuteSecondFormat(_endTime,
                    TimeManager.GetInstance().GetServerTime());
            }

            if (TimeManager.GetInstance().GetServerTime() >= _endTime)
            {
                CancelCountDownTime();
                if (_onCountDownTimeCallback != null)
                {
                    _onCountDownTimeCallback();
                }
            }
        }

        //s 倒计时
        private void UpdateCountDownTimeBySecond()
        {

            if (countDownTimeLabel != null)
            {
                countDownTimeLabel.text = CountDownTimeUtility.GetCountDownTimeBySecondFormat(_endTime,
                    TimeManager.GetInstance().GetServerTime());
            }

            if (TimeManager.GetInstance().GetServerTime() >= _endTime)
            {
                CancelCountDownTime();
                if (_onCountDownTimeCallback != null)
                {
                    _onCountDownTimeCallback();
                }
            }
        }

        private void CancelCountDownTime()
        {
            CancelInvoke("OnUpdateCountDownTime");
        }
        
    }

}
