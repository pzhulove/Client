using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少枪手特殊子弹数量
class Mechanism126 : BeMechanism
{
    public int bulletType;
    public int bulletNum;

    public Mechanism126(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        bulletType = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        bulletNum = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

}