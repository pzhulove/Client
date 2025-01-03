using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少黑洞机制的吸附范围和吸附力（千分比）
class Mechanism117 : BeMechanism
{
    public int radiusRate;
    public int speedRate;

    public Mechanism117(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        radiusRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        speedRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

}