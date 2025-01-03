using System;
using System.Collections.Generic;

public class Mechanism145 : BeMechanism
{
    enum BUFF_STAT
    {
        NONE,
        ADD,
        REMOVE
    }
    int maxMonsterCount = 0;
    int minMonsterCount = 0;
    List<int> buffInfoIds = new List<int>();
    int durTime = 0;
    readonly int interval = 500;
    BUFF_STAT mLastStat = BUFF_STAT.NONE;
    public Mechanism145(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        buffInfoIds.Clear();
    }
    public override void OnInit()
    {
        maxMonsterCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        minMonsterCount = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        for(int i  = 0; i < data.ValueC.Length;i++)
        {
            buffInfoIds.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
    }
    public override void OnStart()
    {
        durTime = 0;
        mLastStat = BUFF_STAT.NONE;
    }
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (owner == null) return;
        durTime += deltaTime;
        if (durTime < interval) return;
        durTime -= interval;
        int curMonsterCount = 0;
        var entities = owner.CurrentBeScene.GetEntities();
        for (int i = 0; i < entities.Count;i++)
        {
            var curActor = entities[i] as BeActor;
            if(curActor != null && !curActor.IsDead() && 
                curActor.IsMonster() && !curActor.IsSkillMonster() 
               /* &&!curActor.GetEntityData().isSummonMonster*/)
            {
                curMonsterCount++;
            }
        }
        BUFF_STAT state = BUFF_STAT.NONE;
        if(curMonsterCount >= maxMonsterCount)
        {
            state = BUFF_STAT.ADD;
        }
        else if(curMonsterCount <= minMonsterCount)
        {
            state = BUFF_STAT.REMOVE;
        }
        if (mLastStat == state) return;
        mLastStat = state;
        if (mLastStat == BUFF_STAT.ADD)
        {
            for(int i = 0; i < buffInfoIds.Count;i++)
            {
                owner.buffController.AddTriggerBuff(buffInfoIds[i]);
            }
        }
        else if (mLastStat == BUFF_STAT.REMOVE)
        {
            for(int i = 0; i < buffInfoIds.Count;i++)
            {
                owner.buffController.RemoveBuffByBuffInfoID(buffInfoIds[i]);
            }
        }
    }
}

