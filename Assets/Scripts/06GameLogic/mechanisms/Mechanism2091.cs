using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 用于炫纹强压装备机制，根据炫纹强压炫纹数，增加技能攻击力(正相关)
/// </summary>
public class Mechanism2091 : BeMechanism
{
    private VRate m_SkillAttackAddRate = 0;

    public VRate SkillAttackAddRate
    {
        get { return m_SkillAttackAddRate; }
    }

    public Mechanism2091(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
        m_SkillAttackAddRate = new VRate(TableManager.GetValueFromUnionCell(data.ValueA[0], level));
    }
}

