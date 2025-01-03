using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少杀意波动机制的范围（千分比），攻击时间间隔（千分比）
class Mechanism116 : BeMechanism
{
    public int radiusRate;
    public bool isChangeEffectSize;
    public int intervalRate;
    public List<int> impactMechanismIdList = new List<int>();           //影响的机制ID列表

    public Mechanism116(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        radiusRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        isChangeEffectSize = TableManager.GetValueFromUnionCell(data.ValueB[0], level) == 1;
        intervalRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        if (data.ValueD.Count > 0)
            impactMechanismIdList.Add(TableManager.GetValueFromUnionCell(data.ValueD[0], level));
    }
    
}