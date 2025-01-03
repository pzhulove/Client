using System;
using System.Collections.Generic;
//巨大化机制
public class Mechanism2046 : BeMechanism
{
    private VFactor percent = VFactor.zero;
    private int totalTime = 0;
    private int curTime = 0;
    private VInt originalScale = VInt.one;
    private bool bStarted = false;
    public Mechanism2046(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        int percentVal = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        totalTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        percent = VFactor.NewVFactor(percentVal, GlobalLogic.VALUE_1000);
    }

    public override void OnReset()
    {
        curTime = 0;
        originalScale = VInt.one;
        bStarted = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        originalScale = owner.GetScale();
        bStarted = true;
        
    }
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (!bStarted) return;
        curTime += deltaTime;
        var factor = VFactor.NewVFactor(curTime, totalTime);
        if(factor >= VFactor.one)
        {
            factor = VFactor.one;
            bStarted = false;
        }
        var curPercent = factor * percent;
        var scale = originalScale.i * (VFactor.one + curPercent);
        owner.SetScale(scale);
    }
}

