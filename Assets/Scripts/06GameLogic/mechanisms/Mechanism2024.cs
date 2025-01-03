using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//胜利之矛装备机制
public class Mechanism2024: BeMechanism
{
    public Mechanism2024(int sid, int skillLevel) : base(sid, skillLevel){}

    public int[] addCountArr = new int[2];     //增加的胜利之矛的穿刺数量

    public override void OnInit()
    {
        base.OnInit();
        addCountArr[0] = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        addCountArr[1] = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
    }
}
