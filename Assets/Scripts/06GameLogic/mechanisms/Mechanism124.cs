using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少尼尔狙击子弹数量
class Mechanism124 : BeMechanism
{
    public int bulletNum;
    public int addTime;

    public Mechanism124(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        bulletNum = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        addTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

}