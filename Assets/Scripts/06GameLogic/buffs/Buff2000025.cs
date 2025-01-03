using UnityEngine;
using System.Collections.Generic;

//羽毛传递Buff

public class Buff2000025 : Buff2000020
{
    public Buff2000025(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    {

    }

    private int flashSummonId = 0;
    private int createEntityInterval = 1000;
    private int curCreateEntityTime = 0;

    public override void OnInit()
    {
        base.OnInit();
        flashSummonId = TableManager.GetValueFromUnionCell(buffData.ValueD[0], level);
        createEntityInterval = TableManager.GetValueFromUnionCell(buffData.ValueE[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        curCreateEntityTime = createEntityInterval;
    }

    public void SetEntityTotalTime(int time)
    {
        curCreateEntityTime = time;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if(flashSummonId != 0)
        {
            UpdateCreateEntity(deltaTime);
        }
    }

    /// <summary>
    /// 困难团本召唤实体
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateCreateEntity(int deltaTime)
    {
        if (curCreateEntityTime <= 0)
        {
            curCreateEntityTime = createEntityInterval;
            CreateBoomEntity(flashSummonId);
        }
        else
        {
            curCreateEntityTime -= deltaTime;
        }
    }

    /// <summary>
    /// 传递Buff
    /// </summary>
    protected override void PassBuff(BeActor target,int time)
    {
        var buff = target.buffController.TryAddBuff(2000025, time, level) as Buff2000025;
        if (buff != null)
        {
            passFlag = true;
            buff.SetTotalTime(curTotalTime);
            buff.SetEntityTotalTime(createEntityInterval);
        }
    }
}
