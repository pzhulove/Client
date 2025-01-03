using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 在角色挂点创建特效机制 支持本地做偏移
/// </summary>
public class Mechanism1126 : BeMechanism
{
    public Mechanism1126(int mid, int lv) : base(mid, lv) { }

    protected string m_EffectPath = null;
    protected string m_AttchNodeName = null;
    protected Vector3 m_OffsetPos = Vector3.zero;

    protected GeEffectEx m_Effect = null;

    public override void OnInit()
    {
        base.OnInit();
        m_EffectPath = data.StringValueA[0];
        m_AttchNodeName = data.StringValueA[1];
        m_OffsetPos.x = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_OffsetPos.y = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        m_OffsetPos.z = TableManager.GetValueFromUnionCell(data.ValueA[2], level);
    }

    public override void OnReset()
    {
        m_Effect = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        CreateEffect();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RecyleEffect();
    }

    protected void CreateEffect()
    {
#if !LOGIC_SERVER
        if (owner.m_pkGeActor == null)
            return;
        m_Effect = owner.m_pkGeActor.CreateEffect(m_EffectPath,m_AttchNodeName,9999999999,Vec3.zero);
        m_OffsetPos.x /= 1000.0f;
        m_OffsetPos.y /= 1000.0f;
        m_OffsetPos.z /= 1000.0f;
        m_Effect.SetLocalPosition(m_OffsetPos);
#endif
    }

    protected void RecyleEffect()
    {
#if !LOGIC_SERVER
        if (m_Effect == null)
            return;
        m_Effect.Remove();
        m_Effect = null;
#endif
    }
}

