using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 念气环绕机制
 */

public class Mechanism37 : BeMechanism
{
    protected VInt m_Radius = VInt.one.i * 2;            //伤害范围半径
    protected int m_HurtID = 0;                 //伤害Id

    protected int m_TimeAcc = 0;
    readonly protected int m_CheckInterval = 200;
    protected int m_TimeAcc2 = 0;
    protected int m_Hurtm_CheckInterval = 1000;
    protected List<BeActor> inRangers = new List<BeActor>();

    public Mechanism37(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        m_TimeAcc = 0;
        m_TimeAcc2 = 0;
        inRangers.Clear();
    }

    public override void OnInit()
    {
        m_Radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level),1000);
        m_HurtID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_Hurtm_CheckInterval = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnUpdate(int deltaTime)
    {
        UpdateCheckRange(deltaTime);
        UpdateCheckHurt(deltaTime);
    }

    protected void UpdateCheckHurt(int deltaTime)
    {
        m_TimeAcc2 += deltaTime;
        if (m_TimeAcc2 > m_Hurtm_CheckInterval)
        {
            m_TimeAcc2 -= m_Hurtm_CheckInterval;
            for (int i = 0; i < inRangers.Count; ++i)
            {
                var actor = inRangers[i];
                if (!actor.IsDead() && actor.GetCamp() != owner.GetCamp() && actor.GetLifeState() == (int)EntityLifeState.ELS_ALIVE)
                    owner.DoAttackTo(actor, m_HurtID, false);
            }
        }
    }

    protected void UpdateCheckRange(int deltaTime)
    {
        m_TimeAcc += deltaTime;
        if (m_TimeAcc > m_CheckInterval)
        {
            m_TimeAcc -= m_CheckInterval;

            CheckRange();
        }
    }

    protected void CheckRange()
    {
        List<BeActor> targets = GamePool.ListPool<BeActor>.Get();

        var pos = owner.GetPosition();
        pos.z = 0;
        owner.CurrentBeScene.FindActorInRange(targets, pos, m_Radius);

        for (int i = 0; i < inRangers.Count; ++i)
        {
            if (!targets.Contains(inRangers[i]))
            {
                inRangers.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < targets.Count; ++i)
        {
            if (!inRangers.Contains(targets[i]))
            {
                inRangers.Add(targets[i]);
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);
    }

    public override void OnFinish()
    {
        inRangers.Clear();
    }
}