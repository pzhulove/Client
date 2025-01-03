using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少某个特效的大小X、Y、Z轴分开（千分比）
class Mechanism122 : BeMechanism
{
    public List<string> effectNameList = new List<string>();
    public VFactor xRate;
    public VFactor yRate;
    public VFactor zRate;

    public Mechanism122(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        effectNameList.Clear();
        
    }
    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Length; i++)
        {
            var name = TableManager.GetValueFromUnionCell(data.ValueA[i], level).ToString();
            effectNameList.Add(name);
        }
        xRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
        yRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
        zRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueD[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnFinish()
    {
        effectNameList.Clear();
    }
}
