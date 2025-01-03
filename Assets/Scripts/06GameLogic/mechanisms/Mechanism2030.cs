using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//圣光守护装备机制 增加承担伤害上限
public class Mechanism2030: BeMechanism
{
    public Mechanism2030(int sid, int skillLevel) : base(sid, skillLevel){}

    public int hurtMaxLimitAddRate = 0;    //圣光守护承担伤害上限增加(千分比)

    public override void OnInit()
    {
        base.OnInit();

        hurtMaxLimitAddRate = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
    }
}
