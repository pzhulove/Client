using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 魔法护盾的伤害吸收率额外增加X%
/// </summary>
public class Mechanism2094 : BeMechanism
{
    private VFactor m_HpReplaceDelta = VFactor.zero;

    public VFactor HpReplaceDelta
    {
        get { return m_HpReplaceDelta; }
    }

    public Mechanism2094(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
        m_HpReplaceDelta = new VFactor(TableManager.GetValueFromUnionCell(data.ValueA[0], level));
    }
}

