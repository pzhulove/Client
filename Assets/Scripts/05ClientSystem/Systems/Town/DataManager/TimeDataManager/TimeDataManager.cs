
using System;
using System.Collections.Generic;

namespace GameClient
{
    public interface ITimeData
    {
        long StartTime { get; }
        long EndTime { get; }
    }

    public struct TimeData : ITimeData
    {
        long _startTime;
        long _endTime;

        public TimeData(long startTime, long endTime)
        {
            _startTime = startTime;
            _endTime = endTime;
        }

        public long EndTime
        {
            get
            {
                return _endTime;
            }
        }

        public long StartTime
        {
            get
            {
                return _startTime;
            }
        }
    }

    public class TimeDataManager : DataManager<TimeDataManager>
    {
        List<TimeModel> _unixDataList = new List<TimeModel>();

        public int CalcRemainTimeByUnix(ITimeData time)
        {
            int remainTime = (int)(time.EndTime - TimeManager.GetInstance().GetServerTime());
            if (remainTime < 0)
            {
                remainTime = 0;
            }

            return remainTime;
        }

        public int CalcDuration(ITimeData time)
        {
            var ticks = (DateTime.Now - Function.sStartTime).TotalSeconds;
            return (int)ticks;
        }

        public void Register(UnixTimeModel data)
        {
            _unixDataList.Add(data);
        }

        public void Unregister(UnixTimeModel data)
        {
            if (_unixDataList.Contains(data))
            {
                _unixDataList.Remove(data);
            }
        }

        public override void Update(float delta)
        {
            for(int i = _unixDataList.Count - 1; i >= 0; --i)
            {
                _unixDataList[i].Update(delta);
            }
        }

        public override void Initialize()
        {
        }

        public override void Clear()
        {
            _unixDataList.Clear();
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.TimeDataManager;
        }
    }
}