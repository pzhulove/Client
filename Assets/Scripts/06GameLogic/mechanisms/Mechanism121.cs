using System;
using System.Collections.Generic;
using UnityEngine;

//紫阵范围加成
class Mechanism121 : BeMechanism
{
    public int radiusRate;

    public Mechanism121(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        radiusRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

}