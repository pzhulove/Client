using System;

namespace GameClient
{
    public interface ITimeModel
    {
        string GetRemainString();
        float GetRemainTime();
        void Update(float deltaTime);
    }

    public abstract class TimeModel : ITimeModel
    {
        public float RemainTime { get { return _remainTime; } }
        protected float _remainTime;


        public string GetRemainString()
        {
            return Function.GetShortTimeString(_remainTime);
        }

        public float GetRemainTime()
        {
            return _remainTime;
        }

        public abstract void Update(float delta);

        protected void UpdateRemainTime(float delta)
        {
            _remainTime -= delta;
            if (_remainTime <= 0)
            {
                _remainTime = 0;
            }
        }
    }

    public class UnixTimeModel : TimeModel,IDisposable
    {
        public ITimeData Data { get { return _progressTime; } }
        ITimeData _progressTime;

        public UnixTimeModel(ITimeData progressTime)
        {
            _progressTime = progressTime;
            _remainTime = TimeDataManager.GetInstance().CalcRemainTimeByUnix(progressTime);
            UpdateRemainTime(0f);
            TimeDataManager.GetInstance().Register(this);
        }

        public void Dispose()
        {
            TimeDataManager.GetInstance().Unregister(this);
        }

        public int GetDuration()
        {
            return TimeDataManager.GetInstance().CalcDuration(_progressTime);
        }

        public override void Update(float delta)
        {
            UpdateRemainTime(delta);
            if (_remainTime <= 0)
            {
                TimeDataManager.GetInstance().Unregister(this);
            }
        }
    }

    
    //public class GrowthTimeData : TimeData
    //{
    //    public int Value { get; private set; }
    //    public int Max { get; private set; }
    //    public int CdTime { get; private set; }

    //    public GrowthTimeData(msg.GrowthTimes time)
    //    {
    //        Value = time.value;
    //        Max = time.max;
    //        _remainTime = time.leftCdTime;
    //        CdTime = 300;
    //        TimeDataManager.Instance.Register(this);
    //    }

    //    public override void Update(float delta)
    //    {
    //        _remainTime -= delta;
    //        if (_remainTime <= 0)
    //        {
    //            if (Value < Max)
    //            {
    //                _remainTime = CdTime;
    //                Value++;
    //            }
    //        }

    //    }
    //}
}