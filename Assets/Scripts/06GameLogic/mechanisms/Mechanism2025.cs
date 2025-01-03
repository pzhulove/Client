using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//忏悔之锤装备机制
public class Mechanism2025: BeMechanism
{
    public Mechanism2025(int sid, int skillLevel) : base(sid, skillLevel){}

    public int addRate = 0;     //增加的忏悔概率
    public int addBuffTime = 0;     //增加的Buff时间

    public override void OnInit()
    {
        base.OnInit();
        addRate = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        addBuffTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }
}
