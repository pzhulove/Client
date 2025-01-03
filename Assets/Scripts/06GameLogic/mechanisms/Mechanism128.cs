using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//增加不动冥王阵旋转的时间
class Mechanism128 : BeMechanism
{
    public int time;
    public int timeRate;
    public Mechanism128(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        time = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        timeRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }
}