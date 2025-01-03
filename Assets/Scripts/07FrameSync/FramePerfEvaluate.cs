//Created Time : 2020-7-27
//Created By Shensi
//Description:
//用于评估是否需要追帧，分两个档，当需要执行的调用次数大于最大档位，直接开启追帧，而大于次大档位的次数超过阀值后也开启追帧
//当条件满足后，持续一段时间后，不满足条件了，就退出追帧
public class FrameCountPerformance
{
    private int _curTime = 0;       //当前时间累加
    private int _curCount = 0;      //当前累积次数
    private int _timeMax = 10;      //时间累积最大值
    private int _countMax;          //最大累积次数

    private int _addCount;          //每次增加数量
    private int _maxCall;
    private int _lastUpdateCount = 0;
    public FrameCountPerformance(int addCount, int countMax, int maxTime = 1000, int maxCall = 8)
    {
        _addCount = addCount;
        _countMax = countMax;
        _timeMax = maxTime;
        _maxCall = maxCall;
    }
    public void ReInit(int count, int maxCount, int maxTime, int maxCall)
    {
        _addCount = count;
        _countMax = maxCount;
        _timeMax = maxTime;
        _maxCall = maxCall;
    }
    public void Update(int deltaTime)
    {
        _curTime += deltaTime;
        if (_curTime >= _timeMax)
        {
            _curTime = 0;
            _curCount = 0;
        }
    }

    public void RefreshCurCount(int callCount)
    {
        _lastUpdateCount = callCount;
        if (_maxCall <= callCount)
        {
            _curTime = 0;
            _curCount = _countMax;
        }
        else if (_addCount < callCount)
        {
            _curTime = 0;
            _curCount++;
        }
        else
        {
            _curCount = 0;
            _curTime = 0;
        }
    }

    public bool IsFit()
    {
        return _curCount >= _countMax;
    }
    public void Reset()
    {
        _curTime = 0;
        _curCount = 0;
    }
    public string OutputString()
    {
        return BeUtility.Format("{0} {1} {2} {3} {4} {5}", _addCount, _countMax, _timeMax, _maxCall, _lastUpdateCount, _curTime);
    }
}
