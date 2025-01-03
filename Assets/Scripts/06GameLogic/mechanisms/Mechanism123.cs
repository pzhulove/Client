using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少血之狂暴每10秒消耗的HP量（千分比）
class Mechanism123 : BeMechanism
{
    public int hpRate;

    public Mechanism123(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        hpRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

}