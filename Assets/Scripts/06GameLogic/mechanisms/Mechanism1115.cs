using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

public class CoolDown
{
    private bool m_IsStartCD = false;
    private int m_CurCDAcc = 0;
    private int m_CDTime = 0;
    
    public void StartCD(int time, bool immediatelyStart = false)
    {
        m_IsStartCD = true;
        if (immediatelyStart)
        {
            m_CurCDAcc = time + 1;
        }
        else
        {
            m_CurCDAcc = 0;
        }
        m_CDTime = time;
    }

    public void UpdateCD(int deltaTime)
    {
        if (m_IsStartCD)
        {
            m_CurCDAcc += deltaTime;
            if (m_CurCDAcc >= m_CDTime)
            {
                m_IsStartCD = false;
            }
        }
    }

    public bool IsCD()
    {
        return m_IsStartCD;
    }

    public void Clear()
    {
        m_IsStartCD = false;
        m_CurCDAcc = 0;
        m_CDTime = 0;
    }
}

/// <summary>
/// 当触发指定触发效果时，给对方加buffinfo，有CD时间(选填)
/// </summary>
public class Mechanism1115 : BeMechanism
{
    private CoolDown m_CD = new CoolDown();
    
    private HashSet<int> m_TargetHurtSet = new HashSet<int>();
    private int m_BuffInfoId = 0;
    private int m_BuffPercent = 1000;
    private int m_TriggerCD = 0;


    public Mechanism1115(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_TargetHurtSet.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        m_BuffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        if (data.ValueC.Count > 0)
        {
            m_BuffPercent = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }

        if (data.ValueD.Count > 0)
        {
            m_TriggerCD = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }
    }

    public override void OnReset()
    {
        m_CD.Clear();
        m_TargetHurtSet.Clear();
        m_BuffPercent = 1000;
        m_TriggerCD = 0;
    }

    public override void OnStart()
    {
        //handleA = owner.RegisterEvent(BeEventType.onBeforeHit, OnHit);
        handleA = owner.RegisterEventNew(BeEventType.onBeforeHit, OnHit);
    }

    private void OnHit(BeEvent.BeEventParam param)
    {
        if(m_TriggerCD > 0 && m_CD.IsCD())
            return;
        
        BeActor actor = param.m_Obj as BeActor;
        int hurtID = param.m_Int;
        if (actor != null && m_TargetHurtSet.Contains(hurtID))
        {
            //给攻击目标添加的Buff信息
            if (m_BuffInfoId != 0)
            {
                if (FrameRandom.Range1000() <= m_BuffPercent)
                {
                    if (actor.buffController.TryAddBuffInfo(m_BuffInfoId, owner, level) != null)
                    {
                        if (m_TriggerCD > 0)
                        { 
                            m_CD.StartCD(m_TriggerCD);
                        }
                    }
                }
            }
        }
    }


    public override void OnUpdate(int deltaTime)
    {
        if (m_TriggerCD > 0)
        {
            m_CD.UpdateCD(deltaTime);
        }
    }
}

