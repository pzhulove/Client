using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少闪电之舞攻击次数、攻击范围圈大小（千分比）
class Mechanism127 : BeMechanism
{
    public int attackNum;
    public int radiusRate;

    public Mechanism127(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        attackNum = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        radiusRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

}