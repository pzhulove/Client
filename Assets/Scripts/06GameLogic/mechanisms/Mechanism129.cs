using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//修改不动冥王阵的球的半径
class Mechanism129 : BeMechanism
{
    public VInt dis;
    public int disRate;
    public Mechanism129(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        dis = new VInt( TableManager.GetValueFromUnionCell(data.ValueA[0], level)/1000.0f);
        disRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }
}