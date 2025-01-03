using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SimpleTimer2 {
    int timeAcc = 0;
    bool run = false;
    int seconds = 0;
	int countdown = 0;

	public delegate void SecondCallBack(int second);
	public delegate void TimeUpCallBack();

	public SecondCallBack secondCallBack;
	public TimeUpCallBack timeupCallBack;

    const int INTERVAL = 1000;

    public void StartTimer()
    {
		timeAcc = 0;
        seconds = 0;
        run = true;
    }

	public void SetCountdown(int cd)
	{
		countdown = cd;
	}

	public void Reset()
	{
		seconds = 0;
	}

    public void StopTimer()
    {
        run = false;
		secondCallBack = null;
		timeupCallBack = null;
    }

	public bool IsCount()
	{
		return run;
	}

    public void UpdateTimer(int delta)
    {
        if (!run)
            return;

        timeAcc += delta;
        if (timeAcc >= INTERVAL)
        {
            timeAcc -= INTERVAL;
			seconds++;
			if (secondCallBack != null)
				secondCallBack(seconds);
			if (IsTimeUp())
			{
				if (timeupCallBack != null)
					timeupCallBack();
				StopTimer();
			}
        }
    }

	public bool IsTimeUp()
	{
		if (countdown > 0)
			return seconds >= countdown;
		return false;
	}

}
