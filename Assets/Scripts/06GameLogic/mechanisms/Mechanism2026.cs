using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//守护徽章装备机制
public class Mechanism2026: BeMechanism
{
    public Mechanism2026(int sid, int skillLevel) : base(sid, skillLevel){}

    public VFactor singleHurtRateAdd = VFactor.zero;   //为单个队友分担伤害上限（千分比）

    public override void OnInit()
    {
        base.OnInit();
        singleHurtRateAdd = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
    }
}
